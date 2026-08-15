using System.ComponentModel.DataAnnotations;

namespace Vcrm.Models;

public class User
{
    public int UserId { get; set; }

    [Required, MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required, MaxLength(20)]
    public string Role { get; set; } = "User";

    public DateTime CreatedAt { get; set; }

    public bool IsEmailVerified { get; set; }

    public string? OtpCode { get; set; }

    public DateTime? OtpExpiresAt { get; set; }
}
