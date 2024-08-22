namespace SafeEscape.Models
{
    public class GeoJsonFloorData
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid BuildingId { get; set; }
        public int FloorNumber { get; set; }
        public string GeojsonData { get; set; }

        public Building Building { get; set; }
    }
}