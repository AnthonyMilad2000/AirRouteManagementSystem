using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AirRouteManagementSystem.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Route("[Area]/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        
        private readonly IAccountServices _accountServices;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(IAccountServices accountServices,
            UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager)
        {
            _accountServices = accountServices;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterDTORequest registerDTORequest)
        {
            var result = await _accountServices.RegisterAsync(registerDTORequest, Request.Scheme);

            if (!result.Succeeded)
                return BadRequest(result.Errors);
            return CreatedAtAction(nameof(Login), new
            {
                msg = "Please check your email to confirm your account."
            });
        }
        [HttpPost]
        [Route(nameof(Login))]
        public async Task<IActionResult> Login(LoginDTORequest loginDTORequest)
        {

            var result = await _accountServices.LoginAsync(loginDTORequest);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(new
            {
                msg= result.Message,
                AccessToken = result.AccessToken,
                validTo=result.ExpiresAt,
                RefreshToken=result.RefreshToken,
                RefreshTokenVaildTo=result.RefreshTokenExpiryTime
            });
        }

        [HttpGet]
        [Route("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token,string id)
        {
            var result=await _accountServices.ConfirmEmailAsync(token, id);

            if (!result)
                return BadRequest(new ErrorModel
                {
                    Code= "Invalid or Expired Token! ",
                    Description= "Invalid or Expired Token!"
                });
            return CreatedAtAction(nameof(Login),new
            {
                msg="Email Confirmed Success"
            });
        }

        [HttpPost]
        [Route("SendEmailConfirmation")]
        public async Task<IActionResult> SendEmailConfirmation(SendEmailConfirmDTORequest sendEmailConfirmDTORequest)
        {
            var result=await _accountServices.SendEmailConfirmationAsync(sendEmailConfirmDTORequest,Request.Scheme);

            if(!result.Stutas)
                return BadRequest(new ErrorModel
                {
                    Code = result.Message,
                    Description = result.Message
                });

            return CreatedAtAction(nameof(Login), new
            {
                Success = result.Message
            });
        }
        [HttpPost]
        [Route("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordRequest forgetPasswordRequest, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(forgetPasswordRequest.UsernameOrEmail))
                return BadRequest(new ErrorModel
                {
                    Code= "Please enter email or username",
                    Description= "Please enter email or username"
                });

            var result=await _accountServices.ForgetPasswordAsync(forgetPasswordRequest.UsernameOrEmail, cancellationToken);

            if (!result.Stutas)
                return BadRequest(new ErrorModel
                {
                    Code = result.Message,
                    Description = result.Message
                });

            return CreatedAtAction(nameof(ValidateOtp),
                new { userId = result.UserId},
                new
                {
                    Message = result.Message.ToString()
                });
        }
        [HttpPost]
        [Route("ValidateOtp/{userId}")]
        public async Task<IActionResult> ValidateOtp([FromRoute] string userId, [FromBody] ValidateOtpRequest validateOtpRequest)
        {

            var result = await _accountServices.ValidateOtpAsync(validateOtpRequest, userId);

            if (!result.Stutas)
                return BadRequest(new ErrorModel
                {
                    Code = result.Message,
                    Description = result.Message
                });

            return CreatedAtAction(nameof(NewPassword),
                new {userId=result.UserId},
                new
                {
                    Message = result.Message.ToString()
                });
        }

        [HttpPost]
        [Route("NewPassword/{userId}")]
        public async Task<IActionResult> NewPassword([FromRoute] string userId, [FromBody] NewPasswordRequest newPasswordRequest)
        {
            var result=await _accountServices.NewPasswordAsync(newPasswordRequest, userId);

            if (!result.Stutas)
                return BadRequest(new ErrorModel
                {
                    Code = result.Message,
                    Description = result.Message
                });


            return Ok(new 
            {
                Message = result.Message.ToString()
            });
        }

        [HttpPost]
        [Route("LoginWithGoogle")]
        public async Task<IActionResult> LoginWithGoogle(
         [FromBody] GoogleLoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.IdToken))
                return BadRequest("Google token is required");

            var result = await _accountServices.LoginWithGoogleAsync(request.IdToken);

            if (!result.IsSuccess)
                return BadRequest(new ErrorModel
                {
                    Code = "GoogleLoginFailed",
                    Description = result.Message!
                });

            return Ok(new
            {
                AccessToken = result.AccessToken,
                ExpiresAt = result.ExpiresAt,
                Message = result.Message,
                RefreshToken= result.RefreshToken,
                RefreshTokenValidTo=result.RefreshTokenExpiryTime
            });
        }


        [HttpPost]
        [Route("LoginWithFacebook")]
        public async Task<IActionResult> LoginWithFacebook(
    [FromBody] FacebookLoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.AccessToken))
                return BadRequest("Facebook access token is required");

            var result = await _accountServices.LoginWithFacebookAsync(request.AccessToken);

            if (!result.IsSuccess)
                return BadRequest(new ErrorModel
                {
                    Code = "FacebookLoginFailed",
                    Description = result.Message!
                });

            return Ok(new
            {
                AccessToken = result.AccessToken,
                ExpiresAt = result.ExpiresAt,
                Message = result.Message,
                RefreshToken = result.RefreshToken,
                RefreshTokenValidTo = result.RefreshTokenExpiryTime
            });
        }



    }
}
