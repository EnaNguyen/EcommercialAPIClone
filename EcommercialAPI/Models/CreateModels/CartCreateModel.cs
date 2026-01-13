namespace EcommercialAPI.Models.CreateModels
{
    public class CartCreateModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ReEnterPassword { get; set; }
        public string Fullname { get; set;}
        public string Phone { get; set; }
        public int Gender { get; set; }
        public DateOnly DayOfBirth { get; set; }

    }
}
