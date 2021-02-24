using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WetosMVCMainApp.Models
{
    public class Company
    {
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyWebsite { get; set; }
    }
}