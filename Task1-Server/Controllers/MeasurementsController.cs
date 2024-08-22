using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafeEscape.Interfaces;
using SafeEscape.Models;
using SafeEscape.Models.DTO;
using SafeEscape.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SafeEscape.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MeasurementsController : ControllerBase
    {
        private readonly IMeasurementService _measurementService;
        private readonly ISensorService _sensorService;
        private readonly IBuildingService _buildingService;
        private readonly IRoomService _roomService;
        private readonly IRoomEdgeService _roomEdgeService;
        private readonly INotificationService _notificationService;
        private readonly IWebSocketService _webSocketService;

        public MeasurementsController(IMeasurementService measurementService, ISensorService sensorService, IRoomEdgeService roomEdgeService, INotificationService notificationService, IWebSocketService webSocketService, IRoomService roomService, IBuildingService buildingService)
        {
            _measurementService = measurementService;
            _sensorService = sensorService;
            _roomEdgeService = roomEdgeService;
            _notificationService = notificationService;
            _webSocketService = webSocketService;
            _roomService = roomService;
            _buildingService = buildingService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var measurements = await _measurementService.GetAllAsync();
            return Ok(measurements);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var measurement = await _measurementService.GetByIdAsync(id);
            if (measurement == null)
            {
                return NotFound();
            }
            return Ok(measurement);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(MeasurementDto measurementDto)
        {
            var measurement = new Measurement
            {
                SensorId = measurementDto.SensorId,
                Timestamp = measurementDto.Timestamp,
                Value = measurementDto.Value
            };

            await _measurementService.CreateAsync(measurement);
            return CreatedAtAction(nameof(GetById), new { id = measurement.Id }, measurement);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, MeasurementDto measurementDto)
        {
            var measurement = new Measurement
            {
                Id = id,
                SensorId = measurementDto.SensorId,
                Timestamp = measurementDto.Timestamp,
                Value = measurementDto.Value
            };

            await _measurementService.UpdateAsync(id, measurement);
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _measurementService.DeleteAsync(id);
            return NoContent();
        }

        [Authorize]
        [HttpGet("/api/sensors/{sensorId}/measurements")]
        public async Task<IActionResult> GetMeasurementsBySensorId(Guid sensorId)
        {
            var measurements = await _measurementService.GetMeasurementsBySensorIdAsync(sensorId);
            return Ok(measurements);
        }

        [HttpPost("receive")]
        public async Task<IActionResult> ReceiveMeasurement([FromBody] IoTMeasurementDto measurementDto)
        {
            var sensor = await _sensorService.GetByIdAsync(measurementDto.SensorId);

            if (sensor == null)
            {
                return BadRequest(new { message = "Sensor not found" });
            }

            var measurement = new Measurement
            {
                SensorId = measurementDto.SensorId,
                Value = measurementDto.Value,
                Timestamp = DateTime.UtcNow
            };

            await _measurementService.CreateAsync(measurement);

            if (measurementDto.IsAboveThreshold)
            {
                var evacuationRoute = await _roomEdgeService.GetEvacuationRouteAsync(sensor.RoomId);

                var room = await _roomService.GetByIdAsync(sensor.RoomId);
                if (room == null)
                {
                    return BadRequest(new { message = "Room not found" });
                }

                Guid buildingId = room.BuildingId;
                var users = await _buildingService.GetUsersByBuildingIdAsync(buildingId);

                foreach (var user in users)
                {
                    var webSocketMessage = new
                    {
                        RoomNamesRoute = string.Join(" -> ", evacuationRoute.RoomNames),
                        RoomIdsRoute = string.Join(" ", evacuationRoute.RoomIds)
                    };

                    var notificationMessage = new Notification
                    {
                        UserId = user.Id,
                        Message = $"Smoke detected! Evacuation route: {webSocketMessage.RoomNamesRoute}",
                        CreatedAt = DateTime.UtcNow,
                        Read = false
                    };

                    await _notificationService.CreateAsync(notificationMessage);
                    await _webSocketService.SendMessageAsync(user.Id, $"{webSocketMessage.RoomIdsRoute}");
                }

                return Ok(evacuationRoute);
            }

            return Ok();
        }


    }
}
