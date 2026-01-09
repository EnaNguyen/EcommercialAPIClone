namespace EcommercialAPI.Models.CreateModels
{
    public class ProductCreateModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Brand { get; set; }
        public float Price { get; set; }
        public int Quantity { get; set; }
        public int Status { get; set; }
    }
}
