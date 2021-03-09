using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;
using WetosDB;

namespace WetosMVC.Controllers
{
    [SessionExpire] 
    [Authorize]
    public class WetosEmployeeManagementController : BaseController
    {
        //
        // GET: /WetosEmployeeManagement/
        

        public ActionResult Index()
        {
            // ADDED BY MSJ ON 08 DEC 2017 START

            // Updated by Rajas on 20 MARCH 2017

            #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN
            //List<SP_ActiveEmployeeInEmployeeMaster_Result> ActiveEmployeeList = new List<SP_ActiveEmployeeInEmployeeMaster_Result>();
           // List<SP_ActiveEmployeeInEmployeeMasterNew_Result> ActiveEmployeeList = new List<SP_ActiveEmployeeInEmployeeMasterNew_Result>();
            #endregion

            // Updated by Rajas on 15 JUNE 2017
            DateTime DefaultDate = new DateTime(1900, 01, 01);

            #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN
            List<SP_EmployeeListForResignationAndConfirmationNew_Result_New_Result> ActiveEmployeeList1 = new List<SP_EmployeeListForResignationAndConfirmationNew_Result_New_Result>();
            var EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            //ActiveEmployeeList = WetosDB.SP_ActiveEmployeeInEmployeeMaster().Where(a => a.Leavingdate == null).ToList();
            //ActiveEmployeeList = WetosDB.SP_ActiveEmployeeInEmployeeMasterNew(EmployeeId).Where(a => a.Leavingdate == null).ToList();
            ActiveEmployeeList1 = WetosDB.SP_EmployeeListForResignationAndConfirmationNew_Result_New(EmployeeId).Where(a => a.Leavingdate == null).ToList();
            #endregion


            //List<SP_EmployeeListForResignationAndConfirmationNew_Result> ActiveEmployeeList = WetosDB.SP_EmployeeListForResignationAndConfirmationNew().ToList();

            // ADDED BY MSJ ON 08 DEC 2017 END

            //CODE ADDED BY SHRADDHA ON 30 MAY 2017 TO GET EMPLOYEE GROUP LIST ON MANAGE EMPLOYEE
            var EmployeeGroupName = new List<EmployeeGroup>(); //WetosDB.EmployeeGroups.Select(a => new { EmployeeGroupId = a.EmployeeGroupId, EmployeeGroupName = a.EmployeeGroupName }).ToList();
            ViewBag.EmployeeGroupNameList = new SelectList(EmployeeGroupName, "EmployeeGroupId", "EmployeeGroupName").ToList();

            return View(ActiveEmployeeList1);
        }

        //[HttpPost]
        //public ActionResult GetListOfActiveEmployee()
        //{

        //   // List<Employee> ActiveEmployeeList = WetosDB.Employees.Where(a => a.ActiveFlag == true || a.ActiveFlag == null).ToList();
        //    //return PartialView(ActiveEmployeeList);
        //    return PartialView();
        //}

        public ActionResult ResignEmployee(int EmployeeId, string ResignationDate)
        {
            Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == EmployeeId).FirstOrDefault();
            EmployeeObj.ActiveFlag = false;
            DateTime ResignDate = Convert.ToDateTime(ResignationDate);
            EmployeeObj.Leavingdate = ResignDate;
            WetosDB.SaveChanges();

            //CODE ADDED BY SHRADDHA ON 31 JAN 2017 TO REMOVE RESIGN EMPLOYEE'S ENTRY FROM USER TABLE START
            User UserObj = WetosDB.Users.Where(a => a.EmployeeId == EmployeeId).FirstOrDefault();
            if (UserObj != null)
            {
                WetosDB.Users.DeleteObject(UserObj);
                WetosDB.SaveChanges();
            }
            //CODE ADDED BY SHRADDHA ON 31 JAN 2017 TO REMOVE RESIGN EMPLOYEE'S ENTRY FROM USER TABLE END

            //TO PRINT EXPERIENCE LETTER ON EXIT
            //CODE ADDED BY SHRADDHA ON 20 APR 2017

