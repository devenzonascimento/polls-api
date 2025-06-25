using System.Data;
using Dapper;
using Dapper.Contrib.Extensions;
using PollsApp.Domain.Aggregates;
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

    public async Task<PollSummary> GetPollSummaryAsync(Guid pollId)
    {
        var sql = @"
            SELECT
                p.id AS Id,
                p.title AS Title,
                p.description AS Description,
                p.is_open AS IsOpen,
                p.created_by AS CreatedBy,
                p.created_at AS CreatedAt,
                p.closed_at AS ClosedAt,
                p.closes_at AS ClosesAt
            FROM
                polls p
            WHERE
                p.id = @pollId  
                AND p.is_deleted = false
        ";

        var pollSummary = await Connection.QuerySingleOrDefaultAsync<PollSummary>(sql, param: new { pollId }).ConfigureAwait(false);

        if (pollSummary == null)
            return null;

        var pollsSummariesWithOptions = await GetPollsSummariesWithOptionsAsync([pollSummary]).ConfigureAwait(false);

        return pollsSummariesWithOptions.FirstOrDefault();
    }

    public async Task<IEnumerable<PollSummary>> GetPollsSummariesAsync()
    {
        var sql = @"
            SELECT
                p.id AS Id,
                p.title AS Title,
                p.description AS Description,
                p.is_open AS IsOpen,
                p.created_by AS CreatedBy,
                p.created_at AS CreatedAt,
                p.closed_at AS ClosedAt,
                p.closes_at AS ClosesAt
            FROM
                polls p
            WHERE
                p.is_deleted = false
        ";

        var pollsSummaries = await Connection.QueryAsync<PollSummary>(sql).ConfigureAwait(false);

        var pollsSummariesWithOptions = await GetPollsSummariesWithOptionsAsync(pollsSummaries).ConfigureAwait(false);

        return pollsSummariesWithOptions;
    }

    private async Task<IEnumerable<PollSummary>> GetPollsSummariesWithOptionsAsync(IEnumerable<PollSummary> pollsSummaries)
    {
        var sql = $@"
            SELECT
                po.poll_id AS PollId,
                po.id AS Id,
                po.text AS Text,
                count(v.id) AS VotesCount
            FROM 
                poll_options po
            INNER JOIN votes v ON
                po.id = v.poll_option_id
                AND po.poll_id = ANY(@pollsIds)
            GROUP BY po.id
        ";

        var pollsDictionary = new Dictionary<Guid, List<PollOptionSummary>>();

        var pollOptionsSummaries = await Connection.QueryAsync<Guid, PollOptionSummary, Guid>(
            sql,
            (pollId, pollOptionSummary) =>
            {
                if (!pollsDictionary.TryGetValue(pollId, out var groupEntry))
                {
                    groupEntry = new List<PollOptionSummary>();
                    pollsDictionary.Add(pollId, groupEntry);
                }

                groupEntry.Add(pollOptionSummary);

                return pollId;
            },
            param: new { pollsIds = pollsSummaries.Select(p => p.Id).ToArray() },
            splitOn: "Id"
        ).ConfigureAwait(false);

        foreach (var pollSummary in pollsSummaries)
        {
            pollSummary.Options = pollsDictionary.GetValueOrDefault(pollSummary.Id) ?? [];
        }

        return pollsSummaries;
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
    public bool is_open { get; set; }
    public bool is_deleted { get; set; }
    public Guid created_by { get; set; }
    public DateTime created_at { get; set; }
    public DateTime? closed_at { get; set; }
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
        is_open = domain.IsOpen;
        is_deleted = domain.IsDeleted;
        created_by = domain.CreatedBy;
        created_at = domain.CreatedAt;
        closes_at = domain.ClosesAt;
    }

    public Poll Export()
        => new Poll(id, title, description, is_open, is_deleted, created_by, created_at, closed_at, closes_at);
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