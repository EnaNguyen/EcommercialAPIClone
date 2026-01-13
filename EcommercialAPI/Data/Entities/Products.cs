namespace EcommercialAPI.Data.Entities
{
    public class Products
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateOnly ReleaseDate { get;set; }
        public string Brand { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public int Status { get; set; }
        public string? Img { get; set; }
    }
}
