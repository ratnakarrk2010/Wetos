using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WetosDB;
//using WetosMVCMainApp.Models;
using WetosMVC.Models;
using WetosMVCMainApp.Models;

namespace WetosMVC.Controllers
{
    [SessionExpire] 
    [Authorize]
    public class WetosDesignationController : BaseController
    {
        
        //
        // GET: /WetosDesignation/

        public ActionResult Index()
        {
            try
            {
                List<VwDesignationDetail> DesignationsList = WetosDB.VwDesignationDetails.Where(a => a.MarkedAsDelete == 0 || a.MarkedAsDelete == null).ToList();

                //ADDED BY RAJAS ON 27 DEC 2016
                AddAuditTrail("Success - Visited Designation master"); // Updated by Rajas on 16 JAN 2017

                return View(DesignationsList);
            }

            catch (System.Exception ex)
            {
                // Added by Rajas on 16 JAN 2017
                AddAuditTrail("Exception - " + ex.Message);

                return View();
            }

        }

        /// <summary>
        /// Updated by Rajason 29 MARCH 2017
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int id)
        {
            try
            {
                VwDesignationDetail DesignationDetails = WetosDB.VwDesignationDetails.Where(a => a.MarkedAsDelete == 0 || a.MarkedAsDelete == null).Single(b => b.DesignationId == id);

                //ADDED BY RAJAS ON 27 DEC 2016
                AddAuditTrail("Checked details for " + DesignationDetails.DepartmentName); // Updated on 16 JAN 2017 by Rajas

                return View(DesignationDetails);
            }

            catch (System.Exception ex)
            {
                // Added by Rajas on 16 JAN 2017
                AddAuditTrail("Exception - " + ex.Message);

                return RedirectToAction("Index");
            }
        }


