using AutoMapper;
using Backbone.Modules.Synchronization.Application.Datawallets.DTOs;
using Backbone.Modules.Synchronization.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Synchronization.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Modules.Synchronization.Application.Datawallets.Queries.GetDatawallet;

internal class Handler : IRequestHandler<GetDatawalletQuery, DatawalletDTO>
{
    private readonly IdentityAddress _activeIdentity;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public Handler(IUnitOfWork unitOfWork, IUserContext userContext, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _activeIdentity = userContext.GetAddress();
    }

    public async Task<DatawalletDTO> Handle(GetDatawalletQuery request, CancellationToken cancellationToken)
    {
        var datawallet = await _unitOfWork.DatawalletsRepository.Find(_activeIdentity, cancellationToken);

        if (datawallet == null)
            throw new NotFoundException(nameof(Datawallet));

        return _mapper.Map<DatawalletDTO>(datawallet);
    }
}
