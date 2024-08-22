using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafeEscape.Interfaces;
using SafeEscape.Models.DTO;
using SafeEscape.Models;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SafeEscape.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SensorsController : ControllerBase
    {
        private readonly ISensorService _sensorService;
        private readonly IHttpClientFactory _httpClientFactory;

        public SensorsController(ISensorService sensorService, IHttpClientFactory httpClientFactory)
        {
            _sensorService = sensorService;
            _httpClientFactory = httpClientFactory;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var sensors = await _sensorService.GetAllAsync();
            return Ok(sensors);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var sensor = await _sensorService.GetByIdAsync(id);
            if (sensor == null)
            {
                return NotFound();
            }
            return Ok(sensor);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(SensorDto sensorDto)
        {
            var sensor = new Sensor
            {
                RoomId = sensorDto.RoomId,
                SensorName = sensorDto.SensorName,
                Type = sensorDto.Type,
                Threshold = sensorDto.Threshold,
                GeojsonData = sensorDto.GeojsonData
            };

            await _sensorService.CreateAsync(sensor);
            return CreatedAtAction(nameof(GetById), new { id = sensor.Id }, sensor);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, SensorDto sensorDto)
        {
            var sensor = new Sensor
            {
                Id = id,
                RoomId = sensorDto.RoomId,
                SensorName = sensorDto.SensorName,
                Type = sensorDto.Type,
                Threshold = sensorDto.Threshold,
                GeojsonData = sensorDto.GeojsonData
            };

            await _sensorService.UpdateAsync(id, sensor);
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _sensorService.DeleteAsync(id);
            return NoContent();
        }

        [Authorize]
        [HttpGet("/api/rooms/{roomId}/sensors")]
        public async Task<IActionResult> GetSensorsByRoomId(Guid roomId)
        {
            var sensors = await _sensorService.GetSensorsByRoomIdAsync(roomId);
            return Ok(sensors);
        }


        [HttpPut("{id}/update-threshold")]
        public async Task<IActionResult> UpdateThreshold(Guid id, [FromBody] float threshold)
        {
            try
            {
                await _sensorService.UpdateThresholdAsync(id, threshold);

                var sensor = await _sensorService.GetByIdAsync(id);
                if (sensor != null)
                {
                    var client = _httpClientFactory.CreateClient();
                    var updateThresholdUrl = $"http://localhost:8080/update-threshold";

                    var payload = new
                    {
                        sensor_id = id,
                        threshold = threshold
                    };
                    var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                    await client.PostAsync(updateThresholdUrl, content);
                }

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
