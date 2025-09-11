using PollsApp.Infrastructure.Data.Search.Documents;

namespace PollsApp.Application.Services.Interfaces;

public interface IPollSearchService
{
    Task CreateIndexAsync();
    Task IndexAsync(PollDocument doc);
    Task UpdateAsync(Guid pollId, PollDocument doc);
    Task DeleteAsync(Guid pollId);
    Task<IEnumerable<Guid>> SearchAsync(string searchTerm, bool? isOpen);
}
