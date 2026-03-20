using System.ComponentModel.DataAnnotations;

namespace API.Dtos
{
    public class RegisterRequestDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public List<ClaimDto>? Claims { get; set; }
    }


}