namespace SafeEscape.Models.DTO
{
    public class IoTMeasurementDto
    {
        public Guid SensorId { get; set; }
        public float Value { get; set; }
        public bool IsAboveThreshold { get; set; }
    }
}