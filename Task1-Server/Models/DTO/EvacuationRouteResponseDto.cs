namespace SafeEscape.Models.DTO
{
    public class EvacuationRouteResponseDto
    {
        public List<Guid> RoomIds { get; set; }
        public List<string> RoomNames { get; set; }
        public int TotalDistance { get; set; }
    }
}
