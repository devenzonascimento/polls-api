namespace PollsApp.Domain.Entities;

public class Poll
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string Status { get; private set; }

    public Poll(Guid id, string title, string description, DateTime createdAt, string status)
    {
        Id = id;
        Title = title;
        Description = description;
        CreatedAt = createdAt;
        Status = status;
    }

    public Poll(string title, string description)
    {
        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        CreatedAt = DateTime.UtcNow;
        Status = "Active";
    }

    public void Update(string title, string description)
    {
        Title = title;
        Description = description;
    }
}
