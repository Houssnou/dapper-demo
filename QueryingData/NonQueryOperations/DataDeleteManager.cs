using Dapper;
using Microsoft.Extensions.Options;
using QueryingData.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.NonQueryOperations
{
    public class DataDeleteManager
    {
        private readonly IOptions<ConfigOptions> _options;

        public DataDeleteManager(IOptions<ConfigOptions> options)
        {
            _options = options;
        }

        public void DeleteActorAsync(int actorId)
        {
            using var connection = new Npgsql.NpgsqlConnection(_options.Value.DefaultConnection);

            var sql = "DELETE FROM actor WHERE actor_id = @ActorId";

            connection.Execute(sql, new { ActorId = actorId });

        }

        public void DeleteActorsWithIdsAsync(IEnumerable<int> actorIds)
        {
            using var connection = new Npgsql.NpgsqlConnection(_options.Value.DefaultConnection);
            var sql = "DELETE FROM actor WHERE actor_id = ANY(@ActorIds)";
            connection.Execute(sql, new { ActorIds = actorIds });
        }
    }
}
