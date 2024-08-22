namespace SafeEscape.Models.DTO
{
    public class ExitDto
    {
        public Guid BuildingId { get; set; }
        public int Floor { get; set; }
        public string Type { get; set; }
        public Guid RoomId { get; set; }
    }
}
