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
    public class WetosLeaveController : BaseController
    {
        

        /// <summary>
        /// Added by Rajas on 10 OCT 2016 for WetosLeaveController
        /// </summary>
        /// <returns></returns>

        public ActionResult Index()
        {
            try
            {
                PopulateDropDown();
                //List<WetosDB.LeaveMaster> LeaveMaster = WetosDB.LeaveMasters.Where(a => a.CompanyId == 2 && a.BranchId == 1 && a.EmployeeGrpId == 1).ToList();

                //ADDED BY RAJAS ON 27 DEC 2016
                AddAuditTrail("Success - Visited Leave master"); // Updated by Rajas on 16 JAN 2017

                return View();
            }
            catch (System.Exception ex)
            {
                //throw (ex);
                return View();
            }

        }

        /// <summary>
        /// LeaveMasterModel used for posting the selected dropdown values to be passed on to create view
        /// Added by Rajas on 29 DEC 2016
        /// </summary>
        /// <param name="LeaveMasterObj"></param>
        /// <returns></returns>

        [HttpPost]
        public ActionResult Index(LeaveCompBranchDeptModel LeaveCompBranchDeptModelObj)  //LeaveMasterModel LeaveMasterObj)
        {
            try
            {
                // MODIFIED BY MSJ ON 
                if (ModelState.IsValid)
                {
                    PopulateDropDown();
                    //List<WetosDB.LeaveMaster> LeaveMaster = WetosDB.LeaveMasters.Where(a => a.CompanyId == 2 && a.BranchId == 1 && a.EmployeeGrpId == 1).ToList();

                    //ADDED BY RAJAS ON 27 DEC 2016
                    AddAuditTrail("Success - Visited Leave master"); // Updated by Rajas on 16 JAN 2017

                    return RedirectToAction("Create", new
                    {
                        // MODIFIED BY MSJ ON 23 JAN 2020
                        CompanyId = LeaveCompBranchDeptModelObj.CompanyId,// LeaveCompBranchDeptModelObj.CompanyId == null ? 0 : LeaveCompBranchDeptModelObj.CompanyId
                        BranchId = LeaveCompBranchDeptModelObj.BranchId == null ? 0 : LeaveCompBranchDeptModelObj.BranchId,
                        EmployeeGroupId = LeaveCompBranchDeptModelObj.EmployeeGrpId == null ? 0 : LeaveCompBranchDeptModelObj.EmployeeGrpId
                    });
                    //return View();
                }
                else
                {
                    // Added by Rajas on 16 JAN 2017
                    AddAuditTrail("Error - ");

                    Error("Incorrect data. Please verify and try again!");

                    PopulateDropDown();

                    return View();
                }
            }
            catch (System.Exception ex)
            {
                PopulateDropDown();


                // Added by Rajas on 16 JAN 2017
                AddAuditTrail("Exception - " + ex.Message);

                Error("Incorrect data. Please verify and try again!");

                return View(LeaveCompBranchDeptModelObj);
                //throw (ex);
            }

        }

        /// <summary>
        /// Partial view GET method added for returning list of existing leaves from table
        /// Added By Rajas on 28 DEC 2016
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <param name="BranchId"></param>
        /// <param name="EmployeeGroupId"></param>
        /// <returns></returns>
        /// Updated by Rajas on 27 MARCH 2017
        public ActionResult GetLeaveDetails(int CompanyId, int BranchId, int EmployeeGroupId)
        {
            try
            {
                // Updated by Rajas on 29 MARCH 2017
                List<WetosDB.LeaveMaster> LeaveMaster = WetosDB.LeaveMasters.Where(a => a.Company.CompanyId == CompanyId && a.BranchId == BranchId
                    && a.EmployeeGroup.EmployeeGroupId == EmployeeGroupId && (a.MarkedAsDelete == 0 || a.MarkedAsDelete == null)).ToList();

                //ADDED BY RAJAS ON 28 DEC 2016
                AddAuditTrail("Leave details checked");

                return PartialView(LeaveMaster);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Incorrect data, Please try again!");

                return RedirectToAction("Index");
            }

        }

        /// <summary>
        /// For ajax return for create new leave
        /// Added by Rajas on 28 DEC 2016
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <param name="BranchId"></param>
        /// <param name="EmployeeGroupId"></param>
        /// <returns></returns>
        public ActionResult Create(int CompanyId, int BranchId, int EmployeeGroupId)
        {
            // Try catch handling updated by Rajas on 1 MARCH 2017
            try
            {
                //var Location = WetosDB.Locations.Select(a => new { LocationId = a.LocationId, LocationName = a.LocationName }).ToList();
                //ViewBag.LocatioList = new SelectList(Location, "LocationId", "LocationName").ToList();

                LeaveMasterModel LeaveMasterObj = new LeaveMasterModel();
                LeaveMasterObj.CompanyId = CompanyId;
                LeaveMasterObj.BranchId = BranchId;
                LeaveMasterObj.EmployeeGrpId = EmployeeGroupId;

                LeaveMasterObj.ISHalfAllowed = false;
                LeaveMasterObj.IsCarryForword = false;
                LeaveMasterObj.HHBetLevConsiderLeave = false;
                LeaveMasterObj.IsLeaveCombination = false;
                LeaveMasterObj.LeaveType = false;
                LeaveMasterObj.NegativeBalanceAllowed = false;
                LeaveMasterObj.WOBetLevConsiderLeave = false;
                LeaveMasterObj.LeaveType = false;
                LeaveMasterObj.EncashmentAllowedOrNot = false;
                LeaveMasterObj.AccumulationAllowedOrNot = false;
                LeaveMasterObj.IsAttachmentNeeded = false; //CODE ADDED BY SHRADDHA ON 16 FEB 2018
                // Updated by Rajas on 1 MARCH 2017
                PopulateDropDownEdit(LeaveMasterObj);   //PopulateDropDown();

                return View(LeaveMasterObj);
            }

            catch (System.Exception ex)
            {
                PopulateDropDown();

                AddAuditTrail("Create new leave details failed due to " + ex.Message);

                Error("Create new leave details failed");

                return RedirectToAction("Index");
            }


        }

        /// <summary>
        /// Added for validate and POST create leave data
        /// Added by Rajas on 30 DEC 2016
        /// </summary>
        /// <param name="LeaveMasterObj"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(LeaveMasterModel LeaveMasterObj, FormCollection fc)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string UpdateStatus = string.Empty; // Added by Rajas on 27 MARCH 2017

                    // Added by Rajas on 15 MAY 2017
                    bool IsEdit = false;

                    if (UpdateLeaveMasterData(LeaveMasterObj, IsEdit, fc, ref UpdateStatus) == true)
                    {
                        // ADDED BY RAJAS FOR AuditLog ON 27 DEC 2016
                        AddAuditTrail("Success - Leave code : " + LeaveMasterObj.LeaveCode + " is added successfully"); // Updated by Rajas on 16 JAN 2017


                        //ADDED CODE FOR SUCCESS MESSAGE BY RAJAS ON 16 JAN 2017
                        Success("Success - Leave code : " + LeaveMasterObj.LeaveCode + " is added successfully"); // Updated by Rajas on 16 JAN 2017
                    }
                    else
                    {
                        return ReportError(LeaveMasterObj, UpdateStatus);
                    }

                    return RedirectToAction("Index");
                }
                else
                {
                    // Updated by Rajas on 1 JUNE 2017
                    PopulateDropDownEdit(LeaveMasterObj);

                    //Error("Please fill all mandatory details!");

                    return View(LeaveMasterObj);
                }
            }
            catch (System.Exception ex)
            {
                // ADDED BY RAJAS FOR AuditLog ON 27 DEC 2016
                AddAuditTrail("Exception - Leave create failed due to " + ex.Message); // Updated by Rajas on 16 JAN 2017


                //ADDED CODE FOR SUCCESS MESSAGE BY RAJAS ON 16 JAN 2017
                Error("Leave create failed"); // Updated by Rajas on 16 JAN 2017

                return RedirectToAction("Index");
            }

        }

        /// <summary>
        /// Try catch handling added by Rajas on 1 MARCH 2017
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            try
            {
                WetosDB.LeaveMaster LeaveEdit = new WetosDB.LeaveMaster();
                LeaveEdit = WetosDB.LeaveMasters.Single(b => b.LeaveId == id);

                LeaveMasterModel LeaveMasterObj = new LeaveMasterModel();

                LeaveMasterObj.LeaveId = LeaveEdit.LeaveId;

                LeaveMasterObj.LeaveCode = LeaveEdit.LeaveCode;

                LeaveMasterObj.LeaveName = LeaveEdit.LeaveName;

                LeaveMasterObj.LeaveDescription = LeaveEdit.LeaveDescription;

                LeaveMasterObj.NoOfDaysAllowedInYear = LeaveEdit.NoOfDaysAllowedInYear;

                LeaveMasterObj.MaxNoOfTimesAllowedInYear = LeaveEdit.MaxNoOfTimesAllowedInYear;

                LeaveMasterObj.MinNoofLeaveAllowedatatime = LeaveEdit.MinNoofLeaveAllowedatatime;

                LeaveMasterObj.MaxNoOfLeavesAllowedInMonth = LeaveEdit.MaxNoOfLeavesAllowedInMonth;

                //Added by Rajas on 29 MARCH 2017
                LeaveMasterObj.MarkedAsDelete = LeaveEdit.MarkedAsDelete;  // MarkedAsDelete

                // Checkbox null value handling added by Rajas on 10 MARCH 2017 START

                // AccumulationAllowedOrNot
                if (LeaveEdit.AccumulationAllowedOrNot == null)
                {
                    LeaveMasterObj.AccumulationAllowedOrNot = false;
                }
                else
                {
                    LeaveMasterObj.AccumulationAllowedOrNot = LeaveEdit.AccumulationAllowedOrNot;
                }

                // EncashmentAllowedOrNot
                if (LeaveEdit.AccumulationAllowedOrNot == null)
                {
                    LeaveMasterObj.EncashmentAllowedOrNot = false;
                }
                else
                {
                    LeaveMasterObj.EncashmentAllowedOrNot = LeaveEdit.EncashmentAllowedOrNot;
                }

                // NegativeBalanceAllowed
                if (LeaveEdit.NegativeBalanceAllowed == null)
                {
                    LeaveMasterObj.NegativeBalanceAllowed = false;
                }
                else
                {
                    LeaveMasterObj.NegativeBalanceAllowed = LeaveEdit.NegativeBalanceAllowed;
                }

                // IsCarryForword
                if (LeaveEdit.IsCarryForword == null)
                {
                    LeaveMasterObj.IsCarryForword = false;
                }
                else
                {
                    LeaveMasterObj.IsCarryForword = LeaveEdit.IsCarryForword;
                }

                // WOBetLevConsiderLeave
                if (LeaveEdit.WOBetLevConsiderLeave == null)
                {
                    LeaveMasterObj.WOBetLevConsiderLeave = false;
                }
                else
                {
                    LeaveMasterObj.WOBetLevConsiderLeave = LeaveEdit.WOBetLevConsiderLeave;
                }

                // HHBetLevConsiderLeave
                if (LeaveEdit.HHBetLevConsiderLeave == null)
                {
                    LeaveMasterObj.HHBetLevConsiderLeave = false;
                }
                else
                {
                    LeaveMasterObj.HHBetLevConsiderLeave = LeaveEdit.HHBetLevConsiderLeave;
                }

                // ISHalfAllowed
                if (LeaveEdit.ISHalfAllowed == null)
                {
                    LeaveMasterObj.ISHalfAllowed = false;
                }
                else
                {
                    LeaveMasterObj.ISHalfAllowed = LeaveEdit.ISHalfAllowed;
                }

                // IsLeaveCombination
                if (LeaveEdit.IsLeaveCombination == null)
                {
                    LeaveMasterObj.IsLeaveCombination = false;
                }
                else
                {
                    LeaveMasterObj.IsLeaveCombination = LeaveEdit.IsLeaveCombination;
                }
                // Checkbox null value handling added by Rajas on 10 MARCH 2017 END

                LeaveMasterObj.MaxAccumulationDays = LeaveEdit.MaxAccumulationDays;

                LeaveMasterObj.NoOfDaysEncashed = LeaveEdit.NoOfDaysEncashed;

                LeaveMasterObj.EmployeeGrpId = LeaveEdit.EmployeeGroup.EmployeeGroupId;

                LeaveMasterObj.BranchId = LeaveEdit.BranchId;

                LeaveMasterObj.CompanyId = LeaveEdit.Company.CompanyId;

                LeaveMasterObj.LeaveType = LeaveEdit.LeaveType;

                LeaveMasterObj.MaxNoOfLeavesAllowedAtaTime = LeaveEdit.MaxNoOfLeavesAllowedAtaTime;

                // Added this tow extra fields to mapp with Eviska database, by Rajas on 1 MARCH 2017
                LeaveMasterObj.ApplicableToMaleFemale = LeaveEdit.ApplicableToMaleFemale;

                LeaveMasterObj.LeaveCreditTypeId = LeaveEdit.LeaveCreditTypeId;

                //CODE ADDED BY SHRADDHA ON 16 FEB 2018 START
                // IsAttachmentNeeded
                if (LeaveEdit.IsAttachmentNeeded == null)
                {
                    LeaveMasterObj.IsAttachmentNeeded = false;
                }
                else
                {
                    LeaveMasterObj.IsAttachmentNeeded = LeaveEdit.IsAttachmentNeeded;
                }
                LeaveMasterObj.AttachmentRequiredForMinNoOfLeave = LeaveEdit.AttachmentRequiredForMinNoOfLeave;
                //CODE ADDED BY SHRADDHA ON 16 FEB 2018 END

                PopulateDropDownEdit(LeaveMasterObj);

                if (LeaveEdit.Prefix == null && LeaveEdit.Suffix == null)
                {

                    //ADDED BY PUSHKAR ON 10 MAY 2018 FOR PREFIX and SUFFIX
                    List<WetosDB.LeaveMaster> LeaveMaster = WetosDB.LeaveMasters.Where(a => a.Company.CompanyId == LeaveMasterObj.CompanyId && a.BranchId == LeaveMasterObj.BranchId
                        && a.EmployeeGroup.EmployeeGroupId == LeaveMasterObj.EmployeeGrpId && (a.MarkedAsDelete == 0 || a.MarkedAsDelete == null) && a.LeaveId != LeaveMasterObj.LeaveId).ToList();

                    ViewBag.LeaveListPre = new SelectList(LeaveMaster, " LeaveId", "LeaveCode").ToList();
                    ViewBag.LeaveListSuf = new SelectList(LeaveMaster, " LeaveId", "LeaveCode").ToList();

                }
                else
                {
                    List<WetosDB.LeaveMaster> LeaveMaster = WetosDB.LeaveMasters.Where(a => a.Company.CompanyId == LeaveMasterObj.CompanyId && a.BranchId == LeaveMasterObj.BranchId
                        && a.EmployeeGroup.EmployeeGroupId == LeaveMasterObj.EmployeeGrpId && (a.MarkedAsDelete == 0 || a.MarkedAsDelete == null) && a.LeaveId != LeaveMasterObj.LeaveId).ToList();


                    ViewBag.LeaveListPre = new SelectList(LeaveMaster, " LeaveId", "LeaveCode").ToList();
                    ViewBag.LeaveListSuf = new SelectList(LeaveMaster, " LeaveId", "LeaveCode").ToList();

                    LeaveMasterObj.PrefixId = LeaveEdit.Prefix;
                    LeaveMasterObj.SuffixId = LeaveEdit.Suffix;

                    LeaveMasterObj.PrefixIdStr = LeaveEdit.Prefix;
                    LeaveMasterObj.SuffixIdStr = LeaveEdit.Suffix;

                }


                // Updated by Rajas on 01 MARCH 2017
                return View(LeaveMasterObj); //return View(LeaveEdit);
            }

            catch (System.Exception ex)
            {
                //PopulateDropDownEdit();

                AddAuditTrail("Leave details edit failed due to " + ex.Message);

                Error("Leave details edit failed");

                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Edit post
        /// Added by Rajas on 25 OCT 2016
        /// </summary>
        /// <param name="id"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(int id, LeaveMasterModel LeaveMasterObj, FormCollection fc)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string UpdateStatus = string.Empty;  // Added by Rajas on 27 MARCH 2017

                    // Added by Rajas on 15 MAY 2017
                    bool IsEdit = true;

                    //string Prefix = fc["PrefixId"];

                    if (UpdateLeaveMasterData(LeaveMasterObj, IsEdit, fc, ref UpdateStatus) == true)
                    {
                        // ADDED BY RAJAS FOR AuditLog ON 27 DEC 2016
                        AddAuditTrail("Success - Leave : " + LeaveMasterObj.LeaveCode + " is updated successfully"); // Updated by Rajas on 16 JAN 2017


                        //ADDED CODE FOR SUCCESS MESSAGE BY RAJAS ON 16 JAN 2017
                        Success("Success - Leave : " + LeaveMasterObj.LeaveCode + " is updated successfully"); // Updated by Rajas on 16 JAN 2017
                    }
                    else
                    {
                        return ReportError(LeaveMasterObj, UpdateStatus);
                    }

                    return RedirectToAction("Index");
                }
                else
                {
                    PopulateDropDownEdit(LeaveMasterObj);

                    return View(LeaveMasterObj);
                }
            }
            catch (System.Exception ex)
            {
                // ADDED BY RAJAS FOR AuditLog ON 27 DEC 2016
                AddAuditTrail("Exception - Leave : " + LeaveMasterObj.LeaveCode + " update failed due to " + ex.Message); // Updated by Rajas on 16 JAN 2017


                //ADDED CODE FOR SUCCESS MESSAGE BY RAJAS ON 16 JAN 2017
                Error("Exception - Leave : " + LeaveMasterObj.LeaveCode + " update failed due to " + ex.Message); // Updated by Rajas on 16 JAN 2017

                PopulateDropDownEdit(LeaveMasterObj);

                return View(LeaveMasterObj);
            }
        }

        /// <summary>
        /// Updated by Rajas on 27 MARCH 2017
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int id)
        {
            try
            {
                WetosDB.LeaveMaster LeaveEdit = new WetosDB.LeaveMaster();
                LeaveEdit = WetosDB.LeaveMasters.Single(b => b.LeaveId == id);

                LeaveMasterModel LeaveMasterObj = new LeaveMasterModel();

                LeaveMasterObj.LeaveId = LeaveEdit.LeaveId;

                LeaveMasterObj.LeaveCode = LeaveEdit.LeaveCode;

                LeaveMasterObj.LeaveName = LeaveEdit.LeaveName;

                LeaveMasterObj.LeaveDescription = LeaveEdit.LeaveDescription;

                LeaveMasterObj.NoOfDaysAllowedInYear = LeaveEdit.NoOfDaysAllowedInYear;

                LeaveMasterObj.MaxNoOfTimesAllowedInYear = LeaveEdit.MaxNoOfTimesAllowedInYear;

                LeaveMasterObj.MinNoofLeaveAllowedatatime = LeaveEdit.MinNoofLeaveAllowedatatime;

                LeaveMasterObj.MaxNoOfLeavesAllowedInMonth = LeaveEdit.MaxNoOfLeavesAllowedInMonth;

                //Added by Rajas on 29 MARCH 2017
                LeaveMasterObj.MarkedAsDelete = LeaveEdit.MarkedAsDelete;  // MarkedAsDelete

                // Checkbox null value handling added by Rajas on 10 MARCH 2017 START

                // AccumulationAllowedOrNot
                if (LeaveEdit.AccumulationAllowedOrNot == null)
                {
                    LeaveMasterObj.AccumulationAllowedOrNot = false;
                }
                else
                {
                    LeaveMasterObj.AccumulationAllowedOrNot = LeaveEdit.AccumulationAllowedOrNot;
                }

                // EncashmentAllowedOrNot
                if (LeaveEdit.AccumulationAllowedOrNot == null)
                {
                    LeaveMasterObj.EncashmentAllowedOrNot = false;
                }
                else
                {
                    LeaveMasterObj.EncashmentAllowedOrNot = LeaveEdit.EncashmentAllowedOrNot;
                }

                // NegativeBalanceAllowed
                if (LeaveEdit.NegativeBalanceAllowed == null)
                {
                    LeaveMasterObj.NegativeBalanceAllowed = false;
                }
                else
                {
                    LeaveMasterObj.NegativeBalanceAllowed = LeaveEdit.NegativeBalanceAllowed;
                }

                // IsCarryForword
                if (LeaveEdit.IsCarryForword == null)
                {
                    LeaveMasterObj.IsCarryForword = false;
                }
                else
                {
                    LeaveMasterObj.IsCarryForword = LeaveEdit.IsCarryForword;
                }

                // WOBetLevConsiderLeave
                if (LeaveEdit.WOBetLevConsiderLeave == null)
                {
                    LeaveMasterObj.WOBetLevConsiderLeave = false;
                }
                else
                {
                    LeaveMasterObj.WOBetLevConsiderLeave = LeaveEdit.WOBetLevConsiderLeave;
                }

                // HHBetLevConsiderLeave
                if (LeaveEdit.HHBetLevConsiderLeave == null)
                {
                    LeaveMasterObj.HHBetLevConsiderLeave = false;
                }
                else
                {
                    LeaveMasterObj.HHBetLevConsiderLeave = LeaveEdit.HHBetLevConsiderLeave;
                }

                // ISHalfAllowed
                if (LeaveEdit.ISHalfAllowed == null)
                {
                    LeaveMasterObj.ISHalfAllowed = false;
                }
                else
                {
                    LeaveMasterObj.ISHalfAllowed = LeaveEdit.ISHalfAllowed;
                }

                // IsLeaveCombination
                if (LeaveEdit.IsLeaveCombination == null)
                {
                    LeaveMasterObj.IsLeaveCombination = false;
                }
                else
                {
                    LeaveMasterObj.IsLeaveCombination = LeaveEdit.IsLeaveCombination;
                }
                // Checkbox null value handling added by Rajas on 10 MARCH 2017 END

                //CODE ADDED BY SHRADDHA ON 16 FEB 2018 START
                // IsLeaveCombination
                if (LeaveEdit.IsAttachmentNeeded == null)
                {
                    LeaveMasterObj.IsAttachmentNeeded = false;
                }
                else
                {
                    LeaveMasterObj.IsAttachmentNeeded = LeaveEdit.IsAttachmentNeeded;
                }
                LeaveMasterObj.AttachmentRequiredForMinNoOfLeave = LeaveEdit.AttachmentRequiredForMinNoOfLeave;
                //CODE ADDED BY SHRADDHA ON 16 FEB 2018 END

                LeaveMasterObj.MaxAccumulationDays = LeaveEdit.MaxAccumulationDays;

                LeaveMasterObj.NoOfDaysEncashed = LeaveEdit.NoOfDaysEncashed;

                LeaveMasterObj.EmployeeGrpId = LeaveEdit.EmployeeGroup.EmployeeGroupId;

                LeaveMasterObj.BranchId = LeaveEdit.BranchId;

                LeaveMasterObj.CompanyId = LeaveEdit.Company.CompanyId;

                LeaveMasterObj.LeaveType = LeaveEdit.LeaveType;

                LeaveMasterObj.MaxNoOfLeavesAllowedAtaTime = LeaveEdit.MaxNoOfLeavesAllowedAtaTime;

                // Added this tow extra fields to mapp with Eviska database, by Rajas on 1 MARCH 2017
                LeaveMasterObj.ApplicableToMaleFemale = LeaveEdit.ApplicableToMaleFemale;

                LeaveMasterObj.LeaveCreditTypeId = LeaveEdit.LeaveCreditTypeId;

                PopulateDropDownEdit(LeaveMasterObj);

                // Updated by Rajas on 01 MARCH 2017
                return View(LeaveMasterObj); //return View(LeaveEdit);
            }

            catch (System.Exception ex)
            {
                //PopulateDropDownEdit();

                AddAuditTrail("Error in viewing Leave details due to " + ex.Message);

                Error("Error in viewing Leave details");

                return RedirectToAction("Index");
            }

        }

        /// <summary>
        /// Added by Rajas on 29 MARCH 2017
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            try
            {
                WetosDB.LeaveMaster LeaveEdit = new WetosDB.LeaveMaster();
                LeaveEdit = WetosDB.LeaveMasters.Single(b => b.LeaveId == id);

                LeaveMasterModel LeaveMasterObj = new LeaveMasterModel();

                LeaveMasterObj.LeaveId = LeaveEdit.LeaveId;

                LeaveMasterObj.LeaveCode = LeaveEdit.LeaveCode;

                LeaveMasterObj.LeaveName = LeaveEdit.LeaveName;

                LeaveMasterObj.LeaveDescription = LeaveEdit.LeaveDescription;

                LeaveMasterObj.NoOfDaysAllowedInYear = LeaveEdit.NoOfDaysAllowedInYear;

                LeaveMasterObj.MaxNoOfTimesAllowedInYear = LeaveEdit.MaxNoOfTimesAllowedInYear;

                //MODIFIED BY PUSHKAR ON 28 NOV 2017 FOR NULL REFERENCE ERROR ON VIEW
                LeaveMasterObj.MinNoofLeaveAllowedatatime = LeaveEdit.MinNoofLeaveAllowedatatime == null ? 0 : LeaveEdit.MinNoofLeaveAllowedatatime;

                LeaveMasterObj.MaxNoOfLeavesAllowedInMonth = LeaveEdit.MaxNoOfLeavesAllowedInMonth;

                //Added by Rajas on 29 MARCH 2017
                LeaveMasterObj.MarkedAsDelete = LeaveEdit.MarkedAsDelete;  // MarkedAsDelete

                // Checkbox null value handling added by Rajas on 10 MARCH 2017 START

                // AccumulationAllowedOrNot
                if (LeaveEdit.AccumulationAllowedOrNot == null)
                {
                    LeaveMasterObj.AccumulationAllowedOrNot = false;
                }
                else
                {
                    LeaveMasterObj.AccumulationAllowedOrNot = LeaveEdit.AccumulationAllowedOrNot;
                }

                // EncashmentAllowedOrNot
                if (LeaveEdit.AccumulationAllowedOrNot == null)
                {
                    LeaveMasterObj.EncashmentAllowedOrNot = false;
                }
                else
                {
                    LeaveMasterObj.EncashmentAllowedOrNot = LeaveEdit.EncashmentAllowedOrNot;
                }

                // NegativeBalanceAllowed
                if (LeaveEdit.NegativeBalanceAllowed == null)
                {
                    LeaveMasterObj.NegativeBalanceAllowed = false;
                }
                else
                {
                    LeaveMasterObj.NegativeBalanceAllowed = LeaveEdit.NegativeBalanceAllowed;
                }

                // IsCarryForword
                if (LeaveEdit.IsCarryForword == null)
                {
                    LeaveMasterObj.IsCarryForword = false;
                }
                else
                {
                    LeaveMasterObj.IsCarryForword = LeaveEdit.IsCarryForword;
                }

                // WOBetLevConsiderLeave
                if (LeaveEdit.WOBetLevConsiderLeave == null)
                {
                    LeaveMasterObj.WOBetLevConsiderLeave = false;
                }
                else
                {
                    LeaveMasterObj.WOBetLevConsiderLeave = LeaveEdit.WOBetLevConsiderLeave;
                }

                // HHBetLevConsiderLeave
                if (LeaveEdit.HHBetLevConsiderLeave == null)
                {
                    LeaveMasterObj.HHBetLevConsiderLeave = false;
                }
                else
                {
                    LeaveMasterObj.HHBetLevConsiderLeave = LeaveEdit.HHBetLevConsiderLeave;
                }

                // ISHalfAllowed
                if (LeaveEdit.ISHalfAllowed == null)
                {
                    LeaveMasterObj.ISHalfAllowed = false;
                }
                else
                {
                    LeaveMasterObj.ISHalfAllowed = LeaveEdit.ISHalfAllowed;
                }

                // IsLeaveCombination
                if (LeaveEdit.IsLeaveCombination == null)
                {
                    LeaveMasterObj.IsLeaveCombination = false;
                }
                else
                {
                    LeaveMasterObj.IsLeaveCombination = LeaveEdit.IsLeaveCombination;
                }
                // Checkbox null value handling added by Rajas on 10 MARCH 2017 END

                LeaveMasterObj.MaxAccumulationDays = LeaveEdit.MaxAccumulationDays;

                LeaveMasterObj.NoOfDaysEncashed = LeaveEdit.NoOfDaysEncashed;

                LeaveMasterObj.EmployeeGrpId = LeaveEdit.EmployeeGroup.EmployeeGroupId;

                LeaveMasterObj.BranchId = LeaveEdit.BranchId;

                LeaveMasterObj.CompanyId = LeaveEdit.Company.CompanyId;

                LeaveMasterObj.LeaveType = LeaveEdit.LeaveType;

                LeaveMasterObj.MaxNoOfLeavesAllowedAtaTime = LeaveEdit.MaxNoOfLeavesAllowedAtaTime;

                // Added this tow extra fields to mapp with Eviska database, by Rajas on 1 MARCH 2017
                LeaveMasterObj.ApplicableToMaleFemale = LeaveEdit.ApplicableToMaleFemale;

                LeaveMasterObj.LeaveCreditTypeId = LeaveEdit.LeaveCreditTypeId;

                PopulateDropDownEdit(LeaveMasterObj);

                // Updated by Rajas on 01 MARCH 2017
                return View(LeaveMasterObj); //return View(LeaveEdit);
            }

            catch (System.Exception ex)
            {
                //PopulateDropDownEdit();

                AddAuditTrail("Leave delete failed due to " + ex.Message);

                Error("Leave delete failed");

                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Added by Rajas on 29 MARCH 2017
        /// DELETE POST
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fc"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(int id, FormCollection fc)
        {
            try
            {
                WetosDB.LeaveMaster LeaveMasterObj = WetosDB.LeaveMasters.Where(a => a.LeaveId == id).FirstOrDefault();

                if (LeaveMasterObj != null)
                {
                    LeaveMasterObj.MarkedAsDelete = 1;

                    WetosDB.SaveChanges();

                    Success("Leave : " + LeaveMasterObj.LeaveCode + " deleted successfully");

                    AddAuditTrail("Leave : " + LeaveMasterObj.LeaveCode + " deleted successfully");
                }

                return RedirectToAction("Index");
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Please try again!");

                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Fill Drop down data 
        /// </summary>
        /// <returns></returns>
        /// Updated by Rajas on 27 MARCH 2017
        public ActionResult PopulateDropDown()
        {
            try
            {
                // Updated by Rajas on 6 MAY 2017 to get data which is not MarkedAsDelete
                #region COMPANY LIST CODE ADDED BY SHRADDHA ON 30 MAR 2018
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //var CompanyObj = WetosDB.Companies.Select(a => new { CompanyId = a.CompanyId, CompanyName = a.CompanyName }).ToList();
                var CompanyObj = WetosDB.SP_GetCompanyList(EmployeeId).Select(m => new { Companyid = m.CompanyId, CompanyName = m.CompanyName }).ToList();
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
                #endregion

                ViewBag.CompanyList = new SelectList(CompanyObj, " CompanyId", "CompanyName").ToList();

                var BranchObj = new List<Branch>(); //WetosDB.Branches.Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList();

                ViewBag.BranchList = new SelectList(BranchObj, " BranchId", "BranchName").ToList();

                var EmployeeGroupObj = new List<EmployeeGroup>();  //WetosDB.EmployeeGroups.Select(a => new { EmployeeGroupId = a.EmployeeGroupId, GroupName = a.EmployeeGroupName }).ToList();

                ViewBag.EmployeeGroupList = new SelectList(EmployeeGroupObj, "EmployeeGroupId", "GroupName").ToList();

                // Added by Rajas on 10 MARCH 2017 for Leave credit type dropdown data
                var LeaveCreditType = WetosDB.DropdownDatas.Where(a => a.TypeId == 14).Select(a => new { LeaveCreditTypeId = a.Value, LeaveCreditType = a.Text }).ToList();

                ViewBag.LeaveCreditTypeList = new SelectList(LeaveCreditType, "LeaveCreditTypeId", "LeaveCreditType").ToList();

                return View();
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                return View();
            }
        }

        /// <summary>
        /// Added by Rajas on 1 MARCH 2017
        /// </summary>
        /// <returns></returns>
        ///  Updated by Rajas on 27 MARCH 2017
        ///  PARAMETERS ADDED BY RAJAS ON 31 MAY 2017
        public ActionResult PopulateDropDownEdit(LeaveMasterModel LeaveMasterObj)
        {
            try
            {
                // Updated by Rajas on 6 MAY 2017 to get data which is not MarkedAsDelete START
                #region COMPANY LIST CODE ADDED BY SHRADDHA ON 30 MAR 2018
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //var CompanyObj = WetosDB.Companies.Select(a => new { CompanyId = a.CompanyId, CompanyName = a.CompanyName }).ToList();
                var CompanyObj = WetosDB.SP_GetCompanyList(EmployeeId).Select(m => new { Companyid = m.CompanyId, CompanyName = m.CompanyName }).ToList();
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
                #endregion

                ViewBag.CompanyList = new SelectList(CompanyObj, " CompanyId", "CompanyName").ToList();

                // Get branch as per company

                #region CODE FOR GET BRANCH LIST ADDED BY SHRADDHA ON 30 MAR 2018
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
                // var BranchObj = WetosDB.Branches.Where(a => (a.MarkedAsDelete == null || a.MarkedAsDelete == 0) 
                //&& a.Company.CompanyId == LeaveMasterObj.CompanyId).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList();
                var BranchObj = WetosDB.SP_GetBranchList(EmployeeId).Where(a => a.CompanyId == LeaveMasterObj.CompanyId).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList(); //.Select(m => new { BranchId = m.BranchId, BranchName = m.BranchName}).ToList();
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
                #endregion

                ViewBag.BranchList = new SelectList(BranchObj, " BranchId", "BranchName").ToList();

                // Get Employee group as per company and branch
                var EmployeeGroupObj = WetosDB.EmployeeGroups.Where(a => (a.MarkedAsDelete == null || a.MarkedAsDelete == 0)
                    && a.Company.CompanyId == LeaveMasterObj.CompanyId && a.Branch.BranchId == LeaveMasterObj.BranchId)
                    .Select(a => new { EmployeeGroupId = a.EmployeeGroupId, GroupName = a.EmployeeGroupName }).ToList();

                ViewBag.EmployeeGroupList = new SelectList(EmployeeGroupObj, "EmployeeGroupId", "GroupName").ToList();

                // Added by Rajas on 10 MARCH 2017 for Leave credit type dropdown data
                var LeaveCreditType = WetosDB.DropdownDatas.Where(a => a.TypeId == 14).Select(a => new { LeaveCreditTypeId = a.Value, LeaveCreditType = a.Text }).ToList();

                ViewBag.LeaveCreditTypeList = new SelectList(LeaveCreditType, "LeaveCreditTypeId", "LeaveCreditType").ToList();
                // Updated by Rajas on 6 MAY 2017 to get data which is not MarkedAsDelete END

                return View();
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

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

            // Updated by Rajas on 30 MAY 2017
            var BranchList = WetosDB.Branches.Where(a => a.Company.CompanyId == SelCompanyId && (a.MarkedAsDelete == null || a.MarkedAsDelete == 0)).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList();

            return Json(BranchList);
        }

        /// <summary>
        /// Json return for to get Employeegroup dropdown list on basis of branch selection
        /// Added by Rajas on 27 DEC 2016
        /// </summary>
        /// <returns></returns>
        public JsonResult GetEmployeeGroup(string Branchid, string Companyid)
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

            // Updated by Rajas on 30 MAY 2017
            var EmployeeGroupList = WetosDB.EmployeeGroups.Where(a => a.Branch.BranchId == SelBranchId && a.Company.CompanyId == SelCompanyId
                && (a.MarkedAsDelete == null || a.MarkedAsDelete == 0)).Select(a => new { EmployeeGroupId = a.EmployeeGroupId, EmployeeGroupName = a.EmployeeGroupName }).ToList();

            return Json(EmployeeGroupList);
        }

        /// <summary>
        /// Common Error reporting function
        /// Added by Rajas on 3 JUNE 2017
        /// </summary>
        /// <param name="DepartmentObj"></param>
        /// <param name="ErrorMessage"></param>
        /// <returns></returns>
        public ActionResult ReportError(LeaveMasterModel LeaveMasterObj, string ErrorMessage)
        {
            PopulateDropDownEdit(LeaveMasterObj);//

            AddAuditTrail(ErrorMessage);
            Error(ErrorMessage);

            return View(LeaveMasterObj);
        }

        /// <summary>
        /// Common function to validate the data for edit or create
        /// Added by Rajas on 30 DEC 2016
        /// </summary>
        /// Updated by Rajas on 27 MARCH 2017
        private bool UpdateLeaveMasterData(LeaveMasterModel LeaveMasterObj, bool IsEdit, FormCollection fc, ref string UpdateStatus)
        {
            bool ReturnStatus = false;
            try
            {
                #region IF ALL BRANCH ARE SELECTED
                if (LeaveMasterObj.BranchId == 0)
                {
                    List<Int32> BranchObj = WetosDB.Branches.Where(a => a.Company.CompanyId == LeaveMasterObj.CompanyId).Select(a => a.BranchId).ToList();

                    foreach (Int32 BranchId in BranchObj)
                    {
                        List<Int32> EmpGrp = WetosDB.EmployeeGroups.Where(a => a.Branch.BranchId == BranchId).Select(a => a.EmployeeGroupId).ToList();

                        foreach (Int32 EmpGrpId in EmpGrp)
                        {

                            WetosDB.LeaveMaster LeaveMasterTblObj = WetosDB.LeaveMasters.Where(a => (a.LeaveId == LeaveMasterObj.LeaveId || a.LeaveCode.ToUpper() == LeaveMasterObj.LeaveCode.ToUpper())
                                && a.Company.CompanyId == LeaveMasterObj.CompanyId && a.BranchId == BranchId && a.EmployeeGroup.EmployeeGroupId == EmpGrpId
                                && a.MarkedAsDelete == 0).FirstOrDefault();

                            ////AUDIT LOG GENERATION ADDED BY PUSHKAR ON 9 SEPTEMBER 2017 FOR EDIT
                            WetosDB.LeaveMaster LeaveMasterTblObjEDIT = WetosDB.LeaveMasters.Where(a => (a.LeaveId == LeaveMasterObj.LeaveId || a.LeaveCode.ToUpper() == LeaveMasterObj.LeaveCode.ToUpper())
                               && a.Company.CompanyId == LeaveMasterObj.CompanyId && a.BranchId == BranchId && a.EmployeeGroup.EmployeeGroupId == EmpGrpId
                               && a.MarkedAsDelete == 0).FirstOrDefault();

                            // ADDED BY RAJAS ON 03 JUNE 2017 START
                            bool IsNew = false;

                            // IS OBJECT PRESET
                            if (LeaveMasterTblObj != null)  // Added by Rajas on 2 JUNE 2017
                            {
                                if (IsEdit == false)  // CREATE            
                                {
                                    UpdateStatus = "Leave Master already available."; //WetosEmployeeController.GetErrorMessage(1);

                                    //AddAuditTrail("Error in Department update : " + UpdateStatus);
                                    //return ReturnStatus;
                                    break;
                                }
                            }
                            else
                            {
                                if (IsEdit == false) // CREATE    
                                {
                                    LeaveMasterTblObj = new WetosDB.LeaveMaster();
                                    IsNew = true;
                                }
                                else // EDIT    
                                {
                                    UpdateStatus = "Error in updating Leave Master."; // WetosEmployeeController.GetErrorMessage(1); 
                                    //AddAuditTrail("Error in Department update : " + UpdateStatus);
                                    //return ReturnStatus;
                                    break;
                                }
                            }
                            // ADDED BY RAJAS ON 03 JUNE 2017 END

                            // New Leave table object
                            LeaveMasterTblObj.LeaveCode = LeaveMasterObj.LeaveCode;

                            LeaveMasterTblObj.LeaveName = LeaveMasterObj.LeaveName;

                            LeaveMasterTblObj.LeaveDescription = LeaveMasterObj.LeaveDescription;

                            LeaveMasterTblObj.NoOfDaysAllowedInYear = LeaveMasterObj.NoOfDaysAllowedInYear;

                            LeaveMasterTblObj.MaxNoOfTimesAllowedInYear = LeaveMasterObj.MaxNoOfTimesAllowedInYear;

                            LeaveMasterTblObj.MinNoofLeaveAllowedatatime = LeaveMasterObj.MinNoofLeaveAllowedatatime;

                            LeaveMasterTblObj.MaxNoOfLeavesAllowedAtaTime = LeaveMasterObj.MaxNoOfLeavesAllowedAtaTime; // Updated by Rajas on 11 MARCH 2017

                            LeaveMasterTblObj.MaxNoOfLeavesAllowedInMonth = LeaveMasterObj.MaxNoOfLeavesAllowedInMonth;

                            LeaveMasterTblObj.MaxAccumulationDays = LeaveMasterObj.MaxAccumulationDays;

                            LeaveMasterTblObj.NoOfDaysEncashed = LeaveMasterObj.NoOfDaysEncashed; // Added by Rajas on 30 MAY 2017

                            // Updated by Rajas on 1 MARCH 2017
                            LeaveMasterTblObj.EmployeeGroup = WetosDB.EmployeeGroups.Where(a => a.EmployeeGroupId == EmpGrpId).FirstOrDefault();   // EmployeeGroup.EmployeeGroupId

                            LeaveMasterTblObj.Company = WetosDB.Companies.Where(a => a.CompanyId == LeaveMasterObj.CompanyId).FirstOrDefault(); // Company.CompanyId

                            LeaveMasterTblObj.BranchId = BranchId;

                            LeaveMasterTblObj.LeaveType = LeaveMasterObj.LeaveType;

                            LeaveMasterTblObj.MinNoofLeaveAllowedatatime = LeaveMasterObj.MinNoofLeaveAllowedatatime;

                            LeaveMasterTblObj.IsLeaveCombination = LeaveMasterObj.IsLeaveCombination;

                            // Added this tow extra fields to mapp with Eviska database, by Rajas on 1 MARCH 2017

                            LeaveMasterTblObj.ApplicableToMaleFemale = LeaveMasterObj.ApplicableToMaleFemale;

                            LeaveMasterTblObj.LeaveCreditTypeId = LeaveMasterObj.LeaveCreditTypeId;

                            // Added by Rajas on 29 MARCH 2017
                            LeaveMasterTblObj.MarkedAsDelete = 0; // Default value

                            // Checkbox null value handling added by Rajas on 11 MARCH 2017 START

                            // AccumulationAllowedOrNot
                            if (LeaveMasterObj.AccumulationAllowedOrNot == null)
                            {
                                LeaveMasterTblObj.AccumulationAllowedOrNot = false;
                            }
                            else
                            {
                                LeaveMasterTblObj.AccumulationAllowedOrNot = LeaveMasterObj.AccumulationAllowedOrNot;
                            }

                            // EncashmentAllowedOrNot
                            if (LeaveMasterObj.AccumulationAllowedOrNot == null)
                            {
                                LeaveMasterTblObj.EncashmentAllowedOrNot = false;
                            }
                            else
                            {
                                LeaveMasterTblObj.EncashmentAllowedOrNot = LeaveMasterObj.EncashmentAllowedOrNot;
                            }

                            // NegativeBalanceAllowed
                            if (LeaveMasterObj.NegativeBalanceAllowed == null)
                            {
                                LeaveMasterTblObj.NegativeBalanceAllowed = false;
                            }
                            else
                            {
                                LeaveMasterTblObj.NegativeBalanceAllowed = LeaveMasterObj.NegativeBalanceAllowed;
                            }

                            // IsCarryForword
                            if (LeaveMasterObj.IsCarryForword == null)
                            {
                                LeaveMasterTblObj.IsCarryForword = false;
                            }
                            else
                            {
                                LeaveMasterTblObj.IsCarryForword = LeaveMasterObj.IsCarryForword;
                            }

                            // WOBetLevConsiderLeave
                            if (LeaveMasterObj.WOBetLevConsiderLeave == null)
                            {
                                LeaveMasterTblObj.WOBetLevConsiderLeave = false;
                            }
                            else
                            {
                                LeaveMasterTblObj.WOBetLevConsiderLeave = LeaveMasterObj.WOBetLevConsiderLeave;
                            }

                            // HHBetLevConsiderLeave
                            if (LeaveMasterObj.HHBetLevConsiderLeave == null)
                            {
                                LeaveMasterTblObj.HHBetLevConsiderLeave = false;
                            }
                            else
                            {
                                LeaveMasterTblObj.HHBetLevConsiderLeave = LeaveMasterObj.HHBetLevConsiderLeave;
                            }

                            // ISHalfAllowed
                            if (LeaveMasterObj.ISHalfAllowed == null)
                            {
                                LeaveMasterTblObj.ISHalfAllowed = false;
                            }
                            else
                            {
                                LeaveMasterTblObj.ISHalfAllowed = LeaveMasterObj.ISHalfAllowed;
                            }

                            // IsLeaveCombination
                            if (LeaveMasterObj.IsLeaveCombination == null)
                            {
                                LeaveMasterTblObj.IsLeaveCombination = false;
                            }
                            else
                            {
                                LeaveMasterTblObj.IsLeaveCombination = LeaveMasterObj.IsLeaveCombination;
                            }
                            // Checkbox null value handling added by Rajas on 11 MARCH 2017 END

                            //CODE ADDED BY SHRADDHA ON 16 FEB 2018 START
                            // IsAttachmentNeeded
                            if (LeaveMasterObj.IsAttachmentNeeded == null)
                            {
                                LeaveMasterTblObj.IsAttachmentNeeded = false;
                            }
                            else
                            {
                                LeaveMasterTblObj.IsAttachmentNeeded = LeaveMasterObj.IsAttachmentNeeded;
                            }
                            LeaveMasterTblObj.AttachmentRequiredForMinNoOfLeave = LeaveMasterObj.AttachmentRequiredForMinNoOfLeave;
                            //CODE ADDED BY SHRADDHA ON 16 FEB 2018 END


                            LeaveMasterTblObj.Prefix = fc["PrefixId"];//LeaveMasterObj.PrefixId;
                            LeaveMasterTblObj.Suffix = fc["SuffixId"];//LeaveMasterObj.SuffixId;

                            // Add new table object 
                            if (IsNew)
                            {
                                // Added by Rajas on 14 MARCH 2017 for Calculaig max of LeaveId and save START
                                LeaveMaster LastLeaveId = WetosDB.LeaveMasters.OrderByDescending(a => a.LeaveId).FirstOrDefault();

                                int MaxOfLeaveId = 0;

                                int Max = 0;
                                if (LastLeaveId != null)
                                {
                                    Max = Convert.ToInt32(LastLeaveId.LeaveId);
                                }


                                MaxOfLeaveId = Max + 1;

                                LeaveMasterTblObj.LeaveId = MaxOfLeaveId;
                                // END

                                //ADDED BY PUSHKAR FOR PREFIX AND SUFFIX ON 10 MAY 2018



                                WetosDB.LeaveMasters.Add(LeaveMasterTblObj);
                            }

                            WetosDB.SaveChanges();


                            //-------------------------------------------AuditLog---------------------------------------------------------------------------

                            if (IsNew)
                            {
                                //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 11 SEPTEMBER 2017
                                #region ADD AUDIT LOG
                                //OLD RECORD IS BLANK
                                string Newrecord = "Leave Code : " + LeaveMasterTblObj.LeaveCode + ", LeaveName :"
                                    + LeaveMasterTblObj.LeaveName + ", LeaveDescription :" + LeaveMasterTblObj.LeaveDescription + ", LeaveType :"
                                    + LeaveMasterTblObj.LeaveType + ", Employee GroupID :" + LeaveMasterTblObj.EmployeeGroup.EmployeeGroupId + ",Accumulation Allowed or Not :"
                                    + LeaveMasterTblObj.AccumulationAllowedOrNot + ", EncashmentAllowedOrNot :" + LeaveMasterTblObj.EncashmentAllowedOrNot
                                    + ", HHBetLevConsiderLeave :" + LeaveMasterTblObj.HHBetLevConsiderLeave + ", IsCarryForword :" + LeaveMasterTblObj.IsCarryForword
                                    + ", ISHalfAllowed :" + LeaveMasterTblObj.ISHalfAllowed + ", MaxNoOfLeavesAllowedAtaTime :"
                                    + LeaveMasterTblObj.MaxNoOfLeavesAllowedAtaTime + ",MaxNoOfLeavesAllowedInMonth:"
                                    + LeaveMasterTblObj.MaxNoOfLeavesAllowedInMonth + ", MaxNoOfTimesAllowedInYear :"
                                    + LeaveMasterTblObj.NoOfDaysAllowedInYear + ", NoOfDaysAllowedInYear : " + LeaveMasterTblObj.NoOfDaysAllowedInYear
                                    + ", WOBetLevConsiderLeave : " + LeaveMasterTblObj.WOBetLevConsiderLeave + ", NegativeBalanceAllowed : "
                                    + LeaveMasterTblObj.NegativeBalanceAllowed + ", Branch ID : " + LeaveMasterTblObj.BranchId + ", Company  ID : "
                                    + LeaveMasterTblObj.Company.CompanyId;


                                //LOGIN ID IS TAKEN FROM SESSION PERSISTER
                                string Formname = "RULE SETTING-LEAVE MASTER";
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
                                string Oldrecord = "Leave Code : " + LeaveMasterTblObjEDIT.LeaveCode + ", LeaveName :"
                                    + LeaveMasterTblObjEDIT.LeaveName + ", LeaveDescription :" + LeaveMasterTblObjEDIT.LeaveDescription + ", LeaveType :"
                                    + LeaveMasterTblObjEDIT.LeaveType + ", Employee GroupID :" + LeaveMasterTblObjEDIT.EmployeeGroup.EmployeeGroupId + ",Accumulation Allowed or Not :"
                                    + LeaveMasterTblObjEDIT.AccumulationAllowedOrNot + ", EncashmentAllowedOrNot :" + LeaveMasterTblObjEDIT.EncashmentAllowedOrNot
                                    + ", HHBetLevConsiderLeave :" + LeaveMasterTblObjEDIT.HHBetLevConsiderLeave + ", IsCarryForword :" + LeaveMasterTblObjEDIT.IsCarryForword
                                    + ", ISHalfAllowed :" + LeaveMasterTblObjEDIT.ISHalfAllowed + ", MaxNoOfLeavesAllowedAtaTime :"
                                    + LeaveMasterTblObjEDIT.MaxNoOfLeavesAllowedAtaTime + ",MaxNoOfLeavesAllowedInMonth:"
                                    + LeaveMasterTblObjEDIT.MaxNoOfLeavesAllowedInMonth + ", MaxNoOfTimesAllowedInYear :"
                                    + LeaveMasterTblObjEDIT.NoOfDaysAllowedInYear + ", NoOfDaysAllowedInYear : " + LeaveMasterTblObjEDIT.NoOfDaysAllowedInYear
                                    + ", WOBetLevConsiderLeave : " + LeaveMasterTblObjEDIT.WOBetLevConsiderLeave + ", NegativeBalanceAllowed : "
                                    + LeaveMasterTblObjEDIT.NegativeBalanceAllowed + ", Branch ID : " + LeaveMasterTblObjEDIT.BranchId + ", Company  ID : "
                                    + LeaveMasterTblObjEDIT.Company.CompanyId;
                                //---x---------x-----------------x------------x----------------x-----------x-------------x------------x--------------x----------x---------x-----
                                string Newrecord = "Leave Code : " + LeaveMasterTblObj.LeaveCode + ", LeaveName :"
                                    + LeaveMasterTblObj.LeaveName + ", LeaveDescription :" + LeaveMasterTblObj.LeaveDescription + ", LeaveType :"
                                    + LeaveMasterTblObj.LeaveType + ", Employee GroupID :" + LeaveMasterTblObj.EmployeeGroup.EmployeeGroupId + ",Accumulation Allowed or Not :"
                                    + LeaveMasterTblObj.AccumulationAllowedOrNot + ", EncashmentAllowedOrNot :" + LeaveMasterTblObj.EncashmentAllowedOrNot
                                    + ", HHBetLevConsiderLeave :" + LeaveMasterTblObj.HHBetLevConsiderLeave + ", IsCarryForword :" + LeaveMasterTblObj.IsCarryForword
                                    + ", ISHalfAllowed :" + LeaveMasterTblObj.ISHalfAllowed + ", MaxNoOfLeavesAllowedAtaTime :"
                                    + LeaveMasterTblObj.MaxNoOfLeavesAllowedAtaTime + ",MaxNoOfLeavesAllowedInMonth:"
                                    + LeaveMasterTblObj.MaxNoOfLeavesAllowedInMonth + ", MaxNoOfTimesAllowedInYear :"
                                    + LeaveMasterTblObj.NoOfDaysAllowedInYear + ", NoOfDaysAllowedInYear : " + LeaveMasterTblObj.NoOfDaysAllowedInYear
                                    + ", WOBetLevConsiderLeave : " + LeaveMasterTblObj.WOBetLevConsiderLeave + ", NegativeBalanceAllowed : "
                                    + LeaveMasterTblObj.NegativeBalanceAllowed + ", Branch ID : " + LeaveMasterTblObj.BranchId + ", Company  ID : "
                                    + LeaveMasterTblObj.Company.CompanyId;


                                //LOGIN ID IS TAKEN FROM SESSION PERSISTER
                                string Formname = "RULE SETTING-LEAVE MASTER";
                                //ACTION IS UPDATE
                                string Message = " ";

                                WetosAdministrationController.GenerateAuditLogsUpdate(WetosDB, 0, Formname, Oldrecord,
                                    Newrecord, ref Message);
                                #endregion
                                //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 11 SEPTEMBER 2017
                            }

                            //-------------------------------------------AuditLog---------------------------------------------------------------------------




                        }

                    }

                    ReturnStatus = true;

                    return ReturnStatus;

                }
                #endregion

                #region IF ONLY EmployeeGrp is null
                else if (LeaveMasterObj.EmployeeGrpId == 0)
                {
                    List<Int32> EmpGrp = WetosDB.EmployeeGroups.Where(a => a.Branch.BranchId == LeaveMasterObj.BranchId).Select(a => a.EmployeeGroupId).ToList();

                    foreach (Int32 EmpGrpId in EmpGrp)
                    {

                        WetosDB.LeaveMaster LeaveMasterTblObj = WetosDB.LeaveMasters.Where(a => (a.LeaveId == LeaveMasterObj.LeaveId || a.LeaveCode.ToUpper() == LeaveMasterObj.LeaveCode.ToUpper())
                            && a.Company.CompanyId == LeaveMasterObj.CompanyId && a.BranchId == LeaveMasterObj.BranchId && a.EmployeeGroup.EmployeeGroupId == EmpGrpId
                            && a.MarkedAsDelete == 0).FirstOrDefault();

                        ////AUDIT LOG GENERATION ADDED BY PUSHKAR ON 9 SEPTEMBER 2017 FOR EDIT
                        WetosDB.LeaveMaster LeaveMasterTblObjEDIT = WetosDB.LeaveMasters.Where(a => (a.LeaveId == LeaveMasterObj.LeaveId || a.LeaveCode.ToUpper() == LeaveMasterObj.LeaveCode.ToUpper())
                           && a.Company.CompanyId == LeaveMasterObj.CompanyId && a.BranchId == LeaveMasterObj.BranchId && a.EmployeeGroup.EmployeeGroupId == EmpGrpId
                           && a.MarkedAsDelete == 0).FirstOrDefault();

                        // ADDED BY RAJAS ON 03 JUNE 2017 START
                        bool IsNew = false;

                        // IS OBJECT PRESET
                        if (LeaveMasterTblObj != null)  // Added by Rajas on 2 JUNE 2017
                        {
                            if (IsEdit == false)  // CREATE            
                            {
                                UpdateStatus = "Leave Master already available."; //WetosEmployeeController.GetErrorMessage(1);

                                //AddAuditTrail("Error in Department update : " + UpdateStatus);
                                //return ReturnStatus;
                                break;
                            }
                        }
                        else
                        {
                            if (IsEdit == false) // CREATE    
                            {
                                LeaveMasterTblObj = new WetosDB.LeaveMaster();
                                IsNew = true;
                            }
                            else // EDIT    
                            {
                                UpdateStatus = "Error in updating Leave Master."; // WetosEmployeeController.GetErrorMessage(1); 
                                //AddAuditTrail("Error in Department update : " + UpdateStatus);
                                //return ReturnStatus;
                                break;
                            }
                        }
                        // ADDED BY RAJAS ON 03 JUNE 2017 END

                        // New Leave table object
                        LeaveMasterTblObj.LeaveCode = LeaveMasterObj.LeaveCode;

                        LeaveMasterTblObj.LeaveName = LeaveMasterObj.LeaveName;

                        LeaveMasterTblObj.LeaveDescription = LeaveMasterObj.LeaveDescription;

                        LeaveMasterTblObj.NoOfDaysAllowedInYear = LeaveMasterObj.NoOfDaysAllowedInYear;

                        LeaveMasterTblObj.MaxNoOfTimesAllowedInYear = LeaveMasterObj.MaxNoOfTimesAllowedInYear;

                        LeaveMasterTblObj.MinNoofLeaveAllowedatatime = LeaveMasterObj.MinNoofLeaveAllowedatatime;

                        LeaveMasterTblObj.MaxNoOfLeavesAllowedAtaTime = LeaveMasterObj.MaxNoOfLeavesAllowedAtaTime; // Updated by Rajas on 11 MARCH 2017

                        LeaveMasterTblObj.MaxNoOfLeavesAllowedInMonth = LeaveMasterObj.MaxNoOfLeavesAllowedInMonth;

                        LeaveMasterTblObj.MaxAccumulationDays = LeaveMasterObj.MaxAccumulationDays;

                        LeaveMasterTblObj.NoOfDaysEncashed = LeaveMasterObj.NoOfDaysEncashed; // Added by Rajas on 30 MAY 2017

                        // Updated by Rajas on 1 MARCH 2017
                        LeaveMasterTblObj.EmployeeGroup = WetosDB.EmployeeGroups.Where(a => a.EmployeeGroupId == EmpGrpId).FirstOrDefault();   // EmployeeGroup.EmployeeGroupId

                        LeaveMasterTblObj.Company = WetosDB.Companies.Where(a => a.CompanyId == LeaveMasterObj.CompanyId).FirstOrDefault(); // Company.CompanyId

                        LeaveMasterTblObj.BranchId = LeaveMasterObj.BranchId;

                        LeaveMasterTblObj.LeaveType = LeaveMasterObj.LeaveType;

                        LeaveMasterTblObj.MinNoofLeaveAllowedatatime = LeaveMasterObj.MinNoofLeaveAllowedatatime;

                        LeaveMasterTblObj.IsLeaveCombination = LeaveMasterObj.IsLeaveCombination;

                        // Added this tow extra fields to mapp with Eviska database, by Rajas on 1 MARCH 2017

                        LeaveMasterTblObj.ApplicableToMaleFemale = LeaveMasterObj.ApplicableToMaleFemale;

                        LeaveMasterTblObj.LeaveCreditTypeId = LeaveMasterObj.LeaveCreditTypeId;

                        // Added by Rajas on 29 MARCH 2017
                        LeaveMasterTblObj.MarkedAsDelete = 0; // Default value

                        // Checkbox null value handling added by Rajas on 11 MARCH 2017 START

                        // AccumulationAllowedOrNot
                        if (LeaveMasterObj.AccumulationAllowedOrNot == null)
                        {
                            LeaveMasterTblObj.AccumulationAllowedOrNot = false;
                        }
                        else
                        {
                            LeaveMasterTblObj.AccumulationAllowedOrNot = LeaveMasterObj.AccumulationAllowedOrNot;
                        }

                        // EncashmentAllowedOrNot
                        if (LeaveMasterObj.AccumulationAllowedOrNot == null)
                        {
                            LeaveMasterTblObj.EncashmentAllowedOrNot = false;
                        }
                        else
                        {
                            LeaveMasterTblObj.EncashmentAllowedOrNot = LeaveMasterObj.EncashmentAllowedOrNot;
                        }

                        // NegativeBalanceAllowed
                        if (LeaveMasterObj.NegativeBalanceAllowed == null)
                        {
                            LeaveMasterTblObj.NegativeBalanceAllowed = false;
                        }
                        else
                        {
                            LeaveMasterTblObj.NegativeBalanceAllowed = LeaveMasterObj.NegativeBalanceAllowed;
                        }

                        // IsCarryForword
                        if (LeaveMasterObj.IsCarryForword == null)
                        {
                            LeaveMasterTblObj.IsCarryForword = false;
                        }
                        else
                        {
                            LeaveMasterTblObj.IsCarryForword = LeaveMasterObj.IsCarryForword;
                        }

                        // WOBetLevConsiderLeave
                        if (LeaveMasterObj.WOBetLevConsiderLeave == null)
                        {
                            LeaveMasterTblObj.WOBetLevConsiderLeave = false;
                        }
                        else
                        {
                            LeaveMasterTblObj.WOBetLevConsiderLeave = LeaveMasterObj.WOBetLevConsiderLeave;
                        }

                        // HHBetLevConsiderLeave
                        if (LeaveMasterObj.HHBetLevConsiderLeave == null)
                        {
                            LeaveMasterTblObj.HHBetLevConsiderLeave = false;
                        }
                        else
                        {
                            LeaveMasterTblObj.HHBetLevConsiderLeave = LeaveMasterObj.HHBetLevConsiderLeave;
                        }

                        // ISHalfAllowed
                        if (LeaveMasterObj.ISHalfAllowed == null)
                        {
                            LeaveMasterTblObj.ISHalfAllowed = false;
                        }
                        else
                        {
                            LeaveMasterTblObj.ISHalfAllowed = LeaveMasterObj.ISHalfAllowed;
                        }

                        // IsLeaveCombination
                        if (LeaveMasterObj.IsLeaveCombination == null)
                        {
                            LeaveMasterTblObj.IsLeaveCombination = false;
                        }
                        else
                        {
                            LeaveMasterTblObj.IsLeaveCombination = LeaveMasterObj.IsLeaveCombination;
                        }
                        // Checkbox null value handling added by Rajas on 11 MARCH 2017 END

                        //CODE ADDED BY SHRADDHA ON 16 FEB 2018 START
                        // IsAttachmentNeeded
                        if (LeaveMasterObj.IsAttachmentNeeded == null)
                        {
                            LeaveMasterTblObj.IsAttachmentNeeded = false;
                        }
                        else
                        {
                            LeaveMasterTblObj.IsAttachmentNeeded = LeaveMasterObj.IsAttachmentNeeded;
                        }
                        LeaveMasterTblObj.AttachmentRequiredForMinNoOfLeave = LeaveMasterObj.AttachmentRequiredForMinNoOfLeave;
                        //CODE ADDED BY SHRADDHA ON 16 FEB 2018 END


                        LeaveMasterTblObj.Prefix = fc["PrefixId"];//LeaveMasterObj.PrefixId;
                        LeaveMasterTblObj.Suffix = fc["SuffixId"];//LeaveMasterObj.SuffixId;

                        // Add new table object 
                        if (IsNew)
                        {
                            // Added by Rajas on 14 MARCH 2017 for Calculaig max of LeaveId and save START
                            LeaveMaster LastLeaveId = WetosDB.LeaveMasters.OrderByDescending(a => a.LeaveId).FirstOrDefault();

                            int MaxOfLeaveId = 0;

                            int Max = 0;
                            if (LastLeaveId != null)
                            {
                                Max = Convert.ToInt32(LastLeaveId.LeaveId);
                            }


                            MaxOfLeaveId = Max + 1;

                            LeaveMasterTblObj.LeaveId = MaxOfLeaveId;
                            // END

                            //ADDED BY PUSHKAR FOR PREFIX AND SUFFIX ON 10 MAY 2018



                            WetosDB.LeaveMasters.Add(LeaveMasterTblObj);
                        }

                        WetosDB.SaveChanges();


                        //-------------------------------------------AuditLog---------------------------------------------------------------------------

                        if (IsNew)
                        {
                            //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 11 SEPTEMBER 2017
                            #region ADD AUDIT LOG
                            //OLD RECORD IS BLANK
                            string Newrecord = "Leave Code : " + LeaveMasterTblObj.LeaveCode + ", LeaveName :"
                                + LeaveMasterTblObj.LeaveName + ", LeaveDescription :" + LeaveMasterTblObj.LeaveDescription + ", LeaveType :"
                                + LeaveMasterTblObj.LeaveType + ", Employee GroupID :" + LeaveMasterTblObj.EmployeeGroup.EmployeeGroupId + ",Accumulation Allowed or Not :"
                                + LeaveMasterTblObj.AccumulationAllowedOrNot + ", EncashmentAllowedOrNot :" + LeaveMasterTblObj.EncashmentAllowedOrNot
                                + ", HHBetLevConsiderLeave :" + LeaveMasterTblObj.HHBetLevConsiderLeave + ", IsCarryForword :" + LeaveMasterTblObj.IsCarryForword
                                + ", ISHalfAllowed :" + LeaveMasterTblObj.ISHalfAllowed + ", MaxNoOfLeavesAllowedAtaTime :"
                                + LeaveMasterTblObj.MaxNoOfLeavesAllowedAtaTime + ",MaxNoOfLeavesAllowedInMonth:"
                                + LeaveMasterTblObj.MaxNoOfLeavesAllowedInMonth + ", MaxNoOfTimesAllowedInYear :"
                                + LeaveMasterTblObj.NoOfDaysAllowedInYear + ", NoOfDaysAllowedInYear : " + LeaveMasterTblObj.NoOfDaysAllowedInYear
                                + ", WOBetLevConsiderLeave : " + LeaveMasterTblObj.WOBetLevConsiderLeave + ", NegativeBalanceAllowed : "
                                + LeaveMasterTblObj.NegativeBalanceAllowed + ", Branch ID : " + LeaveMasterTblObj.BranchId + ", Company  ID : "
                                + LeaveMasterTblObj.Company.CompanyId;


                            //LOGIN ID IS TAKEN FROM SESSION PERSISTER
                            string Formname = "RULE SETTING-LEAVE MASTER";
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
                            string Oldrecord = "Leave Code : " + LeaveMasterTblObjEDIT.LeaveCode + ", LeaveName :"
                                + LeaveMasterTblObjEDIT.LeaveName + ", LeaveDescription :" + LeaveMasterTblObjEDIT.LeaveDescription + ", LeaveType :"
                                + LeaveMasterTblObjEDIT.LeaveType + ", Employee GroupID :" + LeaveMasterTblObjEDIT.EmployeeGroup.EmployeeGroupId + ",Accumulation Allowed or Not :"
                                + LeaveMasterTblObjEDIT.AccumulationAllowedOrNot + ", EncashmentAllowedOrNot :" + LeaveMasterTblObjEDIT.EncashmentAllowedOrNot
                                + ", HHBetLevConsiderLeave :" + LeaveMasterTblObjEDIT.HHBetLevConsiderLeave + ", IsCarryForword :" + LeaveMasterTblObjEDIT.IsCarryForword
                                + ", ISHalfAllowed :" + LeaveMasterTblObjEDIT.ISHalfAllowed + ", MaxNoOfLeavesAllowedAtaTime :"
                                + LeaveMasterTblObjEDIT.MaxNoOfLeavesAllowedAtaTime + ",MaxNoOfLeavesAllowedInMonth:"
                                + LeaveMasterTblObjEDIT.MaxNoOfLeavesAllowedInMonth + ", MaxNoOfTimesAllowedInYear :"
                                + LeaveMasterTblObjEDIT.NoOfDaysAllowedInYear + ", NoOfDaysAllowedInYear : " + LeaveMasterTblObjEDIT.NoOfDaysAllowedInYear
                                + ", WOBetLevConsiderLeave : " + LeaveMasterTblObjEDIT.WOBetLevConsiderLeave + ", NegativeBalanceAllowed : "
                                + LeaveMasterTblObjEDIT.NegativeBalanceAllowed + ", Branch ID : " + LeaveMasterTblObjEDIT.BranchId + ", Company  ID : "
                                + LeaveMasterTblObjEDIT.Company.CompanyId;
                            //---x---------x-----------------x------------x----------------x-----------x-------------x------------x--------------x----------x---------x-----
                            string Newrecord = "Leave Code : " + LeaveMasterTblObj.LeaveCode + ", LeaveName :"
                                + LeaveMasterTblObj.LeaveName + ", LeaveDescription :" + LeaveMasterTblObj.LeaveDescription + ", LeaveType :"
                                + LeaveMasterTblObj.LeaveType + ", Employee GroupID :" + LeaveMasterTblObj.EmployeeGroup.EmployeeGroupId + ",Accumulation Allowed or Not :"
                                + LeaveMasterTblObj.AccumulationAllowedOrNot + ", EncashmentAllowedOrNot :" + LeaveMasterTblObj.EncashmentAllowedOrNot
                                + ", HHBetLevConsiderLeave :" + LeaveMasterTblObj.HHBetLevConsiderLeave + ", IsCarryForword :" + LeaveMasterTblObj.IsCarryForword
                                + ", ISHalfAllowed :" + LeaveMasterTblObj.ISHalfAllowed + ", MaxNoOfLeavesAllowedAtaTime :"
                                + LeaveMasterTblObj.MaxNoOfLeavesAllowedAtaTime + ",MaxNoOfLeavesAllowedInMonth:"
                                + LeaveMasterTblObj.MaxNoOfLeavesAllowedInMonth + ", MaxNoOfTimesAllowedInYear :"
                                + LeaveMasterTblObj.NoOfDaysAllowedInYear + ", NoOfDaysAllowedInYear : " + LeaveMasterTblObj.NoOfDaysAllowedInYear
                                + ", WOBetLevConsiderLeave : " + LeaveMasterTblObj.WOBetLevConsiderLeave + ", NegativeBalanceAllowed : "
                                + LeaveMasterTblObj.NegativeBalanceAllowed + ", Branch ID : " + LeaveMasterTblObj.BranchId + ", Company  ID : "
                                + LeaveMasterTblObj.Company.CompanyId;


                            //LOGIN ID IS TAKEN FROM SESSION PERSISTER
                            string Formname = "RULE SETTING-LEAVE MASTER";
                            //ACTION IS UPDATE
                            string Message = " ";

                            WetosAdministrationController.GenerateAuditLogsUpdate(WetosDB, 0, Formname, Oldrecord,
                                Newrecord, ref Message);
                            #endregion
                            //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 11 SEPTEMBER 2017
                        }

                        //-------------------------------------------AuditLog---------------------------------------------------------------------------

                    }
                    ReturnStatus = true;

                    return ReturnStatus;
                }

                #endregion

                #region SINGLE ENTRY FOR LEAVE
                // Updated by Rajas on 31 MAY 2017
                WetosDB.LeaveMaster LeaveMasterTblObjS = WetosDB.LeaveMasters.Where(a => (a.LeaveId == LeaveMasterObj.LeaveId || a.LeaveCode.ToUpper() == LeaveMasterObj.LeaveCode.ToUpper())
                    && a.Company.CompanyId == LeaveMasterObj.CompanyId && a.BranchId == LeaveMasterObj.BranchId && a.EmployeeGroup.EmployeeGroupId == LeaveMasterObj.EmployeeGrpId
                    && a.MarkedAsDelete == 0).FirstOrDefault();

                ////AUDIT LOG GENERATION ADDED BY PUSHKAR ON 9 SEPTEMBER 2017 FOR EDIT
                WetosDB.LeaveMaster LeaveMasterTblObjEDITS = WetosDB.LeaveMasters.Where(a => (a.LeaveId == LeaveMasterObj.LeaveId || a.LeaveCode.ToUpper() == LeaveMasterObj.LeaveCode.ToUpper())
                   && a.Company.CompanyId == LeaveMasterObj.CompanyId && a.BranchId == LeaveMasterObj.BranchId && a.EmployeeGroup.EmployeeGroupId == LeaveMasterObj.EmployeeGrpId
                   && a.MarkedAsDelete == 0).FirstOrDefault();

                // ADDED BY RAJAS ON 03 JUNE 2017 START
                bool IsNewS = false;

                // IS OBJECT PRESET
                if (LeaveMasterTblObjS != null)  // Added by Rajas on 2 JUNE 2017
                {
                    if (IsEdit == false)  // CREATE            
                    {
                        UpdateStatus = "Leave Master already available."; //WetosEmployeeController.GetErrorMessage(1);

                        //AddAuditTrail("Error in Department update : " + UpdateStatus);
                        return ReturnStatus;
                    }
                }
                else
                {
                    if (IsEdit == false) // CREATE    
                    {
                        LeaveMasterTblObjS = new WetosDB.LeaveMaster();
                        IsNewS = true;
                    }
                    else // EDIT    
                    {
                        UpdateStatus = "Error in updating Leave Master."; // WetosEmployeeController.GetErrorMessage(1); 
                        //AddAuditTrail("Error in Department update : " + UpdateStatus);
                        return ReturnStatus;
                    }
                }
                // ADDED BY RAJAS ON 03 JUNE 2017 END

                // New Leave table object
                LeaveMasterTblObjS.LeaveCode = LeaveMasterObj.LeaveCode;

                LeaveMasterTblObjS.LeaveName = LeaveMasterObj.LeaveName;

                LeaveMasterTblObjS.LeaveDescription = LeaveMasterObj.LeaveDescription;

                LeaveMasterTblObjS.NoOfDaysAllowedInYear = LeaveMasterObj.NoOfDaysAllowedInYear;

                LeaveMasterTblObjS.MaxNoOfTimesAllowedInYear = LeaveMasterObj.MaxNoOfTimesAllowedInYear;

                LeaveMasterTblObjS.MinNoofLeaveAllowedatatime = LeaveMasterObj.MinNoofLeaveAllowedatatime;

                LeaveMasterTblObjS.MaxNoOfLeavesAllowedAtaTime = LeaveMasterObj.MaxNoOfLeavesAllowedAtaTime; // Updated by Rajas on 11 MARCH 2017

                LeaveMasterTblObjS.MaxNoOfLeavesAllowedInMonth = LeaveMasterObj.MaxNoOfLeavesAllowedInMonth;

                LeaveMasterTblObjS.MaxAccumulationDays = LeaveMasterObj.MaxAccumulationDays;

                LeaveMasterTblObjS.NoOfDaysEncashed = LeaveMasterObj.NoOfDaysEncashed; // Added by Rajas on 30 MAY 2017

                // Updated by Rajas on 1 MARCH 2017
                LeaveMasterTblObjS.EmployeeGroup = WetosDB.EmployeeGroups.Where(a => a.EmployeeGroupId == LeaveMasterObj.EmployeeGrpId).FirstOrDefault();   // EmployeeGroup.EmployeeGroupId

                LeaveMasterTblObjS.Company = WetosDB.Companies.Where(a => a.CompanyId == LeaveMasterObj.CompanyId).FirstOrDefault(); // Company.CompanyId

                LeaveMasterTblObjS.BranchId = LeaveMasterObj.BranchId;

                LeaveMasterTblObjS.LeaveType = LeaveMasterObj.LeaveType;

                LeaveMasterTblObjS.MinNoofLeaveAllowedatatime = LeaveMasterObj.MinNoofLeaveAllowedatatime;

                LeaveMasterTblObjS.IsLeaveCombination = LeaveMasterObj.IsLeaveCombination;

                // Added this tow extra fields to mapp with Eviska database, by Rajas on 1 MARCH 2017

                LeaveMasterTblObjS.ApplicableToMaleFemale = LeaveMasterObj.ApplicableToMaleFemale;

                LeaveMasterTblObjS.LeaveCreditTypeId = LeaveMasterObj.LeaveCreditTypeId;                

                // Added by Rajas on 29 MARCH 2017
                LeaveMasterTblObjS.MarkedAsDelete = 0; // Default value

                // Checkbox null value handling added by Rajas on 11 MARCH 2017 START

                // AccumulationAllowedOrNot
                if (LeaveMasterObj.AccumulationAllowedOrNot == null)
                {
                    LeaveMasterTblObjS.AccumulationAllowedOrNot = false;
                }
                else
                {
                    LeaveMasterTblObjS.AccumulationAllowedOrNot = LeaveMasterObj.AccumulationAllowedOrNot;
                }

                // EncashmentAllowedOrNot
                if (LeaveMasterObj.AccumulationAllowedOrNot == null)
                {
                    LeaveMasterTblObjS.EncashmentAllowedOrNot = false;
                }
                else
                {
                    LeaveMasterTblObjS.EncashmentAllowedOrNot = LeaveMasterObj.EncashmentAllowedOrNot;
                }

                // NegativeBalanceAllowed
                if (LeaveMasterObj.NegativeBalanceAllowed == null)
                {
                    LeaveMasterTblObjS.NegativeBalanceAllowed = false;
                }
                else
                {
                    LeaveMasterTblObjS.NegativeBalanceAllowed = LeaveMasterObj.NegativeBalanceAllowed;
                }

                // IsCarryForword
                if (LeaveMasterObj.IsCarryForword == null)
                {
                    LeaveMasterTblObjS.IsCarryForword = false;
                }
                else
                {
                    LeaveMasterTblObjS.IsCarryForword = LeaveMasterObj.IsCarryForword;
                }

                // WOBetLevConsiderLeave
                if (LeaveMasterObj.WOBetLevConsiderLeave == null)
                {
                    LeaveMasterTblObjS.WOBetLevConsiderLeave = false;
                }
                else
                {
                    LeaveMasterTblObjS.WOBetLevConsiderLeave = LeaveMasterObj.WOBetLevConsiderLeave;
                }

                // HHBetLevConsiderLeave
                if (LeaveMasterObj.HHBetLevConsiderLeave == null)
                {
                    LeaveMasterTblObjS.HHBetLevConsiderLeave = false;
                }
                else
                {
                    LeaveMasterTblObjS.HHBetLevConsiderLeave = LeaveMasterObj.HHBetLevConsiderLeave;
                }

                // ISHalfAllowed
                if (LeaveMasterObj.ISHalfAllowed == null)
                {
                    LeaveMasterTblObjS.ISHalfAllowed = false;
                }
                else
                {
                    LeaveMasterTblObjS.ISHalfAllowed = LeaveMasterObj.ISHalfAllowed;
                }

                // IsLeaveCombination
                if (LeaveMasterObj.IsLeaveCombination == null)
                {
                    LeaveMasterTblObjS.IsLeaveCombination = false;
                }
                else
                {
                    LeaveMasterTblObjS.IsLeaveCombination = LeaveMasterObj.IsLeaveCombination;
                }
                // Checkbox null value handling added by Rajas on 11 MARCH 2017 END

                //CODE ADDED BY SHRADDHA ON 16 FEB 2018 START
                // IsAttachmentNeeded
                if (LeaveMasterObj.IsAttachmentNeeded == null)
                {
                    LeaveMasterTblObjS.IsAttachmentNeeded = false;
                }
                else
                {
                    LeaveMasterTblObjS.IsAttachmentNeeded = LeaveMasterObj.IsAttachmentNeeded;
                }
                LeaveMasterTblObjS.AttachmentRequiredForMinNoOfLeave = LeaveMasterObj.AttachmentRequiredForMinNoOfLeave;
                //CODE ADDED BY SHRADDHA ON 16 FEB 2018 END


                LeaveMasterTblObjS.Prefix = fc["PrefixId"];//LeaveMasterObj.PrefixId;
                LeaveMasterTblObjS.Suffix = fc["SuffixId"];//LeaveMasterObj.SuffixId;

                // Add new table object 
                if (IsNewS)
                {
                    // Added by Rajas on 14 MARCH 2017 for Calculaig max of LeaveId and save START
                    LeaveMaster LastLeaveId = WetosDB.LeaveMasters.OrderByDescending(a => a.LeaveId).FirstOrDefault();

                    int MaxOfLeaveId = 0;

                    int Max = 0;
                    if (LastLeaveId != null)
                    {
                        Max = Convert.ToInt32(LastLeaveId.LeaveId);
                    }


                    MaxOfLeaveId = Max + 1;

                    LeaveMasterTblObjS.LeaveId = MaxOfLeaveId;
                    // END

                    //ADDED BY PUSHKAR FOR PREFIX AND SUFFIX ON 10 MAY 2018



                    WetosDB.LeaveMasters.Add(LeaveMasterTblObjS);
                }

                WetosDB.SaveChanges();


                //-------------------------------------------AuditLog---------------------------------------------------------------------------

                if (IsNewS)
                {
                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 11 SEPTEMBER 2017
                    #region ADD AUDIT LOG
                    //OLD RECORD IS BLANK
                    string Newrecord = "Leave Code : " + LeaveMasterTblObjS.LeaveCode + ", LeaveName :"
                        + LeaveMasterTblObjS.LeaveName + ", LeaveDescription :" + LeaveMasterTblObjS.LeaveDescription + ", LeaveType :"
                        + LeaveMasterTblObjS.LeaveType + ", Employee GroupID :" + LeaveMasterTblObjS.EmployeeGroup.EmployeeGroupId + ",Accumulation Allowed or Not :"
                        + LeaveMasterTblObjS.AccumulationAllowedOrNot + ", EncashmentAllowedOrNot :" + LeaveMasterTblObjS.EncashmentAllowedOrNot
                        + ", HHBetLevConsiderLeave :" + LeaveMasterTblObjS.HHBetLevConsiderLeave + ", IsCarryForword :" + LeaveMasterTblObjS.IsCarryForword
                        + ", ISHalfAllowed :" + LeaveMasterTblObjS.ISHalfAllowed + ", MaxNoOfLeavesAllowedAtaTime :"
                        + LeaveMasterTblObjS.MaxNoOfLeavesAllowedAtaTime + ",MaxNoOfLeavesAllowedInMonth:"
                        + LeaveMasterTblObjS.MaxNoOfLeavesAllowedInMonth + ", MaxNoOfTimesAllowedInYear :"
                        + LeaveMasterTblObjS.NoOfDaysAllowedInYear + ", NoOfDaysAllowedInYear : " + LeaveMasterTblObjS.NoOfDaysAllowedInYear
                        + ", WOBetLevConsiderLeave : " + LeaveMasterTblObjS.WOBetLevConsiderLeave + ", NegativeBalanceAllowed : "
                        + LeaveMasterTblObjS.NegativeBalanceAllowed + ", Branch ID : " + LeaveMasterTblObjS.BranchId + ", Company  ID : "
                        + LeaveMasterTblObjS.Company.CompanyId;


                    //LOGIN ID IS TAKEN FROM SESSION PERSISTER
                    string Formname = "RULE SETTING-LEAVE MASTER";
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
                    string Oldrecord = "Leave Code : " + LeaveMasterTblObjEDITS.LeaveCode + ", LeaveName :"
                        + LeaveMasterTblObjEDITS.LeaveName + ", LeaveDescription :" + LeaveMasterTblObjEDITS.LeaveDescription + ", LeaveType :"
                        + LeaveMasterTblObjEDITS.LeaveType + ", Employee GroupID :" + LeaveMasterTblObjEDITS.EmployeeGroup.EmployeeGroupId + ",Accumulation Allowed or Not :"
                        + LeaveMasterTblObjEDITS.AccumulationAllowedOrNot + ", EncashmentAllowedOrNot :" + LeaveMasterTblObjEDITS.EncashmentAllowedOrNot
                        + ", HHBetLevConsiderLeave :" + LeaveMasterTblObjEDITS.HHBetLevConsiderLeave + ", IsCarryForword :" + LeaveMasterTblObjEDITS.IsCarryForword
                        + ", ISHalfAllowed :" + LeaveMasterTblObjEDITS.ISHalfAllowed + ", MaxNoOfLeavesAllowedAtaTime :"
                        + LeaveMasterTblObjEDITS.MaxNoOfLeavesAllowedAtaTime + ",MaxNoOfLeavesAllowedInMonth:"
                        + LeaveMasterTblObjEDITS.MaxNoOfLeavesAllowedInMonth + ", MaxNoOfTimesAllowedInYear :"
                        + LeaveMasterTblObjEDITS.NoOfDaysAllowedInYear + ", NoOfDaysAllowedInYear : " + LeaveMasterTblObjEDITS.NoOfDaysAllowedInYear
                        + ", WOBetLevConsiderLeave : " + LeaveMasterTblObjEDITS.WOBetLevConsiderLeave + ", NegativeBalanceAllowed : "
                        + LeaveMasterTblObjEDITS.NegativeBalanceAllowed + ", Branch ID : " + LeaveMasterTblObjEDITS.BranchId + ", Company  ID : "
                        + LeaveMasterTblObjEDITS.Company.CompanyId;
                    //---x---------x-----------------x------------x----------------x-----------x-------------x------------x--------------x----------x---------x-----
                    string Newrecord = "Leave Code : " + LeaveMasterTblObjS.LeaveCode + ", LeaveName :"
                        + LeaveMasterTblObjS.LeaveName + ", LeaveDescription :" + LeaveMasterTblObjS.LeaveDescription + ", LeaveType :"
                        + LeaveMasterTblObjS.LeaveType + ", Employee GroupID :" + LeaveMasterTblObjS.EmployeeGroup.EmployeeGroupId + ",Accumulation Allowed or Not :"
                        + LeaveMasterTblObjS.AccumulationAllowedOrNot + ", EncashmentAllowedOrNot :" + LeaveMasterTblObjS.EncashmentAllowedOrNot
                        + ", HHBetLevConsiderLeave :" + LeaveMasterTblObjS.HHBetLevConsiderLeave + ", IsCarryForword :" + LeaveMasterTblObjS.IsCarryForword
                        + ", ISHalfAllowed :" + LeaveMasterTblObjS.ISHalfAllowed + ", MaxNoOfLeavesAllowedAtaTime :"
                        + LeaveMasterTblObjS.MaxNoOfLeavesAllowedAtaTime + ",MaxNoOfLeavesAllowedInMonth:"
                        + LeaveMasterTblObjS.MaxNoOfLeavesAllowedInMonth + ", MaxNoOfTimesAllowedInYear :"
                        + LeaveMasterTblObjS.NoOfDaysAllowedInYear + ", NoOfDaysAllowedInYear : " + LeaveMasterTblObjS.NoOfDaysAllowedInYear
                        + ", WOBetLevConsiderLeave : " + LeaveMasterTblObjS.WOBetLevConsiderLeave + ", NegativeBalanceAllowed : "
                        + LeaveMasterTblObjS.NegativeBalanceAllowed + ", Branch ID : " + LeaveMasterTblObjS.BranchId + ", Company  ID : "
                        + LeaveMasterTblObjS.Company.CompanyId;


                    //LOGIN ID IS TAKEN FROM SESSION PERSISTER
                    string Formname = "RULE SETTING-LEAVE MASTER";
                    //ACTION IS UPDATE
                    string Message = " ";

                    WetosAdministrationController.GenerateAuditLogsUpdate(WetosDB, 0, Formname, Oldrecord,
                        Newrecord, ref Message);
                    #endregion
                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 11 SEPTEMBER 2017
                }

                //-------------------------------------------AuditLog---------------------------------------------------------------------------



                ReturnStatus = true;

                return ReturnStatus;

                #endregion
            }

            catch (System.Exception ex)
            {
                UpdateStatus = "Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message);

                return ReturnStatus;
            }
        }

    }
}
