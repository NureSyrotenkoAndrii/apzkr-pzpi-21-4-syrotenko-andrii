namespace SafeEscape.Models
{
    public class Measurement
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid SensorId { get; set; }
        public DateTime Timestamp { get; set; }
        public float Value { get; set; }

        public Sensor Sensor { get; set; }
    }
}