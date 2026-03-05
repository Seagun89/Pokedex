using API.Mappers;
using API.Dtos;
using API.Repos;
using API.Models;
using API.HelperObjects;

namespace API.Services
{
    public class PokemonService : IPokemonService // handles business logic for PokemonController, calls methods from PokemonRepository to interact with the database
    {
        public IPokemonRepository _pokemonRepository;
        public PokemonService(IPokemonRepository pokemonRepository)
        {
            _pokemonRepository = pokemonRepository;
        }
        public async Task AddPokemonAsync(PokemonRequestDto pokemon)
        {
            // Base case: if pokemon is null throw an exception
            if (pokemon == null) throw new ArgumentNullException(nameof(pokemon));

            // Checking 
            var pokemonExists = await _pokemonRepository.PokemonExistsAsync(pokemon.Name);
            if(pokemonExists) throw new InvalidOperationException("Pokemon already exists");
            
            // Process request and add pokemon into database
            await _pokemonRepository.AddPokemonAsync(pokemon);
            await _pokemonRepository.SaveChangesAsync();
        }

        public async Task DeletePokemonAsync(int id)
        {
            //Base Case: if id is less than or equal to 0, throw an exception
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id), "Id must be greater than 0.");

            //Check if pokemon with given id exists, if not throw an exception
            var pokemon = await GetPokemonOrThrowAsync(id);

            //Process request and delete pokemon from database
            await _pokemonRepository.DeletePokemonAsync(pokemon);
            await _pokemonRepository.SaveChangesAsync();
        }

        public async Task<List<PokemonResponseDto>> GetAllPokemonAsync(QueryPokemonRequest query)
        {
            return await _pokemonRepository.GetAllPokemonAsync(query);
        }

        public async Task<PokemonResponseDto> GetPokemonByIdAsync(int id)
        {
            var pokemon = await GetPokemonOrThrowAsync(id);
            return pokemon.MapToPokemonResponseDto();
        }

        public async Task UpdatePokemonAsync(PokemonUpdateRequestDto UpdateRequest, int id)
        {
            if (UpdateRequest == null) throw new ArgumentNullException(nameof(UpdateRequest));

            if (id <= 0) throw new ArgumentOutOfRangeException($"{id} is not a valid id. Id must be greater than 0.");

            var pokemon = await GetPokemonOrThrowAsync(id);
            pokemon.Name = UpdateRequest.Name ?? pokemon.Name;
            pokemon.Height = UpdateRequest.Height;
            pokemon.Weight = UpdateRequest.Weight;
            pokemon.Abilities.ForEach(a =>
            {
                var updatedAbility = UpdateRequest.Abilities.First(ua => ua.Name == a.Name);
                a.Name = updatedAbility.Name ?? a.Name;
                a.Description = updatedAbility.Description ?? a.Description;
                a.AbilityType = updatedAbility.AbilityType ?? a.AbilityType;
                a.Damage = updatedAbility.Damage != 0 ? updatedAbility.Damage : a.Damage;
            });

            await _pokemonRepository.SaveChangesAsync();
        }

        // helper methods
        public async Task<Pokemon> GetPokemonOrThrowAsync(int id)
        {
            return await _pokemonRepository.GetPokemonAsync(id) ?? throw new KeyNotFoundException("Pokemon with this ID does not exist.");
        }
    }
}