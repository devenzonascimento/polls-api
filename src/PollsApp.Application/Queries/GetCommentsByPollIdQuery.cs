using MediatR;
using PollsApp.Domain.Aggregates;

namespace PollsApp.Application.Queries;

public record GetCommentsByPollIdQuery(Guid PollId) : IRequest<IEnumerable<CommentSummary>>;
