using MediatR;

namespace PollsApp.Application.Commands;

public record CreateCommentCommand(Guid UserId, Guid PollId, string Text) : IRequest<Guid>;
