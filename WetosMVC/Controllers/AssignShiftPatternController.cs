using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using WetosDB;
using WetosMVCMainApp.Models;

namespace WetosMVC.Controllers
{
    [SessionExpire]
    [Authorize]
    public class shift_not_assigned_empids
    {
        public int emp_id { get; set; }
    }

    public class AssignShiftPatternController : BaseController
    {
        //
        // GET: /AssignShiftPattern/

        

        public ActionResult Index()
        {
            var shift_pattern_emp = WetosDB.ShiftPatternRules.ToList();

            var shiftPatternRule = WetosDB.ShiftPatternRules.Select(a => new { id = a.ShiftPatternRuleId, name = a.ShiftPatternName }).ToList();
            ViewBag.shiftPatternRule = new SelectList(shiftPatternRule, "id", "name");
            return View(shift_pattern_emp);
        }


        public ActionResult AssignShiftToShiftNotAssigned(string fromdate, string todate, string sn_emp_id, int empShiftPatternid, string shift_rule)
        {
            string[] fd = fromdate.Split('-');
            string[] td = todate.Split('-');
            DateTime fmdate = new DateTime(Convert.ToInt32(fd[2]), Convert.ToInt32(fd[1]), Convert.ToInt32(fd[0]), 0, 0, 0);
            DateTime tdate = new DateTime(Convert.ToInt32(td[2]), Convert.ToInt32(td[1]), Convert.ToInt32(td[0]), 23, 59, 0);
            DateTime startdate = fmdate;    //  This is for each employee

            string[] emp_ids = sn_emp_id.Split('-');
            int totalday = Convert.ToInt32((tdate - fmdate).TotalDays);
            int s_days = Convert.ToInt32(WetosDB.ShiftPatternRules.Where(a => a.ShiftPatternRuleId == empShiftPatternid).Select(a => a.NoofDays).FirstOrDefault());

            string get_shiftdaypattern = WetosDB.ShiftPatternRules.Where(a => a.ShiftPatternRuleId == empShiftPatternid).Select(a => a.DayPattern).FirstOrDefault();

            var get_shiftpattern = WetosDB.ShiftPatternRules.Where(a => a.ShiftPatternRuleId == empShiftPatternid).Select(a => a.ShiftPattern).FirstOrDefault();

            string[] shift_days = get_shiftdaypattern.Split('-');

            string[] shift_patts = get_shiftpattern.Split('-');

            List<string> ShiftDayList = shift_days.ToList();

            List<string> ShiftPatternList = shift_patts.ToList();

            string F_days = shift_days[0];
            int day_count = 0;
            int added_days = 0;
            string error_line = "";
            //string start_shift = WetosDB.vRuleTypeMasters.Where(a => a.ruleid == shift_ruleid).Select(a => a.rulename).FirstOrDefault();
            //int i = 0;
            int insert_flag = 0; //to check whether record inserted in shiftschedule as per pattern shift-day and start day selected
            // string from_day = fmdate.DayOfWeek.ToString();
            string first_day = fmdate.DayOfWeek.ToString().Substring(0, 3).ToUpper();
            try
            {

                //CODE CHANGED BY SHRADDHA  ON 17 MAY 2017
                WetosDB.ShiftPatternRule ShiftPatternRuleObj = WetosDB.ShiftPatternRules.Where(a => a.ShiftPatternRuleId == empShiftPatternid).FirstOrDefault();

                if (ShiftPatternRuleObj != null)
                {

                }

                // error_line = "1";
                foreach (var id in emp_ids)
                {
                    int EmpGrpId = Convert.ToInt32(id);
                    List<EmployeeGroupDetail> EmployeeGroupDetailList = WetosDB.EmployeeGroupDetails.Where(a => a.EmployeeGroup.EmployeeGroupId == EmpGrpId).ToList();
                    if (EmployeeGroupDetailList.Count > 0)
                    {
                        foreach (var EmployeeGroupDetailObj in EmployeeGroupDetailList)
                        {
                            Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == EmployeeGroupDetailObj.Employee.EmployeeId).FirstOrDefault();

                            //if (first_day == days && shift_rule == shift_patts[day_count])

                            error_line = "2";
                            string d = shift_days[day_count];
                            error_line = "3";
                            string sd = shift_patts[day_count];
                            error_line = "4";

                            int emp_id = Convert.ToInt32(id);

                            added_days++;
                            ShiftSchedule ss = new ShiftSchedule();
                            ss.EmployeeGroupId = Convert.ToInt32(id);
                            ss.EmployeeId = EmployeeObj.EmployeeId; //0;
                            ss.RotationId = 0;
                            ss.ShiftMonth = fmdate.Month;
                            ss.ShiftYear = fmdate.Year;
                            ss.BranchId = EmployeeObj.BranchId;   //0;
                            ss.CompanyId = EmployeeObj.CompanyId; //0;

                            WetosDB.ShiftSchedules.Add(ss);
                            WetosDB.SaveChanges();

                            TimeSpan ts = (Convert.ToDateTime(tdate)) - (Convert.ToDateTime(fmdate));
                            float AppliedDay = ts.Days;


                            // ADDED BY SHRADDHA ON 17 MAY 2017 START

                            #region CODE TO UPDATE SHIFT SCHEDULE

                            // CODE BY MSJ
                            DateTime StartDate = fmdate;
                            DateTime EndDate = tdate;

                            //GET DAY OF START DATE
                            string StartDay = fmdate.ToString("ddd");
                            StartDay = StartDay.ToUpper();
                            //GET INDEX OF START DAY IN SELECTED PATTERN
                            //FOR EG. IF PATTERN STARTS WITH MONDAY AND FROM DATE IS WEDNESDAY THEN X WILL RETURN 3 RD INDEX
                            int x = ShiftDayList.IndexOf(StartDay);

                            //LAST COUNT OF SHIFTPATTERNLIST
                            int y = ShiftPatternList.Count() - 1;



                            List<string> MonthList = MyFunction(StartDate, EndDate);
                            MonthList.Add(EndDate.ToString("yyyy,MM", CultureInfo.InvariantCulture));


                            //
                            foreach (string MonthYear in MonthList)
                            {

                                List<string> ColumnStringList = new List<string>();
                                List<string> ValueStringList = new List<string>();

                                string[] StartDateArray = MonthYear.Split(',');
                                int TmpStartYear = Convert.ToInt32(StartDateArray[0].Trim());
                                int TmpStartMonth = Convert.ToInt32(StartDateArray[1].Trim());

                                DateTime StartDateObj = new DateTime();
                                DateTime EndDateObj = new DateTime();

                                if (MonthList.Count > 1)
                                {
                                    // START MONTH
                                    if (StartDate.ToString("yyyy,MM", CultureInfo.InvariantCulture) == MonthYear) // FIRST MONTH
                                    {
                                        // GET LAST DATE OF MONTH 

                                        StartDateObj = StartDate;

                                        DateTime TempStartDateObj = new DateTime(StartDateObj.Year, StartDateObj.Month, 1);
                                        EndDateObj = TempStartDateObj.AddMonths(1).AddDays(-1);

                                        for (int j = StartDate.Day; j <= EndDateObj.Day; j++)
                                        {
                                            ColumnStringList.Add("Day" + j.ToString());

                                            if (x > y)
                                            {
                                                x = 0;
                                                //x = ShiftDayList.IndexOf(StartDay);
                                            }


                                            ValueStringList.Add("'" + shift_patts[x].ToString() + "'");


                                            x++;
                                            //ValueStringList.Add("'Day" + j + "'".ToString());
                                        }
                                    }
                                    else if (EndDate.ToString("yyyy,MM", CultureInfo.InvariantCulture) == MonthYear) // LAST MONTH     
                                    {
                                        // GET LAST DATE OF MONTH 

                                        EndDateObj = EndDate;
                                        StartDateObj = new DateTime(EndDateObj.Year, EndDateObj.Month, 1);
                                        for (int j = 1; j <= EndDate.Day; j++)
                                        {
                                            // UPDATE
                                            ColumnStringList.Add("Day" + j.ToString());

                                            if (x > y)
                                            {
                                                x = 0;
                                                //x = ShiftDayList.IndexOf(StartDay);
                                            }

                                            ValueStringList.Add("'" + shift_patts[x].ToString() + "'");


                                            x++;
                                            //ValueStringList.Add("'Day" + j + "'".ToString());
                                        }
                                    }
                                    else
                                    {
                                        string[] ABCD = MonthYear.Split(',');
                                        int ABCDYear = Convert.ToInt32(ABCD[0]);
                                        int ABCDMonth = Convert.ToInt32(ABCD[1]);

                                        DateTime StartMonth = new DateTime(ABCDYear, ABCDMonth, 1);
                                        DateTime EndMonth = StartMonth.AddMonths(1).AddDays(-1);

                                        EndDateObj = StartMonth;
                                        StartDateObj = EndMonth;

                                        // GET LAST DATE OF MONTH 
                                        for (int j = 1; j <= EndMonth.Day; j++)
                                        {
                                            // UPDATE
                                            ColumnStringList.Add("Day" + j.ToString());

                                            if (x > y)
                                            {
                                                //x = ShiftDayList.IndexOf(StartDay);
                                                x = 0;
                                            }

                                            ValueStringList.Add("'" + shift_patts[x].ToString() + "'");


                                            x++;
                                            //ValueStringList.Add("'Day" + j + "'".ToString());
                                        }
                                    }
                                }
                                else
                                {
                                    // START MONTH
                                    if (StartDate.ToString("yyyy,MM", CultureInfo.InvariantCulture) == MonthYear && EndDate.ToString("yyyy,MM", CultureInfo.InvariantCulture) == MonthYear)
                                    {
                                        // GET LAST DATE OF MONTH 

                                        EndDateObj = EndDate;
                                        StartDateObj = StartDate;
                                        for (int j = StartDate.Day; j <= EndDate.Day; j++)
                                        {
                                            // UPDATE
                                            ColumnStringList.Add("Day" + j.ToString());


                                            if (x > y)
                                            {
                                                //x = ShiftDayList.IndexOf(StartDay);
                                                x = 0;
                                            }

                                            ValueStringList.Add("'" + shift_patts[x].ToString() + "'");


                                            x++;

                                            // ValueStringList.Add("'Day" + j + "'".ToString());
                                        }
                                    }
                                }

                                // 
                                string ColumnStr = string.Join(",", ColumnStringList.ToArray());
                                string ValueStr = string.Join(",", ValueStringList.ToArray());

                                int ColValue = ColumnStringList.Count();

                                var check_shft = WetosDB.ShiftSchedulePatterns.Where(a => a.EffectiveStartDate == fmdate 
                                    && a.EffectiveEndDate == tdate && a.EmployeeId == EmployeeGroupDetailObj.Employee.EmployeeId).FirstOrDefault();

                                DateTime StartDateToUseInQuery = new DateTime(StartDateObj.Year, StartDateObj.Month, StartDateObj.Day, 0, 0, 0);
                                DateTime EndDateToUseInQuery = new DateTime(EndDateObj.Year, EndDateObj.Month, EndDateObj.Day, 23, 59, 0);

                                #region INSERT INTO ShiftSchedulePattern

                                // Updated by Rajas on 17 AUGUST 2017 START

                                // GET existing schedule entries
                                ShiftSchedulePattern ShiftSchedulePatternExistObj = WetosDB.ShiftSchedulePatterns.Where(a => a.EmployeeId == EmployeeGroupDetailObj.Employee.EmployeeId
                                    && a.ShiftMonth == TmpStartMonth && a.ShiftYear == TmpStartYear).FirstOrDefault();

                                string command = string.Empty;

                                if (ShiftSchedulePatternExistObj == null)
                                {
                                    command = string.Format(@"insert into ShiftSchedulePattern (EmployeeId,EmployeeGroupId,ShiftMonth,ShiftYear,EffectiveStartDate,EffectiveEndDate,DayCount,BranchId,CompanyId,ShiftPatternId,{16}) values ({0},{1},{2},{3},'{4}/{5}/{6} {7}','{8}/{9}/{10} {11}',{12},{13},{14},{15},{17});"
                                        , EmployeeGroupDetailObj.Employee.EmployeeId, Convert.ToInt32(id), TmpStartMonth, TmpStartYear, StartDateToUseInQuery.Year, StartDateToUseInQuery.Month, StartDateToUseInQuery.Day, "00:00", EndDateToUseInQuery.Year, EndDateToUseInQuery.Month, EndDateToUseInQuery.Day, "23:59", s_days, EmployeeObj.BranchId, EmployeeObj.CompanyId, empShiftPatternid, ColumnStr, ValueStr);

                                    //WetosDB.ExecuteStoreQuery<List<string>>(command, "").ToList();

                                }
                                else
                                {
                                    for (int k = 0; k < ColValue; k++)
                                    {
                                        command = string.Format(@"update ShiftSchedulePattern SET {0} = {1} where ShiftPatternId={2};"
                                                , ColumnStringList[k], ValueStringList[k], ShiftSchedulePatternExistObj.ShiftSchedulePatternId);

                                        //WetosDB.ExecuteStoreQuery<List<string>>(command, "").ToList();

                                    }

                                }
                                // Updated by Rajas on 17 AUGUST 2017 END

                                #endregion INSERT INTO ShiftSchedulePattern

                            }

                            // Added by Rajas on 17 AUGUST 2017 START
                            AddAuditTrail("Shift schedule pattern updated for EmployeeGroupId " + EmpGrpId + " between " + fmdate + " and " + tdate);
                            AddAuditTrail("Shift schedule pattern updated by " + Session["Id"] + " - " + Session["Username"]);
                            // Added by Rajas on 17 AUGUST 2017 END

                            #endregion


                            // ADDED BY SHRADDHA ON 17 MAY 2017 END

                            #region
                            //ShiftSchedulePattern ssp = new ShiftSchedulePattern();

                            //ssp.EmployeeGroupId = Convert.ToInt32(id);
                            //ssp.EmployeeId = EmployeeGroupDetailObj.EmployeeId;
                            //ssp.ShiftMonth = fmdate.Month;
                            //ssp.ShiftYear = fmdate.Year;
                            //ssp.EffectiveStartDate = fmdate;
                            //DateTime GetFromDate = new DateTime(fmdate.Year, fmdate.Month, 1);
                            //DateTime ToDate = GetFromDate.AddMonths(1).AddDays(-1);
                            //if (tdate > ToDate)
                            //{
                            //    ssp.EffectiveEndDate = ToDate;
                            //}
                            //else
                            //{
                            //    ssp.EffectiveEndDate = tdate;
                            //}
                            //ssp.DayCount = s_days;
                            //ssp.BranchId = 0;
                            //ssp.CompanyId = 0;
                            //if (EmployeeObj != null)
                            //{
                            //    ssp.BranchId = EmployeeObj.CompanyId;
                            //    ssp.CompanyId = EmployeeObj.BranchId;
                            //}
                            //ssp.ShiftPatternId = empShiftPatternid;
                            ////ss.shiftrul keid = WetosDB.RuleMasters.Where(a => a.RuleShortName == sd).Select(a => a.RuleId).FirstOrDefault();
                            //// ss.ShiftDate = fmdate;
                            //WetosDB.ShiftSchedulePatterns.Add(ssp);
                            //WetosDB.SaveChanges();
                            #endregion

                            insert_flag = 1;

                            #region Commented code
                            //s_day = Convert.ToInt32(o_d[0]) + 1;
                            //fmdate = fmdate.AddDays(1);
                            //day_count++;
                            //if (s_days == day_count)
                            //{
                            //    day_count = 0;
                            //}
                            //if (added_days == totalday)
                            //{
                            //    i = 0;
                            //    added_days = 0;
                            //    day_count = 0;
                            //    fmdate = startdate;
                            //    break;
                            //}
                            #endregion
                        }
                    }


                }
            }
            catch (System.Exception ex)
            {
                return Json(error_line + " - " + ex.Message);
            }
            return Json(new
            {
                EmployeeList = MvcHelpers.RenderPartialView(this, "GetEmployeeName", null),
                insert_flag = insert_flag
            });
        }

