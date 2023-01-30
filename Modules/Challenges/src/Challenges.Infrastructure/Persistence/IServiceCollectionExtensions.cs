﻿using System.Reflection;
using Backbone.Modules.Challenges.Application.Infrastructure.Persistence;
using Backbone.Modules.Challenges.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Challenges.Infrastructure.Persistence;

public static class IServiceCollectionExtensions
{
    public static void AddDatabase(this IServiceCollection services, Action<DbOptions> setupOptions)
    {
        var options = new DbOptions();
        setupOptions?.Invoke(options);

        services.AddDbContext<ApplicationDbContext>(dbContextOptions =>
            dbContextOptions.UseSqlServer(options.DbConnectionString, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).GetTypeInfo().Assembly.GetName().Name);
                sqlOptions.EnableRetryOnFailure(options.RetryOptions.MaxRetryCount, TimeSpan.FromSeconds(options.RetryOptions.MaxRetryDelayInSeconds), null);
            }));

        services.AddScoped<IChallengesDbContext, ApplicationDbContext>();
    }

    public class DbOptions
    {
        public string DbConnectionString { get; set; }
        public RetryOptions RetryOptions { get; set; } = new();
    }

    public class RetryOptions
    {
        public byte MaxRetryCount { get; set; } = 15;
        public int MaxRetryDelayInSeconds { get; set; } = 30;
    }
}
