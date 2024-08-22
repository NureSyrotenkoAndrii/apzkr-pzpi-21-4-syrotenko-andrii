namespace SafeEscape.Models.DTO
{
    public class RoomDto
    {
        public Guid BuildingId { get; set; }
        public Guid FloorId { get; set; }
        public int FloorNumber { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
