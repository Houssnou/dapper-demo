using FluentValidation;
using VideoRental.API.Features.Films;
using VideoRental.API.Shared.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<ConfigOptions>()
    .Bind(config: builder.Configuration.GetSection(key: ConfigOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register memory cache for caching categories
builder.Services.AddMemoryCache();

// Register Film interfaces and services
builder.Services.AddScoped<IFilmRepository, FilmRepositrory>();
builder.Services.AddScoped<IFilmService, FilmService>();

// Register validators (if using FluentValidation)
builder.Services.AddValidatorsFromAssemblyContaining<FilmDto>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
// Register Film endpoints
app.MapFilmEndpoints();

app.Run();
