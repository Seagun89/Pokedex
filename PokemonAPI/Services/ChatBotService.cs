using SharedDtos.Dtos;
using SharedDtos.HelperObjects;
using PokemonAPI.MessageBroker;

namespace PokemonAPI.Services
{
    public class ChatBotService : IChatBotService
    {
        private readonly ILogger<ChatBotService> _logger;
        private readonly IRabbitMQPublisher<ChatBotRequestDto> _publisher;
        private readonly IRabbitMQConsumer<ChatBotResponseDto> _consumer;

        public ChatBotService(
            ILogger<ChatBotService> logger,
            IRabbitMQPublisher<ChatBotRequestDto> publisher,
            IRabbitMQConsumer<ChatBotResponseDto> consumer)
        {
            _logger = logger;
            _publisher = publisher;
            _consumer = consumer;
        }

        public async Task<ChatBotResponseDto> ChatBotAsync(ChatBotRequestDto request)
        {
            try
            {
                _logger.LogInformation($"Processing chatbot request with correlation ID: {request.CorrelationId}");

                // 1. Publish the request to the worker service
                await _publisher.PublishAsync(request);
                _logger.LogInformation($"Published chatbot request to queue");

                // 2. Wait for the worker service to respond (with 30 second timeout)
                var response = await _consumer.ConsumeAsync(request.CorrelationId, timeoutMs: 30000);

                if (response == null)
                {
                    _logger.LogWarning($"No response received for correlation ID: {request.CorrelationId}");
                    return new ChatBotResponseDto
                    {
                        Message = "The chatbot service did not respond in time. Please try again.",
                        CorrelationId = request.CorrelationId,
                        Timestamp = DateTime.UtcNow,
                        Success = false
                    };
                }

                _logger.LogInformation($"Received chatbot response for correlation ID: {request.CorrelationId}");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing chatbot request: {ex.Message}");

                return new ChatBotResponseDto
                {
                    Message = "An error occurred processing your request.",
                    CorrelationId = request.CorrelationId,
                    Timestamp = DateTime.UtcNow,
                    Success = false
                };
            }
        }
    }
}
