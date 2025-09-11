using MediatR;
using PollsApp.Domain.Aggregates;

namespace PollsApp.Application.Queries;

public record GetAllPollsQuery : IRequest<IEnumerable<PollSummary>>;
