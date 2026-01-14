using EcommercialAPI.Models.CreateModels;
using EcommercialAPI.Models.EditModels;
using EcommercialAPI.Respository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommercialAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserServices _services;
        public UserController(IUserServices services)
        {
            _services = services;
        }
        [HttpGet("ListUser")]
        public async Task<IActionResult> GetListUser(string? username)
        {
            var data = await _services.GetListUser(username);
            return Ok(data);
        }
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateNewUser(UserCreateModal model)
        {
            var data=  await _services.CreateUser(model);
            return Ok(data);
        }
        [HttpPost("UpdateUserInfo")]
        public async Task<IActionResult> UpdateUserInfo(UserInfoChangeModel model,string username)
        {
            var data= await _services.UpdateInfoUser(model,username);
            return Ok(data);
        }
        [HttpPost("ResetPasswordRequest")]
        public async Task<IActionResult> ResetPasswordUser(string username)
        {
            var data = await _services.ResetPasswordUser(username);
            return Ok(data);
        }
        [HttpPost("ResetPasswordOTP")]
        public async Task<IActionResult> ResetPasswordUserOTP(string username, string otp)
        {
            var data = await _services.ResetPasswordUserOTP(username, otp);
            return Ok(data);
        }
        
    }
}
