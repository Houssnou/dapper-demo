using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QueryingData.Options;
using QueryingData.QueryingData;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddOptions<ConfigOptions>()
    .Bind(config: builder.Configuration.GetSection(key: ConfigOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Register SingleValueQueryManager as a service
builder.Services.AddTransient<SingleValueQueryManager>();

var app = builder.Build();

// Get an instance from the service provider
var singleValueQueryManager = app.Services.GetRequiredService<SingleValueQueryManager>();

// Call the method and get the count
var count = singleValueQueryManager.GetTotalMoviesCountAsync().GetAwaiter().GetResult();

Console.WriteLine($"Total movies count: {count}");

Console.WriteLine("Dapper demo app. Press Enter to exit.");
Console.ReadLine();