using MediatR;
using PollsApp.Infrastructure.Data.Repositories.Interfaces;

namespace PollsApp.Application.Commands.Handlers;

public class DeletePollCommandHandler : IRequestHandler<DeletePollCommand, Guid>
{
    private readonly IPollRepository pollRepository;

    public DeletePollCommandHandler(IPollRepository pollRepository)
    {
        this.pollRepository = pollRepository;
    }

    public async Task<Guid> Handle(DeletePollCommand request, CancellationToken cancellationToken)
    {
        var poll = await pollRepository.GetByIdAsync(request.PollId).ConfigureAwait(false);

        if (poll == null || poll.IsDeleted)
            throw new ArgumentException($"Poll with ID {request.PollId} not found.");

        if (poll.CreatedBy != request.UserId)
            throw new UnauthorizedAccessException("You are not authorized to delete this poll.");

        poll.MarkAsDeleted();

        await pollRepository.SaveAsync(poll).ConfigureAwait(false);

        return poll.Id;
    }
}
