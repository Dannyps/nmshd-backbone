using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Synchronization.Application.Infrastructure.Persistence.Repository;

public interface ISyncRunsRepository
{
    Task<SyncRun> Find(SyncRunId syncRunId, IdentityAddress createdBy, CancellationToken cancellationToken, bool track = false);
    Task<SyncRun> FindPrevious(IdentityAddress createdBy, CancellationToken cancellationToken, bool track = false);
    Task<bool> IsActiveSyncRunAvailable(IdentityAddress createdBy, CancellationToken cancellationToken);
    void Update(SyncRun syncRun);
    void Add(SyncRun syncRun);
}
