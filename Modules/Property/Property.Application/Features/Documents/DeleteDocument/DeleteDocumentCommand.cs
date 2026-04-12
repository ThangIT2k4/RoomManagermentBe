using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.Documents.DeleteDocument;

public sealed record DeleteDocumentCommand(Guid OrganizationId, Guid UserId, Guid DocumentId)
    : IAppRequest<bool>;
