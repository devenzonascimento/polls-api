using Hangfire;
using Microsoft.Extensions.Logging;
using PollsApp.Domain.Repositories;
using PollsApp.Infrastructure.Events.Interfaces;

namespace PollsApp.Application.Jobs;

public class PollsJobs(IUnitOfWork unitOfWork, IDomainEventDispatcher domainEventDispatcher, ILogger<PollsJobs> logger)
{
    private readonly IUnitOfWork unitOfWork = unitOfWork;
    private readonly IDomainEventDispatcher domainEventDispatcher = domainEventDispatcher;
    private readonly ILogger<PollsJobs> logger = logger;

    [AutomaticRetry(Attempts = 2)]
    public async Task CloseExpiredPollsAsync()
    {
        var polls = await unitOfWork.PollRepository.GetExpiredPollsAsync().ConfigureAwait(false);

        foreach (var poll in polls)
        {
            logger.LogInformation("Closing poll {PollId} scheduled at {Time}", poll.Id, DateTime.UtcNow);

            if (poll.IsDeleted || !poll.IsOpen)
            {
                logger.LogInformation("Poll {PollId} already closed or not found", poll?.Id);
                return;
            }

            poll.Close();

            await unitOfWork.PollRepository.UpdateAsync(poll).ConfigureAwait(false);

            logger.LogInformation("Poll {PollId} closed successfully", poll.Id);

            await domainEventDispatcher.Dispatch(poll.Events).ConfigureAwait(false);
        }
    }
}
