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
[Route("api/polls")]
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
    public async Task<IActionResult> GetPollById([FromRoute] Guid id)
    {
        var query = new GetPollByIdQuery(id);

        var pollSummary = await mediator.Send(query).ConfigureAwait(false);

        return Ok(pollSummary);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string search, [FromQuery] bool? isOpen)
    {
        var query = new SearchPollsQuery(search, isOpen);

        var pollsSummaries = await mediator.Send(query).ConfigureAwait(false);

        return Ok(pollsSummaries);
    }

    [HttpGet("top-ranking")]
    public async Task<IActionResult> GetTop()
    {
        var query = new GetRankedPollsQuery();

        var pollsSummaries = await mediator.Send(query).ConfigureAwait(false);

        return Ok(pollsSummaries);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePoll([FromBody] CreatePollRequest request)
    {
        var userRequesterId = User.GetUserId();

        var command = new CreatePollCommand(
            userRequesterId,
            request.Title,
            request.Description,
            request.AllowMultiple,
            request.ClosesAt,
            request.Options
        );

        var pollId = await mediator.Send(command).ConfigureAwait(false);

        return CreatedAtAction(nameof(GetPollById), new { id = pollId }, new { id = pollId });
    }

    [HttpPut]
    public async Task<IActionResult> UpdatePoll([FromBody] UpdatePollRequest request)
    {
        var userRequesterId = User.GetUserId();

        var command = new UpdatePollCommand(
            userRequesterId,
            request.PollId,
            request?.Title,
            request?.Description,
            request?.ClosesAt,
            request?.Options.Select(o => new Application.Commands.OptionToUpdateData(o.OldText, o.NewText))
        );

        await mediator.Send(command).ConfigureAwait(false);

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePoll([FromRoute] Guid id)
    {
        var userRequesterId = User.GetUserId();

        var command = new DeletePollCommand(userRequesterId, id);

        await mediator.Send(command).ConfigureAwait(false);

        return Ok();
    }
}
