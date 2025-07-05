using Dapper;
using Microsoft.Extensions.Options;
using QueryingData.Models;
using QueryingData.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Relationships
{
    public class RelationalQueryManager
    {
        private readonly IOptions<ConfigOptions> _options;

        public RelationalQueryManager(IOptions<ConfigOptions> options)
        {
            _options = options;
        }

        public async Task<FilmWithCategory> FilmWithCategoryAsync(int filmId)
        {
            using var connection = new Npgsql.NpgsqlConnection(_options.Value.DefaultConnection);

            // join the film, film_category, and category tables to get the film details along with its category
            var sql = @"
                SELECT f.film_id, f.title, c.name AS category
                FROM film f
                JOIN film_category fc ON f.film_id = fc.film_id
                JOIN category c ON fc.category_id = c.category_id
                WHERE f.film_id = @filmId";


            var result = await connection.QueryAsync<FilmWithCategory>(sql, new { filmId });

            return result.FirstOrDefault() ?? new FilmWithCategory();
        }
    }
}
