using System.Web.Mvc;

namespace WetosMVC.Controllers
{
    [SessionExpire] 
    [Authorize]
    public class WetosDashboardController : BaseController
    {
        
        //
        // GET: /WetosDashboard/

        public ActionResult Index()
        {
            //ADDED BY RAJAS ON 27 DEC 2016
            AddAuditTrail("");
            return View();
        }

        //
        // GET: /WetosDashboard/Details/5

        public ActionResult Details(int id)
        {
            //ADDED BY RAJAS ON 27 DEC 2016
            AddAuditTrail("");
            return View();
        }

        //
        // GET: /WetosDashboard/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /WetosDashboard/Create

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
        // GET: /WetosDashboard/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /WetosDashboard/Edit/5

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
        // GET: /WetosDashboard/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /WetosDashboard/Delete/5

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
