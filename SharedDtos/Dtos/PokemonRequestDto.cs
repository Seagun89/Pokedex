using System.ComponentModel.DataAnnotations;

namespace SharedDtos.Dtos
{
    public class PokemonRequestDto
    {
        [Required]
        [MinLength(4), MaxLength(20)]
        public string Name { get; set; } = string.Empty;
        [Required]
        public int PokemonId { get; set; }  
        [Required]
        public float Height { get; set; }
        [Required]
        public float Weight { get; set; }
        [Required]
        public string AbilityType { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        [Required]
        public List<AbilityRequestDto> Abilities { get; set; } = new List<AbilityRequestDto>();
    }
}