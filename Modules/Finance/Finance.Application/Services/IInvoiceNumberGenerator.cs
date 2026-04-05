namespace Finance.Application.Services;

public interface IInvoiceNumberGenerator
{
    Task<string> NextAsync(Guid organizationId, DateOnly invoiceDate, CancellationToken cancellationToken = default);
}
