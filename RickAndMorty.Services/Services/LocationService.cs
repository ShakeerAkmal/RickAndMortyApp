using Microsoft.EntityFrameworkCore;
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

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            PropertyNameCaseInsensitive = true
        };

        public LocationService(HttpClient http, RickAndMortyContext db)
        {
            _http = http;
            _db = db;
        }

        public async Task DeleteAllData(CancellationToken cancellationToken = default)
        {
            using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                await _db.CharacterEpisodes.ExecuteDeleteAsync(cancellationToken);
                await _db.Episodes.ExecuteDeleteAsync(cancellationToken);
                await _db.Characters.ExecuteDeleteAsync(cancellationToken);
                await _db.Locations.ExecuteDeleteAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public async Task FetchAndSaveLocationsAsync(CancellationToken cancellationToken = default)
        {
            var allLocations = new List<LocationDto>();

            for (int page = 1; ; page++)
            {
                var locations = await FetchLocationsAsync(page, cancellationToken);
                if (locations == null || locations.Count == 0)
                {
                    break;
                }
                allLocations.AddRange(locations);
            }

            if (allLocations.Count > 0)
            {
                await SaveLocationsWithTransactionAsync(allLocations, cancellationToken);
            }
        }

        private async Task SaveLocationsWithTransactionAsync(
            List<LocationDto> locations,
            CancellationToken cancellationToken)
        {
            using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var locationEntities = locations.Select(Converters.ToEntity).ToList();
                _db.Locations.AddRange(locationEntities);

                await _db.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        private async Task<List<LocationDto>?> FetchLocationsAsync(int page, CancellationToken cancellationToken)
        {
            var response = await _http.GetAsync($"location?page={page}", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return new List<LocationDto>();
            }

            var jsonString = await response.Content.ReadAsStringAsync(cancellationToken);
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<LocationDto>>(jsonString, JsonOptions);
            return apiResponse?.Results ?? new List<LocationDto>();
        }
    }
}
