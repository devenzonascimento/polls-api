using PollsApp.Domain.Entities;
using PollsApp.Domain.Primitives;

namespace PollsApp.Domain.Events;

public record CommentDeletedDomainEvent(PollComment comment) : IDomainEvent;
