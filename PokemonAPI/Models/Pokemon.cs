namespace PokemonAPI.Models
{
    public class Pokemon // Represents a Pokemon, with properties for name, height, weight, ability type, and a list of abilities. Also includes an Primary ID property for database management.
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public float Height { get; set; }
        public float Weight { get; set; }
        public string AbilityType { get; set; } = string.Empty;
        public List<Ability> Abilities { get; set; } = new List<Ability>();
    }
}