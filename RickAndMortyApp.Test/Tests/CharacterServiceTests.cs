using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using RickAndMorty.Services.Dtos;
using RickAndMorty.Services.Services;
using RickAndMortyApp.Data;
using RickAndMortyApp.Data.Entities;
using RickAndMortyApp.Test.Helpers;

namespace RickAndMorty.Tests
{
    public class CharacterServiceTests
    {
        private IMemoryCache GetMemoryCache()
        {
            return new MemoryCache(new MemoryCacheOptions());
        }

        [Fact]
        public async Task CreateCharacter_ValidData_ReturnsCharacterId()
        {
            // Arrange
            var dbContext = DbContextHelper.GetInMemoryDb();
            var cache = GetMemoryCache();
            var httpClient = HttpClientHelper.GetMockHttpClient("{}");

            var service = new CharacterService(httpClient, dbContext, cache);

            var location1 = new Location { Id = 1, Name = "Earth", Type = "Planet", Dimension = "C-137", Created = DateTime.UtcNow };
            var location2 = new Location { Id = 2, Name = "Mars", Type = "Planet", Dimension = "C-137", Created = DateTime.UtcNow };
            var episode1 = new Episode { Id = 1, Name = "Pilot", Created = DateTime.UtcNow };
            var episode2 = new Episode { Id = 2, Name = "Lawnmower Dog", Created = DateTime.UtcNow };

            dbContext.Locations.AddRange(location1, location2);
            dbContext.Episodes.AddRange(episode1, episode2);
            await dbContext.SaveChangesAsync();

            var createCharacterDto = new CreateCharacterDto
            {
                Name = "Test Character",
                Status = "Alive",
                Species = "Human",
                Type = "Test Type",
                Gender = "Male",
                Image = "test.jpg",
                OriginId = 1,
                CurrentLocationId = 2,
                EpisodeIds = new List<int> { 1, 2 }
            };

            // Act
            var result = await service.CreateCharacter(createCharacterDto);

            // Assert
            Assert.True(result > 0);
            var character = await dbContext.Characters.FindAsync(result);
            Assert.NotNull(character);
            Assert.Equal("Test Character", character.Name);
            Assert.Equal("Alive", character.Status);
            Assert.Equal(1, character.OriginId);
            Assert.Equal(2, character.CurrentLocationId);

            var characterEpisodes = await dbContext.CharacterEpisodes
                .Where(ce => ce.CharacterId == result)
                .ToListAsync();
            Assert.Equal(2, characterEpisodes.Count);
        }

        [Fact]
        public async Task CreateCharacter_InvalidOriginLocationId_ThrowsArgumentException()
        {
            // Arrange
            var dbContext = DbContextHelper.GetInMemoryDb();
            var cache = GetMemoryCache();
            var httpClient = HttpClientHelper.GetMockHttpClient("{}");
           
            var service = new CharacterService(httpClient, dbContext, cache);

            var createCharacterDto = new CreateCharacterDto
            {
                Name = "Test Character",
                Status = "Alive",
                Species = "Human",
                Gender = "Male",
                Image = "test.jpg",
                OriginId = 999 // Invalid ID
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => service.CreateCharacter(createCharacterDto));
            Assert.Contains("Invalid origin location ID: 999", exception.Message);
        }

        [Fact]
        public async Task CreateCharacter_InvalidCurrentLocationId_ThrowsArgumentException()
        {
            // Arrange
            var dbContext = DbContextHelper.GetInMemoryDb();
            var cache = GetMemoryCache();
            var httpClient = HttpClientHelper.GetMockHttpClient("{}");
            
            var service = new CharacterService(httpClient, dbContext, cache);

            var createCharacterDto = new CreateCharacterDto
            {
                Name = "Test Character",
                Status = "Alive",
                Species = "Human",
                Gender = "Male",
                Image = "test.jpg",
                CurrentLocationId = 999 // Invalid ID
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => service.CreateCharacter(createCharacterDto));
            Assert.Contains("Invalid current location ID: 999", exception.Message);
        }

        [Fact]
        public async Task CreateCharacter_InvalidEpisodeIds_ThrowsArgumentException()
        {
            // Arrange
            var dbContext = DbContextHelper.GetInMemoryDb();
            var cache = GetMemoryCache();
            var httpClient = HttpClientHelper.GetMockHttpClient("{}");

            var service = new CharacterService(httpClient, dbContext, cache);

            var episode1 = new Episode { Id = 1, Name = "Pilot", Created = DateTime.UtcNow };
            dbContext.Episodes.Add(episode1);
            await dbContext.SaveChangesAsync();

            var createCharacterDto = new CreateCharacterDto
            {
                Name = "Test Character",
                Status = "Alive",
                Species = "Human",
                Gender = "Male",
                Image = "test.jpg",
                EpisodeIds = new List<int> { 1, 999, 888 } // 999 and 888 are invalid
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => service.CreateCharacter(createCharacterDto));
            Assert.Contains("Invalid episode IDs:", exception.Message);
            Assert.Contains("999", exception.Message);
            Assert.Contains("888", exception.Message);
        }

