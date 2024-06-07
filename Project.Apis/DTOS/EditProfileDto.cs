using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace Project.Apis.DTOS
{
    public class EditProfileDto
    {
        [RegularExpression(@"^[a-zA-Z]+\s[a-zA-Z]+$", ErrorMessage = "Full name must consist of at least two names.")]
        public string? FullName { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Phone number must be 11 digits.")]
        public string? PhoneNumber { get; set; }
    }
}
