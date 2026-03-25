using PokemonAPI.Models;
using PokemonAPI.Dtos;
using PokemonAPI.HelperObjects;

namespace PokemonAPI.Repos
{
    public interface IPokemonRepository
    {
        public Task<Pokemon> GetPokemonAsync(int id);
        public Task<List<PokemonResponseDto>> GetAllPokemonAsync(QueryPokemonRequest query);
        public Task AddPokemonAsync(PokemonRequestDto pokemon);
        public Task DeletePokemonAsync(Pokemon pokemon);
        public Task<bool> PokemonExistsAsync(string name);
        public Task SaveChangesAsync();
        public Task<List<PokemonResponseDto>> ExportAllPokemonAsync();
    }
}