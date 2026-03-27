using SharedDtos.Dtos;

namespace AuthAPI.Services
{
    public interface IAccountService
    {
        public Task RegisterAsync(RegisterRequestDto registerDto);
        public Task<LoginResponseDto> LoginAsync(LoginRequestDto loginDto);
    }
}