using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WetosMVCMainApp.Models;
using WetosDB;
using System.Net.Mail;
using System.Text;
using System.Configuration;

namespace WetosMVC.Controllers
{
    public class WetosCumulativeCompOffUtilizeController : BaseController
    {
        //
        // GET: /WetosCumulativeCompOffUtilize/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /WetosCumulativeCompOffUtilize/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /WetosCumulativeCompOffUtilize/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /WetosCumulativeCompOffUtilize/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /WetosCumulativeCompOffUtilize/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /WetosCumulativeCompOffUtilize/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /WetosCumulativeCompOffUtilize/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /WetosCumulativeCompOffUtilize/Delete/5

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
        /// To Populate Dropdown
        /// Added By shraddha on 17 OCT 2016
        /// </summary>
        public void PopulateDropDown(bool IsSelf = true)
        {
            //// Updated by Rajas on 6 MAY 2017 to get data which is not MarkedAsDelete
            //var CompanyObj = WetosDB.Companies.Where(a => a.MarkedAsDelete == null || a.MarkedAsDelete == 0).Select(a => new { CompanyId = a.CompanyId, CompanyName = a.CompanyName }).ToList();
            //ViewBag.CompanyList = new SelectList(CompanyObj, " CompanyId", "CompanyName").ToList();

            //var BranchObj = WetosDB.Branches.Where(a => a.MarkedAsDelete == null || a.MarkedAsDelete == 0).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList();
            //ViewBag.BranchList = new SelectList(BranchObj, " BranchId", "BranchName").ToList();

            //Added by shraddha on 14th oct 2016 start

            //var EmployeeObj = WetosDB.Employees.Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.FirstName + " " + a.LastName }).ToList();

            //ViewBag.EmployeeList = new SelectList(EmployeeObj, "EmployeeId", "EmployeeName").ToList();

            var RequisitionTYpeObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 1).Select(a => new { ODTourId = a.Text, ODTourType = a.Text }).ToList();

            ViewBag.RequisitionTYpeList = new SelectList(RequisitionTYpeObj, "ODTourId", "ODTourType").ToList();

            //var EmployeeCodeObj = WetosDB.Employees.Select(a => new { EmployeeId = a.EmployeeId, EmployeeCode = a.EmployeeCode }).ToList();

            //ViewBag.EmployeeCodeList = new SelectList(EmployeeCodeObj, "EmployeeId", "EmployeeCode").ToList();

            // Added by Rajas on 22 OCT 2016 for Dropdown list for FULL DAY or HALF DAY Leave
            var LeaveTypeStatusObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 9).Select(a => new { LeaveStatusID = a.Value, LeaveStatus = a.Text }).ToList();
            ViewBag.LeaveTypeStatusList = new SelectList(LeaveTypeStatusObj, "LeaveStatusID", "LeaveStatus").ToList();

            //Added by Shraddha on 17 DEC 2016 for displaying employee code and name both in dropdown
            //    var EmployeeCodeAndNameObj = WetosDB.VwActiveEmployees.Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + "   " + a.FirstName + " " + a.MiddleName + " " + a.LastName }).ToList();

            //Added by Shalaka on 26th OCT 2017

            #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
            int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            //var EmployeeCodeAndNameObj = WetosDB.VwActiveEmployees.Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
            var EmployeeCodeAndNameObj = WetosDB.SP_VwActiveEmployee(EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
            #endregion

            ViewBag.EmployeeCodeAndNameList = new SelectList(EmployeeCodeAndNameObj, "EmployeeId", "EmployeeName").ToList();

            //ADDED BY SHRADDHA ON 28 DEC 2016 FOR SELECT CRITERIA START
            List<string> SelectCriteriaObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 5).Select(a => a.Text).ToList();
            ViewBag.SelectCriteriaList = SelectCriteriaObj;
            //ADDED BY SHRADDHA ON 28 DEC 2016 FOR SELECT CRITERIA END

            // Added by Rajas on 19 MAY 2017
            var ODpurpose = WetosDB.DropdownDatas.Where(a => a.TypeId == 15).Select(a => new { ODPurposeId = a.Text, ODPurposeType = a.Text }).ToList();
            ViewBag.ODPurposeList = new SelectList(ODpurpose, "ODPurposeId", "ODPurposeType").ToList();

            //Added on 22nd DEC 2017 for getting only allowed encashment leave type-- Start
            var LeaveCodeObj = WetosDB.LeaveMasters.Where(a => a.EncashmentAllowedOrNot == true).Select(a => new { LeaveCode = a.LeaveCode, LeaveName = a.LeaveName }).Distinct().ToList();
            //Added on 22nd DEC 2017 for getting only allowed encashment leave type-- End

            ViewBag.LeaveCodeList = new SelectList(LeaveCodeObj, " LeaveCode", "LeaveName").ToList();

