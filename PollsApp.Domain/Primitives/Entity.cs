namespace PollsApp.Domain.Primitives;

public abstract class Entity
{
    private readonly List<IDomainEvent> domainEvents = [];

    public ICollection<IDomainEvent> Events => domainEvents;

    public void ClearEvents() => domainEvents.Clear();

    protected void Raise(IDomainEvent domainEvent)
    {
        domainEvents.Add(domainEvent);
    }
}
