using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WetosMVC.Models
{
    public class PayRollModel
    {

    }


    public class SalaryStructure
    {

        public int? SalStructureId { get; set; }

        public string SalStructureLoc { get; set; }

        public int? ComponantId { get; set; }

        public string Formula { get; set; }

        public string ComponantShort { get; set; }

        public double? MaxValue { get; set; }

        public double? MinValue { get; set; }


        public string StringSalStr { get; set; }

        public string StringComp { get; set; }

    }

    //Loan Aapplication Model
    public class CompanyLoanApplicationModel
    {
        public int LoanApplicationId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public double? RequiredLoanAmount { get; set; }
        public double? EligibleLoanAmount { get; set; }
        public string LoanReason { get; set; }
        public DateTime? LoanRequiredDate { get; set; }
        public string AttachProof { get; set; }
        public int? RepaymentMonths { get; set; }
        public int RepaymentMonthsCount { get; set; }
        public double? SanctionedAmount { get; set; }
        public double? RateOfInterest { get; set; }
        public double? EMI { get; set; }
        public DateTime? LoanSanctionedDate { get; set; }
        public string RejectionReason { get; set; }
        public int? StatusId { get; set; }
        public string Status { get; set; }

        public bool IsPaymentProcess { get; set; }
        public DateTime? PaymentDate { get; set; }
        public DateTime? EMIStartDate { get; set; }
        public DateTime? EMIEndDate { get; set; }

        public int? LoanDisbursedBy { get; set; }
        public DateTime? LoanDisbursedDate { get; set; }
    }

    // SALARY ADVANCE Application Model
    public class AdvanceSalaryApplicationModel
    {
        public int AdvSalaryId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public double? RequiredSalAdvAmount { get; set; }
        public double? EligibleSalAdvAmount { get; set; }
        public string SalAdvReason { get; set; }
        public DateTime? SalAdvRequiredDate { get; set; }
        public string AttachProof { get; set; }
        //public int? RepaymentMonths { get; set; }
        // public int RepaymentMonthsCount { get; set; }
        public double? SanctionedAmount { get; set; }
        // public double? RateOfInterest { get; set; }
        // public double? EMI { get; set; }
        public DateTime? SalAdvSanctionedDate { get; set; }
        public string RejectionReason { get; set; }
        public int? StatusId { get; set; }
        public string Status { get; set; }

        public bool IsPaymentProcess { get; set; }
        public DateTime? PaymentDate { get; set; }

        public double? DeductedSalAdvAmount1 { get; set; }
        public DateTime? DeductedSalAdv1Date { get; set; }

        public double? DeductedSalAdvAmount2 { get; set; }
        public DateTime? DeductedSalAdv2Date { get; set; }
    }

}