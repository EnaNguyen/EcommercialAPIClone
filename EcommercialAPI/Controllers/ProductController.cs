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
        [HttpGet("ProductList")]
        public async Task<IActionResult> ListSanPhamUser(string? name)
        {
            var data = await _services.UserViewProduct(name);
            return Ok(data);
        }
        [HttpPost("AddNewProduct")]
        public async Task<IActionResult> AddNewProduct(ProductCreateModel model)
        {
            var data = await _services.AdminAddNewProduct(model);
            return Ok(data);
        }
        [HttpPut("EditProduct")]
        public async Task<IActionResult> EditProduct(int id, ProductEditModel model)
        {
            var data = await _services.AdminUpdateProduct(id, model);
            return Ok(data);
        }
        [HttpPut("StatusProductChange")]
        public async Task<IActionResult> UpdateProductStatus(int id)
        {
            var data = await _services.UpdateStatusProduct(id);
            return Ok(data);
        }
        [HttpDelete("ProductRemove")]
        public async Task<IActionResult> RemoveProduct(int id)
        {
            var data= await _services.DeleteProduct(id);
            return Ok(data);
        }   
    }

}
