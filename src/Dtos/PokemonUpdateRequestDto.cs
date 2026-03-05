using API.Models;
using System.ComponentModel.DataAnnotations;
namespace API.Dtos
{
    public class PokemonUpdateRequestDto
    {
        [Required]
        [MinLength(4), MaxLength(20)]
        public int Id { get; set;}
        [Required]
        public string? Name { get; set; }
        [Required]
        public float Height { get; set; }
        [Required]
        public float Weight { get; set; }
        [Required]
        public string? AbilityType { get; set; }
        [Required]
        public List<Ability> Abilities { get; set; } = new List<Ability>();
    }
}