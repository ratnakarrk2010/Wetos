using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WetosDB;
using WetosMVCMainApp.Models;

namespace WetosMVC.Controllers
{
    public class WetosTimelineController : BaseController
    {
        //
        // GET: /WetosTimeline/

        public ActionResult Index()
        {
            int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            SP_EmployeeTimeLine_Result EmployeeTimeLineObj = WetosDB.SP_EmployeeTimeLine(EmployeeId).FirstOrDefault();
            return View(EmployeeTimeLineObj);
        }

        //
        // GET: /WetosTimeline/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /WetosTimeline/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /WetosTimeline/Create

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
        // GET: /WetosTimeline/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /WetosTimeline/Edit/5

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
        // GET: /WetosTimeline/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /WetosTimeline/Delete/5

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
