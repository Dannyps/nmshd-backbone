﻿using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Enmeshed.Tooling;

namespace Backbone.Modules.Synchronization.Domain.Entities.Sync;

public class SyncRun
{
    public enum SyncRunType
    {
        ExternalEventSync = 0,
        DatawalletVersionUpgrade = 1
    }

    private readonly List<SyncError> _errors = new();
    private readonly List<ExternalEvent> _externalEvents = new();

#pragma warning disable CS8618
    private SyncRun() { }
#pragma warning restore CS8618

    public SyncRun(long index, ushort duration, IdentityAddress createdBy, DeviceId createdByDevice, IEnumerable<ExternalEvent> items, SyncRunType type)
    {
        Id = SyncRunId.New();
        ExpiresAt = SystemTime.UtcNow.AddSeconds(duration);
        CreatedAt = SystemTime.UtcNow;
        Index = index;
        CreatedBy = createdBy;
        CreatedByDevice = createdByDevice;
        Type = type;
        _externalEvents = items.ToList();
        EventCount = _externalEvents.Count;
    }

    public SyncRunId Id { get; }
    public SyncRunType Type { get; set; }
    public DateTime ExpiresAt { get; internal set; }
    public long Index { get; }
    public DateTime CreatedAt { get; }
    public IdentityAddress CreatedBy { get; }
    public DeviceId CreatedByDevice { get; }
    public DateTime? FinalizedAt { get; internal set; }
    public IReadOnlyList<ExternalEvent> ExternalEvents => _externalEvents;
    public int EventCount { get; }
    public IReadOnlyList<SyncError> Errors => _errors.AsReadOnly();


    public bool IsFinalized => FinalizedAt != null;
    public bool IsExpired => ExpiresAt < SystemTime.UtcNow;

    public void RefreshExpirationTime()
    {
        ExpiresAt = SystemTime.UtcNow.AddSeconds(10);
    }

    public void FinalizeExternalEventSync(ExternalEventResult[] itemResults)
    {
        if (Type != SyncRunType.ExternalEventSync)
            throw new Exception($"Only SyncRuns with type {SyncRunType.ExternalEventSync} can work on external events.");

        FinalizedAt = SystemTime.UtcNow;

        var items = _externalEvents.ToArray();
        foreach (var item in items)
        {
            var result = itemResults.FirstOrDefault(i => i.ExternalEventId == item.Id);
            ProcessItemResult(result, item);
        }
    }

    public void FinalizeDatawalletVersionUpgrade()
    {
        if (Type != SyncRunType.DatawalletVersionUpgrade)
            throw new Exception($"Only SyncRuns with type {SyncRunType.DatawalletVersionUpgrade} finalize without external events.");

        FinalizedAt = SystemTime.UtcNow;
    }

    private void ProcessItemResult(ExternalEventResult? itemResult, ExternalEvent item)
    {
        if (itemResult == null)
        {
            ItemSyncFailed(item, "notProcessed");
            return;
        }

        var isErrorResult = !string.IsNullOrEmpty(itemResult.ErrorCode);
        if (isErrorResult)
            ItemSyncFailed(item, itemResult.ErrorCode);
    }

    private void ItemSyncFailed(ExternalEvent item, string errorCode)
    {
        var error = new SyncError(this, item, errorCode);

        item.SyncFailed(error);
        AddError(error);
        _externalEvents.Remove(item);
    }

    private void AddError(SyncError error)
    {
        _errors.Add(error);
    }

    public void Cancel()
    {
        FinalizedAt = SystemTime.UtcNow;

        var items = _externalEvents.ToArray();
        foreach (var item in items)
        {
            ItemSyncFailed(item, "syncRunCanceled");
        }
    }
}

public record ExternalEventResult
{
#pragma warning disable CS8618
    public ExternalEventId ExternalEventId { get; set; }
    public string ErrorCode { get; set; }
#pragma warning restore CS8618
}
