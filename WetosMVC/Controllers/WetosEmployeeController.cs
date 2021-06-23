using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//using Microsoft.Reporting.WebForms;
using WetosDB;
using WetosMVCMainApp.Models;

namespace WetosMVC.Controllers
{
    [SessionExpire] 
    [Authorize]
    public class WetosEmployeeController : BaseController
    {
        //
        // GET: /WetosEmployee/

        private void PopulateDropDown()
        {
            try
            {
                //CODE FOR DROPDOWN
                // code for company drop down list
                // Updated by Rajas on 6 MAY 2017 to get data which is not MarkedAsDelete
                #region COMPANY LIST CODE ADDED BY SHRADDHA ON 30 MAR 2018
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //var CompanyObj = WetosDB.Companies.Select(a => new { CompanyId = a.CompanyId, CompanyName = a.CompanyName }).ToList();
                var CompanyName = WetosDB.SP_GetCompanyList(EmployeeId).Select(m => new { Companyid = m.CompanyId, CompanyName = m.CompanyName }).ToList();
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
                #endregion
                ViewBag.CompanyNameList = new SelectList(CompanyName, "CompanyId", "CompanyName").ToList();

                // code for Department drop down list
                var Department = new List<Department>(); //WetosDB.Departments.Select(a => new { DepartmentId = a.DepartmentId, DepartmentName = a.DepartmentName }).ToList();
                ViewBag.DepartmentNameList = new SelectList(Department, "DepartmentId", "DepartmentName").ToList();

                // code for Division drop down list
                var Division = WetosDB.Divisions.Select(a => new { DIVISIONID = a.DIVISIONID, DIVISIONNAME = a.DIVISIONNAME }).ToList();
                ViewBag.DivisionNameList = new SelectList(Division, "DIVISIONID", "DIVISIONNAME").ToList();

                //drop down for branch name list
                var BranchName = new List<Branch>(); //WetosDB.Branches.Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList();
                ViewBag.BranchNameList = new SelectList(BranchName, "BranchId", "BranchName").ToList();

                //drop down for Grade name list
                var GradeName = WetosDB.Grades.Select(a => new { GradeId = a.GradeId, GradeName = a.GradeName }).ToList();
                ViewBag.GradeNameList = new SelectList(GradeName, "GradeId", "GradeName").ToList();

                // drop down for employeed type name
                // Updated by Rajas on 6 MAY 2017 to get data which is not MarkedAsDelete
                var EmployeeTypeName = WetosDB.EmployeeTypes.Where(a => a.MarkedAsDelete == null || a.MarkedAsDelete == 0).Select(a => new { EmployeeTypeId = a.EmployeeTypeId, EmployeeTypeName = a.EmployeeTypeName }).ToList();
                ViewBag.EmployeeTypeNameList = new SelectList(EmployeeTypeName, "EmployeeTypeId", "EmployeeTypeName").ToList();

                // drop down for Designation type name
                var DesignationName = new List<Designation>(); //WetosDB.Designations.Select(a => new { DesignationId = a.DesignationId, DesignationName = a.DesignationName }).ToList();
                ViewBag.DesignationNameList = new SelectList(DesignationName, "DesignationId", "DesignationName").ToList();

                // drop down for employee Group name
                var EmployeeGroupName = new List<EmployeeGroup>(); //WetosDB.EmployeeGroups.Select(a => new { EmployeeGroupId = a.EmployeeGroupId, EmployeeGroupName = a.EmployeeGroupName }).ToList();
                ViewBag.EmployeeGroupNameList = new SelectList(EmployeeGroupName, "EmployeeGroupId", "EmployeeGroupName").ToList();

                //Drop down list for Blood group
                var BloodGroupName = WetosDB.BloodGroups.Select(a => new { BloodGroupId = a.BloodGroupId, BloodGroupName = a.BloodGroupName }).ToList();
                ViewBag.BloodGroupNameList = new SelectList(BloodGroupName, "BloodGroupId", "BloodGroupName").ToList();

                //Drop down list for religion
                var ReligionName = WetosDB.Religions.Where(a=>a.MarkedAsDelete == null || a.MarkedAsDelete == 0).Select(a => new { ReligionId = a.ReligionId, ReligionName = a.ReligionName }).ToList();//Modified by dhanashri on 3 june 2019
                ViewBag.ReligionNameList = new SelectList(ReligionName, "ReligionId", "ReligionName").ToList();

                //Added by shraddha on 17th OCT 2016 For dropdown Employee Name

                // Updated by Rajas on 7 MARCH 2017 for active employees
                

                #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                //var EmployeeObj = WetosDB.VwActiveEmployees.Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList(); //ADDED EMPLOYEE CODE BY SHRADDHA ON 15 FEB 2018 
                var EmployeeObj = WetosDB.SP_VwActiveEmployee(EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
                #endregion
                
                ViewBag.EmployeeList = new SelectList(EmployeeObj, "EmployeeId", "EmployeeName").ToList();

                //Added By Shraddha on 28 DEC 2016

                List<SelectListItem> TitleList = new List<SelectListItem>();
                TitleList.Add(new SelectListItem { Text = "Mr.", Value = "Mr." });
                TitleList.Add(new SelectListItem { Text = "Ms.", Value = "Ms." });
                TitleList.Add(new SelectListItem { Text = "Mrs.", Value = "Mrs." });
                ViewBag.TitleListVB = new SelectList(TitleList, "Value", "Text").ToList();

                // Added By Rajas on 11 JAN 2017 for dropdown list selection for weekoff1 and weekoff2 start

                List<SelectListItem> DayList = new List<SelectListItem>();
                DayList.Add(new SelectListItem { Text = "SUNDAY", Value = "SUNDAY" });
                DayList.Add(new SelectListItem { Text = "MONDAY", Value = "MONDAY" });
                DayList.Add(new SelectListItem { Text = "TUESDAY", Value = "TUESDAY" });
                DayList.Add(new SelectListItem { Text = "WEDNESDAY", Value = "WEDNESDAY" });
                DayList.Add(new SelectListItem { Text = "THURSDAY", Value = "THURSDAY" });
                DayList.Add(new SelectListItem { Text = "FRIDAY", Value = "FRIDAY" });
                DayList.Add(new SelectListItem { Text = "SATURDAY", Value = "SATURDAY" });
                ViewBag.DayDdlList = new SelectList(DayList, "Value", "Text").ToList();
                // Added By Rajas on 11 JAN 2017 for dropdown list selection for weekoff1 and weekoff2 END


                // Added By Rajas on 11 JAN 2017 for dropdown list selection for Default shift START

                //MODIFIED BY SHRADDHA ON 15 MARCH 2017 TAKEN SHIFT CODE IN BOTH PLACES SIFTID AND SHIFTCODE TO HANDLE SHIFT CODE EXCEPTION IN POSTING PROCESS START
                // Updated by Rajas on 2 JUNE 2017
                var ShiftObj = new List<Shift>(); // WetosDB.Shifts.Select(a => new { ShiftId = a.ShiftCode, ShiftCode = a.ShiftCode }).ToList();
                ViewBag.ShiftList = new SelectList(ShiftObj, "ShiftId", "ShiftCode").ToList();

                // Added By Rajas on 11 JAN 2017 for dropdown list selection for Default shift END
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );
            }
        }

        /// <summary>
        /// added by Rajas on 3 JUNE 2017
        /// </summary>
        private void PopulateDropDownEdit(EmployeeModel EmployeeModelObj)
        {
            try
            {
                //CODE FOR DROPDOWN
                // code for company drop down list
                // Updated by Rajas on 6 MAY 2017 to get data which is not MarkedAsDelete START
                #region COMPANY LIST CODE ADDED BY SHRADDHA ON 30 MAR 2018
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //var CompanyObj = WetosDB.Companies.Select(a => new { CompanyId = a.CompanyId, CompanyName = a.CompanyName }).ToList();
                var CompanyName = WetosDB.SP_GetCompanyList(EmployeeId).Select(m => new { Companyid = m.CompanyId, CompanyName = m.CompanyName }).ToList();
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
                #endregion
                ViewBag.CompanyNameList = new SelectList(CompanyName, "CompanyId", "CompanyName").ToList();

                // code for Department drop down list
                var Department = WetosDB.Departments.Where(a => (a.MarkedAsDelete == null || a.MarkedAsDelete == 0) && a.Branch.BranchId == EmployeeModelObj.BranchId
                    && a.Company.CompanyId == EmployeeModelObj.CompanyId)
                    .Select(a => new { DepartmentId = a.DepartmentId, DepartmentName = a.DepartmentCode + " - " + a.DepartmentName }).ToList(); //ADDED DEPARTMENT CODE BY SHRADDHA ON 15 FEB 2018 
                ViewBag.DepartmentNameList = new SelectList(Department, "DepartmentId", "DepartmentName").ToList();

                // code for Division drop down list
                var Division = WetosDB.Divisions.Where(a => a.Branch.BranchId == EmployeeModelObj.BranchId && a.Company.CompanyId == EmployeeModelObj.CompanyId)
                    .Select(a => new { DIVISIONID = a.DIVISIONID, DIVISIONNAME = a.DIVISIONNAME }).ToList();
                ViewBag.DivisionNameList = new SelectList(Division, "DIVISIONID", "DIVISIONNAME").ToList();

                //drop down for branch name list
               
                #region CODE FOR GET BRANCH LIST ADDED BY SHRADDHA ON 30 MAR 2018
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
                // var BranchName = WetosDB.Branches.Where(a => (a.MarkedAsDelete == null || a.MarkedAsDelete == 0) && a.Company.CompanyId == EmployeeModelObj.CompanyId)
                   // .Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList();
                var BranchName = WetosDB.SP_GetBranchList(EmployeeId).Where(a => a.CompanyId == EmployeeModelObj.CompanyId).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList(); //.Select(m => new { BranchId = m.BranchId, BranchName = m.BranchName}).ToList();
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
                #endregion

                ViewBag.BranchNameList = new SelectList(BranchName, "BranchId", "BranchName").ToList();

                //drop down for Grade name list
                var GradeName = WetosDB.Grades.Where(a => (a.MarkedAsDelete == null || a.MarkedAsDelete == 0) && a.Branch.BranchId == EmployeeModelObj.BranchId
                    && a.Company.CompanyId == EmployeeModelObj.CompanyId)
                    .Select(a => new { GradeId = a.GradeId, GradeName = a.GradeName }).ToList();
                ViewBag.GradeNameList = new SelectList(GradeName, "GradeId", "GradeName").ToList();

                // drop down for employeed type name
                var EmployeeTypeName = WetosDB.EmployeeTypes.Where(a => a.MarkedAsDelete == null || a.MarkedAsDelete == 0)
                    .Select(a => new { EmployeeTypeId = a.EmployeeTypeId, EmployeeTypeName = a.EmployeeTypeName }).ToList();
                ViewBag.EmployeeTypeNameList = new SelectList(EmployeeTypeName, "EmployeeTypeId", "EmployeeTypeName").ToList();

                // drop down for Designation type name
                var DesignationName = WetosDB.Designations.Where(a => (a.MarkedAsDelete == null || a.MarkedAsDelete == 0) && a.Branch.BranchId == EmployeeModelObj.BranchId
                    && a.Company.CompanyId == EmployeeModelObj.CompanyId)
                    .Select(a => new { DesignationId = a.DesignationId, DesignationName = a.DesignationName }).ToList();
                ViewBag.DesignationNameList = new SelectList(DesignationName, "DesignationId", "DesignationName").ToList();

                // drop down for employee Group name
                var EmployeeGroupName = WetosDB.EmployeeGroups.Where(a => (a.MarkedAsDelete == null || a.MarkedAsDelete == 0)
                    && a.Company.CompanyId == EmployeeModelObj.CompanyId && a.Branch.BranchId == EmployeeModelObj.BranchId)
                    .Select(a => new { EmployeeGroupId = a.EmployeeGroupId, EmployeeGroupName = a.EmployeeGroupName }).ToList();
                ViewBag.EmployeeGroupNameList = new SelectList(EmployeeGroupName, "EmployeeGroupId", "EmployeeGroupName").ToList();

                //Drop down list for Blood group
                var BloodGroupName = WetosDB.BloodGroups.Select(a => new { BloodGroupId = a.BloodGroupId, BloodGroupName = a.BloodGroupName }).ToList();
                ViewBag.BloodGroupNameList = new SelectList(BloodGroupName, "BloodGroupId", "BloodGroupName").ToList();

                //Drop down list for religion
                var ReligionName = WetosDB.Religions.Select(a => new { ReligionId = a.ReligionId, ReligionName = a.ReligionName }).ToList();
                ViewBag.ReligionNameList = new SelectList(ReligionName, "ReligionId", "ReligionName").ToList();

                // Updated by Rajas on 7 MARCH 2017 for active employee list
                
                #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                //var EmployeeObj = WetosDB.VwActiveEmployees.Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList(); //ADDED EMPLOYEE CODE BY SHRADDHA ON 15 FEB 2018 
                var EmployeeObj = WetosDB.SP_VwActiveEmployee(EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
                #endregion
                
                ViewBag.EmployeeList = new SelectList(EmployeeObj, "EmployeeId", "EmployeeName").ToList();
                // Updated by Rajas on 6 MAY 2017 to get data which is not MarkedAsDelete END

                //Added By Shraddha on 28 DEC 2016

                List<SelectListItem> TitleList = new List<SelectListItem>();
                TitleList.Add(new SelectListItem { Text = "Mr.", Value = "Mr." });
                TitleList.Add(new SelectListItem { Text = "Ms.", Value = "Ms." });
                TitleList.Add(new SelectListItem { Text = "Mrs.", Value = "Mrs." });
                ViewBag.TitleListVB = new SelectList(TitleList, "Value", "Text").ToList();

                // Added By Rajas on 11 JAN 2017 for dropdown list selection for weekoff1 and weekoff2 start

                List<SelectListItem> DayList = new List<SelectListItem>();
                DayList.Add(new SelectListItem { Text = "SUNDAY", Value = "SUNDAY" });
                DayList.Add(new SelectListItem { Text = "MONDAY", Value = "MONDAY" });
                DayList.Add(new SelectListItem { Text = "TUESDAY", Value = "TUESDAY" });
                DayList.Add(new SelectListItem { Text = "WEDNESDAY", Value = "WEDNESDAY" });
                DayList.Add(new SelectListItem { Text = "THURSDAY", Value = "THURSDAY" });
                DayList.Add(new SelectListItem { Text = "FRIDAY", Value = "FRIDAY" });
                DayList.Add(new SelectListItem { Text = "SATURDAY", Value = "SATURDAY" });
                ViewBag.DayDdlList = new SelectList(DayList, "Value", "Text").ToList();
                // Added By Rajas on 11 JAN 2017 for dropdown list selection for weekoff1 and weekoff2 END


                // Added By Rajas on 11 JAN 2017 for dropdown list selection for Default shift START

                var ShiftObj = WetosDB.Shifts.Where(a => a.Company.CompanyId == EmployeeModelObj.CompanyId && a.BranchId == EmployeeModelObj.BranchId)
                    .Select(a => new { ShiftId = a.ShiftCode, ShiftCode = a.ShiftCode }).ToList();
                ViewBag.ShiftList = new SelectList(ShiftObj, "ShiftId", "ShiftCode").ToList();

                // Added By Rajas on 11 JAN 2017 for dropdown list selection for Default shift END
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );
            }
        }

        /// <summary>
        /// To show active employees by default
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            try
            {
                // Updated by Rajas on 20 MARCH 2017
                #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN
                //List<SP_ActiveEmployeeInEmployeeMaster_Result> ActiveEmployeeList = new List<SP_ActiveEmployeeInEmployeeMaster_Result>();
                List<SP_ActiveEmployeeInEmployeeMasterNew_Result> ActiveEmployeeList = new List<SP_ActiveEmployeeInEmployeeMasterNew_Result>();
                #endregion
                // Updated by Rajas on 15 JUNE 2017
                DateTime DefaultDate = new DateTime(1900, 01, 01);
                
                #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //ActiveEmployeeList = WetosDB.SP_ActiveEmployeeInEmployeeMaster().Where(a => a.Leavingdate == null || a.Leavingdate > DateTime.Now.Date).ToList(); // CODE ADDED a.Leavingdate > DateTime.Now.Date BY SHRADDHA ON 06 MARCH 2018
                ActiveEmployeeList = WetosDB.SP_ActiveEmployeeInEmployeeMasterNew(EmployeeId).Where(a => a.Leavingdate == null || a.Leavingdate > DateTime.Now.Date).ToList(); // CODE ADDED a.Leavingdate > DateTime.Now.Date BY SHRADDHA ON 06 MARCH 2018
                #endregion

                //ADDED BY RAJAS ON 27 DEC 2016
                AddAuditTrail("Success - Visited Employee master"); // Updated by Rajas on 16 JAN 2017

                return View(ActiveEmployeeList);

            }
            catch (System.Exception ex)
            {
                // Added by Rajas on 16 JAN 2017
                AddAuditTrail("Exception - " + ex.Message);

                return RedirectToAction("Index");
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult ExportData()
        {
            try
            {
                //ADDED BY NANDINI ON 21 JULY 2020
                System.Web.UI.WebControls.GridView gv = new System.Web.UI.WebControls.GridView();
                gv.DataSource = WetosDB.usp_Report_EmployeeDetails().ToList();
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=EmployeeData.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                System.IO.StringWriter sw = new System.IO.StringWriter();
                System.Web.UI.HtmlTextWriter htw = new System.Web.UI.HtmlTextWriter(sw);
                gv.RenderControl(htw);
                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();

                return RedirectToAction("Index");
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in Export employee due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Export employee data failed.");

                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// To show all employees on employee master
        /// Added by Rajas on 20 MARCH 2017
        /// </summary>
        /// <returns></returns>
        public ActionResult AllEmployees()
        {
            try
            {
                List<SP_EmployeeDetailsOnEmployeeMaster_Result> AllEmployeeList = new List<SP_EmployeeDetailsOnEmployeeMaster_Result>();

                AllEmployeeList = WetosDB.SP_EmployeeDetailsOnEmployeeMaster().ToList();

                //ADDED BY RAJAS ON 27 DEC 2016
                AddAuditTrail("Success - Visited Employee master for all employees"); // Updated by Rajas on 16 JAN 2017

                return View(AllEmployeeList);
            }
            catch (System.Exception ex)
            {
                // Added by Rajas on 16 JAN 2017
                AddAuditTrail("Exception - " + ex.Message);

                return RedirectToAction("Index");
            }
        }

        //
        // GET: /WetosEmployee/Details/5

        /// <summary>
        /// View Employee Details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int id)
        {
            try
            {
                //ADDED BY RAJAS ON 27 DEC 2016
                AddAuditTrail("Employee profile checked"); // Updated on 16 JAN 2017 by Rajas

                return RedirectToAction("Profile", "WetosEmployee", new { id = id });
            }

            catch (System.Exception ex)
            {
                // Added by Rajas on 16 JAN 2017
                AddAuditTrail("Exception - " + ex.Message);

                return View();
            }
        }

        //
        // GET: /WetosEmployee/Create

        public ActionResult Create(int submitted = 0, int EmployeeId = 0) //ADDED PARAMETERS BY SHRADDHA ON 20 JULY 2017
        {
            try
            {
                // Added by Rajas to find out max of employee code on 14 JAN 2017 START

                // Commented by Raja on 17 FEB 2017 START
                //Employee LastEmployee = WetosDB.Employees.OrderByDescending(a => a.EmployeeId).FirstOrDefault();
                //int MaxOfEmployee = 0;

                //int Max = Convert.ToInt32(LastEmployee.EmployeeCode);

                //int MaxEmpCode = Max + 1;

                //string MaxEmployeeCode = Convert.ToString(MaxEmpCode);
                // Commented by Raja on 17 FEB 2017 END

                #region Employee Code Max Logic Added By Shraddha on 23 FEB 2017 Temporary commented (If require in future then use it)
                //String LastEmployeeCode = WetosDB.Employees.Select(a=>a.EmployeeCode).Max();
                //int MaxEmpCode = Convert.ToInt32(LastEmployeeCode) + 1;
                //string MaxEmployeeCode = Convert.ToString(MaxEmpCode);
                #endregion

                EmployeeModel NewEmployeeModelObj = new EmployeeModel();

                // Commented by Raja on 17 FEB 2017
                //NewEmployeeModelObj.EmployeeCode = MaxEmployeeCode;
                //NewEmployeeModelObj.CardNumber = MaxEmployeeCode;

                NewEmployeeModelObj.First = false;
                NewEmployeeModelObj.Fifth = false;
                NewEmployeeModelObj.Second = false;
                NewEmployeeModelObj.Third = false;
                NewEmployeeModelObj.Fourth = false;
                NewEmployeeModelObj.IsSecondWeekoffHalfDay = false; //ADDED BY SHRADDHA ON 12 MAR 2018
                // Added by Rajas to find out max of employee code on 14 JAN 2017 END


                // POPULATE VIEWBAG
                PopulateDropDown();

                // ADDED by SHRADDHA on 20 JULY 2017 START
                if (submitted == 1)
                {
                    ViewBag.Result = "Appointment";
                    NewEmployeeModelObj.EmployeeId = EmployeeId;
                }
                else if (submitted == 2)
                {
                    ViewBag.Result = "Confirm";
                    NewEmployeeModelObj.EmployeeId = EmployeeId;
                }
                else if (submitted == 3)
                {
                    ViewBag.Result = "ConfirmWithInc";
                    NewEmployeeModelObj.EmployeeId = EmployeeId;
                }
                else if (submitted == 4)
                {
                    ViewBag.Result = "ServiceBond";
                    NewEmployeeModelObj.EmployeeId = EmployeeId;
                }
                else
                {
                    ViewBag.Result = string.Empty;
                }
                // ADDED by SHRADDHA on 20 JULY 2017 END

                #region EMPLOYEEMODEL.TBLFAMILYDETAILSLIST
                //CODE ADDED BY SHRADDHA ON 10 FEB 2018 START
                //GlobalSettingsConstant FamilyDetailsMaxRecords 
                GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.FamilyDetailsMaxRecords).FirstOrDefault();
                int FamilyDetailsMaxRecordsCount = 0;
                if (GlobalSettingObj != null)
                {
                    FamilyDetailsMaxRecordsCount = Convert.ToInt32(GlobalSettingObj.SettingValue);
                }

                NewEmployeeModelObj.FamilyDetailsMaxRecordsCount = FamilyDetailsMaxRecordsCount;

                if ((NewEmployeeModelObj.TblFamilyDetailList == null ? 0 : NewEmployeeModelObj.TblFamilyDetailList.Count) == 0)
                {
                    NewEmployeeModelObj.TblFamilyDetailList = new List<FamilyDetails>();
                    for (int i = 0; i < FamilyDetailsMaxRecordsCount; i++)
                    {
                        NewEmployeeModelObj.TblFamilyDetailList.Add(new FamilyDetails());
                    }

                }
                //CODE ADDED BY SHRADDHA ON 10 FEB 2018 END
                #endregion

                return View(NewEmployeeModelObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));
                ViewBag.Result = string.Empty;
                Error("Please try again!");

                return RedirectToAction("Index");  // Verify ?
            }

        }

        //
        // POST: /WetosEmployee/Create

        /// <summary>
        /// EMPLOYEE POST WITH VALIDATE DATA
        /// ADDED BY RAJAS ON 30 DEC 2016
        /// </summary>
        /// <param name="EmployeeModelObj"></param>
        /// <param name="UploadFileEmployee"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        /// Updated Rajas on 3 JUNE 2017
        [HttpPost]
        public ActionResult Create(EmployeeModel EmployeeModelObj, HttpPostedFileBase UploadFileEmployee, FormCollection collection)
        {
            try
            {
                string UpdateStatus = string.Empty;  // Added by Rajas on 16 MARCH 2017

                // CHECK EMPLOYEE ALREADY EXIST // ADDED BY MSJ ON 11 JAN 2018 START
                Employee ExistingEmployee = WetosDB.Employees.Where(a => (a.FirstName.ToUpper() == EmployeeModelObj.FirstName.ToUpper()
                    //&& a.MiddleName.ToUpper() == EmployeeModelObj.MiddleName.ToUpper()
                    && a.LastName.ToUpper() == EmployeeModelObj.LastName.ToUpper()
                    && a.Gender == EmployeeModelObj.Gender) || (a.EmployeeCode == EmployeeModelObj.EmployeeCode) || (a.Email == EmployeeModelObj.Email && EmployeeModelObj.Email != null) //ADDED EMPLOYEE CODE AND EMPLOYEE EMAIL ID CONDITION BY SHRADDHA ON 16 JAN 2018
                    ).FirstOrDefault();

                if (ExistingEmployee != null) // CHECKING
                {
                    ModelState.AddModelError("", "Employee already present!!!");
                    PopulateDropDown();

                    // ADDED BY MSJ ON 13 JAN 2018 START
                    EmployeeModelObj.First = EmployeeModelObj.First == null ? false : EmployeeModelObj.First.Value;
                    EmployeeModelObj.Second = EmployeeModelObj.Second == null ? false : EmployeeModelObj.Second.Value;
                    EmployeeModelObj.Third = EmployeeModelObj.Third == null ? false : EmployeeModelObj.Third.Value;
                    EmployeeModelObj.Fourth = EmployeeModelObj.Fourth == null ? false : EmployeeModelObj.Fourth.Value;
                    EmployeeModelObj.Fifth = EmployeeModelObj.Fifth == null ? false : EmployeeModelObj.Fifth.Value;
                    EmployeeModelObj.IsSecondWeekoffHalfDay = EmployeeModelObj.IsSecondWeekoffHalfDay == null ? false : EmployeeModelObj.IsSecondWeekoffHalfDay.Value;// CODE ADDED BY SHRADDHA ON 12 MAR 2018
                    // ADDED BY MSJ ON 13 JAN 2018 END

                    #region EMPLOYEEMODEL.TBLFAMILYDETAILSLIST
                    //CODE ADDED BY SHRADDHA ON 10 FEB 2018 START
                    //GlobalSettingsConstant FamilyDetailsMaxRecords 
                    GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.FamilyDetailsMaxRecords).FirstOrDefault();
                    int FamilyDetailsMaxRecordsCount = 0;
                    if (GlobalSettingObj != null)
                    {
                        FamilyDetailsMaxRecordsCount = Convert.ToInt32(GlobalSettingObj.SettingValue);
                    }
                    EmployeeModelObj.FamilyDetailsMaxRecordsCount = FamilyDetailsMaxRecordsCount;
                    if ((EmployeeModelObj.TblFamilyDetailList == null ? 0 : EmployeeModelObj.TblFamilyDetailList.Count) == 0)
                    {
                        EmployeeModelObj.TblFamilyDetailList = new List<FamilyDetails>();
                        for (int i = 0; i < FamilyDetailsMaxRecordsCount; i++)
                        {
                            EmployeeModelObj.TblFamilyDetailList.Add(new FamilyDetails());
                        }

                    }
                    //CODE ADDED BY SHRADDHA ON 10 FEB 2018 END
                    #endregion

                    return View(EmployeeModelObj);
                }
                else
                {
                    if (EmployeeModelObj.Gender.ToUpper() == "F")
                    {
                        if (EmployeeModelObj.Prefix.ToUpper() == "MR" || EmployeeModelObj.Prefix.ToUpper() == "MR.")
                        {
                            ModelState.AddModelError("", "Inconsist Gender and Prefix.");

                            PopulateDropDown();

                            // ADDED BY MSJ ON 13 JAN 2018 START
                            EmployeeModelObj.First = EmployeeModelObj.First == null ? false : EmployeeModelObj.First.Value;
                            EmployeeModelObj.Second = EmployeeModelObj.Second == null ? false : EmployeeModelObj.Second.Value;
                            EmployeeModelObj.Third = EmployeeModelObj.Third == null ? false : EmployeeModelObj.Third.Value;
                            EmployeeModelObj.Fourth = EmployeeModelObj.Fourth == null ? false : EmployeeModelObj.Fourth.Value;
                            EmployeeModelObj.Fifth = EmployeeModelObj.Fifth == null ? false : EmployeeModelObj.Fifth.Value;
                            EmployeeModelObj.IsSecondWeekoffHalfDay = EmployeeModelObj.IsSecondWeekoffHalfDay == null ? false : EmployeeModelObj.IsSecondWeekoffHalfDay.Value;// CODE ADDED BY SHRADDHA ON 12 MAR 2018
                            // ADDED BY MSJ ON 13 JAN 2018 END

                            #region EMPLOYEEMODEL.TBLFAMILYDETAILSLIST
                            //CODE ADDED BY SHRADDHA ON 10 FEB 2018 START
                            //GlobalSettingsConstant FamilyDetailsMaxRecords 
                            GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.FamilyDetailsMaxRecords).FirstOrDefault();
                            int FamilyDetailsMaxRecordsCount = 0;
                            if (GlobalSettingObj != null)
                            {
                                FamilyDetailsMaxRecordsCount = Convert.ToInt32(GlobalSettingObj.SettingValue);
                            }
                            EmployeeModelObj.FamilyDetailsMaxRecordsCount = FamilyDetailsMaxRecordsCount;
                            if ((EmployeeModelObj.TblFamilyDetailList == null ? 0 : EmployeeModelObj.TblFamilyDetailList.Count) == 0)
                            {
                                EmployeeModelObj.TblFamilyDetailList = new List<FamilyDetails>();
                                for (int i = 0; i < FamilyDetailsMaxRecordsCount; i++)
                                {
                                    EmployeeModelObj.TblFamilyDetailList.Add(new FamilyDetails());
                                }

                            }
                            //CODE ADDED BY SHRADDHA ON 10 FEB 2018 END
                            #endregion

                            return View(EmployeeModelObj);
                        }
                    }

                }

                //CODE ADDED BY SHRADDHA ON 06 MARCH 2018 START
                if (EmployeeModelObj.Leavingdate != null && EmployeeModelObj.SeperationRemark == null) // CHECKING
                {
                    ModelState.AddModelError("", "Seperation Remark is mandatory in case of seperation date is entered");
                    PopulateDropDown();

                    // ADDED BY MSJ ON 13 JAN 2018 START
                    EmployeeModelObj.First = EmployeeModelObj.First == null ? false : EmployeeModelObj.First.Value;
                    EmployeeModelObj.Second = EmployeeModelObj.Second == null ? false : EmployeeModelObj.Second.Value;
                    EmployeeModelObj.Third = EmployeeModelObj.Third == null ? false : EmployeeModelObj.Third.Value;
                    EmployeeModelObj.Fourth = EmployeeModelObj.Fourth == null ? false : EmployeeModelObj.Fourth.Value;
                    EmployeeModelObj.Fifth = EmployeeModelObj.Fifth == null ? false : EmployeeModelObj.Fifth.Value;
                    EmployeeModelObj.IsSecondWeekoffHalfDay = EmployeeModelObj.IsSecondWeekoffHalfDay == null ? false : EmployeeModelObj.IsSecondWeekoffHalfDay.Value;// CODE ADDED BY SHRADDHA ON 12 MAR 2018
                    // ADDED BY MSJ ON 13 JAN 2018 END

                    #region EMPLOYEEMODEL.TBLFAMILYDETAILSLIST
                    //CODE ADDED BY SHRADDHA ON 10 FEB 2018 START
                    //GlobalSettingsConstant FamilyDetailsMaxRecords 
                    GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.FamilyDetailsMaxRecords).FirstOrDefault();
                    int FamilyDetailsMaxRecordsCount = 0;
                    if (GlobalSettingObj != null)
                    {
                        FamilyDetailsMaxRecordsCount = Convert.ToInt32(GlobalSettingObj.SettingValue);
                    }
                    EmployeeModelObj.FamilyDetailsMaxRecordsCount = FamilyDetailsMaxRecordsCount;
                    if ((EmployeeModelObj.TblFamilyDetailList == null ? 0 : EmployeeModelObj.TblFamilyDetailList.Count) == 0)
                    {
                        EmployeeModelObj.TblFamilyDetailList = new List<FamilyDetails>();
                        for (int i = 0; i < FamilyDetailsMaxRecordsCount; i++)
                        {
                            EmployeeModelObj.TblFamilyDetailList.Add(new FamilyDetails());
                        }

                    }
                    //CODE ADDED BY SHRADDHA ON 10 FEB 2018 END
                    #endregion

                    return View(EmployeeModelObj);
                }
                //CODE ADDED BY SHRADDHA ON 06 MARCH 2018 END

                //CODE ADDED BY SHRADDHA O 24 FEB 2018 FOR FAMILY MEMBER BIRTH DATE SHOULD NOT BE GREATER THAN TODAY VALIDATION START
                foreach (FamilyDetails FamilyDetailsObjNew in EmployeeModelObj.TblFamilyDetailList)
                {
                    if (!string.IsNullOrEmpty(FamilyDetailsObjNew.DateOfBirth))
                    {
                        DateTime DateOfBirth = Convert.ToDateTime(FamilyDetailsObjNew.DateOfBirth);
                        if (DateOfBirth > DateTime.Now)
                        {
                            //ModelState.AddModelError("", "Age limit should be greater than 18 years");

                            PopulateDropDownEdit(EmployeeModelObj);

                            // Added by Rajas on 16 JAN 2017 START
                            AddAuditTrail("Error - Birth date of any Family Member should not be greater than Today's date");

                            Error("Error - Birth date of any Family Member should not be greater than Today's date");

                            // Added by Rajas on 17 APRIL 2017, function to get checkbox values
                            GetCheckBoxValues(EmployeeModelObj, collection);

                            //REMOVED ENTIRE REGION CODE ADDED THAT REGION IN FUNCTION GETFAMILYDETAILS AND CALLED THAT REGION HERE TO REMOVE REDUNDETNT CODE START
                            GETFAMILYDETAILS(ref EmployeeModelObj);
                            //REMOVED ENTIRE REGION CODE ADDED THAT REGION IN FUNCTION GETFAMILYDETAILS AND CALLED THAT REGION HERE TO REMOVE REDUNDETNT CODE END
                            
                            return View(EmployeeModelObj);
                        }
                    }

                }
                //CODE ADDED BY SHRADDHA O 24 FEB 2018 FOR FAMILY MEMBER BIRTH DATE SHOULD NOT BE GREATER THAN TODAY VALIDATION END
                // CHECK EMPLOYEE ALREADY EXIST // ADDED BY MSJ ON 11 JAN 2018 END

                // Added by Rajas on 15 MAY 2017
                bool IsEdit = false;


                // Added by Rajas on 17 APRIL 2017, function to get checkbox values
                GetCheckBoxValues(EmployeeModelObj, collection);

                #region DATE VALIDATIONS

                // Region updated by Rajas on 17 APRIL 2017 START

                // Check birth date validation 
                DateTime Today = DateTime.Now;

                DateTime DOB = Today.AddYears(-18);

                if (Today.Date >= DOB.Date && EmployeeModelObj.BirthDate >= DOB.Date)
                {
                    //ModelState.AddModelError("", "Age limit should be greater than 18 years");

                    PopulateDropDownEdit(EmployeeModelObj);

                    // Added by Rajas on 16 JAN 2017 START
                    AddAuditTrail("Error - Employee can not be added due to invalid birth date");

                    Error("Age limit should be greater than 18 years");

                    // Added by Rajas on 17 APRIL 2017, function to get checkbox values
                    GetCheckBoxValues(EmployeeModelObj, collection);

                    // ADDED BY MSJ ON 13 JAN 2018 START
                    EmployeeModelObj.First = EmployeeModelObj.First == null ? false : EmployeeModelObj.First.Value;
                    EmployeeModelObj.Second = EmployeeModelObj.Second == null ? false : EmployeeModelObj.Second.Value;
                    EmployeeModelObj.Third = EmployeeModelObj.Third == null ? false : EmployeeModelObj.Third.Value;
                    EmployeeModelObj.Fourth = EmployeeModelObj.Fourth == null ? false : EmployeeModelObj.Fourth.Value;
                    EmployeeModelObj.Fifth = EmployeeModelObj.Fifth == null ? false : EmployeeModelObj.Fifth.Value;
                    // ADDED BY MSJ ON 13 JAN 2018 END

                    #region EMPLOYEEMODEL.TBLFAMILYDETAILSLIST
                    //CODE ADDED BY SHRADDHA ON 10 FEB 2018 START
                    //GlobalSettingsConstant FamilyDetailsMaxRecords 
                    GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.FamilyDetailsMaxRecords).FirstOrDefault();
                    int FamilyDetailsMaxRecordsCount = 0;
                    if (GlobalSettingObj != null)
                    {
                        FamilyDetailsMaxRecordsCount = Convert.ToInt32(GlobalSettingObj.SettingValue);
                    }
                    EmployeeModelObj.FamilyDetailsMaxRecordsCount = FamilyDetailsMaxRecordsCount;
                    if ((EmployeeModelObj.TblFamilyDetailList == null ? 0 : EmployeeModelObj.TblFamilyDetailList.Count) == 0)
                    {
                        EmployeeModelObj.TblFamilyDetailList = new List<FamilyDetails>();
                        for (int i = 0; i < FamilyDetailsMaxRecordsCount; i++)
                        {
                            EmployeeModelObj.TblFamilyDetailList.Add(new FamilyDetails());
                        }

                    }
                    //CODE ADDED BY SHRADDHA ON 10 FEB 2018 END
                    #endregion

                    return View(EmployeeModelObj);
                }


                if (EmployeeModelObj.JoiningDate > EmployeeModelObj.ConfirmDate) // MODIFIED CONDITION TO CHECK JOINING DATE BY MSJ ON 09 JAN 2018
                {
                    PopulateDropDownEdit(EmployeeModelObj);

                    // Added by Rajas on 16 JAN 2017 START
                    AddAuditTrail("Error - Employee can not be added due to invalid birth date");

                    Error("Invalid Date of Confirmation");

                    // Added by Rajas on 17 APRIL 2017, function to get checkbox values
                    GetCheckBoxValues(EmployeeModelObj, collection);

                    // ADDED BY MSJ ON 13 JAN 2018 START
                    EmployeeModelObj.First = EmployeeModelObj.First == null ? false : EmployeeModelObj.First.Value;
                    EmployeeModelObj.Second = EmployeeModelObj.Second == null ? false : EmployeeModelObj.Second.Value;
                    EmployeeModelObj.Third = EmployeeModelObj.Third == null ? false : EmployeeModelObj.Third.Value;
                    EmployeeModelObj.Fourth = EmployeeModelObj.Fourth == null ? false : EmployeeModelObj.Fourth.Value;
                    EmployeeModelObj.Fifth = EmployeeModelObj.Fifth == null ? false : EmployeeModelObj.Fifth.Value;
                    // ADDED BY MSJ ON 13 JAN 2018 END

                    #region EMPLOYEEMODEL.TBLFAMILYDETAILSLIST
                    //CODE ADDED BY SHRADDHA ON 10 FEB 2018 START
                    //GlobalSettingsConstant FamilyDetailsMaxRecords 
                    GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.FamilyDetailsMaxRecords).FirstOrDefault();
                    int FamilyDetailsMaxRecordsCount = 0;
                    if (GlobalSettingObj != null)
                    {
                        FamilyDetailsMaxRecordsCount = Convert.ToInt32(GlobalSettingObj.SettingValue);
                    }
                    EmployeeModelObj.FamilyDetailsMaxRecordsCount = FamilyDetailsMaxRecordsCount;
                    if ((EmployeeModelObj.TblFamilyDetailList == null ? 0 : EmployeeModelObj.TblFamilyDetailList.Count) == 0)
                    {
                        EmployeeModelObj.TblFamilyDetailList = new List<FamilyDetails>();
                        for (int i = 0; i < FamilyDetailsMaxRecordsCount; i++)
                        {
                            EmployeeModelObj.TblFamilyDetailList.Add(new FamilyDetails());
                        }

                    }
                    //CODE ADDED BY SHRADDHA ON 10 FEB 2018 END
                    #endregion

                    return View(EmployeeModelObj);

                }




                // Region updated by Rajas on 17 APRIL 2017 End
                #endregion

                //if (EmployeeModelObj.WeeklyOff1 == EmployeeModelObj.WeeklyOff2)
                //{
                //    AddAuditTrail("Invalid weekoff2, as Weekoff1 and weekoff2 are same");

                //    Error("Weekoff2 and WeekOff 1 can't be same");

                //    PopulateDropDownEdit();

                //    return View(EmployeeModelObj);
                //}

                /// To validate if Employee reporting Sanctioner kept blank then treat both reporting person as same
                /// Added by Rajas on 17 APRIL 2017
                if (EmployeeModelObj.EmployeeReportingId2 == 0)
                {
                    EmployeeModelObj.EmployeeReportingId2 = Convert.ToInt32(EmployeeModelObj.EmployeeReportingId);
                }

                if (ModelState.IsValid)
                {
                    Employee EmployeeObj = new Employee();
                    if (UploadFileEmployee != null && UploadFileEmployee.ContentLength > 1048576)
                    {
                        ModelState.AddModelError("", "Photo should not be greater than 1 MB.");

                        PopulateDropDown();

                        // ADDED BY MSJ ON 13 JAN 2018 START
                        EmployeeModelObj.First = EmployeeModelObj.First == null ? false : EmployeeModelObj.First.Value;
                        EmployeeModelObj.Second = EmployeeModelObj.Second == null ? false : EmployeeModelObj.Second.Value;
                        EmployeeModelObj.Third = EmployeeModelObj.Third == null ? false : EmployeeModelObj.Third.Value;
                        EmployeeModelObj.Fourth = EmployeeModelObj.Fourth == null ? false : EmployeeModelObj.Fourth.Value;
                        EmployeeModelObj.Fifth = EmployeeModelObj.Fifth == null ? false : EmployeeModelObj.Fifth.Value;
                        EmployeeModelObj.IsSecondWeekoffHalfDay = EmployeeModelObj.IsSecondWeekoffHalfDay == null ? false : EmployeeModelObj.IsSecondWeekoffHalfDay.Value;// CODE ADDED BY SHRADDHA ON 12 MAR 2018
                        // ADDED BY MSJ ON 13 JAN 2018 END

                        #region EMPLOYEEMODEL.TBLFAMILYDETAILSLIST
                        //CODE ADDED BY SHRADDHA ON 10 FEB 2018 START
                        //GlobalSettingsConstant FamilyDetailsMaxRecords 
                        GlobalSetting FamilyDetailsMaxRecordsObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.FamilyDetailsMaxRecords).FirstOrDefault();
                        int FamilyDetailsMaxRecordsCount = 0;
                        if (FamilyDetailsMaxRecordsObj != null)
                        {
                            FamilyDetailsMaxRecordsCount = Convert.ToInt32(FamilyDetailsMaxRecordsObj.SettingValue);
                        }
                        EmployeeModelObj.FamilyDetailsMaxRecordsCount = FamilyDetailsMaxRecordsCount;
                        if ((EmployeeModelObj.TblFamilyDetailList == null ? 0 : EmployeeModelObj.TblFamilyDetailList.Count) == 0)
                        {
                            EmployeeModelObj.TblFamilyDetailList = new List<FamilyDetails>();
                            for (int i = 0; i < FamilyDetailsMaxRecordsCount; i++)
                            {
                                EmployeeModelObj.TblFamilyDetailList.Add(new FamilyDetails());
                            }

                        }
                        //CODE ADDED BY SHRADDHA ON 10 FEB 2018 END
                        #endregion

                        return View(EmployeeModelObj);
                    }
                    else
                    {
                        if (UploadFileEmployee != null && UploadFileEmployee.ContentLength > 0)
                        {
                            try
                            {
                                if (UploadFileEmployee.ContentLength > 1048576)
                                {
                                    ViewBag.Message = "Maximum allowed photo size is 1 MB.";
                                }
                                else
                                {
                                    //Added By Shraddha on 09 Nov 2016
                                    string Attachment = UploadFileEmployee.FileName;   //UPDATED CODE BY SHRADDHA ON 30 OCT 2017 TO GET CORRECT FILENAME
                                    string path = Path.Combine(Server.MapPath("~/User_Data/Upload_image"), Attachment);

                                    string fileExtension = Path.GetExtension(UploadFileEmployee.FileName);

                                    if (fileExtension == ".jpg" || fileExtension == ".png" || fileExtension == ".jpeg")
                                    {
                                        UploadFileEmployee.SaveAs(path);

                                        byte[] filename = System.IO.File.ReadAllBytes(path);

                                        ViewBag.Message = "File Uploaded Successfully";

                                        EmployeeModelObj.Picture = filename;  // Updated by Rajas on 23 FEB 2017 replaced EmployeeObj with EmployeeModelObj

                                        //WetosDB.SaveChanges();
                                    }
                                    else
                                    {
                                        ModelState.AddModelError("Error", "Please upload image in .jpg, .jpeg, .png format.");

                                        PopulateDropDown();

                                        // ADDED BY MSJ ON 13 JAN 2018 START
                                        EmployeeModelObj.First = EmployeeModelObj.First == null ? false : EmployeeModelObj.First.Value;
                                        EmployeeModelObj.Second = EmployeeModelObj.Second == null ? false : EmployeeModelObj.Second.Value;
                                        EmployeeModelObj.Third = EmployeeModelObj.Third == null ? false : EmployeeModelObj.Third.Value;
                                        EmployeeModelObj.Fourth = EmployeeModelObj.Fourth == null ? false : EmployeeModelObj.Fourth.Value;
                                        EmployeeModelObj.Fifth = EmployeeModelObj.Fifth == null ? false : EmployeeModelObj.Fifth.Value;
                                        EmployeeModelObj.IsSecondWeekoffHalfDay = EmployeeModelObj.IsSecondWeekoffHalfDay == null ? false : EmployeeModelObj.IsSecondWeekoffHalfDay.Value;// CODE ADDED BY SHRADDHA ON 12 MAR 2018
                                        // ADDED BY MSJ ON 13 JAN 2018 END

                                        #region EMPLOYEEMODEL.TBLFAMILYDETAILSLIST
                                        //CODE ADDED BY SHRADDHA ON 10 FEB 2018 START
                                        //GlobalSettingsConstant FamilyDetailsMaxRecords 
                                        GlobalSetting FamilyDetailsMaxRecordsObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.FamilyDetailsMaxRecords).FirstOrDefault();
                                        int FamilyDetailsMaxRecordsCount = 0;
                                        if (FamilyDetailsMaxRecordsObj != null)
                                        {
                                            FamilyDetailsMaxRecordsCount = Convert.ToInt32(FamilyDetailsMaxRecordsObj.SettingValue);
                                        }
                                        EmployeeModelObj.FamilyDetailsMaxRecordsCount = FamilyDetailsMaxRecordsCount;
                                        if ((EmployeeModelObj.TblFamilyDetailList == null ? 0 : EmployeeModelObj.TblFamilyDetailList.Count) == 0)
                                        {
                                            EmployeeModelObj.TblFamilyDetailList = new List<FamilyDetails>();
                                            for (int i = 0; i < FamilyDetailsMaxRecordsCount; i++)
                                            {
                                                EmployeeModelObj.TblFamilyDetailList.Add(new FamilyDetails());
                                            }

                                        }
                                        //CODE ADDED BY SHRADDHA ON 10 FEB 2018 END
                                        #endregion

                                        return View(EmployeeModelObj);
                                    }

                                }
                            }
                            catch (System.Exception ex)
                            {
                                ViewBag.Message = "Error:" + ex.Message.ToString();
                                AddAuditTrail("Error image upload:" + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );
                            }
                        }
                        else
                        {
                            ViewBag.Message = "You have not specified a file";
                        }
                    }

                    // Updated by Rajas on 15 MAY 2017
                    if (UpdateEmployeeData(EmployeeModelObj, IsEdit, ref UpdateStatus) == true)
                    {
                        AddAuditTrail(UpdateStatus);

                        Success(UpdateStatus);
                    }
                    else
                    {
                        AddAuditTrail(UpdateStatus);

                        Error(UpdateStatus);

                        PopulateDropDownEdit(EmployeeModelObj);

                        // ADDED BY MSJ ON 13 JAN 2018 START
                        EmployeeModelObj.First = EmployeeModelObj.First == null ? false : EmployeeModelObj.First.Value;
                        EmployeeModelObj.Second = EmployeeModelObj.Second == null ? false : EmployeeModelObj.Second.Value;
                        EmployeeModelObj.Third = EmployeeModelObj.Third == null ? false : EmployeeModelObj.Third.Value;
                        EmployeeModelObj.Fourth = EmployeeModelObj.Fourth == null ? false : EmployeeModelObj.Fourth.Value;
                        EmployeeModelObj.Fifth = EmployeeModelObj.Fifth == null ? false : EmployeeModelObj.Fifth.Value;
                        EmployeeModelObj.IsSecondWeekoffHalfDay = EmployeeModelObj.IsSecondWeekoffHalfDay == null ? false : EmployeeModelObj.IsSecondWeekoffHalfDay.Value;// CODE ADDED BY SHRADDHA ON 12 MAR 2018
                        // ADDED BY MSJ ON 13 JAN 2018 END

                        #region EMPLOYEEMODEL.TBLFAMILYDETAILSLIST
                        //CODE ADDED BY SHRADDHA ON 10 FEB 2018 START
                        //GlobalSettingsConstant FamilyDetailsMaxRecords 
                        GlobalSetting FamilyDetailsMaxRecordsObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.FamilyDetailsMaxRecords).FirstOrDefault();
                        int FamilyDetailsMaxRecordsCount = 0;
                        if (FamilyDetailsMaxRecordsObj != null)
                        {
                            FamilyDetailsMaxRecordsCount = Convert.ToInt32(FamilyDetailsMaxRecordsObj.SettingValue);
                        }
                        EmployeeModelObj.FamilyDetailsMaxRecordsCount = FamilyDetailsMaxRecordsCount;
                        if ((EmployeeModelObj.TblFamilyDetailList == null ? 0 : EmployeeModelObj.TblFamilyDetailList.Count) == 0)
                        {
                            EmployeeModelObj.TblFamilyDetailList = new List<FamilyDetails>();
                            for (int i = 0; i < FamilyDetailsMaxRecordsCount; i++)
                            {
                                EmployeeModelObj.TblFamilyDetailList.Add(new FamilyDetails());
                            }

                        }
                        //CODE ADDED BY SHRADDHA ON 10 FEB 2018 END
                        #endregion

                        return View(EmployeeModelObj);
                    }

                    // CODE IN BELOW REGION COMMNETED BY RAJAS ON 30 DEC 2016
                    #region EMPLOYEE CREATE POST REGION
                    //EmployeeObj.EmployeeId = EmployeeModelObj.EmployeeId;
                    //EmployeeObj.EmployeeCode = EmployeeModelObj.EmployeeCode;
                    //EmployeeObj.FirstName = EmployeeModelObj.FirstName;
                    //EmployeeObj.LastName = EmployeeModelObj.LastName;
                    //EmployeeObj.MiddleName = EmployeeModelObj.MiddleName;
                    //EmployeeObj.Address1 = EmployeeModelObj.Address1;
                    //EmployeeObj.Address2 = EmployeeModelObj.Address2;
                    //EmployeeObj.MailAddress = EmployeeModelObj.MailAddress;
                    //EmployeeObj.Title = EmployeeModelObj.Title;
                    //EmployeeObj.Gender = EmployeeModelObj.Gender;
                    //EmployeeObj.BloodGroupId = EmployeeModelObj.BloodGroupId;
                    //EmployeeObj.ReligionId = EmployeeModelObj.ReligionId;
                    //EmployeeObj.DesignationId = EmployeeModelObj.DesignationId;
                    //EmployeeObj.DepartmentId = EmployeeModelObj.DepartmentId;
                    //EmployeeObj.EmployeeReportingId = EmployeeModelObj.EmployeeReportingId;
                    //EmployeeObj.GradeId = EmployeeModelObj.GradeId;
                    //EmployeeObj.MarritalStatus = EmployeeModelObj.MarritalStatus;
                    //EmployeeObj.Email = EmployeeModelObj.Email;
                    //EmployeeObj.Telephone1 = EmployeeModelObj.Telephone1;
                    //EmployeeObj.Telephone2 = EmployeeModelObj.Telephone2;
                    //EmployeeObj.Moblie = EmployeeModelObj.Moblie;

                    //EmployeeObj.BirthDate = EmployeeObj.BirthDate != null ? (EmployeeModelObj.BirthDate.Value.Year <= 1753 ? null : EmployeeModelObj.BirthDate) : null;
                    //EmployeeObj.ConfirmDate = EmployeeObj.ConfirmDate != null ? (EmployeeModelObj.ConfirmDate.Value.Year <= 1753 ? null : EmployeeObj.ConfirmDate) : null;
                    //EmployeeObj.JoiningDate = EmployeeObj.JoiningDate != null ? (EmployeeModelObj.JoiningDate.Value.Year <= 1753 ? null : EmployeeObj.JoiningDate) : null;
                    //EmployeeObj.Leavingdate = EmployeeObj.Leavingdate != null ? (EmployeeModelObj.Leavingdate.Value.Year <= 1753 ? null : EmployeeObj.Leavingdate) : null;

                    //EmployeeObj.PFNO = EmployeeModelObj.PFNO;
                    //EmployeeObj.PANNO = EmployeeModelObj.PANNO;
                    //EmployeeObj.ESICNO = EmployeeModelObj.ESICNO;
                    //EmployeeObj.ESICDCD = EmployeeModelObj.ESICDCD;
                    //EmployeeObj.BankCode = EmployeeModelObj.BankCode;
                    //EmployeeObj.BankNo = EmployeeModelObj.BankNo;
                    //EmployeeObj.LICNo = EmployeeModelObj.LICNo;
                    //EmployeeObj.CompanyId = EmployeeModelObj.CompanyId;
                    //EmployeeObj.BranchId = EmployeeModelObj.BranchId;
                    //EmployeeObj.DivisionId = EmployeeModelObj.DivisionId;
                    //EmployeeObj.EmployeeReportingId2 = EmployeeModelObj.EmployeeReportingId2;
                    //EmployeeObj.Qualification = EmployeeModelObj.Qualification;
                    //EmployeeObj.Institute = EmployeeModelObj.Institute;
                    //EmployeeObj.PassingYear = EmployeeModelObj.PassingYear;
                    //EmployeeObj.Subject = EmployeeModelObj.Subject;
                    //EmployeeObj.Zone = EmployeeModelObj.Zone;
                    //EmployeeObj.PrevExperience = EmployeeModelObj.PrevExperience;
                    //EmployeeObj.BSVExperience = EmployeeModelObj.BSVExperience;
                    //EmployeeObj.State = EmployeeModelObj.State;
                    //EmployeeObj.ZipCode = EmployeeModelObj.ZipCode;
                    //EmployeeObj.TransferDate =EmployeeObj.TransferDate!=null?(  EmployeeModelObj.TransferDate.Value.Year <= 1753 ? null : EmployeeObj.TransferDate):null;//; //EmployeeModelObj.TransferDate;

                    // COMMNTED BY MSJ ON 11 NOV 2016 START
                    // EmployeeObj.EmployeeJobInfo = EmployeeModelObj.EmployeeJobInfo;
                    // EmployeeObj.Employeedetails = EmployeeModelObj.Employeedetails;
                    // COMMNTED BY MSJ ON 11 NOV 2016 END

                    //EmployeeObj.CardNumber = EmployeeModelObj.CardNumber;

                    //EmployeeObj.WeeklyOff1 = "Sunday";
                    //EmployeeObj.WeeklyOff2 = "Saturday";


                    //WetosDB.Employees.Add(EmployeeObj);
                    //WetosDB.SaveChanges();


                    //EmployeeMaster EmployeeMasterObj = new EmployeeMaster();
                    //{
                    //    // Updated by Rajas on 17 Nov 2016 for saving Same employee id in Employee master table
                    //    EmployeeMasterObj.EmployeeId = EmployeeObj.EmployeeId;

                    //    EmployeeMasterObj.EmployeeCode = EmployeeModelObj.EmployeeCode;
                    //    EmployeeMasterObj.Age = EmployeeModelObj.Age;
                    //    EmployeeMasterObj.ExtNo = EmployeeModelObj.ExtNo;
                    //}

                    //WetosDB.EmployeeMasters.Add(EmployeeMasterObj);
                    //WetosDB.SaveChanges();

                    //EmployeeDetail EmployeeDetailObj = new EmployeeDetail();
                    //{
                    //    //EmployeeDetailObj.EmployeeMasterID = EmployeeMasterObj.EmployeeMasterID;

                    //    // Updated by Rajas on 17 Nov 2016 for saving Same employee id in Employee details table
                    //    EmployeeDetailObj.EmployeeId = EmployeeObj.EmployeeId;
                    //    // EmployeeDetailObj.ID = EmployeeObj.EmployeeId;
                    //    EmployeeDetailObj.EmployeeCode = EmployeeModelObj.EmployeeCode;
                    //    //EmployeeDetailObj.CTCOld = EmployeeModelObj.CTCOld;
                    //    EmployeeDetailObj.GrossSalary = EmployeeModelObj.GrossSalary;
                    //    EmployeeDetailObj.Basic = EmployeeModelObj.Basic;
                    //    EmployeeDetailObj.HRA = EmployeeModelObj.HRA;
                    //    EmployeeDetailObj.Medical = EmployeeModelObj.Medical;
                    //    EmployeeDetailObj.LTA = EmployeeModelObj.LTA;
                    //    EmployeeDetailObj.ConveyanceAllowance = EmployeeModelObj.ConveyanceAllowance;
                    //    EmployeeDetailObj.SupplementaryAllowance = EmployeeModelObj.SupplementaryAllowance;
                    //    EmployeeDetailObj.EmployeePF = EmployeeModelObj.EmployeePF;
                    //    EmployeeDetailObj.LaptopNo = EmployeeModelObj.LaptopNo;
                    //    EmployeeDetailObj.MonthlyIncentive = EmployeeModelObj.MonthlyIncentive;
                    //    EmployeeDetailObj.MonthlyCosttoCompany = EmployeeModelObj.MonthlyCosttoCompany;
                    //    EmployeeDetailObj.SpeicalIncentivePA = EmployeeModelObj.SpeicalIncentivePA;
                    //    EmployeeDetailObj.LeaveEncashmentPA = EmployeeModelObj.LeaveEncashmentPA;
                    //    EmployeeDetailObj.GratuityPA = EmployeeModelObj.GratuityPA;
                    //    EmployeeDetailObj.LoyaltyBonusPA = EmployeeModelObj.LoyaltyBonusPA;
                    //    EmployeeDetailObj.MedicalInsurance = EmployeeModelObj.MedicalInsurance;
                    //    EmployeeDetailObj.CTCPAFY = EmployeeModelObj.CTCPAFY;
                    //}

                    //WetosDB.EmployeeDetails.Add(EmployeeDetailObj);
                    //WetosDB.SaveChanges();

                    #endregion

                    //return RedirectToAction("Create");

                    // Above line commented and below new line added by Rajas on 16 JAN 2017 for redirection to index page
                    Employee EmployeeForLetter = WetosDB.Employees.Where(a => a.EmployeeCode == EmployeeModelObj.EmployeeCode).FirstOrDefault();

                    // Position changed 

                    //CODE MODIFIED BY SHRADDHA ON 01 NOV 2017 TO COMPARE GLOBAL SETTING BY STRING INSTEAD OF ID BECAUSE IT IS NOT NECESSORY TO HAVE SAME ID FOR ALL INSTALLATIONS AT ALL LOCATIONS AND WE ARE GOING TO MAKE GENERIC CODE FOR ALL LOCATION START
                    // Updated by Rajas on 31 JULY 2017 to resolve green and red message bar after addtion of employee
                    //GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingId == 11).FirstOrDefault();
                    GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == "HRLetterDefaultDate").FirstOrDefault();
                    //CODE MODIFIED BY SHRADDHA ON 01 NOV 2017 TO COMPARE GLOBAL SETTING BY STRING INSTEAD OF ID BECAUSE IT IS NOT NECESSORY TO HAVE SAME ID FOR ALL INSTALLATIONS AT ALL LOCATIONS AND WE ARE GOING TO MAKE GENERIC CODE FOR ALL LOCATION END


                    if (EmployeeForLetter != null && GlobalSettingObj != null)
                    {
                        DateTime date = Convert.ToDateTime(GlobalSettingObj.SettingValue);

                        if (EmployeeModelObj.JoiningDate > date)
                        {
                            if (EmployeeModelObj.ConfirmDate != null && EmployeeModelObj.JoiningDate != null)
                            {
                                // Currently printing both confirm letter formats
                                return RedirectToAction("Create", new { submitted = 2, EmployeeId = EmployeeForLetter.EmployeeId });
                            }
                            else if (EmployeeModelObj.JoiningDate != null && EmployeeModelObj.ConfirmDate == null)
                            {
                                // Currently generating Appointment and Bond
                                return RedirectToAction("Create", new { submitted = 1, EmployeeId = EmployeeForLetter.EmployeeId });
                            }
                            else
                            {
                                return RedirectToAction("Index");
                            }

                        }
                        else
                        {

                            if (EmployeeModelObj.ConfirmDate != null && EmployeeModelObj.JoiningDate != null)
                            {
                                // Currently printing both confirm letter formats
                                return RedirectToAction("Create", new { submitted = 2, EmployeeId = EmployeeForLetter.EmployeeId });
                            }
                            else if (EmployeeModelObj.JoiningDate != null && EmployeeModelObj.ConfirmDate == null)
                            {
                                // Currently generating Appointment and Bond
                                return RedirectToAction("Create", new { submitted = 1, EmployeeId = EmployeeForLetter.EmployeeId });
                            }
                            else
                            {
                                return RedirectToAction("Index");
                            }
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }

                }
                else
                {
                    // Code added to check model state error list
                    var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();

                    PopulateDropDownEdit(EmployeeModelObj);

                    // Added by Rajas on 16 JAN 2017 START
                    //AddAuditTrail("Error - Employee can not be added");

                    //Error("Please enter all required fields from below tabs...");

                    // Added by Rajas on 17 APRIL 2017, function to get checkbox values
                    GetCheckBoxValues(EmployeeModelObj, collection);

                    // ADDED BY MSJ ON 13 JAN 2018 START
                    EmployeeModelObj.First = EmployeeModelObj.First == null ? false : EmployeeModelObj.First.Value;
                    EmployeeModelObj.Second = EmployeeModelObj.Second == null ? false : EmployeeModelObj.Second.Value;
                    EmployeeModelObj.Third = EmployeeModelObj.Third == null ? false : EmployeeModelObj.Third.Value;
                    EmployeeModelObj.Fourth = EmployeeModelObj.Fourth == null ? false : EmployeeModelObj.Fourth.Value;
                    EmployeeModelObj.Fifth = EmployeeModelObj.Fifth == null ? false : EmployeeModelObj.Fifth.Value;
                    EmployeeModelObj.IsSecondWeekoffHalfDay = EmployeeModelObj.IsSecondWeekoffHalfDay == null ? false : EmployeeModelObj.IsSecondWeekoffHalfDay.Value;// CODE ADDED BY SHRADDHA ON 12 MAR 2018
                    // ADDED BY MSJ ON 13 JAN 2018 END

                    #region EMPLOYEEMODEL.TBLFAMILYDETAILSLIST
                    //CODE ADDED BY SHRADDHA ON 10 FEB 2018 START
                    //GlobalSettingsConstant FamilyDetailsMaxRecords 
                    GlobalSetting FamilyDetailsMaxRecordsObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.FamilyDetailsMaxRecords).FirstOrDefault();
                    int FamilyDetailsMaxRecordsCount = 0;
                    if (FamilyDetailsMaxRecordsObj != null)
                    {
                        FamilyDetailsMaxRecordsCount = Convert.ToInt32(FamilyDetailsMaxRecordsObj.SettingValue);
                    }
                    EmployeeModelObj.FamilyDetailsMaxRecordsCount = FamilyDetailsMaxRecordsCount;
                    if ((EmployeeModelObj.TblFamilyDetailList == null ? 0 : EmployeeModelObj.TblFamilyDetailList.Count) == 0)
                    {
                        EmployeeModelObj.TblFamilyDetailList = new List<FamilyDetails>();
                        for (int i = 0; i < FamilyDetailsMaxRecordsCount; i++)
                        {
                            EmployeeModelObj.TblFamilyDetailList.Add(new FamilyDetails());
                        }

                    }
                    //CODE ADDED BY SHRADDHA ON 10 FEB 2018 END
                    #endregion

                    return View(EmployeeModelObj);
                    //return RedirectToAction("Create"); // Updated by Rajas on 23 FEB 2017
                }
            }
            catch (System.Exception Ex)
            {
                PopulateDropDownEdit(EmployeeModelObj);

                // Added by Rajas on 17 APRIL 2017, function to get checkbox values
                GetCheckBoxValues(EmployeeModelObj, collection);

                // Added by Rajas on 16 JAN 2017 START
                AddAuditTrail("Exception - Employee : " + EmployeeModelObj.EmployeeCode + " " + EmployeeModelObj.FirstName + " " + EmployeeModelObj.LastName + " create failed due to " + Ex.Message + " " + (Ex.InnerException == null ? string.Empty : Ex.InnerException.Message));

                Error("Error - Employee create failed due to required fields are not filled.");
                // Added by Rajas on 16 JAN 2017 END

                // ADDED BY MSJ ON 13 JAN 2018 START
                EmployeeModelObj.First = EmployeeModelObj.First == null ? false : EmployeeModelObj.First.Value;
                EmployeeModelObj.Second = EmployeeModelObj.Second == null ? false : EmployeeModelObj.Second.Value;
                EmployeeModelObj.Third = EmployeeModelObj.Third == null ? false : EmployeeModelObj.Third.Value;
                EmployeeModelObj.Fourth = EmployeeModelObj.Fourth == null ? false : EmployeeModelObj.Fourth.Value;
                EmployeeModelObj.Fifth = EmployeeModelObj.Fifth == null ? false : EmployeeModelObj.Fifth.Value;
                EmployeeModelObj.IsSecondWeekoffHalfDay = EmployeeModelObj.IsSecondWeekoffHalfDay == null ? false : EmployeeModelObj.IsSecondWeekoffHalfDay.Value;// CODE ADDED BY SHRADDHA ON 12 MAR 2018
                // ADDED BY MSJ ON 13 JAN 2018 END

                #region EMPLOYEEMODEL.TBLFAMILYDETAILSLIST
                //CODE ADDED BY SHRADDHA ON 10 FEB 2018 START
                //GlobalSettingsConstant FamilyDetailsMaxRecords 
                GlobalSetting FamilyDetailsMaxRecordsObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.FamilyDetailsMaxRecords).FirstOrDefault();
                int FamilyDetailsMaxRecordsCount = 0;
                if (FamilyDetailsMaxRecordsObj != null)
                {
                    FamilyDetailsMaxRecordsCount = Convert.ToInt32(FamilyDetailsMaxRecordsObj.SettingValue);
                }
                EmployeeModelObj.FamilyDetailsMaxRecordsCount = FamilyDetailsMaxRecordsCount;
                if ((EmployeeModelObj.TblFamilyDetailList == null ? 0 : EmployeeModelObj.TblFamilyDetailList.Count) == 0)
                {
                    EmployeeModelObj.TblFamilyDetailList = new List<FamilyDetails>();
                    for (int i = 0; i < FamilyDetailsMaxRecordsCount; i++)
                    {
                        EmployeeModelObj.TblFamilyDetailList.Add(new FamilyDetails());
                    }

                }
                //CODE ADDED BY SHRADDHA ON 10 FEB 2018 END
                #endregion

                return View(EmployeeModelObj); // Updated by Rajas on 23 FEB 2017
            }
        }


        //ADDED BY Snehal on 4may2020 FOR IMAGE SHOW     START
        public ActionResult ShowImageWorker(int id)
        {
            var imageData = WetosDB.Employees.Where(a => a.EmployeeId == id).Select(a => a.Picture).FirstOrDefault();

            if (imageData != null)
            {
                return File(imageData, "image/jpg");
            }
            else
            {
                return null;
            }

        }


        //
        // GET: /WetosEmployee/Edit/5

        /// <summary>
        /// EMPLOYEE EDIT GET
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public ActionResult Edit(int id)
        {
            try
            {
                // EmployeeEdit = new SP_EmployeeDetailsNew_Result();
                //EmployeeEdit = WetosDB.SP_EmployeeDetailsNew().Where(b => b.EmployeeId == id).FirstOrDefault();

                EmployeeModel EmployeeModelObj = new EmployeeModel();

                string Code = WetosDB.Employees.Where(a => a.EmployeeId == id).Select(a => a.EmployeeCode).FirstOrDefault();

                // Added by Rajas on 18 FEB 2017 for saving EmployeeGroupId to EmployeeGroupdetail
                EmployeeGroupDetail EmployeeGroupdetailObj = WetosDB.EmployeeGroupDetails.Where(a => a.Employee.EmployeeId == id).FirstOrDefault();

                if (EmployeeGroupdetailObj != null)
                {
                    EmployeeModelObj.EmployeeGroupId = EmployeeGroupdetailObj.EmployeeGroup.EmployeeGroupId;
                }

                Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeCode == Code).FirstOrDefault();

                EmployeeExtraDetail EmployeeExtraDetailsObj = WetosDB.EmployeeExtraDetails.Where(a => a.EmployeeCode == Code || a.EmployeeId == EmployeeObj.EmployeeId).FirstOrDefault();

                if (EmployeeExtraDetailsObj == null)
                {
                    EmployeeExtraDetailsObj = new EmployeeExtraDetail();
                }


                if (EmployeeObj != null)
                {
                    EmployeeModelObj.EmployeeId = EmployeeObj.EmployeeId;

                    EmployeeModelObj.EmployeeCode = EmployeeObj.EmployeeCode;

                    EmployeeModelObj.FirstName = EmployeeObj.FirstName;

                    EmployeeModelObj.LastName = EmployeeObj.LastName;

                    EmployeeModelObj.MiddleName = EmployeeObj.MiddleName;

                    EmployeeModelObj.Address1 = EmployeeObj.Address1;

                    EmployeeModelObj.Address2 = EmployeeObj.Address2;

                    EmployeeModelObj.MailAddress = EmployeeObj.MailAddress;

                    EmployeeModelObj.Prefix = EmployeeObj.Title;

                    EmployeeModelObj.Gender = EmployeeObj.Gender;

                    //EmployeeModelObj.BloodGroupId = Convert.ToInt32(EmployeeObj.BloodGroup.BloodGroupId); // Uncommented by Rajas on 17 FEB 2017



                    EmployeeModelObj.ReligionId = Convert.ToInt32(EmployeeObj.ReligionId);

                    EmployeeModelObj.DesignationId = EmployeeObj.DesignationId;

                    EmployeeModelObj.DepartmentId = EmployeeObj.DepartmentId;

                    EmployeeModelObj.EmployeeReportingId = EmployeeObj.EmployeeReportingId;

                    EmployeeModelObj.GradeId = EmployeeObj.GradeId;

                    EmployeeModelObj.MarritalStatus = EmployeeObj.MarritalStatus;

                    EmployeeModelObj.Email = EmployeeObj.Email;

                    EmployeeModelObj.Telephone1 = EmployeeObj.Telephone1;

                    EmployeeModelObj.Telephone2 = EmployeeObj.Telephone2;

                    EmployeeModelObj.Moblie = EmployeeObj.Moblie;

                    EmployeeModelObj.EmployeeTypeId = EmployeeObj.EmployeeTypeId; // Added by Rajas on 17 FEB 2017

                    EmployeeModelObj.NoticePeriod = EmployeeObj.NoticePeriod; //ADDED BY PRATHMESH ON 17 APRIL 2019 

                    EmployeeModelObj.ResignationDate = EmployeeObj.ResignationDate; //ADDED BY PRATHMESH ON 17 APRIL 2019 

                    //EmployeeModelObj.BirthDate = Convert.ToDateTime(EmployeeObj.BirthDate);
                    //EmployeeModelObj.ConfirmDate = Convert.ToDateTime(EmployeeObj.ConfirmDate);
                    ////EmployeeModelObj.ConfirmDate = DateTime.Now.AddMonths(6);
                    //EmployeeModelObj.JoiningDate = Convert.ToDateTime(EmployeeObj.JoiningDate);
                    //EmployeeModelObj.Leavingdate = Convert.ToDateTime(EmployeeObj.Leavingdate);

                    // UPDATED BY RAJAS ON 3 DEC 2017 FOR HANDLING NULL AND OUT OF RANGE DATE VALUE START
                    EmployeeModelObj.BirthDate = EmployeeObj.BirthDate != null ? (EmployeeObj.BirthDate.Value.Year <= 1753 ? null : EmployeeObj.BirthDate) : null;

                    EmployeeModelObj.ConfirmDate = EmployeeObj.ConfirmDate != null ? (EmployeeObj.ConfirmDate.Value.Year <= 1753 ? null : EmployeeObj.ConfirmDate) : null;

                    EmployeeModelObj.JoiningDate = EmployeeObj.JoiningDate != null ? (EmployeeObj.JoiningDate.Value.Year <= 1753 ? null : EmployeeObj.JoiningDate) : null;

                    EmployeeModelObj.Leavingdate = EmployeeObj.Leavingdate != null ? (EmployeeObj.Leavingdate.Value.Year <= 1753 ? null : EmployeeObj.Leavingdate) : null;
                    // END

                    EmployeeModelObj.PFNO = EmployeeObj.PFNO;

                    EmployeeModelObj.PANNO = EmployeeObj.PANNO;

                    EmployeeModelObj.ESICNO = EmployeeObj.ESICNO;

                    EmployeeModelObj.ESICDCD = EmployeeObj.ESICDCD;

                    EmployeeModelObj.BankCode = EmployeeObj.BankCode;

                    EmployeeModelObj.BankNo = EmployeeObj.BankNo;

                    EmployeeModelObj.LICNo = EmployeeObj.LICNo;

                    EmployeeModelObj.CompanyId = EmployeeObj.CompanyId;

                    EmployeeModelObj.BranchId = EmployeeObj.BranchId;

                    EmployeeModelObj.DivisionId = Convert.ToInt32(EmployeeObj.DivisionId);

                    EmployeeModelObj.EmployeeReportingId2 = Convert.ToInt32(EmployeeObj.EmployeeReportingId2);


                    //Code added by shraddha to add extra details on 24 MAY 2017
                    EmployeeModelObj.OtherPassingYear = EmployeeExtraDetailsObj.OtherPaasingYear;
                    EmployeeModelObj.GradPassingYear = EmployeeExtraDetailsObj.GraduationPassingYear;
                    EmployeeModelObj.PostGradPassingYear = EmployeeExtraDetailsObj.PostGraduationPassingYear;
                    EmployeeModelObj.PrevExperience = EmployeeExtraDetailsObj.ExperienceBeforeFlagship;
                    EmployeeModelObj.FlagshipExperience = EmployeeExtraDetailsObj.ExperienceWithFlagship;
                    //EmployeeModelObj.PFNO = EmployeeExtraDetailsObj.PFNo;
                    EmployeeModelObj.ESICNO = EmployeeExtraDetailsObj.ESICNo;

                    EmployeeModelObj.GraduationQualification = EmployeeExtraDetailsObj.GraduationQualification;
                    EmployeeModelObj.PostGraduationQualification = EmployeeExtraDetailsObj.PostGraduationQualification;
                    EmployeeModelObj.OtherGraduationQualification = EmployeeExtraDetailsObj.OtherGraduationQualification;

                    //ADDED BY SHRADDHA ON 21 JUNE 2017 TO ADD LEAVE EFFECTIVE FROM DATE IN TABLE
                    EmployeeModelObj.LeaveEffectiveFromDate = EmployeeExtraDetailsObj.LeaveEffectiveFromDate;

                    // COMMENTED BY RAJAS ON 14 FEB 2017 START as it does not contain defination in Employee table for eviska
                    //EmployeeModelObj.Qualification = EmployeeObj.Qualification;
                    //EmployeeModelObj.Institute = EmployeeObj.Institute;
                    //EmployeeModelObj.PassingYear = EmployeeObj.PassingYear;
                    //EmployeeModelObj.Subject = EmployeeObj.Subject;
                    //EmployeeModelObj.Zone = EmployeeObj.Zone;
                    //EmployeeModelObj.PrevExperience = EmployeeObj.PrevExperience;
                    //EmployeeModelObj.BSVExperience = EmployeeObj.BSVExperience;
                    //EmployeeModelObj.State = EmployeeObj.State;
                    //EmployeeModelObj.ZipCode = EmployeeObj.ZipCode;
                    //EmployeeModelObj.TransferDate = EmployeeObj.TransferDate;
                    // COMMENTED BY RAJAS ON 14 FEB 2017 END

                    EmployeeModelObj.Picture = EmployeeObj.Picture;

                    // Added by Rajas on 11 JAN 2017, as week off selection provided instead of hardcode value
                    EmployeeModelObj.WeeklyOff1 = EmployeeObj.WeeklyOff1;

                    EmployeeModelObj.WeeklyOff2 = EmployeeObj.WeeklyOff2;

                    // MODIFIED BY MSJ on 14 JAN 2016 ADDED NULL HANDLER START
                    EmployeeModelObj.First = EmployeeObj.First == null ? false : EmployeeObj.First.Value;

                    EmployeeModelObj.Second = EmployeeObj.Second == null ? false : EmployeeObj.Second.Value;

                    EmployeeModelObj.Third = EmployeeObj.Third == null ? false : EmployeeObj.Third.Value;

                    EmployeeModelObj.Fourth = EmployeeObj.Fourth == null ? false : EmployeeObj.Fourth.Value;

                    EmployeeModelObj.Fifth = EmployeeObj.Fifth == null ? false : EmployeeObj.Fifth.Value;


                    #region CODE ADDED BY SHRADDHA ON 12 MAR 2018 FOR IsSecondWeekoffHalfDay
                    if (!string.IsNullOrEmpty(EmployeeObj.WeeklyHalfDay))
                    {
                        EmployeeModelObj.IsSecondWeekoffHalfDay = true;
                    }
                    else
                    {
                        EmployeeModelObj.IsSecondWeekoffHalfDay = false;
                    }
                    #endregion

                    // Added by Rajas on 15 JAN 2017
                    //EmployeeModelObj.IsSecondWeekoffHalfDay = false;

                    // MODIFIED BY MSJ on 14 JAN 2016 ADDED NULL HANDLER END

                    EmployeeModelObj.DefaultShift = EmployeeObj.DefaultShift;


                    // COMMNTED BY MSJ ON 11 NOV 2016 START
                    //EmployeeModelObj.EmployeeJobInfo = EmployeeObj.EmployeeJobInfo;
                    //EmployeeModelObj.Employeedetails = EmployeeObj.Employeedetails;
                    // COMMNTED BY MSJ ON 11 NOV 2016 END

                    EmployeeModelObj.CardNumber = EmployeeObj.CardNumber;

                    EmployeeModelObj.SeperationRemark = EmployeeObj.SeperationRemark; //CODE ADDED BY SHRADDHA ON 06 MARCH 2018

                    // EmployeeModelObj.WeeklyOff1 = "Sunday";
                    // EmployeeModelObj.WeeklyOff2 = "Saturday";

                    PopulateDropDownEdit(EmployeeModelObj);
                }

                //REMOVED ENTIRE REGION CODE ADDED THAT REGION IN FUNCTION GETFAMILYDETAILS AND CALLED THAT REGION HERE TO REMOVE REDUNDETNT CODE START
                GETFAMILYDETAILS(ref EmployeeModelObj);
                //REMOVED ENTIRE REGION CODE ADDED THAT REGION IN FUNCTION GETFAMILYDETAILS AND CALLED THAT REGION HERE TO REMOVE REDUNDETNT CODE END


                return View(EmployeeModelObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Please try again!!");

                return RedirectToAction("Index");  // Updated by Rajas on 22 SEP 2017
            }

        }

        //
        // POST: /WetosEmployee/Edit/5

        /// <summary>
        /// EMPLOYEE EDIT POST WITH VALID DATA
        /// ADDED BY RAJAS ON 30 DEC 2016
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="UploadFileEmployee"></param>
        /// <param name="EmployeeModelObj"></param>
        /// <returns></returns>
        /// Updated by Rajas on 3 JUNE 2017
        [HttpPost]
        public ActionResult Edit(int Id, HttpPostedFileBase UploadFileEmployee, EmployeeModel EmployeeModelObj, FormCollection collection)  //SP_EmployeeDetailsNew_Result
        {
            // Employee EmployeeEdit = WetosDB.Employees.Where(a => a.EmployeeCode == EmployeeModelObj.EmployeeCode).FirstOrDefault();
            try
            {

                // CHECK EMPLOYEE ALREADY EXIST // ADDED BY MSJ ON 11 JAN 2018 START
                Employee ExistingEmployee = WetosDB.Employees.Where(a => (a.FirstName.ToUpper() == EmployeeModelObj.FirstName.ToUpper()
                    && a.MiddleName.ToUpper() == EmployeeModelObj.MiddleName.ToUpper()
                    && a.LastName.ToUpper() == EmployeeModelObj.LastName.ToUpper()
                    && a.Gender == EmployeeModelObj.Gender && a.EmployeeCode == EmployeeModelObj.EmployeeCode && a.EmployeeId != Id) //ADDED EMPLOYEE CODE AND EMPLOYEE EMAIL ID CONDITION BY SHRADDHA ON 16 JAN 2018
                   ).FirstOrDefault();

                if (ExistingEmployee != null) // CHECKING
                {
                    ModelState.AddModelError("", "Employee already present!!!");
                    PopulateDropDownEdit(EmployeeModelObj);

                    // ADDED BY MSJ ON 13 JAN 2018 START
                    EmployeeModelObj.First = EmployeeModelObj.First == null ? false : EmployeeModelObj.First.Value;
                    EmployeeModelObj.Second = EmployeeModelObj.Second == null ? false : EmployeeModelObj.Second.Value;
                    EmployeeModelObj.Third = EmployeeModelObj.Third == null ? false : EmployeeModelObj.Third.Value;
                    EmployeeModelObj.Fourth = EmployeeModelObj.Fourth == null ? false : EmployeeModelObj.Fourth.Value;
                    EmployeeModelObj.Fifth = EmployeeModelObj.Fifth == null ? false : EmployeeModelObj.Fifth.Value;
                    EmployeeModelObj.IsSecondWeekoffHalfDay = EmployeeModelObj.IsSecondWeekoffHalfDay == null ? false : EmployeeModelObj.IsSecondWeekoffHalfDay.Value;// CODE ADDED BY SHRADDHA ON 12 MAR 2018
                    // ADDED BY MSJ ON 13 JAN 2018 END

                    //REMOVED ENTIRE REGION CODE ADDED THAT REGION IN FUNCTION GETFAMILYDETAILS AND CALLED THAT REGION HERE TO REMOVE REDUNDETNT CODE START
                    GETFAMILYDETAILS(ref EmployeeModelObj);
                    //REMOVED ENTIRE REGION CODE ADDED THAT REGION IN FUNCTION GETFAMILYDETAILS AND CALLED THAT REGION HERE TO REMOVE REDUNDETNT CODE END


                    return View(EmployeeModelObj);
                }
                else
                {
                    if (EmployeeModelObj.Gender.ToUpper() == "F")
                    {
                        if (EmployeeModelObj.Prefix.ToUpper() == "MR" || EmployeeModelObj.Prefix.ToUpper() == "MR.")
                        {
                            ModelState.AddModelError("", "Inconsist Gender and Prefix.");

                            PopulateDropDownEdit(EmployeeModelObj);

                            // ADDED BY MSJ ON 13 JAN 2018 START
                            EmployeeModelObj.First = EmployeeModelObj.First == null ? false : EmployeeModelObj.First.Value;
                            EmployeeModelObj.Second = EmployeeModelObj.Second == null ? false : EmployeeModelObj.Second.Value;
                            EmployeeModelObj.Third = EmployeeModelObj.Third == null ? false : EmployeeModelObj.Third.Value;
                            EmployeeModelObj.Fourth = EmployeeModelObj.Fourth == null ? false : EmployeeModelObj.Fourth.Value;
                            EmployeeModelObj.Fifth = EmployeeModelObj.Fifth == null ? false : EmployeeModelObj.Fifth.Value;
                            EmployeeModelObj.IsSecondWeekoffHalfDay = EmployeeModelObj.IsSecondWeekoffHalfDay == null ? false : EmployeeModelObj.IsSecondWeekoffHalfDay.Value;// CODE ADDED BY SHRADDHA ON 12 MAR 2018
                            // ADDED BY MSJ ON 13 JAN 2018 END

                            //REMOVED ENTIRE REGION CODE ADDED THAT REGION IN FUNCTION GETFAMILYDETAILS AND CALLED THAT REGION HERE TO REMOVE REDUNDETNT CODE START
                            GETFAMILYDETAILS(ref EmployeeModelObj);
                            //REMOVED ENTIRE REGION CODE ADDED THAT REGION IN FUNCTION GETFAMILYDETAILS AND CALLED THAT REGION HERE TO REMOVE REDUNDETNT CODE END


                            return View(EmployeeModelObj);
                        }
                    }

                }

                //CODE ADDED BY SHRADDHA ON 06 MARCH 2018 START
                if (EmployeeModelObj.Leavingdate != null && EmployeeModelObj.SeperationRemark == null) // CHECKING
                {
                    ModelState.AddModelError("", "Seperation Remark is mandatory in case of seperation date is entered");
                    PopulateDropDownEdit(EmployeeModelObj);

                    // ADDED BY MSJ ON 13 JAN 2018 START
                    EmployeeModelObj.First = EmployeeModelObj.First == null ? false : EmployeeModelObj.First.Value;
                    EmployeeModelObj.Second = EmployeeModelObj.Second == null ? false : EmployeeModelObj.Second.Value;
                    EmployeeModelObj.Third = EmployeeModelObj.Third == null ? false : EmployeeModelObj.Third.Value;
                    EmployeeModelObj.Fourth = EmployeeModelObj.Fourth == null ? false : EmployeeModelObj.Fourth.Value;
                    EmployeeModelObj.Fifth = EmployeeModelObj.Fifth == null ? false : EmployeeModelObj.Fifth.Value;
                    EmployeeModelObj.IsSecondWeekoffHalfDay = EmployeeModelObj.IsSecondWeekoffHalfDay == null ? false : EmployeeModelObj.IsSecondWeekoffHalfDay.Value;// CODE ADDED BY SHRADDHA ON 12 MAR 2018
                    // ADDED BY MSJ ON 13 JAN 2018 END

                    //REMOVED ENTIRE REGION CODE ADDED THAT REGION IN FUNCTION GETFAMILYDETAILS AND CALLED THAT REGION HERE TO REMOVE REDUNDETNT CODE START
                    GETFAMILYDETAILS(ref EmployeeModelObj);
                    //REMOVED ENTIRE REGION CODE ADDED THAT REGION IN FUNCTION GETFAMILYDETAILS AND CALLED THAT REGION HERE TO REMOVE REDUNDETNT CODE END


                    return View(EmployeeModelObj);
                }
                //CODE ADDED BY SHRADDHA ON 06 MARCH 2018 END

                // CHECK EMPLOYEE ALREADY EXIST // ADDED BY MSJ ON 11 JAN 2018 END

                string UpdateStatus = string.Empty;

                // Added by Rajas on 15 MAY 2017
                bool IsEdit = true;

                #region DATE VALIDATIONS

                // Region updated by Rajas on 17 APRIL 2017 START

                // Check birth date validation 
                DateTime Today = DateTime.Now;

                DateTime DOB = Today.AddYears(-18);

                if (Today.Date >= DOB.Date && EmployeeModelObj.BirthDate >= DOB.Date)
                {
                    //ModelState.AddModelError("", "Age limit should be greater than 18 years");

                    PopulateDropDownEdit(EmployeeModelObj);

                    // Added by Rajas on 16 JAN 2017 START
                    AddAuditTrail("Error - Employee can not be added due to invalid birth date");

                    Error("Age limit should be greater than 18 years");

                    // ADDED BY MSJ ON 13 JAN 2018 START
                    EmployeeModelObj.First = EmployeeModelObj.First == null ? false : EmployeeModelObj.First.Value;
                    EmployeeModelObj.Second = EmployeeModelObj.Second == null ? false : EmployeeModelObj.Second.Value;
                    EmployeeModelObj.Third = EmployeeModelObj.Third == null ? false : EmployeeModelObj.Third.Value;
                    EmployeeModelObj.Fourth = EmployeeModelObj.Fourth == null ? false : EmployeeModelObj.Fourth.Value;
                    EmployeeModelObj.Fifth = EmployeeModelObj.Fifth == null ? false : EmployeeModelObj.Fifth.Value;
                    EmployeeModelObj.IsSecondWeekoffHalfDay = EmployeeModelObj.IsSecondWeekoffHalfDay == null ? false : EmployeeModelObj.IsSecondWeekoffHalfDay.Value;// CODE ADDED BY SHRADDHA ON 12 MAR 2018
                    // ADDED BY MSJ ON 13 JAN 2018 END

                    // Added by Rajas on 17 APRIL 2017, function to get checkbox values
                    GetCheckBoxValues(EmployeeModelObj, collection);

                    //REMOVED ENTIRE REGION CODE ADDED THAT REGION IN FUNCTION GETFAMILYDETAILS AND CALLED THAT REGION HERE TO REMOVE REDUNDETNT CODE START
                    GETFAMILYDETAILS(ref EmployeeModelObj);
                    //REMOVED ENTIRE REGION CODE ADDED THAT REGION IN FUNCTION GETFAMILYDETAILS AND CALLED THAT REGION HERE TO REMOVE REDUNDETNT CODE END


                    return View(EmployeeModelObj);
                }


                //CODE ADDED BY SHRADDHA O 24 FEB 2018 FOR FAMILY MEMBER BIRTH DATE SHOULD NOT BE GREATER THAN TODAY VALIDATION START
                foreach (FamilyDetails FamilyDetailsObjNew in EmployeeModelObj.TblFamilyDetailList)
                {
                    if (!string.IsNullOrEmpty(FamilyDetailsObjNew.DateOfBirth))
                    {
                        DateTime DateOfBirth = Convert.ToDateTime(FamilyDetailsObjNew.DateOfBirth);
                        if (DateOfBirth > DateTime.Now)
                        {
                            //ModelState.AddModelError("", "Age limit should be greater than 18 years");

                            PopulateDropDownEdit(EmployeeModelObj);

                            // ADDED BY MSJ ON 13 JAN 2018 START
                            EmployeeModelObj.First = EmployeeModelObj.First == null ? false : EmployeeModelObj.First.Value;
                            EmployeeModelObj.Second = EmployeeModelObj.Second == null ? false : EmployeeModelObj.Second.Value;
                            EmployeeModelObj.Third = EmployeeModelObj.Third == null ? false : EmployeeModelObj.Third.Value;
                            EmployeeModelObj.Fourth = EmployeeModelObj.Fourth == null ? false : EmployeeModelObj.Fourth.Value;
                            EmployeeModelObj.Fifth = EmployeeModelObj.Fifth == null ? false : EmployeeModelObj.Fifth.Value;
                            EmployeeModelObj.IsSecondWeekoffHalfDay = EmployeeModelObj.IsSecondWeekoffHalfDay == null ? false : EmployeeModelObj.IsSecondWeekoffHalfDay.Value;// CODE ADDED BY SHRADDHA ON 12 MAR 2018
                            // ADDED BY MSJ ON 13 JAN 2018 END

                            // Added by Rajas on 16 JAN 2017 START
                            AddAuditTrail("Error - Birth date of any Family Member should not be greater than Today's date");

                            Error("Error - Birth date of any Family Member should not be greater than Today's date");

                            // Added by Rajas on 17 APRIL 2017, function to get checkbox values
                            GetCheckBoxValues(EmployeeModelObj, collection);

                            //REMOVED ENTIRE REGION CODE ADDED THAT REGION IN FUNCTION GETFAMILYDETAILS AND CALLED THAT REGION HERE TO REMOVE REDUNDETNT CODE START
                            GETFAMILYDETAILS(ref EmployeeModelObj);
                            //REMOVED ENTIRE REGION CODE ADDED THAT REGION IN FUNCTION GETFAMILYDETAILS AND CALLED THAT REGION HERE TO REMOVE REDUNDETNT CODE END


                            return View(EmployeeModelObj);
                        }
                    }

                }
                //CODE ADDED BY SHRADDHA O 24 FEB 2018 FOR FAMILY MEMBER BIRTH DATE SHOULD NOT BE GREATER THAN TODAY VALIDATION END

                if (EmployeeModelObj.JoiningDate >= EmployeeModelObj.ConfirmDate)
                {
                    PopulateDropDownEdit(EmployeeModelObj);

                    // ADDED BY MSJ ON 13 JAN 2018 START
                    EmployeeModelObj.First = EmployeeModelObj.First == null ? false : EmployeeModelObj.First.Value;
                    EmployeeModelObj.Second = EmployeeModelObj.Second == null ? false : EmployeeModelObj.Second.Value;
                    EmployeeModelObj.Third = EmployeeModelObj.Third == null ? false : EmployeeModelObj.Third.Value;
                    EmployeeModelObj.Fourth = EmployeeModelObj.Fourth == null ? false : EmployeeModelObj.Fourth.Value;
                    EmployeeModelObj.Fifth = EmployeeModelObj.Fifth == null ? false : EmployeeModelObj.Fifth.Value;
                    EmployeeModelObj.IsSecondWeekoffHalfDay = EmployeeModelObj.IsSecondWeekoffHalfDay == null ? false : EmployeeModelObj.IsSecondWeekoffHalfDay.Value;// CODE ADDED BY SHRADDHA ON 12 MAR 2018
                    // ADDED BY MSJ ON 13 JAN 2018 END

                    // Added by Rajas on 16 JAN 2017 START
                    AddAuditTrail("Error - Employee can not be added due to invalid birth date");

                    Error("Invalid Date of Confirmation, please verify Date of Confirmation");

                    // Added by Rajas on 17 APRIL 2017, function to get checkbox values
                    GetCheckBoxValues(EmployeeModelObj, collection);

                    //REMOVED ENTIRE REGION CODE ADDED THAT REGION IN FUNCTION GETFAMILYDETAILS AND CALLED THAT REGION HERE TO REMOVE REDUNDETNT CODE START
                    GETFAMILYDETAILS(ref EmployeeModelObj);
                    //REMOVED ENTIRE REGION CODE ADDED THAT REGION IN FUNCTION GETFAMILYDETAILS AND CALLED THAT REGION HERE TO REMOVE REDUNDETNT CODE END


                    return View(EmployeeModelObj);

                }
                // Region updated by Rajas on 17 APRIL 2017 End
                #endregion

                if (ModelState.IsValid)
                {
                    // Added by Rajas on 17 APRIL 2017, function to get checkbox values
                    GetCheckBoxValues(EmployeeModelObj, collection);

                    if (UploadFileEmployee != null && UploadFileEmployee.ContentLength > 1048576)
                    {
                        ModelState.AddModelError("Error", "Photo should not be greater than 1 MB.");
                        PopulateDropDownEdit(EmployeeModelObj);

                        // ADDED BY MSJ ON 13 JAN 2018 START
                        EmployeeModelObj.First = EmployeeModelObj.First == null ? false : EmployeeModelObj.First.Value;
                        EmployeeModelObj.Second = EmployeeModelObj.Second == null ? false : EmployeeModelObj.Second.Value;
                        EmployeeModelObj.Third = EmployeeModelObj.Third == null ? false : EmployeeModelObj.Third.Value;
                        EmployeeModelObj.Fourth = EmployeeModelObj.Fourth == null ? false : EmployeeModelObj.Fourth.Value;
                        EmployeeModelObj.Fifth = EmployeeModelObj.Fifth == null ? false : EmployeeModelObj.Fifth.Value;
                        EmployeeModelObj.IsSecondWeekoffHalfDay = EmployeeModelObj.IsSecondWeekoffHalfDay == null ? false : EmployeeModelObj.IsSecondWeekoffHalfDay.Value;// CODE ADDED BY SHRADDHA ON 12 MAR 2018
                        // ADDED BY MSJ ON 13 JAN 2018 END

                        //REMOVED ENTIRE REGION CODE ADDED THAT REGION IN FUNCTION GETFAMILYDETAILS AND CALLED THAT REGION HERE TO REMOVE REDUNDETNT CODE START
                        GETFAMILYDETAILS(ref EmployeeModelObj);
                        //REMOVED ENTIRE REGION CODE ADDED THAT REGION IN FUNCTION GETFAMILYDETAILS AND CALLED THAT REGION HERE TO REMOVE REDUNDETNT CODE END


                        return View(EmployeeModelObj);
                    }
                    else
                    {
                        if (UploadFileEmployee != null && UploadFileEmployee.ContentLength > 0 && UploadFileEmployee.ContentLength <= 1048576)
                        {
                            try
                            {

                                //Added By Shraddha on 09 Nov 2016
                                string Attachment = UploadFileEmployee.FileName;   //UPDATED CODE BY SHRADDHA ON 30 OCT 2017 TO GET CORRECT FILENAME
                                string path = Path.Combine(Server.MapPath("~/User_Data/Upload_image"), Attachment);
                                //string path = Path.Combine(Server.MapPath("~/User_Data/UploadImage"), Attachment);

                                //AddAuditTrail("1 " + path);

                                string fileExtension = Path.GetExtension(UploadFileEmployee.FileName);
                                //AddAuditTrail("2 " + fileExtension);
                                if (fileExtension == ".jpg" || fileExtension == ".png" || fileExtension == ".jpeg")
                                {
                                    UploadFileEmployee.SaveAs(path);

                                    byte[] filename = System.IO.File.ReadAllBytes(path);

                                    //AddAuditTrail("3 " + filename);

                                    ViewBag.Message = "File Uploaded Successfully";



                                    EmployeeModelObj.Picture = filename;

                                    WetosDB.SaveChanges();
                                }
                                else
                                {
                                    ModelState.AddModelError("Error", "Please upload image in .jpg, .jpeg, .png format.");
                                    PopulateDropDownEdit(EmployeeModelObj);

                                    //REMOVED ENTIRE REGION CODE ADDED THAT REGION IN FUNCTION GETFAMILYDETAILS AND CALLED THAT REGION HERE TO REMOVE REDUNDETNT CODE START
                                    GETFAMILYDETAILS(ref EmployeeModelObj);
                                    //REMOVED ENTIRE REGION CODE ADDED THAT REGION IN FUNCTION GETFAMILYDETAILS AND CALLED THAT REGION HERE TO REMOVE REDUNDETNT CODE END

                                    // ADDED BY MSJ ON 13 JAN 2018 START
                                    EmployeeModelObj.First = EmployeeModelObj.First == null ? false : EmployeeModelObj.First.Value;
                                    EmployeeModelObj.Second = EmployeeModelObj.Second == null ? false : EmployeeModelObj.Second.Value;
                                    EmployeeModelObj.Third = EmployeeModelObj.Third == null ? false : EmployeeModelObj.Third.Value;
                                    EmployeeModelObj.Fourth = EmployeeModelObj.Fourth == null ? false : EmployeeModelObj.Fourth.Value;
                                    EmployeeModelObj.Fifth = EmployeeModelObj.Fifth == null ? false : EmployeeModelObj.Fifth.Value;
                                    EmployeeModelObj.IsSecondWeekoffHalfDay = EmployeeModelObj.IsSecondWeekoffHalfDay == null ? false : EmployeeModelObj.IsSecondWeekoffHalfDay.Value;// CODE ADDED BY SHRADDHA ON 12 MAR 2018
                                    // ADDED BY MSJ ON 13 JAN 2018 END

                                    return View(EmployeeModelObj);
                                }


                            }
                            catch (System.Exception ex)
                            {
                                ViewBag.Message = "Error:" + ex.Message.ToString();
                                AddAuditTrail(" " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                                // ADDED BY MSJ ON 13 JAN 2018 START
                                EmployeeModelObj.First = EmployeeModelObj.First == null ? false : EmployeeModelObj.First.Value;
                                EmployeeModelObj.Second = EmployeeModelObj.Second == null ? false : EmployeeModelObj.Second.Value;
                                EmployeeModelObj.Third = EmployeeModelObj.Third == null ? false : EmployeeModelObj.Third.Value;
                                EmployeeModelObj.Fourth = EmployeeModelObj.Fourth == null ? false : EmployeeModelObj.Fourth.Value;
                                EmployeeModelObj.Fifth = EmployeeModelObj.Fifth == null ? false : EmployeeModelObj.Fifth.Value;
                                EmployeeModelObj.IsSecondWeekoffHalfDay = EmployeeModelObj.IsSecondWeekoffHalfDay == null ? false : EmployeeModelObj.IsSecondWeekoffHalfDay.Value;// CODE ADDED BY SHRADDHA ON 12 MAR 2018
                                // ADDED BY MSJ ON 13 JAN 2018 END

                                PopulateDropDownEdit(EmployeeModelObj);

                                //REMOVED ENTIRE REGION CODE ADDED THAT REGION IN FUNCTION GETFAMILYDETAILS AND CALLED THAT REGION HERE TO REMOVE REDUNDETNT CODE START
                                GETFAMILYDETAILS(ref EmployeeModelObj);
                                //REMOVED ENTIRE REGION CODE ADDED THAT REGION IN FUNCTION GETFAMILYDETAILS AND CALLED THAT REGION HERE TO REMOVE REDUNDETNT CODE END

                            }

                        }
                        else
                        {
                            ViewBag.Message = "You have not specified a file";

                        }
                    }

                    // Updated by Rajas on 15 MAY 2017
                    if (UpdateEmployeeData(EmployeeModelObj, IsEdit, ref UpdateStatus) == true)
                    {
                        AddAuditTrail(UpdateStatus);

                        Success(UpdateStatus);
                    }
                    else
                    {
                        AddAuditTrail(UpdateStatus);

                        Error(UpdateStatus);

                        PopulateDropDownEdit(EmployeeModelObj);

                        //REMOVED ENTIRE REGION CODE ADDED THAT REGION IN FUNCTION GETFAMILYDETAILS AND CALLED THAT REGION HERE TO REMOVE REDUNDETNT CODE START
                        GETFAMILYDETAILS(ref EmployeeModelObj);
                        //REMOVED ENTIRE REGION CODE ADDED THAT REGION IN FUNCTION GETFAMILYDETAILS AND CALLED THAT REGION HERE TO REMOVE REDUNDETNT CODE END

                        // ADDED BY MSJ ON 13 JAN 2018 START
                        EmployeeModelObj.First = EmployeeModelObj.First == null ? false : EmployeeModelObj.First.Value;
                        EmployeeModelObj.Second = EmployeeModelObj.Second == null ? false : EmployeeModelObj.Second.Value;
                        EmployeeModelObj.Third = EmployeeModelObj.Third == null ? false : EmployeeModelObj.Third.Value;
                        EmployeeModelObj.Fourth = EmployeeModelObj.Fourth == null ? false : EmployeeModelObj.Fourth.Value;
                        EmployeeModelObj.Fifth = EmployeeModelObj.Fifth == null ? false : EmployeeModelObj.Fifth.Value;
                        EmployeeModelObj.IsSecondWeekoffHalfDay = EmployeeModelObj.IsSecondWeekoffHalfDay == null ? false : EmployeeModelObj.IsSecondWeekoffHalfDay.Value;// CODE ADDED BY SHRADDHA ON 12 MAR 2018
                        // ADDED BY MSJ ON 13 JAN 2018 END


                        return View(EmployeeModelObj);
                    }

                    //EMPLOYEE
                    // Employee EmployeeEdit = WetosDB.Employees.Where(a => a.EmployeeCode == EmployeeModelObj.EmployeeCode).FirstOrDefault();


                    //ViewBag.Error("Employee Edit Unsuccessful");

                    return RedirectToAction("Index");

                }

                else
                {
                    PopulateDropDownEdit(EmployeeModelObj);

                    // Added by Rajas on 17 APRIL 2017, function to get checkbox values
                    GetCheckBoxValues(EmployeeModelObj, collection);

                    //AddAuditTrail("Error - Employee can not be added");

                    //REMOVED ENTIRE REGION CODE ADDED THAT REGION IN FUNCTION GETFAMILYDETAILS AND CALLED THAT REGION HERE TO REMOVE REDUNDETNT CODE START
                    GETFAMILYDETAILS(ref EmployeeModelObj);
                    //REMOVED ENTIRE REGION CODE ADDED THAT REGION IN FUNCTION GETFAMILYDETAILS AND CALLED THAT REGION HERE TO REMOVE REDUNDETNT CODE END

                    // ADDED BY MSJ ON 13 JAN 2018 START
                    EmployeeModelObj.First = EmployeeModelObj.First == null ? false : EmployeeModelObj.First.Value;
                    EmployeeModelObj.Second = EmployeeModelObj.Second == null ? false : EmployeeModelObj.Second.Value;
                    EmployeeModelObj.Third = EmployeeModelObj.Third == null ? false : EmployeeModelObj.Third.Value;
                    EmployeeModelObj.Fourth = EmployeeModelObj.Fourth == null ? false : EmployeeModelObj.Fourth.Value;
                    EmployeeModelObj.Fifth = EmployeeModelObj.Fifth == null ? false : EmployeeModelObj.Fifth.Value;
                    EmployeeModelObj.IsSecondWeekoffHalfDay = EmployeeModelObj.IsSecondWeekoffHalfDay == null ? false : EmployeeModelObj.IsSecondWeekoffHalfDay.Value;// CODE ADDED BY SHRADDHA ON 12 MAR 2018
                    // ADDED BY MSJ ON 13 JAN 2018 END

                    return View(EmployeeModelObj);
                }
            }
            catch (System.Exception Ex)
            {
                PopulateDropDownEdit(EmployeeModelObj);

                // Added by Rajas on 17 APRIL 2017, function to get checkbox values
                GetCheckBoxValues(EmployeeModelObj, collection);

                // Added by Rajas on 16 JAN 2017 START
                AddAuditTrail("Exception - Employee : " + EmployeeModelObj.EmployeeCode + " " + EmployeeModelObj.FirstName + " " + EmployeeModelObj.LastName + " update failed due to " + Ex.Message);

                Error("Error - Employee : " + EmployeeModelObj.EmployeeCode + " " + EmployeeModelObj.FirstName + " " + EmployeeModelObj.LastName + " update failed due to " + Ex.Message);
                // Added by Rajas on 16 JAN 2017 END

                //REMOVED ENTIRE REGION CODE ADDED THAT REGION IN FUNCTION GETFAMILYDETAILS AND CALLED THAT REGION HERE TO REMOVE REDUNDETNT CODE START
                GETFAMILYDETAILS(ref EmployeeModelObj);
                //REMOVED ENTIRE REGION CODE ADDED THAT REGION IN FUNCTION GETFAMILYDETAILS AND CALLED THAT REGION HERE TO REMOVE REDUNDETNT CODE END

                // ADDED BY MSJ ON 13 JAN 2018 START
                EmployeeModelObj.First = EmployeeModelObj.First == null ? false : EmployeeModelObj.First.Value;
                EmployeeModelObj.Second = EmployeeModelObj.Second == null ? false : EmployeeModelObj.Second.Value;
                EmployeeModelObj.Third = EmployeeModelObj.Third == null ? false : EmployeeModelObj.Third.Value;
                EmployeeModelObj.Fourth = EmployeeModelObj.Fourth == null ? false : EmployeeModelObj.Fourth.Value;
                EmployeeModelObj.Fifth = EmployeeModelObj.Fifth == null ? false : EmployeeModelObj.Fifth.Value;
                EmployeeModelObj.IsSecondWeekoffHalfDay = EmployeeModelObj.IsSecondWeekoffHalfDay == null ? false : EmployeeModelObj.IsSecondWeekoffHalfDay.Value;// CODE ADDED BY SHRADDHA ON 12 MAR 2018
                // ADDED BY MSJ ON 13 JAN 2018 END

                return View(EmployeeModelObj);
            }
        }

        //
        // GET: /WetosEmployee/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /WetosEmployee/Delete/5

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
        /// Added by shraddha on 14th OCT 2016 start
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// Updated by Rajas on 27 MARCH 2017
        public ActionResult profile(int id)
        {
            try
            {
                //SP_EmployeeDetailsNew_Result Employee = WetosDB.SP_EmployeeDetailsNew().Where(a => a.EmployeeId == id).FirstOrDefault();
                SP_EmployeeProfile_Result Employee = WetosDB.SP_EmployeeProfile(id).FirstOrDefault();

                // Added by Rajas on 31 MAY 2017 START
                EmployeeGroupDetail EmpGroupDet = WetosDB.EmployeeGroupDetails.Where(a => a.Employee.EmployeeId == id).FirstOrDefault();

                if (EmpGroupDet != null)
                {
                    // Get employee group
                    EmployeeGroup EmpGrp = WetosDB.EmployeeGroups.Where(a => a.EmployeeGroupId == EmpGroupDet.EmployeeGroup.EmployeeGroupId).FirstOrDefault();

                    if (EmpGrp != null)
                    {
                        ViewBag.EmployeeGroup = EmpGrp.EmployeeGroupName;
                    }
                }
                // Added by Rajas on 31 MAY 2017 END

                // GET GlobalSetting Current FY
                // GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == "CurrentFinancialYear").FirstOrDefault();

                // CODE commented by Rajas on 1 JUNE 2017 START
                //if (GlobalSettingObj != null)
                //{
                //    FinancialYear GetFYObj = WetosDB.FinancialYears.Where(a => a.FinancialName == GlobalSettingObj.SettingValue).FirstOrDefault();

                //    List<LeaveCredit> LeaveCreditCheckList = WetosDB.LeaveCredits.Where(a => a.EmployeeId == id && a.FinancialYearId == GetFYObj.FinancialYearId).ToList();

                //    if (LeaveCreditCheckList.Count == 0)
                //    {
                //        LeaveAutoCredit(id);
                //    }
                //}
                // CODE commented by Rajas on 1 JUNE 2017 END

                if (Employee != null)
                {
                    //ADDED BY SHRADDHA ON 09 FEB 2017 FOR SHOWING DIFFERENT IMAGES FOR DIFFERENT GENDER START 
                    string Gender = Employee.Gender;
                    Session["Gender"] = Gender;
                    //ADDED BY SHRADDHA ON 09 FEB 2017 FOR SHOWING DIFFERENT IMAGES FOR DIFFERENT GENDER end 

                    #region COMMENTED CODE
                    //LeaveBalance LeaveBalanceObj = WetosDB.LeaveBalances.Where(a => a.EmployeeId == id).FirstOrDefault();
                    //LeaveCredit LeaveCreditObj = WetosDB.LeaveCredits.Where(a => a.EmployeeId == id).FirstOrDefault();

                    ////MODIFIED BY SHRADDHA ON 01 FEB 2017 AS PER SUGGESTED BY SIR START
                    //ViewBag.LeaveTYpe = "No Leave Data";
                    //ViewBag.CurrentBalance = "";
                    //ViewBag.LeaveUsed = "";
                    //ViewBag.Opening = "";

                    //// MODIFIED BY MSJ on 14 JAN 2017 Change 
                    //if (LeaveCreditObj != null)
                    //{
                    //    ViewBag.LeaveTYpe = LeaveCreditObj.LeaveType;
                    //    ViewBag.Opening = LeaveCreditObj.OpeningBalance;

                    //    if (LeaveBalanceObj != null)
                    //    {
                    //        ViewBag.CurrentBalance = LeaveBalanceObj.CurrentBalance;
                    //        ViewBag.LeaveUsed = LeaveBalanceObj.LeaveUsed;
                    //    }
                    //    else
                    //    {
                    //        ViewBag.CurrentBalance = LeaveCreditObj.OpeningBalance;
                    //        ViewBag.LeaveUsed = 0;
                    //    }

                    //}
                    //MODIFIED BY SHRADDHA ON 01 FEB 2017 AS PER SUGGESTED BY SIR END


                    //Added By Shraddha On 15TH Oct 2016 For Showing Leave Balance Details Table End
                    #endregion

                    // Updated by Rajas on 14 AUGUST 2017 START
                    //GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText.Trim() == "IsDetailedLeaveData").FirstOrDefault();

                    //ABOVE LINE COMMENTED AND BELOW LINE IS ADDED BY SHALAKA ON 13TH DEC 2017
                    GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText.Trim() == GlobalSettingsConstant.IsDetailedLeaveData).FirstOrDefault();

                    if (GlobalSettingObj == null)
                    {
                        ViewBag.LeaveBalanceDetailsVB = DashboardController.GetLeaveDetailsOnallControllers(WetosDB, id, DateTime.Now);
                        ViewBag.LeaveTableview = "0";
                    }
                    else
                    {
                        if (GlobalSettingObj.SettingValue == "1")
                        {
                            ViewBag.LeaveBalanceDetailsVB = DashboardController.GetLeaveDetailsOnallControllersNew(WetosDB, id, DateTime.Now);
                            ViewBag.LeaveTableview = "1";
                        }
                        else
                        {
                            ViewBag.LeaveBalanceDetailsVB = DashboardController.GetLeaveDetailsOnallControllers(WetosDB, id, DateTime.Now);
                            ViewBag.LeaveTableview = "0";
                        }
                    }
                    // Updated by Rajas on 14 AUGUST 2017 END

                    return View(Employee);
                }

                //return View();
                return RedirectToAction("Index", "Home");
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Please try again!");

                return RedirectToAction("Index");  // Verify ?
            }
        }

        /// <summary>
        /// CODE ADDED BY SHRADDHA ON 24 FEB 2018
        /// </summary>
        /// <param name="EmployeeModelObj"></param>
        public void GETFAMILYDETAILS(ref EmployeeModel EmployeeModelObj)
        {
            int EmployeeId = EmployeeModelObj.EmployeeId;
            #region GET FAMILY DETAILS DATA FROM TBLFAMILYDETAILS TABLE FOR SELECTED EMPLOYEE
            //CODE ADDED BY SHRADDHA ON 10 FEB 2018 START
            List<TblFamilyDetail> TblFamilyDetailList = WetosDB.TblFamilyDetails.Where(a => a.EmployeeId == EmployeeId).ToList();

            //GlobalSettingsConstant FamilyDetailsMaxRecords 
            GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.FamilyDetailsMaxRecords).FirstOrDefault();
            int FamilyDetailsMaxRecordsCount = 0;
            if (GlobalSettingObj != null)
            {
                FamilyDetailsMaxRecordsCount = Convert.ToInt32(GlobalSettingObj.SettingValue);
            }

            if (TblFamilyDetailList == null)
            {
                TblFamilyDetailList = new List<WetosDB.TblFamilyDetail>();
                for (int i = 0; i < FamilyDetailsMaxRecordsCount; i++)
                {
                    TblFamilyDetailList.Add(new WetosDB.TblFamilyDetail());
                }

            }
            #region CODE COMMENTED AND ADDED NEW CODE BY SHRADHA BECAUSE TAKEN FAMILYDETAILS CLASS INSTEAD OF TBLFAMILYDETAILS TABLE

            else if (TblFamilyDetailList.Count < FamilyDetailsMaxRecordsCount)
            {
                int j = FamilyDetailsMaxRecordsCount - TblFamilyDetailList.Count;
                for (int i = 0; i < j; i++)
                {
                    TblFamilyDetailList.Add(new WetosDB.TblFamilyDetail());
                }
            }
            List<FamilyDetails> FamilyDetailsList = new List<FamilyDetails>();
            foreach (TblFamilyDetail TblFamilyDetailObj in TblFamilyDetailList)
            {
                FamilyDetails FamilyDetailsObj = new FamilyDetails();
                if (TblFamilyDetailObj.DateOfBirth != null)
                {
                    FamilyDetailsObj.DateOfBirth = Convert.ToDateTime(TblFamilyDetailObj.DateOfBirth).ToString("dd-MMM-yyyy");
                }
                FamilyDetailsObj.EmployeeId = TblFamilyDetailObj.EmployeeId;
                FamilyDetailsObj.Gender = TblFamilyDetailObj.Gender;
                FamilyDetailsObj.Id = TblFamilyDetailObj.Id;
                FamilyDetailsObj.Name = TblFamilyDetailObj.Name;
                FamilyDetailsObj.Picture = TblFamilyDetailObj.Picture;
                FamilyDetailsObj.Relation = TblFamilyDetailObj.Relation;

                FamilyDetailsList.Add(FamilyDetailsObj);
            }
            if (FamilyDetailsList.Count == FamilyDetailsMaxRecordsCount)
            {
                EmployeeModelObj.TblFamilyDetailList = FamilyDetailsList;
            }
            else
            {
                EmployeeModelObj.TblFamilyDetailList = FamilyDetailsList;
                int j = FamilyDetailsMaxRecordsCount - TblFamilyDetailList.Count;
                for (int i = 0; i < j; i++)
                {
                    TblFamilyDetailList.Add(new WetosDB.TblFamilyDetail());
                }
            }
            #endregion
            EmployeeModelObj.TblFamilyDetailList = FamilyDetailsList;
            EmployeeModelObj.FamilyDetailsMaxRecordsCount = FamilyDetailsMaxRecordsCount;
            //CODE ADDED BY SHRADDHA ON 10 FEB 2018 END
            #endregion
        }

        /// <summary>
        /// Added by Rajas on 10 JAN 2016 for view only mode profile
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// Updated by Rajas on 27 MARCH 2017
        public ActionResult ViewProfile(int id)
        {
            try
            {
                //CODE COMMENTED BY SHRADDHA ON 01 FEB 2017 AND TAKEN NEW SP_EmployeeProfile_Result TO TAKE SALARY DATA FROM SALARYMASTER TABLE START
                //SP_EmployeeDetailsNew_Result Employee = WetosDB.SP_EmployeeDetailsNew().Where(a => a.EmployeeId == id).FirstOrDefault();
                SP_EmployeeProfile_Result Employee = WetosDB.SP_EmployeeProfile(id).FirstOrDefault();

                //CODE COMMENTED BY SHRADDHA ON 01 FEB 2017 AND TAKEN NEW SP_EmployeeProfile_Result TO TAKE SALARY DATA FROM SALARYMASTER TABLE END

                //ADDED BY SHRADDHA ON 09 FEB 2017 FOR SHOWING DIFFERENT IMAGES FOR DIFFERENT GENDER START 
                string Gender = Employee.Gender;
                Session["Gender"] = Gender;
                //ADDED BY SHRADDHA ON 09 FEB 2017 FOR SHOWING DIFFERENT IMAGES FOR DIFFERENT GENDER end 
                // Added by Rajas on 31 MAY 2017 START
                EmployeeGroupDetail EmpGroupDet = WetosDB.EmployeeGroupDetails.Where(a => a.Employee.EmployeeId == id).FirstOrDefault();

                if (EmpGroupDet != null)
                {
                    // Get employee group
                    EmployeeGroup EmpGrp = WetosDB.EmployeeGroups.Where(a => a.EmployeeGroupId == EmpGroupDet.EmployeeGroup.EmployeeGroupId).FirstOrDefault();

                    if (EmpGrp != null)
                    {
                        ViewBag.EmployeeGroup = EmpGrp.EmployeeGroupName;
                    }
                }
                // Added by Rajas on 31 MAY 2017 END

                // GET GlobalSetting Current FY
                //GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == "CurrentFinancialYear").FirstOrDefault();

                //if (GlobalSettingObj != null)
                //{
                //    FinancialYear GetFYObj = WetosDB.FinancialYears.Where(a => a.FinancialName == GlobalSettingObj.SettingValue).FirstOrDefault();

                //    List<LeaveCredit> LeaveCreditCheckList = WetosDB.LeaveCredits.Where(a => a.EmployeeId == id && a.FinancialYearId == GetFYObj.FinancialYearId).ToList();

                //    if (LeaveCreditCheckList.Count == 0)
                //    {
                //        LeaveAutoCredit(id);
                //    }
                //}

                #region COMMENTED CODE
                //Added By Shraddha On 15TH Oct 2016 For Showing Leave Balance Details Table Start
                //int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);

                var ActualDays = WetosDB.LeaveBalances.Where(a => a.EmployeeId == id).FirstOrDefault();
                var Opening = WetosDB.LeaveCredits.Where(a => a.EmployeeId == id).FirstOrDefault();

                ViewBag.LeaveTYpe = "";
                ViewBag.CurrentBalance = "";
                ViewBag.LeaveUsed = "";
                ViewBag.Opening = "";

                if (ActualDays != null && Opening != null)
                {
                    ViewBag.LeaveTYpe = ActualDays.LeaveType;
                    ViewBag.CurrentBalance = ActualDays.CurrentBalance;
                    ViewBag.LeaveUsed = ActualDays.LeaveUsed;
                    ViewBag.Opening = Opening.OpeningBalance;
                }
                //Added By Shraddha On 15TH Oct 2016 For Showing Leave Balance Details Table End
                #endregion

                // Updated by Rajas on 14 AUGUST 2017 START
                //GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText.Trim() == "IsDetailedLeaveData").FirstOrDefault();

                //ABOVE LINE COMMENTED AND BELOW LINE IS ADDED BY SHALAKA ON 13TH DEC 2017
                GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText.Trim() == GlobalSettingsConstant.IsDetailedLeaveData).FirstOrDefault();

                if (GlobalSettingObj == null)
                {
                    ViewBag.LeaveBalanceDetailsVB = DashboardController.GetLeaveDetailsOnallControllers(WetosDB, id, DateTime.Now);
                    ViewBag.LeaveTableview = "0";
                }
                else
                {
                    if (GlobalSettingObj.SettingValue == "1")
                    {
                        ViewBag.LeaveBalanceDetailsVB = DashboardController.GetLeaveDetailsOnallControllersNew(WetosDB, id, DateTime.Now);
                        ViewBag.LeaveTableview = "1";
                    }
                    else
                    {
                        ViewBag.LeaveBalanceDetailsVB = DashboardController.GetLeaveDetailsOnallControllers(WetosDB, id, DateTime.Now);
                        ViewBag.LeaveTableview = "0";
                    }
                }
                // Updated by Rajas on 14 AUGUST 2017 END


                return View(Employee);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Please try again!");

                return RedirectToAction("Index");  // Verify ?
            }

        }

        /// <summary>
        /// ADDED BY SHRADDHA ON 24 JULY 2017
        /// </summary>
        /// <param name="LeaveAsOnDate"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetLeaveDetailsAsOnDate(DateTime LeaveAsOnDate, int id)
        {
            SP_EmployeeProfile_Result Employee = WetosDB.SP_EmployeeProfile(id).FirstOrDefault();

            EmployeeGroupDetail EmpGroupDet = WetosDB.EmployeeGroupDetails.Where(a => a.Employee.EmployeeId == id).FirstOrDefault();

            if (EmpGroupDet != null)
            {
                EmployeeGroup EmpGrp = WetosDB.EmployeeGroups.Where(a => a.EmployeeGroupId == EmpGroupDet.EmployeeGroup.EmployeeGroupId).FirstOrDefault();

                if (EmpGrp != null)
                {
                    ViewBag.EmployeeGroup = EmpGrp.EmployeeGroupName;
                }
            }

            //GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == "Current Financial Year").FirstOrDefault();

            //ABOVE LINE COMMENTED AND BELOW LINE IS ADDED BY SHALAKA ON 13TH DEC 2017
            GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.CurrentFinancialYear).FirstOrDefault();

            if (Employee != null)
            {
                string Gender = Employee.Gender;
                Session["Gender"] = Gender;


                ViewBag.LeaveBalanceDetailsVB = DashboardController.GetLeaveDetailsOnallControllers(WetosDB, id, LeaveAsOnDate);
            }
            return Json(JsonRequestBehavior.AllowGet);
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

            // Updated by Rajas on 30 MAY 2017
            

            #region CODE FOR GET BRANCH LIST ADDED BY SHRADDHA ON 30 MAR 2018
            int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
            //var BranchList = WetosDB.Branches.Where(a => a.Company.CompanyId == SelCompanyId && (a.MarkedAsDelete == null || a.MarkedAsDelete == 0))
                //.Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList();
            var BranchList = WetosDB.SP_GetBranchList(EmployeeId).Where(a => a.CompanyId == SelCompanyId).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList(); //.Select(m => new { BranchId = m.BranchId, BranchName = m.BranchName}).ToList();
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
            #endregion

            return Json(BranchList);
        }



        /// <summary>
        /// Json return for to get Department dropdown list on basis of branch selection
        /// Added by Rajas on 27 DEC 2016
        /// </summary>
        /// <returns></returns>
        public JsonResult GetDepartment(string Branchid)
        {

            int SelBranchId = 0;
            if (!string.IsNullOrEmpty(Branchid))
            {
                if (Branchid.ToUpper() != "NULL")
                {
                    SelBranchId = Convert.ToInt32(Branchid);
                }
            }

            // Updated by Rajas on 30 MAY 2017
            var DepartmentList = WetosDB.Departments.Where(a => a.Branch.BranchId == SelBranchId && (a.MarkedAsDelete == null || a.MarkedAsDelete == 0))
                .Select(a => new { DepartmentId = a.DepartmentId, DepartmentName = a.DepartmentCode + " - " + a.DepartmentName }).ToList(); // DEPARTMENT CODE ADDED BY SHRADDHA ON 14 MAR 2018

            return Json(DepartmentList);
        }

        /// <summary>
        /// Json return for to get Shift dropdown list on basis of branch, company selection
        /// Added by Rajas on 2 JUNE 2017
        /// </summary>
        /// <returns></returns>
        public JsonResult GetShift(string Companyid, string Branchid)
        {

            int SelBranchId = 0;
            if (!string.IsNullOrEmpty(Branchid))
            {
                if (Branchid.ToUpper() != "NULL")
                {
                    SelBranchId = Convert.ToInt32(Branchid);
                }
            }

            int SelCompanyId = 0;
            if (!string.IsNullOrEmpty(Companyid))
            {
                if (Companyid.ToUpper() != "NULL")
                {
                    SelCompanyId = Convert.ToInt32(Companyid);
                }
            }

            var ShiftList = WetosDB.Shifts.Where(a => a.BranchId == SelBranchId && a.Company.CompanyId == SelCompanyId && (a.MarkedAsDelete == null || a.MarkedAsDelete == 0)).Select(a => new { ShiftId = a.ShiftCode, ShiftCode = a.ShiftCode }).ToList();

            return Json(ShiftList);
        }

        /// <summary>
        /// Json return for to get Department dropdown list on basis of branch, company selection
        /// Added by Rajas on 27 FEB 2017
        /// </summary>
        /// <returns></returns>
        public JsonResult GetEmployeeGroup(string Companyid, string Branchid)
        {

            int SelBranchId = 0;
            if (!string.IsNullOrEmpty(Branchid))
            {
                if (Branchid.ToUpper() != "NULL")
                {
                    SelBranchId = Convert.ToInt32(Branchid);
                }
            }

            int SelCompanyId = 0;
            if (!string.IsNullOrEmpty(Companyid))
            {
                if (Companyid.ToUpper() != "NULL")
                {
                    SelCompanyId = Convert.ToInt32(Companyid);
                }
            }

            // Updated by Rajas on 30 MAY 2017
            var EmployeeGroupList = WetosDB.EmployeeGroups.Where(a => a.Branch.BranchId == SelBranchId && a.Company.CompanyId == SelCompanyId
                && (a.MarkedAsDelete == null || a.MarkedAsDelete == 0)).Select(a => new { EmployeeGroupId = a.EmployeeGroupId, EmployeeGroupName = a.EmployeeGroupName }).ToList();

            return Json(EmployeeGroupList);
        }


        /// <summary>
        /// Json return for to get designation dropdown list on basis of Department selection
        /// Added by Rajas on 27 DEC 2016
        /// </summary>
        /// <returns></returns>
        public JsonResult GetDesignation(string Departmentid, string companyid, string branchid)
        {

            int SelDepartmentId = 0;
            if (!string.IsNullOrEmpty(Departmentid))
            {
                if (Departmentid.ToUpper() != "NULL")
                {
                    SelDepartmentId = Convert.ToInt32(Departmentid);
                }
            }

            int SelBranchId = 0;
            if (!string.IsNullOrEmpty(branchid))
            {
                if (branchid.ToUpper() != "NULL")
                {
                    SelBranchId = Convert.ToInt32(branchid);
                }
            }

            int SelCompanyId = 0;
            if (!string.IsNullOrEmpty(companyid))
            {
                if (companyid.ToUpper() != "NULL")
                {
                    SelCompanyId = Convert.ToInt32(companyid);
                }
            }

            // Updated by Rajas on 30 MAY 2017
            var DesignationList = WetosDB.Designations.Where(a => a.DepartmentId == SelDepartmentId && a.Company.CompanyId == SelCompanyId
                && a.Branch.BranchId == SelBranchId && (a.MarkedAsDelete == null || a.MarkedAsDelete == 0))
                .Select(a => new { DesignationId = a.DesignationId, DesignationName = a.DesignationName }).ToList();

            return Json(DesignationList);
        }


        /// <summary>
        /// To get check box values
        /// Added by Rajas on 17 APRIL 2017
        /// </summary>
        /// <param name="EmployeeModelObj"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public bool GetCheckBoxValues(EmployeeModel EmployeeModelObj, FormCollection collection)
        {
            bool ReturnStatus = false;

            try
            {
                // Added by Rajas on 13 JAN 2017 START

                // save first form collection value
                string first = collection["First.Value"];
                string FirstValue;
                if (first.Contains(","))
                {
                    string[] firstRecord = first.Split(',');
                    FirstValue = firstRecord[0].Trim();
                }
                else
                {
                    FirstValue = first;
                }

                // save second form collection value
                string second = collection["Second.Value"];
                string SecondValue;
                if (second.Contains(","))
                {
                    string[] secondRecord = second.Split(',');
                    SecondValue = secondRecord[0].Trim();
                }
                else
                {
                    SecondValue = second;
                }

                // save third form collection value
                string third = collection["Third.Value"];
                string ThirdValue;
                if (third.Contains(","))
                {
                    string[] thirdRecord = third.Split(',');
                    ThirdValue = thirdRecord[0].Trim();
                }
                else
                {
                    ThirdValue = third;
                }

                // save third form collection value
                string fourth = collection["Fourth.Value"];
                string FourthValue;
                if (fourth.Contains(","))
                {
                    string[] fourthRecord = fourth.Split(',');
                    FourthValue = fourthRecord[0].Trim();
                }
                else
                {
                    FourthValue = fourth;
                }

                // save fifth form collection value
                string fifth = collection["Fifth.Value"];
                string FifthValue;
                if (fifth.Contains(","))
                {
                    string[] fifthRecord = fifth.Split(',');
                    FifthValue = fifthRecord[0].Trim();
                }
                else
                {
                    FifthValue = fifth;
                }

                #region CODE ADDED BY SHRADDHA 12 MAR 2018
                // save first form collection value
                string IsSecondWeekoffHalfDay = collection["IsSecondWeekoffHalfDay.Value"];
                string IsSecondWeekoffHalfDayValue;
                if (IsSecondWeekoffHalfDay.Contains(","))
                {
                    string[] IsSecondWeekoffHalfDayRecord = IsSecondWeekoffHalfDay.Split(',');
                    IsSecondWeekoffHalfDayValue = IsSecondWeekoffHalfDayRecord[0].Trim();
                }
                else
                {
                    IsSecondWeekoffHalfDayValue = IsSecondWeekoffHalfDay;
                }
                EmployeeModelObj.IsSecondWeekoffHalfDay = Convert.ToBoolean(IsSecondWeekoffHalfDayValue);
                #endregion

                EmployeeModelObj.First = Convert.ToBoolean(FirstValue);
                EmployeeModelObj.Second = Convert.ToBoolean(SecondValue);
                EmployeeModelObj.Third = Convert.ToBoolean(ThirdValue);
                EmployeeModelObj.Fourth = Convert.ToBoolean(FourthValue);
                EmployeeModelObj.Fifth = Convert.ToBoolean(FifthValue);

                return ReturnStatus = true;

                // Added by Rajas on 13 JAN 2017 END
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in GetCheckBoxValues due to" + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                return ReturnStatus;
            }

        }

        /// <summary>
        /// UpdateEmployeeData
        /// ADDED BY Rajas ON 29 DEC 2016
        /// </summary>
        /// <param name="NewCompanyObj"></param>
        /// <returns></returns>
        /// UPDATED BY RAJAS ON 3 JUNE 2017
        private bool UpdateEmployeeData(EmployeeModel EmployeeModelObj, bool IsEdit, ref string UpdateStatus)
        {
            bool ReturnStatus = false;

            try
            {
                // Added for saving the companymaster model to company table object, by Rajas on 29 DEC 2016 START
                WetosDB.Employee EmployeeTblObj = WetosDB.Employees.Where(a => a.EmployeeCode.Trim() == EmployeeModelObj.EmployeeCode.Trim()
                    || a.EmployeeId == EmployeeModelObj.EmployeeId).FirstOrDefault();

                WetosDB.EmployeeExtraDetail EmployeeExtraDetailTblObj = WetosDB.EmployeeExtraDetails.Where(a => a.EmployeeCode.Trim() == EmployeeModelObj.EmployeeCode.Trim()
                    || a.EmployeeId == EmployeeModelObj.EmployeeId).FirstOrDefault();

                //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 11 SEPTEMBER 2017 FOR EDIT
                WetosDB.Employee EmployeeTblObjEDIT = WetosDB.Employees.Where(a => a.EmployeeCode.Trim() == EmployeeModelObj.EmployeeCode.Trim()
                    || a.EmployeeId == EmployeeModelObj.EmployeeId).FirstOrDefault();

                bool IsNew = false;
                bool IsNewEmployeeExtraDetals = false;
                if (EmployeeTblObj == null && IsEdit == false)
                {
                    EmployeeTblObj = new WetosDB.Employee();
                    IsNew = true;
                }

                if (EmployeeExtraDetailTblObj == null)
                {
                    EmployeeExtraDetailTblObj = new WetosDB.EmployeeExtraDetail();
                    IsNewEmployeeExtraDetals = true;
                }


                int OldCompanyId = 0;
                int OldBranchId = 0;

                // New Leave table object
                if (IsNew == false)
                {
                    OldCompanyId = EmployeeTblObj.CompanyId;
                    OldBranchId = EmployeeTblObj.BranchId;
                }

                EmployeeTblObj.EmployeeCode = EmployeeModelObj.EmployeeCode;

                EmployeeTblObj.FirstName = EmployeeModelObj.FirstName;

                EmployeeTblObj.LastName = EmployeeModelObj.LastName;

                EmployeeTblObj.MiddleName = EmployeeModelObj.MiddleName;

                EmployeeTblObj.Gender = EmployeeModelObj.Gender;

                EmployeeTblObj.Address1 = EmployeeModelObj.Address1;

                EmployeeTblObj.Address2 = EmployeeModelObj.Address2;

                EmployeeTblObj.MailAddress = EmployeeModelObj.MailAddress;

                EmployeeTblObj.ResignationDate = EmployeeModelObj.ResignationDate;//ADDED BY PRATHMESH ON 17 APRIL 2019 

                EmployeeTblObj.Leavingdate = EmployeeModelObj.Leavingdate;//ADDED BY PRATHMESH ON 17 APRIL 2019 

                EmployeeTblObj.NoticePeriod = EmployeeModelObj.NoticePeriod;//ADDED BY PRATHMESH ON 17 APRIL 2019 

                // Null value exception handling updated by Rajas on 17 FEB 2017
                if (EmployeeModelObj.Prefix != null)
                {
                    EmployeeTblObj.Title = EmployeeModelObj.Prefix == "--Select--" ? " " : EmployeeModelObj.Prefix;
                }
                else
                {
                    EmployeeTblObj.Title = string.Empty;  // Updated by Rajas on 17 FEB 2017
                }

                //EmployeeTblObj.BloodGroup.BloodGroupId = EmployeeModelObj.BloodGroupId == null ? 0 : EmployeeModelObj.BloodGroupId.Value; // Uncommented by Rajas on 17 FEB 2017

                EmployeeTblObj.ReligionId = EmployeeModelObj.ReligionId;

                EmployeeTblObj.DesignationId = EmployeeModelObj.DesignationId;

                EmployeeTblObj.DepartmentId = EmployeeModelObj.DepartmentId;

                EmployeeTblObj.EmployeeReportingId = EmployeeModelObj.EmployeeReportingId;

                EmployeeTblObj.GradeId = EmployeeModelObj.GradeId;

                EmployeeTblObj.MarritalStatus = EmployeeModelObj.MarritalStatus;

                EmployeeTblObj.Email = EmployeeModelObj.Email;

                EmployeeTblObj.Telephone1 = EmployeeModelObj.Telephone1;

                EmployeeTblObj.Telephone2 = EmployeeModelObj.Telephone2;

                EmployeeTblObj.Moblie = EmployeeModelObj.Moblie;

                //CODE ADDED BY SHRADDHA ON 06 MARCH 2018 START
                if (EmployeeModelObj.Leavingdate == null)
                {
                    EmployeeTblObj.SeperationRemark = string.Empty;
                }
                else
                {
                    EmployeeTblObj.SeperationRemark = EmployeeModelObj.SeperationRemark;
                }
                //CODE ADDED BY SHRADDHA ON 06 MARCH 2018 END


                //EmployeeTblObj.BirthDate = EmployeeTblObj.BirthDate != null ? (EmployeeModelObj.BirthDate.Value.Year <= 1753 ? null : EmployeeModelObj.BirthDate) : null;  // EmployeeModelObj.BirthDate;
                //EmployeeTblObj.JoiningDate = EmployeeTblObj.JoiningDate != null ? (EmployeeModelObj.JoiningDate.Value.Year <= 1753 ? null : EmployeeTblObj.JoiningDate) : null; // EmployeeModelObj.JoiningDate;  //null;
                //EmployeeTblObj.Leavingdate = EmployeeTblObj.Leavingdate != null ? (EmployeeModelObj.Leavingdate.Value.Year <= 1753 ? null : EmployeeTblObj.Leavingdate) : null; // EmployeeModelObj.Leavingdate; //null;

                // Added by Rajas on 16 MARCH 2017 START

                DateTime DefaultDate = Convert.ToDateTime("1900/01/01");   // Default date 
                EmployeeTblObj.BirthDate = EmployeeTblObj.BirthDate != null ? (EmployeeModelObj.BirthDate.Value.Year <= 1947 ? null : EmployeeModelObj.BirthDate) : EmployeeModelObj.BirthDate;

                //MODIFIED BY PUSHKAR ON 21 FEB 2019
                EmployeeTblObj.JoiningDate = EmployeeTblObj.JoiningDate != null ? (EmployeeModelObj.JoiningDate.Value.Year <= 1947 ? null : EmployeeModelObj.JoiningDate) : EmployeeModelObj.JoiningDate;

                EmployeeTblObj.Leavingdate = EmployeeModelObj.Leavingdate;

                //if (EmployeeModelObj.Leavingdate == null)
                //{
                //    EmployeeTblObj.Leavingdate = DefaultDate;
                //}
                //else
                //{
                //    EmployeeTblObj.Leavingdate = EmployeeModelObj.Leavingdate;
                //}

                if (EmployeeModelObj.JoiningDate != null) //  && EmployeeModelObj.Leavingdate == DefaultDate)
                {
                    EmployeeTblObj.ActiveFlag = true;
                }
                else if (EmployeeModelObj.JoiningDate != null && EmployeeModelObj.Leavingdate != DefaultDate)
                {
                    EmployeeTblObj.ActiveFlag = false;
                }


                //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO RESOLVE THE ISSUE THAT AFTER ADDING LEAVING DATE ACTIVE FLAG REMAINS TRUE ISSUE POINTED BY DEEPTI MAM EVISKA FOR REPORT ON 01 NOV 2017 START
                if (EmployeeModelObj.Leavingdate != null && EmployeeModelObj.Leavingdate < DateTime.Now) //  && EmployeeModelObj.Leavingdate == DefaultDate)
                {
                    EmployeeTblObj.ActiveFlag = false;
                }
                //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO RESOLVE THE ISSUE THAT AFTER ADDING LEAVING DATE ACTIVE FLAG REMAINS TRUE ISSUE POINTED BY DEEPTI MAM EVISKA FOR REPORT ON 01 NOV 2017 END

                #region COMMENTED CODE
                // check confirm date and joining date validation
                //if (EmployeeModelObj.ConfirmDate < EmployeeModelObj.JoiningDate)
                //{
                //    UpdateStatus = " Confirmation date should be greater than Joining date";

                //    ReturnStatus = false;

                //    return ReturnStatus;
                //}

                //// Check birth date validation 
                //DateTime Today = DateTime.Now;

                //DateTime DOB = Today.AddYears(-18);

                //if (Today.Date >= DOB.Date && EmployeeModelObj.BirthDate >= DOB.Date)
                //{
                //    UpdateStatus = "Age limit should be greater than 18 years";

                //    ReturnStatus = false;

                //    return ReturnStatus;
                //}
                #endregion

                // Added by Rajas on 16 MARCH 2017 START

                //EmployeeTblObj.Leavingdate = EmployeeTblObj.Leavingdate != null ? (EmployeeModelObj.Leavingdate.Value.Year <= 1753 ? null : EmployeeTblObj.Leavingdate) : EmployeeModelObj.Leavingdate;

                // Updated by Rajas on 11 JAN 2017 for calculating confirm date = joining date + 6 months
                //EmployeeTblObj.ConfirmDate = EmployeeTblObj.ConfirmDate == null ? EmployeeModelObj.JoiningDate.Value.AddMonths(6) : EmployeeModelObj.ConfirmDate;

                EmployeeTblObj.ConfirmDate = EmployeeModelObj.ConfirmDate != null ? (EmployeeModelObj.ConfirmDate.Value.Year <= 1753 ? null : EmployeeModelObj.ConfirmDate) : EmployeeModelObj.ConfirmDate;

                EmployeeTblObj.PFNO = EmployeeModelObj.PFNO;
                EmployeeTblObj.PANNO = EmployeeModelObj.PANNO;
                //EmployeeTblObjbj.ESICNO = EmployeeModelObj.ESICNO;
                //EmployeeTblObjbj.ESICDCD = EmployeeModelObj.ESICDCD;
                //EmployeeTblObjbj.BankCode = EmployeeModelObj.BankCode;
                EmployeeTblObj.BankNo = EmployeeModelObj.BankNo;
                //EmployeeTblObjbj.LICNo = EmployeeModelObj.LICNo;

                EmployeeTblObj.CompanyId = EmployeeModelObj.CompanyId;

                EmployeeTblObj.BranchId = EmployeeModelObj.BranchId;

                EmployeeTblObj.DivisionId = EmployeeModelObj.DivisionId;

                EmployeeTblObj.EmployeeReportingId2 = EmployeeModelObj.EmployeeReportingId2;

                EmployeeTblObj.EmployeeTypeId = EmployeeModelObj.EmployeeTypeId; // Added by Rajas on 17 FEB 2017

                EmployeeTblObj.CardNumber = EmployeeModelObj.CardNumber;

                // Added by Rajas on 11 JAN 2017, as week off selection provided instead of hardcode value START

                EmployeeTblObj.WeeklyOff1 = EmployeeModelObj.WeeklyOff1;

                EmployeeTblObj.WeeklyOff2 = EmployeeModelObj.WeeklyOff2;

                EmployeeTblObj.First = EmployeeModelObj.First;

                EmployeeTblObj.Second = EmployeeModelObj.Second;

                EmployeeTblObj.Third = EmployeeModelObj.Third;

                EmployeeTblObj.Fourth = EmployeeModelObj.Fourth;

                EmployeeTblObj.Fifth = EmployeeModelObj.Fifth;

                EmployeeTblObj.DefaultShift = EmployeeModelObj.DefaultShift;
                //EmployeeTblObj.JoiningDate = EmployeeModelObj.JoiningDate;
                // Added by Rajas on 11 JAN 2017, as week off selection provided instead of hardcode value END

                EmployeeTblObj.Picture = EmployeeModelObj.Picture; // Updated by Rajas on 23 FEB 2017

                #region CODE ADDED BY SHRADDHA ON 12 MAR 2018 FOR SECOND WEEK OFF HALF DAY
                //CODE ADDED BY SHRADDHA ON 12 MAR 2018 START
                EmployeeModelObj.IsSecondWeekoffHalfDay = EmployeeModelObj.IsSecondWeekoffHalfDay == null ? false : EmployeeModelObj.IsSecondWeekoffHalfDay.Value;// CODE ADDED BY SHRADDHA ON 12 MAR 2018
                if (EmployeeModelObj.IsSecondWeekoffHalfDay == true)
                {
                    EmployeeTblObj.WeeklyHalfDay = EmployeeTblObj.WeeklyOff2;
                }

                //CODE ADDED BY SHRADDHA ON 12 MAR 2018 END
                #endregion

                // Add new table object 
                if (IsNew)
                {
                    WetosDB.Employees.Add(EmployeeTblObj);

                    WetosDB.SaveChanges();


                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 11 SEPTEMBER 2017
                    #region ADD AUDIT LOG
                    //OLD RECORD IS BLANK
                    string Newrecord = "Employee Code :" + EmployeeTblObj.EmployeeCode + ", First Name :" + EmployeeTblObj.FirstName +
                        ", Last Name :" + EmployeeTblObj.LastName + ", Middle Name :" + EmployeeTblObj.MiddleName +
                        ", Address1 :" + EmployeeTblObj.Address1 + ", Address2 :" + EmployeeTblObj.Address2
                        + ", Mail Address :" + EmployeeTblObj.MailAddress + "Title :" + EmployeeTblObj.Title
                        + ", Gender :" + EmployeeTblObj.Gender + ", Card Number :" + EmployeeTblObj.CardNumber
                        + ", Default Shift :" + EmployeeTblObj.DefaultShift
                        //+ ", Blood Group ID :" + EmployeeTblObj.BloodGroup.BloodGroupId == null ? string.Empty : EmployeeTblObj.BloodGroup.BloodGroupId // Updated by Rajas on 22 SEP 2017 to Handle null error
                        + ", Religion ID :" + EmployeeTblObj.ReligionId + ", Designation ID :" + EmployeeTblObj.DesignationId +
                        ", Department ID :" + EmployeeTblObj.DepartmentId + ", EmployeeReporting ID :" + EmployeeTblObj.EmployeeReportingId +
                        ", Second EmployeeReporting ID :" + EmployeeTblObj.EmployeeReportingId2 + ", Grade ID :" + EmployeeTblObj.GradeId +
                        ", Marital Status :" + EmployeeTblObj.MarritalStatus + ", Email :" + EmployeeTblObj.Email
                        + ", Telephone1 :" + EmployeeTblObj.Telephone1 + ", Telephone2 :" + EmployeeTblObj.Telephone2
                        + ", Mobile :" + EmployeeTblObj.Moblie + ", Birth Date :" + EmployeeTblObj.BirthDate
                        + ", Confirm Date :" + EmployeeTblObj.ConfirmDate + ", Joining Date :" + EmployeeTblObj.JoiningDate
                        + ", ActiveFlag :" + EmployeeTblObj.ActiveFlag
                        + ", WeekOff1 :" + EmployeeTblObj.WeeklyOff1
                        + ", WeekOff2 :" + EmployeeTblObj.WeeklyOff2 + ", First :" + EmployeeTblObj.First
                        + ", Second :" + EmployeeTblObj.Second + ", Third :" + EmployeeTblObj.Third
                        + ", Fourth :" + EmployeeTblObj.Fourth + ", Fifth :" + EmployeeTblObj.Fifth
                        + ", CompanyId :" + EmployeeTblObj.CompanyId + ", BranchId :" + EmployeeTblObj.BranchId
                        + ", DivisionId :" + EmployeeTblObj.DivisionId;

                    //LOGIN ID IS TAKEN FROM SESSION PERSISTER
                    string Formname = "EMPLOYEE MASTER";
                    //ACTION IS UPDATE
                    string Message = " ";

                    WetosAdministrationController.GenerateAuditLogsInsert(WetosDB, EmployeeTblObj.EmployeeId, Formname, Newrecord, ref Message);
                    #endregion
                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 11 SEPTEMBER 2017


                    AddAuditTrail("Employee added");
                }
                else
                {
                    WetosDB.SaveChanges();


                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 11 SEPTEMBER 2017
                    #region ADD AUDIT LOG
                    string Oldrecord = "Employee Code :" + EmployeeTblObjEDIT.EmployeeCode + ", First Name :" + EmployeeTblObjEDIT.FirstName +
                       ", Last Name :" + EmployeeTblObjEDIT.LastName + ", Middle Name :" + EmployeeTblObjEDIT.MiddleName +
                       ", Address1 :" + EmployeeTblObjEDIT.Address1 + ", Address2 :" + EmployeeTblObjEDIT.Address2
                       + ", Mail Address :" + EmployeeTblObjEDIT.MailAddress + "Title :" + EmployeeTblObjEDIT.Title
                       + ", Gender :" + EmployeeTblObjEDIT.Gender + ", Card Number :" + EmployeeTblObjEDIT.CardNumber
                       + ", Default Shift :" + EmployeeTblObjEDIT.DefaultShift
                        //+ ", Blood Group ID :" + EmployeeTblObjEDIT.BloodGroup.BloodGroupId == null ? string.Empty : EmployeeTblObjEDIT.BloodGroup.BloodGroupId // Updated by Rajas on 22 SEP 2017 to handle null error
                       + ", Religion ID :" + EmployeeTblObjEDIT.ReligionId + ", Designation ID :" + EmployeeTblObjEDIT.DesignationId +
                       ", Department ID :" + EmployeeTblObjEDIT.DepartmentId + ", EmployeeReporting ID :" + EmployeeTblObjEDIT.EmployeeReportingId +
                       ", Second EmployeeReporting ID :" + EmployeeTblObjEDIT.EmployeeReportingId2 + ", Grade ID :" + EmployeeTblObjEDIT.GradeId +
                       ", Marital Status :" + EmployeeTblObjEDIT.MarritalStatus + ", Email :" + EmployeeTblObjEDIT.Email
                       + ", Telephone1 :" + EmployeeTblObjEDIT.Telephone1 + ", Telephone2 :" + EmployeeTblObjEDIT.Telephone2
                       + ", Mobile :" + EmployeeTblObjEDIT.Moblie + ", Birth Date :" + EmployeeTblObjEDIT.BirthDate
                       + ", Confirm Date :" + EmployeeTblObjEDIT.ConfirmDate + ", Joining Date :" + EmployeeTblObjEDIT.JoiningDate
                       + ", ActiveFlag :" + EmployeeTblObjEDIT.ActiveFlag
                       + ", WeekOff1 :" + EmployeeTblObjEDIT.WeeklyOff1
                       + ", WeekOff2 :" + EmployeeTblObjEDIT.WeeklyOff2 + ", First :" + EmployeeTblObjEDIT.First
                       + ", Second :" + EmployeeTblObjEDIT.Second + ", Third :" + EmployeeTblObjEDIT.Third
                       + ", Fourth :" + EmployeeTblObjEDIT.Fourth + ", Fifth :" + EmployeeTblObjEDIT.Fifth
                       + ", CompanyId :" + EmployeeTblObjEDIT.CompanyId + ", BranchId :" + EmployeeTblObjEDIT.BranchId
                       + ", DivisionId :" + EmployeeTblObjEDIT.DivisionId;
                    //---x---------x-----------------x------------x----------------x-----------x-------------x------------x--------------x----------x---------x-----
                    string Newrecord = "Employee Code :" + EmployeeTblObj.EmployeeCode + ", First Name :" + EmployeeTblObj.FirstName +
                       ", Last Name :" + EmployeeTblObj.LastName + ", Middle Name :" + EmployeeTblObj.MiddleName +
                       ", Address1 :" + EmployeeTblObj.Address1 + ", Address2 :" + EmployeeTblObj.Address2
                       + ", Mail Address :" + EmployeeTblObj.MailAddress + "Title :" + EmployeeTblObj.Title
                       + ", Gender :" + EmployeeTblObj.Gender + ", Card Number :" + EmployeeTblObj.CardNumber
                       + ", Default Shift :" + EmployeeTblObj.DefaultShift
                        // + ", Blood Group ID :" + EmployeeTblObj.BloodGroup.BloodGroupId == null ? string.Empty : EmployeeTblObj.BloodGroup.BloodGroupId // Updated by Rajas on 22 SEP 2017 to handle null error
                       + ", Religion ID :" + EmployeeTblObj.ReligionId + ", Designation ID :" + EmployeeTblObj.DesignationId +
                       ", Department ID :" + EmployeeTblObj.DepartmentId + ", EmployeeReporting ID :" + EmployeeTblObj.EmployeeReportingId +
                       ", Second EmployeeReporting ID :" + EmployeeTblObj.EmployeeReportingId2 + ", Grade ID :" + EmployeeTblObj.GradeId +
                       ", Marital Status :" + EmployeeTblObj.MarritalStatus + ", Email :" + EmployeeTblObj.Email
                       + ", Telephone1 :" + EmployeeTblObj.Telephone1 + ", Telephone2 :" + EmployeeTblObj.Telephone2
                       + ", Mobile :" + EmployeeTblObj.Moblie + ", Birth Date :" + EmployeeTblObj.BirthDate
                       + ", Confirm Date :" + EmployeeTblObj.ConfirmDate + ", Joining Date :" + EmployeeTblObj.JoiningDate
                       + ", ActiveFlag :" + EmployeeTblObj.ActiveFlag
                       + ", WeekOff1 :" + EmployeeTblObj.WeeklyOff1
                       + ", WeekOff2 :" + EmployeeTblObj.WeeklyOff2 + ", First :" + EmployeeTblObj.First
                       + ", Second :" + EmployeeTblObj.Second + ", Third :" + EmployeeTblObj.Third
                       + ", Fourth :" + EmployeeTblObj.Fourth + ", Fifth :" + EmployeeTblObj.Fifth
                       + ", CompanyId :" + EmployeeTblObj.CompanyId + ", BranchId :" + EmployeeTblObj.BranchId
                       + ", DivisionId :" + EmployeeTblObj.DivisionId;

                    //LOGIN ID IS TAKEN FROM SESSION PERSISTER
                    string Formname = "EMPLOYEE MASTER";
                    //ACTION IS UPDATE
                    string Message = " ";

                    WetosAdministrationController.GenerateAuditLogsUpdate(WetosDB, EmployeeTblObj.EmployeeId, Formname, Oldrecord,
                        Newrecord, ref Message);
                    #endregion
                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 11 SEPTEMBER 2017


                    //Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeCode == EmployeeModelObj.EmployeeCode).FirstOrDefault();

                    DateTime LeaveEffectiveDateFrom = Convert.ToDateTime(EmployeeModelObj.LeaveEffectiveFromDate);
                    #region CODE COMMENTED BY SHRADDHA ON 09 FEB 2018 BECAUSE AS PER MSJ LEAVE SHOULD NOT BE DIRECTLY ADDED ON EMPLOYEE ADD BUT WE HAVE PROVIDED MANUAL FACILITY FOR LEAVE CREDIT. IT SHOULD BE USED.
                    //WetosEmployeeController.UpdateLeaveCreditBalance(WetosDB, EmployeeObj.EmployeeId, LeaveEffectiveDateFrom, OldCompanyId, EmployeeObj.CompanyId, OldBranchId, EmployeeObj.BranchId);
                    #endregion
                }

                #region ADD / UPDATE FAMILY DETAILS IN TBLFAMILYDETAILS
                //CODE ADDED BY SHRADDHA ON 10 FEB 2018 START
                int EmployeeId = WetosDB.Employees.Where(a => a.EmployeeCode == EmployeeModelObj.EmployeeCode).Select(a => a.EmployeeId).FirstOrDefault();
                foreach (FamilyDetails EmployeeModelForFamilyDetails in EmployeeModelObj.TblFamilyDetailList)
                {
                    if (!string.IsNullOrEmpty(EmployeeModelForFamilyDetails.Name))
                    {
                        TblFamilyDetail TblFamilyDetailObj = WetosDB.TblFamilyDetails.Where(a => a.EmployeeId == EmployeeId && a.Id == EmployeeModelForFamilyDetails.Id).FirstOrDefault();
                        bool IsNewTblFamilyDetail = false;
                        if (TblFamilyDetailObj == null)
                        {
                            TblFamilyDetailObj = new TblFamilyDetail();
                            IsNewTblFamilyDetail = true;
                        }
                        TblFamilyDetailObj.EmployeeId = EmployeeId;
                        if (EmployeeModelForFamilyDetails.DateOfBirth != null)
                        {
                            TblFamilyDetailObj.DateOfBirth = Convert.ToDateTime(EmployeeModelForFamilyDetails.DateOfBirth != null ? (Convert.ToDateTime(EmployeeModelForFamilyDetails.DateOfBirth).Year <= 1753 ? null : EmployeeModelForFamilyDetails.DateOfBirth) : EmployeeModelForFamilyDetails.DateOfBirth);
                        }
                        TblFamilyDetailObj.Name = EmployeeModelForFamilyDetails.Name;
                        TblFamilyDetailObj.Relation = EmployeeModelForFamilyDetails.Relation;
                        TblFamilyDetailObj.Gender = EmployeeModelForFamilyDetails.Gender;
                        TblFamilyDetailObj.Picture = EmployeeModelForFamilyDetails.Picture;

                        if (IsNewTblFamilyDetail == true)
                        {
                            WetosDB.TblFamilyDetails.Add(TblFamilyDetailObj);
                        }
                        WetosDB.SaveChanges();
                    }
                    else
                    {
                        TblFamilyDetail TblFamilyDetailObj = WetosDB.TblFamilyDetails.Where(a => a.EmployeeId == EmployeeId && a.Id == EmployeeModelForFamilyDetails.Id).FirstOrDefault();
                        if (TblFamilyDetailObj != null)
                        {
                            WetosDB.TblFamilyDetails.Remove(TblFamilyDetailObj);
                            WetosDB.SaveChanges();
                        }
                    }
                }
                //CODE ADDED BY SHRADDHA ON 10 FEB 2018 END
                #endregion


                // Added by Rajas on 18 FEB 2017 to add employee group in EmployeeGroupdetail table START

                // Check if data is available in table
                WetosDB.EmployeeGroupDetail EmployeeGroupdetailsTblObj = WetosDB.EmployeeGroupDetails.Where(a => a.Employee.EmployeeId == EmployeeModelObj.EmployeeId).FirstOrDefault();

                bool IsNewGroup = false;

                if (EmployeeGroupdetailsTblObj == null)
                {
                    EmployeeGroupdetailsTblObj = new WetosDB.EmployeeGroupDetail();

                    IsNewGroup = true;
                }

                // Update details

                // Added by Rajas on 22 SEP 2017 to handle Employee null error in FK ref
                EmployeeGroupdetailsTblObj.Employee = WetosDB.Employees.Where(a => a.EmployeeCode == EmployeeModelObj.EmployeeCode).FirstOrDefault();

                EmployeeGroupdetailsTblObj.Employee.EmployeeId = EmployeeTblObj.EmployeeId;
                DateTime LeaveEffectiveFromDate = Convert.ToDateTime(EmployeeModelObj.LeaveEffectiveFromDate);

                if (IsNewGroup)
                {
                    // Added by Rajas on 22 SEP 2017 to handle EmployeeGroup null error in FK ref
                    EmployeeGroupdetailsTblObj.EmployeeGroup = WetosDB.EmployeeGroups.Where(a => a.EmployeeGroupId == EmployeeModelObj.EmployeeGroupId).FirstOrDefault();


                    //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 27 NOV 2017 TO RESOLVE EMPLOYEEGROUPID OBJECT REFERENCE KEY ERROR START
                    //EmployeeGroupdetailsTblObj.EmployeeGroup.EmployeeGroupId = EmployeeModelObj.EmployeeGroupId;
                    EmployeeGroupdetailsTblObj.EmployeeGroup = WetosDB.EmployeeGroups.Where(a => a.EmployeeGroupId == EmployeeModelObj.EmployeeGroupId).FirstOrDefault();
                    //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 27 NOV 2017 TO RESOLVE EMPLOYEEGROUPID OBJECT REFERENCE KEY ERROR END

                    WetosDB.EmployeeGroupDetails.Add(EmployeeGroupdetailsTblObj);

                    WetosDB.SaveChanges();
                    AddAuditTrail("Employee group details added");

                    #region CODE COMMENTED BY SHRADDHA ON 09 FEB 2018 BECAUSE AS PER MSJ LEAVE SHOULD NOT BE DIRECTLY ADDED ON EMPLOYEE ADD BUT WE HAVE PROVIDED MANUAL FACILITY FOR LEAVE CREDIT. IT SHOULD BE USED.
                    //CODE ADDED BY SHRADDHA ON 30 MAY 2017
                    //if (WetosEmployeeController.AddUpdateLeaveCreditBalance(WetosDB, EmployeeGroupdetailsTblObj.Employee.EmployeeId, LeaveEffectiveFromDate, -1, EmployeeModelObj.EmployeeGroupId) == true)
                    //{
                    //    AddAuditTrail("Credit entry added");
                    //}
                    //else
                    //{
                    //    AddAuditTrail("Credit entry Failed");
                    //}
                    #endregion
                }
                else
                {
                    #region CODE COMMENTED BY SHRADDHA ON 09 FEB 2018 BECAUSE AS PER MSJ LEAVE SHOULD NOT BE DIRECTLY ADDED ON EMPLOYEE ADD BUT WE HAVE PROVIDED MANUAL FACILITY FOR LEAVE CREDIT. IT SHOULD BE USED.
                    //if (WetosEmployeeController.AddUpdateLeaveCreditBalance(WetosDB, EmployeeGroupdetailsTblObj.Employee.EmployeeId, LeaveEffectiveFromDate, EmployeeGroupdetailsTblObj.EmployeeGroup.EmployeeGroupId, EmployeeModelObj.EmployeeGroupId) == true)
                    //{
                    //    AddAuditTrail("Balance entry added/updated");
                    //}
                    //else
                    //{
                    //    AddAuditTrail("Balance entry Failed");
                    //}
                    #endregion
                    //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 27 NOV 2017 TO RESOLVE EMPLOYEEGROUPID OBJECT REFERENCE KEY ERROR START
                    //EmployeeGroupdetailsTblObj.EmployeeGroup.EmployeeGroupId = EmployeeModelObj.EmployeeGroupId;
                    EmployeeGroupdetailsTblObj.EmployeeGroup = WetosDB.EmployeeGroups.Where(a => a.EmployeeGroupId == EmployeeModelObj.EmployeeGroupId).FirstOrDefault();
                    //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 27 NOV 2017 TO RESOLVE EMPLOYEEGROUPID OBJECT REFERENCE KEY ERROR END
                    WetosDB.SaveChanges();
                }


                if (IsNew)
                {
                    Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeCode == EmployeeTblObj.EmployeeCode).FirstOrDefault();

                    GetLeavedataNewLogic(WetosDB, EmployeeObj.EmployeeId, DateTime.Now);
                }
                Employee EmployeeObjNew = WetosDB.Employees.Where(a => a.EmployeeCode == EmployeeTblObj.EmployeeCode).FirstOrDefault();

                EmployeeExtraDetailTblObj.ExperienceWithFlagship = EmployeeModelObj.FlagshipExperience;
                EmployeeExtraDetailTblObj.ExperienceBeforeFlagship = EmployeeModelObj.PrevExperience;
                EmployeeExtraDetailTblObj.GraduationPassingYear = EmployeeModelObj.GradPassingYear;
                EmployeeExtraDetailTblObj.PostGraduationPassingYear = EmployeeModelObj.PostGradPassingYear;
                EmployeeExtraDetailTblObj.OtherPaasingYear = EmployeeModelObj.OtherPassingYear;
                EmployeeExtraDetailTblObj.PFNo = EmployeeModelObj.PFNO;
                EmployeeExtraDetailTblObj.ESICNo = EmployeeModelObj.ESICNO;
                //EmployeeExtraDetailTblObj.ExperienceWithFlagship = EmployeeModelObj.FlagshipExperience;
                EmployeeExtraDetailTblObj.EmployeeId = EmployeeObjNew.EmployeeId;
                EmployeeExtraDetailTblObj.EmployeeCode = EmployeeObjNew.EmployeeCode;

                EmployeeExtraDetailTblObj.GraduationQualification = EmployeeModelObj.GraduationQualification;
                EmployeeExtraDetailTblObj.PostGraduationQualification = EmployeeModelObj.PostGraduationQualification;
                EmployeeExtraDetailTblObj.OtherGraduationQualification = EmployeeModelObj.OtherGraduationQualification;

                //ADDED BY SHRADDHA ON 21 JUNE 2017 TO ADD LEAVE EFFECTIVE FROM DATE IN TABLE
                EmployeeExtraDetailTblObj.LeaveEffectiveFromDate = EmployeeModelObj.LeaveEffectiveFromDate;
                if (IsNewEmployeeExtraDetals == true)
                {
                    WetosDB.EmployeeExtraDetails.Add(EmployeeExtraDetailTblObj);

                }
                WetosDB.SaveChanges();
                AddAuditTrail("Update function complete save");
                // Added by Rajas on 23 FEB 2017, original change added by Shraddha
                //Session["Picture"] = WetosDB.Employees.Where(a => a.EmployeeId == EmployeeTblObj.EmployeeId).Select(a => a.Picture).FirstOrDefault();

                if (IsNew)
                {
                    // ADDED BY RAJAS FOR SUCCESS and AUDITLOG ON 27 DEC 2016
                    UpdateStatus = "Success - Employee : " + EmployeeModelObj.EmployeeCode + " " + EmployeeModelObj.FirstName + " " + EmployeeModelObj.LastName + " is added Successfully"; // Updated by Rajas on 16 JAN 2017
                }
                else
                {
                    //ADDED CODE FOR SUCCESS and AUDITLOG MESSAGE BY RAJAS ON 16 JAN 2017
                    UpdateStatus = "Success - Employee : " + EmployeeModelObj.EmployeeCode + " " + EmployeeModelObj.FirstName + " " + EmployeeModelObj.LastName + " is updated Successfully"; // Updated by Rajas on 16 JAN 2017

                }

                return ReturnStatus = true;  // Updated by Rajas on 4 APRIL 2017
            }

            catch (System.Exception ex)
            {
                AddAuditTrail("Error in updating employee data due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                UpdateStatus = "Error in updating employee data due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message);

                ReturnStatus = false;

                return ReturnStatus;
            }
        }

        // COMMNETED BY MSJ ON 15 SEP 2017   //UNCOMMENTED BY SHRADDHA ON 16 SEP 2017 BECAUSE IT IS REQUIRED
        ///// <summary> 
        ///// Leave auto credit code 
        ///// Added by Rajas on 10 MAY 2017
        ///// </summary>
        ///// <returns></returns>
        public bool LeaveAutoCredit(int id)
        {
            bool ReturnStatus = false;

            try
            {
                // ADDED BY MSJ ON 08 MAY 2017 START
                //DateTime LeaveAsonDate = new DateTime(2017, 2, 1);
                //DateTime LeaveAsonDate = new DateTime(2017, 2, 15);
                DateTime LeaveAsonDate = DateTime.Now;
                //DateTime LeaveAsonDate = new DateTime(2017, 3, 15);
                //DateTime LeaveAsonDate = new DateTime(2017, 3, 31);

                VwActiveEmployee SelectedEmp = WetosDB.VwActiveEmployees.Where(a => a.EmployeeId == id).FirstOrDefault();

                bool MonthlyCreditFlag = true; // GLOBAL SETTING
                // Get FY
                WetosDB.FinancialYear FYLA = WetosDB.FinancialYears.Where(a => a.StartDate <= LeaveAsonDate && a.EndDate >= LeaveAsonDate && a.Company.CompanyId == SelectedEmp.CompanyId && a.Branch.BranchId == SelectedEmp.BranchId).FirstOrDefault();

                double OpenCarryForward = 0.0;
                double OpenOpenBal = 0.0;
                double LeaveAllowed = 0.0;
                double LeaveBalance = 0.0;
                double TotalLeaveSanctioned = 0;

                var Opening = WetosDB.LeaveCredits.Where(a => a.EmployeeId == id && a.FinancialYearId == FYLA.FinancialYearId).FirstOrDefault();

                if (Opening != null)
                {
                    // Updated by Rajas on 1 AUGUST 2017 to handle null value START
                    OpenCarryForward = Opening.CarryForward == null ? 0 : Opening.CarryForward.Value;
                    OpenOpenBal = Opening.OpeningBalance == null ? 0 : Opening.OpeningBalance.Value;
                    // Updated by Rajas on 1 AUGUST 2017 to handle null value END

                    LeaveAllowed = OpenCarryForward;
                    LeaveBalance = OpenCarryForward;

                    DateTime ApplicableFrom = Opening.ApplicableEffectiveDate == null ? FYLA.StartDate : Opening.ApplicableEffectiveDate.Value;

                    if (MonthlyCreditFlag)
                    {
                        int count = 0;
                        for (int i = 1; ; i++)
                        {
                            if (ApplicableFrom.AddMonths(i) <= LeaveAsonDate)
                            {
                                count++;
                            }
                            else
                            {
                                break;
                            }
                        }

                        // LeaveAllowed = OpenCarryForward + (count * 1.75);
                    }

                    TotalLeaveSanctioned = GetAppliedDays(WetosDB, ApplicableFrom, LeaveAsonDate, id);
                    LeaveBalance = LeaveAllowed - TotalLeaveSanctioned;
                }
                else
                {
                    Opening = new LeaveCredit();

                    Opening.FinancialYearId = FYLA.FinancialYearId;

                    // GET LAST FY CLSOING BALANCAE
                    //DateTime LFYLeaveAsonDate = new DateTime(2017, 4, 1);
                    //DateTime LeaveAsonDate = new DateTime(2017, 3, 15);
                    //DateTime LeaveAsonDate = new DateTime(2017, 3, 31);
                    bool LFYMonthlyCreditFlag = true;
                    // Get FY
                    WetosDB.FinancialYear LFYFYLA = WetosDB.FinancialYears.Where(a => a.FinancialYearId == FYLA.PrevFYId).FirstOrDefault();

                    double LFYOpenCarryForward = 0.0;
                    double LFYOpenOpenBal = 0.0;
                    double LFYLeaveAllowed = 0.0;
                    double LFYLeaveBalance = 0.0;
                    double LFYTotalLeaveSanctioned = 0;

                    // CODE UPDATED BY RAJAS ON 12 MAY 2017 START

                    //var LFYOpening = WetosDB.LeaveCredits.Where(a => a.EmployeeId == id && a.FinancialYearId == LFYFYLA.FinancialYearId).FirstOrDefault();

                    List<LeaveCredit> LFYOpening = WetosDB.LeaveCredits.Where(a => a.EmployeeId == id && a.FinancialYearId == LFYFYLA.FinancialYearId).ToList();

                    if (LFYOpening.Count > 0)
                    {
                        // GET Employee group for selected employee
                        EmployeeGroupDetail EmployeeGroupDetailObj = WetosDB.EmployeeGroupDetails.Where(a => a.Employee.EmployeeId == id).FirstOrDefault(); //CODE MODIFIED BY SHRADDHA ON 16 SEP 2017 TAKEN a.Employee.EmployeeId INSTEAD OF a.EmployeeId IN WHERE CONDITION

                        List<LeaveMaster> LeaveMasterList = new List<LeaveMaster>();  // Verify

                        if (EmployeeGroupDetailObj != null)
                        {
                            // Get list of Leaves
                            LeaveMasterList = WetosDB.LeaveMasters.Where(a => a.EmployeeGroup.EmployeeGroupId == EmployeeGroupDetailObj.EmployeeGroup.EmployeeGroupId).ToList();  //CODE MODIFIED BY SHRADDHA ON 16 SEP 2017 TAKEN EmployeeGroupDetailObj.EmployeeGroup.EmployeeGroupId INSTEAD OF EmployeeGroupDetailObj.EmployeeGroupId IN WHERE CONDITION
                        }

                        foreach (var item in LFYOpening)
                        {
                            //foreach (var Leaves in LeaveMasterList)
                            //{
                            LFYOpenCarryForward = item.CarryForward.Value;
                            LFYOpenOpenBal = item.OpeningBalance.Value;
                            LFYLeaveAllowed = LFYOpenCarryForward;
                            LFYLeaveBalance = LFYOpenCarryForward;

                            DateTime LFYApplicableFrom = item.ApplicableEffectiveDate == null ? LFYFYLA.StartDate : item.ApplicableEffectiveDate.Value;

                            if (LFYMonthlyCreditFlag)
                            {
                                int LFYcount = 0;
                                for (int i = 1; ; i++)
                                {
                                    if (LFYApplicableFrom.AddMonths(i) <= LFYFYLA.EndDate)
                                    {
                                        LFYcount++;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                //  LFYLeaveAllowed = LFYOpenCarryForward + (LFYcount * 1.75);
                            }

                            LFYTotalLeaveSanctioned = GetAppliedDays(WetosDB, LFYApplicableFrom, LFYFYLA.EndDate, id);
                            LFYLeaveBalance = LFYLeaveAllowed - LFYTotalLeaveSanctioned;

                            // ASSIGNED VALUE
                            Opening.CarryForward = LFYLeaveBalance;
                            Opening.CompanyId = LFYFYLA.Company.CompanyId;
                            Opening.BranchId = LFYFYLA.Branch.BranchId;
                            double Balance = 0.0;

                            foreach (var Leaves in LeaveMasterList)
                            {
                                // Leave type from Prev credit matches with leave master list
                                if (item.LeaveType == Leaves.LeaveCode)
                                {
                                    Balance = Leaves.NoOfDaysAllowedInYear.Value;

                                    Opening.OpeningBalance = LFYLeaveBalance + Balance;
                                }
                            }

                            //ADDED BY SHRADDHA ON 10 MAY 2017 START
                            Opening.LeaveType = item.LeaveType;
                            Opening.ApplicableEffectiveDate = LFYApplicableFrom;
                            //ADDED BY SHRADDHA ON 10 MAY 2017 END

                            // Added by Rajas on 12 MAY 2017
                            Opening.EmployeeId = id;  // To save employee id
                            WetosDB.LeaveCredits.Add(Opening); // NEW CREDIT ENTRY FY
                            WetosDB.SaveChanges();

                            // Updated by Rajas on 17 MAY 2017
                            LeaveBalance LeaveBalances = WetosDB.LeaveBalances.Where(a => a.EmployeeId == id && a.LeaveType == item.LeaveType).FirstOrDefault();

                            if (LeaveBalances != null)  // Verify?
                            {
                                // Code added by Rajas on 17 MAY 2017 START
                                // Take leave balance table backup before updating entry
                                LeaveBalanceBackupData LeaveBalanceBackupDataObj = WetosDB.LeaveBalanceBackupDatas.Where(a => a.EmployeeId == id && a.LeaveType == item.LeaveType).FirstOrDefault();

                                bool IsNew = false;
                                if (LeaveBalanceBackupDataObj == null)
                                {
                                    LeaveBalanceBackupDataObj = new LeaveBalanceBackupData();
                                    IsNew = true;
                                }

                                LeaveBalanceBackupDataObj.LeaveType = item.LeaveType;

                                LeaveBalanceBackupDataObj.CurrentBalance = LeaveBalances.CurrentBalance;

                                LeaveBalanceBackupDataObj.EmployeeId = LeaveBalances.EmployeeId;

                                LeaveBalanceBackupDataObj.PreviousBalance = LeaveBalances.PreviousBalance;

                                LeaveBalanceBackupDataObj.CompanyId = LeaveBalances.CompanyId;

                                LeaveBalanceBackupDataObj.BranchId = LeaveBalances.BranchId;

                                LeaveBalanceBackupDataObj.LeaveUsed = LeaveBalances.LeaveUsed;

                                if (IsNew == true)
                                {
                                    WetosDB.LeaveBalanceBackupDatas.Add(LeaveBalanceBackupDataObj);
                                }

                                WetosDB.SaveChanges();

                                AddAuditTrail("Leave balance data backup to LeaveBalanceDataBackup updated for " + item.EmployeeId + " and leave type " + item.LeaveType);
                                // Code added by Rajas on 17 MAY 2017 END

                                // Update leave balance
                                LeaveBalances.PreviousBalance = LeaveBalances.CurrentBalance;
                                LeaveBalances.CurrentBalance = LFYLeaveBalance + Balance;
                                WetosDB.SaveChanges();
                            }

                            // get data for new fy start
                            // Updated by Rajas on 1 AUGUST 2017 to handle null value START
                            OpenCarryForward = Opening.CarryForward == null ? 0 : Opening.CarryForward.Value;
                            OpenOpenBal = Opening.OpeningBalance == null ? 0 : Opening.OpeningBalance.Value;
                            // Updated by Rajas on 1 AUGUST 2017 to handle null value END

                            LeaveAllowed = OpenCarryForward;
                            LeaveBalance = OpenCarryForward;

                            DateTime ApplicableFrom = Opening.ApplicableEffectiveDate == null ? FYLA.StartDate : Opening.ApplicableEffectiveDate.Value;

                            if (MonthlyCreditFlag)
                            {
                                int count = 0;
                                for (int i = 1; ; i++)
                                {
                                    if (ApplicableFrom.AddMonths(i) <= LeaveAsonDate)
                                    {
                                        count++;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                //  LeaveAllowed = OpenCarryForward + (count * 1.75);
                            }

                            TotalLeaveSanctioned = GetAppliedDays(WetosDB, ApplicableFrom, LeaveAsonDate, id);
                            LeaveBalance = LeaveAllowed - TotalLeaveSanctioned;
                            // get data fro new fy END

                            // Added by Rajas on 15 MAY 2017 START...Verify?
                            Opening = new LeaveCredit(); // Reassign value for object for next instanace

                            Opening.FinancialYearId = FYLA.FinancialYearId;
                            // Added by Rajas on 15 MAY 2017 END
                        }
                        //}
                    }
                }
                // ADDED BY MSJ ON 08 MAY 2017 END

                return ReturnStatus;
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error leave auto credit fuction due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                return ReturnStatus = false;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static double GetAppliedDays(WetosDBEntities WetosDB, DateTime FromDate, DateTime ToDate, int EmpId)
        {
            //ADDED MARK AS DELETE CONDITION BY SHRADDHA ON 18 JULY 2017
            List<LeaveApplication> LeaveAppList = WetosDB.LeaveApplications
                .Where(a => a.FromDate >= FromDate && a.FromDate <= ToDate && a.StatusId == 2 && a.EmployeeId == EmpId && a.MarkedAsDelete == 0).ToList();

            double TotalAppliedDays = 0.0;
            foreach (LeaveApplication LeaveTypeObj in LeaveAppList)
            {

                TotalAppliedDays = TotalAppliedDays + LeaveTypeObj.ActualDays.Value;
            }

            return TotalAppliedDays;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static double GetAppliedDaysTillMonthEnd(WetosDBEntities WetosDB, DateTime FromDate, DateTime ToDate, int EmpId, string LeaveType)
        {
            //ADDED MARK AS DELETE CONDITION BY SHRADDHA ON 18 JULY 2017
            // Get LastDayofMonth 
            DateTime LastDayOfMonth = (new DateTime(ToDate.Year, ToDate.Month, 1)).AddMonths(1).AddSeconds(-1);

            //ADDED MARK AS DELETE CONDITION BY SHRADDHA ON 18 JULY 2017
            //<LeaveApplication> LeaveAppList =



            //LeaveApplication LeaveAppObj = WetosDB.LeaveApplications
            //    .Where(a => a.FromDate >= FromDate && a.FromDate <= LastDayOfMonth && a.StatusId == 2
            //        && a.EmployeeId == EmpId && a.MarkedAsDelete == 0 && a.LeaveType == LeaveType).FirstOrDefault();
            List<LeaveApplication> LeaveAppList = WetosDB.LeaveApplications
                 .Where(a => a.FromDate >= FromDate && a.FromDate <= LastDayOfMonth && a.StatusId == 2
                     && a.EmployeeId == EmpId && a.MarkedAsDelete == 0 && a.LeaveType == LeaveType).ToList();

            double TotalAppliedDays = 0.0;


            //TotalAppliedDays = TotalAppliedDays + (LeaveAppObj == null ? 0.0 : LeaveAppObj.ActualDays.Value); // ADDED BY MSJ ON 22 JULY 2017 
            TotalAppliedDays = TotalAppliedDays + LeaveAppList.Sum(a => a.ActualDays).Value;
            //}

            return TotalAppliedDays;
        }

        /// <summary>
        /// Updated by Rajas on 15 MAY 2017
        /// </summary>
        /// <returns></returns>
        public double GetAppliedDaysForCO(DateTime FromDate, DateTime ToDate, int EmpId)
        {
            List<CompOffApplication> COApplicationList = WetosDB.CompOffApplications
                .Where(a => a.FromDate >= FromDate && a.FromDate <= ToDate && a.StatusId == 2 && a.EmployeeId == EmpId).ToList();

            double TotalAppliedDays = 0.0;
            foreach (CompOffApplication CoAppObj in COApplicationList)
            {

                // TotalAppliedDays = TotalAppliedDays + CoAppObj.Value;
            }

            return TotalAppliedDays;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="WetosDB"></param>
        /// <param name="id"></param>
        /// <param name="LeaveAsonDate"></param>
        /// <returns></returns>
        public static List<SP_LeaveTableData_Result> GetLeavedataNewLogic(WetosDBEntities WetosDB, int id, DateTime LeaveAsonDate)
        {

            //GET LEAVE BALANCE DATA
            List<SP_LeaveTableData_Result> LeaveBalanceList = new List<SP_LeaveTableData_Result>();

            try
            {

                #region CODE ADDED BY SHRADDHA ON 24 MAY 2017 TO GET GENERIC FUNCTION ACROSS APPLICATION FOR ALLOWED LEAVES


                // GET EMPLOYEE
                SP_EmployeeProfile_Result Employee = WetosDB.SP_EmployeeProfile(id).FirstOrDefault();

                // GET EMP GRP
                EmployeeGroupDetail EmployeeGroupDetailObj = WetosDB.EmployeeGroupDetails.Where(a => a.Employee.EmployeeId == Employee.EmployeeId).FirstOrDefault();


                // ADDED BY MSJ ON 08 MAY 2017 START
                //DateTime LeaveAsonDate = new DateTime(2017, 2, 1);
                //DateTime LeaveAsonDate = new DateTime(2017, 2, 15);
                //DateTime LeaveAsonDate = DateTime.Now;
                //DateTime LeaveAsonDate = new DateTime(2017, 3, 15);
                //DateTime LeaveAsonDate = new DateTime(2017, 3, 31);

                //bool MonthlyCreditFlag = true; // GLOBAL SETTING
                // Get FY
                WetosDB.FinancialYear FYLA = WetosDB.FinancialYears.Where(a => a.StartDate <= LeaveAsonDate && a.EndDate >= LeaveAsonDate
                    && a.Company.CompanyId == Employee.CompanyId && a.Branch.BranchId == Employee.BranchId).FirstOrDefault();


                // Updated by Rajas on 2 JUNE 2017
                if (FYLA == null)
                {
                    return LeaveBalanceList;
                }

                double OpenCarryForward = 0.0;
                double OpenOpenBal = 0.0;
                double LeaveAllowed = 0.0;
                double LeaveBalance = 0.0;
                double TotalLeaveSanctioned = 0;

                // GET LEAVE MASTER FROM EMPLOYEE ID
                List<LeaveMaster> EmpAllowedLeaveList = WetosDB.LeaveMasters.Where(a => a.Company.CompanyId == Employee.CompanyId
                    && a.BranchId == Employee.BranchId && a.EmployeeGroup.EmployeeGroupId == EmployeeGroupDetailObj.EmployeeGroup.EmployeeGroupId
                    && (a.MarkedAsDelete == 0 || a.MarkedAsDelete == null)).ToList(); // MODIFIED BY PUSHKAR ON 28 NOV 2017 FOR MARK AS DELETE CHECK CONDITION

                foreach (LeaveMaster LM in EmpAllowedLeaveList)
                {
                    // GET LIST OF LEAVE CREDIT FOR FY

                    var Opening = WetosDB.LeaveCredits.Where(a => a.EmployeeId == id && a.FinancialYearId == FYLA.FinancialYearId
                        && a.LeaveType.Trim().ToUpper() == LM.LeaveCode.Trim().ToUpper()).FirstOrDefault();

                    if (Opening != null) // UPDATE
                    {

                        // ADDED BY MSJ ON 29 MAY 2017 for MULTIPLE LEAVE TYPE START
                        //List<LeaveMaster> LeaveMasterList = WetosDB.LeaveMasters.Where(a => a.EmployeeGrpId == EmployeeGroupDetailObj.EmployeeGroupId
                        //    && a.CompanyId == Employee.CompanyId && a.BranchId == Employee.BranchId).ToList();

                        //LeaveMaster LeaveMasterObj = WetosDB.LeaveMasters.Where(a => a.EmployeeGrpId == EmployeeGroupDetailObj.EmployeeGroupId
                        //    && a.CompanyId == Employee.CompanyId && a.BranchId == Employee.BranchId
                        //    && a.LeaveCode.Trim().ToUpper() == Opening.LeaveType.Trim().ToUpper()).FirstOrDefault();

                        // Updated by Rajas on 1 AUGUST 2017 to handle null value START
                        OpenCarryForward = Opening.CarryForward == null ? 0 : Opening.CarryForward.Value;
                        OpenOpenBal = Opening.OpeningBalance == null ? 0 : Opening.OpeningBalance.Value;
                        // Updated by Rajas on 1 AUGUST 2017 to handle null value END

                        LeaveAllowed = OpenCarryForward;
                        LeaveBalance = OpenCarryForward;

                        DateTime ApplicableFrom = Opening.ApplicableEffectiveDate == null ? FYLA.StartDate : Opening.ApplicableEffectiveDate.Value;
                        //if (LeaveMasterObj != null)
                        //{
                        if (LM.LeaveCreditTypeId != null)
                        {
                            if (LM.LeaveCreditTypeId == 2)
                            {
                                int count = 1;
                                for (int i = 1; ; i++)
                                {
                                    if (ApplicableFrom.AddMonths(i) <= LeaveAsonDate)
                                    {
                                        count++;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                LeaveAllowed = Convert.ToDouble(OpenCarryForward + (count * LM.MaxNoOfLeavesAllowedInMonth));
                            }
                            else
                            {
                                //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 START
                                //LeaveAllowed = Convert.ToDouble(LM.MaxNoOfTimesAllowedInYear);
                                //LeaveAllowed = Convert.ToDouble(LM.NoOfDaysAllowedInYear);
                                //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 END

                            }
                        }
                        else
                        {
                            //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 START
                            //LeaveAllowed = Convert.ToDouble(LM.MaxNoOfTimesAllowedInYear);
                            //LeaveAllowed = Convert.ToDouble(LM.NoOfDaysAllowedInYear);
                            //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 END
                        }

                        //COMMENTED BELOW CODE AND USED NEW FUNCTION GetAppliedDaysTillMonthEnd BY SHRADDHA ON 24 JULY 2017 START
                        //TotalLeaveSanctioned = GetAppliedDays(WetosDB, ApplicableFrom, LeaveAsonDate, id);
                        TotalLeaveSanctioned = GetAppliedDaysTillMonthEnd(WetosDB, ApplicableFrom, LeaveAsonDate, id, LM.LeaveCode);
                        //COMMENTED BELOW CODE AND USED NEW FUNCTION GetAppliedDaysTillMonthEnd BY SHRADDHA ON 24 JULY 2017 END

                        LeaveBalance = LeaveAllowed - TotalLeaveSanctioned;

                        LeaveBalance EmpoyeeLeaveBalanceObj = WetosDB.LeaveBalances.Where(a => a.EmployeeId == id
                       && a.LeaveType.ToUpper().Trim() == LM.LeaveCode.ToUpper().Trim()).FirstOrDefault();

                        if (EmpoyeeLeaveBalanceObj != null)
                        {

                            SP_LeaveTableData_Result LeaveBalanceObj = new SP_LeaveTableData_Result();

                            // Updated by Rajas on 22 AUGUST 2017 START
                            LeaveBalanceObj.LeaveType = EmpoyeeLeaveBalanceObj.LeaveType.Trim();
                            LeaveBalanceObj.LeaveAllowed = Convert.ToDecimal(LeaveAllowed); // ActualDays.CurrentBalance;
                            LeaveBalanceObj.CurrentBalance = EmpoyeeLeaveBalanceObj.CurrentBalance.Value;  //LeaveBalance; // ActualDays.CurrentBalance;
                            LeaveBalanceObj.LeaveUsed = EmpoyeeLeaveBalanceObj.LeaveUsed; //TotalLeaveSanctioned; // ActualDays.LeaveUsed;
                            LeaveBalanceObj.OpeningBalance = OpenOpenBal; // Opening.OpeningBalance;
                            // Updated by Rajas on 22 AUGUST 2017 END

                            LeaveBalanceList.Add(LeaveBalanceObj);
                        }
                        //}
                    }
                    else
                    {
                        // ADD NEW LEAVE CREDIT
                        Opening = new LeaveCredit();

                        Opening.FinancialYearId = FYLA.FinancialYearId;

                        // GET LAST FY CLSOING BALANCAE
                        //DateTime LFYLeaveAsonDate = new DateTime(2017, 4, 1);
                        //DateTime LeaveAsonDate = new DateTime(2017, 3, 15);
                        //DateTime LeaveAsonDate = new DateTime(2017, 3, 31);

                        // Get FY
                        WetosDB.FinancialYear LFYFYLA = WetosDB.FinancialYears.Where(a => a.FinancialYearId == FYLA.PrevFYId).FirstOrDefault();

                        // Added by Rajas on 1 JUNE 2017 to handle null error
                        if (LFYFYLA != null)
                        {

                            double LFYOpenCarryForward = 0.0;
                            double LFYOpenOpenBal = 0.0;
                            double LFYLeaveAllowed = 0.0;
                            double LFYLeaveBalance = 0.0;
                            double LFYTotalLeaveSanctioned = 0;

                            // LASTS YEAR
                            //var LFYOpening = WetosDB.LeaveCredits.Where(a => a.EmployeeId == id && a.FinancialYearId == LFYFYLA.FinancialYearId).FirstOrDefault();
                            var LFYOpening = WetosDB.LeaveCredits.Where(a => a.EmployeeId == id
                                && a.FinancialYearId == LFYFYLA.FinancialYearId
                                && a.LeaveType.Trim() == LM.LeaveCode.Trim()).FirstOrDefault(); // ADDED && a.LeaveType.Trim() == LM.LeaveCode.Trim() on 02 JAN 2018

                            if (LFYOpening != null)// && LFYOpening.LeaveType.Trim() == LM.LeaveCode.Trim()) // COMMENTED BY MSJ ON 02 JAN 2018
                            {
                                // EmployeeGroupDetail EmployeeGroupDetailObj = WetosDB.EmployeeGroupDetails.Where(a => a.EmployeeId == Employee.EmployeeId).FirstOrDefault();

                                //LeaveMaster LeaveMasterObj = WetosDB.LeaveMasters.Where(a => a.EmployeeGrpId == EmployeeGroupDetailObj.EmployeeGroupId
                                //    && a.CompanyId == Employee.CompanyId && a.BranchId == Employee.BranchId
                                //    && a.LeaveCode.Trim().ToUpper() == LFYOpening.LeaveType.Trim().ToUpper()).FirstOrDefault();

                                //if (LeaveMasterObj != null)
                                //{

                                // Updated by Rajas on 1 AUGUST 2017 to handle null values START
                                LFYOpenCarryForward = LFYOpening.CarryForward == null ? 0 : LFYOpening.CarryForward.Value;
                                LFYOpenOpenBal = LFYOpening.OpeningBalance == null ? 0 : LFYOpening.OpeningBalance.Value;
                                // Updated by Rajas on 1 AUGUST 2017 to handle null values END

                                LFYLeaveAllowed = LFYOpenCarryForward;
                                LFYLeaveBalance = LFYOpenCarryForward;

                                DateTime LFYApplicableFrom = LFYOpening.ApplicableEffectiveDate == null ? FYLA.StartDate : LFYOpening.ApplicableEffectiveDate.Value;

                                if (LM.LeaveCreditTypeId != null)
                                {
                                    if (LM.LeaveCreditTypeId == 2)
                                    {
                                        int LFYcount = 1;
                                        for (int i = 1; ; i++)
                                        {
                                            if (LFYApplicableFrom.AddMonths(i) <= FYLA.EndDate)
                                            {
                                                LFYcount++;
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }

                                        LFYLeaveAllowed = Convert.ToDouble(LFYOpenCarryForward + (LFYcount * LM.MaxNoOfLeavesAllowedInMonth));
                                    }

                                    else
                                    {
                                        //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 START
                                        //LFYLeaveAllowed = Convert.ToDouble(LM.MaxNoOfTimesAllowedInYear);
                                        LFYLeaveAllowed = Convert.ToDouble(LM.NoOfDaysAllowedInYear);
                                        //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 END
                                    }
                                }
                                else
                                {
                                    //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 START
                                    //LFYLeaveAllowed = Convert.ToDouble(LM.MaxNoOfTimesAllowedInYear);
                                    LFYLeaveAllowed = Convert.ToDouble(LM.NoOfDaysAllowedInYear);
                                    //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 END
                                }

                                //COMMENTED BELOW CODE AND USED NEW FUNCTION GetAppliedDaysTillMonthEnd BY SHRADDHA ON 24 JULY 2017 START
                                //LFYTotalLeaveSanctioned = GetAppliedDays(WetosDB, LFYApplicableFrom, LFYFYLA.EndDate, id);
                                LFYTotalLeaveSanctioned = GetAppliedDaysTillMonthEnd(WetosDB, LFYApplicableFrom, LFYFYLA.EndDate, id, LM.LeaveCode);
                                //COMMENTED BELOW CODE AND USED NEW FUNCTION GetAppliedDaysTillMonthEnd BY SHRADDHA ON 24 JULY 2017 END


                                LFYLeaveBalance = LFYLeaveAllowed - LFYTotalLeaveSanctioned;

                                // ASSIGNED VALUE
                                Opening.CarryForward = LFYLeaveBalance;
                                Opening.CompanyId = LFYFYLA.Company.CompanyId;
                                Opening.BranchId = LFYFYLA.Branch.BranchId;
                                //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 START
                                //Opening.OpeningBalance = LFYLeaveBalance + LM.MaxNoOfTimesAllowedInYear; //
                                Opening.OpeningBalance = LFYLeaveBalance + LM.NoOfDaysAllowedInYear;
                                //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 END
                                //ADDED BY SHRADDHA ON 10 MAY 2017 START
                                Opening.LeaveType = LFYOpening.LeaveType;
                                Opening.ApplicableEffectiveDate = LFYApplicableFrom;
                                //ADDED BY SHRADDHA ON 10 MAY 2017 END
                                WetosDB.LeaveCredits.Add(Opening); // NEW CREDIT ENTRY FY
                                WetosDB.SaveChanges();

                                // Update leave balance
                                LeaveBalance LeaveBalances = WetosDB.LeaveBalances.Where(a => a.EmployeeId == id).FirstOrDefault();
                                LeaveBalances.PreviousBalance = 0;//LeaveBalances.CurrentBalance;
                                //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 START
                                //LeaveBalances.CurrentBalance = LFYLeaveBalance + LM.MaxNoOfTimesAllowedInYear;
                                LeaveBalances.CurrentBalance = LFYLeaveBalance;
                                //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 END
                                WetosDB.SaveChanges();

                                // get data fro new fy start
                                // Updated by Rajas on 1 AUGUST 2017 START to handle null value
                                OpenCarryForward = Opening.CarryForward == null ? 0 : Opening.CarryForward.Value;
                                OpenOpenBal = Opening.OpeningBalance == null ? 0 : Opening.OpeningBalance.Value;
                                // Updated by Rajas on 1 AUGUST 2017 START to handle null value

                                LeaveAllowed = OpenCarryForward;
                                LeaveBalance = OpenCarryForward;

                                DateTime ApplicableFrom = Opening.ApplicableEffectiveDate == null ? FYLA.StartDate : Opening.ApplicableEffectiveDate.Value;


                                if (LM.LeaveCreditTypeId != null)
                                {
                                    if (LM.LeaveCreditTypeId == 2)
                                    {
                                        int count = 1;
                                        for (int i = 1; ; i++)
                                        {
                                            if (ApplicableFrom.AddMonths(i) <= LeaveAsonDate)
                                            {
                                                count++;
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }

                                        LeaveAllowed = Convert.ToDouble(OpenCarryForward + (count * LM.MaxNoOfLeavesAllowedInMonth));
                                    }
                                    else
                                    {
                                        //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 START
                                        //LeaveAllowed = Convert.ToDouble(LM.MaxNoOfTimesAllowedInYear);
                                        LeaveAllowed = Convert.ToDouble(LM.NoOfDaysAllowedInYear);
                                        //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 END

                                    }
                                }
                                else
                                {

                                    //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 START
                                    //LeaveAllowed = Convert.ToDouble(LM.MaxNoOfTimesAllowedInYear);
                                    LeaveAllowed = Convert.ToDouble(LM.NoOfDaysAllowedInYear);
                                    //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 END

                                }

                                //COMMENTED BELOW CODE AND USED NEW FUNCTION GetAppliedDaysTillMonthEnd BY SHRADDHA ON 24 JULY 2017 START
                                //TotalLeaveSanctioned = GetAppliedDays(WetosDB, ApplicableFrom, LeaveAsonDate, id);
                                TotalLeaveSanctioned = GetAppliedDaysTillMonthEnd(WetosDB, ApplicableFrom, LeaveAsonDate, id, LM.LeaveCode);
                                //COMMENTED BELOW CODE AND USED NEW FUNCTION GetAppliedDaysTillMonthEnd BY SHRADDHA ON 24 JULY 2017 END

                                LeaveBalance = LeaveAllowed - TotalLeaveSanctioned;
                                //}
                                // get data fro new fy END

                                LeaveBalance EmpoyeeLeaveBalanceObj = WetosDB.LeaveBalances.Where(a => a.EmployeeId == id
                       && a.LeaveType.ToUpper().Trim() == LM.LeaveCode.ToUpper().Trim()).FirstOrDefault();

                                if (EmpoyeeLeaveBalanceObj != null)
                                {

                                    SP_LeaveTableData_Result LeaveBalanceObj = new SP_LeaveTableData_Result();

                                    // Updated by Rajas on 22 AUGUST 2017 START
                                    LeaveBalanceObj.LeaveType = EmpoyeeLeaveBalanceObj.LeaveType.Trim();
                                    LeaveBalanceObj.LeaveAllowed = Convert.ToDecimal(LeaveAllowed); // ActualDays.CurrentBalance;
                                    LeaveBalanceObj.CurrentBalance = EmpoyeeLeaveBalanceObj.CurrentBalance.Value;  //LeaveBalance; // ActualDays.CurrentBalance;
                                    LeaveBalanceObj.LeaveUsed = EmpoyeeLeaveBalanceObj.LeaveUsed; //TotalLeaveSanctioned; // ActualDays.LeaveUsed;
                                    LeaveBalanceObj.OpeningBalance = OpenOpenBal; // Opening.OpeningBalance;
                                    // Updated by Rajas on 22 AUGUST 2017 END

                                    LeaveBalanceList.Add(LeaveBalanceObj);
                                }
                            }
                        }
                        else
                        {
                            // Error is handled 

                        }
                    }


                    // ADDED BY MSJ ON 08 MAY 2017 END


                    ////LeaveBalance EmpoyeeLeaveBalanceObj = WetosDB.LeaveBalances.Where(a => a.EmployeeId == id
                    ////    && a.LeaveType.ToUpper().Trim() == LM.LeaveCode.ToUpper().Trim()).FirstOrDefault();

                    ////if (EmpoyeeLeaveBalanceObj != null)
                    ////{

                    ////    SP_LeaveTableData_Result LeaveBalanceObj = new SP_LeaveTableData_Result();

                    ////    // Updated by Rajas on 22 AUGUST 2017 START
                    ////    LeaveBalanceObj.LeaveType = EmpoyeeLeaveBalanceObj.LeaveType.Trim();
                    ////    LeaveBalanceObj.LeaveAllowed = Convert.ToDecimal(LeaveAllowed); // ActualDays.CurrentBalance;
                    ////    LeaveBalanceObj.CurrentBalance = EmpoyeeLeaveBalanceObj.CurrentBalance.Value;  //LeaveBalance; // ActualDays.CurrentBalance;
                    ////    LeaveBalanceObj.LeaveUsed = EmpoyeeLeaveBalanceObj.LeaveUsed; //TotalLeaveSanctioned; // ActualDays.LeaveUsed;
                    ////    LeaveBalanceObj.OpeningBalance = OpenOpenBal; // Opening.OpeningBalance;
                    ////    // Updated by Rajas on 22 AUGUST 2017 END

                    ////    LeaveBalanceList.Add(LeaveBalanceObj);
                    ////}
                }

                return LeaveBalanceList;


                #endregion
            }
            catch
            {
                return LeaveBalanceList;
            }
        }

        /// <summary>
        /// Get Leave balance during leave application
        /// Added by Rajas on 5 SEP 2017
        /// </summary>
        /// <param name="WetosDB"></param>
        /// <param name="id"></param>
        /// <param name="LeaveAsonDate"></param>
        /// <returns></returns>
        public static double GetAvailableLeaveBalance(WetosDBEntities WetosDB, int id, ref LeaveModel LeaveModelObj)
        {
            try
            {
                #region CODE ADDED BY SHRADDHA ON 24 MAY 2017 TO GET GENERIC FUNCTION ACROSS APPLICATION FOR ALLOWED LEAVES

                // GET EMPLOYEE
                SP_EmployeeProfile_Result Employee = WetosDB.SP_EmployeeProfile(id).FirstOrDefault();

                // GET EMP GRP
                EmployeeGroupDetail EmployeeGroupDetailObj = WetosDB.EmployeeGroupDetails.Where(a => a.Employee.EmployeeId == Employee.EmployeeId).FirstOrDefault();


                // ADDED BY MSJ ON 08 MAY 2017 START
                //DateTime LeaveAsonDate = new DateTime(2017, 2, 1);
                //DateTime LeaveAsonDate = new DateTime(2017, 2, 15);
                //DateTime LeaveAsonDate = DateTime.Now;
                //DateTime LeaveAsonDate = new DateTime(2017, 3, 15);
                //DateTime LeaveAsonDate = new DateTime(2017, 3, 31);

                DateTime LeaveAsonDate = LeaveModelObj.ToDate;

                // Get FY
                WetosDB.FinancialYear FYLA = WetosDB.FinancialYears.Where(a => a.StartDate <= LeaveAsonDate && a.EndDate >= LeaveAsonDate
                    && a.Company.CompanyId == Employee.CompanyId && a.Branch.BranchId == Employee.BranchId).FirstOrDefault();


                // Updated by Rajas on 2 JUNE 2017
                if (FYLA == null)
                {
                    return 0; //LeaveBalanceList;
                }

                double OpenCarryForward = 0.0;
                double OpenOpenBal = 0.0;
                double LeaveAllowed = 0.0;
                double LeaveBalance = 0.0;
                double TotalLeaveSanctioned = 0;

                // Updated by Rajas on 5 SEP 2017 START
                int LeaveId = LeaveModelObj.LeaveId;

                LeaveMaster LM = WetosDB.LeaveMasters.Where(a => a.Company.CompanyId == Employee.CompanyId
                    && a.BranchId == Employee.BranchId && a.EmployeeGroup.EmployeeGroupId == EmployeeGroupDetailObj.EmployeeGroup.EmployeeGroupId
                    && a.LeaveId == LeaveId).FirstOrDefault();

                LeaveModelObj.LeaveName = LM.LeaveCode;
                // Updated by Rajas on 5 SEP 2017 END

                // foreach (LeaveMaster LM in EmpAllowedLeaveList)
                {
                    // GET LIST OF LEAVE CREDIT FOR FY

                    var Opening = WetosDB.LeaveCredits.Where(a => a.EmployeeId == id && a.FinancialYearId == FYLA.FinancialYearId
                        && a.LeaveType.Trim().ToUpper() == LM.LeaveCode.Trim().ToUpper()).FirstOrDefault();

                    if (Opening != null) // UPDATE
                    {

                        // ADDED BY MSJ ON 29 MAY 2017 for MULTIPLE LEAVE TYPE START
                        //List<LeaveMaster> LeaveMasterList = WetosDB.LeaveMasters.Where(a => a.EmployeeGrpId == EmployeeGroupDetailObj.EmployeeGroupId
                        //    && a.CompanyId == Employee.CompanyId && a.BranchId == Employee.BranchId).ToList();

                        //LeaveMaster LeaveMasterObj = WetosDB.LeaveMasters.Where(a => a.EmployeeGrpId == EmployeeGroupDetailObj.EmployeeGroupId
                        //    && a.CompanyId == Employee.CompanyId && a.BranchId == Employee.BranchId
                        //    && a.LeaveCode.Trim().ToUpper() == Opening.LeaveType.Trim().ToUpper()).FirstOrDefault();

                        // Updated by Rajas on 1 AUGUST 2017 to handle null value START
                        OpenCarryForward = Opening.CarryForward == null ? 0 : Opening.CarryForward.Value;
                        OpenOpenBal = Opening.OpeningBalance == null ? 0 : Opening.OpeningBalance.Value;
                        // Updated by Rajas on 1 AUGUST 2017 to handle null value END

                        LeaveAllowed = OpenCarryForward;
                        LeaveBalance = OpenCarryForward;

                        DateTime ApplicableFrom = Opening.ApplicableEffectiveDate == null ? FYLA.StartDate : Opening.ApplicableEffectiveDate.Value;
                        //if (LeaveMasterObj != null)
                        //{
                        if (LM.LeaveCreditTypeId != null)
                        {
                            if (LM.LeaveCreditTypeId == 2)
                            {
                                int count = 1;
                                for (int i = 1; ; i++)
                                {
                                    if (ApplicableFrom.AddMonths(i) <= LeaveAsonDate)
                                    {
                                        count++;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                LeaveAllowed = Convert.ToDouble(OpenCarryForward + (count * LM.MaxNoOfLeavesAllowedInMonth));
                            }
                            else
                            {
                                //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 START
                                //LeaveAllowed = Convert.ToDouble(LM.MaxNoOfTimesAllowedInYear);
                                //LeaveAllowed = Convert.ToDouble(LM.NoOfDaysAllowedInYear);
                                //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 END

                                LeaveAllowed = Convert.ToDouble(LM.NoOfDaysAllowedInYear);

                            }
                        }
                        else
                        {
                            //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 START
                            //LeaveAllowed = Convert.ToDouble(LM.MaxNoOfTimesAllowedInYear);
                            //LeaveAllowed = Convert.ToDouble(LM.NoOfDaysAllowedInYear);
                            //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 END

                            LeaveAllowed = Convert.ToDouble(LM.NoOfDaysAllowedInYear);
                        }

                        //COMMENTED BELOW CODE AND USED NEW FUNCTION GetAppliedDaysTillMonthEnd BY SHRADDHA ON 24 JULY 2017 START
                        //TotalLeaveSanctioned = GetAppliedDays(WetosDB, ApplicableFrom, LeaveAsonDate, id);
                        TotalLeaveSanctioned = GetAppliedDaysTillMonthEnd(WetosDB, ApplicableFrom, LeaveAsonDate, id, LM.LeaveCode);
                        //COMMENTED BELOW CODE AND USED NEW FUNCTION GetAppliedDaysTillMonthEnd BY SHRADDHA ON 24 JULY 2017 END

                        LeaveBalance = LeaveAllowed - TotalLeaveSanctioned;
                        //}
                    }
                    else
                    {
                        // ADD NEW LEAVE CREDIT
                        Opening = new LeaveCredit();

                        Opening.FinancialYearId = FYLA.FinancialYearId;

                        // GET LAST FY CLSOING BALANCAE
                        //DateTime LFYLeaveAsonDate = new DateTime(2017, 4, 1);
                        //DateTime LeaveAsonDate = new DateTime(2017, 3, 15);
                        //DateTime LeaveAsonDate = new DateTime(2017, 3, 31);

                        // Get FY
                        WetosDB.FinancialYear LFYFYLA = WetosDB.FinancialYears.Where(a => a.FinancialYearId == FYLA.PrevFYId).FirstOrDefault();

                        // Added by Rajas on 1 JUNE 2017 to handle null error
                        if (LFYFYLA != null)
                        {

                            double LFYOpenCarryForward = 0.0;
                            double LFYOpenOpenBal = 0.0;
                            double LFYLeaveAllowed = 0.0;
                            double LFYLeaveBalance = 0.0;
                            double LFYTotalLeaveSanctioned = 0;

                            // LASTS YEAR
                            var LFYOpening = WetosDB.LeaveCredits.Where(a => a.EmployeeId == id && a.FinancialYearId == LFYFYLA.FinancialYearId).FirstOrDefault();

                            if (LFYOpening != null && LFYOpening.LeaveType.Trim() == LM.LeaveCode.Trim())
                            {
                                // EmployeeGroupDetail EmployeeGroupDetailObj = WetosDB.EmployeeGroupDetails.Where(a => a.EmployeeId == Employee.EmployeeId).FirstOrDefault();

                                //LeaveMaster LeaveMasterObj = WetosDB.LeaveMasters.Where(a => a.EmployeeGrpId == EmployeeGroupDetailObj.EmployeeGroupId
                                //    && a.CompanyId == Employee.CompanyId && a.BranchId == Employee.BranchId
                                //    && a.LeaveCode.Trim().ToUpper() == LFYOpening.LeaveType.Trim().ToUpper()).FirstOrDefault();

                                //if (LeaveMasterObj != null)
                                //{

                                // Updated by Rajas on 1 AUGUST 2017 to handle null values START
                                LFYOpenCarryForward = LFYOpening.CarryForward == null ? 0 : LFYOpening.CarryForward.Value;
                                LFYOpenOpenBal = LFYOpening.OpeningBalance == null ? 0 : LFYOpening.OpeningBalance.Value;
                                // Updated by Rajas on 1 AUGUST 2017 to handle null values END

                                LFYLeaveAllowed = LFYOpenCarryForward;
                                LFYLeaveBalance = LFYOpenCarryForward;

                                DateTime LFYApplicableFrom = LFYOpening.ApplicableEffectiveDate == null ? FYLA.StartDate : LFYOpening.ApplicableEffectiveDate.Value;

                                if (LM.LeaveCreditTypeId != null)
                                {
                                    if (LM.LeaveCreditTypeId == 2)
                                    {
                                        int LFYcount = 1;
                                        for (int i = 1; ; i++)
                                        {
                                            if (LFYApplicableFrom.AddMonths(i) <= FYLA.EndDate)
                                            {
                                                LFYcount++;
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }

                                        LFYLeaveAllowed = Convert.ToDouble(LFYOpenCarryForward + (LFYcount * LM.MaxNoOfLeavesAllowedInMonth));
                                    }

                                    else
                                    {
                                        //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 START
                                        //LFYLeaveAllowed = Convert.ToDouble(LM.MaxNoOfTimesAllowedInYear);
                                        LFYLeaveAllowed = Convert.ToDouble(LM.NoOfDaysAllowedInYear);
                                        //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 END
                                    }
                                }
                                else
                                {
                                    //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 START
                                    //LFYLeaveAllowed = Convert.ToDouble(LM.MaxNoOfTimesAllowedInYear);
                                    LFYLeaveAllowed = Convert.ToDouble(LM.NoOfDaysAllowedInYear);
                                    //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 END
                                }

                                //COMMENTED BELOW CODE AND USED NEW FUNCTION GetAppliedDaysTillMonthEnd BY SHRADDHA ON 24 JULY 2017 START
                                //LFYTotalLeaveSanctioned = GetAppliedDays(WetosDB, LFYApplicableFrom, LFYFYLA.EndDate, id);
                                LFYTotalLeaveSanctioned = GetAppliedDaysTillMonthEnd(WetosDB, LFYApplicableFrom, LFYFYLA.EndDate, id, LM.LeaveCode);
                                //COMMENTED BELOW CODE AND USED NEW FUNCTION GetAppliedDaysTillMonthEnd BY SHRADDHA ON 24 JULY 2017 END


                                LFYLeaveBalance = LFYLeaveAllowed - LFYTotalLeaveSanctioned;

                                // ASSIGNED VALUE
                                Opening.CarryForward = LFYLeaveBalance;
                                Opening.CompanyId = LFYFYLA.Company.CompanyId;
                                Opening.BranchId = LFYFYLA.Branch.BranchId;
                                //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 START
                                //Opening.OpeningBalance = LFYLeaveBalance + LM.MaxNoOfTimesAllowedInYear; //
                                Opening.OpeningBalance = LFYLeaveBalance + LM.NoOfDaysAllowedInYear;
                                //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 END
                                //ADDED BY SHRADDHA ON 10 MAY 2017 START
                                Opening.LeaveType = LFYOpening.LeaveType;
                                Opening.ApplicableEffectiveDate = LFYApplicableFrom;
                                //ADDED BY SHRADDHA ON 10 MAY 2017 END
                                WetosDB.LeaveCredits.Add(Opening); // NEW CREDIT ENTRY FY
                                WetosDB.SaveChanges();

                                // Update leave balance
                                LeaveBalance LeaveBalances = WetosDB.LeaveBalances.Where(a => a.EmployeeId == id).FirstOrDefault();
                                LeaveBalances.PreviousBalance = 0;//LeaveBalances.CurrentBalance;
                                //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 START
                                //LeaveBalances.CurrentBalance = LFYLeaveBalance + LM.MaxNoOfTimesAllowedInYear;
                                LeaveBalances.CurrentBalance = LFYLeaveBalance;
                                //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 END
                                WetosDB.SaveChanges();

                                // get data fro new fy start
                                // Updated by Rajas on 1 AUGUST 2017 START to handle null value
                                OpenCarryForward = Opening.CarryForward == null ? 0 : Opening.CarryForward.Value;
                                OpenOpenBal = Opening.OpeningBalance == null ? 0 : Opening.OpeningBalance.Value;
                                // Updated by Rajas on 1 AUGUST 2017 START to handle null value

                                LeaveAllowed = OpenCarryForward;
                                LeaveBalance = OpenCarryForward;

                                DateTime ApplicableFrom = Opening.ApplicableEffectiveDate == null ? FYLA.StartDate : Opening.ApplicableEffectiveDate.Value;


                                if (LM.LeaveCreditTypeId != null)
                                {
                                    if (LM.LeaveCreditTypeId == 2)
                                    {
                                        int count = 1;
                                        for (int i = 1; ; i++)
                                        {
                                            if (ApplicableFrom.AddMonths(i) <= LeaveAsonDate)
                                            {
                                                count++;
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }

                                        LeaveAllowed = Convert.ToDouble(OpenCarryForward + (count * LM.MaxNoOfLeavesAllowedInMonth));
                                    }
                                    else
                                    {
                                        //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 START
                                        //LeaveAllowed = Convert.ToDouble(LM.MaxNoOfTimesAllowedInYear);
                                        LeaveAllowed = Convert.ToDouble(LM.NoOfDaysAllowedInYear);
                                        //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 END

                                    }
                                }
                                else
                                {

                                    //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 START
                                    //LeaveAllowed = Convert.ToDouble(LM.MaxNoOfTimesAllowedInYear);
                                    LeaveAllowed = Convert.ToDouble(LM.NoOfDaysAllowedInYear);
                                    //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 END

                                }

                                //COMMENTED BELOW CODE AND USED NEW FUNCTION GetAppliedDaysTillMonthEnd BY SHRADDHA ON 24 JULY 2017 START
                                //TotalLeaveSanctioned = GetAppliedDays(WetosDB, ApplicableFrom, LeaveAsonDate, id);
                                TotalLeaveSanctioned = GetAppliedDaysTillMonthEnd(WetosDB, ApplicableFrom, LeaveAsonDate, id, LM.LeaveCode);
                                //COMMENTED BELOW CODE AND USED NEW FUNCTION GetAppliedDaysTillMonthEnd BY SHRADDHA ON 24 JULY 2017 END

                                LeaveBalance = LeaveAllowed - TotalLeaveSanctioned;
                                //}
                                // get data fro new fy END
                            }
                        }
                        else
                        {
                            // Error is handled 

                        }
                    }


                    // ADDED BY MSJ ON 08 MAY 2017 END


                    LeaveBalance EmpoyeeLeaveBalanceObj = WetosDB.LeaveBalances.Where(a => a.EmployeeId == id
                        && a.LeaveType.ToUpper().Trim() == LM.LeaveCode.ToUpper().Trim()).FirstOrDefault();

                    if (EmpoyeeLeaveBalanceObj != null)
                    {
                        LeaveBalance = EmpoyeeLeaveBalanceObj.CurrentBalance.Value;
                        //SP_LeaveTableData_Result LeaveBalanceObj = new SP_LeaveTableData_Result();

                        //// Updated by Rajas on 22 AUGUST 2017 START
                        //LeaveBalanceObj.LeaveType = EmpoyeeLeaveBalanceObj.LeaveType.Trim();
                        //LeaveBalanceObj.LeaveAllowed = Convert.ToDecimal(LeaveAllowed); // ActualDays.CurrentBalance;
                        //LeaveBalanceObj.CurrentBalance = EmpoyeeLeaveBalanceObj.CurrentBalance.Value;  //LeaveBalance; // ActualDays.CurrentBalance;
                        //LeaveBalanceObj.LeaveUsed = EmpoyeeLeaveBalanceObj.LeaveUsed; //TotalLeaveSanctioned; // ActualDays.LeaveUsed;
                        //LeaveBalanceObj.OpeningBalance = OpenOpenBal; // Opening.OpeningBalance;
                        //// Updated by Rajas on 22 AUGUST 2017 END

                        //LeaveBalanceList.Add(LeaveBalanceObj);
                    }
                }

                return LeaveBalance;


                #endregion
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// NewLy Created By Shraddha on 26 JULY 2017
        /// For Getting Correct Leave Data on Report
        /// <param name="WetosDB"></param>
        /// <param name="id"></param>
        /// <param name="LeaveAsonDate"></param>
        /// <returns></returns>
        public static List<SP_LeaveTableDataNew_Result> GetLeaveDataOnReport(WetosDBEntities WetosDB, int id, DateTime LeaveAsonDate, string LeaveType)
        {

            #region CODE ADDED BY SHRADDHA ON 24 MAY 2017 TO GET GENERIC FUNCTION ACROSS APPLICATION FOR ALLOWED LEAVES


            // GET EMPLOYEE
            SP_EmployeeProfile_Result Employee = WetosDB.SP_EmployeeProfile(id).FirstOrDefault();

            // GET EMP GRP
            //EmployeeGroupDetail EmployeeGroupDetailObj = WetosDB.EmployeeGroupDetails.Where(a => a.EmployeeId == Employee.EmployeeId).FirstOrDefault();


            // ADDED BY MSJ ON 08 MAY 2017 START
            //DateTime LeaveAsonDate = new DateTime(2017, 2, 1);
            //DateTime LeaveAsonDate = new DateTime(2017, 2, 15);
            //DateTime LeaveAsonDate = DateTime.Now;
            //DateTime LeaveAsonDate = new DateTime(2017, 3, 15);
            //DateTime LeaveAsonDate = new DateTime(2017, 3, 31);

            //bool MonthlyCreditFlag = true; // GLOBAL SETTING
            // Get FY
            WetosDB.FinancialYear FYLA = WetosDB.FinancialYears.Where(a => a.StartDate <= LeaveAsonDate && a.EndDate >= LeaveAsonDate
                && a.Company.CompanyId == Employee.CompanyId && a.Branch.BranchId == Employee.BranchId).FirstOrDefault();

            //GET LEAVE BALANCE DATA
            List<SP_LeaveTableDataNew_Result> LeaveBalanceList = new List<SP_LeaveTableDataNew_Result>();

            // Updated by Rajas on 2 JUNE 2017
            if (FYLA == null)
            {
                return LeaveBalanceList;
            }

            double OpenCarryForward = 0.0;
            double OpenOpenBal = 0.0;
            double LeaveAllowed = 0.0;
            double LeaveBalance = 0.0;
            double TotalLeaveSanctioned = 0;
            double LeaveBalanceForCurrentMonth = 0.0; //ADDED BY SHRADDHA ON 25 JULY 2017
            double TotalLeaveSanctionedForCurrentMonth = 0; //ADDED BY SHRADDHA ON 25 JULY 2017
            // GET LEAVE MASTER FROM EMPLOYEE ID
            //List<LeaveMaster> EmpAllowedLeaveList = WetosDB.LeaveMasters.Where(a => a.Company.CompanyId == Employee.CompanyId
            //    && a.BranchId == Employee.BranchId && a.EmployeeGroup.EmployeeGroupId == EmployeeGroupDetailObj.EmployeeGroupId).ToList();
            List<LeaveMaster> EmpAllowedLeaveList = WetosDB.LeaveMasters.Where(a => a.Company.CompanyId == Employee.CompanyId
                && a.BranchId == Employee.BranchId && a.LeaveCode.Trim().ToUpper() == LeaveType.Trim().ToUpper()).ToList();

            foreach (LeaveMaster LM in EmpAllowedLeaveList)
            {
                // GET LIST OF LEAVE CREDIT FOR FY

                var Opening = WetosDB.LeaveCredits.Where(a => a.EmployeeId == id && a.FinancialYearId == FYLA.FinancialYearId
                    && a.LeaveType.Trim().ToUpper() == LM.LeaveCode.Trim().ToUpper()).FirstOrDefault();

                if (Opening != null) // UPDATE
                {

                    // ADDED BY MSJ ON 29 MAY 2017 for MULTIPLE LEAVE TYPE START
                    //List<LeaveMaster> LeaveMasterList = WetosDB.LeaveMasters.Where(a => a.EmployeeGrpId == EmployeeGroupDetailObj.EmployeeGroupId
                    //    && a.CompanyId == Employee.CompanyId && a.BranchId == Employee.BranchId).ToList();

                    //LeaveMaster LeaveMasterObj = WetosDB.LeaveMasters.Where(a => a.EmployeeGrpId == EmployeeGroupDetailObj.EmployeeGroupId
                    //    && a.CompanyId == Employee.CompanyId && a.BranchId == Employee.BranchId
                    //    && a.LeaveCode.Trim().ToUpper() == Opening.LeaveType.Trim().ToUpper()).FirstOrDefault();

                    // Updated by Rajas on 1 AUGUST 2017 to handle null values START
                    OpenCarryForward = Opening.CarryForward == null ? 0 : Opening.CarryForward.Value;
                    OpenOpenBal = Opening.OpeningBalance == null ? 0 : Opening.OpeningBalance.Value;
                    // Updated by Rajas on 1 AUGUST 2017 to handle null values END

                    LeaveAllowed = OpenCarryForward;
                    LeaveBalance = OpenCarryForward;

                    DateTime ApplicableFrom = Opening.ApplicableEffectiveDate == null ? FYLA.StartDate : Opening.ApplicableEffectiveDate.Value;
                    //if (LeaveMasterObj != null)
                    //{
                    if (LM.LeaveCreditTypeId != null)
                    {
                        if (LM.LeaveCreditTypeId == 2)
                        {
                            int count = 1;
                            for (int i = 1; ; i++)
                            {
                                if (ApplicableFrom.AddMonths(i) <= LeaveAsonDate)
                                {
                                    count++;
                                }
                                else
                                {
                                    break;
                                }
                            }

                            LeaveAllowed = Convert.ToDouble(OpenCarryForward + (count * LM.MaxNoOfLeavesAllowedInMonth));
                        }
                        else
                        {
                            //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 START
                            //LeaveAllowed = Convert.ToDouble(LM.MaxNoOfTimesAllowedInYear);
                            //LeaveAllowed = Convert.ToDouble(LM.NoOfDaysAllowedInYear);
                            //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 END

                        }
                    }
                    else
                    {
                        //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 START
                        //LeaveAllowed = Convert.ToDouble(LM.MaxNoOfTimesAllowedInYear);
                        //LeaveAllowed = Convert.ToDouble(LM.NoOfDaysAllowedInYear);
                        //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 END
                    }

                    //COMMENTED BELOW CODE AND USED NEW FUNCTION GetAppliedDaysTillMonthEnd BY SHRADDHA ON 24 JULY 2017 START
                    //TotalLeaveSanctioned = GetAppliedDays(WetosDB, ApplicableFrom, LeaveAsonDate, id);
                    DateTime FromDate = new DateTime(LeaveAsonDate.Year, LeaveAsonDate.Month, 01); //ADDED BY SHRADDHA ON 25 JULY 2017
                    TotalLeaveSanctioned = GetAppliedDaysTillMonthEnd(WetosDB, ApplicableFrom, LeaveAsonDate, id, LM.LeaveCode);
                    TotalLeaveSanctionedForCurrentMonth = GetAppliedDaysTillMonthEnd(WetosDB, FromDate, LeaveAsonDate, id, LM.LeaveCode); //ADDED BY SHRADDHA ON 25 JULY 2017
                    //COMMENTED BELOW CODE AND USED NEW FUNCTION GetAppliedDaysTillMonthEnd BY SHRADDHA ON 24 JULY 2017 END

                    LeaveBalance = LeaveAllowed - TotalLeaveSanctioned; //ADDED BY SHRADDHA ON 25 JULY 2017
                    LeaveBalanceForCurrentMonth = LeaveAllowed - TotalLeaveSanctionedForCurrentMonth; //ADDED BY SHRADDHA ON 25 JULY 2017
                    //}
                }
                else
                {
                    // ADD NEW LEAVE CREDIT
                    Opening = new LeaveCredit();

                    Opening.FinancialYearId = FYLA.FinancialYearId;

                    // GET LAST FY CLSOING BALANCAE
                    //DateTime LFYLeaveAsonDate = new DateTime(2017, 4, 1);
                    //DateTime LeaveAsonDate = new DateTime(2017, 3, 15);
                    //DateTime LeaveAsonDate = new DateTime(2017, 3, 31);

                    // Get FY
                    WetosDB.FinancialYear LFYFYLA = WetosDB.FinancialYears.Where(a => a.FinancialYearId == FYLA.PrevFYId).FirstOrDefault();

                    // Added by Rajas on 1 JUNE 2017 to handle null error
                    if (LFYFYLA != null)
                    {

                        double LFYOpenCarryForward = 0.0;
                        double LFYOpenOpenBal = 0.0;
                        double LFYLeaveAllowed = 0.0;
                        double LFYLeaveBalance = 0.0;
                        double LFYTotalLeaveSanctioned = 0;
                        double LFYLeaveBalanceforCurrentMonth = 0.0; //ADDED BY SHRADDHA ON 25 JULY 2017
                        double LFYTotalLeaveSanctionedforCurrentMonth = 0; //ADDED BY SHRADDHA ON 25 JULY 2017
                        // LASTS YEAR
                        var LFYOpening = WetosDB.LeaveCredits.Where(a => a.EmployeeId == id && a.FinancialYearId == LFYFYLA.FinancialYearId).FirstOrDefault();

                        if (LFYOpening != null && LFYOpening.LeaveType.Trim() == LM.LeaveCode.Trim())
                        {
                            // EmployeeGroupDetail EmployeeGroupDetailObj = WetosDB.EmployeeGroupDetails.Where(a => a.EmployeeId == Employee.EmployeeId).FirstOrDefault();

                            //LeaveMaster LeaveMasterObj = WetosDB.LeaveMasters.Where(a => a.EmployeeGrpId == EmployeeGroupDetailObj.EmployeeGroupId
                            //    && a.CompanyId == Employee.CompanyId && a.BranchId == Employee.BranchId
                            //    && a.LeaveCode.Trim().ToUpper() == LFYOpening.LeaveType.Trim().ToUpper()).FirstOrDefault();

                            //if (LeaveMasterObj != null)
                            //{

                            // Updated by Rajas on 1 AUGUST 2017 to handle null value START
                            LFYOpenCarryForward = LFYOpening.CarryForward == null ? 0 : LFYOpening.CarryForward.Value;
                            LFYOpenOpenBal = LFYOpening.OpeningBalance == null ? 0 : LFYOpening.OpeningBalance.Value;
                            // Updated by Rajas on 1 AUGUST 2017 to handle null value END

                            LFYLeaveAllowed = LFYOpenCarryForward;
                            LFYLeaveBalance = LFYOpenCarryForward;

                            DateTime LFYApplicableFrom = LFYOpening.ApplicableEffectiveDate == null ? FYLA.StartDate : LFYOpening.ApplicableEffectiveDate.Value;

                            if (LM.LeaveCreditTypeId != null)
                            {
                                if (LM.LeaveCreditTypeId == 2)
                                {
                                    int LFYcount = 1;
                                    for (int i = 1; ; i++)
                                    {
                                        if (LFYApplicableFrom.AddMonths(i) <= FYLA.EndDate)
                                        {
                                            LFYcount++;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }

                                    LFYLeaveAllowed = Convert.ToDouble(LFYOpenCarryForward + (LFYcount * LM.MaxNoOfLeavesAllowedInMonth));
                                }

                                else
                                {
                                    //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 START
                                    //LFYLeaveAllowed = Convert.ToDouble(LM.MaxNoOfTimesAllowedInYear);
                                    LFYLeaveAllowed = Convert.ToDouble(LM.NoOfDaysAllowedInYear);
                                    //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 END
                                }
                            }
                            else
                            {
                                //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 START
                                //LFYLeaveAllowed = Convert.ToDouble(LM.MaxNoOfTimesAllowedInYear);
                                LFYLeaveAllowed = Convert.ToDouble(LM.NoOfDaysAllowedInYear);
                                //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 END
                            }




                            //COMMENTED BELOW CODE AND USED NEW FUNCTION GetAppliedDaysTillMonthEnd BY SHRADDHA ON 24 JULY 2017 START
                            //LFYTotalLeaveSanctioned = GetAppliedDays(WetosDB, LFYApplicableFrom, LFYFYLA.EndDate, id);
                            DateTime FromDate = new DateTime(LFYFYLA.EndDate.Year, LFYFYLA.EndDate.Month, 01);
                            LFYTotalLeaveSanctioned = GetAppliedDaysTillMonthEnd(WetosDB, LFYApplicableFrom, LFYFYLA.EndDate, id, LM.LeaveCode);
                            LFYTotalLeaveSanctionedforCurrentMonth = GetAppliedDaysTillMonthEnd(WetosDB, FromDate, LFYFYLA.EndDate, id, LM.LeaveCode); //ADDED BY SHRADDHA ON 25 JULY 2017
                            //COMMENTED BELOW CODE AND USED NEW FUNCTION GetAppliedDaysTillMonthEnd BY SHRADDHA ON 24 JULY 2017 END


                            LFYLeaveBalance = LFYLeaveAllowed - LFYTotalLeaveSanctioned;
                            LFYLeaveBalanceforCurrentMonth = LFYLeaveAllowed - LFYTotalLeaveSanctionedforCurrentMonth; //ADDED BY SHRADDHA ON 25 JULY 2017

                            // ASSIGNED VALUE
                            Opening.CarryForward = LFYLeaveBalance;
                            Opening.CompanyId = LFYFYLA.Company.CompanyId;
                            Opening.BranchId = LFYFYLA.Branch.BranchId;
                            //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 START
                            //Opening.OpeningBalance = LFYLeaveBalance + LM.MaxNoOfTimesAllowedInYear; //
                            Opening.OpeningBalance = LFYLeaveBalance + LM.NoOfDaysAllowedInYear;
                            //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 END
                            //ADDED BY SHRADDHA ON 10 MAY 2017 START
                            Opening.LeaveType = LFYOpening.LeaveType;
                            Opening.ApplicableEffectiveDate = LFYApplicableFrom;
                            //ADDED BY SHRADDHA ON 10 MAY 2017 END
                            WetosDB.LeaveCredits.Add(Opening); // NEW CREDIT ENTRY FY
                            WetosDB.SaveChanges();

                            // Update leave balance
                            LeaveBalance LeaveBalances = WetosDB.LeaveBalances.Where(a => a.EmployeeId == id).FirstOrDefault();
                            LeaveBalances.PreviousBalance = 0;//LeaveBalances.CurrentBalance;
                            //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 START
                            //LeaveBalances.CurrentBalance = LFYLeaveBalance + LM.MaxNoOfTimesAllowedInYear;
                            LeaveBalances.CurrentBalance = LFYLeaveBalance;
                            //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 END
                            WetosDB.SaveChanges();

                            // get data fro new fy start
                            // Updated by Rajas on 1 AUGUST 2017 to handle null value START
                            OpenCarryForward = Opening.CarryForward == null ? 0 : Opening.CarryForward.Value;
                            OpenOpenBal = Opening.OpeningBalance == null ? 0 : Opening.OpeningBalance.Value;
                            // Updated by Rajas on 1 AUGUST 2017 to handle null value END

                            LeaveAllowed = OpenCarryForward;
                            LeaveBalance = OpenCarryForward;

                            DateTime ApplicableFrom = Opening.ApplicableEffectiveDate == null ? FYLA.StartDate : Opening.ApplicableEffectiveDate.Value;


                            if (LM.LeaveCreditTypeId != null)
                            {
                                if (LM.LeaveCreditTypeId == 2)
                                {
                                    int count = 1;
                                    for (int i = 1; ; i++)
                                    {
                                        if (ApplicableFrom.AddMonths(i) <= LeaveAsonDate)
                                        {
                                            count++;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }

                                    LeaveAllowed = Convert.ToDouble(OpenCarryForward + (count * LM.MaxNoOfLeavesAllowedInMonth));
                                }
                                else
                                {
                                    //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 START
                                    //LeaveAllowed = Convert.ToDouble(LM.MaxNoOfTimesAllowedInYear);
                                    LeaveAllowed = Convert.ToDouble(LM.NoOfDaysAllowedInYear);
                                    //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 END

                                }
                            }
                            else
                            {

                                //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 START
                                //LeaveAllowed = Convert.ToDouble(LM.MaxNoOfTimesAllowedInYear);
                                LeaveAllowed = Convert.ToDouble(LM.NoOfDaysAllowedInYear);
                                //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 END

                            }

                            //COMMENTED BELOW CODE AND USED NEW FUNCTION GetAppliedDaysTillMonthEnd BY SHRADDHA ON 24 JULY 2017 START
                            //TotalLeaveSanctioned = GetAppliedDays(WetosDB, ApplicableFrom, LeaveAsonDate, id);
                            FromDate = new DateTime(LeaveAsonDate.Year, LeaveAsonDate.Month, 01); //ADDED BY SHRADDHA ON 25 JULY 2017
                            TotalLeaveSanctioned = GetAppliedDaysTillMonthEnd(WetosDB, ApplicableFrom, LeaveAsonDate, id, LM.LeaveCode);
                            TotalLeaveSanctionedForCurrentMonth = GetAppliedDaysTillMonthEnd(WetosDB, FromDate, LeaveAsonDate, id, LM.LeaveCode); //ADDED BY SHRADDHA ON 25 JULY 2017
                            //COMMENTED BELOW CODE AND USED NEW FUNCTION GetAppliedDaysTillMonthEnd BY SHRADDHA ON 24 JULY 2017 END

                            LeaveBalance = LeaveAllowed - TotalLeaveSanctioned;
                            LeaveBalanceForCurrentMonth = LeaveAllowed - TotalLeaveSanctionedForCurrentMonth; //ADDED BY SHRADDHA ON 25 JULY 2017
                            //}
                            // get data fro new fy END
                        }
                    }
                    else
                    {
                        // Error is handled 

                    }
                }


                // ADDED BY MSJ ON 08 MAY 2017 END


                LeaveBalance EmpoyeeLeaveBalanceObj = WetosDB.LeaveBalances.Where(a => a.EmployeeId == id
                    && a.LeaveType.ToUpper().Trim() == LM.LeaveCode.ToUpper().Trim()).FirstOrDefault();

                if (EmpoyeeLeaveBalanceObj != null)
                {

                    SP_LeaveTableDataNew_Result LeaveBalanceObj = new SP_LeaveTableDataNew_Result();

                    LeaveBalanceObj.LeaveType = EmpoyeeLeaveBalanceObj.LeaveType.Trim();
                    LeaveBalanceObj.LeaveAllowed = Convert.ToDecimal(LeaveAllowed); // ActualDays.CurrentBalance;
                    LeaveBalanceObj.LeaveUsedAsOnDate = Convert.ToDecimal(TotalLeaveSanctionedForCurrentMonth); //ADDED BY SHRADDHA ON 25 JULY 2017
                    LeaveBalanceObj.CurrentBalance = LeaveBalance; // ActualDays.CurrentBalance;
                    LeaveBalanceObj.LeaveUsed = TotalLeaveSanctioned; // ActualDays.LeaveUsed;
                    LeaveBalanceObj.OpeningBalance = OpenOpenBal; // Opening.OpeningBalance;

                    LeaveBalanceList.Add(LeaveBalanceObj);
                }
            }

            return LeaveBalanceList;


            #endregion
        }


        /// <summary>
        /// NewLy Created By Shraddha on 24 JULY 2017
        /// </summary>
        /// <param name="WetosDB"></param>
        /// <param name="id"></param>
        /// <param name="LeaveAsonDate"></param>
        /// <returns></returns>
        public static List<SP_LeaveTableDataNew_Result> GetLeavedataNewLogicNew(WetosDBEntities WetosDB, int id, DateTime LeaveAsonDate)
        {

            #region CODE ADDED BY SHRADDHA ON 24 MAY 2017 TO GET GENERIC FUNCTION ACROSS APPLICATION FOR ALLOWED LEAVES

            //GET LEAVE BALANCE DATA
            List<SP_LeaveTableDataNew_Result> LeaveBalanceList = new List<SP_LeaveTableDataNew_Result>();

            // GET EMPLOYEE
            SP_EmployeeProfile_Result Employee = WetosDB.SP_EmployeeProfile(id).FirstOrDefault();

            // GET EMP GRP
            EmployeeGroupDetail EmployeeGroupDetailObj = WetosDB.EmployeeGroupDetails.Where(a => a.Employee.EmployeeId == Employee.EmployeeId).FirstOrDefault();

            // Added by Rajas on 10 AUGUST 2017
            // To handle null Object
            if (EmployeeGroupDetailObj == null)
            {
                return LeaveBalanceList;
            }

            // ADDED BY MSJ ON 08 MAY 2017 START
            //DateTime LeaveAsonDate = new DateTime(2017, 2, 1);
            //DateTime LeaveAsonDate = new DateTime(2017, 2, 15);
            //DateTime LeaveAsonDate = DateTime.Now;
            //DateTime LeaveAsonDate = new DateTime(2017, 3, 15);
            //DateTime LeaveAsonDate = new DateTime(2017, 3, 31);

            //bool MonthlyCreditFlag = true; // GLOBAL SETTING
            // Get FY
            WetosDB.FinancialYear FYLA = WetosDB.FinancialYears.Where(a => a.StartDate <= LeaveAsonDate && a.EndDate >= LeaveAsonDate
                && a.Company.CompanyId == Employee.CompanyId && a.Branch.BranchId == Employee.BranchId).FirstOrDefault();

            // Updated by Rajas on 2 JUNE 2017
            if (FYLA == null)
            {
                return LeaveBalanceList;
            }

            double OpenCarryForward = 0.0;
            double OpenOpenBal = 0.0;
            double LeaveAllowed = 0.0;
            double LeaveBalance = 0.0;
            double TotalLeaveSanctioned = 0;
            double LeaveBalanceForCurrentMonth = 0.0; //ADDED BY SHRADDHA ON 25 JULY 2017
            double TotalLeaveSanctionedForCurrentMonth = 0; //ADDED BY SHRADDHA ON 25 JULY 2017
            // GET LEAVE MASTER FROM EMPLOYEE ID
            List<LeaveMaster> EmpAllowedLeaveList = WetosDB.LeaveMasters.Where(a => a.Company.CompanyId == Employee.CompanyId
                && a.BranchId == Employee.BranchId && a.EmployeeGroup.EmployeeGroupId == EmployeeGroupDetailObj.EmployeeGroup.EmployeeGroupId).ToList();

            foreach (LeaveMaster LM in EmpAllowedLeaveList)
            {
                // GET LIST OF LEAVE CREDIT FOR FY

                var Opening = WetosDB.LeaveCredits.Where(a => a.EmployeeId == id && a.FinancialYearId == FYLA.FinancialYearId
                    && a.LeaveType.Trim().ToUpper() == LM.LeaveCode.Trim().ToUpper()).FirstOrDefault();

                if (Opening != null) // UPDATE
                {

                    // ADDED BY MSJ ON 29 MAY 2017 for MULTIPLE LEAVE TYPE START
                    //List<LeaveMaster> LeaveMasterList = WetosDB.LeaveMasters.Where(a => a.EmployeeGrpId == EmployeeGroupDetailObj.EmployeeGroupId
                    //    && a.CompanyId == Employee.CompanyId && a.BranchId == Employee.BranchId).ToList();

                    //LeaveMaster LeaveMasterObj = WetosDB.LeaveMasters.Where(a => a.EmployeeGrpId == EmployeeGroupDetailObj.EmployeeGroupId
                    //    && a.CompanyId == Employee.CompanyId && a.BranchId == Employee.BranchId
                    //    && a.LeaveCode.Trim().ToUpper() == Opening.LeaveType.Trim().ToUpper()).FirstOrDefault();

                    // Updated by Rajas on 31 JULY 2017 to handle null value
                    OpenCarryForward = Opening.CarryForward == null ? 0 : Opening.CarryForward.Value;

                    // Updated by Rajas on 1 AUGUST 2017 to handle null value 
                    OpenOpenBal = Opening.OpeningBalance == null ? 0 : Opening.OpeningBalance.Value;
                    LeaveAllowed = OpenCarryForward;
                    LeaveBalance = OpenCarryForward;

                    DateTime ApplicableFrom = Opening.ApplicableEffectiveDate == null ? FYLA.StartDate : Opening.ApplicableEffectiveDate.Value;
                    //if (LeaveMasterObj != null)
                    //{
                    if (LM.LeaveCreditTypeId != null)
                    {
                        if (LM.LeaveCreditTypeId == 2)
                        {
                            int count = 1;
                            for (int i = 1; ; i++)
                            {
                                if (ApplicableFrom.AddMonths(i) <= LeaveAsonDate)
                                {
                                    count++;
                                }
                                else
                                {
                                    break;
                                }
                            }

                            LeaveAllowed = Convert.ToDouble(OpenCarryForward + (count * LM.MaxNoOfLeavesAllowedInMonth));
                        }
                        else
                        {
                            //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 START
                            //LeaveAllowed = Convert.ToDouble(LM.MaxNoOfTimesAllowedInYear);
                            //LeaveAllowed = Convert.ToDouble(LM.NoOfDaysAllowedInYear);
                            //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 END

                        }
                    }
                    else
                    {
                        //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 START
                        //LeaveAllowed = Convert.ToDouble(LM.MaxNoOfTimesAllowedInYear);
                        //LeaveAllowed = Convert.ToDouble(LM.NoOfDaysAllowedInYear);
                        //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 END
                    }

                    //COMMENTED BELOW CODE AND USED NEW FUNCTION GetAppliedDaysTillMonthEnd BY SHRADDHA ON 24 JULY 2017 START
                    //TotalLeaveSanctioned = GetAppliedDays(WetosDB, ApplicableFrom, LeaveAsonDate, id);
                    DateTime FromDate = new DateTime(LeaveAsonDate.Year, LeaveAsonDate.Month, 01); //ADDED BY SHRADDHA ON 25 JULY 2017
                    TotalLeaveSanctioned = GetAppliedDaysTillMonthEnd(WetosDB, ApplicableFrom, LeaveAsonDate, id, LM.LeaveCode);
                    TotalLeaveSanctionedForCurrentMonth = GetAppliedDaysTillMonthEnd(WetosDB, FromDate, LeaveAsonDate, id, LM.LeaveCode); //ADDED BY SHRADDHA ON 25 JULY 2017
                    //COMMENTED BELOW CODE AND USED NEW FUNCTION GetAppliedDaysTillMonthEnd BY SHRADDHA ON 24 JULY 2017 END

                    LeaveBalance = LeaveAllowed - TotalLeaveSanctioned; //ADDED BY SHRADDHA ON 25 JULY 2017
                    LeaveBalanceForCurrentMonth = LeaveAllowed - TotalLeaveSanctionedForCurrentMonth; //ADDED BY SHRADDHA ON 25 JULY 2017
                    //}
                }
                else
                {
                    // ADD NEW LEAVE CREDIT
                    Opening = new LeaveCredit();

                    Opening.FinancialYearId = FYLA.FinancialYearId;

                    // GET LAST FY CLSOING BALANCAE
                    //DateTime LFYLeaveAsonDate = new DateTime(2017, 4, 1);
                    //DateTime LeaveAsonDate = new DateTime(2017, 3, 15);
                    //DateTime LeaveAsonDate = new DateTime(2017, 3, 31);

                    // Get FY
                    WetosDB.FinancialYear LFYFYLA = WetosDB.FinancialYears.Where(a => a.FinancialYearId == FYLA.PrevFYId).FirstOrDefault();

                    // Added by Rajas on 1 JUNE 2017 to handle null error
                    if (LFYFYLA != null)
                    {

                        double LFYOpenCarryForward = 0.0;
                        double LFYOpenOpenBal = 0.0;
                        double LFYLeaveAllowed = 0.0;
                        double LFYLeaveBalance = 0.0;
                        double LFYTotalLeaveSanctioned = 0;
                        double LFYLeaveBalanceforCurrentMonth = 0.0; //ADDED BY SHRADDHA ON 25 JULY 2017
                        double LFYTotalLeaveSanctionedforCurrentMonth = 0; //ADDED BY SHRADDHA ON 25 JULY 2017
                        // LASTS YEAR
                        var LFYOpening = WetosDB.LeaveCredits.Where(a => a.EmployeeId == id && a.FinancialYearId == LFYFYLA.FinancialYearId).FirstOrDefault();

                        if (LFYOpening != null && LFYOpening.LeaveType.Trim() == LM.LeaveCode.Trim())
                        {
                            // EmployeeGroupDetail EmployeeGroupDetailObj = WetosDB.EmployeeGroupDetails.Where(a => a.EmployeeId == Employee.EmployeeId).FirstOrDefault();

                            //LeaveMaster LeaveMasterObj = WetosDB.LeaveMasters.Where(a => a.EmployeeGrpId == EmployeeGroupDetailObj.EmployeeGroupId
                            //    && a.CompanyId == Employee.CompanyId && a.BranchId == Employee.BranchId
                            //    && a.LeaveCode.Trim().ToUpper() == LFYOpening.LeaveType.Trim().ToUpper()).FirstOrDefault();

                            //if (LeaveMasterObj != null)
                            //{
                            // Updated by Rajas on 1 AUGUST 2017 to handle null value
                            LFYOpenCarryForward = LFYOpening.CarryForward == null ? 0 : LFYOpening.CarryForward.Value;

                            // Updated by Rajas on 1 AUGUST 2017 to handle null value
                            LFYOpenOpenBal = LFYOpening.OpeningBalance == null ? 0 : LFYOpening.OpeningBalance.Value;
                            LFYLeaveAllowed = LFYOpenCarryForward;
                            LFYLeaveBalance = LFYOpenCarryForward;

                            DateTime LFYApplicableFrom = LFYOpening.ApplicableEffectiveDate == null ? FYLA.StartDate : LFYOpening.ApplicableEffectiveDate.Value;

                            if (LM.LeaveCreditTypeId != null)
                            {
                                if (LM.LeaveCreditTypeId == 2)
                                {
                                    int LFYcount = 1;
                                    for (int i = 1; ; i++)
                                    {
                                        if (LFYApplicableFrom.AddMonths(i) <= FYLA.EndDate)
                                        {
                                            LFYcount++;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }

                                    LFYLeaveAllowed = Convert.ToDouble(LFYOpenCarryForward + (LFYcount * LM.MaxNoOfLeavesAllowedInMonth));
                                }

                                else
                                {
                                    //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 START
                                    //LFYLeaveAllowed = Convert.ToDouble(LM.MaxNoOfTimesAllowedInYear);
                                    LFYLeaveAllowed = Convert.ToDouble(LM.NoOfDaysAllowedInYear);
                                    //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 END
                                }
                            }
                            else
                            {
                                //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 START
                                //LFYLeaveAllowed = Convert.ToDouble(LM.MaxNoOfTimesAllowedInYear);
                                LFYLeaveAllowed = Convert.ToDouble(LM.NoOfDaysAllowedInYear);
                                //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 END
                            }

                            //COMMENTED BELOW CODE AND USED NEW FUNCTION GetAppliedDaysTillMonthEnd BY SHRADDHA ON 24 JULY 2017 START
                            //LFYTotalLeaveSanctioned = GetAppliedDays(WetosDB, LFYApplicableFrom, LFYFYLA.EndDate, id);
                            DateTime FromDate = new DateTime(LFYFYLA.EndDate.Year, LFYFYLA.EndDate.Month, 01);
                            LFYTotalLeaveSanctioned = GetAppliedDaysTillMonthEnd(WetosDB, LFYApplicableFrom, LFYFYLA.EndDate, id, LM.LeaveCode);
                            LFYTotalLeaveSanctionedforCurrentMonth = GetAppliedDaysTillMonthEnd(WetosDB, FromDate, LFYFYLA.EndDate, id, LM.LeaveCode); //ADDED BY SHRADDHA ON 25 JULY 2017
                            //COMMENTED BELOW CODE AND USED NEW FUNCTION GetAppliedDaysTillMonthEnd BY SHRADDHA ON 24 JULY 2017 END


                            LFYLeaveBalance = LFYLeaveAllowed - LFYTotalLeaveSanctioned;
                            LFYLeaveBalanceforCurrentMonth = LFYLeaveAllowed - LFYTotalLeaveSanctionedforCurrentMonth; //ADDED BY SHRADDHA ON 25 JULY 2017

                            // ASSIGNED VALUE
                            Opening.CarryForward = LFYLeaveBalance;
                            Opening.CompanyId = LFYFYLA.Company.CompanyId;
                            Opening.BranchId = LFYFYLA.Branch.BranchId;
                            //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 START
                            //Opening.OpeningBalance = LFYLeaveBalance + LM.MaxNoOfTimesAllowedInYear; //
                            Opening.OpeningBalance = LFYLeaveBalance + LM.NoOfDaysAllowedInYear;
                            //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 END
                            //ADDED BY SHRADDHA ON 10 MAY 2017 START
                            Opening.LeaveType = LFYOpening.LeaveType;
                            Opening.ApplicableEffectiveDate = LFYApplicableFrom;
                            //ADDED BY SHRADDHA ON 10 MAY 2017 END

                            WetosDB.LeaveCredits.Add(Opening); // NEW CREDIT ENTRY FY
                            WetosDB.SaveChanges();

                            // Update leave balance
                            LeaveBalance LeaveBalances = WetosDB.LeaveBalances.Where(a => a.EmployeeId == id).FirstOrDefault();
                            LeaveBalances.PreviousBalance = 0;//LeaveBalances.CurrentBalance;
                            //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 START
                            //LeaveBalances.CurrentBalance = LFYLeaveBalance + LM.MaxNoOfTimesAllowedInYear;
                            LeaveBalances.CurrentBalance = LFYLeaveBalance;
                            //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 END
                            WetosDB.SaveChanges();

                            // get data fro new fy start
                            OpenCarryForward = Opening.CarryForward.Value;
                            OpenOpenBal = Opening.OpeningBalance.Value;
                            LeaveAllowed = OpenCarryForward;
                            LeaveBalance = OpenCarryForward;

                            DateTime ApplicableFrom = Opening.ApplicableEffectiveDate == null ? FYLA.StartDate : Opening.ApplicableEffectiveDate.Value;


                            if (LM.LeaveCreditTypeId != null)
                            {
                                if (LM.LeaveCreditTypeId == 2)
                                {
                                    int count = 1;
                                    for (int i = 1; ; i++)
                                    {
                                        if (ApplicableFrom.AddMonths(i) <= LeaveAsonDate)
                                        {
                                            count++;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }

                                    LeaveAllowed = Convert.ToDouble(OpenCarryForward + (count * LM.MaxNoOfLeavesAllowedInMonth));
                                }
                                else
                                {
                                    //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 START
                                    //LeaveAllowed = Convert.ToDouble(LM.MaxNoOfTimesAllowedInYear);
                                    LeaveAllowed = Convert.ToDouble(LM.NoOfDaysAllowedInYear);
                                    //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 END
                                }
                            }
                            else
                            {

                                //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 START
                                //LeaveAllowed = Convert.ToDouble(LM.MaxNoOfTimesAllowedInYear);
                                LeaveAllowed = Convert.ToDouble(LM.NoOfDaysAllowedInYear);
                                //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 END

                            }

                            //COMMENTED BELOW CODE AND USED NEW FUNCTION GetAppliedDaysTillMonthEnd BY SHRADDHA ON 24 JULY 2017 START
                            //TotalLeaveSanctioned = GetAppliedDays(WetosDB, ApplicableFrom, LeaveAsonDate, id);
                            FromDate = new DateTime(LeaveAsonDate.Year, LeaveAsonDate.Month, 01); //ADDED BY SHRADDHA ON 25 JULY 2017
                            TotalLeaveSanctioned = GetAppliedDaysTillMonthEnd(WetosDB, ApplicableFrom, LeaveAsonDate, id, LM.LeaveCode);
                            TotalLeaveSanctionedForCurrentMonth = GetAppliedDaysTillMonthEnd(WetosDB, FromDate, LeaveAsonDate, id, LM.LeaveCode); //ADDED BY SHRADDHA ON 25 JULY 2017
                            //COMMENTED BELOW CODE AND USED NEW FUNCTION GetAppliedDaysTillMonthEnd BY SHRADDHA ON 24 JULY 2017 END

                            LeaveBalance = LeaveAllowed - TotalLeaveSanctioned;
                            LeaveBalanceForCurrentMonth = LeaveAllowed - TotalLeaveSanctionedForCurrentMonth; //ADDED BY SHRADDHA ON 25 JULY 2017
                            //}
                            // get data fro new fy END
                        }
                    }
                    else
                    {
                        // Error is handled 

                    }
                }


                // ADDED BY MSJ ON 08 MAY 2017 END


                LeaveBalance EmpoyeeLeaveBalanceObj = WetosDB.LeaveBalances.Where(a => a.EmployeeId == id
                    && a.LeaveType.ToUpper().Trim() == LM.LeaveCode.ToUpper().Trim()).FirstOrDefault();

                if (EmpoyeeLeaveBalanceObj != null)
                {

                    SP_LeaveTableDataNew_Result LeaveBalanceObj = new SP_LeaveTableDataNew_Result();

                    LeaveBalanceObj.LeaveType = EmpoyeeLeaveBalanceObj.LeaveType.Trim();
                    LeaveBalanceObj.LeaveAllowed = Convert.ToDecimal(LeaveAllowed); // ActualDays.CurrentBalance;
                    LeaveBalanceObj.LeaveUsedAsOnDate = Convert.ToDecimal(TotalLeaveSanctionedForCurrentMonth); //ADDED BY SHRADDHA ON 25 JULY 2017
                    LeaveBalanceObj.CurrentBalance = LeaveBalance; // ActualDays.CurrentBalance;
                    LeaveBalanceObj.LeaveUsed = TotalLeaveSanctioned; // ActualDays.LeaveUsed;
                    LeaveBalanceObj.OpeningBalance = OpenOpenBal; // Opening.OpeningBalance;

                    LeaveBalanceList.Add(LeaveBalanceObj);
                }
            }

            return LeaveBalanceList;


            #endregion
        }


        public static List<SP_LeaveTableData_Result> GetLeavedataOnProfileNewLogic(WetosDBEntities WetosDB, int id)
        {

            #region CODE ADDED BY SHRADDHA ON 24 MAY 2017 TO GET GENERIC FUNCTION ACROSS APPLICATION FOR ALLOWED LEAVES
            SP_EmployeeProfile_Result Employee = WetosDB.SP_EmployeeProfile(id).FirstOrDefault();
            // ADDED BY MSJ ON 08 MAY 2017 START
            //DateTime LeaveAsonDate = new DateTime(2017, 2, 1);
            //DateTime LeaveAsonDate = new DateTime(2017, 2, 15);
            DateTime LeaveAsonDate = DateTime.Now;
            //DateTime LeaveAsonDate = new DateTime(2017, 3, 15);
            //DateTime LeaveAsonDate = new DateTime(2017, 3, 31);

            List<SP_LeaveTableData_Result> LeaveBalanceList = new List<SP_LeaveTableData_Result>();

            // Added by Rajas on 10 AUGUST 2017
            // To handle null object
            if (Employee == null)
            {
                return LeaveBalanceList;
            }

            bool MonthlyCreditFlag = true; // GLOBAL SETTING
            // Get FY
            WetosDB.FinancialYear FYLA = WetosDB.FinancialYears.Where(a => a.StartDate <= LeaveAsonDate && a.EndDate >= LeaveAsonDate && a.Company.CompanyId == Employee.CompanyId && a.Branch.BranchId == Employee.BranchId).FirstOrDefault();

            if (FYLA == null)
            {
                // Updated by Rajas on 10 AUGUST 2017
                // To handle null object
                return LeaveBalanceList;
            }

            double OpenCarryForward = 0.0;
            double OpenOpenBal = 0.0;
            double LeaveAllowed = 0.0;
            double LeaveBalance = 0.0;
            double TotalLeaveSanctioned = 0;

            var Opening = WetosDB.LeaveCredits.Where(a => a.EmployeeId == id && a.FinancialYearId == FYLA.FinancialYearId).FirstOrDefault();

            if (Opening != null)
            {
                // Updated by Rajas on 1 AUGUST 2017 to handle null value START
                OpenCarryForward = Opening.CarryForward == null ? 0 : Opening.CarryForward.Value;
                OpenOpenBal = Opening.OpeningBalance == null ? 0 : Opening.OpeningBalance.Value;
                // Updated by Rajas on 1 AUGUST 2017 to handle null value END

                LeaveAllowed = OpenCarryForward;
                LeaveBalance = OpenCarryForward;

                DateTime ApplicableFrom = Opening.ApplicableEffectiveDate == null ? FYLA.StartDate : Opening.ApplicableEffectiveDate.Value;

                if (MonthlyCreditFlag)
                {
                    int count = 0;
                    for (int i = 1; ; i++)
                    {
                        if (ApplicableFrom.AddMonths(i) <= LeaveAsonDate)
                        {
                            count++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    LeaveAllowed = OpenCarryForward + (count * 1.75);
                }

                TotalLeaveSanctioned = GetAppliedDays(WetosDB, ApplicableFrom, LeaveAsonDate, id);
                LeaveBalance = LeaveAllowed - TotalLeaveSanctioned;

                // ADDED BY MSJ ON 08 MAY 2017 END

                var ActualDays = WetosDB.LeaveBalances.Where(a => a.EmployeeId == id).FirstOrDefault();


                // Above line commented and below SP code adde by Rajas on 13 APRIL 2017
                LeaveBalanceList = WetosDB.SP_LeaveTableData(id).ToList();

                foreach (SP_LeaveTableData_Result LeaveBalanceObj in LeaveBalanceList)
                {
                    if (ActualDays != null && Opening != null)
                    {
                        if (LeaveBalanceObj.LeaveType == ActualDays.LeaveType)
                        {
                            LeaveBalanceObj.LeaveType = ActualDays.LeaveType;
                            LeaveBalanceObj.LeaveAllowed = Convert.ToDecimal(LeaveAllowed); // ActualDays.CurrentBalance;
                            LeaveBalanceObj.CurrentBalance = LeaveBalance; // ActualDays.CurrentBalance;
                            LeaveBalanceObj.LeaveUsed = TotalLeaveSanctioned; // ActualDays.LeaveUsed;
                            LeaveBalanceObj.OpeningBalance = OpenOpenBal; // Opening.OpeningBalance;
                        }

                    }
                }

                return LeaveBalanceList;
            }
            else
            {
                return LeaveBalanceList;
            }


            #endregion
        }

        /// <summary>
        /// LEAVE CREDIT FOR CHANGE IN COMPANY AND BRANCH
        /// </summary>
        /// <param name="WetosDB"></param>
        /// <param name="id"></param>
        /// <param name="LeaveAsonDate"></param>
        /// <returns></returns>
        public static bool UpdateLeaveCreditBalance(WetosDBEntities WetosDB, int id, DateTime LeaveAsOnDate, int OldCompany, int NewCompany, int OldBranch, int NewBranch)
        {
            bool ReturnStatus = false;

            try
            {
                // GET EMPLOYEE         
                Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == id).FirstOrDefault();

                // Updated by Rajas on 10 AUGUST 2017
                // Prevent from null object error
                if (EmployeeObj == null)
                {
                    return ReturnStatus;
                }

                EmployeeGroupDetail EmpGrpDetail = WetosDB.EmployeeGroupDetails.Where(a => a.Employee.EmployeeId == EmployeeObj.EmployeeId).FirstOrDefault();

                // Updated by Rajas on 10 AUGUST 2017
                // Prevent from null object error
                if (EmpGrpDetail == null)
                {
                    return ReturnStatus;
                }

                // 
                WetosDB.FinancialYear FYLA = WetosDB.FinancialYears.Where(a => a.StartDate <= LeaveAsOnDate && a.EndDate >= LeaveAsOnDate
                          && a.Company.CompanyId == EmployeeObj.CompanyId && a.Branch.BranchId == EmployeeObj.BranchId).FirstOrDefault();


                //// GET Leave Applicable for old Grp
                List<LeaveMaster> GrpLeaveMasterList = WetosDB.LeaveMasters.Where(a => a.EmployeeGroup.EmployeeGroupId == EmpGrpDetail.EmployeeGroup.EmployeeGroupId).ToList();

                foreach (LeaveMaster LMObj in GrpLeaveMasterList)
                {
                    // UPDATE LEAVE CREDIT
                    LeaveCredit LeaveCreditObj = WetosDB.LeaveCredits.Where(a => a.EmployeeId == EmpGrpDetail.Employee.EmployeeId
                        && a.FinancialYearId == FYLA.FinancialYearId && a.CompanyId == OldCompany && a.BranchId == OldBranch).FirstOrDefault();

                    if (LeaveCreditObj != null)
                    {
                        LeaveCreditObj.FinancialYearId = FYLA.FinancialYearId;
                        LeaveCreditObj.CompanyId = EmployeeObj.CompanyId;
                        LeaveCreditObj.BranchId = EmployeeObj.BranchId;

                        WetosDB.SaveChanges();
                    }

                    // UPDATE LEAVE BALANCE
                    LeaveBalance LeaveBalanceObj = WetosDB.LeaveBalances.Where(a => a.EmployeeId == EmpGrpDetail.Employee.EmployeeId).FirstOrDefault();

                    if (LeaveBalanceObj != null)
                    {
                        LeaveBalanceObj.CompanyId = EmployeeObj.CompanyId;
                        LeaveBalanceObj.BranchId = EmployeeObj.BranchId;

                        WetosDB.SaveChanges();
                    }
                }

                return ReturnStatus;
            }
            catch
            {
                return ReturnStatus;
            }
        }


        /// <summary>
        /// LEAVE CREDIT FOR NEW EMPLOYEE AND 
        /// </summary>
        /// <param name="WetosDB"></param>
        /// <param name="id"></param>
        /// <param name="LeaveAsonDate"></param>
        /// <returns></returns>
        public static bool AddUpdateLeaveCreditBalance(WetosDBEntities WetosDB, int id, DateTime LeaveAsOnDate, int OldEmpGrp, int NewEmpGrp)
        {
            bool ReturnStatus = false;

            try
            {
                if (LeaveAsOnDate.Day > 15)
                {
                    DateTime NewDate = LeaveAsOnDate.AddMonths(1);
                    LeaveAsOnDate = new DateTime(NewDate.Year, NewDate.Month, 01);
                }
                else
                {
                    LeaveAsOnDate = new DateTime(LeaveAsOnDate.Year, LeaveAsOnDate.Month, 01);
                }
                DateTime LeaveEffectiveLastDate = LeaveAsOnDate.AddDays(-1);



                // GET EMPLOYEE         
                Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == id).FirstOrDefault();

                WetosDB.FinancialYear FYLA = WetosDB.FinancialYears.Where(a => a.StartDate <= LeaveAsOnDate && a.EndDate >= LeaveAsOnDate
                          && a.Company.CompanyId == EmployeeObj.CompanyId && a.Branch.BranchId == EmployeeObj.BranchId).FirstOrDefault();

                //if (FYLA != null)
                //{

                //}

                if (OldEmpGrp == -1) // NEW EMPLOYEE
                {
                    // GET Leave Applicable
                    List<LeaveMaster> LeaveMasterList = WetosDB.LeaveMasters.Where(a => a.EmployeeGroup.EmployeeGroupId == NewEmpGrp).ToList();

                    foreach (LeaveMaster LMObj in LeaveMasterList)
                    {
                        // ADD TO LEAVE CREDIT
                        LeaveCredit LeaveCreditObj = new LeaveCredit();

                        LeaveCreditObj.FinancialYearId = FYLA.FinancialYearId;
                        LeaveCreditObj.CarryForward = 0;
                        LeaveCreditObj.CompanyId = EmployeeObj.CompanyId;
                        LeaveCreditObj.BranchId = EmployeeObj.BranchId;
                        //LeaveCreditObj.OpeningBalance = LMObj.MaxNoOfTimesAllowedInYear;
                        //LeaveCreditObj.OpeningBalance = LMObj.NoOfDaysAllowedInYear;
                        if (LMObj.LeaveCreditTypeId == 2) //MONTHLY CREDITABLE
                        {
                            int count = 1;
                            for (int i = 1; ; i++)
                            {
                                if (LeaveAsOnDate.AddMonths(i) <= FYLA.EndDate)
                                {
                                    count++;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            LeaveCreditObj.OpeningBalance = (count * LMObj.MaxNoOfLeavesAllowedInMonth);
                        }
                        else
                        {

                            LeaveCreditObj.OpeningBalance = LMObj.NoOfDaysAllowedInYear;
                        }

                        LeaveCreditObj.ApplicableEffectiveDate = LeaveAsOnDate;
                        LeaveCreditObj.EmployeeId = id;
                        LeaveCreditObj.LeaveType = LMObj.LeaveCode.Trim();  // Added by Rajas on 6 JUNE 2017
                        WetosDB.LeaveCredits.Add(LeaveCreditObj);
                        WetosDB.SaveChanges();

                        // ADD TO LEAVE BALANCE
                        LeaveBalance LeaveBalanceObj = new LeaveBalance();

                        LeaveBalanceObj.CompanyId = EmployeeObj.CompanyId;
                        LeaveBalanceObj.BranchId = EmployeeObj.BranchId;
                        LeaveBalanceObj.PreviousBalance = 0;
                        //LeaveBalanceObj.CurrentBalance = LMObj.MaxNoOfTimesAllowedInYear;
                        //LeaveBalanceObj.CurrentBalance = LMObj.NoOfDaysAllowedInYear;
                        if (LMObj.LeaveCreditTypeId == 2) //MONTHLY CREDITABLE
                        {
                            int count = 1;
                            for (int i = 1; ; i++)
                            {
                                if (LeaveAsOnDate.AddMonths(i) <= FYLA.EndDate)
                                {
                                    count++;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            LeaveBalanceObj.CurrentBalance = (count * LMObj.MaxNoOfLeavesAllowedInMonth);
                        }
                        else
                        {

                            LeaveBalanceObj.CurrentBalance = LMObj.NoOfDaysAllowedInYear;
                        }
                        LeaveBalanceObj.EmployeeId = id;
                        LeaveBalanceObj.LeaveType = LMObj.LeaveCode.Trim();  // Added by Rajas on 6 JUNE 2017
                        WetosDB.LeaveBalances.Add(LeaveBalanceObj);
                        WetosDB.SaveChanges();

                    }
                }
                else // NEW GROUP
                {
                    List<LeaveMaster> OldGrpLeaveMasterList = WetosDB.LeaveMasters.Where(a => a.EmployeeGroup.EmployeeGroupId == OldEmpGrp).ToList();

                    // GET Leave Applicable for new Grp
                    List<LeaveMaster> NewGrpLeaveMasterList = WetosDB.LeaveMasters.Where(a => a.EmployeeGroup.EmployeeGroupId == NewEmpGrp).ToList();

                    if (OldGrpLeaveMasterList.Count > 0)
                    {
                        foreach (LeaveMaster OldGrpLeaveMasterObj in OldGrpLeaveMasterList)
                        {

                            LeaveCredit LeaveCreditObj = WetosDB.LeaveCredits.Where(a => a.LeaveType.Trim().ToUpper() == OldGrpLeaveMasterObj.LeaveCode.Trim().ToUpper()).FirstOrDefault();
                            if (LeaveCreditObj != null)
                            {
                                LeaveCreditObj.ApplicableEffectiveEndDate = LeaveEffectiveLastDate;
                                WetosDB.SaveChanges();
                            }
                        }

                    }

                    foreach (LeaveMaster LMObj in NewGrpLeaveMasterList)
                    {
                        // CHECK LEAVE TYE AVAILABLE IN OLD GRP
                        LeaveMaster LeaveCommon = OldGrpLeaveMasterList.Where(a => a.LeaveId == LMObj.LeaveId).FirstOrDefault();

                        if (LeaveCommon == null)
                        {
                            // ADD TO LEAVE CREDIT
                            LeaveCredit LeaveCreditObj = new LeaveCredit();

                            LeaveCreditObj.FinancialYearId = FYLA.FinancialYearId;
                            LeaveCreditObj.CarryForward = 0;
                            LeaveCreditObj.CompanyId = EmployeeObj.CompanyId;
                            LeaveCreditObj.BranchId = EmployeeObj.BranchId;
                            LeaveCreditObj.LeaveType = LMObj.LeaveCode;
                            //LeaveCreditObj.OpeningBalance = LMObj.MaxNoOfTimesAllowedInYear;
                            if (LMObj.LeaveCreditTypeId == 2) //MONTHLY CREDITABLE
                            {
                                int count = 1;
                                for (int i = 1; ; i++)
                                {
                                    if (LeaveAsOnDate.AddMonths(i) <= FYLA.EndDate)
                                    {
                                        count++;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                LeaveCreditObj.OpeningBalance = (count * LMObj.MaxNoOfLeavesAllowedInMonth);
                            }
                            else
                            {

                                LeaveCreditObj.OpeningBalance = LMObj.NoOfDaysAllowedInYear;
                            }
                            LeaveCreditObj.ApplicableEffectiveDate = LeaveAsOnDate;
                            LeaveCreditObj.EmployeeId = id;
                            WetosDB.LeaveCredits.Add(LeaveCreditObj);
                            WetosDB.SaveChanges();

                            // ADD TO LEAVE BALANCE
                            LeaveBalance LeaveBalanceObj = new LeaveBalance();

                            LeaveBalanceObj.CompanyId = EmployeeObj.CompanyId;
                            LeaveBalanceObj.BranchId = EmployeeObj.BranchId;
                            LeaveBalanceObj.PreviousBalance = 0;
                            //LeaveBalanceObj.CurrentBalance = LMObj.MaxNoOfTimesAllowedInYear;

                            if (LMObj.LeaveCreditTypeId == 2) //MONTHLY CREDITABLE
                            {
                                int count = 1;
                                for (int i = 1; ; i++)
                                {
                                    if (LeaveAsOnDate.AddMonths(i) <= FYLA.EndDate)
                                    {
                                        count++;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                LeaveBalanceObj.CurrentBalance = (count * LMObj.MaxNoOfLeavesAllowedInMonth);
                            }
                            else
                            {

                                LeaveBalanceObj.CurrentBalance = LMObj.NoOfDaysAllowedInYear;
                            }
                            //LeaveBalanceObj.CurrentBalance = LMObj.NoOfDaysAllowedInYear;
                            LeaveBalanceObj.LeaveType = LMObj.LeaveCode;
                            LeaveBalanceObj.EmployeeId = id;
                            WetosDB.LeaveBalances.Add(LeaveBalanceObj);
                            WetosDB.SaveChanges();
                            ReturnStatus = true;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {

            }

            return ReturnStatus;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ErrorNo"></param>
        /// <returns></returns>
        public static string GetErrorMessage(int ErrorNo = 0)
        {
            string ErrorMessage = string.Empty;

            switch (ErrorNo)
            {
                case 1:
                    ErrorMessage = "Error in creating/updating departent.";
                    break;
                default:
                    ErrorMessage = "Error";
                    break;
            }

            return ErrorMessage;
        }

        // NEW CODE TO FIND LEAVE ALLOWED
        /// <summary>
        /// 
        /// </summary>
        /// <param name="WetosDB"></param>
        /// <param name="id"></param>
        /// <param name="LeaveAsonDate"></param>
        /// <returns></returns>
        public static double GetLeaveAllowed(WetosDBEntities WetosDB, int EmpId, ref LeaveModel LeaveModelObj, bool WithLeavePending) // LeaveId ADDED BY MSJ
        {
            //List<SP_LeaveTableData_Result>
            #region CODE ADDED BY SHRADDHA ON 24 MAY 2017 TO GET GENERIC FUNCTION ACROSS APPLICATION FOR ALLOWED LEAVES


            // GET EMPLOYEE
            SP_EmployeeProfile_Result Employee = WetosDB.SP_EmployeeProfile(EmpId).FirstOrDefault();

            // GET EMP GRP
            EmployeeGroupDetail EmployeeGroupDetailObj = WetosDB.EmployeeGroupDetails.Where(a => a.Employee.EmployeeId == Employee.EmployeeId).FirstOrDefault();

            DateTime LeaveAsonDate = LeaveModelObj.ToDate;
            WetosDB.FinancialYear FYLA = WetosDB.FinancialYears.Where(a => a.StartDate <= LeaveAsonDate && a.EndDate >= LeaveAsonDate
                && a.Company.CompanyId == Employee.CompanyId && a.Branch.BranchId == Employee.BranchId).FirstOrDefault();

            //GET LEAVE BALANCE DATA
            // List<SP_LeaveTableData_Result> LeaveBalanceList = new List<SP_LeaveTableData_Result>();

            // Updated by Rajas on 2 JUNE 2017
            if (FYLA == null)
            {
                return 0;// false; // LeaveBalanceList;
            }

            double OpenCarryForward = 0.0;
            double OpenOpenBal = 0.0;
            double LeaveAllowed = 0.0;
            double LeaveBalance = 0.0;
            double TotalLeaveSanctioned = 0;

            // GET LEAVE MASTER FROM EMPLOYEE ID
            //List<LeaveMaster> EmpAllowedLeaveList = 
            int LeaveId = LeaveModelObj.LeaveId;
            LeaveMaster LM = WetosDB.LeaveMasters.Where(a => a.Company.CompanyId == Employee.CompanyId
            && a.BranchId == Employee.BranchId && a.EmployeeGroup.EmployeeGroupId == EmployeeGroupDetailObj.EmployeeGroup.EmployeeGroupId && a.LeaveId == LeaveId).FirstOrDefault();
            //.ToList();

            LeaveModelObj.LeaveName = LM.LeaveName; // ADDED BY MSJ 22 JULY 2017
            LeaveModelObj.LeaveCode = LM.LeaveCode;  // ADDED BY SHRADDHA on 29 NOV 2017  TO HANDLE EXCEPTION INCASE OF HAVING LEAVENAME OF MORE THAN 10 CHARACTERS
            //foreach (LeaveMaster LM in EmpAllowedLeaveList)
            {
                // GET LIST OF LEAVE CREDIT FOR FY

                var Opening = WetosDB.LeaveCredits.Where(a => a.EmployeeId == EmpId && a.FinancialYearId == FYLA.FinancialYearId
                    && a.LeaveType.Trim().ToUpper() == LM.LeaveCode.Trim().ToUpper()).FirstOrDefault();

                var Balance = WetosDB.LeaveBalances.Where(a => a.EmployeeId == EmpId
                    && a.LeaveType.Trim().ToUpper() == LM.LeaveCode.Trim().ToUpper()).FirstOrDefault();

                if (Opening != null) // UPDATE
                {
                    // Updated by Rajas on 1 AUGUST 2017 to handle null value START
                    OpenCarryForward = Opening.CarryForward == null ? 0 : Opening.CarryForward.Value;
                    OpenOpenBal = Opening.OpeningBalance == null ? 0 : Opening.OpeningBalance.Value;
                    // Updated by Rajas on 1 AUGUST 2017 to handle null value END

                    LeaveAllowed = OpenCarryForward;
                    LeaveBalance = OpenCarryForward;

                    DateTime ApplicableFrom = Opening.ApplicableEffectiveDate == null ? FYLA.StartDate : Opening.ApplicableEffectiveDate.Value;
                    //if (LeaveMasterObj != null)
                    //{
                    if (LM.LeaveCreditTypeId != null)
                    {
                        if (LM.LeaveCreditTypeId == 2) // MONTHLY LEAVE CREDIT 2
                        {
                            int count = 1;
                            for (int i = 1; ; i++)
                            {
                                if (ApplicableFrom.AddMonths(i) <= LeaveAsonDate)
                                {
                                    count++;
                                }
                                else
                                {
                                    break;
                                }
                            }

                            LeaveAllowed = Convert.ToDouble(OpenCarryForward + (count * LM.MaxNoOfLeavesAllowedInMonth));
                            TotalLeaveSanctioned = GetAppliedDaysTillMonthEnd(WetosDB, ApplicableFrom, LeaveAsonDate, EmpId, LM.LeaveName); // MODIFIED BY MSJ ON 21 JULY 2017
                        }
                        else
                        {
                            //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 START                            
                            LeaveAllowed = OpenCarryForward > 0 ? OpenCarryForward : Convert.ToDouble(LM.NoOfDaysAllowedInYear);
                            TotalLeaveSanctioned = GetAppliedDaysTillMonthEnd(WetosDB, ApplicableFrom, LeaveAsonDate, EmpId, LM.LeaveName); // MODIFIED BY MSJ ON 21 JULY 2017
                            //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 END
                        }
                    }
                    else
                    {
                        //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 START
                        //LeaveAllowed = Convert.ToDouble(LM.MaxNoOfTimesAllowedInYear);
                        LeaveAllowed = Convert.ToDouble(LM.NoOfDaysAllowedInYear);
                        TotalLeaveSanctioned = GetAppliedDaysTillMonthEnd(WetosDB, ApplicableFrom, LeaveAsonDate, EmpId, LM.LeaveName); // MODIFIED BY MSJ ON 21 JULY 2017
                        //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 END
                    }


                    //TotalLeaveSanctioned = GetAppliedDaysTillMonthEnd(WetosDB, ApplicableFrom, LeaveAsonDate, EmpId, LM.LeaveName); // MODIFIED BY MSJ ON 21 JULY 2017


                    LeaveBalance = LeaveAllowed - TotalLeaveSanctioned;
                    //}
                }
                else
                {
                    // ADD NEW LEAVE CREDIT
                    Opening = new LeaveCredit();

                    Opening.FinancialYearId = FYLA.FinancialYearId;

                    // GET LAST FY CLSOING BALANCAE
                    //DateTime LFYLeaveAsonDate = new DateTime(2017, 4, 1);
                    //DateTime LeaveAsonDate = new DateTime(2017, 3, 15);
                    //DateTime LeaveAsonDate = new DateTime(2017, 3, 31);

                    // Get FY
                    WetosDB.FinancialYear LFYFYLA = WetosDB.FinancialYears.Where(a => a.FinancialYearId == FYLA.PrevFYId).FirstOrDefault();

                    // Added by Rajas on 1 JUNE 2017 to handle null error
                    if (LFYFYLA != null)
                    {

                        double LFYOpenCarryForward = 0.0;
                        double LFYOpenOpenBal = 0.0;
                        double LFYLeaveAllowed = 0.0;
                        double LFYLeaveBalance = 0.0;
                        double LFYTotalLeaveSanctioned = 0;

                        // LASTS YEAR
                        var LFYOpening = WetosDB.LeaveCredits.Where(a => a.EmployeeId == EmpId && a.FinancialYearId == LFYFYLA.FinancialYearId).FirstOrDefault();

                        if (LFYOpening != null && LFYOpening.LeaveType.Trim() == LM.LeaveCode.Trim())
                        {
                            // EmployeeGroupDetail EmployeeGroupDetailObj = WetosDB.EmployeeGroupDetails.Where(a => a.EmployeeId == Employee.EmployeeId).FirstOrDefault();

                            //LeaveMaster LeaveMasterObj = WetosDB.LeaveMasters.Where(a => a.EmployeeGrpId == EmployeeGroupDetailObj.EmployeeGroupId
                            //    && a.CompanyId == Employee.CompanyId && a.BranchId == Employee.BranchId
                            //    && a.LeaveCode.Trim().ToUpper() == LFYOpening.LeaveType.Trim().ToUpper()).FirstOrDefault();

                            //if (LeaveMasterObj != null)
                            //{

                            // Updated by Rajas on 1 AUGUST 2017 to handle null value START
                            LFYOpenCarryForward = LFYOpening.CarryForward == null ? 0 : LFYOpening.CarryForward.Value;
                            LFYOpenOpenBal = LFYOpening.OpeningBalance == null ? 0 : LFYOpening.OpeningBalance.Value;
                            // Updated by Rajas on 1 AUGUST 2017 to handle null value END
                            LFYLeaveAllowed = LFYOpenCarryForward;
                            LFYLeaveBalance = LFYOpenCarryForward;

                            DateTime LFYApplicableFrom = LFYOpening.ApplicableEffectiveDate == null ? FYLA.StartDate : LFYOpening.ApplicableEffectiveDate.Value;

                            if (LM.LeaveCreditTypeId != null)
                            {
                                if (LM.LeaveCreditTypeId == 2)
                                {
                                    int LFYcount = 1;
                                    for (int i = 1; ; i++)
                                    {
                                        if (LFYApplicableFrom.AddMonths(i) <= FYLA.EndDate)
                                        {
                                            LFYcount++;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }

                                    LFYLeaveAllowed = Convert.ToDouble(LFYOpenCarryForward + (LFYcount * LM.MaxNoOfLeavesAllowedInMonth));
                                }

                                else
                                {
                                    //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 START
                                    //LFYLeaveAllowed = Convert.ToDouble(LM.MaxNoOfTimesAllowedInYear);
                                    LFYLeaveAllowed = Convert.ToDouble(LM.NoOfDaysAllowedInYear);
                                    //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 END
                                }
                            }
                            else
                            {
                                //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 START
                                //LFYLeaveAllowed = Convert.ToDouble(LM.MaxNoOfTimesAllowedInYear);
                                LFYLeaveAllowed = Convert.ToDouble(LM.NoOfDaysAllowedInYear);
                                //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 END
                            }


                            //LFYTotalLeaveSanctioned = GetAppliedDays(WetosDB, LFYApplicableFrom, LFYFYLA.EndDate, EmpId);
                            LFYTotalLeaveSanctioned = GetAppliedDaysTillMonthEnd(WetosDB, LFYApplicableFrom, LeaveAsonDate, EmpId, LM.LeaveName); // MODIFIED BY MSJ ON 21 JULY 2017

                            LFYLeaveBalance = LFYLeaveAllowed - LFYTotalLeaveSanctioned;

                            // ASSIGNED VALUE
                            Opening.CarryForward = LFYLeaveBalance;
                            Opening.CompanyId = LFYFYLA.Company.CompanyId;
                            Opening.BranchId = LFYFYLA.Branch.BranchId;
                            //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 START
                            //Opening.OpeningBalance = LFYLeaveBalance + LM.MaxNoOfTimesAllowedInYear; //
                            Opening.OpeningBalance = LFYLeaveBalance + LM.NoOfDaysAllowedInYear;
                            //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 END
                            //ADDED BY SHRADDHA ON 10 MAY 2017 START
                            Opening.LeaveType = LFYOpening.LeaveType;
                            Opening.ApplicableEffectiveDate = LFYApplicableFrom;
                            //ADDED BY SHRADDHA ON 10 MAY 2017 END
                            WetosDB.LeaveCredits.Add(Opening); // NEW CREDIT ENTRY FY
                            WetosDB.SaveChanges();

                            // Update leave balance
                            LeaveBalance LeaveBalances = WetosDB.LeaveBalances.Where(a => a.EmployeeId == EmpId).FirstOrDefault();
                            LeaveBalances.PreviousBalance = 0;//LeaveBalances.CurrentBalance;
                            //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 START
                            //LeaveBalances.CurrentBalance = LFYLeaveBalance + LM.MaxNoOfTimesAllowedInYear;
                            LeaveBalances.CurrentBalance = LFYLeaveBalance;
                            //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 END
                            WetosDB.SaveChanges();

                            // get data fro new fy start
                            // Updated by Rajas on 1 AUGUST 2017 to handle null value START
                            OpenCarryForward = Opening.CarryForward == null ? 0 : Opening.CarryForward.Value;
                            OpenOpenBal = Opening.OpeningBalance == null ? 0 : Opening.OpeningBalance.Value;
                            // Updated by Rajas on 1 AUGUST 2017 to handle null value END
                            LeaveAllowed = OpenCarryForward;
                            LeaveBalance = OpenCarryForward;

                            DateTime ApplicableFrom = Opening.ApplicableEffectiveDate == null ? FYLA.StartDate : Opening.ApplicableEffectiveDate.Value;

                            if (LM.LeaveCreditTypeId != null)
                            {
                                if (LM.LeaveCreditTypeId == 2)
                                {
                                    int count = 1;
                                    for (int i = 1; ; i++)
                                    {
                                        if (ApplicableFrom.AddMonths(i) <= LeaveAsonDate)
                                        {
                                            count++;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }

                                    LeaveAllowed = Convert.ToDouble(OpenCarryForward + (count * LM.MaxNoOfLeavesAllowedInMonth));
                                }
                                else
                                {
                                    //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 START
                                    //LeaveAllowed = Convert.ToDouble(LM.MaxNoOfTimesAllowedInYear);
                                    LeaveAllowed = Convert.ToDouble(LM.NoOfDaysAllowedInYear);
                                    //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 END

                                }
                            }
                            else
                            {

                                //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 START
                                //LeaveAllowed = Convert.ToDouble(LM.MaxNoOfTimesAllowedInYear);
                                LeaveAllowed = Convert.ToDouble(LM.NoOfDaysAllowedInYear);
                                //CODE CHANGED BY SHRADDHA ON 18 JULY 2017 END

                            }


                            // TOTAL SANCTIONED 
                            //TotalLeaveSanctioned = GetAppliedDays(WetosDB, ApplicableFrom, LeaveAsonDate, EmpId);
                            TotalLeaveSanctioned = GetAppliedDaysTillMonthEnd(WetosDB, ApplicableFrom, LeaveAsonDate, EmpId, LM.LeaveName); // MODIFIED BY MSJ ON 21 JULY 2017

                            //double TotalAppliedDays = 0.0;
                            //foreach (LeaveApplication LeaveTypeObj in LeaveAppList)
                            //{

                            //    TotalAppliedDays = TotalAppliedDays + LeaveTypeObj.ActualDays.Value;
                            //}

                            LeaveBalance = LeaveAllowed - TotalLeaveSanctioned;
                            // LeaveModelObj.
                            //}
                            // get data fro new fy END
                        }
                    }
                    else
                    {
                        // Error is handled 

                    }
                }
                // ADDED BY MSJ ON 08 MAY 2017 END


                LeaveBalance EmpoyeeLeaveBalanceObj = WetosDB.LeaveBalances.Where(a => a.EmployeeId == EmpId
                    && a.LeaveType.ToUpper().Trim() == LM.LeaveCode.ToUpper().Trim()).FirstOrDefault();

                if (EmpoyeeLeaveBalanceObj != null)
                {
                    return LeaveBalance;// LeaveBalanceList;
                    // SP_LeaveTableData_Result LeaveBalanceObj = new SP_LeaveTableData_Result();

                    //LeaveModelObj.LeaveType = EmpoyeeLeaveBalanceObj.LeaveType.Trim();
                    //LeaveModelObj.LeaveAllowed = Convert.ToDecimal(LeaveAllowed); // ActualDays.CurrentBalance;
                    //LeaveModelObj.CurrentBalance = LeaveBalance; // ActualDays.CurrentBalance;
                    //LeaveModelObj.LeaveUsed = TotalLeaveSanctioned; // ActualDays.LeaveUsed;
                    //LeaveModelObj.OpeningBalance = OpenOpenBal; // Opening.OpeningBalance;

                    // LeaveBalanceList.Add(LeaveBalanceObj);
                }
            }

            return 0; // LeaveBalance;// LeaveBalanceList;


            #endregion
        }

        #region GENERATE HR LETTERS CODE ADDED BY SHRADDHA ON 20 JULY 2017

        /// <summary>
        /// Added by Rajas on 19 APRIL 2017
        /// NOTE :
        /// Generate Appointment letter, Confirm letter(both) and Agreement. 
        /// Letters generated on edit POST
        /// Modification pending.
        /// </summary>
        /// <param name="EmployeeId"></param>
        /// <returns></returns>
        public ActionResult AppointmentLetter(int EmployeeId)
        {
            Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == EmployeeId).FirstOrDefault();

            string DocumentFileName = string.Empty;

            UploadDocument UploadDocumentTblObj = WetosDB.UploadDocuments.Where(a => a.EmployeeID == EmployeeId && a.DocContentType == "Appointment_Letter").FirstOrDefault();

            if (UploadDocumentTblObj == null)
            {
                // GENERATE PDF
                AppointmentLetterPrintFile(EmployeeId, ref DocumentFileName);

                UploadDocumentTblObj = new UploadDocument();

                UploadDocumentTblObj.EmployeeID = EmployeeId;

                UploadDocumentTblObj.DocContentType = "Appointment_Letter";

                UploadDocumentTblObj.DocDescription = "System Generated";

                UploadDocumentTblObj.DocType = ".pdf";

                UploadDocumentTblObj.DocFolder = "Employee_Official_Documents";

                UploadDocumentTblObj.FileName = DocumentFileName;

                WetosDB.UploadDocuments.Add(UploadDocumentTblObj);

                WetosDB.SaveChanges();

                AddAuditTrail("System generated Appointment_Letter for " + EmployeeObj.EmployeeCode + " | " + EmployeeObj.FirstName + EmployeeObj.LastName);

            }
            else
            {
                Information("Appointment_Letter is already generated.");

                AddAuditTrail("Appointment_Letter is already generated for " + EmployeeObj.EmployeeCode + " | " + EmployeeObj.FirstName + EmployeeObj.LastName);
            }

            return null;
        }

        /// <summary>
        /// Added by Rajas on 20 APRIL 2017
        /// NOTE :
        /// Generate Confirm letter with increment
        /// Letters generated on edit POST
        /// Modification pending.
        /// </summary>
        /// <param name="EmployeeId"></param>
        /// <returns></returns>
        public ActionResult ConfirmLetterWithInc(int EmployeeId)
        {
            Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == EmployeeId).FirstOrDefault();

            string DocumentFileName = string.Empty;

            UploadDocument UploadDocumentTblObj = WetosDB.UploadDocuments.Where(a => a.EmployeeID == EmployeeId && a.DocContentType == "Confirm_Letter_With_Increment").FirstOrDefault();

            if (UploadDocumentTblObj == null)
            {
                // GENERATE PDF
                ConfirmationLetterWithIncrementPrintFile(EmployeeId, ref DocumentFileName);

                UploadDocumentTblObj = new UploadDocument();

                UploadDocumentTblObj.EmployeeID = EmployeeId;

                UploadDocumentTblObj.DocContentType = "Confirm_Letter_With_Increment";

                UploadDocumentTblObj.DocDescription = "System Generated";

                UploadDocumentTblObj.DocType = ".pdf";

                UploadDocumentTblObj.DocFolder = "Employee_Official_Documents";

                UploadDocumentTblObj.FileName = DocumentFileName;

                WetosDB.UploadDocuments.Add(UploadDocumentTblObj);

                WetosDB.SaveChanges();

                AddAuditTrail("System generated Confirm_Letter_With_Increment for " + EmployeeObj.EmployeeCode + " | " + EmployeeObj.FirstName + EmployeeObj.LastName);

            }
            else
            {
                Information("Confirm_Letter_With_Increment is already generated.");

                AddAuditTrail("Confirm_Letter_With_Increment is already generated for " + EmployeeObj.EmployeeCode + " | " + EmployeeObj.FirstName + EmployeeObj.LastName);
            }

            return null;
        }

        /// <summary>
        /// Added by Rajas on 20 APRIL 2017
        /// NOTE :
        /// Generate Confirm letter without increment
        /// Letters generated on edit POST
        /// Modification pending.
        /// </summary>
        /// <param name="EmployeeId"></param>
        /// <returns></returns>
        public ActionResult ConfirmLetterWithoutInc(int EmployeeId)
        {
            Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == EmployeeId).FirstOrDefault();

            string DocumentFileName = string.Empty;

            UploadDocument UploadDocumentTblObj = WetosDB.UploadDocuments.Where(a => a.EmployeeID == EmployeeId && a.DocContentType == "Confirm_Letter_Without_Increment").FirstOrDefault();

            if (UploadDocumentTblObj == null)
            {
                // GENERATE PDF
                ConfirmationLetterWithoutIncrementPrintFile(EmployeeId, ref DocumentFileName);

                UploadDocumentTblObj = new UploadDocument();

                UploadDocumentTblObj.EmployeeID = EmployeeId;

                UploadDocumentTblObj.DocContentType = "Confirm_Letter_Without_Increment";

                UploadDocumentTblObj.DocDescription = "System Generated";

                UploadDocumentTblObj.DocType = ".pdf";

                UploadDocumentTblObj.DocFolder = "Employee_Official_Documents";

                UploadDocumentTblObj.FileName = DocumentFileName;

                WetosDB.UploadDocuments.Add(UploadDocumentTblObj);

                WetosDB.SaveChanges();

                AddAuditTrail("System generated Confirm_Letter_Without_Increment for " + EmployeeObj.EmployeeCode + " | " + EmployeeObj.FirstName + EmployeeObj.LastName);

            }
            else
            {
                Information("Confirm_Letter_Without_Increment is already generated.");

                AddAuditTrail("Confirm_Letter_Without_Increment is already generated for " + EmployeeObj.EmployeeCode + " | " + EmployeeObj.FirstName + EmployeeObj.LastName);
            }

            return null;
        }

        /// <summary>
        /// Added by Rajas on 20 APRIL 2017
        /// NOTE :
        /// Generate Service Agreement
        /// Letters generated on edit POST
        /// Modification pending.
        /// </summary>
        /// <param name="EmployeeId"></param>
        /// <returns></returns>
        public ActionResult ServiceAgreement(int EmployeeId)
        {
            Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == EmployeeId).FirstOrDefault();

            string DocumentFileName = string.Empty;

            UploadDocument UploadDocumentTblObj = WetosDB.UploadDocuments.Where(a => a.EmployeeID == EmployeeId && a.DocContentType == "Service_Agreement_Bond").FirstOrDefault();

            if (UploadDocumentTblObj == null)
            {
                // GENERATE PDF
                ServiceAgreementBond(EmployeeId, ref DocumentFileName);

                UploadDocumentTblObj = new UploadDocument();

                UploadDocumentTblObj.EmployeeID = EmployeeId;

                UploadDocumentTblObj.DocContentType = "Service_Agreement_Bond";

                UploadDocumentTblObj.DocDescription = "System Generated";

                UploadDocumentTblObj.DocType = ".pdf";

                UploadDocumentTblObj.DocFolder = "Employee_Official_Documents";

                UploadDocumentTblObj.FileName = DocumentFileName;

                WetosDB.UploadDocuments.Add(UploadDocumentTblObj);

                WetosDB.SaveChanges();

                AddAuditTrail("System generated Service_Agreement_Bond for " + EmployeeObj.EmployeeCode + " | " + EmployeeObj.FirstName + EmployeeObj.LastName);

            }
            else
            {
                Information("Service_Agreement_Bond is already generated.");

                AddAuditTrail("Service_Agreement_Bond already generated for " + EmployeeObj.EmployeeCode + " | " + EmployeeObj.FirstName + EmployeeObj.LastName);
            }

            return null;
        }

        #region COMMENTED CODE
        /// <summary>
        /// Added by Rajas on 19 APRIL 2017
        /// </summary>
        /// <param name="EmployeeModelObj"></param>
        /// <param name="Fc"></param>
        /// <returns></returns>
        //public bool HRLetters(EmployeeModel EmployeeModelObj, FormCollection Fc)
        //{
        //    bool ReturnStatus = false;

        //    string PdfFileName = string.Empty;

        //    try
        //    {
        //        if (EmployeeModelObj.JoiningDate == null)
        //        {
        //            AppointmentLetterPrintFile(EmployeeModelObj, Fc);
        //        }

        //        if (EmployeeModelObj.JoiningDate != null && EmployeeModelObj.ConfirmDate == null)
        //        {
        //            ConfirmationLetterWithoutIncrementPrintFile(EmployeeModelObj, Fc);

        //            ConfirmationLetterWithIncrementPrintFile(EmployeeModelObj, Fc);

        //            ServiceAgreementBond(EmployeeModelObj, Fc);
        //        }

        //        if (EmployeeModelObj.Leavingdate != null)
        //        {
        //            RelievingLetterPrintFile(EmployeeModelObj, Fc);

        //            ExperienceLetterPrintFile(EmployeeModelObj, Fc);
        //        }

        //        return ReturnStatus = true;
        //        //return File(PdfFileName, "application/pdf");
        //    }
        //    catch (System.Exception ex)
        //    {
        //        AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

        //        return ReturnStatus;
        //    }
        //}
        #endregion

        public string ExperienceLetterPrintFile(EmployeeModel EmployeeModelObj, FormCollection Fc)
        {
            string PdfFileName = string.Empty;

            try
            {
                //int EmpId = Convert.ToInt32(Session["EmployeeNo"]);

                // ------------------------------------------------REPORT RDLC TimeCardReport START-------------------------------------------
                //// PRINT START
                DataTable DT = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                SqlDataAdapter DA = new SqlDataAdapter(" select * from Employee where EmployeeId= " + EmployeeModelObj.EmployeeId, con);



                DA.Fill(DT);


                Warning[] warnings;
                //string[] streamids;
                string mimeType;
                string encoding;
                //string filenameExtension;
                //string filePath;

                LocalReport localReport = new LocalReport();
                // localReport.ReportPath = Server.MapPath("~/Reports/AppointmentLetter.rdlc");
                localReport.ReportPath = Server.MapPath("~/Reports/ExperienceLetter.rdlc");

                //ReportDataSource reportDataSource = new ReportDataSource("Customers", Customers.GetAllCustomers());
                localReport.DataSources.Clear();
                localReport.DataSources.Add(new ReportDataSource("EmployeeData", DT));

                localReport.Refresh();
                string reportType = "PDF";
                //string mimeType;
                //string encoding;
                string fileNameExtension;

                SP_EmployeeDetailsOnEmployeeMaster_Result EmployeeDetails = WetosDB.SP_EmployeeDetailsOnEmployeeMaster().Where(a => a.EmployeeId == EmployeeModelObj.EmployeeId).FirstOrDefault();

                string DesignationName = string.Empty;

                string DepartmentName = string.Empty;

                string CompanyName = string.Empty;

                string FileNameEx = string.Empty;

                if (EmployeeDetails != null)
                {
                    DesignationName = EmployeeDetails.DesignationName;

                    DepartmentName = EmployeeDetails.DepartmentName;

                    CompanyName = EmployeeDetails.CompanyName;

                    FileNameEx = EmployeeDetails.EmployeeCode; // +" | " + EmployeeDetails.FirstName;
                }

                ReportParameter ReportParameter1 = new ReportParameter("ReportParameter1", DesignationName);
                ReportParameter ReportParameter2 = new ReportParameter("ReportParameter2", DepartmentName);
                ReportParameter ReportParameter3 = new ReportParameter("ReportParameter3", CompanyName);

                localReport.SetParameters(new ReportParameter[] { ReportParameter1, ReportParameter2, ReportParameter3 });

                localReport.EnableHyperlinks = true;

                //The DeviceInfo settings should be changed based on the reportType
                //http://msdn2.microsoft.com/en-us/library/ms155397.aspx
                string deviceInfo =

                "<DeviceInfo>" +
                "  <OutputFormat>PDF</OutputFormat>" +
                "</DeviceInfo>";

                //Warning[] warnings;
                string[] streams;
                byte[] renderedBytes;

                //Render the report

                renderedBytes = localReport.Render(
                    reportType,
                    deviceInfo,
                    out mimeType,
                    out encoding,
                    out fileNameExtension,
                    out streams,
                    out warnings);

                //FileStream fs = new FileStream(filePath, FileMode.Create);

                // File name updated by Rajas on 19 JAN 2017

                PdfFileName = Server.MapPath("~/User_Data/Employee_Official_Documents/") + "ExperienceLetter_" + FileNameEx + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf";

                using (FileStream fs = new FileStream(PdfFileName, FileMode.Create))
                {
                    fs.Write(renderedBytes, 0, renderedBytes.Length);
                }

                //Response.AddHeader("Content-Disposition", "inline; filename=" + "TimeCardReport" + ".pdf;");

                Response.AddHeader("Content-Disposition", "attachment; filename=" + "ExperienceLetter" + ".pdf;"); // Updated by Rajas on 19 JAN 2017
                //string PdfUrl = "download/" + "ExperienceLetter" + ".pdf";


            }
            catch (System.Exception)
            {
                //throw;
            }

            return PdfFileName;
        }

        public string ConfirmationLetterWithoutIncrementPrintFile(int EmployeeId, ref string DocumentFileName) //(EmployeeModel EmployeeModelObj, FormCollection Fc)
        {
            string PdfFileName = string.Empty;

            try
            {
                // Added by Rajas on 19 JAN 2017
                //int EmpId = Convert.ToInt32(Session["EmployeeNo"]);

                // ------------------------------------------------REPORT RDLC TimeCardReport START-------------------------------------------
                //// PRINT START
                DataTable DT = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                SqlDataAdapter DA = new SqlDataAdapter(" select * from Employee where EmployeeId= " + EmployeeId, con);


                DA.Fill(DT);


                Warning[] warnings;
                //string[] streamids;
                string mimeType;
                string encoding;
                //string filenameExtension;
                //string filePath;

                LocalReport localReport = new LocalReport();
                // localReport.ReportPath = Server.MapPath("~/Reports/AppointmentLetter.rdlc");
                localReport.ReportPath = Server.MapPath("~/Reports/ConfirmationLetterWithoutIncrement.rdlc");

                //ReportDataSource reportDataSource = new ReportDataSource("Customers", Customers.GetAllCustomers());
                localReport.DataSources.Clear();
                localReport.DataSources.Add(new ReportDataSource("EmployeeData", DT));

                localReport.Refresh();
                string reportType = "PDF";
                //string mimeType;
                //string encoding;
                string fileNameExtension;

                SP_EmployeeDetailsOnEmployeeMaster_Result EmployeeDetails = WetosDB.SP_EmployeeDetailsOnEmployeeMaster().Where(a => a.EmployeeId == EmployeeId).FirstOrDefault();

                string DesignationName = string.Empty;

                string DepartmentName = string.Empty;

                string CompanyName = string.Empty;

                string FileNameEx = string.Empty;

                if (EmployeeDetails != null)
                {
                    DesignationName = EmployeeDetails.DesignationName;

                    DepartmentName = EmployeeDetails.DepartmentName;

                    CompanyName = EmployeeDetails.CompanyName;

                    FileNameEx = EmployeeDetails.EmployeeCode; // +" | " + EmployeeDetails.FirstName;
                }

                ReportParameter ReportParameter1 = new ReportParameter("ReportParameter1", DesignationName);
                ReportParameter ReportParameter2 = new ReportParameter("ReportParameter2", DepartmentName);
                ReportParameter ReportParameter3 = new ReportParameter("ReportParameter3", CompanyName);

                localReport.SetParameters(new ReportParameter[] { ReportParameter1, ReportParameter2, ReportParameter3 });

                localReport.EnableHyperlinks = true;

                //The DeviceInfo settings should be changed based on the reportType
                //http://msdn2.microsoft.com/en-us/library/ms155397.aspx
                string deviceInfo =

                "<DeviceInfo>" +
                "  <OutputFormat>PDF</OutputFormat>" +
                "</DeviceInfo>";

                //Warning[] warnings;
                string[] streams;
                byte[] renderedBytes;

                //Render the report

                renderedBytes = localReport.Render(
                    reportType,
                    deviceInfo,
                    out mimeType,
                    out encoding,
                    out fileNameExtension,
                    out streams,
                    out warnings);

                //FileStream fs = new FileStream(filePath, FileMode.Create);

                // File name updated by Rajas on 19 JAN 2017

                // FILE NAME ONLY
                DocumentFileName = "ConfirmationLetterWithoutIncrement_" + FileNameEx + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf";

                PdfFileName = Server.MapPath("~/User_Data/Employee_Official_Documents/") + DocumentFileName;

                using (FileStream fs = new FileStream(PdfFileName, FileMode.Create))
                {
                    fs.Write(renderedBytes, 0, renderedBytes.Length);
                }

                //Response.AddHeader("Content-Disposition", "inline; filename=" + "TimeCardReport" + ".pdf;");

                Response.AddHeader("Content-Disposition", "attachment; filename=" + "ConfirmationLetterWithoutIncrement" + ".pdf;"); // Updated by Rajas on 19 JAN 2017
                //string PdfUrl = "Employee_Official_Documents/" + "ConfirmationLetterWithoutIncrement" + ".pdf";


            }
            catch (System.Exception)
            {
                //throw;
            }

            return PdfFileName;
        }

        public string ConfirmationLetterWithIncrementPrintFile(int EmployeeId, ref string DocumentFileName) //(EmployeeModel EmployeeModelObj, FormCollection Fc)
        {
            string PdfFileName = string.Empty;

            try
            {
                // Added by Rajas on 19 JAN 2017
                //int EmpId = Convert.ToInt32(Session["EmployeeNo"]);

                // ------------------------------------------------REPORT RDLC TimeCardReport START-------------------------------------------
                //// PRINT START
                DataTable DT = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                SqlDataAdapter DA = new SqlDataAdapter(" select * from Employee where EmployeeId= " + EmployeeId, con);



                DA.Fill(DT);


                Warning[] warnings;
                //string[] streamids;
                string mimeType;
                string encoding;
                //string filenameExtension;
                //string filePath;

                LocalReport localReport = new LocalReport();
                // localReport.ReportPath = Server.MapPath("~/Reports/AppointmentLetter.rdlc");
                localReport.ReportPath = Server.MapPath("~/Reports/ConfirmationLetterWithIncrement.rdlc");

                //ReportDataSource reportDataSource = new ReportDataSource("Customers", Customers.GetAllCustomers());
                localReport.DataSources.Clear();
                localReport.DataSources.Add(new ReportDataSource("EmployeeData", DT));

                localReport.Refresh();
                string reportType = "PDF";
                //string mimeType;
                //string encoding;
                string fileNameExtension;

                SP_EmployeeDetailsOnEmployeeMaster_Result EmployeeDetails = WetosDB.SP_EmployeeDetailsOnEmployeeMaster().Where(a => a.EmployeeId == EmployeeId).FirstOrDefault();

                string DesignationName = string.Empty;

                string DepartmentName = string.Empty;

                string CompanyName = string.Empty;

                string FileNameEx = string.Empty;

                if (EmployeeDetails != null)
                {
                    DesignationName = EmployeeDetails.DesignationName;

                    DepartmentName = EmployeeDetails.DepartmentName;

                    CompanyName = EmployeeDetails.CompanyName;

                    FileNameEx = EmployeeDetails.EmployeeCode; // +" | " + EmployeeDetails.FirstName;
                }

                ReportParameter ReportParameter1 = new ReportParameter("ReportParameter1", DesignationName);
                ReportParameter ReportParameter2 = new ReportParameter("ReportParameter2", DepartmentName);
                ReportParameter ReportParameter3 = new ReportParameter("ReportParameter3", CompanyName);

                localReport.SetParameters(new ReportParameter[] { ReportParameter1, ReportParameter2, ReportParameter3 });

                localReport.EnableHyperlinks = true;

                //The DeviceInfo settings should be changed based on the reportType
                //http://msdn2.microsoft.com/en-us/library/ms155397.aspx
                string deviceInfo =

                "<DeviceInfo>" +
                "  <OutputFormat>PDF</OutputFormat>" +
                "</DeviceInfo>";

                //Warning[] warnings;
                string[] streams;
                byte[] renderedBytes;

                //Render the report

                renderedBytes = localReport.Render(
                    reportType,
                    deviceInfo,
                    out mimeType,
                    out encoding,
                    out fileNameExtension,
                    out streams,
                    out warnings);

                //FileStream fs = new FileStream(filePath, FileMode.Create);

                // File name updated by Rajas on 19 JAN 2017

                //FILE NAME ONLY 

                DocumentFileName = "ConfirmationLetter_" + FileNameEx + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf";

                PdfFileName = Server.MapPath("~/User_Data/Employee_Official_Documents/") + DocumentFileName;
                using (FileStream fs = new FileStream(PdfFileName, FileMode.Create))
                {
                    fs.Write(renderedBytes, 0, renderedBytes.Length);
                }

                //Response.AddHeader("Content-Disposition", "inline; filename=" + "TimeCardReport" + ".pdf;");

                Response.AddHeader("Content-Disposition", "attachment; filename=" + "ConfirmationLetter" + ".pdf;"); // Updated by Rajas on 19 JAN 2017
                //string PdfUrl = "Employee_Official_Documents/" + "ConfirmationLetter" + ".pdf";
            }
            catch (System.Exception)
            {
                //throw;
            }

            return PdfFileName;
        }

        public string RelievingLetterPrintFile(EmployeeModel EmployeeModelObj, FormCollection Fc)
        {
            string PdfFileName = string.Empty;

            try
            {
                // Added by Rajas on 19 JAN 2017
                int EmpId = Convert.ToInt32(Session["EmployeeNo"]);

                // ------------------------------------------------REPORT RDLC TimeCardReport START-------------------------------------------
                //// PRINT START
                DataTable DT = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                SqlDataAdapter DA = new SqlDataAdapter(" select * from Employee where EmployeeId= " + EmployeeModelObj.EmployeeId, con);



                DA.Fill(DT);


                Warning[] warnings;
                //string[] streamids;
                string mimeType;
                string encoding;
                //string filenameExtension;
                //string filePath;

                LocalReport localReport = new LocalReport();
                // localReport.ReportPath = Server.MapPath("~/Reports/AppointmentLetter.rdlc");
                localReport.ReportPath = Server.MapPath("~/Reports/RelievingLetter.rdlc");

                //ReportDataSource reportDataSource = new ReportDataSource("Customers", Customers.GetAllCustomers());
                localReport.DataSources.Clear();
                localReport.DataSources.Add(new ReportDataSource("EmployeeData", DT));

                localReport.Refresh();
                string reportType = "PDF";
                //string mimeType;
                //string encoding;
                string fileNameExtension;

                SP_EmployeeDetailsOnEmployeeMaster_Result EmployeeDetails = WetosDB.SP_EmployeeDetailsOnEmployeeMaster().Where(a => a.EmployeeId == EmployeeModelObj.EmployeeId).FirstOrDefault();

                string DesignationName = string.Empty;

                string DepartmentName = string.Empty;

                string CompanyName = string.Empty;

                string FileNameEx = string.Empty;

                if (EmployeeDetails != null)
                {
                    DesignationName = EmployeeDetails.DesignationName;

                    DepartmentName = EmployeeDetails.DepartmentName;

                    CompanyName = EmployeeDetails.CompanyName;

                    FileNameEx = EmployeeDetails.EmployeeCode + " | " + EmployeeDetails.FirstName;

                }

                ReportParameter ReportParameter1 = new ReportParameter("ReportParameter1", DesignationName);
                ReportParameter ReportParameter2 = new ReportParameter("ReportParameter2", DepartmentName);
                ReportParameter ReportParameter3 = new ReportParameter("ReportParameter3", CompanyName);

                localReport.SetParameters(new ReportParameter[] { ReportParameter1, ReportParameter2, ReportParameter3 });

                localReport.EnableHyperlinks = true;

                //The DeviceInfo settings should be changed based on the reportType
                //http://msdn2.microsoft.com/en-us/library/ms155397.aspx
                string deviceInfo =

                "<DeviceInfo>" +
                "  <OutputFormat>PDF</OutputFormat>" +
                "</DeviceInfo>";

                //Warning[] warnings;
                string[] streams;
                byte[] renderedBytes;

                //Render the report

                renderedBytes = localReport.Render(
                    reportType,
                    deviceInfo,
                    out mimeType,
                    out encoding,
                    out fileNameExtension,
                    out streams,
                    out warnings);

                //FileStream fs = new FileStream(filePath, FileMode.Create);

                // File name updated by Rajas on 19 JAN 2017

                PdfFileName = Server.MapPath("~/User_Data/Employee_Official_Documents/") + "RelievingLetter_" + FileNameEx + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf";
                using (FileStream fs = new FileStream(PdfFileName, FileMode.Create))
                {
                    fs.Write(renderedBytes, 0, renderedBytes.Length);
                }

                //Response.AddHeader("Content-Disposition", "inline; filename=" + "TimeCardReport" + ".pdf;");

                Response.AddHeader("Content-Disposition", "attachment; filename=" + "RelievingLetter" + ".pdf;"); // Updated by Rajas on 19 JAN 2017
                //string PdfUrl = "Employee_Official_Documents/" + "RelievingLetter" + ".pdf";


            }
            catch (System.Exception)
            {
                //throw;
            }

            return PdfFileName;
        }

        public string AppointmentLetterPrintFile(int EmployeeId, ref string DocumentFileName) // (EmployeeModel EmployeeModelObj, FormCollection Fc)
        {
            string PdfFileName = string.Empty;

            try
            {
                // Added by Rajas on 19 JAN 2017
                //int EmpId = Convert.ToInt32(Session["EmployeeNo"]);

                // ------------------------------------------------REPORT RDLC TimeCardReport START-------------------------------------------
                //// PRINT START
                DataTable DT = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                SqlDataAdapter DA = new SqlDataAdapter(" select * from Employee where EmployeeId= " + EmployeeId, con);



                DA.Fill(DT);


                Warning[] warnings;
                //string[] streamids;
                string mimeType;
                string encoding;
                //string filenameExtension;
                //string filePath;

                LocalReport localReport = new LocalReport();
                // localReport.ReportPath = Server.MapPath("~/Reports/AppointmentLetter.rdlc");
                localReport.ReportPath = Server.MapPath("~/Reports/AppointmentLetter.rdlc");

                //ReportDataSource reportDataSource = new ReportDataSource("Customers", Customers.GetAllCustomers());
                localReport.DataSources.Clear();
                localReport.DataSources.Add(new ReportDataSource("EmployeeData", DT));

                localReport.Refresh();
                string reportType = "PDF";
                //string mimeType;
                //string encoding;
                string fileNameExtension;

                SP_EmployeeDetailsOnEmployeeMaster_Result EmployeeDetails = WetosDB.SP_EmployeeDetailsOnEmployeeMaster().Where(a => a.EmployeeId == EmployeeId).FirstOrDefault();

                string DesignationName = string.Empty;

                string DepartmentName = string.Empty;

                string CompanyName = string.Empty;

                string FileNameEx = string.Empty;


                if (EmployeeDetails != null)
                {
                    DesignationName = EmployeeDetails.DesignationName;

                    DepartmentName = EmployeeDetails.DepartmentName;

                    CompanyName = EmployeeDetails.CompanyName;

                    FileNameEx = EmployeeDetails.EmployeeCode; // +" | " + EmployeeDetails.FirstName;
                }

                ReportParameter ReportParameter1 = new ReportParameter("ReportParameter1", DesignationName);
                ReportParameter ReportParameter2 = new ReportParameter("ReportParameter2", DepartmentName);
                ReportParameter ReportParameter3 = new ReportParameter("ReportParameter3", CompanyName);

                localReport.SetParameters(new ReportParameter[] { ReportParameter1, ReportParameter2, ReportParameter3 });

                localReport.EnableHyperlinks = true;

                //The DeviceInfo settings should be changed based on the reportType
                //http://msdn2.microsoft.com/en-us/library/ms155397.aspx
                string deviceInfo =

                "<DeviceInfo>" +
                "  <OutputFormat>PDF</OutputFormat>" +
                "</DeviceInfo>";

                //Warning[] warnings;
                string[] streams;
                byte[] renderedBytes;

                //Render the report

                renderedBytes = localReport.Render(
                    reportType,
                    deviceInfo,
                    out mimeType,
                    out encoding,
                    out fileNameExtension,
                    out streams,
                    out warnings);

                //FileStream fs = new FileStream(filePath, FileMode.Create);

                // File name updated by Rajas on 19 JAN 2017

                // File name only
                DocumentFileName = "AppointmentLetter_" + FileNameEx + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf";

                PdfFileName = Server.MapPath("~/User_Data/Employee_Official_Documents/") + DocumentFileName;

                using (FileStream fs = new FileStream(PdfFileName, FileMode.Create))
                {
                    fs.Write(renderedBytes, 0, renderedBytes.Length);
                }

                //Response.AddHeader("Content-Disposition", "inline; filename=" + "TimeCardReport" + ".pdf;");

                Response.AddHeader("Content-Disposition", "attachment; filename=" + "AppointmentLetter" + ".pdf;"); // Updated by Rajas on 19 JAN 2017



            }
            catch (System.Exception)
            {

            }

            return PdfFileName;
        }

        public string ServiceAgreementBond(int EmployeeId, ref string DocumentFileName) //(EmployeeModel EmployeeModelObj, FormCollection Fc)
        {
            string PdfFileName = string.Empty;

            try
            {
                // Added by Rajas on 19 JAN 2017
                //int EmpId = Convert.ToInt32(Session["EmployeeNo"]);

                // ------------------------------------------------REPORT RDLC TimeCardReport START-------------------------------------------
                //// PRINT START
                DataTable DT = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                SqlDataAdapter DA = new SqlDataAdapter(" select * from Employee where EmployeeId= " + EmployeeId, con);


                DA.Fill(DT);


                Warning[] warnings;
                //string[] streamids;
                string mimeType;
                string encoding;
                //string filenameExtension;
                //string filePath;

                LocalReport localReport = new LocalReport();
                // localReport.ReportPath = Server.MapPath("~/Reports/AppointmentLetter.rdlc");
                localReport.ReportPath = Server.MapPath("~/Reports/ServiceAgreementBond.rdlc");

                //ReportDataSource reportDataSource = new ReportDataSource("Customers", Customers.GetAllCustomers());
                localReport.DataSources.Clear();
                localReport.DataSources.Add(new ReportDataSource("EmployeeData", DT));

                localReport.Refresh();
                string reportType = "PDF";
                //string mimeType;
                //string encoding;
                string fileNameExtension;

                SP_EmployeeDetailsOnEmployeeMaster_Result EmployeeDetails = WetosDB.SP_EmployeeDetailsOnEmployeeMaster().Where(a => a.EmployeeId == EmployeeId).FirstOrDefault();

                string DesignationName = string.Empty;

                string DepartmentName = string.Empty;

                string CompanyName = string.Empty;

                string FileNameEx = string.Empty;

                // Added by Rajas on  20 APRIL 2017 START
                string CompanyAddresss = string.Empty;

                string Address = WetosDB.Companies.Where(a => a.CompanyId == EmployeeDetails.CompanyId).Select(a => a.Address1).FirstOrDefault();

                if (Address != null)
                {
                    CompanyAddresss = Address;
                }
                // Added by Rajas on  20 APRIL 2017 END

                if (EmployeeDetails != null)
                {
                    DesignationName = EmployeeDetails.DesignationName;

                    DepartmentName = EmployeeDetails.DepartmentName;

                    CompanyName = EmployeeDetails.CompanyName;

                    FileNameEx = EmployeeDetails.EmployeeCode;  //  + " | " + EmployeeDetails.FirstName;
                }

                ReportParameter ReportParameter1 = new ReportParameter("ReportParameter1", DesignationName);
                ReportParameter ReportParameter2 = new ReportParameter("ReportParameter2", DepartmentName);
                ReportParameter ReportParameter3 = new ReportParameter("ReportParameter3", CompanyName);
                ReportParameter ReportParameter4 = new ReportParameter("ReportParameter4", CompanyAddresss); // Added by Rajas on  20 APRIL 2017 as extra rpt parameter


                localReport.SetParameters(new ReportParameter[] { ReportParameter1, ReportParameter2, ReportParameter3, ReportParameter4 });

                localReport.EnableHyperlinks = true;

                //The DeviceInfo settings should be changed based on the reportType
                //http://msdn2.microsoft.com/en-us/library/ms155397.aspx
                string deviceInfo =

                "<DeviceInfo>" +
                "  <OutputFormat>PDF</OutputFormat>" +
                "</DeviceInfo>";

                //Warning[] warnings;
                string[] streams;
                byte[] renderedBytes;

                //Render the report

                renderedBytes = localReport.Render(
                    reportType,
                    deviceInfo,
                    out mimeType,
                    out encoding,
                    out fileNameExtension,
                    out streams,
                    out warnings);

                //FileStream fs = new FileStream(filePath, FileMode.Create);

                // File name updated by Rajas on 19 JAN 2017

                // ONLY FILE NAME
                DocumentFileName = "ServiceAgreementBond_" + FileNameEx + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf";

                PdfFileName = Server.MapPath("~/User_Data/Employee_Official_Documents/") + DocumentFileName;

                using (FileStream fs = new FileStream(PdfFileName, FileMode.Create))
                {
                    fs.Write(renderedBytes, 0, renderedBytes.Length);
                }

                //Response.AddHeader("Content-Disposition", "inline; filename=" + "TimeCardReport" + ".pdf;");

                Response.AddHeader("Content-Disposition", "attachment; filename=" + "ServiceAgreementBond" + ".pdf;"); // Updated by Rajas on 19 JAN 2017

            }
            catch (System.Exception)
            {

            }

            return PdfFileName;
        }

        #endregion
    }
}
