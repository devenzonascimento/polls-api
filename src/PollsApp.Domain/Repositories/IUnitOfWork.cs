using System.Data;

namespace PollsApp.Domain.Repositories;

public interface IUnitOfWork
{
    IDbConnection GetConnection();
    IDbTransaction StartTransaction();
    IUnitOfWork WithTransaction(IDbTransaction transaction = null);

    public IUserRepository UserRepository { get; }
    public IPollRepository PollRepository { get; }
    public IVoteRepository VoteRepository { get; }
    public IPollCommentRepository PollCommentRepository { get; }
}
