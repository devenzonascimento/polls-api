using MediatR;
using PollsApp.Domain.Entities;
using PollsApp.Domain.Exceptions;
using PollsApp.Domain.Repositories;
using PollsApp.Infrastructure.Events.Interfaces;

namespace PollsApp.Application.Commands.Handlers;

public class DeleteCommentCommandHandler(
    IUnitOfWork unitOfWork,
    IDomainEventDispatcher domainEventDispatcher
) : IRequestHandler<DeleteCommentCommand, bool>
{
    private readonly IUnitOfWork unitOfWork = unitOfWork;
    private readonly IDomainEventDispatcher domainEventDispatcher = domainEventDispatcher;

    public async Task<bool> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = await unitOfWork.PollCommentRepository.GetByIdAsync(request.CommentId).ConfigureAwait(false);

        if (comment == null || comment.IsDeleted)
            throw new ArgumentException($"Comment with ID {request.CommentId} not found.");

        var poll = await unitOfWork.PollRepository.GetByIdAsync(comment.PollId).ConfigureAwait(false);

        if (poll == null || poll.IsDeleted)
            throw new NotFoundException("Poll", comment.PollId);

        if (!poll.IsOpen)
            throw new InvalidStateException("This poll is closed.");

        var deletedCommentsToUpdate = new List<PollComment>();

        comment.MarkAsDeleted(request.UserId);
        deletedCommentsToUpdate.Add(comment);

        var commentReplies = await unitOfWork.PollCommentRepository.GetAllCommentReplies(comment.Id).ConfigureAwait(false);

        foreach (var commentReply in commentReplies)
        {
            commentReply.MarkAsDeletedByParentCommentDeletion();
            deletedCommentsToUpdate.Add(commentReply);
        }

        using var transaction = unitOfWork.PollCommentRepository.StartTransaction();

        try
        {
            foreach (var deletedCommentToUpdate in deletedCommentsToUpdate)
            {
                await unitOfWork.PollCommentRepository.WithTransaction(transaction).UpdateAsync(deletedCommentToUpdate).ConfigureAwait(false);
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
