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
        var poll = await pollRepository.GetByIdAsync(request.PollId).ConfigureAwait(false);

        if (poll == null)
            throw new ArgumentException($"Poll with ID {request.PollId} not found.");

        var options = await pollRepository.GetOptionsByPollIdAsync(request.PollId).ConfigureAwait(false);

        return new PollSummary(poll, options);
    }
}
