using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WetosDB;
using WetosMVCMainApp.Models;
using System.Globalization;
using System.Configuration;
using System.Net.Mail;
using System.Text;
using System.IO;

namespace WetosMVC.Controllers
{
    public class WetosLeaveTransactionController : BaseController
    {
        #region LEAVE RULE USED AND VERIFIED

        //1.  NoOfDaysAllowedInYear - NOT USED
        //2.  MaxNoOfTimesAllowedInYear - USED
        //3.  MaxNoOfLeavesAllowedAtaTime - USED
        //4.  MaxNoOfLeavesAllowedInMonth - USED
        //5.  AccumulationAllowedOrNot - NOT USED
        //6.  MaxAccumulationDays - NOT USED
        //7.  EncashmentAllowedOrNot - NOT USED
        //8.  NoOfDaysEncashed - NOT USED
        //9.  NegativeBalanceAllowed - NOT USED
        //10. IsCarryForword - NOT USED
        //11. WOBetLevConsiderLeave - USED
        //12. HHBetLevConsiderLeave - USED
        //13. ISHalfAllowed - USED
        //14. ApplicableToMaleFemale - NOT USED
        //15. IsLeaveCombination - USED
        //16. MinNoofLeaveAllowedatatime - USED
        //17. IsInsufficientBalAllowed - USED
        //18. IsPendingLeaveConsider - NOT USED

        #endregion

        public ActionResult Index()
        {
            return View();
        }

        #region Leave Application

        #region Get Leave Application List
        // GET: /WetosApplication/Create
        /// <summary>
        /// 
        /// </summary>
        /// <param name="IsMySelf"></param>
        /// <returns></returns>
        public ActionResult LeaveApplicationIndex(string IsMySelf = "true")
        {
            try
            {
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);

                // Added by Rajas on 17 MARCH 2017 START

                // Get current FY from global setting
                //GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == "Current Financial Year").FirstOrDefault();

                //ABOVE LINE COMMENTED AND BELOW LINE IS ADDED BY SHALAKA ON 13TH DEC 2017
                GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.CurrentFinancialYear).FirstOrDefault();

                #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST

                //List<VwLeaveApplicationIndex> LeaveApplicationListObj = new List<VwLeaveApplicationIndex>();  // Verify ?
                List<SP_VwLeaveApplicationIndex_Result> LeaveApplicationListObj = new List<SP_VwLeaveApplicationIndex_Result>();  // Verify ?
                #endregion

                // Error
                if (GlobalSettingObj == null)
                {
                    Error("Inconsistent Financial year data");

                    return View(LeaveApplicationListObj);
                }

                DateTime CalanderStartDate = GetFYStartDate();
                //DateTime CalanderStartDate = GetFYStartDate(); // DateTime CalanderStartDate = GetFYStartDate(); // DateTime CalanderStartDate = WetosDB.FinancialYears.Where(a => a.FinancialName == GlobalSettingObj.SettingValue).Select(a => a.StartDate).FirstOrDefault();


                //FinancialYear TempCurrentFY = WetosDB.FinancialYears.Where(a => a.FinancialName == GlobalSettingObj.SettingValue).FirstOrDefault();
                //FinancialYear TempPreviousFY = WetosDB.FinancialYears.Where(a => a.FinancialYearId == TempCurrentFY.PrevFYId).FirstOrDefault();

                ////FinancialYear FY = TempPreviousFY == null ? TempCurrentFY : TempPreviousFY;

                //CalanderStartDate = TempPreviousFY == null ? TempCurrentFY.StartDate : TempPreviousFY.StartDate;

                // DateTime CalanderStartDate = new DateTime(2016, 01, 01); // PLS CHECK ?

                if (CalanderStartDate != null)
                {
                    if (IsMySelf == "true")
                    {
                        // Updated by Rajas on 29 MARCH 2017
                        #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                        //int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                        //LeaveApplicationListObj = WetosDB.VwLeaveApplicationIndexes.Where(a => a.EmployeeId == EmployeeId
                        //    && a.FromDate >= CalanderStartDate && (a.MarkedAsDelete == 0 || a.MarkedAsDelete == null)).OrderByDescending(a => a.FromDate).ToList();  // || a.Status == 5
                        LeaveApplicationListObj = WetosDB.SP_VwLeaveApplicationIndex(EmployeeId).Where(a => a.EmployeeId == EmployeeId
                            && a.FromDate >= CalanderStartDate && (a.MarkedAsDelete == 0 || a.MarkedAsDelete == null)).OrderByDescending(a => a.FromDate).ToList();  // || a.Status == 5
                        #endregion
                    }
                    else  // Added by Rajas on 29 AUGUST 2017
                    {
                        #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                        //LeaveApplicationListObj = WetosDB.VwLeaveApplicationIndexes.Where(a => a.FromDate >= CalanderStartDate
                        //    && a.MarkedAsDelete == 0 && a.EmployeeId != EmployeeId).OrderByDescending(a => a.FromDate).ToList();
                        LeaveApplicationListObj = WetosDB.SP_VwLeaveApplicationIndex(EmployeeId).Where(a => a.FromDate >= CalanderStartDate
                            && a.MarkedAsDelete == 0 && a.EmployeeId != EmployeeId).OrderByDescending(a => a.FromDate).ToList();
                        #endregion
                    }

                }
                // Added by Rajas on 17 MARCH 2017 END

                ViewBag.ForOthers = IsMySelf;

                //ADDED BY RAJAS ON 27 DEC 2016
                AddAuditTrail("Success - Visited Leave application"); // Updated by Rajas on 17 JAN 2017

                return View(LeaveApplicationListObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in Leave application list view due to " + ex.Message + ( ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                Error("Error in viewing list for Leave application");

                return View("Error");
            }
        }
        #endregion

        #region Populate Leave Data View

        // Added by Rajas on 29 SEP 2017 START

        /// <summary>
        ///  PARTIALVIEW TO DISPLAY LEAVE DETAILS FOR SELECTED EMP
        ///  FUNCTION FOR OTHER'S LEAVE APPLICATION
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="BranchId"></param>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <param name="selectCriteria"></param>
        /// <param name="EmployeeId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LeaveDataView(int EmpId, FormCollection fc)
        {
            try
            {
                //Modified by Shraddha on 07 FEB 2016 for getting Leave type dropdown data based on EMPLOYEE TYPE, EMPLOYEE COMPANY, EMPLOYEE BRANCH Start
                Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == EmpId).FirstOrDefault();

                // Added by Rajas on 23 FEB 2017 generate list for available leaves as per employee group assigned
                EmployeeGroupDetail EmployeeGroupDetailObj = WetosDB.EmployeeGroupDetails.Where(a => a.Employee.EmployeeId == EmpId).FirstOrDefault();

                if (EmployeeObj != null && EmployeeGroupDetailObj != null)  // Updated by Rajas on 23 FEB 2017, added && EmployeeGroupDetailObj != null
                {
                    // Updated by Rajas on 23 FEB 2017, added && a.EmployeeGrpId == EmployeeGroupDetailObj.EmployeeGroupId
                    //MODIFIED BY PUSHKAR ON 28 NOV 2017 FOR CHECKING MARK AS DELETE CONDITION
                    var LeaveMasterList = WetosDB.LeaveMasters.Where(a => a.Company.CompanyId == EmployeeObj.CompanyId && a.BranchId == EmployeeObj.BranchId
                        && a.EmployeeGroup.EmployeeGroupId == EmployeeGroupDetailObj.EmployeeGroup.EmployeeGroupId
                        && (a.MarkedAsDelete == 0 || a.MarkedAsDelete == null)).Select(a => new { LeaveTypeID = a.LeaveId, LeaveType = a.LeaveCode }).ToList();

                    ViewBag.LeaveTypeList = new SelectList(LeaveMasterList, "LeaveTypeID", "LeaveType").ToList();

                    // Added by Rajas on 9 MARCH 2017 for Displaying error message if rules are not set to employee group
                    if (LeaveMasterList.Count <= 0)
                    {
                        ModelState.AddModelError("", " Please create leave rules for the particular Employee group");
                    }

                    //List<LeaveBalance> LeaveBalanceList = WetosDB.LeaveBalances.Where(a => a.EmployeeId == EmpId).ToList();

                    //// Added by Rajas on 13 APRIL 2017
                    //List<SP_LeaveTableData_Result> LeaveBalanceList = WetosDB.SP_LeaveTableData(EmpId).ToList();

                    //ADDED BY SAHRADDHA ON 29 DEC 2016
                    // Updated by Rajas on 7 APRIL 2017 for displaying only Sanctioned or Rejected leaves
                    #region CODE ADDED BY SHRADDHA ON 29 MAR 2018
                    string YearInfo = Session["CurrentFinancialYear"].ToString();
                    DateTime FromDate = new DateTime(DateTime.Now.Year, 1, 1);
                    DateTime ToDate = new DateTime((DateTime.Now.Year + 1), 1, 1);
                    try
                    {
                        if (string.IsNullOrEmpty(YearInfo))
                        {
                            try
                            {
                                FromDate = WetosDB.FinancialYears.Where(a => a.FinancialName == YearInfo).Select(a => a.StartDate).FirstOrDefault();
                                ToDate = WetosDB.FinancialYears.Where(a => a.FinancialName == YearInfo).Select(a => a.EndDate).FirstOrDefault();
                            }
                            catch
                            {
                                FromDate = new DateTime(DateTime.Now.Year, 1, 1);
                                ToDate = new DateTime((DateTime.Now.Year + 1), 1, 1);
                            }
                        }
                    }
                    catch
                    {
                        FromDate = new DateTime(DateTime.Now.Year, 1, 1);
                        ToDate = new DateTime((DateTime.Now.Year + 1), 1, 1);
                    }
                    #endregion
                    List<SP_LeaveApplicationDetailsList_Result> LeaveApplicationDetailsList = WetosDB.SP_LeaveApplicationDetailsList(EmpId, FromDate, ToDate).ToList();

                    ViewBag.LeaveApplicationDetailsListVB = LeaveApplicationDetailsList;

                    LeaveApplicationDetailsList = WetosDB.SP_LeaveApplicationDetailsList(EmpId, FromDate, ToDate).Where(a => a.StatusId == 1).ToList();

                    //Added by shraddha on 03 JUNE 2017 created generic function
                    ViewBag.LeaveBalanceDetailsVB = DashboardController.GetLeaveDetailsOnallControllers(WetosDB, EmpId, DateTime.Now); //FinalLeaveBalanceList;


                    // Added by Rajas on 18 MARCH 2017 END

                    string LeaveType = LeaveMasterList.Select(a => a.LeaveType).FirstOrDefault();
                    //Added By Shraddha on 19 JAN 2017 for showing Allowable Days End

                    LeaveCredit LeaveCreditObj = WetosDB.LeaveCredits.Where(a => a.EmployeeId == EmpId).FirstOrDefault();
                    ViewBag.LeaveCreditObjVB = LeaveCreditObj;
                }
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Exception during validating leave " + ex.Message + ( ex.InnerException == null ? string.Empty : ex.InnerException.Message));
            }

            return PartialView();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="EmpId"></param>
        /// <returns></returns>
        public ActionResult LeaveDataView(int EmpId)
        {
            try
            {
                //Modified by Shraddha on 07 FEB 2016 for getting Leave type dropdown data based on EMPLOYEE TYPE, EMPLOYEE COMPANY, EMPLOYEE BRANCH Start
                Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == EmpId).FirstOrDefault();

                // Added by Rajas on 23 FEB 2017 generate list for available leaves as per employee group assigned
                EmployeeGroupDetail EmployeeGroupDetailObj = WetosDB.EmployeeGroupDetails.Where(a => a.Employee.EmployeeId == EmpId).FirstOrDefault();

                if (EmployeeObj != null && EmployeeGroupDetailObj != null)  // Updated by Rajas on 23 FEB 2017, added && EmployeeGroupDetailObj != null
                {
                    // Updated by Rajas on 23 FEB 2017, added && a.EmployeeGrpId == EmployeeGroupDetailObj.EmployeeGroupId
                    //MODIFIED BY PUSHKAR ON 28 NOV 2017 FOR CHECKING MARK AS DELETE CONDITION
                    var LeaveMasterList = WetosDB.LeaveMasters.Where(a => a.Company.CompanyId == EmployeeObj.CompanyId && a.BranchId == EmployeeObj.BranchId
                        && a.EmployeeGroup.EmployeeGroupId == EmployeeGroupDetailObj.EmployeeGroup.EmployeeGroupId
                        && (a.MarkedAsDelete == 0 || a.MarkedAsDelete == null))
                        .Select(a => new { LeaveTypeID = a.LeaveId, LeaveType = a.LeaveCode }).ToList();

                    ViewBag.LeaveTypeList = new SelectList(LeaveMasterList, "LeaveTypeID", "LeaveType").ToList();

                    // Added by Rajas on 9 MARCH 2017 for Displaying error message if rules are not set to employee group
                    if (LeaveMasterList.Count <= 0)
                    {
                        ModelState.AddModelError("", " Please create leave rules for the particular Employee group");
                    }

                    //List<LeaveBalance> LeaveBalanceList = WetosDB.LeaveBalances.Where(a => a.EmployeeId == EmpId).ToList();

                    // Added by Rajas on 13 APRIL 2017
                    List<SP_LeaveTableData_Result> LeaveBalanceList = WetosDB.SP_LeaveTableData(EmpId).ToList();

                    //ViewBag.LeaveBalanceDetailsVB = WetosEmployeeController.GetLeavedataNewLogic(WetosDB, EmployeeObj.EmployeeId,DateTime.Now);


                    //Added by shraddha on 03 JUNE 2017 created generic function
                    ViewBag.LeaveBalanceDetailsVB = DashboardController.GetLeaveDetailsOnallControllers(WetosDB, EmpId, DateTime.Now); //FinalLeaveBalanceList;

                    //ADDED BY SAHRADDHA ON 29 DEC 2016
                    // Updated by Rajas on 7 APRIL 2017 for displaying only Sanctioned or Rejected leaves

                    #region CODE ADDED BY SHRADDHA ON 29 MAR 2018
                    string YearInfo = Session["CurrentFinancialYear"].ToString();
                    DateTime FromDate = new DateTime(DateTime.Now.Year, 1, 1);
                    DateTime ToDate = new DateTime((DateTime.Now.Year + 1), 1, 1);
                    try
                    {
                        if (string.IsNullOrEmpty(YearInfo))
                        {
                            try
                            {
                                FromDate = WetosDB.FinancialYears.Where(a => a.FinancialName == YearInfo).Select(a => a.StartDate).FirstOrDefault();
                                ToDate = WetosDB.FinancialYears.Where(a => a.FinancialName == YearInfo).Select(a => a.EndDate).FirstOrDefault();
                            }
                            catch
                            {
                                FromDate = new DateTime(DateTime.Now.Year, 1, 1);
                                ToDate = new DateTime((DateTime.Now.Year + 1), 1, 1);
                            }
                        }
                    }
                    catch
                    {
                        FromDate = new DateTime(DateTime.Now.Year, 1, 1);
                        ToDate = new DateTime((DateTime.Now.Year + 1), 1, 1);
                    }
                    #endregion
                    List<SP_LeaveApplicationDetailsList_Result> LeaveApplicationDetailsList = WetosDB.SP_LeaveApplicationDetailsList(EmpId, FromDate, ToDate).ToList();

                    ViewBag.LeaveApplicationDetailsListVB = LeaveApplicationDetailsList;

                    LeaveApplicationDetailsList = WetosDB.SP_LeaveApplicationDetailsList(EmpId, FromDate, ToDate).Where(a => a.StatusId == 1).ToList();


                    // Added by Rajas on 18 MARCH 2017 END

                    string LeaveType = LeaveMasterList.Select(a => a.LeaveType).FirstOrDefault();
                    //Added By Shraddha on 19 JAN 2017 for showing Allowable Days End

                    LeaveCredit LeaveCreditObj = WetosDB.LeaveCredits.Where(a => a.EmployeeId == EmpId).FirstOrDefault();
                    ViewBag.LeaveCreditObjVB = LeaveCreditObj;
                }
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Exception during validating leave " + ex.Message + ( ex.InnerException == null ? string.Empty : ex.InnerException.Message));
            }

