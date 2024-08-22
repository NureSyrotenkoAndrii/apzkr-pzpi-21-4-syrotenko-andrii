using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SafeEscape.Interfaces;
using SafeEscape.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace SafeEscape.Services
{
    public class WebSocketService : IWebSocketService
    {
        // ConcurrentDictionary to hold WebSocket connections with user IDs as keys.
        private readonly ConcurrentDictionary<Guid, WebSocket> _sockets = new();

        // Adds a WebSocket connection for a specific user ID to the dictionary.
        public void AddSocket(Guid userId, WebSocket socket)
        {
            _sockets.TryAdd(userId, socket);
        }

        // Removes a WebSocket connection for a specific user ID from the dictionary.
        public void RemoveSocket(Guid userId)
        {
            _sockets.TryRemove(userId, out _);
        }

        // Sends a message to the WebSocket connection associated with the given user ID.
        public async Task SendMessageAsync(Guid userId, string message)
        {
            if (_sockets.TryGetValue(userId, out var socket) && socket.State == WebSocketState.Open)
            {
                var buffer = Encoding.UTF8.GetBytes(message);
                var segment = new ArraySegment<byte>(buffer);

                await socket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}
