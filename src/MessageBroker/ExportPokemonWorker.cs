using System.Text;
using System.Text.Json;
using API.Dtos;
using API.Services;
using RabbitMQ.Client.Events;

namespace API.MessageBroker
{
    public class ExportPokemonWorker : BackgroundService
    {
        private IRabbitMQPublisher<List<PokemonResponseDto>> _publisher;
        private IServiceProvider _serviceProvider;

        public ExportPokemonWorker(IRabbitMQPublisher<List<PokemonResponseDto>> publisher, IServiceProvider serviceProvider)
        {
            _publisher = publisher;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var queueName = "Pokemon_Export_Worker";
            var channel = await _publisher.CreateAsync(queueName);
            var consumer = new AsyncEventingBasicConsumer(channel);

            Console.WriteLine(" [*] Waiting for messages.");

            consumer.ReceivedAsync += async (model, ea) =>
            {
                using var scope = _serviceProvider.CreateAsyncScope();
                var _pokemonService = scope.ServiceProvider.GetRequiredService<IPokemonService>();

                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                
                var pokemonList = JsonSerializer.Deserialize<List<PokemonResponseDto>>(message) ?? throw new ArgumentNullException("Failed to deserialize message to List<PokemonResponseDto>.");
                await _pokemonService.ExportAsync(pokemonList);

                Console.WriteLine($" [x] Received {message}");
                await Task.CompletedTask;
            };

            await channel.BasicConsumeAsync(queueName, autoAck: true, consumerTag: "ExportPokemonWorker", noLocal: false, exclusive: false, arguments: null, consumer: consumer, cancellationToken: stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}