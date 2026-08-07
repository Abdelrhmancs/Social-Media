using Application.DTOs.ChatDTOs;
using Domain.Validation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IConversationRepository
    {
        Task<ResultT<ConversationDto>> CreateDirectConversationAsync(string currentUserId, string targetUserId);
        Task<ResultT<ConversationDto>> CreateGroupConversationAsync(string currentUserId, CreateGroupConversationDto dto);
        Task<ResultT<List<ConversationDto>>> GetUserConversationsAsync(string userId);
        Task<bool> IsUserMemberAsync(long conversationId, string userId);
        Task<Result> AddMemberAsync(long conversationId, string userId, string requesterId);
        Task<Result> RemoveMemberAsync(long conversationId, string userId, string requesterId);
        Task<List<long>> GetUserConversationIdsAsync(string userId);
    }
}
