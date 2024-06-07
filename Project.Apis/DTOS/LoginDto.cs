using System.ComponentModel.DataAnnotations;

namespace Project.Apis.DTOS
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string password { get; set; }
    }
}