        [Fact]
        public async Task CreateCharacter_WithoutOptionalFields_ReturnsCharacterId()
        {
            // Arrange
            var dbContext = DbContextHelper.GetInMemoryDb();
            var cache = GetMemoryCache();
            var httpClient = HttpClientHelper.GetMockHttpClient("{}");
            
            var service = new CharacterService(httpClient, dbContext, cache);

            var createCharacterDto = new CreateCharacterDto
            {
                Name = "Simple Character",
                Status = "Alive",
                Species = "Human",
                Gender = "Female",
                Image = "simple.jpg"
                // No OriginId, CurrentLocationId, or EpisodeIds
            };

            // Act
            var result = await service.CreateCharacter(createCharacterDto);

            // Assert
            Assert.True(result > 0);
            var character = await dbContext.Characters.FindAsync(result);
            Assert.NotNull(character);
            Assert.Equal("Simple Character", character.Name);
            Assert.Null(character.OriginId);
            Assert.Null(character.CurrentLocationId);

            var characterEpisodes = await dbContext.CharacterEpisodes
                .Where(ce => ce.CharacterId == result)
                .ToListAsync();
            Assert.Empty(characterEpisodes);
        }

        [Fact]
        public async Task CreateCharacter_ClearsCache()
        {
            // Arrange
            var dbContext = DbContextHelper.GetInMemoryDb();
            var cache = GetMemoryCache();
            var httpClient = HttpClientHelper.GetMockHttpClient("{}");

            var service = new CharacterService(httpClient, dbContext, cache);

            // Add something to cache first
            var cacheKey = "CharacterList";
            var cachedData = new List<CharacterDto>();
            cache.Set(cacheKey, cachedData);

            var createCharacterDto = new CreateCharacterDto
            {
                Name = "Cache Test Character",
                Status = "Alive",
                Species = "Human",
                Gender = "Male",
                Image = "cache_test.jpg"
            };

            // Verify cache has data before
            Assert.True(cache.TryGetValue(cacheKey, out _));

            // Act
            await service.CreateCharacter(createCharacterDto);

            // Assert
            Assert.False(cache.TryGetValue(cacheKey, out _));
        }

