using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WetosDB;
using WetosMVC.Controllers;
using WetosMVCMainApp.Models;

namespace WetosMVC.Controllers
{
    [SessionExpire] 
    [Authorize]
    public class RoleNavMenuController : BaseController
    {
        //
        // GET: /RoleNavMenu/
        

        public ActionResult RoleNavMenu()
        {
            var RoleDef = WetosDB.RoleDefs.Where(a => a.MarkedAsDelete == 0).Select(m => new { Roleid = m.RoleId, RoleName = m.RoleName }).ToList();

            #region COMPANY LIST CODE ADDED BY SHRADDHA ON 30 MAR 2018
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
            int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            //var Company = WetosDB.Companies.Where(a => a.MarkedAsDelete == 0).Select(m => new { Companyid = m.CompanyId, Comapnayname = m.CompanyName }).ToList();
            var Company = WetosDB.SP_GetCompanyList(EmployeeId).Select(m => new { Companyid = m.CompanyId, Comapnayname = m.CompanyName }).ToList();
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
            #endregion

            // Add extra dropdown to filter user list 
            // Added by Rajas on 7 JULY 2017 START

            #region CODE FOR GET BRANCH LIST ADDED BY SHRADDHA ON 30 MAR 2018
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
            //var Branch = WetosDB.Branches.Where(a => a.MarkedAsDelete == 0).Select(a => new { Branchid = a.BranchId, BranchName = a.BranchName }).ToList();

            var Branch = WetosDB.SP_GetBranchList(EmployeeId).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList(); //.Select(m => new { BranchId = m.BranchId, BranchName = m.BranchName}).ToList();

            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
            #endregion

            var Department = WetosDB.Departments.Where(a => a.MarkedAsDelete == 0).Select(a => new { Departmentid = a.DepartmentId, DepartmentName = a.DepartmentCode + " - " + a.DepartmentName }).ToList(); //ADDED DEPARTMENT CODE BY SHRADDHA ON 15 FEB 2018 

            ViewBag.Branch = new SelectList(Branch, "Branchid", "BranchName");
            ViewBag.Department = new SelectList(Department, "Departmentid", "DepartmentName");
            // Added by Rajas on 7 JULY 2017 END

            ViewBag.RoleDef = new SelectList(RoleDef, "Roleid", "RoleName");
            ViewBag.Company = new SelectList(Company, "Companyid", "Comapnayname");

            AddAuditTrail("Clicked on Assign Role To Menu ");

            return View();
        }

        public JsonResult YearList(int Company)
        {
            //var Yearlist = WetosDB.YearLists.Select(m => new { Yearid = m.y_id, yearname = m.period, companyid = m.c_id }).Where(c => c.companyid == Company).ToList();
            //GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == "Current Financial Year").FirstOrDefault();

            //ABOVE LINE COMMENTED AND BELOW LINE IS ADDED BY SHALAKA ON 13TH DEC 2017
            GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.CurrentFinancialYear).FirstOrDefault();

            var Yearlist = WetosDB.FinancialYears.Where(a => a.FinancialName == GlobalSettingObj.SettingValue && a.MarkedAsDelete == 0).Select(m => new { Yearid = m.FinancialYearId, yearname = m.FinancialName, companyid = m.Company.CompanyId }).Where(c => c.companyid == Company).ToList();
            // MODIFIED BY MSJ ON 09 FEB 2017
            ViewBag.Year = new SelectList(Yearlist, "Yearid", "yearname");

            return Json(new
            {
                year = MvcHelpers.RenderPartialView(this, "YearList", Yearlist)
            });
        }

        //public JsonResult GetNavMenuData(int Roleid, int Companyid, int Yearid) --- Commented by Shalaka on 27th DEC 2017 Yearid not required
        public JsonResult GetNavMenuData(int Roleid, int Companyid)
        {
            try
            { 
               List<sp_get_RoleNavmenu_Result> RoleNavMenuList = WetosDB.sp_get_RoleNavmenu(1, Roleid).ToList();

                return Json(new
                 {
                    RoleData = MvcHelpers.RenderPartialView(this, "GetNavMenuData", RoleNavMenuList)
                 });
            }
            catch(Exception ex1)
            {
                return null;
            }
        }