            #region
            Employee EmployeeForLetter = WetosDB.Employees.Where(a => a.EmployeeId == EmployeeId).FirstOrDefault();

            string DocumentFileName = string.Empty;

            UploadDocument UploadDocumentTblObj = WetosDB.UploadDocuments.Where(a => a.EmployeeID == EmployeeId && a.DocContentType == "Experience_Letter").FirstOrDefault();

            if (UploadDocumentTblObj == null)
            {
                // GENERATE PDF
                ExperienceLetterPrintFile(EmployeeId, ref DocumentFileName);

                UploadDocumentTblObj = new UploadDocument();

                UploadDocumentTblObj.EmployeeID = EmployeeId;

                UploadDocumentTblObj.DocContentType = "Experience_Letter";

                UploadDocumentTblObj.DocDescription = "System Generated";

                UploadDocumentTblObj.DocType = ".pdf";

                UploadDocumentTblObj.DocFolder = "Employee_Official_Documents";

                UploadDocumentTblObj.FileName = DocumentFileName;

                WetosDB.UploadDocuments.AddObject(UploadDocumentTblObj);

                WetosDB.SaveChanges();

                AddAuditTrail("System generated Experience_Letter for " + EmployeeForLetter.EmployeeCode + "_" + EmployeeForLetter.FirstName + EmployeeForLetter.LastName);

            }
            else
            {
                Information("Experience_Letter is already generated.");

                AddAuditTrail("Experience_Letter is already generated for " + EmployeeForLetter.EmployeeCode + "_" + EmployeeForLetter.FirstName + EmployeeForLetter.LastName);
            }
            #endregion

            //ExperienceLetterPrintFile(EmployeeId);

            return PartialView();
        }

        /// <summary>
        /// CODE ADDED BY SHRADDHA ON 20 APR 2017
        /// FOR RESIGNATION APPROVE FOR SELECTED EMPLOYEE
        /// <param name="EmployeeId"></param>
        /// <returns></returns>
        public ActionResult ResignationApproveEmployee(int EmployeeId, string ResignationDate)
        {
            int LoginEmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            EmployeeLifeCycle EmployeeLifeCycleObj = WetosDB.EmployeeLifeCycles.Where(a => a.EmployeeId == EmployeeId).FirstOrDefault();

            // COMMENTED BY MSJ 15 SEP 2017 //UNCOMMENTED BY SHRADDHA ON 16 SEP 2017 BECAUSE IT IS REQUIRED
            EmployeeLifeCycleObj.LastModifiedOn = DateTime.Now;
            EmployeeLifeCycleObj.LoggedInUser = LoginEmployeeId;
            EmployeeLifeCycleObj.ResignationAcceptedOrNot = true;
            // COMMENTED BY MSJ 15 SEP 2017 //UNCOMMENTED BY SHRADDHA ON 16 SEP 2017 BECAUSE IT IS REQUIRED

            EmployeeLifeCycleObj.ResignationAcceptedOn = DateTime.Now;
            WetosDB.SaveChanges();

            //TO PRINT EXPERIENCE LETTER ON EXIT
            //CODE ADDED BY SHRADDHA ON 20 APR 2017

            #region
            Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == EmployeeId).FirstOrDefault();

            string DocumentFileName = string.Empty;

            UploadDocument UploadDocumentTblObj = WetosDB.UploadDocuments.Where(a => a.EmployeeID == EmployeeId && a.DocContentType == "Relieving_Letter").FirstOrDefault();

