using System.ComponentModel.DataAnnotations;

namespace OrysysLoanApplication.Models
{
    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }
    }
}
