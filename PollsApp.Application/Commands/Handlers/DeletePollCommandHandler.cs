using MediatR;
using PollsApp.Domain.Exceptions;
using PollsApp.Infrastructure.Data.Repositories.Interfaces;
using PollsApp.Infrastructure.Events.Interfaces;

namespace PollsApp.Application.Commands.Handlers;

public class DeletePollCommandHandler : IRequestHandler<DeletePollCommand, Guid>
{
    private readonly IPollRepository pollRepository;
    private readonly IDomainEventDispatcher domainEventDispatcher;

    public DeletePollCommandHandler(IPollRepository pollRepository, IDomainEventDispatcher domainEventDispatcher)
    {
        this.pollRepository = pollRepository;
        this.domainEventDispatcher = domainEventDispatcher;
    }

    public async Task<Guid> Handle(DeletePollCommand request, CancellationToken cancellationToken)
    {
        var poll = await pollRepository.GetByIdAsync(request.PollId).ConfigureAwait(false);

        if (poll == null || poll.IsDeleted)
            throw new NotFoundException("Poll", request.PollId);

        if (!poll.IsOpen)
            throw new InvalidStateException("This poll is closed.");

        if (poll.CreatedBy != request.UserId)
            throw new UnauthorizedAccessException("You are not authorized to delete this poll.");

        poll.MarkAsDeleted();

        await pollRepository.SaveAsync(poll).ConfigureAwait(false);

        await domainEventDispatcher.Dispatch(poll.Events).ConfigureAwait(false);

        return poll.Id;
    }
}
