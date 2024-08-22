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
    public class UserBuildingsController : ControllerBase
    {
        private readonly IUserBuildingService _userBuildingService;

        public UserBuildingsController(IUserBuildingService userBuildingService)
        {
            _userBuildingService = userBuildingService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userBuilding = await _userBuildingService.GetAllAsync();
            return Ok(userBuilding);
        }

        [Authorize]
        [HttpGet("{userId}/{buildingId}")]
        public async Task<IActionResult> GetById(Guid userId, Guid buildingId)
        {
            var userBuilding = await _userBuildingService.GetByIdAsync(userId, buildingId);
            if (userBuilding == null)
            {
                return NotFound();
            }
            return Ok(userBuilding);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(UserBuildingDto userBuildingDto)
        {
            var userBuilding = new UserBuilding
            {
                UserId = userBuildingDto.UserId,
                BuildingId = userBuildingDto.BuildingId,
                IsManager = userBuildingDto.IsManager
            };

            await _userBuildingService.CreateAsync(userBuilding);
            return CreatedAtAction(nameof(GetById), new { userId = userBuilding.UserId, buildingId = userBuilding.BuildingId }, userBuilding);
        }

        [Authorize]
        [HttpPut("{userId}/{buildingId}")]
        public async Task<IActionResult> Update(Guid userId, Guid buildingId, UserBuildingDto userBuildingDto)
        {
            var userBuilding = new UserBuilding
            {
                UserId = userId,
                BuildingId = buildingId,
                IsManager = userBuildingDto.IsManager
            };

            await _userBuildingService.UpdateAsync(userId, buildingId, userBuilding);
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{userId}/{buildingId}")]
        public async Task<IActionResult> Delete(Guid userId, Guid buildingId)
        {
            await _userBuildingService.DeleteAsync(userId, buildingId);
            return NoContent();
        }

        [Authorize]
        [HttpPost("add-user")]
        public async Task<IActionResult> AddUserToBuilding(AddUserBuildingDto dto)
        {
            try
            {
                await _userBuildingService.AddUserToBuildingAsync(dto);
                return Ok(new { message = "User added to building successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
