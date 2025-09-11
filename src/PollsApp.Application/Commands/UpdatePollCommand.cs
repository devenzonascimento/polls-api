using MediatR;

namespace PollsApp.Application.Commands;

public record UpdatePollCommand(
    Guid UserRequesterId,
    Guid PollId,
    string? Title,
    string? Description,
    DateTime? ClosesAt,
    IEnumerable<OptionToUpdateData> Options
) : IRequest<Guid>;

public record OptionToUpdateData(string? OldText, string NewText);