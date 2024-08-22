using Microsoft.EntityFrameworkCore;
using SafeEscape.Interfaces;
using SafeEscape.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SafeEscape.Services
{
    public class ExitService : IExitService
    {
        private readonly ApplicationDbContext _context;

        // Constructor to initialize the service with the database context.
        public ExitService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Retrieves all exits from the database.
        public async Task<IEnumerable<Exit>> GetAllAsync()
        {
            return await _context.Exits.ToListAsync();
        }

        // Retrieves a specific exit by its ID.
        public async Task<Exit> GetByIdAsync(Guid id)
        {
            return await _context.Exits.FindAsync(id);
        }

        // Creates a new exit if the building and room exist.
        public async Task CreateAsync(Exit exit)
        {
            // Validate if the associated building exists.
            if (!await _context.Buildings.AnyAsync(b => b.Id == exit.BuildingId))
            {
                throw new KeyNotFoundException("Building not found");
            }

            // Validate if the associated room exists.
            if (!await _context.Rooms.AnyAsync(r => r.Id == exit.RoomId))
            {
                throw new KeyNotFoundException("Room not found");
            }

            _context.Exits.Add(exit);
            await _context.SaveChangesAsync();
        }

        // Updates an existing exit's details.
        public async Task UpdateAsync(Guid id, Exit exit)
        {
            var existingExit = await _context.Exits.FindAsync(id);
            if (existingExit == null)
            {
                throw new KeyNotFoundException("Exit not found");
            }

            // Validate if the updated building exists.
            if (!await _context.Buildings.AnyAsync(b => b.Id == exit.BuildingId))
            {
                throw new KeyNotFoundException("Building not found");
            }

            // Validate if the updated room exists.
            if (!await _context.Rooms.AnyAsync(r => r.Id == exit.RoomId))
            {
                throw new KeyNotFoundException("Room not found");
            }

            // Update the exit properties.
            existingExit.BuildingId = exit.BuildingId;
            existingExit.Floor = exit.Floor;
            existingExit.Type = exit.Type;
            existingExit.RoomId = exit.RoomId;

            _context.Exits.Update(existingExit);
            await _context.SaveChangesAsync();
        }

        // Deletes an exit by its ID.
        public async Task DeleteAsync(Guid id)
        {
            var exit = await _context.Exits.FindAsync(id);
            if (exit == null)
            {
                throw new KeyNotFoundException("Exit not found");
            }

            _context.Exits.Remove(exit);
            await _context.SaveChangesAsync();
        }

        // Retrieves the building ID associated with a specific exit.
        public async Task<Guid?> GetBuildingIdByExitIdAsync(Guid exitId)
        {
            var exit = await _context.Exits.FindAsync(exitId);
            return exit?.BuildingId; // Return building ID if exit is found.
        }
    }
}
