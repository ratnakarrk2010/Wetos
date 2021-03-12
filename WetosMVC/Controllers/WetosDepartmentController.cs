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
    public class WetosDepartmentController : BaseController
    {
        
        //
        // GET: /WetosDepartment/
        /// <summary>
        /// Added by Mousami on 07 SEPT 2016 start
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            try
            {
                // Updated by Rajas on 29 MARCH 2017 after MarkedAsDelete changes
                List<VwDepartmentDetail> DepartmentList = WetosDB.VwDepartmentDetails.Where(a => a.MarkedAsDelete == 0
                    || a.MarkedAsDelete == null).OrderByDescending(a => a.DepartmentId).ToList();

                //ADDED BY RAJAS ON 27 DEC 2016
                AddAuditTrail("Success - Visited Department master"); // Updated by Rajas on 16 JAN 2017

                return View(DepartmentList);
            }

            catch (System.Exception ex)
            {
                // Added by Rajas on 16 JAN 2017
                AddAuditTrail("Exception - " + ex.Message);

                return View();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int id)
        {
            try
            {
                VwDepartmentDetail DepartmentDetails = WetosDB.VwDepartmentDetails.Single(b => b.DepartmentId == id);

                //ADDED BY RAJAS ON 27 DEC 2016
                AddAuditTrail("Checked details for " + DepartmentDetails.DepartmentName); // Updated on 16 JAN 2017 by Rajas

                return View(DepartmentDetails);
            }

            catch (System.Exception ex)
            {
                // Added by Rajas on 16 JAN 2017
                AddAuditTrail("Exception - " + ex.Message);

                return View();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            try
            {
                // POPULATE VIEWBAG FRO DROPDOWN
                PopulateDropDown();

                DepartmentModel DepartmentObj = new DepartmentModel();

                return View(DepartmentObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Please try again");

                return View();  // Verify ?
            }
        }

        /// <summary>
        ///  Populate All Dropdown ViewBag
        /// </summary>
        public void PopulateDropDown()
        {
            try
            {
                //CODE FOR DROPDOWN for COMPANY
                // Updated by Rajas on 6 MAY 2017 to get data which is not MarkedAsDelete
                #region COMPANY LIST CODE ADDED BY SHRADDHA ON 30 MAR 2018
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //var CompanyObj = WetosDB.Companies.Select(a => new { CompanyId = a.CompanyId, CompanyName = a.CompanyName }).ToList();
                var CompanyObj = WetosDB.SP_GetCompanyList(EmployeeId).Select(m => new { Companyid = m.CompanyId, CompanyName = m.CompanyName }).ToList();
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
                #endregion

                ViewBag.CompanyList = new SelectList(CompanyObj, "CompanyId", "CompanyName").ToList();

                //CODE FOR DROPDOWN for BRANCH
                var Branch = new List<Branch>();
                ViewBag.BranchList = new SelectList(Branch, "BranchId", "BranchName").ToList();

            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message);

                Error("Inconsistent data. Please try again!");
            }
        }

        /// <summary>
        ///  Populate All Dropdown ViewBag
        ///  Added by Rajas on 27 FEB 2017
        /// </summary>
        public void PopulateDropDownEdit(DepartmentModel DepartmentObj)
        {
            try
            {
                //CODE FOR DROPDOWN for COMPANY

                #region COMPANY LIST CODE ADDED BY SHRADDHA ON 30 MAR 2018
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //var CompanyObj = WetosDB.Companies.Select(a => new { CompanyId = a.CompanyId, CompanyName = a.CompanyName }).ToList();
                var CompanyObj = WetosDB.SP_GetCompanyList(EmployeeId).Select(m => new { Companyid = m.CompanyId, CompanyName = m.CompanyName }).ToList();
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
                #endregion

                ViewBag.CompanyList = new SelectList(CompanyObj, "CompanyId", "CompanyName").ToList();

                //CODE FOR DROPDOWN for BRANCH
                // Updated by Rajas on 6 MAY 2017 to get data which is not MarkedAsDelete

                #region CODE FOR GET BRANCH LIST ADDED BY SHRADDHA ON 30 MAR 2018
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
                //var Branch = WetosDB.Branches.Where(a => (a.MarkedAsDelete == null || a.MarkedAsDelete == 0)
                //&& a.Company.CompanyId == DepartmentObj.CompanyId)
                //.Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).OrderByDescending(a => a.BranchId).ToList();

                var BranchList = WetosDB.SP_GetBranchList(EmployeeId).Where(a => a.CompanyId == DepartmentObj.CompanyId).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).OrderByDescending(a => a.BranchId).ToList(); //.Select(m => new { BranchId = m.BranchId, BranchName = m.BranchName}).ToList();
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
                #endregion

                ViewBag.BranchList = new SelectList(BranchList, "BranchId", "BranchName").ToList();
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message);

                Error("Inconsistent data. Please try again!");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ReportError(bool ReturnStatus, string ErrorStr)
        {
            //return ErrorStr;
            return ReturnStatus;
        }

        /// <summary>
        /// Common Error reporting function
        /// Added by MSJ on 2 JUNE 2017
        /// </summary>
        /// <param name="DepartmentObj"></param>
        /// <param name="ErrorMessage"></param>
        /// <returns></returns>
        public ActionResult ReportError(DepartmentModel DepartmentObj, string ErrorMessage)
        {
            PopulateDropDownEdit(DepartmentObj);//

            AddAuditTrail(ErrorMessage);
            Error(ErrorMessage);

            return View(DepartmentObj);
        }

        //
        // POST: /WetosDepartment/Create

        [HttpPost]
        public ActionResult Create(DepartmentModel DepartmentObj, FormCollection collection)
        {
            try
            {
                // Added by Rajas on 27 MARCH 2017
                string UpdateStatus = string.Empty;

                // Added by Rajas on 15 MAY 2017
                bool IsEdit = false;

                // ADDED FOR VALIDATION 
                if (ModelState.IsValid)
                {
                    if (UpdateDepartmentData(DepartmentObj, IsEdit, ref UpdateStatus) == true)
                    {

                        // ADDED BY RAJAS FOR AuditLog ON 27 DEC 2016
                        // Updated by Rajas on 16 JAN 2017
                        AddAuditTrail("Success - Department : " + DepartmentObj.DepartmentName + " is added successfully");

                        //ADDED CODE FOR SUCCESS MESSAGE BY RAJAS ON 16 JAN 2017
                        Success("Department : " + DepartmentObj.DepartmentName + " is added successfully"); // Updated by Rajas on 16 JAN 2017
                    }
                    else
                    {
                        return ReportError(DepartmentObj, UpdateStatus);
                        //AddAuditTrail(UpdateStatus);
                        //Error(UpdateStatus);
                        //return View(DepartmentObj);
                    }

                    // Above line commented and below new line added by Rajas on 16 JAN 2017 for redirection to index page
                    return RedirectToAction("Index");
                }
                else
                {
                    // POPULATE VIEWBAG FRO DROPDOWN
                    //PopulateDropDown(); 
                    PopulateDropDownEdit(DepartmentObj);//

                    // ADDED BY RAJAS FOR AuditLog ON 27 DEC 2016
                    //AddAuditTrail("Error - Department : " + DepartmentObj.DepartmentName + " create failed"); // Updated by Rajas on 16 JAN 2017


                    //ADDED CODE FOR SUCCESS MESSAGE BY RAJAS ON 16 JAN 2017
                    //Error("Error - Department : " + DepartmentObj.DepartmentName + " create failed"); // Updated by Rajas on 16 JAN 2017

                    return View(DepartmentObj);
                }
            }
            catch (System.Exception ex)
            {
                //PopulateDropDown();
                PopulateDropDownEdit(DepartmentObj);//

                // ADDED BY RAJAS FOR AuditLog ON 27 DEC 2016
                AddAuditTrail("Exception - Department : " + DepartmentObj.DepartmentName + " create failed due to " + ex.Message); // Updated by Rajas on 16 JAN 2017


                //ADDED CODE FOR SUCCESS MESSAGE BY RAJAS ON 16 JAN 2017
                Error("Exception - Department : " + DepartmentObj.DepartmentName + " create failed due to " + ex.Message); // Updated by Rajas on 16 JAN 2017

                return View(DepartmentObj);
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
                Department DepartmentEdit = WetosDB.Departments.Single(b => b.DepartmentId == id);

                DepartmentModel DepartmentObj = new DepartmentModel();

                DepartmentObj.DepartmentId = DepartmentEdit.DepartmentId;

                DepartmentObj.DepartmentName = DepartmentEdit.DepartmentName;

                DepartmentObj.DepartmentCode = DepartmentEdit.DepartmentCode;

                DepartmentObj.CompanyId = DepartmentEdit.Company.CompanyId;

                DepartmentObj.BranchId = DepartmentEdit.Branch.BranchId;

                // Added by Rajas on 29 MARCH 2017
                DepartmentObj.MarkedAsDelete = DepartmentEdit.MarkedAsDelete;  // MarkedAsDelete

                // POPULATE VIEWBAG FROM DROPDOWN
                PopulateDropDownEdit(DepartmentObj);

                return View(DepartmentObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message);

                Error("Inconsistent data. Please try again!");

                return View();
            }
        }

        /// <summary>
        /// Updated by Rajas on 27 MARCH 2017
        /// </summary>
        /// <param name="id"></param>
        /// <param name="DepartmentObj"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(int id, DepartmentModel DepartmentObj)
        {
            try
            {
                // Added by Rajas on 27 MARCH 2017
                string UpdateStatus = string.Empty;

                // Added by Rajas on 15 MAY 2017
                bool IsEdit = true;

                if (ModelState.IsValid)
                {
                    // Updated by Rajas on 15 MAY 2017
                    if (UpdateDepartmentData(DepartmentObj, IsEdit, ref UpdateStatus) == true)
                    {

                        // ADDED BY RAJAS FOR AuditLog ON 27 DEC 2016
                        AddAuditTrail("Success - Department : " + DepartmentObj.DepartmentName + " is updated successfully"); // Updated by Rajas on 16 JAN 2017


                        //ADDED CODE FOR SUCCESS MESSAGE BY RAJAS ON 16 JAN 2017
                        Success("Department : " + DepartmentObj.DepartmentName + " is updated successfully"); // Updated by Rajas on 16 JAN 2017
                    }
                    else
                    {
                        return ReportError(DepartmentObj, UpdateStatus);
                    }

                    return RedirectToAction("Index");
                }

                else
                {
                    // POPULATE VIEWBAG FRO DROPDOWN
                    PopulateDropDownEdit(DepartmentObj);

                    // ADDED BY RAJAS FOR AuditLog ON 27 DEC 2016
                    // AddAuditTrail("Error - Department : " + DepartmentObj.DepartmentName + " update failed"); // Updated by Rajas on 16 JAN 2017


                    //ADDED CODE FOR SUCCESS MESSAGE BY RAJAS ON 16 JAN 2017
                    //Error("Error - Department : " + DepartmentObj.DepartmentName + " update failed"); // Updated by Rajas on 16 JAN 2017

                    return View(DepartmentObj);

                }

            }
            catch (System.Exception ex)
            {
                // POPULATE VIEWBAG FRO DROPDOWN
                PopulateDropDownEdit(DepartmentObj);

                // ADDED BY RAJAS FOR AuditLog ON 27 DEC 2016
                AddAuditTrail("Error - Department : " + DepartmentObj.DepartmentName + " update failed due to " + ex.Message); // Updated by Rajas on 16 JAN 2017


                //ADDED CODE FOR SUCCESS MESSAGE BY RAJAS ON 16 JAN 2017
                Error("Error - Department : " + DepartmentObj.DepartmentName + " update failed due to " + ex.Message); // Updated by Rajas on 16 JAN 2017

                return View(DepartmentObj);
            }
        }


        /// <summary>
        /// DELETE GET
        /// Added by Rajas on 29 MARCH 2017
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            try
            {
                VwDepartmentDetail DepartmentDetails = WetosDB.VwDepartmentDetails.Where(a => a.MarkedAsDelete == 0 || a.MarkedAsDelete == null)
                    .Single(b => b.DepartmentId == id);

                //ADDED BY RAJAS ON 27 DEC 2016
                AddAuditTrail("Checked details for " + DepartmentDetails.DepartmentName);

                return View(DepartmentDetails);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Please try again!");

                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// DELETE POST
        /// Added by Rajas on 29 MARCH 2017
        /// </summary>
        /// <param name="id"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                Department DepartmentObj = WetosDB.Departments.Where(a => a.DepartmentId == id).FirstOrDefault();

                List<Employee> EmployeeListObj = WetosDB.Employees.Where(a => a.DepartmentId == id).ToList();

                if (EmployeeListObj.Count > 0)
                {
                    // Updated by Rajas on 15 MAY 2017 START
                    Information("You can not delete this Department, as " + DepartmentObj.DepartmentName + " is assigned to employee");

                    AddAuditTrail("Can not delete Department as " + DepartmentObj.DepartmentName + " is assigned to employee");
                    // Updated by Rajas on 15 MAY 2017 END

                    return RedirectToAction("Index");
                }

                if (DepartmentObj != null)
                {
                    DepartmentObj.MarkedAsDelete = 1;

                    WetosDB.SaveChanges();

                    Success("Department : " + DepartmentObj.DepartmentName + " deleted successfully");

                    AddAuditTrail("Department : " + DepartmentObj.DepartmentName + " deleted successfully");
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
            //var BranchList = WetosDB.Branches.Where(a => a.Company.CompanyId == SelCompanyId && (a.MarkedAsDelete == null || a.MarkedAsDelete == 0))
            //.Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList();

            var BranchList = WetosDB.SP_GetBranchList(EmployeeId).Where(a => a.CompanyId == SelCompanyId).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList(); //.Select(m => new { BranchId = m.BranchId, BranchName = m.BranchName}).ToList();
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
            #endregion

            return Json(BranchList);
        }

        /// <summary>
        /// Common function to validate the data for edit or create
        /// Added by Rajas on 30 DEC 2016
        /// </summary>
        /// Updated by Rajas on 27 MARCH 2017
        private bool UpdateDepartmentData(DepartmentModel DepartmentObj, bool IsEdit, ref string UpdateStatus)
        {
            bool ReturnStatus = false;

            try
            {
                // a.DepartmentCode == DepartmentObj.DepartmentCode added by Rajas on 16 JAN 2017
                // Updated by Rajas on 20 JULY 2017 to resolve Defect Id 3 from Defect Sheet by Ulka(Eviska)
                WetosDB.Department DepartmentTblObj = WetosDB.Departments.Where(a => ((a.DepartmentName.Trim().ToUpper() == DepartmentObj.DepartmentName.Trim().ToUpper()
                    && a.DepartmentCode.ToUpper() == DepartmentObj.DepartmentCode)
                    || a.DepartmentId == DepartmentObj.DepartmentId)
                    && a.Company.CompanyId == DepartmentObj.CompanyId && a.Branch.BranchId == DepartmentObj.BranchId && a.MarkedAsDelete == 0).FirstOrDefault();

                //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 9 SEPTEMBER 2017 FOR EDIT
                WetosDB.Department DepartmentTblObjEDIT = WetosDB.Departments.Where(a => ((a.DepartmentName.Trim().ToUpper() == DepartmentObj.DepartmentName.Trim().ToUpper() && a.DepartmentCode.ToUpper() == DepartmentObj.DepartmentCode)
                    || a.DepartmentId == DepartmentObj.DepartmentId)
                    && a.Company.CompanyId == DepartmentObj.CompanyId && a.Branch.BranchId == DepartmentObj.BranchId && a.MarkedAsDelete == 0).FirstOrDefault();

                // Added by Rajas on 26 JULY 2017 START
                WetosDB.Department ValidateDeptCode = WetosDB.Departments.Where(a => (a.DepartmentCode.ToUpper() == DepartmentObj.DepartmentCode)
                    && a.Company.CompanyId == DepartmentObj.CompanyId && a.Branch.BranchId == DepartmentObj.BranchId && a.MarkedAsDelete == 0).FirstOrDefault();
                // Added by Rajas on 26 JULY 2017 END

                // ADDED BY MSJ ON 02 JUNE 2017 START
                bool IsNew = false;

                // IS OBJECT PRESET
                if (DepartmentTblObj != null)  // Added by Rajas on 2 JUNE 2017
                {
                    //if (IsEdit == false)  // CREATE            
                    //{
                    //    UpdateStatus = "Department already available."; //WetosEmployeeController.GetErrorMessage(1);

                    //    //AddAuditTrail("Error in Department update : " + UpdateStatus);
                    //    return ReturnStatus;
                    //}
                    //else
                    //{
                    //}

                    if (IsEdit == false) // false // CREATE // CHANGED FROM FALSE TO TRUE BY MSJ ON 05 MARCH 2018
                    {
                        // Added by Rajas on 26 JULY 2017 START
                        if (ValidateDeptCode == null)
                        {
                            DepartmentTblObj = new WetosDB.Department();
                            IsNew = true;
                        }
                        else
                        {
                            UpdateStatus = "Department code already available";
                            return ReturnStatus;
                        }
                    }
                    else
                    {

                        // Added by Rajas on 26 JULY 2017 START
                        if (ValidateDeptCode != null)
                        {
                            UpdateStatus = "Department code already available";
                            return ReturnStatus;
                        }
                    }
                    // Added by Rajas on 26 JULY 2017 END
                }
                else
                {
                    if (IsEdit == false) // false // CREATE // CHANGED FROM FALSE TO TRUE BY MSJ ON 05 MARCH 2018
                    {
                        // Added by Rajas on 26 JULY 2017 START
                        if (ValidateDeptCode == null)
                        {
                            DepartmentTblObj = new WetosDB.Department();
                            IsNew = true;
                        }
                        else
                        {
                            UpdateStatus = "Department code already available";
                            return ReturnStatus;
                        }
                        // Added by Rajas on 26 JULY 2017 END
                    }
                    else // EDIT    
                    {
                        UpdateStatus = "Error in updating department."; //WetosEmployeeController.GetErrorMessage(1);
                        //AddAuditTrail("Error in Department update : " + UpdateStatus);
                        return ReturnStatus;
                    }
                }
                // ADDED BY MSJ ON 02 JUNE 2017 END

                // New Leave table object
                DepartmentTblObj.DepartmentName = DepartmentObj.DepartmentName.Trim();

                DepartmentTblObj.DepartmentCode = DepartmentObj.DepartmentCode.Trim();

                DepartmentTblObj.Company = WetosDB.Companies.Where(a => a.CompanyId == DepartmentObj.CompanyId).FirstOrDefault();
                DepartmentTblObj.Company.CompanyId = DepartmentObj.CompanyId;  // Updated by Rajas on 21 FEB 2017 Company.CompanyId replaced with CompanyId 

                DepartmentTblObj.Branch = WetosDB.Branches.Where(a => a.BranchId == DepartmentObj.BranchId).FirstOrDefault();
                DepartmentTblObj.Branch.BranchId = DepartmentObj.BranchId; // Updated by Rajas on 21 FEB 2017 Branch.BranchId replaced with BranchId

                // Added by Rajas on 29 MARCH 2017
                DepartmentTblObj.MarkedAsDelete = 0;  // default zero value


                // Add new table object 
                if (IsNew)
                {
                    WetosDB.Departments.Add(DepartmentTblObj);
                }

                WetosDB.SaveChanges();


                //-------------------------------------------AuditLog---------------------------------------------------------------------------

                if (IsNew)
                {
                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 11 SEPTEMBER 2017
                    #region ADD AUDIT LOG
                    //OLD RECORD IS BLANK
                    string Newrecord = "DepartmentCode :" + DepartmentTblObj.DepartmentCode + ", DepartmentName :"
                        + DepartmentTblObj.DepartmentName + ", BranchId :" + DepartmentTblObj.Branch.BranchId
                        + ", CompanyId:" + DepartmentTblObj.Company.CompanyId;

                    //LOGIN ID IS TAKEN FROM SESSION PERSISTER
                    string Formname = "DEPARTMENT MASTER";
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
                    string Oldrecord = "DepartmentCode :" + DepartmentTblObjEDIT.DepartmentCode + ", DepartmentName :"
                        + DepartmentTblObjEDIT.DepartmentName + ", BranchId :" + DepartmentTblObjEDIT.Branch.BranchId
                        + ", CompanyId:" + DepartmentTblObjEDIT.Company.CompanyId;
                    //---x---------x-----------------x------------x----------------x-----------x-------------x------------x--------------x----------x---------x-----
                    string Newrecord = "DepartmentCode :" + DepartmentTblObj.DepartmentCode + ", DepartmentName :"
                        + DepartmentTblObj.DepartmentName + ", BranchId :" + DepartmentTblObj.Branch.BranchId
                        + ", CompanyId:" + DepartmentTblObj.Company.CompanyId;

                    //LOGIN ID IS TAKEN FROM SESSION PERSISTER
                    string Formname = "DEPARTMENT MASTER";
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

                string ErrorMessage = WetosEmployeeController.GetErrorMessage() + ex.Message;                                     //"Error due to " + ex.Message;
                AddAuditTrail(ErrorMessage);
                UpdateStatus = ErrorMessage;
                return ReturnStatus;
            }
        }
    }
}
