using System.ComponentModel.DataAnnotations;

namespace Project.Apis.DTOS
{
    public class RegisterDto
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z]+\s[a-zA-Z]+$", ErrorMessage = "Full name must consist of at least two names.")]
        public string FullName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Phone number must be 11 digits.")]
        public string PhoneNumber { get; set; }

        [Required]
        [RegularExpression("(?=^.{6,10}$)(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[!@#$%^&amp;*()_+]).*$",
          ErrorMessage = "Password must contains 1 Uppercase, 1 Lowercase, 1 Digit, 1 Spaecial Character")]
        public string password { get; set; }


    }
}
