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
    public class RoomEdgesController : ControllerBase
    {
        private readonly IRoomEdgeService _roomEdgeService;

        public RoomEdgesController(IRoomEdgeService roomEdgeService)
        {
            _roomEdgeService = roomEdgeService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var roomEdges = await _roomEdgeService.GetAllAsync();
            return Ok(roomEdges);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var roomEdge = await _roomEdgeService.GetByIdAsync(id);
            if (roomEdge == null)
            {
                return NotFound();
            }
            return Ok(roomEdge);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(RoomEdgeDto roomEdgeDto)
        {
            var roomEdge = new RoomEdge
            {
                Room1Id = roomEdgeDto.Room1Id,
                Room2Id = roomEdgeDto.Room2Id,
                Distance = roomEdgeDto.Distance
            };

            await _roomEdgeService.CreateAsync(roomEdge);
            return CreatedAtAction(nameof(GetById), new { id = roomEdge.Id }, roomEdge);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, RoomEdgeDto roomEdgeDto)
        {
            var roomEdge = new RoomEdge
            {
                Id = id,
                Room1Id = roomEdgeDto.Room1Id,
                Room2Id = roomEdgeDto.Room2Id,
                Distance = roomEdgeDto.Distance
            };

            await _roomEdgeService.UpdateAsync(id, roomEdge);
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _roomEdgeService.DeleteAsync(id);
            return NoContent();
        }

        [Authorize]
        [HttpGet("{roomId}/evacuation-route")]
        public async Task<IActionResult> GetEvacuationRoute(Guid roomId)
        {
            var route = await _roomEdgeService.GetEvacuationRouteAsync(roomId);
            return Ok(route);
        }
    }
}
