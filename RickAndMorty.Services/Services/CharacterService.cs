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
           var allCharacters = new List<ApiCharacter>();
            for (int page = 1; ; page++)
            {
                var characters = await FetchCharactersAsync(page);
                if (characters.Count == 0)
                {
                    break;
                }
                allCharacters.AddRange(characters);
            }

            var alive = allCharacters.Where(c => c.Status == "Alive").ToList();

            //_db.Characters.RemoveRange(_db.Characters);
            //await _db.SaveChangesAsync();

            //_db.Characters.AddRange(alive.Select(c => new Character
            //{
            //    Id = c.Id,
            //    Name = c.Name,
            //    Status = c.Status,
            //    Species = c.Species
            //}));
            //await _db.SaveChangesAsync();
        }

        private async Task<List<ApiCharacter>?> FetchCharactersAsync(int page)
        {
            var response = await _http.GetAsync($"character?page={page}");
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse>();
                return apiResponse?.Results ?? new List<ApiCharacter>();
            }
            return new List<ApiCharacter>();
        }

        public Task<List<CharacterDto>> GetCharactersAsync() =>
            _db.Characters.ToListAsync();

        private record ApiResponse(List<ApiCharacter> Results);
        private record ApiCharacter(int Id, string Name, string Status, string Species);
    }

}
