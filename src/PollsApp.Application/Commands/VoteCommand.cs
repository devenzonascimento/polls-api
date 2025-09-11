using MediatR;

namespace PollsApp.Application.Commands;

public record VoteCommand(Guid UserId, Guid OptionId) : IRequest<bool>;