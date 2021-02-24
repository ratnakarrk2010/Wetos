using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WetosDB;
using WetosMVCMainApp.Models;

namespace WetosMVC.Controllers
{
    [SessionExpire]  // Added by Rajas on 4 MAY 2017
    [Authorize] // Added by MSJ on 18 JULY 2017
    public class WetosShiftScheduleController : BaseController
    {
        /// <summary>
        /// Added by Rajas on 11 APRIL 2017
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            try
            {
                //GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == "Current Financial Year").FirstOrDefault();

                //ABOVE LINE COMMENTED AND BELOW LINE IS ADDED BY SHALAKA ON 13TH DEC 2017
                GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.CurrentFinancialYear).FirstOrDefault();

                List<SP_ShiftScheduleList_Result> ShiftScheduleList = new List<SP_ShiftScheduleList_Result>();

                if (GlobalSettingObj != null)
                {
                    int CurrentFY = Convert.ToInt32(GlobalSettingObj.SettingValue);

                    ShiftScheduleList = WetosDB.SP_ShiftScheduleList().Where(a => a.ShiftYear == CurrentFY).ToList();

                    //ADDED BY RAJAS ON 27 DEC 2016
                    AddAuditTrail("Success - Visited news master"); // Updated by Rajas on 16 JAN 2017
                }
                else
                {
                    AddAuditTrail("Inconsistent FY data in shift schedule");

                    Information("Inconsistent FY data in shift schedule, Please verify");

                    return View(ShiftScheduleList);
                }

                return View(ShiftScheduleList);

            }

