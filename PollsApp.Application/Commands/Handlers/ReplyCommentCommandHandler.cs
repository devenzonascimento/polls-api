using MediatR;
using PollsApp.Domain.Entities;
using PollsApp.Infrastructure.Data.Repositories.Interfaces;
using PollsApp.Infrastructure.Events.Interfaces;

namespace PollsApp.Application.Commands.Handlers;

public class ReplyCommentCommandHandler : IRequestHandler<ReplyCommentCommand, Guid>
{
    private readonly IPollRepository pollRepository;
    private readonly IPollCommentRepository pollCommentRepository;
    private readonly IDomainEventDispatcher domainEventDispatcher;

    public ReplyCommentCommandHandler(
        IPollRepository pollRepository,
        IPollCommentRepository pollCommentRepository,
        IDomainEventDispatcher domainEventDispatcher
    )
    {
        this.pollRepository = pollRepository;
        this.pollCommentRepository = pollCommentRepository;
        this.domainEventDispatcher = domainEventDispatcher;
    }

    public async Task<Guid> Handle(ReplyCommentCommand request, CancellationToken cancellationToken)
    {
        var commentToReply = await pollCommentRepository.GetByIdAsync(request.CommentIdToReply).ConfigureAwait(false);

        if (commentToReply == null || commentToReply.IsDeleted)
            throw new ArgumentException($"Comment with ID {request.CommentIdToReply} not found.");

        var poll = await pollRepository.GetByIdAsync(commentToReply.PollId).ConfigureAwait(false);

        if (poll == null || poll.IsDeleted)
            throw new ArgumentException($"Poll with ID {commentToReply.PollId} not found.");

        if (!poll.IsOpen)
            throw new ArgumentException("This poll is closed.");

        var replyComment = commentToReply.Reply(request.Comment, request.UserId);

        await pollCommentRepository.InsertAsync(replyComment).ConfigureAwait(false);

        await domainEventDispatcher.Dispatch(replyComment.Events).ConfigureAwait(false);

        return replyComment.Id;
    }
}
