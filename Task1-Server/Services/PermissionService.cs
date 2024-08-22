using Microsoft.EntityFrameworkCore;
using SafeEscape.Interfaces;
using SafeEscape.Models;

namespace SafeEscape.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;
        private readonly IUserBuildingService _userBuildingService;

        // Constructor to inject dependencies for user service and user building service.
        public PermissionService(ApplicationDbContext context, IUserService userService, IUserBuildingService userBuildingService)
        {
            _context = context;
            _userService = userService;
            _userBuildingService = userBuildingService;
        }

        // Checks if a user has permission to access or manage a specific building.
        public async Task<bool> UserHasPermissionAsync(Guid userId, Guid buildingId)
        {
            // The user has permission if they are an Admin or a manager of the specified building.
            return await _userService.IsUserInRoleAsync(userId, "Admin") ||
                   await _userBuildingService.IsBuildingManagerAsync(userId, buildingId);
        }

    }
}
