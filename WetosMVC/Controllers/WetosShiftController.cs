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
    public class WetosShiftController : BaseController
    {

        
        //
        // GET: /WetosShift/

        public ActionResult Index()
        {
            try
            {
                // Updated by Rajas on 29 MARCh 2017
                List<WetosDB.VwShiftDetail> ShiftDetails = WetosDB.VwShiftDetails.Where(a => a.MarkedAsDelete == 0 || a.MarkedAsDelete == null).ToList();

                //ADDED BY RAJAS ON 27 DEC 2016
                AddAuditTrail("Success - Visited Shift master"); // Updated by Rajas on 16 JAN 2017

                return View(ShiftDetails);
            }
            catch (System.Exception ex)
            {
                // Added by Rajas on 16 JAN 2017
                AddAuditTrail("Exception - " + ex.Message);

                return View("Error");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int id)
        {
            try
            {
                ShiftModel ShiftModelObj = new ShiftModel();

                WetosDB.Shift ShiftEdit = WetosDB.Shifts.Where(b => b.ShiftId == id).FirstOrDefault();

                ShiftModelObj.BranchId = ShiftEdit.BranchId;
                ShiftModelObj.ShiftCode = ShiftEdit.ShiftCode;
                ShiftModelObj.ShiftId = ShiftEdit.ShiftId;
                ShiftModelObj.CompanyId = Convert.ToInt32(ShiftEdit.Company.CompanyId);

                ShiftModelObj.FirstInTime = Convert.ToString(ShiftEdit.FirstInTime.TimeOfDay);
                ShiftModelObj.FirstOutTime = Convert.ToString(ShiftEdit.FirstOutTime.TimeOfDay);
                ShiftModelObj.SecondInTime = Convert.ToString(ShiftEdit.SecondInTime.Value.TimeOfDay);
                ShiftModelObj.SecondOutTime = Convert.ToString(ShiftEdit.SecondOutTime.Value.TimeOfDay);
                ShiftModelObj.LunchStartTime = Convert.ToString(ShiftEdit.LunchStartTime.Value.TimeOfDay);
                ShiftModelObj.LunchEndTime = Convert.ToString(ShiftEdit.LunchEndTime.Value.TimeOfDay);
                ShiftModelObj.LunchTime = Convert.ToString(ShiftEdit.LunchTime.Value.TimeOfDay);
                ShiftModelObj.WorkHours = Convert.ToString(ShiftEdit.WorkingHours.TimeOfDay);


                //ShiftModelObj.FirstInTimeHH = ShiftEdit.FirstInTime.Hour.ToString("D2");
                //ShiftModelObj.FirstInTimeMM = ShiftEdit.FirstInTime.Minute.ToString("D2");
                //ShiftModelObj.FirstOutHH = ShiftEdit.FirstOutTime.Hour.ToString("D2");
                //ShiftModelObj.FirstOutMM = ShiftEdit.FirstOutTime.Minute.ToString("D2");
                //ShiftModelObj.LunchEndHH = ShiftEdit.LunchEndTime.Value.Hour.ToString("D2");
                //ShiftModelObj.LunchEndMM = ShiftEdit.LunchEndTime.Value.Minute.ToString("D2");
                //ShiftModelObj.LunchStartHH = ShiftEdit.LunchStartTime.Value.Hour.ToString("D2");
                //ShiftModelObj.LunchStartMM = ShiftEdit.LunchStartTime.Value.Minute.ToString("D2");
                //if (ShiftEdit.LunchTime != null)
                //{
                //    ShiftModelObj.LunchTimeMM = ShiftEdit.LunchTime.Value.Minute.ToString("D2");
                //    ShiftModelObj.LunchTimeHH = ShiftEdit.LunchTime.Value.Hour.ToString("D2");
                //}
                //else
                //{
                //    ShiftModelObj.LunchTimeMM = "00";
                //    ShiftModelObj.LunchTimeHH = "00";
                //}

                //if (ShiftEdit.LunchTime != null)
                //{
                //    ShiftModelObj.LunchTimeMM = ShiftEdit.LunchTime.Value.Minute.ToString("D2");
                //    ShiftModelObj.LunchTimeHH = ShiftEdit.LunchTime.Value.Hour.ToString("D2");
                //}
                ShiftModelObj.LunchTimeExcludeFlag = Convert.ToBoolean(ShiftEdit.LunchTimeExcludeFlag);
                ShiftModelObj.NightShiftFlag = Convert.ToBoolean(ShiftEdit.NightShiftFlag);
                //ShiftModelObj.SecondInHH = ShiftEdit.SecondInTime.Value.Hour.ToString("D2");
                //ShiftModelObj.SecondInMM = ShiftEdit.SecondInTime.Value.Minute.ToString("D2");
                //ShiftModelObj.SecondOutHH = ShiftEdit.SecondOutTime.Value.Hour.ToString("D2");
                //ShiftModelObj.SecondOutMM = ShiftEdit.SecondOutTime.Value.Minute.ToString("D2");
                ShiftModelObj.ShiftName = ShiftEdit.ShiftName;
                ShiftModelObj.ShiftType = ShiftEdit.ShiftType;
                ShiftModelObj.ShiftCode = ShiftEdit.ShiftCode;
                //ShiftModelObj.WorkHoursHH = ShiftEdit.WorkingHours.Hour.ToString("D2");
                //ShiftModelObj.WorkHoursMM = ShiftEdit.WorkingHours.Minute.ToString("D2");
                //ShiftModelObj.WorkingHours = ShiftEdit.WorkingHours;

                // Added by Rajas on 29 MARCH 2017
                ShiftModelObj.MarkedAsDelete = ShiftEdit.MarkedAsDelete;  // MarkedAsDelete

                //// MINUTE LIST
                //List<SelectListItem> MinSelList = new List<SelectListItem>();
                //for (int i = 0; i < 60; i++)
                //{
                //    MinSelList.Add(new SelectListItem { Text = i.ToString("D2"), Value = i.ToString("D2") });
                //}
                //ViewBag.MinListVB = new SelectList(MinSelList, "Value", "Text").ToList();

                //// HOUR LIST
                //List<SelectListItem> HourSelList = new List<SelectListItem>();
                //for (int i = 0; i < 24; i++)
                //{
                //    HourSelList.Add(new SelectListItem { Text = i.ToString("D2"), Value = i.ToString("D2") });
                //}
                //ViewBag.HourListVB = new SelectList(HourSelList, "Value", "Text").ToList();

                // Added by Rajas on 17 JAN 2017
                PopulateDropDownEdit(ShiftModelObj);

                return View(ShiftModelObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Incorrect data. Please try again!");

                return RedirectToAction("Index"); // Verify ?
            }
        }

        //
        // GET: /WetosShift/Create
        /// <summary>
        /// Updated by Rajas on 6 JUNE 2017
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            ShiftModel ShiftModelObj = new ShiftModel();

            try
            {
                PopulateDropDown();

                return View(ShiftModelObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Please try again!");

                return View(ShiftModelObj); // Verify ?
            }
        }

        //
        // POST: /WetosShift/Create

        /// <summary>
        /// For create new shift
        /// Updated on 17 Nov 2016 by Rajas
        /// Latest updated by Rajas on 30 DEC 2016 for validation of data
        /// </summary>
        /// <param name="NewShift"></param>
        /// <param name="collection"></param>
        /// <param name="LunchTimeExcludeFlag"></param>
        /// <param name="NightShiftFlag"></param>
        /// <returns></returns>
        /// Updated by Rajas on 27 MARCH 2017
        [HttpPost]
        public ActionResult Create(ShiftModel ShiftModelObj, FormCollection collection, bool LunchTimeExcludeFlag = false, bool NightShiftFlag = false)
        {
            try
            {
                string LunchTimeInHours = collection["LunchTimeInHours"];
                string LunchTimeInMinutes = collection["LunchTimeInMinutes"];
                string WorkingTimeInHours = collection["WorkingTimeInHours"];
                string WorkingTimeInMinutes = collection["WorkingTimeInMinutes"];
                //ShiftModelObj.WorkHoursMM = WorkingTimeInMinutes;
                //ShiftModelObj.WorkHoursHH = WorkingTimeInHours;
                //ShiftModelObj.LunchTimeHH = LunchTimeInHours;
                //ShiftModelObj.LunchTimeMM = LunchTimeInMinutes;

                if (ModelState.IsValid)
                {
                    // Addded by Rajas on 27 MARCH 2017
                    string UpdateStatus = string.Empty;

                    // Added by Rajas on 15 MAY 2017
                    bool IsEdit = false;

                    // Updated by Rajas on 15 MAY 2017
                    if (UpdateShiftData(ShiftModelObj, IsEdit, ref UpdateStatus) == true)
                    {
                        // ADDED BY RAJAS FOR AuditLog ON 27 DEC 2016
                        AddAuditTrail("Success - Shift : " + ShiftModelObj.ShiftName + " is added Successfully"); // Updated by Rajas on 16 JAN 2017

                        //ADDED CODE FOR SUCCESS MESSAGE BY RAJAS ON 16 JAN 2017
                        Success("Success - Shift : " + ShiftModelObj.ShiftName + " is added Successfully"); // Updated by Rajas on 16 JAN 2017
                    }
                    else
                    {
                        return ReportError(ShiftModelObj, UpdateStatus);
                    }

                }

                else
                {
                    var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();

                    PopulateDropDownEdit(ShiftModelObj);

                    return View(ShiftModelObj);
                }

                // CODE in below region commented by Rajas on 30 DEC 2016 
                #region POST DATA FROM FORM COLLECTION
                //var FirstInTimeHH = collection["FirstInTimeHH"];
                //var FirstInTimeMM = collection["FirstInTimeMM"];
                //NewShift.FirstInTime = new DateTime(2016, 01, 01, Convert.ToInt32(FirstInTimeHH), Convert.ToInt32(FirstInTimeMM), 0);

                //var FirstOutHH = collection["FirstOutHH"];
                //var FirstOutMM = collection["FirstOutMM"];
                //NewShift.FirstOutTime = new DateTime(2016, 01, 01, Convert.ToInt32(FirstOutHH), Convert.ToInt32(FirstOutMM), 0);

                //var SecondInHH = collection["SecondInHH"];
                //var SecondInMM = collection["SecondInMM"];
                //NewShift.SecondInTime = new DateTime(2016, 01, 01, Convert.ToInt32(SecondInHH), Convert.ToInt32(SecondInHH), 0);

                //var SecondOutHH = collection["SecondOutHH"];
                //var SecondOutMM = collection["SecondOutMM"];
                //NewShift.SecondOutTime = new DateTime(2016, 01, 01, Convert.ToInt32(SecondInMM), Convert.ToInt32(SecondInMM), 0);

                //var LunchStartHH = collection["LunchStartHH"];
                //var LunchStartMM = collection["LunchStartMM"];
                //NewShift.LunchStartTime = new DateTime(2016, 01, 01, Convert.ToInt32(LunchStartHH), Convert.ToInt32(LunchStartMM), 0);

                //var LunchEndHH = collection["LunchEndHH"];
                //var LunchEndMM = collection["LunchEndMM"];
                //NewShift.LunchEndTime = new DateTime(2016, 01, 01, Convert.ToInt32(LunchEndHH), Convert.ToInt32(LunchEndMM), 0);

                //var WorkHoursHH = collection["WorkHoursHH"];
                //var WorkHoursMM = collection["WorkHoursMM"];
                //NewShift.WorkingHours = new DateTime(2016, 01, 01, Convert.ToInt32(WorkHoursHH), Convert.ToInt32(WorkHoursMM), 0);

                //WetosDB.Shifts.Add(NewShift);

                //WetosDB.SaveChanges();

                #endregion

                return RedirectToAction("Index");
            }
            catch (System.Exception ex)
            {
                PopulateDropDownEdit(ShiftModelObj);

                // Added by Rajas on 16 JAN 2017 START
                AddAuditTrail("Exception - Shift : " + ShiftModelObj.ShiftName + " create failed due to " + ex.Message);

                Error("Exception - Shift : " + ShiftModelObj.ShiftName + " create failed due to " + ex.Message);
                // Added by Rajas on 16 JAN 2017 END

                return View(ShiftModelObj);
            }
        }


        /// <summary>
        /// Updated by Rajas on 27 MARCH 2017
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            try
            {
                ShiftModel ShiftModelObj = new ShiftModel();

                WetosDB.Shift ShiftEdit = WetosDB.Shifts.Where(b => b.ShiftId == id).FirstOrDefault();

                ShiftModelObj.BranchId = ShiftEdit.BranchId;
                ShiftModelObj.ShiftCode = ShiftEdit.ShiftCode;
                ShiftModelObj.ShiftId = ShiftEdit.ShiftId;
                ShiftModelObj.CompanyId = Convert.ToInt32(ShiftEdit.Company.CompanyId);

                ShiftModelObj.FirstInTime = Convert.ToString(ShiftEdit.FirstInTime.TimeOfDay);
                ShiftModelObj.FirstOutTime = Convert.ToString(ShiftEdit.FirstOutTime.TimeOfDay);
                ShiftModelObj.SecondInTime = Convert.ToString(ShiftEdit.SecondInTime.Value.TimeOfDay);
                ShiftModelObj.SecondOutTime = Convert.ToString(ShiftEdit.SecondOutTime.Value.TimeOfDay);
                ShiftModelObj.LunchStartTime = Convert.ToString(ShiftEdit.LunchStartTime.Value.TimeOfDay);
                ShiftModelObj.LunchEndTime = Convert.ToString(ShiftEdit.LunchEndTime.Value.TimeOfDay);
                ShiftModelObj.LunchTime = Convert.ToString(ShiftEdit.LunchTime.Value.TimeOfDay);
                ShiftModelObj.WorkHours = Convert.ToString(ShiftEdit.WorkingHours.TimeOfDay);

                ShiftModelObj.IsOutPunchInNextDay = ShiftEdit.IsOutPunchInNextDay; //CODE ADDED BY SHRADDHA ON 15 JAN 2018 FOR IsOutPunchInNextDay

                //ShiftModelObj.FirstInTimeHH = ShiftEdit.FirstInTime.Hour.ToString("D2");
                //ShiftModelObj.FirstInTimeMM = ShiftEdit.FirstInTime.Minute.ToString("D2");
                //ShiftModelObj.FirstOutHH = ShiftEdit.FirstOutTime.Hour.ToString("D2");
                //ShiftModelObj.FirstOutMM = ShiftEdit.FirstOutTime.Minute.ToString("D2");
                //ShiftModelObj.LunchEndHH = ShiftEdit.LunchEndTime.Value.Hour.ToString("D2");
                //ShiftModelObj.LunchEndMM = ShiftEdit.LunchEndTime.Value.Minute.ToString("D2");
                //ShiftModelObj.LunchStartHH = ShiftEdit.LunchStartTime.Value.Hour.ToString("D2");
                //ShiftModelObj.LunchStartMM = ShiftEdit.LunchStartTime.Value.Minute.ToString("D2");
                //if (ShiftEdit.LunchTime != null)
                //{
                //    ShiftModelObj.LunchTimeMM = ShiftEdit.LunchTime.Value.Minute.ToString("D2");
                //    ShiftModelObj.LunchTimeHH = ShiftEdit.LunchTime.Value.Hour.ToString("D2");
                //}
                //else
                //{
                //    ShiftModelObj.LunchTimeMM = "00";
                //    ShiftModelObj.LunchTimeHH = "00";
                //}

                //if (ShiftEdit.LunchTime != null)
                //{
                //    ShiftModelObj.LunchTimeMM = ShiftEdit.LunchTime.Value.Minute.ToString("D2");
                //    ShiftModelObj.LunchTimeHH = ShiftEdit.LunchTime.Value.Hour.ToString("D2");
                //}
                ShiftModelObj.LunchTimeExcludeFlag = Convert.ToBoolean(ShiftEdit.LunchTimeExcludeFlag);
                ShiftModelObj.NightShiftFlag = Convert.ToBoolean(ShiftEdit.NightShiftFlag);
                //ShiftModelObj.SecondInHH = ShiftEdit.SecondInTime.Value.Hour.ToString("D2");
                //ShiftModelObj.SecondInMM = ShiftEdit.SecondInTime.Value.Minute.ToString("D2");
                //ShiftModelObj.SecondOutHH = ShiftEdit.SecondOutTime.Value.Hour.ToString("D2");
                //ShiftModelObj.SecondOutMM = ShiftEdit.SecondOutTime.Value.Minute.ToString("D2");
                ShiftModelObj.ShiftName = ShiftEdit.ShiftName;
                ShiftModelObj.ShiftType = ShiftEdit.ShiftType;
                ShiftModelObj.ShiftCode = ShiftEdit.ShiftCode;
                //ShiftModelObj.WorkHoursHH = ShiftEdit.WorkingHours.Hour.ToString("D2");
                //ShiftModelObj.WorkHoursMM = ShiftEdit.WorkingHours.Minute.ToString("D2");
                //ShiftModelObj.WorkingHours = ShiftEdit.WorkingHours;

                // Added by Rajas on 29 MARCH 2017
                ShiftModelObj.MarkedAsDelete = ShiftEdit.MarkedAsDelete;  // MarkedAsDelete

                //// MINUTE LIST
                //List<SelectListItem> MinSelList = new List<SelectListItem>();
                //for (int i = 0; i < 60; i++)
                //{
                //    MinSelList.Add(new SelectListItem { Text = i.ToString("D2"), Value = i.ToString("D2") });
                //}
                //ViewBag.MinListVB = new SelectList(MinSelList, "Value", "Text").ToList();

                //// HOUR LIST
                //List<SelectListItem> HourSelList = new List<SelectListItem>();
                //for (int i = 0; i < 24; i++)
                //{
                //    HourSelList.Add(new SelectListItem { Text = i.ToString("D2"), Value = i.ToString("D2") });
                //}
                //ViewBag.HourListVB = new SelectList(HourSelList, "Value", "Text").ToList();

                // Added by Rajas on 17 JAN 2017
                PopulateDropDownEdit(ShiftModelObj);

                return View(ShiftModelObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Incorrect data. Please try again!");

                return RedirectToAction("Index"); // Verify ?
            }
        }

        //
        // POST: /WetosShift/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, ShiftModel ShiftModelObj, FormCollection FC, bool LunchTimeExcludeFlag = false, bool NightShiftFlag = false)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string LunchTimeInHours = FC["LunchTimeInHours"];
                    string LunchTimeInMinutes = FC["LunchTimeInMinutes"];
                    string WorkingTimeInHours = FC["WorkingTimeInHours"];
                    string WorkingTimeInMinutes = FC["WorkingTimeInMinutes"];

                    string NightShift = FC["NightShiftFlag"];
                    //ShiftModelObj.WorkHoursMM = WorkingTimeInMinutes;
                    //ShiftModelObj.WorkHoursHH = WorkingTimeInHours;
                    //ShiftModelObj.LunchTimeHH = LunchTimeInHours;
                    //ShiftModelObj.LunchTimeMM = LunchTimeInMinutes;

                    // Addded by Rajas on 27 MARCH 2017
                    string UpdateStatus = string.Empty;

                    // Added by Rajas on 15 MAY 2017
                    bool IsEdit = true;

                    // Updated by Rajas on 15 MAY 2017
                    if (UpdateShiftData(ShiftModelObj, IsEdit, ref UpdateStatus) == true)
                    {
                        // ADDED BY RAJAS FOR AuditLog ON 27 DEC 2016
                        AddAuditTrail("Success - Shift : " + ShiftModelObj.ShiftName + " is updated Successfully"); // Updated by Rajas on 16 JAN 2017

                        //ADDED CODE FOR SUCCESS MESSAGE BY RAJAS ON 16 JAN 2017
                        Success("Success - Shift : " + ShiftModelObj.ShiftName + " is updated Successfully"); // Updated by Rajas on 16 JAN 2017
                    }
                    else
                    {
                        return ReportError(ShiftModelObj, UpdateStatus);
                    }

                }

                else
                {
                    PopulateDropDownEdit(ShiftModelObj);

                    return View(ShiftModelObj);
                }

                // Code in below region commented by Rajas on 30 DEC 2016
                #region POST SHIFT DETAILS
                //WetosDB.Shift ShiftObj = WetosDB.Shifts.Single(e => e.ShiftId == id);

                //ShiftObj.CompanyId = ShiftEdit.CompanyId;
                //ShiftObj.BranchId = ShiftEdit.BranchId;
                //ShiftObj.ShiftName = ShiftEdit.ShiftName;
                //ShiftObj.ShiftType = ShiftEdit.ShiftType;

                //ShiftObj.FirstInTime = Convert.ToDateTime(ShiftEdit.FirstInTimeHH + ":" + ShiftEdit.FirstInTimeMM);
                //ShiftObj.FirstOutTime = Convert.ToDateTime(ShiftEdit.FirstOutHH + ":" + ShiftEdit.FirstOutMM);
                //ShiftObj.SecondInTime = Convert.ToDateTime(ShiftEdit.SecondInHH + ":" + ShiftEdit.SecondInMM);
                //ShiftObj.SecondOutTime = Convert.ToDateTime(ShiftEdit.SecondOutHH + ":" + ShiftEdit.SecondOutMM);
                //ShiftObj.LunchStartTime = Convert.ToDateTime(ShiftEdit.LunchStartHH + ":" + ShiftEdit.LunchStartMM);
                //ShiftObj.LunchEndTime = Convert.ToDateTime(ShiftEdit.LunchEndHH + ":" + ShiftEdit.LunchEndMM);

                //ShiftObj.WorkingHours = Convert.ToDateTime(ShiftEdit.WorkHoursHH + ":" + ShiftEdit.WorkHoursMM);


                ////ShiftObj.FirstInTime = ShiftEdit.FirstInTime;
                ////ShiftObj.FirstOutTime = Convert.ToDateTime(ShiftEdit.FirstOutTime);
                ////ShiftObj.SecondInTime = ShiftEdit.SecondInTime;
                ////ShiftObj.SecondOutTime = ShiftEdit.SecondOutTime;
                ////ShiftObj.LunchStartTime = ShiftEdit.LunchStartTime;
                ////ShiftObj.LunchEndTime = ShiftEdit.LunchEndTime;
                //ShiftObj.LunchTime = ShiftEdit.LunchTime;
                //ShiftObj.LunchTimeExcludeFlag = ShiftEdit.LunchTimeExcludeFlag;
                //ShiftObj.NightShiftFlag = ShiftEdit.NightShiftFlag;


                //// TODO: Add update logic here

                //WetosDB.SaveChanges();

                //AddAuditTrail("Shift details updated");
                #endregion

                return RedirectToAction("Index");
            }
            catch (System.Exception ex)
            {
                PopulateDropDownEdit(ShiftModelObj);

                // Added by Rajas on 16 JAN 2017 START
                AddAuditTrail("Exception - Shift : " + ShiftModelObj.ShiftName + " update failed due to " + ex.Message);

                Error("Exception - Shift : " + ShiftModelObj.ShiftName + " update failed due to " + ex.Message);
                // Added by Rajas on 16 JAN 2017 END

                return View(ShiftModelObj);
            }
        }


        /// <summary>
        /// Added by Rajas on 29 MARCH 2017
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// Updated by Rajas on 30 MAY 2017
        public ActionResult Delete(int id)
        {
            try
            {
                ShiftModel ShiftModelObj = new ShiftModel();

                WetosDB.Shift ShiftEdit = WetosDB.Shifts.Where(b => b.ShiftId == id).FirstOrDefault();

                ShiftModelObj.BranchId = ShiftEdit.BranchId;
                ShiftModelObj.ShiftCode = ShiftEdit.ShiftCode;
                ShiftModelObj.ShiftId = ShiftEdit.ShiftId;
                ShiftModelObj.CompanyId = Convert.ToInt32(ShiftEdit.Company.CompanyId);

                ShiftModelObj.FirstInTime = Convert.ToString(ShiftEdit.FirstInTime.TimeOfDay);
                ShiftModelObj.FirstOutTime = Convert.ToString(ShiftEdit.FirstOutTime.TimeOfDay);
                ShiftModelObj.SecondInTime = Convert.ToString(ShiftEdit.SecondInTime.Value.TimeOfDay);
                ShiftModelObj.SecondOutTime = Convert.ToString(ShiftEdit.SecondOutTime.Value.TimeOfDay);
                ShiftModelObj.LunchStartTime = Convert.ToString(ShiftEdit.LunchStartTime.Value.TimeOfDay);
                ShiftModelObj.LunchEndTime = Convert.ToString(ShiftEdit.LunchEndTime.Value.TimeOfDay);
                ShiftModelObj.LunchTime = Convert.ToString(ShiftEdit.LunchTime.Value.TimeOfDay);
                ShiftModelObj.WorkHours = Convert.ToString(ShiftEdit.WorkingHours.TimeOfDay);


                //ShiftModelObj.FirstInTimeHH = ShiftEdit.FirstInTime.Hour.ToString("D2");
                //ShiftModelObj.FirstInTimeMM = ShiftEdit.FirstInTime.Minute.ToString("D2");
                //ShiftModelObj.FirstOutHH = ShiftEdit.FirstOutTime.Hour.ToString("D2");
                //ShiftModelObj.FirstOutMM = ShiftEdit.FirstOutTime.Minute.ToString("D2");
                //ShiftModelObj.LunchEndHH = ShiftEdit.LunchEndTime.Value.Hour.ToString("D2");
                //ShiftModelObj.LunchEndMM = ShiftEdit.LunchEndTime.Value.Minute.ToString("D2");
                //ShiftModelObj.LunchStartHH = ShiftEdit.LunchStartTime.Value.Hour.ToString("D2");
                //ShiftModelObj.LunchStartMM = ShiftEdit.LunchStartTime.Value.Minute.ToString("D2");
                //if (ShiftEdit.LunchTime != null)
                //{
                //    ShiftModelObj.LunchTimeMM = ShiftEdit.LunchTime.Value.Minute.ToString("D2");
                //    ShiftModelObj.LunchTimeHH = ShiftEdit.LunchTime.Value.Hour.ToString("D2");
                //}
                //else
                //{
                //    ShiftModelObj.LunchTimeMM = "00";
                //    ShiftModelObj.LunchTimeHH = "00";
                //}

                //if (ShiftEdit.LunchTime != null)
                //{
                //    ShiftModelObj.LunchTimeMM = ShiftEdit.LunchTime.Value.Minute.ToString("D2");
                //    ShiftModelObj.LunchTimeHH = ShiftEdit.LunchTime.Value.Hour.ToString("D2");
                //}
                ShiftModelObj.LunchTimeExcludeFlag = Convert.ToBoolean(ShiftEdit.LunchTimeExcludeFlag);
                ShiftModelObj.NightShiftFlag = Convert.ToBoolean(ShiftEdit.NightShiftFlag);
                //ShiftModelObj.SecondInHH = ShiftEdit.SecondInTime.Value.Hour.ToString("D2");
                //ShiftModelObj.SecondInMM = ShiftEdit.SecondInTime.Value.Minute.ToString("D2");
                //ShiftModelObj.SecondOutHH = ShiftEdit.SecondOutTime.Value.Hour.ToString("D2");
                //ShiftModelObj.SecondOutMM = ShiftEdit.SecondOutTime.Value.Minute.ToString("D2");
                ShiftModelObj.ShiftName = ShiftEdit.ShiftName;
                ShiftModelObj.ShiftType = ShiftEdit.ShiftType;
                ShiftModelObj.ShiftCode = ShiftEdit.ShiftCode;
                //ShiftModelObj.WorkHoursHH = ShiftEdit.WorkingHours.Hour.ToString("D2");
                //ShiftModelObj.WorkHoursMM = ShiftEdit.WorkingHours.Minute.ToString("D2");
                //ShiftModelObj.WorkingHours = ShiftEdit.WorkingHours;

                // Added by Rajas on 29 MARCH 2017
                ShiftModelObj.MarkedAsDelete = ShiftEdit.MarkedAsDelete;  // MarkedAsDelete

                //// MINUTE LIST
                //List<SelectListItem> MinSelList = new List<SelectListItem>();
                //for (int i = 0; i < 60; i++)
                //{
                //    MinSelList.Add(new SelectListItem { Text = i.ToString("D2"), Value = i.ToString("D2") });
                //}
                //ViewBag.MinListVB = new SelectList(MinSelList, "Value", "Text").ToList();

                //// HOUR LIST
                //List<SelectListItem> HourSelList = new List<SelectListItem>();
                //for (int i = 0; i < 24; i++)
                //{
                //    HourSelList.Add(new SelectListItem { Text = i.ToString("D2"), Value = i.ToString("D2") });
                //}
                //ViewBag.HourListVB = new SelectList(HourSelList, "Value", "Text").ToList();

                // Added by Rajas on 17 JAN 2017
                PopulateDropDownEdit(ShiftModelObj);

                return View(ShiftModelObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Incorrect data. Please try again!");

                return RedirectToAction("Index"); // Verify ?
            }
        }


        /// <summary>
        /// Added by Rajas on 29 MARCH 2017
        /// DELETE POST
        /// </summary>
        /// <param name="id"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                Shift ShiftObj = WetosDB.Shifts.Where(a => a.ShiftId == id).FirstOrDefault();

                List<Employee> EmployeeListObj = WetosDB.Employees.Where(a => a.DefaultShift == ShiftObj.ShiftCode).ToList();

                if (EmployeeListObj.Count > 0)
                {
                    // Updated by Rajas on 15 MAY 2017 START
                    Information("You can not delete this Shift, as " + ShiftObj.ShiftCode + " is assigned to employee");

                    AddAuditTrail("Can not delete Shift as " + ShiftObj.ShiftCode + " is used in employee");
                    // Updated by Rajas on 15 MAY 2017 END

                    return RedirectToAction("Index");
                }

                if (ShiftObj != null)
                {
                    ShiftObj.MarkedAsDelete = 1;

                    WetosDB.SaveChanges();

                    Success("Shift : " + ShiftObj.ShiftCode + " " + ShiftObj.ShiftName + " deleted successfully");

                    AddAuditTrail("Shift : " + ShiftObj.ShiftCode + " " + ShiftObj.ShiftName + " deleted successfully");
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
            //var BranchList = WetosDB.Branches.Where(a => a.Company.CompanyId == SelCompanyId && (a.MarkedAsDelete == null || a.MarkedAsDelete == 0)).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList();
            var BranchList = WetosDB.SP_GetBranchList(EmployeeId).Where(a => a.CompanyId == SelCompanyId).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList(); //.Select(m => new { BranchId = m.BranchId, BranchName = m.BranchName}).ToList();
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
            #endregion

            return Json(BranchList);
        }

        /// <summary>
        /// Json return to get Working hours
        /// Added by Rajas on 3 JUNE 2017
        /// </summary>
        /// <returns></returns>
        public JsonResult GetWorkHours(string FirstInTime, string FirstOutTime, string flag, string IsOutPunchInNextDay)
        {

            if (string.IsNullOrEmpty(FirstInTime))
            {
                FirstInTime = "00:00:00";
            }

            if (string.IsNullOrEmpty(FirstOutTime))
            {
                FirstOutTime = "00:00:00";
            }

            string[] FirstInTimeArray = FirstInTime.Split(':');
            if (FirstInTimeArray[0].Contains('_') || FirstInTimeArray[1].Contains('_') || FirstInTimeArray[2].Contains('_'))
            {
                FirstInTime = "00:00:00";
            }

            string[] FirstOutTimeArray = FirstOutTime.Split(':');
            if (FirstOutTimeArray[0].Contains('_') || FirstOutTimeArray[1].Contains('_') || FirstOutTimeArray[2].Contains('_'))
            {
                FirstOutTime = "00:00:00";
            }


            DateTime FIT = Convert.ToDateTime(FirstInTime);
            DateTime FOT = Convert.ToDateTime(FirstOutTime);

            string Workinghours = string.Empty;

            //Added by Shalaka on 22nd DEC 2017 --- Start to solve Intime 00:00:00 issue
            if (FirstInTime != "00:00:00")
            {
                if (flag != null && flag.Trim() == "true")
                {
                    FOT = FOT.AddDays(1);

                    Workinghours = FOT.Subtract(FIT).ToString();

                }
                else
                {
                    //CODE ADDED BY SHRADDHA ON 15 JAN 2018 START
                    if (IsOutPunchInNextDay != null && IsOutPunchInNextDay.Trim() == "true")
                    {
                        FOT = FOT.AddDays(1);
                    }
                    //CODE ADDED BY SHRADDHA ON 15 JAN 2018 END
                    Workinghours = FOT.Subtract(FIT).ToString().Trim();
                }
            }
            else
            {
                //Workinghours = (FOT.TimeOfDay - FIT.TimeOfDay).ToString().Trim();

                Workinghours = FOT.Subtract(FIT).ToString().Trim();
            }

            return Json(Workinghours, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Json return to get Working hours during break shift
        /// Added by Rajas on 19 SEP 2017
        /// </summary>
        /// <returns></returns>
        public JsonResult GetWorkHoursForBreakShift(string FirstInTime, string FirstOutTime, string SecondInTime, string SecondOutTime, string flag)
        {

            if (string.IsNullOrEmpty(FirstInTime))
            {
                FirstInTime = "00:00:00";
            }

            if (string.IsNullOrEmpty(FirstOutTime))
            {
                FirstOutTime = "00:00:00";
            }

            if (string.IsNullOrEmpty(SecondInTime))
            {
                SecondInTime = "00:00:00";
            }

            if (string.IsNullOrEmpty(SecondOutTime))
            {
                SecondOutTime = "00:00:00";
            }

            string[] FirstInTimeArray = FirstInTime.Split(':');
            if (FirstInTimeArray[0].Contains('_') || FirstInTimeArray[1].Contains('_') || FirstInTimeArray[2].Contains('_'))
            {
                FirstInTime = "00:00:00";
            }

            string[] FirstOutTimeArray = FirstOutTime.Split(':');
            if (FirstOutTimeArray[0].Contains('_') || FirstOutTimeArray[1].Contains('_') || FirstOutTimeArray[2].Contains('_'))
            {
                FirstOutTime = "00:00:00";
            }

            string[] SecondInTimeArray = FirstOutTime.Split(':');
            if (SecondInTimeArray[0].Contains('_') || SecondInTimeArray[1].Contains('_') || SecondInTimeArray[2].Contains('_'))
            {
                SecondInTime = "00:00:00";
            }

            string[] SecondOutTimeArray = FirstOutTime.Split(':');
            if (SecondOutTimeArray[0].Contains('_') || SecondOutTimeArray[1].Contains('_') || SecondOutTimeArray[2].Contains('_'))
            {
                SecondOutTime = "00:00:00";
            }


            DateTime FIT = Convert.ToDateTime(FirstInTime);
            DateTime FOT = Convert.ToDateTime(FirstOutTime);
            DateTime SIT = Convert.ToDateTime(SecondInTime);
            DateTime SOT = Convert.ToDateTime(SecondOutTime);
            string Workinghours = string.Empty;

            if (flag.Trim() == "true")
            {
                // Logic for Night Shift break shift

            }
            else
            {
                TimeSpan WorkingFirstHalf = FOT.Subtract(FIT);
                TimeSpan WorkingSecondHalf = SOT.Subtract(SIT);

                Workinghours = WorkingSecondHalf.Add(WorkingFirstHalf).ToString().Trim();


            }

            return Json(Workinghours, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Json return to get Lunch time
        /// Added by Rajas on 3 JUNE 2017
        /// </summary>
        /// <returns></returns>
        public JsonResult GetLunchHours(string LunchStartTime, string LunchEndTime, string flag)
        {
            if (string.IsNullOrEmpty(LunchStartTime))
            {
                LunchStartTime = "00:00:00";
            }

            if (string.IsNullOrEmpty(LunchEndTime))
            {
                LunchEndTime = "00:00:00";
            }

            string[] LunchStartTimeArray = LunchStartTime.Split(':');
            if (LunchStartTimeArray[0].Contains('_') || LunchStartTimeArray[1].Contains('_') || LunchStartTimeArray[2].Contains('_'))
            {
                LunchStartTime = "00:00:00";
            }

            string[] LunchEndTimeArray = LunchEndTime.Split(':');
            if (LunchEndTimeArray[0].Contains('_') || LunchEndTimeArray[1].Contains('_') || LunchEndTimeArray[2].Contains('_'))
            {
                LunchEndTime = "00:00:00";
            }

            DateTime LST = Convert.ToDateTime(LunchStartTime);
            DateTime LET = Convert.ToDateTime(LunchEndTime);

            string Lunchtime = string.Empty;

            if (flag != null && flag.Trim() == "true")
            {
                if (LET.TimeOfDay > LST.TimeOfDay)
                {
                    Lunchtime = LET.Subtract(LST).ToString();
                }
                else
                {
                    LET = LET.AddDays(1);

                    Lunchtime = LET.Subtract(LST).ToString();
                }

            }
            else
            {

                Lunchtime = LET.Subtract(LST).ToString();
            }



            return Json(Lunchtime, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Common Error reporting function
        /// Added by Rajas on 3 JUNE 2017
        /// </summary>
        /// <param name="DepartmentObj"></param>
        /// <param name="ErrorMessage"></param>
        /// <returns></returns>
        public ActionResult ReportError(ShiftModel ShiftModelObj, string ErrorMessage)
        {
            PopulateDropDownEdit(ShiftModelObj);//

            AddAuditTrail(ErrorMessage);
            Error(ErrorMessage);

            return View(ShiftModelObj);
        }

        /// <summary>
        /// Common function to validate the data for edit or create
        /// Added by Rajas on 30 DEC 2016
        /// </summary>
        /// Updated by Rajas on 27 MARCH 2017
        private bool UpdateShiftData(ShiftModel ShiftModelObj, bool IsEdit, ref string UpdateStatus)
        {
            bool ReturnStatus = false;

            try
            {
                WetosDB.Shift ShiftTblObj = WetosDB.Shifts.Where(a => (a.ShiftId == ShiftModelObj.ShiftId || a.ShiftCode == ShiftModelObj.ShiftCode)
                    && a.Company.CompanyId == ShiftModelObj.CompanyId && a.BranchId == ShiftModelObj.BranchId && a.MarkedAsDelete == 0).FirstOrDefault();


                //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 11 SEPTEMBER 2017 FOR EDIT
                WetosDB.Shift ShiftTblObjEDIT = WetosDB.Shifts.Where(a => (a.ShiftId == ShiftModelObj.ShiftId || a.ShiftCode == ShiftModelObj.ShiftCode)
                    && a.Company.CompanyId == ShiftModelObj.CompanyId && a.BranchId == ShiftModelObj.BranchId && a.MarkedAsDelete == 0).FirstOrDefault();


                // ADDED BY RAJAS ON 03 JUNE 2017 START
                bool IsNew = false;

                // IS OBJECT PRESET
                if (ShiftTblObj != null)  // Added by Rajas on 2 JUNE 2017
                {
                    if (IsEdit == false)  // CREATE            
                    {
                        UpdateStatus = "Shift already available."; //WetosEmployeeController.GetErrorMessage(1);

                        //AddAuditTrail("Error in Department update : " + UpdateStatus);
                        return ReturnStatus;
                    }
                }
                else
                {
                    if (IsEdit == false) // CREATE    
                    {
                        ShiftTblObj = new WetosDB.Shift();
                        IsNew = true;
                    }
                    else // EDIT    
                    {
                        UpdateStatus = "Error in updating Shift."; // WetosEmployeeController.GetErrorMessage(1); 
                        //AddAuditTrail("Error in Department update : " + UpdateStatus);
                        return ReturnStatus;
                    }
                }
                // ADDED BY RAJAS ON 03 JUNE 2017 END

                // New Leave table object
                ShiftTblObj.Company = WetosDB.Companies.Where(a => a.CompanyId == ShiftModelObj.CompanyId).FirstOrDefault();
                ShiftTblObj.BranchId = ShiftModelObj.BranchId;
                ShiftTblObj.ShiftName = ShiftModelObj.ShiftName;
                ShiftTblObj.ShiftType = ShiftModelObj.ShiftType;
                ShiftTblObj.ShiftCode = ShiftModelObj.ShiftCode;

                ShiftTblObj.FirstInTime = Convert.ToDateTime(ShiftModelObj.FirstInTime);
                ShiftTblObj.FirstOutTime = Convert.ToDateTime(ShiftModelObj.FirstOutTime);

                //CODE ADDED BY SHRADDHA ON 15 JAN 2018 START
                if (ShiftModelObj.IsOutPunchInNextDay == true)
                {
                    ShiftTblObj.FirstOutTime = ShiftTblObj.FirstOutTime.AddDays(1);
                }
                //CODE ADDED BY SHRADDHA ON 15 JAN 2018 END
                if (ShiftModelObj.SecondInTime == null)
                {
                    ShiftTblObj.SecondInTime = Convert.ToDateTime("00:00:00");
                }
                else
                {
                    ShiftTblObj.SecondInTime = Convert.ToDateTime(ShiftModelObj.SecondInTime);
                }

                if (ShiftModelObj.SecondOutTime == null)
                {
                    ShiftTblObj.SecondOutTime = Convert.ToDateTime("00:00:00");
                }
                else
                {
                    ShiftTblObj.SecondOutTime = Convert.ToDateTime(ShiftModelObj.SecondOutTime);
                }

                DateTime WrkHrs = Convert.ToDateTime(ShiftModelObj.WorkHours);

                ShiftTblObj.WorkingHours = WrkHrs;
                ShiftTblObj.LunchStartTime = Convert.ToDateTime(ShiftModelObj.LunchStartTime);
                ShiftTblObj.LunchEndTime = Convert.ToDateTime(ShiftModelObj.LunchEndTime);
                ShiftTblObj.LunchTime = Convert.ToDateTime(ShiftModelObj.LunchTime);
                ShiftTblObj.IsOutPunchInNextDay = ShiftModelObj.IsOutPunchInNextDay; //CODE ADDED BY SHRADDHA ON 15 JAN 2018 FOR IsOutPunchInNextDay
                //ShiftTblObj.FirstInTime = Convert.ToDateTime(ShiftModelObj.FirstInTimeHH + ":" + ShiftModelObj.FirstInTimeMM);
                //ShiftTblObj.FirstOutTime = Convert.ToDateTime(ShiftModelObj.FirstOutHH + ":" + ShiftModelObj.FirstOutMM);

                ////CODE CHANGED BY SHRADDHA ON 15 MARCH 2017 TO GET ACCURATE DATA START
                //ShiftTblObj.SecondInTime = Convert.ToDateTime(ShiftModelObj.FirstInTimeHH + ":" + ShiftModelObj.FirstInTimeMM);
                //ShiftTblObj.SecondOutTime = Convert.ToDateTime(ShiftModelObj.FirstOutHH + ":" + ShiftModelObj.FirstOutMM);
                ////if (ShiftModelObj.SecondInHH != null && ShiftModelObj.SecondInMM != null)
                ////{
                ////    ShiftTblObj.SecondInTime = Convert.ToDateTime(ShiftModelObj.SecondInHH + ":" + ShiftModelObj.SecondInMM);
                ////}
                ////if (ShiftModelObj.SecondOutHH != null && ShiftModelObj.SecondOutMM != null)
                ////{
                ////    ShiftTblObj.SecondOutTime = Convert.ToDateTime(ShiftModelObj.SecondOutHH + ":" + ShiftModelObj.SecondOutMM);
                ////}
                ////CODE CHANGED BY SHRADDHA ON 15 MARCH 2017 TO GET ACCURATE DATA END


                //if (ShiftModelObj.LunchStartHH != null && ShiftModelObj.LunchStartMM != null)
                //{
                //    ShiftTblObj.LunchStartTime = Convert.ToDateTime(ShiftModelObj.LunchStartHH + ":" + ShiftModelObj.LunchStartMM);
                //}
                //if (ShiftModelObj.LunchEndHH != null && ShiftModelObj.LunchEndMM != null)
                //{
                //    ShiftTblObj.LunchEndTime = Convert.ToDateTime(ShiftModelObj.LunchEndHH + ":" + ShiftModelObj.LunchEndMM);
                //}
                //// ShiftTblObj.LunchTime = ShiftModelObj.LunchTime;
                //if (ShiftModelObj.WorkHoursHH != null && ShiftModelObj.WorkHoursMM != null)
                //{
                //    ShiftTblObj.WorkingHours = Convert.ToDateTime(ShiftModelObj.WorkHoursHH + ":" + ShiftModelObj.WorkHoursMM);
                //}
                //if (ShiftModelObj.LunchTimeHH != null && ShiftModelObj.LunchTimeMM != null)
                //{
                //    ShiftTblObj.LunchTime = Convert.ToDateTime(ShiftModelObj.LunchTimeHH + ":" + ShiftModelObj.LunchTimeMM);
                //}
                // Updated by Rajas on 14 JAN 2017
                ShiftTblObj.LunchTimeExcludeFlag = ShiftModelObj.LunchTimeExcludeFlag;

                //COMMENTED BY SHRADDHA ON 15 JAN 2018 BECAUSE IT IS ALREADY BEING REDUCE LUNCH TIME ON VIEW AND BECAUSE OF THIS CODE IT IS BEING REDUCED TWICE START
                //ADDED BY SHALAKA ON 05TH DEC 2017 TO CALCULATE WORKING HRS AFTER EXCLUDING LUNCH TIME FROM SHIFT -- START
                //if (ShiftModelObj.LunchTimeExcludeFlag == true)
                //{
                //    DateTime WorkHrs = Convert.ToDateTime(ShiftModelObj.WorkHours);
                //    DateTime LunchTime = Convert.ToDateTime(ShiftModelObj.LunchTime);

                //    TimeSpan WH = WorkHrs.Subtract(LunchTime);
                //    ShiftTblObj.WorkingHours = Convert.ToDateTime(WH.ToString());
                //}
                //else
                //{
                //    ShiftTblObj.WorkingHours = Convert.ToDateTime(ShiftModelObj.WorkHours);
                //}
                //ADDED BY SHALAKA ON 05TH DEC 2017 TO CALCULATE WORKING HRS AFTER EXCLUDING LUNCH TIME FROM SHIFT -- END
                //COMMENTED BY SHRADDHA ON 15 JAN 2018 BECAUSE IT IS ALREADY BEING REDUCE LUNCH TIME ON VIEW AND BECAUSE OF THIS CODE IT IS BEING REDUCED TWICE END

                ShiftTblObj.NightShiftFlag = ShiftModelObj.NightShiftFlag;

                // Added by Rajas on 29 MARCH 2017
                ShiftTblObj.MarkedAsDelete = 0;   // Default value zero

                // Add new table object 
                if (IsNew)
                {
                    WetosDB.Shifts.Add(ShiftTblObj);
                }

                WetosDB.SaveChanges();


                //-------------------------------------------AuditLog---------------------------------------------------------------------------

                if (IsNew)
                {
                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 11 SEPTEMBER 2017
                    #region ADD AUDIT LOG
                    //OLD RECORD IS BLANK
                    string Newrecord = "ShiftId :" + ShiftTblObj.ShiftId + ", ShiftName :" + ShiftTblObj.ShiftName
                        + ", ShiftType :" + ShiftTblObj.ShiftType + ", FirstInTime :" + ShiftTblObj.FirstInTime
                        + ", FirstOutTime :" + ShiftTblObj.FirstOutTime + ",SecondInTime:" + ShiftTblObj.SecondInTime
                        + ", SecondOutTime :" + ShiftTblObj.SecondOutTime + ", LunchStartTime :" + ShiftTblObj.LunchStartTime
                        + ", LunchEndTime :" + ShiftTblObj.LunchEndTime + ", LunchTime :" + ShiftTblObj.LunchTime
                        + ", LunchTimeExcludeFlag :" + ShiftTblObj.LunchTimeExcludeFlag
                        + ",WorkingHours :" + ShiftTblObj.WorkingHours + ", NightShiftFlag :" + ShiftTblObj.NightShiftFlag
                        + ", CompanyId :" + ShiftTblObj.Company.CompanyId + ", BranchId : " + ShiftTblObj.BranchId;


                    //LOGIN ID IS TAKEN FROM SESSION PERSISTER
                    string Formname = "SHIFT MASTER";
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
                    string Oldrecord = "ShiftId :" + ShiftTblObjEDIT.ShiftId + ", ShiftName :" + ShiftTblObjEDIT.ShiftName
                        + ", ShiftType :" + ShiftTblObjEDIT.ShiftType + ", FirstInTime :" + ShiftTblObjEDIT.FirstInTime
                        + ", FirstOutTime :" + ShiftTblObjEDIT.FirstOutTime + ",SecondInTime:" + ShiftTblObjEDIT.SecondInTime
                        + ", SecondOutTime :" + ShiftTblObjEDIT.SecondOutTime + ", LunchStartTime :" + ShiftTblObjEDIT.LunchStartTime
                        + ", LunchEndTime :" + ShiftTblObjEDIT.LunchEndTime + ", LunchTime :" + ShiftTblObjEDIT.LunchTime
                        + ", LunchTimeExcludeFlag :" + ShiftTblObjEDIT.LunchTimeExcludeFlag
                        + ",WorkingHours :" + ShiftTblObjEDIT.WorkingHours + ", NightShiftFlag :" + ShiftTblObjEDIT.NightShiftFlag
                        + ", CompanyId :" + ShiftTblObjEDIT.Company.CompanyId + ", BranchId : " + ShiftTblObjEDIT.BranchId;
                    //---x---------x-----------------x------------x----------------x-----------x-------------x------------x--------------x----------x---------x-----
                    string Newrecord = "ShiftId :" + ShiftTblObj.ShiftId + ", ShiftName :" + ShiftTblObj.ShiftName
                        + ", ShiftType :" + ShiftTblObj.ShiftType + ", FirstInTime :" + ShiftTblObj.FirstInTime
                        + ", FirstOutTime :" + ShiftTblObj.FirstOutTime + ",SecondInTime:" + ShiftTblObj.SecondInTime
                        + ", SecondOutTime :" + ShiftTblObj.SecondOutTime + ", LunchStartTime :" + ShiftTblObj.LunchStartTime
                        + ", LunchEndTime :" + ShiftTblObj.LunchEndTime + ", LunchTime :" + ShiftTblObj.LunchTime
                        + ", LunchTimeExcludeFlag :" + ShiftTblObj.LunchTimeExcludeFlag
                        + ",WorkingHours :" + ShiftTblObj.WorkingHours + ", NightShiftFlag :" + ShiftTblObj.NightShiftFlag
                        + ", CompanyId :" + ShiftTblObj.Company.CompanyId + ", BranchId : " + ShiftTblObj.BranchId;


                    //LOGIN ID IS TAKEN FROM SESSION PERSISTER
                    string Formname = "SHIFT MASTER";
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
            }
            catch (System.Exception ex)
            {
                UpdateStatus = "Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message);

                return ReturnStatus;
            }

        }

        /// <summary>
        /// Added by Rajas on 17 JAN 2017 for dropdown data common function
        /// </summary>
        /// Updated by Rajas on 27 MARCH 2017
        private void PopulateDropDown()
        {
            try
            {
                // Drop down for company list
                // Updated by Rajas on 6 MAY 2017 to get data which is not MarkedAsDelete

                #region COMPANY LIST CODE ADDED BY SHRADDHA ON 30 MAR 2018
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //var CompanyList = WetosDB.Companies.Select(a => new { CompanyId = a.CompanyId, CompanyName = a.CompanyName }).ToList();
                var CompanyList = WetosDB.SP_GetCompanyList(EmployeeId).Select(m => new { Companyid = m.CompanyId, CompanyName = m.CompanyName }).ToList();
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
                #endregion

                ViewBag.CompanyNameList = new SelectList(CompanyList, "CompanyId", "CompanyName").ToList();

                // Drop down for Branch name
                var BranchList = new List<Branch>(); //WetosDB.Branches.Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList();
                ViewBag.BranchNameList = new SelectList(BranchList, "BranchId", "BranchName").ToList();

                // Drop down shift type
                var ShiftList = WetosDB.Shifts.Where(a => a.MarkedAsDelete == 0).Select(a => new { ShiftId = a.ShiftName, ShiftName = a.ShiftName }).ToList();
                ViewBag.ShiftTypeList = new SelectList(ShiftList, "ShiftId", "ShiftName").ToList();

                // Drop down list for shift type
                var ShiftTypeList = WetosDB.DropdownDatas.Where(a => a.TypeId == 13).Select(a => new { ShiftTypeId = a.Text, ShiftTypeName = a.Text }).ToList();
                ViewBag.ShiftTypeNameList = new SelectList(ShiftTypeList, "ShiftTypeId", "ShiftTypeName").ToList();
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );
            }


        }

        /// <summary>
        /// Added by Rajas on 17 JAN 2017 for dropdown data common function
        /// </summary>
        /// Updated by Rajas on 3 JUNE 2017
        private void PopulateDropDownEdit(ShiftModel ShiftModelObj)
        {
            try
            {
                // Drop down for company list
                // Updated by Rajas on 6 MAY 2017 to get data which is not MarkedAsDelete

                #region COMPANY LIST CODE ADDED BY SHRADDHA ON 30 MAR 2018
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //var CompanyList = WetosDB.Companies.Select(a => new { CompanyId = a.CompanyId, CompanyName = a.CompanyName }).ToList();
                var CompanyList = WetosDB.SP_GetCompanyList(EmployeeId).Select(m => new { Companyid = m.CompanyId, CompanyName = m.CompanyName }).ToList();
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
                #endregion

                ViewBag.CompanyNameList = new SelectList(CompanyList, "CompanyId", "CompanyName").ToList();

                // Drop down for Branch name


                #region CODE FOR GET BRANCH LIST ADDED BY SHRADDHA ON 30 MAR 2018
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
                //var BranchList = WetosDB.Branches.Where(a => (a.MarkedAsDelete == null || a.MarkedAsDelete == 0) && a.Company.CompanyId == ShiftModelObj.CompanyId)
                //.Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList();
                var BranchList = WetosDB.SP_GetBranchList(EmployeeId).Where(a => a.CompanyId == ShiftModelObj.CompanyId).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList(); //.Select(m => new { BranchId = m.BranchId, BranchName = m.BranchName}).ToList();
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
                #endregion

                ViewBag.BranchNameList = new SelectList(BranchList, "BranchId", "BranchName").ToList();

                // Drop down shift type
                // Updated by Rajas on 31 MAY 2017 for ShiftId
                var ShiftList = WetosDB.Shifts.Where(a => (a.MarkedAsDelete == null || a.MarkedAsDelete == 0)
                    && a.Company.CompanyId == ShiftModelObj.CompanyId && a.BranchId == ShiftModelObj.BranchId)
                    .Select(a => new { ShiftId = a.ShiftId, ShiftName = a.ShiftCode }).ToList();
                ViewBag.ShiftTypeList = new SelectList(ShiftList, "ShiftId", "ShiftName").ToList();

                // Drop down list for shift type
                var ShiftTypeList = WetosDB.DropdownDatas.Where(a => a.TypeId == 13).Select(a => new { ShiftTypeId = a.Text, ShiftTypeName = a.Text }).ToList();
                ViewBag.ShiftTypeNameList = new SelectList(ShiftTypeList, "ShiftTypeId", "ShiftTypeName").ToList();
                // Updated by Rajas on 6 MAY 2017 to get data which is not MarkedAsDelete END
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );
            }

        }

        //Start
        //Added by shalaka On 06th DEC 2017 to reflect Working Hrs on Screen after Excluding Lunch Time from working hrs.
        [HttpPost]
        public JsonResult ExcludeLunchTime(DateTime WorkingHrs, DateTime LunchTime, string FirstInTime, string FirstOutTime, string flag)
        {

            string Workinghours = "00:00:00";
            //if (WorkingHrs != null || LunchTime != null)
            //    {
            //        TimeSpan WH = WorkingHrs.Subtract(LunchTime);
            //        Workinghours = WH.ToString();
            //    }
            //    else
            //    {
            //        Workinghours = WorkingHrs.ToString();
            //    }

            DateTime FIT = Convert.ToDateTime(FirstInTime);
            DateTime FOT = Convert.ToDateTime(FirstOutTime);

            //Midified by Shalaka on 22nd DEC 2017 --- Start
            //If flag is false it will consider as Lunch time Exclude flag is checked.
            if (flag == "false")
            {
                if (WorkingHrs != null || LunchTime != null)
                {
                    TimeSpan WH = WorkingHrs.Subtract(LunchTime);
                    Workinghours = WH.ToString();
                }
                else
                {
                    Workinghours = WorkingHrs.ToString();
                }
            }
            else
            {
                if (FirstInTime != "00:00:00")
                {
                    //Flag false considering as true
                    if (flag != null && flag.Trim() == "false")
                    {
                        FOT = FOT.AddDays(1);

                        Workinghours = FOT.Subtract(FIT).ToString();

                    }
                    else
                    {
                        Workinghours = FOT.Subtract(FIT).ToString().Trim();
                    }
                }
                else
                {
                    Workinghours = FOT.Subtract(FIT).ToString().Trim();
                }

            }

            return Json(Workinghours, JsonRequestBehavior.AllowGet);
        }
        //End   

    }
}
