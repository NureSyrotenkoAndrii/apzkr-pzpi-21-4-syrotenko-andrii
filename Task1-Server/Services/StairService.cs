using Microsoft.EntityFrameworkCore;
using SafeEscape.Interfaces;
using SafeEscape.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SafeEscape.Services
{
    public class StairService : IStairService
    {
        private readonly ApplicationDbContext _context;

        public StairService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Retrieves all stairs from the database.
        public async Task<IEnumerable<Stair>> GetAllAsync()
        {
            return await _context.Stairs.ToListAsync();
        }

        // Retrieves a stair by its ID.
        public async Task<Stair> GetByIdAsync(Guid id)
        {
            return await _context.Stairs.FindAsync(id);
        }

        // Creates a new stair entry in the database.
        public async Task CreateAsync(Stair stair)
        {
            if (!await _context.Buildings.AnyAsync(b => b.Id == stair.BuildingId))
            {
                throw new KeyNotFoundException("Building not found");
            }

            if (!await _context.Rooms.AnyAsync(r => r.Id == stair.Floor1RoomId))
            {
                throw new KeyNotFoundException("Floor1Room not found");
            }

            if (!await _context.Rooms.AnyAsync(r => r.Id == stair.Floor2RoomId))
            {
                throw new KeyNotFoundException("Floor2Room not found");
            }

            await _context.Stairs.AddAsync(stair);
            await _context.SaveChangesAsync();
        }

        // Updates an existing stair entry in the database.
        public async Task UpdateAsync(Guid id, Stair stair)
        {
            var existingStair = await _context.Stairs.FindAsync(id);
            if (existingStair == null)
            {
                throw new KeyNotFoundException("Stair not found");
            }

            if (!await _context.Buildings.AnyAsync(b => b.Id == stair.BuildingId))
            {
                throw new KeyNotFoundException("Building not found");
            }

            if (!await _context.Rooms.AnyAsync(r => r.Id == stair.Floor1RoomId))
            {
                throw new KeyNotFoundException("Floor1Room not found");
            }

            if (!await _context.Rooms.AnyAsync(r => r.Id == stair.Floor2RoomId))
            {
                throw new KeyNotFoundException("Floor2Room not found");
            }

            existingStair.BuildingId = stair.BuildingId;
            existingStair.Floor1RoomId = stair.Floor1RoomId;
            existingStair.Floor2RoomId = stair.Floor2RoomId;

            _context.Stairs.Update(existingStair);
            await _context.SaveChangesAsync();
        }

        // Deletes a stair from the database.
        public async Task DeleteAsync(Guid id)
        {
            var stair = await _context.Stairs.FindAsync(id);
            if (stair == null)
            {
                throw new KeyNotFoundException("Stair not found");
            }

            _context.Stairs.Remove(stair);
            await _context.SaveChangesAsync();
        }

        // Retrieves the building ID for a specific stair.
        public async Task<Guid?> GetBuildingIdByStairIdAsync(Guid stairId)
        {
            var stair = await _context.Stairs.FindAsync(stairId);
            return stair?.BuildingId;
        }
    }
}
