using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WetosMVC.Models
{
    public class CompanyLoanSanctionModel
    {
        public int LoanSanctionedId { get; set; }

        public int LoanApplicationId { get; set; }

        public int EmployeeId { get; set; }

        public double? RequiredLoanAmount { get; set; }

        public double? EligibleLoanAmount { get; set; }

        public string LoanReason { get; set; }

        public DateTime? LoanRequiredDate { get; set; }

        public string AttachProof { get; set; }

        public int? RepaymentMonths { get; set; }

        public double? SanctionedAmount { get; set; }

        public double? RateOfInterest { get; set; }

        public double? EMI { get; set; }

        public DateTime? LoanSanctionedDate { get; set; }
    }

}