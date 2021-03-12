using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web.Mvc;
using WetosDB;
using WetosMVCMainApp.Models;

namespace WetosMVC.Controllers
{
    [SessionExpire] 
    [Authorize]
    public class WetosApplicationForOtherController : BaseController
    {
        // GET: /WetosApplication/

        public ActionResult Index()
        {
            //ADDED BY RAJAS ON 27 DEC 2016
            AddAuditTrail("Success - Visited Applications"); // Updated by Rajas on 17 JAN 2017
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="EmployeeId"></param>
        /// <returns></returns>
        public ActionResult AjaxPopulateDropdown(int EmployeeId)
        {

            //var EmployeeCodeObj = WetosDB.Employees.Where(b => b.EmployeeId == EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeCode = a.EmployeeCode }).ToList();
            // var EmployeeNameList = WetosDB.Employees.Where(b => b.EmployeeId == EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeCode = a.EmployeeCode }).ToList();
            // ViewBag.EmployeeCodeList = new SelectList(EmployeeNameList, "EmployeeId", "EmployeeCode").ToList();

            var EmployeeNameList = (from r in WetosDB.Employees

                                    where r.EmployeeId == EmployeeId
                                    //join x in db.Unit_of_Measure on r.UOM_Code equals x.UOM_Code
                                    select new { EmployeeName = r.FirstName + " " + r.MiddleName + " " + r.LastName, EmployeeId = r.EmployeeId }).FirstOrDefault();

            return Json(EmployeeNameList, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// To Populate Dropdown
        /// Added By shraddha on 17 OCT 2016
        /// </summary>
        public void PopulateDropDown()
        {
            #region COMPANY LIST CODE ADDED BY SHRADDHA ON 30 MAR 2018
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
            int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            //var CompanyObj = WetosDB.Companies.Select(a => new { CompanyId = a.CompanyId, CompanyName = a.CompanyName }).ToList();
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

            //Added by shraddha on 14th oct 2016 start

            DateTime Leavingdate = Convert.ToDateTime("01/01/1900");  // Added by Rajas on 9 MARCH 2017

            // Updated by Rajas on 10 MARCH 2017

            #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
            EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            //var EmployeeObj = WetosDB.Employees.Where(a => a.Leavingdate == Leavingdate).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
            var EmployeeObj = WetosDB.SP_VwActiveEmployee(EmployeeId).Where(a => a.Leavingdate == Leavingdate).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
            #endregion

            ViewBag.EmployeeList = new SelectList(EmployeeObj, "EmployeeId", "EmployeeName").ToList();

            //CODE CHANGED BY SHRADDHA ON 07 FEB 2017 TAKEN FINANCIAL YEAR ID ALONG WITH NAME START
            var FinancialYearObj = WetosDB.FinancialYears.Where(a => a.MarkedAsDelete == null || a.MarkedAsDelete == 0).Select(a => new { FinancialId = a.FinancialYearId, FinancialName = a.FinancialName }).ToList();

            ViewBag.FinancialYearList = new SelectList(FinancialYearObj, "FinancialId", "FinancialName").ToList();

            //var FinancialYearObj = WetosDB.FinancialYears.Select(a => a.FinancialName).Distinct().ToList();
            //ViewBag.FinancialYearList = new SelectList(FinancialYearObj, "FinancialName").ToList();

            //CODE CHANGED BY SHRADDHA ON 07 FEB 2017 TAKEN FINANCIAL YEAR ID ALONG WITH NAME END

            //var LeaveCodeObj = WetosDB.LeaveMasters.Where(a => a.MarkedAsDelete == null || a.MarkedAsDelete == 0).Select(a => new { LeaveCode = a.LeaveCode, LeaveName = a.LeaveName }).Distinct().ToList();
            //ViewBag.LeaveCodeList = new SelectList(LeaveCodeObj, " LeaveCode", "LeaveName").ToList();

            //Above Code commented and Added below code on 29th DEC 2017 for getting only allowed encashment leave type-- Start
            var LeaveCodeObj = WetosDB.LeaveMasters.Where(a => a.EncashmentAllowedOrNot == true).Select(a => new { LeaveCode = a.LeaveCode, LeaveName = a.LeaveName }).Distinct().ToList();

            ViewBag.LeaveCodeList = new SelectList(LeaveCodeObj, " LeaveCode", "LeaveName").ToList();
            //Added on 29th DEC 2017 for getting only allowed encashment leave type-- End

            var RequisitionTYpeObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 1).Select(a => new { ODTourId = a.Text, ODTourType = a.Text }).ToList();

            ViewBag.RequisitionTYpeList = new SelectList(RequisitionTYpeObj, "ODTourId", "ODTourType").ToList();



            #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
            EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            //var EmployeeCodeObj = WetosDB.Employees.Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
            var EmployeeCodeObj = WetosDB.SP_VwActiveEmployee(EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
            #endregion

            ViewBag.EmployeeCodeList = new SelectList(EmployeeCodeObj, "EmployeeId", "EmployeeName").ToList();

            // Added by Rajas on 22 OCT 2016 for Dropdown list for FULL DAY or HALF DAY Leave
            var LeaveTypeStatusObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 9).Select(a => new { LeaveStatusID = a.Value, LeaveStatus = a.Text }).ToList();

            ViewBag.LeaveTypeStatusList = new SelectList(LeaveTypeStatusObj, "LeaveStatusID", "LeaveStatus").ToList();

            //Added by Shraddha on 17 DEC 2016 for displaying employee code and name both in dropdown
            // Updated by Rajas on 31 JULY 2017

            #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
            //var EmployeeCodeAndNameObj = WetosDB.Employees.Where(a => a.ActiveFlag == true).Select
            //(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " " + a.FirstName + " " + a.LastName }).ToList();
            var EmployeeCodeAndNameObj = WetosDB.SP_VwActiveEmployee(EmployeeId).Select
            (a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " " + a.FirstName + " " + a.LastName }).ToList();
            #endregion


            ViewBag.EmployeeCodeAndNameList = new SelectList(EmployeeCodeAndNameObj, "EmployeeId", "EmployeeName").ToList();

            //ADDED BY SHRADDHA ON 28 DEC 2016 FOR SELECT CRITERIA START
            List<string> SelectCriteriaObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 5).Select(a => a.Text).ToList();

            ViewBag.SelectCriteriaList = SelectCriteriaObj;
            //ADDED BY SHRADDHA ON 28 DEC 2016 FOR SELECT CRITERIA END

            // Added by Rajas on 7 MARCH 2017
            var LeaveTypeObj = new List<LeaveMaster>(); //WetosDB.LeaveMasters.Select(a => new { LeaveTypeID = a.LeaveId, LeaveType = a.LeaveCode }).ToList();

            ViewBag.LeaveTypeList = new SelectList(LeaveTypeObj, "LeaveTypeID", "LeaveType").ToList();

            // Added by Rajas on 19 MAY 2017
            var ODpurpose = WetosDB.DropdownDatas.Where(a => a.TypeId == 15).Select(a => new { ODPurposeId = a.Text, ODPurposeType = a.Text }).ToList();
            ViewBag.ODPurposeList = new SelectList(ODpurpose, "ODPurposeId", "ODPurposeType").ToList();
        }

        /// <summary>
        /// To Populate Dropdown
        ///Added by Rajas on 7 MARCh 2017
        /// </summary>
        public void PopulateDropDownEdit()
        {
            #region COMPANY LIST CODE ADDED BY SHRADDHA ON 30 MAR 2018
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
            int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            //var CompanyObj = WetosDB.Companies.Select(a => new { CompanyId = a.CompanyId, CompanyName = a.CompanyName }).ToList();
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

            //Added by shraddha on 14th oct 2016 start

            DateTime Leavingdate = Convert.ToDateTime("01/01/1900");  // Added by Rajas on 9 MARCH 2017

            // Updated by Rajas on 10 MARCH 2017


            #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
            EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            //var EmployeeObj = WetosDB.Employees.Where(a => a.Leavingdate == Leavingdate).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
            var EmployeeObj = WetosDB.SP_VwActiveEmployee(EmployeeId).Where(a => a.Leavingdate == Leavingdate).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
            #endregion

            ViewBag.EmployeeList = new SelectList(EmployeeObj, "EmployeeId", "EmployeeName").ToList();

            //CODE CHANGED BY SHRADDHA ON 07 FEB 2017 TAKEN FINANCIAL YEAR ID ALONG WITH NAME START
            var FinancialYearObj = WetosDB.FinancialYears.Where(a => a.MarkedAsDelete == null || a.MarkedAsDelete == 0).Select(a => new { FinancialId = a.FinancialYearId, FinancialName = a.FinancialName }).ToList();

            ViewBag.FinancialYearList = new SelectList(FinancialYearObj, "FinancialId", "FinancialName").ToList();

            //var FinancialYearObj = WetosDB.FinancialYears.Select(a => a.FinancialName).Distinct().ToList();
            //ViewBag.FinancialYearList = new SelectList(FinancialYearObj, "FinancialName").ToList();

            //CODE CHANGED BY SHRADDHA ON 07 FEB 2017 TAKEN FINANCIAL YEAR ID ALONG WITH NAME END

            var LeaveCodeObj = WetosDB.LeaveMasters.Where(a => a.MarkedAsDelete == null || a.MarkedAsDelete == 0).Select(a => new { LeaveCode = a.LeaveCode, LeaveName = a.LeaveName }).Distinct().ToList();

            ViewBag.LeaveCodeList = new SelectList(LeaveCodeObj, " LeaveCode", "LeaveName").ToList();

            var RequisitionTYpeObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 1).Select(a => new { ODTourId = a.Text, ODTourType = a.Text }).ToList();

            ViewBag.RequisitionTYpeList = new SelectList(RequisitionTYpeObj, "ODTourId", "ODTourType").ToList();



            #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
            EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            //var EmployeeCodeObj = WetosDB.Employees.Select(a => new { EmployeeId = a.EmployeeId, EmployeeCode = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
            var EmployeeCodeObj = WetosDB.SP_VwActiveEmployee(EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeCode = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
            #endregion

            ViewBag.EmployeeCodeList = new SelectList(EmployeeCodeObj, "EmployeeId", "EmployeeCode").ToList();

            // Added by Rajas on 22 OCT 2016 for Dropdown list for FULL DAY or HALF DAY Leave
            var LeaveTypeStatusObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 9).Select(a => new { LeaveStatusID = a.Value, LeaveStatus = a.Text }).ToList();

            ViewBag.LeaveTypeStatusList = new SelectList(LeaveTypeStatusObj, "LeaveStatusID", "LeaveStatus").ToList();

            //Added by Shraddha on 17 DEC 2016 for displaying employee code and name both in dropdown

            // Updated by Rajas on 10 MARCH 2017


