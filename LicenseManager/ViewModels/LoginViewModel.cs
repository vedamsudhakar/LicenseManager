using System.ComponentModel.DataAnnotations;

namespace LicenseManager.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public required string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public required string Password { get; set; }
    }
}
