namespace srv.slots.domain.Entities;

public class ProviderService
{
    public uint Id { get; set; }
    public uint ProviderId { get; set; }
    public uint? SubCategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DurationMins { get; set; } = 30;
    public decimal Price { get; set; }
    public string? PriceLabel { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
