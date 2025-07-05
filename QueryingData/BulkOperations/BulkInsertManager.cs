using ConsoleApp.Models;
using Microsoft.Extensions.Options;
using QueryingData.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.Dapper.Plus;

namespace ConsoleApp.BulkOperations
{
    /// <summary>
    /// BulkInsertManager is responsible for efficiently inserting large collections of entities into the database.
    /// This class is designed to handle high-volume data operations that would be inefficient with individual inserts.
    /// <para>
    /// The implementation will use the Z.Dapper.Plus library, which provides high-performance bulk operations for Dapper.
    /// Note: Z.Dapper.Plus is a commercial library, but it is free to use in development environments.
    /// </para>
    /// </summary>
    public class BulkInsertManager
    {
        private readonly IOptions<ConfigOptions> _options;

        public static readonly List<Actor> SampleActors = new()
        {
            new Actor { FirstName = "John", LastName = "Smith", LastUpdate = DateTime.UtcNow },
            new Actor { FirstName = "Emma", LastName = "Johnson", LastUpdate = DateTime.UtcNow },
            new Actor { FirstName = "Michael", LastName = "Williams", LastUpdate = DateTime.UtcNow },
            new Actor { FirstName = "Olivia", LastName = "Brown", LastUpdate = DateTime.UtcNow },
            new Actor { FirstName = "David", LastName = "Jones", LastUpdate = DateTime.UtcNow }
        };

        public BulkInsertManager(IOptions<ConfigOptions> options)
        {
            _options = options;
        }

        public async Task BulkInsertActorsAsync(IEnumerable<Actor> actors)
        {
            using var connection = new Npgsql.NpgsqlConnection(_options.Value.DefaultConnection);

            // Ensure the connection is open
            await connection.OpenAsync();

            // Use Z.Dapper.Plus for bulk insert
            await connection.BulkInsertAsync(actors);
        }
    }
}
