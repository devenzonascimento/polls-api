using MediatR;
using PollsApp.Application.Services.Interfaces;
using PollsApp.Domain.Aggregates;
using PollsApp.Domain.Repositories;

namespace PollsApp.Application.Queries.Handlers;

public class SearchPollsQueryHandler(
    IUnitOfWork unitOfWork,
    IPollSearchService pollSearchService
) : IRequestHandler<SearchPollsQuery, IEnumerable<PollSummary>>
{
    private readonly IUnitOfWork unitOfWork = unitOfWork;
    private readonly IPollSearchService pollSearchService = pollSearchService;

    public async Task<IEnumerable<PollSummary>> Handle(SearchPollsQuery request, CancellationToken cancellationToken)
    {
        var pollsIdsFound = await pollSearchService.SearchAsync(request.Search, request.IsOpen).ConfigureAwait(false);

        return await unitOfWork.PollRepository.GetPollsSummariesByIdsAsync(pollsIdsFound).ConfigureAwait(false);
    }
}
