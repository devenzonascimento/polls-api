namespace PollsApp.Domain.Entities;

public class Poll
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public bool IsOpen { get; private set; }
    public bool IsDeleted { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ClosedAt { get; private set; }
    public DateTime? ClosesAt { get; private set; }

    public Poll(Guid id, string title, string description, bool isOpen, bool isDeleted, Guid createdBy, DateTime createdAt, DateTime? closedAt, DateTime? closesAt)
    {
        Id = id;
        Title = title;
        Description = description;
        IsOpen = isOpen;
        IsDeleted = isDeleted;
        CreatedBy = createdBy;
        CreatedAt = createdAt;
        ClosedAt = closedAt;
        ClosesAt = closesAt;
    }

    public Poll(string title, string description, Guid createdBy, DateTime? closesAt)
    {
        Id = Guid.Empty;
        Title = title;
        Description = description;
        IsOpen = true;
        IsDeleted = false;
        CreatedBy = createdBy;
        CreatedAt = DateTime.UtcNow;
        ClosedAt = null;
        ClosesAt = closesAt;
    }

    public void Update(string title, string description, DateTime? closesAt)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty.", nameof(title));

        if (closesAt != null && closesAt <= DateTime.UtcNow)
            throw new ArgumentException("ClosesAt must be in the future.", nameof(closesAt));

        Title = title;
        Description = description;
        ClosesAt = closesAt;
    }
}
