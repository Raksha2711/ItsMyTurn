using System.ComponentModel.DataAnnotations;

namespace srv.slots.application.DTOs.ProviderAuth;

public class ProviderLoginDto
{
    /// <summary>Mobile (10-digit) OR Email.</summary>
    [Required]
    public string Identifier { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
