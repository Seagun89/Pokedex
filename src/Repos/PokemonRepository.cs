using API.Data;
using API.Models;
using API.Dtos;
using API.Mappers;
using Microsoft.EntityFrameworkCore;
using API.HelperObjects;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace API.Repos
{
    public class PokemonRepository : IPokemonRepository // handles database interactions for pokemoncontroller
    {
        private readonly PokemonDBContext _context;
        private readonly IDistributedCache _cache; // Injecting IDistributedCache for caching pokemon data, allows for improved performance and reduced database load by caching frequently accessed data
        public PokemonRepository(PokemonDBContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<List<PokemonResponseDto>> GetAllPokemonAsync(QueryPokemonRequest query)
        {
            // cached both the list and query to ensure that if same query params are used within the cache expiration time frame, the data can be retrieved from cache without having to query the database again, improving performance and reducing database load
            var cachedPokemonList = await _cache.GetStringAsync("pokemonList_");
            var cachedPokemonQuery = await _cache.GetStringAsync("pokemonQuery_");
            
            // if cache contains data of query return from cache
            if (cachedPokemonList != null && JsonSerializer.Serialize(query) == cachedPokemonQuery)
            {
                return JsonSerializer.Deserialize<List<PokemonResponseDto>>(cachedPokemonList) ?? throw new ArgumentNullException("Cached pokemon list is null.");
            }
            
            var pokemon = _context.Pokemon
            .AsNoTracking() // improves performance by not tracking changes to the entities, since we are only reading data and not modifying it, this can reduce memory usage and speed up query execution, especially when dealing with large datasets
            .Include(p => p.Abilities) // Include the related Abilities for each Pokemon
            .AsQueryable(); // primes query for filtering based on query parameters

            // Adding Filtering based on query parameters, allows clients to filter pokemon by using query parameters
            return await FilterPokemonAsync(pokemon, query); 
        }

        public async Task<Pokemon> GetPokemonAsync(int id)
        {
            var cachedPokemon = await _cache.GetStringAsync($"pokemon_:{id}");
            if(cachedPokemon != null)
            {
                return JsonSerializer.Deserialize<Pokemon>(cachedPokemon) ?? throw new ArgumentNullException("Cache is doesn't exist.");
            }

            var pokemon = await _context.Pokemon
            .AsNoTracking()
            .Include(p => p.Abilities)
            .FirstOrDefaultAsync(p => p.Id == id);

            await _cache.SetStringAsync($"pokemon_:{id}", JsonSerializer.Serialize(pokemon ?? throw new ArgumentNullException("Pokemon is doesn't exist.")),  new DistributedCacheEntryOptions() 
            {
                SlidingExpiration = TimeSpan.FromSeconds(5),
            });

            return pokemon ?? throw new KeyNotFoundException("Pokemon with this ID does not exist.");
        }

        public async Task AddPokemonAsync(PokemonRequestDto pokemon)
        {
            await _context.Pokemon.AddAsync(pokemon.MapToPokemonModel());
            await SaveChangesAsync();

            await _cache.RemoveAsync("pokemonList_");
        }

        public async Task DeletePokemonAsync(Pokemon pokemon)
        {
        
            _context.Pokemon.Remove(pokemon);
            await SaveChangesAsync();

            await _cache.RemoveAsync($"pokemon_:{pokemon.Id}");
            await _cache.RemoveAsync("pokemonList_");
        }
        
#region Helper methods
        public async Task<bool> PokemonExistsAsync(string name)
        {
            return await _context.Pokemon.AsNoTracking().Where(p => p.Name == name).AnyAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<PokemonResponseDto>> FilterPokemonAsync(IQueryable<Pokemon> pokemon, QueryPokemonRequest query)
        {
            //TODO: Turn if statements into switch statements 
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

            var pokemonList = await pokemon.Select(pokemon => pokemon.MapToPokemonResponseDto()).ToListAsync();

            await _cache.SetStringAsync("pokemonQuery_", JsonSerializer.Serialize(query), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
            });

            await _cache.SetStringAsync("pokemonList_", JsonSerializer.Serialize(pokemonList), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1) // Cache expires after 10 minutes, allows for improved performance while ensuring data is not stale for too long
            });
            
            return pokemonList;
        }
#endregion Helper methods
    }
}