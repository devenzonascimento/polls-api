using MediatR;
using PollsApp.Domain.Primitives;
using PollsApp.Infrastructure.Events.Interfaces;

namespace PollsApp.Infrastructure.Events;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IMediator mediator;
    public DomainEventDispatcher(IMediator mediator) => this.mediator = mediator;

    public async Task Dispatch(IEnumerable<IDomainEvent> events)
    {
        foreach (var evt in events)
        {
            await mediator.Publish(evt).ConfigureAwait(false);
        }
    }
}
