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
    public class WetosHolidayController : BaseController
    {
        //
        // GET: /WetosHoliday/

        public ActionResult Index()
        {
            // ADDED BY MSJ ON 28 JAN 2020 START
            // GET CURRENT FY
            GlobalSetting GlobalSettingCFY = WetosDB.GlobalSettings.Where(a=> a.SettingId == 2).FirstOrDefault();
            int CurrentYear = GlobalSettingCFY == null ? DateTime.Now.Year : Convert.ToInt32(GlobalSettingCFY.SettingValue);
            // ADDED BY MSJ ON 28 JAN 2020 END

            // Updated by Rajas on 29 MARCH 2017
            List<SP_HolidayListView_Result> HolidayList = WetosDB.SP_HolidayListView()
                .Where(a => a.MarkedAsDelete == 0 && a.FromDate.Year == CurrentYear).OrderByDescending(a => a.FromDate).ToList();
            // || a.MarkedAsDelete == null // MODIFIED BY MSJ ON 23 JAN 2020

            return View(HolidayList);
        }


        /// <summary>
        /// Added by Rajas on 29 MARCH 2017
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int id)
        {
            try
            {
                // Updated by Rajas on 29 MARCH 2017
                SP_HolidayListView_Result HolidayView = WetosDB.SP_HolidayListView()
                    .Where(a => a.MarkedAsDelete == 0).OrderByDescending(a => a.FromDate).Single(a => a.HoliDayId == id); // || a.MarkedAsDelete == null// MODIFIED BY MSJ ON 23 JAN 2020

                AddAuditTrail("Details for Holiday checked");

                return View(HolidayView);

            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Please try again!");

                return RedirectToAction("Index");  // verify
            }

        }

        //
        // GET: /WetosHoliday/Create

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

            // Updated by Rajas on 27 FEB 2017
            // Updated by Rajas on 30 MAY 2017
            
            #region CODE FOR GET BRANCH LIST ADDED BY SHRADDHA ON 30 MAR 2018
            int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
            //var BranchList = WetosDB.Branches.Where(a => a.Company.CompanyId == SelCompanyId && (a.MarkedAsDelete == null || a.MarkedAsDelete == 0)).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList();
            var BranchList = WetosDB.SP_GetBranchList(EmployeeId).Where(a => a.CompanyId == SelCompanyId).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList(); //.Select(m => new { BranchId = m.BranchId, BranchName = m.BranchName}).ToList();
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
            #endregion

            return Json(BranchList);
        }

        /// <summary>
        /// Updated by Rajas on 27 MARCH 2017
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            try
            {
                HolidayInputModel HolidayModelObj = new HolidayInputModel();

                PopulateDropDown();

                return View(HolidayModelObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Incorrect data. please try again!");

                return RedirectToAction("Index");  // Verify
            }
        }


        /// <summary>
        /// UPDATED BY RAJAS ON 3 JUNE 2017
        /// </summary>
        /// <param name="HolidayModelObj"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(HolidayInputModel HolidayModelObj, FormCollection collection)
        {
            try
            {
                // Added by Rajas on 15 MAY 2017 START
                string UpdateStatus = string.Empty;

                bool IsEdit = false;
                // Added by Rajas on 15 MAY 2017 END

                //Modified By shraddha on 17th OCT 2016 for model validation
                if (ModelState.IsValid)
                {
                    // Updated by Rajas on 15 MAY 2017
                    if (UpdateHolidayData(HolidayModelObj, IsEdit, ref UpdateStatus) == true)
                    {
                        // ADDED BY PUSHKAR FOR AuditLog ON 21 DEC 2016
                        AddAuditTrail(UpdateStatus);

                        Success(UpdateStatus);
                    }
                    else
                    {
                        return ReportError(HolidayModelObj, UpdateStatus);
                    }
                    // ModelState.AddModelError("Success", "Holiday has been created successfully:");
                    return RedirectToAction("Index", "WetosHoliday");
                }
                else
                {
                    PopulateDropDownEdit(HolidayModelObj);

                    // ADDED BY PUSHKAR FOR AuditLog ON 21 DEC 2016
                    //AddAuditTrail("Holiday Entry failed");

                    //Error("Holiday Entry failed");

                    return View(HolidayModelObj);
                }
            }

            catch (System.Exception ex)
            {
                // ADDED BY PUSHKAR FOR AuditLog ON 21 DEC 2016
                AddAuditTrail("Holiday Entry failed due to " + ex.Message);

                Error("Holiday Entry failed");

                PopulateDropDownEdit(HolidayModelObj);

                return View(HolidayModelObj);

            }
        }


        /// <summary>
        /// Updated by Rajas on 27 MARCH 2017
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            try
            {
                HoliDay HolidayObj = WetosDB.HoliDays.Where(a => a.HoliDayId == id).FirstOrDefault();

                HolidayInputModel HolidayModelObj = new HolidayInputModel();

                HolidayModelObj.Branchid = HolidayObj.Branchid;

                HolidayModelObj.HoliDayId = HolidayObj.HoliDayId; // Added by Rajas on 14 MARCH 2017

                HolidayModelObj.CompanyId = HolidayObj.CompanyId;

                HolidayModelObj.Criteria = HolidayObj.Criteria;

                HolidayModelObj.DayStatus = HolidayObj.DayStatus;

                HolidayModelObj.Description = HolidayObj.Description;

                HolidayModelObj.EmployeeId = HolidayObj.EmployeeId;

                HolidayModelObj.FromDate = HolidayObj.FromDate;

                HolidayModelObj.HLType = HolidayObj.HLType;

                // Added by Rajas on 29 MARCH 2017
                HolidayModelObj.MarkedAsDelete = HolidayObj.MarkedAsDelete;

                HolidayModelObj.ReligionId = HolidayObj.ReligionId;

                HolidayModelObj.ToDate = HolidayObj.ToDate;

                int FinancialId = WetosDB.FinancialYears.Where(a => a.Company.CompanyId == HolidayModelObj.CompanyId && a.Branch.BranchId == HolidayModelObj.Branchid && (HolidayModelObj.FromDate >= a.StartDate && HolidayModelObj.FromDate <= a.EndDate)).Select(a => a.FinancialYearId).FirstOrDefault();

                if (FinancialId != null || FinancialId != 0)
                {
                    HolidayModelObj.FinancialYearId = FinancialId;
                }
                else
                {
                    FinancialId = WetosDB.FinancialYears.Where(a => (HolidayModelObj.FromDate >= a.StartDate && HolidayModelObj.FromDate <= a.EndDate)).Select(a => a.FinancialYearId).FirstOrDefault();

                    HolidayModelObj.FinancialYearId = FinancialId;
                }

                PopulateDropDownEdit(HolidayModelObj);

                return View(HolidayModelObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Please try again!");

                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// UPDATED BY RAJAS ON 3 JUNE 2017
        /// </summary>
        /// <param name="id"></param>
        /// <param name="HolidayModelObj"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(int id, HolidayInputModel HolidayModelObj)
        {
            try
            {
                // Updated by Rajas on 15 MAY 2017 START
                string UpdateStatus = string.Empty;

                bool IsEdit = true;
                // Updated by Rajas on 15 MAY 2017 END

                if (ModelState.IsValid)
                {
                    // Updated by Rajas on 15 MAY 2017
                    if (UpdateHolidayData(HolidayModelObj, IsEdit, ref UpdateStatus) == true)
                    {
                        AddAuditTrail(UpdateStatus);

                        Success(UpdateStatus);
                    }
                    else
                    {
                        return ReportError(HolidayModelObj, UpdateStatus);
                    }
                    return RedirectToAction("Index", "WetosHoliday");
                }
                else
                {
                    PopulateDropDownEdit(HolidayModelObj);

                    return View(HolidayModelObj);
                }
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error : " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                PopulateDropDownEdit(HolidayModelObj);

                return View(HolidayModelObj);
            }
        }


        /// <summary>
        /// Added by rajas on 29 MARCH 2017
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            try
            {
                // Updated by Rajas on 29 MARCH 2017
                SP_HolidayListView_Result HolidayView = WetosDB.SP_HolidayListView()
                    .Where(a => a.MarkedAsDelete == 0 || a.MarkedAsDelete == null).OrderByDescending(a => a.FromDate).Single(a => a.HoliDayId == id);

                AddAuditTrail("Details for Holiday checked");

                return View(HolidayView);

            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Please try again!");

                return RedirectToAction("Index");  // verify
            }
        }


        /// <summary>
        /// Added by Rajas on 29 MARCH 2017
        /// POST
        /// </summary>
        /// <param name="id"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                HoliDay HolidayObj = WetosDB.HoliDays.Where(a => a.HoliDayId == id).FirstOrDefault();

                if (HolidayObj != null)
                {
                    HolidayObj.MarkedAsDelete = 1;

                    WetosDB.SaveChanges();

                    Success("Holiday : " + HolidayObj.FromDate.ToString("dd-MMM-yyyy") + " - " + HolidayObj.Description + " deleted successfully");

                    AddAuditTrail("Holiday : " + HolidayObj.FromDate.ToString("dd-MMM-yyyy") + " - " + HolidayObj.Description + " deleted successfully");
                }

                return RedirectToAction("Index");
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Please try again!");

                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// To Populate Dropdown Added by Shraddha on 17th Oct 2016
        /// </summary>
        public void PopulateDropDown()
        {
            try
            {
                List<string> SelectCriteriaObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 5).Select(a => a.Text).ToList();
                ViewBag.SelectCriteriaList = SelectCriteriaObj;

                #region COMPANY LIST CODE ADDED BY SHRADDHA ON 30 MAR 2018
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //var CompanyObj = WetosDB.Companies.Select(a => new { CompanyId = a.CompanyId, CompanyName = a.CompanyName }).ToList();
                var CompanyObj = WetosDB.SP_GetCompanyList(EmployeeId).Select(m => new { Companyid = m.CompanyId, CompanyName = m.CompanyName }).ToList();
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
                #endregion

                ViewBag.CompanyList = new SelectList(CompanyObj, "CompanyId", "CompanyName").ToList();

                var BranchObj = new List<Branch>(); //WetosDB.Branches.Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList();

                ViewBag.BranchList = new SelectList(BranchObj, " BranchId", "BranchName").ToList();

                List<string> SearchByObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 4).Select(a => a.Text).ToList();
                ViewBag.SearchByList = SearchByObj;

                // Added by Rajas on 21 NOV 2016 for Dropdown list start
                var DayStatusObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 9).Select(a => new { LeaveStatusID = a.Value, LeaveStatus = a.Text }).ToList();
                ViewBag.DayStatusList = new SelectList(DayStatusObj, "LeaveStatusID", "LeaveStatus").ToList();

                
                #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //var EmployeeObj = WetosDB.Employees.Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList(); //ADDED EMPLOYEE CODE BY SHRADDHA ON 15 FEB 2018 
                var EmployeeObj = WetosDB.SP_VwActiveEmployee(EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
                #endregion
                
                ViewBag.EmployeeList = new SelectList(EmployeeObj, "EmployeeId", "EmployeeName").ToList();


                var ShiftObj = WetosDB.Shifts.Where(a => a.MarkedAsDelete == null || a.MarkedAsDelete == 0).Select(a => new { ShiftId = a.ShiftId, ShiftName = a.ShiftName }).ToList();
                ViewBag.ShiftList = new SelectList(ShiftObj, "ShiftId", "ShiftName").ToList();


                //Modified By Shraddha on 15Th OCT 2016 Start
                var HolidayTypeObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 6).Select(a => new { HolidayName = a.Text, HolidayValue = a.Text }).ToList();
                ViewBag.HolidayTypeList = new SelectList(HolidayTypeObj, "HolidayName", "HolidayValue").ToList();
                //Modified By Shraddha on 15Th OCT 2016 Start

                var LeaveSanctionObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 3).Select(a => new { LeaveSanctionId = a.Value, Status = a.Text }).ToList();

                ViewBag.LeaveSanctionList = new SelectList(LeaveSanctionObj, "LeaveSanctionId", "Status").ToList();

                var CondoneEntry1Obj = WetosDB.CondoneTrns.Select(a => new { ShiftId = a.ShiftId, CondoneShift = a.ShiftId }).Distinct().ToList();

                ViewBag.CondoneShiftList = new SelectList(CondoneEntry1Obj, "ShiftId", "CondoneShift").ToList();

                //Added By Shraddha on 28 DEC 2016 For condone status add START
                var CondoneStatusObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 11).Select(a => new { CondoneStatusID = a.Text, CondoneStatus = a.Text }).ToList();
                ViewBag.CondoneStatusObj = new SelectList(CondoneStatusObj, "CondoneStatusID", "CondoneStatus").ToList();
                //Added By Shraddha on 28 DEC 2016 For condone status add END
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );
            }
        }

        /// <summary>
        /// Populate dropdown edit mode
        /// </summary>
        public void PopulateDropDownEdit(HolidayInputModel HolidayModelObj)
        {
            try
            {
                List<string> SelectCriteriaObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 5).Select(a => a.Text).ToList();
                ViewBag.SelectCriteriaList = SelectCriteriaObj;

                #region COMPANY LIST CODE ADDED BY SHRADDHA ON 30 MAR 2018
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //var CompanyObj = WetosDB.Companies.Select(a => new { CompanyId = a.CompanyId, CompanyName = a.CompanyName }).ToList();
                var CompanyObj = WetosDB.SP_GetCompanyList(EmployeeId).Select(m => new { Companyid = m.CompanyId, CompanyName = m.CompanyName }).ToList();
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
                #endregion

                ViewBag.CompanyList = new SelectList(CompanyObj, "CompanyId", "CompanyName").ToList();

                

                #region CODE FOR GET BRANCH LIST ADDED BY SHRADDHA ON 30 MAR 2018
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
                //var BranchObj = WetosDB.Branches.Where(a => (a.MarkedAsDelete == null || a.MarkedAsDelete == 0) && a.Company.CompanyId == HolidayModelObj.CompanyId)
                   // .Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList();
                var BranchObj = WetosDB.SP_GetBranchList(EmployeeId).Where(a => a.CompanyId == HolidayModelObj.CompanyId).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList(); //.Select(m => new { BranchId = m.BranchId, BranchName = m.BranchName}).ToList();
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
                #endregion

                ViewBag.BranchList = new SelectList(BranchObj, " BranchId", "BranchName").ToList();

                List<string> SearchByObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 4).Select(a => a.Text).ToList();
                ViewBag.SearchByList = SearchByObj;

                // Added by Rajas on 21 NOV 2016 for Dropdown list start
                var DayStatusObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 9).Select(a => new { LeaveStatusID = a.Value, LeaveStatus = a.Text }).ToList();
                ViewBag.DayStatusList = new SelectList(DayStatusObj, "LeaveStatusID", "LeaveStatus").ToList();

                
                #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //var EmployeeObj = WetosDB.Employees.Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList(); //ADDED EMPLOYEE CODE BY SHRADDHA ON 15 FEB 2018 
                var EmployeeObj = WetosDB.SP_VwActiveEmployee(EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
                #endregion
                
                ViewBag.EmployeeList = new SelectList(EmployeeObj, "EmployeeId", "EmployeeName").ToList();

                //Modified By Shraddha on 15Th OCT 2016 Start
                var HolidayTypeObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 6).Select(a => new { HolidayName = a.Text, HolidayValue = a.Text }).ToList();
                ViewBag.HolidayTypeList = new SelectList(HolidayTypeObj, "HolidayName", "HolidayValue").ToList();
                //Modified By Shraddha on 15Th OCT 2016 Start
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );
            }
        }

        /// <summary>
        /// Common Error reporting function
        /// Added by Rajas on 3 JUNE 2017
        /// </summary>
        /// <param name="DepartmentObj"></param>
        /// <param name="ErrorMessage"></param>
        /// <returns></returns>
        public ActionResult ReportError(HolidayInputModel HolidayModelObj, string ErrorMessage)
        {
            PopulateDropDownEdit(HolidayModelObj);//

            AddAuditTrail(ErrorMessage);
            Error(ErrorMessage);

            return View(HolidayModelObj);
        }

        /// <summary>
        /// Common function to validate the data for edit or create
        /// Added by Rajas on 14 MARCH 2017
        /// </summary>
        private bool UpdateHolidayData(HolidayInputModel HolidayModelObj, bool IsEdit, ref string UpdateStatus)
        {
            bool ReturnStatus = false;

            try
            {
                // Updated by Rajas on 20 JULY 2017 to resolve Defect Id 11 from Defect Sheet by Ulka(Eviska)
                //WetosDB.HoliDay HolidayTblObj = WetosDB.HoliDays.Where(a => a.HoliDayId == HolidayModelObj.HoliDayId && a.CompanyId == HolidayModelObj.CompanyId
                //    && a.Branchid == HolidayModelObj.Branchid && a.MarkedAsDelete == 0).FirstOrDefault();

                //Added by Shalaka on 16th DEC 2017 --- Start
                WetosDB.HoliDay HolidayTblObj = WetosDB.HoliDays.Where(a => a.HoliDayId == HolidayModelObj.HoliDayId && a.CompanyId == HolidayModelObj.CompanyId
                    && a.Branchid == HolidayModelObj.Branchid && a.MarkedAsDelete == 0 && a.FromDate == HolidayModelObj.FromDate).FirstOrDefault();
                //Added by Shalaka on 16th DEC 2017 --- End
                
                //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 9 SEPTEMBER 2017 FOR EDIT
                WetosDB.HoliDay HolidayTblObjEDIT = WetosDB.HoliDays.Where(a => a.HoliDayId == HolidayModelObj.HoliDayId && a.CompanyId == HolidayModelObj.CompanyId
                    && a.Branchid == HolidayModelObj.Branchid && a.MarkedAsDelete == 0).FirstOrDefault();

                // Added by Rajas on 29 JULY 2017
                //MODIFIED BY PUSHKAR ON 29 MAY 2018 FOR ISSUE REPORTED BY ANAGHA 
                WetosDB.HoliDay IsHolidayDateAvailable = WetosDB.HoliDays.Where(a => a.FromDate == HolidayModelObj.FromDate && a.MarkedAsDelete == 0
                    && a.CompanyId == HolidayModelObj.CompanyId && a.Branchid == HolidayModelObj.Branchid).FirstOrDefault();

                // ADDED BY RAJAS ON 03 JUNE 2017 START
                bool IsNew = false;

                // IS OBJECT PRESET
                if (HolidayTblObj != null)  // Added by Rajas on 2 JUNE 2017
                {
                    if (IsEdit == false)  // CREATE            
                    {
                        UpdateStatus = "Holiday already available."; //WetosEmployeeController.GetErrorMessage(1);

                        //AddAuditTrail("Error in Department update : " + UpdateStatus);
                        return ReturnStatus;
                    }
                }
                else
                {
                    // Added by Rajas on 29 JULY 2017 START
                    if (IsHolidayDateAvailable != null)
                    {
                        UpdateStatus = "Holiday for selected date is available, please verify.";
                        
                        return ReturnStatus;
                    }
                    else
                    {
                        if (IsEdit == false) // CREATE    
                        {
                            HolidayTblObj = new WetosDB.HoliDay();
                            IsNew = true;
                        }
                        else // EDIT    
                        {
                            UpdateStatus = "Error in updating Holiday."; // WetosEmployeeController.GetErrorMessage(1); 
                            //AddAuditTrail("Error in Department update : " + UpdateStatus);
                            return ReturnStatus;
                        }
                    }
                    // Added by Rajas on 29 JULY 2017 END
                }
                // ADDED BY RAJAS ON 03 JUNE 2017 END

                // New Leave table object
                HolidayTblObj.Branchid = HolidayModelObj.Branchid;

                HolidayTblObj.CompanyId = HolidayModelObj.CompanyId;

                HolidayTblObj.Criteria = HolidayModelObj.Criteria;

                HolidayTblObj.DayStatus = HolidayModelObj.DayStatus;

                HolidayTblObj.Description = HolidayModelObj.Description;

                HolidayTblObj.EmployeeId = HolidayModelObj.EmployeeId;

                HolidayTblObj.FromDate = HolidayModelObj.FromDate;

                HolidayTblObj.HLType = HolidayModelObj.HLType;

                HolidayTblObj.ReligionId = HolidayModelObj.ReligionId;

                HolidayTblObj.ToDate = HolidayModelObj.FromDate;

                // Added by Rajas on 29 MARCH 2017
                HolidayTblObj.MarkedAsDelete = 0;  // Default value

                int FinancialId = WetosDB.FinancialYears.Where(a => a.Company.CompanyId == HolidayModelObj.CompanyId && a.Branch.BranchId == HolidayModelObj.Branchid && (HolidayModelObj.FromDate >= a.StartDate && HolidayModelObj.FromDate <= a.EndDate)).Select(a => a.FinancialYearId).FirstOrDefault();

                // Added by Rajas on 12 MAY 2017, to handle Inconsistent FY data for selected company and branch
                if (FinancialId == 0)
                {
                    // Updated by Rajas on 20 JULY 2017 to resolve Defect Id 13 from Defect Sheet by Ulka(Eviska)
                    UpdateStatus = "Financial year not available for selected Company and branch, please verify and try agian!";

                    return ReturnStatus = false;
                }
                else if (FinancialId != 0)
                {
                    HolidayTblObj.FinancialYearId = FinancialId;
                }
                else
                {
                    FinancialId = WetosDB.FinancialYears.Where(a => (HolidayModelObj.FromDate >= a.StartDate && HolidayModelObj.FromDate <= a.EndDate)).Select(a => a.FinancialYearId).FirstOrDefault();

                    HolidayTblObj.FinancialYearId = FinancialId;
                }


                // Add new table object 
                if (IsNew)
                {

                    WetosDB.HoliDays.AddObject(HolidayTblObj);
                }

                WetosDB.SaveChanges();


                //-------------------------------------------AuditLog---------------------------------------------------------------------------

                if (IsNew)
                {
                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 11 SEPTEMBER 2017
                    #region ADD AUDIT LOG
                    //OLD RECORD IS BLANK
                    string Newrecord = "EmployeeId : "+HolidayTblObj.EmployeeId+", HolidayType :"+HolidayTblObj.DayStatus
                        +", DayStatus : "+HolidayTblObj.DayStatus+", Criteria : "+HolidayTblObj.Criteria
                        +", FromDate : "+HolidayTblObj.FromDate+", ToDate :"+HolidayTblObj.ToDate
                        +", Description : "+HolidayTblObj.Description+", Religion ID : "+HolidayTblObj.ReligionId
                        +", Division ID : "+HolidayTblObj.DivisionId+", Branch ID : "+HolidayTblObj.Branchid
                        +", Company ID : "+HolidayTblObj.CompanyId;


                    //LOGIN ID IS TAKEN FROM SESSION PERSISTER
                    string Formname = "HOLIDAY MASTER";
                    //ACTION IS UPDATE
                    string Message = " ";

                    WetosAdministrationController.GenerateAuditLogsInsert(WetosDB, 0, Formname, Newrecord, ref Message);
                    #endregion
                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 11 SEPTEMBER 2017
                }
                else
                {
                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 11 SEPTEMBER 2017
                    #region ADD AUDIT LOG
                    string Oldrecord = "EmployeeId : " + HolidayTblObjEDIT.EmployeeId + ", HolidayType :" + HolidayTblObjEDIT.DayStatus
                        + ", DayStatus : " + HolidayTblObjEDIT.DayStatus + ", Criteria : " + HolidayTblObjEDIT.Criteria
                        + ", FromDate : " + HolidayTblObjEDIT.FromDate + ", ToDate :" + HolidayTblObjEDIT.ToDate
                        + ", Description : " + HolidayTblObjEDIT.Description + ", Religion ID : " + HolidayTblObjEDIT.ReligionId
                        + ", Division ID : " + HolidayTblObjEDIT.DivisionId + ", Branch ID : " + HolidayTblObjEDIT.Branchid
                        + ", Company ID : " + HolidayTblObjEDIT.CompanyId;
                    //---x---------x-----------------x------------x----------------x-----------x-------------x------------x--------------x----------x---------x-----
                    string Newrecord = "EmployeeId : " + HolidayTblObj.EmployeeId + ", HolidayType :" + HolidayTblObj.DayStatus
                        + ", DayStatus : " + HolidayTblObj.DayStatus + ", Criteria : " + HolidayTblObj.Criteria
                        + ", FromDate : " + HolidayTblObj.FromDate + ", ToDate :" + HolidayTblObj.ToDate
                        + ", Description : " + HolidayTblObj.Description + ", Religion ID : " + HolidayTblObj.ReligionId
                        + ", Division ID : " + HolidayTblObj.DivisionId + ", Branch ID : " + HolidayTblObj.Branchid
                        + ", Company ID : " + HolidayTblObj.CompanyId;


                    //LOGIN ID IS TAKEN FROM SESSION PERSISTER
                    string Formname = "HOLIDAY MASTER";
                    //ACTION IS UPDATE
                    string Message = " ";

                    WetosAdministrationController.GenerateAuditLogsUpdate(WetosDB, 0, Formname, Oldrecord,
                        Newrecord, ref Message);
                    #endregion
                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 11 SEPTEMBER 2017
                }
                    
                //-------------------------------------------AuditLog---------------------------------------------------------------------------



                ReturnStatus = true;
                if (IsNew)
                {
                    UpdateStatus = "Success: Holiday Added Successfully";
                }
                else
                {
                    UpdateStatus = "Success: Holiday Updated Successfully";

                }
                return ReturnStatus;
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in updating holiday data due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );
                UpdateStatus = "Error in updating holiday data due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message);
                ReturnStatus = false;
                return ReturnStatus;
            }
        }
    }
}
