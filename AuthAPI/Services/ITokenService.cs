using AuthAPI.Infrastructure.Models;

namespace AuthAPI.Services
{
    public interface ITokenService 
    {
        Task<string> CreateTokenAsync(AppUser user);
    }
}