using PollsApp.Domain.Entities;
using PollsApp.Domain.Primitives;

namespace PollsApp.Domain.Events;

public record CommentRepliedDomainEvent(PollComment commentToReply, PollComment replyComment) : IDomainEvent;
