namespace SharedDtos.HelperObjects
{
    public class ChatBotRequestDto
    {
        public string UserMessage { get; set; } = string.Empty;
        public string CorrelationId { get; set; } = Guid.NewGuid().ToString();
    }
}
