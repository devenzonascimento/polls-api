using MediatR;
using PollsApp.Application.Services.Interfaces;
using PollsApp.Domain.Aggregates;
using PollsApp.Infrastructure.Data.Repositories.Interfaces;

namespace PollsApp.Application.Queries.Handlers;

public class GetRankedPollsQueryHandler : IRequestHandler<GetRankedPollsQuery, IEnumerable<PollSummary>>
{
    private readonly IPollRepository pollRepository;
    private readonly IPollRankingService pollRankingService;

    public GetRankedPollsQueryHandler(IPollRepository pollRepository, IPollRankingService pollRankingService)
    {
        this.pollRepository = pollRepository;
        this.pollRankingService = pollRankingService;
    }

    public async Task<IEnumerable<PollSummary>> Handle(GetRankedPollsQuery request, CancellationToken cancellationToken)
    {
        var rankedPolls = await pollRankingService.GetTopPollsAsync(10).ConfigureAwait(false);

        if (rankedPolls == null || !rankedPolls.Any())
            return Enumerable.Empty<PollSummary>();

        var polls = await pollRepository.GetPollsSummariesByIdsAsync(rankedPolls.Select(p => p.pollId)).ConfigureAwait(false);

        return polls;
    }
}
