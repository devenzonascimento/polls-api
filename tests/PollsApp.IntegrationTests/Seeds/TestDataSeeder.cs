using System.Data;
using PollsApp.Application.Services;
using PollsApp.Domain.Entities;
using PollsApp.Infrastructure.Data.Repositories;

namespace PollsApp.IntegrationTests.Seeds;

public class TestDataSeeder(IDbConnection connection)
{
    private readonly UserRepository userRepository = new UserRepository(connection);

    public async Task<User> GenerateUserAsync()
    {
        var authService = new AuthService(userRepository, config: null);

        await authService.RegisterAsync("John Due", "johndue@example.com", "12345678");

        return await userRepository.GetByEmailAsync("johndue@example.com");
    }
}
