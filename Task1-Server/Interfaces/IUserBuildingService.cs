using SafeEscape.Models;
using SafeEscape.Models.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SafeEscape.Interfaces
{
    public interface IUserBuildingService
    {
        Task<IEnumerable<UserBuilding>> GetAllAsync();
        Task<UserBuilding> GetByIdAsync(Guid userId, Guid buildingId);
        Task CreateAsync(UserBuilding userBuilding);
        Task UpdateAsync(Guid userId, Guid buildingId, UserBuilding userBuilding);
        Task DeleteAsync(Guid userId, Guid buildingId);
        Task AddUserToBuildingAsync(AddUserBuildingDto dto);
        Task<bool> IsBuildingManagerAsync(Guid userId, Guid buildingId);
    }
}
