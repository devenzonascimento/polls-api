namespace PollsApp.Domain.Aggregates;

public class PollSummary
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsOpen { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public DateTime? ClosesAt { get; set; }
    public IEnumerable<PollOptionSummary> Options { get; set; }

    public PollSummary() { }

    public PollSummary(Guid id, string title, string description, bool active, Guid createdBy, DateTime createdAt, DateTime closedAt, DateTime? closesAt, IEnumerable<PollOptionSummary> options)
    {
        Id = id;
        Title = title;
        Description = description;
        IsOpen = active;
        CreatedBy = createdBy;
        CreatedAt = createdAt;
        ClosedAt = closedAt;
        ClosesAt = closesAt;
        Options = options ?? new List<PollOptionSummary>();
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
