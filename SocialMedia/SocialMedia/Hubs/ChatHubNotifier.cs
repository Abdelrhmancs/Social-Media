using Application.DTOs.ChatDTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Hubs
{
    public class ChatHubNotifier : IChatHubNotifier
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatHubNotifier(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendMessageToConversationAsync(long conversationId, MessageDto message)
        {
            await _hubContext.Clients.Group($"Conv_{conversationId}").SendAsync("ReceiveMessage", message);
        }

        public async Task NotifyTypingAsync(long conversationId, string userId, string userName, bool isTyping)
        {
            var eventName = isTyping ? "UserTyping" : "UserStoppedTyping";
            var payload = isTyping 
                ? (object)new { ConversationId = conversationId, UserId = userId, UserName = userName }
                : new { ConversationId = conversationId, UserId = userId };

            await _hubContext.Clients.Group($"Conv_{conversationId}").SendAsync(eventName, payload);
        }

        public async Task NotifyMessageReadAsync(long conversationId, long messageId, ReadReceiptDto receipt)
        {
            await _hubContext.Clients.Group($"Conv_{conversationId}").SendAsync("MessageReadBy", new { MessageId = messageId, Receipt = receipt });
        }

        public async Task NotifyUserOnlineStatusAsync(IEnumerable<string> userIds, string changedUserId, bool isOnline)
        {
            var eventName = isOnline ? "UserOnline" : "UserOffline";
            // This could be optimized to target specific users, but for now we broadcast globally
            // as handled in OnConnectedAsync/OnDisconnectedAsync directly.
            // This method allows the server to trigger it if needed.
            await _hubContext.Clients.All.SendAsync(eventName, new { UserId = changedUserId });
        }
    }
}
