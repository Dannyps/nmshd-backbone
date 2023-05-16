using AutoMapper;
using Backbone.Modules.Synchronization.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Synchronization.Application.SyncRuns.DTOs;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Queries.GetExternalEventsOfSyncRun;

public class Handler : IRequestHandler<GetExternalEventsOfSyncRunQuery, GetExternalEventsOfSyncRunResponse>
{
    private readonly DeviceId _activeDevice;
    private readonly IdentityAddress _activeIdentity;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public Handler(IUnitOfWork unitOfWork, IMapper mapper, IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _activeIdentity = userContext.GetAddress();
        _activeDevice = userContext.GetDeviceId();
    }

    public async Task<GetExternalEventsOfSyncRunResponse> Handle(GetExternalEventsOfSyncRunQuery request, CancellationToken cancellationToken)
    {
        var syncRun = await _unitOfWork.SyncRunsRepository.Find(request.SyncRunId, _activeIdentity, cancellationToken);

        if (syncRun.IsFinalized)
            throw new OperationFailedException(ApplicationErrors.SyncRuns.SyncRunAlreadyFinalized());

        if (syncRun.CreatedByDevice != _activeDevice)
            throw new OperationFailedException(ApplicationErrors.SyncRuns.CannotReadExternalEventsOfSyncRunStartedByAnotherDevice());

        var dbPaginationResult = await _unitOfWork.ExternalEventsRepository.FindExternalEventsOfSyncRun(request.PaginationFilter, _activeIdentity, syncRun.Id, cancellationToken);

        var dtos = _mapper.Map<IEnumerable<ExternalEventDTO>>(dbPaginationResult.ItemsOnPage);

        return new GetExternalEventsOfSyncRunResponse(dtos, request.PaginationFilter, dbPaginationResult.TotalNumberOfItems);
    }
}
