namespace PokemonAPI.MessageBroker
{
    public interface IRabbitMQPublisher<T>
    {
        public Task CreateAsync(string queueName);
        public Task PublishAsync(T message);
    }
}