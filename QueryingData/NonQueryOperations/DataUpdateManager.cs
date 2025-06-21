using Dapper;
using Microsoft.Extensions.Options;
using QueryingData.Options;

namespace ConsoleApp.NonQueryOperations
{
    public class DataUpdateManager
    {
        private readonly IOptions<ConfigOptions> _options;

        public DataUpdateManager(IOptions<ConfigOptions> options)
        {
            _options = options;
        }


        public void SoftDeleteActorAsync(int actorId)
        {
            using var connection = new Npgsql.NpgsqlConnection(_options.Value.DefaultConnection);
            var sql = "UPDATE actor SET is_deleted = TRUE WHERE actor_id = @ActorId";

            connection.Execute(sql, new { ActorId = actorId });
        }

        public void UpdateActorAsync(int actorId, string firstName, string lastName)
        {
            using var connection = new Npgsql.NpgsqlConnection(_options.Value.DefaultConnection);
            var sql = "UPDATE actor SET first_name = @FirstName, last_name = @LastName WHERE actor_id = @ActorId";

            connection.Execute(sql, new { FirstName = firstName, LastName = lastName, ActorId = actorId });
        }
    }
}
