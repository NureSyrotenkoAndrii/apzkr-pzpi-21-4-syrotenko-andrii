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
    public class GeoJsonFloorDataController : ControllerBase
    {
        private readonly IGeoJsonFloorDataService _geoJsonFloorDataService;
        private readonly IPermissionService _permissionService;

        public GeoJsonFloorDataController(IGeoJsonFloorDataService geoJsonFloorDataService, IPermissionService permissionService)
        {
            _geoJsonFloorDataService = geoJsonFloorDataService;
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
            var geoJsonFloorData = await _geoJsonFloorDataService.GetAllAsync();
            return Ok(geoJsonFloorData);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var geoJsonFloorData = await _geoJsonFloorDataService.GetByIdAsync(id);
            if (geoJsonFloorData == null)
            {
                return NotFound();
            }
            return Ok(geoJsonFloorData);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(GeoJsonFloorDataDto geoJsonFloorDataDto)
        {
            var geoJsonFloorData = new GeoJsonFloorData
            {
                BuildingId = geoJsonFloorDataDto.BuildingId,
                FloorNumber = geoJsonFloorDataDto.FloorNumber,
                GeojsonData = geoJsonFloorDataDto.GeojsonData
            };

            await _geoJsonFloorDataService.CreateAsync(geoJsonFloorData);
            return CreatedAtAction(nameof(GetById), new { id = geoJsonFloorData.Id }, geoJsonFloorData);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, GeoJsonFloorDataDto geoJsonFloorDataDto)
        {

            var userId = GetUserId();
            if (!await _permissionService.UserHasPermissionAsync(userId, geoJsonFloorDataDto.BuildingId))
            {
                return StatusCode(403, "You do not have permission to edit this data.");
            }

            var geoJsonFloorData = new GeoJsonFloorData
            {
                Id = id,
                BuildingId = geoJsonFloorDataDto.BuildingId,
                FloorNumber = geoJsonFloorDataDto.FloorNumber,
                GeojsonData = geoJsonFloorDataDto.GeojsonData
            };

            await _geoJsonFloorDataService.UpdateAsync(id, geoJsonFloorData);
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {

            var userId = GetUserId();
            var buildingId = await _geoJsonFloorDataService.GetBuildingIdByGeoIdAsync(id);

            if (!await _permissionService.UserHasPermissionAsync(userId, buildingId.Value))
            {
                return StatusCode(403, "You do not have permission to delete this data.");
            }

            await _geoJsonFloorDataService.DeleteAsync(id);
            return NoContent();
        }
    }
}
