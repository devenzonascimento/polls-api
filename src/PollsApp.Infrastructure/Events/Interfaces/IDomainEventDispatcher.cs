using PollsApp.Domain.Primitives;

namespace PollsApp.Infrastructure.Events.Interfaces;

public interface IDomainEventDispatcher
{
    Task Dispatch(IEnumerable<IDomainEvent> events);
}
