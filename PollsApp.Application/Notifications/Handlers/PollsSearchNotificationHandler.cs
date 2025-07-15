using Hangfire;
using MediatR;
using PollsApp.Application.Services.Interfaces;
using PollsApp.Domain.Events;
using PollsApp.Infrastructure.Data.Repositories.Interfaces;
using PollsApp.Infrastructure.Data.Search.Documents;

namespace PollsApp.Application.Notifications.Handlers;

public class PollsSearchNotificationHandler :
    INotificationHandler<PollCreatedDomainEvent>,
    INotificationHandler<PollUpdatedDomainEvent>,
    INotificationHandler<PollClosedDomainEvent>,
    INotificationHandler<PollDeletedDomainEvent>
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
        var options = await pollRepository.GetOptionsByPollIdAsync(notification.Poll.Id).ConfigureAwait(false);

        var pollDocument = new PollDocument(notification.Poll, options);

        jobClient.Enqueue<IPollSearchService>(j => j.IndexAsync(pollDocument));
    }

    public async Task Handle(PollUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var options = await pollRepository.GetOptionsByPollIdAsync(notification.Poll.Id).ConfigureAwait(false);

        var pollDocument = new PollDocument(notification.Poll, options);

        jobClient.Enqueue<IPollSearchService>(j => j.UpdateAsync(notification.Poll.Id, pollDocument));
    }

    public async Task Handle(PollClosedDomainEvent notification, CancellationToken cancellationToken)
    {
        var options = await pollRepository.GetOptionsByPollIdAsync(notification.Poll.Id).ConfigureAwait(false);

        var pollDocument = new PollDocument(notification.Poll, options);

        jobClient.Enqueue<IPollSearchService>(j => j.UpdateAsync(notification.Poll.Id, pollDocument));
    }

    public Task Handle(PollDeletedDomainEvent notification, CancellationToken cancellationToken)
    {
        jobClient.Enqueue<IPollSearchService>(j => j.DeleteAsync(notification.PollId));

        return Task.CompletedTask;
    }
}
