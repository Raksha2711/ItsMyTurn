namespace srv.slots.application.DTOs.Provider;

public class ServiceDomainDto
{
    public uint Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public int SortOrder { get; set; }
    public List<ServiceCategoryDto> Categories { get; set; } = new();
}

public class ServiceCategoryDto
{
    public uint Id { get; set; }
    public uint DomainId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? Description { get; set; }
    public List<ServiceSubCategoryDto> SubCategories { get; set; } = new();
}

public class ServiceSubCategoryDto
{
    public uint Id { get; set; }
    public uint CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Icon { get; set; }
}
