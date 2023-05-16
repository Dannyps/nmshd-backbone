using AutoMapper;
using Backbone.Modules.Synchronization.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Synchronization.Application.SyncRuns.DTOs;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Queries.GetSyncRunById;

public class Handler : IRequestHandler<GetSyncRunByIdQuery, SyncRunDTO>
{
    private readonly IdentityAddress _activeIdentity;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public Handler(IUnitOfWork unitOfWork, IUserContext userContext, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _activeIdentity = userContext.GetAddress();
        userContext.GetDeviceId();
    }

    public async Task<SyncRunDTO> Handle(GetSyncRunByIdQuery request, CancellationToken cancellationToken)
    {
        var syncRun = await _unitOfWork.SyncRunsRepository.Find(request.SyncRunId, _activeIdentity, CancellationToken.None);
        var dto = _mapper.Map<SyncRunDTO>(syncRun);

        return dto;
    }
}
