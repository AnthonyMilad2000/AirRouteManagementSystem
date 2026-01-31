using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.IdentityModel.Tokens;
using Stripe;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace AirRouteManagementSystem.Services
{
    public class AccountService : IAccountServices
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountService> _logger;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public AccountService(UserManager<ApplicationUser> userManager,
            IEmailSender emailSender,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountService> logger,
            ITokenService tokenService,
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            HttpClient httpClient)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _signInManager = signInManager;
            _logger = logger;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _httpClient = httpClient;
        }
        private string BuildConfirmEmailBody(string link)
        {
            return $@"
        <h2>Welcome to Air Route Management System</h2>
        <p>Please confirm your email by clicking the link below:</p>
        <a href='{link}'>Confirm Email</a>
        <br/><br/>
        <small>If you did not create this account, please ignore this email.</small>";
        }
        public async Task<IdentityResult> RegisterAsync(RegisterDTORequest registerDTORequest, string scheme)
        {
            var check = await _userManager.FindByEmailAsync(registerDTORequest.Email) ??
                await _userManager.FindByNameAsync(registerDTORequest.UserName);

            if (check is not null)
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "Email Or UserName Is Already Use"
                });

            var user = new ApplicationUser()
            {
                UserName = registerDTORequest.UserName,
                Email = registerDTORequest.Email,
                Address = registerDTORequest.Address,
                PhoneNumber = registerDTORequest.PhoneNumber,
            };

            var result = await _userManager.CreateAsync(user, registerDTORequest.Password);

            if (!result.Succeeded)
                return result;

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var TokenEncode = Uri.EscapeDataString(token);

            var Link = $"{scheme}://localhost:7070/Identity/Account/ConfirmEmail?token={TokenEncode}&id={user.Id}";

            await _userManager.AddToRoleAsync(user, SD.CustomerRole);


            try
            {
                await _emailSender.SendEmailAsync
              (
                 registerDTORequest.Email,
           "Air Route Management System , Confirm Email",
    BuildConfirmEmailBody(Link)
              );
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error {ex.Message}");
            }

            return result;
        }

        public async Task<LoginResponseDTO> LoginAsync(LoginDTORequest loginDTORequest)
        {
            var user = await _userManager.FindByEmailAsync(loginDTORequest.UsernameOrEmail) ??
                await _userManager.FindByNameAsync(loginDTORequest.UsernameOrEmail);

            if (user is null)
                return new LoginResponseDTO
                {
                    IsSuccess = false,
                    Status = "Invalid",
                    Message = "Invalid username or password"
                };

            var result = await _signInManager.PasswordSignInAsync(user,
                loginDTORequest.Password,
                loginDTORequest.RememberMe,
                lockoutOnFailure: true
                );

            if (result.IsLockedOut)
                return new LoginResponseDTO
                {
                    IsSuccess = false,
                    Status = "Is Locked",
                    Message = "Account locked. Try again later"
                };

            if (result.IsNotAllowed)
                return new LoginResponseDTO
                {
                    IsSuccess = false,
                    Status = "NotAllowed",
                    Message = "Please confirm your email first"
                };

            if (!result.Succeeded)
                return new LoginResponseDTO
                {
                    IsSuccess = false,
                    Status = "Invalid",
                    Message = "Invalid username or password"
                };

            var AccessToken = await _tokenService.GenerateJwtTokenAsync(user);
            var RefreshToken =  _tokenService.GenerateRefreshToken();

            user.RefreshToken = RefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _userManager.UpdateAsync(user);

            return new LoginResponseDTO
            {
                IsSuccess = true,
                Status = "Succeeded",
                Message = "Login Succeeded",
                AccessToken = AccessToken,
                RefreshToken= RefreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(30).ToString("O"),
                RefreshTokenExpiryTime=DateTime.UtcNow.AddDays(7)
            };
        }

        public async Task<bool> ConfirmEmailAsync(string token, string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return false;
            token = Uri.UnescapeDataString(token);

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
                return false;

            return true;
        }

        public async Task<ResultResponse> SendEmailConfirmationAsync(SendEmailConfirmDTORequest sendEmailConfirmDTORequest, string scheme)
        {
            var user = await _userManager.FindByEmailAsync(sendEmailConfirmDTORequest.usernameOrEmail) ??
                await _userManager.FindByNameAsync(sendEmailConfirmDTORequest.usernameOrEmail);

            if (user == null)
                return new ResultResponse
                {
                    Stutas = false,
                    Message = "Email Or UserName Not Found"
                };

            if (user.EmailConfirmed)
                return new ResultResponse
                {
                    Stutas = true,
                    Message = "Email is Aready Confirmed"
                };

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var TokenEncode = Uri.EscapeDataString(token);

            var Link = $"{scheme}://localhost:7070/Identity/Account/ConfirmEmail?token={TokenEncode}&id={user.Id}";

            try
            {
                await _emailSender.SendEmailAsync(
                 user.Email!,
                 "Air Route Management System , Confirm Email",
                 BuildConfirmEmailBody(Link));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending confirmation email to {Email}", user.Email);
            }

            return new ResultResponse
            {
                Stutas = true,
                Message = "Confirmation email has been sent, please check your inbox!"
            };
        }

        public async Task<ResultResponse> ForgetPasswordAsync(string usernameOrEmail, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(usernameOrEmail) ??
                await _userManager.FindByNameAsync(usernameOrEmail);

            if (user == null)
                return new ResultResponse
                {
                    Stutas = false,
                    Message = "Email Or UserName Not Found"
                };

            var UserOtp = await _unitOfWork.ApplicationUserOTPrepository
                .GetAsync(x => x.ApplicationUserId == user.Id);

            var totelOtp = UserOtp.Count(e => (DateTime.UtcNow - e.CreateAt).TotalHours < 24);

            if (totelOtp > 3)
                return new ResultResponse
                {
                    Stutas = false,
                    Message = "You tried too many attempts. Try again after 1 day."
                };

            var OTP = new Random().Next(100000, 999999).ToString();

            var userOtp = new ApplicationUserOTP()
            {
                ApplicationUserId = user.Id,
                CreateAt = DateTime.UtcNow,
                ValidTo = DateTime.UtcNow.AddMinutes(30),
                Isvalid = true,
                OtpCode = OTP
            };

            await _unitOfWork.ApplicationUserOTPrepository.CreateAsync(userOtp, cancellationToken);

            await _unitOfWork.Commit();

            try
            {
                await _emailSender.SendEmailAsync(user.Email!, "Air Route Management System , otp number",
             $@"
        <h2>Welcome to Air Route Management System</h2>
        <p>Your otp number is <strong>{OTP}</strong></p>
        <p>Please dont share otp number:</p>"
        );
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex, "Error sending OTP email");
            }

            return new ResultResponse
            {
                Stutas = true,
                Message = "Check in box for otp number",
                UserId = user.Id,
            };

        }

        public async Task<ResultResponse> ValidateOtpAsync(ValidateOtpRequest validateOtpRequest,string userId)
        {
            var otpNumbers = await _unitOfWork.ApplicationUserOTPrepository
                .GetAsync(x =>
                x.ApplicationUserId == userId &&
                x.ValidTo > DateTime.UtcNow &&
                x.Isvalid);

            if (!otpNumbers.Any())
                return new ResultResponse
                {
                    Stutas = false,
                    Message = "Not valid otp number form user"
                };

            var matchNumber=otpNumbers.FirstOrDefault(x=>x.OtpCode== validateOtpRequest.OTP);

            if(matchNumber is null)
                return new ResultResponse
                {
                    Stutas = false,
                    Message = "Not valid otp number"
                };

            matchNumber.Isvalid = false;
            await _unitOfWork.Commit();

            return new ResultResponse
            {
                Stutas = true,
                Message = "Otp is valid",
                UserId= userId
            };
        }

        public async Task<ResultResponse> NewPasswordAsync(NewPasswordRequest newPasswordRequest,string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return new ResultResponse
                {
                    Stutas = false,
                    Message = "User Not Found"
                };
            var token=await _userManager.GeneratePasswordResetTokenAsync(user);

           var result=await _userManager.ResetPasswordAsync(user, token,newPasswordRequest.Newpassword);

            if (!result.Succeeded)
                return new ResultResponse
                {
                    Stutas = false,
                    Message = string.Join(" | ", result.Errors.Select(e => e.Description))
                };

            var userOtp = await _unitOfWork.ApplicationUserOTPrepository
                .GetAsync(x => x.ApplicationUserId == userId &&
                x.Isvalid);

            foreach(var otp in userOtp)
                otp.Isvalid = false;

            await _unitOfWork.Commit();

            return new ResultResponse
            {
                Stutas = true,
                Message = "Password Reset Success"
            };
        }

        public async Task<LoginResponseDTO> LoginWithGoogleAsync(string idToken)
        {
            GoogleJsonWebSignature.Payload payload;

            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(
                    idToken,
                    new GoogleJsonWebSignature.ValidationSettings
                    {
                        Audience = new[]
                        {
                    _configuration["Authentication:Google:ClientId"]
                        }
                    });
            }
            catch
            {
                return new LoginResponseDTO
                {
                    IsSuccess = false,
                    Message = "Invalid Google token"
                };
            }

            var email = payload.Email;

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                await _userManager.CreateAsync(user);
            }

            var AccessjwtToken = await _tokenService.GenerateJwtTokenAsync(user);
            var RefreshjwtToken =  _tokenService.GenerateRefreshToken();

            user.RefreshToken = RefreshjwtToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            return new LoginResponseDTO
            {
                IsSuccess = true,
                Message = "Login with Google successful",
                AccessToken = AccessjwtToken,
                RefreshToken = RefreshjwtToken,
                RefreshTokenExpiryTime = user.RefreshTokenExpiryTime,
                ExpiresAt = DateTime.UtcNow.AddMinutes(30).ToString()
            };
        }

        public async Task<LoginResponseDTO> LoginWithFacebookAsync(string accessToken)
        {
            var url =
                $"https://graph.facebook.com/me?fields=id,name,email&access_token={accessToken}";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return new LoginResponseDTO
                {
                    IsSuccess = false,
                    Message = "Invalid Facebook token"
                };

            var json = await response.Content.ReadAsStringAsync();
            var fbUser = JsonSerializer.Deserialize<FacebookUserDto>(json);

            if (fbUser?.Email == null)
                return new LoginResponseDTO
                {
                    IsSuccess = false,
                    Message = "Facebook account has no email"
                };

            var user = await _userManager.FindByEmailAsync(fbUser.Email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = fbUser.Email,
                    Email = fbUser.Email,
                    EmailConfirmed = true
                };

                await _userManager.CreateAsync(user);
            }

           
            var Accesstoken = await _tokenService.GenerateJwtTokenAsync(user);
            var RefreshjwtToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = RefreshjwtToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            return new LoginResponseDTO
            {
                IsSuccess = true,
                Message = "Login with Facebook successful",
                AccessToken = Accesstoken,
                RefreshToken= RefreshjwtToken,
                RefreshTokenExpiryTime = user.RefreshTokenExpiryTime,
                ExpiresAt = DateTime.UtcNow.AddMinutes(30).ToString()
            };
        }
    }
}
