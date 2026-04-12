using Property.Application.Dtos;
using Property.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.Documents.UploadDocument;

public sealed class UploadDocumentCommandHandler(IPropertyApplicationService service)
    : IAppRequestHandler<UploadDocumentCommand, DocumentDto>
{
    public Task<DocumentDto> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
        => service.UploadDocumentAsync(request.OrganizationId, request.UserId, request.Request, cancellationToken);
}
