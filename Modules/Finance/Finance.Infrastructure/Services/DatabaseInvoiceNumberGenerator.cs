using Finance.Application.Services;
using Finance.Domain.Repositories;

namespace Finance.Infrastructure.Services;

public sealed class DatabaseInvoiceNumberGenerator(IInvoiceRepository invoiceRepository) : IInvoiceNumberGenerator
{
    public async Task<string> NextAsync(Guid organizationId, DateOnly invoiceDate, CancellationToken cancellationToken = default)
    {
        var year = invoiceDate.Year;
        var max = await invoiceRepository.GetMaxInvoiceSequenceForYearAsync(organizationId, year, cancellationToken);
        return $"INV{year}-{(max + 1):D4}";
    }
}
