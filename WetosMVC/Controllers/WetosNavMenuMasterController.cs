using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WetosDB;
using WetosMVCMainApp.Models;

namespace WetosMVC.Controllers
{
    [SessionExpire]
    [Authorize]
    public class WetosNavMenuMasterController : BaseController
    {
        //WetosNavMenuMaster
        //ADDED BY NANDINI ON 14 APRIL 2020
        public ActionResult Index()
        {
            try
            {
                List<NavMenu> NavMenuList = WetosDB.NavMenus.OrderByDescending(a => a.navmenuId).ToList();

                AddAuditTrail("Success - Visited Nav Menus Index");

                return View(NavMenuList);
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
                NavMenu NavMenuObj = WetosDB.NavMenus.Where(a => a.navmenuId == id).FirstOrDefault();
                NavMenuModel NMMObj = new NavMenuModel();
                if (NavMenuObj != null)
                {
                    NMMObj.NavMenuId = NavMenuObj.navmenuId;
                    NMMObj.MenuNameId = NavMenuObj.parId;
                    NMMObj.NavMenuName = NavMenuObj.navmenuname;
                    NMMObj.NavLink = NavMenuObj.navlink;
                    NMMObj.SrNo = NavMenuObj.srno;
                    DropdownData DDObj = WetosDB.DropdownDatas.Where(a => a.Text.Trim().ToUpper() == NavMenuObj.Icon.Trim().ToUpper() && a.TypeId == 33).FirstOrDefault();
                    if (DDObj != null)
                    {
                        NMMObj.NavMenuIcon = DDObj.Value.Value;
                    }


                }
                PopulateDropDown();
                AddAuditTrail("Success - Visited Nav Menu Details");
                return View(NMMObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + ex.InnerException);

                return RedirectToAction("Index");
            }
        }

        //ADDED BY NANDINI ON 14 APRIL 2020
        [HttpGet]
        public ActionResult Create()
        {
            try
            {
                AddAuditTrail("Success - Visited Nav Menu Create");
                PopulateDropDown();
                return View();
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + ex.InnerException);

                return View();  // Verify ?
            }
        }

