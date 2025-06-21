namespace PollsApp.Domain.Entities;

public class PollOption
{
    public Guid Id { get; private set; }
    public Guid PollId { get; private set; }
    public string Text { get; private set; }

    public PollOption(Guid pollId, string text)
    {
        Id = Guid.NewGuid();
        PollId = pollId;
        Text = text;
    }

    public void Update(string text)
    {
        Text = text;
    }
}
