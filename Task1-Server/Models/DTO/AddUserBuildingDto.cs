namespace SafeEscape.Models.DTO
{
    public class AddUserBuildingDto
    {
        public Guid BuildingId { get; set; }
        public string UserEmail { get; set; }
        public bool IsManager { get; set; }
    }
}
