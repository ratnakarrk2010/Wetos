using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;
using WetosDB;
using WetosMVCMainApp.Models;
using System.Net.Mail;
using System.Threading;
using System.Web.Hosting;
using ClosedXML.Excel;
using System.Globalization;

namespace WetosMVC.Controllers
{
    [SessionExpire]
    [Authorize]
    public class WetosReportsController : BaseController
    {

        // GET: /WetosReports/

        // Master Report Start

        /// <summary>
        /// Populate Drop Down
        /// </summary>
        private void PopulateDropDown()
        {
            //CODE FOR DROPDOWN
            // code for company drop down list
            // Updated by Rajas on 6 MAY 2017 to get data which is not MarkedAsDelete

            #region COMPANY LIST CODE ADDED BY SHRADDHA ON 30 MAR 2018
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
            int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            //var CompanyName = WetosDB.Companies.Select(a => new { CompanyId = a.CompanyId, CompanyName = a.CompanyName }).ToList();
            var CompanyList = WetosDB.SP_GetCompanyList(EmployeeId).Select(m => new { Companyid = m.CompanyId, CompanyName = m.CompanyName }).ToList();
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
            #endregion

            ViewBag.CompanyNameList = new SelectList(CompanyList, "CompanyId", "CompanyName").ToList();

            //drop down for branch name list
            List<Branch> BranchList = new List<Branch>();
            ViewBag.BranchNameList = new SelectList(BranchList, "BranchId", "BranchName").ToList();


            // code for Department drop down list
            List<Department> DepartmentList = new List<Department>();
            ViewBag.DepartmentNameList = new SelectList(DepartmentList, "DepartmentId", "DepartmentName").ToList();

            //Drop down for employee
            var EmployeeNameList = new List<Employee>();
            ViewBag.EmployeeList = new SelectList(EmployeeNameList, "EmployeeId", "Name").ToList();

            //CODE ADDED BY SHRADDHA ON 15 FEB 2018 FOR EMPLOYEE TYPE START
            List<WetosDB.EmployeeType> EmployeeTypeList = WetosDB.EmployeeTypes.Where(a => a.MarkedAsDelete == null || a.MarkedAsDelete == 0).ToList();
            ViewBag.EmployeeTypeList = new SelectList(EmployeeTypeList, "EmployeeTypeId", "EmployeeTypeName").ToList();
            //CODE ADDED BY SHRADDHA ON 15 FEB 2018 FOR EMPLOYEE TYPE END


            // ADDED BY MSJ ON 01 FEB 2018 START
            var MonthList = WetosDB.DropdownDatas.Where(a => a.TypeId == 7).Select(a => new { Value = a.Value, Text = a.Text }).ToList();

            ViewBag.MonthListVB = new SelectList(MonthList, " Value", "Text").ToList();

            GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.CurrentFinancialYear).FirstOrDefault();
            if (GlobalSettingObj != null)
            {
                PopulateYearListVB();
            }
            else
            {
                var YearList = new List<FinancialYear>();
                ViewBag.YearListVB = new SelectList(YearList, " FinancialId", "FinancialName").ToList();

                Information("Inconsistent Financial Year data, Please verify and try again!");
            }
            // ADDED BY  MSJ ON 01 FEB 2018 END

        }

        /// <summary>
        /// Master Report Index
        /// </summary>
        /// <returns></returns>
        public ActionResult MasterReportIndex()
        {
            PopulateDropDown();

            MasterReportsModel abcd = new MasterReportsModel();

            int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);

            Employee LoggedInEmployee = WetosDB.Employees.Where(a => a.EmployeeId == EmployeeId).FirstOrDefault();

            UserRole ChechUserRole = WetosDB.UserRoles.Where(a => a.EmployeeId == EmployeeId).FirstOrDefault();

            //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO HANDLE NULL OBJECT REFERECE ERROR FOR LoggedInEmployee OBJ AND ChechUserRole OBJ START
            if (LoggedInEmployee == null)
            {
                LoggedInEmployee = new Employee();
            }
            if (ChechUserRole == null)
            {
                ChechUserRole = new UserRole();
            }
            //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO HANDLE NULL OBJECT REFERECE ERROR FOR LoggedInEmployee OBJ AND ChechUserRole OBJ END

            abcd.CompanyId = LoggedInEmployee.CompanyId;
            abcd.BranchId = LoggedInEmployee.BranchId;
            abcd.DepartmentId = LoggedInEmployee.DepartmentId;
            //abcd.EmployeeId = LoggedInEmployee.EmployeeId;
            //abcd.UserId = ChechUserRole.RoleTypeId;


            //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable START
            ExportToExcelAndPDFEnable();
            //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable END

            return View(abcd);
        }

        /// <summary>
        /// Master Report Index - POST
        /// </summary>
        /// <param name="ReportModel"></param>
        /// <param name="fc"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult MasterReportIndex(MasterReportsModel ReportModel, FormCollection fc)
        {
            //if (ModelState.IsValid)
            //{
            try
            {
                string PdfFileName = "";

                ReportModel.ReportFormat = Convert.ToInt32(fc["Export"]);

                switch (ReportModel.ReportsSelected)
                {
                    case "1":
                        string Report1 = "Branch Master Report";
                        ReportModel.ReportName = Report1;
                        PdfFileName = BranchMasterReport(ReportModel);
                        break;
                    case "2":
                        string Report2 = "Company Master Report";
                        ReportModel.ReportName = Report2;
                        PdfFileName = CompanyMasterReport(ReportModel);
                        break;
                    case "3":
                        string Report3 = "Department Master Report";
                        ReportModel.ReportName = Report3;
                        PdfFileName = DepartmentMasterReport(ReportModel);
                        break;
                    case "4":
                        string Report4 = "Designation Master Report";
                        ReportModel.ReportName = Report4;
                        PdfFileName = DesignationMasterReport(ReportModel);
                        break;
                    case "5":
                        string Report5 = "Employee Group Master Report";
                        ReportModel.ReportName = Report5;
                        PdfFileName = EmployeeGroupMasterReport(ReportModel);
                        break;
                    case "6":
                        string Report6 = "Grade Master Report";
                        ReportModel.ReportName = Report6;
                        PdfFileName = GradeMasterReport(ReportModel);
                        break;
                    case "7":
                        string Report7 = "Shift Rotation Master Report";
                        ReportModel.ReportName = Report7;
                        PdfFileName = ShiftRotationMasterReport(ReportModel);
                        break;
                    case "8":
                        string Report8 = "Shift Master Report";
                        ReportModel.ReportName = Report8;
                        PdfFileName = ShiftMasterReport(ReportModel);
                        break;
                    case "9":
                        string Report9 = "Employee Master Report";
                        ReportModel.ReportName = Report9;
                        PdfFileName = EmployeeMasterReport(ReportModel);
                        break;
                    case "10":
                        string Report10 = "Holiday Master Report";
                        ReportModel.ReportName = Report10;
                        PdfFileName = HolidayMasterReport(ReportModel);
                        break;
                    case "11":
                        string Report11 = "Family Details Report";
                        ReportModel.ReportName = Report11;
                        PdfFileName = FamilyDetailsReport(ReportModel);
                        break;
                    default:
                        break;
                    //}
                }

                //string path = Path.Combine(@"C:\path\to\files", fileName);

                // Added by Rajas on 23 DEC 2016 for handling null or empty PdfFileName START

                if (string.IsNullOrEmpty(PdfFileName))
                {
                    PopulateDropDown();
                    ModelState.AddModelError("Error", "Please try again");

                    //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable START
                    ExportToExcelAndPDFEnable();
                    //CODE. ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable END

                    return View(ReportModel);
                }
                else
                {
                    // Added by Rajas on 14 MARCh 2017
                    if (ReportModel.ReportFormat == 2)  // excel format
                    {
                        return File(PdfFileName, "application/xls");
                    }
                    else // PDF format
                    {
                        return File(PdfFileName, "application/pdf");
                    }

                }

                // Added by Rajas on 23 DEC 2016 for handling null or empty PdfFileName END

            }

            //else 
            catch (System.Exception ex)
            {
                PopulateDropDown();

                //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable START
                ExportToExcelAndPDFEnable();
                //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable END

                AddAuditTrail(ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));
                return View(ReportModel);
            }

            //return View();
        }

        // master report end

        /// <summary>
        /// Added by Rajas on 12 SEP 2017
        /// Reports to HR
        /// </summary>
        /// <returns></returns>
        public ActionResult DailyReportsIndexHR()
        {
            DailyReportsModel abcd = new DailyReportsModel();

            PopulateDropDown();

            //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable START
            ExportToExcelAndPDFEnable();
            //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable END
            return View(abcd);

        }

        /// <summary>
        /// Updated by Rajas on 12 SEP 2017
        /// For reports selection for logged in employee or team
        /// </summary>
        /// <returns></returns>
        public ActionResult DailyReportsIndex()
        {
            DailyReportsModel DailyReportsModelObj = new DailyReportsModel();
            try
            {
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);

                Employee LoggedInEmployee = WetosDB.Employees.Where(a => a.EmployeeId == EmployeeId).FirstOrDefault();

                UserRole ChechUserRole = WetosDB.UserRoles.Where(a => a.EmployeeId == EmployeeId).FirstOrDefault();

                //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO HANDLE NULL OBJECT REFERECE ERROR FOR LoggedInEmployee OBJ AND ChechUserRole OBJ START
                if (LoggedInEmployee == null)
                {
                    LoggedInEmployee = new Employee();
                }
                if (ChechUserRole == null)
                {
                    ChechUserRole = new UserRole();
                }
                //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO HANDLE NULL OBJECT REFERECE ERROR FOR LoggedInEmployee OBJ AND ChechUserRole OBJ END

                DailyReportsModelObj.CompanyId = LoggedInEmployee.CompanyId;
                DailyReportsModelObj.BranchId = LoggedInEmployee.BranchId;
                DailyReportsModelObj.DepartmentId = LoggedInEmployee.DepartmentId;
                DailyReportsModelObj.EmployeeId = LoggedInEmployee.EmployeeId;
                DailyReportsModelObj.UserId = ChechUserRole.RoleTypeId;



                #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //var EmployeeNameList = WetosDB.Employees.Where(a => a.ActiveFlag == null || a.ActiveFlag == true
                //&& a.CompanyId == LoggedInEmployee.CompanyId && a.BranchId == LoggedInEmployee.BranchId
                //&& a.EmployeeReportingId == LoggedInEmployee.EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
                var EmployeeNameList = WetosDB.SP_VwActiveEmployee(EmployeeId).Where(a => a.ActiveFlag == null || a.ActiveFlag == true
                        && a.CompanyId == LoggedInEmployee.CompanyId && a.BranchId == LoggedInEmployee.BranchId
                        && a.EmployeeReportingId == LoggedInEmployee.EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();

                // ADDED BY MSJ ON 20 MAY 2020
                if (EmployeeNameList.Where(a => a.EmployeeId == LoggedInEmployee.EmployeeId).FirstOrDefault() == null)
                {
                    // ADDED BY MSJ ON 15 MAY 2020
                    var SelfEmployeeNameList = WetosDB.SP_VwActiveEmployee(EmployeeId).Where(a => a.ActiveFlag == null || a.ActiveFlag == true
                           && a.CompanyId == LoggedInEmployee.CompanyId && a.BranchId == LoggedInEmployee.BranchId
                           && a.EmployeeId == LoggedInEmployee.EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();

                    EmployeeNameList.AddRange(SelfEmployeeNameList);
                }

                #endregion

                ViewBag.EmployeeList = new SelectList(EmployeeNameList, "EmployeeId", "EmployeeName").ToList();

            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in monthly reports selection due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));
            }

            //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable START
            ExportToExcelAndPDFEnable();
            //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable END


            return View(DailyReportsModelObj);
        }

        // Daily report Start
        /// <summary>
        /// Daily Reports Index POST
        /// Updated by Rajas on 22 DEC 2016
        /// </summary>
        /// <param name="FC"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DailyReportsIndex(DailyReportsModel ReportModel, FormCollection fc)
        {
            try
            {
                string PdfFileName = "";

                #region GENERIC CODE ADDED BY SHRADDHA ON 17 MAR 2018

                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                DateTime FDate = DateTime.Now;
                DateTime TDate = DateTime.Now;
                string EmployeeString = string.Empty;

                if ((!string.IsNullOrEmpty(ReportModel.FromDate)))
                {
                    //CODE ADDED FOR DATE RANGE VALIDATION BY SHRADDHA ON 23 FEB 2018 START
                    FDate = Convert.ToDateTime(ReportModel.FromDate);
                    TDate = Convert.ToDateTime(ReportModel.FromDate);

                    if ((TDate - FDate).TotalDays > 31)
                    {
                        PopulateDropDown();
                        ModelState.AddModelError("Error", "Please select date range less than or equal to 31 days.");
                        Error("Please select date range less than or equal to 31 days.");
                        ExportToExcelAndPDFEnable();
                        return View(ReportModel);
                    }
                }

                FDate = Convert.ToDateTime(ReportModel.FromDate);
                TDate = Convert.ToDateTime(ReportModel.ToDate);


                #region ADDED REGION BY SHRADDHA ON 17 MAR 2018
                if (ReportModel.EmployeeId == 0 || ReportModel.EmployeeId == null)
                {
                    int? CompanyId = ReportModel.CompanyId == 0 ? (int?)null : ReportModel.CompanyId;
                    int? BranchId = ReportModel.BranchId == 0 ? (int?)null : ReportModel.BranchId;
                    int? EmployeeTypeId = ReportModel.EmployeeTypeId == 0 ? (int?)null : ReportModel.EmployeeTypeId;
                    int? DepartmentId = ReportModel.DepartmentId == 0 ? (int?)null : ReportModel.DepartmentId;
                    EmployeeString = GetEmployeeString(FDate, TDate, CompanyId, BranchId, EmployeeTypeId, DepartmentId);
                }
                else
                {
                    EmployeeString = fc["EmployeeId"];
                }
                ReportModel.EmployeeString = EmployeeString;
                #endregion
                #endregion

                ReportModel.ReportFormat = Convert.ToInt32(fc["Export"]);

                switch (ReportModel.ReportSelected)
                {
                    case "1":
                        string Report1 = "Daily Attendance Report";
                        ReportModel.ReportName = Report1;
                        PdfFileName = DailyAttedenceReport(ReportModel, fc);
                        break;
                    case "2":
                        string Report2 = "Current Day Report";
                        ReportModel.ReportName = Report2;
                        // ADDED BY MSJ ON 02 MAR 2020 START
                        //PdfFileName = CurrentDayReport(ReportModel, fc);
                        PdfFileName = CDR(ReportModel, fc);
                        // ADDED BY MSJ ON 02 MAR 2020 END
                        break;
                    case "3":
                        string Report3 = "Previous Day Attendance Report";
                        ReportModel.ReportName = Report3;

                        // ADDED BY MSJ ON 02 MAR 2020 START
                        //PdfFileName = PreviousDayAttendanceReport(ReportModel, fc);
                        PdfFileName = PDR(ReportModel, fc);
                        // ADDED BY MSJ ON 02 MAR 2020 END
                        break;
                    case "4":
                        string Report4 = "Swipe Movement Report";
                        ReportModel.ReportName = Report4;
                        PdfFileName = SwipeMovementReport(ReportModel, fc);
                        break;
                    case "5":
                        string Report5 = "Audit Log Report";
                        ReportModel.ReportName = Report5;
                        PdfFileName = AuditLogReport(ReportModel, fc);
                        break;
                    case "6":
                        string Report6 = "Daily Attendance";
                        ReportModel.ReportName = Report6;
                        PdfFileName = DailyAttendanceLogReport(ReportModel, fc);
                        break;
                    case "7":
                        string Report7 = "Current Day Attendence Report";
                        ReportModel.ReportName = Report7;
                        PdfFileName = CurrentDayAttendenceReport(ReportModel, fc);
                        break;
                    case "8":
                        string Report8 = "Previous Day Attendence Report";
                        ReportModel.ReportName = Report8;
                        PdfFileName = PreviousDayAttendenceReportNew(ReportModel, fc);
                        break;

                    // PdfFileName = string.Empty;

                    default:
                        break;
                    //}
                }

                // Added by Rajas on 23 DEC 2016 for handling null or empty PdfFileName START

                if (string.IsNullOrEmpty(PdfFileName))
                {
                    PopulateDropDown();
                    ModelState.AddModelError("Error", "Please try again");

                    //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable START
                    ExportToExcelAndPDFEnable();
                    //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable END

                    return View(ReportModel);
                }
                else
                {
                    // Added by Rajas on 14 MARCh 2017
                    if (ReportModel.ReportFormat == 2)  // excel format
                    {
                        return File(PdfFileName, "application/xls");
                    }
                    else // PDF format
                    {
                        return File(PdfFileName, "application/pdf");
                    }

                }

                // Added by Rajas on 23 DEC 2016 for handling null or empty PdfFileName END
            }

            //else
            catch (System.Exception ex)
            {
                PopulateDropDown();
                //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable START
                ExportToExcelAndPDFEnable();
                //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable END

                AddAuditTrail(ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                return View(ReportModel);
            }
        }

        // Daily report Start
        /// <summary>
        /// Daily Reports Index POST
        /// Updated by Rajas on 22 DEC 2016
        /// </summary>
        /// <param name="FC"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DailyReportsIndexHR(DailyReportsModel ReportModel, FormCollection fc)
        {
            //if (ModelState.IsValid)
            //{
            try
            {
                string PdfFileName = "";


                #region GENERIC CODE ADDED BY SHRADDHA ON 17 MAR 2018
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                DateTime FDate = DateTime.Now;
                DateTime TDate = DateTime.Now;
                string EmployeeString = string.Empty;

                if ((!string.IsNullOrEmpty(ReportModel.FromDate)))
                {
                    //CODE ADDED FOR DATE RANGE VALIDATION BY SHRADDHA ON 23 FEB 2018 START
                    FDate = Convert.ToDateTime(ReportModel.FromDate);
                    TDate = Convert.ToDateTime(ReportModel.FromDate);

                    if ((TDate - FDate).TotalDays > 31)
                    {
                        PopulateDropDown();
                        ModelState.AddModelError("Error", "Please select date range less than or equal to 31 days.");
                        Error("Please select date range less than or equal to 31 days.");
                        ExportToExcelAndPDFEnable();
                        return View(ReportModel);
                    }
                }

                FDate = Convert.ToDateTime(ReportModel.FromDate);
                TDate = Convert.ToDateTime(ReportModel.FromDate);

                string DepStrFC = fc["DepartmentId"];
                #region ADDED REGION BY SHRADDHA ON 17 MAR 2018
                if (ReportModel.EmployeeId == 0 || ReportModel.EmployeeId == null)
                {
                    int? CompanyId = ReportModel.CompanyId == 0 ? (int?)null : ReportModel.CompanyId;
                    int? BranchId = ReportModel.BranchId == 0 ? (int?)null : ReportModel.BranchId;
                    int? EmployeeTypeId = ReportModel.EmployeeTypeId == 0 ? (int?)null : ReportModel.EmployeeTypeId;
                    int? DepartmentId = ReportModel.DepartmentId == 0 ? (int?)null : ReportModel.DepartmentId;
                    EmployeeString = GetEmployeeStringDept(FDate, TDate, CompanyId, BranchId, EmployeeTypeId, DepartmentId, DepStrFC);
                }
                else
                {
                    EmployeeString = fc["EmployeeId"];
                }
                ReportModel.EmployeeString = EmployeeString;
                #endregion
                #endregion

                ReportModel.ReportFormat = Convert.ToInt32(fc["Export"]);

                switch (ReportModel.ReportSelected)
                {
                    case "1":
                        string Report1 = "Daily Attendance Report";
                        ReportModel.ReportName = Report1;
                        PdfFileName = DailyAttedenceReport(ReportModel, fc);
                        break;
                    case "2":
                        string Report2 = "Current Day Report";
                        ReportModel.ReportName = Report2;

                        // ADDED BY MSJ ON 02 MAR 2020 START
                        //PdfFileName = CurrentDayReport(ReportModel, fc);
                        PdfFileName = CDR(ReportModel, fc);
                        // ADDED BY MSJ ON 02 MAR 2020 END
                        break;
                    case "3":
                        string Report3 = "Previous Day Attendance Report";
                        ReportModel.ReportName = Report3;

                        // ADDED BY MSJ ON 02 MAR 2020 START
                        //PdfFileName = PreviousDayAttendanceReport(ReportModel, fc);
                        PdfFileName = PDR(ReportModel, fc);
                        // ADDED BY MSJ ON 02 MAR 2020 END
                        break;
                    case "4":
                        string Report4 = "Swipe Movement Report";
                        ReportModel.ReportName = Report4;
                        PdfFileName = SwipeMovementReport(ReportModel, fc);
                        break;
                    case "5":
                        string Report5 = "Audit Log Report";
                        ReportModel.ReportName = Report5;
                        PdfFileName = AuditLogReport(ReportModel, fc);
                        break;
                    case "6":
                        string Report6 = "";
                        ReportModel.ReportName = Report6;
                        PdfFileName = string.Empty;
                        break;
                    case "7":
                        string Report7 = "";
                        ReportModel.ReportName = Report7;
                        PdfFileName = CurrentDayAttendenceReport(ReportModel, fc);
                        break;
                    case "8":
                        string Report8 = "";
                        ReportModel.ReportName = Report8;
                        PdfFileName = PreviousDayAttendenceReportNew(ReportModel, fc);
                        break;
                    default:
                        break;
                    //}
                }

                // Added by Rajas on 23 DEC 2016 for handling null or empty PdfFileName START

                if (string.IsNullOrEmpty(PdfFileName))
                {
                    PopulateDropDown();
                    ModelState.AddModelError("Error", "Please try again");

                    //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable START
                    ExportToExcelAndPDFEnable();
                    //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable END

                    return View(ReportModel);
                }
                else
                {
                    // Added by Rajas on 14 MARCh 2017
                    if (ReportModel.ReportFormat == 2)  // excel format
                    {
                        return File(PdfFileName, "application/xls");
                    }
                    else // PDF format
                    {
                        return File(PdfFileName, "application/pdf");
                    }

                }

                // Added by Rajas on 23 DEC 2016 for handling null or empty PdfFileName END
            }

            //else
            catch (System.Exception ex)
            {
                PopulateDropDown();
                //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable START
                ExportToExcelAndPDFEnable();
                //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable END

                AddAuditTrail(ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                return View(ReportModel);
            }
        }

        // Partial view for Attendance Report
        // Partial views for attendance report and Previous day are same
        public ActionResult AttendanceReport()
        {
            // Drop down for Branch name
            #region CODE FOR GET BRANCH LIST ADDED BY SHRADDHA ON 30 MAR 2018
            int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
            //var BranchName = WetosDB.Branches.Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList();
            var BranchName = WetosDB.SP_GetBranchList(EmployeeId).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList(); //.Select(m => new { BranchId = m.BranchId, BranchName = m.BranchName}).ToList();
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
            #endregion
            ViewBag.BranchNameList = new SelectList(BranchName, "BranchId", "BranchName").ToList();
            return View();
        }

        // Partial views for leave balance, current day report, swipe movement are same
        /// <summary>
        /// Leave Balance
        /// </summary>
        /// <returns></returns>
        public ActionResult LeaveBalance()
        {
            // Drop down for Branch name
            #region CODE FOR GET BRANCH LIST ADDED BY SHRADDHA ON 30 MAR 2018
            int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
            //var BranchName = WetosDB.Branches.Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList();
            var BranchName = WetosDB.SP_GetBranchList(EmployeeId).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList(); //.Select(m => new { BranchId = m.BranchId, BranchName = m.BranchName}).ToList();
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
            #endregion

            ViewBag.BranchNameList = new SelectList(BranchName, "BranchId", "BranchName").ToList();
            return View();
        }

        // Similar for monthly reports partial view except Requisition, payslip and leave application
        /// <summary>
        /// Audit Log
        /// </summary>
        /// <returns></returns>
        public ActionResult AuditLog()
        {
            // Drop down for Branch name
            #region CODE FOR GET BRANCH LIST ADDED BY SHRADDHA ON 30 MAR 2018
            int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
            //var BranchName = WetosDB.Branches.Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList();
            var BranchName = WetosDB.SP_GetBranchList(EmployeeId).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList(); //.Select(m => new { BranchId = m.BranchId, BranchName = m.BranchName}).ToList();
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
            #endregion

            ViewBag.BranchNameList = new SelectList(BranchName, "BranchId", "BranchName").ToList();
            return View();
        }

        // Daily reoprts end

        /// <summary>
        /// MonthlyReportsIndex
        /// </summary>
        /// <returns></returns>
        public ActionResult MonthlyReportsIndex()
        {
            MonthlyReportsModel MonthlyReportsObj = new MonthlyReportsModel();

            try
            {
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);

                Employee LoggedInEmployee = WetosDB.Employees.Where(a => a.EmployeeId == EmployeeId).FirstOrDefault();

                UserRole ChechUserRole = WetosDB.UserRoles.Where(a => a.EmployeeId == EmployeeId).OrderByDescending(a => a.RoleTypeId).FirstOrDefault();

                //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO HANDLE NULL OBJECT REFERECE ERROR FOR LoggedInEmployee OBJ AND ChechUserRole OBJ START
                if (LoggedInEmployee == null)
                {
                    LoggedInEmployee = new Employee();
                }
                if (ChechUserRole == null)
                {
                    ChechUserRole = new UserRole();
                }
                //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO HANDLE NULL OBJECT REFERECE ERROR FOR LoggedInEmployee OBJ AND ChechUserRole OBJ END

                MonthlyReportsObj.CompanyId = LoggedInEmployee.CompanyId;
                MonthlyReportsObj.BranchId = LoggedInEmployee.BranchId;
                MonthlyReportsObj.DepartmentId = LoggedInEmployee.DepartmentId;
                MonthlyReportsObj.EmployeeId = LoggedInEmployee.EmployeeId; // UN COMMNETD BY MSJ ON 11 MAY 2020
                MonthlyReportsObj.UserId = ChechUserRole.RoleTypeId;

                #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //var EmployeeNameList = WetosDB.Employees.Where(a => a.ActiveFlag == null || a.ActiveFlag == true
                //&& a.CompanyId == LoggedInEmployee.CompanyId && a.BranchId == LoggedInEmployee.BranchId
                //&& a.EmployeeReportingId == LoggedInEmployee.EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
                var EmployeeNameList = WetosDB.SP_VwActiveEmployee(EmployeeId).Where(a => a.ActiveFlag == null || a.ActiveFlag == true
                        && a.CompanyId == LoggedInEmployee.CompanyId && a.BranchId == LoggedInEmployee.BranchId
                        && a.EmployeeReportingId == LoggedInEmployee.EmployeeId).Select(a => new
                        {
                            EmployeeId = a.EmployeeId
                            ,
                            EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName
                        }).ToList();

                // ADDED BY MSJ ON 20 MAY 2020
                if (EmployeeNameList.Where(a => a.EmployeeId == LoggedInEmployee.EmployeeId).FirstOrDefault() == null)
                {
                    // ADDED BY MSJ ON 15 MAY 2020
                    var SelfEmployeeNameList = WetosDB.SP_VwActiveEmployee(EmployeeId).Where(a => a.ActiveFlag == null || a.ActiveFlag == true
                           && a.CompanyId == LoggedInEmployee.CompanyId && a.BranchId == LoggedInEmployee.BranchId
                           && a.EmployeeId == LoggedInEmployee.EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();

                    EmployeeNameList.AddRange(SelfEmployeeNameList);
                }

                #endregion

                ViewBag.EmployeeList = new SelectList(EmployeeNameList, "EmployeeId", "EmployeeName").ToList();

                DailyTransactionAttendanceStatus DailyTransactionAttendanceStatusObj = new DailyTransactionAttendanceStatus();

                var MonthList = WetosDB.DropdownDatas.Where(a => a.TypeId == 7).Select(a => new { Value = a.Value, Text = a.Text }).ToList();

                ViewBag.MonthListVB = new SelectList(MonthList, " Value", "Text").ToList();

                //GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == "Current Financial Year").FirstOrDefault();

                //ABOVE LINE COMMENTED AND BELOW LINE IS ADDED BY SHALAKA ON 13TH DEC 2017 

                GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.CurrentFinancialYear).FirstOrDefault();
                if (GlobalSettingObj != null)
                {
                    PopulateYearListVB();
                }
                else
                {
                    var YearList = new List<FinancialYear>();
                    ViewBag.YearListVB = new SelectList(YearList, " FinancialId", "FinancialName").ToList();

                    Information("Inconsistent Financial Year data, Please verify and try again!");
                }

            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in monthly reports selection due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));
            }

            //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable START
            ExportToExcelAndPDFEnable();
            //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable END

            return View(MonthlyReportsObj);
        }

        /// <summary>
        /// Added by Rajas on 12 SEP 2017 for HR
        /// </summary>
        /// <returns></returns>
        public ActionResult MonthlyReportsIndexHR()
        {
            int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);

            PopulateDropDown(EmployeeId);

            //PopulateDropDown();
            DailyTransactionAttendanceStatus DailyTransactionAttendanceStatusObj = new DailyTransactionAttendanceStatus();

            var MonthList = WetosDB.DropdownDatas.Where(a => a.TypeId == 7).Select(a => new { Value = a.Value, Text = a.Text }).ToList();

            ViewBag.MonthListVB = new SelectList(MonthList, " Value", "Text").ToList();


            // ADDED BY MSJ ON 08 JAN 2018 START
            GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.CurrentFinancialYear).FirstOrDefault();

            if (GlobalSettingObj != null)
            {
                PopulateYearListVB();
            }
            else
            {
                var YearList = new List<FinancialYear>();
                ViewBag.YearListVB = new SelectList(YearList, " FinancialId", "FinancialName").ToList();

                Information("Inconsistent Financial Year data, Please verify and try again!");
            }


            MonthlyReportsModel MonthlyReportsObj = new MonthlyReportsModel();

            //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable START
            ExportToExcelAndPDFEnable();
            //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable END

            return View(MonthlyReportsObj);
        }


        /// <summary>
        /// Populate Drop Down
        /// </summary>
        private void PopulateDropDown(int EmployeeId)
        {
            List<UserRole> UserRoleList = WetosDB.UserRoles.Where(a => a.EmployeeId == EmployeeId).ToList();
            //List<WetosDB.Company> CompanyList = new List<WetosDB.Company>();
            List<Branch> BranchList = new List<Branch>();

            //CODE FOR DROPDOWN
            // code for company drop down list
            // Updated by Rajas on 6 MAY 2017 to get data which is not MarkedAsDelete
            #region COMPANY LIST CODE ADDED BY SHRADDHA ON 30 MAR 2018
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
            //if (UserRoleList != null && UserRoleList.Count > 0)
            //{
            //    foreach (UserRole UserRoleObj in UserRoleList)
            //    {
            //        #region COMPANY DROPDOWN
            //        int CompanyId = Convert.ToInt32(UserRoleObj.CompanyId == null ? 0 : UserRoleObj.CompanyId);
            //        if (CompanyId > 0)
            //        {
            //            WetosDB.Company CompanyObjNew = CompanyList.Where(a => a.CompanyId == CompanyId).FirstOrDefault();
            //            if (CompanyObjNew == null)
            //            {
            //                CompanyObjNew = WetosDB.Companies.Where(a => a.CompanyId == CompanyId).FirstOrDefault();
            //                CompanyList.Add(CompanyObjNew);
            //            }

            //        }
            //        else
            //        {
            //            CompanyList = WetosDB.Companies.Where(a => a.MarkedAsDelete == null || a.MarkedAsDelete == 0).ToList();

            //        }
            //        #endregion

            //    }
            //}
            var CompanyList = WetosDB.SP_GetCompanyList(EmployeeId).Select(m => new { Companyid = m.CompanyId, CompanyName = m.CompanyName }).ToList();
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
            #endregion
            ViewBag.CompanyNameList = new SelectList(CompanyList, "CompanyId", "CompanyName").ToList();

            //drop down for branch name list

            ViewBag.BranchNameList = new SelectList(BranchList, "BranchId", "BranchName").ToList();


            // code for Department drop down list
            List<Department> DepartmentList = new List<Department>();
            ViewBag.DepartmentNameList = new SelectList(DepartmentList, "DepartmentId", "DepartmentName").ToList();

            //Drop down for employee
            var EmployeeNameList = new List<Employee>();
            ViewBag.EmployeeList = new SelectList(EmployeeNameList, "EmployeeId", "Name").ToList();

            //CODE ADDED BY SHRADDHA ON 15 FEB 2018 FOR EMPLOYEE TYPE START
            List<WetosDB.EmployeeType> EmployeeTypeList = WetosDB.EmployeeTypes.Where(a => a.MarkedAsDelete == null || a.MarkedAsDelete == 0).ToList();
            ViewBag.EmployeeTypeList = new SelectList(EmployeeTypeList, "EmployeeTypeId", "EmployeeTypeName").ToList();
            //CODE ADDED BY SHRADDHA ON 15 FEB 2018 FOR EMPLOYEE TYPE END


            // ADDED BY MSJ ON 01 FEB 2018 START
            var MonthList = WetosDB.DropdownDatas.Where(a => a.TypeId == 7).Select(a => new { Value = a.Value, Text = a.Text }).ToList();

            ViewBag.MonthListVB = new SelectList(MonthList, " Value", "Text").ToList();

            GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.CurrentFinancialYear).FirstOrDefault();
            if (GlobalSettingObj != null)
            {
                PopulateYearListVB();
            }
            else
            {
                var YearList = new List<FinancialYear>();
                ViewBag.YearListVB = new SelectList(YearList, " FinancialId", "FinancialName").ToList();

                Information("Inconsistent Financial Year data, Please verify and try again!");
            }
            // ADDED BY  MSJ ON 01 FEB 2018 END

        }

        /// <summary>
        /// Validation added for from date, to date and branch
        /// Updated by Rajas on 22 DEC 2016
        /// </summary>
        /// <param name="ReportModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult MonthlyReportsIndex(MonthlyReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 7 FEB 2017
        {
            //if (ModelState.IsValid)
            //{
            try
            {
                //if (ModelState.IsValid)
                //{

                #region GENERIC CODE ADDED BY SHRADDHA ON 17 MAR 2018

                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                DateTime FDate = DateTime.Now;
                DateTime TDate = DateTime.Now;
                string EmployeeString = string.Empty;

                string opt = fc["opt"];


                if (opt == "MonthWise")
                {
                    ReportModel.FromDate = null;
                    ReportModel.ToDate = null;

                    if ((ReportModel.FinancialYearId == null || ReportModel.FinancialYearId == 0) || (ReportModel.MonthId == null || ReportModel.MonthId == 0))
                    {
                        PopulateDropDown();
                        //ModelState.AddModelError("Error", "Please select date range less than or equal to 31 days.");
                        Error("Please select Year and Month");
                        ExportToExcelAndPDFEnable();
                        return View(ReportModel);
                    }
                }
                else
                {
                    ReportModel.FinancialYearId = 0;
                    ReportModel.MonthId = 0;

                    if ((!string.IsNullOrEmpty(ReportModel.ToDate)) && (!string.IsNullOrEmpty(ReportModel.FromDate)))
                    {
                        //CODE ADDED FOR DATE RANGE VALIDATION BY SHRADDHA ON 23 FEB 2018 START
                        FDate = Convert.ToDateTime(ReportModel.FromDate);
                        TDate = Convert.ToDateTime(ReportModel.ToDate);

                        if ((TDate - FDate).TotalDays > 31)
                        {
                            PopulateDropDown();
                            //ModelState.AddModelError("Error", "Please select date range less than or equal to 31 days.");
                            Error("Please select date range less than or equal to 31 days.");
                            ExportToExcelAndPDFEnable();
                            return View(ReportModel);
                        }
                    }
                    else
                    {
                        PopulateDropDown();
                        ModelState.AddModelError("Error", "Please select date range.");
                        //Error("Please select date range.");
                        ExportToExcelAndPDFEnable();
                        return View(ReportModel);
                    }
                }

                if (ReportModel.FromDate == null && ReportModel.ToDate == null)
                {

                    int SelectedYear = Convert.ToInt32(ReportModel.FinancialYearId); // MODIFIED BY MSJ ON 08 JAN 2018 Convert.ToInt32(SelectedAttendenceYear.FinancialName);
                    int SelectedMonth = Convert.ToInt32(ReportModel.MonthId);

                    // MODIFIED BY MSJ ON 19 MAY 2020
                    //ReportModel.FromDate = FirstDayOfMonthFromDateTime(new DateTime(SelectedYear, SelectedMonth, 1)).ToString("yyyy/MM/dd");
                    //ReportModel.ToDate = LastDayOfMonthFromDateTime(new DateTime(SelectedYear, SelectedMonth, 1)).ToString("yyyy/MM/dd");

                    ReportModel.FromDate = FirstDayOfMonthFromDateTime(new DateTime(SelectedYear, SelectedMonth, 1)).ToString("dd-MMM-yyyy");
                    ReportModel.ToDate = LastDayOfMonthFromDateTime(new DateTime(SelectedYear, SelectedMonth, 1)).ToString("dd-MMM-yyyy");

                    FDate = FirstDayOfMonthFromDateTime(new DateTime(SelectedYear, SelectedMonth, 1));
                    TDate = LastDayOfMonthFromDateTime(new DateTime(SelectedYear, SelectedMonth, 1));
                }
                else
                {

                    FDate = Convert.ToDateTime(ReportModel.FromDate);
                    TDate = Convert.ToDateTime(ReportModel.ToDate);
                }

                #region ADDED REGION BY SHRADDHA ON 17 MAR 2018
                if (ReportModel.EmployeeId == 0 || ReportModel.EmployeeId == null)
                {
                    int? CompanyId = ReportModel.CompanyId == 0 ? (int?)null : ReportModel.CompanyId;
                    int? BranchId = ReportModel.BranchId == 0 ? (int?)null : ReportModel.BranchId;
                    int? EmployeeTypeId = ReportModel.EmployeeTypeId == 0 ? (int?)null : ReportModel.EmployeeTypeId;
                    int? DepartmentId = ReportModel.DepartmentId == 0 ? (int?)null : ReportModel.DepartmentId;
                    EmployeeString = GetEmployeeString(FDate, TDate, CompanyId, BranchId, EmployeeTypeId, DepartmentId);
                }
                else
                {
                    EmployeeString = fc["EmployeeId"];
                }
                ReportModel.EmployeeString = EmployeeString;
                #endregion
                #endregion

                string PdfFileName = "";

                ReportModel.ReportFormat = Convert.ToInt32(fc["Export"]);

                switch (ReportModel.RaportSelected)
                {
                    case "1":
                        string Report1 = "Monthly Working Hours Report";
                        ReportModel.ReportName = Report1;
                        PdfFileName = MonthlyWorkingHoursReport(ReportModel, fc);
                        break;
                    case "2":
                        string Report2 = "Continuous Absentism Report";
                        ReportModel.ReportName = Report2;
                        PdfFileName = ContinuousAbsentismReport(ReportModel, fc);
                        break;
                    case "3":
                        string Report3 = "Late Summary Report";  // Updated by Rajas on 23 MAY 2017
                        ReportModel.ReportName = Report3;
                        PdfFileName = LateSummaryReport(ReportModel, fc);
                        break;
                    case "4":
                        string Report4 = "Time Card Report";
                        ReportModel.ReportName = Report4;
                        PdfFileName = TimeCardReport(ReportModel, fc); // Updated by Rajas on 7 FEB 2017
                        break;
                    case "5":
                        string Report5 = "Requisition Application Report";
                        ReportModel.ReportName = Report5;
                        PdfFileName = RequisitionApplicationReport(ReportModel, fc);
                        break;
                    case "6":
                        string Report6 = "Late Coming Report";
                        ReportModel.ReportName = Report6;
                        PdfFileName = LateComing(ReportModel, fc); //L&T report done
                        break;
                    case "7":
                        //string Report7 = "";
                        //ReportModel.ReportName = Report7;
                        break;
                    case "8":
                        string Report8 = "Attendance Summary Report";
                        ReportModel.ReportName = Report8;
                        PdfFileName = AttendanceSummary(ReportModel, fc);
                        break;
                    case "9":
                        string Report9 = "Absenteeism Report";
                        ReportModel.ReportName = Report9;
                        PdfFileName = AbsenteeismReport(ReportModel, fc);
                        break;
                    case "10":
                        string Report10 = "Leave Application Report";
                        ReportModel.ReportName = Report10;
                        PdfFileName = LeaveApplicationReport(ReportModel, fc);
                        break;
                    case "11":
                        string Report11 = "Leave Balance Report";
                        ReportModel.ReportName = Report11;
                        PdfFileName = LeaveBalanceReport(ReportModel, fc);
                        break;
                    //REPORT NO 12 TO 16 ARE UNUSED AS ON 17 MAR 2018
                    case "12":
                        string Report12 = "Lunch Late Employee";
                        ReportModel.ReportName = Report12;
                        PdfFileName = LunchLateEmployee(ReportModel, fc);
                        break;
                    case "13":
                        string Report13 = "Daily Early Departure Report";
                        ReportModel.ReportName = Report13;
                        PdfFileName = DailyEarlyDepartureReport(ReportModel, fc); //L&T report done
                        break;
                    case "14":
                        string Report14 = "Employee Performance Report";
                        ReportModel.ReportName = Report14;
                        PdfFileName = EmployeePerformanceReport(ReportModel, fc); //L&T report done
                        break;
                    case "15":
                        string Report15 = "Daily Over Stay Report";
                        ReportModel.ReportName = Report15;
                        PdfFileName = DailyOverStayReport(ReportModel, fc); //L&T report done
                        break;

                    case "16":
                        string Report16 = "Monthly Muster Attendance Report";
                        ReportModel.ReportName = Report16;

                        // MODIFIED BY MSJ ON 20 FEB 2020
                        //PdfFileName = MonthlyMusterReport(ReportModel, fc);
                        PdfFileName = MonthlyAttendanceReport(ReportModel, fc);
                        // PdfFileName = MonthlyAttendanceReport(ReportModel, fc);
                        break;

                    case "17":
                        string Report17 = "Monthly Over Time Report";
                        ReportModel.ReportName = Report17;
                        // PdfFileName = MonthlyMusterReport(ReportModel, fc);
                        PdfFileName = MonthlyOverTimeReport(ReportModel, fc);
                        break;

                    case "18":
                        string Report18 = "Monthly Over Time Summary Report";
                        ReportModel.ReportName = Report18;
                        // PdfFileName = MonthlyMusterReport(ReportModel, fc);
                        PdfFileName = MonthlyOTSummaryReport(ReportModel, fc);
                        break;

                    default:
                        break;
                    //}
                }



                //return RedirectToAction("MIS");

                // Added by Rajas on 23 DEC 2016 for handling null or empty PdfFileName START

                if (string.IsNullOrEmpty(PdfFileName))
                {
                    PopulateDropDown();
                    Error("Please try again");
                    //ModelState.AddModelError("Error", "Please try again");

                    //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable START
                    ExportToExcelAndPDFEnable();
                    //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable END

                    return View(ReportModel);
                }
                else
                {
                    Clear();

                    // Added by Rajas on 14 MARCh 2017
                    if (ReportModel.ReportFormat == 2)  // excel format
                    {
                        return File(PdfFileName, "application/xls");
                        //return View(PdfFileName, "application/pdf");
                    }
                    else // PDF format
                    {

                        //return View(PdfFileName, "application/pdf");
                        return File(PdfFileName, "application/pdf");

                        //FileStream fs = new FileStream(PdfFileName, FileMode.Open);

                        //var fileName = PdfFileName; // "ABCD";
                        //Session[fileName] = fs; // PdfFileName; // baseOutputStream;
                        //return Json(new { success = true, fileName }, JsonRequestBehavior.AllowGet);
                    }
                }

                // Added by Rajas on 23 DEC 2016 for handling null or empty PdfFileName END
                //}
                //else
                //{
                //    PopulateDropDown();
                //    //ModelState.AddModelError("Error", "Please select date range.");
                //    Error("Please select date range.");
                //    ExportToExcelAndPDFEnable();
                //    return View(ReportModel);
                //}

            }
            catch (System.Exception ex)
            {
                PopulateDropDown();

                //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable START
                ExportToExcelAndPDFEnable();
                //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable END

                AddAuditTrail(ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                return View(ReportModel);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public FileStreamResult DownloadExport(string fileName)
        {
            var file = (FileStream)Session[fileName];

            string[] GeneratedFileName = fileName.Split(new string[] { "download\\" }, StringSplitOptions.None);

            Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName[1] + ";");

            return new FileStreamResult(file, "application/pdf");// "your file type goes here");

        }

        /// <summary>
        /// Validation added for from date, to date and branch
        /// Updated by Rajas on 22 DEC 2016
        /// </summary>
        /// <param name="ReportModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult MonthlyReportsIndexHR(MonthlyReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 7 FEB 2017
        {
            try
            {
                //if (ModelState.IsValid)
                //{

                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);

                #region GENERIC CODE ADDED BY SHRADDHA ON 17 MAR 2018

                DateTime FDate = DateTime.Now;
                DateTime TDate = DateTime.Now;
                string EmployeeString = string.Empty;

                //if ((!string.IsNullOrEmpty(ReportModel.ToDate)) && (!string.IsNullOrEmpty(ReportModel.FromDate)))
                //{
                //    //CODE ADDED FOR DATE RANGE VALIDATION BY SHRADDHA ON 23 FEB 2018 START
                //    FDate = Convert.ToDateTime(ReportModel.FromDate);
                //    TDate = Convert.ToDateTime(ReportModel.ToDate);

                //    if ((TDate - FDate).TotalDays > 31)
                //    {
                //        PopulateDropDown();
                //        ModelState.AddModelError("Error", "Please select date range less than or equal to 31 days.");
                //        Error("Please select date range less than or equal to 31 days.");
                //        ExportToExcelAndPDFEnable();
                //        return View(ReportModel);
                //    }
                //}
                //else
                //{
                //    PopulateDropDown();
                //    ModelState.AddModelError("Error", "Please select date range.");
                //    Error("Please select date range.");
                //    ExportToExcelAndPDFEnable();
                //    return View(ReportModel);
                //}

                string opt = fc["opt"];
                string DepStrFC = fc["DepartmentId"];
                if (ReportModel.RaportSelected != "11")
                {
                    if (opt == "MonthWise")
                    {
                        ReportModel.FromDate = null;
                        ReportModel.ToDate = null;

                        if ((ReportModel.FinancialYearId == null || ReportModel.FinancialYearId == 0) || (ReportModel.MonthId == null || ReportModel.MonthId == 0))
                        {
                            PopulateDropDown();
                            //ModelState.AddModelError("Error", "Please select date range less than or equal to 31 days.");
                            Error("Please select Year and Month");
                            ExportToExcelAndPDFEnable();
                            return View(ReportModel);
                        }
                    }
                    else
                    {
                        ReportModel.FinancialYearId = 0;
                        ReportModel.MonthId = 0;

                        if ((!string.IsNullOrEmpty(ReportModel.ToDate)) && (!string.IsNullOrEmpty(ReportModel.FromDate)))
                        {
                            //CODE ADDED FOR DATE RANGE VALIDATION BY SHRADDHA ON 23 FEB 2018 START
                            FDate = Convert.ToDateTime(ReportModel.FromDate);
                            TDate = Convert.ToDateTime(ReportModel.ToDate);

                            if ((TDate - FDate).TotalDays > 31)
                            {
                                PopulateDropDown();
                                //ModelState.AddModelError("Error", "Please select date range less than or equal to 31 days.");
                                Error("Please select date range less than or equal to 31 days.");
                                ExportToExcelAndPDFEnable();
                                return View(ReportModel);
                            }
                        }
                        else
                        {
                            PopulateDropDown();
                            ModelState.AddModelError("Error", "Please select date range.");
                            //Error("Please select date range.");
                            ExportToExcelAndPDFEnable();
                            return View(ReportModel);
                        }
                    }

                    if (ReportModel.FromDate == null && ReportModel.ToDate == null)
                    {
                        int SelectedYear = Convert.ToInt32(ReportModel.FinancialYearId); // MODIFIED BY MSJ ON 08 JAN 2018 Convert.ToInt32(SelectedAttendenceYear.FinancialName);

                        int SelectedMonth = Convert.ToInt32(ReportModel.MonthId);

                        // MODIFIED BY MSJ ON 19 MAY 2020
                        //ReportModel.FromDate = FirstDayOfMonthFromDateTime(new DateTime(SelectedYear, SelectedMonth, 1)).ToString("yyyy/MM/dd");
                        //ReportModel.ToDate = LastDayOfMonthFromDateTime(new DateTime(SelectedYear, SelectedMonth, 1)).ToString("yyyy/MM/dd");

                        ReportModel.FromDate = FirstDayOfMonthFromDateTime(new DateTime(SelectedYear, SelectedMonth, 1)).ToString("dd-MMM-yyyy");
                        ReportModel.ToDate = LastDayOfMonthFromDateTime(new DateTime(SelectedYear, SelectedMonth, 1)).ToString("dd-MMM-yyyy");

                        FDate = FirstDayOfMonthFromDateTime(new DateTime(SelectedYear, SelectedMonth, 1));
                        TDate = LastDayOfMonthFromDateTime(new DateTime(SelectedYear, SelectedMonth, 1));
                    }
                    else
                    {
                        FDate = Convert.ToDateTime(ReportModel.FromDate);
                        TDate = Convert.ToDateTime(ReportModel.ToDate);
                    }
                }
                #region ADDED REGION BY SHRADDHA ON 17 MAR 2018
                if (ReportModel.EmployeeId == 0 || ReportModel.EmployeeId == null)
                {
                    int? CompanyId = ReportModel.CompanyId == 0 ? (int?)null : ReportModel.CompanyId;
                    int? BranchId = ReportModel.BranchId == 0 ? (int?)null : ReportModel.BranchId;
                    int? EmployeeTypeId = ReportModel.EmployeeTypeId == 0 ? (int?)null : ReportModel.EmployeeTypeId;
                    int? DepartmentId = ReportModel.DepartmentId == 0 ? (int?)null : ReportModel.DepartmentId;
                    EmployeeString = GetEmployeeStringDept(FDate.Date, TDate.Date, CompanyId, BranchId, EmployeeTypeId, 0, DepStrFC);
                }
                else
                {
                    EmployeeString = fc["EmployeeId"];
                }
                ReportModel.EmployeeString = EmployeeString;
                #endregion
                #endregion

                string PdfFileName = "";

                ReportModel.ReportFormat = Convert.ToInt32(fc["Export"]);

                switch (ReportModel.RaportSelected)
                {
                    case "1":
                        string Report1 = "Monthly Working Hours Report";
                        ReportModel.ReportName = Report1;
                        PdfFileName = MonthlyWorkingHoursReport(ReportModel, fc);
                        break;

                    case "2":
                        string Report2 = "Continuous Absentism Report";
                        ReportModel.ReportName = Report2;
                        PdfFileName = ContinuousAbsentismReport(ReportModel, fc);
                        break;

                    case "3":
                        string Report3 = "Late Summary Report";  // Updated by Rajas on 23 MAY 2017
                        ReportModel.ReportName = Report3;
                        PdfFileName = LateSummaryReport(ReportModel, fc);
                        break;

                    case "4":
                        string Report4 = "Time Card Report";
                        ReportModel.ReportName = Report4;
                        PdfFileName = TimeCardReport(ReportModel, fc); // Updated by Rajas on 7 FEB 2017
                        break;

                    case "5":
                        string Report5 = "Requisition Application Report";
                        ReportModel.ReportName = Report5;
                        PdfFileName = RequisitionApplicationReport(ReportModel, fc);
                        break;

                    case "6":
                        string Report6 = "Late Coming Report";
                        ReportModel.ReportName = Report6;
                        PdfFileName = LateComing(ReportModel, fc); //L&T report done
                        break;

                    case "7":
                        //string Report7 = "";
                        //ReportModel.ReportName = Report7;
                        break;
                    case "8":
                        string Report8 = "Attendance Summary Report";
                        ReportModel.ReportName = Report8;
                        PdfFileName = AttendanceSummary(ReportModel, fc);
                        break;

                    case "9":
                        string Report9 = "Absenteeism Report";
                        ReportModel.ReportName = Report9;
                        PdfFileName = AbsenteeismReport(ReportModel, fc);
                        break;

                    case "10":
                        string Report10 = "Leave Application Report";
                        ReportModel.ReportName = Report10;
                        PdfFileName = LeaveApplicationReport(ReportModel, fc);
                        break;
                    case "11":
                        string Report11 = "Leave Balance Report";
                        ReportModel.ReportName = Report11;
                        PdfFileName = LeaveBalanceReport(ReportModel, fc);
                        break;

                    //REPORT NO 12 TO 16 ARE UNUSED AS ON 17 MAR 2018
                    case "12":
                        string Report12 = "Lunch Late Employee";
                        ReportModel.ReportName = Report12;
                        PdfFileName = LunchLateEmployee(ReportModel, fc);
                        break;

                    case "13":
                        string Report13 = "Daily Early Departure Report";
                        ReportModel.ReportName = Report13;
                        PdfFileName = DailyEarlyDepartureReport(ReportModel, fc); //L&T report done
                        break;

                    case "14":
                        string Report14 = "Employee Performance Report";
                        ReportModel.ReportName = Report14;
                        PdfFileName = EmployeePerformanceReport(ReportModel, fc); //L&T report done
                        break;

                    case "15":
                        string Report15 = "Daily Over Stay Report";
                        ReportModel.ReportName = Report15;
                        PdfFileName = DailyOverStayReport(ReportModel, fc); //L&T report done
                        break;

                    case "16":
                        string Report16 = "Monthly Muster Attendance Report";
                        ReportModel.ReportName = Report16;
                        //PdfFileName = MonthlyMusterReport(ReportModel, fc);
                        PdfFileName = MonthlyAttendanceReport(ReportModel, fc);
                        break;
                    case "17":
                        string Report17 = "Monthly Over Time Report";
                        ReportModel.ReportName = Report17;
                        // PdfFileName = MonthlyMusterReport(ReportModel, fc);
                        PdfFileName = MonthlyOverTimeReport(ReportModel, fc);
                        break;
                    case "18":
                        string Report18 = "Monthly Over Time Summary Report";
                        ReportModel.ReportName = Report18;
                        // PdfFileName = MonthlyMusterReport(ReportModel, fc);
                        PdfFileName = MonthlyOTSummaryReport(ReportModel, fc);
                        break;

                    default:
                        break;
                    //}
                }

                //return RedirectToAction("MIS");

                // Added by Rajas on 23 DEC 2016 for handling null or empty PdfFileName START

                if (string.IsNullOrEmpty(PdfFileName))
                {
                    PopulateDropDown();
                    ModelState.AddModelError("Error", "Please try again");

                    //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable START
                    ExportToExcelAndPDFEnable();
                    //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable END

                    return View(ReportModel);
                }
                else
                {
                    // Added by Rajas on 14 MARCh 2017
                    if (ReportModel.ReportFormat == 2)  // excel format
                    {
                        return File(PdfFileName, "application/xls");
                    }
                    else // PDF format
                    {
                        return File(PdfFileName, "application/pdf");
                    }
                }
                // Added by Rajas on 23 DEC 2016 for handling null or empty PdfFileName END
                //}
                //else
                //{
                //    PopulateDropDown();
                //    ModelState.AddModelError("Error", "Please select date range.");
                //    Error("Please select date range.");
                //    ExportToExcelAndPDFEnable();
                //    return View(ReportModel);
                //}
            }
            catch (System.Exception ex)
            {
                PopulateDropDown();

                //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable START
                ExportToExcelAndPDFEnable();
                //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable END

                AddAuditTrail(ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                return View(ReportModel);
            }
        }

        /// <summary>
        /// Added by Rajas on 7 SEP 2017
        /// </summary>
        /// <returns></returns>
        public ActionResult SendAutoMailThroughEmail()
        {
            MonthlyReportsModel AttendanceMailObj = new MonthlyReportsModel();
            int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);

            Employee LoggedInEmployee = WetosDB.Employees.Where(a => a.EmployeeId == EmployeeId).FirstOrDefault();

            PopulateDropDown();

            AttendanceMailObj.CompanyId = LoggedInEmployee.CompanyId;
            AttendanceMailObj.BranchId = LoggedInEmployee.BranchId;
            AttendanceMailObj.DepartmentId = LoggedInEmployee.DepartmentId;
            AttendanceMailObj.EmployeeId = LoggedInEmployee.EmployeeId;



            #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
            EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            //var EmployeeNameList = WetosDB.Employees.Where(a => a.ActiveFlag == null || a.ActiveFlag == true
            //&& a.CompanyId == LoggedInEmployee.CompanyId && a.BranchId == LoggedInEmployee.BranchId
            //&& a.EmployeeReportingId == LoggedInEmployee.EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
            var EmployeeNameList = WetosDB.SP_VwActiveEmployee(EmployeeId).Where(a => a.ActiveFlag == null || a.ActiveFlag == true
                   && a.CompanyId == LoggedInEmployee.CompanyId && a.BranchId == LoggedInEmployee.BranchId
                   && a.EmployeeReportingId == LoggedInEmployee.EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
            #endregion

            ViewBag.EmployeeList = new SelectList(EmployeeNameList, "EmployeeId", "EmployeeName").ToList();
            return View(AttendanceMailObj);
        }

        /// <summary>
        /// Added by Rajas on 7 SEP 2017
        /// </summary>
        /// <param name="AttendanceMailObj"></param>
        /// <param name="fc"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SendAutoMailThroughEmail(MonthlyReportsModel AttendanceMailObj, FormCollection fc)
        {
            //string EmployeeString = (String.Join(",", WetosDB.Employees.Where(a => a.ActiveFlag == null || a.ActiveFlag == true
            //            && a.CompanyId == AttendanceMailObj.CompanyId && a.BranchId == AttendanceMailObj.BranchId && a.EmployeeReportingId == AttendanceMailObj.EmployeeId).Select(a => a.EmployeeId).ToArray()))
            //            + "," + fc["EmployeeId"];

            //AutoMissPunchEmail(WetosDB, AttendanceMailObj, EmployeeString);
            //AutoAttendanceEmail(WetosDB, AttendanceMailObj, EmployeeString);

            DateTime StartTime = DateTime.Now;

            GenerateTimeCardReport(WetosDB, AttendanceMailObj);

            PopulateDropDown();

            var EmployeeNameList = new List<Employee>();
            ViewBag.EmployeeList = new SelectList(EmployeeNameList, "EmployeeId", "Name").ToList();

            DateTime EndTime = DateTime.Now;
            TimeSpan ExeTime = EndTime.TimeOfDay - StartTime.TimeOfDay;

            AddAuditTrail("Auto email send Execution time : " + ExeTime);

            return View(AttendanceMailObj);
        }

        /// <summary>
        /// HTML email code
        /// Added by Rajas on 28 AUGUST 2017
        /// </summary>
        /// <returns></returns>
        /// Updated by Rajas on 7 SEP 2017
        public static bool AutoAttendanceEmail(WetosDBEntities WetosDB, MonthlyReportsModel AttendanceMailObj, string EmployeeStr)
        {
            try
            {
                //int CurrentYearInt = DateTime.Now.Year; // ADDED BY MSJ ON 02 JAN 2018
                DateTime FDate = new DateTime(2017, 07, 16);
                DateTime TDate = new DateTime(2017, 08, 15);
                //List<DailyTransaction> DTObj = WetosDB.DailyTransactions.Where(a => a.TranDate == Date).ToList();

                List<usp_Rpt_GetTimeCard_Result> DTObj = WetosDB.usp_Rpt_GetTimeCard(FDate, TDate, EmployeeStr, null, null, null, "1", null).ToList();
                List<int> EmpIdList = DTObj.Select(a => a.EmployeeId).Distinct().ToList();

                List<string> EmailNotSentToEmployee = new List<string>();  // Added by Rajas on 31 AUGUST 2017
                Queue<string> QueueObj = new Queue<string>();

                StringBuilder StrBuild = new StringBuilder();

                // for (int i = 1; i < 2; i++)
                {
                    foreach (int EmpIDobj in EmpIdList)
                    {
                        try
                        {
                            StrBuild.Append("<!DOCTYPE html><html><head><title>TEST</title></head><body>");
                            usp_Rpt_GetTimeCard_Result EmployeeDetObj = DTObj.Where(a => a.EmployeeId == EmpIDobj).FirstOrDefault();
                            List<usp_Rpt_GetTimeCard_Result> AttDetList = DTObj.Where(a => a.EmployeeId == EmpIDobj).ToList();
                            StrBuild.Append("<div style=margin:0in;margin-bottom:.0001pt;font-size:12.0pt;font-family:'Times New Roman''>");
                            StrBuild.Append("<p>");
                            //StrBuild.Append("<text>Report Date : " + DateTime.Now.ToShortDateString() + "</text>");
                            StrBuild.Append("<p>Attendance Mail</p>");
                            StrBuild.Append("<p>Dear " + EmployeeDetObj.EmpName + ",</p>");
                            StrBuild.Append("<p>Your attendance details from " + FDate.ToShortDateString() + " to " + TDate.ToShortDateString() + " is as follows</p>");
                            StrBuild.Append("<br/>" + "<text>Code and Name : " + EmployeeDetObj.EmployeeCode + " " + EmployeeDetObj.EmpName + "</text>");
                            //StrBuild.Append("<br/>" + "<text>Name : " + EmployeeDetObj.EmpName + "</text>");
                            StrBuild.Append("<br/>" + "<text>Company : " + EmployeeDetObj.CompanyName + "</text>");
                            StrBuild.Append("<br/>" + "<text>Branch : " + EmployeeDetObj.BranchName + "</text>");
                            StrBuild.Append("</p></div>");

                            //StrBuild.Append("<table class='table table-striped' style='margin:0in;margin-bottom:.0001pt;font-size:12.0pt;font-family:'Times New Roman''>");
                            StrBuild.Append("<table border='1' cellpadding='1' cellspacing='1' style='width:500px'>");
                            StrBuild.Append("<tr>");
                            StrBuild.Append("<td>Date</td>");
                            StrBuild.Append("<td>Shift</td>");
                            StrBuild.Append("<td>Login</td>");
                            StrBuild.Append("<td>Logout</td>");
                            StrBuild.Append("<td>Work Hours</td>");
                            StrBuild.Append("<td>Late Count</td>");
                            StrBuild.Append("<td>Status</td>");
                            StrBuild.Append("</tr>");

                            foreach (var ListObj in AttDetList)
                            {
                                StrBuild.Append("<tr>");
                                StrBuild.Append("<td>" + ListObj.TranDate.ToShortDateString() + "</td>");
                                StrBuild.Append("<td>" + ListObj.ShiftId + "</td>");
                                StrBuild.Append("<td>" + ListObj.Login.ToShortTimeString() + "</td>");
                                StrBuild.Append("<td>" + ListObj.LogOut.ToShortTimeString() + "</td>");
                                StrBuild.Append("<td>" + ListObj.WorkingHrs.ToShortTimeString() + "</td>");
                                StrBuild.Append("<td>" + ListObj.LateCount + "</td>");
                                StrBuild.Append("<td>" + ListObj.Status + "</td>");
                                StrBuild.Append("</tr>");
                            }
                            StrBuild.Append("</table>");
                            StrBuild.Append("<br/>");
                            StrBuild.Append("<p>");
                            StrBuild.Append("<text>Employee Signature:____________________________________</text><br/><br/><br/>");
                            StrBuild.Append("<text>Manager Signature:____________________________________</text><br/>");
                            StrBuild.Append("<text>(E166 DEEPTI KHAMBE)</text>");
                            StrBuild.Append("</p>");
                            StrBuild.Append("<br/>");

                        }
                        catch
                        {
                            EmailNotSentToEmployee.Add(EmpIDobj.ToString());
                        }

                        StrBuild.Append("</body></html>");
                        string ABC = StrBuild.ToString();
                        QueueObj.Enqueue(ABC);

                    }
                }

                // SendEmail("webapp@sushmatechnology.com", "Test HTML mail", QueueObj.Dequeue());

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// AutoMissPunchEmail
        /// </summary>
        /// <param name="WetosDB"></param>
        /// <param name="AttendanceMailObj"></param>
        /// <param name="EmployeeStr"></param>
        /// <returns></returns>
        public static bool AutoMissPunchEmail(WetosDBEntities WetosDB, MonthlyReportsModel AttendanceMailObj, string EmployeeStr)
        {
            try
            {
                DateTime FDate = new DateTime(2017, 07, 16);
                DateTime TDate = new DateTime(2017, 08, 15);
                //List<DailyTransaction> DTObj = WetosDB.DailyTransactions.Where(a => a.TranDate == Date).ToList();

                List<usp_Rpt_GetTimeCard_Result> DTObj = WetosDB.usp_Rpt_GetTimeCard(FDate, TDate, EmployeeStr, null, null, null, "1", null).ToList();
                List<int> EmpIdList = DTObj.Select(a => a.EmployeeId).Distinct().ToList();

                List<string> EmailNotSentToEmployee = new List<string>();  // Added by Rajas on 31 AUGUST 2017
                Queue<string> QueueObj = new Queue<string>();

                StringBuilder StrBuild = new StringBuilder();

                // for (int i = 1; i < 2; i++)
                //{
                //    foreach (int EmpIDobj in EmpIdList)
                //    {
                //        try
                //        {
                //            StrBuild.Append("<!DOCTYPE html><html><head><title>TEST</title></head><body>");
                //            usp_Rpt_GetTimeCard_Result EmployeeDetObj = DTObj.Where(a => a.EmployeeId == EmpIDobj).FirstOrDefault();
                //            List<usp_Rpt_GetTimeCard_Result> AttDetList = DTObj.Where(a => a.EmployeeId == EmpIDobj).ToList();
                //            StrBuild.Append("<div style=margin:0in;margin-bottom:.0001pt;font-size:12.0pt;font-family:'Times New Roman''>");
                //            StrBuild.Append("<p>");
                //            //StrBuild.Append("<text>Report Date : " + DateTime.Now.ToShortDateString() + "</text>");
                //            StrBuild.Append("Dear Sir/Madam<br />");
                //            StrBuild.Append("This is to remind you to kindly regularize your pending leaves and attendance records on Wetos Desire portal for the following dates.<br /></p>");
                //            StrBuild.Append("The details are as follows.<br />");
                //            StrBuild.Append("For Period 01.06.2017 to 30.06.2017</p>");

                //            //StrBuild.Append("<table class='table table-striped' style='margin:0in;margin-bottom:.0001pt;font-size:12.0pt;font-family:'Times New Roman''>");
                //            StrBuild.Append("<table border='1' cellpadding='1' cellspacing='1' style='width:500px'>");
                //            StrBuild.Append("<tr>");
                //            StrBuild.Append("<td>Date</td>");
                //            StrBuild.Append("<td>Shift</td>");
                //            StrBuild.Append("<td>Login</td>");
                //            StrBuild.Append("<td>Logout</td>");
                //            StrBuild.Append("<td>Work Hours</td>");
                //            StrBuild.Append("<td>Late Count</td>");
                //            StrBuild.Append("<td>Status</td>");
                //            StrBuild.Append("</tr>");

                //            foreach (var ListObj in AttDetList)
                //            {
                //                StrBuild.Append("<tr>");
                //                StrBuild.Append("<td>" + ListObj.TranDate.ToShortDateString() + "</td>");
                //                StrBuild.Append("<td>" + ListObj.ShiftId + "</td>");
                //                StrBuild.Append("<td>" + ListObj.Login.ToShortTimeString() + "</td>");
                //                StrBuild.Append("<td>" + ListObj.LogOut.ToShortTimeString() + "</td>");
                //                StrBuild.Append("<td>" + ListObj.WorkingHrs.ToShortTimeString() + "</td>");
                //                StrBuild.Append("<td>" + ListObj.LateCount + "</td>");
                //                StrBuild.Append("<td>" + ListObj.Status + "</td>");
                //                StrBuild.Append("</tr>");
                //            }
                //            StrBuild.Append("</table>");
                //            StrBuild.Append("<br/>");
                //            StrBuild.Append("<p>");
                //            StrBuild.Append("<text>Employee Signature:____________________________________</text><br/><br/><br/>");
                //            StrBuild.Append("<text>Manager Signature:____________________________________</text><br/>");
                //            StrBuild.Append("<text>(E166 DEEPTI KHAMBE)</text>");
                //            StrBuild.Append("</p>");
                //            StrBuild.Append("<br/>");

                //        }
                //        catch
                //        {
                //            EmailNotSentToEmployee.Add(EmpIDobj.ToString());
                //        }

                //        StrBuild.Append("</body></html>");
                //        string ABC = StrBuild.ToString();
                //        QueueObj.Enqueue(ABC);

                //    }
                //}

                StrBuild.Append("<!DOCTYPE html><html><head></head><body>");
                //usp_Rpt_GetTimeCard_Result EmployeeDetObj = DTObj.Where(a => a.EmployeeId == EmpIDobj).FirstOrDefault();
                //List<usp_Rpt_GetTimeCard_Result> AttDetList = DTObj.Where(a => a.EmployeeId == EmpIDobj).ToList();
                StrBuild.Append("<div style=margin:0in;margin-bottom:.0001pt;font-size:12.0pt;font-family:'Times New Roman''>");
                //StrBuild.Append("<p>");
                //StrBuild.Append("<text>Report Date : " + DateTime.Now.ToShortDateString() + "</text>");
                StrBuild.Append("<p>Hi Pradeep sir<br /></p>");
                StrBuild.Append("<p>Please find attachment for attendance report PDF format in attachments. (With dummy data)<br /></p>");
                StrBuild.Append("<p>Request you to confirm report format.<br /></p>");
                StrBuild.Append("Thanks and regards, <br />");
                StrBuild.Append("Rajas Jog</p>");
                StrBuild.Append("</body></html>");
                string ABC = StrBuild.ToString();

                // SendEmail("", "", ABC); //QueueObj.Dequeue());

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Added by Rajas on 9 SEP 2017
        /// </summary>
        /// <param name="WetosDB"></param>
        /// <param name="Fdate"></param>
        /// <param name="Tdate"></param>
        /// <returns></returns>
        public static bool AttendanceMailFromSchedular(WetosDBEntities WetosDB, DateTime Fdate, DateTime Tdate)
        {
            try
            {
                WetosReportsController.PrintPDFs(WetosDB, Fdate, Tdate);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Added by Rajas on 9 SEP 2017
        /// </summary>
        /// <param name="WetosDB"></param>
        /// <param name="ReportModel"></param>
        /// <returns></returns>
        public bool GenerateTimeCardReport(WetosDBEntities WetosDB, MonthlyReportsModel ReportModel)
        {
            bool RetrunState = false;

            try
            {
                DateTime Fdate = Convert.ToDateTime(ReportModel.FromDate);
                DateTime Tdate = Convert.ToDateTime(ReportModel.ToDate);

                PrintPDFs(WetosDB, Fdate, Tdate);

                RetrunState = true;
            }
            catch
            {
                RetrunState = false;
            }

            return RetrunState;
        }

        /// <summary>
        /// Added by Rajas on 9 SEP 2017
        /// </summary>
        /// <param name="Fdate"></param>
        /// <param name="Tdate"></param>
        /// <returns></returns>
        public static bool PrintPDFs(WetosDBEntities WetosDB, DateTime Fdate, DateTime Tdate)
        {
            try
            {
                #region GENERATE Report

                List<AttendanceMail> AttMailQueueObj = new List<AttendanceMail>();

                List<SP_ActiveEmployeeAsOnDate_Result> EmployeeForReportListObj = WetosDB.SP_ActiveEmployeeAsOnDate(Fdate, Tdate).ToList();

                foreach (var EachEmployee in EmployeeForReportListObj)
                {
                    // ------------------------------------------------REPORT RDLC TimeCardReport START-------------------------------------------
                    //// PRINT START
                    DataTable table5 = new DataTable();
                    DataTable AttSummary = new DataTable();
                    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                    //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                    SqlCommand com1 = new SqlCommand("usp_Rpt_GetTimeCard", con);
                    SqlCommand Com2 = new SqlCommand("usp_Report_GetAllAttendanceSumarry", con);

                    com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018

                    com1.CommandType = CommandType.StoredProcedure;
                    com1.Parameters.Add(new SqlParameter("@Fdate", SqlDbType.DateTime)).Value = Fdate; //ReportModel.FromDate; //new DateTime(2013, 01, 01);
                    com1.Parameters.Add(new SqlParameter("@Tdate", SqlDbType.DateTime)).Value = Tdate; //ReportModel.ToDate; //new DateTime(2013, 01, 01);

                    com1.Parameters.Add(new SqlParameter("@RoleTypeId", SqlDbType.NVarChar, 500)).Value = "1";
                    com1.Parameters.Add(new SqlParameter("@ReportingId", SqlDbType.Int)).Value = DBNull.Value;
                    com1.Parameters.Add(new SqlParameter("@BranchListId", SqlDbType.NVarChar, 500)).Value = DBNull.Value;
                    com1.Parameters.Add(new SqlParameter("@CompanyListId", SqlDbType.NVarChar, 500)).Value = DBNull.Value;
                    com1.Parameters.Add(new SqlParameter("@Empid", SqlDbType.NVarChar)).Value = EachEmployee.EmployeeId.ToString();

                    SqlDataAdapter da1 = new SqlDataAdapter(com1);
                    da1.Fill(table5);

                    Com2.CommandType = CommandType.StoredProcedure;
                    Com2.Parameters.Add(new SqlParameter("@Fdate", SqlDbType.DateTime)).Value = Fdate;
                    Com2.Parameters.Add(new SqlParameter("@Tdate", SqlDbType.DateTime)).Value = Tdate;
                    Com2.Parameters.Add(new SqlParameter("@EmpidList", SqlDbType.NVarChar)).Value = EachEmployee.EmployeeId.ToString();

                    SqlDataAdapter da2 = new SqlDataAdapter(Com2);
                    Com2.CommandTimeout = 3000;
                    da2.Fill(AttSummary);

                    //// ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                    //string GeneratedFileName = GenerateReport(WetosDB, table5, "~/Reports/TimeCardNew.rdlc",
                    //    "TimeCardNew", "~/User_Data/download/", "Leave_Balance_Report_");

                    //Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

                    Warning[] warnings;
                    //string[] streamids;
                    string mimeType;
                    string encoding;
                    //string filenameExtension;
                    //string filePath;

                    LocalReport localReport = new LocalReport();
                    localReport.ReportPath = HostingEnvironment.MapPath("~/Reports/TimeCardNew.rdlc"); //  MISSING

                    localReport.DataSources.Clear();

                    localReport.DataSources.Add(new ReportDataSource("TimeCardNew", table5));
                    localReport.DataSources.Add(new ReportDataSource("AttendanceSummary", AttSummary));

                    localReport.Refresh();

                    // Updated by Rajas on 14 MARCH 2017
                    string reportType = string.Empty;
                    string PdfFileName = string.Empty;
                    reportType = "PDF";

                    PdfFileName = HostingEnvironment.MapPath("~/User_Data/download/AttendanceMailReport/") + EachEmployee.EmployeeCode + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf";
                    //AddAuditTrail("path " + PdfFileName);


                    //string mimeType;
                    //string encoding;
                    string fileNameExtension;

                    // Displaying Name from Id, which is coming from reportmodel
                    //Added by Rajas on 14 OCT 2016
                    //string CompanyName = WetosDB.Companies.Where(a => a.CompanyId == ReportModel.CompanyId).Select(a => a.CompanyName).FirstOrDefault();
                    //string Branchname = WetosDB.Branches.Where(a => a.BranchId == ReportModel.BranchId).Select(a => a.BranchName).FirstOrDefault();
                    //string DepartmentName = WetosDB.Departments.Where(a => a.DepartmentId == ReportModel.DepartmentId).Select(a => a.DepartmentName).FirstOrDefault();

                    ReportParameter ReportParameter1 = new ReportParameter("ReportParameter1", string.Empty); // Company name
                    ReportParameter ReportParameter2 = new ReportParameter("ReportParameter2", "Time Card Report"); // report name
                    ReportParameter ReportParameter3 = new ReportParameter("ReportParameter3", Fdate.ToString()); // from date
                    ReportParameter ReportParameter4 = new ReportParameter("ReportParameter4", Tdate.ToString()); // to date
                    // blank parameters to be used later as per requirement 
                    ReportParameter ReportParameter5 = new ReportParameter("ReportParameter5", "");
                    ReportParameter ReportParameter6 = new ReportParameter("ReportParameter6", "");
                    ReportParameter ReportParameter7 = new ReportParameter("ReportParameter7", "");
                    localReport.SetParameters(new ReportParameter[] { ReportParameter1, ReportParameter2, ReportParameter3, ReportParameter4, ReportParameter5, ReportParameter6, ReportParameter7 });

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
                    using (FileStream fs = new FileStream(PdfFileName, FileMode.Create))
                    {
                        fs.Write(renderedBytes, 0, renderedBytes.Length);
                    }

                    string EmployeeName = EachEmployee.FirstName + " " + EachEmployee.LastName;
                    string MailBody = string.Empty;

                    AttendanceMailBody(Fdate, Tdate, EmployeeName, ref MailBody);

                    AttendanceMail AttendanceMailObj = new AttendanceMail();

                    AttendanceMailObj.AttachmentPath = PdfFileName;
                    AttendanceMailObj.ToEmail = EachEmployee.Email == null ? string.Empty : EachEmployee.Email.Trim();  // "webapp@sushmatechnology.com"; 
                    AttendanceMailObj.FileNameInAttachment = "Attendance Report.pdf";
                    AttendanceMailObj.MailBody = MailBody;
                    AttendanceMailObj.EmployeeCode = EachEmployee.EmployeeCode;
                    AttendanceMailObj.EmployeeId = EachEmployee.EmployeeId; // Added by Rajas on 15 SEP 2017

                    AttMailQueueObj.Add(AttendanceMailObj);

                    //Response.AddHeader("Content-Disposition", "attachment; filename=" + "TimeCardReport" + ".pdf;"); // Updated by Rajas on 19 JAN 2017
                }

                //AddAuditTrail("PDF ready for send, Count : " + AttMailQueueObj.Count());
                SendEmail(WetosDB, AttMailQueueObj);
                #endregion GENERATE Report

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Added by Rajas on 9 SEP 2017
        /// Generate Email body
        /// </summary>
        /// <param name="Fdate"></param>
        /// <param name="Tdate"></param>
        /// <param name="EmployeeName"></param>
        /// <param name="MailBody"></param>
        /// <returns></returns>
        public static bool AttendanceMailBody(DateTime Fdate, DateTime Tdate, string EmployeeName, ref string MailBody)
        {
            try
            {
                StringBuilder StrBuild = new StringBuilder();
                StrBuild.Append("<!DOCTYPE html><html><head></head><body>");
                StrBuild.Append("<div style=margin:0in;margin-bottom:.0001pt;font-size:12.0pt;font-family:'Times New Roman''>");
                StrBuild.Append("<p>");
                StrBuild.Append("<p>Attendance Mail</p>");
                StrBuild.Append("<p>Dear " + EmployeeName + ",</p>");
                StrBuild.Append("<p>Your attendance details from " + Fdate.ToShortDateString() + " to " + Tdate.ToShortDateString() + " is as attached along with this mail.</p>");
                StrBuild.Append("<p>Thank you</p>");
                StrBuild.Append("<p>Regards,</p>");
                StrBuild.Append("<p>WETOS D'zire</p>");
                StrBuild.Append("<p>Note: This is auto generated mail from System please do not reply. For any queries Contact HR Department</p>");
                StrBuild.Append("</p></div>");
                StrBuild.Append("</body></html>");
                MailBody = StrBuild.ToString();

                return true;
            }
            catch
            {
                return false;
            }

        }

        //public bool ThreadingTest()
        //{
        //    string ABC = string.Empty;
        //    Thread TestThreadObj = new Thread(new ParameterizedThreadStart(SendEmailStart));
        //    return true;
        //}

        //private static void SendEmailStart()
        //{
        //    SendEmail("webapp@sushmatechnology.com", "Test HTML mail", ABC);
        //}

        /// <summary>
        /// SendEmail
        /// </summary>
        /// <param name="ToEmail"></param>
        /// <param name="Subject"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        //public static bool SendEmail(string ToEmail, string Subject, string Message)
        public static bool SendEmail(WetosDBEntities WetosDB, List<AttendanceMail> AttMailQueue)
        {
            // SEND EMAIL START
            string SMTPServerName = ConfigurationManager.AppSettings["SMTPServerName"];
            string SMTPUsername = ConfigurationManager.AppSettings["SMTPUsername"];
            string SMTPPassword = ConfigurationManager.AppSettings["SMTPPassword"];
            string SMTPPort = ConfigurationManager.AppSettings["SMTPPort"];
            string Contact1 = ConfigurationManager.AppSettings["Contact1"];

            int SMTPPortInt = Convert.ToInt32(SMTPPort);

            bool ReturnStatus = false;

            try
            {
                // MailMessage msg = new MailMessage();

                // MailAddress from = new MailAddress(SMTPUsername); //"info@foodpatronservices.com");
                // StringBuilder sb = new StringBuilder();              

                SmtpClient smtp = new SmtpClient();
                smtp.Host = SMTPServerName;
                smtp.EnableSsl = true;
                smtp.Port = SMTPPortInt; // 25;

                //smtp.Credentials = new System.Net.NetworkCredential("info@foodpatronservices.com", "info@fps");
                smtp.Credentials = new System.Net.NetworkCredential(SMTPUsername, SMTPPassword);//"info@foodpatronservices.com", "info@fps");

                MailMessage msg = new MailMessage();

                foreach (AttendanceMail SendMail in AttMailQueue)
                {
                    try
                    {

                        msg = new MailMessage();

                        msg.IsBodyHtml = true;

                        msg.From = new MailAddress(SMTPUsername); //"info@foodpatronservices.com");

                        //TO
                        // msg.To.Add("webapp@sushmatechnology.com");
                        msg.To.Add(SendMail.ToEmail);

                        // cc
                        //msg.CC.Add("mjoshi@sushmatechnology.com");
                        msg.CC.Add(Contact1);

                        //BCC
                        //msg.Bcc.Add(Support);
                        msg.Subject = "Attendance Report for " + SendMail.EmployeeCode; // "Contact Us";

                        msg.Body = SendMail.MailBody; //Message;

                        System.Net.Mail.Attachment AttachedFile = new System.Net.Mail.Attachment(SendMail.AttachmentPath);

                        AttachedFile.Name = SendMail.FileNameInAttachment; //"Time Card report";

                        msg.Attachments.Add(AttachedFile);

                        //smtp.Send(msg);

                        string NewRecord = "Email Sent to Employee code " + SendMail.EmployeeCode + " on EmailId " + SendMail.ToEmail;
                        string Message = string.Empty;
                        WetosAdministrationController.GenerateAuditLogsInsert(WetosDB, SendMail.EmployeeId, "SendMail", NewRecord, ref Message);
                    }
                    catch
                    {

                    }
                }

                msg.Dispose();

                return ReturnStatus = true;

                //return RedirectToAction("Index", "Home");
            }
            catch (System.Exception ex)
            {
                return ReturnStatus;
            }

            // SEND EMAIL END

            //return View();
        }


        /// <summary>
        /// Requisition Application - Requisition application partial view
        /// </summary>
        /// <returns></returns>
        public ActionResult RequisitionApplication()
        {
            // Drop down for Branch name
            #region CODE FOR GET BRANCH LIST ADDED BY SHRADDHA ON 30 MAR 2018
            int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
            //var BranchName = WetosDB.Branches.Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList();
            var BranchName = WetosDB.SP_GetBranchList(EmployeeId).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList(); //.Select(m => new { BranchId = m.BranchId, BranchName = m.BranchName}).ToList();
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
            #endregion

            ViewBag.BranchNameList = new SelectList(BranchName, "BranchId", "BranchName").ToList();
            return View();
        }

        /// <summary>
        ///  // leave application partial view
        /// </summary>
        /// <returns></returns>
        public ActionResult LeaveApplication()
        {
            // Drop down for Branch name
            #region CODE FOR GET BRANCH LIST ADDED BY SHRADDHA ON 30 MAR 2018
            int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
            //var BranchName = WetosDB.Branches.Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList();
            var BranchName = WetosDB.SP_GetBranchList(EmployeeId).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList(); //.Select(m => new { BranchId = m.BranchId, BranchName = m.BranchName}).ToList();
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
            #endregion

            ViewBag.BranchNameList = new SelectList(BranchName, "BranchId", "BranchName").ToList();
            return View();
        }

        /// <summary>
        /// Payslip partial view
        /// </summary>
        /// <returns></returns>
        public ActionResult PaySlipReport()
        {
            var FinancialYear = WetosDB.FinancialYears.Select(a => new { FinancialYearId = a.FinancialYearId, FinancialYearName = a.FinancialName }).ToList();
            ViewBag.FinancialYearList = new SelectList(FinancialYear, "FinancialYearId", "FinancialYearName").ToList();
            return View();
        }

        // Monthly report end

        // Holiday list start

        /// <summary>
        /// Holiday List Index
        /// </summary>
        /// <returns></returns>
        public ActionResult HolidayListIndex()
        {
            // Drop down for Branch name
            #region CODE FOR GET BRANCH LIST ADDED BY SHRADDHA ON 30 MAR 2018
            int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
            //var BranchName = WetosDB.Branches.Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList();
            var BranchName = WetosDB.SP_GetBranchList(EmployeeId).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList(); //.Select(m => new { BranchId = m.BranchId, BranchName = m.BranchName}).ToList();
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
            #endregion

            ViewBag.BranchNameList = new SelectList(BranchName, "BranchId", "BranchName").ToList();
            return View();
        }

        // Holiday List end

        // Mis Report Start 
        public ActionResult MisReportsIndex()
        {
            return View();
        }
        // Mis Reports End

        /// <summary>
        /// Commented by Rajas on 30 JAN 2017
        /// Json return for to get Branch dropdown list on basis of company selection
        /// Added by Rajas on 27 DEC 2016
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

            #region BRANCH DROPDOWN
            int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            List<UserRole> UserRoleList = WetosDB.UserRoles.Where(a => a.EmployeeId == EmployeeId).ToList();
            List<WetosDB.Company> CompanyList = new List<WetosDB.Company>();
            List<SP_GetBranchList_Result> BranchListNew = new List<SP_GetBranchList_Result>();
            //if (UserRoleList != null && UserRoleList.Count > 0)
            //{
            //    foreach (UserRole UserRoleObj in UserRoleList)
            //    {
            //        int BranchId = Convert.ToInt32(UserRoleObj.BranchId == null ? 0 : UserRoleObj.BranchId);
            //        if (BranchId > 0)
            //        {
            //            WetosDB.Branch BranchObjNew = BranchListNew.Where(a => a.BranchId == BranchId).FirstOrDefault();
            //            if (BranchObjNew == null)
            //            {
            //                BranchObjNew = WetosDB.Branches.Where(a => a.BranchId == BranchId).FirstOrDefault();
            //                BranchListNew.Add(BranchObjNew);
            //            }

            //        }
            //        else
            //        {
            //            BranchListNew = WetosDB.Branches.Where(a => (a.MarkedAsDelete == null || a.MarkedAsDelete == 0) && a.Company.CompanyId == SelCompanyId).ToList();
            //        }
            //    }
            //}
            #region CODE FOR GET BRANCH LIST ADDED BY SHRADDHA ON 30 MAR 2018
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
            //var BranchName = WetosDB.Branches.Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList();
            var BranchList = WetosDB.SP_GetBranchList(EmployeeId).Where(a => a.CompanyId == SelCompanyId).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList(); //.Select(m => new { BranchId = m.BranchId, BranchName = m.BranchName}).ToList();
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
            #endregion
            #endregion

            // Updated by Rajas on 30 MAY 2017
            //var BranchList = WetosDB.Branches.Where(a => a.Company.CompanyId == SelCompanyId
            //    && (a.MarkedAsDelete == null || a.MarkedAsDelete == 0)).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList();

            return Json(BranchList);
        }

        /// <summary>
        /// Json return for to get Employee dropdown list on basis of department selection
        /// Added by Shraddha on 18 jan 2018
        /// </summary>
        /// <returns></returns>
        //public JsonResult GetEmployeeDateSelection(int? Companyid, int? Branchid, int? Departmentid, int? EmployeeTypeId, DateTime? Fromdate, DateTime? Todate)
        [HttpPost]
        public ActionResult GetEmployeeDateSelectionEx(string DepartmentStrEx, int? Companyid, int? Branchid, int? EmployeeTypeId, DateTime? Fromdate, DateTime? Todate)
        {
            if (Fromdate == null && Todate == null)
            {
                Fromdate = WetosDB.DailyTransactions.OrderBy(a => a.TranDate).Select(a => a.TranDate).FirstOrDefault();
                Todate = DateTime.Now;
            }

            int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);

            Companyid = Companyid == 0 ? (int?)null : Companyid;
            Branchid = Branchid == 0 ? (int?)null : Branchid;
            EmployeeTypeId = EmployeeTypeId == 0 ? (int?)null : EmployeeTypeId;

            // LIST OF DEPARTMENT
            string[] DepartmentIdArray = string.IsNullOrEmpty(DepartmentStrEx) == true ? string.Empty.Split(',') : DepartmentStrEx.Split(',');

            //Departmentid = Departmentid == 0 ? (int?)null : Departmentid;

            //var EmployeeObj = WetosDB.SP_GetEmployeeCSVForReportFilter(Fromdate, Todate, Companyid, Branchid, EmployeeTypeId, Departmentid)
            //    .Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " " + a.FirstName + " " + a.LastName }).ToList();

            List<SP_GetEmployeeCSVForReportFilterNew_Result> EmpDeptList = new List<SP_GetEmployeeCSVForReportFilterNew_Result>();

            foreach (string SingleDepartmentStr in DepartmentIdArray)
            {
                int DepartmentIntId;
                bool isNumeric = int.TryParse(SingleDepartmentStr, out DepartmentIntId);

                List<SP_GetEmployeeCSVForReportFilterNew_Result> TempEmpDeptList = WetosDB.SP_GetEmployeeCSVForReportFilterNew(Fromdate, Todate, Companyid, Branchid, EmployeeTypeId, DepartmentIntId, EmployeeId).ToList();
                //    .Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " " + a.FirstName + " " + a.LastName }).ToList();

                EmpDeptList.AddRange(TempEmpDeptList);
            }

            var EmployeeObj = EmpDeptList.Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " " + a.FirstName + " " + a.LastName }).ToList();

            return Json(EmployeeObj);
            // Updated on Rajas on 8 FEB 2017 for reducing ajax return list and json return as per required parameter END
        }

        //[HttpPost]
        //public ActionResult GetEmployeeDateSelection(int? Companyid, int? Branchid, int? EmployeeTypeId, DateTime? Fromdate, DateTime? Todate)
        //{
        //    if (Fromdate == null && Todate == null)
        //    {
        //        Fromdate = WetosDB.DailyTransactions.OrderBy(a => a.TranDate).Select(a => a.TranDate).FirstOrDefault();
        //        Todate = DateTime.Now;
        //    }

        //    int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);

        //    Companyid = Companyid == 0 ? (int?)null : Companyid;
        //    Branchid = Branchid == 0 ? (int?)null : Branchid;
        //    EmployeeTypeId = EmployeeTypeId == 0 ? (int?)null : EmployeeTypeId;

        //    // LIST OF DEPARTMENT
        //    string[] DepartmentIdArray = string.IsNullOrEmpty(DepartmentStrEx) == true ? string.Empty.Split(',') : DepartmentStrEx.Split(',');

        //    //Departmentid = Departmentid == 0 ? (int?)null : Departmentid;

        //    //var EmployeeObj = WetosDB.SP_GetEmployeeCSVForReportFilter(Fromdate, Todate, Companyid, Branchid, EmployeeTypeId, Departmentid)
        //    //    .Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " " + a.FirstName + " " + a.LastName }).ToList();

        //    List<SP_GetEmployeeCSVForReportFilterNew_Result> EmpDeptList = new List<SP_GetEmployeeCSVForReportFilterNew_Result>();

        //    foreach (string SingleDepartmentStr in DepartmentIdArray)
        //    {
        //        int DepartmentIntId;
        //        bool isNumeric = int.TryParse(SingleDepartmentStr, out DepartmentIntId);

        //        List<SP_GetEmployeeCSVForReportFilterNew_Result> TempEmpDeptList = WetosDB.SP_GetEmployeeCSVForReportFilterNew(Fromdate, Todate, Companyid, Branchid, EmployeeTypeId, DepartmentIntId, EmployeeId).ToList();
        //        //    .Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " " + a.FirstName + " " + a.LastName }).ToList();

        //        EmpDeptList.AddRange(TempEmpDeptList);
        //    }

        //    var EmployeeObj = EmpDeptList.Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " " + a.FirstName + " " + a.LastName }).ToList();

        //    return Json(EmployeeObj);
        //    // Updated on Rajas on 8 FEB 2017 for reducing ajax return list and json return as per required parameter END
        //}

        // Region added by Rajas on 8 FEB 2017
        #region COMMENTED CODE
        /// <summary>
        /// To get Employee partial view list on basis of button selection
        /// Added by Rajas 7 JAN 2017
        /// </summary>
        /// <returns></returns>
        //public ActionResult GetEmployeeList(int Companyid, int Branchid, int Departmentid)
        //{

        //    //int SelEmployeeId = 0;
        //    //if (!string.IsNullOrEmpty(Employeeid))
        //    //{
        //    //    if (Employeeid.ToUpper() != "NULL")
        //    //    {
        //    //        SelEmployeeId = Convert.ToInt32(Employeeid);
        //    //    }
        //    //}

        //    List<Employee> EmployeeList = WetosDB.Employees.Where(a => a.BranchId == Branchid && a.CompanyId == Companyid && a.DepartmentId == Departmentid).ToList();

        //    //return Json(EmployeeList);
        //    return PartialView(EmployeeList);
        //}

        /// <summary>
        /// Added by Rajas on 7 JAN 2017 for posting checked employees
        /// </summary>
        /// <param name="EmployeeObj"></param>
        /// <returns></returns>
        //[HttpPost]
        //public ActionResult GetEmployeeList(WetosDB.Employee EmployeeObj)
        //{
        //    AddAuditTrail("");

        //    return RedirectToAction("MonthlyReportsIndex", new { EmployeeId = EmployeeObj.EmployeeId });

        //}
        #endregion

        /// <summary>
        /// Get LeaveReport For Selected Month
        /// </summary>
        /// <param name="EmployeeDataObj"></param>
        /// <param name="LeaveCalculationSheetModelObj"></param>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        public void GetLeaveReportForSelectedMonth(SP_LeaveCalculationSheetNew_Result EmployeeDataObj, ref LeaveCalculationSheetModel LeaveCalculationSheetModelObj, DateTime FromDate, DateTime ToDate)
        {
            float LateCount = WetosDB.DailyTransactions.Where(a => a.EmployeeId == EmployeeDataObj.EmployeeId && a.TranDate >= FromDate && a.TranDate <= ToDate && a.Status == "AAPP^" && a.Late != null && a.LateCount != null && a.Status != null).Count();
            int EmployeeId = Convert.ToInt32(EmployeeDataObj.EmployeeId);


            LeaveCalculationSheetModelObj.LateMarks = LateCount.ToString();
            float LateMarksAsAbsentDays = 0.00f;
            if (LateCount > 0)
            {
                LateMarksAsAbsentDays = (float)((LateCount) * 0.5);
            }

            // Updated by Shraddha on 26 JULY 2017 START
            float FirstHalfAbsentCount = WetosDB.DailyTransactions.Where(a => a.EmployeeId == EmployeeDataObj.EmployeeId && a.TranDate >= FromDate && a.TranDate <= ToDate && a.Status == "AAPP^" || a.Status == "AAPP").Count();
            float SecondHalfAbsentCount = WetosDB.DailyTransactions.Where(a => a.EmployeeId == EmployeeDataObj.EmployeeId && a.TranDate >= FromDate && a.TranDate <= ToDate && (a.Status == "PPAA" || a.Status == "PPAA^")).Count();
            float FullDayAbsentCount = WetosDB.DailyTransactions.Where(a => a.EmployeeId == EmployeeDataObj.EmployeeId && a.TranDate >= FromDate && a.TranDate <= ToDate && (a.Status == "AAAA" || a.Status == "AAAA^")).Count();
            // Updated by Shraddha on 26 JULY 2017 END

            float FirstHalfAbsentDays = 0.00f;
            if (FirstHalfAbsentCount > 0)
            {
                FirstHalfAbsentDays = (float)((FirstHalfAbsentCount) * 0.5);
            }

            float SecondHalfAbsentDays = 0.00f;
            if (SecondHalfAbsentCount > 0)
            {
                SecondHalfAbsentDays = (float)((SecondHalfAbsentCount) * 0.5);
            }

            float FullDayAbsentDays = 0.0f;
            if (FullDayAbsentCount > 0)
            {
                FullDayAbsentDays = (float)((FullDayAbsentCount) * 1);
            }

            //BELOW CODE COMMENTED BY SHRADDHA AND ADDED NEW CODE BECAUSE NEW FUNCTION GetLeaveDetailsForReport CREATED ON DASHBOARDCONTROLLER FOR GETTING CORRECT LEAVE DATA ON 26 JULY 2017 START
            //List<SP_LeaveTableDataNew_Result> SP_LeaveTableDataNewList = DashboardController.GetLeaveDetailsOnallControllersNew(WetosDB, EmployeeId, ToDate);
            List<SP_LeaveTableDataNew_Result> SP_LeaveTableDataNewList = DashboardController.GetLeaveDetailsForReport(WetosDB, EmployeeId, ToDate, EmployeeDataObj.LeaveType);
            //BELOW CODE COMMENTED BY SHRADDHA AND ADDED NEW CODE BECAUSE NEW FUNCTION GetLeaveDetailsForReport CREATED ON DASHBOARDCONTROLLER FOR GETTING CORRECT LEAVE DATA ON 26 JULY 2017 END
            if (SP_LeaveTableDataNewList.Count > 0)
            {
                foreach (SP_LeaveTableDataNew_Result SP_LeaveTableDataNewObj in SP_LeaveTableDataNewList)
                {
                    if (SP_LeaveTableDataNewObj != null)
                    {
                        LeaveCalculationSheetModelObj.ConfirmDate = EmployeeDataObj.ConfirmDate == null ? "" : EmployeeDataObj.ConfirmDate.Value.ToString("MMM-yyyy");
                        LeaveCalculationSheetModelObj.UnpaidLeaves = SP_LeaveTableDataNewObj.TotalUnPaidLeaves == null ? "0" : SP_LeaveTableDataNewObj.TotalUnPaidLeaves.ToString();
                        LeaveCalculationSheetModelObj.AppliedDays = SP_LeaveTableDataNewObj.TotalDeductableDays == null ? "0" : SP_LeaveTableDataNewObj.TotalDeductableDays.ToString();
                        LeaveCalculationSheetModelObj.PaidLeaves = SP_LeaveTableDataNewObj.TotalPaidLeaves == null ? "0" : SP_LeaveTableDataNewObj.TotalPaidLeaves.ToString();

                        LeaveCalculationSheetModelObj.CurrentBalance = ((SP_LeaveTableDataNewObj.OpeningBalance - Convert.ToDouble(SP_LeaveTableDataNewObj.LeaveUsed)) + SP_LeaveTableDataNewObj.TotalPaidLeaves).ToString();
                        LeaveCalculationSheetModelObj.ClosingBalance = (SP_LeaveTableDataNewObj.OpeningBalance - Convert.ToDouble(SP_LeaveTableDataNewObj.LeaveUsed)).ToString();

                        LeaveCalculationSheetModelObj.EmployeeNumber = EmployeeDataObj.EmployeeCode == null ? "0" : EmployeeDataObj.EmployeeCode.ToString();
                        LeaveCalculationSheetModelObj.NameOfEmployee = (EmployeeDataObj.FirstName == null ? "" : EmployeeDataObj.FirstName.ToString()) + ' ' + (EmployeeDataObj.LastName == null ? "" : EmployeeDataObj.LastName.ToString());
                        LeaveCalculationSheetModelObj.LeaveType = EmployeeDataObj.LeaveType == null ? "" : EmployeeDataObj.LeaveType.ToString();
                        LeaveCalculationSheetModelObj.LeaveUsed = SP_LeaveTableDataNewObj.LeaveUsed == null ? "" : SP_LeaveTableDataNewObj.LeaveUsed.ToString();
                        LeaveCalculationSheetModelObj.OpeningBalance = SP_LeaveTableDataNewObj.OpeningBalance == null ? "0" : (SP_LeaveTableDataNewObj.OpeningBalance - SP_LeaveTableDataNewObj.LeaveUsed).ToString();
                        //LeaveCalculationSheetModelObj.PreviousBalance = EmployeeDataObj.PreviousBalance == null ? "" : EmployeeDataObj.PreviousBalance.ToString();
                        //LeaveCalculationSheetModelObj.LeaveStatus = EmployeeDataObj.Status == null ? "" : EmployeeDataObj.Status.ToString();


                        LeaveCalculationSheetModelObj.TotalDays = System.DateTime.DaysInMonth(ToDate.Year, ToDate.Month).ToString();
                        LeaveCalculationSheetModelObj.TotalPayableDays = (System.DateTime.DaysInMonth(ToDate.Year, ToDate.Month) - (Convert.ToDouble(LeaveCalculationSheetModelObj.UnpaidLeaves))).ToString();
                        LeaveCalculationSheetModelObj.TotalLWP = (LeaveCalculationSheetModelObj.UnpaidLeaves) == null ? "0" : (Convert.ToDouble(LeaveCalculationSheetModelObj.UnpaidLeaves) + Convert.ToDouble(LateMarksAsAbsentDays) + Convert.ToDouble(FirstHalfAbsentDays) + Convert.ToDouble(SecondHalfAbsentDays) + Convert.ToDouble(FullDayAbsentDays)).ToString();
                        LeaveCalculationSheetModelObj.AllowedLeaves = SP_LeaveTableDataNewObj.LeaveAllowed.ToString();

                    }
                }
            }

        }

        /// <summary>
        /// CODE FOR DISPLAYING LEAVE CALCULATION SHEET
        /// ADDED BY SHRADDHA ON 07 FEB 2017
        /// <returns></returns>
        [HttpPost]
        public ActionResult LeaveCalculationReport(DateTime FromDate, DateTime ToDate)
        {
            try
            {
                List<LeaveCalculationSheetModel> LeaveCalculationSheetList = new List<LeaveCalculationSheetModel>();
                if (FromDate.Year != ToDate.Year)
                {

                    ViewBag.ReturnMessage = "From Date and Todate should be of same month.";
                    return PartialView(LeaveCalculationSheetList);
                }

                else
                {
                    #region COMMENTED CODE
                    //if (FromDate.Month != ToDate.Month)
                    //{
                    //    ViewBag.ReturnMessage = "From Date and To date should be of same month.";
                    //    return PartialView(LeaveCalculationSheetList);
                    //}
                    //else
                    //{
                    //    ViewBag.FromDate = FromDate.ToString("MMM-yyyy");
                    //    ViewBag.ToDate = ToDate.ToString("MMM-yyyy");

                    //    var EmployeeDataList = WetosDB.SP_LeaveCalculationSheet(FromDate, ToDate).ToList();

                    //    foreach (var EmployeeDataObj in EmployeeDataList)
                    //    {
                    //        float LateCount = WetosDB.DailyTransactions.Where(a => a.EmployeeId == EmployeeDataObj.EmployeeId && a.TranDate >= FromDate && a.TranDate <= ToDate && a.Status == "AAPP^" && a.Late != null && a.LateCount != null && a.Status != null).Count();

                    //        //Code CHANGED BY SHRADDHA ON 08 FEB 2017 START    
                    //        LeaveCalculationSheetModel LeaveCalculationSheetModelObj = new LeaveCalculationSheetModel();

                    //        LeaveCalculationSheetModelObj.LateMarks = LateCount.ToString();
                    //        float LateMarksAsAbsentDays = 0.00f;
                    //        if (LateCount > 0)
                    //        {
                    //            LateMarksAsAbsentDays = (float)((LateCount) * 0.5);
                    //        }


                    //        float FirstHalfAbsentCount = WetosDB.DailyTransactions.Where(a => a.EmployeeId == EmployeeDataObj.EmployeeId && a.TranDate >= FromDate && a.TranDate <= ToDate && a.Status == "AAPP^" && a.Late == null && a.LateCount == null).Count();
                    //        float SecondHalfAbsentCount = WetosDB.DailyTransactions.Where(a => a.EmployeeId == EmployeeDataObj.EmployeeId && a.TranDate >= FromDate && a.TranDate <= ToDate && (a.Status == "PPAA" || a.Status == "PPAA^")).Count();
                    //        float FullDayAbsentCount = WetosDB.DailyTransactions.Where(a => a.EmployeeId == EmployeeDataObj.EmployeeId && a.TranDate >= FromDate && a.TranDate <= ToDate && (a.Status == "PPAA" || a.Status == "PPAA^")).Count();

                    //        float FirstHalfAbsentDays = 0.00f;
                    //        if (FirstHalfAbsentCount > 0)
                    //        {
                    //            FirstHalfAbsentDays = (float)((FirstHalfAbsentCount) * 0.5);
                    //        }

                    //        float SecondHalfAbsentDays = 0.00f;
                    //        if (SecondHalfAbsentCount > 0)
                    //        {
                    //            SecondHalfAbsentDays = (float)((SecondHalfAbsentCount) * 0.5);
                    //        }

                    //        float FullDayAbsentDays = 0.0f;
                    //        if (FullDayAbsentCount > 0)
                    //        {
                    //            FullDayAbsentDays = (float)((FullDayAbsentCount) * 1);
                    //        }

                    //        List<SP_LeaveTableDataNew_Result> SP_LeaveTableDataNewList = DashboardController.GetLeaveDetailsOnallControllersNew(WetosDB, EmployeeDataObj.EmployeeId, ToDate);
                    //        if (SP_LeaveTableDataNewList.Count > 0)
                    //        {
                    //            foreach (SP_LeaveTableDataNew_Result SP_LeaveTableDataNewObj in SP_LeaveTableDataNewList)
                    //            {
                    //                if (SP_LeaveTableDataNewObj != null)
                    //                {
                    //                    Double LeaveUsedForSelectedMonth = 0;
                    //                    LeaveUsedForSelectedMonth = Convert.ToDouble(WetosDB.LeaveApplications
                    //                      .Where(a => a.FromDate >= FromDate && a.ToDate <= ToDate && a.EmployeeId == EmployeeDataObj.EmployeeId && a.LeaveType == EmployeeDataObj.LeaveType).Sum(a => a.ActualDays));

                    //                    //LeaveCalculationSheetModelObj.AppliedDays = EmployeeDataObj.AppliedDays == null ? "0" : EmployeeDataObj.AppliedDays.ToString();
                    //                    //LeaveCalculationSheetModelObj.PaidLeaves = EmployeeDataObj.ActualDays == null ? "0" : EmployeeDataObj.ActualDays.ToString();
                    //                    //LeaveCalculationSheetModelObj.UnpaidLeaves = (Convert.ToDouble(EmployeeDataObj.AppliedDays) - Convert.ToDouble(EmployeeDataObj.ActualDays)).ToString();

                    //                    LeaveCalculationSheetModelObj.ConfirmDate = EmployeeDataObj.ConfirmDate == null ? "" : EmployeeDataObj.ConfirmDate.ToString();
                    //                    LeaveCalculationSheetModelObj.UnpaidLeaves = SP_LeaveTableDataNewObj.TotalUnPaidLeaves == null ? "0" : SP_LeaveTableDataNewObj.TotalUnPaidLeaves.ToString();
                    //                    LeaveCalculationSheetModelObj.AppliedDays = SP_LeaveTableDataNewObj.TotalDeductableDays == null ? "0" : SP_LeaveTableDataNewObj.TotalDeductableDays.ToString();
                    //                    LeaveCalculationSheetModelObj.PaidLeaves = SP_LeaveTableDataNewObj.TotalPaidLeaves == null ? "0" : SP_LeaveTableDataNewObj.TotalPaidLeaves.ToString();

                    //                    LeaveCalculationSheetModelObj.CurrentBalance = ((SP_LeaveTableDataNewObj.OpeningBalance - Convert.ToDouble(SP_LeaveTableDataNewObj.LeaveUsed)) + SP_LeaveTableDataNewObj.TotalPaidLeaves).ToString();
                    //                    LeaveCalculationSheetModelObj.ClosingBalance = (SP_LeaveTableDataNewObj.OpeningBalance - Convert.ToDouble(SP_LeaveTableDataNewObj.LeaveUsed)).ToString();

                    //                    //}
                    //                    LeaveCalculationSheetModelObj.EmployeeNumber = EmployeeDataObj.EmployeeCode == null ? "0" : EmployeeDataObj.EmployeeCode.ToString();
                    //                    //LeaveCalculationSheetModelObj.Encash = EmployeeDataObj.Encash == null ? "" : EmployeeDataObj.Encash.ToString();
                    //                    LeaveCalculationSheetModelObj.NameOfEmployee = (EmployeeDataObj.FirstName == null ? "": EmployeeDataObj.FirstName.ToString()) + ' ' + (EmployeeDataObj.LastName == null ? "":  EmployeeDataObj.LastName.ToString());


                    //                    LeaveCalculationSheetModelObj.LeaveType = EmployeeDataObj.LeaveType == null ? "" : EmployeeDataObj.LeaveType.ToString();
                    //                    //LeaveCalculationSheetModelObj.LeaveUsed = EmployeeDataObj.LeaveUsed == null ? "" : EmployeeDataObj.LeaveUsed.ToString();
                    //                    LeaveCalculationSheetModelObj.OpeningBalance = SP_LeaveTableDataNewObj.OpeningBalance == null ? "0" : SP_LeaveTableDataNewObj.OpeningBalance.ToString();
                    //                    //LeaveCalculationSheetModelObj.PreviousBalance = EmployeeDataObj.PreviousBalance == null ? "" : EmployeeDataObj.PreviousBalance.ToString();
                    //                    //LeaveCalculationSheetModelObj.LeaveStatus = EmployeeDataObj.Status == null ? "" : EmployeeDataObj.Status.ToString();


                    //                    LeaveCalculationSheetModelObj.TotalDays = System.DateTime.DaysInMonth(ToDate.Year, ToDate.Month).ToString();
                    //                    LeaveCalculationSheetModelObj.TotalPayableDays = (System.DateTime.DaysInMonth(ToDate.Year, ToDate.Month) - (Convert.ToDouble(LeaveCalculationSheetModelObj.UnpaidLeaves))).ToString();
                    //                    LeaveCalculationSheetModelObj.TotalLWP = (LeaveCalculationSheetModelObj.UnpaidLeaves) == null ? "0" : (Convert.ToDouble(LeaveCalculationSheetModelObj.UnpaidLeaves) + Convert.ToDouble(LateMarksAsAbsentDays) + Convert.ToDouble(FirstHalfAbsentDays) + Convert.ToDouble(SecondHalfAbsentDays) + Convert.ToDouble(FullDayAbsentDays)).ToString();


                    //                    //List<SP_LeaveTableData_Result> AllLeaveDetailsList = WetosEmployeeController.GetLeavedataNewLogic(WetosDB, EmployeeDataObj.EmployeeId, FromDate);
                    //                    //double AllowedLeaves = AllLeaveDetailsList.Count > 0 ? Convert.ToDouble(AllLeaveDetailsList[0].LeaveAllowed) : 0.00;
                    //                    //// MODIFIED BY MSJ ON 05 JUNE 2017 END
                    //                    if (Convert.ToDouble(SP_LeaveTableDataNewObj.LeaveAllowed) == 0)
                    //                    {
                    //                        LeaveCalculationSheetModelObj.AllowedLeaves = SP_LeaveTableDataNewObj.CurrentBalance.ToString();
                    //                    }
                    //                    else
                    //                    {
                    //                        LeaveCalculationSheetModelObj.AllowedLeaves = (SP_LeaveTableDataNewObj.LeaveAllowed + Convert.ToDecimal(SP_LeaveTableDataNewObj.TotalPaidLeaves)).ToString();
                    //                    }
                    //                }
                    //            }
                    //        }
                    //        if (!string.IsNullOrEmpty(LeaveCalculationSheetModelObj.NameOfEmployee))
                    //        {
                    //            LeaveCalculationSheetList.Add(LeaveCalculationSheetModelObj);
                    //        }
                    //    }
                    //    ViewBag.ReturnMessage = "Leave Calculation Report from " + FromDate.ToString("dd-MMM-yyyy") + " to " + ToDate.ToString("dd-MMM-yyyy");
                    //    //Code CHANGED BY SHRADDHA ON 08 FEB 2017 END
                    //    return PartialView(LeaveCalculationSheetList);
                    //}
                    #endregion



                    #region NEW LOGIC
                    if (FromDate.Month != ToDate.Month)
                    {
                        ViewBag.ReturnMessage = "From Date and To date should be of same month.";
                        return PartialView(LeaveCalculationSheetList);
                    }
                    else
                    {
                        ViewBag.FromDate = FromDate.ToString("MMM-yyyy");
                        ViewBag.ToDate = ToDate.ToString("MMM-yyyy");

                        var EmployeeDataList = WetosDB.SP_LeaveCalculationSheetNew(FromDate).ToList();

                        foreach (var EmployeeDataObj in EmployeeDataList)
                        {
                            //Code CHANGED BY SHRADDHA ON 08 FEB 2017 START    
                            LeaveCalculationSheetModel LeaveCalculationSheetModelObj = new LeaveCalculationSheetModel();
                            GetLeaveReportForSelectedMonth(EmployeeDataObj, ref LeaveCalculationSheetModelObj, FromDate, ToDate);
                            if (!string.IsNullOrEmpty(LeaveCalculationSheetModelObj.NameOfEmployee))
                            {
                                LeaveCalculationSheetList.Add(LeaveCalculationSheetModelObj);
                            }

                        }
                        return PartialView(LeaveCalculationSheetList);
                    }
                    #endregion

                }
            }
            catch (System.Exception E)
            {
                List<LeaveCalculationSheetModel> LeaveCalculationSheetList = new List<LeaveCalculationSheetModel>();
                ViewBag.ReturnMessage = "From Date and Todate should be of same month.";
                return PartialView(LeaveCalculationSheetList);
            }

        }

        /// <summary>
        /// GENERIC FUNCTION TO GET ALLOWED LEAVES ON LEAVE APPLICATION CREATE AND EDIT
        /// ADDED BY SHRADDHA ON 24 MAY 2017
        /// <param name="AllowedLeaveAsonDate"></param>
        /// <returns></returns>
        public double GetAllowedLeaves(DateTime AllowedLeaveAsonDate, int id)
        {
            // int id = Convert.ToInt32(Session["EmployeeNo"]);
            double AllowedLeaves = 0.0d;
            DateTime LeaveAsonDate = AllowedLeaveAsonDate;
            //DateTime LeaveAsonDate = new DateTime(2017, 3, 15);
            //DateTime LeaveAsonDate = new DateTime(2017, 3, 31);



            Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == id).FirstOrDefault();
            // Get FY
            WetosDB.FinancialYear FYLA = WetosDB.FinancialYears.Where(a => a.StartDate <= LeaveAsonDate && a.EndDate >= LeaveAsonDate && a.Company.CompanyId == EmployeeObj.CompanyId && a.Branch.BranchId == EmployeeObj.BranchId).FirstOrDefault();

            if (FYLA == null)
            {

            }

            double OpenCarryForward = 0.0;
            double OpenOpenBal = 0.0;
            double LeaveAllowed = 0.0;
            double LeaveBalance = 0.0;
            double TotalLeaveSanctioned = 0;

            var Opening = WetosDB.LeaveCredits.Where(a => a.EmployeeId == id && a.FinancialYearId == FYLA.FinancialYearId).FirstOrDefault();

            if (Opening != null)
            {
                EmployeeGroupDetail EmployeeGroupDetailObj = WetosDB.EmployeeGroupDetails.Where(a => a.Employee.EmployeeId == EmployeeObj.EmployeeId).FirstOrDefault();


                LeaveMaster LeaveMasterObj = WetosDB.LeaveMasters.Where(a => a.EmployeeGroup.EmployeeGroupId == EmployeeGroupDetailObj.EmployeeGroup.EmployeeGroupId
                    && a.Company.CompanyId == EmployeeObj.CompanyId && a.BranchId == EmployeeObj.BranchId
                    && a.LeaveCode.Trim().ToUpper() == Opening.LeaveType.Trim().ToUpper()).FirstOrDefault();

                // Updated by Rajas on 1 AUGUST 2017 START to handle null values
                OpenCarryForward = Opening.CarryForward == null ? 0 : Opening.CarryForward.Value;
                OpenOpenBal = Opening.OpeningBalance == null ? 0 : Opening.OpeningBalance.Value;
                // Updated by Rajas on 1 AUGUST 2017 to handle null values END

                LeaveAllowed = OpenCarryForward;
                LeaveBalance = OpenCarryForward;

                DateTime ApplicableFrom = Opening.ApplicableEffectiveDate == null ? FYLA.StartDate : Opening.ApplicableEffectiveDate.Value;

                if (LeaveMasterObj.LeaveCreditTypeId != null)
                {
                    if (LeaveMasterObj.LeaveCreditTypeId == 2)
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

                        LeaveAllowed = Convert.ToDouble(OpenCarryForward + (count * LeaveMasterObj.MaxNoOfLeavesAllowedInMonth));
                    }
                    else
                    {
                        LeaveAllowed = Convert.ToDouble(LeaveMasterObj.MaxNoOfTimesAllowedInYear);
                    }
                }
                else
                {
                    LeaveAllowed = Convert.ToDouble(LeaveMasterObj.MaxNoOfTimesAllowedInYear);
                }


                TotalLeaveSanctioned = GetAppliedDays(ApplicableFrom, LeaveAsonDate, id);
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

                // Get FY
                WetosDB.FinancialYear LFYFYLA = WetosDB.FinancialYears.Where(a => a.FinancialYearId == FYLA.PrevFYId).FirstOrDefault();

                double LFYOpenCarryForward = 0.0;
                double LFYOpenOpenBal = 0.0;
                double LFYLeaveAllowed = 0.0;
                double LFYLeaveBalance = 0.0;
                double LFYTotalLeaveSanctioned = 0;

                var LFYOpening = WetosDB.LeaveCredits.Where(a => a.EmployeeId == id && a.FinancialYearId == LFYFYLA.FinancialYearId).FirstOrDefault();

                if (LFYOpening != null)
                {
                    EmployeeGroupDetail EmployeeGroupDetailObj = WetosDB.EmployeeGroupDetails.Where(a => a.Employee.EmployeeId == EmployeeObj.EmployeeId).FirstOrDefault();

                    LeaveMaster LeaveMasterObj = WetosDB.LeaveMasters
                        .Where(a => a.EmployeeGroup.EmployeeGroupId == EmployeeGroupDetailObj.EmployeeGroup.EmployeeGroupId && a.Company.CompanyId == EmployeeObj.CompanyId
                            && a.BranchId == EmployeeObj.BranchId && a.LeaveCode.Trim().ToUpper() == LFYOpening.LeaveType.Trim().ToUpper()).FirstOrDefault();

                    // Updated by Rajas on 1 AUGUST 2017 to handle null value START
                    LFYOpenCarryForward = LFYOpening.CarryForward == null ? 0 : LFYOpening.CarryForward.Value;
                    LFYOpenOpenBal = LFYOpening.OpeningBalance == null ? 0 : LFYOpening.OpeningBalance.Value;
                    // Updated by Rajas on 1 AUGUST 2017 to handle null value END

                    LFYLeaveAllowed = LFYOpenCarryForward;
                    LFYLeaveBalance = LFYOpenCarryForward;

                    DateTime LFYApplicableFrom = LFYOpening.ApplicableEffectiveDate == null ? FYLA.StartDate : LFYOpening.ApplicableEffectiveDate.Value;

                    //if (LFYMonthlyCreditFlag)
                    //{
                    //    int LFYcount = 0;
                    //    for (int i = 1; ; i++)
                    //    {
                    //        if (LFYApplicableFrom.AddMonths(i) <= FYLA.EndDate)
                    //        {
                    //            LFYcount++;
                    //        }
                    //        else
                    //        {
                    //            break;
                    //        }
                    //    }

                    //    LFYLeaveAllowed =Convert.ToDouble( LFYOpenCarryForward + (LFYcount * LeaveMasterObj.MaxNoOfLeavesAllowedInMonth));
                    //}

                    if (LeaveMasterObj.LeaveCreditTypeId != null)
                    {
                        if (LeaveMasterObj.LeaveCreditTypeId == 2)
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

                            LFYLeaveAllowed = Convert.ToDouble(LFYOpenCarryForward + (LFYcount * LeaveMasterObj.MaxNoOfLeavesAllowedInMonth));
                        }

                        else
                        {
                            LFYLeaveAllowed = Convert.ToDouble(LeaveMasterObj.MaxNoOfTimesAllowedInYear);
                        }
                    }
                    else
                    {
                        LFYLeaveAllowed = Convert.ToDouble(LeaveMasterObj.MaxNoOfTimesAllowedInYear);
                    }


                    LFYTotalLeaveSanctioned = GetAppliedDays(LFYApplicableFrom, LFYFYLA.EndDate, id);
                    LFYLeaveBalance = LFYLeaveAllowed - LFYTotalLeaveSanctioned;

                    // ASSIGNED VALUE
                    Opening.CarryForward = LFYLeaveBalance;
                    Opening.CompanyId = LFYFYLA.Company.CompanyId;
                    Opening.BranchId = LFYFYLA.Branch.BranchId;
                    Opening.OpeningBalance = LFYLeaveBalance + LeaveMasterObj.MaxNoOfTimesAllowedInYear; //
                    //ADDED BY SHRADDHA ON 10 MAY 2017 START
                    Opening.LeaveType = LFYOpening.LeaveType;
                    Opening.ApplicableEffectiveDate = LFYApplicableFrom;
                    //ADDED BY SHRADDHA ON 10 MAY 2017 END
                    WetosDB.LeaveCredits.Add(Opening); // NEW CREDIT ENTRY FY
                    WetosDB.SaveChanges();

                    // Update leave balance
                    LeaveBalance LeaveBalances = WetosDB.LeaveBalances.Where(a => a.EmployeeId == id).FirstOrDefault();
                    LeaveBalances.PreviousBalance = 0;//LeaveBalances.CurrentBalance;
                    LeaveBalances.CurrentBalance = LFYLeaveBalance + LeaveMasterObj.MaxNoOfTimesAllowedInYear;
                    WetosDB.SaveChanges();

                    // get data fro new fy start
                    // Updated by Rajas on 1 AUGUST 2017 START to handle null value
                    OpenCarryForward = Opening.CarryForward == null ? 0 : Opening.CarryForward.Value;
                    OpenOpenBal = Opening.OpeningBalance == null ? 0 : Opening.OpeningBalance.Value;
                    // Updated by Rajas on 1 AUGUST 2017 END to handle null value

                    LeaveAllowed = OpenCarryForward;
                    LeaveBalance = OpenCarryForward;

                    DateTime ApplicableFrom = Opening.ApplicableEffectiveDate == null ? FYLA.StartDate : Opening.ApplicableEffectiveDate.Value;

                    //if (MonthlyCreditFlag)
                    //{
                    //    int count = 0;
                    //    for (int i = 1; ; i++)
                    //    {
                    //        if (ApplicableFrom.AddMonths(i) <= LeaveAsonDate)
                    //        {
                    //            count++;
                    //        }
                    //        else
                    //        {
                    //            break;
                    //        }
                    //    }

                    //    LeaveAllowed = OpenCarryForward + (count * 1.75);
                    //}

                    if (LeaveMasterObj.LeaveCreditTypeId != null)
                    {
                        if (LeaveMasterObj.LeaveCreditTypeId == 2)
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

                            LeaveAllowed = Convert.ToDouble(OpenCarryForward + (count * LeaveMasterObj.MaxNoOfLeavesAllowedInMonth));
                        }
                        else
                        {
                            LeaveAllowed = Convert.ToDouble(LeaveMasterObj.MaxNoOfTimesAllowedInYear);
                        }
                    }
                    else
                    {
                        LeaveAllowed = Convert.ToDouble(LeaveMasterObj.MaxNoOfTimesAllowedInYear);
                    }
                    TotalLeaveSanctioned = GetAppliedDays(ApplicableFrom, LeaveAsonDate, id);
                    LeaveBalance = LeaveAllowed - TotalLeaveSanctioned;
                    // get data fro new fy END
                }
                AllowedLeaves = LeaveBalance;
            }
            return AllowedLeaves;
        }

        /// <summary>
        /// ActionResult code added BY SHRADDHA ON 25 MAY 2017 from  madhav sir's code
        /// </summary>
        /// <returns></returns>
        public double GetAppliedDays(DateTime FromDate, DateTime ToDate, int EmpId)
        {
            List<LeaveApplication> LeaveAppList = WetosDB.LeaveApplications
                .Where(a => a.FromDate >= FromDate && a.FromDate <= ToDate && a.StatusId == 2 && a.EmployeeId == EmpId).ToList();

            double TotalAppliedDays = 0.0;
            foreach (LeaveApplication LeaveTypeObj in LeaveAppList)
            {

                TotalAppliedDays = TotalAppliedDays + LeaveTypeObj.ActualDays.Value;
            }

            return TotalAppliedDays;
        }

        /// <summary>
        /// CODE FOR EXPORT LEAVE CALCULATION REPORT
        /// ADDED BY SHRADDHA ON 19 APR 2017
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <returns></returns>
        public ActionResult ExportData(DateTime FromDate, DateTime ToDate)
        {
            System.Web.UI.WebControls.GridView gv = new System.Web.UI.WebControls.GridView();

            #region LeaveCalculationReport Export
            List<LeaveCalculationSheetModel> LeaveCalculationSheetList = new List<LeaveCalculationSheetModel>();

            //ADDED NEW CODE BY SHRADDHA ON 25 JULY 2017
            //TAKEN NEW FUNCTION GetLeaveReportForSelectedMonth TO GET LEAVE REPORT EXPORT TO EXCEL FOR SELECTED MONTH

            var EmployeeDataList = WetosDB.SP_LeaveCalculationSheetNew(FromDate).ToList();

            foreach (var EmployeeDataObj in EmployeeDataList)
            {
                //Code CHANGED BY SHRADDHA ON 08 FEB 2017 START    
                LeaveCalculationSheetModel LeaveCalculationSheetModelObj = new LeaveCalculationSheetModel();
                GetLeaveReportForSelectedMonth(EmployeeDataObj, ref LeaveCalculationSheetModelObj, FromDate, ToDate);
                if (!string.IsNullOrEmpty(LeaveCalculationSheetModelObj.NameOfEmployee))
                {
                    LeaveCalculationSheetList.Add(LeaveCalculationSheetModelObj);
                }

            }
            #endregion

            gv.DataSource = LeaveCalculationSheetList;

            gv.DataBind();
            //gv.Columns[0].Visible = false;
            //gv.Columns[3].Visible = false;

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=LeavCalculationReport.xls");
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

        /// <summary>
        /// CODE ADDED BY SHRADDHA ON 08 FEB 2017 
        /// </summary>
        /// <returns></returns>
        public ActionResult LeaveSheet()
        {

            return View();
        }

        /// <summary>
        /// Json return for to get Department dropdown list on basis of branch selection
        /// Added by Rajas on 27 DEC 2016
        /// Commented by Rajas on 30 JAN 2017
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
            var DepartmentList = WetosDB.Departments.Where(a => a.Branch.BranchId == SelBranchId && (a.MarkedAsDelete == null || a.MarkedAsDelete == 0)).Select(a => new { DepartmentId = a.DepartmentId, DepartmentName = a.DepartmentCode + " - " + a.DepartmentName }).ToList();

            // Updated by Rajas on 8 FEB 2017 for selection default select value
            //DepartmentList.Insert(0, new Department { DepartmentId = 0, DepartmentName = "ALL" });

            return Json(DepartmentList);
        }

        /// <summary>
        /// Monthly Reports Index 2
        /// </summary>
        /// <returns></returns>
        public ActionResult MonthlyReportsIndex2()
        {
            return View();
        }

        /// <summary>
        /// MIS
        /// </summary>
        /// <returns></returns>
        public ActionResult MIS()
        {
            try
            {
                // ---------------------------------------REPORT RDLC MonthlyWorkingHoursReport START-----------------------------------------
                // PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                SqlCommand com1 = new SqlCommand("usp_Report_GetMonthlyWorkingHours", con);

                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018

                com1.CommandType = CommandType.StoredProcedure;
                com1.Parameters.Add(new SqlParameter("@Fdate", SqlDbType.DateTime)).Value = new DateTime(2017, 06, 01);
                com1.Parameters.Add(new SqlParameter("@Tdate", SqlDbType.DateTime)).Value = new DateTime(2017, 06, 17); //new DateTime(2013, 01, 31);

                // Updated by Rajas on 13 Jan 2017
                string EmployeeString = "";

                EmployeeString = (String.Join(",", WetosDB.Employees.Where(a => a.ActiveFlag == null || a.ActiveFlag == true).Select(a => a.EmployeeId).ToArray()));


                com1.Parameters.Add(new SqlParameter("@EmpidList", SqlDbType.NVarChar)).Value = EmployeeString;

                //com1.Parameters.Add(new SqlParameter("@BranchListId", SqlDbType.NVarChar, 500)).Value = "1,3,7";
                //com1.Parameters.Add(new SqlParameter("@CompanyListId", SqlDbType.NVarChar, 500)).Value = "2";
                //com1.Parameters.Add(new SqlParameter("@RoleTypeId", SqlDbType.NVarChar, 500)).Value = "4";
                //com1.Parameters.Add(new SqlParameter("@ReportingId", SqlDbType.Int)).Value = "76";
                SqlDataAdapter da1 = new SqlDataAdapter(com1);

                da1.Fill(table5);
                //

                System.Data.DataColumn newColumn = new System.Data.DataColumn("TotalWHrs", typeof(System.String));
                newColumn.DefaultValue = "0.00";
                table5.Columns.Add(newColumn);

                //
                //int TotalMin = 0;
                foreach (DataRow dr in table5.Rows)
                {
                    int TotalMin = 0;

                    foreach (DataColumn dc in table5.Columns)
                    {
                        string DailyHrs = dr[dc].ToString();
                        if (DailyHrs.Length > 2 && DailyHrs.Length < 6)
                        {
                            int Hrs = 0;
                            int Min = 0;

                            string[] HrsMin = DailyHrs.Split(':');

                            if (HrsMin.Count() == 2)
                            {
                                Hrs = Convert.ToInt32(HrsMin[0]);
                                Min = Convert.ToInt32(HrsMin[1]);
                            }

                            TotalMin = TotalMin + ((Hrs * 60) + Min);
                        }
                    }

                    int CalcHrs = TotalMin / 60;
                    int CalcMin = TotalMin - (CalcHrs * 60);

                    dr["TotalWHrs"] = CalcHrs.ToString("#00") + ":" + CalcMin.ToString("#00");
                }

            }
            catch (System.Exception)
            {
                //throw;
            }
            return View();
        }

        /// <summary>
        /// Ajax Method
        /// </summary>
        /// <param name="MonthlyWorkingHoursReport"></param>
        /// <returns></returns>
        public ContentResult AjaxMethod(string MonthlyWorkingHoursReport)
        {
            string query = "SELECT top 10 employeeid,WorkingHrs FROM DailyTransaction ORDER BY WorkingHrs Desc";

            string constr = ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString;
            StringBuilder sb = new StringBuilder();
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    //cmd.Parameters.AddWithValue("@DailyTransaction", MonthlyWorkingHoursReport);
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        sb.Append("[");
                        while (sdr.Read())
                        {
                            sb.Append("{");
                            System.Threading.Thread.Sleep(50);
                            string color = String.Format("#{0:X6}", new Random().Next(0x1000000));
                            sb.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", sdr[1].ToString(), sdr[00].ToString(), color));
                            //     sb.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", "ABC","20", color));

                            sb.Append("},");
                        }
                        // sb = sb.Remove(sb.Length - 1, 1);
                        sb.Append("]");
                    }

                    con.Close();
                }
            }

            return Content(sb.ToString());
        }

        //public JsonResult AjaxMethod2(string MonthlyWorkingHoursReport)
        //{
        //    var result = WetosDB.Companies.Select(a=>a.CompanyName).FirstOrDefault();
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        // GENERATE PDF START

        #region GENERATE PDF

        /// CODE IN BELOW REGION UPDATED BY RAJAS ON 4 FEB 2017
        /// "a => a.ActiveFlag == null && a.Leavingdate == null" REPLACED WITH "a => a.ActiveFlag == null || a.ActiveFlag == true"
        /// <summary>
        /// DailyAttedenceReport
        /// </summary>
        /// <returns></returns>
        private string DailyAttedenceReport(DailyReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 8 FEB 2017
        {
            //bool ReturnStatus = false;
            string PdfFileName = "";

            try
            {
                // ---------------------------------------REPORT RDLC DailyAttendance START-----------------------------------------
                // PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                SqlCommand com1 = new SqlCommand("usp_Rpt_GetTimeCard", con);

                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018

                com1.CommandType = CommandType.StoredProcedure;
                com1.Parameters.Add(new SqlParameter("@Fdate", SqlDbType.DateTime)).Value = ReportModel.FromDate; //new DateTime(2013, 01, 01);
                com1.Parameters.Add(new SqlParameter("@Tdate", SqlDbType.DateTime)).Value = ReportModel.FromDate; //new DateTime(2013, 01, 01);
                com1.Parameters.Add(new SqlParameter("@RoleTypeId", SqlDbType.NVarChar, 500)).Value = "1";
                com1.Parameters.Add(new SqlParameter("@ReportingId", SqlDbType.Int)).Value = DBNull.Value;
                com1.Parameters.Add(new SqlParameter("@Empid", SqlDbType.NVarChar)).Value = ReportModel.EmployeeString;

                // Branch selection
                if (ReportModel.BranchId != 0)
                {
                    com1.Parameters.Add(new SqlParameter("@BranchListId", SqlDbType.NVarChar, 500)).Value = ReportModel.BranchId;
                }
                else
                {
                    com1.Parameters.Add(new SqlParameter("@BranchListId", SqlDbType.NVarChar, 500)).Value = DBNull.Value;
                }

                // company selection
                if (ReportModel.CompanyId != 0)
                {
                    com1.Parameters.Add(new SqlParameter("@CompanyListId", SqlDbType.NVarChar, 500)).Value = ReportModel.CompanyId;
                }
                else
                {
                    com1.Parameters.Add(new SqlParameter("@CompanyListId", SqlDbType.NVarChar, 500)).Value = DBNull.Value;
                }

                SqlDataAdapter da1 = new SqlDataAdapter(com1);

                da1.Fill(table5);

                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate,
                    table5, "~/Reports/DailyAttendance.rdlc",
                    "DailyAttendance", "~/User_Data/download/", "Daily_Attedence_Report_", ref PdfFileName);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");
                //Response.AddHeader("Content-Disposition", "attachment; filename=\"" + GeneratedFileName + "\"");
                //ReturnStatus = true;

            }
            catch (System.Exception)
            {
            }

            return PdfFileName;

            // PRINT END
            //---------------------------------------------REPORT RDLC DailyAttendance END-------------------------------------------
        }

        /// <summary>
        /// CDR
        /// </summary>
        /// <returns></returns>
        private string CDR(DailyReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 8 FEB 2017
        {
            //bool ReturnStatus = false;
            string PdfFileName = "";

            try
            {
                // ---------------------------------------REPORT RDLC DailyAttendance START-----------------------------------------
                // PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                SqlCommand com1 = new SqlCommand("usp_Rpt_GetTimeCard", con);

                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018

                // ADDED BY MSJ ON 21 FEB 2020 START
                //DateTime CurrentDate = DateTime.Now.Date;
                string timezoneId = System.Configuration.ConfigurationManager.AppSettings["TimeZoneId"];
                DateTime CurrentDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timezoneId)).Date;

                // ADDED BY MSJ ON 27 FEB 2020 START
                ReportModel.FromDate = CurrentDate.ToString("dd/MM/yyyy");
                ReportModel.ToDate = ReportModel.FromDate;

                com1.CommandType = CommandType.StoredProcedure;
                com1.Parameters.Add(new SqlParameter("@Fdate", SqlDbType.DateTime)).Value = CurrentDate; // ReportModel.FromDate; 
                com1.Parameters.Add(new SqlParameter("@Tdate", SqlDbType.DateTime)).Value = CurrentDate; // ReportModel.FromDate;

                com1.Parameters.Add(new SqlParameter("@RoleTypeId", SqlDbType.NVarChar, 500)).Value = "1";
                com1.Parameters.Add(new SqlParameter("@ReportingId", SqlDbType.Int)).Value = DBNull.Value;
                com1.Parameters.Add(new SqlParameter("@Empid", SqlDbType.NVarChar)).Value = ReportModel.EmployeeString;

                // Branch selection
                if (ReportModel.BranchId != 0)
                {
                    com1.Parameters.Add(new SqlParameter("@BranchListId", SqlDbType.NVarChar, 500)).Value = ReportModel.BranchId;
                }
                else
                {
                    com1.Parameters.Add(new SqlParameter("@BranchListId", SqlDbType.NVarChar, 500)).Value = DBNull.Value;
                }

                // company selection
                if (ReportModel.CompanyId != 0)
                {
                    com1.Parameters.Add(new SqlParameter("@CompanyListId", SqlDbType.NVarChar, 500)).Value = ReportModel.CompanyId;
                }
                else
                {
                    com1.Parameters.Add(new SqlParameter("@CompanyListId", SqlDbType.NVarChar, 500)).Value = DBNull.Value;
                }

                SqlDataAdapter da1 = new SqlDataAdapter(com1);

                da1.Fill(table5);

                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate,
                    table5, "~/Reports/DailyAttendance.rdlc",
                    "DailyAttendance", "~/User_Data/download/", "Daily_Attedence_Report_", ref PdfFileName);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

                //ReturnStatus = true;

            }
            catch (System.Exception)
            {
            }

            return PdfFileName;

            // PRINT END
            //---------------------------------------------REPORT RDLC DailyAttendance END-------------------------------------------
        }

        /// <summary>
        /// PDR
        /// </summary>
        /// <returns></returns>
        private string PDR(DailyReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 8 FEB 2017
        {
            //bool ReturnStatus = false;
            string PdfFileName = "";

            try
            {
                // ---------------------------------------REPORT RDLC DailyAttendance START-----------------------------------------
                // PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                SqlCommand com1 = new SqlCommand("usp_Rpt_GetTimeCard", con);

                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018

                // ADDED BY MSJ ON 21 FEB 2020 START
                string timezoneId = System.Configuration.ConfigurationManager.AppSettings["TimeZoneId"];
                DateTime CurrentDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timezoneId)).Date;
                //DateTime CurrentDate = DateTime.Now.Date;
                DateTime PrevDay = CurrentDate.AddDays(-1);

                // ADDED BY MSJ ON 27 FEB 2020 START
                ReportModel.FromDate = PrevDay.ToString("dd/MM/yyyy");
                ReportModel.ToDate = ReportModel.FromDate;

                com1.CommandType = CommandType.StoredProcedure;
                com1.Parameters.Add(new SqlParameter("@Fdate", SqlDbType.DateTime)).Value = PrevDay; // ReportModel.FromDate; //new DateTime(2013, 01, 01);
                com1.Parameters.Add(new SqlParameter("@Tdate", SqlDbType.DateTime)).Value = PrevDay; // ReportModel.FromDate; //new DateTime(2013, 01, 01);
                com1.Parameters.Add(new SqlParameter("@RoleTypeId", SqlDbType.NVarChar, 500)).Value = "1";
                com1.Parameters.Add(new SqlParameter("@ReportingId", SqlDbType.Int)).Value = DBNull.Value;
                com1.Parameters.Add(new SqlParameter("@Empid", SqlDbType.NVarChar)).Value = ReportModel.EmployeeString;

                // Branch selection
                if (ReportModel.BranchId != 0)
                {
                    com1.Parameters.Add(new SqlParameter("@BranchListId", SqlDbType.NVarChar, 500)).Value = ReportModel.BranchId;
                }
                else
                {
                    com1.Parameters.Add(new SqlParameter("@BranchListId", SqlDbType.NVarChar, 500)).Value = DBNull.Value;
                }

                // company selection
                if (ReportModel.CompanyId != 0)
                {
                    com1.Parameters.Add(new SqlParameter("@CompanyListId", SqlDbType.NVarChar, 500)).Value = ReportModel.CompanyId;
                }
                else
                {
                    com1.Parameters.Add(new SqlParameter("@CompanyListId", SqlDbType.NVarChar, 500)).Value = DBNull.Value;
                }

                SqlDataAdapter da1 = new SqlDataAdapter(com1);

                da1.Fill(table5);

                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate,
                    table5, "~/Reports/DailyAttendance.rdlc",
                    "DailyAttendance", "~/User_Data/download/", "Daily_Attedence_Report_", ref PdfFileName);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

                //ReturnStatus = true;

            }
            catch (System.Exception)
            {
            }

            return PdfFileName;

            // PRINT END
            //---------------------------------------------REPORT RDLC DailyAttendance END-------------------------------------------
        }

        /// <summary>
        /// LeaveBalanceReport
        /// </summary>
        /// <returns></returns>
        /// UPDATED BY RAJAS ON 22 MAY 2017
        private string LeaveBalanceReport(MonthlyReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 8 FEB 2017
        {
            // bool ReturnStatus = false;
            string PdfFileName = "";

            try
            {
                // ---------------------------------------REPORT RDLC LeaveBalanceReport START-----------------------------------------
                // PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                //SqlCommand com1 = new SqlCommand("usp_RPT_GetAllLeaveBalanceMonthlyMuster", con);
                SqlCommand com1 = new SqlCommand("usp_RPT_GetAllLeaveBalanceMonthlyMuster", con);
                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018
                com1.CommandType = CommandType.StoredProcedure;
                com1.Parameters.Add(new SqlParameter("@EmpidList", SqlDbType.NVarChar)).Value = ReportModel.EmployeeString;

                // Get current FY from global setting
                //GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == "Current Financial Year").FirstOrDefault();

                //ABOVE LINE COMMENTED AND BELOW LINE IS ADDED BY SHALAKA ON 13TH DEC 2017
                //GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.CurrentFinancialYear).FirstOrDefault();

                //if (GlobalSettingObj != null)
                //{
                //    com1.Parameters.Add(new SqlParameter("@FinancialName", SqlDbType.NVarChar)).Value = GlobalSettingObj.SettingValue;  // Added by Rajas on 6 FEB 2017 as additional parameter}
                //}
                //else // Handle null object
                //{
                //    com1.Parameters.Add(new SqlParameter("@FinancialName", SqlDbType.NVarChar)).Value = string.Empty;
                //}

                SqlDataAdapter da1 = new SqlDataAdapter(com1);

                da1.Fill(table5);

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate,
                    table5, "~/Reports/LeaveBalanceReport.rdlc",
                    "LeaveBalance", "~/User_Data/download/", "Leave_Balance_Report_", ref PdfFileName);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");


                //ReturnStatus = true;

            }
            catch (System.Exception)
            {

            }

            return PdfFileName;

            // PRINT END
            ////---------------------------------------------REPORT RDLC LeaveBalanceReport END-------------------------------------------

        }

        /// <summary>
        /// MonthlyShiftChartReport
        /// </summary>
        /// <returns></returns>
        private bool MonthlyShiftChartReport(MasterReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 8 FEB 2017
        {
            bool ReturnStatus = false;
            string PdfFileName = "";

            try
            {
                // // ---------------------------------------REPORT RDLC MonthlyShiftChartReport START-----------------------------------------
                // // PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                SqlCommand com1 = new SqlCommand("usp_Report_GetMonthlyShiftChart", con);

                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018

                com1.CommandType = CommandType.StoredProcedure;
                com1.Parameters.Add(new SqlParameter("@Month", SqlDbType.Int)).Value = "8";
                com1.Parameters.Add(new SqlParameter("@Year", SqlDbType.Int)).Value = "2013";

                // Added By Rajas on 22 DEC 2016 for EmployeeId list selected branchwise START
                //List<WetosDB.Employee> EmployeeList = WetosDB.Employees.ToList();
                string EmployeeString = "";
                EmployeeString = (String.Join(",", WetosDB.Employees.Where(a => a.BranchId == ReportModel.BranchId).Select(a => a.EmployeeId).ToArray()));
                // Added By Rajas on 22 DEC 2016 for EmployeeId list selected branchwise END

                com1.Parameters.Add(new SqlParameter("@EmpidList", SqlDbType.NVarChar)).Value = EmployeeString; //DBNull.Value;
                SqlDataAdapter da1 = new SqlDataAdapter(com1);

                da1.Fill(table5);

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate,
                    table5, "~/Reports/MonthlyShiftChartReport.rdlc",
                    "MonthlyShiftChart", "~/User_Data/download/", "Monthly_Shift_Chart_Report_", ref PdfFileName);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

                //ReturnStatus = true;

            }
            catch (System.Exception)
            {
                //throw;
            }

            return ReturnStatus;


            // PRINT END
            //---------------------------------------------REPORT RDLC MonthlyShiftChartReport END-------------------------------------------
        }

        /// <summary>
        /// EmployeeMasterReport
        /// Commented during merge 30 SEP 2016
        /// </summary>
        /// <returns></returns>
        ////private bool EmployeeMasterReport()
        ////{
        ////    bool ReturnStatus = false;

        ////    try
        ////    {
        ////        // ---------------------------------------REPORT RDLC - EmployeeMasterReport START-----------------------------------------
        ////        // PRINT START
        ////        DataTable table5 = new DataTable();
        ////        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
        ////        //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

        ////        SqlCommand com1 = new SqlCommand("usp_Report_EmployeeDetails", con);
        ////        com1.CommandType = CommandType.StoredProcedure;
        ////        //com1.Parameters.Add(new SqlParameter("@InputChalId", SqlDbType.Int)).Value = ordno;
        ////        SqlDataAdapter da1 = new SqlDataAdapter(com1);

        ////        da1.Fill(table5);


        ////        Warning[] warnings;
        ////        string[] streamids;
        ////        string mimeType;
        ////        string encoding;
        ////        string filenameExtension;
        ////        string filePath;

        ////        LocalReport localReport = new LocalReport();
        ////        localReport.ReportPath = Server.MapPath("~/Reports/EmployeeMasterReport.rdlc");
        ////        //ReportDataSource reportDataSource = new ReportDataSource("Customers", Customers.GetAllCustomers());
        ////        localReport.DataSources.Clear();
        ////        localReport.DataSources.Add(new ReportDataSource("WetosDataSet", table5));

        ////        localReport.Refresh();
        ////        string reportType = "PDF";
        ////        //string mimeType;
        ////        //string encoding;
        ////        string fileNameExtension;

        ////        ReportParameter criteria1 = new ReportParameter("Title", "FIRST RDLC REPORT");
        ////        localReport.SetParameters(new ReportParameter[] { criteria1 });
        ////        localReport.EnableHyperlinks = true;

        ////        //The DeviceInfo settings should be changed based on the reportType
        ////        //http://msdn2.microsoft.com/en-us/library/ms155397.aspx
        ////        string deviceInfo =

        ////        "<DeviceInfo>" +
        ////        "  <OutputFormat>PDF</OutputFormat>" +
        ////        "</DeviceInfo>";

        ////        //Warning[] warnings;
        ////        string[] streams;
        ////        byte[] renderedBytes;

        ////        //Render the report

        ////        renderedBytes = localReport.Render(
        ////            reportType,
        ////            deviceInfo,
        ////            out mimeType,
        ////            out encoding,
        ////            out fileNameExtension,
        ////            out streams,
        ////            out warnings);

        ////        //FileStream fs = new FileStream(filePath, FileMode.Create);
        ////        using (FileStream fs = new FileStream(Server.MapPath("~/User_Data/download/") + "Employee Master Report.pdf", FileMode.Create))
        ////        {
        ////            fs.Write(renderedBytes, 0, renderedBytes.Length);
        ////        }

        ////        Response.AddHeader("Content-Disposition", "attachment; filename=" + "abcd" + ".pdf;");
        ////        string PdfUrl = "download/" + "abcd" + ".pdf";




        ////        ReturnStatus = true;

        ////    }
        ////    catch (System.Exception)
        ////    {
        ////        //throw;
        ////    }

        ////    return ReturnStatus;


        ////    // PRINT END
        ////    //---------------------------------------------REPORT RDLC EmployeeMasterReport END-------------------------------------------
        ////}

        /// <summary>
        /// TimeCardReport
        /// </summary>
        /// <returns></returns>
        /// Updated by Rajas on 6 SEP 2017
        private string TimeCardReport(MonthlyReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 7 FEB 2017
        {
            //bool ReturnStatus = false;
            string PdfFileName = "";

            try
            {

                // ------------------------------------------------REPORT RDLC TimeCardReport START-------------------------------------------
                //// PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                SqlCommand com1 = new SqlCommand("usp_Rpt_GetTimeCard", con);

                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018

                com1.CommandType = CommandType.StoredProcedure;
                com1.Parameters.Add(new SqlParameter("@Fdate", SqlDbType.DateTime)).Value = ReportModel.FromDate; //new DateTime(2013, 01, 01);
                com1.Parameters.Add(new SqlParameter("@Tdate", SqlDbType.DateTime)).Value = ReportModel.ToDate; //new DateTime(2013, 01, 01);
                com1.Parameters.Add(new SqlParameter("@RoleTypeId", SqlDbType.NVarChar, 500)).Value = "1";
                com1.Parameters.Add(new SqlParameter("@ReportingId", SqlDbType.Int)).Value = DBNull.Value;
                com1.Parameters.Add(new SqlParameter("@Empid", SqlDbType.NVarChar)).Value = ReportModel.EmployeeString;

                // Branch selection
                if (ReportModel.BranchId != 0)
                {
                    com1.Parameters.Add(new SqlParameter("@BranchListId", SqlDbType.NVarChar, 500)).Value = ReportModel.BranchId;
                }
                else
                {
                    com1.Parameters.Add(new SqlParameter("@BranchListId", SqlDbType.NVarChar, 500)).Value = DBNull.Value;
                }

                // company selection
                if (ReportModel.CompanyId != 0)
                {
                    com1.Parameters.Add(new SqlParameter("@CompanyListId", SqlDbType.NVarChar, 500)).Value = ReportModel.CompanyId;
                }
                else
                {
                    com1.Parameters.Add(new SqlParameter("@CompanyListId", SqlDbType.NVarChar, 500)).Value = DBNull.Value;
                }

                SqlDataAdapter da1 = new SqlDataAdapter(com1);

                da1.Fill(table5);

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/TimeCardReportOLD.rdlc",
                    "TimeCardDataset", "~/User_Data/download/", "Time_Card_Report_", ref PdfFileName);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

                //ReturnStatus = true;

            }
            catch (System.Exception ex)
            {
                AddAuditTrail("TimeCardReport " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));
            }

            return PdfFileName;


            // PRINT END
            //---------------------------------------------REPORT RDLC TimeCardReport END-------------------------------------------
        }

        /// <summary>
        /// SwipeMovementReport
        /// </summary>
        /// <returns></returns>
        private string SwipeMovementReport(DailyReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 8 FEB 2017
        {
            //bool ReturnStatus = false;
            string PdfFileName = "";

            try
            {
                // ------------------------------------------------REPORT RDLC SwipeMovementReport START-------------------------------------------
                //// PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                SqlCommand com1 = new SqlCommand("usp_Report_SwipeMovement", con);

                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018

                com1.CommandType = CommandType.StoredProcedure;
                com1.Parameters.Add(new SqlParameter("@date", SqlDbType.DateTime)).Value = ReportModel.FromDate; //new DateTime(2011, 05, 01);
                com1.Parameters.Add(new SqlParameter("@empidList", SqlDbType.VarChar)).Value = ReportModel.EmployeeString;
                SqlDataAdapter da1 = new SqlDataAdapter(com1);
                da1.Fill(table5);

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/SwipeMovementReport.rdlc",
                    "SwipeMove", "~/User_Data/download/", "Swipe_Movement_Report_", ref PdfFileName);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

                //ReturnStatus = true;

            }
            catch (System.Exception)
            {
                //throw;
            }

            return PdfFileName;


            // PRINT END
            //---------------------------------------------REPORT RDLC SwipeMovementReport END-------------------------------------------

        }

        /// <summary>
        /// AuditLogReport
        /// </summary>
        /// <returns></returns>
        private string AuditLogReport(DailyReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 8 FEB 2017
        {
            //bool ReturnStatus = false;
            string PdfFileName = "";

            try
            {

                //// ------------------------------------------------REPORT RDLC AuditLogReport START-------------------------------------------
                ////// PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                SqlCommand com1 = new SqlCommand("usp_Report_AuditLogDetails", con);

                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018

                com1.CommandType = CommandType.StoredProcedure;

                // Branch selection
                if (ReportModel.BranchId != 0)
                {
                    com1.Parameters.Add(new SqlParameter("@PBranchListId", SqlDbType.NVarChar, 500)).Value = ReportModel.BranchId;
                }
                else
                {
                    com1.Parameters.Add(new SqlParameter("@PBranchListId", SqlDbType.NVarChar, 500)).Value = DBNull.Value;
                }

                // company selection
                if (ReportModel.CompanyId != 0)
                {
                    com1.Parameters.Add(new SqlParameter("@PCompanyListId", SqlDbType.NVarChar, 500)).Value = ReportModel.CompanyId;
                }
                else
                {
                    com1.Parameters.Add(new SqlParameter("@PCompanyListId", SqlDbType.NVarChar, 500)).Value = DBNull.Value;
                }

                // department selection
                if (ReportModel.DepartmentId != 0)
                {
                    com1.Parameters.Add(new SqlParameter("@PDepartmentListId", SqlDbType.NVarChar, 500)).Value = ReportModel.DepartmentId;
                }
                else
                {
                    com1.Parameters.Add(new SqlParameter("@PDepartmentListId", SqlDbType.NVarChar, 500)).Value = DBNull.Value;
                }

                com1.Parameters.Add(new SqlParameter("@PEmpId", SqlDbType.NVarChar)).Value = ReportModel.EmployeeString; //DBNull.Value;


                com1.Parameters.Add(new SqlParameter("@PFdate", SqlDbType.DateTime)).Value = ReportModel.FromDate; //new DateTime(2011, 05, 01);

                // Updated by Rajas on 22 MARCH 2017
                com1.Parameters.Add(new SqlParameter("@PTdate", SqlDbType.DateTime)).Value = ReportModel.FromDate; //new DateTime(2014, 09, 15);

                com1.Parameters.Add(new SqlParameter("@PRoleTypeId", SqlDbType.NVarChar, 500)).Value = "1";//DBNull.Value; //"4";
                com1.Parameters.Add(new SqlParameter("@PReportingId", SqlDbType.Int)).Value = DBNull.Value; //"76";
                SqlDataAdapter da1 = new SqlDataAdapter(com1);

                // Added by Rajas on 21 DEC 2016 for connection time out
                com1.CommandTimeout = 3000;

                da1.Fill(table5);

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/AuditLogReport.rdlc",
                    "AuditLog", "~/User_Data/download/", "Audit_Log_Report_", ref PdfFileName);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

                //ReturnStatus = true;

            }
            catch (System.Exception)
            {
                //throw;
            }

            return PdfFileName;


            // PRINT END
            //---------------------------------------------REPORT RDLC AuditLogReport END-------------------------------------------

        }
        #region
        //ADDED BY NISHIGANDHA ON 25 NOV 2019 START 
        private string DailyAttendanceLogReport(DailyReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 8 FEB 2017
        {
            //bool ReturnStatus = false;
            string PdfFileName = "";

            try
            {

                //// ------------------------------------------------REPORT RDLC AuditLogReport START-------------------------------------------
                ////// PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                SqlCommand com1 = new SqlCommand("SP_GetAttendanceData", con);

                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018

                com1.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da1 = new SqlDataAdapter(com1);

                com1.CommandTimeout = 3000;


                com1.Parameters.Add(new SqlParameter("@FromDate", SqlDbType.DateTime)).Value = ReportModel.FromDate; //new DateTime(2014, 09, 15);
                com1.Parameters.Add(new SqlParameter("@EmpidList", SqlDbType.NVarChar)).Value = ReportModel.EmployeeString;
                da1.Fill(table5);


                //string ABCD = string.Empty;

                //table5.TableName = "Sheet1";


                //using (XLWorkbook Workbook = new XLWorkbook()) //ExcelOutFilePath))
                //{
                //    Workbook.Worksheets.Add(table5);

                //    Response.Clear();
                //    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                //    // ADDED BY MSJ ON 16 JUNE 2017 START
                //    ABCD = "attachment;filename=\"" + "HelloWorld.xlsx" + "\"";

                //    Response.AddHeader("content-disposition", ABCD);
                //    //Response.AddHeader("content-disposition", "attachment;filename=\"HelloWorld.xlsx\"");

                //    // Flush the workbook to the Response.OutputStream
                //    using (MemoryStream memoryStream = new MemoryStream())
                //    {
                //        Workbook.SaveAs(memoryStream);
                //        memoryStream.WriteTo(Response.OutputStream);
                //        memoryStream.Close();
                //    }

                //    Response.End();
                //    // ReportPath = PdfFileName;
                //}


                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/DailyMailAttendenceReport.rdlc",
                    "DataSet1", "~/User_Data/download/", "DailyMailAttendenceReport_", ref PdfFileName);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

            }
            catch (System.Exception)
            {
                //throw;
            }

            return PdfFileName;


            // PRINT END
            //---------------------------------------------REPORT RDLC AuditLogReport END-------------------------------------------

        }
        #endregion


        #region
        //ADDED BY Snehal ON 28 NOV 2019 START 
        private string CurrentDayAttendenceReport(DailyReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 8 FEB 2017
        {
            //bool ReturnStatus = false;
            string PdfFileName = "";

            try
            {

                //// ------------------------------------------------REPORT RDLC AuditLogReport START-------------------------------------------
                ////// PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                SqlCommand com1 = new SqlCommand("SP_GetAttendanceData", con);

                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018

                com1.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da1 = new SqlDataAdapter(com1);

                com1.CommandTimeout = 3000;

                DateTime CurrentDate = DateTime.Now.Date; // ADDED BY MSJ ON 21 FEB 2020 

                com1.Parameters.Add(new SqlParameter("@FromDate", SqlDbType.DateTime)).Value = CurrentDate; // ReportModel.FromDate; //new DateTime(2014, 09, 15);
                com1.Parameters.Add(new SqlParameter("@EmpidList", SqlDbType.NVarChar)).Value = ReportModel.EmployeeString;

                da1.Fill(table5);

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/DailyMailAttendenceReport.rdlc",
                    "DataSet1", "~/User_Data/download/", "CurrentDayAttendenceReport_", ref PdfFileName);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

            }
            catch (System.Exception)
            {
                //throw;
            }

            return PdfFileName;


            // PRINT END
            //---------------------------------------------REPORT RDLC AuditLogReport END-------------------------------------------

        }
        //ADDED BY Snehal ON 28 NOV 2019 end 
        #endregion

        #region
        //ADDED BY Snehal ON 28 NOV 2019 START 
        private string PreviousDayAttendenceReportNew(DailyReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 8 FEB 2017
        {
            //bool ReturnStatus = false;
            string PdfFileName = "";

            try
            {

                //// ------------------------------------------------REPORT RDLC AuditLogReport START-------------------------------------------
                ////// PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                SqlCommand com1 = new SqlCommand("SP_GetAttendanceData", con);

                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018

                com1.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da1 = new SqlDataAdapter(com1);

                com1.CommandTimeout = 3000;
                //DateTime PrevDay = Convert.ToDateTime(ReportModel.FromDate).AddDays(-1);

                // ADDED BY MSJ ON 21 FEB 2020 START
                DateTime CurrentDate = DateTime.Now.Date;
                DateTime PrevDay = CurrentDate.AddDays(-1);

                // ADDED BY MSJ ON 27 FEB 2020 START
                ReportModel.FromDate = PrevDay.ToString("dd/MM/yyyy");
                ReportModel.ToDate = ReportModel.FromDate;

                com1.Parameters.Add(new SqlParameter("@FromDate", SqlDbType.DateTime)).Value = ReportModel.FromDate; //new DateTime(2014, 09, 15);
                com1.Parameters.Add(new SqlParameter("@EmpidList", SqlDbType.NVarChar)).Value = ReportModel.EmployeeString;

                da1.Fill(table5);

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/DailyMailAttendenceReport.rdlc",
                    "DataSet1", "~/User_Data/download/", "PreviousDayAttendenceReport_", ref PdfFileName);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

            }
            catch (System.Exception)
            {
                //throw;
            }

            return PdfFileName;


            // PRINT END
            //---------------------------------------------REPORT RDLC AuditLogReport END-------------------------------------------

        }
        //ADDED BY Snehal ON 28 NOV 2019 end 
        #endregion


        /// <summary>
        /// CurrentDayReport
        /// </summary>
        /// <returns></returns>
        private string CurrentDayReport(DailyReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 8 FEB 2017
        {
            //bool ReturnStatus = false;
            string PdfFileName = "";

            try
            {
                // ------------------------------------------------REPORT RDLC CurrentDayReport START-------------------------------------------
                //// PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                SqlCommand com1 = new SqlCommand("usp_Rpt_EmpCDR", con);

                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018

                com1.CommandType = CommandType.StoredProcedure;

                // ADDED BY MSJ ON 21 FEB 2020 START
                DateTime CurrentDate = DateTime.Now.Date;

                // ADDED BY MSJ ON 27 FEB 2020 START
                ReportModel.FromDate = CurrentDate.ToString("dd/MM/yyyy");
                ReportModel.ToDate = ReportModel.FromDate;

                com1.Parameters.Add(new SqlParameter("@Fdate", SqlDbType.DateTime)).Value = ReportModel.FromDate; //new DateTime(2013, 01, 01);                // Updated by Rajas on 22 MARCH 2017
                com1.Parameters.Add(new SqlParameter("@Tdate", SqlDbType.DateTime)).Value = ReportModel.ToDate; // ReportModel.FromDate; //new DateTime(2013, 01, 01);

                com1.Parameters.Add(new SqlParameter("@Empid", SqlDbType.NVarChar)).Value = ReportModel.EmployeeString;
                com1.Parameters.Add(new SqlParameter("@RoleTypeId", SqlDbType.NVarChar, 500)).Value = "1";
                com1.Parameters.Add(new SqlParameter("@ReportingId", SqlDbType.Int)).Value = DBNull.Value;

                // Branch selection
                if (ReportModel.BranchId != 0)
                {
                    com1.Parameters.Add(new SqlParameter("@BranchListId", SqlDbType.NVarChar, 500)).Value = ReportModel.BranchId;
                }
                else
                {
                    com1.Parameters.Add(new SqlParameter("@BranchListId", SqlDbType.NVarChar, 500)).Value = DBNull.Value;
                }

                // company selection
                if (ReportModel.CompanyId != 0)
                {
                    com1.Parameters.Add(new SqlParameter("@CompanyListId", SqlDbType.NVarChar, 500)).Value = ReportModel.CompanyId;
                }
                else
                {
                    com1.Parameters.Add(new SqlParameter("@CompanyListId", SqlDbType.NVarChar, 500)).Value = DBNull.Value;
                }

                SqlDataAdapter da1 = new SqlDataAdapter(com1);

                da1.Fill(table5);

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/CurrentDayReport.rdlc",
                    "CurrentDay", "~/User_Data/download/", "Current_Day_Report_", ref PdfFileName);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

                //ReturnStatus = true;

            }
            catch (System.Exception)
            {
                //throw;
            }

            return PdfFileName;


            // PRINT END
            //---------------------------------------------REPORT RDLC CurrentDayReport END-------------------------------------------
        }

        /// <summary>
        /// PreviousDayAttendanceReport
        /// </summary>
        /// <returns></returns>
        private string PreviousDayAttendanceReport(DailyReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 8 FEB 2017
        {
            //bool ReturnStatus = false;
            string PdfFileName = "";

            try
            {
                // ---------------------------------------REPORT RDLC PreviousDayAttendance START-----------------------------------------
                // PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);
                //DateTime PrevDay = Convert.ToDateTime(ReportModel.FromDate).AddDays(-1);

                SqlCommand com1 = new SqlCommand("usp_Rpt_GetTimeCard", con);

                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018

                // ADDED BY MSJ ON 21 FEB 2020 START
                DateTime CurrentDate = DateTime.Now.Date;
                DateTime PrevDay = CurrentDate.AddDays(-1);

                // ADDED BY MSJ ON 27 FEB 2020 START
                ReportModel.FromDate = PrevDay.ToString("dd/MM/yyyy");
                ReportModel.ToDate = ReportModel.FromDate;

                com1.CommandType = CommandType.StoredProcedure;
                com1.Parameters.Add(new SqlParameter("@Fdate", SqlDbType.DateTime)).Value = ReportModel.FromDate; //new DateTime(2013, 01, 01);
                com1.Parameters.Add(new SqlParameter("@Tdate", SqlDbType.DateTime)).Value = ReportModel.ToDate; //new DateTime(2013, 01, 01);
                // ADDED BY MSJ ON 21 FEB 2020 END

                com1.Parameters.Add(new SqlParameter("@RoleTypeId", SqlDbType.NVarChar, 500)).Value = "1";
                com1.Parameters.Add(new SqlParameter("@ReportingId", SqlDbType.Int)).Value = DBNull.Value;
                com1.Parameters.Add(new SqlParameter("@Empid", SqlDbType.NVarChar)).Value = ReportModel.EmployeeString;

                // Branch selection
                if (ReportModel.BranchId != 0)
                {
                    com1.Parameters.Add(new SqlParameter("@BranchListId", SqlDbType.NVarChar, 500)).Value = ReportModel.BranchId;
                }
                else
                {
                    com1.Parameters.Add(new SqlParameter("@BranchListId", SqlDbType.NVarChar, 500)).Value = DBNull.Value;
                }

                // company selection
                if (ReportModel.CompanyId != 0)
                {
                    com1.Parameters.Add(new SqlParameter("@CompanyListId", SqlDbType.NVarChar, 500)).Value = ReportModel.CompanyId;
                }
                else
                {
                    com1.Parameters.Add(new SqlParameter("@CompanyListId", SqlDbType.NVarChar, 500)).Value = DBNull.Value;
                }

                SqlDataAdapter da1 = new SqlDataAdapter(com1);

                da1.Fill(table5);

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/PreviousDayAttendanceReport.rdlc",
                    "PreviousDayAttendance", "~/User_Data/download/", "Previous_Day_Attendance_Report_", ref PdfFileName);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

                //ReturnStatus = true;

            }
            catch (System.Exception)
            {
                //throw;
            }

            return PdfFileName;

            // PRINT END
            //---------------------------------------------REPORT RDLC PreviousDayAttendance END-------------------------------------------
        }

        /// <summary>
        /// AttendanceSummaryReport
        /// usp_Report_GetMonthlyAttendance
        /// </summary>
        /// <returns></returns>
        private string AttendanceSummary(MonthlyReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 8 FEB 2017
        {
            //bool ReturnStatus = false;
            string PdfFileName = "";

            try
            {
                // ---------------------------------------REPORT RDLC AttendanceSummary START-----------------------------------------
                // PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                SqlCommand com1 = new SqlCommand("usp_Report_GetAllAttendanceSumarry", con);

                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018

                com1.CommandType = CommandType.StoredProcedure;
                com1.Parameters.Add(new SqlParameter("@Fdate", SqlDbType.DateTime)).Value = ReportModel.FromDate; //new DateTime(2013, 01, 01);
                com1.Parameters.Add(new SqlParameter("@Tdate", SqlDbType.DateTime)).Value = ReportModel.ToDate; //new DateTime(2013, 01, 31);
                com1.Parameters.Add(new SqlParameter("@EmpidList", SqlDbType.NVarChar)).Value = ReportModel.EmployeeString; //DBNull.Value; //"1195";

                SqlDataAdapter da1 = new SqlDataAdapter(com1);

                com1.CommandTimeout = 3000;

                da1.Fill(table5);

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/AttendanceSummaryReport.rdlc",
                 "AttendanceSummary", "~/User_Data/download/", "Attendance_Summary_Report_", ref PdfFileName);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

                //ReturnStatus = true;
            }
            catch (System.Exception)
            {
                //throw;
            }

            return PdfFileName;

            // PRINT END
            //---------------------------------------------REPORT RDLC AttendanceSummary END-------------------------------------------
        }

        /// <summary>
        /// MonthlyMusterReport
        /// </summary>
        /// <returns></returns>
        private string MonthlyMusterReport(MonthlyReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 8 FEB 2017
        {
            //bool ReturnStatus = false;
            string PdfFilename = "";

            try
            {

                // ADDED BY MSJ ON 29 DEC 2017 START
                // GET FROM AND TO DATE FROM YEAR AND MONTH ID

                //FinancialYear SelectedAttendenceYear = WetosDB.FinancialYears.Where(a => a.FinancialYearId == ReportModel.FinancialYearId).FirstOrDefault();

                // GET FY from ID

                //ADDED IF CONDITION BY SHRADDHA ON 15 JAN 2018 START

                string opt = fc["opt"];
                if (opt == "MonthWise")
                {
                    ReportModel.FromDate = null;
                    ReportModel.ToDate = null;
                }
                else
                {
                    ReportModel.FinancialYearId = 0;
                    ReportModel.MonthId = 0;
                }

                if (ReportModel.FromDate == null && ReportModel.ToDate == null)
                {
                    int SelectedYear = Convert.ToInt32(ReportModel.FinancialYearId); // Convert.ToInt32(SelectedAttendenceYear.FinancialName);

                    // Get MOnthNo from Id
                    int SelectedMonth = Convert.ToInt32(ReportModel.MonthId);

                    ReportModel.FromDate = FirstDayOfMonthFromDateTime(new DateTime(SelectedYear, SelectedMonth, 1)).ToString("yyyy/MM/dd");
                    ReportModel.ToDate = LastDayOfMonthFromDateTime(new DateTime(SelectedYear, SelectedMonth, 1)).ToString("yyyy/MM/dd");
                }
                //ADDED IF CONDITION BY SHRADDHA ON 15 JAN 2018 END

                // ADDED BY MSJ ON 29 DEC 2017 END


                // ---------------------------------------REPORT RDLC MonthlyMusterReport START-----------------------------------------
                // PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                //SqlCommand com1 = new SqlCommand("usp_Report_GetMonthlyMusterPart2", con);
                SqlCommand com1 = new SqlCommand("usp_Report_GetMonthlyAttendance", con);

                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018

                com1.CommandType = CommandType.StoredProcedure;
                com1.Parameters.Add(new SqlParameter("@Fdate", SqlDbType.DateTime)).Value = ReportModel.FromDate; //new DateTime(2013, 01, 01);
                com1.Parameters.Add(new SqlParameter("@Tdate", SqlDbType.DateTime)).Value = ReportModel.ToDate; //new DateTime(2013, 01, 02);

                // Updated by Rajas on 13 Jan 2017

                #region EMPLOYEE STRING
                //string EmployeeString = "";

                //if (ReportModel.CompanyId == 0 && ReportModel.BranchId == 0 && ReportModel.DepartmentId == 0 && ReportModel.EmployeeId == 0 && ReportModel.EmployeeTypeId == 0)
                //{
                //    EmployeeString = (String.Join(",", WetosDB.Employees.Where(a => a.ActiveFlag == null || a.ActiveFlag == true).Select(a => a.EmployeeId).ToArray()));

                //}
                //else if (ReportModel.CompanyId == 0 && ReportModel.BranchId == 0 && ReportModel.DepartmentId == 0 && ReportModel.EmployeeId == 0 && ReportModel.EmployeeTypeId != 0)
                //{
                //    EmployeeString = (String.Join(",", WetosDB.Employees.Where(a => a.ActiveFlag == null || a.ActiveFlag == true && a.EmployeeTypeId == ReportModel.EmployeeTypeId).Select(a => a.EmployeeId).ToArray()));

                //}
                //else if (ReportModel.BranchId == 0 && ReportModel.DepartmentId == 0 && ReportModel.EmployeeId == 0 && ReportModel.EmployeeTypeId == 0)
                //{

                //    EmployeeString = (String.Join(",", WetosDB.Employees.Where(a => a.ActiveFlag == null || a.ActiveFlag == true && a.CompanyId == ReportModel.CompanyId).Select(a => a.EmployeeId).ToArray()));
                //}
                //else if (ReportModel.BranchId == 0 && ReportModel.DepartmentId == 0 && ReportModel.EmployeeId == 0 && ReportModel.EmployeeTypeId != 0)
                //{

                //    EmployeeString = (String.Join(",", WetosDB.Employees.Where(a => a.ActiveFlag == null || a.ActiveFlag == true && a.CompanyId == ReportModel.CompanyId && a.EmployeeTypeId == ReportModel.EmployeeTypeId).Select(a => a.EmployeeId).ToArray()));
                //}

                //else if (ReportModel.DepartmentId == 0 && ReportModel.EmployeeId == 0 && ReportModel.EmployeeTypeId == 0)
                //{
                //    EmployeeString = (String.Join(",", WetosDB.Employees.Where(a => a.ActiveFlag == null || a.ActiveFlag == true && a.CompanyId == ReportModel.CompanyId && a.BranchId == ReportModel.BranchId).Select(a => a.EmployeeId).ToArray()));
                //}

                //else if (ReportModel.DepartmentId == 0 && ReportModel.EmployeeId == 0 && ReportModel.EmployeeTypeId != 0)
                //{
                //    EmployeeString = (String.Join(",", WetosDB.Employees.Where(a => a.ActiveFlag == null || a.ActiveFlag == true && a.CompanyId == ReportModel.CompanyId && a.BranchId == ReportModel.BranchId && a.EmployeeTypeId == ReportModel.EmployeeTypeId).Select(a => a.EmployeeId).ToArray()));
                //}
                //// Updated by Rajas on 8 FEB 2017 START
                //else if (ReportModel.CompanyId == 0 && ReportModel.BranchId == 0 && ReportModel.DepartmentId == 0 && ReportModel.EmployeeId != 0)
                //{
                //    EmployeeString = fc["EmployeeId"];
                //}
                //else if (ReportModel.BranchId == 0 && ReportModel.DepartmentId == 0 && ReportModel.EmployeeId != 0)
                //{

                //    EmployeeString = fc["EmployeeId"];
                //}
                //else if (ReportModel.DepartmentId == 0 && ReportModel.EmployeeId != 0)
                //{
                //    EmployeeString = fc["EmployeeId"];
                //}
                //// Updated by Rajas on 8 FEB 2017 END

                //else if (ReportModel.EmployeeId != 0)
                //{
                //    // Updated by Rajas on 6 SEP 2017 START
                //    EmployeeString = fc["EmployeeId"];
                //    // Updated by Rajas on 6 SEP 2017 END
                //}
                //else if (ReportModel.EmployeeTypeId != 0)
                //{
                //    // Updated by Rajas on 6 SEP 2017 START
                //    EmployeeString = fc["EmployeeId"];
                //    // Updated by Rajas on 6 SEP 2017 END
                //}
                //else
                //{
                //    // Updated by Rajas on 6 SEP 2017 START
                //    EmployeeString = EmployeeString = (String.Join(",", WetosDB.Employees.Where(a => a.ActiveFlag == null || a.ActiveFlag == true
                //        && a.CompanyId == ReportModel.CompanyId && a.BranchId == ReportModel.BranchId && a.EmployeeReportingId == ReportModel.EmployeeId && a.EmployeeTypeId == ReportModel.EmployeeTypeId).Select(a => a.EmployeeId).ToArray()))
                //        + "," + fc["EmployeeId"];
                //    // Updated by Rajas on 6 SEP 2017 END
                //}
                #endregion

                com1.Parameters.Add(new SqlParameter("@EmpidList", SqlDbType.NVarChar)).Value = ReportModel.EmployeeString;
                //com1.Parameters.Add(new SqlParameter("@BranchListId", SqlDbType.NVarChar, 500)).Value = "1,3,7";
                //com1.Parameters.Add(new SqlParameter("@CompanyListId", SqlDbType.NVarChar, 500)).Value = "2";
                //com1.Parameters.Add(new SqlParameter("@RoleTypeId", SqlDbType.NVarChar, 500)).Value = "4";
                //com1.Parameters.Add(new SqlParameter("@ReportingId", SqlDbType.Int)).Value = "76";
                SqlDataAdapter da1 = new SqlDataAdapter(com1);

                da1.Fill(table5);

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/MonthlyMuster.rdlc",
                 "abc", "~/User_Data/download/", "Monthly_Muster_Report_", ref PdfFilename);

                //Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";"); // ADDED BY MSJ ON 02 MARCH 2018

                //ReturnStatus = true;

            }
            catch (System.Exception ex)
            {
                AddAuditTrail("IE " + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));
            }

            return PdfFilename;

            // PRINT END
            //---------------------------------------------REPORT RDLC MonthlyMusterReport END-------------------------------------------
        }

        /// <summary>
        /// MonthlyWorkingHoursReport
        /// </summary>
        /// <returns></returns>
        private string MonthlyWorkingHoursReport(MonthlyReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 8 FEB 2017
        {
            //bool ReturnStatus = false;
            string PdfFileName = "";

            try
            {
                // ---------------------------------------REPORT RDLC MonthlyWorkingHoursReport START-----------------------------------------
                // PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                SqlCommand com1 = new SqlCommand("usp_Report_GetMonthlyWorkingHours", con);

                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018

                com1.CommandType = CommandType.StoredProcedure;
                com1.Parameters.Add(new SqlParameter("@Fdate", SqlDbType.DateTime)).Value = ReportModel.FromDate; //new DateTime(2013, 01, 01);
                com1.Parameters.Add(new SqlParameter("@Tdate", SqlDbType.DateTime)).Value = ReportModel.ToDate; //new DateTime(2013, 01, 31);
                com1.Parameters.Add(new SqlParameter("@EmpidList", SqlDbType.NVarChar)).Value = ReportModel.EmployeeString;

                //com1.Parameters.Add(new SqlParameter("@BranchListId", SqlDbType.NVarChar, 500)).Value = "1,3,7";
                //com1.Parameters.Add(new SqlParameter("@CompanyListId", SqlDbType.NVarChar, 500)).Value = "2";
                //com1.Parameters.Add(new SqlParameter("@RoleTypeId", SqlDbType.NVarChar, 500)).Value = "4";
                //com1.Parameters.Add(new SqlParameter("@ReportingId", SqlDbType.Int)).Value = "76";
                SqlDataAdapter da1 = new SqlDataAdapter(com1);

                da1.Fill(table5);
                //

                System.Data.DataColumn newColumn = new System.Data.DataColumn("TotalWHrs", typeof(System.String));
                newColumn.DefaultValue = "0.00";
                table5.Columns.Add(newColumn);

                //
                //int TotalMin = 0;
                foreach (DataRow dr in table5.Rows)
                {
                    int TotalMin = 0;

                    foreach (DataColumn dc in table5.Columns)
                    {
                        string DailyHrs = dr[dc].ToString();
                        if (DailyHrs.Length > 2 && DailyHrs.Length < 6)
                        {
                            int Hrs = 0;
                            int Min = 0;

                            string[] HrsMin = DailyHrs.Split(':');

                            if (HrsMin.Count() == 2)
                            {
                                Hrs = Convert.ToInt32(HrsMin[0]);
                                Min = Convert.ToInt32(HrsMin[1]);
                            }

                            TotalMin = TotalMin + ((Hrs * 60) + Min);
                        }
                    }

                    int CalcHrs = TotalMin / 60;
                    int CalcMin = TotalMin - (CalcHrs * 60);

                    dr["TotalWHrs"] = CalcHrs.ToString("#00") + ":" + CalcMin.ToString("#00");
                }

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/MonthlyWorkingHoursReport.rdlc",
                 "MonthlyWorkingHours", "~/User_Data/download/", "Monthly_Working_Hours_Report_", ref PdfFileName);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

                //ReturnStatus = true;

            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in MonthlyMusterReport " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));
            }

            return PdfFileName;

            // PRINT END
            //---------------------------------------------REPORT RDLC MonthlyWorkingHoursReport END-------------------------------------------
        }

        /// <summary>
        /// ContinuousAbsentismReport
        /// </summary>
        /// <returns></returns>
        private string ContinuousAbsentismReport(MonthlyReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 8 FEB 2017
        {
            //bool ReturnStatus = false;
            string PdfFileName = "";

            try
            {

                // ---------------------------------------REPORT RDLC ContinuousAbsentismReport START-----------------------------------------
                // PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                SqlCommand com1 = new SqlCommand("usp_Report_GetAllCantinousAbsentisum", con);

                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018

                com1.CommandType = CommandType.StoredProcedure;
                com1.Parameters.Add(new SqlParameter("@Fdate", SqlDbType.DateTime)).Value = ReportModel.FromDate; //new DateTime(2013, 01, 01);
                com1.Parameters.Add(new SqlParameter("@Tdate", SqlDbType.DateTime)).Value = ReportModel.ToDate; //new DateTime(2014, 01, 31);
                com1.Parameters.Add(new SqlParameter("@NoOfDays", SqlDbType.Int)).Value = 1;


                com1.Parameters.Add(new SqlParameter("@EmpidList", SqlDbType.NVarChar, 500)).Value = ReportModel.EmployeeString;
                //com1.Parameters.Add(new SqlParameter("@CompanyListId", SqlDbType.NVarChar, 500)).Value = "2";
                //com1.Parameters.Add(new SqlParameter("@RoleTypeId", SqlDbType.NVarChar, 500)).Value = "4";
                //com1.Parameters.Add(new SqlParameter("@ReportingId", SqlDbType.Int)).Value = "76";
                SqlDataAdapter da1 = new SqlDataAdapter(com1);

                da1.Fill(table5);

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/ContinuousAbsentismReport.rdlc",
                 "ContAbsent", "~/User_Data/download/", "Long_Absenty_Report_", ref PdfFileName);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

                //ReturnStatus = true;

            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in MonthlyMusterReport " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));
            }

            return PdfFileName;

            // PRINT END
            //---------------------------------------------REPORT RDLC ContinuousAbsentismReport END-------------------------------------------
        }


        /// <summary>
        /// LateComing
        /// </summary>
        /// <returns></returns>
        private string LateComing(MonthlyReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 8 FEB 2017
        {
            //bool ReturnStatus = false;
            string PdfFilename = "";

            try
            {
                // ---------------------------------------REPORT RDLC LateComingReport START-----------------------------------------
                // PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                SqlCommand com1 = new SqlCommand("usp_rptLateAttendance", con);

                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018

                com1.CommandType = CommandType.StoredProcedure;
                com1.Parameters.Add(new SqlParameter("@Fromdate", SqlDbType.DateTime)).Value = ReportModel.FromDate; //new DateTime(2013, 01, 01);
                com1.Parameters.Add(new SqlParameter("@Todate", SqlDbType.DateTime)).Value = ReportModel.ToDate; // new DateTime(2013, 01, 31);
                com1.Parameters.Add(new SqlParameter("@Employeeidrpt", SqlDbType.NVarChar)).Value = ReportModel.EmployeeString; //"1504";
                //com1.Parameters.Add(new SqlParameter("@NoOfDays", SqlDbType.NVarChar, 500)).Value = "6";
                //com1.Parameters.Add(new SqlParameter("@CompanyListId", SqlDbType.NVarChar, 500)).Value = "2";
                //com1.Parameters.Add(new SqlParameter("@RoleTypeId", SqlDbType.NVarChar, 500)).Value = "4";
                //com1.Parameters.Add(new SqlParameter("@ReportingId", SqlDbType.Int)).Value = "76";

                SqlDataAdapter da1 = new SqlDataAdapter(com1);

                da1.Fill(table5);

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/LateComingReport.rdlc",
                 "LateSumarry", "~/User_Data/download/", "Daily_Late_Arrival_Report_", ref PdfFilename);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

                //ReturnStatus = true;

            }
            catch (System.Exception)
            {
                //throw;
            }

            return PdfFilename;

            // PRINT END
            //---------------------------------------------REPORT RDLC LateComingReport END-------------------------------------------
        }

        /// <summary>
        /// usp_GetEmployeeforAbsentismReport
        /// AbsenteeismReport
        /// </summary>
        /// <returns></returns>
        private string AbsenteeismReport(MonthlyReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 8 FEB 2017
        {
            //bool ReturnStatus = false;
            string PdfFileName = "";

            try
            {
                // ---------------------------------------REPORT RDLC AbsenteeismReport START-----------------------------------------
                // PRINT START
                DataTable table5 = new DataTable();

                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                SqlCommand com1 = new SqlCommand("usp_GetEmployeeforAbsentismReport", con);

                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018

                com1.CommandType = CommandType.StoredProcedure;

                com1.Parameters.Add(new SqlParameter("@dtfrom", SqlDbType.DateTime)).Value = ReportModel.FromDate; //new DateTime(2013, 01, 01);
                com1.Parameters.Add(new SqlParameter("@todate", SqlDbType.DateTime)).Value = ReportModel.ToDate; //new DateTime(2013, 01, 31);
                com1.Parameters.Add(new SqlParameter("@EmployeeListId", SqlDbType.NVarChar)).Value = ReportModel.EmployeeString; //DBNull.Value; //"1200, 1195, 1205";  //"1504, 1508" ;
                SqlDataAdapter da1 = new SqlDataAdapter(com1);
                com1.CommandTimeout = 3000;

                da1.Fill(table5);

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/AbsenteeismReport.rdlc",
                 "AbsenteeismReport", "~/User_Data/download/", "Absenteeism_Report_", ref PdfFileName);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

                //ReturnStatus = true;

            }
            catch (System.Exception ex1)
            {
                //throw;
            }

            return PdfFileName;

            // PRINT END
            //---------------------------------------------REPORT RDLC AbsenteeismReport END-------------------------------------------
        }

        /// <summary>
        /// usp_Generate_PayslipReport
        /// GeneratePaySlipReport
        /// </summary>
        /// <returns></returns>
        private string GeneratePaySlipReport(MonthlyReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 8 FEB 2017
        {
            //bool ReturnStatus = false;
            string PdfFileName = "";

            try
            {
                // ---------------------------------------REPORT RDLC PaySlipReport START-----------------------------------------
                // PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                SqlCommand com1 = new SqlCommand("usp_Generate_PayslipReport", con);

                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018

                com1.CommandType = CommandType.StoredProcedure;
                com1.Parameters.Add(new SqlParameter("@Month", SqlDbType.Int)).Value = "2";
                com1.Parameters.Add(new SqlParameter("@Year", SqlDbType.Int)).Value = "2014";

                // Added By Rajas on 22 DEC 2016 for EmployeeId list selected branchwise START
                //List<WetosDB.Employee> EmployeeList = WetosDB.Employees.ToList();
                string EmployeeString = "";
                EmployeeString = (String.Join(",", WetosDB.Employees.Where(a => a.BranchId == ReportModel.BranchId).Select(a => a.EmployeeId).ToArray()));
                // Added By Rajas on 22 DEC 2016 for EmployeeId list selected branchwise END

                com1.Parameters.Add(new SqlParameter("@EmplpyeeCode", SqlDbType.NVarChar)).Value = EmployeeString;
                SqlDataAdapter da1 = new SqlDataAdapter(com1);

                da1.Fill(table5);

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/PaySlipReport.rdlc",
                 "PaySlip", "~/User_Data/download/", "Pay_Slip_Report_", ref PdfFileName);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

                //ReturnStatus = true;

            }
            catch (System.Exception)
            {
                //throw;
            }

            return PdfFileName;

            // PRINT END
            //---------------------------------------------REPORT RDLC PaySlipReport END-------------------------------------------
        }

        /// <summary>
        /// LunchLateEmployeeReport
        /// </summary>
        /// <returns></returns>
        private string LunchLateEmployee(MonthlyReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 8 FEB 2017
        {
            //bool ReturnStatus = false;
            string PdfFileName = "";

            try
            {// ---------------------------------------REPORT RDLC LunchLateEmployeeReport START-----------------------------------------
                // PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                SqlCommand com1 = new SqlCommand("usp_LunchTimeLateComersReport", con);

                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018

                com1.CommandType = CommandType.StoredProcedure;
                com1.Parameters.Add(new SqlParameter("@date", SqlDbType.DateTime)).Value = ReportModel.FromDate; //new DateTime(2013, 01, 01);
                com1.Parameters.Add(new SqlParameter("@BeforeSearchHrs", SqlDbType.Int)).Value = "2";
                com1.Parameters.Add(new SqlParameter("@AfterSearchHrs", SqlDbType.Int)).Value = "6";

                com1.Parameters.Add(new SqlParameter("@employeeid", SqlDbType.NVarChar, 500)).Value = ReportModel.EmployeeString; //DBNull.Value; //"1504";
                //com1.Parameters.Add(new SqlParameter("@CompanyListId", SqlDbType.NVarChar, 500)).Value = "2";
                //com1.Parameters.Add(new SqlParameter("@RoleTypeId", SqlDbType.NVarChar, 500)).Value = "4";
                //com1.Parameters.Add(new SqlParameter("@ReportingId", SqlDbType.Int)).Value = "76";
                SqlDataAdapter da1 = new SqlDataAdapter(com1);

                da1.Fill(table5);

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/LunchLateEmployeeReport.rdlc",
                 "LunchTimeLateComers", "~/User_Data/download/", "Lunch_Late_Employee_Report_", ref PdfFileName);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

                //ReturnStatus = true;

            }
            catch (System.Exception)
            {
                //throw;
            }

            return PdfFileName;

            // PRINT END
            //---------------------------------------------REPORT RDLC LunchLateEmployeeReport END-------------------------------------------
        }

        /// <summary>
        /// LateSummaryReport
        /// </summary>
        /// <returns></returns>
        private string LateSummaryReport(MonthlyReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 8 FEB 2017
        {
            //bool ReturnStatus = false;
            string PdfFileName = "";

            try
            {

                // ---------------------------------------REPORT RDLC LateSummaryReport START-----------------------------------------
                // PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                SqlCommand com1 = new SqlCommand("usp_Report_GetAllLateSummaryInformation", con);

                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018

                com1.CommandType = CommandType.StoredProcedure;
                com1.Parameters.Add(new SqlParameter("@Fdate", SqlDbType.DateTime)).Value = ReportModel.FromDate; //new DateTime(2013, 01, 01);
                com1.Parameters.Add(new SqlParameter("@Tdate", SqlDbType.DateTime)).Value = ReportModel.ToDate; //new DateTime(2013, 01, 31);

                com1.Parameters.Add(new SqlParameter("@EmpidList", SqlDbType.NVarChar)).Value = ReportModel.EmployeeString; //DBNull.Value; //"1503";
                //com1.Parameters.Add(new SqlParameter("@BranchListId", SqlDbType.NVarChar, 500)).Value = "1,3,7";
                //com1.Parameters.Add(new SqlParameter("@CompanyListId", SqlDbType.NVarChar, 500)).Value = "2";
                //com1.Parameters.Add(new SqlParameter("@RoleTypeId", SqlDbType.NVarChar, 500)).Value = "4";
                //com1.Parameters.Add(new SqlParameter("@ReportingId", SqlDbType.Int)).Value = "76";
                SqlDataAdapter da1 = new SqlDataAdapter(com1);

                da1.Fill(table5);

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/LateSummaryReport.rdlc",
                 "Late", "~/User_Data/download/", "Late_Summary_Report_", ref PdfFileName);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

                //ReturnStatus = true;

            }
            catch (System.Exception)
            {
                //throw;
            }

            return PdfFileName;

            // PRINT END
            //---------------------------------------------REPORT RDLC LateSummaryReport END-------------------------------------------
        }

        /// <summary>
        /// LeaveApplicationReport
        /// </summary>
        /// <returns></returns>
        private string LeaveApplicationReport(MonthlyReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 8 FEB 2017
        {
            //bool ReturnStatus = false;
            string PdfFileName = "";

            try
            {
                // ---------------------------------------REPORT RDLC LeaveApplicationReport START-----------------------------------------
                // PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                //SqlCommand com1 = new SqlCommand("usp_Rpt_EmpLeaveDetails", con);
                SqlCommand com1 = new SqlCommand("usp_Rpt_EmpLeaveDetails", con);

                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018

                com1.CommandType = CommandType.StoredProcedure;
                com1.Parameters.Add(new SqlParameter("@Fdate", SqlDbType.DateTime)).Value = ReportModel.FromDate; //new DateTime(2013, 01, 01);
                com1.Parameters.Add(new SqlParameter("@Tdate", SqlDbType.DateTime)).Value = ReportModel.ToDate; //new DateTime(2013, 01, 31);
                com1.Parameters.Add(new SqlParameter("@Empid", SqlDbType.NVarChar)).Value = ReportModel.EmployeeString; //DBNull.Value; //"1503";

                // PLEASE DO NOT DELETE THIS REGION, ADDED BY RAJAS ON 9 FEB 2017
                #region NOTE
                ///ReportModel.Status has to be updated in case of Eviska as Status is NVarChar field as S,P and R
                ///Status should be saved as S,P, R not as StatusId based or Sanctioned, Pending or Rejected
                #endregion

                // Updated by Rajas on 9 FEB 2017, Removed hardcode S,P,R status in place used StatusId and SqlDbType.NVarChar replaced with SqlDbType.Int
                com1.Parameters.Add(new SqlParameter("@Status", SqlDbType.Int)).Value = ReportModel.Status;


                SqlDataAdapter da1 = new SqlDataAdapter(com1);

                da1.Fill(table5);

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/LeaveApplicationReport.rdlc",
                 "LeaveApplication", "~/User_Data/download/", "Leave_Application_Report_", ref PdfFileName);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

                //ReturnStatus = true;

            }
            catch (System.Exception ex1)
            {
                //throw;
            }

            return PdfFileName;

            // PRINT END
            //---------------------------------------------REPORT RDLC LeaveApplicationReport END-------------------------------------------
        }


        // Added by Pushkar on 29 SEP -------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// BranchMasterReport
        /// </summary>
        /// <returns></returns>
        private string BranchMasterReport(MasterReportsModel ReportModel)
        {
            string PdfFileName = "";
            // bool ReturnStatus = false;

            try
            {
                // ------------------------------------------------REPORT RDLC BranchMasterReport START-------------------------------------------
                //// PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                SqlCommand com1 = new SqlCommand("usp_Report_BranchDetails", con);

                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018

                com1.CommandType = CommandType.StoredProcedure;
                //com1.Parameters.Add(new SqlParameter("@date", SqlDbType.DateTime)).Value = new DateTime(2011, 05, 01);
                //com1.Parameters.Add(new SqlParameter("@empidList", SqlDbType.VarChar)).Value = "123,8";
                SqlDataAdapter da1 = new SqlDataAdapter(com1);
                da1.Fill(table5);

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/BranchMasterReport.rdlc",
                 "BranchMaster", "~/User_Data/download/", "Branch_Master_Report_", ref PdfFileName);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

                //ReturnStatus = true;                
            }
            catch (System.Exception ex1)
            {
                //throw;
            }

            return PdfFileName;



            // PRINT END
            //---------------------------------------------REPORT RDLC BranchMasterReport END-------------------------------------------

        }


        /// <summary>
        /// CompanyMasterReport
        /// </summary>
        /// <returns></returns>
        private string CompanyMasterReport(MasterReportsModel ReportModel)
        {
            //bool ReturnStatus = false;
            string PdfFileName = "";
            try
            {
                // ------------------------------------------------REPORT RDLC CompanyMasterReport START-------------------------------------------
                //// PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                SqlCommand com1 = new SqlCommand("usp_Report_CompanyDetails", con);

                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018

                com1.CommandType = CommandType.StoredProcedure;
                //com1.Parameters.Add(new SqlParameter("@date", SqlDbType.DateTime)).Value = new DateTime(2011, 05, 01);
                //com1.Parameters.Add(new SqlParameter("@empidList", SqlDbType.VarChar)).Value = "123,8";
                SqlDataAdapter da1 = new SqlDataAdapter(com1);

                da1.Fill(table5);

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/CompanyMasterReport.rdlc",
                 "CompanyMaster", "~/User_Data/download/", "Company_Master_Report_", ref PdfFileName);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

                //ReturnStatus = true;

            }
            catch (System.Exception Ex)
            {
                //throw;
            }

            return PdfFileName;


            // PRINT END
            //---------------------------------------------REPORT RDLC CompanyMasterReport END-------------------------------------------

        }


        /// <summary>
        /// DepartmentMasterReport
        /// </summary>
        /// <returns></returns>
        private string DepartmentMasterReport(MasterReportsModel ReportModel)
        {
            //bool ReturnStatus = false;
            string PdfFileName = "";
            try
            {
                // ------------------------------------------------REPORT RDLC DepartmentMasterReport START-------------------------------------------
                //// PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                SqlCommand com1 = new SqlCommand("usp_Report_DepartmentDetails", con);

                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018

                com1.CommandType = CommandType.StoredProcedure;
                //com1.Parameters.Add(new SqlParameter("@date", SqlDbType.DateTime)).Value = new DateTime(2011, 05, 01);
                //com1.Parameters.Add(new SqlParameter("@empidList", SqlDbType.VarChar)).Value = "123,8";
                SqlDataAdapter da1 = new SqlDataAdapter(com1);
                da1.Fill(table5);

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/DepartmentMasterReport.rdlc",
                 "DepartmentMaster", "~/User_Data/download/", "Department_Master_Report_", ref PdfFileName);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

                //ReturnStatus = true;

            }
            catch (System.Exception)
            {
                //throw;
            }

            return PdfFileName;


            // PRINT END
            //---------------------------------------------REPORT RDLC DepartmentMasterReport END-------------------------------------------

        }


        // Added by Pushkar on 29 SEP ---------------------------------------------------------------------------------------------------------------------



        // Added by Pushkar on 30 SEP ---------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// DesignationMasterReport
        /// </summary>
        /// <returns></returns>
        private string DesignationMasterReport(MasterReportsModel ReportModel)
        {
            //bool ReturnStatus = false;
            string PdfFileName = "";

            try
            {
                // ------------------------------------------------REPORT RDLC DesignationMasterReport START-------------------------------------------
                //// PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                SqlCommand com1 = new SqlCommand("usp_Report_DesignationDetails", con);

                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018

                com1.CommandType = CommandType.StoredProcedure;
                //com1.Parameters.Add(new SqlParameter("@date", SqlDbType.DateTime)).Value = new DateTime(2011, 05, 01);
                //com1.Parameters.Add(new SqlParameter("@empidList", SqlDbType.VarChar)).Value = "123,8";
                SqlDataAdapter da1 = new SqlDataAdapter(com1);
                da1.Fill(table5);

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/DesignationMasterReport.rdlc",
                 "DesignationMaster", "~/User_Data/download/", "Designation_Master_Report_", ref PdfFileName);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

                //ReturnStatus = true;

            }
            catch (System.Exception)
            {
                //throw;
            }

            return PdfFileName;


            // PRINT END
            //---------------------------------------------REPORT RDLC DesignationMasterReport END-------------------------------------------

        }




        /// <summary>
        /// EmployeeGroupMasterReport
        /// </summary>
        /// <returns></returns>
        private string EmployeeGroupMasterReport(MasterReportsModel ReportModel)
        {
            //bool ReturnStatus = false;
            string PdfFileName = "";

            try
            {
                // ------------------------------------------------REPORT RDLC EmployeeGroupMasterReport START-------------------------------------------
                //// PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                SqlCommand com1 = new SqlCommand("usp_Report_EmployeeGroupDetails", con);

                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018

                com1.CommandType = CommandType.StoredProcedure;
                //com1.Parameters.Add(new SqlParameter("@date", SqlDbType.DateTime)).Value = new DateTime(2011, 05, 01);
                //com1.Parameters.Add(new SqlParameter("@empidList", SqlDbType.VarChar)).Value = "123,8";
                SqlDataAdapter da1 = new SqlDataAdapter(com1);
                da1.Fill(table5);

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/EmployeeGroupMasterReport.rdlc",
                 "EmployeeGroupMaster", "~/User_Data/download/", "Employee_Group_Master_Report_", ref PdfFileName);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

                //ReturnStatus = true;

            }
            catch (System.Exception)
            {
                //throw;
            }

            return PdfFileName;


            // PRINT END
            //---------------------------------------------REPORT RDLC EmployeeGroupMasterReport END-------------------------------------------

        }


        /// <summary>
        /// GradeMasterReport
        /// </summary>
        /// <returns></returns>
        private string GradeMasterReport(MasterReportsModel ReportModel)
        {
            //bool ReturnStatus = false;
            string PdfFileName = "";

            try
            {
                // ------------------------------------------------REPORT RDLC GradeMasterReport START-------------------------------------------
                //// PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                // 
                //SqlCommand com1 = new SqlCommand("usp_Report_EmployeeGroupDetails", con);
                SqlCommand com1 = new SqlCommand("usp_Report_EmployeeGradeDetails", con);



                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018


                com1.CommandType = CommandType.StoredProcedure;
                //com1.Parameters.Add(new SqlParameter("@date", SqlDbType.DateTime)).Value = new DateTime(2011, 05, 01);
                //com1.Parameters.Add(new SqlParameter("@empidList", SqlDbType.VarChar)).Value = "123,8";
                SqlDataAdapter da1 = new SqlDataAdapter(com1);
                da1.Fill(table5);

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/GradeMasterReport.rdlc",
                 "GradeMaster", "~/User_Data/download/", "Grade_Master_Report_", ref PdfFileName);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

                //ReturnStatus = true;

            }
            catch (System.Exception)
            {
                //throw;
            }

            return PdfFileName;


            // PRINT END
            //---------------------------------------------REPORT RDLC GradeMasterReport END-------------------------------------------

        }


        /// <summary>
        /// ShiftRotationMasterReport
        /// </summary>
        /// <returns></returns>
        private string ShiftRotationMasterReport(MasterReportsModel ReportModel)
        {
            //bool ReturnStatus = false;
            string PdfFileName = "";

            try
            {
                // ------------------------------------------------REPORT RDLC ShiftRotationMasterReport START-------------------------------------------
                //// PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                SqlCommand com1 = new SqlCommand("usp_Report_ShiftRotationDetails", con);
                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018
                com1.CommandType = CommandType.StoredProcedure;
                //com1.Parameters.Add(new SqlParameter("@date", SqlDbType.DateTime)).Value = new DateTime(2011, 05, 01);
                //com1.Parameters.Add(new SqlParameter("@empidList", SqlDbType.VarChar)).Value = "123,8";
                SqlDataAdapter da1 = new SqlDataAdapter(com1);
                da1.Fill(table5);

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/ShiftRotationMasterReport.rdlc",
                 "ShiftRotationMaster", "~/User_Data/download/", "Shift_Rotation_Master_Report_", ref PdfFileName);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

                //ReturnStatus = true;

            }
            catch (System.Exception)
            {
                //throw;
            }

            return PdfFileName;


            // PRINT END
            //---------------------------------------------REPORT RDLC ShiftRotationMasterReport END-------------------------------------------

        }


        /// <summary>
        /// ShiftMasterReport
        /// </summary>
        /// <returns></returns>
        private string ShiftMasterReport(MasterReportsModel ReportModel)
        {
            //bool ReturnStatus = false;
            string PdfFileName = "";

            try
            {
                // ------------------------------------------------REPORT RDLC ShiftMasterReport START-------------------------------------------
                //// PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                SqlCommand com1 = new SqlCommand("usp_Report_ShiftDetails", con);
                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 20183
                com1.CommandType = CommandType.StoredProcedure;
                //com1.Parameters.Add(new SqlParameter("@date", SqlDbType.DateTime)).Value = new DateTime(2011, 05, 01);
                //com1.Parameters.Add(new SqlParameter("@empidList", SqlDbType.VarChar)).Value = "123,8";
                SqlDataAdapter da1 = new SqlDataAdapter(com1);
                da1.Fill(table5);

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/ShiftMasterReport.rdlc",
                 "ShiftMaster", "~/User_Data/download/", "Shift_Master_Report_", ref PdfFileName);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

                //ReturnStatus = true;

            }
            catch (System.Exception)
            {
                //throw;
            }

            return PdfFileName;


            // PRINT END
            //---------------------------------------------REPORT RDLC ShiftMasterReport END-------------------------------------------

        }

        /// <summary>
        /// EmployeeMasterReport
        /// </summary>
        /// <returns></returns>
        private string EmployeeMasterReport(MasterReportsModel ReportModel)
        {
            //bool ReturnStatus = false;
            string PdfFileName = "";

            try
            {
                // ------------------------------------------------REPORT RDLC EmployeeMasterReport START-------------------------------------------
                //// PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                SqlCommand com1 = new SqlCommand("usp_Report_EmployeeDetails", con);
                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018
                com1.CommandType = CommandType.StoredProcedure;
                //com1.Parameters.Add(new SqlParameter("@date", SqlDbType.DateTime)).Value = new DateTime(2011, 05, 01);
                //com1.Parameters.Add(new SqlParameter("@empidList", SqlDbType.VarChar)).Value = "123,8";
                SqlDataAdapter da1 = new SqlDataAdapter(com1);
                da1.Fill(table5);

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/EmployeeMasterReport.rdlc",
                 "EmployeeMaster", "~/User_Data/download/", "Employee_Master_Report_", ref PdfFileName);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

                //ReturnStatus = true;

            }
            catch (System.Exception)
            {
                //throw;
            }

            return PdfFileName;


            // PRINT END
            //---------------------------------------------REPORT RDLC EmployeeMasterReport END-------------------------------------------
        }


        //########################################

        /// <summary>
        /// EmployeeMasterReport
        /// </summary>
        /// <returns></returns>
        private string FamilyDetailsReport(MasterReportsModel ReportModel)
        {
            //bool ReturnStatus = false;
            string PdfFileName = "";

            try
            {
                // ------------------------------------------------REPORT RDLC EmployeeMasterReport START-------------------------------------------
                //// PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                SqlCommand com1 = new SqlCommand("SP_GetFamilyDetailsReport", con);
                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018
                com1.CommandType = CommandType.StoredProcedure;
                //com1.Parameters.Add(new SqlParameter("@date", SqlDbType.DateTime)).Value = new DateTime(2011, 05, 01);
                //com1.Parameters.Add(new SqlParameter("@empidList", SqlDbType.VarChar)).Value = "123,8";
                SqlDataAdapter da1 = new SqlDataAdapter(com1);
                da1.Fill(table5);

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/FamilyDetailsReport.rdlc",
                 "EmployeeMaster", "~/User_Data/download/", "FamilyDetails_Report_", ref PdfFileName);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

                //ReturnStatus = true;

            }
            catch (System.Exception)
            {
                //throw;
            }

            return PdfFileName;


            // PRINT END
            //---------------------------------------------REPORT RDLC EmployeeMasterReport END-------------------------------------------
        }


        //########################################

        /// <summary>
        /// HolidayMasterReport
        /// </summary>
        /// <returns></returns>
        private string HolidayMasterReport(MasterReportsModel ReportModel)
        {
            //bool ReturnStatus = false;
            string PdfFileName = "";

            try
            {
                // ------------------------------------------------REPORT RDLC HolidayMasterReport START-------------------------------------------
                //// PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                SqlCommand com1 = new SqlCommand("usp_Report_HoliDayDetails", con);
                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018
                com1.CommandType = CommandType.StoredProcedure;
                //com1.Parameters.Add(new SqlParameter("@date", SqlDbType.DateTime)).Value = new DateTime(2011, 05, 01);
                //com1.Parameters.Add(new SqlParameter("@empidList", SqlDbType.VarChar)).Value = "123,8";
                SqlDataAdapter da1 = new SqlDataAdapter(com1);
                da1.Fill(table5);

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/HolidayMasterReport.rdlc",
                 "HolidayMaster", "~/User_Data/download/", "Holiday_Master_Report_", ref PdfFileName);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

                //ReturnStatus = true;

            }
            catch (System.Exception)
            {
                //throw;
            }

            return PdfFileName;


            // PRINT END
            //---------------------------------------------REPORT RDLC HolidayMasterReport END-------------------------------------------
        }

        /// <summary>
        /// DailyEarlyDepartureReport
        /// </summary>
        /// <returns></returns>
        private string RequisitionApplicationReport(MonthlyReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 8 FEB 2017
        {
            // bool ReturnStatus = false;
            string PdfFileName = "";

            try
            {
                // ------------------------------------------------REPORT RDLC RequisitionApplicationReport START-------------------------------------------
                //// PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                SqlCommand com1 = new SqlCommand("usp_Report_GetAllODTourApplication", con);
                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018
                com1.CommandType = CommandType.StoredProcedure;
                com1.Parameters.Add(new SqlParameter("@Fdate", SqlDbType.DateTime)).Value = ReportModel.FromDate; //new DateTime(2013, 01, 01);
                com1.Parameters.Add(new SqlParameter("@Tdate", SqlDbType.DateTime)).Value = ReportModel.ToDate; //new DateTime(2013, 01, 01);
                com1.Parameters.Add(new SqlParameter("@EmpidList", SqlDbType.NVarChar)).Value = ReportModel.EmployeeString;

                // PLEASE DO NOT DELETE THIS REGION, ADDED BY RAJAS ON 9 FEB 2017
                #region NOTE
                ///ReportModel.Status has to be updated in case of Eviska as Status is NVarChar field as S,P and R
                ///Status should be saved as S,P, R not as StatusId based or Sanctioned, Pending or Rejected
                #endregion

                // Updated by Rajas on 9 FEB 2017, Removed hardcode S,P,R status in place used StatusId and SqlDbType.NVarChar replaced with SqlDbType.Int
                com1.Parameters.Add(new SqlParameter("@Status", SqlDbType.Int)).Value = DBNull.Value; // ReportModel.Status;

                SqlDataAdapter da1 = new SqlDataAdapter(com1);
                da1.Fill(table5);

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/RequisitionApplication.rdlc",
                 "ODTourApplication", "~/User_Data/download/", "Requisition_Application_Report_", ref PdfFileName);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

                //ReturnStatus = true;

            }
            catch (System.Exception)
            {
                //throw;
            }

            return PdfFileName;


            // PRINT END
            //---------------------------------------------REPORT RDLC RequisitionApplicationReport END-------------------------------------------
        }

        // Added by Pushkar on 30 SEP ----------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// DailyEarlyDepartureReport
        /// </summary>
        /// <returns></returns>
        private string DailyEarlyDepartureReport(MonthlyReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 8 FEB 2017
        {
            //bool ReturnStatus = false;
            string PdfFileName = "";

            try
            {
                //// PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                SqlCommand com1 = new SqlCommand("usp_Rpt_GetTimeCard", con);
                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018
                com1.CommandType = CommandType.StoredProcedure;
                com1.Parameters.Add(new SqlParameter("@Fdate", SqlDbType.DateTime)).Value = ReportModel.FromDate; //new DateTime(2013, 01, 01);
                com1.Parameters.Add(new SqlParameter("@Tdate", SqlDbType.DateTime)).Value = ReportModel.ToDate; //new DateTime(2013, 01, 01);
                com1.Parameters.Add(new SqlParameter("@RoleTypeId", SqlDbType.NVarChar, 500)).Value = "1";
                com1.Parameters.Add(new SqlParameter("@ReportingId", SqlDbType.Int)).Value = DBNull.Value;

                // Added by Rajas to generate report for all active employees on basis of selected parameter
                string EmployeeString = "";
                int? company = 0;
                int? branch = 0;
                if (ReportModel.CompanyId == 0 && ReportModel.BranchId == 0 && ReportModel.DepartmentId == 0 && ReportModel.EmployeeId == 0)
                {
                    // MODIFIED BY MSJ ON 29 JAN 2018
                    //EmployeeString = null;
                    EmployeeString = (String.Join(",", WetosDB.Employees.Where(a => a.ActiveFlag == null || a.ActiveFlag == true).Select(a => a.EmployeeId).ToArray()));

                }
                else if (ReportModel.BranchId == 0 && ReportModel.DepartmentId == 0 && ReportModel.EmployeeId == 0)
                {
                    company = ReportModel.CompanyId;
                    EmployeeString = (String.Join(",", WetosDB.Employees.Where(a => a.ActiveFlag == null || a.ActiveFlag == true && a.CompanyId == ReportModel.CompanyId).Select(a => a.EmployeeId).ToArray()));
                }
                else if (ReportModel.DepartmentId == 0 && ReportModel.EmployeeId == 0)
                {
                    company = ReportModel.CompanyId;
                    branch = ReportModel.BranchId;
                    EmployeeString = (String.Join(",", WetosDB.Employees.Where(a => a.ActiveFlag == null || a.ActiveFlag == true && a.CompanyId == ReportModel.CompanyId && a.BranchId == ReportModel.BranchId).Select(a => a.EmployeeId).ToArray()));
                }

                else
                {
                    // Modified by Rajas on 8 FEB 2017 START
                    EmployeeString = fc["EmployeeId"];
                    company = ReportModel.CompanyId;
                    branch = ReportModel.BranchId;
                    // Modified by Rajas on 8 FEB 2017 END
                }

                // Branch selection
                if (branch != 0)
                {
                    com1.Parameters.Add(new SqlParameter("@BranchListId", SqlDbType.NVarChar, 500)).Value = branch;
                }
                else
                {
                    com1.Parameters.Add(new SqlParameter("@BranchListId", SqlDbType.NVarChar, 500)).Value = DBNull.Value;
                }

                // company selection
                if (company != 0)
                {
                    com1.Parameters.Add(new SqlParameter("@CompanyListId", SqlDbType.NVarChar, 500)).Value = company;
                }
                else
                {
                    com1.Parameters.Add(new SqlParameter("@CompanyListId", SqlDbType.NVarChar, 500)).Value = DBNull.Value;
                }

                SqlDataAdapter da1 = new SqlDataAdapter(com1);

                Warning[] warnings;
                //string[] streamids;
                string mimeType;
                string encoding;
                //string filenameExtension;
                //string filePath;

                LocalReport localReport = new LocalReport();
                localReport.ReportPath = Server.MapPath("~/Reports/DailyEarlyDepartureReport.rdlc");
                //ReportDataSource reportDataSource = new ReportDataSource("Customers", Customers.GetAllCustomers());
                localReport.DataSources.Clear();
                localReport.DataSources.Add(new ReportDataSource("DailyEarly", table5));

                // Added by Rajas on 4 FEB 2017 START
                int EmpId = Convert.ToInt32(Session["EmployeeNo"]);

                // SELECT Employee code to generate pdf file name according to logged in user
                string EmployeeCode = WetosDB.Employees.Where(a => a.EmployeeId == EmpId).Select(a => a.EmployeeCode).FirstOrDefault();


                // Added by Rajas on 4 FEB 2017 END

                localReport.Refresh();

                // Added by Rajas on 15 MARCH 2017
                string reportType = string.Empty;

                if (ReportModel.ReportFormat == 2)
                {
                    reportType = "Excel";

                    PdfFileName = Server.MapPath("~/User_Data/download/") + "Daily_Early_Report_" + EmployeeCode + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xls";
                }
                else // PDF
                {
                    reportType = "PDF";

                    PdfFileName = Server.MapPath("~/User_Data/download/") + "Daily_Early_Report_" + EmployeeCode + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf";
                }

                string fileNameExtension;

                // Displaying Name from Id, which is coming from reportmodel
                //Added by Rajas on 14 OCT 2016
                string CompanyName = WetosDB.Companies.Where(a => a.CompanyId == ReportModel.CompanyId).Select(a => a.CompanyName).FirstOrDefault();
                string Branchname = WetosDB.Branches.Where(a => a.BranchId == ReportModel.BranchId).Select(a => a.BranchName).FirstOrDefault();
                string DepartmentName = WetosDB.Departments.Where(a => a.DepartmentId == ReportModel.DepartmentId).Select(a => a.DepartmentName).FirstOrDefault();

                ReportParameter ReportParameter1 = new ReportParameter("ReportParameter1", CompanyName); // Company name
                ReportParameter ReportParameter2 = new ReportParameter("ReportParameter2", ReportModel.ReportName); // report name
                ReportParameter ReportParameter3 = new ReportParameter("ReportParameter3", ReportModel.FromDate.ToString()); // from date
                ReportParameter ReportParameter4 = new ReportParameter("ReportParameter4", ReportModel.ToDate.ToString()); // to date
                // blank parameters to be used later as per requirement 
                ReportParameter ReportParameter5 = new ReportParameter("ReportParameter5", "");
                ReportParameter ReportParameter6 = new ReportParameter("ReportParameter6", "");
                ReportParameter ReportParameter7 = new ReportParameter("ReportParameter7", "");
                localReport.SetParameters(new ReportParameter[] { ReportParameter1, ReportParameter2, ReportParameter3, ReportParameter4, ReportParameter5, ReportParameter6, ReportParameter7 });

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
                //PdfFileName = Server.MapPath("~/User_Data/download/") + "Daily Early Departure Report.pdf";
                using (FileStream fs = new FileStream(PdfFileName, FileMode.Create))
                {
                    fs.Write(renderedBytes, 0, renderedBytes.Length);
                }

                // Added by Rajas on 15 MARCH 2017
                if (ReportModel.ReportFormat == 2)
                {
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + "DailyEarlyDepartureReport" + ".xls;");
                    //string PdfUrl = "download/" + "DailyEarlyReport" + ".xls";
                }
                else
                {
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + "DailyEarlyDepartureReport" + ".pdf;");
                    //string PdfUrl = "download/" + "DailyEarlyReport" + ".pdf";
                }

                //ReturnStatus = true;

            }
            catch (System.Exception)
            {
                //throw;
            }

            return PdfFileName;


            // PRINT END
            //---------------------------------------------REPORT RDLC DailyEarlyDepartureReport END-------------------------------------------
        }

        /// <summary>
        /// DailyOverStayReport
        /// </summary>
        /// <returns></returns>
        private string DailyOverStayReport(MonthlyReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 8 FEB 2017
        {
            //bool ReturnStatus = false;
            string PdfFileName = "";

            try
            {

                // ADDED BY MSJ ON 29 DEC 2017 START
                // GET FROM AND TO DATE FROM YEAR AND MONTH ID

                // GET FY from ID
                // Get MOnthNo from Id
                //FinancialYear SelectedAttendenceYear = WetosDB.FinancialYears.Where(a => a.FinancialYearId == ReportModel.FinancialYearId).FirstOrDefault();


                //ADDED IF CONDITION BY SHRADDHA ON 15 JAN 2018 START

                string opt = fc["opt"];
                if (opt == "MonthWise")
                {
                    ReportModel.FromDate = null;
                    ReportModel.ToDate = null;
                }
                else
                {
                    ReportModel.FinancialYearId = 0;
                    ReportModel.MonthId = 0;
                }

                if (ReportModel.FromDate == null && ReportModel.ToDate == null)
                {
                    int SelectedYear = Convert.ToInt32(ReportModel.FinancialYearId); // MODIFIED BY MSJ ON 08 JAN 2018 Convert.ToInt32(SelectedAttendenceYear.FinancialName);
                    int SelectedMonth = Convert.ToInt32(ReportModel.MonthId);

                    ReportModel.FromDate = FirstDayOfMonthFromDateTime(new DateTime(SelectedYear, SelectedMonth, 1)).ToString("yyyy/MM/dd");
                    ReportModel.ToDate = LastDayOfMonthFromDateTime(new DateTime(SelectedYear, SelectedMonth, 1)).ToString("yyyy/MM/dd");
                }
                //ADDED IF CONDITION BY SHRADDHA ON 15 JAN 2018 END

                // ADDED BY MSJ ON 29 DEC 2017 END


                // ------------------------------------------------REPORT RDLC DailyOverStayReport START-------------------------------------------
                //// PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                SqlCommand com1 = new SqlCommand("usp_Rpt_GetTimeCard", con);
                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018
                com1.CommandType = CommandType.StoredProcedure;
                com1.Parameters.Add(new SqlParameter("@Fdate", SqlDbType.DateTime)).Value = ReportModel.FromDate; //new DateTime(2013, 01, 01);
                com1.Parameters.Add(new SqlParameter("@Tdate", SqlDbType.DateTime)).Value = ReportModel.ToDate; //new DateTime(2013, 01, 01);

                string EmployeeString = "";
                int? company = 0;
                int? branch = 0;
                if (ReportModel.CompanyId == 0 && ReportModel.BranchId == 0 && ReportModel.DepartmentId == 0 && ReportModel.EmployeeId == 0)
                {
                    // MODIFIED BY MSJ ON 29 JAN 2018
                    //EmployeeString = null;
                    EmployeeString = (String.Join(",", WetosDB.Employees.Where(a => a.ActiveFlag == null || a.ActiveFlag == true).Select(a => a.EmployeeId).ToArray()));

                }
                else if (ReportModel.BranchId == 0 && ReportModel.DepartmentId == 0 && ReportModel.EmployeeId == 0)
                {
                    company = ReportModel.CompanyId;
                    EmployeeString = (String.Join(",", WetosDB.Employees.Where(a => a.ActiveFlag == null || a.ActiveFlag == true && a.CompanyId == ReportModel.CompanyId).Select(a => a.EmployeeId).ToArray()));
                }
                else if (ReportModel.DepartmentId == 0 && ReportModel.EmployeeId == 0)
                {
                    company = ReportModel.CompanyId;
                    branch = ReportModel.BranchId;
                    EmployeeString = (String.Join(",", WetosDB.Employees.Where(a => a.ActiveFlag == null || a.ActiveFlag == true && a.CompanyId == ReportModel.CompanyId && a.BranchId == ReportModel.BranchId).Select(a => a.EmployeeId).ToArray()));
                }

                else
                {
                    // Modified by Rajas on 8 FEB 2017 START
                    EmployeeString = fc["EmployeeId"];
                    company = ReportModel.CompanyId;
                    branch = ReportModel.BranchId;
                    // Modified by Rajas on 8 FEB 2017 END
                }

                // Branch selection
                if (branch != 0)
                {
                    com1.Parameters.Add(new SqlParameter("@BranchListId", SqlDbType.NVarChar, 500)).Value = branch;
                }
                else
                {
                    com1.Parameters.Add(new SqlParameter("@BranchListId", SqlDbType.NVarChar, 500)).Value = DBNull.Value;
                }

                // company selection
                if (company != 0)
                {
                    com1.Parameters.Add(new SqlParameter("@CompanyListId", SqlDbType.NVarChar, 500)).Value = company;
                }
                else
                {
                    com1.Parameters.Add(new SqlParameter("@CompanyListId", SqlDbType.NVarChar, 500)).Value = DBNull.Value;
                }

                com1.Parameters.Add(new SqlParameter("@Empid", SqlDbType.NVarChar)).Value = EmployeeString; //DBNull.Value;
                //com1.Parameters.Add(new SqlParameter("@DepartmentListLId", SqlDbType.NVarChar)).Value = "184";
                com1.Parameters.Add(new SqlParameter("@BranchListId", SqlDbType.NVarChar, 500)).Value = ReportModel.BranchId; //"1";
                com1.Parameters.Add(new SqlParameter("@CompanyListId", SqlDbType.NVarChar, 500)).Value = ReportModel.CompanyId; //"2";
                com1.Parameters.Add(new SqlParameter("@RoleTypeId", SqlDbType.NVarChar, 500)).Value = DBNull.Value; //"4";
                com1.Parameters.Add(new SqlParameter("@ReportingId", SqlDbType.Int)).Value = DBNull.Value; //"76";
                SqlDataAdapter da1 = new SqlDataAdapter(com1);

                da1.Fill(table5);

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/DailyOverStayReport.rdlc",
                 "DailyOverStay", "~/User_Data/download/", "Daily_Over_Stay_Report_", ref PdfFileName);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

                //ReturnStatus = true;
            }
            catch (System.Exception)
            {
                //throw;
            }

            return PdfFileName;


            // PRINT END
            //---------------------------------------------REPORT RDLC DailyOverStayReport END-------------------------------------------
        }

        /// <summary>
        /// EmployeePerformanceReport
        /// </summary>
        /// <returns></returns>
        private string EmployeePerformanceReport(MonthlyReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 8 FEB 2017
        {
            string PdfFileName = "";

            try
            {
                // ADDED BY MSJ ON 29 DEC 2017 START
                // GET FROM AND TO DATE FROM YEAR AND MONTH ID

                // GET FY from ID
                // Get MOnthNo from Id
                //FinancialYear SelectedAttendenceYear = WetosDB.FinancialYears.Where(a => a.FinancialYearId == ReportModel.FinancialYearId).FirstOrDefault();


                //ADDED IF CONDITION BY SHRADDHA ON 15 JAN 2018 START

                string opt = fc["opt"];
                if (opt == "MonthWise")
                {
                    ReportModel.FromDate = null;
                    ReportModel.ToDate = null;
                }
                else
                {
                    ReportModel.FinancialYearId = 0;
                    ReportModel.MonthId = 0;
                }

                if (ReportModel.FromDate == null && ReportModel.ToDate == null)
                {
                    int SelectedYear = Convert.ToInt32(ReportModel.FinancialYearId); // MODIFIED BY MSJ ON 08 JAN 2018 Convert.ToInt32(SelectedAttendenceYear.FinancialName);
                    int SelectedMonth = Convert.ToInt32(ReportModel.MonthId);

                    ReportModel.FromDate = FirstDayOfMonthFromDateTime(new DateTime(SelectedYear, SelectedMonth, 1)).ToString("yyyy/MM/dd");
                    ReportModel.ToDate = LastDayOfMonthFromDateTime(new DateTime(SelectedYear, SelectedMonth, 1)).ToString("yyyy/MM/dd");
                }
                //ADDED IF CONDITION BY SHRADDHA ON 15 JAN 2018 END

                // ADDED BY MSJ ON 29 DEC 2017 END

                // ------------------------------------------------REPORT RDLC EmployeePerformanceReport START-------------------------------------------
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                SqlCommand com1 = new SqlCommand("usp_Rpt_GetTimeCard", con);
                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018
                com1.CommandType = CommandType.StoredProcedure;
                com1.Parameters.Add(new SqlParameter("@Fdate", SqlDbType.DateTime)).Value = ReportModel.FromDate; //new DateTime(2013, 01, 01);
                com1.Parameters.Add(new SqlParameter("@Tdate", SqlDbType.DateTime)).Value = ReportModel.ToDate; // new DateTime(2013, 01, 01);

                // Added by Rajas to generate report for all active employees on basis of selected parameter
                string EmployeeString = "";
                int? company = 0;
                int? branch = 0;
                if (ReportModel.CompanyId == 0 && ReportModel.BranchId == 0 && ReportModel.DepartmentId == 0 && ReportModel.EmployeeId == 0)
                {
                    //EmployeeString = null;

                    EmployeeString = (String.Join(",", WetosDB.Employees.Where(a => a.ActiveFlag == null || a.ActiveFlag == true).Select(a => a.EmployeeId).ToArray()));

                }
                else if (ReportModel.BranchId == 0 && ReportModel.DepartmentId == 0 && ReportModel.EmployeeId == 0)
                {
                    company = ReportModel.CompanyId;
                    EmployeeString = (String.Join(",", WetosDB.Employees.Where(a => a.ActiveFlag == null || a.ActiveFlag == true && a.CompanyId == ReportModel.CompanyId).Select(a => a.EmployeeId).ToArray()));
                }
                else if (ReportModel.DepartmentId == 0 && ReportModel.EmployeeId == 0)
                {
                    company = ReportModel.CompanyId;
                    branch = ReportModel.BranchId;
                    EmployeeString = (String.Join(",", WetosDB.Employees.Where(a => a.ActiveFlag == null || a.ActiveFlag == true && a.CompanyId == ReportModel.CompanyId && a.BranchId == ReportModel.BranchId).Select(a => a.EmployeeId).ToArray()));
                }
                else
                {
                    // Modified by Rajas on 8 FEB 2017 START
                    EmployeeString = fc["EmployeeId"];
                    company = ReportModel.CompanyId;
                    branch = ReportModel.BranchId;
                    // Modified by Rajas on 8 FEB 2017 END
                }

                com1.Parameters.Add(new SqlParameter("@RoleTypeId", SqlDbType.NVarChar, 500)).Value = "1";
                com1.Parameters.Add(new SqlParameter("@ReportingId", SqlDbType.Int)).Value = DBNull.Value;

                // Branch selection
                if (branch != 0)
                {
                    com1.Parameters.Add(new SqlParameter("@BranchListId", SqlDbType.NVarChar, 500)).Value = branch;
                }
                else
                {
                    com1.Parameters.Add(new SqlParameter("@BranchListId", SqlDbType.NVarChar, 500)).Value = DBNull.Value;
                }

                // company selection
                if (company != 0)
                {
                    com1.Parameters.Add(new SqlParameter("@CompanyListId", SqlDbType.NVarChar, 500)).Value = company;
                }
                else
                {
                    com1.Parameters.Add(new SqlParameter("@CompanyListId", SqlDbType.NVarChar, 500)).Value = DBNull.Value;
                }

                SqlDataAdapter da1 = new SqlDataAdapter(com1);

                da1.Fill(table5);

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/EmployeePerformanceReport.rdlc",
                    "EmployeePerformance", "~/User_Data/download/", "Employee_Performance_Report_", ref PdfFileName);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

                //ReturnStatus = true;
            }
            catch (System.Exception)
            {
                //throw;
            }

            return PdfFileName;


            // PRINT END
            //---------------------------------------------REPORT RDLC EmployeePerformanceReport END-------------------------------------------
        }
        // Added by Rajas on 28 SEP 2016

        // ADDED BY MSJ ON 30 OCT 2017 START -- GENERIC CODE FOR REPORT
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ReportFormat"></param>
        /// <param name="RdlcFileName"></param>
        /// <param name="DataSourceName"></param>
        /// <param name="ReportFolder"></param>
        /// <param name="ReportFileName"></param>
        /// <returns></returns>
        private string GenerateReportMonthly(int? BranchId, int? CompanyId, int? DepartmentId, int ReportFormat, string ReportName, string FromDate, string ToDate,
            DataTable ReportDT, string RdlcFileName, string DataSourceName, string ReportFolder, string ReportFileName, ref string ReportPath, DataTable ReportDTHeader
            , string DataSourceHeader)
        {

            try
            {
                string PdfFileName = string.Empty;

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                ReportModel ReportModelObj = new ReportModel();

                ReportModelObj.BranchId = BranchId;
                ReportModelObj.CompanyId = CompanyId;
                ReportModelObj.DepartmentId = DepartmentId;
                ReportModelObj.ReportFormat = ReportFormat;
                ReportModelObj.ReportName = ReportName;
                ReportModelObj.FromDate = FromDate;
                ReportModelObj.ToDate = ToDate;

                // REPORT CODE START
                Warning[] warnings;
                //string[] streamids;
                string mimeType;
                string encoding;
                //string filenameExtension;
                //string filePath;

                LocalReport localReport = new LocalReport();
                localReport.ReportPath = Server.MapPath(RdlcFileName);
                //ReportDataSource reportDataSource = new ReportDataSource("Customers", Customers.GetAllCustomers());
                localReport.DataSources.Clear();
                localReport.DataSources.Add(new ReportDataSource(DataSourceName, ReportDT));
                localReport.DataSources.Add(new ReportDataSource(DataSourceHeader, ReportDTHeader));


                // Added by Rajas on 4 FEB 2017 START
                int EmpId = Convert.ToInt32(Session["EmployeeNo"]);

                // SELECT Employee code to generate pdf file name according to logged in user
                string EmployeeCode = WetosDB.Employees.Where(a => a.EmployeeId == EmpId).Select(a => a.EmployeeCode).FirstOrDefault();

                localReport.Refresh();

                // Added by Rajas on 15 MARCH 2017
                string reportType = string.Empty;

                string GeneratedFileName = ReportFileName + EmployeeCode + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");

                if (ReportModelObj.ReportFormat == 2)
                {
                    reportType = "Excel";
                    GeneratedFileName = GeneratedFileName + ".xls";
                    PdfFileName = Server.MapPath(ReportFolder + GeneratedFileName);

                    // ADD CLOSED XLS CODE START


                    // USING CLOSEDXML ADDED BY MSJ ON 27 JUNE 2017 START
                    string ABCD = string.Empty;
                    //string ExcelFilePath = Server.MapPath("~/Template/ptw10052 to ptw100075 -GST.xlsx");

                    //string sheetName = "Sheet1";
                    ReportDT.TableName = "Sheet1";

                    using (XLWorkbook Workbook = new XLWorkbook()) //ExcelOutFilePath))
                    {
                        //IXLWorksheet Worksheet = Workbook.Worksheet(sheetName);
                        Workbook.Worksheets.Add(ReportDT);
                        //int NumberOfLastRow = Worksheet.LastRowUsed().RowNumber;
                        //Worksheet.Cell(2, 1).SetValue("Hello World");
                        //Workbook.SaveAs(PdfFileName);

                        // Prepare the response
                        //HttpResponse httpResponse = Response;
                        ///Response.Clear();
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                        // ADDED BY MSJ ON 16 JUNE 2017 START
                        ABCD = "attachment;filename=\"" + GeneratedFileName + "\"";

                        Response.AddHeader("content-disposition", ABCD);
                        //Response.AddHeader("content-disposition", "attachment;filename=\"HelloWorld.xlsx\"");

                        // Flush the workbook to the Response.OutputStream
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            Workbook.SaveAs(memoryStream);
                            memoryStream.WriteTo(Response.OutputStream);
                            memoryStream.Close();
                        }

                        Response.End();
                        ReportPath = PdfFileName;
                    }

                    // ADD CLOSED XLS CODE END
                    return GeneratedFileName;

                }
                else
                {
                    reportType = "PDF";
                    GeneratedFileName = GeneratedFileName + ".pdf";
                    PdfFileName = Server.MapPath(ReportFolder + GeneratedFileName);

                    //} // commnented by MSJ on 17 MAR 2018


                    //string mimeType;
                    //string encoding;
                    string fileNameExtension;

                    // Displaying Name from Id, which is coming from reportmodel
                    //Added by Rajas on 14 OCT 2016

                    WetosDB.Company RptCompany = WetosDB.Companies.Where(a => a.CompanyId == ReportModelObj.CompanyId).FirstOrDefault();

                    string CompanyName = RptCompany == null ? string.Empty : RptCompany.CompanyName;
                    string Address = RptCompany == null ? string.Empty :
                        ((string.IsNullOrEmpty(RptCompany.Address1) == true ? string.Empty : RptCompany.Address1)
                        + (string.IsNullOrEmpty(RptCompany.Address2) == true ? string.Empty : " " + RptCompany.Address2)
                        + (string.IsNullOrEmpty(RptCompany.City) == true ? string.Empty : " " + RptCompany.City));

                    string Branchname = WetosDB.Branches.Where(a => a.BranchId == ReportModelObj.BranchId).Select(a => a.BranchName).FirstOrDefault();
                    string DepartmentName = WetosDB.Departments.Where(a => a.DepartmentId == ReportModelObj.DepartmentId).Select(a => a.DepartmentName).FirstOrDefault();

                    //CODE ADDED TO HANDLE NULL EXCEPTION FOR FROM DATE AND TODATE BY SHRADDHA ON 01 NOV 2017 START
                    string FromDateStr = ReportModelObj.FromDate == null ? string.Empty : ReportModelObj.FromDate.ToString();
                    string ToDateStr = ReportModelObj.ToDate == null ? string.Empty : ReportModelObj.ToDate.ToString();
                    //CODE ADDED TO HANDLE NULL EXCEPTION FOR FROM DATE AND TODATE BY SHRADDHA ON 01 NOV 2017 END

                    //ADDED ADDRESS BY PUSHKAR ON 22 DEC 2017 FOR TAKING LnT Address
                    //string Address = "Gate No. 3, A-9, MIDC, Ahmednagar-414111";


                    //CODE ADDED BY SHRADDHA ON 19 MAR 2018 START

                    //if (ReportModelObj.CompanyId == null || ReportModelObj.CompanyId == 0)
                    //{
                    //    WetosDB.Company CompanyObj = WetosDB.Companies.FirstOrDefault();
                    //    CompanyName = CompanyObj.CompanyName;
                    //    Address = CompanyObj.Address1;
                    //}

                    //CODE ADDED BY SHRADDHA ON 19 MAR 2018 END

                    ReportParameter ReportParameter1 = new ReportParameter("ReportParameter1", CompanyName); // Company name
                    ReportParameter ReportParameter2 = new ReportParameter("ReportParameter2", ReportModelObj.ReportName); // report name
                    ReportParameter ReportParameter3 = new ReportParameter("ReportParameter3", FromDateStr);//ReportModelObj.FromDate.ToString()); //CODE ADDED TO HANDLE NULL EXCEPTION FOR FROM DATE AND TODATE BY SHRADDHA ON 01 NOV 2017 START
                    ReportParameter ReportParameter4 = new ReportParameter("ReportParameter4", ToDateStr);//ReportModelObj.ToDate.ToString()); //CODE ADDED TO HANDLE NULL EXCEPTION FOR FROM DATE AND TODATE BY SHRADDHA ON 01 NOV 2017 START
                    // blank parameters to be used later as per requirement 
                    ReportParameter ReportParameter5 = new ReportParameter("ReportParameter5", Branchname);//ADDED BY PUSHKAR ON 22 DEC 2017 FOR BRANCH
                    ReportParameter ReportParameter6 = new ReportParameter("ReportParameter6", Address);//ADDED BY PUSHKAR ON 22 DEC 2017 FOR ADDRESS
                    ReportParameter ReportParameter7 = new ReportParameter("ReportParameter7", "");
                    localReport.SetParameters(new ReportParameter[] { ReportParameter1, ReportParameter2, ReportParameter3, ReportParameter4, ReportParameter5, ReportParameter6, ReportParameter7 });

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


                    // Added by Rajas on 4 FEB 2017 END

                    //FileStream fs = new FileStream(filePath, FileMode.Create);
                    //PdfFileName = Server.MapPath("~/User_Data/download/") + "Employee Performance Report.pdf";

                    Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

                    using (FileStream fs = new FileStream(PdfFileName, FileMode.Create))
                    {
                        fs.Write(renderedBytes, 0, renderedBytes.Length);
                    }
                    ReportPath = PdfFileName;
                }

                return GeneratedFileName;
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in Generate Report comman function due to :" + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));
                return string.Empty;
            }

            // REPORT CODE END
        }




        // ADDED BY MSJ ON 30 OCT 2017 START -- GENERIC CODE FOR REPORT
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ReportFormat"></param>
        /// <param name="RdlcFileName"></param>
        /// <param name="DataSourceName"></param>
        /// <param name="ReportFolder"></param>
        /// <param name="ReportFileName"></param>
        /// <returns></returns>
        private string GenerateReport(int? BranchId, int? CompanyId, int? DepartmentId, int ReportFormat, string ReportName, string FromDate, string ToDate,
            DataTable ReportDT, string RdlcFileName, string DataSourceName, string ReportFolder, string ReportFileName, ref string ReportPath)
        {
            try
            {
                //if (ReportDT.Rows.Count == 0)
                //{
                //    System.Data.DataColumn newColumn = new System.Data.DataColumn("NOData", typeof(System.String));
                //    newColumn.DefaultValue = "NO Data";
                //    ReportDT.Columns.Add(newColumn);                    
                //}

                string PdfFileName = string.Empty;


                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                ReportModel ReportModelObj = new ReportModel();

                ReportModelObj.BranchId = BranchId;
                ReportModelObj.CompanyId = CompanyId;
                ReportModelObj.DepartmentId = DepartmentId;
                ReportModelObj.ReportFormat = ReportFormat;
                ReportModelObj.ReportName = ReportName;
                ReportModelObj.FromDate = FromDate;
                ReportModelObj.ToDate = ToDate;

                // REPORT CODE START
                Warning[] warnings;
                //string[] streamids;
                string mimeType;
                string encoding;
                //string filenameExtension;
                //string filePath;

                LocalReport localReport = new LocalReport();
                localReport.ReportPath = Server.MapPath(RdlcFileName);
                //ReportDataSource reportDataSource = new ReportDataSource("Customers", Customers.GetAllCustomers());
                localReport.DataSources.Clear();
                localReport.DataSources.Add(new ReportDataSource(DataSourceName, ReportDT));
                //localReport.DataSources.Add(new ReportDataSource(DataSourceHeader, ReportDTHeader));


                // Added by Rajas on 4 FEB 2017 START
                int EmpId = Convert.ToInt32(Session["EmployeeNo"]);

                // SELECT Employee code to generate pdf file name according to logged in user
                string EmployeeCode = WetosDB.Employees.Where(a => a.EmployeeId == EmpId).Select(a => a.EmployeeCode).FirstOrDefault();

                localReport.Refresh();

                // Added by Rajas on 15 MARCH 2017
                string reportType = string.Empty;

                string GeneratedFileName = ReportFileName + EmployeeCode + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");

                int ExcelFormatFlag = 0;
                GlobalSetting GlobalSettingExcel = WetosDB.GlobalSettings.Where(a => a.SettingText == "ExcelFormat").FirstOrDefault();
                if (GlobalSettingExcel != null)
                {
                    ExcelFormatFlag = Convert.ToInt32(GlobalSettingExcel.SettingValue);
                }


                if (ReportModelObj.ReportFormat == 2 && ExcelFormatFlag == 1)
                {
                    reportType = "Excel";
                    GeneratedFileName = GeneratedFileName + ".xls";
                    PdfFileName = Server.MapPath(ReportFolder + GeneratedFileName);

                    // ADD CLOSED XLS CODE START


                    // USING CLOSEDXML ADDED BY MSJ ON 27 JUNE 2017 START
                    string ABCD = string.Empty;
                    //string ExcelFilePath = Server.MapPath("~/Template/ptw10052 to ptw100075 -GST.xlsx");

                    //string sheetName = "Sheet1";
                    ReportDT.TableName = "Sheet1";

                    using (XLWorkbook Workbook = new XLWorkbook()) //ExcelOutFilePath))
                    {
                        //IXLWorksheet Worksheet = Workbook.Worksheet(sheetName);
                        Workbook.Worksheets.Add(ReportDT);
                        //int NumberOfLastRow = Worksheet.LastRowUsed().RowNumber;
                        //Worksheet.Cell(2, 1).SetValue("Hello World");
                        //Workbook.SaveAs(PdfFileName);

                        // Prepare the response
                        //HttpResponse httpResponse = Response;
                       // Response.Clear();
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                        // ADDED BY MSJ ON 16 JUNE 2017 START
                        ABCD = "attachment;filename=\"" + GeneratedFileName + "\"";

                        //Response.AddHeader("content-disposition", ABCD);
                        //Response.AddHeader("content-disposition", "attachment;filename=\"HelloWorld.xlsx\"");

                        // Flush the workbook to the Response.OutputStream
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            Workbook.SaveAs(memoryStream);
                            memoryStream.WriteTo(Response.OutputStream);
                            memoryStream.Close();
                        }

                        Response.End();
                        ReportPath = PdfFileName;
                    }

                    // ADD CLOSED XLS CODE END
                    return GeneratedFileName;

                }
                else
                {
                    if (ReportModelObj.ReportFormat == 2)
                    {
                        reportType = "Excel";
                        GeneratedFileName = GeneratedFileName + ".xls";
                        PdfFileName = Server.MapPath(ReportFolder + GeneratedFileName);
                    }

                    else
                    {

                        reportType = "PDF";
                        GeneratedFileName = GeneratedFileName + ".pdf";
                        PdfFileName = Server.MapPath(ReportFolder + GeneratedFileName);
                    }

                    //} // commnented by MSJ on 17 MAR 2018


                    //string mimeType;
                    //string encoding;
                    string fileNameExtension;

                    // Displaying Name from Id, which is coming from reportmodel
                    //Added by Rajas on 14 OCT 2016
                    //string CompanyName = WetosDB.Companies.Where(a => a.CompanyId == ReportModelObj.CompanyId).Select(a => a.CompanyName).FirstOrDefault();

                    WetosDB.Company RptCompany = WetosDB.Companies.Where(a => a.CompanyId == ReportModelObj.CompanyId).FirstOrDefault();

                    string CompanyName = RptCompany == null ? string.Empty : RptCompany.CompanyName;
                    string Address = RptCompany == null ? string.Empty :
                        ((string.IsNullOrEmpty(RptCompany.Address1) == true ? string.Empty : RptCompany.Address1)
                        + (string.IsNullOrEmpty(RptCompany.Address2) == true ? string.Empty : " " + RptCompany.Address2)
                        + (string.IsNullOrEmpty(RptCompany.City) == true ? string.Empty : " " + RptCompany.City));

                    string Branchname = WetosDB.Branches.Where(a => a.BranchId == ReportModelObj.BranchId).Select(a => a.BranchName).FirstOrDefault();
                    string DepartmentName = WetosDB.Departments.Where(a => a.DepartmentId == ReportModelObj.DepartmentId).Select(a => a.DepartmentName).FirstOrDefault();

                    //CODE ADDED TO HANDLE NULL EXCEPTION FOR FROM DATE AND TODATE BY SHRADDHA ON 01 NOV 2017 START
                    string FromDateStr = ReportModelObj.FromDate == null ? string.Empty : ReportModelObj.FromDate.ToString();
                    string ToDateStr = ReportModelObj.ToDate == null ? string.Empty : ReportModelObj.ToDate.ToString();
                    //CODE ADDED TO HANDLE NULL EXCEPTION FOR FROM DATE AND TODATE BY SHRADDHA ON 01 NOV 2017 END

                    //ADDED ADDRESS BY PUSHKAR ON 22 DEC 2017 FOR TAKING LnT Address
                    //string Address = "Gate No. 3, A-9, MIDC, Ahmednagar-414111";
                    //string Address = WetosDB.Companies.Where(a => a.CompanyId == ReportModelObj.CompanyId).Select(a => a.Address1).FirstOrDefault();

                    //CODE ADDED BY SHRADDHA ON 19 MAR 2018 START
                    //if (ReportModelObj.CompanyId == null || ReportModelObj.CompanyId == 0)
                    //{
                    //    WetosDB.Company CompanyObj = WetosDB.Companies.FirstOrDefault();
                    //    CompanyName = CompanyObj.CompanyName;
                    //    Address = CompanyObj.Address1;
                    //}
                    //CODE ADDED BY SHRADDHA ON 19 MAR 2018 END

                    ReportParameter ReportParameter1 = new ReportParameter("ReportParameter1", CompanyName); // Company name
                    ReportParameter ReportParameter2 = new ReportParameter("ReportParameter2", ReportModelObj.ReportName); // report name
                    ReportParameter ReportParameter3 = new ReportParameter("ReportParameter3", FromDateStr);//ReportModelObj.FromDate.ToString()); //CODE ADDED TO HANDLE NULL EXCEPTION FOR FROM DATE AND TODATE BY SHRADDHA ON 01 NOV 2017 START
                    ReportParameter ReportParameter4 = new ReportParameter("ReportParameter4", ToDateStr);//ReportModelObj.ToDate.ToString()); //CODE ADDED TO HANDLE NULL EXCEPTION FOR FROM DATE AND TODATE BY SHRADDHA ON 01 NOV 2017 START
                    // blank parameters to be used later as per requirement 
                    ReportParameter ReportParameter5 = new ReportParameter("ReportParameter5", Branchname);//ADDED BY PUSHKAR ON 22 DEC 2017 FOR BRANCH
                    ReportParameter ReportParameter6 = new ReportParameter("ReportParameter6", Address);//ADDED BY PUSHKAR ON 22 DEC 2017 FOR ADDRESS
                    ReportParameter ReportParameter7 = new ReportParameter("ReportParameter7", "");
                    localReport.SetParameters(new ReportParameter[] { ReportParameter1, ReportParameter2, ReportParameter3, ReportParameter4, ReportParameter5, ReportParameter6, ReportParameter7 });

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


                    // Added by Rajas on 4 FEB 2017 END

                    //FileStream fs = new FileStream(filePath, FileMode.Create);
                    //PdfFileName = Server.MapPath("~/User_Data/download/") + "Employee Performance Report.pdf";
                    Response.Clear();
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

                    using (FileStream fs = new FileStream(PdfFileName, FileMode.Create))
                    {
                        fs.Write(renderedBytes, 0, renderedBytes.Length);
                    }
                    ReportPath = PdfFileName;
                }

                return GeneratedFileName;
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in Generate Report comman function due to :" + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));
                return string.Empty;
            }

            // REPORT CODE END
        }

        // ADDED BY MSJ ON 30 OCT 2017 END
        #endregion

        #region EXPORT TO EXCEL AND PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS

        public void ExportToExcelAndPDFEnable()
        {
            //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS START
            ViewBag.ExportToExcelEnabledObj = 1;
            ViewBag.ExportToPDFEnabledObj = 1;

            //GlobalSetting ExportToExcelEnabledObj = WetosDB.GlobalSettings.Where(a => a.SettingText == "ExportToExcelEnabled").FirstOrDefault();
            //GlobalSetting ExportToPDFEnabledObj = WetosDB.GlobalSettings.Where(a => a.SettingText == "ExportToPDFEnabled").FirstOrDefault();

            //ABOVE TWO LINES COMMENTED BY SHALAKA ON 13TH DEC 2017 AND ADDED BELOW TO LINES --- START
            GlobalSetting ExportToExcelEnabledObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.ExportToExcelEnabled).FirstOrDefault();
            GlobalSetting ExportToPDFEnabledObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.ExportToPDFEnabled).FirstOrDefault();
            //--- END

            if (ExportToExcelEnabledObj != null)
            {
                if (ExportToExcelEnabledObj.SettingValue == "1")
                {
                    ViewBag.ExportToExcelEnabledObj = 1;
                }
                else if (ExportToExcelEnabledObj.SettingValue == "0")
                {
                    ViewBag.ExportToExcelEnabledObj = 0;
                }
            }
            if (ExportToPDFEnabledObj != null)
            {
                if (ExportToPDFEnabledObj.SettingValue == "1")
                {
                    ViewBag.ExportToPDFEnabledObj = 1;
                }
                else if (ExportToPDFEnabledObj.SettingValue == "0")
                {
                    ViewBag.ExportToPDFEnabledObj = 0;
                }
            }
            //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS END
        }

        #endregion

        #region LNT REPORTS

        // 1 Monthly Performance Report - TimeCardReport
        // 2 DailyLong Absnteeism Report
        // 3 Employees Detail Leave Report
        // 4 Monthly Attendance Report
        // 5 Daily Multiple Punches Report
        // 6 Monthly Left Report
        // 7 Monthly Addition List Report
        // 8 Monthly Salary Output Report
        // 9 Yearly Perfomance Report MaleWise
        // 10 Yearly Encashment Leave Report
        // 12 Yearly Leave Report

        /// <summary>
        /// LNT Reports Index
        /// </summary>
        /// <returns></returns>
        public ActionResult LNTReportsIndex()
        {
            MonthlyReportsModel MonthlyReportsObj = new MonthlyReportsModel();

            try
            {
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);

                Employee LoggedInEmployee = WetosDB.Employees.Where(a => a.EmployeeId == EmployeeId).FirstOrDefault();

                UserRole ChechUserRole = WetosDB.UserRoles.Where(a => a.EmployeeId == EmployeeId).FirstOrDefault();

                //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO HANDLE NULL OBJECT REFERECE ERROR FOR LoggedInEmployee OBJ AND ChechUserRole OBJ START
                if (LoggedInEmployee == null)
                {
                    LoggedInEmployee = new Employee();
                }
                if (ChechUserRole == null)
                {
                    ChechUserRole = new UserRole();
                }
                //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO HANDLE NULL OBJECT REFERECE ERROR FOR LoggedInEmployee OBJ AND ChechUserRole OBJ END

                MonthlyReportsObj.CompanyId = LoggedInEmployee.CompanyId;
                MonthlyReportsObj.BranchId = LoggedInEmployee.BranchId;
                MonthlyReportsObj.DepartmentId = LoggedInEmployee.DepartmentId;
                MonthlyReportsObj.EmployeeId = LoggedInEmployee.EmployeeId;
                MonthlyReportsObj.UserId = ChechUserRole.RoleTypeId;


                #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //var EmployeeNameList = WetosDB.Employees.Where(a => a.ActiveFlag == null || a.ActiveFlag == true
                //&& a.CompanyId == LoggedInEmployee.CompanyId && a.BranchId == LoggedInEmployee.BranchId
                //&& a.EmployeeReportingId == LoggedInEmployee.EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
                var EmployeeNameList = WetosDB.SP_VwActiveEmployee(EmployeeId).Where(a => a.ActiveFlag == null || a.ActiveFlag == true
                       && a.CompanyId == LoggedInEmployee.CompanyId && a.BranchId == LoggedInEmployee.BranchId
                       && a.EmployeeReportingId == LoggedInEmployee.EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
                #endregion

                ViewBag.EmployeeList = new SelectList(EmployeeNameList, "EmployeeId", "EmployeeName").ToList();

            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in monthly reports selection due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));
            }

            //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable START
            ExportToExcelAndPDFEnable();
            //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable END

            return View(MonthlyReportsObj);
        }

        /// <summary>
        /// LNTReportsIndex - Validation added for from date, to date and branch        
        /// </summary>
        /// <param name="ReportModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LNTReportsIndex(MonthlyReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 7 FEB 2017
        {
            //if (ModelState.IsValid)
            //{
            try
            {
                string PdfFileName = "";

                ReportModel.ReportFormat = Convert.ToInt32(fc["Export"]);

                switch (ReportModel.RaportSelected)
                {
                    case "1": // 1 Monthly Performance Report - TimeCardReport
                        string Report1 = "Monthly Performance Report";
                        ReportModel.ReportName = Report1;
                        PdfFileName = MonthlyPerformanceReport(ReportModel, fc);
                        break;
                    //case "2": // 2 DailyLong Absnteeism Report
                    //    string Report2 = "DailyLong Absnteeism Report";
                    //    ReportModel.ReportName = Report2;
                    //    PdfFileName = DailyLongAbsnteeismReport(ReportModel, fc);
                    //    break;
                    case "3":  // 3 Employees Detail Leave Report
                        string Report3 = "Employees Detail Leave Report";  // Updated by Rajas on 23 MAY 2017
                        ReportModel.ReportName = Report3;
                        PdfFileName = EmployeesDetailLeaveReport(ReportModel, fc);
                        break;
                    case "4":  // 4 Monthly Attendance Report
                        string Report4 = "Monthly Attendance Report";
                        ReportModel.ReportName = Report4;
                        PdfFileName = MonthlyAttendanceReport(ReportModel, fc); // Updated by Rajas on 7 FEB 2017
                        break;
                    //case "5":  // 5 Daily Multiple Punches Report
                    //    string Report5 = "Daily Multiple Punches Report";
                    //    ReportModel.ReportName = Report5;
                    //    PdfFileName = DailyMultiplePunchesReport(ReportModel, fc);
                    //    break;
                    case "6": // 6 Monthly Left Report
                        string Report6 = "Monthly Left Report";
                        ReportModel.ReportName = Report6;
                        PdfFileName = MonthlyLeftReport(ReportModel, fc); //L&T report done
                        break;
                    case "7": // 7 Monthly Addition List Report
                        string Report7 = "Monthly Addition List Report";
                        ReportModel.ReportName = Report7;
                        PdfFileName = MonthlyAdditionListReport(ReportModel, fc); //L&T report done
                        break;
                    case "8": // 8 Monthly Salary Output Report
                        string Report8 = "Monthly Salary Output Report";
                        ReportModel.ReportName = Report8;
                        PdfFileName = MonthlySalaryOutputReport(ReportModel, fc);
                        break;
                    case "9": // 9 Yearly Perfomance Report MaleWise
                        string Report9 = "Yearly Perfomance Report MaleWise";
                        ReportModel.ReportName = Report9;
                        PdfFileName = YearlyPerfomanceReportMaleWise(ReportModel, fc);
                        break;
                    case "10": // 10 Yearly Encashment Leave Report
                        string Report10 = "Yearly Encashment Leave Report";
                        ReportModel.ReportName = Report10;
                        PdfFileName = YearlyEncashmentLeaveReport(ReportModel, fc);
                        break;
                    case "11":  // 11 Yearly Leave Report
                        string Report11 = "Yearly Leave Report";
                        ReportModel.ReportName = Report11;
                        PdfFileName = YearlyLeaveReport(ReportModel, fc);
                        break;
                    default:
                        break;
                }

                // Added by Rajas on 23 DEC 2016 for handling null or empty PdfFileName START
                if (string.IsNullOrEmpty(PdfFileName))
                {
                    PopulateDropDown();

                    ModelState.AddModelError("Error", "Please try again");

                    //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable START
                    ExportToExcelAndPDFEnable();
                    //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable END

                    return View(ReportModel);
                }
                else
                {
                    // Added by Rajas on 14 MARCh 2017
                    if (ReportModel.ReportFormat == 2)  // excel format
                    {
                        return File(PdfFileName, "application/xls");
                    }
                    else // PDF format
                    {
                        return File(PdfFileName, "application/pdf");
                    }

                }
                // Added by Rajas on 23 DEC 2016 for handling null or empty PdfFileName END

            }
            catch (System.Exception ex)
            {
                PopulateDropDown();

                //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable START
                ExportToExcelAndPDFEnable();
                //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable END

                AddAuditTrail(ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                return View(ReportModel);
            }
        }

        /// <summary>
        /// // 1 Monthly Performance Report - TimeCardReport
        /// </summary>
        /// <returns></returns>       
        private string MonthlyPerformanceReport(MonthlyReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 7 FEB 2017
        {
            //bool ReturnStatus = false;
            string PdfFileName = "";

            try
            {
                //// PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                SqlCommand com1 = new SqlCommand("SP_MonthlyPerformanceReport", con);
                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018
                com1.CommandType = CommandType.StoredProcedure;
                com1.Parameters.Add(new SqlParameter("@Fdate", SqlDbType.DateTime)).Value = ReportModel.FromDate; //new DateTime(2013, 01, 01);
                com1.Parameters.Add(new SqlParameter("@Tdate", SqlDbType.DateTime)).Value = ReportModel.ToDate; //new DateTime(2013, 01, 01);
                com1.Parameters.Add(new SqlParameter("@EmpidList", SqlDbType.NVarChar)).Value = ReportModel.EmployeeString;

                SqlDataAdapter da1 = new SqlDataAdapter(com1);

                da1.Fill(table5);

                if (ReportModel.ReportFormat == 2)
                {


                    table5.Columns.Remove("EMPLOYEETYPEID");
                    table5.Columns.Remove("LateCount");
                    table5.Columns.Remove("TotalDaysInYear");
                    table5.Columns.Remove("EmployeeId");

                }

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/MonthlyPerformanceReport.rdlc",
                    "TimeCardDataset", "~/User_Data/download/", "Monthly_Performance_Report", ref PdfFileName);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

            }
            catch (System.Exception ex)
            {
                AddAuditTrail("TimeCardReport " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));
            }

            return PdfFileName;


            // PRINT END
            //---------------------------------------------REPORT RDLC TimeCardReport END-------------------------------------------
        }

        private string MonthlyTotalLateEarly(MonthlyReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 7 FEB 2017
        {
            //bool ReturnStatus = false;
            string PdfFileName = "";

            try
            {
                //// PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                SqlCommand com1 = new SqlCommand("SP_TotalMonthlylateEarly", con);
                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018
                com1.CommandType = CommandType.StoredProcedure;
                com1.Parameters.Add(new SqlParameter("@Fdate", SqlDbType.DateTime)).Value = ReportModel.FromDate; //new DateTime(2013, 01, 01);
                com1.Parameters.Add(new SqlParameter("@Tdate", SqlDbType.DateTime)).Value = ReportModel.ToDate; //new DateTime(2013, 01, 01);
                com1.Parameters.Add(new SqlParameter("@EmpidList", SqlDbType.NVarChar)).Value = ReportModel.EmployeeString;

                SqlDataAdapter da1 = new SqlDataAdapter(com1);

                da1.Fill(table5);

                if (ReportModel.ReportFormat == 2)
                {


                }

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/MonthlyPerformanceReport.rdlc",
                    "TimeCardDataset", "~/User_Data/download/", "Monthly_Total_LateEarly_Report", ref PdfFileName);

                //Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

            }
            catch (System.Exception ex)
            {
                AddAuditTrail("TimeCardReport " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));
            }

            return PdfFileName;


            // PRINT END
            //---------------------------------------------REPORT RDLC TimeCardReport END-------------------------------------------
        }



        private string MonthlyLOPForEffDateReport(MonthlyReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 7 FEB 2017
        {
            //bool ReturnStatus = false;
            string PdfFileName = "";

            try
            {
                //// PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                SqlCommand com1 = new SqlCommand("SP_EffectiveDateList_LossOfPay", con);
                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018
                com1.CommandType = CommandType.StoredProcedure;
                com1.Parameters.Add(new SqlParameter("@Fdate", SqlDbType.DateTime)).Value = ReportModel.FromDate; //new DateTime(2013, 01, 01);
                com1.Parameters.Add(new SqlParameter("@Tdate", SqlDbType.DateTime)).Value = ReportModel.ToDate; //new DateTime(2013, 01, 01);
                com1.Parameters.Add(new SqlParameter("@EmpidList", SqlDbType.NVarChar)).Value = ReportModel.EmployeeString;

                SqlDataAdapter da1 = new SqlDataAdapter(com1);

                da1.Fill(table5);

                if (ReportModel.ReportFormat == 2)
                {



                }

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/MonthlyPerformanceReport.rdlc",
                    "TimeCardDataset", "~/User_Data/download/", "Monthly_LOPForEffDate_Report", ref PdfFileName);

                //Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

            }
            catch (System.Exception ex)
            {
                AddAuditTrail("TimeCardReport " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));
            }

            return PdfFileName;


            // PRINT END
            //---------------------------------------------REPORT RDLC TimeCardReport END-------------------------------------------
        }

        //----------ADDED BY PUSHKAR FOR OVERTIME REPORT 

        private string MonthlyOverTimeReport(MonthlyReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 7 FEB 2017
        {
            //bool ReturnStatus = false;
            string PdfFileName = "";

            try
            {
                // ------------------------------------------------REPORT RDLC TimeCardReport START-------------------------------------------
                //// PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                SqlCommand com1 = new SqlCommand("SP_MonthlyOTReport_Customized", con);
                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018
                com1.CommandType = CommandType.StoredProcedure;
                com1.Parameters.Add(new SqlParameter("@Fdate", SqlDbType.DateTime)).Value = ReportModel.FromDate; //new DateTime(2013, 01, 01);
                com1.Parameters.Add(new SqlParameter("@Tdate", SqlDbType.DateTime)).Value = ReportModel.ToDate; //new DateTime(2013, 01, 01);
                com1.Parameters.Add(new SqlParameter("@EmpidList", SqlDbType.NVarChar)).Value = ReportModel.EmployeeString;

                SqlDataAdapter da1 = new SqlDataAdapter(com1);

                da1.Fill(table5);

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/MonthlyOTReport.rdlc",
                    "TimeCardDataset", "~/User_Data/download/", "Monthly_OverTime_Report", ref PdfFileName);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Monthly OT Report " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));
            }

            return PdfFileName;


            // PRINT END
            //---------------------------------------------REPORT RDLC TimeCardReport END-------------------------------------------
        }

        //----------------------------------------------


        //ADDED BY PUSHKAR ON 26 MAY 2020 FOR MONTHLY OT SUMMARY REPORT START

        private string MonthlyOTSummaryReport(MonthlyReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 7 FEB 2017
        {
            //bool ReturnStatus = false;
            string PdfFileName = "";

            try
            {
                // ------------------------------------------------REPORT RDLC TimeCardReport START-------------------------------------------
                //// PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                SqlCommand com1 = new SqlCommand("SP_MonthlyOTSummary", con);
                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018
                com1.CommandType = CommandType.StoredProcedure;
                com1.Parameters.Add(new SqlParameter("@Fdate", SqlDbType.DateTime)).Value = ReportModel.FromDate; //new DateTime(2013, 01, 01);
                com1.Parameters.Add(new SqlParameter("@Tdate", SqlDbType.DateTime)).Value = ReportModel.ToDate; //new DateTime(2013, 01, 01);
                com1.Parameters.Add(new SqlParameter("@EmpidList", SqlDbType.NVarChar)).Value = ReportModel.EmployeeString;

                SqlDataAdapter da1 = new SqlDataAdapter(com1);

                da1.Fill(table5);

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/MonthlyOTSummaryReport.rdlc",
                    "AttendanceSummary", "~/User_Data/download/", "Monthly_OTSummary_Report", ref PdfFileName);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Monthly OT Summary Report " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));
            }

            return PdfFileName;


            // PRINT END
            //---------------------------------------------REPORT RDLC TimeCardReport END-------------------------------------------
        }

        //ADDED BY PUSHKAR ON 26 MAY 2020 FOR MONTHLY OT SUMMARY REPORT START
        //################# ADDED BY PUSHKAR ON 23 FEB 2018

        private string MonthlyLateEarlyReport(MonthlyReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 7 FEB 2017
        {
            //bool ReturnStatus = false;
            string PdfFileName = "";

            try
            {
                // ------------------------------------------------REPORT RDLC TimeCardReport START-------------------------------------------
                //// PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                SqlCommand com1 = new SqlCommand("SP_MonthlyLateEarlyReport", con);
                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018
                com1.CommandType = CommandType.StoredProcedure;
                com1.Parameters.Add(new SqlParameter("@Fdate", SqlDbType.DateTime)).Value = ReportModel.FromDate; //new DateTime(2013, 01, 01);
                com1.Parameters.Add(new SqlParameter("@Tdate", SqlDbType.DateTime)).Value = ReportModel.ToDate; //new DateTime(2013, 01, 01);
                com1.Parameters.Add(new SqlParameter("@EmpidList", SqlDbType.NVarChar)).Value = ReportModel.EmployeeString;


                SqlDataAdapter da1 = new SqlDataAdapter(com1);

                da1.Fill(table5);

                if (ReportModel.ReportFormat == 2)
                {
                    table5.Columns.Remove("CompanyId");
                    table5.Columns.Remove("BranchId");
                    table5.Columns.Remove("DepartmentId");
                    table5.Columns.Remove("DesignationId");
                    table5.Columns.Remove("EmployeeId");
                }

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/MonthlyLateEarlyReport.rdlc",
                    "TimeCardDataset", "~/User_Data/download/", "Monthly_LateEarly_Report", ref PdfFileName);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Monthly Late Early " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));
            }

            return PdfFileName;


            // PRINT END
            //---------------------------------------------REPORT RDLC TimeCardReport END-------------------------------------------
        }

        //################# ADDED BY PUSHKAR ON 23 FEB 2018

        /// <summary>
        /// usp_GetEmployeeforAbsentismReport // 2 DailyLong Absnteeism Report
        /// AbsenteeismReport
        /// </summary>
        /// <returns></returns>
        private string DailyLongAbsnteeismReport(DailyReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 8 FEB 2017
        {
            //bool ReturnStatus = false;

            string PdfFileName = "";

            try
            {
                // ---------------------------------------REPORT RDLC AbsenteeismReport START-----------------------------------------
                // PRINT START
                DataTable table5 = new DataTable();

                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                SqlCommand com1 = new SqlCommand("SP_DailyAbsenteeismReport", con);
                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018
                com1.CommandType = CommandType.StoredProcedure;

                com1.Parameters.Add(new SqlParameter("@Fdate", SqlDbType.DateTime)).Value = ReportModel.FromDate; //new DateTime(2013, 01, 01);
                com1.Parameters.Add(new SqlParameter("@Tdate", SqlDbType.DateTime)).Value = ReportModel.FromDate; //new DateTime(2013, 01, 31);
                com1.Parameters.Add(new SqlParameter("@NoOfDays", SqlDbType.Int)).Value = 1;
                com1.Parameters.Add(new SqlParameter("@EmpidList", SqlDbType.NVarChar)).Value = ReportModel.EmployeeString; //DBNull.Value; //"1200, 1195, 1205";  //"1504, 1508" ;
                SqlDataAdapter da1 = new SqlDataAdapter(com1);
                com1.CommandTimeout = 3000;

                da1.Fill(table5);

                if (ReportModel.ReportFormat == 2)
                {
                    table5.Columns.Remove("CompanyIdT");
                    table5.Columns.Remove("DepartmentIdT");
                    table5.Columns.Remove("BranchIdT");
                    table5.Columns.Remove("DesignationIdT");
                    table5.Columns.Remove("EmployeeIdT");

                }

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/DailyLongAbsenteeismReport.rdlc",
                 "AbsenteeismReport", "~/User_Data/download/", "DailyLong_Absenteeism_Report", ref PdfFileName);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

            }
            catch (System.Exception)
            {
                //throw;
            }

            return PdfFileName;

            // PRINT END
            //---------------------------------------------REPORT RDLC AbsenteeismReport END-------------------------------------------
        }

        // 3 Employees Detail Leave Report
        /// <summary>
        /// LeaveApplicationReport
        /// </summary>
        /// <returns></returns>
        private string EmployeesDetailLeaveReport(MonthlyReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 8 FEB 2017
        {
            //bool ReturnStatus = false;
            string PdfFileName = "";

            try
            {
                // ---------------------------------------REPORT RDLC LeaveApplicationReport START-----------------------------------------
                // PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                //SqlCommand com1 = new SqlCommand("usp_Rpt_EmpLeaveDetails", con);
                SqlCommand com1 = new SqlCommand("SP_EmployeesDetailLeaveReport", con);
                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018
                com1.CommandType = CommandType.StoredProcedure;
                com1.Parameters.Add(new SqlParameter("@EmpidList", SqlDbType.NVarChar)).Value = ReportModel.EmployeeString; //DBNull.Value; //"1503";

                string FinNameEx = ReportModel.FinancialYearId.ToString(); //MODIFIED BY PUSHKAR ON 15 JAN 2018

                com1.Parameters.Add(new SqlParameter("@FinancialName", SqlDbType.NVarChar)).Value = FinNameEx;//MODIFIED BY PUSHKAR ON 15 JAN 2018 

                // PLEASE DO NOT DELETE THIS REGION, ADDED BY RAJAS ON 9 FEB 2017
                #region NOTE
                ///ReportModel.Status has to be updated in case of Eviska as Status is NVarChar field as S,P and R
                ///Status should be saved as S,P, R not as StatusId based or Sanctioned, Pending or Rejected
                #endregion

                // Updated by Rajas on 9 FEB 2017, Removed hardcode S,P,R status in place used StatusId and SqlDbType.NVarChar replaced with SqlDbType.Int
                //com1.Parameters.Add(new SqlParameter("@Status", SqlDbType.Int)).Value = ReportModel.Status;
                //com1.Parameters.Add(new SqlParameter("@Status", SqlDbType.Int)).Value = 2; //COMMENTED BY PUSHKAR ON 24 JAN 2018 BY PUSHKAR

                SqlDataAdapter da1 = new SqlDataAdapter(com1);

                da1.Fill(table5);

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/EmployeesDetailLeaveReport.rdlc",
                 "LeaveApplication", "~/User_Data/download/", "EmployeesDetail_Leave_Report", ref PdfFileName);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

            }
            catch (System.Exception)
            {
            }

            return PdfFileName;

            // PRINT END
            //---------------------------------------------REPORT RDLC LeaveApplicationReport END-------------------------------------------
        }

        // 4 Monthly Attendance Report
        /// <summary>
        /// MonthlyMusterReport
        /// </summary>
        /// <returns></returns>
        private string MonthlyAttendanceReport(MonthlyReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 8 FEB 2017
        {
            //bool ReturnStatus = false;
            string PdfFilename = "";
            DataTable table5 = new DataTable();
            DataTable tableH = new DataTable();
            try
            {
                // ---------------------------------------REPORT RDLC MonthlyMusterReport START-----------------------------------------
                // PRINT START


                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                //SqlCommand com1 = new SqlCommand("usp_Report_GetMonthlyMusterPart2", con);
                SqlCommand com1 = new SqlCommand("SP_MonthlyAttendanceReportNew", con);
                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018
                com1.CommandType = CommandType.StoredProcedure;
                com1.Parameters.Add(new SqlParameter("@Fdate", SqlDbType.DateTime)).Value = ReportModel.FromDate; //new DateTime(2013, 01, 01);
                com1.Parameters.Add(new SqlParameter("@Tdate", SqlDbType.DateTime)).Value = ReportModel.ToDate; //new DateTime(2013, 01, 02);
                com1.Parameters.Add(new SqlParameter("@EmpidList", SqlDbType.NVarChar)).Value = ReportModel.EmployeeString;

                SqlCommand com2 = new SqlCommand("SP_MonthlyReportHeader", con);
                com2.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018
                com2.CommandType = CommandType.StoredProcedure;
                com2.Parameters.Add(new SqlParameter("@start_date", SqlDbType.DateTime)).Value = ReportModel.FromDate; //new DateTime(2013, 01, 01);
                com2.Parameters.Add(new SqlParameter("@end_date", SqlDbType.DateTime)).Value = ReportModel.ToDate; //new DateTime(2013, 01, 02);

                //com1.Parameters.Add(new SqlParameter("@BranchListId", SqlDbType.NVarChar, 500)).Value = "1,3,7";
                //com1.Parameters.Add(new SqlParameter("@CompanyListId", SqlDbType.NVarChar, 500)).Value = "2";
                //com1.Parameters.Add(new SqlParameter("@RoleTypeId", SqlDbType.NVarChar, 500)).Value = "4";
                //com1.Parameters.Add(new SqlParameter("@ReportingId", SqlDbType.Int)).Value = "76";
                SqlDataAdapter da1 = new SqlDataAdapter(com1);
                SqlDataAdapter da2 = new SqlDataAdapter(com2);

                da1.Fill(table5);
                da2.Fill(tableH);

                if (ReportModel.ReportFormat == 2)
                {
                    table5.Columns.Remove("CompanyIdT");
                    table5.Columns.Remove("DepartmentIdT");
                    table5.Columns.Remove("BranchIdT");
                    table5.Columns.Remove("DesignationIdT");
                    table5.Columns.Remove("EmployeeIdT");
                    table5.Columns.Remove("StatusT");
                    table5.Columns.Remove("TranDateT");

                    table5.Columns.Remove("ComapanyNameT");
                    table5.Columns.Remove("BranchNameT");
                    table5.Columns.Remove("DepartmentNameT");
                    table5.Columns.Remove("DesignationNameT");
                    table5.Columns.Remove("CL");
                    table5.Columns.Remove("SL");
                    table5.Columns.Remove("PL");
                    table5.Columns.Remove("OD");
                    table5.Columns.Remove("CO");

                    table5.Columns["EmployeeCodeT"].ColumnName = "Employee Code";
                    table5.Columns["EmployeeNameT"].ColumnName = "Employee Name";

                    table5.Columns["PresentT"].ColumnName = "P";
                    table5.Columns["AbsentT"].ColumnName = "A";
                    table5.Columns["PaidHolidayT"].ColumnName = "PH";
                    table5.Columns["WeeklyOffT"].ColumnName = "W";
                    table5.Columns["TotalPaidDaysT"].ColumnName = "Total";


                    int ColumnCount = tableH.Columns.Count;

                    for (int i = 1; i <= ColumnCount; i++)
                    {
                        String DayStr = "Day" + i.ToString();
                        try
                        {
                            table5.Columns[DayStr].ColumnName = tableH.Rows[0][i - 1].ToString();
                        }
                        catch (Exception ex1)
                        {
                        }
                    }
                    //table5.Columns["Day1"].ColumnName = tableH.Rows[0][0].ToString(); // "";// tableH.Rows[0].ItemArray[0]; // "Employee Name";	
                    //table5.Columns["Day2"].ColumnName = tableH.Rows[0][1].ToString(); //"Employee Name";	
                    //table5.Columns["Day3"].ColumnName = tableH.Rows[0][2].ToString();
                    //table5.Columns["Day4"].ColumnName = tableH.Rows[0][3].ToString();
                    //table5.Columns["Day5"].ColumnName = tableH.Rows[0][4].ToString();
                    //table5.Columns["Day6"].ColumnName = tableH.Rows[0][5].ToString();
                    //table5.Columns["Day7"].ColumnName = tableH.Rows[0][6].ToString();
                    //table5.Columns["Day8"].ColumnName = tableH.Rows[0][7].ToString();
                    //table5.Columns["Day9"].ColumnName = tableH.Rows[0][8].ToString();
                    //table5.Columns["Day10"].ColumnName = tableH.Rows[0][9].ToString();
                    //table5.Columns["Day11"].ColumnName = tableH.Rows[0][10].ToString();
                    //table5.Columns["Day12"].ColumnName = tableH.Rows[0][11].ToString();
                    //table5.Columns["Day13"].ColumnName = tableH.Rows[0][12].ToString();
                    //table5.Columns["Day14"].ColumnName = tableH.Rows[0][13].ToString();
                    //table5.Columns["Day15"].ColumnName = tableH.Rows[0][14].ToString();
                    //table5.Columns["Day16"].ColumnName = tableH.Rows[0][15].ToString();
                    //table5.Columns["Day17"].ColumnName = tableH.Rows[0][16].ToString();
                    //table5.Columns["Day18"].ColumnName = tableH.Rows[0][17].ToString();
                    //table5.Columns["Day19"].ColumnName = tableH.Rows[0][18].ToString();
                    //table5.Columns["Day20"].ColumnName = tableH.Rows[0][19].ToString();
                    //table5.Columns["Day21"].ColumnName = tableH.Rows[0][20].ToString();
                    //table5.Columns["Day22"].ColumnName = tableH.Rows[0][21].ToString();
                    //table5.Columns["Day23"].ColumnName = tableH.Rows[0][22].ToString();
                    //table5.Columns["Day24"].ColumnName = tableH.Rows[0][23].ToString();
                    //table5.Columns["Day25"].ColumnName = tableH.Rows[0][24].ToString();
                    //table5.Columns["Day26"].ColumnName = tableH.Rows[0][25].ToString();
                    //table5.Columns["Day27"].ColumnName = tableH.Rows[0][26].ToString();
                    //table5.Columns["Day28"].ColumnName = tableH.Rows[0][27].ToString();
                    //table5.Columns["Day29"].ColumnName = tableH.Rows[0][28].ToString();
                    //table5.Columns["Day30"].ColumnName = tableH.Rows[0][29].ToString();
                    //table5.Columns["Day31"].ColumnName = tableH.Rows[0][30].ToString();	

                }

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReportMonthly(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/MonthlyAttendanceReport.rdlc",
                 "abc", "~/User_Data/download/", "Monthly_Attendance_Report", ref PdfFilename, tableH, "DataSet1");

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

            }
            catch (System.Exception ex)
            {

                AddAuditTrail("IE " + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));
            }

            return PdfFilename;

            // PRINT END
            //---------------------------------------------REPORT RDLC MonthlyMusterReport END-------------------------------------------
        }

        // 5 Daily Multiple Punches Report


        private string DailyMultiplePunchesReport(DailyReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 8 FEB 2017
        {
            //bool ReturnStatus = false;
            string PdfFilename = "";

            try
            {
                // ---------------------------------------REPORT RDLC MonthlyMusterReport START-----------------------------------------
                // PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                //SqlCommand com1 = new SqlCommand("usp_Report_GetMonthlyMusterPart2", con);
                SqlCommand com1 = new SqlCommand("SP_DailyMultiplePunchingReport", con);
                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018
                com1.CommandType = CommandType.StoredProcedure;
                com1.Parameters.Add(new SqlParameter("@Fdate", SqlDbType.DateTime)).Value = ReportModel.FromDate; //new DateTime(2013, 01, 01);
                com1.Parameters.Add(new SqlParameter("@Tdate", SqlDbType.DateTime)).Value = ReportModel.FromDate; //new DateTime(2013, 01, 02); //MODIFIED BY PUSHKAR ON 27 DEC 2017
                com1.Parameters.Add(new SqlParameter("@Empid", SqlDbType.NVarChar)).Value = ReportModel.EmployeeString;
                SqlDataAdapter da1 = new SqlDataAdapter(com1);

                da1.Fill(table5);

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/DailyMultiplePunchingReport.rdlc",
                 "DailyOverStay", "~/User_Data/download/", "DailyMultiple_Punching_Report", ref PdfFilename);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

            }
            catch (System.Exception ex)
            {
                AddAuditTrail("IE " + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));
            }

            return PdfFilename;

            // PRINT END
            //---------------------------------------------REPORT RDLC MonthlyMusterReport END-------------------------------------------
        }


        //######################

        private string DailyLateEarlyReport(DailyReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 8 FEB 2017
        {
            //bool ReturnStatus = false;
            string PdfFilename = "";

            try
            {
                // ---------------------------------------REPORT RDLC MonthlyMusterReport START-----------------------------------------
                // PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                //SqlCommand com1 = new SqlCommand("usp_Report_GetMonthlyMusterPart2", con);
                SqlCommand com1 = new SqlCommand("SP_DailyLateEarlyReport", con);
                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018
                com1.CommandType = CommandType.StoredProcedure;
                com1.Parameters.Add(new SqlParameter("@Fdate", SqlDbType.DateTime)).Value = ReportModel.FromDate; //new DateTime(2013, 01, 01);
                com1.Parameters.Add(new SqlParameter("@Tdate", SqlDbType.DateTime)).Value = ReportModel.FromDate; //new DateTime(2013, 01, 02); //MODIFIED BY PUSHKAR ON 27 DEC 2017
                com1.Parameters.Add(new SqlParameter("@Empid", SqlDbType.NVarChar)).Value = ReportModel.EmployeeString;
                //com1.Parameters.Add(new SqlParameter("@BranchListId", SqlDbType.NVarChar, 500)).Value = "1,3,7";
                //com1.Parameters.Add(new SqlParameter("@CompanyListId", SqlDbType.NVarChar, 500)).Value = "2";
                //com1.Parameters.Add(new SqlParameter("@RoleTypeId", SqlDbType.NVarChar, 500)).Value = "4";
                //com1.Parameters.Add(new SqlParameter("@ReportingId", SqlDbType.Int)).Value = "76";
                SqlDataAdapter da1 = new SqlDataAdapter(com1);

                da1.Fill(table5);

                if (ReportModel.ReportFormat == 2)
                {

                    table5.Columns.Remove("CompanyId");
                    table5.Columns.Remove("BranchId");
                    table5.Columns.Remove("DepartmentId");
                    table5.Columns.Remove("DesignationId");
                    table5.Columns.Remove("EMPLOYEETYPEID");
                    table5.Columns.Remove("LateCount");
                    table5.Columns.Remove("EmployeeId");

                }

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/DailyLateEarlyReport.rdlc",
                 "DailyAttendance", "~/User_Data/download/", "DailyLateEarly_Report", ref PdfFilename);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

            }
            catch (System.Exception ex)
            {
                AddAuditTrail("IE " + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));
            }

            return PdfFilename;

            // PRINT END
            //---------------------------------------------REPORT RDLC MonthlyMusterReport END-------------------------------------------
        }

        //######################

        // 6 Monthly Left Report


        private string MonthlyLeftReport(MonthlyReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 8 FEB 2017
        {
            //bool ReturnStatus = false;
            string PdfFilename = "";

            try
            {

                // ---------------------------------------REPORT RDLC MonthlyMusterReport START-----------------------------------------
                // PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                //SqlCommand com1 = new SqlCommand("usp_Report_GetMonthlyMusterPart2", con);
                SqlCommand com1 = new SqlCommand("SP_MonthlyLeftReport", con);
                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018
                com1.CommandType = CommandType.StoredProcedure;
                com1.Parameters.Add(new SqlParameter("@Fdate", SqlDbType.DateTime)).Value = ReportModel.FromDate; //new DateTime(2013, 01, 01);
                com1.Parameters.Add(new SqlParameter("@Tdate", SqlDbType.DateTime)).Value = ReportModel.ToDate; //new DateTime(2013, 01, 02);
                com1.Parameters.Add(new SqlParameter("@Empid", SqlDbType.NVarChar)).Value = ReportModel.EmployeeString;
                //com1.Parameters.Add(new SqlParameter("@BranchListId", SqlDbType.NVarChar, 500)).Value = "1,3,7";
                //com1.Parameters.Add(new SqlParameter("@CompanyListId", SqlDbType.NVarChar, 500)).Value = "2";
                //com1.Parameters.Add(new SqlParameter("@RoleTypeId", SqlDbType.NVarChar, 500)).Value = "4";
                //com1.Parameters.Add(new SqlParameter("@ReportingId", SqlDbType.Int)).Value = "76";
                SqlDataAdapter da1 = new SqlDataAdapter(com1);

                da1.Fill(table5);

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/MonthlyLeftReport.rdlc",
                 "CurrentDay", "~/User_Data/download/", "Monthly_Left_Report", ref PdfFilename);

                //Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

            }
            catch (System.Exception ex)
            {
                AddAuditTrail("IE " + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));
            }

            return PdfFilename;

            // PRINT END
            //---------------------------------------------REPORT RDLC MonthlyMusterReport END-------------------------------------------
        }


        // 7 Monthly Addition List Report


        private string MonthlyAdditionListReport(MonthlyReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 8 FEB 2017
        {
            //bool ReturnStatus = false;
            string PdfFilename = "";

            try
            {
                // ---------------------------------------REPORT RDLC MonthlyMusterReport START-----------------------------------------
                // PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);
                //SqlCommand com1 = new SqlCommand("usp_Report_GetMonthlyMusterPart2", con);
                SqlCommand com1 = new SqlCommand("SP_MonthlyAdditionListReport", con);
                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018
                com1.CommandType = CommandType.StoredProcedure;
                com1.Parameters.Add(new SqlParameter("@Fdate", SqlDbType.DateTime)).Value = ReportModel.FromDate; //new DateTime(2013, 01, 01);
                com1.Parameters.Add(new SqlParameter("@Tdate", SqlDbType.DateTime)).Value = ReportModel.ToDate; //new DateTime(2013, 01, 02);
                com1.Parameters.Add(new SqlParameter("@Empid", SqlDbType.NVarChar)).Value = ReportModel.EmployeeString;
                //com1.Parameters.Add(new SqlParameter("@BranchListId", SqlDbType.NVarChar, 500)).Value = "1,3,7";
                //com1.Parameters.Add(new SqlParameter("@CompanyListId", SqlDbType.NVarChar, 500)).Value = "2";
                //com1.Parameters.Add(new SqlParameter("@RoleTypeId", SqlDbType.NVarChar, 500)).Value = "4";
                //com1.Parameters.Add(new SqlParameter("@ReportingId", SqlDbType.Int)).Value = "76";
                SqlDataAdapter da1 = new SqlDataAdapter(com1);

                da1.Fill(table5);

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/MonthlyAdditionListReport.rdlc",
                 "CurrentDay", "~/User_Data/download/", "Monthly_AdditionList_Report_", ref PdfFilename);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

            }
            catch (System.Exception ex)
            {
                AddAuditTrail("IE " + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));
            }

            return PdfFilename;

            // PRINT END
            //---------------------------------------------REPORT RDLC MonthlyMusterReport END-------------------------------------------
        }




        // 8 Monthly Salary Output Report


        private string MonthlySalaryOutputReport(MonthlyReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 8 FEB 2017
        {
            //bool ReturnStatus = false;
            string PdfFilename = "";

            try
            {

                // ADDED BY MSJ ON 29 DEC 2017 END

                //ADDED CODE BY SHRADDHA ON 18 JAN 2018 START
                List<FinancialYear> FYObjList = WetosDB.FinancialYears.ToList();
                List<string> FYStrList = new List<string>();
                foreach (FinancialYear CurrentFYObj in FYObjList)
                {
                    if (CurrentFYObj.StartDate.Year != CurrentFYObj.EndDate.Year) // DIFF YEARS
                    {
                        FYStrList.Add(CurrentFYObj.StartDate.Year.ToString());
                        FYStrList.Add(CurrentFYObj.EndDate.Year.ToString());

                    }
                    else
                    {
                        FYStrList.Add(CurrentFYObj.StartDate.Year.ToString());
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
                //ADDED CODE BY SHRADDHA ON 18 JAN 2018 END


                // ---------------------------------------REPORT RDLC MonthlyMusterReport START-----------------------------------------
                // PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                //SqlCommand com1 = new SqlCommand("usp_Report_GetMonthlyMusterPart2", con);
                SqlCommand com1 = new SqlCommand("SP_MonthlySalaryOutputReport", con);

                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018

                com1.CommandType = CommandType.StoredProcedure;
                com1.Parameters.Add(new SqlParameter("@Fdate", SqlDbType.DateTime)).Value = ReportModel.FromDate; //new DateTime(2013, 01, 01);
                com1.Parameters.Add(new SqlParameter("@Tdate", SqlDbType.DateTime)).Value = ReportModel.ToDate; //new DateTime(2013, 01, 02);
                com1.Parameters.Add(new SqlParameter("@Empid", SqlDbType.NVarChar)).Value = ReportModel.EmployeeString;
                //com1.Parameters.Add(new SqlParameter("@BranchListId", SqlDbType.NVarChar, 500)).Value = "1,3,7";
                //com1.Parameters.Add(new SqlParameter("@CompanyListId", SqlDbType.NVarChar, 500)).Value = "2";
                //com1.Parameters.Add(new SqlParameter("@RoleTypeId", SqlDbType.NVarChar, 500)).Value = "4";
                //com1.Parameters.Add(new SqlParameter("@ReportingId", SqlDbType.Int)).Value = "76";
                SqlDataAdapter da1 = new SqlDataAdapter(com1);

                da1.Fill(table5);

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/MonthlySalaryOutputReport.rdlc",
                 "abc", "~/User_Data/download/", "Monthly_SalaryOutput_Report_", ref PdfFilename);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

            }
            catch (System.Exception ex)
            {
                AddAuditTrail("IE " + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));
            }

            return PdfFilename;

            // PRINT END
            //---------------------------------------------REPORT RDLC MonthlyMusterReport END-------------------------------------------
        }




        private string MonthlySalaryAbsentismReport(MonthlyReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 8 FEB 2017
        {
            //bool ReturnStatus = false;
            string PdfFilename = "";

            try
            {

                //ADDED CODE BY SHRADDHA ON 18 JAN 2018 START
                List<FinancialYear> FYObjList = WetosDB.FinancialYears.ToList();
                List<string> FYStrList = new List<string>();
                foreach (FinancialYear CurrentFYObj in FYObjList)
                {
                    if (CurrentFYObj.StartDate.Year != CurrentFYObj.EndDate.Year) // DIFF YEARS
                    {
                        FYStrList.Add(CurrentFYObj.StartDate.Year.ToString());
                        FYStrList.Add(CurrentFYObj.EndDate.Year.ToString());

                    }
                    else
                    {
                        FYStrList.Add(CurrentFYObj.StartDate.Year.ToString());
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
                //ADDED CODE BY SHRADDHA ON 18 JAN 2018 END


                // ---------------------------------------REPORT RDLC MonthlyMusterReport START-----------------------------------------
                // PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                //SqlCommand com1 = new SqlCommand("usp_Report_GetMonthlyMusterPart2", con);
                SqlCommand com1 = new SqlCommand("SP_MonthlySalaryOutputReport", con);

                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018

                com1.CommandType = CommandType.StoredProcedure;
                com1.Parameters.Add(new SqlParameter("@Fdate", SqlDbType.DateTime)).Value = ReportModel.FromDate; //new DateTime(2013, 01, 01);
                com1.Parameters.Add(new SqlParameter("@Tdate", SqlDbType.DateTime)).Value = ReportModel.ToDate; //new DateTime(2013, 01, 02);
                com1.Parameters.Add(new SqlParameter("@Empid", SqlDbType.NVarChar)).Value = ReportModel.EmployeeString;
                //com1.Parameters.Add(new SqlParameter("@BranchListId", SqlDbType.NVarChar, 500)).Value = "1,3,7";
                //com1.Parameters.Add(new SqlParameter("@CompanyListId", SqlDbType.NVarChar, 500)).Value = "2";
                //com1.Parameters.Add(new SqlParameter("@RoleTypeId", SqlDbType.NVarChar, 500)).Value = "4";
                //com1.Parameters.Add(new SqlParameter("@ReportingId", SqlDbType.Int)).Value = "76";
                SqlDataAdapter da1 = new SqlDataAdapter(com1);

                da1.Fill(table5);

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/MonthlySalaryAbsentismReport.rdlc",
                 "abc", "~/User_Data/download/", "Monthly_SalaryAbsentism_Report_", ref PdfFilename);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

            }
            catch (System.Exception ex)
            {
                AddAuditTrail("IE " + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));
            }

            return PdfFilename;

            // PRINT END
            //---------------------------------------------REPORT RDLC MonthlyMusterReport END-------------------------------------------
        }


        //####### ADDED BY PUSHKAR ON 24 FEB 2018
        private string MonthlySalaryShiftAllowanceReport(MonthlyReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 8 FEB 2017
        {
            //bool ReturnStatus = false;
            string PdfFilename = "";

            try
            {
                // ADDED BY MSJ ON 29 DEC 2017 END

                //ADDED CODE BY SHRADDHA ON 18 JAN 2018 START
                List<FinancialYear> FYObjList = WetosDB.FinancialYears.ToList();
                List<string> FYStrList = new List<string>();
                foreach (FinancialYear CurrentFYObj in FYObjList)
                {
                    if (CurrentFYObj.StartDate.Year != CurrentFYObj.EndDate.Year) // DIFF YEARS
                    {
                        FYStrList.Add(CurrentFYObj.StartDate.Year.ToString());
                        FYStrList.Add(CurrentFYObj.EndDate.Year.ToString());

                    }
                    else
                    {
                        FYStrList.Add(CurrentFYObj.StartDate.Year.ToString());
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
                //ADDED CODE BY SHRADDHA ON 18 JAN 2018 END


                // ---------------------------------------REPORT RDLC MonthlyMusterReport START-----------------------------------------
                // PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                //SqlCommand com1 = new SqlCommand("usp_Report_GetMonthlyMusterPart2", con);
                SqlCommand com1 = new SqlCommand("SP_MonthlyShiftAlllowanceReport", con);

                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018

                com1.CommandType = CommandType.StoredProcedure;
                com1.Parameters.Add(new SqlParameter("@Fdate", SqlDbType.DateTime)).Value = ReportModel.FromDate; //new DateTime(2013, 01, 01);
                com1.Parameters.Add(new SqlParameter("@Tdate", SqlDbType.DateTime)).Value = ReportModel.ToDate; //new DateTime(2013, 01, 02);
                com1.Parameters.Add(new SqlParameter("@EmpidList", SqlDbType.NVarChar)).Value = ReportModel.EmployeeString;
                //com1.Parameters.Add(new SqlParameter("@BranchListId", SqlDbType.NVarChar, 500)).Value = "1,3,7";
                //com1.Parameters.Add(new SqlParameter("@CompanyListId", SqlDbType.NVarChar, 500)).Value = "2";
                //com1.Parameters.Add(new SqlParameter("@RoleTypeId", SqlDbType.NVarChar, 500)).Value = "4";
                //com1.Parameters.Add(new SqlParameter("@ReportingId", SqlDbType.Int)).Value = "76";
                SqlDataAdapter da1 = new SqlDataAdapter(com1);

                da1.Fill(table5);

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/MonthlySalaryShiftAllowanceReport.rdlc",
                 "abc", "~/User_Data/download/", "Monthly_SalaryShiftAllowance_Report_", ref PdfFilename);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

            }
            catch (System.Exception ex)
            {
                AddAuditTrail("IE " + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));
            }

            return PdfFilename;

            // PRINT END
            //---------------------------------------------REPORT RDLC MonthlyMusterReport END-------------------------------------------
        }
        //####### ADDED BY PUSHKAR ON 24 FEB 2018

        // 9 Yearly Perfomance Report MaleWise


        //################################################################# ADDED BY PUSHKAR ON 22 FEB 2018 START


        // 4 Monthly Attendance Report
        /// <summary>
        /// MonthlyMusterReport
        /// </summary>
        /// <returns></returns>
        private string MonthlyLossofPayReport(MonthlyReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 8 FEB 2017
        {
            //bool ReturnStatus = false;
            string PdfFilename = "";

            try
            {

                // ---------------------------------------REPORT RDLC MonthlyMusterReport START-----------------------------------------
                // PRINT START
                DataTable table5 = new DataTable();
                DataTable tableH = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                //SqlCommand com1 = new SqlCommand("usp_Report_GetMonthlyMusterPart2", con);
                SqlCommand com1 = new SqlCommand("SP_MonthlyAttendanceReport", con);
                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018
                com1.CommandType = CommandType.StoredProcedure;
                com1.Parameters.Add(new SqlParameter("@Fdate", SqlDbType.DateTime)).Value = ReportModel.FromDate; //new DateTime(2013, 01, 01);
                com1.Parameters.Add(new SqlParameter("@Tdate", SqlDbType.DateTime)).Value = ReportModel.ToDate; //new DateTime(2013, 01, 02);
                com1.Parameters.Add(new SqlParameter("@EmpidList", SqlDbType.NVarChar)).Value = ReportModel.EmployeeString;

                SqlCommand com2 = new SqlCommand("SP_MonthlyReportHeader", con);
                com2.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018
                com2.CommandType = CommandType.StoredProcedure;
                com2.Parameters.Add(new SqlParameter("@start_date", SqlDbType.DateTime)).Value = ReportModel.FromDate; //new DateTime(2013, 01, 01);
                com2.Parameters.Add(new SqlParameter("@end_date", SqlDbType.DateTime)).Value = ReportModel.ToDate; //new DateTime(2013, 01, 02);

                //com1.Parameters.Add(new SqlParameter("@BranchListId", SqlDbType.NVarChar, 500)).Value = "1,3,7";
                //com1.Parameters.Add(new SqlParameter("@CompanyListId", SqlDbType.NVarChar, 500)).Value = "2";
                //com1.Parameters.Add(new SqlParameter("@RoleTypeId", SqlDbType.NVarChar, 500)).Value = "4";
                //com1.Parameters.Add(new SqlParameter("@ReportingId", SqlDbType.Int)).Value = "76";
                SqlDataAdapter da1 = new SqlDataAdapter(com1);
                SqlDataAdapter da2 = new SqlDataAdapter(com2);

                da1.Fill(table5);
                da2.Fill(tableH);

                if (ReportModel.ReportFormat == 2)
                {
                    table5.Columns.Remove("CompanyIdT");
                    table5.Columns.Remove("DepartmentIdT");
                    table5.Columns.Remove("BranchIdT");
                    table5.Columns.Remove("DesignationIdT");
                    table5.Columns.Remove("EmployeeIdT");
                    table5.Columns.Remove("StatusT");
                    table5.Columns.Remove("TranDateT");
                    table5.Columns.Remove("PresentT");
                    table5.Columns.Remove("WeeklyOffT");
                    table5.Columns.Remove("PaidHolidayT");
                    table5.Columns.Remove("TotalPaidDaysT");

                }

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReportMonthly(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/MonthlyLossofPayReport.rdlc",
                 "abc", "~/User_Data/download/", "Monthly_LossofPay_Report", ref PdfFilename, tableH, "DataSet1");

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

            }
            catch (System.Exception ex)
            {
                AddAuditTrail("IE " + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));
            }

            return PdfFilename;

            // PRINT END
            //---------------------------------------------REPORT RDLC MonthlyMusterReport END-------------------------------------------
        }



        //################################################################# ADDED BY PUSHKAR ON 22 FEB 2018 END

        private string YearlyPerfomanceReportMaleWise(MonthlyReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 8 FEB 2017
        {
            //bool ReturnStatus = false;
            string PdfFilename = "";

            try
            {
                string FinName = ReportModel.FinancialYearId.ToString();
                // ---------------------------------------REPORT RDLC MonthlyMusterReport START-----------------------------------------
                // PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                //SqlCommand com1 = new SqlCommand("usp_Report_GetMonthlyMusterPart2", con);
                SqlCommand com1 = new SqlCommand("SP_YearlyPerformanceReportMaleWise", con);
                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018
                com1.CommandType = CommandType.StoredProcedure;
                com1.Parameters.Add(new SqlParameter("@EmpidList", SqlDbType.NVarChar)).Value = ReportModel.EmployeeString;
                com1.Parameters.Add(new SqlParameter("@FinancialName", SqlDbType.NVarChar)).Value = FinName;//MODIFIED BY PUSHKAR ON 19 JAN 2018
                SqlDataAdapter da1 = new SqlDataAdapter(com1);

                da1.Fill(table5);

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/YearlyPerformanceReportMaleWise.rdlc",
                 "abc", "~/User_Data/download/", "YearlyPerformance_Report_MaleWise_", ref PdfFilename);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

            }
            catch (System.Exception ex)
            {
                AddAuditTrail("IE " + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));
            }

            return PdfFilename;

            // PRINT END
            //---------------------------------------------REPORT RDLC MonthlyMusterReport END-------------------------------------------
        }


        //ADDED BY PUSHKAR ON 21 FEB 2018 START

        private string YearlyPerfomanceReportGeneric(MonthlyReportsModel ReportModel, FormCollection fc)
        {
            //bool ReturnStatus = false;
            string PdfFilename = "";

            try
            {

                string FinName = ReportModel.FinancialYearId.ToString(); //MODIFIED BY PUSHKAR ON 15 JAN 2018
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                //SqlCommand com1 = new SqlCommand("usp_Report_GetMonthlyMusterPart2", con);
                SqlCommand com1 = new SqlCommand("SP_YearlyPerformanceReportGeneric", con);
                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018
                com1.CommandType = CommandType.StoredProcedure;
                com1.Parameters.Add(new SqlParameter("@EmpidList", SqlDbType.NVarChar)).Value = ReportModel.EmployeeString;
                com1.Parameters.Add(new SqlParameter("@FinancialName", SqlDbType.NVarChar)).Value = FinName;//MODIFIED BY PUSHKAR ON 19 JAN 2018
                //com1.Parameters.Add(new SqlParameter("@Gender", SqlDbType.NVarChar)).Value = 'M';
                //com1.Parameters.Add(new SqlParameter("@BranchListId", SqlDbType.NVarChar, 500)).Value = "1,3,7";
                //com1.Parameters.Add(new SqlParameter("@CompanyListId", SqlDbType.NVarChar, 500)).Value = "2";
                //com1.Parameters.Add(new SqlParameter("@RoleTypeId", SqlDbType.NVarChar, 500)).Value = "4";
                //com1.Parameters.Add(new SqlParameter("@ReportingId", SqlDbType.Int)).Value = "76";
                SqlDataAdapter da1 = new SqlDataAdapter(com1);

                da1.Fill(table5);

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/YearlyPerformanceReportGeneric.rdlc",
                 "abc", "~/User_Data/download/", "YearlyPerformance_Report_", ref PdfFilename);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

            }
            catch (System.Exception ex)
            {
                AddAuditTrail("IE " + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));
            }

            return PdfFilename;

            // PRINT END

        }

        //ADDED BY PUSHKAR ON 21 FEB 2018 END

        // 10 Yearly Encashment Leave Report

        private string YearlyEncashmentLeaveReport(MonthlyReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 8 FEB 2017
        {
            //bool ReturnStatus = false;
            string PdfFilename = "";

            try
            {

                // ADDED BY MSJ ON 29 DEC 2017 START
                // GET FROM AND TO DATE FROM YEAR AND MONTH ID

                // GET FY from ID
                // Get MOnthNo from Id
                string FinName = ReportModel.FinancialYearId.ToString(); //MODIFIED BY PUSHKAR ON 15 JAN 2018

                // ---------------------------------------REPORT RDLC MonthlyMusterReport START-----------------------------------------
                // PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                //SqlCommand com1 = new SqlCommand("usp_Report_GetMonthlyMusterPart2", con);
                SqlCommand com1 = new SqlCommand("SP_YearlyEncashLeaveReport", con);
                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018
                com1.CommandType = CommandType.StoredProcedure;
                //com1.Parameters.Add(new SqlParameter("@Fdate", SqlDbType.DateTime)).Value = ReportModel.FromDate; //new DateTime(2013, 01, 01);
                //com1.Parameters.Add(new SqlParameter("@Tdate", SqlDbType.DateTime)).Value = ReportModel.ToDate; //new DateTime(2013, 01, 02);
                com1.Parameters.Add(new SqlParameter("@FinancialYearId", SqlDbType.Int)).Value = 1; //HARD CODED BY PUSHKAR ON 27 DEC 2017
                com1.Parameters.Add(new SqlParameter("@Empid", SqlDbType.NVarChar)).Value = ReportModel.EmployeeString;
                //com1.Parameters.Add(new SqlParameter("@BranchListId", SqlDbType.NVarChar, 500)).Value = "1,3,7";
                //com1.Parameters.Add(new SqlParameter("@CompanyListId", SqlDbType.NVarChar, 500)).Value = "2";
                //com1.Parameters.Add(new SqlParameter("@RoleTypeId", SqlDbType.NVarChar, 500)).Value = "4";
                //com1.Parameters.Add(new SqlParameter("@ReportingId", SqlDbType.Int)).Value = "76";
                SqlDataAdapter da1 = new SqlDataAdapter(com1);

                da1.Fill(table5);

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/YearlyEncashLeaveReport.rdlc",
                 "CompanyMaster", "~/User_Data/download/", "Yearly_EncashLeave_Report_", ref PdfFilename);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

            }
            catch (System.Exception ex)
            {
                AddAuditTrail("IE " + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));
            }

            return PdfFilename;

            // PRINT END
            //---------------------------------------------REPORT RDLC MonthlyMusterReport END-------------------------------------------
        }


        // 11 Yearly Leave Report.

        private string YearlyLeaveReport(MonthlyReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 8 FEB 2017
        {
            //bool ReturnStatus = false;
            string PdfFilename = "";

            try
            {
                string FinName = ReportModel.FinancialYearId.ToString(); //MODIFIED BY PUSHKAR ON 15 JAN 2018

                // PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                //SqlCommand com1 = new SqlCommand("usp_Report_GetMonthlyMusterPart2", con);
                SqlCommand com1 = new SqlCommand("SP_YearlyLeaveReport", con);
                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018
                com1.CommandType = CommandType.StoredProcedure;
                //com1.Parameters.Add(new SqlParameter("@Fdate", SqlDbType.DateTime)).Value = ReportModel.FromDate; //new DateTime(2013, 01, 01);
                //com1.Parameters.Add(new SqlParameter("@Tdate", SqlDbType.DateTime)).Value = ReportModel.ToDate; //new DateTime(2013, 01, 02);
                com1.Parameters.Add(new SqlParameter("@FinancialName", SqlDbType.NVarChar)).Value = FinName;//MODIFIED BY PUSHKAR ON 15 JAN 2018 //HARD CODED BY PUSHKAR ON 27 DEC 2017
                com1.Parameters.Add(new SqlParameter("@EmpidList", SqlDbType.NVarChar)).Value = ReportModel.EmployeeString;
                //com1.Parameters.Add(new SqlParameter("@BranchListId", SqlDbType.NVarChar, 500)).Value = "1,3,7";
                //com1.Parameters.Add(new SqlParameter("@CompanyListId", SqlDbType.NVarChar, 500)).Value = "2";
                //com1.Parameters.Add(new SqlParameter("@RoleTypeId", SqlDbType.NVarChar, 500)).Value = "4";
                //com1.Parameters.Add(new SqlParameter("@ReportingId", SqlDbType.Int)).Value = "76";
                SqlDataAdapter da1 = new SqlDataAdapter(com1);

                da1.Fill(table5);

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/YearlyLeaveReport.rdlc",
                 "abc", "~/User_Data/download/", "Yearly_Leave_Report_", ref PdfFilename);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

            }
            catch (System.Exception ex)
            {
                AddAuditTrail("IE " + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));
            }

            return PdfFilename;

            // PRINT END
            //---------------------------------------------REPORT RDLC MonthlyMusterReport END-------------------------------------------
        }


        #endregion

        /// <summary>
        /// Added by shalaka on 25th DEC 2017
        /// For L&T Daily Report 
        /// </summary>
        /// <returns></returns>
        public ActionResult LNTDailyReportsIndex()
        {
            DailyReportsModel DailyReportsModelObj = new DailyReportsModel();
            try
            {
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);

                Employee LoggedInEmployee = WetosDB.Employees.Where(a => a.EmployeeId == EmployeeId).FirstOrDefault();

                UserRole ChechUserRole = WetosDB.UserRoles.Where(a => a.EmployeeId == EmployeeId).FirstOrDefault();

                //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO HANDLE NULL OBJECT REFERECE ERROR FOR LoggedInEmployee OBJ AND ChechUserRole OBJ START
                if (LoggedInEmployee == null)
                {
                    LoggedInEmployee = new Employee();
                }
                if (ChechUserRole == null)
                {
                    ChechUserRole = new UserRole();
                }
                //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO HANDLE NULL OBJECT REFERECE ERROR FOR LoggedInEmployee OBJ AND ChechUserRole OBJ END

                DailyReportsModelObj.CompanyId = LoggedInEmployee.CompanyId;
                DailyReportsModelObj.BranchId = LoggedInEmployee.BranchId;
                DailyReportsModelObj.DepartmentId = LoggedInEmployee.DepartmentId;
                DailyReportsModelObj.EmployeeId = LoggedInEmployee.EmployeeId;
                DailyReportsModelObj.UserId = ChechUserRole.RoleTypeId;


                #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //var EmployeeNameList = WetosDB.Employees.Where(a => a.ActiveFlag == null || a.ActiveFlag == true
                //&& a.CompanyId == LoggedInEmployee.CompanyId && a.BranchId == LoggedInEmployee.BranchId
                //&& a.EmployeeReportingId == LoggedInEmployee.EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
                var EmployeeNameList = WetosDB.SP_VwActiveEmployee(EmployeeId).Where(a => a.ActiveFlag == null || a.ActiveFlag == true
                        && a.CompanyId == LoggedInEmployee.CompanyId && a.BranchId == LoggedInEmployee.BranchId
                        && a.EmployeeReportingId == LoggedInEmployee.EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
                #endregion
                ViewBag.EmployeeList = new SelectList(EmployeeNameList, "EmployeeId", "EmployeeName").ToList();

            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in monthly reports selection due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));
            }

            //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable START
            ExportToExcelAndPDFEnable();
            //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable END


            return View(DailyReportsModelObj);
        }

        /// <summary>
        /// Added by shalaka on 25th DEC 2017
        /// For L&T Daily Report 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LNTDailyReportsIndex(DailyReportsModel ReportModel, FormCollection fc)
        {
            DailyReportsModel DailyReportsModelObj = new DailyReportsModel();
            try
            {
                string PdfFileName = "";

                #region GENERIC CODE ADDED BY SHRADDHA ON 17 MAR 2018
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                DateTime FDate = WetosDB.DailyTransactions.OrderBy(a => a.TranDate).Select(a => a.TranDate).FirstOrDefault();
                DateTime TDate = DateTime.Now;
                string EmployeeString = string.Empty;

                if ((!string.IsNullOrEmpty(ReportModel.FromDate)))
                {
                    //CODE ADDED FOR DATE RANGE VALIDATION BY SHRADDHA ON 23 FEB 2018 START
                    FDate = Convert.ToDateTime(ReportModel.FromDate);
                    TDate = Convert.ToDateTime(ReportModel.FromDate);

                    if ((TDate - FDate).TotalDays > 31)
                    {
                        PopulateDropDown();
                        ModelState.AddModelError("Error", "Please select date range less than or equal to 31 days.");
                        Error("Please select date range less than or equal to 31 days.");
                        ExportToExcelAndPDFEnable();
                        return View(ReportModel);
                    }
                }

                FDate = Convert.ToDateTime(ReportModel.FromDate);
                TDate = Convert.ToDateTime(ReportModel.FromDate);


                #region ADDED REGION BY SHRADDHA ON 17 MAR 2018
                if (ReportModel.EmployeeId == 0 || ReportModel.EmployeeId == null)
                {
                    int? CompanyId = ReportModel.CompanyId == 0 ? (int?)null : ReportModel.CompanyId;
                    int? BranchId = ReportModel.BranchId == 0 ? (int?)null : ReportModel.BranchId;
                    int? EmployeeTypeId = ReportModel.EmployeeTypeId == 0 ? (int?)null : ReportModel.EmployeeTypeId;
                    int? DepartmentId = ReportModel.DepartmentId == 0 ? (int?)null : ReportModel.DepartmentId;
                    EmployeeString = GetEmployeeString(FDate, TDate, CompanyId, BranchId, EmployeeTypeId, DepartmentId);
                }
                else
                {
                    EmployeeString = fc["EmployeeId"];
                }
                ReportModel.EmployeeString = EmployeeString;
                #endregion
                #endregion

                ReportModel.ReportFormat = Convert.ToInt32(fc["Export"]);

                switch (ReportModel.ReportSelected)
                {
                    case "1":  // 5 Daily Multiple Punches Report
                        string Report5 = "Daily Multiple Punches Report";
                        ReportModel.ReportName = Report5;
                        PdfFileName = DailyMultiplePunchesReport(ReportModel, fc);

                        break;
                    case "2": // 2 DailyLong Absnteeism Report
                        string Report2 = "DailyLong Absnteeism Report";
                        ReportModel.ReportName = Report2;
                        PdfFileName = DailyLongAbsnteeismReport(ReportModel, fc);
                        break;

                    case "3": // 2 DailyLong Absnteeism Report
                        string Report3 = "Daily Late Early Report";
                        ReportModel.ReportName = Report3;
                        PdfFileName = DailyLateEarlyReport(ReportModel, fc);
                        break;

                    default:
                        break;
                }

                // Added by Rajas on 23 DEC 2016 for handling null or empty PdfFileName START
                if (string.IsNullOrEmpty(PdfFileName))
                {
                    PopulateDropDown();

                    ModelState.AddModelError("Error", "Please try again");

                    //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable START
                    ExportToExcelAndPDFEnable();
                    //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable END

                    return View(ReportModel);
                }
                else
                {
                    // Added by Rajas on 14 MARCh 2017
                    if (ReportModel.ReportFormat == 2)  // excel format
                    {
                        return File(PdfFileName, "application/xls");
                    }
                    else // PDF format
                    {
                        return File(PdfFileName, "application/pdf");
                    }

                }
                // Added by Rajas on 23 DEC 2016 for handling null or empty PdfFileName END

            }
            catch (System.Exception ex)
            {
                PopulateDropDown();

                //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable START
                ExportToExcelAndPDFEnable();
                //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable END

                AddAuditTrail(ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                return View(ReportModel);
            }

            // return View(DailyReportsModelObj); // COMMENTED BY MSJ ON 13 JAN 2019
        }

        #region LNT DAILY REPORTS FOR HR CODE ADDED BY SHRADDHA ON 18 JAN 2018
        /// <summary>
        /// Added by shalaka on 25th DEC 2017
        /// For L&T Daily Report 
        /// </summary>
        /// <returns></returns>
        public ActionResult LNTDailyReportsIndexHR()
        {
            DailyReportsModel DailyReportsModelObj = new DailyReportsModel();
            try
            {
                PopulateDropDown();//code added by shraddha on 18 jan 2018

                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);

                Employee LoggedInEmployee = WetosDB.Employees.Where(a => a.EmployeeId == EmployeeId).FirstOrDefault();

                UserRole ChechUserRole = WetosDB.UserRoles.Where(a => a.EmployeeId == EmployeeId).FirstOrDefault();

                //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO HANDLE NULL OBJECT REFERECE ERROR FOR LoggedInEmployee OBJ AND ChechUserRole OBJ START
                if (LoggedInEmployee == null)
                {
                    LoggedInEmployee = new Employee();
                }
                if (ChechUserRole == null)
                {
                    ChechUserRole = new UserRole();
                }
                //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO HANDLE NULL OBJECT REFERECE ERROR FOR LoggedInEmployee OBJ AND ChechUserRole OBJ END

                //DailyReportsModelObj.CompanyId = LoggedInEmployee.CompanyId;
                //DailyReportsModelObj.BranchId = LoggedInEmployee.BranchId;
                //DailyReportsModelObj.DepartmentId = LoggedInEmployee.DepartmentId;
                //DailyReportsModelObj.EmployeeId = LoggedInEmployee.EmployeeId;
                //DailyReportsModelObj.UserId = ChechUserRole.RoleTypeId;

                //var EmployeeNameList = WetosDB.Employees.Where(a => a.ActiveFlag == null || a.ActiveFlag == true
                //        && a.CompanyId == LoggedInEmployee.CompanyId && a.BranchId == LoggedInEmployee.BranchId
                //        && a.EmployeeReportingId == LoggedInEmployee.EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " " + a.FirstName + " " + a.LastName }).ToList();
                //ViewBag.EmployeeList = new SelectList(EmployeeNameList, "EmployeeId", "EmployeeName").ToList();

            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in monthly reports selection due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));
                PopulateDropDown();//code added by shraddha on 18 jan 2018
            }

            //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable START
            ExportToExcelAndPDFEnable();
            //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable END


            return View(DailyReportsModelObj);
        }

        /// <summary>
        /// Added by shalaka on 25th DEC 2017
        /// For L&T Daily Report 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LNTDailyReportsIndexHR(DailyReportsModel ReportModel, FormCollection fc)
        {
            DailyReportsModel DailyReportsModelObj = new DailyReportsModel();
            try
            {
                string PdfFileName = "";

                #region GENERIC CODE ADDED BY SHRADDHA ON 17 MAR 2018
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                DateTime FDate = WetosDB.DailyTransactions.OrderBy(a => a.TranDate).Select(a => a.TranDate).FirstOrDefault();
                DateTime TDate = DateTime.Now;
                string EmployeeString = string.Empty;

                if ((!string.IsNullOrEmpty(ReportModel.FromDate)))
                {
                    //CODE ADDED FOR DATE RANGE VALIDATION BY SHRADDHA ON 23 FEB 2018 START
                    FDate = Convert.ToDateTime(ReportModel.FromDate);
                    TDate = Convert.ToDateTime(ReportModel.FromDate);

                    if ((TDate - FDate).TotalDays > 31)
                    {
                        PopulateDropDown();
                        ModelState.AddModelError("Error", "Please select date range less than or equal to 31 days.");
                        Error("Please select date range less than or equal to 31 days.");
                        ExportToExcelAndPDFEnable();
                        return View(ReportModel);
                    }
                }

                FDate = Convert.ToDateTime(ReportModel.FromDate);
                TDate = Convert.ToDateTime(ReportModel.FromDate);


                #region ADDED REGION BY SHRADDHA ON 17 MAR 2018
                if (ReportModel.EmployeeId == 0 || ReportModel.EmployeeId == null)
                {
                    int? CompanyId = ReportModel.CompanyId == 0 ? (int?)null : ReportModel.CompanyId;
                    int? BranchId = ReportModel.BranchId == 0 ? (int?)null : ReportModel.BranchId;
                    int? EmployeeTypeId = ReportModel.EmployeeTypeId == 0 ? (int?)null : ReportModel.EmployeeTypeId;
                    int? DepartmentId = ReportModel.DepartmentId == 0 ? (int?)null : ReportModel.DepartmentId;
                    EmployeeString = GetEmployeeString(FDate, TDate, CompanyId, BranchId, EmployeeTypeId, DepartmentId);
                }
                else
                {
                    EmployeeString = fc["EmployeeId"];
                }
                ReportModel.EmployeeString = EmployeeString;
                #endregion
                #endregion

                ReportModel.ReportFormat = Convert.ToInt32(fc["Export"]);

                switch (ReportModel.ReportSelected)
                {
                    case "1":  // 5 Daily Multiple Punches Report
                        string Report5 = "Daily Multiple Punches Report";
                        ReportModel.ReportName = Report5;
                        PdfFileName = DailyMultiplePunchesReport(ReportModel, fc);

                        break;
                    case "2": // 2 DailyLong Absnteeism Report
                        string Report2 = "DailyLong Absnteeism Report";
                        ReportModel.ReportName = Report2;
                        PdfFileName = DailyLongAbsnteeismReport(ReportModel, fc);
                        break;

                    case "3": // 2 DailyLong Absnteeism Report
                        string Report3 = "Daily Late Early Report";
                        ReportModel.ReportName = Report3;
                        PdfFileName = DailyLateEarlyReport(ReportModel, fc);
                        break;

                    default:
                        break;
                }

                // Added by Rajas on 23 DEC 2016 for handling null or empty PdfFileName START
                if (string.IsNullOrEmpty(PdfFileName))
                {
                    PopulateDropDown();

                    ModelState.AddModelError("Error", "Please try again");

                    //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable START
                    ExportToExcelAndPDFEnable();
                    //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable END

                    return View(ReportModel);
                }
                else
                {
                    // Added by Rajas on 14 MARCh 2017
                    if (ReportModel.ReportFormat == 2)  // excel format
                    {
                        return File(PdfFileName, "application/xls");
                    }
                    else // PDF format
                    {
                        return File(PdfFileName, "application/pdf");
                    }

                }
                // Added by Rajas on 23 DEC 2016 for handling null or empty PdfFileName END

            }
            catch (System.Exception ex)
            {
                PopulateDropDown();

                //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable START
                ExportToExcelAndPDFEnable();
                //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable END

                AddAuditTrail(ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                return View(ReportModel);
            }
            //return View(DailyReportsModelObj);  // COMMENTED BY MSJ ON 13 JAN 2019
        }

        #endregion

        /// <summary>
        /// Added by shalaka on 25th DEC 2017
        /// For L&T Yearly Report 
        /// </summary>
        /// <returns></returns>
        public ActionResult LNTYearlyReportsIndex()
        {
            MonthlyReportsModel MonthlyReportsObj = new MonthlyReportsModel();
            try
            {
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);

                Employee LoggedInEmployee = WetosDB.Employees.Where(a => a.EmployeeId == EmployeeId).FirstOrDefault();

                UserRole ChechUserRole = WetosDB.UserRoles.Where(a => a.EmployeeId == EmployeeId).FirstOrDefault();

                //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO HANDLE NULL OBJECT REFERECE ERROR FOR LoggedInEmployee OBJ AND ChechUserRole OBJ START
                if (LoggedInEmployee == null)
                {
                    LoggedInEmployee = new Employee();
                }
                if (ChechUserRole == null)
                {
                    ChechUserRole = new UserRole();
                }
                //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO HANDLE NULL OBJECT REFERECE ERROR FOR LoggedInEmployee OBJ AND ChechUserRole OBJ END

                MonthlyReportsObj.CompanyId = LoggedInEmployee.CompanyId;
                MonthlyReportsObj.BranchId = LoggedInEmployee.BranchId;
                MonthlyReportsObj.DepartmentId = LoggedInEmployee.DepartmentId;
                MonthlyReportsObj.EmployeeId = LoggedInEmployee.EmployeeId;
                MonthlyReportsObj.UserId = ChechUserRole.RoleTypeId;


                #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //var EmployeeNameList = WetosDB.Employees.Where(a => a.ActiveFlag == null || a.ActiveFlag == true
                // && a.CompanyId == LoggedInEmployee.CompanyId && a.BranchId == LoggedInEmployee.BranchId
                // && a.EmployeeReportingId == LoggedInEmployee.EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
                var EmployeeNameList = WetosDB.SP_VwActiveEmployee(EmployeeId).Where(a => a.ActiveFlag == null || a.ActiveFlag == true
                        && a.CompanyId == LoggedInEmployee.CompanyId && a.BranchId == LoggedInEmployee.BranchId
                        && a.EmployeeReportingId == LoggedInEmployee.EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
                #endregion
                ViewBag.EmployeeList = new SelectList(EmployeeNameList, "EmployeeId", "EmployeeName").ToList();

                var MonthList = WetosDB.DropdownDatas.Where(a => a.TypeId == 7).Select(a => new { Value = a.Value, Text = a.Text }).ToList();

                ViewBag.MonthListVB = new SelectList(MonthList, " Value", "Text").ToList();

                GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.CurrentFinancialYear).FirstOrDefault();
                if (GlobalSettingObj != null)
                {
                    PopulateYearListVB();
                }
                else
                {
                    var YearList = new List<FinancialYear>();
                    ViewBag.YearListVB = new SelectList(YearList, " FinancialId", "FinancialName").ToList();

                    Information("Inconsistent Financial Year data, Please verify and try again!");
                }
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in monthly reports selection due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));
            }

            //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable START
            ExportToExcelAndPDFEnable();
            //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable END

            return View(MonthlyReportsObj);
        }

        /// <summary>
        /// Added by shalaka on 25th DEC 2017
        /// For L&T Yearly Report 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LNTYearlyReportsIndex(MonthlyReportsModel ReportModel, FormCollection fc)
        {
            MonthlyReportsModel MonthlyReportsObj = new MonthlyReportsModel();
            try
            {
                string PdfFileName = "";
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                #region GENERIC CODE ADDED BY SHRADDHA ON 17 MAR 2018

                DateTime FDate = WetosDB.DailyTransactions.OrderBy(a => a.TranDate).Select(a => a.TranDate).FirstOrDefault();
                DateTime TDate = DateTime.Now;
                string EmployeeString = string.Empty;

                string FinName = ReportModel.FinancialYearId.ToString(); //MODIFIED BY PUSHKAR ON 15 JAN 2018

                FinancialYear SelectedAttendenceYear = WetosDB.FinancialYears.Where(a => a.FinancialName == FinName).FirstOrDefault(); //MODIFIED BY PUSHKAR ON 15 JAN 2018

                //int SelectedYear = Convert.ToInt32(SelectedAttendenceYear.FinancialName);
                //int SelectedMonth = 1; // ReportModel.MonthId;

                ReportModel.FromDate = (SelectedAttendenceYear.StartDate).ToString("yyyy/MM/dd");  //FirstDayOfMonthFromDateTime(new DateTime(SelectedYear, 1, 1)).ToString("yyyy/MM/dd");
                ReportModel.ToDate = (SelectedAttendenceYear.EndDate).ToString("yyyy/MM/dd");  //LastDayOfMonthFromDateTime(new DateTime(SelectedYear, 12, 1)).ToString("yyyy/MM/dd");
                // ADDED BY MSJ ON 29 DEC 2017 END
                FDate = Convert.ToDateTime(ReportModel.FromDate);
                TDate = Convert.ToDateTime(ReportModel.ToDate);


                #region ADDED REGION BY SHRADDHA ON 17 MAR 2018
                if (ReportModel.EmployeeId == 0 || ReportModel.EmployeeId == null)
                {
                    int? CompanyId = ReportModel.CompanyId == 0 ? (int?)null : ReportModel.CompanyId;
                    int? BranchId = ReportModel.BranchId == 0 ? (int?)null : ReportModel.BranchId;
                    int? EmployeeTypeId = ReportModel.EmployeeTypeId == 0 ? (int?)null : ReportModel.EmployeeTypeId;
                    int? DepartmentId = ReportModel.DepartmentId == 0 ? (int?)null : ReportModel.DepartmentId;
                    EmployeeString = GetEmployeeString(FDate, TDate, CompanyId, BranchId, EmployeeTypeId, DepartmentId);
                }
                else
                {
                    EmployeeString = fc["EmployeeId"];
                }
                ReportModel.EmployeeString = EmployeeString;
                #endregion
                #endregion

                ReportModel.ReportFormat = Convert.ToInt32(fc["Export"]);

                switch (ReportModel.RaportSelected)
                {
                    case "1": // 9 Yearly Perfomance Report MaleWise
                        string Report9 = "Yearly Perfomance Report MaleWise";
                        ReportModel.ReportName = Report9;
                        PdfFileName = YearlyPerfomanceReportMaleWise(ReportModel, fc);
                        break;
                    case "2": // 10 Yearly Encashment Leave Report
                        string Report10 = "Yearly Encashment Leave Report";
                        ReportModel.ReportName = Report10;
                        PdfFileName = YearlyEncashmentLeaveReport(ReportModel, fc);
                        break;
                    case "3":  // 11 Yearly Leave Report
                        string Report11 = "Yearly Leave Report";
                        ReportModel.ReportName = Report11;
                        PdfFileName = YearlyLeaveReport(ReportModel, fc);
                        break;
                    case "4":  // 3 Employees Detail Leave Report
                        string Report3 = "Employees Detail Leave Report";  // Updated by Rajas on 23 MAY 2017
                        ReportModel.ReportName = Report3;
                        PdfFileName = EmployeesDetailLeaveReport(ReportModel, fc);
                        break;
                    case "5":  // 3 Employees Detail Leave Report
                        string Report12 = "Yearly Perfomance Report";  // Updated by Rajas on 23 MAY 2017
                        ReportModel.ReportName = Report12;
                        PdfFileName = YearlyPerfomanceReportGeneric(ReportModel, fc);
                        break;
                    default:
                        break;
                }

                // Added by Rajas on 23 DEC 2016 for handling null or empty PdfFileName START
                if (string.IsNullOrEmpty(PdfFileName))
                {
                    PopulateDropDown();

                    // ADDED BY MSJ ON 29 JAN 2018 START
                    var MonthList = WetosDB.DropdownDatas.Where(a => a.TypeId == 7).Select(a => new { Value = a.Value, Text = a.Text }).ToList();

                    ViewBag.MonthListVB = new SelectList(MonthList, " Value", "Text").ToList();

                    GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.CurrentFinancialYear).FirstOrDefault();
                    if (GlobalSettingObj != null)
                    {
                        PopulateYearListVB();
                    }
                    else
                    {
                        var YearList = new List<FinancialYear>();
                        ViewBag.YearListVB = new SelectList(YearList, " FinancialId", "FinancialName").ToList();

                        Information("Inconsistent Financial Year data, Please verify and try again!");
                    }
                    // ADDED BY MSJ ON 29 JAN 2018 END

                    ModelState.AddModelError("Error", "Please try again");

                    //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable START
                    ExportToExcelAndPDFEnable();
                    //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable END

                    return View(ReportModel);
                }
                else
                {
                    // Added by Rajas on 14 MARCh 2017
                    if (ReportModel.ReportFormat == 2)  // excel format
                    {
                        return File(PdfFileName, "application/xls");
                    }
                    else // PDF format
                    {
                        return File(PdfFileName, "application/pdf");
                    }

                }
                // Added by Rajas on 23 DEC 2016 for handling null or empty PdfFileName END

            }
            catch (System.Exception ex)
            {
                PopulateDropDown();

                // ADDED BY MSJ ON 29 JAN 2018 START
                var MonthList = WetosDB.DropdownDatas.Where(a => a.TypeId == 7).Select(a => new { Value = a.Value, Text = a.Text }).ToList();

                ViewBag.MonthListVB = new SelectList(MonthList, " Value", "Text").ToList();

                GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.CurrentFinancialYear).FirstOrDefault();
                if (GlobalSettingObj != null)
                {
                    PopulateYearListVB();
                }
                else
                {
                    var YearList = new List<FinancialYear>();
                    ViewBag.YearListVB = new SelectList(YearList, " FinancialId", "FinancialName").ToList();

                    Information("Inconsistent Financial Year data, Please verify and try again!");
                }
                // ADDED BY MSJ ON 29 JAN 2018 END

                //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable START
                ExportToExcelAndPDFEnable();
                //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable END

                AddAuditTrail(ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                return View(ReportModel);
            }


        }


        #region LNT YEARLY REPORTS FOR HR

        /// <summary>
        /// Added by shalaka on 25th DEC 2017
        /// For L&T Yearly Report 
        /// </summary>
        /// <returns></returns>
        public ActionResult LNTYearlyReportsIndexHR()
        {
            MonthlyReportsModel MonthlyReportsObj = new MonthlyReportsModel();
            try
            {
                PopulateDropDown();//code added by shraddha on 18 jan 2018

                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);

                Employee LoggedInEmployee = WetosDB.Employees.Where(a => a.EmployeeId == EmployeeId).FirstOrDefault();

                UserRole ChechUserRole = WetosDB.UserRoles.Where(a => a.EmployeeId == EmployeeId).FirstOrDefault();

                //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO HANDLE NULL OBJECT REFERECE ERROR FOR LoggedInEmployee OBJ AND ChechUserRole OBJ START
                if (LoggedInEmployee == null)
                {
                    LoggedInEmployee = new Employee();
                }
                if (ChechUserRole == null)
                {
                    ChechUserRole = new UserRole();
                }
                //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO HANDLE NULL OBJECT REFERECE ERROR FOR LoggedInEmployee OBJ AND ChechUserRole OBJ END

                //MonthlyReportsObj.CompanyId = LoggedInEmployee.CompanyId;
                //MonthlyReportsObj.CompanyId = 0;
                //MonthlyReportsObj.BranchId = LoggedInEmployee.BranchId;
                //MonthlyReportsObj.DepartmentId = LoggedInEmployee.DepartmentId;
                //MonthlyReportsObj.EmployeeId = LoggedInEmployee.EmployeeId;
                //MonthlyReportsObj.UserId = ChechUserRole.RoleTypeId;



                #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //var EmployeeNameList = WetosDB.Employees.Where(a => a.ActiveFlag == null || a.ActiveFlag == true
                //&& a.CompanyId == LoggedInEmployee.CompanyId && a.BranchId == LoggedInEmployee.BranchId
                //&& a.EmployeeReportingId == LoggedInEmployee.EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
                var EmployeeNameList = WetosDB.SP_VwActiveEmployee(EmployeeId).Where(a => a.ActiveFlag == null || a.ActiveFlag == true
                        && a.CompanyId == LoggedInEmployee.CompanyId && a.BranchId == LoggedInEmployee.BranchId
                        && a.EmployeeReportingId == LoggedInEmployee.EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
                #endregion

                ViewBag.EmployeeList = new SelectList(EmployeeNameList, "EmployeeId", "EmployeeName").ToList();

                var MonthList = WetosDB.DropdownDatas.Where(a => a.TypeId == 7).Select(a => new { Value = a.Value, Text = a.Text }).ToList();

                ViewBag.MonthListVB = new SelectList(MonthList, " Value", "Text").ToList();

                GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.CurrentFinancialYear).FirstOrDefault();
                if (GlobalSettingObj != null)
                {
                    PopulateYearListVB();
                }
                else
                {
                    var YearList = new List<FinancialYear>();
                    ViewBag.YearListVB = new SelectList(YearList, " FinancialId", "FinancialName").ToList();

                    Information("Inconsistent Financial Year data, Please verify and try again!");
                }
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in monthly reports selection due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));
            }

            //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable START
            ExportToExcelAndPDFEnable();
            //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable END

            return View(MonthlyReportsObj);
        }

        /// <summary>
        /// Added by shalaka on 25th DEC 2017
        /// For L&T Yearly Report 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LNTYearlyReportsIndexHR(MonthlyReportsModel ReportModel, FormCollection fc)
        {
            MonthlyReportsModel MonthlyReportsObj = new MonthlyReportsModel();
            try
            {
                //ReportModel.CompanyId = fc["CompanyId"].ToString() == null ? (int?)null : Convert.ToInt32(fc["CompanyId"].ToString());
                //ReportModel.BranchId = fc["BranchId"].ToString() == null ? (int?)null : Convert.ToInt32(fc["BranchId"].ToString());
                //ReportModel.DepartmentId = fc["DepartmentId"].ToString() == null ? (int?)null : Convert.ToInt32(fc["DepartmentId"].ToString());
                //ReportModel.EmployeeTypeId = fc["EmployeeTypeId"].ToString() == null ? (int?)null : Convert.ToInt32(fc["EmployeeTypeId"].ToString());

                string PdfFileName = "";

                #region GENERIC CODE ADDED BY SHRADDHA ON 17 MAR 2018

                DateTime FDate = WetosDB.DailyTransactions.OrderBy(a => a.TranDate).Select(a => a.TranDate).FirstOrDefault();
                DateTime TDate = DateTime.Now;
                string EmployeeString = string.Empty;

                string FinName = ReportModel.FinancialYearId.ToString(); //MODIFIED BY PUSHKAR ON 15 JAN 2018

                FinancialYear SelectedAttendenceYear = WetosDB.FinancialYears.Where(a => a.FinancialName == FinName).FirstOrDefault(); //MODIFIED BY PUSHKAR ON 15 JAN 2018

                ReportModel.FromDate = (SelectedAttendenceYear.StartDate).ToString("yyyy/MM/dd");  //FirstDayOfMonthFromDateTime(new DateTime(SelectedYear, 1, 1)).ToString("yyyy/MM/dd");
                ReportModel.ToDate = (SelectedAttendenceYear.EndDate).ToString("yyyy/MM/dd");  //LastDayOfMonthFromDateTime(new DateTime(SelectedYear, 12, 1)).ToString("yyyy/MM/dd");

                FDate = Convert.ToDateTime(ReportModel.FromDate);
                TDate = Convert.ToDateTime(ReportModel.ToDate);


                #region ADDED REGION BY SHRADDHA ON 17 MAR 2018
                if (ReportModel.EmployeeId == 0 || ReportModel.EmployeeId == null)
                {
                    int? CompanyId = ReportModel.CompanyId == 0 ? (int?)null : ReportModel.CompanyId;
                    int? BranchId = ReportModel.BranchId == 0 ? (int?)null : ReportModel.BranchId;
                    int? EmployeeTypeId = ReportModel.EmployeeTypeId == 0 ? (int?)null : ReportModel.EmployeeTypeId;
                    int? DepartmentId = ReportModel.DepartmentId == 0 ? (int?)null : ReportModel.DepartmentId;
                    EmployeeString = GetEmployeeString(FDate, TDate, CompanyId, BranchId, EmployeeTypeId, DepartmentId);
                }
                else
                {
                    EmployeeString = fc["EmployeeId"];
                }
                ReportModel.EmployeeString = EmployeeString;
                #endregion
                #endregion

                ReportModel.ReportFormat = Convert.ToInt32(fc["Export"]);

                switch (ReportModel.RaportSelected)
                {
                    case "1": // 9 Yearly Perfomance Report MaleWise
                        string Report9 = "Yearly Perfomance Report MaleWise";
                        ReportModel.ReportName = Report9;
                        PdfFileName = YearlyPerfomanceReportMaleWise(ReportModel, fc);
                        break;
                    case "2": // 10 Yearly Encashment Leave Report
                        string Report10 = "Yearly Encashment Leave Report";
                        ReportModel.ReportName = Report10;
                        PdfFileName = YearlyEncashmentLeaveReport(ReportModel, fc);
                        break;
                    case "3":  // 11 Yearly Leave Report
                        string Report11 = "Yearly Leave Report";
                        ReportModel.ReportName = Report11;
                        PdfFileName = YearlyLeaveReport(ReportModel, fc);
                        break;
                    case "4":  // 3 Employees Detail Leave Report
                        string Report3 = "Employees Detail Leave Report";  // Updated by Rajas on 23 MAY 2017
                        ReportModel.ReportName = Report3;
                        PdfFileName = EmployeesDetailLeaveReport(ReportModel, fc);
                        break;
                    case "5":  // 3 Employees Detail Leave Report
                        string Report12 = "Yearly Perfomance Report";  // Updated by Rajas on 23 MAY 2017
                        ReportModel.ReportName = Report12;
                        PdfFileName = YearlyPerfomanceReportGeneric(ReportModel, fc);
                        break;
                    default:
                        break;
                }

                // Added by Rajas on 23 DEC 2016 for handling null or empty PdfFileName START
                if (string.IsNullOrEmpty(PdfFileName))
                {
                    PopulateDropDown();

                    // ADDED BY MSJ ON 29 JAN 2018 START
                    var MonthList = WetosDB.DropdownDatas.Where(a => a.TypeId == 7).Select(a => new { Value = a.Value, Text = a.Text }).ToList();

                    ViewBag.MonthListVB = new SelectList(MonthList, " Value", "Text").ToList();

                    GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.CurrentFinancialYear).FirstOrDefault();
                    if (GlobalSettingObj != null)
                    {
                        PopulateYearListVB();
                    }
                    else
                    {
                        var YearList = new List<FinancialYear>();
                        ViewBag.YearListVB = new SelectList(YearList, " FinancialId", "FinancialName").ToList();

                        Information("Inconsistent Financial Year data, Please verify and try again!");
                    }
                    // ADDED BY MSJ ON 29 JAN 2018 END

                    ModelState.AddModelError("Error", "Please try again");

                    //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable START
                    ExportToExcelAndPDFEnable();
                    //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable END

                    return View(ReportModel);
                }
                else
                {
                    // Added by Rajas on 14 MARCh 2017
                    if (ReportModel.ReportFormat == 2)  // excel format
                    {
                        return File(PdfFileName, "application/xls");
                    }
                    else // PDF format
                    {
                        return File(PdfFileName, "application/pdf");
                    }

                }
                // Added by Rajas on 23 DEC 2016 for handling null or empty PdfFileName END

            }
            catch (System.Exception ex)
            {
                PopulateDropDown();

                // ADDED BY MSJ ON 29 JAN 2018 START
                var MonthList = WetosDB.DropdownDatas.Where(a => a.TypeId == 7).Select(a => new { Value = a.Value, Text = a.Text }).ToList();

                ViewBag.MonthListVB = new SelectList(MonthList, " Value", "Text").ToList();

                GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.CurrentFinancialYear).FirstOrDefault();
                if (GlobalSettingObj != null)
                {
                    PopulateYearListVB();
                }
                else
                {
                    var YearList = new List<FinancialYear>();
                    ViewBag.YearListVB = new SelectList(YearList, " FinancialId", "FinancialName").ToList();

                    Information("Inconsistent Financial Year data, Please verify and try again!");
                }
                // ADDED BY MSJ ON 29 JAN 2018 END

                //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable START
                ExportToExcelAndPDFEnable();
                //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable END

                AddAuditTrail(ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                return View(ReportModel);
            }


        }


        #endregion

        /// <summary>
        /// Custom Reports Index
        /// </summary>
        /// <returns></returns>
        public ActionResult LNTMonthlyReportsIndex()
        {
            MonthlyReportsModel MonthlyReportsObj = new MonthlyReportsModel();

            try
            {
                PopulateDropDown();//code added by shraddha on 18 jan 2018

                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);

                Employee LoggedInEmployee = WetosDB.Employees.Where(a => a.EmployeeId == EmployeeId).FirstOrDefault();

                UserRole ChechUserRole = WetosDB.UserRoles.Where(a => a.EmployeeId == EmployeeId).FirstOrDefault();

                //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO HANDLE NULL OBJECT REFERECE ERROR FOR LoggedInEmployee OBJ AND ChechUserRole OBJ START
                if (LoggedInEmployee == null)
                {
                    LoggedInEmployee = new Employee();
                }
                if (ChechUserRole == null)
                {
                    ChechUserRole = new UserRole();
                }
                //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO HANDLE NULL OBJECT REFERECE ERROR FOR LoggedInEmployee OBJ AND ChechUserRole OBJ END

                // COMMENTED BY MSJ ON 23 JAn 2018 START
                MonthlyReportsObj.CompanyId = LoggedInEmployee.CompanyId;
                MonthlyReportsObj.BranchId = LoggedInEmployee.BranchId;
                MonthlyReportsObj.DepartmentId = LoggedInEmployee.DepartmentId;
                MonthlyReportsObj.EmployeeId = LoggedInEmployee.EmployeeId;
                MonthlyReportsObj.UserId = ChechUserRole.RoleTypeId;
                // COMMENTED BY MSJ ON 23 JAn 2018 START


                #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //var EmployeeNameList = WetosDB.Employees.Where(a => a.ActiveFlag == null || a.ActiveFlag == true
                //&& a.CompanyId == LoggedInEmployee.CompanyId && a.BranchId == LoggedInEmployee.BranchId
                //&& a.EmployeeReportingId == LoggedInEmployee.EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
                var EmployeeNameList = WetosDB.SP_VwActiveEmployee(EmployeeId).Where(a => a.ActiveFlag == null || a.ActiveFlag == true
                        && a.CompanyId == LoggedInEmployee.CompanyId && a.BranchId == LoggedInEmployee.BranchId
                        && a.EmployeeReportingId == LoggedInEmployee.EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
                #endregion
                ViewBag.EmployeeList = new SelectList(EmployeeNameList, "EmployeeId", "EmployeeName").ToList();


                DailyTransactionAttendanceStatus DailyTransactionAttendanceStatusObj = new DailyTransactionAttendanceStatus();

                var MonthList = WetosDB.DropdownDatas.Where(a => a.TypeId == 7).Select(a => new { Value = a.Value, Text = a.Text }).ToList();

                ViewBag.MonthListVB = new SelectList(MonthList, " Value", "Text").ToList();

                GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.CurrentFinancialYear).FirstOrDefault();
                if (GlobalSettingObj != null)
                {
                    PopulateYearListVB();
                }
                else
                {
                    var YearList = new List<FinancialYear>();
                    ViewBag.YearListVB = new SelectList(YearList, " FinancialId", "FinancialName").ToList();

                    Information("Inconsistent Financial Year data, Please verify and try again!");
                }

            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in monthly reports selection due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));
            }

            //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable START
            ExportToExcelAndPDFEnable();
            //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable END

            return View(MonthlyReportsObj);
        }

        #region LNT MONTHLY REPORT FOR HR CODE ADDED BY SHRADDHA ON 18 JAN 2018
        /// <summary>
        /// Custom Reports Index
        /// MONTHLY REPORTS FOR HR
        /// CODE ADDED BY SHRADDHA ON 18 JAN 2018
        public ActionResult LNTMonthlyReportsIndexHR()
        {
            MonthlyReportsModel MonthlyReportsObj = new MonthlyReportsModel();

            try
            {
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);

                Employee LoggedInEmployee = WetosDB.Employees.Where(a => a.EmployeeId == EmployeeId).FirstOrDefault();

                UserRole ChechUserRole = WetosDB.UserRoles.Where(a => a.EmployeeId == EmployeeId).FirstOrDefault();

                PopulateDropDown();

                //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO HANDLE NULL OBJECT REFERECE ERROR FOR LoggedInEmployee OBJ AND ChechUserRole OBJ START
                if (LoggedInEmployee == null)
                {
                    LoggedInEmployee = new Employee();
                }
                if (ChechUserRole == null)
                {
                    ChechUserRole = new UserRole();
                }
                //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO HANDLE NULL OBJECT REFERECE ERROR FOR LoggedInEmployee OBJ AND ChechUserRole OBJ END

                //MonthlyReportsObj.CompanyId = LoggedInEmployee.CompanyId;
                //MonthlyReportsObj.BranchId = LoggedInEmployee.BranchId;
                //MonthlyReportsObj.DepartmentId = LoggedInEmployee.DepartmentId;
                //MonthlyReportsObj.EmployeeId = LoggedInEmployee.EmployeeId;
                //MonthlyReportsObj.UserId = ChechUserRole.RoleTypeId;

                //var EmployeeNameList = WetosDB.Employees.Where(a => a.ActiveFlag == null || a.ActiveFlag == true
                //        && a.CompanyId == LoggedInEmployee.CompanyId && a.BranchId == LoggedInEmployee.BranchId
                //        && a.EmployeeReportingId == LoggedInEmployee.EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " " + a.FirstName + " " + a.LastName }).ToList();
                //ViewBag.EmployeeList = new SelectList(EmployeeNameList, "EmployeeId", "EmployeeName").ToList();


                DailyTransactionAttendanceStatus DailyTransactionAttendanceStatusObj = new DailyTransactionAttendanceStatus();

                var MonthList = WetosDB.DropdownDatas.Where(a => a.TypeId == 7).Select(a => new { Value = a.Value, Text = a.Text }).ToList();

                ViewBag.MonthListVB = new SelectList(MonthList, " Value", "Text").ToList();

                GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.CurrentFinancialYear).FirstOrDefault();
                if (GlobalSettingObj != null)
                {
                    PopulateYearListVB();
                }
                else
                {
                    var YearList = new List<FinancialYear>();
                    ViewBag.YearListVB = new SelectList(YearList, " FinancialId", "FinancialName").ToList();

                    Information("Inconsistent Financial Year data, Please verify and try again!");
                }

            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in monthly reports selection due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));
            }

            //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable START
            ExportToExcelAndPDFEnable();
            //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable END

            return View(MonthlyReportsObj);
        }


        /// <summary>
        /// CustomReportsIndex - Validation added for from date, to date and branch        
        /// </summary>
        /// <param name="ReportModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LNTMonthlyReportsIndexHR(MonthlyReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 7 FEB 2017
        {
            //if (ModelState.IsValid)
            //{
            try
            {
                #region GENERIC CODE ADDED BY SHRADDHA ON 17 MAR 2018
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                DateTime FDate = WetosDB.DailyTransactions.OrderBy(a => a.TranDate).Select(a => a.TranDate).FirstOrDefault();
                DateTime TDate = DateTime.Now;

                string EmployeeString = string.Empty;

                if ((!string.IsNullOrEmpty(ReportModel.ToDate)) && (!string.IsNullOrEmpty(ReportModel.FromDate)))
                {
                    //CODE ADDED FOR DATE RANGE VALIDATION BY SHRADDHA ON 23 FEB 2018 START
                    FDate = Convert.ToDateTime(ReportModel.FromDate);
                    TDate = Convert.ToDateTime(ReportModel.ToDate);

                    if ((TDate - FDate).TotalDays > 31)
                    {
                        PopulateDropDown();
                        ModelState.AddModelError("Error", "Please select date range less than or equal to 31 days.");
                        Error("Please select date range less than or equal to 31 days.");
                        ExportToExcelAndPDFEnable();
                        return View(ReportModel);
                    }
                }

                string opt = fc["opt"];
                if (opt == "MonthWise")
                {
                    ReportModel.FromDate = null;
                    ReportModel.ToDate = null;
                }
                else
                {
                    ReportModel.FinancialYearId = 0;
                    ReportModel.MonthId = 0;
                }
                if (ReportModel.FromDate == null && ReportModel.ToDate == null)
                {
                    if (opt == "MonthWise")
                    {
                        int SelectedYear = Convert.ToInt32(ReportModel.FinancialYearId); // MODIFIED BY MSJ ON 08 JAN 2018 Convert.ToInt32(SelectedAttendenceYear.FinancialName);

                        int SelectedMonth = Convert.ToInt32(ReportModel.MonthId);

                        ReportModel.FromDate = FirstDayOfMonthFromDateTime(new DateTime(SelectedYear, SelectedMonth, 1)).ToString("yyyy/MM/dd");
                        ReportModel.ToDate = LastDayOfMonthFromDateTime(new DateTime(SelectedYear, SelectedMonth, 1)).ToString("yyyy/MM/dd");

                        FDate = FirstDayOfMonthFromDateTime(new DateTime(SelectedYear, SelectedMonth, 1));
                        TDate = LastDayOfMonthFromDateTime(new DateTime(SelectedYear, SelectedMonth, 1));

                    }
                }
                else
                {
                    FDate = Convert.ToDateTime(ReportModel.FromDate);
                    TDate = Convert.ToDateTime(ReportModel.ToDate);
                }

                #region ADDED REGION BY SHRADDHA ON 17 MAR 2018
                if (ReportModel.EmployeeId == 0 || ReportModel.EmployeeId == null)
                {
                    int? CompanyId = ReportModel.CompanyId == 0 ? (int?)null : ReportModel.CompanyId;
                    int? BranchId = ReportModel.BranchId == 0 ? (int?)null : ReportModel.BranchId;
                    int? EmployeeTypeId = ReportModel.EmployeeTypeId == 0 ? (int?)null : ReportModel.EmployeeTypeId;
                    int? DepartmentId = ReportModel.DepartmentId == 0 ? (int?)null : ReportModel.DepartmentId;
                    EmployeeString = GetEmployeeString(FDate, TDate, CompanyId, BranchId, EmployeeTypeId, DepartmentId);
                }
                else
                {
                    EmployeeString = fc["EmployeeId"];
                }
                ReportModel.EmployeeString = EmployeeString;
                #endregion
                #endregion

                string PdfFileName = "";

                ReportModel.ReportFormat = Convert.ToInt32(fc["Export"]);

                switch (ReportModel.RaportSelected)
                {
                    case "1": // 1 Monthly Performance Report - TimeCardReport
                        string Report1 = "Monthly Performance Report";
                        ReportModel.ReportName = Report1;
                        PdfFileName = MonthlyPerformanceReport(ReportModel, fc);
                        break;
                    //case "2": // 2 DailyLong Absnteeism Report
                    //    string Report2 = "DailyLong Absnteeism Report";
                    //    ReportModel.ReportName = Report2;
                    //    PdfFileName = DailyLongAbsnteeismReport(ReportModel, fc);
                    //    break;
                    case "3":  // 3 Employees Detail Leave Report
                        string Report3 = "Employees Detail Leave Report";  // Updated by Rajas on 23 MAY 2017
                        ReportModel.ReportName = Report3;
                        PdfFileName = EmployeesDetailLeaveReport(ReportModel, fc);
                        break;
                    case "4":  // 4 Monthly Attendance Report
                        string Report4 = "Monthly Attendance Report";
                        ReportModel.ReportName = Report4;
                        PdfFileName = MonthlyAttendanceReport(ReportModel, fc); // Updated by Rajas on 7 FEB 2017
                        break;
                    case "5": // 6 Monthly Left Report
                        string Report6 = "Monthly Left Report";
                        ReportModel.ReportName = Report6;
                        PdfFileName = MonthlyLeftReport(ReportModel, fc); //L&T report done
                        break;
                    case "6": // 7 Monthly Addition List Report
                        string Report7 = "Monthly Addition List Report";
                        ReportModel.ReportName = Report7;
                        PdfFileName = MonthlyAdditionListReport(ReportModel, fc); //L&T report done
                        break;
                    case "7": // 8 Monthly Salary Output Report
                        string Report8 = "Monthly Salary Output Report";
                        ReportModel.ReportName = Report8;
                        PdfFileName = MonthlySalaryOutputReport(ReportModel, fc);
                        break;
                    case "8": // 8 Monthly Salary Output Report
                        string Report9 = "Monthly Loss Of Pay Report";
                        ReportModel.ReportName = Report9;
                        PdfFileName = MonthlyLossofPayReport(ReportModel, fc);
                        break;
                    case "9": // 8 Monthly Salary Output Report
                        string Report10 = "Monthly Late Early Report"; //"Monthly Loss Of Pay Report"; // ADDED BY MSJ ON 05 MARCH 2018
                        ReportModel.ReportName = Report10;
                        PdfFileName = MonthlyLateEarlyReport(ReportModel, fc);
                        break;
                    case "10": // 8 Monthly Salary Output Report
                        string Report11 = "Monthly Salary Absentism Report";
                        ReportModel.ReportName = Report11;
                        PdfFileName = MonthlySalaryAbsentismReport(ReportModel, fc);
                        break;
                    case "11": // 8 Monthly Salary Output Report
                        string Report12 = "Monthly Salary Shift Allowance Report "; //"Monthly Salary Absentism Report"; // ADDED BY MSJ ON 05 MARCH 2018
                        ReportModel.ReportName = Report12;
                        PdfFileName = MonthlySalaryShiftAllowanceReport(ReportModel, fc);
                        break;
                    // ADDED BY MSJ ON 19 MARCH 2018 START
                    case "12":
                        string Report13 = "Family Details Report";
                        ReportModel.ReportName = Report13;
                        PdfFileName = FamilyDetailsReport(ReportModel, fc);
                        break;
                    // ADDED BY MSJ ON 19 MARCH 2018 END
                    case "13":
                        string Report14 = "Monthly Loss of Pay For Eff Date Report";
                        ReportModel.ReportName = Report14;
                        PdfFileName = MonthlyLOPForEffDateReport(ReportModel, fc);
                        break;

                    case "14":
                        string Report15 = "Monthly Total Late Early Report";
                        ReportModel.ReportName = Report15;
                        PdfFileName = MonthlyTotalLateEarly(ReportModel, fc);
                        break;

                    default:
                        break;
                }

                // Added by Rajas on 23 DEC 2016 for handling null or empty PdfFileName START
                if (string.IsNullOrEmpty(PdfFileName))
                {
                    PopulateDropDown();

                    // ADDED BY MSJ ON 29 JAN 2018 START
                    var MonthList = WetosDB.DropdownDatas.Where(a => a.TypeId == 7).Select(a => new { Value = a.Value, Text = a.Text }).ToList();

                    ViewBag.MonthListVB = new SelectList(MonthList, " Value", "Text").ToList();

                    GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.CurrentFinancialYear).FirstOrDefault();
                    if (GlobalSettingObj != null)
                    {
                        PopulateYearListVB();
                    }
                    else
                    {
                        var YearList = new List<FinancialYear>();
                        ViewBag.YearListVB = new SelectList(YearList, " FinancialId", "FinancialName").ToList();

                        Information("Inconsistent Financial Year data, Please verify and try again!");
                    }
                    // ADDED BY MSJ ON 29 JAN 2018 END

                    ModelState.AddModelError("Error", "Please try again");

                    //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable START
                    ExportToExcelAndPDFEnable();
                    //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable END

                    return View(ReportModel);
                }
                else
                {
                    // Added by Rajas on 14 MARCh 2017
                    if (ReportModel.ReportFormat == 2)  // excel format
                    {
                        return File(PdfFileName, "application/xls");
                    }
                    else // PDF format
                    {
                        return File(PdfFileName, "application/pdf");
                    }

                }
                // Added by Rajas on 23 DEC 2016 for handling null or empty PdfFileName END

            }
            catch (System.Exception ex)
            {
                PopulateDropDown();

                // ADDED BY MSJ ON 29 JAN 2018 START
                var MonthList = WetosDB.DropdownDatas.Where(a => a.TypeId == 7).Select(a => new { Value = a.Value, Text = a.Text }).ToList();

                ViewBag.MonthListVB = new SelectList(MonthList, " Value", "Text").ToList();

                GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.CurrentFinancialYear).FirstOrDefault();
                if (GlobalSettingObj != null)
                {
                    PopulateYearListVB();
                }
                else
                {
                    var YearList = new List<FinancialYear>();
                    ViewBag.YearListVB = new SelectList(YearList, " FinancialId", "FinancialName").ToList();

                    Information("Inconsistent Financial Year data, Please verify and try again!");
                }
                // ADDED BY MSJ ON 29 JAN 2018 END

                //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable START
                ExportToExcelAndPDFEnable();
                //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable END

                AddAuditTrail(ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                return View(ReportModel);
            }
        }
        #endregion

        /// <summary>
        /// CustomReportsIndex - Validation added for from date, to date and branch        
        /// </summary>
        /// <param name="ReportModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LNTMonthlyReportsIndex(MonthlyReportsModel ReportModel, FormCollection fc) // Updated by Rajas on 7 FEB 2017
        {
            //if (ModelState.IsValid)
            //{
            try
            {
                #region GENERIC CODE ADDED BY SHRADDHA ON 17 MAR 2018
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                DateTime FDate = WetosDB.DailyTransactions.OrderBy(a => a.TranDate).Select(a => a.TranDate).FirstOrDefault();
                DateTime TDate = DateTime.Now;
                string EmployeeString = string.Empty;

                if ((!string.IsNullOrEmpty(ReportModel.ToDate)) && (!string.IsNullOrEmpty(ReportModel.FromDate)))
                {
                    //CODE ADDED FOR DATE RANGE VALIDATION BY SHRADDHA ON 23 FEB 2018 START
                    FDate = Convert.ToDateTime(ReportModel.FromDate);
                    TDate = Convert.ToDateTime(ReportModel.ToDate);

                    if ((TDate - FDate).TotalDays > 31)
                    {
                        PopulateDropDown();
                        ModelState.AddModelError("Error", "Please select date range less than or equal to 31 days.");
                        Error("Please select date range less than or equal to 31 days.");
                        ExportToExcelAndPDFEnable();
                        return View(ReportModel);
                    }
                }

                string opt = fc["opt"];
                if (opt == "MonthWise")
                {
                    ReportModel.FromDate = null;
                    ReportModel.ToDate = null;
                }
                else
                {
                    ReportModel.FinancialYearId = 0;
                    ReportModel.MonthId = 0;
                }
                if (ReportModel.FromDate == null && ReportModel.ToDate == null)
                {
                    int SelectedYear = Convert.ToInt32(ReportModel.FinancialYearId); // MODIFIED BY MSJ ON 08 JAN 2018 Convert.ToInt32(SelectedAttendenceYear.FinancialName);

                    int SelectedMonth = Convert.ToInt32(ReportModel.MonthId);

                    ReportModel.FromDate = FirstDayOfMonthFromDateTime(new DateTime(SelectedYear, SelectedMonth, 1)).ToString("yyyy/MM/dd");
                    ReportModel.ToDate = LastDayOfMonthFromDateTime(new DateTime(SelectedYear, SelectedMonth, 1)).ToString("yyyy/MM/dd");

                    FDate = FirstDayOfMonthFromDateTime(new DateTime(SelectedYear, SelectedMonth, 1));
                    TDate = LastDayOfMonthFromDateTime(new DateTime(SelectedYear, SelectedMonth, 1));
                }
                else
                {
                    FDate = Convert.ToDateTime(ReportModel.FromDate);
                    TDate = Convert.ToDateTime(ReportModel.ToDate);
                }

                #region ADDED REGION BY SHRADDHA ON 17 MAR 2018
                if (ReportModel.EmployeeId == 0 || ReportModel.EmployeeId == null)
                {
                    int? CompanyId = ReportModel.CompanyId == 0 ? (int?)null : ReportModel.CompanyId;
                    int? BranchId = ReportModel.BranchId == 0 ? (int?)null : ReportModel.BranchId;
                    int? EmployeeTypeId = ReportModel.EmployeeTypeId == 0 ? (int?)null : ReportModel.EmployeeTypeId;
                    int? DepartmentId = ReportModel.DepartmentId == 0 ? (int?)null : ReportModel.DepartmentId;
                    EmployeeString = GetEmployeeString(FDate, TDate, CompanyId, BranchId, EmployeeTypeId, DepartmentId);
                }
                else
                {
                    EmployeeString = fc["EmployeeId"];
                }
                ReportModel.EmployeeString = EmployeeString;
                #endregion
                #endregion

                string PdfFileName = "";

                ReportModel.ReportFormat = Convert.ToInt32(fc["Export"]);

                switch (ReportModel.RaportSelected)
                {
                    case "1": // 1 Monthly Performance Report - TimeCardReport
                        string Report1 = "Monthly Performance Report";
                        ReportModel.ReportName = Report1;
                        PdfFileName = MonthlyPerformanceReport(ReportModel, fc);
                        break;
                    //case "2": // 2 DailyLong Absnteeism Report
                    //    string Report2 = "DailyLong Absnteeism Report";
                    //    ReportModel.ReportName = Report2;
                    //    PdfFileName = DailyLongAbsnteeismReport(ReportModel, fc);
                    //    break;
                    case "3":  // 3 Employees Detail Leave Report
                        string Report3 = "Employees Detail Leave Report";  // Updated by Rajas on 23 MAY 2017
                        ReportModel.ReportName = Report3;
                        PdfFileName = EmployeesDetailLeaveReport(ReportModel, fc);
                        break;
                    case "4":  // 4 Monthly Attendance Report
                        string Report4 = "Monthly Attendance Report";
                        ReportModel.ReportName = Report4;
                        PdfFileName = MonthlyAttendanceReport(ReportModel, fc); // Updated by Rajas on 7 FEB 2017
                        break;
                    case "5": // 6 Monthly Left Report
                        string Report6 = "Monthly Left Report";
                        ReportModel.ReportName = Report6;
                        PdfFileName = MonthlyLeftReport(ReportModel, fc); //L&T report done
                        break;
                    case "6": // 7 Monthly Addition List Report
                        string Report7 = "Monthly Addition List Report";
                        ReportModel.ReportName = Report7;
                        PdfFileName = MonthlyAdditionListReport(ReportModel, fc); //L&T report done
                        break;
                    case "7": // 8 Monthly Salary Output Report
                        string Report8 = "Monthly Salary Output Report";
                        ReportModel.ReportName = Report8;
                        PdfFileName = MonthlySalaryOutputReport(ReportModel, fc);
                        break;
                    case "8": // 8 Monthly Salary Output Report
                        string Report9 = "Monthly Loss Of Pay Report";
                        ReportModel.ReportName = Report9;
                        PdfFileName = MonthlyLossofPayReport(ReportModel, fc);
                        break;
                    case "9": // 8 Monthly Salary Output Report
                        string Report10 = "Monthly Late Early Report";
                        ReportModel.ReportName = Report10;
                        PdfFileName = MonthlyLateEarlyReport(ReportModel, fc);
                        break;
                    case "10": // 8 Monthly Salary Output Report
                        string Report11 = "Monthly Salary Absentism Report";
                        ReportModel.ReportName = Report11;
                        PdfFileName = MonthlySalaryAbsentismReport(ReportModel, fc);
                        break;
                    case "11": // 8 Monthly Salary Output Report
                        string Report12 = "Monthly Salary Shift Allowance Report";
                        ReportModel.ReportName = Report12;
                        PdfFileName = MonthlySalaryShiftAllowanceReport(ReportModel, fc);
                        break;
                    // ADDED BY MSJ ON 19 MARCH 2018 START
                    case "12":
                        string Report13 = "Family Details Report";
                        ReportModel.ReportName = Report13;
                        PdfFileName = FamilyDetailsReport(ReportModel, fc);
                        break;
                    // ADDED BY MSJ ON 19 MARCH 2018 END
                    case "13":
                        string Report14 = "Monthly Loss of Pay For Eff Date Report";
                        ReportModel.ReportName = Report14;
                        PdfFileName = MonthlyLOPForEffDateReport(ReportModel, fc);
                        break;

                    case "14":
                        string Report15 = "Monthly Total Late Early Report";
                        ReportModel.ReportName = Report15;
                        PdfFileName = MonthlyTotalLateEarly(ReportModel, fc);
                        break;

                    default:
                        break;
                }

                // Added by Rajas on 23 DEC 2016 for handling null or empty PdfFileName START
                if (string.IsNullOrEmpty(PdfFileName))
                {
                    PopulateDropDown();

                    // ADDED BY MSJ ON 29 JAN 2018 START
                    var MonthList = WetosDB.DropdownDatas.Where(a => a.TypeId == 7).Select(a => new { Value = a.Value, Text = a.Text }).ToList();

                    ViewBag.MonthListVB = new SelectList(MonthList, " Value", "Text").ToList();

                    GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.CurrentFinancialYear).FirstOrDefault();
                    if (GlobalSettingObj != null)
                    {
                        PopulateYearListVB();
                    }
                    else
                    {
                        var YearList = new List<FinancialYear>();
                        ViewBag.YearListVB = new SelectList(YearList, " FinancialId", "FinancialName").ToList();

                        Information("Inconsistent Financial Year data, Please verify and try again!");
                    }
                    // ADDED BY MSJ ON 29 JAN 2018 END

                    ModelState.AddModelError("Error", "Please try again");

                    //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable START
                    ExportToExcelAndPDFEnable();
                    //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable END

                    return View(ReportModel);
                }
                else
                {
                    // Added by Rajas on 14 MARCh 2017
                    if (ReportModel.ReportFormat == 2)  // excel format
                    {
                        //return File(PdfFileName, "application/xls");
                        //return RedirectToAction("LNTMonthlyReportsIndex");
                        return File(PdfFileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

                    }
                    else // PDF format
                    {
                        return File(PdfFileName, "application/pdf");
                    }

                }
                // Added by Rajas on 23 DEC 2016 for handling null or empty PdfFileName END

            }
            catch (System.Exception ex)
            {
                PopulateDropDown();

                // ADDED BY MSJ ON 29 JAN 2018 START
                var MonthList = WetosDB.DropdownDatas.Where(a => a.TypeId == 7).Select(a => new { Value = a.Value, Text = a.Text }).ToList();

                ViewBag.MonthListVB = new SelectList(MonthList, " Value", "Text").ToList();

                GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.CurrentFinancialYear).FirstOrDefault();
                if (GlobalSettingObj != null)
                {
                    PopulateYearListVB();
                }
                else
                {
                    var YearList = new List<FinancialYear>();
                    ViewBag.YearListVB = new SelectList(YearList, " FinancialId", "FinancialName").ToList();

                    Information("Inconsistent Financial Year data, Please verify and try again!");
                }
                // ADDED BY MSJ ON 29 JAN 2018 END

                //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable START
                ExportToExcelAndPDFEnable();
                //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO TAKE EXPORT TO EXCEL AND EXPORT TO PDF BUTTON VISIBLE OR HIDDEN BASED ON GLOBAL SETTINGS FUNCTION CALL ExportToExcelAndPDFEnable END

                AddAuditTrail(ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                return View(ReportModel);
            }
        }

        #region DateTimeUtility

        public DateTime FirstDayOfMonthFromDateTime(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1);
        }

        public DateTime LastDayOfMonthFromDateTime(DateTime dateTime)
        {
            DateTime firstDayOfTheMonth = new DateTime(dateTime.Year, dateTime.Month, 1);
            return firstDayOfTheMonth.AddMonths(1).AddDays(-1);
        }

        #endregion

        /// <summary>
        /// CODE ADDED BY SHRADDHA ON 18 JAN 2018
        /// MADE GENERIC FUNCTION FOR YEAR VIEWBAG
        /// </summary>
        public void PopulateYearListVB()
        {
            // MODIFIED AND ADDED CODE BY MSJ ON 08 JAN 2018 START
            List<FinancialYear> FYObjList = WetosDB.FinancialYears.ToList();
            List<string> FYStrList = new List<string>();
            foreach (FinancialYear CurrentFYObj in FYObjList)
            {
                // COMMNETED BY MSJ ON 11 MAY 2020
                //if (CurrentFYObj.StartDate.Year != CurrentFYObj.EndDate.Year) // DIFF YEARS
                //{

                // ADDED BY MSJ ON 11 MAY 2020 START
                if (FYStrList.Contains(CurrentFYObj.StartDate.Year.ToString()) == false)
                {
                    FYStrList.Add(CurrentFYObj.StartDate.Year.ToString());
                }

                if (FYStrList.Contains(CurrentFYObj.EndDate.Year.ToString()) == false)
                {
                    FYStrList.Add(CurrentFYObj.EndDate.Year.ToString());
                }

                // ADDED BY MSJ ON 11 MAY 2020 END

                //}
                //else
                //{
                //    FYStrList.Add(CurrentFYObj.StartDate.Year.ToString());
                //}
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



        //private void rptGetDataset(ReportModel ReportModelObj)
        //{

        //    DataSet ds = new DataSet();
        //    ds.DataSetName = "dsNewDataSet";
        //    //string sql = "";
        //    //sql = "SELECT ID, CLIENT_ID, AGENT_ID FROM TBLMAILDELETED";
        //    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
        //    //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

        //    SqlCommand com1 = new SqlCommand("SP_MonthlyAttendanceReportDATEWISE", con);
        //    com1.CommandType = CommandType.StoredProcedure;
        //    com1.Parameters.Add(new SqlParameter("@Fdate", SqlDbType.DateTime)).Value = ReportModelObj.FromDate; //new DateTime(2013, 01, 01);
        //    com1.Parameters.Add(new SqlParameter("@Tdate", SqlDbType.DateTime)).Value = ReportModelObj.ToDate; //new DateTime(2013, 01, 01);
        //    com1.Parameters.Add(new SqlParameter("@EmpidList", SqlDbType.NVarChar)).Value = null;

        //    SqlDataAdapter da1 = new SqlDataAdapter(com1);
        //    //OdbcDataAdapter da = new OdbcDataAdapter(sql, conn);
        //    ds.GetXmlSchema();
        //    da1.Fill(ds);
        //    ds.WriteXmlSchema(Server.MapPath("~/DataSets/Ds.xsd"));
        //    ds.WriteXml(Server.MapPath("~/DataSets/Ds.xml"));
        //}

        //private DataTable getData(ReportModel ReportModelObj)
        //{
        //    DataSet dss = new DataSet();
        //    //string sql = "";
        //    //sql = "SELECT ID, CLIENT_ID, AGENT_ID FROM TBLMAILDELETED";
        //    //OdbcDataAdapter da = new OdbcDataAdapter(sql, conn)
        //    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
        //    //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);
        //    SqlCommand com1 = new SqlCommand("SP_MonthlyAttendanceReportDATEWISE", con);
        //    com1.CommandType = CommandType.StoredProcedure;
        //    com1.Parameters.Add(new SqlParameter("@Fdate", SqlDbType.DateTime)).Value = ReportModelObj.FromDate; //new DateTime(2013, 01, 01);
        //    com1.Parameters.Add(new SqlParameter("@Tdate", SqlDbType.DateTime)).Value = ReportModelObj.ToDate; //new DateTime(2013, 01, 01);

        //    SqlDataAdapter da1 = new SqlDataAdapter(com1);
        //    da1.Fill(dss);
        //    DataTable dt = dss.Tables[0];
        //    return dt;
        //}

        public string GetEmployeeString(DateTime? FDate, DateTime? TDate, int? CompanyId, int? BranchId, int? EmployeeTypeId, int? DepartmentId)
        {
            string EmployeeString = string.Empty;
            try
            {
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);

                //SP_GetEmployeeCSVForReport_Result EmployeeListCSV = WetosDB.SP_GetEmployeeCSVForReport(FDate, TDate, CompanyId,
                //            BranchId, EmployeeTypeId, DepartmentId).FirstOrDefault();
                SP_GetEmployeeCSVForReportNew_Result EmployeeListCSV = WetosDB.SP_GetEmployeeCSVForReportNew(FDate, TDate, CompanyId,
                            BranchId, EmployeeTypeId, DepartmentId, EmployeeId).FirstOrDefault();

                //SP_GetEmployeeCSVForReportNewDept_Result EmployeeListCSV = WetosDB.SP_GetEmployeeCSVForReportNewDept(FDate, TDate, CompanyId,
                //            BranchId, EmployeeTypeId, DeptStr, EmployeeId).FirstOrDefault();
                EmployeeString = EmployeeListCSV.EmployeeList;
            }
            catch (Exception ex)
            {
            }

            return EmployeeString;
        }

        public string GetEmployeeStringDept(DateTime? FDate, DateTime? TDate, int? CompanyId, int? BranchId, int? EmployeeTypeId, int? DepartmentId, string DeptStr)
        {
            int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            string EmployeeString = string.Empty;
            //SP_GetEmployeeCSVForReport_Result EmployeeListCSV = WetosDB.SP_GetEmployeeCSVForReport(FDate, TDate, CompanyId,
            //            BranchId, EmployeeTypeId, DepartmentId).FirstOrDefault();
            //SP_GetEmployeeCSVForReportNew_Result EmployeeListCSV = WetosDB.SP_GetEmployeeCSVForReportNew(FDate, TDate, CompanyId,
            //            BranchId, EmployeeTypeId, DepartmentId, EmployeeId).FirstOrDefault();

            SP_GetEmployeeCSVForReportNewDept_Result EmployeeListCSV = WetosDB.SP_GetEmployeeCSVForReportNewDept(FDate, TDate, CompanyId,
                        BranchId, EmployeeTypeId, DeptStr, EmployeeId).FirstOrDefault();
            EmployeeString = EmployeeListCSV.EmployeeList;
            return EmployeeString;
        }


        /// <summary>
        /// EmployeeMasterReport
        /// </summary>
        /// <returns></returns>
        private string FamilyDetailsReport(MonthlyReportsModel ReportModel, FormCollection fc) // ADDED MasterReportsModel,  FormCollection fc BYN MSJ ON 19 MARCH 2018 
        {
            //bool ReturnStatus = false;
            string PdfFileName = "";

            try
            {
                // ------------------------------------------------REPORT RDLC EmployeeMasterReport START-------------------------------------------
                //// PRINT START
                DataTable table5 = new DataTable();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
                //SqlDataAdapter da2 = new SqlDataAdapter(" select * from tblOurCompMaster", con);

                SqlCommand com1 = new SqlCommand("SP_GetFamilyDetailsReport", con);
                com1.CommandTimeout = 2000; // ADDED BY MSJ ON 29 JAN 2018
                com1.CommandType = CommandType.StoredProcedure;
                //com1.Parameters.Add(new SqlParameter("@date", SqlDbType.DateTime)).Value = new DateTime(2011, 05, 01);
                //com1.Parameters.Add(new SqlParameter("@empidList", SqlDbType.VarChar)).Value = "123,8";
                SqlDataAdapter da1 = new SqlDataAdapter(com1);
                da1.Fill(table5);

                // ADDED GENERIC FUNCTION FOR GENERATE REPORT FILE EXCEL AND PDF
                string GeneratedFileName = GenerateReport(ReportModel.BranchId, ReportModel.CompanyId, ReportModel.DepartmentId,
                    ReportModel.ReportFormat, ReportModel.ReportName, ReportModel.FromDate, ReportModel.ToDate, table5, "~/Reports/FamilyDetailsReport.rdlc",
                 "EmployeeMaster", "~/User_Data/download/", "FamilyDetails_Report_", ref PdfFileName);

                Response.AddHeader("Content-Disposition", "attachment; filename=" + GeneratedFileName + ";");

                //ReturnStatus = true;

            }
            catch (System.Exception)
            {
                //throw;
            }

            return PdfFileName;


            // PRINT END
            //---------------------------------------------REPORT RDLC EmployeeMasterReport END-------------------------------------------
        }


    }
}


