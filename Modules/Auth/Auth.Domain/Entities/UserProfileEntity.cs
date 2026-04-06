using Auth.Domain.Common;
using Auth.Domain.Enums;
using Auth.Domain.Events;
using Auth.Domain.ValueObjects;

namespace Auth.Domain.Entities;

public sealed class UserProfileEntity : AggregateRoot<Guid>
{
    public FullName? FullName { get; private set; }
    public string? Avatar { get; private set; }
    public DateOnly? Dob { get; private set; }
    public GenderType? Gender { get; private set; }
    public string? IdNumber { get; private set; }
    public string? TaxCode { get; private set; }
    public DateOnly? IdIssuedAt { get; private set; }
    public string? IdCardPlace { get; private set; }
    public string? IdImagesJson { get; private set; }
    public string? Address { get; private set; }
    public string? Note { get; private set; }
    public Guid? SepayBankId { get; private set; }
    public string? AccountNumber { get; private set; }
    public string? AccountHolderName { get; private set; }
    public string? BranchName { get; private set; }
    public string? BranchCode { get; private set; }
    public string? SwiftCode { get; private set; }
    public string? BankingNotes { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private UserProfileEntity() { }

    private UserProfileEntity(Guid userId, FullName? fullName, DateOnly? dob, GenderType? gender, DateTime createdAt, DateTime? updatedAt)
    {
        Id = userId;
        FullName = fullName;
        Dob = dob;
        Gender = gender;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static UserProfileEntity Create(Guid userId, string? fullName = null, DateOnly? dob = null, GenderType? gender = null, DateTime? createdAt = null)
        => new(userId, fullName is null ? null : FullName.Create(fullName), dob, gender, createdAt ?? DateTime.UtcNow, null);

    public static UserProfileEntity Reconstitute(
        Guid userId,
        string? fullName,
        string? avatar,
        DateOnly? dob,
        GenderType? gender,
        string? idNumber,
        string? taxCode,
        DateOnly? idIssuedAt,
        string? idCardPlace,
        string? idImagesJson,
        string? address,
        string? note,
        Guid? sepayBankId,
        string? accountNumber,
        string? accountHolderName,
        string? branchName,
        string? branchCode,
        string? swiftCode,
        string? bankingNotes,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        return new UserProfileEntity(userId, fullName is null ? null : FullName.Create(fullName), dob, gender, createdAt, updatedAt)
        {
            Avatar = avatar?.Trim(),
            IdNumber = idNumber?.Trim(),
            TaxCode = taxCode?.Trim(),
            IdIssuedAt = idIssuedAt,
            IdCardPlace = idCardPlace?.Trim(),
            IdImagesJson = idImagesJson?.Trim(),
            Address = address?.Trim(),
            Note = note?.Trim(),
            SepayBankId = sepayBankId,
            AccountNumber = accountNumber?.Trim(),
            AccountHolderName = accountHolderName?.Trim(),
            BranchName = branchName?.Trim(),
            BranchCode = branchCode?.Trim(),
            SwiftCode = swiftCode?.Trim(),
            BankingNotes = bankingNotes?.Trim()
        };
    }

    public void UpdatePersonalInfo(string? fullName, DateOnly? dob, GenderType? gender, string? idNumber, string? taxCode, DateOnly? idIssuedAt, string? idCardPlace, string? idImagesJson, string? address, string? note, DateTime changedAt)
    {
        EnsureUtc(changedAt);
        FullName = fullName is null ? null : FullName.Create(fullName);
        Dob = dob;
        Gender = gender;
        IdNumber = idNumber?.Trim();
        TaxCode = taxCode?.Trim();
        IdIssuedAt = idIssuedAt;
        IdCardPlace = idCardPlace?.Trim();
        IdImagesJson = idImagesJson?.Trim();
        Address = address?.Trim();
        Note = note?.Trim();
        UpdatedAt = changedAt;
        AddDomainEvent(new UserProfileUpdatedEvent(Id, DateTimeOffset.UtcNow));
    }

    public void UpdateBankingInfo(Guid? sepayBankId, string? accountNumber, string? accountHolderName, string? branchName, string? branchCode, string? swiftCode, string? bankingNotes, DateTime changedAt)
    {
        EnsureUtc(changedAt);
        SepayBankId = sepayBankId;
        AccountNumber = accountNumber?.Trim();
        AccountHolderName = accountHolderName?.Trim();
        BranchName = branchName?.Trim();
        BranchCode = branchCode?.Trim();
        SwiftCode = swiftCode?.Trim();
        BankingNotes = bankingNotes?.Trim();
        UpdatedAt = changedAt;
        AddDomainEvent(new UserProfileUpdatedEvent(Id, DateTimeOffset.UtcNow));
    }

    public void UpdateAvatar(string? avatar, DateTime changedAt)
    {
        EnsureUtc(changedAt);
        var normalized = avatar?.Trim();
        if (Avatar == normalized)
        {
            return;
        }

        Avatar = normalized;
        UpdatedAt = changedAt;
        AddDomainEvent(new UserProfileUpdatedEvent(Id, DateTimeOffset.UtcNow));
    }

    private static void EnsureUtc(DateTime value)
    {
        if (value.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException("Timestamp must be in UTC.", nameof(value));
        }
    }
}

