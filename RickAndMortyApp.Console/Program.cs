using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RickAndMortyApp.Data;
using RickAndMorty.Services;
using RickAndMorty.Services.Interfaces;
using RickAndMorty.Services.Services;
using Microsoft.Extensions.Configuration;




var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {

        var conn = ctx.Configuration.GetConnectionString("Default");        
        services.AddDbContext<RickAndMortyContext>(opt => opt.UseSqlServer(conn));

        // Services
        services.AddHttpClient<ICharacterService, CharacterService>(c =>
        {
            c.BaseAddress = new Uri("https://rickandmortyapi.com/api/");
        });
    })
    .Build();

using var scope = host.Services.CreateScope();
var svc = scope.ServiceProvider.GetRequiredService<ICharacterService>();

await svc.FetchAndSaveAliveCharactersAsync();

Console.WriteLine("✅ Alive characters have been stored in the database.");