        //ADDED BY NANDINI ON 14 APRIL 2020
        [HttpPost]
        public ActionResult Create(NavMenuModel NavMenuModelObj, FormCollection collection)
        {
            try
            {
                NavMenu NavMenuObj = new NavMenu();
                string RadioButton = Request.Form["opt"];
                if (RadioButton == "ChildWise")
                {
                    //ADDED BY NANDINI ON 02 MAY 2020
                    NavMenu NavObj = WetosDB.NavMenus.Where(a => a.navmenuId == NavMenuModelObj.MenuNameId && a.parId == 0).FirstOrDefault();
                    if (NavObj != null)
                    {
                        NavMenuObj.parId = NavObj.navmenuId;
                    }

                    //if (NavMenuModelObj.MenuNameId == 1)
                    //{
                    //    NavMenuObj.parId = NavMenuModelObj.MasterId;
                    //}
                    //if (NavMenuModelObj.MenuNameId == 2)
                    //{
                    //    NavMenuObj.parId = NavMenuModelObj.AttendanceId;
                    //}
                    //if (NavMenuModelObj.MenuNameId == 3)
                    //{
                    //    NavMenuObj.parId = NavMenuModelObj.PayrollId;
                    //}
                    //if (NavMenuModelObj.MenuNameId == 4)
                    //{
                    //    NavMenuObj.parId = NavMenuModelObj.ApplicationId;
                    //}
                    //if (NavMenuModelObj.MenuNameId == 5)
                    //{
                    //    NavMenuObj.parId = NavMenuModelObj.SanctionId;
                    //}
                    //if (NavMenuModelObj.MenuNameId == 59)
                    //{
                    //    NavMenuObj.parId = NavMenuModelObj.ReportsId;
                    //}
                    //if (NavMenuModelObj.MenuNameId == 60)
                    //{
                    //    NavMenuObj.parId = NavMenuModelObj.AdministrationId;
                    //}
                    //if (NavMenuModelObj.MenuNameId == 67)
                    //{
                    //    NavMenuObj.parId = NavMenuModelObj.EmpMgmtId;
                    //}
                    //if (NavMenuModelObj.MenuNameId == 70)
                    //{
                    //    NavMenuObj.parId = NavMenuModelObj.OtherAppId;
                    //}
                    //if (NavMenuModelObj.MenuNameId == 61)
                    //{
                    //    NavMenuObj.parId = NavMenuModelObj.UpdatesId;
                    //}
                    //if (NavMenuModelObj.MenuNameId == 1108)
                    //{
                    //    NavMenuObj.parId = NavMenuModelObj.ChartsId;
                    //}
                    //if (NavMenuModelObj.MenuNameId == 1164)
                    //{
                    //    NavMenuObj.parId = NavMenuModelObj.WetosReportsId;
                    //}

                    NavMenuObj.navmenuname = NavMenuModelObj.NavMenuName;

                    NavMenuObj.description = "";
                    if (NavMenuModelObj.NavLink == "")
                    {
                        NavMenuObj.navlink = "";
                    }
                    else
                    {
                        NavMenuObj.navlink = NavMenuModelObj.NavLink;
                    }
                    NavMenuObj.srno = NavMenuModelObj.SrNo;
                    NavMenuObj.active = "1";

                    DropdownData DDObj = WetosDB.DropdownDatas.Where(a => a.Value == NavMenuModelObj.NavMenuIcon && a.TypeId == 29).FirstOrDefault();
                    if (DDObj != null)
                    {
                        NavMenuObj.Icon = DDObj.Text;
                    }

                    WetosDB.NavMenus.AddObject(NavMenuObj);
                    WetosDB.SaveChanges();
                    Success("Successfully Create Nav Menu Child Name are:  " + NavMenuModelObj.NavMenuName);
                    PopulateDropDown();
                    return RedirectToAction("Index");
                }
                else
                {
                    //ADDED BY NANDINI ON 02 MAY 2020
                    NavMenuObj.parId = 0;
                    NavMenuObj.navmenuname = NavMenuModelObj.NavMenuName;
                    NavMenuObj.description = "";
                    NavMenuObj.navlink = "";
                    NavMenuObj.srno = NavMenuModelObj.SrNo;
                    NavMenuObj.active = "1";
                    DropdownData DDObj = WetosDB.DropdownDatas.Where(a => a.TypeId == NavMenuModelObj.NavMenuIcon && a.TypeId == 29).FirstOrDefault();
                    if (DDObj != null)
                    {
                        NavMenuObj.Icon = DDObj.Text;
                    }
                    WetosDB.NavMenus.AddObject(NavMenuObj);
                    WetosDB.SaveChanges();
                    PopulateDropDown();
                    Success("Successfully Create Nav Menu Parent Name are:  " + NavMenuModelObj.NavMenuName);
                    return RedirectToAction("Create");
                }


            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + ex.InnerException);

                Error(WetosErrorMessageController.GetErrorMessage(6));
                PopulateDropDown();
                return RedirectToAction("Index");
            }

