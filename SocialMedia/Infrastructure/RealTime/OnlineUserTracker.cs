using Application.Interfaces;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.RealTime
{
    public class OnlineUserTracker : IOnlineUserTracker
    {
        // Maps ConnectionId -> UserId
        private readonly ConcurrentDictionary<string, string> _onlineUsers = new();

        public Task UserConnectedAsync(string userId, string connectionId)
        {
            _onlineUsers.AddOrUpdate(connectionId, userId, (key, oldValue) => userId);
            return Task.CompletedTask;
        }

        public Task UserDisconnectedAsync(string connectionId)
        {
            _onlineUsers.TryRemove(connectionId, out _);
            return Task.CompletedTask;
        }

        public Task<bool> IsOnlineAsync(string userId)
        {
            var isOnline = _onlineUsers.Values.Contains(userId);
            return Task.FromResult(isOnline);
        }

        public Task<IEnumerable<string>> GetOnlineUsersAsync()
        {
            var users = _onlineUsers.Values.Distinct();
            return Task.FromResult(users);
        }

        public Task<string?> GetUserIdByConnectionAsync(string connectionId)
        {
            _onlineUsers.TryGetValue(connectionId, out var userId);
            return Task.FromResult(userId);
        }
    }
}
