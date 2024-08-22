using SafeEscape.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SafeEscape.Interfaces
{
    public interface IRoleService
    {
        Task<IEnumerable<Role>> GetAllAsync();
        Task<Role> GetByIdAsync(Guid id);
        Task CreateAsync(Role role);
        Task UpdateAsync(Guid id, Role role);
        Task DeleteAsync(Guid id);
    }
}
