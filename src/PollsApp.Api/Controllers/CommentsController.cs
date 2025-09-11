using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PollsApp.Api.DTOs.Comments;
using PollsApp.Api.Extensions;
using PollsApp.Application.Commands;
using PollsApp.Application.Queries;

namespace PollsApp.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/comments")]
public class CommentsController : ControllerBase
{
    private readonly IMediator mediator;

    public CommentsController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet("{pollId}")]
    public async Task<IActionResult> GetCommentsByPollId([FromRoute] Guid pollId)
    {
        var query = new GetCommentsByPollIdQuery(pollId);

        var comments = await mediator.Send(query).ConfigureAwait(false);

        return Ok(comments);
    }

    [HttpPost]
    public async Task<IActionResult> CreateComment([FromBody] CreateCommentRequest request)
    {
        var userId = User.GetUserId();

        var command = new CreateCommentCommand(userId, request.PollId, request.Comment);

        var commentId = await mediator.Send(command).ConfigureAwait(false);

        return Ok(new { id = commentId });
    }

    [HttpPost("reply")]
    public async Task<IActionResult> ReplyComment([FromBody] ReplyCommentRequest request)
    {
        var userId = User.GetUserId();

        var command = new ReplyCommentCommand(userId, request.CommentIdToReply, request.Comment);

        var commentId = await mediator.Send(command).ConfigureAwait(false);

        return Ok(new { id = commentId });
    }

    [HttpPut("edit")]
    public async Task<IActionResult> EditComment([FromBody] EditCommentRequest request)
    {
        var userId = User.GetUserId();

        var command = new EditCommentCommand(userId, request.CommentId, request.NewComment);

        await mediator.Send(command).ConfigureAwait(false);

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteComment([FromRoute] Guid id)
    {
        var userId = User.GetUserId();

        var command = new DeleteCommentCommand(userId, id);

        await mediator.Send(command).ConfigureAwait(false);

        return Ok();
    }
}
