using MediatR;
using PollsApp.Domain.Aggregates;
using PollsApp.Infrastructure.Data.Repositories.Interfaces;

namespace PollsApp.Application.Queries.Handlers;

public class GetPollByIdQueryHandler : IRequestHandler<GetPollByIdQuery, PollSummary>
{
    private readonly IPollRepository pollRepository;

    public GetPollByIdQueryHandler(IPollRepository pollRepository)
    {
        this.pollRepository = pollRepository;
    }

    public async Task<PollSummary> Handle(GetPollByIdQuery request, CancellationToken cancellationToken)
    {
        var pollSummary = await pollRepository.GetPollSummaryAsync(request.PollId).ConfigureAwait(false);

        if (pollSummary == null)
            throw new ArgumentException($"Poll with ID {request.PollId} not found.");

        return pollSummary;
    }
}
