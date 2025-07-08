using PollsApp.Domain.Entities;

namespace PollsApp.Infrastructure.Data.Search.Documents;

public class PollDocument
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public IEnumerable<string> Options { get; private set; }
    public bool IsOpen { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ClosedAt { get; private set; }

    public PollDocument(Guid id, string title, string description, IEnumerable<string> options, bool isOpen, DateTime createdAt, DateTime? closedAt)
    {
        Id = id;
        Title = title;
        Description = description;
        Options = options;
        IsOpen = isOpen;
        CreatedAt = createdAt;
        ClosedAt = closedAt;
    }

    public PollDocument(Poll poll, IEnumerable<PollOption> pollOptions)
    {
        Id = poll.Id;
        Title = poll.Title;
        Description = poll.Description;
        Options = pollOptions.Select(o => o.Text);
        IsOpen = poll.IsOpen;
        CreatedAt = poll.CreatedAt;
        ClosedAt = poll.ClosedAt;
    }
}