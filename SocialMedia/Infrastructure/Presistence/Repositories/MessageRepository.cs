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
    public class MessageRepository : IMessageRepository
    {
        private readonly SocialMediaContext _context;

        public MessageRepository(SocialMediaContext context)
        {
            _context = context;
        }

        public async Task<ResultT<MessageDto>> SendMessageAsync(string senderId, SendMessageDto dto)
        {
            var message = new Message
            {
                ConversationId = dto.ConversationId,
                SenderId = senderId,
                Content = dto.Content,
                Type = dto.Type,
                SentAt = DateTime.Now,
                IsDeleted = false
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            // Fetch the inserted message with Sender to build DTO
            var insertedMessage = await _context.Messages
                .Include(m => m.Sender)
                .FirstOrDefaultAsync(m => m.Id == message.Id);

            var resultDto = new MessageDto
            {
                Id = insertedMessage!.Id,
                ConversationId = insertedMessage.ConversationId,
                SenderId = insertedMessage.SenderId,
                SenderName = insertedMessage.Sender.Name,
                SenderPic = insertedMessage.Sender.Pic,
                Content = insertedMessage.Content,
                Type = insertedMessage.Type,
                SentAt = insertedMessage.SentAt,
                IsDeleted = insertedMessage.IsDeleted,
                ReadReceipts = new List<ReadReceiptDto>() // Initial message has no receipts
            };

            return ResultT<MessageDto>.success(resultDto);
        }

        public async Task<ResultT<List<MessageDto>>> GetConversationMessagesAsync(long conversationId, string userId, int page, int pageSize)
        {
            var messagesQuery = _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.ReadReceipts)
                    .ThenInclude(r => r.User)
                .Where(m => m.ConversationId == conversationId)
                .OrderByDescending(m => m.SentAt) // Newest first for pagination
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            var messages = await messagesQuery.ToListAsync();

            // Map to DTOs
            var dtos = messages.Select(m => new MessageDto
            {
                Id = m.Id,
                ConversationId = m.ConversationId,
                SenderId = m.SenderId,
                SenderName = m.Sender.Name,
                SenderPic = m.Sender.Pic,
                Content = m.IsDeleted ? "This message was deleted" : m.Content,
                Type = m.Type,
                SentAt = m.SentAt,
                IsDeleted = m.IsDeleted,
                ReadReceipts = m.ReadReceipts.Select(r => new ReadReceiptDto
                {
                    UserId = r.UserId,
                    UserName = r.User.Name,
                    UserPic = r.User.Pic,
                    ReadAt = r.ReadAt
                }).ToList()
            }).ToList();

            // Sort back to chronological order (oldest to newest) before returning
            dtos.Reverse();

            return ResultT<List<MessageDto>>.success(dtos);
        }

        public async Task<Result> MarkMessageAsReadAsync(long messageId, string userId)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if (message == null)
                return Result.Failure(new List<string> { "Message not found" });

            var existingReceipt = await _context.Set<MessageReadReceipt>()
                .FirstOrDefaultAsync(r => r.MessageId == messageId && r.UserId == userId);

            if (existingReceipt != null)
                return Result.success("Message already marked as read"); // Idempotent

            var receipt = new MessageReadReceipt
            {
                MessageId = messageId,
                UserId = userId,
                ReadAt = DateTime.Now
            };

            _context.Set<MessageReadReceipt>().Add(receipt);
            await _context.SaveChangesAsync();

            return Result.success("Message marked as read");
        }

        public async Task<ResultT<List<ReadReceiptDto>>> GetMessageReadReceiptsAsync(long messageId, string requesterId)
        {
            var receipts = await _context.Set<MessageReadReceipt>()
                .Include(r => r.User)
                .Where(r => r.MessageId == messageId)
                .Select(r => new ReadReceiptDto
                {
                    UserId = r.UserId,
                    UserName = r.User.Name,
                    UserPic = r.User.Pic,
                    ReadAt = r.ReadAt
                })
                .ToListAsync();

            return ResultT<List<ReadReceiptDto>>.success(receipts);
        }
    }
}