            catch (System.Exception ex)
            {
                // Added by Rajas on 16 JAN 2017
                AddAuditTrail("Exception - " + ex.Message);

                return View("Error");
            }
        }

        /// <summary>
        /// Details
        /// Added by Rajas on 11 APRIL 2017
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int id)
        {
            try
            {
                SP_ShiftScheduleList_Result ShiftScheduleListDetails = WetosDB.SP_ShiftScheduleList().Single(a => a.ShiftSchedulePatternId == id);

                //ADDED BY RAJAS ON 27 DEC 2016
                AddAuditTrail("Checked details for Shift Schedule");

                return View(ShiftScheduleListDetails);
            }

            catch (System.Exception ex)
            {
                // Added by Rajas on 16 JAN 2017
                AddAuditTrail("Exception in Shift Schedule Details - " + ex.Message);

                return View("Error");
            }
        }

        /// <summary>
        /// CREATE GET
        /// Added by Rajas on 11 APRIL 2017
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            try
            {
                PopulateDropDown();

                return View();
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error on Shift schedule due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                Error("Inconsistent data. Please try again!");

                return RedirectToAction("Index");
            }
        }

        public ActionResult AssignPattern()
        {
            return View();
        }


        /// <summary>
        /// POST
        /// Create 
        /// Added by Rajas on 10 APRIL 2017
        /// </summary>
        /// <param name="id"></param>
        /// <param name="NewsModelObj"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(ShiftScheduleModel ShiftScheduleModelObj)
        {
            try
            {
                // Added by Rajas on 27 MARCH 2017
                string UpdateStatus = string.Empty;

                // Added by Rajas on 15 MAY 2017
                bool IsEdit = false;

                if (ModelState.IsValid)
                {
                    // Updated by Rajas on 15 MAY 2017
                    if (UpdateShiftScheduleData(ShiftScheduleModelObj, IsEdit, ref UpdateStatus) == true)
                    {
                        AddAuditTrail("Shift Schedule is added successfully for " + ShiftScheduleModelObj.EmployeeId);

                        Success("Shift Schedule is added successfully");
                    }
                    else
                    {
                        AddAuditTrail(UpdateStatus);

                        Error(UpdateStatus);
                    }

                    return RedirectToAction("Index");
                }

                else
                {
                    // POPULATE VIEWBAG FRO DROPDOWN
                    PopulateDropDownEdit();

                    AddAuditTrail("Error - Add new Shift Schedule failed");

                    Error("Error - Add new Shift Schedule update failed");

                    return View();

                }

            }
            catch (System.Exception ex)
            {
                // POPULATE VIEWBAG FRO DROPDOWN
                PopulateDropDownEdit();

                AddAuditTrail("Error - Add new Shift Schedule failed due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                Error("Error - Add new Shift Schedule failed");

                return View();
            }
        }


        /// <summary>
        /// Edit
        /// GET
        /// Added by Rajas on 11 APRIL 2017
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            try
            {
                ShiftScheduleModel ShiftScheduleModelObj = new ShiftScheduleModel();

                ShiftSchedule ShiftScheduleEdit = WetosDB.ShiftSchedules.Single(b => b.ShiftScheduleId == id);

                ShiftScheduleModelObj.ShiftScheduleId = ShiftScheduleEdit.ShiftScheduleId;

                ShiftScheduleModelObj.RotationId = ShiftScheduleEdit.RotationId;

                ShiftScheduleModelObj.EmployeeId = ShiftScheduleEdit.EmployeeId;

                ShiftScheduleModelObj.EmployeeGroupId = ShiftScheduleEdit.EmployeeGroupId;

                ShiftScheduleModelObj.ShiftMonth = ShiftScheduleEdit.ShiftMonth;

                ShiftScheduleModelObj.ShiftYear = ShiftScheduleEdit.ShiftYear;

                // Need to verify (== null ? "0" : ShiftScheduleEdit.Day) START

                ShiftScheduleModelObj.Day1 = ShiftScheduleEdit.Day1 == null ? "0" : ShiftScheduleEdit.Day1;

                ShiftScheduleModelObj.Day2 = ShiftScheduleEdit.Day2 == null ? "0" : ShiftScheduleEdit.Day2;

                ShiftScheduleModelObj.Day3 = ShiftScheduleEdit.Day3 == null ? "0" : ShiftScheduleEdit.Day3;

                ShiftScheduleModelObj.Day4 = ShiftScheduleEdit.Day4 == null ? "0" : ShiftScheduleEdit.Day4;

                ShiftScheduleModelObj.Day5 = ShiftScheduleEdit.Day5 == null ? "0" : ShiftScheduleEdit.Day5;

                ShiftScheduleModelObj.Day6 = ShiftScheduleEdit.Day6 == null ? "0" : ShiftScheduleEdit.Day6;

                ShiftScheduleModelObj.Day7 = ShiftScheduleEdit.Day7 == null ? "0" : ShiftScheduleEdit.Day7;

                ShiftScheduleModelObj.Day8 = ShiftScheduleEdit.Day8 == null ? "0" : ShiftScheduleEdit.Day8;

                ShiftScheduleModelObj.Day9 = ShiftScheduleEdit.Day9 == null ? "0" : ShiftScheduleEdit.Day9;

                ShiftScheduleModelObj.Day10 = ShiftScheduleEdit.Day10 == null ? "0" : ShiftScheduleEdit.Day10;

                ShiftScheduleModelObj.Day11 = ShiftScheduleEdit.Day11 == null ? "0" : ShiftScheduleEdit.Day11;

                ShiftScheduleModelObj.Day12 = ShiftScheduleEdit.Day12 == null ? "0" : ShiftScheduleEdit.Day12;

                ShiftScheduleModelObj.Day13 = ShiftScheduleEdit.Day13 == null ? "0" : ShiftScheduleEdit.Day13;

                ShiftScheduleModelObj.Day14 = ShiftScheduleEdit.Day14 == null ? "0" : ShiftScheduleEdit.Day14;

                ShiftScheduleModelObj.Day15 = ShiftScheduleEdit.Day15 == null ? "0" : ShiftScheduleEdit.Day15;

                ShiftScheduleModelObj.Day16 = ShiftScheduleEdit.Day16 == null ? "0" : ShiftScheduleEdit.Day16;

                ShiftScheduleModelObj.Day17 = ShiftScheduleEdit.Day17 == null ? "0" : ShiftScheduleEdit.Day17;

                ShiftScheduleModelObj.Day18 = ShiftScheduleEdit.Day18 == null ? "0" : ShiftScheduleEdit.Day18;

                ShiftScheduleModelObj.Day19 = ShiftScheduleEdit.Day19 == null ? "0" : ShiftScheduleEdit.Day19;

                ShiftScheduleModelObj.Day20 = ShiftScheduleEdit.Day20 == null ? "0" : ShiftScheduleEdit.Day20;

                ShiftScheduleModelObj.Day21 = ShiftScheduleEdit.Day21 == null ? "0" : ShiftScheduleEdit.Day21;

                ShiftScheduleModelObj.Day22 = ShiftScheduleEdit.Day22 == null ? "0" : ShiftScheduleEdit.Day22;

                ShiftScheduleModelObj.Day23 = ShiftScheduleEdit.Day23 == null ? "0" : ShiftScheduleEdit.Day23;

                ShiftScheduleModelObj.Day24 = ShiftScheduleEdit.Day24 == null ? "0" : ShiftScheduleEdit.Day24;

                ShiftScheduleModelObj.Day25 = ShiftScheduleEdit.Day25 == null ? "0" : ShiftScheduleEdit.Day25;

                ShiftScheduleModelObj.Day26 = ShiftScheduleEdit.Day26 == null ? "0" : ShiftScheduleEdit.Day26;

                ShiftScheduleModelObj.Day27 = ShiftScheduleEdit.Day27 == null ? "0" : ShiftScheduleEdit.Day27;

                ShiftScheduleModelObj.Day28 = ShiftScheduleEdit.Day28 == null ? "0" : ShiftScheduleEdit.Day28;

                ShiftScheduleModelObj.Day29 = ShiftScheduleEdit.Day29 == null ? "0" : ShiftScheduleEdit.Day29;

                ShiftScheduleModelObj.Day30 = ShiftScheduleEdit.Day30 == null ? "0" : ShiftScheduleEdit.Day30;

                ShiftScheduleModelObj.Day31 = ShiftScheduleEdit.Day31 == null ? "0" : ShiftScheduleEdit.Day31;

                // END

                ShiftScheduleModelObj.CompanyId = ShiftScheduleEdit.CompanyId;

                ShiftScheduleModelObj.BranchId = ShiftScheduleEdit.BranchId;

                PopulateDropDownEdit();

                return View(ShiftScheduleModelObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message);

                Error("Inconsistent data. Please try again!");

                return View("Error");
            }
        }

        /// <summary>
        /// POST
        /// Edit News
        /// Added by Rajas on 10 APRIL 2017
        /// </summary>
        /// <param name="id"></param>
        /// <param name="NewsModelObj"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(int id, ShiftScheduleModel ShiftScheduleModelObj)
        {
            try
            {
                // Added by Rajas on 27 MARCH 2017
                string UpdateStatus = string.Empty;

                // Added by Rajas on 15 MAY 2017
                bool IsEdit = true;

                if (ModelState.IsValid)
                {
                    if (UpdateShiftScheduleData(ShiftScheduleModelObj, IsEdit, ref UpdateStatus) == true)
                    {
                        AddAuditTrail("Shift Schedule is updated successfully for " + ShiftScheduleModelObj.EmployeeId);

                        Success("Shift Schedule is updated successfully");
                    }
                    else
                    {
                        AddAuditTrail(UpdateStatus);

                        Error(UpdateStatus);
                    }

                    return RedirectToAction("Index");
                }

                else
                {
                    // POPULATE VIEWBAG FRO DROPDOWN
                    PopulateDropDownEdit();

                    AddAuditTrail("Error - Shift Schedule update failed");

                    Error("Error - Shift Schedule update failed");

                    return View();

                }

            }
            catch (System.Exception ex)
            {
                // POPULATE VIEWBAG FRO DROPDOWN
                PopulateDropDownEdit();

                AddAuditTrail("Error - Shift Schedule update failed due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                Error("Error - Shift Schedule update failed");

                return View();
            }
        }

        //
        // GET: /WetosShiftSchedule/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /WetosShiftSchedule/Delete/5

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

        /// <summary>
        ///  Populate All Dropdown ViewBag
        ///  Added by Rajas on 10 APRIL 2017
        /// </summary>
        public bool PopulateDropDown()
        {
            bool ReturnStatus = false;

            try
            {
                //CODE FOR DROPDOWN for COMPANY

                #region COMPANY LIST CODE ADDED BY SHRADDHA ON 30 MAR 2018
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //var CompanyList = WetosDB.Companies.Select(a => new { CompanyId = a.CompanyId, CompanyName = a.CompanyName }).ToList();
                var CompanyName = WetosDB.SP_GetCompanyList(EmployeeId).Select(m => new { Companyid = m.CompanyId, CompanyName = m.CompanyName }).ToList();
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
                #endregion

                ViewBag.CompanyList = new SelectList(CompanyName, "CompanyId", "CompanyName").ToList();

                //CODE FOR DROPDOWN for BRANCH
                var Branch = new List<Branch>();
                ViewBag.BranchList = new SelectList(Branch, "BranchId", "BranchName").ToList();

                //CODE FOR DROPDOWN for Employee group
                var EmployeeGroup = new List<EmployeeGroup>();
                ViewBag.EmployeeGroupList = new SelectList(EmployeeGroup, "EmployeeGroupId", "EmployeeGroupName").ToList();

                //CODE FOR DROPDOWN for Employee
                

                #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                //var Employee = WetosDB.VwActiveEmployees.Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
                var Employee = WetosDB.SP_VwActiveEmployee(EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
                #endregion
                
                ViewBag.EmployeeList = new SelectList(Employee, "EmployeeId", "EmployeeName").ToList();

                // Dropdown for month details
                var Month = WetosDB.MonthDetails.Select(a => new { Value = a.Id, Text = a.MonthName }).ToList();
                ViewBag.MonthDetailsList = new SelectList(Month, "Value", "Text").ToList();

                // Drop down for Shift
                var Shift = WetosDB.Shifts.Where(a => a.MarkedAsDelete == null || a.MarkedAsDelete == 0).Select(a => new { value = a.ShiftCode, Text = a.ShiftCode }).ToList();
                ViewBag.ShiftList = new SelectList(Shift, "Value", "Text").ToList();

                return ReturnStatus = true;

            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message);

                Error("Inconsistent data. Please try again!");

                return ReturnStatus;
            }
        }

        /// <summary>
        ///  Populate All Dropdown ViewBag
        ///  Added by Rajas on 11 APRIL 2017
        /// </summary>
        public bool PopulateDropDownEdit()
        {
            bool ReturnStatus = false;

            try
            {
                // Updated by Rajas on 6 MAY 2017 to get data which is not MarkedAsDelete START

                //CODE FOR DROPDOWN for COMPANY

                #region COMPANY LIST CODE ADDED BY SHRADDHA ON 30 MAR 2018
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //var CompanyList = WetosDB.Companies.Select(a => new { CompanyId = a.CompanyId, CompanyName = a.CompanyName }).ToList();
                var CompanyName = WetosDB.SP_GetCompanyList(EmployeeId).Select(m => new { Companyid = m.CompanyId, CompanyName = m.CompanyName }).ToList();
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
                #endregion

                ViewBag.CompanyList = new SelectList(CompanyName, "CompanyId", "CompanyName").ToList();

                //CODE FOR DROPDOWN for BRANCH
                #region CODE FOR GET BRANCH LIST ADDED BY SHRADDHA ON 30 MAR 2018
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
                //var Branch = WetosDB.Branches.Where(a => a.MarkedAsDelete == null || a.MarkedAsDelete == 0).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).OrderByDescending(a => a.BranchId).ToList();
                var BranchList = WetosDB.SP_GetBranchList(EmployeeId).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList(); //.Select(m => new { BranchId = m.BranchId, BranchName = m.BranchName}).ToList();
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
                #endregion

                ViewBag.BranchList = new SelectList(BranchList, "BranchId", "BranchName").ToList();

                //CODE FOR DROPDOWN for EmployeeGroup
                var EmployeeGroup = WetosDB.EmployeeGroups.Where(a => a.MarkedAsDelete == null || a.MarkedAsDelete == 0).Select(a => new { EmployeeGroupId = a.EmployeeGroupId, EmployeeGroupName = a.EmployeeGroupName }).ToList();
                ViewBag.EmployeeGroupList = new SelectList(EmployeeGroup, "EmployeeGroupId", "EmployeeGroupName").ToList();

                //CODE FOR DROPDOWN for Employee
                

                #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                //var Employee = WetosDB.VwActiveEmployees.Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
                var Employee = WetosDB.SP_VwActiveEmployee(EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
                #endregion

                ViewBag.EmployeeList = new SelectList(Employee, "EmployeeId", "EmployeeName").ToList();

                // Dropdown for month details
                var Month = WetosDB.MonthDetails.Select(a => new { Value = a.Id, Text = a.MonthName }).ToList();
                ViewBag.MonthDetailsList = new SelectList(Month, "Value", "Text").ToList();

                // Drop down for Shift
                var Shift = WetosDB.Shifts.Where(a => a.MarkedAsDelete == null || a.MarkedAsDelete == 0).Select(a => new { value = a.ShiftCode, Text = a.ShiftCode }).ToList();
                ViewBag.ShiftList = new SelectList(Shift, "Value", "Text").ToList();
                // Updated by Rajas on 6 MAY 2017 to get data which is not MarkedAsDelete END

                return ReturnStatus = true;
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message);

                Error("Inconsistent data. Please try again!");

                return ReturnStatus;
            }
        }

        /// <summary>
        /// Json return for to get Branch dropdown list on basis of company selection
        /// Added by Rajas on 11 APRIL 2017
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpPost]
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
            //var BranchList = WetosDB.Branches.Where(a => a.Company.CompanyId == SelCompanyId).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList();
            var BranchList = WetosDB.SP_GetBranchList(EmployeeId).Where(a => a.CompanyId == SelCompanyId).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList(); //.Select(m => new { BranchId = m.BranchId, BranchName = m.BranchName}).ToList();
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
            #endregion

            return Json(BranchList);
        }

        /// <summary>
        /// Json return for to get Employee dropdown list on basis of department selection
        /// Added by Rajas 11 APRIL 2017
        /// </summary>
        /// <returns></returns>
        public JsonResult GetEmployee(int Companyid, int Branchid)
        {
            // Updated on Rajas on 8 FEB 2017 for reducing ajax return list and json return as per required parameter START

            
            #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
            int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            //var EmployeeObj = WetosDB.Employees.Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " " + a.FirstName + " " + a.LastName }).ToList();
            var EmployeeObj = WetosDB.SP_VwActiveEmployee(EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " " + a.FirstName + " " + a.LastName }).ToList();
            #endregion

            //var EmployeeObj = WetosDB.Employees.Where(a => a.CompanyId == Companyid || a.BranchId == Branchid || a.DepartmentId == Departmentid).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.FirstName + " " + a.LastName }).ToList();

            if (Branchid == 0) // Modified by Rajas on 9 FEB 2017
            {
                

                #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                
                //EmployeeObj = WetosDB.Employees.Where(a => a.CompanyId == Companyid)
                   //.Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " " + a.FirstName + " " + a.LastName }).ToList();
                 EmployeeObj = WetosDB.SP_VwActiveEmployee(EmployeeId).Where(a => a.CompanyId == Companyid)
                   .Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " " + a.FirstName + " " + a.LastName }).ToList();
                #endregion
            }

            // COMMENTED BY RAJAS ON 9 FEB 2017
            //else if (Companyid != 0 && Branchid == 0 && Departmentid != 0)
            //{
            //    EmployeeObj = WetosDB.Employees.Where(a => a.BranchId == Companyid && a.DepartmentId == Departmentid)
            //          .Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.FirstName + " " + a.LastName }).ToList();
            //}
            //else if (Companyid == 0 && Branchid != 0 && Departmentid != 0)
            //{
            //    EmployeeObj = WetosDB.Employees.Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.FirstName + " " + a.LastName }).ToList();
            //}

            else
            {
                
                #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                 EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //EmployeeObj = WetosDB.Employees.Where(a => a.BranchId == Branchid && a.CompanyId == Companyid)
                   //.Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " " + a.FirstName + " " + a.LastName }).ToList();
                 EmployeeObj = WetosDB.SP_VwActiveEmployee(EmployeeId).Where(a => a.BranchId == Branchid && a.CompanyId == Companyid)
                   .Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " " + a.FirstName + " " + a.LastName }).ToList();
                #endregion
            }

            return Json(EmployeeObj);
            // Updated on Rajas on 8 FEB 2017 for reducing ajax return list and json return as per required parameter END
        }

        /// <summary>
        /// Common function to validate the data for edit or create
        /// Added by Rajas on 11 APRIL 2017
        /// </summary>
        private bool UpdateShiftScheduleData(ShiftScheduleModel ShiftScheduleModelObj, bool IsEdit, ref string UpdateStatus)
        {
            bool ReturnStatus = false;

            try
            {
                // a.DepartmentCode == DepartmentObj.DepartmentCode added by Rajas on 16 JAN 2017
                WetosDB.ShiftSchedule ShiftScheduleTblObj = WetosDB.ShiftSchedules.Where(a => a.ShiftScheduleId == ShiftScheduleModelObj.ShiftScheduleId).FirstOrDefault();

                //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 9 SEPTEMBER 2017 FOR EDIT
                WetosDB.ShiftSchedule ShiftScheduleTblObjEDIT = WetosDB.ShiftSchedules.Where(a => a.ShiftScheduleId == ShiftScheduleModelObj.ShiftScheduleId).FirstOrDefault();

                bool IsNew = false;
                if (ShiftScheduleTblObj == null && IsEdit == false)
                {
                    ShiftScheduleTblObj = new WetosDB.ShiftSchedule();

                    IsNew = true;
                }
                else  // Updated by Rajas on 15 MAY 2017
                {
                    UpdateStatus = "Inconsistent data detected, please try again!";

                    AddAuditTrail("Error in update function " + UpdateStatus);

                    return ReturnStatus;
                }

                // New Leave table object

                ShiftScheduleTblObj.RotationId = ShiftScheduleModelObj.RotationId;

                ShiftScheduleTblObj.EmployeeId = ShiftScheduleModelObj.EmployeeId;

                ShiftScheduleTblObj.ShiftMonth = ShiftScheduleModelObj.ShiftMonth;

                ShiftScheduleTblObj.ShiftYear = ShiftScheduleModelObj.ShiftYear;

                ShiftScheduleTblObj.Day1 = ShiftScheduleModelObj.Day1;

                ShiftScheduleTblObj.Day2 = ShiftScheduleModelObj.Day2;

                ShiftScheduleTblObj.Day3 = ShiftScheduleModelObj.Day3;

                ShiftScheduleTblObj.Day4 = ShiftScheduleModelObj.Day4;

                ShiftScheduleTblObj.Day5 = ShiftScheduleModelObj.Day5;

                ShiftScheduleTblObj.Day6 = ShiftScheduleModelObj.Day6;

                ShiftScheduleTblObj.Day7 = ShiftScheduleModelObj.Day7;

                ShiftScheduleTblObj.Day8 = ShiftScheduleModelObj.Day8;

                ShiftScheduleTblObj.Day9 = ShiftScheduleModelObj.Day9;

                ShiftScheduleTblObj.Day10 = ShiftScheduleModelObj.Day10;

                ShiftScheduleTblObj.Day11 = ShiftScheduleModelObj.Day11;

                ShiftScheduleTblObj.Day12 = ShiftScheduleModelObj.Day12;

                ShiftScheduleTblObj.Day13 = ShiftScheduleModelObj.Day13;

                ShiftScheduleTblObj.Day14 = ShiftScheduleModelObj.Day14;

                ShiftScheduleTblObj.Day15 = ShiftScheduleModelObj.Day15;

                ShiftScheduleTblObj.Day16 = ShiftScheduleModelObj.Day16;

                ShiftScheduleTblObj.Day17 = ShiftScheduleModelObj.Day17;

                ShiftScheduleTblObj.Day18 = ShiftScheduleModelObj.Day18;

                ShiftScheduleTblObj.Day19 = ShiftScheduleModelObj.Day19;

                ShiftScheduleTblObj.Day20 = ShiftScheduleModelObj.Day20;

                ShiftScheduleTblObj.Day21 = ShiftScheduleModelObj.Day21;

                ShiftScheduleTblObj.Day22 = ShiftScheduleModelObj.Day22;

                ShiftScheduleTblObj.Day23 = ShiftScheduleModelObj.Day23;

                ShiftScheduleTblObj.Day24 = ShiftScheduleModelObj.Day24;

                ShiftScheduleTblObj.Day25 = ShiftScheduleModelObj.Day25;

                ShiftScheduleTblObj.Day26 = ShiftScheduleModelObj.Day26;

                ShiftScheduleTblObj.Day27 = ShiftScheduleModelObj.Day27;

                ShiftScheduleTblObj.Day28 = ShiftScheduleModelObj.Day28;

                ShiftScheduleTblObj.Day29 = ShiftScheduleModelObj.Day29;

                ShiftScheduleTblObj.Day30 = ShiftScheduleModelObj.Day30;

                ShiftScheduleTblObj.Day31 = ShiftScheduleModelObj.Day31;

                ShiftScheduleTblObj.EmployeeGroupId = ShiftScheduleModelObj.EmployeeGroupId;

                // Add new table object 
                if (IsNew)
                {
                    VwActiveEmployee EmployeeObj = WetosDB.VwActiveEmployees.Where(a => a.EmployeeId == ShiftScheduleModelObj.EmployeeId).FirstOrDefault();

                    if (EmployeeObj != null)
                    {
                        ShiftScheduleTblObj.CompanyId = EmployeeObj.CompanyId;

                        ShiftScheduleTblObj.BranchId = EmployeeObj.BranchId;
                    }

                    WetosDB.ShiftSchedules.AddObject(ShiftScheduleTblObj);
                }

                WetosDB.SaveChanges();


                //-------------------------------------------AuditLog---------------------------------------------------------------------------

                if (IsNew)
                {
                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 11 SEPTEMBER 2017
                    #region ADD AUDIT LOG
                    //OLD RECORD IS BLANK
                    string Newrecord = "Rotation ID :" + ShiftScheduleTblObj.RotationId + ", EmployeeID : " + ShiftScheduleTblObj.EmployeeId
                        + ", EmployeeGroup ID : " + ShiftScheduleTblObj.EmployeeGroupId + ", Shift Month : " + ShiftScheduleTblObj.ShiftMonth
                        + ", Shift Year : " + ShiftScheduleTblObj.ShiftYear + ", CompanyId : " + ShiftScheduleTblObj.CompanyId
                        + ", BranchId : " + ShiftScheduleTblObj.BranchId;


                    //LOGIN ID IS TAKEN FROM SESSION PERSISTER
                    string Formname = "SHIFTSCHEDULE MASTER";
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
                    string Oldrecord = "Rotation ID :" + ShiftScheduleTblObjEDIT.RotationId + ", EmployeeID : " + ShiftScheduleTblObjEDIT.EmployeeId
                        + ", EmployeeGroup ID : " + ShiftScheduleTblObjEDIT.EmployeeGroupId + ", Shift Month : " + ShiftScheduleTblObjEDIT.ShiftMonth
                        + ", Shift Year : " + ShiftScheduleTblObjEDIT.ShiftYear + ", CompanyId : " + ShiftScheduleTblObjEDIT.CompanyId
                        + ", BranchId : " + ShiftScheduleTblObjEDIT.BranchId;
                    //---x---------x-----------------x------------x----------------x-----------x-------------x------------x--------------x----------x---------x-----
                    string Newrecord = "Rotation ID :" + ShiftScheduleTblObj.RotationId + ", EmployeeID : " + ShiftScheduleTblObj.EmployeeId
                        + ", EmployeeGroup ID : " + ShiftScheduleTblObj.EmployeeGroupId + ", Shift Month : " + ShiftScheduleTblObj.ShiftMonth
                        + ", Shift Year : " + ShiftScheduleTblObj.ShiftYear + ", CompanyId : " + ShiftScheduleTblObj.CompanyId
                        + ", BranchId : " + ShiftScheduleTblObj.BranchId;


                    //LOGIN ID IS TAKEN FROM SESSION PERSISTER
                    string Formname = "SHIFTSCHEDULE MASTER";
                    //ACTION IS UPDATE
                    string Message = " ";

                    WetosAdministrationController.GenerateAuditLogsUpdate(WetosDB, 0, Formname, Oldrecord,
                        Newrecord, ref Message);
                    #endregion
                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 11 SEPTEMBER 2017
                }

                //-------------------------------------------AuditLog---------------------------------------------------------------------------


                ReturnStatus = true;

                return ReturnStatus;
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message);

                UpdateStatus = "Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message);

                return ReturnStatus;
            }
        }
    }
}
