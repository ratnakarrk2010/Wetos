using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WetosDB;
using WetosMVCMainApp.Models;

namespace WetosMVC.Controllers
{
    [SessionExpire] 
    [Authorize]
    public class WetosRuleTransactionController : BaseController
    {
        /// <summary>
        /// Added by Rajas for Rule Transaction Master
        /// </summary>
        
        //
        // GET: /WetosRuleTransaction/

        /// <summary>
        /// List view for RULE ENGINE
        /// Updated by Rajas on 21 APRIL 2017 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            try
            {
                List<RuleEngine> RuleEngineList = WetosDB.RuleEngines.ToList();

                //ADDED BY RAJAS ON 27 DEC 2016
                AddAuditTrail("Rule Engine visited");

                return View(RuleEngineList);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Inconsistent data");

                return View();
            }
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public ActionResult Details(int id)
        //{
        //    try
        //    {
        //        RuleEngine RuleTransDet = WetosDB.RuleEngines.Where(a => a.RuleEngineId == id).FirstOrDefault();

        //        //ADDED BY RAJAS ON 27 DEC 2016
        //        AddAuditTrail("Details checked for Rule Engine");

        //        return View(RuleTransDet);
        //    }
        //    catch (System.Exception ex)
        //    {
        //        AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

        //        Error("Incorrect data. Please try again!");

        //        return RedirectToAction("Index"); // Verify ?
        //    }

        //}


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            try
            {
                PopulateDropdown();

                return View();
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Please try again!");

                return View("Index");  // Verify ?
            }
        }

        /// <summary>
        /// Json return for branch list on selection of company
        /// Added by Rajas on 26 DEC 2016
        /// COMMENTED BY RAJAS, TESTING SHOULD BE DONE (26 DEC 2016)
        /// </summary>
        /// <param name="RuleTrans"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        //[HttpPost]
        //public ActionResult GetBranchByCompanyId(int branchid)
        //{
        //    List<Branch> Branchobj = new List<Branch>();
        //    Branchobj = WetosDB.Branches.Where(a => a.BranchId == branchid).ToList();
        //    SelectList BranchobjList = new SelectList(Branchobj, "BranchId", "BranchName");
        //    return Json(Branchobj);
        //}

