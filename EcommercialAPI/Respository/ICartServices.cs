using EcommercialAPI.Helper;
using EcommercialAPI.Models.ViewModels.Admin;
namespace EcommercialAPI.Respository
{
    public interface ICartServices
    {
        Task<List<GetCartList>> GetCartList(string? username);
        Task<APIResponse> AddItemToCart(string userId, int productId, int quantity);
        Task<APIResponse> RemoveItemFromCart(string username, int productId);
        Task<APIResponse> IncreaseQuantity(string username, int productId);
        Task<APIResponse> DecreaseQuantity(string username, int productId);
        Task<APIResponse> ChangeAmountDirect(string username, int productId, int quantity);
    }
}
