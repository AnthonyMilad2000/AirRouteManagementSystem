using AirRouteManagementSystem.Model;
using System.Security.Claims;

namespace AirRouteManagementSystem.Services.Interface
{
    public interface ITokenService
    {
        public Task<string> GenerateJwtTokenAsync(ApplicationUser user);
        public string GenerateRefreshToken();
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
