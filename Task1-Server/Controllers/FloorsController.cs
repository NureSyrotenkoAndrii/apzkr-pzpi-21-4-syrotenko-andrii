using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafeEscape.Interfaces;
using SafeEscape.Models;
using SafeEscape.Models.DTO;
using SafeEscape.Services;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SafeEscape.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FloorsController : ControllerBase
    {
        private readonly IFloorService _floorService;
        private readonly S3Service _s3Service;
        private readonly IBuildingService _buildingService;
        private readonly IPermissionService _permissionService;

        public FloorsController(IFloorService floorService, S3Service s3Service, IBuildingService buildingService, IPermissionService permissionService)
        {
            _floorService = floorService;
            _s3Service = s3Service;
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
            var floors = await _floorService.GetAllAsync();
            return Ok(floors);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var floor = await _floorService.GetByIdAsync(id);
            if (floor == null)
            {
                return NotFound();
            }
            return Ok(floor);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] FloorCreateUpdateDto floorDto)
        {
            var building = await _buildingService.GetByIdAsync(floorDto.BuildingId);
            if (building == null)
            {
                return NotFound("Building not found");
            }

            var floor = new Floor
            {
                BuildingId = floorDto.BuildingId,
                FloorNumber = floorDto.FloorNumber,
            };

            if (floorDto.PlanImage != null)
            {
                var key = $"{floorDto.BuildingId}/{floorDto.FloorNumber}.jpg";
                var s3Url = await _s3Service.UploadFileAsync(floorDto.PlanImage, key);
                floor.PlanImageUrl = s3Url;
            }

            await _floorService.CreateAsync(floor);
            return CreatedAtAction(nameof(GetById), new { id = floor.Id }, floor);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromForm] FloorCreateUpdateDto floorDto)
        {
            var userId = GetUserId();
            if (!await _permissionService.UserHasPermissionAsync(userId, floorDto.BuildingId))
            {
                return StatusCode(403, "You do not have permission to edit this floor.");
            }

            var existingFloor = await _floorService.GetByIdAsync(id);
            if (existingFloor == null)
            {
                return NotFound("Floor not found");
            }

            existingFloor.FloorNumber = floorDto.FloorNumber;

            if (floorDto.PlanImage != null)
            {
                var key = $"{floorDto.BuildingId}/{floorDto.FloorNumber}.jpg";
                var s3Url = await _s3Service.UploadFileAsync(floorDto.PlanImage, key);
                existingFloor.PlanImageUrl = s3Url;
            }

            await _floorService.UpdateAsync(id, existingFloor);
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = GetUserId();
            var buildingId = await _floorService.GetBuildingIdByFloorIdAsync(id);

            if (!await _permissionService.UserHasPermissionAsync(userId, buildingId.Value))
            {
                return StatusCode(403, "You do not have permission to delete this floor.");
            }

            var floor = await _floorService.GetByIdAsync(id);
            if (floor == null)
            {
                return NotFound("Floor not found");
            }

            if (!string.IsNullOrEmpty(floor.PlanImageUrl))
            {
                var key = $"{floor.BuildingId}/{floor.FloorNumber}.jpg";
                await _s3Service.DeleteFileAsync(key);
            }

            await _floorService.DeleteAsync(id);
            return NoContent();
        }

        [Authorize]
        [HttpGet("/api/floors/{floorId}/smoke-statistics")]
        public async Task<IActionResult> GetSmokeStatistics(Guid floorId)
        {
            try
            {
                var averageSmokeLevel = await _floorService.GetSmokeStatisticsAsync(floorId);
                return Ok(new { AverageSmokeLevel = averageSmokeLevel });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("{floorId}/rooms")]
        public async Task<IActionResult> GetRoomsByFloorId(Guid floorId)
        {
            var rooms = await _floorService.GetRoomsByFloorIdAsync(floorId);
            return Ok(rooms);
        }
    }
}
