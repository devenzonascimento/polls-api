using PollsApp.Domain.Events;
using PollsApp.Domain.Primitives;

namespace PollsApp.Domain.Entities;

public class PollComment : Entity
{
    public Guid Id { get; private set; }
    public Guid PollId { get; private set; }
    public string Text { get; private set; }
    public bool IsEdited { get; private set; }
    public Guid? ReplyTo { get; private set; }
    public Guid CreatedBy { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public PollComment(Guid id, Guid pollId, string text, bool isEdited, Guid? replyTo, Guid createdBy, bool isDeleted, DateTime createdAt)
    {
        Id = id;
        PollId = pollId;
        Text = text;
        IsEdited = isEdited;
        ReplyTo = replyTo;
        CreatedBy = createdBy;
        IsDeleted = isDeleted;
        CreatedAt = createdAt;
    }

    private PollComment() { }

    public static PollComment Create(string text, Guid pollId, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Comment cannot be null or empty.");

        var comment = new PollComment()
        {
            Id = Guid.NewGuid(),
            PollId = pollId,
            Text = text,
            IsEdited = false,
            ReplyTo = null,
            CreatedBy = userId,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
        };

        comment.Raise(new CommentCreatedDomainEvent(comment));

        return comment;
    }

    public PollComment Reply(string text, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Comment cannot be null or empty.");

        var replyComment = new PollComment()
        {
            ReplyTo = this.Id,
            PollId = this.PollId,

            Id = Guid.NewGuid(),
            Text = text,
            IsEdited = false,
            CreatedBy = userId,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
        };

        replyComment.Raise(new CommentRepliedDomainEvent(this, replyComment));

        return replyComment;
    }

    public void Edit(string newText)
    {
        Text = newText;
        IsEdited = true;

        Raise(new CommentEditedDomainEvent(this));
    }

    public void MarkAsDeleted()
    {
        IsDeleted = true;

        Raise(new CommentDeletedDomainEvent(this));
    }
}
