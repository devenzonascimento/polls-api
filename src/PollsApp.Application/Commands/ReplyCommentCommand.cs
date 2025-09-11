using MediatR;

namespace PollsApp.Application.Commands;

public record ReplyCommentCommand(Guid UserId, Guid CommentIdToReply, string Comment) : IRequest<Guid>;