        public JsonResult GetEmployeeName(int empShiftPatternid, string fromdate)
        {
            string[] fmdate = fromdate.Split('-');
            DateTime f_date = Convert.ToDateTime(fmdate[2] + '-' + fmdate[1] + '-' + fmdate[0] + " 00:00");
            var emp_duty_list = WetosDB.EmployeeGroups.ToList();

            List<ShiftSchedulePattern> shift_master = new List<ShiftSchedulePattern>();

            string shift_pattern_val = WetosDB.ShiftPatternRules.Where(a => a.ShiftPatternRuleId == empShiftPatternid).Select(a => a.ShiftPattern).FirstOrDefault();
            string[] shf_pat = shift_pattern_val.Split('-');

            foreach (var sp in shf_pat)
            {
                var shift_rule = WetosDB.ShiftSchedulePatterns.Where(a => a.ShiftPatternId == empShiftPatternid).FirstOrDefault();
                if (shift_rule != null)
                {
                    shift_master.Add(shift_rule);
                }
            }

            ViewBag.shift_rule = shift_master;
            ViewBag.ruleid = empShiftPatternid;
            if (emp_duty_list.Count() != 0)
            {
                return Json(new
                {
                    EmployeeList = MvcHelpers.RenderPartialView(this, "GetEmployeeName", emp_duty_list)
                });
            }
            else
            {
                return Json(new { EmployeeList = "" });
            }
        }

