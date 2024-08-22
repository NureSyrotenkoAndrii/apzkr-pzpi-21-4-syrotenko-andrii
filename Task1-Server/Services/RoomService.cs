using Microsoft.EntityFrameworkCore;
using SafeEscape.Interfaces;
using SafeEscape.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SafeEscape.Services
{
    public class RoomService : IRoomService
    {
        private readonly ApplicationDbContext _context;

        public RoomService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Retrieves all Room records from the database.
        public async Task<IEnumerable<Room>> GetAllAsync()
        {
            return await _context.Rooms.ToListAsync();
        }

        // Retrieves a specific Room record by its ID.
        public async Task<Room> GetByIdAsync(Guid id)
        {
            return await _context.Rooms.FindAsync(id);
        }

        // Creates a new Room record after validating that the related building and floor exist.
        public async Task CreateAsync(Room room)
        {
            if (!await _context.Buildings.AnyAsync(b => b.Id == room.BuildingId))
            {
                throw new KeyNotFoundException("Building not found");
            }

            if (!await _context.Floors.AnyAsync(f => f.Id == room.FloorId))
            {
                throw new KeyNotFoundException("Floor not found");
            }

            await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();
        }

        // Updates an existing Room record after validating that the related building and floor exist.
        public async Task UpdateAsync(Guid id, Room room)
        {
            var existingRoom = await _context.Rooms.FindAsync(id);
            if (existingRoom == null)
            {
                throw new KeyNotFoundException("Room not found");
            }

            if (!await _context.Buildings.AnyAsync(b => b.Id == room.BuildingId))
            {
                throw new KeyNotFoundException("Building not found");
            }

            if (!await _context.Floors.AnyAsync(f => f.Id == room.FloorId))
            {
                throw new KeyNotFoundException("Floor not found");
            }

            existingRoom.BuildingId = room.BuildingId;
            existingRoom.FloorId = room.FloorId;
            existingRoom.FloorNumber = room.FloorNumber;
            existingRoom.Name = room.Name;
            existingRoom.Type = room.Type;

            _context.Rooms.Update(existingRoom);
            await _context.SaveChangesAsync();
        }

        // Deletes a Room record by its ID.
        public async Task DeleteAsync(Guid id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                throw new KeyNotFoundException("Room not found");
            }

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
        }

        // Retrieves all rooms in a specified building.
        public async Task<IEnumerable<Room>> GetRoomsByBuildingIdAsync(Guid buildingId)
        {
            return await _context.Rooms.Where(r => r.BuildingId == buildingId).ToListAsync();
        }

        // Calculates the average smoke measurement value for a specified room.
        public async Task<double> GetSmokeStatisticsAsync(Guid roomId)
        {
            var measurements = await _context.Measurements
                .Include(m => m.Sensor)
                .Where(m => m.Sensor.RoomId == roomId && m.Sensor.Type == "smoke")
                .ToListAsync();

            if (measurements.Count == 0)
            {
                throw new KeyNotFoundException("No smoke measurements found for the specified room");
            }

            return measurements.Average(m => m.Value);
        }

        // Retrieves the building ID for a specified room.
        public async Task<Guid?> GetBuildingIdByRoomIdAsync(Guid roomId)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            return room?.BuildingId;
        }
    }
}
