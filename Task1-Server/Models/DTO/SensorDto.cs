namespace SafeEscape.Models.DTO
{
    public class SensorDto
    {
        public Guid RoomId { get; set; }
        public string SensorName { get; set; }
        public string Type { get; set; }
        public float Threshold { get; set; }
        public string GeojsonData { get; set; }
    }
}
