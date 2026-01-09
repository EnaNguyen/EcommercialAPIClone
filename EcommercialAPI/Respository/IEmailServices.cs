using EcommercialAPI.Helper;

namespace EcommercialAPI.Respository
{
    public interface IEmailServices
    {
        Task<APIResponse> SendEmail(string email, string otp, string request, string type);
    }
}
