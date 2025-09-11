namespace PollsApp.Api.DTOs.Poll;

public record UpdatePollRequest(
    Guid PollId,
    string? Title,
    string? Description,
    DateTime? ClosesAt,
    IEnumerable<OptionToUpdateData> Options
);

public record OptionToUpdateData(string? OldText, string NewText);