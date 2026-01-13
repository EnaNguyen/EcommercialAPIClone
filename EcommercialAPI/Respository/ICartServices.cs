using EcommercialAPI.Helper;
using EcommercialAPI.Models.ViewModels.Admin;
namespace EcommercialAPI.Respository
{
    public interface ICartServices
    {
        Task<List<GetCartList>> GetCartList(string userId);
        Task<APIResponse> AddItemToCart(string userId, int productId, int quantity);
        Task<APIResponse> RemoveItemFromCart(string userId, int productId);
        Task<APIResponse> IncreaseQuantity(string userId, int productId);
        Task<APIResponse> DecreaseQuantity(string userId, int productId);
        Task<APIResponse> ChangeAmountDirect(string userId, int productId, int quantity);
    }
}
