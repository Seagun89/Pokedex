using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedDtos;

namespace ExportPokemon
{
    public class ExportPokemonWorker : BackgroundService
    {
        private readonly ILogger<ExportPokemonWorker> _logger;
        public ExportPokemonWorker(ILogger<ExportPokemonWorker> logger)
        {
            _logger = logger;
        }
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
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                //Processing
                var ExportPokemonMessage = JsonSerializer.Deserialize<ExportPokemonMessage>(message);
                await ExportAsync(ExportPokemonMessage?.PokemonList ?? throw new ArgumentNullException("Failed to export List<PokemonResponseDto> because null object."));

                Console.WriteLine($" [x] Received {message}");
                await Task.CompletedTask;
            };

            await channel.BasicConsumeAsync(queueName, autoAck: true, consumerTag: "ExportPokemonWorker", noLocal: false, exclusive: false, arguments: null, consumer: consumer, cancellationToken: stoppingToken);
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
                await Task.Delay(1000, stoppingToken);
            }
        }

        #region Helper Method
        public async Task ExportAsync(List<PokemonResponseDto> pokemon)
        {
            var fileName = @"Pokemon_Export_" + DateTime.UtcNow.ToString("MM_dd_yyyy_HH_mm") + ".csv";
            Directory.CreateDirectory("Exports");

            var path = Path.Combine("Exports", fileName);

            using var StreamWriter = new StreamWriter(path);
            await StreamWriter.WriteLineAsync("Id | Name | AbilityType");
            await StreamWriter.WriteLineAsync("-----------------------------------");

            foreach (var p in pokemon)
            {
                await StreamWriter.WriteLineAsync($"{p.Id} | {p.Name} | {p.AbilityType}");
            }
        }

        #endregion Helper Method
    }
}
