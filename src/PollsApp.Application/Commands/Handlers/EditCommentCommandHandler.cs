using MediatR;
using PollsApp.Domain.Exceptions;
using PollsApp.Domain.Repositories;
using PollsApp.Infrastructure.Events.Interfaces;

namespace PollsApp.Application.Commands.Handlers;

public class EditCommentCommandHandler(
    IUnitOfWork unitOfWork,
    IDomainEventDispatcher domainEventDispatcher
) : IRequestHandler<EditCommentCommand, bool>
{
    private readonly IUnitOfWork unitOfWork = unitOfWork;
    private readonly IDomainEventDispatcher domainEventDispatcher = domainEventDispatcher;

    public async Task<bool> Handle(EditCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = await unitOfWork.PollCommentRepository.GetByIdAsync(request.CommentId).ConfigureAwait(false);

        if (comment == null || comment.IsDeleted)
            throw new ArgumentException($"Comment with ID {request.CommentId} not found.");

        var poll = await unitOfWork.PollRepository.GetByIdAsync(comment.PollId).ConfigureAwait(false);

        if (poll == null || poll.IsDeleted)
            throw new NotFoundException("Poll", comment.PollId);

        if (!poll.IsOpen)
            throw new InvalidStateException("This poll is closed.");

        comment.Edit(request.NewComment, request.UserId);

        await unitOfWork.PollCommentRepository.UpdateAsync(comment).ConfigureAwait(false);

        await domainEventDispatcher.Dispatch(comment.Events).ConfigureAwait(false);

        return true;
    }
}
