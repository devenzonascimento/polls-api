using System.Data;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Processors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Testcontainers.PostgreSql;

namespace PollsApp.IntegrationTests.Abstractions;


public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:15")
        .WithDatabase("pollsdb")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var descriptor = services
                .SingleOrDefault(s => s.ServiceType == typeof(IDbConnection));

            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            var migratorDescriptors = services.Where(s =>
                s.ServiceType == typeof(IMigrationRunner) || s.ServiceType.Name.Contains("FluentMigrator")
            ).ToList();

            foreach (var migratorDescriptor in migratorDescriptors)
            {
                services.Remove(migratorDescriptor);
            }

            services.AddScoped<IDbConnection>(sp =>
                new NpgsqlConnection(dbContainer.GetConnectionString())
            );

            services.Configure<SelectingProcessorAccessorOptions>(options =>
            {
                options.ProcessorId = "PostgreSQL"; // Use o id disponível!
            });

            services.AddFluentMigratorCore().ConfigureRunner(rb => rb
                .AddPostgres()
                .WithGlobalConnectionString(dbContainer.GetConnectionString())
                .ScanIn(typeof(Infrastructure.Data.Migrations.CreateUsersTable).Assembly).For.Migrations()
            );
        });
    }

    public Task InitializeAsync()
    {
        return dbContainer.StartAsync();
    }

    public new Task DisposeAsync()
    {
        return dbContainer.StopAsync();
    }
}
