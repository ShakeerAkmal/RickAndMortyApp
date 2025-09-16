using Microsoft.AspNetCore.Mvc;
using RickAndMorty.Services.Interfaces;
using RickAndMorty.Services.Dtos;
using RickAndMorty.Services.Converter;
namespace RickAndMorty.WebApp.Controllers
{

    [Route("characters")]
    public class CharactersViewController : Controller
    {
        private readonly ICharacterService _characterService;

        public CharactersViewController(ICharacterService characterService)
        {
            _characterService = characterService;
        }

        // GET: /characters/from/Earth (C-137)
        [HttpGet("from/{planetName}")]
        public async Task<IActionResult> FromPlanet(string planetName)
        {
            var characters = await _characterService.GetCharactersByLocationAsync(planetName);
            return View("CharactersByPlanet", characters);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var (characters, fromDb) = await _characterService.GetCharactersAsync();
            Response.Headers["from-database"] = fromDb.ToString();
            return Ok(characters);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCharacterDto createCharacterDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
             var id = await _characterService.CreateCharacter(createCharacterDto);
            return Ok(id);
        }

        
    }

}
