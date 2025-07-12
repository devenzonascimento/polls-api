using PollsApp.Domain.Entities;

namespace PollsApp.Infrastructure.Data.Search.Documents;

public class PollDocument
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public IEnumerable<string> Options { get; set; }
    public bool IsOpen { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ClosedAt { get; set; }

    public PollDocument() { }

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