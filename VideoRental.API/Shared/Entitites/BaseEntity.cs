namespace VideoRental.API.Shared.Entitites
{
    /// <summary>
    /// Based class for all entities in the application and to serve as  a marker for all entities.
    /// </summary>
    public abstract class BaseEntity
    {
        public int Id { get; set; }
    }

    /// <summary>
    /// Ancestor interface for all entities in the application.
    /// </summary>
    public interface IEntity
    {
    }
}