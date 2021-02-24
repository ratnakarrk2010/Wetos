using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WetosDB;
using WetosMVCMainApp.Models;

namespace WetosMVC.Controllers
{
    [SessionExpire] 
    [Authorize]
    public class WetosCompanyController : BaseController
    {
        // 

        // GET: /WetosCompany/

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            try
            {
                List<WetosDB.VwLocationNameInCompany> CompaniesList = WetosDB.VwLocationNameInCompanies.Where(a => a.MarkedAsDelete == 0 || a.MarkedAsDelete == null).ToList();

                //ADDED BY RAJAS ON 27 DEC 2016
                AddAuditTrail("Success - Visited company master"); // Updated by Rajas on 16 JAN 2017

                return View(CompaniesList);
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
                WetosDB.VwLocationNameInCompany CompanyDetails = WetosDB.VwLocationNameInCompanies.Single(b => b.CompanyId == id);

                //ADDED BY RAJAS ON 27 DEC 2016
                AddAuditTrail("Checked details for " + CompanyDetails.CompanyName); // Updated on 16 JAN 2017 by Rajas

                return View(CompanyDetails);
            }

            catch (System.Exception ex)
            {
                // Added by Rajas on 16 JAN 2017
                AddAuditTrail("Exception - " + ex.Message);

                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Updated by Rajas on 6 JUNE 2017
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            // Initialize CompanyModelObj
            CompanyModel CompanyModelObj = new CompanyModel();

            try
            {
                PopulateDropdown();

                return View(CompanyModelObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                return View(CompanyModelObj);  // Verify ?
            }
        }

        /// <summary>
        /// Posting with the help of companymodel, for validation of data
        /// UPDATED BY RAJAS ON 3 JUNE 2017
        /// </summary>
        /// <param name="NewCompanyObj"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(CompanyModel NewCompanyObj, FormCollection collection)
        {
            try
            {
                string UpdateStatus = string.Empty;  // Added by Rajas on 24 MARCH 2017

                // Added by Rajas on 15 MAY 2017
                bool IsEdit = false;

                if (ModelState.IsValid)
                {
                    if (UpdateCompanyData(NewCompanyObj, IsEdit, ref UpdateStatus) == true)
                    {

                        // ADDED BY RAJAS FOR AuditLog ON 27 DEC 2016
                        AddAuditTrail("Success - Company : " + NewCompanyObj.CompanyName + " is added"); // Updated by Rajas on 16 JAN 2017


                        //ADDED CODE FOR SUCCESS MESSAGE BY SHRADDHA ON 16 JAN 2017
                        Success("Company : " + NewCompanyObj.CompanyName + " is added Successfully"); // Updated by Rajas on 16 JAN 2017
                    }
                    else
                    {
                        return ReportError(NewCompanyObj, UpdateStatus);

                    }

                    return RedirectToAction("Index");
                }

                else
                {
                    PopulateDropdown();

                    // Added by Rajas on 16 JAN 2017 START
                    //AddAuditTrail("Error - Company : " + NewCompanyObj.CompanyName + " create failed");

                    //Error("Error - Add New company failed");
                    // Added by Rajas on 16 JAN 2017 END

                    return View(NewCompanyObj);
                }

            }
            catch (System.Exception ex)
            {
                // Added by Rajas on 16 JAN 2017 START
                AddAuditTrail("Exception - Comapny : " + NewCompanyObj.CompanyName + " create failed due to " + ex.Message);

                Error(WetosErrorMessageController.GetErrorMessage(2));  //("Error - Company create failed");
                // Added by Rajas on 16 JAN 2017 END

                PopulateDropdown();

                return View(NewCompanyObj);
            }
        }

        //
        // GET: /WetosCompany/Edit/5

        /// <summary>
        /// Validate the edit function through model
        /// Added by Rajas on 30 DEC 2016
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            try
            {
                WetosDB.Company CompanyEdit = new WetosDB.Company();

                CompanyEdit = WetosDB.Companies.Single(b => b.CompanyId == id);

                // Addded by Rajas on 30 DEC 2016 for Saving data from Table object to Model object 
                CompanyModel CompanyModelObj = new CompanyModel();
                CompanyModelObj.CompanyId = CompanyEdit.CompanyId;
                CompanyModelObj.CompanyName = CompanyEdit.CompanyName;
                CompanyModelObj.LocationId = CompanyEdit.Location.LocationId;
                CompanyModelObj.Address1 = CompanyEdit.Address1;
                CompanyModelObj.Address2 = CompanyEdit.Address2;
                CompanyModelObj.Telephone = CompanyEdit.Telephone;
                CompanyModelObj.Fax = CompanyEdit.Fax;
                CompanyModelObj.Email = CompanyEdit.Email;
                CompanyModelObj.PAN = CompanyEdit.PAN;

                // Added by Rajas on 28 MARCH 2017 
                CompanyModelObj.MarkedAsDelete = CompanyEdit.MarkedAsDelete;

                PopulateDropdown();

                return View(CompanyModelObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in Company edit due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error(WetosErrorMessageController.GetErrorMessage(1));

                return RedirectToAction("Index");  // Verify ?
            }
        }

        //
        // POST: /WetosCompany/Edit/5

        /// <summary>
        /// Updated by Rajas on 3 JUNE 2017 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="CompanyModelObj"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(int id, CompanyModel CompanyModelObj)
        {
            try
            {
                string UpdateStatus = string.Empty;  // Added by Rajas on 24 MARCH 2017

                // Added by Rajas on 15 MAY 2017
                bool IsEdit = true;

                if (ModelState.IsValid)
                {
                    // Updated by Rajas on 15 MAY 2017
                    if (UpdateCompanyData(CompanyModelObj, IsEdit, ref UpdateStatus) == true)
                    {

                        // ADDED BY RAJAS FOR AuditLog ON 27 DEC 2016
                        AddAuditTrail("Success - Company : " + CompanyModelObj.CompanyName + " is edited successfully");

                        // Added by Rajas on 16 JAN 2017
                        Success("Company : " + CompanyModelObj.CompanyName + " is edited successfully");
                    }
                    else
                    {
                        return ReportError(CompanyModelObj, UpdateStatus);
                    }

                    return RedirectToAction("Index");
                }

                else
                {
                    PopulateDropdown();

                    // Added by Rajas on 16 JAN 2017 START
                    //AddAuditTrail("Error - Company : " + CompanyModelObj.CompanyName + " update failed");

                    //Error("Company: " + CompanyModelObj.CompanyName + " update failed");
                    // Added by Rajas on 16 JAN 2017 END

                    return View(CompanyModelObj);

                }
            }
            catch (System.Exception ex)
            {
                // Added by Rajas on 16 JAN 2017 START
                AddAuditTrail("Exception - " + CompanyModelObj.CompanyName + " update failed due to " + ex.Message);

                Error(WetosErrorMessageController.GetErrorMessage(1));
                // Added by Rajas on 16 JAN 2017 END

                PopulateDropdown();

                return View(CompanyModelObj);
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
                WetosDB.VwLocationNameInCompany CompanyDetails = WetosDB.VwLocationNameInCompanies.Single(b => b.CompanyId == id);

                //ADDED BY RAJAS ON 27 DEC 2016
                AddAuditTrail("Checked details for " + CompanyDetails.CompanyName); // Updated on 16 JAN 2017 by Rajas

                return View(CompanyDetails);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error(WetosErrorMessageController.GetErrorMessage(3));

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
                WetosDB.Company CompanyObj = WetosDB.Companies.Where(a => a.CompanyId == id).FirstOrDefault();

                List<Employee> EmployeeListObj = WetosDB.Employees.Where(a => a.CompanyId == id).ToList();

                if (EmployeeListObj.Count > 0)
                {
                    // Updated by Rajas on 15 MAY 2017 START
                    Information("You can not delete this company, as " + CompanyObj.CompanyName + " is assigned to employee");

                    AddAuditTrail("Can not delete company as " + CompanyObj.CompanyName + " is assigned to employee");
                    // Updated by Rajas on 15 MAY 2017 END

                    return RedirectToAction("Index");
                }

                if (CompanyObj != null)
                {
                    CompanyObj.MarkedAsDelete = 1;

                    WetosDB.SaveChanges();

                    Success("Company " + CompanyObj.CompanyName + " deleted successfully");

                    AddAuditTrail("Company " + CompanyObj.CompanyName + " deleted successfully");
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
        /// Common Error reporting function
        /// Added by Rajas on 3 JUNE 2017
        /// </summary>
        /// <param name="DepartmentObj"></param>
        /// <param name="ErrorMessage"></param>
        /// <returns></returns>
        public ActionResult ReportError(CompanyModel NewCompanyObj, string ErrorMessage)
        {
            PopulateDropdown();

            AddAuditTrail(ErrorMessage);
            Error(ErrorMessage);

            return View(NewCompanyObj);
        }

        /// <summary>
        /// Added by Rajas on 3 JUNE 2017
        /// </summary>
        public void PopulateDropdown()
        {
            // GET LOCATION AND FILL VIEWBAG FOR DROPDOWN
            // Updated by Rajas on 6 MAY 2017 to display data which is not MarkedAsDelete
            var Location = WetosDB.Locations.Where(a => a.MarkedAsDelete == null || a.MarkedAsDelete == 0).Select(a => new { LocationId = a.LocationId, LocationName = a.LocationName }).ToList();
            ViewBag.LocatioList = new SelectList(Location, "LocationId", "LocationName").ToList();

        }

        /// <summary>
        /// UpdateCompanyData
        /// ADDED BY MSJ ON 29 DEC 2016
        /// </summary>
        /// <param name="NewCompanyObj"></param>
        /// <returns></returns>
        private bool UpdateCompanyData(CompanyModel NewCompanyObj, bool IsEdit, ref string UpdateStatus)
        {
            bool ReturnStatus = false;

            try
            {
                // Added for saving the companymaster model to company table object, by Rajas on 29 DEC 2016 START
                // Updated by Rajas on 31 MAY 2017
                WetosDB.Company CompanyTblObj = WetosDB.Companies.Where(a => (a.CompanyName.Trim().ToUpper() == NewCompanyObj.CompanyName.Trim().ToUpper() || a.CompanyId == NewCompanyObj.CompanyId) 
                    && a.MarkedAsDelete == 0).FirstOrDefault();

                //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 9 SEPTEMBER 2017 FOR EDIT
                WetosDB.Company CompanyTblObjEDIT = WetosDB.Companies.Where(a => (a.CompanyName.Trim().ToUpper() == NewCompanyObj.CompanyName.Trim().ToUpper() || a.CompanyId == NewCompanyObj.CompanyId)
                    && a.MarkedAsDelete == 0).FirstOrDefault();

                // ADDED BY RAJAS ON 03 JUNE 2017 START
                bool IsNew = false;

                // IS OBJECT PRESET
                if (CompanyTblObj != null)  // Added by Rajas on 2 JUNE 2017
                {
                    if (IsEdit == false)  // CREATE            
                    {
                        UpdateStatus = WetosErrorMessageController.GetErrorMessage(4);

                        //AddAuditTrail("Error in Department update : " + UpdateStatus);
                        return ReturnStatus;
                    }
                }
                else
                {
                    if (IsEdit == false) // CREATE    
                    {
                        CompanyTblObj = new WetosDB.Company();
                        IsNew = true;
                    }
                    else // EDIT    
                    {
                        UpdateStatus = WetosErrorMessageController.GetErrorMessage(1);
                        //AddAuditTrail("Error in Department update : " + UpdateStatus);
                        return ReturnStatus;
                    }
                }
                // ADDED BY RAJAS ON 03 JUNE 2017 END

                // New Leave table object

                //ADDED NEW CODE TO RESOLVE Location NULL ISSUE BY SHRADDHA ON 01 NOV 2017 START
                if (CompanyTblObj.Location == null)
                {
                    CompanyTblObj.Location = WetosDB.Locations.Where(a => a.LocationId == NewCompanyObj.LocationId).FirstOrDefault();
                }
                //CompanyTblObj.Location.LocationId = NewCompanyObj.LocationId; // Updated by Rajas on 21 FEB 2017 replaced Location.LocationId with LocationId
                //ADDED NEW CODE TO RESOLVE Location NULL ISSUE BY SHRADDHA ON 01 NOV 2017 END
                
                CompanyTblObj.CompanyName = NewCompanyObj.CompanyName.Trim();
                CompanyTblObj.Address1 = NewCompanyObj.Address1;
                CompanyTblObj.Address2 = NewCompanyObj.Address2;
                CompanyTblObj.Telephone = NewCompanyObj.Telephone;
                CompanyTblObj.Fax = NewCompanyObj.Fax;
                CompanyTblObj.Email = NewCompanyObj.Email;
                CompanyTblObj.PAN = NewCompanyObj.PAN;

                //Added by shalaka on 16th DEC 2017 --- Start
                CompanyTblObj.PINCode = NewCompanyObj.PinCode;
                //Added by shalaka on 16th DEC 2017 --- End
                
                // Added by Rajas on 28 MARCH 2017
                CompanyTblObj.MarkedAsDelete = 0;  // Save default 0 as status is not deleted

                // Add new table object 
                if (IsNew)
                {
                    WetosDB.Companies.AddObject(CompanyTblObj);
                }

                // Added for saving the company master model to company table object, by Rajas on 29 DEC 2016 END

                WetosDB.SaveChanges();


                //-------------------------------------------AuditLog---------------------------------------------------------------------------

                if (IsNew)
                {
                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 11 SEPTEMBER 2017
                    #region ADD AUDIT LOG
                    //OLD RECORD IS BLANK
                    string Newrecord = "Company Name :" + CompanyTblObj.CompanyName + ", Location ID :" + CompanyTblObj.Location.LocationId
                        + ", Address1 :" + CompanyTblObj.Address1 + ", Address2 :" + CompanyTblObj.Address2
                        + ", Telephone :" + CompanyTblObj.Telephone + ", Fax :" + CompanyTblObj.Fax + ", Email :" + CompanyTblObj.Email
                        + ", PAN :" + CompanyTblObj.PAN;

                    //LOGIN ID IS TAKEN FROM SESSION PERSISTER
                    string Formname = "COMPANY MASTER";
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
                    string Oldrecord = "Company Name :" + CompanyTblObjEDIT.CompanyName + ", Location ID :" + CompanyTblObjEDIT.Location.LocationId
                        + ", Address1 :" + CompanyTblObjEDIT.Address1 + ", Address2 :" + CompanyTblObjEDIT.Address2
                        + ", Telephone :" + CompanyTblObjEDIT.Telephone + ", Fax :" + CompanyTblObjEDIT.Fax + ", Email :" + CompanyTblObjEDIT.Email
                        + ", PAN :" + CompanyTblObjEDIT.PAN;
                    //---x---------x-----------------x------------x----------------x-----------x-------------x------------x--------------x----------x---------x-----
                    string Newrecord = "Company Name :" + CompanyTblObj.CompanyName + ", Location ID :" + CompanyTblObj.Location.LocationId
                        + ", Address1 :" + CompanyTblObj.Address1 + ", Address2 :" + CompanyTblObj.Address2
                        + ", Telephone :" + CompanyTblObj.Telephone + ", Fax :" + CompanyTblObj.Fax + ", Email :" + CompanyTblObj.Email
                        + ", PAN :" + CompanyTblObj.PAN;

                    //LOGIN ID IS TAKEN FROM SESSION PERSISTER
                    string Formname = "COMPANY MASTER";
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
    }
}
