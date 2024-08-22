using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafeEscape.Interfaces;
using SafeEscape.Models.DTO;
using SafeEscape.Models;
using System;
using System.Threading.Tasks;

namespace SafeEscape.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserRolesController : ControllerBase
    {
        private readonly IUserRoleService _userRoleService;

        public UserRolesController(IUserRoleService userRoleService)
        {
            _userRoleService = userRoleService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userRoles = await _userRoleService.GetAllAsync();
            return Ok(userRoles);
        }

        [Authorize]
        [HttpGet("{userId}/{roleId}")]
        public async Task<IActionResult> GetById(Guid userId, Guid roleId)
        {
            var userRole = await _userRoleService.GetByIdAsync(userId, roleId);
            if (userRole == null)
            {
                return NotFound();
            }
            return Ok(userRole);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(UserRoleDto userRoleDto)
        {
            var userRole = new UserRole
            {
                UserId = userRoleDto.UserId,
                RoleId = userRoleDto.RoleId
            };

            await _userRoleService.CreateAsync(userRole);
            return CreatedAtAction(nameof(GetById), new { userId = userRole.UserId, roleId = userRole.RoleId }, userRole);
        }

        [Authorize]
        [HttpDelete("{userId}/{roleId}")]
        public async Task<IActionResult> Delete(Guid userId, Guid roleId)
        {
            await _userRoleService.DeleteAsync(userId, roleId);
            return NoContent();
        }
    }
}
