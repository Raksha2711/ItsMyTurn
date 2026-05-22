namespace srv.slots.domain.Entities;

public class ServiceSubCategory
{
    public uint Id { get; set; }
    public uint CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
}
