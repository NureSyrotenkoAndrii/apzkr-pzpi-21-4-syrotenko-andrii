using SafeEscape.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SafeEscape.Interfaces
{
    public interface IStairService
    {
        Task<IEnumerable<Stair>> GetAllAsync();
        Task<Stair> GetByIdAsync(Guid id);
        Task CreateAsync(Stair stair);
        Task UpdateAsync(Guid id, Stair stair);
        Task DeleteAsync(Guid id);
        Task<Guid?> GetBuildingIdByStairIdAsync(Guid stairId);
    }
}
