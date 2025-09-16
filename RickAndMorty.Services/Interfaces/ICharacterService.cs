using RickAndMorty.Services.Dtos;

namespace RickAndMorty.Services.Interfaces
{
    public interface ICharacterService
    {
        Task FetchAndSaveAliveCharactersAsync();
        Task<(List<CharacterDto> Characters, bool FromDatabase)> GetCharactersAsync();
        Task<List<CharacterDto>> GetCharactersByLocationAsync(string locationName);
    }

}
