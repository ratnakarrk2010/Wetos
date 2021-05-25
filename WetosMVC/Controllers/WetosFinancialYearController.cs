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
    public class WetosFinancialYearController : BaseController
    {

        // GET: /WetosFinancialYearController/

        public ActionResult Index()
        {
            try
            {
                List<FinancialYear> FinancialYearListObj = WetosDB.FinancialYears.Where(a => a.MarkedAsDelete == 0).ToList();

                return View(FinancialYearListObj);
            }

            catch (System.Exception ex)
            {
                AddAuditTrail("Can't redirect to financial year due to " + ex.Message);

                Error("Something seems to be wrong, please try again");

                return RedirectToAction("LeaveCreditIndex");

            }
        }


        // GET: /WetosFinancialYearController/Create
        //ADDED BY NANDINI ON 14 APRIL 2020 START

        [HttpGet]
        public ActionResult Create()
        {
            try
            {
                FinancialYearModel FYM = new FinancialYearModel();
                PopulateDropdown();

                return View(FYM);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Exception due to " + ex.Message);

                Error(WetosErrorMessageController.GetErrorMessage(11));

                return RedirectToAction("Index");
            }
        }

        //
        // POST: /WetosFinancialYearController/Create
        //ADDED BY NANDINI ON  14 APRIL 2020 START

        [HttpPost]
        public ActionResult Create(FinancialYearModel NewFinancialYear, FormCollection fc)
        {
            try
            {
                if (NewFinancialYear.StartDate > NewFinancialYear.EndDate)
                {
                    PopulateDropdown();


                    AddAuditTrail("FY-Create : Invalid End Date in FY");

                    Error(WetosErrorMessageController.GetErrorMessage(14));
                    return View(NewFinancialYear);
                }


                string UpdateStatus = string.Empty;


                bool IsEdit = false;

                if (UpdateFinancialYearData(NewFinancialYear, IsEdit, ref UpdateStatus) == true)
                {

                    AddAuditTrail("New Financial year added successfully");

                    Success("New Financial year " + NewFinancialYear.FinancialName + " Added successfully");
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
                PopulateDropdown();


                AddAuditTrail("New financial year not added due to " + ex.Message + ex.InnerException);

                Error(WetosErrorMessageController.GetErrorMessage(11));

                return RedirectToAction("Create");
            }

        }

        //
        // GET: /WetosFinancialYearController/Edit/5
        //ADDED BY NANDINI ON  14 APRIL 2020 

        [HttpGet]
        public ActionResult Edit(int id)
        {
            try
            {

                WetosDB.FinancialYear FinancialYearEditObj = WetosDB.FinancialYears.Single(b => b.FinancialYearId == id);

                FinancialYearModel NewFinancialYear = new FinancialYearModel();

                NewFinancialYear.FinancialId = FinancialYearEditObj.FinancialYearId;


                NewFinancialYear.FinancialName = FinancialYearEditObj.FinancialName.Trim();

                NewFinancialYear.FYStartDate = FinancialYearEditObj.StartDate;

                NewFinancialYear.FYEndDate = FinancialYearEditObj.EndDate;

                NewFinancialYear.CompanyId = FinancialYearEditObj.Company.CompanyId;

                NewFinancialYear.BranchId = FinancialYearEditObj.Branch.BranchId;

                PopulateDropdown();

                return View(NewFinancialYear);
            }

            catch (System.Exception ex)
            {
                PopulateDropdown();

                AddAuditTrail("Exception due to " + ex.Message + ex.InnerException);

                Error(WetosErrorMessageController.GetErrorMessage(10));

                return RedirectToAction("Index");
            }

        }

        //
        // POST: /WetosFinancialYearController/Edit/5
        //ADDED BY NANDINI ON  14 APRIL 2020
        [HttpPost]
        public ActionResult Edit(int id, FinancialYearModel NewFinancialYear)
        {
            try
            {

                if (NewFinancialYear.StartDate > NewFinancialYear.EndDate)
                {
                    PopulateDropdown();


                    AddAuditTrail("FY-Edit: Invalid End Date in FY");

                    Error(WetosErrorMessageController.GetErrorMessage(14));
                    return View(NewFinancialYear);
                }

                string UpdateStatus = string.Empty;


                bool IsEdit = true;


                if (UpdateFinancialYearData(NewFinancialYear, IsEdit, ref UpdateStatus) == true)
                {

                    AddAuditTrail("New financial year updated successfully");

                    Success("New Financial Year " + NewFinancialYear.FinancialName + " Updated Successfully");
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
                #region

                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);

                var CompanyObj = WetosDB.SP_GetCompanyList(EmployeeId).Select(m => new { Companyid = m.CompanyId, Comapnayname = m.CompanyName }).ToList();

                #endregion

                ViewBag.CompanyList = new SelectList(CompanyObj, " CompanyId", "CompanyName").ToList();

                #region

                var BranchObj = WetosDB.SP_GetBranchList(EmployeeId).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList(); //.Select(m => new { BranchId = m.BranchId, BranchName = m.BranchName}).ToList();

                #endregion

                ViewBag.BranchList = new SelectList(BranchObj, " BranchId", "BranchName").ToList();


                AddAuditTrail("New Financial Year not updated due to " + ex.Message + ex.InnerException);

                Error(WetosErrorMessageController.GetErrorMessage(10));

                return View();
            }

        }


        //ADDED BY NANDINI ON  14 APRIL 2020 START
        public void PopulateDropdown()
        {
            var CompanyList = WetosDB.Companies.Where(a => a.MarkedAsDelete == 0).Select(c => new { CompId = c.CompanyId, CompName = c.CompanyName }).Distinct().ToList();
            ViewBag.CompanyListView = new SelectList(CompanyList, "CompId", "CompName").ToList();

            var BranchList = WetosDB.Branches.Where(a => a.MarkedAsDelete == 0).Select(c => new { BranchId = c.BranchId, BranchName = c.BranchName }).Distinct().ToList();
            ViewBag.BranchListView = new SelectList(BranchList, "BranchId", "BranchName").ToList();


        }
        //ADDED BY NANDINI ON  14 APRIL 2020 END




        //ADDED BY NANDINI ON  14 APRIL 2020 START
        private bool UpdateFinancialYearData(FinancialYearModel NewFinancialYear, bool IsEdit, ref string UpdateStatus)
        {
            bool ReturnStatus = false;
            bool IsNew = false;

            try
            {
                DateTime CurrentServerDate = DateTime.Now;
                FinancialYear CurrentFY = WetosDB.FinancialYears.Where(a => a.StartDate <= CurrentServerDate && a.EndDate >= CurrentServerDate && a.MarkedAsDelete == 0).FirstOrDefault();
                string Series = string.Empty;
                if (CurrentFY == null)
                {
                    int MaxFYId = WetosDB.FinancialYears.Max(a => a.FinancialYearId);

                    FinancialYear LastFY = WetosDB.FinancialYears.Where(a => a.FinancialYearId == MaxFYId).FirstOrDefault();

                    int CurrentYear = CurrentServerDate.Year - 2000;
                    int NextYear = CurrentYear + 1;
                    int PrevYear = CurrentYear - 1;

                    if (CurrentServerDate.Month <= 3)
                    {
                        Series = string.Format("{0}-{1}", PrevYear, CurrentYear);
                    }
                    else
                    {
                        Series = string.Format("{0}-{1}", CurrentYear, NextYear);
                    }

                    // ADD NEW FY
                    CurrentFY = new FinancialYear();
                    CurrentFY.Series = Series;
                    CurrentFY.StartDate = LastFY.StartDate.AddYears(1);
                    CurrentFY.EndDate = LastFY.EndDate.AddYears(1);
                    CurrentFY.PrevFYId = MaxFYId;
                    CurrentFY.NextFYId = MaxFYId + 2;
                    CurrentFY.FinancialYearId = MaxFYId + 1;



                    WetosDB.SaveChanges();

                    if (IsNew)
                    {
                        WetosDB.FinancialYears.Add(CurrentFY);
                    }

                    WetosDB.SaveChanges();

                    ReturnStatus = true;


                }
                return ReturnStatus;
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + ex.InnerException);

                UpdateStatus = "Error due to " + ex.Message + ex.InnerException;

                return ReturnStatus;
            }
        }
        //ADDED BY NANDINI ON 30 MARCH 2020 END

        //ADDED BY NANDINI ON 28 APRIL 2020 START
        [HttpGet]
        public ActionResult Delete(int id)
        {
            try
            {

                WetosDB.FinancialYear FinancialYearEditObj = WetosDB.FinancialYears.Single(b => b.FinancialYearId == id);

                FinancialYearModel NewFinancialYear = new FinancialYearModel();

                NewFinancialYear.FinancialId = FinancialYearEditObj.FinancialYearId;


                NewFinancialYear.FinancialName = FinancialYearEditObj.FinancialName.Trim();

                NewFinancialYear.FYStartDate = FinancialYearEditObj.StartDate;

                NewFinancialYear.FYEndDate = FinancialYearEditObj.EndDate;

                NewFinancialYear.CompanyId = FinancialYearEditObj.Company.CompanyId;

                NewFinancialYear.BranchId = FinancialYearEditObj.Branch.BranchId;

                PopulateDropdown();

                return View(NewFinancialYear);
            }

            catch (System.Exception ex)
            {
                PopulateDropdown();

                AddAuditTrail("Exception due to " + ex.Message + ex.InnerException);

                Error(WetosErrorMessageController.GetErrorMessage(10));

                return RedirectToAction("Index");
            }

        }
        //ADDED BY NANDINI ON 28 APRIL 2020 END

        //ADDED BY NANDINI ON 28 APRIL 2020
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                WetosDB.FinancialYear FinancialYearEditObj = WetosDB.FinancialYears.Single(b => b.FinancialYearId == id);


                if (FinancialYearEditObj != null)
                {

                    //WetosDB.PayrollFinancialYears.Add(FinancialYearEditObj);
                    FinancialYearEditObj.MarkedAsDelete = 1;
                    WetosDB.SaveChanges();

                    Success("FinancialYear " + FinancialYearEditObj.Series + " deleted successfully");

                    AddAuditTrail("FinancialYear " + FinancialYearEditObj.Series + " deleted successfully");
                }

                return RedirectToAction("Index");
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + ex.InnerException);

                Error("Please try again!");

                return RedirectToAction("Index");
            }
        }
    }
}
