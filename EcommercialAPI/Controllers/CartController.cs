using EcommercialAPI.Respository;
using Microsoft.AspNetCore.Mvc;

namespace EcommercialAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : Controller
    {
        private readonly ICartServices _services;
        public CartController(ICartServices services)
        {
            _services = services;
        }
        [HttpGet("GetCartList")]
        public async Task<IActionResult> GetCartList(string? id)
        {
            var data = await _services.GetCartList(id);
            return Ok(data);
        }
        [HttpPost("AddItemToCart")]
        public async Task<IActionResult> AddItemToCard(string username, int productId, int quantity)
        {
            var data = await _services.AddItemToCart(username, productId, quantity);
            return Ok(data);
        }
        [HttpDelete("RemoveCartItems")]
        public async Task<IActionResult> RemoveCartItems(string userId, int productId)
        {
            var data = await _services.RemoveItemFromCart(userId, productId);
            return Ok(data);
        }
        [HttpPut("IncreaseAmount")]
        public async Task<IActionResult> IncreaseQuantity(string userId, int productId)
        {
            var data=await _services.IncreaseQuantity(userId, productId);
            return Ok(data);
        }
        [HttpPut("DecreaseAmount")]
        public async Task<IActionResult> DecreaseQuantity(string userId, int productId)
        {
            var data = await _services.DecreaseQuantity(userId, productId);
            return Ok(data);
        }
        [HttpPut("ChangeQuantityDirect")]
        public async Task<IActionResult> ChangeQuantity(string userId, int productId, int quantity)
        {
            var data = await _services.ChangeAmountDirect(userId, productId, quantity);
            return Ok(data);
        }
    }
}
