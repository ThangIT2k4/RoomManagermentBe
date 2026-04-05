using Auth.Domain.Entities;
using Auth.Domain.Enums;
using Auth.Domain.ValueObjects;

namespace Auth.Domain.Repositories;

public interface IEmailOtpRepository
{
    Task<EmailOtpEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<EmailOtpEntity?> GetLatestAsync(Email email, EmailOtpType type, CancellationToken cancellationToken = default);
    Task<EmailOtpEntity> AddAsync(EmailOtpEntity otp, CancellationToken cancellationToken = default);
    Task<EmailOtpEntity> UpdateAsync(EmailOtpEntity otp, CancellationToken cancellationToken = default);
}

