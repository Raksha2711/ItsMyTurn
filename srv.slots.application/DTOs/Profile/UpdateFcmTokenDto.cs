using System.ComponentModel.DataAnnotations;

namespace srv.slots.application.DTOs.Profile;

public class UpdateFcmTokenDto
{
    [Required, MaxLength(500)]
    public string FcmToken { get; set; } = string.Empty;
}
