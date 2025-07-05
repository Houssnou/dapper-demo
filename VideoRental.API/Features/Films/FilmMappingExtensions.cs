namespace VideoRental.API.Features.Films
{
    public static class FilmMappingExtensions
    {
        // Mapping helpers
        public static Film MapToEntity(FilmDto dto)
        {
            return new Film
            {
                FilmId = dto.FilmId,
                Title = dto.Title,
                Description = dto.Description,
                ReleaseYear = dto.ReleaseYear,
                LanguageId = dto.LanguageId,
                RentalDuration = dto.RentalDuration,
                RentalRate = dto.RentalRate,
                Length = dto.Length,
                ReplacementCost = dto.ReplacementCost,
                Rating = dto.Rating,
                LastUpdate = dto.LastUpdate,
                SpecialFeatures = dto.SpecialFeatures,
                // FullText is not mapped from DTO (DTO exposes as object for JSON)
            };
        }

        public static FilmDto MapToDto(Film film)
        {
            return new FilmDto
            {
                FilmId = film.FilmId,
                Title = film.Title,
                Description = film.Description,
                ReleaseYear = film.ReleaseYear,
                LanguageId = film.LanguageId,
                RentalDuration = film.RentalDuration,
                RentalRate = film.RentalRate,
                Length = film.Length,
                ReplacementCost = film.ReplacementCost,
                Rating = film.Rating,
                LastUpdate = film.LastUpdate,
                SpecialFeatures = film.SpecialFeatures,
                FullText = film.FullText?.ToString() // Expose as string or convert to JSON as needed
            };
        }
    }
}
