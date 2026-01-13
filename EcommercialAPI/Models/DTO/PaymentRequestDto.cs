namespace EcommercialAPI.Models.DTO
{
    public class PaymentRequestDto
    {
        public int CartId { get; set; }
        public string PaymentMethod { get; set; }
        public string Receiver { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
