using FluentResults;
using Microsoft.Extensions.Caching.Memory;



namespace VideoRental.API.Features.Films
{
    public class FilmService : IFilmService
    {
        private readonly IFilmRepository _filmRepository;
        private readonly IMemoryCache _memoryCache;
        private const string CategoryCacheKey = "FilmCategories";

        public FilmService(IFilmRepository filmRepository, IMemoryCache memoryCache)
        {
            _filmRepository = filmRepository;
            _memoryCache = memoryCache;
        }

        public async Task<Result<FilmDto>> AddAsync(FilmDto filmDto)
        {
            var film = FilmMappingExtensions.MapToEntity(filmDto);
            var added = await _filmRepository.AddAsync(film);
            return added is not null
                ? Result.Ok(FilmMappingExtensions.MapToDto(added))
                : Result.Fail<FilmDto>("Failed to add film.");
        }

        public async Task<Result> DeleteAsync(int id)
        {
            var deleted = await _filmRepository.DeleteAsync(id);
            return deleted
                ? Result.Ok()
                : Result.Fail("Film not found or could not be deleted.");
        }

        public async Task<Result<IEnumerable<FilmDto>>> GetAllAsync()
        {
            var films = await _filmRepository.GetAllAsync();
            var dtos = films.Select(FilmMappingExtensions.MapToDto);
            return Result.Ok(dtos);
        }

        public async Task<Result<IEnumerable<FilmDto>>> GetAllAsync(int pageNumber, int pageSize)
        {
            var films = await _filmRepository.GetAllAsync(pageNumber, pageSize);
            var dtos = films.Select(FilmMappingExtensions.MapToDto);
            return Result.Ok(dtos);
        }

        public async Task<Result<FilmDto>> GetByIdAsync(int id)
        {
            var film = await _filmRepository.GetByIdAsync(id);
            return film is not null
                ? Result.Ok(FilmMappingExtensions.MapToDto(film))
                : Result.Fail<FilmDto>("Film not found.");
        }

        public async Task<Result<IEnumerable<FilmDto>>> GetFilmsByCategory(string category)
        {

            //var films = await _filmRepository.GetFilmsByCategory(categoryId);
            //var dtos = films.Select(FilmMappingExtensions.MapToDto);
            //return Result.Ok(dtos);

            throw new NotImplementedException("GetFilmsByCategory is not implemented yet.");
        }


    }
}