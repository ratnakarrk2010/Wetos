using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace WetosMVCMainApp.Models
{
    public class LeaveDetailsModel
    {
        public int LeaveID { get; set; }

        public int EmployeeID { get; set; }
    
        [Required]
        public string LeaveType { get; set; }

        [Required, DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0: dd-MM-yyyy}")]
        public DateTime FromDate { get; set; }

        [Required,DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0: dd-MM-yyyy}")]
        public DateTime ToDate { get; set; }

        public int AppliedDays { get; set; }

        public int ActualDays { get; set; }

        [Required]
        public string Purpose { get; set; }

        public string Status { get; set; }

        public string RejectReason { get; set; }
    }
}