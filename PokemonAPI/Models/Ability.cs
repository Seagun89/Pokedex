namespace PokemonAPI.Models
{
    public class Ability // Represents the abilities of a Pokemon, with properties for name, description, type, and damage. Also includes a foreign key to associate it with a specific Pokemon.
    {
        public int Id { get; set; }
        public int PokemonId { get; set; }
        public string? Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public string AbilityType { get; set; } = string.Empty;
        public float Damage { get; set; }
    }
}