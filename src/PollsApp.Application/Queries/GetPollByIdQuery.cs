using MediatR;
using PollsApp.Domain.Aggregates;

namespace PollsApp.Application.Queries;

public record GetPollByIdQuery(Guid PollId) : IRequest<PollSummary>;
