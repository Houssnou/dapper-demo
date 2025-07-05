using FluentValidation;

namespace VideoRental.API.Features.Films
{
    public class FilmDtoValidator : AbstractValidator<FilmDto>
    {
        public FilmDtoValidator()
        {
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.Rating).NotEmpty();
        }
    }
}
