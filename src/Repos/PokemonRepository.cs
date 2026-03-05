using API.Data;
using API.Models;
using API.Dtos;
using API.Mappers;
using Microsoft.EntityFrameworkCore;

namespace API.Repos
{
    public class PokemonRepository : IPokemonRepository // handles database interactions for  pokemoncontroller
    {
        private readonly PokemonDBContext _context;

        public PokemonRepository(PokemonDBContext context)
        {
            _context = context;
        }

        public async Task<List<PokemonResponseDto>> GetAllPokemonAsync()
        {
            var pokemonList = await _context.Pokemon
            .Include(p => p.Abilities) // Include the related Abilities for each Pokemon
            .Select(p => p.MapToPokemonResponseDto())
            .ToListAsync();

            return pokemonList;
        }

        public async Task<Pokemon> GetPokemonAsync(int id)
        {
            var pokemon = await _context.Pokemon
            .Include(p => p.Abilities)
            .FirstOrDefaultAsync(p => p.Id == id);

            return pokemon ?? throw new KeyNotFoundException("Pokemon with this ID does not exist.");
        }

        public async Task AddPokemonAsync(PokemonRequestDto pokemon)
        {
            await _context.Pokemon.AddAsync(pokemon.MapToPokemonModel());
            await SaveChangesAsync();
        }

        public async Task DeletePokemonAsync(Pokemon pokemon)
        {
            _context.Pokemon.Remove(pokemon);
            await SaveChangesAsync();
        }
        // Helper methods
        public async Task<bool> PokemonExistsAsync(string name)
        {
            return await _context.Pokemon.Where(p => p.Name == name).AnyAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}