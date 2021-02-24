using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
using WetosDB;

namespace WetosMVCMainApp.Models
{
    /// <summary>
    /// ShiftPattern
    /// </summary>
    public class DayShiftPatternModel
    {
       public int DayNo { get; set; }
       public string DayName { get; set; }
       public string ShiftName { get; set; }      
    }

    /// <summary>
    /// week_parameter
    /// </summary>
    public class week_parameterModel
    {
        public string NO_OF_DAYS_IN_WEEK { get; set; }
        public string FIRST_DAY_OF_WEEK { get; set; }
        public string FIRST_WEEKLY_OFF { get; set; }
        public string IS_FIRST_WEEKLY_OFF_HALF_DAY { get; set; }
        public string OCCURRENCE_OF_FIRST_WEEKLY_OFF { get; set; }
        public string SECOND_WEEKLY_OFF { get; set; }
        public string IS_SECOND_WEEKLY_OFF_HALF_DAY { get; set; }
        public string OCCURRENCE_OF_SECOND_WEEKLY_OFF { get; set; }
        public string NO_OF_WORKING_HOURS_PER_WEEK { get; set; }
        public string NO_OF_WORKING_HOURS_TO_GET_COMPULSORY_OFF { get; set; }
        public string NO_OF_HOURS_TO_BE_WORKED_IN_A_DAY_TO_CONSIDER_HALF_DAY { get; set; }
        public string NO_OF_HOURS_TO_BE_WORKED_IN_A_DAY_TO_CONSIDER_FULL_DAY { get; set; }

        //Added by shalaka on 15th DEC 2017--- Start
        public string SHIFT_SCHEDULE_DAY { get; set; }
        //Added by shalaka on 15th DEC 2017--- End

    }

    /// <summary>
    /// ShiftPatternRule
    /// </summary>
    public partial class ShiftPatternRuleModel
    {
        public int ShiftPatternRuleId { get; set; }
        public string ShiftPatternName { get; set; }
        public string ShiftPatternShortName { get; set; }
        public string ShiftPatternDescription { get; set; }
        public int WeekRuleId { get; set; }
        public decimal NoofDays { get; set; }
        public string DayPattern { get; set; }
        public string ShiftPattern { get; set; }

        //Added by shalaka on 18th DEC 2017--- Start
        public string ShiftScheduleStartDay { get; set; }
        //public int ShiftScheduleStartDayId { get; set; }
        //Added by shalaka on 18th DEC 2017--- End

        public List<Shift> ShiftList { get; set; }

        public List<DayShiftPatternModel> ShiftPatternList { get; set; }

        /// <summary>
        /// CONSTRUCTOR
        /// </summary>
        public ShiftPatternRuleModel()
        {
            if (ShiftPatternList == null)
            {
                ShiftPatternList = new List<DayShiftPatternModel>();
            }
        }
    }


}