using Microsoft.EntityFrameworkCore;
using SafeEscape.Interfaces;
using SafeEscape.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SafeEscape.Services
{
    public class SensorService : ISensorService
    {
        private readonly ApplicationDbContext _context;

        public SensorService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Retrieves all sensors from the database.
        public async Task<IEnumerable<Sensor>> GetAllAsync()
        {
            return await _context.Sensors.ToListAsync();
        }

        // Retrieves a sensor by its ID.
        public async Task<Sensor> GetByIdAsync(Guid id)
        {
            return await _context.Sensors.FindAsync(id);
        }

        // Creates a new sensor entry in the database.
        public async Task CreateAsync(Sensor sensor)
        {
            if (!await _context.Rooms.AnyAsync(r => r.Id == sensor.RoomId))
            {
                throw new KeyNotFoundException("Room not found");
            }

            await _context.Sensors.AddAsync(sensor);
            await _context.SaveChangesAsync();
        }

        // Updates an existing sensor entry in the database.
        public async Task UpdateAsync(Guid id, Sensor sensor)
        {
            var existingSensor = await _context.Sensors.FindAsync(id);
            if (existingSensor == null)
            {
                throw new KeyNotFoundException("Sensor not found");
            }

            if (!await _context.Rooms.AnyAsync(r => r.Id == sensor.RoomId))
            {
                throw new KeyNotFoundException("Room not found");
            }

            existingSensor.RoomId = sensor.RoomId;
            existingSensor.SensorName = sensor.SensorName;
            existingSensor.Type = sensor.Type;
            existingSensor.Threshold = sensor.Threshold;
            existingSensor.GeojsonData = sensor.GeojsonData;

            _context.Sensors.Update(existingSensor);
            await _context.SaveChangesAsync();
        }

        // Deletes a sensor from the database.
        public async Task DeleteAsync(Guid id)
        {
            var sensor = await _context.Sensors.FindAsync(id);
            if (sensor == null)
            {
                throw new KeyNotFoundException("Sensor not found");
            }

            _context.Sensors.Remove(sensor);
            await _context.SaveChangesAsync();
        }

        // Retrieves sensors by their associated room ID.
        public async Task<IEnumerable<Sensor>> GetSensorsByRoomIdAsync(Guid roomId)
        {
            return await _context.Sensors.Where(s => s.RoomId == roomId).ToListAsync();
        }

        // Updates the threshold value for a specific sensor.
        public async Task UpdateThresholdAsync(Guid id, float threshold)
        {
            var sensor = await _context.Sensors.FindAsync(id);
            if (sensor == null)
            {
                throw new KeyNotFoundException("Sensor not found");
            }

            sensor.Threshold = threshold;
            _context.Sensors.Update(sensor);
            await _context.SaveChangesAsync();
        }
    }
}
