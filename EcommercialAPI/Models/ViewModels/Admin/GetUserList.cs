namespace EcommercialAPI.Models.ViewModels.Admin
{
    public class GetUserList
    { 
        public string Id { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateOnly CreatedAt { get; set; }
        public string Role { get; set; }
        public int Status { get; set; }
        public DateOnly DateOfBirth { get; set; } 
        public string? Img { get; set; }
        public int Gender { get; set; }
    }
}
