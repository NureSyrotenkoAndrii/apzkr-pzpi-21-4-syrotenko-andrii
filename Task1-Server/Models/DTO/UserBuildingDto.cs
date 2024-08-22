namespace SafeEscape.Models.DTO
{
    public class UserBuildingDto
    {
        public Guid UserId { get; set; }
        public Guid BuildingId { get; set; }
        public bool IsManager { get; set; }
    }
}
