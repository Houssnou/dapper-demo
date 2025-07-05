using ConsoleApp.Models;
using Dapper;
using Microsoft.Extensions.Options;
using QueryingData.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.QueryingData
{
    public class StoreProcedureManager
    {
        private readonly IOptions<ConfigOptions> _options;

        public StoreProcedureManager(IOptions<ConfigOptions> options)
        {
            _options = options;
        }

        public async Task<Actor?> GetActorAsync(int actorId)
        {
            using var connection = new Npgsql.NpgsqlConnection(_options.Value.DefaultConnection);

            var parameters = new DynamicParameters();
            parameters.Add("p_actor_id", actorId, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);
            parameters.Add("actor_id", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);
            parameters.Add("first_name", dbType: System.Data.DbType.String, size: 50, direction: System.Data.ParameterDirection.Output);
            parameters.Add("last_name", dbType: System.Data.DbType.String, size: 50, direction: System.Data.ParameterDirection.Output);
            parameters.Add("last_update", dbType: System.Data.DbType.DateTime, direction: System.Data.ParameterDirection.Output);

            await connection.ExecuteAsync("get_actor_by_id", parameters, commandType: System.Data.CommandType.StoredProcedure);

            // Read output parameters
            var id = parameters.Get<int?>("actor_id");
            var firstName = parameters.Get<string>("first_name");
            var lastName = parameters.Get<string>("last_name");
            var lastUpdate = parameters.Get<DateTime?>("last_update");

            if (id == null) return null;

            return new Actor
            {
                ActorId = id.Value,
                FirstName = firstName,
                LastName = lastName,
                LastUpdate = lastUpdate ?? DateTime.MinValue
            };
        }

        public async Task<Actor> GetActorUsingReader(int actorId)
        {
            using var connection = new Npgsql.NpgsqlConnection(_options.Value.DefaultConnection);
            var parameters = new DynamicParameters();
            parameters.Add("p_actor_id", actorId, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);
            using var reader = await connection.ExecuteReaderAsync("get_actor_by_id", parameters, commandType: System.Data.CommandType.StoredProcedure);
            if (await reader.ReadAsync())
            {
                return new Actor
                {
                    ActorId = reader.GetInt32(reader.GetOrdinal("actor_id")),
                    FirstName = reader.GetString(reader.GetOrdinal("first_name")),
                    LastName = reader.GetString(reader.GetOrdinal("last_name")),
                    LastUpdate = reader.GetDateTime(reader.GetOrdinal("last_update"))
                };
            }
            return null;
        }
    }
}