            // Success("Successfully Create Nav Menu Name are:  " + NavMenuModelObj.NavMenuName);

        }

        //ADDED BY NANDINI ON 14 APRIL 2020
        [HttpGet]
        public ActionResult Edit(int id)
        {
            try
            {
                NavMenu NavMenuObj = WetosDB.NavMenus.Where(a => a.navmenuId == id).FirstOrDefault();
                NavMenuModel NMMObj = new NavMenuModel();
                if (NavMenuObj != null)
                {
                    NMMObj.NavMenuId = NavMenuObj.navmenuId;
                    NMMObj.MenuNameId = NavMenuObj.parId;
                    NMMObj.NavMenuName = NavMenuObj.navmenuname;
                    NMMObj.NavLink = NavMenuObj.navlink;
                    NMMObj.SrNo = NavMenuObj.srno;
                    DropdownData DDObj = WetosDB.DropdownDatas.Where(a => a.Text.Trim().ToUpper() == NavMenuObj.Icon.Trim().ToUpper() && a.TypeId == 29).FirstOrDefault();
                    NMMObj.NavMenuIcon = DDObj.Value.Value;


                }
                PopulateDropDown();
                AddAuditTrail("Success - Visited Nav Menu Edit");
                return View(NMMObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + ex.InnerException);

                return RedirectToAction("Index");
            }
        }

        //ADDED BY NANDINI ON 14 APRIL 2020
        [HttpPost]
        public ActionResult Edit(int id, NavMenuModel NavMenuModelObj)
        {
            try
            {
                NavMenu NavMenuObj = WetosDB.NavMenus.Where(a => a.navmenuId == id).FirstOrDefault();
                if (NavMenuObj != null)
                {

                    NavMenuObj.parId = NavMenuModelObj.MenuNameId;
                    NavMenuObj.navmenuname = NavMenuModelObj.NavMenuName;
                    NavMenuObj.description = "";
                    NavMenuObj.navlink = NavMenuModelObj.NavLink;
                    NavMenuObj.srno = NavMenuModelObj.SrNo;
                    NavMenuObj.active = "1";
                    DropdownData DDObj = WetosDB.DropdownDatas.Where(a => a.Value == NavMenuModelObj.NavMenuIcon && a.TypeId == 29).FirstOrDefault();
                    NavMenuObj.Icon = DDObj.Text;
                    WetosDB.SaveChanges();
                }

            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + ex.InnerException);

                Error(WetosErrorMessageController.GetErrorMessage(6));

                return RedirectToAction("Index");
            }
            Success("Successfully Update Nav Menu are:  " + NavMenuModelObj.NavMenuName);
            return RedirectToAction("Index");
        }

        //ADDED BY NANDINI ON 14 APRIL 2020
        [HttpGet]
        public ActionResult Delete(int id)
        {
            try
            {
                NavMenu NavMenuObj = WetosDB.NavMenus.Where(a => a.navmenuId == id).FirstOrDefault();
                NavMenuModel NMMObj = new NavMenuModel();
                if (NavMenuObj != null)
                {
                    NMMObj.NavMenuId = NavMenuObj.navmenuId;
                    NMMObj.MenuNameId = NavMenuObj.parId;
                    NMMObj.NavMenuName = NavMenuObj.navmenuname;
                    NMMObj.NavLink = NavMenuObj.navlink;
                    NMMObj.SrNo = NavMenuObj.srno;
                    DropdownData DDObj = WetosDB.DropdownDatas.Where(a => a.Text.Trim().ToUpper() == NavMenuObj.Icon.Trim().ToUpper() && a.TypeId == 29).FirstOrDefault();
                    NMMObj.NavMenuIcon = DDObj.Value.Value;


                }
                PopulateDropDown();
                AddAuditTrail("Success - Visited Nav Menu Delete");
                return View(NMMObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + ex.InnerException);

                return RedirectToAction("Index");
            }
        }


        //ADDED BY NANDINI ON 14 APRIL 2020
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                NavMenu NavMenuObj = WetosDB.NavMenus.Where(a => a.navmenuId == id).FirstOrDefault();
                if (NavMenuObj != null)
                {
                    WetosDB.NavMenus.DeleteObject(NavMenuObj);
                    WetosDB.SaveChanges();
                }

                Success("Successfully Delete " + NavMenuObj.navmenuname);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }


        //ADDED BY NANDINI ON 14 APRIL 2020
        private void PopulateDropDown()
        {
            try
            {

                var NavMenuIcon = WetosDB.DropdownDatas.Where(a => a.TypeId == 29).Select(a => new { TypeId = a.Value, IconType = a.Text }).ToList();
                ViewBag.NavMenuIconList = new SelectList(NavMenuIcon, "TypeId", "IconType").ToList();

                var NavMenuParent = WetosDB.NavMenus.Where(a => a.parId == 0).Select(a => new { Id = a.navmenuId, Name = a.navmenuname }).ToList();
                ViewBag.NavMenuParentList = new SelectList(NavMenuParent, "Id", "Name").ToList();

                //ADDED BY NANDINI ON 02 MAY 2020 START
                var MasterParent = WetosDB.NavMenus.Where(a => a.parId == 1).Select(a => new { Id = a.navmenuId, Name = a.navmenuname }).ToList();
                ViewBag.MasterParentList = new SelectList(MasterParent, "Id", "Name").ToList();

                var AttendanceList = WetosDB.NavMenus.Where(a => a.parId == 2).Select(a => new { Id = a.navmenuId, Name = a.navmenuname }).ToList();
                ViewBag.AttendanceListVB = new SelectList(AttendanceList, "Id", "Name").ToList();

                var PayrollList = WetosDB.NavMenus.Where(a => a.parId == 3).Select(a => new { Id = a.navmenuId, Name = a.navmenuname }).ToList();
                ViewBag.PayrollListVB = new SelectList(PayrollList, "Id", "Name").ToList();

                var ApplicationList = WetosDB.NavMenus.Where(a => a.parId == 4).Select(a => new { Id = a.navmenuId, Name = a.navmenuname }).ToList();
                ViewBag.ApplicationListVB = new SelectList(ApplicationList, "Id", "Name").ToList();

                var SanctionList = WetosDB.NavMenus.Where(a => a.parId == 5).Select(a => new { Id = a.navmenuId, Name = a.navmenuname }).ToList();
                ViewBag.SanctionListVB = new SelectList(SanctionList, "Id", "Name").ToList();

                var ReportsList = WetosDB.NavMenus.Where(a => a.parId == 59).Select(a => new { Id = a.navmenuId, Name = a.navmenuname }).ToList();
                ViewBag.ReportsListVB = new SelectList(ReportsList, "Id", "Name").ToList();

                var AdministrationList = WetosDB.NavMenus.Where(a => a.parId == 60).Select(a => new { Id = a.navmenuId, Name = a.navmenuname }).ToList();
                ViewBag.AdministrationListVB = new SelectList(AdministrationList, "Id", "Name").ToList();

                var EmployeeManagementList = WetosDB.NavMenus.Where(a => a.parId == 67).Select(a => new { Id = a.navmenuId, Name = a.navmenuname }).ToList();
                ViewBag.EmployeeManagementListVB = new SelectList(EmployeeManagementList, "Id", "Name").ToList();

                var OtherApplicationList = WetosDB.NavMenus.Where(a => a.parId == 70).Select(a => new { Id = a.navmenuId, Name = a.navmenuname }).ToList();
                ViewBag.OtherApplicationListVB = new SelectList(OtherApplicationList, "Id", "Name").ToList();

                var UpdatesList = WetosDB.NavMenus.Where(a => a.parId == 61).Select(a => new { Id = a.navmenuId, Name = a.navmenuname }).ToList();
                ViewBag.UpdatesListVB = new SelectList(UpdatesList, "Id", "Name").ToList();

                var ChartsList = WetosDB.NavMenus.Where(a => a.parId == 1108).Select(a => new { Id = a.navmenuId, Name = a.navmenuname }).ToList();
                ViewBag.ChartsListVB = new SelectList(ChartsList, "Id", "Name").ToList();

                var WetosReportList = WetosDB.NavMenus.Where(a => a.parId == 1164).Select(a => new { Id = a.navmenuId, Name = a.navmenuname }).ToList();
                ViewBag.WetosReportListVB = new SelectList(WetosReportList, "Id", "Name").ToList();

                //ADDED BY NANDINI ON 02 MAY 2020 END

            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + ex.InnerException);


            }
        }

    }
}

