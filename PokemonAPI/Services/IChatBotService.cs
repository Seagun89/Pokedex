using SharedDtos.HelperObjects;

namespace PokemonAPI.Services
{
    public interface IChatBotService
    {
        public Task<ChatBotResponseDto> ChatBotAsync(ChatBotRequestDto request);
    }
}
