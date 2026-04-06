using CRM.API.Common;
using CRM.Application.Features.Leads;
using CRM.Application.Services;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CRM.API.Controllers;

[ApiController]
[Route("api/leads")]
public sealed class LeadsController(
    ICrmApplicationService crmService,
    IValidator<CreateLeadRequest> createLeadValidator,
    IValidator<UpdateLeadStatusRequest> updateStatusValidator) : ControllerBase
{
    [HttpPost]
    [EnableRateLimiting("ApiPolicy")]
    [ProducesResponseType(typeof(LeadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LeadDto>> CreateLead([FromBody] CreateLeadRequest request, CancellationToken cancellationToken)
    {
        var validation = await createLeadValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(ToValidationErrorPayload(validation));
        }

        var result = await crmService.CreateLeadAsync(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("{leadId:guid}")]
    [ProducesResponseType(typeof(LeadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LeadDto>> GetLeadById([FromRoute] Guid leadId, CancellationToken cancellationToken)
    {
        var result = await crmService.GetLeadByIdAsync(leadId, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPatch("{leadId:guid}/status")]
    [EnableRateLimiting("ApiPolicy")]
    [ProducesResponseType(typeof(LeadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LeadDto>> UpdateLeadStatus([FromRoute] Guid leadId, [FromBody] UpdateLeadStatusRequest request, CancellationToken cancellationToken)
    {
        var command = request with { LeadId = leadId };
        var validation = await updateStatusValidator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(ToValidationErrorPayload(validation));
        }

        var result = await crmService.UpdateLeadStatusAsync(command, cancellationToken);
        return result.ToActionResult();
    }

    private static object ToValidationErrorPayload(ValidationResult validationResult)
    {
        return new
        {
            message = "Validation failed",
            errors = validationResult.Errors.Select(error => new
            {
                field = error.PropertyName,
                error = error.ErrorMessage
            })
        };
    }
}
