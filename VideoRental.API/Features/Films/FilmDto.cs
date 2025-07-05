namespace VideoRental.API.Features.Films
{
    public class FilmDto
    {
        public int FilmId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int ReleaseYear { get; set; }
        public int LanguageId { get; set; }
        public int RentalDuration { get; set; }
        public decimal RentalRate { get; set; }
        public int Length { get; set; }
        public decimal ReplacementCost { get; set; }
        public string Rating { get; set; }
        public DateTime LastUpdate { get; set; }
        public string? SpecialFeatures { get; set; }
        public object? FullText { get; set; } 

    }

    public class FilmCreateDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int ReleaseYear { get; set; }
        public int LanguageId { get; set; }
        public int RentalDuration { get; set; }
        public decimal RentalRate { get; set; }
        public int Length { get; set; }
        public decimal ReplacementCost { get; set; }
        public string Rating { get; set; }
        public string? SpecialFeatures { get; set; }
    }


}
