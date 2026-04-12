using Organization.Application.Common;
using Organization.Application.Dtos;
using Organization.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Organization.Application.Features.Banking.RemoveBanking;

public sealed class RemoveBankingCommandHandler(IOrganizationApplicationService service)
    : IAppRequestHandler<RemoveBankingCommand, Result>
{
    public Task<Result> Handle(RemoveBankingCommand request, CancellationToken cancellationToken)
        => service.RemoveBankingAsync(
            new RemoveBankingRequest(request.OrganizationId, request.BankingId, request.ActorUserId),
            cancellationToken);
}
