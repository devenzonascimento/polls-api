using PollsApp.Domain.Events;
using PollsApp.Domain.Primitives;

namespace PollsApp.Domain.Entities;

public class Poll : Entity
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public bool AllowMultiple { get; private set; }
    public bool IsOpen { get; private set; }
    public bool IsDeleted { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ClosedAt { get; private set; }
    public DateTime? ClosesAt { get; private set; }

    public Poll(Guid id, string title, string description, bool allowMultiple, bool isOpen, bool isDeleted, Guid createdBy, DateTime createdAt, DateTime? closedAt, DateTime? closesAt)
    {
        Id = id;
        Title = title;
        Description = description;
        AllowMultiple = allowMultiple;
        IsOpen = isOpen;
        IsDeleted = isDeleted;
        CreatedBy = createdBy;
        CreatedAt = createdAt;
        ClosedAt = closedAt;
        ClosesAt = closesAt;
    }

    private Poll() { }

    public static Poll Create(string title, string description, bool allowMultiple, Guid createdBy, DateTime? closesAt)
    {
        var poll = new Poll
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = description,
            AllowMultiple = allowMultiple,
            IsOpen = true,
            IsDeleted = false,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow,
            ClosedAt = null,
            ClosesAt = closesAt,
        };

        poll.Raise(new PollCreatedDomainEvent(poll));

        return poll;
    }

    public void Update(string? title, string? description, DateTime? closesAt)
    {
        if (!string.IsNullOrWhiteSpace(title))
            Title = title;

        if (!string.IsNullOrWhiteSpace(description))
            Description = description;

        if (closesAt != null && closesAt.HasValue)
            SetClosesAt(closesAt.Value);

        Raise(new PollUpdatedDomainEvent(this));
    }

    public void Close()
    {
        IsOpen = false;
        ClosedAt = DateTime.UtcNow;

        Raise(new PollClosedDomainEvent(this));
    }

    public void MarkAsDeleted()
    {
        IsOpen = false;
        IsDeleted = true;
        ClosedAt = DateTime.UtcNow;

        Raise(new PollDeletedDomainEvent(Id));
    }

    public void EnsureExistsAndOpen()
    {
        if (IsDeleted)
            throw new ArgumentException($"Poll with ID {Id} not found.");

        if (!IsOpen)
            throw new ArgumentException("This poll is closed.");
    }

    private void SetClosesAt(DateTime closesAt)
    {
        if (closesAt <= DateTime.UtcNow)
            throw new ArgumentException("ClosesAt must be in the future.", nameof(closesAt));

        var oldClosesAt = ClosesAt;

        ClosesAt = closesAt;
    }
}
