namespace EcommercialAPI.Data.Entities
{
    public class CartDetails
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public Carts Cart { get; set; } = null!;
    }
}
