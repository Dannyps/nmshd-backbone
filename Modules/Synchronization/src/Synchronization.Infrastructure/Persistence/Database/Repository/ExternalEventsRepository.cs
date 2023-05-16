using Backbone.Modules.Synchronization.Application.Extensions;
using Backbone.Modules.Synchronization.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Extensions;
using Enmeshed.BuildingBlocks.Application.Pagination;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Synchronization.Infrastructure.Persistence.Database.Repository;

public class ExternalEventsRepository : IExternalEventsRepository
{
    private readonly DbSet<ExternalEvent> _externalEvents;
    private readonly IQueryable<ExternalEvent> _readOnlyExternalEvents;

    public ExternalEventsRepository(SynchronizationDbContext dbContext)
    {
        _externalEvents = dbContext.ExternalEvents;
        _readOnlyExternalEvents = dbContext.ExternalEvents.AsNoTracking();
    }

    public void Add(ExternalEvent externalEvent)
    {
        _externalEvents.Add(externalEvent);
    }

    public async Task<long> FindNextIndexForIdentity(IdentityAddress identity)
    {
        var latestIndex = await _externalEvents
            .AsQueryable()
            .WithOwner(identity)
            .OrderByDescending(s => s.Index)
            .Select(s => (long?)s.Index)
            .FirstOrDefaultAsync();

        if (latestIndex == null)
            return 0;

        return latestIndex.Value + 1;
    }

    public async Task<DbPaginationResult<ExternalEvent>> FindExternalEventsOfSyncRun(PaginationFilter paginationFilter, IdentityAddress owner, SyncRunId syncRunId, CancellationToken cancellationToken)
    {
        var query = await _externalEvents
            .AsQueryable()
            .WithOwner(owner)
            .AssignedToSyncRun(syncRunId)
            .OrderAndPaginate(x => x.Index, paginationFilter);

        return query;
    }

    public async Task<List<ExternalEvent>> FindUnsynced(IdentityAddress owner, byte maxErrorCount, CancellationToken cancellationToken)
    {
        var unsyncedEvents = await _externalEvents
            .WithOwner(owner)
            .Unsynced()
            .WithErrorCountBelow(maxErrorCount)
            .ToListAsync(cancellationToken);

        return unsyncedEvents;
    }
}
