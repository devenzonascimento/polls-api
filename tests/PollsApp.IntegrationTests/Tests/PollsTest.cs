using PollsApp.Application.Commands;
using Dapper;
using PollsApp.Infrastructure.Data.Repositories;
using PollsApp.IntegrationTests.Abstractions;

namespace PollsApp.IntegrationTests.Tests;

public class PollsTest(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Register_ShouldBla_Haha()
    {
        var user = await DataSeeder.GenerateUserAsync();

        var command = new CreatePollCommand(
            UserRequesterId: user.Id,
            Title: "Title",
            Description: "Description",
            AllowMultiple: false,
            ClosesAt: DateTime.UtcNow.AddMonths(3),
            Options: ["Option 1", "Option 2"]
        );

        var savedPollId = await Sender.Send(command);

        var savedPoll = await DbConnection.QueryAsync<PollDao>(
            sql: "SELECT * FROM polls p WHERE p.id = @id",
            param: new { id = savedPollId }
        );

        Assert.NotNull(savedPoll);
    }
}
