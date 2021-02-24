using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web.Mvc;
using WetosDB;
using WetosMVCMainApp.Models;

namespace WetosMVC.Controllers
{

    [SessionExpire]
    [Authorize]
    public class WetosApplicationController : BaseController
    {
        // GET: /WetosApplication/

        public ActionResult Index()
        {
            //ADDED BY RAJAS ON 27 DEC 2016
            AddAuditTrail("Success - Visited Applications"); // Updated by Rajas on 17 JAN 2017

            return View();
        }

        /// <summary>
        /// Added by Rajas on 18 MAY 2017
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static int GetWeekOfMonth(DateTime date)
        {
            DateTime beginningOfMonth = new DateTime(date.Year, date.Month, 1);

            while (date.Date.AddDays(1).DayOfWeek != CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek)
                date = date.AddDays(1);

            return (int)Math.Truncate((double)date.Subtract(beginningOfMonth).TotalDays / 7f) + 1;
        }


        public ActionResult AjaxPopulateDropdown(int EmployeeId)
        {
            var EmployeeNameList = (from r in WetosDB.Employees

                                    where r.EmployeeId == EmployeeId
                                    //join x in db.Unit_of_Measure on r.UOM_Code equals x.UOM_Code

                                    //EARLIER CODE COMMENTED AND ADDED NEW CODE BY SHRADDHA AS PER SUGGESTED BY MSJ ON TO HANDLE EMPTY MIDDLE NAME PROBLEM 30 OCT 2017 START
                                    //select new { EmployeeName = r.FirstName + " " + r.MiddleName + " " + r.LastName, EmployeeId = r.EmployeeId }).FirstOrDefault();
                                    select new { EmployeeName = r.FirstName + " " + r.LastName, EmployeeId = r.EmployeeId }).FirstOrDefault();
            //EARLIER CODE COMMENTED AND ADDED NEW CODE BY SHRADDHA AS PER SUGGESTED BY MSJ ON TO HANDLE EMPTY MIDDLE NAME PROBLEM 30 OCT 2017 END
            return Json(EmployeeNameList, JsonRequestBehavior.AllowGet);
        }


        //Modified By shraddha on 17th OCT 2016 for taking model instead of table
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MySelf"></param>
        /// <returns></returns>
        /// Updated by Rajas on 22 AUGUST 2017
        public ActionResult ODTravelApplication(bool MySelf)
        {
            // Try catch block added by Rajas on 14 FEB 2017
            try
            {
                ODTravelModel ODTourObj = new ODTravelModel();

                ODTourObj.MySelf = MySelf;
                ODTourObj.EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);   // Added by Rajas on 27 SEP 2017

                PopulateDropDown();

                // Assigning initial pending status
                ODTourObj.StatusId = 1;

                return View(ODTourObj);
            }

            catch (System.Exception ex)
            {
                PopulateDropDown();

                AddAuditTrail("Exception - ODtravel application failed due to " + ex.Message);

                Error("Error - ODTravel application failed");

                return View();
            }
        }


        /// <summary>
        /// ADDED BY MOUSAMI ON 14 SEPT 2016 
        /// </summary>
        /// <param name="ODTourObj"></param>
        /// <param name="Collection"></param>
        /// <returns></returns>

