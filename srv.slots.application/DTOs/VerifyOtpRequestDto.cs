using System.ComponentModel.DataAnnotations;
using srv.slots.domain.Enums;

namespace srv.slots.application.DTOs.Auth;

public class VerifyOtpRequestDto
{
    [Required, RegularExpression(@"^[0-9]{10}$")]
    public string Mobile { get; set; } = string.Empty;

    [Required, StringLength(6, MinimumLength = 4)]
    public string OtpCode { get; set; } = string.Empty;

    [Required]
    public OtpPurpose Purpose { get; set; }
}
