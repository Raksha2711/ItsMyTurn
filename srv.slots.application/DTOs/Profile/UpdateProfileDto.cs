using System.ComponentModel.DataAnnotations;
using srv.slots.domain.Enums;

namespace srv.slots.application.DTOs.Profile;

public class UpdateProfileDto
{
    [Required, MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    [EmailAddress, MaxLength(191)]
    public string? Email { get; set; }

    [MaxLength(500)]
    public string? ProfileImage { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public GenderType? Gender { get; set; }
}
