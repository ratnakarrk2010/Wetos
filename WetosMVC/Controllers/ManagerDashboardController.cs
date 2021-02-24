using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WetosDB;
using WetosMVCMainApp.Models;

namespace WetosMVC.Controllers
{
    [SessionExpire] 
    [Authorize]
    public class ManagerDashboardController : BaseController
    {
        

        /// <summary>
        /// Updated by Rajas on 27 APRIL 2017
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            try
            {
                int EmpId = Convert.ToInt32(Session["EmployeeNo"]);

                Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == EmpId).FirstOrDefault();

                DailyTransactionAttendanceStatus DailyTransactionAttendanceStatusObj = new DailyTransactionAttendanceStatus();

                var MonthList = WetosDB.DropdownDatas.Where(a => a.TypeId == 7).Select(a => new { Value = a.Value, Text = a.Text }).ToList();

                ViewBag.MonthListVB = new SelectList(MonthList, " Value", "Text").ToList();

                //GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == "Current Financial Year").FirstOrDefault();

                //ABOVE LINE COMMENTED AND BELOW LINE IS ADDED BY SHALAKA ON 13TH DEC 2017 
                GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.CurrentFinancialYear).FirstOrDefault();

                if (GlobalSettingObj != null)
                {
                    // MODIFIED AND ADDED CODE BY MSJ ON 08 JAN 2018 START
                    //var YearList = WetosDB.FinancialYears.Where(a => a.Company.CompanyId == LoggedInEmployee.CompanyId 
                    //    && a.Branch.BranchId == LoggedInEmployee.BranchId 
                    //    && a.FinancialName.Trim() == GlobalSettingObj.SettingValue.Trim()).Select(a => 
                    //        new { FinancialId = a.FinancialYearId, FinancialName = a.FinancialName }).Distinct().ToList();

                    List<FinancialYear> FYObjList = WetosDB.FinancialYears.ToList();
                    List<string> FYStrList = new List<string>();
                    foreach (FinancialYear CurrentFYObj in FYObjList)
                    {
                        if (CurrentFYObj.StartDate.Year != CurrentFYObj.EndDate.Year) // DIFF YEARS
                        {
                            FYStrList.Add(CurrentFYObj.StartDate.Year.ToString());
                            FYStrList.Add(CurrentFYObj.EndDate.Year.ToString());

                            //SelectListItemList.Add(new SelectListItem { Text = CurrentFYObj.StartDate.Year.ToString(), Value = CurrentFYObj.StartDate.Year.ToString() });
                            //SelectListItemList.Add(new SelectListItem { Text = CurrentFYObj.EndDate.Year.ToString(), Value = CurrentFYObj.EndDate.Year.ToString() });                           
                        }
                        else
                        {
                            FYStrList.Add(CurrentFYObj.StartDate.Year.ToString());
                            //SelectListItemList.Add(new SelectListItem { Text = CurrentFYObj.StartDate.Year.ToString(), Value = CurrentFYObj.StartDate.Year.ToString() });
                        }
                    }

                    List<SelectListItem> SelectListItemList = new List<SelectListItem>();
                    List<string> FinalFYStrList = FYStrList.Distinct().ToList();
                    foreach (string CFYStr in FYStrList)
                    {
                        SelectListItemList.Add(new SelectListItem { Text = CFYStr, Value = CFYStr });
                    }
                    var FYListVar = SelectListItemList;
                    ViewBag.YearListVB = FYListVar;

                    // MODIFIED AND ADDED CODE BY MSJ ON 08 JAN 2018 END
                }
                else
                {
                    var YearList = new List<FinancialYear>();
                    ViewBag.YearListVB = new SelectList(YearList, " FinancialId", "FinancialName").ToList();

                    Information("Inconsistent Financial Year data, Please verify and try again!");
                }

                return View(DailyTransactionAttendanceStatusObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Errror in Attendance status view due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                return View("Error");
            }

        }


        /// <summary>
        /// ADDED ON 11 JAN 2017 TO TRY CALENDAR STRUCTURE EVENT PUSH BY SHRADDHA
        /// </summary>
        /// <returns></returns>
        public ActionResult CalendarIndex()
        {
            return View();
        }

        public ActionResult statisticsforselectedMonth()
        {

            return View();
        }

        /// <summary>
        /// List of employees reporting to me
        /// Added by Rajas on 10 JAN 2017
        /// </summary>
        /// <returns></returns>
        public ActionResult EmployeeReportingIndex()
        {
            int EmpId = Convert.ToInt32(Session["EmployeeNo"]);
            // List<SP_EmployeeDetailsOnEmployeeMaster_Result> EmployeeListObj = WetosDB.SP_EmployeeDetailsOnEmployeeMaster().Where(a => a.EmployeeId == EmpId).ToList();

            // Updated by Rajas on 2 MARCH 2017 for list of active employees

            #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
            int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            //List<Employee> EmployeeObj = WetosDB.Employees.Where(a => a.ConfirmDate.Value.Month == CurrentMonth && a.ConfirmDate.Value.Year == CurrentYear && a.EmployeeId == EmpId).ToList();
            var EmployeeListObj = WetosDB.SP_VwActiveEmployee(EmployeeId).Where(a => a.EmployeeReportingId == EmpId && (a.ActiveFlag == null || a.ActiveFlag == true)).ToList();
            #endregion

            return View(EmployeeListObj);
        }


        //Added By Shraddha on 27 DEC 2016 to show List of Attendance after login instead of calender

        //CODE CHANGED BY SHRADDHA ON 09 FEB 2017 START
        public ActionResult DailyTransactionIndex(int FinancialYearId, int MonthId)  // (DateTime FromDate, DateTime ToDate)
        {
            // Extra green code removed 
            // Updated by Rajas on 27 APRIL 2017
            int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);

            FinancialYear FinancialYearObj = WetosDB.FinancialYears.Where(a => a.FinancialYearId == FinancialYearId).FirstOrDefault();
            int Year = FinancialYearId; //2017; REMOVED HARD CODED 2017 BY SHRADDHA ON 02 FEB 2018
            if (FinancialYearObj != null)
            {
                if (MonthId >= 4)
                {
                    Year = FinancialYearObj.StartDate.Year;
                }
                else
                {
                    Year = FinancialYearObj.EndDate.Year;
                }
            }

            //ADDED ORDER BY DATE CONDITION BY SHRADDHA ON 06 FEB 2017 START
            // Updated by Rajas on 27 APRIL 2017
            List<WetosDB.SP_GetAttendanceIndex_Result> GetAttendanceList = WetosDB.SP_GetAttendanceIndex(EmployeeId, MonthId, Year).ToList(); // FromDate.Month, FromDate.Year
            //ADDED ORDER BY DATE CONDITION BY SHRADDHA ON 06 FEB 2017 END


            List<LeaveApplication> LeaveApplicationListForLoggedInEmployee = WetosDB.LeaveApplications.Where(a => a.EmployeeId == EmployeeId).ToList();
            List<CompOffApplication> CompOffApplicationListForLoggedInEmployee = WetosDB.CompOffApplications.Where(a => a.EmployeeId == EmployeeId).ToList();
            List<ODTour> ODTOURApplicationListForLoggedInEmployee = WetosDB.ODTours.Where(a => a.EmployeeId == EmployeeId).ToList();
            Session["LeaveApplicationIsAvailable"] = false;
            Session["CompOffApplicationIsAvailable"] = false;
            Session["ODTOURApplicationIsAvailable"] = false;
            if (LeaveApplicationListForLoggedInEmployee.Count > 0)
            {
                foreach (var i in GetAttendanceList)
                {
                    foreach (var item in LeaveApplicationListForLoggedInEmployee)
                    {
                        for (DateTime CurrentDate = item.FromDate; CurrentDate.Date <= item.ToDate; CurrentDate = CurrentDate.AddDays(1))
                        {
                            if (i.TranDate == CurrentDate && item.StatusId != 2)
                            {
                                //i.StatusName = "Applied Leave";
                                i.Remark = "Applied Leave" + "/" + item.Status;
                                //Session["LeaveApplicationIsAvailable"] = true;
                            }
                            else if (i.TranDate == CurrentDate && item.StatusId == 2)
                            {

                                i.Remark = item.Status;
                            }
                        }
                    }
                }
            }
            if (CompOffApplicationListForLoggedInEmployee.Count > 0)
            {
                foreach (var i in GetAttendanceList)
                {
                    foreach (var item in CompOffApplicationListForLoggedInEmployee)
                    {
                        for (DateTime CurrentDate = Convert.ToDateTime(item.FromDate); CurrentDate.Date <= item.ToDate; CurrentDate = CurrentDate.AddDays(1))
                        {
                            if (i.TranDate == CurrentDate)
                            {
                                //i.StatusName = "Applied CompOff";
                                i.Remark = "Applied CompOff" + "/" + item.Status;
                                //Session["CompOffApplicationIsAvailable"] = true;
                            }
                            else if (i.TranDate == CurrentDate && item.StatusId == 2)
                            {
                                i.Remark = item.Status;
                            }
                        }
                    }
                }
            }
            if (ODTOURApplicationListForLoggedInEmployee.Count > 0)
            {
                foreach (var i in GetAttendanceList)
                {
                    foreach (var item in ODTOURApplicationListForLoggedInEmployee)
                    {
                        for (DateTime CurrentDate = Convert.ToDateTime(item.FromDate); CurrentDate.Date <= item.ToDate; CurrentDate = CurrentDate.AddDays(1))
                        {
                            if (i.TranDate == CurrentDate)
                            {

                                i.Remark = "Applied OD" + "/" + item.Status;
                                //Session["ODTOURApplicationIsAvailable"] = true;
                            }
                            else if (i.TranDate == CurrentDate && item.StatusId == 2)
                            {
                                i.Remark = item.Status;
                            }
                        }
                    }
                }
            }
            int TotalDays = Convert.ToInt32(GetAttendanceList.Count());
            ViewBag.TotalDays = TotalDays;
            int AbsentDays = Convert.ToInt32(GetAttendanceList.Where(a => a.StatusName == "AAAA").Count());
            ViewBag.AbsentDays = AbsentDays;
            int LateByDays = Convert.ToInt32(GetAttendanceList.Where(a => a.StatusName.Contains("AAPP")).Count());
            ViewBag.LateByDays = LateByDays;
            int WeekOffDays = Convert.ToInt32(GetAttendanceList.Where(a => a.Status.Trim() == "WOWO" || a.StatusName.Trim() == "WO").Count());
            ViewBag.WeekOffDays = WeekOffDays;
            int PresentDays = TotalDays - AbsentDays;
            ViewBag.PresentDays = PresentDays;

            return PartialView(GetAttendanceList);
        }
        //CODE CHANGED BY SHRADDHA ON 09 FEB 2017 END

        // ADDED BY MSJ ON 12 DEC 2018 START
        public ActionResult EmpDailyTransaction(int EmployeeId = 0)  // (DateTime FromDate, DateTime ToDate)
        {
            var MonthList = WetosDB.DropdownDatas.Where(a => a.TypeId == 7).Select(a => new { Value = a.Value, Text = a.Text }).ToList();

            ViewBag.MonthListVB = new SelectList(MonthList, " Value", "Text").ToList();

            //GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == "Current Financial Year").FirstOrDefault();

            //ABOVE LINE COMMENTED AND BELOW LINE IS ADDED BY SHALAKA ON 13TH DEC 2017 
            GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.CurrentFinancialYear).FirstOrDefault();

            if (GlobalSettingObj != null)
            {
                // MODIFIED AND ADDED CODE BY MSJ ON 08 JAN 2018 START
                //var YearList = WetosDB.FinancialYears.Where(a => a.Company.CompanyId == LoggedInEmployee.CompanyId 
                //    && a.Branch.BranchId == LoggedInEmployee.BranchId 
                //    && a.FinancialName.Trim() == GlobalSettingObj.SettingValue.Trim()).Select(a => 
                //        new { FinancialId = a.FinancialYearId, FinancialName = a.FinancialName }).Distinct().ToList();

                List<FinancialYear> FYObjList = WetosDB.FinancialYears.ToList();
                List<string> FYStrList = new List<string>();
                foreach (FinancialYear CurrentFYObj in FYObjList)
                {
                    if (CurrentFYObj.StartDate.Year != CurrentFYObj.EndDate.Year) // DIFF YEARS
                    {
                        FYStrList.Add(CurrentFYObj.StartDate.Year.ToString());
                        FYStrList.Add(CurrentFYObj.EndDate.Year.ToString());

                        //SelectListItemList.Add(new SelectListItem { Text = CurrentFYObj.StartDate.Year.ToString(), Value = CurrentFYObj.StartDate.Year.ToString() });
                        //SelectListItemList.Add(new SelectListItem { Text = CurrentFYObj.EndDate.Year.ToString(), Value = CurrentFYObj.EndDate.Year.ToString() });                           
                    }
                    else
                    {
                        FYStrList.Add(CurrentFYObj.StartDate.Year.ToString());
                        //SelectListItemList.Add(new SelectListItem { Text = CurrentFYObj.StartDate.Year.ToString(), Value = CurrentFYObj.StartDate.Year.ToString() });
                    }
                }

                List<SelectListItem> SelectListItemList = new List<SelectListItem>();
                List<string> FinalFYStrList = FYStrList.Distinct().ToList();
                foreach (string CFYStr in FYStrList)
                {
                    SelectListItemList.Add(new SelectListItem { Text = CFYStr, Value = CFYStr });
                }
                var FYListVar = SelectListItemList;
                ViewBag.YearListVB = FYListVar;

                var FYYearList = WetosDB.FinancialYears.Select(a => new { FinancialId = a.FinancialName, FinancialName = a.FinancialName }).ToList();
                ViewBag.FYYearListVB = new SelectList(FYYearList, "FinancialId", "FinancialName").ToList();
            }

            int UserRoleId = Convert.ToInt32(Session["UserRoleId"]);

            if (UserRoleId == 1)
            {
                #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN
                int LoginEmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                List<SP_ActiveEmployeeInEmployeeMasterNew_Result> TempActiveEmployeeList = new List<SP_ActiveEmployeeInEmployeeMasterNew_Result>();
                TempActiveEmployeeList = WetosDB.SP_ActiveEmployeeInEmployeeMasterNew(LoginEmployeeId).Where(a => a.Leavingdate == null || a.Leavingdate > DateTime.Now.Date).ToList(); // CODE ADDED a.Leavingdate > DateTime.Now.Date BY SHRADDHA ON 06 MARCH 2018
                var ActiveEmployeeList = TempActiveEmployeeList.Select(a => new { Value = a.EmployeeId, Text = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
                ViewBag.ActiveEmployeeListVB = new SelectList(ActiveEmployeeList, " Value", "Text").ToList();
                #endregion
            }
            else
            {
                // LIST OF EMPLOYEE REPORTING START // ADDED BY MSJ ON 24 DEC 2018
                int EmpId = Convert.ToInt32(Session["EmployeeNo"]);
                var TempReportingEmployeeListObj = WetosDB.SP_VwActiveEmployee(EmployeeId).Where(a => a.EmployeeReportingId == EmpId && (a.ActiveFlag == null || a.ActiveFlag == true)).ToList();
                var ReportingEmployeeList = TempReportingEmployeeListObj.Select(a => new { Value = a.EmployeeId, Text = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
                ViewBag.ActiveEmployeeListVB = new SelectList(ReportingEmployeeList, " Value", "Text").ToList();

                int ReportingEmpCount = ReportingEmployeeList.Count;
                ViewBag.ActiveEmployeeListCountVB = ReportingEmpCount;
                // LIST OF EMPLOYEE REPORTING END // ADDED BY MSJ ON 24 DEC 2018
            }
                      


            GlobalSetting GSForCurrentFY = WetosDB.GlobalSettings.Where(a => a.SettingId == 2).FirstOrDefault();
            int FinancialYearId = Convert.ToInt32(GSForCurrentFY.SettingValue);
            int MonthId = DateTime.Now.Date.Month;

            FinancialYear FinancialYearObj = WetosDB.FinancialYears.Where(a => a.FinancialName == GSForCurrentFY.SettingValue).FirstOrDefault();
            int Year = FinancialYearId; //2017; REMOVED HARD CODED 2017 BY SHRADDHA ON 02 FEB 2018
            if (FinancialYearObj != null)
            {
                if (MonthId >= 4)
                {
                    Year = FinancialYearObj.StartDate.Year;
                }
                else
                {
                    Year = FinancialYearObj.EndDate.Year;
                }
            }

            //ADDED ORDER BY DATE CONDITION BY SHRADDHA ON 06 FEB 2017 START
            // Updated by Rajas on 27 APRIL 2017
            List<WetosDB.SP_GetAttendanceIndex_Result> GetAttendanceList = WetosDB.SP_GetAttendanceIndex(EmployeeId, MonthId, Year).ToList(); // FromDate.Month, FromDate.Year
            //ADDED ORDER BY DATE CONDITION BY SHRADDHA ON 06 FEB 2017 END


            List<LeaveApplication> LeaveApplicationListForLoggedInEmployee = WetosDB.LeaveApplications.Where(a => a.EmployeeId == EmployeeId).ToList();
            List<CompOffApplication> CompOffApplicationListForLoggedInEmployee = WetosDB.CompOffApplications.Where(a => a.EmployeeId == EmployeeId).ToList();
            List<ODTour> ODTOURApplicationListForLoggedInEmployee = WetosDB.ODTours.Where(a => a.EmployeeId == EmployeeId).ToList();
            Session["LeaveApplicationIsAvailable"] = false;
            Session["CompOffApplicationIsAvailable"] = false;
            Session["ODTOURApplicationIsAvailable"] = false;
            if (LeaveApplicationListForLoggedInEmployee.Count > 0)
            {
                foreach (var i in GetAttendanceList)
                {
                    foreach (var item in LeaveApplicationListForLoggedInEmployee)
                    {
                        for (DateTime CurrentDate = item.FromDate; CurrentDate.Date <= item.ToDate; CurrentDate = CurrentDate.AddDays(1))
                        {
                            if (i.TranDate == CurrentDate && item.StatusId != 2)
                            {
                                //i.StatusName = "Applied Leave";
                                i.Remark = "Applied Leave" + "/" + item.Status;
                                //Session["LeaveApplicationIsAvailable"] = true;
                            }
                            else if (i.TranDate == CurrentDate && item.StatusId == 2)
                            {

                                i.Remark = item.Status;
                            }
                        }
                    }
                }
            }
            if (CompOffApplicationListForLoggedInEmployee.Count > 0)
            {
                foreach (var i in GetAttendanceList)
                {
                    foreach (var item in CompOffApplicationListForLoggedInEmployee)
                    {
                        for (DateTime CurrentDate = Convert.ToDateTime(item.FromDate); CurrentDate.Date <= item.ToDate; CurrentDate = CurrentDate.AddDays(1))
                        {
                            if (i.TranDate == CurrentDate)
                            {
                                //i.StatusName = "Applied CompOff";
                                i.Remark = "Applied CompOff" + "/" + item.Status;
                                //Session["CompOffApplicationIsAvailable"] = true;
                            }
                            else if (i.TranDate == CurrentDate && item.StatusId == 2)
                            {
                                i.Remark = item.Status;
                            }
                        }
                    }
                }
            }
            if (ODTOURApplicationListForLoggedInEmployee.Count > 0)
            {
                foreach (var i in GetAttendanceList)
                {
                    foreach (var item in ODTOURApplicationListForLoggedInEmployee)
                    {
                        for (DateTime CurrentDate = Convert.ToDateTime(item.FromDate); CurrentDate.Date <= item.ToDate; CurrentDate = CurrentDate.AddDays(1))
                        {
                            if (i.TranDate == CurrentDate)
                            {

                                i.Remark = "Applied OD" + "/" + item.Status;
                                //Session["ODTOURApplicationIsAvailable"] = true;
                            }
                            else if (i.TranDate == CurrentDate && item.StatusId == 2)
                            {
                                i.Remark = item.Status;
                            }
                        }
                    }
                }
            }
            int TotalDays = Convert.ToInt32(GetAttendanceList.Count());
            ViewBag.TotalDays = TotalDays;
            int AbsentDays = Convert.ToInt32(GetAttendanceList.Where(a => a.StatusName == "AAAA").Count());
            ViewBag.AbsentDays = AbsentDays;
            int LateByDays = Convert.ToInt32(GetAttendanceList.Where(a => a.StatusName.Contains("AAPP")).Count());
            ViewBag.LateByDays = LateByDays;
            int WeekOffDays = Convert.ToInt32(GetAttendanceList.Where(a => a.Status.Trim() == "WOWO" || a.StatusName.Trim() == "WO").Count());
            ViewBag.WeekOffDays = WeekOffDays;
            int PresentDays = TotalDays - AbsentDays;
            ViewBag.PresentDays = PresentDays;

            return View(GetAttendanceList);
        }
        // ADDED BY MSJ ON 12 DEC 2018 END

        // ADDED BY MSJ ON 12 DEC 2018 START
        [HttpPost]
        public ActionResult EmpDailyTransactionEx(int EmployeeId, int FYId, int MonthId)  // (DateTime FromDate, DateTime ToDate)
        {
            var MonthList = WetosDB.DropdownDatas.Where(a => a.TypeId == 7).Select(a => new { Value = a.Value, Text = a.Text }).ToList();

            ViewBag.MonthListVB = new SelectList(MonthList, " Value", "Text").ToList();

            //GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == "Current Financial Year").FirstOrDefault();

            //ABOVE LINE COMMENTED AND BELOW LINE IS ADDED BY SHALAKA ON 13TH DEC 2017 
            GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.CurrentFinancialYear).FirstOrDefault();

            if (GlobalSettingObj != null)
            {
                // MODIFIED AND ADDED CODE BY MSJ ON 08 JAN 2018 START
                //var YearList = WetosDB.FinancialYears.Where(a => a.Company.CompanyId == LoggedInEmployee.CompanyId 
                //    && a.Branch.BranchId == LoggedInEmployee.BranchId 
                //    && a.FinancialName.Trim() == GlobalSettingObj.SettingValue.Trim()).Select(a => 
                //        new { FinancialId = a.FinancialYearId, FinancialName = a.FinancialName }).Distinct().ToList();

                List<FinancialYear> FYObjList = WetosDB.FinancialYears.ToList();
                List<string> FYStrList = new List<string>();
                foreach (FinancialYear CurrentFYObj in FYObjList)
                {
                    if (CurrentFYObj.StartDate.Year != CurrentFYObj.EndDate.Year) // DIFF YEARS
                    {
                        FYStrList.Add(CurrentFYObj.StartDate.Year.ToString());
                        FYStrList.Add(CurrentFYObj.EndDate.Year.ToString());

                        //SelectListItemList.Add(new SelectListItem { Text = CurrentFYObj.StartDate.Year.ToString(), Value = CurrentFYObj.StartDate.Year.ToString() });
                        //SelectListItemList.Add(new SelectListItem { Text = CurrentFYObj.EndDate.Year.ToString(), Value = CurrentFYObj.EndDate.Year.ToString() });                           
                    }
                    else
                    {
                        FYStrList.Add(CurrentFYObj.StartDate.Year.ToString());
                        //SelectListItemList.Add(new SelectListItem { Text = CurrentFYObj.StartDate.Year.ToString(), Value = CurrentFYObj.StartDate.Year.ToString() });
                    }
                }

                List<SelectListItem> SelectListItemList = new List<SelectListItem>();
                List<string> FinalFYStrList = FYStrList.Distinct().ToList();
                foreach (string CFYStr in FYStrList)
                {
                    SelectListItemList.Add(new SelectListItem { Text = CFYStr, Value = CFYStr });
                }
                var FYListVar = SelectListItemList;
                ViewBag.YearListVB = FYListVar;
            }

            #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN
            int LoginEmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            List<SP_ActiveEmployeeInEmployeeMasterNew_Result> TempActiveEmployeeList = new List<SP_ActiveEmployeeInEmployeeMasterNew_Result>();
            TempActiveEmployeeList = WetosDB.SP_ActiveEmployeeInEmployeeMasterNew(LoginEmployeeId).Where(a => a.Leavingdate == null || a.Leavingdate > DateTime.Now.Date).ToList(); // CODE ADDED a.Leavingdate > DateTime.Now.Date BY SHRADDHA ON 06 MARCH 2018
            var ActiveEmployeeList = TempActiveEmployeeList.Select(a => new { Value = a.EmployeeId, Text = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
            ViewBag.ActiveEmployeeListVB = new SelectList(ActiveEmployeeList, " Value", "Text").ToList();
            #endregion


            //GlobalSetting GSForCurrentFY = WetosDB.GlobalSettings.Where(a => a.SettingId == 2).FirstOrDefault();
            //int FinancialYearId = Convert.ToInt32(GSForCurrentFY.SettingValue);
            ////int MonthId = DateTime.Now.Date.Month;

            //FinancialYear FinancialYearObj = WetosDB.FinancialYears.Where(a => a.FinancialName == GSForCurrentFY.SettingValue).FirstOrDefault();
            //int Year = FinancialYearId; //2017; REMOVED HARD CODED 2017 BY SHRADDHA ON 02 FEB 2018
            //if (FinancialYearObj != null)
            //{
            //    if (MonthId >= 4)
            //    {
            //        Year = FinancialYearObj.StartDate.Year;
            //    }
            //    else
            //    {
            //        Year = FinancialYearObj.EndDate.Year;
            //    }
            //}

            ////ADDED ORDER BY DATE CONDITION BY SHRADDHA ON 06 FEB 2017 START
            //// Updated by Rajas on 27 APRIL 2017
            //List<WetosDB.SP_GetAttendanceIndex_Result> GetAttendanceList = WetosDB.SP_GetAttendanceIndex(EmployeeId, MonthId, Year).ToList(); // FromDate.Month, FromDate.Year
            ////ADDED ORDER BY DATE CONDITION BY SHRADDHA ON 06 FEB 2017 END


            //List<LeaveApplication> LeaveApplicationListForLoggedInEmployee = WetosDB.LeaveApplications.Where(a => a.EmployeeId == EmployeeId).ToList();
            //List<CompOffApplication> CompOffApplicationListForLoggedInEmployee = WetosDB.CompOffApplications.Where(a => a.EmployeeId == EmployeeId).ToList();
            //List<ODTour> ODTOURApplicationListForLoggedInEmployee = WetosDB.ODTours.Where(a => a.EmployeeId == EmployeeId).ToList();
            //Session["LeaveApplicationIsAvailable"] = false;
            //Session["CompOffApplicationIsAvailable"] = false;
            //Session["ODTOURApplicationIsAvailable"] = false;
            //if (LeaveApplicationListForLoggedInEmployee.Count > 0)
            //{
            //    foreach (var i in GetAttendanceList)
            //    {
            //        foreach (var item in LeaveApplicationListForLoggedInEmployee)
            //        {
            //            for (DateTime CurrentDate = item.FromDate; CurrentDate.Date <= item.ToDate; CurrentDate = CurrentDate.AddDays(1))
            //            {
            //                if (i.TranDate == CurrentDate && item.StatusId != 2)
            //                {
            //                    //i.StatusName = "Applied Leave";
            //                    i.Remark = "Applied Leave" + "/" + item.Status;
            //                    //Session["LeaveApplicationIsAvailable"] = true;
            //                }
            //                else if (i.TranDate == CurrentDate && item.StatusId == 2)
            //                {

            //                    i.Remark = item.Status;
            //                }
            //            }
            //        }
            //    }
            //}
            //if (CompOffApplicationListForLoggedInEmployee.Count > 0)
            //{
            //    foreach (var i in GetAttendanceList)
            //    {
            //        foreach (var item in CompOffApplicationListForLoggedInEmployee)
            //        {
            //            for (DateTime CurrentDate = Convert.ToDateTime(item.FromDate); CurrentDate.Date <= item.ToDate; CurrentDate = CurrentDate.AddDays(1))
            //            {
            //                if (i.TranDate == CurrentDate)
            //                {
            //                    //i.StatusName = "Applied CompOff";
            //                    i.Remark = "Applied CompOff" + "/" + item.Status;
            //                    //Session["CompOffApplicationIsAvailable"] = true;
            //                }
            //                else if (i.TranDate == CurrentDate && item.StatusId == 2)
            //                {
            //                    i.Remark = item.Status;
            //                }
            //            }
            //        }
            //    }
            //}
            //if (ODTOURApplicationListForLoggedInEmployee.Count > 0)
            //{
            //    foreach (var i in GetAttendanceList)
            //    {
            //        foreach (var item in ODTOURApplicationListForLoggedInEmployee)
            //        {
            //            for (DateTime CurrentDate = Convert.ToDateTime(item.FromDate); CurrentDate.Date <= item.ToDate; CurrentDate = CurrentDate.AddDays(1))
            //            {
            //                if (i.TranDate == CurrentDate)
            //                {

            //                    i.Remark = "Applied OD" + "/" + item.Status;
            //                    //Session["ODTOURApplicationIsAvailable"] = true;
            //                }
            //                else if (i.TranDate == CurrentDate && item.StatusId == 2)
            //                {
            //                    i.Remark = item.Status;
            //                }
            //            }
            //        }
            //    }
            //}
            //int TotalDays = Convert.ToInt32(GetAttendanceList.Count());
            //ViewBag.TotalDays = TotalDays;
            //int AbsentDays = Convert.ToInt32(GetAttendanceList.Where(a => a.StatusName == "AAAA").Count());
            //ViewBag.AbsentDays = AbsentDays;
            //int LateByDays = Convert.ToInt32(GetAttendanceList.Where(a => a.StatusName.Contains("AAPP")).Count());
            //ViewBag.LateByDays = LateByDays;
            //int WeekOffDays = Convert.ToInt32(GetAttendanceList.Where(a => a.Status.Trim() == "WOWO" || a.StatusName.Trim() == "WO").Count());
            //ViewBag.WeekOffDays = WeekOffDays;
            //int PresentDays = TotalDays - AbsentDays;
            //ViewBag.PresentDays = PresentDays;

            //return PartialView(GetAttendanceList);
            return View();
        }

        [HttpPost]
        public ActionResult EmpDailyTransactionExPV(int EmployeeId, int FYId, int MonthId)  // (DateTime FromDate, DateTime ToDate)
        {
            
            //ADDED ORDER BY DATE CONDITION BY SHRADDHA ON 06 FEB 2017 START
            // Updated by Rajas on 27 APRIL 2017
            List<WetosDB.SP_GetAttendanceIndex_Result> GetAttendanceList = WetosDB.SP_GetAttendanceIndex(EmployeeId, MonthId, FYId).ToList(); // FromDate.Month, FromDate.Year
            //ADDED ORDER BY DATE CONDITION BY SHRADDHA ON 06 FEB 2017 END

            // MarkedAsDelete addded in FOLLOWING CASES BY MSJ ON 04 FEB 2019

            List<LeaveApplication> LeaveApplicationListForLoggedInEmployee = WetosDB.LeaveApplications.Where(a => a.EmployeeId == EmployeeId && a.MarkedAsDelete == 0).ToList();
            List<CompOffApplication> CompOffApplicationListForLoggedInEmployee = WetosDB.CompOffApplications.Where(a => a.EmployeeId == EmployeeId && a.MarkedAsDelete == 0).ToList();
            List<ODTour> ODTOURApplicationListForLoggedInEmployee = WetosDB.ODTours.Where(a => a.EmployeeId == EmployeeId && a.MarkedAsDelete == 0).ToList();
            Session["LeaveApplicationIsAvailable"] = false;
            Session["CompOffApplicationIsAvailable"] = false;
            Session["ODTOURApplicationIsAvailable"] = false;
            if (LeaveApplicationListForLoggedInEmployee.Count > 0)
            {
                foreach (var i in GetAttendanceList)
                {
                    foreach (var item in LeaveApplicationListForLoggedInEmployee)
                    {
                        for (DateTime CurrentDate = item.FromDate; CurrentDate.Date <= item.ToDate; CurrentDate = CurrentDate.AddDays(1))
                        {
                            if (i.TranDate == CurrentDate && item.StatusId != 2)
                            {
                                //i.StatusName = "Applied Leave";
                                i.Remark = "Applied Leave" + "/" + item.Status;
                                //Session["LeaveApplicationIsAvailable"] = true;
                            }
                            else if (i.TranDate == CurrentDate && item.StatusId == 2)
                            {

                                i.Remark = item.Status;
                            }
                        }
                    }
                }
            }
            if (CompOffApplicationListForLoggedInEmployee.Count > 0)
            {
                foreach (var i in GetAttendanceList)
                {
                    foreach (var item in CompOffApplicationListForLoggedInEmployee)
                    {
                        for (DateTime CurrentDate = Convert.ToDateTime(item.FromDate); CurrentDate.Date <= item.ToDate; CurrentDate = CurrentDate.AddDays(1))
                        {
                            if (i.TranDate == CurrentDate)
                            {
                                //i.StatusName = "Applied CompOff";
                                i.Remark = "Applied CompOff" + "/" + item.Status;
                                //Session["CompOffApplicationIsAvailable"] = true;
                            }
                            else if (i.TranDate == CurrentDate && item.StatusId == 2)
                            {
                                i.Remark = item.Status;
                            }
                        }
                    }
                }
            }
            if (ODTOURApplicationListForLoggedInEmployee.Count > 0)
            {
                foreach (var i in GetAttendanceList)
                {
                    foreach (var item in ODTOURApplicationListForLoggedInEmployee)
                    {
                        for (DateTime CurrentDate = Convert.ToDateTime(item.FromDate); CurrentDate.Date <= item.ToDate; CurrentDate = CurrentDate.AddDays(1))
                        {
                            if (i.TranDate == CurrentDate)
                            {

                                i.Remark = "Applied OD" + "/" + item.Status;
                                //Session["ODTOURApplicationIsAvailable"] = true;
                            }
                            else if (i.TranDate == CurrentDate && item.StatusId == 2)
                            {
                                i.Remark = item.Status;
                            }
                        }
                    }
                }
            }
            int TotalDays = Convert.ToInt32(GetAttendanceList.Count());
            ViewBag.TotalDays = TotalDays;
            int AbsentDays = Convert.ToInt32(GetAttendanceList.Where(a => a.StatusName == "AAAA").Count());
            ViewBag.AbsentDays = AbsentDays;
            int LateByDays = Convert.ToInt32(GetAttendanceList.Where(a => a.StatusName.Contains("AAPP")).Count());
            ViewBag.LateByDays = LateByDays;
            int WeekOffDays = Convert.ToInt32(GetAttendanceList.Where(a => a.Status.Trim() == "WOWO" || a.StatusName.Trim() == "WO").Count());
            ViewBag.WeekOffDays = WeekOffDays;
            int PresentDays = TotalDays - AbsentDays;
            ViewBag.PresentDays = PresentDays;

            return PartialView(GetAttendanceList);
        }
        // ADDED BY MSJ ON 12 DEC 2018 END

        public ActionResult RegularizeAttendance(int DailyTrnId)
        {
            DailyTransaction DailyTransactionObj = WetosDB.DailyTransactions.Where(a => a.DailyTrnId == DailyTrnId).FirstOrDefault();
            return PartialView(DailyTransactionObj);
        }

        /// <summary>
        /// FOR APPLY LEAVE FROM CALENDAR FOR SPECIFIC DAY
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ApplyLeaveForSpecificDay(int id)
        {
            DailyTransaction DailyTransactionObj = WetosDB.DailyTransactions.Where(a => a.DailyTrnId == id).FirstOrDefault();
            LeaveApplication LeaveApplicationObj = new LeaveApplication();
            LeaveApplicationObj.EmployeeId = Convert.ToInt32(DailyTransactionObj.EmployeeId);
            LeaveApplicationObj.FromDate = DailyTransactionObj.TranDate;
            LeaveApplicationObj.ToDate = DailyTransactionObj.TranDate;
            LeaveApplicationObj.ToDayStatus = 1;
            LeaveApplicationObj.FromDayStatus = 1;
            LeaveApplicationObj.AppliedDays = 1;
            LeaveApplicationObj.CompanyId = DailyTransactionObj.CompanyId;
            LeaveApplicationObj.BranchId = DailyTransactionObj.BranchId;
            var LeaveBalancesObj = WetosDB.LeaveBalances.Where(a => a.EmployeeId == LeaveApplicationObj.EmployeeId).FirstOrDefault();
            var LeaveMastersObj = WetosDB.LeaveMasters.Where(a => a.LeaveCode == LeaveBalancesObj.LeaveType).FirstOrDefault();
            LeaveApplicationObj.LeaveType = LeaveMastersObj.LeaveCode;
            //LeaveApplicationObj.LeaveTypeId = LeaveMastersObj.LeaveId;
            LeaveMastersObj.MaxNoOfLeavesAllowedInMonth = (LeaveMastersObj.NoOfDaysAllowedInYear + LeaveBalancesObj.PreviousBalance) / 12;
            var LeaveApplicationactualdays = WetosDB.LeaveApplications.Where(a => a.EmployeeId == LeaveApplicationObj.EmployeeId && a.StatusId == 2).Select(a => a.ActualDays).FirstOrDefault();
            var LeaveApplicationapplieddays = WetosDB.LeaveApplications.Where(a => a.EmployeeId == LeaveApplicationObj.EmployeeId && a.StatusId == 2).Select(a => a.AppliedDays).FirstOrDefault();
            if (LeaveApplicationactualdays == null)
            {
                LeaveApplicationactualdays = 0;
            }
            else
            {

            }
            if (LeaveApplicationapplieddays == null)
            {
                LeaveApplicationapplieddays = 0;
            }
            else
            {

            }

            //var AllowedDays = LeaveMastersObj.MaxNoOfLeavesAllowedInMonth;
            var AllowedLeaves = 0D;
            if (LeaveApplicationObj.LeaveType == "PL")
            {

                //Modified By Shraddha on 30 DEC 2016 FOR GIVING PROPER VALIDATION OF MONTH SELECTION ALLOWED LEAVES
                if (LeaveApplicationObj.ToDate.Month >= 4 && LeaveApplicationObj.ToDate.Month <= 12)
                {
                    //ActualDays = Convert.ToDouble(LeaveMastersObj.MaxNoOfTimesAllowedInYear - (LeaveMastersObj.MaxNoOfLeavesAllowedInMonth * 0) + (LeaveMastersObj.MaxNoOfLeavesAllowedInMonth - LeaveBalancesObj.LeaveUsed));
                    AllowedLeaves = Convert.ToDouble((LeaveMastersObj.MaxNoOfLeavesAllowedInMonth * (LeaveApplicationObj.ToDate.Month - 3)) - LeaveApplicationactualdays);
                }
                else if (LeaveApplicationObj.ToDate.Month == 1)
                {
                    //AllowedLeaves = Convert.ToDouble((LeaveMastersObj.MaxNoOfLeavesAllowedInMonth * 10) + (LeaveApplicationactualdays - LeaveApplicationapplieddays));
                    AllowedLeaves = Convert.ToDouble((LeaveMastersObj.MaxNoOfLeavesAllowedInMonth * 10) - LeaveApplicationactualdays);
                }
                else if (LeaveApplicationObj.ToDate.Month == 2)
                {
                    AllowedLeaves = Convert.ToDouble((LeaveMastersObj.MaxNoOfLeavesAllowedInMonth * 11) - LeaveApplicationactualdays);
                }
                else if (LeaveApplicationObj.ToDate.Month == 3)
                {
                    AllowedLeaves = Convert.ToDouble((LeaveMastersObj.MaxNoOfLeavesAllowedInMonth * 12) - LeaveApplicationactualdays);
                }
                else { }
                if (LeaveApplicationObj.AppliedDays > AllowedLeaves)
                {
                    LeaveApplicationObj.ActualDays = AllowedLeaves;
                }
                else
                {
                    LeaveApplicationObj.ActualDays = Convert.ToDouble(LeaveApplicationObj.AppliedDays);
                }
                LeaveApplicationObj.StatusId = 1;
                LeaveApplicationObj.Status = "Pending";
            }
            WetosDB.LeaveApplications.AddObject(LeaveApplicationObj);
            WetosDB.SaveChanges();

            return RedirectToAction("Index");
        }
        /// <summary>
        /// FOR APPLY COMPOFF FROM CALENDAR FOR SPECIFIC DAY
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ApplyCompOffForSpecificDay(int id)
        {
            DailyTransaction DailyTransactionObj = WetosDB.DailyTransactions.Where(a => a.DailyTrnId == id).FirstOrDefault();
            CompOffApplication COMPOFFAPPLICATIONObj = new CompOffApplication();
            COMPOFFAPPLICATIONObj.EmployeeId = Convert.ToInt32(DailyTransactionObj.EmployeeId);
            COMPOFFAPPLICATIONObj.FromDate = DailyTransactionObj.TranDate;
            COMPOFFAPPLICATIONObj.ToDate = DailyTransactionObj.TranDate;
            COMPOFFAPPLICATIONObj.ToDateStatus = 1;
            COMPOFFAPPLICATIONObj.FromDateStatus = 1;
            COMPOFFAPPLICATIONObj.Purpose = "";
            COMPOFFAPPLICATIONObj.CompanyId = DailyTransactionObj.CompanyId;
            COMPOFFAPPLICATIONObj.BranchId = DailyTransactionObj.BranchId;
            COMPOFFAPPLICATIONObj.RejectReason = "";
            COMPOFFAPPLICATIONObj.Status = "Pending";
            COMPOFFAPPLICATIONObj.StatusId = 1;
            WetosDB.CompOffApplications.AddObject(COMPOFFAPPLICATIONObj);
            WetosDB.SaveChanges();

            return RedirectToAction("Index");
        }
        /// <summary>
        /// FOR APPLY OD FROM CALENDAR FOR SPECIFIC DAY
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ApplyODForSpecificDay(int id)
        {
            DailyTransaction DailyTransactionObj = WetosDB.DailyTransactions.Where(a => a.DailyTrnId == id).FirstOrDefault();
            ODTour ODTOURObj = new ODTour();
            ODTOURObj.EmployeeId = Convert.ToInt32(DailyTransactionObj.EmployeeId);
            ODTOURObj.FromDate = DailyTransactionObj.TranDate;
            ODTOURObj.ToDate = DailyTransactionObj.TranDate;

            // Updated by Rajas on 19 AUGUST 2017 START
            // Data type changed from string to int 
            ODTOURObj.ODDayStatus = 1;
            ODTOURObj.ODDayStatus1 = 1;
            // Updated by Rajas on 19 AUGUST 2017 END

            ODTOURObj.Place = string.Empty;
            ODTOURObj.CompanyId = DailyTransactionObj.CompanyId;
            ODTOURObj.BranchId = DailyTransactionObj.BranchId;
            ODTOURObj.RejectReason = "";
            ODTOURObj.Status = "Pending";
            ODTOURObj.StatusId = 1;
            WetosDB.ODTours.AddObject(ODTOURObj);
            WetosDB.SaveChanges();

            return RedirectToAction("Index");
        }


    }
}
