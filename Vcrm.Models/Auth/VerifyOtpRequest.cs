using System.ComponentModel.DataAnnotations;

namespace Vcrm.Models.Auth;

public class VerifyOtpRequest
{
    [Required, EmailAddress, MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required, MaxLength(10)]
    public string Code { get; set; } = string.Empty;
}
