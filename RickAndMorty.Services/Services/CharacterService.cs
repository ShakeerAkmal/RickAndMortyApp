using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RickAndMortyApp.Data;
using RickAndMortyApp.Data.Entities;
using RickAndMorty.Services.Interfaces;
using RickAndMortyApp.Data.Entities;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using RickAndMorty.Services.Dtos;
using System.ComponentModel.DataAnnotations;
using RickAndMorty.Services.Converter;

namespace RickAndMorty.Services.Services
{
    public class CharacterService : ICharacterService
    {
        private readonly HttpClient _http;
        private readonly RickAndMortyContext _db;

        public CharacterService(HttpClient http, RickAndMortyContext db)
        {
            _http = http;
            _db = db;
        }

        public async Task FetchAndSaveAliveCharactersAsync()
        {
            var episodes = await _db.Episodes.ToListAsync();
            var locations = await _db.Locations.ToListAsync();
            var allCharacters = new List<CharacterDto>();
            var characterEpisodesList = new List<CharacterEpisode>();
            for (int page = 1; ; page++)
            {
                var characters = await FetchCharactersAsync(page);
                if (characters.Count == 0)
                {
                    break;
                }
                foreach (var character in characters.Where(c => c.Status == "Alive").ToList())
                {
                    character.OriginId = locations.FirstOrDefault(l => l.Name.Equals(character.Origin.Name,StringComparison.OrdinalIgnoreCase))?.Id ?? 1;
                    character.CurrentLocationId = locations.FirstOrDefault(l => l.Name.Equals(character.Location.Name,StringComparison.OrdinalIgnoreCase))?.Id ?? 1;

                    var characterEpisodes = await MapEpisodesAsync(episodes, character);
                    if (characterEpisodes != null)
                    {
                        characterEpisodesList.AddRange(characterEpisodes);
                    }
                    allCharacters.Add(character);
                }
            }

            var characterEntities = allCharacters.Select(e => Converters.ToEntity(e)).ToList();
            _db.Characters.AddRange(characterEntities);
            _db.CharacterEpisodes.AddRange(characterEpisodesList);
            await _db.SaveChangesAsync();

        }

        private async Task<List<CharacterDto>?> FetchCharactersAsync(int page)
        {
            var response = await _http.GetAsync($"character?page={page}");
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse>();
                return apiResponse?.Results ?? new List<CharacterDto>();
            }
            return new List<CharacterDto>();
        }

        private async Task<List<CharacterEpisode>?> MapEpisodesAsync(List<Episode> episodes, CharacterDto character)
        {
            var characterEpisodes = new List<CharacterEpisode>();
            foreach (var episodeUrl in character.Episode)
            {
                var episodeIdStr = episodeUrl.Split('/').Last();
                if (int.TryParse(episodeIdStr, out int episodeId))
                {
                    var episode = await _db.Episodes.FindAsync(episodeId);
                    if (episode != null)
                    {
                        characterEpisodes.Add(new CharacterEpisode
                        {
                            CharacterId = character.Id,
                            EpisodeId = episode.Id
                        });
                    }
                }
            }
            return characterEpisodes;
        }

        //public Task<List<CharacterDto>> GetCharactersAsync() =>
        //    _db.Characters.ToListAsync();

        private record ApiResponse(List<CharacterDto> Results);
    }

}
