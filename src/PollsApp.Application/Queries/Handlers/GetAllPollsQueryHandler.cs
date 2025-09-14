using MediatR;
using PollsApp.Domain.Aggregates;
using PollsApp.Domain.Repositories;

namespace PollsApp.Application.Queries.Handlers;

public class GetAllPollsQueryHandler(
    IUnitOfWork unitOfWork
) : IRequestHandler<GetAllPollsQuery, IEnumerable<PollSummary>>
{
    private readonly IUnitOfWork unitOfWork = unitOfWork;

    public async Task<IEnumerable<PollSummary>> Handle(GetAllPollsQuery request, CancellationToken cancellationToken)
    {
        return await unitOfWork.PollRepository.GetPollsSummariesAsync().ConfigureAwait(false);
    }
}
