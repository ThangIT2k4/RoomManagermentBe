using Property.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.Documents.UploadDocument;

public sealed record UploadDocumentCommand(Guid OrganizationId, Guid UserId, UploadDocumentRequest Request)
    : IAppRequest<DocumentDto>;
