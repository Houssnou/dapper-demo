using Microsoft.Extensions.Options;
using QueryingData.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.Dapper.Plus;

namespace ConsoleApp.BulkOperations
{
    public class BulkUpdateManager
    {
        private readonly IOptions<ConfigOptions> _options;

        public BulkUpdateManager(IOptions<ConfigOptions> options)
        {
            _options = options;
        }

        public async Task BulkUpdateActorsAsync(IEnumerable<int> actorIds)
        {
            // Create a list of actors to update
            var actors = actorIds.Select(id => new Models.Actor
            {
                ActorId = id,
                FirstName = "UpdatedFirstName", 
                LastName = "UpdatedLastName",
                LastUpdate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)

            });

            using var connection = new Npgsql.NpgsqlConnection(_options.Value.DefaultConnection);
            // Ensure the connection is open
            await connection.OpenAsync();
            // Use Z.Dapper.Plus for bulk update
            await connection.BulkUpdateAsync(actors);
        }
    }
}
