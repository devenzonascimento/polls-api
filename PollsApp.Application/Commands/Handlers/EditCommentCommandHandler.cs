using MediatR;
using PollsApp.Domain.Exceptions;
using PollsApp.Infrastructure.Data.Repositories.Interfaces;
using PollsApp.Infrastructure.Events.Interfaces;

namespace PollsApp.Application.Commands.Handlers;

public class EditCommentCommandHandler : IRequestHandler<EditCommentCommand, bool>
{
    private readonly IPollRepository pollRepository;
    private readonly IPollCommentRepository pollCommentRepository;
    private readonly IDomainEventDispatcher domainEventDispatcher;

    public EditCommentCommandHandler(
        IPollRepository pollRepository,
        IPollCommentRepository pollCommentRepository,
        IDomainEventDispatcher domainEventDispatcher
    )
    {
        this.pollRepository = pollRepository;
        this.pollCommentRepository = pollCommentRepository;
        this.domainEventDispatcher = domainEventDispatcher;
    }

    public async Task<bool> Handle(EditCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = await pollCommentRepository.GetByIdAsync(request.CommentId).ConfigureAwait(false);

        if (comment == null || comment.IsDeleted)
            throw new ArgumentException($"Comment with ID {request.CommentId} not found.");

        var poll = await pollRepository.GetByIdAsync(comment.PollId).ConfigureAwait(false);

        if (poll == null || poll.IsDeleted)
            throw new NotFoundException("Poll", comment.PollId);

        if (!poll.IsOpen)
            throw new InvalidStateException("This poll is closed.");

        comment.Edit(request.NewComment, request.UserId);

        await pollCommentRepository.UpdateAsync(comment).ConfigureAwait(false);

        await domainEventDispatcher.Dispatch(comment.Events).ConfigureAwait(false);

        return true;
    }
}
