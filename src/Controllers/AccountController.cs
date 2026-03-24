using API.Models;
using API.Dtos;
using API.Mappers;
using API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace API.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        // TODO: Create an AccountService file to handle the business logic of account controller
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        public AccountController(UserManager<AppUser> userManager, ITokenService tokenService)
        {
            _userManager = userManager; 
            _tokenService = tokenService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequestDto registerDto) {
            var user = registerDto.MapToAppUser(); // using extension method to map RegisterRequestDto to AppUser, simplifies the mapping process and keeps the controller code clean and maintainable

            var createUserResult = await _userManager.CreateAsync(user, registerDto.Password);

            if (createUserResult.Succeeded)
            {
                // TODO: Refractor to handle different roles and claims
                if (registerDto.Claims != null && registerDto.Claims.Any(c => c.Type.ToLower() == "role" && c.Value.ToLower() == "user"))
                {
                    var roleResult = await _userManager.AddToRoleAsync(user, "User"); // role-based access control, assigns the "User" role to the newly created user, allowing them to have specific permissions and access levels defined for that role in the application
                
                    if (roleResult.Errors.Any()) {
                        throw new InvalidOperationException("An error occurred while adding the user to this role.");
                    }
                    
                    var claims = registerDto.MapToClaims();
                    var claimResult = await _userManager.AddClaimsAsync(user, claims);
                    
                    if (claimResult.Errors.Any())
                    {
                        throw new InvalidOperationException("An error occurred while adding claims to the user.");
                    }
                }
                else
                {
                    throw new InvalidOperationException("Cannot assign claim with non-User role or missing role claim.");
                }
            }
            else if (createUserResult.Errors.Any()) {
                throw new InvalidOperationException("An error occurred while creating the user.");
            }

            return Created($"/Account/Register/{user.Id}", "User created successfully with claims and role 'User'."); 
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequestDto loginDto) {
            if (loginDto == null || string.IsNullOrWhiteSpace(loginDto.UserName) || string.IsNullOrWhiteSpace(loginDto.Password))
            {
                throw new ArgumentNullException("LoginRequestDto is null or contains invalid data.");
            }
            var verifiedUser = await _userManager.FindByNameAsync(loginDto.UserName);
            if (verifiedUser == null) throw new UnauthorizedAccessException("Username not found.");

            var passwordCheck = await _userManager.CheckPasswordAsync(verifiedUser, loginDto.Password);
            if (passwordCheck)
            {
                var token = await _tokenService.CreateTokenAsync(verifiedUser);
                return Ok(new { Token = token });
            }
            else
            {
                throw new UnauthorizedAccessException("Invalid password.");
            }
        }
    }
}