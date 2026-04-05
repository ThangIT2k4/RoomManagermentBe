using Finance.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Finance.API.Controllers;

[ApiController]
[Route("api/finance/webhooks")]
public sealed class PaymentWebhookController(IFinanceApplicationService finance) : ControllerBase
{
    [HttpPost("payment")]
    public async Task<IActionResult> Handle(CancellationToken cancellationToken)
    {
        Request.EnableBuffering();
        using var reader = new StreamReader(Request.Body, leaveOpen: true);
        var raw = await reader.ReadToEndAsync(cancellationToken);
        Request.Body.Position = 0;

        var headers = Request.Headers.ToDictionary(
            h => h.Key,
            h => h.Value.FirstOrDefault() ?? string.Empty,
            StringComparer.OrdinalIgnoreCase);

        await finance.HandlePaymentWebhookAsync(raw, headers, cancellationToken);
        return Ok();
    }
}
