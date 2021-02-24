using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WetosMVC.Models
{
    public class AnnualDisbusmentModel
    {
        public string EmployeeName { get; set; }

        public int Days { get; set; }

        public int EligibleAmt { get; set; }

        public int ActualAmt { get; set; }

        public DateTime Date { get; set; }
    }
}