using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;
using VideoRental.API.Shared.Options;

namespace VideoRental.API.Features.Films
{
    public class FilmRepositrory : IFilmRepository
    {
        private readonly IOptions<ConfigOptions> _options;

        public FilmRepositrory(IOptions<ConfigOptions> options)
        {
            _options = options;
        }

        public async Task<Film> AddAsync(Film entity)
        {
            const string sql = @"
                INSERT INTO film (title, description, release_year, language_id, rental_duration, rental_rate, length, replacement_cost, rating, last_update, special_features)
                VALUES (@Title, @Description, @ReleaseYear, @LanguageId, @RentalDuration, @RentalRate, @Length, @ReplacementCost, @Rating, @LastUpdate, @SpecialFeatures)
                RETURNING film_id;";

            using var connection = new NpgsqlConnection(_options.Value.DefaultConnection);
            entity.LastUpdate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            entity.FilmId = await connection.ExecuteScalarAsync<int>(sql, entity);
            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string sql = "DELETE FROM film WHERE film_id = @Id";
            using var connection = new NpgsqlConnection(_options.Value.DefaultConnection);
            var affected = await connection.ExecuteAsync(sql, new { Id = id });
            return affected > 0;
        }

        /// <summary>
        /// Get all films (for IRepository compatibility).
        /// </summary>
        public async Task<IEnumerable<Film>> GetAllAsync()
        {
            const string sql = "SELECT * FROM film";
            using var connection = new NpgsqlConnection(_options.Value.DefaultConnection);
            return await connection.QueryAsync<Film>(sql);
        }

        /// <summary>
        /// Get paginated films.
        /// </summary>
        /// <param name="pageNumber">1-based page number</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <returns>Paged list of films</returns>
        public async Task<IEnumerable<Film>> GetAllAsync(int pageNumber, int pageSize)
        {
            const string sql = @"
                SELECT * FROM film
                ORDER BY film_id
                OFFSET @Offset ROWS
                LIMIT @PageSize";
            using var connection = new NpgsqlConnection(_options.Value.DefaultConnection);
            int offset = (pageNumber - 1) * pageSize;
            return await connection.QueryAsync<Film>(sql, new { Offset = offset, PageSize = pageSize });
        }
        public async Task<Film?> GetByIdAsync(int id)
        {
            const string sql = "SELECT * FROM film WHERE film_id = @Id";
            using var connection = new NpgsqlConnection(_options.Value.DefaultConnection);
            return await connection.QueryFirstOrDefaultAsync<Film>(sql, new { Id = id });
        }

        public async Task<IEnumerable<Film>> GetFilmsByCategory(int categoryId)
        {
            const string sql = @"
                SELECT f.* FROM film f
                INNER JOIN film_category fc ON f.film_id = fc.film_id
                WHERE fc.category_id = @CategoryId";
            using var connection = new NpgsqlConnection(_options.Value.DefaultConnection);
            return await connection.QueryAsync<Film>(sql, new { CategoryId = categoryId });
        }

        public async Task<IEnumerable<Film>> GetTop10ByRentalRate()
        {
            const string sql = "SELECT * FROM film ORDER BY rental_rate DESC LIMIT 10";
            using var connection = new NpgsqlConnection(_options.Value.DefaultConnection);
            return await connection.QueryAsync<Film>(sql);
        }

        public async Task<bool> UpdateAsync(Film entity)
        {
            const string sql = @"
                UPDATE film SET
                    title = @Title,
                    description = @Description,
                    release_year = @ReleaseYear,
                    language_id = @LanguageId,
                    rental_duration = @RentalDuration,
                    rental_rate = @RentalRate,
                    length = @Length,
                    replacement_cost = @ReplacementCost,
                    rating = @Rating,
                    last_update = @LastUpdate,
                    special_features = @SpecialFeatures
                WHERE film_id = @FilmId";
            using var connection = new NpgsqlConnection(_options.Value.DefaultConnection);
            entity.LastUpdate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            var affected = await connection.ExecuteAsync(sql, entity);
            return affected > 0;
        }

        public async Task<IEnumerable<string>> GetAllCategoriesAsync()
        {
            const string sql = "SELECT DISTINCT name FROM category ORDER BY name";
            using var connection = new NpgsqlConnection(_options.Value.DefaultConnection);
            return await connection.QueryAsync<string>(sql);
        }
    }
}