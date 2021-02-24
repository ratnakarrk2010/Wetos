using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WetosDB;
using WetosMVCMainApp.Models;

namespace WetosMVC.Controllers
{
    [SessionExpire] 
    [Authorize]
    public class WetosReligionController : BaseController
    {
        
        //
        // GET: /WetosReligion/
        /// <summary>
        /// Addedby Mousami on 02 SEP 2016 start
        /// </summary>
        /// <returns></returns>

        public ActionResult Index()
        {           
            //Added by dhanashri on 3 june 2019 START
            try
            {
                // Updated by Rajas on 28 MARCH 2017
                List<Religion> LocationListObj = WetosDB.Religions.ToList(); //.Where(a => a.MarkedAsDelete == 0 || a.MarkedAsDelete == null).ToList();

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
            //Added by dhanashri on 3 june 2019 END
        }

        //
        // GET: /WetosReligion/Details/5

        public ActionResult Details(int id)
        {
            Religion ReligionDetails = WetosDB.Religions.Single(b => b.ReligionId == id);
            return View(ReligionDetails);
        }

        //
        // GET: /WetosReligion/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /WetosReligion/Create
        //Added by dhanashri on 3 june 2019 Start
        [HttpPost]
        public ActionResult Create(ReligionModel ReligionModelObj, FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                //WetosDB.Religions.AddObject(ReligionObj);
                Religion ReligionObj = new Religion();
                Religion ExistingReligion = WetosDB.Religions.Where(a => a.ReligionName == ReligionModelObj.ReligionName && a.MarkedAsDelete !=1).FirstOrDefault();
                if (ExistingReligion != null) // CHECKING
                {
                    ModelState.AddModelError("", "Religion already exist!!!");
                    Error("Religion already exist");
                }
                else
                {

                    ReligionObj.ReligionName = ReligionModelObj.ReligionName;
                    WetosDB.Religions.AddObject(ReligionObj);
                    WetosDB.SaveChanges();
                    
                    // ADDED BY RAJAS FOR AuditLog ON 27 DEC 2016
                    AddAuditTrail("New religion added");
                }
              
               

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        //Added by dhanashri on 3 june 2019 End

        //
        // GET: /WetosReligion/Edit/5
 
        public ActionResult Edit(int id)
        {
            //Added by dhanashri on 3 june 2019 Start
            Religion ReligionEdit = WetosDB.Religions.Single(b => b.ReligionId == id);
            return View(ReligionEdit);
            //Added by dhanashri on 3 june 2019 End
        }

        //
        // POST: /WetosReligion/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, Religion ReligionEdit)
        {
            try
            {
                // TODO: Add update logic here
                Religion ReligionObj = WetosDB.Religions.Where(b => b.ReligionId == id).FirstOrDefault();
                ReligionObj.ReligionName = ReligionEdit.ReligionName;
                WetosDB.SaveChanges();

                // ADDED BY RAJAS FOR AuditLog ON 27 DEC 2016
                AddAuditTrail("Religion details updated");
  
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /WetosReligion/Delete/5
 
        public ActionResult Delete(int id)
        {
            //Added by dhanashri on 3 june 2019 Start
            Religion ReligionDelete = WetosDB.Religions.Single(b => b.ReligionId == id);
            return View(ReligionDelete);
            //Added by dhanashri on 3 june 2019 End
        }

        //
        // POST: /WetosReligion/Delete/5
        //Added by dhanashri on 3 june 2019 START
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)  
        {
            try
            {
                Religion ReligionObj = WetosDB.Religions.Where(a => a.ReligionId == id).FirstOrDefault();

                List<Employee> EmployeeListObj = WetosDB.Employees.Where(a => a.CompanyId == id).ToList();

                if (EmployeeListObj.Count > 0)
                {
                    Information("You can not delete this religion, as it is assigned to employee");

                    AddAuditTrail("Can not delete religion as it is used in employee");

                    return RedirectToAction("Index");
                }

                if (ReligionObj != null)
                {
                    ReligionObj.MarkedAsDelete = 1;

                    WetosDB.SaveChanges();

                    Success("Religion " + ReligionObj.ReligionName + " deleted successfully");

                    AddAuditTrail("Religion " + ReligionObj.ReligionName + " deleted successfully");
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
        //Added by dhanashri on 3 june 2019 END
    }
}
