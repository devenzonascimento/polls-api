using Hangfire;
using MediatR;
using PollsApp.Application.Jobs;
using PollsApp.Application.Services.Interfaces;
using PollsApp.Domain.Entities;
using PollsApp.Infrastructure.Data.Repositories.Interfaces;
using PollsApp.Infrastructure.Data.Search.Documents;

namespace PollsApp.Application.Commands.Handlers;

public class CreatePollCommandHandler : IRequestHandler<CreatePollCommand, Guid>
{
    private readonly IPollRepository pollRepository;
    private readonly IPollSearchService pollSearchService;
    private readonly IBackgroundJobClient jobClient;

    public CreatePollCommandHandler(IPollRepository pollRepository, IPollSearchService pollSearchService, IBackgroundJobClient jobClient)
    {
        this.pollRepository = pollRepository;
        this.pollSearchService = pollSearchService;
        this.jobClient = jobClient;
    }

    public async Task<Guid> Handle(CreatePollCommand request, CancellationToken cancellationToken)
    {
        var poll = new Poll(request.Title, request.Description, request.AllowMultiple, request.UserRequesterId, request.ClosesAt);

        PollDocument pollDocument;
        using (var transaction = pollRepository.StartTransaction())
        {
            try
            {
                var pollRepositoryWithTransaction = pollRepository.WithTransaction(transaction);
                poll = await pollRepositoryWithTransaction.SaveAsync(poll).ConfigureAwait(false);

                var options = request.Options
                    .Select((optionText, index) => new PollOption(poll.Id, optionText, index))
                    .ToList();

                await pollRepositoryWithTransaction.SaveAsync(options).ConfigureAwait(false);

                pollDocument = new PollDocument(poll, options);

                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw new InvalidOperationException("Failed to create poll and options", e);
            }
        }

        if (poll.ClosesAt != null)
        {
            jobClient.Schedule<PollsJobs>(
                job => job.ClosePollAsync(poll.Id),
                poll.ClosesAt.Value
            );
        }

        await pollSearchService.IndexPollAsync(pollDocument).ConfigureAwait(false);

        return poll.Id;
    }
}