using System.Data;
using Dapper;
using Dapper.Contrib.Extensions;
using PollsApp.Domain.Entities;
using PollsApp.Infrastructure.Data.Repositories.Interfaces;

namespace PollsApp.Infrastructure.Data.Repositories;

public class PollOptionRepository : BaseRepository<PollOptionRepository, IPollOptionRepository>, IPollOptionRepository
{
    public PollOptionRepository(IDbConnection connection, IDbTransaction transaction = null)
        : base(connection, transaction) { }

    public async Task<PollOption> SaveAsync(PollOption pollOption)
        => await SaveAsync<PollOption, PollOptionDao>(pollOption, Transaction).ConfigureAwait(false);

    public async Task<IEnumerable<PollOption>> SaveAsync(IEnumerable<PollOption> pollOptions)
        => await SaveAsync<PollOption, PollOptionDao>(pollOptions, Transaction).ConfigureAwait(false);

    public async Task<PollOption?> GetByIdAsync(Guid id)
    {
        var sql = "SELECT * FROM poll_options WHERE id = @id;";

        var pollOptionsDao = await Connection.QuerySingleOrDefaultAsync<PollOptionDao>(sql, new { id }).ConfigureAwait(false);

        return pollOptionsDao?.Export();
    }

    public async Task<IEnumerable<PollOption>> GetByPollIdAsync(Guid pollId)
    {
        var sql = "SELECT * FROM poll_options WHERE poll_id = @pollId;";

        var pollOptionsDao = await Connection.QueryAsync<PollOptionDao>(sql, new { pollId }).ConfigureAwait(false);

        return pollOptionsDao.Select(p => p.Export());
    }

    public async Task<IEnumerable<PollOption>> GetByPollsIdsAsync(IList<Guid> pollsIds)
    {
        if (pollsIds == null || !pollsIds.Any())
            return [];

        var sql = "SELECT * FROM poll_options WHERE poll_id = ANY(@pollsIds);";

        var pollOptionsDao = await Connection.QueryAsync<PollOptionDao>(sql, new { pollsIds = pollsIds.ToArray() }).ConfigureAwait(false);

        return pollOptionsDao.Select(p => p.Export());
    }
}

[Table("poll_options")]
public class PollOptionDao : IBaseDao<PollOption>
{
    #pragma warning disable IDE1006 // Estilos de Nomenclatura
    #pragma warning disable SA1300 // Element should begin with upper-case letter
    #pragma warning disable CA1707 // Identificadores não devem conter sublinhados
    [ExplicitKey]
    public Guid id { get; set; }
    public Guid poll_id { get; set; }
    public string text { get; set; }
    #pragma warning restore CA1707 // Identificadores não devem conter sublinhados
    #pragma warning restore SA1300 // Element should begin with upper-case letter
    #pragma warning restore IDE1006 // Estilos de Nomenclatura

    // Construtor para Dapper
    public PollOptionDao() { }

    // Construtor que mapeia da entidade de domínio
    public PollOptionDao(PollOption domain)
    {
        id = domain.Id;
        poll_id = domain.PollId;
        text = domain.Text;
    }

    public PollOption Export() => new PollOption(id, poll_id, text);
}
