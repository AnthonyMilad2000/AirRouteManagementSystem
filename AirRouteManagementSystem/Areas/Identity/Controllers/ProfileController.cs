using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AirRouteManagementSystem.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Route("[Area]/[controller]")]
    [Authorize]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        [Route(nameof(GetInfo))]
        public async Task<IActionResult> GetInfo()
        {
            var user=await _userManager.GetUserAsync(User);

            if (user is null) 
                return Unauthorized();

            var userResponse=user.Adapt<ApplicationUserResponse>();

            return Ok(userResponse);
        }

        [HttpPost]
        [Route (nameof(UpdateProfile))]
        public async Task<IActionResult> UpdateProfile(ApplicationUserRequest applicationUserRequest)
        {
            var user = await _userManager.GetUserAsync(User);

            if(user is null)
                return Unauthorized();

            user.Name = applicationUserRequest.Name;
            user.Email = applicationUserRequest.Email;
            user.PhoneNumber = applicationUserRequest.PhoneNumber;
            user.Address = applicationUserRequest.Address!;

            await _userManager.UpdateAsync(user);

            return Ok(new
            {
                Success = "Update Profile Success"
            });
        }

        [HttpPost]
        [Route(nameof(UpdatePassword))]
        public async Task<IActionResult> UpdatePassword(ApplicationUserRequest applicationUserRequest)
        {
            var user=await _userManager.GetUserAsync(User);

             if (user is null)
                return Unauthorized();

            if (applicationUserRequest.CurrentPassword is null || applicationUserRequest.NewPassword is null)
                return BadRequest(new ErrorModel
                {
                    Code="Enter your current password and new password",
                    Description="Enter your current password and new password"
                });

           var result= await _userManager.ChangePasswordAsync(user,
               applicationUserRequest.CurrentPassword
               ,applicationUserRequest.NewPassword);

            if (!result.Succeeded)
                return BadRequest(result.Errors);
            else
                return Ok(new
                {
                    Success = "Update Password Success"
                });
        }


    }
}
