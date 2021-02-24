using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;

namespace WetosMVC.Models
{
    public class HolidayInputModel
    {
        public int EmployeeId { get; set; }
        public int ReligionId { get; set; }
        public string Criteria { get; set; }
        public string DayStatus { get; set; }
        public string HLType { get; set; }
       
        [Required]
        public int CompanyId { get; set; }
      
        [Required]
        public DateTime FromDate { get; set; }
       
        [Required]
        public int Branchid { get; set; }
      
        [Required]
        public DateTime ToDate { get; set; }

        [StringLength(150, MinimumLength = 1, ErrorMessage = "Description cannot be longer than 150 characters.")]
        public string Description { get; set; }
    }
}