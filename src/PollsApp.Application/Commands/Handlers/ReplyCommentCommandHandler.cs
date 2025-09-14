using MediatR;
using PollsApp.Domain.Exceptions;
using PollsApp.Domain.Repositories;
using PollsApp.Infrastructure.Events.Interfaces;

namespace PollsApp.Application.Commands.Handlers;

public class ReplyCommentCommandHandler(
    IUnitOfWork unitOfWork,
    IDomainEventDispatcher domainEventDispatcher
) : IRequestHandler<ReplyCommentCommand, Guid>
{
    private readonly IUnitOfWork unitOfWork = unitOfWork;
    private readonly IDomainEventDispatcher domainEventDispatcher = domainEventDispatcher;

    public async Task<Guid> Handle(ReplyCommentCommand request, CancellationToken cancellationToken)
    {
        var commentToReply = await unitOfWork.PollCommentRepository.GetByIdAsync(request.CommentIdToReply).ConfigureAwait(false);

        if (commentToReply == null || commentToReply.IsDeleted)
            throw new ArgumentException($"Comment with ID {request.CommentIdToReply} not found.");

        var poll = await unitOfWork.PollRepository.GetByIdAsync(commentToReply.PollId).ConfigureAwait(false);

        if (poll == null || poll.IsDeleted)
            throw new NotFoundException("Poll", commentToReply.PollId);

        if (!poll.IsOpen)
            throw new InvalidStateException("This poll is closed.");

        var replyComment = commentToReply.Reply(request.Comment, request.UserId);

        await unitOfWork.PollCommentRepository.InsertAsync(replyComment).ConfigureAwait(false);

        await domainEventDispatcher.Dispatch(replyComment.Events).ConfigureAwait(false);

        return replyComment.Id;
    }
}
