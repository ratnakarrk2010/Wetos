using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WetosDB;
using WetosMVCMainApp.Models;

namespace WetosMVC.Controllers
{
    [SessionExpire]
    [Authorize]
    public class DropdownDataController : BaseController
    {
        //DropdownData
        public ActionResult Index()
        {
            try
            {
                List<DropdownData> DropdownDataList = WetosDB.DropdownDatas.ToList();

                AddAuditTrail("Success - Visited Dropdown Data Index");

                return View(DropdownDataList);
            }

            catch (System.Exception ex)
            {
                AddAuditTrail("Exception - " + ex.Message);

                return View();
            }
        }

        //ADDED BY NANDINI ON 14 APRIL 2020
        [HttpGet]
        public ActionResult Details(int id)
        {
            try
            {
                DropdownData DDObj = WetosDB.DropdownDatas.Where(a => a.ID == id).FirstOrDefault();
                DropdownDataModel DDMObj = new DropdownDataModel();
                if (DDObj != null)
                {
                    DDMObj.DropdownId = DDObj.ID;
                    DDMObj.TypeId = DDObj.TypeId.Value;
                    DDMObj.TypeName = DDObj.Type;
                    DDMObj.TextName = DDObj.Text;
                    DDMObj.Value = DDObj.Value.Value;
                    PopulateDropdown();  //ADDED BY NANDINI ON 02 MAY 2020
                }
                AddAuditTrail("Success - Visited Dropdown Data Details");
                return View(DDMObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + ex.InnerException);
                PopulateDropdown();  //ADDED BY NANDINI ON 02 MAY 2020
                return RedirectToAction("Index");
            }
        }

        //ADDED BY NANDINI ON 14 APRIL 2020
        [HttpGet]
        public ActionResult Create()
        {
            try
            {
                AddAuditTrail("Success - Visited Dropdown Data Create");
                PopulateDropdown();  //ADDED BY NANDINI ON 02 MAY 2020
                return View();
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + ex.InnerException);
                PopulateDropdown();  //ADDED BY NANDINI ON 02 MAY 2020
                return View();  // Verify ?
            }
        }

