using MediatR;
using PollsApp.Domain.Aggregates;

namespace PollsApp.Application.Queries;

public record GetRankedPollsQuery : IRequest<IEnumerable<PollSummary>>;
