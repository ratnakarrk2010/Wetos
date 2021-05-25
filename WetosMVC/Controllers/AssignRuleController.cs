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
    public class AssignRuleController : BaseController
    {
        //
        // GET: /RoleNavMenu/
        

        public ActionResult AssignRuleIndex()
        {
            var RoleDef = WetosDB.RoleDefs.Where(a => a.MarkedAsDelete == 0).Select(m => new { Roleid = m.RoleId, RoleName = m.RoleName }).ToList();

            #region COMPANY LIST CODE ADDED BY SHRADDHA ON 30 MAR 2018
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
            int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            //var Company = WetosDB.Companies.Where(a => a.MarkedAsDelete == 0).Select(m => new { Companyid = m.CompanyId, Comapnayname = m.CompanyName }).ToList();
            var Company = WetosDB.SP_GetCompanyList(EmployeeId).Select(m => new { Companyid = m.CompanyId, Comapnayname = m.CompanyName }).ToList();
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
            #endregion

            //List<WetosDB.Company> CompanyList = WetosDB.Companies.Where(a => a.MarkedAsDelete == 0).ToList(); //.Select(m => new { BranchId = m.BranchId, BranchName = m.BranchName}).ToList();
            //if (CompanyList.Count > 1)
            //{
            //    CompanyList.Insert(0, new WetosDB.Company { CompanyName = "Select", CompanyId = 0 });
            //}
            //ViewBag.Company = new SelectList(CompanyList, "CompanyId", "CompanyName");

            ViewBag.RoleDef = new SelectList(RoleDef, "Roleid", "RoleName");
            ViewBag.Company = new SelectList(Company, "Companyid", "Comapnayname");

            AddAuditTrail("Clicked on Assign Rules");

            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Company"></param>
        /// <returns></returns>
        public JsonResult BranchList(int Company)
        {
            #region CODE FOR GET BRANCH LIST ADDED BY SHRADDHA ON 30 MAR 2018
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
            int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            //List<Branch> BranchList = WetosDB.Branches.Where(a => a.Company.CompanyId == Company && a.MarkedAsDelete == 0).ToList();
            List<SP_GetBranchList_Result> BranchList = WetosDB.SP_GetBranchList(EmployeeId).Where(a => a.CompanyId == Company).ToList(); //.Select(m => new { BranchId = m.BranchId, BranchName = m.BranchName}).ToList();

            //BranchList.Insert(0, new Branch { BranchName = "Select", BranchId = 0 });
            BranchList.Insert(0, new SP_GetBranchList_Result { BranchName = "Select", BranchId = 0 });
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
            #endregion

            // MODIFIED BY MSJ ON 09 FEB 2017


            ViewBag.Branch = new SelectList(BranchList, "BranchId", "BranchName");

            return Json(new
            {
                Branch = MvcHelpers.RenderPartialView(this, "Branchlist", BranchList)
            });
        }


        /// <summary>
        /// AADED BY SHRADDHA ON 07 JUNE 2017
        /// </summary>
        /// <param name="Company"></param>
        /// <returns></returns>
        public JsonResult EmployeeGroupList(int CompanyId, int BranchId)
        {

            List<EmployeeGroup> EmployeeGroupList = WetosDB.EmployeeGroups.Where(a => a.Company.CompanyId == CompanyId
                && a.Branch.BranchId == BranchId && a.MarkedAsDelete == 0).ToList(); //.Select(m => new { BranchId = m.BranchId, BranchName = m.BranchName}).ToList();

            // MODIFIED BY MSJ ON 09 FEB 2017
            EmployeeGroupList.Insert(0, new EmployeeGroup { EmployeeGroupName = "Select", EmployeeGroupId = 0 });

            ViewBag.EmployeeGroup = new SelectList(EmployeeGroupList, "EmployeeGroupId", "EmployeeGroupName");

            return Json(new
            {
                EmployeeGroup = MvcHelpers.RenderPartialView(this, "EmployeeGroupList", EmployeeGroupList)
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public JsonResult GetNavMenuData()
        {

            List<RuleEngine> RulesMasterList = WetosDB.RuleEngines.ToList();

            return Json(new
            {
                RuleData = MvcHelpers.RenderPartialView(this, "GetNavMenuData", RulesMasterList)

            });
        }

        public JsonResult GetRulesData(int EmployeeGroupId)
        {
            try
            {
                ViewBag.EmployeeGroupId = EmployeeGroupId;
                List<SP_GetAssignedRuleForSelectedEmployeeGroup_Result> RulesEngineList = WetosDB.SP_GetAssignedRuleForSelectedEmployeeGroup(EmployeeGroupId).OrderBy(a => a.RuleEngineId).ToList();
                return Json(new
                {
                    RuleData = MvcHelpers.RenderPartialView(this, "GetRulesData", RulesEngineList)

                });
            }
            catch (Exception ex)
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
        public JsonResult GetSave(int EmployeeGroupId, string Ruleid, string UncheckRuleid, int Companyid, int Branchid)
        {

            String[] RuleIdArray = Ruleid.Split(',');
            //var us = dbu.UserRoles.Where(urs => urs.roleId == Roleid && urs.y_id == Yearid).ToList();

            //int j = 0;

            String[] UncheckRuleIdArray = UncheckRuleid.Split(',');
            foreach (string UncheckRuleidstring in UncheckRuleIdArray)
            {
                int URule_id = -1;
                try
                {
                    URule_id = Convert.ToInt32(UncheckRuleidstring);

                    /*RuleTransaction URule = WetosDB.RuleTransactions.Where(urs => urs.CompanyId == Companyid && urs.BranchId == Branchid
                        && urs.EmployeeGroupId == EmployeeGroupId && urs.RuleId == URule_id).FirstOrDefault(); */
                    var URule = WetosDB.RuleTransactions.Where(urs => urs.CompanyId == Companyid && urs.BranchId == Branchid
                    && urs.EmployeeGroupId == EmployeeGroupId && urs.RuleId == URule_id).ToList();
                    
                    if (URule.Count() > 0)
                    {
                        foreach (var URuleEntry in URule)
                        {
                            WetosDB.RuleTransactions.Remove(URuleEntry);
                        }
                    //WetosDB.RuleTransactions.Add(URule);
                    WetosDB.SaveChanges();
                    }
                }
                catch (Exception Ex)
                {                   
                }
            }
            foreach (var RuleIdArrayObj in RuleIdArray)
            {
                int RuleIdInRuleTransaction = Convert.ToInt32(RuleIdArrayObj);
                RuleTransaction RT = WetosDB.RuleTransactions.Where(a => a.CompanyId == Companyid && a.BranchId == Branchid
                    && a.EmployeeGroupId == EmployeeGroupId && a.RuleId == RuleIdInRuleTransaction).FirstOrDefault();

                //int RuleIdInRuleTransaction = Convert.ToInt32(RuleIdArrayObj);
                if (RT == null)
                {
                    RT = new RuleTransaction();
                    // ADDED BY MDJ ON 09 FEB 2017 START
                    RT.RuleId = Convert.ToInt32(RuleIdInRuleTransaction); //.userId
                    RT.CompanyId = Companyid;
                    RT.BranchId = Branchid;
                    RT.EmployeeGroupId = EmployeeGroupId;
                    RuleEngine RuleEngineObj = WetosDB.RuleEngines.Where(a => a.RuleId == RuleIdInRuleTransaction).FirstOrDefault();
                    RT.Formula = RuleEngineObj.Formula;
                    RT.HeadCode = "BASIC";
                    RT.Active = "A";
                    WetosDB.RuleTransactions.Add(RT);
                }               
                
                WetosDB.SaveChanges();
            }

            WetosDB.SaveChanges();

            AddAuditTrail("Assigned Rule " + Ruleid + " To Company:" + Companyid + " and Branch:" + Branchid);

            return Json("Sucesses");
        }



        public JsonResult GetCheckDatas(int uid)
        {
            //var us = WetosDB.UserRoles.Where(urs => urs.UserRoleId == uid && urs.y_id == SessionPersister.YearInfo.y_id).ToList();
            var us = WetosDB.RuleEngines.Where(urs => urs.RuleId == uid).ToList();

            return Json("s");
        }


    }
}
