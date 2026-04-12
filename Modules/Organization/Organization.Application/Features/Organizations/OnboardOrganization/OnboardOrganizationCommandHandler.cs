using Organization.Application.Common;
using Organization.Application.Dtos;
using Organization.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Organization.Application.Features.Organizations.OnboardOrganization;

public sealed class OnboardOrganizationCommandHandler(IOrganizationApplicationService service)
    : IAppRequestHandler<OnboardOrganizationCommand, Result<OrganizationDto>>
{
    public Task<Result<OrganizationDto>> Handle(OnboardOrganizationCommand request, CancellationToken cancellationToken)
        => service.OnboardOrganizationAsync(
            new OnboardOrganizationRequest(request.OrganizationId, request.ActorUserId, request.Name, request.Phone, request.Email, request.TaxCode, request.Address),
            cancellationToken);
}
