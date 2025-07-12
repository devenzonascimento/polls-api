using MediatR;
using PollsApp.Application.Services.Interfaces;
using PollsApp.Domain.Events;
using PollsApp.Infrastructure.Data.Repositories.Interfaces;
using PollsApp.Infrastructure.Data.Search.Documents;

namespace PollsApp.Application.Notifications.Handlers;

public class PollIndexerNotificationHandler : INotificationHandler<PollCreatedDomainEvent>
{
    private readonly IPollRepository pollRepository;
    private readonly IPollSearchService pollSearchService;

    public PollIndexerNotificationHandler(IPollRepository pollRepository, IPollSearchService pollSearchService)
    {
        this.pollRepository = pollRepository;
        this.pollSearchService = pollSearchService;
    }

    public async Task Handle(PollCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var poll = await pollRepository.GetByIdAsync(notification.Id).ConfigureAwait(false);

        var options = await pollRepository.GetOptionsByPollIdAsync(notification.Id).ConfigureAwait(false);

        PollDocument pollDocument = new PollDocument(poll, options);

        await pollSearchService.IndexPollAsync(pollDocument).ConfigureAwait(false);
    }
}
