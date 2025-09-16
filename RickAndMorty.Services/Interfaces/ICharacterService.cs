using RickAndMorty.Services.Dtos;

namespace RickAndMorty.Services.Interfaces
{
      public interface ICharacterService
    {
        Task<int> CreateCharacter(CreateCharacterDto createCharacterDto, CancellationToken cancellationToken = default);
        Task FetchAndSaveAliveCharactersAsync(CancellationToken cancellationToken = default);
        Task<(List<CharacterDto> Characters, bool FromDatabase)> GetCharactersAsync(CancellationToken cancellationToken = default);
        Task<List<CharacterDto>> GetCharactersByLocationAsync(string locationName, CancellationToken cancellationToken = default);
    }

}
