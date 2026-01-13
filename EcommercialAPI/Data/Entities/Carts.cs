namespace EcommercialAPI.Data.Entities
{
    public class Carts
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public double TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<CartDetails>? CartDetails { get; set; } = new List<CartDetails>();
        public Users User { get; set; } = null!;
    }
}
