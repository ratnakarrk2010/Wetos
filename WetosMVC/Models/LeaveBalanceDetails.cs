using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WetosMVCMainApp.Models
{
    public class LeaveBalanceDetailsModel
    {
        public int EmployeeID { get; set; }

        public int EmployeeNumber { get; set; }

        public string Prefix { get; set; }

        public string NameOfEmployee { get; set; }

        public string NameofCompany { get; set; }

        public string Gender { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0: dd-MM-yyyy}")]
        public string ActualDays { get; set; }

        [Required]
        public int Purpose { get; set; }

        public int AppliedDays { get; set; }



    }

    //Model Added By shraddha on 07 FEB 2017 start for Leave Calculation Report for flagship start
    public class LeaveCalculationSheetModel
    {
        public int EmployeeID { get; set; }

        public string EmployeeNumber { get; set; }

        public string NameOfEmployee { get; set; }

        //public string NameofCompany { get; set; } //COMMENTED BY SHRADDHA ON 25 JULY 2017 BECAUSE NOT REQUIRED

        public string UnpaidLeaves { get; set; }
       
        public string PaidLeaves { get; set; }

        public string AppliedDays { get; set; }

        public string FromDate { get; set; }

        public string ToDate { get; set; }

        public string FromDayStatus { get; set; }

        public string ToDayStatus { get; set; }

        public string LeaveType { get; set; }

        public string ConfirmDate { get; set; }

        //public string LeaveStatus { get; set; } //COMMENTED BY SHRADDHA ON 25 JULY 2017 BECAUSE NOT REQUIRED

        //public string Branch { get; set; } //COMMENTED BY SHRADDHA ON 25 JULY 2017 BECAUSE NOT REQUIRED

        // public string Company { get; set; } //COMMENTED BY SHRADDHA ON 25 JULY 2017 BECAUSE NOT REQUIRED

        //public string PreviousBalance { get; set; } //COMMENTED BY SHRADDHA ON 25 JULY 2017 BECAUSE NOT REQUIRED

        public string CurrentBalance { get; set; }

        public string LeaveUsed { get; set; }

        public string OpeningBalance { get; set; }
        //public string Encash { get; set; } //COMMENTED BY SHRADDHA ON 25 JULY 2017 BECAUSE NOT REQUIRED
        //public string Availed { get; set; } //COMMENTED BY SHRADDHA ON 25 JULY 2017 BECAUSE NOT REQUIRED

        public string CarryForward { get; set; }

        public string AllowedLeaves { get; set; }


        public string ClosingBalance { get; set; }

        

        public string LateMarks { get; set; }

        public string TotalLWP { get; set; }

        public string TotalDays { get; set; }

        public string TotalPayableDays { get; set; }
    }
    //Model Added By shraddha on 07 FEB 2017 start for Leave Calculation Report for flagship END
}
