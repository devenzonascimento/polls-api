using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using PollsApp.Application.Services;
using PollsApp.Application.Services.Interfaces;
using PollsApp.Domain.Aggregates;
using PollsApp.Infrastructure.Data.Repositories.Interfaces;

namespace PollsApp.Application.Queries.Handlers;

public class SearchPollsQueryHandler : IRequestHandler<SearchPollsQuery, IEnumerable<PollSummary>>
{
    private readonly IPollRepository pollRepository;
    private readonly IPollSearchService pollSearchService;

    public SearchPollsQueryHandler(IPollRepository pollRepository, IPollSearchService pollSearchService)
    {
        this.pollRepository = pollRepository;
        this.pollSearchService = pollSearchService;
    }

    public async Task<IEnumerable<PollSummary>> Handle(SearchPollsQuery request, CancellationToken cancellationToken)
    {
        var pollsIdsFound = await pollSearchService.SearchAsync(request.Search, request.IsOpen).ConfigureAwait(false);

        return await pollRepository.GetPollsSummariesByIdsAsync(pollsIdsFound).ConfigureAwait(false);
    }
}
