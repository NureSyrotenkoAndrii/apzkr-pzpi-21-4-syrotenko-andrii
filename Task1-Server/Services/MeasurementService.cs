using Microsoft.EntityFrameworkCore;
using SafeEscape.Interfaces;
using SafeEscape.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SafeEscape.Services
{
    public class MeasurementService : IMeasurementService
    {
        private readonly ApplicationDbContext _context;

        // Constructor to inject the database context.
        public MeasurementService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Retrieves all measurements from the database.
        public async Task<IEnumerable<Measurement>> GetAllAsync()
        {
            return await _context.Measurements.ToListAsync();
        }

        // Retrieves a specific measurement by its ID.
        public async Task<Measurement> GetByIdAsync(Guid id)
        {
            return await _context.Measurements.FindAsync(id);
        }

        // Creates a new measurement record if the associated sensor exists.
        public async Task CreateAsync(Measurement measurement)
        {
            // Ensure the associated sensor exists before creating the measurement.
            if (!await _context.Sensors.AnyAsync(s => s.Id == measurement.SensorId))
            {
                throw new KeyNotFoundException("Sensor not found");
            }

            await _context.Measurements.AddAsync(measurement);
            await _context.SaveChangesAsync();
        }

        // Updates an existing measurement record.
        public async Task UpdateAsync(Guid id, Measurement measurement)
        {
            var existingMeasurement = await _context.Measurements.FindAsync(id);
            if (existingMeasurement == null)
            {
                throw new KeyNotFoundException("Measurement not found");
            }

            if (!await _context.Sensors.AnyAsync(s => s.Id == measurement.SensorId))
            {
                throw new KeyNotFoundException("Sensor not found");
            }

            existingMeasurement.SensorId = measurement.SensorId;
            existingMeasurement.Timestamp = measurement.Timestamp;
            existingMeasurement.Value = measurement.Value;

            _context.Measurements.Update(existingMeasurement);
            await _context.SaveChangesAsync();
        }

        // Deletes a measurement record by its ID.
        public async Task DeleteAsync(Guid id)
        {
            var measurement = await _context.Measurements.FindAsync(id);
            if (measurement == null)
            {
                throw new KeyNotFoundException("Measurement not found");
            }

            _context.Measurements.Remove(measurement);
            await _context.SaveChangesAsync();
        }

        // Retrieves all measurements associated with a specific sensor ID.
        public async Task<IEnumerable<Measurement>> GetMeasurementsBySensorIdAsync(Guid sensorId)
        {
            return await _context.Measurements.Where(m => m.SensorId == sensorId).ToListAsync();
        }
    }
}
