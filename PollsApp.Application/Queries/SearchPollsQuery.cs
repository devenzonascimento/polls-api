using MediatR;
using PollsApp.Domain.Aggregates;

namespace PollsApp.Application.Queries;

public record SearchPollsQuery(string Search, bool? IsOpen) : IRequest<IEnumerable<PollSummary>>;
