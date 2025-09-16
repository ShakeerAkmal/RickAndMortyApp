using Microsoft.EntityFrameworkCore;
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

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            PropertyNameCaseInsensitive = true
        };

        public EpisodeService(HttpClient http, RickAndMortyContext db)
        {
            _http = http;
            _db = db;
        }

        public async Task FetchAndSaveEpisodesAsync(CancellationToken cancellationToken = default)
        {
            var allEpisodes = new List<EpisodeDto>();

            for (int page = 1; ; page++)
            {
                var episodes = await FetchEpisodesAsync(page, cancellationToken);
                if (episodes == null || episodes.Count == 0)
                {
                    break;
                }
                allEpisodes.AddRange(episodes);
            }

            if (allEpisodes?.Count > 0)
            {
                await SaveEpisodesWithTransactionAsync(allEpisodes, cancellationToken);
            }
        }

        private async Task SaveEpisodesWithTransactionAsync(
            List<EpisodeDto> episodes,
            CancellationToken cancellationToken)
        {
            using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var episodeEntities = episodes.Select(Converters.ToEntity).ToList();
                _db.Episodes.AddRange(episodeEntities);

                await _db.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        private async Task<List<EpisodeDto>?> FetchEpisodesAsync(int page, CancellationToken cancellationToken)
        {
            var response = await _http.GetAsync($"episode?page={page}", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return new List<EpisodeDto>();
            }

            var jsonString = await response.Content.ReadAsStringAsync(cancellationToken);
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<EpisodeDto>>(jsonString, JsonOptions);
            return apiResponse?.Results ?? new List<EpisodeDto>();
        }
    }
}
