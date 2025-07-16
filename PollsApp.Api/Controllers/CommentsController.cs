using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PollsApp.Api.DTOs.Comments;
using PollsApp.Api.Extensions;
using PollsApp.Application.Commands;

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

        var command = new ReplyCommentCommand(userId, request.CommentIdToReply, request.ReplyComment);

        var commentId = await mediator.Send(command).ConfigureAwait(false);

        return Ok(new { id = commentId });
    }
}
