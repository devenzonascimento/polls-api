using System.Data;
using Dapper;
using Dapper.Contrib.Extensions;

namespace PollsApp.Infrastructure.Data.Repositories;

public abstract class BaseRepository<TRepoClass, TRepoInterface>
    where TRepoClass : class
    where TRepoInterface : class
{
    protected readonly IDbConnection Connection;
    protected readonly IDbTransaction Transaction;

    protected BaseRepository(IDbConnection connection, IDbTransaction transaction = null)
    {
        Connection = connection;
        Transaction = transaction;
    }

    public IDbConnection GetConnection() => Connection;

    public IDbTransaction StartTransaction()
    {
        Connection.Open();
        return Connection.BeginTransaction();
    }

    /// <summary>
    /// Cria uma nova instância do repositório com transação.
    /// </summary>
    public virtual TRepoInterface WithTransaction(IDbTransaction tran)
    {
        return (TRepoInterface)Activator.CreateInstance(
            typeof(TRepoClass),
            Connection,
            tran
        );
    }

    /// <summary>
    /// Salva ou atualiza a entidade de domínio, via DAO.
    /// </summary>
    protected async Task<TDomain> SaveAsync<TDomain, TDao>(TDomain domain, IDbTransaction transaction = null)
        where TDao : class, IBaseDao<TDomain>
    {
        // Cria o DAO a partir da entidade de domínio
        var dao = (IBaseDao<TDomain>)Activator.CreateInstance(typeof(TDao), domain);

        if (dao is null)
        {
            return default;
        }

        // Se for novo
        if (dao.id == Guid.Empty)
        {
            dao.id = Guid.NewGuid();
            await Connection.InsertAsync(dao as TDao, transaction ?? Transaction).ConfigureAwait(false);
        }
        else
        {
            await Connection.UpdateAsync(dao as TDao, transaction ?? Transaction).ConfigureAwait(false);
        }

        return dao.Export();
    }

    /// <summary>
    /// Salva ou atualiza uma coleção de entidades de domínio.
    /// </summary>
    protected async Task<IEnumerable<TDomain>> SaveAsync<TDomain, TDao>(
        IEnumerable<TDomain> domains,
        IDbTransaction transaction = null)
        where TDao : class, IBaseDao<TDomain>
    {
        if (domains is null)
        {
            return [];
        }

        var results = new List<TDomain>();

        foreach (var d in domains)
        {
            results.Add(await SaveAsync<TDomain, TDao>(d, transaction).ConfigureAwait(false));
        }

        return results;
    }
}

public interface IBaseDao<TDomain>
{
#pragma warning disable IDE1006 // Estilos de Nomenclatura
#pragma warning disable SA1300 // Element should begin with upper-case letter
    [Key]
    public Guid id { get; set; }
#pragma warning restore CA1707 // Identificadores não devem conter sublinhados
#pragma warning restore SA1300 // Element should begin with upper-case letter

    /// <summary>
    /// Converte este DAO em sua entidade de domínio.
    /// </summary>
    TDomain Export();
}