            var StatusObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 3).Select(a => new { Value = a.Value, Text = a.Text }).ToList();
            ViewBag.StatusList = new SelectList(StatusObj, "Value", "Text").ToList();

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
                WetosDB.CumulativeCompOffApplication CompOffApplicationEdit = WetosDB.CumulativeCompOffApplications.Where(a => a.CompOffApplicationId == id).FirstOrDefault();

                TimeSpan ts = (Convert.ToDateTime(CompOffApplicationEdit.ToDate)) - (Convert.ToDateTime(CompOffApplicationEdit.FromDate));

                float AppliedDay = ts.Days;

                if (CompOffApplicationEdit.FromDateStatus == 1 && CompOffApplicationEdit.ToDateStatus == 1)
                {
                    AppliedDay = AppliedDay + 1;
                } // Modified By Shraddha on 30 DEC 2016 for leave application validation changes start
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
        public ActionResult CompOffApplicationEdit(int id, WetosDB.CumulativeCompOffApplication CompOffApplicationEdit, FormCollection Collection)
        {
            try
            {
                List<CumulativeCompOff> CompOffList = WetosDB.CumulativeCompOffs.Where(a => a.CompOffApplicationID == CompOffApplicationEdit.CompOffApplicationId).ToList();

                int EmpId = Convert.ToInt32(Session["EmployeeNo"]);

                //Added By Shraddha on 12 DEC 2016 for adding date validation start
                if (CompOffApplicationEdit.FromDate > CompOffApplicationEdit.ToDate)
                {
                    ModelState.AddModelError("", "End Date should be Greater than or equal to From Date");
                }

                //Added By Shraddha on 12 DEC 2016 for adding date validation End

                //Modified By Shraddha on 12 DEC 2016 Added if(ModelState.IsValid) Condition instead of try catch block
                if (ModelState.IsValid)
                {
                    #region VALIDATE CO DATA

                    // Updated by Rajas on 27 SEP 2017 START
                    // Fixed Issue No. 6 in Flagship Defect Sheet raised by Sandeep sir
                    LeaveApplication LeaveApplicationObj = WetosDB.LeaveApplications.Where(a => a.EmployeeId == EmpId
                        && (a.StatusId == 1 || a.StatusId == 2 || a.StatusId == 4) && a.MarkedAsDelete == 0
                        //&& a.FromDate <= CompOffApplicationEdit.FromDate && a.ToDate >= CompOffApplicationEdit.ToDate
                        && ((a.FromDate == CompOffApplicationEdit.FromDate) // && a.ToDate > LeaveTypeObj.ToDate) 
                             || (a.FromDate < CompOffApplicationEdit.FromDate && a.ToDate == CompOffApplicationEdit.ToDate)
                             || (a.FromDate < CompOffApplicationEdit.FromDate && a.ToDate > CompOffApplicationEdit.ToDate)
                             || (a.FromDate > CompOffApplicationEdit.FromDate && a.ToDate < CompOffApplicationEdit.ToDate)
                             || (a.FromDate == CompOffApplicationEdit.FromDate && a.ToDate == CompOffApplicationEdit.ToDate)
                             || (a.FromDate > CompOffApplicationEdit.FromDate && a.FromDate < CompOffApplicationEdit.ToDate) // ADDED BY MSJ ON 11 JAN 2018
                             || (a.ToDate > CompOffApplicationEdit.FromDate && a.ToDate < CompOffApplicationEdit.ToDate) // ADDED BY MSJ ON 11 JAN 2018
                             || (a.ToDate == CompOffApplicationEdit.FromDate))
                        ).FirstOrDefault();

                    // Added by Rajas on 10 MARCH 2017 for validation to check if already pending or sanctioned CO is applied for same date
                    CompOffApplication CompoffApplicationObj = WetosDB.CompOffApplications.Where(a => a.EmployeeId == EmpId
                        && (a.StatusId == 1 || a.StatusId == 2 || a.StatusId == 4)
                        && a.MarkedAsDelete == 0
                        //&& a.FromDate <= CompOffApplicationEdit.FromDate && a.ToDate >= CompOffApplicationEdit.ToDate
                        && ((a.FromDate == CompOffApplicationEdit.FromDate) // && a.ToDate > LeaveTypeObj.ToDate) 
                             || (a.FromDate < CompOffApplicationEdit.FromDate && a.ToDate == CompOffApplicationEdit.ToDate)
                             || (a.FromDate < CompOffApplicationEdit.FromDate && a.ToDate > CompOffApplicationEdit.ToDate)
                             || (a.FromDate > CompOffApplicationEdit.FromDate && a.ToDate < CompOffApplicationEdit.ToDate)
                             || (a.FromDate == CompOffApplicationEdit.FromDate && a.ToDate == CompOffApplicationEdit.ToDate)
                             || (a.FromDate > CompOffApplicationEdit.FromDate && a.FromDate < CompOffApplicationEdit.ToDate) // ADDED BY MSJ ON 11 JAN 2018
                             || (a.ToDate > CompOffApplicationEdit.FromDate && a.ToDate < CompOffApplicationEdit.ToDate) // ADDED BY MSJ ON 11 JAN 2018
                             || (a.ToDate == CompOffApplicationEdit.FromDate))
                        && a.CompOffId != CompOffApplicationEdit.CompOffApplicationId).FirstOrDefault();

                    ODTour ODTourTblObj = WetosDB.ODTours.Where(a => a.EmployeeId == EmpId && (a.StatusId == 1 || a.StatusId == 2 || a.StatusId == 4)
                                    && a.MarkedAsDelete == 0
                        //&& a.FromDate <= CompOffApplicationEdit.FromDate && a.ToDate >= CompOffApplicationEdit.ToDate
                                    && ((a.FromDate == CompOffApplicationEdit.FromDate) // && a.ToDate > LeaveTypeObj.ToDate) 
                             || (a.FromDate < CompOffApplicationEdit.FromDate && a.ToDate == CompOffApplicationEdit.ToDate)
                             || (a.FromDate < CompOffApplicationEdit.FromDate && a.ToDate > CompOffApplicationEdit.ToDate)
                             || (a.FromDate > CompOffApplicationEdit.FromDate && a.ToDate < CompOffApplicationEdit.ToDate)
                             || (a.FromDate == CompOffApplicationEdit.FromDate && a.ToDate == CompOffApplicationEdit.ToDate)
                             || (a.FromDate > CompOffApplicationEdit.FromDate && a.FromDate < CompOffApplicationEdit.ToDate) // ADDED BY MSJ ON 11 JAN 2018
                             || (a.ToDate > CompOffApplicationEdit.FromDate && a.ToDate < CompOffApplicationEdit.ToDate) // ADDED BY MSJ ON 11 JAN 2018
                             || (a.ToDate == CompOffApplicationEdit.FromDate))
                                    ).FirstOrDefault();
                    // Updated by Rajas on 27 SEP 2017 END

                    for (DateTime CurrentDate = Convert.ToDateTime(CompOffApplicationEdit.FromDate); CurrentDate.Date <= CompOffApplicationEdit.ToDate; CurrentDate = CurrentDate.AddDays(1))
                    {
                        #region LEAVE ALREADY APPLIED BETWEEN SELECTED DATE
                        if (LeaveApplicationObj != null)
                        {
                            #region Validate Leave Data

                            // Code added by Rajas on 27 SEP 2017 START
                            if (LeaveApplicationObj.FromDate == LeaveApplicationObj.ToDate) // Single day
                            {
                                if (CurrentDate == LeaveApplicationObj.FromDate)
                                {
                                    if (LeaveApplicationObj.FromDayStatus == 1)  // Full day in single day
                                    {
                                        Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                        PopulateDropDown();
                                        CompOffApplicationNecessaryContent(EmpId);
                                        return View(CompOffApplicationEdit);
                                    }
                                    else
                                    {
                                        if (LeaveApplicationObj.FromDayStatus == CompOffApplicationEdit.FromDateStatus)
                                        {
                                            Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                            PopulateDropDown();
                                            CompOffApplicationNecessaryContent(EmpId);
                                            return View(CompOffApplicationEdit);
                                        }
                                        else
                                        {
                                            if (CompOffApplicationEdit.FromDateStatus == 1)
                                            {
                                                Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                                PopulateDropDown();
                                                CompOffApplicationNecessaryContent(EmpId);
                                                return View(CompOffApplicationEdit);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (CurrentDate == LeaveApplicationObj.FromDate)
                                {
                                    if (LeaveApplicationObj.FromDayStatus == 1)  // Full day in single day
                                    {
                                        Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                        PopulateDropDown();
                                        CompOffApplicationNecessaryContent(EmpId);
                                        return View(CompOffApplicationEdit);
                                    }
                                    else
                                    {
                                        if (LeaveApplicationObj.FromDayStatus == CompOffApplicationEdit.FromDateStatus)
                                        {
                                            Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                            PopulateDropDown();
                                            CompOffApplicationNecessaryContent(EmpId);
                                            return View(CompOffApplicationEdit);
                                        }
                                        else
                                        {
                                            if (CompOffApplicationEdit.FromDateStatus == 1)
                                            {
                                                Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                                PopulateDropDown();
                                                CompOffApplicationNecessaryContent(EmpId);
                                                return View(CompOffApplicationEdit);
                                            }
                                        }
                                    }
                                }
                                else if (CurrentDate == LeaveApplicationObj.ToDate)
                                {
                                    if (LeaveApplicationObj.ToDayStatus == 1)  // Full day in single day
                                    {
                                        Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                        PopulateDropDown();
                                        CompOffApplicationNecessaryContent(EmpId);
                                        return View(CompOffApplicationEdit);
                                    }
                                    else
                                    {
                                        if (LeaveApplicationObj.FromDayStatus == CompOffApplicationEdit.FromDateStatus)
                                        {
                                            Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                                + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                            PopulateDropDown();
                                            CompOffApplicationNecessaryContent(EmpId);
                                            return View(CompOffApplicationEdit);
                                        }
                                    }
                                }
                                else
                                {
                                    Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                        + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                    PopulateDropDown();
                                    CompOffApplicationNecessaryContent(EmpId);
                                    return View(CompOffApplicationEdit);
                                }
                            }
                            // Code added by Rajas on 27 SEP 2017 END

                            #endregion Validate OD Data
                        }
                        #endregion

                        #region VALIDATE COMPOFF
                        // Added by Rajas on 10 MARCH 2017 for validating is compoff already present for same date range as of leave START
                        if (CompoffApplicationObj != null)
                        {
                            #region Validate Compoff Data

                            // Code added by Rajas on 27 SEP 2017 START
                            if (CompoffApplicationObj.FromDate == CompoffApplicationObj.ToDate) // Single day
                            {
                                if (CurrentDate == CompoffApplicationObj.FromDate)
                                {
                                    if (CompoffApplicationObj.FromDateStatus == 1)  // Full day in single day
                                    {
                                        Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                            + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range"
                                            + "<br/>" + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                        PopulateDropDown();
                                        CompOffApplicationNecessaryContent(EmpId);
                                        return View(CompOffApplicationEdit);
                                    }
                                    else
                                    {
                                        if (CompoffApplicationObj.FromDateStatus == CompOffApplicationEdit.FromDateStatus)
                                        {
                                            Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                                + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                                + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                            PopulateDropDown();
                                            CompOffApplicationNecessaryContent(EmpId);
                                            return View(CompOffApplicationEdit);
                                        }
                                        else
                                        {
                                            if (CompOffApplicationEdit.FromDateStatus == 1)
                                            {
                                                Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                                    + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range"
                                                    + "<br/>" + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                                PopulateDropDown();
                                                CompOffApplicationNecessaryContent(EmpId);
                                                return View(CompOffApplicationEdit);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (CurrentDate == CompoffApplicationObj.FromDate)
                                {
                                    if (CompoffApplicationObj.FromDateStatus == 1)  // Full day in single day
                                    {
                                        Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                            + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                            + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                        PopulateDropDown();
                                        CompOffApplicationNecessaryContent(EmpId);
                                        return View(CompOffApplicationEdit);
                                    }
                                    else
                                    {
                                        if (CompoffApplicationObj.FromDateStatus == CompOffApplicationEdit.FromDateStatus)
                                        {
                                            Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                                + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                                + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                            PopulateDropDown();
                                            CompOffApplicationNecessaryContent(EmpId);
                                            return View(CompOffApplicationEdit);
                                        }
                                        else
                                        {
                                            if (CompOffApplicationEdit.FromDateStatus == 1)
                                            {
                                                Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                                    + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range"
                                                    + "<br/>" + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                                PopulateDropDown();
                                                CompOffApplicationNecessaryContent(EmpId);
                                                return View(CompOffApplicationEdit);
                                            }
                                        }
                                    }
                                }
                                else if (CurrentDate == CompoffApplicationObj.ToDate)
                                {
                                    if (CompoffApplicationObj.ToDateStatus == 1)  // Full day in single day
                                    {
                                        Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                            + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                            + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                        PopulateDropDown();
                                        CompOffApplicationNecessaryContent(EmpId);
                                        return View(CompOffApplicationEdit);
                                    }
                                    else
                                    {
                                        if (CompoffApplicationObj.FromDateStatus == CompOffApplicationEdit.FromDateStatus)
                                        {
                                            Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                                + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                                + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                            PopulateDropDown();
                                            CompOffApplicationNecessaryContent(EmpId);
                                            return View(CompOffApplicationEdit);
                                        }
                                    }
                                }
                                else
                                {
                                    Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                        + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                        + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                    PopulateDropDown();
                                    CompOffApplicationNecessaryContent(EmpId);
                                    return View(CompOffApplicationEdit);
                                }
                            }
                            // Code added by Rajas on 27 SEP 2017 END

                            #endregion Validate OD Data
                        }
                        // Added by Rajas on 10 MARCH 2017 for validating is compoff already present for same date range as of leave END
                        #endregion

                        #region VALIDATE OD
                        // Added by Rajas on 10 MARCH 2017 for validating is OD already present for same date range as of leave START
                        if (ODTourTblObj != null)
                        {
                            #region Validate OD Data

                            // Code added by Rajas on 27 SEP 2017 START
                            if (CompoffApplicationObj.FromDate == CompoffApplicationObj.ToDate) // Single day
                            {
                                if (CurrentDate == ODTourTblObj.FromDate)
                                {
                                    if (CompoffApplicationObj.FromDateStatus == 1)  // Full day in single day
                                    {
                                        Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                            + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                        PopulateDropDown();
                                        CompOffApplicationNecessaryContent(EmpId);
                                        return View(CompOffApplicationEdit);
                                    }
                                    else
                                    {
                                        if (ODTourTblObj.ODDayStatus == CompOffApplicationEdit.FromDateStatus)
                                        {
                                            Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                                + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                                + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                            PopulateDropDown();
                                            CompOffApplicationNecessaryContent(EmpId);
                                            return View(CompOffApplicationEdit);
                                        }
                                        else
                                        {
                                            if (CompOffApplicationEdit.FromDateStatus == 1)
                                            {
                                                Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                                    + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                                    + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                                PopulateDropDown();
                                                CompOffApplicationNecessaryContent(EmpId);
                                                return View(CompOffApplicationEdit);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (CurrentDate == ODTourTblObj.FromDate)
                                {
                                    if (ODTourTblObj.ODDayStatus == 1)  // Full day in single day
                                    {
                                        Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                            + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                            + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                        PopulateDropDown();
                                        CompOffApplicationNecessaryContent(EmpId);
                                        return View(CompOffApplicationEdit);
                                    }
                                    else
                                    {
                                        if (ODTourTblObj.ODDayStatus == CompOffApplicationEdit.FromDateStatus)
                                        {
                                            Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                                + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                                + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                            PopulateDropDown();
                                            CompOffApplicationNecessaryContent(EmpId);
                                            return View(CompOffApplicationEdit);
                                        }
                                        else
                                        {
                                            if (CompOffApplicationEdit.FromDateStatus == 1)
                                            {
                                                Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                                    + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                                    + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                                PopulateDropDown();
                                                CompOffApplicationNecessaryContent(EmpId);
                                                return View(CompOffApplicationEdit);
                                            }
                                        }
                                    }
                                }
                                else if (CurrentDate == ODTourTblObj.ToDate)
                                {
                                    if (ODTourTblObj.ODDayStatus == 1)  // Full day in single day
                                    {
                                        Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                            + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                            + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                        PopulateDropDown();
                                        CompOffApplicationNecessaryContent(EmpId);
                                        return View(CompOffApplicationEdit);
                                    }
                                    else
                                    {
                                        if (ODTourTblObj.ODDayStatus == CompOffApplicationEdit.FromDateStatus)
                                        {
                                            Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                                + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                                + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                            PopulateDropDown();
                                            CompOffApplicationNecessaryContent(EmpId);
                                            return View(CompOffApplicationEdit);
                                        }
                                    }
                                }
                                else
                                {
                                    Error("You Can not Apply Comp Off For " + CompOffApplicationEdit.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                        + CompOffApplicationEdit.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                        + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                    PopulateDropDown();
                                    CompOffApplicationNecessaryContent(EmpId);
                                    return View(CompOffApplicationEdit);
                                }
                            }
                            // Code added by Rajas on 27 SEP 2017 END

                            #endregion Validate OD Data
                        }
                        // Added by Rajas on 10 MARCH 2017 for validating is compoff already present for same date range as of leave END
                        #endregion
                    }

                    #region VALIDATE FOR LOCKED DATA
                    // Added by Rajas on 10 MARCH 2017 to Validate if data is locked, can not apply leave START
                    DailyTransaction DailyTransactionListObj = WetosDB.DailyTransactions.Where(a => a.TranDate >= CompOffApplicationEdit.FromDate && a.TranDate <= CompOffApplicationEdit.ToDate && a.EmployeeId == EmpId && a.Lock == "Y").FirstOrDefault();

                    if (DailyTransactionListObj != null)
                    {
                        ModelState.AddModelError("", "You Can not apply Comp Off for this range as Data is Locked");

                        PopulateDropDown();

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
                        //List<CompOffApplication> CompOffApplicationList = WetosDB.CompOffApplications.Where(a => a.EmployeeId == EmployeeNo).ToList();
                        return View(CompOffApplicationEdit);

                    }

                    if (AppliedDay < AllowedDays)
                    {

                        ModelState.AddModelError("", "You can not apply comp off for less than Total Allowed CompOff Days");
                        CompOffApplicationNecessaryContent(Convert.ToInt32(CompOffApplicationEdit.EmployeeId));
                        PopulateDropDown();
                        //List<CompOffApplication> CompOffApplicationList = WetosDB.CompOffApplications.Where(a => a.EmployeeId == EmployeeNo).ToList();
                        return View(CompOffApplicationEdit);

                    }
                    #endregion
                    else
                    {

                        WetosDB.CumulativeCompOffApplication CompOffApplicationObj = WetosDB.CumulativeCompOffApplications.Where(b => b.CompOffApplicationId == id).FirstOrDefault();

                        //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 11 SEPTEMBER 2017 FOR EDIT
                        WetosDB.CumulativeCompOffApplication CompOffApplicationObjEDIT = WetosDB.CumulativeCompOffApplications.Where(b => b.CompOffApplicationId == id).FirstOrDefault();


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
                            foreach (CumulativeCompOff CompOffObj in CompOffList)
                            {
                                CompOffObj.CoDate = CompOffApplicationEdit.FromDate;
                                CompOffObj.CompOffApplicationID = CompOffApplicationObj.CompOffApplicationId;
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
                        //if (MySelf == true)
                        //{
                        //    return RedirectToAction("CompOffApplicationIndex");
                        //}
                        //else
                        //{
                        //    return RedirectToAction("CompOffApplicationIndex", new { IsMySelf = "false" });
                        //}

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
        /// Comp Off application index
        /// Added by Rajas on 17 NOV 2016
        /// </summary>
        /// <returns></returns>
        public ActionResult CompOffApplicationIndex(string IsMySelf = "true")
        {
            try
            {
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);

                //GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == "Current Financial Year").FirstOrDefault();

                //ABOVE LINE COMMENTED AND BELOW LINE IS ADDED BY SHALAKA ON 13TH DEC 2017
                GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.CurrentFinancialYear).FirstOrDefault();

                if (GlobalSettingObj == null)
                {
                    Error("Inconsistent Financial year data");
                }
                DateTime CalanderStartDate = GetFYStartDate(); // DateTime CalanderStartDate = WetosDB.FinancialYears.Where(a => a.FinancialName == GlobalSettingObj.SettingValue).Select(a => a.StartDate).FirstOrDefault();

                #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                //List<Vw_CumulativeCompOffApplicationIndex> CompoffApplicationListObj = new List<Vw_CumulativeCompOffApplicationIndex>();  // Verify ?
                List<SP_Vw_CumulativeCompOffApplicationIndex_Result> CompoffApplicationListObj = new List<SP_Vw_CumulativeCompOffApplicationIndex_Result>();  // Verify ?
                #endregion
                if (CalanderStartDate != null)
                {
                    //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO MAKE SELF AND OTHER COMP OFF APPLICATION GENERIC CODE START
                    if (IsMySelf == "true")
                    {
                        #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                        //CompoffApplicationListObj = WetosDB.Vw_CumulativeCompOffApplicationIndex.Where(a => a.EmployeeId == EmployeeId
                        //&& a.FromDate >= CalanderStartDate && (a.MarkedAsDelete == 0 || a.MarkedAsDelete == null)).ToList();
                        CompoffApplicationListObj = WetosDB.SP_Vw_CumulativeCompOffApplicationIndex(EmployeeId).Where(a => a.EmployeeId == EmployeeId
                       && a.FromDate >= CalanderStartDate && (a.MarkedAsDelete == 0 || a.MarkedAsDelete == null)).ToList();
                        #endregion
                    }
                    else
                    {
                        #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                        //CompoffApplicationListObj = WetosDB.Vw_CumulativeCompOffApplicationIndex.Where(a => a.EmployeeId != EmployeeId && a.FromDate >= CalanderStartDate && (a.MarkedAsDelete == 0 || a.MarkedAsDelete == null)).ToList();
                        CompoffApplicationListObj = WetosDB.SP_Vw_CumulativeCompOffApplicationIndex(EmployeeId).Where(a => a.EmployeeId != EmployeeId && a.FromDate >= CalanderStartDate && (a.MarkedAsDelete == 0 || a.MarkedAsDelete == null)).ToList();
                        #endregion
                    }
                    //CODE ADDED BY SHRADDHA ON 01 NOV 2017 TO MAKE SELF AND OTHER COMP OFF APPLICATION GENERIC CODE END
                }
                AddAuditTrail("List of applied CO checked");
                ViewBag.ForOthers = IsMySelf;
                return View(CompoffApplicationListObj);
            }

            catch (System.Exception ex)
            {
                AddAuditTrail("Error in Compoff list view due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Error in viewing list for Compoff");
                ViewBag.ForOthers = IsMySelf;
                return View("Error");
            }
        }

        /// <summary>
        /// CompOffApplicationDelete
        /// Updated by Rajas on 9 JUNE 2017
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult CumulativeCompOffApplicationDelete(int CompOffApplicationId)
        {
            try
            {

                CumulativeCompOffApplication COMPOFFAPPLICATIONObj = WetosDB.CumulativeCompOffApplications.Where(a => a.CompOffApplicationId == CompOffApplicationId).FirstOrDefault();


                //List<CumulativeCompOff> CompOffList = WetosDB.CumulativeCompOffs.Where(a => a.CompOffApplicationID == CompOffApplicationId).ToList();
                //int CompOffIdDel = Convert.ToInt32(COMPOFFAPPLICATIONObj.CompOffId);
                //List<CumulativeCompOff> CompOffList = WetosDB.CumulativeCompOffs.Where(a => a.CompOffId == CompOffIdDel).ToList();

                if (COMPOFFAPPLICATIONObj != null)
                {
                    COMPOFFAPPLICATIONObj.MarkedAsDelete = 1;

                    List<string> CompOffIdDeleteSplit = new List<string>();

                    List<string> CompOffIdSplit = COMPOFFAPPLICATIONObj.CompOffId.Split(',').ToList();

                    foreach (string StrIdMin in CompOffIdSplit)
                    {
                        List<string> StrIdMinList = StrIdMin.Split('-').ToList();
                        int CompOffIdStringConv = Convert.ToInt32(StrIdMinList[0]);

                        int ConsumedMinCompOff = Convert.ToInt32(StrIdMinList[1]);

                        CumulativeCompOff CompOffObj = WetosDB.CumulativeCompOffs.Where(a => a.CompOffId == CompOffIdStringConv).FirstOrDefault();

                        if (CompOffObj != null)
                        {
                            CompOffObj.AppStatus = null;
                            CompOffObj.CoDate = null;
                            CompOffObj.CompOffApplicationID = null;


                            //if (COMPOFFAPPLICATIONObj.FromDateStatus == 1 && COMPOFFAPPLICATIONObj.ToDateStatus == 1)
                            //{

                            CompOffObj.BalanceCoHours = CompOffObj.BalanceCoHours + ConsumedMinCompOff;
                            CompOffObj.UsedCoHours = CompOffObj.UsedCoHours - ConsumedMinCompOff;
                            //}
                            //else
                            //{
                            //CompOffObj.BalanceCoHours = CompOffObj.BalanceCoHours + 240;
                            //CompOffObj.UsedCoHours = CompOffObj.UsedCoHours - 240;
                            //}

                            WetosDB.SaveChanges();

                        }

                    }


                    //COMMENTED BY PUSHKAR ON 24 APRIL 2018s

                    //if (CompOffList.Count > 0)
                    //{
                    //    foreach (CumulativeCompOff CompOffObj in CompOffList)
                    //    {
                    //        #region COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 11 APR 2018 TO RESOLVE ISSUES POINTED BY DAYANAND SIR AND ANAGHA IN EMAIL (REFER EMAIL - 05 APR 2018 SUB - Anagha Khaire)
                    //        //CompOffObj.AppStatus = null;
                    //        //CompOffObj.CoDate = null; // ADDED BY SHRADDHA ON 11 AUG 2017
                    //        //CompOffObj.CompOffApplicationID = null;
                    //        //WetosDB.SaveChanges();

                    //        CompOffObj.AppStatus = null;
                    //        CompOffObj.CoDate = null;
                    //        CompOffObj.CompOffApplicationID = null;

                    //        //CompOffSanctionObjforcompoff.BalanceCoHours = CompOffSanctionObjforcompoff.BalanceCoHours + TotalWorkingHours; //CODE ADDED BY SHRADDHA ON 20 MAR 2018
                    //        //if (CompOffObj.UsedCoHours > CompOffObj.CoHours)
                    //        //{
                    //        //    CompOffObj.BalanceCoHours = CompOffObj.CoHours;
                    //        //}
                    //        //else
                    //        //{
                    //        //    CompOffObj.BalanceCoHours = CompOffObj.BalanceCoHours + CompOffObj.UsedCoHours;
                    //        //}
                    //        //CompOffObj.UsedCoHours = CompOffObj.UsedCoHours - CompOffObj.CoHours;

                    //        if (COMPOFFAPPLICATIONObj.FromDateStatus == 1 && COMPOFFAPPLICATIONObj.ToDateStatus == 1)
                    //        {

                    //            CompOffObj.BalanceCoHours = CompOffObj.BalanceCoHours + 480;
                    //            CompOffObj.UsedCoHours = CompOffObj.UsedCoHours - 480;
                    //        }
                    //        else
                    //        {
                    //            CompOffObj.BalanceCoHours = CompOffObj.BalanceCoHours + 240;
                    //            CompOffObj.UsedCoHours = CompOffObj.UsedCoHours - 240;
                    //        }

                    //        WetosDB.SaveChanges();
                    //        #endregion
                    //    }
                    //}

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
        /// FUNCTION ADDED BY SHRADDHA ON 09 MARCH 2017
        /// </summary>
        /// <returns></returns>
        private bool CompOffApplicationNecessaryContent(int EmployeeId)
        {
            bool ReturnStatus = false;

            int EmployeeGroupId = WetosDB.EmployeeGroupDetails.Where(a => a.Employee.EmployeeId == EmployeeId).Select(a => a.EmployeeGroup.EmployeeGroupId).FirstOrDefault();

            string EmployeeGroupName = WetosDB.EmployeeGroups.Where(a => a.EmployeeGroupId == EmployeeGroupId).Select(a => a.EmployeeGroupName).FirstOrDefault();

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

            Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == EmployeeId).FirstOrDefault();
            List<RuleTransaction> RuleTransactionList = WetosDB.RuleTransactions.Where(a => a.EmployeeGroupId == EmployeeGroupId).ToList();

            RuleTransaction RuleTransactionObj = RuleTransactionList.Where(a => a.RuleId == 9).FirstOrDefault(); //9 - COMP OFF IS AVAIALABLE OR NOT
            RuleTransaction RTObjForCompOffFullDayLimitValue = RuleTransactionList.Where(a => a.RuleId == 10).FirstOrDefault();
            RuleTransaction RTObjForCompOffHalfDayLimitValue = RuleTransactionList.Where(a => a.RuleId == 11).FirstOrDefault();
            //ADDED NEW LOGIC BY SHRADDHA ON 09 MAR 2017 END

            //ADDED NEW LOGIC BY SHRADDHA ON 10 JAN 2017
            double ActualStatusDoubleforfullday = 0;
            double ActualStatusDoubleforhalfday = 0;

            //CODE ADDED BY SHRADDHA ON 09 MARCH 2017
            DateTime NewDateForFullDay = new DateTime();
            //DateTime NewDateForHalfDay = new DateTime();

            List<CumulativeCompOff> AttendanceStatusList = new List<CumulativeCompOff>();
            if (RuleTransactionObj != null)
            {
                if (RuleTransactionObj.Formula.ToUpper().Trim() == "TRUE")
                {
                    //CODE ADDED BY SHRADDHA ON 20 FEB 2018 START
                    RuleTransaction ExtraHoursAccumulationAllowedornot = WetosDB.RuleTransactions.Where(a => a.RuleId == 34).FirstOrDefault(); // 34 - Extra Hours Accumulation Allowed or not //CODE ADDED BY SHRADDHA ON 19 FEB 2018
                    RuleTransaction MinRequiredExtraHoursForAccumulation = WetosDB.RuleTransactions.Where(a => a.RuleId == 35).FirstOrDefault(); //35 - Min. required extra hours for accumulation //CODE ADDED BY SHRADDHA ON 19 FEB 2018

                    if (ExtraHoursAccumulationAllowedornot != null && MinRequiredExtraHoursForAccumulation != null)
                    {
                        string[] MinRequiredExtraHoursForAccumulationSplitValue = MinRequiredExtraHoursForAccumulation.Formula.Split(':');
                        int AccumulationHourInt = 0;
                        int AccumulationMinuteInt = 0;
                        double MinTotalTimeRequiredForAccumulation = 0;
                        AccumulationHourInt = Convert.ToInt32(MinRequiredExtraHoursForAccumulationSplitValue[0]); // Minutes missing
                        AccumulationMinuteInt = Convert.ToInt32(MinRequiredExtraHoursForAccumulationSplitValue[1]); // Minutes missing
                        MinTotalTimeRequiredForAccumulation = (60 * AccumulationHourInt) + AccumulationMinuteInt;
                        //AttendanceStatusList = WetosDB.CompOffs.Where(a => (a.EmployeeId == EmployeeId && (a.CoHours.Value.Hour * 60 + a.CoHours.Value.Minute) >= MinTotalTimeRequiredForAccumulation) && a.DayStatus == null).ToList();
                        AttendanceStatusList = WetosDB.CumulativeCompOffs.Where(a => (a.EmployeeId == EmployeeId && a.CoHours >= MinTotalTimeRequiredForAccumulation) && (a.DayStatus == null || string.IsNullOrEmpty(a.DayStatus))).ToList(); // COMMNETED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 20 MAR 2018 
                    }
                    else
                    { }

                    ReturnStatus = true;
                }
                else
                {
                    Error("Cumulative Comp Off is not Allowed for Employee Group : " + EmployeeGroupName);
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
                        //if (compoffobj.CoHours.Value.TimeOfDay >= NewDateForHalfDay.TimeOfDay && (compoffobj.CoHours.Value.Hour * 60 + compoffobj.CoHours.Value.Minute) < (NewDateForFullDay.Hour * 60 + NewDateForFullDay.Minute))
                        if ((compoffobj.CoHours) < (NewDateForFullDay.Hour * 60 + NewDateForFullDay.Minute)) // COMMNETED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 20 MAR 2018 
                        {
                            ActualStatusDoubleforhalfday = 0.5;
                            actualstatusdoubleobj = actualstatusdoubleobj + ActualStatusDoubleforhalfday;
                        }

                        //else if (compoffobj.CoHours.Value.TimeOfDay >= NewDateForFullDay.TimeOfDay)
                        else // COMMNETED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 20 MAR 2018 
                        {
                            ActualStatusDoubleforfullday = 1;
                            actualstatusdoubleobj = ActualStatusDoubleforfullday + actualstatusdoubleobj;
                        }

                    }

                    foreach (var AttendanceStatusObj in AttendanceStatusList)
                    {
                        AttendanceStatusObj.DayStatus = AttendanceStatusObj.DayStatus == null ? "" : AttendanceStatusObj.DayStatus.Trim();
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
        /// MODIFIED CODE BY SHRADDHA ON 09 MARCH 2017 FOR CompOffApplication GET
        /// Try catch block added by Rajas on 22 FEB 2017
        /// </summary>
        /// <returns></returns>
        public ActionResult CompOffApplication(bool Myself = true)
        {
            try
            {
                int EmpId = Convert.ToInt32(Session["EmployeeNo"]);
                COMPOffApplicationModel ComOffApp = new COMPOffApplicationModel();
                CompOffApplicationNecessaryContent(EmpId);
                ViewBag.Myself = Myself.ToString();
                ComOffApp.Myself = Myself.ToString();
                ViewBag.EmployeeId = EmpId;
                PopulateDropDown();
                ComOffApp.EmployeeId = EmpId;
                return View(ComOffApp);
            }

            catch (System.Exception ex)
            {
                AddAuditTrail("CompOff application failed due to " + ex.Message);
                Error("Can not apply CompOff");

                return RedirectToAction("CompOffApplicationIndex");
            }
        }


        public ActionResult CumulativeCompOffApplicationPV(int EmpId)
        {
            CompOffApplicationNecessaryContent(EmpId);
            return PartialView();
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
            bool MySelf = false;
            int EmpId = CompOffModelObj.EmployeeId;
            try
            {
                try
                {
                    string MySelfStr = (Collection["MySelf"].ToString());
                    if (!string.IsNullOrEmpty(MySelfStr))
                    {
                        try
                        {
                            MySelf = Convert.ToBoolean(MySelfStr);
                        }
                        catch
                        {
                            MySelf = false;
                        }
                    }
                }
                catch
                {
                    MySelf = false;
                }
                #region COMPOFF CODE MODIFIED BY SHRADDHA

                if (MySelf == true)
                {
                    EmpId = Convert.ToInt32(Session["EmployeeNo"]);
                    CompOffModelObj.EmployeeId = EmpId;
                }
                VwActiveEmployee EmployeeObj = WetosDB.VwActiveEmployees.Where(a => a.EmployeeId == CompOffModelObj.EmployeeId).FirstOrDefault();


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

                        CompOffApplicationNecessaryContent(EmpId);

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


                    // double TotalAllowedCompOff = Convert.ToDouble(Collection["TtlAllwdDys"]);
                    //Added By Shraddha on 12 DEC 2016 for adding date validation start
                    if (Convert.ToDateTime(CompOffModelObj.FromDate) > Convert.ToDateTime(CompOffModelObj.ToDate))
                    {
                        ModelState.AddModelError("", "End Date should be Greater than or equal to From Date");
                        CompOffApplicationNecessaryContent(EmpId);
                        PopulateDropDown();
                        //List<CompOffApplication> CompOffApplicationList = WetosDB.CompOffApplications.Where(a => a.EmployeeId == EmployeeNo).ToList();
                        return View(CompOffModelObj);
                    }
                    //Added By Shraddha on 12 DEC 2016 for adding date validation End

                    if (ModelState.IsValid)
                    {
                        #region VALIDATE CO DATA

                        // Updated by Rajas on 27 SEP 2017 START
                        // Fixed Issue No. 6 in Flagship Defect Sheet raised by Sandeep sir
                        LeaveApplication LeaveApplicationObj = WetosDB.LeaveApplications.Where(a => a.EmployeeId == EmpId
                            && (a.StatusId == 1 || a.StatusId == 2 || a.StatusId == 4) && a.MarkedAsDelete == 0
                            && ((a.FromDate == CompOffModelObj.FromDate) // && a.ToDate > LeaveTypeObj.ToDate) 
                             || (a.FromDate < CompOffModelObj.FromDate && a.ToDate == CompOffModelObj.ToDate)
                             || (a.FromDate < CompOffModelObj.FromDate && a.ToDate > CompOffModelObj.ToDate)
                             || (a.FromDate > CompOffModelObj.FromDate && a.ToDate < CompOffModelObj.ToDate)
                             || (a.FromDate == CompOffModelObj.FromDate && a.ToDate == CompOffModelObj.ToDate)
                             || (a.FromDate > CompOffModelObj.FromDate && a.FromDate < CompOffModelObj.ToDate) // ADDED BY MSJ ON 11 JAN 2018
                             || (a.ToDate > CompOffModelObj.FromDate && a.ToDate < CompOffModelObj.ToDate) // ADDED BY MSJ ON 11 JAN 2018
                             || (a.ToDate == CompOffModelObj.FromDate))
                            //&& a.FromDate <= CompOffModelObj.FromDate && a.ToDate >= CompOffModelObj.ToDate
                            ).FirstOrDefault();

                        // Added by Rajas on 10 MARCH 2017 for validation to check if already pending or sanctioned CO is applied for same date
                        CumulativeCompOffApplication CompoffApplicationObj = WetosDB.CumulativeCompOffApplications.Where(a => a.EmployeeId == EmpId
                            && (a.StatusId == 1 || a.StatusId == 2 || a.StatusId == 4)
                            && a.MarkedAsDelete == 0
                            //&& a.FromDate <= CompOffModelObj.FromDate && a.ToDate >= CompOffModelObj.ToDate
                             && ((a.FromDate == CompOffModelObj.FromDate) // && a.ToDate > LeaveTypeObj.ToDate) 
                             || (a.FromDate < CompOffModelObj.FromDate && a.ToDate == CompOffModelObj.ToDate)
                             || (a.FromDate < CompOffModelObj.FromDate && a.ToDate > CompOffModelObj.ToDate)
                             || (a.FromDate > CompOffModelObj.FromDate && a.ToDate < CompOffModelObj.ToDate)
                             || (a.FromDate == CompOffModelObj.FromDate && a.ToDate == CompOffModelObj.ToDate)
                             || (a.FromDate > CompOffModelObj.FromDate && a.FromDate < CompOffModelObj.ToDate) // ADDED BY MSJ ON 11 JAN 2018
                             || (a.ToDate > CompOffModelObj.FromDate && a.ToDate < CompOffModelObj.ToDate) // ADDED BY MSJ ON 11 JAN 2018
                             || (a.ToDate == CompOffModelObj.FromDate))
                            ).FirstOrDefault();

                        ODTour ODTourTblObj = WetosDB.ODTours.Where(a => a.EmployeeId == EmpId
                            && (a.StatusId == 1 || a.StatusId == 2 || a.StatusId == 4)
                                        && a.MarkedAsDelete == 0
                            //&& a.FromDate <= CompOffModelObj.FromDate && a.ToDate >= CompOffModelObj.ToDate
                                         && ((a.FromDate == CompOffModelObj.FromDate) // && a.ToDate > LeaveTypeObj.ToDate) 
                             || (a.FromDate < CompOffModelObj.FromDate && a.ToDate == CompOffModelObj.ToDate)
                             || (a.FromDate < CompOffModelObj.FromDate && a.ToDate > CompOffModelObj.ToDate)
                             || (a.FromDate > CompOffModelObj.FromDate && a.ToDate < CompOffModelObj.ToDate)
                             || (a.FromDate == CompOffModelObj.FromDate && a.ToDate == CompOffModelObj.ToDate)
                             || (a.FromDate > CompOffModelObj.FromDate && a.FromDate < CompOffModelObj.ToDate) // ADDED BY MSJ ON 11 JAN 2018
                             || (a.ToDate > CompOffModelObj.FromDate && a.ToDate < CompOffModelObj.ToDate) // ADDED BY MSJ ON 11 JAN 2018
                             || (a.ToDate == CompOffModelObj.FromDate))
                                        ).FirstOrDefault();
                        // Updated by Rajas on 27 SEP 2017 END

                        for (DateTime CurrentDate = Convert.ToDateTime(CompOffModelObj.FromDate); CurrentDate.Date <= CompOffModelObj.ToDate; CurrentDate = CurrentDate.AddDays(1))
                        {
                            #region LEAVE ALREADY APPLIED BETWEEN SELECTED DATE
                            if (LeaveApplicationObj != null)
                            {
                                #region Validate Leave Data

                                // Code added by Rajas on 27 SEP 2017 START
                                if (LeaveApplicationObj.FromDate == LeaveApplicationObj.ToDate) // Single day
                                {
                                    if (CurrentDate == LeaveApplicationObj.FromDate)
                                    {
                                        if (LeaveApplicationObj.FromDayStatus == 1)  // Full day in single day
                                        {
                                            Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                            PopulateDropDown();
                                            CompOffApplicationNecessaryContent(EmpId);
                                            return View(CompOffModelObj);
                                        }
                                        else
                                        {
                                            if (LeaveApplicationObj.FromDayStatus == CompOffModelObj.FromDateStatus)
                                            {
                                                Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                                PopulateDropDown();
                                                CompOffApplicationNecessaryContent(EmpId);
                                                return View(CompOffModelObj);
                                            }
                                            else
                                            {
                                                if (CompOffModelObj.FromDateStatus == 1)
                                                {
                                                    Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                                    PopulateDropDown();
                                                    CompOffApplicationNecessaryContent(EmpId);
                                                    return View(CompOffModelObj);
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (CurrentDate == LeaveApplicationObj.FromDate)
                                    {
                                        if (LeaveApplicationObj.FromDayStatus == 1)  // Full day in single day
                                        {
                                            Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                            PopulateDropDown();
                                            CompOffApplicationNecessaryContent(EmpId);
                                            return View(CompOffModelObj);
                                        }
                                        else
                                        {
                                            if (LeaveApplicationObj.FromDayStatus == CompOffModelObj.FromDateStatus)
                                            {
                                                Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                                PopulateDropDown();
                                                CompOffApplicationNecessaryContent(EmpId);
                                                return View(CompOffModelObj);
                                            }
                                            else
                                            {
                                                if (CompOffModelObj.FromDateStatus == 1)
                                                {
                                                    Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                                    PopulateDropDown();
                                                    CompOffApplicationNecessaryContent(EmpId);
                                                    return View(CompOffModelObj);
                                                }
                                            }
                                        }
                                    }
                                    else if (CurrentDate == LeaveApplicationObj.ToDate)
                                    {
                                        if (LeaveApplicationObj.ToDayStatus == 1)  // Full day in single day
                                        {
                                            Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                            PopulateDropDown();
                                            CompOffApplicationNecessaryContent(EmpId);
                                            return View(CompOffModelObj);
                                        }
                                        else
                                        {
                                            if (LeaveApplicationObj.FromDayStatus == CompOffModelObj.FromDateStatus)
                                            {
                                                Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                                PopulateDropDown();
                                                CompOffApplicationNecessaryContent(EmpId);
                                                return View(CompOffModelObj);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>" + "You may already have pending or approved/sanctioned leaves for this date range.");

                                        PopulateDropDown();
                                        CompOffApplicationNecessaryContent(EmpId);
                                        return View(CompOffModelObj);
                                    }
                                }
                                // Code added by Rajas on 27 SEP 2017 END

                                #endregion Validate OD Data
                            }
                            #endregion

                            #region VALIDATE COMPOFF
                            // Added by Rajas on 10 MARCH 2017 for validating is compoff already present for same date range as of leave START
                            if (CompoffApplicationObj != null)
                            {
                                #region Validate Compoff Data

                                // Code added by Rajas on 27 SEP 2017 START
                                if (CompoffApplicationObj.FromDate == CompoffApplicationObj.ToDate) // Single day
                                {
                                    if (CurrentDate == CompoffApplicationObj.FromDate)
                                    {
                                        if (CompoffApplicationObj.FromDateStatus == 1)  // Full day in single day
                                        {
                                            Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range"
                                                + "<br/>" + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                            PopulateDropDown();
                                            CompOffApplicationNecessaryContent(EmpId);
                                            return View(CompOffModelObj);
                                        }
                                        else
                                        {
                                            if (CompoffApplicationObj.FromDateStatus == CompOffModelObj.FromDateStatus)
                                            {
                                                Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                                    + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                                    + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                                PopulateDropDown();
                                                CompOffApplicationNecessaryContent(EmpId);
                                                return View(CompOffModelObj);
                                            }
                                            else
                                            {
                                                if (CompOffModelObj.FromDateStatus == 1)
                                                {
                                                    Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range"
                                                        + "<br/>" + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                                    PopulateDropDown();
                                                    CompOffApplicationNecessaryContent(EmpId);
                                                    return View(CompOffModelObj);
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (CurrentDate == CompoffApplicationObj.FromDate)
                                    {
                                        if (CompoffApplicationObj.FromDateStatus == 1)  // Full day in single day
                                        {
                                            Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                                + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                                + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                            PopulateDropDown();
                                            CompOffApplicationNecessaryContent(EmpId);
                                            return View(CompOffModelObj);
                                        }
                                        else
                                        {
                                            if (CompoffApplicationObj.FromDateStatus == CompOffModelObj.FromDateStatus)
                                            {
                                                Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                                    + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                                    + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                                PopulateDropDown();
                                                CompOffApplicationNecessaryContent(EmpId);
                                                return View(CompOffModelObj);
                                            }
                                            else
                                            {
                                                if (CompOffModelObj.FromDateStatus == 1)
                                                {
                                                    Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range"
                                                        + "<br/>" + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                                    PopulateDropDown();
                                                    CompOffApplicationNecessaryContent(EmpId);
                                                    return View(CompOffModelObj);
                                                }
                                            }
                                        }
                                    }
                                    else if (CurrentDate == CompoffApplicationObj.ToDate)
                                    {
                                        if (CompoffApplicationObj.ToDateStatus == 1)  // Full day in single day
                                        {
                                            Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                                + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                                + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                            PopulateDropDown();
                                            CompOffApplicationNecessaryContent(EmpId);
                                            return View(CompOffModelObj);
                                        }
                                        else
                                        {
                                            if (CompoffApplicationObj.FromDateStatus == CompOffModelObj.FromDateStatus)
                                            {
                                                Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                                    + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                                PopulateDropDown();
                                                CompOffApplicationNecessaryContent(EmpId);
                                                return View(CompOffModelObj);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                            + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                        PopulateDropDown();
                                        CompOffApplicationNecessaryContent(EmpId);
                                        return View(CompOffModelObj);
                                    }
                                }
                                // Code added by Rajas on 27 SEP 2017 END

                                #endregion Validate OD Data
                            }
                            // Added by Rajas on 10 MARCH 2017 for validating is compoff already present for same date range as of leave END
                            #endregion

                            #region VALIDATE OD
                            // Added by Rajas on 10 MARCH 2017 for validating is OD already present for same date range as of leave START
                            if (ODTourTblObj != null)
                            {
                                #region Validate OD Data

                                // Code added by Rajas on 27 SEP 2017 START
                                if (CompoffApplicationObj.FromDate == CompoffApplicationObj.ToDate) // Single day
                                {
                                    if (CurrentDate == ODTourTblObj.FromDate)
                                    {
                                        if (CompoffApplicationObj.FromDateStatus == 1)  // Full day in single day
                                        {
                                            Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To " + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                                + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                            PopulateDropDown();
                                            CompOffApplicationNecessaryContent(EmpId);
                                            return View(CompOffModelObj);
                                        }
                                        else
                                        {
                                            if (ODTourTblObj.ODDayStatus == CompOffModelObj.FromDateStatus)
                                            {
                                                Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                                    + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                                    + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                                PopulateDropDown();
                                                CompOffApplicationNecessaryContent(EmpId);
                                                return View(CompOffModelObj);
                                            }
                                            else
                                            {
                                                if (CompOffModelObj.FromDateStatus == 1)
                                                {
                                                    Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                                        + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                                        + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                                    PopulateDropDown();
                                                    CompOffApplicationNecessaryContent(EmpId);
                                                    return View(CompOffModelObj);
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (CurrentDate == ODTourTblObj.FromDate)
                                    {
                                        if (ODTourTblObj.ODDayStatus == 1)  // Full day in single day
                                        {
                                            Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                                + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                                + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                            PopulateDropDown();
                                            CompOffApplicationNecessaryContent(EmpId);
                                            return View(CompOffModelObj);
                                        }
                                        else
                                        {
                                            if (ODTourTblObj.ODDayStatus == CompOffModelObj.FromDateStatus)
                                            {
                                                Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                                    + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                                    + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                                PopulateDropDown();
                                                CompOffApplicationNecessaryContent(EmpId);
                                                return View(CompOffModelObj);
                                            }
                                            else
                                            {
                                                if (CompOffModelObj.FromDateStatus == 1)
                                                {
                                                    Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                                        + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                                        + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                                    PopulateDropDown();
                                                    CompOffApplicationNecessaryContent(EmpId);
                                                    return View(CompOffModelObj);
                                                }
                                            }
                                        }
                                    }
                                    else if (CurrentDate == ODTourTblObj.ToDate)
                                    {
                                        if (ODTourTblObj.ODDayStatus == 1)  // Full day in single day
                                        {
                                            Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                                + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                                + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                            PopulateDropDown();
                                            CompOffApplicationNecessaryContent(EmpId);
                                            return View(CompOffModelObj);
                                        }
                                        else
                                        {
                                            if (ODTourTblObj.ODDayStatus == CompOffModelObj.FromDateStatus)
                                            {
                                                Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                                    + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                                    + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                                PopulateDropDown();
                                                CompOffApplicationNecessaryContent(EmpId);
                                                return View(CompOffModelObj);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Error("You Can not Apply Comp Off For " + CompOffModelObj.FromDate.Value.ToString("dd-MMM-yyyy") + " To "
                                            + CompOffModelObj.ToDate.Value.ToString("dd-MMM-yyyy") + " Range" + "<br/>"
                                            + "You may already have pending or approved/sanctioned Compoff for this date range.");

                                        PopulateDropDown();
                                        CompOffApplicationNecessaryContent(EmpId);
                                        return View(CompOffModelObj);
                                    }
                                }
                                // Code added by Rajas on 27 SEP 2017 END

                                #endregion Validate OD Data
                            }
                            // Added by Rajas on 10 MARCH 2017 for validating is compoff already present for same date range as of leave END
                            #endregion
                        }

                        #region VALIDATE FOR LOCKED DATA
                        // Added by Rajas on 10 MARCH 2017 to Validate if data is locked, can not apply leave START
                        DailyTransaction DailyTransactionListObj = WetosDB.DailyTransactions.Where(a => a.TranDate >= CompOffModelObj.FromDate && a.TranDate <= CompOffModelObj.ToDate && a.EmployeeId == EmpId && a.Lock == "Y").FirstOrDefault();

                        if (DailyTransactionListObj != null)
                        {
                            Error("You Can not apply Comp Off for this range as Data is Locked");

                            PopulateDropDown();
                            CompOffApplicationNecessaryContent(EmpId);
                            return View(CompOffModelObj);
                        }
                        // Added by Rajas on 10 MARCH 2017 to Validate if data is locked, can not apply leave END
                        #endregion

                        #endregion

                        int TotalWorkingHourTimeInMinuteInt = 0;
                        int DayCountCompOff = 0;
                        for (DateTime CurrentCODate = Convert.ToDateTime(CompOffModelObj.FromDate); CurrentCODate.Date <= CompOffModelObj.ToDate; CurrentCODate = CurrentCODate.AddDays(1))
                        {
                            DailyTransaction DailyTransactionObj = WetosDB.DailyTransactions.Where(a => a.EmployeeId == EmpId && a.TranDate == CurrentCODate).FirstOrDefault();
                            int WorkingHourTimeInMinuteInt = 0;
                            if (DailyTransactionObj != null)
                            {

                                Shift ShiftObj = WetosDB.Shifts.Where(a => a.ShiftCode == DailyTransactionObj.ShiftId && a.Company.CompanyId == EmployeeObj.CompanyId
                                    && a.BranchId == EmployeeObj.BranchId).FirstOrDefault();
                                if (ShiftObj != null)
                                {
                                    WorkingHourTimeInMinuteInt = (ShiftObj.WorkingHours.Hour * 60) + ShiftObj.WorkingHours.Minute;
                                }
                                else
                                {
                                    #region CODE NEED TO BE ADDED FOR DEFAULT CUMULATIVE COMP OFF WORKING HOURS INCASE OF NO SHIFTID AVAILABLE IN DAILYTRANSACTION TABLE
                                    int EmployeeGroupId = WetosDB.EmployeeGroupDetails.Where(a => a.Employee.EmployeeId == EmpId).Select(a => a.EmployeeGroup.EmployeeGroupId).FirstOrDefault();

                                    string DefaultWorkingHoursForCumulativeCompOff = WetosDB.RuleTransactions.Where(a => a.RuleId == 42 && a.EmployeeGroupId == EmployeeGroupId).Select(a => a.Formula).FirstOrDefault();
                                    if (!string.IsNullOrEmpty(DefaultWorkingHoursForCumulativeCompOff))
                                    {
                                        string[] DefaultWorkingHoursForCumulativeCompOffFullDayLimitArray = DefaultWorkingHoursForCumulativeCompOff.Split(':');
                                        int DefaultWorkingHoursForCumulativeCompOffHour = Convert.ToInt32(DefaultWorkingHoursForCumulativeCompOffFullDayLimitArray[0]);
                                        int DefaultWorkingHoursForCumulativeCompOffMinute = Convert.ToInt32(DefaultWorkingHoursForCumulativeCompOffFullDayLimitArray[1]);
                                        WorkingHourTimeInMinuteInt = (DefaultWorkingHoursForCumulativeCompOffHour * 60) + DefaultWorkingHoursForCumulativeCompOffMinute;
                                    }
                                    else
                                    {
                                        Error("Please set default working hours for cumulative comp off");


                                        PopulateDropDown();
                                        CompOffApplicationNecessaryContent(EmpId);
                                        return View(CompOffModelObj);
                                    }
                                    #endregion
                                }
                                if (CurrentCODate == CompOffModelObj.FromDate && CurrentCODate == CompOffModelObj.ToDate
                                    && (CompOffModelObj.FromDateStatus == 2 || CompOffModelObj.FromDateStatus == 2)
                                    && (CompOffModelObj.ToDateStatus == 2 || CompOffModelObj.ToDateStatus == 2))
                                {
                                    WorkingHourTimeInMinuteInt = Convert.ToInt32(WorkingHourTimeInMinuteInt / 2);
                                    DayCountCompOff = DayCountCompOff + 1;
                                }
                                else if (CurrentCODate == CompOffModelObj.FromDate && (CompOffModelObj.FromDateStatus == 2 || CompOffModelObj.FromDateStatus == 2))
                                {
                                    WorkingHourTimeInMinuteInt = Convert.ToInt32(WorkingHourTimeInMinuteInt / 2);
                                    DayCountCompOff = DayCountCompOff + 1;
                                }
                                else if (CurrentCODate == CompOffModelObj.ToDate && (CompOffModelObj.ToDateStatus == 2 || CompOffModelObj.ToDateStatus == 2))
                                {
                                    WorkingHourTimeInMinuteInt = Convert.ToInt32(WorkingHourTimeInMinuteInt / 2);
                                    DayCountCompOff = DayCountCompOff + 1;
                                }

                                //ADDED BY PUSHKAR ON 26 MAY 2018
                                else if (CurrentCODate == CompOffModelObj.FromDate && (CompOffModelObj.FromDateStatus == 3
                                || CompOffModelObj.FromDateStatus == 3))
                                {
                                    WorkingHourTimeInMinuteInt = Convert.ToInt32(WorkingHourTimeInMinuteInt / 2);
                                    DayCountCompOff = DayCountCompOff + 1;
                                }
                                else if (CurrentCODate == CompOffModelObj.ToDate && (CompOffModelObj.ToDateStatus == 3
                                    || CompOffModelObj.ToDateStatus == 3))
                                {
                                    WorkingHourTimeInMinuteInt = Convert.ToInt32(WorkingHourTimeInMinuteInt / 2);
                                    DayCountCompOff = DayCountCompOff + 1;
                                }

                                else
                                {
                                    DayCountCompOff = DayCountCompOff + 2;
                                }
                                TotalWorkingHourTimeInMinuteInt = TotalWorkingHourTimeInMinuteInt + WorkingHourTimeInMinuteInt;
                                //DayCountCompOff = DayCountCompOff + 2;

                            }
                            else
                            {
                                #region CODE NEED TO BE ADDED FOR DEFAULT CUMULATIVE COMP OFF WORKING HOURS INCASE OF NO SHIFTID AVAILABLE IN DAILYTRANSACTION TABLE
                                int EmployeeGroupId = WetosDB.EmployeeGroupDetails.Where(a => a.Employee.EmployeeId == EmpId).Select(a => a.EmployeeGroup.EmployeeGroupId).FirstOrDefault();

                                string DefaultWorkingHoursForCumulativeCompOff = WetosDB.RuleTransactions.Where(a => a.RuleId == 42 && a.EmployeeGroupId == EmployeeGroupId).Select(a => a.Formula).FirstOrDefault();
                                if (!string.IsNullOrEmpty(DefaultWorkingHoursForCumulativeCompOff))
                                {
                                    string[] DefaultWorkingHoursForCumulativeCompOffFullDayLimitArray = DefaultWorkingHoursForCumulativeCompOff.Split(':');
                                    int DefaultWorkingHoursForCumulativeCompOffHour = Convert.ToInt32(DefaultWorkingHoursForCumulativeCompOffFullDayLimitArray[0]);
                                    int DefaultWorkingHoursForCumulativeCompOffMinute = Convert.ToInt32(DefaultWorkingHoursForCumulativeCompOffFullDayLimitArray[1]);
                                    WorkingHourTimeInMinuteInt = (DefaultWorkingHoursForCumulativeCompOffHour * 60) + DefaultWorkingHoursForCumulativeCompOffMinute;
                                }
                                else
                                {
                                    Error("Please set default working hours for cumulative comp off");

                                    PopulateDropDown();
                                    CompOffApplicationNecessaryContent(EmpId);
                                    return View(CompOffModelObj);
                                }
                                #endregion

                                if (CurrentCODate == CompOffModelObj.FromDate && CurrentCODate == CompOffModelObj.ToDate
                                    && (CompOffModelObj.FromDateStatus == 2 || CompOffModelObj.FromDateStatus == 2)
                                    && (CompOffModelObj.ToDateStatus == 2 || CompOffModelObj.ToDateStatus == 2))
                                {
                                    WorkingHourTimeInMinuteInt = Convert.ToInt32(WorkingHourTimeInMinuteInt / 2);
                                    DayCountCompOff = DayCountCompOff + 1;
                                }
                                else if (CurrentCODate == CompOffModelObj.FromDate && (CompOffModelObj.FromDateStatus == 2
                                    || CompOffModelObj.FromDateStatus == 2))
                                {
                                    WorkingHourTimeInMinuteInt = Convert.ToInt32(WorkingHourTimeInMinuteInt / 2);
                                    DayCountCompOff = DayCountCompOff + 1;
                                }
                                else if (CurrentCODate == CompOffModelObj.ToDate && (CompOffModelObj.ToDateStatus == 2
                                    || CompOffModelObj.ToDateStatus == 2))
                                {
                                    WorkingHourTimeInMinuteInt = Convert.ToInt32(WorkingHourTimeInMinuteInt / 2);
                                    DayCountCompOff = DayCountCompOff + 1;
                                }

                                //ADDED BY PUSHKAR ON 26 MAY 2018
                                else if (CurrentCODate == CompOffModelObj.FromDate && (CompOffModelObj.FromDateStatus == 3
                                || CompOffModelObj.FromDateStatus == 3))
                                {
                                    WorkingHourTimeInMinuteInt = Convert.ToInt32(WorkingHourTimeInMinuteInt / 2);
                                    DayCountCompOff = DayCountCompOff + 1;
                                }
                                else if (CurrentCODate == CompOffModelObj.ToDate && (CompOffModelObj.ToDateStatus == 3
                                    || CompOffModelObj.ToDateStatus == 3))
                                {
                                    WorkingHourTimeInMinuteInt = Convert.ToInt32(WorkingHourTimeInMinuteInt / 2);
                                    DayCountCompOff = DayCountCompOff + 1;
                                }

                                else
                                {
                                    DayCountCompOff = DayCountCompOff + 2;
                                }

                                TotalWorkingHourTimeInMinuteInt = TotalWorkingHourTimeInMinuteInt + WorkingHourTimeInMinuteInt;
                                //DayCountCompOff = DayCountCompOff + 2;
                            }
                        }

                        //ADDED BY PUSHKAR ON 24 APRIL 2018
                        TotalWorkingHourTimeInMinuteInt = (240 * DayCountCompOff);

                        // ADDED BY MSJ on 18 APRIL 2018 START
                        //if (CompOffModelObj.FromDateStatus == 1)
                        //{
                        //    TotalWorkingHourTimeInMinuteInt = 480;
                        //}
                        //else
                        //{
                        //    TotalWorkingHourTimeInMinuteInt = 240;
                        //}
                        // ADDED BY MSJ on 18 APRIL 2018 END

                        string TtlAllwdDys = Collection["TtlAllwdDys"];
                        string[] TtlAllwdDysSplitValue = TtlAllwdDys.Split(':');
                        int TotalAllowedDaysHourInt = Convert.ToInt32(TtlAllwdDysSplitValue[0]);
                        int TotalAllowedDaysMinuteInt = Convert.ToInt32(TtlAllwdDysSplitValue[1]);
                        int TotalAllowedTimeInt = (60 * TotalAllowedDaysHourInt) + TotalAllowedDaysMinuteInt;

                        if (TotalWorkingHourTimeInMinuteInt > TotalAllowedTimeInt)
                        {

                            ModelState.AddModelError("", "You Can't Apply Comp Off for more than allowed days");
                            CompOffApplicationNecessaryContent(EmpId);
                            PopulateDropDown();
                            //List<CompOffApplication> CompOffApplicationList = WetosDB.CompOffApplications.Where(a => a.EmployeeId == EmployeeNo).ToList();
                            return View(CompOffModelObj);

                        }


                        CumulativeCompOffApplication COMPOFFAPPLICATIONObj = new CumulativeCompOffApplication();

                        //added by shraddha on 10 jan 2017 
                        Employee EmployeeDetailsObj = WetosDB.Employees.Where(a => a.EmployeeId == EmpId).FirstOrDefault();
                        COMPOFFAPPLICATIONObj.BranchId = EmployeeDetailsObj.BranchId;
                        COMPOFFAPPLICATIONObj.CompanyId = EmployeeDetailsObj.CompanyId;
                        COMPOFFAPPLICATIONObj.CompOffApplicationId = CompOffModelObj.CompOffApplicationId;

                        //added by shraddha on 10 jan 2017 
                        COMPOFFAPPLICATIONObj.EmployeeId = EmpId;
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

                        WetosDB.CumulativeCompOffApplications.Add(COMPOFFAPPLICATIONObj);

                        WetosDB.SaveChanges();
                        //ADDED BY PUSHKAR ON 24 APRIL 2018
                        int OldUsedMin = 0;
                        int CurrentUsedMin = 0;

                        foreach (int CheckCompOffIdObj in CheckCompOffIdList)
                        {
                            if (TotalWorkingHourTimeInMinuteInt > 0)
                            {
                                CumulativeCompOff CompOffObjForUpdateStatus = WetosDB.CumulativeCompOffs.Where(a => a.CompOffId == CheckCompOffIdObj).FirstOrDefault();
                                CompOffObjForUpdateStatus.AppStatus = "P";
                                CompOffObjForUpdateStatus.CoDate = Convert.ToDateTime(CompOffModelObj.FromDate);
                                CompOffObjForUpdateStatus.CompOffApplicationID = COMPOFFAPPLICATIONObj.CompOffApplicationId;
                                //ADDED BY PUSHKAR ON 24 APRIL 2018
                                OldUsedMin = CompOffObjForUpdateStatus.BalanceCoHours == null ? 0 : CompOffObjForUpdateStatus.BalanceCoHours.Value;


                                if (CompOffObjForUpdateStatus.BalanceCoHours > TotalWorkingHourTimeInMinuteInt)
                                {
                                    //CompOffObjForUpdateStatus.BalanceCoHours = CompOffObjForUpdateStatus.BalanceCoHours - TotalWorkingHourTimeInMinuteInt; //CODE ADDED BY SHRADDHA ON 20 MAR 2018
                                    CompOffObjForUpdateStatus.BalanceCoHours = CompOffObjForUpdateStatus.BalanceCoHours - TotalWorkingHourTimeInMinuteInt; //CODE ADDED BY SHRADDHA ON 20 MAR 2018
                                }
                                else
                                {
                                    //CompOffObjForUpdateStatus.BalanceCoHours = TotalWorkingHourTimeInMinuteInt - CompOffObjForUpdateStatus.BalanceCoHours;
                                    CompOffObjForUpdateStatus.BalanceCoHours = 0;
                                }

                                //ADDED BY PUSHKAR ON 24 APRIL 2018
                                CurrentUsedMin = CompOffObjForUpdateStatus.BalanceCoHours == null ? 0 : CompOffObjForUpdateStatus.BalanceCoHours.Value;

                                //TotalWorkingHourTimeInMinuteInt = Convert.ToInt32(TotalWorkingHourTimeInMinuteInt - (CompOffObjForUpdateStatus.CoHours - CompOffObjForUpdateStatus.BalanceCoHours));
                                //ADDED BY PUSHKAR ON 24 APRIL 2018
                                TotalWorkingHourTimeInMinuteInt = Convert.ToInt32(TotalWorkingHourTimeInMinuteInt - (OldUsedMin - CurrentUsedMin));

                                CompOffObjForUpdateStatus.UsedCoHours = CompOffObjForUpdateStatus.CoHours - CompOffObjForUpdateStatus.BalanceCoHours;

                                int? BalString = OldUsedMin - CurrentUsedMin;

                                CompOffObjForUpdateStatus.Remark = BalString.ToString();

                                WetosDB.SaveChanges();
                            }
                        }

                        // GTE LIST OF COMP WTH COMPOFFAPPID
                        List<CumulativeCompOff> CompOffList = WetosDB.CumulativeCompOffs.Where(a => a.CompOffApplicationID == COMPOFFAPPLICATIONObj.CompOffApplicationId).ToList();

                        // GET COMP OFF APPLICATION OBJ FROM CompOffApplicationId
                        CumulativeCompOffApplication CompOffApplicationExistingObj = WetosDB.CumulativeCompOffApplications.Where(a => a.CompOffApplicationId
                            == COMPOFFAPPLICATIONObj.CompOffApplicationId).FirstOrDefault();


                        if (CompOffApplicationExistingObj != null)
                        {

                            //ADDED BY MSJ ON 20 MARCH 2018 START
                            List<string> CompOffStringList = CompOffList.Select(s => s.CompOffId.ToString()).ToList();
                            //ADDED BY PUSHKAR ON 24 APRIL 2018
                            List<string> CompOffStringBal = new List<string>();

                            foreach (CumulativeCompOff CompOffListAdd in CompOffList)
                            {
                                CompOffListAdd.Remark = CompOffListAdd.CompOffId.ToString() + "-" + CompOffListAdd.Remark;
                                CompOffStringBal.Add(CompOffListAdd.Remark);
                                CompOffListAdd.Remark = "";
                            }


                            //string joined = string.Join(",", CompOffStringList);
                            string joined = string.Join(",", CompOffStringBal);
                            CompOffApplicationExistingObj.CompOffId = joined;
                            WetosDB.SaveChanges();
                            //ADDED BY MSJ ON 20 MARCH 2018 END

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
                        AddAuditTrail("Success - New CompOff application created successfully");  // Updated by Rajas on 17 JAN 2017

                        // Added by Rajas on 17 JAN 2017 START
                        Success("Success - New CompOff application created successfully");

                        if (MySelf == true)
                        {
                            return RedirectToAction("CompOffApplicationIndex");
                        }
                        else
                        {
                            return RedirectToAction("CompOffApplicationIndex", new { IsMySelf = "false" });
                        }


                    }
                    else
                    {
                        CompOffApplicationNecessaryContent(EmpId);

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

                    AddAuditTrail("Error - New CompOff application failed");  // Updated by Rajas on 17 JAN 2017

                    // Added by Rajas on 17 JAN 2017 START
                    Error("No Pending comp off for Employee: " + EmployeeObj.FirstName + " " + EmployeeObj.LastName);

                    PopulateDropDown();

                    return View(CompOffModelObj);

                }
            }
            catch (System.Exception ex)
            {
                CompOffApplicationNecessaryContent(EmpId);

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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult CumulativeCompOffIndex(int selectCriteria = 0)
        {
            try
            {
                int EmpNo = Convert.ToInt32(Session["EmployeeNo"]);
                int Id = Convert.ToInt32(Session["Id"]);

                // Added by Rajas on 9 MAY 2017 START
                // Select Criteria as per dropdown value
                int Status = 0;

                // Updated by Rajas on 20 MAY 2017 START
                List<SP_WetosGetEmployeeCumulativeCompOffSanction_Result> CompOffSanctionObj = new List<SP_WetosGetEmployeeCumulativeCompOffSanction_Result>();

                #region UPDATED BY RAJAS ON 23 MAY 2017
                // Get current FY from global setting
                //GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == "Current Financial Year").FirstOrDefault();

                //ABOVE LINE COMMENTED AND BELOW LINE IS ADDED BY SHALAKA ON 13TH DEC 2017
                GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.CurrentFinancialYear).FirstOrDefault();

                // Error
                if (GlobalSettingObj == null)
                {
                    Error("Inconsistent Financial year data");

                    //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 START
                    PopulateDropDown();
                    //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 END

                    return View(CompOffSanctionObj);
                }

                DateTime CalanderStartDate = GetFYStartDate(); // DateTime CalanderStartDate = WetosDB.FinancialYears.Where(a => a.FinancialName == GlobalSettingObj.SettingValue).Select(a => a.StartDate).FirstOrDefault();

                if (CalanderStartDate != null)
                {
                    //CODE REPLACED BY SHRADDHA ON 12 SEP 2017 REMOVED REDUNDANT AND UNNECESSARY IF ELSE CODE AND TAKEN ONE SINGLE LINE CODE START
                    Status = selectCriteria;
                    CompOffSanctionObj = WetosDB.SP_WetosGetEmployeeCumulativeCompOffSanction(EmpNo).Where(a => ((a.EmployeeReportingId == EmpNo || a.EmployeeReportingId2 == EmpNo) && a.Id == Status && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null)).OrderByDescending(a => a.FromDate).OrderByDescending(a => a.ToDate).ToList();
                    //CODE REPLACED BY SHRADDHA ON 12 SEP 2017 REMOVED REDUNDANT AND UNNECESSARY IF ELSE CODE AND TAKEN ONE SINGLE LINE CODE END


                }
                #endregion

                // Updated by Rajas on 20 MAY 2017 END
                ViewBag.Status = Status;
                ViewBag.SelectedStatus = selectCriteria;
                // Added by Rajas on 9 MAY 2017 END



                #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //var EmployeeData = WetosDB.VwActiveEmployees.ToList();
                List<SP_VwActiveEmployee_Result> EmployeeData = WetosDB.SP_VwActiveEmployee(EmployeeId).ToList();
                #endregion
                // List<SP_WetosGetEmployeeLeaveSanction_Result> LeaveSanctionObj = WetosDB.SP_WetosGetEmployeeLeaveSanction(EmpNo).OrderByDescending(a => a.FromDate).OrderByDescending(a => a.ToDate).ToList();

                // Added by Rajas on 9 MAY 2017
                // Populate Status dropdown
                //REPLACED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 START
                //List<string> StatusObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 3).Select(a => a.Text).ToList();
                //ViewBag.StatusList = StatusObj;
                PopulateDropDown();
                //REPLACED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 END

                return View(CompOffSanctionObj);
            }
            catch
            {
                //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 START
                PopulateDropDown();
                //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 END
                return RedirectToAction("EmployeeDashboard", "Dashboard");
            }
        }

        [HttpPost]
        public ActionResult CumulativeCOSanctionPV(int selectCriteria = 0)
        {
            try
            {
                int EmpNo = Convert.ToInt32(Session["EmployeeNo"]);
                int Id = Convert.ToInt32(Session["Id"]);

                // Added by Rajas on 9 MAY 2017 START
                // Select Criteria as per dropdown value
                int Status = 0;

                // Updated by Rajas on 20 MAY 2017 START
                List<SP_WetosGetEmployeeCumulativeCompOffSanction_Result> CompOffSanctionObj = new List<SP_WetosGetEmployeeCumulativeCompOffSanction_Result>();

                #region UPDATED BY RAJAS ON 23 MAY 2017
                // Get current FY from global setting
                //GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == "Current Financial Year").FirstOrDefault();

                //ABOVE LINE COMMENTED AND BELOW LINE IS ADDED BY SHALAKA ON 13TH DEC 2017
                GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.CurrentFinancialYear).FirstOrDefault();


                // Error
                if (GlobalSettingObj == null)
                {
                    Error("Inconsistent Financial year data");
                    //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 START
                    PopulateDropDown();
                    //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 END

                    return View(CompOffSanctionObj);
                }

                DateTime CalanderStartDate = GetFYStartDate(); // DateTime CalanderStartDate = WetosDB.FinancialYears.Where(a => a.FinancialName == GlobalSettingObj.SettingValue).Select(a => a.StartDate).FirstOrDefault();

                if (CalanderStartDate != null)
                {
                    //CODE REPLACED BY SHRADDHA ON 12 SEP 2017 REMOVED REDUNDANT AND UNNECESSARY IF ELSE CODE AND TAKEN ONE SINGLE LINE CODE START
                    //Status = selectCriteria;
                    //CompOffSanctionObj = WetosDB.SP_WetosGetEmployeeCompOffSanction(EmpNo).Where(a => ((a.EmployeeReportingId == EmpNo) && a.Id == Status && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null)).OrderByDescending(a => a.FromDate).OrderByDescending(a => a.ToDate).ToList();
                    //CODE REPLACED BY SHRADDHA ON 12 SEP 2017 REMOVED REDUNDANT AND UNNECESSARY IF ELSE CODE AND TAKEN ONE SINGLE LINE CODE END

                    // Above code commented. Error was list for reportingId2 was not displayed
                    // Code updated by Rajas on 22 SEP 2017 START
                    if (selectCriteria == 1)  // Pending
                    {
                        Status = 1;

                        // Updated by Rajas on 7 JUNE 2017, ( || a.EmployeeReportingId2 == EmpNo) removed
                        //CompOffSanctionObj = WetosDB.SP_WetosGetEmployeeCumulativeCompOffSanction(EmpNo).Where(a => ((a.EmployeeReportingId == EmpNo) && a.Id == Status
                        //    && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null))
                        //    .OrderByDescending(a => a.FromDate).ToList();  // && a.Id == 4)
                        CompOffSanctionObj = WetosDB.SP_WetosGetEmployeeCumulativeCompOffSanction(EmpNo).Where(a => ((a.EmployeeReportingId == EmpNo)
                            && a.Id == Status
                            //&& a.EmployeeId == 896 
                            //&& a.ExtrasHoursDate == "2018/03/28" 
                            && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null))
                            .OrderByDescending(a => a.FromDate).ToList();  // && a.Id == 4)

                    }
                    else if (selectCriteria == 2)  // Sanction
                    {
                        Status = 2;

                        CompOffSanctionObj = WetosDB.SP_WetosGetEmployeeCumulativeCompOffSanction(EmpNo)
                            .Where(a => ((a.EmployeeReportingId == EmpNo || a.EmployeeReportingId2 == EmpNo) && a.Id == Status && a.FromDate >= CalanderStartDate
                                && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null))
                            .OrderByDescending(a => a.FromDate).ToList();  // && a.Id == 4)
                    }
                    else if (selectCriteria == 3) // Rejected by Approver
                    {
                        Status = 3;

                        CompOffSanctionObj = WetosDB.SP_WetosGetEmployeeCumulativeCompOffSanction(EmpNo)
                            .Where(a => ((a.EmployeeReportingId == EmpNo || a.EmployeeReportingId2 == EmpNo)
                                && a.Id == Status && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null) || (a.EmployeeReportingId2 == EmpNo
                                && a.Id == Status && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null))
                                .OrderByDescending(a => a.FromDate).ToList();  // && a.Id == 4)
                    }
                    else if (selectCriteria == 4)  // Approve
                    {
                        Status = 4;

                        CompOffSanctionObj = WetosDB.SP_WetosGetEmployeeCumulativeCompOffSanction(EmpNo)
                            .Where(a => ((a.EmployeeReportingId2 == EmpNo || a.EmployeeReportingId == EmpNo)
                                && a.Id == Status && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null))
                                .OrderByDescending(a => a.FromDate).ToList();  // && a.Id == 4)
                    }
                    else if (selectCriteria == 5)  // Cancel
                    {
                        Status = 5;

                        CompOffSanctionObj = WetosDB.SP_WetosGetEmployeeCumulativeCompOffSanction(EmpNo)
                            .Where(a => ((a.EmployeeReportingId == EmpNo || a.EmployeeReportingId2 == EmpNo) && a.Id == Status
                                && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null))
                                .OrderByDescending(a => a.FromDate).ToList();  // && a.Id == 4)
                    }
                    else  // Rejected by Sanctioner
                    {
                        Status = 6;
                        CompOffSanctionObj = WetosDB.SP_WetosGetEmployeeCumulativeCompOffSanction(EmpNo)
                            .Where(a => ((a.EmployeeReportingId2 == EmpNo) && a.Id == Status
                                && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null))
                            .OrderByDescending(a => a.FromDate).ToList();  // && a.Id == 4)
                    }
                    // Code updated by Rajas on 22 SEP 2017 END


                }
                #endregion

                // Updated by Rajas on 20 MAY 2017 END
                ViewBag.Status = Status;
                ViewBag.SelectedStatus = selectCriteria;
                // Added by Rajas on 9 MAY 2017 END



                #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //var EmployeeData = WetosDB.VwActiveEmployees.ToList();
                List<SP_VwActiveEmployee_Result> EmployeeData = WetosDB.SP_VwActiveEmployee(EmployeeId).ToList();
                #endregion
                // List<SP_WetosGetEmployeeLeaveSanction_Result> LeaveSanctionObj = WetosDB.SP_WetosGetEmployeeLeaveSanction(EmpNo).OrderByDescending(a => a.FromDate).OrderByDescending(a => a.ToDate).ToList();

                // Added by Rajas on 9 MAY 2017
                // Populate Status dropdown
                //REPLACED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 START
                //List<string> StatusObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 3).Select(a => a.Text).ToList();
                //ViewBag.StatusList = StatusObj;
                PopulateDropDown();
                //REPLACED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 END

                return PartialView(CompOffSanctionObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in Leave Sanction due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Error in Leave Sanction due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 START
                PopulateDropDown();
                //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 END

                return RedirectToAction("EmployeeDashboard", "Dashboard");
            }
        }

        /// <summary>
        /// MODIFIED CODE BY SHRADDHA ON 10 MARCH 2017 FOR CompOffSANCTION POST
        /// </summary>
        /// <param name="CompOffSanction"></param>
        /// <param name="FC"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CumulativeCompOffIndex(List<SP_WetosGetEmployeeCumulativeCompOffSanction_Result> CompOffSanction, FormCollection FC)
        {
            #region TRY BLOCK
            try
            {
                int EmpNo = Convert.ToInt32(Session["EmployeeNo"]);

                //FAKE LOGIC FOR FILTERING
                #region FAKE LOGIC FOR FILTERING

                CompOffSanction = null;

                #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //var EmployeeData = WetosDB.Employees.ToList();
                List<SP_VwActiveEmployee_Result> EmployeeData = WetosDB.SP_VwActiveEmployee(EmployeeId).ToList();
                #endregion

                List<SP_WetosGetEmployeeCumulativeCompOffSanction_Result> CompOffSanctionObjForCount = new List<SP_WetosGetEmployeeCumulativeCompOffSanction_Result>();
                string RejectReasonText = FC["RejectReasonText"];
                if (CompOffSanction == null)
                {
                    CompOffSanction = new List<SP_WetosGetEmployeeCumulativeCompOffSanction_Result>();
                    int SingleApplicationID = 0;
                    int SingleStatusID = 0;
                    bool StatusIdSelected = false;
                    foreach (var key in FC.AllKeys)
                    {
                        string KeyStr = key.ToString();
                        if (KeyStr.Contains("StatusId") == true)
                        {
                            SingleStatusID = Convert.ToInt32(FC[KeyStr]);
                            string TempKeyStr = KeyStr.Replace("StatusId", "CompOffApplicationId");
                            SingleApplicationID = Convert.ToInt32(FC[TempKeyStr]);
                            SP_WetosGetEmployeeCumulativeCompOffSanction_Result TesmpCOSanction = WetosDB.SP_WetosGetEmployeeCumulativeCompOffSanction(EmpNo).Where(a => a.CompOffApplicationId == SingleApplicationID).FirstOrDefault(); //.ToList();
                            if (TesmpCOSanction != null)
                            {
                                TesmpCOSanction.StatusId = SingleStatusID;// UPDATE STATUS
                                CompOffSanction.Add(TesmpCOSanction);
                            }
                            StatusIdSelected = true;
                        }
                    }

                    if (StatusIdSelected == false)
                    {
                        Information("Please select atleast one entry for processing");
                        PopulateDropDown();
                        return View(CompOffSanctionObjForCount);
                    }

                }

                #endregion

                //GET LIST OF APPLICATIONS FOR WHICH STATUS REJECTED BY APPROVER / REJECTED BY SANCTIONER / CANCEL IS SELECTED
                CompOffSanctionObjForCount = CompOffSanction.Where(a => a.StatusId == 3 || a.StatusId == 5 || a.StatusId == 6).ToList();


                #region CHECK WHETHER HAVING CO AS LEAVE OR NOT
                GlobalSetting IsHavingCOLeave = WetosDB.GlobalSettings.Where(a => a.SettingText == "IsHavingCOLeave").FirstOrDefault();
                #endregion


                #region REAL LOGIC

                #region ENTER REJECT/CANCEL REASON VALIDATION
                if (CompOffSanctionObjForCount.Count() > 0 && (RejectReasonText == null || RejectReasonText == ""))
                {
                    ModelState.AddModelError("CustomError", "Please Enter Reject/cancel Reason");
                    List<SP_WetosGetEmployeeCumulativeCompOffSanction_Result> CompOffSanctionList = WetosDB.SP_WetosGetEmployeeCumulativeCompOffSanction(EmpNo).OrderByDescending(a => a.FromDate).OrderByDescending(a => a.ToDate).ToList();
                    PopulateDropDown();
                    return View(CompOffSanctionObjForCount);
                }
                #endregion


                else
                {
                    foreach (var i in CompOffSanctionObjForCount)
                    {
                        CumulativeCompOffApplication COMPOFFAPPLICATIONObj = WetosDB.CumulativeCompOffApplications.Where(a => a.CompOffApplicationId == i.CompOffApplicationId).FirstOrDefault();
                        COMPOFFAPPLICATIONObj.RejectReason = RejectReasonText;
                        WetosDB.SaveChanges();
                    }

                    foreach (SP_WetosGetEmployeeCumulativeCompOffSanction_Result item in CompOffSanction)
                    {

                        #region TOTAL ALLOWED COMP OFF MINUTES
                        int TotalWorkingHours = 0;
                        SP_VwActiveEmployee_Result EmployeeObj = EmployeeData.Where(a => a.EmployeeId == item.EmployeeId).FirstOrDefault();
                        DateTime CompOffFromDate = Convert.ToDateTime(item.FromDate);
                        DateTime CompOffToDate = Convert.ToDateTime(item.ToDate);
                        for (DateTime CompOffDate = CompOffFromDate; CompOffDate <= CompOffToDate; CompOffDate = CompOffDate.AddDays(1))
                        {
                            int TotalWorkingHoursLocalRegion = 0;
                            #region  FIND NEAREST SHIFT FROM IN OUT AND MARK LATE / EARLY
                            string ShiftId = string.Empty;
                            Shift ShiftObjForCurrentEmployee = new Shift();

                            #region GET DAILY TRANSACTION TABLE RECORD FOR SELECTED DATE OF SELECTED EMPLOYEE
                            DailyTransaction DailyTransactionObj = WetosDB.DailyTransactions.Where(a => a.TranDate == CompOffDate
                                && a.EmployeeId == item.EmployeeId).FirstOrDefault();
                            #endregion
                            if (DailyTransactionObj != null)
                            {
                                #region IF DATA AVAILABLE IN DAILY TRANSACTION TABLE THEN TAKE SHIFT FROM DT TABLE
                                ShiftId = DailyTransactionObj.ShiftId;
                                #endregion

                                if (ShiftId != null)
                                {
                                    ShiftObjForCurrentEmployee = WetosDB.Shifts.Where(a => a.ShiftCode == ShiftId).FirstOrDefault();
                                    if (ShiftObjForCurrentEmployee != null)
                                    {
                                        int TotalAllowedDaysHourInt = ShiftObjForCurrentEmployee.WorkingHours.Hour;
                                        int TotalAllowedDaysMinuteInt = ShiftObjForCurrentEmployee.WorkingHours.Minute;
                                        TotalWorkingHoursLocalRegion = (60 * TotalAllowedDaysHourInt) + TotalAllowedDaysMinuteInt;
                                    }
                                    else
                                    {

                                    }
                                }
                                else
                                {
                                    #region CODE NEED TO BE ADDED FOR DEFAULT CUMULATIVE COMP OFF WORKING HOURS INCASE OF NO SHIFTID AVAILABLE IN DAILYTRANSACTION TABLE
                                    int EmployeeGroupId = WetosDB.EmployeeGroupDetails.Where(a => a.Employee.EmployeeId == item.EmployeeId).Select(a => a.EmployeeGroup.EmployeeGroupId).FirstOrDefault();

                                    string DefaultWorkingHoursForCumulativeCompOff = WetosDB.RuleTransactions.Where(a => a.RuleId == 42 && a.EmployeeGroupId == EmployeeGroupId).Select(a => a.Formula).FirstOrDefault();
                                    if (!string.IsNullOrEmpty(DefaultWorkingHoursForCumulativeCompOff))
                                    {
                                        string[] DefaultWorkingHoursForCumulativeCompOffFullDayLimitArray = DefaultWorkingHoursForCumulativeCompOff.Split(':');
                                        int DefaultWorkingHoursForCumulativeCompOffHour = Convert.ToInt32(DefaultWorkingHoursForCumulativeCompOffFullDayLimitArray[0]);
                                        int DefaultWorkingHoursForCumulativeCompOffMinute = Convert.ToInt32(DefaultWorkingHoursForCumulativeCompOffFullDayLimitArray[1]);
                                        TotalWorkingHoursLocalRegion = (DefaultWorkingHoursForCumulativeCompOffHour * 60) + DefaultWorkingHoursForCumulativeCompOffMinute;
                                    }
                                    else
                                    {
                                        VwActiveEmployee ValidEmpObj2 = WetosDB.VwActiveEmployees.Where(a => a.EmployeeId == item.EmployeeId).FirstOrDefault();

                                        Error("Please set default working hours for cumulative comp off " + item.FromDate.Value.ToString("dd/MMM/yyyy") + " - " + item.ToDate.Value.ToString("dd/MMM/yyyy") + " For EmployeeCode: " + ValidEmpObj2.EmployeeCode + "-" + ValidEmpObj2.FirstName + " " + ValidEmpObj2.MiddleName + " " + ValidEmpObj2.LastName);
                                        PopulateDropDown();
                                        return View(CompOffSanctionObjForCount);

                                    }
                                    #endregion
                                }
                            }

                            else
                            {
                                #region IF DATA AVAILABLE IN DAILY TRANSACTION TABLE THEN TAKE SHIFT FROM DEFAULT SHIFT
                                #region CODE NEED TO BE ADDED FOR DEFAULT CUMULATIVE COMP OFF WORKING HOURS INCASE OF NO SHIFTID AVAILABLE IN DAILYTRANSACTION TABLE
                                int EmployeeGroupId = WetosDB.EmployeeGroupDetails.Where(a => a.Employee.EmployeeId == item.EmployeeId).Select(a => a.EmployeeGroup.EmployeeGroupId).FirstOrDefault();

                                string DefaultWorkingHoursForCumulativeCompOff = WetosDB.RuleTransactions.Where(a => a.RuleId == 42 && a.EmployeeGroupId == EmployeeGroupId).Select(a => a.Formula).FirstOrDefault();
                                if (!string.IsNullOrEmpty(DefaultWorkingHoursForCumulativeCompOff))
                                {
                                    string[] DefaultWorkingHoursForCumulativeCompOffFullDayLimitArray = DefaultWorkingHoursForCumulativeCompOff.Split(':');
                                    int DefaultWorkingHoursForCumulativeCompOffHour = Convert.ToInt32(DefaultWorkingHoursForCumulativeCompOffFullDayLimitArray[0]);
                                    int DefaultWorkingHoursForCumulativeCompOffMinute = Convert.ToInt32(DefaultWorkingHoursForCumulativeCompOffFullDayLimitArray[1]);
                                    TotalWorkingHoursLocalRegion = (DefaultWorkingHoursForCumulativeCompOffHour * 60) + DefaultWorkingHoursForCumulativeCompOffMinute;
                                }
                                else
                                {
                                    VwActiveEmployee ValidEmpObj2 = WetosDB.VwActiveEmployees.Where(a => a.EmployeeId == item.EmployeeId).FirstOrDefault();

                                    Error("Please set default working hours for cumulative comp off " + item.FromDate.Value.ToString("dd/MMM/yyyy") + " - " + item.ToDate.Value.ToString("dd/MMM/yyyy") + " For EmployeeCode: " + ValidEmpObj2.EmployeeCode + "-" + ValidEmpObj2.FirstName + " " + ValidEmpObj2.MiddleName + " " + ValidEmpObj2.LastName);
                                    PopulateDropDown();
                                    return View(CompOffSanctionObjForCount);

                                }
                                #endregion
                                #endregion
                            }
                            #endregion


                            if (CompOffDate == CompOffFromDate && CompOffDate == CompOffToDate) //IF COMP OFF DATE IS FROM DATE
                            {
                                if (item.FromDateStatus == 3 || item.FromDateStatus == 2 || item.ToDateStatus == 3 || item.ToDateStatus == 2)
                                {
                                    TotalWorkingHoursLocalRegion = Convert.ToInt32(TotalWorkingHoursLocalRegion / 2);
                                }

                            }
                            else
                            {
                                if (CompOffDate == CompOffFromDate) //IF COMP OFF DATE IS FROM DATE
                                {
                                    if (item.FromDateStatus == 3 || item.FromDateStatus == 2)
                                    {
                                        TotalWorkingHoursLocalRegion = Convert.ToInt32(TotalWorkingHoursLocalRegion / 2);
                                    }

                                }
                                if (CompOffDate == CompOffToDate) //IF COMP OFF DATE IS TO DATE
                                {
                                    if (item.ToDateStatus == 3 || item.ToDateStatus == 2)
                                    {
                                        TotalWorkingHoursLocalRegion = Convert.ToInt32(TotalWorkingHoursLocalRegion / 2);
                                    }

                                }
                            }
                            TotalWorkingHours = TotalWorkingHoursLocalRegion + TotalWorkingHours;
                        }
                        #endregion

                        CumulativeCompOffApplication COMPOFFAPPLICATIONObj = WetosDB.CumulativeCompOffApplications.Where(a => a.CompOffApplicationId == item.CompOffApplicationId).FirstOrDefault();

                        //CODE TO CHECK WHETHER EMPLOYEE IS VALID / ACTIVE OR NOT
                        VwActiveEmployee ValidEmpObj = WetosDB.VwActiveEmployees.Where(a => a.EmployeeId == COMPOFFAPPLICATIONObj.EmployeeId).FirstOrDefault();

                        //IF EMPLOYEE IS VALID / ACTIVE
                        if (ValidEmpObj != null)
                        {
                            //IF SELECTED STATUS FOR APPLICATION IS VALID
                            if (item.StatusId != 0)
                            {
                                #region PREVENT SANCTIONING CO WHICH IS ALREADY SANCTIONED
                                if (COMPOFFAPPLICATIONObj.StatusId == 2 && item.StatusId == 2)
                                {
                                    Error("Comp Off is already Sanction for Comp Off " + COMPOFFAPPLICATIONObj.FromDate.Value.ToString("dd/MMM/yyyy") + " - " + COMPOFFAPPLICATIONObj.ToDate.Value.ToString("dd/MMM/yyyy") + " For EmployeeCode: " + ValidEmpObj.EmployeeCode + "-" + ValidEmpObj.FirstName + " " + ValidEmpObj.MiddleName + " " + ValidEmpObj.LastName);
                                    PopulateDropDown();
                                    return View(CompOffSanctionObjForCount);
                                }
                                #endregion

                                //GET RECORD FROM COMP OFF TABLE ENTRY AGAINST WHICH COMP OFF IS APPLIED
                                //List<CompOff> CompOffSanctionListforcompoff = WetosDB.CompOffs.Where(a => a.EmployeeId == COMPOFFAPPLICATIONObj.EmployeeId && a.CompOffApplicationID == COMPOFFAPPLICATIONObj.CompOffApplicationId
                                //    && a.CoDate != null).ToList();
                                List<CumulativeCompOff> CompOffSanctionListforcompoff = new List<CumulativeCompOff>();
                                CumulativeCompOffApplication CompOffApplicationObj = WetosDB.CumulativeCompOffApplications.Where(a => a.CompOffApplicationId
                                    == COMPOFFAPPLICATIONObj.CompOffApplicationId && a.EmployeeId == COMPOFFAPPLICATIONObj.EmployeeId).FirstOrDefault();
                                if (CompOffApplicationObj.CompOffId != null)
                                {
                                    if (CompOffApplicationObj.CompOffId.Contains(","))
                                    {
                                        string[] CompOffIdArray = CompOffApplicationObj.CompOffId.Split(',');
                                        foreach (string CompOffId in CompOffIdArray)
                                        {
                                            string[] CompOffIdBreak = CompOffId.Split('-');
                                            string CompOffIdStr = CompOffIdBreak[0];

                                            //int CompOffIdInt = Convert.ToInt32(CompOffId);
                                            int CompOffIdInt = Convert.ToInt32(CompOffIdStr);
                                            CompOffSanctionListforcompoff.Add(WetosDB.CumulativeCompOffs.Where(a => a.CompOffId == CompOffIdInt).FirstOrDefault());
                                        }
                                    }
                                    else
                                    {
                                        string[] CompOffIdArray = CompOffApplicationObj.CompOffId.Split('-');
                                        //int CompOffIdInt = Convert.ToInt32(CompOffApplicationObj.CompOffId);
                                        int CompOffIdInt = Convert.ToInt32(CompOffIdArray[0]);
                                        CompOffSanctionListforcompoff.Add(WetosDB.CumulativeCompOffs.Where(a => a.CompOffId == CompOffIdInt).FirstOrDefault());
                                    }
                                }



                                #region SANCTION CO START
                                if (COMPOFFAPPLICATIONObj.StatusId != 2 && item.StatusId == 2)
                                {
                                    //IF LOGIN EMPLOYEE IS APPROVER AS WELL AS SANCTIONER
                                    #region IF LOGIN EMPLOYEE IS APPROVER AS WELL AS SANCTIONER
                                    if (ValidEmpObj.EmployeeReportingId2 == EmpNo || ValidEmpObj.EmployeeReportingId == EmpNo)
                                    {
                                        COMPOFFAPPLICATIONObj.StatusId = item.StatusId;
                                        COMPOFFAPPLICATIONObj.Status = WetosDB.DropdownDatas.Where(a => a.TypeId == 3 && a.Value == COMPOFFAPPLICATIONObj.StatusId).Select(a => a.Text).FirstOrDefault();
                                        COMPOFFAPPLICATIONObj.RejectReason = string.Empty;



                                        if (IsHavingCOLeave != null)
                                        {
                                            //IF CO LEAVE IS APPLICABLE
                                            #region IF CO LEAVE IS APPLICABLE
                                            if (IsHavingCOLeave.SettingValue == "1")
                                            {
                                                //GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == "Current Financial Year").FirstOrDefault();

                                                //ABOVE LINE COMMENTED AND BELOW LINE IS ADDED BY SHALAKA ON 13TH DEC 2017
                                                GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.CurrentFinancialYear).FirstOrDefault();

                                                if (GlobalSettingObj != null)
                                                {
                                                    FinancialYear GetFYObj = WetosDB.FinancialYears.Where(a => a.FinancialName == GlobalSettingObj.SettingValue).FirstOrDefault();
                                                    LeaveCredit LeaveCreditForCO = WetosDB.LeaveCredits.Where(a => a.EmployeeId == COMPOFFAPPLICATIONObj.EmployeeId && a.FinancialYearId == GetFYObj.FinancialYearId && a.LeaveType == "CO").FirstOrDefault();
                                                    LeaveBalance LeaveBalanceForCO = WetosDB.LeaveBalances.Where(a => a.EmployeeId == COMPOFFAPPLICATIONObj.EmployeeId && a.LeaveType == "CO").FirstOrDefault();

                                                    TimeSpan TS = (Convert.ToDateTime(COMPOFFAPPLICATIONObj.ToDate)) - (Convert.ToDateTime(COMPOFFAPPLICATIONObj.FromDate));
                                                    double TotalDays = TS.Days;

                                                    TotalDays = TotalDays + 1;

                                                    // Added by Rajas on 16 MAY 2017
                                                    double Availed = 0.0;
                                                    double Encash = 0.0;
                                                    double CarryForward = 0.0;

                                                    // If CO is not credited to Leavecredit for Current FY
                                                    if (LeaveCreditForCO == null)
                                                    {
                                                        // Code added by Rajas on 16 MAY 2017 START
                                                        LeaveCreditForCO = new LeaveCredit();

                                                        LeaveCreditForCO.FinancialYearId = GetFYObj.FinancialYearId;

                                                        LeaveCreditForCO.LeaveType = "CO";  // verify?

                                                        LeaveCreditForCO.OpeningBalance = TotalDays;

                                                        LeaveCreditForCO.Availed = Availed;

                                                        LeaveCreditForCO.CarryForward = CarryForward;

                                                        LeaveCreditForCO.Encash = Encash;

                                                        LeaveCreditForCO.CompanyId = item.CompanyId.Value;

                                                        LeaveCreditForCO.BranchId = item.BranchId.Value;

                                                        LeaveCreditForCO.EmployeeId = item.EmployeeId.Value;

                                                        WetosDB.LeaveCredits.Add(LeaveCreditForCO); // NEW CREDIT ENTRY FY

                                                        WetosDB.SaveChanges();

                                                        LeaveBalanceForCO.PreviousBalance = LeaveBalanceForCO.CurrentBalance;

                                                        LeaveBalanceForCO.CurrentBalance = LeaveBalanceForCO.CurrentBalance + TotalDays;

                                                        WetosDB.SaveChanges();
                                                    }
                                                    else
                                                    {
                                                        LeaveCreditForCO.OpeningBalance = LeaveCreditForCO.OpeningBalance + TotalDays;  // Update credit table

                                                        LeaveBalanceForCO.PreviousBalance = LeaveBalanceForCO.CurrentBalance;

                                                        LeaveBalanceForCO.CurrentBalance = LeaveBalanceForCO.CurrentBalance + TotalDays;  // update balance table

                                                        WetosDB.SaveChanges();
                                                    }
                                                }
                                                else
                                                {
                                                    Information("Inconsistent FY data detected, please verify and try again!");
                                                    List<SP_WetosGetEmployeeCumulativeCompOffSanction_Result> COSanctionObj = WetosDB.SP_WetosGetEmployeeCumulativeCompOffSanction(EmpNo).OrderByDescending(a => a.FromDate).OrderByDescending(a => a.ToDate).ToList();
                                                    PopulateDropDown();
                                                    return View(CompOffSanctionObjForCount);
                                                }
                                            }
                                            #region IF CO LEAVE IS NOT APPLICABLE
                                            //IF CO LEAVE IS NOT AVAILABLE
                                            else
                                            {
                                                #region UPDATE DAILYTRANSACTION STATUS START
                                                for (DateTime CurrentDate = Convert.ToDateTime(COMPOFFAPPLICATIONObj.FromDate); CurrentDate.Date <= Convert.ToDateTime(COMPOFFAPPLICATIONObj.ToDate); CurrentDate = CurrentDate.AddDays(1))
                                                {
                                                    DailyTransaction DailyTransactionObj = WetosDB.DailyTransactions.Where(a => a.EmployeeId == COMPOFFAPPLICATIONObj.EmployeeId && a.TranDate == CurrentDate).FirstOrDefault();
                                                    if (DailyTransactionObj != null)
                                                    {
                                                        DailyTransactionObj.ActualStatus = DailyTransactionObj.Status;
                                                        WetosDB.SaveChanges();
                                                    }
                                                }

                                                string ReturnMessage = string.Empty;
                                                if (WetosAdministrationController.CumulativeCOProcessingEx(WetosDB, Convert.ToDateTime(item.FromDate), Convert.ToDateTime(item.ToDate), Convert.ToInt32(COMPOFFAPPLICATIONObj.EmployeeId), ref ReturnMessage) == false)
                                                {
                                                    AddAuditTrail(ReturnMessage);
                                                }
                                                #endregion
                                            }
                                            #endregion
                                            #endregion
                                        }

                                        //IF CO LEAVE IS NOT APPLICABLE
                                        #region IF CO LEAVE IS NOT APPLICABLE
                                        //IF CO LEAVE IS NOT AVAILABLE
                                        else
                                        {
                                            #region UPDATE DAILYTRANSACTION STATUS START
                                            string ReturnMessage = string.Empty;
                                            for (DateTime CurrentDate = Convert.ToDateTime(COMPOFFAPPLICATIONObj.FromDate); CurrentDate.Date <= Convert.ToDateTime(COMPOFFAPPLICATIONObj.ToDate); CurrentDate = CurrentDate.AddDays(1))
                                            {
                                                DailyTransaction DailyTransactionObj = WetosDB.DailyTransactions.Where(a => a.EmployeeId == COMPOFFAPPLICATIONObj.EmployeeId && a.TranDate == CurrentDate).FirstOrDefault();
                                                if (DailyTransactionObj != null)
                                                {
                                                    DailyTransactionObj.ActualStatus = DailyTransactionObj.Status;
                                                    WetosDB.SaveChanges();
                                                }
                                            }
                                            if (WetosAdministrationController.CumulativeCOProcessingEx(WetosDB, Convert.ToDateTime(item.FromDate), Convert.ToDateTime(item.ToDate), Convert.ToInt32(COMPOFFAPPLICATIONObj.EmployeeId), ref ReturnMessage) == false)
                                            {
                                                AddAuditTrail(ReturnMessage);
                                            }
                                            #endregion
                                        }
                                        #endregion

                                        //IF COMP OFF ENTRY IS AVAILABLE IN COMP OFF TABLE AND COMP OFF IS BEING SANCTIONED THEN UPDATE APPSTATUS = S IN COMP OFF TABLE
                                        #region IF COMP OFF ENTRY IS AVAILABLE IN COMP OFF TABLE AND COMP OFF IS BEING SANCTIONED THEN UPDATE APPSTATUS = S IN COMP OFF TABLE
                                        if (CompOffSanctionListforcompoff.Count > 0)
                                        {
                                            foreach (CumulativeCompOff CompOffSanctionObjforcompoff in CompOffSanctionListforcompoff)
                                            {

                                                if (CompOffSanctionObjforcompoff != null)
                                                {
                                                    int? BalMinCount = CompOffSanctionObjforcompoff.BalanceCoHours == null ? 0 : CompOffSanctionObjforcompoff.BalanceCoHours;
                                                    if (BalMinCount == 0)
                                                    {
                                                        CompOffSanctionObjforcompoff.AppStatus = "S";

                                                        //    if (CompOffSanctionObjforcompoff.BalanceCoHours > TotalWorkingHours)
                                                        //    {
                                                        //        CompOffSanctionObjforcompoff.BalanceCoHours = CompOffSanctionObjforcompoff.BalanceCoHours - TotalWorkingHours; //CODE ADDED BY SHRADDHA ON 20 MAR 2018
                                                        //    }
                                                        //    else
                                                        //    {
                                                        //        CompOffSanctionObjforcompoff.BalanceCoHours = TotalWorkingHours - CompOffSanctionObjforcompoff.BalanceCoHours;
                                                        //    }
                                                        //    TotalWorkingHours = Convert.ToInt32(TotalWorkingHours - CompOffSanctionObjforcompoff.BalanceCoHours);
                                                    }
                                                    WetosDB.SaveChanges();
                                                }
                                            }
                                        }
                                        #endregion

                                    }
                                    #endregion

                                    #region IF LOGIN EMPLOYEE IS NOT APPROVER AS WELL AS SANCTIONER THEN PROVIDE ERROR MESSAGE
                                    else
                                    {
                                        Information("You can not sanction this CO entry as you are not sanctioner for selected employee");
                                        List<SP_WetosGetEmployeeCumulativeCompOffSanction_Result> COSanctionObj = WetosDB.SP_WetosGetEmployeeCumulativeCompOffSanction(EmpNo).OrderByDescending(a => a.FromDate).OrderByDescending(a => a.ToDate).ToList();
                                        PopulateDropDown();

                                        return View(CompOffSanctionObjForCount);
                                    }
                                    #endregion


                                }
                                #endregion

                                #region REJECT BY APPROVER / REJECT BY SANCTIONER / CANCEL / APPROVE CO START
                                else if (item.StatusId == 3 || item.StatusId == 6 || item.StatusId == 5 || item.StatusId == 4)
                                {
                                    if (IsHavingCOLeave != null)
                                    {
                                        //IF CO LEAVE IS APPLICABLE
                                        #region IF CO LEAVE IS APPLICABLE
                                        if (IsHavingCOLeave.SettingValue == "1")
                                        {
                                            COMPOFFAPPLICATIONObj.StatusId = item.StatusId;
                                            COMPOFFAPPLICATIONObj.Status = WetosDB.DropdownDatas.Where(a => a.TypeId == 3 && a.Value == COMPOFFAPPLICATIONObj.StatusId).Select(a => a.Text).FirstOrDefault();
                                            WetosDB.SaveChanges();
                                        }
                                        #endregion
                                        else
                                        {
                                            #region IF CO LEAVE IS NOT APPLICABLE OR GLOBAL SETTING VALUE IS NULL
                                            if (COMPOFFAPPLICATIONObj.StatusId == 2)
                                            {
                                                for (DateTime CurrentDate = Convert.ToDateTime(COMPOFFAPPLICATIONObj.FromDate); CurrentDate.Date <= Convert.ToDateTime(COMPOFFAPPLICATIONObj.ToDate); CurrentDate = CurrentDate.AddDays(1))
                                                {
                                                    DailyTransaction DailyTransactionObj = WetosDB.DailyTransactions.Where(a => a.EmployeeId == COMPOFFAPPLICATIONObj.EmployeeId && a.TranDate == CurrentDate).FirstOrDefault();
                                                    if (DailyTransactionObj != null)
                                                    {
                                                        DailyTransactionObj.Status = DailyTransactionObj.ActualStatus;
                                                        WetosDB.SaveChanges();
                                                    }
                                                }
                                            }

                                            if (CompOffSanctionListforcompoff.Count > 0)
                                            {

                                                List<string> CompOffIdDeleteSplit = new List<string>();

                                                List<string> CompOffIdSplit = COMPOFFAPPLICATIONObj.CompOffId.Split(',').ToList();

                                                foreach (string StrIdMin in CompOffIdSplit)
                                                {
                                                    List<string> StrIdMinList = StrIdMin.Split('-').ToList();
                                                    int CompOffIdStringConv = Convert.ToInt32(StrIdMinList[0]);

                                                    int ConsumedMinCompOff = Convert.ToInt32(StrIdMinList[1]);

                                                    CumulativeCompOff CompOffSanctionObjforcompoff = CompOffSanctionListforcompoff.Where(a => a.CompOffId == CompOffIdStringConv).FirstOrDefault();

                                                    if (CompOffSanctionObjforcompoff != null)
                                                    {
                                                        if (item.StatusId == 3 || item.StatusId == 6 || item.StatusId == 5)
                                                        {

                                                            CompOffSanctionObjforcompoff.AppStatus = null;
                                                            CompOffSanctionObjforcompoff.CoDate = null;
                                                            CompOffSanctionObjforcompoff.CompOffApplicationID = null;

                                                            CompOffSanctionObjforcompoff.BalanceCoHours = CompOffSanctionObjforcompoff.BalanceCoHours + ConsumedMinCompOff;

                                                            CompOffSanctionObjforcompoff.UsedCoHours = CompOffSanctionObjforcompoff.UsedCoHours - ConsumedMinCompOff;

                                                            WetosDB.SaveChanges();

                                                        }

                                                    }
                                                }
                                            }

                                            //if (CompOffSanctionListforcompoff.Count > 0)
                                            //{
                                            //    foreach (CumulativeCompOff CompOffSanctionObjforcompoff in CompOffSanctionListforcompoff)
                                            //    {
                                            //        if (item.StatusId == 3 || item.StatusId == 6 || item.StatusId == 5)
                                            //        {
                                            //            CompOffSanctionObjforcompoff.AppStatus = null;
                                            //            CompOffSanctionObjforcompoff.CoDate = null;
                                            //            CompOffSanctionObjforcompoff.CompOffApplicationID = null;

                                            //            //CompOffSanctionObjforcompoff.BalanceCoHours = CompOffSanctionObjforcompoff.BalanceCoHours + TotalWorkingHours; //CODE ADDED BY SHRADDHA ON 20 MAR 2018
                                            //            if (CompOffSanctionObjforcompoff.UsedCoHours > CompOffSanctionObjforcompoff.CoHours)
                                            //            {
                                            //                CompOffSanctionObjforcompoff.BalanceCoHours = CompOffSanctionObjforcompoff.CoHours;
                                            //            }
                                            //            else
                                            //            {
                                            //                CompOffSanctionObjforcompoff.BalanceCoHours = CompOffSanctionObjforcompoff.BalanceCoHours + CompOffSanctionObjforcompoff.UsedCoHours;
                                            //            }
                                            //            CompOffSanctionObjforcompoff.UsedCoHours = CompOffSanctionObjforcompoff.UsedCoHours - CompOffSanctionObjforcompoff.CoHours;

                                            //            WetosDB.SaveChanges();
                                            //        }
                                            //    }
                                            //}
                                            else
                                            {
                                                Error("You can not cancel comp off for " + item.FromDate + ", Employee: " + ValidEmpObj.EmployeeCode + " - " + ValidEmpObj.FirstName + " " + ValidEmpObj.LastName + ". COMP OFF Data is not available.");
                                                PopulateDropDown();
                                                return View(CompOffSanctionObjForCount);
                                            }

                                            COMPOFFAPPLICATIONObj.StatusId = item.StatusId;
                                            COMPOFFAPPLICATIONObj.Status = WetosDB.DropdownDatas.Where(a => a.TypeId == 3 && a.Value == COMPOFFAPPLICATIONObj.StatusId).Select(a => a.Text).FirstOrDefault();
                                            WetosDB.SaveChanges();
                                            #endregion
                                        }
                                    }
                                    else
                                    {
                                        #region IF CO LEAVE IS NOT APPLICABLE OR GLOBAL SETTING VALUE IS NULL
                                        if (COMPOFFAPPLICATIONObj.StatusId == 2)
                                        {
                                            for (DateTime CurrentDate = Convert.ToDateTime(COMPOFFAPPLICATIONObj.FromDate); CurrentDate.Date <= Convert.ToDateTime(COMPOFFAPPLICATIONObj.ToDate); CurrentDate = CurrentDate.AddDays(1))
                                            {
                                                DailyTransaction DailyTransactionObj = WetosDB.DailyTransactions.Where(a => a.EmployeeId == COMPOFFAPPLICATIONObj.EmployeeId && a.TranDate == CurrentDate).FirstOrDefault();
                                                if (DailyTransactionObj != null)
                                                {
                                                    DailyTransactionObj.Status = DailyTransactionObj.ActualStatus;
                                                    WetosDB.SaveChanges();
                                                }
                                            }
                                        }


                                        if (CompOffSanctionListforcompoff.Count > 0)
                                        {

                                            List<string> CompOffIdDeleteSplit = new List<string>();

                                            List<string> CompOffIdSplit = COMPOFFAPPLICATIONObj.CompOffId.Split(',').ToList();

                                            foreach (string StrIdMin in CompOffIdSplit)
                                            {
                                                List<string> StrIdMinList = StrIdMin.Split('-').ToList();
                                                int CompOffIdStringConv = Convert.ToInt32(StrIdMinList[0]);

                                                int ConsumedMinCompOff = Convert.ToInt32(StrIdMinList[1]);

                                                CumulativeCompOff CompOffSanctionObjforcompoff = CompOffSanctionListforcompoff.Where(a => a.CompOffId == CompOffIdStringConv).FirstOrDefault();

                                                if (CompOffSanctionObjforcompoff != null)
                                                {
                                                    if (item.StatusId == 3 || item.StatusId == 6 || item.StatusId == 5)
                                                    {

                                                        CompOffSanctionObjforcompoff.AppStatus = null;
                                                        CompOffSanctionObjforcompoff.CoDate = null;
                                                        CompOffSanctionObjforcompoff.CompOffApplicationID = null;

                                                        CompOffSanctionObjforcompoff.BalanceCoHours = CompOffSanctionObjforcompoff.BalanceCoHours + ConsumedMinCompOff;

                                                        CompOffSanctionObjforcompoff.UsedCoHours = CompOffSanctionObjforcompoff.UsedCoHours - ConsumedMinCompOff;

                                                        WetosDB.SaveChanges();

                                                    }

                                                }
                                            }
                                        }

                                        //if (CompOffSanctionListforcompoff.Count > 0)
                                        //{
                                        //    foreach (CumulativeCompOff CompOffSanctionObjforcompoff in CompOffSanctionListforcompoff)
                                        //    {
                                        //        if (item.StatusId == 3 || item.StatusId == 6 || item.StatusId == 5)
                                        //        {
                                        //            CompOffSanctionObjforcompoff.AppStatus = null;
                                        //            CompOffSanctionObjforcompoff.CoDate = null;
                                        //            CompOffSanctionObjforcompoff.CompOffApplicationID = null;
                                        //            //CompOffSanctionObjforcompoff.BalanceCoHours = CompOffSanctionObjforcompoff.BalanceCoHours + TotalWorkingHours; //CODE ADDED BY SHRADDHA ON 20 MAR 2018
                                        //            if (CompOffSanctionObjforcompoff.UsedCoHours > CompOffSanctionObjforcompoff.CoHours)
                                        //            {
                                        //                CompOffSanctionObjforcompoff.BalanceCoHours = CompOffSanctionObjforcompoff.CoHours;
                                        //            }
                                        //            else
                                        //            {
                                        //                CompOffSanctionObjforcompoff.BalanceCoHours = CompOffSanctionObjforcompoff.BalanceCoHours + CompOffSanctionObjforcompoff.UsedCoHours;
                                        //            }
                                        //            CompOffSanctionObjforcompoff.UsedCoHours = CompOffSanctionObjforcompoff.UsedCoHours - CompOffSanctionObjforcompoff.CoHours;

                                        //            WetosDB.SaveChanges();
                                        //        }
                                        //    }
                                        //}
                                        else
                                        {
                                            Error("You can not cancel comp off for " + item.FromDate + ", Employee: " + ValidEmpObj.EmployeeCode + " - " + ValidEmpObj.FirstName + " " + ValidEmpObj.LastName + ". COMP OFF Data is not available.");
                                            PopulateDropDown();
                                            return View(CompOffSanctionObjForCount);
                                        }

                                        COMPOFFAPPLICATIONObj.StatusId = item.StatusId;
                                        COMPOFFAPPLICATIONObj.Status = WetosDB.DropdownDatas.Where(a => a.TypeId == 3 && a.Value == COMPOFFAPPLICATIONObj.StatusId).Select(a => a.Text).FirstOrDefault();

                                        // ADDED BY MSJ ON 09 JAN 2019 START
                                        COMPOFFAPPLICATIONObj.AppliedBy = Convert.ToInt32(Session["EmployeeNo"]);
                                        COMPOFFAPPLICATIONObj.AppliedOn = DateTime.Now;
                                        COMPOFFAPPLICATIONObj.SanctionedBy = Convert.ToInt32(Session["EmployeeNo"]);
                                        COMPOFFAPPLICATIONObj.SanctionedOn = DateTime.Now;
                                        // ADDED BY MSJ ON 09 JAN 2019 END


                                        WetosDB.SaveChanges();
                                        #endregion
                                    }
                                }
                                #endregion

                                #region ADD AUDIT LOG
                                string Oldrecord = "Employee ID : " + EmpNo
                                                    + "FromDate :" + COMPOFFAPPLICATIONObj.FromDate + "To Date :" + COMPOFFAPPLICATIONObj.ToDate
                                                    + "FromDateStatus :" + COMPOFFAPPLICATIONObj.FromDateStatus + "ToDateStatus :" + COMPOFFAPPLICATIONObj.ToDateStatus
                                                    + "Purpose :" + COMPOFFAPPLICATIONObj.Purpose + " Status : " + COMPOFFAPPLICATIONObj.Status + "Reject Reason :"
                                                    + COMPOFFAPPLICATIONObj.RejectReason + "ExtraHoursDate :" + COMPOFFAPPLICATIONObj.ExtrasHoursDate + "Branch ID :"
                                                    + COMPOFFAPPLICATIONObj.BranchId + "Company ID:" + COMPOFFAPPLICATIONObj.CompanyId;
                                //---x---------x-----------------x------------x----------------x-----------x-------------x------------x--------------x----------x---------x-----

                                string Newrecord = "Employee ID : " + EmpNo
                                                    + "FromDate :" + COMPOFFAPPLICATIONObj.FromDate + "To Date :" + COMPOFFAPPLICATIONObj.ToDate
                                                    + "FromDateStatus :" + COMPOFFAPPLICATIONObj.FromDateStatus + "ToDateStatus :" + COMPOFFAPPLICATIONObj.ToDateStatus
                                                    + "Purpose :" + COMPOFFAPPLICATIONObj.Purpose + " Status : " + COMPOFFAPPLICATIONObj.Status + "Reject Reason :"
                                                    + COMPOFFAPPLICATIONObj.RejectReason + "ExtraHoursDate :" + COMPOFFAPPLICATIONObj.ExtrasHoursDate + "Branch ID :"
                                                    + COMPOFFAPPLICATIONObj.BranchId + "Company ID:" + COMPOFFAPPLICATIONObj.CompanyId;

                                //LOGIN ID IS TAKEN FROM SESSION PERSISTER
                                string Formname = "CUMULATIVE COMPOFF SANCTION";
                                //ACTION IS UPDATE
                                string Message = " ";
                                int EMP = Convert.ToInt32(COMPOFFAPPLICATIONObj.EmployeeId);

                                WetosAdministrationController.GenerateAuditLogsUpdate(WetosDB, EMP, Formname, Oldrecord,
                                    Newrecord, ref Message);
                                #endregion

                                #region COMPOFF SANCTION NOTIFICATION
                                #region SEND COMP OFF NOTIFICATION TO EMPLOYEE WHOM COMP OFF IS BEING PROCESSED
                                Notification NotificationObj = new Notification();
                                NotificationObj.FromID = ValidEmpObj.EmployeeReportingId;
                                NotificationObj.ToID = ValidEmpObj.EmployeeId;
                                NotificationObj.SendDate = DateTime.Now;
                                NotificationObj.NotificationContent = "Your CompOff application from " + COMPOFFAPPLICATIONObj.FromDate.Value.ToString("dd-MMM-yyyy") + " to " + COMPOFFAPPLICATIONObj.ToDate.Value.ToString("dd-MMM-yyyy") + " is " + COMPOFFAPPLICATIONObj.Status;
                                NotificationObj.ReadFlag = false;
                                NotificationObj.SendDate = DateTime.Now;

                                WetosDB.Notifications.Add(NotificationObj);
                                WetosDB.SaveChanges();
                                #endregion


                                if (COMPOFFAPPLICATIONObj.StatusId == 4 && ValidEmpObj.EmployeeReportingId == EmpNo)
                                {
                                    Notification NotificationObj2 = new Notification();
                                    NotificationObj.FromID = ValidEmpObj.EmployeeId;
                                    NotificationObj.ToID = ValidEmpObj.EmployeeReportingId2;
                                    NotificationObj.SendDate = DateTime.Now;
                                    NotificationObj.NotificationContent = "Received CompOff application for sanctioning from " + COMPOFFAPPLICATIONObj.FromDate.Value.ToString("dd-MMM-yyyy") + " to " + COMPOFFAPPLICATIONObj.ToDate.Value.ToString("dd-MMM-yyyy") + " for " + ValidEmpObj.FirstName + " " + ValidEmpObj.LastName;
                                    //NotificationObj.NotificationContent = "Leave applied from " + LeaveApplicationObj.FromDate.ToString("dd-MMM-yyyy") + " to " + LeaveApplicationObj.ToDate.ToString("dd-MMM-yyyy") + " is " + LeaveApplicationObj.Status;
                                    NotificationObj.ReadFlag = false;
                                    NotificationObj.SendDate = DateTime.Now;

                                    WetosDB.Notifications.Add(NotificationObj2);

                                    WetosDB.SaveChanges();

                                }

                                // NOTIFICATION COUNT
                                int NoTiCount = WetosDB.Notifications.Where(a => a.ToID == EmpNo && (a.ReadFlag == false || a.ReadFlag == null)).Count();
                                Session["NotificationCount"] = NoTiCount;

                                #endregion

                                #region EMAIL
                                //GlobalSetting GlobalSettingObj2 = WetosDB.GlobalSettings.Where(a => a.SettingText == "Send Email").FirstOrDefault();

                                //ABOVE LINE COMMENTED AND BELOW LINE IS ADDED BY SHALAKA ON 13TH DEC 2017
                                GlobalSetting GlobalSettingObj2 = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.SendEmail).FirstOrDefault();

                                if (GlobalSettingObj2 != null)
                                {
                                    if (GlobalSettingObj2.SettingValue == "1")
                                    {

                                        string EmailUpdateStatus = string.Empty;
                                        if (ValidEmpObj.Email != null)
                                        {
                                            if (SendEmail(ValidEmpObj.Email, NotificationObj.NotificationContent, NotificationObj.NotificationContent, ref EmailUpdateStatus, "CompOff " + COMPOFFAPPLICATIONObj.Status) == false)
                                            {
                                                Error(EmailUpdateStatus);
                                            }
                                        }
                                        else
                                        {
                                            AddAuditTrail("Unable to send email, as Email Id not available for " + ValidEmpObj.EmployeeCode);
                                        }
                                    }
                                    else
                                    {
                                        AddAuditTrail("Unable to send Email as email utility is disabled");
                                    }
                                }

                                #endregion
                            }
                        }
                    }

                    List<SP_WetosGetEmployeeCumulativeCompOffSanction_Result> CompOffSanctionObj = WetosDB.SP_WetosGetEmployeeCumulativeCompOffSanction(EmpNo).OrderByDescending(a => a.FromDate).OrderByDescending(a => a.ToDate).ToList();
                    AddAuditTrail("Success - Compoff application processed successfully");  // Updated by Rajas on 17 JAN 2017
                    Success("Success - Compoff application processed successfully");
                    PopulateDropDown();
                    return View(CompOffSanctionObjForCount);
                }
                #endregion
            }
            #endregion

            #region CATCH BLOCK TO HANDLE RUNTIME EXCEPTION
            catch (System.Exception ex)
            {
                AddAuditTrail("Error - Compoff application sanctioned failed " + ex.Message);  // Updated by Rajas on 17 JAN 2017
                Error("Error - Compoff application sanctioned failed");
                PopulateDropDown();
                return RedirectToAction("CumulativeCompOffIndex");

            }
            #endregion
        }


    }
}
