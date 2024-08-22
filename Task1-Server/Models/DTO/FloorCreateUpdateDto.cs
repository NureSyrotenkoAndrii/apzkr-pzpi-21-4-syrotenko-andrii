namespace SafeEscape.Models.DTO
{
    public class FloorCreateUpdateDto
    {
        public Guid BuildingId { get; set; }
        public int FloorNumber { get; set; }
        public IFormFile PlanImage { get; set; }
    }
}