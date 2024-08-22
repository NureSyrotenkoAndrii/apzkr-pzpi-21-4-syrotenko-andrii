namespace SafeEscape.Models.DTO
{
    public class MeasurementDto
    {
        public Guid SensorId { get; set; }
        public DateTime Timestamp { get; set; }
        public float Value { get; set; }
    }
}
