namespace EcommercialAPI.Data.Entities
{
    public class Users
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role {  get; set; }
        public string Email { get; set; }   
        public string Phone { get; set; }
        public int Gender { get; set; }
        public DateOnly DayOfBirth { get; set; }
        public DateOnly CreatedAd { get; set; }
        public int Status { get; set; }
        public string? Img { get; set; }
        public bool TwoFA { get; set; } = false;
        public string? CurrentOtpCode { get; set; }              
        public DateTime? OtpExpiryTime { get; set; }

    }
}
