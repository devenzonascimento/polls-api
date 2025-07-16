using System.Data;
using Dapper;
using Dapper.Contrib.Extensions;
using PollsApp.Domain.Entities;
using PollsApp.Infrastructure.Data.Repositories.Interfaces;

namespace PollsApp.Infrastructure.Data.Repositories;

public class PollCommentRepository : BaseRepository<PollCommentRepository, IPollCommentRepository>, IPollCommentRepository
{
    public PollCommentRepository(IDbConnection connection, IDbTransaction transaction = null)
        : base(connection, transaction) { }

    public async Task InsertAsync(PollComment pollComment)
        => await InsertAsync<PollComment, PollCommentDao>(pollComment, Transaction).ConfigureAwait(false);

    public async Task UpdateAsync(PollComment pollComment)
        => await UpdateAsync<PollComment, PollCommentDao>(pollComment, Transaction).ConfigureAwait(false);

    public async Task<PollComment?> GetByIdAsync(Guid commentId)
    {
        var sql = "SELECT * FROM poll_comments p WHERE p.id = @commentId";

        var pollCommentDao = await Connection.QuerySingleOrDefaultAsync<PollCommentDao>(sql, new { commentId }, Transaction).ConfigureAwait(false);

        return pollCommentDao?.Export();
    }

    public async Task<IEnumerable<PollComment>> GetAllCommentReplies(Guid commentId)
    {
        var sql = "SELECT * FROM poll_comments p WHERE p.reply_to = @commentId AND p.is_deleted = FALSE";

        var pollCommentDao = await Connection.QueryAsync<PollCommentDao>(sql, new { commentId }, Transaction).ConfigureAwait(false);

        return pollCommentDao.Select(c => c.Export());
    }

    public async Task<IEnumerable<PollComment>> GetCommentsByPollAsync(Guid pollId)
    {
        var sql = "SELECT * FROM poll_comments p WHERE p.poll_id = @pollId AND p.is_deleted = FALSE";

        var pollCommentDao = await Connection.QueryAsync<PollCommentDao>(sql, new { pollId }, Transaction).ConfigureAwait(false);

        return pollCommentDao.Select(c => c.Export());
    }
}

[Table("poll_comments")]
public class PollCommentDao : IBaseDao<PollComment>
{
    #pragma warning disable IDE1006 // Estilos de Nomenclatura
    #pragma warning disable SA1300 // Element should begin with upper-case letter
    #pragma warning disable CA1707 // Identificadores não devem conter sublinhados
    [ExplicitKey]
    public Guid id { get; set; }
    public Guid poll_id { get; set; }
    public string text { get; set; }
    public bool is_edited { get; set; }
    public Guid? reply_to { get; set; }
    public Guid created_by { get; set; }
    public bool is_deleted { get; set; }
    public DateTime created_at { get; set; }
    #pragma warning restore CA1707 // Identificadores não devem conter sublinhados
    #pragma warning restore SA1300 // Element should begin with upper-case letter
    #pragma warning restore IDE1006 // Estilos de Nomenclatura

    public PollCommentDao() { }

    public PollCommentDao(PollComment domain)
    {
        id = domain.Id;
        poll_id = domain.PollId;
        text = domain.Text;
        is_edited = domain.IsEdited;
        reply_to = domain.ReplyTo;
        created_by = domain.CreatedBy;
        is_deleted = domain.IsDeleted;
        created_at = domain.CreatedAt;
    }

    public PollComment Export()
        => new PollComment(id, poll_id, text, is_edited, reply_to, created_by, is_deleted, created_at);
}