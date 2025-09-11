using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PollsApp.Application.Services.Interfaces;
using PollsApp.Domain.Entities;
using PollsApp.Infrastructure.Data.Repositories.Interfaces;

namespace PollsApp.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository userRepository;
    private readonly IConfiguration config;

    public AuthService(IUserRepository userRepository, IConfiguration config)
    {
        this.userRepository = userRepository;
        this.config = config;
    }

    public async Task RegisterAsync(string username, string email, string password)
    {
        var exists = await userRepository.GetByEmailAsync(email).ConfigureAwait(false);
        if (exists is not null)
            throw new ApplicationException("Email já cadastrado.");

        var user = new User(username, email, password);
        await userRepository.SaveAsync(user).ConfigureAwait(false);
    }

    public async Task<string> LoginAsync(string email, string password)
    {
        var user = await userRepository.GetByEmailAsync(email).ConfigureAwait(false);

        if (user is null || !user.ValidatePassword(password))
            throw new ApplicationException("Credenciais inválidas.");

        // Gera JWT
        var jwtKey = config["Jwt:Key"];
        var jwtIss = config["Jwt:Issuer"];
        var jwtExp = int.Parse(config["Jwt:ExpireMinutes"]);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, (string)user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, (string)user.Email),
            new Claim("username", (string)user.Username),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtIss,
            audience: jwtIss,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(jwtExp),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
