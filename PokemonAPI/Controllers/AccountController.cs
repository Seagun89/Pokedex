using SharedDtos.Dtos;
using PokemonAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace PokemonAPI.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequestDto registerDto) {
            await _accountService.RegisterAsync(registerDto);
            return Created($"/Account/Register/", "User created successfully with claims and role 'User'."); 
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequestDto loginDto) {
            var result = await _accountService.LoginAsync(loginDto);
            return Ok(result);
        }
    }
}