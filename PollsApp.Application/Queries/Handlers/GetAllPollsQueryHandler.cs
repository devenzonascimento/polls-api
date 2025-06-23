using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using PollsApp.Domain.Aggregates;
using PollsApp.Infrastructure.Data.Repositories.Interfaces;

namespace PollsApp.Application.Queries.Handlers;

public class GetAllPollsQueryHandler : IRequestHandler<GetAllPollsQuery, IEnumerable<PollSummary>>
{
    private readonly IPollRepository pollRepository;
    private readonly IPollOptionRepository pollOptionRepository;

    public GetAllPollsQueryHandler(IPollRepository pollRepository, IPollOptionRepository pollOptionRepository)
    {
        this.pollRepository = pollRepository;
        this.pollOptionRepository = pollOptionRepository;
    }

    public async Task<IEnumerable<PollSummary>> Handle(GetAllPollsQuery request, CancellationToken cancellationToken)
    {
        var polls = await pollRepository.GetAllAsync().ConfigureAwait(false);

        var options = await pollOptionRepository.GetByPollsIdsAsync(polls.Select(p => p.Id).ToList()).ConfigureAwait(false);

        return polls.Select(p => new PollSummary(p, options.Where(o => o.PollId == p.Id)));
    }
}
