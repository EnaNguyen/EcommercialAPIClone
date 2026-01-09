namespace EcommercialAPI.Models.EditModels
{
    public class UserChangePasswordModel
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ReEnterNewPassword { get; set; }
    }
    public class UserInfoChangeModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public int Gender { get; set; }
        public int Status { get; set; }
        public bool TwoFA { get; set; }
        public string? Img { get; set; }
    }
}
