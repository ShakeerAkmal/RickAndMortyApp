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

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            PropertyNameCaseInsensitive = true
        };

        public CharacterService(HttpClient http, RickAndMortyContext db, IMemoryCache cache)
        {
            _http = http;
            _db = db;
            _cache = cache;
        }
        public async Task<(List<CharacterDto> Characters, bool FromDatabase)> GetCharactersAsync(CancellationToken cancellationToken = default)
        {
            if (_cache.TryGetValue(CacheKey, out List<CharacterDto> cachedCharacters))
            {
                return (cachedCharacters, false);
            }

            var dbCharacters = await _db.Characters
                                    .Include(a => a.Location)
                                    .Include(a => a.CharacterEpisodes)
                                    .ThenInclude(a => a.Episode)
                                    .AsNoTracking()
                                    .ToListAsync(cancellationToken);

            if (dbCharacters.Any())
            {
                var dbCharacterDtos = dbCharacters.Select(Converters.ToDto).ToList();
                _cache.Set(CacheKey, dbCharacterDtos, RefreshInterval);
                return (dbCharacterDtos, true);
            }
            return (new List<CharacterDto>(), true);

        }

        public async Task FetchAndSaveAliveCharactersAsync(CancellationToken cancellationToken = default)
        {
            var episodes = await _db.Episodes.AsNoTracking().ToListAsync(cancellationToken);
            var locations = await _db.Locations.AsNoTracking().ToListAsync(cancellationToken);

            var locationLookup = locations.ToDictionary(
                 l => l.Name,
                 l => l.Id,
                 StringComparer.OrdinalIgnoreCase);

            var allCharacters = new List<CharacterDto>();
            var characterEpisodesList = new List<CharacterEpisode>();

            for (int page = 1; ; page++)
            {
                var characters = await FetchCharactersAsync(page);
                if (characters.Count == 0)
                {
                    break;
                }
                var aliveCharacters = characters.Where(c => c.Status == "Alive");

                foreach (var character in aliveCharacters)
                {
                    character.OriginId = GetLocationId(locationLookup, character?.Origin?.Name);
                    character.CurrentLocationId = GetLocationId(locationLookup, character?.Location?.Name);

                    var characterEpisodes = MapEpisodesAsync(episodes, character);
                    if (characterEpisodes.Any())
                    {
                        characterEpisodesList.AddRange(characterEpisodes);
                    }
                    allCharacters.Add(character);
                }
            }

            if (allCharacters.Any())
            {
                await SaveCharactersWithTransactionAsync(allCharacters, characterEpisodesList, cancellationToken);
            }

        }
        public async Task<List<CharacterDto>> GetCharactersByLocationAsync(string locationName, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(locationName))
                throw new ArgumentException("Location name cannot be null or empty", nameof(locationName));

            var characters = await _db.Characters
                    .Where(c => c.Origin != null && c.Origin.Name == locationName)
                    .Include(a => a.Origin)
                    .AsNoTracking()
                    .Select(c => Converters.ToDto(c))
                    .ToListAsync(cancellationToken);

            return characters;
        }

        public async Task<int> CreateCharacter(CreateCharacterDto createCharacterDto, CancellationToken cancellationToken = default)
        {

            if (createCharacterDto == null)
                throw new ArgumentNullException(nameof(createCharacterDto));

            using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                await ValidateCharacterDataAsync(createCharacterDto, cancellationToken);

                var maxId = await _db.Characters.MaxAsync(c => (int?)c.Id, cancellationToken) ?? 0;
                var character = CreateCharacterEntity(createCharacterDto, maxId + 1);

                _db.Characters.Add(character);

                if (createCharacterDto.EpisodeIds?.Any() == true)
                {
                    var characterEpisodes = createCharacterDto.EpisodeIds.Select(episodeId =>
                        new CharacterEpisode { CharacterId = character.Id, EpisodeId = episodeId });
                    _db.CharacterEpisodes.AddRange(characterEpisodes);
                }

                await _db.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                _cache.Remove(CacheKey);
                return character.Id;
            }
            catch(Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        private async Task ValidateCharacterDataAsync(CreateCharacterDto dto, CancellationToken cancellationToken)
        {
            var locationIds = new List<int>();
            if (dto.OriginId.HasValue) locationIds.Add(dto.OriginId.Value);
            if (dto.CurrentLocationId.HasValue) locationIds.Add(dto.CurrentLocationId.Value);

            var episodeIds = dto.EpisodeIds?.ToList() ?? new List<int>();

            var validLocationIds = locationIds.Any() ?
                await _db.Locations.Where(l => locationIds.Contains(l.Id)).Select(l => l.Id).ToListAsync(cancellationToken) :
                new List<int>();

            var validEpisodeIds = episodeIds.Any() ?
                await _db.Episodes.Where(e => episodeIds.Contains(e.Id)).Select(e => e.Id).ToListAsync(cancellationToken) :
                new List<int>();

            if (dto.OriginId.HasValue && !validLocationIds.Contains(dto.OriginId.Value))
                throw new ArgumentException($"Invalid origin location ID: {dto.OriginId}");

            if (dto.CurrentLocationId.HasValue && !validLocationIds.Contains(dto.CurrentLocationId.Value))
                throw new ArgumentException($"Invalid current location ID: {dto.CurrentLocationId}");

            var invalidEpisodeIds = episodeIds.Except(validEpisodeIds).ToList();
            if (invalidEpisodeIds.Any())
                throw new ArgumentException($"Invalid episode IDs: {string.Join(", ", invalidEpisodeIds)}");
        }
        private async Task<List<CharacterDto>?> FetchCharactersAsync(int page)
        {
            var response = await _http.GetAsync($"character?page={page}");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<CharacterDto>>(jsonString, JsonOptions);
                return apiResponse?.Results ?? new List<CharacterDto>();
            }
            return new List<CharacterDto>();
        }
        private static int? GetLocationId(Dictionary<string, int> locationLookup, string locationName)
        {
            return string.IsNullOrEmpty(locationName) ? null :
                   locationLookup.TryGetValue(locationName, out var id) ? id : null;
        }
        private List<CharacterEpisode> MapEpisodesAsync(List<Episode> episodes, CharacterDto character)
        {
            var characterEpisodes = new List<CharacterEpisode>();

            if (character.Episode?.Any() != true)
                return characterEpisodes;

            foreach (var episodeUrl in character.Episode)
            {
                var episodeIdStr = episodeUrl.Split('/').Last();
                if (int.TryParse(episodeIdStr, out int episodeId))
                {
                    var episode = episodes.FirstOrDefault(e => e.Id == episodeId);
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
        private async Task SaveCharactersWithTransactionAsync(
            List<CharacterDto> characters,
            List<CharacterEpisode> characterEpisodes,
            CancellationToken cancellationToken)
        {
            using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var characterEntities = characters.Select(Converters.ToEntity).ToList();

                _db.Characters.AddRange(characterEntities);
                _db.CharacterEpisodes.AddRange(characterEpisodes);

                await _db.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                _cache.Remove(CacheKey); 
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        private Character CreateCharacterEntity(CreateCharacterDto dto, int id)
        {
            return new Character
            {
                Id = id,
                Name = dto.Name,
                Status = dto.Status,
                Species = dto.Species,
                Type = dto.Type,
                Gender = dto.Gender,
                Image = dto.Image,
                OriginId = dto.OriginId,
                CurrentLocationId = dto.CurrentLocationId
            };
        }
    }
}
