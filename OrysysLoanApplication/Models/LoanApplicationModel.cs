using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OrysysLoanApplication.Models
{
    public class LoanApplicationModel
    {
        [Required]
        public int LoanID { get; set; }

        [Required]
        [DisplayName("Customer Name")]
        public string CustomerName { get; set; }

        [Required]
        [DisplayName("Loan Type")]
        public string LoanTypeName { get; set; }

        [Required]
        public int LoanTypeID { get; set; }

        [Required]
        [DisplayName("Interest Rate")]
        public decimal InterestRate { get; set; }

        [Required]
        [DisplayName("Loan Amount")]
        public decimal LoanAmount { get; set; }

        [Required]
        [DisplayName("Duration (Month)")]
        public int Duration { get; set; }

        [Required]
        [DisplayName("Registered Date")]
        public DateTime RegisteredDate { get; set; }

        [Required]
        [DisplayName("Status")]
        public string Status { get; set; }

    }
}
