using MediatR;
using PollsApp.Domain.Aggregates;
using PollsApp.Infrastructure.Data.Repositories.Interfaces;

namespace PollsApp.Application.Queries.Handlers;

public class GetAllPollsQueryHandler : IRequestHandler<GetAllPollsQuery, IEnumerable<PollSummary>>
{
    private readonly IPollRepository pollRepository;

    public GetAllPollsQueryHandler(IPollRepository pollRepository)
    {
        this.pollRepository = pollRepository;
    }

    public async Task<IEnumerable<PollSummary>> Handle(GetAllPollsQuery request, CancellationToken cancellationToken)
    {
        var polls = await pollRepository.GetAllAsync().ConfigureAwait(false);

        var options = await pollRepository.GetOptionsByPollsIdsAsync(polls.Select(p => p.Id).ToList()).ConfigureAwait(false);

        return polls.Select(p => new PollSummary(p, options.Where(o => o.PollId == p.Id)));
    }
}
