using Organization.Domain.ValueObjects;

namespace Organization.Domain.Entities;

public sealed class OrganizationBankingEntity
{
    public Guid Id { get; }
    public Guid OrganizationId { get; }
    public Guid? SepayBankId { get; }
    public string AccountNumber { get; }
    public string AccountHolderName { get; }
    public string? BranchName { get; }
    public string? BranchCode { get; }
    public string? SwiftCode { get; }
    public bool IsPrimary { get; private set; }
    public DateTime CreatedAt { get; }

    private OrganizationBankingEntity(
        Guid id,
        Guid organizationId,
        Guid? sepayBankId,
        string accountNumber,
        string accountHolderName,
        string? branchName,
        string? branchCode,
        string? swiftCode,
        bool isPrimary,
        DateTime createdAt)
    {
        Id = id;
        OrganizationId = organizationId;
        SepayBankId = sepayBankId;
        AccountNumber = accountNumber;
        AccountHolderName = accountHolderName;
        BranchName = branchName;
        BranchCode = branchCode;
        SwiftCode = swiftCode;
        IsPrimary = isPrimary;
        CreatedAt = createdAt;
    }

    public static OrganizationBankingEntity Create(
        Guid organizationId,
        Guid? sepayBankId,
        string accountNumber,
        string accountHolderName,
        string? branchName,
        string? branchCode,
        string? swiftCode,
        bool isPrimary,
        DateTime now)
        => new(
            Guid.NewGuid(),
            organizationId,
            sepayBankId,
            BankAccountNumber.Create(accountNumber).Value,
            accountHolderName.Trim(),
            branchName?.Trim(),
            branchCode?.Trim(),
            swiftCode?.Trim(),
            isPrimary,
            now);

    public static OrganizationBankingEntity FromPersistence(
        Guid id,
        Guid organizationId,
        Guid? sepayBankId,
        string accountNumber,
        string accountHolderName,
        string? branchName,
        string? branchCode,
        string? swiftCode,
        bool isPrimary,
        DateTime createdAt)
        => new(
            id,
            organizationId,
            sepayBankId,
            accountNumber,
            accountHolderName,
            branchName,
            branchCode,
            swiftCode,
            isPrimary,
            createdAt);

    public void SetPrimary(bool isPrimary) => IsPrimary = isPrimary;
}
