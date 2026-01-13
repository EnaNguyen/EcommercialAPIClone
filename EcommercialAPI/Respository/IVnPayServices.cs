using EcommercialAPI.Helper;

namespace EcommercialAPI.Respository
{
    public interface IVnPayServices
    {
        string CreatePaymentUrl(HttpContext httpContext, VnPaymentRequest request);
        VnPaymentResponse PaymentExecute(IQueryCollection collections);
    }
}
