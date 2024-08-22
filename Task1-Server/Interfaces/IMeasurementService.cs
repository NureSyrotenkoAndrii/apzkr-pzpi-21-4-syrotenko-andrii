using SafeEscape.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SafeEscape.Interfaces
{
    public interface IMeasurementService
    {
        Task<IEnumerable<Measurement>> GetAllAsync();
        Task<Measurement> GetByIdAsync(Guid id);
        Task CreateAsync(Measurement measurement);
        Task UpdateAsync(Guid id, Measurement measurement);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<Measurement>> GetMeasurementsBySensorIdAsync(Guid sensorId);
    }
}
