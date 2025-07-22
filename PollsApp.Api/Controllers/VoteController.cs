using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PollsApp.Api.Extensions;
using PollsApp.Application.Commands;

namespace PollsApp.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/votes")]
public class VoteController : ControllerBase
{
    private readonly IMediator mediator;

    public VoteController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Vote([FromQuery] Guid optionId)
    {
        var userId = User.GetUserId();

        var command = new VoteCommand(userId, optionId);

        await mediator.Send(command).ConfigureAwait(false);

        return Ok();
    }
}
