using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WetosDB;
using WetosMVC.Models;
using WetosMVCMainApp.Models;
using System.Data.SqlClient;
using System.Configuration;

namespace WetosMVC.Controllers
{
    public class WetosSqlQueryController : BaseController
    {
        //
        // GET: /WetosSqlQuery/

        public ActionResult Index()
        {
            return View(WetosDB.SqlQueries.ToList());
        }

        //
        // GET: /WetosSqlQuery/Details/5

        public ActionResult Details(int id = 0)
        {
            SqlQuery sqlquery = WetosDB.SqlQueries.Single(s => s.SqlQueryId == id);
            if (sqlquery == null)
            {
                return HttpNotFound();
            }
            return View(sqlquery);
        }

        //
        // GET: /WetosSqlQuery/Create

        public ActionResult Create()
        {
            SqlQueryModel sqlqueryModel = new SqlQueryModel();
            return View(sqlqueryModel);
        }

        //
        // POST: /WetosSqlQuery/Create

        [HttpPost]
        public ActionResult Create(SqlQueryModel sqlqueryModel)
        {
            try
            {
                SqlQuery sqlquery = new SqlQuery();
                sqlquery.SqlQueryName = sqlqueryModel.SqlQueryName;
                sqlquery.SqlQueryDescription = sqlqueryModel.SqlQueryDescription;
                sqlquery.SqlQuery1 = sqlqueryModel.SqlQuery;
                sqlquery.Status = 1;
                WetosDB.SqlQueries.AddObject(sqlquery);
                WetosDB.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }

            Success("Successfully Created SQL Query" + sqlqueryModel.SqlQueryName);
            return RedirectToAction("Index");
        }

        //
        // GET: /WetosSqlQuery/Edit/5

        public ActionResult Edit(int id = 0)
        {
            SqlQuery sqlquery = new SqlQuery();
            sqlquery = WetosDB.SqlQueries.Single(b => b.SqlQueryId == id);

            SqlQueryModel sqlqueryModel = new SqlQueryModel();
            sqlqueryModel.SqlQueryName = sqlquery.SqlQueryName;
            sqlqueryModel.SqlQueryDescription = sqlquery.SqlQueryDescription;
            sqlqueryModel.SqlQuery = sqlquery.SqlQuery1;

            AddAuditTrail("Visit SQL Edit View");
            return View(sqlqueryModel);
        }

        //
        // POST: /WetosSqlQuery/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, SqlQueryModel sqlqueryModel)
        {
            try
            {
                SqlQuery sqlquery = WetosDB.SqlQueries.Single(s => s.SqlQueryId == id);
                if (sqlquery != null)
                {

                    sqlquery.SqlQueryName = sqlqueryModel.SqlQueryName;
                    sqlquery.SqlQueryDescription = sqlqueryModel.SqlQueryDescription;
                    sqlquery.SqlQuery1 = sqlqueryModel.SqlQuery;

                    WetosDB.SaveChanges();
                }
                Success("Successfully Update SQL Query" + sqlqueryModel.SqlQueryName);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /WetosSqlQuery/Delete/5

        public ActionResult Delete(int id = 0)
        {
            try
            {
                SqlQuery sqlquery = WetosDB.SqlQueries.Single(s => s.SqlQueryId == id);
                if (sqlquery == null)
                {


                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }

        }

        //
        // POST: /WetosSqlQuery/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            SqlQuery sqlquery = WetosDB.SqlQueries.Single(s => s.SqlQueryId == id);
            WetosDB.SqlQueries.DeleteObject(sqlquery);
            WetosDB.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            WetosDB.Dispose();
            base.Dispose(disposing);
        }

        //ADDED BY NANDINI ON 17 APRIL 2020
        public JsonResult SQLQueryDetails(int SqlQueryId)
        {
            SqlQuery SQLObj = WetosDB.SqlQueries.Where(a => a.SqlQueryId == SqlQueryId).FirstOrDefault();
            return Json(SQLObj);
        }

        //ADDED BY NANDINI ON 17 APRIL 2020
        [HttpGet]
        public ActionResult SQLQueryCreate()
        {
            SqlQueryModel sqlqueryModel = new SqlQueryModel();
            var SqlQueryObj = WetosDB.SqlQueries.Select(a => new { Value = a.SqlQueryId, Text = a.SqlQueryName }).ToList();
            ViewBag.SqlQueryObjVBList = new SelectList(SqlQueryObj, "Value", "Text").ToList();
            return View(sqlqueryModel);
        }

        //ADDED BY NANDINI ON 17 APRIL 2020 
        [HttpPost]
        public ActionResult SQLQueryListPV(int SqlQueryId)
        {
          var SqlQueryModelObj = new List<SqlQueryModel>();
            SqlQuery SQLObj = WetosDB.SqlQueries.Where(a => a.SqlQueryId == SqlQueryId).FirstOrDefault();
            DataSet DT = new DataSet();
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);
            SqlDataAdapter DA = new SqlDataAdapter(SQLObj.SqlQuery1, con);
            DA.Fill(DT);
            //List<DataRow> SqlData1 = DT.AsEnumerable().ToList();
             
            //SqlQueryModelObj = DA;
            return PartialView(DT);

        } 
    }
}