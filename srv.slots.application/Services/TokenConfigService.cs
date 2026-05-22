using srv.slots.application.Common.Exceptions;
using srv.slots.application.DTOs.TokenConfig;
using srv.slots.application.Interfaces.Repositories;
using srv.slots.application.Interfaces.Services;

namespace srv.slots.application.Services;

public class TokenConfigService : ITokenConfigService
{
    private readonly ITokenConfigRepository _repo;
    public TokenConfigService(ITokenConfigRepository repo) => _repo = repo;

    public async Task<TokenConfigDto> GetAsync(uint providerId)
    {
        var config = await _repo.GetByProviderAsync(providerId);
        // Return default config if none exists yet
        return config ?? new TokenConfigDto
        {
            MaxTokensPerDay = 50,
            AvgWaitPerTokenMins = 10,
            TokenHoursStart = new TimeSpan(9, 0, 0),
            TokenHoursEnd = new TimeSpan(18, 0, 0),
            AutoResetDaily = true,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public async Task UpsertAsync(uint providerId, UpsertTokenConfigDto dto)
    {
        if (dto.TokenHoursEnd <= dto.TokenHoursStart)
            throw new AppException("TokenHoursEnd must be after TokenHoursStart.");

        await _repo.UpsertAsync(providerId, dto);
    }
}
