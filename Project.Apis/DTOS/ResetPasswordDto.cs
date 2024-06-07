using System.ComponentModel.DataAnnotations;

namespace Project.Apis.DTOS
{
    public class ResetPasswordDto
    {
        [Required(ErrorMessage = "New Password is required")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "New Password is required")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Password doesn't match")]
        public string ConfirmNewPassword { get; set; }
        public int Code {  get; set; }

        public string Email { get; set; }   

    }
}
