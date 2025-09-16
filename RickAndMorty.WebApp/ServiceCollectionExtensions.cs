
using RickAndMorty.Services.Interfaces;
using RickAndMorty.Services.Services;

namespace RickAndMorty.WebApp
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRickAndMortyServices(this IServiceCollection services)
        {
            void ConfigureClient(HttpClient client)
            {
                client.BaseAddress = new Uri("https://rickandmortyapi.com/api/");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            }

            services.AddHttpClient<ILocationService, LocationService>(ConfigureClient);
            services.AddHttpClient<ICharacterService, CharacterService>(ConfigureClient);
            services.AddHttpClient<IEpisodeService, EpisodeService>(ConfigureClient);

            return services;
        }
    }
}
