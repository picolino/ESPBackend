using System.ComponentModel.DataAnnotations;

namespace Authorization.Models.Register
{
    public class RegisterUserModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }
        
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public override string ToString()
        {
            return $"{nameof(UserName)}: {UserName}, {nameof(Password)}: {Password}, {nameof(ConfirmPassword)}: {ConfirmPassword}";
        }
    }
}