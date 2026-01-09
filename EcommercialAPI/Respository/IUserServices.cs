using EcommercialAPI.Helper;
using EcommercialAPI.Models.CreateModels;
using EcommercialAPI.Models.ViewModels.Admin;
using EcommercialAPI.Models.EditModels;
namespace EcommercialAPI.Respository
{
    public interface IUserServices
    {
        Task<List<GetUserList>> GetListUser(string? usernameOrEmail);
        Task<APIResponse> CreateUser(UserCreateModal model, string role);
        Task<APIResponse> UpdateInfoUser(UserInfoChangeModel model, string username);
        Task<APIResponse> UpdatePasswordUser(UserChangePasswordModel model,string username);
        Task<APIResponse> ResetPasswordUser(string username);
        Task<APIResponse> ResetPasswordUserOTP(string username, string otp);
    }
}