        /// <summary>
        /// Json return for to get Rule details on basis of rule selection
        /// Added by Rajas on 27 DEC 2016
        /// </summary>
        /// <returns></returns>
        public JsonResult GetRule(int Ruleid)
        {
            RuleTransaction RuleTransObj = WetosDB.RuleTransactions.Where(a => a.RuleId == Ruleid).FirstOrDefault();
            if (RuleTransObj != null)
            {
                return Json(RuleTransObj);

            }

            else
            {
                return Json(RuleTransObj);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="RuleEngineModelobj"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        /// Updated by Rajas on 28 SEP 2017
        [HttpPost]
        public ActionResult Create(RuleEngineModel RuleEngineModelobj, FormCollection collection)
        {
            try
            {
                if (RuleEngineModelobj.RuleUnit == "Select")
                {
                    Error("Please Select Proper Rule Unit");
                    PopulateDropdown();
                    return View(RuleEngineModelobj);
                }

                //GET MAX OF RULE ID
                RuleEngine RulesEngineObj = WetosDB.RuleEngines.OrderByDescending(a => a.RuleId).FirstOrDefault();

                int MaxOfRuleId = 0;

                int Max = Convert.ToInt32(RulesEngineObj.RuleId);

                MaxOfRuleId = Max + 1;

                RuleEngineModelobj.RuleId = MaxOfRuleId;

                RuleEngineModelobj.HeadCode = "BASIC"; // Tem hard code value

                string UpdateStatus = string.Empty;

                // Added by Rajas on 15 MAY 2017
                bool IsEdit = false;

                if (UpdateRuleEngineData(RuleEngineModelobj, IsEdit, collection, ref UpdateStatus) == true)
                {
                    AddAuditTrail("New rule added " + RuleEngineModelobj.RuleName);

                    Success("New rule added successfully");
                }
                else
                {
                    AddAuditTrail(UpdateStatus);

                    Error(UpdateStatus);
                }

                return RedirectToAction("Index");
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in Rule Engine due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Inconsistent data, Please try again!");

                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Added by Rajas on 21 APRIL 2017
        /// </summary>
        /// <param name="RuleEngineModelobj"></param>
        /// <param name="collection"></param>
        /// <param name="UpdateStatus"></param>
        /// <returns></returns>
        public bool UpdateRuleEngineData(RuleEngineModel RuleEngineModelobj, bool IsEdit, FormCollection collection, ref string UpdateStatus)
        {
            bool ReturnStatus = false;

            try
            {
                // Rule Engine table
                RuleEngine RuleEngineTblObj = WetosDB.RuleEngines.Where(a => a.RuleEngineId == RuleEngineModelobj.RuleEngineId).FirstOrDefault();

                //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 11 SEPTEMBER 2017 FOR EDIT
                RuleEngine RuleEngineTblObjEDIT = WetosDB.RuleEngines.Where(a => a.RuleEngineId == RuleEngineModelobj.RuleEngineId).FirstOrDefault();

                bool IsNew = false;
                if (RuleEngineTblObj == null && IsEdit == false)
                {
                    RuleEngineTblObj = new RuleEngine();
                    IsNew = true;
                }

                if (RuleEngineTblObj == null && IsEdit == true)  // Updated by Rajas on 16 MAY 2017
                {
                    UpdateStatus = "Inconsistent data detected, please try again!";

                    AddAuditTrail("Error in update function " + UpdateStatus);

                    return ReturnStatus;
                }

                // New Leave table object

                RuleEngineTblObj.RuleId = RuleEngineModelobj.RuleId;

                RuleEngineTblObj.Active = RuleEngineModelobj.Active;

                RuleEngineTblObj.Formula = RuleEngineModelobj.Formula;

                RuleEngineTblObj.SubCode = RuleEngineModelobj.SubCode;

                RuleEngineTblObj.HeadCode = RuleEngineModelobj.HeadCode;

                RuleEngineTblObj.RuleType = RuleEngineModelobj.RuleType;

                RuleEngineTblObj.RuleUnit = RuleEngineModelobj.RuleUnit;

                RuleEngineTblObj.RuleName = RuleEngineModelobj.RuleName;  // Added by Rajas on 28 SEP 2017


                // Add new table object 
                if (IsNew)
                {
                    WetosDB.RuleEngines.AddObject(RuleEngineTblObj);
                }

                WetosDB.SaveChanges();


                //-------------------------------------------AuditLog---------------------------------------------------------------------------

                if (IsNew)
                {
                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 11 SEPTEMBER 2017
                    #region ADD AUDIT LOG
                    //OLD RECORD IS BLANK
                    string Newrecord = "Rule ID : " + RuleEngineTblObj.RuleId + ", Active : " + RuleEngineTblObj.Active
                        + ", Formula : " + RuleEngineTblObj.Formula + ", SubCode : " + RuleEngineTblObj.SubCode
                        + ", HeadCode :" + RuleEngineTblObj.HeadCode + ", RuleType : " + RuleEngineTblObj.RuleType
                        + ", RuleUnit : " + RuleEngineTblObj.RuleUnit;


                    //LOGIN ID IS TAKEN FROM SESSION PERSISTER
                    string Formname = "RULE ENGINE MASTER";
                    //ACTION IS UPDATE
                    string Message = " ";

                    WetosAdministrationController.GenerateAuditLogsInsert(WetosDB, 0, Formname, Newrecord, ref Message);
                    #endregion
                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 11 SEPTEMBER 2017
                }
                else
                {
                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 11 SEPTEMBER 2017
                    #region ADD AUDIT LOG
                    string Oldrecord = "Rule ID : " + RuleEngineTblObjEDIT.RuleId + ", Active : " + RuleEngineTblObjEDIT.Active
                        + ", Formula : " + RuleEngineTblObjEDIT.Formula + ", SubCode : " + RuleEngineTblObjEDIT.SubCode
                        + ", HeadCode :" + RuleEngineTblObjEDIT.HeadCode + ", RuleType : " + RuleEngineTblObjEDIT.RuleType
                        + ", RuleUnit : " + RuleEngineTblObjEDIT.RuleUnit;

                    //---x---------x-----------------x------------x----------------x-----------x-------------x------------x--------------x----------x---------x-----
                    string Newrecord = "Rule ID : " + RuleEngineTblObj.RuleId + ", Active : " + RuleEngineTblObj.Active
                        + ", Formula : " + RuleEngineTblObj.Formula + ", SubCode : " + RuleEngineTblObj.SubCode
                        + ", HeadCode :" + RuleEngineTblObj.HeadCode + ", RuleType : " + RuleEngineTblObj.RuleType
                        + ", RuleUnit : " + RuleEngineTblObj.RuleUnit;


                    //LOGIN ID IS TAKEN FROM SESSION PERSISTER
                    string Formname = "RULE ENGINE MASTER";
                    //ACTION IS UPDATE
                    string Message = " ";

                    WetosAdministrationController.GenerateAuditLogsUpdate(WetosDB, 0, Formname, Oldrecord,
                        Newrecord, ref Message);
                    #endregion
                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 11 SEPTEMBER 2017
                }

                //-------------------------------------------AuditLog---------------------------------------------------------------------------


                return ReturnStatus = true;
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                UpdateStatus = "Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message);

                return ReturnStatus;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// Updated by Rajas on 21 APRIL 2017
        public ActionResult Edit(int id)
        {
            try
            {

                RuleEngine RuleEngineObj = WetosDB.RuleEngines.Where(b => b.RuleEngineId == id).FirstOrDefault();

                RuleEngineModel RuleEngineModelObj = new RuleEngineModel();

                RuleEngineModelObj.RuleId = RuleEngineObj.RuleId;

                RuleEngineModelObj.RuleEngineId = RuleEngineObj.RuleEngineId;

                RuleEngineModelObj.Formula = RuleEngineObj.Formula;

                RuleEngineModelObj.HeadCode = RuleEngineObj.HeadCode;

                RuleEngineModelObj.SubCode = RuleEngineObj.SubCode;

                RuleEngineModelObj.RuleName = RuleEngineObj.RuleName; // CODE  ADDED BY SHRADDHA ON 08 FEB 2018

                RuleEngineModelObj.RuleUnit = RuleEngineObj.RuleUnit;

                RuleEngineModelObj.RuleType = RuleEngineObj.RuleType;

                RuleEngineModelObj.Active = RuleEngineObj.Active;

                PopulateDropdown();

                return View(RuleEngineModelObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) + ex.Message);

                Error("Please try again!");

                return RedirectToAction("Index");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="RuleTransEdit"></param>
        /// <returns></returns>
        /// Updated by Rajas on 21 APRIl 2017
        [HttpPost]
        public ActionResult Edit(int id, RuleEngineModel RuleEngineModelobj, FormCollection collection)
        {
            try
            {
                if (RuleEngineModelobj.RuleUnit == "Select")
                {
                    Error("Please Select Proper Rule Unit");
                    PopulateDropdown();
                    return View(RuleEngineModelobj);
                }

                string UpdateStatus = string.Empty;

                // Added by Rajas on 15 MAY 2017
                bool IsEdit = true;

                if (UpdateRuleEngineData(RuleEngineModelobj, IsEdit, collection, ref UpdateStatus) == true)
                {
                    AddAuditTrail("New rule added " + RuleEngineModelobj.RuleName);

                    Success("Rule updated successfully");
                }
                else
                {
                    AddAuditTrail(UpdateStatus);

                    Error(UpdateStatus);
                }

                return RedirectToAction("Index");
            }
            catch (System.Exception ex)
            {
                throw (ex);
            }
        }

        //
        // GET: /WetosRuleTransaction/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /WetosRuleTransaction/Delete/5

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

        public ActionResult PopulateDropdown()
        {
            try
            {
                // Updated by Rajas on 6 MAY 2017 to get data which is not MarkedAsDelete START
                #region COMPANY LIST CODE ADDED BY SHRADDHA ON 30 MAR 2018
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //var CompanyName = WetosDB.Companies.Select(a => new { CompanyId = a.CompanyId, CompanyName = a.CompanyName }).ToList();
                var CompanyObj = WetosDB.SP_GetCompanyList(EmployeeId).Select(m => new { Companyid = m.CompanyId, CompanyName = m.CompanyName }).ToList();
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
                #endregion

                ViewBag.CompanyList = new SelectList(CompanyObj, " CompanyId", "CompanyName").ToList();

                 #region CODE FOR GET BRANCH LIST ADDED BY SHRADDHA ON 30 MAR 2018
               //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
                //var BranchObj = WetosDB.Branches.Where(a => a.MarkedAsDelete == null || a.MarkedAsDelete == 0).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList();
                var BranchObj = WetosDB.SP_GetBranchList(EmployeeId).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList(); //.Select(m => new { BranchId = m.BranchId, BranchName = m.BranchName}).ToList();
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
                #endregion

                ViewBag.BranchList = new SelectList(BranchObj, " BranchId", "BranchName").ToList();


                var RulesObj = WetosDB.RuleEngines.Select(a => new { RuleId = a.RuleId, RuleName = a.RuleName }).ToList();

                ViewBag.RulesMasterList = new SelectList(RulesObj, " RuleId", "RuleName").ToList();

                return View();
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                return View();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// Updated by Rajas on 21 APRIL 2017
        public ActionResult Details(int id)
        {
            try
            {

                RuleEngine RuleEngineObj = WetosDB.RuleEngines.Where(b => b.RuleEngineId == id).FirstOrDefault();

                RuleEngineModel RuleEngineModelObj = new RuleEngineModel();

                RuleEngineModelObj.RuleId = RuleEngineObj.RuleId;

                RuleEngineModelObj.RuleEngineId = RuleEngineObj.RuleEngineId;

                RuleEngineModelObj.Formula = RuleEngineObj.Formula;

                RuleEngineModelObj.HeadCode = RuleEngineObj.HeadCode;

                RuleEngineModelObj.SubCode = RuleEngineObj.SubCode;


                RuleEngineModelObj.RuleUnit = RuleEngineObj.RuleUnit;

                RuleEngineModelObj.RuleType = RuleEngineObj.RuleType;

                RuleEngineModelObj.Active = RuleEngineObj.Active;

                PopulateDropdown();

                return View(RuleEngineModelObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) + ex.Message);

                Error("Please try again!");

                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// CODE ADDED BY SHRADDHA ON 08 FEB 2018
        /// </summary>
        /// <returns></returns>
        public ActionResult ExportData()
        {
            try
            {
                System.Web.UI.WebControls.GridView gv = new System.Web.UI.WebControls.GridView();
                gv.DataSource = WetosDB.RuleEngines.OrderBy(a => a.RuleId).ToList();
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=RuleEngineData.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                System.IO.StringWriter sw = new System.IO.StringWriter();
                System.Web.UI.HtmlTextWriter htw = new System.Web.UI.HtmlTextWriter(sw);
                gv.RenderControl(htw);
                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();

                return RedirectToAction("Index");
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in Export employee due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Export employee data failed.");

                return RedirectToAction("Index");
            }
        }

    }
}
