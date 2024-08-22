using SafeEscape.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SafeEscape.Interfaces
{
    public interface IFloorService
    {
        Task<IEnumerable<Floor>> GetAllAsync();
        Task<Floor> GetByIdAsync(Guid id);
        Task CreateAsync(Floor floor);
        Task UpdateAsync(Guid id, Floor floor);
        Task DeleteAsync(Guid id);
        Task<double> GetSmokeStatisticsAsync(Guid floorId);
        Task<IEnumerable<Room>> GetRoomsByFloorIdAsync(Guid floorId);
        Task<Guid?> GetBuildingIdByFloorIdAsync(Guid floorId);
    }
}
