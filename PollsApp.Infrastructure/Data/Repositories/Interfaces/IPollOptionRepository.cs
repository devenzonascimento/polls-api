using PollsApp.Domain.Entities;

namespace PollsApp.Infrastructure.Data.Repositories.Interfaces;

public interface IPollOptionRepository : IBaseRepository<IPollOptionRepository>
{
    Task<PollOption> SaveAsync(PollOption pollOption);
    Task<IEnumerable<PollOption>> SaveAsync(IEnumerable<PollOption> pollOptions);
    Task<PollOption?> GetByIdAsync(Guid id);
    Task<IEnumerable<PollOption>> GetByPollsIdsAsync(IList<Guid> pollsIds);
    Task<IEnumerable<PollOption>> GetByPollIdAsync(Guid pollId);
}
