using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WetosDB;
using WetosMVCMainApp.Models;
using System;

namespace WetosMVC.Controllers
{
    [SessionExpire]
    [Authorize]
    public class WetosSchedularController : BaseController
    {
        //
        // GET: /WetosSchedular/

        public ActionResult Index()
        {
            List<Schedular> SchedularList = WetosDB.Schedulars.ToList();
            return View(SchedularList);
        }

        //
        // GET: /WetosSchedular/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /WetosSchedular/Create

        public ActionResult Create()
        {
            SchedularModel SchedularModelObj = new SchedularModel();
            PopulateDropdown();
            return View(SchedularModelObj);
        }

        //
        // POST: /WetosSchedular/Create

        [HttpPost]
        public ActionResult Create(SchedularModel SchedularModelObj, FormCollection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Schedular SchedularObj = new Schedular();
                    SchedularObj.SchedularName = SchedularModelObj.SchedularName;
                    SchedularObj.Description = SchedularModelObj.Description;
                    SchedularObj.ScheduleType = SchedularModelObj.ScheduleType;
                    SchedularObj.StartTime = Convert.ToDateTime(SchedularModelObj.StartTime);
                    SchedularObj.EndTime = Convert.ToDateTime(SchedularModelObj.EndTime);
                    SchedularObj.FrequencyInMin = SchedularModelObj.FrequencyInMin;
                    SchedularObj.RepeatCycle = SchedularModelObj.RepeatCycle;
                    WetosDB.Schedulars.AddObject(SchedularObj);
                    WetosDB.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    PopulateDropdown();
                    return View(SchedularModelObj);
                }

            }
            catch (Exception E)
            {
                PopulateDropdown();
                AddAuditTrail("Error in Schedular Create:" + E.Message + (E.InnerException == null ? string.Empty : E.InnerException.Message));
                return View(SchedularModelObj);
            }
        }

        //
        // GET: /WetosSchedular/Edit/5

        public ActionResult Edit(int id)
        {
            //CODE ADDED BY SHRADDHA ON 13 FEB 2018 START
            Schedular SchedularObj = WetosDB.Schedulars.Where(a => a.ScheduleId == id).FirstOrDefault();
            if (SchedularObj == null)
            {
                SchedularObj = new Schedular();
            }
            SchedularModel SchedularModelObj = new SchedularModel();
            SchedularModelObj.SchedularName = SchedularObj.SchedularName;
            SchedularModelObj.Description = SchedularObj.Description;
            SchedularModelObj.ScheduleType = SchedularObj.ScheduleType;
            SchedularModelObj.StartTime = Convert.ToString(SchedularObj.StartTime.Value.TimeOfDay);
            SchedularModelObj.EndTime = Convert.ToString(SchedularObj.EndTime.Value.TimeOfDay);
            SchedularModelObj.FrequencyInMin = SchedularObj.FrequencyInMin;
            SchedularModelObj.RepeatCycle = SchedularObj.RepeatCycle;
            SchedularModelObj.ScheduleId = SchedularObj.ScheduleId;
            PopulateDropdown();
            //CODE ADDED BY SHRADDHA ON 13 FEB 2018 END
            return View(SchedularModelObj);
        }

        //
        // POST: /WetosSchedular/Edit/5

        [HttpPost]
        public ActionResult Edit(SchedularModel SchedularModelObj, FormCollection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Schedular SchedularObj = WetosDB.Schedulars.Where(a => a.ScheduleId == SchedularModelObj.ScheduleId).FirstOrDefault();
                    SchedularObj.SchedularName = SchedularModelObj.SchedularName;
                    SchedularObj.Description = SchedularModelObj.Description;
                    SchedularObj.ScheduleType = SchedularModelObj.ScheduleType;
                    SchedularObj.StartTime = Convert.ToDateTime(SchedularModelObj.StartTime);
                    SchedularObj.EndTime = Convert.ToDateTime(SchedularModelObj.EndTime);
                    SchedularObj.FrequencyInMin = SchedularModelObj.FrequencyInMin;
                    SchedularObj.RepeatCycle = SchedularModelObj.RepeatCycle;
                    WetosDB.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    PopulateDropdown();
                    return View(SchedularModelObj);
                }

            }
            catch (Exception E)
            {
                PopulateDropdown();
                AddAuditTrail("Error in Schedular Edit:" + E.Message + (E.InnerException == null ? string.Empty : E.InnerException.Message));
                return View(SchedularModelObj);
            }
        }

        //
        // GET: /WetosSchedular/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /WetosSchedular/Delete/5

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

        public void PopulateDropdown()
        {
            // Drop down for Scheduler Type
            // Updated by Rajas on 6 MAY 2017 to get data which is not MarkedAsDelete
            var SchedulerTypeList = WetosDB.DropdownDatas.Where(a => a.TypeId == 20).Select(a => new { Value = a.Value, Text = a.Text }).ToList();
            ViewBag.SchedulerTypeList = new SelectList(SchedulerTypeList, "Value", "Text").ToList();
        }
    }
}
