using SafeEscape.Models;
using SafeEscape.Models.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SafeEscape.Interfaces
{
    public interface IRoomEdgeService
    {
        Task<IEnumerable<RoomEdge>> GetAllAsync();
        Task<RoomEdge> GetByIdAsync(Guid id);
        Task CreateAsync(RoomEdge roomEdge);
        Task UpdateAsync(Guid id, RoomEdge roomEdge);
        Task DeleteAsync(Guid id);
        Task<EvacuationRouteResponseDto> GetEvacuationRouteAsync(Guid startRoomId);
    }
}
