using PollsApp.Application.Services.Interfaces;
using StackExchange.Redis;

namespace PollsApp.Application.Services;

public class PollRankingService : IPollRankingService
{
    private const string KEY = "poll:ranking";
    private readonly IDatabase db;

    public PollRankingService(IConnectionMultiplexer mux)
      => db = mux.GetDatabase();

    public Task IncrementVoteAsync(Guid pollId)
      => db.SortedSetIncrementAsync(KEY, pollId.ToString(), 1);

    public Task DecrementVoteAsync(Guid pollId)
      => db.SortedSetDecrementAsync(KEY, pollId.ToString(), 1);

    public async Task<IList<(Guid, long)>> GetTopPollsAsync(int count)
    {
        var entries = await db.SortedSetRangeByRankWithScoresAsync(
            KEY, 0, count - 1, Order.Descending
        ).ConfigureAwait(false);

        return entries
          .Select(e => (Guid.Parse(e.Element), (long)e.Score))
          .ToList();
    }
}
