using EcommercialAPI.Helper;
using EcommercialAPI.Models.DTO;

namespace EcommercialAPI.Respository
{
    public interface ICheckoutServices
    {
        Task<PaymentResponse> ProcessPaymentAsync(PaymentRequestDto request, HttpContext httpContext);
        Task<PaymentResponse> ProcessVnPayCallbackAsync(IQueryCollection query, HttpContext httpContext);
    }
}
