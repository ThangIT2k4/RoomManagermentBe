using Property.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.Units.SearchUnits;

public sealed record SearchUnitsQuery(
    Guid OrganizationId,
    Guid? PropertyId,
    IReadOnlyCollection<short>? Statuses,
    string? UnitType,
    decimal? MinRent,
    decimal? MaxRent,
    string? Search,
    int Page,
    int PerPage) : IAppRequest<IReadOnlyList<UnitDto>>;
