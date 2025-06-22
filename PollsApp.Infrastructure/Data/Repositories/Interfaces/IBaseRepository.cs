using System.Data;

namespace PollsApp.Infrastructure.Data.Repositories.Interfaces
{
    public interface IBaseRepository<T>
    {
        IDbConnection GetConnection();
        T WithTransaction(IDbTransaction tran = null);
    }
}
