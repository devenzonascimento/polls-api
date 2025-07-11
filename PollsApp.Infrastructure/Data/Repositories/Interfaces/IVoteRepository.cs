using PollsApp.Domain.Entities;

namespace PollsApp.Infrastructure.Data.Repositories.Interfaces;

public interface IVoteRepository : IBaseRepository<IVoteRepository>
{
    Task<Vote> SaveAsync(Vote vote);
    Task<IEnumerable<Vote>> SaveAsync(IEnumerable<Vote> votes);
    Task DeleteByIdAsync(Guid voteId);
    Task<Vote?> GetByIdAsync(Guid id);
    Task<IEnumerable<Vote>> GetAllByPollIdAsync(Guid pollId);
    Task<IEnumerable<Vote>> GetAllByPollOptionIdAsync(Guid pollOptionId);
    Task<Vote?> FindUniqueVoteByPollAsync(Guid pollId, Guid userId);
    Task<Vote?> FindUniqueVoteByOptionAsync(Guid pollOptionId, Guid userId);
}
