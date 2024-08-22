namespace SafeEscape.Models
{
    public class Exit
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid BuildingId { get; set; }
        public int Floor { get; set; }
        public string Type { get; set; }
        public Guid RoomId { get; set; }

        public Building Building { get; set; }
        public Room Room { get; set; }
    }
}