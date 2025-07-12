using MediatR;

namespace PollsApp.Domain.Primitives;

public record DomainEvent(Guid Id) : INotification;
