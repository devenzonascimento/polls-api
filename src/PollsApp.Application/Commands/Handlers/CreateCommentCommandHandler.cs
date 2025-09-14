using MediatR;
using PollsApp.Domain.Entities;
using PollsApp.Domain.Exceptions;
using PollsApp.Domain.Repositories;
using PollsApp.Infrastructure.Events.Interfaces;

namespace PollsApp.Application.Commands.Handlers;

public class CreateCommentCommandHandler(
    IUnitOfWork unitOfWork,
    IDomainEventDispatcher domainEventDispatcher
) : IRequestHandler<CreateCommentCommand, Guid>
{
    private readonly IUnitOfWork unitOfWork = unitOfWork;
    private readonly IDomainEventDispatcher domainEventDispatcher = domainEventDispatcher;

    public async Task<Guid> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        var poll = await unitOfWork.PollRepository.GetByIdAsync(request.PollId).ConfigureAwait(false);

        if (poll == null || poll.IsDeleted)
            throw new NotFoundException("Poll", request.PollId);

        if (!poll.IsOpen)
            throw new InvalidStateException("This poll is closed.");

        var comment = PollComment.Create(
            request.Text,
            request.PollId,
            request.UserId
        );

        await unitOfWork.PollCommentRepository.InsertAsync(comment).ConfigureAwait(false);

        await domainEventDispatcher.Dispatch(comment.Events).ConfigureAwait(false);

        return comment.Id;
    }
}
