using System.ComponentModel.DataAnnotations;

namespace srv.slots.application.DTOs.Auth;

public class RefreshTokenRequestDto
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
