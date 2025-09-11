namespace PollsApp.Domain.Aggregates;

public class CommentSummary
{
    public Guid CommentId { get; set; }
    public string Text { get; set; }
    public bool IsEdited { get; set; }
    public Guid? ReplyToCommentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorUsername { get; set; }

    public CommentSummary() { }

    public CommentSummary(Guid commentId, string text, bool isEdited, Guid? replyToCommentId, DateTime createdAt, Guid authorId, string authorUsername)
    {
        CommentId = commentId;
        Text = text;
        IsEdited = isEdited;
        ReplyToCommentId = replyToCommentId;
        CreatedAt = createdAt;
        AuthorId = authorId;
        AuthorUsername = authorUsername;
    }
}
