using SafeEscape.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SafeEscape.Interfaces
{
    public interface ISensorService
    {
        Task<IEnumerable<Sensor>> GetAllAsync();
        Task<Sensor> GetByIdAsync(Guid id);
        Task CreateAsync(Sensor sensor);
        Task UpdateAsync(Guid id, Sensor sensor);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<Sensor>> GetSensorsByRoomIdAsync(Guid roomId);
        Task UpdateThresholdAsync(Guid id, float threshold);
    }
}
