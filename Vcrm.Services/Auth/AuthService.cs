using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Vcrm.Models;
using Vcrm.Models.Auth;
using Vcrm.Repositories;
using Vcrm.Services.Email;

namespace Vcrm.Services.Auth;

public class AuthService : IAuthService
{
    private const int OtpExpiryMinutes = 10;

    private readonly IUserRepository _repository;
    private readonly IEmailSender _emailSender;
    private readonly JwtSettings _jwtSettings;
    private readonly PasswordHasher<User> _passwordHasher = new();

    public AuthService(IUserRepository repository, IEmailSender emailSender, IOptions<JwtSettings> jwtSettings)
    {
        _repository = repository;
        _emailSender = emailSender;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<SignUpResponse?> SignUpAsync(SignUpRequest request)
    {
        var existingUsername = await _repository.GetByUsernameAsync(request.Username);
        if (existingUsername is not null)
        {
            return null;
        }

        var existingEmail = await _repository.GetByEmailAsync(request.Email);
        if (existingEmail is not null)
        {
            return null;
        }

        var otpCode = GenerateOtpCode();

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            Role = "User",
            CreatedAt = DateTime.UtcNow,
            IsEmailVerified = false,
            OtpCode = otpCode,
            OtpExpiresAt = DateTime.UtcNow.AddMinutes(OtpExpiryMinutes),
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        var inserted = await _repository.InsertAsync(user);

        try
        {
            await SendOtpEmailAsync(inserted.Email, otpCode);
        }
        catch
        {
            await _repository.DeleteUnverifiedUserAsync(inserted.UserId);
            throw;
        }

        return new SignUpResponse
        {
            Message = "Account created. Please check your email for a verification code.",
            Email = inserted.Email,
        };
    }

    public async Task<LoginResult> LoginAsync(LoginRequest request)
    {
        var user = await _repository.GetByUsernameAsync(request.Username);
        if (user is null)
        {
            return new LoginResult { Status = LoginStatus.InvalidCredentials };
        }

        var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            return new LoginResult { Status = LoginStatus.InvalidCredentials };
        }

        if (!user.IsEmailVerified)
        {
            return new LoginResult { Status = LoginStatus.EmailNotVerified };
        }

        return new LoginResult { Status = LoginStatus.Success, Response = BuildAuthResponse(user) };
    }

    public async Task<AuthResponse?> VerifyOtpAsync(VerifyOtpRequest request)
    {
        var user = await _repository.GetByEmailAsync(request.Email);
        if (user is null)
        {
            return null;
        }

        var verified = await _repository.VerifyEmailAsync(user.UserId, request.Code);
        if (verified is null)
        {
            return null;
        }

        return BuildAuthResponse(verified);
    }

    public async Task<bool> ResendOtpAsync(ResendOtpRequest request)
    {
        var user = await _repository.GetByEmailAsync(request.Email);
        if (user is null || user.IsEmailVerified)
        {
            return false;
        }

        var otpCode = GenerateOtpCode();
        await _repository.SetOtpAsync(user.UserId, otpCode, DateTime.UtcNow.AddMinutes(OtpExpiryMinutes));

        await SendOtpEmailAsync(user.Email, otpCode);

        return true;
    }

    private Task SendOtpEmailAsync(string email, string otpCode)
    {
        return _emailSender.SendAsync(
            email,
            "Verify your Vcrm account",
            $"Your verification code is {otpCode}. It expires in {OtpExpiryMinutes} minutes.");
    }

    private static string GenerateOtpCode()
    {
        return RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
    }

    private AuthResponse BuildAuthResponse(User user)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(ClaimTypes.Role, user.Role),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new AuthResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt = expiresAt,
            Username = user.Username,
            Role = user.Role,
        };
    }
}
