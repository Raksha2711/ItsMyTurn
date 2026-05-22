using srv.slots.application.Common.Exceptions;
using srv.slots.application.DTOs.ProviderAuth;
using srv.slots.application.Interfaces.Repositories;
using srv.slots.application.Interfaces.Services;
using srv.slots.domain.Entities;
using srv.slots.domain.Enums;

namespace srv.slots.application.Services;

public class ProviderAuthService : IProviderAuthService
{
    private readonly IServiceProviderRepository _providerRepo;
    private readonly IProviderResubmissionRepository _resubmissionRepo;
    private readonly IOtpRepository _otpRepo;
    private readonly IRefreshTokenRepository _refreshRepo;
    private readonly ILookupRepository _lookupRepo;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenService _jwt;

    public ProviderAuthService(
        IServiceProviderRepository providerRepo,
        IProviderResubmissionRepository resubmissionRepo,
        IOtpRepository otpRepo,
        IRefreshTokenRepository refreshRepo,
        ILookupRepository lookupRepo,
        IPasswordHasher hasher,
        IJwtTokenService jwt)
    {
        _providerRepo = providerRepo;
        _resubmissionRepo = resubmissionRepo;
        _otpRepo = otpRepo;
        _refreshRepo = refreshRepo;
        _lookupRepo = lookupRepo;
        _hasher = hasher;
        _jwt = jwt;
    }

    public async Task<ProviderAuthResponseDto> SignupAsync(ProviderSignupDto dto, string? ip, string? device)
    {
        // Uniqueness checks
        if (await _providerRepo.ExistsByMobileAsync(dto.Mobile))
            throw new ConflictException("Mobile number already registered.");

        if (await _providerRepo.ExistsByEmailAsync(dto.Email.Trim().ToLower()))
            throw new ConflictException("Email already registered.");

        // Foreign-key validations
        await ValidateLookupsAsync(dto.DomainId, dto.CategoryId, dto.CityId, dto.BoundaryId);

        var entity = new ServiceProviderEntity
        {
            FirmName = dto.FirmName.Trim(),
            OwnerName = dto.OwnerName.Trim(),
            Email = dto.Email.Trim().ToLower(),
            Mobile = dto.Mobile,
            WhatsappNumber = dto.WhatsappNumber,
            PasswordHash = _hasher.Hash(dto.Password),
            LogoUrl = dto.LogoUrl,
            DomainId = dto.DomainId,
            CategoryId = dto.CategoryId,
            Specialization = dto.Specialization,
            Description = dto.Description,
            FeesStructure = dto.FeesStructure,
            Languages = dto.Languages,
            FullAddress = dto.FullAddress.Trim(),
            CityId = dto.CityId,
            Pincode = dto.Pincode,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            BoundaryId = dto.BoundaryId,
            ServiceType = dto.ServiceType,
            AvgServiceDurationMins = dto.AvgServiceDurationMins,
            MaxCapacityPerDay = dto.MaxCapacityPerDay,
            AvgWaitPerTokenMins = dto.AvgWaitPerTokenMins,
            PlanId = 1,                  // default Free plan
            Status = ProviderStatus.Pending,
            IsVerified = false,
            IsActive = true,
            IsTerminated = false
        };

        var id = await _providerRepo.CreateAsync(entity);

        // Signup does NOT issue tokens — provider must wait for approval
        return new ProviderAuthResponseDto
        {
            ProviderId = id,
            FirmName = entity.FirmName,
            OwnerName = entity.OwnerName,
            Email = entity.Email,
            Mobile = entity.Mobile,
            Status = ProviderStatus.Pending,
            Message = "Signup successful. Please wait for Super Admin approval before logging in."
        };
    }