        #region MYFUNCTION by MSJ
        private static List<string> MyFunction(DateTime date1, DateTime date2)
        {

            var listOfMonths = new List<string>();
            if (date1.CompareTo(date2) == 1)
            {
                var temp = date2;
                date2 = date1;
                date1 = temp;
            }

            var mosSt = date1.Month;
            var mosEn = date2.Month;
            var yearSt = date1.Year;
            var yearEn = date2.Year;

            while (mosSt < mosEn || yearSt < yearEn)
            {
                var temp = new DateTime(yearSt, mosSt, 1);
                listOfMonths.Add(temp.ToString("yyyy,MM", CultureInfo.InvariantCulture));
                mosSt++;
                if (mosSt < 13) continue;
                yearSt++;
                mosSt = 1;
            }

            return listOfMonths;
        }
        #endregion

        //public JsonResult GetShiftOnFirstDay(string fromdate, string todate, int empShiftPatternid)
        //{
        //    string[] fd = fromdate.Split('-');
        //    string[] td = todate.Split('-');
        //    DateTime fmdate = Convert.ToDateTime(fd[2] + "-" + fd[1] + "-" + fd[0] + " 00:00");
        //    DateTime tdate = Convert.ToDateTime(td[2] + "-" + td[1] + "-" + td[0] + " 23:59");
        //    DateTime lastdate = Convert.ToDateTime(fd[2] + "-" + fd[1] + "-" + fd[0] + " 00:00").AddDays(-1);

