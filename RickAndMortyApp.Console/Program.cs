using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RickAndMortyApp.Data;
using RickAndMorty.Services.Interfaces;
using RickAndMorty.Services.Services;

var host = CreateHostBuilder(args).Build();

await RunDataImportAsync(host);

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration(ConfigureApp)
        .ConfigureServices(ConfigureServices)
        .ConfigureLogging(ConfigureLogging);

static void ConfigureApp(HostBuilderContext context, IConfigurationBuilder config)
{
    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
}

static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
{
    var connectionString = context.Configuration.GetConnectionString("DefaultConnection");
    
    services.AddDbContext<RickAndMortyContext>(options => 
        options.UseSqlServer(connectionString));

    RegisterHttpClients(services);
    services.AddMemoryCache();
}

static void RegisterHttpClients(IServiceCollection services)
{
    const string baseUrl = "https://rickandmortyapi.com/api/";
    
    services.AddHttpClient<ICharacterService, CharacterService>(client => 
        client.BaseAddress = new Uri(baseUrl));
    
    services.AddHttpClient<ILocationService, LocationService>(client => 
        client.BaseAddress = new Uri(baseUrl));
    
    services.AddHttpClient<IEpisodeService, EpisodeService>(client => 
        client.BaseAddress = new Uri(baseUrl));
}

static void ConfigureLogging(ILoggingBuilder logging)
{
    logging.ClearProviders()
           .AddConsole()
           .SetMinimumLevel(LogLevel.Warning);
}

static async Task RunDataImportAsync(IHost host)
{
    using var scope = host.Services.CreateScope();
    var services = scope.ServiceProvider;

    var characterService = services.GetRequiredService<ICharacterService>();
    var episodeService = services.GetRequiredService<IEpisodeService>();
    var locationService = services.GetRequiredService<ILocationService>();

    var importSteps = new (string StartMessage, string SuccessMessage, Func<Task> Action)[]
    {
        ("Starting data cleanup...", "Existing data deleted successfully", 
         () => locationService.DeleteAllData()),
        
        ("Fetching location data...", "Locations saved successfully", 
         locationService.FetchAndSaveLocationsAsync),
        
        ("Fetching episode data...", "Episodes saved successfully", 
         episodeService.FetchAndSaveEpisodesAsync),
        
        ("Fetching character data...", "Alive characters have been stored in the database.", 
         () => characterService.FetchAndSaveAliveCharactersAsync())
    };

    foreach (var (startMessage, successMessage, action) in importSteps)
    {
        Console.WriteLine(startMessage);
        await action();
        Console.WriteLine(successMessage);
    }
}
