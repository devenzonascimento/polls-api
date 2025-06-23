using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PollsApp.Api.DTOs.Poll;
using PollsApp.Api.Extensions;
using PollsApp.Application.Commands;
using PollsApp.Application.Queries;

namespace PollsApp.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/poll")]
public class PollController : ControllerBase
{
    private readonly IMediator mediator;

    public PollController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPolls()
    {
        var query = new GetAllPollsQuery();

        var polls = await mediator.Send(query).ConfigureAwait(false);

        return Ok(polls);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPollById(Guid id)
    {
        var query = new GetPollByIdQuery(id);

        var pollSummary = await mediator.Send(query).ConfigureAwait(false);

        return Ok(pollSummary);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePoll([FromBody] CreatePollRequest request)
    {
        var userRequesterId = User.GetUserId();

        var command = new CreatePollCommand(
            userRequesterId,
            request.Title,
            request.Description,
            request.ClosesAt,
            request.Options
        );

        var pollId = await mediator.Send(command).ConfigureAwait(false);

        return CreatedAtAction(nameof(GetPollById), new { id = pollId }, new { id = pollId });
    }
}
