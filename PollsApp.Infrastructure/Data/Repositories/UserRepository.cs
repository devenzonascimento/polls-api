using System.Data;
using Dapper;
using Dapper.Contrib.Extensions;
using PollsApp.Domain.Entities;
using PollsApp.Infrastructure.Data.Repositories.Interfaces;

namespace PollsApp.Infrastructure.Data.Repositories;

public class UserRepository
    : BaseRepository<PollRepository, IUserRepository>,
      IUserRepository
{
    public UserRepository(IDbConnection connection, IDbTransaction transaction = null)
        : base(connection, transaction) { }

    public async Task<User> SaveAsync(User user)
        => await SaveAsync<User, UserDao>(user, Transaction).ConfigureAwait(false);

    public async Task<IEnumerable<User>> SaveAsync(IEnumerable<User> users)
        => await SaveAsync<User, UserDao>(users, Transaction).ConfigureAwait(false);

    public async Task<User?> GetByIdAsync(Guid id)
    {
        var sql = "SELECT * FROM users WHERE id = @id;";

        var userDao = await Connection.QuerySingleOrDefaultAsync<UserDao>(sql, new { id }).ConfigureAwait(false);

        return userDao?.Export();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var sql = $@"SELECT * FROM users WHERE email = @email;";

        var userDao = await Connection.QuerySingleOrDefaultAsync<UserDao>(sql, new { email }).ConfigureAwait(false);

        return userDao?.Export();
    }
}

[Table("users")]
public class UserDao : IBaseDao<User>
{
    #pragma warning disable IDE1006 // Estilos de Nomenclatura
    #pragma warning disable SA1300 // Element should begin with upper-case letter
    #pragma warning disable CA1707 // Identificadores não devem conter sublinhados
    [ExplicitKey]
    public Guid id { get; set; }
    public string username { get; set; }
    public string email { get; set; }
    public string password_hash { get; set; }
    public bool deleted { get; set; }
    public DateTime created_at { get; set; }
    #pragma warning restore CA1707 // Identificadores não devem conter sublinhados
    #pragma warning restore SA1300 // Element should begin with upper-case letter
    #pragma warning restore IDE1006 // Estilos de Nomenclatura

    // Construtor para Dapper
    public UserDao() { }

    // Construtor que mapeia da entidade de domínio
    public UserDao(User domain)
    {
        id = domain.Id;
        username = domain.Username;
        email = domain.Email;
        password_hash = domain.PasswordHash;
        deleted = domain.Deleted;
        created_at = domain.CreatedAt;
    }

    public User Export()
    {
        // Aqui você pode usar um construtor ou factory de Poll
        return new User(id, username, email, password_hash, deleted, created_at);
    }
}
