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
            var response = await _http.GetAsync($"location?page={page}");
            if (response.IsSuccessStatusCode)
            {
                var cc = await response.Content.ReadAsStringAsync();
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse>();
                return apiResponse?.Results ?? new List<LocationDto>();
            }
            return new List<LocationDto>();
        }

        private record ApiResponse(List<LocationDto> Results);
    }
}
