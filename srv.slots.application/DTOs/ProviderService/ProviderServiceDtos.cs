using System.ComponentModel.DataAnnotations;

namespace srv.slots.application.DTOs.ProviderService;

public class ProviderServiceDto
{
    public uint Id { get; set; }
    public uint? SubCategoryId { get; set; }
    public string? SubCategoryName { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DurationMins { get; set; }
    public decimal Price { get; set; }
    public string? PriceLabel { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UpsertProviderServiceDto
{
    public uint? SubCategoryId { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Range(5, 480)]
    public int DurationMins { get; set; } = 30;

    [Range(0, 999999.99)]
    public decimal Price { get; set; }

    [MaxLength(100)]
    public string? PriceLabel { get; set; }
}