        /// <summary>
        /// Updated by Rajas on 27 MARCH 2017
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
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                return View();
            }
        }

        /// <summary>
        /// Designation posting along with the validation using DesignationModel
        /// UPDATED BY RAJAS ON 3 JUNE 2017
        /// </summary>
        /// <param name="DesignationObj"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(DesignationModel DesignationObj, FormCollection collection)
        {
            try
            {
                // Added by Rajas on 27 MARCH 2017
                string UpdateStatus = string.Empty;

                // Added by Rajas on 15 MAY 2017
                bool IsEdit = false;

                if (ModelState.IsValid)
                {
                    if (UpdateDesignationData(DesignationObj, IsEdit, ref UpdateStatus) == true)
                    {
                        // ADDED BY RAJAS FOR AUDITLOG ON 27 DEC 2016
                        AddAuditTrail("Success - Designation : " + DesignationObj.DesignationName + " is added successfully"); // Updated by Rajas on 16 JAN 2017


                        //ADDED CODE FOR SUCCESS MESSAGE BY RAJAS ON 16 JAN 2017
                        Success("Designation : " + DesignationObj.DesignationName + " is added successfully"); // Updated by Rajas on 16 JAN 2017
                    }
                    else
                    {
                        return ReportError(DesignationObj, UpdateStatus);
                    }

                    // Above line commented and below new line added by Rajas on 16 JAN 2017 for redirection to index page
                    return RedirectToAction("Index");
                }

                else
                {
                    //var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();

                    PopulateDropDownEdit(DesignationObj);

                    // ADDED BY RAJAS FOR AUDITLOG ON 27 DEC 2016
                    //AddAuditTrail("Error - Designation : " + DesignationObj.DesignationName + " create failed"); // Updated by Rajas on 16 JAN 2017


                    //ADDED CODE FOR SUCCESS MESSAGE BY RAJAS ON 16 JAN 2017
                    //Error("Error - Add new designation failed"); // Updated by Rajas on 16 JAN 2017

                    return View(DesignationObj);
                }
            }
            catch (System.Exception ex)
            {

                // ADDED BY RAJAS FOR AUDITLOG ON 27 DEC 2016
                AddAuditTrail("Exception - Designation : " + DesignationObj.DesignationName + " create failed due to " + ex.Message); // Updated by Rajas on 16 JAN 2017

                PopulateDropDownEdit(DesignationObj);

                //ADDED CODE FOR SUCCESS MESSAGE BY RAJAS ON 16 JAN 2017
                Error("Exception - Designation create failed"); // Updated by Rajas on 16 JAN 2017

                return View(DesignationObj);
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
                Designation DesignationEdit = new Designation();
                DesignationEdit = WetosDB.Designations.Single(b => b.DesignationId == id);

                DesignationModel DesignationObj = new DesignationModel();

                DesignationObj.DesignationId = DesignationEdit.DesignationId;

                DesignationObj.CompanyId = DesignationEdit.Company.CompanyId;   // Updated by Rajas on Company.CompanyId replaced with CompanyId

                DesignationObj.BranchId = DesignationEdit.Branch.BranchId;   // Updated by Rajas on Branch.BranchId replaced with Branch.BranchId 

                DesignationObj.DepartmentId = DesignationEdit.DepartmentId;

                DesignationObj.DesignationCode = DesignationEdit.DesignationCode;

                DesignationObj.DesignationName = DesignationEdit.DesignationName;

                DesignationObj.MarkedAsDelete = DesignationEdit.MarkedAsDelete;

                PopulateDropDownEdit(DesignationObj);

                return View(DesignationObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Inconsistent designation data please try again and verify");

                return RedirectToAction("Index"); // verify ?
            }
        }


        /// <summary>
        /// Updated by Rajas on 3 JUNE 2017
        /// </summary>
        /// <param name="id"></param>
        /// <param name="DesignationObj"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(int id, DesignationModel DesignationObj)
        {
            try
            {
                // Added by Rajas on 27 MARCH 2017
                string UpdateStatus = string.Empty;

                // Updated by Rajas on 15 MAY 2017
                bool IsEdit = true;

                if (ModelState.IsValid)
                {
                    if (UpdateDesignationData(DesignationObj, IsEdit, ref UpdateStatus) == true)
                    {
                        // ADDED BY RAJAS FOR AUDITLOG ON 27 DEC 2016
                        AddAuditTrail("Success - Designation : " + DesignationObj.DesignationName + " is updated successfully"); // Updated by Rajas on 16 JAN 2017


                        //ADDED CODE FOR SUCCESS MESSAGE BY RAJAS ON 16 JAN 2017
                        Success("Designation : " + DesignationObj.DesignationName + " is updated successfully"); // Updated by Rajas on 16 JAN 2017
                    }
                    else
                    {
                        return ReportError(DesignationObj, UpdateStatus);
                    }
                }
                else
                {
                    PopulateDropDownEdit(DesignationObj);

                    // ADDED BY RAJAS FOR AUDITLOG ON 27 DEC 2016
                    //AddAuditTrail("Error - Designation : " + DesignationObj.DesignationName + " update failed"); // Updated by Rajas on 16 JAN 2017


                    //ADDED CODE FOR SUCCESS MESSAGE BY RAJAS ON 16 JAN 2017
                    //Error("Error - Designation : " + DesignationObj.DesignationName + " update failed"); // Updated by Rajas on 16 JAN 2017

                    return View(DesignationObj);

                }

                return RedirectToAction("Index");
            }
            catch (System.Exception ex)
            {
                PopulateDropDownEdit(DesignationObj);

                // ADDED BY RAJAS FOR AUDITLOG ON 27 DEC 2016
                AddAuditTrail("Exception - Designation : " + DesignationObj.DesignationName + " update failed due to " + ex.Message); // Updated by Rajas on 16 JAN 2017


                //ADDED CODE FOR SUCCESS MESSAGE BY RAJAS ON 16 JAN 2017
                Error("Exception - Designation : " + DesignationObj.DesignationName + " update failed"); // Updated by Rajas on 16 JAN 2017

                return View(DesignationObj);
            }
        }


        /// <summary>
        /// Added by Rajas on 29 MARCH 2017
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            try
            {
                VwDesignationDetail DesignationDetails = WetosDB.VwDesignationDetails.Where(a => a.MarkedAsDelete == 0 || a.MarkedAsDelete == null).Single(b => b.DesignationId == id);

                //ADDED BY RAJAS ON 27 DEC 2016
                AddAuditTrail("Checked details for " + DesignationDetails.DepartmentName); // Updated on 16 JAN 2017 by Rajas

                return View(DesignationDetails);
            }

            catch (System.Exception ex)
            {
                // Added by Rajas on 16 JAN 2017
                AddAuditTrail("Exception - " + ex.Message);

                Error("Please try again");

                return RedirectToAction("Index");
            }
        }


        /// <summary>
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
                Designation DesignationObj = WetosDB.Designations.Where(a => a.DesignationId == id).FirstOrDefault();

                List<Employee> EmployeeListObj = WetosDB.Employees.Where(a => a.DesignationId == id).ToList();

                if (EmployeeListObj.Count > 0)
                {
                    // Updated by Rajas on 15 MAY 2017 START
                    Information("You can not delete this Designation, as " + DesignationObj.DesignationName + " is assigned to employee");

                    AddAuditTrail("Can not delete Designation as " + DesignationObj.DesignationName + " is assigned to employee");
                    // Updated by Rajas on 15 MAY 2017 END

                    return RedirectToAction("Index");
                }

                if (DesignationObj != null)
                {
                    DesignationObj.MarkedAsDelete = 1;

                    WetosDB.SaveChanges();

                    Success("Designation : " + DesignationObj.DesignationName + " deleted successfully");

                    AddAuditTrail("Designation : " + DesignationObj.DesignationName + " deleted successfully");
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
            //var BranchList = WetosDB.Branches.Where(a => a.Company.CompanyId == SelCompanyId && (a.MarkedAsDelete == null || a.MarkedAsDelete == 0)).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList();
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

            // Updated by Rajas on 27 FEB 2017
            // Updated by Rajas on 30 MAY 2017 for Marked as delete
            var DepartmentList = WetosDB.Departments.Where(a => a.Branch.BranchId == SelBranchId && (a.MarkedAsDelete == null || a.MarkedAsDelete == 0)).Select(a => new { DepartmentId = a.DepartmentId, DepartmentName = a.DepartmentCode + " - " + a.DepartmentName }).ToList();//ADDED DEPARTMENT CODE BY SHRADDHA ON 15 FEB 2018 

            return Json(DepartmentList);
        }

        /// <summary>
        /// Common Error reporting function
        /// Added by Rajas on 3 JUNE 2017
        /// </summary>
        /// <param name="DepartmentObj"></param>
        /// <param name="ErrorMessage"></param>
        /// <returns></returns>
        public ActionResult ReportError(DesignationModel DesignationObj, string ErrorMessage)
        {
            PopulateDropDownEdit(DesignationObj);//

            AddAuditTrail(ErrorMessage);
            Error(ErrorMessage);

            return View(DesignationObj);
        }

        /// <summary>
        /// Common function to validate the data for edit or create
        /// Added by Rajas on 30 DEC 2016
        /// </summary>
        /// Updated by Rajas on 27 MARCH 2017
        private bool UpdateDesignationData(DesignationModel DesignationObj, bool IsEdit, ref string UpdateStatus)
        {
            bool ReturnStatus = false;

            try
            {
                // Updated by Rajas on 31 MAY 2017
                WetosDB.Designation DesignationTblObj = WetosDB.Designations.Where(a => (a.DesignationName.Trim().ToUpper() == DesignationObj.DesignationName.Trim().ToUpper() || a.DesignationId == DesignationObj.DesignationId)
                    && a.Branch.BranchId == DesignationObj.BranchId
                    && a.Company.CompanyId == DesignationObj.CompanyId && a.DepartmentId == DesignationObj.DepartmentId
                    && a.MarkedAsDelete == 0).FirstOrDefault();

                //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 11 SEPTEMBER 2017 FOR EDIT
                WetosDB.Designation DesignationTblObjEDIT = WetosDB.Designations.Where(a => (a.DesignationName.Trim().ToUpper() == DesignationObj.DesignationName.Trim().ToUpper() || a.DesignationId == DesignationObj.DesignationId)
                    && a.Branch.BranchId == DesignationObj.BranchId
                    && a.Company.CompanyId == DesignationObj.CompanyId && a.DepartmentId == DesignationObj.DepartmentId
                    && a.MarkedAsDelete == 0).FirstOrDefault();

                // ADDED BY RAJAS ON 03 JUNE 2017 START
                bool IsNew = false;

                // IS OBJECT PRESET
                if (DesignationTblObj != null)  // Added by Rajas on 2 JUNE 2017
                {
                    if (IsEdit == false)  // CREATE            
                    {
                        UpdateStatus = "Designation already available."; //WetosEmployeeController.GetErrorMessage(1);

                        //AddAuditTrail("Error in Department update : " + UpdateStatus);
                        return ReturnStatus;
                    }
                }
                else
                {
                    if (IsEdit == false) // CREATE    
                    {
                        DesignationTblObj = new WetosDB.Designation();
                        IsNew = true;
                    }
                    else // EDIT    
                    {
                        UpdateStatus = "Error in updating designation."; // WetosEmployeeController.GetErrorMessage(1); 
                        //AddAuditTrail("Error in Department update : " + UpdateStatus);
                        return ReturnStatus;
                    }
                }
                // ADDED BY RAJAS ON 03 JUNE 2017 END


                // New Leave table object

                //ADDED NEW CODE TO RESOLVE Company NULL ISSUE BY SHRADDHA ON 01 NOV 2017 START
                if (DesignationTblObj.Company == null)
                {
                    DesignationTblObj.Company = WetosDB.Companies.Where(a => a.CompanyId == DesignationObj.CompanyId).FirstOrDefault();
                }
                //DesignationTblObj.Company.CompanyId = DesignationObj.CompanyId;   // Updated by Rajas on Company.CompanyId replaced with CompanyId
                //ADDED NEW CODE TO RESOLVE Company NULL ISSUE BY SHRADDHA ON 01 NOV 2017 END

                //ADDED NEW CODE TO RESOLVE Branch NULL ISSUE BY SHRADDHA ON 01 NOV 2017 START
                if (DesignationTblObj.Branch == null)
                {
                    DesignationTblObj.Branch = WetosDB.Branches.Where(a => a.BranchId == DesignationObj.BranchId).FirstOrDefault();
                }
                //DesignationTblObj.Branch.BranchId = DesignationObj.BranchId;   // Updated by Rajas on Branch.BranchId replaced with Branch.BranchId
                //ADDED NEW CODE TO RESOLVE Branch NULL ISSUE BY SHRADDHA ON 01 NOV 2017 END



                DesignationTblObj.DepartmentId = DesignationObj.DepartmentId;

                DesignationTblObj.DesignationCode = DesignationObj.DesignationCode;

                DesignationTblObj.DesignationName = DesignationObj.DesignationName.Trim();

                // Added by Rajas on 29 MARCH 2017
                DesignationTblObj.MarkedAsDelete = 0;  // default zero value


                // Add new table object 
                if (IsNew)
                {
                    WetosDB.Designations.Add(DesignationTblObj);
                }

                WetosDB.SaveChanges();



                //-------------------------------------------AUDITLOG---------------------------------------------------------------------------

                if (IsNew)
                {
                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 11 SEPTEMBER 2017
                    #region ADD AUDIT LOG
                    //OLD RECORD IS BLANK
                    string Newrecord = "Designation Name :" + DesignationTblObj.DesignationName
                        + ", Designation Code :" + DesignationTblObj.DesignationCode + ", Department ID :" + DesignationTblObj.DepartmentId
                        + ", Company ID :" + DesignationTblObj.Company.CompanyId + ", Branch ID :" + DesignationTblObj.Branch.BranchId;


                    //LOGIN ID IS TAKEN FROM SESSION PERSISTER
                    string Formname = "Designation MASTER";
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
                    string Oldrecord = "Designation Name :" + DesignationTblObjEDIT.DesignationName
                        + ", Designation Code :" + DesignationTblObjEDIT.DesignationCode + ", Department ID :" + DesignationTblObjEDIT.DepartmentId
                        + ", Company ID :" + DesignationTblObjEDIT.Company.CompanyId + ", Branch ID :" + DesignationTblObjEDIT.Branch.BranchId;
                    //---x---------x-----------------x------------x----------------x-----------x-------------x------------x--------------x----------x---------x-----
                    string Newrecord = "Designation Name :" + DesignationTblObj.DesignationName
                        + ", Designation Code :" + DesignationTblObj.DesignationCode + ", Department ID :" + DesignationTblObj.DepartmentId
                        + ", Company ID :" + DesignationTblObj.Company.CompanyId + ", Branch ID :" + DesignationTblObj.Branch.BranchId;


                    //LOGIN ID IS TAKEN FROM SESSION PERSISTER
                    string Formname = "Designation MASTER";
                    //ACTION IS UPDATE
                    string Message = " ";

                    WetosAdministrationController.GenerateAuditLogsUpdate(WetosDB, 0, Formname, Oldrecord,
                        Newrecord, ref Message);
                    #endregion
                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 11 SEPTEMBER 2017
                }

                //-------------------------------------------AUDITLOG---------------------------------------------------------------------------



                ReturnStatus = true;

                return ReturnStatus;
            }
            catch (System.Exception ex)
            {
                UpdateStatus = "Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message);

                return ReturnStatus;
            }
        }

        /// <summary>
        /// Added by Rajas on 16 JAN 2017
        /// Updated by Rajas on 27 FEB 2017
        /// </summary>
        private void PopulateDropDown()
        {
            try
            {
                // Updated by Rajas on 6 MAY 2017 to get data which is not MarkedAsDelete
                #region COMPANY LIST CODE ADDED BY SHRADDHA ON 30 MAR 2018
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //var CompanyObj = WetosDB.Companies.Select(a => new { CompanyId = a.CompanyId, CompanyName = a.CompanyName }).ToList();
                var CompanyObj = WetosDB.SP_GetCompanyList(EmployeeId).Select(m => new { Companyid = m.CompanyId, CompanyName = m.CompanyName }).ToList();
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
                #endregion
                ViewBag.CompanyList = new SelectList(CompanyObj, "CompanyId", "CompanyName").ToList();

                var Branch = new List<Branch>(); //WetosDB.Branches.Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList();
                ViewBag.BranchList = new SelectList(Branch, "BranchId", "BranchName").ToList();

                var Department = new List<Department>(); //WetosDB.Departments.Select(a => new { DepartmentId = a.DepartmentId, DepartmentName = a.DepartmentName }).ToList();
                ViewBag.DepartmentList = new SelectList(Department, "DepartmentId", "DepartmentName").ToList();
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Inconsistent data. Please try again and verify");
            }

        }

        /// <summary>
        /// Updated by Rajas on 3 JUNE 2017
        /// </summary>
        private void PopulateDropDownEdit(DesignationModel DesignationObj)
        {
            try
            {
                // Updated by Rajas on 6 MAY 2017 to get data which is not MarkedAsDelete START
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
                // var Branch = WetosDB.Branches.Where(a => (a.MarkedAsDelete == null || a.MarkedAsDelete == 0) && a.Company.CompanyId == DesignationObj.CompanyId)
                    //.Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList();
                var BranchList = WetosDB.SP_GetBranchList(EmployeeId).Where(a => a.CompanyId == DesignationObj.CompanyId).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList(); //.Select(m => new { BranchId = m.BranchId, BranchName = m.BranchName}).ToList();
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
                #endregion


                ViewBag.BranchList = new SelectList(BranchList, "BranchId", "BranchName").ToList();

                var Department = WetosDB.Departments.Where(a => (a.MarkedAsDelete == null || a.MarkedAsDelete == 0)
                    && a.Company.CompanyId == DesignationObj.CompanyId && a.Branch.BranchId == DesignationObj.BranchId)
                    .Select(a => new { DepartmentId = a.DepartmentId, DepartmentName = a.DepartmentCode + " - " + a.DepartmentName }).ToList();
                ViewBag.DepartmentList = new SelectList(Department, "DepartmentId", "DepartmentName").ToList(); //ADDED DEPARTMENT CODE BY SHRADDHA ON 15 FEB 2018 
                // Updated by Rajas on 6 MAY 2017 to get data which is not MarkedAsDelete END
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Inconsistent data. Please try again and verify");
            }

        }
    }
}
