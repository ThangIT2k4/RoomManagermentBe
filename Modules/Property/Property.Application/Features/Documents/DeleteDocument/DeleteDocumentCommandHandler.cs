using Property.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.Documents.DeleteDocument;

public sealed class DeleteDocumentCommandHandler(IPropertyApplicationService service)
    : IAppRequestHandler<DeleteDocumentCommand, bool>
{
    public Task<bool> Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
        => service.DeleteDocumentAsync(request.OrganizationId, request.UserId, request.DocumentId, cancellationToken);
}
