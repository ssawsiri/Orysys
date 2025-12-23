using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace OrysysLoanApplication.Views.Shared
{
    public class SLNICPassportValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            string input = value.ToString().Trim();

            var oldNicPattern = new Regex(@"^\d{9}[VX]$", RegexOptions.IgnoreCase);
            var newNicPattern = new Regex(@"^\d{12}$");

            var passportPattern = new Regex(@"^[A-Z]{1,2}\d{7}$");

            if (oldNicPattern.IsMatch(input) ||
                newNicPattern.IsMatch(input) ||
                passportPattern.IsMatch(input))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("Invalid NIC or Passport number.");
        }
    }
}
