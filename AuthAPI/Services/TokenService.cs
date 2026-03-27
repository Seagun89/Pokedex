using AuthAPI.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AuthAPI.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config; // used to grab jwt settings from appsettings.json
        private UserManager<AppUser> _userManager; // class used to manage user accounts, including creating, deleting, and retrieving user information, as well as handling authentication and authorization tasks such as role management.
        
        public TokenService(IConfiguration configuration, UserManager<AppUser> userManager)
        {
            _config = configuration;
            _userManager = userManager;
        }

        public async Task<string> CreateTokenAsync(AppUser user)
        {
            var claims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(_config["JWT:Secret"] ?? 
                throw new ArgumentNullException("JWT:Secret configuration value is missing.")
            ));

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _config["JWT:Issuer"],
                audience: _config["JWT:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(7),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
        
    }
}