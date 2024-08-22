using SafeEscape.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SafeEscape.Interfaces
{
    public interface IExitService
    {
        Task<IEnumerable<Exit>> GetAllAsync();
        Task<Exit> GetByIdAsync(Guid id);
        Task CreateAsync(Exit exit);
        Task UpdateAsync(Guid id, Exit exit);
        Task DeleteAsync(Guid id);
        Task<Guid?> GetBuildingIdByExitIdAsync(Guid exitId);
    }
}
