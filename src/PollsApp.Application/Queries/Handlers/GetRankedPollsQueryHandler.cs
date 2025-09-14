using MediatR;
using PollsApp.Application.Services.Interfaces;
using PollsApp.Domain.Aggregates;
using PollsApp.Domain.Repositories;

namespace PollsApp.Application.Queries.Handlers;

public class GetRankedPollsQueryHandler(
    IUnitOfWork unitOfWork,
    IPollRankingService pollRankingService
) : IRequestHandler<GetRankedPollsQuery, IEnumerable<PollSummary>>
{
    private readonly IUnitOfWork unitOfWork = unitOfWork;
    private readonly IPollRankingService pollRankingService = pollRankingService;

    public async Task<IEnumerable<PollSummary>> Handle(GetRankedPollsQuery request, CancellationToken cancellationToken)
    {
        var rankedPolls = await pollRankingService.GetTopPollsAsync(10).ConfigureAwait(false);

        if (rankedPolls == null || !rankedPolls.Any())
            return [];

        var polls = await unitOfWork.PollRepository.GetPollsSummariesByIdsAsync(rankedPolls.Select(p => p.pollId)).ConfigureAwait(false);

        return polls;
    }
}
