using System.Data;
using Dapper;
using Dapper.Contrib.Extensions;
using PollsApp.Domain.Entities;
using PollsApp.Infrastructure.Data.Repositories.Interfaces;

namespace PollsApp.Infrastructure.Data.Repositories;
public class VoteRepository : BaseRepository<VoteRepository, IVoteRepository>, IVoteRepository
{
    public VoteRepository(IDbConnection connection, IDbTransaction transaction = null)
        : base(connection, transaction) { }
    public async Task<Vote> SaveAsync(Vote vote)
        => await SaveAsync<Vote, VoteDao>(vote, Transaction).ConfigureAwait(false);
    public async Task<IEnumerable<Vote>> SaveAsync(IEnumerable<Vote> votes)
        => await SaveAsync<Vote, VoteDao>(votes, Transaction).ConfigureAwait(false);

    public async Task DeleteByIdAsync(Guid voteId)
    {
        var sql = "DELETE FROM votes WHERE id = @voteId;";
        await Connection.ExecuteAsync(sql, new { voteId }, Transaction).ConfigureAwait(false);
    }

    public async Task<Vote?> GetByIdAsync(Guid id)
    {
        var sql = "SELECT * FROM votes WHERE id = @id;";

        var voteDao = await Connection.QuerySingleOrDefaultAsync<VoteDao>(sql, new { id }).ConfigureAwait(false);

        return voteDao?.Export();
    }

    public async Task<IEnumerable<Vote>> GetAllByPollIdAsync(Guid pollId)
    {
        var sql = "SELECT * FROM votes WHERE poll_id = @pollId;";

        var votesDao = await Connection.QueryAsync<VoteDao>(sql, new { pollId }).ConfigureAwait(false);

        return votesDao.Select(v => v.Export());
    }

    public async Task<IEnumerable<Vote>> GetAllByPollOptionIdAsync(Guid pollOptionId)
    {
        var sql = "SELECT * FROM votes WHERE poll_option_id = @pollOptionId;";

        var votesDao = await Connection.QueryAsync<VoteDao>(sql, new { pollOptionId }).ConfigureAwait(false);

        return votesDao.Select(v => v.Export());
    }

    public async Task<Vote?> FindUniqueVoteByPollAsync(Guid pollId, Guid userId)
    {
        var sql = "SELECT * FROM votes WHERE poll_id = @pollId AND user_id = @userId;";

        var voteDao = await Connection.QuerySingleOrDefaultAsync<VoteDao>(sql, new { pollId, userId }).ConfigureAwait(false);

        return voteDao?.Export();
    }

    public async Task<Vote?> FindUniqueVoteByOptionAsync(Guid pollOptionId, Guid userId)
    {
        var sql = "SELECT * FROM votes WHERE poll_option_id = @pollOptionId AND user_id = @userId;";

        var voteDao = await Connection.QuerySingleOrDefaultAsync<VoteDao>(sql, new { pollOptionId, userId }).ConfigureAwait(false);

        return voteDao?.Export();
    }
}

[Table("votes")]
public class VoteDao : IBaseDao<Vote>
{
    #pragma warning disable IDE1006 // Estilos de Nomenclatura
    #pragma warning disable SA1300 // Element should begin with upper-case letter
    #pragma warning disable CA1707 // Identificadores não devem conter sublinhados
    [ExplicitKey]
    public Guid id { get; set; }
    public Guid poll_id { get; set; }
    public Guid poll_option_id { get; set; }
    public Guid user_id { get; set; }
    public DateTime voted_at { get; set; }
    #pragma warning restore CA1707 // Identificadores não devem conter sublinhados
    #pragma warning restore SA1300 // Element should begin with upper-case letter
    #pragma warning restore IDE1006 // Estilos de Nomenclatura

    // Construtor para Dapper
    public VoteDao() { }

    public VoteDao(Vote domain)
    {
        id = domain.Id;
        poll_id = domain.PollId;
        poll_option_id = domain.PollOptionId;
        user_id = domain.UserId;
        voted_at = domain.VotedAt;
    }

    public Vote Export() => new Vote(id, poll_id, poll_option_id, user_id, voted_at);
}