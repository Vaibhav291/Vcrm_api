using Microsoft.AspNetCore.Mvc;
using Vcrm.Models.Auth;
using Vcrm.Services.Auth;

namespace Vcrm.Controllers;

[ApiController]
[Route("vcrm/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("signup")]
    public async Task<ActionResult<SignUpResponse>> SignUp(SignUpRequest request)
    {
        var result = await _authService.SignUpAsync(request);
        if (result is null)
        {
            return Conflict("Username or email is already taken.");
        }

        return Ok(result);
    }

    [HttpPost("verify-otp")]
    public async Task<ActionResult<AuthResponse>> VerifyOtp(VerifyOtpRequest request)
    {
        var result = await _authService.VerifyOtpAsync(request);
        if (result is null)
        {
            return BadRequest("Invalid or expired verification code.");
        }

        return Ok(result);
    }

    [HttpPost("resend-otp")]
    public async Task<IActionResult> ResendOtp(ResendOtpRequest request)
    {
        var sent = await _authService.ResendOtpAsync(request);
        if (!sent)
        {
            return BadRequest("Unable to resend a verification code for that email.");
        }

        return Ok(new { message = "Verification code resent." });
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        return result.Status switch
        {
            LoginStatus.Success => Ok(result.Response),
            LoginStatus.EmailNotVerified => StatusCode(StatusCodes.Status403Forbidden, "Email not verified. Please verify your email before logging in."),
            _ => Unauthorized("Invalid username or password."),
        };
    }
}
