namespace PollsApp.Domain.Primitives;

public abstract class Entity
{
    private readonly List<DomainEvent> domainEvents = [];

    public ICollection<DomainEvent> DomainEvents => domainEvents;

    public void ClearEvents() => domainEvents.Clear();

    protected void Raise(DomainEvent domainEvent)
    {
        domainEvents.Add(domainEvent);
    }
}
