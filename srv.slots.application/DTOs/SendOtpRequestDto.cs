using System.ComponentModel.DataAnnotations;
using srv.slots.domain.Enums;

namespace srv.slots.application.DTOs.Auth;

public class SendOtpRequestDto
{
    [Required, RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Mobile must be 10 digits")]
    public string Mobile { get; set; } = string.Empty;

    [Required]
    public OtpPurpose Purpose { get; set; }
}
