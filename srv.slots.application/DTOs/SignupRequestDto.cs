using System.ComponentModel.DataAnnotations;

namespace srv.slots.application.DTOs.Auth;

public class SignupRequestDto
{
    [Required, MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    [EmailAddress, MaxLength(191)]
    public string? Email { get; set; }

    [Required, RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Mobile must be 10 digits")]
    public string Mobile { get; set; } = string.Empty;

    [Required, MinLength(6), MaxLength(50)]
    public string Password { get; set; } = string.Empty;
}
