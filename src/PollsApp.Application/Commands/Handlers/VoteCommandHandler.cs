using MediatR;
using PollsApp.Application.Services.Interfaces;
using PollsApp.Domain.Entities;
using PollsApp.Domain.Exceptions;
using PollsApp.Domain.Repositories;

namespace PollsApp.Application.Commands.Handlers;

public class VoteCommandHandler(
    IUnitOfWork unitOfWork,
    IPollRankingService pollRankingService
) : IRequestHandler<VoteCommand, bool>
{
    private readonly IUnitOfWork unitOfWork = unitOfWork;
    private readonly IPollRankingService pollRankingService = pollRankingService;

    public async Task<bool> Handle(VoteCommand request, CancellationToken cancellationToken)
    {
        var option = await unitOfWork.PollRepository.GetOptionByIdAsync(request.OptionId).ConfigureAwait(false);

        if (option == null)
            throw new ArgumentException("Option not found.");

        var poll = await unitOfWork.PollRepository.GetByIdAsync(option.PollId).ConfigureAwait(false);

        if (poll == null || poll.IsDeleted)
            throw new NotFoundException("Poll", option.PollId);

        if (!poll.IsOpen)
            throw new InvalidStateException("This poll is closed.");

        var existingVote = poll.AllowMultiple
            ? await unitOfWork.VoteRepository.FindUniqueVoteByOptionAsync(option.Id, request.UserId).ConfigureAwait(false)
            : await unitOfWork.VoteRepository.FindUniqueVoteByPollAsync(option.PollId, request.UserId).ConfigureAwait(false);

        // Novo voto
        if (existingVote is null)
        {
            var vote = new Vote(option.PollId, option.Id, request.UserId);

            await unitOfWork.VoteRepository.SaveAsync(vote).ConfigureAwait(false);

            await pollRankingService.IncrementVoteAsync(option.PollId).ConfigureAwait(false);

            return true;
        }

        // Trocando de voto
        if (option.Id != existingVote.PollOptionId)
        {
            existingVote.ChangeOption(option.Id);

            await unitOfWork.VoteRepository.SaveAsync(existingVote).ConfigureAwait(false);

            await pollRankingService.IncrementVoteAsync(option.PollId).ConfigureAwait(false);

            return true;
        }

        // Removendo voto
        if (option.Id == existingVote.PollOptionId)
        {
            await unitOfWork.VoteRepository.DeleteByIdAsync(existingVote.Id).ConfigureAwait(false);

            await pollRankingService.DecrementVoteAsync(option.PollId).ConfigureAwait(false);

            return true;
        }

        return true;
    }
}
