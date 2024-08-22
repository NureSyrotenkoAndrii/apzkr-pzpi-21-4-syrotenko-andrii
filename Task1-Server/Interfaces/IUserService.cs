using SafeEscape.Models;
using SafeEscape.Models.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SafeEscape.Interfaces
{
    public interface IUserService
    {
        Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest model);
        Task<IEnumerable<User>> GetAllAsync();
        Task RegisterAsync(RegisterRequest model);
        Task<AuthenticateResponse> RefreshTokenAsync(string token);
        Task RevokeTokenAsync(string token);
        Task<User> GetByIdAsync(Guid id);
        Task CreateAsync(User user);
        Task UpdateAsync(Guid id, User user);
        Task DeleteAsync(Guid id);
        Task BanUserAsync(Guid id);
        Task UnbanUserAsync(Guid id);
        Task UpdateUserAsync(Guid id, UserDto userDto);
        Task ChangePasswordAsync(Guid id, ChangePasswordDto passwordDto);
        Task<bool> IsUserInRoleAsync(Guid userId, string roleName);
    }
}
