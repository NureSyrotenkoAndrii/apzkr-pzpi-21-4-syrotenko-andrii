using Microsoft.EntityFrameworkCore;
using SafeEscape.Interfaces;
using SafeEscape.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SafeEscape.Services
{
    public class GeoJsonFloorDataService : IGeoJsonFloorDataService
    {
        private readonly ApplicationDbContext _context;

        // Constructor to initialize the service with the database context.
        public GeoJsonFloorDataService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Retrieves all GeoJSON floor data records from the database.
        public async Task<IEnumerable<GeoJsonFloorData>> GetAllAsync()
        {
            return await _context.GeoJsonFloorData.ToListAsync();
        }

        // Retrieves a specific GeoJSON floor data record by its ID.
        public async Task<GeoJsonFloorData> GetByIdAsync(Guid id)
        {
            return await _context.GeoJsonFloorData.FindAsync(id);
        }

        // Creates a new GeoJSON floor data record if the building exists.
        public async Task CreateAsync(GeoJsonFloorData geoJsonFloorData)
        {
            // Ensure the associated building exists.
            if (!await _context.Buildings.AnyAsync(b => b.Id == geoJsonFloorData.BuildingId))
            {
                throw new KeyNotFoundException("Building not found");
            }

            _context.GeoJsonFloorData.Add(geoJsonFloorData);
            await _context.SaveChangesAsync();
        }

        // Updates an existing GeoJSON floor data record.
        public async Task UpdateAsync(Guid id, GeoJsonFloorData geoJsonFloorData)
        {
            var existingGeoJsonFloorData = await _context.GeoJsonFloorData.FindAsync(id);
            if (existingGeoJsonFloorData == null)
            {
                throw new KeyNotFoundException("GeoJsonFloorData not found");
            }

            if (!await _context.Buildings.AnyAsync(b => b.Id == geoJsonFloorData.BuildingId))
            {
                throw new KeyNotFoundException("Building not found");
            }

            existingGeoJsonFloorData.BuildingId = geoJsonFloorData.BuildingId;
            existingGeoJsonFloorData.FloorNumber = geoJsonFloorData.FloorNumber;
            existingGeoJsonFloorData.GeojsonData = geoJsonFloorData.GeojsonData;

            _context.GeoJsonFloorData.Update(existingGeoJsonFloorData);
            await _context.SaveChangesAsync();
        }

        // Deletes a GeoJSON floor data record by its ID.
        public async Task DeleteAsync(Guid id)
        {
            var geoJsonFloorData = await _context.GeoJsonFloorData.FindAsync(id);
            if (geoJsonFloorData == null)
            {
                throw new KeyNotFoundException("GeoJsonFloorData not found");
            }

            _context.GeoJsonFloorData.Remove(geoJsonFloorData);
            await _context.SaveChangesAsync();
        }

        // Retrieves the building ID associated with a specific GeoJSON floor data record.
        public async Task<Guid?> GetBuildingIdByGeoIdAsync(Guid geoDataId)
        {
            var geoData = await _context.GeoJsonFloorData.FindAsync(geoDataId);
            return geoData?.BuildingId;
        }
    }
}
