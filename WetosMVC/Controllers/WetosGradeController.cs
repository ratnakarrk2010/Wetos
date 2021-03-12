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
    public class WetosGradeController : BaseController
    {
        
        // GET: /WetosGrade/

        public ActionResult Index()
        {
            try
            {
                // Updated by Rajas after MarkedAsDelete on 29 MARCH 2017
                List<VwGradeDetail> GradeList = WetosDB.VwGradeDetails.Where(a => a.MarkedAsDelete == 0 || a.MarkedAsDelete == null).ToList();

                //ADDED BY RAJAS ON 27 DEC 2016
                AddAuditTrail("Grade list viewed.");  // Updated by Rajas on 15 MAY 2017

                return View(GradeList);
            }

            catch (System.Exception ex)
            {
                // Added by Rajas on 18 FEB 2017
                AddAuditTrail("Exception - " + ex.Message);

                return View();
            }

        }


        /// <summary>
        /// Updated by Rajas on 29 MARCH 2017
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int id)
        {
            try
            {
                // Get List of grades
                VwGradeDetail GradeDetails = WetosDB.VwGradeDetails.Where(a => a.GradeId == id).FirstOrDefault();

                //ADDED BY RAJAS ON 27 DEC 2016
                AddAuditTrail("Grade details checked");

                return View(GradeDetails);
            }

            catch (System.Exception ex)
            {
                // Added by Rajas on 18 FEB 2017
                AddAuditTrail("Exception - " + ex.Message);

                Error("Inconsistent data. Please try again!");

                return RedirectToAction("Index");  // Verify ?
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
                //GET List Of Grade
                PopulateDropDown();

                return View();
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Inconsistent data. Please try again!");

                return View("Index"); // Verify ?
            }
        }

        //
        // POST: /WetosGrade/Create

        /// <summary>
        /// POST validate data for grade save
        /// Added by Rajas on 30 DEC 2016 
        /// </summary>
        /// <param name="NewGradeObj"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        /// Updated by Rajas on 3 JUNE 2017
        [HttpPost]
        public ActionResult Create(GradeModel NewGradeObj, FormCollection collection)
        {
            try
            {
                // Added by Rajas on 27 MARCH 2017
                string UpdateStatus = string.Empty;

                // Added by Rajas on 15 MAY 2017
                bool IsEdit = false;

                if (ModelState.IsValid)
                {
                    if (UpdateGradeData(NewGradeObj, IsEdit, ref UpdateStatus) == true)
                    {
                        // ADDED BY RAJAS FOR AuditLog ON 27 DEC 2016
                        AddAuditTrail("New grade added");

                        // Added by Rajas on 21 FEB 2017
                        Success("New grade " + NewGradeObj.GradeName + " is added successfully");
                    }
                    else
                    {
                        return ReportError(NewGradeObj, UpdateStatus);
                    }

                    // TODO: Add insert logic here

                    return RedirectToAction("Index");
                }

                else
                {
                    PopulateDropDownEdit(NewGradeObj);

                    // ADDED BY RAJAS FOR AuditLog ON 27 DEC 2016
                    //AddAuditTrail("New Grade is not added");

                    // Added by Rajas on 21 FEB 2017
                    //Error("New grade is not added");

                    return View(NewGradeObj);
                }
            }
            catch (System.Exception ex)
            {
                PopulateDropDownEdit(NewGradeObj);

                // ADDED BY RAJAS FOR AuditLog ON 27 DEC 2016
                AddAuditTrail("New Grade is not added due to " + ex.Message);

                // Added by Rajas on 21 FEB 2017
                Error("New grade is not added");

                return View(NewGradeObj);
            }
        }


        /// <summary>
        /// Updated by Rajas on 3 JUNE 2017
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            try
            {
                Grade GradeDetails = new Grade();
                GradeDetails = WetosDB.Grades.Where(a => a.GradeId == id).FirstOrDefault();

                GradeModel NewGradeObj = new GradeModel();

                NewGradeObj.GradeId = GradeDetails.GradeId;

                NewGradeObj.GradeName = GradeDetails.GradeName;

                NewGradeObj.EmployeeTypeId = GradeDetails.EmployeeType.EmployeeTypeId;

                NewGradeObj.CompanyId = GradeDetails.Company.CompanyId;     // Updated by Rajas on Company.CompanyId replaced with CompanyId

                NewGradeObj.BranchId = GradeDetails.Branch.BranchId;    // Updated by Rajas on Branch.BranchId replaced with Branch.BranchId 

                // Added by Rajas on 29 MARCH 2017
                NewGradeObj.MarkedAsDelete = GradeDetails.MarkedAsDelete;   // MarkedAsDelete

                PopulateDropDownEdit(NewGradeObj);

                return View(NewGradeObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Error due to inconsistent data. Please try again");

                return View("Index");  // Verify ?
            }
        }


        /// <summary>
        /// Updated by Rajas on 3 JUNE 2017
        /// </summary>
        /// <param name="id"></param>
        /// <param name="NewGradeObj"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(int id, GradeModel NewGradeObj)
        {
            try
            {
                // Added by Rajas on 27 MARCH 2017
                string UpdateStatus = string.Empty;

                // Added by Rajas on 15 MAY 2017
                bool IsEdit = true;

                if (ModelState.IsValid)
                {
                    if (UpdateGradeData(NewGradeObj, IsEdit, ref UpdateStatus) == true)
                    {

                        // ADDED BY RAJAS FOR AuditLog ON 27 DEC 2016
                        AddAuditTrail("Grade details updated");

                        // Added by Rajas on 21 FEB 2017
                        Success("New grade " + NewGradeObj.GradeName + " is updated successfully");
                    }
                    else
                    {
                        return ReportError(NewGradeObj, UpdateStatus);
                    }
                }
                else
                {
                    PopulateDropDownEdit(NewGradeObj);

                    // ADDED BY RAJAS FOR AuditLog ON 27 DEC 2016
                    //AddAuditTrail("New Grade is not updated");

                    // Added by Rajas on 21 FEB 2017
                    //Error("New grade is not updated");

                    return View(NewGradeObj);
                }

                return RedirectToAction("Index");
            }
            catch (System.Exception ex)
            {
                PopulateDropDownEdit(NewGradeObj);

                // ADDED BY RAJAS FOR AuditLog ON 27 DEC 2016
                AddAuditTrail("New Grade is not updated due to " + ex.Message);

                // Added by Rajas on 21 FEB 2017
                Error("New grade is not updated");

                return View(NewGradeObj);
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
                // Get List of grades
                VwGradeDetail GradeDetails = WetosDB.VwGradeDetails.Where(a => a.GradeId == id).FirstOrDefault();

                //ADDED BY RAJAS ON 27 DEC 2016
                AddAuditTrail("Grade details checked");

                return View(GradeDetails);
            }

            catch (System.Exception ex)
            {
                // Added by Rajas on 18 FEB 2017
                AddAuditTrail("Exception - " + ex.Message);

                Error("Inconsistent data. Please try again!");

                return RedirectToAction("Index");  // Verify ?
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
                Grade GradeObj = WetosDB.Grades.Where(a => a.GradeId == id).FirstOrDefault();

                List<Employee> EmployeeListObj = WetosDB.Employees.Where(a => a.CompanyId == id).ToList();

                if (EmployeeListObj.Count > 0)
                {
                    // Updated by Rajas on 15 MAY 2017 START
                    Information("You can not delete this Grade, as " + GradeObj.GradeName + " is assigned to employee");

                    AddAuditTrail("Can not delete Grade as " + GradeObj.GradeName + " is used in employee");
                    // Updated by Rajas on 15 MAY 2017 END

                    return RedirectToAction("Index");
                }

                if (GradeObj != null)
                {
                    GradeObj.MarkedAsDelete = 1;

                    WetosDB.SaveChanges();

                    Success("Grade : " + GradeObj.GradeName + " deleted successfully");

                    AddAuditTrail("Grade : " + GradeObj.GradeName + " deleted successfully");
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
        /// POPULATE ALL DROPDOWN LIST LIKE COMPANY
        /// Updated by Rajas on 27 FEB 2017
        /// </summary>
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

                //drop down for branch name list
                var BranchName = new List<Branch>(); //WetosDB.Branches.Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList();
                ViewBag.BranchNameList = new SelectList(BranchName, "BranchId", "BranchName").ToList();

                // drop down for employeed type name
                // Updated by Rajas on 6 MAY 2017 to get data which is not MarkedAsDelete
                var EmployeeTypeName = WetosDB.EmployeeTypes.Where(a => a.MarkedAsDelete == null || a.MarkedAsDelete == 0).Select(a => new { EmployeeTypeId = a.EmployeeTypeId, EmployeeTypeName = a.EmployeeTypeName }).ToList();
                ViewBag.EmployeeTypeNameList = new SelectList(EmployeeTypeName, "EmployeeTypeId", "EmployeeTypeName").ToList();
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Inconsistent data. Please try again");
            }
        }

        /// <summary>
        /// Updated by Rajas on 27 FEB 2017
        /// </summary>
        private void PopulateDropDownEdit(GradeModel NewGradeObj)
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

                //drop down for branch name list
                // Updated by Rajas on 6 MAY 2017 to get data which is not MarkedAsDelete
                var BranchName = WetosDB.Branches.Where(a => (a.MarkedAsDelete == null || a.MarkedAsDelete == 0) && a.Company.CompanyId == NewGradeObj.CompanyId)
                    .Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList();
                ViewBag.BranchNameList = new SelectList(BranchName, "BranchId", "BranchName").ToList();

                // drop down for employeed type name
                // Updated by Rajas on 6 MAY 2017 to get data which is not MarkedAsDelete
                var EmployeeTypeName = WetosDB.EmployeeTypes.Where(a => a.MarkedAsDelete == null || a.MarkedAsDelete == 0)
                    .Select(a => new { EmployeeTypeId = a.EmployeeTypeId, EmployeeTypeName = a.EmployeeTypeName }).ToList();
                ViewBag.EmployeeTypeNameList = new SelectList(EmployeeTypeName, "EmployeeTypeId", "EmployeeTypeName").ToList();
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
        public ActionResult ReportError(GradeModel NewGradeObj, string ErrorMessage)
        {
            PopulateDropDownEdit(NewGradeObj);//

            AddAuditTrail(ErrorMessage);
            Error(ErrorMessage);

            return View(NewGradeObj);
        }

        /// <summary>
        /// Common function to validate the data for edit or create
        /// Added by Rajas on 30 DEC 2016
        /// </summary>
        /// Updated by Rajas on 27 MARCH 2017
        private bool UpdateGradeData(GradeModel NewGradeObj, bool IsEdit, ref string UpdateStatus)
        {
            bool ReturnStatus = false;

            try
            {
                // Updated by Rajas on 31 MAY 2017
                WetosDB.Grade GradeTblObj = WetosDB.Grades.Where(a => (a.GradeName.Trim().ToUpper() == NewGradeObj.GradeName.Trim().ToUpper()
                    || a.GradeId == NewGradeObj.GradeId) && a.Company.CompanyId == NewGradeObj.CompanyId
                    && a.Branch.BranchId == NewGradeObj.BranchId && a.EmployeeType.EmployeeTypeId == NewGradeObj.EmployeeTypeId
                    && a.MarkedAsDelete == 0).FirstOrDefault();


                //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 9 SEPTEMBER 2017 FOR EDIT
                WetosDB.Grade GradeTblObjEDIT = WetosDB.Grades.Where(a => (a.GradeName.Trim().ToUpper() == NewGradeObj.GradeName.Trim().ToUpper()
                    || a.GradeId == NewGradeObj.GradeId) && a.Company.CompanyId == NewGradeObj.CompanyId
                    && a.Branch.BranchId == NewGradeObj.BranchId && a.EmployeeType.EmployeeTypeId == NewGradeObj.EmployeeTypeId
                    && a.MarkedAsDelete == 0).FirstOrDefault();

                // ADDED BY RAJAS ON 03 JUNE 2017 START
                bool IsNew = false;

                // IS OBJECT PRESET
                if (GradeTblObj != null)  // Added by Rajas on 2 JUNE 2017
                {
                    if (IsEdit == false)  // CREATE            
                    {
                        UpdateStatus = "Grade already available."; //WetosEmployeeController.GetErrorMessage(1);

                        //AddAuditTrail("Error in Department update : " + UpdateStatus);
                        return ReturnStatus;
                    }
                }
                else
                {
                    if (IsEdit == false) // CREATE    
                    {
                        GradeTblObj = new WetosDB.Grade();
                        IsNew = true;
                    }
                    else // EDIT    
                    {
                        UpdateStatus = "Error in updating Grade."; // WetosEmployeeController.GetErrorMessage(1); 
                        //AddAuditTrail("Error in Department update : " + UpdateStatus);
                        return ReturnStatus;
                    }
                }
                // ADDED BY RAJAS ON 03 JUNE 2017 END

                // New Leave table object
                GradeTblObj.GradeName = NewGradeObj.GradeName.Trim();



                //ADDED NEW CODE TO RESOLVE EmployeeType NULL ISSUE BY SHRADDHA ON 31 OCT 2017 START
                if (GradeTblObj.EmployeeType == null)
                {
                    GradeTblObj.EmployeeType = WetosDB.EmployeeTypes.Where(a => a.EmployeeTypeId == NewGradeObj.EmployeeTypeId).FirstOrDefault();
                }
                //GradeTblObj.EmployeeType.EmployeeTypeId = NewGradeObj.EmployeeTypeId;  // Updated by Rajas on EmployeeType.EmployeeTypeId replaced with EmployeeTypeId
                //ADDED NEW CODE TO RESOLVE EmployeeType NULL ISSUE BY SHRADDHA ON 31 OCT 2017 END

                //ADDED NEW CODE TO RESOLVE Company NULL ISSUE BY SHRADDHA ON 31 OCT 2017 START
                if (GradeTblObj.Company == null)
                {
                    GradeTblObj.Company = WetosDB.Companies.Where(a => a.CompanyId == NewGradeObj.CompanyId).FirstOrDefault();
                }
                //GradeTblObj.Company.CompanyId = NewGradeObj.CompanyId;  // Updated by Rajas on Company.CompanyId replaced with CompanyId
                //ADDED NEW CODE TO RESOLVE Company NULL ISSUE BY SHRADDHA ON 31 OCT 2017 END

                //ADDED NEW CODE TO RESOLVE Branch NULL ISSUE BY SHRADDHA ON 31 OCT 2017 START
                if (GradeTblObj.Branch == null)
                {
                    GradeTblObj.Branch = WetosDB.Branches.Where(a => a.BranchId == NewGradeObj.BranchId).FirstOrDefault();
                }
                //GradeTblObj.Branch.BranchId = NewGradeObj.BranchId;  // Updated by Rajas on Branch.BranchId replaced with Branch.BranchId 
                //ADDED NEW CODE TO RESOLVE Branch NULL ISSUE BY SHRADDHA ON 31 OCT 2017 END



                // Added by Rajas on 29 MARCH 2017
                GradeTblObj.MarkedAsDelete = 0;    // Default zero value


                // Add new table object 
                if (IsNew)
                {
                    WetosDB.Grades.Add(GradeTblObj);
                }

                WetosDB.SaveChanges();


                //-------------------------------------------AuditLog---------------------------------------------------------------------------

                if (IsNew)
                {
                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 11 SEPTEMBER 2017
                    #region ADD AUDIT LOG
                    //OLD RECORD IS BLANK
                    string Newrecord = "Grade Name :" + GradeTblObj.GradeName + ", EmployeeType ID :" + GradeTblObj.EmployeeType.EmployeeTypeId
                        + ", Company ID :" + GradeTblObj.Company.CompanyId + ", Branch ID :" + GradeTblObj.Branch.BranchId;


                    //LOGIN ID IS TAKEN FROM SESSION PERSISTER
                    string Formname = "GRADE MASTER";
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
                    string Oldrecord = "Grade Name :" + GradeTblObjEDIT.GradeName + ", EmployeeType ID :" + GradeTblObjEDIT.EmployeeType.EmployeeTypeId
                        + ", Company ID :" + GradeTblObjEDIT.Company.CompanyId + ", Branch ID :" + GradeTblObjEDIT.Branch.BranchId;
                    //---x---------x-----------------x------------x----------------x-----------x-------------x------------x--------------x----------x---------x-----
                    string Newrecord = "Grade Name :" + GradeTblObj.GradeName + ", EmployeeType ID :" + GradeTblObj.EmployeeType.EmployeeTypeId
                        + ", Company ID :" + GradeTblObj.Company.CompanyId + ", Branch ID :" + GradeTblObj.Branch.BranchId;


                    //LOGIN ID IS TAKEN FROM SESSION PERSISTER
                    string Formname = "GRADE MASTER";
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
                UpdateStatus = "Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message);

                return ReturnStatus;
            }
        }
    }
}
