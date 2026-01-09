using Microsoft.AspNetCore.Mvc;
using EcommercialAPI.Respository;
using EcommercialAPI.Models.CreateModels;
using EcommercialAPI.Models.EditModels;
namespace EcommercialAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : Controller
    {
        private readonly IProductServices _services;
        public ProductController(IProductServices services)
        {   
            _services = services;
        }
        [HttpGet("/User/ProductList")]
        public async Task<IActionResult> ListSanPhamUser()
        {
            var data = await _services.UserViewProduct();
            return Ok(data);
        }
        [HttpPost("/User/AddNewProduct")]
        public async Task<IActionResult> AddNewProduct(ProductCreateModel model)
        {
            var data = await _services.AdminAddNewProduct(model);
            return Ok(data);
        }
        [HttpPut("/User/EditProduct")]
        public async Task<IActionResult> EditProduct(string id, ProductEditModel model)
        {
            var data = await _services.AdminUpdateProduct(id, model);
            return Ok(data);
        }
        [HttpPut("/User/StatusProductChange")]
        public async Task<IActionResult> UpdateProductStatus(string id)
        {
            var data = await _services.UpdateStatusProduct(id);
            return Ok(data);
        }
        [HttpDelete("/User/ProductRemove")]
        public async Task<IActionResult> RemoveProduct(string id)
        {
            var data= await _services.DeleteProduct(id);
            return Ok(data);
        }   
    }

}
