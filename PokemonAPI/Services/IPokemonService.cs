using SharedDtos.HelperObjects;
using SharedDtos.Dtos;
using Infrastructure.Models;

namespace PokemonAPI.Services
{
    public interface IPokemonService // Interface for pokemon service which defines the methods that the PokemonService class must implement. This allows for better separation of concerns and makes it easier to test the service layer independently of the controller layer.
    {
        public Task<List<PokemonResponseDto>> GetAllPokemonAsync(QueryPokemonRequest query);
        public Task<PokemonResponseDto> GetPokemonByIdAsync(int id);
        public Task AddPokemonAsync(PokemonRequestDto pokemon);
        public Task UpdatePokemonAsync(PokemonUpdateRequestDto UpdateRequest, int id);
        public Task DeletePokemonAsync(int id);
        public Task<Pokemon> GetPokemonOrThrowAsync(int id);
        public Task ExportAllPokemonAsync();
    }
}