        //    DateTime startdate = fmdate;

        //    string f_day = fmdate.DayOfWeek.ToString();
        //    //List<shift_not_assigned_empids> shift_not_assigned_empid = new List<shift_not_assigned_empids>();
        //    string first_day = fmdate.DayOfWeek.ToString().Substring(0, 3);
        //    int i = 0;
        //    int added_days = 0;
        //    int totalday = Convert.ToInt32((tdate - fmdate).TotalDays) + 1;
        //    string new_days = null;
        //    //List<vEmployeeDetail> Emp = WetosDB.vEmployeeDetails.Where(a => a.shiftpatternruleid == empShiftPatternid).ToList();
        //    List<MarsData.sp_get_List_Employees_With_Last_Duty_Result> Emp = WetosDB.sp_get_List_Employees_With_Last_Duty(empShiftPatternid, fmdate).Where(a => a.NewOrOld != "*").ToList();
        //    int emp_count = Emp.Where(a => a.NewOrOld != "*").Count();
        //    int flag = 0, flag1 = 0;
        //    int fmday = fmdate.Day;
        //    foreach (var e in Emp)
        //    {
        //        var shift = "";
        //        var last_shift = "";
        //        var shf = WetosDB.ShiftSchedules.Where(a => a.EmployeeId == e.EmployeeId && a.ShiftDate == lastdate).FirstOrDefault();
        //        for (int d = 1; d <= 6; d++)
        //        {

