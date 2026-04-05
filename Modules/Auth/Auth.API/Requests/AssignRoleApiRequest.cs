namespace Auth.API.Requests;

public sealed record AssignRoleApiRequest(Guid OrganizationId, Guid RoleId);
