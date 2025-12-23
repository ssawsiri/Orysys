using System.ComponentModel;

namespace OrysysLoanApplication.Models
{
    public class LoanTypesModel
    {
        public int LoanTypeid { get; set; }

        [DisplayName("Loan Type")]
        public string LoanTypeName { get; set; }

        [DisplayName("Interest Rate")]
        public double InterestRate { get; set; }
    }
}
