using EcommercialAPI.Helper;
using EcommercialAPI.Models.CreateModels;
using EcommercialAPI.Models.EditModels;
using EcommercialAPI.Models.ViewModels.User.Products;

namespace EcommercialAPI.Respository
{
    public interface IProductServices
    {
        Task<APIResponse> UserViewProduct();
        Task<APIResponse> AdminAddNewProduct(ProductCreateModel productCreateModel);
        Task<APIResponse> AdminUpdateProduct(string id, ProductEditModel productEditModel);
        Task<APIResponse> UpdateStatusProduct(string id);
        Task<APIResponse> DeleteProduct(string id);

    }
}
