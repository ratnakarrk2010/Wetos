using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WetosDB;
using WetosMVCMainApp.Models;
using System.Configuration;
using System.Text;
using System.Net.Mail;

namespace WetosMVC.Controllers
{
    public class WetosCumulativeCompOffController : BaseController
    {
        //
        // GET: /WetosCumulativeCompOff/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /WetosCumulativeCompOff/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /WetosCumulativeCompOff/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /WetosCumulativeCompOff/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /WetosCumulativeCompOff/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /WetosCumulativeCompOff/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /WetosCumulativeCompOff/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /WetosCumulativeCompOff/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult CumulativeCompOffIndex(string IsMySelf = "true")
        {
            #region SP ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
            //List<VwCumulativeManualCompOffList> ManualCompOffList = new List<VwCumulativeManualCompOffList>();
            List<SP_VwCumulativeManualCompOffList_Result> ManualCompOffList = new List<SP_VwCumulativeManualCompOffList_Result>();
            #endregion
            try
            {

                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);

                //GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == "Current Financial Year").FirstOrDefault();

                //ABOVE LINE COMMENTED AND BELOW LINE IS ADDED BY SHALAKA ON 13TH DEC 2017
                GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.CurrentFinancialYear).FirstOrDefault();

                // Error
                if (GlobalSettingObj == null)
                {
                    Error("Inconsistent Financial year data");

                    return View(ManualCompOffList);
                }

                DateTime CalanderStartDate = GetFYStartDate(); // DateTime CalanderStartDate = WetosDB.FinancialYears.Where(a => a.FinancialName == GlobalSettingObj.SettingValue).Select(a => a.StartDate).FirstOrDefault();

                if (CalanderStartDate != null)
                {
                    if (IsMySelf == "true")
                    {
                        #region SP ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                        //ManualCompOffList = WetosDB.VwCumulativeManualCompOffLists.Where(a => a.EmployeeId == EmployeeId)
                        //    .Where(a => a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0).OrderByDescending(a => a.FromDate).ToList();
                        ManualCompOffList = WetosDB.SP_VwCumulativeManualCompOffList(EmployeeId).Where(a => a.EmployeeId == EmployeeId)
                            .Where(a => a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0).OrderByDescending(a => a.FromDate).ToList();
                        #endregion
                    }
                    else
                    {
                        #region SP ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                        //ManualCompOffList = WetosDB.VwCumulativeManualCompOffLists
                        //    .Where(a => a.MarkedAsDelete == 0 && a.FromDate >= CalanderStartDate && a.EmployeeId != EmployeeId).OrderByDescending(a => a.FromDate).ToList();
                        ManualCompOffList = WetosDB.SP_VwCumulativeManualCompOffList(EmployeeId)
                            .Where(a => a.MarkedAsDelete == 0 && a.FromDate >= CalanderStartDate && a.EmployeeId != EmployeeId).OrderByDescending(a => a.FromDate).ToList();
                        #endregion
                    }
                }

