using MediatR;
using PollsApp.Domain.Aggregates;
using PollsApp.Domain.Exceptions;
using PollsApp.Domain.Repositories;

namespace PollsApp.Application.Queries.Handlers;

public class GetCommentsByPollIdQueryHandler(
    IUnitOfWork unitOfWork
) : IRequestHandler<GetCommentsByPollIdQuery, IEnumerable<CommentSummary>>
{
    private readonly IUnitOfWork unitOfWork = unitOfWork;

    public async Task<IEnumerable<CommentSummary>> Handle(GetCommentsByPollIdQuery request, CancellationToken cancellationToken)
    {
        var poll = await unitOfWork.PollRepository.GetByIdAsync(request.PollId).ConfigureAwait(false);

        if (poll == null || poll.IsDeleted)
            throw new NotFoundException("Poll", request.PollId);

        return await unitOfWork.PollCommentRepository.GetCommentsByPollAsync(request.PollId).ConfigureAwait(false);
    }
}
