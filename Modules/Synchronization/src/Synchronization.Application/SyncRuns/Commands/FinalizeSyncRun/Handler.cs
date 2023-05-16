using AutoMapper;
using Backbone.Modules.Synchronization.Application.Datawallets.DTOs;
using Backbone.Modules.Synchronization.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Synchronization.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Synchronization.Domain.Entities;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using MediatR;
using ApplicationException = Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Commands.FinalizeSyncRun;

public class Handler : IRequestHandler<FinalizeExternalEventSyncSyncRunCommand, FinalizeExternalEventSyncSyncRunResponse>, IRequestHandler<FinalizeDatawalletVersionUpgradeSyncRunCommand, FinalizeDatawalletVersionUpgradeSyncRunResponse>
{
    private readonly DeviceId _activeDevice;
    private readonly IdentityAddress _activeIdentity;
    private readonly IEventBus _eventBus;
    private readonly IMapper _mapper;
    private Datawallet _datawallet;
    private SyncRun _syncRun;
    private readonly IUnitOfWork _unitOfWork;

    public Handler(IUnitOfWork unitOfWork, IUserContext userContext, IMapper mapper, IEventBus eventBus)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _eventBus = eventBus;
        _activeIdentity = userContext.GetAddress();
        _activeDevice = userContext.GetDeviceId();
    }

    public async Task<FinalizeDatawalletVersionUpgradeSyncRunResponse> Handle(FinalizeDatawalletVersionUpgradeSyncRunCommand request, CancellationToken cancellationToken)
    {
        _syncRun = await _unitOfWork.SyncRunsRepository.Find(request.SyncRunId, _activeIdentity, cancellationToken, true);

        if (_syncRun.Type != SyncRun.SyncRunType.DatawalletVersionUpgrade)
            throw new ApplicationException(ApplicationErrors.SyncRuns.UnexpectedSyncRunType(SyncRun.SyncRunType.DatawalletVersionUpgrade));

        CheckPreconditions();

        _syncRun.FinalizeDatawalletVersionUpgrade();

        _unitOfWork.SyncRunsRepository.Update(_syncRun);

        _datawallet = await _unitOfWork.DatawalletsRepository.FindDatawalletForInsertion(_activeIdentity, cancellationToken);

        var newModifications = AddModificationsToDatawallet(request.DatawalletModifications);

        if (_datawallet == null)
        {
            _datawallet = new Datawallet(new Datawallet.DatawalletVersion(request.NewDatawalletVersion), _activeIdentity);
            _unitOfWork.DatawalletsRepository.Add(_datawallet);
        }
        else
        {
            _datawallet.Upgrade(new Datawallet.DatawalletVersion(request.NewDatawalletVersion));
            _unitOfWork.DatawalletsRepository.Update(_datawallet);
        }

        await _unitOfWork.Save(cancellationToken);

        var response = new FinalizeDatawalletVersionUpgradeSyncRunResponse
        {
            NewDatawalletModificationIndex = _datawallet.LatestModification?.Index,
            DatawalletModifications = _mapper.Map<CreatedDatawalletModificationDTO[]>(newModifications)
        };

        PublishDatawalletModifiedIntegrationEvent();

        return response;
    }

    public async Task<FinalizeExternalEventSyncSyncRunResponse> Handle(FinalizeExternalEventSyncSyncRunCommand request, CancellationToken cancellationToken)
    {
        _syncRun = await _unitOfWork.SyncRunsRepository.Find(request.SyncRunId, _activeIdentity, cancellationToken, true);

        if (_syncRun.Type != SyncRun.SyncRunType.ExternalEventSync)
            throw new ApplicationException(ApplicationErrors.SyncRuns.UnexpectedSyncRunType(SyncRun.SyncRunType.ExternalEventSync));

        CheckPreconditions();

        _datawallet = await _unitOfWork.DatawalletsRepository.FindDatawalletForInsertion(_activeIdentity, cancellationToken);

        if (_datawallet == null)
            throw new NotFoundException(nameof(Datawallet));

        var eventResults = _mapper.Map<ExternalEventResult[]>(request.ExternalEventResults);
        _syncRun.FinalizeExternalEventSync(eventResults);

        _unitOfWork.SyncRunsRepository.Update(_syncRun);

        var newModifications = AddModificationsToDatawallet(request.DatawalletModifications);
        _unitOfWork.DatawalletsRepository.Update(_datawallet);

        await _unitOfWork.Save(cancellationToken);

        var response = new FinalizeExternalEventSyncSyncRunResponse
        {
            NewDatawalletModificationIndex = _datawallet.LatestModification?.Index,
            DatawalletModifications = _mapper.Map<CreatedDatawalletModificationDTO[]>(newModifications)
        };

        if (newModifications.Count > 0)
            PublishDatawalletModifiedIntegrationEvent();

        return response;
    }

    private void CheckPreconditions()
    {
        if (_syncRun.CreatedByDevice != _activeDevice)
            throw new OperationFailedException(ApplicationErrors.SyncRuns.CannotFinalizeSyncRunStartedByAnotherDevice());

        if (_syncRun.IsFinalized)
            throw new OperationFailedException(ApplicationErrors.SyncRuns.SyncRunAlreadyFinalized());
    }

    private List<DatawalletModification> AddModificationsToDatawallet(List<PushDatawalletModificationItem> modifications)
    {
        if (_datawallet == null)
            throw new NotFoundException(nameof(Datawallet));

        if (!modifications.Any())
            return new List<DatawalletModification>();

        var newModifications = new List<DatawalletModification>();
        foreach (var modificationDto in modifications)
        {
            var newModification = _datawallet.AddModification(
                _mapper.Map<DatawalletModificationType>(modificationDto.Type),
                new Datawallet.DatawalletVersion(modificationDto.DatawalletVersion),
                modificationDto.Collection,
                modificationDto.ObjectIdentifier,
                modificationDto.PayloadCategory,
                modificationDto.EncryptedPayload,
                _activeDevice);

            newModifications.Add(newModification);
        }

        _datawallet.NewModifications = newModifications;

        return newModifications;
    }

    private void PublishDatawalletModifiedIntegrationEvent()
    {
        _eventBus.Publish(new DatawalletModifiedIntegrationEvent(_activeIdentity, _activeDevice));
    }
}
