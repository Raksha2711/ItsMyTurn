using Dapper;
using srv.slots.application.DTOs.Provider;
using srv.slots.application.Interfaces.Repositories;
using srv.slots.infrastructure.Persistence;

namespace srv.slots.infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly IDbConnectionFactory _factory;
    public CategoryRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<List<ServiceDomainDto>> GetAllWithCategoriesAsync()
    {
        // Three result sets in one round-trip
        const string sql = @"
            SELECT id AS Id, name AS Name, icon AS Icon, sort_order AS SortOrder
            FROM service_domains WHERE is_active = 1 ORDER BY sort_order, name;

            SELECT id AS Id, domain_id AS DomainId, name AS Name, icon AS Icon,
                   description AS Description
            FROM service_categories WHERE is_active = 1 ORDER BY sort_order, name;

            SELECT id AS Id, category_id AS CategoryId, name AS Name, icon AS Icon
            FROM service_sub_categories WHERE is_active = 1 ORDER BY sort_order, name;";

        using var conn = _factory.CreateConnection();
        using var multi = await conn.QueryMultipleAsync(sql);
        var domains = (await multi.ReadAsync<ServiceDomainDto>()).ToList();
        var cats = (await multi.ReadAsync<ServiceCategoryDto>()).ToList();
        var subs = (await multi.ReadAsync<ServiceSubCategoryDto>()).ToList();

        foreach (var cat in cats)
            cat.SubCategories = subs.Where(s => s.CategoryId == cat.Id).ToList();

        foreach (var d in domains)
            d.Categories = cats.Where(c => c.DomainId == d.Id).ToList();

        return domains;
    }

    public async Task<List<ServiceSubCategoryDto>> GetSubCategoriesByCategoryAsync(uint categoryId)
    {
        const string sql = @"
            SELECT id AS Id, category_id AS CategoryId, name AS Name, icon AS Icon
            FROM service_sub_categories
            WHERE category_id = @Id AND is_active = 1
            ORDER BY sort_order, name;";
        using var conn = _factory.CreateConnection();
        var rows = await conn.QueryAsync<ServiceSubCategoryDto>(sql, new { Id = categoryId });
        return rows.ToList();
    }
}
