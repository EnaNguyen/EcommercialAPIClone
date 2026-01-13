using EcommercialAPI.Data;
using EcommercialAPI.Models.DTO;
using EcommercialAPI.Respository;
using Microsoft.AspNetCore.Mvc;

namespace EcommercialAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckOutController : ControllerBase
    {
        private readonly ICheckoutServices _service;
        private readonly ApplicationDbContext _context;

        public CheckOutController(
            ICheckoutServices service,
            ApplicationDbContext context)
        {
            _service = service;
            _context = context;
        }

        [HttpPost("process-payment")]
        public async Task<IActionResult> ProcessCODPayment([FromBody] PaymentRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Dữ liệu đầu vào không hợp lệ",
                    Errors = ModelState.Values.SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                });
            }

            var response = await _service.ProcessPaymentAsync(request, HttpContext);

            return Ok(response);
        }

        [HttpGet("vnpay-callback")]
        public async Task VnPayCallback()
        {
            var query = HttpContext.Request.Query;
            var callbackResponse = await _service.ProcessVnPayCallbackAsync(query, HttpContext);

        }
    }
}