using MediatR;
using PollsApp.Domain.Aggregates;
using PollsApp.Domain.Exceptions;
using PollsApp.Infrastructure.Data.Repositories.Interfaces;

namespace PollsApp.Application.Queries.Handlers;

public class GetCommentsByPollIdQueryHandler : IRequestHandler<GetCommentsByPollIdQuery, IEnumerable<CommentSummary>>
{
    private readonly IPollRepository pollRepository;
    private readonly IPollCommentRepository pollCommentRepository;

    public GetCommentsByPollIdQueryHandler(IPollRepository pollRepository, IPollCommentRepository pollCommentRepository)
    {
        this.pollRepository = pollRepository;
        this.pollCommentRepository = pollCommentRepository;
    }

    public async Task<IEnumerable<CommentSummary>> Handle(GetCommentsByPollIdQuery request, CancellationToken cancellationToken)
    {
        var poll = await pollRepository.GetByIdAsync(request.PollId).ConfigureAwait(false);

        if (poll == null || poll.IsDeleted)
            throw new NotFoundException("Poll", request.PollId);

        return await pollCommentRepository.GetCommentsByPollAsync(request.PollId).ConfigureAwait(false);
    }
}