    public async Task<ProviderAuthResponseDto> LoginAsync(ProviderLoginDto dto, string? ip, string? device)
    {
        // Identifier may be mobile or email
        var provider = await ResolveProviderAsync(dto.Identifier)
            ?? throw new UnauthorizedException("Invalid credentials.");

        if (provider.IsTerminated)
            throw new UnauthorizedException("Account has been terminated. Reason: " + (provider.TerminatedReason ?? "Contact support."));

        if (!provider.IsActive)
            throw new UnauthorizedException("Account is inactive. Contact support.");

        if (!_hasher.Verify(dto.Password, provider.PasswordHash))
            throw new UnauthorizedException("Invalid credentials.");

        // Status gate — only Approved can login
        if (provider.Status != ProviderStatus.Approved)
        {
            var msg = provider.Status switch
            {
                ProviderStatus.Pending => "Your account is pending Super Admin approval.",
                ProviderStatus.Rejected => $"Your application was rejected. Reason: {provider.RejectionReason ?? "Not specified."}",
                ProviderStatus.Returned => $"Your application requires correction. Reason: {provider.ReturnReason ?? "Not specified."} Please resubmit.",
                ProviderStatus.Suspended => "Your account has been suspended. Contact support.",
                _ => "Your account is not in an approvable state."
            };

            return new ProviderAuthResponseDto
            {
                ProviderId = provider.Id,
                FirmName = provider.FirmName,
                OwnerName = provider.OwnerName,
                Email = provider.Email,
                Mobile = provider.Mobile,
                Status = provider.Status,
                RejectionReason = provider.RejectionReason,
                ReturnReason = provider.ReturnReason,
                Message = msg
            };
        }

        await _providerRepo.UpdateLastLoginAsync(provider.Id);
        return await IssueTokensAsync(provider, ip, device);
    }

    public async Task<string> SendOtpAsync(ProviderSendOtpDto dto)
    {
        if (dto.Purpose == OtpPurpose.Login || dto.Purpose == OtpPurpose.ForgotPassword)
        {
            var exists = await _providerRepo.ExistsByMobileAsync(dto.Mobile);
            if (!exists) throw new NotFoundException("No provider account found for this mobile.");
        }

        var code = Random.Shared.Next(100000, 999999).ToString();

        await _otpRepo.CreateAsync(new CustomerOtp
        {
            Mobile = dto.Mobile,
            OtpCode = code,
            Purpose = dto.Purpose,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            IsUsed = false
        });

        return code;  // TODO: replace with SMS provider integration
    }

    public async Task<ProviderAuthResponseDto> VerifyOtpAsync(ProviderVerifyOtpDto dto, string? ip, string? device)
    {
        var otp = await _otpRepo.GetLatestActiveAsync(dto.Mobile, dto.OtpCode, dto.Purpose)
            ?? throw new UnauthorizedException("Invalid or expired OTP.");

        if (otp.ExpiresAt < DateTime.UtcNow)
            throw new UnauthorizedException("OTP has expired.");

        await _otpRepo.MarkUsedAsync(otp.Id);

        var provider = await _providerRepo.GetByMobileAsync(dto.Mobile)
            ?? throw new NotFoundException("Provider account not found.");

        if (provider.IsTerminated)
            throw new UnauthorizedException("Account has been terminated.");

        if (provider.Status != ProviderStatus.Approved)
        {
            return new ProviderAuthResponseDto
            {
                ProviderId = provider.Id,
                FirmName = provider.FirmName,
                OwnerName = provider.OwnerName,
                Email = provider.Email,
                Mobile = provider.Mobile,
                Status = provider.Status,
                Message = $"OTP verified. Current status: {provider.Status}. Login not yet allowed."
            };
        }

        await _providerRepo.UpdateLastLoginAsync(provider.Id);
        return await IssueTokensAsync(provider, ip, device);
    }

    public async Task<ProviderAuthResponseDto> RefreshTokenAsync(string refreshToken, string? ip, string? device)
    {
        var hash = _jwt.HashRefreshToken(refreshToken);
        var stored = await _refreshRepo.GetActiveByHashAsync(hash)
            ?? throw new UnauthorizedException("Invalid refresh token.");

        if (stored.ExpiresAt < DateTime.UtcNow || stored.RevokedAt != null)
            throw new UnauthorizedException("Refresh token expired or revoked.");

        if (stored.UserType != UserType.Provider)
            throw new UnauthorizedException("Token user type mismatch.");

        var provider = await _providerRepo.GetByIdAsync(stored.UserId)
            ?? throw new UnauthorizedException("Provider no longer exists.");

        if (provider.IsTerminated || provider.Status != ProviderStatus.Approved)
            throw new UnauthorizedException("Account is no longer eligible to refresh tokens.");

        await _refreshRepo.RevokeAsync(stored.Id);
        return await IssueTokensAsync(provider, ip, device);
    }

