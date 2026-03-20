using API.Models;
using API.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity.Data;

namespace API.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        public AccountController(UserManager<AppUser> userManager)
        {
            _userManager = userManager; 
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequestDto registerDto) {
            // TODO: Create an account mapper class to map registerDto to AppUser, claimDto to claims
            var user =  new AppUser
            {
                UserName = registerDto.Username,
                Email = registerDto.Email,
            };

            var createUserResult = await _userManager.CreateAsync(user, registerDto.Password);

            if (createUserResult.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(user, "User"); // role-based access control, assigns the "User" role to the newly created user, allowing them to have specific permissions and access levels defined for that role in the application
                
                if (roleResult.Errors.Any()) {
                    throw new InvalidOperationException("An error occurred while adding the user to this role.");
                }

                if (registerDto.Claims != null && registerDto.Claims.Any())
                {
                    var claims = registerDto.Claims
                        .Select(c => new Claim(c.Type, c.Value))
                        .ToList();
                    
                    var claimResult = await _userManager.AddClaimsAsync(user, claims);
                    
                    if (claimResult.Errors.Any())
                    {
                        throw new InvalidOperationException("An error occurred while adding claims to the user.");
                    }
                }
            }
            else if (createUserResult.Errors.Any()) {
                throw new InvalidOperationException("An error occurred while creating the user.");
            }

            return Created($"/Account/Register/{user.Id}", "User created successfully with claims and role 'User'."); 
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync(LoginRequestDto loginDto) {
            return Ok("Login endpoint is not implemented yet.");
        }
    }
}