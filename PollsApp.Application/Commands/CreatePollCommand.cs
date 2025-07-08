using MediatR;

namespace PollsApp.Application.Commands;

public record CreatePollCommand(
    Guid UserRequesterId,
    string Title,
    string Description,
    bool AllowMultiple,
    DateTime ClosesAt,
    List<string> Options
) : IRequest<Guid>;
