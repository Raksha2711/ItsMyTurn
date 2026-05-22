using System.ComponentModel.DataAnnotations;

namespace srv.slots.application.DTOs.Address;

public class UpsertAddressDto
{
    [Required, MaxLength(50)]
    public string Label { get; set; } = string.Empty;

    [Required]
    public string FullAddress { get; set; } = string.Empty;

    [Required]
    public uint CityId { get; set; }

    [MaxLength(10)]
    public string? Pincode { get; set; }

    [Range(-90, 90)]
    public decimal? Latitude { get; set; }

    [Range(-180, 180)]
    public decimal? Longitude { get; set; }

    public bool IsDefault { get; set; }
}
