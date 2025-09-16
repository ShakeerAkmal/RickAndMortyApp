using Microsoft.AspNetCore.Mvc;
using RickAndMorty.Services.Interfaces;
using RickAndMorty.Services.Dtos;

namespace RickAndMorty.WebApp.Controllers
{
    public class CharactersViewController : Controller
    {
        private readonly ICharacterService _characterService;

        public CharactersViewController(ICharacterService characterService)
        {
            _characterService = characterService;
        }

        // Razor Page endpoint - returns HTML view
        [Route("characters/from/{planetName}")]
        [HttpGet]
        public async Task<IActionResult> FromPlanet(string planetName, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(planetName))
            {
                return NotFound("Planet name is required");
            }

            try
            {
                var characters = await _characterService.GetCharactersByLocationAsync(planetName, cancellationToken);
                ViewData["PlanetName"] = planetName;
                return View("CharactersByPlanet", characters);
            }
            catch (Exception)
            {
                TempData["Error"] = $"Unable to load characters from {planetName}. Please try again.";
                return View("CharactersByPlanet", new List<CharacterDto>());
            }
        }
    }

   
}
