using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RickAndMortyApp.Data;
using RickAndMortyApp.Data.Entities;
using RickAndMorty.Services.Interfaces;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using RickAndMorty.Services.Dtos;
using RickAndMorty.Services.Converter;

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
            var response = await _http.GetAsync($"episode?page={page}");
            if (response.IsSuccessStatusCode)
            {
                var cc = await response.Content.ReadAsStringAsync();
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse>();
                return apiResponse?.Results ?? new List<EpisodeDto>();
            }
            return new List<EpisodeDto>();
        }

        private record ApiResponse(List<EpisodeDto> Results);
    }
}
