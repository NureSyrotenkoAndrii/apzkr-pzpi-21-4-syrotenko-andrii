using Microsoft.EntityFrameworkCore;
using SafeEscape.Interfaces;
using SafeEscape.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SafeEscape.Services
{
    public class FloorService : IFloorService
    {
        private readonly ApplicationDbContext _context;

        // Constructor initializes the service with the database context.
        public FloorService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Retrieves all floors from the database.
        public async Task<IEnumerable<Floor>> GetAllAsync()
        {
            return await _context.Floors.ToListAsync();
        }

        // Retrieves a specific floor by its ID.
        public async Task<Floor> GetByIdAsync(Guid id)
        {
            return await _context.Floors.FindAsync(id);
        }

        // Creates a new floor if the building exists.
        public async Task CreateAsync(Floor floor)
        {
            // Check if the building exists.
            if (!await _context.Buildings.AnyAsync(b => b.Id == floor.BuildingId))
            {
                throw new KeyNotFoundException("Building not found");
            }

            _context.Floors.Add(floor);
            await _context.SaveChangesAsync();
        }

        // Updates an existing floor's details.
        public async Task UpdateAsync(Guid id, Floor floor)
        {
            var existingFloor = await _context.Floors.FindAsync(id);
            if (existingFloor == null)
            {
                throw new KeyNotFoundException("Floor not found");
            }

            if (!await _context.Buildings.AnyAsync(b => b.Id == floor.BuildingId))
            {
                throw new KeyNotFoundException("Building not found");
            }

            existingFloor.FloorNumber = floor.FloorNumber;
            existingFloor.PlanImageUrl = floor.PlanImageUrl;
            existingFloor.BuildingId = floor.BuildingId;

            _context.Floors.Update(existingFloor);
            await _context.SaveChangesAsync();
        }

        // Deletes a floor by its ID.
        public async Task DeleteAsync(Guid id)
        {
            var floor = await _context.Floors.FindAsync(id);
            if (floor == null)
            {
                throw new KeyNotFoundException("Floor not found");
            }

            _context.Floors.Remove(floor);
            await _context.SaveChangesAsync();
        }

        // Calculates the average smoke level for a specific floor.
        public async Task<double> GetSmokeStatisticsAsync(Guid floorId)
        {
            var rooms = await _context.Rooms.Where(r => r.FloorId == floorId).Select(r => r.Id).ToListAsync();
            var measurements = await _context.Measurements
                .Include(m => m.Sensor)
                .Where(m => rooms.Contains(m.Sensor.RoomId) && m.Sensor.Type == "smoke")
                .ToListAsync();

            if (measurements.Count == 0)
            {
                throw new KeyNotFoundException("No smoke measurements found for the specified floor");
            }

            return measurements.Average(m => m.Value); // Calculate the average smoke level.
        }

        // Retrieves all rooms on a specific floor.
        public async Task<IEnumerable<Room>> GetRoomsByFloorIdAsync(Guid floorId)
        {
            return await _context.Rooms.Where(r => r.FloorId == floorId).ToListAsync();
        }

        // Retrieves the building ID associated with a specific floor.
        public async Task<Guid?> GetBuildingIdByFloorIdAsync(Guid floorId)
        {
            var floor = await _context.Floors.FindAsync(floorId);
            return floor?.BuildingId;
        }
    }
}
