using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WetosMVC.Models
{
    public class OTCalculationMainModel
    {
        public List<OTCalculationModel> OTCalculationModelList { get; set; }


        public OTCalculationMainModel()
        {
            if (OTCalculationModelList == null)
            {
                OTCalculationModelList = new List<OTCalculationModel>();
            }
        }
    }
    //added by Sanket on 14-1-19 
    public class OTCalculationModel
    {
        public int SerialNo { get; set; }
        public String EmployeeName { get; set; }
        public int MonthlyGross { get; set; }
        public int Basic { get; set; }
        public int SpecialAllowance { get; set; }
        public int OldOTRate { get; set; }
        public double OTRate { get; set; }
        public double OTHrs { get; set; }
        public int OTAmount { get; set; }
    }

}