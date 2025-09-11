namespace PollsApp.Domain.Entities;

public class Vote
{
    public Guid Id { get; private set; }
    public Guid PollId { get; private set; }
    public Guid PollOptionId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime VotedAt { get; private set; }

    public Vote(Guid id, Guid pollId, Guid pollOptionId, Guid userId, DateTime votedAt)
    {
        Id = id;
        PollId = pollId;
        PollOptionId = pollOptionId;
        UserId = userId;
        VotedAt = votedAt;
    }

    public Vote(Guid pollId, Guid pollOptionId, Guid userId)
    {
        Id = Guid.Empty;
        PollId = pollId;
        PollOptionId = pollOptionId;
        UserId = userId;
        VotedAt = DateTime.UtcNow;
    }

    public void ChangeOption(Guid newPollOptionId)
    {
        PollOptionId = newPollOptionId;
    }
}
