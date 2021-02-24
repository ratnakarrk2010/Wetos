using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WetosDB;
using WetosMVCMainApp.Models;

namespace WetosMVC.Controllers
{
    [SessionExpire] 
    [Authorize]
    public class WetosRoleController : BaseController
    {
        //
        // GET: /WetosRole/
        

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            try
            {
                // Updated by Rajas on 29 MARCH 2017
                List<WetosDB.RoleDef> RoleList = WetosDB.RoleDefs.Where(a => a.MarkedAsDelete == 0 || a.MarkedAsDelete == null).ToList();

                //ADDED BY RAJAS ON 27 DEC 2016
                AddAuditTrail("Success - Visited Role master"); // Updated by Rajas on 16 JAN 2017

                return View(RoleList);
            }

            catch (System.Exception ex)
            {
                // Added by Rajas on 16 JAN 2017
                AddAuditTrail("Exception - " + ex.Message);

                Error("Inconsistent data. Please try again!");

                return View();
            }
        }

        public ActionResult Create()
        {

            return View();
        }

        //
        // POST: /WetosCompany/Create


        /// <summary>
        /// Posting with the help of companymodel, for validation of data
        /// Added By Rajas on 29 DEC 2016
        /// </summary>
        /// <param name="NewCompanyObj"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(RoleDefModel NewRoleDefObj, FormCollection collection)
        {
            try
            {
                // Added by Rajas on 27 MARCH 2017
                string UpdateStatus = string.Empty;

                // Added by Rajas on 15 MAY 2017
                bool IsEdit = false;

                if (ModelState.IsValid)
                {
                    if (UpdateRoleData(NewRoleDefObj, IsEdit, ref UpdateStatus) == true)
                    {
                        // ADDED BY RAJAS FOR AuditLog ON 27 DEC 2016
                        AddAuditTrail("Success - Role : " + NewRoleDefObj.RoleName + " is added"); // Updated by Rajas on 16 JAN 2017


                        //ADDED CODE FOR SUCCESS MESSAGE BY SHRADDHA ON 16 JAN 2017
                        Success("Role : " + NewRoleDefObj.RoleName + " is added Successfully"); // Updated by Rajas on 16 JAN 2017
                    }
                    else
                    {
                        AddAuditTrail(UpdateStatus);

                        Error(UpdateStatus);

                        return View(NewRoleDefObj);
                    }

                    return RedirectToAction("List");
                }

                else
                {
                    // Added by Rajas on 16 JAN 2017 START
                    //AddAuditTrail("Error - Role : " + NewRoleDefObj.RoleName + " create failed");

                    // Error("Error - Add New Role failed");
                    // Added by Rajas on 16 JAN 2017 END

                    return View(NewRoleDefObj);
                }

            }
            catch (System.Exception ex)
            {
                // Added by Rajas on 16 JAN 2017 START
                AddAuditTrail("Exception - Role : " + NewRoleDefObj.RoleName + " create failed due to " + ex.Message);

                Error("Error - " + ex.Message);
                // Added by Rajas on 16 JAN 2017 END

                return View(NewRoleDefObj);
            }
        }

