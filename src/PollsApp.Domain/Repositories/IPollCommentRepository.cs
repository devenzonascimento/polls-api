using PollsApp.Domain.Aggregates;
using PollsApp.Domain.Entities;

namespace PollsApp.Domain.Repositories;

public interface IPollCommentRepository : IBaseRepository<IPollCommentRepository>
{
    Task InsertAsync(PollComment pollComment);
    Task UpdateAsync(PollComment pollComment);
    Task<PollComment?> GetByIdAsync(Guid commentId);
    Task<IEnumerable<PollComment>> GetAllCommentReplies(Guid commentId);
    Task<IEnumerable<CommentSummary>> GetCommentsByPollAsync(Guid pollId);
}
