namespace EcommercialAPI.Data.Entities
{
    public class JWTAccessToken
    {
        public string Id { get; set; }
        public DateTime CreatedTime { get; set; }
        public double Expires { get; set; }
        public string UserId { get; set; }    
        public string RefreshTokenId { get; set; }
        public bool isActive { get; set; } = true;
    }
    public class RefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Token { get; set; } 
    public string UserId { get; set; }
    public Users User { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }

    public bool IsRevoked { get; set; } = false;
    public bool IsUsed { get; set; } = false;

    public string? ReplacedByTokenId { get; set; } 

    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}
}
