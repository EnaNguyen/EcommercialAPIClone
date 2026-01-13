namespace EcommercialAPI.Models.ViewModels.Admin
{
    public class GetCartList
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public double TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<GetCartDetailList>? Details { get; set; }
    }
    public class GetCartDetailList
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
    }
}
