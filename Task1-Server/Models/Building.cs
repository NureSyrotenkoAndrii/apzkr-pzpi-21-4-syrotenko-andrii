using System.Text.Json.Serialization;

namespace SafeEscape.Models
{
    public class Building
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Address { get; set; }
        public int NumberOfFloors { get; set; }

        [JsonIgnore]
        public ICollection<Floor> Floors { get; set; }

        [JsonIgnore]
        public ICollection<UserBuilding> UserBuildings { get; set; }
        public ICollection<Exit> Exits { get; set; }
        public ICollection<Stair> Stairs { get; set; }
        public ICollection<GeoJsonFloorData> GeoJsonFloorData { get; set; }

    }
}