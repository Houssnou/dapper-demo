using ConsoleApp.Models;
using Microsoft.Extensions.Options;
using QueryingData.Options;

using Z.Dapper.Plus;

namespace ConsoleApp.BulkOperations
{
    public class BulkUpsertManager
    {
        private readonly IOptions<ConfigOptions> _options;

        public BulkUpsertManager(IOptions<ConfigOptions> options)
        {
            _options = options;
        }

        public async Task BulkUpsertActorsAsync()
        {
            //build a list of actors to upsert
            var actors = new List<Actor>
            {
                new Actor { FirstName = "Alice", LastName = "Smith", LastUpdate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified) },
                new Actor { FirstName = "Bob", LastName = "Johnson", LastUpdate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified) },
                new Actor { FirstName = "Charlie", LastName = "Williams", LastUpdate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified) },
                new Actor { FirstName = "Diana", LastName = "Brown", LastUpdate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified) },
                new Actor {ActorId = 207, FirstName = "Upsert", LastName = "Upsert", LastUpdate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified) }
            };

            using var connection = new Npgsql.NpgsqlConnection(_options.Value.DefaultConnection);
            // Ensure the connection is open
            await connection.OpenAsync();
            // Use Z.Dapper.Plus for bulk upsert
            await connection.BulkMergeAsync(actors);
        }
    }
}
