namespace SafeEscape.Models
{
    public class RoomEdge
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid Room1Id { get; set; }
        public Guid Room2Id { get; set; }
        public int Distance { get; set; }

        public Room Room1 { get; set; }
        public Room Room2 { get; set; }
    }
}