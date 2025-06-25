using MediatR;
using PollsApp.Domain.Entities;
using PollsApp.Infrastructure.Data.Repositories.Interfaces;

namespace PollsApp.Application.Commands.Handlers;

public class VoteCommandHandler : IRequestHandler<VoteCommand, bool>
{
    private readonly IPollRepository pollRepository;
    private readonly IVoteRepository voteRepository;

    public VoteCommandHandler(IPollRepository pollRepository, IVoteRepository voteRepository)
    {
        this.pollRepository = pollRepository;
        this.voteRepository = voteRepository;
    }

    public async Task<bool> Handle(VoteCommand request, CancellationToken cancellationToken)
    {
        var option = await pollRepository.GetOptionByIdAsync(request.OptionId).ConfigureAwait(false);

        if (option == null)
            throw new ArgumentException("Option not found.");

        var poll = await pollRepository.GetByIdAsync(option.PollId).ConfigureAwait(false);

        if (poll == null)
            throw new ArgumentException("Poll not found.");

        if (!poll.IsOpen)
            throw new ArgumentException("This poll is closed");

        var existingVote = await voteRepository.FindUserVote(option.PollId, request.UserId).ConfigureAwait(false);

        if (existingVote is null)
        {
            var vote = new Vote(option.PollId, option.Id, request.UserId);

            await voteRepository.SaveAsync(vote).ConfigureAwait(false);

            return true;
        }

        if (option.Id != existingVote.PollOptionId)
        {
            existingVote.ChangeOption(option.Id);

            await voteRepository.SaveAsync(existingVote).ConfigureAwait(false);

            return true;
        }

        return true;
    }
}
