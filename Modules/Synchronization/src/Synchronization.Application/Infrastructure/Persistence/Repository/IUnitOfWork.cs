namespace Backbone.Modules.Synchronization.Application.Infrastructure.Persistence.Repository;

public interface IUnitOfWork
{
    ISyncRunsRepository SyncRunsRepository { get; }
    IExternalEventsRepository ExternalEventsRepository { get; }
    IDatawalletsRepository DatawalletsRepository { get; }

    Task Save(CancellationToken cancellationToken);
    void Dispose();
}
