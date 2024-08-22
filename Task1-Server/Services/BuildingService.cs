using Microsoft.EntityFrameworkCore;
using SafeEscape.Interfaces;
using SafeEscape.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SafeEscape.Services
{
    public class BuildingService : IBuildingService
    {
        private readonly ApplicationDbContext _context;

        // Constructor that initializes the service with the provided database context.
        public BuildingService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Retrieves all buildings from the database.
        public async Task<IEnumerable<Building>> GetAllAsync()
        {
            return await _context.Buildings.ToListAsync();
        }

        // Retrieves a specific building by its ID.
        public async Task<Building> GetByIdAsync(Guid id)
        {
            return await _context.Buildings.FindAsync(id);
        }

        // Creates a new building and assigns a user as its manager.
        public async Task CreateAsync(Building building, Guid userId)
        {
            _context.Buildings.Add(building);
            await _context.SaveChangesAsync();

            // Assign the user as the manager of the new building.
            var userBuilding = new UserBuilding
            {
                UserId = userId,
                BuildingId = building.Id,
                IsManager = true
            };

            _context.UserBuildings.Add(userBuilding);
            await _context.SaveChangesAsync();
        }

        // Updates an existing building's details.
        public async Task UpdateAsync(Guid id, Building building)
        {
            var existingBuilding = await _context.Buildings.FindAsync(id);
            if (existingBuilding == null)
            {
                throw new KeyNotFoundException("Building not found");
            }

            // Update building properties.
            existingBuilding.Name = building.Name;
            existingBuilding.Address = building.Address;
            existingBuilding.NumberOfFloors = building.NumberOfFloors;

            _context.Buildings.Update(existingBuilding);
            await _context.SaveChangesAsync();
        }

        // Deletes a building by its ID.
        public async Task DeleteAsync(Guid id)
        {
            var building = await _context.Buildings.FindAsync(id);
            if (building == null)
            {
                throw new KeyNotFoundException("Building not found");
            }

            _context.Buildings.Remove(building);
            await _context.SaveChangesAsync();
        }

        // Calculates the average smoke level for a given building.
        public async Task<double> GetSmokeStatisticsAsync(Guid buildingId)
        {
            var rooms = await _context.Rooms.Where(r => r.BuildingId == buildingId).Select(r => r.Id).ToListAsync();
            var measurements = await _context.Measurements
                .Include(m => m.Sensor)
                .Where(m => rooms.Contains(m.Sensor.RoomId) && m.Sensor.Type == "smoke")
                .ToListAsync();

            if (measurements.Count == 0)
            {
                throw new KeyNotFoundException("No smoke measurements found for the specified building");
            }

            return measurements.Average(m => m.Value); // Calculate average smoke value.
        }

        // Retrieves all users associated with a specific building.
        public async Task<IEnumerable<User>> GetUsersByBuildingIdAsync(Guid buildingId)
        {
            var userBuildings = await _context.UserBuildings
                .Where(ub => ub.BuildingId == buildingId)
                .ToListAsync();

            var userIds = userBuildings.Select(ub => ub.UserId).ToList();

            return await _context.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();
        }

        // Retrieves all floors associated with a specific building.
        public async Task<IEnumerable<Floor>> GetFloorsByBuildingIdAsync(Guid buildingId)
        {
            return await _context.Floors.Where(f => f.BuildingId == buildingId).ToListAsync();
        }
    }
}