            #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
            EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            //var EmployeeCodeAndNameObj = WetosDB.Employees.Where(a => a.Leavingdate == Leavingdate).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " " + a.FirstName + " " + a.LastName }).ToList();
            var EmployeeCodeAndNameObj = WetosDB.SP_VwActiveEmployee(EmployeeId).Where(a => a.Leavingdate == Leavingdate).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " " + a.FirstName + " " + a.LastName }).ToList();
            #endregion

            ViewBag.EmployeeCodeAndNameList = new SelectList(EmployeeCodeAndNameObj, "EmployeeId", "EmployeeName").ToList();

            //ADDED BY SHRADDHA ON 28 DEC 2016 FOR SELECT CRITERIA START
            List<string> SelectCriteriaObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 5).Select(a => a.Text).ToList();

            ViewBag.SelectCriteriaList = SelectCriteriaObj;
            //ADDED BY SHRADDHA ON 28 DEC 2016 FOR SELECT CRITERIA END

            // Added by Rajas on 7 MARCH 2017
            var LeaveTypeObj = WetosDB.LeaveMasters.Where(a => a.MarkedAsDelete == null || a.MarkedAsDelete == 0).Select(a => new { LeaveTypeID = a.LeaveId, LeaveType = a.LeaveCode }).ToList();

            ViewBag.LeaveTypeList = new SelectList(LeaveTypeObj, "LeaveTypeID", "LeaveType").ToList();

            // Added by Rajas on 19 MAY 2017
            var ODpurpose = WetosDB.DropdownDatas.Where(a => a.TypeId == 15).Select(a => new { ODPurposeId = a.Text, ODPurposeType = a.Text }).ToList();
            ViewBag.ODPurposeList = new SelectList(ODpurpose, "ODPurposeId", "ODPurposeType").ToList();
        }



        /// <summary>
        /// CompOff application edit 
        /// Added by Rajas on 21 NOV 2016
        /// MODIFIED CODE BY SHRADDHA ON 10 MARCH 2017 FOR CompOffApplicationEdit GET
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult CompOffApplicationEdit(int id)
        {
            try
            {
                WetosDB.CompOffApplication CompOffApplicationEdit = WetosDB.CompOffApplications.Where(a => a.CompOffId == id).FirstOrDefault();

                TimeSpan ts = (Convert.ToDateTime(CompOffApplicationEdit.ToDate)) - (Convert.ToDateTime(CompOffApplicationEdit.FromDate));
                float AppliedDay = ts.Days;
                if (CompOffApplicationEdit.FromDateStatus == 1 && CompOffApplicationEdit.ToDateStatus == 1)
                {
                    AppliedDay = AppliedDay + 1;
                }

                    //Modified By Shraddha on 30 DEC 2016 for leave application validation changes start
                else if ((CompOffApplicationEdit.FromDateStatus == 2) && (CompOffApplicationEdit.ToDateStatus == 2) && CompOffApplicationEdit.FromDate == CompOffApplicationEdit.ToDate)
                {
                    AppliedDay = 0.5F;
                }
                else if ((CompOffApplicationEdit.FromDateStatus == 3) && (CompOffApplicationEdit.ToDateStatus == 3) && CompOffApplicationEdit.FromDate == CompOffApplicationEdit.ToDate)
                {
                    AppliedDay = 0.5F;
                }
                else if ((CompOffApplicationEdit.FromDateStatus == 2) && (CompOffApplicationEdit.ToDateStatus == 3) && CompOffApplicationEdit.FromDate == CompOffApplicationEdit.ToDate)
                {
                    AppliedDay = 1F;
                }

                else if ((CompOffApplicationEdit.FromDateStatus == 2 || CompOffApplicationEdit.FromDateStatus == 3) && (CompOffApplicationEdit.ToDateStatus == 2 || CompOffApplicationEdit.ToDateStatus == 3) && CompOffApplicationEdit.FromDate != CompOffApplicationEdit.ToDate)
                {
                    AppliedDay = AppliedDay + 0;
                }

                else if (CompOffApplicationEdit.FromDateStatus == 2 || CompOffApplicationEdit.ToDateStatus == 2 || CompOffApplicationEdit.FromDateStatus == 3 || CompOffApplicationEdit.ToDateStatus == 3)
                {
                    AppliedDay = (float)(AppliedDay + 0.5);
                }

                Session["AppliedDay"] = AppliedDay;


                PopulateDropDown();



                #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //var EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == CompOffApplicationEdit.EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
                var EmployeeObj = WetosDB.SP_VwActiveEmployee(EmployeeId).Where(a => a.EmployeeId == CompOffApplicationEdit.EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
                #endregion

                ViewBag.EmployeeList = new SelectList(EmployeeObj, "EmployeeId", "EmployeeName").ToList();

                return View(CompOffApplicationEdit);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                return View("Error");
            }
        }



        /// <summary>
        /// MODIFIED CODE BY SHRADDHA ON 10 MARCH 2017 FOR CompOffApplicationEdit POST
        /// </summary>
        /// <param name="id"></param>
        /// <param name="CompOffApplicationEdit"></param>
        /// <param name="Collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CompOffApplicationEdit(int id, WetosDB.CompOffApplication CompOffApplicationEdit, FormCollection Collection)
        {
            try
            {
                List<CompOff> CompOffList = WetosDB.CompOffs.Where(a => a.CompOffApplicationID == CompOffApplicationEdit.CompOffId).ToList();

                int EmpId = Convert.ToInt32(Session["EmployeeNo"]);

                //Added By Shraddha on 12 DEC 2016 for adding date validation start
                if (CompOffApplicationEdit.FromDate > CompOffApplicationEdit.ToDate)
                {
                    ModelState.AddModelError("", "End Date should be Greater than or equal to From Date");
                    PopulateDropDown();



                    #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                    int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                    //var EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == CompOffApplicationEdit.EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
                    var EmployeeObj = WetosDB.SP_VwActiveEmployee(EmployeeId).Where(a => a.EmployeeId == CompOffApplicationEdit.EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
                    #endregion

                    ViewBag.EmployeeList = new SelectList(EmployeeObj, "EmployeeId", "EmployeeName").ToList();
                    return View(CompOffApplicationEdit);
                }

                //Added By Shraddha on 12 DEC 2016 for adding date validation End

                //Modified By Shraddha on 12 DEC 2016 Added if(ModelState.IsValid) Condition instead of try catch block
                if (ModelState.IsValid)
                {
                    #region VALIDATE OD DATA
                    // Validate leave already applied between date range selected
                    // Updated by Rajas on 9 JULY 2017 START
                    // Fixed Issue No. 6 in Flagship Defect Sheet raised by Sandeep sir
                    List<LeaveApplication> LeaveApplicationListObj = WetosDB.LeaveApplications.Where(a => a.EmployeeId == CompOffApplicationEdit.EmployeeId && (a.StatusId == 1 || a.StatusId == 2 || a.StatusId == 4)
                        && (a.MarkedAsDelete == 0 || a.MarkedAsDelete == null)).ToList();

                    // Added by Rajas on 10 MARCH 2017 for validation to check if already pending or sanctioned CO is applied for same date
                    List<CompOffApplication> CompoffApplicationListObj = WetosDB.CompOffApplications.Where(a => a.EmployeeId == CompOffApplicationEdit.EmployeeId && (a.StatusId == 1 || a.StatusId == 2 || a.StatusId == 4)
                        && (a.MarkedAsDelete == 0 || a.MarkedAsDelete == null) && a.CompOffId != CompOffApplicationEdit.CompOffId).ToList();

                    // // Added by Rajas on 10 MARCH 2017 for validation to check if already pending or sanctioned OD is applied for same date
                    List<ODTour> ODTourListObj = WetosDB.ODTours.Where(a => a.EmployeeId == CompOffApplicationEdit.EmployeeId && (a.StatusId == 1 || a.StatusId == 2 || a.StatusId == 4) && (a.MarkedAsDelete == 0 || a.MarkedAsDelete == null)).ToList();
                    // Updated by Rajas on 9 JULY 2017 END

                    for (DateTime CurrentDate = Convert.ToDateTime(CompOffApplicationEdit.FromDate); CurrentDate.Date <= CompOffApplicationEdit.ToDate; CurrentDate = CurrentDate.AddDays(1))
                    {
                        #region LEAVE ALREADY APPLIED BETWEEN SELECTED DATE
                        if (LeaveApplicationListObj.Count > 0)
                        {
                            foreach (var i in LeaveApplicationListObj)
                            {
                                for (DateTime LeaveCurentDate = i.FromDate; LeaveCurentDate.Date <= i.ToDate; LeaveCurentDate = LeaveCurentDate.AddDays(1))
                                {
                                    if (LeaveCurentDate == CurrentDate)
                                    {
                                        ModelState.AddModelError("", "You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                        PopulateDropDown();



                                        #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                                        int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                                        //var EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == CompOffApplicationEdit.EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
                                        var EmployeeObj = WetosDB.SP_VwActiveEmployee(EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
                                        #endregion

                                        ViewBag.EmployeeList = new SelectList(EmployeeObj, "EmployeeId", "EmployeeName").ToList();

                                        return View(CompOffApplicationEdit);
                                    }

                                }
                            }
                        }
                        #endregion

                        #region VALIDATE COMPOFF
                        // Added by Rajas on 10 MARCH 2017 for validating is compoff already present for same date range as of leave START
                        if (CompoffApplicationListObj.Count > 0)
                        {
                            foreach (var i in CompoffApplicationListObj)
                            {
                                for (DateTime LeaveCurentDate = Convert.ToDateTime(i.FromDate); LeaveCurentDate.Date <= i.ToDate; LeaveCurentDate = LeaveCurentDate.AddDays(1))
                                {
                                    if (LeaveCurentDate == CurrentDate)
                                    {
                                        ModelState.AddModelError("", "You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned CompOff for this date range.");

                                        PopulateDropDown();



                                        #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                                        int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                                        //var EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == CompOffApplicationEdit.EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
                                        var EmployeeObj = WetosDB.SP_VwActiveEmployee(EmployeeId).Where(a => a.EmployeeId == CompOffApplicationEdit.EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
                                        #endregion

                                        ViewBag.EmployeeList = new SelectList(EmployeeObj, "EmployeeId", "EmployeeName").ToList();

                                        return View(CompOffApplicationEdit);
                                    }

                                }
                            }
                        }
                        // Added by Rajas on 10 MARCH 2017 for validating is compoff already present for same date range as of leave END
                        #endregion

                        #region VALIDATE OD
                        // Added by Rajas on 10 MARCH 2017 for validating is OD already present for same date range as of leave START
                        if (ODTourListObj.Count > 0)
                        {
                            foreach (var i in ODTourListObj)
                            {
                                for (DateTime LeaveCurentDate = Convert.ToDateTime(i.FromDate); LeaveCurentDate.Date <= i.ToDate; LeaveCurentDate = LeaveCurentDate.AddDays(1))
                                {
                                    if (LeaveCurentDate == CurrentDate)
                                    {
                                        ModelState.AddModelError("", "You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned OD for this date range.");
                                        PopulateDropDown();



                                        #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                                        int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                                        //var EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == CompOffApplicationEdit.EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
                                        var EmployeeObj = WetosDB.SP_VwActiveEmployee(EmployeeId).Where(a => a.EmployeeId == CompOffApplicationEdit.EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
                                        #endregion

                                        ViewBag.EmployeeList = new SelectList(EmployeeObj, "EmployeeId", "EmployeeName").ToList();

                                        // LeaveApplicationNecessaryContent(EmpId);

                                        return View(CompOffApplicationEdit);
                                    }

                                }
                            }
                        }
                        // Added by Rajas on 10 MARCH 2017 for validating is compoff already present for same date range as of leave END
                        #endregion
                    }

                    #region VALIDATE FOR LOCKED DATA
                    // Added by Rajas on 10 MARCH 2017 to Validate if data is locked, can not apply leave START
                    DailyTransaction DailyTransactionListObj = WetosDB.DailyTransactions.Where(a => a.TranDate >= CompOffApplicationEdit.FromDate && a.TranDate <= CompOffApplicationEdit.ToDate && a.EmployeeId == CompOffApplicationEdit.EmployeeId && a.Lock == "Y").FirstOrDefault();

                    if (DailyTransactionListObj != null)
                    {
                        ModelState.AddModelError("", "You Can not apply Comp Off for this range as Data is Locked");

                        PopulateDropDown();



                        #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                        int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                        //var EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == CompOffApplicationEdit.EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
                        var EmployeeObj = WetosDB.SP_VwActiveEmployee(EmployeeId).Where(a => a.EmployeeId == CompOffApplicationEdit.EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
                        #endregion

                        ViewBag.EmployeeList = new SelectList(EmployeeObj, "EmployeeId", "EmployeeName").ToList();

                        return View(CompOffApplicationEdit);
                    }
                    // Added by Rajas on 10 MARCH 2017 to Validate if data is locked, can not apply leave END
                    #endregion

                    #endregion
                    #region CompOff Allowed Days Logic

                    double AllowedDays = Convert.ToDouble(Collection["AppliedDays"]);

                    TimeSpan ts = (Convert.ToDateTime(CompOffApplicationEdit.ToDate)) - (Convert.ToDateTime(CompOffApplicationEdit.FromDate));
                    float AppliedDay = ts.Days;
                    if (CompOffApplicationEdit.FromDateStatus == 1 && CompOffApplicationEdit.ToDateStatus == 1)
                    {
                        AppliedDay = AppliedDay + 1;
                    }

                        //Modified By Shraddha on 30 DEC 2016 for leave application validation changes start
                    else if ((CompOffApplicationEdit.FromDateStatus == 2) && (CompOffApplicationEdit.ToDateStatus == 2) && CompOffApplicationEdit.FromDate == CompOffApplicationEdit.ToDate)
                    {
                        AppliedDay = 0.5F;
                    }
                    else if ((CompOffApplicationEdit.FromDateStatus == 3) && (CompOffApplicationEdit.ToDateStatus == 3) && CompOffApplicationEdit.FromDate == CompOffApplicationEdit.ToDate)
                    {
                        AppliedDay = 0.5F;
                    }
                    else if ((CompOffApplicationEdit.FromDateStatus == 2) && (CompOffApplicationEdit.ToDateStatus == 3) && CompOffApplicationEdit.FromDate == CompOffApplicationEdit.ToDate)
                    {
                        AppliedDay = 1F;
                    }

                    else if ((CompOffApplicationEdit.FromDateStatus == 2 || CompOffApplicationEdit.FromDateStatus == 3) && (CompOffApplicationEdit.ToDateStatus == 2 || CompOffApplicationEdit.ToDateStatus == 3) && CompOffApplicationEdit.FromDate != CompOffApplicationEdit.ToDate)
                    {
                        AppliedDay = AppliedDay + 0;
                    }

                    else if (CompOffApplicationEdit.FromDateStatus == 2 || CompOffApplicationEdit.ToDateStatus == 2 || CompOffApplicationEdit.FromDateStatus == 3 || CompOffApplicationEdit.ToDateStatus == 3)
                    {
                        AppliedDay = (float)(AppliedDay + 0.5);
                    }

                    if (AppliedDay > AllowedDays)
                    {

                        ModelState.AddModelError("", "You Can't Apply Comp Off for more than allowed days");
                        CompOffApplicationNecessaryContent(Convert.ToInt32(CompOffApplicationEdit.EmployeeId));
                        PopulateDropDown();



                        #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                        int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                        //var EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == CompOffApplicationEdit.EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
                        var EmployeeObj = WetosDB.SP_VwActiveEmployee(EmployeeId).Where(a => a.EmployeeId == CompOffApplicationEdit.EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
                        #endregion

                        ViewBag.EmployeeList = new SelectList(EmployeeObj, "EmployeeId", "EmployeeName").ToList();
                        //List<CompOffApplication> CompOffApplicationList = WetosDB.CompOffApplications.Where(a => a.EmployeeId == EmployeeNo).ToList();
                        return View(CompOffApplicationEdit);

                    }

                    if (AppliedDay < AllowedDays)
                    {

                        ModelState.AddModelError("", "You can not apply comp off for less than Total Allowed CompOff Days");
                        CompOffApplicationNecessaryContent(Convert.ToInt32(CompOffApplicationEdit.EmployeeId));
                        PopulateDropDown();



                        #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                        int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                        //var EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == CompOffApplicationEdit.EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " " + a.FirstName + " " + a.LastName }).ToList();
                        var EmployeeObj = WetosDB.SP_VwActiveEmployee(EmployeeId).Where(a => a.EmployeeId == CompOffApplicationEdit.EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
                        #endregion

                        ViewBag.EmployeeList = new SelectList(EmployeeObj, "EmployeeId", "EmployeeName").ToList();
                        //List<CompOffApplication> CompOffApplicationList = WetosDB.CompOffApplications.Where(a => a.EmployeeId == EmployeeNo).ToList();
                        return View(CompOffApplicationEdit);

                    }
                    #endregion
                    else
                    {

                        WetosDB.CompOffApplication CompOffApplicationObj = WetosDB.CompOffApplications.Where(b => b.CompOffId == id).FirstOrDefault();

                        //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 11 SEPTEMBER 2017 FOR EDIT
                        WetosDB.CompOffApplication CompOffApplicationObjEDIT = WetosDB.CompOffApplications.Where(b => b.CompOffId == id).FirstOrDefault();


                        Employee EmployeeDetailsObj = WetosDB.Employees.Where(a => a.EmployeeId == CompOffApplicationEdit.EmployeeId).FirstOrDefault();
                        CompOffApplicationObj.FromDate = CompOffApplicationEdit.FromDate;
                        CompOffApplicationObj.FromDateStatus = CompOffApplicationEdit.FromDateStatus;
                        CompOffApplicationObj.ToDate = CompOffApplicationEdit.ToDate;
                        CompOffApplicationObj.ToDateStatus = CompOffApplicationEdit.ToDateStatus;
                        CompOffApplicationObj.EmployeeId = CompOffApplicationEdit.EmployeeId;
                        CompOffApplicationObj.CompanyId = EmployeeDetailsObj.CompanyId;
                        CompOffApplicationObj.BranchId = EmployeeDetailsObj.BranchId;
                        CompOffApplicationObj.Purpose = CompOffApplicationEdit.Purpose;
                        //Added By Shraddha on 12 DEC 2016 for taking Status along with statusId Start
                        CompOffApplicationObj.Status = WetosDB.DropdownDatas.Where(a => a.TypeId == 3 && a.Value == CompOffApplicationEdit.StatusId).Select(a => a.Text).FirstOrDefault();
                        //Added By Shraddha on 12 DEC 2016 for taking Status along with statusId End

                        WetosDB.SaveChanges();

                        //ADDED CODE BY SHRADDHA ON 11 AUG 2017 START
                        if (CompOffList != null)
                        {
                            foreach (CompOff CompOffObj in CompOffList)
                            {
                                CompOffObj.CoDate = CompOffApplicationEdit.FromDate;
                                CompOffObj.CompOffApplicationID = CompOffApplicationObj.CompOffId;
                                WetosDB.SaveChanges();
                            }
                        }
                        //ADDED CODE BY SHRADDHA ON 11 AUG 2017 END

                        //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 11 SEPTEMBER 2017
                        #region ADD AUDIT LOG
                        string Oldrecord = "Employee ID : " + EmpId
                                            + "FromDate :" + CompOffApplicationObjEDIT.FromDate + "To Date :" + CompOffApplicationObjEDIT.ToDate
                                            + "FromDateStatus :" + CompOffApplicationObjEDIT.FromDateStatus + "ToDateStatus :" + CompOffApplicationObjEDIT.ToDateStatus
                                            + "Purpose :" + CompOffApplicationObjEDIT.Purpose + " Status : " + CompOffApplicationObjEDIT.Status + "Reject Reason :"
                                            + CompOffApplicationObjEDIT.RejectReason + "ExtraHoursDate :" + CompOffApplicationObjEDIT.ExtrasHoursDate + "Branch ID :"
                                            + CompOffApplicationObjEDIT.BranchId + "Company ID:" + CompOffApplicationObjEDIT.CompanyId;
                        //---x---------x-----------------x------------x----------------x-----------x-------------x------------x--------------x----------x---------x-----

                        string Newrecord = "Employee ID : " + EmpId
                                            + "FromDate :" + CompOffApplicationObj.FromDate + "To Date :" + CompOffApplicationObj.ToDate
                                            + "FromDateStatus :" + CompOffApplicationObj.FromDateStatus + "ToDateStatus :" + CompOffApplicationObj.ToDateStatus
                                            + "Purpose :" + CompOffApplicationObj.Purpose + " Status : " + CompOffApplicationObj.Status + "Reject Reason :"
                                            + CompOffApplicationObj.RejectReason + "ExtraHoursDate :" + CompOffApplicationObj.ExtrasHoursDate + "Branch ID :"
                                            + CompOffApplicationObj.BranchId + "Company ID:" + CompOffApplicationObj.CompanyId;

                        //LOGIN ID IS TAKEN FROM SESSION PERSISTER
                        string Formname = "CUMULATIVE COMPOFF APPLICATION";
                        //ACTION IS UPDATE
                        string Message = " ";

                        WetosAdministrationController.GenerateAuditLogsUpdate(WetosDB, EmployeeDetailsObj.EmployeeId, Formname, Oldrecord,
                            Newrecord, ref Message);
                        #endregion
                        //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 11 SEPTEMBER 2017


                        #region COMPOFF EDIT NOTIFICATION

                        // FOR NOTIFICATION TO BE SENT TO REPORTING PERSON

                        Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == CompOffApplicationEdit.EmployeeId).FirstOrDefault();
                        Notification NotificationObj = new Notification();
                        NotificationObj.FromID = EmployeeObj.EmployeeId;
                        NotificationObj.ToID = EmployeeObj.EmployeeReportingId;
                        NotificationObj.SendDate = DateTime.Now;
                        NotificationObj.NotificationContent = "Applied CompOff edited by " + EmployeeObj.FirstName + " " + EmployeeObj.LastName + " , is pending for approval";
                        NotificationObj.ReadFlag = false;
                        NotificationObj.SendDate = DateTime.Now;

                        WetosDB.Notifications.Add(NotificationObj);

                        WetosDB.SaveChanges();

                        //FOR SELF NOTIFICATION

                        Notification NotificationObj2 = new Notification();
                        NotificationObj2.FromID = EmployeeObj.EmployeeId;
                        NotificationObj2.ToID = EmployeeObj.EmployeeId;
                        NotificationObj2.SendDate = DateTime.Now;
                        NotificationObj2.NotificationContent = "Applied CompOff edited successfully";
                        NotificationObj2.ReadFlag = false;
                        NotificationObj2.SendDate = DateTime.Now;

                        WetosDB.Notifications.Add(NotificationObj2);

                        WetosDB.SaveChanges();

                        // NOTIFICATION COUNT
                        int NoTiCount = WetosDB.Notifications.Where(a => a.ToID == EmpId && (a.ReadFlag == false || a.ReadFlag == null)).Count();
                        Session["NotificationCount"] = NoTiCount;

                        #endregion

                        // Code updated by Rajas on 13 JUNE 2017
                        #region EMAIL
                        // Added by Rajas on 19 JULY 2017 START
                        //GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == "Send Email").FirstOrDefault();

                        //ABOVE LINE COMMENTED AND BELOW LINE IS ADDED BY SHALAKA ON 13TH DEC 2017
                        GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.SendEmail).FirstOrDefault();

                        if (GlobalSettingObj != null)
                        {
                            // Send email ON
                            if (GlobalSettingObj.SettingValue == "1")
                            {

                                // Added by Rajas on 30 MARCH 2017 START
                                string EmailUpdateStatus = string.Empty;

                                if (EmployeeObj.Email != null)
                                {
                                    // Updated by Rajas on 19 JULY 2017 extra added parameter EmailFromWhichApplication
                                    if (SendEmail(EmployeeObj.Email, NotificationObj.NotificationContent, NotificationObj.NotificationContent, ref EmailUpdateStatus, "CompOff Application") == false)
                                    {
                                        Error(EmailUpdateStatus);

                                    }
                                }
                                else
                                {
                                    //Information("Email Id is not Provided; Please Provide Email Id to your Admin");
                                    AddAuditTrail("Unable to send email, as Email Id not available for " + EmployeeObj.EmployeeCode);
                                }
                                // Added by Rajas on 30 MARCH 2017 END
                            }
                            else // Send email OFF
                            {
                                AddAuditTrail("Unable to send Email as email utility is disabled");
                            }
                        }
                        // Added by Rajas on 19 JULY 2017 END
                        #endregion

                        // ADDED BY PUSHKAR FOR AuditLog ON 21 DEC 2016
                        AddAuditTrail("Success - CompOff application edited successfully");  // Updated by Rajas on 17 JAN 2017

                        // Added by Rajas on 17 JAN 2017 START
                        Success("Success - CompOff application edited successfully");

                        return RedirectToAction("CompOffApplicationIndex");
                    }
                }
                else
                {
                    //Modified By Shraddha on 12 DEC 2016 Added Populate Dropdown and CompOffApplicationEdit object pass start
                    PopulateDropDown();

                    //Modified By Shraddha on 12 DEC 2016 Added Populate Dropdown and CompOffApplicationEdit object pass End

                    // Added by Rajas on 17 JAN 2017 START
                    AddAuditTrail("Error - CompOff application edit failed");  // Updated by Rajas on 17 JAN 2017

                    Error("Error - CompOff application edit failed");
                    // Added by Rajas on 17 JAN 2017 END

                    return View(CompOffApplicationEdit);
                }
            }

            catch (System.Exception ex)
            {
                PopulateDropDown();

                // Added by Rajas on 17 JAN 2017 START
                AddAuditTrail("Error - CompOff application edit failed due to " + ex.Message);  // Updated by Rajas on 17 JAN 2017

                Error("Error - CompOff application edit failed due to " + ex.Message);
                // Added by Rajas on 17 JAN 2017 END

                return View(CompOffApplicationEdit);

            }
        }


        /// <summary>
        /// FUNCTION ADDED BY SHRADDHA ON 10 MARCH 2017
        /// </summary>
        /// <returns></returns>
        public bool CompOffApplicationNecessaryContent(int EmpId)
        {
            bool ReturnStatus = false;
            //int EmpId = Convert.ToInt32(EmployeeId);
            int EmployeeGroupId = WetosDB.EmployeeGroupDetails.Where(a => a.Employee.EmployeeId == EmpId).Select(a => a.EmployeeGroup.EmployeeGroupId).FirstOrDefault();

            string EmployeeGroupName = WetosDB.EmployeeGroups.Where(a => a.EmployeeGroupId == EmployeeGroupId).Select(a => a.EmployeeGroupName).FirstOrDefault();

            Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == EmpId).FirstOrDefault();
            List<RuleTransaction> RuleTransactionList = WetosDB.RuleTransactions.Where(a => a.EmployeeGroupId == EmployeeGroupId && a.CompanyId == EmployeeObj.CompanyId && a.BranchId == EmployeeObj.BranchId).ToList();

            RuleTransaction RuleTransactionObj = RuleTransactionList.Where(a => a.RuleId == 9).FirstOrDefault();
            RuleTransaction RTObjForCompOffFullDayLimitValue = RuleTransactionList.Where(a => a.RuleId == 10).FirstOrDefault();
            RuleTransaction RTObjForCompOffHalfDayLimitValue = RuleTransactionList.Where(a => a.RuleId == 11).FirstOrDefault();
            //ADDED NEW LOGIC BY SHRADDHA ON 09 MAR 2017 END

            //ADDED NEW LOGIC BY SHRADDHA ON 10 JAN 2017
            double ActualStatusDoubleforfullday = 0;
            double ActualStatusDoubleforhalfday = 0;

            //CODE ADDED BY SHRADDHA ON 09 MARCH 2017
            DateTime NewDateForFullDay = new DateTime();
            DateTime NewDateForHalfDay = new DateTime();

            List<CompOff> AttendanceStatusList = new List<CompOff>();
            if (RuleTransactionObj != null)
            {
                if (RuleTransactionObj.Formula.ToUpper().Trim() == "TRUE")
                {

                    if (RTObjForCompOffFullDayLimitValue != null)
                    {
                        string[] FullDayRuleSplitValue = RTObjForCompOffFullDayLimitValue.Formula.Split(':');
                        int FullDayRuleHourInt = Convert.ToInt32(FullDayRuleSplitValue[0]);
                        int FullDayRuleMinuteInt = Convert.ToInt32(FullDayRuleSplitValue[1]);
                        int FullDayRuleSecondInt = Convert.ToInt32(FullDayRuleSplitValue[2]);

                        int CurrentYearInt = DateTime.Now.Year; // ADDED BY MSJ ON 02 JAN 2018
                        NewDateForFullDay = new DateTime(CurrentYearInt, 01, 01, FullDayRuleHourInt, FullDayRuleMinuteInt, FullDayRuleSecondInt);


                    }
                    if (RTObjForCompOffHalfDayLimitValue != null)
                    {
                        string[] HalfDayRuleSplitValue = RTObjForCompOffHalfDayLimitValue.Formula.Split(':');
                        int HalfDayRuleHourInt = Convert.ToInt32(HalfDayRuleSplitValue[0]);
                        int HalfDayRuleMinuteInt = Convert.ToInt32(HalfDayRuleSplitValue[1]);
                        int HalfDayRuleSecondInt = Convert.ToInt32(HalfDayRuleSplitValue[2]);

                        int CurrentYearInt = DateTime.Now.Year; // ADDED BY MSJ ON 02 JAN 2018
                        NewDateForHalfDay = new DateTime(CurrentYearInt, 01, 01, HalfDayRuleHourInt, HalfDayRuleMinuteInt, HalfDayRuleSecondInt);
                    }

                    //AttendanceStatusList = WetosDB.CompOffs.Where(a => a.EmployeeId == EmpId && a.AppStatus != "S" && a.CoHours.Value.TimeOfDay >= NewDateForHalfDay.TimeOfDay).ToList();
                    AttendanceStatusList = WetosDB.CompOffs.Where(a => a.EmployeeId == EmpId && (a.CoHours.Value.Hour * 60 + a.CoHours.Value.Minute) >= (NewDateForHalfDay.Hour * 60 + NewDateForHalfDay.Minute)).ToList();

                    //CODE ADDED BY SHRADDHA ON 20 SEP 2017 FOR CO VALIDITY DAYS START
                    ViewBag.COValidityDays = 365;
                    //GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == "CompOff Validity Days").FirstOrDefault();

                    //ABOVE LINE COMMENTED AND BELOW LINE IS ADDED BY SHALAKA ON 13TH DEC 2017
                    GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.CompOffValidityDays).FirstOrDefault();

                    if (GlobalSettingObj != null)
                    {
                        ViewBag.COValidityDays = GlobalSettingObj.SettingValue;
                    }
                    //CODE ADDED BY SHRADDHA ON 20 SEP 2017 FOR CO VALIDITY DAYS END

                    ReturnStatus = true;
                }

                else
                {
                    Error("Comp Off is not Allowed for Employee Group : " + EmployeeGroupName);
                    ReturnStatus = false;
                    return ReturnStatus;

                }
            }
            else
            {
                Error("Please set rules related to Comp Off for Employee Group : " + EmployeeGroupName);

                ReturnStatus = false;
                return ReturnStatus;

            }


            //CODE UNCOMMENTED BY SHRADDHA ON 09 MARCH 2017 START

            if (AttendanceStatusList != null && AttendanceStatusList.Count > 0)  //modified by pushkar for wrong exception shown on create and to display count 
            {
                double actualstatusdoubleobj = 0;
                foreach (var compoffobj in AttendanceStatusList)
                {
                    if (compoffobj.CoHours != null)
                    {
                        if (compoffobj.CoHours.Value.TimeOfDay >= NewDateForHalfDay.TimeOfDay && (compoffobj.CoHours.Value.Hour * 60 + compoffobj.CoHours.Value.Minute) < (NewDateForFullDay.Hour * 60 + NewDateForFullDay.Minute))
                        {
                            ActualStatusDoubleforhalfday = 0.5;
                            actualstatusdoubleobj = actualstatusdoubleobj + ActualStatusDoubleforhalfday;
                        }
                        else if (compoffobj.CoHours.Value.TimeOfDay >= NewDateForFullDay.TimeOfDay)
                        {
                            ActualStatusDoubleforfullday = 1;
                            actualstatusdoubleobj = ActualStatusDoubleforfullday + actualstatusdoubleobj;
                        }

                    }


                    ViewBag.AllowedCompOffObjVB = 0;
                    ViewBag.AttendanceStatusListVB = AttendanceStatusList;
                    ViewBag.AttendanceStatusCountVB = AttendanceStatusList.Count();
                }
            }
            else
            {
                ViewBag.AllowedCompOffObjVB = 0;
                ViewBag.AttendanceStatusListVB = 0;
                ViewBag.AttendanceStatusCountVB = 0;

            }
            ReturnStatus = true;
            return ReturnStatus;
        }



        /// <summary>
        /// Comp Off application index
        /// Added by Rajas on 17 NOV 2016
        /// Updated by Rajas on 7 MARCH 2017
        /// </summary>
        /// <returns></returns>
        public ActionResult CompOffApplicationIndex()
        {
            try
            {
                // Added by Rajas on 17 MARCH 2017 START

                // Get current FY from global setting
                //GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => Current Financial Year).FirstOrDefault();

                //ABOVE LINE COMMENTED AND BELOW LINE IS ADDED BY SHALAKA ON 13TH DEC 2017
                GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.CurrentFinancialYear).FirstOrDefault();


                if (GlobalSettingObj == null)
                {
                    Error("Inconsistent Financial year data");
                }

                DateTime CalanderStartDate = GetFYStartDate(); // DateTime CalanderStartDate = WetosDB.FinancialYears.Where(a => a.FinancialName == GlobalSettingObj.SettingValue).Select(a => a.StartDate).FirstOrDefault();

                // DateTime CalanderStartDate = new DateTime(2016, 01, 01); 
                #region SP ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                List<SP_Vw_CompOffApplicationIndex_Result> CompoffApplicationListObj = new List<SP_Vw_CompOffApplicationIndex_Result>();  // Verify ?
                //List<Vw_CompOffApplicationIndex> CompoffApplicationListObj = new List<Vw_CompOffApplicationIndex>();  // Verify ?
                #endregion
                if (CalanderStartDate != null)
                {
                    // Updated by Rajas on 1 APRIL 2017 for MarkedAsDelete
                    #region SP ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                    //CompoffApplicationListObj = WetosDB.Vw_CompOffApplicationIndex.Where(a => a.FromDate >= CalanderStartDate
                    //    && (a.MarkedAsDelete == 0 || a.MarkedAsDelete == null)).ToList();
                    CompoffApplicationListObj = WetosDB.SP_Vw_CompOffApplicationIndex(EmployeeId).Where(a => a.FromDate >= CalanderStartDate
                        && (a.MarkedAsDelete == 0 || a.MarkedAsDelete == null)).ToList();
                    #endregion
                }
                // Added by Rajas on 17 MARCH 2017 END

                return View(CompoffApplicationListObj);
            }

            catch (System.Exception ex)
            {
                AddAuditTrail("Error in Compoff list view due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Error in viewing list for Compoff");

                return View("Error");
            }
        }


        /// <summary>
        /// CompOffApplicationDelete
        /// Updated by Rajas on 9 JUNE 2017
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult CompOffApplicationDelete(int CompOffApplicationId)
        {
            try
            {
                CompOffApplication COMPOFFAPPLICATIONObj = WetosDB.CompOffApplications.Where(a => a.CompOffId == CompOffApplicationId).FirstOrDefault();


                List<CompOff> CompOffList = WetosDB.CompOffs.Where(a => a.CompOffApplicationID == CompOffApplicationId).ToList();
                if (COMPOFFAPPLICATIONObj != null)
                {
                    COMPOFFAPPLICATIONObj.MarkedAsDelete = 1;


                    if (CompOffList.Count > 0)
                    {
                        foreach (CompOff CompOffObj in CompOffList)
                        {
                            CompOffObj.AppStatus = null;
                            CompOffObj.CoDate = null; // ADDED BY SHRADDHA ON 11 AUG 2017
                            CompOffObj.CompOffApplicationID = null;
                            WetosDB.SaveChanges();
                        }
                    }

                    // ADDED BY MSJ ON 09 JAN 2019 START
                    COMPOFFAPPLICATIONObj.CancelledBy = Convert.ToInt32(Session["EmployeeNo"]);
                    COMPOFFAPPLICATIONObj.CancelledOn = DateTime.Now;
                    // ADDED BY MSJ ON 09 JAN 2019 END

                    WetosDB.SaveChanges();


                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 12 SEPTEMBER 2017
                    #region ADD AUDIT LOG
                    //OLD RECORD IS BLANK
                    string Oldrecord = "Employee ID : " + COMPOFFAPPLICATIONObj.EmployeeId
                        + "FromDate :" + COMPOFFAPPLICATIONObj.FromDate + "To Date :" + COMPOFFAPPLICATIONObj.ToDate
                        + "FromDateStatus :" + COMPOFFAPPLICATIONObj.FromDateStatus + "ToDateStatus :" + COMPOFFAPPLICATIONObj.ToDateStatus
                        + "Purpose :" + COMPOFFAPPLICATIONObj.Purpose + " Status : " + COMPOFFAPPLICATIONObj.Status + "Reject Reason :"
                        + COMPOFFAPPLICATIONObj.RejectReason + "ExtraHoursDate :" + COMPOFFAPPLICATIONObj.ExtrasHoursDate + "Branch ID :"
                        + COMPOFFAPPLICATIONObj.BranchId + "Company ID:" + COMPOFFAPPLICATIONObj.CompanyId;

                    //LOGIN ID IS TAKEN FROM SESSION PERSISTER
                    string Formname = "CUMULATIVE COMPOFF DELETE";
                    //ACTION IS UPDATE
                    string Message = " ";
                    int EMP = Convert.ToInt32(COMPOFFAPPLICATIONObj.EmployeeId);

                    WetosAdministrationController.GenerateAuditLogsDelete(WetosDB, EMP, Formname, Oldrecord, ref Message);
                    #endregion
                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 12 SEPTEMBER 2017


                    Success("CO applied between  " + COMPOFFAPPLICATIONObj.FromDate.Value.ToString("dd-MMM-yyyy")
                        + " to " + COMPOFFAPPLICATIONObj.ToDate.Value.ToString("dd-MMM-yyyy") + " deleted successfully");

                    AddAuditTrail("CO applied between  " + COMPOFFAPPLICATIONObj.FromDate.Value.ToString("dd-MMM-yyyy")
                        + " to " + COMPOFFAPPLICATIONObj.ToDate.Value.ToString("dd-MMM-yyyy") + " deleted successfully");
                }

                return RedirectToAction("CompOffApplicationIndex");
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Please try again!");

                return RedirectToAction("CompOffApplicationIndex");
            }
        }


        /// <summary>
        /// CompOffApplicationIndex POST
        /// Added by Rajas on 1 APRIL 2017
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CompOffApplicationIndex(int id)
        {
            try
            {
                CompOffApplication COMPOFFAPPLICATIONObj = WetosDB.CompOffApplications.Where(a => a.CompOffId == id).FirstOrDefault();

                if (COMPOFFAPPLICATIONObj != null)
                {
                    COMPOFFAPPLICATIONObj.MarkedAsDelete = 1;

                    WetosDB.SaveChanges();

                    Success("CO applied between  " + COMPOFFAPPLICATIONObj.FromDate.Value.ToString("dd-MMM-yyyy")
                        + " to " + COMPOFFAPPLICATIONObj.ToDate.Value.ToString("dd-MMM-yyyy") + " deleted successfully");

                    AddAuditTrail("CO applied between  " + COMPOFFAPPLICATIONObj.FromDate.Value.ToString("dd-MMM-yyyy")
                        + " to " + COMPOFFAPPLICATIONObj.ToDate.Value.ToString("dd-MMM-yyyy") + " deleted successfully");
                }

                return RedirectToAction("CompOffApplicationIndex");
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Please try again!");

                return RedirectToAction("CompOffApplicationIndex");
            }
        }

        /// <summary>
        /// MODIFIED CODE BY SHRADDHA ON 10 MARCH 2017 FOR CompOffApplication GET
        /// Try catch block added by Rajas on 22 FEB 2017
        /// </summary>
        /// <returns></returns>
        public ActionResult CompOffApplication(int EmployeeId)
        {
            try
            {


                COMPOffApplicationModel ComOffApp = new COMPOffApplicationModel();
                if (EmployeeId == 0)
                {
                    ViewBag.AllowedCompOffObjVB = 0;
                    ViewBag.AttendanceStatusListVB = 0;
                    ViewBag.AttendanceStatusCountVB = 0;
                }
                else
                {
                    CompOffApplicationNecessaryContent(EmployeeId);
                }
                PopulateDropDown();
                //List<CompOffApplication> CompOffApplicationList = WetosDB.CompOffApplications.Where(a => a.EmployeeId == EmployeeNo).ToList();
                return View(ComOffApp);
            }

            catch (System.Exception ex)
            {
                AddAuditTrail("Can not apply CompOff");
                Error("Can not apply CompOff");

                return RedirectToAction("CompOffApplicationIndex");
            }
        }

        /// <summary>
        /// Added by MOUSAMI ON 14 SEPT 2016
        /// MODIFIED CODE BY SHRADDHA ON 09 MARCH 2017 FOR CompOffApplication POST 
        /// <param name="ODTourObj"></param>
        /// <param name="Collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CompOffApplication(COMPOffApplicationModel CompOffModelObj, FormCollection Collection)
        {
            #region COMPOFF CODE MODIFIED BY SHRADDHA
            int EmpId = Convert.ToInt32(Session["EmployeeNo"]);
            try
            {
                int CheckCompOffId = 0;
                string CheckStatus = Collection["CheckStatus"];
                if (CheckStatus != null)
                {
                    List<string> CheckStatusDataList = CheckStatus.Split(',').ToList();

                    List<int> CheckCompOffIdList = new List<int>();
                    int FalseCount = CheckStatusDataList.Where(a => a.Contains("false")).Count();

                    if (FalseCount == CheckStatusDataList.Count())
                    {
                        ModelState.AddModelError("", "Please Select atleast one CheckBox to apply CompOff");

                        CompOffApplicationNecessaryContent(CompOffModelObj.EmployeeId);

                        PopulateDropDown();

                        return View(CompOffModelObj);
                    }
                    else
                    {

                        foreach (string CheckStatusDataObj in CheckStatusDataList)
                        {
                            if (CheckStatusDataObj == "false")
                            {

                            }
                            else
                            {
                                CheckCompOffId = Convert.ToInt32(CheckStatusDataObj);
                                CheckCompOffIdList.Add(CheckCompOffId);

                            }
                        }

                    }


                    double TotalAllowedCompOff = Convert.ToDouble(Collection["TtlAllwdDys"]);
                    //Added By Shraddha on 12 DEC 2016 for adding date validation start
                    if (Convert.ToDateTime(CompOffModelObj.FromDate) > Convert.ToDateTime(CompOffModelObj.ToDate))
                    {
                        ModelState.AddModelError("", "End Date should be Greater than or equal to From Date");
                        CompOffApplicationNecessaryContent(CompOffModelObj.EmployeeId);
                        PopulateDropDown();
                        //List<CompOffApplication> CompOffApplicationList = WetosDB.CompOffApplications.Where(a => a.EmployeeId == EmployeeNo).ToList();
                        return View(CompOffModelObj);
                    }
                    //Added By Shraddha on 12 DEC 2016 for adding date validation End

                    if (ModelState.IsValid)
                    {
                        #region VALIDATE OD DATA
                        // Validate leave already applied between date range selected
                        // Updated by Rajas on 9 JULY 2017 START
                        // Fixed Issue No. 6 in Flagship Defect Sheet raised by Sandeep sir
                        List<LeaveApplication> LeaveApplicationListObj = WetosDB.LeaveApplications.Where(a => a.EmployeeId == CompOffModelObj.EmployeeId && (a.StatusId == 1 || a.StatusId == 2 || a.StatusId == 4)
                            && a.MarkedAsDelete == 0).ToList();

                        // Added by Rajas on 10 MARCH 2017 for validation to check if already pending or sanctioned CO is applied for same date
                        List<CompOffApplication> CompoffApplicationListObj = WetosDB.CompOffApplications.Where(a => a.EmployeeId == CompOffModelObj.EmployeeId && (a.StatusId == 1 || a.StatusId == 2 || a.StatusId == 4)
                            && a.MarkedAsDelete == 0).ToList();

                        // // Added by Rajas on 10 MARCH 2017 for validation to check if already pending or sanctioned OD is applied for same date
                        List<ODTour> ODTourListObj = WetosDB.ODTours.Where(a => a.EmployeeId == CompOffModelObj.EmployeeId && (a.StatusId == 1 || a.StatusId == 2 || a.StatusId == 4) && a.MarkedAsDelete == 0).ToList();
                        // Updated by Rajas on 9 JULY 2017 END

                        for (DateTime CurrentDate = Convert.ToDateTime(CompOffModelObj.FromDate); CurrentDate.Date <= CompOffModelObj.ToDate; CurrentDate = CurrentDate.AddDays(1))
                        {
                            #region LEAVE ALREADY APPLIED BETWEEN SELECTED DATE
                            if (LeaveApplicationListObj.Count > 0)
                            {
                                foreach (var i in LeaveApplicationListObj)
                                {
                                    for (DateTime LeaveCurentDate = i.FromDate; LeaveCurentDate.Date <= i.ToDate; LeaveCurentDate = LeaveCurentDate.AddDays(1))
                                    {
                                        if (LeaveCurentDate == CurrentDate)
                                        {
                                            Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                            PopulateDropDown();
                                            CompOffApplicationNecessaryContent(CompOffModelObj.EmployeeId);
                                            return View(CompOffModelObj);
                                        }

                                    }
                                }
                            }
                            #endregion

                            #region VALIDATE COMPOFF
                            // Added by Rajas on 10 MARCH 2017 for validating is compoff already present for same date range as of leave START
                            if (CompoffApplicationListObj.Count > 0)
                            {
                                foreach (var i in CompoffApplicationListObj)
                                {
                                    for (DateTime LeaveCurentDate = Convert.ToDateTime(i.FromDate); LeaveCurentDate.Date <= i.ToDate; LeaveCurentDate = LeaveCurentDate.AddDays(1))
                                    {
                                        if (LeaveCurentDate == CurrentDate)
                                        {
                                            Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned CompOff for this date range.");

                                            PopulateDropDown();
                                            CompOffApplicationNecessaryContent(CompOffModelObj.EmployeeId);
                                            return View(CompOffModelObj);
                                        }

                                    }
                                }
                            }
                            // Added by Rajas on 10 MARCH 2017 for validating is compoff already present for same date range as of leave END
                            #endregion

                            #region VALIDATE OD
                            // Added by Rajas on 10 MARCH 2017 for validating is OD already present for same date range as of leave START
                            if (ODTourListObj.Count > 0)
                            {
                                foreach (var i in ODTourListObj)
                                {
                                    for (DateTime LeaveCurentDate = Convert.ToDateTime(i.FromDate); LeaveCurentDate.Date <= i.ToDate; LeaveCurentDate = LeaveCurentDate.AddDays(1))
                                    {
                                        if (LeaveCurentDate == CurrentDate)
                                        {
                                            Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned OD for this date range.");
                                            PopulateDropDown();

                                            // LeaveApplicationNecessaryContent(EmpId);
                                            CompOffApplicationNecessaryContent(CompOffModelObj.EmployeeId);
                                            return View(CompOffModelObj);
                                        }

                                    }
                                }
                            }
                            // Added by Rajas on 10 MARCH 2017 for validating is compoff already present for same date range as of leave END
                            #endregion
                        }

                        #region VALIDATE FOR LOCKED DATA
                        // Added by Rajas on 10 MARCH 2017 to Validate if data is locked, can not apply leave START
                        DailyTransaction DailyTransactionListObj = WetosDB.DailyTransactions.Where(a => a.TranDate >= CompOffModelObj.FromDate && a.TranDate <= CompOffModelObj.ToDate && a.EmployeeId == CompOffModelObj.EmployeeId && a.Lock == "Y").FirstOrDefault();

                        if (DailyTransactionListObj != null)
                        {
                            Error("You Can not apply Comp Off for this range as Data is Locked");

                            PopulateDropDown();
                            CompOffApplicationNecessaryContent(CompOffModelObj.EmployeeId);
                            return View(CompOffModelObj);
                        }
                        // Added by Rajas on 10 MARCH 2017 to Validate if data is locked, can not apply leave END
                        #endregion

                        #endregion


                        //Added by Shraddha on 7 JAN 2017 START
                        TimeSpan ts = (Convert.ToDateTime(CompOffModelObj.ToDate)) - (Convert.ToDateTime(CompOffModelObj.FromDate));
                        float AppliedDay = ts.Days;
                        if (CompOffModelObj.FromDateStatus == 1 && CompOffModelObj.ToDateStatus == 1)
                        {
                            AppliedDay = AppliedDay + 1;
                        }

                            //Modified By Shraddha on 30 DEC 2016 for leave application validation changes start
                        else if ((CompOffModelObj.FromDateStatus == 2) && (CompOffModelObj.ToDateStatus == 2) && CompOffModelObj.FromDate == CompOffModelObj.ToDate)
                        {
                            AppliedDay = 0.5F;
                        }
                        else if ((CompOffModelObj.FromDateStatus == 3) && (CompOffModelObj.ToDateStatus == 3) && CompOffModelObj.FromDate == CompOffModelObj.ToDate)
                        {
                            AppliedDay = 0.5F;
                        }
                        else if ((CompOffModelObj.FromDateStatus == 2) && (CompOffModelObj.ToDateStatus == 3) && CompOffModelObj.FromDate == CompOffModelObj.ToDate)
                        {
                            AppliedDay = 1F;
                        }
                        else if ((CompOffModelObj.FromDateStatus == 3) && (CompOffModelObj.ToDateStatus == 2) && CompOffModelObj.FromDate == CompOffModelObj.ToDate)
                        {
                            ModelState.AddModelError("", "Please Select Proper from day Status and Today Status");
                            //PopulateDropDown();

                            CompOffApplicationNecessaryContent(CompOffModelObj.EmployeeId);
                            PopulateDropDown();
                            //List<CompOffApplication> CompOffApplicationList = WetosDB.CompOffApplications.Where(a => a.EmployeeId == EmployeeNo).ToList();
                            return View(CompOffModelObj);
                        }

                            //Modified By Shraddha on 30 DEC 2016 for leave application validation changes End
                        else if ((CompOffModelObj.FromDateStatus == 2 || CompOffModelObj.FromDateStatus == 3) && (CompOffModelObj.ToDateStatus == 2 || CompOffModelObj.ToDateStatus == 3) && CompOffModelObj.FromDate != CompOffModelObj.ToDate)
                        {
                            AppliedDay = AppliedDay + 0;
                        }

                        else if (CompOffModelObj.FromDateStatus == 2 || CompOffModelObj.ToDateStatus == 2 || CompOffModelObj.FromDateStatus == 3 || CompOffModelObj.ToDateStatus == 3)
                        {
                            AppliedDay = (float)(AppliedDay + 0.5);
                        }
                        if (AppliedDay > TotalAllowedCompOff)
                        {

                            ModelState.AddModelError("", "You Can't Apply Comp Off for more than allowed days");
                            CompOffApplicationNecessaryContent(CompOffModelObj.EmployeeId);
                            PopulateDropDown();
                            //List<CompOffApplication> CompOffApplicationList = WetosDB.CompOffApplications.Where(a => a.EmployeeId == EmployeeNo).ToList();
                            return View(CompOffModelObj);

                        }

                        if (AppliedDay < TotalAllowedCompOff)
                        {

                            ModelState.AddModelError("", "You can not apply comp off for less than Total Allowed CompOff Days");
                            CompOffApplicationNecessaryContent(CompOffModelObj.EmployeeId);
                            PopulateDropDown();
                            //List<CompOffApplication> CompOffApplicationList = WetosDB.CompOffApplications.Where(a => a.EmployeeId == EmployeeNo).ToList();
                            return View(CompOffModelObj);

                        }


                        CompOffApplication COMPOFFAPPLICATIONObj = new CompOffApplication();

                        //added by shraddha on 10 jan 2017 
                        Employee EmployeeDetailsObj = WetosDB.Employees.Where(a => a.EmployeeId == CompOffModelObj.EmployeeId).FirstOrDefault();
                        COMPOFFAPPLICATIONObj.BranchId = EmployeeDetailsObj.BranchId;
                        COMPOFFAPPLICATIONObj.CompanyId = EmployeeDetailsObj.CompanyId;
                        COMPOFFAPPLICATIONObj.CompOffId = CompOffModelObj.CompOffId;

                        //added by shraddha on 10 jan 2017 
                        COMPOFFAPPLICATIONObj.EmployeeId = CompOffModelObj.EmployeeId;
                        COMPOFFAPPLICATIONObj.ExtrasHoursDate = CompOffModelObj.ExtrasHoursDate;
                        COMPOFFAPPLICATIONObj.FromDate = Convert.ToDateTime(CompOffModelObj.FromDate);
                        COMPOFFAPPLICATIONObj.ToDate = Convert.ToDateTime(CompOffModelObj.ToDate);
                        COMPOFFAPPLICATIONObj.FromDateStatus = CompOffModelObj.FromDateStatus;
                        COMPOFFAPPLICATIONObj.Purpose = CompOffModelObj.Purpose;
                        COMPOFFAPPLICATIONObj.RejectReason = CompOffModelObj.RejectReason;
                        COMPOFFAPPLICATIONObj.StatusId = CompOffModelObj.StatusId;
                        COMPOFFAPPLICATIONObj.ToDateStatus = CompOffModelObj.ToDateStatus;

                        //Added By Shraddha on 12 DEC 2016 for taking Status along with statusId Start
                        COMPOFFAPPLICATIONObj.Status = WetosDB.DropdownDatas.Where(a => a.TypeId == 3 && a.Value == CompOffModelObj.StatusId).Select(a => a.Text).FirstOrDefault();
                        //Added By Shraddha on 12 DEC 2016 for taking Status along with statusId End

                        WetosDB.CompOffApplications.Add(COMPOFFAPPLICATIONObj);

                        WetosDB.SaveChanges();

                        foreach (int CheckCompOffIdObj in CheckCompOffIdList)
                        {
                            CompOff CompOffObjForUpdateStatus = WetosDB.CompOffs.Where(a => a.CompOffId == CheckCompOffIdObj).FirstOrDefault();
                            CompOffObjForUpdateStatus.AppStatus = "P";
                            CompOffObjForUpdateStatus.CoDate = Convert.ToDateTime(CompOffModelObj.FromDate);
                            CompOffObjForUpdateStatus.CompOffApplicationID = COMPOFFAPPLICATIONObj.CompOffId;
                            WetosDB.SaveChanges();
                        }



                        //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 9 SEPTEMBER 2017
                        #region ADD AUDIT LOG
                        //OLD RECORD IS BLANK
                        string Newrecord = "Employee ID : " + EmpId
                            + "FromDate :" + COMPOFFAPPLICATIONObj.FromDate + "To Date :" + COMPOFFAPPLICATIONObj.ToDate
                            + "FromDateStatus :" + COMPOFFAPPLICATIONObj.FromDateStatus + "ToDateStatus :" + COMPOFFAPPLICATIONObj.ToDateStatus
                            + "Purpose :" + COMPOFFAPPLICATIONObj.Purpose + " Status : " + COMPOFFAPPLICATIONObj.Status + "Reject Reason :"
                            + COMPOFFAPPLICATIONObj.RejectReason + "ExtraHoursDate :" + COMPOFFAPPLICATIONObj.ExtrasHoursDate + "Branch ID :"
                            + COMPOFFAPPLICATIONObj.BranchId + "Company ID:" + COMPOFFAPPLICATIONObj.CompanyId;

                        //LOGIN ID IS TAKEN FROM SESSION PERSISTER
                        string Formname = "CUMULATIVE COMPOFF APPLICATION";
                        //ACTION IS UPDATE
                        string Message = " ";

                        WetosAdministrationController.GenerateAuditLogsInsert(WetosDB, EmpId, Formname, Newrecord, ref Message);
                        #endregion
                        //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 9 SEPTEMBER 2017



                        // NOTIFICATION ADDED BY RAJAS ON 31 DEC 2016
                        #region COMPOFF APPLICATION NOTIFICATION

                        // FOR NOTIFICATION TO BE SENT TO REPORTING PERSON

                        //Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == COMPOFFAPPLICATIONObj.EmployeeId).FirstOrDefault();
                        //Notification NotificationObj = new Notification();
                        //NotificationObj.FromID = EmployeeObj.EmployeeId;
                        //NotificationObj.ToID = EmployeeObj.EmployeeReportingId;
                        //NotificationObj.SendDate = DateTime.Now;
                        //NotificationObj.NotificationContent = "CompOff applied by " + EmployeeObj.FirstName + " " + EmployeeObj.LastName + " is pending for approval";
                        //NotificationObj.ReadFlag = false;
                        //NotificationObj.SendDate = DateTime.Now;

                        //WetosDB.Notifications.Add(NotificationObj);

                        //WetosDB.SaveChanges();

                        //// Check if both reporting person are are different
                        //if (EmployeeObj.EmployeeReportingId != EmployeeObj.EmployeeReportingId2)
                        //{
                        //    // FOR NOTIFICATION TO BE SENT TO REPORTING PERSON AS SANCTIONER

                        //    //Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == LeaveTypeObj.EmployeeId).FirstOrDefault();
                        //    Notification NotificationObj3 = new Notification();
                        //    NotificationObj3.FromID = EmployeeObj.EmployeeId;
                        //    NotificationObj3.ToID = EmployeeObj.EmployeeReportingId;
                        //    NotificationObj3.SendDate = DateTime.Now;
                        //    NotificationObj3.NotificationContent = "CompOff applied by " + EmployeeObj.FirstName + " " + EmployeeObj.LastName + " is pending for approval";
                        //    NotificationObj3.ReadFlag = false;
                        //    NotificationObj3.SendDate = DateTime.Now;

                        //    WetosDB.Notifications.Add(NotificationObj3);

                        //    WetosDB.SaveChanges();
                        //}

                        ////FOR SELF NOTIFICATION

                        //Notification NotificationObj2 = new Notification();
                        //NotificationObj2.FromID = EmployeeObj.EmployeeId;
                        //NotificationObj2.ToID = EmployeeObj.EmployeeId;
                        //NotificationObj2.SendDate = DateTime.Now;
                        //NotificationObj2.NotificationContent = "CompOff applied successfully";
                        //NotificationObj2.ReadFlag = false;
                        //NotificationObj2.SendDate = DateTime.Now;

                        //WetosDB.Notifications.Add(NotificationObj2);

                        //WetosDB.SaveChanges();

                        VwActiveEmployee EmployeeObj = WetosDB.VwActiveEmployees.Where(a => a.EmployeeId == COMPOFFAPPLICATIONObj.EmployeeId).FirstOrDefault();
                        Notification NotificationObj = new Notification();
                        NotificationObj.FromID = EmployeeObj.EmployeeId;
                        NotificationObj.ToID = EmployeeObj.EmployeeReportingId;
                        NotificationObj.SendDate = DateTime.Now;
                        NotificationObj.NotificationContent = "Received CompOff application for approval from " + EmployeeObj.FirstName + " " + EmployeeObj.LastName + " dated on " + COMPOFFAPPLICATIONObj.FromDate.Value.ToString("dd-MMM-yyyy") + " to " + COMPOFFAPPLICATIONObj.ToDate.Value.ToString("dd-MMM-yyyy");
                        NotificationObj.ReadFlag = false;
                        NotificationObj.SendDate = DateTime.Now;

                        WetosDB.Notifications.Add(NotificationObj);

                        WetosDB.SaveChanges();

                        // NOTIFICATION COUNT
                        int NoTiCount = WetosDB.Notifications.Where(a => a.ToID == EmpId && (a.ReadFlag == false || a.ReadFlag == null)).Count();
                        Session["NotificationCount"] = NoTiCount;

                        #endregion

                        // Code updated by Rajas on 13 JUNE 2017
                        #region EMAIL
                        // Added by Rajas on 19 JULY 2017 START
                        //GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == "Send Email").FirstOrDefault();

                        //ABOVE LINE COMMENTED AND BELOW LINE IS ADDED BY SHALAKA ON 13TH DEC 
                        GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.SendEmail).FirstOrDefault();

                        if (GlobalSettingObj != null)
                        {
                            // Send email ON
                            if (GlobalSettingObj.SettingValue == "1")
                            {

                                // Added by Rajas on 30 MARCH 2017 START
                                string EmailUpdateStatus = string.Empty;

                                if (EmployeeObj.Email != null)
                                {
                                    // Updated by Rajas on 19 JULY 2017 extra added parameter EmailFromWhichApplication
                                    if (SendEmail(EmployeeObj.Email, NotificationObj.NotificationContent, NotificationObj.NotificationContent, ref EmailUpdateStatus, "CompOff Application") == false)
                                    {
                                        Error(EmailUpdateStatus);

                                    }
                                }
                                else
                                {
                                    //Information("Email Id is not Provided; Please Provide Email Id to your Admin");
                                    AddAuditTrail("Unable to send email, as Email Id not available for " + EmployeeObj.EmployeeCode);
                                }
                                // Added by Rajas on 30 MARCH 2017 END
                            }
                            else // Send email OFF
                            {
                                AddAuditTrail("Unable to send Email as email utility is disabled");
                            }
                        }
                        // Added by Rajas on 19 JULY 2017 END
                        #endregion

                        // ADDED BY PUSHKAR FOR AuditLog ON 21 DEC 2016
                        AddAuditTrail("Success - New CompOff application created successfully");  // Updated by Rajas on 17 JAN 2017

                        // Added by Rajas on 17 JAN 2017 START
                        Success("Success - New CompOff application created successfully");

                        return RedirectToAction("CompOffApplicationIndex");
                    }
                    else
                    {
                        CompOffApplicationNecessaryContent(CompOffModelObj.EmployeeId);

                        //List<CompOffApplication> CompOffApplicationList = WetosDB.CompOffApplications.Where(a => a.EmployeeId == EmployeeNo).ToList();


                        // ADDED BY PUSHKAR FOR AuditLog ON 21 DEC 2016
                        AddAuditTrail("Error - New CompOff application failed");  // Updated by Rajas on 17 JAN 2017

                        // Added by Rajas on 17 JAN 2017 START
                        Error("Error - New CompOff application failed");

                        PopulateDropDown();

                        return View(CompOffModelObj);
                    }
                }
                else
                {
                    Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == EmpId).FirstOrDefault();
                    AddAuditTrail("Error - New CompOff application failed");  // Updated by Rajas on 17 JAN 2017
                    CompOffApplicationNecessaryContent(CompOffModelObj.EmployeeId);
                    // Added by Rajas on 17 JAN 2017 START
                    Error("No Pending comp off for Employee: " + EmployeeObj.FirstName + " " + EmployeeObj.LastName);

                    PopulateDropDown();

                    return View(CompOffModelObj);

                }
            }
            catch (System.Exception ex)
            {
                CompOffApplicationNecessaryContent(CompOffModelObj.EmployeeId);

                // ADDED BY PUSHKAR FOR AuditLog ON 21 DEC 2016
                AddAuditTrail("Exception - New CompOff application failed due to " + ex.Message);  // Updated by Rajas on 17 JAN 2017

                // Added by Rajas on 17 JAN 2017 START
                Error("Exception - New CompOff application failed due to " + ex.Message);

                PopulateDropDown();

                return View(CompOffModelObj);
            }
            #endregion



        }


        /// <summary>
        /// List view for Exception entry
        /// Added by Rajas on 6 MARCH 2017
        /// </summary>
        /// <returns></returns>
        /// Updated by Rajas on 16 MAY 2017
        public ActionResult ExceptionEntryListView()
        {
            try
            {
                // Updated by Rajas on 1 APRIL 2017 for MarkedAsDelete and EmployeeId
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                #region SP MODIFIED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                List<SP_ExceptionEntryListView_Result> ExceptionEntryListViewObj = WetosDB.SP_ExceptionEntryListView(EmployeeId)
                    .Where(a => a.MarkedAsDelete == null || a.MarkedAsDelete == 0).ToList();
                #endregion
                //ADDED BY RAJAS ON 27 DEC 2016
                AddAuditTrail("Success - Viewed exception entry list"); // Updated by Rajas on 6 MARCH 2017

                return View(ExceptionEntryListViewObj);
            }

            catch (System.Exception ex)
            {
                AddAuditTrail("Error : Error during checking list for condone entry due to " + ex.Message + " and " + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Error : Unable to display condone transaction list");

                return View();
            }
        }

        /// <summary>
        /// ExceptionEntryListViewDelete
        /// Added by Rajas on 9 JUNE 2017
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// Updated by Rajas on 22 AUGUST 2017
        public ActionResult ExceptionEntryListViewDelete(int id)
        {
            try
            {
                WetosDB.ExceptionEntry ExceptionObj = WetosDB.ExceptionEntries.Where(a => a.ExceptionId == id).FirstOrDefault();

                if (ExceptionObj != null)
                {
                    ExceptionObj.MarkedAsDelete = 1;

                    WetosDB.SaveChanges();

                    Success("Exception entry applied on  " + ExceptionObj.ExceptionDate.Value.ToString("dd-MMM-yyyy") + " deleted successfully");

                    AddAuditTrail("Exception entry applied on  " + ExceptionObj.ExceptionDate.Value.ToString("dd-MMM-yyyy") + " deleted successfully");
                }

                return RedirectToAction("ExceptionEntryListView");
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Please try again!");

                return RedirectToAction("ExceptionEntryListView");
            }
        }

        /// <summary>
        /// ADDED BY SHRADDHA ON 19 DEC 2016 FOR ExceptionEntry Edit POST
        /// </summary>
        /// <param name="EmployeeId"></param>
        /// <param name="TranDate"></param>
        /// <returns></returns>
        /// Updated by Rajas on 22 AUGUST 2017
        public ActionResult ExceptionEntryApply(int EmployeeId, DateTime TranDate)
        {
            int EmpId = Convert.ToInt32(Session["EmployeeNo"]);
            Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == EmpId).FirstOrDefault();
            try
            {
                ViewBag.active = true;
                DailyTransaction ExceptionEntryEdit = WetosDB.DailyTransactions.Where(b => b.EmployeeId == EmployeeId && b.TranDate == TranDate).FirstOrDefault();

                // Updated by Rajas on 18 JUNE 2017
                WetosDB.ExceptionEntry ExceptionEntryIsAvailableForEmployee = WetosDB.ExceptionEntries.Where(a => a.ExceptionDate == TranDate
                    && a.EmployeeId == EmployeeId && a.MarkedAsDelete == 0 && (a.Status != null || a.Status != "2")).FirstOrDefault();
                if (ExceptionEntryIsAvailableForEmployee != null)
                {
                    // Updated by Rajas on 20 JUNE 2017 to ignore Rejection condition
                    if (ExceptionEntryIsAvailableForEmployee.Status != "3")
                    {
                        Error("You Can not Apply Exception Entry For " + ExceptionEntryIsAvailableForEmployee.ExceptionDate.Value.ToString("dd-MMM-yyyy") + ". You may already have pending Exception Entries for this date.");
                        ViewBag.active = false;
                    }
                }

                var Shift = WetosDB.Shifts.Where(a => a.Company.CompanyId == EmployeeObj.CompanyId && a.BranchId == EmployeeObj.BranchId).Select(a => new { ShiftId = a.ShiftCode, ShiftName = a.ShiftName }).ToList();
                ViewBag.ShiftList = new SelectList(Shift, "ShiftId", "ShiftName").ToList();
                ExceptionEntryEdit.Remark = "";
                return View(ExceptionEntryEdit);
            }

            catch (System.Exception ex)
            {
                // Added by Rajas on 17 FEB 2017 START
                AddAuditTrail("Error - Error in ExceptionEntry due to " + ex.Message);

                Error("Please select valid entry from list");
                ViewBag.active = true;
                DailyTransaction ExceptionEntryEdit = WetosDB.DailyTransactions.Where(b => b.EmployeeId == EmployeeId && b.TranDate == TranDate).FirstOrDefault();

                // Updated by Rajas on 18 JUNE 2017
                WetosDB.ExceptionEntry ExceptionEntryIsAvailableForEmployee = WetosDB.ExceptionEntries.Where(a => a.ExceptionDate == TranDate
                    && a.EmployeeId == EmployeeId && a.MarkedAsDelete == 0 && (a.Status != null || a.Status != "2")).FirstOrDefault();
                if (ExceptionEntryIsAvailableForEmployee != null)
                {
                    // Updated by Rajas on 20 JUNE 2017 to ignore Rejection condition
                    if (ExceptionEntryIsAvailableForEmployee.Status != "3")
                    {
                        Error("You Can not Apply Exception Entry For " + ExceptionEntryIsAvailableForEmployee.ExceptionDate.Value.ToString("dd-MMM-yyyy") + ". You may already have pending Exception Entries for this date.");
                        ViewBag.active = false;
                    }
                }

                var Shift = WetosDB.Shifts.Where(a => a.Company.CompanyId == EmployeeObj.CompanyId && a.BranchId == EmployeeObj.BranchId).Select(a => new { ShiftId = a.ShiftCode, ShiftName = a.ShiftName }).ToList();
                ViewBag.ShiftList = new SelectList(Shift, "ShiftId", "ShiftName").ToList();

                return View(ExceptionEntryEdit);
            }
        }



        /// <summary>
        /// ADDED BY SHRADDHA ON 19 DEC 2016 FOPR ExceptionEntry Edit POST
        /// </summary>
        /// <param name="ExceptionEntryEdit"></param>
        /// <param name="fc"></param>
        /// <returns></returns>
        /// Updated by Rajas on 22 AUGUST 2017
        [HttpPost]
        public ActionResult ExceptionEntryApply(WetosDB.DailyTransaction ExceptionEntryEdit, FormCollection fc)
        {
            int EmpId = Convert.ToInt32(ExceptionEntryEdit.EmployeeId);
            //int EmpId = Convert.ToInt32(Session["EmployeeNo"]);
            Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == EmpId).FirstOrDefault();
            try
            {
                DateTime ExceptionDate = Convert.ToDateTime(ExceptionEntryEdit.TranDate);
                DailyTransaction ExceptionEntryObj = WetosDB.DailyTransactions.Where(b => b.TranDate == ExceptionDate
                    && b.EmployeeId == ExceptionEntryEdit.EmployeeId).FirstOrDefault();
                if (ExceptionEntryEdit.ShiftId == null)
                {
                    var Shift = WetosDB.Shifts.Select(a => new { ShiftId = a.ShiftCode, ShiftName = a.ShiftName }).ToList();
                    ViewBag.ShiftList = new SelectList(Shift, "ShiftId", "ShiftName").ToList();

                    // Added by Rajas on 17 JAN 2017 START
                    AddAuditTrail("Error - Exception entry failed due to shift id null");

                    Error("Please Enter valid shift");
                    // Added by Rajas on 17 JAN 2017 END

                    return View(ExceptionEntryEdit);
                }

                DateTime DefaultDate = new DateTime(0001, 01, 01, 00, 00, 00);
                if (ExceptionEntryEdit.Login == DefaultDate)
                {
                    var Shift = WetosDB.Shifts.Select(a => new { ShiftId = a.ShiftCode, ShiftName = a.ShiftName }).ToList();
                    ViewBag.ShiftList = new SelectList(Shift, "ShiftId", "ShiftName").ToList();

                    //Error("Invalid Login time");

                    return View(ExceptionEntryEdit);

                }
                else if (ExceptionEntryEdit.LogOut == DefaultDate)
                {
                    var Shift = WetosDB.Shifts.Select(a => new { ShiftId = a.ShiftCode, ShiftName = a.ShiftName }).ToList();
                    ViewBag.ShiftList = new SelectList(Shift, "ShiftId", "ShiftName").ToList();

                    //Error("Invalid Login time");

                    return View(ExceptionEntryEdit);
                }

                if (ExceptionEntryEdit.LogOut < ExceptionEntryEdit.Login)
                {
                    var Shift = WetosDB.Shifts.Where(a => a.Company.CompanyId == EmployeeObj.CompanyId && a.BranchId == EmployeeObj.BranchId).Select(a => new { ShiftId = a.ShiftCode, ShiftName = a.ShiftName }).ToList();
                    ViewBag.ShiftList = new SelectList(Shift, "ShiftId", "ShiftName").ToList();

                    Error("Log out time should be greater than LogIn time");
                    // Added by Rajas on 17 JAN 2017 END

                    return View(ExceptionEntryEdit);
                }

                string Reason = fc["Reason"].ToString();

                string OurRemark = fc["Remark"].ToString();  // To save remark from view in case of Special approval

                // Added by Rajas on 12 AUGUST 2017 START
                if (Reason == "Special Approval")
                {
                    if (OurRemark == null || OurRemark == string.Empty)
                    {
                        var Shift = WetosDB.Shifts.Where(a => a.Company.CompanyId == EmployeeObj.CompanyId && a.BranchId == EmployeeObj.BranchId).Select(a => new { ShiftId = a.ShiftCode, ShiftName = a.ShiftName }).ToList();
                        ViewBag.ShiftList = new SelectList(Shift, "ShiftId", "ShiftName").ToList();

                        Error("Please select remark");
                        // Added by Rajas on 17 JAN 2017 END

                        return View(ExceptionEntryObj);
                    }
                }
                // Added by Rajas on 12 AUGUST 2017 END

                WetosDB.ExceptionEntry ExceptionTbleObj = new WetosDB.ExceptionEntry();
                ExceptionTbleObj.DailyTrnId = ExceptionEntryEdit.DailyTrnId;

                //ExceptionTbleObj.Company = WetosDB.Companies.Where(a => a.CompanyId == ExceptionEntryEdit.CompanyId).FirstOrDefault();
                ExceptionTbleObj.CompanyId = Convert.ToInt32(WetosDB.Employees.Where(a => a.EmployeeId == ExceptionEntryEdit.EmployeeId).Select(a => a.CompanyId).FirstOrDefault());

                //ExceptionTbleObj.Branch = WetosDB.Branches.Where(a => a.BranchId == ExceptionEntryEdit.BranchId).FirstOrDefault();
                ExceptionTbleObj.BranchId = Convert.ToInt32(WetosDB.Employees.Where(a => a.EmployeeId == ExceptionEntryEdit.EmployeeId).Select(a => a.BranchId).FirstOrDefault());

                //ExceptionTbleObj.Employee = WetosDB.Employees.Where(a => a.EmployeeId == ExceptionEntryEdit.EmployeeId).FirstOrDefault();
                ExceptionTbleObj.EmployeeId = Convert.ToInt32(ExceptionEntryEdit.EmployeeId);

                // DateTime ExceptionDate = Convert.ToDateTime(ExceptionEntryEdit.Login.ToShortDateString() + " " + ExceptionEntryEdit.TranDate.ToShortTimeString());
                //string LoginDate = ExceptionEntryEdit.Login.ToShortDateString();
                ExceptionTbleObj.ExceptionDate = new DateTime(ExceptionDate.Year, ExceptionDate.Month, ExceptionDate.Day, 00, 00, 00);
                //ExceptionTbleObj.ExtraHrs = Convert.ToDateTime(ExceptionEntryEdit.ExtraHrs);
                //ExceptionTbleObj.Late = Convert.ToDateTime(ExceptionEntryEdit.Late);
                ExceptionTbleObj.PreviousShiftId = ExceptionEntryEdit.PreviousShiftId == null ? " " : ExceptionEntryEdit.PreviousShiftId;
                ExceptionTbleObj.StatusId = 1; // UPDATED BY SHRADDHA ON 12 SEP 2017 STRING TO INT
                ExceptionTbleObj.Status = WetosDB.DropdownDatas.Where(a => a.TypeId == 3 && a.Value == ExceptionTbleObj.StatusId)
                    .Select(a => a.Text).FirstOrDefault();//ADDED BY SHRADDHA ON 12 SEP 2017 TO SAVE STATUS
                // Updated by Rajas on 2 MARCH 2017 for saving special approval remark in Database Remar field
                if (OurRemark != null)
                {
                    ExceptionTbleObj.Remark = Reason == null ? "" : Reason + ": " + OurRemark;
                }

                else
                {
                    ExceptionTbleObj.Remark = Reason == null ? " " : Reason;
                }
                ExceptionTbleObj.LogOutTime = ExceptionEntryEdit.LogOut;
                // Updated by Rajas on 2 MARCH 2017
                ExceptionTbleObj.LoginTime = ExceptionEntryEdit.Login; //Convert.ToDateTime(LoginDate + " " + LoginTime);

                ExceptionTbleObj.ShiftId = ExceptionEntryEdit.ShiftId == null ? " " : ExceptionEntryEdit.ShiftId;

                DateTime LoginDate = Convert.ToDateTime(ExceptionDate.ToShortDateString() + " " + ExceptionEntryEdit.Login.ToShortTimeString());
                DateTime LogOutDate = Convert.ToDateTime(ExceptionDate.ToShortDateString() + " " + ExceptionEntryEdit.LogOut.ToShortTimeString());

                ExceptionTbleObj.LoginTime = LoginDate;
                ExceptionTbleObj.LogOutTime = LogOutDate;

                WetosDB.ExceptionEntries.Add(ExceptionTbleObj);

                WetosDB.SaveChanges();

                // NOTIFICATION ADDED BY RAJAS ON 31 DEC 2016
                #region EXCEPTION ENTRY APPLICATION NOTIFICATION

                // FOR NOTIFICATION TO BE SENT TO REPORTING PERSON

                //Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == ExceptionTbleObj.EmployeeId).FirstOrDefault();
                //Notification NotificationObj = new Notification();
                //NotificationObj.FromID = EmployeeObj.EmployeeId;
                //NotificationObj.ToID = EmployeeObj.EmployeeReportingId;
                //NotificationObj.SendDate = DateTime.Now;
                //NotificationObj.NotificationContent = "Exception entry applied by " + EmployeeObj.FirstName + " " + EmployeeObj.LastName + " is pending for approval";
                //NotificationObj.ReadFlag = false;
                //NotificationObj.SendDate = DateTime.Now;

                //WetosDB.Notifications.Add(NotificationObj);

                //WetosDB.SaveChanges();

                // Check if both reporting person are are different
                //if (EmployeeObj.EmployeeReportingId != EmployeeObj.EmployeeReportingId2)
                //{
                //    // FOR NOTIFICATION TO BE SENT TO REPORTING PERSON AS SANCTIONER

                //    //Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == LeaveTypeObj.EmployeeId).FirstOrDefault();
                //    Notification NotificationObj3 = new Notification();
                //    NotificationObj3.FromID = EmployeeObj.EmployeeId;
                //    NotificationObj3.ToID = EmployeeObj.EmployeeReportingId;
                //    NotificationObj3.SendDate = DateTime.Now;
                //    NotificationObj3.NotificationContent = "Exception entry applied by " + EmployeeObj.FirstName + " " + EmployeeObj.LastName + " is pending for approval";
                //    NotificationObj3.ReadFlag = false;
                //    NotificationObj3.SendDate = DateTime.Now;

                //    WetosDB.Notifications.Add(NotificationObj3);

                //    WetosDB.SaveChanges();
                //}

                ////FOR SELF NOTIFICATION

                //Notification NotificationObj2 = new Notification();
                //NotificationObj2.FromID = EmployeeObj.EmployeeId;
                //NotificationObj2.ToID = EmployeeObj.EmployeeId;
                //NotificationObj2.SendDate = DateTime.Now;
                //NotificationObj2.NotificationContent = "Exception entry applied successfully for " + ExceptionEntryEdit.Login.ToString("dd-MMM-yyyy") + " on " + DateTime.Now.ToString("dd-MMM-yyyy");
                //NotificationObj2.ReadFlag = false;
                //NotificationObj2.SendDate = DateTime.Now;

                //WetosDB.Notifications.Add(NotificationObj2);

                //WetosDB.SaveChanges();

                EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == ExceptionTbleObj.EmployeeId).FirstOrDefault();
                Notification NotificationObj = new Notification();
                NotificationObj.FromID = EmployeeObj.EmployeeId;
                NotificationObj.ToID = EmployeeObj.EmployeeReportingId;
                NotificationObj.SendDate = DateTime.Now;
                NotificationObj.NotificationContent = "Received Exception entry application for approval from " + EmployeeObj.FirstName + " " + EmployeeObj.LastName + " for " + ExceptionTbleObj.ExceptionDate.Value.ToString("dd-MMM-yyyy");
                NotificationObj.ReadFlag = false;
                NotificationObj.SendDate = DateTime.Now;

                WetosDB.Notifications.Add(NotificationObj);

                WetosDB.SaveChanges();

                // NOTIFICATION COUNT
                int NoTiCount = WetosDB.Notifications.Where(a => a.ToID == EmpId && (a.ReadFlag == false || a.ReadFlag == null)).Count();
                Session["NotificationCount"] = NoTiCount;

                #endregion

                // Code updated by Rajas on 13 JUNE 2017
                #region EMAIL
                // Added by Rajas on 19 JULY 2017 START
                //GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == "Send Email").FirstOrDefault();

                //ABOVE LINE COMMENTED AND BELOW LINE IS ADDED BY SHALAKA ON 13TH DEC 2017
                GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.SendEmail).FirstOrDefault();

                if (GlobalSettingObj != null)
                {
                    // Send email ON
                    if (GlobalSettingObj.SettingValue == "1")
                    {

                        // Added by Rajas on 30 MARCH 2017 START
                        string EmailUpdateStatus = string.Empty;

                        if (EmployeeObj.Email != null)
                        {
                            // Updated by Rajas on 19 JULY 2017 extra added parameter EmailFromWhichApplication
                            if (SendEmail(EmployeeObj.Email, NotificationObj.NotificationContent, NotificationObj.NotificationContent, ref EmailUpdateStatus, "Exception entry Application") == false)
                            {
                                Error(EmailUpdateStatus);

                            }
                        }
                        else
                        {
                            //Information("Email Id is not Provided; Please Provide Email Id to your Admin");
                            AddAuditTrail("Unable to send email, as Email Id not available for " + EmployeeObj.EmployeeCode);
                        }
                        // Added by Rajas on 30 MARCH 2017 END
                    }
                    else // Send email OFF
                    {
                        AddAuditTrail("Unable to send Email as email utility is disabled");
                    }
                }
                // Added by Rajas on 19 JULY 2017 END
                #endregion

                // Added by Rajas on 17 JAN 2017 START
                AddAuditTrail("Success - Exception entry successful");

                Success("Success - Exception entry successful");
                // Added by Rajas on 17 JAN 2017 END

                return RedirectToAction("ExceptionEntryListView");
                // }

                //else
                //{

                //    // DailyTransaction ExceptionEntryObj = WetosDB.DailyTransactions.Where(b => b.DailyTrnId == ExceptionEntryEdit.DailyTrnId).FirstOrDefault();

                //    var Shift = WetosDB.Shifts.Where(a => a.CompanyId == EmployeeObj.CompanyId && a.BranchId == EmployeeObj.BranchId).Select(a => new { ShiftId = a.ShiftCode, ShiftName = a.ShiftName }).ToList();
                //    ViewBag.ShiftList = new SelectList(Shift, "ShiftId", "ShiftName").ToList();
                //    // Added by Rajas on 17 JAN 2017 START
                //    AddAuditTrail("Error - Exception entry failed");

                //    Error("Error - Exception entry failed");
                //    // Added by Rajas on 17 JAN 2017 END

                //    return View(ExceptionEntryObj);


                //}


            }
            catch (System.Exception ex)
            {
                // Added by Rajas on 17 FEB 2017 START
                AddAuditTrail("Error - Error in ExceptionEntry due to " + ex.Message);

                Error("Please select valid entry from list");
                ViewBag.active = true;

                DailyTransaction ExceptionEntryEditObj = WetosDB.DailyTransactions.Where(b => b.EmployeeId == EmpId && b.TranDate == ExceptionEntryEdit.TranDate).FirstOrDefault();

                WetosDB.ExceptionEntry ExceptionEntryIsAvailableForEmployee = WetosDB.ExceptionEntries.Where(a => a.ExceptionDate == ExceptionEntryEdit.TranDate
                    && a.EmployeeId == EmpId && (a.Status != null || a.Status != "2")).FirstOrDefault();
                if (ExceptionEntryIsAvailableForEmployee != null)
                {
                    Error("You Can not Apply Exception Entry For " + ExceptionEntryIsAvailableForEmployee.ExceptionDate.Value.ToString("dd-MMM-yyyy") + ". You may already have pending Exception Entries for this date.");
                    ViewBag.active = false;
                }

                var Shift = WetosDB.Shifts.Where(a => a.Company.CompanyId == EmployeeObj.CompanyId && a.BranchId == EmployeeObj.BranchId).Select(a => new { ShiftId = a.ShiftCode, ShiftName = a.ShiftName }).ToList();
                ViewBag.ShiftList = new SelectList(Shift, "ShiftId", "ShiftName").ToList();

                return View(ExceptionEntryEdit);
            }
        }

        /// <summary>
        /// EXCEPTION ENTRY PAGE ON WHICH EXCEPTIONENTRY PARTIALVIEW WILL BE OPENED ADDED BY SHRADDHA ON 19 DEC 2016
        /// </summary>
        /// <returns></returns>
        public ActionResult ExceptionEntry()
        {
            PopulateDropDown();

            return View();
        }

        [HttpPost]
        //public ActionResult ExceptionEntryIndex(int companyId, int BranchId, string FromDate, string ToDate)
        public ActionResult ExceptionEntryIndex(int companyId, int BranchId, DateTime FromDate, DateTime ToDate, string selectCriteria, int EmployeeId)
        {
            //PASSING AN INPUT PARAMETER TO getExceptionEntryList() FUNCTION TO TAKE LOGIN EMPLOYEEWISE DATA BY SHRADDHA ON 12 JAN 2017
            int EmpId = EmployeeId;

            //List<WetosDB.DailyTransactions> ExceptionEntryList = WetosDB.DailyTransactions.ToList();

            List<SP_GetDailyTransactionList_Result> ExceptionEntryList = WetosDB.SP_GetDailyTransactionList(FromDate, ToDate).Where(a => a.EmployeeId == EmpId).ToList();

            if (selectCriteria == "Present")
            {
                ExceptionEntryList = ExceptionEntryList.Where(a => a.Status == "PPPP").ToList();
            }
            if (selectCriteria == "Absent")
            {
                ExceptionEntryList = ExceptionEntryList.Where(a => a.Status == "AAAA").ToList();
            }

            if (selectCriteria == "Single Punch")
            {
                ExceptionEntryList = ExceptionEntryList.Where(a => a.Status == "AAPP").ToList();
            }
            if (selectCriteria == "Late & Present")
            {
                ExceptionEntryList = ExceptionEntryList.Where(a => a.Status == "PPPP").ToList();
            }

            // ADDED BY PUSHKAR FOR AuditLog ON 21 DEC 2016
            AddAuditTrail("Exception Entry");

            return PartialView(ExceptionEntryList);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="EmployeeId"></param>
        /// <param name="ExceptionDate"></param>
        /// <returns></returns>
        /// Updated by Rajas on 22 AUGUST 2017
        public ActionResult ExceptionEntryEdit(int EmployeeId, DateTime ExceptionDate)
        {
            int EmpId = Convert.ToInt32(Session["EmployeeNo"]);
            Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == EmpId).FirstOrDefault();
            try
            {
                // DailyTransaction ExceptionEntryEdit = WetosDB.DailyTransactions.Where(b => b.EmployeeId == EmployeeId && b.TranDate == TranDate).FirstOrDefault();
                WetosDB.ExceptionEntry ExceptionEntryIsAvailableForEmployee = WetosDB.ExceptionEntries.Where(a => a.ExceptionDate == ExceptionDate
                    && a.EmployeeId == EmployeeId).FirstOrDefault();


                var Shift = WetosDB.Shifts.Where(a => a.Company.CompanyId == EmployeeObj.CompanyId && a.BranchId == EmployeeObj.BranchId).Select(a => new { ShiftId = a.ShiftCode, ShiftName = a.ShiftName }).ToList();
                ViewBag.ShiftList = new SelectList(Shift, "ShiftId", "ShiftName").ToList();

                string[] Remark = ExceptionEntryIsAvailableForEmployee.Remark.Split(':');
                ExceptionEntryIsAvailableForEmployee.Remark = Remark[0];
                return View(ExceptionEntryIsAvailableForEmployee);
            }

            catch (System.Exception ex)
            {
                // Added by Rajas on 17 FEB 2017 START
                AddAuditTrail("Error - Error in ExceptionEntry due to " + ex.Message);

                Error("Please select valid entry from list");
                var Shift = WetosDB.Shifts.Where(a => a.Company.CompanyId == EmployeeObj.CompanyId && a.BranchId == EmployeeObj.BranchId).Select(a => new { ShiftId = a.ShiftCode, ShiftName = a.ShiftName }).ToList();
                ViewBag.ShiftList = new SelectList(Shift, "ShiftId", "ShiftName").ToList();
                // Added by Rajas on 17 FEB 2017 END

                return View();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ExceptionEntryEdit"></param>
        /// <param name="fc"></param>
        /// <returns></returns>
        /// Updated by Rajas on 22 AUGUST 2017 2017
        [HttpPost]
        public ActionResult ExceptionEntryEdit(WetosDB.ExceptionEntry ExceptionEntryEdit, FormCollection fc)
        {
            int EmpId = Convert.ToInt32(ExceptionEntryEdit.EmployeeId);
            //int EmpId = Convert.ToInt32(Session["EmployeeNo"]);
            Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == EmpId).FirstOrDefault();
            try
            {
                DailyTransaction ExceptionEntryObj = WetosDB.DailyTransactions.Where(b => b.TranDate == ExceptionEntryEdit.ExceptionDate
                    && b.EmployeeId == ExceptionEntryEdit.EmployeeId).FirstOrDefault();

                if (ExceptionEntryEdit.ShiftId == null)
                {


                    var Shift = WetosDB.Shifts.Select(a => new { ShiftId = a.ShiftCode, ShiftName = a.ShiftName }).ToList();
                    ViewBag.ShiftList = new SelectList(Shift, "ShiftId", "ShiftName").ToList();

                    // Added by Rajas on 17 JAN 2017 START
                    AddAuditTrail("Error - Exception entry failed due to shift id null");

                    Error("Please Enter valid shift");
                    // Added by Rajas on 17 JAN 2017 END

                    return View(ExceptionEntryObj);
                }

                if (ExceptionEntryEdit.LogOutTime < ExceptionEntryEdit.LoginTime)
                {
                    var Shift = WetosDB.Shifts.Where(a => a.Company.CompanyId == EmployeeObj.CompanyId && a.BranchId == EmployeeObj.BranchId).Select(a => new { ShiftId = a.ShiftCode, ShiftName = a.ShiftName }).ToList();
                    ViewBag.ShiftList = new SelectList(Shift, "ShiftId", "ShiftName").ToList();

                    // Added by Rajas on 17 JAN 2017 START
                    AddAuditTrail("Log out time should be greater than LogIn time");

                    Error("Log out time should be greater than LogIn time");
                    // Added by Rajas on 17 JAN 2017 END

                    return View(ExceptionEntryObj);
                }

                //ADDED ModelState.IsValid BY SHRADDHA FOR VALIDATING DATA ON 17 JAN 2017
                if (ModelState.IsValid)
                {
                    // Updated by Rajas on 22 AUGUST 2017 START
                    // To handle null object error
                    string Reason = fc["Reason"] == null ? string.Empty : fc["Reason"].ToString();
                    string OurRemark = fc["Remark"] == null ? string.Empty : fc["Remark"].ToString();  // To save remark from view in case of Special approval
                    // Updated by Rajas on 22 AUGUST 2017 END

                    WetosDB.ExceptionEntry ExceptionTbleObj = WetosDB.ExceptionEntries.Where(a => a.ExceptionDate == ExceptionEntryEdit.ExceptionDate
                        && a.EmployeeId == ExceptionEntryEdit.EmployeeId).FirstOrDefault();
                    ExceptionTbleObj.DailyTrnId = ExceptionEntryEdit.DailyTrnId;

                    //ExceptionTbleObj.BranchId = Convert.ToInt32(ExceptionEntryEdit.BranchId);
                    // Above line commented and below line added by Rajas on 28 APRIL 2017, to Handle BranchId = 0 issue 
                    ExceptionTbleObj.BranchId = Convert.ToInt32(WetosDB.Employees.Where(a => a.EmployeeId == ExceptionEntryEdit.EmployeeId).Select(a => a.BranchId).FirstOrDefault());

                    ExceptionTbleObj.CompanyId = Convert.ToInt32(WetosDB.Employees.Where(a => a.EmployeeId == ExceptionEntryEdit.EmployeeId).Select(a => a.CompanyId).FirstOrDefault());
                    ExceptionTbleObj.EmployeeId = Convert.ToInt32(ExceptionEntryEdit.EmployeeId);
                    //string LoginDate = ExceptionEntryEdit.Login.ToShortDateString();

                    //DateTime ExceptionDate = Convert.ToDateTime(ExceptionEntryEdit.WorkingHrs.Value.ToShortDateString() + " " + ExceptionEntryEdit.ExceptionDate.Value.ToShortTimeString());
                    //string LoginDate = ExceptionEntryEdit.Login.ToShortDateString();
                    ExceptionTbleObj.ExceptionDate = new DateTime(ExceptionEntryEdit.ExceptionDate.Value.Year, ExceptionEntryEdit.ExceptionDate.Value.Month, ExceptionEntryEdit.ExceptionDate.Value.Day, 00, 00, 00);

                    //ExceptionTbleObj.ExceptionDate = (ExceptionEntryEdit.ExceptionDate);
                    //ExceptionTbleObj.ExtraHrs = Convert.ToDateTime(ExceptionEntryEdit.ExtraHrs);
                    //ExceptionTbleObj.Late = Convert.ToDateTime(ExceptionEntryEdit.Late);
                    ExceptionTbleObj.PreviousShiftId = ExceptionEntryEdit.PreviousShiftId == null ? " " : ExceptionEntryEdit.PreviousShiftId;
                    ExceptionTbleObj.StatusId = 1; // UPDATED BY SHRADDHA ON 12 SEP 2017 STRING TO INT
                    ExceptionTbleObj.Status = WetosDB.DropdownDatas.Where(a => a.TypeId == 3 && a.Value == ExceptionTbleObj.StatusId)
                        .Select(a => a.Text).FirstOrDefault();//ADDED BY SHRADDHA ON 12 SEP 2017 TO SAVE STATUS
                    // Updated by Rajas on 2 MARCH 2017 for saving special approval remark in Database Remar field
                    if (OurRemark != null)
                    {
                        ExceptionTbleObj.Remark = Reason == null ? "" : Reason + ": " + OurRemark;
                    }

                    else
                    {
                        ExceptionTbleObj.Remark = Reason == null ? " " : Reason;
                    }
                    ExceptionTbleObj.LogOutTime = ExceptionEntryEdit.LogOutTime;
                    // Updated by Rajas on 2 MARCH 2017
                    ExceptionTbleObj.LoginTime = ExceptionEntryEdit.LoginTime; //Convert.ToDateTime(LoginDate + " " + LoginTime);

                    ExceptionTbleObj.ShiftId = ExceptionEntryEdit.ShiftId == null ? " " : ExceptionEntryEdit.ShiftId;

                    //DateTime LoginDate = Convert.ToDateTime(ExceptionEntryEdit.ExceptionDate.Value.ToShortDateString() + " " + ExceptionEntryEdit.LoginTime.ToShortTimeString());
                    //DateTime LogOutDate = Convert.ToDateTime(ExceptionEntryEdit.ExceptionDate.Value.ToShortDateString() + " " + ExceptionEntryEdit.LogOutTime.ToShortTimeString());

                    DateTime LoginDate = new DateTime(ExceptionEntryEdit.ExceptionDate.Value.Year, ExceptionEntryEdit.ExceptionDate.Value.Month, ExceptionEntryEdit.ExceptionDate.Value.Day, ExceptionEntryEdit.LoginTime.Hour, ExceptionEntryEdit.LoginTime.Minute, ExceptionEntryEdit.LoginTime.Second);
                    DateTime LogOutDate = new DateTime(ExceptionEntryEdit.ExceptionDate.Value.Year, ExceptionEntryEdit.ExceptionDate.Value.Month, ExceptionEntryEdit.ExceptionDate.Value.Day, ExceptionEntryEdit.LogOutTime.Hour, ExceptionEntryEdit.LogOutTime.Minute, ExceptionEntryEdit.LogOutTime.Second);

                    ExceptionTbleObj.LoginTime = LoginDate;
                    ExceptionTbleObj.LogOutTime = LogOutDate;

                    // WetosDB.Exceptions.Add(ExceptionTbleObj);

                    WetosDB.SaveChanges();

                    // NOTIFICATION ADDED BY RAJAS ON 31 DEC 2016
                    #region EXCEPTION ENTRY APPLICATION NOTIFICATION

                    // FOR NOTIFICATION TO BE SENT TO REPORTING PERSON

                    //Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == ExceptionTbleObj.EmployeeId).FirstOrDefault();
                    Notification NotificationObj = new Notification();
                    NotificationObj.FromID = EmployeeObj.EmployeeId;
                    NotificationObj.ToID = EmployeeObj.EmployeeReportingId;
                    NotificationObj.SendDate = DateTime.Now;
                    NotificationObj.NotificationContent = "Exception entry applied by " + EmployeeObj.FirstName + " " + EmployeeObj.LastName + " is pending for approval";
                    NotificationObj.ReadFlag = false;
                    NotificationObj.SendDate = DateTime.Now;

                    WetosDB.Notifications.Add(NotificationObj);

                    WetosDB.SaveChanges();

                    //FOR SELF NOTIFICATION

                    Notification NotificationObj2 = new Notification();
                    NotificationObj2.FromID = EmployeeObj.EmployeeId;
                    NotificationObj2.ToID = EmployeeObj.EmployeeId;
                    NotificationObj2.SendDate = DateTime.Now;
                    NotificationObj2.NotificationContent = "Exception entry applied successfully for " + ExceptionEntryEdit.LoginTime.ToString("dd-MMM-yyyy") + " on " + DateTime.Now.ToString("dd-MMM-yyyy");
                    NotificationObj2.ReadFlag = false;
                    NotificationObj2.SendDate = DateTime.Now;

                    WetosDB.Notifications.Add(NotificationObj2);

                    WetosDB.SaveChanges();

                    // NOTIFICATION COUNT
                    int NoTiCount = WetosDB.Notifications.Where(a => a.ToID == EmpId && (a.ReadFlag == false || a.ReadFlag == null)).Count();
                    Session["NotificationCount"] = NoTiCount;

                    #endregion

                    // Code updated by Rajas on 13 JUNE 2017
                    #region EMAIL
                    // Added by Rajas on 19 JULY 2017 START
                    //GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == "Send Email").FirstOrDefault();

                    //ABOVE LINE COMMENTED AND BELOW LINE IS ADDED BY SHALAKA ON 13TH DEC 2017
                    GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.SendEmail).FirstOrDefault();

                    if (GlobalSettingObj != null)
                    {
                        // Send email ON
                        if (GlobalSettingObj.SettingValue == "1")
                        {

                            // Added by Rajas on 30 MARCH 2017 START
                            string EmailUpdateStatus = string.Empty;

                            if (EmployeeObj.Email != null)
                            {
                                // Updated by Rajas on 19 JULY 2017 extra added parameter EmailFromWhichApplication
                                if (SendEmail(EmployeeObj.Email, NotificationObj.NotificationContent, NotificationObj.NotificationContent, ref EmailUpdateStatus, "Exception entry Application") == false)
                                {
                                    Error(EmailUpdateStatus);

                                }
                            }
                            else
                            {
                                //Information("Email Id is not Provided; Please Provide Email Id to your Admin");
                                AddAuditTrail("Unable to send email, as Email Id not available for " + EmployeeObj.EmployeeCode);
                            }
                            // Added by Rajas on 30 MARCH 2017 END
                        }
                        else // Send email OFF
                        {
                            AddAuditTrail("Unable to send Email as email utility is disabled");
                        }
                    }
                    // Added by Rajas on 19 JULY 2017 END
                    #endregion

                    // Added by Rajas on 17 JAN 2017 START
                    AddAuditTrail("Success - Exception entry successful");

                    Success("Success - Exception entry successful");
                    // Added by Rajas on 17 JAN 2017 END

                    return RedirectToAction("ExceptionEntryListView");
                }

                else
                {

                    // DailyTransaction ExceptionEntryObj = WetosDB.DailyTransactions.Where(b => b.DailyTrnId == ExceptionEntryEdit.DailyTrnId).FirstOrDefault();

                    var Shift = WetosDB.Shifts.Where(a => a.Company.CompanyId == EmployeeObj.CompanyId && a.BranchId == EmployeeObj.BranchId).Select(a => new { ShiftId = a.ShiftCode, ShiftName = a.ShiftName }).ToList();
                    ViewBag.ShiftList = new SelectList(Shift, "ShiftId", "ShiftName").ToList();
                    // Added by Rajas on 17 JAN 2017 START
                    AddAuditTrail("Error - Exception entry failed");

                    Error("Error - Exception entry failed");
                    // Added by Rajas on 17 JAN 2017 END

                    return View(ExceptionEntryObj);
                }
            }
            catch (System.Exception ex)
            {
                // Added by Rajas on 17 FEB 2017 START
                AddAuditTrail("Error - Error in ExceptionEntry due to " + ex.Message);

                Error("Please select valid entry from list");
                ViewBag.active = true;
                DailyTransaction ExceptionEntryEditObj = WetosDB.DailyTransactions.Where(b => b.EmployeeId == EmpId
                    && b.TranDate == ExceptionEntryEdit.ExceptionDate).FirstOrDefault();
                WetosDB.ExceptionEntry ExceptionEntryIsAvailableForEmployee = WetosDB.ExceptionEntries.Where(a => a.ExceptionDate == ExceptionEntryEdit.ExceptionDate
                    && a.EmployeeId == EmpId && (a.Status != null || a.Status != "2")).FirstOrDefault();

                if (ExceptionEntryIsAvailableForEmployee != null)
                {
                    Error("You Can not Apply Exception Entry For " + ExceptionEntryIsAvailableForEmployee.ExceptionDate.Value.ToString("dd-MMM-yyyy")
                        + ". You may already have pending Exception Entries for this date.");
                    ViewBag.active = false;
                }

                var Shift = WetosDB.Shifts.Where(a => a.Company.CompanyId == EmployeeObj.CompanyId
                    && a.BranchId == EmployeeObj.BranchId).Select(a => new { ShiftId = a.ShiftCode, ShiftName = a.ShiftName }).ToList();
                ViewBag.ShiftList = new SelectList(Shift, "ShiftId", "ShiftName").ToList();

                return View(ExceptionEntryEdit);
            }
        }



        //Added by shraddha on 14th OCT 2016 For Actual Days
        //public JsonResult GetActualDays(string LeaveTypeid)
        //{
        //   var CompanyId= Session["CompanyId"];
        //   var BranchId = Session["BranchId"];
        //   var EmployeeId1 = Session["EmployeeNo"];
        //   int EmployeeId = Convert.ToInt32(EmployeeId1);



        //    return Json();
        //}

        //added by shraddha on 14th oct 2016 for actual days
        public JsonResult GetLeaveDetails()
        {
            int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);

            var ActualDays = WetosDB.LeaveBalances.Where(a => a.EmployeeId == EmployeeId).FirstOrDefault();

            return Json(ActualDays);
        }


        /// <summary>
        /// DISPLAY LEAVE ENCASHMENT LIST ON INDEX PAGE ADDED BY SHRADDHA ON 17 DEC 2016
        /// </summary>
        /// <returns></returns>
        public ActionResult EncashmentIndex()
        {
            #region SP MODIFIED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
            int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            List<SP_GetEncashmentList_Result> EncashmentListObj = WetosDB.SP_GetEncashmentList(EmployeeId).ToList();
            #endregion
            AddAuditTrail("");
            return View(EncashmentListObj);
        }


        /// <summary>
        /// APPLY FOR LEAVE ENCASHMENT PAGE ADDED BY SHRADDHA ON 17 DEC 2016
        /// </summary>
        /// <returns></returns>
        public ActionResult EncashmentApplication()
        {
            PopulateDropDown();

            return View();
        }

        /// <summary>
        /// APPLY FOR LEAVE ENCASHMENT PAGE ADDED BY SHRADDHA ON 17 DEC 2016
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EncashmentApplication(LeaveEncash LeaveEncashObj, FormCollection fc)
        {
            try
            {
                int EmpId = Convert.ToInt32(LeaveEncashObj.EmployeeId);

                int EmpTypeIdEncash = WetosDB.Employees.Where(a => a.EmployeeId == EmpId).Select(a => a.EmployeeTypeId).FirstOrDefault();

                // Above Line commented and below line added by Shalaka on 28th DEC 2017 -- Start
                //int EmpId = Convert.ToInt32(Session["EmployeeNo"]);
                // Above Line commented and below line added by Shalaka on 28th DEC 2017 -- End

                //CODE COMMENTED BY SHRADDHA ON 07 FEB 2017 START
                //string FinancialYear = LeaveEncashObj.FinancialYearId.ToString();
                // LeaveEncashObj.FinancialYearId = WetosDB.FinancialYears.Where(a => a.FinancialName == FinancialYear && a.BranchId == LeaveEncashObj.BranchId).Select(a => a.FinancialId).FirstOrDefault();
                //CODE COMMENTED BY SHRADDHA ON 07 FEB 2017 END

                //Added on 28th DEC 2017 for getting only allowed encashment leave type with encashable days -- Start
                var LeaveCodeObj = WetosDB.LeaveMasters.Where(a => a.EncashmentAllowedOrNot == true).Select(a => new { LeaveCode = a.LeaveCode, LeaveName = a.LeaveName, NoOfDaysEncashed = a.NoOfDaysEncashed }).FirstOrDefault();

                Double? NoOfEncashableDays = LeaveCodeObj.NoOfDaysEncashed;

                //Added By shalaka on 28th DEC 2017 to Check Encash Value is less than No Of Days to Encashed as per Leave Rule. -- Start
                if (NoOfEncashableDays >= LeaveEncashObj.EncashValue)
                {
                    var EncashLeaveObj = DashboardController.GetLeaveDetailsOnallControllers(WetosDB, EmpId, DateTime.Now);

                    string leavecode = LeaveEncashObj.LeaveCode;

                    SP_LeaveTableData_Result EncashLeaveEx = EncashLeaveObj.Where(a => a.LeaveType.Trim().ToUpper() == leavecode.ToUpper()).FirstOrDefault();

                    double Opening = EncashLeaveEx.OpeningBalance == null ? 0 : EncashLeaveEx.OpeningBalance.Value;
                    double Pending = EncashLeaveEx.Pending; //EncashLeaveEx.Pending == null ? 0 : EncashLeaveEx.Pending; // MODIFIED BY MSJ ON 23 JAN 2020
                    double LeaveUsed = EncashLeaveEx.LeaveUsed == null ? 0 : EncashLeaveEx.LeaveUsed.Value;

                    double EncashValueApp = LeaveEncashObj.EncashValue; //LeaveEncashObj.EncashValue == null ? 0 : LeaveEncashObj.EncashValue; // MODIFIED BY MSJ ON 23 JAN 2020

                    double CurrentBalance = Opening - (Pending + LeaveUsed);

                    double BalEncashAsperRule = CurrentBalance - EncashValueApp;

                    RuleTransaction RuleForEncashMinBal = WetosDB.RuleTransactions.Where(a => a.RuleId == 44 && a.EmployeeGroupId == EmpTypeIdEncash).FirstOrDefault();

                    if (RuleForEncashMinBal != null)
                    {
                        double RuleForEncashMinBalApplication = Convert.ToDouble(RuleForEncashMinBal.Formula);

                        if (BalEncashAsperRule >= RuleForEncashMinBalApplication)
                        {
                            RuleTransaction RuleForEncashMonth = WetosDB.RuleTransactions.Where(a => a.RuleId == 43 && a.EmployeeGroupId == EmpTypeIdEncash).FirstOrDefault();

                            if (RuleForEncashMonth != null)
                            {

                                string RuleForEncashMonthApp = RuleForEncashMonth.Formula;

                                string[] SplitRuleMonth = RuleForEncashMonthApp.Split(',');

                                string Month = DateTime.Now.ToString("MMMM");

                                if (DateTime.Now.ToString("MMMM").ToUpper() == SplitRuleMonth[0].ToUpper() || DateTime.Now.ToString("MMMM").ToUpper() == SplitRuleMonth[1].ToUpper())
                                {

                                    Employee EmpObj = WetosDB.Employees.Where(a => a.EmployeeId == EmpId).FirstOrDefault();

                                    //Branch of that Employee
                                    Branch BranchObj = WetosDB.Branches.Where(a => a.BranchId == EmpObj.BranchId).FirstOrDefault();
                                    WetosDB.Company CompanyObj = WetosDB.Companies.Where(a => a.CompanyId == EmpObj.CompanyId).FirstOrDefault();

                                    LeaveEncashObj.BranchId = BranchObj.BranchId;
                                    LeaveEncashObj.CompanyId = CompanyObj.CompanyId;
                                    LeaveEncashObj.EmployeeId = EmpObj.EmployeeId;

                                    FinancialYear FinancialYearObj = WetosDB.FinancialYears.Where(a => a.Company.CompanyId == CompanyObj.CompanyId && a.Branch.BranchId == BranchObj.BranchId).FirstOrDefault();
                                    LeaveEncashObj.FinancialYearId = FinancialYearObj.FinancialYearId;
                                    //Added by Shalaka on 28th DEC 2017 --- End

                                    if (Month.ToUpper() == SplitRuleMonth[0].ToUpper())
                                    {
                                        List<LeaveEncash> LeaveEncashFirstEntry = WetosDB.LeaveEncashes.Where(a => a.FinancialYearId == FinancialYearObj.FinancialYearId
                                            && a.EmployeeId == EmpId).ToList();

                                        

                                        if (LeaveEncashFirstEntry.Count > 0)
                                        {
                                            AddAuditTrail("Error - You cannot encash leave for this month as Encashment already exists");

                                            Error("Error - You cannot encash leave for this month as Encashment already exists");
                                            // Added by Shalaka on 28th DEC 2017--- End

                                            //ADDED BY PUSHKAR ON 14 MAY 2018
                                            PopulateDropDown();
                                            return View();
                                        }
                                    }

                                    if (Month.ToUpper() == SplitRuleMonth[1].ToUpper())
                                    {
                                        List<LeaveEncash> LeaveEncashSecondEntry = WetosDB.LeaveEncashes.Where(a => a.FinancialYearId == FinancialYearObj.FinancialYearId
                                            && a.EmployeeId == EmpId).ToList();

                                        if (LeaveEncashSecondEntry.Count == 1)
                                        {
                                            //LeaveEncash LeaveEncashSecondEntryEx = LeaveEncashSecondEntry.FirstOrDefault();

                                            
                                        }


                                        if (LeaveEncashSecondEntry.Count > 1)
                                        {
                                            AddAuditTrail("Error - You cannot encash leave for this month as Encashment already exists");

                                            Error("Error - You cannot encash leave for this month as Encashment already exists");
                                            // Added by Shalaka on 28th DEC 2017--- End

                                            //ADDED BY PUSHKAR ON 14 MAY 2018
                                            PopulateDropDown();
                                            return View();
                                        }
                                    }

                                    LeaveEncashObj.StatusId = 1;
                                    LeaveEncashObj.EncashDate = DateTime.Now;

                                    WetosDB.LeaveEncashes.Add(LeaveEncashObj);

                                    WetosDB.SaveChanges();

                                    // NOTIFICATION ADDED BY RAJAS ON 31 DEC 2016
                                    #region ENCASHMENT APPLICATION NOTIFICATION

                                    // FOR NOTIFICATION TO BE SENT TO REPORTING PERSON

                                    Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == LeaveEncashObj.EmployeeId).FirstOrDefault();
                                    Notification NotificationObj = new Notification();
                                    NotificationObj.FromID = EmployeeObj.EmployeeId;
                                    NotificationObj.ToID = EmployeeObj.EmployeeReportingId;
                                    NotificationObj.SendDate = DateTime.Now;
                                    NotificationObj.NotificationContent = "Leave encashment applied by " + EmployeeObj.FirstName + " " + EmployeeObj.LastName + " is pending for approval";
                                    NotificationObj.ReadFlag = false;
                                    NotificationObj.SendDate = DateTime.Now;

                                    WetosDB.Notifications.Add(NotificationObj);

                                    WetosDB.SaveChanges();

                                    //FOR SELF NOTIFICATION

                                    Notification NotificationObj2 = new Notification();
                                    NotificationObj2.FromID = EmployeeObj.EmployeeId;
                                    NotificationObj2.ToID = EmployeeObj.EmployeeId;
                                    NotificationObj2.SendDate = DateTime.Now;
                                    NotificationObj2.NotificationContent = "Leave encashment applied successfully";
                                    NotificationObj2.ReadFlag = false;
                                    NotificationObj2.SendDate = DateTime.Now;

                                    WetosDB.Notifications.Add(NotificationObj2);

                                    WetosDB.SaveChanges();

                                    // NOTIFICATION COUNT
                                    int NoTiCount = WetosDB.Notifications.Where(a => a.ToID == EmpId && (a.ReadFlag == false || a.ReadFlag == null)).Count();
                                    Session["NotificationCount"] = NoTiCount;

                                    #endregion

                                    // ADDED BY PUSHKAR FOR AuditLog ON 21 DEC 2016
                                    AddAuditTrail("Success - New encashment application created successfully");  // Updated by Rajas on 17 JAN 2017

                                    // Added by Rajas on 17 JAN 2017 START
                                    Success("Success - New encashment application created successfully");
                                }
                                else
                                {
                                    AddAuditTrail("Error - You cannot encash leave for this month Encashment only allowed in " + SplitRuleMonth[0] +
                                        " and " + SplitRuleMonth[1] + " Months");

                                    Error("Error - You cannot encash leave for this month Encashment only allowed in " + SplitRuleMonth[0] +
                                        " and " + SplitRuleMonth[1] + " Months");
                                    // Added by Shalaka on 28th DEC 2017--- End

                                    //ADDED BY PUSHKAR ON 14 MAY 2018
                                    PopulateDropDown();
                                    return View();

                                }
                            }
                            else
                            {
                                AddAuditTrail("Error - No rule set for Encashment Applicable Months , Please set the rule");

                                Error("Error - No rule set for Encashment Applicable Months , Please set the rule");
                                // Added by Shalaka on 28th DEC 2017--- End

                                //ADDED BY PUSHKAR ON 14 MAY 2018
                                PopulateDropDown();
                                return View();
                            }
                        }
                        else
                        {

                            AddAuditTrail("Error - Minimum Balance of " + RuleForEncashMinBalApplication + " should be maintained after application");

                            Error("Error - Minimum Balance of " + RuleForEncashMinBalApplication + " should be maintained after application");
                            // Added by Shalaka on 28th DEC 2017--- End

                            //ADDED BY PUSHKAR ON 14 MAY 2018
                            PopulateDropDown();
                            return View();
                        }
                    }
                    else
                    {
                        AddAuditTrail("Error - No Rule found for Minimum Balance After Application , Please set the rule");

                        Error("Error - No Rule found for Minimum Balance After Application , Please set the rule");
                        // Added by Shalaka on 28th DEC 2017--- End

                        //ADDED BY PUSHKAR ON 14 MAY 2018
                        PopulateDropDown();
                        return View();
                    }
                }
                else
                {
                    // Added by Shalaka on 28th DEC 2017--- Start
                    AddAuditTrail("Error - You cannot encash more than " + NoOfEncashableDays + " Leaves at once");

                    Error("Error - You cannot encash more than " + NoOfEncashableDays + " Leaves at once");
                    // Added by Shalaka on 28th DEC 2017--- End

                    //ADDED BY PUSHKAR ON 14 MAY 2018
                    PopulateDropDown();
                    return View();
                }

                return RedirectToAction("EncashmentIndex");
            }

            catch (System.Exception ex)
            {
                // Added by Rajas on 17 JAN 2017 START
                AddAuditTrail("Exception - New encashment application failed due " + ex.Message);  // Updated by Rajas on 17 JAN 2017

                Error("Exception - New encashment application failed due " + ex.Message);
                // Added by Rajas on 17 JAN 2017 END

                PopulateDropDown();
                return View();
            }
        }

        public ActionResult EncashmentApplicationEdit(int LeaveEncashId)
        {
            LeaveEncash LeaveEncashObj = WetosDB.LeaveEncashes.Where(a => a.LeaveEncashId == LeaveEncashId).FirstOrDefault();

            PopulateDropDown();

            //????
            // var FinancialYearObj = WetosDB.FinancialYears.Where(a => a.FinancialId == ).Select(a => a.FinancialName).Distinct().ToList();
            var FinancialYearObj = WetosDB.FinancialYears.Where(a => a.FinancialYearId == 4).Select(a => a.FinancialName).Distinct().ToList();
            ViewBag.FinancialYearList = new SelectList(FinancialYearObj, "FinancialName").ToList();

            //Commented by Shalaka on 29th DEC 2017 -- Start
            //var LeaveCodeObj = WetosDB.LeaveMasters.Select(a => new { LeaveCode = a.LeaveCode, LeaveName = a.LeaveName }).Distinct().ToList();
            //ViewBag.LeaveCodeList = new SelectList(LeaveCodeObj, "LeaveCode", "LeaveName").ToList();
            //Commented by Shalaka on 29th DEC 2017 -- Start

            return View(LeaveEncashObj);
        }

        [HttpPost]
        public ActionResult EncashmentApplicationEdit(WetosDB.LeaveEncash LeaveEncashEdit)
        {
            try
            {
                int EmpId = Convert.ToInt32(LeaveEncashEdit.EmployeeId);
                //Modified By Shraddha on 19 DEC 2016 Added if(ModelState.IsValid) Condition instead of try catch block
                if (ModelState.IsValid)
                {
                    WetosDB.LeaveEncash LeaveEncashApplicationObj = WetosDB.LeaveEncashes.Where(b => b.LeaveEncashId == LeaveEncashEdit.LeaveEncashId).FirstOrDefault();
                    LeaveEncashApplicationObj.BranchId = LeaveEncashEdit.BranchId;
                    LeaveEncashApplicationObj.CompanyId = LeaveEncashEdit.CompanyId;
                    LeaveEncashApplicationObj.EmployeeId = LeaveEncashEdit.EmployeeId;
                    LeaveEncashApplicationObj.FinancialYearId = LeaveEncashEdit.FinancialYearId;
                    LeaveEncashApplicationObj.LeaveCode = LeaveEncashEdit.LeaveCode;
                    LeaveEncashApplicationObj.RejectReason = LeaveEncashEdit.RejectReason;
                    LeaveEncashApplicationObj.StatusId = 1;
                    LeaveEncashApplicationObj.EncashValue = LeaveEncashEdit.EncashValue;

                    WetosDB.SaveChanges();

                    // NOTIFICATION ADDED BY RAJAS ON 31 DEC 2016
                    #region EDIT ENCASHMENT APPLICATION NOTIFICATION

                    // FOR NOTIFICATION TO BE SENT TO REPORTING PERSON

                    Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == LeaveEncashEdit.EmployeeId).FirstOrDefault();
                    Notification NotificationObj = new Notification();
                    NotificationObj.FromID = EmployeeObj.EmployeeId;
                    NotificationObj.ToID = EmployeeObj.EmployeeReportingId;
                    NotificationObj.SendDate = DateTime.Now;
                    NotificationObj.NotificationContent = "Leave encashment edited by " + EmployeeObj.FirstName + " " + EmployeeObj.LastName + " is pending for approval";
                    NotificationObj.ReadFlag = false;
                    NotificationObj.SendDate = DateTime.Now;

                    WetosDB.Notifications.Add(NotificationObj);

                    WetosDB.SaveChanges();

                    //FOR SELF NOTIFICATION

                    Notification NotificationObj2 = new Notification();
                    NotificationObj2.FromID = EmployeeObj.EmployeeId;
                    NotificationObj2.ToID = EmployeeObj.EmployeeId;
                    NotificationObj2.SendDate = DateTime.Now;
                    NotificationObj2.NotificationContent = "Leave encashment edited successfully";
                    NotificationObj2.ReadFlag = false;
                    NotificationObj2.SendDate = DateTime.Now;

                    WetosDB.Notifications.Add(NotificationObj2);

                    WetosDB.SaveChanges();

                    // NOTIFICATION COUNT
                    int NoTiCount = WetosDB.Notifications.Where(a => a.ToID == EmpId && (a.ReadFlag == false || a.ReadFlag == null)).Count();
                    Session["NotificationCount"] = NoTiCount;

                    #endregion

                    // Added by Rajas on 30 MARCH 2017 START
                    #region EMAIL
                    string EmailUpdateStatus = string.Empty;

                    if (EmployeeObj.Email != null)
                    {

                        if (SendEmail(EmployeeObj.Email, NotificationObj.NotificationContent, NotificationObj.NotificationContent, ref  EmailUpdateStatus, "OD/Travel Application") == false)
                        {
                            AddAuditTrail(EmailUpdateStatus);

                            // ADDED BY PUSHKAR FOR AuditLog ON 21 DEC 2016
                            AddAuditTrail("Success - Encashment application edited successfully");  // Updated by Rajas on 17 JAN 2017

                            // Added by Rajas on 17 JAN 2017 START
                            Success("Success - Encashment application edited successfully");

                            return RedirectToAction("EncashmentIndex");
                        }
                    }
                    else
                    {
                        //Information("Email Id is not Provided; Please Provide Email Id to your Admin");
                        Information("Email Id is not Provided. Please Provide Email Id.");
                    }

                    #endregion
                    // Added by Rajas on 30 MARCH 2017 END

                    // ADDED BY PUSHKAR FOR AuditLog ON 21 DEC 2016
                    AddAuditTrail("Success - Encashment application edited successfully");  // Updated by Rajas on 17 JAN 2017

                    // Added by Rajas on 17 JAN 2017 START
                    Success("Success - Encashment application edited successfully");

                    return RedirectToAction("EncashmentIndex");
                }
                else
                {


                    //Modified By Shraddha on 19 DEC 2016 Added Populate Dropdown and LeaveEncashEdit object pass start
                    PopulateDropDown();

                    //Modified By Shraddha on 19 DEC 2016 Added Populate Dropdown and LeaveEncashEdit object pass End

                    // Added by Rajas on 17 JAN 2017 START
                    AddAuditTrail("Error - Encashment application edit failed");  // Updated by Rajas on 17 JAN 2017

                    Error("Error - Encashment application edit failed");

                    // Added by Rajas on 17 JAN 2017 END

                    return View(LeaveEncashEdit);
                }
            }

            catch (System.Exception ex)
            {
                PopulateDropDown();

                // Added by Rajas on 17 JAN 2017 START
                AddAuditTrail("Error - Encashment application edit faileddue to " + ex.Message);  // Updated by Rajas on 17 JAN 2017

                Error("Error - Encashment application edit failed due to " + ex.Message);

                // Added by Rajas on 17 JAN 2017 END

                return View(LeaveEncashEdit);
            }

        }


        /// <summary>
        /// Added By Shraddha on 19 DEC 2016 For Deleting Entry From database
        /// </summary>
        /// <param name="LeaveEncashId"></param>
        /// <returns></returns>
        public ActionResult EncashmentApplicationDelete(int LeaveEncashId)
        {
            LeaveEncash LeaveEncashObj = WetosDB.LeaveEncashes.Where(a => a.LeaveEncashId == LeaveEncashId).FirstOrDefault();
            PopulateDropDown();
            // var FinancialYearObj = WetosDB.FinancialYears.Where(a => a.FinancialId == LeaveEncashObj.FinancialYearId).Select(a => a.FinancialName).Distinct().ToList();
            //ViewBag.FinancialYearList = new SelectList(FinancialYearObj, "FinancialName").ToList();

            var LeaveCodeObj = WetosDB.LeaveMasters.Select(a => new { LeaveCode = a.LeaveCode, LeaveName = a.LeaveName }).Distinct().ToList();
            ViewBag.LeaveCodeList = new SelectList(LeaveCodeObj, " LeaveCode", "LeaveName").ToList();
            return View(LeaveEncashObj);
        }


        [HttpPost]
        public ActionResult EncashmentApplicationDelete(WetosDB.LeaveEncash LeaveEncashDeleteObj)
        {
            LeaveEncash LeaveEncashObj = WetosDB.LeaveEncashes.Where(a => a.LeaveEncashId == LeaveEncashDeleteObj.LeaveEncashId).FirstOrDefault();
            WetosDB.LeaveEncashes.Add(LeaveEncashObj);
            WetosDB.SaveChanges();


            // ADDED BY PUSHKAR FOR AuditLog ON 21 DEC 2016
            AddAuditTrail("Encashmant Application Delete");


            return RedirectToAction("EncashmentIndex");
        }

        /// <summary>
        /// ADDED BY PUSHKAR ON 12 MAY 2018 FOR LEAVE DATA ON ENCASHMENT
        /// </summary>
        /// <param name="Companyid"></param>
        /// <returns></returns>
        public ActionResult EncashmentLeaveData(int EmpIdEncash)
        {

            var EncashLeaveObj = DashboardController.GetLeaveDetailsOnallControllers(WetosDB, EmpIdEncash, DateTime.Now);

            EncashLeaveObj = EncashLeaveObj.Where(a => a.LeaveType.Trim().ToUpper() == "PL").ToList();

            ViewBag.LeaveBalanceDetailsVB = EncashLeaveObj;

            return PartialView();
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
            var BranchList = WetosDB.SP_GetBranchList(EmployeeId).Where(a => a.CompanyId == SelCompanyId).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList(); //.Select(m => new { BranchId = m.BranchId, BranchName = m.BranchName}).ToList();
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
            #endregion
            return Json(BranchList);
        }

        /// <summary>
        /// Json return for to get Employee dropdown list on basis of Branch selection
        /// Added by Rajas on 27 DEC 2016
        /// </summary>
        /// <returns></returns>
        public JsonResult GetEmployee(string Companyid, string Branchid)
        {

            int SelBranchId = 0;
            if (!string.IsNullOrEmpty(Branchid))
            {
                if (Branchid.ToUpper() != "NULL")
                {
                    SelBranchId = Convert.ToInt32(Branchid);
                }
            }

            // DateTime Leavingdate = Convert.ToDateTime("01/01/1900");  // Added by Rajas on 10 MARCH 2017

            // Updated by Rajas to pass active employee list, on 10 MARCH 2017
            //List<Employee> EmployeeList = WetosDB.Employees.Where(a => a.BranchId == SelBranchId && a.Leavingdate == Leavingdate).ToList();

            #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
            int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            //List<VwActiveEmployee> EmployeeList = WetosDB.VwActiveEmployees.ToList();
            List<SP_VwActiveEmployee_Result> EmployeeList = WetosDB.SP_VwActiveEmployee(EmployeeId).ToList();
            #endregion
            return Json(EmployeeList);
        }


        /// <summary>
        /// SEND MAIL
        /// Added by Rajas on 31 MARCH 2017
        /// </summary>
        /// <param name="ToEmail"></param>
        /// <param name="Subject"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        /// Updated by Rajas on 22 AUGUST 2017
        public bool SendEmail(string ToEmail, string Subject, string Message, ref string UpdateStatus, string EmailFromWhichApplication)
        {
            // SEND EMAIL START
            string SMTPServerName = ConfigurationManager.AppSettings["SMTPServerName"];
            string SMTPUsername = ConfigurationManager.AppSettings["SMTPUsername"];
            string SMTPPassword = ConfigurationManager.AppSettings["SMTPPassword"];
            string SMTPPort = ConfigurationManager.AppSettings["SMTPPort"];

            int SMTPPortInt = Convert.ToInt32(SMTPPort);
            //string Contact1 = ConfigurationManager.AppSettings["Contact1"];
            //string Contact2 = ConfigurationManager.AppSettings["Contact2"];
            //string Support = ConfigurationManager.AppSettings["Support"];

            //String name = //collection.Get("name");
            //String email = //collection.Get("email");
            //string subject = Subject;// //collection.Get("subject");
            // string message = Message; ////collection.Get("message");

            //if (ModelState.IsValid)
            //{
            bool ReturnStatus = false;

            try
            {
                MailMessage msg = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                MailAddress from = new MailAddress(SMTPUsername); //"info@foodpatronservices.com");
                StringBuilder sb = new StringBuilder();

                //TO
                msg.To.Add(ToEmail); //"mjoshi@sushmatechnology.com");
                //msg.To.Add(Contact1); //"mjoshi@sushmatechnology.com");
                //msg.To.Add(email); //"mjoshi@sushmatechnology.com");

                //BCC
                //msg.Bcc.Add(Support);
                msg.Subject = Subject; // "Contact Us";

                msg.IsBodyHtml = false;

                msg.From = new MailAddress(SMTPUsername); //"info@foodpatronservices.com");

                smtp.Host = SMTPServerName; // "smtpout.asia.secureserver.net";

                smtp.EnableSsl = true;

                //smtp.Credentials = new System.Net.NetworkCredential("info@foodpatronservices.com", "info@fps");
                smtp.Credentials = new System.Net.NetworkCredential(SMTPUsername, SMTPPassword);//"info@foodpatronservices.com", "info@fps");

                smtp.Port = SMTPPortInt; // 25;


                //sb.Append("Name: " + name);
                //sb.Append(Environment.NewLine);
                //sb.Append("Email: " + email);
                //sb.Append(Environment.NewLine);
                //sb.Append("Subject: " + subject);
                //sb.Append(Environment.NewLine);
                //sb.Append("Comments: " + message);
                //Message

                msg.Body = Message; // sb.ToString();

                smtp.Send(msg);

                msg.Dispose();

                return ReturnStatus = true;

                //return RedirectToAction("Index", "Home");
            }
            catch (System.Exception ex)
            {
                // Updated by Rajas on 19 JULY 2017 
                // Added EmailFromWhichApplication where values for EmailFromWhichApplication will be Leave Application, Od/travel, ... etc 
                AddAuditTrail("Error in sending email in " + EmailFromWhichApplication + " : " + " due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                UpdateStatus = "Unable to send email!";

                return ReturnStatus;

                //return View("Error");
            }

            // SEND EMAIL END

            //return View();
        }
    }
}

