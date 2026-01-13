using EcommercialAPI.Respository;
using Microsoft.AspNetCore.Mvc;

namespace EcommercialAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : Controller
    {
        private readonly IOrderServices _services;
        public OrderController(IOrderServices services)
        {
            _services = services;
        }
        [HttpGet("GetOrderList")]
        public async Task<IActionResult> GetOrderList(string? Username)
        {
            var data = await _services.GetOrderList(Username);
            return Ok(data);
        }
        [HttpPut("CancelOrder")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var data = await _services.CancelOrder(id);
            return Ok(data);
        }
        [HttpPut("AcceptOrder")]
        public async Task<IActionResult> AcceptOrder(int id)
        {
            var data = await _services.AcceptOrder(id);
            return Ok(data);
        }
        [HttpPut("PaidOrderCOD")]
        public async Task<IActionResult> PaidOrderCOD(int id)
        {
            var data = await _services.PaidOrderCOD(id);
            return Ok(data);
        }
        [HttpPut("ReceivedOrderVisa")]
        public async Task<IActionResult> ReceivedOrderVisa(int id)
        {
            var data = await _services.ReceivedOrderVisa(id);
            return Ok(data);
        }
        [HttpPut("RefundOrder")]
        public async Task<IActionResult> RefundOrder(int id)
        {
            var data = await _services.RefundOrder(id);
            return Ok(data);
        }
    }
}
