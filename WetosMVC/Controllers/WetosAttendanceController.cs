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
    public class WetosAttendanceController : BaseController
    {

        


        //
        // GET: /WetosAttendance/



        /// <summary>
        /// Added by Rajas on 6 MARCh 2017 for List view for Condone entry
        /// </summary>
        /// <returns></returns>
        public ActionResult CondoneEntryIndex()
        {
            try
            {
                // Order by desc, updated by Rajas on 18 MARCH 2017
                List<SP_CondoneTrnListView_Result> CondoneTransactionListObj = new List<SP_CondoneTrnListView_Result>();
                if (CondoneTransactionListObj.Count > 0)
                {
                    CondoneTransactionListObj = WetosDB.SP_CondoneTrnListView().OrderByDescending(a => a.CondoneDate).ToList();

                    //ADDED BY RAJAS ON 27 DEC 2016
                    AddAuditTrail("Success - Checked Condone transaction list"); // Updated by Rajas on 6 MARCH 2017
                }
                return View(CondoneTransactionListObj);
            }

            catch (System.Exception ex)
            {
                AddAuditTrail("Error : Error during checking list for condone entry due to " + ex.Message + " and " + ( ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                Error("Error : Unable to display condone transaction list");

                return View();
            }
        }

        /// <summary>
        /// DELETE GET
        /// Added by Rajas on 28 MARCH 2017
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult CondoneEntryDelete(int id)
        {
            try
            {
                CondoneTrn CondoneTrnObj = WetosDB.CondoneTrns.Where(b => b.CondoneId == id).FirstOrDefault();

                if (CondoneTrnObj != null)
                {
                    WetosDB.CondoneTrns.Remove(CondoneTrnObj);
                    WetosDB.SaveChanges();

                    AddAuditTrail("Deleted condone entry for " + id); // Updated on 16 JAN 2017 by Rajas

                }

                return RedirectToAction("CondoneEntryIndex");

            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in Delete condone Entry due to " + ex.Message + ( ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                Error(WetosErrorMessageController.GetErrorMessage(15));

                return RedirectToAction("CondoneEntryIndex");
            }
        }


        /// <summary>
        /// Updated by Rajas on 27 MARCH 2017
        /// </summary>
        /// <returns></returns>
        public ActionResult CondoneEntry()
        {
            try
            {
                PopulateDropDown();

                return View();
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + ( ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                return RedirectToAction("CondoneEntryIndex");
            }
        }

        /// <summary>
        /// Modified by Shraddha on 18th OCT 2016
        /// Updated by Rajas on 6 MARCH 2017
        /// </summary>
        /// <param name="CondoneObj"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        /// Updated by Rajas on 3 JUNE 2017
        [HttpPost]
        public ActionResult CondoneEntry(CondoneModel CondoneModelObj, FormCollection collection)
        {
            try
            {
                //if (ModelState.IsValid)
                //{

                // Addded by Rajas on 27 MARCH 2017
                string UpdateStatus = string.Empty;

                bool IsEdit = false;

                if (UpdateCondoneEntryData(CondoneModelObj, IsEdit, ref UpdateStatus) == true)
                {
                    // ADDED BY PUSHKAR FOR AUDITLOG ON 21 DEC 2016
                    AddAuditTrail("Condone entry created for " + CondoneModelObj.CondoneDate.ToString("dd-MMM-yyyy"));

                    Success("Condone entry for " + CondoneModelObj.CondoneDate.ToString("dd-MMM-yyyy") + " added successfully");
                }
                else
                {
                    AddAuditTrail(UpdateStatus);

                    Error(UpdateStatus);

                    PopulateDropDownEdit(CondoneModelObj);

                    return View(CondoneModelObj);
                }

                return RedirectToAction("CondoneEntryIndex");
                //}
                //else
                //{
                //    AddAuditTrail("Error : Condone entry not applied as required fields are not filled correctly");

                //    Error("Error : Condone entry not applied as required fields are not filled correctly");

                //    PopulateDropDown();

                //    return View(CondoneModelObj);
                //}
            }

            catch (System.Exception ex)
            {
                AddAuditTrail("Error : Condone entry failed due to " + ex.Message);

                Error("Error : Condone entry failed");

                return RedirectToAction("CondoneEntryIndex");
            }
        }

        /// <summary>
        /// Condone entry
        /// Added by Rajas on 6 MARCH 2017
        /// Edit GET
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// UPDATED BY RAJAS ON 3 JUNE 2017
        public ActionResult CondoneEntryEdit(string id)
        {
            try
            {
                // GET EXISTNG ENTRY FROM ID

                int ID = Convert.ToInt32(id);  // Updated by Rajas on 6 MARCH 2017 as int Id was giving error

                CondoneTrn CondoneTrnEdit = WetosDB.CondoneTrns.Single(b => b.CondoneId == ID);

                CondoneModel CondoneModelObj = new CondoneModel();

                CondoneModelObj.BranchId = CondoneTrnEdit.Branch.BranchId;

                CondoneModelObj.CondoneId = CondoneTrnEdit.CondoneId;

                CondoneModelObj.CompanyId = CondoneTrnEdit.Company.CompanyId;

                CondoneModelObj.CondoneDate = CondoneTrnEdit.CondoneDate;

                CondoneModelObj.Reason = CondoneTrnEdit.Reason;

                CondoneModelObj.ShiftId = CondoneTrnEdit.ShiftId;

                CondoneModelObj.LateEarly = CondoneTrnEdit.LateEarly;

                CondoneModelObj.CondoneStatus = CondoneTrnEdit.Status;

                CondoneModelObj.TransportId = CondoneTrnEdit.TransportId;

                CondoneModelObj.DivisionId = CondoneTrnEdit.DivisionId;

                CondoneModelObj.Employeelist = CondoneTrnEdit.Employeelist;

                PopulateDropDownEdit(CondoneModelObj);

                return View(CondoneModelObj);
            }

            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + ( ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                Error("Condone entry edit failed");

                return RedirectToAction("CondoneEntryIndex");
            }
        }

        /// <summary>
        /// Condone entry edit
        /// POST
        /// Added By Rajas on 6 MARCH 2017
        /// </summary>
        /// <param name="id"></param>
        /// <param name="CondoneModelObj"></param>
        /// <returns></returns>
        /// Updated by Rajas on 27 MARCH 2017
        [HttpPost]
        public ActionResult CondoneEntryEdit(int id, CondoneModel CondoneModelObj)
        {
            try
            {
                // Addded by Rajas on 27 MARCH 2017
                string UpdateStatus = string.Empty;

                bool IsEdit = true;

                if (ModelState.IsValid)
                {
                    if (UpdateCondoneEntryData(CondoneModelObj, IsEdit, ref UpdateStatus) == true)
                    {

                        // ADDED BY RAJAS FOR AUDITLOG ON 27 DEC 2016
                        AddAuditTrail("Condone entry for : " + CondoneModelObj.CondoneDate.ToString("dd-MMM-yyyy") + " is updated Successfully"); // Updated by Rajas on 6 MARCH 2017

                        //ADDED CODE FOR SUCCESS MESSAGE BY RAJAS ON 16 JAN 2017
                        Success("Condone entry for : " + CondoneModelObj.CondoneDate.ToString("dd-MMM-yyyy") + " is updated Successfully"); // Updated by Rajas 6 MARCH 2017
                    }
                    else
                    {
                        AddAuditTrail(UpdateStatus);

                        Error(UpdateStatus);

                        PopulateDropDownEdit(CondoneModelObj);

                        return View(CondoneModelObj);
                    }

                    return RedirectToAction("CondoneEntryIndex");
                }
                else
                {
                    PopulateDropDownEdit(CondoneModelObj);

                    // ADDED BY RAJAS FOR AUDITLOG ON 27 DEC 2016
                    //AddAuditTrail("Error - Condone entry update failed"); // Updated by Rajas on 6 MARCH 2017


                    //ADDED CODE FOR SUCCESS MESSAGE BY RAJAS ON 16 JAN 2017
                    //Error("Error - Condone entry update failed"); // Updated by Rajas on 6 MARCH 2017

                    return View(CondoneModelObj);

                }
            }
            catch (System.Exception ex)
            {
                PopulateDropDownEdit(CondoneModelObj);

                // Added by Rajas on 16 JAN 2017 START
                AddAuditTrail("Error - Condone entry update failed due to " + ex.Message);

                Error("Error - Condone entry update failed " + ex.Message);
                // Added by Rajas on 16 JAN 2017 END

                return View();
            }
        }

        /// <summary>
        /// Condone entry
        /// Common function to validate the data for edit or create
        /// Added by Rajas on 6 MARCH 2017
        /// </summary>
        /// Updated by Rajas on 27 MARCH 2017
        private bool UpdateCondoneEntryData(CondoneModel CondoneModelObj, bool IsEdit, ref string UpdateStatus)
        {
            bool ReturnStatus = false;

            try
            {
                // Updated by Rajas on 3 OCT 2017. Added && a.ShiftId == CondoneModelObj.ShiftId condition
                WetosDB.CondoneTrn CondoneTrnTblObj = WetosDB.CondoneTrns.Where(a => a.CondoneId == CondoneModelObj.CondoneId || (a.CondoneDate == CondoneModelObj.CondoneDate
                    && a.ShiftId == CondoneModelObj.ShiftId && a.Status.Trim() == CondoneModelObj.CondoneStatus.Trim())).FirstOrDefault(); // ADDED STATUS CONDITION BY SHRADDHA ON 16 JAN 2018 TO COMPARE STATUS AS WELL

                // ADDED BY RAJAS ON 03 JUNE 2017 START
                bool IsNew = false;

                // IS OBJECT PRESET
                if (CondoneTrnTblObj != null)  // Added by Rajas on 2 JUNE 2017
                {
                    if (IsEdit == false)  // CREATE            
                    {
                        UpdateStatus = "Condone entry already available."; //WetosEmployeeController.GetErrorMessage(1);

                        //AddAuditTrail("Error in Department update : " + UpdateStatus);
                        return ReturnStatus;
                    }
                    else // ELSE CODE ADDED BY SHRADDHA ON 16 JAN 2018
                    {
                        WetosDB.CondoneTrn CondoneTrnTblObjNew = WetosDB.CondoneTrns.Where(a => a.CondoneDate == CondoneModelObj.CondoneDate
                        && a.ShiftId == CondoneModelObj.ShiftId && a.Status.Trim() == CondoneModelObj.CondoneStatus.Trim()).FirstOrDefault();
                        if (CondoneTrnTblObjNew != null)  // CREATE            
                        {
                            UpdateStatus = "Condone entry already available for selected date and selected status. Please enter valid status."; //WetosEmployeeController.GetErrorMessage(1);

                            //AddAuditTrail("Error in Department update : " + UpdateStatus);
                            return ReturnStatus;
                        }
                    }
                }
                else
                {
                    if (IsEdit == false) // CREATE    
                    {
                        CondoneTrnTblObj = new WetosDB.CondoneTrn();
                        IsNew = true;
                    }
                    else // EDIT    
                    {
                        UpdateStatus = "Error in updating Condone entry."; // WetosEmployeeController.GetErrorMessage(1); 
                        //AddAuditTrail("Error in Department update : " + UpdateStatus);
                        return ReturnStatus;
                    }
                }
                // ADDED BY RAJAS ON 03 JUNE 2017 END

                // Added by Rajas on 3 OCT 2017 START
                CondoneTrnTblObj.Branch = WetosDB.Branches.Where(a => a.BranchId == CondoneModelObj.BranchId).FirstOrDefault();
                CondoneTrnTblObj.Company = WetosDB.Companies.Where(a => a.CompanyId == CondoneModelObj.CompanyId).FirstOrDefault();
                // Added by Rajas on 3 OCT 2017 END

                CondoneTrnTblObj.Branch.BranchId = CondoneModelObj.BranchId;

                CondoneTrnTblObj.Company.CompanyId = CondoneModelObj.CompanyId;

                CondoneTrnTblObj.Branch.BranchId = CondoneModelObj.BranchId;

                CondoneTrnTblObj.CondoneDate = CondoneModelObj.CondoneDate;

                CondoneTrnTblObj.Reason = CondoneModelObj.Reason;

                CondoneTrnTblObj.ShiftId = CondoneModelObj.ShiftId;

                CondoneTrnTblObj.LateEarly = CondoneModelObj.LateEarly;

                CondoneTrnTblObj.Status = CondoneModelObj.CondoneStatus;

                CondoneTrnTblObj.TransportId = CondoneModelObj.TransportId;

                CondoneTrnTblObj.DivisionId = CondoneModelObj.DivisionId;

                CondoneTrnTblObj.Employeelist = CondoneModelObj.Employeelist;


                // Add new table object 
                if (IsNew)
                {
                    WetosDB.CondoneTrns.Add(CondoneTrnTblObj);
                }

                WetosDB.SaveChanges();

                ReturnStatus = true;

                return ReturnStatus;
            }
            catch (System.Exception ex)
            {
                UpdateStatus = "Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message);

                return ReturnStatus;
            }
        }

        public ActionResult ShiftSchedule()
        {
            //var ShiftScheduleObj = WetosDB.ShiftSchedules.Select(a => new { ShiftScheduledId = a.ShiftMonth, ShiftYear = a.ShiftYear }).ToList();

            //ViewBag.ShiftScheduleList = new SelectList(ShiftScheduleObj, "ShiftMonth", "ShiftScheduleId").ToList();

            #region CODE FOR GET BRANCH LIST ADDED BY SHRADDHA ON 30 MAR 2018
            int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
            //var BranchObj = WetosDB.Branches.Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList();
            var BranchObj = WetosDB.SP_GetBranchList(EmployeeId).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList(); //.Select(m => new { BranchId = m.BranchId, BranchName = m.BranchName}).ToList();
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
            #endregion
            ViewBag.BranchList = new SelectList(BranchObj, "BranchId", "BranchName").ToList();

            List<string> MonthObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 7).Select(a => a.Text).ToList();
            ViewBag.MonthList = MonthObj;

            List<string> YearObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 8).Select(a => a.Text).ToList();
            ViewBag.YearList = YearObj;

            return View();

        }

        /// <summary>
        /// Added By Shraddha On 15th OCCT 2016 for Adjustment post
        /// </summary>
        /// <param name="HolidayObj"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ShiftSchedule(WetosDB.ShiftSchedule ShiftScheduleObj, FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                WetosDB.ShiftSchedules.Add(ShiftScheduleObj);
                return RedirectToAction("Index", "WetosDashboard");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult HolidayEntry()//Modified By shraddha on 17th OCT 2016 for model validation
        {
            HolidayInputModel hol = new HolidayInputModel();
            PopulateDropDown();
            //HoliDay HolidayObj = new HoliDay();

            //HolidayObj.HLType = "abcd";

            return View(hol);
        }
        /// <summary>
        /// ADDED BY MOUSAMI ON 15 SEPT 2016
        /// </summary>
        /// <param name="HolidayObj"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult HolidayEntry(HolidayInputModel HolidayObj, FormCollection collection)
        {
            //Modified By shraddha on 17th OCT 2016 for model validation
            if (ModelState.IsValid)
            {
                HoliDay Holiday = new HoliDay();

                Holiday.Branchid = HolidayObj.Branchid;

                Holiday.CompanyId = HolidayObj.CompanyId;

                Holiday.Criteria = HolidayObj.Criteria;

                Holiday.DayStatus = HolidayObj.DayStatus;

                Holiday.Description = HolidayObj.Description;

                Holiday.EmployeeId = HolidayObj.EmployeeId;

                Holiday.FromDate = Convert.ToDateTime(HolidayObj.FromDate);

                Holiday.ToDate = Convert.ToDateTime(HolidayObj.ToDate);

                Holiday.HLType = HolidayObj.HLType;

                Holiday.ReligionId = HolidayObj.ReligionId;

                WetosDB.HoliDays.Add(Holiday);


                WetosDB.SaveChanges();


                // ADDED BY PUSHKAR FOR AUDITLOG ON 21 DEC 2016
                AddAuditTrail("Holiday Entry Created");

                // ModelState.AddModelError("Success", "Holiday has been created successfully:");
                return RedirectToAction("HolidayList", "WetosAttendance");
            }
            else
            {
                PopulateDropDown();
                return View(HolidayObj);
            }
        }

        /// <summary>
        /// ADDED BY SHRADDHA ON 02 JAN 2016 FOR DISPLAYING HOLIDAY LIST
        /// </summary>
        /// <returns></returns>
        public ActionResult HolidayList()
        {

            List<WetosDB.HoliDay> HolidayList = WetosDB.HoliDays.OrderByDescending(a => a.FromDate).ToList();
            return View(HolidayList);
        }

        /// <summary>
        /// ADDED BY SHRADDHA ON 02 JAN 2016 FOR DISPLAYING HOLIDAY LIST
        /// </summary>
        /// <returns></returns>
        public ActionResult HolidayEdit(int id)
        {
            HoliDay HolidayObj = WetosDB.HoliDays.Where(a => a.HoliDayId == id).FirstOrDefault();
            HolidayInputModel HolidayModelObj = new HolidayInputModel();
            HolidayModelObj.Branchid = HolidayObj.Branchid;
            HolidayModelObj.CompanyId = HolidayObj.CompanyId;
            HolidayModelObj.Criteria = HolidayObj.Criteria;
            HolidayModelObj.DayStatus = HolidayObj.DayStatus;
            HolidayModelObj.Description = HolidayObj.Description;
            HolidayModelObj.EmployeeId = HolidayObj.EmployeeId;
            HolidayModelObj.FromDate = HolidayObj.FromDate;
            HolidayModelObj.HLType = HolidayObj.HLType;
            HolidayModelObj.ReligionId = HolidayObj.ReligionId;
            HolidayModelObj.ToDate = HolidayObj.ToDate;

            PopulateDropDown();
            return View(HolidayModelObj);
        }

        [HttpPost]
        public ActionResult HolidayEdit(int id, HolidayInputModel HolidayModelObj)
        {
            HoliDay HolidayObj = WetosDB.HoliDays.Where(a => a.HoliDayId == id).FirstOrDefault();
            if (ModelState.IsValid)
            {
                HolidayObj.Branchid = HolidayModelObj.Branchid;
                HolidayObj.CompanyId = HolidayModelObj.CompanyId;
                HolidayObj.Criteria = HolidayModelObj.Criteria;
                HolidayObj.DayStatus = HolidayModelObj.DayStatus;
                HolidayObj.Description = HolidayModelObj.Description;
                HolidayObj.EmployeeId = HolidayModelObj.EmployeeId;
                HolidayObj.FromDate = HolidayModelObj.FromDate;
                HolidayObj.HLType = HolidayModelObj.HLType;
                HolidayObj.ReligionId = HolidayModelObj.ReligionId;
                HolidayObj.ToDate = HolidayModelObj.ToDate;
                WetosDB.SaveChanges();

                return RedirectToAction("HolidayList");
            }
            else
            {
                PopulateDropDown();
                return View(HolidayModelObj);
            }

        }

        public ActionResult DeclaredHoliday()
        {
             #region CODE FOR GET BRANCH LIST ADDED BY SHRADDHA ON 30 MAR 2018
            int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
            //var BranchObj = WetosDB.Branches.Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList();
            var BranchObj = WetosDB.SP_GetBranchList(EmployeeId).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList(); //.Select(m => new { BranchId = m.BranchId, BranchName = m.BranchName}).ToList();
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
            #endregion
            ViewBag.BranchList = new SelectList(BranchObj, " BranchId", "BranchName").ToList();

            return View();
        }

        [HttpPost]
        public ActionResult DeclaredHoliday(DeclaredHoliday DeclrdHolidayObj, FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                WetosDB.DeclaredHolidays.Add(DeclrdHolidayObj);
                return RedirectToAction("Index", "WetosDashboard");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /WetosAttendance/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /WetosAttendance/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /WetosAttendance/Create



        //
        // GET: /WetosAttendance/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /WetosAttendance/Edit/5

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
        // GET: /WetosAttendance/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /WetosAttendance/Delete/5

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
        /// To Populate Dropdown Added by Shraddha on 17th Oct 2016
        /// </summary>
        public void PopulateDropDown()
        {

            List<string> SelectCriteriaObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 5).Select(a => a.Text).ToList();
            ViewBag.SelectCriteriaList = SelectCriteriaObj;

            // Updated by Rajas on 6 MAY 2017 to get data which is not MarkedAsDelete
            #region COMPANY LIST CODE ADDED BY SHRADDHA ON 30 MAR 2018
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
            int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            //var CompanyObj = WetosDB.Companies.Select(a => new { CompanyId = a.CompanyId, CompanyName = a.CompanyName }).ToList();
            var CompanyObj = WetosDB.SP_GetCompanyList(EmployeeId).Select(m => new { Companyid = m.CompanyId, CompanyName = m.CompanyName }).ToList();
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
            #endregion

            ViewBag.CompanyList = new SelectList(CompanyObj, "CompanyId", "CompanyName").ToList();

            var BranchObj = new List<Branch>();  //WetosDB.Branches.Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList();

            ViewBag.BranchList = new SelectList(BranchObj, " BranchId", "BranchName").ToList();

            List<string> SearchByObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 4).Select(a => a.Text).ToList();
            ViewBag.SearchByList = SearchByObj;

            // Added by Rajas on 21 NOV 2016 for Dropdown list start
            var DayStatusObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 9).Select(a => new { LeaveStatusID = a.Value, LeaveStatus = a.Text }).ToList();
            ViewBag.DayStatusList = new SelectList(DayStatusObj, "LeaveStatusID", "LeaveStatus").ToList();

            var EmployeeObj = new List<Employee>(); //WetosDB.Employees.Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.FirstName + " " + a.LastName }).ToList();
            ViewBag.EmployeeList = new SelectList(EmployeeObj, "EmployeeId", "EmployeeName").ToList();

            var ShiftObj = WetosDB.Shifts.Where(a => a.MarkedAsDelete == null || a.MarkedAsDelete == 0)
                .Select(a => new { ShiftId = a.ShiftId, ShiftName = a.ShiftName }).ToList();
            ViewBag.ShiftList = new SelectList(ShiftObj, "ShiftId", "ShiftName").ToList();


            //Modified By Shraddha on 15Th OCT 2016 Start
            var HolidayTypeObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 6).Select(a => new { HolidayName = a.Text, HolidayValue = a.Text }).ToList();
            ViewBag.HolidayTypeList = new SelectList(HolidayTypeObj, "HolidayName", "HolidayValue").ToList();
            //Modified By Shraddha on 15Th OCT 2016 Start

            var LeaveSanctionObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 3).Select(a => new { LeaveSanctionId = a.Value, Status = a.Text }).ToList();

            ViewBag.LeaveSanctionList = new SelectList(LeaveSanctionObj, "LeaveSanctionId", "Status").ToList();

            // Updated by Rajas on 17 AUGUST 2017 for Condone shift START
            var CondoneEntry1Obj = WetosDB.Shifts.Select(a => new { ShiftId = a.ShiftId, CondoneShift = a.ShiftCode }).Distinct().ToList();
            ViewBag.CondoneShiftList = new SelectList(CondoneEntry1Obj, "ShiftId", "CondoneShift").ToList();
            // Updated by Rajas on 17 AUGUST 2017 for Condone shift END

            //Added By Shraddha on 28 DEC 2016 For condone status add START
            var CondoneStatusObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 11).Select(a => new { CondoneStatusID = a.Text, CondoneStatus = a.Text }).ToList();
            ViewBag.CondoneStatusObj = new SelectList(CondoneStatusObj, "CondoneStatusID", "CondoneStatus").ToList();
            //Added By Shraddha on 28 DEC 2016 For condone status add END
        }

        /// <summary>
        /// To Populate Dropdownedit
        /// Added by Rajas on 6 MARCH 2017
        /// </summary>
        public void PopulateDropDownEdit(CondoneModel CondoneModelObj)
        {
            // Updated by Rajas on 6 MAY 2017 to get data which is not MarkedAsDelete START
            List<string> SelectCriteriaObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 5).Select(a => a.Text).ToList();
            ViewBag.SelectCriteriaList = SelectCriteriaObj;

            #region COMPANY LIST CODE ADDED BY SHRADDHA ON 30 MAR 2018
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
            int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            //var CompanyObj = WetosDB.Companies.Select(a => new { CompanyId = a.CompanyId, CompanyName = a.CompanyName }).ToList();
            var CompanyObj = WetosDB.SP_GetCompanyList(EmployeeId).Select(m => new { Companyid = m.CompanyId, CompanyName = m.CompanyName }).ToList();
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
            #endregion

            ViewBag.CompanyList = new SelectList(CompanyObj, "CompanyId", "CompanyName").ToList();

            
            #region CODE FOR GET BRANCH LIST ADDED BY SHRADDHA ON 30 MAR 2018
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
            //var BranchObj = WetosDB.Branches.Where(a => (a.MarkedAsDelete == null || a.MarkedAsDelete == 0) && a.Company.CompanyId == CondoneModelObj.CompanyId)
                //.Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList();
            var BranchObj = WetosDB.SP_GetBranchList(EmployeeId).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList(); //.Select(m => new { BranchId = m.BranchId, BranchName = m.BranchName}).ToList();
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
            #endregion
            ViewBag.BranchList = new SelectList(BranchObj, " BranchId", "BranchName").ToList();

            List<string> SearchByObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 4).Select(a => a.Text).ToList();
            ViewBag.SearchByList = SearchByObj;

            // Added by Rajas on 21 NOV 2016 for Dropdown list start
            var DayStatusObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 9).Select(a => new { LeaveStatusID = a.Value, LeaveStatus = a.Text }).ToList();
            ViewBag.DayStatusList = new SelectList(DayStatusObj, "LeaveStatusID", "LeaveStatus").ToList();


            DateTime Leavingdate = Convert.ToDateTime("01/01/1900");  // Added by Rajas on 10 MARCH 2017

            // Updated by Rajas on 10 MARCH 2017 to display list of active employees with employee name and code
            

            #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
             EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            //var EmployeeObj = WetosDB.Employees.Where(a => a.Leavingdate == Leavingdate && a.CompanyId == CondoneModelObj.CompanyId && a.BranchId == CondoneModelObj.BranchId)
                //.Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
            var EmployeeObj = WetosDB.SP_VwActiveEmployee(EmployeeId).Where(a => a.Leavingdate == Leavingdate && a.CompanyId == CondoneModelObj.CompanyId && a.BranchId == CondoneModelObj.BranchId)
                .Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
            #endregion
            
            ViewBag.EmployeeList = new SelectList(EmployeeObj, "EmployeeId", "EmployeeName").ToList();


            var ShiftObj = WetosDB.Shifts.Where(a => (a.MarkedAsDelete == null || a.MarkedAsDelete == 0) && a.BranchId == CondoneModelObj.BranchId && a.Company.CompanyId == CondoneModelObj.CompanyId)
                .Select(a => new { ShiftId = a.ShiftId, ShiftName = a.ShiftName }).ToList();
            ViewBag.ShiftList = new SelectList(ShiftObj, "ShiftId", "ShiftName").ToList();
            // Updated by Rajas on 6 MAY 2017 to get data which is not MarkedAsDelete END


            //Modified By Shraddha on 15Th OCT 2016 Start
            var HolidayTypeObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 6).Select(a => new { HolidayName = a.Text, HolidayValue = a.Text }).ToList();
            ViewBag.HolidayTypeList = new SelectList(HolidayTypeObj, "HolidayName", "HolidayValue").ToList();
            //Modified By Shraddha on 15Th OCT 2016 Start

            var LeaveSanctionObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 3).Select(a => new { LeaveSanctionId = a.Value, Status = a.Text }).ToList();
            ViewBag.LeaveSanctionList = new SelectList(LeaveSanctionObj, "LeaveSanctionId", "Status").ToList();

            // Updated by Rajas on 17 AUGUST 2017 for Condone shift START
            var CondoneEntry1Obj = WetosDB.Shifts.Select(a => new { ShiftId = a.ShiftId, CondoneShift = a.ShiftCode }).Distinct().ToList();
            ViewBag.CondoneShiftList = new SelectList(CondoneEntry1Obj, "ShiftId", "CondoneShift").ToList();
            // Updated by Rajas on 17 AUGUST 2017 for Condone shift END

            //Added By Shraddha on 28 DEC 2016 For condone status add START
            var CondoneStatusObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 11).Select(a => new { CondoneStatusID = a.Text, CondoneStatus = a.Text }).ToList();
            ViewBag.CondoneStatusObj = new SelectList(CondoneStatusObj, "CondoneStatusID", "CondoneStatus").ToList();
            //Added By Shraddha on 28 DEC 2016 For condone status add END
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

            // Updated by Rajas on 30 MAY 2017
            
            #region CODE FOR GET BRANCH LIST ADDED BY SHRADDHA ON 30 MAR 2018
            int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
            //var BranchList = WetosDB.Branches.Where(a => a.Company.CompanyId == SelCompanyId && (a.MarkedAsDelete == null || a.MarkedAsDelete == 0))
               // .Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList();
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
        public JsonResult GetEmployee(string Branchid, string Companyid)
        {

            int SelBranchId = 0;
            if (!string.IsNullOrEmpty(Branchid))
            {
                if (Branchid.ToUpper() != "NULL")
                {
                    SelBranchId = Convert.ToInt32(Branchid);
                }
            }

            int SelCompanyId = 0;
            if (!string.IsNullOrEmpty(Companyid))
            {
                if (Companyid.ToUpper() != "NULL")
                {
                    SelCompanyId = Convert.ToInt32(Companyid);
                }
            }

            DateTime Leavingdate = Convert.ToDateTime("01/01/1900");  // Added by Rajas on 10 MARCH 2017

            // Updated by Rajas to pass active employee list, on 10 MARCH 2017
            
            
            #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
            int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            //var EmployeeList = WetosDB.Employees.Where(a => a.BranchId == SelBranchId && a.CompanyId == SelCompanyId && a.Leavingdate == Leavingdate)
                //.Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + a.LastName }).ToList();
            //.Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
            var EmployeeList = WetosDB.SP_VwActiveEmployee(EmployeeId).Where(a => a.BranchId == SelBranchId && a.CompanyId == SelCompanyId && a.Leavingdate == Leavingdate)
                .Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + a.LastName }).ToList();
            #endregion

            return Json(EmployeeList);
        }


    }
}