                ViewBag.ForOthers = IsMySelf;
            }
            catch (System.Exception E)
            {
                AddAuditTrail("Error occurred in Manual comp off list open:" + (E.InnerException == null ? string.Empty : E.InnerException.Message));
            }
            return View(ManualCompOffList);
        }

        public ActionResult CumulativeCompOffCreate(bool Myself = true)
        {
            int EmpId = Convert.ToInt32(Session["EmployeeNo"]);
            List<SP_GetDailyTransactionListForCumulativeCompOffNew_Result> DTExtraHoursView = new List<SP_GetDailyTransactionListForCumulativeCompOffNew_Result>();
            ViewBag.Myself = Myself.ToString();
            ViewBag.EmployeeId = EmpId;
            PopulateDropDown();

            return View(DTExtraHoursView);
        }

        [HttpPost]
        public ActionResult CumulativeCompOffCreatePV(int EmpId = 0)
        {
            if (EmpId == 0)
            {
                EmpId = Convert.ToInt32(Session["EmployeeNo"]);
            }
            List<SP_GetDailyTransactionListForCumulativeCompOffNew_Result> DTExtraHoursView = new List<SP_GetDailyTransactionListForCumulativeCompOffNew_Result>();
            try
            {
                #region EXTRA WORKING HOURS
                RuleTransaction ExtraHoursAccumulationAllowedornot = WetosDB.RuleTransactions.Where(a => a.RuleId == 34).FirstOrDefault(); // 34 - Extra Hours Accumulation Allowed or not //CODE ADDED BY SHRADDHA ON 19 FEB 2018
                RuleTransaction MinRequiredExtraHoursForAccumulation = WetosDB.RuleTransactions.Where(a => a.RuleId == 35).FirstOrDefault(); //35 - Min. required extra hours for accumulation //CODE ADDED BY SHRADDHA ON 19 FEB 2018

                if (ExtraHoursAccumulationAllowedornot != null && MinRequiredExtraHoursForAccumulation != null)
                {
                    string[] MinRequiredExtraHoursForAccumulationSplitValue = MinRequiredExtraHoursForAccumulation.Formula.Split(':');
                    int AccumulationHourInt = 0;
                    int AccumulationMinuteInt = 0;
                    double MinTotalTimeRequiredForAccumulation = 0;
                    AccumulationHourInt = Convert.ToInt32(MinRequiredExtraHoursForAccumulationSplitValue[0]); // Minutes missing
                    AccumulationMinuteInt = Convert.ToInt32(MinRequiredExtraHoursForAccumulationSplitValue[1]); // Minutes missing
                    MinTotalTimeRequiredForAccumulation = (60 * AccumulationHourInt) + AccumulationMinuteInt;
                    DTExtraHoursView = WetosDB.SP_GetDailyTransactionListForCumulativeCompOffNew(EmpId).Where(a => a.ExtraHrs != null && ((60 * a.ExtraHrs.Value.Hour) + a.ExtraHrs.Value.Minute) >= MinTotalTimeRequiredForAccumulation).ToList();
                }
                else
                {
                    if (DTExtraHoursView == null)
                    {
                        DTExtraHoursView = new List<SP_GetDailyTransactionListForCumulativeCompOffNew_Result>();
                    }
                }
                #endregion
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Exception during validating leave " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );
            }
            return PartialView(DTExtraHoursView);
        }


        [HttpPost]
        public ActionResult CumulativeCompOffCreate(List<SP_GetDailyTransactionListForCumulativeCompOff_Result> DailyTransactionList, string[] DailyTrnIdList, FormCollection FC)
        {
            bool MySelf = Convert.ToBoolean(FC["MySelf"].ToString());

            int EmpId = Convert.ToInt32(Session["EmployeeNo"]);

            if (DailyTrnIdList != null)
            {
                foreach (string DailyTrnIdStr in DailyTrnIdList)
                {
                    int DailyTrnIdInt = Convert.ToInt32(DailyTrnIdStr);
                    DailyTransaction DailyTransactionObj = WetosDB.DailyTransactions.Where(a => a.DailyTrnId == DailyTrnIdInt).FirstOrDefault();
                    #region IF EXTRA HOURS ACCUMULATION IS ALLOWED
                    int EmployeeGroupIdObj = WetosDB.EmployeeGroupDetails.Where(a => a.Employee.EmployeeId == DailyTransactionObj.EmployeeId)
                                          .Select(a => a.EmployeeGroup.EmployeeGroupId).FirstOrDefault();
                    List<RuleTransaction> RuleTransactionList = WetosDB.RuleTransactions.Where(a => a.EmployeeGroupId == EmployeeGroupIdObj).ToList();
                    RuleTransaction ExtraHoursAccumulationAllowedornot = RuleTransactionList.Where(a => a.RuleId == 34).FirstOrDefault(); // 34 - Extra Hours Accumulation Allowed or not //CODE ADDED BY SHRADDHA ON 19 FEB 2018
                    RuleTransaction MinRequiredExtraHoursForAccumulation = RuleTransactionList.Where(a => a.RuleId == 35).FirstOrDefault(); //35 - Min. required extra hours for accumulation //CODE ADDED BY SHRADDHA ON 19 FEB 2018
                    RuleTransaction WeekDayCompOffFactor = RuleTransactionList.Where(a => a.RuleId == 36).FirstOrDefault(); //36 - Week day comp off factor //CODE ADDED BY SHRADDHA ON 20 FEB 2018
                    RuleTransaction WeekOffCompOffFactor = RuleTransactionList.Where(a => a.RuleId == 37).FirstOrDefault(); //37 - Week off comp off factor //CODE ADDED BY SHRADDHA ON 20 FEB 2018
                    RuleTransaction HolidayCompOffFactor = RuleTransactionList.Where(a => a.RuleId == 38).FirstOrDefault(); //38 - Holiday comp off factor //CODE ADDED BY SHRADDHA ON 20 FEB 2018
                    RuleTransaction CompOffSetTime = RuleTransactionList.Where(a => a.RuleId == 39).FirstOrDefault(); //39 - Comp off set time //CODE ADDED BY SHRADDHA ON 20 FEB 2018

                    #region RULES ADDED BY SHRADDHA ON 20 FEB 2018 AS PER SUGGESTED BY KATRE SIR BUT NOT USED
                    RuleTransaction IsExtraHoursRoundedOffForCompOff = RuleTransactionList.Where(a => a.RuleId == 40).FirstOrDefault(); //40 - Is Extra hours rounded off for Comp off //CODE ADDED BY SHRADDHA ON 20 FEB 2018
                    RuleTransaction CompOffRoundedOffTime = RuleTransactionList.Where(a => a.RuleId == 41).FirstOrDefault(); //41 - Comp Off Rounded Off Time //CODE ADDED BY SHRADDHA ON 20 FEB 2018
                    #endregion

                    if (MinRequiredExtraHoursForAccumulation != null)
                    {
                        // MIN REQUIRED EXTRA HOURS FOR ACCUMULATION
                        int AccumulationHourInt = 0;
                        int AccumulationMinuteInt = 0;
                        double MinTotalTimeRequiredForAccumulation = 0;
                        double CompOffBalance = 0;
                        string ReferenceString = string.Empty;

                        //CODE ADDED BY SHRADDHA ON 20 FEB 2018 START
                        int CompOffSetTimeHourInt = 0;
                        int CompOffSetTimeMinuteInt = 0;
                        double CompOffSetTimeDouble = 0;
                        //DateTime CompOffHours = Convert.ToDateTime(DailyTransactionObj.ExtraHrs);
                        int CompOffHours = DailyTransactionObj.ExtraHrs == null ? 0 : ((DailyTransactionObj.ExtraHrs.Value.Hour * 60) + DailyTransactionObj.ExtraHrs.Value.Minute);
                        //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 20 MAR 2018

                        if (MinRequiredExtraHoursForAccumulation != null)
                        {
                            // 08:00:00
                            string[] MinRequiredExtraHoursForAccumulationSplitValue = MinRequiredExtraHoursForAccumulation.Formula.Split(':');
                            AccumulationHourInt = Convert.ToInt32(MinRequiredExtraHoursForAccumulationSplitValue[0]); // Minutes missing
                            AccumulationMinuteInt = Convert.ToInt32(MinRequiredExtraHoursForAccumulationSplitValue[1]); // Minutes missing
                            MinTotalTimeRequiredForAccumulation = (60 * AccumulationHourInt) + AccumulationMinuteInt;

                            //CODE ADDED BY SHRADDHA ON 20 FEB 2018 START
                            bool IsWeekOff = DailyTransactionObj.Status.Contains(WetosAdministrationController.StatusConstants.PresentOnWeekOff) == true ? true : false;
                            bool IsHoliday = DailyTransactionObj.Status.Contains(WetosAdministrationController.StatusConstants.PresentOnHoliday) == true ? true : false;
                            bool IsWeekDay = true;
                            if (DailyTransactionObj.Status.Contains(WetosAdministrationController.StatusConstants.PresentOnHoliday) == true || DailyTransactionObj.Status.Contains(WetosAdministrationController.StatusConstants.PresentOnWeekOff) == true)
                            {
                                IsWeekDay = false;
                            }

                            #region CUMULATIVE COMP OFF FORMULA

                            #endregion
                            //CODE ADDED BY SHRADDHA ON 20 FEB 2018 END

                            if (DailyTransactionObj.Status.Contains(WetosAdministrationController.StatusConstants.PresentOnHoliday) || DailyTransactionObj.Status.Contains(WetosAdministrationController.StatusConstants.PresentOnWeekOff))
                            {

                                DailyTransactionObj.ExtraHrs = DailyTransactionObj.WorkingHrs;
                            }

                            #region PRESENT ON NORMAL WEEK DAY OR WEEKOFF OR HOLIDAY
                            //double TotalExtraHoursTimeForWeekDays = (60 * DailyTransactionObj.ExtraHrs.Value.Hour) + DailyTransactionObj.ExtraHrs.Value.Minute;
                            int TotalExtraHoursTimeForWeekDays = (60 * DailyTransactionObj.ExtraHrs.Value.Hour) + DailyTransactionObj.ExtraHrs.Value.Minute;


                            #region IF WORKING HOURS TOTAL TIME MORE THAN OR EQUAL TO REQUIRED TIME FOR ACCUMULATION
                            if (TotalExtraHoursTimeForWeekDays >= MinTotalTimeRequiredForAccumulation)
                            {
                                // 08:00:00
                                string[] CompOffSetTimeSplitValue = CompOffSetTime.Formula.Split(':');
                                CompOffSetTimeHourInt = Convert.ToInt32(CompOffSetTimeSplitValue[0]); // Minutes missing
                                CompOffSetTimeMinuteInt = Convert.ToInt32(CompOffSetTimeSplitValue[1]); // Minutes missing
                                CompOffSetTimeDouble = (60 * CompOffSetTimeHourInt) + CompOffSetTimeMinuteInt;

                                double ExtraCoHourMinutes = TotalExtraHoursTimeForWeekDays - MinTotalTimeRequiredForAccumulation;
                                if (ExtraCoHourMinutes % CompOffSetTimeDouble == 0)
                                {

                                }
                                else
                                {
                                    #region CODE TO BE ADDED WITH HELP OF MSJ NEED HELP GF MSJ
                                    double ExtraMinutesFraction = (ExtraCoHourMinutes % CompOffSetTimeDouble);
                                    ExtraCoHourMinutes = ExtraCoHourMinutes - ExtraMinutesFraction;
                                    //TotalExtraHoursTimeForWeekDays = TotalExtraHoursTimeForWeekDays - ExtraMinutesFraction;
                                    TotalExtraHoursTimeForWeekDays = Convert.ToInt32(TotalExtraHoursTimeForWeekDays - ExtraMinutesFraction);
                                    #endregion
                                }

                                //int ExtraCoHourMinutesInt = Convert.ToInt32(ExtraCoHourMinutes % 60);
                                int IsWeekOffInt = Convert.ToInt32(IsWeekOff);
                                int IsWeekDayInt = Convert.ToInt32(IsWeekDay);
                                int IsHolidayInt = Convert.ToInt32(IsHoliday);
                                //TotalExtraHoursTimeForWeekDays = (TotalExtraHoursTimeForWeekDays * IsWeekOffInt * Convert.ToDouble(WeekOffCompOffFactor.Formula)) + (TotalExtraHoursTimeForWeekDays * IsWeekDayInt * Convert.ToDouble(WeekDayCompOffFactor.Formula)) + (TotalExtraHoursTimeForWeekDays * IsHolidayInt * Convert.ToDouble(HolidayCompOffFactor.Formula));
                                TotalExtraHoursTimeForWeekDays = Convert.ToInt32((TotalExtraHoursTimeForWeekDays * IsWeekOffInt * Convert.ToDouble(WeekOffCompOffFactor.Formula)) + (TotalExtraHoursTimeForWeekDays * IsWeekDayInt * Convert.ToDouble(WeekDayCompOffFactor.Formula)) + (TotalExtraHoursTimeForWeekDays * IsHolidayInt * Convert.ToDouble(HolidayCompOffFactor.Formula)));

                                int TotalExtraMinutesForWeekDays = Convert.ToInt32(TotalExtraHoursTimeForWeekDays % 60);
                                int TotalExtraHoursForWeekDays = Convert.ToInt32((TotalExtraHoursTimeForWeekDays - TotalExtraMinutesForWeekDays) / 60);
                                //CompOffHours = new DateTime(DailyTransactionObj.TranDate.Year, DailyTransactionObj.TranDate.Month, DailyTransactionObj.TranDate.Day, TotalExtraHoursForWeekDays, TotalExtraMinutesForWeekDays, 0);
                                CompOffHours = TotalExtraHoursTimeForWeekDays;


                                //WetosAdministrationController.UpdateManualCompOffTbl(WetosDB, DailyTransactionObj, CompOffHours, IsWeekDay, IsWeekOff, IsHoliday, CompOffBalance, ref ReferenceString);
                                WetosAdministrationController.UpdateCumulativeManualCompOffTbl(WetosDB, DailyTransactionObj, CompOffHours, IsWeekDay, IsWeekOff, IsHoliday, CompOffBalance, ref ReferenceString);
                            }
                            #endregion
                            #endregion

                        }
                    }
                    #endregion
                }
            }
            if (MySelf == true)
            {
                return RedirectToAction("CumulativeCompOffIndex");
            }
            else
            {
                return RedirectToAction("CumulativeCompOffIndex", new { IsMySelf = "false" });
            }

        }

        /// <summary>
        /// MODIFIED CODE BY SHRADDHA ON 09 MARCH 2017 FOR CompOffApplication GET
        /// Try catch block added by Rajas on 22 FEB 2017
        /// </summary>
        /// <returns></returns>
        public ActionResult CumulativeCompOffApplication()
        {
            try
            {
                int EmpId = Convert.ToInt32(Session["EmployeeNo"]);
                COMPOffApplicationModel ComOffApp = new COMPOffApplicationModel();

                CompOffApplicationNecessaryContent(EmpId);
                ComOffApp.EmployeeId = EmpId;
                PopulateDropDown();
                return View(ComOffApp);
            }

            catch (System.Exception ex)
            {
                AddAuditTrail("CompOff application failed due to " + ex.Message);
                Error("Can not apply CompOff");

                return RedirectToAction("CompOffApplicationIndex");
            }
        }


        /// <summary>
        /// Added by MOUSAMI ON 14 SEPT 2016
        /// MODIFIED CODE BY SHRADDHA ON 09 MARCH 2017 FOR CompOffApplication POST 
        /// <param name="ODTourObj"></param>
        /// <param name="Collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CumulativeCompOffApplication(COMPOffApplicationModel CompOffModelObj, FormCollection Collection)
        {
            #region COMPOFF CODE MODIFIED BY SHRADDHA
            int EmpId = Convert.ToInt32(Session["EmployeeNo"]);
            Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == CompOffModelObj.EmployeeId).FirstOrDefault();

            try
            {
                int CheckCompOffId = 0;
                string CheckStatus = Collection["CheckStatus"];
                if (CheckStatus != null)
                {
                    List<string> CheckStatusDataList = CheckStatus.Split(',').ToList();
                    List<CompOff> CompOffList = new List<CompOff>();
                    List<int> CheckCompOffIdList = new List<int>();
                    int FalseCount = CheckStatusDataList.Where(a => a.Contains("false")).Count();

                    if (FalseCount == CheckStatusDataList.Count())
                    {
                        ModelState.AddModelError("", "Please Select atleast one CheckBox to apply CompOff");

                        CompOffApplicationNecessaryContent(EmpId);

                        PopulateDropDown();

                        return View(CompOffModelObj);
                    }
                    else
                    {

                        foreach (string CheckStatusDataObj in CheckStatusDataList)
                        {
                            if (!CheckStatusDataObj.Contains("false"))
                            {
                                CheckCompOffId = Convert.ToInt32(CheckStatusDataObj);
                                CheckCompOffIdList.Add(CheckCompOffId);

                                CompOff CompOffObj = WetosDB.CompOffs.Where(a => a.CompOffId == CheckCompOffId).FirstOrDefault();
                                if (CompOffObj != null)
                                {
                                    CompOffList.Add(CompOffObj);
                                }
                            }
                        }

                    }


                    //double TotalAllowedCompOff = Convert.ToDouble(Collection["TtlAllwdDys"]);
                    //Added By Shraddha on 12 DEC 2016 for adding date validation start
                    if (Convert.ToDateTime(CompOffModelObj.FromDate) > Convert.ToDateTime(CompOffModelObj.ToDate))
                    {
                        ModelState.AddModelError("", "End Date should be Greater than or equal to From Date");
                        CompOffApplicationNecessaryContent(EmpId);
                        PopulateDropDown();
                        //List<CompOffApplication> CompOffApplicationList = WetosDB.CompOffApplications.Where(a => a.EmployeeId == EmployeeNo).ToList();
                        return View(CompOffModelObj);
                    }
                    //Added By Shraddha on 12 DEC 2016 for adding date validation End

                    if (ModelState.IsValid)
                    {
                        #region VALIDATE CO DATA

                        // Updated by Rajas on 27 SEP 2017 START
                        // Fixed Issue No. 6 in Flagship Defect Sheet raised by Sandeep sir
                        LeaveApplication LeaveApplicationObj = WetosDB.LeaveApplications.Where(a => a.EmployeeId == EmpId
                            && (a.StatusId == 1 || a.StatusId == 2 || a.StatusId == 4) && a.MarkedAsDelete == 0
                            && ((a.FromDate == CompOffModelObj.FromDate) // && a.ToDate > LeaveTypeObj.ToDate) 
                             || (a.FromDate < CompOffModelObj.FromDate && a.ToDate == CompOffModelObj.ToDate)
                             || (a.FromDate < CompOffModelObj.FromDate && a.ToDate > CompOffModelObj.ToDate)
                             || (a.FromDate > CompOffModelObj.FromDate && a.ToDate < CompOffModelObj.ToDate)
                             || (a.FromDate == CompOffModelObj.FromDate && a.ToDate == CompOffModelObj.ToDate)
                             || (a.FromDate > CompOffModelObj.FromDate && a.FromDate < CompOffModelObj.ToDate) // ADDED BY MSJ ON 11 JAN 2018
                             || (a.ToDate > CompOffModelObj.FromDate && a.ToDate < CompOffModelObj.ToDate) // ADDED BY MSJ ON 11 JAN 2018
                             || (a.ToDate == CompOffModelObj.FromDate))
                            //&& a.FromDate <= CompOffModelObj.FromDate && a.ToDate >= CompOffModelObj.ToDate
                            ).FirstOrDefault();

                        // Added by Rajas on 10 MARCH 2017 for validation to check if already pending or sanctioned CO is applied for same date
                        CompOffApplication CompoffApplicationObj = WetosDB.CompOffApplications.Where(a => a.EmployeeId == EmpId
                            && (a.StatusId == 1 || a.StatusId == 2 || a.StatusId == 4)
                            && a.MarkedAsDelete == 0
                            //&& a.FromDate <= CompOffModelObj.FromDate && a.ToDate >= CompOffModelObj.ToDate
                             && ((a.FromDate == CompOffModelObj.FromDate) // && a.ToDate > LeaveTypeObj.ToDate) 
                             || (a.FromDate < CompOffModelObj.FromDate && a.ToDate == CompOffModelObj.ToDate)
                             || (a.FromDate < CompOffModelObj.FromDate && a.ToDate > CompOffModelObj.ToDate)
                             || (a.FromDate > CompOffModelObj.FromDate && a.ToDate < CompOffModelObj.ToDate)
                             || (a.FromDate == CompOffModelObj.FromDate && a.ToDate == CompOffModelObj.ToDate)
                             || (a.FromDate > CompOffModelObj.FromDate && a.FromDate < CompOffModelObj.ToDate) // ADDED BY MSJ ON 11 JAN 2018
                             || (a.ToDate > CompOffModelObj.FromDate && a.ToDate < CompOffModelObj.ToDate) // ADDED BY MSJ ON 11 JAN 2018
                             || (a.ToDate == CompOffModelObj.FromDate))
                            ).FirstOrDefault();

                        ODTour ODTourTblObj = WetosDB.ODTours.Where(a => a.EmployeeId == EmpId
                            && (a.StatusId == 1 || a.StatusId == 2 || a.StatusId == 4)
                                        && a.MarkedAsDelete == 0
                            //&& a.FromDate <= CompOffModelObj.FromDate && a.ToDate >= CompOffModelObj.ToDate
                                         && ((a.FromDate == CompOffModelObj.FromDate) // && a.ToDate > LeaveTypeObj.ToDate) 
                             || (a.FromDate < CompOffModelObj.FromDate && a.ToDate == CompOffModelObj.ToDate)
                             || (a.FromDate < CompOffModelObj.FromDate && a.ToDate > CompOffModelObj.ToDate)
                             || (a.FromDate > CompOffModelObj.FromDate && a.ToDate < CompOffModelObj.ToDate)
                             || (a.FromDate == CompOffModelObj.FromDate && a.ToDate == CompOffModelObj.ToDate)
                             || (a.FromDate > CompOffModelObj.FromDate && a.FromDate < CompOffModelObj.ToDate) // ADDED BY MSJ ON 11 JAN 2018
                             || (a.ToDate > CompOffModelObj.FromDate && a.ToDate < CompOffModelObj.ToDate) // ADDED BY MSJ ON 11 JAN 2018
                             || (a.ToDate == CompOffModelObj.FromDate))
                                        ).FirstOrDefault();
                        // Updated by Rajas on 27 SEP 2017 END

                        for (DateTime CurrentDate = Convert.ToDateTime(CompOffModelObj.FromDate); CurrentDate.Date <= CompOffModelObj.ToDate; CurrentDate = CurrentDate.AddDays(1))
                        {
                            #region LEAVE ALREADY APPLIED BETWEEN SELECTED DATE
                            if (LeaveApplicationObj != null)
                            {
                                #region Validate Leave Data

                                // Code added by Rajas on 27 SEP 2017 START
                                if (LeaveApplicationObj.FromDate == LeaveApplicationObj.ToDate) // Single day
                                {
                                    if (CurrentDate == LeaveApplicationObj.FromDate)
                                    {
                                        if (LeaveApplicationObj.FromDayStatus == 1)  // Full day in single day
                                        {
                                            Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                            PopulateDropDown();
                                            CompOffApplicationNecessaryContent(EmpId);
                                            return View(CompOffModelObj);
                                        }
                                        else
                                        {
                                            if (LeaveApplicationObj.FromDayStatus == CompOffModelObj.FromDateStatus)
                                            {
                                                Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                                PopulateDropDown();
                                                CompOffApplicationNecessaryContent(EmpId);
                                                return View(CompOffModelObj);
                                            }
                                            else
                                            {
                                                if (CompOffModelObj.FromDateStatus == 1)
                                                {
                                                    Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                                    PopulateDropDown();
                                                    CompOffApplicationNecessaryContent(EmpId);
                                                    return View(CompOffModelObj);
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (CurrentDate == LeaveApplicationObj.FromDate)
                                    {
                                        if (LeaveApplicationObj.FromDayStatus == 1)  // Full day in single day
                                        {
                                            Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                            PopulateDropDown();
                                            CompOffApplicationNecessaryContent(EmpId);
                                            return View(CompOffModelObj);
                                        }
                                        else
                                        {
                                            if (LeaveApplicationObj.FromDayStatus == CompOffModelObj.FromDateStatus)
                                            {
                                                Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                                PopulateDropDown();
                                                CompOffApplicationNecessaryContent(EmpId);
                                                return View(CompOffModelObj);
                                            }
                                            else
                                            {
                                                if (CompOffModelObj.FromDateStatus == 1)
                                                {
                                                    Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                                    PopulateDropDown();
                                                    CompOffApplicationNecessaryContent(EmpId);
                                                    return View(CompOffModelObj);
                                                }
                                            }
                                        }
                                    }
                                    else if (CurrentDate == LeaveApplicationObj.ToDate)
                                    {
                                        if (LeaveApplicationObj.ToDayStatus == 1)  // Full day in single day
                                        {
                                            Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                            PopulateDropDown();
                                            CompOffApplicationNecessaryContent(EmpId);
                                            return View(CompOffModelObj);
                                        }
                                        else
                                        {
                                            if (LeaveApplicationObj.FromDayStatus == CompOffModelObj.FromDateStatus)
                                            {
                                                Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                                PopulateDropDown();
                                                CompOffApplicationNecessaryContent(EmpId);
                                                return View(CompOffModelObj);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                        PopulateDropDown();
                                        CompOffApplicationNecessaryContent(EmpId);
                                        return View(CompOffModelObj);
                                    }
                                }
                                // Code added by Rajas on 27 SEP 2017 END

                                #endregion Validate OD Data
                            }
                            #endregion

                            #region VALIDATE COMPOFF
                            // Added by Rajas on 10 MARCH 2017 for validating is compoff already present for same date range as of leave START
                            if (CompoffApplicationObj != null)
                            {
                                #region Validate Compoff Data

                                // Code added by Rajas on 27 SEP 2017 START
                                if (CompoffApplicationObj.FromDate == CompoffApplicationObj.ToDate) // Single day
                                {
                                    if (CurrentDate == CompoffApplicationObj.FromDate)
                                    {
                                        if (CompoffApplicationObj.FromDateStatus == 1)  // Full day in single day
                                        {
                                            Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range"
                                                + "<br/>" + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                            PopulateDropDown();
                                            CompOffApplicationNecessaryContent(EmpId);
                                            return View(CompOffModelObj);
                                        }
                                        else
                                        {
                                            if (CompoffApplicationObj.FromDateStatus == CompOffModelObj.FromDateStatus)
                                            {
                                                Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                                    + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                                    + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                                PopulateDropDown();
                                                CompOffApplicationNecessaryContent(EmpId);
                                                return View(CompOffModelObj);
                                            }
                                            else
                                            {
                                                if (CompOffModelObj.FromDateStatus == 1)
                                                {
                                                    Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range"
                                                        + "<br/>" + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                                    PopulateDropDown();
                                                    CompOffApplicationNecessaryContent(EmpId);
                                                    return View(CompOffModelObj);
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (CurrentDate == CompoffApplicationObj.FromDate)
                                    {
                                        if (CompoffApplicationObj.FromDateStatus == 1)  // Full day in single day
                                        {
                                            Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                                + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                                + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                            PopulateDropDown();
                                            CompOffApplicationNecessaryContent(EmpId);
                                            return View(CompOffModelObj);
                                        }
                                        else
                                        {
                                            if (CompoffApplicationObj.FromDateStatus == CompOffModelObj.FromDateStatus)
                                            {
                                                Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                                    + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                                    + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                                PopulateDropDown();
                                                CompOffApplicationNecessaryContent(EmpId);
                                                return View(CompOffModelObj);
                                            }
                                            else
                                            {
                                                if (CompOffModelObj.FromDateStatus == 1)
                                                {
                                                    Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range"
                                                        + "<br/>" + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                                    PopulateDropDown();
                                                    CompOffApplicationNecessaryContent(EmpId);
                                                    return View(CompOffModelObj);
                                                }
                                            }
                                        }
                                    }
                                    else if (CurrentDate == CompoffApplicationObj.ToDate)
                                    {
                                        if (CompoffApplicationObj.ToDateStatus == 1)  // Full day in single day
                                        {
                                            Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                                + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                                + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                            PopulateDropDown();
                                            CompOffApplicationNecessaryContent(EmpId);
                                            return View(CompOffModelObj);
                                        }
                                        else
                                        {
                                            if (CompoffApplicationObj.FromDateStatus == CompOffModelObj.FromDateStatus)
                                            {
                                                Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                                    + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                                PopulateDropDown();
                                                CompOffApplicationNecessaryContent(EmpId);
                                                return View(CompOffModelObj);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                            + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                        PopulateDropDown();
                                        CompOffApplicationNecessaryContent(EmpId);
                                        return View(CompOffModelObj);
                                    }
                                }
                                // Code added by Rajas on 27 SEP 2017 END

                                #endregion Validate OD Data
                            }
                            // Added by Rajas on 10 MARCH 2017 for validating is compoff already present for same date range as of leave END
                            #endregion

                            #region VALIDATE OD
                            // Added by Rajas on 10 MARCH 2017 for validating is OD already present for same date range as of leave START
                            if (ODTourTblObj != null)
                            {
                                #region Validate OD Data

                                // Code added by Rajas on 27 SEP 2017 START
                                if (CompoffApplicationObj.FromDate == CompoffApplicationObj.ToDate) // Single day
                                {
                                    if (CurrentDate == ODTourTblObj.FromDate)
                                    {
                                        if (CompoffApplicationObj.FromDateStatus == 1)  // Full day in single day
                                        {
                                            Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                                + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                            PopulateDropDown();
                                            CompOffApplicationNecessaryContent(EmpId);
                                            return View(CompOffModelObj);
                                        }
                                        else
                                        {
                                            if (ODTourTblObj.ODDayStatus == CompOffModelObj.FromDateStatus)
                                            {
                                                Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                                    + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                                    + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                                PopulateDropDown();
                                                CompOffApplicationNecessaryContent(EmpId);
                                                return View(CompOffModelObj);
                                            }
                                            else
                                            {
                                                if (CompOffModelObj.FromDateStatus == 1)
                                                {
                                                    Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                                        + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                                        + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                                    PopulateDropDown();
                                                    CompOffApplicationNecessaryContent(EmpId);
                                                    return View(CompOffModelObj);
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (CurrentDate == ODTourTblObj.FromDate)
                                    {
                                        if (ODTourTblObj.ODDayStatus == 1)  // Full day in single day
                                        {
                                            Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                                + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                                + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                            PopulateDropDown();
                                            CompOffApplicationNecessaryContent(EmpId);
                                            return View(CompOffModelObj);
                                        }
                                        else
                                        {
                                            if (ODTourTblObj.ODDayStatus == CompOffModelObj.FromDateStatus)
                                            {
                                                Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                                    + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                                    + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                                PopulateDropDown();
                                                CompOffApplicationNecessaryContent(EmpId);
                                                return View(CompOffModelObj);
                                            }
                                            else
                                            {
                                                if (CompOffModelObj.FromDateStatus == 1)
                                                {
                                                    Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                                        + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                                        + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                                    PopulateDropDown();
                                                    CompOffApplicationNecessaryContent(EmpId);
                                                    return View(CompOffModelObj);
                                                }
                                            }
                                        }
                                    }
                                    else if (CurrentDate == ODTourTblObj.ToDate)
                                    {
                                        if (ODTourTblObj.ODDayStatus == 1)  // Full day in single day
                                        {
                                            Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                                + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                                + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                            PopulateDropDown();
                                            CompOffApplicationNecessaryContent(EmpId);
                                            return View(CompOffModelObj);
                                        }
                                        else
                                        {
                                            if (ODTourTblObj.ODDayStatus == CompOffModelObj.FromDateStatus)
                                            {
                                                Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                                    + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                                    + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                                PopulateDropDown();
                                                CompOffApplicationNecessaryContent(EmpId);
                                                return View(CompOffModelObj);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                            + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                            + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                        PopulateDropDown();
                                        CompOffApplicationNecessaryContent(EmpId);
                                        return View(CompOffModelObj);
                                    }
                                }
                                // Code added by Rajas on 27 SEP 2017 END

                                #endregion Validate OD Data
                            }
                            // Added by Rajas on 10 MARCH 2017 for validating is compoff already present for same date range as of leave END
                            #endregion
                        }

                        #region VALIDATE FOR LOCKED DATA
                        // Added by Rajas on 10 MARCH 2017 to Validate if data is locked, can not apply leave START
                        DailyTransaction DailyTransactionListObj = WetosDB.DailyTransactions.Where(a => a.TranDate >= CompOffModelObj.FromDate && a.TranDate <= CompOffModelObj.ToDate && a.EmployeeId == EmpId && a.Lock == "Y").FirstOrDefault();

                        if (DailyTransactionListObj != null)
                        {
                            Error("You Can not apply Comp Off for this range as Data is Locked");

                            PopulateDropDown();
                            CompOffApplicationNecessaryContent(EmpId);
                            return View(CompOffModelObj);
                        }
                        // Added by Rajas on 10 MARCH 2017 to Validate if data is locked, can not apply leave END
                        #endregion

                        #endregion


                        //Added by Shraddha on 7 JAN 2017 START
                        TimeSpan ts = (Convert.ToDateTime(CompOffModelObj.ToDate)) - (Convert.ToDateTime(CompOffModelObj.FromDate));
                        float AppliedDay = ts.Days;
                        if (CompOffModelObj.FromDateStatus == 1 && CompOffModelObj.ToDateStatus == 1)
                        {
                            AppliedDay = AppliedDay + 1;
                        }

                            //Modified By Shraddha on 30 DEC 2016 for leave application validation changes start
                        else if ((CompOffModelObj.FromDateStatus == 2) && (CompOffModelObj.ToDateStatus == 2) && CompOffModelObj.FromDate == CompOffModelObj.ToDate)
                        {
                            AppliedDay = 0.5F;
                        }
                        else if ((CompOffModelObj.FromDateStatus == 3) && (CompOffModelObj.ToDateStatus == 3) && CompOffModelObj.FromDate == CompOffModelObj.ToDate)
                        {
                            AppliedDay = 0.5F;
                        }
                        else if ((CompOffModelObj.FromDateStatus == 2) && (CompOffModelObj.ToDateStatus == 3) && CompOffModelObj.FromDate == CompOffModelObj.ToDate)
                        {
                            AppliedDay = 1F;
                        }
                        else if ((CompOffModelObj.FromDateStatus == 3) && (CompOffModelObj.ToDateStatus == 2) && CompOffModelObj.FromDate == CompOffModelObj.ToDate)
                        {
                            ModelState.AddModelError("", "Please Select Proper from day Status and Today Status");
                            //PopulateDropDown();

                            CompOffApplicationNecessaryContent(EmpId);
                            PopulateDropDown();
                            //List<CompOffApplication> CompOffApplicationList = WetosDB.CompOffApplications.Where(a => a.EmployeeId == EmployeeNo).ToList();
                            return View(CompOffModelObj);
                        }

                            //Modified By Shraddha on 30 DEC 2016 for leave application validation changes End
                        else if ((CompOffModelObj.FromDateStatus == 2 || CompOffModelObj.FromDateStatus == 3) && (CompOffModelObj.ToDateStatus == 2 || CompOffModelObj.ToDateStatus == 3) && CompOffModelObj.FromDate != CompOffModelObj.ToDate)
                        {
                            AppliedDay = AppliedDay + 0;
                        }

                        else if (CompOffModelObj.FromDateStatus == 2 || CompOffModelObj.ToDateStatus == 2 || CompOffModelObj.FromDateStatus == 3 || CompOffModelObj.ToDateStatus == 3)
                        {
                            AppliedDay = (float)(AppliedDay + 0.5);
                        }

                        #region TOTAL ALLOWED COMP OFF MINUTES
                        //double TotalAllowedCompOff = Convert.ToDouble(Collection["TtlAllwdDys"]);
                        string TtlAllwdDys = (Collection["TtlAllwdDys"]).ToString();
                        int TotalAllowedCompOffTime = 0;
                        int TotalWorkingHours = 0;
                        if (!string.IsNullOrEmpty(TtlAllwdDys))
                        {
                            string[] TtlAllwdDysSplitValue = TtlAllwdDys.Split(':');
                            int TotalAllowedDaysHourInt = Convert.ToInt32(TtlAllwdDysSplitValue[0]);
                            int TotalAllowedDaysMinuteInt = Convert.ToInt32(TtlAllwdDysSplitValue[1]);

                            TotalAllowedCompOffTime = (60 * TotalAllowedDaysHourInt) + TotalAllowedDaysMinuteInt;
                        }
                        DateTime CompOffFromDate = Convert.ToDateTime(CompOffModelObj.FromDate);
                        DateTime CompOffToDate = Convert.ToDateTime(CompOffModelObj.ToDate);
                        for (DateTime CompOffDate = CompOffFromDate; CompOffDate <= CompOffToDate; CompOffDate = CompOffDate.AddDays(1))
                        {
                            int TotalWorkingHoursLocalRegion = 0;
                            #region  FIND NEAREST SHIFT FROM IN OUT AND MARK LATE / EARLY
                            string ShiftId = string.Empty;
                            Shift ShiftObjForCurrentEmployee = new Shift();

                            #region GET DAILY TRANSACTION TABLE RECORD FOR SELECTED DATE OF SELECTED EMPLOYEE
                            DailyTransaction DailyTransactionObj = WetosDB.DailyTransactions.Where(a => a.TranDate == CompOffDate
                                && a.EmployeeId == CompOffModelObj.EmployeeId).FirstOrDefault();
                            #endregion
                            if (DailyTransactionObj != null)
                            {
                                #region IF DATA AVAILABLE IN DAILY TRANSACTION TABLE THEN TAKE SHIFT FROM DT TABLE
                                ShiftId = DailyTransactionObj.ShiftId;
                                #endregion
                            }

                            else
                            {
                                #region IF DATA AVAILABLE IN DAILY TRANSACTION TABLE THEN TAKE SHIFT FROM DEFAULT SHIFT
                                if (EmployeeObj != null)
                                {
                                    ShiftId = EmployeeObj.DefaultShift;
                                }
                                #endregion
                            }
                            #endregion

                            if (ShiftId != null)
                            {
                                ShiftObjForCurrentEmployee = WetosDB.Shifts.Where(a => a.ShiftCode == ShiftId).FirstOrDefault();
                                if (ShiftObjForCurrentEmployee != null)
                                {
                                    int TotalAllowedDaysHourInt = ShiftObjForCurrentEmployee.WorkingHours.Hour;
                                    int TotalAllowedDaysMinuteInt = ShiftObjForCurrentEmployee.WorkingHours.Minute;
                                    TotalWorkingHoursLocalRegion = (60 * TotalAllowedDaysHourInt) + TotalAllowedDaysMinuteInt;
                                }
                                else
                                {
                                    Error("Shift data is not avaialable for " + CompOffDate);
                                    PopulateDropDown();
                                    CompOffApplicationNecessaryContent(EmpId);
                                    return View(CompOffModelObj);
                                }
                            }
                            else
                            {
                                Error("Shift data is not avaialable for " + CompOffDate);
                                PopulateDropDown();
                                CompOffApplicationNecessaryContent(EmpId);
                                return View(CompOffModelObj);
                            }
                            if (CompOffDate == CompOffFromDate && CompOffDate == CompOffToDate) //IF COMP OFF DATE IS FROM DATE
                            {
                                if (CompOffModelObj.FromDateStatus == 3 || CompOffModelObj.FromDateStatus == 2 || CompOffModelObj.ToDateStatus == 3 || CompOffModelObj.ToDateStatus == 2)
                                {
                                    TotalWorkingHoursLocalRegion = Convert.ToInt32(TotalWorkingHoursLocalRegion / 2);
                                }

                            }
                            else
                            {
                                if (CompOffDate == CompOffFromDate) //IF COMP OFF DATE IS FROM DATE
                                {
                                    if (CompOffModelObj.FromDateStatus == 3 || CompOffModelObj.FromDateStatus == 2)
                                    {
                                        TotalWorkingHoursLocalRegion = Convert.ToInt32(TotalWorkingHoursLocalRegion / 2);
                                    }

                                }
                                if (CompOffDate == CompOffToDate) //IF COMP OFF DATE IS TO DATE
                                {
                                    if (CompOffModelObj.ToDateStatus == 3 || CompOffModelObj.ToDateStatus == 2)
                                    {
                                        TotalWorkingHoursLocalRegion = Convert.ToInt32(TotalWorkingHoursLocalRegion / 2);
                                    }

                                }
                            }
                            TotalWorkingHours = TotalWorkingHoursLocalRegion + TotalWorkingHours;
                        }
                        #endregion

                        if (TotalWorkingHours > TotalAllowedCompOffTime)
                        {

                            ModelState.AddModelError("", "You Can't Apply Comp Off for more than allowed days");
                            CompOffApplicationNecessaryContent(EmpId);
                            PopulateDropDown();
                            //List<CompOffApplication> CompOffApplicationList = WetosDB.CompOffApplications.Where(a => a.EmployeeId == EmployeeNo).ToList();
                            return View(CompOffModelObj);

                        }

                        if (TotalWorkingHours < TotalAllowedCompOffTime)
                        {
                            int TotalBalanceWorkingHours = TotalWorkingHours;
                            if (CompOffList != null && CompOffList.Count() > 0)
                            {
                                CompOffList = CompOffList.OrderByDescending(a => a.CompOffId).ToList();
                                foreach (CompOff CompOffObj in CompOffList)
                                {
                                    if (TotalBalanceWorkingHours > 0)
                                    {
                                        int BalanceCompOffHour = CompOffObj.BalanceCoHours == null ? 0 : CompOffObj.BalanceCoHours.Value.Hour;
                                        int BalanceCompOffMinute = CompOffObj.BalanceCoHours == null ? 0 : CompOffObj.BalanceCoHours.Value.Hour;
                                        int BalanceCompOffTotalTime = (BalanceCompOffHour * 60) + BalanceCompOffMinute;
                                        if (TotalBalanceWorkingHours >= BalanceCompOffTotalTime)
                                        {
                                            TotalBalanceWorkingHours = TotalBalanceWorkingHours - BalanceCompOffTotalTime;
                                            BalanceCompOffTotalTime = BalanceCompOffTotalTime - BalanceCompOffTotalTime;
                                            int BalanceCompOffMinuteInt = BalanceCompOffTotalTime % 60;
                                            int BalanceCompOffHourInt = (BalanceCompOffTotalTime - BalanceCompOffMinuteInt) / 60;
                                            if (CompOffObj.BalanceCoHours != null)
                                            {
                                                CompOffObj.BalanceCoHours = new DateTime(CompOffObj.BalanceCoHours.Value.Year, CompOffObj.BalanceCoHours.Value.Month, CompOffObj.BalanceCoHours.Value.Day,
                                                   BalanceCompOffHourInt, BalanceCompOffMinuteInt, 0);
                                                WetosDB.SaveChanges();
                                            }

                                        }
                                        else
                                        {
                                            TotalBalanceWorkingHours = TotalBalanceWorkingHours - TotalBalanceWorkingHours;
                                            BalanceCompOffTotalTime = BalanceCompOffTotalTime - TotalBalanceWorkingHours;
                                            int TotalBalanceWorkingHoursMinuteInt = TotalBalanceWorkingHours % 60;
                                            int TotalBalanceWorkingHoursHourInt = (TotalBalanceWorkingHours - TotalBalanceWorkingHoursMinuteInt) / 60;
                                            if (CompOffObj.BalanceCoHours != null)
                                            {
                                                CompOffObj.BalanceCoHours = new DateTime(CompOffObj.BalanceCoHours.Value.Year, CompOffObj.BalanceCoHours.Value.Month, CompOffObj.BalanceCoHours.Value.Day,
                                                   TotalBalanceWorkingHoursHourInt, TotalBalanceWorkingHoursMinuteInt, 0);
                                                WetosDB.SaveChanges();
                                            }
                                            TotalBalanceWorkingHours = TotalBalanceWorkingHours - TotalBalanceWorkingHours;
                                        }
                                    }
                                }
                            }
                        }


                        CompOffApplication COMPOFFAPPLICATIONObj = new CompOffApplication();

                        //added by shraddha on 10 jan 2017 
                        COMPOFFAPPLICATIONObj.BranchId = EmployeeObj.BranchId;
                        COMPOFFAPPLICATIONObj.CompanyId = EmployeeObj.CompanyId;
                        COMPOFFAPPLICATIONObj.CompOffId = CompOffModelObj.CompOffId;

                        //added by shraddha on 10 jan 2017 
                        COMPOFFAPPLICATIONObj.EmployeeId = EmpId;
                        COMPOFFAPPLICATIONObj.ExtrasHoursDate = CompOffModelObj.ExtrasHoursDate;
                        COMPOFFAPPLICATIONObj.FromDate = Convert.ToDateTime(CompOffModelObj.FromDate);
                        COMPOFFAPPLICATIONObj.ToDate = Convert.ToDateTime(CompOffModelObj.ToDate);
                        COMPOFFAPPLICATIONObj.FromDateStatus = CompOffModelObj.FromDateStatus;
                        COMPOFFAPPLICATIONObj.Purpose = CompOffModelObj.Purpose;
                        COMPOFFAPPLICATIONObj.RejectReason = CompOffModelObj.RejectReason;
                        COMPOFFAPPLICATIONObj.StatusId = CompOffModelObj.StatusId;
                        COMPOFFAPPLICATIONObj.ToDateStatus = CompOffModelObj.ToDateStatus;

                        //Added By Shraddha on 12 DEC 2016 for taking Status along with statusId Start
                        COMPOFFAPPLICATIONObj.Status = WetosDB.DropdownDatas.Where(a => a.TypeId == 3 && a.Value == CompOffModelObj.StatusId).Select(a => a.Text).FirstOrDefault();
                        //Added By Shraddha on 12 DEC 2016 for taking Status along with statusId End

                        WetosDB.CompOffApplications.AddObject(COMPOFFAPPLICATIONObj);

                        WetosDB.SaveChanges();

                        foreach (int CheckCompOffIdObj in CheckCompOffIdList)
                        {
                            CompOff CompOffObjForUpdateStatus = WetosDB.CompOffs.Where(a => a.CompOffId == CheckCompOffIdObj).FirstOrDefault();
                            CompOffObjForUpdateStatus.AppStatus = "P";
                            CompOffObjForUpdateStatus.CoDate = Convert.ToDateTime(CompOffModelObj.FromDate);
                            CompOffObjForUpdateStatus.CompOffApplicationID = COMPOFFAPPLICATIONObj.CompOffId;
                            WetosDB.SaveChanges();
                        }



                        //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 9 SEPTEMBER 2017
                        #region ADD AUDIT LOG
                        //OLD RECORD IS BLANK
                        string Newrecord = "Employee ID : " + EmpId
                            + "FromDate :" + COMPOFFAPPLICATIONObj.FromDate + "To Date :" + COMPOFFAPPLICATIONObj.ToDate
                            + "FromDateStatus :" + COMPOFFAPPLICATIONObj.FromDateStatus + "ToDateStatus :" + COMPOFFAPPLICATIONObj.ToDateStatus
                            + "Purpose :" + COMPOFFAPPLICATIONObj.Purpose + " Status : " + COMPOFFAPPLICATIONObj.Status + "Reject Reason :"
                            + COMPOFFAPPLICATIONObj.RejectReason + "ExtraHoursDate :" + COMPOFFAPPLICATIONObj.ExtrasHoursDate + "Branch ID :"
                            + COMPOFFAPPLICATIONObj.BranchId + "Company ID:" + COMPOFFAPPLICATIONObj.CompanyId;

                        //LOGIN ID IS TAKEN FROM SESSION PERSISTER
                        string Formname = "CUMULATIVE COMPOFF APPLICATION";
                        //ACTION IS UPDATE
                        string Message = " ";

                        WetosAdministrationController.GenerateAuditLogsInsert(WetosDB, EmpId, Formname, Newrecord, ref Message);
                        #endregion
                        //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 9 SEPTEMBER 2017



                        // NOTIFICATION ADDED BY RAJAS ON 31 DEC 2016
                        #region COMPOFF APPLICATION NOTIFICATION

                        // FOR NOTIFICATION TO BE SENT TO REPORTING PERSON

                        //Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == COMPOFFAPPLICATIONObj.EmployeeId).FirstOrDefault();
                        //Notification NotificationObj = new Notification();
                        //NotificationObj.FromID = EmployeeObj.EmployeeId;
                        //NotificationObj.ToID = EmployeeObj.EmployeeReportingId;
                        //NotificationObj.SendDate = DateTime.Now;
                        //NotificationObj.NotificationContent = "CompOff applied by " + EmployeeObj.FirstName + " " + EmployeeObj.LastName + " is pending for approval";
                        //NotificationObj.ReadFlag = false;
                        //NotificationObj.SendDate = DateTime.Now;

                        //WetosDB.Notifications.AddObject(NotificationObj);

                        //WetosDB.SaveChanges();

                        //// Check if both reporting person are are different
                        //if (EmployeeObj.EmployeeReportingId != EmployeeObj.EmployeeReportingId2)
                        //{
                        //    // FOR NOTIFICATION TO BE SENT TO REPORTING PERSON AS SANCTIONER

                        //    //Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == LeaveTypeObj.EmployeeId).FirstOrDefault();
                        //    Notification NotificationObj3 = new Notification();
                        //    NotificationObj3.FromID = EmployeeObj.EmployeeId;
                        //    NotificationObj3.ToID = EmployeeObj.EmployeeReportingId;
                        //    NotificationObj3.SendDate = DateTime.Now;
                        //    NotificationObj3.NotificationContent = "CompOff applied by " + EmployeeObj.FirstName + " " + EmployeeObj.LastName + " is pending for approval";
                        //    NotificationObj3.ReadFlag = false;
                        //    NotificationObj3.SendDate = DateTime.Now;

                        //    WetosDB.Notifications.AddObject(NotificationObj3);

                        //    WetosDB.SaveChanges();
                        //}

                        ////FOR SELF NOTIFICATION

                        //Notification NotificationObj2 = new Notification();
                        //NotificationObj2.FromID = EmployeeObj.EmployeeId;
                        //NotificationObj2.ToID = EmployeeObj.EmployeeId;
                        //NotificationObj2.SendDate = DateTime.Now;
                        //NotificationObj2.NotificationContent = "CompOff applied successfully";
                        //NotificationObj2.ReadFlag = false;
                        //NotificationObj2.SendDate = DateTime.Now;

                        //WetosDB.Notifications.AddObject(NotificationObj2);

                        //WetosDB.SaveChanges();

                        Notification NotificationObj = new Notification();
                        NotificationObj.FromID = EmployeeObj.EmployeeId;
                        NotificationObj.ToID = EmployeeObj.EmployeeReportingId;
                        NotificationObj.SendDate = DateTime.Now;
                        NotificationObj.NotificationContent = "Received CompOff application for approval from " + EmployeeObj.FirstName + " " + EmployeeObj.LastName + " dated on " + COMPOFFAPPLICATIONObj.FromDate.Value.ToString("dd-MMM-yyyy") + " to " + COMPOFFAPPLICATIONObj.ToDate.Value.ToString("dd-MMM-yyyy");
                        NotificationObj.ReadFlag = false;
                        NotificationObj.SendDate = DateTime.Now;

                        WetosDB.Notifications.AddObject(NotificationObj);

                        WetosDB.SaveChanges();

                        // NOTIFICATION COUNT
                        int NoTiCount = WetosDB.Notifications.Where(a => a.ToID == EmpId && (a.ReadFlag == false || a.ReadFlag == null)).Count();
                        Session["NotificationCount"] = NoTiCount;

                        #endregion

                        // Code updated by Rajas on 13 JUNE 2017
                        #region EMAIL
                        // Added by Rajas on 19 JULY 2017 START
                        //GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == "Send Email").FirstOrDefault();

                        //ABOVE LINE COMMENTED AND BELOW LINE IS ADDED BY SHALAKA ON 13TH DEC 2017
                        GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.SendEmail).FirstOrDefault();

                        if (GlobalSettingObj != null)
                        {
                            // Send email ON
                            if (GlobalSettingObj.SettingValue == "1")
                            {

                                // Added by Rajas on 30 MARCH 2017 START
                                string EmailUpdateStatus = string.Empty;

                                if (EmployeeObj.Email != null)
                                {
                                    // Updated by Rajas on 19 JULY 2017 extra added parameter EmailFromWhichApplication
                                    if (SendEmail(EmployeeObj.Email, NotificationObj.NotificationContent, NotificationObj.NotificationContent, ref EmailUpdateStatus, "CompOff Application") == false)
                                    {
                                        Error(EmailUpdateStatus);

                                    }
                                }
                                else
                                {
                                    //Information("Email Id is not Provided; Please Provide Email Id to your Admin");
                                    AddAuditTrail("Unable to send email, as Email Id not available for " + EmployeeObj.EmployeeCode);
                                }
                                // Added by Rajas on 30 MARCH 2017 END
                            }
                            else // Send email OFF
                            {
                                AddAuditTrail("Unable to send Email as email utility is disabled");
                            }
                        }
                        // Added by Rajas on 19 JULY 2017 END
                        #endregion

                        // ADDED BY PUSHKAR FOR AuditLog ON 21 DEC 2016
                        AddAuditTrail("Success - New CompOff application created successfully");  // Updated by Rajas on 17 JAN 2017

                        // Added by Rajas on 17 JAN 2017 START
                        Success("Success - New CompOff application created successfully");

                        return RedirectToAction("CompOffApplicationIndex");
                    }
                    else
                    {
                        CompOffApplicationNecessaryContent(EmpId);

                        //List<CompOffApplication> CompOffApplicationList = WetosDB.CompOffApplications.Where(a => a.EmployeeId == EmployeeNo).ToList();


                        // ADDED BY PUSHKAR FOR AuditLog ON 21 DEC 2016
                        AddAuditTrail("Error - New CompOff application failed");  // Updated by Rajas on 17 JAN 2017

                        // Added by Rajas on 17 JAN 2017 START
                        Error("Error - New CompOff application failed");

                        PopulateDropDown();

                        return View(CompOffModelObj);
                    }
                }
                else
                {
                    AddAuditTrail("Error - New CompOff application failed");  // Updated by Rajas on 17 JAN 2017

                    // Added by Rajas on 17 JAN 2017 START
                    Error("No Pending comp off for Employee: " + EmployeeObj.FirstName + " " + EmployeeObj.LastName);

                    PopulateDropDown();

                    return View(CompOffModelObj);

                }
            }
            catch (System.Exception ex)
            {
                CompOffApplicationNecessaryContent(EmpId);

                // ADDED BY PUSHKAR FOR AuditLog ON 21 DEC 2016
                AddAuditTrail("Exception - New CompOff application failed due to " + ex.Message);  // Updated by Rajas on 17 JAN 2017

                // Added by Rajas on 17 JAN 2017 START
                Error("Exception - New CompOff application failed due to " + ex.Message);

                PopulateDropDown();

                return View(CompOffModelObj);
            }
            #endregion
        }

        /// <summary>
        /// FUNCTION ADDED BY SHRADDHA ON 09 MARCH 2017
        /// </summary>
        /// <returns></returns>
        private bool CompOffApplicationNecessaryContent(int EmployeeId)
        {
            bool ReturnStatus = false;

            int EmployeeGroupId = WetosDB.EmployeeGroupDetails.Where(a => a.Employee.EmployeeId == EmployeeId).Select(a => a.EmployeeGroup.EmployeeGroupId).FirstOrDefault();

            string EmployeeGroupName = WetosDB.EmployeeGroups.Where(a => a.EmployeeGroupId == EmployeeGroupId).Select(a => a.EmployeeGroupName).FirstOrDefault();

            //CODE ADDED BY SHRADDHA ON 20 SEP 2017 FOR CO VALIDITY DAYS START
            ViewBag.COValidityDays = 365;
            //GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == "CompOff Validity Days").FirstOrDefault();

            //ABOVE LINE COMMENTED AND BELOW LINE IS ADDED BY SHALAKA ON 13TH DEC 2017
            GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.CompOffValidityDays).FirstOrDefault();

            if (GlobalSettingObj != null)
            {
                ViewBag.COValidityDays = GlobalSettingObj.SettingValue;
            }
            //CODE ADDED BY SHRADDHA ON 20 SEP 2017 FOR CO VALIDITY DAYS END

            Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == EmployeeId).FirstOrDefault();
            List<RuleTransaction> RuleTransactionList = WetosDB.RuleTransactions.Where(a => a.EmployeeGroupId == EmployeeGroupId).ToList();

            RuleTransaction RuleTransactionObj = RuleTransactionList.Where(a => a.RuleId == 9).FirstOrDefault(); //9 - COMP OFF IS AVAIALABLE OR NOT
            RuleTransaction RTObjForCompOffFullDayLimitValue = RuleTransactionList.Where(a => a.RuleId == 10).FirstOrDefault();
            RuleTransaction RTObjForCompOffHalfDayLimitValue = RuleTransactionList.Where(a => a.RuleId == 11).FirstOrDefault();
            //ADDED NEW LOGIC BY SHRADDHA ON 09 MAR 2017 END

            //ADDED NEW LOGIC BY SHRADDHA ON 10 JAN 2017
            double ActualStatusDoubleforfullday = 0;
            double ActualStatusDoubleforhalfday = 0;

            //CODE ADDED BY SHRADDHA ON 09 MARCH 2017
            DateTime NewDateForFullDay = new DateTime();
            DateTime NewDateForHalfDay = new DateTime();

            List<CompOff> AttendanceStatusList = new List<CompOff>();
            if (RuleTransactionObj != null)
            {
                if (RuleTransactionObj.Formula.ToUpper().Trim() == "TRUE")
                {
                    //CODE ADDED BY SHRADDHA ON 20 FEB 2018 START
                    RuleTransaction ExtraHoursAccumulationAllowedornot = WetosDB.RuleTransactions.Where(a => a.RuleId == 34).FirstOrDefault(); // 34 - Extra Hours Accumulation Allowed or not //CODE ADDED BY SHRADDHA ON 19 FEB 2018
                    RuleTransaction MinRequiredExtraHoursForAccumulation = WetosDB.RuleTransactions.Where(a => a.RuleId == 35).FirstOrDefault(); //35 - Min. required extra hours for accumulation //CODE ADDED BY SHRADDHA ON 19 FEB 2018

                    if (ExtraHoursAccumulationAllowedornot != null && MinRequiredExtraHoursForAccumulation != null)
                    {
                        string[] MinRequiredExtraHoursForAccumulationSplitValue = MinRequiredExtraHoursForAccumulation.Formula.Split(':');
                        int AccumulationHourInt = 0;
                        int AccumulationMinuteInt = 0;
                        double MinTotalTimeRequiredForAccumulation = 0;
                        AccumulationHourInt = Convert.ToInt32(MinRequiredExtraHoursForAccumulationSplitValue[0]); // Minutes missing
                        AccumulationMinuteInt = Convert.ToInt32(MinRequiredExtraHoursForAccumulationSplitValue[1]); // Minutes missing
                        MinTotalTimeRequiredForAccumulation = (60 * AccumulationHourInt) + AccumulationMinuteInt;
                        AttendanceStatusList = WetosDB.CompOffs.Where(a => a.EmployeeId == EmployeeId && (a.CoHours.Value.Hour * 60 + a.CoHours.Value.Minute) >= MinTotalTimeRequiredForAccumulation).ToList();
                    }
                    else
                    {
                        //CODE ADDED BY SHRADDHA ON 20 FEB 2018 END
                        if (RTObjForCompOffFullDayLimitValue != null)
                        {
                            string[] FullDayRuleSplitValue = RTObjForCompOffFullDayLimitValue.Formula.Split(':');
                            int FullDayRuleHourInt = Convert.ToInt32(FullDayRuleSplitValue[0]);
                            int FullDayRuleMinuteInt = Convert.ToInt32(FullDayRuleSplitValue[1]);
                            int FullDayRuleSecondInt = Convert.ToInt32(FullDayRuleSplitValue[2]);

                            int CurrentYearInt = DateTime.Now.Year; // ADDED BY MSJ ON 02 JAN 2018
                            NewDateForFullDay = new DateTime(CurrentYearInt, 01, 01, FullDayRuleHourInt, FullDayRuleMinuteInt, FullDayRuleSecondInt);  // ADDED BY MSJ ON 02 JAN 2018


                        }
                        if (RTObjForCompOffHalfDayLimitValue != null)
                        {
                            string[] HalfDayRuleSplitValue = RTObjForCompOffHalfDayLimitValue.Formula.Split(':');
                            int HalfDayRuleHourInt = Convert.ToInt32(HalfDayRuleSplitValue[0]);
                            int HalfDayRuleMinuteInt = Convert.ToInt32(HalfDayRuleSplitValue[1]);
                            int HalfDayRuleSecondInt = Convert.ToInt32(HalfDayRuleSplitValue[2]);

                            int CurrentYearInt = DateTime.Now.Year; // ADDED BY MSJ ON 02 JAN 2018
                            NewDateForHalfDay = new DateTime(CurrentYearInt, 01, 01, HalfDayRuleHourInt, HalfDayRuleMinuteInt, HalfDayRuleSecondInt);
                        }
                        AttendanceStatusList = WetosDB.CompOffs.Where(a => a.EmployeeId == EmployeeId && (a.CoHours.Value.Hour * 60 + a.CoHours.Value.Minute) >= (NewDateForHalfDay.Hour * 60 + NewDateForHalfDay.Minute)).ToList();
                    }

                    ReturnStatus = true;
                }
                else
                {
                    Error("Comp Off is not Allowed for Employee Group : " + EmployeeGroupName);
                    ReturnStatus = false;
                    return ReturnStatus;

                }
            }
            else
            {
                Error("Please set rules related to Comp Off for Employee Group : " + EmployeeGroupName);

                ReturnStatus = false;
                return ReturnStatus;

            }


            //CODE UNCOMMENTED BY SHRADDHA ON 09 MARCH 2017 START

            if (AttendanceStatusList != null && AttendanceStatusList.Count > 0)  //modified by pushkar for wrong exception shown on create and to display count 
            {
                double actualstatusdoubleobj = 0;
                foreach (var compoffobj in AttendanceStatusList)
                {
                    if (compoffobj.CoHours != null)
                    {
                        if (compoffobj.CoHours.Value.TimeOfDay >= NewDateForHalfDay.TimeOfDay && (compoffobj.CoHours.Value.Hour * 60 + compoffobj.CoHours.Value.Minute) < (NewDateForFullDay.Hour * 60 + NewDateForFullDay.Minute))
                        {
                            ActualStatusDoubleforhalfday = 0.5;
                            actualstatusdoubleobj = actualstatusdoubleobj + ActualStatusDoubleforhalfday;
                        }
                        else if (compoffobj.CoHours.Value.TimeOfDay >= NewDateForFullDay.TimeOfDay)
                        {
                            ActualStatusDoubleforfullday = 1;
                            actualstatusdoubleobj = ActualStatusDoubleforfullday + actualstatusdoubleobj;
                        }

                    }

                    foreach (var AttendanceStatusObj in AttendanceStatusList)
                    {
                        AttendanceStatusObj.DayStatus = AttendanceStatusObj.DayStatus == null ? "" : AttendanceStatusObj.DayStatus.Trim();
                    }


                    ViewBag.AllowedCompOffObjVB = 0;
                    ViewBag.AttendanceStatusListVB = AttendanceStatusList;
                    ViewBag.AttendanceStatusCountVB = AttendanceStatusList.Count();
                }
            }
            else
            {
                ViewBag.AllowedCompOffObjVB = 0;
                ViewBag.AttendanceStatusListVB = 0;
                ViewBag.AttendanceStatusCountVB = 0;

            }
            ReturnStatus = true;
            return ReturnStatus;
        }

        /// <summary>
        /// To Populate Dropdown
        /// Added By shraddha on 17 OCT 2016
        /// </summary>
        public void PopulateDropDown(bool IsSelf = true)
        {
            //// Updated by Rajas on 6 MAY 2017 to get data which is not MarkedAsDelete
            //var CompanyObj = WetosDB.Companies.Where(a => a.MarkedAsDelete == null || a.MarkedAsDelete == 0).Select(a => new { CompanyId = a.CompanyId, CompanyName = a.CompanyName }).ToList();
            //ViewBag.CompanyList = new SelectList(CompanyObj, " CompanyId", "CompanyName").ToList();

            //var BranchObj = WetosDB.Branches.Where(a => a.MarkedAsDelete == null || a.MarkedAsDelete == 0).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList();
            //ViewBag.BranchList = new SelectList(BranchObj, " BranchId", "BranchName").ToList();

            //Added by shraddha on 14th oct 2016 start

            //var EmployeeObj = WetosDB.Employees.Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.FirstName + " " + a.LastName }).ToList();

            //ViewBag.EmployeeList = new SelectList(EmployeeObj, "EmployeeId", "EmployeeName").ToList();

            var RequisitionTYpeObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 1).Select(a => new { ODTourId = a.Text, ODTourType = a.Text }).ToList();

            ViewBag.RequisitionTYpeList = new SelectList(RequisitionTYpeObj, "ODTourId", "ODTourType").ToList();

            //var EmployeeCodeObj = WetosDB.Employees.Select(a => new { EmployeeId = a.EmployeeId, EmployeeCode = a.EmployeeCode }).ToList();

            //ViewBag.EmployeeCodeList = new SelectList(EmployeeCodeObj, "EmployeeId", "EmployeeCode").ToList();

            // Added by Rajas on 22 OCT 2016 for Dropdown list for FULL DAY or HALF DAY Leave
            var LeaveTypeStatusObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 9).Select(a => new { LeaveStatusID = a.Value, LeaveStatus = a.Text }).ToList();
            ViewBag.LeaveTypeStatusList = new SelectList(LeaveTypeStatusObj, "LeaveStatusID", "LeaveStatus").ToList();

            //Added by Shraddha on 17 DEC 2016 for displaying employee code and name both in dropdown
            //    var EmployeeCodeAndNameObj = WetosDB.VwActiveEmployees.Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + "   " + a.FirstName + " " + a.MiddleName + " " + a.LastName }).ToList();

            //Added by Shalaka on 26th OCT 2017

            #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
            int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            //var EmployeeCodeAndNameObj = WetosDB.VwActiveEmployees.Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
            var EmployeeCodeAndNameObj = WetosDB.SP_VwActiveEmployee(EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
            #endregion

            ViewBag.EmployeeCodeAndNameList = new SelectList(EmployeeCodeAndNameObj, "EmployeeId", "EmployeeName").ToList();

            //ADDED BY SHRADDHA ON 28 DEC 2016 FOR SELECT CRITERIA START
            List<string> SelectCriteriaObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 5).Select(a => a.Text).ToList();
            ViewBag.SelectCriteriaList = SelectCriteriaObj;
            //ADDED BY SHRADDHA ON 28 DEC 2016 FOR SELECT CRITERIA END

            // Added by Rajas on 19 MAY 2017
            var ODpurpose = WetosDB.DropdownDatas.Where(a => a.TypeId == 15).Select(a => new { ODPurposeId = a.Text, ODPurposeType = a.Text }).ToList();
            ViewBag.ODPurposeList = new SelectList(ODpurpose, "ODPurposeId", "ODPurposeType").ToList();

            //Added on 22nd DEC 2017 for getting only allowed encashment leave type-- Start
            var LeaveCodeObj = WetosDB.LeaveMasters.Where(a => a.EncashmentAllowedOrNot == true).Select(a => new { LeaveCode = a.LeaveCode, LeaveName = a.LeaveName }).Distinct().ToList();
            //Added on 22nd DEC 2017 for getting only allowed encashment leave type-- End

            ViewBag.LeaveCodeList = new SelectList(LeaveCodeObj, " LeaveCode", "LeaveName").ToList();

            var StatusObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 3).Select(a => new { Value = a.Value, Text = a.Text }).ToList();
            ViewBag.StatusList = new SelectList(StatusObj, "Value", "Text").ToList();

        }


        /// <summary>
        /// SEND MAIL
        /// Added by Rajas on 31 MARCH 2017
        /// </summary>
        /// <param name="ToEmail"></param>
        /// <param name="Subject"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        /// Updated by Rajas on 22 AUGUST 2017
        public bool SendEmail(string ToEmail, string Subject, string Message, ref string UpdateStatus, string EmailFromWhichApplication)
        {
            // SEND EMAIL START
            string SMTPServerName = ConfigurationManager.AppSettings["SMTPServerName"];
            string SMTPUsername = ConfigurationManager.AppSettings["SMTPUsername"];
            string SMTPPassword = ConfigurationManager.AppSettings["SMTPPassword"];
            string SMTPPort = ConfigurationManager.AppSettings["SMTPPort"];

            int SMTPPortInt = Convert.ToInt32(SMTPPort);
            bool ReturnStatus = false;

            try
            {
                MailMessage msg = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                MailAddress from = new MailAddress(SMTPUsername); //"info@foodpatronservices.com");
                StringBuilder sb = new StringBuilder();

                //TO
                msg.To.Add(ToEmail); //"mjoshi@sushmatechnology.com");
                //msg.To.Add(Contact1); //"mjoshi@sushmatechnology.com");
                //msg.To.Add(email); //"mjoshi@sushmatechnology.com");

                //BCC
                //msg.Bcc.Add(Support);
                msg.Subject = Subject; // "Contact Us";

                msg.IsBodyHtml = false;

                msg.From = new MailAddress(SMTPUsername); //"info@foodpatronservices.com");

                smtp.Host = SMTPServerName; // "smtpout.asia.secureserver.net";

                smtp.EnableSsl = true;

                //smtp.Credentials = new System.Net.NetworkCredential("info@foodpatronservices.com", "info@fps");
                smtp.Credentials = new System.Net.NetworkCredential(SMTPUsername, SMTPPassword);//"info@foodpatronservices.com", "info@fps");

                smtp.Port = SMTPPortInt; // 25;


                //sb.Append("Name: " + name);
                //sb.Append(Environment.NewLine);
                //sb.Append("Email: " + email);
                //sb.Append(Environment.NewLine);
                //sb.Append("Subject: " + subject);
                //sb.Append(Environment.NewLine);
                //sb.Append("Comments: " + message);
                //Message

                msg.Body = Message; // sb.ToString();

                smtp.Send(msg);

                msg.Dispose();

                return ReturnStatus = true;

                //return RedirectToAction("Index", "Home");
            }
            catch (System.Exception ex)
            {
                // Updated by Rajas on 19 JULY 2017 
                // Added EmailFromWhichApplication where values for EmailFromWhichApplication will be Leave Application, Od/travel, ... etc 
                AddAuditTrail("Error in sending email in " + EmailFromWhichApplication + " : " + " due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                UpdateStatus = "Unable to send email!";

                return ReturnStatus;

                //return View("Error");
            }

            // SEND EMAIL END

            //return View();
        }


        /// <summary>
        /// CODE ADDED BY SHRADDHA ON 21 FEB 2018
        /// TO ADD COMP OFF HOURS
        /// <param name="CompOffId"></param>
        /// <param name="TtlAllwdDys"></param>
        /// <param name="BalanceCoHoursTime"></param>
        /// <param name="BalanceCoHours"></param>
        /// <returns></returns>
        [HttpPost]
        public string CompOffAddHours(string TtlAllwdDys, string BalanceCoHours)
        {
            int TotalAllowedTimeInt = 0;
            int BalanceCoHoursInt = 0;
            if (!string.IsNullOrEmpty(TtlAllwdDys))
            {
                string[] TtlAllwdDysSplitValue = TtlAllwdDys.Split(':');
                int TotalAllowedDaysHourInt = Convert.ToInt32(TtlAllwdDysSplitValue[0]);
                int TotalAllowedDaysMinuteInt = Convert.ToInt32(TtlAllwdDysSplitValue[1]);
                TotalAllowedTimeInt = (60 * TotalAllowedDaysHourInt) + TotalAllowedDaysMinuteInt;
            }
            if (!string.IsNullOrEmpty(BalanceCoHours))
            {
                string[] BalanceCoHoursSplitValue = BalanceCoHours.Split(':');
                int BalanceCoHoursHourInt = Convert.ToInt32(BalanceCoHoursSplitValue[0]);
                int BalanceCoHoursMinuteInt = Convert.ToInt32(BalanceCoHoursSplitValue[1]);
                BalanceCoHoursInt = (60 * BalanceCoHoursHourInt) + BalanceCoHoursMinuteInt;
            }
            int TotalAllowedDaysInt = BalanceCoHoursInt + TotalAllowedTimeInt;
            int TotalAllowedDaysMinutes = TotalAllowedDaysInt % 60;
            int TotalAllowedDaysHours = (TotalAllowedDaysInt - TotalAllowedDaysMinutes) / 60;
            string TotalAllowedDaysStr = TotalAllowedDaysHours + ":" + TotalAllowedDaysMinutes;
            return TotalAllowedDaysStr;
        }

        /// <summary>
        /// CODE ADDED BY SHRADDHA ON 21 FEB 2018
        /// TO REMOVE COMP OFF HOURS
        /// <param name="CompOffId"></param>
        /// <param name="TtlAllwdDys"></param>
        /// <param name="BalanceCoHoursTime"></param>
        /// <param name="BalanceCoHours"></param>
        /// <returns></returns>
        [HttpPost]
        public string CompOffRemoveHours(string TtlAllwdDys, string BalanceCoHours)
        {
            int TotalAllowedTimeInt = 0;
            int BalanceCoHoursInt = 0;
            if (!string.IsNullOrEmpty(TtlAllwdDys))
            {
                string[] TtlAllwdDysSplitValue = TtlAllwdDys.Split(':');
                int TotalAllowedDaysHourInt = Convert.ToInt32(TtlAllwdDysSplitValue[0]);
                int TotalAllowedDaysMinuteInt = Convert.ToInt32(TtlAllwdDysSplitValue[1]);

                TotalAllowedTimeInt = (60 * TotalAllowedDaysHourInt) + TotalAllowedDaysMinuteInt;
            }
            if (!string.IsNullOrEmpty(BalanceCoHours))
            {
                string[] BalanceCoHoursSplitValue = BalanceCoHours.Split(':');
                int BalanceCoHoursHourInt = Convert.ToInt32(BalanceCoHoursSplitValue[0]);
                int BalanceCoHoursMinuteInt = Convert.ToInt32(BalanceCoHoursSplitValue[1]);
                BalanceCoHoursInt = (60 * BalanceCoHoursHourInt) + BalanceCoHoursMinuteInt;
            }
            int TotalAllowedDaysInt = TotalAllowedTimeInt - BalanceCoHoursInt;
            int TotalAllowedDaysMinutes = TotalAllowedDaysInt % 60;
            int TotalAllowedDaysHours = (TotalAllowedDaysInt - TotalAllowedDaysMinutes) / 60;
            string TotalAllowedDaysStr = TotalAllowedDaysHours + ":" + TotalAllowedDaysMinutes;
            return TotalAllowedDaysStr;
        }
        /// <summary>
        /// ADDED BY SHRADDHA ON 09 SEP 2017
        /// GET
        /// MANUAL COMP OFF SANCTION LIST
        /// <returns></returns>
        public ActionResult CumulativeCOSanctionIndex(int selectCriteria = 0)
        {
            try
            {
                int EmpNo = Convert.ToInt32(Session["EmployeeNo"]);
                //int Id = Convert.ToInt32(Session["Id"]);

                // Added by Rajas on 9 MAY 2017 START
                // Select Criteria as per dropdown value
                int Status = 0;

                // Updated by Rajas on 20 MAY 2017 START
                List<SP_CumulativeManualCOSanctionIndex_Result> ManualCOSanctionList = new List<SP_CumulativeManualCOSanctionIndex_Result>();

                #region UPDATED BY RAJAS ON 23 MAY 2017
                // Get current FY from global setting

                //GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == "Current Financial Year").FirstOrDefault();

                //ABOVE LINE COMMENTED AND BELOW LINE IS ADDED BY SHALAKA ON 13TH DEC 2017
                GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.CurrentFinancialYear).FirstOrDefault();

                // Error
                if (GlobalSettingObj == null)
                {
                    Error("Inconsistent Financial year data");
                    //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 START
                    PopulateDropDown();
                    //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 END
                    return View(ManualCOSanctionList);
                }

                DateTime CalanderStartDate = GetFYStartDate(); // DateTime CalanderStartDate = WetosDB.FinancialYears.Where(a => a.FinancialName == GlobalSettingObj.SettingValue).Select(a => a.StartDate).FirstOrDefault();

                if (CalanderStartDate != null)
                {

                    //CODE REPLACED BY SHRADDHA ON 12 SEP 2017 REMOVED REDUNDANT AND UNNECESSARY IF ELSE CODE AND TAKEN ONE SINGLE LINE CODE START
                    Status = selectCriteria;
                    ManualCOSanctionList = WetosDB.SP_CumulativeManualCOSanctionIndex(EmpNo).Where(a => ((a.EmployeeReportingId == EmpNo || a.EmployeeReportingId2 == EmpNo) && a.Id == Status && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null) && a.CompOffBalance == 0) //ADDED COMP OFF BALANCE CONDITION BY SHRADDHA ON 05 MAR 2018
                            .OrderByDescending(a => a.FromDate).OrderByDescending(a => a.FromDate).ToList();
                    //CODE REPLACED BY SHRADDHA ON 12 SEP 2017 REMOVED REDUNDANT AND UNNECESSARY IF ELSE CODE AND TAKEN ONE SINGLE LINE CODE END
                }
                #endregion

                // Updated by Rajas on 20 MAY 2017 END
                ViewBag.Status = Status;
                ViewBag.SelectedStatus = selectCriteria;
                // Added by Rajas on 9 MAY 2017 END

                var EmployeeData = WetosDB.VwActiveEmployees.ToList();

                // Added by Rajas on 9 MAY 2017
                // Populate Status dropdown
                //REPLACED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 START
                //List<string> StatusObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 3).Select(a => a.Text).ToList();
                //ViewBag.StatusList = StatusObj;
                PopulateDropDown();
                //REPLACED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 END

                return View(ManualCOSanctionList);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in  Manual CompOff Sanction due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Error in  Manual CompOff Sanction due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );


                //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 START
                PopulateDropDown();
                //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 END

                return RedirectToAction("EmployeeDashboard", "Dashboard");
            }
        }

        //ADDED BY PUSHKAR ON 21 APRIL 2018 FOR FAKE LOGIC FOR FILTERING COMP-OFF START
        #region Leave Sanction fake logic for filered leave Processing
        /// <summary>
        /// ADDED BY SHRADDHA ON 22 JULY 2017
        /// FUNCTION FOR FAKE LOGIC ADDED BY MSJ
        /// BECAUSE WITHOUT THIS CODE LEAVE SANCTION NOT WORKING INCASE OF FILTER
        /// <param name="EmpNo"></param>
        /// <param name="LeaveSanction"></param>
        /// <param name="FC"></param>
        /// <returns></returns>
        public static List<SP_CumulativeManualCOSanctionIndex_Result> CumComOffSanctionFakeLogic(WetosDBEntities WetosDB, int EmpNo, List<SP_CumulativeManualCOSanctionIndex_Result> ManualCOSanction, FormCollection FC)
        {

            ManualCOSanction = new List<SP_CumulativeManualCOSanctionIndex_Result>();

            int SingleApplicationID = 0;
            int SingleStatusID = 0;

            foreach (var key in FC.AllKeys)
            {
                string KeyStr = key.ToString();

                if (KeyStr.Contains("StatusId") == true)
                {
                    SingleStatusID = Convert.ToInt32(FC[KeyStr]);

                    string TempKeyStr = KeyStr.Replace("StatusId", "CMCompOffId");

                    SingleApplicationID = Convert.ToInt32(FC[TempKeyStr]);
                    SP_CumulativeManualCOSanctionIndex_Result TesmpLeaveSanction = WetosDB.SP_CumulativeManualCOSanctionIndex(EmpNo).Where(a => a.CMCompOffId == SingleApplicationID).FirstOrDefault(); //.ToList();

                    if (TesmpLeaveSanction != null)
                    {
                        TesmpLeaveSanction.StatusId = SingleStatusID;// UPDATE STATUS
                        ManualCOSanction.Add(TesmpLeaveSanction);
                    }

                }

            }

            return ManualCOSanction;
        }
        #endregion
        //ADDED BY PUSHKAR ON 21 APRIL 2018 FOR FAKE LOGIC FOR FILTERING COMP-OFF END

        /// <summary>
        /// CODE FOR MANUAL COMP OFF SANCTION
        /// ENTIRE CODE MODIFIED ON SHRADDHA ON 20 SEP 2017
        /// <param name="LeaveSanction"></param>
        /// <param name="FC"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CumulativeCOSanctionIndex(List<SP_CumulativeManualCOSanctionIndex_Result> ManualCOSanction, FormCollection FC)
        {
            //TRY BLOCK
            #region TRY BLOCK
            try
            {
                int EmpNo = Convert.ToInt32(Session["EmployeeNo"]);
                string RejectReasonText = FC["RejectReasonText"];

                //FAKE LOGIC USED FOR FILTERING
                #region FAKE LOGIC USED FOR FILTERING
                List<SP_CumulativeManualCOSanctionIndex_Result> ManualCOSanctionObjForCount = new List<SP_CumulativeManualCOSanctionIndex_Result>();

                //ADDED BY PUSHKAR ON 21 APRIL 2018 FOR FAKE LOGIC FOR FILTERING COMP-OFF
                ManualCOSanction = CumComOffSanctionFakeLogic(WetosDB, EmpNo, ManualCOSanction, FC);
                if (ManualCOSanction.Count == 0)
                {
                    Information("Please select atleast one entry for processing");
                    PopulateDropDown();
                    return View(ManualCOSanction);
                }

                ManualCOSanctionObjForCount = ManualCOSanction.Where(a => a.StatusId == 3 || a.StatusId == 5 || a.StatusId == 6).ToList();

                //IF REJECTED BY APPROVER / REJECTED BY SANCTIONER / CANCELLED MANUAL COMP OFF AVAILABLE AND REJECT / CANCEL REASON IS NOT AVAILABLE THEN PROVIDE ERROR MESSAGE
                #region IF REJECTED BY APPROVER / REJECTED BY SANCTIONER / CANCELLED MANUAL COMP OFF AVAILABLE AND REJECT / CANCEL REASON IS NOT AVAILABLE THEN PROVIDE ERROR MESSAGE
                if (ManualCOSanctionObjForCount.Count() > 0 && (RejectReasonText == null || RejectReasonText == ""))
                {
                    ModelState.AddModelError("CustomError", "Please Enter Reject/Cancel Reason");
                    PopulateDropDown();
                    Error("Please enter Reject/Cancel reason");

                    return View(ManualCOSanctionObjForCount); // ODTravelSanctionList
                }
                #endregion

                #endregion

                //REAL LOGIC
                #region REAL LOGIC
                foreach (SP_CumulativeManualCOSanctionIndex_Result item in ManualCOSanction)
                {
                    int EmpId = Convert.ToInt32(Session["EmployeeNo"]);

                    CumulativeManualCompOff CumulativeManualCOObj = WetosDB.CumulativeManualCompOffs.Where(a => a.CMCompOffId == item.CMCompOffId).FirstOrDefault();
                    //FIND EMPLOYEE IS VALID / ACTIVE OR NOT
                    VwActiveEmployee ValidEmpObj = WetosDB.VwActiveEmployees.Where(a => a.EmployeeId == CumulativeManualCOObj.EmployeeId).FirstOrDefault();

                    //IF EMPLOYEE IS VALID
                    #region IF EMPLOYEE IS VALID
                    if (ValidEmpObj != null)
                    {
                        //IF APPLICATION SANCTION STATUS ID IS VALID
                        #region IF APPLICATION SANCTION STATUS ID IS VALID
                        if (item.StatusId != 0)
                        {
                            //IF STATUS IS SANCTION OR (STATUS IS APPROVED BUT APPROVER AND SANCTIONER IS SAME PERSON
                            #region IF STATUS IS SANCTION OR (STATUS IS APPROVED BUT APPROVER AND SANCTIONER IS SAME PERSON
                            if ((item.StatusId == 2) || (item.StatusId == 4 && (ValidEmpObj.EmployeeReportingId == EmpId && ValidEmpObj.EmployeeReportingId2 == EmpId)))
                            {
                                //IF LOGIN EMPLOYEE IS APPROVER OR SANCTIONER
                                #region IF LOGIN EMPLOYEE IS LEAVE APPROVER OR SANCTIONER
                                #region IF LOGIN EMPLOYEE IS LEAVE APPROVER OR SANCTIONER
                                if (ValidEmpObj.EmployeeReportingId2 == EmpId || ValidEmpObj.EmployeeReportingId == EmpId)
                                {
                                    //UPDATE MANUAL COMP OFF DATA INTO MANUAL COMP OFF TABLE IN CASE OF SANCTIONING MANUAL COMP OFF
                                    #region UPDATE MANUAL COMP OFF DATA INTO MANUAL COMP OFF TABLE IN CASE OF SANCTIONING MANUAL COMP OFF
                                    item.StatusId = 2;
                                    CumulativeManualCOObj.StatusId = item.StatusId;
                                    CumulativeManualCOObj.RejectReason = string.Empty;
                                    WetosDB.SaveChanges();
                                    #endregion

                                    //CODE TO ADD MANUAL COMP OFF ENTRY INTO COMP OFF TABLE IN CASE OF SANCTIONING MANUAL COMP OFF
                                    #region CODE TO ADD MANUAL COMP OFF ENTRY INTO COMP OFF TABLE IN CASE OF SANCTIONING MANUAL COMP OFF
                                    CumulativeCompOff NewCompOffObj = new CumulativeCompOff();
                                    DailyTransaction DailyTransactionObj = WetosDB.DailyTransactions.Where(a => a.TranDate == CumulativeManualCOObj.FromDate && a.EmployeeId == CumulativeManualCOObj.EmployeeId).FirstOrDefault();

                                    //IF ATTENDANCE DATA IS AVAILABLE FOR COMP OFF DATE THEN MARK SHIFT FROM DT TABLE
                                    #region IF ATTENDANCE DATA IS AVAILABLE FOR COMP OFF DATE THEN MARK SHIFT FROM DT TABLE
                                    if (DailyTransactionObj != null)
                                    {
                                        NewCompOffObj.ShiftId = DailyTransactionObj.ShiftId;
                                    }
                                    #endregion

                                    //IF ATTENDANCE DATA IS NOT AVAILABLE FOR COMP OFF DATE THEN MARK GEN SHIFT
                                    #region IF ATTENDANCE DATA IS NOT AVAILABLE FOR COMP OFF DATE THEN MARK GEN SHIFT
                                    else
                                    {
                                        NewCompOffObj.ShiftId = "GEN";  // Verify??
                                    }
                                    #endregion

                                    NewCompOffObj.EmployeeId = Convert.ToInt32(CumulativeManualCOObj.EmployeeId);
                                    NewCompOffObj.WoDate = CumulativeManualCOObj.FromDate;
                                    //IF COMP OFF BALANCE IS 0.5 THEN ADD DAYSTATUS IN COMP OFF TABLE IS 'Half Day'
                                    #region IF COMP OFF BALANCE IS 0.5 THEN ADD DAYSTATUS IN COMP OFF TABLE IS 'Half Day'
                                    if (CumulativeManualCOObj.CompOffBalance == 0.5)
                                    {
                                        NewCompOffObj.DayStatus = "Half Day";   // Mapp with Id ?
                                        //NewCompOffObj.CoHours = ManualCOObj.ExtraWorkingHrs;
                                        NewCompOffObj.CoHours = CumulativeManualCOObj.ExtraWorkingHrs == null ? 0 :
                                            ((CumulativeManualCOObj.ExtraWorkingHrs.Hour * 60) + CumulativeManualCOObj.ExtraWorkingHrs.Minute);// COMMNETED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 20 MAR 2018 
                                    }
                                    #endregion

                                    //IF COMP OFF BALANCE IS 1 THEN ADD DAYSTATUS IN COMP OFF TABLE IS 'Full Day'
                                    #region IF COMP OFF BALANCE IS 1 THEN ADD DAYSTATUS IN COMP OFF TABLE IS 'Full Day'
                                    else if (CumulativeManualCOObj.CompOffBalance == 1)
                                    {
                                        NewCompOffObj.DayStatus = "Full Day";
                                        //NewCompOffObj.CoHours = ManualCOObj.ExtraWorkingHrs;
                                        NewCompOffObj.CoHours = CumulativeManualCOObj.ExtraWorkingHrs == null ? 0 :
                                            ((CumulativeManualCOObj.ExtraWorkingHrs.Hour * 60) + CumulativeManualCOObj.ExtraWorkingHrs.Minute);// COMMNETED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 20 MAR 2018 
                                    }

                                    //CODE ADDED BY SHRADDHA ON 20 FEB 2018 START
                                    else
                                    {
                                        NewCompOffObj.CoHours = CumulativeManualCOObj.CompOffHours;
                                    }
                                    //CODE ADDED BY SHRADDHA ON 20 FEB 2018 END
                                    #endregion

                                    NewCompOffObj.BalanceCoHours = NewCompOffObj.CoHours; //CODE ADDED BY SHRADDHA ON 21 FEB 2018

                                    NewCompOffObj.CompanyId = CumulativeManualCOObj.CompanyId;
                                    NewCompOffObj.BranchId = CumulativeManualCOObj.BranchId;


                                    // Updated by Rajas on 29 SEP 2017 START
                                    //DateTime.Now modified
                                    int CurrentYearInt = DateTime.Now.Year; // ADDED BY MSJ ON 02 JAN 2018
                                    NewCompOffObj.LoginTime = CumulativeManualCOObj.LoginTime == null ? new DateTime(CurrentYearInt, 01, 01, 00, 00, 00) : CumulativeManualCOObj.LoginTime.Value;
                                    NewCompOffObj.LogOutTime = CumulativeManualCOObj.LogoutTime == null ? new DateTime(CurrentYearInt, 01, 01, 00, 00, 00) : CumulativeManualCOObj.LogoutTime.Value;
                                    // Updated by Rajas on 29 SEP 2017 END

                                    NewCompOffObj.ManualCompOffId = CumulativeManualCOObj.CMCompOffId;

                                    WetosDB.CumulativeCompOffs.AddObject(NewCompOffObj);
                                    WetosDB.SaveChanges();
                                    #endregion
                                }
                                #endregion

                                #endregion

                                //IF LOGIN EMPLOYEE IS NOT APPROVER OR SANCTIONER THEN PROVIDE ERROR MESSAGE
                                #region IF LOGIN EMPLOYEE IS NOT LEAVE APPROVER OR SANCTIONER THEN PROVIDE ERROR MESSAGE
                                else
                                {
                                    Information("You can not sanction this Manual CompOff as you are not sanctioner for selected employee");
                                    List<SP_CumulativeManualCOSanctionIndex_Result> ODTObj = WetosDB.SP_CumulativeManualCOSanctionIndex(EmpNo).OrderByDescending(a => a.FromDate).ToList();
                                    PopulateDropDown();

                                    return View(ODTObj);
                                }
                                #endregion
                            }
                            #endregion

                            //IF APPLICATION SANCTION IS CANCEL/ REJECTED BY APPROVER / REJECTED BY SANCTIONER
                            #region IF APPLICATION SANCTION IS CANCEL/ REJECTED BY APPROVER / REJECTED BY SANCTIONER
                            else if (item.StatusId == 5 || item.StatusId == 3 || item.StatusId == 6)   // Added by Rajas on 9 JULY 2017 to Fix Issue no. 2, defect id= FB0012 as per Test Cases sheet
                            {
                                //IF REJECT REASON IS EMPTY THEN PROVIDE ERROR MESSAGE
                                #region IF REJECT REASON IS EMPTY THEN PROVIDE ERROR MESSAGE
                                if (RejectReasonText == null || RejectReasonText == string.Empty)
                                {
                                    ModelState.AddModelError("CustomError", "Please Enter reject/cancellation Reason");
                                    Error("Please enter reject/cancellation reason");
                                    PopulateDropDown();
                                    return View(ManualCOSanction);
                                }
                                #endregion

                                //IF REJECT REASON IS NOT EMPTY 
                                #region IF REJECT REASON IS NOT EMPTY
                                else
                                {
                                    //CHECK WHETHER COMP OFF ENTRY IS AVAIALBLE IN COMP OFF TABLE OR NOT
                                    CumulativeCompOff CumulativeCompOffObj = WetosDB.CumulativeCompOffs.Where(a => a.ManualCompOffId == CumulativeManualCOObj.CMCompOffId).FirstOrDefault();
                                    //IF COMP OFF ENTRY IS AVAIALBLE IN COMP OFF TABLE
                                    #region IF COMP OFF ENTRY IS AVAIALBLE IN COMP OFF TABLE
                                    if (CumulativeCompOffObj != null)
                                    {
                                        //IF COMP OFF IS USED (PENDING/SANCTIONED) AGAINST SELECTED COMP OFF WHICH ENTRY WE ARE TRYING TO DELETE THEN PROVIDE ERROR MESSAGE
                                        #region IF COMP OFF IS USED (PENDING/SANCTIONED) AGAINST SELECTED COMP OFF WHICH ENTRY WE ARE TRYING TO DELETE THEN PROVIDE ERROR MESSAG
                                        if (CumulativeCompOffObj.AppStatus != null)
                                        {
                                            if (CumulativeCompOffObj.AppStatus.Trim() == "P" || CumulativeCompOffObj.AppStatus.Trim() == "S")
                                            {
                                                Error("You are not allowed to reject/Cancel comp off for date:" + CumulativeCompOffObj.WoDate
                                                    + " and Employee:" + ValidEmpObj.FirstName + " " + ValidEmpObj.LastName + "<br/>" + " Selected Comp off is already used.");
                                                PopulateDropDown();
                                                return View(ManualCOSanction);
                                            }
                                        }
                                        #endregion

                                        //IF COMP OFF IS NOT USED (PENDING/SANCTIONED) AGAINST SELECTED COMP OFF THEN WE CAN REJECT / CANCEL SUCH MANUAL COMP OFF
                                        #region IF COMP OFF IS NOT USED (PENDING/SANCTIONED) AGAINST SELECTED COMP OFF THEN WE CAN REJECT / CANCEL SUCH MANUAL COMP OFF
                                        else
                                        {
                                            //IF COMP OFF IS REJECTED BY APPROVER/ REJECTED BY SANCTIONER THEN UPDATE COMP OFF APPSTATUS IN COMP OFF TABLE - RM
                                            #region IF COMP OFF IS REJECTED BY APPROVER/ REJECTED BY SANCTIONER THEN UPDATE COMP OFF APPSTATUS IN COMP OFF TABLE - RM
                                            if (item.StatusId == 3 || item.StatusId == 6)
                                            {
                                                CumulativeCompOffObj.AppStatus = "RM";
                                                CumulativeCompOffObj.BalanceCoHours = 0;
                                            }
                                            #endregion

                                            //IF COMP OFF IS CANCELLED BY SANCTIONER THEN UPDATE COMP OFF APPSTATUS IN COMP OFF TABLE - CM
                                            #region IF COMP OFF IS CANCELLED BY SANCTIONER THEN UPDATE COMP OFF APPSTATUS IN COMP OFF TABLE - CM
                                            if (item.StatusId == 5)
                                            {
                                                CumulativeCompOffObj.AppStatus = "CM";
                                                CumulativeCompOffObj.BalanceCoHours = 0;
                                            }
                                            #endregion

                                            WetosDB.SaveChanges();

                                            //UPDATE MANUAL COMP OFF TABLE ENTRY IN CASE OF SUCCESSFULL REJECTION / CANCELLATION OF MANUAL COMP OFF
                                            #region UPDATE MANUAL COMP OFF TABLE ENTRY IN CASE OF SUCCESSFULL REJECTION / CANCELLATION OF MANUAL COMP OFF
                                            CumulativeManualCOObj.RejectReason = RejectReasonText;
                                            CumulativeManualCOObj.StatusId = item.StatusId;
                                            WetosDB.SaveChanges();
                                            #endregion
                                        }
                                        #endregion
                                    }
                                    #endregion

                                    //IF COMP OFF ENTRY IS NOT AVAIALBLE IN COMP OFF TABLE
                                    #region IF COMP OFF ENTRY IS NOT AVAIALBLE IN COMP OFF TABLE
                                    else
                                    {
                                        //UPDATE MANUAL COMP OFF TABLE ENTRY IN CASE OF SUCCESSFULL REJECTION / CANCELLATION OF MANUAL COMP OFF
                                        #region UPDATE MANUAL COMP OFF TABLE ENTRY IN CASE OF SUCCESSFULL REJECTION / CANCELLATION OF MANUAL COMP OFF
                                        CumulativeManualCOObj.RejectReason = RejectReasonText;
                                        CumulativeManualCOObj.StatusId = item.StatusId;
                                        WetosDB.SaveChanges();
                                        #endregion
                                    }
                                    #endregion
                                }
                                #endregion
                            }
                            #endregion

                            //HANDLE MANUAL COMP OFF VALID CASE DEFAULT TRANSACTION
                            #region HANDLE MANUAL COMP OFF VALID CASE DEFAULT TRANSACTION
                            else
                            {
                                CumulativeManualCOObj.StatusId = item.StatusId;
                                if (CumulativeManualCOObj.StatusId == 2 || CumulativeManualCOObj.StatusId == 4)
                                {
                                    CumulativeManualCOObj.RejectReason = string.Empty;
                                }
                                else
                                {
                                    CumulativeManualCOObj.RejectReason = RejectReasonText;
                                }
                                WetosDB.SaveChanges();
                            }
                            #endregion
                        }
                        #endregion

                        WetosDB.SaveChanges();

                        //SEND NOTIFICATION
                        #region MANUAL COMP OFF SANCTION NOTIFICATION
                        //SEND NOTIFICATION TO EMPLOYEE WHOM MANUAL COMP OFF IS PROCESSED
                        #region SEND NOTIFICATION TO EMPLOYEE WHOM MANUAL COMP OFF IS PROCESSED
                        Notification NotificationObj = new Notification();
                        NotificationObj.FromID = ValidEmpObj.EmployeeReportingId;
                        NotificationObj.ToID = ValidEmpObj.EmployeeId;
                        NotificationObj.SendDate = DateTime.Now;
                        string StatusName = WetosDB.DropdownDatas.Where(a => a.TypeId == 3 && a.Value == item.StatusId).Select(a => a.Text).FirstOrDefault();
                        NotificationObj.NotificationContent = "Your Manual CO application from " + CumulativeManualCOObj.FromDate.ToString("dd-MMM-yyyy") + " is " + StatusName;
                        NotificationObj.ReadFlag = false;
                        NotificationObj.SendDate = DateTime.Now;

                        WetosDB.Notifications.AddObject(NotificationObj);
                        WetosDB.SaveChanges();
                        #endregion


                        //SEND NOTIFICATION TO SANCTIONER IF MANUAL COMP OFF IS APPROVED BY APPROVER IN CASE OF APPROVER AND SANCTIONER ARE BOTH DIFFERENT PERSON
                        #region SEND NOTIFICATION TO SANCTIONER IF MANUAL COMP OFF IS APPROVED BY APPROVER IN CASE OF APPROVER AND SANCTIONER ARE BOTH DIFFERENT PERSON
                        if (CumulativeManualCOObj.StatusId == 4 && ValidEmpObj.EmployeeReportingId == EmpNo)
                        {
                            Notification NotificationObj2 = new Notification();
                            NotificationObj.FromID = ValidEmpObj.EmployeeId;
                            NotificationObj.ToID = ValidEmpObj.EmployeeReportingId2;
                            NotificationObj.SendDate = DateTime.Now;
                            NotificationObj.NotificationContent = "Received Manual CO application for sanctioning from " + ValidEmpObj.FirstName + " " + ValidEmpObj.LastName + " - from " + CumulativeManualCOObj.FromDate.ToString("dd-MMM-yyyy");
                            NotificationObj.ReadFlag = false;
                            NotificationObj.SendDate = DateTime.Now;

                            WetosDB.Notifications.AddObject(NotificationObj2);
                            WetosDB.SaveChanges();

                        }
                        #endregion

                        int NoTiCount = WetosDB.Notifications.Where(a => a.ToID == EmpId && (a.ReadFlag == false || a.ReadFlag == null)).Count();
                        Session["NotificationCount"] = NoTiCount;

                        #endregion

                        //SEND EMAIL
                        #region SEND EMAIL
                        //GlobalSetting GlobalSettingObj2 = WetosDB.GlobalSettings.Where(a => a.SettingText == "Send Email").FirstOrDefault();

                        //ABOVE LINE COMMENTED AND BELOW LINE IS ADDED BY SHALAKA ON 13TH DEC 2017
                        GlobalSetting GlobalSettingObj2 = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.SendEmail).FirstOrDefault();

                        if (GlobalSettingObj2 != null)
                        {
                            if (GlobalSettingObj2.SettingValue == "1")
                            {

                                string EmailUpdateStatus = string.Empty;
                                if (ValidEmpObj.Email != null)
                                {
                                    if (SendEmail(ValidEmpObj.Email, NotificationObj.NotificationContent, NotificationObj.NotificationContent, ref EmailUpdateStatus, "Manual CompOff " + CumulativeManualCOObj.StatusId) == false)
                                    {
                                        Error(EmailUpdateStatus);
                                    }
                                }
                                else
                                {
                                    AddAuditTrail("Unable to send email, as Email Id not available for " + ValidEmpObj.EmployeeCode);
                                }
                            }
                            else
                            {
                                AddAuditTrail("Unable to send Email as email utility is disabled");
                            }
                        }

                        #endregion
                    }
                    #endregion

                    //IF EMPLOYEE IS NOT VALID
                    #region IF EMPLOYEE IS NOT VALID
                    else
                    {
                        Error("Inconsistent data, Please try again!!");
                        PopulateDropDown();
                        return View(ManualCOSanction);
                    }
                    #endregion
                }
                #endregion

                //RETURN TO VIEW WITH SUCCESS MESSAGE
                #region RETURN TO VIEW WITH SUCCESS MESSAGE
                List<SP_CumulativeManualCOSanctionIndex_Result> ManualCONewObj = WetosDB.SP_CumulativeManualCOSanctionIndex(EmpNo).OrderByDescending(a => a.FromDate).ToList();
                AddAuditTrail("Success - Manual CompOff application processed successfully");  // Updated by Rajas on 17 JAN 2017
                Success("Success - Manual CompOff application processed successfully");
                PopulateDropDown();
                return View(ManualCONewObj);
                #endregion
            }
            #endregion

            //CATCH BLOCK TO HANDLE RUNTIME EXCEPTION
            #region CATCH BLOCK TO HANDLE RUNTIME EXCEPTION
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in Manual Comp Off Sanction due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );
                Error("Please try again!!");
                PopulateDropDown();

                return View(ManualCOSanction);
            }
            #endregion
        }


        /// <summary>
        /// ADDED BY SHRADDHA ON 09 SEP 2017
        /// TO GET MANUAL CO SANCTION PV
        /// <param name="selectCriteria"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CumulativeCOSanctionPV(DateTime? FromDate, int? EmployeeIdEx, int selectCriteria = 0)
        {
            try
            {
                int EmpNo = Convert.ToInt32(Session["EmployeeNo"]);
                int Id = Convert.ToInt32(Session["Id"]);

                // Added by Rajas on 9 MAY 2017 START
                // Select Criteria as per dropdown value
                int Status = 0;

                // Updated by Rajas on 20 MAY 2017 START
                List<SP_CumulativeManualCOSanctionIndex_Result> ManualCOSanctionList = new List<SP_CumulativeManualCOSanctionIndex_Result>();

                #region UPDATED BY RAJAS ON 23 MAY 2017
                // Get current FY from global setting

                //GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == "Current Financial Year").FirstOrDefault();

                //ABOVE LINE COMMENTED AND BELOW LINE IS ADDED BY SHALAKA ON 13TH DEC 2017
                GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.CurrentFinancialYear).FirstOrDefault();

                // Error
                if (GlobalSettingObj == null)
                {
                    Error("Inconsistent Financial year data");

                    //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 START
                    PopulateDropDown();
                    //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 END

                    return View(ManualCOSanctionList);
                }

                DateTime CalanderStartDate = GetFYStartDate(); // DateTime CalanderStartDate = WetosDB.FinancialYears.Where(a => a.FinancialName == GlobalSettingObj.SettingValue).Select(a => a.StartDate).FirstOrDefault();

                if (CalanderStartDate != null)
                {
                    //CODE REPLACED BY SHRADDHA ON 12 SEP 2017 REMOVED REDUNDANT AND UNNECESSARY IF ELSE CODE AND TAKEN ONE SINGLE LINE CODE START
                    //Status = selectCriteria;
                    //ManualCOSanctionList = WetosDB.SP_ManualCOSanctionIndex(EmpNo).Where(a => ((a.EmployeeReportingId == EmpNo) && a.Id == Status && a.FromDate >= CalanderStartDate && (a.MarkedAsDelete == 0 || a.MarkedAsDelete == null)))
                    //        .OrderByDescending(a => a.FromDate).ToList();
                    //CODE REPLACED BY SHRADDHA ON 12 SEP 2017 REMOVED REDUNDANT AND UNNECESSARY IF ELSE CODE AND TAKEN ONE SINGLE LINE CODE END

                    // Above code commented. Error was list for reportingId2 was not displayed
                    // Code updated by Rajas on 22 SEP 2017 START
                    if (selectCriteria == 1)  // Pending
                    {
                        Status = 1;
                        //ADDED COMP OFF BALANCE CONDITION BY SHRADDHA ON 05 MAR 2018
                        // Updated by Rajas on 7 JUNE 2017, ( || a.EmployeeReportingId2 == EmpNo) removed
                        ManualCOSanctionList = WetosDB.SP_CumulativeManualCOSanctionIndex(EmpNo).Where(a => ((a.EmployeeReportingId == EmpNo) && a.Id == Status
                            && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null) && a.CompOffBalance == 0)
                            .OrderByDescending(a => a.FromDate).ToList();  // && a.Id == 4)
                    }
                    else if (selectCriteria == 2)  // Sanction
                    {
                        Status = 2;
                        //ADDED COMP OFF BALANCE CONDITION BY SHRADDHA ON 05 MAR 2018
                        ManualCOSanctionList = WetosDB.SP_CumulativeManualCOSanctionIndex(EmpNo)
                            .Where(a => ((a.EmployeeReportingId == EmpNo || a.EmployeeReportingId2 == EmpNo) && a.Id == Status && a.FromDate >= CalanderStartDate
                                && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null) && a.CompOffBalance == 0) 
                            .OrderByDescending(a => a.FromDate).ToList();  // && a.Id == 4)
                    }
                    else if (selectCriteria == 3) // Rejected by Approver
                    {
                        Status = 3;
                        //ADDED COMP OFF BALANCE CONDITION BY SHRADDHA ON 05 MAR 2018
                        ManualCOSanctionList = WetosDB.SP_CumulativeManualCOSanctionIndex(EmpNo)
                            .Where(a => ((a.EmployeeReportingId == EmpNo || a.EmployeeReportingId2 == EmpNo)
                                && a.Id == Status && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null) 
                                || (a.EmployeeReportingId2 == EmpNo
                                && a.Id == Status && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null) && a.CompOffBalance == 0) 
                                .OrderByDescending(a => a.FromDate).ToList();  // && a.Id == 4)
                    }
                    else if (selectCriteria == 4)  // Approve
                    {
                        Status = 4;

                        //ADDED COMP OFF BALANCE CONDITION BY SHRADDHA ON 05 MAR 2018
                        ManualCOSanctionList = WetosDB.SP_CumulativeManualCOSanctionIndex(EmpNo)
                            .Where(a => a.EmployeeReportingId2 == EmpNo
                                && a.Id == Status && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null && a.CompOffBalance == 0)
                                .OrderByDescending(a => a.FromDate).ToList();  // && a.Id == 4)
                    }
                    else if (selectCriteria == 5)  // Cancel
                    {
                        Status = 5;

                        //ADDED COMP OFF BALANCE CONDITION BY SHRADDHA ON 05 MAR 2018
                        ManualCOSanctionList = WetosDB.SP_CumulativeManualCOSanctionIndex(EmpNo)
                            .Where(a => ((a.EmployeeReportingId == EmpNo || a.EmployeeReportingId2 == EmpNo) && a.Id == Status
                                && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null) && a.CompOffBalance == 0) 
                                .OrderByDescending(a => a.FromDate).ToList();  // && a.Id == 4)
                    }
                    else  // Rejected by Sanctioner
                    {
                        Status = 6;

                        //ADDED COMP OFF BALANCE CONDITION BY SHRADDHA ON 05 MAR 2018
                        ManualCOSanctionList = WetosDB.SP_CumulativeManualCOSanctionIndex(EmpNo)
                            .Where(a => ((a.EmployeeReportingId2 == EmpNo) && a.Id == Status
                                && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null) && a.CompOffBalance == 0) 
                            .OrderByDescending(a => a.FromDate).ToList();  // && a.Id == 4)
                    }
                    // Code updated by Rajas on 22 SEP 2017 END


                }
                #endregion

                // Updated by Rajas on 20 MAY 2017 END
                ViewBag.Status = Status;
                ViewBag.SelectedStatus = selectCriteria;
                // Added by Rajas on 9 MAY 2017 END



                #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //var EmployeeData = WetosDB.VwActiveEmployees.ToList();
                List<SP_VwActiveEmployee_Result> EmployeeObj = WetosDB.SP_VwActiveEmployee(EmployeeId).ToList();
                #endregion
                PopulateDropDown();

                if (ManualCOSanctionList.Count > 0)
                {
                    if(FromDate != null)
                    {
                        ManualCOSanctionList = ManualCOSanctionList.Where(a => a.FromDate == FromDate).ToList();
                    }
                    if (EmployeeIdEx != null)
                    {
                        ManualCOSanctionList = ManualCOSanctionList.Where(a => a.EmployeeId == EmployeeIdEx).ToList();
                    }
                }

                return PartialView(ManualCOSanctionList);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in Cumulative CompOff Sanction due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Error in  Cumulative CompOff Sanction due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 START
                PopulateDropDown();
                //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 END

                return RedirectToAction("EmployeeDashboard", "Dashboard");
            }
        }


    }
}
