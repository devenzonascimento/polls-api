namespace PollsApp.Domain.Entities;

public class PollOption
{
    public Guid Id { get; private set; }
    public Guid PollId { get; private set; }
    public string Text { get; private set; }
    public int Order { get; private set; }

    public PollOption(Guid id, Guid pollId, string text, int order)
    {
        Id = id;
        PollId = pollId;
        Text = text;
        Order = order;
    }

    public PollOption(Guid pollId, string text, int order)
    {
        Id = Guid.Empty;
        PollId = pollId;
        Text = text;
        Order = order;
    }

    public void Update(string text, int order)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Option text cannot be empty.", nameof(text));

        Text = text;
        Order = order;
    }
}