        /// <summary>
        /// Updated by Rajas on 27 MARCH 2017
        /// </summary>
        /// <param name="NewRoleObj"></param>
        /// <returns></returns>
        private bool UpdateRoleData(RoleDefModel NewRoleObj, bool IsEdit, ref string UpdateStatus)
        {
            bool ReturnStatus = false;

            try
            {

                // Added for saving the companymaster model to company table object, by Rajas on 29 DEC 2016 START
                // Updated by Rajas on 31 MAY 2017
                RoleDef RoleDefTblObj = WetosDB.RoleDefs.Where(a => (a.RoleName.Trim().ToUpper() == NewRoleObj.RoleName.Trim().ToUpper() || a.RoleId == NewRoleObj.RoleId) 
                    && a.MarkedAsDelete == 0).FirstOrDefault();

                // ADDED BY RAJAS ON 03 JUNE 2017 START
                bool IsNew = false;

                // IS OBJECT PRESET
                if (RoleDefTblObj != null)  // Added by Rajas on 2 JUNE 2017
                {
                    if (IsEdit == false)  // CREATE            
                    {
                        UpdateStatus = "Role already available."; //WetosEmployeeController.GetErrorMessage(1);

                        //AddAuditTrail("Error in Department update : " + UpdateStatus);
                        return ReturnStatus;
                    }
                }
                else
                {
                    if (IsEdit == false) // CREATE    
                    {
                        RoleDefTblObj = new WetosDB.RoleDef();
                        IsNew = true;
                    }
                    else // EDIT    
                    {
                        UpdateStatus = "Error in updating Role."; // WetosEmployeeController.GetErrorMessage(1); 
                        //AddAuditTrail("Error in Department update : " + UpdateStatus);
                        return ReturnStatus;
                    }
                }
                // ADDED BY RAJAS ON 03 JUNE 2017 END


                // New Leave table object

                RoleDefTblObj.RoleDescription = NewRoleObj.RoleDescription;
                RoleDefTblObj.RoleName = NewRoleObj.RoleName;

                // Added by rajas on 29 MARCH 2017
                RoleDefTblObj.MarkedAsDelete = 0;  // Default value

                // Add new table object 
                if (IsNew)
                {
                    WetosDB.RoleDefs.AddObject(RoleDefTblObj);
                }

                // Added for saving the company master model to company table object, by Rajas on 29 DEC 2016 END

                WetosDB.SaveChanges();

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
        /// Updated by Rajas on 27 MARCH 2017
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            try
            {
                WetosDB.RoleDef RoleDefEdit = new WetosDB.RoleDef();
                RoleDefEdit = WetosDB.RoleDefs.Single(b => b.RoleId == id);

                RoleDefModel NewRoleObj = new RoleDefModel();

                // Addded by Rajas on 30 DEC 2016 for Saving data from Table object to Model object 
                NewRoleObj.RoleId = RoleDefEdit.RoleId;
                NewRoleObj.RoleName = RoleDefEdit.RoleName;
                NewRoleObj.RoleDescription = RoleDefEdit.RoleDescription;

                // Addded by rajas on 29 MARCH 2017
                NewRoleObj.MarkedAsDelete = RoleDefEdit.MarkedAsDelete.Value;

                return View(NewRoleObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Error due to inconsistent data. Please try again!");

                return RedirectToAction("List"); // Verify ?

            }
        }


        /// <summary>
        /// Updated by Rajas on 27 MARCH 2017
        /// </summary>
        /// <param name="id"></param>
        /// <param name="RoleDefObj"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(int id, RoleDefModel NewRoleObj)
        {
            try
            {
                // Added by Rajas on 27 MARCH 2017
                string UpdateStatus = string.Empty;

                // Added by Rajas on 15 MAY 2017
                bool IsEdit = true;

                if (ModelState.IsValid)
                {
                    if (UpdateRoleData(NewRoleObj, IsEdit, ref UpdateStatus) == true)
                    {
                        // ADDED BY RAJAS FOR AuditLog ON 27 DEC 2016
                        AddAuditTrail("Success - Role : " + NewRoleObj.RoleName + " is edited successfully");

                        // Added by Rajas on 16 JAN 2017
                        Success("Role : " + NewRoleObj.RoleName + " is edited successfully");
                    }
                    else
                    {
                        AddAuditTrail(UpdateStatus);

                        Error(UpdateStatus);
                    }

                    return RedirectToAction("List");
                }

                else
                {
                    // Added by Rajas on 16 JAN 2017 START
                    //AddAuditTrail("Error - Role : " + NewRoleObj.RoleName + " update failed");

                    //Error("Role: " + NewRoleObj.RoleName + " update failed");
                    // Added by Rajas on 16 JAN 2017 END

                    return View(NewRoleObj);

                }
            }
            catch (System.Exception ex)
            {
                // Added by Rajas on 16 JAN 2017 START
                AddAuditTrail("Exception - " + NewRoleObj.RoleName + " update failed due to " + ex.Message);

                Error("Role : " + NewRoleObj.RoleName + " update failed");
                // Added by Rajas on 16 JAN 2017 END

                return View(NewRoleObj);
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
                // Updated by Rajas on 29 MARCH 2017
                RoleDef RoleDefDetails = WetosDB.RoleDefs.Where(a => a.MarkedAsDelete == 0 || a.MarkedAsDelete == null).Single(b => b.RoleId == id);

                //ADDED BY RAJAS ON 27 DEC 2016
                AddAuditTrail("Checked details for " + RoleDefDetails.RoleName); // Updated on 16 JAN 2017 by Rajas

                return View(RoleDefDetails);
            }

            catch (System.Exception ex)
            {
                // Added by Rajas on 16 JAN 2017
                AddAuditTrail("Exception - " + ex.Message);

                Error("Please try again!");

                return RedirectToAction("List");
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
                // Updated by Rajas on 29 MARCH 2017
                RoleDef RoleDefDetails = WetosDB.RoleDefs.Where(a => a.MarkedAsDelete == 0 || a.MarkedAsDelete == null).Single(b => b.RoleId == id);

                //ADDED BY RAJAS ON 27 DEC 2016
                AddAuditTrail("Checked details for " + RoleDefDetails.RoleName); // Updated on 16 JAN 2017 by Rajas

                return View(RoleDefDetails);
            }

            catch (System.Exception ex)
            {
                // Added by Rajas on 16 JAN 2017
                AddAuditTrail("Exception - " + ex.Message);

                Error("Please try again!");

                return RedirectToAction("List");
            }
        }

        /// <summary>
        /// Added by Rajas on 29 MARCH 2017
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(int id, FormCollection fc)
        {
            try
            {
                RoleDef RoleDefObj = WetosDB.RoleDefs.Where(a => a.RoleId == id).FirstOrDefault();

                List<User> UserListObj = WetosDB.Users.Where(a => a.RoleTypeId == id).ToList();

                if (UserListObj.Count > 0)
                {
                    Information("You can not delete this role, as " + RoleDefObj.RoleName + " is assigned to employee");

                    AddAuditTrail("Can not delete this branch, as " + RoleDefObj.RoleName + " is assigned to employee");

                    return RedirectToAction("List");
                }

                if (RoleDefObj != null)
                {
                    RoleDefObj.MarkedAsDelete = 1;

                    WetosDB.SaveChanges();

                    Success("Role : " + RoleDefObj.RoleName + " deleted successfully");

                    AddAuditTrail("Role : " + RoleDefObj.RoleName + " deleted successfully");
                }

                return RedirectToAction("List");
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Please try again!");

                return RedirectToAction("List");
            }
        }
    }
}
