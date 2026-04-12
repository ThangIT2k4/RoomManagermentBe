using Organization.Application.Common;
using Organization.Application.Dtos;
using Organization.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Organization.Application.Features.Banking.AddBanking;

public sealed class AddOrganizationBankingCommandHandler(IOrganizationApplicationService service)
    : IAppRequestHandler<AddOrganizationBankingCommand, Result<OrganizationBankingDto>>
{
    public Task<Result<OrganizationBankingDto>> Handle(AddOrganizationBankingCommand request, CancellationToken cancellationToken)
        => service.AddBankingAsync(
            new AddOrganizationBankingRequest(request.OrganizationId, request.ActorUserId, request.SepayBankId, request.AccountNumber, request.AccountHolderName, request.BranchName, request.BranchCode, request.SwiftCode, request.IsPrimary),
            cancellationToken);
}
