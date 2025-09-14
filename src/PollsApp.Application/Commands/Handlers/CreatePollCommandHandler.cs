using MediatR;
using PollsApp.Domain.Entities;
using PollsApp.Domain.Repositories;
using PollsApp.Infrastructure.Events.Interfaces;

namespace PollsApp.Application.Commands.Handlers;

public class CreatePollCommandHandler(
    IUnitOfWork unitOfWork,
    IDomainEventDispatcher domainEventDispatcher
) : IRequestHandler<CreatePollCommand, Guid>
{
    private readonly IUnitOfWork unitOfWork = unitOfWork;
    private readonly IDomainEventDispatcher domainEventDispatcher = domainEventDispatcher;

    public async Task<Guid> Handle(CreatePollCommand request, CancellationToken cancellationToken)
    {
        var poll = Poll.Create(request.Title, request.Description, request.AllowMultiple, request.UserRequesterId, request.ClosesAt);

        using var transaction = unitOfWork.PollRepository.StartTransaction();

        try
        {
            var pollRepositoryWithTransaction = unitOfWork.PollRepository.WithTransaction(transaction);

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
