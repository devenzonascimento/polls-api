using MediatR;
using PollsApp.Domain.Exceptions;
using PollsApp.Domain.Repositories;
using PollsApp.Infrastructure.Events.Interfaces;

namespace PollsApp.Application.Commands.Handlers;

public class DeletePollCommandHandler(
    IUnitOfWork unitOfWork,
    IDomainEventDispatcher domainEventDispatcher
) : IRequestHandler<DeletePollCommand, Guid>
{
    private readonly IUnitOfWork unitOfWork = unitOfWork;
    private readonly IDomainEventDispatcher domainEventDispatcher = domainEventDispatcher;

    public async Task<Guid> Handle(DeletePollCommand request, CancellationToken cancellationToken)
    {
        var poll = await unitOfWork.PollRepository.GetByIdAsync(request.PollId).ConfigureAwait(false);

        if (poll == null || poll.IsDeleted)
            throw new NotFoundException("Poll", request.PollId);

        if (!poll.IsOpen)
            throw new InvalidStateException("This poll is closed.");

        if (poll.CreatedBy != request.UserId)
            throw new UnauthorizedAccessException("You are not authorized to delete this poll.");

        poll.MarkAsDeleted();

        await unitOfWork.PollRepository.SaveAsync(poll).ConfigureAwait(false);

        await domainEventDispatcher.Dispatch(poll.Events).ConfigureAwait(false);

        return poll.Id;
    }
}
