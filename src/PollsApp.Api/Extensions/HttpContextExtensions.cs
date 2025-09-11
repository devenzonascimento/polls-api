using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PollsApp.Api.Extensions;

public static class HttpContextExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var userId = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                  ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (Guid.TryParse(userId, out var guid))
        {
            return guid;
        }

        return Guid.Empty;
    }

    public static string? GetUsername(this ClaimsPrincipal user)
    {
        return user.FindFirst("username")?.Value;
    }

    public static string? GetEmail(this ClaimsPrincipal user)
    {
        return user.FindFirst(JwtRegisteredClaimNames.Email)?.Value;
    }
}
