using Microsoft.AspNetCore.Mvc;

namespace Organization.API.Controllers;

[ApiController]
[Route("api/admin")]
public sealed class AdminController : ControllerBase
{
    [HttpPost("services")]
    [HttpPut("services/{id:guid}")]
    [HttpDelete("services/{id:guid}")]
    [HttpPost("sepay-banks")]
    [HttpPut("sepay-banks/{id:guid}")]
    [HttpDelete("sepay-banks/{id:guid}")]
    [HttpPost("sepay-banks/sync")]
    [HttpPost("subscription-plans")]
    [HttpPut("subscription-plans/{id:guid}")]
    [HttpDelete("subscription-plans/{id:guid}")]
    public IActionResult NotImplementedYet()
        => StatusCode(StatusCodes.Status501NotImplemented, new
        {
            message = "Endpoint contract is reserved. Business flow is implemented via Organization application service and domain model scaffold."
        });
}
