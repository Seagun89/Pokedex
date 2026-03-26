namespace SharedDtos.Dtos
{
   public class AbilityRequestDto
    {
        public string? Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public string AbilityType { get; set; } = string.Empty;
        public float Damage { get; set; }
    }
}