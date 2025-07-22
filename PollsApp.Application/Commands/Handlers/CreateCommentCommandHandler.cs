using MediatR;
using OpenSearch.Client;
using PollsApp.Domain.Entities;
using PollsApp.Domain.Exceptions;
using PollsApp.Infrastructure.Data.Repositories.Interfaces;
using PollsApp.Infrastructure.Events.Interfaces;

namespace PollsApp.Application.Commands.Handlers;

public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, Guid>
{
    private readonly IPollRepository pollRepository;
    private readonly IPollCommentRepository pollCommentRepository;
    private readonly IDomainEventDispatcher domainEventDispatcher;

    public CreateCommentCommandHandler(
        IPollRepository pollRepository,
        IPollCommentRepository pollCommentRepository,
        IDomainEventDispatcher domainEventDispatcher
    )
    {
        this.pollRepository = pollRepository;
        this.pollCommentRepository = pollCommentRepository;
        this.domainEventDispatcher = domainEventDispatcher;
    }

    public async Task<Guid> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        var poll = await pollRepository.GetByIdAsync(request.PollId).ConfigureAwait(false);

        if (poll == null || poll.IsDeleted)
            throw new NotFoundException("Poll", request.PollId);

        if (!poll.IsOpen)
            throw new InvalidStateException("This poll is closed.");

        var comment = PollComment.Create(
            request.Text,
            request.PollId,
            request.UserId
        );

        await pollCommentRepository.InsertAsync(comment).ConfigureAwait(false);

        await domainEventDispatcher.Dispatch(comment.Events).ConfigureAwait(false);

        return comment.Id;
    }
}
