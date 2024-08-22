using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafeEscape.Interfaces;
using SafeEscape.Models;
using SafeEscape.Models.DTO;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SafeEscape.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BuildingController : ControllerBase
    {
        private readonly IBuildingService _buildingService;
        private readonly IPermissionService _permissionService;

        public BuildingController(IBuildingService buildingService, IPermissionService permissionService)
        {
            _buildingService = buildingService;
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
            var buildings = await _buildingService.GetAllAsync();
            return Ok(buildings);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var building = await _buildingService.GetByIdAsync(id);
            if (building == null)
            {
                return NotFound();
            }
            return Ok(building);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(BuildingDto buildingDto, Guid userId)
        {
            var building = new Building
            {
                Name = buildingDto.Name,
                Address = buildingDto.Address,
                NumberOfFloors = buildingDto.NumberOfFloors
            };

            await _buildingService.CreateAsync(building, userId);
            return CreatedAtAction(nameof(GetById), new { id = building.Id }, building);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, BuildingDto buildingDto)
        {
            var userId = GetUserId();
            if (!await _permissionService.UserHasPermissionAsync(userId, id))
            {
                return StatusCode(403, "You do not have permission to edit this building.");
            }

            var building = new Building
            {
                Id = id,
                Name = buildingDto.Name,
                Address = buildingDto.Address,
                NumberOfFloors = buildingDto.NumberOfFloors
            };

            await _buildingService.UpdateAsync(id, building);
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = GetUserId();
            if (!await _permissionService.UserHasPermissionAsync(userId, id))
            {
                return StatusCode(403, "You do not have permission to delete this building.");
            }

            await _buildingService.DeleteAsync(id);
            return NoContent();
        }

        [Authorize]
        [HttpGet("/api/buildings/{buildingId}/smoke-statistics")]
        public async Task<IActionResult> GetSmokeStatistics(Guid buildingId)
        {
            try
            {
                var averageSmokeLevel = await _buildingService.GetSmokeStatisticsAsync(buildingId);
                return Ok(new { AverageSmokeLevel = averageSmokeLevel });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("{buildingId}/users")]
        public async Task<IActionResult> GetUsersByBuildingId(Guid buildingId)
        {
            var users = await _buildingService.GetUsersByBuildingIdAsync(buildingId);
            return Ok(users);
        }

        [Authorize]
        [HttpGet("{buildingId}/floors")]
        public async Task<IActionResult> GetFloorsByBuildingId(Guid buildingId)
        {
            var floors = await _buildingService.GetFloorsByBuildingIdAsync(buildingId);
            return Ok(floors);
        }
    }
}
