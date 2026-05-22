using srv.slots.application.DTOs.Slot;

namespace srv.slots.application.Interfaces.Services;

public interface ISlotManagementService
{
    Task<List<SlotDto>> GetByDateAsync(uint providerId, DateTime date);
    Task<GenerateSlotsResultDto> GenerateAsync(uint providerId, GenerateSlotsDto dto);
    Task ToggleSlotActiveAsync(uint slotId, uint providerId);
}
