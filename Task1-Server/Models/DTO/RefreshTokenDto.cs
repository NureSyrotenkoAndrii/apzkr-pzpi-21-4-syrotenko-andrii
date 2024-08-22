namespace SafeEscape.Models.DTO
{
    public class RefreshTokenDto
    {
        public Guid UserId { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
