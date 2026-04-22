using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace PokemonAPI.MessageBroker
{
    // Retrieved this code from RabbitMQ documentation and modified for needs.
    public class RabbitMQPublisher<T> : IRabbitMQPublisher<T>, IAsyncDisposable
    {
        private ConnectionFactory _factory;
        private IConnection _connection = null!;
        private IChannel _channel = null!;
        private IConfiguration _configuration;
        private string _queueName = string.Empty;

        public RabbitMQPublisher(IConfiguration configuration)
        {
            _configuration = configuration;
            var hostName = _configuration["Rabbit_MQ:HostName"] ?? "localhost";
            _factory = new ConnectionFactory() { HostName = hostName, Port = 5672 };
        }

        public async Task CreateAsync(string queueName)
        {
            _connection = await _factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
            _queueName = queueName;

            await _channel.QueueDeclareAsync(queue: _queueName, durable: true, exclusive: false, autoDelete: false,
                arguments: new Dictionary<string, object?> { { "x-queue-type", "quorum" } });
            
            Console.WriteLine($" [*] Queue '{_queueName}' is ready for publishing.");
        }

        public async Task PublishAsync(T message)
        {
            var messageString = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(messageString);

            await _channel.BasicPublishAsync(exchange: string.Empty, routingKey: _queueName, body: body);
            Console.WriteLine($" [x] Sent {_queueName}: {messageString}");
        }

        public async ValueTask DisposeAsync()
        {
            await _channel.CloseAsync();
            await _connection.CloseAsync();
        }
    }
}