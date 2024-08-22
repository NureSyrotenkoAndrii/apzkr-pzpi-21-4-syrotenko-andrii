using SafeEscape.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SafeEscape.Interfaces
{
    public interface IRoomService
    {
        Task<IEnumerable<Room>> GetAllAsync();
        Task<Room> GetByIdAsync(Guid id);
        Task CreateAsync(Room room);
        Task UpdateAsync(Guid id, Room room);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<Room>> GetRoomsByBuildingIdAsync(Guid buildingId);
        Task<double> GetSmokeStatisticsAsync(Guid roomId);
        Task<Guid?> GetBuildingIdByRoomIdAsync(Guid roomId);
    }
}
