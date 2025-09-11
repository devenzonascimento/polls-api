using MediatR;

namespace PollsApp.Application.Commands;

public record DeletePollCommand(Guid UserId, Guid PollId) : IRequest<Guid>;
