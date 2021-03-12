using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WetosDB;
using WetosMVCMainApp.Models;

namespace WetosMVC.Controllers
{
    [SessionExpire]  // Added by Rajas on 7 JUNE 2017 2017
    [Authorize]
    public class WetosGlobalSettingController : BaseController
    {
        /// <summary>
        /// Index
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            List<GlobalSetting> GlobalSettingObj = new List<GlobalSetting>();

            try
            {
                GlobalSettingObj = WetosDB.GlobalSettings.ToList();

                AddAuditTrail("Global setting viewed");

                return View(GlobalSettingObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in Global setting Index due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                return View(GlobalSettingObj);
            }

        }

        /// <summary>
        /// Details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int id)
        {
            try
            {
                GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Single(a => a.SettingId == id);

                AddAuditTrail("Global setting viewed");

                return View(GlobalSettingObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in Global setting Index due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Error in viewing global setting details");

                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Create GET
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            GlobalSettingModel GlobalSettingModelObj = new GlobalSettingModel();

            try
            {
                return View(GlobalSettingModelObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in Global setting create due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                return View(GlobalSettingModelObj);
            }
        }

        /// <summary>
        /// POST
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(GlobalSettingModel GlobalSettingModelObj, FormCollection collection)
        {
            try
            {
                string UpdateStatus = string.Empty;

                bool IsEdit = false;

                if (ModelState.IsValid)  // Added by Rajas on 29 DEC 2016 for validation
                {
                    // Updated by Rajas on 15 MAY 2017
                    if (UpdateGlobalSettingData(GlobalSettingModelObj, IsEdit, ref UpdateStatus) == true)
                    {
                        // ADDED BY RAJAS FOR AuditLog ON 27 DEC 2016
                        AddAuditTrail("Success - Setting : " + GlobalSettingModelObj.SettingText + " is added successfully");


                        //ADDED CODE FOR SUCCESS MESSAGE BY RAJAS ON 16 JAN 2017
                        Success("Setting : " + GlobalSettingModelObj.SettingText + " is added successfully");
                    }
                    else
                    {
                        return ReportError(GlobalSettingModelObj, UpdateStatus);
                    }

                }
                else
                {
                    return View(GlobalSettingModelObj);
                }

                return RedirectToAction("Index");

            }
            catch (System.Exception ex)
            {
                // Added by Rajas on 16 JAN 2017 START
                AddAuditTrail("Exception - Global setting update failed due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Error in adding new Gloabl Setting");
                // Added by Rajas on 16 JAN 2017 END

                return View(GlobalSettingModelObj);
            }
        }

        /// <summary>
        /// Common Error reporting function
        /// Added by Rajas on 3 JUNE 2017
        /// </summary>
        /// <param name="DepartmentObj"></param>
        /// <param name="ErrorMessage"></param>
        /// <returns></returns>
        public ActionResult ReportError(GlobalSettingModel GlobalSettingModelObj, string ErrorMessage)
        {
            AddAuditTrail(ErrorMessage);
            Error(ErrorMessage);

            return View(GlobalSettingModelObj);
        }

        //
        // GET: /WetosGlobalSetting/Edit/5

        public ActionResult Edit(int id)
        {
            GlobalSettingModel GlobalSettingModelObj = new GlobalSettingModel();

            try
            {
                GlobalSetting GlobalSettingEdit = WetosDB.GlobalSettings.Single(a => a.SettingId == id);

                GlobalSettingModelObj.SettingId = GlobalSettingEdit.SettingId;
                GlobalSettingModelObj.SettingText = GlobalSettingEdit.SettingText;
                GlobalSettingModelObj.SettingType = GlobalSettingEdit.SettingType.Value;
                GlobalSettingModelObj.SettingValue = GlobalSettingEdit.SettingValue;

                return View(GlobalSettingModelObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in Global setting edit due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Error in editing global setting");

                return View(GlobalSettingModelObj);
            }
        }

        //
        // POST: /WetosGlobalSetting/Edit/5

        [HttpPost]
        public ActionResult Edit(GlobalSettingModel GlobalSettingModelObj, int id, FormCollection collection)
        {
            try
            {
                string UpdateStatus = string.Empty;

                bool IsEdit = true;

                if (ModelState.IsValid)  // Added by Rajas on 29 DEC 2016 for validation
                {
                    // Updated by Rajas on 15 MAY 2017
                    if (UpdateGlobalSettingData(GlobalSettingModelObj, IsEdit, ref UpdateStatus) == true)
                    {
                        // ADDED BY RAJAS FOR AuditLog ON 27 DEC 2016
                        AddAuditTrail("Success - Setting : " + GlobalSettingModelObj.SettingText + " is updated successfully");


                        //ADDED CODE FOR SUCCESS MESSAGE BY RAJAS ON 16 JAN 2017
                        Success("Setting : " + GlobalSettingModelObj.SettingText + " is updated successfully");
                    }
                    else
                    {
                        return ReportError(GlobalSettingModelObj, UpdateStatus);
                    }

                }
                else
                {
                    return View(GlobalSettingModelObj);
                }

                return RedirectToAction("Index");

            }
            catch (System.Exception ex)
            {
                // Added by Rajas on 16 JAN 2017 START
                AddAuditTrail("Exception - Global setting update failed due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Error in adding new Gloabl Setting");
                // Added by Rajas on 16 JAN 2017 END

                return View(GlobalSettingModelObj);
            }
        }

        //
        // GET: /WetosGlobalSetting/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /WetosGlobalSetting/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }


        /// <summary>
        /// Common function to validate the data for edit or create
        /// Added by Rajas on 7 JUNE 2017
        /// </summary>
        private bool UpdateGlobalSettingData(GlobalSettingModel GlobalSettingModelObj, bool IsEdit, ref string UpdateStatus)
        {
            bool ReturnStatus = false;

            try
            {
                // Updated by Rajas on 31 MAY 2017
                WetosDB.GlobalSetting GlobalSettingTblObj = WetosDB.GlobalSettings.Where(a => (a.SettingText.Trim().ToUpper() == GlobalSettingModelObj.SettingText.Trim().ToUpper()
                    || a.SettingId == GlobalSettingModelObj.SettingId)).FirstOrDefault();

                // ADDED BY RAJAS ON 03 JUNE 2017 START
                bool IsNew = false;

                // IS OBJECT PRESET
                if (GlobalSettingTblObj != null)  // Added by Rajas on 2 JUNE 2017
                {
                    if (IsEdit == false)  // CREATE            
                    {
                        UpdateStatus = "Setting already available";

                        //AddAuditTrail("Error in Department update : " + UpdateStatus);
                        return ReturnStatus;
                    }
                }
                else
                {
                    if (IsEdit == false) // CREATE    
                    {
                        GlobalSettingTblObj = new WetosDB.GlobalSetting();
                        IsNew = true;
                    }
                    else // EDIT    
                    {
                        UpdateStatus = "Error in creating Global setting";
                        //AddAuditTrail("Error in Department update : " + UpdateStatus);
                        return ReturnStatus;
                    }
                }
                // ADDED BY RAJAS ON 03 JUNE 2017 END

                // New Leave table object
                GlobalSettingTblObj.SettingText = GlobalSettingModelObj.SettingText.Trim();

                GlobalSettingTblObj.SettingType = 1; //GlobalSettingModelObj.SettingType;

                GlobalSettingTblObj.SettingValue = GlobalSettingModelObj.SettingValue.Trim();

                // Add new table object 
                if (IsNew)
                {
                    WetosDB.GlobalSettings.Add(GlobalSettingTblObj);
                }

                WetosDB.SaveChanges();

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
