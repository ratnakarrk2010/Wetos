using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WetosDB;
using WetosMVCMainApp.Models;

namespace WetosMVC.Controllers
{
    [SessionExpire] 
    [Authorize]
    public class WetosEmployeeTypeController : BaseController
    {
        // 
        //
        // GET: /WetosEmployeeType/
        /// <summary>
        /// Added by Mousami on 02 SEP 2016 start
        /// </summary>
        /// <returns></returns>
        /// Updted by Rajas on 27 MARCH 2017
        public ActionResult Index()
        {
            try
            {
                // Updated by Rajas on 29 MARCh 2017
                List<EmployeeType> EmployeeTypeList = WetosDB.EmployeeTypes.Where(a => a.MarkedAsDelete == 0 || a.MarkedAsDelete == null).ToList();

                //ADDED BY RAJAS ON 27 DEC 2016
                AddAuditTrail("Success - Visited Employee type master"); // Updated by Rajas on 16 JAN 2017

                return View(EmployeeTypeList);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Inconsistent data. Please try again and verify!");

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
                // Updated by Rajas on 29 MARCH 2017
                EmployeeType EmployeeTypeDetails = WetosDB.EmployeeTypes.Where(a => a.MarkedAsDelete == 0 || a.MarkedAsDelete == null).Single(b => b.EmployeeTypeId == id);

                //ADDED BY RAJAS ON 27 DEC 2016
                AddAuditTrail("Checked details for " + EmployeeTypeDetails.EmployeeTypeName); // Updated on 16 JAN 2017 by Rajas

                return View(EmployeeTypeDetails);
            }

            catch (System.Exception ex)
            {
                // Added by Rajas on 16 JAN 2017
                AddAuditTrail("Exception - " + ex.Message);

                Error("Please try again!");

                return RedirectToAction("Index");
            }
        }

        //
        // GET: /WetosEmployeeType/Create

        public ActionResult Create()
        {

            return View();
        }

        //
        // POST: /WetosEmployeeType/Create


