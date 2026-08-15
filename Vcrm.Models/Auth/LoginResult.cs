namespace Vcrm.Models.Auth;

public enum LoginStatus
{
    Success,
    InvalidCredentials,
    EmailNotVerified,
}

public class LoginResult
{
    public LoginStatus Status { get; set; }

    public AuthResponse? Response { get; set; }
}
