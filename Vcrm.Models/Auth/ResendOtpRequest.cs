using System.ComponentModel.DataAnnotations;

namespace Vcrm.Models.Auth;

public class ResendOtpRequest
{
    [Required, EmailAddress, MaxLength(150)]
    public string Email { get; set; } = string.Empty;
}
