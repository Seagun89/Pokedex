using API.Models;
using API.Dtos;
using System.Security.Claims;

namespace API.Mappers
{
    public static class AccountMapper
    {
        public static AppUser MapToAppUser(this RegisterRequestDto registerRequestDto)
        {
            return new AppUser
            {
                UserName = registerRequestDto.Username,
                Email = registerRequestDto.Email,
            };
        }

        public static IEnumerable<Claim> MapToClaims(this RegisterRequestDto registerRequestDto)
        {
            return registerRequestDto.Claims?
                .Where(c => !string.IsNullOrWhiteSpace(c.Type) && !string.IsNullOrWhiteSpace(c.Value))
                .Select(c => new Claim(c.Type, c.Value))
                .ToList() ?? Enumerable.Empty<Claim>();
        }
    }
}