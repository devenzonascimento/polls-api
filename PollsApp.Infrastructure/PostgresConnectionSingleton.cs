using System.Data;
using Npgsql;

namespace PollsApp.Infrastructure.Data.Infrastructure
{
    public static class PostgresConnectionSingleton
    {
        private static string readOnlyConnectionString = string.Empty;
        private static string readWriteConnectionString = string.Empty;

        public static bool HasConnectionString =>
            !string.IsNullOrWhiteSpace(readOnlyConnectionString) &&
            !string.IsNullOrWhiteSpace(readWriteConnectionString);

        /// <summary>
        /// Configure suas connection strings de leitura e escrita (por exemplo, em Program.cs).
        /// </summary>
        public static void SetConnectionStrings(string readOnly, string readWrite)
        {
            readOnlyConnectionString = readOnly;
            readWriteConnectionString = readWrite;
        }

        public static string GetReadOnlyConnectionString()
            => readOnlyConnectionString;

        public static string GetReadWriteConnectionString()
            => readWriteConnectionString;

        /// <summary>
        /// Abre e retorna uma conexão *aberta* para operações que exigem consistência (INSERT/UPDATE/DELETE).
        /// </summary>
        public static IDbConnection GetWriteConnection()
        {
            if (string.IsNullOrEmpty(readWriteConnectionString))
                throw new InvalidOperationException("Write connection string not set.");

            var conn = new NpgsqlConnection(readWriteConnectionString);
            conn.Open();
            return conn;
        }

        /// <summary>
        /// Abre e retorna uma conexão *aberta* para operações de leitura pura (SELECT).
        /// </summary>
        public static IDbConnection GetReadConnection()
        {
            if (string.IsNullOrEmpty(readOnlyConnectionString))
                throw new InvalidOperationException("Read connection string not set.");

            var conn = new NpgsqlConnection(readOnlyConnectionString);
            conn.Open();
            return conn;
        }
    }
}
