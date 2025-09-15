using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RickAndMortyApp.Data;
using RickAndMorty.Services;
using RickAndMorty.Services.Interfaces;
using RickAndMorty.Services.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;



var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
    })
    .ConfigureServices((ctx, services) =>
    {

        var conn = ctx.Configuration.GetConnectionString("DefaultConnection");        
        services.AddDbContext<RickAndMortyContext>(opt => opt.UseSqlServer(conn));

        services.AddHttpClient<ICharacterService, CharacterService>(c =>
        {
            c.BaseAddress = new Uri("https://rickandmortyapi.com/api/");
        });

        services.AddHttpClient<ILocationService, LocationService>(c =>
        {
            c.BaseAddress = new Uri("https://rickandmortyapi.com/api/");
        });

        services.AddHttpClient<IEpisodeService, EpisodeService>(c =>
        {
            c.BaseAddress = new Uri("https://rickandmortyapi.com/api/");
        });

    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders(); 
        logging.AddConsole();
        logging.SetMinimumLevel(LogLevel.Warning); 
    })
    .Build();

using var scope = host.Services.CreateScope();
var characterService = scope.ServiceProvider.GetRequiredService<ICharacterService>();
var episodeService = scope.ServiceProvider.GetRequiredService<IEpisodeService>();
var locationService = scope.ServiceProvider.GetRequiredService<ILocationService>();

Console.WriteLine("Starting data cleanup...");
await locationService.DeleteAllData();
Console.WriteLine("Existing data deleted successfully");

Console.WriteLine("Fetching location data...");
await locationService.FetchAndSaveLocationsAsync();
Console.WriteLine("Locations saved successfully");

Console.WriteLine("Fetching episode data...");
await episodeService.FetchAndSaveEpisodesAsync();
Console.WriteLine("Episodes saved successfully");

Console.WriteLine("Fetching character data...");
await characterService.FetchAndSaveAliveCharactersAsync();
Console.WriteLine("Alive characters have been stored in the database.");
