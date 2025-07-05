using FluentResults;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Linq;
using System.Threading.Tasks;

namespace VideoRental.API.Features.Films
{
    public static class FilmEndpoints
    {
        public static void MapFilmEndpoints(this IEndpointRouteBuilder endpoints)
        {
            // GET /films[?pageNumber=1&pageSize=50] - Get all or paginated films
            endpoints.MapGet("/films", async (int? pageNumber, int? pageSize, IFilmService service) =>
            {
                if (pageNumber.HasValue && pageSize.HasValue)
                {
                    var pagedResult = await service.GetAllAsync(pageNumber.Value, pageSize.Value);
                    return pagedResult.IsSuccess ? Results.Ok(pagedResult.Value) : Results.NotFound(pagedResult.Errors);
                }
                else
                {
                    // Use large page size to get all, or implement a GetAllAsync() overload if needed
                    var allResult = await service.GetAllAsync(1, int.MaxValue);
                    return allResult.IsSuccess ? Results.Ok(allResult.Value) : Results.NotFound(allResult.Errors);
                }
            });

            // GET /films/{id} - Get film by ID
            endpoints.MapGet("/films/{id:int}", async (int id, IFilmService service) =>
            {
                var result = await service.GetByIdAsync(id);
                return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(result.Errors);
            });

            // POST /films - Create a new film
            endpoints.MapPost("/films", async (FilmDto filmDto, IFilmService service, IValidator<FilmDto> validator) =>
            {
                var validationResult = await validator.ValidateAsync(filmDto);
                if (!validationResult.IsValid)
                {
                    return Results.BadRequest(validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
                }

                var result = await service.AddAsync(filmDto);
                return result.IsSuccess ? Results.Created($"/films/{result.Value.FilmId}", result.Value) : Results.BadRequest(result.Errors);
            });

            // GET /films/by-category/{category} - Get films by category (category is a string)
            endpoints.MapGet("/films/by-category/{category}", async (string category, IFilmService service) =>
            {
                var result = await service.GetFilmsByCategory(category);
                return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(result.Errors);
            });
        }
    }
}