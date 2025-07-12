using Hangfire;
using MediatR;
using PollsApp.Application.Jobs;
using PollsApp.Domain.Events;

namespace PollsApp.Application.Notifications.Handlers;

public class PollsNotificationHandler
    : INotificationHandler<PollCreatedDomainEvent>
{
    private readonly IBackgroundJobClient jobClient;

    public PollsNotificationHandler(IBackgroundJobClient jobClient) => this.jobClient = jobClient;

    public Task Handle(PollCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (notification.poll.ClosesAt.HasValue)
        {
            jobClient.Schedule<PollsJobs>(
                j => j.ClosePollAsync(notification.poll.Id),
                notification.poll.ClosesAt.Value
            );
        }

        return Task.CompletedTask;
    }
}
