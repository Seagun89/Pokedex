using SharedDtos.HelperObjects;

namespace PokemonAPI.MessageBroker
{
    public class ChatBotRabbitMQInitializationService : IHostedService
    {
        private readonly IRabbitMQPublisher<ChatBotRequestDto> _publisher;
        private readonly IRabbitMQConsumer<ChatBotResponseDto> _consumer;
        private readonly ILogger<ChatBotRabbitMQInitializationService> _logger;

        public ChatBotRabbitMQInitializationService(
            IRabbitMQPublisher<ChatBotRequestDto> publisher,
            IRabbitMQConsumer<ChatBotResponseDto> consumer,
            ILogger<ChatBotRabbitMQInitializationService> logger)
        {
            _publisher = publisher;
            _consumer = consumer;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Initializing ChatBot RabbitMQ queues...");
                await _publisher.CreateAsync("PokeDex_ChatBot_Request");
                await _consumer.CreateAsync("PokeDex_ChatBot_Response");
                _logger.LogInformation("✓ ChatBot queues initialized");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "✗ Failed to initialize ChatBot queues");
                throw;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}