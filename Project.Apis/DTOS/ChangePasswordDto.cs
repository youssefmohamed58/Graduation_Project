using System.ComponentModel.DataAnnotations;

namespace Project.Apis.DTOS
{
    public class ChangePasswordDto
    {
        [Required]
        public string OldPassword { get; set; }

        [Required]
        [RegularExpression("(?=^.{6,10}$)(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[!@#$%^&amp;*()_+]).*$",
        ErrorMessage = "Password must contains 1 Uppercase, 1 Lowercase, 1 Digit, 1 Spaecial Character")]
        public string NewPassword { get; set; }
    }
}
