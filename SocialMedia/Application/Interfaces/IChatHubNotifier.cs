using Application.DTOs.ChatDTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IChatHubNotifier
    {
        Task SendMessageToConversationAsync(long conversationId, MessageDto message);
        Task NotifyTypingAsync(long conversationId, string userId, string userName, bool isTyping);
        Task NotifyMessageReadAsync(long conversationId, long messageId, ReadReceiptDto receipt);
        Task NotifyUserOnlineStatusAsync(IEnumerable<string> userIds, string changedUserId, bool isOnline);
    }
}
