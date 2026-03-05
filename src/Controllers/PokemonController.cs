using API.Dtos;
using Microsoft.AspNetCore.Mvc;
using API.Services;

namespace API.Controllers
{
    [Route("PokeDex/[controller]")]
    [ApiController]
    public class PokemonController : ControllerBase // Controller class handles incoming HTTP requests, and uses pokemonservice to perform operations on the pokemon data (business logic), and returns appropriate HTTP responses
    {
        private readonly IPokemonService _pokemonService;
        public PokemonController(IPokemonService pokemonService)
        {
            _pokemonService = pokemonService;
        }

        [HttpGet("PokeDex/All")]
        public async Task<IActionResult> GetAllPokemonAsync()
        {
            var pokemonList = await _pokemonService.GetAllPokemonAsync();
            return Ok(pokemonList);
        }

        [HttpGet("PokeDex/GetPokemon/{id:int}")] // route constraint to ensure id is an integer
        public async Task<IActionResult> GetPokemonByIdAsync([FromRoute] int id)
        {
            var pokemon = await _pokemonService.GetPokemonByIdAsync(id);
            return Ok(pokemon);
        }

        [HttpPost("PokeDex/AddPokemon")]
        public async Task<IActionResult> AddPokemonAsync([FromBody] PokemonRequestDto pokemon)
        {
            await _pokemonService.AddPokemonAsync(pokemon);
            return Ok("Pokemon added successfully");
        }

        [HttpPut("PokeDex/UpdatePokemon/{id:int}")]
        public async Task<IActionResult> UpdatePokemonAsync([FromBody] PokemonUpdateRequestDto UpdateRequest, [FromRoute] int id)
        {
            await _pokemonService.UpdatePokemonAsync(UpdateRequest, id);
            return Ok("Pokemon updated successfully");
        }

        [HttpDelete("DeletePokemon/{id:int}")]
        public async Task<IActionResult> DeletePokemonAsync([FromRoute] int id)
        {
            await _pokemonService.DeletePokemonAsync(id);
            return Ok("Pokemon deleted successfully");
        }
    }
}