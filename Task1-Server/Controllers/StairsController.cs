using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafeEscape.Interfaces;
using SafeEscape.Models.DTO;
using SafeEscape.Models;
using System;
using System.Threading.Tasks;
using System.Security.Claims;
using SafeEscape.Services;

namespace SafeEscape.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StairsController : ControllerBase
    {
        private readonly IStairService _stairService;
        private readonly IPermissionService _permissionService;

        public StairsController(IStairService stairService, IPermissionService permissionService)
        {
            _stairService = stairService;
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
            var stairs = await _stairService.GetAllAsync();
            return Ok(stairs);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var stair = await _stairService.GetByIdAsync(id);
            if (stair == null)
            {
                return NotFound();
            }
            return Ok(stair);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(StairDto stairDto)
        {
            var stair = new Stair
            {
                BuildingId = stairDto.BuildingId,
                Floor1RoomId = stairDto.Floor1RoomId,
                Floor2RoomId = stairDto.Floor2RoomId
            };

            await _stairService.CreateAsync(stair);
            return CreatedAtAction(nameof(GetById), new { id = stair.Id }, stair);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, StairDto stairDto)
        {

            var userId = GetUserId();
            if (!await _permissionService.UserHasPermissionAsync(userId, stairDto.BuildingId))
            {
                return StatusCode(403, "You do not have permission to edit this stair.");
            }

            var stair = new Stair
            {
                Id = id,
                BuildingId = stairDto.BuildingId,
                Floor1RoomId = stairDto.Floor1RoomId,
                Floor2RoomId = stairDto.Floor2RoomId
            };

            await _stairService.UpdateAsync(id, stair);
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {

            var userId = GetUserId();
            var buildingId = await _stairService.GetBuildingIdByStairIdAsync(id);

            if (!await _permissionService.UserHasPermissionAsync(userId, buildingId.Value))
            {
                return StatusCode(403, "You do not have permission to delete this stair.");
            }

            await _stairService.DeleteAsync(id);
            return NoContent();
        }
    }
}
