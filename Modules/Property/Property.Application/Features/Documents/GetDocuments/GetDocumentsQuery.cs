using Property.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.Documents.GetDocuments;

public sealed record GetDocumentsQuery(Guid OrganizationId, string OwnerType, Guid OwnerId)
    : IAppRequest<IReadOnlyList<DocumentDto>>;
