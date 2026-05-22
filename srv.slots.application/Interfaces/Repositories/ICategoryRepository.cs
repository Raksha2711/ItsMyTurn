using srv.slots.application.DTOs.Provider;

namespace srv.slots.application.Interfaces.Repositories;

public interface ICategoryRepository
{
    Task<List<ServiceDomainDto>> GetAllWithCategoriesAsync();
    Task<List<ServiceSubCategoryDto>> GetSubCategoriesByCategoryAsync(uint categoryId);
}
