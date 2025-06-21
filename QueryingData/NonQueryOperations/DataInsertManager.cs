using ConsoleApp.Models;
using Dapper;
using Microsoft.Extensions.Options;
using QueryingData.Models;
using QueryingData.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.NonQueryOperations
{
    public class DataInsertManager
    {
        private readonly IOptions<ConfigOptions> _options;

        public DataInsertManager(IOptions<ConfigOptions> options)
        {
            _options = options;
        }

        public void InsertFilmAsync(Film film)
        {
            using var connection = new Npgsql.NpgsqlConnection(_options.Value.DefaultConnection);

            var sql = "INSERT INTO film (title, description, release_year, language_id, rental_duration, rental_rate, length, replacement_cost, rating, last_update, special_features) " +
                      "VALUES (@Title, @Description, @ReleaseYear, @LanguageId, @RentalDuration, @RentalRate, @Length, @ReplacementCost, @Rating, @LastUpdate, @SpecialFeatures)";

            connection.Execute(sql, film);
        }

        public void InsertActorActorAsync(Actor actor)
        {
            using var connection = new Npgsql.NpgsqlConnection(_options.Value.DefaultConnection);
            var sql = "INSERT INTO actor (first_name, last_name, last_update) " +
                      "VALUES (@FirstName, @LastName, @LastUpdate)";

            connection.Execute(sql, actor);
        }

        public int InsertActorFilmAsyncReturnId(Actor actor)
        {
            using var connection = new Npgsql.NpgsqlConnection(_options.Value.DefaultConnection);

            var sql = "INSERT INTO actor (first_name, last_name, last_update) " +
                      "VALUES (@FirstName, @LastName, @LastUpdate) RETURNING actor_id";

            var actorId = connection.ExecuteScalar<int>(sql, actor);

            return actorId;
        }

        public void InsertActorsAsync(IEnumerable<Actor> actors)
        {
            using var connection = new Npgsql.NpgsqlConnection(_options.Value.DefaultConnection);
            var sql = "INSERT INTO actor (first_name, last_name, last_update) " +
                      "VALUES (@FirstName, @LastName, @LastUpdate)";

            connection.Execute(sql, actors);
        }
    }
}
