namespace PollsApp.Domain.Entities;

public class Poll
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public bool Active { get; private set; }
    public bool Deleted { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ClosesAt { get; private set; }

    public Poll(Guid id, string title, string description, bool active, bool deleted, Guid createdBy, DateTime createdAt, DateTime? closesAt)
    {
        Id = id;
        Title = title;
        Description = description;
        Active = active;
        Deleted = deleted;
        CreatedBy = createdBy;
        CreatedAt = createdAt;
        ClosesAt = closesAt;
    }

    public Poll(string title, string description, Guid createdBy, DateTime? closesAt)
    {
        Id = Guid.Empty;
        Title = title;
        Description = description;
        Active = true;
        Deleted = false;
        CreatedBy = createdBy;
        CreatedAt = DateTime.UtcNow;
        ClosesAt = closesAt;
    }

    public void ChangeTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty.", nameof(title));

        Title = title;
    }

    public void ChangeDescription(string description)
    {
        Description = description;
    }
}
