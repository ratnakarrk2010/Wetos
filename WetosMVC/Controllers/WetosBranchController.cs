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
    public class WetosBranchController : BaseController
    {
        // 
        //
        // GET: /WetosBranch/

        public ActionResult Index()
        {
            try
            {
                List<VwBranchDetail> BranchList = WetosDB.VwBranchDetails.Where(a => a.MarkedAsDelete == 0 || a.MarkedAsDelete == null).ToList();

                //ADDED BY RAJAS ON 27 DEC 2016
                AddAuditTrail("Success - Visited branch master"); // Updated by Rajas on 16 JAN 2017

                return View(BranchList);
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
                VwBranchDetail BranchDetails = WetosDB.VwBranchDetails.Single(b => b.BranchId == id);

                //ADDED BY RAJAS ON 27 DEC 2016
                AddAuditTrail("Checked details for " + BranchDetails.BranchName); // Updated on 16 JAN 2017 by Rajas

                return View(BranchDetails);
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
        public ActionResult Create()
        {
            try
            {
                //GET LIST OF LOCATIONS
                //CODE FOR DROPDOWN
                PopulateDropDown();

                return View();
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Please try again!");

                return View();
            }
        }


        /// <summary>
        /// Updated by Rajas on 3 June 2017
        /// </summary>
        /// <param name="NewBranchObj"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(BranchModel NewBranchObj, FormCollection collection)
        {
            try
            {
                string UpdateStatus = string.Empty;

                // Added by Rajas on 15 MAY 2017
                bool IsEdit = false;

                if (ModelState.IsValid)  // Added by Rajas on 29 DEC 2016 for validation
                {
                    // Updated by Rajas on 15 MAY 2017
                    if (UpdateBranchData(NewBranchObj, IsEdit, ref UpdateStatus) == true)
                    {
                        // ADDED BY RAJAS FOR AuditLog ON 27 DEC 2016
                        AddAuditTrail("Success - Branch : " + NewBranchObj.BranchName + " is added successfully"); // Updated by Rajas on 16 JAN 2017


                        //ADDED CODE FOR SUCCESS MESSAGE BY RAJAS ON 16 JAN 2017
                        Success("Branch : " + NewBranchObj.BranchName + " is added Successfully"); // Updated by Rajas on 16 JAN 2017
                    }
                    else
                    {
                        return ReportError(NewBranchObj, UpdateStatus);
                    }

                    return RedirectToAction("Index");
                }

                else
                {
                    // Updated by Rajas on 2 JUNE 2017
                    PopulateDropDownEdit(NewBranchObj);

                    // Added by Rajas on 16 JAN 2017 START
                    //AddAuditTrail("Error - Branch : " + NewBranchObj.BranchName + " create failed");

                    //Error("Error - Add New branch failed");
                    // Added by Rajas on 16 JAN 2017 END

                    return View(NewBranchObj);
                }
            }

            catch (System.Exception ex)
            {
                // Added by Rajas on 16 JAN 2017 START
                AddAuditTrail("Exception - Branch create failed due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error(WetosErrorMessageController.GetErrorMessage(6));
                // Added by Rajas on 16 JAN 2017 END

                PopulateDropDownEdit(NewBranchObj);

                return View(NewBranchObj);
            }
        }

        //
        // GET: /WetosBranch/Edit/5

        /// <summary>
        /// Get validate data 
        /// Added by Rajas on 30 DEc 2016
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public ActionResult Edit(int id)
        {
            try
            {
                // GET EXISTNG BRANCH from BRNACHID
                Branch BranchEdit = WetosDB.Branches.Single(b => b.BranchId == id);

                BranchModel NewBranchObj = new BranchModel();
                NewBranchObj.BranchId = BranchEdit.BranchId;
                NewBranchObj.BranchName = BranchEdit.BranchName;
                NewBranchObj.CompanyId = BranchEdit.Company.CompanyId;
                NewBranchObj.Address1 = BranchEdit.Address1;
                NewBranchObj.Address2 = BranchEdit.Address2;
                NewBranchObj.Telephone = BranchEdit.Telephone;
                NewBranchObj.FAX = BranchEdit.FAX;
                NewBranchObj.Email = BranchEdit.Email;
                NewBranchObj.LocationId = BranchEdit.Location.LocationId;
                NewBranchObj.MarkedAsDelete = BranchEdit.MarkedAsDelete;

                // Updated by Rajas on 2 JUNE 2017
                PopulateDropDownEdit(NewBranchObj);

                return View(NewBranchObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error(WetosErrorMessageController.GetErrorMessage(6));

                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="NewBranchObj"></param>
        /// <returns></returns>
        /// UPDATED BY RAJAS ON 3 JUNE 2017
        [HttpPost]
        public ActionResult Edit(int id, BranchModel NewBranchObj)
        {
            try
            {
                string UpdateStatus = string.Empty;

                bool IsEdit = true;

                if (ModelState.IsValid)
                {
                    // GET EXISTING BRANCH 

                    // Updated by Rajas on 15 MAY 2017
                    if (UpdateBranchData(NewBranchObj, IsEdit, ref UpdateStatus) == true)
                    {
                        // ADDED BY RAJAS FOR AuditLog ON 27 DEC 2016
                        AddAuditTrail("Success - Branch : " + NewBranchObj.BranchName + " is updated"); // Updated by Rajas on 16 JAN 2017


                        //ADDED CODE FOR SUCCESS MESSAGE BY RAJAS ON 16 JAN 2017
                        Success("Branch : " + NewBranchObj.BranchName + " is updated Successfully"); // Updated by Rajas on 16 JAN 2017
                    }
                    else
                    {
                        return ReportError(NewBranchObj, UpdateStatus);

                    }

                    return RedirectToAction("Index");
                }
                else
                {

                    // Code added to check model state error list
                    var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();

                    // Updated by Rajas on 2 JUNE 2017
                    PopulateDropDownEdit(NewBranchObj);

                    // ADDED BY RAJAS FOR AuditLog ON 27 DEC 2016
                    //AddAuditTrail("Error - Branch update failed"); // Updated by Rajas on 30 MAY 2017


                    //ADDED CODE FOR SUCCESS MESSAGE BY RAJAS ON 16 JAN 2017
                    //Error("Branch update failed"); // Updated by Rajas on 30 MAY 2017

                    return View(NewBranchObj);

                }
            }
            catch (System.Exception ex)
            {
                // Updated by Rajas on 2 JUNE 2017
                PopulateDropDownEdit(NewBranchObj);

                // Added by Rajas on 16 JAN 2017 START
                AddAuditTrail("Exception - Branch update failed due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );  // Updated by Rajas on 30 MAY 2017

                Error(WetosErrorMessageController.GetErrorMessage(5));
                // Added by Rajas on 16 JAN 2017 END

                return View(NewBranchObj);
            }
        }

        /// <summary>
        /// DELETE GET
        /// Added by Rajas on 28 MARCH 2017
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            try
            {
                VwBranchDetail BranchDetails = WetosDB.VwBranchDetails.Single(b => b.BranchId == id);

                //ADDED BY RAJAS ON 27 DEC 2016
                AddAuditTrail("Checked details for " + BranchDetails.BranchName); // Updated on 16 JAN 2017 by Rajas

                return View(BranchDetails);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error(WetosErrorMessageController.GetErrorMessage(7));

                return RedirectToAction("Index");
            }
        }


        /// <summary>
        /// DELETE POST
        /// Added by Rajas on 28 MARCH 2017
        /// </summary>
        /// <param name="id"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                Branch BranchObj = WetosDB.Branches.Where(a => a.BranchId == id).FirstOrDefault();

                List<Employee> EmployeeListObj = WetosDB.Employees.Where(a => a.BranchId == id).ToList();

                if (EmployeeListObj.Count > 0)
                {
                    // Updated by Rajas on 15 MAY 2017 START
                    Information("You can not delete this branch, as " + BranchObj.BranchName + " it is assigned to employee");

                    AddAuditTrail("Can not delete branch as " + BranchObj.BranchName + " it is assigned to employee");
                    // Updated by Rajas on 15 MAY 2017 END

                    return RedirectToAction("Index");
                }

                if (BranchObj != null)
                {
                    BranchObj.MarkedAsDelete = 1;

                    WetosDB.SaveChanges();

                    Success("Branch " + BranchObj.BranchName + " deleted successfully");

                    AddAuditTrail("Branch " + BranchObj.BranchName + " deleted successfully");
                }

                return RedirectToAction("Index");
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error(WetosErrorMessageController.GetErrorMessage(7));

                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Json return for to get Company dropdown list on basis of location selection
        /// Added by Rajas on 27 DEC 2016
        /// Function updated by Rajas on 27 FEB 2017
        /// </summary>
        /// <returns></returns>
        public JsonResult GetCompany(string Locationid)
        {

            int SelLocationId = 0;
            if (!string.IsNullOrEmpty(Locationid))
            {
                if (Locationid.ToUpper() != "NULL")
                {
                    SelLocationId = Convert.ToInt32(Locationid);
                }
            }

            var CompanyList = WetosDB.Companies.Where(a => a.Location.LocationId == SelLocationId && a.MarkedAsDelete == 0).Select(a => new { CompanyId = a.CompanyId, CompanyName = a.CompanyName }).ToList();
            
            
            return Json(CompanyList);
        }

        /// <summary>
        /// Common function to validate the data for edit or create
        /// Added by Rajas on 30 DEC 2016
        /// </summary>
        private bool UpdateBranchData(BranchModel NewBranchObj, bool IsEdit, ref string UpdateStatus)
        {
            bool ReturnStatus = false;

            try
            {
                // Updated by Rajas on 20 JULY 2017 to resolve Defect Id 2 from Defect Sheet by Ulka(Eviska)
                WetosDB.Branch BranchTblObj = WetosDB.Branches.Where(a => (a.BranchName.Trim().ToUpper() == NewBranchObj.BranchName.Trim().ToUpper() || a.BranchId == NewBranchObj.BranchId)
                    && a.Company.CompanyId == NewBranchObj.CompanyId && a.Location.LocationId == NewBranchObj.LocationId && a.MarkedAsDelete == 0).FirstOrDefault();

                //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 11 SEPTEMBER 2017
                WetosDB.Branch BranchTblObjEDIT = WetosDB.Branches.Where(a => (a.BranchName.Trim().ToUpper() == NewBranchObj.BranchName.Trim().ToUpper() || a.BranchId == NewBranchObj.BranchId)
                    && a.Company.CompanyId == NewBranchObj.CompanyId && a.Location.LocationId == NewBranchObj.LocationId && a.MarkedAsDelete == 0).FirstOrDefault();

                // ADDED BY RAJAS ON 03 JUNE 2017 START
                bool IsNew = false;

                // IS OBJECT PRESET
                if (BranchTblObj != null)  // Added by Rajas on 2 JUNE 2017
                {
                    if (IsEdit == false)  // CREATE            
                    {
                        UpdateStatus = WetosErrorMessageController.GetErrorMessage(8);

                        //AddAuditTrail("Error in Department update : " + UpdateStatus);
                        return ReturnStatus;
                    }
                }
                else
                {
                    if (IsEdit == false) // CREATE    
                    {
                        BranchTblObj = new WetosDB.Branch();
                        IsNew = true;
                    }
                    else // EDIT    
                    {
                        UpdateStatus = WetosErrorMessageController.GetErrorMessage(6);
                        //AddAuditTrail("Error in Department update : " + UpdateStatus);
                        return ReturnStatus;
                    }
                }
                // ADDED BY RAJAS ON 03 JUNE 2017 END

                // New Leave table object
                BranchTblObj.BranchName = NewBranchObj.BranchName.Trim();

                //ADDED NEW CODE TO RESOLVE COMPANY NULL ISSUE BY SHRADDHA ON 31 OCT 2017 START
                if (BranchTblObj.Company == null)
                {
                    BranchTblObj.Company = WetosDB.Companies.Where(a => a.CompanyId == NewBranchObj.CompanyId).FirstOrDefault();
                }
                //BranchTblObj.Company.CompanyId = NewBranchObj.CompanyId;  // Updated by Rajas on 21 FEB 2017 Company.CompanyId replaced with CompanyId
                //ADDED NEW CODE TO RESOLVE COMPANY NULL ISSUE BY SHRADDHA ON 31 OCT 2017 END


                BranchTblObj.Address1 = NewBranchObj.Address1;

                BranchTblObj.Address2 = NewBranchObj.Address2;

                BranchTblObj.Telephone = NewBranchObj.Telephone;

                BranchTblObj.FAX = NewBranchObj.FAX;

                BranchTblObj.Email = NewBranchObj.Email; // Updated by Rajas on 14 FEB 2017



                //ADDED NEW CODE TO RESOLVE Location NULL ISSUE BY SHRADDHA ON 31 OCT 2017 START
                if (BranchTblObj.Location == null)
                {
                    BranchTblObj.Location = WetosDB.Locations.Where(a => a.LocationId == NewBranchObj.LocationId).FirstOrDefault();
                }
                //BranchTblObj.Location.LocationId = NewBranchObj.LocationId; // Updated by Rajas on 21 FEB 2017 replaced LocationId with Location.LocationId
                //ADDED NEW CODE TO RESOLVE Location NULL ISSUE BY SHRADDHA ON 31 OCT 2017 END

                // Added by Rajas on 28 MARCH 2017
                BranchTblObj.MarkedAsDelete = 0;  // By default value zero


                // Add new table object 
                if (IsNew)
                {
                    WetosDB.Branches.Add(BranchTblObj);
                }

                WetosDB.SaveChanges();


                //-------------------------------------------AuditLog---------------------------------------------------------------------------

                if (IsNew)
                {
                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 11 SEPTEMBER 2017
                    #region ADD AUDIT LOG
                    //OLD RECORD IS BLANK
                    string Newrecord = "Branch Name :"+BranchTblObj.BranchName+", Company ID :"+BranchTblObj.Company.CompanyId
                        +", Location ID :"+BranchTblObj.Location.LocationId+", Address1 :"+BranchTblObj.Address1
                        +", Address2 :"+BranchTblObj.Address2+", Telephone :"+BranchTblObj.Telephone
                        +", Fax :"+BranchTblObj.FAX+", Email :"+BranchTblObj.Email;

                    //LOGIN ID IS TAKEN FROM SESSION PERSISTER
                    string Formname = "BRANCH MASTER";
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
                    string Oldrecord = "Branch Name :" + BranchTblObjEDIT.BranchName + ", Company ID :" + BranchTblObjEDIT.Company.CompanyId
                        + ", Location ID :" + BranchTblObjEDIT.Location.LocationId + ", Address1 :" + BranchTblObjEDIT.Address1
                        + ", Address2 :" + BranchTblObjEDIT.Address2 + ", Telephone :" + BranchTblObjEDIT.Telephone
                        + ", Fax :" + BranchTblObjEDIT.FAX + ", Email :" + BranchTblObjEDIT.Email;
                    //---x---------x-----------------x------------x----------------x-----------x-------------x------------x--------------x----------x---------x-----
                    string Newrecord = "Branch Name :" + BranchTblObj.BranchName + ", Company ID :" + BranchTblObj.Company.CompanyId
                        + ", Location ID :" + BranchTblObj.Location.LocationId + ", Address1 :" + BranchTblObj.Address1
                        + ", Address2 :" + BranchTblObj.Address2 + ", Telephone :" + BranchTblObj.Telephone
                        + ", Fax :" + BranchTblObj.FAX + ", Email :" + BranchTblObj.Email;

                    //LOGIN ID IS TAKEN FROM SESSION PERSISTER
                    string Formname = "BRANCH MASTER";
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
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                UpdateStatus = "Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message);

                return ReturnStatus;
            }
        }

        /// <summary>
        /// Common Error reporting function
        /// Added by Rajas on 3 JUNE 2017
        /// </summary>
        /// <param name="DepartmentObj"></param>
        /// <param name="ErrorMessage"></param>
        /// <returns></returns>
        public ActionResult ReportError(BranchModel NewBranchObj, string ErrorMessage)
        {
            PopulateDropDownEdit(NewBranchObj);//

            AddAuditTrail(ErrorMessage);
            Error(ErrorMessage);

            return View(NewBranchObj);
        }

        /// <summary>
        /// Added by Rajas on 16 JAN 2017
        /// </summary>
        private void PopulateDropDown()
        {
            try
            {
                // Updated by Rajas on 6 MAY 2017 to display data which is not MarkedAsDelete START
                var Location = WetosDB.Locations.Where(a => a.MarkedAsDelete == null || a.MarkedAsDelete == 0).Select(a => new { LocationId = a.LocationId, LocationName = a.LocationName }).ToList();
                ViewBag.LocatioList = new SelectList(Location, "LocationId", "LocationName").ToList();


                var Company = new List<WetosDB.Company>(); //WetosDB.Companies.Where(a => a.MarkedAsDelete == null || a.MarkedAsDelete == 0).Select(a => new { CompanyId = a.CompanyId, CompanyName = a.CompanyName }).ToList();
                ViewBag.CompanyList = new SelectList(Company, "CompanyId", "CompanyName").ToList();
                // Updated by Rajas on 6 MAY 2017 to display data which is not MarkedAsDelete END
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error(WetosErrorMessageController.GetErrorMessage(9));
            }
        }

        /// <summary>
        /// Added by Rajas on 27 FEB 2017
        /// </summary>
        private void PopulateDropDownEdit(BranchModel NewBranchObj)
        {
            try
            {
                // Updated by Rajas on 6 MAY 2017 to display data which is not MarkedAsDelete START
                var Location = WetosDB.Locations.Where(a => a.MarkedAsDelete == null || a.MarkedAsDelete == 0).Select(a => new { LocationId = a.LocationId, LocationName = a.LocationName }).ToList();
                ViewBag.LocatioList = new SelectList(Location, "LocationId", "LocationName").ToList();

                var Company = WetosDB.Companies.Where(a => (a.MarkedAsDelete == null || a.MarkedAsDelete == 0 && a.Location.LocationId == NewBranchObj.LocationId)).Select(a => new { CompanyId = a.CompanyId, CompanyName = a.CompanyName }).ToList();
                ViewBag.CompanyList = new SelectList(Company, "CompanyId", "CompanyName").ToList();
                // Updated by Rajas on 6 MAY 2017 to display data which is not MarkedAsDelete END
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error(WetosErrorMessageController.GetErrorMessage(9));
            }
        }
    }
}
