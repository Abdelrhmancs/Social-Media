using API.Domain.Entites;
using Application.DTOs.ChatDTOs;
using Application.Interfaces;
using Domain.Validation;
using Infrastructure.Presistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Presistence.Repositories
{
    public class ConversationRepository : IConversationRepository
    {
        private readonly SocialMediaContext _context;
        private readonly IOnlineUserTracker _onlineTracker;

        public ConversationRepository(SocialMediaContext context, IOnlineUserTracker onlineTracker)
        {
            _context = context;
            _onlineTracker = onlineTracker;
        }

        public async Task<ResultT<ConversationDto>> CreateDirectConversationAsync(string currentUserId, string targetUserId)
        {
            var targetUser = await _context.Users.FindAsync(targetUserId);
            if (targetUser == null)
                return ResultT<ConversationDto>.Failure(new List<string> { "Target user not found" }, ErrorType.NotFound);

            // Check if direct conversation already exists between these two users
            var existingConvId = await _context.Conversations
                .Where(c => c.Type == ConversationType.Direct)
                .Where(c => c.Members.Any(m => m.UserId == currentUserId) && c.Members.Any(m => m.UserId == targetUserId))
                .Select(c => c.Id)
                .FirstOrDefaultAsync();

            if (existingConvId > 0)
            {
                var existingConv = await GetConversationDtoByIdAsync(existingConvId, currentUserId);
                return ResultT<ConversationDto>.success(existingConv!);
            }

            var conversation = new Conversation
            {
                Type = ConversationType.Direct,
                CreatedAt = DateTime.Now,
                Members = new List<ConversationMember>
                {
                    new ConversationMember { UserId = currentUserId, Role = MemberRole.Member, JoinedAt = DateTime.Now },
                    new ConversationMember { UserId = targetUserId, Role = MemberRole.Member, JoinedAt = DateTime.Now }
                }
            };

            _context.Conversations.Add(conversation);
            await _context.SaveChangesAsync();

            var dto = await GetConversationDtoByIdAsync(conversation.Id, currentUserId);
            return ResultT<ConversationDto>.success(dto!);
        }

        public async Task<ResultT<ConversationDto>> CreateGroupConversationAsync(string currentUserId, CreateGroupConversationDto dto)
        {
            var members = new List<ConversationMember>
            {
                new ConversationMember { UserId = currentUserId, Role = MemberRole.Admin, JoinedAt = DateTime.Now }
            };

            foreach (var memberId in dto.MemberIds.Distinct())
            {
                if (memberId != currentUserId)
                {
                    var userExists = await _context.Users.AnyAsync(u => u.Id == memberId);
                    if (userExists)
                    {
                        members.Add(new ConversationMember { UserId = memberId, Role = MemberRole.Member, JoinedAt = DateTime.Now });
                    }
                }
            }

            var conversation = new Conversation
            {
                Type = ConversationType.Group,
                Name = dto.Name,
                GroupPicture = dto.GroupPicture,
                CreatedAt = DateTime.Now,
                Members = members
            };

            _context.Conversations.Add(conversation);
            await _context.SaveChangesAsync();

            var resultDto = await GetConversationDtoByIdAsync(conversation.Id, currentUserId);
            return ResultT<ConversationDto>.success(resultDto!);
        }

        public async Task<ResultT<List<ConversationDto>>> GetUserConversationsAsync(string userId)
        {
            var conversationIds = await _context.Conversations
                .Where(c => c.Members.Any(m => m.UserId == userId))
                .Select(c => c.Id)
                .ToListAsync();

            var dtos = new List<ConversationDto>();
            foreach (var id in conversationIds)
            {
                var dto = await GetConversationDtoByIdAsync(id, userId);
                if (dto != null) dtos.Add(dto);
            }

            // Sort by latest message, then creation date
            var sortedDtos = dtos.OrderByDescending(c => c.LastMessage?.SentAt ?? c.CreatedAt).ToList();

            return ResultT<List<ConversationDto>>.success(sortedDtos);
        }

        public async Task<bool> IsUserMemberAsync(long conversationId, string userId)
        {
            return await _context.Conversations
                .AnyAsync(c => c.Id == conversationId && c.Members.Any(m => m.UserId == userId));
        }

        public async Task<Result> AddMemberAsync(long conversationId, string userIdToAdd, string requesterId)
        {
            var conversation = await _context.Conversations
                .Include(c => c.Members)
                .FirstOrDefaultAsync(c => c.Id == conversationId);

            if (conversation == null)
                return Result.Failure(new List<string> { "Conversation not found" });

            if (conversation.Type == ConversationType.Direct)
                return Result.Failure(new List<string> { "Cannot add members to a direct conversation" });

            var requester = conversation.Members.FirstOrDefault(m => m.UserId == requesterId);
            if (requester == null || requester.Role != MemberRole.Admin)
                return Result.Failure(new List<string> { "Only admins can add members" });

            if (conversation.Members.Any(m => m.UserId == userIdToAdd))
                return Result.Failure(new List<string> { "User is already a member" });

            var userToAdd = await _context.Users.FindAsync(userIdToAdd);
            if (userToAdd == null)
                return Result.Failure(new List<string> { "User to add not found" });

            conversation.Members.Add(new ConversationMember
            {
                UserId = userIdToAdd,
                Role = MemberRole.Member,
                JoinedAt = DateTime.Now
            });

            await _context.SaveChangesAsync();
            return Result.success("Member added successfully");
        }

        public async Task<Result> RemoveMemberAsync(long conversationId, string userIdToRemove, string requesterId)
        {
            var conversation = await _context.Conversations
                .Include(c => c.Members)
                .FirstOrDefaultAsync(c => c.Id == conversationId);

            if (conversation == null)
                return Result.Failure(new List<string> { "Conversation not found" });

            if (conversation.Type == ConversationType.Direct)
                return Result.Failure(new List<string> { "Cannot remove members from a direct conversation" });

            var requester = conversation.Members.FirstOrDefault(m => m.UserId == requesterId);
            if (requester == null)
                return Result.Failure(new List<string> { "You are not a member of this conversation" });

            // Only admin can remove others, but anyone can remove themselves
            if (requesterId != userIdToRemove && requester.Role != MemberRole.Admin)
                return Result.Failure(new List<string> { "Only admins can remove other members" });

            var memberToRemove = conversation.Members.FirstOrDefault(m => m.UserId == userIdToRemove);
            if (memberToRemove == null)
                return Result.Failure(new List<string> { "User is not a member of this conversation" });

            conversation.Members.Remove(memberToRemove);
            await _context.SaveChangesAsync();

            return Result.success("Member removed successfully");
        }

        public async Task<List<long>> GetUserConversationIdsAsync(string userId)
        {
            return await _context.Conversations
                .Where(c => c.Members.Any(m => m.UserId == userId))
                .Select(c => c.Id)
                .ToListAsync();
        }

        private async Task<ConversationDto?> GetConversationDtoByIdAsync(long conversationId, string currentUserId)
        {
            var conversation = await _context.Conversations
                .Include(c => c.Members)
                    .ThenInclude(m => m.User)
                .FirstOrDefaultAsync(c => c.Id == conversationId);

            if (conversation == null) return null;

            var lastMessageEntity = await _context.Messages
                .Include(m => m.Sender)
                .Where(m => m.ConversationId == conversationId && !m.IsDeleted)
                .OrderByDescending(m => m.SentAt)
                .FirstOrDefaultAsync();

            var dto = new ConversationDto
            {
                Id = conversation.Id,
                Type = conversation.Type,
                Name = conversation.Name,
                GroupPicture = conversation.GroupPicture,
                CreatedAt = conversation.CreatedAt,
                Members = new List<ConversationMemberDto>()
            };

            // If it's a direct conversation, use the other user's name/pic as the conversation name/pic
            if (conversation.Type == ConversationType.Direct)
            {
                var otherMember = conversation.Members.FirstOrDefault(m => m.UserId != currentUserId);
                if (otherMember != null)
                {
                    dto.Name = otherMember.User.Name;
                    dto.GroupPicture = otherMember.User.Pic;
                }
            }

            foreach (var member in conversation.Members)
            {
                var isOnline = await _onlineTracker.IsOnlineAsync(member.UserId);
                dto.Members.Add(new ConversationMemberDto
                {
                    UserId = member.UserId,
                    UserName = member.User.Name,
                    UserPic = member.User.Pic,
                    Role = member.Role,
                    JoinedAt = member.JoinedAt,
                    IsOnline = isOnline
                });
            }

            if (lastMessageEntity != null)
            {
                dto.LastMessage = new MessageDto
                {
                    Id = lastMessageEntity.Id,
                    ConversationId = lastMessageEntity.ConversationId,
                    Content = lastMessageEntity.Content,
                    Type = lastMessageEntity.Type,
                    SentAt = lastMessageEntity.SentAt,
                    SenderId = lastMessageEntity.SenderId,
                    SenderName = lastMessageEntity.Sender.Name,
                    SenderPic = lastMessageEntity.Sender.Pic,
                    IsDeleted = lastMessageEntity.IsDeleted
                };
            }

            return dto;
        }
    }
}
