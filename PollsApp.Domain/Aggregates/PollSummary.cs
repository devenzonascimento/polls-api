using PollsApp.Domain.Entities;

namespace PollsApp.Domain.Aggregates;

public class PollSummary
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool Active { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ClosesAt { get; set; }
    public IEnumerable<PollOptionSummary> Options { get; set; }

    public PollSummary() { }

    public PollSummary(Guid id, string title, string description, bool active, Guid createdBy, DateTime createdAt, DateTime? closesAt, IEnumerable<PollOptionSummary> options)
    {
        Id = id;
        Title = title;
        Description = description;
        Active = active;
        CreatedBy = createdBy;
        CreatedAt = createdAt;
        ClosesAt = closesAt;
        Options = options ?? new List<PollOptionSummary>();
    }

    public PollSummary(Poll poll, IEnumerable<PollOption> options)
    {
        Id = poll.Id;
        Title = poll.Title;
        Description = poll.Description;
        Active = poll.Active;
        CreatedBy = poll.CreatedBy;
        CreatedAt = poll.CreatedAt;
        ClosesAt = poll.ClosesAt;
        Options = options.Select(o => new PollOptionSummary
        {
            Id = o.Id,
            Text = o.Text,
            VotesCount = 0,
        });
    }
}

public class PollOptionSummary
{
    public Guid Id { get; set; }
    public string Text { get; set; }
    public int VotesCount { get; set; }

    public PollOptionSummary() { }

    public PollOptionSummary(Guid id, string text, int votesCount)
    {
        Id = id;
        Text = text;
        VotesCount = votesCount;
    }
}
