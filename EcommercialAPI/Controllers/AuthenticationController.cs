using EcommercialAPI.Respository;
using Microsoft.AspNetCore.Mvc;
namespace EcommercialAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : Controller
    {
        private readonly IAuthenticationServices _services;
        public AuthenticationController(IAuthenticationServices services)
        {
            _services = services;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(string username , string password)
        {
            var response = await _services.Login(username, password);
            return Ok(response); 
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> TwoFA(string username,string otp)
        {
            var response = await _services.TwoFALogin(username, otp);
            return Ok(response);
        }
        [HttpGet("ResentOtp")]
        public async Task<IActionResult> ResentOtp(string username)
        {
            var response = await _services.ResentOtp(username);
            return Ok(response);

        }
    }
}
