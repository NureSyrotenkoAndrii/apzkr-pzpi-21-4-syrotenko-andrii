using Microsoft.EntityFrameworkCore;
using SafeEscape.Interfaces;
using SafeEscape.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SafeEscape.Services
{
    public class RoleService : IRoleService
    {
        private readonly ApplicationDbContext _context;

        // Constructor to inject the database context.
        public RoleService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Retrieves all roles from the database.
        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        // Retrieves a specific role by its ID.
        public async Task<Role> GetByIdAsync(Guid id)
        {
            return await _context.Roles.FindAsync(id);
        }

        // Creates a new role record in the database.
        public async Task CreateAsync(Role role)
        {
            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();
        }

        // Updates an existing role record.
        public async Task UpdateAsync(Guid id, Role role)
        {
            var existingRole = await _context.Roles.FindAsync(id);
            if (existingRole == null)
            {
                throw new KeyNotFoundException("Role not found");
            }

            existingRole.Name = role.Name;

            _context.Roles.Update(existingRole);
            await _context.SaveChangesAsync();
        }

        // Deletes a role record by its ID.
        public async Task DeleteAsync(Guid id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                throw new KeyNotFoundException("Role not found");
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
        }
    }
}
