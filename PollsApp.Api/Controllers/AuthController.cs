using Microsoft.AspNetCore.Mvc;
using PollsApp.Api.DTOs.Auth;
using PollsApp.Application.Services.Interfaces;

namespace PollsApp.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService authService;

    public AuthController(IAuthService auth) => authService = auth;

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest req)
    {
        await authService.RegisterAsync(req.Username, req.Email, req.Password).ConfigureAwait(false);

        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest req)
    {
        var token = await authService.LoginAsync(req.Email, req.Password).ConfigureAwait(false);

        return Ok(new { token });
    }
}
