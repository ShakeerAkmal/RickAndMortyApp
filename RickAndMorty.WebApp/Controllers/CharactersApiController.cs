using Azure;
using Microsoft.AspNetCore.Mvc;
using RickAndMorty.Services.Dtos;
using RickAndMorty.Services.Interfaces;

namespace RickAndMorty.WebApp.Controllers
{
    // Web API endpoints - return JSON
    [ApiController]
    [Route("api/characters")]
    public class CharactersApiController : ControllerBase
    {
        private readonly ICharacterService _characterService;

        public CharactersApiController(ICharacterService characterService)
        {
            _characterService = characterService;
        }

        /// <summary>
        /// Gets all characters from the service
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>A list of characters</returns>
        /// <response code="200">Returns the list of characters</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<CharacterDto>), 200)]
        public async Task<ActionResult<List<CharacterDto>>> Get(CancellationToken cancellationToken = default)
        {
            var (characters, fromDb) = await _characterService.GetCharactersAsync(cancellationToken);
            Response.Headers.Add("X-From-Database", fromDb.ToString());
            return Ok(characters);
        }

        /// <summary>
        /// Creates a new character
        /// </summary>
        /// <param name="createCharacterDto">Character data to create</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The ID of the created character</returns>
        /// <response code="201">Returns the ID of the newly created character</response>
        /// <response code="400">If the character data is invalid</response>
        [HttpPost]
        [ProducesResponseType(typeof(int), 201)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
        public async Task<ActionResult<int>> Create(
            [FromBody] CreateCharacterDto createCharacterDto,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var id = await _characterService.CreateCharacter(createCharacterDto, cancellationToken);
            return CreatedAtAction(nameof(Get), new { id }, id);
        }
    }
}
