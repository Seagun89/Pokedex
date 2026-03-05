using API.Dtos;
using API.Models;

namespace API.Services
{
    public interface IPokemonService
    {
        public Task<List<PokemonResponseDto>> GetAllPokemonAsync();
        public Task<PokemonResponseDto> GetPokemonByIdAsync(int id);
        public Task AddPokemonAsync(PokemonRequestDto pokemon);
        public Task UpdatePokemonAsync(PokemonUpdateRequestDto UpdateRequest, int id);
        public Task DeletePokemonAsync(int id);
         Task<Pokemon> GetPokemonOrThrowAsync(int id);
    }
}