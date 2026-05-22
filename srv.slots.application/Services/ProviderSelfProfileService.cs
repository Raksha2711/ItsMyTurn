using srv.slots.application.Common.Exceptions;
using srv.slots.application.DTOs.ProviderProfile;
using srv.slots.application.Interfaces.Repositories;
using srv.slots.application.Interfaces.Services;

namespace srv.slots.application.Services;

public class ProviderSelfProfileService : IProviderSelfProfileService
{
    private readonly IProviderSelfProfileRepository _repo;

    public ProviderSelfProfileService(IProviderSelfProfileRepository repo) => _repo = repo;

    public async Task<ProviderProfileDto> GetAsync(uint providerId)
        => await _repo.GetProfileAsync(providerId) ?? throw new NotFoundException("Profile not found.");

    public async Task UpdateAsync(uint providerId, UpdateProviderProfileDto dto)
    {
        var ok = await _repo.UpdateProfileAsync(providerId, dto);
        if (!ok) throw new NotFoundException("Profile not found.");
    }
}
