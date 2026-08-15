using Vcrm.Models.Auth;

namespace Vcrm.Services.Auth;

public interface IAuthService
{
    Task<SignUpResponse?> SignUpAsync(SignUpRequest request);

    Task<LoginResult> LoginAsync(LoginRequest request);

    Task<AuthResponse?> VerifyOtpAsync(VerifyOtpRequest request);

    Task<bool> ResendOtpAsync(ResendOtpRequest request);
}
