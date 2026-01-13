namespace EcommercialAPI.Data.Entities
{
    public class Orders
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public double TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool isPaid { get; set; }
        public DateTime? PaidAt { get; set; }
        public string PayMethod { get; set; }
        public int Status { get; set; }
        public string? Receiver { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public ICollection<OrderDetails>? OrderDetails { get; set; } = new List<OrderDetails>();
        public Users User { get; set; } = null!;
    }
}
//Status: -2: Refunded ; -1 : Cancelled ; 0: Pending ; 1: Shipping ; 2: Completed