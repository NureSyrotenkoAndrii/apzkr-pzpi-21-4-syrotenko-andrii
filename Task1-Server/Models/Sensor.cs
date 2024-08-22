namespace SafeEscape.Models
{
    public class Sensor
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid RoomId { get; set; }
        public string SensorName { get; set; }
        public string Type { get; set; }
        public float Threshold { get; set; }
        public string GeojsonData { get; set; }

        public Room Room { get; set; }
        public ICollection<Measurement> Measurements { get; set; }
    }
}