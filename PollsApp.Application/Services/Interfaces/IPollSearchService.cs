using PollsApp.Infrastructure.Data.Search.Documents;

namespace PollsApp.Application.Services.Interfaces;

public interface IPollSearchService
{
    Task CreateIndexAsync();
    Task IndexPollAsync(PollDocument doc);
    Task UpdatePollStatusAsync(Guid id, string newStatus);
    Task<IEnumerable<Guid>> SearchPollAsync(string searchTerm, bool? isOpen);
}
