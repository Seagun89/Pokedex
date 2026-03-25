using System.Text;
using System.Text.Json;
using PokemonAPI.Dtos;
using PokemonAPI.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PokemonAPI.MessageBroker
{
    public class ExportPokemonWorker : BackgroundService
    {
        private IServiceProvider _serviceProvider;

        public ExportPokemonWorker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        // Retrieved this code from RabbitMQ documentation and modified for needs.
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var queueName = "Pokemon_Export_Worker";
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var connection = await factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false,
                arguments: new Dictionary<string, object?> { { "x-queue-type", "quorum" } });

            var consumer = new AsyncEventingBasicConsumer(channel);

            Console.WriteLine(" [*] Waiting for messages.");

            consumer.ReceivedAsync += async (model, ea) =>
            {
                using var scope = _serviceProvider.CreateAsyncScope();
                var _pokemonService = scope.ServiceProvider.GetRequiredService<IPokemonService>();

                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                
                //Processing
                var pokemonList = JsonSerializer.Deserialize<List<PokemonResponseDto>>(message) ?? throw new ArgumentNullException("Failed to deserialize message to List<PokemonResponseDto>.");
                await _pokemonService.ExportAsync(pokemonList);

                Console.WriteLine($" [x] Received {message}");
                await Task.CompletedTask;
            };

            await channel.BasicConsumeAsync(queueName, autoAck: true, consumerTag: "ExportPokemonWorker", noLocal: false, exclusive: false, arguments: null, consumer: consumer, cancellationToken: stoppingToken);

            // Keep worker alive until cancellation
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}