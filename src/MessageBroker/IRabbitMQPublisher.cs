using RabbitMQ.Client;

namespace API.MessageBroker
{
    public interface IRabbitMQPublisher<T>
    {
        public Task<IChannel> CreateAsync(string queueName);
        public Task PublishAsync(T message);
    }
}