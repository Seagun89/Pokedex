namespace API.Dtos
{
    public class AbilityFilterDto
    {
        public string? Name { get; set; }
        public string AbilityType { get; set; } = string.Empty;
        public float? Damage { get; set; }
    }
}