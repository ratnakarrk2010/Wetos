using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WetosDB;
using WetosMVCMainApp.Models;

namespace WetosMVC.Controllers
{
    [SessionExpire] 
    [Authorize]
    public class WetosDeviceMasterController : BaseController
    {
        
        //
        // GET: /WetosDeviceMaster/

        public ActionResult Index()
        {
            List<Device> DeviceMastersList = WetosDB.Devices.ToList();

            //ADDED BY RAJAS ON 27 DEC 2016
            AddAuditTrail("");

            return View(DeviceMastersList);
        }

        //
        // GET: /WetosDeviceMaster/Details/5

        public ActionResult Details(int id)
        {
            Device DeviceMasterDetails = WetosDB.Devices.Single(b => b.DeviceId == id);

            //ADDED BY RAJAS ON 27 DEC 2016
            AddAuditTrail("");

            return View(DeviceMasterDetails);
        }

        //
        // GET: /WetosDeviceMaster/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /WetosDeviceMaster/Create

        /// <summary>
        /// Added by 30 DEC 2016 by Rajas to validate data and save
        /// </summary>
        /// <param name="NewDeviceMasterObj"></param>
        /// <param name="collection"></param>
        /// <returns></returns>

        [HttpPost]
        public ActionResult Create(DeviceModel NewDeviceMasterObj, FormCollection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    UpdateDeviceMasterData(NewDeviceMasterObj);

                    // ADDED BY RAJAS FOR AuditLog ON 27 DEC 2016
                    AddAuditTrail("New device added");

                    return RedirectToAction("Create");
                }

                else
                {
                    return View();
                }
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /WetosDeviceMaster/Edit/5


        /// <summary>
        /// GET valid data for edit
        /// Added by Rajas on 30 DEC 2016 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            Device DeviceMasterEdit = new Device();
            DeviceMasterEdit = WetosDB.Devices.Single(b => b.DeviceId == id);

            DeviceModel NewDeviceMasterObj = new DeviceModel();
            NewDeviceMasterObj.DeviceId = DeviceMasterEdit.DeviceId;
            NewDeviceMasterObj.DeviceName = DeviceMasterEdit.DeviceName;
            NewDeviceMasterObj.DeviceNo = DeviceMasterEdit.DeviceNo;
            NewDeviceMasterObj.DISPLAYNAME = DeviceMasterEdit.DISPLAYNAME;

            return View(NewDeviceMasterObj);
        }

        //
        // POST: /WetosDeviceMaster/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, DeviceModel NewDeviceMasterObj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    UpdateDeviceMasterData(NewDeviceMasterObj);

                    // ADDED BY RAJAS FOR AuditLog ON 27 DEC 2016
                    AddAuditTrail("Device details updated");
                }

                else
                {
                    return View();
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /WetosDeviceMaster/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /WetosDeviceMaster/Delete/5

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
        /// Added by Rajas on 30 DEC 2016
        /// </summary>

        private bool UpdateDeviceMasterData(DeviceModel NewDeviceMasterObj)
        {
            bool ReturnStatus = false;

            WetosDB.Device DeviceTblObj = WetosDB.Devices.Where(a => a.DeviceName == NewDeviceMasterObj.DeviceName || a.DeviceId == NewDeviceMasterObj.DeviceId).FirstOrDefault();

            bool IsNew = false;
            if (DeviceTblObj == null)
            {
                DeviceTblObj = new WetosDB.Device();
                IsNew = true;
            }


            // New Leave table object
            DeviceTblObj.DeviceId = NewDeviceMasterObj.DeviceId;
            DeviceTblObj.DeviceName = NewDeviceMasterObj.DeviceName;
            DeviceTblObj.DeviceNo = NewDeviceMasterObj.DeviceNo;
            DeviceTblObj.DISPLAYNAME = NewDeviceMasterObj.DISPLAYNAME;


            // Add new table object 
            if (IsNew)
            {
                WetosDB.Devices.Add(DeviceTblObj);
            }

            WetosDB.SaveChanges();

            return ReturnStatus;
        }
    }
}
