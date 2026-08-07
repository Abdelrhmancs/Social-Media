using Application.DTOs.ChatDTOs;
using Domain.Validation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IMessageRepository
    {
        Task<ResultT<MessageDto>> SendMessageAsync(string senderId, SendMessageDto dto);
        Task<ResultT<List<MessageDto>>> GetConversationMessagesAsync(long conversationId, string userId, int page, int pageSize);
        Task<Result> MarkMessageAsReadAsync(long messageId, string userId);
        Task<ResultT<List<ReadReceiptDto>>> GetMessageReadReceiptsAsync(long messageId, string requesterId);
    }
}
