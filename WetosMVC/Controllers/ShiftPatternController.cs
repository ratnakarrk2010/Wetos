using System;
using System.Linq;
using System.Web.Mvc;
using WetosMVCMainApp.Models;
using System.Collections.Generic;

namespace WetosMVC.Controllers
{
    [SessionExpire] 
    [Authorize]
    public class ShiftPatternController : BaseController
    {
        //
        // GET: /ShiftPattern/
        

        public ActionResult Index()
        {
            List<WetosDB.ShiftPatternRule> ShiftPatternRule = WetosDB.ShiftPatternRules.ToList();

            return View(ShiftPatternRule);
        }

        public ActionResult Create()
        {
            AddAuditTrail("Clicked on Create New Shift Pattern");
            //var weekrule = WetosDB.vRuleTypeParameterDetailValues.Where(a => a.RuleTypeName == "WEEK").Select(a => new { id = a.RuleId, name = a.RuleShortName }).Distinct().ToList();
            //ViewBag.weekrule = new SelectList(weekrule, "id", "name");
            return View();
        }

        public JsonResult DrawPattern(int noofdays, string StartDay)
        {
            week_parameterModel wp = new week_parameterModel();
            ViewBag.noofdays = noofdays;
            var shiftrule = WetosDB.Shifts.Distinct().ToList();
            //wp.NO_OF_DAYS_IN_WEEK = "6";
            
            wp.FIRST_DAY_OF_WEEK = "MONDAY";
            wp.FIRST_WEEKLY_OFF = "SUNDAY";
            wp.IS_FIRST_WEEKLY_OFF_HALF_DAY = "NO";
            wp.OCCURRENCE_OF_FIRST_WEEKLY_OFF = "EVERY";
            wp.SECOND_WEEKLY_OFF = "SATURDAY";
            wp.IS_SECOND_WEEKLY_OFF_HALF_DAY = "NO";
            wp.OCCURRENCE_OF_SECOND_WEEKLY_OFF = "2,4";
            wp.NO_OF_WORKING_HOURS_PER_WEEK = "48";
            wp.NO_OF_WORKING_HOURS_TO_GET_COMPULSORY_OFF = "45";
            wp.NO_OF_HOURS_TO_BE_WORKED_IN_A_DAY_TO_CONSIDER_HALF_DAY = "3";
            wp.NO_OF_HOURS_TO_BE_WORKED_IN_A_DAY_TO_CONSIDER_FULL_DAY = "7";

            //Added by shalaka on 15th DEC 2017-- Start
            wp.SHIFT_SCHEDULE_DAY = StartDay;
            //Added by shalaka on 15th DEC 2017-- End

            ViewBag.week_parameter = wp;

            var obj = MvcHelpers.RenderPartialView(this, "DrawPattern", shiftrule);
            return Json(obj);
        }

        [HttpPost]
        public ActionResult Create(FormCollection fc)
        {
            int icount = Convert.ToInt32(fc.GetValue("hid_icount").AttemptedValue);
            string shift_pattern = null;
            //string shift_day_pattern = null;

            for (int i = 1; i <= icount; i++)
            {
                if (shift_pattern == null)
                {
                    shift_pattern = fc.GetValue("cboShift-" + i).AttemptedValue;
                    // shift_day_pattern = fc.GetValue("").AttemptedValue;
                }
                else
                {
                    shift_pattern = shift_pattern + '-' + fc.GetValue("cboShift-" + i).AttemptedValue;
                    //shift_day_pattern =shift_day_pattern +'-'+ fc.GetValue("").AttemptedValue; 
                }

            }

            WetosDB.ShiftPatternRule shiftpatt = new WetosDB.ShiftPatternRule();
            shiftpatt.ShiftPatternName = fc.GetValue("txtShiftPatternName").AttemptedValue;
            shiftpatt.ShiftPatternShortName = fc.GetValue("txtShortName").AttemptedValue;
            shiftpatt.ShiftPatternDescription = fc.GetValue("txtDescription").AttemptedValue;
            shiftpatt.NoofDays = Convert.ToInt32(fc.GetValue("txtNoOfDays").AttemptedValue);
            shiftpatt.WeekRuleId = 0;
            shiftpatt.ShiftPattern = shift_pattern;
            shiftpatt.DayPattern = fc.GetValue("hid_day_pattern").AttemptedValue;
            WetosDB.ShiftPatternRules.Add(shiftpatt);
            WetosDB.SaveChanges();
            AddAuditTrail("Created Shift Pattern as " + shiftpatt.ShiftPattern);

            // Updated "Leave Application Created successfully !!!", by Rajas on 20 MAY 2017
            TempData["AlertMessage"] = "Shift Pattern Created successfully !!!";

            return Redirect("Create");
        }

