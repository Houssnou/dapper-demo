
using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;
using QueryingData.Options;

namespace QueryingData.QueryingData
{
    internal class SingleValueQueryManager
    {
        private readonly IOptions<ConfigOptions> _configOptions;

        public SingleValueQueryManager(IOptions<ConfigOptions> configOptions)
        {
            _configOptions = configOptions;
        }

        /// <summary>
        /// Retrieves the total number of movies in the database.
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetTotalMoviesCountAsync()
        {
            using var connection = new NpgsqlConnection(_configOptions.Value.DefaultConnection);

            var sql = "SELECT COUNT(*) FROM Film";
            var count = await connection.ExecuteScalarAsync<int>(sql);
            return count;
        }

        public async Task<string> GetMovieTitleAsync(int filmId)
        {
            using var connection = new NpgsqlConnection(_configOptions.Value.DefaultConnection);

            var sql = "SELECT title FROM film WHERE film_id = @filmId";

            var title = await connection.ExecuteScalarAsync<string>(sql, new { filmId });

            return title;
        }
    }
}
