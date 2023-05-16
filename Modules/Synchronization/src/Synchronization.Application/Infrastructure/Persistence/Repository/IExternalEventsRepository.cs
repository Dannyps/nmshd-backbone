using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Pagination;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Synchronization.Application.Infrastructure.Persistence.Repository;

public interface IExternalEventsRepository
{
    void Add(ExternalEvent externalEvent);
    Task<long> FindNextIndexForIdentity(IdentityAddress identity);
    Task<DbPaginationResult<ExternalEvent>> FindExternalEventsOfSyncRun(PaginationFilter paginationFilter, IdentityAddress owner, SyncRunId syncRunId, CancellationToken cancellationToken);
    Task<List<ExternalEvent>> FindUnsynced(IdentityAddress owner, byte maxErrorCount, CancellationToken cancellationToken);
}