        [HttpPost]
        // public JsonResult GetUsersData(int Roleid, int Companyid, int Yearid) --- Commented by Shalaka on 27th DEC 2017 Yearid not required
        public JsonResult GetUsersData(int Roleid, int Companyid, string BranchId = null)
        {
            try
            {
                int BranchIdInt = 0;
                if (!string.IsNullOrEmpty(BranchId))
                {
                    BranchIdInt = Convert.ToInt32(BranchId);
                }
                List<sp_get_Roleuser_Result> UserRoleList = WetosDB.sp_get_Roleuser(Roleid, 1, Companyid, BranchIdInt).ToList(); //.Users.ToList(); // WetosDB.Users.ToList(); //.sp_get_Roleuser(Roleid, Yearid)\.ToList();

                return Json(new
                {
                    RoleData = MvcHelpers.RenderPartialView(this, "GetUsersData", UserRoleList)
                });
            }
            catch(Exception ex1)
            {
                return null;
            }
        }

        [HttpGet]
        // public JsonResult GetUsersData(int Roleid, int Companyid, int Yearid) --- Commented by Shalaka on 27th DEC 2017 Yearid not required
        public ActionResult GetUsersData_New(int Roleid, int Companyid, string BranchId = null)
        {
            try
            {
                int BranchIdInt = 0;
                if (!string.IsNullOrEmpty(BranchId))
                {
                    BranchIdInt = Convert.ToInt32(BranchId);
                }
                List<sp_get_Roleuser_Result> UserRoleList = WetosDB.sp_get_Roleuser(Roleid, 1, Companyid, BranchIdInt).ToList(); //.Users.ToList(); // WetosDB.Users.ToList(); //.sp_get_Roleuser(Roleid, Yearid)\.ToList();
               
                return PartialView(UserRoleList);
                //return Json(new
                //{
                //    RoleData = MvcHelpers.RenderPartialView(this, "GetUsersData", UserRoleList)
                //});
            }
            catch (Exception ex1)
            {
                return null;
            }
        }

        /// <summary>
        /// Updated by Shraddha on 13 APR 2017
        /// </summary>
        /// <param name="Roleid"></param>
        /// <param name="SubChildId"></param>
        /// <param name="childid"></param>
        /// <param name="parid"></param>
        /// <param name="uid"></param>
        /// <param name="Companyid"></param>
        /// <param name="Yearid"></param>
        /// <returns></returns>

        // Below code Commented by Shalaka on 27th DEC 2017
        // public JsonResult GetSave(int Roleid, string SubChildId, string childid, string parid, string uid, int Companyid, int Yearid) --- Year Id Not Required 

