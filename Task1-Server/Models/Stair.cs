namespace SafeEscape.Models
{
    public class Stair
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid BuildingId { get; set; }
        public Guid Floor1RoomId { get; set; }
        public Guid Floor2RoomId { get; set; }

        public Building Building { get; set; }
        public Room Floor1Room { get; set; }
        public Room Floor2Room { get; set; }
    }
}