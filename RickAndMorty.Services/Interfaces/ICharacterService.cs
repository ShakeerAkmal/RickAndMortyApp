using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RickAndMortyApp.Data.Entities;

namespace RickAndMorty.Services.Interfaces
{
    public interface ICharacterService
    {
        Task FetchAndSaveAliveCharactersAsync();
        Task<List<CharacterDto>> GetCharactersAsync();
    }

}
