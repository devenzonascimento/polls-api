using PollsApp.Domain.Entities;

namespace PollsApp.Infrastructure.Data.Repositories.Interfaces;

public interface IPollRepository : IBaseRepository<IPollRepository>
{
    Task<Poll> SaveAsync(Poll poll);
    Task<IEnumerable<Poll>> SaveAsync(IEnumerable<Poll> polls);
    Task<IEnumerable<Poll>> GetAllAsync();
    Task<Poll?> GetByIdAsync(Guid id);
}
