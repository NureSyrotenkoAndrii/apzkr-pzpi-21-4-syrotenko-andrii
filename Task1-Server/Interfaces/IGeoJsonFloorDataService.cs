using SafeEscape.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SafeEscape.Interfaces
{
    public interface IGeoJsonFloorDataService
    {
        Task<IEnumerable<GeoJsonFloorData>> GetAllAsync();
        Task<GeoJsonFloorData> GetByIdAsync(Guid id);
        Task CreateAsync(GeoJsonFloorData geoJsonFloorData);
        Task UpdateAsync(Guid id, GeoJsonFloorData geoJsonFloorData);
        Task DeleteAsync(Guid id);
        Task<Guid?> GetBuildingIdByGeoIdAsync(Guid floorId);
    }
}
