using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafeEscape.Interfaces;
using SafeEscape.Models.DTO;
using SafeEscape.Models;
using System;
using System.Threading.Tasks;
using System.Security.Claims;

namespace SafeEscape.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExitsController : ControllerBase
    {
        private readonly IExitService _exitService;
        private readonly IPermissionService _permissionService;

        public ExitsController(IExitService exitService, IPermissionService permissionService)
        {
            _exitService = exitService;
            _permissionService = permissionService;
        }

        private Guid GetUserId()
        {
            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var exits = await _exitService.GetAllAsync();
            return Ok(exits);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var exit = await _exitService.GetByIdAsync(id);
            if (exit == null)
            {
                return NotFound();
            }
            return Ok(exit);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(ExitDto exitDto)
        {
            var exit = new Exit
            {
                BuildingId = exitDto.BuildingId,
                Floor = exitDto.Floor,
                Type = exitDto.Type,
                RoomId = exitDto.RoomId
            };

            await _exitService.CreateAsync(exit);
            return CreatedAtAction(nameof(GetById), new { id = exit.Id }, exit);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ExitDto exitDto)
        {
            var userId = GetUserId();
            if (!await _permissionService.UserHasPermissionAsync(userId, exitDto.BuildingId))
            {
                return StatusCode(403, "You do not have permission to edit this exit.");
            }

            var exit = new Exit
            {
                Id = id,
                BuildingId = exitDto.BuildingId,
                Floor = exitDto.Floor,
                Type = exitDto.Type,
                RoomId = exitDto.RoomId
            };

            await _exitService.UpdateAsync(id, exit);
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = GetUserId();
            var buildingId = await _exitService.GetBuildingIdByExitIdAsync(id);

            if (!await _permissionService.UserHasPermissionAsync(userId, buildingId.Value))
            {
                return StatusCode(403, "You do not have permission to delete this exit.");
            }

            await _exitService.DeleteAsync(id);
            return NoContent();
        }
    }
}
