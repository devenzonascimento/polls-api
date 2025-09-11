namespace PollsApp.Api.DTOs.Comments;

public record ReplyCommentRequest(Guid CommentIdToReply, string Comment);
