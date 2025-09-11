using PollsApp.Domain.Entities;

namespace PollsApp.Infrastructure.Data.Repositories.Interfaces;

public interface IUserRepository : IBaseRepository<IUserRepository>
{
    Task<User> SaveAsync(User user);
    Task<IEnumerable<User>> SaveAsync(IEnumerable<User> users);
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
}
