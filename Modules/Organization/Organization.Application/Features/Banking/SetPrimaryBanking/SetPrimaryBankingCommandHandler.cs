using Organization.Application.Common;
using Organization.Application.Dtos;
using Organization.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Organization.Application.Features.Banking.SetPrimaryBanking;

public sealed class SetPrimaryBankingCommandHandler(IOrganizationApplicationService service)
    : IAppRequestHandler<SetPrimaryBankingCommand, Result>
{
    public Task<Result> Handle(SetPrimaryBankingCommand request, CancellationToken cancellationToken)
        => service.SetPrimaryBankingAsync(
            new SetPrimaryBankingRequest(request.OrganizationId, request.BankingId, request.ActorUserId),
            cancellationToken);
}
