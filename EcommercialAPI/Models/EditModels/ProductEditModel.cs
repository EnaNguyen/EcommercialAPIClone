namespace EcommercialAPI.Models.EditModels
{
    public class ProductEditModel
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string Brand { get; set; }
        public int Status { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string? Img { get; set; }
    }
}
