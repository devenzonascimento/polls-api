namespace PollsApp.Api.DTOs.Comments;

public record EditCommentRequest(Guid CommentId, string NewComment);
