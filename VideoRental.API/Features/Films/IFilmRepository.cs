using VideoRental.API.Shared.Interfaces;

namespace VideoRental.API.Features.Films
{
    public interface IFilmRepository : IRepository<Film>
    {
        Task<IEnumerable<Film>> GetFilmsByCategory(int categoryId);
        Task<IEnumerable<Film>> GetTop10ByRentalRate();
        Task<IEnumerable<Film>> GetAllAsync(int pageNumber, int pageSize);
        Task<IEnumerable<string>> GetAllCategoriesAsync();
    }
}
