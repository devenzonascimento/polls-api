using System.Data;
using Dapper;
using Dapper.Contrib.Extensions;
using PollsApp.Domain.Entities;
using PollsApp.Infrastructure.Data.Repositories.Interfaces;

namespace PollsApp.Infrastructure.Data.Repositories;

public class PollRepository : BaseRepository<PollRepository, IPollRepository>, IPollRepository
{
    public PollRepository(IDbConnection connection, IDbTransaction transaction = null)
        : base(connection, transaction) { }

    public async Task<Poll> SaveAsync(Poll poll)
        => await SaveAsync<Poll, PollDao>(poll, Transaction).ConfigureAwait(false);

    public async Task<IEnumerable<Poll>> SaveAsync(IEnumerable<Poll> polls)
        => await SaveAsync<Poll, PollDao>(polls, Transaction).ConfigureAwait(false);

    public async Task<Poll?> GetByIdAsync(Guid id)
    {
        var sql = "SELECT * FROM polls WHERE id = @id;";

        var pollDao = await Connection.QuerySingleOrDefaultAsync<PollDao>(sql, new { id }).ConfigureAwait(false);

        return pollDao?.Export();
    }
}

[Table("polls")]
public class PollDao : IBaseDao<Poll>
{
    #pragma warning disable IDE1006 // Estilos de Nomenclatura
    #pragma warning disable SA1300 // Element should begin with upper-case letter
    #pragma warning disable CA1707 // Identificadores não devem conter sublinhados
    [ExplicitKey]
    public Guid id { get; set; }
    public string title { get; set; }
    public string description { get; set; }
    public bool active { get; set; }
    public bool deleted { get; set; }
    public Guid created_by { get; set; }
    public DateTime created_at { get; set; }
    public DateTime? closes_at { get; set; }
    #pragma warning restore CA1707 // Identificadores não devem conter sublinhados
    #pragma warning restore SA1300 // Element should begin with upper-case letter
    #pragma warning restore IDE1006 // Estilos de Nomenclatura

    // Construtor para Dapper
    public PollDao() { }

    // Construtor que mapeia da entidade de domínio
    public PollDao(Poll domain)
    {
        id = domain.Id;
        title = domain.Title;
        description = domain.Description;
        active = domain.Active;
        deleted = domain.Deleted;
        created_by = domain.CreatedBy;
        created_at = domain.CreatedAt;
        closes_at = domain.ClosesAt;
    }

    public Poll Export()
        => new Poll(id, title, description, active, deleted, created_by, created_at, closes_at);
}
