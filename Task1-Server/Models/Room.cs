using System.Collections.Generic;

namespace SafeEscape.Models
{
    public class Room
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid BuildingId { get; set; }
        public Guid FloorId { get; set; }
        public int FloorNumber { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }

        public Building Building { get; set; }
        public Floor Floor { get; set; }
        public ICollection<Sensor> Sensors { get; set; }
        public ICollection<RoomEdge> RoomEdges1 { get; set; }
        public ICollection<RoomEdge> RoomEdges2 { get; set; }
    }
}