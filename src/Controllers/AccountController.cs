using API.Models;
using API.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;

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
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto registerDto) {
            try
            {
                var user =  new AppUser
                {
                    UserName = registerDto.Username,
                    Email = registerDto.Email,
                };     

                var createUserResult = await _userManager.CreateAsync(user, registerDto.Password);

                if (createUserResult.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(user, "User");
                    
                    if (roleResult.Errors.Any()) {
                        return StatusCode(500, roleResult.Errors);
                    }
                }
                else if (createUserResult.Errors.Any()) {
                    return StatusCode(500, createUserResult.Errors);
                }
            } catch (Exception ex)
            {
                //check to see if user already exists, if so return invalidOperationException with message "User already exists"
                throw new InvalidOperationException("An error occurred while creating the user.", ex);
            }

            return Ok("User created successfully with role 'User'."); 
        }
    }
}