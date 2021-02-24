using System;
using System.Collections.Generic;
using System.Configuration;
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
    public class WetosSanctionsController : BaseController
    {
        
        //
        // GET: /WetosSanctions/

        PostingFlagModel PostingFlagModelObj = new PostingFlagModel();

        public ActionResult Index()
        {
            //ADDED BY RAJAS ON 27 DEC 2016
            AddAuditTrail(" Visited sanction module");
            return View();
        }

        /// <summary>
        /// Updated by Rajas on 30 MARCH 2017
        /// GET
        /// </summary>
        /// <returns></returns>
        public ActionResult ODTravelSanctionIndex(int selectCriteria = 0)
        {
            try
            {
                int EmpNo = Convert.ToInt32(Session["EmployeeNo"]);
                int Id = Convert.ToInt32(Session["Id"]);

                // Added by Rajas on 9 MAY 2017 START
                // Select Criteria as per dropdown value
                int Status = 0;

                // Updated by Rajas on 20 MAY 2017 START
                List<SP_WetosGetEmployeeODTravelSanction_Result> ODTOURObj = new List<SP_WetosGetEmployeeODTravelSanction_Result>();


                // Get current FY from global setting
                //GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == "Current Financial Year").FirstOrDefault();

                //ABOVE LINE COMMENTED AND BELOW LINE IS ADDED BY SHALAKA ON 13TH DEC 2017
                GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.CurrentFinancialYear).FirstOrDefault();

                // Error
                if (GlobalSettingObj == null)
                {
                    Error("Inconsistent Financial year data");
                    PopulateDropDown();
                    return View(ODTOURObj);
                }

                DateTime CalanderStartDate = GetFYStartDate(); // DateTime CalanderStartDate = WetosDB.FinancialYears.Where(a => a.FinancialName == GlobalSettingObj.SettingValue).Select(a => a.StartDate).FirstOrDefault();

                if (CalanderStartDate != null)
                {
                    //CODE REPLACED BY SHRADDHA ON 12 SEP 2017 REMOVED REDUNDANT AND UNNECESSARY IF ELSE CODE AND TAKEN ONE SINGLE LINE CODE START
                    Status = selectCriteria;

                    // MODIFIED BY MSJ ON 23 JAN 2020
                    ODTOURObj = WetosDB.SP_WetosGetEmployeeODTravelSanction(EmpNo).Where(a => ((a.EmployeeReportingId == EmpNo || a.EmployeeReportingId2 == EmpNo) && a.Id == selectCriteria && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0)) //|| a.MarkedAsDelete == null
                    .OrderByDescending(a => a.FromDate).OrderByDescending(a => a.ToDate).ToList();
                    //CODE REPLACED BY SHRADDHA ON 12 SEP 2017 REMOVED REDUNDANT AND UNNECESSARY IF ELSE CODE AND TAKEN ONE SINGLE LINE CODE END
                }


                // Updated by Rajas on 20 MAY 2017 END
                ViewBag.Status = Status;
                ViewBag.SelectedStatus = selectCriteria;
                // Added by Rajas on 9 MAY 2017 END



                #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //var EmployeeData = WetosDB.VwActiveEmployees.ToList();
                List<SP_VwActiveEmployee_Result> EmployeeData = WetosDB.SP_VwActiveEmployee(EmployeeId).ToList();
                #endregion

                // List<SP_WetosGetEmployeeLeaveSanction_Result> LeaveSanctionObj = WetosDB.SP_WetosGetEmployeeLeaveSanction(EmpNo).OrderByDescending(a => a.FromDate).OrderByDescending(a => a.ToDate).ToList();

                // Added by Rajas on 9 MAY 2017
                // Populate Status dropdown

                //REPLACED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 START
                //List<string> StatusObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 3).Select(a => a.Text).ToList();
                //ViewBag.StatusList = StatusObj;
                PopulateDropDown();
                //REPLACED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 END


                return View(ODTOURObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in ODTravelSanction due to " + ex.Message + ( ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                Error("Error in ODTravelSanction due to " + ex.Message + ( ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 START
                PopulateDropDown();
                //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 END

                return RedirectToAction("EmployeeDashboard", "Dashboard");
            }
        }

        /// <summary>
        /// Added by Rajas on 18 MAY 2017
        /// </summary>
        /// <param name="selectCriteria"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ODTravelSanctionPV(int selectCriteria)
        {
            try
            {
                int EmpNo = Convert.ToInt32(Session["EmployeeNo"]);
                int Id = Convert.ToInt32(Session["Id"]);

                // Added by Rajas on 9 MAY 2017 START
                // Select Criteria as per dropdown value
                int Status = 0;

                // Updated by Rajas on 20 MAY 2017 START
                List<SP_WetosGetEmployeeODTravelSanction_Result> ODTOURObj = new List<SP_WetosGetEmployeeODTravelSanction_Result>();

                #region UPDATED BY RAJAS ON 23 MAY 2017
                // Get current FY from global setting

                //GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == "Current Financial Year").FirstOrDefault();

                //ABOVE LINE COMMENTED AND BELOW LINE IS ADDED BY SHALAKA ON 13TH DEC 2017
                GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.CurrentFinancialYear).FirstOrDefault();

                // Error
                if (GlobalSettingObj == null)
                {
                    Error("Inconsistent Financial year data");
                    PopulateDropDown();
                    return View(ODTOURObj);
                }

                DateTime CalanderStartDate = GetFYStartDate(); // DateTime CalanderStartDate = WetosDB.FinancialYears.Where(a => a.FinancialName == GlobalSettingObj.SettingValue).Select(a => a.StartDate).FirstOrDefault();

                if (CalanderStartDate != null)
                {

                    //CODE REPLACED BY SHRADDHA ON 12 SEP 2017 REMOVED REDUNDANT AND UNNECESSARY IF ELSE CODE AND TAKEN ONE SINGLE LINE CODE START
                    //Status = selectCriteria;
                    //ODTOURObj = WetosDB.SP_WetosGetEmployeeODTravelSanction(EmpNo).Where(a => ((a.EmployeeReportingId == EmpNo || a.EmployeeReportingId2 == EmpNo) && a.Id == selectCriteria && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null))
                    //.OrderByDescending(a => a.FromDate).OrderByDescending(a => a.ToDate).ToList();
                    //CODE REPLACED BY SHRADDHA ON 12 SEP 2017 REMOVED REDUNDANT AND UNNECESSARY IF ELSE CODE AND TAKEN ONE SINGLE LINE CODE END

                    // Above code commented. Error was list for reportingId2 was not displayed
                    // Code updated by Rajas on 22 SEP 2017 START
                    if (selectCriteria == 1)  // Pending
                    {
                        Status = 1;

                        // Updated by Rajas on 7 JUNE 2017, ( || a.EmployeeReportingId2 == EmpNo) removed
                        ODTOURObj = WetosDB.SP_WetosGetEmployeeODTravelSanction(EmpNo).Where(a => ((a.EmployeeReportingId == EmpNo) && a.Id == Status
                            && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null))
                            .OrderByDescending(a => a.FromDate).ToList();  // && a.Id == 4)
                    }
                    else if (selectCriteria == 2)  // Sanction
                    {
                        Status = 2;

                        ODTOURObj = WetosDB.SP_WetosGetEmployeeODTravelSanction(EmpNo)
                            .Where(a => a.EmployeeReportingId2 == EmpNo && a.Id == Status && a.FromDate >= CalanderStartDate
                                && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null)
                            .OrderByDescending(a => a.FromDate).ToList();  // && a.Id == 4)
                    }
                    else if (selectCriteria == 3) // Rejected by Approver
                    {
                        Status = 3;

                        ODTOURObj = WetosDB.SP_WetosGetEmployeeODTravelSanction(EmpNo)
                            .Where(a => ((a.EmployeeReportingId == EmpNo || a.EmployeeReportingId2 == EmpNo)
                                && a.Id == Status && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null) || (a.EmployeeReportingId2 == EmpNo
                                && a.Id == Status && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null))
                                .OrderByDescending(a => a.FromDate).ToList();  // && a.Id == 4)
                    }
                    else if (selectCriteria == 4)  // Approve
                    {
                        Status = 4;

                        ODTOURObj = WetosDB.SP_WetosGetEmployeeODTravelSanction(EmpNo)
                            .Where(a => ((a.EmployeeReportingId2 == EmpNo || a.EmployeeReportingId == EmpNo)
                                && a.Id == Status && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null))
                                .OrderByDescending(a => a.FromDate).ToList();  // && a.Id == 4)
                    }
                    else if (selectCriteria == 5)  // Cancel
                    {
                        Status = 5;

                        ODTOURObj = WetosDB.SP_WetosGetEmployeeODTravelSanction(EmpNo)
                            .Where(a => ((a.EmployeeReportingId == EmpNo || a.EmployeeReportingId2 == EmpNo) && a.Id == Status
                                && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null))
                                .OrderByDescending(a => a.FromDate).ToList();  // && a.Id == 4)
                    }
                    else  // Rejected by Sanctioner
                    {
                        Status = 6;
                        ODTOURObj = WetosDB.SP_WetosGetEmployeeODTravelSanction(EmpNo)
                            .Where(a => ((a.EmployeeReportingId2 == EmpNo) && a.Id == Status
                                && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null))
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
                List<SP_VwActiveEmployee_Result> EmployeeData = WetosDB.SP_VwActiveEmployee(EmployeeId).ToList();
                #endregion

                // List<SP_WetosGetEmployeeLeaveSanction_Result> LeaveSanctionObj = WetosDB.SP_WetosGetEmployeeLeaveSanction(EmpNo).OrderByDescending(a => a.FromDate).OrderByDescending(a => a.ToDate).ToList();

                // Added by Rajas on 9 MAY 2017
                // Populate Status dropdown
                //REPLACED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 START
                //List<string> StatusObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 3).Select(a => a.Text).ToList();
                //ViewBag.StatusList = StatusObj;
                PopulateDropDown();
                //REPLACED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 END

                return PartialView(ODTOURObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in Leave Sanction due to " + ex.Message + ( ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                Error("Error in Leave Sanction due to " + ex.Message + ( ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 START
                PopulateDropDown();
                //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 END

                return RedirectToAction("EmployeeDashboard", "Dashboard");
            }
        }

        /// <summary>
        /// Added By Shraddha on 13 DEC 2016 For OD Travel sanction Index POST used for multiple sanction, reject at selection of radio button
        /// </summary>
        /// <param name="LeaveSanction"></param>
        /// <param name="FC"></param>
        /// <returns></returns>
        /// <summary>
        /// Added By Shraddha on 13 DEC 2016 For OD Travel sanction Index POST used for multiple sanction, reject at selection of radio button
        /// </summary>
        /// <param name="LeaveSanction"></param>
        /// <param name="FC"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ODTravelSanctionIndex(List<SP_WetosGetEmployeeODTravelSanction_Result> ODTravelSanction, FormCollection FC)
        {
            try
            {

                #region FAKE LOGIC

                ODTravelSanction = null; // new List<SP_LeaveSanctionIndex_Result>();

                // FAKE LOGIC MSJ
                int EmpNo = Convert.ToInt32(Session["EmployeeNo"]);
                int Id = Convert.ToInt32(Session["Id"]);

                //List<SP_WetosGetEmployeeLeaveSanction_Result> LeaveSanctionObj1 = WetosDB.SP_WetosGetEmployeeLeaveSanction(EmpNo).ToList();

                DateTime abc = Convert.ToDateTime("2016/01/01");

                #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //var EmployeeData = WetosDB.Employees.ToList();
                List<SP_VwActiveEmployee_Result> EmployeeData = WetosDB.SP_VwActiveEmployee(EmployeeId).ToList();
                #endregion

                List<SP_WetosGetEmployeeODTravelSanction_Result> ODTravelSanctionObjForCount = new List<SP_WetosGetEmployeeODTravelSanction_Result>();

                string RejectReasonText = FC["RejectReasonText"];

                //List<SP_LeaveSanctionIndex_Result> LeaveSanctionObj = new List<SP_LeaveSanctionIndex_Result>();
                if (ODTravelSanction == null)
                {
                    ODTravelSanction = new List<SP_WetosGetEmployeeODTravelSanction_Result>();

                    int SingleApplicationID = 0;
                    int SingleStatusID = 0;

                    // Added by Rajas on 19 MAY 2017
                    bool StatusIdSelected = false;

                    foreach (var key in FC.AllKeys)
                    {
                        string KeyStr = key.ToString();

                        if (KeyStr.Contains("StatusId") == true)
                        {
                            SingleStatusID = Convert.ToInt32(FC[KeyStr]);

                            string TempKeyStr = KeyStr.Replace("StatusId", "ODTourId");

                            SingleApplicationID = Convert.ToInt32(FC[TempKeyStr]);
                            SP_WetosGetEmployeeODTravelSanction_Result TempODTravelSanction = WetosDB.SP_WetosGetEmployeeODTravelSanction(EmpNo).Where(a => a.ODTourId == SingleApplicationID).FirstOrDefault(); //.ToList();

                            if (TempODTravelSanction != null)
                            {
                                TempODTravelSanction.StatusId = SingleStatusID;// UPDATE STATUS
                                ODTravelSanction.Add(TempODTravelSanction);
                            }

                            StatusIdSelected = true;

                            //"[6].LeaveApplicationId"
                        }

                    }

                    // Added by Rajas on 19 MAY 2017
                    if (StatusIdSelected == false)
                    {
                        Information("Please select atleast one entry for processing");

                        PopulateDropDown();

                        return View(ODTravelSanction);
                    }

                }

                #endregion

                ODTravelSanctionObjForCount = ODTravelSanction.Where(a => a.StatusId == 3 || a.StatusId == 6 || a.StatusId == 5).ToList();
                // Below code modified by Rajas on 19 MAY 2017 START
                if (ODTravelSanctionObjForCount.Count() > 0 && (RejectReasonText == null || RejectReasonText == ""))
                {
                    ModelState.AddModelError("CustomError", "Please Enter Reject/Cancel Reason");

                    //List<SP_WetosGetEmployeeODTravelSanction_Result> ODTravelSanctionList = WetosDB.SP_WetosGetEmployeeODTravelSanction(EmpNo).OrderByDescending(a => a.FromDate).OrderByDescending(a => a.ToDate).ToList();

                    // Added by Rajas on 9 MAY 2017
                    // Populate Status dropdown
                    PopulateDropDown();

                    Error("Please Enter Reject/Cancel Reason");

                    return View(ODTravelSanctionObjForCount); // ODTravelSanctionList
                }
                // Below code modified by Rajas on 19 MAY 2017 END
                else
                {
                    foreach (var i in ODTravelSanctionObjForCount)
                    {
                        ODTour ODObj = WetosDB.ODTours.Where(a => a.ODTourId == i.ODTourId).FirstOrDefault();

                        ODObj.RejectReason = RejectReasonText;

                        WetosDB.SaveChanges();

                    }
                    foreach (SP_WetosGetEmployeeODTravelSanction_Result item in ODTravelSanction)
                    {
                        if (item.StatusId == 5)   // Added by Rajas on 9 JULY 2017 to Fix Issue no. 2, defect id= FB0012 as per Test Cases sheet
                        {
                            // Cancellation reason code
                            // Added by Rajas on 12 AUGUST 2017 START
                            if (RejectReasonText == null || RejectReasonText == string.Empty)
                            {
                                ModelState.AddModelError("CustomError", "Please Enter Cancellation Reason");

                                //List<SP_WetosGetEmployeeODTravelSanction_Result> ODTravelSanctionList = WetosDB.SP_WetosGetEmployeeODTravelSanction(EmpNo).OrderByDescending(a => a.FromDate).OrderByDescending(a => a.ToDate).ToList();

                                // Added by Rajas on 9 MAY 2017
                                // Populate Status dropdown
                                PopulateDropDown();

                                Error("Please enter  Cancellation Reason");

                                return View(ODTravelSanction); // ODTravelSanctionList
                            }
                        }
                        int EmpId = Convert.ToInt32(Session["EmployeeNo"]);

                        ODTour ObjODT = WetosDB.ODTours.Where(a => a.ODTourId == item.ODTourId).FirstOrDefault();

                        // Validate Valid employee from obj
                        VwActiveEmployee ValidEmpObj = WetosDB.VwActiveEmployees.Where(a => a.EmployeeId == ObjODT.EmployeeId).FirstOrDefault();

                        if (ValidEmpObj != null)
                        {

                            if (item.StatusId != 0)
                            {
                                ODTour ODTravelObj = WetosDB.ODTours.Where(a => a.ODTourId == item.ODTourId).FirstOrDefault();

                                //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 12 SEPTEMBER 2017 FOR EDIT
                                ODTour ODTravelObjEDIT = WetosDB.ODTours.Where(a => a.ODTourId == item.ODTourId).FirstOrDefault();

                                ODTravelObj.StatusId = item.StatusId;
                                ODTravelObj.Status = WetosDB.DropdownDatas.Where(a => a.TypeId == 3 && a.Value == ODTravelObj.StatusId).Select(a => a.Text).FirstOrDefault();                               

                                for (DateTime CurrentODDate = Convert.ToDateTime(ODTravelObj.FromDate); CurrentODDate.Date <= ODTravelObj.ToDate; CurrentODDate = CurrentODDate.AddDays(1))
                                {

                                    //CODE MODIFIED BY SHRADDHA ON 13 FEB 2017 CONDITION ADDED FOR TODATE START
                                    DailyTransaction DailyTransactionObj = WetosDB.DailyTransactions.Where(a => a.EmployeeId == ODTravelObj.EmployeeId
                                        && a.TranDate == CurrentODDate).FirstOrDefault();
                                    //CODE MODIFIED BY SHRADDHA ON 13 FEB 2017 CONDITION ADDED FOR TODATE END

                                    #region SAVE ORIGINAL DailyTransaction STATUS

                                    // Save Actual DailyTransaction Status
                                    // Added by Rajas on 5 AUGUST 2017

                                    //if (item.StatusId != 2)
                                    //{
                                    //    if (DailyTransactionObj != null)
                                    //    {
                                    //        DailyTransactionObj.Status = DailyTransactionObj.ActualStatus;
                                    //        WetosDB.SaveChanges();
                                    //    }
                                    //}

                                    #endregion

                                    if ((item.StatusId == 2) || (item.StatusId == 4 && LoginPersonIsApproverAndSanctioner(ValidEmpObj.EmployeeId, EmpId)))//(ValidEmpObj.EmployeeReportingId == EmpId && ValidEmpObj.EmployeeReportingId2 == EmpId)))//UPDATED BY SHRADDHA ON 11 SEP 2017 TAKEN LoginPersonIsApproverAndSanctioner FUNCTION
                                    {
                                        if (LoginPersonIsApprover(ValidEmpObj.EmployeeId, EmpId) || LoginPersonIsSanctioner(ValidEmpObj.EmployeeId, EmpId))//ValidEmpObj.EmployeeReportingId2 == EmpId || ValidEmpObj.EmployeeReportingId == EmpId)//UPDATED BY SHRADDHA ON 11 SEP 2017 TAKEN LoginPersonIsApprover() AND LoginPersonIsSanctioner() FUNCTION
                                        {
                                            item.StatusId = 2;

                                            ODTravelObj.RejectReason = string.Empty;

                                            // Added By RAJAS on 10 JAN 2017 to Handle null object
                                            if (DailyTransactionObj != null)
                                            {
                                                PostingFlagModelObj.IsOdTour = true;

                                                // Status as per selection of ODTourType
                                                // Added by Rajas on 9 JULY 2017 to Fix issue no. 4, defect id=	FB0014 as per Test Cases sheet
                                                // Updated by Rajas on 5 AUGUST 2017
                                                string NewStatus = string.Empty;
                                                string UpdatedStatus = string.Empty;
                                                string ReturnMessage = string.Empty;

                                                if (ODTravelObj.ODTourType.ToUpper().Trim() == "TOUR")
                                                {
                                                    NewStatus = "TO";
                                                }
                                                else if (ODTravelObj.ODTourType.ToUpper().Trim() == "OD")
                                                {
                                                    NewStatus = "OD";
                                                }
                                                else
                                                {
                                                    NewStatus = "PP";
                                                }

                                                // Following code updated by Rajas on 9 JULY 2017 for Status defined above START
                                                //DailyTransactionObj.Status = "ODOD";
                                                //ADDED CODE FOR ADDING HALF DAY AND FULL DAY OD WISE STATUS IN DAILY TRANSACTION TABLE BY SHRADDHA ON 13 FEB 2017 START

                                                #region CODE UPDATE : Mark Attendance Status changes
                                                #region COMMENTED CODE BY SHRADDHA AND CALLED GENERIC FUNCTION ON 22 NOV 2017 NEED TO TEST
                                                // Below code updated by Rajas on 7 AUGUST 2017 START
                                                //if (DailyTransactionObj != null)
                                                //{
                                                //    if (DailyTransactionObj.Status.Trim() == "PPPP")  // need to verify
                                                //    {
                                                //        DailyTransactionObj.Status = "PPPP";
                                                //    }
                                                //    else if (CurrentODDate == ODTravelObj.FromDate && ODTravelObj.StatusId == 2)
                                                //    {
                                                //        if (ODTravelObj.ODDayStatus == 2)
                                                //        {
                                                //            PostingFlagModelObj.IsFirstHalfOD = true;

                                                //            //Added by Shalaka with help of Sharddha on 28th OCT 2017 For WorkingHours time Update 
                                                //            //Issue raised by Ulka on 27th OCT
                                                //            //Get Object of Shift
                                                //            Shift ShiftObj = WetosDB.Shifts.Where(a => a.BranchId == ODTravelObj.BranchId && a.Company.CompanyId == ODTravelObj.CompanyId).FirstOrDefault();

                                                //            if (ShiftObj != null)
                                                //            {
                                                //                int workingHours = ShiftObj.WorkingHours.Hour;
                                                //                int workingMinute = ShiftObj.WorkingHours.Minute;
                                                //                int workingSeconds = ShiftObj.WorkingHours.Second;

                                                //                //If First Half then
                                                //                int HrsInMinutes = workingHours * 60;
                                                //                int TotalMinutes = HrsInMinutes + workingMinute;
                                                //                int halfofTime = TotalMinutes / 2;

                                                //                #region WORKING HOURS CALCULATION CORRECTION CODE IN CASE OF HALF DAY ADDED BY SHRADDHA ON 30 OCT 2017
                                                //                //CODE ADDED BY SHRADDHA ON 30 OCT 2017 TO GET ALREADY HAVING WORKING HOURS + HALF DAY WORKING HOURS AS TOTAL WORKING HOURS START

                                                //                int DTworkingHours = 0;
                                                //                int DTworkingMinute = 0;
                                                //                int DTworkingSeconds = 0;
                                                //                if (DailyTransactionObj.WorkingHrs != null)
                                                //                {
                                                //                    DTworkingHours = DailyTransactionObj.WorkingHrs.Value.Hour;
                                                //                    DTworkingMinute = DailyTransactionObj.WorkingHrs.Value.Minute;
                                                //                    DTworkingSeconds = DailyTransactionObj.WorkingHrs.Value.Second;
                                                //                }
                                                //                //If First Half then
                                                //                int DTHrsInMinutes = DTworkingHours * 60;
                                                //                int DTTotalMinutes = DTHrsInMinutes + DTworkingMinute;

                                                //                int TotalWorkingHours = halfofTime + DTTotalMinutes;

                                                //                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 OCT 2017 START
                                                //                //Get Working Hrs for FirstHalf
                                                //                //int FirstHalfWorkingHrs = halfofTime / 60;
                                                //                int FirstHalfWorkingHrs = TotalWorkingHours / 60;
                                                //                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 OCT 2017 END
                                                //                //CODE ADDED BY SHRADDHA ON 30 OCT 2017 TO GET ALREADY HAVING WORKING HOURS + HALF DAY WORKING HOURS AS TOTAL WORKING HOURS END
                                                //                #endregion


                                                //                //To get Working minutes from FirstHalfWorkingHrs 
                                                //                int t1 = FirstHalfWorkingHrs * 60;
                                                //                int FirstHalfWorkingMinutes = TotalWorkingHours - t1;

                                                //                //Added workingHrs
                                                //                DailyTransactionObj.WorkingHrs = new DateTime(DailyTransactionObj.TranDate.Year, DailyTransactionObj.TranDate.Month, DailyTransactionObj.TranDate.Day, FirstHalfWorkingHrs, FirstHalfWorkingMinutes, workingSeconds);
                                                //            }
                                                //        }
                                                //        else if (ODTravelObj.ODDayStatus == 3)
                                                //        {
                                                //            PostingFlagModelObj.IsSecondHalfOD = true;

                                                //            //Added by Shalaka with help of Sharddha on 28th OCT 2017 For WorkingHours time Update
                                                //            //Get Object of Shift
                                                //            Shift ShiftObj = WetosDB.Shifts.Where(a => a.BranchId == ODTravelObj.BranchId && a.Company.CompanyId == ODTravelObj.CompanyId).FirstOrDefault();

                                                //            if (ShiftObj != null)
                                                //            {
                                                //                int workingHours = ShiftObj.WorkingHours.Hour;
                                                //                int workingMinute = ShiftObj.WorkingHours.Minute;
                                                //                int workingSeconds = ShiftObj.WorkingHours.Second;

                                                //                //If Second Half then
                                                //                int HrsInMinutes = workingHours * 60;
                                                //                int TotalMinutes = HrsInMinutes + workingMinute;
                                                //                int HalfofTime = TotalMinutes / 2;


                                                //                #region WORKING HOURS CALCULATION CORRECTION CODE IN CASE OF HALF DAY ADDED BY SHRADDHA ON 30 OCT 2017
                                                //                //CODE ADDED BY SHRADDHA ON 30 OCT 2017 TO GET ALREADY HAVING WORKING HOURS + HALF DAY WORKING HOURS AS TOTAL WORKING HOURS START
                                                //                int DTworkingHours = 0;
                                                //                int DTworkingMinute = 0;
                                                //                int DTworkingSeconds = 0;
                                                //                if (DailyTransactionObj.WorkingHrs != null)
                                                //                {
                                                //                    DTworkingHours = DailyTransactionObj.WorkingHrs.Value.Hour;
                                                //                    DTworkingMinute = DailyTransactionObj.WorkingHrs.Value.Minute;
                                                //                    DTworkingSeconds = DailyTransactionObj.WorkingHrs.Value.Second;
                                                //                }

                                                //                //If First Half then
                                                //                int DTHrsInMinutes = DTworkingHours * 60;
                                                //                int DTTotalMinutes = DTHrsInMinutes + DTworkingMinute;

                                                //                int TotalWorkingHours = HalfofTime + DTTotalMinutes;

                                                //                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 OCT 2017 START
                                                //                //Get Working Hrs for SecondHalf
                                                //                //int SecondHalfWorkingHrs = halfofTime / 60;
                                                //                int SecondHalfWorkingHrs = TotalWorkingHours / 60;
                                                //                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 OCT 2017 END
                                                //                //CODE ADDED BY SHRADDHA ON 30 OCT 2017 TO GET ALREADY HAVING WORKING HOURS + HALF DAY WORKING HOURS AS TOTAL WORKING HOURS END
                                                //                #endregion
                                                //                //Working Hrs time for SecondHalf

                                                //                //To get Working minutes from FirstHalfWorkingHrs 
                                                //                int t1 = SecondHalfWorkingHrs * 60;
                                                //                int SecondHalfWorkingMinutes = TotalWorkingHours - t1;

                                                //                //Added workingHrs
                                                //                DailyTransactionObj.WorkingHrs = new DateTime(DailyTransactionObj.TranDate.Year, DailyTransactionObj.TranDate.Month, DailyTransactionObj.TranDate.Day, SecondHalfWorkingHrs, SecondHalfWorkingMinutes, workingSeconds);
                                                //            }
                                                //        }
                                                //        else
                                                //        {
                                                //            PostingFlagModelObj.IsFullDayOD = true;

                                                //            //Added by Shalaka with help of Sharddha on 28th OCT 2017 For WorkingHours time Update
                                                //            //Get Object of Shift
                                                //            Shift ShiftObj = WetosDB.Shifts.Where(a => a.BranchId == ODTravelObj.BranchId && a.Company.CompanyId == ODTravelObj.CompanyId).FirstOrDefault();

                                                //            if (ShiftObj != null)
                                                //            {
                                                //                int workingHours = ShiftObj.WorkingHours.Hour;
                                                //                int workingMinute = ShiftObj.WorkingHours.Minute;
                                                //                int workingSeconds = ShiftObj.WorkingHours.Second;

                                                //                //Added workingHrs for Full Day
                                                //                DailyTransactionObj.WorkingHrs = new DateTime(DailyTransactionObj.TranDate.Year, DailyTransactionObj.TranDate.Month, DailyTransactionObj.TranDate.Day, workingHours, workingMinute, workingSeconds);
                                                //            }
                                                //        }

                                                //        DailyTransactionObj.ActualStatus = DailyTransactionObj.Status; // CODE ADDED BY SHRADDHA ON 11 SEP 2017 START
                                                //        WetosAdministrationController.GetActualDailyTransactionStatus(PostingFlagModelObj, DailyTransactionObj.Status, NewStatus, ref UpdatedStatus, ref ReturnMessage);

                                                //        DailyTransactionObj.Status = UpdatedStatus;
                                                //    }

                                                //    else if (CurrentODDate == ODTravelObj.ToDate && ODTravelObj.StatusId == 2)
                                                //    {
                                                //        if (ODTravelObj.ODDayStatus1 == 2)
                                                //        {
                                                //            PostingFlagModelObj.IsFirstHalfOD = true;
                                                //        }
                                                //        else if (ODTravelObj.ODDayStatus1 == 3)
                                                //        {
                                                //            PostingFlagModelObj.IsSecondHalfOD = true;
                                                //        }
                                                //        else
                                                //        {
                                                //            PostingFlagModelObj.IsFullDayOD = true;
                                                //        }

                                                //        //Added by Shalaka with help of Sharddha on 28th OCT 2017 For WorkingHours time Update
                                                //        //Get Object of Shift
                                                //        Shift ShiftObj = WetosDB.Shifts.Where(a => a.BranchId == ODTravelObj.BranchId && a.Company.CompanyId == ODTravelObj.CompanyId).FirstOrDefault();

                                                //        WetosAdministrationController.GetActualDailyTransactionStatus(PostingFlagModelObj, DailyTransactionObj.Status, NewStatus, ref UpdatedStatus, ref ReturnMessage);
                                                //        DailyTransactionObj.ActualStatus = DailyTransactionObj.Status; // CODE ADDED BY SHRADDHA ON 11 SEP 2017 START
                                                //        DailyTransactionObj.Status = UpdatedStatus;

                                                //        //Added By shalaka on 28th OCT 2017 to update WorkingHrs time
                                                //        if (ShiftObj != null)
                                                //        {
                                                //            int workingHours = ShiftObj.WorkingHours.Hour;
                                                //            int workingMinute = ShiftObj.WorkingHours.Minute;
                                                //            int workingSeconds = ShiftObj.WorkingHours.Second;

                                                //            //Added workingHrs
                                                //            DailyTransactionObj.WorkingHrs = new DateTime(DailyTransactionObj.TranDate.Year, DailyTransactionObj.TranDate.Month, DailyTransactionObj.TranDate.Day, workingHours, workingMinute, workingSeconds);
                                                //        }
                                                //    }
                                                //    else if (DailyTransactionObj.Status.Trim() == WetosAdministrationController.StatusConstants.FullDayAbsentStatus && ODTravelObj.StatusId == 2)  // Added by Rajas on 28 AUGUST 2017
                                                //    {
                                                //        PostingFlagModelObj.IsFullDayOD = true;

                                                //        WetosAdministrationController.GetActualDailyTransactionStatus(PostingFlagModelObj, DailyTransactionObj.Status, NewStatus, ref UpdatedStatus, ref ReturnMessage);
                                                //        DailyTransactionObj.ActualStatus = DailyTransactionObj.Status; // CODE ADDED BY SHRADDHA ON 11 SEP 2017 START
                                                //        DailyTransactionObj.Status = UpdatedStatus;
                                                //    }


                                                //}
                                                #endregion

                                                //EARLIER CODE COMMENTED BY SHRADDHA AND ADDED NEW FUNCTION CalculateODWorkingHours ON 22 NOV 2017 TO GET GENERIC CODE START
                                                WetosAdministrationController.CalculateODWorkingHours(WetosDB, DailyTransactionObj, ODTravelObj, PostingFlagModelObj, CurrentODDate, UpdatedStatus, NewStatus, ref ReturnMessage);
                                                //EARLIER CODE COMMENTED BY SHRADDHA AND ADDED NEW FUNCTION CalculateODWorkingHours ON 22 NOV 2017 TO GET GENERIC CODE END

                                                #endregion

                                                //CODE ADDED BY SHRADDHA ON 05 APR 2017 TO SOLVE LATECOUNT PROBLEM AT OD SANCTION

                                                WetosDB.SaveChanges(); // Verify?

                                                DateTime FirstDate = new DateTime(DailyTransactionObj.TranDate.Year, DailyTransactionObj.TranDate.Month, 1);
                                                DateTime LastDate = FirstDate.AddMonths(1).AddDays(-1);

                                                // Updated by Rajas on 29 AUGUST 2017 START
                                                List<DailyTransaction> AllEmpLatetData = WetosDB.DailyTransactions.Where(a => a.TranDate >= DailyTransactionObj.TranDate
                                              && a.TranDate <= LastDate && a.EmployeeId == DailyTransactionObj.EmployeeId).ToList();  // && (a.Remark.ToUpper() == "LATE")

                                                List<DailyTransaction> AllEmpEarlyData = WetosDB.DailyTransactions.Where(a => a.TranDate >= DailyTransactionObj.TranDate
                                             && a.TranDate <= LastDate && a.EmployeeId == DailyTransactionObj.EmployeeId).ToList();  // && (a.Remark.ToUpper() == "EARLY") 
                                                // Updated by Rajas on 29 AUGUST 2017 END

                                                //Code Added By Shraddha on 06 JUNE 2017 To Get LateCount from Rules start
                                                int EmployeeGroupIdObj = WetosDB.EmployeeGroupDetails.Where(a => a.Employee.EmployeeId == DailyTransactionObj.EmployeeId)
                                                    .Select(a => a.EmployeeGroup.EmployeeGroupId).FirstOrDefault();
                                                List<RuleTransaction> RuleTransactionList = WetosDB.RuleTransactions.Where(a => a.CompanyId == DailyTransactionObj.CompanyId && a.BranchId == DailyTransactionObj.BranchId && a.EmployeeGroupId == EmployeeGroupIdObj).ToList();
                                                RuleTransaction RuleForLateCount = RuleTransactionList.Where(a => a.RuleId == 16).FirstOrDefault();

                                                //Code Added By Shraddha on 06 JUNE 2017 To Get LateCount from Rules end

                                                #region CODE TO DEDUCT ALREADY MARKED LATE COUNT

                                                NewStatus = string.Empty;
                                                UpdatedStatus = string.Empty;
                                                ReturnMessage = string.Empty;

                                                // Updated by Rajas on 5 AUGUST 2017 START
                                                if (AllEmpLatetData != null)
                                                {
                                                    foreach (DailyTransaction DailyTransObj in AllEmpLatetData)
                                                    {
                                                        if (RuleForLateCount != null)
                                                        {

                                                            if (DailyTransObj.LateCount >= 1)
                                                            {
                                                                DailyTransObj.LateCount = DailyTransObj.LateCount - 1;
                                                                WetosDB.SaveChanges();

                                                                PostingFlagModelObj.IsLateCountReduced = true;

                                                                if (DailyTransObj.LateCount <= Convert.ToInt32(RuleForLateCount.Formula))
                                                                {
                                                                    WetosAdministrationController.GetActualDailyTransactionStatus(PostingFlagModelObj, DailyTransactionObj.Status, NewStatus, ref UpdatedStatus, ref ReturnMessage);
                                                                    DailyTransObj.Remark = string.Empty;  // Updated by Rajas on 10 AUGUST 2017
                                                                    DailyTransObj.Status = UpdatedStatus;
                                                                    WetosDB.SaveChanges();
                                                                }

                                                                else if (DailyTransObj.LateCount > Convert.ToInt32(RuleForLateCount.Formula))
                                                                {
                                                                    //abcd.Status = "AAPP^";
                                                                    WetosAdministrationController.GetActualDailyTransactionStatus(PostingFlagModelObj, DailyTransactionObj.Status, NewStatus, ref UpdatedStatus, ref ReturnMessage);
                                                                    DailyTransObj.Remark = "Late";
                                                                    WetosDB.SaveChanges();
                                                                }
                                                            }
                                                            // > 2 ABSENT
                                                            // ==2 PRESENT 

                                                            // Initialise flags
                                                            PostingFlagModelObj = new PostingFlagModel();  // Added by Rajas on 8 AUGUST 2017
                                                        }
                                                    }
                                                }
                                                // Updated by Rajas on 5 AUGUST 2017 END

                                                #endregion

                                                #region CODE TO DEDUCT ALREADY MARKED EARLY COUNT

                                                NewStatus = string.Empty;
                                                UpdatedStatus = string.Empty;
                                                ReturnMessage = string.Empty;
                                                if (AllEmpEarlyData != null)
                                                {
                                                    // Updated by Rajas on 5 AUGUST 2017 START
                                                    foreach (DailyTransaction DailyTransObj in AllEmpEarlyData)
                                                    {
                                                        if (RuleForLateCount != null)
                                                        {
                                                            if (DailyTransObj.EarlyCount >= 1)
                                                            {
                                                                DailyTransObj.EarlyCount = DailyTransObj.EarlyCount - 1;
                                                                WetosDB.SaveChanges();

                                                                PostingFlagModelObj.IsLateCountReduced = true;

                                                                if (DailyTransObj.LateCount <= Convert.ToInt32(RuleForLateCount.Formula))
                                                                {
                                                                    WetosAdministrationController.GetActualDailyTransactionStatus(PostingFlagModelObj, DailyTransactionObj.Status, NewStatus, ref UpdatedStatus, ref ReturnMessage);
                                                                    DailyTransObj.Remark = string.Empty;  // Updated by Rajas on 10 AUGUST 2017
                                                                    DailyTransObj.Status = UpdatedStatus;
                                                                    WetosDB.SaveChanges();
                                                                }

                                                                else if (DailyTransObj.EarlyCount > Convert.ToInt32(RuleForLateCount.Formula))
                                                                {
                                                                    //abcd.Status = "AAPP^";
                                                                    WetosAdministrationController.GetActualDailyTransactionStatus(PostingFlagModelObj, DailyTransactionObj.Status, NewStatus, ref UpdatedStatus, ref ReturnMessage);
                                                                    DailyTransObj.Remark = "Late";
                                                                    WetosDB.SaveChanges();
                                                                }
                                                            }
                                                            // > 2 ABSENT
                                                            // ==2 PRESENT 

                                                            // Initialise flags
                                                            PostingFlagModelObj = new PostingFlagModel();  // Added by Rajas on 8 AUGUST 2017
                                                        }
                                                    }
                                                }
                                                // Updated by Rajas on 5 AUGUST 2017 END

                                                #endregion

                                                //ADDED CODE FOR ADDING HALF DAY AND FULL DAY OD WISE STATUS IN DAILY TRANSACTION TABLE BY SHRADDHA ON 13 FEB 2017 END

                                                WetosDB.SaveChanges();
                                            }

                                        }

                                        else
                                        {
                                            Information("You can not sanction this OD/Travel as you are not OD/Travel sanctioner for selected employee");

                                            List<SP_WetosGetEmployeeODTravelSanction_Result> ODTObj = WetosDB.SP_WetosGetEmployeeODTravelSanction(EmpNo).OrderByDescending(a => a.FromDate).OrderByDescending(a => a.ToDate).ToList();
                                            //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 START
                                            PopulateDropDown();
                                            //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 END

                                            return View(ODTObj);
                                        }
                                    }
                                    //CODE ADDED BY SHRADDHA ON 13 FEB 2017 CONDITION ADDED FOR TODATE START
                                    else if (item.StatusId == 3 || item.StatusId == 6 || item.StatusId == 5)
                                    {
                                        if (DailyTransactionObj != null)
                                        {
                                            DailyTransactionObj.Status = DailyTransactionObj.ActualStatus; // CODE ADDED BY SHRADDHA ON 11 SEP 2017 START
                                            WetosDB.SaveChanges();
                                        }
                                        ODTravelObj.RejectReason = RejectReasonText;
                                        WetosDB.SaveChanges();

                                    }

                                        //UPDATED BY SHRADDHA ON 11 SEP 2017 TAKEN STATUSID = 5
                                    else if (item.StatusId == 5)   // Added by Rajas on 9 JULY 2017 to Fix Issue no. 2, defect id= FB0012 as per Test Cases sheet
                                    {
                                        if (DailyTransactionObj != null)
                                        {
                                            DailyTransactionObj.Status = DailyTransactionObj.ActualStatus;// CODE ADDED BY SHRADDHA ON 11 SEP 2017 START
                                            WetosDB.SaveChanges();
                                        }
                                        // Cancellation reason code
                                        // Added by Rajas on 12 AUGUST 2017 START
                                        if (RejectReasonText == null || RejectReasonText == string.Empty)
                                        {
                                            ModelState.AddModelError("CustomError", "Please Enter Cancellation Reason");

                                            //List<SP_WetosGetEmployeeODTravelSanction_Result> ODTravelSanctionList = WetosDB.SP_WetosGetEmployeeODTravelSanction(EmpNo).OrderByDescending(a => a.FromDate).OrderByDescending(a => a.ToDate).ToList();

                                            // Added by Rajas on 9 MAY 2017
                                            // Populate Status dropdown
                                            PopulateDropDown();

                                            Error("Please enter  Cancellation Reason");

                                            return View(ODTravelSanction); // ODTravelSanctionList
                                        }
                                        // Added by Rajas on 12 AUGUST 2017 END

                                        if (DailyTransactionObj != null)
                                        {
                                            // Updated by Rajas on 5 AUGUST 2017, restored Actual status 
                                            if (DailyTransactionObj != null)
                                            {
                                                DailyTransactionObj.Status = DailyTransactionObj.ActualStatus; // "AAAA"
                                                WetosDB.SaveChanges();
                                            }
                                        }
                                    }
                                }

                                // ADDED BY MSJ ON 09 JAN 2019 START
                                ODTravelObjEDIT.AppliedBy = Convert.ToInt32(Session["EmployeeNo"]);
                                ODTravelObjEDIT.AppliedOn = DateTime.Now;
                                ODTravelObjEDIT.SanctionedBy = Convert.ToInt32(Session["EmployeeNo"]);
                                ODTravelObjEDIT.SanctionedOn = DateTime.Now;
                                // ADDED BY MSJ ON 09 JAN 2019 END

                                //CODE ADDED BY SHRADDHA ON 13 FEB 2017 CONDITION ADDED FOR TODATE START
                                WetosDB.SaveChanges();


                                //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 12 SEPTEMBER 2017
                                #region ADD AUDIT LOG
                                string Oldrecord = "ODTourType : " + ODTravelObjEDIT.ODTourType + ", FromDate : " + ODTravelObjEDIT.FromDate
                                   + ", ToDate : " + ODTravelObjEDIT.ToDate + ", ODDayStatus : " + ODTravelObjEDIT.ODDayStatus
                                   + ", ODDayStatus1 : " + ODTravelObjEDIT.ODDayStatus1 + ", AppliedDays :" + ODTravelObjEDIT.AppliedDay + ", ActualDays :"
                                   + ODTravelObjEDIT.ActualDay + ", Purpose :" + ODTravelObjEDIT.Purpose + ", Status :" + ODTravelObjEDIT.Status
                                   + ", BranchId :" + ODTravelObjEDIT.BranchId
                                   + ", CompanyId :" + ODTravelObjEDIT.CompanyId;
                                //---x---------x-----------------x------------x----------------x-----------x-------------x------------x--------------x----------x---------x-----
                                string Newrecord = "ODTourType : " + ODTravelObj.ODTourType + ", FromDate : " + ODTravelObj.FromDate
                                   + ", ToDate : " + ODTravelObj.ToDate + ", ODDayStatus : " + ODTravelObj.ODDayStatus
                                   + ", ODDayStatus1 : " + ODTravelObj.ODDayStatus1 + ", AppliedDays :" + ODTravelObj.AppliedDay + ", ActualDays :"
                                   + ODTravelObj.ActualDay + ", Purpose :" + ODTravelObj.Purpose + ", Status :" + ODTravelObj.Status
                                   + ", BranchId :" + ODTravelObj.BranchId
                                   + ", CompanyId :" + ODTravelObj.CompanyId;

                                //LOGIN ID TAKEN FROM SESSION PERSISTER
                                string Formname = "REQUISITION SANCTION";
                                //ACTION IS UPDATE
                                string Message = " ";

                                WetosAdministrationController.GenerateAuditLogsUpdate(WetosDB, ODTravelObj.EmployeeId, Formname, Oldrecord,
                                    Newrecord, ref Message);
                                #endregion
                                //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 12 SEPTEMBER 2017


                                #region PROCESS OD/TRAVEL APPLICATION

                                // Updated by Rajas on 5 AUGUST 2017
                                string ErrorMessage = string.Empty;
                                if (WetosAdministrationController.ODOTProcessingEx(WetosDB, Convert.ToDateTime(item.FromDate), Convert.ToDateTime(item.ToDate), ODTravelObj.EmployeeId, ref ErrorMessage) == false)
                                {
                                    AddAuditTrail(ErrorMessage);
                                }

                                #endregion

                                // Added by Rajas on 20 MAY 2017
                                // Common POSTING code
                                //ProcessAttendance(item.FromDate.Value, item.ToDate.Value);

                                // NOTIFICATION ADDED BY RAJAS ON 31 DEC 2016
                                #region OD SANCTION NOTIFICATION

                                // Notification from Reporting person to Employee
                                Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == ODTravelObj.EmployeeId).FirstOrDefault();
                                Notification NotificationObj = new Notification();
                                NotificationObj.FromID = EmployeeObj.EmployeeReportingId;
                                NotificationObj.ToID = EmployeeObj.EmployeeId;
                                NotificationObj.SendDate = DateTime.Now;
                                string StatusName = WetosDB.DropdownDatas.Where(a => a.TypeId == 3 && a.Value == item.StatusId).Select(a => a.Text).FirstOrDefault();
                                NotificationObj.NotificationContent = "Your OD/Travel application from " + ODTravelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " to " + ODTravelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " is " + StatusName;
                                //NotificationObj.NotificationContent = "Leave applied from " + LeaveApplicationObj.FromDate.ToString("dd-MMM-yyyy") + " to " + LeaveApplicationObj.ToDate.ToString("dd-MMM-yyyy") + " is " + LeaveApplicationObj.Status;
                                NotificationObj.ReadFlag = false;
                                NotificationObj.SendDate = DateTime.Now;

                                WetosDB.Notifications.AddObject(NotificationObj);

                                WetosDB.SaveChanges();

                                if (ODTravelObj.StatusId == 4 && ValidEmpObj.EmployeeReportingId == EmpNo)
                                {

                                    // Notification from Reporting person to Employee
                                    Notification NotificationObj2 = new Notification();
                                    NotificationObj.FromID = EmployeeObj.EmployeeId;
                                    NotificationObj.ToID = EmployeeObj.EmployeeReportingId2;
                                    NotificationObj.SendDate = DateTime.Now;
                                    NotificationObj.NotificationContent = "Received OD/Travel application for sanctioning from " + EmployeeObj.FirstName + " " + EmployeeObj.LastName + " - from " + ODTravelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " to " + ODTravelObj.ToDate.Value.ToString("dd-MMM-yyyy");
                                    //NotificationObj.NotificationContent = "Leave applied from " + LeaveApplicationObj.FromDate.ToString("dd-MMM-yyyy") + " to " + LeaveApplicationObj.ToDate.ToString("dd-MMM-yyyy") + " is " + LeaveApplicationObj.Status;
                                    NotificationObj.ReadFlag = false;
                                    NotificationObj.SendDate = DateTime.Now;

                                    WetosDB.Notifications.AddObject(NotificationObj2);

                                    WetosDB.SaveChanges();

                                }

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
                                            if (SendEmail(EmployeeObj.Email, NotificationObj.NotificationContent, NotificationObj.NotificationContent, ref EmailUpdateStatus, "OD/Travel Sanction") == false)
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
                            }
                            else
                            {
                                Error("Inconsistent data, Please try again!!");

                                return View("Error");
                            }
                        }
                    }
                    // int EmpNo = Convert.ToInt32(Session["EmployeeNo"]);

                    //List<SP_WetosGetEmployeeLeaveSanction_Result> LeaveSanctionObj1 = WetosDB.SP_WetosGetEmployeeLeaveSanction(EmpNo).ToList();

                    List<SP_WetosGetEmployeeODTravelSanction_Result> ODTOURObj = WetosDB.SP_WetosGetEmployeeODTravelSanction(EmpNo).OrderByDescending(a => a.FromDate).OrderByDescending(a => a.ToDate).ToList();

                    // ADDED BY PUSHKAR FOR AuditLog ON 21 DEC 2016
                    AddAuditTrail("Success - OD/Travel application processed successfully");  // Updated by Rajas on 17 JAN 2017

                    // Added by Rajas on 17 JAN 2017 START
                    Success("Success - OD/Travel application processed successfully");

                    // Added by Rajas on 9 MAY 2017
                    // Populate Status dropdown
                    //REPLACED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 START
                    //List<string> StatusObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 3).Select(a => a.Text).ToList();
                    //ViewBag.StatusList = StatusObj;
                    PopulateDropDown();
                    //REPLACED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 END

                    return View(ODTOURObj);
                }
            }

            catch (System.Exception ex)
            {
                AddAuditTrail("Error in OD/Travel due to " + ex.Message + ( ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                Error("Please try again!!");

                //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 START
                PopulateDropDown();
                //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 END
                return View("Error");
            }
        }



        /// <summary>
        /// Updated by Rajas on 30 MARCH 2017
        /// GET
        /// </summary>
        /// <returns></returns>
        public ActionResult LeaveSanctionIndex(int selectCriteria = 0)
        {
            try
            {
                int EmpNo = Convert.ToInt32(Session["EmployeeNo"]);
                int Id = Convert.ToInt32(Session["Id"]);

                // Added by Rajas on 9 MAY 2017 START
                // Select Criteria as per dropdown value
                int Status = 0;

                // Updated by Rajas on 20 MAY 2017 START
                List<SP_LeaveSanctionIndex_Result> LeaveSanctionList = new List<SP_LeaveSanctionIndex_Result>();

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

                    return View(LeaveSanctionList);
                }

                DateTime CalanderStartDate = GetFYStartDate(); // DateTime CalanderStartDate = WetosDB.FinancialYears.Where(a => a.FinancialName == GlobalSettingObj.SettingValue).Select(a => a.StartDate).FirstOrDefault();

                if (CalanderStartDate != null)
                {

                    //CODE REPLACED BY SHRADDHA ON 12 SEP 2017 REMOVED REDUNDANT AND UNNECESSARY IF ELSE CODE AND TAKEN ONE SINGLE LINE CODE START
                    Status = selectCriteria;
                    LeaveSanctionList = WetosDB.SP_LeaveSanctionIndex(EmpNo).Where(a => ((a.EmployeeReportingId == EmpNo || a.EmployeeReportingId2 == EmpNo) && a.Id == Status && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null))
                            .OrderByDescending(a => a.FromDate).OrderByDescending(a => a.ToDate).ToList();
                    //CODE REPLACED BY SHRADDHA ON 12 SEP 2017 REMOVED REDUNDANT AND UNNECESSARY IF ELSE CODE AND TAKEN ONE SINGLE LINE CODE END

                }
                #endregion

                // Updated by Rajas on 20 MAY 2017 END
                ViewBag.Status = Status;
                ViewBag.SelectedStatus = selectCriteria;
                // Added by Rajas on 9 MAY 2017 END

                #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //var EmployeeData = WetosDB.VwActiveEmployees.ToList();
                List<SP_VwActiveEmployee_Result> EmployeeData = WetosDB.SP_VwActiveEmployee(EmployeeId).ToList();
                #endregion
                // List<SP_WetosGetEmployeeLeaveSanction_Result> LeaveSanctionObj = WetosDB.SP_WetosGetEmployeeLeaveSanction(EmpNo).OrderByDescending(a => a.FromDate).OrderByDescending(a => a.ToDate).ToList();

                //List<SP_LeaveSanctionIndex_Result> LeaveSanctionList = WetosDB.SP_LeaveSanctionIndex(EmpNo).Where(a => (a.EmployeeReportingId == EmpNo && a.Id == Status) || a.EmployeeReportingId2 == EmpNo && a.Id == Status).OrderByDescending(a => a.FromDate).OrderByDescending(a => a.ToDate).ToList();  // && a.Id == 4)

                // Added by Rajas on 9 MAY 2017
                // Populate Status dropdown
                //REPLACED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 START
                //List<string> StatusObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 3).Select(a => a.Text).ToList();
                //ViewBag.StatusList = StatusObj;
                PopulateDropDown();
                //REPLACED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 END

                return View(LeaveSanctionList);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in Leave Sanction due to " + ex.Message + ( ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                Error("Error in Leave Sanction due to " + ex.Message + ( ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 START
                PopulateDropDown();
                //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 END

                return RedirectToAction("EmployeeDashboard", "Dashboard");
            }
        }


        [HttpPost]
        public ActionResult LeaveSanctionPV(int selectCriteria = 0)
        {
            try
            {
                int EmpNo = Convert.ToInt32(Session["EmployeeNo"]);
                int Id = Convert.ToInt32(Session["Id"]);

                // Added by Rajas on 9 MAY 2017 START
                // Select Criteria as per dropdown value
                int Status = 0;

                // Updated by Rajas on 20 MAY 2017 START
                List<SP_LeaveSanctionIndex_Result> LeaveSanctionList = new List<SP_LeaveSanctionIndex_Result>();

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

                    return View(LeaveSanctionList);
                }

                DateTime CalanderStartDate = GetFYStartDate(); // DateTime CalanderStartDate = WetosDB.FinancialYears.Where(a => a.FinancialName == GlobalSettingObj.SettingValue).Select(a => a.StartDate).FirstOrDefault();

                if (CalanderStartDate != null)
                {

                    //CODE REPLACED BY SHRADDHA ON 12 SEP 2017 REMOVED REDUNDANT AND UNNECESSARY IF ELSE CODE AND TAKEN ONE SINGLE LINE CODE START
                    Status = selectCriteria;
                    LeaveSanctionList = WetosDB.SP_LeaveSanctionIndex(EmpNo).Where(a => ((a.EmployeeReportingId == EmpNo) && a.Id == Status && a.FromDate >= CalanderStartDate && (a.MarkedAsDelete == 0 || a.MarkedAsDelete == null)))
                            .OrderByDescending(a => a.FromDate).OrderByDescending(a => a.ToDate).ToList();
                    //CODE REPLACED BY SHRADDHA ON 12 SEP 2017 REMOVED REDUNDANT AND UNNECESSARY IF ELSE CODE AND TAKEN ONE SINGLE LINE CODE END


                }
                #endregion

                // Updated by Rajas on 20 MAY 2017 END
                ViewBag.Status = Status;
                ViewBag.SelectedStatus = selectCriteria;
                // Added by Rajas on 9 MAY 2017 END


                #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //var EmployeeData = WetosDB.VwActiveEmployees.ToList();
                List<SP_VwActiveEmployee_Result> EmployeeData = WetosDB.SP_VwActiveEmployee(EmployeeId).ToList();
                #endregion
                // List<SP_WetosGetEmployeeLeaveSanction_Result> LeaveSanctionObj = WetosDB.SP_WetosGetEmployeeLeaveSanction(EmpNo).OrderByDescending(a => a.FromDate).OrderByDescending(a => a.ToDate).ToList();

                //List<SP_LeaveSanctionIndex_Result> LeaveSanctionList = WetosDB.SP_LeaveSanctionIndex(EmpNo).Where(a => (a.EmployeeReportingId == EmpNo && a.Id == Status) || a.EmployeeReportingId2 == EmpNo && a.Id == Status).OrderByDescending(a => a.FromDate).OrderByDescending(a => a.ToDate).ToList();  // && a.Id == 4)

                // Added by Rajas on 9 MAY 2017
                // Populate Status dropdown
                //REPLACED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 START
                //List<string> StatusObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 3).Select(a => a.Text).ToList();
                //ViewBag.StatusList = StatusObj;
                PopulateDropDown();
                //REPLACED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 END

                return PartialView(LeaveSanctionList);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in Leave Sanction due to " + ex.Message + ( ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                Error("Error in Leave Sanction due to " + ex.Message + ( ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 START
                PopulateDropDown();
                //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 END

                return RedirectToAction("EmployeeDashboard", "Dashboard");
            }
        }



        /// <summary>
        /// ADDED BY SHRADDHA ON 22 JULY 2017
        /// FUNCTION FOR FAKE LOGIC ADDED BY MSJ
        /// BECAUSE WITHOUT THIS CODE LEAVE SANCTION NOT WORKING INCASE OF FILTER
        /// <param name="EmpNo"></param>
        /// <param name="LeaveSanction"></param>
        /// <param name="FC"></param>
        /// <returns></returns>
        public static List<SP_LeaveSanctionIndex_Result> LeaveSanctionFakeLogic(WetosDBEntities WetosDB, int EmpNo, List<SP_LeaveSanctionIndex_Result> LeaveSanction, FormCollection FC)
        {

            LeaveSanction = new List<SP_LeaveSanctionIndex_Result>();

            int SingleApplicationID = 0;
            int SingleStatusID = 0;

            foreach (var key in FC.AllKeys)
            {
                string KeyStr = key.ToString();

                if (KeyStr.Contains("StatusId") == true)
                {
                    SingleStatusID = Convert.ToInt32(FC[KeyStr]);

                    string TempKeyStr = KeyStr.Replace("StatusId", "LeaveApplicationId");

                    SingleApplicationID = Convert.ToInt32(FC[TempKeyStr]);
                    SP_LeaveSanctionIndex_Result TesmpLeaveSanction = WetosDB.SP_LeaveSanctionIndex(EmpNo).Where(a => a.LeaveApplicationId == SingleApplicationID).FirstOrDefault(); //.ToList();

                    if (TesmpLeaveSanction != null)
                    {
                        TesmpLeaveSanction.StatusId = SingleStatusID;// UPDATE STATUS
                        LeaveSanction.Add(TesmpLeaveSanction);
                    }

                }

            }

            return LeaveSanction;
        }

        [HttpPost]
        public ActionResult LeaveSanctionIndex(List<SP_LeaveSanctionIndex_Result> LeaveSanction, FormCollection FC)
        {
            int EmpNo = Convert.ToInt32(Session["EmployeeNo"]);
            try
            {
                LeaveSanction = null; // new List<SP_LeaveSanctionIndex_Result>();

                // FAKE LOGIC MSJ
                #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //var EmployeeData = WetosDB.VwActiveEmployees.ToList();
                List<SP_VwActiveEmployee_Result> EmployeeData = WetosDB.SP_VwActiveEmployee(EmployeeId).ToList();
                #endregion

                List<SP_LeaveSanctionIndex_Result> LeaveSanctionObj = new List<SP_LeaveSanctionIndex_Result>();

                string RejectReasonText = FC["RejectReasonText"];

                #region FAKE LOGIC

                LeaveSanction = WetosLeaveTransactionController.LeaveSanctionFakeLogic(WetosDB, EmpNo, LeaveSanction, FC);
                if (LeaveSanction.Count == 0)
                {
                    Information("Please select atleast one entry for processing");
                    PopulateDropDown();
                    return View(LeaveSanction);
                }
                #endregion


                LeaveSanctionObj = LeaveSanction.Where(a => a.StatusId == 3 || a.StatusId == 5 || a.StatusId == 6).ToList(); // ADDED BY SHRADDHA ON 12 SEP 2017 FOR STATUSID = 6

                #region VALIDATION REJECT REASON IS COMPULSORY INCASE OF REJECTING LEAVE APPLICATION
                if (LeaveSanctionObj.Count() > 0 && (RejectReasonText == null || RejectReasonText == ""))
                {
                    ModelState.AddModelError("CustomError", "Please Enter Reject/Cancel Reason");
                    Error("Please Enter Reject/Cancel Reason");
                    PopulateDropDown();
                    return View(LeaveSanction);
                }


                #endregion
                else
                {
                    foreach (var i in LeaveSanctionObj)
                    {
                        LeaveApplication LeaveApplicationObj = WetosDB.LeaveApplications.Where(a => a.LeaveApplicationId == i.LeaveApplicationId).FirstOrDefault();

                        LeaveApplicationObj.RejectReason = RejectReasonText;
                    }

                    foreach (SP_LeaveSanctionIndex_Result item in LeaveSanction)
                    {
                        LeaveApplication LeaveApplicationObj = WetosDB.LeaveApplications.Where(a => a.LeaveApplicationId == item.LeaveApplicationId).FirstOrDefault();

                        //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 9 SEPTEMBER 2017 FOR EDIT
                        LeaveApplication LeaveApplicationObjEDIT = WetosDB.LeaveApplications.Where(a => a.LeaveApplicationId == item.LeaveApplicationId).FirstOrDefault();

                        // Validate Valid employee from obj
                        SP_VwActiveEmployee_Result ValidEmpObj = EmployeeData.Where(a => a.EmployeeId == LeaveApplicationObj.EmployeeId).FirstOrDefault();

                        if (item.StatusId != 0)
                        {
                            LeaveBalance LeaveBalanceObj = WetosDB.LeaveBalances.Where(a => a.EmployeeId == LeaveApplicationObj.EmployeeId && a.LeaveType == LeaveApplicationObj.LeaveType).FirstOrDefault();

                            //Added By Shraddha on 14 DEC 2016 for reduce No. Of Leave Days from LeaveBalance Table after sanctioning the leaves start
                            if (LeaveApplicationObj.StatusId != 2 && item.StatusId == 2)
                            {
                                #region ALLOW TO SANCTION LEAVE ONLY IF LOGIN EMPLOYEE IS REPORTING PERSON OF THE EMPLOYEE WHOM LEAVE IS BEING SANCTIONED
                                if (ValidEmpObj.EmployeeReportingId2 == EmpNo || ValidEmpObj.EmployeeReportingId == EmpNo)
                                {


                                    #region FUNCTION IsSandwichforLeaveSanction CALL TO CHECK WHETHER HAVING SANCTIONED SANDWICH LEAVES OR NOT
                                    // 3. IS SC
                                    // Added by SHRADDHA on 24 JULY 2017
                                    // Static IsSandwich function
                                    string ErrorMessage = string.Empty;
                                    WetosLeaveTransactionController.IsSandwichforLeaveSanction(WetosDB, LeaveApplicationObj.EmployeeId, ref ErrorMessage, ref LeaveApplicationObj);
                                    #endregion

                                    #region GET ALLOWED LEAVES
                                    //CODE TO GET ALLOWED LEAVES ON SANCTION TIME START
                                    var AllowedLeaves = 0D;
                                    DateTime StartDate = new DateTime(LeaveApplicationObj.ToDate.Year, LeaveApplicationObj.ToDate.Month, 1);
                                    DateTime LeaveAsOnDate = StartDate.AddMonths(1).AddDays(-1);
                                    SP_LeaveTableData_Result LeaveDataObj = WetosEmployeeController.GetLeavedataNewLogic(WetosDB, LeaveApplicationObj.EmployeeId, LeaveAsOnDate).Where(a => a.LeaveType.ToUpper().Trim() == LeaveApplicationObj.LeaveType.ToUpper().Trim()).FirstOrDefault();

                                    // MODIFIED BY MSJ ON 08 DEC 2017
                                    AllowedLeaves = LeaveDataObj.CurrentBalance == null ? 0.0 : LeaveDataObj.CurrentBalance.Value;
                                    if (AllowedLeaves > LeaveApplicationObj.TotalDeductableDays)
                                    {
                                        LeaveApplicationObj.ActualDays = LeaveApplicationObj.TotalDeductableDays;
                                    }
                                    else
                                    {
                                        LeaveApplicationObj.ActualDays = AllowedLeaves;
                                    }
                                    LeaveApplicationObj.RejectReason = string.Empty;
                                    WetosDB.SaveChanges();
                                    //CODE TO GET ALLOWED LEAVES ON SANCTION TIME END
                                    #endregion

                                    #region TO UPDATE LEAVE DETAILS IN LEAVEBALANCE TABLE
                                    if (LeaveBalanceObj != null)
                                    {
                                        var UpdatedBalance = LeaveBalanceObj.CurrentBalance - LeaveApplicationObj.ActualDays;  // AppliedDays
                                        var LeaveUsedObj = (LeaveBalanceObj.LeaveUsed == null ? 0 : LeaveBalanceObj.LeaveUsed) + LeaveApplicationObj.ActualDays;
                                        LeaveBalanceObj.PreviousBalance = LeaveBalanceObj.CurrentBalance;
                                        LeaveBalanceObj.CurrentBalance = UpdatedBalance;
                                        LeaveBalanceObj.LeaveUsed = LeaveUsedObj;
                                        WetosDB.SaveChanges(); //ADDED BY SHRADDHA ON 22 JULY 2017 BECAUSE I THINK THIS LINE SHOULD BE HERE TO SAVE CHANGES. NEED TO CHECK AND VERIFY
                                    }
                                    #endregion

                                }
                                else
                                {
                                    Information("You can not sanction this leave as you are not Leave sanctioner for selected employee");

                                    //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 START
                                    PopulateDropDown();
                                    //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 END

                                    return View(LeaveSanction);
                                }
                                #endregion

                                if (LeaveApplicationObj.StatusId == 2 && (item.StatusId == 3 || item.StatusId == 5 || item.StatusId == 6)) // ADDED BY SHRADDHA ON 12 SEP 2017 FOR STATUSID = 6 //Added item.statusid=5 logic by shraddha on 11 sep 2017 to update status as cancelled start
                                {
                                    #region TO UPDATE LEAVE DETAILS IN LEAVEBALANCE TABLE
                                    if (LeaveBalanceObj != null)
                                    {
                                        var UpdatedBalance = LeaveBalanceObj.CurrentBalance + LeaveApplicationObj.ActualDays;  // AppliedDays
                                        var LeaveUsedObj = (LeaveBalanceObj.LeaveUsed == null ? 0 : LeaveBalanceObj.LeaveUsed) + LeaveApplicationObj.ActualDays;

                                        LeaveBalanceObj.CurrentBalance = UpdatedBalance;
                                        LeaveBalanceObj.LeaveUsed = LeaveUsedObj;
                                        LeaveBalanceObj.PreviousBalance = LeaveBalanceObj.PreviousBalance - LeaveApplicationObj.ActualDays;
                                        WetosDB.SaveChanges(); //ADDED BY SHRADDHA ON 22 JULY 2017 BECAUSE I THINK THIS LINE SHOULD BE HERE TO SAVE CHANGES. NEED TO CHECK AND VERIFY
                                    }
                                    #endregion
                                    for (DateTime CurrentDate = LeaveApplicationObj.FromDate; CurrentDate.Date <= LeaveApplicationObj.ToDate; CurrentDate = CurrentDate.AddDays(1))
                                    {
                                        DailyTransaction DailyTransactionObj = WetosDB.DailyTransactions.Where(a => a.EmployeeId == LeaveApplicationObj.EmployeeId && a.TranDate == CurrentDate).FirstOrDefault();
                                        if (DailyTransactionObj != null)
                                        {

                                            DailyTransactionObj.Status = DailyTransactionObj.ActualStatus;
                                            WetosDB.SaveChanges();
                                        }
                                    }
                                }
                                #region CODE TO PREVENT SANCTIONING LEAVE WHICH IS ALREADY SANCTIONED
                                else if (LeaveApplicationObj.StatusId == 2 && item.StatusId == 2)
                                {
                                    Error("Leave is already Sanction for leave " + LeaveApplicationObj.FromDate.ToString("dd/MMM/yyyy") + " - " + LeaveApplicationObj.ToDate.ToString("dd/MMM/yyyy") + " For EmployeeCode: " + ValidEmpObj.EmployeeCode + "-" + ValidEmpObj.FirstName + " " + ValidEmpObj.MiddleName + " " + ValidEmpObj.LastName);

                                    //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 START
                                    PopulateDropDown();
                                    //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 END

                                    return View(LeaveSanction);
                                }
                                #endregion
                            }

                            // Added by Rajas on 14 APRIL 2017, case where both reporting person are same
                            // Updated by Rajas on 7 JUNE 2017 || replaced  with &&
                            if (item.StatusId == 4 && (ValidEmpObj.EmployeeReportingId == EmpNo && ValidEmpObj.EmployeeReportingId2 == EmpNo))
                            {
                                LeaveApplicationObj.RejectReason = string.Empty;
                                #region TO UPDATE LEAVE DETAILS IN LEAVEBALANCE TABLE
                                if (LeaveBalanceObj != null)
                                {
                                    var UpdatedBalance = LeaveBalanceObj.CurrentBalance - LeaveApplicationObj.ActualDays;  // AppliedDays //Replaced AppliedDays with ActualDays by Shraddha on 18 JULY 2017
                                    var LeaveUsedObj = (LeaveBalanceObj.LeaveUsed == null ? 0 : LeaveBalanceObj.LeaveUsed) + LeaveApplicationObj.ActualDays; //Replaced AppliedDays with ActualDays by Shraddha on 18 JULY 2017
                                    LeaveBalanceObj.PreviousBalance = LeaveBalanceObj.CurrentBalance;
                                    LeaveBalanceObj.CurrentBalance = UpdatedBalance;
                                    LeaveBalanceObj.LeaveUsed = LeaveUsedObj;
                                    WetosDB.SaveChanges(); //ADDED BY SHRADDHA ON 22 JULY 2017 BECAUSE I THINK THIS LINE SHOULD BE HERE TO SAVE CHANGES. NEED TO CHECK AND VERIFY
                                }
                                #endregion
                                item.StatusId = 2; // Update Approve status to Sanctioned in case of both reporting person are same

                            }

                            /// Added by Rajas on 9 MAY 2017 START
                            /// If leave is sanctioned and employee require leave less no. of days than earlier applied, sanctioner will cancel leave and it will be return as CANCELLED to employee
                            /// Employee will NOT ABLE TO edit the same and NEED TO REAPPLY
                            /// Deducted leaves will be credited back and if punch is available then updated to AAAA
                            if (item.StatusId == 5 && ((ValidEmpObj.EmployeeReportingId2 == EmpNo || ValidEmpObj.EmployeeReportingId == EmpNo) && LeaveApplicationObj.StatusId == 2)) // Replaced StatusId 1 with 5 by Shraddha on 11 SEP 2017 start
                            {
                                #region TO UPDATE LEAVE DETAILS IN LEAVEBALANCE TABLE
                                if (LeaveBalanceObj != null)
                                {
                                    var UpdatedBalance = LeaveBalanceObj.CurrentBalance + LeaveApplicationObj.ActualDays;  // AppliedDays //Replaced AppliedDays with ActualDays by Shraddha on 18 JULY 2017
                                    var LeaveUsedObj = (LeaveBalanceObj.LeaveUsed == null ? 0 : LeaveBalanceObj.LeaveUsed) - LeaveApplicationObj.ActualDays;  //Replaced AppliedDays with ActualDays by Shraddha on 18 JULY 2017
                                    LeaveBalanceObj.CurrentBalance = UpdatedBalance;
                                    LeaveBalanceObj.LeaveUsed = LeaveUsedObj;
                                    LeaveBalanceObj.PreviousBalance = LeaveBalanceObj.PreviousBalance - LeaveApplicationObj.ActualDays;
                                    WetosDB.SaveChanges(); //ADDED BY SHRADDHA ON 22 JULY 2017 BECAUSE I THINK THIS LINE SHOULD BE HERE TO SAVE CHANGES. NEED TO CHECK AND VERIFY
                                }
                                #endregion
                                for (DateTime CurrentDate = LeaveApplicationObj.FromDate; CurrentDate.Date <= LeaveApplicationObj.ToDate; CurrentDate = CurrentDate.AddDays(1))
                                {
                                    DailyTransaction DailyTransactionObj = WetosDB.DailyTransactions.Where(a => a.EmployeeId == LeaveApplicationObj.EmployeeId && a.TranDate == CurrentDate).FirstOrDefault();
                                    if (DailyTransactionObj != null)
                                    {
                                        ///BELOW LINE COMMENTED AND ADDED BY SHRADDHA ON 22 JULY 2017 
                                        ///TO RETRIVE BACK OLD DAILYTRANSACTION STATUS AS DAILYTRANSACTION STATUS INSTEAD OF TAKING AAAA HARD CODED
                                        //DailyTransactionObj.Status = "AAAA";
                                        DailyTransactionObj.Status = DailyTransactionObj.ActualStatus;
                                        WetosDB.SaveChanges();
                                    }
                                }
                            }

                            LeaveApplicationObj.StatusId = item.StatusId;
                            LeaveApplicationObj.Status = WetosDB.DropdownDatas.Where(a => a.TypeId == 3 && a.Value == LeaveApplicationObj.StatusId).Select(a => a.Text).FirstOrDefault();

                            // ADDED BY MSJ ON 09 JAN 2019 START
                            LeaveApplicationObjEDIT.AppliedBy = Convert.ToInt32(Session["EmployeeNo"]);
                            LeaveApplicationObjEDIT.AppliedOn = DateTime.Now;
                            LeaveApplicationObjEDIT.SanctionedBy = Convert.ToInt32(Session["EmployeeNo"]);
                            LeaveApplicationObjEDIT.SanctionedOn = DateTime.Now;
                            // ADDED BY MSJ ON 09 JAN 2019 END

                            WetosDB.SaveChanges();


                            //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 12 SEPTEMBER 2017
                            #region ADD AUDIT LOG
                            string Oldrecord = "LeaveType : " + LeaveApplicationObjEDIT.LeaveType + ", FromDate : " + LeaveApplicationObjEDIT.FromDate
                                + ", ToDate : " + LeaveApplicationObjEDIT.ToDate + ", FromDayStatus : " + LeaveApplicationObjEDIT.FromDayStatus
                                + ", ToDayStatus :" + LeaveApplicationObjEDIT.ToDayStatus + ", AppliedDays :" + LeaveApplicationObjEDIT.AppliedDays + ", ActualDays :"
                                + LeaveApplicationObjEDIT.ActualDays + ", Purpose :" + LeaveApplicationObjEDIT.Purpose + ", Status :" + LeaveApplicationObjEDIT.Status
                                + ", BranchId :" + LeaveApplicationObjEDIT.BranchId + ", CompanyId :" + LeaveApplicationObjEDIT.CompanyId;
                            //---x---------x-----------------x------------x----------------x-----------x-------------x------------x--------------x----------x---------x-----
                            string Newrecord = "LeaveType : " + LeaveApplicationObj.LeaveType + ", FromDate : " + LeaveApplicationObj.FromDate
                                + ", ToDate : " + LeaveApplicationObj.ToDate + ", FromDayStatus : " + LeaveApplicationObj.FromDayStatus
                                + ", ToDayStatus :" + LeaveApplicationObj.ToDayStatus + ", AppliedDays :" + LeaveApplicationObj.AppliedDays + ", ActualDays :"
                                + LeaveApplicationObj.ActualDays + ", Purpose :" + LeaveApplicationObj.Purpose + ", Status :" + LeaveApplicationObj.Status
                                + ", BranchId :" + LeaveApplicationObj.BranchId + ", CompanyId :" + LeaveApplicationObj.CompanyId;

                            //LOGIN ID TAKEN FROM SESSION PERSISTER
                            string Formname = "LEAVE SANCTION";
                            //ACTION IS UPDATE
                            string Message = " ";

                            WetosAdministrationController.GenerateAuditLogsUpdate(WetosDB, LeaveApplicationObj.EmployeeId, Formname, Oldrecord,
                                Newrecord, ref Message);
                            #endregion
                            //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 12 SEPTEMBER 2017


                            /// FUNCTION CALL FOR POSTING 
                            /// ADDED BY RAJAS ON 18 MAY 2017
                            /// VERIFY
                            /// NOTE: NEED TO UPDATE POSTING FUNCTION CALL AS SINGLE CALL THROUGHOUT APPLICATION
                            //ProcessAttendance(item.FromDate, item.ToDate);
                            if (item.StatusId == 2) //ADDED IF CONDITION BY SHRADDHA ON 22 JULY 2017 BECAUSE THIS IS REQUIRED ONLY INCASE CASE OF SANCTIONING LEAVE (INCASE OF REJECT OR CANCEL WE SIMPLY OVERWRITE ACTUALSTATUS ON STATUS FIELD IN DT TABLE)
                            {
                                #region PROCESS LEAVE APPLICATION

                                // Updated by Rajas on 5 AUGUST 2017 for error entry in Audit
                                string ReturnMessage = string.Empty;
                                if (WetosAdministrationController.LeaveProcessingEx(WetosDB, item.FromDate, item.ToDate, LeaveApplicationObj.EmployeeId, ref ReturnMessage) == false)
                                {
                                    AddAuditTrail(ReturnMessage);
                                }

                                #endregion
                            }
                            // NOTIFICATION ADDED BY RAJAS ON 31 DEC 2016
                            #region LEAVE SANCTION NOTIFICATION

                            // Notification from Reporting person to Employee
                            Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == LeaveApplicationObj.EmployeeId).FirstOrDefault();
                            Notification NotificationObj = new Notification();
                            NotificationObj.FromID = EmployeeObj.EmployeeReportingId;
                            NotificationObj.ToID = EmployeeObj.EmployeeId;
                            NotificationObj.SendDate = DateTime.Now;
                            NotificationObj.NotificationContent = "Leave application from " + LeaveApplicationObj.FromDate.ToString("dd-MMM-yyyy") + " to " + LeaveApplicationObj.ToDate.ToString("dd-MMM-yyyy") + " is " + LeaveApplicationObj.Status;
                            //NotificationObj.NotificationContent = "Leave applied from " + LeaveApplicationObj.FromDate.ToString("dd-MMM-yyyy") + " to " + LeaveApplicationObj.ToDate.ToString("dd-MMM-yyyy") + " is " + LeaveApplicationObj.Status;
                            NotificationObj.ReadFlag = false;
                            NotificationObj.SendDate = DateTime.Now;

                            WetosDB.Notifications.AddObject(NotificationObj);

                            WetosDB.SaveChanges();

                            if (LeaveApplicationObj.StatusId == 4 && ValidEmpObj.EmployeeReportingId == EmpNo)
                            {

                                // Notification from Reporting person to Employee
                                Notification NotificationObj2 = new Notification();
                                NotificationObj.FromID = EmployeeObj.EmployeeId;
                                NotificationObj.ToID = EmployeeObj.EmployeeReportingId2;
                                NotificationObj.SendDate = DateTime.Now;
                                NotificationObj.NotificationContent = "Received Leave application for sanctioning from " + EmployeeObj.FirstName + " " + EmployeeObj.LastName + " - from " + LeaveApplicationObj.FromDate.ToString("dd-MMM-yyyy") + " to " + LeaveApplicationObj.ToDate.ToString("dd-MMM-yyyy");
                                //NotificationObj.NotificationContent = "Leave applied from " + LeaveApplicationObj.FromDate.ToString("dd-MMM-yyyy") + " to " + LeaveApplicationObj.ToDate.ToString("dd-MMM-yyyy") + " is " + LeaveApplicationObj.Status;
                                NotificationObj.ReadFlag = false;
                                NotificationObj.SendDate = DateTime.Now;

                                WetosDB.Notifications.AddObject(NotificationObj2);

                                WetosDB.SaveChanges();

                            }

                            // NOTIFICATION COUNT
                            //int NoTiCount = WetosDB.Notifications.Where(a => a.ToID == EmpNo && (a.ReadFlag == false || a.ReadFlag == null)).Count();
                            //Session["NotificationCount"] = NoTiCount;

                            #endregion

                            // Code updated by Rajas on 13 JUNE 2017
                            #region EMAIL
                            // Added by Rajas on 19 JULY 2017 START
                            //GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == "Send Email").FirstOrDefault();

                            //ABOVE LINE COMMENTED AND BELOW LINE IS ADDED BY SHALAKA ON 13TH DEC                             
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
                                        if (SendEmail(EmployeeObj.Email, NotificationObj.NotificationContent, NotificationObj.NotificationContent, ref EmailUpdateStatus, "Leave Sanction") == false)
                                        {
                                            Error(EmailUpdateStatus);

                                        }
                                    }
                                    else
                                    {
                                        AddAuditTrail("Unable to send email, as Email Id not available for " + EmployeeObj.EmployeeCode);
                                    }
                                }
                                else
                                {
                                    AddAuditTrail("Unable to send Email as email utility is disabled");
                                }
                            }
                            #endregion
                        }

                    }
                }
                List<SP_LeaveSanctionIndex_Result> LeaveSanctionListForAll = new List<SP_LeaveSanctionIndex_Result>();
                AddAuditTrail("Success - Leave application processed successfully");
                Success("Success - Leave application processed successfully");
                PopulateDropDown();
                return View(LeaveSanctionListForAll);

            }

            catch (System.Exception ex)
            {
                AddAuditTrail("Error - Leave Processing failed due to " + ex.Message + ( ex.InnerException == null ? string.Empty : ex.InnerException.Message));
                Error("Error - Leave Processing failed");
                PopulateDropDown();
                List<SP_LeaveSanctionIndex_Result> LeaveSanctionList = WetosDB.SP_LeaveSanctionIndex(EmpNo).OrderByDescending(a => a.FromDate).OrderByDescending(a => a.ToDate).ToList();
                return View(LeaveSanctionList);
            }

        }

       
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult CompOffIndex(int selectCriteria = 0)
        {
            try
            {
                int EmpNo = Convert.ToInt32(Session["EmployeeNo"]);
                int Id = Convert.ToInt32(Session["Id"]);

                // Added by Rajas on 9 MAY 2017 START
                // Select Criteria as per dropdown value
                int Status = 0;

                // Updated by Rajas on 20 MAY 2017 START
                List<SP_WetosGetEmployeeCompOffSanction_Result> CompOffSanctionObj = new List<SP_WetosGetEmployeeCompOffSanction_Result>();

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

                    return View(CompOffSanctionObj);
                }

                DateTime CalanderStartDate = GetFYStartDate(); // DateTime CalanderStartDate = WetosDB.FinancialYears.Where(a => a.FinancialName == GlobalSettingObj.SettingValue).Select(a => a.StartDate).FirstOrDefault();

                if (CalanderStartDate != null)
                {
                    //CODE REPLACED BY SHRADDHA ON 12 SEP 2017 REMOVED REDUNDANT AND UNNECESSARY IF ELSE CODE AND TAKEN ONE SINGLE LINE CODE START
                    Status = selectCriteria;
                    CompOffSanctionObj = WetosDB.SP_WetosGetEmployeeCompOffSanction(EmpNo).Where(a => ((a.EmployeeReportingId == EmpNo || a.EmployeeReportingId2 == EmpNo) && a.Id == Status && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null)).OrderByDescending(a => a.FromDate).OrderByDescending(a => a.ToDate).ToList();
                    //CODE REPLACED BY SHRADDHA ON 12 SEP 2017 REMOVED REDUNDANT AND UNNECESSARY IF ELSE CODE AND TAKEN ONE SINGLE LINE CODE END


                }
                #endregion

                // Updated by Rajas on 20 MAY 2017 END
                ViewBag.Status = Status;
                ViewBag.SelectedStatus = selectCriteria;
                // Added by Rajas on 9 MAY 2017 END


                #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //var EmployeeData = WetosDB.VwActiveEmployees.ToList();
                List<SP_VwActiveEmployee_Result> EmployeeData = WetosDB.SP_VwActiveEmployee(EmployeeId).ToList();
                #endregion

                // List<SP_WetosGetEmployeeLeaveSanction_Result> LeaveSanctionObj = WetosDB.SP_WetosGetEmployeeLeaveSanction(EmpNo).OrderByDescending(a => a.FromDate).OrderByDescending(a => a.ToDate).ToList();

                // Added by Rajas on 9 MAY 2017
                // Populate Status dropdown
                //REPLACED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 START
                //List<string> StatusObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 3).Select(a => a.Text).ToList();
                //ViewBag.StatusList = StatusObj;
                PopulateDropDown();
                //REPLACED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 END

                return View(CompOffSanctionObj);
            }
            catch
            {
                //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 START
                PopulateDropDown();
                //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 END
                return RedirectToAction("EmployeeDashboard", "Dashboard");
            }
        }

        [HttpPost]
        public ActionResult COSanctionPV(int selectCriteria = 0)
        {
            try
            {
                int EmpNo = Convert.ToInt32(Session["EmployeeNo"]);
                int Id = Convert.ToInt32(Session["Id"]);

                // Added by Rajas on 9 MAY 2017 START
                // Select Criteria as per dropdown value
                int Status = 0;

                // Updated by Rajas on 20 MAY 2017 START
                List<SP_WetosGetEmployeeCompOffSanction_Result> CompOffSanctionObj = new List<SP_WetosGetEmployeeCompOffSanction_Result>();

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

                    return View(CompOffSanctionObj);
                }

                DateTime CalanderStartDate = GetFYStartDate(); // DateTime CalanderStartDate = WetosDB.FinancialYears.Where(a => a.FinancialName == GlobalSettingObj.SettingValue).Select(a => a.StartDate).FirstOrDefault();

                if (CalanderStartDate != null)
                {
                    //CODE REPLACED BY SHRADDHA ON 12 SEP 2017 REMOVED REDUNDANT AND UNNECESSARY IF ELSE CODE AND TAKEN ONE SINGLE LINE CODE START
                    //Status = selectCriteria;
                    //CompOffSanctionObj = WetosDB.SP_WetosGetEmployeeCompOffSanction(EmpNo).Where(a => ((a.EmployeeReportingId == EmpNo) && a.Id == Status && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null)).OrderByDescending(a => a.FromDate).OrderByDescending(a => a.ToDate).ToList();
                    //CODE REPLACED BY SHRADDHA ON 12 SEP 2017 REMOVED REDUNDANT AND UNNECESSARY IF ELSE CODE AND TAKEN ONE SINGLE LINE CODE END

                    // Above code commented. Error was list for reportingId2 was not displayed
                    // Code updated by Rajas on 22 SEP 2017 START
                    if (selectCriteria == 1)  // Pending
                    {
                        Status = 1;

                        // Updated by Rajas on 7 JUNE 2017, ( || a.EmployeeReportingId2 == EmpNo) removed
                        CompOffSanctionObj = WetosDB.SP_WetosGetEmployeeCompOffSanction(EmpNo).Where(a => ((a.EmployeeReportingId == EmpNo) && a.Id == Status
                            && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null))
                            .OrderByDescending(a => a.FromDate).ToList();  // && a.Id == 4)
                    }
                    else if (selectCriteria == 2)  // Sanction
                    {
                        Status = 2;

                        CompOffSanctionObj = WetosDB.SP_WetosGetEmployeeCompOffSanction(EmpNo)
                            .Where(a => ((a.EmployeeReportingId == EmpNo || a.EmployeeReportingId2 == EmpNo) && a.Id == Status && a.FromDate >= CalanderStartDate
                                && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null))
                            .OrderByDescending(a => a.FromDate).ToList();  // && a.Id == 4)
                    }
                    else if (selectCriteria == 3) // Rejected by Approver
                    {
                        Status = 3;

                        CompOffSanctionObj = WetosDB.SP_WetosGetEmployeeCompOffSanction(EmpNo)
                            .Where(a => ((a.EmployeeReportingId == EmpNo || a.EmployeeReportingId2 == EmpNo)
                                && a.Id == Status && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null) || (a.EmployeeReportingId2 == EmpNo
                                && a.Id == Status && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null))
                                .OrderByDescending(a => a.FromDate).ToList();  // && a.Id == 4)
                    }
                    else if (selectCriteria == 4)  // Approve
                    {
                        Status = 4;

                        CompOffSanctionObj = WetosDB.SP_WetosGetEmployeeCompOffSanction(EmpNo)
                            .Where(a => ((a.EmployeeReportingId2 == EmpNo || a.EmployeeReportingId == EmpNo)
                                && a.Id == Status && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null))
                                .OrderByDescending(a => a.FromDate).ToList();  // && a.Id == 4)
                    }
                    else if (selectCriteria == 5)  // Cancel
                    {
                        Status = 5;

                        CompOffSanctionObj = WetosDB.SP_WetosGetEmployeeCompOffSanction(EmpNo)
                            .Where(a => ((a.EmployeeReportingId == EmpNo || a.EmployeeReportingId2 == EmpNo) && a.Id == Status
                                && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null))
                                .OrderByDescending(a => a.FromDate).ToList();  // && a.Id == 4)
                    }
                    else  // Rejected by Sanctioner
                    {
                        Status = 6;
                        CompOffSanctionObj = WetosDB.SP_WetosGetEmployeeCompOffSanction(EmpNo)
                            .Where(a => ((a.EmployeeReportingId2 == EmpNo) && a.Id == Status
                                && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null))
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
                List<SP_VwActiveEmployee_Result> EmployeeData = WetosDB.SP_VwActiveEmployee(EmployeeId).ToList();
                #endregion
                // List<SP_WetosGetEmployeeLeaveSanction_Result> LeaveSanctionObj = WetosDB.SP_WetosGetEmployeeLeaveSanction(EmpNo).OrderByDescending(a => a.FromDate).OrderByDescending(a => a.ToDate).ToList();

                // Added by Rajas on 9 MAY 2017
                // Populate Status dropdown
                //REPLACED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 START
                //List<string> StatusObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 3).Select(a => a.Text).ToList();
                //ViewBag.StatusList = StatusObj;
                PopulateDropDown();
                //REPLACED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 END

                return PartialView(CompOffSanctionObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in Leave Sanction due to " + ex.Message + ( ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                Error("Error in Leave Sanction due to " + ex.Message + ( ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 START
                PopulateDropDown();
                //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 END

                return RedirectToAction("EmployeeDashboard", "Dashboard");
            }
        }

        /// <summary>
        /// MODIFIED CODE BY SHRADDHA ON 10 MARCH 2017 FOR CompOffSANCTION POST
        /// </summary>
        /// <param name="CompOffSanction"></param>
        /// <param name="FC"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CompOffIndex(List<SP_WetosGetEmployeeCompOffSanction_Result> CompOffSanction, FormCollection FC)
        {
            #region TRY BLOCK
            try
            {
                int EmpNo = Convert.ToInt32(Session["EmployeeNo"]);

                //FAKE LOGIC FOR FILTERING
                #region FAKE LOGIC FOR FILTERING

                CompOffSanction = null;

                #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //var EmployeeData = WetosDB.Employees.ToList();
                List<SP_VwActiveEmployee_Result> EmployeeObj = WetosDB.SP_VwActiveEmployee(EmployeeId).ToList();
                #endregion

                List<SP_WetosGetEmployeeCompOffSanction_Result> CompOffSanctionObjForCount = new List<SP_WetosGetEmployeeCompOffSanction_Result>();
                string RejectReasonText = FC["RejectReasonText"];
                if (CompOffSanction == null)
                {
                    CompOffSanction = new List<SP_WetosGetEmployeeCompOffSanction_Result>();
                    int SingleApplicationID = 0;
                    int SingleStatusID = 0;
                    bool StatusIdSelected = false;
                    foreach (var key in FC.AllKeys)
                    {
                        string KeyStr = key.ToString();
                        if (KeyStr.Contains("StatusId") == true)
                        {
                            SingleStatusID = Convert.ToInt32(FC[KeyStr]);
                            string TempKeyStr = KeyStr.Replace("StatusId", "CompOffId");
                            SingleApplicationID = Convert.ToInt32(FC[TempKeyStr]);
                            SP_WetosGetEmployeeCompOffSanction_Result TesmpCOSanction = WetosDB.SP_WetosGetEmployeeCompOffSanction(EmpNo).Where(a => a.CompOffId == SingleApplicationID).FirstOrDefault(); //.ToList();
                            if (TesmpCOSanction != null)
                            {
                                TesmpCOSanction.StatusId = SingleStatusID;// UPDATE STATUS
                                CompOffSanction.Add(TesmpCOSanction);
                            }
                            StatusIdSelected = true;
                        }
                    }

                    if (StatusIdSelected == false)
                    {
                        Information("Please select atleast one entry for processing");
                        PopulateDropDown();
                        return View(CompOffSanctionObjForCount);
                    }

                }

                #endregion

                //GET LIST OF APPLICATIONS FOR WHICH STATUS REJECTED BY APPROVER / REJECTED BY SANCTIONER / CANCEL IS SELECTED
                CompOffSanctionObjForCount = CompOffSanction.Where(a => a.StatusId == 3 || a.StatusId == 5 || a.StatusId == 6).ToList();


                #region CHECK WHETHER HAVING CO AS LEAVE OR NOT
                GlobalSetting IsHavingCOLeave = WetosDB.GlobalSettings.Where(a => a.SettingText == "IsHavingCOLeave").FirstOrDefault();
                #endregion


                #region REAL LOGIC

                #region ENTER REJECT/CANCEL REASON VALIDATION
                if (CompOffSanctionObjForCount.Count() > 0 && (RejectReasonText == null || RejectReasonText == ""))
                {
                    ModelState.AddModelError("CustomError", "Please Enter Reject/cancel Reason");
                    List<SP_WetosGetEmployeeCompOffSanction_Result> CompOffSanctionList = WetosDB.SP_WetosGetEmployeeCompOffSanction(EmpNo).OrderByDescending(a => a.FromDate).OrderByDescending(a => a.ToDate).ToList();
                    PopulateDropDown();
                    return View(CompOffSanctionObjForCount);
                }
                #endregion


                else
                {
                    foreach (var i in CompOffSanctionObjForCount)
                    {
                        CompOffApplication COMPOFFAPPLICATIONObj = WetosDB.CompOffApplications.Where(a => a.CompOffId == i.CompOffId).FirstOrDefault();
                        COMPOFFAPPLICATIONObj.RejectReason = RejectReasonText;
                        WetosDB.SaveChanges();
                    }

                    foreach (SP_WetosGetEmployeeCompOffSanction_Result item in CompOffSanction)
                    {
                        CompOffApplication COMPOFFAPPLICATIONObj = WetosDB.CompOffApplications.Where(a => a.CompOffId == item.CompOffId).FirstOrDefault();

                        //CODE TO CHECK WHETHER EMPLOYEE IS VALID / ACTIVE OR NOT
                        VwActiveEmployee ValidEmpObj = WetosDB.VwActiveEmployees.Where(a => a.EmployeeId == COMPOFFAPPLICATIONObj.EmployeeId).FirstOrDefault();

                        //IF EMPLOYEE IS VALID / ACTIVE
                        if (ValidEmpObj != null)
                        {
                            //IF SELECTED STATUS FOR APPLICATION IS VALID
                            if (item.StatusId != 0)
                            {
                                #region PREVENT SANCTIONING CO WHICH IS ALREADY SANCTIONED
                                if (COMPOFFAPPLICATIONObj.StatusId == 2 && item.StatusId == 2)
                                {
                                    Error("Comp Off is already Sanction for Comp Off " + COMPOFFAPPLICATIONObj.FromDate.Value.ToString("dd/MMM/yyyy") + " - " + COMPOFFAPPLICATIONObj.ToDate.Value.ToString("dd/MMM/yyyy") + " For EmployeeCode: " + ValidEmpObj.EmployeeCode + "-" + ValidEmpObj.FirstName + " " + ValidEmpObj.MiddleName + " " + ValidEmpObj.LastName);
                                    PopulateDropDown();
                                    return View(CompOffSanctionObjForCount);
                                }
                                #endregion

                                //GET RECORD FROM COMP OFF TABLE ENTRY AGAINST WHICH COMP OFF IS APPLIED
                                List<CompOff> CompOffSanctionListforcompoff = WetosDB.CompOffs.Where(a => a.EmployeeId == COMPOFFAPPLICATIONObj.EmployeeId && a.CompOffApplicationID == COMPOFFAPPLICATIONObj.CompOffId && a.CoDate != null).ToList();



                                #region SANCTION CO START
                                if (COMPOFFAPPLICATIONObj.StatusId != 2 && item.StatusId == 2)
                                {
                                    //IF LOGIN EMPLOYEE IS APPROVER AS WELL AS SANCTIONER
                                    #region IF LOGIN EMPLOYEE IS APPROVER AS WELL AS SANCTIONER
                                    if (ValidEmpObj.EmployeeReportingId2 == EmpNo || ValidEmpObj.EmployeeReportingId == EmpNo)
                                    {
                                        COMPOFFAPPLICATIONObj.StatusId = item.StatusId;
                                        COMPOFFAPPLICATIONObj.Status = WetosDB.DropdownDatas.Where(a => a.TypeId == 3 && a.Value == COMPOFFAPPLICATIONObj.StatusId).Select(a => a.Text).FirstOrDefault();
                                        COMPOFFAPPLICATIONObj.RejectReason = string.Empty;



                                        if (IsHavingCOLeave != null)
                                        {
                                            //IF CO LEAVE IS APPLICABLE
                                            #region IF CO LEAVE IS APPLICABLE
                                            if (IsHavingCOLeave.SettingValue == "1")
                                            {
                                                //GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == "Current Financial Year").FirstOrDefault();

                                                //ABOVE LINE COMMENTED AND BELOW LINE IS ADDED BY SHALAKA ON 13TH DEC 2017
                                                GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.CurrentFinancialYear).FirstOrDefault();

                                                if (GlobalSettingObj != null)
                                                {
                                                    FinancialYear GetFYObj = WetosDB.FinancialYears.Where(a => a.FinancialName == GlobalSettingObj.SettingValue).FirstOrDefault();
                                                    LeaveCredit LeaveCreditForCO = WetosDB.LeaveCredits.Where(a => a.EmployeeId == COMPOFFAPPLICATIONObj.EmployeeId && a.FinancialYearId == GetFYObj.FinancialYearId && a.LeaveType == "CO").FirstOrDefault();
                                                    LeaveBalance LeaveBalanceForCO = WetosDB.LeaveBalances.Where(a => a.EmployeeId == COMPOFFAPPLICATIONObj.EmployeeId && a.LeaveType == "CO").FirstOrDefault();

                                                    TimeSpan TS = (Convert.ToDateTime(COMPOFFAPPLICATIONObj.ToDate)) - (Convert.ToDateTime(COMPOFFAPPLICATIONObj.FromDate));
                                                    double TotalDays = TS.Days;

                                                    TotalDays = TotalDays + 1;

                                                    // Added by Rajas on 16 MAY 2017
                                                    double Availed = 0.0;
                                                    double Encash = 0.0;
                                                    double CarryForward = 0.0;

                                                    // If CO is not credited to Leavecredit for Current FY
                                                    if (LeaveCreditForCO == null)
                                                    {
                                                        // Code added by Rajas on 16 MAY 2017 START
                                                        LeaveCreditForCO = new LeaveCredit();

                                                        LeaveCreditForCO.FinancialYearId = GetFYObj.FinancialYearId;

                                                        LeaveCreditForCO.LeaveType = "CO";  // verify?

                                                        LeaveCreditForCO.OpeningBalance = TotalDays;

                                                        LeaveCreditForCO.Availed = Availed;

                                                        LeaveCreditForCO.CarryForward = CarryForward;

                                                        LeaveCreditForCO.Encash = Encash;

                                                        LeaveCreditForCO.CompanyId = item.CompanyId.Value;

                                                        LeaveCreditForCO.BranchId = item.BranchId.Value;

                                                        LeaveCreditForCO.EmployeeId = item.EmployeeId.Value;

                                                        WetosDB.LeaveCredits.AddObject(LeaveCreditForCO); // NEW CREDIT ENTRY FY

                                                        WetosDB.SaveChanges();

                                                        LeaveBalanceForCO.PreviousBalance = LeaveBalanceForCO.CurrentBalance;

                                                        LeaveBalanceForCO.CurrentBalance = LeaveBalanceForCO.CurrentBalance + TotalDays;

                                                        WetosDB.SaveChanges();
                                                    }
                                                    else
                                                    {
                                                        LeaveCreditForCO.OpeningBalance = LeaveCreditForCO.OpeningBalance + TotalDays;  // Update credit table

                                                        LeaveBalanceForCO.PreviousBalance = LeaveBalanceForCO.CurrentBalance;

                                                        LeaveBalanceForCO.CurrentBalance = LeaveBalanceForCO.CurrentBalance + TotalDays;  // update balance table

                                                        WetosDB.SaveChanges();
                                                    }
                                                }
                                                else
                                                {
                                                    Information("Inconsistent FY data detected, please verify and try again!");
                                                    List<SP_WetosGetEmployeeCompOffSanction_Result> COSanctionObj = WetosDB.SP_WetosGetEmployeeCompOffSanction(EmpNo).OrderByDescending(a => a.FromDate).OrderByDescending(a => a.ToDate).ToList();
                                                    PopulateDropDown();
                                                    return View(CompOffSanctionObjForCount);
                                                }
                                            }
                                            #region IF CO LEAVE IS NOT APPLICABLE
                                            //IF CO LEAVE IS NOT AVAILABLE
                                            else
                                            {
                                                #region UPDATE DAILYTRANSACTION STATUS START
                                                for (DateTime CurrentDate = Convert.ToDateTime(COMPOFFAPPLICATIONObj.FromDate); CurrentDate.Date <= Convert.ToDateTime(COMPOFFAPPLICATIONObj.ToDate); CurrentDate = CurrentDate.AddDays(1))
                                                {
                                                    DailyTransaction DailyTransactionObj = WetosDB.DailyTransactions.Where(a => a.EmployeeId == COMPOFFAPPLICATIONObj.EmployeeId && a.TranDate == CurrentDate).FirstOrDefault();
                                                    if (DailyTransactionObj != null)
                                                    {
                                                        DailyTransactionObj.ActualStatus = DailyTransactionObj.Status;
                                                        WetosDB.SaveChanges();
                                                    }
                                                }

                                                string ReturnMessage = string.Empty;
                                                if (WetosAdministrationController.COProcessingEx(WetosDB, Convert.ToDateTime(item.FromDate), Convert.ToDateTime(item.ToDate), Convert.ToInt32(COMPOFFAPPLICATIONObj.EmployeeId), ref ReturnMessage) == false)
                                                {
                                                    AddAuditTrail(ReturnMessage);
                                                }
                                                #endregion
                                            }
                                            #endregion
                                            #endregion
                                        }

                                        //IF CO LEAVE IS NOT APPLICABLE
                                        #region IF CO LEAVE IS NOT APPLICABLE
                                        //IF CO LEAVE IS NOT AVAILABLE
                                        else
                                        {
                                            #region UPDATE DAILYTRANSACTION STATUS START
                                            string ReturnMessage = string.Empty;
                                            for (DateTime CurrentDate = Convert.ToDateTime(COMPOFFAPPLICATIONObj.FromDate); CurrentDate.Date <= Convert.ToDateTime(COMPOFFAPPLICATIONObj.ToDate); CurrentDate = CurrentDate.AddDays(1))
                                            {
                                                DailyTransaction DailyTransactionObj = WetosDB.DailyTransactions.Where(a => a.EmployeeId == COMPOFFAPPLICATIONObj.EmployeeId && a.TranDate == CurrentDate).FirstOrDefault();
                                                if (DailyTransactionObj != null)
                                                {
                                                    DailyTransactionObj.ActualStatus = DailyTransactionObj.Status;
                                                    WetosDB.SaveChanges();
                                                }
                                            }
                                            if (WetosAdministrationController.COProcessingEx(WetosDB, Convert.ToDateTime(item.FromDate), Convert.ToDateTime(item.ToDate), Convert.ToInt32(COMPOFFAPPLICATIONObj.EmployeeId), ref ReturnMessage) == false)
                                            {
                                                AddAuditTrail(ReturnMessage);
                                            }
                                            #endregion
                                        }
                                        #endregion

                                        //IF COMP OFF ENTRY IS AVAILABLE IN COMP OFF TABLE AND COMP OFF IS BEING SANCTIONED THEN UPDATE APPSTATUS = S IN COMP OFF TABLE
                                        #region IF COMP OFF ENTRY IS AVAILABLE IN COMP OFF TABLE AND COMP OFF IS BEING SANCTIONED THEN UPDATE APPSTATUS = S IN COMP OFF TABLE
                                        if (CompOffSanctionListforcompoff.Count > 0)
                                        {
                                            foreach (CompOff CompOffSanctionObjforcompoff in CompOffSanctionListforcompoff)
                                            {
                                                if (CompOffSanctionObjforcompoff != null)
                                                {
                                                    CompOffSanctionObjforcompoff.AppStatus = "S";
                                                    WetosDB.SaveChanges();
                                                }
                                            }
                                        }
                                        #endregion

                                    }
                                    #endregion

                                    #region IF LOGIN EMPLOYEE IS NOT APPROVER AS WELL AS SANCTIONER THEN PROVIDE ERROR MESSAGE
                                    else
                                    {
                                        Information("You can not sanction this CO entry as you are not sanctioner for selected employee");
                                        List<SP_WetosGetEmployeeCompOffSanction_Result> COSanctionObj = WetosDB.SP_WetosGetEmployeeCompOffSanction(EmpNo).OrderByDescending(a => a.FromDate).OrderByDescending(a => a.ToDate).ToList();
                                        PopulateDropDown();

                                        return View(CompOffSanctionObjForCount);
                                    }
                                    #endregion


                                }
                                #endregion

                                #region REJECT BY APPROVER / REJECT BY SANCTIONER / CANCEL / APPROVE CO START
                                else if (item.StatusId == 3 || item.StatusId == 6 || item.StatusId == 5 || item.StatusId == 4)
                                {
                                    if (IsHavingCOLeave != null)
                                    {
                                        //IF CO LEAVE IS APPLICABLE
                                        #region IF CO LEAVE IS APPLICABLE
                                        if (IsHavingCOLeave.SettingValue == "1")
                                        {
                                            COMPOFFAPPLICATIONObj.StatusId = item.StatusId;
                                            COMPOFFAPPLICATIONObj.Status = WetosDB.DropdownDatas.Where(a => a.TypeId == 3 && a.Value == COMPOFFAPPLICATIONObj.StatusId).Select(a => a.Text).FirstOrDefault();
                                            WetosDB.SaveChanges();
                                        }
                                        #endregion
                                        else
                                        {
                                            #region IF CO LEAVE IS NOT APPLICABLE OR GLOBAL SETTING VALUE IS NULL
                                            if (COMPOFFAPPLICATIONObj.StatusId == 2)
                                            {
                                                for (DateTime CurrentDate = Convert.ToDateTime(COMPOFFAPPLICATIONObj.FromDate); CurrentDate.Date <= Convert.ToDateTime(COMPOFFAPPLICATIONObj.ToDate); CurrentDate = CurrentDate.AddDays(1))
                                                {
                                                    DailyTransaction DailyTransactionObj = WetosDB.DailyTransactions.Where(a => a.EmployeeId == COMPOFFAPPLICATIONObj.EmployeeId && a.TranDate == CurrentDate).FirstOrDefault();
                                                    if (DailyTransactionObj != null)
                                                    {
                                                        DailyTransactionObj.Status = DailyTransactionObj.ActualStatus;
                                                        WetosDB.SaveChanges();
                                                    }
                                                }
                                            }

                                            if (CompOffSanctionListforcompoff.Count > 0)
                                            {
                                                foreach (CompOff CompOffSanctionObjforcompoff in CompOffSanctionListforcompoff)
                                                {
                                                    if (item.StatusId == 3 || item.StatusId == 6 || item.StatusId == 5)
                                                    {
                                                        CompOffSanctionObjforcompoff.AppStatus = null;
                                                        CompOffSanctionObjforcompoff.CoDate = null;
                                                        CompOffSanctionObjforcompoff.CompOffApplicationID = null;
                                                        WetosDB.SaveChanges();
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                Error("You can not cancel comp off for " + item.FromDate + ", Employee: " + ValidEmpObj.EmployeeCode + " - " + ValidEmpObj.FirstName + " " + ValidEmpObj.LastName + ". COMP OFF Data is not available.");
                                                PopulateDropDown();
                                                return View(CompOffSanctionObjForCount);
                                            }

                                            COMPOFFAPPLICATIONObj.StatusId = item.StatusId;
                                            COMPOFFAPPLICATIONObj.Status = WetosDB.DropdownDatas.Where(a => a.TypeId == 3 && a.Value == COMPOFFAPPLICATIONObj.StatusId).Select(a => a.Text).FirstOrDefault();
                                            WetosDB.SaveChanges();
                                            #endregion
                                        }
                                    }
                                    else
                                    {
                                        #region IF CO LEAVE IS NOT APPLICABLE OR GLOBAL SETTING VALUE IS NULL
                                        if (COMPOFFAPPLICATIONObj.StatusId == 2)
                                        {
                                            for (DateTime CurrentDate = Convert.ToDateTime(COMPOFFAPPLICATIONObj.FromDate); CurrentDate.Date <= Convert.ToDateTime(COMPOFFAPPLICATIONObj.ToDate); CurrentDate = CurrentDate.AddDays(1))
                                            {
                                                DailyTransaction DailyTransactionObj = WetosDB.DailyTransactions.Where(a => a.EmployeeId == COMPOFFAPPLICATIONObj.EmployeeId && a.TranDate == CurrentDate).FirstOrDefault();
                                                if (DailyTransactionObj != null)
                                                {
                                                    DailyTransactionObj.Status = DailyTransactionObj.ActualStatus;
                                                    WetosDB.SaveChanges();
                                                }
                                            }
                                        }

                                        if (CompOffSanctionListforcompoff.Count > 0)
                                        {
                                            foreach (CompOff CompOffSanctionObjforcompoff in CompOffSanctionListforcompoff)
                                            {
                                                if (item.StatusId == 3 || item.StatusId == 6 || item.StatusId == 5)
                                                {
                                                    CompOffSanctionObjforcompoff.AppStatus = null;
                                                    CompOffSanctionObjforcompoff.CoDate = null;
                                                    CompOffSanctionObjforcompoff.CompOffApplicationID = null;
                                                    WetosDB.SaveChanges();
                                                }
                                            }
                                        }
                                        else
                                        {
                                            Error("You can not cancel comp off for " + item.FromDate + ", Employee: " + ValidEmpObj.EmployeeCode + " - " + ValidEmpObj.FirstName + " " + ValidEmpObj.LastName + ". COMP OFF Data is not available.");
                                            PopulateDropDown();
                                            return View(CompOffSanctionObjForCount);
                                        }

                                        COMPOFFAPPLICATIONObj.StatusId = item.StatusId;
                                        COMPOFFAPPLICATIONObj.Status = WetosDB.DropdownDatas.Where(a => a.TypeId == 3 && a.Value == COMPOFFAPPLICATIONObj.StatusId).Select(a => a.Text).FirstOrDefault();

                                        // ADDED BY MSJ ON 09 JAN 2019 START
                                        COMPOFFAPPLICATIONObj.AppliedBy = Convert.ToInt32(Session["EmployeeNo"]);
                                        COMPOFFAPPLICATIONObj.AppliedOn = DateTime.Now;
                                        COMPOFFAPPLICATIONObj.SanctionedBy = Convert.ToInt32(Session["EmployeeNo"]);
                                        COMPOFFAPPLICATIONObj.SanctionedOn = DateTime.Now;
                                        // ADDED BY MSJ ON 09 JAN 2019 END

                                        WetosDB.SaveChanges();
                                        #endregion
                                    }
                                }
                                #endregion

                                #region ADD AUDIT LOG
                                string Oldrecord = "Employee ID : " + EmpNo
                                                    + "FromDate :" + COMPOFFAPPLICATIONObj.FromDate + "To Date :" + COMPOFFAPPLICATIONObj.ToDate
                                                    + "FromDateStatus :" + COMPOFFAPPLICATIONObj.FromDateStatus + "ToDateStatus :" + COMPOFFAPPLICATIONObj.ToDateStatus
                                                    + "Purpose :" + COMPOFFAPPLICATIONObj.Purpose + " Status : " + COMPOFFAPPLICATIONObj.Status + "Reject Reason :"
                                                    + COMPOFFAPPLICATIONObj.RejectReason + "ExtraHoursDate :" + COMPOFFAPPLICATIONObj.ExtrasHoursDate + "Branch ID :"
                                                    + COMPOFFAPPLICATIONObj.BranchId + "Company ID:" + COMPOFFAPPLICATIONObj.CompanyId;
                                //---x---------x-----------------x------------x----------------x-----------x-------------x------------x--------------x----------x---------x-----

                                string Newrecord = "Employee ID : " + EmpNo
                                                    + "FromDate :" + COMPOFFAPPLICATIONObj.FromDate + "To Date :" + COMPOFFAPPLICATIONObj.ToDate
                                                    + "FromDateStatus :" + COMPOFFAPPLICATIONObj.FromDateStatus + "ToDateStatus :" + COMPOFFAPPLICATIONObj.ToDateStatus
                                                    + "Purpose :" + COMPOFFAPPLICATIONObj.Purpose + " Status : " + COMPOFFAPPLICATIONObj.Status + "Reject Reason :"
                                                    + COMPOFFAPPLICATIONObj.RejectReason + "ExtraHoursDate :" + COMPOFFAPPLICATIONObj.ExtrasHoursDate + "Branch ID :"
                                                    + COMPOFFAPPLICATIONObj.BranchId + "Company ID:" + COMPOFFAPPLICATIONObj.CompanyId;

                                //LOGIN ID IS TAKEN FROM SESSION PERSISTER
                                string Formname = "CUMULATIVE COMPOFF SANCTION";
                                //ACTION IS UPDATE
                                string Message = " ";
                                int EMP = Convert.ToInt32(COMPOFFAPPLICATIONObj.EmployeeId);

                                WetosAdministrationController.GenerateAuditLogsUpdate(WetosDB, EMP, Formname, Oldrecord,
                                    Newrecord, ref Message);
                                #endregion

                                #region COMPOFF SANCTION NOTIFICATION
                                #region SEND COMP OFF NOTIFICATION TO EMPLOYEE WHOM COMP OFF IS BEING PROCESSED
                                Notification NotificationObj = new Notification();
                                NotificationObj.FromID = ValidEmpObj.EmployeeReportingId;
                                NotificationObj.ToID = ValidEmpObj.EmployeeId;
                                NotificationObj.SendDate = DateTime.Now;
                                NotificationObj.NotificationContent = "Your CompOff application from " + COMPOFFAPPLICATIONObj.FromDate.Value.ToString("dd-MMM-yyyy") + " to " + COMPOFFAPPLICATIONObj.ToDate.Value.ToString("dd-MMM-yyyy") + " is " + COMPOFFAPPLICATIONObj.Status;
                                NotificationObj.ReadFlag = false;
                                NotificationObj.SendDate = DateTime.Now;

                                WetosDB.Notifications.AddObject(NotificationObj);
                                WetosDB.SaveChanges();
                                #endregion


                                if (COMPOFFAPPLICATIONObj.StatusId == 4 && ValidEmpObj.EmployeeReportingId == EmpNo)
                                {
                                    Notification NotificationObj2 = new Notification();
                                    NotificationObj.FromID = ValidEmpObj.EmployeeId;
                                    NotificationObj.ToID = ValidEmpObj.EmployeeReportingId2;
                                    NotificationObj.SendDate = DateTime.Now;
                                    NotificationObj.NotificationContent = "Received CompOff application for sanctioning from " + COMPOFFAPPLICATIONObj.FromDate.Value.ToString("dd-MMM-yyyy") + " to " + COMPOFFAPPLICATIONObj.ToDate.Value.ToString("dd-MMM-yyyy") + " for " + ValidEmpObj.FirstName + " " + ValidEmpObj.LastName;
                                    //NotificationObj.NotificationContent = "Leave applied from " + LeaveApplicationObj.FromDate.ToString("dd-MMM-yyyy") + " to " + LeaveApplicationObj.ToDate.ToString("dd-MMM-yyyy") + " is " + LeaveApplicationObj.Status;
                                    NotificationObj.ReadFlag = false;
                                    NotificationObj.SendDate = DateTime.Now;

                                    WetosDB.Notifications.AddObject(NotificationObj2);

                                    WetosDB.SaveChanges();

                                }

                                // NOTIFICATION COUNT
                                int NoTiCount = WetosDB.Notifications.Where(a => a.ToID == EmpNo && (a.ReadFlag == false || a.ReadFlag == null)).Count();
                                Session["NotificationCount"] = NoTiCount;

                                #endregion

                                #region EMAIL
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
                                            if (SendEmail(ValidEmpObj.Email, NotificationObj.NotificationContent, NotificationObj.NotificationContent, ref EmailUpdateStatus, "CompOff " + COMPOFFAPPLICATIONObj.Status) == false)
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
                        }
                    }

                    List<SP_WetosGetEmployeeCompOffSanction_Result> CompOffSanctionObj = WetosDB.SP_WetosGetEmployeeCompOffSanction(EmpNo).OrderByDescending(a => a.FromDate).OrderByDescending(a => a.ToDate).ToList();
                    AddAuditTrail("Success - Compoff application processed successfully");  // Updated by Rajas on 17 JAN 2017
                    Success("Success - Compoff application processed successfully");
                    PopulateDropDown();
                    return View(CompOffSanctionObjForCount);
                }
                #endregion
            }
            #endregion

            #region CATCH BLOCK TO HANDLE RUNTIME EXCEPTION
            catch (System.Exception ex)
            {
                AddAuditTrail("Error - Compoff application sanctioned failed " + ex.Message);  // Updated by Rajas on 17 JAN 2017
                Error("Error - Compoff application sanctioned failed");
                PopulateDropDown();
                return RedirectToAction("CompOffIndex");

            }
            #endregion
        }


        /// <summary>
        /// LEAVE ENCASH SANCTION INDEX ADDED BY SHRADDHA ON 19 DEC 2016
        /// </summary>
        /// <returns></returns>
        public ActionResult ExceptionSanctionIndex(int selectCriteria = 0)
        {
            try
            {

                int EmpNo = Convert.ToInt32(Session["EmployeeNo"]);
                int Id = Convert.ToInt32(Session["Id"]);

                // Added by Rajas on 9 MAY 2017 START
                // Select Criteria as per dropdown value 
                int StatusId = 0; //string.Empty;

                List<SP_ExceptionSanctionIndex_Result> ExceptionSanctionList = new List<SP_ExceptionSanctionIndex_Result>();

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
                    return View(ExceptionSanctionList);
                }

                DateTime CalanderStartDate = GetFYStartDate(); // DateTime CalanderStartDate = WetosDB.FinancialYears.Where(a => a.FinancialName == GlobalSettingObj.SettingValue).Select(a => a.StartDate).FirstOrDefault();

                if (CalanderStartDate != null)
                {
                    //CODE REPLACED BY SHRADDHA ON 12 SEP 2017 REMOVED REDUNDANT AND UNNECESSARY IF ELSE CODE AND TAKEN ONE SINGLE LINE CODE START
                    StatusId = selectCriteria;
                    ExceptionSanctionList = WetosDB.SP_ExceptionSanctionIndex(EmpNo).Where(a => ((a.EmployeeReportingId == EmpNo || a.EmployeeReportingId2 == EmpNo) && a.Id == StatusId && a.ExceptionDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null)).ToList();
                    //CODE REPLACED BY SHRADDHA ON 12 SEP 2017 REMOVED REDUNDANT AND UNNECESSARY IF ELSE CODE AND TAKEN ONE SINGLE LINE CODE END



                    // Added by Rajas on 9 MAY 2017 END
                }
                #endregion

                ViewBag.SelectedStatus = selectCriteria;
                // Added by Rajas on 9 MAY 2017 END
                ViewBag.Status = StatusId;

                #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //var EmployeeData = WetosDB.VwActiveEmployees.ToList();
                List<SP_VwActiveEmployee_Result> EmployeeData = WetosDB.SP_VwActiveEmployee(EmployeeId).ToList();
                #endregion
                // List<SP_WetosGetEmployeeLeaveSanction_Result> LeaveSanctionObj = WetosDB.SP_WetosGetEmployeeLeaveSanction(EmpNo).OrderByDescending(a => a.FromDate).OrderByDescending(a => a.ToDate).ToList();

                // Added by Rajas on 9 MAY 2017
                // Populate Status dropdown
                //REPLACED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 START
                //List<string> StatusObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 3).Select(a => a.Text).ToList();
                //ViewBag.StatusList = StatusObj;
                PopulateDropDown();
                //REPLACED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 END

                return View(ExceptionSanctionList);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + ( ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 START
                PopulateDropDown();
                //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 END

                return View("Error");
            }
        }



        [HttpPost]
        public ActionResult ExceptionSanctionPV(int selectCriteria = 0)
        {
            try
            {
                int EmpNo = Convert.ToInt32(Session["EmployeeNo"]);
                int Id = Convert.ToInt32(Session["Id"]);

                // Added by Rajas on 9 MAY 2017 START
                // Select Criteria as per dropdown value 
                int Status = 0; //string.Empty;

                List<SP_ExceptionSanctionIndex_Result> ExceptionSanctionList = new List<SP_ExceptionSanctionIndex_Result>();

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
                    return View(ExceptionSanctionList);
                }

                DateTime CalanderStartDate = GetFYStartDate(); // DateTime CalanderStartDate = WetosDB.FinancialYears.Where(a => a.FinancialName == GlobalSettingObj.SettingValue).Select(a => a.StartDate).FirstOrDefault();

                if (CalanderStartDate != null)
                {
                    //CODE REPLACED BY SHRADDHA ON 12 SEP 2017 REMOVED REDUNDANT AND UNNECESSARY IF ELSE CODE AND TAKEN ONE SINGLE LINE CODE START
                    //StatusId = selectCriteria;
                    //ExceptionSanctionList = WetosDB.SP_ExceptionSanctionIndex(EmpNo)
                    //    .Where(a => ((a.EmployeeReportingId == EmpNo || a.EmployeeReportingId2 == EmpNo) 
                    //        && a.Id == StatusId && a.ExceptionDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null)).ToList();
                    //CODE REPLACED BY SHRADDHA ON 12 SEP 2017 REMOVED REDUNDANT AND UNNECESSARY IF ELSE CODE AND TAKEN ONE SINGLE LINE CODE END

                    // Above code commented. Error was list for reportingId2 was not displayed
                    // Code updated by Rajas on 22 SEP 2017 START
                    if (selectCriteria == 1)  // Pending
                    {
                        Status = 1;

                        // Updated by Rajas on 7 JUNE 2017, ( || a.EmployeeReportingId2 == EmpNo) removed
                        ExceptionSanctionList = WetosDB.SP_ExceptionSanctionIndex(EmpNo).Where(a => ((a.EmployeeReportingId == EmpNo) && a.Id == Status
                            && a.ExceptionDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null))
                            .OrderByDescending(a => a.ExceptionDate).ToList();  // && a.Id == 4)
                    }
                    else if (selectCriteria == 2)  // Sanction
                    {
                        Status = 2;

                        ExceptionSanctionList = WetosDB.SP_ExceptionSanctionIndex(EmpNo)
                            .Where(a => ((a.EmployeeReportingId == EmpNo || a.EmployeeReportingId2 == EmpNo) && a.Id == Status && a.ExceptionDate >= CalanderStartDate
                                && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null))
                            .OrderByDescending(a => a.ExceptionDate).ToList();  // && a.Id == 4)
                    }
                    else if (selectCriteria == 3) // Rejected by Approver
                    {
                        Status = 3;

                        ExceptionSanctionList = WetosDB.SP_ExceptionSanctionIndex(EmpNo)
                            .Where(a => ((a.EmployeeReportingId == EmpNo || a.EmployeeReportingId2 == EmpNo)
                                && a.Id == Status && a.ExceptionDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null) || (a.EmployeeReportingId2 == EmpNo
                                && a.Id == Status && a.ExceptionDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null))
                                .OrderByDescending(a => a.ExceptionDate).ToList();  // && a.Id == 4)
                    }
                    else if (selectCriteria == 4)  // Approve
                    {
                        Status = 4;

                        ExceptionSanctionList = WetosDB.SP_ExceptionSanctionIndex(EmpNo)
                            .Where(a => ((a.EmployeeReportingId2 == EmpNo || a.EmployeeReportingId == EmpNo)
                                && a.Id == Status && a.ExceptionDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null))
                                .OrderByDescending(a => a.ExceptionDate).ToList();  // && a.Id == 4)
                    }
                    else if (selectCriteria == 5)  // Cancel
                    {
                        Status = 5;

                        ExceptionSanctionList = WetosDB.SP_ExceptionSanctionIndex(EmpNo)
                            .Where(a => ((a.EmployeeReportingId == EmpNo || a.EmployeeReportingId2 == EmpNo) && a.Id == Status
                                && a.ExceptionDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null))
                                .OrderByDescending(a => a.ExceptionDate).ToList();  // && a.Id == 4)
                    }
                    else  // Rejected by Sanctioner
                    {
                        Status = 6;
                        ExceptionSanctionList = WetosDB.SP_ExceptionSanctionIndex(EmpNo)
                            .Where(a => ((a.EmployeeReportingId2 == EmpNo) && a.Id == Status
                                && a.ExceptionDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null))
                            .OrderByDescending(a => a.ExceptionDate).ToList();  // && a.Id == 4)
                    }
                    // Code updated by Rajas on 22 SEP 2017 END
                }
                #endregion

                ViewBag.SelectedStatus = selectCriteria;
                // Added by Rajas on 9 MAY 2017 END
                ViewBag.Status = Status;

                #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //var EmployeeData = WetosDB.VwActiveEmployees.ToList();
                List<SP_VwActiveEmployee_Result> EmployeeData = WetosDB.SP_VwActiveEmployee(EmployeeId).ToList();
                #endregion
                // List<SP_WetosGetEmployeeLeaveSanction_Result> LeaveSanctionObj = WetosDB.SP_WetosGetEmployeeLeaveSanction(EmpNo).OrderByDescending(a => a.FromDate).OrderByDescending(a => a.ToDate).ToList();

                // Added by Rajas on 9 MAY 2017
                // Populate Status dropdown
                PopulateDropDown();

                return PartialView(ExceptionSanctionList);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in Attendance Regularization Sanction due to " + ex.Message + ( ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                Error("Error in Attendance Regularization Sanction due to " + ex.Message + ( ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                return RedirectToAction("EmployeeDashboard", "Dashboard");
            }
        }

        /// <summary>
        /// ADDED BY SHRADDHA ON 20 DEC 2016 FOR EXCEPTION SANCTION INDEX POST
        /// </summary>
        /// <param name="LeaveEncashSanction"></param>
        /// <param name="FC"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ExceptionSanctionIndex(List<SP_ExceptionSanctionIndex_Result> ExceptionSanction, FormCollection FC)
        {
            try
            {
                int selectioncriteria = Convert.ToInt32(FC["StatusObj"]);
                ViewBag.Status = selectioncriteria;

                #region FAKE LOGIC

                int StatusId = Convert.ToInt32(FC["[0].StatusId"]);

                #region OLD FAKE LOGIC

                #endregion
                ExceptionSanction = null;

                // FAKE LOGIC MSJ
                int EmpNo = Convert.ToInt32(Session["EmployeeNo"]);
                int Id = Convert.ToInt32(Session["Id"]);

                //List<SP_WetosGetEmployeeLeaveSanction_Result> LeaveSanctionObj1 = WetosDB.SP_WetosGetEmployeeLeaveSanction(EmpNo).ToList();

                DateTime abc = Convert.ToDateTime("2016/01/01");

                #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //var EmployeeData = WetosDB.Employees.ToList();
                List<SP_VwActiveEmployee_Result> EmployeeData = WetosDB.SP_VwActiveEmployee(EmployeeId).ToList();
                #endregion

                List<SP_ExceptionSanctionIndex_Result> ExceptionSanctionList = new List<SP_ExceptionSanctionIndex_Result>();

                string RejectReasonText = FC["RejectReasonText"];

                //List<SP_LeaveSanctionIndex_Result> LeaveSanctionObj = new List<SP_LeaveSanctionIndex_Result>();
                if (ExceptionSanction == null)
                {
                    ExceptionSanction = new List<SP_ExceptionSanctionIndex_Result>();

                    int SingleApplicationID = 0;
                    int SingleStatusID = 0;

                    // Added by Rajas on 20 MAY 2017
                    bool StatusIdSelected = false;

                    foreach (var key in FC.AllKeys)
                    {
                        string KeyStr = key.ToString();

                        if (KeyStr.Contains("StatusId") == true)
                        {
                            string[] KeyStrArray = KeyStr.Split('.');

                            if (KeyStrArray.Count() > 1)
                            {
                                if (KeyStrArray[1] == "StatusId")
                                {
                                    string abcd = FC[KeyStr].ToString();

                                    SingleStatusID = int.Parse(FC[KeyStr]);

                                    string TempKeyStr = KeyStr.Replace("StatusId", "ExceptionId");

                                    SingleApplicationID = Convert.ToInt32(FC[TempKeyStr]);
                                    SP_ExceptionSanctionIndex_Result TesmpExceptionSanction = WetosDB.SP_ExceptionSanctionIndex(EmpNo).Where(a => a.ExceptionId == SingleApplicationID).FirstOrDefault(); //.ToList();

                                    if (TesmpExceptionSanction != null)
                                    {
                                        TesmpExceptionSanction.StatusId = SingleStatusID;// UPDATE STATUS
                                        ExceptionSanction.Add(TesmpExceptionSanction);

                                        StatusIdSelected = true;
                                    }
                                }
                            }
                        }

                    }

                    // Added by Rajas on 20 MAY 2017
                    if (StatusIdSelected == false)
                    {
                        Information("Please select atleast one entry for processing");

                        PopulateDropDown();

                        return View(ExceptionSanction);
                    }

                }
                #endregion

                // 3 - Rejected, 2 - sactined, 1- Pending, 4 - Approved
                ExceptionSanctionList = ExceptionSanction.Where(a => a.StatusId == 3 || a.StatusId == 6 || a.StatusId == 5).ToList(); // EXCEPTION HANDLING

                foreach (SP_ExceptionSanctionIndex_Result item in ExceptionSanction)
                {
                    WetosDB.ExceptionEntry ExceptionObj = new WetosDB.ExceptionEntry();

                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 12 SEPTEMBER 2017 FOR EDIT
                    WetosDB.ExceptionEntry ExceptionObjEDIT = new WetosDB.ExceptionEntry();

                    foreach (var ExceptionSanctionObj in ExceptionSanctionList)
                    {

                        ExceptionObj = WetosDB.ExceptionEntries.Where(a => a.ExceptionId == ExceptionSanctionObj.ExceptionId).FirstOrDefault();

                        if (((ExceptionObj.StatusId != 3 && ExceptionSanctionObj.StatusId == 3)
                            || (ExceptionObj.StatusId != 6 && ExceptionSanctionObj.StatusId == 6)
                            || (ExceptionObj.StatusId != 5 && ExceptionSanctionObj.StatusId == 5))) //ADDED BY SHRADDHA ON 12 SEP 2017 FOR STATUSID = 5
                        {
                            if (string.IsNullOrEmpty(RejectReasonText))
                            {
                                ModelState.AddModelError("CustomError", "Please Enter Reject/Cancel Reason");

                                ExceptionSanctionList = WetosDB.SP_ExceptionSanctionIndex(EmpNo).OrderByDescending(a => a.ExceptionId).ToList();

                                PopulateDropDown();

                                return View(ExceptionSanctionList);
                            }
                        }

                    }

                    int EmpId = Convert.ToInt32(Session["EmployeeNo"]);
                    ExceptionObj = WetosDB.ExceptionEntries.Where(a => a.ExceptionId == item.ExceptionId).FirstOrDefault();

                    if (ExceptionObj.StatusId != item.StatusId)
                    {

                    }
                    else
                    {
                        ExceptionObj.StatusId = item.StatusId;
                        WetosDB.SaveChanges();
                    }
                    if (item.StatusId != null)
                    {
                        //LeaveEncashObj.Status = item.Status;
                        int StatusInt = Convert.ToInt32(ExceptionObj.StatusId);
                        //LeaveEncashObj.Status = WetosDB.DropdownDatas.Where(a => a.TypeId == 3 && a.Value == StatusInt).Select(a => a.Text).FirstOrDefault();

                        WetosDB.DailyTransaction ExceptionEntry = WetosDB.DailyTransactions.Where(b => b.EmployeeId == ExceptionObj.EmployeeId
                            && b.TranDate == ExceptionObj.ExceptionDate).FirstOrDefault();

                        Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == ExceptionEntry.EmployeeId).FirstOrDefault();

                        int EmployeeCompanyId = EmployeeObj.CompanyId;
                        int EmployeeBranchId = EmployeeObj.BranchId;

                        /// Revert Original punch data to repective login or logout column in case of reject or cancel
                        /// Added by Rajas on 18 JUNE 2017 START
                        if ((item.StatusId == 5 || item.StatusId == 3 || item.StatusId == 6) && ExceptionObj.StatusId == 2) // updated item.statusid = 5 instead of 1 by Shraddha on 11 sep 2017
                        {
                            ExceptionEntry.Login = ExceptionEntry.PreviousLogin == null ? new DateTime(ExceptionEntry.TranDate.Year, ExceptionEntry.TranDate.Month, ExceptionEntry.TranDate.Day, 00, 00, 00) : ExceptionEntry.PreviousLogin.Value;
                            ExceptionEntry.LogOut = ExceptionEntry.PreviousLogOut == null ? new DateTime(ExceptionEntry.TranDate.Year, ExceptionEntry.TranDate.Month, ExceptionEntry.TranDate.Day, 00, 00, 00) : ExceptionEntry.PreviousLogOut.Value;
                            ExceptionEntry.PreviousLogOut = new DateTime(ExceptionEntry.TranDate.Year, ExceptionEntry.TranDate.Month, ExceptionEntry.TranDate.Day, 00, 00, 00);
                            ExceptionEntry.PreviousLogin = new DateTime(ExceptionEntry.TranDate.Year, ExceptionEntry.TranDate.Month, ExceptionEntry.TranDate.Day, 00, 00, 00);

                            // Added by Rajas on 20 JUNE 2017 to restore original status
                            ExceptionEntry.Status = ExceptionEntry.ActualStatus;
                            ExceptionEntry.ActualStatus = string.Empty;

                            WetosDB.SaveChanges();

                            //CODE ADDED BY PUSHKAR ON 26 MAY 2018 FOR CANCEL CUMM COMP OFF ON EXCEPTION ENTRY CANCEL START
                            DateTime ExpDateCOff = ExceptionObj.ExceptionDate.Value;

                            CumulativeManualCompOff CumCoffForExp = WetosDB.CumulativeManualCompOffs.Where(a => a.FromDate == ExpDateCOff && a.EmployeeId == ExceptionObj.EmployeeId).FirstOrDefault();
                            if (CumCoffForExp != null)
                            {
                                CumCoffForExp.StatusId = 5;
                            }
                            WetosDB.SaveChanges();


                            CumulativeCompOff CumCompOffUtilizeExp = WetosDB.CumulativeCompOffs.Where(a => a.WoDate == ExpDateCOff && a.EmployeeId == ExceptionObj.EmployeeId).FirstOrDefault();
                            if (CumCompOffUtilizeExp != null)
                            {
                                CumCompOffUtilizeExp.AppStatus = "CM";
                                CumCompOffUtilizeExp.BalanceCoHours = 0;
                            }

                            WetosDB.SaveChanges();
                            //CODE ADDED BY PUSHKAR ON 26 MAY 2018 FOR CANCEL CUMM COMP OFF ON EXCEPTION ENTRY CANCEL END
                        }
                        /// Added by Rajas on 18 JUNE 2017 END

                        //Added By Shraddha on 14 DEC 2016 for reduce No. Of Leave Days from LeaveBalance Table after sanctioning the leaves start
                        if (item.StatusId == 2)
                        {
                            // Validate Valid employee from obj, Added by Rajas on 17 APRIL 2017
                            VwActiveEmployee ValidEmpObj = WetosDB.VwActiveEmployees.Where(a => a.EmployeeId == ExceptionObj.EmployeeId).FirstOrDefault();

                            if (ValidEmpObj != null)
                            {
                                if (LoginPersonIsSanctioner(ValidEmpObj.EmployeeId, EmpId) || LoginPersonIsApprover(ValidEmpObj.EmployeeId, EmpId)) //UPDATED BY SHRADDHA ON 12 SEP 2017 TAKEN LoginPersonIsSanctioner() AND LoginPersonIsApprover() FUNCTIONS
                                {
                                    //CODE COMMENTED AND ADDED NEW CODE BY SHRADDHA ON 16 JAN 2018 START
                                    //DateTime LoginDate = Convert.ToDateTime(ExceptionEntry.TranDate.ToShortDateString() + " " + ExceptionObj.LoginTime.ToShortTimeString());
                                    //DateTime LogOutDate = Convert.ToDateTime(ExceptionEntry.TranDate.ToShortDateString() + " " + ExceptionObj.LogOutTime.ToShortTimeString());
                                    DateTime LoginDate = ExceptionObj.LoginTime;
                                    DateTime LogOutDate = ExceptionObj.LogOutTime;
                                    //CODE COMMENTED AND ADDED NEW CODE BY SHRADDHA ON 16 JAN 2018 END


                                    // Save old login punch data 
                                    ExceptionEntry.PreviousLogin = ExceptionEntry.Login; // Added by Rajas on 17 JUNE 2017
                                    ExceptionEntry.Login = LoginDate;

                                    // Save old logout punch data
                                    ExceptionEntry.PreviousLogOut = ExceptionEntry.LogOut;  // Added by Rajas on 17 JUNE 2017
                                    ExceptionEntry.LogOut = LogOutDate;

                                    // Store original status 
                                    ExceptionEntry.ActualStatus = ExceptionEntry.Status; // Added by Rajas on 20 JUNE 2017
                                    ExceptionEntry.Status = WetosAdministrationController.StatusConstants.FullDayPresentStatus;
                                    // Updated by Rajas on 18 AUGUST 2017


                                    //CODE ADDED BY SHRADDHA ON 04 APR 2017 TO CALCULATE LATE STRING AND WORKING HOURS IN DAILYTRANSACTION FROM LOGOUT AND LOGIN TIME START
                                    //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 16 JAN 2018 START
                                    //string WorkingHrsString = (LogOutDate.TimeOfDay - LoginDate.TimeOfDay).ToString();
                                    string WorkingHrsString = LogOutDate.Subtract(LoginDate).ToString();
                                    //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 16 JAN 2018 END

                                    string WorkingHrs = LoginDate.ToShortDateString() + " " + WorkingHrsString;
                                    TimeSpan CalcTimeDiff = LogOutDate.Subtract(LoginDate);
                                    string WorkingHrsEx = CalcTimeDiff.ToString(@"hh\:mm");

                                    ExceptionEntry.WorkingHrs = Convert.ToDateTime(WorkingHrs);
                                    ExceptionEntry.Late = null;
                                    ExceptionEntry.Early = null;
                                    ExceptionEntry.PreviousShiftId = ExceptionEntry.ShiftId; // CODE ADDED BY SHRADDHA ON 16 JAN 2018
                                    ExceptionEntry.ShiftId = item.ShiftId; // CODE ADDED BY SHRADDHA ON 16 JAN 2018
                                    WetosDB.SaveChanges();

                                    #region  FIND NEAREST SHIFT FROM IN OUT AND MARK LATE / EARLY

                                    // Code Updated by Rajas on 18 AUGUST 2017 START
                                    List<RuleTransaction> AllRuleTransactionList = WetosDB.RuleTransactions.ToList();
                                    int EmployeeGroupIdObj = WetosDB.EmployeeGroupDetails.Where(a => a.Employee.EmployeeId == ExceptionEntry.EmployeeId).Select(a => a.EmployeeGroup.EmployeeGroupId).FirstOrDefault();

                                    List<RuleTransaction> RuleTransactionList = AllRuleTransactionList.Where(a => a.EmployeeGroupId == EmployeeGroupIdObj).ToList();

                                    DateTime Intime = ExceptionEntry.Login;
                                    DateTime OutTime = ExceptionEntry.LogOut;

                                    // Added by Rajas on 14 JUNE 2017
                                    string Today = Intime.DayOfWeek.ToString();

                                    //Added By Shraddha on 10 JAN 2017 to find the Nearest Time Shift
                                    if (ExceptionEntry.Status != WetosAdministrationController.StatusConstants.FullDayAbsentStatus
                                        && ExceptionEntry.Status != WetosAdministrationController.StatusConstants.WeeklyOffStatus
                                        && ExceptionEntry.Status != WetosAdministrationController.StatusConstants.HoliDayStatus) // ????
                                    {
                                        #region COMMENTED CODE
                                        //#region ADDED NEW CODE BY SHRADDHA ON 16 JAN 2018 FOR TAKING SHIFT FROM EXCEPTION ENTRY SELECTED NEW SHIFT
                                        ////CODE CHANGED BY SHRADDHA ON 16 JAN 2018 FOR TAKING SHIFT EXCEPTION ENTRY SELECTED NEW SHIFT START
                                        //GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.MarkHalfDayShift).FirstOrDefault();
                                        //Shift ShiftObjForCurrentEmployee = WetosDB.Shifts.Where(a => a.ShiftCode == item.ShiftId).FirstOrDefault();

                                        //if (ShiftObjForCurrentEmployee == null)
                                        //{
                                        //    ShiftObjForCurrentEmployee = new Shift();
                                        //}
                                        ////CODE CHANGED BY SHRADDHA ON 16 JAN 2018 FOR TAKING SHIFT FROM EXCEPTION ENTRY SELECTED NEW SHIFT END
                                        //#endregion

                                        //#region LATE/EARLY Logic

                                        ///// IsLateCountToBeIncremented YES/NO
                                        ///// Flag added by Rajas on 28 APRIL 2017
                                        //bool IsLateCountToBeIncremented = true;

                                        ///// IsEarlyCountToBeIncremented YES/NO
                                        ///// Flag added by Rajas on 28 APRIL 2017
                                        //bool IsEarlyCountToBeIncremented = true;

                                        //bool IsUpdateStatus = false;  // Added by Rajas on 4 AUGUST 2017

                                        //// Initialise GlobalSettingObj
                                        //GlobalSettingObj = new GlobalSetting();
                                        ////GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText.Trim() == "Is Cont Late Deduction").FirstOrDefault();

                                        ////ABOVE LINE COMMENTED AND BELOW LINE IS ADDED BY SHALAKA ON 13TH DEC 2017
                                        //GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText.Trim() == GlobalSettingsConstant.IsContLateDeduction).FirstOrDefault();

                                        //if (ExceptionEntry.WorkingHrs != null)
                                        //{
                                        //    #region CODE FOR MINIMUM WORKING HOURS LIMIT FOR FULL DAY
                                        //    #region CODE ADDED BY SHRADDHA ON 15 MAR 2018 NEED TO VERIFY BY MSJ
                                        //    string MarkAsPresentFullDayLimit = RuleTransactionList.Where(a => a.RuleId == 21).Select(a => a.Formula).FirstOrDefault(); //Rule 21 - Mark As Present Full Day Limit
                                        //    if (!string.IsNullOrEmpty(MarkAsPresentFullDayLimit))
                                        //    {
                                        //        string[] MarkAsPresentFullDayLimitArray = MarkAsPresentFullDayLimit.Split(':');
                                        //        int MarkAsPresentFullDayLimitHour = Convert.ToInt32(MarkAsPresentFullDayLimitArray[0]);
                                        //        int MarkAsPresentFullDayLimitMinute = Convert.ToInt32(MarkAsPresentFullDayLimitArray[1]);
                                        //        int MarkAsPresentFullDayLimitTotalValue = (MarkAsPresentFullDayLimitHour * 60) + MarkAsPresentFullDayLimitMinute;

                                        //        int WorkingHoursTotal = (ExceptionEntry.WorkingHrs.Value.Hour * 60) + ExceptionEntry.WorkingHrs.Value.Minute;

                                        //        if (WorkingHoursTotal > MarkAsPresentFullDayLimitTotalValue)
                                        //        {
                                        //            ExceptionEntry.Status = WetosAdministrationController.StatusConstants.FullDayPresentStatus; //"PPPP";
                                        //            WetosDB.SaveChanges();
                                        //        }
                                        //        else if (WorkingHoursTotal == MarkAsPresentFullDayLimitTotalValue)
                                        //        {
                                        //            ExceptionEntry.Status = WetosAdministrationController.StatusConstants.SecondHalfAbsentStatus; //"PPAA";
                                        //            WetosDB.SaveChanges();
                                        //        }
                                        //        else
                                        //        {
                                        //            ExceptionEntry.Status = WetosAdministrationController.StatusConstants.FullDayAbsentStatus; //"AAAA";
                                        //            WetosDB.SaveChanges();
                                        //        }
                                        //    }
                                        //    #endregion
                                        //    #endregion
                                        //}

                                        //#region LOGIC FOR LATE MARK AND SHIFT

                                        //string ReturnMessage = string.Empty;
                                        //string UpdatedStatus = string.Empty;
                                        //string NewStatus = string.Empty;

                                        //// Rest all flags
                                        //PostingFlagModelObj = new PostingFlagModel(); // Added by Rajas on 8 AUGUST 2017

                                        //// LOGIC FOR LATE MARK AND SHIFT
                                        //if (ShiftObjForCurrentEmployee == null)
                                        //{
                                        //    ShiftObjForCurrentEmployee = WetosDB.Shifts.FirstOrDefault();
                                        //}

                                        //DateTime FirstInTime = Convert.ToDateTime(ShiftObjForCurrentEmployee.FirstInTime);
                                        //DateTime FirstOutTime = Convert.ToDateTime(ShiftObjForCurrentEmployee.FirstOutTime);

                                        ////CODE ADDED BY SHRADDHA ON 03 APR 2017 FOR TAKING SECOND OUT TIME
                                        //DateTime SecondInTime = Convert.ToDateTime(ShiftObjForCurrentEmployee.SecondInTime);
                                        //DateTime SecondOutTime = Convert.ToDateTime(ShiftObjForCurrentEmployee.SecondOutTime);

                                        //// LATE COMING GRACE TIME FROM RULE 5
                                        ////int LCGraceTimeMinutes = 00;

                                        //if (RuleTransactionList != null)
                                        //{
                                        //    // 5 - allowed late comming min
                                        //    string LateComingAllowedMinutesStr = RuleTransactionList.Where(a => a.RuleId == 5).Select(a => a.Formula).FirstOrDefault();

                                        //    if (!string.IsNullOrEmpty(LateComingAllowedMinutesStr))
                                        //    {
                                        //        string[] LateComingAllowedMinutesStrArray = LateComingAllowedMinutesStr.Split(':');

                                        //        //COMMENTED BY SHRADDHA ON 03 APR 2017 BECAUSE IT IS NOT REQUIREED IN CASE OF FLAGSHIP
                                        //        //  LCGraceTimeMinutes = Convert.ToInt32(LateComingAllowedMinutesStrArray[0]) * 60 + Convert.ToInt32(LateComingAllowedMinutesStrArray[1]);
                                        //    }

                                        //    /// Added by Rajas on 28 APRIL 2017 START
                                        //    /// To check GraceLateAllowed limit

                                        //    // 4 - Grace late allowed limit
                                        //    string GraceLateAllowedLimit = RuleTransactionList.Where(a => a.RuleId == 4).Select(a => a.Formula).FirstOrDefault();

                                        //    DateTime GraceLateAllowedMin = Convert.ToDateTime(GraceLateAllowedLimit);

                                        //    TimeSpan LCGraceTimeMinutes = GraceLateAllowedMin.TimeOfDay;
                                        //    /// Added by Rajas on 28 APRIL 2017 END


                                        //    // Below line for FirstInTime modified by Rajas on 18 APRIL 2017
                                        //    if (Intime.TimeOfDay > FirstInTime.TimeOfDay)  // SecondInTime.TimeOfDay
                                        //    {
                                        //        //Modified By Shraddha on 23 FEB 2017 Taken Late minutes count from second in time instead of taking first in time as Per discuss with Nidhi mam Start

                                        //        // Below line for FirstInTime modified by Rajas on 18 APRIL 2017
                                        //        string LateTimeString = (Intime.TimeOfDay - FirstInTime.TimeOfDay).ToString();  // SecondInTime.TimeOfDay
                                        //        //Modified By Shraddha on 23 FEB 2017 Taken Late minutes count from second in time instead of taking first in time as Per discuss with Nidhi mam Start
                                        //        string Latestring = Intime.ToShortDateString() + " " + LateTimeString;

                                        //        // Added by Rajas on 28 APRIL 2017 START
                                        //        // To deduct Half day directly from Attendance, if employee crosses LateAllowed limit 
                                        //        DateTime LateAllowedLimit = Convert.ToDateTime(LateComingAllowedMinutesStr);
                                        //        DateTime CalculatedLateTime = Convert.ToDateTime(LateTimeString);

                                        //        // Save original attendance status
                                        //        // Added by Rajas on 8 AUGUST 2017
                                        //        ExceptionEntry.ActualStatus = ExceptionEntry.Status;

                                        //        if (CalculatedLateTime > GraceLateAllowedMin)
                                        //        {
                                        //            PostingFlagModelObj.IsLate = true;

                                        //            WetosAdministrationController.GetActualDailyTransactionStatus(PostingFlagModelObj, ExceptionEntry.Status, NewStatus, ref UpdatedStatus, ref ReturnMessage);
                                        //            ExceptionEntry.Status = UpdatedStatus;
                                        //            // Updated by Rajas on 8 AUGUST 2017 END
                                        //        }

                                        //        if (CalculatedLateTime > LateAllowedLimit)
                                        //        {
                                        //            //DailyTransactionObj.Status = "AAPP^";  // AAPP
                                        //            ExceptionEntry.Remark = "Late";

                                        //            // Updated by Rajas on 8 AUGUST 2017 START
                                        //            PostingFlagModelObj.IsFirstHalfAbsentDueToLatecoming = true;

                                        //            WetosAdministrationController.GetActualDailyTransactionStatus(PostingFlagModelObj, ExceptionEntry.Status, NewStatus, ref UpdatedStatus, ref ReturnMessage);
                                        //            ExceptionEntry.Status = UpdatedStatus;
                                        //            // Updated by Rajas on 8 AUGUST 2017 END

                                        //            /// NOTE:
                                        //            /// As per Discussion in Meeting between Katre sir and Deepti Madam following flag included
                                        //            /// If Employee is late beyond late allowed limit then deduct Half day directly
                                        //            /// In this case don't update late count as action already taken for late employee.
                                        //            IsLateCountToBeIncremented = false;
                                        //        }
                                        //        // Added by Rajas on 28 APRIL 2017 END

                                        //        // late by min
                                        //        ExceptionEntry.Late = Convert.ToDateTime(Latestring);
                                        //        //WetosDB.SaveChanges();

                                        //        int EmployeeId = Convert.ToInt32(ExceptionEntry.EmployeeId);
                                        //        int TranDateMonth = Convert.ToInt32(ExceptionEntry.TranDate.Month);
                                        //        int TranDateYear = Convert.ToInt32(ExceptionEntry.TranDate.Year);

                                        //        // Below code updated by Rajas on 18 APRIL 2017, as late time calculate from FirstInTime not from SecondInTime
                                        //        TimeSpan LateBy = Intime.TimeOfDay - FirstInTime.TimeOfDay; // SecondInTime.TimeOfDay;

                                        //        // Updated by Rajas on 28 APRIL 2017, && IsLateCountToBeIncremented == true added
                                        //        if (LateBy.TotalMinutes > LCGraceTimeMinutes.TotalMinutes && IsLateCountToBeIncremented == true)
                                        //        {
                                        //            //int TotalLateCountForEmployeeTillDate = Convert.ToInt32(WetosDB.DailyTransactions.Where(a => a.EmployeeId == EmployeeId
                                        //            //   && a.TranDate.Month == TranDateMonth && a.TranDate.Year == TranDateYear && a.LateCount != null).Count());

                                        //            int MaxLateCountForEmployee = Convert.ToInt32(WetosDB.DailyTransactions.Where(a => a.EmployeeId == EmployeeId
                                        //                && a.TranDate.Month == TranDateMonth && a.TranDate.Year == TranDateYear).Select(a => a.LateCount).Max());

                                        //            // Modified by Rajas on 21 APRIL 2017 for getting allowed late count in a month from rule
                                        //            RuleTransaction RuleForLateCount = RuleTransactionList.Where(a => a.RuleId == 16).FirstOrDefault();

                                        //            if (RuleForLateCount != null)
                                        //            {
                                        //                int LateAllowed = Convert.ToInt32(RuleForLateCount.Formula);

                                        //                if (MaxLateCountForEmployee == 0)
                                        //                {
                                        //                    ExceptionEntry.LateCount = MaxLateCountForEmployee + 1;
                                        //                }
                                        //                else
                                        //                {
                                        //                    ExceptionEntry.LateCount = MaxLateCountForEmployee + 1;
                                        //                }

                                        //                // Added by Rajas on 12 AUGUST 2017 
                                        //                // Global setting value base deduction 
                                        //                if (GlobalSettingObj == null)   // Deduct half day on basis of multiple of allowed late count
                                        //                {
                                        //                    if (ExceptionEntry.LateCount % LateAllowed == 0)
                                        //                    {
                                        //                        ExceptionEntry.Status = WetosAdministrationController.StatusConstants.Absent + ExceptionEntry.Status.Substring(2, 2) + "^"; //"AAPP^";
                                        //                        ExceptionEntry.Remark = "Late";
                                        //                    }
                                        //                }
                                        //                else   // Deduct half day after every allowed late count 
                                        //                {
                                        //                    if (GlobalSettingObj.SettingValue == "1" && ExceptionEntry.LateCount > LateAllowed)
                                        //                    {
                                        //                        ExceptionEntry.Status = WetosAdministrationController.StatusConstants.Absent + ExceptionEntry.Status.Substring(2, 2) + "^"; //"AAPP^";
                                        //                        ExceptionEntry.Remark = "Late";
                                        //                    }
                                        //                }

                                        //            }
                                        //            //CODE ADDED BY SHRADDHA ON 04 EB 2017 FOR HALF DAY MARK AFTER AllowedLimit LATECOUNTS END

                                        //            WetosDB.SaveChanges();

                                        //            IsUpdateStatus = true;


                                        //        }
                                        //        else  // Added by Rajas on 5 AUGUST 2017
                                        //        {

                                        //        }


                                        //    }
                                        //}
                                        //// LATE COMING LOGIC END
                                        //#endregion

                                        //if (ExceptionEntry.WorkingHrs != null)
                                        //{
                                        //    #region CODE FOR CONSIDERING STATUS AS AAAA IF  WORKINGHRS MINUTES ARE ZERO

                                        //    // What will happen in case of night shift

                                        //    //CONDITION ADDED BY SHRADDHA ON 28 MARCH 2017 TO PREVENT CONSIDERING STATUS AS AAAA IF  WORKINGHRS MINUTES ARE ZERO IF TRANDATE IS EQUAL TO TODAY START
                                        //    if (ExceptionEntry.TranDate.ToShortDateString() != DateTime.Now.ToShortDateString())
                                        //    {
                                        //        //CONDITION ADDED BY SHRADDHA ON 28 MARCH 2017 TO PREVENT CONSIDERING STATUS AS AAAA IF  WORKINGHRS MINUTES ARE ZERO IF TRANDATE IS EQUAL TO TODAY START
                                        //        //CODE ADDED BY SHRADDHA ON 02 FEB 2017 FOR CONSIDERING STATUS AS AAAA IF  WORKINGHRS MINUTES ARE ZERO START
                                        //        if (ExceptionEntry.WorkingHrs.Value.Minute == 0 && ExceptionEntry.WorkingHrs.Value.Hour == 0
                                        //         && ExceptionEntry.WorkingHrs.Value.Second == 0)
                                        //        {
                                        //            ExceptionEntry.Status = WetosAdministrationController.StatusConstants.FullDayAbsentStatus; //"AAAA";
                                        //            WetosDB.SaveChanges();
                                        //        }
                                        //    }
                                        //    //CODE ADDED BY SHRADDHA ON 02 FEB 2017 FOR CONSIDERING STATUS AS AAAA IF  WORKINGHRS MINUTES ARE ZERO END
                                        //    #endregion

                                        //    #region  EARLY GOING LOGIC
                                        //    // EARLY GOING LOGIC START
                                        //    //EARLY GOING 

                                        //    // Logic need to be updated as per late count

                                        //    // Early Go Code Modifed by Rajas on 28 APRIL 2017 START

                                        //    ReturnMessage = string.Empty;
                                        //    UpdatedStatus = string.Empty;
                                        //    NewStatus = string.Empty;

                                        //    // Rest all flags
                                        //    PostingFlagModelObj = new PostingFlagModel(); // Added by Rajas on 8 AUGUST 2017

                                        //    if (RuleTransactionList != null)
                                        //    {

                                        //        // 6 - allowed Early going min
                                        //        string EarlyGoAllowedMinutesStr = RuleTransactionList.Where(a => a.RuleId == 6).Select(a => a.Formula).FirstOrDefault();

                                        //        if (!string.IsNullOrEmpty(EarlyGoAllowedMinutesStr))
                                        //        {
                                        //            string[] LateComingAllowedMinutesStrArray = EarlyGoAllowedMinutesStr.Split(':');
                                        //        }

                                        //        /// Added by Rajas on 28 APRIL 2017 START
                                        //        /// To check GraceLateAllowed limit

                                        //        // 3 - Grace early allowed limit
                                        //        string GraceEarlyAllowedLimit = RuleTransactionList.Where(a => a.RuleId == 3).Select(a => a.Formula).FirstOrDefault();

                                        //        if (!string.IsNullOrEmpty(GraceEarlyAllowedLimit) && !string.IsNullOrEmpty(EarlyGoAllowedMinutesStr))
                                        //        {

                                        //            DateTime GraceEarlyAllowedMin = Convert.ToDateTime(GraceEarlyAllowedLimit);

                                        //            TimeSpan LCGraceTimeMinutes = GraceEarlyAllowedMin.TimeOfDay;
                                        //            /// Added by Rajas on 28 APRIL 2017 END

                                        //            if (OutTime.TimeOfDay < FirstOutTime.TimeOfDay)
                                        //            {
                                        //                string EarlyTimeString = (FirstOutTime.TimeOfDay - OutTime.TimeOfDay).ToString();

                                        //                string Earlystring = OutTime.ToShortDateString() + " " + EarlyTimeString;

                                        //                // Added by Rajas on 28 APRIL 2017 START
                                        //                // To deduct Half day directly from Attendance, if employee crosses EarlyAllowed limit 
                                        //                DateTime EarlyAllowedLimit = Convert.ToDateTime(EarlyGoAllowedMinutesStr);
                                        //                DateTime CalculatedEarlyTime = Convert.ToDateTime(EarlyTimeString);

                                        //                // Save original attendance status
                                        //                // Added by Rajas on 8 AUGUST 2017
                                        //                ExceptionEntry.ActualStatus = ExceptionEntry.Status;

                                        //                if (CalculatedEarlyTime > GraceEarlyAllowedMin)
                                        //                {
                                        //                    // Added by Rajas on 29 APRIL 2017
                                        //                    if (!IsUpdateStatus)
                                        //                    {
                                        //                        // Updated by Rajas on 8 AUGUST 2017 START
                                        //                        PostingFlagModelObj.IsLate = true;

                                        //                        WetosAdministrationController.GetActualDailyTransactionStatus(PostingFlagModelObj, ExceptionEntry.Status, NewStatus, ref UpdatedStatus, ref ReturnMessage);
                                        //                        ExceptionEntry.Status = UpdatedStatus;
                                        //                        // Updated by Rajas on 8 AUGUST 2017 END
                                        //                    }
                                        //                }

                                        //                if (CalculatedEarlyTime > EarlyAllowedLimit)
                                        //                {
                                        //                    if (!IsUpdateStatus)
                                        //                    {
                                        //                        // Updated by Rajas on 8 AUGUST 2017 START
                                        //                        PostingFlagModelObj.IsSecondHalfAbsentDueToEarlygoing = true;

                                        //                        WetosAdministrationController.GetActualDailyTransactionStatus(PostingFlagModelObj, ExceptionEntry.Status, NewStatus, ref UpdatedStatus, ref ReturnMessage);
                                        //                        ExceptionEntry.Status = UpdatedStatus;
                                        //                        // Updated by Rajas on 8 AUGUST 2017 END
                                        //                    }

                                        //                    ExceptionEntry.Remark = "Early";

                                        //                    /// NOTE:
                                        //                    /// As per Discussion in Meeting between Katre sir and Deepti Madam following flag included
                                        //                    /// If Employee left early beyond early allowed limit then deduct Half day directly
                                        //                    /// In this case don't update late count as action already taken for late employee.
                                        //                    IsEarlyCountToBeIncremented = false;
                                        //                }
                                        //                // Added by Rajas on 28 APRIL 2017 END

                                        //                // Early by min
                                        //                ExceptionEntry.Early = Convert.ToDateTime(Earlystring);
                                        //                //WetosDB.SaveChanges();

                                        //                int EmployeeId = Convert.ToInt32(ExceptionEntry.EmployeeId);
                                        //                int TranDateMonth = Convert.ToInt32(ExceptionEntry.TranDate.Month);
                                        //                int TranDateYear = Convert.ToInt32(ExceptionEntry.TranDate.Year);

                                        //                // Below code updated by Rajas on 18 APRIL 2017, as late time calculate from FirstInTime not from SecondInTime
                                        //                TimeSpan EarlyBy = FirstOutTime.TimeOfDay - OutTime.TimeOfDay;  // SecondInTime.TimeOfDay;

                                        //                //Updated by Rajas on 28 APRIL 2017, && IsEarlyCountToBeIncremented == true added
                                        //                if (EarlyBy.TotalMinutes > LCGraceTimeMinutes.TotalMinutes && IsEarlyCountToBeIncremented == true)
                                        //                {
                                        //                    // EarlyCount Updated by Rajas on 18 MAY 2017
                                        //                    int MaxearlyCountForEmployee = Convert.ToInt32(WetosDB.DailyTransactions.Where(a => a.EmployeeId == EmployeeId
                                        //                     && a.TranDate.Month == TranDateMonth && a.TranDate.Year == TranDateYear).Select(a => a.EarlyCount).Max());

                                        //                    // Modified by Rajas on 21 APRIL 2017 for getting allowed late count in a month from rule
                                        //                    RuleTransaction RuleForEarlyCount = RuleTransactionList.Where(a => a.RuleId == 16).FirstOrDefault();

                                        //                    if (RuleForEarlyCount != null)
                                        //                    {
                                        //                        int EarlyAllowed = Convert.ToInt32(RuleForEarlyCount.Formula);

                                        //                        if (MaxearlyCountForEmployee == 0)
                                        //                        {
                                        //                            ExceptionEntry.EarlyCount = MaxearlyCountForEmployee + 1;
                                        //                        }

                                        //                        else
                                        //                        {
                                        //                            ExceptionEntry.EarlyCount = MaxearlyCountForEmployee + 1;
                                        //                        }

                                        //                        // Added by Rajas on 12 AUGUST 2017 
                                        //                        // Global setting value base deduction 
                                        //                        if (GlobalSettingObj == null)   // Deduct half day on basis of multiple of allowed late count limit
                                        //                        {
                                        //                            if (ExceptionEntry.EarlyCount % EarlyAllowed == 0)
                                        //                            {
                                        //                                ExceptionEntry.Status = ExceptionEntry.Status.Substring(0, 2) + WetosAdministrationController.StatusConstants.Absent + "^"; //"PPAA^";
                                        //                                ExceptionEntry.Remark = "Early";
                                        //                            }
                                        //                        }
                                        //                        else   // Deduct half day after every allowed early count limit 
                                        //                        {
                                        //                            if (GlobalSettingObj.SettingValue == "1" && ExceptionEntry.EarlyCount > EarlyAllowed)
                                        //                            {
                                        //                                ExceptionEntry.Status = ExceptionEntry.Status.Substring(0, 2) + WetosAdministrationController.StatusConstants.Absent + "^"; //"PPAA^";
                                        //                                ExceptionEntry.Remark = "Early";
                                        //                            }
                                        //                        }

                                        //                    }

                                        //                    WetosDB.SaveChanges();

                                        //                    //// Added by Rajas on 5 AUGUST 2017
                                        //                    //PrevEarlyCount = DailyTransactionObj.EarlyCount == null ? 0 : DailyTransactionObj.EarlyCount.Value;
                                        //                }
                                        //            }
                                        //            else  // Added by Rajas on 5 AUGUST 2017
                                        //            {
                                        //                //// Employee is Not left early, then
                                        //                //DailyTransactionObj.EarlyCount = PrevEarlyCount;
                                        //                //WetosDB.SaveChanges();
                                        //            }


                                        //        }


                                        //        ExceptionEntry.Remark = "ExceptionEntry Sanctioned";
                                        //        ExceptionEntry.LateCount = null;

                                        //        // MONTH DETAILS
                                        //        WetosDB.MonthDetail MonthDetailObj = WetosDB.MonthDetails.Where(a => a.Id == ExceptionEntry.TranDate.Month).FirstOrDefault();

                                        //        DateTime FirstDate = new DateTime(ExceptionEntry.TranDate.Year, ExceptionEntry.TranDate.Month, 1);
                                        //        DateTime LastDate = FirstDate.AddMonths(1).AddDays(-1);


                                        //        List<DailyTransaction> AllEmpLatetData = WetosDB.DailyTransactions.Where(a => a.TranDate > ExceptionEntry.TranDate
                                        //            && a.TranDate <= LastDate && (a.Remark.ToUpper() == "LATE") && a.EmployeeId == ExceptionEntry.EmployeeId).ToList();

                                        //        //Code Added By Shraddha on 06 JUNE 2017 To Get LateCount from Rules start
                                        //        //int EmployeeGroupIdObj = WetosDB.EmployeeGroupDetails.Where(a => a.Employee.EmployeeId == ExceptionEntry.EmployeeId).Select(a => a.EmployeeGroupId).FirstOrDefault();
                                        //        // List<RuleTransaction> RuleTransactionList = WetosDB.RuleTransactions.Where(a => a.CompanyId == ExceptionEntry.CompanyId && a.BranchId == ExceptionEntry.BranchId && a.EmployeeGroupId == EmployeeGroupIdObj).ToList();
                                        //        RuleTransaction RuleForLateCount = RuleTransactionList.Where(a => a.RuleId == 16).FirstOrDefault();

                                        //        //Code Added By Shraddha on 06 JUNE 2017 To Get LateCount from Rules end

                                        //        #region CODE TO DEDUCT ALREADY MARKED LATE COUNT

                                        //        NewStatus = string.Empty;
                                        //        UpdatedStatus = string.Empty;
                                        //        ReturnMessage = string.Empty;

                                        //        // Updated by Rajas on 5 AUGUST 2017 START
                                        //        foreach (DailyTransaction DailyTransObj in AllEmpLatetData)
                                        //        {
                                        //            if (RuleForLateCount != null)
                                        //            {
                                        //                if (DailyTransObj.LateCount >= 1)
                                        //                {
                                        //                    DailyTransObj.LateCount = DailyTransObj.LateCount - 1;
                                        //                    WetosDB.SaveChanges();

                                        //                    PostingFlagModelObj.IsLateCountReduced = true;

                                        //                    if (DailyTransObj.LateCount <= Convert.ToInt32(RuleForLateCount.Formula))
                                        //                    {
                                        //                        WetosAdministrationController.GetActualDailyTransactionStatus(PostingFlagModelObj, ExceptionEntry.Status, NewStatus, ref UpdatedStatus, ref ReturnMessage);
                                        //                        DailyTransObj.Remark = string.Empty;  // Updated by Rajas on 10 AUGUST 2017
                                        //                        DailyTransObj.Status = UpdatedStatus;
                                        //                        WetosDB.SaveChanges();
                                        //                    }

                                        //                    else if (DailyTransObj.LateCount > Convert.ToInt32(RuleForLateCount.Formula))
                                        //                    {
                                        //                        //abcd.Status = "AAPP^";
                                        //                        WetosAdministrationController.GetActualDailyTransactionStatus(PostingFlagModelObj, ExceptionEntry.Status, NewStatus, ref UpdatedStatus, ref ReturnMessage);
                                        //                        DailyTransObj.Remark = "Late";
                                        //                        WetosDB.SaveChanges();
                                        //                    }
                                        //                }
                                        //                // > 2 ABSENT
                                        //                // ==2 PRESENT 

                                        //                // Initialise flags
                                        //                PostingFlagModelObj = new PostingFlagModel();  // Added by Rajas on 8 AUGUST 2017
                                        //            }
                                        //        }
                                        //        // Updated by Rajas on 5 AUGUST 2017 END

                                        //        #endregion

                                        //    }
                                        //    // Early Go Code Modifed by Rajas on 28 APRIL 2017 END

                                        //    #endregion
                                        //}
                                        //#endregion
                                        #endregion

                                        string ReturnMessage = string.Empty;
                                        string UpdatedStatus = string.Empty;
                                        string NewStatus = string.Empty;

                                        WetosAdministrationController.ShiftExtraHrsLateEarly(WetosDB, ExceptionEntry, ExceptionEntry.TranDate, PostingFlagModelObj, ref NewStatus,
                                            ref UpdatedStatus, ref ReturnMessage);
                                    }

                                    // Code Updated by Rajas on 18 AUGUST 2017 END

                                    #endregion

                                    ExceptionEntry.ShiftId = item.ShiftId; // CODE ADDED BY SHRADDHA ON 16 JAN 2018

                                    ExceptionEntry.Remark = ExceptionObj.Remark;

                                    if (ExceptionEntry.LogOut > ExceptionEntry.Login)
                                    {
                                        ExceptionEntry.WorkingHrs = Convert.ToDateTime((ExceptionEntry.LogOut - ExceptionEntry.Login).ToString());
                                        if (ExceptionEntry.WorkingHrs.Value.Minute == 00 && ExceptionEntry.WorkingHrs.Value.Hour == 00 && ExceptionEntry.WorkingHrs.Value.Second == 00)
                                        {
                                            ExceptionEntry.Status = WetosAdministrationController.StatusConstants.FullDayAbsentStatus;  // Updated by Rajas on 18 AUGUST 2017
                                            WetosDB.SaveChanges();
                                        }

                                    }

                                    // ADDED BY MSJ ON 09 JAN 2019 START
                                    ExceptionObjEDIT.AppliedBy = Convert.ToInt32(Session["EmployeeNo"]);
                                    ExceptionObjEDIT.AppliedOn = DateTime.Now;
                                    ExceptionObjEDIT.SanctionedBy = Convert.ToInt32(Session["EmployeeNo"]);
                                    ExceptionObjEDIT.SanctionedOn = DateTime.Now;
                                    // ADDED BY MSJ ON 09 JAN 2019 END

                                    WetosDB.SaveChanges();
                                    // WetosAdministrationController.ExceptionProcessingEx(WetosDB, Convert.ToDateTime(ExceptionObj.ExceptionDate), Convert.ToDateTime(ExceptionObj.ExceptionDate), EmployeeObj.EmployeeId);


                                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 12 SEPTEMBER 2017
                                    #region ADD AUDIT LOG
                                    string Oldrecord = "SHIFT : " + ExceptionObjEDIT.ShiftId + ", LOGIN : "
                                    + ExceptionObjEDIT.LoginTime
                                    + ", LOGOUT : " + ExceptionObjEDIT.LogOutTime + ", REMARK : " + ExceptionObjEDIT.Remark
                                    + ", STATUS : " + ExceptionObjEDIT.Status;
                                    //---x---------x-----------------x------------x----------------x-----------x-------------x------------x--------------x----------x---------x-----
                                    string Newrecord = "SHIFT : " + ExceptionObj.ShiftId + ", LOGIN : "
                                    + ExceptionObj.LoginTime
                                    + ", LOGOUT : " + ExceptionObj.LogOutTime + ", REMARK : " + ExceptionObj.Remark
                                    + ", STATUS : " + ExceptionObj.Status;

                                    //LOGIN ID IS TAKEN FROM SESSION PERSISTER
                                    string Formname = "ADJUSTMENT SANCTION";
                                    //ACTION IS UPDATE
                                    string Message = " ";

                                    WetosAdministrationController.GenerateAuditLogsUpdate(WetosDB, ExceptionObj.EmployeeId, Formname, Oldrecord,
                                        Newrecord, ref Message);
                                    #endregion
                                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 12 SEPTEMBER 2017


                                    // Added by Rajas on 20 MAY 2017
                                    // Common POSTING code
                                    //ProcessAttendance(item.ExceptionDate.Value, item.ExceptionDate.Value);

                                    // NOTIFICATION ADDED BY RAJAS ON 31 DEC 2016
                                    #region EXCEPTION ENTRY SANCTION NOTIFICATION



                                    // Notification from Reporting person to Employee
                                    EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == ExceptionObj.EmployeeId).FirstOrDefault();
                                    Notification NotificationObj = new Notification();
                                    NotificationObj.FromID = EmployeeObj.EmployeeReportingId;
                                    NotificationObj.ToID = EmployeeObj.EmployeeId;
                                    NotificationObj.SendDate = DateTime.Now;
                                    StatusId = Convert.ToInt32(ExceptionObj.StatusId);
                                    string StatusName = WetosDB.DropdownDatas.Where(a => a.TypeId == 3 && a.Value == StatusId).Select(a => a.Text).FirstOrDefault();
                                    NotificationObj.NotificationContent = "Your Attendance Regularization entry application on " + ExceptionObj.ExceptionDate.Value.ToString("dd-MMM-yyyy") + " for " + EmployeeObj.FirstName + " " + EmployeeObj.LastName + " is " + StatusName;
                                    //NotificationObj.NotificationContent = "Leave applied from " + LeaveApplicationObj.FromDate.ToString("dd-MMM-yyyy") + " to " + LeaveApplicationObj.ToDate.ToString("dd-MMM-yyyy") + " is " + LeaveApplicationObj.Status;
                                    NotificationObj.ReadFlag = false;
                                    NotificationObj.SendDate = DateTime.Now;

                                    WetosDB.Notifications.AddObject(NotificationObj);

                                    WetosDB.SaveChanges();

                                    if (ExceptionObj.StatusId == 4 && ValidEmpObj.EmployeeReportingId == EmpNo)
                                    {

                                        // Notification from Reporting person to Employee
                                        Notification NotificationObj2 = new Notification();
                                        NotificationObj.FromID = EmployeeObj.EmployeeId;
                                        NotificationObj.ToID = EmployeeObj.EmployeeReportingId2;
                                        NotificationObj.SendDate = DateTime.Now;
                                        NotificationObj.NotificationContent = "Received Attendance Regularization entry application for sanctioning : " + ExceptionObj.ExceptionDate.Value.ToString("dd-MMM-yyyy") + " for " + EmployeeObj.FirstName + " " + EmployeeObj.LastName;
                                        //NotificationObj.NotificationContent = "Leave applied from " + LeaveApplicationObj.FromDate.ToString("dd-MMM-yyyy") + " to " + LeaveApplicationObj.ToDate.ToString("dd-MMM-yyyy") + " is " + LeaveApplicationObj.Status;
                                        NotificationObj.ReadFlag = false;
                                        NotificationObj.SendDate = DateTime.Now;

                                        WetosDB.Notifications.AddObject(NotificationObj2);

                                        WetosDB.SaveChanges();

                                    }

                                    // NOTIFICATION COUNT
                                    int NoTiCount = WetosDB.Notifications.Where(a => a.ToID == EmpId && (a.ReadFlag == false || a.ReadFlag == null)).Count();
                                    Session["NotificationCount"] = NoTiCount;

                                    #endregion

                                    // Code updated by Rajas on 13 JUNE 2017
                                    #region EMAIL
                                    // Added by Rajas on 19 JULY 2017 START
                                    //GlobalSetting GlobalSettingEmailObj = WetosDB.GlobalSettings.Where(a => a.SettingText == "Send Email").FirstOrDefault();

                                    //ABOVE LINE COMMENTED AND BELOW LINE IS ADDED BY SHALAKA ON 13TH DEC 2017
                                    GlobalSetting GlobalSettingEmailObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.SendEmail).FirstOrDefault();

                                    if (GlobalSettingEmailObj != null)
                                    {
                                        // Send email ON
                                        if (GlobalSettingEmailObj.SettingValue == "1")
                                        {

                                            // Added by Rajas on 30 MARCH 2017 START
                                            string EmailUpdateStatus = string.Empty;

                                            if (EmployeeObj.Email != null)
                                            {
                                                // Updated by Rajas on 19 JULY 2017 extra added parameter EmailFromWhichApplication
                                                if (SendEmail(EmployeeObj.Email, NotificationObj.NotificationContent, NotificationObj.NotificationContent, ref EmailUpdateStatus, "Exception Sanction") == false)
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

                                }
                                else // Validate and give message, Added by Rajas on 17 APRIL 2017
                                {
                                    Information("You can not sanction this Attendance Regularization entry as you are not sanctioner for selected employee");

                                    List<SP_ExceptionSanctionIndex_Result> ExSanctionListobj = WetosDB.SP_ExceptionSanctionIndex(EmpNo).OrderByDescending(a => a.ExceptionId).ToList();
                                    PopulateDropDown();
                                    return View(ExSanctionListobj);
                                }
                            }
                        }

                        //Added By Shraddha on 14 DEC 2016 for reduce No. Of Leave Days from LeaveBalance Table after sanctioning the leaves End

                        //Added By Shraddha on 14 DEC 2016 for add No. Of Leave Days in LeaveBalance Table after rejecting the sanctioned leave start


                        ExceptionObj.StatusId = item.StatusId;
                        ExceptionObj.Status = WetosDB.DropdownDatas.Where(a => a.TypeId == 3 && a.Value == ExceptionObj.StatusId).Select(a => a.Text).FirstOrDefault();//ADDED BY SHRADDHA ON 12 SEP 2017 TO SAVE STATUS

                        WetosDB.SaveChanges();
                    }

                }
                List<SP_ExceptionSanctionIndex_Result> ExceptionSanctionListobj = new List<SP_ExceptionSanctionIndex_Result>();
                // ADDED BY PUSHKAR FOR AuditLog ON 21 DEC 2016
                AddAuditTrail("Success - Attendance Regularization entry processed successfully");  // Updated by Rajas on 17 JAN 2017

                // Added by Rajas on 17 JAN 2017 START
                Success("Success - Attendance Regularization entry processed successfully");

                // Added by Rajas on 9 MAY 2017
                // Populate Status dropdown
                PopulateDropDown();

                return View(ExceptionSanctionListobj);

            }
            catch (System.Exception ex)
            {
                int EmpNo = Convert.ToInt32(Session["EmployeeNo"]);
                //List<SP_ExceptionSanctionIndex_Result> ExceptionSanctionListobj = WetosDB.SP_ExceptionSanctionIndex(EmpNo).OrderByDescending(a => a.ExceptionId).ToList();
                // Above line commented and below line added by Rajas on 18 JUNE 2017
                List<SP_ExceptionSanctionIndex_Result> ExceptionSanctionList = new List<SP_ExceptionSanctionIndex_Result>();

                AddAuditTrail("Error in Attendance Regularization sanction due to " + ex.Message + ( ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                Error("Attendance Regularization entry not sanctioned, please try again and verify!");
                PopulateDropDown();
                return View(ExceptionSanctionList);

            }

        }


        public ActionResult ExceptionEntryEdit(int id)
        {
            WetosDB.ExceptionEntry ExceptionEntryEdit = WetosDB.ExceptionEntries.Single(b => b.ExceptionId == id);

            var Shift = WetosDB.Shifts.Select(a => new { ShiftId = a.ShiftCode, ShiftName = a.ShiftName }).ToList();
            ViewBag.ShiftList = new SelectList(Shift, "ShiftId", "ShiftName").ToList();

            return View(ExceptionEntryEdit);
        }

        [HttpPost]
        public ActionResult ExceptionEntryEdit(WetosDB.ExceptionEntry ExceptionEntryEdit)
        {
            try
            {
                WetosDB.ExceptionEntry ExceptionEntry = WetosDB.ExceptionEntries.Where(b => b.ExceptionId == ExceptionEntryEdit.ExceptionId).FirstOrDefault();
                ExceptionEntry.ShiftId = ExceptionEntryEdit.ShiftId;
                ExceptionEntry.LoginTime = ExceptionEntryEdit.LoginTime;
                ExceptionEntry.LogOutTime = ExceptionEntryEdit.LogOutTime;
                ExceptionEntry.ExceptionDate = ExceptionEntryEdit.ExceptionDate;


                WetosDB.SaveChanges();


                // Added by Rajas on 17 JAN 2017 START
                AddAuditTrail("Success - Attendance Regularization entry sanctioned");  // Updated by Rajas on 17 JAN 2017

                Success("Success - Attendance Regularization entry sanctioned");
                // Added by Rajas on 17 JAN 2017 END


                return RedirectToAction("ExceptionEntryIndex");
            }
            catch (System.Exception ex)
            {
                // Added by Rajas on 17 JAN 2017 START
                AddAuditTrail("Error - Attendance Regularization entry sanctioned failed " + ex.Message);  // Updated by Rajas on 17 JAN 2017

                Error("Error - Attendance Regularization entry sanctioned failed due to " + ex.Message);
                // Added by Rajas on 17 JAN 2017 END

                //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 START
                PopulateDropDown();
                //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 END

                return View();
            }
        }

        ///// <summary>
        ///// LEAVE ENCASH SANCTION INDEX ADDED BY SHRADDHA ON 19 DEC 2016
        ///// </summary>
        ///// <returns></returns>
        public ActionResult LeaveEncashSanctionIndex()
        {
            int EmpNo = Convert.ToInt32(Session["EmployeeNo"]);
            List<SP_LeaveEncashSanctionIndex_Result> LeaveEncashSanctionList = WetosDB.SP_LeaveEncashSanctionIndex(EmpNo).OrderByDescending(a => a.LeaveEncashId).ToList();

            PopulateDropDown();
            return View(LeaveEncashSanctionList);
        }

        ///// <summary>
        ///// Added By Shraddha on 19 DEC 2016 For Leave Encash sanction Index POST used for multiple sanction, reject at selection of radio button
        ///// </summary>
        ///// <param name="LeaveSanction"></param>
        ///// <param name="FC"></param>
        ///// <returns></returns>
        [HttpPost]
        public ActionResult LeaveEncashSanctionIndex(List<SP_LeaveEncashSanctionIndex_Result> LeaveEncashSanction, FormCollection FC)
        {
            int EmpNo = Convert.ToInt32(Session["EmployeeNo"]);

            try
            {
                #region FAKE LOGIC

                LeaveEncashSanction = null; // new List<SP_LeaveSanctionIndex_Result>();

                // FAKE LOGIC MSJ

                int Id = Convert.ToInt32(Session["Id"]);

                //List<SP_WetosGetEmployeeLeaveSanction_Result> LeaveSanctionObj1 = WetosDB.SP_WetosGetEmployeeLeaveSanction(EmpNo).ToList();

                DateTime abc = Convert.ToDateTime("2016/01/01");

                #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //var EmployeeData = WetosDB.Employees.ToList();
                List<SP_VwActiveEmployee_Result> EmployeeData = WetosDB.SP_VwActiveEmployee(EmployeeId).ToList();
                #endregion

                List<SP_LeaveEncashSanctionIndex_Result> LeaveEncashSanctionObj = new List<SP_LeaveEncashSanctionIndex_Result>();

                string RejectReasonText = FC["RejectReasonText"];

                //List<SP_LeaveSanctionIndex_Result> LeaveSanctionObj = new List<SP_LeaveSanctionIndex_Result>();
                if (LeaveEncashSanction == null)
                {
                    LeaveEncashSanction = new List<SP_LeaveEncashSanctionIndex_Result>();

                    int SingleApplicationID = 0;
                    int SingleStatusID = 0;
                    foreach (var key in FC.AllKeys)
                    {
                        string KeyStr = key.ToString();

                        if (KeyStr.Contains("StatusId") == true)
                        {
                            SingleStatusID = Convert.ToInt32(FC[KeyStr]);

                            string TempKeyStr = KeyStr.Replace("StatusId", "LeaveEncashId");

                            SingleApplicationID = Convert.ToInt32(FC[TempKeyStr]);
                            SP_LeaveEncashSanctionIndex_Result TempLeaveEncashSanction = WetosDB.SP_LeaveEncashSanctionIndex(EmpNo).Where(a => a.LeaveEncashId == SingleApplicationID).FirstOrDefault(); //.ToList();

                            if (TempLeaveEncashSanction != null)
                            {
                                TempLeaveEncashSanction.StatusId = SingleStatusID;// UPDATE STATUS
                                LeaveEncashSanction.Add(TempLeaveEncashSanction);
                            }

                            //"[6].LeaveApplicationId"
                        }

                        //if (KeyStr.Contains("LeaveApplicationId") == true)
                        //{
                        //    SingleApplicationID = Convert.ToInt32(FC[KeyStr]);
                        //    SP_LeaveSanctionIndex_Result TesmpLeaveSanction = WetosDB.SP_LeaveSanctionIndex(EmpNo).Where(a => a.LeaveApplicationId == SingleApplicationID).FirstOrDefault(); //.ToList();

                        //    if(TesmpLeaveSanction != null)
                        //    {
                        //        LeaveSanction.Add(TesmpLeaveSanction);
                        //    }
                        //}

                    }

                }

                #region OLD FAKE LOGIC
                // FAKE LOGIC MSJ
                //int EmpNo = Convert.ToInt32(Session["EmployeeNo"]);
                //int Id = Convert.ToInt32(Session["Id"]);
                ////List<SP_WetosGetEmployeeLeaveSanction_Result> LeaveSanctionObj1 = WetosDB.SP_WetosGetEmployeeLeaveSanction(EmpNo).ToList();

                //DateTime abc = Convert.ToDateTime("2016/01/01");  // Verify ?

                //var EmployeeData = WetosDB.Employees.ToList();
                //List<SP_LeaveEncashSanctionIndex_Result> LeaveEncashSanctionObj = new List<SP_LeaveEncashSanctionIndex_Result>();
                //string RejectReasonText = FC["RejectReasonText"];

                //if (LeaveEncashSanction == null)
                //{
                //    foreach (var key in FC.AllKeys)
                //    {
                //        string KeyStr = key.ToString();
                //        int SingleApplicationID = 0;
                //        int SingleStatusID = 0;
                //        if (KeyStr.Contains("StatusId") == true)
                //        {
                //            SingleStatusID = Convert.ToInt32(FC[KeyStr]);
                //        }

                //        if (KeyStr.Contains("LeaveEncashId") == true)
                //        {
                //            SingleApplicationID = Convert.ToInt32(FC[KeyStr]);
                //            LeaveEncashSanction = WetosDB.SP_LeaveEncashSanctionIndex(EmpNo).Where(a => a.LeaveEncashId == SingleApplicationID).ToList();
                //        }

                //    }

                //}
                #endregion
                #endregion

                // MODIFIED BY MSJ ON 23 JAN 2018 START
                //LeaveEncashSanctionObj = LeaveEncashSanction.Where(a => a.Status.Trim() == "3").ToList();
                LeaveEncashSanctionObj = LeaveEncashSanction.Where(a => a.StatusId == 3).ToList();
                // MODIFIED BY MSJ ON 23 JAN 2018 END

                // List<SP_LeaveEncashSanctionIndex_Result> LeaveEncashSanctionObj = LeaveEncashSanction.Where(a => a.Status.Trim() == "3").ToList();
                if (LeaveEncashSanctionObj.Count() > 0 && (RejectReasonText == null || RejectReasonText == ""))
                {
                    ModelState.AddModelError("CustomError", "Please Enter Reject/Cancel Reason");

                    List<SP_LeaveEncashSanctionIndex_Result> LeaveEncashSanctionList = WetosDB.SP_LeaveEncashSanctionIndex(EmpNo).OrderByDescending(a => a.LeaveEncashId).ToList();

                    return View(LeaveEncashSanctionList);
                }
                else
                {
                    foreach (var i in LeaveEncashSanctionObj)
                    {
                        LeaveEncash LeaveEncashObj = WetosDB.LeaveEncashes.Where(a => a.LeaveEncashId == i.LeaveEncashId).FirstOrDefault();

                        LeaveEncashObj.RejectReason = RejectReasonText;
                        WetosDB.SaveChanges();
                    }

                    foreach (SP_LeaveEncashSanctionIndex_Result item in LeaveEncashSanction)
                    {

                        int EmpId = Convert.ToInt32(Session["EmployeeNo"]);
                        LeaveEncash LeaveEncashObj = WetosDB.LeaveEncashes.Where(a => a.LeaveEncashId == item.LeaveEncashId).FirstOrDefault();

                        // MODIFIED BY MSJ ON 23 JAN 2018 START
                        //if (item.Status != "0")
                        // MODIFIED BY MSJ ON 23 JAN 2018 END
                        if (item.StatusId != 0)  // MODIFIED BY MSJ ON 23 JAN 2018 END
                        {
                            //LeaveEncashObj.Status = item.Status;
                            int StatusInt = Convert.ToInt32(LeaveEncashObj.StatusId);
                            //LeaveEncashObj.Status = WetosDB.DropdownDatas.Where(a => a.TypeId == 3 && a.Value == StatusInt).Select(a => a.Text).FirstOrDefault();


                            //Added By Shraddha on 14 DEC 2016 for reduce No. Of Leave Days from LeaveBalance Table after sanctioning the leaves start

                            // MODIFIED BY MSJ ON 23 JAN 2018 START
                            //if (item.Status.Trim() == "2")

                            if (item.StatusId == 2)   // MODIFIED BY MSJ ON 23 JAN 2018 END
                            {
                                LeaveEncashObj.RejectReason = "";

                                var CurrentBal = WetosDB.LeaveBalances.Where(a => a.EmployeeId == LeaveEncashObj.EmployeeId && a.LeaveType == LeaveEncashObj.LeaveCode).Single();
                                var UpdatedBalance = CurrentBal.CurrentBalance - LeaveEncashObj.EncashValue;
                                var LeaveUsedObj = (CurrentBal.LeaveUsed == null ? 0 : CurrentBal.LeaveUsed) + LeaveEncashObj.EncashValue;
                                LeaveBalance LeaveBalanceObj = WetosDB.LeaveBalances.Where(a => a.EmployeeId == LeaveEncashObj.EmployeeId && a.LeaveType == LeaveEncashObj.LeaveCode).Single();
                                LeaveBalanceObj.CurrentBalance = UpdatedBalance;
                                LeaveBalanceObj.LeaveUsed = LeaveUsedObj;

                            }

                            //Added By Shraddha on 14 DEC 2016 for reduce No. Of Leave Days from LeaveBalance Table after sanctioning the leaves End

                            //Added By Shraddha on 14 DEC 2016 for add No. Of Leave Days in LeaveBalance Table after rejecting the sanctioned leave start
                            // MODIFIED BY MSJ ON 23 JAN 2018 START
                            //if (LeaveEncashObj.Status.Trim() == "2" && item.Status == "3")
                            if (LeaveEncashObj.StatusId == 2 && item.StatusId == 3)  // MODIFIED BY MSJ ON 23 JAN 2018 END
                            {

                                var CurrentBal = WetosDB.LeaveBalances.Where(a => a.EmployeeId == LeaveEncashObj.EmployeeId && a.LeaveType == LeaveEncashObj.LeaveCode).Single();
                                var UpdatedBalance = CurrentBal.CurrentBalance + LeaveEncashObj.EncashValue;
                                var LeaveUsedObj = (CurrentBal.LeaveUsed == null ? 0 : CurrentBal.LeaveUsed) - LeaveEncashObj.EncashValue;
                                LeaveBalance LeaveBalanceObj = WetosDB.LeaveBalances.Where(a => a.EmployeeId == LeaveEncashObj.EmployeeId && a.LeaveType == LeaveEncashObj.LeaveCode).Single();
                                LeaveBalanceObj.CurrentBalance = UpdatedBalance;
                                LeaveBalanceObj.LeaveUsed = LeaveUsedObj;
                            }

                            // MODIFIED BY MSJ ON 23 JAN 2018 START
                            LeaveEncashObj.StatusId = item.StatusId;
                            //LeaveEncashObj.Status = item.Status;
                            // MODIFIED BY MSJ ON 23 JAN 2018 END

                            //Added By Shraddha on 14 DEC 2016 for add No. Of Leave Days in LeaveBalance Table after rejecting the sanctioned leave end
                            WetosDB.SaveChanges();


                            // NOTIFICATION ADDED BY RAJAS ON 31 DEC 2016
                            #region LEAVE ENCASH SANCTION NOTIFICATION

                            // FOR NOTIFICATION TO BE SENT TO REPORTING PERSON

                            Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == LeaveEncashObj.EmployeeId).FirstOrDefault();
                            Notification NotificationObj = new Notification();
                            NotificationObj.FromID = EmployeeObj.EmployeeReportingId; ;
                            NotificationObj.ToID = EmployeeObj.EmployeeId;
                            NotificationObj.SendDate = DateTime.Now;
                            int StatusId = Convert.ToInt32(LeaveEncashObj.Status);
                            string StatusName = WetosDB.DropdownDatas.Where(a => a.TypeId == 3 && a.Value == StatusId).Select(a => a.Text).FirstOrDefault();
                            NotificationObj.NotificationContent = "Encashment applied by " + EmployeeObj.FirstName + " " + EmployeeObj.LastName + " is " + StatusName;
                            NotificationObj.ReadFlag = false;
                            NotificationObj.SendDate = DateTime.Now;

                            WetosDB.Notifications.AddObject(NotificationObj);

                            WetosDB.SaveChanges();

                            // Check if both reporting person are are different
                            if (EmployeeObj.EmployeeReportingId != EmployeeObj.EmployeeReportingId2)
                            {
                                // FOR NOTIFICATION TO BE SENT TO REPORTING PERSON AS SANCTIONER

                                //Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == LeaveApplicationObj.EmployeeId).FirstOrDefault();
                                Notification NotificationObj3 = new Notification();
                                NotificationObj3.FromID = EmployeeObj.EmployeeReportingId2; ;
                                NotificationObj3.ToID = EmployeeObj.EmployeeId;
                                NotificationObj3.SendDate = DateTime.Now;
                                NotificationObj3.NotificationContent = "Encashment applied by " + EmployeeObj.FirstName + " " + EmployeeObj.LastName + " is " + StatusName;
                                NotificationObj3.ReadFlag = false;
                                NotificationObj3.SendDate = DateTime.Now;

                                WetosDB.Notifications.AddObject(NotificationObj3);

                                WetosDB.SaveChanges();
                            }

                            //FOR SELF NOTIFICATION

                            Notification NotificationObj2 = new Notification();
                            NotificationObj2.FromID = EmployeeObj.EmployeeReportingId;
                            NotificationObj2.ToID = EmployeeObj.EmployeeReportingId;
                            NotificationObj2.SendDate = DateTime.Now;
                            int StatusId1 = Convert.ToInt32(LeaveEncashObj.Status);
                            string StatusName1 = WetosDB.DropdownDatas.Where(a => a.TypeId == 3 && a.Value == StatusId).Select(a => a.Text).FirstOrDefault();
                            NotificationObj2.NotificationContent = "Encashment applied by " + EmployeeObj.FirstName + " " + EmployeeObj.LastName + " is " + StatusName1;
                            NotificationObj2.ReadFlag = false;
                            NotificationObj2.SendDate = DateTime.Now;

                            WetosDB.Notifications.AddObject(NotificationObj2);

                            WetosDB.SaveChanges();

                            // NOTIFICATION COUNT
                            int NoTiCount = WetosDB.Notifications.Where(a => a.ToID == EmpId && (a.ReadFlag == false || a.ReadFlag == null)).Count();
                            Session["NotificationCount"] = NoTiCount;

                            #endregion

                        }


                    }

                    List<SP_LeaveEncashSanctionIndex_Result> LeaveEncashSanctionList = WetosDB.SP_LeaveEncashSanctionIndex(EmpNo).OrderByDescending(a => a.LeaveEncashId).ToList();


                    // ADDED BY PUSHKAR FOR AuditLog ON 21 DEC 2016
                    AddAuditTrail("Success - Encashment application sanctioned successfully");  // Updated by Rajas on 17 JAN 2017

                    // Added by Rajas on 17 JAN 2017 START
                    Success("Success - Encashmentapplication sanctioned successfully");

                    PopulateDropDown();
                    return View(LeaveEncashSanctionList);
                }
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in LeaveEnacashment due to " + ex.Message + ( ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                List<SP_LeaveEncashSanctionIndex_Result> LeaveEncashSanctionList = WetosDB.SP_LeaveEncashSanctionIndex(EmpNo).OrderByDescending(a => a.LeaveEncashId).ToList();
                //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 START
                PopulateDropDown();
                //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 END
                return View(LeaveEncashSanctionList);
            }
        }



        /// <summary>
        /// ADDED BY PUSHKAR ON 06 JUNE 2018
        /// </summary>
        /// <param name="selectCriteria"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LeaveEncashSanctionPV(int selectCriteria = 0)
        {
            try
            {
                int EmpNo = Convert.ToInt32(Session["EmployeeNo"]);
                int Id = Convert.ToInt32(Session["Id"]);

                // Added by Rajas on 9 MAY 2017 START
                // Select Criteria as per dropdown value 
                int Status = 0; //string.Empty;

                List<SP_LeaveEncashSanctionIndex_Result> LeaveEncashSanctionList = new List<SP_LeaveEncashSanctionIndex_Result>();

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
                    return View(LeaveEncashSanctionList);
                }

                DateTime CalanderStartDate = GetFYStartDate(); // DateTime CalanderStartDate = WetosDB.FinancialYears.Where(a => a.FinancialName == GlobalSettingObj.SettingValue).Select(a => a.StartDate).FirstOrDefault();

                if (CalanderStartDate != null)
                {
                    
                    if (selectCriteria == 1)  // Pending
                    {
                        Status = 1;

                        // Updated by Rajas on 7 JUNE 2017, ( || a.EmployeeReportingId2 == EmpNo) removed
                        LeaveEncashSanctionList = WetosDB.SP_LeaveEncashSanctionIndex(EmpNo).Where(a => ((a.EmployeeReportingId == EmpNo) && a.StatusId == Status
                            && a.EncashDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null))
                            .OrderByDescending(a => a.EncashDate).ToList();  // && a.Id == 4)
                    }
                    else if (selectCriteria == 2)  // Sanction
                    {
                        Status = 2;

                        LeaveEncashSanctionList = WetosDB.SP_LeaveEncashSanctionIndex(EmpNo)
                            .Where(a => ((a.EmployeeReportingId == EmpNo || a.EmployeeReportingId2 == EmpNo) && a.StatusId == Status && a.EncashDate >= CalanderStartDate
                                && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null))
                            .OrderByDescending(a => a.EncashDate).ToList();  // && a.Id == 4)
                    }
                    else if (selectCriteria == 3) // Rejected by Approver
                    {
                        Status = 3;

                        LeaveEncashSanctionList = WetosDB.SP_LeaveEncashSanctionIndex(EmpNo)
                            .Where(a => ((a.EmployeeReportingId == EmpNo || a.EmployeeReportingId2 == EmpNo)
                                && a.StatusId == Status && a.EncashDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null) || (a.EmployeeReportingId2 == EmpNo
                                && a.StatusId == Status && a.EncashDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null))
                                .OrderByDescending(a => a.EncashDate).ToList();  // && a.Id == 4)
                    }
                    else if (selectCriteria == 4)  // Approve
                    {
                        Status = 4;

                        LeaveEncashSanctionList = WetosDB.SP_LeaveEncashSanctionIndex(EmpNo)
                            .Where(a => ((a.EmployeeReportingId2 == EmpNo || a.EmployeeReportingId == EmpNo)
                                && a.StatusId == Status && a.EncashDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null))
                                .OrderByDescending(a => a.EncashDate).ToList();  // && a.Id == 4)
                    }
                    else if (selectCriteria == 5)  // Cancel
                    {
                        Status = 5;

                        LeaveEncashSanctionList = WetosDB.SP_LeaveEncashSanctionIndex(EmpNo)
                            .Where(a => ((a.EmployeeReportingId == EmpNo || a.EmployeeReportingId2 == EmpNo) && a.StatusId == Status
                                && a.EncashDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null))
                                .OrderByDescending(a => a.EncashDate).ToList();  // && a.Id == 4)
                    }
                    else  // Rejected by Sanctioner
                    {
                        Status = 6;
                        LeaveEncashSanctionList = WetosDB.SP_LeaveEncashSanctionIndex(EmpNo)
                            .Where(a => ((a.EmployeeReportingId2 == EmpNo) && a.StatusId == Status
                                && a.EncashDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null))
                            .OrderByDescending(a => a.EncashDate).ToList();  // && a.Id == 4)
                    }
                    // Code updated by Rajas on 22 SEP 2017 END
                }
                #endregion

                ViewBag.SelectedStatus = selectCriteria;
                // Added by Rajas on 9 MAY 2017 END
                ViewBag.Status = Status;

                #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //var EmployeeData = WetosDB.VwActiveEmployees.ToList();
                List<SP_VwActiveEmployee_Result> EmployeeData = WetosDB.SP_VwActiveEmployee(EmployeeId).ToList();
                #endregion
                // List<SP_WetosGetEmployeeLeaveSanction_Result> LeaveSanctionObj = WetosDB.SP_WetosGetEmployeeLeaveSanction(EmpNo).OrderByDescending(a => a.FromDate).OrderByDescending(a => a.ToDate).ToList();

                // Added by Rajas on 9 MAY 2017
                // Populate Status dropdown
                PopulateDropDown();

                return PartialView(LeaveEncashSanctionList);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in LeaveEncash Sanction due to " + ex.Message + ( ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                Error("Error in LeaveEncash Sanction due to " + ex.Message + ( ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                return RedirectToAction("EmployeeDashboard", "Dashboard");
            }
        }

        /// <summary>
        /// SEND MAIL
        /// Added by Rajas on 31 MARCH 2017
        /// </summary>
        /// <param name="ToEmail"></param>
        /// <param name="Subject"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        /// Updated by Rajas on 19 JULY 2017 for EmailFromWhichApplication
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

                msg.Subject = Subject; // "Contact Us";

                msg.IsBodyHtml = false;

                msg.From = new MailAddress(SMTPUsername); //"info@foodpatronservices.com");

                smtp.Host = SMTPServerName; // "smtpout.asia.secureserver.net";

                smtp.EnableSsl = true;

                //smtp.Credentials = new System.Net.NetworkCredential("info@foodpatronservices.com", "info@fps");
                smtp.Credentials = new System.Net.NetworkCredential(SMTPUsername, SMTPPassword);//"info@foodpatronservices.com", "info@fps");

                smtp.Port = SMTPPortInt; // 25;

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
                AddAuditTrail("Error in sending email in " + EmailFromWhichApplication + " : " + " due to " + ex.Message + ( ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                UpdateStatus = "Unable to send email!";

                return ReturnStatus;

                //return View("Error");
            }

            // SEND EMAIL END

            //return View();
        }


        /// <summary>
        /// Process attendance function
        /// Added by Rajas on 18 MAY 2017
        /// </summary>
        /// <param name="fromdate"></param>
        /// <param name="todate"></param>
        /// <returns></returns>
        //public bool ProcessAttendance(DateTime fromdate, DateTime todate)
        //{
        //    bool ReturnStatus = false;

        //    try
        //    {
        //        // CALL STATIC FUNCTION FROM ADMINISTRATOR CONTRLLER -- NEED TO SEPERATE ALL FUNCTION IN STATIC FUNCTION
        //        WetosAdministrationController.ProcessAttendenceEx(WetosDB, fromdate, todate);

        //        AddAuditTrail("Processimg attendance data after sanction transaction");

        //        return ReturnStatus = true;

        //    }
        //    catch (System.Exception ex)
        //    {
        //        //ViewBag.Message = "Error in Processing Attendence";
        //        //Session["SuccessMessage"] = ViewBag.Message;
        //        AddAuditTrail("Error in Processing Attendence due to " + ex.Message + ( ex.InnerException == null ? string.Empty : ex.InnerException.Message));

        //        return ReturnStatus;

        //    }
        //}

        /// <summary>
        /// Added by Rajas on 19 MAY 2017
        /// </summary>
        public void PopulateDropDown()
        {
            // Added by Rajas on 9 MAY 2017
            // Populate Status dropdown
            var StatusObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 3).Select(a => new { Value = a.Value, Text = a.Text }).ToList();
            ViewBag.StatusList = new SelectList(StatusObj, "Value", "Text").ToList();

            //List<string> StatusObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 3).Select(a => a.Text).ToList();
            //ViewBag.StatusList = StatusObj;
        }

        /// <summary>
        /// ADDED BY SHRADDHA ON 11 SEP 2017
        /// TO CHECK WHETHER LOGIN EMPLOYEE IS APPROVER OR NOT FOR SELECTED EMPLOYEE
        /// <param name="EmployeeId"></param>
        /// <param name="LoginEmployee"></param>
        /// <returns></returns>
        public bool LoginPersonIsApprover(int EmployeeId, int LoginEmployee)
        {
            bool Istrue = false;
            Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == EmployeeId && a.EmployeeReportingId == LoginEmployee).FirstOrDefault();
            if (EmployeeObj != null)
            {
                Istrue = true;
            }
            return Istrue;
        }


        /// <summary>
        /// ADDED BY SHRADDHA ON 11 SEP 2017
        /// TO CHECK WHETHER LOGIN EMPLOYEE IS SANCTIONER OR NOT FOR SELECTED EMPLOYEE
        /// <param name="EmployeeId"></param>
        /// <param name="LoginEmployee"></param>
        /// <returns></returns>
        public bool LoginPersonIsSanctioner(int EmployeeId, int LoginEmployee)
        {
            bool Istrue = false;
            Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == EmployeeId && a.EmployeeReportingId2 == LoginEmployee).FirstOrDefault();
            if (EmployeeObj != null)
            {
                Istrue = true;
            }
            return Istrue;
        }

        /// <summary>
        /// ADDED BY SHRADDHA ON 11 SEP 2017
        /// TO CHECK WHETHER LOGIN EMPLOYEE IS APPROVER AND SANCTIONER OR NOT FOR SELECTED EMPLOYEE
        /// <param name="EmployeeId"></param>
        /// <param name="LoginEmployee"></param>
        /// <returns></returns>
        public bool LoginPersonIsApproverAndSanctioner(int EmployeeId, int LoginEmployee)
        {
            bool Istrue = false;
            Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == EmployeeId && a.EmployeeReportingId2 == LoginEmployee && a.EmployeeReportingId == LoginEmployee).FirstOrDefault();
            if (EmployeeObj != null)
            {
                Istrue = true;
            }
            return Istrue;
        }

        /// <summary>
        /// ADDED BY SHRADDHA ON 11 SEP 2017
        /// TO CHECK WHETHER LOGIN EMPLOYEE IS HR OR NOT FOR SELECTED EMPLOYEE
        /// <param name="EmployeeId"></param>
        /// <param name="LoginEmployee"></param>
        /// <returns></returns>
        public bool LoginPersonIsHR(int EmployeeId, int LoginEmployee)
        {
            bool Istrue = false;
            //Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == EmployeeId && a.EmployeeReportingId == LoginEmployee).FirstOrDefault();
            //if (EmployeeObj != null)
            //{
            //    Istrue = true;
            //}
            return Istrue;
        }


        //#######################################################

        public ActionResult ODLateEarlySanctionIndex(int selectCriteria = 0)
        {
            try
            {
                int EmpNo = Convert.ToInt32(Session["EmployeeNo"]);
                int Id = Convert.ToInt32(Session["Id"]);

                // Added by Rajas on 9 MAY 2017 START
                // Select Criteria as per dropdown value
                int Status = 0;

                // Updated by Rajas on 20 MAY 2017 START
                List<SP_WetosGetEmployeeODLateEarlySanction_Result> ODTOURObj = new List<SP_WetosGetEmployeeODLateEarlySanction_Result>();


                // Get current FY from global setting
                //GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == "Current Financial Year").FirstOrDefault();

                //ABOVE LINE COMMENTED AND BELOW LINE IS ADDED BY SHALAKA ON 13TH DEC 2017
                GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.CurrentFinancialYear).FirstOrDefault();

                // Error
                if (GlobalSettingObj == null)
                {
                    Error("Inconsistent Financial year data");
                    PopulateDropDown();
                    return View(ODTOURObj);
                }

                DateTime CalanderStartDate = GetFYStartDate(); // DateTime CalanderStartDate = WetosDB.FinancialYears.Where(a => a.FinancialName == GlobalSettingObj.SettingValue).Select(a => a.StartDate).FirstOrDefault();

                if (CalanderStartDate != null)
                {
                    //CODE REPLACED BY SHRADDHA ON 12 SEP 2017 REMOVED REDUNDANT AND UNNECESSARY IF ELSE CODE AND TAKEN ONE SINGLE LINE CODE START
                    Status = selectCriteria;
                    ODTOURObj = WetosDB.SP_WetosGetEmployeeODLateEarlySanction(EmpNo).Where(a => ((a.EmployeeReportingId == EmpNo || a.EmployeeReportingId2 == EmpNo) && a.Id == selectCriteria && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null))
                    .OrderByDescending(a => a.FromDate).OrderByDescending(a => a.ToDate).ToList();
                    //CODE REPLACED BY SHRADDHA ON 12 SEP 2017 REMOVED REDUNDANT AND UNNECESSARY IF ELSE CODE AND TAKEN ONE SINGLE LINE CODE END
                }


                // Updated by Rajas on 20 MAY 2017 END
                ViewBag.Status = Status;
                ViewBag.SelectedStatus = selectCriteria;
                // Added by Rajas on 9 MAY 2017 END



                #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //var EmployeeData = WetosDB.VwActiveEmployees.ToList();
                List<SP_VwActiveEmployee_Result> EmployeeData = WetosDB.SP_VwActiveEmployee(EmployeeId).ToList();
                #endregion

                // List<SP_WetosGetEmployeeLeaveSanction_Result> LeaveSanctionObj = WetosDB.SP_WetosGetEmployeeLeaveSanction(EmpNo).OrderByDescending(a => a.FromDate).OrderByDescending(a => a.ToDate).ToList();

                // Added by Rajas on 9 MAY 2017
                // Populate Status dropdown

                //REPLACED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 START
                //List<string> StatusObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 3).Select(a => a.Text).ToList();
                //ViewBag.StatusList = StatusObj;
                PopulateDropDown();
                //REPLACED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 END


                return View(ODTOURObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in Late/Early due to " + ex.Message + ( ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                Error("Error in Late/Early due to " + ex.Message + ( ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 START
                PopulateDropDown();
                //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 END

                return RedirectToAction("EmployeeDashboard", "Dashboard");
            }
        }

        /// <summary>
        /// Added by Rajas on 18 MAY 2017
        /// </summary>
        /// <param name="selectCriteria"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ODLateEarlySanctionPV(int selectCriteria)
        {
            try
            {
                int EmpNo = Convert.ToInt32(Session["EmployeeNo"]);
                int Id = Convert.ToInt32(Session["Id"]);

                // Added by Rajas on 9 MAY 2017 START
                // Select Criteria as per dropdown value
                int Status = 0;

                // Updated by Rajas on 20 MAY 2017 START
                List<SP_WetosGetEmployeeODLateEarlySanction_Result> ODTOURObj = new List<SP_WetosGetEmployeeODLateEarlySanction_Result>();

                #region UPDATED BY RAJAS ON 23 MAY 2017
                // Get current FY from global setting

                //GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == "Current Financial Year").FirstOrDefault();

                //ABOVE LINE COMMENTED AND BELOW LINE IS ADDED BY SHALAKA ON 13TH DEC 2017
                GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.CurrentFinancialYear).FirstOrDefault();

                // Error
                if (GlobalSettingObj == null)
                {
                    Error("Inconsistent Financial year data");
                    PopulateDropDown();
                    return View(ODTOURObj);
                }

                DateTime CalanderStartDate = GetFYStartDate(); // DateTime CalanderStartDate = WetosDB.FinancialYears.Where(a => a.FinancialName == GlobalSettingObj.SettingValue).Select(a => a.StartDate).FirstOrDefault();

                if (CalanderStartDate != null)
                {

                    //CODE REPLACED BY SHRADDHA ON 12 SEP 2017 REMOVED REDUNDANT AND UNNECESSARY IF ELSE CODE AND TAKEN ONE SINGLE LINE CODE START
                    //Status = selectCriteria;
                    //ODTOURObj = WetosDB.SP_WetosGetEmployeeODTravelSanction(EmpNo).Where(a => ((a.EmployeeReportingId == EmpNo || a.EmployeeReportingId2 == EmpNo) && a.Id == selectCriteria && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null))
                    //.OrderByDescending(a => a.FromDate).OrderByDescending(a => a.ToDate).ToList();
                    //CODE REPLACED BY SHRADDHA ON 12 SEP 2017 REMOVED REDUNDANT AND UNNECESSARY IF ELSE CODE AND TAKEN ONE SINGLE LINE CODE END

                    // Above code commented. Error was list for reportingId2 was not displayed
                    // Code updated by Rajas on 22 SEP 2017 START
                    if (selectCriteria == 1)  // Pending
                    {
                        Status = 1;

                        // Updated by Rajas on 7 JUNE 2017, ( || a.EmployeeReportingId2 == EmpNo) removed
                        ODTOURObj = WetosDB.SP_WetosGetEmployeeODLateEarlySanction(EmpNo).Where(a => ((a.EmployeeReportingId == EmpNo) && a.Id == Status
                            && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null))
                            .OrderByDescending(a => a.FromDate).ToList();  // && a.Id == 4)
                    }
                    else if (selectCriteria == 2)  // Sanction
                    {
                        Status = 2;

                        ODTOURObj = WetosDB.SP_WetosGetEmployeeODLateEarlySanction(EmpNo)
                            .Where(a => a.EmployeeReportingId2 == EmpNo && a.Id == Status && a.FromDate >= CalanderStartDate
                                && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null)
                            .OrderByDescending(a => a.FromDate).ToList();  // && a.Id == 4)
                    }
                    else if (selectCriteria == 3) // Rejected by Approver
                    {
                        Status = 3;

                        ODTOURObj = WetosDB.SP_WetosGetEmployeeODLateEarlySanction(EmpNo)
                            .Where(a => ((a.EmployeeReportingId == EmpNo || a.EmployeeReportingId2 == EmpNo)
                                && a.Id == Status && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null) || (a.EmployeeReportingId2 == EmpNo
                                && a.Id == Status && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null))
                                .OrderByDescending(a => a.FromDate).ToList();  // && a.Id == 4)
                    }
                    else if (selectCriteria == 4)  // Approve
                    {
                        Status = 4;

                        ODTOURObj = WetosDB.SP_WetosGetEmployeeODLateEarlySanction(EmpNo)
                            .Where(a => ((a.EmployeeReportingId2 == EmpNo || a.EmployeeReportingId == EmpNo)
                                && a.Id == Status && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null))
                                .OrderByDescending(a => a.FromDate).ToList();  // && a.Id == 4)
                    }
                    else if (selectCriteria == 5)  // Cancel
                    {
                        Status = 5;

                        ODTOURObj = WetosDB.SP_WetosGetEmployeeODLateEarlySanction(EmpNo)
                            .Where(a => ((a.EmployeeReportingId == EmpNo || a.EmployeeReportingId2 == EmpNo) && a.Id == Status
                                && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null))
                                .OrderByDescending(a => a.FromDate).ToList();  // && a.Id == 4)
                    }
                    else  // Rejected by Sanctioner
                    {
                        Status = 6;
                        ODTOURObj = WetosDB.SP_WetosGetEmployeeODLateEarlySanction(EmpNo)
                            .Where(a => ((a.EmployeeReportingId2 == EmpNo) && a.Id == Status
                                && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null))
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
                List<SP_VwActiveEmployee_Result> EmployeeData = WetosDB.SP_VwActiveEmployee(EmployeeId).ToList();
                #endregion

                // List<SP_WetosGetEmployeeLeaveSanction_Result> LeaveSanctionObj = WetosDB.SP_WetosGetEmployeeLeaveSanction(EmpNo).OrderByDescending(a => a.FromDate).OrderByDescending(a => a.ToDate).ToList();

                // Added by Rajas on 9 MAY 2017
                // Populate Status dropdown
                //REPLACED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 START
                //List<string> StatusObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 3).Select(a => a.Text).ToList();
                //ViewBag.StatusList = StatusObj;
                PopulateDropDown();
                //REPLACED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 END

                return PartialView(ODTOURObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in Late/Early Sanction due to " + ex.Message + ( ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                Error("Error in Late/Early Sanction due to " + ex.Message + ( ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 START
                PopulateDropDown();
                //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 END

                return RedirectToAction("EmployeeDashboard", "Dashboard");
            }
        }

        /// <summary>
        /// Added By Shraddha on 13 DEC 2016 For OD Travel sanction Index POST used for multiple sanction, reject at selection of radio button
        /// </summary>
        /// <param name="LeaveSanction"></param>
        /// <param name="FC"></param>
        /// <returns></returns>
        /// <summary>
        /// Added By Shraddha on 13 DEC 2016 For OD Travel sanction Index POST used for multiple sanction, reject at selection of radio button
        /// </summary>
        /// <param name="LeaveSanction"></param>
        /// <param name="FC"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ODLateEarlySanctionIndex(List<SP_WetosGetEmployeeODLateEarlySanction_Result> ODTravelSanction, FormCollection FC)
        {
            try
            {

                #region FAKE LOGIC

                ODTravelSanction = null; // new List<SP_LeaveSanctionIndex_Result>();

                // FAKE LOGIC MSJ
                int EmpNo = Convert.ToInt32(Session["EmployeeNo"]);
                int Id = Convert.ToInt32(Session["Id"]);

                //List<SP_WetosGetEmployeeLeaveSanction_Result> LeaveSanctionObj1 = WetosDB.SP_WetosGetEmployeeLeaveSanction(EmpNo).ToList();

                DateTime abc = Convert.ToDateTime("2016/01/01");

                #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //var EmployeeData = WetosDB.Employees.ToList();
                List<SP_VwActiveEmployee_Result> EmployeeData = WetosDB.SP_VwActiveEmployee(EmployeeId).ToList();
                #endregion

                List<SP_WetosGetEmployeeODLateEarlySanction_Result> ODTravelSanctionObjForCount = new List<SP_WetosGetEmployeeODLateEarlySanction_Result>();

                string RejectReasonText = FC["RejectReasonText"];

                //List<SP_LeaveSanctionIndex_Result> LeaveSanctionObj = new List<SP_LeaveSanctionIndex_Result>();
                if (ODTravelSanction == null)
                {
                    ODTravelSanction = new List<SP_WetosGetEmployeeODLateEarlySanction_Result>();

                    int SingleApplicationID = 0;
                    int SingleStatusID = 0;

                    // Added by Rajas on 19 MAY 2017
                    bool StatusIdSelected = false;

                    foreach (var key in FC.AllKeys)
                    {
                        string KeyStr = key.ToString();

                        if (KeyStr.Contains("StatusId") == true)
                        {
                            SingleStatusID = Convert.ToInt32(FC[KeyStr]);

                            string TempKeyStr = KeyStr.Replace("StatusId", "ODTourId");

                            SingleApplicationID = Convert.ToInt32(FC[TempKeyStr]);
                            SP_WetosGetEmployeeODLateEarlySanction_Result TempODTravelSanction = WetosDB.SP_WetosGetEmployeeODLateEarlySanction(EmpNo).Where(a => a.ODTourId == SingleApplicationID).FirstOrDefault(); //.ToList();

                            if (TempODTravelSanction != null)
                            {
                                TempODTravelSanction.StatusId = SingleStatusID;// UPDATE STATUS
                                ODTravelSanction.Add(TempODTravelSanction);
                            }

                            StatusIdSelected = true;

                            //"[6].LeaveApplicationId"
                        }

                    }

                    // Added by Rajas on 19 MAY 2017
                    if (StatusIdSelected == false)
                    {
                        Information("Please select atleast one entry for processing");

                        PopulateDropDown();

                        return View(ODTravelSanction);
                    }

                }

                #endregion

                ODTravelSanctionObjForCount = ODTravelSanction.Where(a => a.StatusId == 3 || a.StatusId == 6 || a.StatusId == 5).ToList();
                // Below code modified by Rajas on 19 MAY 2017 START
                if (ODTravelSanctionObjForCount.Count() > 0 && (RejectReasonText == null || RejectReasonText == ""))
                {
                    ModelState.AddModelError("CustomError", "Please Enter Reject/Cancel Reason");

                    //List<SP_WetosGetEmployeeODTravelSanction_Result> ODTravelSanctionList = WetosDB.SP_WetosGetEmployeeODTravelSanction(EmpNo).OrderByDescending(a => a.FromDate).OrderByDescending(a => a.ToDate).ToList();

                    // Added by Rajas on 9 MAY 2017
                    // Populate Status dropdown
                    PopulateDropDown();

                    Error("Please Enter Reject/Cancel Reason");

                    return View(ODTravelSanctionObjForCount); // ODTravelSanctionList
                }
                // Below code modified by Rajas on 19 MAY 2017 END
                else
                {
                    foreach (var i in ODTravelSanctionObjForCount)
                    {
                        ODLateEarly ODObj = WetosDB.ODLateEarlies.Where(a => a.ODTourId == i.ODTourId).FirstOrDefault();

                        ODObj.RejectReason = RejectReasonText;

                        WetosDB.SaveChanges();

                    }
                    foreach (SP_WetosGetEmployeeODLateEarlySanction_Result item in ODTravelSanction)
                    {
                        if (item.StatusId == 5)   // Added by Rajas on 9 JULY 2017 to Fix Issue no. 2, defect id= FB0012 as per Test Cases sheet
                        {
                            // Cancellation reason code
                            // Added by Rajas on 12 AUGUST 2017 START
                            if (RejectReasonText == null || RejectReasonText == string.Empty)
                            {
                                ModelState.AddModelError("CustomError", "Please Enter Cancellation Reason");

                                //List<SP_WetosGetEmployeeODTravelSanction_Result> ODTravelSanctionList = WetosDB.SP_WetosGetEmployeeODTravelSanction(EmpNo).OrderByDescending(a => a.FromDate).OrderByDescending(a => a.ToDate).ToList();

                                // Added by Rajas on 9 MAY 2017
                                // Populate Status dropdown
                                PopulateDropDown();

                                Error("Please enter  Cancellation Reason");

                                return View(ODTravelSanction); // ODTravelSanctionList
                            }
                        }
                        int EmpId = Convert.ToInt32(Session["EmployeeNo"]);

                        ODLateEarly ObjODT = WetosDB.ODLateEarlies.Where(a => a.ODTourId == item.ODTourId).FirstOrDefault();

                        // Validate Valid employee from obj
                        VwActiveEmployee ValidEmpObj = WetosDB.VwActiveEmployees.Where(a => a.EmployeeId == ObjODT.EmployeeId).FirstOrDefault();

                        if (ValidEmpObj != null)
                        {

                            if (item.StatusId != 0)
                            {
                                ODLateEarly ODTravelObj = WetosDB.ODLateEarlies.Where(a => a.ODTourId == item.ODTourId).FirstOrDefault();

                                //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 12 SEPTEMBER 2017 FOR EDIT
                                ODLateEarly ODTravelObjEDIT = WetosDB.ODLateEarlies.Where(a => a.ODTourId == item.ODTourId).FirstOrDefault();

                                ODTravelObj.StatusId = item.StatusId;
                                ODTravelObj.Status = WetosDB.DropdownDatas.Where(a => a.TypeId == 3 && a.Value == ODTravelObj.StatusId).Select(a => a.Text).FirstOrDefault();

                                for (DateTime CurrentODDate = Convert.ToDateTime(ODTravelObj.FromDate); CurrentODDate.Date <= ODTravelObj.ToDate; CurrentODDate = CurrentODDate.AddDays(1))
                                {

                                    //CODE MODIFIED BY SHRADDHA ON 13 FEB 2017 CONDITION ADDED FOR TODATE START
                                    DailyTransaction DailyTransactionObj = WetosDB.DailyTransactions.Where(a => a.EmployeeId == ODTravelObj.EmployeeId
                                        && a.TranDate == CurrentODDate).FirstOrDefault();
                                    //CODE MODIFIED BY SHRADDHA ON 13 FEB 2017 CONDITION ADDED FOR TODATE END

                                    #region SAVE ORIGINAL DailyTransaction STATUS

                                    // Save Actual DailyTransaction Status
                                    // Added by Rajas on 5 AUGUST 2017

                                    //if (item.StatusId != 2)
                                    //{
                                    //    if (DailyTransactionObj != null)
                                    //    {
                                    //        DailyTransactionObj.Status = DailyTransactionObj.ActualStatus;
                                    //        WetosDB.SaveChanges();
                                    //    }
                                    //}

                                    #endregion

                                    if ((item.StatusId == 2) || (item.StatusId == 4 && LoginPersonIsApproverAndSanctioner(ValidEmpObj.EmployeeId, EmpId)))//(ValidEmpObj.EmployeeReportingId == EmpId && ValidEmpObj.EmployeeReportingId2 == EmpId)))//UPDATED BY SHRADDHA ON 11 SEP 2017 TAKEN LoginPersonIsApproverAndSanctioner FUNCTION
                                    {
                                        if (LoginPersonIsApprover(ValidEmpObj.EmployeeId, EmpId) || LoginPersonIsSanctioner(ValidEmpObj.EmployeeId, EmpId))//ValidEmpObj.EmployeeReportingId2 == EmpId || ValidEmpObj.EmployeeReportingId == EmpId)//UPDATED BY SHRADDHA ON 11 SEP 2017 TAKEN LoginPersonIsApprover() AND LoginPersonIsSanctioner() FUNCTION
                                        {
                                            item.StatusId = 2;

                                            ODTravelObj.RejectReason = string.Empty;

                                            // Added By RAJAS on 10 JAN 2017 to Handle null object
                                            if (DailyTransactionObj != null)
                                            {
                                                PostingFlagModelObj.IsOdTour = true;

                                                // Status as per selection of ODTourType
                                                // Added by Rajas on 9 JULY 2017 to Fix issue no. 4, defect id=	FB0014 as per Test Cases sheet
                                                // Updated by Rajas on 5 AUGUST 2017
                                                string NewStatus = string.Empty;
                                                string UpdatedStatus = string.Empty;
                                                string ReturnMessage = string.Empty;

                                                if (ODTravelObj.ODTourType.ToUpper().Trim() == "TOUR")
                                                {
                                                    NewStatus = "TO";
                                                }
                                                else if (ODTravelObj.ODTourType.ToUpper().Trim() == "HALF/FULL DAY OD")
                                                {
                                                    NewStatus = "OD";
                                                }
                                                else
                                                {
                                                    NewStatus = "PP";
                                                }

                                                // Following code updated by Rajas on 9 JULY 2017 for Status defined above START
                                                //DailyTransactionObj.Status = "ODOD";
                                                //ADDED CODE FOR ADDING HALF DAY AND FULL DAY OD WISE STATUS IN DAILY TRANSACTION TABLE BY SHRADDHA ON 13 FEB 2017 START

                                                #region CODE UPDATE : Mark Attendance Status changes
                                                #region COMMENTED CODE BY SHRADDHA AND CALLED GENERIC FUNCTION ON 22 NOV 2017 NEED TO TEST
                                                // Below code updated by Rajas on 7 AUGUST 2017 START
                                                //if (DailyTransactionObj != null)
                                                //{
                                                //    if (DailyTransactionObj.Status.Trim() == "PPPP")  // need to verify
                                                //    {
                                                //        DailyTransactionObj.Status = "PPPP";
                                                //    }
                                                //    else if (CurrentODDate == ODTravelObj.FromDate && ODTravelObj.StatusId == 2)
                                                //    {
                                                //        if (ODTravelObj.ODDayStatus == 2)
                                                //        {
                                                //            PostingFlagModelObj.IsFirstHalfOD = true;

                                                //            //Added by Shalaka with help of Sharddha on 28th OCT 2017 For WorkingHours time Update 
                                                //            //Issue raised by Ulka on 27th OCT
                                                //            //Get Object of Shift
                                                //            Shift ShiftObj = WetosDB.Shifts.Where(a => a.BranchId == ODTravelObj.BranchId && a.Company.CompanyId == ODTravelObj.CompanyId).FirstOrDefault();

                                                //            if (ShiftObj != null)
                                                //            {
                                                //                int workingHours = ShiftObj.WorkingHours.Hour;
                                                //                int workingMinute = ShiftObj.WorkingHours.Minute;
                                                //                int workingSeconds = ShiftObj.WorkingHours.Second;

                                                //                //If First Half then
                                                //                int HrsInMinutes = workingHours * 60;
                                                //                int TotalMinutes = HrsInMinutes + workingMinute;
                                                //                int halfofTime = TotalMinutes / 2;

                                                //                #region WORKING HOURS CALCULATION CORRECTION CODE IN CASE OF HALF DAY ADDED BY SHRADDHA ON 30 OCT 2017
                                                //                //CODE ADDED BY SHRADDHA ON 30 OCT 2017 TO GET ALREADY HAVING WORKING HOURS + HALF DAY WORKING HOURS AS TOTAL WORKING HOURS START

                                                //                int DTworkingHours = 0;
                                                //                int DTworkingMinute = 0;
                                                //                int DTworkingSeconds = 0;
                                                //                if (DailyTransactionObj.WorkingHrs != null)
                                                //                {
                                                //                    DTworkingHours = DailyTransactionObj.WorkingHrs.Value.Hour;
                                                //                    DTworkingMinute = DailyTransactionObj.WorkingHrs.Value.Minute;
                                                //                    DTworkingSeconds = DailyTransactionObj.WorkingHrs.Value.Second;
                                                //                }
                                                //                //If First Half then
                                                //                int DTHrsInMinutes = DTworkingHours * 60;
                                                //                int DTTotalMinutes = DTHrsInMinutes + DTworkingMinute;

                                                //                int TotalWorkingHours = halfofTime + DTTotalMinutes;

                                                //                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 OCT 2017 START
                                                //                //Get Working Hrs for FirstHalf
                                                //                //int FirstHalfWorkingHrs = halfofTime / 60;
                                                //                int FirstHalfWorkingHrs = TotalWorkingHours / 60;
                                                //                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 OCT 2017 END
                                                //                //CODE ADDED BY SHRADDHA ON 30 OCT 2017 TO GET ALREADY HAVING WORKING HOURS + HALF DAY WORKING HOURS AS TOTAL WORKING HOURS END
                                                //                #endregion


                                                //                //To get Working minutes from FirstHalfWorkingHrs 
                                                //                int t1 = FirstHalfWorkingHrs * 60;
                                                //                int FirstHalfWorkingMinutes = TotalWorkingHours - t1;

                                                //                //Added workingHrs
                                                //                DailyTransactionObj.WorkingHrs = new DateTime(DailyTransactionObj.TranDate.Year, DailyTransactionObj.TranDate.Month, DailyTransactionObj.TranDate.Day, FirstHalfWorkingHrs, FirstHalfWorkingMinutes, workingSeconds);
                                                //            }
                                                //        }
                                                //        else if (ODTravelObj.ODDayStatus == 3)
                                                //        {
                                                //            PostingFlagModelObj.IsSecondHalfOD = true;

                                                //            //Added by Shalaka with help of Sharddha on 28th OCT 2017 For WorkingHours time Update
                                                //            //Get Object of Shift
                                                //            Shift ShiftObj = WetosDB.Shifts.Where(a => a.BranchId == ODTravelObj.BranchId && a.Company.CompanyId == ODTravelObj.CompanyId).FirstOrDefault();

                                                //            if (ShiftObj != null)
                                                //            {
                                                //                int workingHours = ShiftObj.WorkingHours.Hour;
                                                //                int workingMinute = ShiftObj.WorkingHours.Minute;
                                                //                int workingSeconds = ShiftObj.WorkingHours.Second;

                                                //                //If Second Half then
                                                //                int HrsInMinutes = workingHours * 60;
                                                //                int TotalMinutes = HrsInMinutes + workingMinute;
                                                //                int HalfofTime = TotalMinutes / 2;


                                                //                #region WORKING HOURS CALCULATION CORRECTION CODE IN CASE OF HALF DAY ADDED BY SHRADDHA ON 30 OCT 2017
                                                //                //CODE ADDED BY SHRADDHA ON 30 OCT 2017 TO GET ALREADY HAVING WORKING HOURS + HALF DAY WORKING HOURS AS TOTAL WORKING HOURS START
                                                //                int DTworkingHours = 0;
                                                //                int DTworkingMinute = 0;
                                                //                int DTworkingSeconds = 0;
                                                //                if (DailyTransactionObj.WorkingHrs != null)
                                                //                {
                                                //                    DTworkingHours = DailyTransactionObj.WorkingHrs.Value.Hour;
                                                //                    DTworkingMinute = DailyTransactionObj.WorkingHrs.Value.Minute;
                                                //                    DTworkingSeconds = DailyTransactionObj.WorkingHrs.Value.Second;
                                                //                }

                                                //                //If First Half then
                                                //                int DTHrsInMinutes = DTworkingHours * 60;
                                                //                int DTTotalMinutes = DTHrsInMinutes + DTworkingMinute;

                                                //                int TotalWorkingHours = HalfofTime + DTTotalMinutes;

                                                //                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 OCT 2017 START
                                                //                //Get Working Hrs for SecondHalf
                                                //                //int SecondHalfWorkingHrs = halfofTime / 60;
                                                //                int SecondHalfWorkingHrs = TotalWorkingHours / 60;
                                                //                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 OCT 2017 END
                                                //                //CODE ADDED BY SHRADDHA ON 30 OCT 2017 TO GET ALREADY HAVING WORKING HOURS + HALF DAY WORKING HOURS AS TOTAL WORKING HOURS END
                                                //                #endregion
                                                //                //Working Hrs time for SecondHalf

                                                //                //To get Working minutes from FirstHalfWorkingHrs 
                                                //                int t1 = SecondHalfWorkingHrs * 60;
                                                //                int SecondHalfWorkingMinutes = TotalWorkingHours - t1;

                                                //                //Added workingHrs
                                                //                DailyTransactionObj.WorkingHrs = new DateTime(DailyTransactionObj.TranDate.Year, DailyTransactionObj.TranDate.Month, DailyTransactionObj.TranDate.Day, SecondHalfWorkingHrs, SecondHalfWorkingMinutes, workingSeconds);
                                                //            }
                                                //        }
                                                //        else
                                                //        {
                                                //            PostingFlagModelObj.IsFullDayOD = true;

                                                //            //Added by Shalaka with help of Sharddha on 28th OCT 2017 For WorkingHours time Update
                                                //            //Get Object of Shift
                                                //            Shift ShiftObj = WetosDB.Shifts.Where(a => a.BranchId == ODTravelObj.BranchId && a.Company.CompanyId == ODTravelObj.CompanyId).FirstOrDefault();

                                                //            if (ShiftObj != null)
                                                //            {
                                                //                int workingHours = ShiftObj.WorkingHours.Hour;
                                                //                int workingMinute = ShiftObj.WorkingHours.Minute;
                                                //                int workingSeconds = ShiftObj.WorkingHours.Second;

                                                //                //Added workingHrs for Full Day
                                                //                DailyTransactionObj.WorkingHrs = new DateTime(DailyTransactionObj.TranDate.Year, DailyTransactionObj.TranDate.Month, DailyTransactionObj.TranDate.Day, workingHours, workingMinute, workingSeconds);
                                                //            }
                                                //        }

                                                //        DailyTransactionObj.ActualStatus = DailyTransactionObj.Status; // CODE ADDED BY SHRADDHA ON 11 SEP 2017 START
                                                //        WetosAdministrationController.GetActualDailyTransactionStatus(PostingFlagModelObj, DailyTransactionObj.Status, NewStatus, ref UpdatedStatus, ref ReturnMessage);

                                                //        DailyTransactionObj.Status = UpdatedStatus;
                                                //    }

                                                //    else if (CurrentODDate == ODTravelObj.ToDate && ODTravelObj.StatusId == 2)
                                                //    {
                                                //        if (ODTravelObj.ODDayStatus1 == 2)
                                                //        {
                                                //            PostingFlagModelObj.IsFirstHalfOD = true;
                                                //        }
                                                //        else if (ODTravelObj.ODDayStatus1 == 3)
                                                //        {
                                                //            PostingFlagModelObj.IsSecondHalfOD = true;
                                                //        }
                                                //        else
                                                //        {
                                                //            PostingFlagModelObj.IsFullDayOD = true;
                                                //        }

                                                //        //Added by Shalaka with help of Sharddha on 28th OCT 2017 For WorkingHours time Update
                                                //        //Get Object of Shift
                                                //        Shift ShiftObj = WetosDB.Shifts.Where(a => a.BranchId == ODTravelObj.BranchId && a.Company.CompanyId == ODTravelObj.CompanyId).FirstOrDefault();

                                                //        WetosAdministrationController.GetActualDailyTransactionStatus(PostingFlagModelObj, DailyTransactionObj.Status, NewStatus, ref UpdatedStatus, ref ReturnMessage);
                                                //        DailyTransactionObj.ActualStatus = DailyTransactionObj.Status; // CODE ADDED BY SHRADDHA ON 11 SEP 2017 START
                                                //        DailyTransactionObj.Status = UpdatedStatus;

                                                //        //Added By shalaka on 28th OCT 2017 to update WorkingHrs time
                                                //        if (ShiftObj != null)
                                                //        {
                                                //            int workingHours = ShiftObj.WorkingHours.Hour;
                                                //            int workingMinute = ShiftObj.WorkingHours.Minute;
                                                //            int workingSeconds = ShiftObj.WorkingHours.Second;

                                                //            //Added workingHrs
                                                //            DailyTransactionObj.WorkingHrs = new DateTime(DailyTransactionObj.TranDate.Year, DailyTransactionObj.TranDate.Month, DailyTransactionObj.TranDate.Day, workingHours, workingMinute, workingSeconds);
                                                //        }
                                                //    }
                                                //    else if (DailyTransactionObj.Status.Trim() == WetosAdministrationController.StatusConstants.FullDayAbsentStatus && ODTravelObj.StatusId == 2)  // Added by Rajas on 28 AUGUST 2017
                                                //    {
                                                //        PostingFlagModelObj.IsFullDayOD = true;

                                                //        WetosAdministrationController.GetActualDailyTransactionStatus(PostingFlagModelObj, DailyTransactionObj.Status, NewStatus, ref UpdatedStatus, ref ReturnMessage);
                                                //        DailyTransactionObj.ActualStatus = DailyTransactionObj.Status; // CODE ADDED BY SHRADDHA ON 11 SEP 2017 START
                                                //        DailyTransactionObj.Status = UpdatedStatus;
                                                //    }


                                                //}
                                                #endregion

                                                //EARLIER CODE COMMENTED BY SHRADDHA AND ADDED NEW FUNCTION CalculateODWorkingHours ON 22 NOV 2017 TO GET GENERIC CODE START
                                                WetosAdministrationController.CalculateODLateEarlyWorkingHours(WetosDB, DailyTransactionObj, ODTravelObj, PostingFlagModelObj, CurrentODDate, UpdatedStatus, NewStatus, ref ReturnMessage);
                                                //EARLIER CODE COMMENTED BY SHRADDHA AND ADDED NEW FUNCTION CalculateODWorkingHours ON 22 NOV 2017 TO GET GENERIC CODE END

                                                #endregion

                                                //CODE ADDED BY SHRADDHA ON 05 APR 2017 TO SOLVE LATECOUNT PROBLEM AT OD SANCTION

                                                WetosDB.SaveChanges(); // Verify?

                                                DateTime FirstDate = new DateTime(DailyTransactionObj.TranDate.Year, DailyTransactionObj.TranDate.Month, 1);
                                                DateTime LastDate = FirstDate.AddMonths(1).AddDays(-1);

                                                // Updated by Rajas on 29 AUGUST 2017 START
                                                List<DailyTransaction> AllEmpLatetData = WetosDB.DailyTransactions.Where(a => a.TranDate >= DailyTransactionObj.TranDate
                                              && a.TranDate <= LastDate && a.EmployeeId == DailyTransactionObj.EmployeeId).ToList();  // && (a.Remark.ToUpper() == "LATE")

                                                List<DailyTransaction> AllEmpEarlyData = WetosDB.DailyTransactions.Where(a => a.TranDate >= DailyTransactionObj.TranDate
                                             && a.TranDate <= LastDate && a.EmployeeId == DailyTransactionObj.EmployeeId).ToList();  // && (a.Remark.ToUpper() == "EARLY") 
                                                // Updated by Rajas on 29 AUGUST 2017 END

                                                //Code Added By Shraddha on 06 JUNE 2017 To Get LateCount from Rules start
                                                int EmployeeGroupIdObj = WetosDB.EmployeeGroupDetails.Where(a => a.Employee.EmployeeId == DailyTransactionObj.EmployeeId)
                                                    .Select(a => a.EmployeeGroup.EmployeeGroupId).FirstOrDefault();
                                                List<RuleTransaction> RuleTransactionList = WetosDB.RuleTransactions.Where(a => a.CompanyId == DailyTransactionObj.CompanyId && a.BranchId == DailyTransactionObj.BranchId && a.EmployeeGroupId == EmployeeGroupIdObj).ToList();
                                                RuleTransaction RuleForLateCount = RuleTransactionList.Where(a => a.RuleId == 16).FirstOrDefault();

                                                //Code Added By Shraddha on 06 JUNE 2017 To Get LateCount from Rules end

                                                #region CODE TO DEDUCT ALREADY MARKED LATE COUNT

                                                NewStatus = string.Empty;
                                                UpdatedStatus = string.Empty;
                                                ReturnMessage = string.Empty;

                                                // Updated by Rajas on 5 AUGUST 2017 START
                                                if (AllEmpLatetData != null)
                                                {
                                                    foreach (DailyTransaction DailyTransObj in AllEmpLatetData)
                                                    {
                                                        if (RuleForLateCount != null)
                                                        {

                                                            if (DailyTransObj.LateCount >= 1)
                                                            {
                                                                DailyTransObj.LateCount = DailyTransObj.LateCount - 1;
                                                                WetosDB.SaveChanges();

                                                                PostingFlagModelObj.IsLateCountReduced = true;

                                                                if (DailyTransObj.LateCount <= Convert.ToInt32(RuleForLateCount.Formula))
                                                                {
                                                                    WetosAdministrationController.GetActualDailyTransactionStatus(PostingFlagModelObj, DailyTransactionObj.Status, NewStatus, ref UpdatedStatus, ref ReturnMessage);
                                                                    DailyTransObj.Remark = string.Empty;  // Updated by Rajas on 10 AUGUST 2017
                                                                    DailyTransObj.Status = UpdatedStatus;
                                                                    WetosDB.SaveChanges();
                                                                }

                                                                else if (DailyTransObj.LateCount > Convert.ToInt32(RuleForLateCount.Formula))
                                                                {
                                                                    //abcd.Status = "AAPP^";
                                                                    WetosAdministrationController.GetActualDailyTransactionStatus(PostingFlagModelObj, DailyTransactionObj.Status, NewStatus, ref UpdatedStatus, ref ReturnMessage);
                                                                    DailyTransObj.Remark = "Late";
                                                                    WetosDB.SaveChanges();
                                                                }
                                                            }
                                                            // > 2 ABSENT
                                                            // ==2 PRESENT 

                                                            // Initialise flags
                                                            PostingFlagModelObj = new PostingFlagModel();  // Added by Rajas on 8 AUGUST 2017
                                                        }
                                                    }
                                                }
                                                // Updated by Rajas on 5 AUGUST 2017 END

                                                #endregion

                                                #region CODE TO DEDUCT ALREADY MARKED EARLY COUNT

                                                NewStatus = string.Empty;
                                                UpdatedStatus = string.Empty;
                                                ReturnMessage = string.Empty;
                                                if (AllEmpEarlyData != null)
                                                {
                                                    // Updated by Rajas on 5 AUGUST 2017 START
                                                    foreach (DailyTransaction DailyTransObj in AllEmpEarlyData)
                                                    {
                                                        if (RuleForLateCount != null)
                                                        {
                                                            if (DailyTransObj.EarlyCount >= 1)
                                                            {
                                                                DailyTransObj.EarlyCount = DailyTransObj.EarlyCount - 1;
                                                                WetosDB.SaveChanges();

                                                                PostingFlagModelObj.IsLateCountReduced = true;

                                                                if (DailyTransObj.LateCount <= Convert.ToInt32(RuleForLateCount.Formula))
                                                                {
                                                                    WetosAdministrationController.GetActualDailyTransactionStatus(PostingFlagModelObj, DailyTransactionObj.Status, NewStatus, ref UpdatedStatus, ref ReturnMessage);
                                                                    DailyTransObj.Remark = string.Empty;  // Updated by Rajas on 10 AUGUST 2017
                                                                    DailyTransObj.Status = UpdatedStatus;
                                                                    WetosDB.SaveChanges();
                                                                }

                                                                else if (DailyTransObj.EarlyCount > Convert.ToInt32(RuleForLateCount.Formula))
                                                                {
                                                                    //abcd.Status = "AAPP^";
                                                                    WetosAdministrationController.GetActualDailyTransactionStatus(PostingFlagModelObj, DailyTransactionObj.Status, NewStatus, ref UpdatedStatus, ref ReturnMessage);
                                                                    DailyTransObj.Remark = "Late";
                                                                    WetosDB.SaveChanges();
                                                                }
                                                            }
                                                            // > 2 ABSENT
                                                            // ==2 PRESENT 

                                                            // Initialise flags
                                                            PostingFlagModelObj = new PostingFlagModel();  // Added by Rajas on 8 AUGUST 2017
                                                        }
                                                    }
                                                }
                                                // Updated by Rajas on 5 AUGUST 2017 END

                                                #endregion

                                                //ADDED CODE FOR ADDING HALF DAY AND FULL DAY OD WISE STATUS IN DAILY TRANSACTION TABLE BY SHRADDHA ON 13 FEB 2017 END

                                                WetosDB.SaveChanges();
                                            }

                                        }

                                        else
                                        {
                                            Information("You can not sanction this Late/Early as you are not OD/Travel sanctioner for selected employee");

                                            List<SP_WetosGetEmployeeODLateEarlySanction_Result> ODTObj = WetosDB.SP_WetosGetEmployeeODLateEarlySanction(EmpNo).OrderByDescending(a => a.FromDate).OrderByDescending(a => a.ToDate).ToList();
                                            //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 START
                                            PopulateDropDown();
                                            //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 END

                                            return View(ODTObj);
                                        }
                                    }
                                    //CODE ADDED BY SHRADDHA ON 13 FEB 2017 CONDITION ADDED FOR TODATE START
                                    else if (item.StatusId == 3 || item.StatusId == 6 || item.StatusId == 5)
                                    {
                                        if (DailyTransactionObj != null)
                                        {
                                            DailyTransactionObj.Status = DailyTransactionObj.ActualStatus; // CODE ADDED BY SHRADDHA ON 11 SEP 2017 START
                                            WetosDB.SaveChanges();
                                        }
                                        ODTravelObj.RejectReason = RejectReasonText;
                                        WetosDB.SaveChanges();

                                    }

                                        //UPDATED BY SHRADDHA ON 11 SEP 2017 TAKEN STATUSID = 5
                                    else if (item.StatusId == 5)   // Added by Rajas on 9 JULY 2017 to Fix Issue no. 2, defect id= FB0012 as per Test Cases sheet
                                    {
                                        if (DailyTransactionObj != null)
                                        {
                                            DailyTransactionObj.Status = DailyTransactionObj.ActualStatus;// CODE ADDED BY SHRADDHA ON 11 SEP 2017 START
                                            WetosDB.SaveChanges();
                                        }
                                        // Cancellation reason code
                                        // Added by Rajas on 12 AUGUST 2017 START
                                        if (RejectReasonText == null || RejectReasonText == string.Empty)
                                        {
                                            ModelState.AddModelError("CustomError", "Please Enter Cancellation Reason");

                                            //List<SP_WetosGetEmployeeODTravelSanction_Result> ODTravelSanctionList = WetosDB.SP_WetosGetEmployeeODTravelSanction(EmpNo).OrderByDescending(a => a.FromDate).OrderByDescending(a => a.ToDate).ToList();

                                            // Added by Rajas on 9 MAY 2017
                                            // Populate Status dropdown
                                            PopulateDropDown();

                                            Error("Please enter  Cancellation Reason");

                                            return View(ODTravelSanction); // ODTravelSanctionList
                                        }
                                        // Added by Rajas on 12 AUGUST 2017 END

                                        if (DailyTransactionObj != null)
                                        {
                                            // Updated by Rajas on 5 AUGUST 2017, restored Actual status 
                                            if (DailyTransactionObj != null)
                                            {
                                                DailyTransactionObj.Status = DailyTransactionObj.ActualStatus; // "AAAA"
                                                WetosDB.SaveChanges();
                                            }
                                        }
                                    }
                                }
                                //CODE ADDED BY SHRADDHA ON 13 FEB 2017 CONDITION ADDED FOR TODATE START

                               
                                // ADDED BY MSJ ON 09 JAN 2019 START
                                ODTravelObjEDIT.AppliedBy = Convert.ToInt32(Session["EmployeeNo"]);
                                ODTravelObjEDIT.AppliedOn = DateTime.Now;
                                ODTravelObjEDIT.SanctionedBy = Convert.ToInt32(Session["EmployeeNo"]);
                                ODTravelObjEDIT.SanctionedOn = DateTime.Now;
                                // ADDED BY MSJ ON 09 JAN 2019 END
                               

                                WetosDB.SaveChanges();


                                //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 12 SEPTEMBER 2017
                                #region ADD AUDIT LOG
                                string Oldrecord = "ODTourType : " + ODTravelObjEDIT.ODTourType + ", FromDate : " + ODTravelObjEDIT.FromDate
                                   + ", ToDate : " + ODTravelObjEDIT.ToDate + ", ODDayStatus : " + ODTravelObjEDIT.ODDayStatus
                                   + ", ODDayStatus1 : " + ODTravelObjEDIT.ODDayStatus1 + ", AppliedDays :" + ODTravelObjEDIT.AppliedDay + ", ActualDays :"
                                   + ODTravelObjEDIT.ActualDay + ", Purpose :" + ODTravelObjEDIT.Purpose + ", Status :" + ODTravelObjEDIT.Status
                                   + ", BranchId :" + ODTravelObjEDIT.BranchId
                                   + ", CompanyId :" + ODTravelObjEDIT.CompanyId;
                                //---x---------x-----------------x------------x----------------x-----------x-------------x------------x--------------x----------x---------x-----
                                string Newrecord = "ODTourType : " + ODTravelObj.ODTourType + ", FromDate : " + ODTravelObj.FromDate
                                   + ", ToDate : " + ODTravelObj.ToDate + ", ODDayStatus : " + ODTravelObj.ODDayStatus
                                   + ", ODDayStatus1 : " + ODTravelObj.ODDayStatus1 + ", AppliedDays :" + ODTravelObj.AppliedDay + ", ActualDays :"
                                   + ODTravelObj.ActualDay + ", Purpose :" + ODTravelObj.Purpose + ", Status :" + ODTravelObj.Status
                                   + ", BranchId :" + ODTravelObj.BranchId
                                   + ", CompanyId :" + ODTravelObj.CompanyId;

                                //LOGIN ID TAKEN FROM SESSION PERSISTER
                                string Formname = "Late/Early SANCTION";
                                //ACTION IS UPDATE
                                string Message = " ";

                                WetosAdministrationController.GenerateAuditLogsUpdate(WetosDB, ODTravelObj.EmployeeId, Formname, Oldrecord,
                                    Newrecord, ref Message);
                                #endregion
                                //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 12 SEPTEMBER 2017


                                #region PROCESS OD/TRAVEL APPLICATION

                                // Updated by Rajas on 5 AUGUST 2017
                                string ErrorMessage = string.Empty;
                                if (WetosAdministrationController.ODOTProcessingEx(WetosDB, Convert.ToDateTime(item.FromDate), Convert.ToDateTime(item.ToDate), ODTravelObj.EmployeeId, ref ErrorMessage) == false)
                                {
                                    AddAuditTrail(ErrorMessage);
                                }

                                #endregion

                                // Added by Rajas on 20 MAY 2017
                                // Common POSTING code
                                //ProcessAttendance(item.FromDate.Value, item.ToDate.Value);

                                // NOTIFICATION ADDED BY RAJAS ON 31 DEC 2016
                                #region OD SANCTION NOTIFICATION

                                // Notification from Reporting person to Employee
                                Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == ODTravelObj.EmployeeId).FirstOrDefault();
                                Notification NotificationObj = new Notification();
                                NotificationObj.FromID = EmployeeObj.EmployeeReportingId;
                                NotificationObj.ToID = EmployeeObj.EmployeeId;
                                NotificationObj.SendDate = DateTime.Now;
                                string StatusName = WetosDB.DropdownDatas.Where(a => a.TypeId == 3 && a.Value == item.StatusId).Select(a => a.Text).FirstOrDefault();
                                NotificationObj.NotificationContent = "Your Late/Early application from " + ODTravelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " to " + ODTravelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " is " + StatusName;
                                //NotificationObj.NotificationContent = "Leave applied from " + LeaveApplicationObj.FromDate.ToString("dd-MMM-yyyy") + " to " + LeaveApplicationObj.ToDate.ToString("dd-MMM-yyyy") + " is " + LeaveApplicationObj.Status;
                                NotificationObj.ReadFlag = false;
                                NotificationObj.SendDate = DateTime.Now;

                                WetosDB.Notifications.AddObject(NotificationObj);

                                WetosDB.SaveChanges();

                                if (ODTravelObj.StatusId == 4 && ValidEmpObj.EmployeeReportingId == EmpNo)
                                {

                                    // Notification from Reporting person to Employee
                                    Notification NotificationObj2 = new Notification();
                                    NotificationObj.FromID = EmployeeObj.EmployeeId;
                                    NotificationObj.ToID = EmployeeObj.EmployeeReportingId2;
                                    NotificationObj.SendDate = DateTime.Now;
                                    NotificationObj.NotificationContent = "Received Late/Early application for sanctioning from " + EmployeeObj.FirstName + " " + EmployeeObj.LastName + " - from " + ODTravelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " to " + ODTravelObj.ToDate.Value.ToString("dd-MMM-yyyy");
                                    //NotificationObj.NotificationContent = "Leave applied from " + LeaveApplicationObj.FromDate.ToString("dd-MMM-yyyy") + " to " + LeaveApplicationObj.ToDate.ToString("dd-MMM-yyyy") + " is " + LeaveApplicationObj.Status;
                                    NotificationObj.ReadFlag = false;
                                    NotificationObj.SendDate = DateTime.Now;

                                    WetosDB.Notifications.AddObject(NotificationObj2);

                                    WetosDB.SaveChanges();

                                }

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
                                            if (SendEmail(EmployeeObj.Email, NotificationObj.NotificationContent, NotificationObj.NotificationContent, ref EmailUpdateStatus, "OD/Travel Sanction") == false)
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
                            }
                            else
                            {
                                Error("Inconsistent data, Please try again!!");

                                return View("Error");
                            }
                        }
                    }
                    // int EmpNo = Convert.ToInt32(Session["EmployeeNo"]);

                    //List<SP_WetosGetEmployeeLeaveSanction_Result> LeaveSanctionObj1 = WetosDB.SP_WetosGetEmployeeLeaveSanction(EmpNo).ToList();

                    List<SP_WetosGetEmployeeODLateEarlySanction_Result> ODTOURObj = WetosDB.SP_WetosGetEmployeeODLateEarlySanction(EmpNo).OrderByDescending(a => a.FromDate).OrderByDescending(a => a.ToDate).ToList();

                    // ADDED BY PUSHKAR FOR AuditLog ON 21 DEC 2016
                    AddAuditTrail("Success - Late/Early application processed successfully");  // Updated by Rajas on 17 JAN 2017

                    // Added by Rajas on 17 JAN 2017 START
                    Success("Success - Late/Early application processed successfully");

                    // Added by Rajas on 9 MAY 2017
                    // Populate Status dropdown
                    //REPLACED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 START
                    //List<string> StatusObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 3).Select(a => a.Text).ToList();
                    //ViewBag.StatusList = StatusObj;
                    PopulateDropDown();
                    //REPLACED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 END

                    return View(ODTOURObj);
                }
            }

            catch (System.Exception ex)
            {
                AddAuditTrail("Error in Late/Early due to " + ex.Message + ( ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                Error("Please try again!!");

                //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 START
                PopulateDropDown();
                //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 END
                return View("Error");
            }
        }


        //#######################################################

        /// <summary>
        /// FUNCTION ADDED BY SHRADDHA ON 20 SEP 2017
        /// CODE TO SANCTION CO IF CO TYPE LEAVE NOT APPLICABLE
        /// DAILY TRANSACTION REPORT SHOULD GET DIRECTLY UPDATED IN CASE OF SANCTION OR SANCTION THEN REJECT/CANCEL CO APPLICATION
        /// <param name="RejectReason"></param>
        /// <returns></returns>
        public static bool COSanctionIfCOLeaveNotAvailable(List<SP_WetosGetEmployeeCompOffSanction_Result> CompOffSanction, string RejectReason, WetosDBEntities WetosDB)
        {
            bool RetStat = true;

            return RetStat;
        }
    }

}
