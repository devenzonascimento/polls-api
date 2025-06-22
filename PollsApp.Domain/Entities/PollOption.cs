namespace PollsApp.Domain.Entities;

public class PollOption
{
    public Guid Id { get; private set; }
    public Guid PollId { get; private set; }
    public string Text { get; private set; }

    public PollOption(Guid id, Guid pollId, string text)
    {
        Id = id;
        PollId = pollId;
        Text = text;
    }

    public PollOption(Guid pollId, string text)
    {
        Id = Guid.Empty;
        PollId = pollId;
        Text = text;
    }

    public void Update(string text)
    {
        Text = text;
    }
}
