using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WetosDB;
using WetosMVCMainApp.Models;

namespace WetosMVC.Controllers
{
    [SessionExpire] 
    [Authorize]
    public class WetosBloodGroupController : BaseController
    {
        

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            try
            {
                List<BloodGroup> BloodGroupsList = WetosDB.BloodGroups.ToList();

                // ADDED BY RAJAS FOR AuditLog ON 27 DEC 2016
                AddAuditTrail("Visited blood group master");

                return View(BloodGroupsList);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Inconsistent data. Please try again!");

                return View(); // Verify ?
            }
        }

        //
        // GET: /WetosBloodGroup/Details/5

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int id)
        {
            try
            {
                BloodGroup bloodgroupdetails = WetosDB.BloodGroups.Single(a => a.BloodGroupId == id);

                return View(bloodgroupdetails);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Inconsistent data. Please try again and verify!");

                return View();  // Verify ?
            }
        }

        //
        // GET: /WetosBloodGroup/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /WetosBloodGroup/Create

        /// <summary>
        /// Blood group posting along with validation
        /// Added by Rajas on 29 DEC 2016
        /// </summary>
        /// <param name="NewBloodGroupObj"></param>
        /// <param name="collection"></param>
        /// <returns></returns>

        [HttpPost]
        public ActionResult Create(BloodGroupModel NewBloodGroupObj, FormCollection collection)
        {
            try
            {
                // Added by Rajas on 27 MARCH 2017
                string UpdateStatus = string.Empty;

                if (ModelState.IsValid)
                {
                    if (UpdateBloodGroupData(NewBloodGroupObj, ref UpdateStatus) == true)
                    {
                        // ADDED BY RAJAS FOR AuditLog ON 27 DEC 2016
                        AddAuditTrail("Blood group added : " + NewBloodGroupObj.BloodGroupName);

                        Success("Blood Group : " + NewBloodGroupObj.BloodGroupName + " is added successfully");
                    }
                    else
                    {
                        AddAuditTrail(UpdateStatus);

                        Error(UpdateStatus);
                    }

                    // ADDED BY RAJAS FOR AuditLog ON 27 DEC 2016
                    AddAuditTrail("Blood group Created");

                    return RedirectToAction("Index");
                }

                else
                {
                    return View();
                }
            }
            catch (System.Exception ex)
            {
                // ADDED BY RAJAS FOR AuditLog ON 27 DEC 2016
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Can not add blood group. Please try again!");

                return View();
            }
        }

        //
        // GET: /WetosBloodGroup/Edit/5

        public ActionResult Edit(int id)
        {

            BloodGroup BloodGroupdetails = new BloodGroup();
            BloodGroupdetails = WetosDB.BloodGroups.Where(a => a.BloodGroupId == id).FirstOrDefault();

            // Addded by Rajas on 30 DEC 2016 for Saving data from Table object to Model object 
            BloodGroupModel NewBloodGroupObj = new BloodGroupModel();

            NewBloodGroupObj.BloodGroupId = BloodGroupdetails.BloodGroupId;

            NewBloodGroupObj.BloodGroupName = BloodGroupdetails.BloodGroupName;

            return View(NewBloodGroupObj);
        }

        //
        // POST: /WetosBloodGroup/Edit/5

        /// <summary>
        /// POSTING after validation for blood group data through model 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="NewBloodGroupObj"></param>
        /// <returns></returns>

        [HttpPost]
        public ActionResult Edit(int id, BloodGroupModel NewBloodGroupObj)
        {
            try
            {
                // Added by Rajas on 27 MARCH 2017
                string UpdateStatus = string.Empty;

                if (ModelState.IsValid)
                {
                    if (UpdateBloodGroupData(NewBloodGroupObj, ref UpdateStatus) == true)
                    {
                        // ADDED BY RAJAS FOR AuditLog ON 27 DEC 2016
                        AddAuditTrail("Blood group Updated");

                        Success("Blood Group : " + NewBloodGroupObj.BloodGroupName + " is Updated successfully");
                    }
                    else
                    {
                        AddAuditTrail(UpdateStatus);

                        Error(UpdateStatus);
                    }

                }
                else
                {
                    return View(NewBloodGroupObj);
                }

                return RedirectToAction("Index");
            }
            catch (System.Exception ex)
            {
                // ADDED BY RAJAS FOR AuditLog ON 27 DEC 2016
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Can not update blood group. Please try again!");

                return View();
            }
        }

        //
        // GET: /WetosBloodGroup/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /WetosBloodGroup/Delete/5

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
        /// Common function to validate the data for edit or save
        /// Added by Rajas on 30 DEC 2016
        /// ref UpdateStatus, Updated by Rajas on 27 MARCH 2017
        /// </summary>
        /// <param name="NewBloodGroupObj"></param>
        /// <returns></returns>
        private bool UpdateBloodGroupData(BloodGroupModel NewBloodGroupObj, ref string UpdateStatus)
        {
            bool ReturnStatus = false;

            try
            {

                WetosDB.BloodGroup BloodGroupTblObj = WetosDB.BloodGroups.Where(a => a.BloodGroupName == NewBloodGroupObj.BloodGroupName || a.BloodGroupId == NewBloodGroupObj.BloodGroupId).FirstOrDefault();

                bool IsNew = false;
                if (BloodGroupTblObj == null)
                {
                    BloodGroupTblObj = new WetosDB.BloodGroup();
                    IsNew = true;
                }


                // New Leave table object
                BloodGroupTblObj.BloodGroupName = NewBloodGroupObj.BloodGroupName;


                // Add new table object 
                if (IsNew)
                {
                    WetosDB.BloodGroups.AddObject(BloodGroupTblObj);
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
