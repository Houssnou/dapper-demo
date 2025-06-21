using ConsoleApp.Models;
using ConsoleApp.NonQueryOperations;
using Dapper;
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
builder.Services.AddTransient<DataInsertManager>();
builder.Services.AddTransient<DataDeleteManager>();
builder.Services.AddTransient<DataUpdateManager>();

SqlMapper.AddTypeHandler(new MpaaRatingTypeHandler());

var app = builder.Build();

// get services
var singleValueQueryManager = app.Services.GetRequiredService<SingleValueQueryManager>();
var singleRowQueryManager = app.Services.GetRequiredService<SingleRowQueryManager>();
var multipleRowsQueryManager = app.Services.GetRequiredService<MultipleQueryRowManager>();
var insertDataManager = app.Services.GetRequiredService<DataInsertManager>();
var deleteDataManager = app.Services.GetRequiredService<DataDeleteManager>();
var updateDataManager = app.Services.GetRequiredService<DataUpdateManager>();


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
                "Create New Actor",
                "Create Multiple Actors",
                "Delete Actor by ID",
                "Delete Multiple Actors by IDs",
                "Soft Delete Actor by ID",
                "Update Actor by ID",
                "Exit"
            }));

    AnsiConsole.Clear();

    switch (choice)
    {
        case "Single Value (Total Movies Count)":
            {
                var count = await singleValueQueryManager.GetTotalMoviesCountAsync();
                AnsiConsole.MarkupLine($"[green]Total movies count: {count}[/]");
                break;
            }
        case "Single Value (Movie Title by ID)":
            {
                var id = AnsiConsole.Ask<int>("[yellow]Enter the film ID:[/]");
                var title = await singleValueQueryManager.GetMovieTitleAsync(id);
                AnsiConsole.MarkupLine($"[green]Movie by id: {id} - Title: {title}[/]");
                break;
            }
        case "Single Row (Film by ID)":
            {
                var id = AnsiConsole.Ask<int>("[yellow]Enter the film ID:[/]");
                var film = await singleRowQueryManager.GetFilmByIdAsync(id);
                DisplayFilm(film);
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
        case "Create New Actor":
            {
                var firstName = AnsiConsole.Ask<string>("[yellow]Enter actor's first name:[/]");
                var lastName = AnsiConsole.Ask<string>("[yellow]Enter actor's last name:[/]");
                var actor = new Actor
                {
                    FirstName = firstName,
                    LastName = lastName,
                    LastUpdate = DateTime.UtcNow
                };
                insertDataManager.InsertActorActorAsync(actor);
                AnsiConsole.MarkupLine($"[green]Inserted actor: {firstName} {lastName}[/]");
                break;
            }
        case "Create Multiple Actors":
            {
                var actors = new List<Actor>();
                for (int i = 0; i < 3; i++)
                {
                    var firstName = AnsiConsole.Ask<string>($"[yellow]Enter actor {i + 1}'s first name:[/]");
                    var lastName = AnsiConsole.Ask<string>($"[yellow]Enter actor {i + 1}'s last name:[/]");
                    actors.Add(new Actor
                    {
                        FirstName = firstName,
                        LastName = lastName,
                        LastUpdate = DateTime.UtcNow
                    });
                }
                insertDataManager.InsertActorsAsync(actors);
                AnsiConsole.MarkupLine("[green]Inserted multiple actors.[/]");
                break;
            }
        case "Delete Actor by ID":
            {
                var actorId = AnsiConsole.Ask<int>("[yellow]Enter the actor ID to delete:[/]");
                deleteDataManager.DeleteActorAsync(actorId);
                AnsiConsole.MarkupLine($"[green]Deleted actor with ID: {actorId}[/]");
                break;
            }
        case "Delete Multiple Actors by IDs":
            {
                var actorIds = AnsiConsole.Ask<string>("[yellow]Enter actor IDs to delete (comma-separated):[/]");
                var ids = actorIds.Split(',').Select(id => int.Parse(id.Trim())).ToList();
                deleteDataManager.DeleteActorsWithIdsAsync(ids);
                AnsiConsole.MarkupLine($"[green]Deleted actors with IDs: {string.Join(", ", ids)}[/]");
                break;
            }
        case "Soft Delete Actor by ID":
            {
                var actorId = AnsiConsole.Ask<int>("[yellow]Enter the actor ID to soft delete:[/]");
                var dataUpdateManager = app.Services.GetRequiredService<DataUpdateManager>();
                dataUpdateManager.SoftDeleteActorAsync(actorId);
                AnsiConsole.MarkupLine($"[green]Soft deleted actor with ID: {actorId}[/]");
                break;
            }
        case "Update Actor by ID":
            {
                var actorId = AnsiConsole.Ask<int>("[yellow]Enter the actor ID to update:[/]");
                var firstName = AnsiConsole.Ask<string>("[yellow]Enter new first name:[/]");
                var lastName = AnsiConsole.Ask<string>("[yellow]Enter new last name:[/]");
                var dataUpdateManager = app.Services.GetRequiredService<DataUpdateManager>();
                dataUpdateManager.UpdateActorAsync(actorId, firstName, lastName);
                AnsiConsole.MarkupLine($"[green]Updated actor with ID: {actorId}[/]");
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
        film.Rating.ToString(),
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
            film.Rating.ToString(),
            film.SpecialFeatures ?? "",
            film.FullText?.ToString() ?? ""
        );
    }

    AnsiConsole.Write(table);
}