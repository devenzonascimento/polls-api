using MediatR;

namespace PollsApp.Application.Commands;

public record EditCommentCommand(Guid UserId, Guid CommentId, string NewComment) : IRequest<bool>;
