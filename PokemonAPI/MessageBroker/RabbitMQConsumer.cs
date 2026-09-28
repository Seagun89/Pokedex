using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace PokemonAPI.MessageBroker
{
    public class RabbitMQConsumer<T> : IRabbitMQConsumer<T>, IAsyncDisposable
    {
        private ConnectionFactory _factory;
        private IConnection _connection = null!;
        private IChannel _channel = null!;
        private IConfiguration _configuration;
        private string _queueName = string.Empty;
        private readonly ConcurrentDictionary<string, TaskCompletionSource<T?>> _pendingRequests = new();

        public RabbitMQConsumer(IConfiguration configuration)
        {
            _configuration = configuration;
            var hostName = _configuration["Rabbit_MQ:HostName"] ?? "localhost";
            var port = int.Parse(_configuration["Rabbit_MQ:Port"] ?? "5672");
            var username = _configuration["Rabbit_MQ:Username"] ?? "guest";
            var password = _configuration["Rabbit_MQ:Password"] ?? "guest";

            _factory = new ConnectionFactory() 
            { 
                HostName = hostName, 
                Port = port,
                UserName = username,
                Password = password
            };
        }

        public async Task CreateAsync(string queueName)
        {
            _connection = await _factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
            _queueName = queueName;

            await _channel.QueueDeclareAsync(queue: _queueName, durable: true, exclusive: false, autoDelete: false,
                arguments: new Dictionary<string, object?> { { "x-queue-type", "quorum" } });

            Console.WriteLine($" [*] Queue '{_queueName}' is ready for consuming.");

            // Start consuming messages
            _ = ConsumeMessagesAsync();
        }

        public async Task<T?> ConsumeAsync(string correlationId, int timeoutMs = 30000)
        {
            var tcs = new TaskCompletionSource<T?>();
            _pendingRequests.TryAdd(correlationId, tcs);

            try
            {
                // Wait for response with timeout
                using (var cts = new CancellationTokenSource(timeoutMs))
                {
                    cts.Token.Register(() =>
                    {
                        tcs.TrySetCanceled();
                        _pendingRequests.TryRemove(correlationId, out _);
                    });

                    return await tcs.Task;
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($" [!] Timeout waiting for response with correlation ID: {correlationId}");
                return default;
            }
        }

        private async Task ConsumeMessagesAsync()
        {
            try
            {
                var consumer = new AsyncEventingBasicConsumer(_channel);

                consumer.ReceivedAsync += async (model, ea) =>
                {
                    try
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        var response = JsonSerializer.Deserialize<MessageWrapper<T>>(message);

                        if (response?.CorrelationId != null && 
                            _pendingRequests.TryRemove(response.CorrelationId, out var tcs))
                        {
                            tcs.SetResult(response.Data);
                            Console.WriteLine($" [x] Received response for correlation ID: {response.CorrelationId}");
                        }

                        // Acknowledge the message
                        await _channel.BasicAckAsync(ea.DeliveryTag, false);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($" [!] Error processing message: {ex.Message}");
                        await _channel.BasicNackAsync(ea.DeliveryTag, false, true);
                    }
                };

                Console.WriteLine($" [*] Started consuming from queue '{_queueName}'");
                await _channel.BasicConsumeAsync(queue: _queueName, autoAck: false, consumer: consumer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($" [!] Fatal error in consumer: {ex.Message}");
                throw;
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_channel != null)
            {
                await _channel.CloseAsync();
            }

            if (_connection != null)
            {
                await _connection.CloseAsync();
            }
        }

        // Internal wrapper class to handle correlation ID with the message
        private class MessageWrapper<TData>
        {
            public string? CorrelationId { get; set; }
            public TData? Data { get; set; }
        }
    }
}
