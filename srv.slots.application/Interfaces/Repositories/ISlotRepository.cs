using srv.slots.application.DTOs.Slot;
using srv.slots.domain.Entities;

namespace srv.slots.application.Interfaces.Repositories;

public interface ISlotRepository
{
    Task<List<SlotDto>> GetByDateAsync(uint providerId, DateTime date);
    Task<bool> ExistsForDateAsync(uint providerId, DateTime date);
    Task<int> BulkCreateAsync(List<ProviderSlot> slots);
    Task<bool> ToggleActiveAsync(uint id, uint providerId);
    Task<SlotDto?> GetByIdAsync(uint id, uint providerId);
}
