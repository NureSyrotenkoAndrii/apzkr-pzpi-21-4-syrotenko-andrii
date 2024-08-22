using Microsoft.EntityFrameworkCore;
using SafeEscape.Interfaces;
using SafeEscape.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SafeEscape.Services
{
    public class UserRoleService : IUserRoleService
    {
        private readonly ApplicationDbContext _context;

        public UserRoleService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Retrieves all UserRole entries from the database.
        public async Task<IEnumerable<UserRole>> GetAllAsync()
        {
            return await _context.UserRoles.ToListAsync();
        }

        // Retrieves a UserRole entry by user ID and role ID.
        public async Task<UserRole> GetByIdAsync(Guid userId, Guid roleId)
        {
            return await _context.UserRoles.FindAsync(userId, roleId);
        }

        // Creates a new UserRole entry in the database.
        public async Task CreateAsync(UserRole userRole)
        {
            if (!await _context.Users.AnyAsync(u => u.Id == userRole.UserId))
            {
                throw new KeyNotFoundException("User not found");
            }

            if (!await _context.Roles.AnyAsync(r => r.Id == userRole.RoleId))
            {
                throw new KeyNotFoundException("Role not found");
            }

            await _context.UserRoles.AddAsync(userRole);
            await _context.SaveChangesAsync();
        }

        // Deletes a UserRole entry from the database.
        public async Task DeleteAsync(Guid userId, Guid roleId)
        {
            var userRole = await _context.UserRoles.FindAsync(userId, roleId);
            if (userRole == null)
            {
                throw new KeyNotFoundException("UserRole not found");
            }

            _context.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync();
        }
    }
}
