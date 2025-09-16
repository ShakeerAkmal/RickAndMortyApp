using RickAndMorty.Services.Converter;
using RickAndMorty.Services.Dtos;
using RickAndMorty.Services.Interfaces;
using RickAndMortyApp.Data;
using System.Text.Json;

namespace RickAndMorty.Services.Services
{
    public class EpisodeService : IEpisodeService
    {
        private readonly HttpClient _http;
        private readonly RickAndMortyContext _db;

        public EpisodeService(HttpClient http, RickAndMortyContext db)
        {
            _http = http;
            _db = db;
        }

        public async Task FetchAndSaveEpisodesAsync()
        {
           var allEpisodes = new List<EpisodeDto>();
            for (int page = 1; ; page++)
            {
                var episodes = await FetchEpisodesAsync(page);
                if (episodes.Count == 0)
                {
                    break;
                }
                allEpisodes.AddRange(episodes);
            }
            var episodeEntities = allEpisodes.Select(e =>Converters.ToEntity(e)).ToList();
            _db.Episodes.AddRange(episodeEntities);
            var aa = await _db.SaveChangesAsync();
        }

        private async Task<List<EpisodeDto>?> FetchEpisodesAsync(int page)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                PropertyNameCaseInsensitive = true
            };

            var response = await _http.GetAsync($"episode?page={page}");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<EpisodeDto>>(jsonString, options);
                return apiResponse?.Results ?? new List<EpisodeDto>();
            }
            return new List<EpisodeDto>();
        }

    }
}
