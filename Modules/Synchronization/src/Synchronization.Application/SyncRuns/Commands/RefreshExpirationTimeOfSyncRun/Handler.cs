using Backbone.Modules.Synchronization.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Commands.RefreshExpirationTimeOfSyncRun;

public class Handler : IRequestHandler<RefreshExpirationTimeOfSyncRunCommand, RefreshExpirationTimeOfSyncRunResponse>
{
    private readonly DeviceId _activeDevice;
    private readonly IdentityAddress _activeIdentity;
    private readonly IUnitOfWork _unitOfWork;

    public Handler(IUnitOfWork unitOfWork, IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _activeIdentity = userContext.GetAddress();
        _activeDevice = userContext.GetDeviceId();
    }

    public async Task<RefreshExpirationTimeOfSyncRunResponse> Handle(RefreshExpirationTimeOfSyncRunCommand request, CancellationToken cancellationToken)
    {
        var syncRun = await _unitOfWork.SyncRunsRepository.Find(request.SyncRunId, _activeIdentity, cancellationToken);

        CheckPrerequisites(syncRun);

        syncRun.RefreshExpirationTime();

        _unitOfWork.SyncRunsRepository.Update(syncRun);

        await _unitOfWork.Save(cancellationToken);

        return new RefreshExpirationTimeOfSyncRunResponse { ExpiresAt = syncRun.ExpiresAt };
    }

    private void CheckPrerequisites(SyncRun syncRun)
    {
        if (syncRun.CreatedByDevice != _activeDevice)
            throw new OperationFailedException(ApplicationErrors.SyncRuns.CannotRefreshExpirationTimeOfSyncRunStartedByAnotherDevice());

        if (syncRun.IsFinalized)
            throw new OperationFailedException(ApplicationErrors.SyncRuns.SyncRunAlreadyFinalized());
    }
}
