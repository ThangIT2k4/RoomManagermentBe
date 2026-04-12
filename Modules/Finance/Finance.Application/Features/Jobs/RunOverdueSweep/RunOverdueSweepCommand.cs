using RoomManagerment.Shared.Messaging;

namespace Finance.Application.Features.Jobs.RunOverdueSweep;

public sealed record RunOverdueSweepCommand(DateOnly AsOfDate) : IAppRequest<Result<int>>;
