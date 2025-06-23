using PollsApp.Domain.Entities;

namespace PollsApp.Infrastructure.Data.Repositories.Interfaces;

public interface IVoteRepository : IBaseRepository<IVoteRepository>
{
    Task<Vote> SaveAsync(Vote vote);
    Task<IEnumerable<Vote>> SaveAsync(IEnumerable<Vote> votes);
    Task<Vote?> GetByIdAsync(Guid id);
    Task<IEnumerable<Vote>> GetAllByPollIdAsync(Guid pollId);
    Task<IEnumerable<Vote>> GetAllByPollOptionIdAsync(Guid pollOptionId);
    Task<Vote?> FindUserVote(Guid pollId, Guid userId);
}
