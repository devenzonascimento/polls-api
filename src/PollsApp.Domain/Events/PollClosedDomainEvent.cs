using PollsApp.Domain.Entities;
using PollsApp.Domain.Primitives;

namespace PollsApp.Domain.Events;

public record PollClosedDomainEvent(Poll Poll) : IDomainEvent;
