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

    public async Task<PollOption> SaveAsync(PollOption pollOption)
        => await SaveAsync<PollOption, PollOptionDao>(pollOption, Transaction).ConfigureAwait(false);

    public async Task<IEnumerable<PollOption>> SaveAsync(IEnumerable<PollOption> pollOptions)
        => await SaveAsync<PollOption, PollOptionDao>(pollOptions, Transaction).ConfigureAwait(false);

    public async Task<IEnumerable<Poll>> GetAllAsync()
    {
        var sql = "SELECT * FROM polls WHERE deleted = false;";

        var pollsDao = await Connection.QueryAsync<PollDao>(sql).ConfigureAwait(false);

        return pollsDao.Select(p => p.Export());
    }

    public async Task<Poll?> GetByIdAsync(Guid id)
    {
        var sql = "SELECT * FROM polls WHERE id = @id;";

        var pollDao = await Connection.QuerySingleOrDefaultAsync<PollDao>(sql, new { id }).ConfigureAwait(false);

        return pollDao?.Export();
    }

    public async Task<PollOption?> GetOptionByIdAsync(Guid id)
    {
        var sql = "SELECT * FROM poll_options WHERE id = @id;";

        var pollOptionsDao = await Connection.QuerySingleOrDefaultAsync<PollOptionDao>(sql, new { id }).ConfigureAwait(false);

        return pollOptionsDao?.Export();
    }

    public async Task<IEnumerable<PollOption>> GetOptionsByPollIdAsync(Guid pollId)
    {
        var sql = "SELECT * FROM poll_options WHERE poll_id = @pollId;";

        var pollOptionsDao = await Connection.QueryAsync<PollOptionDao>(sql, new { pollId }).ConfigureAwait(false);

        return pollOptionsDao.Select(p => p.Export());
    }

    public async Task<IEnumerable<PollOption>> GetOptionsByPollsIdsAsync(IList<Guid> pollsIds)
    {
        if (pollsIds == null || !pollsIds.Any())
            return [];

        var sql = "SELECT * FROM poll_options WHERE poll_id = ANY(@pollsIds);";

        var pollOptionsDao = await Connection.QueryAsync<PollOptionDao>(sql, new { pollsIds = pollsIds.ToArray() }).ConfigureAwait(false);

        return pollOptionsDao.Select(p => p.Export());
    }

    public async Task DeleteOptionByIdAsync(Guid id)
    {
        var sql = "DELETE FROM poll_options WHERE id = @id;";
        await Connection.ExecuteAsync(sql, new { id }, Transaction).ConfigureAwait(false);
    }

    public async Task DeleteOptionsByIdsAsync(IEnumerable<Guid> ids)
    {
        var sql = "DELETE FROM poll_options WHERE id = ANY(@ids);";
        await Connection.ExecuteAsync(sql, new { ids = ids.ToArray() }, Transaction).ConfigureAwait(false);
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

    public PollDao() { }

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

    public PollOptionDao() { }

    public PollOptionDao(PollOption domain)
    {
        id = domain.Id;
        poll_id = domain.PollId;
        text = domain.Text;
    }

    public PollOption Export() => new PollOption(id, poll_id, text);
}