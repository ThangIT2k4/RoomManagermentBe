using Finance.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Finance.API.Controllers;

[ApiController]
[Route("api/finance/webhooks")]
public sealed class PaymentWebhookController(
    IFinanceApplicationService finance,
    ILogger<PaymentWebhookController> logger) : ControllerBase
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

        var result = await finance.HandlePaymentWebhookAsync(raw, headers, cancellationToken);
        if (result.IsFailure)
        {
            logger.LogError(
                "Payment webhook processing failed: {Code} {Message}",
                result.Error?.Code,
                result.Error?.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        return Ok();
    }
}
