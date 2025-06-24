using MediatR;
using PollsApp.Domain.Entities;
using PollsApp.Infrastructure.Data.Infrastructure;
using PollsApp.Infrastructure.Data.Repositories.Interfaces;

namespace PollsApp.Application.Commands.Handlers;

public class CreatePollCommandHandler : IRequestHandler<CreatePollCommand, Guid>
{
    private readonly IPollRepository pollRepository;

    public CreatePollCommandHandler(IPollRepository pollRepository)
    {
        this.pollRepository = pollRepository;
    }

    public async Task<Guid> Handle(CreatePollCommand request, CancellationToken cancellationToken)
    {
        var poll = new Poll(request.Title, request.Description, request.UserRequesterId, request.ClosesAt);

        using (var transaction = PostgresConnectionSingleton.GetWriteConnection().BeginTransaction())
        {
            try
            {
                var pollRepositoryWithTransaction = pollRepository.WithTransaction(transaction);
                poll = await pollRepositoryWithTransaction.SaveAsync(poll).ConfigureAwait(false);

                var options = request.Options
                    .Select(optionText => new PollOption(poll.Id, optionText))
                    .ToList();

                await pollRepositoryWithTransaction.SaveAsync(options).ConfigureAwait(false);

                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw new InvalidOperationException("Failed to create poll and options", e);
            }
        }

        return poll.Id;
    }
}