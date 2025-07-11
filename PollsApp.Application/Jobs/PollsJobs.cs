using Hangfire;
using Microsoft.Extensions.Logging;
using PollsApp.Infrastructure.Data.Repositories.Interfaces;

namespace PollsApp.Application.Jobs;

public class PollsJobs
{
    private readonly IPollRepository pollRepository;
    private readonly ILogger<PollsJobs> logger;

    public PollsJobs(IPollRepository pollRepository, ILogger<PollsJobs> logger)
    {
        this.pollRepository = pollRepository;
        this.logger = logger;
    }

    [AutomaticRetry(Attempts = 2)]
    public async Task ClosePollAsync(Guid pollId)
    {
        logger.LogInformation("Closing poll {PollId} scheduled at {Time}", pollId, DateTime.UtcNow);

        var poll = await pollRepository.GetByIdAsync(pollId).ConfigureAwait(false);

        if (poll == null || poll.IsDeleted || !poll.IsOpen)
        {
            logger.LogInformation("Poll {PollId} already closed or not found", pollId);
            return;
        }

        poll.Close();

        await pollRepository.SaveAsync(poll).ConfigureAwait(false);

        logger.LogInformation("Poll {PollId} closed successfully", pollId);
    }
}