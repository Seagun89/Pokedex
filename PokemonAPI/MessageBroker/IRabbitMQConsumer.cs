namespace PokemonAPI.MessageBroker
{
    public interface IRabbitMQConsumer<T>
    {
        public Task CreateAsync(string queueName);
        public Task<T?> ConsumeAsync(string correlationId, int timeoutMs = 30000);
    }
}
