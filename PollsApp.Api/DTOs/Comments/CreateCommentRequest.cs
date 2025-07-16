namespace PollsApp.Api.DTOs.Comments;

public record CreateCommentRequest(Guid PollId, string Comment);
