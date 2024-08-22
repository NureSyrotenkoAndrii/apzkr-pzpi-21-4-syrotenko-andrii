using System.Text.Json.Serialization;

namespace SafeEscape.Models
{
    public class UserBuilding
    {
        public Guid UserId { get; set; }
        public Guid BuildingId { get; set; }
        public bool IsManager { get; set; }

        public User User { get; set; }

        [JsonIgnore]
        public Building Building { get; set; }
    }
}