        public JsonResult GetSave(int Roleid, string SubChildId, string childid, string parid, string uid)
        {
            String[] Subchildids = SubChildId.Split(',');
            String[] Childid = childid.Split(',');
            String[] Parid = parid.Split(',');
            var p = Parid.Distinct();
            int i = 0;

            var rolenavmenuss = WetosDB.RoleNavMenus.Where(rv => rv.y_id == 1 && rv.roleId == Roleid).ToList();
            WetosDB.RoleNavMenus.Where(u => u.roleId == Roleid).ToList().ForEach(u => WetosDB.RoleNavMenus.Remove(u));
            WetosDB.SaveChanges();
            foreach (RoleNavMenu rvnm in rolenavmenuss)
            {
                WetosDB.RoleNavMenus.Add(rvnm);
            }
            WetosDB.SaveChanges();

            foreach (var parentid in p)
            {
                RoleNavMenu PARrynm = new RoleNavMenu();


                i = i + 1;
                int Par = Convert.ToInt32(parentid);
                PARrynm.navmenuId = Par;
                PARrynm.roleId = Roleid;
                PARrynm.y_id = 1;
                PARrynm.displayOrder = i;
                WetosDB.RoleNavMenus.Add(PARrynm);
                WetosDB.SaveChanges();


                foreach (var child in Childid)
                {
                    int Ch = Convert.ToInt32(child);
                    RoleNavMenu childrynm = new RoleNavMenu();
                    var chk_role_par_navmenu = WetosDB.RoleNavMenus.Where(a => a.navmenuId == Ch && a.roleId == Roleid).FirstOrDefault();
                    var Parent = WetosDB.NavMenus.Where(m => m.navmenuId == Ch).Select(m => m.parId).FirstOrDefault();

                    if (Par == Parent && chk_role_par_navmenu == null)
                    {
                        i = i + 1;
                        childrynm.navmenuId = Ch;
                        childrynm.roleId = Roleid;
                        childrynm.y_id = 1;
                        childrynm.displayOrder = i;
                        WetosDB.RoleNavMenus.Add(childrynm);
                        WetosDB.SaveChanges();
                    }
                    var have_subchild = WetosDB.NavMenus.Where(t => t.parId == Ch).Select(t => t.navmenuId).ToList();
                    int k = 0;
                    if (have_subchild.Count() != 0)
                    {
                        foreach (var subchild in Subchildids)
                        {
                            try
                            {
                                int Sch = Convert.ToInt32(subchild);
                                if (have_subchild[k] == Sch)
                                {
                                    RoleNavMenu subchildrynm = new RoleNavMenu();
                                    var chk_role_menu = WetosDB.RoleNavMenus.Where(a => a.roleId == Roleid && a.navmenuId == Sch).FirstOrDefault();
                                    var Subpart = WetosDB.NavMenus.Where(a => a.navmenuId == Sch).Select(a => a.parId).FirstOrDefault();

                                    if (Ch == Subpart && chk_role_menu == null)
                                    {
                                        i = i + 1;
                                        subchildrynm.navmenuId = Sch;
                                        subchildrynm.roleId = Roleid;
                                        subchildrynm.y_id = 1;
                                        subchildrynm.displayOrder = i;
                                        WetosDB.RoleNavMenus.Add(subchildrynm);
                                        WetosDB.SaveChanges();
                                    }
                                    k++;
                                }
                            }
                            catch(Exception ex1)
                            {
                            }

                        }
                    }

                }

            }



            String[] Userid = uid.Split(',');
            //var us = dbu.UserRoles.Where(urs => urs.roleId == Roleid && urs.y_id == Yearid).ToList();

            //int j = 0;

            foreach (string userids in Userid)
            {
                int u_id = Convert.ToInt32(userids);
                //CODE ADDED COMPANYID AND BRANCHID BY SHRADDHA ON 22 FEB 2018 START
                var us = WetosDB.UserRoles.Where(urs => urs.EmployeeId == u_id).ToList(); // && urs.y_id == Yearid).ToList(); // MODIFIED BY MSJ ON 09 FEB 2017

                //CODE ADDED COMPANYID AND BRANCHID BY SHRADDHA ON 22 FEB 2018 END
                foreach (UserRole ur in us)
                {
                    WetosDB.UserRoles.Add(ur);
                    WetosDB.SaveChanges();
                }
            }


            foreach (var u in Userid)
            {
                int Userids = Convert.ToInt32(u);

                UserRole ur = WetosDB.UserRoles.Where(a => a.EmployeeId == Userids && a.RoleTypeId == Roleid).FirstOrDefault();
                bool IsNew = false;
                if (ur == null)
                {
                    ur = new UserRole();
                    IsNew = true;
                }


                // ADDED BY MDJ ON 09 FEB 2017 START
                ur.EmployeeId = Convert.ToInt32(u); //.userId
                ur.RoleTypeId = Roleid; //.roleId
                ur.MenuId = "1";
                //CODE ADDED BY SHRADDHA ON 22 FEB 2018 START
                //ur.CompanyId = Companyid;
                //ur.BranchId = BranchId;
                //CODE ADDED BY SHRADDHA ON 22 FEB 2018 END

                //ur.FY.y_id = Yearid;
                if (IsNew == true)
                {
                    WetosDB.UserRoles.Add(ur);
                }
                //CODE ADDED BY SHRADDHA ON 13 APR 2017 TO CHANGE ROLE TYPE ID WHILE ASSIGNING ROLE START
                User UserObj = WetosDB.Users.Where(a => a.EmployeeId == ur.EmployeeId).FirstOrDefault();
                if (UserObj != null)
                {
                    UserObj.RoleTypeId = ur.RoleTypeId;
                    WetosDB.SaveChanges();
                }
                //CODE ADDED BY SHRADDHA ON 13 APR 2017 TO CHANGE ROLE TYPE ID WHILE ASSIGNING ROLE END
            }
            WetosDB.SaveChanges();

            AddAuditTrail("Assigned Role " + Roleid + " To Userids - " + uid);


            return Json("Sucesses");
        }



