namespace SafeEscape.Models.DTO
{
    public class GeoJsonFloorDataDto
    {
        public Guid BuildingId { get; set; }
        public int FloorNumber { get; set; }
        public string GeojsonData { get; set; }
    }
}
