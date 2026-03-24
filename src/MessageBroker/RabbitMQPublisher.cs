using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace API.MessageBroker
{
    public class RabbitMQPublisher<T> : IRabbitMQPublisher<T>, IAsyncDisposable
    {
        private ConnectionFactory _factory;
        private IConnection _connection;
        private IChannel _channel;
        private string _queueName = string.Empty;

        public RabbitMQPublisher()
        {
            _factory = new ConnectionFactory() { HostName = "localhost" };
        }

        public async Task<IChannel> CreateAsync(string queueName)
        {
            _connection = await _factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
            _queueName = queueName;

            await _channel.QueueDeclareAsync(queue: _queueName, durable: true, exclusive: false, autoDelete: false,
                arguments: new Dictionary<string, object?> { { "x-queue-type", "quorum" } });

            return _channel;
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