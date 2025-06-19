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
    internal class MultipleQueryRowManager
    {
        private readonly IOptions<ConfigOptions> _options;

        public MultipleQueryRowManager(IOptions<ConfigOptions> options)
        {
            _options = options;
        }

        public async Task<IEnumerable<Film>> GetFilmByTitleAsync(string title)
        {
            using var connection = new Npgsql.NpgsqlConnection(_options.Value.DefaultConnection);
            var sql = "SELECT * FROM film WHERE title ILIKE @pattern";
            var films = await connection.QueryAsync<Film>(sql, new { pattern = $"%{title}%" });

            return films;
        }
    }
}
