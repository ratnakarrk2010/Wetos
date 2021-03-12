using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WetosDB;
using WetosMVCMainApp.Models;

namespace WetosMVC.Controllers
{
    [SessionExpire] 
    [Authorize]
    public class WetosLocationController : BaseController
    {
        /// <summary>
        /// Added by Shraddha on 2 SEPT 2016 start
        /// </summary>
        
        //
        // GET: /WetosLocation/

        public ActionResult Index()
        {
            try
            {
                // Updated by Rajas on 28 MARCH 2017
                List<Location> LocationListObj = WetosDB.Locations.Where(a => a.MarkedAsDelete == 0 || a.MarkedAsDelete == null).ToList();

                //ADDED BY RAJAS ON 27 DEC 2016
                AddAuditTrail("Success : Visited location details");

                return View(LocationListObj);
            }

            catch (System.Exception ex)
            {
                // Added by Rajas on 18 FEB 2017
                AddAuditTrail("Exception - " + ex.Message);

                Error("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                return View("Error");
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
                Location LocationDetails = WetosDB.Locations.Single(b => b.LocationId == id);

                //ADDED BY RAJAS ON 27 DEC 2016
                AddAuditTrail("Success : Location details viewed");

                return View(LocationDetails);
            }
            catch (System.Exception ex)
            {
                // Added by Rajas on 18 FEB 2017
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

                return View();
            }
            catch
            {
                return View("Error");
            }
        }

        /// <summary>
        /// Updated by Rajas on 27 MARCH 2017
        /// </summary>
        /// <param name="LocationObj"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(LocationModel LocationObj, FormCollection collection)
        {
            try
            {
                // Added by Rajas on 27 MARCH 2017
                string UpdateStatus = string.Empty;

                // Added by Rajas on 15 MAY 2017
                bool IsEdit = false;

                if (ModelState.IsValid) // Added by Rajas on 29 DEC 2016 for validation
                {
                    // Updated by Rajas on 15 MAY 2017
                    if (UpdateLocationData(LocationObj, IsEdit, ref UpdateStatus) == true)
                    {
                        // ADDED BY RAJAS FOR AuditLog ON 18 FEB 2017
                        AddAuditTrail("New location added");

                        // Added by Rajas on 18 FEB 2017
                        Success("Success - New location added successfully.");
                    }
                    else
                    {
                        return ReportError(LocationObj, UpdateStatus);
                    }

                    return RedirectToAction("Index");
                }

                else
                {
                    // Added by Rajas on 16 JAN 2017 START
                    // AddAuditTrail("Error - Location : " + LocationObj.LocationName + " create failed");

                    // Error("Error - Add New location failed");
                    // Added by Rajas on 16 JAN 2017 END

                    return View(LocationObj);
                }

            }

            catch (System.Exception ex)
            {
                // Added by Rajas on 18 FEB 2017 START
                AddAuditTrail("Exception - Location : " + LocationObj.LocationName + " create failed due to " + ex.Message);

                Error("Error - Add New location failed");
                // Added by Rajas on 18 FEB 2017 END

                return View(LocationObj);
            }

        }


        /// <summary>
        /// Updated by Rajas on 27 MARCH 2017 for try catch
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            try
            {
                Location LocationEdit = new Location();
                LocationEdit = WetosDB.Locations.Single(b => b.LocationId == id);

                LocationModel LocationObj = new LocationModel();

                LocationObj.LocationId = LocationEdit.LocationId;

                LocationObj.LocationName = LocationEdit.LocationName;

                LocationObj.Address = LocationEdit.Address;

                LocationObj.City = LocationEdit.City;

                // Added by Rajas on 28 MARCH 2017
                LocationObj.MarkedAsDelete = LocationEdit.MarkedAsDelete;  // Mark as delete

                return View(LocationObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Location edit failed");

                return View();
            }
        }

        /// <summary>
        /// Updated by Rajas on 27 MARCH 2017
        /// </summary>
        /// <param name="id"></param>
        /// <param name="LocationObj"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(int id, LocationModel LocationObj)
        {
            try
            {
                string UpdateStatus = string.Empty; // Added by Rajas on 27 MARCH 2017

                // Added by rajas on 15 MAY 2017
                bool IsEdit = true;

                if (ModelState.IsValid)
                {
                    // Updated by Rajas on 15 MAY 2017
                    if (UpdateLocationData(LocationObj, IsEdit, ref UpdateStatus) == true)
                    {
                        // ADDED BY RAJAS FOR AuditLog ON 18 FEB 2017
                        AddAuditTrail("Location details updated");

                        // Added by Rajas on 18 FEB 2017
                        Success("Success - Location : " + LocationObj.LocationName + " updated successfully.");
                    }
                    else
                    {
                        return ReportError(LocationObj, UpdateStatus);
                    }
                }

                else
                {
                    // ADDED BY RAJAS FOR AuditLog ON 18 FEB 2017
                    //AddAuditTrail("Location details not updated.");

                    // Added by Rajas on 18 FEB 2017
                    //Error("Error - Location updation failed.");

                    return View(LocationObj);
                }

                return RedirectToAction("Index");
            }
            catch (System.Exception ex)
            {
                // ADDED BY RAJAS FOR AuditLog ON 18 FEB 2017
                AddAuditTrail("Location details not update due to " + ex.Message);

                // Added by Rajas on 18 FEB 2017
                Error("Error - Location updation failed.");

                return View(LocationObj);
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
                Location LocationDetails = WetosDB.Locations.Single(b => b.LocationId == id);

                //ADDED BY RAJAS ON 27 DEC 2016
                AddAuditTrail("Success : Location details viewed");

                return View(LocationDetails);
            }
            catch (System.Exception ex)
            {
                // Added by Rajas on 18 FEB 2017
                AddAuditTrail("Exception - " + ex.Message);

                Error("Please try again!");

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
                Location LocationObj = WetosDB.Locations.Where(a => a.LocationId == id).FirstOrDefault();

                List<WetosDB.Company> CompanyListObj = WetosDB.Companies.Where(a => a.Location.LocationId == id).ToList();

                if (CompanyListObj.Count > 0)
                {
                    Information("You can not delete this location, as it is assigned to company");

                    AddAuditTrail("Can not delete location as it is used in company");

                    return RedirectToAction("Index");
                }

                if (LocationObj != null)
                {
                    LocationObj.MarkedAsDelete = 1;

                    WetosDB.SaveChanges();

                    Success("Location " + LocationObj.LocationName + " deleted successfully");

                    AddAuditTrail("Location " + LocationObj.LocationName + " deleted successfully");
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
        public ActionResult ReportError(LocationModel LocationObj, string ErrorMessage)
        {
            AddAuditTrail(ErrorMessage);
            Error(ErrorMessage);

            return View(LocationObj);
        }

        /// <summary>
        /// Common function to validate the data for edit or create
        /// Added by Rajas on 30 DEC 2016
        /// Updated by Rajas on 27 MARCH 2017
        /// </summary>
        private bool UpdateLocationData(LocationModel LocationObj, bool IsEdit, ref string UpdateStatus)
        {
            bool ReturnStatus = false;

            try
            {
                WetosDB.Location LocationTblObj = WetosDB.Locations.Where(a => (a.LocationName == LocationObj.LocationName || a.LocationId == LocationObj.LocationId) 
                    && a.MarkedAsDelete == 0).FirstOrDefault();

                //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 11 SEPTEMBER 2017 FOR EDIT
                WetosDB.Location LocationTblObjEDIT = WetosDB.Locations.Where(a => (a.LocationName == LocationObj.LocationName || a.LocationId == LocationObj.LocationId)
                    && a.MarkedAsDelete == 0).FirstOrDefault();

                // ADDED BY RAJAS ON 03 JUNE 2017 START
                bool IsNew = false;

                // IS OBJECT PRESET
                if (LocationTblObj != null)  // Added by Rajas on 2 JUNE 2017
                {
                    if (IsEdit == false)  // CREATE            
                    {
                        UpdateStatus = "Location already available."; //WetosEmployeeController.GetErrorMessage(1);

                        //AddAuditTrail("Error in Department update : " + UpdateStatus);
                        return ReturnStatus;
                    }
                }
                else
                {
                    if (IsEdit == false) // CREATE    
                    {
                        LocationTblObj = new WetosDB.Location();
                        IsNew = true;
                    }
                    else // EDIT    
                    {
                        UpdateStatus = "Error in updating Location."; // WetosEmployeeController.GetErrorMessage(1); 
                        //AddAuditTrail("Error in Department update : " + UpdateStatus);
                        return ReturnStatus;
                    }
                }
                // ADDED BY RAJAS ON 03 JUNE 2017 END

                // New Leave table object
                LocationTblObj.LocationName = LocationObj.LocationName.Trim();

                LocationTblObj.Address = LocationObj.Address;

                LocationTblObj.City = LocationObj.City;

                // Added by Rajas on 28 MARCH 2017
                LocationTblObj.MarkedAsDelete = 0;  // Save default 0 as status is not deleted


                // Add new table object 
                if (IsNew)
                {
                    WetosDB.Locations.Add(LocationTblObj);
                }

                WetosDB.SaveChanges();

                //-------------------------------------------AuditLog---------------------------------------------------------------------------

                if (IsNew)
                {
                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 11 SEPTEMBER 2017
                    #region ADD AUDIT LOG
                    //OLD RECORD IS BLANK
                    string Newrecord = "Location Name :" + LocationTblObj.LocationName + ", Address :" + LocationTblObj.Address +
                        ", City :" + LocationTblObj.City;

                    //LOGIN ID IS TAKEN FROM SESSION PERSISTER
                    string Formname = "LOCATION MASTER";
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
                    string Oldrecord = "Location Name :" + LocationTblObjEDIT.LocationName + ", Address :" + LocationTblObjEDIT.Address +
                       ", City :" + LocationTblObjEDIT.City;
                    //---x---------x-----------------x------------x----------------x-----------x-------------x------------x--------------x----------x---------x-----
                    string Newrecord = "Location Name :" + LocationTblObj.LocationName + ", Address :" + LocationTblObj.Address +
                       ", City :" + LocationTblObj.City;

                    //LOGIN ID IS TAKEN FROM SESSION PERSISTER
                    string Formname = "LOCATION MASTER";
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
