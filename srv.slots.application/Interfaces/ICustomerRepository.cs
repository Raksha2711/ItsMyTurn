using srv.slots.domain.Entities;

namespace srv.slots.application.Interfaces.Repositories;

public interface ICustomerRepository
{
    Task<Customer?> GetByMobileAsync(string mobile);
    Task<Customer?> GetByEmailAsync(string email);
    Task<Customer?> GetByIdAsync(uint id);
    Task<bool> ExistsByMobileAsync(string mobile);
    Task<bool> ExistsByEmailAsync(string email);
    Task<uint> CreateAsync(Customer customer);
    Task UpdateLastLoginAsync(uint id);
    Task SetVerifiedAsync(uint id);
}
