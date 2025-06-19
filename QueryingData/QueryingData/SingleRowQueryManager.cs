using Dapper;
using Microsoft.Extensions.Options;
using QueryingData.Models;
using QueryingData.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryingData.QueryingData
{
    internal class SingleRowQueryManager
    {
        private readonly IOptions<ConfigOptions> _options;

        public SingleRowQueryManager(IOptions<ConfigOptions> options)
        {
            _options = options;
        }

        public async Task<Film?> GetFilmByIdAsync(int filmId)
        {
            using var connection = new Npgsql.NpgsqlConnection(_options.Value.DefaultConnection);

            var sql = "SELECT * FROM film WHERE film_id = @filmId";

            var film = await connection.QuerySingleOrDefaultAsync<Film>(sql, new { filmId });

            return film;
        }
    }
}
