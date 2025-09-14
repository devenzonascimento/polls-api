using MediatR;
using PollsApp.Domain.Aggregates;
using PollsApp.Domain.Exceptions;
using PollsApp.Domain.Repositories;

namespace PollsApp.Application.Queries.Handlers;

public class GetPollByIdQueryHandler(
    IUnitOfWork unitOfWork
) : IRequestHandler<GetPollByIdQuery, PollSummary>
{
    private readonly IUnitOfWork unitOfWork = unitOfWork;

    public async Task<PollSummary> Handle(GetPollByIdQuery request, CancellationToken cancellationToken)
    {
        var pollSummary = await unitOfWork.PollRepository.GetPollSummaryAsync(request.PollId).ConfigureAwait(false);

        if (pollSummary == null)
            throw new NotFoundException("Poll", request.PollId);

        return pollSummary;
    }
}
