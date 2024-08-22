using Microsoft.EntityFrameworkCore;
using SafeEscape.Interfaces;
using SafeEscape.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SafeEscape.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly ApplicationDbContext _context;

        // Constructor to inject the database context.
        public RefreshTokenService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Retrieves all refresh tokens from the database.
        public async Task<IEnumerable<RefreshToken>> GetAllAsync()
        {
            return await _context.RefreshTokens.ToListAsync();
        }

        // Retrieves a specific refresh token by its ID.
        public async Task<RefreshToken> GetByIdAsync(Guid id)
        {
            return await _context.RefreshTokens.FindAsync(id);
        }

        // Creates a new refresh token record if the associated user exists.
        public async Task CreateAsync(RefreshToken refreshToken)
        {
            // Ensure the associated user exists before creating the refresh token.
            if (!await _context.Users.AnyAsync(u => u.Id == refreshToken.UserId))
            {
                throw new KeyNotFoundException("User not found");
            }

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
        }

        // Updates an existing refresh token record.
        public async Task UpdateAsync(Guid id, RefreshToken refreshToken)
        {
            var existingRefreshToken = await _context.RefreshTokens.FindAsync(id);
            if (existingRefreshToken == null)
            {
                throw new KeyNotFoundException("RefreshToken not found");
            }

            // Ensure the associated user exists before updating the refresh token.
            if (!await _context.Users.AnyAsync(u => u.Id == refreshToken.UserId))
            {
                throw new KeyNotFoundException("User not found");
            }

            // Update the properties of the existing refresh token.
            existingRefreshToken.UserId = refreshToken.UserId;
            existingRefreshToken.Token = refreshToken.Token;
            existingRefreshToken.ExpiresAt = refreshToken.ExpiresAt;
            existingRefreshToken.Revoked = refreshToken.Revoked;

            _context.RefreshTokens.Update(existingRefreshToken);
            await _context.SaveChangesAsync();
        }

        // Deletes a refresh token record by its ID.
        public async Task DeleteAsync(Guid id)
        {
            var refreshToken = await _context.RefreshTokens.FindAsync(id);
            if (refreshToken == null)
            {
                throw new KeyNotFoundException("RefreshToken not found");
            }

            _context.RefreshTokens.Remove(refreshToken);
            await _context.SaveChangesAsync();
        }
    }
}
