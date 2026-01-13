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
        Task<APIResponse> AdminUpdateProduct(int id, ProductEditModel productEditModel);
        Task<APIResponse> UpdateStatusProduct(int id);
        Task<APIResponse> DeleteProduct(int id);

    }
}
