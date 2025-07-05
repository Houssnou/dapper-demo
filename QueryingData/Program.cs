using ConsoleApp;
using ConsoleApp.BulkOperations;
using ConsoleApp.Models;
using ConsoleApp.NonQueryOperations;
using ConsoleApp.QueryingData;
using ConsoleApp.Relationships;
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
using Z.Dapper.Plus;


var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddOptions<ConfigOptions>()
    .Bind(config: builder.Configuration.GetSection(key: ConfigOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

// dapper plus configuration
DapperPlusManager.Entity<Film>().Table("film");
DapperPlusManager.Entity<Actor>()
    .Table("actor")
    .Map(a => a.ActorId, "actor_id")
    .Map(a => a.FirstName, "first_name")
    .Map(a => a.LastName, "last_name")
    .Map(a => a.LastUpdate, "last_update")
    /*.UseBulkOptions(x=>x.IgnoreOnUpdateExpression = y=>new {y.isDeLeted})*/;

// Register services
builder.Services.AddAppServices();

SqlMapper.AddTypeHandler(new MpaaRatingTypeHandler());

var app = builder.Build();

// get services
var singleValueQueryManager = app.Services.GetRequiredService<SingleValueQueryManager>();
var singleRowQueryManager = app.Services.GetRequiredService<SingleRowQueryManager>();
var multipleRowsQueryManager = app.Services.GetRequiredService<MultipleQueryRowManager>();
var insertDataManager = app.Services.GetRequiredService<DataInsertManager>();
var deleteDataManager = app.Services.GetRequiredService<DataDeleteManager>();
var updateDataManager = app.Services.GetRequiredService<DataUpdateManager>();
var storeProcedureManager = app.Services.GetRequiredService<StoreProcedureManager>();
var relationalQueryManager = app.Services.GetRequiredService<RelationalQueryManager>();
var bulkInsertManager = app.Services.GetRequiredService<BulkInsertManager>();
var bulkUpdateManager = app.Services.GetRequiredService<BulkUpdateManager>();
var bulkDeleteManager = app.Services.GetRequiredService<BulkDeleteManager>();
var bulkUpsertManagger = app.Services.GetRequiredService<BulkUpsertManager>();



while (true)
{
    AnsiConsole.Clear();

    //// Display the menu as a panel at the top
    //var menuPanel = new Panel(
    //    "[yellow]Select a query type:[/]\n" +
    //    "1. Single Value (Total Movies Count)\n" +
    //    "2. Single Value (Movie Title by ID)\n" +
    //    "3. Single Row (Film by ID)\n" +
    //    "4. Multiple Row (Search Films by Title/Keyword)\n" +
    //    "5. Create New Actor\n" +
    //    "6. Create Multiple Actors\n" +
    //    "7. Delete Actor by ID\n" +
    //    "8. Delete Multiple Actors by IDs\n" +
    //    "9. Soft Delete Actor by ID\n" +
    //    "10. Update Actor by ID\n" +
    //    "11. Get Actor by ID (Stored Procedure)\n" +
    //    "12. Get Actor by ID (Using Reader)\n" +
    //    "13. Get Film by ID with Category Name (Relational Query)\n" +
    //    "14. Bulk Insert Actors\n" +
    //    "15. Bulk Update Actors\n" +
    //    "16. Exit"
    //).Border(BoxBorder.Rounded).Header("[bold blue]Main Menu[/]");

    //AnsiConsole.Write(menuPanel);

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
                "Get Actor by ID (Stored Procedure)",
                "Get Actor by ID (Using Reader)",
                "Get Film by ID with Category Name (Relational Query)",
                "Bulk Insert Actors",
                "Bulk Update Actors",
                "Bulk Delete Actors",
                "Bulk Upsert Actors",
                "Exit"
            }));

    AnsiConsole.Clear(); // 

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
        case "Get Actor by ID (Stored Procedure)":
            {
                var actorId = AnsiConsole.Ask<int>("[yellow]Enter the actor ID to retrieve:[/]");
                var actor = await storeProcedureManager.GetActorAsync(actorId);
                if (actor != null)
                {
                    AnsiConsole.MarkupLine($"[green]Actor ID: {actor.ActorId}, Name: {actor.FirstName} {actor.LastName}, Last Update: {actor.LastUpdate}[/]");
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]No actor found with that ID.[/]");
                }
                break;
            }
        case "Get Actor by ID (Using Reader)":
            {
                var actorId = AnsiConsole.Ask<int>("[yellow]Enter the actor ID to retrieve using reader:[/]");
                var readerManager = app.Services.GetRequiredService<ReaderManager>();
                var actor = await readerManager.GetActorAsync(actorId);
                if (actor != null)
                {
                    AnsiConsole.MarkupLine($"[green]Actor ID: {actor.ActorId}, Name: {actor.FirstName} {actor.LastName}, Last Update: {actor.LastUpdate}[/]");
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]No actor found with that ID.[/]");
                }
                break;
            }
        case "Get Film by ID with Category Name (Relational Query)":
            {
                var filmId = AnsiConsole.Ask<int>("[yellow]Enter the film ID to retrieve with category name:[/]");
                var film = await relationalQueryManager.FilmWithCategoryAsync(filmId);
                DisplayFilmWithCategory(film);
                break;
            }
        case "Bulk Insert Actors":
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
                        LastUpdate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
                    });
                }
                await bulkInsertManager.BulkInsertActorsAsync(actors);
                AnsiConsole.MarkupLine("[green]Bulk inserted actors.[/]");
                break;
            }
        case "Bulk Update Actors":
            {
                var actorIdsInput = AnsiConsole.Ask<string>("[yellow]Enter actor IDs to delete (comma-separated):[/]");
                var ids = actorIdsInput
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(id => int.TryParse(id.Trim(), out var parsedId) ? parsedId : (int?)null)
                    .Where(id => id.HasValue)
                    .Select(id => id.Value)
                    .ToList();

                if (ids.Count == 0)
                {
                    AnsiConsole.MarkupLine("[red]No valid IDs entered.[/]");
                    break;
                }

                await bulkUpdateManager.BulkUpdateActorsAsync(ids);

                AnsiConsole.MarkupLine("[green]Bulk updated actors.[/]");
                break;
            }
        case "Bulk Delete Actors":
            {
                var actorIdsInput = AnsiConsole.Ask<string>("[yellow]Enter actor IDs to delete (comma-separated):[/]");
                var ids = actorIdsInput
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(id => int.TryParse(id.Trim(), out var parsedId) ? parsedId : (int?)null)
                    .Where(id => id.HasValue)
                    .Select(id => id.Value)
                    .ToList();
                if (ids.Count == 0)
                {
                    AnsiConsole.MarkupLine("[red]No valid IDs entered.[/]");
                    break;
                }
                await bulkDeleteManager.BulkDeleteActorsAsync(ids);
                AnsiConsole.MarkupLine("[green]Bulk deleted actors.[/]");
                break;
            }
        case "Bulk Upsert Actors":
            {
                await bulkUpsertManagger.BulkUpsertActorsAsync();
                AnsiConsole.MarkupLine("[green]Bulk upserted actors.[/]");
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

void DisplayFilmWithCategory(FilmWithCategory? filmWithCategory)
{
    if (filmWithCategory == null)
    {
        AnsiConsole.MarkupLine("[red]No film found.[/]");
        return;
    }

    var table = new Table().Border(TableBorder.Rounded);
    table.AddColumn("ID");
    table.AddColumn("Title");
    table.AddColumn("Category");

    table.AddRow(
        filmWithCategory.FilmId.ToString(),
        filmWithCategory.Title ?? "",
        filmWithCategory.Category ?? ""
    );

    AnsiConsole.Write(table);
}