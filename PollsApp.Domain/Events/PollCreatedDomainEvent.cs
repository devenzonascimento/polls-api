using PollsApp.Domain.Primitives;

namespace PollsApp.Domain.Events;

public record PollCreatedDomainEvent(Guid Id) : DomainEvent(Id);
