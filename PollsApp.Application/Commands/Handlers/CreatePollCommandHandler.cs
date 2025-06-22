using MediatR;
using PollsApp.Domain.Entities;
using PollsApp.Infrastructure.Data.Repositories.Interfaces;

namespace PollsApp.Application.Commands.Handlers
{
    public class CreatePollCommandHandler : IRequestHandler<CreatePollCommand, Guid>
    {
        private readonly IPollRepository pollRepository;
        private readonly IPollOptionRepository pollOptionRepository;

        public CreatePollCommandHandler(IPollRepository pollRepository, IPollOptionRepository pollOptionRepository)
        {
            this.pollRepository = pollRepository;
            this.pollOptionRepository = pollOptionRepository;
        }

        public async Task<Guid> Handle(CreatePollCommand request, CancellationToken cancellationToken)
        {
            var poll = new Poll(request.Title, request.Description, request.UserRequesterId, request.ClosesAt);

            using (var transaction = pollRepository.GetConnection().BeginTransaction())
            {
                try
                {
                    poll = await pollRepository.WithTransaction(transaction).SaveAsync(poll).ConfigureAwait(false);

                    var options = request.Options
                        .Select(optionText => new PollOption(poll.Id, optionText))
                        .ToList();

                    await pollOptionRepository.WithTransaction(transaction).SaveAsync(options).ConfigureAwait(false);

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

}