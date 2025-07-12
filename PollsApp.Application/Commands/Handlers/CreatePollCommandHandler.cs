using Hangfire;
using MediatR;
using PollsApp.Application.Jobs;
using PollsApp.Domain.Entities;
using PollsApp.Infrastructure.Data.Repositories.Interfaces;

namespace PollsApp.Application.Commands.Handlers;

public class CreatePollCommandHandler : IRequestHandler<CreatePollCommand, Guid>
{
    private readonly IPollRepository pollRepository;
    private readonly IBackgroundJobClient jobClient;
    private readonly IMediator mediator;

    public CreatePollCommandHandler(IPollRepository pollRepository, IBackgroundJobClient jobClient, IMediator mediator)
    {
        this.pollRepository = pollRepository;
        this.jobClient = jobClient;
        this.mediator = mediator;
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

        if (poll.ClosesAt.HasValue)
        {
            jobClient.Schedule<PollsJobs>(j => j.ClosePollAsync(poll.Id), poll.ClosesAt.Value);
        }

        foreach (var domainEvent in poll.DomainEvents)
        {
            await mediator.Publish(domainEvent, cancellationToken).ConfigureAwait(false);
        }

        poll.ClearEvents();

        return poll.Id;
    }
}
