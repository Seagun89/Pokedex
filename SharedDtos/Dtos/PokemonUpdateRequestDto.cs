using System.ComponentModel.DataAnnotations;
namespace SharedDtos.Dtos
{
    public class PokemonUpdateRequestDto
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public int PokemonId { get; set; }
        [Required]
        public float Height { get; set; }
        [Required]
        public float Weight { get; set; }
        [Required]
        public string? AbilityType { get; set; }
        public string? Username { get; set; }
        [Required]
        public List<AbilityRequestDto> Abilities { get; set; } = new List<AbilityRequestDto>();
    }
}