using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WetosMVC.Controllers
{
    public class WetosAssignEmployeeGroupController : BaseController
    {
        //
        // GET: /WetosAssignEmployeeGroup/

        public ActionResult Index()
        {
            try
            {
                List<WetosDB.SP_GetEmployeeGroupDetail_Result> GetEmployeeGroupDetailList = WetosDB.SP_GetEmployeeGroupDetail().ToList();
                if (GetEmployeeGroupDetailList == null)
                {
                    GetEmployeeGroupDetailList = new List<WetosDB.SP_GetEmployeeGroupDetail_Result>();
                }
                return View(GetEmployeeGroupDetailList);
            }
            catch (Exception E)
            {
                List<WetosDB.SP_GetEmployeeGroupDetail_Result> GetEmployeeGroupDetailList = new List<WetosDB.SP_GetEmployeeGroupDetail_Result>();
                AddAuditTrail("Error in Opening Employee group detail List of WetosAssignEmployeeGroupController:" + E.Message + (E.InnerException == null ? string.Empty : E.InnerException.Message));
                return View(GetEmployeeGroupDetailList);
            }
        }

        //
        // GET: /WetosAssignEmployeeGroup/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /WetosAssignEmployeeGroup/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /WetosAssignEmployeeGroup/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /WetosAssignEmployeeGroup/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /WetosAssignEmployeeGroup/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /WetosAssignEmployeeGroup/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /WetosAssignEmployeeGroup/Delete/5

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
