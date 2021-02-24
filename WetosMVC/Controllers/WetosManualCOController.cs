using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web.Mvc;
using WetosDB;
using WetosMVCMainApp.Models;
using System.Globalization;

namespace WetosMVC.Controllers
{
    public class WetosManualCOController : BaseController
    {
        //
        // GET: /WetosManualCO/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Updated by Rajas 
        /// </summary>
        /// <param name="MySelf"></param>
        /// <returns></returns>
        public ActionResult ManualCompOffCreate(bool MySelf)
        {
            try
            {
                ManualCompOffModel ManualCompOffModelObj = new ManualCompOffModel();

                if (MySelf == true)
                {
                    ManualCompOffModelObj.EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                    ManualCompOffModelObj.CompanyId = Convert.ToInt32(Session["CompanyId"]);
                    ManualCompOffModelObj.BranchId = Convert.ToInt32(Session["BranchId"]);
                }

                ManualCompOffModelObj.MySelf = MySelf;
                PopulateDropDownManualCompOff();
                return View(ManualCompOffModelObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in Manual CO due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ManualCompOffId"></param>
        /// <returns></returns>
        /// Error Handling updated by Rajas on 29 SEP 2017
        public ActionResult ManualCompOffEdit(int ManualCompOffId)
        {
            try
            {
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                ManualCompOff ManualCompOffEditObj = WetosDB.ManualCompOffs.Where(a => a.ManualCompOffId == ManualCompOffId).FirstOrDefault();
                ManualCompOffModel ManualCompOffModelObj = new ManualCompOffModel();
                ManualCompOffModelObj.BranchId = ManualCompOffEditObj.BranchId;
                ManualCompOffModelObj.CompanyId = ManualCompOffEditObj.CompanyId;
                ManualCompOffModelObj.CompOffBalance = ManualCompOffEditObj.CompOffBalance;
                ManualCompOffModelObj.CompOffPurpose = ManualCompOffEditObj.CompOffPurpose;
                ManualCompOffModelObj.EmployeeId = ManualCompOffEditObj.EmployeeId;
                ManualCompOffModelObj.ExtraWorkingHrs = ManualCompOffEditObj.ExtraWorkingHrs;
                ManualCompOffModelObj.FromDate = ManualCompOffEditObj.FromDate;

                ManualCompOffModelObj.EffectiveDate = ManualCompOffEditObj.EffectiveDate; //CODE ADDED BY SHRADDHA ON 16 MAR 2018

                //CODE ADDED BY SHRADDHA ON 30 JAN 2018 START
                ManualCompOffModelObj.IsInPunchInNextDay = ManualCompOffEditObj.IsInPunchInNextDay;
                ManualCompOffModelObj.IsOutPunchInNextDay = ManualCompOffEditObj.IsOutPunchInNextDay;
                //CODE ADDED BY SHRADDHA ON 30 JAN 2018 END

                // Uncommented by Rajas on 29 SEP 2017 START
                int CurrentYearInt = DateTime.Now.Year; // ADDED BY MSJ ON 02 JAN 2018
                ManualCompOffModelObj.LoginTime = ManualCompOffEditObj.LoginTime == null ? new DateTime(CurrentYearInt, 01, 01, 00, 00, 00) : ManualCompOffEditObj.LoginTime.Value;
                ManualCompOffModelObj.LogoutTime = ManualCompOffEditObj.LogoutTime == null ? new DateTime(CurrentYearInt, 01, 01, 00, 00, 00) : ManualCompOffEditObj.LogoutTime.Value;
                // Uncommented by Rajas on 29 SEP 2017 END

                ManualCompOffModelObj.ManualCompOffId = ManualCompOffEditObj.ManualCompOffId;
                ManualCompOffModelObj.MarkedAsDelete = ManualCompOffEditObj.MarkedAsDelete;
                ManualCompOffModelObj.RejectReason = ManualCompOffEditObj.RejectReason;
                ManualCompOffModelObj.StatusId = ManualCompOffEditObj.StatusId;

                ManualCompOffModelObj.Purpose = ManualCompOffEditObj.Reason;

                if (ManualCompOffModelObj.EmployeeId == EmployeeId)
                {
                    ManualCompOffModelObj.MySelf = true;
                }
                else
                {
                    ManualCompOffModelObj.MySelf = false;
                }
                PopulateDropDownManualCompOffEdit(ManualCompOffModelObj);
                return View(ManualCompOffModelObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in Manual CompoffEdit due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );
                Error("Error occured while compoff edit.");
                return RedirectToAction("ManualCompOffList");
            }
        }

        [HttpPost]
        public ActionResult ManualCompOffEdit(ManualCompOffModel ManualCompOffObj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (ManualCompOffObj.FromDate >= DateTime.Now)
                    {
                        Error("Extra working Date should be earlier than Today.");
                        AddAuditTrail("Manual Comp Off Entry addition failed for ManualCompOffId:" + ManualCompOffObj.ManualCompOffId);
                        PopulateDropDownManualCompOff();
                        return View(ManualCompOffObj);
                    }

                    ManualCompOff ExistingCompOffAvailableOrnot = WetosDB.ManualCompOffs.Where(a => a.EmployeeId == ManualCompOffObj.EmployeeId && a.FromDate == ManualCompOffObj.FromDate && (a.StatusId == 1 || a.StatusId == 2 || a.StatusId == 4) && (a.MarkedAsDelete == 0 || a.MarkedAsDelete == null) && a.ManualCompOffId != ManualCompOffObj.ManualCompOffId).FirstOrDefault();
                    if (ExistingCompOffAvailableOrnot != null)
                    {
                        Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == ExistingCompOffAvailableOrnot.EmployeeId).FirstOrDefault();
                        Error("You Can not Apply Manual CompOff For " + ExistingCompOffAvailableOrnot.FromDate.ToString("dd-MMM-yyyy") + " for selected employee " + EmployeeObj.EmployeeCode + " - " + EmployeeObj.FirstName + " " + EmployeeObj.LastName
                                            + "<br/>" + "You may already have pending or approved/sanctioned Manual CompOff for this date range.");
                        PopulateDropDownManualCompOff();
                        return View(ManualCompOffObj);
                    }

                    if (ManualCompOffObj.CompOffBalance < 0.5)
                    {
                        Error("Extra working time is less than required time limit for half day");
                        PopulateDropDownManualCompOff();
                        return View(ManualCompOffObj);
                    }

                    bool IsReturnSuccess = UpdateManualCompOff(ManualCompOffObj);
                    if (IsReturnSuccess == true)
                    {
                        AddAuditTrail("Manual Comp Off Entry Updated Successfully for ManualCompOffId:" + ManualCompOffObj.ManualCompOffId);
                        Success("Manual Comp Off Entry Updated Successfully");

                        if (ManualCompOffObj.MySelf == true)
                        {
                            return RedirectToAction("ManualCompOffList");
                        }
                        else
                        {
                            return RedirectToAction("ManualCompOffList", new { IsMySelf = "false" });
                        }
                    }
                    else
                    {
                        Error("Manual Comp Off Entry Updation failed");
                        AddAuditTrail("Manual Comp Off Entry Updation failed for ManualCompOffId:" + ManualCompOffObj.ManualCompOffId);
                        PopulateDropDownManualCompOffEdit(ManualCompOffObj);
                        return View(ManualCompOffObj);
                    }
                }
                else
                {
                    PopulateDropDownManualCompOffEdit(ManualCompOffObj);
                    return View(ManualCompOffObj);
                }
            }
            catch (System.Exception E)
            {
                AddAuditTrail("Error Occured while Updating Manual Comp off for ManualCompOffId:" + ManualCompOffObj.ManualCompOffId + " " + E.Message);
                PopulateDropDownManualCompOffEdit(ManualCompOffObj);
                return View(ManualCompOffObj);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ManualCompOffObj"></param>
        /// <returns></returns>
        private bool UpdateManualCompOff(ManualCompOffModel ManualCompOffObj)
        {
            bool ReturnStatus = false;

            try
            {
                ManualCompOff ManualCompOffEditObj = WetosDB.ManualCompOffs.Where(a => (a.ManualCompOffId == ManualCompOffObj.ManualCompOffId || (a.CompanyId == ManualCompOffObj.CompanyId && a.BranchId == ManualCompOffObj.BranchId && a.EmployeeId == ManualCompOffObj.EmployeeId && a.FromDate == ManualCompOffObj.FromDate)) && (a.MarkedAsDelete == null || a.MarkedAsDelete == 0)).FirstOrDefault();
                bool IsNew = false;
                if (ManualCompOffEditObj == null)
                {
                    ManualCompOffEditObj = new ManualCompOff();
                    IsNew = true;
                }

                ManualCompOffEditObj.StatusId = 1;

                DateTime ExtraHrsDate = new DateTime(ManualCompOffObj.FromDate.Year, ManualCompOffObj.FromDate.Month,
                    ManualCompOffObj.FromDate.Day, ManualCompOffObj.ExtraWorkingHrs.Hour, ManualCompOffObj.ExtraWorkingHrs.Minute, ManualCompOffObj.ExtraWorkingHrs.Second);
                ManualCompOffEditObj.ExtraWorkingHrs = ExtraHrsDate;

                // Uncommented by Rajas on 29 SEP 2017 START
                DateTime LoginTime = new DateTime(ManualCompOffObj.FromDate.Year, ManualCompOffObj.FromDate.Month,
                    ManualCompOffObj.FromDate.Day, ManualCompOffObj.LoginTime.Hour, ManualCompOffObj.LoginTime.Minute, ManualCompOffObj.LoginTime.Second);


                DateTime LogoutTime = new DateTime(ManualCompOffObj.FromDate.Year, ManualCompOffObj.FromDate.Month,
                    ManualCompOffObj.FromDate.Day, ManualCompOffObj.LogoutTime.Hour, ManualCompOffObj.LogoutTime.Minute, ManualCompOffObj.LogoutTime.Second);


                //CODE ADDED BY SHRADDHA ON 29 JAN 2018 START
                DateTime NextDay = ManualCompOffObj.FromDate.AddDays(1);
                if (ManualCompOffObj.IsInPunchInNextDay == true)
                {
                    LoginTime = new DateTime(NextDay.Year, NextDay.Month, NextDay.Day, ManualCompOffObj.LoginTime.Hour, ManualCompOffObj.LoginTime.Minute, ManualCompOffObj.LoginTime.Second);
                }
                else
                {
                    LoginTime = new DateTime(ManualCompOffObj.FromDate.Year, ManualCompOffObj.FromDate.Month, ManualCompOffObj.FromDate.Day, ManualCompOffObj.LoginTime.Hour, ManualCompOffObj.LoginTime.Minute, ManualCompOffObj.LoginTime.Second);
                }
                if (ManualCompOffObj.IsOutPunchInNextDay == true)
                {
                    LogoutTime = new DateTime(NextDay.Year, NextDay.Month, NextDay.Day, ManualCompOffObj.LogoutTime.Hour, ManualCompOffObj.LogoutTime.Minute, ManualCompOffObj.LogoutTime.Second);
                }
                else
                {
                    LogoutTime = new DateTime(ManualCompOffObj.FromDate.Year, ManualCompOffObj.FromDate.Month, ManualCompOffObj.FromDate.Day, ManualCompOffObj.LogoutTime.Hour, ManualCompOffObj.LogoutTime.Minute, ManualCompOffObj.LogoutTime.Second);
                }
                //CODE ADDED BY SHRADDHA ON 29 JAN 2018 END
                ManualCompOffEditObj.LoginTime = LoginTime;
                ManualCompOffEditObj.LogoutTime = LogoutTime;
                ManualCompOffEditObj.CompanyId = ManualCompOffObj.CompanyId;
                ManualCompOffEditObj.BranchId = ManualCompOffObj.BranchId;
                ManualCompOffEditObj.EmployeeId = ManualCompOffObj.EmployeeId;
                ManualCompOffEditObj.FromDate = ManualCompOffObj.FromDate;
                ManualCompOffEditObj.CompOffPurpose = ManualCompOffObj.CompOffPurpose;
                ManualCompOffEditObj.CompOffBalance = ManualCompOffObj.CompOffBalance;

                //CODE ADDED BY SHRADDHA ON 21 FEB 2018 START
                ManualCompOffEditObj.CompOffHours = ManualCompOffEditObj.ExtraWorkingHrs;
                //CODE ADDED BY SHRADDHA ON 21 FEB 2018 END

                // In case of Manual entry through screen
                // Added by Rajas on 29 SEP 2017
                ManualCompOffEditObj.IsAutoEntry = ManualCompOffObj.IsAutoEntry;

                ManualCompOffEditObj.EffectiveDate = ManualCompOffObj.EffectiveDate; //CODE ADDED BY SHRADDHA ON 16 MAR 2018
                ManualCompOffEditObj.Reason = ManualCompOffObj.Purpose;

                if (IsNew == true)
                {
                    WetosDB.ManualCompOffs.AddObject(ManualCompOffEditObj);
                }
                WetosDB.SaveChanges();
                ReturnStatus = true;
            }
            catch (System.Exception E)
            {
                AddAuditTrail(E.Message.ToString() + " " + (E.InnerException == null ? string.Empty : E.InnerException.Message));
                ReturnStatus = false;
            }
            return ReturnStatus;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ManualCompOffObj"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ManualCompOffCreate(ManualCompOffModel ManualCompOffObj)
        {
            try
            {

                if (ModelState.IsValid)
                {

                    ManualCompOff ExistingCompOffAvailableOrnot = WetosDB.ManualCompOffs.Where(a => a.EmployeeId == ManualCompOffObj.EmployeeId && a.FromDate == ManualCompOffObj.FromDate && (a.StatusId == 1 || a.StatusId == 2 || a.StatusId == 4) && (a.MarkedAsDelete == 0 || a.MarkedAsDelete == null)).FirstOrDefault();
                    if (ExistingCompOffAvailableOrnot != null)
                    {
                        Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == ExistingCompOffAvailableOrnot.EmployeeId).FirstOrDefault();
                        Error("You Can not Apply Manual CompOff For " + ExistingCompOffAvailableOrnot.FromDate.ToString("dd-MMM-yyyy") + " for selected employee " + EmployeeObj.EmployeeCode + " - " + EmployeeObj.FirstName + " " + EmployeeObj.LastName
                                            + "<br/>" + "You may already have pending or approved/sanctioned Manual CompOff for this date range.");
                        PopulateDropDownManualCompOff();
                        return View(ManualCompOffObj);
                    }

                    if (ManualCompOffObj.CompOffBalance < 0.5)
                    {

                        Error("Extra working time is less than required time limit");
                        PopulateDropDownManualCompOff();
                        return View(ManualCompOffObj);
                    }

                    int EmployeeGroupId = WetosDB.EmployeeGroupDetails.Where(a => a.Employee.EmployeeId == ManualCompOffObj.EmployeeId).Select(a => a.EmployeeGroup.EmployeeGroupId).FirstOrDefault();
                    RuleTransaction RuleTransactionObj = WetosDB.RuleTransactions.Where(a => a.CompanyId == ManualCompOffObj.CompanyId
                        && a.BranchId == ManualCompOffObj.BranchId && a.EmployeeGroupId == EmployeeGroupId && a.RuleId == 9).FirstOrDefault();
                    if (RuleTransactionObj != null)
                    {
                        if (RuleTransactionObj.Formula.Trim().ToUpper() != "TRUE")
                        {
                            Error("Comp off is not allowed for selected employee");
                            PopulateDropDownManualCompOff();
                            return View(ManualCompOffObj);

                        }
                    }

                    if (ManualCompOffObj.FromDate >= DateTime.Now)
                    {
                        Error("Extra working Date should be earlier than Today.");
                        AddAuditTrail("Manual Comp Off Entry addition failed for ManualCompOffId:" + ManualCompOffObj.ManualCompOffId);
                        PopulateDropDownManualCompOff();
                        return View(ManualCompOffObj);
                    }
                    bool IsReturnSuccess = UpdateManualCompOff(ManualCompOffObj);
                    if (IsReturnSuccess == true)
                    {
                        AddAuditTrail("Manual Comp Off Entry added Successfully for ManualCompOffId:" + ManualCompOffObj.ManualCompOffId);
                        Success("Manual Comp Off Entry Updated Successfully");


                        //ADDED CODE BY SHRADDHA ON 30 JAN 2018 START
                        // SEND ALERT
                        #region LEAVE APPLICATION NOTIFICATION

                        //FOR NOTIFICATION TO BE SENT TO REPORTING PERSON AS APPROVER

                        VwActiveEmployee EmployeeObj = WetosDB.VwActiveEmployees.Where(a => a.EmployeeId == ManualCompOffObj.EmployeeId).FirstOrDefault();

                        // FIRST NOTIFICATION
                        Notification NotificationObj = new Notification();
                        NotificationObj.FromID = EmployeeObj.EmployeeId;
                        NotificationObj.ToID = EmployeeObj.EmployeeReportingId;
                        NotificationObj.SendDate = DateTime.Now;
                        NotificationObj.NotificationContent = "Received Manual Comp Off application for approval from " + EmployeeObj.FirstName + " " + EmployeeObj.LastName + " for " + ManualCompOffObj.FromDate.ToString("dd-MMM-yyyy");
                        NotificationObj.ReadFlag = false;
                        NotificationObj.SendDate = DateTime.Now;

                        // ADD TO NOTIFICATION REPORTING PERSON
                        WetosDB.Notifications.AddObject(NotificationObj);
                        WetosDB.SaveChanges();

                        //Check if both reporting person are are different
                        if (EmployeeObj.EmployeeReportingId != EmployeeObj.EmployeeReportingId2)
                        {
                            // FOR NOTIFICATION TO BE SENT TO REPORTING PERSON AS SANCTIONER
                            Notification NotificationObj3 = new Notification();
                            NotificationObj3.FromID = EmployeeObj.EmployeeId;
                            NotificationObj3.ToID = EmployeeObj.EmployeeReportingId;
                            NotificationObj3.SendDate = DateTime.Now;
                            NotificationObj3.NotificationContent = "Received Manual Comp Off application for approval from " + EmployeeObj.FirstName + " " + EmployeeObj.LastName + " for " + ManualCompOffObj.FromDate.ToString("dd-MMM-yyyy");
                            NotificationObj3.ReadFlag = false;
                            NotificationObj3.SendDate = DateTime.Now;

                            // ADD TO NOTIFICATION
                            WetosDB.Notifications.AddObject(NotificationObj3);
                            WetosDB.SaveChanges();
                        }

                        //FOR SELF NOTIFICATION
                        Notification NotificationObj2 = new Notification();
                        NotificationObj2.FromID = EmployeeObj.EmployeeId;
                        NotificationObj2.ToID = EmployeeObj.EmployeeId;
                        NotificationObj2.SendDate = DateTime.Now;
                        NotificationObj2.NotificationContent = "Manual Comp Off applied successfully for " + ManualCompOffObj.FromDate.ToString("dd-MMM-yyyy");
                        NotificationObj2.ReadFlag = false;
                        NotificationObj2.SendDate = DateTime.Now;

                        WetosDB.Notifications.AddObject(NotificationObj2);

                        WetosDB.SaveChanges();

                        //NOTIFICATION COUNT
                        int NoTiCount = WetosDB.Notifications.Where(a => a.ToID == ManualCompOffObj.EmployeeId && (a.ReadFlag == false || a.ReadFlag == null)).Count();
                        Session["NotificationCount"] = NoTiCount;

                        #endregion

                        #region EMAIL

                        GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.SendEmail).FirstOrDefault();

                        if (GlobalSettingObj != null)
                        {
                            // Send email ON
                            if (GlobalSettingObj.SettingValue == "1")
                            {
                                string EmailUpdateStatus = string.Empty;
                                if (EmployeeObj.Email != null)
                                {
                                    if (SendEmail(EmployeeObj.Email, NotificationObj.NotificationContent, NotificationObj.NotificationContent, ref EmailUpdateStatus, "Leave Application") == false)
                                    {
                                        Error(EmailUpdateStatus);
                                    }
                                }
                                else
                                {
                                    AddAuditTrail("Unable to send email, as Email Id not available for " + EmployeeObj.EmployeeCode);
                                }
                            }
                            else // Send email OFF
                            {
                                AddAuditTrail("Unable to send Email as email utility is disabled");
                            }
                        }
                        #endregion
                        //ADDED CODE BY SHRADDHA ON 30 JAN 2018 END

                        if (ManualCompOffObj.MySelf == true)
                        {
                            return RedirectToAction("ManualCompOffList");
                        }
                        else
                        {
                            return RedirectToAction("ManualCompOffList", new { IsMySelf = "false" });
                        }

                    }
                    else
                    {
                        Error("Manual Comp Off Entry failed");
                        AddAuditTrail("Manual Comp Off Entry addition failed for ManualCompOffId:" + ManualCompOffObj.ManualCompOffId);
                        PopulateDropDownManualCompOff();
                        return View(ManualCompOffObj);
                    }
                }
                else
                {
                    PopulateDropDownManualCompOff();
                    return View(ManualCompOffObj);
                }
            }
            catch (System.Exception E)
            {
                AddAuditTrail("Error Occured while adding Manual Comp off for ManualCompOffId:" + ManualCompOffObj.ManualCompOffId + " " + E.Message);
                PopulateDropDownManualCompOff();
                return View(ManualCompOffObj);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="IsMySelf"></param>
        /// <returns></returns>
        public ActionResult ManualCompOffList(string IsMySelf = "true")
        {
            #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
            int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            //List<VwManualCompOffList> ManualCompOffList = new List<VwManualCompOffList>();
            List<SP_VwManualCompOffList_Result> ManualCompOffList = new List<SP_VwManualCompOffList_Result>();
            #endregion
            try
            {
                //GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == "Current Financial Year").FirstOrDefault();

                //ABOVE LINE COMMENTED AND BELOW LINE IS ADDED BY SHALAKA ON 13TH DEC 2017
                GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.CurrentFinancialYear).FirstOrDefault();

                // Error
                if (GlobalSettingObj == null)
                {
                    Error("Inconsistent Financial year data");

                    return View(ManualCompOffList);
                }

                DateTime CalanderStartDate = GetFYStartDate(); // DateTime CalanderStartDate = WetosDB.FinancialYears.Where(a => a.FinancialName == GlobalSettingObj.SettingValue).Select(a => a.StartDate).FirstOrDefault();

                if (CalanderStartDate != null)
                {
                    if (IsMySelf == "true")
                    {
                        #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                        //ManualCompOffList = WetosDB.VwManualCompOffLists.Where(a => a.EmployeeId == EmployeeId)
                        //    .Where(a => a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0).OrderByDescending(a => a.FromDate).ToList();

                        //ManualCompOffList = WetosDB.SP_VwManualCompOffList(EmployeeId).Where(a => a.EmployeeId == EmployeeId)
                        //   .Where(a => a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0).OrderByDescending(a => a.FromDate).ToList();

                        ManualCompOffList = WetosDB.SP_VwManualCompOffList(EmployeeId).Where(a => a.EmployeeId == EmployeeId)
                           .Where(a => a.MarkedAsDelete == 0).OrderByDescending(a => a.FromDate).ToList();


                        #endregion
                    }
                    else
                    {
                        // All Employee list
                        #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                        //ManualCompOffList = WetosDB.VwManualCompOffLists
                        //    .Where(a => a.MarkedAsDelete == 0 && a.FromDate >= CalanderStartDate && a.EmployeeId != EmployeeId).OrderByDescending(a => a.FromDate).ToList();

                        ManualCompOffList = WetosDB.SP_VwManualCompOffList(EmployeeId)
                            .Where(a => a.MarkedAsDelete == 0 && a.EmployeeId != EmployeeId).OrderByDescending(a => a.FromDate).ToList();
                        #endregion
                    }
                }

                ViewBag.ForOthers = IsMySelf;
            }
            catch (System.Exception E)
            {
                AddAuditTrail("Error occurred in Manual comp off list open:" + (E.InnerException == null ? string.Empty : E.InnerException.Message));
            }
            return View(ManualCompOffList);
        }

        /// <summary>
        /// ADDED BY SHRADDHA ON 09 SEP 2017
        /// FOR GETTTING DROPDOWN DATE ON MANUAL COMP OFF
        private void PopulateDropDownManualCompOff()
        {
            try
            {
                //CODE FOR DROPDOWN

                #region COMPANY LIST CODE ADDED BY SHRADDHA ON 30 MAR 2018
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //var CompanyObj = WetosDB.Companies.Select(a => new { CompanyId = a.CompanyId, CompanyName = a.CompanyName }).ToList();
                var CompanyName = WetosDB.SP_GetCompanyList(EmployeeId).Select(m => new { Companyid = m.CompanyId, CompanyName = m.CompanyName }).ToList();
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
                #endregion

                ViewBag.CompanyNameList = new SelectList(CompanyName, "CompanyId", "CompanyName").ToList();

                //drop down for branch name list
                var BranchName = new List<Branch>();
                ViewBag.BranchNameList = new SelectList(BranchName, "BranchId", "BranchName").ToList();

                var EmployeeObj = new List<VwActiveEmployee>();
                ViewBag.EmployeeList = new SelectList(EmployeeObj, "EmployeeId", "EmployeeName").ToList();

                //CODE ADDED BY SHRADDHA ON 29 JAN 2018 FOR SHIFT DROPDOWN START
                var Shift = new List<Shift>();
                ViewBag.ShiftList = new SelectList(Shift, "ShiftId", "ShiftName").ToList();
                //CODE ADDED BY SHRADDHA ON 29 JAN 2018 FOR SHIFT DROPDOWN END

                // ADDED BY MSJ 04 OCT 2018 START
                var EWpurpose = WetosDB.DropdownDatas.Where(a => a.TypeId == 24).Select(a => new { ODPurposeId = a.Text, ODPurposeType = a.Text }).ToList();
                ViewBag.ODPurposeList = new SelectList(EWpurpose, "ODPurposeId", "ODPurposeType").ToList();
                // ADDED BY MSJ 04 OCT 2018 END
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );
            }
        }


        /// <summary>
        /// ADDED BY SHRADDHA ON 09 SEP 2017
        /// FOR GETTTING DROPDOWN DATE ON MANUAL COMP OFF
        private void PopulateDropDownManualCompOffEdit(ManualCompOffModel ManualCompOffObj)
        {
            try
            {
                //CODE FOR DROPDOWN
                #region COMPANY LIST CODE ADDED BY SHRADDHA ON 30 MAR 2018
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //var CompanyObj = WetosDB.Companies.Select(a => new { CompanyId = a.CompanyId, CompanyName = a.CompanyName }).ToList();
                var CompanyName = WetosDB.SP_GetCompanyList(EmployeeId).Select(m => new { Companyid = m.CompanyId, CompanyName = m.CompanyName }).ToList();
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
                #endregion

                ViewBag.CompanyNameList = new SelectList(CompanyName, "CompanyId", "CompanyName").ToList();

                //drop down for branch name list

                #region CODE FOR GET BRANCH LIST ADDED BY SHRADDHA ON 30 MAR 2018
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
                //var BranchName = WetosDB.Branches.Where(a => a.BranchId == ManualCompOffObj.BranchId && (a.MarkedAsDelete == null || a.MarkedAsDelete == 0)).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList();
                var BranchName = WetosDB.SP_GetBranchList(EmployeeId).Where(a => a.BranchId == ManualCompOffObj.BranchId).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList(); //.Select(m => new { BranchId = m.BranchId, BranchName = m.BranchName}).ToList();
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
                #endregion

                ViewBag.BranchNameList = new SelectList(BranchName, "BranchId", "BranchName").ToList();




                #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                //var EmployeeObj = WetosDB.VwActiveEmployees.Where(a => a.EmployeeId == ManualCompOffObj.EmployeeId && (a.ActiveFlag == null || a.ActiveFlag == true)).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
                var EmployeeObj = WetosDB.SP_VwActiveEmployee(EmployeeId).Where(a => a.EmployeeId == ManualCompOffObj.EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
                #endregion

                ViewBag.EmployeeList = new SelectList(EmployeeObj, "EmployeeId", "EmployeeName").ToList();

                //CODE ADDED BY SHRADDHA ON 29 JAN 2018 FOR SHIFT DROPDOWN START
                var Shift = WetosDB.Shifts.Where(a => a.Company.CompanyId == ManualCompOffObj.CompanyId && a.BranchId == ManualCompOffObj.BranchId && (a.MarkedAsDelete == null || a.MarkedAsDelete == 0))
                   .Select(a => new { ShiftId = a.ShiftCode, ShiftName = a.ShiftName }).ToList();
                ViewBag.ShiftList = new SelectList(Shift, "ShiftId", "ShiftName").ToList();
                //CODE ADDED BY SHRADDHA ON 29 JAN 2018 FOR SHIFT DROPDOWN END

                // ADDED BY MSJ 04 OCT 2018 START
                var EWpurpose = WetosDB.DropdownDatas.Where(a => a.TypeId == 24).Select(a => new { ODPurposeId = a.Text, ODPurposeType = a.Text }).ToList();
                ViewBag.ODPurposeList = new SelectList(EWpurpose, "ODPurposeId", "ODPurposeType").ToList();
                // ADDED BY MSJ 04 OCT 2018 END
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Login"></param>
        /// <param name="Logout"></param>
        /// <param name="TranDate"></param>
        /// <returns></returns>
        public JsonResult GetExtraHours(DateTime Login, DateTime Logout, DateTime TranDate, int EmpId, bool IsInPunchInNextDay, bool IsOutPunchInNextDay) // ADDED IsInPunchInNextDay AND IsOutPunchInNextDay FLAGS BY SHRADDHA ON 29 JAN 2018
        {
            DailyTransaction DailyTransactionObj = WetosDB.DailyTransactions.Where(a => a.EmployeeId == EmpId && a.TranDate == TranDate).FirstOrDefault();

            List<SP_POSTINGGetRequiredEmployeeDetails_Result> RequiredEmpDetListObj = new List<SP_POSTINGGetRequiredEmployeeDetails_Result>();

            //CODE ADDED BY SHRADDHA ON 29 JAN 2018 START
            DateTime NextDay = TranDate.AddDays(1);
            if (IsInPunchInNextDay == true)
            {
                Login = new DateTime(NextDay.Year, NextDay.Month, NextDay.Day, Login.Hour, Login.Minute, Login.Second);
            }
            else
            {
                Login = new DateTime(TranDate.Year, TranDate.Month, TranDate.Day, Login.Hour, Login.Minute, Login.Second);
            }
            if (IsOutPunchInNextDay == true)
            {
                Logout = new DateTime(NextDay.Year, NextDay.Month, NextDay.Day, Logout.Hour, Logout.Minute, Logout.Second);
            }
            else
            {
                Logout = new DateTime(TranDate.Year, TranDate.Month, TranDate.Day, Logout.Hour, Logout.Minute, Logout.Second);
            }
            //CODE ADDED BY SHRADDHA ON 29 JAN 2018 END


            //DateTime ExtraHours = new DateTime(2017,01,01,00, 00, 00);
            string ExtraHours = "00:00:00";

            List<HoliDay> HolidayList = new List<HoliDay>();

            if (DailyTransactionObj != null)
            {
                string ShiftCode = DailyTransactionObj.ShiftId.Trim();

                Shift ShiftObj = WetosDB.Shifts.Where(a => a.ShiftCode == ShiftCode && a.BranchId == DailyTransactionObj.BranchId && (a.MarkedAsDelete == null || a.MarkedAsDelete != 1)).FirstOrDefault();

                if (ShiftObj != null)
                {
                    DateTime ShiftWrkHours = ShiftObj.WorkingHours;

                    //BUG NO : 1000 START
                    //ADDED BY SHALAKA ON 28TH NOV 2017 --- If Day status is WO or HO then calculate working hrs from login to logout time.
                    if (DailyTransactionObj.Status != null && (DailyTransactionObj.Status.Contains('W') || DailyTransactionObj.Status.Contains('H')))
                    {
                        TimeSpan CalWorkHours = Logout.Subtract(Login);
                        ExtraHours = CalWorkHours.ToString();
                    } //BUG NO : 1000 END RESOLVED
                    else
                    {

                    }
                }
                //BUG NO : 1001 START
                // ADDED BY SHALAKA ON 28TH NOV 2017 GET HOLIDAY LIST FOR THIS DATE.
                //HolidayList = WetosDB.HoliDays.Where(a => a.FromDate == TranDate).ToList(); // ADDED BY MSJ ON 04 OCT 2018

                //if (HolidayList.Count > 0)
                {
                    TimeSpan CalWorkHours = Logout.Subtract(Login);
                    ExtraHours = CalWorkHours.ToString();
                }

                bool IsWeeklyOff = false;

                // EMP INFO
                // ADDED BY SHALAKA ON 29th NOV 2017 
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                RequiredEmpDetListObj = WetosDB.SP_POSTINGGetRequiredEmployeeDetails().ToList();
                var EmployeeObj = RequiredEmpDetListObj.Where(a => a.EmployeeId == EmployeeId).FirstOrDefault();

                //ADDED BY SHALAKA ON 29TH NOV 2017-- Get DayOfWeek from Tran Date
                String CurrentDayOfWeekStr = TranDate.DayOfWeek.ToString();

                //Checking for WeekOff1 
                if (EmployeeObj.WeeklyOff1.ToUpper() == CurrentDayOfWeekStr.ToUpper())
                {
                    IsWeeklyOff = true;
                }
                else if (!string.IsNullOrEmpty(EmployeeObj.WeeklyOff2))
                {
                    //Checking for Weekoff2
                    if (EmployeeObj.WeeklyOff2.ToUpper() == CurrentDayOfWeekStr.ToUpper())
                    {
                        IsWeeklyOff = true;
                    }
                    else
                    {

                    }
                }
                else
                {

                }

                /// Added by Shalaka on 29th NOV 2017
                /// Is WeekOff Is true then get working hrs  
                if (IsWeeklyOff == true)
                {
                    TimeSpan CalWorkHours = Logout.Subtract(Login);
                    ExtraHours = CalWorkHours.ToString();
                }
                //BUG NO : 1001 END RESOLVED
            }
            else
            {
                //BUG NO : 1001 START
                // ADDED BY SHALAKA ON 28TH NOV 2017 GET HOLIDAY LIST FOR THIS DATE.
                HolidayList = WetosDB.HoliDays.Where(a => a.FromDate == TranDate && a.MarkedAsDelete == 0).ToList(); // ADDED BY MSJ ON 27 APR 2020

                if (HolidayList.Count > 0)
                {
                    TimeSpan CalWorkHours = Logout.Subtract(Login);
                    ExtraHours = CalWorkHours.ToString();
                }

                bool IsWeeklyOff = false;

                // EMP INFO
                // ADDED BY SHALAKA ON 29th NOV 2017 
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                RequiredEmpDetListObj = WetosDB.SP_POSTINGGetRequiredEmployeeDetails().ToList();
                var EmployeeObj = RequiredEmpDetListObj.Where(a => a.EmployeeId == EmployeeId).FirstOrDefault();

                //ADDED BY SHALAKA ON 29TH NOV 2017-- Get DayOfWeek from Tran Date
                String CurrentDayOfWeekStr = TranDate.DayOfWeek.ToString();

                //Checking for WeekOff1 
                if (EmployeeObj.WeeklyOff1.ToUpper() == CurrentDayOfWeekStr.ToUpper())
                {
                    IsWeeklyOff = true;
                }
                else if (!string.IsNullOrEmpty(EmployeeObj.WeeklyOff2))
                {
                    //Checking for Weekoff2
                    if (EmployeeObj.WeeklyOff2.ToUpper() == CurrentDayOfWeekStr.ToUpper())
                    {
                        IsWeeklyOff = true;
                    }
                    else
                    {

                    }
                }
                else
                {

                }

                /// Added by Shalaka on 29th NOV 2017
                /// Is WeekOff Is true then get working hrs  
                if (IsWeeklyOff == true)
                {
                    TimeSpan CalWorkHours = Logout.Subtract(Login);
                    ExtraHours = CalWorkHours.ToString();
                }
                //BUG NO : 1001 END RESOLVED
            }

            return Json(ExtraHours, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// CODE ADDED BY SHRADDHA ON 19 SEP 2017
        /// TO CALCULATE MANUAL COMP OFF BALANCE BASED ON WORKING HOURS
        /// TO GET MINIMUM COMP OFF HALF DAY & FULL DAY LIMIT LIMIT
        /// <param name="CompanyId"></param>
        /// <param name="BranchId"></param>
        /// <returns></returns>
        public JsonResult CalculateManualCompOffBalance(int CompanyId, int BranchId, int EmployeeId, string ExtraWorkingHours)
        {
            double CompOffBalance = 0;
            try
            {
                int EmployeeGroupId = WetosDB.EmployeeGroupDetails.Where(a => a.Employee.EmployeeId == EmployeeId).Select(a => a.EmployeeGroup.EmployeeGroupId).FirstOrDefault();
                List<RuleTransaction> RuleTransactionList = WetosDB.RuleTransactions.Where(a => a.CompanyId == CompanyId
                    && a.BranchId == BranchId && a.EmployeeGroupId == EmployeeGroupId).ToList();
                if (RuleTransactionList.Count > 0)
                {
                    RuleTransaction RuleTransactionObj = RuleTransactionList.Where(a => a.RuleId == 9).FirstOrDefault();//RuleId == 9 COMP OFF ALLOWED OR NOT
                    if (RuleTransactionObj != null)
                    {
                        if (RuleTransactionObj.Formula.Trim().ToUpper() == "TRUE")
                        {
                            RuleTransaction RTFullDayCOLimitObj = RuleTransactionList.Where(a => a.RuleId == 10).FirstOrDefault(); //RuleId == 10 - FULL DAY COMP OFF LIMIT
                            
                            RuleTransaction RTHalfDayCOLimitObj = RuleTransactionList.Where(a => a.RuleId == 11).FirstOrDefault(); //RuleId == 11 - HALF DAY COMP OFF LIMIT

                            int FullDayRuleHourInt = 0;
                            int FullDayRuleMinuteInt = 0;
                            //int FullDayRuleSecondInt = 0;

                            int HalfDayRuleHourInt = 0;
                            int HalfDayRuleMinuteInt = 0;
                            //int HalfDayRuleSecondInt = 0;
                            int ExtraWoHoursHourInt = 0;
                            int ExtraWoHoursMinuteInt = 0;
                            //int ExtraWoHoursSecondInt = 0;
                            int ExtraWoHoursTotalTime = 0;
                            int FullDayRuleTotalTime = 0;
                            int HalfDayRuleTotalTime = 0;
                            if (RTFullDayCOLimitObj != null)
                            {
                                string[] FullDayRuleSplitValue = RTFullDayCOLimitObj.Formula.Split(':');
                                FullDayRuleHourInt = Convert.ToInt32(FullDayRuleSplitValue[0]);
                                FullDayRuleMinuteInt = Convert.ToInt32(FullDayRuleSplitValue[1]);
                                //FullDayRuleSecondInt = Convert.ToInt32(FullDayRuleSplitValue[2]);
                            }

                            if (RTHalfDayCOLimitObj != null)
                            {
                                string[] HalfDayRuleSplitValue = RTHalfDayCOLimitObj.Formula.Split(':');
                                HalfDayRuleHourInt = Convert.ToInt32(HalfDayRuleSplitValue[0]);
                                HalfDayRuleMinuteInt = Convert.ToInt32(HalfDayRuleSplitValue[1]);
                                //HalfDayRuleSecondInt = Convert.ToInt32(HalfDayRuleSplitValue[2]);
                            }

                            if (!string.IsNullOrEmpty(ExtraWorkingHours))
                            {
                                string[] ExtraWorkingHoursSplitValue = ExtraWorkingHours.Split(':');
                                ExtraWoHoursHourInt = Convert.ToInt32(ExtraWorkingHoursSplitValue[0]);
                                ExtraWoHoursMinuteInt = Convert.ToInt32(ExtraWorkingHoursSplitValue[1]);
                                //ExtraWoHoursSecondInt = Convert.ToInt32(ExtraWorkingHoursSplitValue[2]);
                            }

                            if (ExtraWoHoursHourInt > 0)
                            {
                                if (FullDayRuleHourInt > 0)
                                {
                                    ExtraWoHoursTotalTime = (60 * ExtraWoHoursHourInt) + ExtraWoHoursMinuteInt;
                                    FullDayRuleTotalTime = (60 * FullDayRuleHourInt) + FullDayRuleMinuteInt;
                                    HalfDayRuleTotalTime = (60 * HalfDayRuleHourInt) + HalfDayRuleMinuteInt;
                                    if (ExtraWoHoursTotalTime >= FullDayRuleTotalTime)
                                    {
                                        CompOffBalance = 1;
                                    }
                                    else if (ExtraWoHoursTotalTime >= HalfDayRuleTotalTime)
                                    {
                                        CompOffBalance = 0.5;
                                    }
                                    else if (ExtraWoHoursTotalTime < HalfDayRuleTotalTime) //FullDayRuleTotalTime)
                                    {
                                        CompOffBalance = 0;
                                    }
                                 
                                }

                            }
                            else
                            {
                                CompOffBalance = 0;
                            }

                        }
                    }
                }
            }
            catch
            {
                CompOffBalance = 0;
            }
            return Json(CompOffBalance, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CheckLoginTimeValidationManualCompOff(string LoginTime, string LogoutTime, DateTime TranDate, bool? IsInPunchInNextDay, bool? IsOutPunchInNextDay) // ADDED IsInPunchInNextDay AND IsOutPunchInNextDay FLAGS BY SHRADDHA ON 29 JAN 2018
        {
            double CompOffBalance = 1;
            try
            {

                int LoginTimeHourInt = 0;
                int LoginTimeMinuteInt = 0;
                int LogoutTimeHourInt = 0;
                int LogoutTimeMinuteInt = 0;
                int LoginTimeTotalTime = 0;
                int LogoutTimeTotalTime = 0;
                if (!string.IsNullOrEmpty(LoginTime))
                {
                    string[] LoginTimeSplitValue = LoginTime.Split(':');
                    LoginTimeHourInt = Convert.ToInt32(LoginTimeSplitValue[0]);
                    LoginTimeMinuteInt = Convert.ToInt32(LoginTimeSplitValue[1]);
                    //LoginTimeSecondInt = Convert.ToInt32(LoginTimeSplitValue[2]);
                }

                if (!string.IsNullOrEmpty(LogoutTime))
                {
                    string[] LogoutTimeSplitValue = LogoutTime.Split(':');
                    LogoutTimeHourInt = Convert.ToInt32(LogoutTimeSplitValue[0]);
                    LogoutTimeMinuteInt = Convert.ToInt32(LogoutTimeSplitValue[1]);
                    //LogoutTimeSecondInt = Convert.ToInt32(LogoutTimeSplitValue[2]);
                }

                //CODE ADDED BY SHRADDHA ON 29 JAN 2018 START
                DateTime NextDay = TranDate.AddDays(1);
                DateTime Login = new DateTime(TranDate.Year, TranDate.Month, TranDate.Day, LoginTimeHourInt, LoginTimeMinuteInt, 00);
                DateTime Logout = new DateTime(TranDate.Year, TranDate.Month, TranDate.Day, LogoutTimeHourInt, LogoutTimeMinuteInt, 00);
                if (IsInPunchInNextDay == true)
                {
                    Login = new DateTime(NextDay.Year, NextDay.Month, NextDay.Day, Login.Hour, Login.Minute, Login.Second);
                }

                if (IsOutPunchInNextDay == true)
                {
                    Logout = new DateTime(NextDay.Year, NextDay.Month, NextDay.Day, Logout.Hour, Logout.Minute, Logout.Second);
                }

                //CODE ADDED BY SHRADDHA ON 29 JAN 2018 END


                LoginTimeTotalTime = (60 * LoginTimeHourInt) + LoginTimeMinuteInt;
                LogoutTimeTotalTime = (60 * LogoutTimeHourInt) + LogoutTimeMinuteInt;

                //COMMENTED EARLIER IF CONDITION AND ADDED NEW IF CONDITION BY SHRADDHA ON 29 JAN 2018 START
                //if (LoginTimeTotalTime > LogoutTimeTotalTime)
                if (Login > Logout)
                {
                    CompOffBalance = 0;
                }
                //COMMENTED EARLIER IF CONDITION AND ADDED NEW IF CONDITION BY SHRADDHA ON 29 JAN 2018 END
            }
            catch
            {
                CompOffBalance = 0;
            }
            return Json(CompOffBalance, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// ODTravelDelete
        /// Updated by Rajas on 9 JUNE 2017
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ManualCompOffDelete(int ManualCompOffId)
        {
            bool MySelf = true;
            try
            {
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                ManualCompOff ManualCompOffObj = WetosDB.ManualCompOffs.Where(a => a.ManualCompOffId == ManualCompOffId).FirstOrDefault();
                if (EmployeeId != ManualCompOffObj.EmployeeId)
                {
                    MySelf = false;
                }
                if (ManualCompOffObj != null)
                {
                    ManualCompOffObj.MarkedAsDelete = 1;

                    // ADDED BY MSJ ON 09 JAN 2019 START
                    ManualCompOffObj.CancelledBy = Convert.ToInt32(Session["EmployeeNo"]);
                    ManualCompOffObj.CancelledOn = DateTime.Now;
                    // ADDED BY MSJ ON 09 JAN 2019 END


                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 7 SEPTEMBER 2017
                    #region ADD AUDIT LOG
                    //OLD RECORD IS BLANK
                    string Oldrecord = "EmployeeId : " + ManualCompOffObj.EmployeeId + ", ExtraWorkingDate : " + ManualCompOffObj.FromDate
                       + ", LoginTime : " + ManualCompOffObj.LoginTime + ", LogoutTime : " + ManualCompOffObj.LogoutTime
                       + ", CompanyId : " + ManualCompOffObj.CompanyId + ", BranchId :" + ManualCompOffObj.BranchId + ", CompOffBalance :"
                       + ManualCompOffObj.CompOffBalance + ", CompOffPurpose :" + ManualCompOffObj.CompOffPurpose + ", Status :" + ManualCompOffObj.StatusId
                       + ", ExtraWorkingHrs :" + ManualCompOffObj.ExtraWorkingHrs;

                    //LOGIN ID IS TAKEN FROM SESSION PERSISTER
                    string Formname = "MANUAL COMPOFF DELETE";
                    //ACTION IS UPDATE
                    string Message = " ";

                    WetosAdministrationController.GenerateAuditLogsInsert(WetosDB, ManualCompOffObj.EmployeeId, Formname, Oldrecord, ref Message);
                    #endregion
                    //AUDIT LOG GENERATION ADDED BY PUSHKAR ON 7 SEPTEMBER 2017

                    WetosDB.SaveChanges();

                    Success("Manual Comp Off application for  " + ManualCompOffObj.FromDate.ToString("dd-MMM-yyyy")
                        + " deleted successfully");

                    AddAuditTrail("Manual Comp Off application for  " + ManualCompOffObj.FromDate.ToString("dd-MMM-yyyy")
                         + " deleted successfully");
                }

                if (MySelf == false)
                {
                    return RedirectToAction("ManualCompOffList", new { IsMySelf = "false" });
                }
                else
                {
                    return RedirectToAction("ManualCompOffList");
                }
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Please try again!");

                if (MySelf == false)
                {
                    return RedirectToAction("ManualCompOffList", new { IsMySelf = "false" });
                }
                else
                {
                    return RedirectToAction("ManualCompOffList");
                }
            }
        }


        public JsonResult CheckManualCompOffIsAllowedOrNot(int EmployeeId, int BranchId, int CompanyId)
        {
            bool RetStat = true;
            string ErrorMessage = string.Empty;
            try
            {
                int EmployeeGroupId = WetosDB.EmployeeGroupDetails.Where(a => a.Employee.EmployeeId == EmployeeId).Select(a => a.EmployeeGroup.EmployeeGroupId).FirstOrDefault();
                RuleTransaction RuleTransactionObj = WetosDB.RuleTransactions.Where(a => a.CompanyId == CompanyId
                    && a.BranchId == BranchId && a.EmployeeGroupId == EmployeeGroupId && a.RuleId == 9).FirstOrDefault();
                if (RuleTransactionObj != null)
                {
                    if (RuleTransactionObj.Formula.Trim().ToUpper() != "TRUE")
                    {
                        ErrorMessage = "Comp off is not allowed for selected employee";
                        //Information("Comp off is not allowed for selected employee");
                        RetStat = false;
                    }
                }
            }
            catch
            {
                RetStat = false;
            }

            return Json(new { RetStat = RetStat, ErrorMessage = ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// ADDED BY SHRADDHA ON 09 SEP 2017
        /// GET
        /// MANUAL COMP OFF SANCTION LIST
        /// <returns></returns>
        public ActionResult ManualCOSanctionIndex(int selectCriteria = 0)
        {
            try
            {
                int EmpNo = Convert.ToInt32(Session["EmployeeNo"]);
                //int Id = Convert.ToInt32(Session["Id"]);

                // Added by Rajas on 9 MAY 2017 START
                // Select Criteria as per dropdown value
                int Status = 0;

                // Updated by Rajas on 20 MAY 2017 START
                List<SP_ManualCOSanctionIndex_Result> ManualCOSanctionList = new List<SP_ManualCOSanctionIndex_Result>();

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
                    return View(ManualCOSanctionList);
                }

                DateTime CalanderStartDate = GetFYStartDate(); // DateTime CalanderStartDate = WetosDB.FinancialYears.Where(a => a.FinancialName == GlobalSettingObj.SettingValue).Select(a => a.StartDate).FirstOrDefault();

                if (CalanderStartDate != null)
                {

                    //CODE REPLACED BY SHRADDHA ON 12 SEP 2017 REMOVED REDUNDANT AND UNNECESSARY IF ELSE CODE AND TAKEN ONE SINGLE LINE CODE START
                    Status = selectCriteria;
                    ManualCOSanctionList = WetosDB.SP_ManualCOSanctionIndex(EmpNo).Where(a => ((a.EmployeeReportingId == EmpNo || a.EmployeeReportingId2 == EmpNo) && a.Id == Status && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null) && a.CompOffBalance > 0) //ADDED COMP OFF BALANCE CONDITION BY SHRADDHA ON 05 MAR 2018
                            .OrderByDescending(a => a.FromDate).OrderByDescending(a => a.FromDate).ToList();
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


                // Added by Rajas on 9 MAY 2017
                // Populate Status dropdown
                //REPLACED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 START
                //List<string> StatusObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 3).Select(a => a.Text).ToList();
                //ViewBag.StatusList = StatusObj;
                PopulateDropDown();
                //REPLACED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 END

                return View(ManualCOSanctionList);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in  Manual CompOff Sanction due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Error in  Manual CompOff Sanction due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );


                //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 START
                PopulateDropDown();
                //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 END

                return RedirectToAction("EmployeeDashboard", "Dashboard");
            }
        }


        /// <summary>
        /// CODE FOR MANUAL COMP OFF SANCTION
        /// ENTIRE CODE MODIFIED ON SHRADDHA ON 20 SEP 2017
        /// <param name="LeaveSanction"></param>
        /// <param name="FC"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ManualCOSanctionIndex(List<SP_ManualCOSanctionIndex_Result> ManualCOSanction, FormCollection FC)
        {
            //TRY BLOCK
            #region TRY BLOCK
            try
            {
                int EmpNo = Convert.ToInt32(Session["EmployeeNo"]);
                string RejectReasonText = FC["RejectReasonText"];

                //FAKE LOGIC USED FOR FILTERING
                #region FAKE LOGIC USED FOR FILTERING
                List<SP_ManualCOSanctionIndex_Result> ManualCOSanctionObjForCount = new List<SP_ManualCOSanctionIndex_Result>();

                ManualCOSanctionObjForCount = ManualCOSanction.Where(a => a.StatusId == 3 || a.StatusId == 5 || a.StatusId == 6).ToList();

                //IF REJECTED BY APPROVER / REJECTED BY SANCTIONER / CANCELLED MANUAL COMP OFF AVAILABLE AND REJECT / CANCEL REASON IS NOT AVAILABLE THEN PROVIDE ERROR MESSAGE
                #region IF REJECTED BY APPROVER / REJECTED BY SANCTIONER / CANCELLED MANUAL COMP OFF AVAILABLE AND REJECT / CANCEL REASON IS NOT AVAILABLE THEN PROVIDE ERROR MESSAGE
                if (ManualCOSanctionObjForCount.Count() > 0 && (RejectReasonText == null || RejectReasonText == ""))
                {
                    ModelState.AddModelError("CustomError", "Please Enter Reject/Cancel Reason");
                    PopulateDropDown();
                    Error("Please enter Reject/Cancel reason");

                    return View(ManualCOSanctionObjForCount); // ODTravelSanctionList
                }
                #endregion

                #endregion

                //REAL LOGIC
                #region REAL LOGIC
                foreach (SP_ManualCOSanctionIndex_Result item in ManualCOSanction)
                {

                    int EmpId = Convert.ToInt32(Session["EmployeeNo"]);

                    ManualCompOff ManualCOObj = WetosDB.ManualCompOffs.Where(a => a.ManualCompOffId == item.ManualCompOffId).FirstOrDefault();
                    //FIND EMPLOYEE IS VALID / ACTIVE OR NOT
                    VwActiveEmployee ValidEmpObj = WetosDB.VwActiveEmployees.Where(a => a.EmployeeId == ManualCOObj.EmployeeId).FirstOrDefault();

                    //IF EMPLOYEE IS VALID
                    #region IF EMPLOYEE IS VALID
                    if (ValidEmpObj != null)
                    {
                        //IF APPLICATION SANCTION STATUS ID IS VALID
                        #region IF APPLICATION SANCTION STATUS ID IS VALID
                        if (item.StatusId != 0)
                        {
                            //IF STATUS IS SANCTION OR (STATUS IS APPROVED BUT APPROVER AND SANCTIONER IS SAME PERSON
                            #region IF STATUS IS SANCTION OR (STATUS IS APPROVED BUT APPROVER AND SANCTIONER IS SAME PERSON
                            if ((item.StatusId == 2) || (item.StatusId == 4 && (ValidEmpObj.EmployeeReportingId == EmpId && ValidEmpObj.EmployeeReportingId2 == EmpId)))
                            {
                                //IF LOGIN EMPLOYEE IS APPROVER OR SANCTIONER
                                #region IF LOGIN EMPLOYEE IS LEAVE APPROVER OR SANCTIONER
                                #region IF LOGIN EMPLOYEE IS LEAVE APPROVER OR SANCTIONER
                                if (ValidEmpObj.EmployeeReportingId2 == EmpId || ValidEmpObj.EmployeeReportingId == EmpId)
                                {
                                    //UPDATE MANUAL COMP OFF DATA INTO MANUAL COMP OFF TABLE IN CASE OF SANCTIONING MANUAL COMP OFF
                                    #region UPDATE MANUAL COMP OFF DATA INTO MANUAL COMP OFF TABLE IN CASE OF SANCTIONING MANUAL COMP OFF
                                    item.StatusId = 2;
                                    ManualCOObj.StatusId = item.StatusId;
                                    ManualCOObj.RejectReason = string.Empty;
                                    WetosDB.SaveChanges();
                                    #endregion

                                    //CODE TO ADD MANUAL COMP OFF ENTRY INTO COMP OFF TABLE IN CASE OF SANCTIONING MANUAL COMP OFF
                                    #region CODE TO ADD MANUAL COMP OFF ENTRY INTO COMP OFF TABLE IN CASE OF SANCTIONING MANUAL COMP OFF
                                    CompOff NewCompOffObj = new CompOff();
                                    DailyTransaction DailyTransactionObj = WetosDB.DailyTransactions.Where(a => a.TranDate == ManualCOObj.FromDate && a.EmployeeId == ManualCOObj.EmployeeId).FirstOrDefault();

                                    //IF ATTENDANCE DATA IS AVAILABLE FOR COMP OFF DATE THEN MARK SHIFT FROM DT TABLE
                                    #region IF ATTENDANCE DATA IS AVAILABLE FOR COMP OFF DATE THEN MARK SHIFT FROM DT TABLE
                                    if (DailyTransactionObj != null)
                                    {
                                        NewCompOffObj.ShiftId = DailyTransactionObj.ShiftId;
                                    }
                                    #endregion

                                    //IF ATTENDANCE DATA IS NOT AVAILABLE FOR COMP OFF DATE THEN MARK GEN SHIFT
                                    #region IF ATTENDANCE DATA IS NOT AVAILABLE FOR COMP OFF DATE THEN MARK GEN SHIFT
                                    else
                                    {
                                        NewCompOffObj.ShiftId = "GEN";  // Verify??
                                    }
                                    #endregion

                                    NewCompOffObj.EmployeeId = Convert.ToInt32(ManualCOObj.EmployeeId);
                                    NewCompOffObj.WoDate = ManualCOObj.FromDate;
                                    //IF COMP OFF BALANCE IS 0.5 THEN ADD DAYSTATUS IN COMP OFF TABLE IS 'Half Day'
                                    #region IF COMP OFF BALANCE IS 0.5 THEN ADD DAYSTATUS IN COMP OFF TABLE IS 'Half Day'
                                    //if (ManualCOObj.CompOffBalance == 0.5)
                                    if (item.CompOffBalance == 0.5)
                                    {
                                        NewCompOffObj.DayStatus = "Half Day";   // Mapp with Id ?
                                        NewCompOffObj.CoHours = ManualCOObj.ExtraWorkingHrs;
                                    }
                                    #endregion

                                    //IF COMP OFF BALANCE IS 1 THEN ADD DAYSTATUS IN COMP OFF TABLE IS 'Full Day'
                                    #region IF COMP OFF BALANCE IS 1 THEN ADD DAYSTATUS IN COMP OFF TABLE IS 'Full Day'
                                    //else if (ManualCOObj.CompOffBalance == 1)
                                    else if (item.CompOffBalance == 1)
                                    {
                                        NewCompOffObj.DayStatus = "Full Day";
                                        NewCompOffObj.CoHours = ManualCOObj.ExtraWorkingHrs;
                                    }

                                    //CODE ADDED BY SHRADDHA ON 20 FEB 2018 START
                                    else
                                    {
                                        NewCompOffObj.CoHours = ManualCOObj.CompOffHours;
                                    }
                                    //CODE ADDED BY SHRADDHA ON 20 FEB 2018 END
                                    #endregion

                                    NewCompOffObj.BalanceCoHours = NewCompOffObj.CoHours; //CODE ADDED BY SHRADDHA ON 21 FEB 2018

                                    NewCompOffObj.CompanyId = ManualCOObj.CompanyId;
                                    NewCompOffObj.BranchId = ManualCOObj.BranchId;


                                    // Updated by Rajas on 29 SEP 2017 START
                                    //DateTime.Now modified
                                    int CurrentYearInt = DateTime.Now.Year; // ADDED BY MSJ ON 02 JAN 2018
                                    NewCompOffObj.LoginTime = ManualCOObj.LoginTime == null ? new DateTime(CurrentYearInt, 01, 01, 00, 00, 00) : ManualCOObj.LoginTime.Value;
                                    NewCompOffObj.LogOutTime = ManualCOObj.LogoutTime == null ? new DateTime(CurrentYearInt, 01, 01, 00, 00, 00) : ManualCOObj.LogoutTime.Value;
                                    // Updated by Rajas on 29 SEP 2017 END

                                    NewCompOffObj.ManualCompOffId = ManualCOObj.ManualCompOffId;

                                    WetosDB.CompOffs.AddObject(NewCompOffObj);
                                    WetosDB.SaveChanges();
                                    #endregion
                                }
                                #endregion

                                #endregion

                                //IF LOGIN EMPLOYEE IS NOT APPROVER OR SANCTIONER THEN PROVIDE ERROR MESSAGE
                                #region IF LOGIN EMPLOYEE IS NOT LEAVE APPROVER OR SANCTIONER THEN PROVIDE ERROR MESSAGE
                                else
                                {
                                    Information("You can not sanction this Manual CompOff as you are not sanctioner for selected employee");
                                    List<SP_ManualCOSanctionIndex_Result> ODTObj = WetosDB.SP_ManualCOSanctionIndex(EmpNo).OrderByDescending(a => a.FromDate).ToList();
                                    PopulateDropDown();

                                    return View(ODTObj);
                                }
                                #endregion
                            }
                            #endregion

                            //IF APPLICATION SANCTION IS CANCEL/ REJECTED BY APPROVER / REJECTED BY SANCTIONER
                            #region IF APPLICATION SANCTION IS CANCEL/ REJECTED BY APPROVER / REJECTED BY SANCTIONER
                            else if (item.StatusId == 5 || item.StatusId == 3 || item.StatusId == 6)   // Added by Rajas on 9 JULY 2017 to Fix Issue no. 2, defect id= FB0012 as per Test Cases sheet
                            {
                                //IF REJECT REASON IS EMPTY THEN PROVIDE ERROR MESSAGE
                                #region IF REJECT REASON IS EMPTY THEN PROVIDE ERROR MESSAGE
                                if (RejectReasonText == null || RejectReasonText == string.Empty)
                                {
                                    ModelState.AddModelError("CustomError", "Please Enter reject/cancellation Reason");
                                    Error("Please enter reject/cancellation reason");
                                    PopulateDropDown();
                                    return View(ManualCOSanction);
                                }
                                #endregion

                                //IF REJECT REASON IS NOT EMPTY 
                                #region IF REJECT REASON IS NOT EMPTY
                                else
                                {
                                    //CHECK WHETHER COMP OFF ENTRY IS AVAIALBLE IN COMP OFF TABLE OR NOT
                                    CompOff CompOffObj = WetosDB.CompOffs.Where(a => a.ManualCompOffId == ManualCOObj.ManualCompOffId).FirstOrDefault();
                                    //IF COMP OFF ENTRY IS AVAIALBLE IN COMP OFF TABLE
                                    #region IF COMP OFF ENTRY IS AVAIALBLE IN COMP OFF TABLE
                                    if (CompOffObj != null)
                                    {
                                        //IF COMP OFF IS USED (PENDING/SANCTIONED) AGAINST SELECTED COMP OFF WHICH ENTRY WE ARE TRYING TO DELETE THEN PROVIDE ERROR MESSAGE
                                        #region IF COMP OFF IS USED (PENDING/SANCTIONED) AGAINST SELECTED COMP OFF WHICH ENTRY WE ARE TRYING TO DELETE THEN PROVIDE ERROR MESSAG
                                        if (CompOffObj.AppStatus != null)
                                        {
                                            if (CompOffObj.AppStatus.Trim() == "P" || CompOffObj.AppStatus.Trim() == "S")
                                            {
                                                Error("You are not allowed to reject/Cancel comp off for date:" + CompOffObj.WoDate + " and Employee:" + ValidEmpObj.FirstName + " " + ValidEmpObj.LastName + "<br/>" + " Selected Comp off is already used.");
                                                PopulateDropDown();
                                                return View(ManualCOSanction);
                                            }
                                        }
                                        #endregion

                                        //IF COMP OFF IS NOT USED (PENDING/SANCTIONED) AGAINST SELECTED COMP OFF THEN WE CAN REJECT / CANCEL SUCH MANUAL COMP OFF
                                        #region IF COMP OFF IS NOT USED (PENDING/SANCTIONED) AGAINST SELECTED COMP OFF THEN WE CAN REJECT / CANCEL SUCH MANUAL COMP OFF
                                        else
                                        {
                                            //IF COMP OFF IS REJECTED BY APPROVER/ REJECTED BY SANCTIONER THEN UPDATE COMP OFF APPSTATUS IN COMP OFF TABLE - RM
                                            #region IF COMP OFF IS REJECTED BY APPROVER/ REJECTED BY SANCTIONER THEN UPDATE COMP OFF APPSTATUS IN COMP OFF TABLE - RM
                                            if (item.StatusId == 3 || item.StatusId == 6)
                                            {
                                                CompOffObj.AppStatus = "RM";
                                            }
                                            #endregion

                                            //IF COMP OFF IS CANCELLED BY SANCTIONER THEN UPDATE COMP OFF APPSTATUS IN COMP OFF TABLE - CM
                                            #region IF COMP OFF IS CANCELLED BY SANCTIONER THEN UPDATE COMP OFF APPSTATUS IN COMP OFF TABLE - CM
                                            if (item.StatusId == 5)
                                            {
                                                CompOffObj.AppStatus = "CM";
                                            }
                                            #endregion

                                            WetosDB.SaveChanges();

                                            //UPDATE MANUAL COMP OFF TABLE ENTRY IN CASE OF SUCCESSFULL REJECTION / CANCELLATION OF MANUAL COMP OFF
                                            #region UPDATE MANUAL COMP OFF TABLE ENTRY IN CASE OF SUCCESSFULL REJECTION / CANCELLATION OF MANUAL COMP OFF
                                            ManualCOObj.RejectReason = RejectReasonText;
                                            ManualCOObj.StatusId = item.StatusId;
                                            WetosDB.SaveChanges();
                                            #endregion
                                        }
                                        #endregion
                                    }
                                    #endregion

                                    //IF COMP OFF ENTRY IS NOT AVAIALBLE IN COMP OFF TABLE
                                    #region IF COMP OFF ENTRY IS NOT AVAIALBLE IN COMP OFF TABLE
                                    else
                                    {
                                        //UPDATE MANUAL COMP OFF TABLE ENTRY IN CASE OF SUCCESSFULL REJECTION / CANCELLATION OF MANUAL COMP OFF
                                        #region UPDATE MANUAL COMP OFF TABLE ENTRY IN CASE OF SUCCESSFULL REJECTION / CANCELLATION OF MANUAL COMP OFF
                                        ManualCOObj.RejectReason = RejectReasonText;
                                        ManualCOObj.StatusId = item.StatusId;
                                        WetosDB.SaveChanges();
                                        #endregion
                                    }
                                    #endregion
                                }
                                #endregion
                            }
                            #endregion

                            //HANDLE MANUAL COMP OFF VALID CASE DEFAULT TRANSACTION
                            #region HANDLE MANUAL COMP OFF VALID CASE DEFAULT TRANSACTION
                            else
                            {
                                ManualCOObj.StatusId = item.StatusId;
                                if (ManualCOObj.StatusId == 2 || ManualCOObj.StatusId == 4)
                                {
                                    ManualCOObj.RejectReason = string.Empty;
                                }
                                else
                                {
                                    ManualCOObj.RejectReason = RejectReasonText;
                                }
                                WetosDB.SaveChanges();
                            }
                            #endregion
                        }
                        #endregion

                        WetosDB.SaveChanges();

                        //SEND NOTIFICATION
                        #region MANUAL COMP OFF SANCTION NOTIFICATION
                        //SEND NOTIFICATION TO EMPLOYEE WHOM MANUAL COMP OFF IS PROCESSED
                        #region SEND NOTIFICATION TO EMPLOYEE WHOM MANUAL COMP OFF IS PROCESSED
                        Notification NotificationObj = new Notification();
                        NotificationObj.FromID = ValidEmpObj.EmployeeReportingId;
                        NotificationObj.ToID = ValidEmpObj.EmployeeId;
                        NotificationObj.SendDate = DateTime.Now;
                        string StatusName = WetosDB.DropdownDatas.Where(a => a.TypeId == 3 && a.Value == item.StatusId).Select(a => a.Text).FirstOrDefault();
                        NotificationObj.NotificationContent = "Your Manual CO application from " + ManualCOObj.FromDate.ToString("dd-MMM-yyyy") + " is " + StatusName;
                        NotificationObj.ReadFlag = false;
                        NotificationObj.SendDate = DateTime.Now;

                        WetosDB.Notifications.AddObject(NotificationObj);
                        WetosDB.SaveChanges();
                        #endregion


                        //SEND NOTIFICATION TO SANCTIONER IF MANUAL COMP OFF IS APPROVED BY APPROVER IN CASE OF APPROVER AND SANCTIONER ARE BOTH DIFFERENT PERSON
                        #region SEND NOTIFICATION TO SANCTIONER IF MANUAL COMP OFF IS APPROVED BY APPROVER IN CASE OF APPROVER AND SANCTIONER ARE BOTH DIFFERENT PERSON
                        if (ManualCOObj.StatusId == 4 && ValidEmpObj.EmployeeReportingId == EmpNo)
                        {
                            Notification NotificationObj2 = new Notification();
                            NotificationObj.FromID = ValidEmpObj.EmployeeId;
                            NotificationObj.ToID = ValidEmpObj.EmployeeReportingId2;
                            NotificationObj.SendDate = DateTime.Now;
                            NotificationObj.NotificationContent = "Received Manual CO application for sanctioning from " + ValidEmpObj.FirstName + " " + ValidEmpObj.LastName + " - from " + ManualCOObj.FromDate.ToString("dd-MMM-yyyy");
                            NotificationObj.ReadFlag = false;
                            NotificationObj.SendDate = DateTime.Now;

                            WetosDB.Notifications.AddObject(NotificationObj2);
                            WetosDB.SaveChanges();

                        }
                        #endregion

                        int NoTiCount = WetosDB.Notifications.Where(a => a.ToID == EmpId && (a.ReadFlag == false || a.ReadFlag == null)).Count();
                        Session["NotificationCount"] = NoTiCount;

                        #endregion

                        //SEND EMAIL
                        #region SEND EMAIL
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
                                    if (SendEmail(ValidEmpObj.Email, NotificationObj.NotificationContent, NotificationObj.NotificationContent, ref EmailUpdateStatus, "Manual CompOff " + ManualCOObj.StatusId) == false)
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
                    #endregion

                    //IF EMPLOYEE IS NOT VALID
                    #region IF EMPLOYEE IS NOT VALID
                    else
                    {
                        Error("Inconsistent data, Please try again!!");
                        PopulateDropDown();
                        return View(ManualCOSanction);
                    }
                    #endregion
                }
                #endregion

                //RETURN TO VIEW WITH SUCCESS MESSAGE
                #region RETURN TO VIEW WITH SUCCESS MESSAGE
                List<SP_ManualCOSanctionIndex_Result> ManualCONewObj = WetosDB.SP_ManualCOSanctionIndex(EmpNo).OrderByDescending(a => a.FromDate).ToList();
                AddAuditTrail("Success - Manual CompOff application processed successfully");  // Updated by Rajas on 17 JAN 2017
                Success("Success - Manual CompOff application processed successfully");
                PopulateDropDown();
                return View(ManualCONewObj);
                #endregion
            }
            #endregion

            //CATCH BLOCK TO HANDLE RUNTIME EXCEPTION
            #region CATCH BLOCK TO HANDLE RUNTIME EXCEPTION
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in Manual Comp Off Sanction due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );
                Error("Please try again!!");
                PopulateDropDown();

                return View(ManualCOSanction);
            }
            #endregion
        }


        /// <summary>
        /// Added by Rajas on 19 MAY 2017
        /// </summary>
        public void PopulateDropDown()
        {
            // Added by Rajas on 9 MAY 2017
            // Populate Status dropdown
            var StatusObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 3).Select(a => new { Value = a.Value, Text = a.Text }).ToList();
            ViewBag.StatusList = new SelectList(StatusObj, "Value", "Text").ToList();

            //List<string> StatusObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 3).Select(a => a.Text).ToList();
            //ViewBag.StatusList = StatusObj;
        }


        /// <summary>
        /// SEND MAIL
        /// Added by Rajas on 31 MARCH 2017
        /// </summary>
        /// <param name="ToEmail"></param>
        /// <param name="Subject"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        /// Updated by Rajas on 19 JULY 2017 for EmailFromWhichApplication
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

                msg.Subject = Subject; // "Contact Us";

                msg.IsBodyHtml = false;

                msg.From = new MailAddress(SMTPUsername); //"info@foodpatronservices.com");

                smtp.Host = SMTPServerName; // "smtpout.asia.secureserver.net";

                smtp.EnableSsl = true;

                //smtp.Credentials = new System.Net.NetworkCredential("info@foodpatronservices.com", "info@fps");
                smtp.Credentials = new System.Net.NetworkCredential(SMTPUsername, SMTPPassword);//"info@foodpatronservices.com", "info@fps");

                smtp.Port = SMTPPortInt; // 25;

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
        /// ADDED BY SHRADDHA ON 09 SEP 2017
        /// TO GET MANUAL CO SANCTION PV
        /// <param name="selectCriteria"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ManualCOSanctionPV(int selectCriteria = 0)
        {
            try
            {
                int EmpNo = Convert.ToInt32(Session["EmployeeNo"]);
                int Id = Convert.ToInt32(Session["Id"]);

                // Added by Rajas on 9 MAY 2017 START
                // Select Criteria as per dropdown value
                int Status = 0;

                // Updated by Rajas on 20 MAY 2017 START
                List<SP_ManualCOSanctionIndex_Result> ManualCOSanctionList = new List<SP_ManualCOSanctionIndex_Result>();

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

                    return View(ManualCOSanctionList);
                }

                DateTime CalanderStartDate = GetFYStartDate(); // DateTime CalanderStartDate = WetosDB.FinancialYears.Where(a => a.FinancialName == GlobalSettingObj.SettingValue).Select(a => a.StartDate).FirstOrDefault();

                if (CalanderStartDate != null)
                {
                    //CODE REPLACED BY SHRADDHA ON 12 SEP 2017 REMOVED REDUNDANT AND UNNECESSARY IF ELSE CODE AND TAKEN ONE SINGLE LINE CODE START
                    //Status = selectCriteria;
                    //ManualCOSanctionList = WetosDB.SP_ManualCOSanctionIndex(EmpNo).Where(a => ((a.EmployeeReportingId == EmpNo) && a.Id == Status && a.FromDate >= CalanderStartDate && (a.MarkedAsDelete == 0 || a.MarkedAsDelete == null)))
                    //        .OrderByDescending(a => a.FromDate).ToList();
                    //CODE REPLACED BY SHRADDHA ON 12 SEP 2017 REMOVED REDUNDANT AND UNNECESSARY IF ELSE CODE AND TAKEN ONE SINGLE LINE CODE END

                    // Above code commented. Error was list for reportingId2 was not displayed
                    // Code updated by Rajas on 22 SEP 2017 START
                    if (selectCriteria == 1)  // Pending
                    {
                        Status = 1;

                        // Updated by Rajas on 7 JUNE 2017, ( || a.EmployeeReportingId2 == EmpNo) removed
                        //ManualCOSanctionList = WetosDB.SP_ManualCOSanctionIndex(EmpNo).Where(a => ((a.EmployeeReportingId == EmpNo) && a.Id == Status
                        //    && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null) && a.CompOffBalance > 0) //ADDED COMP OFF BALANCE CONDITION BY SHRADDHA ON 05 MAR 2018
                        //    .OrderByDescending(a => a.FromDate).ToList();// && a.Id == 4)

                        //ABOVE CODE COMMENTED AND BELOW CODE MODIFIED AND ADDED BY PUSHKAR ON 13 APRIL 2018 FOR SEEING PREVIOUS YEAR COMPOFF
                        ManualCOSanctionList = WetosDB.SP_ManualCOSanctionIndex(EmpNo).Where(a => ((a.EmployeeReportingId == EmpNo) && a.Id == Status
                           && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null) && a.CompOffBalance > 0) //ADDED COMP OFF BALANCE CONDITION BY SHRADDHA ON 05 MAR 2018
                           .OrderByDescending(a => a.FromDate).ToList();
                    }
                    else if (selectCriteria == 2)  // Sanction
                    {
                        Status = 2;

                        //ManualCOSanctionList = WetosDB.SP_ManualCOSanctionIndex(EmpNo)
                        //    .Where(a => ((a.EmployeeReportingId == EmpNo || a.EmployeeReportingId2 == EmpNo) && a.Id == Status && a.FromDate >= CalanderStartDate
                        //        && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null) && a.CompOffBalance > 0) //ADDED COMP OFF BALANCE CONDITION BY SHRADDHA ON 05 MAR 2018
                        //    .OrderByDescending(a => a.FromDate).ToList();  // && a.Id == 4)

                        //ABOVE CODE COMMENTED AND BELOW CODE MODIFIED AND ADDED BY PUSHKAR ON 13 APRIL 2018 FOR SEEING PREVIOUS YEAR COMPOFF
                        ManualCOSanctionList = WetosDB.SP_ManualCOSanctionIndex(EmpNo)
                            .Where(a => ((a.EmployeeReportingId == EmpNo || a.EmployeeReportingId2 == EmpNo) && a.Id == Status
                                && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null) && a.CompOffBalance > 0) //ADDED COMP OFF BALANCE CONDITION BY SHRADDHA ON 05 MAR 2018
                            .OrderByDescending(a => a.FromDate).ToList();
                    }
                    else if (selectCriteria == 3) // Rejected by Approver
                    {
                        Status = 3;

                        //ManualCOSanctionList = WetosDB.SP_ManualCOSanctionIndex(EmpNo)
                        //    .Where(a => ((a.EmployeeReportingId == EmpNo || a.EmployeeReportingId2 == EmpNo)
                        //        && a.Id == Status && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null) || (a.EmployeeReportingId2 == EmpNo
                        //        && a.Id == Status && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null) && a.CompOffBalance > 0) //ADDED COMP OFF BALANCE CONDITION BY SHRADDHA ON 05 MAR 2018
                        //        .OrderByDescending(a => a.FromDate).ToList();  // && a.Id == 4)

                        //ABOVE CODE COMMENTED AND BELOW CODE MODIFIED AND ADDED BY PUSHKAR ON 13 APRIL 2018 FOR SEEING PREVIOUS YEAR COMPOFF
                        ManualCOSanctionList = WetosDB.SP_ManualCOSanctionIndex(EmpNo)
                            .Where(a => ((a.EmployeeReportingId == EmpNo || a.EmployeeReportingId2 == EmpNo)
                                && a.Id == Status && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null) || (a.EmployeeReportingId2 == EmpNo
                                && a.Id == Status && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null) && a.CompOffBalance > 0) //ADDED COMP OFF BALANCE CONDITION BY SHRADDHA ON 05 MAR 2018
                                .OrderByDescending(a => a.FromDate).ToList();
                    }
                    else if (selectCriteria == 4)  // Approve
                    {
                        Status = 4;

                        //ManualCOSanctionList = WetosDB.SP_ManualCOSanctionIndex(EmpNo)
                        //    .Where(a => ((a.EmployeeReportingId2 == EmpNo)
                        //        && a.Id == Status && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null) && a.CompOffBalance > 0) //ADDED COMP OFF BALANCE CONDITION BY SHRADDHA ON 05 MAR 2018
                        //        .OrderByDescending(a => a.FromDate).ToList();  // && a.Id == 4)

                        //ABOVE CODE COMMENTED AND BELOW CODE MODIFIED AND ADDED BY PUSHKAR ON 13 APRIL 2018 FOR SEEING PREVIOUS YEAR COMPOFF
                        ManualCOSanctionList = WetosDB.SP_ManualCOSanctionIndex(EmpNo)
                            .Where(a => ((a.EmployeeReportingId2 == EmpNo)
                                && a.Id == Status && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null) && a.CompOffBalance > 0) //ADDED COMP OFF BALANCE CONDITION BY SHRADDHA ON 05 MAR 2018
                                .OrderByDescending(a => a.FromDate).ToList();
                    }
                    else if (selectCriteria == 5)  // Cancel
                    {
                        Status = 5;

                        //ManualCOSanctionList = WetosDB.SP_ManualCOSanctionIndex(EmpNo)
                        //    .Where(a => ((a.EmployeeReportingId == EmpNo || a.EmployeeReportingId2 == EmpNo) && a.Id == Status
                        //        && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null) && a.CompOffBalance > 0) //ADDED COMP OFF BALANCE CONDITION BY SHRADDHA ON 05 MAR 2018
                        //        .OrderByDescending(a => a.FromDate).ToList();  // && a.Id == 4)

                        //ABOVE CODE COMMENTED AND BELOW CODE MODIFIED AND ADDED BY PUSHKAR ON 13 APRIL 2018 FOR SEEING PREVIOUS YEAR COMPOFF
                        ManualCOSanctionList = WetosDB.SP_ManualCOSanctionIndex(EmpNo)
                            .Where(a => ((a.EmployeeReportingId == EmpNo || a.EmployeeReportingId2 == EmpNo) && a.Id == Status
                                && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null) && a.CompOffBalance > 0) //ADDED COMP OFF BALANCE CONDITION BY SHRADDHA ON 05 MAR 2018
                                .OrderByDescending(a => a.FromDate).ToList();
                    }
                    else  // Rejected by Sanctioner
                    {
                        Status = 6;
                        //ManualCOSanctionList = WetosDB.SP_ManualCOSanctionIndex(EmpNo)
                        //    .Where(a => ((a.EmployeeReportingId2 == EmpNo) && a.Id == Status
                        //        && a.FromDate >= CalanderStartDate && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null) && a.CompOffBalance > 0) //ADDED COMP OFF BALANCE CONDITION BY SHRADDHA ON 05 MAR 2018
                        //    .OrderByDescending(a => a.FromDate).ToList();  // && a.Id == 4)

                        //ABOVE CODE COMMENTED AND BELOW CODE MODIFIED AND ADDED BY PUSHKAR ON 13 APRIL 2018 FOR SEEING PREVIOUS YEAR COMPOFF
                        ManualCOSanctionList = WetosDB.SP_ManualCOSanctionIndex(EmpNo)
                            .Where(a => ((a.EmployeeReportingId2 == EmpNo) && a.Id == Status
                                && a.MarkedAsDelete == 0 || a.MarkedAsDelete == null) && a.CompOffBalance > 0) //ADDED COMP OFF BALANCE CONDITION BY SHRADDHA ON 05 MAR 2018
                            .OrderByDescending(a => a.FromDate).ToList();
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
                List<SP_VwActiveEmployee_Result> EmployeeObj = WetosDB.SP_VwActiveEmployee(EmployeeId).ToList();
                #endregion

                PopulateDropDown();
                return PartialView(ManualCOSanctionList);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in Manual CompOff Sanction due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Error in  Manual CompOff Sanction due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 START
                PopulateDropDown();
                //ADDED POPULATEDROPDOWN() FUNCTION BY SHRADDHA ON 12 SEP 2017 END

                return RedirectToAction("EmployeeDashboard", "Dashboard");
            }
        }


        /// <summary>
        /// Json return for to get Employee dropdown list on basis of Branch selection
        /// Added by Rajas on 27 DEC 2016
        /// </summary>
        /// <returns></returns>
        public JsonResult GetEmployee(string Branchid)
        {
            int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            int SelBranchId = 0;
            if (!string.IsNullOrEmpty(Branchid))
            {
                if (Branchid.ToUpper() != "NULL")
                {
                    SelBranchId = Convert.ToInt32(Branchid);
                }
            }

            DateTime Leavingdate = Convert.ToDateTime("01/01/1900");  // Added by Rajas on 10 MARCH 2017

            // Updated by Rajas to pass active employee list, on 10 MARCH 2017

            #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
            int BranchAdmin = Convert.ToInt32(Session["BranchAdmin"]);
            //int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);

            var EmployeeCodeAndNameObj = WetosDB.SP_VwActiveEmployee(EmployeeId).Where(a => a.EmployeeReportingId == EmployeeId || a.EmployeeReportingId2 == EmployeeId)
                    .Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
            //var EmployeeCodeAndNameObj = WetosDB.VwActiveEmployees.Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
            if (BranchAdmin > 0)
            {
                EmployeeCodeAndNameObj = WetosDB.SP_VwActiveEmployee(EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
            }

            #endregion

            return Json(EmployeeCodeAndNameObj);
        }
    }
}