        //ADDED BY NANDINI ON 14 APRIL 2020
        [HttpPost]
        public ActionResult Create(DropdownDataModel DropdownDataModelObj, FormCollection collection)
        {
            try
            {
                string RadioButton = Request.Form["opt"];
                DropdownData DropdownDataObj = new DropdownData();
                //ADDED BY NANDINI ON 02 MAY 2020
                if (RadioButton == "ChildWise")
                {
                    if (DropdownDataModelObj.TypeNameID != 0)
                    {
                        DropdownData DD = WetosDB.DropdownDatas.Where(a => a.TypeId == DropdownDataModelObj.TypeNameID).FirstOrDefault();
                        DropdownDataObj.Type = DD.Type;
                    }

                    DropdownDataObj.Text = DropdownDataModelObj.TextName;
                    DropdownDataObj.Value = DropdownDataModelObj.Value;
                    DropdownDataObj.TypeId = DropdownDataModelObj.TypeId;
                    WetosDB.DropdownDatas.Add(DropdownDataObj);
                    WetosDB.SaveChanges();
                    PopulateDropdown();  //ADDED BY NANDINI ON 02 MAY 2020
                }
                else
                {
                    DropdownDataObj.Type = DropdownDataModelObj.TypeName;

                    DropdownDataObj.Text = DropdownDataModelObj.TextName;
                    DropdownDataObj.Value = DropdownDataModelObj.Value;
                    DropdownDataObj.TypeId = DropdownDataModelObj.TypeId;
                    WetosDB.DropdownDatas.Add(DropdownDataObj);
                    WetosDB.SaveChanges();
                    PopulateDropdown();  //ADDED BY NANDINI ON 02 MAY 2020
                }
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + ex.InnerException);

                Error(WetosErrorMessageController.GetErrorMessage(6));
                PopulateDropdown();  //ADDED BY NANDINI ON 02 MAY 2020
                return RedirectToAction("Index");
            }
            Success("Successfully Create Dropdown Data Name are:  " + DropdownDataModelObj.TypeName);
            return RedirectToAction("Index");
        }

        //ADDED BY NANDINI ON 14 APRIL 2020
        [HttpGet]
        public ActionResult Edit(int id)
        {
            try
            {
                DropdownData DDObj = WetosDB.DropdownDatas.Where(a => a.ID == id).FirstOrDefault();
                DropdownDataModel DDMObj = new DropdownDataModel();
                if (DDObj != null)
                {
                    DDMObj.DropdownId = DDObj.ID;
                    DDMObj.TypeId = DDObj.TypeId.Value;
                    DDMObj.TypeName = DDObj.Type;
                    DDMObj.TextName = DDObj.Text;
                    DDMObj.Value = DDObj.Value.Value;
                    PopulateDropdown();  //ADDED BY NANDINI ON 02 MAY 2020
                }
                AddAuditTrail("Success - Visited Dropdown Data Edit");
                return View(DDMObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + ex.InnerException);

                return RedirectToAction("Index");
            }
        }

        //ADDED BY NANDINI ON 14 APRIL 2020
        [HttpPost]
        public ActionResult Edit(int id, DropdownDataModel DDMObj)
        {
            try
            {
                DropdownData DDObj = WetosDB.DropdownDatas.Where(a => a.ID == id).FirstOrDefault();
                if (DDObj != null)
                {
                    DDObj.TypeId = DDMObj.TypeId;
                    DDObj.Type = DDMObj.TypeName;
                    DDObj.Text = DDMObj.TextName;
                    DDObj.Value = DDMObj.Value;
                    PopulateDropdown();  //ADDED BY NANDINI ON 02 MAY 2020
                    WetosDB.SaveChanges();
                }

            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + ex.InnerException);

                Error(WetosErrorMessageController.GetErrorMessage(6));
                PopulateDropdown();  //ADDED BY NANDINI ON 02 MAY 2020
                return RedirectToAction("Index");
            }
            Success("Successfully Update Dropdown Data Name are: " + DDMObj.TypeName);
            return RedirectToAction("Index");
        }

        //ADDED BY NANDINI ON 14 APRIL 2020
        [HttpGet]
        public ActionResult Delete(int id)
        {
            try
            {
                DropdownData DDObj = WetosDB.DropdownDatas.Where(a => a.ID == id).FirstOrDefault();
                DropdownDataModel DDMObj = new DropdownDataModel();
                if (DDObj != null)
                {
                    DDMObj.DropdownId = DDObj.ID;
                    DDMObj.TypeId = DDObj.TypeId.Value;
                    DDMObj.TypeName = DDObj.Type;
                    DDMObj.TextName = DDObj.Text;
                    DDMObj.Value = DDObj.Value.Value;
                    PopulateDropdown();  //ADDED BY NANDINI ON 02 MAY 2020
                }
                AddAuditTrail("Success - Visited Dropdown Data Delete");
                return View(DDMObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + ex.InnerException);
                PopulateDropdown();  //ADDED BY NANDINI ON 02 MAY 2020
                return RedirectToAction("Index");
            }
        }


        //ADDED BY NANDINI ON 14 APRIL 2020
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                DropdownData DDObj = WetosDB.DropdownDatas.Where(a => a.ID == id).FirstOrDefault();
                if (DDObj != null)
                {
                    WetosDB.DropdownDatas.Add(DDObj);
                    WetosDB.SaveChanges();
                }

                Success("Successfully Delete " + DDObj.Type + " - " + DDObj.Text);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //ADDED BY NANDINI ON 02 MAY 2020
        public void PopulateDropdown()
        {
            var DropdownList = WetosDB.DropdownDatas.Select(a => new { Value = a.TypeId, Text = a.Type }).Distinct().ToList();
            ViewBag.DropdownListVB = new SelectList(DropdownList, "Value", "Text").ToList();
        }
        //ADDED BY NANDIN ON 02 MAY 2020
        [HttpPost]
        public JsonResult ExistingDataDetails(int TypeNameID)
        {
            DropdownData DD = WetosDB.DropdownDatas.Where(a => a.TypeId == TypeNameID).FirstOrDefault();
            return Json(DD, JsonRequestBehavior.AllowGet);
        }
    }
}

