using VideoRental.API.Shared.Entitites;

namespace VideoRental.API.Shared.Interfaces
{
    public interface IRepository<T> where T : IEntity
    {
        /// <summary>
        /// Gets all entities of type T.
        /// </summary>
        /// <returns>A collection of entities.</returns>
        Task<IEnumerable<T>> GetAllAsync();
        /// <summary>
        /// Gets an entity by its ID.
        /// </summary>
        /// <param name="id">The ID of the entity.</param>
        /// <returns>The entity if found, otherwise null.</returns>
        Task<T?> GetByIdAsync(int id);
        /// <summary>
        /// Adds a new entity of type T.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>The added entity.</returns>
        Task<T> AddAsync(T entity);
        /// <summary>
        /// Updates an existing entity of type T.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        Task<bool> UpdateAsync(T entity);
        /// <summary>
        /// Deletes an entity by its ID.
        /// </summary>
        /// <param name="id">The ID of the entity to delete.</param>
        Task<bool> DeleteAsync(int id);
    }
}
