using MediatR;
using PollsApp.Domain.Entities;
using PollsApp.Infrastructure.Data.Repositories.Interfaces;
using PollsApp.Infrastructure.Events.Interfaces;

namespace PollsApp.Application.Commands.Handlers;

public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, bool>
{
    private readonly IPollRepository pollRepository;
    private readonly IPollCommentRepository pollCommentRepository;
    private readonly IDomainEventDispatcher domainEventDispatcher;

    public DeleteCommentCommandHandler(
        IPollRepository pollRepository,
        IPollCommentRepository pollCommentRepository,
        IDomainEventDispatcher domainEventDispatcher
    )
    {
        this.pollRepository = pollRepository;
        this.pollCommentRepository = pollCommentRepository;
        this.domainEventDispatcher = domainEventDispatcher;
    }

    public async Task<bool> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = await pollCommentRepository.GetByIdAsync(request.CommentId).ConfigureAwait(false);

        if (comment == null || comment.IsDeleted)
            throw new ArgumentException($"Comment with ID {request.CommentId} not found.");

        var poll = await pollRepository.GetByIdAsync(comment.PollId).ConfigureAwait(false);

        poll?.EnsureExistsAndOpen();

        var deletedCommentsToUpdate = new List<PollComment>();

        comment.MarkAsDeleted(request.UserId);
        deletedCommentsToUpdate.Add(comment);

        var commentReplies = await pollCommentRepository.GetAllCommentReplies(comment.Id).ConfigureAwait(false);

        foreach (var commentReply in commentReplies)
        {
            commentReply.MarkAsDeletedByParentCommentDeletion();
            deletedCommentsToUpdate.Add(commentReply);
        }

        using var transaction = pollCommentRepository.StartTransaction();

        try
        {
            foreach (var deletedCommentToUpdate in deletedCommentsToUpdate)
            {
                await pollCommentRepository.WithTransaction(transaction).UpdateAsync(deletedCommentToUpdate).ConfigureAwait(false);
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }

        await domainEventDispatcher.Dispatch(deletedCommentsToUpdate.SelectMany(c => c.Events)).ConfigureAwait(false);

        return true;
    }
}
