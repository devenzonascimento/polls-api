namespace PollsApp.Api.DTOs.Poll;

public record CreatePollRequest(string Title, string Description, DateTime ClosesAt, List<string> Options);