        //            DateTime lds = fmdate.AddDays(-d);
        //            shf = WetosDB.ShiftSchedules.Where(a => a.EmployeeId == e.EmployeeId && a.ShiftDate == lds).FirstOrDefault();
        //            shift = WetosDB.ShiftSchedules.Where(a => a.EmployeeId == e.EmployeeId && a.ShiftDate == lds).Select(a => a.Roster).FirstOrDefault();

        //            if (shift != "WO" && shift != null)
        //            {
        //                last_shift = WetosDB.ShiftSchedules.Where(a => a.EmployeeId == e.EmployeeId && a.ShiftDate == lds).Select(a => a.Roster).FirstOrDefault();
        //                break;
        //            }
        //        }

        //        if (shift == "")
        //        {
        //            shift = WetosDB.ShiftSchedules.Where(a => a.EmployeeId == e.EmployeeId && a.ShiftDate == lastdate).Select(a => a.Roster).FirstOrDefault();
        //        }
        //        fmdate = startdate;
        //        DateTime shf_date = Convert.ToDateTime(lastdate);
        //        string shf_day = shf_date.DayOfWeek.ToString().Substring(0, 3);
        //        int s_days = Convert.ToInt32(WetosDB.ShiftPatternRules.Where(a => a.ShiftPatternRuleId == empShiftPatternid).Select(a => a.NoofDays).FirstOrDefault());
        //        string get_shiftdaypattern = WetosDB.ShiftPatternRules.Where(a => a.ShiftPatternRuleId == empShiftPatternid).Select(a => a.DayPattern).FirstOrDefault();
        //        var get_shiftpattern = WetosDB.ShiftPatternRules.Where(a => a.ShiftPatternRuleId == empShiftPatternid).Select(a => a.ShiftPattern).FirstOrDefault();
        //        string[] shift_days = get_shiftdaypattern.Split('-');
        //        string[] shift_patts = get_shiftpattern.Split('-');
        //        string F_days = shift_days[0];
        //        string new_shift_days = null;
        //        int day_count = 0;
        //        string error_line = "";
        //        try
        //        {
        //            if (shf != null)
        //            {
        //                if (added_days <= totalday)
        //                {

