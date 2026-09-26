namespace SharedDtos.HelperObjects
{
    public class ChatBotResponseDto
    {
        public string Message { get; set; } = string.Empty;
        public string CorrelationId { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public bool Success { get; set; } = true;
    }
}