        //Modified By shraddha on 17th OCT 2016 for taking model instead of table
        [HttpPost]
        public ActionResult ODTravelApplication(ODTravelModel ODTourObj, FormCollection Collection)
        {
            try
            {
                int EmpId = ODTourObj.EmployeeId;

                //CODE ADDED BY SHRADDHA ON 30 JAN 2018 START
                if (string.IsNullOrEmpty(ODTourObj.ODTourType))
                {
                    ModelState.AddModelError("", "Please select Proper requisition type");

                    PopulateDropDown();

                    return View(ODTourObj);
                }
                else
                {
                    //ADDED BY PUSHKAR ON 31 AUGUST 2018
                    if (ODTourObj.ODTourType == "Half/Full Day OD" || ODTourObj.ODTourType == "Late Reporting - OD" || ODTourObj.ODTourType == "Early Departure - OD")
                    {
                        ODTourObj.FromDate = ODTourObj.ODDate;
                        ODTourObj.ToDate = ODTourObj.ODDate;
                        ODTourObj.ODDayStatus1 = ODTourObj.ODDayStatus2;
                        ODTourObj.ODDayStatus = ODTourObj.ODDayStatus2;
                        if (ODTourObj.ODDate == null)
                        {
                            ModelState.AddModelError("", "Please enter proper OD Date");

                            PopulateDropDown();

                            return View(ODTourObj);
                        }
                        //if (ODTourObj.ODLoginTime == null)
                        //{
                        //    ModelState.AddModelError("", "Please enter proper Login Time");

                        //    PopulateDropDown();

                        //    return View(ODTourObj);
                        //}

                        //if (ODTourObj.ODLogOutTime == null)
                        //{
                        //    ModelState.AddModelError("", "Please enter proper Logout Time");

                        //    PopulateDropDown();

                        //    return View(ODTourObj);
                        //}
                        ////CODE ADDED BY SHRADDHA ON 29 JAN 2018 START
                        //DateTime NextDay = ODTourObj.FromDate.Value.AddDays(1);
                        //if (ODTourObj.IsInPunchInNextDay == true)
                        //{
                        //    ODTourObj.ODLoginTime = new DateTime(NextDay.Year, NextDay.Month, NextDay.Day, ODTourObj.ODLoginTime.Value.Hour, ODTourObj.ODLoginTime.Value.Minute, ODTourObj.ODLoginTime.Value.Second);
                        //}
                        //else
                        //{
                        //    ODTourObj.ODLoginTime = new DateTime(ODTourObj.FromDate.Value.Year, ODTourObj.FromDate.Value.Month, ODTourObj.FromDate.Value.Day, ODTourObj.ODLoginTime.Value.Hour, ODTourObj.ODLoginTime.Value.Minute, ODTourObj.ODLoginTime.Value.Second);
                        //}
                        //if (ODTourObj.IsOutPunchInNextDay == true)
                        //{
                        //    ODTourObj.ODLogOutTime = new DateTime(NextDay.Year, NextDay.Month, NextDay.Day, ODTourObj.ODLogOutTime.Value.Hour, ODTourObj.ODLogOutTime.Value.Minute, ODTourObj.ODLogOutTime.Value.Second);
                        //}
                        //else
                        //{
                        //    ODTourObj.ODLogOutTime = new DateTime(ODTourObj.FromDate.Value.Year, ODTourObj.FromDate.Value.Month, ODTourObj.FromDate.Value.Day, ODTourObj.ODLogOutTime.Value.Hour, ODTourObj.ODLogOutTime.Value.Minute, ODTourObj.ODLogOutTime.Value.Second);
                        //}
                    }
                }
                //CODE ADDED BY SHRADDHA ON 30 JAN 2018 END


                //Added By Shraddha on 12 DEC 2016 for adding date validation start
                if (ODTourObj.FromDate > ODTourObj.ToDate)
                {
                    ModelState.AddModelError("", "End Date should be Greater than or equal to From Date");

                    PopulateDropDown();

                    return View(ODTourObj);

                }
                //Added By Shraddha on 12 DEC 2016 for adding date validation End

                // Updated by Rajas on 15 JULY 2017
                if (ODTourObj.FromDate == ODTourObj.ToDate)
                {
                    if (ODTourObj.ODDayStatus != ODTourObj.ODDayStatus1)
                    {
                        // ERROR
                        ModelState.AddModelError("", "Select Proper day status");

                        // FILL VIEW BAG AGAIN
                        PopulateDropDown();

                        return View(ODTourObj);
                    }
                }
                else
                {
                    //if (ODTourObj.ODDayStatus == "1") // NEED DISCUSSION
                    //{
                    //    // ERROR
                    //    ModelState.AddModelError("", "Select Proper status for from date");
                    //    PopulateDropDown();
                    //    LeaveApplicationNecessaryContent(EmpId);
                    //    return View(ODTourObj);
                    //}

                }

                /// Validation for Punch relaxed in case of OD 
                /// Updated by Rajas on 17 MAY 2017
                /// As per meeting discussion on 17 MAY 2017 with Katre sir, Kulkarni sir, Kirti madam, Madhav sir and Rajas
                #region PUNCH VALIDATION
                // Check if Punch is available for selected employee
                // Added by Rajas on 10 MAY 2017 START
                //DailyTransaction DailyTransactionObj = WetosDB.DailyTransactions.Where(a => a.TranDate >= LeaveTypeObj.FromDate && a.TranDate <= LeaveTypeObj.ToDate
                //    && a.EmployeeId == EmpId && (a.Status == "AAAA" || a.Status == "AAPP" || a.Status == "PPAA")).FirstOrDefault();

                // Updated by Rajas on 17 MAY 2017
                //DailyTransaction DailyTransactionObj = WetosDB.DailyTransactions.Where(a => a.TranDate >= ODTourObj.FromDate && a.TranDate <= ODTourObj.ToDate
                //   && a.EmployeeId == EmpId).FirstOrDefault();

                //if (DailyTransactionObj != null)
                //{
                //    // Added by Rajas on 18 MAY 2017 
                //    TimeSpan TransTime = DailyTransactionObj.TranDate.TimeOfDay;

                //    TimeSpan LoginTime = DailyTransactionObj.Login.TimeOfDay;

                //    if (TransTime != LoginTime)
                //    {
                //        ModelState.AddModelError("", "You can't apply leave as your punch is already available for selected date range");

                //        // Added by Rajas on 30 MARCH 2017 START
                //        string UpdateStatus = string.Empty;

                //        if (LeaveApplicationNecessaryContent(EmpId, ref UpdateStatus) == false)
                //        {
                //            AddAuditTrail("Error in Leave application due to " + UpdateStatus);

                //            Error(UpdateStatus);

                //            return RedirectToAction("LeaveApplicationIndex");
                //        }

                //        PopulateDropDown();

                //        return View(ODTourObj);
                //    }
                //}
                // Added by Rajas on 10 MAY 2017 END
                #endregion


                #region VALIDATE OD DATA

                // Updated by Rajas on 27 SEP 2017 START
                // Fixed Issue No. 6 in Flagship Defect Sheet raised by Sandeep sir
                LeaveApplication LeaveApplicationObj = WetosDB.LeaveApplications.Where(a => a.EmployeeId == EmpId
                    && (a.StatusId == 1 || a.StatusId == 2 || a.StatusId == 4) && a.MarkedAsDelete == 0
                    && ((a.FromDate == ODTourObj.FromDate) // && a.ToDate > LeaveTypeObj.ToDate) 
                             || (a.FromDate < ODTourObj.FromDate && a.ToDate == ODTourObj.ToDate)
                             || (a.FromDate < ODTourObj.FromDate && a.ToDate > ODTourObj.ToDate)
                             || (a.FromDate > ODTourObj.FromDate && a.ToDate < ODTourObj.ToDate)
                             || (a.FromDate == ODTourObj.FromDate && a.ToDate == ODTourObj.ToDate)
                             || (a.FromDate > ODTourObj.FromDate && a.FromDate < ODTourObj.ToDate) // ADDED BY MSJ ON 11 JAN 2018
                             || (a.ToDate > ODTourObj.FromDate && a.ToDate < ODTourObj.ToDate) // ADDED BY MSJ ON 11 JAN 2018
                             || (a.ToDate == ODTourObj.FromDate))
                    ).FirstOrDefault();// FROM DATE AND TO DATE CONDITION ADDED BY SHRADDHA WITH HELP OF MSJ ON 01 FEB 2018

                // Added by Rajas on 10 MARCH 2017 for validation to check if already pending or sanctioned CO is applied for same date
                CompOffApplication CompoffApplicationObj = WetosDB.CompOffApplications.Where(a => a.EmployeeId == EmpId
                    && (a.StatusId == 1 || a.StatusId == 2 || a.StatusId == 4)
                    && a.MarkedAsDelete == 0
                    && (
                             (a.FromDate == ODTourObj.FromDate) // && a.ToDate > LeaveTypeObj.ToDate) 
                             || (a.FromDate < ODTourObj.FromDate && a.ToDate == ODTourObj.ToDate)
                             || (a.FromDate < ODTourObj.FromDate && a.ToDate > ODTourObj.ToDate)
                             || (a.FromDate > ODTourObj.FromDate && a.ToDate < ODTourObj.ToDate)
                             || (a.FromDate == ODTourObj.FromDate && a.ToDate == ODTourObj.ToDate)
                             || (a.FromDate > ODTourObj.FromDate && a.FromDate < ODTourObj.ToDate) // ADDED BY MSJ ON 11 JAN 2018
                             || (a.ToDate > ODTourObj.FromDate && a.ToDate < ODTourObj.ToDate) // ADDED BY MSJ ON 11 JAN 2018
                             || (a.ToDate == ODTourObj.FromDate))  // ADDED BY MSJ ON 11 JAN 2018
                    ).FirstOrDefault();// FROM DATE AND TO DATE CONDITION ADDED BY SHRADDHA WITH HELP OF MSJ ON 01 FEB 2018

                // MODIFIED BY MSJ ON 09 JAN 2018 START COMMENTED EXISTING FUNCTION AND ADDED NEW FUNCTION
                //ODTour ODTourTblObj = WetosDB.ODTours.Where(a => a.EmployeeId == EmpId && (a.StatusId == 1 || a.StatusId == 2 || a.StatusId == 4)
                //                && a.MarkedAsDelete == 0
                //                && a.FromDate <= ODTourObj.FromDate && a.ToDate >= ODTourObj.ToDate).FirstOrDefault();

                ODTour ODTourTblObj = WetosDB.ODTours.Where(a => a.EmployeeId == EmpId
                             && (a.StatusId == 1 || a.StatusId == 2 || a.StatusId == 4)
                             && a.MarkedAsDelete == 0
                             && (
                             (a.FromDate == ODTourObj.FromDate) // && a.ToDate > LeaveTypeObj.ToDate) 
                             || (a.FromDate < ODTourObj.FromDate && a.ToDate == ODTourObj.ToDate)
                             || (a.FromDate < ODTourObj.FromDate && a.ToDate > ODTourObj.ToDate)
                             || (a.FromDate > ODTourObj.FromDate && a.ToDate < ODTourObj.ToDate)
                             || (a.FromDate == ODTourObj.FromDate && a.ToDate == ODTourObj.ToDate)
                             || (a.FromDate > ODTourObj.FromDate && a.FromDate < ODTourObj.ToDate) // ADDED BY MSJ ON 11 JAN 2018
                             || (a.ToDate > ODTourObj.FromDate && a.ToDate < ODTourObj.ToDate) // ADDED BY MSJ ON 11 JAN 2018
                             || (a.ToDate == ODTourObj.FromDate)  // ADDED BY MSJ ON 11 JAN 2018
                             )).FirstOrDefault();

                // MODIFIED BY MSJ ON 09 JAN 2018 END

                // Updated by Rajas on 27 SEP 2017 END

                for (DateTime CurrentDate = Convert.ToDateTime(ODTourObj.FromDate); CurrentDate.Date <= ODTourObj.ToDate; CurrentDate = CurrentDate.AddDays(1))
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
                                    ModelState.AddModelError("", "You Can not Apply OD For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                    PopulateDropDown();

                                    return View(ODTourObj);
                                }
                                else
                                {
                                    if (LeaveApplicationObj.FromDayStatus == ODTourObj.ODDayStatus)
                                    {
                                        ModelState.AddModelError("", "You Can not Apply OD For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                        PopulateDropDown();

                                        return View(ODTourObj);
                                    }
                                    else
                                    {
                                        if (ODTourObj.ODDayStatus == 1)
                                        {
                                            ModelState.AddModelError("", "You Can not Apply OD For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                            PopulateDropDown();

                                            return View(ODTourObj);
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
                                    ModelState.AddModelError("", "You Can not Apply OD For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                    PopulateDropDown();

                                    return View(ODTourObj);
                                }
                                else
                                {
                                    if (LeaveApplicationObj.FromDayStatus == ODTourObj.ODDayStatus)
                                    {
                                        ModelState.AddModelError("", "You Can not Apply OD For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                        PopulateDropDown();

                                        return View(ODTourObj);
                                    }
                                    else
                                    {
                                        if (ODTourObj.ODDayStatus == 1)
                                        {
                                            ModelState.AddModelError("", "You Can not Apply OD For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                            PopulateDropDown();

                                            return View(ODTourObj);
                                        }
                                    }
                                }
                            }
                            else if (CurrentDate == LeaveApplicationObj.ToDate)
                            {
                                if (LeaveApplicationObj.ToDayStatus == 1)  // Full day in single day
                                {
                                    ModelState.AddModelError("", "You Can not Apply OD For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                    PopulateDropDown();

                                    return View(ODTourObj);
                                }
                                else
                                {
                                    if (LeaveApplicationObj.FromDayStatus == ODTourObj.ODDayStatus)
                                    {
                                        ModelState.AddModelError("", "You Can not Apply OD For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                        PopulateDropDown();

                                        return View(ODTourObj);
                                    }
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", "You Can not Apply OD For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                PopulateDropDown();

                                return View(ODTourObj);
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
                                    ModelState.AddModelError("", "You Can not Apply OD For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                    PopulateDropDown();

                                    return View(ODTourObj);
                                }
                                else
                                {
                                    if (CompoffApplicationObj.FromDateStatus == ODTourObj.ODDayStatus)
                                    {
                                        ModelState.AddModelError("", "You Can not Apply OD For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                        PopulateDropDown();

                                        return View(ODTourObj);
                                    }
                                    else
                                    {
                                        if (ODTourObj.ODDayStatus == 1)
                                        {
                                            ModelState.AddModelError("", "You Can not Apply OD For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                            PopulateDropDown();

                                            return View(ODTourObj);
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
                                    ModelState.AddModelError("", "You Can not Apply OD For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                    PopulateDropDown();

                                    return View(ODTourObj);
                                }
                                else
                                {
                                    if (CompoffApplicationObj.FromDateStatus == ODTourObj.ODDayStatus)
                                    {
                                        ModelState.AddModelError("", "You Can not Apply OD For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                        PopulateDropDown();

                                        return View(ODTourObj);
                                    }
                                    else
                                    {
                                        if (ODTourObj.ODDayStatus == 1)
                                        {
                                            ModelState.AddModelError("", "You Can not Apply OD For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                            PopulateDropDown();

                                            return View(ODTourObj);
                                        }
                                    }
                                }
                            }
                            else if (CurrentDate == CompoffApplicationObj.ToDate)
                            {
                                if (CompoffApplicationObj.ToDateStatus == 1)  // Full day in single day
                                {
                                    ModelState.AddModelError("", "You Can not Apply OD For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                    PopulateDropDown();

                                    return View(ODTourObj);
                                }
                                else
                                {
                                    if (CompoffApplicationObj.FromDateStatus == ODTourObj.ODDayStatus)
                                    {
                                        ModelState.AddModelError("", "You Can not Apply OD For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                        PopulateDropDown();

                                        return View(ODTourObj);
                                    }
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", "You Can not Apply OD For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                PopulateDropDown();

                                return View(ODTourObj);
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
                        //COMMENTED BY SHRADDHA AND ADDED NEW LINE BY SHRADDHA ON 28 OCT 2017 START
                        //if (CompoffApplicationObj.FromDate == CompoffApplicationObj.ToDate) // Single day
                        if (ODTourTblObj.FromDate == ODTourTblObj.ToDate)
                        //COMMENTED BY SHRADDHA AND ADDED NEW LINE BY SHRADDHA ON 28 OCT 2017 END
                        {
                            if (CurrentDate == ODTourTblObj.FromDate)
                            {
                                //COMMENTED BY SHRADDHA AND ADDED NEW LINE BY SHRADDHA ON 28 OCT 2017 START
                                // if (CompoffApplicationObj.FromDateStatus == 1)  // Full day in single day
                                if (ODTourTblObj.ODDayStatus == 1)
                                //COMMENTED BY SHRADDHA AND ADDED NEW LINE BY SHRADDHA ON 28 OCT 2017 END
                                {
                                    ModelState.AddModelError("", "You Can not Apply OD For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned OD for this date range.");

                                    PopulateDropDown();

                                    return View(ODTourObj);
                                }
                                else
                                {
                                    if (ODTourTblObj.ODDayStatus == ODTourObj.ODDayStatus)
                                    {
                                        ModelState.AddModelError("", "You Can not Apply OD For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned OD for this date range.");

                                        PopulateDropDown();

                                        return View(ODTourObj);
                                    }
                                    else
                                    {
                                        if (ODTourObj.ODDayStatus == 1)
                                        {
                                            ModelState.AddModelError("", "You Can not Apply OD For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned OD for this date range.");

                                            PopulateDropDown();

                                            return View(ODTourObj);
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
                                    ModelState.AddModelError("", "You Can not Apply OD For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned OD for this date range.");

                                    PopulateDropDown();

                                    return View(ODTourObj);
                                }
                                else
                                {
                                    if (ODTourTblObj.ODDayStatus == ODTourObj.ODDayStatus)
                                    {
                                        ModelState.AddModelError("", "You Can not Apply OD For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned OD for this date range.");

                                        PopulateDropDown();

                                        return View(ODTourObj);
                                    }
                                    else
                                    {
                                        if (ODTourObj.ODDayStatus == 1)
                                        {
                                            ModelState.AddModelError("", "You Can not Apply OD For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned OD for this date range.");

                                            PopulateDropDown();

                                            return View(ODTourObj);
                                        }
                                    }
                                }
                            }
                            else if (CurrentDate == ODTourTblObj.ToDate)
                            {
                                if (ODTourTblObj.ODDayStatus == 1)  // Full day in single day
                                {
                                    ModelState.AddModelError("", "You Can not Apply OD For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned OD for this date range.");

                                    PopulateDropDown();

                                    return View(ODTourObj);
                                }
                                else
                                {
                                    if (ODTourTblObj.ODDayStatus == ODTourObj.ODDayStatus)
                                    {
                                        ModelState.AddModelError("", "You Can not Apply OD For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned OD for this date range.");

                                        PopulateDropDown();

                                        return View(ODTourObj);
                                    }
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", "You Can not Apply OD For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned OD for this date range.");

                                PopulateDropDown();

                                return View(ODTourObj);
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
                DailyTransaction DailyTransactionListObj = WetosDB.DailyTransactions.Where(a => a.TranDate >= ODTourObj.FromDate && a.TranDate <= ODTourObj.ToDate && a.EmployeeId == EmpId && a.Lock == "Y").FirstOrDefault();

                if (DailyTransactionListObj != null)
                {
                    ModelState.AddModelError("", "You Can not apply leave for this range as Data is Locked");

                    PopulateDropDown();

                    return View(ODTourObj);
                }
                // Added by Rajas on 10 MARCH 2017 to Validate if data is locked, can not apply leave END
                #endregion

                #endregion

                //if (ModelState.IsValid) //IF ELSE CONDITION COMMENTED BY SHRADDHA ON 30 JAN 2018
                {
                    //TODO : Add insert logic here

                    // Calculate applied days
                    // Updated by Rajas on 15 JULY 2017 START
                    TimeSpan ts = (Convert.ToDateTime(ODTourObj.ToDate)) - (Convert.ToDateTime(ODTourObj.FromDate));

                    float AppliedDay = ts.Days;
                    if (ODTourObj.ODDayStatus == 1 && ODTourObj.ODDayStatus1 == 1)
                    {
                        AppliedDay = AppliedDay + 1;
                    }

                        //Modified By Shraddha on 30 DEC 2016 for leave application validation changes start
                    else if ((ODTourObj.ODDayStatus == 2) && (ODTourObj.ODDayStatus1 == 2) && ODTourObj.ODDayStatus == ODTourObj.ODDayStatus1)
                    {
                        AppliedDay = 0.5F;
                    }
                    else if ((ODTourObj.ODDayStatus == 3) && (ODTourObj.ODDayStatus1 == 3) && ODTourObj.ODDayStatus == ODTourObj.ODDayStatus1)
                    {
                        AppliedDay = 0.5F;
                    }
                    else if ((ODTourObj.ODDayStatus == 2) && (ODTourObj.ODDayStatus1 == 3) && ODTourObj.ODDayStatus == ODTourObj.ODDayStatus1)
                    {
                        AppliedDay = 1F;
                    }

                    //Modified By Shraddha on 30 DEC 2016 for leave application validation changes End
                    else if ((ODTourObj.ODDayStatus == 2 || ODTourObj.ODDayStatus == 3) && (ODTourObj.ODDayStatus1 == 2 || ODTourObj.ODDayStatus1 == 3) && ODTourObj.ODDayStatus != ODTourObj.ODDayStatus1)
                    {
                        AppliedDay = AppliedDay + 0;
                    }

                    else if (ODTourObj.ODDayStatus == 2 || ODTourObj.ODDayStatus1 == 2 || ODTourObj.ODDayStatus == 3 || ODTourObj.ODDayStatus1 == 3)
                    {
                        AppliedDay = (float)(AppliedDay + 0.5);
                    }
                    // Updated by Rajas on 15 JULY 2017 END

                    ODTourObj.AppliedDay = AppliedDay;

                    ODTour ODTOURObj = new ODTour();

                    ODTOURObj.ActualDay = ODTourObj.ActualDay;

                    ODTOURObj.AppliedDay = ODTourObj.AppliedDay;

                    //CODE CHANGED BY SHRADDHA ON 13 FEB 2017 FOR TAKING COMPANYID, BRANCHID AND EMPLOYEEID OF LOGIN EMPLOYEE START
                    Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == EmpId).FirstOrDefault();

                    // Handle if EmployeeObj is NULL
                    // Updated by Rajas on 27 SEP 2017 START
                    ODTOURObj.BranchId = EmployeeObj == null ? 0 : EmployeeObj.BranchId;

                    ODTOURObj.CompanyId = EmployeeObj == null ? 0 : EmployeeObj.CompanyId;
                    // Updated by Rajas on 27 SEP 2017 END

                    ODTOURObj.EmployeeId = EmpId;

                    //CODE CHANGED BY SHRADDHA ON 13 FEB 2017 FOR TAKING COMPANYID, BRANCHID AND EMPLOYEEID OF LOGIN EMPLOYEE END

                    ODTOURObj.FromDate = Convert.ToDateTime(ODTourObj.FromDate);

                    ODTOURObj.JourneyType = ODTourObj.JourneyType;

                    ODTOURObj.ODDayStatus = ODTourObj.ODDayStatus;

                    ODTOURObj.ODDayStatus1 = ODTourObj.ODDayStatus1; // Added by Rajas on 10 JULY 2017

                    ODTOURObj.OdTourHeadCode = ODTourObj.OdTourHeadCode;

                    ODTOURObj.ODTourId = ODTourObj.ODTourId;

                    ODTOURObj.ODTourType = ODTourObj.ODTourType;

                    ODTOURObj.RejectReason = ODTourObj.RejectReason;

                    ODTOURObj.Place = ODTourObj.Place;

                    ODTOURObj.Purpose = ODTourObj.Purpose;

                    ODTOURObj.ToDate = Convert.ToDateTime(ODTourObj.ToDate);

                    ODTOURObj.StatusId = ODTourObj.StatusId;

                    ODTOURObj.TransportType = ODTourObj.TransportType;

                    //Added By Shraddha on 12 DEC 2016 for taking Status along with statusId Start
                    ODTOURObj.Status = WetosDB.DropdownDatas.Where(a => a.TypeId == 3 && a.Value == ODTourObj.StatusId).Select(a => a.Text).FirstOrDefault();
                    //Added By Shraddha on 12 DEC 2016 for taking Status along with statusId End

                    //CODE ADDED BY SHRADDHA ON 30 JAN 2018 START
                    //ODTOURObj.ODLoginTime = ODTourObj.ODLoginTime;
                    //ODTOURObj.ODLogOutTime = ODTourObj.ODLogOutTime;
                    //ODTOURObj.IsInPunchInNextDay = ODTourObj.IsInPunchInNextDay;
                    //ODTOURObj.IsOutPunchInNextDay = ODTourObj.IsOutPunchInNextDay;
                    //CODE ADDED BY SHRADDHA ON 30 JAN 2018 END

                    ODTOURObj.EffectiveDate = ODTourObj.EffectiveDate; // CODE ADDED BY SHRADDHA ON 17 MAR 2018

                    ODTOURObj.CustomerName = ODTourObj.CustomerName;
                    ODTOURObj.CSRNo = ODTourObj.CSRNo;
                    ODTOURObj.ClientName = ODTourObj.ClientName;
                    ODTOURObj.PurposeDescription = ODTourObj.PurposeDesc;


                    WetosDB.ODTours.AddObject(ODTOURObj);

                    WetosDB.SaveChanges();

                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 7 SEPTEMBER 2017
                    #region ADD AUDIT LOG
                    //OLD RECORD IS BLANK
                    string Newrecord = "ODTourType : " + ODTOURObj.ODTourType + ", FromDate : " + ODTOURObj.FromDate
                       + ", ToDate : " + ODTOURObj.ToDate + ", ODDayStatus : " + ODTOURObj.ODDayStatus
                       + ", ODDayStatus1 : " + ODTOURObj.ODDayStatus1 + ", AppliedDays :" + ODTOURObj.AppliedDay + ", ActualDays :"
                       + ODTOURObj.ActualDay + ", Purpose :" + ODTOURObj.Purpose + ", Status :" + ODTOURObj.Status
                       + ", BranchId :" + ODTOURObj.BranchId
                       + ", CompanyId :" + ODTOURObj.CompanyId;

                    //LOGIN ID IS TAKEN FROM SESSION PERSISTER
                    string Formname = "REQUISITION APPLICATION";
                    //ACTION IS UPDATE
                    string Message = " ";

                    WetosAdministrationController.GenerateAuditLogsInsert(WetosDB, ODTOURObj.EmployeeId, Formname, Newrecord, ref Message);
                    #endregion
                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 7 SEPTEMBER 2017


                    // NOTIFICATION ADDED BY RAJAS ON 31 DEC 2016
                    #region OD APPLICATION NOTIFICATION

                    // FOR NOTIFICATION TO BE SENT TO REPORTING PERSON

                    //CODE COMMENTED BY SHRADDHA ON 13 FEB 2017 BECAUSE SAME CODE EXISTS ABOVE START
                    //Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == ODTOURObj.EmployeeId).FirstOrDefault();
                    //CODE COMMENTED BY SHRADDHA ON 13 FEB 2017 BECAUSE SAME CODE EXISTS ABOVE END

                    Notification NotificationObj = new Notification();
                    NotificationObj.FromID = EmployeeObj.EmployeeId;
                    NotificationObj.ToID = EmployeeObj.EmployeeReportingId;
                    NotificationObj.SendDate = DateTime.Now;

                    //NOTIFICATION CHANGED BY SHRADDHA ON 13 FEB 2017 FROM OD TOUR TO OD TRAVEL START
                    //NotificationObj.NotificationContent = "OD Travel applied by " + EmployeeObj.FirstName + " " + EmployeeObj.LastName + " for " + ODTOURObj.FromDate.Value.ToString("dd-MMM-yyyy") + " to " + ODTOURObj.ToDate.Value.ToString("dd-MMM-yyyy") + " is pending for approval";
                    NotificationObj.NotificationContent = "Received OD/Travel application for approval from " + EmployeeObj.FirstName + " " + EmployeeObj.LastName;
                    //NOTIFICATION CHANGED BY SHRADDHA ON 13 FEB 2017 FROM OD TOUR TO OD TRAVEL END

                    NotificationObj.ReadFlag = false;
                    NotificationObj.SendDate = DateTime.Now;

                    WetosDB.Notifications.AddObject(NotificationObj);

                    WetosDB.SaveChanges();

                    // Check if both reporting person are are different
                    //if (EmployeeObj.EmployeeReportingId != EmployeeObj.EmployeeReportingId2)
                    //{
                    //    // FOR NOTIFICATION TO BE SENT TO REPORTING PERSON AS SANCTIONER

                    //    //Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == LeaveTypeObj.EmployeeId).FirstOrDefault();
                    //    Notification NotificationObj3 = new Notification();
                    //    NotificationObj3.FromID = EmployeeObj.EmployeeId;
                    //    NotificationObj3.ToID = EmployeeObj.EmployeeReportingId;
                    //    NotificationObj3.SendDate = DateTime.Now;
                    //    NotificationObj3.NotificationContent = "OD Travel applied by " + EmployeeObj.FirstName + " " + EmployeeObj.LastName + " for " + ODTOURObj.FromDate.Value.ToString("dd-MMM-yyyy") + " to " + ODTOURObj.ToDate.Value.ToString("dd-MMM-yyyy") + " is pending for approval";
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
                    ////NOTIFICATION CHANGED BY SHRADDHA ON 13 FEB 2017 FROM OD TOUR TO OD TRAVEL START
                    //NotificationObj2.NotificationContent = "OD Travel" + " for " + ODTOURObj.FromDate.Value.ToString("dd-MMM-yyyy") + " to " + ODTOURObj.ToDate.Value.ToString("dd-MMM-yyyy") + "is applied successfully";
                    ////NOTIFICATION CHANGED BY SHRADDHA ON 13 FEB 2017 FROM OD TOUR TO OD TRAVEL END
                    //NotificationObj2.ReadFlag = false;
                    //NotificationObj2.SendDate = DateTime.Now;

                    //WetosDB.Notifications.AddObject(NotificationObj2);

                    //WetosDB.SaveChanges();

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
                                if (SendEmail(EmployeeObj.Email, NotificationObj.NotificationContent, NotificationObj.NotificationContent, ref EmailUpdateStatus, "OD/Travel Application") == false)
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
                    AddAuditTrail("Success - New OD application : " + ODTourObj.ODTourType + " taken on " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " created successfully"); //Modified by Pushkar on 4 FEB 2017 as datetime in model was updated // Updated by Rajas on 17 JAN 2017

                    // Added by Rajas on 17 JAN 2017 START
                    Success("OD/Travel application for " + EmployeeObj.FirstName + " " + EmployeeObj.LastName + " successfully created");  //Modified by Pushkar on 4 FEB 2017 as datetime in model was updated

                    if (ODTourObj.MySelf == true)
                    {

                        return RedirectToAction("ODtravelIndex");
                    }
                    else
                    {
                        return RedirectToAction("ODtravelIndex", new { IsMySelf = "false" });
                    }
                }
                //else //IF ELSE CONDITION COMMENTED BY SHRADDHA ON 30 JAN 2018
                //{
                //    // ADDED BY PUSHKAR FOR AuditLog ON 21 DEC 2016
                //    AddAuditTrail("Error - New OD application failed");  // Updated by Rajas on 17 JAN 2017

                //    // Added by Rajas on 17 JAN 2017 START
                //    Error("Error - New OD application failed");

                //    PopulateDropDown();
                //    return View(ODTourObj);
                //}
            }

            catch (System.Exception ex)
            {
                // ADDED BY PUSHKAR FOR AuditLog ON 21 DEC 2016
                AddAuditTrail("Error - New OD application failed due to" + ex.Message);  // Updated by Rajas on 17 JAN 2017

                // Added by Rajas on 17 JAN 2017 START
                Error("Error - New OD application failed due to" + ex.Message);

                PopulateDropDown();
                return View(ODTourObj);
            }
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

            var RequisitionTYpeObjLE = WetosDB.DropdownDatas.Where(a => a.TypeId == 25).Select(a => new { ODTourId = a.Text, ODTourType = a.Text }).ToList();

            ViewBag.RequisitionTYpeListLE = new SelectList(RequisitionTYpeObjLE, "ODTourId", "ODTourType").ToList();

            //var EmployeeCodeObj = WetosDB.Employees.Select(a => new { EmployeeId = a.EmployeeId, EmployeeCode = a.EmployeeCode }).ToList();

            //ViewBag.EmployeeCodeList = new SelectList(EmployeeCodeObj, "EmployeeId", "EmployeeCode").ToList();

            // Added by Rajas on 22 OCT 2016 for Dropdown list for FULL DAY or HALF DAY Leave
            var LeaveTypeStatusObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 9).Select(a => new { LeaveStatusID = a.Value, LeaveStatus = a.Text }).ToList();
            ViewBag.LeaveTypeStatusList = new SelectList(LeaveTypeStatusObj, "LeaveStatusID", "LeaveStatus").ToList();

            var LeaveTypeStatusObjLE = WetosDB.DropdownDatas.Where(a => a.TypeId == 9 && a.Value > 1).Select(a => new { LeaveStatusID = a.Value, LeaveStatus = a.Text }).ToList();
            ViewBag.LeaveTypeStatusListLE = new SelectList(LeaveTypeStatusObjLE, "LeaveStatusID", "LeaveStatus").ToList();
            //Added by Shraddha on 17 DEC 2016 for displaying employee code and name both in dropdown
            //    var EmployeeCodeAndNameObj = WetosDB.VwActiveEmployees.Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + "   " + a.FirstName + " " + a.MiddleName + " " + a.LastName }).ToList();

            //Added by Shalaka on 26th OCT 2017

            #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST

            int BranchAdmin = Convert.ToInt32(Session["BranchAdmin"]);
            int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);

            var EmployeeCodeAndNameObj = WetosDB.SP_VwActiveEmployee(EmployeeId).Where(a => a.EmployeeReportingId == EmployeeId || a.EmployeeReportingId2 == EmployeeId)
                    .Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
            //var EmployeeCodeAndNameObj = WetosDB.VwActiveEmployees.Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
            if (BranchAdmin > 0)
            {
                EmployeeCodeAndNameObj = WetosDB.SP_VwActiveEmployee(EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
            }

            #endregion

            ViewBag.EmployeeCodeAndNameList = new SelectList(EmployeeCodeAndNameObj, "EmployeeId", "EmployeeName").ToList();

            //ADDED BY SHRADDHA ON 28 DEC 2016 FOR SELECT CRITERIA START
            List<string> SelectCriteriaObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 5).Select(a => a.Text).ToList();
            ViewBag.SelectCriteriaList = SelectCriteriaObj;
            //ADDED BY SHRADDHA ON 28 DEC 2016 FOR SELECT CRITERIA END

            // Added by Rajas on 19 MAY 2017
            var ODpurpose = WetosDB.DropdownDatas.Where(a => a.TypeId == 15).Select(a => new { ODPurposeId = a.Text, ODPurposeType = a.Text }).ToList();
            ViewBag.ODPurposeList = new SelectList(ODpurpose, "ODPurposeId", "ODPurposeType").ToList();


            var ODpurposeLE = WetosDB.DropdownDatas.Where(a => a.TypeId == 23).Select(a => new { ODPurposeId = a.Text, ODPurposeType = a.Text }).ToList();
            ViewBag.ODPurposeListLE = new SelectList(ODpurposeLE, "ODPurposeId", "ODPurposeType").ToList();

            //Added on 22nd DEC 2017 for getting only allowed encashment leave type-- Start
            var LeaveCodeObj = WetosDB.LeaveMasters.Where(a => a.EncashmentAllowedOrNot == true).Select(a => new { LeaveCode = a.LeaveCode, LeaveName = a.LeaveName }).Distinct().ToList();
            //Added on 22nd DEC 2017 for getting only allowed encashment leave type-- End

            ViewBag.LeaveCodeList = new SelectList(LeaveCodeObj, " LeaveCode", "LeaveName").ToList();
        }

        /// <summary>
        /// CompOff application edit 
        /// Added by Rajas on 21 NOV 2016
        /// MODIFIED CODE BY SHRADDHA ON 10 MARCH 2017 FOR CompOffApplicationEdit GET
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult CompOffApplicationEdit(int id)
        {
            try
            {
                WetosDB.CompOffApplication CompOffApplicationEdit = WetosDB.CompOffApplications.Where(a => a.CompOffId == id).FirstOrDefault();

                TimeSpan ts = (Convert.ToDateTime(CompOffApplicationEdit.ToDate)) - (Convert.ToDateTime(CompOffApplicationEdit.FromDate));

                float AppliedDay = ts.Days;

                if (CompOffApplicationEdit.FromDateStatus == 1 && CompOffApplicationEdit.ToDateStatus == 1)
                {
                    AppliedDay = AppliedDay + 1;
                } // Modified By Shraddha on 30 DEC 2016 for leave application validation changes start
                else if ((CompOffApplicationEdit.FromDateStatus == 2) && (CompOffApplicationEdit.ToDateStatus == 2) && CompOffApplicationEdit.FromDate == CompOffApplicationEdit.ToDate)
                {
                    AppliedDay = 0.5F;
                }
                else if ((CompOffApplicationEdit.FromDateStatus == 3) && (CompOffApplicationEdit.ToDateStatus == 3) && CompOffApplicationEdit.FromDate == CompOffApplicationEdit.ToDate)
                {
                    AppliedDay = 0.5F;
                }
                else if ((CompOffApplicationEdit.FromDateStatus == 2) && (CompOffApplicationEdit.ToDateStatus == 3) && CompOffApplicationEdit.FromDate == CompOffApplicationEdit.ToDate)
                {
                    AppliedDay = 1F;
                }

                else if ((CompOffApplicationEdit.FromDateStatus == 2 || CompOffApplicationEdit.FromDateStatus == 3) && (CompOffApplicationEdit.ToDateStatus == 2 || CompOffApplicationEdit.ToDateStatus == 3) && CompOffApplicationEdit.FromDate != CompOffApplicationEdit.ToDate)
                {
                    AppliedDay = AppliedDay + 0;
                }
                else if (CompOffApplicationEdit.FromDateStatus == 2 || CompOffApplicationEdit.ToDateStatus == 2 || CompOffApplicationEdit.FromDateStatus == 3 || CompOffApplicationEdit.ToDateStatus == 3)
                {
                    AppliedDay = (float)(AppliedDay + 0.5);
                }

                Session["AppliedDay"] = AppliedDay;

                PopulateDropDown();

                return View(CompOffApplicationEdit);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                return View("Error");
            }
        }



        /// <summary>
        /// MODIFIED CODE BY SHRADDHA ON 10 MARCH 2017 FOR CompOffApplicationEdit POST
        /// </summary>
        /// <param name="id"></param>
        /// <param name="CompOffApplicationEdit"></param>
        /// <param name="Collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CompOffApplicationEdit(int id, WetosDB.CompOffApplication CompOffApplicationEdit, FormCollection Collection)
        {
            try
            {
                List<CompOff> CompOffList = WetosDB.CompOffs.Where(a => a.CompOffApplicationID == CompOffApplicationEdit.CompOffId).ToList();

                int EmpId = Convert.ToInt32(Session["EmployeeNo"]);

                //Added By Shraddha on 12 DEC 2016 for adding date validation start
                if (CompOffApplicationEdit.FromDate > CompOffApplicationEdit.ToDate)
                {
                    ModelState.AddModelError("", "End Date should be Greater than or equal to From Date");
                }

                //Added By Shraddha on 12 DEC 2016 for adding date validation End

                //Modified By Shraddha on 12 DEC 2016 Added if(ModelState.IsValid) Condition instead of try catch block
                if (ModelState.IsValid)
                {
                    #region VALIDATE CO DATA

                    // Updated by Rajas on 27 SEP 2017 START
                    // Fixed Issue No. 6 in Flagship Defect Sheet raised by Sandeep sir
                    LeaveApplication LeaveApplicationObj = WetosDB.LeaveApplications.Where(a => a.EmployeeId == EmpId
                        && (a.StatusId == 1 || a.StatusId == 2 || a.StatusId == 4) && a.MarkedAsDelete == 0
                        //&& a.FromDate <= CompOffApplicationEdit.FromDate && a.ToDate >= CompOffApplicationEdit.ToDate
                        && ((a.FromDate == CompOffApplicationEdit.FromDate) // && a.ToDate > LeaveTypeObj.ToDate) 
                             || (a.FromDate < CompOffApplicationEdit.FromDate && a.ToDate == CompOffApplicationEdit.ToDate)
                             || (a.FromDate < CompOffApplicationEdit.FromDate && a.ToDate > CompOffApplicationEdit.ToDate)
                             || (a.FromDate > CompOffApplicationEdit.FromDate && a.ToDate < CompOffApplicationEdit.ToDate)
                             || (a.FromDate == CompOffApplicationEdit.FromDate && a.ToDate == CompOffApplicationEdit.ToDate)
                             || (a.FromDate > CompOffApplicationEdit.FromDate && a.FromDate < CompOffApplicationEdit.ToDate) // ADDED BY MSJ ON 11 JAN 2018
                             || (a.ToDate > CompOffApplicationEdit.FromDate && a.ToDate < CompOffApplicationEdit.ToDate) // ADDED BY MSJ ON 11 JAN 2018
                             || (a.ToDate == CompOffApplicationEdit.FromDate))
                        ).FirstOrDefault();

                    // Added by Rajas on 10 MARCH 2017 for validation to check if already pending or sanctioned CO is applied for same date
                    CompOffApplication CompoffApplicationObj = WetosDB.CompOffApplications.Where(a => a.EmployeeId == EmpId
                        && (a.StatusId == 1 || a.StatusId == 2 || a.StatusId == 4)
                        && a.MarkedAsDelete == 0
                        //&& a.FromDate <= CompOffApplicationEdit.FromDate && a.ToDate >= CompOffApplicationEdit.ToDate
                        && ((a.FromDate == CompOffApplicationEdit.FromDate) // && a.ToDate > LeaveTypeObj.ToDate) 
                             || (a.FromDate < CompOffApplicationEdit.FromDate && a.ToDate == CompOffApplicationEdit.ToDate)
                             || (a.FromDate < CompOffApplicationEdit.FromDate && a.ToDate > CompOffApplicationEdit.ToDate)
                             || (a.FromDate > CompOffApplicationEdit.FromDate && a.ToDate < CompOffApplicationEdit.ToDate)
                             || (a.FromDate == CompOffApplicationEdit.FromDate && a.ToDate == CompOffApplicationEdit.ToDate)
                             || (a.FromDate > CompOffApplicationEdit.FromDate && a.FromDate < CompOffApplicationEdit.ToDate) // ADDED BY MSJ ON 11 JAN 2018
                             || (a.ToDate > CompOffApplicationEdit.FromDate && a.ToDate < CompOffApplicationEdit.ToDate) // ADDED BY MSJ ON 11 JAN 2018
                             || (a.ToDate == CompOffApplicationEdit.FromDate))
                        && a.CompOffId != CompOffApplicationEdit.CompOffId).FirstOrDefault();

                    ODTour ODTourTblObj = WetosDB.ODTours.Where(a => a.EmployeeId == EmpId && (a.StatusId == 1 || a.StatusId == 2 || a.StatusId == 4)
                                    && a.MarkedAsDelete == 0
                        //&& a.FromDate <= CompOffApplicationEdit.FromDate && a.ToDate >= CompOffApplicationEdit.ToDate
                                    && ((a.FromDate == CompOffApplicationEdit.FromDate) // && a.ToDate > LeaveTypeObj.ToDate) 
                             || (a.FromDate < CompOffApplicationEdit.FromDate && a.ToDate == CompOffApplicationEdit.ToDate)
                             || (a.FromDate < CompOffApplicationEdit.FromDate && a.ToDate > CompOffApplicationEdit.ToDate)
                             || (a.FromDate > CompOffApplicationEdit.FromDate && a.ToDate < CompOffApplicationEdit.ToDate)
                             || (a.FromDate == CompOffApplicationEdit.FromDate && a.ToDate == CompOffApplicationEdit.ToDate)
                             || (a.FromDate > CompOffApplicationEdit.FromDate && a.FromDate < CompOffApplicationEdit.ToDate) // ADDED BY MSJ ON 11 JAN 2018
                             || (a.ToDate > CompOffApplicationEdit.FromDate && a.ToDate < CompOffApplicationEdit.ToDate) // ADDED BY MSJ ON 11 JAN 2018
                             || (a.ToDate == CompOffApplicationEdit.FromDate))
                                    ).FirstOrDefault();
                    // Updated by Rajas on 27 SEP 2017 END

                    for (DateTime CurrentDate = Convert.ToDateTime(CompOffApplicationEdit.FromDate); CurrentDate.Date <= CompOffApplicationEdit.ToDate; CurrentDate = CurrentDate.AddDays(1))
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
                                        Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                        PopulateDropDown();
                                        CompOffApplicationNecessaryContent(EmpId);
                                        return View(CompOffApplicationEdit);
                                    }
                                    else
                                    {
                                        if (LeaveApplicationObj.FromDayStatus == CompOffApplicationEdit.FromDateStatus)
                                        {
                                            Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                            PopulateDropDown();
                                            CompOffApplicationNecessaryContent(EmpId);
                                            return View(CompOffApplicationEdit);
                                        }
                                        else
                                        {
                                            if (CompOffApplicationEdit.FromDateStatus == 1)
                                            {
                                                Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                                PopulateDropDown();
                                                CompOffApplicationNecessaryContent(EmpId);
                                                return View(CompOffApplicationEdit);
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
                                        Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                        PopulateDropDown();
                                        CompOffApplicationNecessaryContent(EmpId);
                                        return View(CompOffApplicationEdit);
                                    }
                                    else
                                    {
                                        if (LeaveApplicationObj.FromDayStatus == CompOffApplicationEdit.FromDateStatus)
                                        {
                                            Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                            PopulateDropDown();
                                            CompOffApplicationNecessaryContent(EmpId);
                                            return View(CompOffApplicationEdit);
                                        }
                                        else
                                        {
                                            if (CompOffApplicationEdit.FromDateStatus == 1)
                                            {
                                                Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                                PopulateDropDown();
                                                CompOffApplicationNecessaryContent(EmpId);
                                                return View(CompOffApplicationEdit);
                                            }
                                        }
                                    }
                                }
                                else if (CurrentDate == LeaveApplicationObj.ToDate)
                                {
                                    if (LeaveApplicationObj.ToDayStatus == 1)  // Full day in single day
                                    {
                                        Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                        PopulateDropDown();
                                        CompOffApplicationNecessaryContent(EmpId);
                                        return View(CompOffApplicationEdit);
                                    }
                                    else
                                    {
                                        if (LeaveApplicationObj.FromDayStatus == CompOffApplicationEdit.FromDateStatus)
                                        {
                                            Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                                + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                            PopulateDropDown();
                                            CompOffApplicationNecessaryContent(EmpId);
                                            return View(CompOffApplicationEdit);
                                        }
                                    }
                                }
                                else
                                {
                                    Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                        + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                    PopulateDropDown();
                                    CompOffApplicationNecessaryContent(EmpId);
                                    return View(CompOffApplicationEdit);
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
                                        Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                            + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range"
                                            + "<br/>" + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                        PopulateDropDown();
                                        CompOffApplicationNecessaryContent(EmpId);
                                        return View(CompOffApplicationEdit);
                                    }
                                    else
                                    {
                                        if (CompoffApplicationObj.FromDateStatus == CompOffApplicationEdit.FromDateStatus)
                                        {
                                            Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                                + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                                + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                            PopulateDropDown();
                                            CompOffApplicationNecessaryContent(EmpId);
                                            return View(CompOffApplicationEdit);
                                        }
                                        else
                                        {
                                            if (CompOffApplicationEdit.FromDateStatus == 1)
                                            {
                                                Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                                    + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range"
                                                    + "<br/>" + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                                PopulateDropDown();
                                                CompOffApplicationNecessaryContent(EmpId);
                                                return View(CompOffApplicationEdit);
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
                                        Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                            + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                            + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                        PopulateDropDown();
                                        CompOffApplicationNecessaryContent(EmpId);
                                        return View(CompOffApplicationEdit);
                                    }
                                    else
                                    {
                                        if (CompoffApplicationObj.FromDateStatus == CompOffApplicationEdit.FromDateStatus)
                                        {
                                            Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                                + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                                + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                            PopulateDropDown();
                                            CompOffApplicationNecessaryContent(EmpId);
                                            return View(CompOffApplicationEdit);
                                        }
                                        else
                                        {
                                            if (CompOffApplicationEdit.FromDateStatus == 1)
                                            {
                                                Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                                    + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range"
                                                    + "<br/>" + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                                PopulateDropDown();
                                                CompOffApplicationNecessaryContent(EmpId);
                                                return View(CompOffApplicationEdit);
                                            }
                                        }
                                    }
                                }
                                else if (CurrentDate == CompoffApplicationObj.ToDate)
                                {
                                    if (CompoffApplicationObj.ToDateStatus == 1)  // Full day in single day
                                    {
                                        Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                            + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                            + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                        PopulateDropDown();
                                        CompOffApplicationNecessaryContent(EmpId);
                                        return View(CompOffApplicationEdit);
                                    }
                                    else
                                    {
                                        if (CompoffApplicationObj.FromDateStatus == CompOffApplicationEdit.FromDateStatus)
                                        {
                                            Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                                + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                                + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                            PopulateDropDown();
                                            CompOffApplicationNecessaryContent(EmpId);
                                            return View(CompOffApplicationEdit);
                                        }
                                    }
                                }
                                else
                                {
                                    Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                        + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                        + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                    PopulateDropDown();
                                    CompOffApplicationNecessaryContent(EmpId);
                                    return View(CompOffApplicationEdit);
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
                                        Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                            + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                        PopulateDropDown();
                                        CompOffApplicationNecessaryContent(EmpId);
                                        return View(CompOffApplicationEdit);
                                    }
                                    else
                                    {
                                        if (ODTourTblObj.ODDayStatus == CompOffApplicationEdit.FromDateStatus)
                                        {
                                            Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                                + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                                + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                            PopulateDropDown();
                                            CompOffApplicationNecessaryContent(EmpId);
                                            return View(CompOffApplicationEdit);
                                        }
                                        else
                                        {
                                            if (CompOffApplicationEdit.FromDateStatus == 1)
                                            {
                                                Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                                    + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                                    + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                                PopulateDropDown();
                                                CompOffApplicationNecessaryContent(EmpId);
                                                return View(CompOffApplicationEdit);
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
                                        Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                            + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                            + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                        PopulateDropDown();
                                        CompOffApplicationNecessaryContent(EmpId);
                                        return View(CompOffApplicationEdit);
                                    }
                                    else
                                    {
                                        if (ODTourTblObj.ODDayStatus == CompOffApplicationEdit.FromDateStatus)
                                        {
                                            Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                                + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                                + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                            PopulateDropDown();
                                            CompOffApplicationNecessaryContent(EmpId);
                                            return View(CompOffApplicationEdit);
                                        }
                                        else
                                        {
                                            if (CompOffApplicationEdit.FromDateStatus == 1)
                                            {
                                                Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                                    + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                                    + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                                PopulateDropDown();
                                                CompOffApplicationNecessaryContent(EmpId);
                                                return View(CompOffApplicationEdit);
                                            }
                                        }
                                    }
                                }
                                else if (CurrentDate == ODTourTblObj.ToDate)
                                {
                                    if (ODTourTblObj.ODDayStatus == 1)  // Full day in single day
                                    {
                                        Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                            + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                            + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                        PopulateDropDown();
                                        CompOffApplicationNecessaryContent(EmpId);
                                        return View(CompOffApplicationEdit);
                                    }
                                    else
                                    {
                                        if (ODTourTblObj.ODDayStatus == CompOffApplicationEdit.FromDateStatus)
                                        {
                                            Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                                + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                                + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                            PopulateDropDown();
                                            CompOffApplicationNecessaryContent(EmpId);
                                            return View(CompOffApplicationEdit);
                                        }
                                    }
                                }
                                else
                                {
                                    Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                        + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                        + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                    PopulateDropDown();
                                    CompOffApplicationNecessaryContent(EmpId);
                                    return View(CompOffApplicationEdit);
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
                    DailyTransaction DailyTransactionListObj = WetosDB.DailyTransactions.Where(a => a.TranDate >= CompOffApplicationEdit.FromDate && a.TranDate <= CompOffApplicationEdit.ToDate && a.EmployeeId == EmpId && a.Lock == "Y").FirstOrDefault();

                    if (DailyTransactionListObj != null)
                    {
                        ModelState.AddModelError("", "You Can not apply Comp Off for this range as Data is Locked");

                        PopulateDropDown();

                        return View(CompOffApplicationEdit);
                    }
                    // Added by Rajas on 10 MARCH 2017 to Validate if data is locked, can not apply leave END
                    #endregion

                    #endregion

                    #region CompOff Allowed Days Logic

                    double AllowedDays = Convert.ToDouble(Collection["AppliedDays"]);

                    TimeSpan ts = (Convert.ToDateTime(CompOffApplicationEdit.ToDate)) - (Convert.ToDateTime(CompOffApplicationEdit.FromDate));
                    float AppliedDay = ts.Days;
                    if (CompOffApplicationEdit.FromDateStatus == 1 && CompOffApplicationEdit.ToDateStatus == 1)
                    {
                        AppliedDay = AppliedDay + 1;
                    }

                        //Modified By Shraddha on 30 DEC 2016 for leave application validation changes start
                    else if ((CompOffApplicationEdit.FromDateStatus == 2) && (CompOffApplicationEdit.ToDateStatus == 2) && CompOffApplicationEdit.FromDate == CompOffApplicationEdit.ToDate)
                    {
                        AppliedDay = 0.5F;
                    }
                    else if ((CompOffApplicationEdit.FromDateStatus == 3) && (CompOffApplicationEdit.ToDateStatus == 3) && CompOffApplicationEdit.FromDate == CompOffApplicationEdit.ToDate)
                    {
                        AppliedDay = 0.5F;
                    }
                    else if ((CompOffApplicationEdit.FromDateStatus == 2) && (CompOffApplicationEdit.ToDateStatus == 3) && CompOffApplicationEdit.FromDate == CompOffApplicationEdit.ToDate)
                    {
                        AppliedDay = 1F;
                    }

                    else if ((CompOffApplicationEdit.FromDateStatus == 2 || CompOffApplicationEdit.FromDateStatus == 3) && (CompOffApplicationEdit.ToDateStatus == 2 || CompOffApplicationEdit.ToDateStatus == 3) && CompOffApplicationEdit.FromDate != CompOffApplicationEdit.ToDate)
                    {
                        AppliedDay = AppliedDay + 0;
                    }

                    else if (CompOffApplicationEdit.FromDateStatus == 2 || CompOffApplicationEdit.ToDateStatus == 2 || CompOffApplicationEdit.FromDateStatus == 3 || CompOffApplicationEdit.ToDateStatus == 3)
                    {
                        AppliedDay = (float)(AppliedDay + 0.5);
                    }

                    if (AppliedDay > AllowedDays)
                    {

                        ModelState.AddModelError("", "You Can't Apply Comp Off for more than allowed days");
                        CompOffApplicationNecessaryContent(Convert.ToInt32(CompOffApplicationEdit.EmployeeId));
                        PopulateDropDown();
                        //List<CompOffApplication> CompOffApplicationList = WetosDB.CompOffApplications.Where(a => a.EmployeeId == EmployeeNo).ToList();
                        return View(CompOffApplicationEdit);

                    }

                    if (AppliedDay < AllowedDays)
                    {

                        ModelState.AddModelError("", "You can not apply comp off for less than Total Allowed CompOff Days");
                        CompOffApplicationNecessaryContent(Convert.ToInt32(CompOffApplicationEdit.EmployeeId));
                        PopulateDropDown();
                        //List<CompOffApplication> CompOffApplicationList = WetosDB.CompOffApplications.Where(a => a.EmployeeId == EmployeeNo).ToList();
                        return View(CompOffApplicationEdit);

                    }
                    #endregion
                    else
                    {

                        WetosDB.CompOffApplication CompOffApplicationObj = WetosDB.CompOffApplications.Where(b => b.CompOffId == id).FirstOrDefault();

                        //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 11 SEPTEMBER 2017 FOR EDIT
                        WetosDB.CompOffApplication CompOffApplicationObjEDIT = WetosDB.CompOffApplications.Where(b => b.CompOffId == id).FirstOrDefault();


                        Employee EmployeeDetailsObj = WetosDB.Employees.Where(a => a.EmployeeId == CompOffApplicationEdit.EmployeeId).FirstOrDefault();
                        CompOffApplicationObj.FromDate = CompOffApplicationEdit.FromDate;
                        CompOffApplicationObj.FromDateStatus = CompOffApplicationEdit.FromDateStatus;
                        CompOffApplicationObj.ToDate = CompOffApplicationEdit.ToDate;
                        CompOffApplicationObj.ToDateStatus = CompOffApplicationEdit.ToDateStatus;
                        CompOffApplicationObj.EmployeeId = CompOffApplicationEdit.EmployeeId;
                        CompOffApplicationObj.CompanyId = EmployeeDetailsObj.CompanyId;
                        CompOffApplicationObj.BranchId = EmployeeDetailsObj.BranchId;
                        CompOffApplicationObj.Purpose = CompOffApplicationEdit.Purpose;
                        //Added By Shraddha on 12 DEC 2016 for taking Status along with statusId Start
                        CompOffApplicationObj.Status = WetosDB.DropdownDatas.Where(a => a.TypeId == 3 && a.Value == CompOffApplicationEdit.StatusId).Select(a => a.Text).FirstOrDefault();
                        //Added By Shraddha on 12 DEC 2016 for taking Status along with statusId End

                        CompOffApplicationObj.EffectiveDate = CompOffApplicationEdit.EffectiveDate; // CODE ADDED BY SHRADDHA ON 17 MAR 2018

                        WetosDB.SaveChanges();

                        //ADDED CODE BY SHRADDHA ON 11 AUG 2017 START
                        if (CompOffList != null)
                        {
                            foreach (CompOff CompOffObj in CompOffList)
                            {
                                CompOffObj.CoDate = CompOffApplicationEdit.FromDate;
                                CompOffObj.CompOffApplicationID = CompOffApplicationObj.CompOffId;
                                WetosDB.SaveChanges();
                            }
                        }
                        //ADDED CODE BY SHRADDHA ON 11 AUG 2017 END

                        //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 11 SEPTEMBER 2017
                        #region ADD AUDIT LOG
                        string Oldrecord = "Employee ID : " + EmpId
                                            + "FromDate :" + CompOffApplicationObjEDIT.FromDate + "To Date :" + CompOffApplicationObjEDIT.ToDate
                                            + "FromDateStatus :" + CompOffApplicationObjEDIT.FromDateStatus + "ToDateStatus :" + CompOffApplicationObjEDIT.ToDateStatus
                                            + "Purpose :" + CompOffApplicationObjEDIT.Purpose + " Status : " + CompOffApplicationObjEDIT.Status + "Reject Reason :"
                                            + CompOffApplicationObjEDIT.RejectReason + "ExtraHoursDate :" + CompOffApplicationObjEDIT.ExtrasHoursDate + "Branch ID :"
                                            + CompOffApplicationObjEDIT.BranchId + "Company ID:" + CompOffApplicationObjEDIT.CompanyId;
                        //---x---------x-----------------x------------x----------------x-----------x-------------x------------x--------------x----------x---------x-----

                        string Newrecord = "Employee ID : " + EmpId
                                            + "FromDate :" + CompOffApplicationObj.FromDate + "To Date :" + CompOffApplicationObj.ToDate
                                            + "FromDateStatus :" + CompOffApplicationObj.FromDateStatus + "ToDateStatus :" + CompOffApplicationObj.ToDateStatus
                                            + "Purpose :" + CompOffApplicationObj.Purpose + " Status : " + CompOffApplicationObj.Status + "Reject Reason :"
                                            + CompOffApplicationObj.RejectReason + "ExtraHoursDate :" + CompOffApplicationObj.ExtrasHoursDate + "Branch ID :"
                                            + CompOffApplicationObj.BranchId + "Company ID:" + CompOffApplicationObj.CompanyId;

                        //LOGIN ID IS TAKEN FROM SESSION PERSISTER
                        string Formname = "CUMULATIVE COMPOFF APPLICATION";
                        //ACTION IS UPDATE
                        string Message = " ";

                        WetosAdministrationController.GenerateAuditLogsUpdate(WetosDB, EmployeeDetailsObj.EmployeeId, Formname, Oldrecord,
                            Newrecord, ref Message);
                        #endregion
                        //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 11 SEPTEMBER 2017


                        #region COMPOFF EDIT NOTIFICATION

                        // FOR NOTIFICATION TO BE SENT TO REPORTING PERSON

                        Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == CompOffApplicationEdit.EmployeeId).FirstOrDefault();
                        Notification NotificationObj = new Notification();
                        NotificationObj.FromID = EmployeeObj.EmployeeId;
                        NotificationObj.ToID = EmployeeObj.EmployeeReportingId;
                        NotificationObj.SendDate = DateTime.Now;
                        NotificationObj.NotificationContent = "Applied CompOff edited by " + EmployeeObj.FirstName + " " + EmployeeObj.LastName + " , is pending for approval";
                        NotificationObj.ReadFlag = false;
                        NotificationObj.SendDate = DateTime.Now;

                        WetosDB.Notifications.AddObject(NotificationObj);

                        WetosDB.SaveChanges();

                        //FOR SELF NOTIFICATION

                        Notification NotificationObj2 = new Notification();
                        NotificationObj2.FromID = EmployeeObj.EmployeeId;
                        NotificationObj2.ToID = EmployeeObj.EmployeeId;
                        NotificationObj2.SendDate = DateTime.Now;
                        NotificationObj2.NotificationContent = "Applied CompOff edited successfully";
                        NotificationObj2.ReadFlag = false;
                        NotificationObj2.SendDate = DateTime.Now;

                        WetosDB.Notifications.AddObject(NotificationObj2);

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
                        AddAuditTrail("Success - CompOff application edited successfully");  // Updated by Rajas on 17 JAN 2017

                        // Added by Rajas on 17 JAN 2017 START
                        Success("Success - CompOff application edited successfully");

                        return RedirectToAction("CompOffApplicationIndex");
                    }
                }
                else
                {
                    //Modified By Shraddha on 12 DEC 2016 Added Populate Dropdown and CompOffApplicationEdit object pass start
                    PopulateDropDown();

                    //Modified By Shraddha on 12 DEC 2016 Added Populate Dropdown and CompOffApplicationEdit object pass End

                    // Added by Rajas on 17 JAN 2017 START
                    AddAuditTrail("Error - CompOff application edit failed");  // Updated by Rajas on 17 JAN 2017

                    Error("Error - CompOff application edit failed");
                    // Added by Rajas on 17 JAN 2017 END

                    return View(CompOffApplicationEdit);
                }
            }

            catch (System.Exception ex)
            {
                PopulateDropDown();

                // Added by Rajas on 17 JAN 2017 START
                AddAuditTrail("Error - CompOff application edit failed due to " + ex.Message);  // Updated by Rajas on 17 JAN 2017

                Error("Error - CompOff application edit failed due to " + ex.Message);
                // Added by Rajas on 17 JAN 2017 END

                return View(CompOffApplicationEdit);

            }
        }

        /// <summary>
        /// Comp Off application index
        /// Added by Rajas on 17 NOV 2016
        /// </summary>
        /// <returns></returns>
        public ActionResult CompOffApplicationIndex(string IsMySelf = "true")
        {
            try
            {
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);

                //GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == "Current Financial Year").FirstOrDefault();

                //ABOVE LINE COMMENTED AND BELOW LINE IS ADDED BY SHALAKA ON 13TH DEC 2017
                GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.CurrentFinancialYear).FirstOrDefault();

                if (GlobalSettingObj == null)
                {
                    Error("Inconsistent Financial year data");
                }
                DateTime CalanderStartDate = GetFYStartDate(); // DateTime CalanderStartDate = WetosDB.FinancialYears.Where(a => a.FinancialName == GlobalSettingObj.SettingValue).Select(a => a.StartDate).FirstOrDefault();
                List<Vw_CompOffApplicationIndex> CompoffApplicationListObj = new List<Vw_CompOffApplicationIndex>();  // Verify ?
                if (CalanderStartDate != null)
                {
                    //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO MAKE SELF AND OTHER COMP OFF APPLICATION GENERIC CODE START
                    if (IsMySelf == "true")
                    {
                        //CompoffApplicationListObj = WetosDB.Vw_CompOffApplicationIndex.Where(a => a.EmployeeId == EmployeeId
                        //&& a.FromDate >= CalanderStartDate && (a.MarkedAsDelete == 0 || a.MarkedAsDelete == null)).ToList();

                        CompoffApplicationListObj = WetosDB.Vw_CompOffApplicationIndex.Where(a => a.EmployeeId == EmployeeId
                        && (a.MarkedAsDelete == 0 || a.MarkedAsDelete == null)).ToList();
                    }
                    else
                    {
                        CompoffApplicationListObj = WetosDB.Vw_CompOffApplicationIndex.Where(a => a.FromDate >= CalanderStartDate && (a.MarkedAsDelete == 0 || a.MarkedAsDelete == null)).ToList();
                    }
                    //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO MAKE SELF AND OTHER COMP OFF APPLICATION GENERIC CODE END
                }
                AddAuditTrail("List of applied CO checked");

                return View(CompoffApplicationListObj);
            }

            catch (System.Exception ex)
            {
                AddAuditTrail("Error in Compoff list view due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                Error("Error in viewing list for Compoff");

                return View("Error");
            }
        }

        /// <summary>
        /// CompOffApplicationDelete
        /// Updated by Rajas on 9 JUNE 2017
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult CompOffApplicationDelete(int CompOffApplicationId)
        {
            try
            {
                CompOffApplication COMPOFFAPPLICATIONObj = WetosDB.CompOffApplications.Where(a => a.CompOffId == CompOffApplicationId).FirstOrDefault();


                List<CompOff> CompOffList = WetosDB.CompOffs.Where(a => a.CompOffApplicationID == CompOffApplicationId).ToList();
                if (COMPOFFAPPLICATIONObj != null)
                {
                    COMPOFFAPPLICATIONObj.MarkedAsDelete = 1;


                    if (CompOffList.Count > 0)
                    {
                        foreach (CompOff CompOffObj in CompOffList)
                        {
                            CompOffObj.AppStatus = null;
                            CompOffObj.CoDate = null; // ADDED BY SHRADDHA ON 11 AUG 2017
                            CompOffObj.CompOffApplicationID = null;
                            WetosDB.SaveChanges();
                        }
                    }

                    WetosDB.SaveChanges();

                    // ADDED BY MSJ ON 09 JAN 2019 START
                    COMPOFFAPPLICATIONObj.CancelledBy = Convert.ToInt32(Session["EmployeeNo"]);
                    COMPOFFAPPLICATIONObj.CancelledOn = DateTime.Now;
                    // ADDED BY MSJ ON 09 JAN 2019 END

                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 12 SEPTEMBER 2017
                    #region ADD AUDIT LOG
                    //OLD RECORD IS BLANK
                    string Oldrecord = "Employee ID : " + COMPOFFAPPLICATIONObj.EmployeeId
                        + "FromDate :" + COMPOFFAPPLICATIONObj.FromDate + "To Date :" + COMPOFFAPPLICATIONObj.ToDate
                        + "FromDateStatus :" + COMPOFFAPPLICATIONObj.FromDateStatus + "ToDateStatus :" + COMPOFFAPPLICATIONObj.ToDateStatus
                        + "Purpose :" + COMPOFFAPPLICATIONObj.Purpose + " Status : " + COMPOFFAPPLICATIONObj.Status + "Reject Reason :"
                        + COMPOFFAPPLICATIONObj.RejectReason + "ExtraHoursDate :" + COMPOFFAPPLICATIONObj.ExtrasHoursDate + "Branch ID :"
                        + COMPOFFAPPLICATIONObj.BranchId + "Company ID:" + COMPOFFAPPLICATIONObj.CompanyId;

                    //LOGIN ID IS TAKEN FROM SESSION PERSISTER
                    string Formname = "CUMULATIVE COMPOFF DELETE";
                    //ACTION IS UPDATE
                    string Message = " ";
                    int EMP = Convert.ToInt32(COMPOFFAPPLICATIONObj.EmployeeId);

                    WetosAdministrationController.GenerateAuditLogsDelete(WetosDB, EMP, Formname, Oldrecord, ref Message);
                    #endregion
                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 12 SEPTEMBER 2017


                    Success("CO applied between  " + COMPOFFAPPLICATIONObj.FromDate.Value.ToString("dd-MMM-yyyy")
                        + " to " + COMPOFFAPPLICATIONObj.ToDate.Value.ToString("dd-MMM-yyyy") + " deleted successfully");

                    AddAuditTrail("CO applied between  " + COMPOFFAPPLICATIONObj.FromDate.Value.ToString("dd-MMM-yyyy")
                        + " to " + COMPOFFAPPLICATIONObj.ToDate.Value.ToString("dd-MMM-yyyy") + " deleted successfully");
                }

                return RedirectToAction("CompOffApplicationIndex");
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                Error("Please try again!");

                return RedirectToAction("CompOffApplicationIndex");
            }
        }


        private int TimeOfDTToMinute(TimeSpan DTObj)
        {
            int TotalMinutes = DTObj.Hours * 60 + DTObj.Minutes;
            return TotalMinutes;
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

                    //AttendanceStatusList = WetosDB.CompOffs.Where(a => a.EmployeeId == EmpId && a.AppStatus != "S" && a.CoHours.Value.TimeOfDay >= NewDateForHalfDay.TimeOfDay).ToList();

                    AttendanceStatusList = WetosDB.CompOffs.Where(a => a.EmployeeId == EmployeeId && ((a.CoHours.Value.Hour * 60 + a.CoHours.Value.Minute) >= (NewDateForHalfDay.Hour * 60 + NewDateForHalfDay.Minute)) && a.DayStatus != null).ToList(); //DAYSTATUS CONDITION ADDED BY SHRADDHA ON 05 MARCH 2018

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
        /// MODIFIED CODE BY SHRADDHA ON 09 MARCH 2017 FOR CompOffApplication GET
        /// Try catch block added by Rajas on 22 FEB 2017
        /// </summary>
        /// <returns></returns>
        public ActionResult CompOffApplication()
        {
            try
            {
                int EmpId = Convert.ToInt32(Session["EmployeeNo"]);
                COMPOffApplicationModel ComOffApp = new COMPOffApplicationModel();
                CompOffApplicationNecessaryContent(EmpId);
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
        public ActionResult CompOffApplication(COMPOffApplicationModel CompOffModelObj, FormCollection Collection)
        {
            #region COMPOFF CODE MODIFIED BY SHRADDHA
            int EmpId = Convert.ToInt32(Session["EmployeeNo"]);
            try
            {
                int CheckCompOffId = 0;
                string CheckStatus = Collection["CheckStatus"];
                if (CheckStatus != null)
                {
                    List<string> CheckStatusDataList = CheckStatus.Split(',').ToList();

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
                            if (CheckStatusDataObj == "false")
                            {

                            }
                            else
                            {
                                CheckCompOffId = Convert.ToInt32(CheckStatusDataObj);
                                CheckCompOffIdList.Add(CheckCompOffId);

                            }
                        }

                    }


                    double TotalAllowedCompOff = Convert.ToDouble(Collection["TtlAllwdDys"]);
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



                        if (AppliedDay > TotalAllowedCompOff)
                        {

                            ModelState.AddModelError("", "You Can't Apply Comp Off for more than allowed days");
                            CompOffApplicationNecessaryContent(EmpId);
                            PopulateDropDown();
                            //List<CompOffApplication> CompOffApplicationList = WetosDB.CompOffApplications.Where(a => a.EmployeeId == EmployeeNo).ToList();
                            return View(CompOffModelObj);

                        }

                        if (AppliedDay < TotalAllowedCompOff)
                        {

                            ModelState.AddModelError("", "You can not apply comp off for less than Total Allowed CompOff Days");
                            CompOffApplicationNecessaryContent(EmpId);
                            PopulateDropDown();
                            //List<CompOffApplication> CompOffApplicationList = WetosDB.CompOffApplications.Where(a => a.EmployeeId == EmployeeNo).ToList();
                            return View(CompOffModelObj);

                        }


                        CompOffApplication COMPOFFAPPLICATIONObj = new CompOffApplication();

                        //added by shraddha on 10 jan 2017 
                        Employee EmployeeDetailsObj = WetosDB.Employees.Where(a => a.EmployeeId == EmpId).FirstOrDefault();
                        COMPOFFAPPLICATIONObj.BranchId = EmployeeDetailsObj.BranchId;
                        COMPOFFAPPLICATIONObj.CompanyId = EmployeeDetailsObj.CompanyId;
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

                        COMPOFFAPPLICATIONObj.EffectiveDate = CompOffModelObj.EffectiveDate; // CODE ADDED BY SHRADDHA ON 17 MAR 2018

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

                        VwActiveEmployee EmployeeObj = WetosDB.VwActiveEmployees.Where(a => a.EmployeeId == COMPOFFAPPLICATIONObj.EmployeeId).FirstOrDefault();
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
                    Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == EmpId).FirstOrDefault();
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
        /// List view for Exception entry
        /// Added by Rajas on 6 MARCH 2017
        /// </summary>
        /// <returns></returns>
        /// Updated by Rajas on 2 OCT 2017
        public ActionResult ExceptionEntryListView(string IsMySelf = "true")
        {
            try
            {
                int EmpId = Convert.ToInt32(Session["EmployeeNo"]);

                List<SP_ExceptionEntryListView_Result> ExceptionEntryListViewObj = new List<SP_ExceptionEntryListView_Result>();

                if (IsMySelf == "true")
                {
                    // Updated by Rajas on 29 AUGUST 2017
                    ExceptionEntryListViewObj = WetosDB.SP_ExceptionEntryListView(EmpId)
                        .Where(a => (a.MarkedAsDelete == null || a.MarkedAsDelete == 0) && a.EmployeeId == EmpId).OrderBy(a => a.ExceptionDate).ToList();
                }
                else
                {
                    ExceptionEntryListViewObj = WetosDB.SP_ExceptionEntryListView(EmpId)
                        .Where(a => (a.MarkedAsDelete == null || a.MarkedAsDelete == 0) && a.EmployeeId != EmpId).OrderBy(a => a.ExceptionDate).ToList();
                }

                //ADDED BY RAJAS ON 27 DEC 2016
                AddAuditTrail("Success - Viewed exception entry list"); // Updated by Rajas on 6 MARCH 2017

                ViewBag.ForOthers = IsMySelf; // Added by Rajas on 2 OCT 2017

                return View(ExceptionEntryListViewObj);
            }

            catch (System.Exception ex)
            {
                AddAuditTrail("Error : Error during checking list for Attendance Regularization due to " + ex.Message 
                    + " and " + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                Error("Error : Unable to display Attendance Regularization list");

                return View("Error");
            }
        }

        /// <summary>
        /// ExceptionEntryListViewDelete
        /// Added by Rajas on 9 JUNE 2017
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ExceptionEntryListViewDelete(int id)
        {
            try
            {
                WetosDB.ExceptionEntry ExceptionObj = WetosDB.ExceptionEntries.Where(a => a.ExceptionId == id).FirstOrDefault();

                if (ExceptionObj != null)
                {
                    ExceptionObj.MarkedAsDelete = 1;

                    // ADDED BY MSJ ON 09 JAN 2019 START
                    ExceptionObj.CancelledBy = Convert.ToInt32(Session["EmployeeNo"]);
                    ExceptionObj.CancelledOn = DateTime.Now;
                    // ADDED BY MSJ ON 09 JAN 2019 END

                    WetosDB.SaveChanges();


                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 11 SEPTEMBER 2017
                    #region ADD AUDIT LOG
                    //NEW RECORD IS BLANK
                    string Oldrecord = "SHIFT : " + ExceptionObj.ShiftId + ", LOGIN : "
                        + ExceptionObj.LoginTime
                        + ", LOGOUT : " + ExceptionObj.LogOutTime + "REMARK : " + ExceptionObj.Remark;

                    //LOGIN ID IS TAKEN FROM SESSION PERSISTER
                    string Formname = "ADJUSTMENT DELETE";
                    //ACTION IS UPDATE
                    string Message = " ";

                    WetosAdministrationController.GenerateAuditLogsDelete(WetosDB, ExceptionObj.EmployeeId, Formname, Oldrecord, ref Message);
                    #endregion
                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 11 SEPTEMBER 2017

                    Success("Exception entry applied on  " + ExceptionObj.ExceptionDate.Value.ToString("dd-MMM-yyyy") + " deleted successfully");

                    AddAuditTrail("Exception entry applied on  " + ExceptionObj.ExceptionDate.Value.ToString("dd-MMM-yyyy") + " deleted successfully");
                }

                return RedirectToAction("ExceptionEntryListView");
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                Error("Please try again!");

                return RedirectToAction("ExceptionEntryListView");
            }
        }

        /// <summary>
        /// EXCEPTION ENTRY PAGE ON WHICH EXCEPTIONENTRY PARTIALVIEW WILL BE OPENED ADDED BY SHRADDHA ON 19 DEC 2016
        /// </summary>
        /// <returns></returns>
        /// Updated by Rajas on 2 OCT 2017
        public ActionResult ExceptionEntry(bool MySelf)
        {
            ExceptionEntryModel ExceptionEntryModelObj = new ExceptionEntryModel();

            try
            {
                ExceptionEntryModelObj.MySelf = MySelf;
                ExceptionEntryModelObj.EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);

                PopulateDropDown();

                return View(ExceptionEntryModelObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in Exception entry apply due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                Error("Error in exception entry apply.");

                return View(ExceptionEntryModelObj);
            }
        }

        /// <summary>
        /// Updated by Rajas on 2 OCT 2017
        /// Self and other's selection combined.
        /// </summary>
        /// <param name="EmployeeId"></param>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <param name="selectCriteria"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ExceptionEntryIndex(int EmployeeId, DateTime FromDate, DateTime ToDate, string selectCriteria)
        {
            //PASSING AN INPUT PARAMETER TO getExceptionEntryList() FUNCTION TO TAKE LOGIN EMPLOYEEWISE DATA BY SHRADDHA ON 12 JAN 2017
            //int EmpId = Convert.ToInt32(Session["EmployeeNo"]);  // Commented by Rajas on 2 OCT 2017

            //List<WetosDB.DailyTransactions> ExceptionEntryList = WetosDB.DailyTransactions.ToList();

            List<SP_GetDailyTransactionList_Result> ExceptionEntryList = WetosDB.SP_GetDailyTransactionList(FromDate, ToDate)
                .Where(A => A.EmployeeId == EmployeeId).OrderByDescending(a=>a.Login).ToList();

            if (selectCriteria == "Present")
            {
                ExceptionEntryList = ExceptionEntryList.Where(a => a.Status == "PPPP").ToList();
            }
            if (selectCriteria == "Absent")
            {
                ExceptionEntryList = ExceptionEntryList.Where(a => a.Status == "AAAA").ToList();
            }

            if (selectCriteria == "Single Punch")
            {
                ExceptionEntryList = ExceptionEntryList.Where(a => a.Status == "AAPP" || a.Status == "PPAA").ToList();
            }
            if (selectCriteria == "Late & Present")
            {
                //EARLIER CODE COMMENTED AND ADDED NEW CODE BY SHRADDHA ON 23 NOV 2017 TO GET DATE LIST FOR LATE & PRESENT RECORDS START
                //ExceptionEntryList = ExceptionEntryList.Where(a => a.Status == "AAPP").ToList();
                ExceptionEntryList = ExceptionEntryList.Where(a => a.Status == "AAPP^" || a.Status == "PPPP^").ToList();
                //EARLIER CODE COMMENTED AND ADDED NEW CODE BY SHRADDHA ON 23 NOV 2017 TO GET DATE LIST FOR LATE & PRESENT RECORDS END
            }

            // ADDED BY PUSHKAR FOR AuditLog ON 21 DEC 2016
            AddAuditTrail("Exception Entry");

            return PartialView(ExceptionEntryList);
        }

        /// <summary>
        /// ADDED BY SHRADDHA ON 19 DEC 2016 FOR Exception
        /// </summary>
        /// <param name="EmployeeId"></param>
        /// <param name="TranDate"></param>
        /// <returns></returns>
        /// Updated by Rajas on 2 OCT 2017
        public ActionResult ExceptionEntryApply(int EmployeeId, DateTime TranDate)
        {
            int EmpId = Convert.ToInt32(Session["EmployeeNo"]);  // Get session EmployeeId

            Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == EmployeeId).FirstOrDefault();
            try
            {
                ViewBag.active = true;

                ExceptionEntryModel ExceptionEntryModelObj = new ExceptionEntryModel();  // Added ExceptionEntryModel by Rajas on 2 OCT 2017

                DailyTransaction ExceptionEntryEdit = WetosDB.DailyTransactions.Where(b => b.EmployeeId == EmployeeId && b.TranDate == TranDate).FirstOrDefault();

                if (ExceptionEntryEdit != null)
                {
                    ExceptionEntryModelObj.DailyTrnId = ExceptionEntryEdit.DailyTrnId;
                    ExceptionEntryModelObj.EmployeeId = ExceptionEntryEdit.EmployeeId;
                    ExceptionEntryModelObj.CompanyId = ExceptionEntryEdit.CompanyId;
                    ExceptionEntryModelObj.BranchId = ExceptionEntryEdit.BranchId; // ADDED CODE BY SHRADDHA ON 15 JAN 2018

                    //COMMENTED BY SHRADDHA AND ADDED NEW CODE BY SHRADDHA ON 15 JAN 2018 START
                    //ExceptionEntryModelObj.LoginTime = ExceptionEntryEdit.Login;
                    //ExceptionEntryModelObj.LogOutTime = ExceptionEntryEdit.LogOut;
                    ExceptionEntryModelObj.PreviousLoginTime = ExceptionEntryEdit.Login;
                    ExceptionEntryModelObj.PreviousLogOutTime = ExceptionEntryEdit.LogOut;
                    //COMMENTED BY SHRADDHA AND ADDED NEW CODE BY SHRADDHA ON 15 JAN 2018 END


                    ExceptionEntryModelObj.ShiftId = ExceptionEntryEdit.ShiftId;
                    ExceptionEntryModelObj.Remark = string.Empty;
                    ExceptionEntryModelObj.TranDate = ExceptionEntryEdit.TranDate;
                    ExceptionEntryModelObj.DailyTranStatus = ExceptionEntryEdit.Status;

                    ExceptionEntryModelObj.PreviousShiftId = ExceptionEntryEdit.ShiftId;//CODE ADDED BY SHRADDHA ON 15 JAN 2018


                    if (EmpId == ExceptionEntryEdit.EmployeeId)
                    {
                        ExceptionEntryModelObj.MySelf = true;
                    }
                    else
                    {
                        ExceptionEntryModelObj.MySelf = false;
                    }
                }

                // Updated by Rajas on 18 JUNE 2017
                //TAKEN INT STATUSID INSTEAD OF STRING STATUS BY SHRADDHA ON 12 SEP 2017
                WetosDB.ExceptionEntry ExceptionEntryIsAvailableForEmployee = WetosDB.ExceptionEntries.Where(a => a.ExceptionDate == TranDate
                    && a.EmployeeId == EmployeeId
                    && a.MarkedAsDelete == 0 && (a.StatusId != null || a.StatusId != 2)).FirstOrDefault();

                if (ExceptionEntryIsAvailableForEmployee != null)
                {
                    // MODIFIED Y MSJ ON 07 DEC 2018 START ADDED NEW CONDITION
                    // Updated by Rajas on 20 JUNE 2017 to ignore Rejection condition
                    // if (ExceptionEntryIsAvailableForEmployee.StatusId != 3 ) //ADDED STATUSID INSTEAD OF STATUS BY SHRADDHA ON 12 SEP 2017
                    if (ExceptionEntryIsAvailableForEmployee.StatusId == 5 || ExceptionEntryIsAvailableForEmployee.StatusId == 3 || ExceptionEntryIsAvailableForEmployee.StatusId == 6)
                    {

                    }
                    else
                    {
                        Error("You Can not Apply Attendance Regularization Entry For " + ExceptionEntryIsAvailableForEmployee.ExceptionDate.Value.ToString("dd-MMM-yyyy")
                       + ". You may already have pending Attendance Regularization Entries for this date.");
                        ViewBag.active = false;
                    }
                }



                // Updated by Rajas on 2 OCT 2017 
                // ExceptionEntryModel obj is passed to view instead of DailyTransactionObj
                PopulateDropdownForExceptionEntry(EmployeeObj); // ADDED CODE BY SHRADDHA ON 15 JAN 2018
                return View(ExceptionEntryModelObj);  // ExceptionEntryEdit
            }

            catch (System.Exception ex)
            {
                // Added by Rajas on 17 FEB 2017 START
                AddAuditTrail("Error - Error in Attendance Regularization due to " + ex.Message);

                Error("Please select valid entry from list");
                ViewBag.active = true;
                DailyTransaction ExceptionEntryEdit = WetosDB.DailyTransactions.Where(b => b.EmployeeId == EmployeeId && b.TranDate == TranDate).FirstOrDefault();

                // Updated by Rajas on 18 JUNE 2017
                //TAKEN INT STATUSID INSTEAD OF STRING STATUS BY SHRADDHA ON 12 SEP 2017
                WetosDB.ExceptionEntry ExceptionEntryIsAvailableForEmployee = WetosDB.ExceptionEntries.Where(a => a.ExceptionDate == TranDate
                    && a.EmployeeId == EmployeeId && a.MarkedAsDelete == 0 && (a.StatusId != null || a.StatusId != 2)).FirstOrDefault();
                if (ExceptionEntryIsAvailableForEmployee != null)
                {
                    // Updated by Rajas on 20 JUNE 2017 to ignore Rejection condition
                    if (ExceptionEntryIsAvailableForEmployee.StatusId != 3) //TAKEN INT STATUSID INSTEAD OF STRING STATUS BY SHRADDHA ON 12 SEP 2017
                    {
                        Error("You Can not Apply Attendance Regularization Entry For " + ExceptionEntryIsAvailableForEmployee.ExceptionDate.Value.ToString("dd-MMM-yyyy")
                            + ". You may already have pending Attendance Regularization Entries for this date.");
                        ViewBag.active = false;
                    }
                }

                PopulateDropdownForExceptionEntry(EmployeeObj); // ADDED CODE BY SHRADDHA ON 15 JAN 2018

                return View(ExceptionEntryEdit);
            }
        }


        /// <summary>
        /// ADDED BY SHRADDHA ON 19 DEC 2016 FOPR ExceptionEntry Edit POST
        /// </summary>
        /// <param name="ExceptionEntryEdit"></param>
        /// <param name="fc"></param>
        /// <returns></returns>
        /// Updated by Rajas on 12 AUGUST 2017
        [HttpPost]
        public ActionResult ExceptionEntryApply(ExceptionEntryModel ExceptionEntryEdit, FormCollection fc)
        {
            int EmpId = ExceptionEntryEdit.EmployeeId;

            //int EmpId = Convert.ToInt32(Session["EmployeeNo"]);

            Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == EmpId).FirstOrDefault();
            try
            {
                DateTime ExceptionDate = Convert.ToDateTime(ExceptionEntryEdit.TranDate);

                ExceptionEntryEdit.ExceptionDate = Convert.ToDateTime(ExceptionEntryEdit.TranDate); //CODE ADDED BY SHRADDHA ON 15 JAN 2018

                DateTime LoginDate = new DateTime(ExceptionEntryEdit.ExceptionDate.Year, ExceptionEntryEdit.ExceptionDate.Month, ExceptionEntryEdit.ExceptionDate.Day, ExceptionEntryEdit.LoginTime.Hour, ExceptionEntryEdit.LoginTime.Minute, ExceptionEntryEdit.LoginTime.Second);
                
                DateTime LogOutDate = new DateTime(ExceptionEntryEdit.ExceptionDate.Year, ExceptionEntryEdit.ExceptionDate.Month, ExceptionEntryEdit.ExceptionDate.Day, ExceptionEntryEdit.LogOutTime.Hour, ExceptionEntryEdit.LogOutTime.Minute, ExceptionEntryEdit.LogOutTime.Second);
                //ADDED IF ELSE BY SHRADDHA ON 15 JAN 2018 START

                if (ExceptionEntryEdit.IsInPunchInNextDay == true)
                {
                    ExceptionEntryEdit.LoginTime = LoginDate.AddDays(1);
                    ExceptionEntryEdit.IsOutPunchInNextDay = true;
                }
                else
                {
                    ExceptionEntryEdit.LoginTime = LoginDate;
                }

                if (ExceptionEntryEdit.IsOutPunchInNextDay == true)
                {
                    ExceptionEntryEdit.LogOutTime = LogOutDate.AddDays(1);
                }
                else
                {
                    ExceptionEntryEdit.LogOutTime = LogOutDate;
                }
                //ADDED IF ELSE BY SHRADDHA ON 15 JAN 2018 END

                // Commented by Rajas on 2 OCT 2017, Not Required.
                //DailyTransaction ExceptionEntryObj = WetosDB.DailyTransactions.Where(b => b.TranDate == ExceptionDate && b.EmployeeId == ExceptionEntryEdit.EmployeeId).FirstOrDefault();

                //CODE ADDED BY SHRADDHA ON 30 AUG 2017 FOR VALIDATION EXCEPTION DATE SHOULD  NOT BE GREATER THAN MAX TRANDATE IN DT TABLE START
                DailyTransaction GetMaxTranDateFromDT = WetosDB.DailyTransactions.Where(a => a.EmployeeId == EmpId).OrderByDescending(a => a.TranDate).FirstOrDefault();
                if (ExceptionEntryEdit.TranDate > GetMaxTranDateFromDT.TranDate)
                {
                    PopulateDropdownForExceptionEntry(EmployeeObj);  // Added by Rajas on 2 OCT 2017

                    Error("Attendance Regularization Entry Date is Invalid");
                    // Added by Rajas on 17 JAN 2017 END

                    return View(ExceptionEntryEdit);
                }
                //CODE ADDED BY SHRADDHA ON 30 AUG 2017 FOR VALIDATION EXCEPTION DATE SHOULD  NOT BE GREATER THAN MAX TRANDATE IN DT TABLE END
                
                if (ExceptionEntryEdit.ShiftId == null)
                {
                    PopulateDropdownForExceptionEntry(EmployeeObj);  // Added by Rajas on 2 OCT 2017

                    // Added by Rajas on 17 JAN 2017 START
                    AddAuditTrail("Error - Attendance Regularization entry failed due to shift id null");

                    Error("Please Enter valid shift");
                    // Added by Rajas on 17 JAN 2017 END

                    return View(ExceptionEntryEdit);
                }

                DateTime DefaultDate = new DateTime(0001, 01, 01, 00, 00, 00);
                if (ExceptionEntryEdit.LoginTime == DefaultDate)
                {
                    PopulateDropdownForExceptionEntry(EmployeeObj);  // Added by Rajas on 2 OCT 2017

                    return View(ExceptionEntryEdit);

                }
                else if (ExceptionEntryEdit.LogOutTime == DefaultDate)
                {
                    PopulateDropdownForExceptionEntry(EmployeeObj);  // Added by Rajas on 2 OCT 2017

                    return View(ExceptionEntryEdit);
                }

                if (ExceptionEntryEdit.LogOutTime < ExceptionEntryEdit.LoginTime)
                {
                    PopulateDropdownForExceptionEntry(EmployeeObj);  // Added by Rajas on 2 OCT 2017

                    Error("Log out time should be greater than LogIn time");

                    return View(ExceptionEntryEdit);
                }

                //int n;
                int Reason;
                bool isNumeric = int.TryParse(fc["Reason"], out Reason);

                //int Reason = Convert.ToInt32(fc["Reason"]);

                string OurRemark = fc["Remark"].ToString();  // To save remark from view in case of Special approval

                // Added by Rajas on 12 AUGUST 2017 START
                if (Reason == 9)
                {
                    if (OurRemark == null || OurRemark == string.Empty)
                    {
                        PopulateDropdownForExceptionEntry(EmployeeObj);   // Added by Rajas on 2 OCT 2017

                        Error("Please enter valid remark in case of Special Approval");
                        // Added by Rajas on 17 JAN 2017 END

                        return View(ExceptionEntryEdit);
                    }
                }
                // Added by Rajas on 12 AUGUST 2017 END

                // ADDED BY MSJ ON 09 JAN 2018 START
                // LOCK DATA
                DailyTransaction DailyTransactionObj = WetosDB.DailyTransactions.Where(a => a.EmployeeId == ExceptionEntryEdit.EmployeeId
                    && a.TranDate == ExceptionEntryEdit.TranDate && a.Lock == "Y").FirstOrDefault();

                if (Reason == 9)
                {
                    if (OurRemark == null || OurRemark == string.Empty)
                    {
                        PopulateDropdownForExceptionEntry(EmployeeObj);   // Added by Rajas on 2 OCT 2017

                        Error("You Can not apply Attendance Regularization entry for this date as Data is Locked");
                        // Added by Rajas on 17 JAN 2017 END

                        return View(ExceptionEntryEdit);
                    }
                }


                // EXISTING EXCEPTIONENTRY
                WetosDB.ExceptionEntry ExistingExceptionEntry = WetosDB.ExceptionEntries.Where(a => a.EmployeeId == ExceptionEntryEdit.EmployeeId
                    && a.ExceptionDate == ExceptionEntryEdit.TranDate && a.MarkedAsDelete == 0
                    && (a.StatusId == 1 || a.StatusId == 2 || a.StatusId == 4)).FirstOrDefault();

                // EXISTING EXCEPTION ENTRY
                if (ExistingExceptionEntry == null)  // ADDED BY MSJ ON 09 JAN 2018 
                {
                    WetosDB.ExceptionEntry ExceptionTbleObj = new WetosDB.ExceptionEntry();
                    ExceptionTbleObj.DailyTrnId = ExceptionEntryEdit.DailyTrnId;
                    int CompanyId = WetosDB.Employees.Where(a => a.EmployeeId == ExceptionEntryEdit.EmployeeId).Select(a => a.CompanyId).FirstOrDefault();

                    //ExceptionTbleObj.Company = WetosDB.Companies.Where(a => a.CompanyId == CompanyId).FirstOrDefault();
                    ExceptionTbleObj.CompanyId = Convert.ToInt32(CompanyId);
                    int BranchId = WetosDB.Employees.Where(a => a.EmployeeId == ExceptionEntryEdit.EmployeeId).Select(a => a.BranchId).FirstOrDefault();

                    //ExceptionTbleObj.Branch = WetosDB.Branches.Where(a => a.BranchId == BranchId).FirstOrDefault();
                    ExceptionTbleObj.BranchId = Convert.ToInt32(BranchId);

                    //ExceptionTbleObj.Employee = WetosDB.Employees.Where(a =>a.EmployeeId == ExceptionEntryEdit.EmployeeId).FirstOrDefault();

                    ExceptionTbleObj.EmployeeId = Convert.ToInt32(ExceptionEntryEdit.EmployeeId);
                    ExceptionTbleObj.ExceptionDate = new DateTime(ExceptionDate.Year, ExceptionDate.Month, ExceptionDate.Day, 00, 00, 00);
                    ExceptionTbleObj.PreviousShiftId = ExceptionEntryEdit.PreviousShiftId == null ? " " : ExceptionEntryEdit.PreviousShiftId;
                    ExceptionTbleObj.StatusId = 1; // UPDATED BY SHRADDHA ON 12 SEP 2017 STRING TO INT
                    ExceptionTbleObj.Status = WetosDB.DropdownDatas.Where(a => a.TypeId == 3 && a.Value == ExceptionTbleObj.StatusId)
                        .Select(a => a.Text).FirstOrDefault();//ADDED BY SHRADDHA ON 12 SEP 2017 TO SAVE STATUS
                    string ReasonStr = WetosDB.DropdownDatas.Where(a => a.TypeId == 10 && a.Value == Reason).Select(a => a.Text).FirstOrDefault();
                    if (OurRemark != null)
                    {
                        ExceptionTbleObj.Remark = (ReasonStr == null ? "" : ReasonStr) + " : " + OurRemark;
                    }

                    else
                    {
                        ExceptionTbleObj.Remark = ReasonStr == null ? " " : ReasonStr;
                    }

                    ExceptionTbleObj.LogOutTime = ExceptionEntryEdit.LogOutTime;
                    ExceptionTbleObj.LoginTime = ExceptionEntryEdit.LoginTime;

                    ExceptionTbleObj.ShiftId = ExceptionEntryEdit.ShiftId == null ? " " : ExceptionEntryEdit.ShiftId;

                    //CODE ADDED BY SHRADDHA ON 15 JAN 2018 START
                    ExceptionTbleObj.IsOutPunchInNextDay = ExceptionEntryEdit.IsOutPunchInNextDay;
                    ExceptionTbleObj.IsInPunchInNextDay = ExceptionEntryEdit.IsInPunchInNextDay;
                    ExceptionTbleObj.PreviousLoginTime = ExceptionEntryEdit.PreviousLoginTime;
                    ExceptionTbleObj.PreviousLogOutTime = ExceptionEntryEdit.PreviousLogOutTime;
                    ExceptionTbleObj.LoginTime = ExceptionEntryEdit.LoginTime;
                    ExceptionTbleObj.LogOutTime = ExceptionEntryEdit.LogOutTime;
                    //CODE ADDED BY SHRADDHA ON 15 JAN 2018 END

                    ExceptionTbleObj.EffectiveDate = ExceptionEntryEdit.EffectiveDate; //CODE ADDED BY SHRADDHA ON 17 MAR 2018
                    ExceptionTbleObj.Purpose = ExceptionEntryEdit.Description;

                    WetosDB.ExceptionEntries.AddObject(ExceptionTbleObj);
                    WetosDB.SaveChanges();

                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 11 SEPTEMBER 2017
                    #region ADD AUDIT LOG

                    //OLD RECORD IS BLANK
                    // Updated by Rajas on 2 OCT 2017 START
                    string Newrecord = "SHIFT : " + ExceptionTbleObj.ShiftId + ", LOGIN : "
                        + ExceptionTbleObj.LoginTime
                        + ", LOGOUT : " + ExceptionTbleObj.LogOutTime + ", REMARK : " + ExceptionTbleObj.Remark + ", EmployeeId : " + ExceptionEntryEdit.EmployeeId;
                    // Updated by Rajas on 2 OCT 2017 END

                    //LOGIN ID IS TAKEN FROM SESSION PERSISTER
                    string Formname = "ADJUSTMENT ENTRY";
                    //ACTION IS UPDATE
                    string Message = " ";

                    WetosAdministrationController.GenerateAuditLogsInsert(WetosDB, ExceptionTbleObj.EmployeeId, Formname, Newrecord, ref Message);

                    #endregion
                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 11 SEPTEMBER 2017

                    // NOTIFICATION ADDED BY RAJAS ON 31 DEC 2016
                    #region EXCEPTION ENTRY APPLICATION NOTIFICATION

                    EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == ExceptionTbleObj.EmployeeId).FirstOrDefault();
                    Notification NotificationObj = new Notification();
                    NotificationObj.FromID = EmployeeObj.EmployeeId;
                    NotificationObj.ToID = EmployeeObj.EmployeeReportingId;
                    NotificationObj.SendDate = DateTime.Now;
                    NotificationObj.NotificationContent = "Received Attendance Regularization entry application for approval from "
                        + EmployeeObj.FirstName + " " + EmployeeObj.LastName + " for " + ExceptionTbleObj.ExceptionDate.Value.ToString("dd-MMM-yyyy");
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
                                if (SendEmail(EmployeeObj.Email, NotificationObj.NotificationContent, NotificationObj.NotificationContent, ref EmailUpdateStatus, "Exception entry Application") == false)
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

                    // Added by Rajas on 17 JAN 2017 START
                    AddAuditTrail("Success - Attendance Regularization entry successful");

                    Success("Success - Attendance Regularization entry successful");
                    // Added by Rajas on 17 JAN 2017 END

                    // Updated by Rajas on 2 OCT 2017 START
                    // Redirection on list view on basis of self and other
                    if (ExceptionEntryEdit.MySelf == true)
                    {
                        return RedirectToAction("ExceptionEntryListView");
                    }
                    else
                    {
                        return RedirectToAction("ExceptionEntryListView", new { IsMySelf = "false" });
                    }
                    // Updated by Rajas on 2 OCT 2017 END
                }
                else
                {
                    // ADDED BY MSJ ON 09 JAN 2018 
                    Error("You Can not Apply Attendance Regularization Entry For " + ExceptionEntryEdit.TranDate.ToString("dd-MMM-yyyy")
                           + ". You may already have pending Attendance Regularization Entries for this date.");
                    ViewBag.active = false;

                    PopulateDropdownForExceptionEntry(EmployeeObj);  // ADDED BY MSJ ON 09 JAN 2018

                    return View(ExceptionEntryEdit);

                    // COMMENTED BY MSJ ON 09 JAN 2018 
                    //// Updated by Rajas on 2 OCT 2017 START
                    //// Redirection on list view on basis of self and other
                    //if (ExceptionEntryEdit.MySelf == true)
                    //{
                    //    return RedirectToAction("ExceptionEntryListView");
                    //}
                    //else
                    //{
                    //    return RedirectToAction("ExceptionEntryListView", new { IsMySelf = "false" });
                    //}
                    // Updated by Rajas on 2 OCT 2017 END
                }

                // ADDED BY MSJ ON 09 JAN 2018 END
            }
            catch (System.Exception ex)
            {
                // Added by Rajas on 17 FEB 2017 START
                AddAuditTrail("Error - Error in Attendance Regularization due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                Error("Please select valid entry from list");

                // Updated by Rajas on 2 OCT 2017 START
                // Redirection on list view on basis of self and other
                if (ExceptionEntryEdit.MySelf == true)
                {
                    return RedirectToAction("ExceptionEntryListView");
                }
                else
                {
                    return RedirectToAction("ExceptionEntryListView", new { IsMySelf = "false" });
                }
                // Updated by Rajas on 2 OCT 2017 END
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="EmployeeId"></param>
        /// <param name="ExceptionDate"></param>
        /// <returns></returns>
        /// Updated by Rajas on 27 APRIL 2017
        public ActionResult ExceptionEntryEdit(int EmployeeId, DateTime ExceptionDate)
        {
            int EmpId = Convert.ToInt32(Session["EmployeeNo"]);
            Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == EmployeeId).FirstOrDefault();

            ExceptionEntryModel ExceptionEntryModelObj = new ExceptionEntryModel();
            try
            {
                // DailyTransaction ExceptionEntryEdit = WetosDB.DailyTransactions.Where(b => b.EmployeeId == EmployeeId && b.TranDate == TranDate).FirstOrDefault();
                WetosDB.ExceptionEntry ExceptionEntryIsAvailableForEmployee = WetosDB.ExceptionEntries.Where(a => a.ExceptionDate == ExceptionDate
                    && a.EmployeeId == EmployeeId && a.MarkedAsDelete != 1).FirstOrDefault();

                if (ExceptionEntryIsAvailableForEmployee != null)
                {
                    ExceptionEntryModelObj.EmployeeId = ExceptionEntryIsAvailableForEmployee.EmployeeId;
                    ExceptionEntryModelObj.CompanyId = ExceptionEntryIsAvailableForEmployee.CompanyId;
                    ExceptionEntryModelObj.BranchId = ExceptionEntryIsAvailableForEmployee.BranchId;
                    ExceptionEntryModelObj.LogOutTime = ExceptionEntryIsAvailableForEmployee.LogOutTime;
                    ExceptionEntryModelObj.LoginTime = ExceptionEntryIsAvailableForEmployee.LoginTime;
                    ExceptionEntryModelObj.ShiftId = ExceptionEntryIsAvailableForEmployee.ShiftId;
                    ExceptionEntryModelObj.ExceptionDate = Convert.ToDateTime(ExceptionEntryIsAvailableForEmployee.ExceptionDate); // CODE ADDED BY SHRADDHA ON 15 JAN 2018
                    ExceptionEntryModelObj.TranDate = Convert.ToDateTime(ExceptionEntryIsAvailableForEmployee.ExceptionDate); // CODE ADDED BY SHRADDHA ON 15 JAN 2018
                    ExceptionEntryModelObj.ShiftId = ExceptionEntryIsAvailableForEmployee.ShiftId;
                    ExceptionEntryModelObj.PreviousShiftId = ExceptionEntryIsAvailableForEmployee.PreviousShiftId; // CODE ADDED BY SHRADDHA ON 15 JAN 2018

                    ExceptionEntryModelObj.PreviousLoginTime = ExceptionEntryIsAvailableForEmployee.PreviousLoginTime; // CODE ADDED BY SHRADDHA ON 15 JAN 2018
                    ExceptionEntryModelObj.PreviousLogOutTime = ExceptionEntryIsAvailableForEmployee.PreviousLogOutTime; // CODE ADDED BY SHRADDHA ON 15 JAN 2018

                    ExceptionEntryModelObj.ExceptionDate = ExceptionEntryIsAvailableForEmployee.ExceptionDate.Value;   // Added by Rajas on 3 OCT 2017

                    ExceptionEntryModelObj.EffectiveDate = ExceptionEntryIsAvailableForEmployee.EffectiveDate; //CODE ADDED BY SHRADDHA ON 17 MAR 2018
                    ExceptionEntryModelObj.Description = ExceptionEntryIsAvailableForEmployee.Purpose;

                    if (EmpId == EmployeeId)
                    {
                        ExceptionEntryModelObj.MySelf = true;
                    }
                    else
                    {
                        ExceptionEntryModelObj.MySelf = false;
                    }

                    ExceptionEntryModelObj.Remark = string.Empty;
                }

                PopulateDropdownForExceptionEntry(EmployeeObj);

                return View(ExceptionEntryModelObj);
            }

            catch (System.Exception ex)
            {
                // Added by Rajas on 17 FEB 2017 START
                AddAuditTrail("Error - Error in Attendance Regularization due to " + ex.Message);

                Error("Please select valid entry from list");
                PopulateDropdownForExceptionEntry(EmployeeObj); // ADDED CODE BY SHRADDHA ON 15 JAN 2018

                // Added by Rajas on 17 FEB 2017 END

                return View();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ExceptionEntryEdit"></param>
        /// <param name="fc"></param>
        /// <returns></returns>
        /// Updated by Rajas on 2 OCT 2017
        [HttpPost]
        public ActionResult ExceptionEntryEdit(ExceptionEntryModel ExceptionEntryEdit, FormCollection fc)
        {
            int EmpId = Convert.ToInt32(ExceptionEntryEdit.EmployeeId);
            //int EmpId = Convert.ToInt32(Session["EmployeeNo"]);
            Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == EmpId).FirstOrDefault();
            try
            {
                ExceptionEntryEdit.ExceptionDate = Convert.ToDateTime(ExceptionEntryEdit.TranDate); //CODE ADDED BY SHRADDHA ON 15 JAN 2018
                DateTime LoginDate = new DateTime(ExceptionEntryEdit.ExceptionDate.Year, ExceptionEntryEdit.ExceptionDate.Month, ExceptionEntryEdit.ExceptionDate.Day, ExceptionEntryEdit.LoginTime.Hour, ExceptionEntryEdit.LoginTime.Minute, ExceptionEntryEdit.LoginTime.Second);
                DateTime LogOutDate = new DateTime(ExceptionEntryEdit.ExceptionDate.Year, ExceptionEntryEdit.ExceptionDate.Month, ExceptionEntryEdit.ExceptionDate.Day, ExceptionEntryEdit.LogOutTime.Hour, ExceptionEntryEdit.LogOutTime.Minute, ExceptionEntryEdit.LogOutTime.Second);
                //ADDED IF ELSE BY SHRADDHA ON 15 JAN 2018 START

                if (ExceptionEntryEdit.IsInPunchInNextDay == true)
                {
                    ExceptionEntryEdit.LoginTime = LoginDate.AddDays(1);
                    ExceptionEntryEdit.IsOutPunchInNextDay = true;
                }
                else
                {
                    ExceptionEntryEdit.LoginTime = LoginDate;
                }

                if (ExceptionEntryEdit.IsOutPunchInNextDay == true)
                {
                    ExceptionEntryEdit.LogOutTime = LogOutDate.AddDays(1);
                }
                else
                {
                    ExceptionEntryEdit.LogOutTime = LogOutDate;
                }
                //ADDED IF ELSE BY SHRADDHA ON 15 JAN 2018 END


                // Commented by Rajas on 2 OCT 2017 Not in use.
                //DailyTransaction ExceptionEntryObj = WetosDB.DailyTransactions.Where(b => b.TranDate == ExceptionEntryEdit.ExceptionDate && b.EmployeeId == ExceptionEntryEdit.Employee.EmployeeId).FirstOrDefault();
                if (ExceptionEntryEdit.ShiftId == null)
                {
                    PopulateDropdownForExceptionEntry(EmployeeObj);  // Added by Rajas on 2 OCT 2017

                    // Added by Rajas on 17 JAN 2017 START
                    AddAuditTrail("Error - Attendance Regularization entry failed due to shift id null");

                    Error("Please Enter valid shift");
                    // Added by Rajas on 17 JAN 2017 END

                    return View(ExceptionEntryEdit);
                }

                if (ExceptionEntryEdit.LogOutTime < ExceptionEntryEdit.LoginTime)
                {
                    PopulateDropdownForExceptionEntry(EmployeeObj);  // Added by Rajas on 2 OCT 2017

                    // Added by Rajas on 17 JAN 2017 START
                    AddAuditTrail("Log out time should be greater than LogIn time");

                    Error("Log out time should be greater than LogIn time");
                    // Added by Rajas on 17 JAN 2017 END

                    return View(ExceptionEntryEdit);
                }

                //ADDED ModelState.IsValid BY SHRADDHA FOR VALIDATING DATA ON 17 JAN 2017
                if (ModelState.IsValid)
                {

                    // string LogOutDate = fc["LogOutDate"].ToString();
                    //string LogOutTime = fc["LogOutTime"].ToString();

                    // string LoginTime = fc["LoginTime"].ToString();

                    //int Reason = Convert.ToInt32(fc["Reason"]); // COMMENTED BY MSJ ON 14 FEB 2020
                    string ReasonStr = WetosDB.DropdownDatas.Where(a => a.TypeId == 10 && a.Value == ExceptionEntryEdit.Reason).Select(a => a.Text).FirstOrDefault();

                    // COMMENTED BY MSJ ON 14 FEB 2020
                    string OurRemark = ExceptionEntryEdit.Remark; // fc["Remark"].ToString();  // To save remark from view in case of Special approval

                    WetosDB.ExceptionEntry ExceptionTbleObj = WetosDB.ExceptionEntries.Where(a => a.ExceptionDate == ExceptionEntryEdit.ExceptionDate
                        && a.EmployeeId == ExceptionEntryEdit.EmployeeId && (a.MarkedAsDelete == null || a.MarkedAsDelete == 0)).FirstOrDefault();

                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 11 SEPTEMBER 2017 FOT EDIT
                    WetosDB.ExceptionEntry ExceptionTbleObjEDIT = WetosDB.ExceptionEntries.Where(a => a.ExceptionDate == ExceptionEntryEdit.ExceptionDate
                        && a.EmployeeId == ExceptionEntryEdit.EmployeeId).FirstOrDefault();

                    ExceptionTbleObj.DailyTrnId = ExceptionEntryEdit.DailyTrnId;

                    //ExceptionTbleObj.BranchId = Convert.ToInt32(ExceptionEntryEdit.BranchId);
                    // Above line commented and below line added by Rajas on 28 APRIL 2017, to Handle BranchId = 0 issue 
                    ExceptionTbleObj.BranchId = Convert.ToInt32(WetosDB.Employees.Where(a => a.EmployeeId == ExceptionEntryEdit.EmployeeId)
                        .Select(a => a.BranchId).FirstOrDefault());

                    ExceptionTbleObj.CompanyId = Convert.ToInt32(WetosDB.Employees.Where(a => a.EmployeeId == ExceptionEntryEdit.EmployeeId)
                        .Select(a => a.CompanyId).FirstOrDefault());

                    ExceptionTbleObj.EmployeeId = Convert.ToInt32(ExceptionEntryEdit.EmployeeId);
                    ExceptionTbleObj.ExceptionDate = new DateTime(ExceptionEntryEdit.ExceptionDate.Year, ExceptionEntryEdit.ExceptionDate.Month, ExceptionEntryEdit.ExceptionDate.Day, 00, 00, 00);

                    ExceptionTbleObj.PreviousShiftId = ExceptionEntryEdit.PreviousShiftId == null ? " " : ExceptionEntryEdit.PreviousShiftId;
                    //ExceptionTbleObj.Status = "1";
                    ExceptionTbleObj.StatusId = 1; // UPDATED BY SHRADDHA ON 12 SEP 2017 STRING TO INT
                    ExceptionTbleObj.Status = WetosDB.DropdownDatas.Where(a => a.TypeId == 3 && a.Value == ExceptionTbleObj.StatusId).Select(a => a.Text).FirstOrDefault();//ADDED BY SHRADDHA ON 12 SEP 2017 TO SAVE STATUS
                    //CODE ADDED BY SHRADDHA TO PROVIDE EXCEPTION REMARK COMPLSORY VALIDATION IN CASE OF REASON IS SPECIAL APPROVAL ON 30 AUG 2017 START
                    if (ExceptionEntryEdit.Reason == 9)
                    {
                        if (OurRemark == null || OurRemark == string.Empty)
                        {
                            PopulateDropdownForExceptionEntry(EmployeeObj);  // Added by Rajas on 2 OCT 2017

                            Error("Please enter valid remark in case of Special Approval");
                            // Added by Rajas on 17 JAN 2017 END

                            return View(ExceptionEntryEdit);
                        }
                    }
                    //CODE ADDED BY SHRADDHA TO PROVIDE EXCEPTION REMARK COMPLSORY VALIDATION IN CASE OF REASON IS SPECIAL APPROVAL ON 30 AUG 2017 START


                    if (ExceptionEntryEdit.Reason == 9)
                    {
                        ExceptionTbleObj.Remark = (ReasonStr == null ? "" : ReasonStr) + " : " + OurRemark;
                    }

                    else
                    {
                        ExceptionTbleObj.Remark = ReasonStr == null ? " " : ReasonStr;
                    }
                    ExceptionTbleObj.LogOutTime = ExceptionEntryEdit.LogOutTime;
                    // Updated by Rajas on 2 MARCH 2017
                    ExceptionTbleObj.LoginTime = ExceptionEntryEdit.LoginTime; //Convert.ToDateTime(LoginDate + " " + LoginTime);

                    ExceptionTbleObj.ShiftId = ExceptionEntryEdit.ShiftId == null ? " " : ExceptionEntryEdit.ShiftId;



                    //CODE ADDED BY SHRADDHA ON 15 JAN 2018 START
                    ExceptionTbleObj.IsOutPunchInNextDay = ExceptionEntryEdit.IsOutPunchInNextDay;
                    ExceptionTbleObj.IsInPunchInNextDay = ExceptionEntryEdit.IsInPunchInNextDay;
                    ExceptionTbleObj.PreviousLoginTime = ExceptionEntryEdit.PreviousLoginTime;
                    ExceptionTbleObj.PreviousLogOutTime = ExceptionEntryEdit.PreviousLogOutTime;
                    ExceptionTbleObj.LoginTime = ExceptionEntryEdit.LoginTime;
                    ExceptionTbleObj.LogOutTime = ExceptionEntryEdit.LogOutTime;
                    //CODE ADDED BY SHRADDHA ON 15 JAN 2018 END

                    ExceptionTbleObj.EffectiveDate = ExceptionEntryEdit.EffectiveDate; //CODE ADDED BY SHRADDHA ON 17 MAR 2018
                    ExceptionTbleObj.Purpose = ExceptionEntryEdit.Description;

                    WetosDB.SaveChanges();

                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 11 SEPTEMBER 2017
                    #region ADD AUDIT LOG

                    // Updated by Rajas on 2 OCT 2017 START
                    string Oldrecord = "SHIFT : " + ExceptionTbleObjEDIT.ShiftId + ", LOGIN : "
                    + ExceptionTbleObjEDIT.LoginTime
                    + ", LOGOUT : " + ExceptionTbleObjEDIT.LogOutTime + ", REMARK : " + ExceptionTbleObjEDIT.Remark + ", EmployeeId : " + ExceptionEntryEdit.EmployeeId;
                    //---x---------x-----------------x------------x----------------x-----------x-------------x------------x--------------x----------x---------x-----
                    string Newrecord = "SHIFT : " + ExceptionTbleObj.ShiftId + ", LOGIN : "
                    + ExceptionTbleObj.LoginTime
                    + ", LOGOUT : " + ExceptionTbleObj.LogOutTime + ", REMARK : " + ExceptionTbleObj.Remark + ", EmployeeId : " + ExceptionEntryEdit.EmployeeId;
                    // Updated by Rajas on 2 OCT 2017 END

                    //LOGIN ID IS TAKEN FROM SESSION PERSISTER
                    string Formname = "ADJUSTMENT ENTRY";
                    //ACTION IS UPDATE
                    string Message = " ";

                    WetosAdministrationController.GenerateAuditLogsUpdate(WetosDB, ExceptionTbleObj.EmployeeId, Formname, Oldrecord,
                        Newrecord, ref Message);
                    #endregion
                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 11 SEPTEMBER 2017


                    // NOTIFICATION ADDED BY RAJAS ON 31 DEC 2016
                    #region EXCEPTION ENTRY APPLICATION NOTIFICATION

                    // FOR NOTIFICATION TO BE SENT TO REPORTING PERSON

                    //Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == ExceptionTbleObj.EmployeeId).FirstOrDefault();
                    Notification NotificationObj = new Notification();
                    NotificationObj.FromID = EmployeeObj.EmployeeId;
                    NotificationObj.ToID = EmployeeObj.EmployeeReportingId;
                    NotificationObj.SendDate = DateTime.Now;
                    NotificationObj.NotificationContent = "Attendance Regularization entry applied by " + EmployeeObj.FirstName + " " + EmployeeObj.LastName + " is pending for approval";
                    NotificationObj.ReadFlag = false;
                    NotificationObj.SendDate = DateTime.Now;

                    WetosDB.Notifications.AddObject(NotificationObj);

                    WetosDB.SaveChanges();

                    //FOR SELF NOTIFICATION

                    Notification NotificationObj2 = new Notification();
                    NotificationObj2.FromID = EmployeeObj.EmployeeId;
                    NotificationObj2.ToID = EmployeeObj.EmployeeId;
                    NotificationObj2.SendDate = DateTime.Now;
                    NotificationObj2.NotificationContent = "Attendance Regularization entry applied successfully for " + ExceptionEntryEdit.LoginTime.ToString("dd-MMM-yyyy") + " on " + DateTime.Now.ToString("dd-MMM-yyyy");
                    NotificationObj2.ReadFlag = false;
                    NotificationObj2.SendDate = DateTime.Now;

                    WetosDB.Notifications.AddObject(NotificationObj2);

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
                                if (SendEmail(EmployeeObj.Email, NotificationObj.NotificationContent, NotificationObj.NotificationContent, ref EmailUpdateStatus, "Exception entry Application") == false)
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

                    // Added by Rajas on 17 JAN 2017 START
                    AddAuditTrail("Success - Attendance Regularization entry successful");

                    Success("Success - Attendance Regularization entry successful");
                    // Added by Rajas on 17 JAN 2017 END

                    // Updated by Rajas on 2 OCT 2017 START
                    // Redirection on list view on basis of self and other
                    if (ExceptionEntryEdit.MySelf == true)
                    {
                        return RedirectToAction("ExceptionEntryListView");
                    }
                    else
                    {
                        return RedirectToAction("ExceptionEntryListView", new { IsMySelf = "false" });
                    }
                    // Updated by Rajas on 2 OCT 2017 END
                }

                else
                {
                    PopulateDropdownForExceptionEntry(EmployeeObj);  // Added by Rajas on 2 OCT 2017

                    // Added by Rajas on 17 JAN 2017 START
                    AddAuditTrail("Error - Attendance Regularization entry failed");

                    Error("Error - Attendance Regularization entry failed");
                    // Added by Rajas on 17 JAN 2017 END

                    return View(ExceptionEntryEdit);
                }


            }
            catch (System.Exception ex)
            {
                // Added by Rajas on 17 FEB 2017 START
                AddAuditTrail("Error - Error in Attendance Regularization due to " + ex.Message);

                Error("Please select valid entry from list");

                // Updated by Rajas on 2 OCT 2017 START
                // Redirection on list view on basis of self and other
                if (ExceptionEntryEdit.MySelf == true)
                {
                    return RedirectToAction("ExceptionEntryListView");
                }
                else
                {
                    return RedirectToAction("ExceptionEntryListView", new { IsMySelf = "false" });
                }
                // Updated by Rajas on 2 OCT 2017 END
            }
        }

        /// <summary>
        /// Comman function for Populate dropdown
        /// Added by Rajas on 2 OCT 2017
        /// </summary>
        /// <param name="EmployeeObj"></param>
        /// <returns></returns>
        public bool PopulateDropdownForExceptionEntry(Employee EmployeeObj)
        {
            bool ReturnState = false;

            try
            {
                var Shift = WetosDB.Shifts.Where(a => a.Company.CompanyId == EmployeeObj.CompanyId && a.BranchId == EmployeeObj.BranchId && (a.MarkedAsDelete == 0 || a.MarkedAsDelete == null))
                    .Select(a => new { ShiftId = a.ShiftCode, ShiftName = a.ShiftName }).ToList(); // ADDED MARKEDAS DELETED FLAG BY SHRADDHA ON 02 FEB 2018
                ViewBag.ShiftList = new SelectList(Shift, "ShiftId", "ShiftName").ToList();

                //ADDED CODE BY SHRADDHA ON 30 AUG 2017 FOR EXCEPTION REASON Dropdown list  START
                var ExceptionReasonList = WetosDB.DropdownDatas.Where(a => a.TypeId == 10)
                    .Select(a => new { ExceptionReasonID = a.Value, ExceptionReasonText = a.Text }).ToList();
                ViewBag.ExceptionReasonList = new SelectList(ExceptionReasonList, "ExceptionReasonID", "ExceptionReasonText").ToList();
                //ADDED CODE BY SHRADDHA ON 30 AUG 2017 FOR EXCEPTION REASON Dropdown list END


                //ADDED CODE BY SHRADDHA ON 15 JAN 2018 START
                ViewBag.EmployeeName = (EmployeeObj.FirstName == null ? "" : EmployeeObj.FirstName) + " " + (EmployeeObj.MiddleName == null ? "" : EmployeeObj.MiddleName) + " " + (EmployeeObj.LastName == null ? "" : EmployeeObj.LastName);
                ViewBag.EmployeeCode = EmployeeObj.EmployeeCode == null ? "" : EmployeeObj.EmployeeCode;
                //ADDED CODE BY SHRADDHA ON 15 JAN 2018 END
                return ReturnState = true;
            }
            catch
            {
                return ReturnState;
            }

        }


        //added by shraddha on 14th oct 2016 for actual days
        public JsonResult GetLeaveDetails()
        {
            int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            var ActualDays = WetosDB.LeaveBalances.Where(a => a.EmployeeId == EmployeeId).FirstOrDefault();
            return Json(ActualDays);
        }


        /// <summary>
        /// Updated ODtravelIndex for exception handling
        /// Updated by Rajas on 7 MARCH 2017
        /// </summary>
        /// <returns></returns>
        /// string IsMySelf = "true" added by Rajas on 22 AUGUST 2017
        public ActionResult ODTravelIndex(string IsMySelf = "true")
        {
            try
            {
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);

                #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                //List<VwODTravelIndex> ODTravelObj = new List<VwODTravelIndex>();
                List<SP_VwODTravelIndex_Result> ODTravelObj = new List<SP_VwODTravelIndex_Result>();
                #endregion
                //GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == "Current Financial Year").FirstOrDefault();

                //ABOVE LINE COMMENTED AND BELOW LINE IS ADDED BY SHALAKA ON 13TH DEC 2017
                GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.CurrentFinancialYear).FirstOrDefault();

                // Error
                if (GlobalSettingObj == null)
                {
                    Error("Inconsistent Financial year data");

                    return View(ODTravelObj);
                }

                DateTime CalanderStartDate = GetFYStartDate(); // DateTime CalanderStartDate = WetosDB.FinancialYears.Where(a => a.FinancialName == GlobalSettingObj.SettingValue).Select(a => a.StartDate).FirstOrDefault();

                if (CalanderStartDate != null)
                {
                    if (IsMySelf == "true")
                    {
                        #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                        //ODTravelObj = WetosDB.VwODTravelIndexes.Where(a => a.EmployeeId == EmployeeId)
                        //    .Where(a => a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0).OrderByDescending(a => a.FromDate).ToList();
                        ODTravelObj = WetosDB.SP_VwODTravelIndex(EmployeeId).Where(a => a.EmployeeId == EmployeeId)
                            .Where(a => a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0).OrderByDescending(a => a.FromDate).ToList();
                        #endregion
                    }
                    else
                    {
                        #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                        //ODTravelObj = WetosDB.VwODTravelIndexes
                        //    .Where(a => a.MarkedAsDelete == 0 && a.FromDate >= CalanderStartDate && a.EmployeeId != EmployeeId).OrderByDescending(a => a.FromDate).ToList();
                        ODTravelObj = WetosDB.SP_VwODTravelIndex(EmployeeId)
                            .Where(a => a.MarkedAsDelete == 0 && a.FromDate >= CalanderStartDate && a.EmployeeId != EmployeeId).OrderByDescending(a => a.FromDate).ToList();
                        #endregion
                    }
                }
                // Updated by Rajas on 29 AUGUST 2017 END

                AddAuditTrail("Visited ODTravel list");

                ViewBag.ForOthers = IsMySelf; // Added by Rajas on 22 AUGUST 2017

                return View(ODTravelObj);
            }

            catch (System.Exception ex)
            {
                AddAuditTrail("Error in ODTravel list view due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                Error("Error in viewing list for ODTravel");

                return View("Error");
            }
        }

        /// <summary>
        /// ODTravelDelete
        /// Updated by Rajas on 9 JUNE 2017
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ODTravelDelete(int id)
        {
            try
            {
                ODTour ODTOURObj = WetosDB.ODTours.Where(a => a.ODTourId == id).FirstOrDefault();

                if (ODTOURObj != null)
                {
                    ODTOURObj.MarkedAsDelete = 1;

                    // ADDED BY MSJ ON 09 JAN 2019 START
                    ODTOURObj.CancelledBy = Convert.ToInt32(Session["EmployeeNo"]);
                    ODTOURObj.CancelledOn = DateTime.Now;
                    // ADDED BY MSJ ON 09 JAN 2019 END

                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 7 SEPTEMBER 2017
                    #region ADD AUDIT LOG
                    //OLD RECORD IS BLANK
                    string Oldrecord = "ODTourType : " + ODTOURObj.ODTourType + ", FromDate : " + ODTOURObj.FromDate
                       + ", ToDate : " + ODTOURObj.ToDate + ", ODDayStatus : " + ODTOURObj.ODDayStatus
                       + ", ODDayStatus1 : " + ODTOURObj.ODDayStatus1 + ", AppliedDays :" + ODTOURObj.AppliedDay + ", ActualDays :"
                       + ODTOURObj.ActualDay + ", Purpose :" + ODTOURObj.Purpose + ", Status :" + ODTOURObj.Status
                       + ", BranchId :" + ODTOURObj.BranchId
                       + ", CompanyId :" + ODTOURObj.CompanyId;

                    //LOGIN ID IS TAKEN FROM SESSION PERSISTER
                    string Formname = "REQUISITION DELETE";
                    //ACTION IS UPDATE
                    string Message = " ";

                    WetosAdministrationController.GenerateAuditLogsInsert(WetosDB, ODTOURObj.EmployeeId, Formname, Oldrecord, ref Message);
                    #endregion
                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 7 SEPTEMBER 2017

                    WetosDB.SaveChanges();

                    Success("ODTour applied between  " + ODTOURObj.FromDate.Value.ToString("dd-MMM-yyyy")
                        + " to " + ODTOURObj.ToDate.Value.ToString("dd-MMM-yyyy") + " deleted successfully");

                    AddAuditTrail("ODTour applied between  " + ODTOURObj.FromDate.Value.ToString("dd-MMM-yyyy")
                        + " to " + ODTOURObj.ToDate.Value.ToString("dd-MMM-yyyy") + " deleted successfully");
                }

                return RedirectToAction("ODTravelIndex");
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                Error("Please try again!");

                return RedirectToAction("ODTravelIndex");
            }
        }


        /// <summary>
        /// For editing ODTour application
        /// Added by Rajas On 4 Nov 2016
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// Updated by Rajas on 15 JULY 2017
        public ActionResult ODtravelEdit(int id)
        {
            try
            {
                WetosDB.ODTour ODtravelApplicationEdit = WetosDB.ODTours.Single(a => a.ODTourId == id);

                ODTravelModel ODTravelModelObj = new ODTravelModel();

                ODTravelModelObj.FromDate = ODtravelApplicationEdit.FromDate;

                ODTravelModelObj.ToDate = ODtravelApplicationEdit.ToDate;

                ODTravelModelObj.ODDayStatus = ODtravelApplicationEdit.ODDayStatus.Value;

                ODTravelModelObj.ODDayStatus1 = ODtravelApplicationEdit.ODDayStatus1.Value;

                ODTravelModelObj.EmployeeId = ODtravelApplicationEdit.EmployeeId;

                ODTravelModelObj.ODTourId = ODtravelApplicationEdit.ODTourId;

                ODTravelModelObj.Purpose = ODtravelApplicationEdit.Purpose;

                ODTravelModelObj.Place = ODtravelApplicationEdit.Place;

                // Trim ODTourType status before sending on view
                // Added by Rajas on 15 JULY 2017
                ODTravelModelObj.ODTourType = ODtravelApplicationEdit.ODTourType == null ? string.Empty : ODtravelApplicationEdit.ODTourType.Trim();


                //CODE ADDED BY SHRADDHA ON 30 JAN 2018 START
                //ODTravelModelObj.ODLoginTime = ODtravelApplicationEdit.ODLoginTime;
                //ODTravelModelObj.ODLogOutTime = ODtravelApplicationEdit.ODLogOutTime;
                //ODTravelModelObj.IsInPunchInNextDay = ODtravelApplicationEdit.IsInPunchInNextDay;
                //ODTravelModelObj.IsOutPunchInNextDay = ODtravelApplicationEdit.IsOutPunchInNextDay;

                if (!string.IsNullOrEmpty(ODTravelModelObj.ODTourType))
                {
                    if (ODTravelModelObj.ODTourType == "Half/Full Day OD" || ODTravelModelObj.ODTourType == "Late Reporting - OD"
                        || ODTravelModelObj.ODTourType == "Early Departure - OD")
                    {
                        ODTravelModelObj.ODDate = ODTravelModelObj.FromDate;
                        ODTravelModelObj.ODDayStatus2 = ODTravelModelObj.ODDayStatus1;
                    }
                }
                //CODE ADDED BY SHRADDHA ON 30 JAN 2018 END

                ODTravelModelObj.EffectiveDate = ODtravelApplicationEdit.EffectiveDate; // CODE ADDED BY SHRADDHA ON 17 MAR 2018

                ODTravelModelObj.CustomerName = ODtravelApplicationEdit.CustomerName;

                ODTravelModelObj.CSRNo = ODtravelApplicationEdit.CSRNo;

                ODTravelModelObj.ClientName = ODtravelApplicationEdit.ClientName;

                ODTravelModelObj.PurposeDesc = ODtravelApplicationEdit.PurposeDescription;

                PopulateDropDown();


                return View(ODTravelModelObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("ODTravel edit failed due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                Error("ODTravel edit failed");

                return View();
            }
        }

        /// <summary>
        /// Added by Rajas for ODTravel edit
        /// Updated by Rajas on 17 Nov 2016 for ODTravel edit
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ODtravelApplicationEdit"></param>
        /// <returns></returns>
        /// Updated by Rajas on 22 AUGUST 2017
        [HttpPost]
        public ActionResult ODtravelEdit(int id, ODTravelModel ODtravelApplicationEdit)
        {
            try
            {

                int EmpId = ODtravelApplicationEdit.EmployeeId;

                // In case of self OD/Travel application
                if (EmpId <= 0)
                {
                    EmpId = Convert.ToInt32(Session["EmployeeNo"]);
                }


                //CODE ADDED BY SHRADDHA ON 30 JAN 2018 START
                if (string.IsNullOrEmpty(ODtravelApplicationEdit.ODTourType))
                {
                    ModelState.AddModelError("", "Please select Proper requisition type");

                    PopulateDropDown();

                    return View(ODtravelApplicationEdit);
                }
                else
                {
                    if (ODtravelApplicationEdit.ODTourType == "Half/Full Day OD" || ODtravelApplicationEdit.ODTourType == "Late Reporting - OD" || ODtravelApplicationEdit.ODTourType == "Early Departure - OD")
                    {
                        ODtravelApplicationEdit.FromDate = ODtravelApplicationEdit.ODDate;
                        ODtravelApplicationEdit.ToDate = ODtravelApplicationEdit.ODDate;
                        ODtravelApplicationEdit.ODDayStatus1 = ODtravelApplicationEdit.ODDayStatus2;
                        ODtravelApplicationEdit.ODDayStatus = ODtravelApplicationEdit.ODDayStatus2;
                        if (ODtravelApplicationEdit.ODDate == null)
                        {
                            ModelState.AddModelError("", "Please enter proper OD Date");

                            PopulateDropDown();

                            return View(ODtravelApplicationEdit);
                        }
                        //if (ODtravelApplicationEdit.ODLoginTime == null)
                        //{
                        //    ModelState.AddModelError("", "Please enter proper Login Time");

                        //    PopulateDropDown();

                        //    return View(ODtravelApplicationEdit);
                        //}

                        //if (ODtravelApplicationEdit.ODLogOutTime == null)
                        //{
                        //    ModelState.AddModelError("", "Please enter proper Logout Time");

                        //    PopulateDropDown();

                        //    return View(ODtravelApplicationEdit);
                        //}
                        ////CODE ADDED BY SHRADDHA ON 29 JAN 2018 START
                        //DateTime NextDay = ODtravelApplicationEdit.FromDate.Value.AddDays(1);
                        //if (ODtravelApplicationEdit.IsInPunchInNextDay == true)
                        //{
                        //    ODtravelApplicationEdit.ODLoginTime = new DateTime(NextDay.Year, NextDay.Month, NextDay.Day, ODtravelApplicationEdit.ODLoginTime.Value.Hour, ODtravelApplicationEdit.ODLoginTime.Value.Minute, ODtravelApplicationEdit.ODLoginTime.Value.Second);
                        //}
                        //else
                        //{
                        //    ODtravelApplicationEdit.ODLoginTime = new DateTime(ODtravelApplicationEdit.FromDate.Value.Year, ODtravelApplicationEdit.FromDate.Value.Month, ODtravelApplicationEdit.FromDate.Value.Day, ODtravelApplicationEdit.ODLoginTime.Value.Hour, ODtravelApplicationEdit.ODLoginTime.Value.Minute, ODtravelApplicationEdit.ODLoginTime.Value.Second);
                        //}
                        //if (ODtravelApplicationEdit.IsOutPunchInNextDay == true)
                        //{
                        //    ODtravelApplicationEdit.ODLogOutTime = new DateTime(NextDay.Year, NextDay.Month, NextDay.Day, ODtravelApplicationEdit.ODLogOutTime.Value.Hour, ODtravelApplicationEdit.ODLogOutTime.Value.Minute, ODtravelApplicationEdit.ODLogOutTime.Value.Second);
                        //}
                        //else
                        //{
                        //    ODtravelApplicationEdit.ODLogOutTime = new DateTime(ODtravelApplicationEdit.FromDate.Value.Year, ODtravelApplicationEdit.FromDate.Value.Month, ODtravelApplicationEdit.FromDate.Value.Day, ODtravelApplicationEdit.ODLogOutTime.Value.Hour, ODtravelApplicationEdit.ODLogOutTime.Value.Minute, ODtravelApplicationEdit.ODLogOutTime.Value.Second);
                        //}
                    }
                }
                //CODE ADDED BY SHRADDHA ON 30 JAN 2018 END

                //Added By Shraddha on 12 DEC 2016 for adding date validation start
                if (ODtravelApplicationEdit.FromDate > ODtravelApplicationEdit.ToDate)
                {
                    ModelState.AddModelError("", "End Date should be Greater than or equal to From Date");

                }
                //Added By Shraddha on 12 DEC 2016 for adding date validation End

                // Updated by Rajas on 15 JULY 2017
                if (ODtravelApplicationEdit.FromDate == ODtravelApplicationEdit.ToDate)
                {
                    if (ODtravelApplicationEdit.ODDayStatus != ODtravelApplicationEdit.ODDayStatus1)
                    {
                        // ERROR
                        ModelState.AddModelError("", "Select Proper Day status");

                        // FILL VIEW BAG AGAIN
                        PopulateDropDown();

                        return View(ODtravelApplicationEdit);
                    }
                }
                else
                {
                    //if (ODTourObj.ODDayStatus == "1") // NEED DISCUSSION
                    //{
                    //    // ERROR
                    //    ModelState.AddModelError("", "Select Proper status for from date");
                    //    PopulateDropDown();
                    //    LeaveApplicationNecessaryContent(EmpId);
                    //    return View(ODTourObj);
                    //}

                }

                /// Validation for Punch relaxed in case of OD 
                /// Updated by Rajas on 17 MAY 2017
                /// As per meeting discussion on 17 MAY 2017 with Katre sir, Kulkarni sir, Kirti madam, Madhav sir and Rajas
                #region PUNCH VALIDATION
                // Check if Punch is available for selected employee
                // Added by Rajas on 10 MAY 2017 START
                //DailyTransaction DailyTransactionObj = WetosDB.DailyTransactions.Where(a => a.TranDate >= LeaveTypeObj.FromDate && a.TranDate <= LeaveTypeObj.ToDate
                //    && a.EmployeeId == EmpId && (a.Status == "AAAA" || a.Status == "AAPP" || a.Status == "PPAA")).FirstOrDefault();

                // Updated by Rajas on 17 MAY 2017
                //DailyTransaction DailyTransactionObj = WetosDB.DailyTransactions.Where(a => a.TranDate >= ODTourObj.FromDate && a.TranDate <= ODTourObj.ToDate
                //   && a.EmployeeId == EmpId).FirstOrDefault();

                //if (DailyTransactionObj != null)
                //{
                //    // Added by Rajas on 18 MAY 2017 
                //    TimeSpan TransTime = DailyTransactionObj.TranDate.TimeOfDay;

                //    TimeSpan LoginTime = DailyTransactionObj.Login.TimeOfDay;

                //    if (TransTime != LoginTime)
                //    {
                //        ModelState.AddModelError("", "You can't apply leave as your punch is already available for selected date range");

                //        // Added by Rajas on 30 MARCH 2017 START
                //        string UpdateStatus = string.Empty;

                //        if (LeaveApplicationNecessaryContent(EmpId, ref UpdateStatus) == false)
                //        {
                //            AddAuditTrail("Error in Leave application due to " + UpdateStatus);

                //            Error(UpdateStatus);

                //            return RedirectToAction("LeaveApplicationIndex");
                //        }

                //        PopulateDropDown();

                //        return View(ODTourObj);
                //    }
                //}
                // Added by Rajas on 10 MAY 2017 END
                #endregion

                // Added by Rajas on 9 MAY 2017
                // Updated by Rajas on 9 JULY 2017 START
                // Fixed Issue No. 6 in Flagship Defect Sheet raised by Sandeep sir

                #region VALIDATE OD DATA

                // Updated by Rajas on 27 SEP 2017 START
                // Fixed Issue No. 6 in Flagship Defect Sheet raised by Sandeep sir
                LeaveApplication LeaveApplicationObj = WetosDB.LeaveApplications.Where(a => a.EmployeeId == EmpId
                    && (a.StatusId == 1 || a.StatusId == 2 || a.StatusId == 4) && a.MarkedAsDelete == 0
                    && (
                             (a.FromDate == ODtravelApplicationEdit.FromDate) // && a.ToDate > LeaveTypeObj.ToDate) 
                             || (a.FromDate < ODtravelApplicationEdit.FromDate && a.ToDate == ODtravelApplicationEdit.ToDate)
                             || (a.FromDate < ODtravelApplicationEdit.FromDate && a.ToDate > ODtravelApplicationEdit.ToDate)
                             || (a.FromDate > ODtravelApplicationEdit.FromDate && a.ToDate < ODtravelApplicationEdit.ToDate)
                             || (a.FromDate == ODtravelApplicationEdit.FromDate && a.ToDate == ODtravelApplicationEdit.ToDate)
                             || (a.FromDate > ODtravelApplicationEdit.FromDate && a.FromDate < ODtravelApplicationEdit.ToDate) // ADDED BY MSJ ON 11 JAN 2018
                             || (a.ToDate > ODtravelApplicationEdit.FromDate && a.ToDate < ODtravelApplicationEdit.ToDate) // ADDED BY MSJ ON 11 JAN 2018
                             || (a.ToDate == ODtravelApplicationEdit.FromDate))// FROM DATE AND TO DATE CONDITION ADDED BY SHRADDHA WITH HELP OF MSJ ON 01 FEB 2018

                    ).FirstOrDefault();

                // Added by Rajas on 10 MARCH 2017 for validation to check if already pending or sanctioned CO is applied for same date
                CompOffApplication CompoffApplicationObj = WetosDB.CompOffApplications.Where(a => a.EmployeeId == EmpId
                    && (a.StatusId == 1 || a.StatusId == 2 || a.StatusId == 4)
                    && a.MarkedAsDelete == 0
                    && (
                             (a.FromDate == ODtravelApplicationEdit.FromDate) // && a.ToDate > LeaveTypeObj.ToDate) 
                             || (a.FromDate < ODtravelApplicationEdit.FromDate && a.ToDate == ODtravelApplicationEdit.ToDate)
                             || (a.FromDate < ODtravelApplicationEdit.FromDate && a.ToDate > ODtravelApplicationEdit.ToDate)
                             || (a.FromDate > ODtravelApplicationEdit.FromDate && a.ToDate < ODtravelApplicationEdit.ToDate)
                             || (a.FromDate == ODtravelApplicationEdit.FromDate && a.ToDate == ODtravelApplicationEdit.ToDate)
                             || (a.FromDate > ODtravelApplicationEdit.FromDate && a.FromDate < ODtravelApplicationEdit.ToDate) // ADDED BY MSJ ON 11 JAN 2018
                             || (a.ToDate > ODtravelApplicationEdit.FromDate && a.ToDate < ODtravelApplicationEdit.ToDate) // ADDED BY MSJ ON 11 JAN 2018
                             || (a.ToDate == ODtravelApplicationEdit.FromDate))// FROM DATE AND TO DATE CONDITION ADDED BY SHRADDHA WITH HELP OF MSJ ON 01 FEB 2018
                    ).FirstOrDefault();

                ODTour ODTourTblObj = WetosDB.ODTours.Where(a => a.EmployeeId == EmpId && (a.StatusId == 1 || a.StatusId == 2 || a.StatusId == 4)
                    && a.MarkedAsDelete == 0
                    && (
                             (a.FromDate == ODtravelApplicationEdit.FromDate) // && a.ToDate > LeaveTypeObj.ToDate) 
                             || (a.FromDate < ODtravelApplicationEdit.FromDate && a.ToDate == ODtravelApplicationEdit.ToDate)
                             || (a.FromDate < ODtravelApplicationEdit.FromDate && a.ToDate > ODtravelApplicationEdit.ToDate)
                             || (a.FromDate > ODtravelApplicationEdit.FromDate && a.ToDate < ODtravelApplicationEdit.ToDate)
                             || (a.FromDate == ODtravelApplicationEdit.FromDate && a.ToDate == ODtravelApplicationEdit.ToDate)
                             || (a.FromDate > ODtravelApplicationEdit.FromDate && a.FromDate < ODtravelApplicationEdit.ToDate) // ADDED BY MSJ ON 11 JAN 2018
                             || (a.ToDate > ODtravelApplicationEdit.FromDate && a.ToDate < ODtravelApplicationEdit.ToDate) // ADDED BY MSJ ON 11 JAN 2018
                             || (a.ToDate == ODtravelApplicationEdit.FromDate))
                    && a.ODTourId != ODtravelApplicationEdit.ODTourId).FirstOrDefault();// FROM DATE AND TO DATE CONDITION ADDED BY SHRADDHA WITH HELP OF MSJ ON 01 FEB 2018
                // Updated by Rajas on 9 JULY 2017 END

                for (DateTime CurrentDate = Convert.ToDateTime(ODtravelApplicationEdit.FromDate); CurrentDate.Date <= ODtravelApplicationEdit.ToDate; CurrentDate = CurrentDate.AddDays(1))
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
                                    ModelState.AddModelError("", "You Can not Apply OD For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                    PopulateDropDown();

                                    return View(ODtravelApplicationEdit);
                                }
                                else
                                {
                                    if (LeaveApplicationObj.FromDayStatus == ODtravelApplicationEdit.ODDayStatus)
                                    {
                                        ModelState.AddModelError("", "You Can not Apply OD For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                        PopulateDropDown();

                                        return View(ODtravelApplicationEdit);
                                    }
                                    else
                                    {
                                        if (ODtravelApplicationEdit.ODDayStatus == 1)
                                        {
                                            ModelState.AddModelError("", "You Can not Apply OD For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                            PopulateDropDown();

                                            return View(ODtravelApplicationEdit);
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
                                    ModelState.AddModelError("", "You Can not Apply OD For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                    PopulateDropDown();

                                    return View(ODtravelApplicationEdit);
                                }
                                else
                                {
                                    if (LeaveApplicationObj.FromDayStatus == ODtravelApplicationEdit.ODDayStatus)
                                    {
                                        ModelState.AddModelError("", "You Can not Apply OD For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                        PopulateDropDown();

                                        return View(ODtravelApplicationEdit);
                                    }
                                    else
                                    {
                                        if (ODtravelApplicationEdit.ODDayStatus == 1)
                                        {
                                            ModelState.AddModelError("", "You Can not Apply OD For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                            PopulateDropDown();

                                            return View(ODtravelApplicationEdit);
                                        }
                                    }
                                }
                            }
                            else if (CurrentDate == LeaveApplicationObj.ToDate)
                            {
                                if (LeaveApplicationObj.ToDayStatus == 1)  // Full day in single day
                                {
                                    ModelState.AddModelError("", "You Can not Apply OD For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                    PopulateDropDown();

                                    return View(ODtravelApplicationEdit);
                                }
                                else
                                {
                                    if (LeaveApplicationObj.FromDayStatus == ODtravelApplicationEdit.ODDayStatus)
                                    {
                                        ModelState.AddModelError("", "You Can not Apply OD For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                        PopulateDropDown();

                                        return View(ODtravelApplicationEdit);
                                    }
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", "You Can not Apply OD For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                PopulateDropDown();

                                return View(ODtravelApplicationEdit);
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
                                    ModelState.AddModelError("", "You Can not Apply OD For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                    PopulateDropDown();

                                    return View(ODtravelApplicationEdit);
                                }
                                else
                                {
                                    if (CompoffApplicationObj.FromDateStatus == ODtravelApplicationEdit.ODDayStatus)
                                    {
                                        ModelState.AddModelError("", "You Can not Apply OD For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                        PopulateDropDown();

                                        return View(ODtravelApplicationEdit);
                                    }
                                    else
                                    {
                                        if (ODtravelApplicationEdit.ODDayStatus == 1)
                                        {
                                            ModelState.AddModelError("", "You Can not Apply OD For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                            PopulateDropDown();

                                            return View(ODtravelApplicationEdit);
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
                                    ModelState.AddModelError("", "You Can not Apply OD For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                    PopulateDropDown();

                                    return View(ODtravelApplicationEdit);
                                }
                                else
                                {
                                    if (CompoffApplicationObj.FromDateStatus == ODtravelApplicationEdit.ODDayStatus)
                                    {
                                        ModelState.AddModelError("", "You Can not Apply OD For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                        PopulateDropDown();

                                        return View(ODtravelApplicationEdit);
                                    }
                                    else
                                    {
                                        if (ODtravelApplicationEdit.ODDayStatus == 1)
                                        {
                                            ModelState.AddModelError("", "You Can not Apply OD For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                            PopulateDropDown();

                                            return View(ODtravelApplicationEdit);
                                        }
                                    }
                                }
                            }
                            else if (CurrentDate == CompoffApplicationObj.ToDate)
                            {
                                if (CompoffApplicationObj.ToDateStatus == 1)  // Full day in single day
                                {
                                    ModelState.AddModelError("", "You Can not Apply OD For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                    PopulateDropDown();

                                    return View(ODtravelApplicationEdit);
                                }
                                else
                                {
                                    if (CompoffApplicationObj.FromDateStatus == ODtravelApplicationEdit.ODDayStatus)
                                    {
                                        ModelState.AddModelError("", "You Can not Apply OD For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                        PopulateDropDown();

                                        return View(ODtravelApplicationEdit);
                                    }
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", "You Can not Apply OD For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                PopulateDropDown();

                                return View(ODtravelApplicationEdit);
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

                        //CODE COMMENTED AND ADDED NEW CODE BY SHRADDHA ON 28 OCT 2017 START
                        //if (CompoffApplicationObj.FromDate == CompoffApplicationObj.ToDate) // Single day
                        if (ODTourTblObj.FromDate == ODTourTblObj.ToDate)
                        {
                            //CODE COMMENTED AND ADDED NEW CODE BY SHRADDHA ON 28 OCT 2017 END
                            if (CurrentDate == ODTourTblObj.FromDate)
                            {
                                //CODE COMMENTED AND ADDED NEW CODE BY SHRADDHA ON 28 OCT 2017 START
                                //if (CompoffApplicationObj.FromDateStatus == 1)  // Full day in single day
                                if (ODTourTblObj.ODDayStatus == 1)
                                //CODE COMMENTED AND ADDED NEW CODE BY SHRADDHA ON 28 OCT 2017 END
                                {
                                    ModelState.AddModelError("", "You Can not Apply OD For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned OD for this date range.");

                                    PopulateDropDown();

                                    return View(ODtravelApplicationEdit);
                                }
                                else
                                {
                                    if (ODTourTblObj.ODDayStatus == ODtravelApplicationEdit.ODDayStatus)
                                    {
                                        ModelState.AddModelError("", "You Can not Apply OD For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned OD for this date range.");

                                        PopulateDropDown();

                                        return View(ODtravelApplicationEdit);
                                    }
                                    else
                                    {
                                        if (ODtravelApplicationEdit.ODDayStatus == 1)
                                        {
                                            ModelState.AddModelError("", "You Can not Apply OD For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned OD for this date range.");

                                            PopulateDropDown();

                                            return View(ODtravelApplicationEdit);
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
                                    ModelState.AddModelError("", "You Can not Apply OD For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned OD for this date range.");

                                    PopulateDropDown();

                                    return View(ODtravelApplicationEdit);
                                }
                                else
                                {
                                    if (ODTourTblObj.ODDayStatus == ODtravelApplicationEdit.ODDayStatus)
                                    {
                                        ModelState.AddModelError("", "You Can not Apply OD For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned OD for this date range.");

                                        PopulateDropDown();

                                        return View(ODtravelApplicationEdit);
                                    }
                                    else
                                    {
                                        if (ODtravelApplicationEdit.ODDayStatus == 1)
                                        {
                                            ModelState.AddModelError("", "You Can not Apply OD For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned OD for this date range.");

                                            PopulateDropDown();

                                            return View(ODtravelApplicationEdit);
                                        }
                                    }
                                }
                            }
                            else if (CurrentDate == ODTourTblObj.ToDate)
                            {
                                if (ODTourTblObj.ODDayStatus == 1)  // Full day in single day
                                {
                                    ModelState.AddModelError("", "You Can not Apply OD For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned OD for this date range.");

                                    PopulateDropDown();

                                    return View(ODtravelApplicationEdit);
                                }
                                else
                                {
                                    if (ODTourTblObj.ODDayStatus == ODtravelApplicationEdit.ODDayStatus)
                                    {
                                        ModelState.AddModelError("", "You Can not Apply OD For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned OD for this date range.");

                                        PopulateDropDown();

                                        return View(ODtravelApplicationEdit);
                                    }
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", "You Can not Apply OD For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned OD for this date range.");

                                PopulateDropDown();

                                return View(ODtravelApplicationEdit);
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
                DailyTransaction DailyTransactionListObj = WetosDB.DailyTransactions.Where(a => a.TranDate >= ODtravelApplicationEdit.FromDate
                    && a.TranDate <= ODtravelApplicationEdit.ToDate && a.EmployeeId == EmpId && a.Lock == "Y").FirstOrDefault();

                if (DailyTransactionListObj != null)
                {
                    ModelState.AddModelError("", "You Can not apply leave for this range as Data is Locked");

                    PopulateDropDown();

                    //LeaveApplicationNecessaryContent(EmpId);

                    return View(ODtravelApplicationEdit);
                }
                // Added by Rajas on 10 MARCH 2017 to Validate if data is locked, can not apply leave END
                #endregion

                #endregion

                //Modified By Shraddha on 12 DEC 2016 Added if(ModelState.IsValid) Condition instead of try catch block
                if (ModelState.IsValid)
                {
                    WetosDB.ODTour ODtravelApplicationObj = WetosDB.ODTours.Where(b => b.ODTourId == id).FirstOrDefault();

                    //ADDED BY PUSHKAR ON 7 SEPTEMBER 2017 FOR CREATING NEW OBJ FOR EDIT ON AUDIT LOG
                    WetosDB.ODTour ODtravelApplicationObjEDIT = WetosDB.ODTours.Where(b => b.ODTourId == id).FirstOrDefault();

                    // Updated by Rajas on 15 JULY 2017 START
                    TimeSpan ts = (Convert.ToDateTime(ODtravelApplicationEdit.ToDate)) - (Convert.ToDateTime(ODtravelApplicationEdit.FromDate));

                    float AppliedDay = ts.Days;
                    if (ODtravelApplicationEdit.ODDayStatus == 1 && ODtravelApplicationEdit.ODDayStatus1 == 1)
                    {
                        AppliedDay = AppliedDay + 1;
                    }

                        //Modified By Shraddha on 30 DEC 2016 for leave application validation changes start
                    else if ((ODtravelApplicationEdit.ODDayStatus == 2) && (ODtravelApplicationEdit.ODDayStatus1 == 2) && ODtravelApplicationEdit.ODDayStatus == ODtravelApplicationEdit.ODDayStatus1)
                    {
                        AppliedDay = 0.5F;
                    }
                    else if ((ODtravelApplicationEdit.ODDayStatus == 3) && (ODtravelApplicationEdit.ODDayStatus1 == 3) && ODtravelApplicationEdit.ODDayStatus == ODtravelApplicationEdit.ODDayStatus1)
                    {
                        AppliedDay = 0.5F;
                    }
                    else if ((ODtravelApplicationEdit.ODDayStatus == 2) && (ODtravelApplicationEdit.ODDayStatus1 == 3) && ODtravelApplicationEdit.ODDayStatus == ODtravelApplicationEdit.ODDayStatus1)
                    {
                        AppliedDay = 1F;
                    }

                    //Modified By Shraddha on 30 DEC 2016 for leave application validation changes End
                    else if ((ODtravelApplicationEdit.ODDayStatus == 2 || ODtravelApplicationEdit.ODDayStatus == 3) && (ODtravelApplicationEdit.ODDayStatus1 == 2 || ODtravelApplicationEdit.ODDayStatus1 == 3)
                        && ODtravelApplicationEdit.ODDayStatus != ODtravelApplicationEdit.ODDayStatus1)
                    {
                        AppliedDay = AppliedDay + 0;
                    }

                    else if (ODtravelApplicationEdit.ODDayStatus == 2 || ODtravelApplicationEdit.ODDayStatus1 == 2 || ODtravelApplicationEdit.ODDayStatus == 3 || ODtravelApplicationEdit.ODDayStatus1 == 3)
                    {
                        AppliedDay = (float)(AppliedDay + 0.5);
                    }
                    // Updated by Rajas on 15 JULY 2017 END

                    ODtravelApplicationObj.AppliedDay = AppliedDay;
                    ODtravelApplicationObj.FromDate = ODtravelApplicationEdit.FromDate;
                    ODtravelApplicationObj.ODDayStatus = ODtravelApplicationEdit.ODDayStatus;
                    ODtravelApplicationObj.ToDate = ODtravelApplicationEdit.ToDate;

                    ODtravelApplicationObj.ODDayStatus1 = ODtravelApplicationEdit.ODDayStatus1; // Added by Rajas on 10 JULY 2017

                    ODtravelApplicationObj.Purpose = ODtravelApplicationEdit.Purpose;
                    ODtravelApplicationObj.Place = ODtravelApplicationEdit.Place;
                    ODtravelApplicationObj.ODTourType = ODtravelApplicationEdit.ODTourType;
                    ODtravelApplicationObj.EmployeeId = EmpId;


                    //Added By Shraddha on 12 DEC 2016 for taking Status along with statusId Start
                    ODtravelApplicationObj.Status = WetosDB.DropdownDatas.Where(a => a.TypeId == 3 && a.Value == ODtravelApplicationEdit.StatusId).Select(a => a.Text).FirstOrDefault();
                    //Added By Shraddha on 12 DEC 2016 for taking Status along with statusId End

                    ODtravelApplicationObj.EffectiveDate = ODtravelApplicationEdit.EffectiveDate; // CODE ADDED BY SHRADDHA ON 17 MAR 2018

                    ODtravelApplicationObj.CustomerName = ODtravelApplicationEdit.CustomerName;
                    ODtravelApplicationObj.CSRNo = ODtravelApplicationEdit.CSRNo;
                    ODtravelApplicationObj.ClientName = ODtravelApplicationEdit.ClientName;
                    ODtravelApplicationObj.PurposeDescription = ODtravelApplicationEdit.PurposeDesc;

                    WetosDB.SaveChanges();

                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 7 SEPTEMBER 2017
                    #region ADD AUDIT LOG
                    string Oldrecord = "ODTourType : " + ODtravelApplicationObjEDIT.ODTourType + ", FromDate : " + ODtravelApplicationObjEDIT.FromDate
                       + ", ToDate : " + ODtravelApplicationObjEDIT.ToDate + ", ODDayStatus : " + ODtravelApplicationObjEDIT.ODDayStatus
                       + ", ODDayStatus1 : " + ODtravelApplicationObjEDIT.ODDayStatus1 + ", AppliedDays :" + ODtravelApplicationObjEDIT.AppliedDay + ", ActualDays :"
                       + ODtravelApplicationObjEDIT.ActualDay + ", Purpose :" + ODtravelApplicationObjEDIT.Purpose + ", Status :" + ODtravelApplicationObjEDIT.Status
                       + ", BranchId :" + ODtravelApplicationObjEDIT.BranchId
                       + ", CompanyId :" + ODtravelApplicationObjEDIT.CompanyId;
                    //---x---------x-----------------x------------x----------------x-----------x-------------x------------x--------------x----------x---------x-----
                    string Newrecord = "ODTourType : " + ODtravelApplicationObj.ODTourType + ", FromDate : " + ODtravelApplicationObj.FromDate
                       + ", ToDate : " + ODtravelApplicationObj.ToDate + ", ODDayStatus : " + ODtravelApplicationObj.ODDayStatus
                       + ", ODDayStatus1 : " + ODtravelApplicationObj.ODDayStatus1 + ", AppliedDays :" + ODtravelApplicationObj.AppliedDay + ", ActualDays :"
                       + ODtravelApplicationObj.ActualDay + ", Purpose :" + ODtravelApplicationObj.Purpose + ", Status :" + ODtravelApplicationObj.Status
                       + ", BranchId :" + ODtravelApplicationObj.BranchId
                       + ", CompanyId :" + ODtravelApplicationObj.CompanyId;

                    //LOGIN ID TAKEN FROM SESSION PERSISTER
                    string Formname = "REQUISITION APPLICATION";
                    //ACTION IS UPDATE
                    string Message = " ";

                    WetosAdministrationController.GenerateAuditLogsUpdate(WetosDB, ODtravelApplicationObj.EmployeeId, Formname, Oldrecord,
                        Newrecord, ref Message);
                    #endregion
                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 7 SEPTEMBER 2017


                    #region ODTour APPLICATION EDIT NOTIFICATION

                    // FOR NOTIFICATION TO BE SENT TO REPORTING PERSON

                    Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == ODtravelApplicationEdit.EmployeeId).FirstOrDefault();
                    Notification NotificationObj = new Notification();
                    NotificationObj.FromID = EmployeeObj.EmployeeId;
                    NotificationObj.ToID = EmployeeObj.EmployeeReportingId;
                    NotificationObj.SendDate = DateTime.Now;
                    NotificationObj.NotificationContent = "Applied ODTour edited by " + EmployeeObj.FirstName + " " + EmployeeObj.LastName + " taken for " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " to " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " is pending for approval";
                    NotificationObj.ReadFlag = false;
                    NotificationObj.SendDate = DateTime.Now;

                    WetosDB.Notifications.AddObject(NotificationObj);

                    WetosDB.SaveChanges();

                    //FOR SELF NOTIFICATION

                    Notification NotificationObj2 = new Notification();
                    NotificationObj2.FromID = EmployeeObj.EmployeeId;
                    NotificationObj2.ToID = EmployeeObj.EmployeeId;
                    NotificationObj2.SendDate = DateTime.Now;
                    NotificationObj2.NotificationContent = "ODTour" + " taken for " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " to " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " edited successfully";
                    NotificationObj2.ReadFlag = false;
                    NotificationObj2.SendDate = DateTime.Now;

                    WetosDB.Notifications.AddObject(NotificationObj2);

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
                                if (SendEmail(EmployeeObj.Email, NotificationObj.NotificationContent, NotificationObj.NotificationContent, ref EmailUpdateStatus, "OD/Travel Application") == false)
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
                    AddAuditTrail("Success - OD application edit : " + ODtravelApplicationEdit.ODTourType + " taken for " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " to " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " edited successfully");  // Updated by Rajas on 17 JAN 2017

                    // Added by Rajas on 17 JAN 2017 START
                    Success("Success - OD application edit : " + ODtravelApplicationEdit.ODTourType + " taken  on " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " to " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " edited successfully");

                    return RedirectToAction("ODTravelIndex");
                }
                else
                {
                    //Modified By Shraddha on 12 DEC 2016 Added Populate Dropdown and ODtravelApplicationEdit object pass start
                    PopulateDropDown();

                    //Modified By Shraddha on 12 DEC 2016 Added Populate Dropdown and ODtravelApplicationEdit object pass End

                    // Added by Rajas on 17 JAN 2017 START
                    AddAuditTrail("Error - Edit OD application failed");  // Updated by Rajas on 17 JAN 2017


                    Error("Error - Edit OD application failed");
                    // Added by Rajas on 17 JAN 2017 END

                    return View(ODtravelApplicationEdit);

                }
            }

            catch (System.Exception ex)
            {
                // Added by Rajas on 17 JAN 2017 START
                AddAuditTrail("Error - Edit OD application failed due to " + ex.Message);  // Updated by Rajas on 17 JAN 2017


                Error("Error - Edit OD application failed due to " + ex.Message);
                // Added by Rajas on 17 JAN 2017 END

                PopulateDropDown();

                return View(ODtravelApplicationEdit);
            }

        }

        /// <summary>
        /// DISPLAY LEAVE ENCASHMENT LIST ON INDEX PAGE ADDED BY SHRADDHA ON 17 DEC 2016
        /// </summary>
        /// <returns></returns>
        public ActionResult EncashmentIndex()
        {
            try
            {
                int EmpId = Convert.ToInt32(Session["EmployeeNo"]);

                #region SP MODIFIED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                List<SP_GetEncashmentList_Result> EncashmentListObj = WetosDB.SP_GetEncashmentList(EmpId).Where(a => a.EmployeeId == EmpId
                    && (a.MarkedAsDelete == null || a.MarkedAsDelete == 0)).ToList();
                #endregion

                AddAuditTrail("Encash list visited");

                return View(EncashmentListObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in Encashment list view due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                Error("Error in viewing list for Encashment");

                return View("Error");
            }
        }

        /// <summary>
        /// APPLY FOR LEAVE ENCASHMENT PAGE ADDED BY SHRADDHA ON 17 DEC 2016
        /// </summary>
        /// <returns></returns>
        public ActionResult EncashmentApplication()
        {
            PopulateDropDown();

            //CODE CHANGED BY SHRADDHA ON 07 FEB 2017 TAKEN FINANCIAL YEAR ID ALONG WITH NAME START
            var FinancialYearObj = WetosDB.FinancialYears.Select(a => new { FinancialId = a.FinancialYearId, FinancialName = a.FinancialName }).ToList();
            ViewBag.FinancialYearList = new SelectList(FinancialYearObj, " FinancialId", "FinancialName").ToList();

            //var FinancialYearObj = WetosDB.FinancialYears.Select(a => a.FinancialName).Distinct().ToList();
            //ViewBag.FinancialYearList = new SelectList(FinancialYearObj, "FinancialName").ToList();

            //CODE CHANGED BY SHRADDHA ON 07 FEB 2017 TAKEN FINANCIAL YEAR ID ALONG WITH NAME END

            //var LeaveCodeObj = WetosDB.LeaveMasters.Select(a => new { LeaveCode = a.LeaveCode, LeaveName = a.LeaveName }).Distinct().ToList();

            //Commented on 26th DEC 2017 by Shalaka
            //Added on 22nd DEC 2017 for getting only allowed encashment leave type-- Start
            //var LeaveCodeObj = WetosDB.LeaveMasters.Where(a => a.EncashmentAllowedOrNot == true).Select(a => new { LeaveCode = a.LeaveCode, LeaveName = a.LeaveName }).Distinct().ToList();
            //Added on 22nd DEC 2017 for getting only allowed encashment leave type-- End

            //ViewBag.LeaveCodeList = new SelectList(LeaveCodeObj, " LeaveCode", "LeaveName").ToList();
            //Commented on 26th DEC 2017 by Shalaka
            return View();
        }

        /// <summary>
        /// APPLY FOR LEAVE ENCASHMENT PAGE ADDED BY SHRADDHA ON 17 DEC 2016
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EncashmentApplication(LeaveEncash LeaveEncashObj, FormCollection fc)
        {
            try
            {
                int EmpId = Convert.ToInt32(Session["EmployeeNo"]);

                //CODE COMMENTED BY SHRADDHA ON 07 FEB 2017 START
                //string FinancialYear = LeaveEncashObj.FinancialYear.ToString();

                //LeaveEncashObj.FinancialYear = WetosDB.FinancialYears.Where(a => a.FinancialName == FinancialYear).FirstOrDefault();
                //CODE COMMENTED BY SHRADDHA ON 07 FEB 2017 END

                //Added on 28th DEC 2017 for getting only allowed encashment leave type with encashable days -- Start
                var LeaveCodeObj = WetosDB.LeaveMasters.Where(a => a.EncashmentAllowedOrNot == true).Select(a => new { LeaveCode = a.LeaveCode, LeaveName = a.LeaveName, NoOfDaysEncashed = a.NoOfDaysEncashed }).FirstOrDefault();

                Double? NoOfEncashableDays = LeaveCodeObj.NoOfDaysEncashed;

                //Added By shalaka on 28th DEC 2017 to Check Encash Value is less than No Of Days to Encashed as per Leave Rule.
                if (NoOfEncashableDays > LeaveEncashObj.EncashValue)
                {
                    //Added by Shalaka on 28th DEC 2017 --- Start
                    Employee EmpObj = WetosDB.Employees.Where(a => a.EmployeeId == EmpId).FirstOrDefault();

                    //Branch of that Employee
                    Branch BranchObj = WetosDB.Branches.Where(a => a.BranchId == EmpObj.BranchId).FirstOrDefault();
                    WetosDB.Company CompanyObj = WetosDB.Companies.Where(a => a.CompanyId == EmpObj.CompanyId).FirstOrDefault();

                    LeaveEncashObj.BranchId = BranchObj.BranchId;
                    LeaveEncashObj.CompanyId = CompanyObj.CompanyId;
                    LeaveEncashObj.EmployeeId = EmpObj.EmployeeId;

                    FinancialYear FinancialYearObj = WetosDB.FinancialYears.Where(a => a.Company.CompanyId == CompanyObj.CompanyId && a.Branch.BranchId == BranchObj.BranchId).FirstOrDefault();
                    LeaveEncashObj.FinancialYearId = FinancialYearObj.FinancialYearId;
                    //Added by Shalaka on 28th DEC 2017 --- End

                    LeaveEncashObj.StatusId = 1;

                    WetosDB.LeaveEncashes.AddObject(LeaveEncashObj);

                    WetosDB.SaveChanges();

                    // NOTIFICATION ADDED BY RAJAS ON 31 DEC 2016
                    #region ENCASHMENT APPLICATION NOTIFICATION

                    // FOR NOTIFICATION TO BE SENT TO REPORTING PERSON

                    Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == LeaveEncashObj.EmployeeId).FirstOrDefault();
                    Notification NotificationObj = new Notification();
                    NotificationObj.FromID = EmployeeObj.EmployeeId;
                    NotificationObj.ToID = EmployeeObj.EmployeeReportingId;
                    NotificationObj.SendDate = DateTime.Now;
                    NotificationObj.NotificationContent = "Leave encashment applied by " + EmployeeObj.FirstName + " " + EmployeeObj.LastName + " is pending for approval";
                    NotificationObj.ReadFlag = false;
                    NotificationObj.SendDate = DateTime.Now;

                    WetosDB.Notifications.AddObject(NotificationObj);

                    WetosDB.SaveChanges();

                    // Check if both reporting person are are different
                    if (EmployeeObj.EmployeeReportingId != EmployeeObj.EmployeeReportingId2)
                    {
                        // FOR NOTIFICATION TO BE SENT TO REPORTING PERSON AS SANCTIONER

                        //Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == LeaveTypeObj.EmployeeId).FirstOrDefault();
                        Notification NotificationObj3 = new Notification();
                        NotificationObj3.FromID = EmployeeObj.EmployeeId;
                        NotificationObj3.ToID = EmployeeObj.EmployeeReportingId;
                        NotificationObj3.SendDate = DateTime.Now;
                        NotificationObj3.NotificationContent = "Leave encashment applied by " + EmployeeObj.FirstName + " " + EmployeeObj.LastName + " is pending for approval";
                        NotificationObj3.ReadFlag = false;
                        NotificationObj3.SendDate = DateTime.Now;

                        WetosDB.Notifications.AddObject(NotificationObj3);

                        WetosDB.SaveChanges();
                    }

                    //FOR SELF NOTIFICATION

                    Notification NotificationObj2 = new Notification();
                    NotificationObj2.FromID = EmployeeObj.EmployeeId;
                    NotificationObj2.ToID = EmployeeObj.EmployeeId;
                    NotificationObj2.SendDate = DateTime.Now;
                    NotificationObj2.NotificationContent = "Leave encashment applied successfully";
                    NotificationObj2.ReadFlag = false;
                    NotificationObj2.SendDate = DateTime.Now;

                    WetosDB.Notifications.AddObject(NotificationObj2);

                    WetosDB.SaveChanges();

                    // NOTIFICATION COUNT
                    int NoTiCount = WetosDB.Notifications.Where(a => a.ToID == EmpId && (a.ReadFlag == false || a.ReadFlag == null)).Count();
                    Session["NotificationCount"] = NoTiCount;

                    #endregion

                    // ADDED BY PUSHKAR FOR AuditLog ON 21 DEC 2016
                    AddAuditTrail("Success - New encashment application created successfully");  // Updated by Rajas on 17 JAN 2017

                    // Added by Rajas on 17 JAN 2017 START
                    Success("Success - New encashment application created successfully");
                }
                else
                {
                    // Added by Shalaka on 28th DEC 2017--- Start
                    AddAuditTrail("Error - Encashment application failed due to not enough Encashable Days.");

                    Error("Error - You have not enough Leave to Encash.");
                    // Added by Shalaka on 28th DEC 2017--- End
                }

                return RedirectToAction("EncashmentIndex");
            }

            catch (System.Exception ex)
            {
                // Added by Rajas on 17 JAN 2017 START
                AddAuditTrail("Exception - New encashment application failed due " + ex.Message);  // Updated by Rajas on 17 JAN 2017

                Error("Exception - New encashment application failed due " + ex.Message);
                // Added by Rajas on 17 JAN 2017 END

                PopulateDropDown();
                return View();
            }
        }

        public ActionResult EncashmentApplicationEdit(int LeaveEncashId)
        {
            LeaveEncash LeaveEncashObj = WetosDB.LeaveEncashes.Where(a => a.LeaveEncashId == LeaveEncashId).FirstOrDefault();
            PopulateDropDown();
            //var FinancialYearObj = WetosDB.FinancialYears.Where(a => a.FinancialId == LeaveEncashObj.FinancialYearId).Select(a => a.FinancialName).Distinct().ToList();
            //ViewBag.FinancialYearList = new SelectList(FinancialYearObj, "FinancialName").ToList();

            return View(LeaveEncashObj);
        }

        [HttpPost]
        public ActionResult EncashmentApplicationEdit(WetosDB.LeaveEncash LeaveEncashEdit)
        {
            try
            {
                int EmpId = Convert.ToInt32(Session["EmployeeNo"]);

                //Added by Shalaka on 26th DEC 2017 because not getting values from view --- Start
                Employee EmpObj = WetosDB.Employees.Where(a => a.EmployeeId == EmpId).FirstOrDefault();

                //Branch of that Employee
                Branch BranchObj = WetosDB.Branches.Where(a => a.BranchId == EmpObj.BranchId).FirstOrDefault();
                WetosDB.Company CompanyObj = WetosDB.Companies.Where(a => a.CompanyId == EmpObj.CompanyId).FirstOrDefault();

                LeaveEncashEdit.BranchId = BranchObj.BranchId;
                LeaveEncashEdit.CompanyId = CompanyObj.CompanyId;
                LeaveEncashEdit.EmployeeId = EmpObj.EmployeeId;

                FinancialYear FinancialYearObj = WetosDB.FinancialYears.Where(a => a.Company.CompanyId == CompanyObj.CompanyId && a.Branch.BranchId == BranchObj.BranchId).FirstOrDefault();
                LeaveEncashEdit.FinancialYearId = FinancialYearObj.FinancialYearId;

                //Modified By Shraddha on 19 DEC 2016 Added if(ModelState.IsValid) Condition instead of try catch block
                //Commented by Shalaka on 28th DEC 2017 Model State validation problem 
                //if (ModelState.IsValid)
                //{
                //Below code Added by Shalaka and above code Commented by Shalaka on 28th DEC 2017
                if (LeaveEncashEdit.BranchId != null && LeaveEncashEdit.CompanyId != null && LeaveEncashEdit.EmployeeId != null && LeaveEncashEdit.LeaveCode != null && LeaveEncashEdit.FinancialYearId != null)
                {
                    WetosDB.LeaveEncash LeaveEncashApplicationObj = WetosDB.LeaveEncashes.Where(b => b.LeaveEncashId == LeaveEncashEdit.LeaveEncashId).FirstOrDefault();
                    LeaveEncashApplicationObj.BranchId = LeaveEncashEdit.BranchId;
                    LeaveEncashApplicationObj.CompanyId = LeaveEncashEdit.CompanyId;
                    LeaveEncashApplicationObj.EmployeeId = LeaveEncashEdit.EmployeeId;
                    //LeaveEncashApplicationObj.FinancialYearId = LeaveEncashEdit.FinancialYearId;
                    LeaveEncashApplicationObj.LeaveCode = LeaveEncashEdit.LeaveCode;
                    LeaveEncashApplicationObj.RejectReason = LeaveEncashEdit.RejectReason;

                    //Modified by Shalaka on 26th DEC 2017 -- Start
                    //LeaveEncashApplicationObj.Status = "1";
                    LeaveEncashApplicationObj.StatusId = 1;
                    //Modified by Shalaka on 26th DEC 2017 -- End

                    LeaveEncashApplicationObj.EncashValue = LeaveEncashEdit.EncashValue;

                    WetosDB.SaveChanges();

                    // NOTIFICATION ADDED BY RAJAS ON 31 DEC 2016
                    // NOTIFICATION ADDED BY RAJAS ON 31 DEC 2016
                    #region EDIT ENCASHMENT APPLICATION NOTIFICATION

                    // FOR NOTIFICATION TO BE SENT TO REPORTING PERSON

                    Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == LeaveEncashEdit.EmployeeId).FirstOrDefault();
                    Notification NotificationObj = new Notification();
                    NotificationObj.FromID = EmployeeObj.EmployeeId;
                    NotificationObj.ToID = EmployeeObj.EmployeeReportingId;
                    NotificationObj.SendDate = DateTime.Now;
                    NotificationObj.NotificationContent = "Leave encashment edited by " + EmployeeObj.FirstName + " " + EmployeeObj.LastName + " is pending for approval";
                    NotificationObj.ReadFlag = false;
                    NotificationObj.SendDate = DateTime.Now;

                    WetosDB.Notifications.AddObject(NotificationObj);

                    WetosDB.SaveChanges();

                    //FOR SELF NOTIFICATION

                    Notification NotificationObj2 = new Notification();
                    NotificationObj2.FromID = EmployeeObj.EmployeeId;
                    NotificationObj2.ToID = EmployeeObj.EmployeeId;
                    NotificationObj2.SendDate = DateTime.Now;
                    NotificationObj2.NotificationContent = "Leave encashment edited successfully";
                    NotificationObj2.ReadFlag = false;
                    NotificationObj2.SendDate = DateTime.Now;

                    WetosDB.Notifications.AddObject(NotificationObj2);

                    WetosDB.SaveChanges();

                    // NOTIFICATION COUNT
                    int NoTiCount = WetosDB.Notifications.Where(a => a.ToID == EmpId && (a.ReadFlag == false || a.ReadFlag == null)).Count();
                    Session["NotificationCount"] = NoTiCount;

                    #endregion

                    // ADDED BY PUSHKAR FOR AuditLog ON 21 DEC 2016
                    AddAuditTrail("Success - Encashment application edited successfully");  // Updated by Rajas on 17 JAN 2017

                    // Added by Rajas on 17 JAN 2017 START
                    Success("Success - Encashment application edited successfully");

                    return RedirectToAction("EncashmentIndex");
                }
                else
                {
                    //Modified By Shraddha on 19 DEC 2016 Added Populate Dropdown and LeaveEncashEdit object pass start
                    PopulateDropDown();

                    //Modified By Shraddha on 19 DEC 2016 Added Populate Dropdown and LeaveEncashEdit object pass End

                    // Added by Rajas on 17 JAN 2017 START
                    AddAuditTrail("Error - Encashment application edit failed");  // Updated by Rajas on 17 JAN 2017

                    Error("Error - Encashment application edit failed");

                    // Added by Rajas on 17 JAN 2017 END

                    return View(LeaveEncashEdit);
                }
            }

            catch (System.Exception ex)
            {
                PopulateDropDown();

                // Added by Rajas on 17 JAN 2017 START
                AddAuditTrail("Error - Encashment application edit faileddue to " + ex.Message);  // Updated by Rajas on 17 JAN 2017

                Error("Error - Encashment application edit failed due to " + ex.Message);

                // Added by Rajas on 17 JAN 2017 END

                return View(LeaveEncashEdit);
            }

        }


        /// <summary>
        /// Added By Shraddha on 19 DEC 2016 For Deleting Entry From database
        /// </summary>
        /// <param name="LeaveEncashId"></param>
        /// <returns></returns>
        public ActionResult EncashmentApplicationDelete(int LeaveEncashId)
        {
            LeaveEncash LeaveEncashObj = WetosDB.LeaveEncashes.Where(a => a.LeaveEncashId == LeaveEncashId).FirstOrDefault();
            PopulateDropDown();

            // var FinancialYearObj = WetosDB.FinancialYears.Where(a => a.FinancialId == LeaveEncashObj.FinancialYearId).Select(a => a.FinancialName).Distinct().ToList();
            // ViewBag.FinancialYearList = new SelectList(FinancialYearObj, "FinancialName").ToList();

            //Commented by Shalaka on 28th DEC 2017  --- Start
            //var LeaveCodeObj = WetosDB.LeaveMasters.Select(a => new { LeaveCode = a.LeaveCode, LeaveName = a.LeaveName }).Distinct().ToList();
            //ViewBag.LeaveCodeList = new SelectList(LeaveCodeObj, " LeaveCode", "LeaveName").ToList();
            //Commented by Shalaka on 28th DEC 2017  --- End

            return View(LeaveEncashObj);
        }


        [HttpPost]
        public ActionResult EncashmentApplicationDelete(WetosDB.LeaveEncash LeaveEncashDeleteObj)
        {
            LeaveEncash LeaveEncashObj = WetosDB.LeaveEncashes.Where(a => a.LeaveEncashId == LeaveEncashDeleteObj.LeaveEncashId).FirstOrDefault();
            WetosDB.LeaveEncashes.DeleteObject(LeaveEncashObj);
            WetosDB.SaveChanges();


            // ADDED BY PUSHKAR FOR AuditLog ON 21 DEC 2016
            AddAuditTrail("Encashmant Application Delete");


            return RedirectToAction("EncashmentIndex");
        }

        /// <summary>
        /// Json return for to get Branch dropdown list on basis of company selection
        /// Added by Rajas on 27 DEC 2016
        /// </summary>
        /// <returns></returns>
        public JsonResult GetBranch(string Companyid)
        {

            int SelCompanyId = 0;
            if (!string.IsNullOrEmpty(Companyid))
            {
                if (Companyid.ToUpper() != "NULL")
                {
                    SelCompanyId = Convert.ToInt32(Companyid);
                }
            }

            #region CODE FOR GET BRANCH LIST ADDED BY SHRADDHA ON 30 MAR 2018
            int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
            //List<Branch> BranchList = WetosDB.Branches.Where(a => a.Company.CompanyId == SelCompanyId).ToList();
            var BranchList = WetosDB.SP_GetBranchList(EmployeeId).Where(a => a.CompanyId == SelCompanyId).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList(); //.Select(m => new { BranchId = m.BranchId, BranchName = m.BranchName}).ToList();
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
            #endregion
            return Json(BranchList);
        }

        /// <summary>
        /// Json return for to get Employee dropdown list on basis of Branch selection
        /// Added by Rajas on 27 DEC 2016
        /// </summary>
        /// <returns></returns>
        public JsonResult GetEmployee(string Branchid)
        {

            int SelBranchId = 0;
            if (!string.IsNullOrEmpty(Branchid))
            {
                if (Branchid.ToUpper() != "NULL")
                {
                    SelBranchId = Convert.ToInt32(Branchid);
                }
            }

            DateTime Leavingdate = Convert.ToDateTime("01/01/1900");  // Added by Rajas on 10 MARCH 2017

            // Updated by Rajas to pass active employee list, on 10 MARCH 2017

            #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
            int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            //var EmployeeList = WetosDB.Employees.Where(a => a.BranchId == SelBranchId && a.ActiveFlag == true).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList(); //Removed leaving date condition and taken active flag condition by shraddha on 09 SEP 2017
            var EmployeeList = WetosDB.SP_VwActiveEmployee(EmployeeId).Where(a => a.BranchId == SelBranchId && a.ActiveFlag == true).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList(); //Removed leaving date condition and taken active flag condition by shraddha on 09 SEP 2017
            #endregion
            return Json(EmployeeList);
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
                AddAuditTrail("Error in sending email in " + EmailFromWhichApplication + " : " + " due to " + ex.Message 
                    + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                UpdateStatus = "Unable to send email!";

                return ReturnStatus;

                //return View("Error");
            }

            // SEND EMAIL END

            //return View();
        }


        /// <summary>
        /// CODE ADDED BY SHRADDHA ON 15 JAN 2018 FOR GETTING LOGIN AND LOGOUT TIME FOR SELECTED SHIFT
        /// </summary>
        /// <param name="ShiftCode"></param>
        /// <param name="CompanyId"></param>
        /// <param name="BranchId"></param>
        /// <returns></returns>
        public JsonResult GetLoginLogoutTimeForSelectedShift(string ShiftCode, int CompanyId, int BranchId)
        {
            string LoginTime = "00:00";
            string LogOutTime = "00:00";
            string[] LoginLogoutTime = new string[2];
            Shift ShiftObj = WetosDB.Shifts.Where(a => a.ShiftCode == ShiftCode.Trim() && a.Company.CompanyId == CompanyId && a.BranchId == BranchId && (a.MarkedAsDelete == null || a.MarkedAsDelete == 0)).FirstOrDefault(); //ADDED MARKED AS DELETE CONDITION BY SHRADDHA ON 02 FEB 2018
            if (ShiftObj != null)
            {
                string LoginHour = ShiftObj.FirstInTime.Hour.ToString("D2");
                string LoginMinutes = ShiftObj.FirstInTime.Minute.ToString("D2");

                string LogOutHour = ShiftObj.FirstOutTime.Hour.ToString("D2");
                string LogOutMinutes = ShiftObj.FirstOutTime.Minute.ToString("D2");

                LoginTime = LoginHour + ":" + LoginMinutes;
                LogOutTime = LogOutHour + ":" + LogOutMinutes;


                LoginLogoutTime[0] = LoginTime;
                LoginLogoutTime[1] = LogOutTime;
            }
            return Json(LoginLogoutTime, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ODLateEarlyApplication(bool MySelf)
        {
            // Try catch block added by Rajas on 14 FEB 2017
            try
            {
                ODTravelModel ODTourObj = new ODTravelModel();

                ODTourObj.MySelf = MySelf;
                ODTourObj.EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);   // Added by Rajas on 27 SEP 2017

                PopulateDropDown();

                // Assigning initial pending status
                ODTourObj.StatusId = 1;

                return View(ODTourObj);
            }

            catch (System.Exception ex)
            {
                PopulateDropDown();

                AddAuditTrail("Exception - Late/Early application failed due to " + ex.Message);

                Error("Error - Late/Early application failed");

                return View();
            }
        }


        [HttpPost]
        public ActionResult ODLateEarlyApplication(ODTravelModel ODTourObj, FormCollection Collection)
        {
            try
            {
                int EmpId = ODTourObj.EmployeeId;

                //CODE ADDED BY SHRADDHA ON 30 JAN 2018 START
                if (string.IsNullOrEmpty(ODTourObj.ODTourType))
                {
                    ModelState.AddModelError("", "Please select Proper requisition type");

                    PopulateDropDown();

                    return View(ODTourObj);
                }
                else
                {
                    if (ODTourObj.ODTourType == "Late Reporting" || ODTourObj.ODTourType == "Early Departure")
                    {
                        ODTourObj.FromDate = ODTourObj.ODDate;
                        ODTourObj.ToDate = ODTourObj.ODDate;
                        ODTourObj.ODDayStatus1 = ODTourObj.ODDayStatus2;
                        ODTourObj.ODDayStatus = ODTourObj.ODDayStatus2;
                        if (ODTourObj.ODDate == null)
                        {
                            ModelState.AddModelError("", "Please enter proper Late/Early Date");

                            PopulateDropDown();

                            return View(ODTourObj);
                        }

                    }
                }
                //CODE ADDED BY SHRADDHA ON 30 JAN 2018 END


                //Added By Shraddha on 12 DEC 2016 for adding date validation start
                if (ODTourObj.FromDate > ODTourObj.ToDate)
                {
                    ModelState.AddModelError("", "End Date should be Greater than or equal to From Date");

                    PopulateDropDown();

                    return View(ODTourObj);

                }
                //Added By Shraddha on 12 DEC 2016 for adding date validation End

                // Updated by Rajas on 15 JULY 2017
                if (ODTourObj.FromDate == ODTourObj.ToDate)
                {
                    if (ODTourObj.ODDayStatus != ODTourObj.ODDayStatus1)
                    {
                        // ERROR
                        ModelState.AddModelError("", "Select Proper day status");

                        // FILL VIEW BAG AGAIN
                        PopulateDropDown();

                        return View(ODTourObj);
                    }
                }
                else
                {
                    //if (ODTourObj.ODDayStatus == "1") // NEED DISCUSSION
                    //{
                    //    // ERROR
                    //    ModelState.AddModelError("", "Select Proper status for from date");
                    //    PopulateDropDown();
                    //    LeaveApplicationNecessaryContent(EmpId);
                    //    return View(ODTourObj);
                    //}

                }

                /// Validation for Punch relaxed in case of OD 
                /// Updated by Rajas on 17 MAY 2017
                /// As per meeting discussion on 17 MAY 2017 with Katre sir, Kulkarni sir, Kirti madam, Madhav sir and Rajas
                #region PUNCH VALIDATION
                // Check if Punch is available for selected employee
                // Added by Rajas on 10 MAY 2017 START
                //DailyTransaction DailyTransactionObj = WetosDB.DailyTransactions.Where(a => a.TranDate >= LeaveTypeObj.FromDate && a.TranDate <= LeaveTypeObj.ToDate
                //    && a.EmployeeId == EmpId && (a.Status == "AAAA" || a.Status == "AAPP" || a.Status == "PPAA")).FirstOrDefault();

                // Updated by Rajas on 17 MAY 2017
                //DailyTransaction DailyTransactionObj = WetosDB.DailyTransactions.Where(a => a.TranDate >= ODTourObj.FromDate && a.TranDate <= ODTourObj.ToDate
                //   && a.EmployeeId == EmpId).FirstOrDefault();

                //if (DailyTransactionObj != null)
                //{
                //    // Added by Rajas on 18 MAY 2017 
                //    TimeSpan TransTime = DailyTransactionObj.TranDate.TimeOfDay;

                //    TimeSpan LoginTime = DailyTransactionObj.Login.TimeOfDay;

                //    if (TransTime != LoginTime)
                //    {
                //        ModelState.AddModelError("", "You can't apply leave as your punch is already available for selected date range");

                //        // Added by Rajas on 30 MARCH 2017 START
                //        string UpdateStatus = string.Empty;

                //        if (LeaveApplicationNecessaryContent(EmpId, ref UpdateStatus) == false)
                //        {
                //            AddAuditTrail("Error in Leave application due to " + UpdateStatus);

                //            Error(UpdateStatus);

                //            return RedirectToAction("LeaveApplicationIndex");
                //        }

                //        PopulateDropDown();

                //        return View(ODTourObj);
                //    }
                //}
                // Added by Rajas on 10 MAY 2017 END
                #endregion


                #region VALIDATE OD DATA

                // Updated by Rajas on 27 SEP 2017 START
                // Fixed Issue No. 6 in Flagship Defect Sheet raised by Sandeep sir
                LeaveApplication LeaveApplicationObj = WetosDB.LeaveApplications.Where(a => a.EmployeeId == EmpId
                    && (a.StatusId == 1 || a.StatusId == 2 || a.StatusId == 4) && a.MarkedAsDelete == 0
                    && ((a.FromDate == ODTourObj.FromDate) // && a.ToDate > LeaveTypeObj.ToDate) 
                             || (a.FromDate < ODTourObj.FromDate && a.ToDate == ODTourObj.ToDate)
                             || (a.FromDate < ODTourObj.FromDate && a.ToDate > ODTourObj.ToDate)
                             || (a.FromDate > ODTourObj.FromDate && a.ToDate < ODTourObj.ToDate)
                             || (a.FromDate == ODTourObj.FromDate && a.ToDate == ODTourObj.ToDate)
                             || (a.FromDate > ODTourObj.FromDate && a.FromDate < ODTourObj.ToDate) // ADDED BY MSJ ON 11 JAN 2018
                             || (a.ToDate > ODTourObj.FromDate && a.ToDate < ODTourObj.ToDate) // ADDED BY MSJ ON 11 JAN 2018
                             || (a.ToDate == ODTourObj.FromDate))
                    ).FirstOrDefault();// FROM DATE AND TO DATE CONDITION ADDED BY SHRADDHA WITH HELP OF MSJ ON 01 FEB 2018

                // Added by Rajas on 10 MARCH 2017 for validation to check if already pending or sanctioned CO is applied for same date
                CompOffApplication CompoffApplicationObj = WetosDB.CompOffApplications.Where(a => a.EmployeeId == EmpId
                    && (a.StatusId == 1 || a.StatusId == 2 || a.StatusId == 4)
                    && a.MarkedAsDelete == 0
                    && (
                             (a.FromDate == ODTourObj.FromDate) // && a.ToDate > LeaveTypeObj.ToDate) 
                             || (a.FromDate < ODTourObj.FromDate && a.ToDate == ODTourObj.ToDate)
                             || (a.FromDate < ODTourObj.FromDate && a.ToDate > ODTourObj.ToDate)
                             || (a.FromDate > ODTourObj.FromDate && a.ToDate < ODTourObj.ToDate)
                             || (a.FromDate == ODTourObj.FromDate && a.ToDate == ODTourObj.ToDate)
                             || (a.FromDate > ODTourObj.FromDate && a.FromDate < ODTourObj.ToDate) // ADDED BY MSJ ON 11 JAN 2018
                             || (a.ToDate > ODTourObj.FromDate && a.ToDate < ODTourObj.ToDate) // ADDED BY MSJ ON 11 JAN 2018
                             || (a.ToDate == ODTourObj.FromDate))  // ADDED BY MSJ ON 11 JAN 2018
                    ).FirstOrDefault();// FROM DATE AND TO DATE CONDITION ADDED BY SHRADDHA WITH HELP OF MSJ ON 01 FEB 2018

                // MODIFIED BY MSJ ON 09 JAN 2018 START COMMENTED EXISTING FUNCTION AND ADDED NEW FUNCTION
                //ODTour ODTourTblObj = WetosDB.ODTours.Where(a => a.EmployeeId == EmpId && (a.StatusId == 1 || a.StatusId == 2 || a.StatusId == 4)
                //                && a.MarkedAsDelete == 0
                //                && a.FromDate <= ODTourObj.FromDate && a.ToDate >= ODTourObj.ToDate).FirstOrDefault();

                ODTour ODTourTblObj = WetosDB.ODTours.Where(a => a.EmployeeId == EmpId
                             && (a.StatusId == 1 || a.StatusId == 2 || a.StatusId == 4)
                             && a.MarkedAsDelete == 0
                             && (
                             (a.FromDate == ODTourObj.FromDate) // && a.ToDate > LeaveTypeObj.ToDate) 
                             || (a.FromDate < ODTourObj.FromDate && a.ToDate == ODTourObj.ToDate)
                             || (a.FromDate < ODTourObj.FromDate && a.ToDate > ODTourObj.ToDate)
                             || (a.FromDate > ODTourObj.FromDate && a.ToDate < ODTourObj.ToDate)
                             || (a.FromDate == ODTourObj.FromDate && a.ToDate == ODTourObj.ToDate)
                             || (a.FromDate > ODTourObj.FromDate && a.FromDate < ODTourObj.ToDate) // ADDED BY MSJ ON 11 JAN 2018
                             || (a.ToDate > ODTourObj.FromDate && a.ToDate < ODTourObj.ToDate) // ADDED BY MSJ ON 11 JAN 2018
                             || (a.ToDate == ODTourObj.FromDate)  // ADDED BY MSJ ON 11 JAN 2018
                             )).FirstOrDefault();

                // MODIFIED BY MSJ ON 09 JAN 2018 END

                // Updated by Rajas on 27 SEP 2017 END

                for (DateTime CurrentDate = Convert.ToDateTime(ODTourObj.FromDate); CurrentDate.Date <= ODTourObj.ToDate; CurrentDate = CurrentDate.AddDays(1))
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
                                    ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned Late/Early for this date range.");

                                    PopulateDropDown();

                                    return View(ODTourObj);
                                }
                                else
                                {
                                    if (LeaveApplicationObj.FromDayStatus == ODTourObj.ODDayStatus)
                                    {
                                        ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned Late/Early for this date range.");

                                        PopulateDropDown();

                                        return View(ODTourObj);
                                    }
                                    else
                                    {
                                        if (ODTourObj.ODDayStatus == 1)
                                        {
                                            ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned Late/Early for this date range.");

                                            PopulateDropDown();

                                            return View(ODTourObj);
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
                                    ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned Late/Early for this date range.");

                                    PopulateDropDown();

                                    return View(ODTourObj);
                                }
                                else
                                {
                                    if (LeaveApplicationObj.FromDayStatus == ODTourObj.ODDayStatus)
                                    {
                                        ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned Late/Early for this date range.");

                                        PopulateDropDown();

                                        return View(ODTourObj);
                                    }
                                    else
                                    {
                                        if (ODTourObj.ODDayStatus == 1)
                                        {
                                            ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned Late/Early for this date range.");

                                            PopulateDropDown();

                                            return View(ODTourObj);
                                        }
                                    }
                                }
                            }
                            else if (CurrentDate == LeaveApplicationObj.ToDate)
                            {
                                if (LeaveApplicationObj.ToDayStatus == 1)  // Full day in single day
                                {
                                    ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned Late/Early for this date range.");

                                    PopulateDropDown();

                                    return View(ODTourObj);
                                }
                                else
                                {
                                    if (LeaveApplicationObj.FromDayStatus == ODTourObj.ODDayStatus)
                                    {
                                        ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned Late/Early for this date range.");

                                        PopulateDropDown();

                                        return View(ODTourObj);
                                    }
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned Late/Early for this date range.");

                                PopulateDropDown();

                                return View(ODTourObj);
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
                                    ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned Late/Early for this date range.");

                                    PopulateDropDown();

                                    return View(ODTourObj);
                                }
                                else
                                {
                                    if (CompoffApplicationObj.FromDateStatus == ODTourObj.ODDayStatus)
                                    {
                                        ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned Late/Early for this date range.");

                                        PopulateDropDown();

                                        return View(ODTourObj);
                                    }
                                    else
                                    {
                                        if (ODTourObj.ODDayStatus == 1)
                                        {
                                            ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned Late/Early for this date range.");

                                            PopulateDropDown();

                                            return View(ODTourObj);
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
                                    ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned Late/Early for this date range.");

                                    PopulateDropDown();

                                    return View(ODTourObj);
                                }
                                else
                                {
                                    if (CompoffApplicationObj.FromDateStatus == ODTourObj.ODDayStatus)
                                    {
                                        ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned Late/Early for this date range.");

                                        PopulateDropDown();

                                        return View(ODTourObj);
                                    }
                                    else
                                    {
                                        if (ODTourObj.ODDayStatus == 1)
                                        {
                                            ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned Late/Early for this date range.");

                                            PopulateDropDown();

                                            return View(ODTourObj);
                                        }
                                    }
                                }
                            }
                            else if (CurrentDate == CompoffApplicationObj.ToDate)
                            {
                                if (CompoffApplicationObj.ToDateStatus == 1)  // Full day in single day
                                {
                                    ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned Late/Early for this date range.");

                                    PopulateDropDown();

                                    return View(ODTourObj);
                                }
                                else
                                {
                                    if (CompoffApplicationObj.FromDateStatus == ODTourObj.ODDayStatus)
                                    {
                                        ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned Late/Early for this date range.");

                                        PopulateDropDown();

                                        return View(ODTourObj);
                                    }
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned Late/Early for this date range.");

                                PopulateDropDown();

                                return View(ODTourObj);
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
                        //COMMENTED BY SHRADDHA AND ADDED NEW LINE BY SHRADDHA ON 28 OCT 2017 START
                        //if (CompoffApplicationObj.FromDate == CompoffApplicationObj.ToDate) // Single day
                        if (ODTourTblObj.FromDate == ODTourTblObj.ToDate)
                        //COMMENTED BY SHRADDHA AND ADDED NEW LINE BY SHRADDHA ON 28 OCT 2017 END
                        {
                            if (CurrentDate == ODTourTblObj.FromDate)
                            {
                                //COMMENTED BY SHRADDHA AND ADDED NEW LINE BY SHRADDHA ON 28 OCT 2017 START
                                // if (CompoffApplicationObj.FromDateStatus == 1)  // Full day in single day
                                if (ODTourTblObj.ODDayStatus == 1)
                                //COMMENTED BY SHRADDHA AND ADDED NEW LINE BY SHRADDHA ON 28 OCT 2017 END
                                {
                                    ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned Late/Early for this date range.");

                                    PopulateDropDown();

                                    return View(ODTourObj);
                                }
                                else
                                {
                                    if (ODTourTblObj.ODDayStatus == ODTourObj.ODDayStatus)
                                    {
                                        ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned Late/Early for this date range.");

                                        PopulateDropDown();

                                        return View(ODTourObj);
                                    }
                                    else
                                    {
                                        if (ODTourObj.ODDayStatus == 1)
                                        {
                                            ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned Late/Early for this date range.");

                                            PopulateDropDown();

                                            return View(ODTourObj);
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
                                    ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned Late/Early for this date range.");

                                    PopulateDropDown();

                                    return View(ODTourObj);
                                }
                                else
                                {
                                    if (ODTourTblObj.ODDayStatus == ODTourObj.ODDayStatus)
                                    {
                                        ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned Late/Early for this date range.");

                                        PopulateDropDown();

                                        return View(ODTourObj);
                                    }
                                    else
                                    {
                                        if (ODTourObj.ODDayStatus == 1)
                                        {
                                            ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned Late/Early for this date range.");

                                            PopulateDropDown();

                                            return View(ODTourObj);
                                        }
                                    }
                                }
                            }
                            else if (CurrentDate == ODTourTblObj.ToDate)
                            {
                                if (ODTourTblObj.ODDayStatus == 1)  // Full day in single day
                                {
                                    ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned Late/Early for this date range.");

                                    PopulateDropDown();

                                    return View(ODTourObj);
                                }
                                else
                                {
                                    if (ODTourTblObj.ODDayStatus == ODTourObj.ODDayStatus)
                                    {
                                        ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned Late/Early for this date range.");

                                        PopulateDropDown();

                                        return View(ODTourObj);
                                    }
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODTourObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned Late/Early for this date range.");

                                PopulateDropDown();

                                return View(ODTourObj);
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
                DailyTransaction DailyTransactionListObj = WetosDB.DailyTransactions.Where(a => a.TranDate >= ODTourObj.FromDate && a.TranDate <= ODTourObj.ToDate && a.EmployeeId == EmpId && a.Lock == "Y").FirstOrDefault();

                if (DailyTransactionListObj != null)
                {
                    ModelState.AddModelError("", "You Can not apply Late/Early for this range as Data is Locked");

                    PopulateDropDown();

                    return View(ODTourObj);
                }
                // Added by Rajas on 10 MARCH 2017 to Validate if data is locked, can not apply leave END
                #endregion

                #endregion

                //if (ModelState.IsValid) //IF ELSE CONDITION COMMENTED BY SHRADDHA ON 30 JAN 2018
                {
                    //TODO : Add insert logic here

                    // Calculate applied days
                    // Updated by Rajas on 15 JULY 2017 START
                    TimeSpan ts = (Convert.ToDateTime(ODTourObj.ToDate)) - (Convert.ToDateTime(ODTourObj.FromDate));

                    float AppliedDay = ts.Days;
                    if (ODTourObj.ODDayStatus == 1 && ODTourObj.ODDayStatus1 == 1)
                    {
                        AppliedDay = AppliedDay + 1;
                    }

                        //Modified By Shraddha on 30 DEC 2016 for leave application validation changes start
                    else if ((ODTourObj.ODDayStatus == 2) && (ODTourObj.ODDayStatus1 == 2) && ODTourObj.ODDayStatus == ODTourObj.ODDayStatus1)
                    {
                        AppliedDay = 0.5F;
                    }
                    else if ((ODTourObj.ODDayStatus == 3) && (ODTourObj.ODDayStatus1 == 3) && ODTourObj.ODDayStatus == ODTourObj.ODDayStatus1)
                    {
                        AppliedDay = 0.5F;
                    }
                    else if ((ODTourObj.ODDayStatus == 2) && (ODTourObj.ODDayStatus1 == 3) && ODTourObj.ODDayStatus == ODTourObj.ODDayStatus1)
                    {
                        AppliedDay = 1F;
                    }

                    //Modified By Shraddha on 30 DEC 2016 for leave application validation changes End
                    else if ((ODTourObj.ODDayStatus == 2 || ODTourObj.ODDayStatus == 3) && (ODTourObj.ODDayStatus1 == 2 || ODTourObj.ODDayStatus1 == 3) && ODTourObj.ODDayStatus != ODTourObj.ODDayStatus1)
                    {
                        AppliedDay = AppliedDay + 0;
                    }

                    else if (ODTourObj.ODDayStatus == 2 || ODTourObj.ODDayStatus1 == 2 || ODTourObj.ODDayStatus == 3 || ODTourObj.ODDayStatus1 == 3)
                    {
                        AppliedDay = (float)(AppliedDay + 0.5);
                    }
                    // Updated by Rajas on 15 JULY 2017 END

                    ODTourObj.AppliedDay = AppliedDay;

                    ODLateEarly ODTOURObj = new ODLateEarly();

                    ODTOURObj.ActualDay = ODTourObj.ActualDay;

                    ODTOURObj.AppliedDay = ODTourObj.AppliedDay;

                    //CODE CHANGED BY SHRADDHA ON 13 FEB 2017 FOR TAKING COMPANYID, BRANCHID AND EMPLOYEEID OF LOGIN EMPLOYEE START
                    Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == EmpId).FirstOrDefault();

                    // Handle if EmployeeObj is NULL
                    // Updated by Rajas on 27 SEP 2017 START
                    ODTOURObj.BranchId = EmployeeObj == null ? 0 : EmployeeObj.BranchId;

                    ODTOURObj.CompanyId = EmployeeObj == null ? 0 : EmployeeObj.CompanyId;
                    // Updated by Rajas on 27 SEP 2017 END

                    ODTOURObj.EmployeeId = EmpId;

                    //CODE CHANGED BY SHRADDHA ON 13 FEB 2017 FOR TAKING COMPANYID, BRANCHID AND EMPLOYEEID OF LOGIN EMPLOYEE END

                    ODTOURObj.FromDate = Convert.ToDateTime(ODTourObj.FromDate);

                    ODTOURObj.JourneyType = ODTourObj.JourneyType;

                    ODTOURObj.ODDayStatus = ODTourObj.ODDayStatus;

                    ODTOURObj.ODDayStatus1 = ODTourObj.ODDayStatus1; // Added by Rajas on 10 JULY 2017

                    ODTOURObj.OdTourHeadCode = ODTourObj.OdTourHeadCode;

                    ODTOURObj.ODTourId = ODTourObj.ODTourId;

                    ODTOURObj.ODTourType = ODTourObj.ODTourType;

                    ODTOURObj.RejectReason = ODTourObj.RejectReason;

                    ODTOURObj.Place = ODTourObj.Place;

                    ODTOURObj.Purpose = ODTourObj.Purpose;

                    ODTOURObj.ToDate = Convert.ToDateTime(ODTourObj.ToDate);

                    ODTOURObj.StatusId = ODTourObj.StatusId;

                    ODTOURObj.TransportType = ODTourObj.TransportType;

                    //Added By Shraddha on 12 DEC 2016 for taking Status along with statusId Start
                    ODTOURObj.Status = WetosDB.DropdownDatas.Where(a => a.TypeId == 3 && a.Value == ODTourObj.StatusId).Select(a => a.Text).FirstOrDefault();
                    //Added By Shraddha on 12 DEC 2016 for taking Status along with statusId End


                    ODTOURObj.EffectiveDate = ODTourObj.EffectiveDate; // CODE ADDED BY SHRADDHA ON 17 MAR 2018

                    ODTOURObj.CustomerName = ODTourObj.CustomerName;
                    ODTOURObj.CSRNo = ODTourObj.CSRNo;
                    ODTOURObj.ClientName = ODTourObj.ClientName;
                    ODTOURObj.PurposeDescription = ODTourObj.PurposeDesc;


                    WetosDB.ODLateEarlies.AddObject(ODTOURObj);

                    WetosDB.SaveChanges();

                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 7 SEPTEMBER 2017
                    #region ADD AUDIT LOG
                    //OLD RECORD IS BLANK
                    string Newrecord = "ODTourType : " + ODTOURObj.ODTourType + ", FromDate : " + ODTOURObj.FromDate
                       + ", ToDate : " + ODTOURObj.ToDate + ", ODDayStatus : " + ODTOURObj.ODDayStatus
                       + ", ODDayStatus1 : " + ODTOURObj.ODDayStatus1 + ", AppliedDays :" + ODTOURObj.AppliedDay + ", ActualDays :"
                       + ODTOURObj.ActualDay + ", Purpose :" + ODTOURObj.Purpose + ", Status :" + ODTOURObj.Status
                       + ", BranchId :" + ODTOURObj.BranchId
                       + ", CompanyId :" + ODTOURObj.CompanyId;

                    //LOGIN ID IS TAKEN FROM SESSION PERSISTER
                    string Formname = "Late/Early APPLICATION";
                    //ACTION IS UPDATE
                    string Message = " ";

                    WetosAdministrationController.GenerateAuditLogsInsert(WetosDB, ODTOURObj.EmployeeId, Formname, Newrecord, ref Message);
                    #endregion
                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 7 SEPTEMBER 2017


                    // NOTIFICATION ADDED BY RAJAS ON 31 DEC 2016
                    #region OD APPLICATION NOTIFICATION

                    // FOR NOTIFICATION TO BE SENT TO REPORTING PERSON

                    //CODE COMMENTED BY SHRADDHA ON 13 FEB 2017 BECAUSE SAME CODE EXISTS ABOVE START
                    //Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == ODTOURObj.EmployeeId).FirstOrDefault();
                    //CODE COMMENTED BY SHRADDHA ON 13 FEB 2017 BECAUSE SAME CODE EXISTS ABOVE END

                    Notification NotificationObj = new Notification();
                    NotificationObj.FromID = EmployeeObj.EmployeeId;
                    NotificationObj.ToID = EmployeeObj.EmployeeReportingId;
                    NotificationObj.SendDate = DateTime.Now;

                    //NOTIFICATION CHANGED BY SHRADDHA ON 13 FEB 2017 FROM OD TOUR TO OD TRAVEL START
                    //NotificationObj.NotificationContent = "OD Travel applied by " + EmployeeObj.FirstName + " " + EmployeeObj.LastName + " for " + ODTOURObj.FromDate.Value.ToString("dd-MMM-yyyy") + " to " + ODTOURObj.ToDate.Value.ToString("dd-MMM-yyyy") + " is pending for approval";
                    NotificationObj.NotificationContent = "Received Late/Early application for approval from " + EmployeeObj.FirstName + " " + EmployeeObj.LastName;
                    //NOTIFICATION CHANGED BY SHRADDHA ON 13 FEB 2017 FROM OD TOUR TO OD TRAVEL END

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
                                if (SendEmail(EmployeeObj.Email, NotificationObj.NotificationContent, NotificationObj.NotificationContent, ref EmailUpdateStatus, "OD/Travel Application") == false)
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
                    AddAuditTrail("Success - New Late/Early application : " + ODTourObj.ODTourType + " taken on " + ODTourObj.FromDate.Value.ToString("dd-MMM-yyyy") + " created successfully"); //Modified by Pushkar on 4 FEB 2017 as datetime in model was updated // Updated by Rajas on 17 JAN 2017

                    // Added by Rajas on 17 JAN 2017 START
                    Success("Late/Early application for " + EmployeeObj.FirstName + " " + EmployeeObj.LastName + " successfully created");  //Modified by Pushkar on 4 FEB 2017 as datetime in model was updated

                    return RedirectToAction("ODLateEarlyIndex");
                }
                //else //IF ELSE CONDITION COMMENTED BY SHRADDHA ON 30 JAN 2018
                //{
                //    // ADDED BY PUSHKAR FOR AuditLog ON 21 DEC 2016
                //    AddAuditTrail("Error - New OD application failed");  // Updated by Rajas on 17 JAN 2017

                //    // Added by Rajas on 17 JAN 2017 START
                //    Error("Error - New OD application failed");

                //    PopulateDropDown();
                //    return View(ODTourObj);
                //}
            }

            catch (System.Exception ex)
            {
                // ADDED BY PUSHKAR FOR AuditLog ON 21 DEC 2016
                AddAuditTrail("Error - New Late/Early application failed due to" + ex.Message);  // Updated by Rajas on 17 JAN 2017

                // Added by Rajas on 17 JAN 2017 START
                Error("Error - New Late/Early application failed due to" + ex.Message);

                PopulateDropDown();
                return View(ODTourObj);
            }
        }


        public ActionResult ODLateEarlyEdit(int id)
        {
            try
            {
                WetosDB.ODLateEarly ODtravelApplicationEdit = WetosDB.ODLateEarlies.Single(a => a.ODTourId == id);

                ODTravelModel ODTravelModelObj = new ODTravelModel();

                ODTravelModelObj.FromDate = ODtravelApplicationEdit.FromDate;

                ODTravelModelObj.ToDate = ODtravelApplicationEdit.ToDate;

                ODTravelModelObj.ODDayStatus = ODtravelApplicationEdit.ODDayStatus.Value;

                ODTravelModelObj.ODDayStatus1 = ODtravelApplicationEdit.ODDayStatus1.Value;

                ODTravelModelObj.EmployeeId = ODtravelApplicationEdit.EmployeeId;

                ODTravelModelObj.ODTourId = ODtravelApplicationEdit.ODTourId;

                ODTravelModelObj.Purpose = ODtravelApplicationEdit.Purpose;

                ODTravelModelObj.Place = ODtravelApplicationEdit.Place;

                // Trim ODTourType status before sending on view
                // Added by Rajas on 15 JULY 2017
                ODTravelModelObj.ODTourType = ODtravelApplicationEdit.ODTourType == null ? string.Empty : ODtravelApplicationEdit.ODTourType.Trim();


                //CODE ADDED BY SHRADDHA ON 30 JAN 2018 START
                //ODTravelModelObj.ODLoginTime = ODtravelApplicationEdit.ODLoginTime;
                //ODTravelModelObj.ODLogOutTime = ODtravelApplicationEdit.ODLogOutTime;
                //ODTravelModelObj.IsInPunchInNextDay = ODtravelApplicationEdit.IsInPunchInNextDay;
                //ODTravelModelObj.IsOutPunchInNextDay = ODtravelApplicationEdit.IsOutPunchInNextDay;

                if (!string.IsNullOrEmpty(ODTravelModelObj.ODTourType))
                {
                    if (ODTravelModelObj.ODTourType == "Late Reporting" || ODTravelModelObj.ODTourType == "Early Departure")
                    {
                        ODTravelModelObj.ODDate = ODTravelModelObj.FromDate;
                        ODTravelModelObj.ODDayStatus2 = ODTravelModelObj.ODDayStatus1;
                    }
                }
                //CODE ADDED BY SHRADDHA ON 30 JAN 2018 END

                ODTravelModelObj.EffectiveDate = ODtravelApplicationEdit.EffectiveDate; // CODE ADDED BY SHRADDHA ON 17 MAR 2018

                ODTravelModelObj.CustomerName = ODtravelApplicationEdit.CustomerName;

                ODTravelModelObj.CSRNo = ODtravelApplicationEdit.CSRNo;

                ODTravelModelObj.ClientName = ODtravelApplicationEdit.ClientName;

                ODTravelModelObj.PurposeDesc = ODtravelApplicationEdit.PurposeDescription;

                PopulateDropDown();


                return View(ODTravelModelObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Late/Early edit failed due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                Error("Late/Early edit failed");

                return View();
            }
        }

        /// <summary>
        /// Added by Rajas for ODTravel edit
        /// Updated by Rajas on 17 Nov 2016 for ODTravel edit
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ODtravelApplicationEdit"></param>
        /// <returns></returns>
        /// Updated by Rajas on 22 AUGUST 2017
        [HttpPost]
        public ActionResult ODLateEarlyEdit(int id, ODTravelModel ODtravelApplicationEdit)
        {
            try
            {

                int EmpId = ODtravelApplicationEdit.EmployeeId;

                // In case of self OD/Travel application
                if (EmpId <= 0)
                {
                    EmpId = Convert.ToInt32(Session["EmployeeNo"]);
                }


                //CODE ADDED BY SHRADDHA ON 30 JAN 2018 START
                if (string.IsNullOrEmpty(ODtravelApplicationEdit.ODTourType))
                {
                    ModelState.AddModelError("", "Please select Proper requisition type");

                    PopulateDropDown();

                    return View(ODtravelApplicationEdit);
                }
                else
                {
                    //ADDED BY PUSHKAR ON 31 AUGUST 2018 
                    if (ODtravelApplicationEdit.ODTourType == "Late Reporting" || ODtravelApplicationEdit.ODTourType == "Early Departure")
                    {
                        ODtravelApplicationEdit.FromDate = ODtravelApplicationEdit.ODDate;
                        ODtravelApplicationEdit.ToDate = ODtravelApplicationEdit.ODDate;
                        ODtravelApplicationEdit.ODDayStatus1 = ODtravelApplicationEdit.ODDayStatus2;
                        ODtravelApplicationEdit.ODDayStatus = ODtravelApplicationEdit.ODDayStatus2;
                        if (ODtravelApplicationEdit.ODDate == null)
                        {
                            ModelState.AddModelError("", "Please enter proper Late/Early Date");

                            PopulateDropDown();

                            return View(ODtravelApplicationEdit);
                        }
                        //if (ODtravelApplicationEdit.ODLoginTime == null)
                        //{
                        //    ModelState.AddModelError("", "Please enter proper Login Time");

                        //    PopulateDropDown();

                        //    return View(ODtravelApplicationEdit);
                        //}

                        //if (ODtravelApplicationEdit.ODLogOutTime == null)
                        //{
                        //    ModelState.AddModelError("", "Please enter proper Logout Time");

                        //    PopulateDropDown();

                        //    return View(ODtravelApplicationEdit);
                        //}
                        ////CODE ADDED BY SHRADDHA ON 29 JAN 2018 START
                        //DateTime NextDay = ODtravelApplicationEdit.FromDate.Value.AddDays(1);
                        //if (ODtravelApplicationEdit.IsInPunchInNextDay == true)
                        //{
                        //    ODtravelApplicationEdit.ODLoginTime = new DateTime(NextDay.Year, NextDay.Month, NextDay.Day, ODtravelApplicationEdit.ODLoginTime.Value.Hour, ODtravelApplicationEdit.ODLoginTime.Value.Minute, ODtravelApplicationEdit.ODLoginTime.Value.Second);
                        //}
                        //else
                        //{
                        //    ODtravelApplicationEdit.ODLoginTime = new DateTime(ODtravelApplicationEdit.FromDate.Value.Year, ODtravelApplicationEdit.FromDate.Value.Month, ODtravelApplicationEdit.FromDate.Value.Day, ODtravelApplicationEdit.ODLoginTime.Value.Hour, ODtravelApplicationEdit.ODLoginTime.Value.Minute, ODtravelApplicationEdit.ODLoginTime.Value.Second);
                        //}
                        //if (ODtravelApplicationEdit.IsOutPunchInNextDay == true)
                        //{
                        //    ODtravelApplicationEdit.ODLogOutTime = new DateTime(NextDay.Year, NextDay.Month, NextDay.Day, ODtravelApplicationEdit.ODLogOutTime.Value.Hour, ODtravelApplicationEdit.ODLogOutTime.Value.Minute, ODtravelApplicationEdit.ODLogOutTime.Value.Second);
                        //}
                        //else
                        //{
                        //    ODtravelApplicationEdit.ODLogOutTime = new DateTime(ODtravelApplicationEdit.FromDate.Value.Year, ODtravelApplicationEdit.FromDate.Value.Month, ODtravelApplicationEdit.FromDate.Value.Day, ODtravelApplicationEdit.ODLogOutTime.Value.Hour, ODtravelApplicationEdit.ODLogOutTime.Value.Minute, ODtravelApplicationEdit.ODLogOutTime.Value.Second);
                        //}
                    }
                }
                //CODE ADDED BY SHRADDHA ON 30 JAN 2018 END

                //Added By Shraddha on 12 DEC 2016 for adding date validation start
                if (ODtravelApplicationEdit.FromDate > ODtravelApplicationEdit.ToDate)
                {
                    ModelState.AddModelError("", "End Date should be Greater than or equal to From Date");

                }
                //Added By Shraddha on 12 DEC 2016 for adding date validation End

                // Updated by Rajas on 15 JULY 2017
                if (ODtravelApplicationEdit.FromDate == ODtravelApplicationEdit.ToDate)
                {
                    if (ODtravelApplicationEdit.ODDayStatus != ODtravelApplicationEdit.ODDayStatus1)
                    {
                        // ERROR
                        ModelState.AddModelError("", "Select Proper Day status");

                        // FILL VIEW BAG AGAIN
                        PopulateDropDown();

                        return View(ODtravelApplicationEdit);
                    }
                }
                else
                {
                    //if (ODTourObj.ODDayStatus == "1") // NEED DISCUSSION
                    //{
                    //    // ERROR
                    //    ModelState.AddModelError("", "Select Proper status for from date");
                    //    PopulateDropDown();
                    //    LeaveApplicationNecessaryContent(EmpId);
                    //    return View(ODTourObj);
                    //}

                }

                /// Validation for Punch relaxed in case of OD 
                /// Updated by Rajas on 17 MAY 2017
                /// As per meeting discussion on 17 MAY 2017 with Katre sir, Kulkarni sir, Kirti madam, Madhav sir and Rajas
                #region PUNCH VALIDATION
                // Check if Punch is available for selected employee
                // Added by Rajas on 10 MAY 2017 START
                //DailyTransaction DailyTransactionObj = WetosDB.DailyTransactions.Where(a => a.TranDate >= LeaveTypeObj.FromDate && a.TranDate <= LeaveTypeObj.ToDate
                //    && a.EmployeeId == EmpId && (a.Status == "AAAA" || a.Status == "AAPP" || a.Status == "PPAA")).FirstOrDefault();

                // Updated by Rajas on 17 MAY 2017
                //DailyTransaction DailyTransactionObj = WetosDB.DailyTransactions.Where(a => a.TranDate >= ODTourObj.FromDate && a.TranDate <= ODTourObj.ToDate
                //   && a.EmployeeId == EmpId).FirstOrDefault();

                //if (DailyTransactionObj != null)
                //{
                //    // Added by Rajas on 18 MAY 2017 
                //    TimeSpan TransTime = DailyTransactionObj.TranDate.TimeOfDay;

                //    TimeSpan LoginTime = DailyTransactionObj.Login.TimeOfDay;

                //    if (TransTime != LoginTime)
                //    {
                //        ModelState.AddModelError("", "You can't apply leave as your punch is already available for selected date range");

                //        // Added by Rajas on 30 MARCH 2017 START
                //        string UpdateStatus = string.Empty;

                //        if (LeaveApplicationNecessaryContent(EmpId, ref UpdateStatus) == false)
                //        {
                //            AddAuditTrail("Error in Leave application due to " + UpdateStatus);

                //            Error(UpdateStatus);

                //            return RedirectToAction("LeaveApplicationIndex");
                //        }

                //        PopulateDropDown();

                //        return View(ODTourObj);
                //    }
                //}
                // Added by Rajas on 10 MAY 2017 END
                #endregion

                // Added by Rajas on 9 MAY 2017
                // Updated by Rajas on 9 JULY 2017 START
                // Fixed Issue No. 6 in Flagship Defect Sheet raised by Sandeep sir

                #region VALIDATE OD DATA

                // Updated by Rajas on 27 SEP 2017 START
                // Fixed Issue No. 6 in Flagship Defect Sheet raised by Sandeep sir
                LeaveApplication LeaveApplicationObj = WetosDB.LeaveApplications.Where(a => a.EmployeeId == EmpId
                    && (a.StatusId == 1 || a.StatusId == 2 || a.StatusId == 4) && a.MarkedAsDelete == 0
                    && (
                             (a.FromDate == ODtravelApplicationEdit.FromDate) // && a.ToDate > LeaveTypeObj.ToDate) 
                             || (a.FromDate < ODtravelApplicationEdit.FromDate && a.ToDate == ODtravelApplicationEdit.ToDate)
                             || (a.FromDate < ODtravelApplicationEdit.FromDate && a.ToDate > ODtravelApplicationEdit.ToDate)
                             || (a.FromDate > ODtravelApplicationEdit.FromDate && a.ToDate < ODtravelApplicationEdit.ToDate)
                             || (a.FromDate == ODtravelApplicationEdit.FromDate && a.ToDate == ODtravelApplicationEdit.ToDate)
                             || (a.FromDate > ODtravelApplicationEdit.FromDate && a.FromDate < ODtravelApplicationEdit.ToDate) // ADDED BY MSJ ON 11 JAN 2018
                             || (a.ToDate > ODtravelApplicationEdit.FromDate && a.ToDate < ODtravelApplicationEdit.ToDate) // ADDED BY MSJ ON 11 JAN 2018
                             || (a.ToDate == ODtravelApplicationEdit.FromDate))// FROM DATE AND TO DATE CONDITION ADDED BY SHRADDHA WITH HELP OF MSJ ON 01 FEB 2018

                    ).FirstOrDefault();

                // Added by Rajas on 10 MARCH 2017 for validation to check if already pending or sanctioned CO is applied for same date
                CompOffApplication CompoffApplicationObj = WetosDB.CompOffApplications.Where(a => a.EmployeeId == EmpId
                    && (a.StatusId == 1 || a.StatusId == 2 || a.StatusId == 4)
                    && a.MarkedAsDelete == 0
                    && (
                             (a.FromDate == ODtravelApplicationEdit.FromDate) // && a.ToDate > LeaveTypeObj.ToDate) 
                             || (a.FromDate < ODtravelApplicationEdit.FromDate && a.ToDate == ODtravelApplicationEdit.ToDate)
                             || (a.FromDate < ODtravelApplicationEdit.FromDate && a.ToDate > ODtravelApplicationEdit.ToDate)
                             || (a.FromDate > ODtravelApplicationEdit.FromDate && a.ToDate < ODtravelApplicationEdit.ToDate)
                             || (a.FromDate == ODtravelApplicationEdit.FromDate && a.ToDate == ODtravelApplicationEdit.ToDate)
                             || (a.FromDate > ODtravelApplicationEdit.FromDate && a.FromDate < ODtravelApplicationEdit.ToDate) // ADDED BY MSJ ON 11 JAN 2018
                             || (a.ToDate > ODtravelApplicationEdit.FromDate && a.ToDate < ODtravelApplicationEdit.ToDate) // ADDED BY MSJ ON 11 JAN 2018
                             || (a.ToDate == ODtravelApplicationEdit.FromDate))// FROM DATE AND TO DATE CONDITION ADDED BY SHRADDHA WITH HELP OF MSJ ON 01 FEB 2018
                    ).FirstOrDefault();

                ODTour ODTourTblObj = WetosDB.ODTours.Where(a => a.EmployeeId == EmpId && (a.StatusId == 1 || a.StatusId == 2 || a.StatusId == 4)
                    && a.MarkedAsDelete == 0
                    && (
                             (a.FromDate == ODtravelApplicationEdit.FromDate) // && a.ToDate > LeaveTypeObj.ToDate) 
                             || (a.FromDate < ODtravelApplicationEdit.FromDate && a.ToDate == ODtravelApplicationEdit.ToDate)
                             || (a.FromDate < ODtravelApplicationEdit.FromDate && a.ToDate > ODtravelApplicationEdit.ToDate)
                             || (a.FromDate > ODtravelApplicationEdit.FromDate && a.ToDate < ODtravelApplicationEdit.ToDate)
                             || (a.FromDate == ODtravelApplicationEdit.FromDate && a.ToDate == ODtravelApplicationEdit.ToDate)
                             || (a.FromDate > ODtravelApplicationEdit.FromDate && a.FromDate < ODtravelApplicationEdit.ToDate) // ADDED BY MSJ ON 11 JAN 2018
                             || (a.ToDate > ODtravelApplicationEdit.FromDate && a.ToDate < ODtravelApplicationEdit.ToDate) // ADDED BY MSJ ON 11 JAN 2018
                             || (a.ToDate == ODtravelApplicationEdit.FromDate))
                    && a.ODTourId != ODtravelApplicationEdit.ODTourId).FirstOrDefault();// FROM DATE AND TO DATE CONDITION ADDED BY SHRADDHA WITH HELP OF MSJ ON 01 FEB 2018
                // Updated by Rajas on 9 JULY 2017 END

                for (DateTime CurrentDate = Convert.ToDateTime(ODtravelApplicationEdit.FromDate); CurrentDate.Date <= ODtravelApplicationEdit.ToDate; CurrentDate = CurrentDate.AddDays(1))
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
                                    ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                    PopulateDropDown();

                                    return View(ODtravelApplicationEdit);
                                }
                                else
                                {
                                    if (LeaveApplicationObj.FromDayStatus == ODtravelApplicationEdit.ODDayStatus)
                                    {
                                        ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                        PopulateDropDown();

                                        return View(ODtravelApplicationEdit);
                                    }
                                    else
                                    {
                                        if (ODtravelApplicationEdit.ODDayStatus == 1)
                                        {
                                            ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                            PopulateDropDown();

                                            return View(ODtravelApplicationEdit);
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
                                    ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                    PopulateDropDown();

                                    return View(ODtravelApplicationEdit);
                                }
                                else
                                {
                                    if (LeaveApplicationObj.FromDayStatus == ODtravelApplicationEdit.ODDayStatus)
                                    {
                                        ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                        PopulateDropDown();

                                        return View(ODtravelApplicationEdit);
                                    }
                                    else
                                    {
                                        if (ODtravelApplicationEdit.ODDayStatus == 1)
                                        {
                                            ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                            PopulateDropDown();

                                            return View(ODtravelApplicationEdit);
                                        }
                                    }
                                }
                            }
                            else if (CurrentDate == LeaveApplicationObj.ToDate)
                            {
                                if (LeaveApplicationObj.ToDayStatus == 1)  // Full day in single day
                                {
                                    ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                    PopulateDropDown();

                                    return View(ODtravelApplicationEdit);
                                }
                                else
                                {
                                    if (LeaveApplicationObj.FromDayStatus == ODtravelApplicationEdit.ODDayStatus)
                                    {
                                        ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                        PopulateDropDown();

                                        return View(ODtravelApplicationEdit);
                                    }
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                PopulateDropDown();

                                return View(ODtravelApplicationEdit);
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
                                    ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                    PopulateDropDown();

                                    return View(ODtravelApplicationEdit);
                                }
                                else
                                {
                                    if (CompoffApplicationObj.FromDateStatus == ODtravelApplicationEdit.ODDayStatus)
                                    {
                                        ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                        PopulateDropDown();

                                        return View(ODtravelApplicationEdit);
                                    }
                                    else
                                    {
                                        if (ODtravelApplicationEdit.ODDayStatus == 1)
                                        {
                                            ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                            PopulateDropDown();

                                            return View(ODtravelApplicationEdit);
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
                                    ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                    PopulateDropDown();

                                    return View(ODtravelApplicationEdit);
                                }
                                else
                                {
                                    if (CompoffApplicationObj.FromDateStatus == ODtravelApplicationEdit.ODDayStatus)
                                    {
                                        ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                        PopulateDropDown();

                                        return View(ODtravelApplicationEdit);
                                    }
                                    else
                                    {
                                        if (ODtravelApplicationEdit.ODDayStatus == 1)
                                        {
                                            ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                            PopulateDropDown();

                                            return View(ODtravelApplicationEdit);
                                        }
                                    }
                                }
                            }
                            else if (CurrentDate == CompoffApplicationObj.ToDate)
                            {
                                if (CompoffApplicationObj.ToDateStatus == 1)  // Full day in single day
                                {
                                    ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                    PopulateDropDown();

                                    return View(ODtravelApplicationEdit);
                                }
                                else
                                {
                                    if (CompoffApplicationObj.FromDateStatus == ODtravelApplicationEdit.ODDayStatus)
                                    {
                                        ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                        PopulateDropDown();

                                        return View(ODtravelApplicationEdit);
                                    }
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                PopulateDropDown();

                                return View(ODtravelApplicationEdit);
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

                        //CODE COMMENTED AND ADDED NEW CODE BY SHRADDHA ON 28 OCT 2017 START
                        //if (CompoffApplicationObj.FromDate == CompoffApplicationObj.ToDate) // Single day
                        if (ODTourTblObj.FromDate == ODTourTblObj.ToDate)
                        {
                            //CODE COMMENTED AND ADDED NEW CODE BY SHRADDHA ON 28 OCT 2017 END
                            if (CurrentDate == ODTourTblObj.FromDate)
                            {
                                //CODE COMMENTED AND ADDED NEW CODE BY SHRADDHA ON 28 OCT 2017 START
                                //if (CompoffApplicationObj.FromDateStatus == 1)  // Full day in single day
                                if (ODTourTblObj.ODDayStatus == 1)
                                //CODE COMMENTED AND ADDED NEW CODE BY SHRADDHA ON 28 OCT 2017 END
                                {
                                    ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned OD for this date range.");

                                    PopulateDropDown();

                                    return View(ODtravelApplicationEdit);
                                }
                                else
                                {
                                    if (ODTourTblObj.ODDayStatus == ODtravelApplicationEdit.ODDayStatus)
                                    {
                                        ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned OD for this date range.");

                                        PopulateDropDown();

                                        return View(ODtravelApplicationEdit);
                                    }
                                    else
                                    {
                                        if (ODtravelApplicationEdit.ODDayStatus == 1)
                                        {
                                            ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned OD for this date range.");

                                            PopulateDropDown();

                                            return View(ODtravelApplicationEdit);
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
                                    ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned OD for this date range.");

                                    PopulateDropDown();

                                    return View(ODtravelApplicationEdit);
                                }
                                else
                                {
                                    if (ODTourTblObj.ODDayStatus == ODtravelApplicationEdit.ODDayStatus)
                                    {
                                        ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned OD for this date range.");

                                        PopulateDropDown();

                                        return View(ODtravelApplicationEdit);
                                    }
                                    else
                                    {
                                        if (ODtravelApplicationEdit.ODDayStatus == 1)
                                        {
                                            ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned OD for this date range.");

                                            PopulateDropDown();

                                            return View(ODtravelApplicationEdit);
                                        }
                                    }
                                }
                            }
                            else if (CurrentDate == ODTourTblObj.ToDate)
                            {
                                if (ODTourTblObj.ODDayStatus == 1)  // Full day in single day
                                {
                                    ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned OD for this date range.");

                                    PopulateDropDown();

                                    return View(ODtravelApplicationEdit);
                                }
                                else
                                {
                                    if (ODTourTblObj.ODDayStatus == ODtravelApplicationEdit.ODDayStatus)
                                    {
                                        ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned OD for this date range.");

                                        PopulateDropDown();

                                        return View(ODtravelApplicationEdit);
                                    }
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", "You Can not Apply Late/Early For " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned OD for this date range.");

                                PopulateDropDown();

                                return View(ODtravelApplicationEdit);
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
                DailyTransaction DailyTransactionListObj = WetosDB.DailyTransactions.Where(a => a.TranDate >= ODtravelApplicationEdit.FromDate
                    && a.TranDate <= ODtravelApplicationEdit.ToDate && a.EmployeeId == EmpId && a.Lock == "Y").FirstOrDefault();

                if (DailyTransactionListObj != null)
                {
                    ModelState.AddModelError("", "You Can not apply leave for this range as Data is Locked");

                    PopulateDropDown();

                    //LeaveApplicationNecessaryContent(EmpId);

                    return View(ODtravelApplicationEdit);
                }
                // Added by Rajas on 10 MARCH 2017 to Validate if data is locked, can not apply leave END
                #endregion

                #endregion

                //Modified By Shraddha on 12 DEC 2016 Added if(ModelState.IsValid) Condition instead of try catch block
                if (ModelState.IsValid)
                {
                    //WetosDB.ODTour ODtravelApplicationObj = WetosDB.ODTours.Where(b => b.ODTourId == id).FirstOrDefault();

                    //ADDED BY PUSHKAR ON 7 SEPTEMBER 2017 FOR CREATING NEW OBJ FOR EDIT ON AUDIT LOG
                    WetosDB.ODLateEarly ODtravelApplicationObj = WetosDB.ODLateEarlies.Where(b => b.ODTourId == id).FirstOrDefault();

                    // Updated by Rajas on 15 JULY 2017 START
                    TimeSpan ts = (Convert.ToDateTime(ODtravelApplicationEdit.ToDate)) - (Convert.ToDateTime(ODtravelApplicationEdit.FromDate));

                    float AppliedDay = ts.Days;
                    if (ODtravelApplicationEdit.ODDayStatus == 1 && ODtravelApplicationEdit.ODDayStatus1 == 1)
                    {
                        AppliedDay = AppliedDay + 1;
                    }

                        //Modified By Shraddha on 30 DEC 2016 for leave application validation changes start
                    else if ((ODtravelApplicationEdit.ODDayStatus == 2) && (ODtravelApplicationEdit.ODDayStatus1 == 2) && ODtravelApplicationEdit.ODDayStatus == ODtravelApplicationEdit.ODDayStatus1)
                    {
                        AppliedDay = 0.5F;
                    }
                    else if ((ODtravelApplicationEdit.ODDayStatus == 3) && (ODtravelApplicationEdit.ODDayStatus1 == 3) && ODtravelApplicationEdit.ODDayStatus == ODtravelApplicationEdit.ODDayStatus1)
                    {
                        AppliedDay = 0.5F;
                    }
                    else if ((ODtravelApplicationEdit.ODDayStatus == 2) && (ODtravelApplicationEdit.ODDayStatus1 == 3) && ODtravelApplicationEdit.ODDayStatus == ODtravelApplicationEdit.ODDayStatus1)
                    {
                        AppliedDay = 1F;
                    }

                    //Modified By Shraddha on 30 DEC 2016 for leave application validation changes End
                    else if ((ODtravelApplicationEdit.ODDayStatus == 2 || ODtravelApplicationEdit.ODDayStatus == 3) && (ODtravelApplicationEdit.ODDayStatus1 == 2 || ODtravelApplicationEdit.ODDayStatus1 == 3)
                        && ODtravelApplicationEdit.ODDayStatus != ODtravelApplicationEdit.ODDayStatus1)
                    {
                        AppliedDay = AppliedDay + 0;
                    }

                    else if (ODtravelApplicationEdit.ODDayStatus == 2 || ODtravelApplicationEdit.ODDayStatus1 == 2 || ODtravelApplicationEdit.ODDayStatus == 3 || ODtravelApplicationEdit.ODDayStatus1 == 3)
                    {
                        AppliedDay = (float)(AppliedDay + 0.5);
                    }
                    // Updated by Rajas on 15 JULY 2017 END

                    ODtravelApplicationObj.AppliedDay = AppliedDay;
                    ODtravelApplicationObj.FromDate = ODtravelApplicationEdit.FromDate;
                    ODtravelApplicationObj.ODDayStatus = ODtravelApplicationEdit.ODDayStatus;
                    ODtravelApplicationObj.ToDate = ODtravelApplicationEdit.ToDate;

                    ODtravelApplicationObj.ODDayStatus1 = ODtravelApplicationEdit.ODDayStatus1; // Added by Rajas on 10 JULY 2017

                    ODtravelApplicationObj.Purpose = ODtravelApplicationEdit.Purpose;
                    ODtravelApplicationObj.Place = ODtravelApplicationEdit.Place;
                    ODtravelApplicationObj.ODTourType = ODtravelApplicationEdit.ODTourType;
                    ODtravelApplicationObj.EmployeeId = EmpId;


                    //Added By Shraddha on 12 DEC 2016 for taking Status along with statusId Start
                    ODtravelApplicationObj.Status = WetosDB.DropdownDatas.Where(a => a.TypeId == 3 && a.Value == ODtravelApplicationEdit.StatusId).Select(a => a.Text).FirstOrDefault();
                    //Added By Shraddha on 12 DEC 2016 for taking Status along with statusId End

                    ODtravelApplicationObj.EffectiveDate = ODtravelApplicationEdit.EffectiveDate; // CODE ADDED BY SHRADDHA ON 17 MAR 2018

                    ODtravelApplicationObj.CustomerName = ODtravelApplicationEdit.CustomerName;
                    ODtravelApplicationObj.CSRNo = ODtravelApplicationEdit.CSRNo;
                    ODtravelApplicationObj.ClientName = ODtravelApplicationEdit.ClientName;
                    ODtravelApplicationObj.PurposeDescription = ODtravelApplicationEdit.PurposeDesc;

                    WetosDB.SaveChanges();

                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 7 SEPTEMBER 2017
                    #region ADD AUDIT LOG
                    string Oldrecord = "ODTourType : " + ODtravelApplicationObj.ODTourType + ", FromDate : " + ODtravelApplicationObj.FromDate
                       + ", ToDate : " + ODtravelApplicationObj.ToDate + ", ODDayStatus : " + ODtravelApplicationObj.ODDayStatus
                       + ", ODDayStatus1 : " + ODtravelApplicationObj.ODDayStatus1 + ", AppliedDays :" + ODtravelApplicationObj.AppliedDay + ", ActualDays :"
                       + ODtravelApplicationObj.ActualDay + ", Purpose :" + ODtravelApplicationObj.Purpose + ", Status :" + ODtravelApplicationObj.Status
                       + ", BranchId :" + ODtravelApplicationObj.BranchId
                       + ", CompanyId :" + ODtravelApplicationObj.CompanyId;
                    //---x---------x-----------------x------------x----------------x-----------x-------------x------------x--------------x----------x---------x-----
                    string Newrecord = "ODTourType : " + ODtravelApplicationObj.ODTourType + ", FromDate : " + ODtravelApplicationObj.FromDate
                       + ", ToDate : " + ODtravelApplicationObj.ToDate + ", ODDayStatus : " + ODtravelApplicationObj.ODDayStatus
                       + ", ODDayStatus1 : " + ODtravelApplicationObj.ODDayStatus1 + ", AppliedDays :" + ODtravelApplicationObj.AppliedDay + ", ActualDays :"
                       + ODtravelApplicationObj.ActualDay + ", Purpose :" + ODtravelApplicationObj.Purpose + ", Status :" + ODtravelApplicationObj.Status
                       + ", BranchId :" + ODtravelApplicationObj.BranchId
                       + ", CompanyId :" + ODtravelApplicationObj.CompanyId;

                    //LOGIN ID TAKEN FROM SESSION PERSISTER
                    string Formname = "Late/Early APPLICATION";
                    //ACTION IS UPDATE
                    string Message = " ";

                    WetosAdministrationController.GenerateAuditLogsUpdate(WetosDB, ODtravelApplicationObj.EmployeeId, Formname, Oldrecord,
                        Newrecord, ref Message);
                    #endregion
                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 7 SEPTEMBER 2017


                    #region ODTour APPLICATION EDIT NOTIFICATION

                    // FOR NOTIFICATION TO BE SENT TO REPORTING PERSON

                    Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == ODtravelApplicationEdit.EmployeeId).FirstOrDefault();
                    Notification NotificationObj = new Notification();
                    NotificationObj.FromID = EmployeeObj.EmployeeId;
                    NotificationObj.ToID = EmployeeObj.EmployeeReportingId;
                    NotificationObj.SendDate = DateTime.Now;
                    NotificationObj.NotificationContent = "Applied Late/Early edited by " + EmployeeObj.FirstName + " " + EmployeeObj.LastName + " taken for " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " to " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " is pending for approval";
                    NotificationObj.ReadFlag = false;
                    NotificationObj.SendDate = DateTime.Now;

                    WetosDB.Notifications.AddObject(NotificationObj);

                    WetosDB.SaveChanges();

                    //FOR SELF NOTIFICATION

                    Notification NotificationObj2 = new Notification();
                    NotificationObj2.FromID = EmployeeObj.EmployeeId;
                    NotificationObj2.ToID = EmployeeObj.EmployeeId;
                    NotificationObj2.SendDate = DateTime.Now;
                    NotificationObj2.NotificationContent = "Late/Early" + " taken for " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " to " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " edited successfully";
                    NotificationObj2.ReadFlag = false;
                    NotificationObj2.SendDate = DateTime.Now;

                    WetosDB.Notifications.AddObject(NotificationObj2);

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
                                if (SendEmail(EmployeeObj.Email, NotificationObj.NotificationContent, NotificationObj.NotificationContent, ref EmailUpdateStatus, "OD/Travel Application") == false)
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
                    AddAuditTrail("Success - Late/Early application edit : " + ODtravelApplicationEdit.ODTourType + " taken for " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " to " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " edited successfully");  // Updated by Rajas on 17 JAN 2017

                    // Added by Rajas on 17 JAN 2017 START
                    Success("Success - Late/Early application edit : " + ODtravelApplicationEdit.ODTourType + " taken  on " + ODtravelApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " to " + ODtravelApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " edited successfully");

                    return RedirectToAction("ODLateEarlyIndex");
                }
                else
                {
                    //Modified By Shraddha on 12 DEC 2016 Added Populate Dropdown and ODtravelApplicationEdit object pass start
                    PopulateDropDown();

                    //Modified By Shraddha on 12 DEC 2016 Added Populate Dropdown and ODtravelApplicationEdit object pass End

                    // Added by Rajas on 17 JAN 2017 START
                    AddAuditTrail("Error - Edit Late/Early application failed");  // Updated by Rajas on 17 JAN 2017


                    Error("Error - Edit Late/Early application failed");
                    // Added by Rajas on 17 JAN 2017 END

                    return View(ODtravelApplicationEdit);

                }
            }

            catch (System.Exception ex)
            {
                // Added by Rajas on 17 JAN 2017 START
                AddAuditTrail("Error - Edit Late/Early application failed due to " + ex.Message);  // Updated by Rajas on 17 JAN 2017


                Error("Error - Edit Late/Early application failed due to " + ex.Message);
                // Added by Rajas on 17 JAN 2017 END

                PopulateDropDown();

                return View(ODtravelApplicationEdit);
            }

        }


        public ActionResult ODLateEarlyIndex(string IsMySelf = "true")
        {
            try
            {
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);

                #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                //List<VwODTravelIndex> ODTravelObj = new List<VwODTravelIndex>();
                List<SP_VwODEarlyLateIndex_Result> ODTravelObj = new List<SP_VwODEarlyLateIndex_Result>();
                #endregion
                //GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == "Current Financial Year").FirstOrDefault();

                //ABOVE LINE COMMENTED AND BELOW LINE IS ADDED BY SHALAKA ON 13TH DEC 2017
                GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.CurrentFinancialYear).FirstOrDefault();

                // Error
                if (GlobalSettingObj == null)
                {
                    Error("Inconsistent Financial year data");

                    return View(ODTravelObj);
                }

                DateTime CalanderStartDate = GetFYStartDate(); // DateTime CalanderStartDate = WetosDB.FinancialYears.Where(a => a.FinancialName == GlobalSettingObj.SettingValue).Select(a => a.StartDate).FirstOrDefault();

                if (CalanderStartDate != null)
                {
                    if (IsMySelf == "true")
                    {
                        #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                        //ODTravelObj = WetosDB.VwODTravelIndexes.Where(a => a.EmployeeId == EmployeeId)
                        //    .Where(a => a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0).OrderByDescending(a => a.FromDate).ToList();
                        ODTravelObj = WetosDB.SP_VwODEarlyLateIndex(EmployeeId).Where(a => a.EmployeeId == EmployeeId)
                            .Where(a => a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0).OrderByDescending(a => a.FromDate).ToList();
                        #endregion
                    }
                    else
                    {
                        #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                        //ODTravelObj = WetosDB.VwODTravelIndexes
                        //    .Where(a => a.MarkedAsDelete == 0 && a.FromDate >= CalanderStartDate && a.EmployeeId != EmployeeId).OrderByDescending(a => a.FromDate).ToList();
                        ODTravelObj = WetosDB.SP_VwODEarlyLateIndex(EmployeeId)
                            .Where(a => a.MarkedAsDelete == 0 && a.FromDate >= CalanderStartDate && a.EmployeeId != EmployeeId).OrderByDescending(a => a.FromDate).ToList();
                        #endregion
                    }
                }
                // Updated by Rajas on 29 AUGUST 2017 END

                AddAuditTrail("Visited Late/Early list");

                ViewBag.ForOthers = IsMySelf; // Added by Rajas on 22 AUGUST 2017

                return View(ODTravelObj);
            }

            catch (System.Exception ex)
            {
                AddAuditTrail("Error in Late/Early list view due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                Error("Error in viewing list for Late/Early");

                return View("Error");
            }
        }


        public ActionResult ODLateEarlyDelete(int id)
        {
            try
            {
                ODLateEarly ODTOURObj = WetosDB.ODLateEarlies.Where(a => a.ODTourId == id).FirstOrDefault();

                if (ODTOURObj != null)
                {
                    ODTOURObj.MarkedAsDelete = 1;

                    // ADDED BY MSJ ON 09 JAN 2019 START
                    ODTOURObj.CancelledBy = Convert.ToInt32(Session["EmployeeNo"]);
                    ODTOURObj.CancelledOn = DateTime.Now;
                    // ADDED BY MSJ ON 09 JAN 2019 END

                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 7 SEPTEMBER 2017
                    #region ADD AUDIT LOG
                    //OLD RECORD IS BLANK
                    string Oldrecord = "ODTourType : " + ODTOURObj.ODTourType + ", FromDate : " + ODTOURObj.FromDate
                       + ", ToDate : " + ODTOURObj.ToDate + ", ODDayStatus : " + ODTOURObj.ODDayStatus
                       + ", ODDayStatus1 : " + ODTOURObj.ODDayStatus1 + ", AppliedDays :" + ODTOURObj.AppliedDay + ", ActualDays :"
                       + ODTOURObj.ActualDay + ", Purpose :" + ODTOURObj.Purpose + ", Status :" + ODTOURObj.Status
                       + ", BranchId :" + ODTOURObj.BranchId
                       + ", CompanyId :" + ODTOURObj.CompanyId;

                    //LOGIN ID IS TAKEN FROM SESSION PERSISTER
                    string Formname = "Late/Early DELETE";
                    //ACTION IS UPDATE
                    string Message = " ";

                    WetosAdministrationController.GenerateAuditLogsInsert(WetosDB, ODTOURObj.EmployeeId, Formname, Oldrecord, ref Message);
                    #endregion
                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 7 SEPTEMBER 2017

                    WetosDB.SaveChanges();

                    Success("LateEarly applied between  " + ODTOURObj.FromDate.Value.ToString("dd-MMM-yyyy")
                        + " to " + ODTOURObj.ToDate.Value.ToString("dd-MMM-yyyy") + " deleted successfully");

                    AddAuditTrail("LateEarly applied between  " + ODTOURObj.FromDate.Value.ToString("dd-MMM-yyyy")
                        + " to " + ODTOURObj.ToDate.Value.ToString("dd-MMM-yyyy") + " deleted successfully");
                }

                return RedirectToAction("ODLateEarlyIndex");
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                Error("Please try again!");

                return RedirectToAction("ODLateEarlyIndex");
            }
        }

    }
}

