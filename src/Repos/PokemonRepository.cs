using API.Data;
using API.Models;
using API.Dtos;
using API.Mappers;
using Microsoft.EntityFrameworkCore;
using API.HelperObjects;

namespace API.Repos
{
    public class PokemonRepository : IPokemonRepository // handles database interactions for pokemoncontroller
    {
        private readonly PokemonDBContext _context;

        public PokemonRepository(PokemonDBContext context)
        {
            _context = context;
        }

        public async Task<List<PokemonResponseDto>> GetAllPokemonAsync(QueryPokemonRequest query)
        {
            var pokemon = _context.Pokemon
            .Include(p => p.Abilities) // Include the related Abilities for each Pokemon
            .AsQueryable(); // primes query for filtering based on query parameters

            // Adding Filtering based on query parameters, allows clients to filter pokemon by using query parameters
            return await FilterPokemonAsync(pokemon, query);
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
        
#region Helper methods
        public async Task<bool> PokemonExistsAsync(string name)
        {
            return await _context.Pokemon.Where(p => p.Name == name).AnyAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<PokemonResponseDto>> FilterPokemonAsync(IQueryable<Pokemon> pokemon, QueryPokemonRequest query)
        {
            //TODO: Turn if statements into helper method
            if (!string.IsNullOrWhiteSpace(query.AbilityType))
            {
                pokemon = pokemon.Where(p => p.AbilityType.Contains(query.AbilityType));
            }
            if (query.Height.HasValue)
            {
                pokemon = pokemon.Where(p => p.Height == query.Height.Value);
            }
            if (query.Ability != null)
            {
                if (!string.IsNullOrWhiteSpace(query.Ability.AbilityType))
                {
                    pokemon = pokemon.Where(p => p.Abilities.Any(a => a.AbilityType == query.Ability.AbilityType));
                }
                if (query.Ability.Damage.HasValue)
                {
                    pokemon = pokemon.Where(p => p.Abilities.Any(a => a.Damage == query.Ability.Damage.Value));
                }
                if (!string.IsNullOrWhiteSpace(query.Ability.Name))
                {
                    pokemon = pokemon.Where(p => p.Abilities.Any(a => a.Name == query.Ability.Name));
                }
            }
            if (!string.IsNullOrEmpty(query.SortBy)) // Adding sorting based on query parameters, allows clients to sort pokemon by using query parameters
            {
                if (query.SortBy.Contains("Name", StringComparison.OrdinalIgnoreCase))
                {
                    pokemon = query.IsDescending ? pokemon = pokemon.OrderByDescending(p => p.Name) : pokemon.OrderBy(p => p.Name);
                }    
                else if (query.SortBy.Contains("AbilityType", StringComparison.OrdinalIgnoreCase))
                {
                    pokemon = query.IsDescending ? pokemon = pokemon.OrderByDescending(p => p.AbilityType) : pokemon.OrderBy(p => p.AbilityType);
                }
            }

            // Adding pagination based on query parameters for GetAllPokemon endpoint, allows clients to paginate pokemon by using query parameters, allows for more efficient data retrieval and improved performance when dealing with large datasets
            pokemon = pokemon.Skip((query.PageNumber.GetValueOrDefault() - 1) * query.PageSize.GetValueOrDefault()).Take(query.PageSize.GetValueOrDefault());

            return await pokemon.Select(pokemon => pokemon.MapToPokemonResponseDto()).ToListAsync();
        }
#endregion Helper methods
    }
}