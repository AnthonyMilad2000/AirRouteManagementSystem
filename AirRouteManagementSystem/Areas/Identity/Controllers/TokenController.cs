using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AirRouteManagementSystem.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Route("[Area]/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly UserManager<ApplicationUser> _userManager;

        public TokenController(ITokenService tokenService,UserManager<ApplicationUser> userManager)
        {
            _tokenService = tokenService;
            _userManager = userManager;
        }


        [HttpPost]
        [Route("Refresh")]
        public async Task<IActionResult> Refresh(TokenApiRequest tokenApiRequest)
        {
            if (tokenApiRequest is null || tokenApiRequest.RefreshToken is null || tokenApiRequest.AccessToken is null)
                return BadRequest("Invalid client request");

            string accessToken = tokenApiRequest.AccessToken;
            string refreshToken = tokenApiRequest.RefreshToken;

            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
            var username = principal.Identity!.Name;

            var user = _userManager.Users.FirstOrDefault(e => e.UserName == username);


            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                return BadRequest("Invalid client request");

            var newAccessToken =await _tokenService.GenerateJwtTokenAsync(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            await _userManager.UpdateAsync(user);
            return Ok(new
            {

                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ValidTo = "30 min",
            });
        }

        [HttpPost, Authorize(Roles =$"{SD.SuperAdminRole},{SD.AdminRole}")]
        [Route("revoke")]
        public async Task<IActionResult> Revoke()
        {
            var username = User.Identity!.Name;
            var user = _userManager.Users.SingleOrDefault(u => u.UserName == username);
            if (user == null) return BadRequest();
            user.RefreshToken = null;
            await _userManager.UpdateAsync(user);
            return NoContent();
        }
    }
}
