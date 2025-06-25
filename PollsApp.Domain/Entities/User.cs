namespace PollsApp.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Username { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public User(string username, string email, string plainPassword)
    {
        Id = Guid.Empty;
        Username = username;
        Email = email;
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(plainPassword);
        IsDeleted = false;
        CreatedAt = DateTime.UtcNow;
    }

    public User(Guid id, string username, string email, string passwordHash, bool isDeleted, DateTime createdAt)
    {
        Id = id;
        Username = username;
        Email = email;
        PasswordHash = passwordHash;
        IsDeleted = isDeleted;
        CreatedAt = createdAt;
    }

    public bool ValidatePassword(string plainPassword)
        => BCrypt.Net.BCrypt.Verify(plainPassword, PasswordHash);

    public void ChangePassword(string newPlain)
        => PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPlain);
}
