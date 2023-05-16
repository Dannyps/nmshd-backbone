using AutoMapper;
using Backbone.Modules.Synchronization.Application.Datawallets.DTOs;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Synchronization.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Synchronization.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using MediatR;
using Microsoft.Extensions.Options;
using static Backbone.Modules.Synchronization.Domain.Entities.Datawallet;

namespace Backbone.Modules.Synchronization.Application.Datawallets.Commands.PushDatawalletModifications;

public class Handler : IRequestHandler<PushDatawalletModificationsCommand, PushDatawalletModificationsResponse>
{
    private readonly DeviceId _activeDevice;
    private readonly IdentityAddress _activeIdentity;
    private readonly IBlobStorage _blobStorage;
    private readonly BlobOptions _blobOptions;
    private readonly IEventBus _eventBus;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    private PushDatawalletModificationsCommand _request;
    private CancellationToken _cancellationToken;
    private DatawalletVersion _supportedDatawalletVersion;
    private Datawallet _datawallet;
    private DatawalletModification[] _modifications;
    private PushDatawalletModificationsResponse _response;

    public Handler(IUnitOfWork unitOfWork, IUserContext userContext, IMapper mapper, IBlobStorage blobStorage, IOptions<BlobOptions> blobOptions, IEventBus eventBus)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _blobStorage = blobStorage;
        _blobOptions = blobOptions.Value;
        _eventBus = eventBus;
        _activeIdentity = userContext.GetAddress();
        _activeDevice = userContext.GetDeviceId();
    }

    public async Task<PushDatawalletModificationsResponse> Handle(PushDatawalletModificationsCommand request, CancellationToken cancellationToken)
    {
        _request = request;
        _cancellationToken = cancellationToken;
        _supportedDatawalletVersion = new DatawalletVersion(_request.SupportedDatawalletVersion);

        await EnsureNoActiveSyncRunExists(cancellationToken);
        await ReadDatawallet(cancellationToken);
        EnsureDatawalletExists();
        EnsureSufficientSupportedDatawalletVersion();
        EnsureDeviceIsUpToDate();
        CreateModifications();

        await _unitOfWork.Save(cancellationToken);

        PublishIntegrationEvent();
        BuildResponse();

        return _response;
    }

    private async Task ReadDatawallet(CancellationToken cancellationToken)
    {
        _datawallet = await _unitOfWork.DatawalletsRepository.FindDatawalletForInsertion(_activeIdentity, cancellationToken, true);
    }

    private void EnsureDatawalletExists()
    {
        if (_datawallet == null)
            throw new NotFoundException(nameof(Datawallet));
    }

    private async Task EnsureNoActiveSyncRunExists(CancellationToken cancellationToken)
    {
        var isActiveSyncRunAvailable = await _unitOfWork.SyncRunsRepository.IsActiveSyncRunAvailable(_activeIdentity, cancellationToken);

        if (isActiveSyncRunAvailable)
            throw new OperationFailedException(ApplicationErrors.Datawallet.CannotPushModificationsDuringActiveSyncRun());
    }

    private void EnsureSufficientSupportedDatawalletVersion()
    {
        if (_supportedDatawalletVersion < _datawallet.Version)
            throw new OperationFailedException(ApplicationErrors.Datawallet.InsufficientSupportedDatawalletVersion());
    }

    private void CreateModifications()
    {
        var newModifications = _request.Modifications.Select(CreateModification).ToArray();
        _datawallet.NewModifications = newModifications;

        _unitOfWork.DatawalletsRepository.Update(_datawallet);

        _modifications = newModifications;
    }

    private DatawalletModification CreateModification(PushDatawalletModificationItem modificationDto)
    {
        var newModification = _datawallet.AddModification(
            _mapper.Map<DatawalletModificationType>(modificationDto.Type),
            new DatawalletVersion(modificationDto.DatawalletVersion),
            modificationDto.Collection,
            modificationDto.ObjectIdentifier,
            modificationDto.PayloadCategory,
            modificationDto.EncryptedPayload,
            _activeDevice
        );

        return newModification;
    }

    private void EnsureDeviceIsUpToDate()
    {
        if (_datawallet.LatestModification != null && _datawallet.LatestModification.Index != _request.LocalIndex)
            throw new OperationFailedException(ApplicationErrors.Datawallet.DatawalletNotUpToDate(_request.LocalIndex, _datawallet.LatestModification.Index));
    }

    private void BuildResponse()
    {
        var responseItems = _mapper.Map<PushDatawalletModificationsResponseItem[]>(_modifications);
        _response = new PushDatawalletModificationsResponse { Modifications = responseItems, NewIndex = responseItems.Max(i => i.Index) };
    }

    private void PublishIntegrationEvent()
    {
        _eventBus.Publish(new DatawalletModifiedIntegrationEvent(_activeIdentity, _activeDevice));
    }
}
