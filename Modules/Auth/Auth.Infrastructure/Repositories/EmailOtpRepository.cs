using Auth.Application.Services;
using Auth.Domain.Common;
using Auth.Domain.Entities;
using Auth.Domain.Enums;
using Auth.Domain.Repositories;
using Auth.Domain.ValueObjects;
using Auth.Infrastructure.Mapper;
using Microsoft.Extensions.Logging;
using RoomManagerment.Auth.DatabaseSpecific;
using RoomManagerment.Auth.Linq;
using SD.LLBLGen.Pro.LinqSupportClasses;

namespace Auth.Infrastructure.Repositories;

public sealed class EmailOtpRepository(
    DataAccessAdapter adapter,
    IIntegrationEventPublisher eventPublisher,
    ILogger<EmailOtpRepository> logger) : IEmailOtpRepository
{
    private const string Repo = nameof(EmailOtpRepository);

    public Task<EmailOtpEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => AuthDataAccessGuard.RunAsync(logger, Repo, nameof(GetByIdAsync), cancellationToken, async () =>
        {
            var linq = new LinqMetaData(adapter);
            var dal = await linq.EmailOtp.Where(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
            return dal?.ToDomain();
        });

    public Task<EmailOtpEntity?> GetLatestAsync(Email email, EmailOtpType type, CancellationToken cancellationToken = default)
        => AuthDataAccessGuard.RunAsync(logger, Repo, nameof(GetLatestAsync), cancellationToken, async () =>
        {
            var linq = new LinqMetaData(adapter);
            var emailValue = email.Value;
            var typeValue = EmailOtpTypeStorage.ToPersistedString(type);
            var dal = await linq.EmailOtp
                .Where(x => x.Email == emailValue && x.Type == typeValue)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);
            return dal?.ToDomain();
        });

    public Task<EmailOtpEntity> AddAsync(EmailOtpEntity otp, CancellationToken cancellationToken = default)
        => AuthDataAccessGuard.RunAsync(logger, Repo, nameof(AddAsync), cancellationToken, async () =>
        {
            var dal = otp.ToPersistence();
            await adapter.SaveEntityAsync(dal, true, false, cancellationToken);
            await PublishDomainEventsAsync(otp, cancellationToken);
            return otp;
        });

    public Task<EmailOtpEntity> UpdateAsync(EmailOtpEntity otp, CancellationToken cancellationToken = default)
        => AuthDataAccessGuard.RunAsync(logger, Repo, nameof(UpdateAsync), cancellationToken, async () =>
        {
            var linq = new LinqMetaData(adapter);
            var existing = await linq.EmailOtp.Where(x => x.Id == otp.Id).FirstOrDefaultAsync(cancellationToken);
            if (existing is null)
            {
                throw new InvalidOperationException("Không tìm thấy OTP email để cập nhật.");
            }

            existing.ApplyFromDomain(otp);
            await adapter.SaveEntityAsync(existing, true, false, cancellationToken);
            await PublishDomainEventsAsync(otp, cancellationToken);
            return otp;
        });

    public Task<PagedResult<EmailOtpEntity>> GetByEmailPagedAsync(
        Email email,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
        => AuthDataAccessGuard.RunAsync(logger, Repo, nameof(GetByEmailPagedAsync), cancellationToken, async () =>
        {
            var paging = PagingInput.Create(pageNumber, pageSize);
            var linq = new LinqMetaData(adapter);
            var emailValue = email.Value;

            var query = linq.EmailOtp.Where(x => x.Email == emailValue);
            var totalCount = await query.LongCountAsync(cancellationToken);
            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip(paging.Skip)
                .Take(paging.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<EmailOtpEntity>(
                items.Select(x => x.ToDomain()).ToList(),
                (int)totalCount,
                paging.PageNumber,
                paging.PageSize);
        });

    public Task<bool> HasUnexpiredOtpAsync(
        Email email,
        EmailOtpType type,
        DateTimeOffset now,
        CancellationToken cancellationToken = default)
        => AuthDataAccessGuard.RunAsync(logger, Repo, nameof(HasUnexpiredOtpAsync), cancellationToken, async () =>
        {
            var linq = new LinqMetaData(adapter);
            var emailValue = email.Value;
            var typeValue = EmailOtpTypeStorage.ToPersistedString(type);
            var nowUtc = now.UtcDateTime;

            return await linq.EmailOtp.AnyAsync(
                x => x.Email == emailValue &&
                     x.Type == typeValue &&
                     !x.IsUsed &&
                     x.ExpiresAt > nowUtc,
                cancellationToken);
        });

    private async Task PublishDomainEventsAsync(EmailOtpEntity otp, CancellationToken cancellationToken)
    {
        foreach (var domainEvent in otp.DomainEvents)
        {
            await eventPublisher.PublishAsync(domainEvent, cancellationToken);
        }

        otp.ClearDomainEvents();
    }
}
