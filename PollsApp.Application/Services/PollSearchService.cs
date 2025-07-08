using OpenSearch.Client;
using PollsApp.Application.Services.Interfaces;
using PollsApp.Infrastructure.Data.Search;
using PollsApp.Infrastructure.Data.Search.Documents;

namespace PollsApp.Application.Services;

public class PollSearchService : IPollSearchService
{
    private const string IndexName = "polls";
    private readonly OpenSearchClient client;

    public PollSearchService()
    {
        client = OpenSearchConnectionSingleton.GetClient();
    }

    public async Task CreateIndexAsync()
    {
        var exists = await client.Indices.ExistsAsync(IndexName).ConfigureAwait(false);

        if (exists.Exists)
            return;

        var createResponse = await client.Indices.CreateAsync(IndexName, c => c
            .Settings(s => s
                .Analysis(a => a
                    .Analyzers(an => an
                        .Custom("autocomplete_analyzer", ca => ca
                            .Tokenizer("standard")
                            .Filters("lowercase", "autocomplete_edge_ngram")
                        )
                    )
                    .TokenFilters(tf => tf
                        .EdgeNGram("autocomplete_edge_ngram", e => e
                            .MinGram(2)
                            .MaxGram(20)
                        )
                    )
                )
            )
            .Map<PollDocument>(m => m
                .AutoMap()
                .Properties(ps => ps
                    .Text(t => t
                        .Name(n => n.Title)
                        .Analyzer("autocomplete_analyzer")
                    )
                    .Text(t => t
                        .Name(n => n.Description)
                        .Analyzer("standard")
                    )
                    .Text(t => t
                        .Name(n => n.Options)
                        .Fields(f => f
                            .Text(t => t
                                .Name("text")
                                .Analyzer("standard")
                            )
                        )
                    )
                    .Boolean(b => b
                        .Name(n => n.IsOpen)
                    )
                    .Date(d => d
                        .Name(n => n.CreatedAt)
                    )
                    .Date(d => d
                        .Name(n => n.ClosedAt)
                    )
                )
            )
        ).ConfigureAwait(false);

        if (!createResponse.IsValid)
            throw new Exception($"Failed to create index: {createResponse.DebugInformation}");
    }

    public async Task IndexPollAsync(PollDocument doc)
    {
        await CreateIndexAsync().ConfigureAwait(false);

        var response = await client.IndexAsync(doc, i => i.Index(IndexName).Id(doc.Id)).ConfigureAwait(false);

        if (!response.IsValid)
            throw new Exception($"Index error: {response.DebugInformation}");
    }

    public async Task UpdatePollStatusAsync(Guid id, string newStatus)
    {
        var response = await client.UpdateAsync<PollDocument, object>(id, u => u
            .Index(IndexName)
            .Doc(new { Status = newStatus })
        ).ConfigureAwait(false);

        if (!response.IsValid)
            throw new Exception($"Update error: {response.DebugInformation}");
    }

    public async Task<IEnumerable<Guid>> SearchPollAsync(string searchTerm, bool? isOpen)
    {
        var response = await client.SearchAsync<PollDocument>(sd => sd
            .Index(IndexName)
            .Size(0)
            .TrackTotalHits(false)
            .Query(q => q
                .Bool(b =>
                {
                    // 1) Se isOpen tiver valor, adiciona o Term filter
                    if (isOpen.HasValue)
                    {
                        b.Filter(f => f
                            .Term(t => t
                                .Field(fld => fld.IsOpen)
                                .Value(isOpen.Value)
                            )
                        );
                    }

                    // 2) Sempre adiciona o MultiMatch
                    b.Must(m => m
                        .MultiMatch(mm => mm
                            .Query(searchTerm)
                            .Fields(fs => fs
                                .Field(f => f.Title)
                                .Field(f => f.Description)
                                .Field(f => f.Options)
                            )
                        )
                    );

                    return b;
                })
            )
            .Aggregations(a => a
                .Terms("only_ids", t => t
                    .Field("_id")
                    .Size(10000)
                )
            )
        ).ConfigureAwait(false);

        if (!response.IsValid)
            throw new Exception($"Search error: {response.DebugInformation}");

        var pollsIds = response
            .Aggregations
            .Terms("only_ids")
            .Buckets
            .Select(b => new Guid(b.Key))
            .ToList();

        return pollsIds;
    }
}
