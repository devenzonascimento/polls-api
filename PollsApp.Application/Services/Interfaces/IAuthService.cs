namespace PollsApp.Application.Services.Interfaces;

public interface IAuthService
{
    Task RegisterAsync(string username, string email, string password);
    Task<string> LoginAsync(string email, string password);
}
