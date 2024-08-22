using Newtonsoft.Json;

namespace SafeEscape.Models
{
    public class Floor
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid BuildingId { get; set; }
        public int FloorNumber { get; set; }
        public string PlanImageUrl { get; set; }

        [JsonIgnore]
        public Building Building { get; set; }
        public ICollection<Room> Rooms { get; set; }
    }
}