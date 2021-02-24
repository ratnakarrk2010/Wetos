using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WetosDB;
using System;

namespace WetosMVC.Controllers
{
    [SessionExpire] 
    [Authorize]
    public class WetosPhysicalLocationController : BaseController
    {
        
        //
        // GET: /WetosPhysicalLocation/

        public ActionResult Index()
        {
            List<Location> LocDetails = WetosDB.Locations.ToList();
            return View(LocDetails);
        }

        //
        // GET: /WetosPhysicalLocation/Details/5

        public ActionResult Details(int id)
        {
            Location LocationDetails = WetosDB.Locations.Where(a => a.LocationId == id).FirstOrDefault();
            return View(LocationDetails);
        }

        //
        // GET: /WetosPhysicalLocation/Create

        public ActionResult Create()
        {
            // Drop down for company list

            #region COMPANY LIST CODE ADDED BY SHRADDHA ON 30 MAR 2018
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
            int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            //var CompanyName = WetosDB.Companies.Select(a => new { CompanyId = a.CompanyId, CompanyName = a.CompanyName }).ToList();
            var CompanyName = WetosDB.SP_GetCompanyList(EmployeeId).Select(m => new { Companyid = m.CompanyId, CompanyName = m.CompanyName }).ToList();
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
            #endregion

            ViewBag.CompanyNameList = new SelectList(CompanyName, "CompanyId", "CompanyName").ToList();

            // Drop down for Branch name
            #region CODE FOR GET BRANCH LIST ADDED BY SHRADDHA ON 30 MAR 2018
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
            //var BranchName = WetosDB.Branches.Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList();
            var BranchName = WetosDB.SP_GetBranchList(EmployeeId).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList(); //.Select(m => new { BranchId = m.BranchId, BranchName = m.BranchName}).ToList();
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
            #endregion

            ViewBag.BranchNameList = new SelectList(BranchName, "BranchId", "BranchName").ToList();
            return View();
        }

        //
        // POST: /WetosPhysicalLocation/Create

        [HttpPost]
        public ActionResult Create(Location NewLocation, FormCollection collection)
        {
            try
            {

                WetosDB.Locations.AddObject(NewLocation);
                WetosDB.SaveChanges();
                // TODO: Add insert logic here

                // ADDED BY RAJAS FOR AuditLog ON 27 DEC 2016
                AddAuditTrail("New physical location added");

                return RedirectToAction("Create");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /WetosPhysicalLocation/Edit/5

        public ActionResult Edit(int id)
        {

            // get existing ShiftId
            Location LocEdit = WetosDB.Locations.Single(b => b.LocationId == id);

            // Drop down for company list

            #region COMPANY LIST CODE ADDED BY SHRADDHA ON 30 MAR 2018
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
            int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            //var CompanyName = WetosDB.Companies.Select(a => new { CompanyId = a.CompanyId, CompanyName = a.CompanyName }).ToList();
            var CompanyName = WetosDB.SP_GetCompanyList(EmployeeId).Select(m => new { Companyid = m.CompanyId, CompanyName = m.CompanyName }).ToList();
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
            #endregion

            ViewBag.CompanyNameList = new SelectList(CompanyName, "CompanyId", "CompanyName").ToList();

            // Drop down for Branch name
            #region CODE FOR GET BRANCH LIST ADDED BY SHRADDHA ON 30 MAR 2018
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
            //var BranchName = WetosDB.Branches.Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList();
            var BranchName = WetosDB.SP_GetBranchList(EmployeeId).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList(); //.Select(m => new { BranchId = m.BranchId, BranchName = m.BranchName}).ToList();
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
            #endregion

            ViewBag.BranchNameList = new SelectList(BranchName, "BranchId", "BranchName").ToList();

            return View(LocEdit);
        }

        //
        // POST: /WetosPhysicalLocation/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, Location LocEdit)
        {
            try
            {
                Location LocNewObj = WetosDB.Locations.Single(e => e.LocationId == id);

                //LocNewObj LocationName = LocEdit.LocationName;
                //LocNewObj LocationId = LocEdit.LocationId;

                WetosDB.SaveChanges();

                // TODO: Add update logic here
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /WetosPhysicalLocation/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /WetosPhysicalLocation/Delete/5

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
    }
}
