using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace SafeEscape.Interfaces
{
    public interface IWebSocketService
    {
        void AddSocket(Guid userId, WebSocket socket);
        void RemoveSocket(Guid userId);
        Task SendMessageAsync(Guid userId, string message);
    }
}
