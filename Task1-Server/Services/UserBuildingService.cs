using Microsoft.EntityFrameworkCore;
using SafeEscape.Interfaces;
using SafeEscape.Models;
using SafeEscape.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SafeEscape.Services
{
    public class UserBuildingService : IUserBuildingService
    {
        private readonly ApplicationDbContext _context;

        public UserBuildingService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Retrieves all UserBuilding entries from the database.
        public async Task<IEnumerable<UserBuilding>> GetAllAsync()
        {
            return await _context.UserBuildings.ToListAsync();
        }

        // Retrieves a UserBuilding entry by user ID and building ID.
        public async Task<UserBuilding> GetByIdAsync(Guid userId, Guid buildingId)
        {
            return await _context.UserBuildings.FindAsync(userId, buildingId);
        }

        // Creates a new UserBuilding entry in the database.
        public async Task CreateAsync(UserBuilding userBuilding)
        {
            if (!await _context.Users.AnyAsync(u => u.Id == userBuilding.UserId))
            {
                throw new KeyNotFoundException("User not found");
            }

            if (!await _context.Buildings.AnyAsync(b => b.Id == userBuilding.BuildingId))
            {
                throw new KeyNotFoundException("Building not found");
            }

            await _context.UserBuildings.AddAsync(userBuilding);
            await _context.SaveChangesAsync();
        }
        // Updates an existing UserBuilding entry.
        public async Task UpdateAsync(Guid userId, Guid buildingId, UserBuilding userBuilding)
        {
            var existingUserBuilding = await _context.UserBuildings.FindAsync(userId, buildingId);
            if (existingUserBuilding == null)
            {
                throw new KeyNotFoundException("UserBuilding not found");
            }

            if (!await _context.Users.AnyAsync(u => u.Id == userBuilding.UserId))
            {
                throw new KeyNotFoundException("User not found");
            }

            if (!await _context.Buildings.AnyAsync(b => b.Id == userBuilding.BuildingId))
            {
                throw new KeyNotFoundException("Building not found");
            }

            existingUserBuilding.IsManager = userBuilding.IsManager;

            _context.UserBuildings.Update(existingUserBuilding);
            await _context.SaveChangesAsync();
        }

        // Deletes a UserBuilding entry from the database.
        public async Task DeleteAsync(Guid userId, Guid buildingId)
        {
            var userBuilding = await _context.UserBuildings.FindAsync(userId, buildingId);
            if (userBuilding == null)
            {
                throw new KeyNotFoundException("UserBuilding not found");
            }

            _context.UserBuildings.Remove(userBuilding);
            await _context.SaveChangesAsync();
        }

        // Adds a user to a building with manager status.
        public async Task AddUserToBuildingAsync(AddUserBuildingDto dto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == dto.UserEmail);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            var userBuilding = new UserBuilding
            {
                UserId = user.Id,
                BuildingId = dto.BuildingId,
                IsManager = dto.IsManager
            };

            await _context.UserBuildings.AddAsync(userBuilding);
            await _context.SaveChangesAsync();
        }

        // Checks if a user is a manager for a specific building.
        public async Task<bool> IsBuildingManagerAsync(Guid userId, Guid buildingId)
        {
            var userBuilding = await _context.UserBuildings
                .FirstOrDefaultAsync(ub => ub.UserId == userId && ub.BuildingId == buildingId);
            return userBuilding != null && userBuilding.IsManager;
        }
    }
}
