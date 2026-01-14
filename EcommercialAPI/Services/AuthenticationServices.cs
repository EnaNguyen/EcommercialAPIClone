using EcommercialAPI.Data;
using EcommercialAPI.Data.Entities;
using EcommercialAPI.Helper;
using EcommercialAPI.Respository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;
namespace EcommercialAPI.Services
{
    public class    AuthenticationServices : IAuthenticationServices
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuthenticationServices> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IJWTUlti _jWTUlti;
        private readonly IEmailServices _emailServices;
        public AuthenticationServices(
            ApplicationDbContext context,
            IJWTUlti jWTUlti,
            IEmailServices emailServices,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AuthenticationServices> logger)
        {
            _context = context;
            _jWTUlti = jWTUlti;
            _emailServices = emailServices;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }
        public async Task<APIResponse> Login(string username, string password)
        {
            try
            {
                var userAttempLogin = _context.Users.FirstOrDefault(p => p.Username == username || p.Email==username);
                if (userAttempLogin != null)
                {
                    string AfterHash = HashCode(password);
                    if(AfterHash == userAttempLogin.Password)
                    {
                        if (userAttempLogin.TwoFA==true)
                        {
                            string otp = GenerateOtp();
                            userAttempLogin.CurrentOtpCode = HashCode(otp);
                            userAttempLogin.OtpExpiryTime = DateTime.UtcNow.AddMinutes(5);

                            await _context.SaveChangesAsync();

                            var emailResult = await _emailServices.SendEmail(userAttempLogin.Email, otp, "Xác thực 2 yếu tố qua Email", "OTP");
                            if (emailResult.ResponseCode != 200)
                            {
                                return new APIResponse { ResponseCode = 500, Result = "Failed to send OTP" };
                            }
                            return new APIResponse
                            {
                                ResponseCode = 200,
                                Result = "2FA required",
                                Data = new LoginRequires2FAResponse
                                {
                                    Message = "OTP sent to your email",
                                    Requires2FA = true
                                }
                            };
                        }
                        return await CompleteLoginAndGenerateTokens(userAttempLogin);
                    }
                    return new APIResponse
                    {
                        ResponseCode = 404,
                        Result = "Can't Login",
                        ErrorMessage = "Wrong Username or Password"
                    };
                }
                return new APIResponse
                {
                    ResponseCode = 404,
                    Result = "Not Found",
                    ErrorMessage = "This User Has not been Registered"
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    ResponseCode = 500,
                    Result = "Disconnected",
                    ErrorMessage = "Can't login right now"
                };
            }

        }

        public async Task<APIResponse> TwoFALogin(string username, string otp)
        {
            var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                return new APIResponse { ResponseCode = 404, Result = "User not found" };
            }
            if (user.OtpExpiryTime < DateTime.UtcNow ||
                string.IsNullOrEmpty(user.CurrentOtpCode) ||
                user.CurrentOtpCode != HashCode(otp))
            {
                return new APIResponse
                {
                    ResponseCode = 400,
                    Result = "Invalid or expired OTP"
                };
            }
            user.CurrentOtpCode = null;
            user.OtpExpiryTime = null;
            await _context.SaveChangesAsync();  
            return await CompleteLoginAndGenerateTokens(user);
        }
        public string HashCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return string.Empty;
            using var sha256 = SHA256.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(code);
            byte[] hashBytes = sha256.ComputeHash(bytes);
            return Convert.ToHexString(hashBytes).ToLowerInvariant();
        }
        public string GenerateOtp()
        {
            return new Random().Next(100000, 999999).ToString(); 
        }
        private async Task<APIResponse> CompleteLoginAndGenerateTokens(Users user)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return new APIResponse
                {
                    ResponseCode = 500,
                    Result = "Cannot access HttpContext"
                };
            }

            var (accessToken, refreshToken) = await _jWTUlti.GenerateBothTokensAsync(
                user.Id, user.Username, user.Role ?? "User");

            return new APIResponse
            {
                ResponseCode = 200,
                Result = "Login successfully",
                Data = new
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    Username = user.Username,
                    Role = user.Role
                }
            };
        }

        public async Task<APIResponse> ResentOtp(string username)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var account = _context.Users.FirstOrDefault(u => u.Username == username||u.Email==username);
                if (account != null)
                {
                    string otp = GenerateOtp();
                    account.CurrentOtpCode = HashCode(otp);
                    account.OtpExpiryTime = DateTime.UtcNow.AddMinutes(5);
                    await _context.SaveChangesAsync();
                    var emailResult = await _emailServices.SendEmail(account.Email, otp, "Xác thực 2 yếu tố qua Email", "OTP");
                    if (emailResult.ResponseCode != 200)
                    {
                        transaction.Rollback();
                        return new APIResponse { ResponseCode = 500, Result = "Failed to send OTP" };
                    }
                    transaction.Commit();
                    return new APIResponse
                    {
                        ResponseCode = 200,
                        Result = "OTP resent successfully"
                    };
                }    
                return new APIResponse
                {
                    ResponseCode = 404,
                    Result = "Not Found",
                    ErrorMessage = "User not found"
                };
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError(ex, "Error resending OTP for user {Username}", username);
                return new APIResponse
                {
                    ResponseCode = 500,
                    Result = "Error",
                    ErrorMessage = "An error occurred while resending OTP."
                };
            }
        }
    }
}
