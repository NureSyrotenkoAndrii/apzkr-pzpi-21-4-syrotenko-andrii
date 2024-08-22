using SafeEscape.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace SafeEscape.Interfaces
{
    public interface INotificationService
    {
        Task<IEnumerable<Notification>> GetAllAsync();
        Task<Notification> GetByIdAsync(Guid id);
        Task CreateAsync(Notification notification);
        Task UpdateAsync(Guid id, Notification notification);
        Task DeleteAsync(Guid id);
        Task MarkAsReadAsync(Guid id);
    }
}
