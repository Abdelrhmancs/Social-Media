using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IOnlineUserTracker
    {
        Task UserConnectedAsync(string userId, string connectionId);
        Task UserDisconnectedAsync(string connectionId);
        Task<bool> IsOnlineAsync(string userId);
        Task<IEnumerable<string>> GetOnlineUsersAsync();
        Task<string?> GetUserIdByConnectionAsync(string connectionId);
    }
}
