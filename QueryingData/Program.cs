using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QueryingData.Models;
using QueryingData.Options;
using QueryingData.QueryingData;
using Spectre.Console;
using Spectre.Console.Json;
using System.Text.Json;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddOptions<ConfigOptions>()
    .Bind(config: builder.Configuration.GetSection(key: ConfigOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Register services
builder.Services.AddTransient<SingleValueQueryManager>();
builder.Services.AddTransient<SingleRowQueryManager>();
builder.Services.AddTransient<MultipleQueryRowManager>();

var app = builder.Build();

// get services
var singleValueQueryManager = app.Services.GetRequiredService<SingleValueQueryManager>();
var singleRowQueryManager = app.Services.GetRequiredService<SingleRowQueryManager>();
var multipleRowsQueryManager = app.Services.GetRequiredService<MultipleQueryRowManager>();

while (true)
{
    var choice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("[yellow]Select a query type:[/]")
            .AddChoices(new[] {
                "Single Value (Total Movies Count)",
                "Single Value (Movie Title by ID)",
                "Single Row (Film by ID)",
                "Multiple Row (Search Films by Title/Keyword)",
                "Exit"
            }));

    AnsiConsole.Clear();

    switch (choice)
    {
        case "Single Value (Total Movies Count)":
            {
                var count = singleValueQueryManager.GetTotalMoviesCountAsync().GetAwaiter().GetResult();
                AnsiConsole.MarkupLine($"[green]Total movies count: {count}[/]");
                break;
            }
        case "Single Value (Movie Title by ID)":
            {
                var id = AnsiConsole.Ask<int>("[yellow]Enter the film ID:[/]");
                var title = singleValueQueryManager.GetMovieTitleAsync(id).GetAwaiter().GetResult();
                AnsiConsole.MarkupLine($"[green]Movie by id: {id} - Title: {title}[/]");
                break;
            }
        case "Single Row (Film by ID)":
            {
                var id = AnsiConsole.Ask<int>("[yellow]Enter the film ID:[/]");
                var film = singleRowQueryManager.GetFilmByIdAsync(id).GetAwaiter().GetResult();
                DisplayFilm(film);

                // Display as JSON
                if (film != null)
                {
                    var filmJson = JsonSerializer.Serialize(film, new JsonSerializerOptions { WriteIndented = true });
                    AnsiConsole.Write(new JsonText(filmJson));
                }
                break;
            }
        case "Multiple Row (Search Films by Title/Keyword)":
            {
                var searchTerm = AnsiConsole.Ask<string>("[yellow]Enter a film title or keyword to search:[/]");
                AnsiConsole.MarkupLine($"[bold blue]Results for:[/] [green]{searchTerm}[/]");
                var filmsByTitle = multipleRowsQueryManager.GetFilmByTitleAsync(searchTerm).GetAwaiter().GetResult();
                DisplayFilms(filmsByTitle);
                break;
            }
        case "Exit":
            AnsiConsole.MarkupLine("[green]Dapper demo app. Press Enter to exit.[/]");
            Console.ReadLine();
            return;
    }

    AnsiConsole.MarkupLine("\n[grey]Press any key to return to the menu...[/]");
    Console.ReadKey(true);
    AnsiConsole.Clear();
}

void DisplayFilm(Film? film)
{
    if (film == null)
    {
        AnsiConsole.MarkupLine("[red]No film found.[/]");
        return;
    }

    var table = new Table().Border(TableBorder.Rounded);
    table.AddColumn("ID");
    table.AddColumn("Title");
    table.AddColumn("Description");
    table.AddColumn("Year");
    table.AddColumn("Lang");
    table.AddColumn("Dur");
    table.AddColumn("Rate");
    table.AddColumn("Len");
    table.AddColumn("Repl.Cost");
    table.AddColumn("Rating");
    table.AddColumn("Features");
    table.AddColumn("FullText");

    table.AddRow(
        film.FilmId.ToString(),
        film.Title ?? "",
        film.Description?.Length > 40 ? film.Description.Substring(0, 40) + "…" : film.Description ?? "",
        film.ReleaseYear.ToString(),
        film.LanguageId.ToString(),
        film.RentalDuration.ToString(),
        film.RentalRate.ToString("0.00"),
        film.Length.ToString(),
        film.ReplacementCost.ToString("0.00"),
        film.Rating ?? "",
        film.SpecialFeatures ?? "",
        film.FullText?.ToString() ?? ""
    );

    AnsiConsole.Write(table);
}

void DisplayFilms(IEnumerable<Film> films)
{
    var table = new Table().Border(TableBorder.Rounded);
    table.AddColumn("ID");
    table.AddColumn("Title");
    table.AddColumn("Description");
    table.AddColumn("Year");
    table.AddColumn("Lang");
    table.AddColumn("Dur");
    table.AddColumn("Rate");
    table.AddColumn("Len");
    table.AddColumn("Repl.Cost");
    table.AddColumn("Rating");
    table.AddColumn("Features");
    table.AddColumn("FullText");

    foreach (var film in films)
    {
        table.AddRow(
            film.FilmId.ToString(),
            film.Title ?? "",
            film.Description?.Length > 40 ? film.Description.Substring(0, 40) + "…" : film.Description ?? "",
            film.ReleaseYear.ToString(),
            film.LanguageId.ToString(),
            film.RentalDuration.ToString(),
            film.RentalRate.ToString("0.00"),
            film.Length.ToString(),
            film.ReplacementCost.ToString("0.00"),
            film.Rating ?? "",
            film.SpecialFeatures ?? "",
            film.FullText?.ToString() ?? ""
        );
    }

    AnsiConsole.Write(table);
}