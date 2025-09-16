using RickAndMortyApp.Data;
using RickAndMorty.Services.Interfaces;
using RickAndMorty.Services.Dtos;
using RickAndMorty.Services.Converter;
using System.Text.Json;

namespace RickAndMorty.Services.Services
{
    public class LocationService : ILocationService
    {
        private readonly HttpClient _http;
        private readonly RickAndMortyContext _db;

        public LocationService(HttpClient http, RickAndMortyContext db)
        {
            _http = http;
            _db = db;
        }

        public async Task DeleteAllData()
        {
            _db.CharacterEpisodes.RemoveRange(_db.CharacterEpisodes);
            _db.Episodes.RemoveRange(_db.Episodes);
            _db.Characters.RemoveRange(_db.Characters);
            _db.Locations.RemoveRange(_db.Locations);
        }
        
        public async Task FetchAndSaveLocationsAsync()
        {
            _db.CharacterEpisodes.RemoveRange(_db.CharacterEpisodes);
            _db.Episodes.RemoveRange(_db.Episodes);
            _db.Characters.RemoveRange(_db.Characters);
            _db.Locations.RemoveRange(_db.Locations);

            var allLocations = new List<LocationDto>();
            for (int page = 1; ; page++)
            {
                var locations = await FetchLocationsAsync(page);
                if (locations.Count == 0)
                {
                    break;
                }
                allLocations.AddRange(locations);
            }
            var locationEntities = allLocations.Select(l =>Converters.ToEntity(l)).ToList();
            _db.Locations.AddRange(locationEntities);
            var a = await _db.SaveChangesAsync();

            var aa = allLocations;
        }

        private async Task<List<LocationDto>?> FetchLocationsAsync(int page)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                PropertyNameCaseInsensitive = true
            };
            var response = await _http.GetAsync($"location?page={page}");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<LocationDto>>(jsonString, options);
                return apiResponse?.Results ?? new List<LocationDto>();
            }
            return new List<LocationDto>();
        }

    }
}