            if (UploadDocumentTblObj == null)
            {
                // GENERATE PDF
                RelievingLetterPrintFile(EmployeeId, ref DocumentFileName);

                UploadDocumentTblObj = new UploadDocument();

                UploadDocumentTblObj.EmployeeID = EmployeeId;

                UploadDocumentTblObj.DocContentType = "Relieving_Letter";

                UploadDocumentTblObj.DocDescription = "System Generated";

                UploadDocumentTblObj.DocType = ".pdf";

                UploadDocumentTblObj.DocFolder = "Employee_Official_Documents";

                UploadDocumentTblObj.FileName = DocumentFileName;

                WetosDB.UploadDocuments.AddObject(UploadDocumentTblObj);

                WetosDB.SaveChanges();

                AddAuditTrail("System generated Relieving_Letter for " + EmployeeObj.EmployeeCode + " | " + EmployeeObj.FirstName + EmployeeObj.LastName);

            }
            else
            {
                Information("Relieving_Letter is already generated.");

                AddAuditTrail("Relieving_Letter is already generated for " + EmployeeObj.EmployeeCode + " | " + EmployeeObj.FirstName + EmployeeObj.LastName);
            }
            #endregion

            //RelievingLetterPrintFile(EmployeeId);

            return PartialView();
        }


        /// <summary>
        /// CODE ADDED BY SHRADDHA ON 20 APR 2017
        /// FOR RESIGNATION APPLICATION
        /// <param name="EmployeeId"></param>
        /// <returns></returns>
        public ActionResult ResignationApplication()
        {
            
            #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
            int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            //var EmployeeList = WetosDB.Employees.Select(a => new { id = a.EmployeeId, name = a.EmployeeCode + " - " + a.FirstName + " " + a.MiddleName + " " + a.LastName }).ToList();
            var EmployeeList = WetosDB.SP_VwActiveEmployee(EmployeeId).Select(a => new { id = a.EmployeeId, name = a.EmployeeCode + " - " + a.FirstName + " " + a.MiddleName + " " + a.LastName }).ToList();
            #endregion
            
            ViewBag.EmployeeList = new SelectList(EmployeeList, "id", "name").ToList();
            return View();
        }

        /// <summary>
        ///  CODE ADDED BY SHRADDHA ON 20 APR 2017
        ///FOR RESIGNATION APPLICATION FOR SELECTED EMPLOYEE
        /// <param name="EmployeeId"></param>
        /// <returns></returns>
        public ActionResult ResignationApplicationforSelectedEmployee(int EmployeeId)
        {
            try
            {
                int LoginEmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                EmployeeLifeCycle EmployeeLifeCycleObj = new EmployeeLifeCycle();
                EmployeeLifeCycleObj.EmployeeId = EmployeeId;
                Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == EmployeeId).FirstOrDefault();
                EmployeeLifeCycleObj.EmployeeCode = EmployeeObj.EmployeeCode;

                // COMMENTED BY MSJ 15 SEP 2017 //UNCOMMENTED BY SHRADDHA ON 16 SEP 2017 BECAUSE IT IS REQUIRED
                EmployeeLifeCycleObj.LoggedInUser = LoginEmployeeId;
                EmployeeLifeCycleObj.LastModifiedOn = DateTime.Now;
                // COMMENTED BY MSJ 15 SEP 2017 //UNCOMMENTED BY SHRADDHA ON 16 SEP 2017 BECAUSE IT IS REQUIRED

                return PartialView(EmployeeLifeCycleObj);

            }

            catch (System.Exception ex)
            {
                ViewBag.Message = "Error:" + ex.Message.ToString();
                AddAuditTrail("Error in Resignation Application");
                return RedirectToAction("ResignationApplication");

            }

        }

        [HttpPost]
        public ActionResult ResignationApplicationforSelectedEmployee(EmployeeLifeCycle EmployeeLifeCycleObj)
        {
            try
            {
                WetosDB.EmployeeLifeCycles.AddObject(EmployeeLifeCycleObj);
                WetosDB.SaveChanges();
                ViewBag.Message = "Resignation Application succeed for EmployeeCode: " + EmployeeLifeCycleObj.EmployeeCode;
                return RedirectToAction("ResignationApplication");
            }
            catch (System.Exception ex)
            {
                ViewBag.Message = "Error:" + ex.Message.ToString();
                AddAuditTrail("Error in Resignation Application for EmployeeCode: " + EmployeeLifeCycleObj.EmployeeCode);
                return RedirectToAction("ResignationApplication");

            }
        }


        public ActionResult Resignation(int EmployeeId)
        {
            string PdfFileName = "RelievingLetter";

            //PdfFileName = RelievingLetterPrintFile(EmployeeId);

            //ExperienceLetterPrintFile(EmployeeId);

            return File(PdfFileName, "application/pdf");
        }

        public string RelievingLetterPrintFile(int EmployeeId, ref string DocumentFileName)
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
                localReport.ReportPath = Server.MapPath("~/Reports/RelievingLetter.rdlc");

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

                    FileNameEx = EmployeeDetails.EmployeeCode; //+ " | " + EmployeeDetails.FirstName;

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

                DocumentFileName = "RelievingLetter_" + FileNameEx + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf";

                PdfFileName = Server.MapPath("~/User_Data/Employee_Official_Documents/") + DocumentFileName;
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

        public string ExperienceLetterPrintFile(int EmployeeId, ref string DocumentFileName)
        {
            string PdfFileName = string.Empty;

            try
            {
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
                localReport.ReportPath = Server.MapPath("~/Reports/ExperienceLetter.rdlc");

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

                DocumentFileName = "ExperienceLetter_" + FileNameEx + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf";

                PdfFileName = Server.MapPath("~/User_Data/Employee_Official_Documents/") + DocumentFileName;

                using (FileStream fs = new FileStream(PdfFileName, FileMode.Create))
                {
                    fs.Write(renderedBytes, 0, renderedBytes.Length);
                }

                //Response.AddHeader("Content-Disposition", "inline; filename=" + "TimeCardReport" + ".pdf;");

                Response.AddHeader("Content-Disposition", "attachment; filename=" + "ExperienceLetter" + ".pdf;"); // Updated by Rajas on 19 JAN 2017
                // string PdfUrl = "Employee_Official_Documents/" + "ExperienceLetter" + ".pdf";


            }
            catch (System.Exception)
            {
                //throw;
            }

            return PdfFileName;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="EmployeeId"></param>
        ///// <returns></returns>
        //public ActionResult ConfirmEmployee(int EmployeeId, string ConfirmationDate, string EmployeeGroupId)
        //{
        //    bool Successflag = false;
        //    try
        //    {
                
        //        DateTime EmployeeConfirmationDate=Convert.ToDateTime(ConfirmationDate);
        //        if (EmployeeConfirmationDate > DateTime.Now)
        //        {
        //            Error("Confirmation Date should not be greater than Today");
                  
        //            Successflag = false;
        //            return Json(Successflag, JsonRequestBehavior.AllowGet);
        //        }
        //        else
        //        {
        //            int OldEmpGroupId = 0;
        //            int NewEmployeeGroupId = Convert.ToInt32(EmployeeGroupId);
        //            Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == EmployeeId).FirstOrDefault();
        //            DateTime SelectedConfirmDate = Convert.ToDateTime(ConfirmationDate);

        //            bool IsNew = false;
        //            if (EmployeeObj == null)
        //            {
        //                EmployeeObj = new WetosDB.Employee();
        //                IsNew = true;
        //            }
        //            else
        //            {

        //                EmployeeGroupDetail EmployeeGroupDetailObj = WetosDB.EmployeeGroupDetails.Where(a => a.Employee.EmployeeId == EmployeeObj.EmployeeId).FirstOrDefault();
        //                bool IsNewEmployee = false;
        //                if (EmployeeGroupDetailObj != null)
        //                {
        //                    OldEmpGroupId = EmployeeGroupDetailObj.EmployeeGroup.EmployeeGroupId;
        //                }
        //                if (EmployeeGroupDetailObj == null)
        //                {
        //                    EmployeeGroupDetailObj = new EmployeeGroupDetail();
        //                    IsNewEmployee = true;
        //                }
        //                EmployeeGroupDetailObj.EmployeeGroup.EmployeeGroupId = NewEmployeeGroupId;
        //                EmployeeGroupDetailObj.Employee.EmployeeId = EmployeeId;
        //                if (IsNewEmployee == true)
        //                {
        //                    WetosDB.EmployeeGroupDetails.AddObject(EmployeeGroupDetailObj);
        //                }
        //                WetosDB.SaveChanges();

        //            }
        //            //EmployeeObj.EmployeeTypeId = 1;
        //            EmployeeObj.ConfirmDate = SelectedConfirmDate;

        //            WetosDB.SaveChanges();

        //            // WetosEmployeeController.GetLeavedataNewLogic(WetosDB, EmployeeObj.EmployeeId, SelectedConfirmDate);
        //            WetosEmployeeController.AddUpdateLeaveCreditBalance(WetosDB, EmployeeObj.EmployeeId, SelectedConfirmDate, OldEmpGroupId, NewEmployeeGroupId);



        //            AddAuditTrail("Success - Employee Confirmation: " + EmployeeObj.FirstName + " " + EmployeeObj.LastName);  // Updated by Rajas on 17 JAN 2017

        //            // Added by Rajas on 17 JAN 2017 START
        //            Success("Success - Employee Confirmation: " + EmployeeObj.FirstName + " " + EmployeeObj.LastName);
        //            Successflag = true;
        //            return Json(Successflag,JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    catch (System.Exception E)
        //    {
        //        AddAuditTrail("System Error - Employee Confirmation: ");  // Updated by Rajas on 17 JAN 2017

        //        // Added by Rajas on 17 JAN 2017 START
        //        Error("System Error - Employee Confirmation: ");
        //        Successflag = false;
        //        return Json(Successflag, JsonRequestBehavior.AllowGet);
        //    }
        //}

        /// <summary>
        /// Added By Rajas on 19 APRIL 2017
        /// Print confirmation
        /// </summary>
        /// <param name="EmployeeId"></param>
        /// <returns></returns>
        public ActionResult ConfirmationLetter(int EmployeeId, string LetterType)
        {
            string DocumentFileName = string.Empty;
            string PdfFileName = "";
            if (LetterType == "Confirm Letter without Increment")
            {
                PdfFileName = "ConfirmationLetterWithoutIncrement";

                PdfFileName = ConfirmationLetterWithoutIncrementPrintFile(EmployeeId, ref DocumentFileName);

                UploadDocument UploadDocumentTblObj = WetosDB.UploadDocuments.Where(a => a.EmployeeID == EmployeeId && a.DocContentType == "ConfirmationLetter_Without_Increment").FirstOrDefault();
                Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == EmployeeId).FirstOrDefault();
                if (UploadDocumentTblObj == null)
                {


                    // GENERATE PDF
                    //ConfirmationLetterWithoutIncrementPrintFile(EmployeeId);

                    UploadDocumentTblObj = new UploadDocument();

                    UploadDocumentTblObj.EmployeeID = EmployeeId;

                    UploadDocumentTblObj.DocContentType = "ConfirmationLetter_Without_Increment";

                    UploadDocumentTblObj.DocDescription = "System Generated";

                    UploadDocumentTblObj.DocType = ".pdf";

                    UploadDocumentTblObj.DocFolder = "Employee_Official_Documents";

                    UploadDocumentTblObj.FileName = DocumentFileName;

                    WetosDB.UploadDocuments.AddObject(UploadDocumentTblObj);

                    WetosDB.SaveChanges();

                    AddAuditTrail("System generated ConfirmationLetter_Without_Increment for " + EmployeeObj.EmployeeCode + " | " + EmployeeObj.FirstName + EmployeeObj.LastName);

                }
                else
                {
                    Information("ConfirmationLetter_Without_Increment is already generated.");

                    AddAuditTrail("ConfirmationLetter_Without_Increment already generated for " + EmployeeObj.EmployeeCode + " | " + EmployeeObj.FirstName + EmployeeObj.LastName);
                }

            }
            else if (LetterType == "Confirm Letter with Increment")
            {
                PdfFileName = "ConfirmationLetterWithIncrement";

                PdfFileName = ConfirmationLetterWithIncrementPrintFile(EmployeeId, ref DocumentFileName);

                UploadDocument UploadDocumentTblObj = WetosDB.UploadDocuments.Where(a => a.EmployeeID == EmployeeId && a.DocContentType == "ConfirmationLetter_With_Increment").FirstOrDefault();
                Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == EmployeeId).FirstOrDefault();
                if (UploadDocumentTblObj == null)
                {


                    // GENERATE PDF
                    //ConfirmationLetterWithoutIncrementPrintFile(EmployeeId);

                    UploadDocumentTblObj = new UploadDocument();

                    UploadDocumentTblObj.EmployeeID = EmployeeId;

                    UploadDocumentTblObj.DocContentType = "ConfirmationLetter_With_Increment";

                    UploadDocumentTblObj.DocDescription = "System Generated";

                    UploadDocumentTblObj.DocType = ".pdf";

                    UploadDocumentTblObj.DocFolder = "Employee_Official_Documents";

                    UploadDocumentTblObj.FileName = DocumentFileName;

                    WetosDB.UploadDocuments.AddObject(UploadDocumentTblObj);

                    WetosDB.SaveChanges();

                    AddAuditTrail("System generated ConfirmationLetter_With_Increment for " + EmployeeObj.EmployeeCode + " | " + EmployeeObj.FirstName + EmployeeObj.LastName);

                }
                else
                {
                    Information("ConfirmationLetter_With_Increment is already generated.");

                    AddAuditTrail("ConfirmationLetter_With_Increment already generated for " + EmployeeObj.EmployeeCode + " | " + EmployeeObj.FirstName + EmployeeObj.LastName);
                }
            }
            return File(PdfFileName, "application/pdf");

            //return PartialView();
        }

        /// <summary>
        /// Added by Rajas on 19 APRIL 2017
        /// Print function
        /// </summary>
        /// <param name="EmployeeId"></param>
        /// <returns></returns>
        public string ConfirmationLetterWithoutIncrementPrintFile(int EmployeeId, ref string DocumentFileName) // (HRLetterModel HRLetterModelObj, FormCollection Fc)
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

                    FileNameEx = EmployeeDetails.EmployeeCode + "_" + EmployeeDetails.FirstName;
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

                PdfFileName = Server.MapPath("~/User_Data/Employee_Official_Documents/") + "ConfirmationLetterWithoutIncrement_" + FileNameEx + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf";

                // ONLY FILE NAME
                DocumentFileName = "ConfirmationLetterWithoutIncrement_" + FileNameEx + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf";


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

        // Added by Rajas On 18 APRIL 2017
        #region Print HR Letters

       

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

        #endregion

        /// <summary>
        /// Json return for to get Department dropdown list on basis of branch, company selection
        /// Added by SHRADDHA ON 30 MAY 2017
        /// </summary>
        /// <returns></returns>
        public JsonResult GetEmployeeGroup(string EmployeeId)
        {
            int EmpId = Convert.ToInt32(EmployeeId);
            Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == EmpId).FirstOrDefault();
            string Branchid = EmployeeObj.BranchId.ToString();
            string Companyid = EmployeeObj.CompanyId.ToString();
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

            int EmpRecentGrpId = 0;
            EmployeeGroupDetail EmployeeGroupDetailObj = WetosDB.EmployeeGroupDetails.Where(a => a.Employee.EmployeeId == EmpId).FirstOrDefault();
            if (EmployeeGroupDetailObj != null)
            {
                EmpRecentGrpId = EmployeeGroupDetailObj.EmployeeGroup.EmployeeGroupId;
            }
            var EmployeeGroupList = WetosDB.EmployeeGroups.Where(a => a.Branch.BranchId == SelBranchId && a.Company.CompanyId == SelCompanyId 
                && a.EmployeeGroupId != EmpRecentGrpId).Select(a => new { EmployeeGroupId = a.EmployeeGroupId, EmployeeGroupName = a.EmployeeGroupName }).ToList();

            return Json(EmployeeGroupList);
        }

    }
}