        /// <summary>
        /// POST for validate employee and create
        /// Added by Rajas on 30 DEC 2016
        /// </summary>
        /// <param name="EmployeeTypeObj"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        /// Updated by Rajas on 3 JUNE 2017
        [HttpPost]
        public ActionResult Create(EmployeeTypeModel EmployeeTypeObj, FormCollection collection)
        {
            try
            {
                // Added by Rajas on 27 MARCH 2017
                string UpdatedStatus = string.Empty;

                // Added by Rajas on 15 MAY 2017 
                bool IsEdit = false;

                if (ModelState.IsValid)
                {
                    // Updated by Rajas on 15 MAY 2017
                    if (UpdateEmployeeTypeData(EmployeeTypeObj, IsEdit, ref UpdatedStatus) == true)
                    {

                        // ADDED BY RAJAS FOR AuditLog ON 27 DEC 2016
                        AddAuditTrail("Success - Employee Type : " + EmployeeTypeObj.EmployeeTypeName + " is added successfully"); // Updated by Rajas on 16 JAN 2017


                        //ADDED CODE FOR SUCCESS MESSAGE BY RAJAS ON 16 JAN 2017
                        Success("Success - Employee Type : " + EmployeeTypeObj.EmployeeTypeName + " is added successfully"); // Updated by Rajas on 16 JAN 2017
                    }
                    else
                    {
                        return ReportError(EmployeeTypeObj, UpdatedStatus);

                    }

                    //return RedirectToAction("Create");

                    // Above line commented and below line added by Rajas on 16 JAN 2017 for redirection to index page 
                    return RedirectToAction("Index");
                }

                else
                {
                    // Added by Rajas on 16 JAN 2017 START
                    //AddAuditTrail("Error - Employee Type create failed");  // Updated by Rajas on 30 MAY 2017

                    //Error("Error - Employee type create failed");
                    // Added by Rajas on 16 JAN 2017 END

                    return View(EmployeeTypeObj);
                }
            }
            catch (System.Exception ex)
            {
                // Added by Rajas on 16 JAN 2017 START
                AddAuditTrail("Error - Employee Type create failed due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );  // Updated by Rajas on 30 MAY 2017

                Error("Error - Employee type create failed");
                // Added by Rajas on 16 JAN 2017 END

                return View(EmployeeTypeObj);
            }
        }


        /// <summary>
        /// Updated by Rajas on 29 MARCH 2017
        /// GET
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            try
            {

                //EmployeeType EmployeeTypeEdit = new EmployeeType();
                EmployeeType EmployeeTypeEdit = WetosDB.EmployeeTypes.Where(b => b.EmployeeTypeId == id).FirstOrDefault();

                EmployeeTypeModel EmployeeTypeObj = new EmployeeTypeModel();

                EmployeeTypeObj.EmployeeTypeId = EmployeeTypeEdit.EmployeeTypeId;

                EmployeeTypeObj.EmployeeTypeName = EmployeeTypeEdit.EmployeeTypeName;

                // Added by Rajas on 29 MARCH 2017
                EmployeeTypeObj.MarkedAsDelete = EmployeeTypeEdit.MarkedAsDelete;

                return View(EmployeeTypeObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Inconsistent data. Please try again!");

                return RedirectToAction("Index");  // Verify ?
            }
        }

        /// <summary>
        /// UPDATED BY RAJAS ON 3 JUNE 2017
        /// </summary>
        /// <param name="id"></param>
        /// <param name="EmployeeTypeObj"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(int id, EmployeeTypeModel EmployeeTypeObj)
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
                    if (UpdateEmployeeTypeData(EmployeeTypeObj, IsEdit, ref UpdateStatus) == true)
                    {

                        // ADDED BY RAJAS FOR AuditLog ON 27 DEC 2016
                        AddAuditTrail("Success - Employee Type : " + EmployeeTypeObj.EmployeeTypeName + " is updated successfully"); // Updated by Rajas on 16 JAN 2017


                        //ADDED CODE FOR SUCCESS MESSAGE BY RAJAS ON 16 JAN 2017
                        Success("Success - Employee Type : " + EmployeeTypeObj.EmployeeTypeName + " is updated successfully"); // Updated by Rajas on 16 JAN 2017
                    }
                    else
                    {
                        return ReportError(EmployeeTypeObj, UpdateStatus);
                    }

                }
                else
                {
                    // Added by Rajas on 16 JAN 2017 START
                    //AddAuditTrail("Error - Employee Type : " + EmployeeTypeObj.EmployeeTypeName + " update failed");

                    //Error("Error - Employee Type : " + EmployeeTypeObj.EmployeeTypeName + " update failed");
                    // Added by Rajas on 16 JAN 2017 END

                    return View(EmployeeTypeObj);
                }

