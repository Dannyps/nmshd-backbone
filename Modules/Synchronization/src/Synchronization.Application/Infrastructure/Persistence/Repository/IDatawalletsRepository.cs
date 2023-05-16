using Backbone.Modules.Synchronization.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Pagination;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Synchronization.Application.Infrastructure.Persistence.Repository;

public interface IDatawalletsRepository
{
    Task<Datawallet> Find(IdentityAddress owner, CancellationToken cancellationToken, bool track = false);
    Task<Datawallet> FindDatawalletForInsertion(IdentityAddress owner, CancellationToken cancellationToken, bool track = false);
    Task<DbPaginationResult<DatawalletModification>> GetDatawalletModifications(IdentityAddress activeIdentity, long? localIndex, PaginationFilter paginationFilter);
    void Add(Datawallet datawallet);
    void Update(Datawallet datawallet);
}
