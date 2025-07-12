using MediatR;
using PollsApp.Domain.Entities;
using PollsApp.Infrastructure.Data.Repositories.Interfaces;
using PollsApp.Infrastructure.Events.Interfaces;

namespace PollsApp.Application.Commands.Handlers;

public class CreatePollCommandHandler : IRequestHandler<CreatePollCommand, Guid>
{
    private readonly IPollRepository pollRepository;
    private readonly IDomainEventDispatcher domainEventDispatcher;

    public CreatePollCommandHandler(IPollRepository pollRepository, IDomainEventDispatcher domainEventDispatcher)
    {
        this.pollRepository = pollRepository;
        this.domainEventDispatcher = domainEventDispatcher;
    }

    public async Task<Guid> Handle(CreatePollCommand request, CancellationToken cancellationToken)
    {
        var poll = Poll.Create(request.Title, request.Description, request.AllowMultiple, request.UserRequesterId, request.ClosesAt);

        using var transaction = pollRepository.StartTransaction();

        try
        {
            var pollRepositoryWithTransaction = pollRepository.WithTransaction(transaction);

            await pollRepositoryWithTransaction.InsertAsync(poll).ConfigureAwait(false);

            var options = request.Options
                .Select((optionText, index) => new PollOption(poll.Id, optionText, index))
                .ToList();

            await pollRepositoryWithTransaction.SaveAsync(options).ConfigureAwait(false);

            transaction.Commit();
        }
        catch (Exception e)
        {
            transaction.Rollback();
            throw new InvalidOperationException("Failed to create poll and options", e);
        }

        await domainEventDispatcher.Dispatch(poll.Events).ConfigureAwait(false);

        return poll.Id;
    }
}
