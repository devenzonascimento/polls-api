using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PollsApp.Api.DTOs.Poll;
using PollsApp.Api.Extensions;
using PollsApp.Application.Commands;
using PollsApp.Infrastructure.Data.Repositories.Interfaces;

namespace PollsApp.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/poll")]
    public class PollController : ControllerBase
    {
        private readonly IMediator mediator;

        public PollController(IMediator mediator, IPollOptionRepository pollRepository, IPollOptionRepository optionRepository)
        {
            this.mediator = mediator;
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

            return Created();
            //return CreatedAtAction(nameof(GetPoll), new { id = pollId }, new { id = pollId });
        }

        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetPoll(Guid id)
        //{
        //    var poll = await pollRepository.GetByIdAsync(id).ConfigureAwait(false);

        //    if (poll is null)
        //        return NotFound();

        //    //var options = await _optionRepository.GetByPollIdAsync(id);

        //    return Ok(new
        //    {
        //        poll.Id,
        //        poll.Title,
        //        poll.Description,
        //        poll.Status,
        //        poll.CreatedAt,
        //        //Options = options.Select(o => new { o.Id, o.Text })
        //    });
        //}
    }
}
