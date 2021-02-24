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
    public class WetosDeclaredHolidayController : BaseController
    {
        


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// Updated by Rajas on 25 SEP 2017
        public ActionResult Index()
        {
            List<SP_DeclaredHolidayListView_Result> HolidayList = new List<SP_DeclaredHolidayListView_Result>();
            try
            {
                HolidayList = WetosDB.SP_DeclaredHolidayListView().OrderByDescending(a => a.HolidayDate).ToList();

                return View(HolidayList);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in declared holiday list due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );
                return View(HolidayList);
            }
        }

        //
        // GET: /WetosHoliday/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }


        public ActionResult Create()
        {
            DeclaredHolidayInputModel DeclaredHolidayModelObj = new DeclaredHolidayInputModel();

            PopulateDropDown();

            return View(DeclaredHolidayModelObj);
        }

        //
        // POST: /WetosHoliday/Create

        [HttpPost]
        public ActionResult Create(DeclaredHolidayInputModel DeclaredHolidayModelObj, FormCollection collection)
        {
            try
            {
                string UpdateStatus = string.Empty;

                // Added by Rajas on 15 MAY 2017
                bool IsEdit = false;

                //Modified By shraddha on 17th OCT 2016 for model validation
                if (ModelState.IsValid)
                {
                    // Updated by Rajas on 15 MAY 2017
                    if (UpdateHolidayData(DeclaredHolidayModelObj, IsEdit, ref UpdateStatus) == true)
                    {
                        // ADDED BY PUSHKAR FOR AUDITLOG ON 21 DEC 2016
                        AddAuditTrail(UpdateStatus);

                        Success(UpdateStatus);
                    }
                    else
                    {
                        AddAuditTrail(UpdateStatus);

                        Error(UpdateStatus);

                        PopulateDropDownEdit();

                        return View(DeclaredHolidayModelObj);
                    }
                    // ModelState.AddModelError("Success", "Holiday has been created successfully:");
                    return RedirectToAction("Index", "WetosDeclaredHoliday");
                }
                else
                {
                    PopulateDropDownEdit();

                    // ADDED BY PUSHKAR FOR AUDITLOG ON 21 DEC 2016
                    //AddAuditTrail("Declared Holiday Entry failed");

                    //Error("Declared Holiday Entry failed");

                    return View(DeclaredHolidayModelObj);
                }
            }

            catch (System.Exception ex)
            {
                // ADDED BY PUSHKAR FOR AUDITLOG ON 21 DEC 2016
                AddAuditTrail("Declared Holiday Entry failed due to " + ex.Message);

                Error("Declared Holiday Entry failed");

                // ModelState.AddModelError("Success", "Holiday has been created successfully:");
                return RedirectToAction("Index", "WetosDeclaredHoliday");

            }
        }

        //
        // GET: /WetosHoliday/Edit/5

        public ActionResult Edit(int id)
        {
            DeclaredHoliday DeclaredHolidayObj = WetosDB.DeclaredHolidays.Where(a => a.DeclaredHolidayId == id).FirstOrDefault();

            DeclaredHolidayInputModel HolidayModelObj = new DeclaredHolidayInputModel();

            HolidayModelObj.Branchid = Convert.ToInt32(Session["BranchId"]);

            HolidayModelObj.HoliDayId = DeclaredHolidayObj.DeclaredHolidayId; // Added by Rajas on 14 MARCH 2017

            HolidayModelObj.Description = DeclaredHolidayObj.Description;

            HolidayModelObj.HolidayDate = DeclaredHolidayObj.HolidayDate;

            HolidayModelObj.CompWorkDay1 = DeclaredHolidayObj.CompWorkDay1;

            PopulateDropDownEdit();

            return View(HolidayModelObj);
        }

        //
        // POST: /WetosHoliday/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, DeclaredHolidayInputModel DeclaredHolidayModelObj)
        {
            string ReturnStatus = string.Empty;

            // Added by Rajas on 15 MAY 2017
            bool IsEdit = true;

            if (ModelState.IsValid)
            {
                // Updated by Rajas on 15 MAY 2017
                UpdateHolidayData(DeclaredHolidayModelObj, IsEdit, ref ReturnStatus);

                AddAuditTrail(ReturnStatus);

                Information(ReturnStatus);

                return RedirectToAction("Index", "WetosDeclaredHoliday");
            }
            else
            {
                PopulateDropDown();

                return View(DeclaredHolidayModelObj);
            }
        }

        //
        // GET: /WetosHoliday/Delete/5

        public ActionResult Delete(int id)
        {
            DeclaredHoliday HolidayObj = WetosDB.DeclaredHolidays.Where(a => a.DeclaredHolidayId == id).FirstOrDefault();

            return View(HolidayObj);
        }

        //
        // POST: /WetosHoliday/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                DeclaredHoliday HolidayObj = WetosDB.DeclaredHolidays.Where(a => a.DeclaredHolidayId == id).FirstOrDefault();

                if (HolidayObj != null)
                {
                    HolidayObj.MarkASDelete = 1;

                    WetosDB.SaveChanges();

                    Success("Holiday : " + HolidayObj.HolidayDate.ToString("dd-MMM-yyyy") + " - " + HolidayObj.Description + " deleted successfully");

                    AddAuditTrail("Holiday : " + HolidayObj.HolidayDate.ToString("dd-MMM-yyyy") + " - " + HolidayObj.Description + " deleted successfully");
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
            //var BranchObj = WetosDB.Branches.Where(a => a.MarkedAsDelete == null || a.MarkedAsDelete == 0)
                //.Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList();
            var BranchObj = WetosDB.SP_GetBranchList(EmployeeId).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList(); //.Select(m => new { BranchId = m.BranchId, BranchName = m.BranchName}).ToList();
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

            var ShiftObj = WetosDB.Shifts.Where(a => a.MarkedAsDelete == null || a.MarkedAsDelete == 0)
                .Select(a => new { ShiftId = a.ShiftId, ShiftName = a.ShiftName }).ToList();
            ViewBag.ShiftList = new SelectList(ShiftObj, "ShiftId", "ShiftName").ToList();
            // Updated by Rajas on 6 MAY 2017 to display data which is not MarkedAsDelete END


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

        /// <summary>
        /// Populate dropdown edit mode
        /// </summary>
        public void PopulateDropDownEdit()
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
            //var BranchObj = WetosDB.Branches.Where(a => a.MarkedAsDelete == null || a.MarkedAsDelete == 0)
                //.Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList();
            var BranchObj = WetosDB.SP_GetBranchList(EmployeeId).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList(); //.Select(m => new { BranchId = m.BranchId, BranchName = m.BranchName}).ToList();
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

        /// <summary>
        /// Common function to validate the data for edit or create
        /// Added by Rajas on 14 MARCH 2017
        /// </summary>
        private bool UpdateHolidayData(DeclaredHolidayInputModel DeclaredHolidayModelObj, bool IsEdit, ref string ReturnStatus)
        {

            try
            {
                WetosDB.DeclaredHoliday DeclaredHolidayTblObj = WetosDB.DeclaredHolidays.Where(a => a.DeclaredHolidayId == DeclaredHolidayModelObj.HoliDayId
                    && a.Branch.BranchId == DeclaredHolidayModelObj.Branchid).FirstOrDefault();

                // ADDED BY RAJAS ON 03 JUNE 2017 START
                bool IsNew = false;

                // IS OBJECT PRESET
                if (DeclaredHolidayTblObj != null)  // Added by Rajas on 2 JUNE 2017
                {
                    if (IsEdit == false)  // CREATE            
                    {
                        ReturnStatus = "DeclaredHoliday already available."; //WetosEmployeeController.GetErrorMessage(1);

                        //AddAuditTrail("Error in Department update : " + UpdateStatus);
                        return false;
                    }
                }
                else
                {
                    if (IsEdit == false) // CREATE    
                    {
                        DeclaredHolidayTblObj = new WetosDB.DeclaredHoliday();
                        IsNew = true;
                    }
                    else // EDIT    
                    {
                        ReturnStatus = "Error in updating DeclaredHoliday."; // WetosEmployeeController.GetErrorMessage(1); 
                        //AddAuditTrail("Error in Department update : " + UpdateStatus);
                        return false;
                    }
                }
                // ADDED BY RAJAS ON 03 JUNE 2017 END

                // New Leave table object

                // Added by Rajas on 25 SEP 2017 for Null object error in Branch constarint
                DeclaredHolidayTblObj.Branch = WetosDB.Branches.Where(a => a.BranchId == DeclaredHolidayModelObj.Branchid).FirstOrDefault();

                DeclaredHolidayTblObj.Branch.BranchId = DeclaredHolidayModelObj.Branchid;

                DeclaredHolidayTblObj.Description = DeclaredHolidayModelObj.Description;

                DeclaredHolidayTblObj.HolidayDate = DeclaredHolidayModelObj.HolidayDate;

                DeclaredHolidayTblObj.CompWorkDay1 = DeclaredHolidayModelObj.CompWorkDay1;

                DeclaredHolidayTblObj.Type = true;
                // Add new table object 
                if (IsNew)
                {
                    WetosDB.DeclaredHolidays.AddObject(DeclaredHolidayTblObj);
                }

                WetosDB.SaveChanges();
                if (IsNew)
                {
                    ReturnStatus = "Success: Declared Holiday Added Successfully";
                }
                else
                {
                    ReturnStatus = "Success: Declared Holiday Updated Successfully";

                }
                return true;
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in updating Declared holiday data due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );
                ReturnStatus = "Error in updating Declared holiday data due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message);
                return false;
            }
        }
    }
}
