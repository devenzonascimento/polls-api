using OpenSearch.Client;

namespace PollsApp.Infrastructure.Data.Search;

public static class OpenSearchConnectionSingleton
{
    private static string connectionString = string.Empty;

    /// <summary>
    /// Deve ser chamado uma única vez na inicialização da aplicação.
    /// </summary>
    public static void SetConnectionString(string connectionString)
        => OpenSearchConnectionSingleton.connectionString = connectionString;

    /// <summary>
    /// Instância única e lazy do OpenSearchClient.
    /// </summary>
    public static OpenSearchClient GetClient() => LazyClient.Value;

    private static readonly Lazy<OpenSearchClient> LazyClient =
        new Lazy<OpenSearchClient>(() =>
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("OpenSearch connection string not set.");

            var node = new Uri(connectionString);

            var settings = new ConnectionSettings(node);

            settings.EnableDebugMode();

            return new OpenSearchClient(settings);
        });
}
