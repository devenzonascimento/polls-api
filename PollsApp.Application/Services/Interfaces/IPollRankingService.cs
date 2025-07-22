namespace PollsApp.Application.Services.Interfaces;

public interface IPollRankingService
{
    Task IncrementVoteAsync(Guid pollId);
    Task DecrementVoteAsync(Guid pollId);
    Task<IList<(Guid pollId, long score)>> GetTopPollsAsync(int count);
}