            return PartialView();
        }

        // Added by Rajas on 29 SEP 2017 END

        #endregion Populate Leave Data View

        #region Apply leave Module
        /// <summary>
        /// Try catch block added by Rajas on 14 FEB 2017
        /// </summary>
        /// <returns></returns>
        public ActionResult LeaveApplication(bool MySelf)
        {
            try
            {
                LeaveModel LeaveModelObj = new LeaveModel();

                LeaveModelObj.MySelf = MySelf;
                LeaveModelObj.ToDate = DateTime.Now;
                LeaveModelObj.FromDate = DateTime.Now;
                LeaveModelObj.EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);

                // Added by Rajas on 30 MARCH 2017 START
                string UpdateStatus = string.Empty;

                if (LeaveApplicationNecessaryContent(LeaveModelObj.EmployeeId, ref UpdateStatus) == false)
                {
                    AddAuditTrail("Error in Leave application due to " + UpdateStatus);

                    Error(UpdateStatus);

                    if (MySelf == true)
                    {
                        return RedirectToAction("LeaveApplicationIndex");
                    }
                    else
                    {
                        return RedirectToAction("LeaveApplicationIndex", new { IsMySelf = "false" });
                    }
                }
                // Added by Rajas on 30 MARCH 2017 END

                LeaveModelObj.ActualDays = Convert.ToDouble(Session["AllowedLeaves"]);

                ViewBag.DeductDays = 0.0;

                // Populate select2 
                PopulateDropDown();

                return View(LeaveModelObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Exception - Leave application failed due to " + ex.Message);

                Error("Error - Leave application failed");

                //  PopulateDropDown();

                if (MySelf == true)
                {
                    return RedirectToAction("LeaveApplicationIndex");
                }
                else
                {
                    return RedirectToAction("LeaveApplicationIndex", new { IsMySelf = "false" });
                }
            }
        }
        /// <summary>
        /// Leave Application
        /// </summary>
        /// <param name="LeaveTypeObj"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        /// UPDATED BY RAJAS ON 14 AUGUST 2017
        [HttpPost]
        public ActionResult LeaveApplication(LeaveModel LeaveTypeObj, FormCollection collection, HttpPostedFileBase MediCert)    //Modified By Shraddha on 17TH Oct 2016 for model validation
        {
            try
            {
                int EmpId = LeaveTypeObj.EmployeeId;

                //// In case of self leave application
                //if (EmpId <= 0)
                //{
                //    EmpId = Convert.ToInt32(Session["EmployeeNo"]);
                //}

                //string SandwitchCaseDetected = collection["SandwitchCase"];

                double DeductDays = LeaveTypeObj.DayDeduct;

                string ErrorMessage = string.Empty;
                //bool ReturnStatus = true;

                if (ModelState.IsValid)
                {

                    // 1. CHECK prev date or post date
                    // 2. in prev date then already present // STOP                                                      

                    // 4. IS SUFFICIENT BALANCE
                    double LeaveAllowed = WetosEmployeeController.GetLeaveAllowed(WetosDB, EmpId, ref LeaveTypeObj, false);

                    //double AvailableLeaveBalance = WetosEmployeeController.GetAvailableLeaveBalance(WetosDB, EmpId, ref LeaveTypeObj);

                    double AppliedDays = CalculateAppliedDays(LeaveTypeObj.FromDate, LeaveTypeObj.ToDate, LeaveTypeObj.FromDayStatus, LeaveTypeObj.ToDayStatus);
                    LeaveTypeObj.AppliedDays = AppliedDays;

                    // 3. IS SC
                    // Added by Rajas on 21 JULY 2017
                    // Static IsSandwich function
                    bool IsLeaveCombine = false;   // Added by Rajas on 14 AUGUST 2017
                    IsSandwich(WetosDB, EmpId, ref ErrorMessage, ref LeaveTypeObj, ref IsLeaveCombine);

                    // IS LWP CASE 
                    if (AppliedDays > LeaveAllowed)
                    {
                        LeaveTypeObj.IsLWP = true;
                        LeaveTypeObj.LWP = LeaveAllowed - AppliedDays;
                        //System.TimeSpan diffResult = dtTodayNoon - dtYestMidnight;
                    }
                    //else if (AppliedDays > AvailableLeaveBalance)
                    //{

                    //}

                    // 5. Current Balance (CF + CREDIT - SANCTIONED)

                    // 6. Current Balance with pending (CF + CREDIT - SANCTIONED)

                    // 7. Generete Message

                    // ADD TO DATABASE 
                    LeaveApplication LeaveApplicationObj = new LeaveApplication();

                    // Updated by Rajas on 30 AUGUST 2017 START
                    // Earlier Company and branchId updated in database as 0
                    Employee EmpObj = WetosDB.Employees.Where(a => a.EmployeeId == EmpId).FirstOrDefault();
                    LeaveApplicationObj.CompanyId = EmpObj.CompanyId; //LeaveTypeObj.CompanyId;
                    LeaveApplicationObj.BranchId = EmpObj.BranchId; //LeaveTypeObj.Branchid;
                    // Updated by Rajas on 30 AUGUST 2017 END

                    /// Below code change done due to Employee id during saving to db was same as login id and not as selected employee id in case of other's application
                    LeaveApplicationObj.EmployeeId = EmpId;

                    LeaveApplicationObj.FromDate = Convert.ToDateTime(LeaveTypeObj.FromDate);
                    LeaveApplicationObj.ToDate = Convert.ToDateTime(LeaveTypeObj.ToDate);

                    //LeaveApplicationObj.LeaveTypeId = LeaveTypeObj.LeaveType;
                    LeaveApplicationObj.Purpose = LeaveTypeObj.Purpose;
                    //LeaveApplicationObj.LeaveType = LeaveTypeObj.LeaveName; // MODIFIED BY MSJ on 22 JULY 2017  // COMMENTED BY SHRADDHA ON 29 NOV 2017  TO HANDLE EXCEPTION INCASE OF HAVING LEAVENAME OF MORE THAN 10 CHARACTERS
                    LeaveApplicationObj.LeaveType = LeaveTypeObj.LeaveCode; //ADDED BY SHRADDHA ON 29 NOV 2017  TO HANDLE EXCEPTION INCASE OF HAVING LEAVENAME OF MORE THAN 10 CHARACTERS
                    LeaveApplicationObj.ActualDays = LeaveTypeObj.ActualDays;

                    //Added by shraddha on 19th Oct 2016 for Saving Applied Days Start
                    LeaveApplicationObj.AppliedDays = AppliedDays; // LeaveTypeObj.AppliedDays;
                    LeaveApplicationObj.StatusId = LeaveTypeObj.StatusId;

                    string Status = WetosDB.DropdownDatas.Where(a => a.TypeId == 3 && a.Value == LeaveTypeObj.StatusId).Select(a => a.Text).FirstOrDefault();
                    LeaveApplicationObj.Status = Status;

                    LeaveApplicationObj.FromDayStatus = LeaveTypeObj.FromDayStatus;
                    LeaveApplicationObj.ToDayStatus = LeaveTypeObj.ToDayStatus;

                    // ADDED LATER BY MSJ 
                    LeaveApplicationObj.TotalDeductableDays = LeaveTypeObj.TotalDeductableDays;

                    LeaveApplicationObj.IsLWP = LeaveTypeObj.IsLWP;
                    LeaveApplicationObj.IsPreSandwichCase = LeaveTypeObj.IsPreSandwichCase;
                    LeaveApplicationObj.IsPostSandwichCase = LeaveTypeObj.IsPostSandwichCase;

                    LeaveApplicationObj.EffectiveDate = LeaveTypeObj.EffectiveDate; // CODE ADDED BY SHRADDHA ON 16 MAR 2018

                    //ADDDED BY PUSHKAR ON 20 JUNE 2018 FOR TEKMAN REQ
                    LeaveApplicationObj.AltContactNo = LeaveTypeObj.AltContactNo;
                    LeaveApplicationObj.LeaveAddress = LeaveTypeObj.LeaveAddress;

                    #region UPLOAD MEDICAL CERTIFICATE
                    // UPLOAD MEDICAL CERTIFICATE
                    if (MediCert != null && MediCert.ContentLength > 0)
                    {
                        try
                        {
                            //Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == UploadDocumentObj.EmployeeID).FirstOrDefault();

                            // ADDED BY MSJ ON 20 JULY 2017 START
                            // GENERATE 
                            string DocContType = "Medical Certificate";

                            //EARLIER CODE COMMENTED AND ADDED NEW CODE BY SHRADDHA AS PER SUGGESTED BY MSJ ON TO HANDLE EMPTY MIDDLE NAME PROBLEM 30 OCT 2017 START
                            //string DocumentFileName = EmployeeObj.FirstName.Trim() + "_" + EmployeeObj.MiddleName.Trim() + "_"
                            //+ EmployeeObj.LastName.Trim() + "_" + DocContType + "_" + (DateTime.Now.ToString("yyyy_MM_dd_HHmmss"));

                            string DocumentFileName = (EmpObj.FirstName == string.Empty ? "" : EmpObj.FirstName.Trim()) + "_" + (EmpObj.MiddleName == string.Empty ? "" : EmpObj.MiddleName.Trim()) + "_"
                                + (EmpObj.LastName == string.Empty ? "" : EmpObj.LastName.Trim()) + "_" + DocContType + "_" + (DateTime.Now.ToString("yyyy_MM_dd_HHmmss"));

                            ////EARLIER CODE COMMENTED AND ADDED NEW CODE BY SHRADDHA AS PER SUGGESTED BY MSJ ON TO HANDLE EMPTY MIDDLE NAME PROBLEM 30 OCT 2017 END
                            DocumentFileName = DocumentFileName.Replace(' ', '_');
                            // ADDED BY MSJ ON 20 JULY 2017 END

                            string Attachment = DocumentFileName;

                            string path = Path.Combine(Server.MapPath("~/User_Data/Document_Upload"), Attachment);
                            string fileExtension = Path.GetExtension(MediCert.FileName);

                            MediCert.SaveAs(path + fileExtension);

                            LeaveApplicationObj.DocFolder = "Document_Upload";
                            LeaveApplicationObj.DocPath = path;
                            LeaveApplicationObj.DocFileName = Attachment + fileExtension;


                            ViewBag.Message = "File Processed Successfully";

                        }
                        catch (System.Exception ex)
                        {

                            AddAuditTrail("Unable to upload medical certificate for  " + EmpObj.EmployeeCode + ( ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                            return View();

                        }


                    }
                    #endregion

                    //COMMENTED BY PUSHKAR ON 9 SEPTEMBER 2017 AS IS NOT REQUIRED NOW NEW MECHANISM IS USED FOR SAME NOW
                    //#region ADD AUDIT LOG

                    ///// Generate Audit Log
                    ///// Added by Rajas on 30 AUGUST 2017
                    //WetosDB.SP_UpdateLeaveApplicationAuditLog(LeaveApplicationObj.LeaveApplicationId, LeaveApplicationObj.EmployeeId, LeaveApplicationObj.LeaveType
                    //    , LeaveApplicationObj.FromDate, LeaveApplicationObj.ToDate, LeaveApplicationObj.FromDayStatus
                    //    , LeaveApplicationObj.ToDayStatus, LeaveApplicationObj.AppliedDays, LeaveApplicationObj.ActualDays, LeaveApplicationObj.Purpose
                    //    , LeaveApplicationObj.Status, LeaveApplicationObj.BranchId, LeaveApplicationObj.CompanyId, Convert.ToInt32(Session["EmployeeNo"]));

                    //#endregion ADD AUDIT LOG

                    // ADDED BY MSJ ON 09 JAN 2019 START
                    LeaveApplicationObj.AppliedBy = Convert.ToInt32(Session["EmployeeNo"]);
                    LeaveApplicationObj.AppliedOn = DateTime.Now;
                    // ADDED BY MSJ ON 09 JAN 2019 END


                    // ADD NEW LEAVE TO DATABASE 
                    WetosDB.LeaveApplications.AddObject(LeaveApplicationObj);
                    WetosDB.SaveChanges();




                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 9 SEPTEMBER 2017
                    #region ADD AUDIT LOG

                    //OLD RECORD IS BLANK
                    string Newrecord = "LeaveType : " + LeaveApplicationObj.LeaveType + ", FromDate : " + LeaveApplicationObj.FromDate
                        + ", ToDate : " + LeaveApplicationObj.ToDate + ", FromDayStatus : " + LeaveApplicationObj.FromDayStatus
                        + ", ToDayStatus :" + LeaveApplicationObj.ToDayStatus + ", AppliedDays :" + LeaveApplicationObj.AppliedDays + ", ActualDays :"
                        + LeaveApplicationObj.ActualDays + ", Purpose :" + LeaveApplicationObj.Purpose + ", Status :" + LeaveApplicationObj.Status
                        + ", BranchId :" + LeaveApplicationObj.BranchId + ", CompanyId :" + LeaveApplicationObj.CompanyId;

                    //LOGIN ID IS TAKEN FROM SESSION PERSISTER
                    string Formname = "LEAVE APPLICATION";
                    //ACTION IS UPDATE
                    string Message = " ";

                    WetosAdministrationController.GenerateAuditLogsInsert(WetosDB, LeaveApplicationObj.EmployeeId, Formname, Newrecord, ref Message);

                    #endregion
                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 9 SEPTEMBER 2017


                    // SEND ALERT
                    #region LEAVE APPLICATION NOTIFICATION

                    //FOR NOTIFICATION TO BE SENT TO REPORTING PERSON AS APPROVER

                    VwActiveEmployee EmployeeObj = WetosDB.VwActiveEmployees.Where(a => a.EmployeeId == EmpId).FirstOrDefault();

                    // FIRST NOTIFICATION
                    Notification NotificationObj = new Notification();
                    NotificationObj.FromID = EmployeeObj.EmployeeId;
                    NotificationObj.ToID = EmployeeObj.EmployeeReportingId;
                    NotificationObj.SendDate = DateTime.Now;
                    NotificationObj.NotificationContent = "Received Leave application for approval from " + EmployeeObj.FirstName + " " + EmployeeObj.LastName + " dated on " + LeaveTypeObj.FromDate.ToString("dd-MMM-yyyy") + " to " + LeaveTypeObj.ToDate.ToString("dd-MMM-yyyy");
                    NotificationObj.ReadFlag = false;
                    NotificationObj.SendDate = DateTime.Now;

                    // ADD TO NOTIFICATION REPORTING PERSON
                    WetosDB.Notifications.AddObject(NotificationObj);
                    WetosDB.SaveChanges();

                    //Check if both reporting person are are different
                    if (EmployeeObj.EmployeeReportingId != EmployeeObj.EmployeeReportingId2)
                    {
                        // FOR NOTIFICATION TO BE SENT TO REPORTING PERSON AS SANCTIONER
                        Notification NotificationObj3 = new Notification();
                        NotificationObj3.FromID = EmployeeObj.EmployeeId;
                        NotificationObj3.ToID = EmployeeObj.EmployeeReportingId;
                        NotificationObj3.SendDate = DateTime.Now;
                        NotificationObj3.NotificationContent = "Received Leave application for sanctioning from " + EmployeeObj.FirstName + " " + EmployeeObj.LastName;
                        NotificationObj3.ReadFlag = false;
                        NotificationObj3.SendDate = DateTime.Now;

                        // ADD TO NOTIFICATION
                        WetosDB.Notifications.AddObject(NotificationObj3);
                        WetosDB.SaveChanges();
                    }

                    //FOR SELF NOTIFICATION

                    Notification NotificationObj2 = new Notification();
                    NotificationObj2.FromID = EmployeeObj.EmployeeId;
                    NotificationObj2.ToID = EmployeeObj.EmployeeId;
                    NotificationObj2.SendDate = DateTime.Now;
                    NotificationObj2.NotificationContent = "Leave applied successfully for " + LeaveTypeObj.FromDate.ToString("dd-MMM-yyyy") + " to " + LeaveTypeObj.ToDate.ToString("dd-MMM-yyyy");
                    NotificationObj2.ReadFlag = false;
                    NotificationObj2.SendDate = DateTime.Now;

                    WetosDB.Notifications.AddObject(NotificationObj2);

                    WetosDB.SaveChanges();

                    //NOTIFICATION COUNT
                    int NoTiCount = WetosDB.Notifications.Where(a => a.ToID == EmpId && (a.ReadFlag == false || a.ReadFlag == null)).Count();
                    Session["NotificationCount"] = NoTiCount;

                    #endregion

                    // Code updated by Rajas on 25 JULY 2017
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
                            Employee EmployeeRep = new Employee();
                            if (EmployeeObj.EmployeeReportingId != EmployeeObj.EmployeeReportingId2)
                            {

                            }
                            else
                            {
                                EmployeeRep = WetosDB.Employees.Where(a => a.EmployeeId == EmployeeObj.EmployeeReportingId).FirstOrDefault();
                            }


                            if (EmployeeRep.Email != null)
                            {
                                // Updated by Rajas on 19 JULY 2017 extra added parameter EmailFromWhichApplication
                                if (SendEmail(EmployeeRep.Email, NotificationObj.NotificationContent, NotificationObj.NotificationContent, ref EmailUpdateStatus, "Leave Application") == false)
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

                    //Added by Rajas on 17 JAN 2017 START
                    // ADD TO AUDIT TRAIL
                    AddAuditTrail("Leave application successfully for " + LeaveTypeObj.FromDate.ToString("dd-MMM-yyyy")
                        + " to " + LeaveTypeObj.ToDate.ToString("dd-MMM-yyyy"));

                    // SUCCESS
                    Success("Leave application successfully for " + LeaveTypeObj.FromDate.ToString("dd-MMM-yyyy")
                        + " to " + LeaveTypeObj.ToDate.ToString("dd-MMM-yyyy"));

                    if (LeaveTypeObj.MySelf == true)
                    {
                        return RedirectToAction("LeaveApplicationIndex");
                    }
                    else
                    {
                        return RedirectToAction("LeaveApplicationIndex", new { IsMySelf = "false" });
                    }
                }
                else
                {

                }
                return View(LeaveTypeObj);
            }
            catch (System.Exception ex)
            {
                PopulateDropDown();

                // Added by Rajas on 30 MARCH 2017 START
                string UpdateStatus = string.Empty;

                if (LeaveApplicationNecessaryContent(LeaveTypeObj.EmployeeId, ref UpdateStatus) == false)
                {
                    AddAuditTrail("Error in Leave application due to " + UpdateStatus);

                    Error(UpdateStatus);

                    if (LeaveTypeObj.MySelf == true)
                    {
                        return RedirectToAction("LeaveApplicationIndex");
                    }
                    else
                    {
                        return RedirectToAction("LeaveApplicationIndex", new { IsMySelf = "false" });
                    }
                }
                // Added by Rajas on 30 MARCH 2017 END

                // Added by Rajas on 17 JAN 2017 START
                AddAuditTrail("Exception - New Leave application : " + LeaveTypeObj.LeaveName + " taken from " + LeaveTypeObj.FromDate.ToString("dd-MMM-yyyy") + " to " + LeaveTypeObj.ToDate.ToString("dd-MMM-yyyy") + " failed due to " + ex.Message + ( ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                Error("Exception - New Leave application failed");
                // Added by Rajas on 17 JAN 2017 END

                return View(LeaveTypeObj);
            }
        }

        #endregion

        #region Update Existing Leave Application

        /// <summary>
        /// Updated by Rajas on 20 MARCH 2017
        /// Fill LeaveModel from LeaveApplication table
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult LeaveApplicationEdit(int id)
        {
            try
            {

                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);

                //CHANGED BY SHRADDHA ON 07 FEB 2017 USED WHERE CONDITION AND FIRSTORDEFAULT INSTEAD OF USING SINGLE() START
                LeaveApplication LeaveApplicationedit = WetosDB.LeaveApplications.Where(a => a.LeaveApplicationId == id).FirstOrDefault();

                LeaveModel LeaveModelObj = new LeaveModel();

                LeaveModelObj.EmployeeId = LeaveApplicationedit.EmployeeId;

                // Added by Rajas on 30 AUGUST 2017 START
                LeaveModelObj.CompanyId = LeaveApplicationedit.CompanyId;

                LeaveModelObj.Branchid = LeaveApplicationedit.BranchId;
                // Added by Rajas on 30 AUGUST 2017 END

                LeaveModelObj.ToDate = LeaveApplicationedit.ToDate;

                LeaveModelObj.FromDate = LeaveApplicationedit.FromDate;

                LeaveModelObj.FromDayStatus = LeaveApplicationedit.FromDayStatus;

                LeaveModelObj.ToDayStatus = LeaveApplicationedit.ToDayStatus;

                LeaveModelObj.Purpose = LeaveApplicationedit.Purpose;

                LeaveModelObj.LeaveApplicationId = LeaveApplicationedit.LeaveApplicationId;

                //LeaveModelObj.LeaveName = LeaveApplicationedit.LeaveType.Trim(); //COMMENTED EARLIER CODE BY SHRADDHA  ON 29 NOV 2017 TO HANDLE EXCEPTION INCASE OF HAVING LEAVENAME OF MORE THAN 10 CHARACTERS
                LeaveModelObj.LeaveCode = LeaveApplicationedit.LeaveType.Trim();//ADDED BY SHRADDHA ON 29 NOV 2017 TO HANDLE EXCEPTION INCASE OF HAVING LEAVENAME OF MORE THAN 10 CHARACTERS
                //LeaveModelObj.LeaveId = LeaveApplicationedit.Le

                //CODE ADDED BY SHRADDHA ON 16 FEB 2018 START
                LeaveModelObj.DocFileName = LeaveApplicationedit.DocFileName;
                LeaveModelObj.DocFolder = LeaveApplicationedit.DocFolder;
                LeaveModelObj.DocPath = LeaveApplicationedit.DocPath;
                //CODE ADDED BY SHRADDHA ON 16 FEB 2018 END

                if (LeaveModelObj.EmployeeId == EmployeeId)
                {
                    LeaveModelObj.MySelf = true;
                }
                else
                {
                    LeaveModelObj.MySelf = false;
                }

                LeaveModelObj.EffectiveDate = LeaveApplicationedit.EffectiveDate; // CODE ADDED BY SHRADDHA ON 16 MAR 2018

                LeaveModelObj.AltContactNo = LeaveApplicationedit.AltContactNo;

                LeaveModelObj.LeaveAddress = LeaveApplicationedit.LeaveAddress;

                // Added by Rajas on 28 MARCH 2017 START
                EmployeeGroupDetail EmployeeGroupDetailObj = WetosDB.EmployeeGroupDetails.Where(a => a.Employee.EmployeeId == LeaveApplicationedit.EmployeeId).FirstOrDefault();

                if (EmployeeGroupDetailObj != null)
                {
                    // MODIFIED BY MSJ ON 25 JULY 2017 START
                    // commented next time as company and branch id IS ZERO 
                    //var LeaveId = WetosDB.LeaveMasters.Where(a => a.Company.CompanyId == LeaveApplicationedit.CompanyId && a.BranchId == LeaveApplicationedit.BranchId
                    //        && a.EmployeeGroup.EmployeeGroupId == EmployeeGroupDetailObj.EmployeeGroup.EmployeeGroupId
                    //        && a.LeaveCode == LeaveModelObj.LeaveName).Select(a => a.LeaveId).FirstOrDefault();

                    //COMMENTED EARLIER CODE BY SHRADDHA  ON 29 NOV 2017 TO HANDLE EXCEPTION INCASE OF HAVING LEAVENAME OF MORE THAN 10 CHARACTERS
                    //var LeaveId = WetosDB.LeaveMasters.Where(a => a.EmployeeGroup.EmployeeGroupId == EmployeeGroupDetailObj.EmployeeGroup.EmployeeGroupId
                    //&& a.LeaveCode == LeaveModelObj.LeaveName).Select(a => a.LeaveId).FirstOrDefault();
                    //ADDED BY SHRADDHA ON 29 NOV 2017 TO HANDLE EXCEPTION INCASE OF HAVING LEAVENAME OF MORE THAN 10 CHARACTERS
                    var LeaveId = WetosDB.LeaveMasters.Where(a => a.EmployeeGroup.EmployeeGroupId == EmployeeGroupDetailObj.EmployeeGroup.EmployeeGroupId
                          && a.LeaveCode == LeaveModelObj.LeaveCode).Select(a => a.LeaveId).FirstOrDefault();
                    // MODIFIED BY MSJ ON 25 JULY 2017 END

                    LeaveModelObj.LeaveId = LeaveId;
                }
                else  // Error displayed in case of inconistent data detected
                {
                    Error("Inconsistent leave data. Please verify data and try again!");

                    AddAuditTrail("Inconsistent Leave data detected during Leave application edit");

                    if (LeaveModelObj.MySelf == true)
                    {
                        return RedirectToAction("LeaveApplicationIndex");
                    }
                    else
                    {
                        return RedirectToAction("LeaveApplicationIndex", new { IsMySelf = "false" });
                    }
                }
                // Added by Rajas on 28 MARCH 2017 END

                PopulateDropDown();

                // Added by Rajas on 30 MARCH 2017 START
                string UpdateStatus = string.Empty;

                if (LeaveApplicationNecessaryContent(LeaveApplicationedit.EmployeeId, ref UpdateStatus) == false)
                {
                    AddAuditTrail("Error in Leave application edit due to " + UpdateStatus);

                    Error(UpdateStatus);

                    if (LeaveModelObj.MySelf == true)
                    {
                        return RedirectToAction("LeaveApplicationIndex");
                    }
                    else
                    {
                        return RedirectToAction("LeaveApplicationIndex", new { IsMySelf = "false" });
                    }
                }
                // Added by Rajas on 30 MARCH 2017 END

                LeaveModelObj.ActualDays = Convert.ToDouble(Session["AllowedLeaves"]);

                return View(LeaveModelObj);
            }
            catch (System.Exception ex)
            {
                PopulateDropDown();

                AddAuditTrail("Leave application edit failed due to " + ex.Message + ( ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                Error("Error in editing leave application");

                return View();
            }
        }

        /// <summary>
        /// Leave Application EDIT
        /// </summary>
        /// <param name="LeaveTypeObj"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        /// Updated by Rajas on 14 AUGUST 2017
        [HttpPost]
        public ActionResult LeaveApplicationEdit(LeaveModel LeaveTypeObj, FormCollection collection, HttpPostedFileBase MediCert)    //Modified By Shraddha on 17TH Oct 2016 for model validation
        {
            try
            {
                int EmpId = LeaveTypeObj.EmployeeId;

                // In case of self leave application
                if (EmpId <= 0)
                {
                    EmpId = Convert.ToInt32(Session["EmployeeNo"]);
                }

                //string SandwitchCaseDetected = collection["SandwitchCase"];

                double DeductDays = LeaveTypeObj.DayDeduct;

                string ErrorMessage = string.Empty;

               // bool ReturnStatus = true;

                if (ModelState.IsValid)
                {

                    // 1. CHECK prev date or post date
                    // 2. in prev date then already present // STOP                                                      

                    // 4. IS SUFFICIENT BALANCE
                    double LeaveAllowed = WetosEmployeeController.GetLeaveAllowed(WetosDB, EmpId, ref LeaveTypeObj, false);

                    double AppliedDays = CalculateAppliedDays(LeaveTypeObj.FromDate, LeaveTypeObj.ToDate, LeaveTypeObj.FromDayStatus, LeaveTypeObj.ToDayStatus);
                    LeaveTypeObj.AppliedDays = AppliedDays;

                    // 3. IS SC
                    // Added by Rajas on 21 JULY 2017
                    // Static IsSandwich function
                    bool IsLeaveCombine = false;    // Added by Rajas on 14 AUGUST 2017
                    IsSandwich(WetosDB, EmpId, ref ErrorMessage, ref LeaveTypeObj, ref IsLeaveCombine);

                    // IS LWP CASE 
                    if (AppliedDays > LeaveAllowed)
                    {
                        LeaveTypeObj.IsLWP = true;
                        LeaveTypeObj.LWP = LeaveAllowed - AppliedDays;
                        //System.TimeSpan diffResult = dtTodayNoon - dtYestMidnight;
                    }

                    // 5. Current Balance (CF + CREDIT - SANCTIONED)

                    // 6. Current Balance with pending (CF + CREDIT - SANCTIONED)

                    // 7. Generete Message


                    // 
                    LeaveApplication LeaveApplicationObj = WetosDB.LeaveApplications.Where(a => a.LeaveApplicationId == LeaveTypeObj.LeaveApplicationId).FirstOrDefault();

                    LeaveApplication LeaveApplicationObjEDIT = WetosDB.LeaveApplications.Where(a => a.LeaveApplicationId == LeaveTypeObj.LeaveApplicationId).FirstOrDefault();

                    // ADD TO DATABASE 

                    if (LeaveApplicationObj == null)
                    {
                        LeaveApplicationObj = new LeaveApplication();
                    }

                    LeaveApplicationObj.CompanyId = LeaveTypeObj.CompanyId;
                    LeaveApplicationObj.BranchId = LeaveTypeObj.Branchid;

                    /// Below code change done due to Employee id during saving to db was same as login id and not as selected employee id in case of other's application
                    LeaveApplicationObj.EmployeeId = EmpId;

                    LeaveApplicationObj.FromDate = Convert.ToDateTime(LeaveTypeObj.FromDate);
                    LeaveApplicationObj.ToDate = Convert.ToDateTime(LeaveTypeObj.ToDate);

                    //LeaveApplicationObj.LeaveTypeId = LeaveTypeObj.LeaveType;
                    LeaveApplicationObj.Purpose = LeaveTypeObj.Purpose;
                    //LeaveApplicationObj.LeaveType = LeaveTypeObj.LeaveName; // MODIFIED BY MSJ on 22 JULY 2017 //COMMENTED EARLIER CODE BY SHRADDHA  ON 29 NOV 2017 TO HANDLE EXCEPTION INCASE OF HAVING LEAVENAME OF MORE THAN 10 CHARACTERS
                    LeaveApplicationObj.LeaveType = LeaveTypeObj.LeaveCode;//ADDED NEW CODE BY SHRADDHA ON 29 NOV 2017 TO HANDLE EXCEPTION INCASE OF HAVING LEAVENAME OF MORE THAN 10 CHARACTERS
                    LeaveApplicationObj.ActualDays = LeaveTypeObj.ActualDays;

                    //Added by shraddha on 19th Oct 2016 for Saving Applied Days Start
                    LeaveApplicationObj.AppliedDays = AppliedDays; // LeaveTypeObj.AppliedDays;
                    LeaveApplicationObj.StatusId = LeaveTypeObj.StatusId;

                    string Status = WetosDB.DropdownDatas.Where(a => a.TypeId == 3 && a.Value == LeaveTypeObj.StatusId).Select(a => a.Text).FirstOrDefault();
                    LeaveApplicationObj.Status = Status;

                    LeaveApplicationObj.FromDayStatus = LeaveTypeObj.FromDayStatus;
                    LeaveApplicationObj.ToDayStatus = LeaveTypeObj.ToDayStatus;

                    // ADDED LATER BY MSJ 
                    LeaveApplicationObj.TotalDeductableDays = LeaveTypeObj.TotalDeductableDays;

                    LeaveApplicationObj.IsLWP = LeaveTypeObj.IsLWP;
                    LeaveApplicationObj.IsPreSandwichCase = LeaveTypeObj.IsPreSandwichCase;
                    LeaveApplicationObj.IsPostSandwichCase = LeaveTypeObj.IsPostSandwichCase;

                    LeaveApplicationObj.EffectiveDate = LeaveTypeObj.EffectiveDate; // CODE ADDED BY SHRADDHA ON 16 MAR 2018

                    LeaveApplicationObj.LeaveAddress = LeaveTypeObj.LeaveAddress;
                    LeaveApplicationObj.AltContactNo = LeaveTypeObj.AltContactNo;

                    // ADDED BY MSJ ON 09 JAN 2019 START
                    LeaveApplicationObj.AppliedBy = Convert.ToInt32(Session["EmployeeNo"]);
                    LeaveApplicationObj.AppliedOn = DateTime.Now;
                    LeaveApplicationObjEDIT.SanctionedBy = Convert.ToInt32(Session["EmployeeNo"]);
                    LeaveApplicationObjEDIT.SanctionedOn = DateTime.Now;

                    LeaveApplicationObjEDIT.AttendanceYearId = 1;
                    LeaveApplicationObjEDIT.FinancialYearId = 1;
                    // ADDED BY MSJ ON 09 JAN 2019 END


                    #region UPLOAD MEDICAL CERTIFICATE
                    // UPLOAD MEDICAL CERTIFICATE
                    if (MediCert != null && MediCert.ContentLength > 0)
                    {
                        Employee EmpObj = WetosDB.Employees.Where(a => a.EmployeeId == EmpId).FirstOrDefault();

                        try
                        {

                            // ADDED BY MSJ ON 20 JULY 2017 START
                            // GENERATE 
                            string DocContType = "Medical Certificate";

                            //EARLIER CODE COMMENTED AND ADDED NEW CODE BY SHRADDHA AS PER SUGGESTED BY MSJ ON TO HANDLE EMPTY MIDDLE NAME PROBLEM 30 OCT 2017 START
                            //string DocumentFileName = EmployeeObj.FirstName.Trim() + "_" + EmployeeObj.MiddleName.Trim() + "_"
                            //+ EmployeeObj.LastName.Trim() + "_" + DocContType + "_" + (DateTime.Now.ToString("yyyy_MM_dd_HHmmss"));

                            string DocumentFileName = (EmpObj.FirstName == string.Empty ? "" : EmpObj.FirstName.Trim()) + "_" + (EmpObj.MiddleName == string.Empty ? "" : EmpObj.MiddleName.Trim()) + "_"
                                + (EmpObj.LastName == string.Empty ? "" : EmpObj.LastName.Trim()) + "_" + DocContType + "_" + (DateTime.Now.ToString("yyyy_MM_dd_HHmmss"));

                            ////EARLIER CODE COMMENTED AND ADDED NEW CODE BY SHRADDHA AS PER SUGGESTED BY MSJ ON TO HANDLE EMPTY MIDDLE NAME PROBLEM 30 OCT 2017 END
                            DocumentFileName = DocumentFileName.Replace(' ', '_');
                            // ADDED BY MSJ ON 20 JULY 2017 END

                            string Attachment = DocumentFileName;

                            string path = Path.Combine(Server.MapPath("~/User_Data/Document_Upload"), Attachment);
                            string fileExtension = Path.GetExtension(MediCert.FileName);

                            MediCert.SaveAs(path + fileExtension);

                            LeaveApplicationObj.DocFolder = "Document_Upload";
                            LeaveApplicationObj.DocPath = path;
                            LeaveApplicationObj.DocFileName = Attachment + fileExtension;


                            ViewBag.Message = "File Processed Successfully";

                        }
                        catch (System.Exception ex)
                        {

                            AddAuditTrail("Unable to upload medical certificate for  " + EmpObj.EmployeeCode + ( ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                            return View();

                        }


                    }
                    #endregion

                    //COMMENTED BY PUSHKAR ON 9 SEPTEMBER 2017 AS NOW AUDITLOGS ARE SAVED FROM NEW MECHANISM
                    //#region ADD AUDIT LOG

                    ///// Generate Audit Log
                    ///// Added by Rajas on 30 AUGUST 2017
                    //WetosDB.SP_UpdateLeaveApplicationAuditLog(LeaveApplicationObj.LeaveApplicationId, LeaveApplicationObj.EmployeeId, LeaveApplicationObj.LeaveType
                    //    , LeaveApplicationObj.FromDate, LeaveApplicationObj.ToDate, LeaveApplicationObj.FromDayStatus
                    //    , LeaveApplicationObj.ToDayStatus, LeaveApplicationObj.AppliedDays, LeaveApplicationObj.ActualDays, LeaveApplicationObj.Purpose
                    //    , LeaveApplicationObj.Status, LeaveApplicationObj.BranchId, LeaveApplicationObj.CompanyId, Convert.ToInt32(Session["EmployeeNo"]));

                    //#endregion ADD AUDIT LOG

                    // ADD NEW LEAVE TO DATABASE 
                    // WetosDB.LeaveApplications.AddObject(LeaveApplicationObj);
                    WetosDB.SaveChanges();

                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 9 SEPTEMBER 2017
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
                    string Formname = "LEAVE APPLICATION";
                    //ACTION IS UPDATE
                    string Message = " ";

                    WetosAdministrationController.GenerateAuditLogsUpdate(WetosDB, LeaveApplicationObj.EmployeeId, Formname, Oldrecord,
                        Newrecord, ref Message);
                    #endregion
                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 9 SEPTEMBER 2017

                    // SEND ALERT
                    #region LEAVE APPLICATION NOTIFICATION

                    //FOR NOTIFICATION TO BE SENT TO REPORTING PERSON AS APPROVER

                    VwActiveEmployee EmployeeObj = WetosDB.VwActiveEmployees.Where(a => a.EmployeeId == EmpId).FirstOrDefault();

                    // FIRST NOTIFICATION
                    Notification NotificationObj = new Notification();
                    NotificationObj.FromID = EmployeeObj.EmployeeId;
                    NotificationObj.ToID = EmployeeObj.EmployeeReportingId;
                    NotificationObj.SendDate = DateTime.Now;
                    NotificationObj.NotificationContent = "Received Leave edited for approval from " + EmployeeObj.FirstName + " "
                        + EmployeeObj.LastName + " dated on " + LeaveTypeObj.FromDate.ToString("dd-MMM-yyyy") + " to " + LeaveTypeObj.ToDate.ToString("dd-MMM-yyyy");
                    NotificationObj.ReadFlag = false;
                    NotificationObj.SendDate = DateTime.Now;

                    // ADD TO NOTIFICATION REPORTING PERSON
                    WetosDB.Notifications.AddObject(NotificationObj);
                    WetosDB.SaveChanges();

                    //Check if both reporting person are are different
                    if (EmployeeObj.EmployeeReportingId != EmployeeObj.EmployeeReportingId2)
                    {
                        // FOR NOTIFICATION TO BE SENT TO REPORTING PERSON AS SANCTIONER
                        Notification NotificationObj3 = new Notification();
                        NotificationObj3.FromID = EmployeeObj.EmployeeId;
                        NotificationObj3.ToID = EmployeeObj.EmployeeReportingId;
                        NotificationObj3.SendDate = DateTime.Now;
                        NotificationObj3.NotificationContent = "Received Leave edited for sanctioning from " + EmployeeObj.FirstName + " " + EmployeeObj.LastName;
                        NotificationObj3.ReadFlag = false;
                        NotificationObj3.SendDate = DateTime.Now;

                        // ADD TO NOTIFICATION
                        WetosDB.Notifications.AddObject(NotificationObj3);
                        WetosDB.SaveChanges();
                    }

                    //FOR SELF NOTIFICATION

                    Notification NotificationObj2 = new Notification();
                    NotificationObj2.FromID = EmployeeObj.EmployeeId;
                    NotificationObj2.ToID = EmployeeObj.EmployeeId;
                    NotificationObj2.SendDate = DateTime.Now;
                    NotificationObj2.NotificationContent = "Leave edited successfully for " + LeaveTypeObj.FromDate.ToString("dd-MMM-yyyy")
                        + " to " + LeaveTypeObj.ToDate.ToString("dd-MMM-yyyy");
                    NotificationObj2.ReadFlag = false;
                    NotificationObj2.SendDate = DateTime.Now;

                    WetosDB.Notifications.AddObject(NotificationObj2);

                    WetosDB.SaveChanges();

                    //NOTIFICATION COUNT
                    int NoTiCount = WetosDB.Notifications.Where(a => a.ToID == EmpId && (a.ReadFlag == false || a.ReadFlag == null)).Count();
                    Session["NotificationCount"] = NoTiCount;

                    #endregion

                    // Code updated by Rajas on 25 JULY 2017
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
                                if (SendEmail(EmployeeObj.Email, NotificationObj.NotificationContent, NotificationObj.NotificationContent, ref EmailUpdateStatus, "Leave Application Edit") == false)
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

                    //Added by Rajas on 17 JAN 2017 START
                    // ADD TO AUDIT TRAIL
                    AddAuditTrail("Leave application successfully for " + LeaveTypeObj.FromDate.ToString("dd-MMM-yyyy")
                        + " to " + LeaveTypeObj.ToDate.ToString("dd-MMM-yyyy"));

                    // SUCCESS
                    Success("Leave application successfully for " + LeaveTypeObj.FromDate.ToString("dd-MMM-yyyy")
                        + " to " + LeaveTypeObj.ToDate.ToString("dd-MMM-yyyy"));

                    if (LeaveTypeObj.MySelf == true)
                    {
                        return RedirectToAction("LeaveApplicationIndex");
                    }
                    else
                    {
                        return RedirectToAction("LeaveApplicationIndex", new { IsMySelf = "false" });
                    }
                }

                return View(LeaveTypeObj);
            }
            catch (System.Exception ex)
            {
                //
                PopulateDropDown();

                // Added by Rajas on 30 MARCH 2017 START
                string UpdateStatus = string.Empty;

                if (LeaveApplicationNecessaryContent(LeaveTypeObj.EmployeeId, ref UpdateStatus) == false)
                {
                    AddAuditTrail("Error in Leave application due to " + UpdateStatus);

                    Error(UpdateStatus);

                    if (LeaveTypeObj.MySelf == true)
                    {
                        return RedirectToAction("LeaveApplicationIndex");
                    }
                    else
                    {
                        return RedirectToAction("LeaveApplicationIndex", new { IsMySelf = "false" });
                    }
                }
                // Added by Rajas on 30 MARCH 2017 END

                // Added by Rajas on 17 JAN 2017 START
                AddAuditTrail("Exception - New Leave application : " + LeaveTypeObj.LeaveName + " taken from " + LeaveTypeObj.FromDate.ToString("dd-MMM-yyyy")
                    + " to " + LeaveTypeObj.ToDate.ToString("dd-MMM-yyyy") + " failed due to " + ex.Message + ( ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                Error("Exception - New Leave application failed");
                // Added by Rajas on 17 JAN 2017 END

                return View(LeaveTypeObj);
            }
        }
        #endregion

        #region Delete Existing Leave Application

        /// <summary>
        /// LeaveApplicationDelete
        /// Updated by Rajas on 9 JUNE 2017
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult LeaveApplicationDelete(int id)
        {
            bool MySelf = true;
            try
            {

                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                LeaveApplication LeaveApplicationObj = WetosDB.LeaveApplications.Where(a => a.LeaveApplicationId == id).FirstOrDefault();

                if (LeaveApplicationObj != null)
                {
                    LeaveApplicationObj.MarkedAsDelete = 1;

                    // Added by Rajas on 9 JUNE 2017 START
                    LeaveApplicationObj.StatusId = 2;  // Cancel or Reject status used in case of delete

                    LeaveApplicationObj.Status = "Deleted";
                    // Added by Rajas on 9 JUNE 2017 END

                    // ADDED BY MSJ ON 09 JAN 2019 START
                    LeaveApplicationObj.CancelledBy = Convert.ToInt32(Session["EmployeeNo"]);
                    LeaveApplicationObj.CancelledOn = DateTime.Now;
                    // ADDED BY MSJ ON 09 JAN 2019 END

                    WetosDB.SaveChanges();


                    if (LeaveApplicationObj.EmployeeId == EmployeeId)
                    {
                        MySelf = true;
                    }
                    else
                    {
                        MySelf = false;
                    }

                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 12 SEPTEMBER 2017
                    #region ADD AUDIT LOG
                    //OLD RECORD IS BLANK
                    string Oldrecord = "LeaveType : " + LeaveApplicationObj.LeaveType + ", FromDate : " + LeaveApplicationObj.FromDate
                        + ", ToDate : " + LeaveApplicationObj.ToDate + ", FromDayStatus : " + LeaveApplicationObj.FromDayStatus
                        + ", ToDayStatus :" + LeaveApplicationObj.ToDayStatus + ", AppliedDays :" + LeaveApplicationObj.AppliedDays + ", ActualDays :"
                        + LeaveApplicationObj.ActualDays + ", Purpose :" + LeaveApplicationObj.Purpose + ", Status :" + LeaveApplicationObj.Status
                        + ", BranchId :" + LeaveApplicationObj.BranchId + ", CompanyId :" + LeaveApplicationObj.CompanyId;

                    //LOGIN ID IS TAKEN FROM SESSION PERSISTER
                    string Formname = "LEAVE DELETE";
                    //ACTION IS UPDATE
                    string Message = " ";

                    WetosAdministrationController.GenerateAuditLogsInsert(WetosDB, LeaveApplicationObj.EmployeeId, Formname, Oldrecord, ref Message);
                    #endregion
                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 12 SEPTEMBER 2017


                    Success("LeaveApplication " + LeaveApplicationObj.LeaveType + " applied between  " + LeaveApplicationObj.FromDate.ToString("dd-MMM-yyyy")
                        + " to " + LeaveApplicationObj.ToDate.ToString("dd-MMM-yyyy") + " deleted successfully");

                    AddAuditTrail("LeaveApplication " + LeaveApplicationObj.LeaveType + " applied between  " + LeaveApplicationObj.FromDate.ToString("dd-MMM-yyyy")
                        + " to " + LeaveApplicationObj.ToDate.ToString("dd-MMM-yyyy") + " deleted successfully");
                }

                if (MySelf == true)
                {
                    return RedirectToAction("LeaveApplicationIndex");
                }
                else
                {
                    return RedirectToAction("LeaveApplicationIndex", new { IsMySelf = "false" });
                }
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + ( ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                Error("Please try again!");

                if (MySelf == true)
                {
                    return RedirectToAction("LeaveApplicationIndex");
                }
                else
                {
                    return RedirectToAction("LeaveApplicationIndex", new { IsMySelf = "false" });
                }
            }
        }
        #endregion

        #region Calculate No. Of days for which leave is applied
        /// <summary>
        /// ADDED BY SHRADDHA ON 19 JULY 2017
        /// TO MAKE GENERIC FUNCTION TO CALCULATE APPLIED DAYS
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="FromDayStatus"></param>
        /// <param name="ToDayStatus"></param>
        /// <returns></returns>
        public static double CalculateAppliedDays(DateTime StartDate, DateTime EndDate, int? FromDayStatus, int? ToDayStatus)
        {

            TimeSpan ts = (Convert.ToDateTime(EndDate)) - (Convert.ToDateTime(StartDate));
            float AppliedDay = ts.Days;
            if (FromDayStatus == 1 && ToDayStatus == 1)
            {
                AppliedDay = AppliedDay + 1;
            }

                //Modified By Shraddha on 30 DEC 2016 for leave application validation changes start
            else if ((FromDayStatus == 2) && (ToDayStatus == 2) && StartDate == EndDate)
            {
                AppliedDay = 0.5F;
            }
            else if ((FromDayStatus == 3) && (ToDayStatus == 3) && StartDate == EndDate)
            {
                AppliedDay = 0.5F;
            }
            else if ((FromDayStatus == 2) && (ToDayStatus == 3) && StartDate == EndDate)
            {
                AppliedDay = 1F;
            }

            //Modified By Shraddha on 30 DEC 2016 for leave application validation changes End
            else if ((FromDayStatus == 2 || FromDayStatus == 3) && (ToDayStatus == 2 || ToDayStatus == 3) && StartDate != EndDate)
            {
                AppliedDay = AppliedDay + 0;
            }

            else if (FromDayStatus == 2 || ToDayStatus == 2 || FromDayStatus == 3 || ToDayStatus == 3)
            {
                AppliedDay = (float)(AppliedDay + 0.5);
            }

            return AppliedDay;
        }

        #endregion

        #region Calculate No. Of days from View
        /// <summary>
        /// ADDED BY SHRADDHA ON 19 JULY 2017
        /// TO MAKE GENERIC FUNCTION TO CALCULATE APPLIED DAYS
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="FromDayStatus"></param>
        /// <param name="ToDayStatus"></param>
        [HttpGet]
        public double CalculateAppliedDaysNew(string StartDate, string EndDate, int? FromDayStatus, int? ToDayStatus)
        {

            TimeSpan ts = (Convert.ToDateTime(EndDate)) - (Convert.ToDateTime(StartDate));
            float AppliedDay = ts.Days;
            if (FromDayStatus == 1 && ToDayStatus == 1)
            {
                AppliedDay = AppliedDay + 1;
            }

                //Modified By Shraddha on 30 DEC 2016 for leave application validation changes start
            else if ((FromDayStatus == 2) && (ToDayStatus == 2) && StartDate == EndDate)
            {
                AppliedDay = 0.5F;
            }
            else if ((FromDayStatus == 3) && (ToDayStatus == 3) && StartDate == EndDate)
            {
                AppliedDay = 0.5F;
            }
            else if ((FromDayStatus == 2) && (ToDayStatus == 3) && StartDate == EndDate)
            {
                AppliedDay = 1F;
            }

            //Modified By Shraddha on 30 DEC 2016 for leave application validation changes End
            else if ((FromDayStatus == 2 || FromDayStatus == 3) && (ToDayStatus == 2 || ToDayStatus == 3) && StartDate != EndDate)
            {
                AppliedDay = AppliedDay + 0;
            }

            else if (FromDayStatus == 2 || ToDayStatus == 2 || FromDayStatus == 3 || ToDayStatus == 3)
            {
                AppliedDay = (float)(AppliedDay + 0.5);
            }

            return AppliedDay;
        }

        #endregion

        #region Code to check whether sandwich case is available for applied leave date
        /// <summary>
        /// 
        /// </summary>
        /// <param name="WetosDB"></param>
        /// <param name="LeaveTypeObj"></param>
        /// <param name="collection"></param>
        /// <param name="EmpId"></param>
        /// <param name="ErrorMessage"></param>
        /// <returns></returns>
        /// Updated by Rajas on 14 AUGUST 2017
        public static bool IsSandwich(WetosDBEntities WetosDB, int EmpId, ref string ErrorMessage, ref LeaveModel LeaveTypeObj, ref bool IsLeaveCombine)
        {
            bool ReturnStatus = false;
            try
            {
                Employee SelectedEmp = WetosDB.Employees.Where(a => a.EmployeeId == EmpId).FirstOrDefault();

                int EmployeeGroupId = WetosDB.EmployeeGroupDetails.Where(a => a.Employee.EmployeeId == EmpId).Select(a => a.EmployeeGroup.EmployeeGroupId).FirstOrDefault();

                int LeaveTypeInt = LeaveTypeObj.LeaveId;

                string LeaveType = WetosDB.LeaveMasters.Where(a => a.LeaveId == LeaveTypeInt).Select(a => a.LeaveCode).FirstOrDefault();

                LeaveMaster LeaveMasterObj = WetosDB.LeaveMasters.Where(a => a.LeaveCode == LeaveType && a.EmployeeGroup.EmployeeGroupId == EmployeeGroupId).FirstOrDefault();

                string AppliedLeaveType = string.Empty;
                //CODE ADDED BY SHRADDHA ON 28 NOV 2017 START
                DateTime CombinationCaseLeaveDate = DateTime.Now;
                //CODE ADDED BY SHRADDHA ON 28 NOV 2017 END


                //GENERIC FUNCTION USED BY SHRADDHA ON 20 JULY 2017
                double AppliedDay = CalculateAppliedDays(LeaveTypeObj.FromDate, LeaveTypeObj.ToDate, LeaveTypeObj.FromDayStatus, LeaveTypeObj.ToDayStatus);

                //string SandwitchCaseDetected = collection["SandwitchCase"];

                //--------------------------------------------------------------------------------------------------------------------------------------
                // 2. IS PRVE DATE SANDWITCH CASE
                //--------------------------------------------------------------------------------------------------------------------------------------
                #region PRESANDWITH
                //  NEW LOGIC START

                //first date
                DateTime TempCurrent = LeaveTypeObj.FromDate;
                DateTime TempPrev = TempCurrent;

                bool IsAppDayOkay = false;
                bool ContinueFlag = false;
                bool IsHoliday = false;
                bool IsWO = false;

                int SandwichDays = 0;

                for (int pi = 0; pi < 30; pi--)
                {
                    TempCurrent = TempPrev;
                    TempPrev = TempPrev.AddDays(-1);

                    if (LeaveMasterObj.HHBetLevConsiderLeave == true)
                    {
                        // HOLODAY
                        HoliDay PrevHoliDay = WetosDB.HoliDays.Where(a => a.FromDate == TempPrev).FirstOrDefault();

                        ContinueFlag = false;

                        if (PrevHoliDay != null)
                        {

                            ContinueFlag = true;
                            IsHoliday = true;
                            SandwichDays++;

                        }
                    }

                    if (LeaveMasterObj.WOBetLevConsiderLeave == true)
                    {
                        // WEEKOFF 1
                        if (ContinueFlag == false && TempPrev.DayOfWeek.ToString().ToUpper() == SelectedEmp.WeeklyOff1)
                        {
                            ContinueFlag = true;
                            IsWO = true;
                            SandwichDays++;

                        }

                        // Validate Weekoff2 as per assigned to employee
                        if (ContinueFlag == false && TempPrev.DayOfWeek.ToString().ToUpper() == SelectedEmp.WeeklyOff2)
                        {
                            if (!string.IsNullOrEmpty(SelectedEmp.WeeklyOff2))
                            {
                                int WeekNumber = GetWeekOfMonth(TempPrev);

                                if (SelectedEmp.First == true && WeekNumber == 1)
                                {
                                    ContinueFlag = true;
                                    IsWO = true;
                                    SandwichDays++;
                                }
                                else if (SelectedEmp.Second == true && WeekNumber == 2)
                                {
                                    ContinueFlag = true;
                                    IsWO = true;
                                    SandwichDays++;
                                }
                                else if (SelectedEmp.Third == true && WeekNumber == 3)
                                {
                                    ContinueFlag = true;
                                    IsWO = true;
                                    SandwichDays++;
                                }
                                else if (SelectedEmp.Fourth == true && WeekNumber == 4)
                                {
                                    ContinueFlag = true;
                                    IsWO = true;
                                    SandwichDays++;
                                }
                                else if (SelectedEmp.Fifth == true && WeekNumber == 5)
                                {
                                    ContinueFlag = true;
                                    IsWO = true;
                                    SandwichDays++;
                                }
                            }
                        }

                    }

                    if (!ContinueFlag)
                    {
                        //MODIFIED BY PUSHKAR ON 24 MAY 2018 FOR ALL STATUS 3-REJ BY APP / 5- CANCELLED / 6- REJ BY SANC
                        List<LeaveApplication> LeaveApplicationABCD = WetosDB.LeaveApplications.Where(a => a.EmployeeId == SelectedEmp.EmployeeId
                             && a.FromDate <= TempPrev && a.ToDate >= TempPrev && a.MarkedAsDelete == 0 && a.StatusId != 3 && a.StatusId != 6 && a.StatusId != 5).ToList();

                        if (LeaveApplicationABCD.Count <= 0)
                        {
                            IsAppDayOkay = true;
                        }
                        else
                        {
                            foreach (var Leaves in LeaveApplicationABCD)
                            {
                                if (Leaves.FromDayStatus == 1 && LeaveTypeObj.FromDayStatus == 3)
                                {
                                    IsAppDayOkay = true;
                                }
                                else if (Leaves.FromDayStatus == 2 && LeaveTypeObj.FromDayStatus == 1)
                                {
                                    IsAppDayOkay = true;
                                }
                                else if (Leaves.FromDayStatus == 3 && LeaveTypeObj.FromDayStatus == 3)
                                {
                                    IsAppDayOkay = true;
                                }
                                else if (Leaves.FromDayStatus == 2 && LeaveTypeObj.FromDayStatus == 2)
                                {
                                    IsAppDayOkay = true;
                                }
                                else
                                {
                                    IsAppDayOkay = false;
                                    AppliedLeaveType = Leaves.LeaveType.Trim();

                                    //CODE ADDED BY SHRADDHA ON 28 NOV 2017 START
                                    CombinationCaseLeaveDate = Leaves.ToDate;
                                    //CODE ADDED BY SHRADDHA ON 28 NOV 2017 END
                                }

                            }
                        }

                        break;
                    }
                }

                //if (SandwitchCaseDetected != "true")
                {
                    if (!IsAppDayOkay && (IsHoliday == true || IsWO == true))
                    {
                        LeaveTypeObj.IsPreSandwichCase = true;


                        //MODIFIED BY MSJ ON 24 JULY 2017 
                        LeaveTypeObj.PreSandwitchDays = SandwichDays; // ((LeaveTypeObj.FromDate.AddDays(-1)) - (TempPrev.AddDays(1))).TotalDays;

                        LeaveTypeObj.TotalDeductableDays = AppliedDay + LeaveTypeObj.PreSandwitchDays;

                        ErrorMessage = "Sandwitch case detected in Start date - Total " + "<b>" + LeaveTypeObj.TotalDeductableDays + "</b>" + " Days would be consider as Leave." + "<br/>"
                            + "Do you really want to Continue?";


                        //LeaveTypeObj.PreSandwichStartDate = TempPrev.AddDays(1);
                        //LeaveTypeObj.PreSandwichEndDate = LeaveTypeObj.FromDate.AddDays(-1);

                        //LeaveTypeObj


                        //return false;
                    }
                }
                #endregion

                #region CHECK LEAVE COMBINATION IS ALLOWED OR NOT


                string PrefixComb = LeaveMasterObj.Prefix;
                int Pre = 1;

                // Added by Rajas on 14 AUGUST 2017 START
                string Message = string.Empty;
                bool IsSuccess = false;
                //if (IsLeaveCombination(LeaveMasterObj, AppliedLeaveType, IsAppDayOkay, ref Message, ref IsSuccess) == false) //COMMENTED CODE BY SHRADDHA ON 28 NOV 2017
                if (IsLeaveCombination(WetosDB, LeaveMasterObj, LeaveTypeObj, CombinationCaseLeaveDate, AppliedLeaveType, IsAppDayOkay, PrefixComb, Pre, ref Message, ref IsSuccess) == false)  //ADDED CODE BY SHRADDHA ON 28 NOV 2017
                {
                    IsLeaveCombine = true;
                    ErrorMessage = Message;
                    return ReturnStatus = false;
                }
                // Added by Rajas on 14 AUGUST 2017 END

                #endregion

                //------------------------------------------------------------------------------------------------------------------------------
                // 3.IS NEXT DATE SANDWITCH CASE
                //------------------------------------------------------------------------------------------------------------------------------
                #region POST SANDWITCH
                // last day
                TempCurrent = LeaveTypeObj.ToDate;
                DateTime TempNext = TempCurrent;

                IsAppDayOkay = false;
                ContinueFlag = false;
                IsHoliday = false;
                IsWO = false;

                for (int pi = 0; pi < 30; pi++)
                {
                    TempCurrent = TempPrev;
                    TempNext = TempNext.AddDays(+1);

                    // Validate HHBetLevConsiderLeave rule assigned in leave rule
                    if (LeaveMasterObj.HHBetLevConsiderLeave == true)
                    {
                        // HOLODAY
                        HoliDay PrevHoliDay = WetosDB.HoliDays.Where(a => a.FromDate == TempNext).FirstOrDefault();

                        ContinueFlag = false;

                        if (PrevHoliDay != null)
                        {
                            ContinueFlag = true;
                            IsHoliday = true;

                            // If Sandwich already detected then Next day condition checking should be skipped
                            // Discussion with Katre sir on 4 JULY 2017
                            // Updated by Rajas on 4 JULY 2017
                            //if (SandwitchCaseDetected != "true")
                            {
                                SandwichDays++;
                            }
                        }
                    }


                    // Validate WOBetLevConsiderLeave rule assigned in leave rule
                    if (LeaveMasterObj.WOBetLevConsiderLeave == true)
                    {
                        // WEEKOFF 1
                        if (ContinueFlag == false && TempNext.DayOfWeek.ToString().ToUpper() == SelectedEmp.WeeklyOff1)
                        {
                            ContinueFlag = true;
                            IsWO = true;
                            SandwichDays++;
                        }

                        // Validate Weekoff2 as per assigned to employee
                        if (ContinueFlag == false && TempNext.DayOfWeek.ToString().ToUpper() == SelectedEmp.WeeklyOff2)
                        {
                            if (!string.IsNullOrEmpty(SelectedEmp.WeeklyOff2))
                            {
                                int WeekNumber = GetWeekOfMonth(TempNext);

                                if (SelectedEmp.First == true && WeekNumber == 1)
                                {
                                    ContinueFlag = true;
                                    IsWO = true;
                                    SandwichDays++;
                                }
                                else if (SelectedEmp.Second == true && WeekNumber == 2)
                                {
                                    ContinueFlag = true;
                                    IsWO = true;
                                    SandwichDays++;
                                }
                                else if (SelectedEmp.Third == true && WeekNumber == 3)
                                {
                                    ContinueFlag = true;
                                    IsWO = true;
                                    SandwichDays++;
                                }
                                else if (SelectedEmp.Fourth == true && WeekNumber == 4)
                                {
                                    ContinueFlag = true;
                                    IsWO = true;
                                    SandwichDays++;
                                }
                                else if (SelectedEmp.Fifth == true && WeekNumber == 5)
                                {
                                    ContinueFlag = true;
                                    IsWO = true;
                                    SandwichDays++;
                                }
                            }
                        }

                    }

                    if (!ContinueFlag)
                    {
                        //MODIFIED BY PUSHKAR ON 24 MAY 2018 FOR ALL STATUS 3-REJ BY APP / 5- CANCELLED / 6- REJ BY SANC
                        List<LeaveApplication> LeaveApplicationList = WetosDB.LeaveApplications.Where(a => a.EmployeeId == SelectedEmp.EmployeeId && a.FromDate <= TempNext
                            && a.ToDate >= TempNext && a.MarkedAsDelete == 0 && a.StatusId != 3 && a.MarkedAsDelete == 0 && a.StatusId != 5 && a.StatusId != 6).ToList();
                        if (LeaveApplicationList.Count == 0)
                        {
                            IsAppDayOkay = true;
                        }
                        else
                        {
                            foreach (var Leaves in LeaveApplicationList)
                            {
                                if (Leaves.ToDayStatus == 3 && LeaveTypeObj.ToDayStatus == 3)
                                {
                                    IsAppDayOkay = true;
                                }
                                else if (Leaves.ToDayStatus == 1 && LeaveTypeObj.ToDayStatus == 2)
                                {
                                    IsAppDayOkay = true;
                                }
                                else if (Leaves.ToDayStatus == 2 && LeaveTypeObj.FromDayStatus == 2)   // Updated by Rajas on 21 JULY 2017
                                {
                                    IsAppDayOkay = true;
                                }
                                else
                                {
                                    IsAppDayOkay = false;
                                    AppliedLeaveType = Leaves.LeaveType.Trim();


                                    //CODE ADDED BY SHRADDHA ON 28 NOV 2017 START
                                    CombinationCaseLeaveDate = Leaves.FromDate;
                                    //CODE ADDED BY SHRADDHA ON 28 NOV 2017 END
                                }

                            }
                        }

                        break;
                    }
                }

                //if (SandwitchCaseDetected != "true")
                {
                    if (!IsAppDayOkay && (IsHoliday == true || IsWO == true))
                    {
                        // ADDED BY MSJ ON 19 JULY 2017 START
                        LeaveTypeObj.IsPostSandwichCase = true;

                        // MODIFIED BY MSJ ON 24 JULY 2017 START
                        LeaveTypeObj.PostSandwitchDays = SandwichDays;

                        LeaveTypeObj.TotalDeductableDays = AppliedDay + LeaveTypeObj.PostSandwitchDays;

                        ErrorMessage = "Sandwitch case detected in End date - Total " + "<b>" + LeaveTypeObj.TotalDeductableDays + "</b>" + " Days would be consider as Leave." + "<br/>"
                            + "Do you really want to Continue?";


                        //((TempNext.AddDays(-1)) - (LeaveTypeObj.ToDate.AddDays(1))).TotalDays;
                        //LeaveTypeObj.PostSandwichStartDate = LeaveTypeObj.ToDate.AddDays(+1);
                        //LeaveTypeObj.PostSandwichEndDate = TempNext.AddDays(-1);
                        // ADDED BY MSJ ON 19 JULY 2017 END

                        //return false;
                    }
                }

                #endregion


                #region CHECK LEAVE COMBINATION IS ALLOWED OR NOT


                string SuffixComb = LeaveMasterObj.Suffix;
                int Suff = 2;

                // Added by Rajas on 14 AUGUST 2017 START
                Message = string.Empty;
                IsSuccess = false;
                //if (IsLeaveCombination(LeaveMasterObj, AppliedLeaveType, IsAppDayOkay, ref Message, ref IsSuccess) == false) //COMMENTED CODE BY SHRADDHA ON 28 NOV 2017
                if (IsLeaveCombination(WetosDB, LeaveMasterObj, LeaveTypeObj, CombinationCaseLeaveDate, AppliedLeaveType, IsAppDayOkay, SuffixComb, Suff, ref Message, ref IsSuccess) == false)  //ADDED CODE BY SHRADDHA ON 28 NOV 2017
                {
                    IsLeaveCombine = true;
                    ErrorMessage = Message;
                    return ReturnStatus = false;
                }
                // Added by Rajas on 14 AUGUST 2017 END

                #endregion


                #region IS WEEK OFF IN DATERANGE

                // ADDED BY MSJ on  29 APRIL 2018
                bool IsSandwitchCaseInDateRange = false;
                double SandwitchCaseInDateRangeDays = 0.0;
                double SandwitchCaseInDateRangeDaysHol = 0.0;
                //if (SandwitchCaseDetected != "true")
                {
                    #region SANDWICH CASE BETWEEN SELECTED FROM AND TO DATE

                    if (LeaveTypeObj.FromDate != LeaveTypeObj.ToDate)
                    {
                        TempCurrent = LeaveTypeObj.FromDate;
                        TempNext = TempCurrent;

                        IsAppDayOkay = false;

                        int IsSandwich = 0;
                        int IsHolidayDeduct = 0;

                        for (int pi = 1; pi <= AppliedDay; pi++)
                        {
                            TempNext = TempNext.AddDays(+1);

                            if (TempNext > LeaveTypeObj.ToDate)
                            {
                                break;
                            }

                            ContinueFlag = false;

                            // HOLODAY
                            HoliDay PrevHoliDay = WetosDB.HoliDays.Where(a => a.FromDate == TempNext && a.Branchid == SelectedEmp.BranchId).FirstOrDefault();


                            if (PrevHoliDay != null)
                            {
                                ContinueFlag = true;
                                IsHolidayDeduct++;

                            }
                            // WEEKOFF 1
                            if (ContinueFlag == false && TempNext.DayOfWeek.ToString().ToUpper() == SelectedEmp.WeeklyOff1)
                            {
                                ContinueFlag = true;
                                IsSandwich++;

                            }
                            // WeekOff 2 (2nd forth)
                            if (ContinueFlag == false && TempNext.DayOfWeek.ToString().ToUpper() == SelectedEmp.WeeklyOff2)
                            {
                                int WeekNumber = GetWeekOfMonth(TempNext);

                                if (SelectedEmp.First == true && WeekNumber == 1)
                                {
                                    ContinueFlag = true;
                                    IsSandwich++;
                                }
                                else if (SelectedEmp.Second == true && WeekNumber == 2)
                                {
                                    ContinueFlag = true;
                                    IsSandwich++;
                                }
                                else if (SelectedEmp.Third == true && WeekNumber == 3)
                                {
                                    ContinueFlag = true;
                                    IsSandwich++;
                                }
                                else if (SelectedEmp.Fourth == true && WeekNumber == 4)
                                {
                                    ContinueFlag = true;
                                    IsSandwich++;
                                }
                                else if (SelectedEmp.Fifth == true && WeekNumber == 5)
                                {
                                    ContinueFlag = true;
                                    IsSandwich++;
                                }
                            }

                        }

                        if (!ContinueFlag && IsSandwich == 0 && IsHolidayDeduct == 0)
                        {
                            if (WetosDB.LeaveApplications.Where(a => a.EmployeeId == SelectedEmp.EmployeeId && a.FromDate >= TempNext
                                && a.ToDate <= TempNext).ToList().Count == 0)
                            {
                                IsAppDayOkay = true;
                            }

                            //break;
                        }


                        if (!IsAppDayOkay)
                        {
                            IsSandwitchCaseInDateRange = true; // ADDED BY MSJ ON 24 JULY 2017 

                            // ERROR
                            ErrorMessage = "Sandwitch case detected - You Can not Apply Leave For " + LeaveTypeObj.FromDate.ToString("dd-MMM-yyyy")
                                              + " To " + LeaveTypeObj.ToDate.ToString("dd-MMM-yyyy") + " Range";

                            // ADDED BY MSJ ON 24 JULY 2017
                            SandwitchCaseInDateRangeDays = IsSandwich;
                            SandwitchCaseInDateRangeDaysHol = IsHolidayDeduct;

                            if (LeaveMasterObj.WOBetLevConsiderLeave == false)
                            {
                                AppliedDay = AppliedDay - SandwitchCaseInDateRangeDays;
                            }

                            //ADDED BY PUSHKAR ON 25 MAY 2018 FOR ISHOLIDAY DEDUCT LEAVE RULE

                            if (LeaveMasterObj.HHBetLevConsiderLeave == false)
                            {
                                AppliedDay = AppliedDay - SandwitchCaseInDateRangeDaysHol;
                            }
                        }
                    }

                    #endregion
                }

                // TOTALS DAYS

                #endregion COMMENTED CODE

                LeaveTypeObj.TotalDeductableDays = AppliedDay + LeaveTypeObj.PostSandwitchDays + LeaveTypeObj.PreSandwitchDays;// +SandwitchCaseInDateRangeDays; // MODIFIED BY MSJ ON 24 JULY 2017

                if (LeaveTypeObj.IsPreSandwichCase || LeaveTypeObj.IsPostSandwichCase)// || IsSandwitchCaseInDateRange) // ADDED IsSandwitchCaseInDateRange
                {
                    ReturnStatus = false;
                }
                else
                {
                    ReturnStatus = true;
                }

                return ReturnStatus;
            }
            catch
            {
                return ReturnStatus;
            }
        }

        #endregion

        #region Leave Application Necessary content
        /// <summary>
        /// ADDED BY SHRADDHA ON 29 DEC 2016 TO CREATE FUNCTION FOR LOGIC USED FOR LEAVE APPLICATION TO REDUCE EFFORTS
        /// Updated by Rajas on 11 MARCH 2017
        /// </summary>
        public bool LeaveApplicationNecessaryContent(int EmpId, ref string UpdateStatus)
        {
            bool ReturnStatus = false;

            try
            {
                //Modified by Shraddha on 07 FEB 2016 for getting Leave type dropdown data based on EMPLOYEE TYPE, EMPLOYEE COMPANY, EMPLOYEE BRANCH Start
                Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == EmpId).FirstOrDefault();

                // Added by Rajas on 23 FEB 2017 generate list for available leaves as per employee group assigned
                EmployeeGroupDetail EmployeeGroupDetailObj = WetosDB.EmployeeGroupDetails.Where(a => a.Employee.EmployeeId == EmpId).FirstOrDefault();

                if (EmployeeObj != null && EmployeeGroupDetailObj != null)  // Updated by Rajas on 23 FEB 2017, added && EmployeeGroupDetailObj != null
                {

                    if (EmployeeObj.Gender.ToUpper() == "F")
                    {
                        //MODIFIED BY PUSHKAR ON 28 NOV 2017 FOR CHECKING MARK AS DELETE CONDITION
                        var LeaveMasterList = WetosDB.LeaveMasters.Where(a => a.Company.CompanyId == EmployeeObj.CompanyId && a.BranchId == EmployeeObj.BranchId
                            && a.EmployeeGroup.EmployeeGroupId == EmployeeGroupDetailObj.EmployeeGroup.EmployeeGroupId
                            && (a.MarkedAsDelete == 0 || a.MarkedAsDelete == null))
                            .Select(a => new { LeaveTypeID = a.LeaveId, LeaveType = a.LeaveCode }).ToList();

                        ViewBag.LeaveTypeList = new SelectList(LeaveMasterList, "LeaveTypeID", "LeaveType").ToList();

                        string LeaveType = LeaveMasterList.Select(a => a.LeaveType).FirstOrDefault();
                    }
                    else
                    {
                        //MODIFIED BY PUSHKAR ON 28 NOV 2017 FOR CHECKING MARK AS DELETE CONDITION
                        var LeaveMasterList = WetosDB.LeaveMasters.Where(a => a.Company.CompanyId == EmployeeObj.CompanyId && a.BranchId == EmployeeObj.BranchId
                            && a.EmployeeGroup.EmployeeGroupId == EmployeeGroupDetailObj.EmployeeGroup.EmployeeGroupId && a.ApplicableToMaleFemale.ToUpper() == "B"
                            && (a.MarkedAsDelete == 0 || a.MarkedAsDelete == null))
                            .Select(a => new { LeaveTypeID = a.LeaveId, LeaveType = a.LeaveCode }).ToList();

                        ViewBag.LeaveTypeList = new SelectList(LeaveMasterList, "LeaveTypeID", "LeaveType").ToList();

                        string LeaveType = LeaveMasterList.Select(a => a.LeaveType).FirstOrDefault();
                    }


                    // Added by Rajas on 9 MARCH 2017 for Displaying error message if rules are not set to employee group
                    //if (LeaveMasterList.Count <= 0)
                    //{
                    //    ModelState.AddModelError("", " Please create leave rules for the particular Employee group");
                    //}

                    //List<LeaveBalance> LeaveBalanceList = WetosDB.LeaveBalances.Where(a => a.EmployeeId == EmpId).ToList();

                    // Added by Rajas on 13 APRIL 2017
                    List<SP_LeaveTableData_Result> LeaveBalanceList = WetosDB.SP_LeaveTableData(EmpId).ToList();

                    //ADDED BY SAHRADDHA ON 29 DEC 2016
                    // Updated by Rajas on 7 APRIL 2017 for displaying only Sanctioned or Rejected leaves
                    #region CODE ADDED BY SHRADDHA ON 29 MAR 2018

                    string YearInfo = Session["CurrentFinancialYear"].ToString();
                    DateTime FromDate = new DateTime(DateTime.Now.Year, 1, 1);
                    DateTime ToDate = new DateTime((DateTime.Now.Year + 1), 1, 1);



                    try
                    {
                        if (string.IsNullOrEmpty(YearInfo))
                        {
                            try
                            {
                                //FromDate = WetosDB.FinancialYears.Where(a => a.FinancialName == YearInfo).Select(a => a.StartDate).FirstOrDefault();
                                //ToDate = WetosDB.FinancialYears.Where(a => a.FinancialName == YearInfo).Select(a => a.EndDate).FirstOrDefault();

                                FinancialYear TempCurrentFY = WetosDB.FinancialYears.Where(a => a.FinancialName == YearInfo).FirstOrDefault();
                                FinancialYear TempPreviousFY = WetosDB.FinancialYears.Where(a => a.FinancialYearId == TempCurrentFY.PrevFYId).FirstOrDefault();

                                //FinancialYear FY = TempPreviousFY == null ? TempCurrentFY : TempPreviousFY;

                                FromDate = TempPreviousFY == null ? TempPreviousFY.StartDate : TempCurrentFY.StartDate;
                                ToDate = TempCurrentFY.EndDate;
                            }
                            catch
                            {
                                FromDate = new DateTime(DateTime.Now.Year, 1, 1);
                                ToDate = new DateTime((DateTime.Now.Year + 1), 1, 1);
                            }
                        }
                    }
                    catch
                    {
                        FromDate = new DateTime(DateTime.Now.Year, 1, 1);
                        ToDate = new DateTime((DateTime.Now.Year + 1), 1, 1);
                    }
                    #endregion
                    List<SP_LeaveApplicationDetailsList_Result> LeaveApplicationDetailsList = WetosDB.SP_LeaveApplicationDetailsList(EmpId, FromDate, ToDate).ToList();

                    ViewBag.LeaveApplicationDetailsListVB = LeaveApplicationDetailsList;

                    LeaveApplicationDetailsList = WetosDB.SP_LeaveApplicationDetailsList(EmpId, FromDate, ToDate).Where(a => a.StatusId == 1).ToList();

                    //ViewBag.LeaveBalanceDetailsVB = WetosEmployeeController.GetLeavedataNewLogic(WetosDB, EmpId,DateTime.Now);

                    //Added by shraddha on 03 JUNE 2017 created generic function
                    ViewBag.LeaveBalanceDetailsVB = DashboardController.GetLeaveDetailsOnallControllers(WetosDB, EmpId, DateTime.Now); //FinalLeaveBalanceList;
                    // MODIFIED BY MSJ on 31 MAY 2017 END

                    //ViewBag.LeaveBalanceDetailsVB = LeaveBalanceList;
                    // Added by Rajas on 18 MARCH 2017 END

                    //Added By Shraddha on 19 JAN 2017 for showing Allowable Days End

                    LeaveCredit LeaveCreditObj = WetosDB.LeaveCredits.Where(a => a.EmployeeId == EmpId).FirstOrDefault();
                    ViewBag.LeaveCreditObjVB = LeaveCreditObj;
                }
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Exception during validating leave " + ex.Message + ( ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                UpdateStatus = "Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message);

                return ReturnStatus = false;
            }

            ReturnStatus = true;

            return ReturnStatus;

        }
        #endregion

        #region Code To Populate Dropdown for Leave
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

            ////Added by shraddha on 14th oct 2016 start

            //var EmployeeObj = WetosDB.Employees.Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.FirstName + " " + a.LastName }).ToList();

            //ViewBag.EmployeeList = new SelectList(EmployeeObj, "EmployeeId", "EmployeeName").ToList();


            //var EmployeeCodeObj = WetosDB.Employees.Select(a => new { EmployeeId = a.EmployeeId, EmployeeCode = a.EmployeeCode }).ToList();

            //ViewBag.EmployeeCodeList = new SelectList(EmployeeCodeObj, "EmployeeId", "EmployeeCode").ToList();

            // Added by Rajas on 22 OCT 2016 for Dropdown list for FULL DAY or HALF DAY Leave
            var LeaveTypeStatusObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 9).Select(a => new { LeaveStatusID = a.Value, LeaveStatus = a.Text }).ToList();
            ViewBag.LeaveTypeStatusList = new SelectList(LeaveTypeStatusObj, "LeaveStatusID", "LeaveStatus").ToList();

            //Added by Shraddha on 17 DEC 2016 for displaying employee code and name both in dropdown




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

            ////ADDED BY SHRADDHA ON 28 DEC 2016 FOR SELECT CRITERIA START
            //// 5 - Exception entry
            //List<string> SelectCriteriaObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 5).Select(a => a.Text).ToList();
            //ViewBag.SelectCriteriaList = SelectCriteriaObj;
            ////ADDED BY SHRADDHA ON 28 DEC 2016 FOR SELECT CRITERIA END

            var StatusObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 3).Select(a => new { Value = a.Value, Text = a.Text }).ToList();
            ViewBag.StatusList = new SelectList(StatusObj, "Value", "Text").ToList();
        }

        #endregion

        #region CODE TO GET WEEK NUMBER OF RESPECTIVE MONTH FOR SELECTED DATE
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

        #endregion

        #region Code to Send Email
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
            //string Contact1 = ConfigurationManager.AppSettings["Contact1"];
            //string Contact2 = ConfigurationManager.AppSettings["Contact2"];
            //string Support = ConfigurationManager.AppSettings["Support"];

            //String name = //collection.Get("name");
            //String email = //collection.Get("email");
            //string subject = Subject;// //collection.Get("subject");
            // string message = Message; ////collection.Get("message");

            //if (ModelState.IsValid)
            //{
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
                AddAuditTrail("Error in sending email in " + EmailFromWhichApplication + " : " + " due to " + ex.Message + ( ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                UpdateStatus = "Unable to send email!";

                return ReturnStatus;

                //return View("Error");
            }

            // SEND EMAIL END

            //return View();
        }
        #endregion

        #region Code to check whether Leave Combination case available for Applied leave date
        /// <summary>
        /// IsLeaveCombination
        /// Added by Rajas on 14 AUGUST 2017
        /// </summary>
        /// <param name="WetosDB"></param>
        /// <param name="EmpId"></param>
        /// <param name="LeaveTypeObj"></param>
        /// <param name="MessageString"></param>
        /// <param name="IsSuccess"></param>
        /// <returns></returns>
        // public static bool IsLeaveCombination(LeaveMaster LeaveMasterObj, string AppliedLeaveType, bool IsAppOk, ref string MessageString, ref bool IsSuccess)//CODE COMMENTED BY SHRADDHA ON 28 NOV 2017 START
        public static bool IsLeaveCombination(WetosDBEntities WetosDB, LeaveMaster LeaveMasterObj, LeaveModel LeaveTypeObj, DateTime CombinationCaseLeaveDate, string AppliedLeaveType, bool IsAppOk, string PrefixOrSuffix, int PreOrSuff, ref string MessageString, ref bool IsSuccess)//CODE ADDED BY SHRADDHA ON 28 NOV 2017 START
        {
            IsSuccess = true;

            try
            {
                // COMMENTED RULE RELATED FUNCTION BY MSJ ON 21 DEC 2017 START

                //CODE ADDED BY SHRADDHA ON 29 NOV 2017 TO SET RULE FOR ( RULE - 27 - Is Leave Combination applicable within same month only) START
                //BECAUSE AS PER DEEPTI MAM's TODAY's EMAIL COMBINATION CASE SHOULD BE APPLICABLE FOR SAME MONTH ONLY BASED ON RULE (FOR EVISKA RULE FOR SAME MONTH IS APPLICABLE NOT FOR OTHER COMPANY)
                //RuleTransaction RuleTransactionObj = WetosDB.RuleTransactions.Where(a => a.CompanyId == LeaveMasterObj.Company.CompanyId && a.BranchId == LeaveMasterObj.BranchId && a.EmployeeGroupId == LeaveMasterObj.EmployeeGroup.EmployeeGroupId
                // && a.RuleId == 28).FirstOrDefault(); // REPLACED RULE ID FROM 27 to 28

                //if (RuleTransactionObj != null && RuleTransactionObj.Formula.Trim().ToUpper() == "TRUE")
                {
                    if (IsAppOk == false) // ADDED FROM DATE AND END DATE MONTH AND YEAR COMPARISON CODE BY SHRADDHA ON 28 NOV 2017 
                    //BECAUSE AS PER DEEPTI MAM's TODAY's EMAIL COMBINATION CASE SHOULD BE APPLICABLE FOR SAME MONTH ONLY
                    {
                        // ADDED FROM DATE AND END DATE MONTH AND YEAR COMPARISON CODE BY SHRADDHA ON 28 NOV 2017 
                        //BECAUSE AS PER DEEPTI MAM's TODAY's EMAIL COMBINATION CASE SHOULD BE APPLICABLE FOR SAME MONTH ONLY

                        //if (LeaveTypeObj.FromDate.Month == CombinationCaseLeaveDate.Month || LeaveTypeObj.ToDate.Month == CombinationCaseLeaveDate.Month)
                        {
                            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 16 JAN 2018 START
                            //THERE IS UNDERSTANDING PROBLEM IN THIS RULE CODE THAT IF RULL VALUE IS FALSE THEN COMBINATION CASE SHOULD BE NOT ALLOWED
                            //if (!IsAppOk && (LeaveMasterObj.IsLeaveCombination == true || LeaveMasterObj.IsLeaveCombination == null)
                            //    && LeaveMasterObj.LeaveCode.ToUpper().Trim() != AppliedLeaveType)
                            if (!IsAppOk && (LeaveMasterObj.IsLeaveCombination == false)
                            && LeaveMasterObj.LeaveCode.ToUpper().Trim() != AppliedLeaveType)
                            {
                                MessageString = LeaveMasterObj.LeaveCode + " - " + AppliedLeaveType + " combination is not allowed.";
                                IsSuccess = false;
                            }
                            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 16 JAN 2018 END

                            //ADDED BY PUSHKAR ON 26 MAY 2018 FOR PREFIX AND SUFFIX
                            if (LeaveMasterObj.IsLeaveCombination == true && LeaveMasterObj.LeaveCode.ToUpper().Trim() != AppliedLeaveType.ToUpper().Trim())
                            {
                                if (PrefixOrSuffix != null || PrefixOrSuffix != "")
                                {
                                    if (PrefixOrSuffix.Contains(','))
                                    {
                                        List<string> CombLeaveId = PrefixOrSuffix.Split(',').ToList();
                                        bool breakloop = false;

                                        foreach (string CombLeaveIdStr in CombLeaveId)
                                        {
                                            int SinglePrefixSuffix = Convert.ToInt32(CombLeaveIdStr);
                                            string LeaveCode = WetosDB.LeaveMasters.Where(a => a.LeaveId == SinglePrefixSuffix).Select(a => a.LeaveCode).FirstOrDefault();

                                            if (!IsAppOk && AppliedLeaveType == LeaveCode)
                                            {
                                                //MessageString = LeaveMasterObj.LeaveCode + " - " + AppliedLeaveType + " combination is not allowed.";
                                                //IsSuccess = false;
                                                breakloop = true;
                                                break;
                                            }
                                        }
                                        if (breakloop == false)
                                        {
                                            MessageString = LeaveMasterObj.LeaveCode + " - " + AppliedLeaveType + " combination is not allowed.";
                                            IsSuccess = false;
                                        }

                                    }
                                    else
                                    {
                                        int SinglePrefixSuffix = Convert.ToInt32(PrefixOrSuffix);

                                        string LeaveCode = WetosDB.LeaveMasters.Where(a => a.LeaveId == SinglePrefixSuffix).Select(a => a.LeaveCode).FirstOrDefault();

                                        if (!IsAppOk && AppliedLeaveType != LeaveCode)
                                        {
                                            MessageString = LeaveMasterObj.LeaveCode + " - " + AppliedLeaveType + " combination is not allowed.";
                                            IsSuccess = false;
                                        }

                                    }
                                    //ADDED BY PUSHKAR ON 28 MAY 2018 PREFIX OR SUFFIX CHECK FOR PREVIOUS DAY LEAVE COMBINATION IS ALLOWED OR NOT
                                    if (IsSuccess == true)
                                    {
                                        int EmployeeIdGrpIdEx = LeaveMasterObj.EmployeeGroup.EmployeeGroupId;

                                        LeaveMaster LeaveMasterForDoubleCheck = WetosDB.LeaveMasters.Where(a => a.EmployeeGroup.EmployeeGroupId == EmployeeIdGrpIdEx &&
                                            a.LeaveCode == AppliedLeaveType).FirstOrDefault();

                                        if (LeaveMasterForDoubleCheck != null)
                                        {
                                            if (PreOrSuff == 1) //IsPrefix
                                            {
                                                if (LeaveMasterForDoubleCheck.IsLeaveCombination == false)
                                                {
                                                    MessageString = LeaveMasterObj.LeaveCode + " - " + AppliedLeaveType + " combination is not allowed.";
                                                    IsSuccess = false;
                                                }
                                                if (LeaveMasterForDoubleCheck.IsLeaveCombination == true)
                                                {
                                                    string SuffixCheck = LeaveMasterForDoubleCheck.Suffix;

                                                    if (SuffixCheck.Contains(','))
                                                    {
                                                        List<string> CombLeaveIdEx = SuffixCheck.Split(',').ToList();
                                                        bool breakloopEx = false;

                                                        foreach (string CombLeaveIdStrEx in CombLeaveIdEx)
                                                        {
                                                            int SinglePrefixSuffixEx = Convert.ToInt32(CombLeaveIdStrEx);

                                                            if (SinglePrefixSuffixEx == LeaveMasterObj.LeaveId)
                                                            {
                                                                breakloopEx = true;
                                                                break;
                                                            }

                                                        }
                                                        if (breakloopEx == false)
                                                        {
                                                            MessageString = LeaveMasterObj.LeaveCode + " - " + AppliedLeaveType + " combination is not allowed.";
                                                            IsSuccess = false;
                                                        }


                                                    }
                                                    else
                                                    {
                                                        int StrSuffix = Convert.ToInt32(SuffixCheck);
                                                        if (StrSuffix != LeaveMasterObj.LeaveId)
                                                        {
                                                            MessageString = LeaveMasterObj.LeaveCode + " - " + AppliedLeaveType + " combination is not allowed.";
                                                            IsSuccess = false;
                                                        }
                                                    }

                                                }
                                            }

                                            if (PreOrSuff == 2)//IsSuffix
                                            {
                                                if (LeaveMasterForDoubleCheck.IsLeaveCombination == false)
                                                {
                                                    MessageString = LeaveMasterObj.LeaveCode + " - " + AppliedLeaveType + " combination is not allowed.";
                                                    IsSuccess = false;
                                                }

                                                if (LeaveMasterForDoubleCheck.IsLeaveCombination == true)
                                                {
                                                    string PrefixCheck = LeaveMasterForDoubleCheck.Prefix;

                                                    if (PrefixCheck.Contains(','))
                                                    {
                                                        List<string> CombLeaveIdEx = PrefixCheck.Split(',').ToList();
                                                        bool breakloopEx = false;

                                                        foreach (string CombLeaveIdStrEx in CombLeaveIdEx)
                                                        {
                                                            int SinglePrefixSuffixEx = Convert.ToInt32(CombLeaveIdStrEx);

                                                            if (SinglePrefixSuffixEx == LeaveMasterObj.LeaveId)
                                                            {
                                                                breakloopEx = true;
                                                                break;
                                                            }

                                                        }
                                                        if (breakloopEx == false)
                                                        {
                                                            MessageString = LeaveMasterObj.LeaveCode + " - " + AppliedLeaveType + " combination is not allowed.";
                                                            IsSuccess = false;
                                                        }


                                                    }
                                                    else
                                                    {
                                                        int StrPrefix = Convert.ToInt32(PrefixCheck);
                                                        if (StrPrefix != LeaveMasterObj.LeaveId)
                                                        {
                                                            MessageString = LeaveMasterObj.LeaveCode + " - " + AppliedLeaveType + " combination is not allowed.";
                                                            IsSuccess = false;
                                                        }
                                                    }

                                                }



                                            }

                                        }

                                    }

                                }
                                else
                                {


                                }
                            }
                        }
                    }

                }
                //else
                //{
                //    if (!IsAppOk && (LeaveMasterObj.IsLeaveCombination == true || LeaveMasterObj.IsLeaveCombination == null)
                //        && LeaveMasterObj.LeaveCode.ToUpper().Trim() != AppliedLeaveType)
                //    {
                //        MessageString = LeaveMasterObj.LeaveCode + " - " + AppliedLeaveType + " combination is not allowed.";
                //        IsSuccess = false;
                //    }

                //}
            }
            catch
            {
                IsSuccess = false;
            }
            return IsSuccess;
        }
        #endregion

        #region Code To check Initial Validation before applying leave

        /// <summary>
        /// ADDED CODE BY SHRADDHA ON 21 JULY 2017
        /// TO CHECK INITIAL VALIDATION BY AJAX ON SUBMIT BUTTON CHECK 
        /// <param name="LeaveTypeObj"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CheckInitialValidationBeforeSubmit(DateTime FromDate, DateTime ToDate, int FromDayStatus, int ToDayStatus,
            int LeaveId, int EmployeeId, string MediCert, string AlreadyUploadedMediCert, int LeaveApplicationId = 0)
        {
            LeaveModel LeaveTypeObj = new LeaveModel();

            bool RetStat = true;
            string ErrorMessage = string.Empty;

            ///FLAG TO CHECK WHETHER FORM IS SUBMITTABLE OR NOT IF RetStat = FALSE //ADDED BY SHRADDHA ON 22 JULY 2017
            ///IN CASE OF (IsLeaveApplicable = FALSE SET IsFormSubmittable =FALSE) AND INCASE OF (IsSandwich = FALSE SET IsFormSubmittable=TRUE)
            bool IsFormSubmittable = true;
            try
            {
                bool ReturnStatus = true;

                int EmpId = EmployeeId;
                if (EmpId <= 0)
                {
                    EmpId = Convert.ToInt32(Session["EmployeeNo"]);
                }

                LeaveTypeObj.EmployeeId = EmpId;
                LeaveTypeObj.FromDate = FromDate;
                LeaveTypeObj.ToDate = ToDate;
                LeaveTypeObj.FromDayStatus = FromDayStatus;
                LeaveTypeObj.ToDayStatus = ToDayStatus;
                LeaveTypeObj.LeaveId = LeaveId;

                // ADDED BY MSJ ON 05 JAN 2018 START
                LeaveTypeObj.LeaveCode = WetosDB.LeaveMasters.Where(a => a.LeaveId == LeaveId).FirstOrDefault().LeaveCode;
                LeaveTypeObj.LeaveName = LeaveTypeObj.LeaveCode;
                // ADDED BY MSJ ON 05 JAN 2018 END

                LeaveTypeObj.LeaveApplicationId = LeaveApplicationId; // ADDED BY MSJ ON 25 JULY 2017 for LEAVE EDIT

                // CHECK Min Leave Allowed for PL
                LeaveMaster LeaveMasterData = WetosDB.LeaveMasters.Where(a => a.LeaveId == LeaveTypeObj.LeaveId).FirstOrDefault();

                //ADDED BY PUSHKAR ON 31 JULY 2018
                double AppliedDays = CalculateAppliedDays(LeaveTypeObj.FromDate, LeaveTypeObj.ToDate, LeaveTypeObj.FromDayStatus, LeaveTypeObj.ToDayStatus);
                LeaveTypeObj.AppliedDays = AppliedDays;

                #region CHECK WHETHER LEAVE IS APPLICABLE OR NOT

                RetStat = IsLeaveApplicable(WetosDB, EmpId, LeaveTypeObj, LeaveMasterData, ref ErrorMessage, ref ReturnStatus);
                IsFormSubmittable = RetStat;

                #endregion

                #region IF LEAVE IS APPLICABLE AND FORM IS SUBMITTABLE THEN CHECK FOR SANDWICH CASE

                if (IsFormSubmittable == true)
                {
                    // Updated by Rajas on 14 AUGUST 2017 START
                    bool IsLeaveCombine = false; // Added by Rajas on 14 AUGUST 2017

                    // START
                    //double AppliedDays = CalculateAppliedDays(LeaveTypeObj.FromDate, LeaveTypeObj.ToDate, LeaveTypeObj.FromDayStatus, LeaveTypeObj.ToDayStatus);
                    //LeaveTypeObj.AppliedDays = AppliedDays;

                    // MODIFIED BY MSJ ON 14 DEC 2017 START

                    Employee EmpObj = WetosDB.Employees.Where(a => a.EmployeeId == EmpId).FirstOrDefault();


                    //NEW CODE ADDED BY SHRADDHA ON 16 JAN 2018 TO CONSIDER LEAVE RULE MAX NO. OF TIMES LEAVE ALLOWED IN YEAR START
                    FinancialYear FinancialYearObj = WetosDB.FinancialYears.Where(a => a.StartDate <= FromDate && a.EndDate >= FromDate).FirstOrDefault();
                    if (FinancialYearObj == null)
                    {
                        string CurrentFinancialYear = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.CurrentFinancialYear).Select(a => a.SettingValue).FirstOrDefault();

                        FinancialYearObj = WetosDB.FinancialYears.Where(a => a.FinancialName == CurrentFinancialYear).FirstOrDefault();
                    }

                    var TotalNoOfTimesSelectedLeavesTakenInYear = WetosDB.LeaveApplications.Where(a => a.EmployeeId == EmpId && a.LeaveType == LeaveMasterData.LeaveCode
                        && a.StatusId == 2 && a.FromDate.Year == FinancialYearObj.StartDate.Year && (a.MarkedAsDelete == null || a.MarkedAsDelete == 0)).Count();

                    if (TotalNoOfTimesSelectedLeavesTakenInYear >= LeaveMasterData.MaxNoOfTimesAllowedInYear)
                    {
                        IsFormSubmittable = false;
                        ErrorMessage = string.Format("You have already taken leave for no. of times allowed in year {0} for leave type {1}", LeaveMasterData.MaxNoOfTimesAllowedInYear, LeaveMasterData.LeaveName);
                    }

                    //CODE ADDED BY SHRADDHA ON 17 JAN 2018 TO GET TOTAL NO. OF LEAVES TAKEN IN MONTH START
                    var TotalNoOfLeavesTakenInMonth = Convert.ToDouble(WetosDB.LeaveApplications.Where(a => a.EmployeeId == EmpId && a.LeaveType == LeaveMasterData.LeaveCode
                       && a.StatusId == 2 && a.FromDate.Year == FromDate.Year && a.FromDate.Month == FromDate.Month && (a.MarkedAsDelete == null || a.MarkedAsDelete == 0) && a.LeaveApplicationId != LeaveApplicationId).Select(a => a.ActualDays).FirstOrDefault());
                    //CODE ADDED BY SHRADDHA ON 17 JAN 2018 TO GET TOTAL NO. OF LEAVES TAKEN IN MONTH END


                    //NEW CODE ADDED BY SHRADDHA ON 16 JAN 2018 TO CONSIDER LEAVE RULE MAX NO. OF TIMES LEAVE ALLOWED IN YEAR END

                    // CHECK FOR NULL
                    if (LeaveMasterData != null)
                    {
                        //IS HALF DAY ALLOWED VALIDATION ADDED BY SHRADDHA ON 16 JAN 2018 START
                        if (AppliedDays == 0.5 && LeaveMasterData.ISHalfAllowed == false)
                        {
                            IsFormSubmittable = false;
                            ErrorMessage = string.Format("Half Day leave not allowed for leave type {0}", LeaveMasterData.LeaveName);
                        }
                        //IS HALF DAY ALLOWED VALIDATION ADDED BY SHRADDHA ON 16 JAN 2018 END

                        // LESS THAN MINIMUM ALLOWED
                        if (AppliedDays < LeaveMasterData.MinNoofLeaveAllowedatatime)
                        {
                            IsFormSubmittable = false;
                            ErrorMessage = string.Format("Applying for lesser than Minimum limit allowed at a time of {0}", LeaveMasterData.MinNoofLeaveAllowedatatime);
                        }
                        else if (AppliedDays > LeaveMasterData.MaxNoOfLeavesAllowedAtaTime) // GREATER THAN MAX ALLOWED IN MONTH //TEMP COMMENTED BY MSJ ON 19 DEC 2017 START
                        {
                            IsFormSubmittable = false;

                            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 16 JAN 2018 START
                            //ErrorMessage = string.Format("Applying for more than Leave Allowed at a Time limit of {0}", LeaveMasterData.MinNoofLeaveAllowedatatime);
                            ErrorMessage = string.Format("Applying for more than Leave Allowed at a Time limit of {0}", LeaveMasterData.MaxNoOfLeavesAllowedAtaTime);
                            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 16 JAN 2018 END

                        }

                        else if ((Convert.ToDouble(TotalNoOfLeavesTakenInMonth) + Convert.ToDouble(AppliedDays)) > LeaveMasterData.MaxNoOfLeavesAllowedInMonth) // GREATER THAN MAX ALLOWED IN MONTH //TEMP COMMENTED BY MSJ ON 19 DEC 2017 START
                        {
                            IsFormSubmittable = false;

                            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 16 JAN 2018 START
                            //ErrorMessage = string.Format("Applying for more than Leave Allowed at a Time limit of {0}", LeaveMasterData.MinNoofLeaveAllowedatatime);
                            ErrorMessage = string.Format("Applying for more than Leave Allowed in a month limit of {0}", LeaveMasterData.MaxNoOfLeavesAllowedInMonth);
                            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 16 JAN 2018 END

                        }

                        // TEMP COMMENTED BY MSJ ON 19 DEC 2017 END

                        // IS MEDICAL CERTIFICATE NEED in SL
                        //int IsMediCertAttached = MediCert == null ? 0 : MediCert.Value; // ADDED BY MSJ ON 18 DEC 2017

                        //if (LeaveMasterData.LeaveCode.ToUpper() == "SL")
                        //{
                        //    if (AppliedDays >= 2 && IsMediCertAttached == 0) // 0 - NOT ATTACHED, 1 - ATTACHED
                        //    {
                        //        IsFormSubmittable = false;
                        //        ErrorMessage = "Attach Medical Certificate while Applying for Sick Leave.";
                        //    }
                        //}

                        bool IsAttachmentRequiredFlag = false;

                        LeaveMaster LeaveMasterObj = WetosDB.LeaveMasters.Where(a => a.LeaveId == LeaveId).FirstOrDefault();
                        if (LeaveMasterObj != null)
                        {
                            bool IsAttachmentRequired = Convert.ToBoolean(LeaveMasterObj.IsAttachmentNeeded == null ? false : LeaveMasterObj.IsAttachmentNeeded);
                            double AttachmentRequiredForMinNoOfLeave = Convert.ToDouble(LeaveMasterObj.AttachmentRequiredForMinNoOfLeave == null ? 0 : LeaveMasterObj.AttachmentRequiredForMinNoOfLeave);
                            if (IsAttachmentRequired == true && AttachmentRequiredForMinNoOfLeave <= AppliedDays)
                            {
                                IsAttachmentRequiredFlag = true;
                            }
                        }
                        if (IsAttachmentRequiredFlag == true && (string.IsNullOrEmpty(MediCert) && string.IsNullOrEmpty(AlreadyUploadedMediCert)))
                        {
                            IsFormSubmittable = false;
                            ErrorMessage = "Attach Medical Certificate while Applying for Sick Leave.";
                        }
                    }

                    RetStat = IsFormSubmittable; // ADDED BY MSJ ON 14 DEC 2017
                    // 
                    if (IsFormSubmittable == true) // OTHER LEAVE LIMITS ADDED 
                    {
                        // Minimum 3 or above
                        // IS MEDICAL CERTIFICATE NEED in SL
                        // COMBINATION

                        // MODIFIED BY MSJ ON 14 DEC 2017 END

                        // END

                        // IsLeaveCombine flag added to get combination case (Y/N)
                        RetStat = IsSandwich(WetosDB, EmpId, ref ErrorMessage, ref LeaveTypeObj, ref IsLeaveCombine);

                        // If Leave combination is detected then form is not submittable
                        // Added by Rajas on 14 AUGUST 2017
                        if (IsLeaveCombine == true)
                        {
                            IsFormSubmittable = false;
                        }
                        // Updated by Rajas on 14 AUGUST 2017 END
                    }

                    // MODIFIED BY MSJ ON 14 DEC 2017 END
                }

                #endregion
            }
            catch
            {
                RetStat = false;
            }

            return Json(new { RetStat = RetStat, IsFormSubmittable = IsFormSubmittable, ErrorMessage = ErrorMessage }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Code to check whether Leave is applicable or not
        /// ADDED BY SHRADDHA ON 21 JULY 2017 
        /// TO CHECK WHETHER LEAVE IS APPLICABLE OR NOT WITH INITIAL VALIDATIONS
        /// </summary>
        /// <returns></returns>
        public static bool IsLeaveApplicable(WetosDBEntities WetosDB, int EmpId, LeaveModel LeaveTypeObj, LeaveMaster LeaveMasterData, ref string SuccessMessage, ref bool IsSuccess)
        {
            string MessageString = string.Empty;
            bool IsValidFlag = true;
            IsSuccess = true;
            bool IsLeaveCombine = false;
            string ErrorMessage = "";
            try
            {
                IsSandwich(WetosDB, EmpId, ref ErrorMessage, ref LeaveTypeObj, ref IsLeaveCombine);
                //TO GET EMPLOYEE TABLE DATA FOR LOGIN EMPLOYEE
                Employee LoginEmployee = WetosDB.Employees.Where(a => a.EmployeeId == EmpId).FirstOrDefault();

                //TO GET EMPLOYEE GROUP DETAIL TABLE DATA FOR LOGIN EMPLOYEE
                EmployeeGroupDetail EmployeeGroupDetailObj = WetosDB.EmployeeGroupDetails.Where(a => a.Employee.EmployeeId == EmpId).FirstOrDefault();

                //TO GET LeaveMaster TABLE DATA FOR LOGIN EMPLOYEE 
                LeaveMaster LeaveMasterObj = WetosDB.LeaveMasters.Where(a => a.LeaveId == LeaveTypeObj.LeaveId
                    && a.EmployeeGroup.EmployeeGroupId == EmployeeGroupDetailObj.EmployeeGroup.EmployeeGroupId).FirstOrDefault();

                #region CODE TO CHECK WHETHER DATE RANGE AND STATUS IS VALID OR NOT
                if (IsSuccess == true)
                {
                    IsSuccess = IsDateRangeandStatusValidOrNot(WetosDB, LeaveTypeObj, ref MessageString, ref IsValidFlag);
                }
                #endregion

                #region GET AVAILABLE BALANCE FOR LEAVE APPLICATION

                // ADDED BY MSJ ON 05 JAN 2018 START
                List<SP_LeaveTableData_Result> BBB = DashboardController.GetLeaveDetailsOnallControllers(WetosDB, EmpId, DateTime.Now);

                SP_LeaveTableData_Result CCC = BBB.Where(a => a.LeaveType.Trim() == LeaveTypeObj.LeaveName.Trim()).FirstOrDefault();
                //ADDED VALIDATION TO HANDLE NULL EXCEPTION ERROR FOR CCC BY SHRADDHA ON 16 JAN 2018 START
                if (CCC != null)
                {
                    double AvailableLeaveBalance = CCC.OpeningBalance.Value - (Convert.ToDouble(CCC.LeaveUsed) + CCC.Pending);

                    // ADDED BY MSJ ON 31 JULY 2018 
                    double AvailableLeaveBalanceAfter = AvailableLeaveBalance - LeaveTypeObj.TotalDeductableDays;

                    // Replaced IsInsufficientBalAllowed with NegativeBalanceAllowed // MODIFIED BY MSJ ON 30 JULY 2018 
                    //if (AvailableLeaveBalance <= 0 && LeaveMasterData.NegativeBalanceAllowed == false)
                    if (AvailableLeaveBalanceAfter < 0 && LeaveMasterData.NegativeBalanceAllowed == false) // ADDED BY MSJ ON 31 JULY 2018 replaced <= with <
                    {
                        IsSuccess = false;
                        MessageString = "You have zero balance for leave type : " + LeaveTypeObj.LeaveName;
                    }

                    // COMMNETD BY MSJ ON 30 JULY 2018 START
                    //if (AvailableLeaveBalance == 0 && LeaveMasterData.IsInsufficientBalAllowed == false) // ADDED IsInsufficientBalAllowed CONDITION BY SHRADDHA ON 17 JAN 2018
                    //{
                    //    IsSuccess = false;
                    //    MessageString = "You have zero balance for leave type : " + LeaveTypeObj.LeaveName;
                    //}
                    // COMMNETD BY MSJ ON 30 JULY 2018 END
                }
                else
                {
                    IsSuccess = false;
                    string EmployeeName = (LoginEmployee.FirstName == null ? "" : LoginEmployee.FirstName) + " " + (LoginEmployee.MiddleName == null ? "" : LoginEmployee.MiddleName) + " " + (LoginEmployee.LastName == null ? "" : LoginEmployee.LastName);
                    MessageString = "No leave data available for -" + LoginEmployee.EmployeeCode + " - " + EmployeeName + " for selected leave type " + LeaveTypeObj.LeaveName + ". Contact Administration.";
                }
                //ADDED VALIDATION TO HANDLE NULL EXCEPTION ERROR FOR CCC BY SHRADDHA ON 16 JAN 2018 END
                // ADDED BY MSJ ON 05 JAN 2018 END



                #endregion GET AVAILABLE BALANCE FOR LEAVE APPLICATION

                #region CODE TO GET IS HOLIDAY AVAILABLE
                if (IsSuccess == true)
                {
                    IsSuccess = IsHolidayAvailable(WetosDB, LoginEmployee.CompanyId, LoginEmployee.BranchId, LeaveTypeObj, ref MessageString, ref IsValidFlag);
                }
                #endregion

                #region CODE TO CHECK WHETHER PUNCH IS AVAIALBLE OR NOT FOR SELECTED DATE RANGE AND SELECTED FROMDATE AND TODATE STATUS
                if (IsSuccess == true)
                {
                    IsSuccess = WetosLeaveTransactionController.IsPunchAvailable(WetosDB, EmpId, LeaveTypeObj, ref MessageString, ref IsValidFlag);
                }
                #endregion

                #region CODE TO CHECK WHETHER ANY PENDING OR SANCTIONED LEAVE IS ALREADY AVAILABLE FOR SELECTED DATE RANGE
                if (IsSuccess == true)
                {
                    IsSuccess = IsLeaveAlreadyAvailableForSelectedDateRange(WetosDB, EmpId, LeaveTypeObj, ref MessageString, ref IsValidFlag);
                }
                #endregion

                #region CODE TO CHECK WHETHER ANY PENDING OR SANCTIONED COMPOFF IS ALREADY AVAILABLE FOR SELECTED DATE RANGE
                if (IsSuccess == true)
                {
                    IsSuccess = IsCOMPOFFAlreadyAvailableForSelectedDateRange(WetosDB, EmpId, LeaveTypeObj, ref MessageString, ref IsValidFlag);
                }
                #endregion

                #region CODE TO CHECK WHETHER ANY PENDING OR SANCTIONED OD/OT IS ALREADY AVAILABLE FOR SELECTED DATE RANGE
                if (IsSuccess == true)
                {
                    IsSuccess = IsODAlreadyAvailableForSelectedDateRange(WetosDB, EmpId, LeaveTypeObj, ref MessageString, ref IsValidFlag);
                }
                #endregion

                #region CODE TO CHECK WHETHER DATA IS LOCKED OR NOT FOR SELECTED DATE RANGE
                if (IsSuccess == true)
                {
                    IsSuccess = IsDataLockedForSelectedDateRange(WetosDB, EmpId, LeaveTypeObj, ref MessageString, ref IsValidFlag);
                }
                #endregion

            }
            catch (System.Exception E)
            {
                IsSuccess = false;
                MessageString = "Error in Leave application validation" + E.Message + (E.InnerException == null ? string.Empty : E.InnerException.Message);
            }

            SuccessMessage = MessageString;

            return IsSuccess;
        }
        #endregion

        #region Code to check whether any Comp off is available or not for Applied leave Date
        /// <summary>
        /// ADDED BY SHRADDHA ON 21 JULY 2017 
        /// TO CHECK WHETHER ANY PENDING OR SANCTIONED COMPOFF IS ALREADY AVAILABLE FOR SELECTED DATE RANGE
        /// <param name="EmployeeId"></param>
        /// <param name="LeaveTypeObj"></param>
        /// <param name="Message"></param>
        /// <param name="IsSuccess"></param>
        /// <returns></returns>
        public static bool IsCOMPOFFAlreadyAvailableForSelectedDateRange(WetosDBEntities WetosDB, int EmployeeId, LeaveModel LeaveTypeObj, ref string Message, ref bool IsSuccess)
        {
            IsSuccess = true;
            try
            {
                for (DateTime CurrentDate = LeaveTypeObj.FromDate; CurrentDate.Date <= LeaveTypeObj.ToDate; CurrentDate = CurrentDate.AddDays(1))
                {
                    // MODIFIED BY MSJ 19 JAN 2018 START
                    //TO GET LIST OF COMPOFF APPLICATION FOR LOGIN EMPLOYEE
                    //CompOffApplication CompoffApplicationListObj = WetosDB.CompOffApplications.Where(a => a.EmployeeId == EmployeeId
                    //     && (a.StatusId == 1 || a.StatusId == 2 || a.StatusId == 4) && a.MarkedAsDelete == 0
                    //     && a.FromDate <= LeaveTypeObj.FromDate && a.ToDate >= LeaveTypeObj.ToDate).FirstOrDefault();

                    CompOffApplication CompoffApplicationListObj = WetosDB.CompOffApplications.Where(a => a.EmployeeId == EmployeeId
                         && (a.StatusId == 1 || a.StatusId == 2 || a.StatusId == 4) && a.MarkedAsDelete == 0
                         && ((a.FromDate == LeaveTypeObj.FromDate)
                             || (a.FromDate < LeaveTypeObj.FromDate && a.ToDate == LeaveTypeObj.ToDate)
                             || (a.FromDate < LeaveTypeObj.FromDate && a.ToDate > LeaveTypeObj.ToDate)
                             || (a.FromDate > LeaveTypeObj.FromDate && a.ToDate < LeaveTypeObj.ToDate)
                             || (a.FromDate == LeaveTypeObj.FromDate && a.ToDate == LeaveTypeObj.ToDate)
                             || (a.FromDate > LeaveTypeObj.FromDate && a.FromDate < LeaveTypeObj.ToDate)
                             || (a.ToDate > LeaveTypeObj.FromDate && a.ToDate < LeaveTypeObj.ToDate)
                             || (a.ToDate == LeaveTypeObj.FromDate))
                             ).FirstOrDefault();
                    // MODIFIED BY MSJ 19 JAN 2018 END

                    if (CompoffApplicationListObj != null)
                    {
                        #region Validate Leave Data

                        // Code added by Rajas on 27 SEP 2017 START
                        if (CompoffApplicationListObj.FromDate == CompoffApplicationListObj.ToDate) // Single day
                        {
                            if (CurrentDate == CompoffApplicationListObj.FromDate)
                            {
                                if (CompoffApplicationListObj.FromDateStatus == 1)  // Full day in single day
                                {
                                    Message = "You Can not Apply Leave For " + LeaveTypeObj.FromDate.ToString("dd-MMM-yyyy") + " To " + LeaveTypeObj.ToDate.ToString("dd-MMM-yyyy") + " Range"
                                      + "<br/>" + "You may already have pending or approved/sanctioned Compoff for this date range.";
                                    IsSuccess = false;
                                }
                                else
                                {
                                    if (CompoffApplicationListObj.FromDateStatus == LeaveTypeObj.FromDayStatus)
                                    {
                                        Message = "You Can not Apply Leave For " + LeaveTypeObj.FromDate.ToString("dd-MMM-yyyy") + " To " + LeaveTypeObj.ToDate.ToString("dd-MMM-yyyy") + " Range"
                                      + "<br/>" + "You may already have pending or approved/sanctioned Compoff for this date range.";
                                        IsSuccess = false;
                                    }
                                    else
                                    {
                                        if (LeaveTypeObj.FromDayStatus == 1)
                                        {
                                            Message = "You Can not Apply Leave For " + LeaveTypeObj.FromDate.ToString("dd-MMM-yyyy") + " To " + LeaveTypeObj.ToDate.ToString("dd-MMM-yyyy") + " Range"
                                         + "<br/>" + "You may already have pending or approved/sanctioned Leave for this date range.";
                                            IsSuccess = false;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (CurrentDate == CompoffApplicationListObj.FromDate)
                            {
                                if (CompoffApplicationListObj.FromDateStatus == 1)  // Full day in single day
                                {
                                    Message = "You Can not Apply Leave For " + LeaveTypeObj.FromDate.ToString("dd-MMM-yyyy") + " To " + LeaveTypeObj.ToDate.ToString("dd-MMM-yyyy") + " Range"
                                      + "<br/>" + "You may already have pending or approved/sanctioned Compoff for this date range.";
                                    IsSuccess = false;
                                }
                                else
                                {
                                    if (CompoffApplicationListObj.FromDateStatus == LeaveTypeObj.FromDayStatus)
                                    {
                                        Message = "You Can not Apply Leave For " + LeaveTypeObj.FromDate.ToString("dd-MMM-yyyy") + " To " + LeaveTypeObj.ToDate.ToString("dd-MMM-yyyy") + " Range"
                                      + "<br/>" + "You may already have pending or approved/sanctioned Compoff for this date range.";
                                        IsSuccess = false;
                                    }
                                    else
                                    {
                                        if (LeaveTypeObj.FromDayStatus == 1)
                                        {
                                            Message = "You Can not Apply Leave For " + LeaveTypeObj.FromDate.ToString("dd-MMM-yyyy") + " To " + LeaveTypeObj.ToDate.ToString("dd-MMM-yyyy") + " Range"
                                         + "<br/>" + "You may already have pending or approved/sanctioned Compoff for this date range.";
                                            IsSuccess = false;
                                        }
                                    }
                                }
                            }
                            else if (CurrentDate == CompoffApplicationListObj.ToDate)
                            {
                                if (CompoffApplicationListObj.ToDateStatus == 1)  // Full day in single day
                                {
                                    Message = "You Can not Apply Leave For " + LeaveTypeObj.FromDate.ToString("dd-MMM-yyyy") + " To " + LeaveTypeObj.ToDate.ToString("dd-MMM-yyyy") + " Range"
                                      + "<br/>" + "You may already have pending or approved/sanctioned Compoff for this date range.";
                                    IsSuccess = false;
                                }
                                else
                                {
                                    if (CompoffApplicationListObj.FromDateStatus == LeaveTypeObj.FromDayStatus)
                                    {
                                        Message = "You Can not Apply Leave For " + LeaveTypeObj.FromDate.ToString("dd-MMM-yyyy") + " To " + LeaveTypeObj.ToDate.ToString("dd-MMM-yyyy") + " Range"
                                      + "<br/>" + "You may already have pending or approved/sanctioned Compoff for this date range.";
                                        IsSuccess = false;
                                    }
                                }
                            }
                            else
                            {
                                Message = "You Can not Apply Leave For " + LeaveTypeObj.FromDate.ToString("dd-MMM-yyyy") + " To " + LeaveTypeObj.ToDate.ToString("dd-MMM-yyyy") + " Range"
                                        + "<br/>" + "You may already have pending or approved/sanctioned Compoff for this date range.";
                                IsSuccess = false;
                            }
                        }
                        // Code added by Rajas on 27 SEP 2017 END

                        #endregion Validate OD Data
                    }

                }
            }
            catch
            {
                IsSuccess = false;
            }
            return IsSuccess;

        }

        #endregion

        #region Code to check whether Data is locked or not for Applied leave date
        /// ADDED BY SHRADDHA ON 21 JULY 2017 
        /// TO CHECK WHETHER DATA IS LOCKED OR NOT FOR SELECTED DATE RANGE
        /// </summary>
        /// <returns></returns>
        public static bool IsDataLockedForSelectedDateRange(WetosDBEntities WetosDB, int EmployeeId, LeaveModel LeaveTypeObj, ref string Message, ref bool IsSuccess)
        {
            IsSuccess = true;
            try
            {
                DailyTransaction DailyTransactionListObj = WetosDB.DailyTransactions.Where(a => a.TranDate >= LeaveTypeObj.FromDate
                    && a.TranDate <= LeaveTypeObj.ToDate && a.EmployeeId == EmployeeId && a.Lock == "Y").FirstOrDefault();

                if (DailyTransactionListObj != null)
                {
                    Message = "You Can not apply leave for this range as Data is Locked";
                    IsSuccess = false;
                }
            }
            catch
            {
                IsSuccess = false;
            }
            return IsSuccess;
        }
        #endregion

        #region Code to check whether Holiday is available or not for Applied leave
        /// <summary>
        /// ADDED CODE BY SHRADDHA ON 21 JULY 2017
        /// TO CHECK WHETHER HOLIDAY AVAILABLE FOR SELECTED DATE OR NOT
        /// IT ONLY CHECKS VALIDATION FOR SINGLE DAY LEAVE APPLICATION (FROMDATE == TODATE)
        /// <param name="BranchId"></param>
        /// <param name="LeaveTypeObj"></param>
        /// <param name="Message"></param>
        /// <param name="IsSuccess"></param>
        /// <returns></returns>
        public static bool IsHolidayAvailable(WetosDBEntities WetosDB, int CompanyId, int BranchId, LeaveModel LeaveTypeObj, ref string Message, ref bool IsSuccess)
        {
            IsSuccess = true;
            try
            {
                //TO GET HOLIDAY TABLE DATA FOR LOGIN EMPLOYEE // Modified by MSJ ON 29 DEC 2018
                HoliDay HolidayObj = WetosDB.HoliDays.Where(a => a.FromDate == LeaveTypeObj.FromDate && a.CompanyId == CompanyId && a.Branchid == BranchId && a.MarkedAsDelete != 1).FirstOrDefault();


                // MSJ 15 SEP 2017
                //TO GET DECLARED HOLIDAY TABLE DATA FOR LOGIN EMPLOYEE
                DeclaredHoliday DeclaredHolidayObj = WetosDB.DeclaredHolidays.Where(a => a.HolidayDate == LeaveTypeObj.FromDate && a.Branch.BranchId == BranchId && a.MarkASDelete != 1).FirstOrDefault();

                if (HolidayObj != null)
                {
                    if (HolidayObj.FromDate == LeaveTypeObj.FromDate && HolidayObj.DayStatus == 1)
                    {
                        Message = "You can not apply leave on Holiday";
                        IsSuccess = false;
                    }
                    else if (HolidayObj.FromDate == LeaveTypeObj.FromDate && HolidayObj.DayStatus == LeaveTypeObj.FromDayStatus)
                    {
                        Message = "You can not apply leave on Holiday";
                        IsSuccess = false;
                    }
                }
                else if (DeclaredHolidayObj != null)
                {
                    Message = "You can not apply leave on Declared Holiday";
                    IsSuccess = false;
                }
            }
            catch
            {
                IsSuccess = false;
            }
            return IsSuccess;
        }
        #endregion

        #region Code to check whether selected date range and day status are valid or not for Applied leave
        /// ADDED BY SHRADDHA ON 21 JULY 2017 
        /// TO CHECK WHETHER SELECTED DATE RANGE AND STATUS IS VALID OR NOT
        /// </summary>
        /// <returns></returns>
        public static bool IsDateRangeandStatusValidOrNot(WetosDBEntities WetosDB, LeaveModel LeaveTypeObj, ref string Message, ref bool IsSuccess)
        {
            IsSuccess = true;
            try
            {
                if (LeaveTypeObj.FromDate > LeaveTypeObj.ToDate)
                {
                    Message = "End Date should be Greater than or equal to From Date";
                    IsSuccess = false;
                }

                //COMMENTED BY PUSHKAR ON 27 FEB 2018 TO SOLVE ISSUE AS SUGGESTED BY MSJ
                //if (LeaveTypeObj.FromDate.Month != LeaveTypeObj.ToDate.Month)
                //{
                //    Message = "From Date and To Date should be of same month";
                //    IsSuccess = false;
                //}

                if (LeaveTypeObj.FromDate == LeaveTypeObj.ToDate)
                {
                    if (LeaveTypeObj.FromDayStatus != LeaveTypeObj.ToDayStatus)
                    {
                        Message = "Select Proper status";
                        IsSuccess = false;
                    }
                }
            }
            catch
            {
                IsSuccess = false;
            }
            return IsSuccess;
        }
        #endregion

        #region Code to check whether any another pending or Sanctioned leave is available or not for Applied leave date
        /// <summary>
        /// ADDED BY SHRADDHA ON 21 JULY 2017 
        /// TO CHECK WHETHER ANY PENDING OR SANCTIONED LEAVE IS ALREADY AVAILABLE FOR SELECTED DATE RANGE
        /// <param name="EmployeeId"></param>
        /// <param name="LeaveTypeObj"></param>
        /// <param name="Message"></param>
        /// <param name="IsSuccess"></param>
        /// <returns></returns>
        public static bool IsLeaveAlreadyAvailableForSelectedDateRange(WetosDBEntities WetosDB, int EmployeeId, LeaveModel LeaveTypeObj, ref string Message, ref bool IsSuccess)
        {
            IsSuccess = true;

            try
            {
                for (DateTime CurrentDate = LeaveTypeObj.FromDate; CurrentDate.Date <= LeaveTypeObj.ToDate; CurrentDate = CurrentDate.AddDays(1))
                {
                    //TO GET LIST OF LEAVE APPLICATION FOR LOGIN EMPLOYEE
                    //LeaveApplication LeaveApplicationObj = WetosDB.LeaveApplications.Where(a => a.EmployeeId == EmployeeId
                    //         && (a.StatusId == 1 || a.StatusId == 2 || a.StatusId == 4) && a.LeaveApplicationId != LeaveTypeObj.LeaveApplicationId
                    //         && a.MarkedAsDelete == 0 && a.FromDate <= LeaveTypeObj.FromDate && a.ToDate >= LeaveTypeObj.ToDate).FirstOrDefault(); 
                    //Added Leave Application ID validation by shraddha on 13 SEP 2017

                    LeaveApplication LeaveApplicationObj = WetosDB.LeaveApplications.Where(a => a.EmployeeId == EmployeeId
                             && (a.StatusId == 1 || a.StatusId == 2 || a.StatusId == 4) && a.LeaveApplicationId != LeaveTypeObj.LeaveApplicationId
                             && a.MarkedAsDelete == 0
                             && (
                             (a.FromDate == LeaveTypeObj.FromDate) // && a.ToDate > LeaveTypeObj.ToDate) 
                             || (a.FromDate < LeaveTypeObj.FromDate && a.ToDate == LeaveTypeObj.ToDate)
                             || (a.FromDate < LeaveTypeObj.FromDate && a.ToDate > LeaveTypeObj.ToDate)
                             || (a.FromDate > LeaveTypeObj.FromDate && a.ToDate < LeaveTypeObj.ToDate)
                             || (a.FromDate == LeaveTypeObj.FromDate && a.ToDate == LeaveTypeObj.ToDate)
                             || (a.FromDate > LeaveTypeObj.FromDate && a.FromDate < LeaveTypeObj.ToDate) // ADDED BY MSJ ON 11 JAN 2018
                             || (a.ToDate > LeaveTypeObj.FromDate && a.ToDate < LeaveTypeObj.ToDate) // ADDED BY MSJ ON 11 JAN 2018
                             || (a.ToDate == LeaveTypeObj.FromDate)  // ADDED BY MSJ ON 11 JAN 2018
                             )).FirstOrDefault();

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
                                    Message = "You Can not Apply Leave For " + LeaveTypeObj.FromDate.ToString("dd-MMM-yyyy") + " To " + LeaveTypeObj.ToDate.ToString("dd-MMM-yyyy") + " Range"
                                      + "<br/>" + "You may already have pending or approved/sanctioned Leave for this date range.";
                                    IsSuccess = false;
                                }
                                else
                                {
                                    if (LeaveApplicationObj.FromDayStatus == LeaveTypeObj.FromDayStatus)
                                    {
                                        Message = "You Can not Apply Leave For " + LeaveTypeObj.FromDate.ToString("dd-MMM-yyyy") + " To " + LeaveTypeObj.ToDate.ToString("dd-MMM-yyyy") + " Range"
                                      + "<br/>" + "You may already have pending or approved/sanctioned Leave for this date range.";
                                        IsSuccess = false;
                                    }
                                    else
                                    {
                                        if (LeaveTypeObj.FromDayStatus == 1)
                                        {
                                            Message = "You Can not Apply Leave For " + LeaveTypeObj.FromDate.ToString("dd-MMM-yyyy") + " To " + LeaveTypeObj.ToDate.ToString("dd-MMM-yyyy") + " Range"
                                         + "<br/>" + "You may already have pending or approved/sanctioned Leave for this date range.";
                                            IsSuccess = false;
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
                                    Message = "You Can not Apply Leave For " + LeaveTypeObj.FromDate.ToString("dd-MMM-yyyy") + " To " + LeaveTypeObj.ToDate.ToString("dd-MMM-yyyy") + " Range"
                                      + "<br/>" + "You may already have pending or approved/sanctioned Leave for this date range.";
                                    IsSuccess = false;
                                }
                                else
                                {
                                    if (LeaveApplicationObj.FromDayStatus == LeaveTypeObj.FromDayStatus)
                                    {
                                        Message = "You Can not Apply Leave For " + LeaveTypeObj.FromDate.ToString("dd-MMM-yyyy") + " To " + LeaveTypeObj.ToDate.ToString("dd-MMM-yyyy") + " Range"
                                      + "<br/>" + "You may already have pending or approved/sanctioned Leave for this date range.";
                                        IsSuccess = false;
                                    }
                                    else
                                    {
                                        if (LeaveTypeObj.FromDayStatus == 1)
                                        {
                                            Message = "You Can not Apply Leave For " + LeaveTypeObj.FromDate.ToString("dd-MMM-yyyy") + " To " + LeaveTypeObj.ToDate.ToString("dd-MMM-yyyy") + " Range"
                                         + "<br/>" + "You may already have pending or approved/sanctioned Leave for this date range.";
                                            IsSuccess = false;
                                        }
                                    }
                                }
                            }
                            else if (CurrentDate == LeaveApplicationObj.ToDate)
                            {
                                if (LeaveApplicationObj.ToDayStatus == 1)  // Full day in single day
                                {
                                    Message = "You Can not Apply Leave For " + LeaveTypeObj.FromDate.ToString("dd-MMM-yyyy") + " To " + LeaveTypeObj.ToDate.ToString("dd-MMM-yyyy") + " Range"
                                      + "<br/>" + "You may already have pending or approved/sanctioned Leave for this date range.";
                                    IsSuccess = false;
                                }
                                else
                                {
                                    if (LeaveApplicationObj.FromDayStatus == LeaveTypeObj.FromDayStatus)
                                    {
                                        Message = "You Can not Apply Leave For " + LeaveTypeObj.FromDate.ToString("dd-MMM-yyyy") + " To " + LeaveTypeObj.ToDate.ToString("dd-MMM-yyyy") + " Range"
                                      + "<br/>" + "You may already have pending or approved/sanctioned Leave for this date range.";
                                        IsSuccess = false;
                                    }
                                }
                            }
                            else
                            {
                                Message = "You Can not Apply Leave For " + LeaveTypeObj.FromDate.ToString("dd-MMM-yyyy") + " To " + LeaveTypeObj.ToDate.ToString("dd-MMM-yyyy") + " Range"
                                        + "<br/>" + "You may already have pending or approved/sanctioned Leave for this date range.";
                                IsSuccess = false;
                            }
                        }
                        // Code added by Rajas on 27 SEP 2017 END

                        #endregion Validate OD Data
                    }
                }

            }
            catch
            {
                IsSuccess = false;
            }
            return IsSuccess;

        }
        #endregion

        #region Code to check whether OD is available or not for Applied leave date
        /// <summary>
        /// ADDED BY SHRADDHA ON 21 JULY 2017 
        /// TO CHECK WHETHER ANY PENDING OR SANCTIONED OD/OT IS ALREADY AVAILABLE FOR SELECTED DATE RANGE
        /// <param name="EmployeeId"></param>
        /// <param name="LeaveTypeObj"></param>
        /// <param name="Message"></param>
        /// <param name="IsSuccess"></param>
        /// <returns></returns>
        public static bool IsODAlreadyAvailableForSelectedDateRange(WetosDBEntities WetosDB, int EmployeeId, LeaveModel LeaveTypeObj, ref string Message, ref bool IsSuccess)
        {
            IsSuccess = true;
            try
            {
                for (DateTime CurrentDate = LeaveTypeObj.FromDate; CurrentDate.Date <= LeaveTypeObj.ToDate; CurrentDate = CurrentDate.AddDays(1))
                {
                    // MODIFIED BY MSJ ON 19 JAN 2018 START
                    //TO GET LIST OF OD APPLICATION FOR LOGIN EMPLOYEE
                    //ODTour ODTourListObj = WetosDB.ODTours.Where(a => a.EmployeeId == EmployeeId && (a.StatusId == 1 || a.StatusId == 2 || a.StatusId == 4)
                    //    && a.MarkedAsDelete == 0 && (a.FromDate <= LeaveTypeObj.FromDate && a.ToDate >= LeaveTypeObj.ToDate)).FirstOrDefault();

                    ODTour ODTourListObj = WetosDB.ODTours.Where(a => a.EmployeeId == EmployeeId && (a.StatusId == 1 || a.StatusId == 2 || a.StatusId == 4)
                        && a.MarkedAsDelete == 0
                    && ((a.FromDate == LeaveTypeObj.FromDate)
                             || (a.FromDate < LeaveTypeObj.FromDate && a.ToDate == LeaveTypeObj.ToDate)
                             || (a.FromDate < LeaveTypeObj.FromDate && a.ToDate > LeaveTypeObj.ToDate)
                             || (a.FromDate > LeaveTypeObj.FromDate && a.ToDate < LeaveTypeObj.ToDate)
                             || (a.FromDate == LeaveTypeObj.FromDate && a.ToDate == LeaveTypeObj.ToDate)
                             || (a.FromDate > LeaveTypeObj.FromDate && a.FromDate < LeaveTypeObj.ToDate)
                             || (a.ToDate > LeaveTypeObj.FromDate && a.ToDate < LeaveTypeObj.ToDate)
                             || (a.ToDate == LeaveTypeObj.FromDate))
                             ).FirstOrDefault();
                    // MODIFIED BY MSJ ON 19 JAN 2018 END

                    if (ODTourListObj != null)
                    {
                        #region Validate OD Data

                        // Code added by Rajas on 26 SEP 2017 START
                        if (ODTourListObj.FromDate == ODTourListObj.ToDate) // Single day
                        {
                            if (CurrentDate == ODTourListObj.FromDate)
                            {
                                if (ODTourListObj.ODDayStatus == 1)  // Full day in single day
                                {
                                    Message = "You Can not Apply Leave For " + LeaveTypeObj.FromDate.ToString("dd-MMM-yyyy") + " To " + LeaveTypeObj.ToDate.ToString("dd-MMM-yyyy") + " Range"
                                      + "<br/>" + "You may already have pending or approved/sanctioned OD for this date range.";
                                    IsSuccess = false;
                                }
                                else
                                {
                                    if (ODTourListObj.ODDayStatus == LeaveTypeObj.FromDayStatus)
                                    {
                                        Message = "You Can not Apply Leave For " + LeaveTypeObj.FromDate.ToString("dd-MMM-yyyy") + " To " + LeaveTypeObj.ToDate.ToString("dd-MMM-yyyy") + " Range"
                                      + "<br/>" + "You may already have pending or approved/sanctioned OD for this date range.";
                                        IsSuccess = false;
                                    }
                                    else
                                    {
                                        if (LeaveTypeObj.FromDayStatus == 1)
                                        {
                                            Message = "You Can not Apply Leave For " + LeaveTypeObj.FromDate.ToString("dd-MMM-yyyy") + " To " + LeaveTypeObj.ToDate.ToString("dd-MMM-yyyy") + " Range"
                                         + "<br/>" + "You may already have pending or approved/sanctioned OD for this date range.";
                                            IsSuccess = false;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (CurrentDate == ODTourListObj.FromDate)
                            {
                                if (ODTourListObj.ODDayStatus == 1)  // Full day in single day
                                {
                                    Message = "You Can not Apply Leave For " + LeaveTypeObj.FromDate.ToString("dd-MMM-yyyy") + " To " + LeaveTypeObj.ToDate.ToString("dd-MMM-yyyy") + " Range"
                                      + "<br/>" + "You may already have pending or approved/sanctioned OD for this date range.";
                                    IsSuccess = false;
                                }
                                else
                                {
                                    if (ODTourListObj.ODDayStatus == LeaveTypeObj.FromDayStatus)
                                    {
                                        Message = "You Can not Apply Leave For " + LeaveTypeObj.FromDate.ToString("dd-MMM-yyyy") + " To " + LeaveTypeObj.ToDate.ToString("dd-MMM-yyyy") + " Range"
                                      + "<br/>" + "You may already have pending or approved/sanctioned OD for this date range.";
                                        IsSuccess = false;
                                    }
                                    else
                                    {
                                        if (LeaveTypeObj.FromDayStatus == 1)
                                        {
                                            Message = "You Can not Apply Leave For " + LeaveTypeObj.FromDate.ToString("dd-MMM-yyyy") + " To " + LeaveTypeObj.ToDate.ToString("dd-MMM-yyyy") + " Range"
                                         + "<br/>" + "You may already have pending or approved/sanctioned OD for this date range.";
                                            IsSuccess = false;
                                        }
                                    }
                                }
                            }
                            else if (CurrentDate == ODTourListObj.ToDate)
                            {
                                if (ODTourListObj.ODDayStatus1 == 1)  // Full day in single day
                                {
                                    Message = "You Can not Apply Leave For " + LeaveTypeObj.FromDate.ToString("dd-MMM-yyyy") + " To " + LeaveTypeObj.ToDate.ToString("dd-MMM-yyyy") + " Range"
                                      + "<br/>" + "You may already have pending or approved/sanctioned OD for this date range.";
                                    IsSuccess = false;
                                }
                                else
                                {
                                    if (ODTourListObj.ODDayStatus == LeaveTypeObj.FromDayStatus)
                                    {
                                        Message = "You Can not Apply Leave For " + LeaveTypeObj.FromDate.ToString("dd-MMM-yyyy") + " To " + LeaveTypeObj.ToDate.ToString("dd-MMM-yyyy") + " Range"
                                      + "<br/>" + "You may already have pending or approved/sanctioned OD for this date range.";
                                        IsSuccess = false;
                                    }
                                }
                            }
                            else
                            {
                                Message = "You Can not Apply Leave For " + LeaveTypeObj.FromDate.ToString("dd-MMM-yyyy") + " To " + LeaveTypeObj.ToDate.ToString("dd-MMM-yyyy") + " Range"
                                        + "<br/>" + "You may already have pending or approved/sanctioned OD for this date range.";
                                IsSuccess = false;
                            }
                        }
                        // Code added by Rajas on 26 SEP 2017 END

                        #endregion Validate OD Data
                    }
                }
            }
            catch
            {
                IsSuccess = false;
            }
            return IsSuccess;

        }
        #endregion

        #region Code to check whether punch is available or not for Applied leave date
        /// <summary>
        /// ADDED BY SHRADDHA ON 21 JULY 2017
        /// TO CHECK WHETHER PUNCH IS AVAIALBLE OR NOT FOR SELECTED DATE RANGE AND SELECTED FROMDATE AND TODATE STATUS
        /// <param name="EmployeeId"></param>
        /// ASSUMPTION IS FROMDATE, TODATE AND TRNADATE ALL ARE 00:00:00 HOURS
        /// LeaveTypeObj.FromDayStatus==1 FULL DAY , LeaveTypeObj.FromDayStatus==2 FIRST HALF , LeaveTypeObj.FromDayStatus==3 SECOND HALF
        /// LeaveTypeObj.ToDayStatus==1 FULL DAY , LeaveTypeObj.ToDayStatus==2 FIRST HALF , LeaveTypeObj.ToDayStatus==3 SECOND HALF
        /// <returns></returns>
        public static bool IsPunchAvailable(WetosDBEntities WetosDB, int EmployeeId, LeaveModel LeaveTypeObj, ref string Message, ref bool IsSuccess)
        {
            IsSuccess = true;

            //TO GET DAILY TRANSACTION TABLE DATA FOR SELECTED DATE RANGE
            List<DailyTransaction> DailyTransactionList = WetosDB.DailyTransactions.Where(a => a.TranDate >= LeaveTypeObj.FromDate && a.TranDate <= LeaveTypeObj.ToDate
               && a.EmployeeId == EmployeeId).OrderBy(a => a.TranDate).ToList();
            foreach (var DailyTransactionObj in DailyTransactionList)
            {
                if (DailyTransactionObj.TranDate == LeaveTypeObj.FromDate)
                {
                    switch (LeaveTypeObj.FromDayStatus)
                    {
                        case 1:
                            if (DailyTransactionObj.Status.Contains("PP"))
                            {
                                IsSuccess = false;
                                Message = "You can't apply leave as your punch is already available for selected date range";
                            }
                            break;

                        case 2:
                            if (DailyTransactionObj.Status.Substring(0, 2) == "PP")
                            {
                                IsSuccess = false;
                                Message = "You can't apply leave as your punch is already available for selected date range";
                            }
                            break;

                        case 3:
                            if (DailyTransactionObj.Status.Substring(2, 2) == "PP")
                            {
                                IsSuccess = false;
                                Message = "You can't apply leave as your punch is already available for selected date range";
                            }
                            break;

                        default:
                            IsSuccess = false;
                            Message = "Please select proper from day status";
                            break;
                    }
                }

                if (DailyTransactionObj.TranDate == LeaveTypeObj.ToDate)
                {
                    switch (LeaveTypeObj.ToDayStatus)
                    {

                        case 1:
                            if (DailyTransactionObj.Status.Contains("PP"))
                            {
                                IsSuccess = false;
                                Message = "You can't apply leave as your punch is already available for selected date range";
                            }
                            break;

                        case 2:
                            if (DailyTransactionObj.Status.Substring(0, 2) == "PP")
                            {
                                IsSuccess = false;
                                Message = "You can't apply leave as your punch is already available for selected date range";
                            }
                            break;

                        case 3:
                            if (DailyTransactionObj.Status.Substring(2, 2) == "PP")
                            {
                                IsSuccess = false;
                                Message = "You can't apply leave as your punch is already available for selected date range";
                            }
                            break;

                        default:
                            IsSuccess = false;
                            Message = "Please select proper from day status";
                            break;
                    }
                }
            }
            return IsSuccess;
        }


        #endregion

        #region Code to Populate Employee Name Dropdown for selected EmployeeId
        public ActionResult AjaxPopulateEmployeeDropdown(int EmployeeId)
        {
            var EmployeeNameList = (from r in WetosDB.Employees

                                    where r.EmployeeId == EmployeeId
                                    select new { EmployeeName = r.FirstName + " " + r.MiddleName + " " + r.LastName, EmployeeId = r.EmployeeId }).FirstOrDefault();

            return Json(EmployeeNameList, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Code to get leave type list for selected Employee
        /// <summary>
        /// To fill leave type on selection of employee from dropdown
        /// Added by Rajas on 13 MARCH 2017
        /// </summary>
        /// <param name="Employeeid"></param>
        /// <returns></returns>
        public JsonResult GetLeaveTypeListForSelectedEmployee(String Employeeid)
        {
            int SelEmployeeId = 0;
            if (!string.IsNullOrEmpty(Employeeid))
            {
                if (Employeeid.ToUpper() != "NULL")
                {
                    SelEmployeeId = Convert.ToInt32(Employeeid);
                }
            }

            int EmployeeGroupid = WetosDB.EmployeeGroupDetails.Where(a => a.Employee.EmployeeId == SelEmployeeId).Select(a => a.EmployeeGroup.EmployeeGroupId).FirstOrDefault();

            var LeaveTypeList = WetosDB.LeaveMasters.Where(a => a.EmployeeGroup.EmployeeGroupId == EmployeeGroupid).Select(a => new { LeaveTypeID = a.LeaveId, LeaveType = a.LeaveCode }).ToList();

            return Json(LeaveTypeList);
        }
        #endregion

        #region Code To get leave application details for Sandwich case leave
        /// <summary>
        /// ADDED BY SHRADDHA ON 19 JULY 2017 
        /// TO GET LEAVEAPPLICATION EXTRA DETAILS SUCH AS ISLWP,ISPRESANDWICH, ISPOSTSANDWICH
        /// <param name="LeaveApplicationId"></param>
        /// <returns></returns>

        public ActionResult GetLeaveApplicationExtraDetails(int LeaveApplicationId)
        {
            LeaveApplication LeaveApplicationObj = WetosDB.LeaveApplications.Where(a => a.LeaveApplicationId == LeaveApplicationId).FirstOrDefault();

            return PartialView(LeaveApplicationObj);
        }

        #endregion

        #region Code for document attachment required or not
        //CODE ADDED BY SHRADDHA ON 16 FEB 2018 START
        public bool IsAttachmentRequiredOrNot(int LeaveId, double AppliedDays)
        {
            bool IsAttachmentRequiredFlag = false;

            LeaveMaster LeaveMasterObj = WetosDB.LeaveMasters.Where(a => a.LeaveId == LeaveId).FirstOrDefault();
            if (LeaveMasterObj != null)
            {
                bool IsAttachmentRequired = Convert.ToBoolean(LeaveMasterObj.IsAttachmentNeeded == null ? false : LeaveMasterObj.IsAttachmentNeeded);
                double AttachmentRequiredForMinNoOfLeave = Convert.ToDouble(LeaveMasterObj.AttachmentRequiredForMinNoOfLeave == null ? 0 : LeaveMasterObj.AttachmentRequiredForMinNoOfLeave);
                if (IsAttachmentRequired == true && AttachmentRequiredForMinNoOfLeave <= AppliedDays)
                {
                    IsAttachmentRequiredFlag = true;
                }
            }
            return IsAttachmentRequiredFlag;
        }
        //CODE ADDED BY SHRADDHA ON 16 FEB 2018 END
        #endregion
        #endregion


        #region Leave Sanction

        #region Leave Sanction fake logic for filered leave Processing
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
        #endregion

        #region Leave Sanction Index View
        /// <summary>
        /// 
        /// </summary>
        /// <param name="LeaveSanction"></param>
        /// <param name="FC"></param>
        /// <returns></returns>
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
                                    //MODIFIED BY PUSHKAR ON 27 MARCH 2018
                                    AllowedLeaves = LeaveDataObj == null ? 0.0 : LeaveDataObj.CurrentBalance == null ? 0.0 : LeaveDataObj.CurrentBalance.Value;

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

                DateTime CalanderStartDate = GetFYStartDate(); // DateTime CalanderStartDate = GetFYStartDate(); // DateTime CalanderStartDate = WetosDB.FinancialYears.Where(a => a.FinancialName == GlobalSettingObj.SettingValue).Select(a => a.StartDate).FirstOrDefault();

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
        #endregion



        #region Code to check whether sandwich case is available for applied leave date while sanctioning leave
        /// <summary>
        /// TO GET SANDWICH DAYS ON WETOS SANCTION
        /// </summary>
        /// <param name="WetosDB"></param>
        /// <param name="EmpId"></param>
        /// <param name="ErrorMessage"></param>
        /// <param name="LeaveApplicationObj"></param>
        /// <returns></returns>
        public static bool IsSandwichforLeaveSanction(WetosDBEntities WetosDB, int EmpId, ref string ErrorMessage, ref LeaveApplication LeaveApplicationObj)
        {
            bool ReturnStatus = false;
            try
            {
                Employee SelectedEmp = WetosDB.Employees.Where(a => a.EmployeeId == EmpId).FirstOrDefault();

                int EmployeeGroupId = WetosDB.EmployeeGroupDetails.Where(a => a.Employee.EmployeeId == EmpId).Select(a => a.EmployeeGroup.EmployeeGroupId).FirstOrDefault();
                string LeaveTypeStr = LeaveApplicationObj.LeaveType;

                //LEAVE MODEL OBJ START
                LeaveModel LeaveTypeObj = new LeaveModel();
                LeaveTypeObj.ActualDays = LeaveApplicationObj.ActualDays == null ? 0.0d : LeaveApplicationObj.ActualDays.Value;
                LeaveTypeObj.AppliedDays = LeaveApplicationObj.AppliedDays == null ? 0.0d : LeaveApplicationObj.AppliedDays.Value;
                LeaveTypeObj.EmployeeId = LeaveApplicationObj.EmployeeId;
                LeaveTypeObj.FromDate = LeaveApplicationObj.FromDate;
                LeaveTypeObj.FromDayStatus = LeaveApplicationObj.FromDayStatus;
                LeaveTypeObj.IsLWP = LeaveApplicationObj.IsLWP == null ? false : Convert.ToBoolean(LeaveApplicationObj.IsLWP);
                LeaveTypeObj.IsPostSandwichCase = LeaveApplicationObj.IsPostSandwichCase == null ? false : Convert.ToBoolean(LeaveApplicationObj.IsPostSandwichCase);
                LeaveTypeObj.IsPreSandwichCase = LeaveApplicationObj.IsPreSandwichCase == null ? false : Convert.ToBoolean(LeaveApplicationObj.IsPreSandwichCase);
                LeaveTypeObj.LeaveApplicationId = LeaveApplicationObj.LeaveApplicationId;
                LeaveTypeObj.LeaveConsumed = LeaveApplicationObj.LeaveConsumed == null ? 0.0d : LeaveApplicationObj.LeaveConsumed.Value;
                LeaveTypeObj.LeaveId = WetosDB.LeaveMasters.Where(a => a.LeaveCode.Trim().ToUpper() == LeaveTypeStr.Trim().ToUpper()
                                        && a.EmployeeGroup.EmployeeGroupId == EmployeeGroupId).Select(a => a.LeaveId).FirstOrDefault();
                LeaveTypeObj.LWP = LeaveApplicationObj.LWP == null ? 0.0d : LeaveApplicationObj.LWP.Value;

                LeaveTypeObj.MarkedAsDelete = LeaveApplicationObj.MarkedAsDelete;
                //LeaveTypeObj.PostSandwitchDays = LeaveApplicationObj.PostSandwichdays == null ? 0.0d : LeaveApplicationObj.PostSandwichdays.Value;
                LeaveTypeObj.PostSandwitchCaseLeaveId = LeaveApplicationObj.PostSandwichCaseLeaveID == null ? 0 : LeaveApplicationObj.PostSandwichCaseLeaveID.Value;
                LeaveTypeObj.PreSandwitchCaseLeaveId = LeaveApplicationObj.PreSandwichCaseLeaveID == null ? 0 : LeaveApplicationObj.PreSandwichCaseLeaveID.Value;
                //LeaveTypeObj.PreSandwitchDays = LeaveApplicationObj.PreSandwichdays == null ? 0.0d : LeaveApplicationObj.PreSandwichdays.Value;
                LeaveTypeObj.Purpose = LeaveApplicationObj.Purpose;
                LeaveTypeObj.RejectReason = LeaveApplicationObj.RejectReason;
                LeaveTypeObj.Status = LeaveApplicationObj.Status;
                LeaveTypeObj.StatusId = LeaveApplicationObj.StatusId == null ? 0 : LeaveApplicationObj.StatusId.Value;
                LeaveTypeObj.ToDate = LeaveApplicationObj.ToDate;
                LeaveTypeObj.ToDayStatus = LeaveApplicationObj.ToDayStatus;
                LeaveTypeObj.TotalDeductableDays = LeaveApplicationObj.TotalDeductableDays == null ? 0.0d : LeaveApplicationObj.TotalDeductableDays.Value;
                //LEAVE MODEL OBJ END


                int LeaveTypeInt = LeaveTypeObj.LeaveId;

                string LeaveType = WetosDB.LeaveMasters.Where(a => a.LeaveId == LeaveTypeInt).Select(a => a.LeaveCode).FirstOrDefault();

                LeaveMaster LeaveMasterObj = WetosDB.LeaveMasters.Where(a => a.LeaveCode == LeaveType && a.EmployeeGroup.EmployeeGroupId == EmployeeGroupId).FirstOrDefault();

                string AppliedLeaveType = string.Empty;

                //GENERIC FUNCTION USED BY SHRADDHA ON 20 JULY 2017
                double AppliedDay = CalculateAppliedDays(LeaveTypeObj.FromDate, LeaveTypeObj.ToDate, LeaveTypeObj.FromDayStatus, LeaveTypeObj.ToDayStatus);
                double AppliedDayEx = AppliedDay;

                DateTime CombinationCaseLeaveDate = DateTime.Now;
                //bool IsLeaveCombine = false;
                //string SandwitchCaseDetected = collection["SandwitchCase"];

                //--------------------------------------------------------------------------------------------------------------------------------------
                // 2. IS PRVE DATE SANDWITCH CASE
                //--------------------------------------------------------------------------------------------------------------------------------------
                #region PRESANDWITH
                //  NEW LOGIC START

                //first date
                DateTime TempCurrent = LeaveTypeObj.FromDate;
                DateTime TempPrev = TempCurrent;

                bool IsAppDayOkay = false;
                bool ContinueFlag = false;
                bool IsHoliday = false;
                bool IsWO = false;

                int SandwichDays = 0;

                for (int pi = 0; pi < 30; pi--)
                {
                    TempCurrent = TempPrev;
                    TempPrev = TempPrev.AddDays(-1);

                    if (LeaveMasterObj.HHBetLevConsiderLeave == true)
                    {
                        // HOLODAY
                        HoliDay PrevHoliDay = WetosDB.HoliDays.Where(a => a.FromDate == TempPrev).FirstOrDefault();

                        ContinueFlag = false;

                        if (PrevHoliDay != null)
                        {

                            ContinueFlag = true;
                            IsHoliday = true;
                            SandwichDays++;

                        }
                    }

                    if (LeaveMasterObj.WOBetLevConsiderLeave == true)
                    {
                        // WEEKOFF 1
                        if (ContinueFlag == false && TempPrev.DayOfWeek.ToString().ToUpper() == SelectedEmp.WeeklyOff1)
                        {
                            ContinueFlag = true;
                            IsWO = true;
                            SandwichDays++;

                        }

                        // Validate Weekoff2 as per assigned to employee
                        if (ContinueFlag == false && TempPrev.DayOfWeek.ToString().ToUpper() == SelectedEmp.WeeklyOff2)
                        {
                            if (!string.IsNullOrEmpty(SelectedEmp.WeeklyOff2))
                            {
                                int WeekNumber = GetWeekOfMonth(TempPrev);

                                if (SelectedEmp.First == true && WeekNumber == 1)
                                {
                                    ContinueFlag = true;
                                    IsWO = true;
                                    SandwichDays++;
                                }
                                else if (SelectedEmp.Second == true && WeekNumber == 2)
                                {
                                    ContinueFlag = true;
                                    IsWO = true;
                                    SandwichDays++;
                                }
                                else if (SelectedEmp.Third == true && WeekNumber == 3)
                                {
                                    ContinueFlag = true;
                                    IsWO = true;
                                    SandwichDays++;
                                }
                                else if (SelectedEmp.Fourth == true && WeekNumber == 4)
                                {
                                    ContinueFlag = true;
                                    IsWO = true;
                                    SandwichDays++;
                                }
                                else if (SelectedEmp.Fifth == true && WeekNumber == 5)
                                {
                                    ContinueFlag = true;
                                    IsWO = true;
                                    SandwichDays++;
                                }
                            }
                        }

                    }

                    if (!ContinueFlag)
                    {
                        //MODIFIED BY PUSHKAR ON 24 MAY 2018 FOR ALL STATUS 3-REJ BY APP / 5- CANCELLED / 6- REJ BY SANC
                        List<LeaveApplication> LeaveApplicationABCD = WetosDB.LeaveApplications.Where(a => a.EmployeeId == SelectedEmp.EmployeeId
                             && a.FromDate <= TempPrev && a.ToDate >= TempPrev && a.MarkedAsDelete == 0 && a.StatusId != 3 && a.StatusId != 6 && a.StatusId != 5).ToList();

                        if (LeaveApplicationABCD.Count <= 0)
                        {
                            IsAppDayOkay = true;
                        }
                        else
                        {
                            foreach (var Leaves in LeaveApplicationABCD)
                            {
                                if (Leaves.FromDayStatus == 1 && LeaveTypeObj.FromDayStatus == 3)
                                {
                                    IsAppDayOkay = true;
                                }
                                else if (Leaves.FromDayStatus == 2 && LeaveTypeObj.FromDayStatus == 1)
                                {
                                    IsAppDayOkay = true;
                                }
                                else if (Leaves.FromDayStatus == 3 && LeaveTypeObj.FromDayStatus == 3)
                                {
                                    IsAppDayOkay = true;
                                }
                                else if (Leaves.FromDayStatus == 2 && LeaveTypeObj.FromDayStatus == 2)
                                {
                                    IsAppDayOkay = true;
                                }
                                else
                                {
                                    IsAppDayOkay = false;
                                    AppliedLeaveType = Leaves.LeaveType.Trim();

                                    //CODE ADDED BY SHRADDHA ON 28 NOV 2017 START
                                    CombinationCaseLeaveDate = Leaves.ToDate;
                                    //CODE ADDED BY SHRADDHA ON 28 NOV 2017 END
                                }

                            }
                        }

                        break;
                    }
                }

                //if (SandwitchCaseDetected != "true")
                {
                    if (!IsAppDayOkay && (IsHoliday == true || IsWO == true))
                    {
                        LeaveTypeObj.IsPreSandwichCase = true;


                        //MODIFIED BY MSJ ON 24 JULY 2017 
                        LeaveTypeObj.PreSandwitchDays = SandwichDays; // ((LeaveTypeObj.FromDate.AddDays(-1)) - (TempPrev.AddDays(1))).TotalDays;

                        LeaveTypeObj.TotalDeductableDays = AppliedDay + LeaveTypeObj.PreSandwitchDays;

                        ErrorMessage = "Sandwitch case detected in Start date - Total " + "<b>" + LeaveTypeObj.TotalDeductableDays + "</b>" + " Days would be consider as Leave." + "<br/>"
                            + "Do you really want to Continue?";


                        //LeaveTypeObj.PreSandwichStartDate = TempPrev.AddDays(1);
                        //LeaveTypeObj.PreSandwichEndDate = LeaveTypeObj.FromDate.AddDays(-1);

                        //LeaveTypeObj


                        //return false;
                    }
                }
                #endregion

                #region CHECK LEAVE COMBINATION IS ALLOWED OR NOT


                string PrefixComb = LeaveMasterObj.Prefix;
                int Pre = 1;

                // Added by Rajas on 14 AUGUST 2017 START
                string Message = string.Empty;
                bool IsSuccess = false;
                //if (IsLeaveCombination(LeaveMasterObj, AppliedLeaveType, IsAppDayOkay, ref Message, ref IsSuccess) == false) //COMMENTED CODE BY SHRADDHA ON 28 NOV 2017
                if (IsLeaveCombination(WetosDB, LeaveMasterObj, LeaveTypeObj, CombinationCaseLeaveDate, AppliedLeaveType, IsAppDayOkay, PrefixComb, Pre, ref Message, ref IsSuccess) == false)  //ADDED CODE BY SHRADDHA ON 28 NOV 2017
                {
                    //IsLeaveCombine = true;
                    ErrorMessage = Message;
                    return ReturnStatus = false;
                }
                // Added by Rajas on 14 AUGUST 2017 END

                #endregion

                //------------------------------------------------------------------------------------------------------------------------------
                // 3.IS NEXT DATE SANDWITCH CASE
                //------------------------------------------------------------------------------------------------------------------------------
                #region POST SANDWITCH
                // last day
                TempCurrent = LeaveTypeObj.ToDate;
                DateTime TempNext = TempCurrent;

                IsAppDayOkay = false;
                ContinueFlag = false;
                IsHoliday = false;
                IsWO = false;

                for (int pi = 0; pi < 30; pi++)
                {
                    TempCurrent = TempPrev;
                    TempNext = TempNext.AddDays(+1);

                    // Validate HHBetLevConsiderLeave rule assigned in leave rule
                    if (LeaveMasterObj.HHBetLevConsiderLeave == true)
                    {
                        // HOLODAY
                        HoliDay PrevHoliDay = WetosDB.HoliDays.Where(a => a.FromDate == TempNext).FirstOrDefault();

                        ContinueFlag = false;

                        if (PrevHoliDay != null)
                        {
                            ContinueFlag = true;
                            IsHoliday = true;

                            // If Sandwich already detected then Next day condition checking should be skipped
                            // Discussion with Katre sir on 4 JULY 2017
                            // Updated by Rajas on 4 JULY 2017
                            //if (SandwitchCaseDetected != "true")
                            {
                                SandwichDays++;
                            }
                        }
                    }


                    // Validate WOBetLevConsiderLeave rule assigned in leave rule
                    if (LeaveMasterObj.WOBetLevConsiderLeave == true)
                    {
                        // WEEKOFF 1
                        if (ContinueFlag == false && TempNext.DayOfWeek.ToString().ToUpper() == SelectedEmp.WeeklyOff1)
                        {
                            ContinueFlag = true;
                            IsWO = true;
                            SandwichDays++;
                        }

                        // Validate Weekoff2 as per assigned to employee
                        if (ContinueFlag == false && TempNext.DayOfWeek.ToString().ToUpper() == SelectedEmp.WeeklyOff2)
                        {
                            if (!string.IsNullOrEmpty(SelectedEmp.WeeklyOff2))
                            {
                                int WeekNumber = GetWeekOfMonth(TempNext);

                                if (SelectedEmp.First == true && WeekNumber == 1)
                                {
                                    ContinueFlag = true;
                                    IsWO = true;
                                    SandwichDays++;
                                }
                                else if (SelectedEmp.Second == true && WeekNumber == 2)
                                {
                                    ContinueFlag = true;
                                    IsWO = true;
                                    SandwichDays++;
                                }
                                else if (SelectedEmp.Third == true && WeekNumber == 3)
                                {
                                    ContinueFlag = true;
                                    IsWO = true;
                                    SandwichDays++;
                                }
                                else if (SelectedEmp.Fourth == true && WeekNumber == 4)
                                {
                                    ContinueFlag = true;
                                    IsWO = true;
                                    SandwichDays++;
                                }
                                else if (SelectedEmp.Fifth == true && WeekNumber == 5)
                                {
                                    ContinueFlag = true;
                                    IsWO = true;
                                    SandwichDays++;
                                }
                            }
                        }

                    }

                    if (!ContinueFlag)
                    {
                        //MODIFIED BY PUSHKAR ON 24 MAY 2018 FOR ALL STATUS 3-REJ BY APP / 5- CANCELLED / 6- REJ BY SANC
                        List<LeaveApplication> LeaveApplicationList = WetosDB.LeaveApplications.Where(a => a.EmployeeId == SelectedEmp.EmployeeId && a.FromDate <= TempNext
                            && a.ToDate >= TempNext && a.MarkedAsDelete == 0 && a.StatusId != 3 && a.MarkedAsDelete == 0 && a.StatusId != 5 && a.StatusId != 6).ToList();
                        if (LeaveApplicationList.Count == 0)
                        {
                            IsAppDayOkay = true;
                        }
                        else
                        {
                            foreach (var Leaves in LeaveApplicationList)
                            {
                                if (Leaves.ToDayStatus == 3 && LeaveTypeObj.ToDayStatus == 3)
                                {
                                    IsAppDayOkay = true;
                                }
                                else if (Leaves.ToDayStatus == 1 && LeaveTypeObj.ToDayStatus == 2)
                                {
                                    IsAppDayOkay = true;
                                }
                                else if (Leaves.ToDayStatus == 2 && LeaveTypeObj.FromDayStatus == 2)   // Updated by Rajas on 21 JULY 2017
                                {
                                    IsAppDayOkay = true;
                                }
                                else
                                {
                                    IsAppDayOkay = false;
                                    AppliedLeaveType = Leaves.LeaveType.Trim();


                                    //CODE ADDED BY SHRADDHA ON 28 NOV 2017 START
                                    CombinationCaseLeaveDate = Leaves.FromDate;
                                    //CODE ADDED BY SHRADDHA ON 28 NOV 2017 END
                                }

                            }
                        }

                        break;
                    }
                }

                //if (SandwitchCaseDetected != "true")
                {
                    if (!IsAppDayOkay && (IsHoliday == true || IsWO == true))
                    {
                        // ADDED BY MSJ ON 19 JULY 2017 START
                        LeaveTypeObj.IsPostSandwichCase = true;

                        // MODIFIED BY MSJ ON 24 JULY 2017 START
                        LeaveTypeObj.PostSandwitchDays = SandwichDays;

                        LeaveTypeObj.TotalDeductableDays = AppliedDay + LeaveTypeObj.PostSandwitchDays;

                        ErrorMessage = "Sandwitch case detected in End date - Total " + "<b>" + LeaveTypeObj.TotalDeductableDays + "</b>" + " Days would be consider as Leave." + "<br/>"
                            + "Do you really want to Continue?";


                        //((TempNext.AddDays(-1)) - (LeaveTypeObj.ToDate.AddDays(1))).TotalDays;
                        //LeaveTypeObj.PostSandwichStartDate = LeaveTypeObj.ToDate.AddDays(+1);
                        //LeaveTypeObj.PostSandwichEndDate = TempNext.AddDays(-1);
                        // ADDED BY MSJ ON 19 JULY 2017 END

                        //return false;
                    }
                }

                #endregion


                #region CHECK LEAVE COMBINATION IS ALLOWED OR NOT


                string SuffixComb = LeaveMasterObj.Suffix;
                int Suff = 2;

                // Added by Rajas on 14 AUGUST 2017 START
                Message = string.Empty;
                IsSuccess = false;
                //if (IsLeaveCombination(LeaveMasterObj, AppliedLeaveType, IsAppDayOkay, ref Message, ref IsSuccess) == false) //COMMENTED CODE BY SHRADDHA ON 28 NOV 2017
                if (IsLeaveCombination(WetosDB, LeaveMasterObj, LeaveTypeObj, CombinationCaseLeaveDate, AppliedLeaveType, IsAppDayOkay, SuffixComb, Suff, ref Message, ref IsSuccess) == false)  //ADDED CODE BY SHRADDHA ON 28 NOV 2017
                {
                    //IsLeaveCombine = true;
                    ErrorMessage = Message;
                    return ReturnStatus = false;
                }
                // Added by Rajas on 14 AUGUST 2017 END

                #endregion


                #region IS WEEK OFF IN DATERANGE
                // ADDED BY MSJ on  29 APRIL 2018
                bool IsSandwitchCaseInDateRange = false;
                double SandwitchCaseInDateRangeDays = 0.0;
                double SandwitchCaseInDateRangeDaysHol = 0.0;
                //if (SandwitchCaseDetected != "true")
                {
                    #region SANDWICH CASE BETWEEN SELECTED FROM AND TO DATE

                    if (LeaveTypeObj.FromDate != LeaveTypeObj.ToDate)
                    {
                        TempCurrent = LeaveTypeObj.FromDate;
                        TempNext = TempCurrent;

                        IsAppDayOkay = false;

                        int IsSandwich = 0;
                        int IsHolidayDeduct = 0;

                        for (int pi = 1; pi <= AppliedDay; pi++)
                        {
                            TempNext = TempNext.AddDays(+1);

                            if (TempNext > LeaveTypeObj.ToDate)
                            {
                                break;
                            }

                            ContinueFlag = false;

                            // HOLODAY
                            HoliDay PrevHoliDay = WetosDB.HoliDays.Where(a => a.FromDate == TempNext && a.Branchid == SelectedEmp.BranchId).FirstOrDefault();


                            if (PrevHoliDay != null)
                            {
                                ContinueFlag = true;
                                IsHolidayDeduct++;

                            }
                            // WEEKOFF 1
                            if (ContinueFlag == false && TempNext.DayOfWeek.ToString().ToUpper() == SelectedEmp.WeeklyOff1)
                            {
                                ContinueFlag = true;
                                IsSandwich++;

                            }
                            // WeekOff 2 (2nd forth)
                            if (ContinueFlag == false && TempNext.DayOfWeek.ToString().ToUpper() == SelectedEmp.WeeklyOff2)
                            {
                                int WeekNumber = GetWeekOfMonth(TempNext);

                                if (SelectedEmp.First == true && WeekNumber == 1)
                                {
                                    ContinueFlag = true;
                                    IsSandwich++;
                                }
                                else if (SelectedEmp.Second == true && WeekNumber == 2)
                                {
                                    ContinueFlag = true;
                                    IsSandwich++;
                                }
                                else if (SelectedEmp.Third == true && WeekNumber == 3)
                                {
                                    ContinueFlag = true;
                                    IsSandwich++;
                                }
                                else if (SelectedEmp.Fourth == true && WeekNumber == 4)
                                {
                                    ContinueFlag = true;
                                    IsSandwich++;
                                }
                                else if (SelectedEmp.Fifth == true && WeekNumber == 5)
                                {
                                    ContinueFlag = true;
                                    IsSandwich++;
                                }
                            }

                        }

                        if (!ContinueFlag && IsSandwich == 0 && IsHolidayDeduct == 0)
                        {
                            if (WetosDB.LeaveApplications.Where(a => a.EmployeeId == SelectedEmp.EmployeeId && a.FromDate >= TempNext
                                && a.ToDate <= TempNext).ToList().Count == 0)
                            {
                                IsAppDayOkay = true;
                            }

                            //break;
                        }


                        if (!IsAppDayOkay)
                        {
                            IsSandwitchCaseInDateRange = true; // ADDED BY MSJ ON 24 JULY 2017 

                            // ERROR
                            ErrorMessage = "Sandwitch case detected - You Can not Apply Leave For " + LeaveTypeObj.FromDate.ToString("dd-MMM-yyyy")
                                              + " To " + LeaveTypeObj.ToDate.ToString("dd-MMM-yyyy") + " Range";

                            // ADDED BY MSJ ON 24 JULY 2017
                            SandwitchCaseInDateRangeDays = IsSandwich;
                            SandwitchCaseInDateRangeDaysHol = IsHolidayDeduct;

                            if (LeaveMasterObj.WOBetLevConsiderLeave == false)
                            {
                                AppliedDay = AppliedDay - SandwitchCaseInDateRangeDays;
                            }

                            //ADDED BY PUSHKAR ON 25 MAY 2018 FOR ISHOLIDAY DEDUCT LEAVE RULE

                            if (LeaveMasterObj.HHBetLevConsiderLeave == false)
                            {
                                AppliedDay = AppliedDay - SandwitchCaseInDateRangeDaysHol;
                            }
                        }
                    }

                    #endregion
                }

                // TOTALS DAYS

                #endregion COMMENTED CODE


                LeaveTypeObj.TotalDeductableDays = AppliedDay + LeaveTypeObj.PostSandwitchDays + LeaveTypeObj.PreSandwitchDays;// + SandwitchCaseInDateRangeDays; // MODIFIED BY MSJ ON 24 JULY 2017
                LeaveTypeObj.AppliedDays = AppliedDayEx;
                if (LeaveTypeObj.IsPreSandwichCase == true || LeaveTypeObj.IsPostSandwichCase == true)//|| IsSandwitchCaseInDateRange) // ADDED IsSandwitchCaseInDateRange
                {
                    ReturnStatus = false;
                }
                else
                {
                    ReturnStatus = true;
                }


                // MODIFIED BY MSJ ON 23 JAN 2020 START
                //LEAVE APPLICATION OBJ START
                LeaveApplicationObj.ActualDays = LeaveTypeObj.ActualDays; // LeaveTypeObj.ActualDays == null ? 0.0d : LeaveTypeObj.ActualDays;
                LeaveApplicationObj.AppliedDays = LeaveTypeObj.AppliedDays; // LeaveTypeObj.AppliedDays == null ? 0.0d : LeaveTypeObj.AppliedDays;
                LeaveApplicationObj.EmployeeId = LeaveTypeObj.EmployeeId;
                LeaveApplicationObj.FromDate = LeaveTypeObj.FromDate;
                LeaveApplicationObj.FromDayStatus = LeaveTypeObj.FromDayStatus;
                LeaveApplicationObj.IsLWP = Convert.ToBoolean(LeaveTypeObj.IsLWP); // LeaveTypeObj.IsLWP == null ? false : Convert.ToBoolean(LeaveTypeObj.IsLWP);
                LeaveApplicationObj.IsPostSandwichCase = Convert.ToBoolean(LeaveTypeObj.IsPostSandwichCase);  // LeaveTypeObj.IsPostSandwichCase == null ? false : Convert.ToBoolean(LeaveTypeObj.IsPostSandwichCase);
                LeaveApplicationObj.IsPreSandwichCase = Convert.ToBoolean(LeaveTypeObj.IsPreSandwichCase); //LeaveTypeObj.IsPreSandwichCase == null ? false : Convert.ToBoolean(LeaveTypeObj.IsPreSandwichCase);
                LeaveApplicationObj.LeaveApplicationId = LeaveTypeObj.LeaveApplicationId;
                LeaveApplicationObj.LeaveConsumed = LeaveTypeObj.LeaveConsumed;  //LeaveTypeObj.LeaveConsumed == null ? 0.0d : LeaveTypeObj.LeaveConsumed;
                LeaveApplicationObj.LWP = LeaveTypeObj.LWP; // LeaveTypeObj.LWP == null ? 0.0d : LeaveTypeObj.LWP;
                LeaveApplicationObj.MarkedAsDelete = LeaveTypeObj.MarkedAsDelete == null ? 0 : LeaveTypeObj.MarkedAsDelete.Value;
                LeaveApplicationObj.PostSandwichdays = LeaveTypeObj.PostSandwitchDays; //LeaveTypeObj.PostSandwitchDays == null ? 0.0d : LeaveTypeObj.PostSandwitchDays;
                LeaveApplicationObj.PostSandwichCaseLeaveID = LeaveTypeObj.PostSandwitchCaseLeaveId; //LeaveTypeObj.PostSandwitchCaseLeaveId == null ? 0 : LeaveTypeObj.PostSandwitchCaseLeaveId;
                LeaveApplicationObj.PreSandwichCaseLeaveID = LeaveTypeObj.PreSandwitchCaseLeaveId; // LeaveTypeObj.PreSandwitchCaseLeaveId == null ? 0 : LeaveTypeObj.PreSandwitchCaseLeaveId;
                LeaveApplicationObj.PreSandwichdays = LeaveTypeObj.PreSandwitchDays; //LeaveTypeObj.PreSandwitchDays == null ? 0.0d : LeaveTypeObj.PreSandwitchDays;
                LeaveApplicationObj.Purpose = LeaveTypeObj.Purpose;
                LeaveApplicationObj.RejectReason = LeaveTypeObj.RejectReason;
                LeaveApplicationObj.Status = LeaveTypeObj.Status;
                LeaveApplicationObj.StatusId = LeaveTypeObj.StatusId; //LeaveTypeObj.StatusId == null ? 0 : LeaveTypeObj.StatusId;
                LeaveApplicationObj.ToDate = LeaveTypeObj.ToDate;
                LeaveApplicationObj.ToDayStatus = LeaveTypeObj.ToDayStatus;
                LeaveApplicationObj.TotalDeductableDays = LeaveTypeObj.TotalDeductableDays; // LeaveTypeObj.TotalDeductableDays == null ? 0.0d : LeaveTypeObj.TotalDeductableDays;
                //LEAVE APPLICATION OBJ END
                // MODIFIED BY MSJ ON 23 JAN 2020 END

                return ReturnStatus;
            }
            catch
            {
                return ReturnStatus;
            }
        }
        #endregion

        #region Leave Sanction Partial View
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

                DateTime CalanderStartDate = GetFYStartDate(); // DateTime CalanderStartDate = GetFYStartDate(); // DateTime CalanderStartDate = WetosDB.FinancialYears.Where(a => a.FinancialName == GlobalSettingObj.SettingValue).Select(a => a.StartDate).FirstOrDefault();

                if (CalanderStartDate != null)
                {

                    //CODE REPLACED BY SHRADDHA ON 12 SEP 2017 REMOVED REDUNDANT AND UNNECESSARY IF ELSE CODE AND TAKEN ONE SINGLE LINE CODE START
                    //Status = selectCriteria;
                    //LeaveSanctionList = WetosDB.SP_LeaveSanctionIndex(EmpNo).Where(a => ((a.EmployeeReportingId == EmpNo) && a.Id == Status && a.FromDate >= CalanderStartDate && (a.MarkedAsDelete == 0 || a.MarkedAsDelete == null)))
                    //        .OrderByDescending(a => a.FromDate).OrderByDescending(a => a.ToDate).ToList();
                    //CODE REPLACED BY SHRADDHA ON 12 SEP 2017 REMOVED REDUNDANT AND UNNECESSARY IF ELSE CODE AND TAKEN ONE SINGLE LINE CODE END

                    // Above code commented. Error was list for reportingId2 was not displayed
                    // Code updated by Rajas on 22 SEP 2017 START
                    if (selectCriteria == 1)  // Pending
                    {
                        Status = 1;

                        // Updated by Rajas on 7 JUNE 2017, ( || a.EmployeeReportingId2 == EmpNo) removed
                        LeaveSanctionList = WetosDB.SP_LeaveSanctionIndex(EmpNo).Where(a => ((a.EmployeeReportingId == EmpNo) && a.Id == Status
                            && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null))
                            .OrderByDescending(a => a.FromDate).ToList();  // && a.Id == 4)
                    }
                    else if (selectCriteria == 2)  // Sanction
                    {
                        Status = 2;

                        LeaveSanctionList = WetosDB.SP_LeaveSanctionIndex(EmpNo)
                            .Where(a => a.EmployeeReportingId2 == EmpNo && a.Id == Status && a.FromDate >= CalanderStartDate
                                && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null)
                            .OrderByDescending(a => a.FromDate).ToList();  // && a.Id == 4)
                    }
                    else if (selectCriteria == 3) // Rejected by Approver
                    {
                        Status = 3;

                        LeaveSanctionList = WetosDB.SP_LeaveSanctionIndex(EmpNo)
                            .Where(a => ((a.EmployeeReportingId == EmpNo || a.EmployeeReportingId2 == EmpNo)
                                && a.Id == Status && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null) || (a.EmployeeReportingId2 == EmpNo
                                && a.Id == Status && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null))
                                .OrderByDescending(a => a.FromDate).ToList();  // && a.Id == 4)
                    }
                    else if (selectCriteria == 4)  // Approve
                    {
                        Status = 4;

                        LeaveSanctionList = WetosDB.SP_LeaveSanctionIndex(EmpNo)
                            .Where(a => ((a.EmployeeReportingId2 == EmpNo || a.EmployeeReportingId == EmpNo)
                                && a.Id == Status && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null))
                                .OrderByDescending(a => a.FromDate).ToList();  // && a.Id == 4)
                    }
                    else if (selectCriteria == 5)  // Cancel
                    {
                        Status = 5;

                        LeaveSanctionList = WetosDB.SP_LeaveSanctionIndex(EmpNo)
                            .Where(a => ((a.EmployeeReportingId == EmpNo || a.EmployeeReportingId2 == EmpNo) && a.Id == Status
                                && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null))
                                .OrderByDescending(a => a.FromDate).ToList();  // && a.Id == 4)
                    }
                    else  // Rejected by Sanctioner
                    {
                        Status = 6;
                        LeaveSanctionList = WetosDB.SP_LeaveSanctionIndex(EmpNo)
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
        #endregion

        /// <summary>
        /// ADDED BY SHRADDHA ON 18 MAY 2017
        /// </summary>
        /// <param name="EmpId"></param>
        /// <param name="fc"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LeaveDataViewForLeaveBalance(int EmpId, FormCollection fc)
        {
            try
            {
                Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == EmpId).FirstOrDefault();
                EmployeeGroupDetail EmployeeGroupDetailObj = WetosDB.EmployeeGroupDetails.Where(a => a.Employee.EmployeeId == EmpId).FirstOrDefault();

                if (EmployeeObj != null && EmployeeGroupDetailObj != null)  // Updated by Rajas on 23 FEB 2017, added && EmployeeGroupDetailObj != null
                {
                    //MODIFIED BY PUSHKAR ON 28 NOV 2017 FOR CHECKING MARK AS DELETE CONDITION
                    var LeaveMasterList = WetosDB.LeaveMasters.Where(a => a.Company.CompanyId == EmployeeObj.CompanyId && a.BranchId == EmployeeObj.BranchId
                        && a.EmployeeGroup.EmployeeGroupId == EmployeeGroupDetailObj.EmployeeGroup.EmployeeGroupId
                        && (a.MarkedAsDelete == 0 || a.MarkedAsDelete == null)).Select(a => new { LeaveTypeID = a.LeaveId, LeaveType = a.LeaveCode }).ToList();

                    ViewBag.LeaveTypeList = new SelectList(LeaveMasterList, "LeaveTypeID", "LeaveType").ToList();

                    if (LeaveMasterList.Count <= 0)
                    {
                        ModelState.AddModelError("", " Please create leave rules for the particular Employee group");
                    }

                    List<SP_LeaveTableData_Result> LeaveBalanceList = WetosDB.SP_LeaveTableData(EmpId).ToList();

                    #region CODE ADDED BY SHRADDHA ON 29 MAR 2018
                    string YearInfo = Session["CurrentFinancialYear"].ToString();
                    DateTime FromDate = new DateTime(DateTime.Now.Year, 1, 1);
                    DateTime ToDate = new DateTime((DateTime.Now.Year + 1), 1, 1);
                    try
                    {
                        if (string.IsNullOrEmpty(YearInfo))
                        {
                            try
                            {
                                FromDate = WetosDB.FinancialYears.Where(a => a.FinancialName == YearInfo).Select(a => a.StartDate).FirstOrDefault();
                                ToDate = WetosDB.FinancialYears.Where(a => a.FinancialName == YearInfo).Select(a => a.EndDate).FirstOrDefault();
                            }
                            catch
                            {
                                FromDate = new DateTime(DateTime.Now.Year, 1, 1);
                                ToDate = new DateTime((DateTime.Now.Year + 1), 1, 1);
                            }
                        }
                    }
                    catch
                    {
                        FromDate = new DateTime(DateTime.Now.Year, 1, 1);
                        ToDate = new DateTime((DateTime.Now.Year + 1), 1, 1);
                    }
                    #endregion
                    List<SP_LeaveApplicationDetailsList_Result> LeaveApplicationDetailsList = WetosDB.SP_LeaveApplicationDetailsList(EmpId, FromDate, ToDate).ToList();

                    ViewBag.LeaveApplicationDetailsListVB = LeaveApplicationDetailsList;

                    LeaveApplicationDetailsList = WetosDB.SP_LeaveApplicationDetailsList(EmpId, FromDate, ToDate).Where(a => a.StatusId == 1).ToList();

                    ViewBag.LeaveBalanceDetailsVB = DashboardController.GetLeaveDetailsOnallControllers(WetosDB, EmpId, DateTime.Now); //FinalLeaveBalanceList;
                    string LeaveType = LeaveMasterList.Select(a => a.LeaveType).FirstOrDefault();
                    LeaveCredit LeaveCreditObj = WetosDB.LeaveCredits.Where(a => a.EmployeeId == EmpId).FirstOrDefault();
                    ViewBag.LeaveCreditObjVB = LeaveCreditObj;
                }
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Exception during validating leave " + ex.Message + ( ex.InnerException == null ? string.Empty : ex.InnerException.Message));
            }

            return PartialView();
        }

        #endregion

        public ActionResult GetAttendanceDetails(int EmployeeId, string FromDate, string ToDate)
        {
            DateTime FromDateDT = Convert.ToDateTime(FromDate);
            DateTime ToDateDT = Convert.ToDateTime(ToDate);
            List<SP_GetAttendanceDetails_Result> DailyTransactionList = WetosDB.SP_GetAttendanceDetails(EmployeeId, FromDateDT, ToDateDT).ToList();
            if (DailyTransactionList == null)
            {
                DailyTransactionList = new List<SP_GetAttendanceDetails_Result>();
            }
            return PartialView(DailyTransactionList);
        }

    }
}
