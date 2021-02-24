using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WetosMVC.Models
{
    public class AdvanceSalarySanctionModel
    {
        public int AdvSalarySanctionId { get; set; }

        public int AdvSalaryId { get; set; }

        public int EmployeeId { get; set; }

        public double? RequiredAdvsalAmount { get; set; }

        public double? EligibleAdvsalAmount { get; set; }

        public string AdvSalaryReason { get; set; }

        public string AttachProof { get; set; }

        public double? SanctionedAmount { get; set; }

        public DateTime? AdvSalarySanctionedDate { get; set; }
    }

    /// <summary>
    /// HoldSalaryModel
    /// </summary>
    public class HoldSalaryModel
    {
        public int HoldSalaryId { get; set; }

        [Required]
        [Display(Name = "User name")]
        public int? EmployeeId { get; set; }

        [Display(Name = "User name")]
        public string EmployeeName { get; set; }

        [Required]
        [Display(Name = "Reason")]
        public string Reason { get; set; }

        [Display(Name = "Status")]
        public int? HoldStatus { get; set; }

        public int? SalaryHoldBy { get; set; }
        public DateTime? SalaryHoldOn { get; set; }

        public int? SalaryUnHoldBy { get; set; }
        public DateTime? SalaryUnHoldOn { get; set; }

        //[Required]
        public int? FromSalaryMonth { get; set; }

        //[Required]
        public int? FromSalaryYear { get; set; }


        public int? ToSalaryMonth { get; set; }

        public int? ToSalaryYear { get; set; }

        public int? EmployeeResigned { get; set; }

        // ADDED BY MSJ ON 13 JAN 2019
        [Display(Name = "UnHold Reason")]
        public string UnHoldReason { get; set; }

        // ADDED BY MSJ ON 13 JAN 2019
        [Required]
        [Display(Name = "Hold Date")]
        public DateTime? FromDate { get; set; }

        // ADDED BY MSJ ON 13 JAN 2019
        [Display(Name = "UnHold Date")]
        public DateTime? ToDate { get; set; }
    }
}