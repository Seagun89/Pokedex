using API.Models;
using API.Dtos;

namespace API.Repos
{
    public interface IPokemonRepository
    {
        public Task<Pokemon> GetPokemonAsync(int id);
        public Task<List<PokemonResponseDto>> GetAllPokemonAsync();
        public Task AddPokemonAsync(PokemonRequestDto pokemon);
        public Task DeletePokemonAsync(Pokemon pokemon);
        public Task<bool> PokemonExistsAsync(string name);
        public Task SaveChangesAsync();
    }
}