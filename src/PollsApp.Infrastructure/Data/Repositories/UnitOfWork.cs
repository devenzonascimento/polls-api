using System.Data;
using PollsApp.Domain.Repositories;

namespace PollsApp.Infrastructure.Data.Repositories;

public class UnitOfWork(IDbConnection connection, IDbTransaction transaction = null) : IUnitOfWork
{
    private readonly IDbConnection connection = connection;
    private readonly IDbTransaction transaction = transaction;

    public IDbConnection GetConnection() => connection;

    public IDbTransaction StartTransaction()
    {
        connection.Open();
        return connection.BeginTransaction();
    }

    public IUnitOfWork WithTransaction(IDbTransaction transaction) => new UnitOfWork(connection, transaction);

    public IUserRepository UserRepository => (IUserRepository)new UserRepository(connection, transaction);
    public IPollRepository PollRepository => (IPollRepository)new PollRepository(connection, transaction);
    public IVoteRepository VoteRepository => (IVoteRepository)new VoteRepository(connection, transaction);
    public IPollCommentRepository PollCommentRepository => (IPollCommentRepository)new PollCommentRepository(connection, transaction);
}
