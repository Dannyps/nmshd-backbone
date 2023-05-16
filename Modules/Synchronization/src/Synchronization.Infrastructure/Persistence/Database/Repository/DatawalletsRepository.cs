using System.Data;
using System.Data.Common;
using Backbone.Modules.Synchronization.Application.Extensions;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Synchronization.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Extensions;
using Enmeshed.BuildingBlocks.Application.Pagination;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;
using NpgsqlTypes;

namespace Backbone.Modules.Synchronization.Infrastructure.Persistence.Database.Repository;

public class DatawalletsRepository : IDatawalletsRepository
{
    private readonly SynchronizationDbContext _dbContext;
    private readonly DbSet<Datawallet> _datawallets;
    private readonly DbSet<DatawalletModification> _datawalletModifications;
    private readonly IQueryable<Datawallet> _readOnlyDatawallets;
    private readonly IBlobStorage _blobStorage;
    private readonly BlobOptions _blobOptions;

    public DatawalletsRepository(SynchronizationDbContext dbContext, IBlobStorage blobStorage, IOptions<BlobOptions> blobOptions)
    {
        _datawallets = dbContext.Datawallets;
        _readOnlyDatawallets = dbContext.Datawallets.AsNoTracking();
        _blobStorage = blobStorage;
        _blobOptions = blobOptions.Value;
    }

    public void Add(Datawallet datawallet)
    {
        SaveNewModifications(datawallet);
        _datawallets.Add(datawallet);
    }

    public void Update(Datawallet datawallet)
    {
        SaveNewModifications(datawallet);
        _datawallets.Update(datawallet);
    }

    public async Task<Datawallet> Find(IdentityAddress owner, CancellationToken cancellationToken, bool track = false)
    {
        return await (track ? _datawallets : _readOnlyDatawallets)
        .AsQueryable()
        .OfOwner(owner, cancellationToken);
    }

    public async Task<Datawallet> FindDatawalletForInsertion(IdentityAddress owner, CancellationToken cancellationToken, bool track = false)
    {
        return await (track ? _datawallets : _readOnlyDatawallets)
        .AsQueryable()
        .WithLatestModification(owner)
        .OfOwner(owner, cancellationToken);
    }

    public async Task<DbPaginationResult<DatawalletModification>> GetDatawalletModifications(IdentityAddress activeIdentity, long? localIndex, PaginationFilter paginationFilter)
    {
        // Use SqlParameter here in order to define the type of the activeIdentity parameter explicitly. Otherwise nvarchar(4000) is used, which causes performance problems.
        // (https://docs.microsoft.com/en-us/archive/msdn-magazine/2009/brownfield/how-data-access-code-affects-database-performance)
        DbParameter activeIdentityParam = null;
        if (_dbContext.Database.IsSqlServer())
            activeIdentityParam = new SqlParameter("createdBy", SqlDbType.Char, IdentityAddress.MAX_LENGTH, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, activeIdentity.StringValue);
        else if (_dbContext.Database.IsNpgsql())
            activeIdentityParam = new NpgsqlParameter("createdBy", NpgsqlDbType.Char, IdentityAddress.MAX_LENGTH, "", ParameterDirection.Input, false, 0, 0, DataRowVersion.Default, activeIdentity.StringValue);
        else
            activeIdentityParam = new SqliteParameter("createdBy", activeIdentity.StringValue);

        var paginationResult = _dbContext.Database.IsNpgsql()
            ? await _datawalletModifications
            .FromSqlInterpolated($@"SELECT * FROM(SELECT *, ROW_NUMBER() OVER(PARTITION BY ""ObjectIdentifier"", ""Type"", ""PayloadCategory"" ORDER BY ""Index"" DESC) AS rank FROM ""Synchronization"".""DatawalletModifications"" m1 WHERE ""CreatedBy"" = {activeIdentityParam} AND ""Index"" > {localIndex ?? -1}) AS ignoreDuplicates WHERE rank = 1")
            .AsNoTracking()
            .OrderAndPaginate(m => m.Index, paginationFilter)
            : await _datawalletModifications
            .FromSqlInterpolated($"SELECT * FROM(SELECT *, ROW_NUMBER() OVER(PARTITION BY ObjectIdentifier, Type, PayloadCategory ORDER BY [Index] DESC) AS rank FROM [DatawalletModifications] m1 WHERE CreatedBy = {activeIdentityParam} AND [Index] > {localIndex ?? -1}) AS ignoreDuplicates WHERE rank = 1")
            .AsNoTracking()
            .OrderAndPaginate(m => m.Index, paginationFilter);

        return paginationResult;
    }

    private void SaveNewModifications(Datawallet datawallet)
    {
        foreach (var newModification in datawallet.NewModifications)
        {
            if (newModification.EncryptedPayload != null)
                _blobStorage.Add(_blobOptions.RootFolder, newModification.Id, newModification.EncryptedPayload);
        }
    }



}