        public JsonResult GetCheckDatas(int Roleid, int uid)
        {
            //var us = WetosDB.UserRoles.Where(urs => urs.UserRoleId == uid && urs.y_id == SessionPersister.YearInfo.y_id).ToList();
            var us = WetosDB.UserRoles.Where(urs => urs.UserRoleId == uid).ToList();

            if (us.Count() == 0)
            {
                return Json("s");
            }
            else
            {

                return Json("Aleardy Exist Current User For another Role");
            }
        }


        public ActionResult AssignBranchAdmin()
        {
            AssignHRManagerModel AssignHRManagerModelObj = new AssignHRManagerModel();
            PopulateDropdown();
            AssignHRManagerModelObj.RoleId = 6; // ROLEID = 6 - BRANCH ADMIN
            ViewBag.Message = string.Empty;
            AssignHRManagerModelObj.BranchAdminList = WetosDB.SP_BranchAdminList().ToList();

            return View(AssignHRManagerModelObj);
        }

        public void PopulateDropdown()
        {
            var RoleDef = WetosDB.RoleDefs.Where(a => a.MarkedAsDelete == 0 && a.RoleId == 6).Select(m => new { Roleid = m.RoleId, RoleName = m.RoleName }).ToList();
            ViewBag.RoleDef = new SelectList(RoleDef, "Roleid", "RoleName");

            #region COMPANY LIST CODE ADDED BY SHRADDHA ON 30 MAR 2018
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
            int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            //var Company = WetosDB.Companies.Where(a => a.MarkedAsDelete == 0).Select(m => new { Companyid = m.CompanyId, Comapnayname = m.CompanyName }).ToList();
            var Company = WetosDB.SP_GetCompanyList(EmployeeId).Select(m => new { Companyid = m.CompanyId, Comapnayname = m.CompanyName }).ToList();
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
            #endregion
            ViewBag.Company = new SelectList(Company, "Companyid", "Comapnayname");

            #region CODE FOR GET BRANCH LIST ADDED BY SHRADDHA ON 30 MAR 2018
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
            //var Branch = WetosDB.Branches.Where(a => a.MarkedAsDelete == 0).Select(a => new { Branchid = a.BranchId, BranchName = a.BranchName }).ToList();
            var Branch = WetosDB.SP_GetBranchList(EmployeeId).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList(); //.Select(m => new { BranchId = m.BranchId, BranchName = m.BranchName}).ToList();
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
            #endregion


            ViewBag.Branch = new SelectList(Branch, "Branchid", "BranchName");

            var Department = WetosDB.Departments.Where(a => a.MarkedAsDelete == 0).Select(a => new { Departmentid = a.DepartmentId, DepartmentName = a.DepartmentCode + " - " + a.DepartmentName }).ToList(); //ADDED DEPARTMENT CODE BY SHRADDHA ON 15 FEB 2018 
            ViewBag.Department = new SelectList(Department, "Departmentid", "DepartmentName");


            #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
            //var EmployeeObj = WetosDB.VwActiveEmployees.Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList(); //ADDED EMPLOYEE CODE BY SHRADDHA ON 15 FEB 2018 
            var EmployeeObj = WetosDB.SP_VwActiveEmployee(EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
            #endregion

            ViewBag.EmployeeList = new SelectList(EmployeeObj, "EmployeeId", "EmployeeName").ToList();
        }

       [HttpPost]
        public ActionResult EditBranchAdmin(AssignHRManagerModel AssignHRManagerModelObj, FormCollection FC)
        {
            try
            {
                int EmployeeId = AssignHRManagerModelObj.EmployeeId;
                if (ModelState.IsValid)
                {
                    
                    string CompanyIdStr = FC["CompanyId"];
                    string BranchIdStr = FC["BranchId"];
                    if (!string.IsNullOrEmpty(BranchIdStr)&& !string.IsNullOrEmpty(CompanyIdStr))
                    {
                        if (BranchIdStr.ToUpper() != "NULL" && CompanyIdStr.ToUpper() != "NULL")
                        {
                            string[] BranchIdArray = BranchIdStr.Split(',');
                            string[] CompanyIdArray = CompanyIdStr.Split(',');
                            if (BranchIdArray.Count() > 0 && CompanyIdArray.Count() > 0)
                            {
                                foreach (string CompanyIdObj in CompanyIdArray)
                                {
                                    foreach (string BranchIdObj in BranchIdArray)
                                    {
                                        //int EmployeeId = AssignHRManagerModelObj.EmployeeId;
                                        //int RoleId = 6;  // ROLEID = 6 - BRANCH ADMIN

                                        List<UserRole> UserRoleList = WetosDB.UserRoles.Where(a => a.EmployeeId == EmployeeId).ToList();
                                        if (UserRoleList != null)
                                        {
                                            foreach (UserRole UserRoleObjToBeDeleted in UserRoleList)
                                            {
                                                WetosDB.UserRoles.Add(UserRoleObjToBeDeleted);
                                                WetosDB.SaveChanges();
                                            }
                                        }
                                    }
                                }
                                foreach (string CompanyIdObj in CompanyIdArray)
                                {
                                   
                                    int RoleId = 6;  // ROLEID = 6 - BRANCH ADMIN
                                   
                                    int companyid = Convert.ToInt32(CompanyIdObj);
                                    //.roleId
                                    foreach (string BranchIdObj in BranchIdArray)
                                    {
                                        int BranchIdInt = Convert.ToInt32(BranchIdObj);
                                       // int CompanyIdInt = WetosDB.Branches.Where(a => a.BranchId == BranchIdInt).Select(a => a.Company.CompanyId).FirstOrDefault());
                                        UserRole UserRoleObj = WetosDB.UserRoles.Where(a => a.BranchId == BranchIdInt && a.CompanyId == companyid && a.EmployeeId == EmployeeId
                                           && a.RoleTypeId == RoleId).FirstOrDefault();
                                        bool IsNew = false;
                                        if (UserRoleObj == null)
                                        {
                                            IsNew = true;
                                            UserRoleObj = new UserRole();
                                        }
                                        //UserRole ur = new UserRole();

                                        // ADDED BY MDJ ON 09 FEB 2017 START
                                        UserRoleObj.EmployeeId = EmployeeId; //.userId
                                        UserRoleObj.RoleTypeId = RoleId; //.roleId
                                        UserRoleObj.CompanyId = companyid;
                                        UserRoleObj.BranchId = BranchIdInt; //.roleId
                                        UserRoleObj.MenuId = "1";
                                        if (IsNew == true)
                                        {
                                            WetosDB.UserRoles.Add(UserRoleObj);
                                        }
                                        WetosDB.SaveChanges();
                                    }
                                }
                            }
                        }
                    }

                    //CODE ADDED BY SHRADDHA ON 23 FEB 2018 TO CHANGE ROLE TYPE ID WHILE ASSIGNING ROLE START
                    User UserObj = WetosDB.Users.Where(a => a.EmployeeId == AssignHRManagerModelObj.EmployeeId).FirstOrDefault();
                    if (UserObj != null)
                    {
                        UserObj.RoleTypeId = 6;
                        WetosDB.SaveChanges();
                    }
                    //CODE ADDED BY SHRADDHA ON 23 FEB 2018 TO CHANGE ROLE TYPE ID WHILE ASSIGNING ROLE END
                }
                else
                {
                }
                ViewBag.Message = "Assign Branch Admin successful";
                PopulateDropdown();
                var CompanyObj = WetosDB.Companies.Where(a => a.MarkedAsDelete == 0).Select(m => new { CompanyId = m.CompanyId, CompanyName = m.CompanyName }).ToList();
                ViewBag.CompanyList = new SelectList(CompanyObj, "CompanyId", "CompanyName").ToList();
                var BranchObj = WetosDB.Branches.Where(a => a.MarkedAsDelete == 0).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList();
                ViewBag.BranchList = new SelectList(BranchObj, " BranchId", "BranchName").ToList();
                AssignHRManagerModelObj.BranchAdminList = WetosDB.SP_BranchAdminList().ToList();
                return View(AssignHRManagerModelObj);
            }
            catch (System.Exception E)
            {
                ViewBag.Message = "Error Occured in assign Branch Admin" + E.Message + (E.InnerException == null ? string.Empty : E.InnerException.Message);
                PopulateDropdown();
                AssignHRManagerModelObj.BranchAdminList = WetosDB.SP_BranchAdminList().ToList();
                return View(AssignHRManagerModelObj);
            }
        }

        [HttpPost]
        public JsonResult GetBranch(string Companyid)
        {
            try
            {
                List<Branch> BranchList = new List<Branch>();

                if (!string.IsNullOrEmpty(Companyid))
                {
                    if (Companyid.ToUpper() != "NULL")
                    {
                        string[] CompanyIdArray = Companyid.Split(',');
                        if (CompanyIdArray.Count() > 0)
                        {
                            foreach (string CompanyIdObj in CompanyIdArray)
                            {
                                int CompanyIdInt = Convert.ToInt32(CompanyIdObj);
                                List<Branch> BranchListTemp = WetosDB.Branches.Where(a => a.Company.CompanyId == CompanyIdInt && (a.MarkedAsDelete == null || a.MarkedAsDelete == 0)).ToList();
                                if (BranchListTemp != null && BranchListTemp.Count() > 0)
                                {
                                    foreach (Branch BranchObj in BranchListTemp)
                                    {
                                        if (BranchObj != null)
                                        {
                                            BranchList.Add(BranchObj);
                                        }
                                    }
                                }
                            }
                        }
                        //SelCompanyId = Convert.ToInt32(Companyid);
                    }
                }

                // Updated by Rajas on 30 MAY 2017
                var BranchListNew = BranchList.Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList();
                return Json(BranchListNew);
            }
            catch
            {
                List<Branch> BranchList = new List<Branch>();
                return Json(BranchList);
            }

        }


        public ActionResult EditBranchAdmin(int id, string Branch)
        {
            try
            {
                AssignHRManagerModel AssignHRManagerModelObj = new AssignHRManagerModel();
               
              
                ViewBag.Message = string.Empty;
                int BranchIdInt = WetosDB.Branches.Where(a => a.BranchName == Branch).Select(a => a.BranchId).FirstOrDefault();
                int CompanyIdInt = Convert.ToInt32(WetosDB.Branches.Where(a => a.BranchName == Branch).Select(a => a.Company.CompanyId).FirstOrDefault());
                int EmployeeId = id;
                int RoleId = 6;  // ROLEID = 6 - BRANCH ADMIN
                UserRole UserRoleObj = WetosDB.UserRoles.Where(a => a.BranchId == BranchIdInt && a.EmployeeId == EmployeeId && a.RoleTypeId == RoleId).FirstOrDefault();

                var CompanyObj = WetosDB.Companies.Where(a => a.MarkedAsDelete == 0).Select(m => new { CompanyId = m.CompanyId, CompanyName = m.CompanyName }).ToList();
                ViewBag.CompanyList = new SelectList(CompanyObj, "CompanyId", "CompanyName").ToList();

                var BranchObj = WetosDB.Branches.Where(a => a.MarkedAsDelete == 0).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList();
                //WetosDB.SP_GetBranchList(EmployeeId).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList();


                ViewBag.BranchList = new SelectList(BranchObj, " BranchId", "BranchName").ToList();

                AssignHRManagerModelObj.EmployeeId = EmployeeId;
                AssignHRManagerModelObj.CompanyId = Convert.ToString(CompanyIdInt);
                AssignHRManagerModelObj.BranchId = Convert.ToString(BranchIdInt);
                AssignHRManagerModelObj.BranchAdminList = WetosDB.SP_BranchAdminList().ToList();

                return View(AssignHRManagerModelObj);
            }
            catch (Exception ex1)
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// POST
        /// </summary>
        /// <param name="id"></param>
        /// <param name="EditBranchAdmin"></param>
        /// <returns></returns>
        //[HttpPost]
        //public ActionResult EditAssignBranchAdmin(UserRole UserObj, FormCollection FC)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            string CompanyIdStr = FC["CompanyId"];
        //            string BranchIdStr = FC["BranchId"];
        //            var userid = Convert.ToInt32(UserObj.UserRoleId);
        //            var Branch1 = BranchIdStr.Split(',');
        //            //db.Users.Where(u => u.UserId == userid).ToList().ForEach(u => db.Users.Remove(u));
        //            //db.SaveChanges();
        //            UserRole UserRoleObj = WetosDB.UserRoles.Where(a => a.BranchId == UserObj.BranchId && a.EmployeeId == UserObj.EmployeeId
        //                                   && a.RoleTypeId == UserObj.RoleTypeId).FirstOrDefault();
        //            bool IsNew = false;
        //            if (UserRoleObj == null)
        //            {
        //                IsNew = true;
        //                UserRoleObj = new UserRole();
        //            }
        //            foreach (var user in Branch1)
        //            {

        //                //UserRole UserRoleObj = new UserRole();
        //                UserRoleObj.UserId = Convert.ToInt32(UserObj.UserId);
        //                UserRoleObj.EmployeeId = EmployeeId;
        //                UserRoleObj.RoleTypeId = RoleId;
        //                UserRoleObj.CompanyId = CompanyIdInt;
        //                UserRoleObj.BranchId = Convert.ToInt32(Branch1);
        //                UserRoleObj.MenuId = "1";

        //            }
        //            if (IsNew == true)
        //            {
        //                WetosDB.UserRoles.Add(UserRoleObj);
        //            }
        //            WetosDB.SaveChanges();
        //        }
        //        return View();
        //    }

        //    catch (System.Exception ex)
        //    {
        //        return null; ;
        //    }
        //}




        [HttpPost]
        public string DeleteBranchAdmin(string UserRoleId)
        {
            try
            {
                int roleId = Convert.ToInt32(UserRoleId);
                List<UserRole> UserRoleList = WetosDB.UserRoles.Where(a => a.UserRoleId == roleId).ToList();
                if (UserRoleList != null)
                {
                    foreach (UserRole UserRoleObjToBeDeleted in UserRoleList)
                    {
                        WetosDB.UserRoles.Add(UserRoleObjToBeDeleted);
                        WetosDB.SaveChanges();
                    }
                }
                return "success";
            }
            catch (Exception ex)
            {
                return "Error";
            }
        }

    }
}
