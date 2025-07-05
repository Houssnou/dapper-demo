using FluentResults;

namespace VideoRental.API.Features.Films
{
    public interface IFilmService
    {
        Task<Result<FilmDto>> AddAsync(FilmDto filmDto);
        Task<Result> DeleteAsync(int id);
        Task<Result<IEnumerable<FilmDto>>> GetAllAsync(int pageNumber, int pageSize);
        Task<Result<FilmDto>> GetByIdAsync(int id);
        Task<Result<IEnumerable<FilmDto>>> GetFilmsByCategory(string category);
    }
}
