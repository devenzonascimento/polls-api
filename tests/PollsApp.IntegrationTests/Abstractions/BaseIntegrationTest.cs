using System.Data;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PollsApp.IntegrationTests.Seeds;

namespace PollsApp.IntegrationTests.Abstractions;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly IServiceScope _scope;
    protected readonly ISender Sender;
    protected readonly IDbConnection DbConnection;
    protected readonly TestDataSeeder DataSeeder;

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();

        Sender = _scope.ServiceProvider.GetRequiredService<ISender>();
        DbConnection = _scope.ServiceProvider.GetRequiredService<IDbConnection>();
        DataSeeder = new TestDataSeeder(DbConnection);
    }

    public Task DisposeAsync()
    {
        _scope?.Dispose();
        DbConnection?.Dispose();

        return Task.CompletedTask;
    }
}