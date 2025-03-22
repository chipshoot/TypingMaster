using System.ComponentModel.DataAnnotations;

namespace TypingMaster.Business.Models
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        
        [Required]
        public string Password { get; set; } = null!;
        
        public bool RememberMe { get; set; } = false;
    }
}
