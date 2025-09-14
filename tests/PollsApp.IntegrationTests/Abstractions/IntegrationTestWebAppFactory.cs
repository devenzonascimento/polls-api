using System.Data;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Processors;
using Hangfire;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PollsApp.Api.Extensions;
using StackExchange.Redis;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace PollsApp.IntegrationTests.Abstractions;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:15")
        .WithDatabase("pollsdb")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private readonly RedisContainer cacheContainer = new RedisBuilder()
        .WithImage("redis:7")
        .Build();

    // TODO: CONFIGURAR REDIS, OPENSEARCH, ETC...
    // TODO: CRIAR TESTES MAIS COMPLEXOS
    // TODO: ADICIONAR UM UNIT_OF_WORK PARA FACILITAR A VIDA
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, configBuilder) =>
        {
            var testConfig = new Dictionary<string, string?>
            {
                ["ConnectionStrings:PostgreSql"] = dbContainer.GetConnectionString(),
                ["ConnectionStrings:Redis"] = cacheContainer.GetConnectionString(),
            };

            configBuilder.AddInMemoryCollection(testConfig);
        });

        builder.ConfigureTestServices(services =>
        {
            List<Type> serviceTypesToRemove = [typeof(IDbConnection), typeof(IConnectionMultiplexer), typeof(IMigrationRunner)];

            var serviceDescriptorsToRemove = services.Where(
                s => serviceTypesToRemove.Contains(s.ServiceType)
                    || (s.ServiceType.Namespace?.StartsWith("Hangfire") ?? false)
            ).ToList();

            foreach (var serviceDescriptorToRemove in serviceDescriptorsToRemove)
                services.Remove(serviceDescriptorToRemove);

            var sp = services.BuildServiceProvider();
            var configuration = sp.GetRequiredService<IConfiguration>();

            if (configuration is not null)
            {
                services.AddDatabase(configuration);
                services.AddMigrations(configuration);
                services.AddRedis(configuration);
                services.AddHangfire(configuration);
            }

            // Necessário para a Migration rodar sem erros
            services.Configure<SelectingProcessorAccessorOptions>(options =>
            {
                options.ProcessorId = "PostgreSQL";
            });
        });
    }

    public async Task InitializeAsync()
    {
        await cacheContainer.StartAsync();
        await dbContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await cacheContainer.StopAsync();
        await dbContainer.StopAsync();
    }
}
