using srv.slots.application.Common.Exceptions;
using srv.slots.application.DTOs.Auth;
using srv.slots.application.Interfaces.Repositories;
using srv.slots.application.Interfaces.Services;
using srv.slots.domain.Entities;
using srv.slots.domain.Enums;

namespace srv.slots.application.Services;

public class CustomerAuthService : ICustomerAuthService
{
    private readonly ICustomerRepository _customerRepo;
    private readonly IOtpRepository _otpRepo;
    private readonly IRefreshTokenRepository _refreshRepo;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenService _jwt;

    public CustomerAuthService(
        ICustomerRepository customerRepo,
        IOtpRepository otpRepo,
        IRefreshTokenRepository refreshRepo,
        IPasswordHasher hasher,
        IJwtTokenService jwt)
    {
        _customerRepo = customerRepo;
        _otpRepo = otpRepo;
        _refreshRepo = refreshRepo;
        _hasher = hasher;
        _jwt = jwt;
    }

    public async Task<AuthResponseDto> SignupAsync(SignupRequestDto dto, string? ip, string? device)
    {
        if (await _customerRepo.ExistsByMobileAsync(dto.Mobile))
            throw new ConflictException("Mobile number already registered.");

        if (!string.IsNullOrWhiteSpace(dto.Email) && await _customerRepo.ExistsByEmailAsync(dto.Email))
            throw new ConflictException("Email already registered.");

        var customer = new Customer
        {
            FullName = dto.FullName.Trim(),
            Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim().ToLower(),
            Mobile = dto.Mobile,
            PasswordHash = _hasher.Hash(dto.Password),
            IsVerified = false,
            IsActive = true
        };

        var id = await _customerRepo.CreateAsync(customer);
        customer.Id = id;

        return await IssueTokensAsync(customer, ip, device);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto, string? ip, string? device)
    {
        var customer = await _customerRepo.GetByMobileAsync(dto.Mobile)
            ?? throw new UnauthorizedException("Invalid mobile or password.");

        if (customer.IsTerminated)
            throw new UnauthorizedException("Account has been terminated.");

        if (!customer.IsActive)
            throw new UnauthorizedException("Account is inactive.");

        if (string.IsNullOrEmpty(customer.PasswordHash) || !_hasher.Verify(dto.Password, customer.PasswordHash))
            throw new UnauthorizedException("Invalid mobile or password.");

        await _customerRepo.UpdateLastLoginAsync(customer.Id);
        return await IssueTokensAsync(customer, ip, device);
    }

    public async Task<string> SendOtpAsync(SendOtpRequestDto dto)
    {
        // For Login OTP, ensure customer exists
        if (dto.Purpose == OtpPurpose.Login || dto.Purpose == OtpPurpose.ForgotPassword)
        {
            var exists = await _customerRepo.ExistsByMobileAsync(dto.Mobile);
            if (!exists) throw new NotFoundException("No account found for this mobile.");
        }

        // Generate 6-digit OTP
        var code = Random.Shared.Next(100000, 999999).ToString();

        var otp = new CustomerOtp
        {
            Mobile = dto.Mobile,
            OtpCode = code,
            Purpose = dto.Purpose,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            IsUsed = false
        };
        await _otpRepo.CreateAsync(otp);

        // TODO: Integrate with SMS provider (Twilio/MSG91). For now return for dev/testing.
        return code;
    }

    public async Task<AuthResponseDto> VerifyOtpAsync(VerifyOtpRequestDto dto, string? ip, string? device)
    {
        var otp = await _otpRepo.GetLatestActiveAsync(dto.Mobile, dto.OtpCode, dto.Purpose)
            ?? throw new UnauthorizedException("Invalid or expired OTP.");

        if (otp.ExpiresAt < DateTime.UtcNow)
            throw new UnauthorizedException("OTP has expired.");

        await _otpRepo.MarkUsedAsync(otp.Id);

        var customer = await _customerRepo.GetByMobileAsync(dto.Mobile)
            ?? throw new NotFoundException("Account not found.");

        if (!customer.IsVerified)
            await _customerRepo.SetVerifiedAsync(customer.Id);

        await _customerRepo.UpdateLastLoginAsync(customer.Id);
        return await IssueTokensAsync(customer, ip, device);
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken, string? ip, string? device)
    {
        var hash = _jwt.HashRefreshToken(refreshToken);
        var stored = await _refreshRepo.GetActiveByHashAsync(hash)
            ?? throw new UnauthorizedException("Invalid refresh token.");

        if (stored.ExpiresAt < DateTime.UtcNow || stored.RevokedAt != null)
            throw new UnauthorizedException("Refresh token expired or revoked.");

        if (stored.UserType != UserType.Customer)
            throw new UnauthorizedException("Token user type mismatch.");

        var customer = await _customerRepo.GetByIdAsync(stored.UserId)
            ?? throw new UnauthorizedException("User no longer exists.");

        // Rotate: revoke old, issue new
        await _refreshRepo.RevokeAsync(stored.Id);
        return await IssueTokensAsync(customer, ip, device);
    }

    public async Task LogoutAsync(string refreshToken)
    {
        var hash = _jwt.HashRefreshToken(refreshToken);
        var stored = await _refreshRepo.GetActiveByHashAsync(hash);
        if (stored != null) await _refreshRepo.RevokeAsync(stored.Id);
    }

    private async Task<AuthResponseDto> IssueTokensAsync(Customer customer, string? ip, string? device)
    {
        var (access, expiresAt) = _jwt.GenerateAccessToken(customer.Id, customer.Mobile, UserType.Customer);
        var refresh = _jwt.GenerateRefreshToken();

        await _refreshRepo.CreateAsync(new RefreshToken
        {
            UserType = UserType.Customer,
            UserId = customer.Id,
            TokenHash = _jwt.HashRefreshToken(refresh),
            DeviceInfo = device,
            IpAddress = ip,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        });

        return new AuthResponseDto
        {
            UserId = customer.Id,
            FullName = customer.FullName,
            Mobile = customer.Mobile,
            Email = customer.Email,
            AccessToken = access,
            RefreshToken = refresh,
            AccessTokenExpiresAt = expiresAt
        };
    }
}
