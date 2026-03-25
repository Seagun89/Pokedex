using PokemonAPI.Models;
namespace PokemonAPI.Services
{
    public interface ITokenService 
    {
        Task<string> CreateTokenAsync(AppUser user);
    }
}