        public ActionResult Edit(int id)
        {
            try
            {
                WetosDB.ShiftPatternRule ShiftPatternRuleEditObj = WetosDB.ShiftPatternRules.Single(a => a.ShiftPatternRuleId == id);
                return View(ShiftPatternRuleEditObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in editing shift pattern due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );
                return RedirectToAction("Index");
            }

        }

        [HttpPost]
        public JsonResult DrawPatternForEdit(int? noofdays, int? ShiftPId) 
        {
            week_parameterModel wp = new week_parameterModel();

            ViewBag.noofdays = noofdays;
            WetosDB.ShiftPatternRule ShiftPatternRulDB = WetosDB.ShiftPatternRules.Where(a => a.ShiftPatternRuleId == ShiftPId).FirstOrDefault();

           ShiftPatternRuleModel ShiftPatternRuleModelObj = new ShiftPatternRuleModel();

            //List<WetosDB.Shift> ShiftPatternList = new List<WetosDB.Shift>();
            var ShiftPatternList = WetosDB.Shifts.Distinct().ToList(); // TO FILL VIEWBAG

            // VIEW BAG FOR DAY PATTERN            
            string[] DayPattern = ShiftPatternRulDB.DayPattern.Split('-');
            int i = 0;
            foreach (string Day in DayPattern)
            {
                DayShiftPatternModel DayShiftPatternModelModelObj = new DayShiftPatternModel();
                DayShiftPatternModelModelObj.DayNo = i;

                string DayName = string.Empty;
                //int DayNo = 
                switch (Day)
                {
                    case "MON":
                        DayName = "MONDAY";
                        break;
                    case "TUE":
                        DayName = "TUESDAY";
                        break;
                    case "WED":
                        DayName = "WEDNESDAY";
                        break;
                    case "THU":
                        DayName = "THURSDAY";
                        break;
                    case "FRI":
                        DayName = "FRIDAY";
                        break;
                    case "SAT":
                        DayName = "SATURDAY";
                        break;
                    case "SUN":
                        DayName = "SUNDAY";
                        break;
                }
                
                DayShiftPatternModelModelObj.DayName = DayName;
                ShiftPatternRuleModelObj.ShiftPatternList.Add(DayShiftPatternModelModelObj);

                if (i == 0)
                {
                    ShiftPatternRuleModelObj.ShiftScheduleStartDay = DayName;
                }
                 
                i++;
            }

            // VIEW BAG FOR SHIFT PATTERN
            string[] Shiftpattern = ShiftPatternRulDB.ShiftPattern.Split('-');

            i = 0;
            foreach (string Shift in Shiftpattern)
            {
                DayShiftPatternModel DayShiftPatternModelModelObj = ShiftPatternRuleModelObj.ShiftPatternList.Where(a => a.DayNo == i).FirstOrDefault();
                DayShiftPatternModelModelObj.ShiftName = Shift;
                i++;
            }

            ShiftPatternRuleModelObj.ShiftList = WetosDB.Shifts.Distinct().ToList();
            ShiftPatternRuleModelObj.NoofDays = Shiftpattern.Count();

            //wp.NO_OF_DAYS_IN_WEEK = "6";
            wp.FIRST_DAY_OF_WEEK = "MONDAY";
            wp.FIRST_WEEKLY_OFF = "SUNDAY";
            wp.IS_FIRST_WEEKLY_OFF_HALF_DAY = "NO";
            wp.OCCURRENCE_OF_FIRST_WEEKLY_OFF = "EVERY";
            wp.SECOND_WEEKLY_OFF = "SATURDAY";
            wp.IS_SECOND_WEEKLY_OFF_HALF_DAY = "NO";
            wp.OCCURRENCE_OF_SECOND_WEEKLY_OFF = "2,4";
            wp.NO_OF_WORKING_HOURS_PER_WEEK = "48";
            wp.NO_OF_WORKING_HOURS_TO_GET_COMPULSORY_OFF = "45";
            wp.NO_OF_HOURS_TO_BE_WORKED_IN_A_DAY_TO_CONSIDER_HALF_DAY = "3";
            wp.NO_OF_HOURS_TO_BE_WORKED_IN_A_DAY_TO_CONSIDER_FULL_DAY = "7";
            ViewBag.week_parameter = wp;

            var obj = MvcHelpers.RenderPartialView(this, "DrawPatternForEdit", ShiftPatternRuleModelObj);

            return Json(obj);            
        }

    }
}
