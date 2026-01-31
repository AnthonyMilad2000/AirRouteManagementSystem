using AirRouteManagementSystem.DTOs.Response;
using AirRouteManagementSystem.DTOS.Request;
using Microsoft.AspNetCore.Identity;

namespace AirRouteManagementSystem.Services.Interface
{
    public interface IAccountServices
    {
        public Task<IdentityResult> RegisterAsync(RegisterDTORequest registerDTORequest, string scheme);
        public Task<LoginResponseDTO> LoginAsync(LoginDTORequest loginDTORequest);
        public Task<bool> ConfirmEmailAsync(string token, string id);
        public Task<ResultResponse> ForgetPasswordAsync(string usernameOrEmail, CancellationToken cancellationToken);
        public Task<ResultResponse> SendEmailConfirmationAsync(SendEmailConfirmDTORequest sendEmailConfirmDTORequest, string scheme);
        public Task<ResultResponse> NewPasswordAsync(NewPasswordRequest newPasswordRequest, string userId);
        public Task<ResultResponse> ValidateOtpAsync(ValidateOtpRequest validateOtpRequest, string userId);
        public Task<LoginResponseDTO> LoginWithGoogleAsync(string idToken);
        public Task<LoginResponseDTO> LoginWithFacebookAsync(string accessToken);

    }
}
