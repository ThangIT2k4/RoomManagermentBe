namespace CRM.Application.Features.UseCases;

public sealed record CreateViewingCommand(Guid LeadId, DateTime ScheduledAt, string? Location, Guid AgentUserId, string? Note = null);
public sealed record UpdateViewingCommand(Guid ViewingId, DateTime? ScheduledAt = null, string? Location = null, string? Note = null);
public sealed record CancelViewingCommand(Guid ViewingId, string Reason, Guid RequestedBy);
public sealed record GetViewingsQuery(Guid OrganizationId, Guid? LeadId = null, Guid? AgentUserId = null, DateTime? From = null, DateTime? To = null, PagingRequest? Paging = null);
public sealed record GetViewingByIdQuery(Guid ViewingId);
public sealed record CompleteViewingCommand(Guid ViewingId, string? Summary = null, Guid? CompletedBy = null);
public sealed record CheckInViewingCommand(Guid ViewingId, DateTime? CheckedInAt = null, string? GeoLocation = null);
public sealed record AddViewingFeedbackCommand(Guid ViewingId, short Rating, string? Feedback = null, Guid? CreatedBy = null);
public sealed record UpdateViewingFeedbackCommand(Guid ViewingId, short Rating, string? Feedback = null, Guid? UpdatedBy = null);
public sealed record SetViewingRouteCommand(Guid ViewingId, string RouteData, Guid RequestedBy);
