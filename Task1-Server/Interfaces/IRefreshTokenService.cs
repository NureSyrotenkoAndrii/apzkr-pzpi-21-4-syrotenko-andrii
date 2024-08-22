using SafeEscape.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SafeEscape.Interfaces
{
    public interface IRefreshTokenService
    {
        Task<IEnumerable<RefreshToken>> GetAllAsync();
        Task<RefreshToken> GetByIdAsync(Guid id);
        Task CreateAsync(RefreshToken refreshToken);
        Task UpdateAsync(Guid id, RefreshToken refreshToken);
        Task DeleteAsync(Guid id);
    }
}
