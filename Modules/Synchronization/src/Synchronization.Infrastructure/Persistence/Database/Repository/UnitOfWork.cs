using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Application.Infrastructure.Persistence.Repository;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Synchronization.Infrastructure.Persistence.Database.Repository;

public class UnitOfWork : IUnitOfWork
{
    private readonly SynchronizationDbContext _dbContext;
    private readonly IBlobStorage _blobStorage;
    private readonly IOptions<BlobOptions> _blobOptions;
    private ISyncRunsRepository _syncRunsRepository;
    private IExternalEventsRepository _externalEventsRepository;
    private IDatawalletsRepository _datawalletsRepository;

    public UnitOfWork(SynchronizationDbContext dbContext, IBlobStorage blobStorage, IOptions<BlobOptions> blobOptions)
    {
        _dbContext = dbContext;
        _blobStorage = blobStorage;
        _blobOptions = blobOptions;
    }

    public ISyncRunsRepository SyncRunsRepository
    {
        get
        {
            _syncRunsRepository ??= new SyncRunsRepository(_dbContext);
            return _syncRunsRepository;
        }
    }

    public IExternalEventsRepository ExternalEventsRepository
    {
        get
        {
            _externalEventsRepository ??= new ExternalEventsRepository(_dbContext);
            return _externalEventsRepository;
        }
    }

    public IDatawalletsRepository DatawalletsRepository
    {
        get
        {
            _datawalletsRepository ??= new DatawalletsRepository(_dbContext, _blobStorage, _blobOptions);
            return _datawalletsRepository;
        }
    }

    public async Task Save(CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
        await _blobStorage.SaveAsync();
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}
