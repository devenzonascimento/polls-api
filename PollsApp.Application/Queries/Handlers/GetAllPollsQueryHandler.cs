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
        return await pollRepository.GetPollsSummariesAsync().ConfigureAwait(false);
    }
}