        //                    for (int dd = 0; dd < s_days; dd++)
        //                    {

        //                        //error_line = "1";
        //                        string d = shift_days[dd];
        //                        string sd = shift_patts[dd];
        //                        // string[] s_date = fmdate.ToString().Split(' ');
        //                        // string[] o_d = s_date[0].Split('-');
        //                        // error_line = "2";


        //                        if (first_day.ToUpper() == d && shift == sd || i == 1)
        //                        {
        //                            if (new_days != null)
        //                            {
        //                                new_days = new_days + "-" + d;
        //                                new_shift_days = new_shift_days + "-" + sd;

        //                            }
        //                            else
        //                            {
        //                                new_days = d;
        //                                new_shift_days = sd;
        //                            }

        //                            i = 1;

        //                            added_days++;

        //                            if (added_days == totalday)
        //                            {
        //                                i = 0;
        //                                //  dd = 0;
        //                                added_days = 0;
        //                                day_count = 0;
        //                                fmdate = startdate;
        //                                break;
        //                            }

        //                            error_line = "3";
        //                            ShiftSchedule ss = new ShiftSchedule();
        //                            ss.EmployeeId = e.EmployeeId;
        //                            ss.EmployeeGroupId = 0;
        //                            ss.ShiftMonth = fmdate.Month;
        //                            ss.ShiftYear = fmdate.Year;
        //                            ss.ShiftDay = fmdate.Day;
        //                            ss.Roster = sd;
        //                            ss.BranchId = 0;
        //                            ss.CompanyId = 0;
        //                            ss.shiftruleid = WetosDB.RuleMasters.Where(a => a.RuleShortName == sd).Select(a => a.RuleId).FirstOrDefault();
        //                            ss.ShiftDate = fmdate;
        //                            WetosDB.ShiftSchedules.Add(ss);
        //                            WetosDB.SaveChanges();
        //                            //s_day = Convert.ToInt32(o_d[0]) + 1;
        //                            fmdate = fmdate.AddDays(1);

        //                        }

        //                        day_count++;
        //                        if (day_count == s_days)
        //                        {
        //                            dd = -1;
        //                        }

        //                    }
        //                }
        //            }
        //            //flag++;
        //        }
        //        catch (System.Exception ex)
        //        {
        //            return Json(error_line + " - " + ex.Message);
        //        }

        //    }

        //    return Json("");
        //}

    }
}
