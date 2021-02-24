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
    public class WetosEmployeeGroupController : BaseController
    {
        
        //
        // GET: /WetosEmployeeGroup/

        public ActionResult Index()
        {
            try
            {
                // Updated by Rajas on 29 MARCH 2017
                List<VwEmployeeGroupDetail> EmployeeGroupList = WetosDB.VwEmployeeGroupDetails.Where(a => a.MarkedAsDelete == 0 || a.MarkedAsDelete == null).ToList();

                //ADDED BY RAJAS ON 27 DEC 2016
                AddAuditTrail("Success - Visited Employee group master"); // Updated by Rajas on 16 JAN 2017


                return View(EmployeeGroupList);
            }

            catch (System.Exception ex)
            {
                // Added by Rajas on 16 JAN 2017
                AddAuditTrail("Exception - " + ex.Message);

                return View();
            }
        }

        /// <summary>
        /// Updated by Rajas on 3 JUNE 2017
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int id)
        {
            try
            {
                VwEmployeeGroupDetail EmployeeGroupDetails = WetosDB.VwEmployeeGroupDetails.Single(b => b.EmployeeGroupId == id);

                //ADDED BY RAJAS ON 27 DEC 2016
                AddAuditTrail("Checked details for " + EmployeeGroupDetails.EmployeeGroupName); // Updated on 16 JAN 2017 by Rajas

                return View(EmployeeGroupDetails);
            }

            catch (System.Exception ex)
            {
                // Added by Rajas on 16 JAN 2017
                AddAuditTrail("Exception - " + ex.Message);

                return RedirectToAction("Index");
            }
        }

        //
        // GET: /WetosEmployeeGroup/Create

        public ActionResult Create()
        {
            try
            {

                // Added by Rajas on 16 JAN 2017
                PopulateDropDown();

                return View();
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Inconsistent data. Please try again!");

                return View();
            }
        }

        /// <summary>
        /// UPDATED BY RAJAS ON 3 JUNE 2017
        /// </summary>
        /// <param name="NewEmployeeGroupObj"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(EmployeeGroupModel NewEmployeeGroupObj, FormCollection collection)
        {
            try
            {
                string UpdateStatus = String.Empty;

                // Added by Rajas on 15 MAY 2017
                bool IsEdit = false;

                if (ModelState.IsValid)
                {
                    // Updated by Rajas on 15 MAY 2017
                    if (UpdateEmployeeGroupData(NewEmployeeGroupObj, IsEdit, ref UpdateStatus) == true)
                    {
                        // ADDED BY RAJAS FOR AUDITLOG ON 27 DEC 2016
                        AddAuditTrail("Success - Employee group : " + NewEmployeeGroupObj.EmployeeGroupName + " is added Successfully"); // Updated by Rajas on 16 JAN 2017


                        //ADDED CODE FOR SUCCESS MESSAGE BY RAJAS ON 16 JAN 2017
                        Success("Success - Employee group : " + NewEmployeeGroupObj.EmployeeGroupName + " is added Successfully"); // Updated by Rajas on 16 JAN 2017
                    }
                    else
                    {
                        return ReportError(NewEmployeeGroupObj, UpdateStatus);
                    }

                    //return RedirectToAction("Create");

                    // Above line commented and below line added by Rajas on 16 JAN 2017 for redirection to index page 
                    return RedirectToAction("Index");
                }

                else
                {
                    // Added by Rajas on 16 JAN 2017
                    PopulateDropDownEdit(NewEmployeeGroupObj);

                    // Added by Rajas on 16 JAN 2017 START
                    //AddAuditTrail("Error - Employee group create failed"); // Updated by Rajas on 30 MAY 2017

                    //Error("Error - Employee group create failed");  // Updated by Rajas on 30 MAY 2017
                    // Added by Rajas on 16 JAN 2017 END

                    return View(NewEmployeeGroupObj);
                }
            }
            catch (System.Exception ex)
            {
                // Added by Rajas on 16 JAN 2017 START
                AddAuditTrail("Exception - Employee group create failed due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) ); // Updated by Rajas on 30 MAY 2017

                Error("Error - Employee group create failed");  // Updated by Rajas on 30 MAY 2017
                // Added by Rajas on 16 JAN 2017 END

                PopulateDropDownEdit(NewEmployeeGroupObj);

                return View(NewEmployeeGroupObj);
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
                EmployeeGroup EmployeeGroupEdit = new EmployeeGroup();
                EmployeeGroupEdit = WetosDB.EmployeeGroups.Single(b => b.EmployeeGroupId == id);

                EmployeeGroupModel NewEmployeeGroupObj = new EmployeeGroupModel();

                NewEmployeeGroupObj.EmployeeGroupId = EmployeeGroupEdit.EmployeeGroupId;

                NewEmployeeGroupObj.EmployeeGroupName = EmployeeGroupEdit.EmployeeGroupName;

                NewEmployeeGroupObj.CompanyId = EmployeeGroupEdit.Company.CompanyId;  // Updated by Rajas on Company.CompanyId replaced with CompanyId

                NewEmployeeGroupObj.BranchId = EmployeeGroupEdit.Branch.BranchId;  // Updated by Rajas on Branch.BranchId replaced with Branch.BranchId 

                // Added by Rajas on 29 MARCH 2017
                NewEmployeeGroupObj.MarkedAsDelete = EmployeeGroupEdit.MarkedAsDelete;

                // Added by Rajas on 16 JAN 2017
                PopulateDropDownEdit(NewEmployeeGroupObj);

                return View(NewEmployeeGroupObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Inconsistent data. Please try again!");

                return RedirectToAction("Index");  // Verify ?
            }
        }


        /// <summary>
        /// Updated by Rajas on 3 JUNE 2017
        /// </summary>
        /// <param name="id"></param>
        /// <param name="NewEmployeeGroupObj"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(int id, EmployeeGroupModel NewEmployeeGroupObj)
        {
            try
            {
                // Added by Rajas on 27 MARCH 2017
                string UpdateStatus = string.Empty;

                // Added by Rajas on 15 MAY 2017
                bool IsEdit = true;  // Updated by Rajas on 30 MAY 2017

                if (ModelState.IsValid)
                {
                    // Updated by Rajas on 15 MAY 2017
                    if (UpdateEmployeeGroupData(NewEmployeeGroupObj, IsEdit, ref UpdateStatus) == true)
                    {
                        // ADDED BY RAJAS FOR AUDITLOG ON 27 DEC 2016
                        AddAuditTrail("Success - Employee group : " + NewEmployeeGroupObj.EmployeeGroupName + " is updated Successfully"); // Updated by Rajas on 16 JAN 2017


                        //ADDED CODE FOR SUCCESS MESSAGE BY RAJAS ON 16 JAN 2017
                        Success("Success - Employee group : " + NewEmployeeGroupObj.EmployeeGroupName + " is updated Successfully"); // Updated by Rajas on 16 JAN 2017
                    }
                    else
                    {
                        return ReportError(NewEmployeeGroupObj, UpdateStatus);
                    }

                    return RedirectToAction("Index");
                }
                else
                {
                    // Added by Rajas on 16 JAN 2017
                    PopulateDropDownEdit(NewEmployeeGroupObj);

                    // Added by Rajas on 16 JAN 2017 START
                    //AddAuditTrail("Error - Employee group update failed"); // Updated by Rajas on 30 MAY 2017

                    //Error("Error - Employee group update failed");  // Updated by Rajas on 30 MAY 2017
                    // Added by Rajas on 16 JAN 2017 END

                    return View(NewEmployeeGroupObj);
                }

            }
            catch (System.Exception ex)
            {
                PopulateDropDownEdit(NewEmployeeGroupObj);

                // Added by Rajas on 16 JAN 2017 START
                AddAuditTrail("Error - Employee group update failed due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) ); // Updated by Rajas on 30 MAY 2017

                Error("Error - Employee group create failed");  // Updated by Rajas on 30 MAY 2017
                // Added by Rajas on 16 JAN 2017 END

                return View(NewEmployeeGroupObj);
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
                VwEmployeeGroupDetail EmployeeGroupDetails = WetosDB.VwEmployeeGroupDetails.Where(a => a.MarkedAsDelete == 0 || a.MarkedAsDelete == null)
                    .Single(b => b.EmployeeGroupId == id);

                //ADDED BY RAJAS ON 27 DEC 2016
                AddAuditTrail("Checked details for " + EmployeeGroupDetails.EmployeeGroupName); // Updated on 16 JAN 2017 by Rajas

                return View(EmployeeGroupDetails);
            }

            catch (System.Exception ex)
            {
                // Added by Rajas on 16 JAN 2017
                AddAuditTrail("Exception - " + ex.Message);

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
                EmployeeGroup EmployeeGroupObj = WetosDB.EmployeeGroups.Where(a => a.EmployeeGroupId == id).FirstOrDefault();

                List<EmployeeGroupDetail> EmployeeListObj = WetosDB.EmployeeGroupDetails.Where(a => a.EmployeeGroup.EmployeeGroupId == id).ToList();

                if (EmployeeListObj.Count > 0)
                {
                    // Updated by Rajas on 15 MAY 2017 START
                    Information("You can not delete this Employee group, as " + EmployeeGroupObj.EmployeeGroupName + " is assigned to employee");

                    AddAuditTrail("Can not delete Employee group as " + EmployeeGroupObj.EmployeeGroupName + " is used in employee");
                    // Updated by Rajas on 15 MAY 2017 END

                    return RedirectToAction("Index");
                }

                if (EmployeeGroupObj != null)
                {
                    EmployeeGroupObj.MarkedAsDelete = 1;

                    WetosDB.SaveChanges();

                    Success("Employee Group : " + EmployeeGroupObj.EmployeeGroupName + " deleted successfully");

                    AddAuditTrail("Employee Group : " + EmployeeGroupObj.EmployeeGroupName + " deleted successfully");
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
            // var BranchList = WetosDB.Branches.Where(a => a.Company.CompanyId == SelCompanyId && (a.MarkedAsDelete == null || a.MarkedAsDelete == 0))
            //.Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList();
            var BranchList = WetosDB.SP_GetBranchList(EmployeeId).Where(a => a.CompanyId == SelCompanyId).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList(); //.Select(m => new { BranchId = m.BranchId, BranchName = m.BranchName}).ToList();
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
            #endregion

            return Json(BranchList);
        }

        /// <summary>
        /// Common Error reporting function
        /// Added by Rajas on 3 JUNE 2017
        /// </summary>
        /// <param name="DepartmentObj"></param>
        /// <param name="ErrorMessage"></param>
        /// <returns></returns>
        public ActionResult ReportError(EmployeeGroupModel NewEmployeeGroupObj, string ErrorMessage)
        {
            PopulateDropDownEdit(NewEmployeeGroupObj);//

            AddAuditTrail(ErrorMessage);
            Error(ErrorMessage);

            return View(NewEmployeeGroupObj);
        }

        /// <summary>
        /// Common function to validate the data for edit or create
        /// Added by Rajas on 30 DEC 2016
        /// </summary>
        /// Updated by Rajas on 27 MARCH 2017
        private bool UpdateEmployeeGroupData(EmployeeGroupModel NewEmployeeGroupObj, bool IsEdit, ref string UpdateStatus)
        {
            bool ReturnStatus = false;

            try
            {
                // Updated by Rajas on 31 MAY 2017
                WetosDB.EmployeeGroup EmployeeGroupTblObj = WetosDB.EmployeeGroups.Where(a => (a.EmployeeGroupName.Trim().ToUpper() == NewEmployeeGroupObj.EmployeeGroupName.Trim().ToUpper() || a.EmployeeGroupId == NewEmployeeGroupObj.EmployeeGroupId)
                    && a.Company.CompanyId == NewEmployeeGroupObj.CompanyId && a.Branch.BranchId == NewEmployeeGroupObj.BranchId && a.MarkedAsDelete == 0).FirstOrDefault();

                //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 9 SEPTEMBER 2017 FOR EDIT
                WetosDB.EmployeeGroup EmployeeGroupTblObjEDIT = WetosDB.EmployeeGroups.Where(a => (a.EmployeeGroupName.Trim().ToUpper() == NewEmployeeGroupObj.EmployeeGroupName.Trim().ToUpper() || a.EmployeeGroupId == NewEmployeeGroupObj.EmployeeGroupId)
                    && a.Company.CompanyId == NewEmployeeGroupObj.CompanyId && a.Branch.BranchId == NewEmployeeGroupObj.BranchId && a.MarkedAsDelete == 0).FirstOrDefault();

                // ADDED BY RAJAS ON 03 JUNE 2017 START
                bool IsNew = false;

                // IS OBJECT PRESET
                if (EmployeeGroupTblObj != null)  // Added by Rajas on 2 JUNE 2017
                {
                    if (IsEdit == false)  // CREATE            
                    {
                        UpdateStatus = "Employee Group already available."; //WetosEmployeeController.GetErrorMessage(1);

                        //AddAuditTrail("Error in Department update : " + UpdateStatus);
                        return ReturnStatus;
                    }
                }
                else
                {
                    if (IsEdit == false) // CREATE    
                    {
                        EmployeeGroupTblObj = new WetosDB.EmployeeGroup();
                        IsNew = true;
                    }
                    else // EDIT    
                    {
                        UpdateStatus = "Error in updating Employee group."; // WetosEmployeeController.GetErrorMessage(1); 
                        //AddAuditTrail("Error in Department update : " + UpdateStatus);
                        return ReturnStatus;
                    }
                }
                // ADDED BY RAJAS ON 03 JUNE 2017 END

                // New Leave table object
                EmployeeGroupTblObj.EmployeeGroupName = NewEmployeeGroupObj.EmployeeGroupName.Trim();



                //ADDED NEW CODE TO RESOLVE Company NULL ISSUE BY SHRADDHA ON 31 OCT 2017 START
                if (EmployeeGroupTblObj.Company == null)
                {
                    EmployeeGroupTblObj.Company = WetosDB.Companies.Where(a => a.CompanyId == NewEmployeeGroupObj.CompanyId).FirstOrDefault();
                }
                //EmployeeGroupTblObj.Company.CompanyId = NewEmployeeGroupObj.CompanyId;   // Updated by Rajas on Company.CompanyId replaced with CompanyId
                //ADDED NEW CODE TO RESOLVE Company NULL ISSUE BY SHRADDHA ON 31 OCT 2017 END

                //ADDED NEW CODE TO RESOLVE Branch NULL ISSUE BY SHRADDHA ON 31 OCT 2017 START
                if (EmployeeGroupTblObj.Branch == null)
                {
                    EmployeeGroupTblObj.Branch = WetosDB.Branches.Where(a => a.BranchId == NewEmployeeGroupObj.BranchId).FirstOrDefault();
                }
                //EmployeeGroupTblObj.Branch.BranchId = NewEmployeeGroupObj.BranchId;   // Updated by Rajas on Branch.BranchId replaced with Branch.BranchId
                //ADDED NEW CODE TO RESOLVE Branch NULL ISSUE BY SHRADDHA ON 31 OCT 2017 END


                // Added by Rajas on 29 MARCH 2017
                EmployeeGroupTblObj.MarkedAsDelete = 0; // Default value


                // Add new table object 
                if (IsNew)
                {
                    WetosDB.EmployeeGroups.AddObject(EmployeeGroupTblObj);
                }

                WetosDB.SaveChanges();


                //-------------------------------------------AUDITLOG---------------------------------------------------------------------------

                if (IsNew)
                {
                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 11 SEPTEMBER 2017
                    #region ADD AUDIT LOG
                    //OLD RECORD IS BLANK
                    string Newrecord = "EmployeeGroup Name :" + EmployeeGroupTblObj.EmployeeGroupName + ", Branch ID :" + EmployeeGroupTblObj.Branch.BranchId
                        + ", Company ID :" + EmployeeGroupTblObj.Company.CompanyId;


                    //LOGIN ID IS TAKEN FROM SESSION PERSISTER
                    string Formname = "EMPLOYEEGROUP MASTER";
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
                    string Oldrecord = "EmployeeGroup Name :" + EmployeeGroupTblObjEDIT.EmployeeGroupName + ", Branch ID :" + EmployeeGroupTblObjEDIT.Branch.BranchId
                        + ", Company ID :" + EmployeeGroupTblObjEDIT.Company.CompanyId;
                    //---x---------x-----------------x------------x----------------x-----------x-------------x------------x--------------x----------x---------x-----
                    string Newrecord = "EmployeeGroup Name :" + EmployeeGroupTblObj.EmployeeGroupName + ", Branch ID :" + EmployeeGroupTblObj.Branch.BranchId
                        + ", Company ID :" + EmployeeGroupTblObj.Company.CompanyId;


                    //LOGIN ID IS TAKEN FROM SESSION PERSISTER
                    string Formname = "EMPLOYEEGROUP MASTER";
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
        /// </summary>
        private void PopulateDropDown()
        {
            try
            {
                // Updated by Rajas on 6 MAY 2017 to get data which is not MarkedAsDelete
                // company
                #region COMPANY LIST CODE ADDED BY SHRADDHA ON 30 MAR 2018
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //var CompanyObj = WetosDB.Companies.Select(a => new { CompanyId = a.CompanyId, CompanyName = a.CompanyName }).ToList();
                var CompanyName = WetosDB.SP_GetCompanyList(EmployeeId).Select(m => new { Companyid = m.CompanyId, CompanyName = m.CompanyName }).ToList();
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
                #endregion
                ViewBag.CompanyList = new SelectList(CompanyName, "CompanyId", "CompanyName").ToList();

                // branch
                var Branch = new List<Branch>(); //WetosDB.Branches.Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList();
                ViewBag.BranchList = new SelectList(Branch, "BranchId", "BranchName").ToList();
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Inconsistent data. Please verify and try again!");
            }
        }

        /// <summary>
        /// Added by Rajas on 27 FEB 2017
        /// </summary>
        private void PopulateDropDownEdit(EmployeeGroupModel NewEmployeeGroupObj)
        {
            try
            {
                // company
                // Updated by Rajas on 6 MAY 2017 to get data which is not MarkedAsDelete

                #region COMPANY LIST CODE ADDED BY SHRADDHA ON 30 MAR 2018
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //var CompanyObj = WetosDB.Companies.Select(a => new { CompanyId = a.CompanyId, CompanyName = a.CompanyName }).ToList();
                var CompanyName = WetosDB.SP_GetCompanyList(EmployeeId).Select(m => new { Companyid = m.CompanyId, CompanyName = m.CompanyName }).ToList();
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
                #endregion

                ViewBag.CompanyList = new SelectList(CompanyName, "CompanyId", "CompanyName").ToList();

                // branch
                // Updated by Rajas on 6 MAY 2017 to get data which is not MarkedAsDelete

                #region CODE FOR GET BRANCH LIST ADDED BY SHRADDHA ON 30 MAR 2018
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
                //var Branch = WetosDB.Branches.Where(a => (a.MarkedAsDelete == null || a.MarkedAsDelete == 0) && a.Company.CompanyId == )
                //.Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList();
                var BranchList = WetosDB.SP_GetBranchList(EmployeeId).Where(a => a.CompanyId == NewEmployeeGroupObj.CompanyId).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList(); //.Select(m => new { BranchId = m.BranchId, BranchName = m.BranchName}).ToList();
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
                #endregion
                ViewBag.BranchList = new SelectList(BranchList, "BranchId", "BranchName").ToList();
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Inconsistent data. Please verify and try again!");
            }
        }
    }
}
