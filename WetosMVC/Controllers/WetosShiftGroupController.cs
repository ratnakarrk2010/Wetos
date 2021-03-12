using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WetosDB;

namespace WetosMVC.Controllers
{
    [SessionExpire] 
    [Authorize]
    public class WetosShiftGroupController : BaseController
    {
        
        //
        // GET: /WetosShiftGroup/

        public ActionResult Index()
        {
            List<VwShiftGroupDetail> ShiftGroups = WetosDB.VwShiftGroupDetails.ToList();

            //ADDED BY RAJAS ON 27 DEC 2016
            AddAuditTrail("");

            return View(ShiftGroups);
        }

        //
        // GET: /WetosShiftGroup/Details/5

        public ActionResult Details(int id)
        {

            VwShiftGroupDetail ShiftGroupDetails = WetosDB.VwShiftGroupDetails.Where(a => a.RotationId == id).FirstOrDefault();

            //ADDED BY RAJAS ON 27 DEC 2016
            AddAuditTrail("");

            return View(ShiftGroupDetails);
        }

        //
        // GET: /WetosShiftGroup/Create

        public ActionResult Create()
        {
            // Drop down for company list

            #region COMPANY LIST CODE ADDED BY SHRADDHA ON 30 MAR 2018
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
            int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            //var CompanyList = WetosDB.Companies.Select(a => new { CompanyId = a.CompanyId, CompanyName = a.CompanyName }).ToList();
            var CompanyName = WetosDB.SP_GetCompanyList(EmployeeId).Select(m => new { Companyid = m.CompanyId, CompanyName = m.CompanyName }).ToList();
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
            #endregion

            ViewBag.CompanyNameList = new SelectList(CompanyName, "CompanyId", "CompanyName").ToList();

            // Drop down for Branch name
            #region CODE FOR GET BRANCH LIST ADDED BY SHRADDHA ON 30 MAR 2018
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
            //var BranchName = WetosDB.Branches.Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList();
            var BranchName = WetosDB.SP_GetBranchList(EmployeeId).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList(); //.Select(m => new { BranchId = m.BranchId, BranchName = m.BranchName}).ToList();
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
            #endregion
            ViewBag.BranchNameList = new SelectList(BranchName, "BranchId", "BranchName").ToList();

            // Drop down shift type
            var ShiftType = WetosDB.Shifts.Select(a => new { ShiftId = a.ShiftId, ShiftName = a.ShiftName }).ToList();
            ViewBag.ShiftTypeList = new SelectList(ShiftType, "ShiftId", "ShiftName").ToList();
            return View();
        }

        //
        // POST: /WetosShiftGroup/Create

        [HttpPost]
        public ActionResult Create(ShiftRotation NewShiftRotation, FormCollection collection)
        {
            try
            {
                WetosDB.ShiftRotations.Add(NewShiftRotation);
                WetosDB.SaveChanges();

                AddAuditTrail("New shift group added");

                return RedirectToAction("Create");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /WetosShiftGroup/Edit/5

        public ActionResult Edit(int id)
        {
            // get existing ShiftId
            ShiftRotation ShiftRotDet = WetosDB.ShiftRotations.Single(b => b.RotationId == id);

            // Drop down for company list
            #region COMPANY LIST CODE ADDED BY SHRADDHA ON 30 MAR 2018
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
            int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            //var CompanyList = WetosDB.Companies.Select(a => new { CompanyId = a.CompanyId, CompanyName = a.CompanyName }).ToList();
            var CompanyName = WetosDB.SP_GetCompanyList(EmployeeId).Select(m => new { Companyid = m.CompanyId, CompanyName = m.CompanyName }).ToList();
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
            #endregion

            ViewBag.CompanyNameList = new SelectList(CompanyName, "CompanyId", "CompanyName").ToList();

            // Drop down for Branch name
            #region CODE FOR GET BRANCH LIST ADDED BY SHRADDHA ON 30 MAR 2018
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
            //var BranchName = WetosDB.Branches.Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList();
            var BranchName = WetosDB.SP_GetBranchList(EmployeeId).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList(); //.Select(m => new { BranchId = m.BranchId, BranchName = m.BranchName}).ToList();
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
            #endregion

            ViewBag.BranchNameList = new SelectList(BranchName, "BranchId", "BranchName").ToList();

            // Drop down shift type
            var ShiftType = WetosDB.Shifts.Select(a => new { ShiftId = a.ShiftId, ShiftName = a.ShiftName }).ToList();
            ViewBag.ShiftTypeList = new SelectList(ShiftType, "ShiftId", "ShiftName").ToList();


            return View(ShiftRotDet);

        }

        //
        // POST: /WetosShiftGroup/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, ShiftRotation ShiftRotDet)
        {
            try
            {
                ShiftRotation ShRot = WetosDB.ShiftRotations.Single(e => e.RotationId == id);

                ShRot.Company.CompanyId = ShiftRotDet.Company.CompanyId;
                ShRot.Branch.BranchId = ShiftRotDet.Branch.BranchId;
                ShRot.RotationCode = ShiftRotDet.RotationCode;
                ShRot.RotationName = ShiftRotDet.RotationName;
                ShRot.RotationOnDayBasis = ShiftRotDet.RotationOnDayBasis;
                ShRot.RotationOnMonthBasis = ShiftRotDet.RotationOnMonthBasis;
                ShRot.RotationOnWeeklyBasis = ShiftRotDet.RotationOnWeeklyBasis;
                ShRot.ShiftsChanges = ShiftRotDet.ShiftsChanges;

                WetosDB.SaveChanges();

                //Added by Rajas on 27 DEC 2016
                AddAuditTrail("Shift group details updated");

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /WetosShiftGroup/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /WetosShiftGroup/Delete/5

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
        /// Json return for to get Branch dropdown list on basis of company selection
        /// Added by Rajas on 27 DEC 2016
        /// </summary>
        /// <returns></returns>
        public JsonResult GetBranch(string Companyid)
        {

            int SelCompanyId = 0;
            if (!string.IsNullOrEmpty(Companyid))
            {
                if (Companyid.ToUpper() != "NULL")
                {
                    SelCompanyId = Convert.ToInt32(Companyid);
                }
            }


            #region CODE FOR GET BRANCH LIST ADDED BY SHRADDHA ON 30 MAR 2018
            int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
            //List<Branch> BranchList = WetosDB.Branches.Where(a => a.Company.CompanyId == SelCompanyId).ToList();
            var BranchList = WetosDB.SP_GetBranchList(EmployeeId).Where(a => a.CompanyId == SelCompanyId).ToList(); //.Select(m => new { BranchId = m.BranchId, BranchName = m.BranchName}).ToList();
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
            #endregion
            return Json(BranchList);
        }
    }
}
