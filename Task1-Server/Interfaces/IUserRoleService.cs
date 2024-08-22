using SafeEscape.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SafeEscape.Interfaces
{
    public interface IUserRoleService
    {
        Task<IEnumerable<UserRole>> GetAllAsync();
        Task<UserRole> GetByIdAsync(Guid userId, Guid roleId);
        Task CreateAsync(UserRole userRole);
        Task DeleteAsync(Guid userId, Guid roleId);
    }
}
