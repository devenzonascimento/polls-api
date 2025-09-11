using Hangfire;
using Microsoft.Extensions.Logging;
using PollsApp.Infrastructure.Data.Repositories.Interfaces;
using PollsApp.Infrastructure.Events.Interfaces;

namespace PollsApp.Application.Jobs;

public class PollsJobs
{
    private readonly IPollRepository pollRepository;
    private readonly IDomainEventDispatcher domainEventDispatcher;
    private readonly ILogger<PollsJobs> logger;

    public PollsJobs(IPollRepository pollRepository, IDomainEventDispatcher domainEventDispatcher, ILogger<PollsJobs> logger)
    {
        this.pollRepository = pollRepository;
        this.domainEventDispatcher = domainEventDispatcher;
        this.logger = logger;
    }

    [AutomaticRetry(Attempts = 2)]
    public async Task CloseExpiredPollsAsync()
    {
        var polls = await pollRepository.GetExpiredPollsAsync().ConfigureAwait(false);

        foreach (var poll in polls)
        {
            logger.LogInformation("Closing poll {PollId} scheduled at {Time}", poll.Id, DateTime.UtcNow);

            if (poll.IsDeleted || !poll.IsOpen)
            {
                logger.LogInformation("Poll {PollId} already closed or not found", poll?.Id);
                return;
            }

            poll.Close();

            await pollRepository.UpdateAsync(poll).ConfigureAwait(false);

            logger.LogInformation("Poll {PollId} closed successfully", poll.Id);

            await domainEventDispatcher.Dispatch(poll.Events).ConfigureAwait(false);
        }
    }
}
