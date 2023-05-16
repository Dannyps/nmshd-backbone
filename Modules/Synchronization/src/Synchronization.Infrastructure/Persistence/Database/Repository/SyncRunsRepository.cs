using Backbone.Modules.Synchronization.Application.Extensions;
using Backbone.Modules.Synchronization.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Synchronization.Infrastructure.Persistence.Database.Repository;

public class SyncRunsRepository : ISyncRunsRepository
{
    private readonly DbSet<SyncRun> _syncRuns;
    private readonly IQueryable<SyncRun> _readOnlySyncRuns;

    public SyncRunsRepository(SynchronizationDbContext dbContext)
    {
        _syncRuns = dbContext.SyncRuns;
        _readOnlySyncRuns = dbContext.SyncRuns.AsNoTracking();
    }

    public async Task<SyncRun> Find(SyncRunId syncRunId, IdentityAddress createdBy, CancellationToken cancellationToken, bool track = false)
    {
        return await (track ? _syncRuns : _readOnlySyncRuns)
          .AsQueryable()
          .IncludeExternalEvents()
          .CreatedBy(createdBy)
          .Where(s => s.Id == syncRunId)
          .GetFirst(cancellationToken);
    }

    public async Task<SyncRun> FindPrevious(IdentityAddress createdBy, CancellationToken cancellationToken, bool track = false)
    {
        var previousSyncRun = await (track ? _syncRuns : _readOnlySyncRuns)
            .AsQueryable()
            .Include(s => s.ExternalEvents)
            .CreatedBy(createdBy)
            .OrderByDescending(s => s.Index)
            .FirstOrDefaultAsync(cancellationToken);

        return previousSyncRun;
    }

    public async Task<bool> IsActiveSyncRunAvailable(IdentityAddress createdBy, CancellationToken cancellationToken)
    {
        return await _readOnlySyncRuns
            .CreatedBy(createdBy)
            .NotFinalized()
            .Select(s => true)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public void Add(SyncRun syncRun)
    {
        _syncRuns.Add(syncRun);
    }

    public void Update(SyncRun syncRun)
    {
        _syncRuns.Update(syncRun);
    }
}
