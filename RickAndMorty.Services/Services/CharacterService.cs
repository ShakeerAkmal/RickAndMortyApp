using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using RickAndMorty.Services.Converter;
using RickAndMorty.Services.Dtos;
using RickAndMorty.Services.Interfaces;
using RickAndMortyApp.Data;
using RickAndMortyApp.Data.Entities;
using System.Text.Json;

namespace RickAndMorty.Services.Services
{
    public class CharacterService : ICharacterService
    {
        private readonly HttpClient _http;
        private readonly RickAndMortyContext _db;
        private readonly IMemoryCache _cache;

        private const string CacheKey = "CharacterList";
        private static readonly TimeSpan RefreshInterval = TimeSpan.FromMinutes(5);

        public CharacterService(HttpClient http, RickAndMortyContext db, IMemoryCache cache)
        {
            _http = http;
            _db = db;
            _cache = cache;
        }
        public async Task<(List<CharacterDto> Characters, bool FromDatabase)> GetCharactersAsync()
        {
            if (_cache.TryGetValue(CacheKey, out List<CharacterDto> cachedCharacters))
            {
                return (cachedCharacters, false);
            }

            var dbCharacters = await _db.Characters
                                    .Include(a => a.Location)
                                    .Include(a => a.CharacterEpisodes)
                                    .ThenInclude(a => a.Episode)
                                    .ToListAsync();

            if (dbCharacters.Any())
            {
                var dbCharacterDtos = dbCharacters.Select(Converters.ToDto).ToList();
                _cache.Set(CacheKey, dbCharacterDtos, RefreshInterval);
                return (dbCharacterDtos, true);
            }
            return (new List<CharacterDto>(), true);

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
                    character.OriginId = locations.FirstOrDefault(l => l.Name.Equals(character?.Origin?.Name, StringComparison.OrdinalIgnoreCase))?.Id;
                    character.CurrentLocationId = locations.FirstOrDefault(l => l.Name.Equals(character?.Location?.Name, StringComparison.OrdinalIgnoreCase))?.Id;

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
        public async Task<List<CharacterDto>> GetCharactersByLocationAsync(string locationName)
        {
            var characters = _db.Characters
                            .Where(c => c.Location != null && c.Location.Name == locationName)
                            .Include(a => a.Location)
                            .Select(Converters.ToDto).ToList();
            return characters;
        }

        public async Task<int> CreateCharacter(CreateCharacterDto createCharacterDto)
        {
            var validLocationIds = await _db.Locations
                .Where(l => l.Id == createCharacterDto.OriginId || l.Id == createCharacterDto.CurrentLocationId)
                .Select(l => l.Id)
                .ToListAsync();

            if (createCharacterDto.OriginId.HasValue && !validLocationIds.Contains(createCharacterDto.OriginId.Value))
            {
                throw new ArgumentException($"Invalid origin location ID: {createCharacterDto.OriginId}");
            }

            if (createCharacterDto.CurrentLocationId.HasValue && !validLocationIds.Contains(createCharacterDto.CurrentLocationId.Value))
            {
                throw new ArgumentException($"Invalid current location ID: {createCharacterDto.CurrentLocationId}");
            }

            if (createCharacterDto.EpisodeIds?.Count > 0)
            {
                var validEpisodeIds = await _db.Episodes
                    .Where(e => createCharacterDto.EpisodeIds.Contains(e.Id))
                    .Select(e => e.Id)
                    .ToListAsync();

                var invalidEpisodeIds = createCharacterDto.EpisodeIds.Except(validEpisodeIds).ToList();
                if (invalidEpisodeIds.Any())
                {
                    throw new ArgumentException($"Invalid episode IDs: {string.Join(", ", invalidEpisodeIds)}");
                }
            }

            var maxId = await _db.Characters.MaxAsync(c => (int?)c.Id) ?? 0;
            var character = new Character
            {
                Id = maxId + 1,
                Name = createCharacterDto.Name,
                Status = createCharacterDto.Status,
                Species = createCharacterDto.Species,
                Type = createCharacterDto.Type,
                Gender = createCharacterDto.Gender,
                Image = createCharacterDto.Image,
                Created = DateTime.UtcNow,
                OriginId = createCharacterDto.OriginId,
                CurrentLocationId = createCharacterDto.CurrentLocationId
            };

            var entityEntry = _db.Characters.Add(character);

            if (createCharacterDto.EpisodeIds?.Count > 0)
            {
                foreach (var episodeId in createCharacterDto.EpisodeIds)
                {
                    var characterEpisode = new CharacterEpisode
                    {
                        CharacterId = character.Id,
                        EpisodeId = episodeId
                    };
                    _db.CharacterEpisodes.Add(characterEpisode);
                }
            }

            await _db.SaveChangesAsync();
            _cache.Remove(CacheKey);

            return entityEntry.Entity.Id;
        }

        private async Task<List<CharacterDto>?> FetchCharactersAsync(int page)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                PropertyNameCaseInsensitive = true
            };

            var response = await _http.GetAsync($"character?page={page}");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<CharacterDto>>(jsonString, options);
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


    }

}
