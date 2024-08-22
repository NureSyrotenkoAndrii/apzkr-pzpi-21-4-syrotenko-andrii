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
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _roomService;
        private readonly IPermissionService _permissionService;

        public RoomsController(IRoomService roomService, IPermissionService permissionService)
        {
            _roomService = roomService;
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
            var rooms = await _roomService.GetAllAsync();
            return Ok(rooms);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var room = await _roomService.GetByIdAsync(id);
            if (room == null)
            {
                return NotFound();
            }
            return Ok(room);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(RoomDto roomDto)
        {
            var room = new Room
            {
                BuildingId = roomDto.BuildingId,
                FloorId = roomDto.FloorId,
                FloorNumber = roomDto.FloorNumber,
                Name = roomDto.Name,
                Type = roomDto.Type
            };

            await _roomService.CreateAsync(room);
            return CreatedAtAction(nameof(GetById), new { id = room.Id }, room);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, RoomDto roomDto)
        {
            var userId = GetUserId();
            if (!await _permissionService.UserHasPermissionAsync(userId, roomDto.BuildingId))
            {
                return StatusCode(403, "You do not have permission to edit this room.");
            }

            var room = new Room
            {
                Id = id,
                BuildingId = roomDto.BuildingId,
                FloorId = roomDto.FloorId,
                FloorNumber = roomDto.FloorNumber,
                Name = roomDto.Name,
                Type = roomDto.Type
            };

            await _roomService.UpdateAsync(id, room);
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {

            var userId = GetUserId();
            var buildingId = await _roomService.GetBuildingIdByRoomIdAsync(id);

            if (!await _permissionService.UserHasPermissionAsync(userId, buildingId.Value))
            {
                return StatusCode(403, "You do not have permission to delete this room.");
            }

            await _roomService.DeleteAsync(id);
            return NoContent();
        }

        [Authorize]
        [HttpGet("/api/buildings/{buildingId}/rooms")]
        public async Task<IActionResult> GetRoomsByBuildingId(Guid buildingId)
        {
            var rooms = await _roomService.GetRoomsByBuildingIdAsync(buildingId);
            return Ok(rooms);
        }

        [Authorize]
        [HttpGet("/api/rooms/{roomId}/smoke-statistics")]
        public async Task<IActionResult> GetSmokeStatistics(Guid roomId)
        {
            try
            {
                var averageSmokeLevel = await _roomService.GetSmokeStatisticsAsync(roomId);
                return Ok(new { AverageSmokeLevel = averageSmokeLevel });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
