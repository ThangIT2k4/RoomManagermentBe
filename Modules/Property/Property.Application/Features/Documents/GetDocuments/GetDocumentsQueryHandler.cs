using Property.Application.Dtos;
using Property.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.Documents.GetDocuments;

public sealed class GetDocumentsQueryHandler(IPropertyApplicationService service)
    : IAppRequestHandler<GetDocumentsQuery, IReadOnlyList<DocumentDto>>
{
    public Task<IReadOnlyList<DocumentDto>> Handle(GetDocumentsQuery request, CancellationToken cancellationToken)
        => service.GetDocumentsAsync(request.OrganizationId, request.OwnerType, request.OwnerId, cancellationToken);
}
