using MediatR;

namespace PollsApp.Application.Commands;

public record DeleteCommentCommand(Guid UserId, Guid CommentId) : IRequest<bool>;
