using MediatR;
using PollsApp.Domain.Aggregates;
using PollsApp.Infrastructure.Data.Repositories.Interfaces;

namespace PollsApp.Application.Queries.Handlers;

public class GetPollByIdQueryHandler : IRequestHandler<GetPollByIdQuery, PollSummary>
{
    private readonly IPollRepository pollRepository;
    private readonly IPollOptionRepository pollOptionRepository;

    public GetPollByIdQueryHandler(IPollRepository pollRepository, IPollOptionRepository pollOptionRepository)
    {
        this.pollRepository = pollRepository;
        this.pollOptionRepository = pollOptionRepository;
    }

    public async Task<PollSummary> Handle(GetPollByIdQuery request, CancellationToken cancellationToken)
    {
        var poll = await pollRepository.GetByIdAsync(request.PollId).ConfigureAwait(false);

        if (poll == null)
        {
            throw new KeyNotFoundException($"Poll with ID {request.PollId} not found.");
        }

        var options = await pollOptionRepository.GetByPollIdAsync(request.PollId).ConfigureAwait(false);

        return new PollSummary(poll, options);
    }
}