        [Fact]
        public async Task CreateCharacter_EmptyEpisodeIds_DoesNotCreateCharacterEpisodes()
        {
            // Arrange
            var dbContext = DbContextHelper.GetInMemoryDb();
            var cache = GetMemoryCache();
            var httpClient = HttpClientHelper.GetMockHttpClient("{}");
            
            var service = new CharacterService(httpClient, dbContext, cache);

            var createCharacterDto = new CreateCharacterDto
            {
                Name = "No Episodes Character",
                Status = "Alive",
                Species = "Human",
                Gender = "Male",
                Image = "no_episodes.jpg",
                EpisodeIds = new List<int>() // Empty list
            };

            // Act
            var result = await service.CreateCharacter(createCharacterDto);

            // Assert
            Assert.True(result > 0);
            var characterEpisodes = await dbContext.CharacterEpisodes
                .Where(ce => ce.CharacterId == result)
                .ToListAsync();
            Assert.Empty(characterEpisodes);
        }
        [Fact]
        public async Task FetchAndSaveCharactersAsync_SavesCharactersToDb()
        {
            // Arrange
            var locations = new List<Location>
            {
                new Location
                {
                    Id = 1,
                    Name = "Earth (C-137)",
                    Type = "Planet",
                    Url = "https://rickandmortyapi.com/api/location/1",
                    Dimension = "Dimension C-137",
                    Created = DateTime.Parse("2017-11-10T12:42:04.162Z")
                },
                new Location
                {
                    Id = 2,
                    Name = "Citadel of Ricks",
                    Type = "Space station",
                    Url = "https://rickandmortyapi.com/api/location/2",
                    Dimension = "unknown",
                    Created = DateTime.Parse("2017-11-10T12:42:04.162Z")
                }
            };
            var episodes = new List<Episode>
            {
                new Episode
                {
                    Id = 1,
                    Name = "Pilot",
                    AirDate = "December 2, 2013",
                    EpisodeCode = "S01E01",
                    Url = "https://rickandmortyapi.com/api/episode/1",
                    Created = DateTime.Parse("2017-11-10T12:56:33.798Z")
                },
                new Episode
                {
                    Id = 2,
                    Name = "Pilot2",
                    AirDate = "December 2, 2013",
                    EpisodeCode = "S01E01",
                    Url = "https://rickandmortyapi.com/api/episode/1",
                    Created = DateTime.Parse("2017-11-10T12:56:33.798Z")
                }
            };


            var dbContext = DbContextHelper.GetInMemoryDb();
            dbContext.Locations.AddRange(locations);
            dbContext.Episodes.AddRange(episodes);
            await dbContext.SaveChangesAsync();

            var json = @"{""info"":{""count"":826,""pages"":42,""next"":""https://rickandmortyapi.com/api/character?page=2"",""prev"":null},""results"":[{""id"":1,""name"":""Rick Sanchez"",""status"":""Alive"",""species"":""Human"",""type"":"""",""gender"":""Male"",""origin"":{""name"":""Earth (C-137)"",""url"":""https://rickandmortyapi.com/api/location/1""},""location"":{""name"":""Earth (C-137)"",""url"":""https://rickandmortyapi.com/api/location/1""},""image"":""https://rickandmortyapi.com/api/character/avatar/1.jpeg"",""episode"":[""https://rickandmortyapi.com/api/episode/1""],""url"":""https://rickandmortyapi.com/api/character/1"",""created"":""2017-11-04T18:48:46.250Z""},{""id"":2,""name"":""Morty Smith"",""status"":""Alive"",""species"":""Human"",""type"":"""",""gender"":""Male"",""origin"":{""name"":""unknown"",""url"":""""},""location"":{""name"":""Citadel of Ricks"",""url"":""https://rickandmortyapi.com/api/location/2""},""image"":""https://rickandmortyapi.com/api/character/avatar/2.jpeg"",""episode"":[""https://rickandmortyapi.com/api/episode/1"",""https://rickandmortyapi.com/api/episode/2""],""url"":""https://rickandmortyapi.com/api/character/2"",""created"":""2017-11-04T18:50:21.651Z""},{""id"":3,""name"":""Summer Smith"",""status"":""Alive"",""species"":""Human"",""type"":"""",""gender"":""Female"",""origin"":{""name"":""Earth (C-137)"",""url"":""https://rickandmortyapi.com/api/location/1""},""location"":{""name"":""Citadel of Ricks"",""url"":""https://rickandmortyapi.com/api/location/2""},""image"":""https://rickandmortyapi.com/api/character/avatar/3.jpeg"",""episode"":[""https://rickandmortyapi.com/api/episode/1"",""https://rickandmortyapi.com/api/episode/2""],""url"":""https://rickandmortyapi.com/api/character/3"",""created"":""2017-11-04T19:09:56.428Z""},{""id"":4,""name"":""Beth Smith"",""status"":""Alive"",""species"":""Human"",""type"":"""",""gender"":""Female"",""origin"":{""name"":""Earth (C-137)"",""url"":""https://rickandmortyapi.com/api/location/1""},""location"":{""name"":""Citadel of Ricks"",""url"":""https://rickandmortyapi.com/api/location/2""},""image"":""https://rickandmortyapi.com/api/character/avatar/4.jpeg"",""episode"":[""https://rickandmortyapi.com/api/episode/2""],""url"":""https://rickandmortyapi.com/api/character/4"",""created"":""2017-11-04T19:22:43.665Z""}]}";
            var httpClient = HttpClientHelper.GetMockHttpClient(json);
            var memoryCache = new MemoryCache(new MemoryCacheOptions());

            var service = new CharacterService(httpClient, dbContext, memoryCache);

            // Act
            await service.FetchAndSaveAliveCharactersAsync();

            // Assert
            var characters = await dbContext.Characters
                .Include(c => c.Origin)
                .Include(c => c.Location)
                .Include(c => c.CharacterEpisodes)
                    .ThenInclude(ce => ce.Episode)
                .ToListAsync();

            // Assert character count
            Assert.Equal(4, characters.Count);

            // Assert first character (Rick) details
            var rick = characters.OrderBy(a => a.Id).FirstOrDefault();
            Assert.NotNull(rick);
            Assert.Equal(1, rick.Id);
            Assert.Equal("Rick Sanchez", rick.Name);
            Assert.Equal("Alive", rick.Status);
            Assert.Equal("Human", rick.Species);
            Assert.Equal("Male", rick.Gender);

            // Assert Rick's location relationships
            Assert.NotNull(rick.Origin);
            Assert.Equal("Earth (C-137)", rick.Origin.Name);
            Assert.NotNull(rick.Location);
            Assert.Equal("Earth (C-137)", rick.Location.Name);

            // Assert Rick's episode relationship
            Assert.Single(rick.CharacterEpisodes);
            Assert.Equal(1, rick.CharacterEpisodes.First().Episode.Id);
            Assert.Equal("Pilot", rick.CharacterEpisodes.First().Episode.Name);

            // Assert other characters
            var morty = characters.FirstOrDefault(c => c.Id == 2);
            Assert.NotNull(morty);
            Assert.Equal("Morty Smith", morty.Name);
            Assert.Equal(2, morty.CharacterEpisodes.Count);
            Assert.Equal("Citadel of Ricks", morty.Location.Name);

            // Verify Beth has only one episode
            var beth = characters.FirstOrDefault(c => c.Id == 4);
            Assert.NotNull(beth);
            Assert.Single(beth.CharacterEpisodes);
            Assert.Equal(2, beth.CharacterEpisodes.First().Episode.Id);
        }
    }
}
