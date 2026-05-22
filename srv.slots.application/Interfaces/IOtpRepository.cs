using srv.slots.domain.Entities;
using srv.slots.domain.Enums;

namespace srv.slots.application.Interfaces.Repositories;

public interface IOtpRepository
{
    Task<uint> CreateAsync(CustomerOtp otp);
    Task<CustomerOtp?> GetLatestActiveAsync(string mobile, string otpCode, OtpPurpose purpose);
    Task MarkUsedAsync(uint id);
}
