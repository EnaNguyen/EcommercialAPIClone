using EcommercialAPI.Helper;
using EcommercialAPI.Data.Entities;
namespace EcommercialAPI.Respository
{
    public interface IAuthenticationServices
    {
        Task<APIResponse> Login(string username, string password);
        Task<APIResponse> TwoFALogin(string username, string otp);
        string HashCode(string code);
        string GenerateOtp();
    }
}