                return RedirectToAction("Index");
            }
            catch (System.Exception ex)
            {
                // Added by Rajas on 16 JAN 2017 START
                AddAuditTrail("Error - Employee Type : " + EmployeeTypeObj.EmployeeTypeName + " update failed due to " + ex.Message);

                Error("Error in updating Employee type");
                // Added by Rajas on 16 JAN 2017 END

                return View(EmployeeTypeObj);
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
                EmployeeType EmployeeTypeDetails = WetosDB.EmployeeTypes.Where(a => a.MarkedAsDelete == 0 || a.MarkedAsDelete == null).Single(b => b.EmployeeTypeId == id);

                //ADDED BY RAJAS ON 27 DEC 2016
                AddAuditTrail("Checked details for " + EmployeeTypeDetails.EmployeeTypeName); // Updated on 16 JAN 2017 by Rajas

                return View(EmployeeTypeDetails);
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
                EmployeeType EmployeeTypeObj = WetosDB.EmployeeTypes.Where(a => a.EmployeeTypeId == id).FirstOrDefault();

                List<Employee> EmployeeListObj = WetosDB.Employees.Where(a => a.EmployeeTypeId == id).ToList();

                if (EmployeeListObj.Count > 0)
                {
                    // Updated by Rajas on 15 MAY 2017 START
                    Information("You can not delete this Employee type, as " + EmployeeTypeObj.EmployeeTypeName + " is assigned to employee");

                    AddAuditTrail("Can not delete Employee type as " + EmployeeTypeObj.EmployeeTypeName + " is used in employee");
                    // Updated by Rajas on 15 MAY 2017 START

                    return RedirectToAction("Index");
                }

                if (EmployeeTypeObj != null)
                {
                    EmployeeTypeObj.MarkedAsDelete = 1;

                    WetosDB.SaveChanges();

                    Success("Employee Type : " + EmployeeTypeObj.EmployeeTypeName + " deleted successfully");

                    AddAuditTrail("Employee Type : " + EmployeeTypeObj.EmployeeTypeName + " deleted successfully");
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
        public ActionResult ReportError(EmployeeTypeModel EmployeeTypeObj, string ErrorMessage)
        {
            AddAuditTrail(ErrorMessage);
            Error(ErrorMessage);

            return View(EmployeeTypeObj);
        }

        /// <summary>
        /// Common function to validate the data for edit or create
        /// Added by Rajas on 30 DEC 2016
        /// </summary>
        /// Updated by Rajas on 27 MARCH 2017
        private bool UpdateEmployeeTypeData(EmployeeTypeModel EmployeeTypeObj, bool IsEdit, ref string UpdateStatus)
        {
            bool ReturnStatus = false;

            try
            {
                // Updated by Rajas on 31 MAY 2017
                WetosDB.EmployeeType EmployeeTypeTblObj = WetosDB.EmployeeTypes.Where(a => (a.EmployeeTypeName.Trim().ToUpper() == EmployeeTypeObj.EmployeeTypeName.Trim().ToUpper() || a.EmployeeTypeId == EmployeeTypeObj.EmployeeTypeId)
                    && a.MarkedAsDelete == 0).FirstOrDefault();


                //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 9 SEPTEMBER 2017 FOR EDIT
                WetosDB.EmployeeType EmployeeTypeTblObjEDIT = WetosDB.EmployeeTypes.Where(a => (a.EmployeeTypeName.Trim().ToUpper() == EmployeeTypeObj.EmployeeTypeName.Trim().ToUpper() || a.EmployeeTypeId == EmployeeTypeObj.EmployeeTypeId)
                   && a.MarkedAsDelete == 0).FirstOrDefault();

                // ADDED BY RAJAS ON 03 JUNE 2017 START
                bool IsNew = false;

                // IS OBJECT PRESET
                if (EmployeeTypeTblObj != null)  // Added by Rajas on 2 JUNE 2017
                {
                    if (IsEdit == false)  // CREATE            
                    {
                        UpdateStatus = "Employee Type already available."; //WetosEmployeeController.GetErrorMessage(1);

                        //AddAuditTrail("Error in Department update : " + UpdateStatus);
                        return ReturnStatus;
                    }
                }
                else
                {
                    if (IsEdit == false) // CREATE    
                    {
                        EmployeeTypeTblObj = new WetosDB.EmployeeType();
                        IsNew = true;
                    }
                    else // EDIT    
                    {
                        UpdateStatus = "Error in updating Employee type."; // WetosEmployeeController.GetErrorMessage(1); 
                        //AddAuditTrail("Error in Department update : " + UpdateStatus);
                        return ReturnStatus;
                    }
                }
                // ADDED BY RAJAS ON 03 JUNE 2017 END


                // New Leave table object

                EmployeeTypeTblObj.EmployeeTypeName = EmployeeTypeObj.EmployeeTypeName.Trim();

                // Added by Rajas on 29 MARCH 2017
                EmployeeTypeTblObj.MarkedAsDelete = 0; // Default value

                // Add new table object 
                if (IsNew)
                {
                    WetosDB.EmployeeTypes.Add(EmployeeTypeTblObj);
                }

                WetosDB.SaveChanges();


                //-------------------------------------------AuditLog---------------------------------------------------------------------------

                if (IsNew)
                {
                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 11 SEPTEMBER 2017
                    #region ADD AUDIT LOG
                    //OLD RECORD IS BLANK
                    string Newrecord = "EmployeeTypeName :"+EmployeeTypeTblObj.EmployeeTypeName;

                    //LOGIN ID IS TAKEN FROM SESSION PERSISTER
                    string Formname = "EmployeeType MASTER";
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
                    string Oldrecord = "EmployeeTypeName :" + EmployeeTypeTblObjEDIT.EmployeeTypeName;
                    //---x---------x-----------------x------------x----------------x-----------x-------------x------------x--------------x----------x---------x-----
                    string Newrecord = "EmployeeTypeName :" + EmployeeTypeTblObj.EmployeeTypeName;

                    //LOGIN ID IS TAKEN FROM SESSION PERSISTER
                    string Formname = "EmployeeType MASTER";
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
