using PollsApp.Domain.Aggregates;
using PollsApp.Domain.Entities;

namespace PollsApp.Infrastructure.Data.Repositories.Interfaces;

public interface IPollRepository : IBaseRepository<IPollRepository>
{
    Task<Poll> SaveAsync(Poll poll);
    Task InsertAsync(Poll poll);
    Task UpdateAsync(Poll poll);
    Task<IEnumerable<Poll>> SaveAsync(IEnumerable<Poll> polls);
    Task<PollOption> SaveAsync(PollOption pollOption);
    Task<IEnumerable<PollOption>> SaveAsync(IEnumerable<PollOption> pollOptions);
    Task<IEnumerable<Poll>> GetAllAsync();
    Task<Poll?> GetByIdAsync(Guid id);
    Task<PollOption?> GetOptionByIdAsync(Guid id);
    Task<IEnumerable<PollOption>> GetOptionsByPollsIdsAsync(IList<Guid> pollsIds);
    Task<IEnumerable<PollOption>> GetOptionsByPollIdAsync(Guid pollId);
    Task<PollSummary> GetPollSummaryAsync(Guid pollId);
    Task<IEnumerable<PollSummary>> GetPollsSummariesAsync();
    Task<IEnumerable<PollSummary>> GetPollsSummariesByIdsAsync(IEnumerable<Guid> pollsIds);
    Task DeleteOptionByIdAsync(Guid id);
    Task DeleteOptionsByIdsAsync(IEnumerable<Guid> ids);
}
