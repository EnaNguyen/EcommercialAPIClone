namespace EcommercialAPI.Models.ViewModels.Admin
{
    public class GetOrderList
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserFullName { get; set; }
        public string Receiver {  get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public bool isPaid { get; set; }
        public DateTime CreateAt { get; set; }
        public string PayMethod { get; set; }
        public double TotalPrice { get; set; }
        public DateTime? PaidAt { get; set; }
        public int Status { get; set; }
        public List<GetOrderDetailsList>? DetailsLists { get; set; }
    }
    public class GetOrderDetailsList
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string? ProductImg {  get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
    }
}
