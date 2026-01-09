using EcommercialAPI.Data.Entities;
using EcommercialAPI.Helper;

namespace EcommercialAPI.Respository
{
    public interface IJWTUlti
    {
        string GenerateAccessToken(string userId, string username, string role);
        Task<string> GenerateRefreshTokenAsync(string userId);
        Task<string?> ValidateAndRefreshTokenAsync(string refreshToken);
        Task<(string AccessToken, string RefreshToken)> GenerateBothTokensAsync(string userId, string username, string role);
    }
}
