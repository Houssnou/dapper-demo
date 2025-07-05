using ConsoleApp.Models;
using Microsoft.Extensions.Options;
using QueryingData.Options;

namespace ConsoleApp.QueryingData
{
    /// <summary>
    /// ReaderManager demonstrates how to retrieve data from the database using a raw data reader (NpgsqlDataReader)
    /// instead of relying on Dapper's automatic model mapping.
    ///
    /// <para><b>Pros of using a data reader directly:</b></para>
    /// <list type="bullet">
    /// <item><description>Finer control over data access and column mapping, allowing for custom logic or performance optimizations.</description></item>
    /// <item><description>Can handle complex or dynamic result sets that do not map cleanly to a single model.</description></item>
    /// <item><description>Reduces dependencies on external libraries for mapping.</description></item>
    /// </list>
    ///
    /// <para><b>Cons of using a data reader directly:</b></para>
    /// <list type="bullet">
    /// <item><description>Requires more boilerplate code for reading and converting each column.</description></item>
    /// <item><description>Increases risk of runtime errors due to manual column name/ordinal mismatches or type conversion issues.</description></item>
    /// <item><description>Less maintainable and harder to refactor compared to Dapper's model mapping.</description></item>
    /// <item><description>Misses out on Dapper's features like multi-mapping, parameterization, and automatic type handling.</description></item>
    /// </list>
    /// </summary>

    public class ReaderManager
    {
        private readonly IOptions<ConfigOptions> _options;

        public ReaderManager(IOptions<ConfigOptions> options)
        {
            _options = options;
        }

        public async Task<Actor?> GetActorAsync(int actorId)
        {
            using var connection = new Npgsql.NpgsqlConnection(_options.Value.DefaultConnection);

            var sql = "SELECT actor_id, first_name, last_name, last_update FROM actor WHERE actor_id = @actorId";

            using var command = new Npgsql.NpgsqlCommand(sql, connection);

            command.Parameters.AddWithValue("actorId", actorId);

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
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
            return null; // or throw an exception if not found
        }

        public async Task<Actor?> GetActorAsync_Example2(int actorId)
        {
            using var connection = new Npgsql.NpgsqlConnection(_options.Value.DefaultConnection);

            await connection.OpenAsync();

            const string sql = "SELECT actor_id, first_name, last_name, last_update FROM actor WHERE actor_id = @actorId";

            using var command = new Npgsql.NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("actorId", actorId);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                // Use column ordinals: 0=actor_id, 1=first_name, 2=last_name, 3=last_update
                return new Actor
                {
                    ActorId = !reader.IsDBNull(0) ? reader.GetInt32(0) : 0,
                    FirstName = !reader.IsDBNull(1) ? reader.GetString(1) : string.Empty,
                    LastName = !reader.IsDBNull(2) ? reader.GetString(2) : string.Empty,
                    LastUpdate = !reader.IsDBNull(3) ? reader.GetDateTime(3) : DateTime.MinValue
                };
            }
            return null;
        }
    }
}
