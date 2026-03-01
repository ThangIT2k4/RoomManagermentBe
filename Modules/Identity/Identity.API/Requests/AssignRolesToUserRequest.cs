namespace Identity.API.Requests;

public sealed record AssignRolesToUserRequest(IReadOnlyCollection<Guid> RoleIds);

