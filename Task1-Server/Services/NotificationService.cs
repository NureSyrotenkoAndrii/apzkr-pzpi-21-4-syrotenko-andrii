using Microsoft.EntityFrameworkCore;
using SafeEscape.Interfaces;
using SafeEscape.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SafeEscape.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;

        // Constructor to inject the database context.
        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Retrieves all notifications from the database.
        public async Task<IEnumerable<Notification>> GetAllAsync()
        {
            return await _context.Notifications.ToListAsync();
        }

        // Retrieves a specific notification by its ID.
        public async Task<Notification> GetByIdAsync(Guid id)
        {
            return await _context.Notifications.FindAsync(id);
        }

        // Creates a new notification record if the associated user exists.
        public async Task CreateAsync(Notification notification)
        {
            if (!await _context.Users.AnyAsync(u => u.Id == notification.UserId))
            {
                throw new KeyNotFoundException("User not found");
            }

            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }

        // Updates an existing notification record.
        public async Task UpdateAsync(Guid id, Notification notification)
        {
            var existingNotification = await _context.Notifications.FindAsync(id);
            if (existingNotification == null)
            {
                throw new KeyNotFoundException("Notification not found");
            }

            if (!await _context.Users.AnyAsync(u => u.Id == notification.UserId))
            {
                throw new KeyNotFoundException("User not found");
            }

            existingNotification.UserId = notification.UserId;
            existingNotification.Message = notification.Message;
            existingNotification.Read = notification.Read;

            _context.Notifications.Update(existingNotification);
            await _context.SaveChangesAsync();
        }

        // Deletes a notification record by its ID.
        public async Task DeleteAsync(Guid id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
            {
                throw new KeyNotFoundException("Notification not found");
            }

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
        }

        // Marks a notification as read by updating its "Read" status.
        public async Task MarkAsReadAsync(Guid id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
            {
                throw new KeyNotFoundException("Notification not found");
            }

            notification.Read = true;
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();
        }
    }
}