    public async Task LogoutAsync(string refreshToken)
    {
        var hash = _jwt.HashRefreshToken(refreshToken);
        var stored = await _refreshRepo.GetActiveByHashAsync(hash);
        if (stored != null) await _refreshRepo.RevokeAsync(stored.Id);
    }

    public async Task<ProviderStatusDto> GetStatusAsync(uint providerId)
    {
        return await _providerRepo.GetStatusAsync(providerId)
            ?? throw new NotFoundException("Provider not found.");
    }

    public async Task ResubmitAsync(uint providerId, ProviderResubmitDto dto)
    {
        var provider = await _providerRepo.GetByIdAsync(providerId)
            ?? throw new NotFoundException("Provider not found.");

        if (provider.Status != ProviderStatus.Returned)
            throw new AppException("Resubmission is allowed only when your application status is 'Returned'.");

        // Validate FK references if provided
        if (dto.DomainId.HasValue || dto.CategoryId.HasValue || dto.CityId.HasValue || dto.BoundaryId.HasValue)
        {
            await ValidateLookupsAsync(
                dto.DomainId ?? provider.DomainId,
                dto.CategoryId ?? provider.CategoryId,
                dto.CityId ?? provider.CityId,
                dto.BoundaryId ?? provider.BoundaryId);
        }

        var ok = await _providerRepo.ResubmitUpdateAsync(providerId, dto);
        if (!ok) throw new AppException("Could not save resubmission.");

        // Mark the latest pending resubmission record as 'resubmitted_at'
        var pending = await _resubmissionRepo.GetLatestPendingAsync(providerId);
        if (pending != null) await _resubmissionRepo.MarkResubmittedAsync(pending.Id);
    }

    // ─────────────────────────────────────────────────────────
    private async Task<ServiceProviderEntity?> ResolveProviderAsync(string identifier)
    {
        var trimmed = identifier.Trim();
        if (trimmed.Contains('@'))
            return await _providerRepo.GetByEmailAsync(trimmed.ToLower());

        if (trimmed.Length == 10 && trimmed.All(char.IsDigit))
            return await _providerRepo.GetByMobileAsync(trimmed);

        return null;
    }

    private async Task ValidateLookupsAsync(uint domainId, uint categoryId, uint cityId, uint? boundaryId)
    {
        if (!await _lookupRepo.DomainExistsAsync(domainId))
            throw new AppException("Invalid domain.");

        if (!await _lookupRepo.CategoryBelongsToDomainAsync(categoryId, domainId))
            throw new AppException("Category does not belong to the selected domain.");

        if (!await _lookupRepo.CityExistsAsync(cityId))
            throw new AppException("Invalid city.");

        if (boundaryId.HasValue && !await _lookupRepo.BoundaryBelongsToCityAsync(boundaryId.Value, cityId))
            throw new AppException("Selected boundary is not within the chosen city.");
    }

    private async Task<ProviderAuthResponseDto> IssueTokensAsync(ServiceProviderEntity provider, string? ip, string? device)
    {
        var (access, expiresAt) = _jwt.GenerateAccessToken(provider.Id, provider.Mobile, UserType.Provider);
        var refresh = _jwt.GenerateRefreshToken();

        await _refreshRepo.CreateAsync(new RefreshToken
        {
            UserType = UserType.Provider,
            UserId = provider.Id,
            TokenHash = _jwt.HashRefreshToken(refresh),
            DeviceInfo = device,
            IpAddress = ip,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        });

        return new ProviderAuthResponseDto
        {
            ProviderId = provider.Id,
            FirmName = provider.FirmName,
            OwnerName = provider.OwnerName,
            Email = provider.Email,
            Mobile = provider.Mobile,
            Status = provider.Status,
            AccessToken = access,
            RefreshToken = refresh,
            AccessTokenExpiresAt = expiresAt,
            Message = "Login successful."
        };
    }
}
