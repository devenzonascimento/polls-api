using System.Data;

namespace PollsApp.Domain.Repositories;

public interface IBaseRepository<T>
{
    IDbConnection GetConnection();
    IDbTransaction StartTransaction();
    T WithTransaction(IDbTransaction tran = null);
}
