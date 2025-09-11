using MediatR;
using PollsApp.Domain.Entities;
using PollsApp.Domain.Exceptions;
using PollsApp.Infrastructure.Data.Repositories.Interfaces;
using PollsApp.Infrastructure.Events.Interfaces;

namespace PollsApp.Application.Commands.Handlers;

public class UpdatePollCommandHandler : IRequestHandler<UpdatePollCommand, Guid>
{
    private readonly IPollRepository pollRepository;
    private readonly IDomainEventDispatcher domainEventDispatcher;

    public UpdatePollCommandHandler(IPollRepository pollRepository, IDomainEventDispatcher domainEventDispatcher)
    {
        this.pollRepository = pollRepository;
        this.domainEventDispatcher = domainEventDispatcher;
    }

    public async Task<Guid> Handle(UpdatePollCommand request, CancellationToken cancellationToken)
    {
        var poll = await pollRepository.GetByIdAsync(request.PollId).ConfigureAwait(false);
        var pollOptions = await pollRepository.GetOptionsByPollIdAsync(request.PollId).ConfigureAwait(false);

        if (poll == null || poll.IsDeleted)
            throw new NotFoundException("Poll", request.PollId);

        if (!poll.IsOpen)
            throw new InvalidStateException("This poll is closed.");

        if (poll.CreatedBy != request.UserRequesterId)
            throw new UnauthorizedAccessException("You are not authorized to update this poll.");

        var newOptions = new List<PollOption>();
        var optionsIdsToDelete = new List<Guid>();
        var orderIndex = 0;

        if (request.Options != null && request.Options.Any())
        {
            foreach (var option in request.Options)
            {
                if (string.IsNullOrWhiteSpace(option.NewText))
                    throw new ArgumentException("Option text cannot be empty.");

                if (string.IsNullOrWhiteSpace(option.OldText))
                {
                    var newOption = new PollOption(poll.Id, option.NewText, orderIndex);

                    newOptions.Add(newOption);
                }

                var foundOption = pollOptions.FirstOrDefault(po => po.Text == option.OldText);

                if (foundOption != null)
                {
                    foundOption.Update(option.NewText, orderIndex);

                    newOptions.Add(foundOption);
                }

                orderIndex++;
            }

            optionsIdsToDelete.AddRange(
                pollOptions
                    .Where(po => !request.Options.Any(opt => opt.OldText == po.Text))
                    .Select(po => po.Id)
            );
        }

        poll.Update(request.Title, request.Description, request.ClosesAt);

        using var transaction = pollRepository.StartTransaction();

        try
        {
            var pollRepositoryWithTransaction = pollRepository.WithTransaction(transaction);

            await pollRepositoryWithTransaction.SaveAsync(poll).ConfigureAwait(false);

            await pollRepositoryWithTransaction.DeleteOptionsByIdsAsync(optionsIdsToDelete).ConfigureAwait(false);
            await pollRepositoryWithTransaction.SaveAsync(newOptions).ConfigureAwait(false);

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }

        await domainEventDispatcher.Dispatch(poll.Events).ConfigureAwait(false);

        return poll.Id;
    }
}
