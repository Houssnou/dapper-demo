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
    public class BulkDeleteManager
    {
        private readonly IOptions<ConfigOptions> _options;

        public BulkDeleteManager(IOptions<ConfigOptions> options)
        {
            _options = options;
        }

        public async Task BulkDeleteActorsAsync(IEnumerable<int> actorIds)
        {
            using var connection = new Npgsql.NpgsqlConnection(_options.Value.DefaultConnection);
            // Ensure the connection is open
            await connection.OpenAsync();
            // Use Z.Dapper.Plus for bulk delete
            await connection.BulkDeleteAsync(actorIds.Select(id => new Models.Actor { ActorId = id }));
        }
    }
}
