using Hangfire;
using MediatR;
using PollsApp.Application.Services.Interfaces;
using PollsApp.Domain.Events;
using PollsApp.Infrastructure.Data.Repositories.Interfaces;
using PollsApp.Infrastructure.Data.Search.Documents;

namespace PollsApp.Application.Notifications.Handlers;

public class PollsSearchNotificationHandler
    : INotificationHandler<PollCreatedDomainEvent>
{
    private readonly IPollRepository pollRepository;
    private readonly IBackgroundJobClient jobClient;

    public PollsSearchNotificationHandler(IPollRepository pollRepository, IBackgroundJobClient jobClient)
    {
        this.pollRepository = pollRepository;
        this.jobClient = jobClient;
    }

    public async Task Handle(PollCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var options = await pollRepository.GetOptionsByPollIdAsync(notification.poll.Id).ConfigureAwait(false);

        var pollDocument = new PollDocument(notification.poll, options);

        jobClient.Enqueue<IPollSearchService>(j => j.IndexPollAsync(pollDocument));
    }
}
