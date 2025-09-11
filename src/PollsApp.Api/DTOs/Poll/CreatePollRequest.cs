namespace PollsApp.Api.DTOs.Poll;

public record CreatePollRequest(
    string Title,
    string Description,
    bool AllowMultiple,
    DateTime ClosesAt,
    List<string> Options
);
