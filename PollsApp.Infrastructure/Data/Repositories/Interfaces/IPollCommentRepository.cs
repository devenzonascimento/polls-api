using PollsApp.Domain.Entities;

namespace PollsApp.Infrastructure.Data.Repositories.Interfaces;

public interface IPollCommentRepository : IBaseRepository<IPollCommentRepository>
{
    Task InsertAsync(PollComment pollComment);
    Task UpdateAsync(PollComment pollComment);
    Task<PollComment?> GetByIdAsync(Guid commentId);
    Task<IEnumerable<PollComment>> GetAllCommentReplies(Guid commentId);
    Task<IEnumerable<PollComment>> GetCommentsByPollAsync(Guid pollId);
}
