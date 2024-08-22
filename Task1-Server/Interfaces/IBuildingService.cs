using SafeEscape.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SafeEscape.Interfaces
{
    public interface IBuildingService
    {
        Task<IEnumerable<Building>> GetAllAsync();
        Task<Building> GetByIdAsync(Guid id);
        Task CreateAsync(Building building, Guid userId);
        Task UpdateAsync(Guid id, Building building);
        Task DeleteAsync(Guid id);
        Task<double> GetSmokeStatisticsAsync(Guid buildingId);
        Task<IEnumerable<User>> GetUsersByBuildingIdAsync(Guid buildingId);
        Task<IEnumerable<Floor>> GetFloorsByBuildingIdAsync(Guid buildingId);
    }
}
