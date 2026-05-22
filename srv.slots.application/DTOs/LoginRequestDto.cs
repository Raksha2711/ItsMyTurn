using System.ComponentModel.DataAnnotations;

namespace srv.slots.application.DTOs.Auth;

public class LoginRequestDto
{
    [Required, RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Mobile must be 10 digits")]
    public string Mobile { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
