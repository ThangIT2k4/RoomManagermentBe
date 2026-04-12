using Organization.Application.Common;
using Organization.Application.Dtos;
using Organization.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Organization.Application.Features.Organizations.UpdateOrganization;

public sealed class UpdateOrganizationCommandHandler(IOrganizationApplicationService service)
    : IAppRequestHandler<UpdateOrganizationCommand, Result<OrganizationDto>>
{
    public Task<Result<OrganizationDto>> Handle(UpdateOrganizationCommand request, CancellationToken cancellationToken)
        => service.UpdateOrganizationAsync(
            new UpdateOrganizationRequest(request.OrganizationId, request.ActorUserId, request.Name, request.Phone, request.Email, request.PublicMail, request.TaxCode, request.Address),
            cancellationToken);
}
