using Application.DTOs.ChatDTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IOnlineUserTracker _onlineTracker;
        private readonly IConversationRepository _conversationRepo;

        public ChatHub(IOnlineUserTracker onlineTracker, IConversationRepository conversationRepo)
        {
            _onlineTracker = onlineTracker;
            _conversationRepo = conversationRepo;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                await _onlineTracker.UserConnectedAsync(userId, Context.ConnectionId);

                // Join SignalR groups for all conversations the user is part of
                var conversationIds = await _conversationRepo.GetUserConversationIdsAsync(userId);
                foreach (var convId in conversationIds)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"Conv_{convId}");
                }

                // Notify others that this user is online
                await Clients.Others.SendAsync("UserOnline", new { UserId = userId });
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                await _onlineTracker.UserDisconnectedAsync(Context.ConnectionId);
                
                // If user has no other active connections, broadcast offline
                var isStillOnline = await _onlineTracker.IsOnlineAsync(userId);
                if (!isStillOnline)
                {
                    await Clients.Others.SendAsync("UserOffline", new { UserId = userId });
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task StartTyping(long conversationId)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = Context.User?.FindFirst(ClaimTypes.Name)?.Value ?? "User";
            
            if (!string.IsNullOrEmpty(userId))
            {
                var isMember = await _conversationRepo.IsUserMemberAsync(conversationId, userId);
                if (isMember)
                {
                    await Clients.GroupExcept($"Conv_{conversationId}", Context.ConnectionId)
                        .SendAsync("UserTyping", new { ConversationId = conversationId, UserId = userId, UserName = userName });
                }
            }
        }

        public async Task StopTyping(long conversationId)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                var isMember = await _conversationRepo.IsUserMemberAsync(conversationId, userId);
                if (isMember)
                {
                    await Clients.GroupExcept($"Conv_{conversationId}", Context.ConnectionId)
                        .SendAsync("UserStoppedTyping", new { ConversationId = conversationId, UserId = userId });
                }
            }
        }
    }
}
