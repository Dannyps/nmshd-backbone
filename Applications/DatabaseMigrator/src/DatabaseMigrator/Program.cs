﻿using System.Reflection;
using Autofac.Extensions.DependencyInjection;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Domain.Events;
using Backbone.DatabaseMigrator;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;
using Serilog.Settings.Configuration;
using IServiceCollectionExtensions = Backbone.Modules.Challenges.Infrastructure.Persistence.IServiceCollectionExtensions;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var app = CreateHostBuilder(args).Build();

    var executor = app.Services.GetRequiredService<Executor>();

    await executor.Execute();

    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    return 1;
}
finally
{
    await Log.CloseAndFlushAsync();
}


static IHostBuilder CreateHostBuilder(string[] args)
{
    return Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((hostContext, configuration) =>
        {
            configuration.Sources.Clear();
            var env = hostContext.HostingEnvironment;

            configuration
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.override.json", optional: true, reloadOnChange: true);

            if (env.IsDevelopment())
            {
                var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                configuration.AddUserSecrets(appAssembly, optional: true);
            }

            configuration.AddEnvironmentVariables();
            configuration.AddCommandLine(args);
        })
        .ConfigureServices((hostContext, services) =>
        {
            var configuration = hostContext.Configuration;

            services.ConfigureAndValidate<Configuration>(configuration.Bind);
            var parsedConfiguration = services.BuildServiceProvider().GetRequiredService<IOptions<Configuration>>().Value;

            services.AddSingleton<Executor>();

            services.AddSingleton<IEventBus, DummyEventBus>();

            # region Add db contexts

            IServiceCollectionExtensions.AddDatabase(services, options =>
            {
                options.Provider = parsedConfiguration.Infrastructure.SqlDatabase.Provider;
                options.DbConnectionString = parsedConfiguration.Infrastructure.SqlDatabase.ConnectionString;
                options.CommandTimeout = parsedConfiguration.Infrastructure.SqlDatabase.CommandTimeout;
            });

            Backbone.Modules.Devices.Infrastructure.Persistence.IServiceCollectionExtensions.AddDatabase(services, options =>
            {
                options.Provider = parsedConfiguration.Infrastructure.SqlDatabase.Provider;
                options.ConnectionString = parsedConfiguration.Infrastructure.SqlDatabase.ConnectionString;
                options.CommandTimeout = parsedConfiguration.Infrastructure.SqlDatabase.CommandTimeout;
            });

            Backbone.Modules.Files.Infrastructure.Persistence.Database.IServiceCollectionExtensions.AddDatabase(services, options =>
            {
                options.Provider = parsedConfiguration.Infrastructure.SqlDatabase.Provider;
                options.DbConnectionString = parsedConfiguration.Infrastructure.SqlDatabase.ConnectionString;
                options.CommandTimeout = parsedConfiguration.Infrastructure.SqlDatabase.CommandTimeout;
            });

            Backbone.Modules.Messages.Infrastructure.Persistence.Database.IServiceCollectionExtensions.AddDatabase(services, options =>
            {
                options.Provider = parsedConfiguration.Infrastructure.SqlDatabase.Provider;
                options.DbConnectionString = parsedConfiguration.Infrastructure.SqlDatabase.ConnectionString;
                options.CommandTimeout = parsedConfiguration.Infrastructure.SqlDatabase.CommandTimeout;
            });

            Backbone.Modules.Quotas.Infrastructure.Persistence.Database.IServiceCollectionExtensions.AddDatabase(services, options =>
            {
                options.Provider = parsedConfiguration.Infrastructure.SqlDatabase.Provider;
                options.DbConnectionString = parsedConfiguration.Infrastructure.SqlDatabase.ConnectionString;
                options.CommandTimeout = parsedConfiguration.Infrastructure.SqlDatabase.CommandTimeout;
            });

            Backbone.Modules.Relationships.Infrastructure.Persistence.Database.IServiceCollectionExtensions.AddDatabase(services, options =>
            {
                options.Provider = parsedConfiguration.Infrastructure.SqlDatabase.Provider;
                options.DbConnectionString = parsedConfiguration.Infrastructure.SqlDatabase.ConnectionString;
                options.CommandTimeout = parsedConfiguration.Infrastructure.SqlDatabase.CommandTimeout;
            });

            Backbone.Modules.Synchronization.Infrastructure.Persistence.Database.IServiceCollectionExtensions.AddDatabase(services, options =>
            {
                options.Provider = parsedConfiguration.Infrastructure.SqlDatabase.Provider;
                options.DbConnectionString = parsedConfiguration.Infrastructure.SqlDatabase.ConnectionString;
                options.CommandTimeout = parsedConfiguration.Infrastructure.SqlDatabase.CommandTimeout;
            });

            Backbone.Modules.Tokens.Infrastructure.Persistence.Database.IServiceCollectionExtensions.AddDatabase(services, options =>
            {
                options.Provider = parsedConfiguration.Infrastructure.SqlDatabase.Provider;
                options.DbConnectionString = parsedConfiguration.Infrastructure.SqlDatabase.ConnectionString;
                options.CommandTimeout = parsedConfiguration.Infrastructure.SqlDatabase.CommandTimeout;
            });

            Backbone.AdminApi.Infrastructure.Persistence.IServiceCollectionExtensions.AddDatabase(services, options =>
            {
                options.Provider = parsedConfiguration.Infrastructure.SqlDatabase.Provider;
                options.ConnectionString = parsedConfiguration.Infrastructure.SqlDatabase.ConnectionString;
                options.CommandTimeout = parsedConfiguration.Infrastructure.SqlDatabase.CommandTimeout;
            });

            #endregion
        })
        .UseServiceProviderFactory(new AutofacServiceProviderFactory())
        .UseSerilog((context, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration, new ConfigurationReaderOptions { SectionName = "Logging" })
            .Enrich.WithDemystifiedStackTraces()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("service", "databasemigrator")
            .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
                .WithDefaultDestructurers()
                .WithDestructurers([new DbUpdateExceptionDestructurer()])
            )
        );
}

namespace Backbone.DatabaseMigrator
{
    internal class DummyEventBus : IEventBus
    {
        public void Publish(DomainEvent @event)
        {
        }

        public void StartConsuming()
        {
        }

        public void Subscribe<T, TH>() where T : DomainEvent where TH : IDomainEventHandler<T>
        {
        }
    }
}
