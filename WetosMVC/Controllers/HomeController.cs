using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WETOS.Util;
using WetosDB;
using WetosMVCMainApp.Models;

namespace WetosMVC.Controllers
{
    [System.Web.Mvc.OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    [Authorize]
    [SessionExpire]
    //[SessionTimeout]
    public class HomeController : BaseController
    {


        [Authorize]
        public ActionResult Index()
        {
            // Check whether data is consistent or not on click of Home button
            // Added by Rajas on 10 JULY 2017 START
            string UpdateStatus = string.Empty;
            int ErrorNumber = 0;

            // Modified by Shalaka on 17th July 2017 --Start
            int CompanyListCount = 0;
            int BranchListCount = 0;
            int DepartmentListCount = 0;
            int DesignationListCount = 0;
            int GradeListCount = 0;
            int EmployeeListCount = 0;
            int EmployeeGroupListCount = 0;
            int LeaveMasterListCount = 0;
            int LeaveBalanceListCount = 0;
            int LeaveCreditListCount = 0;
            int UserListCount = 0;
            int ShiftCount = 0;
            int FinancialYearCount = 0;
            int UserRoleCount = 0;
            int RuleEngineDataCount = 0; // CODE ADDED BY SHRADDHA ON 08 FEB 2018

            try
            {
                if (AccountController.IsDataConsistent(WetosDB, ref UpdateStatus, ref ErrorNumber,
                    ref CompanyListCount, ref BranchListCount, ref DepartmentListCount, ref DesignationListCount, ref GradeListCount, ref EmployeeListCount, ref EmployeeGroupListCount,
                    ref LeaveMasterListCount, ref LeaveBalanceListCount, ref LeaveCreditListCount, ref UserListCount, ref ShiftCount, ref FinancialYearCount, ref UserRoleCount, ref RuleEngineDataCount) == true)
                {
                    List<sp_get_user_role_menu_Result> ri = WetosDB.sp_get_user_role_menu(SessionPersister.UserInfo.UserId, SessionPersister.YearInfo.y_id).ToList();
                    SessionPersister.roleInfo = ri;



                    return RedirectToAction("Index", "Dashboard");
                    // Added by Rajas on 10 JULY 2017 END
                }
                else
                {
                    Error(UpdateStatus);

                    return RedirectToAction("ImportData", "WetosAdministration");
                }

            }
            catch (System.Exception ex)
            {
                // Updated by Rajas on 30 MARCH 2017 START
                AddAuditTrail("Error in Home/Index due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                Error("" + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));
                // Updated by Rajas on 30 MARCH 2017 END

                return View();
            }
        }



        [HttpPost]
        public JsonResult GetTimetable(string start)
        {
            string[] parsestring = start.Split('-');
            int year = Convert.ToInt32(ConvertDigits(parsestring[0]));
            int month = Convert.ToInt32(ConvertDigits(parsestring[1]));
            int date = Convert.ToInt32(ConvertDigits(parsestring[2]));
            DateTime start2 = new DateTime(year, month, date);

            DateTime ToDate = start2.AddDays(42);
            int EmpId = Convert.ToInt32(Session["EmployeeNo"]);

            var ApptListForDate = CalendarEvent.LoadAllAppointmentsInDateRange(EmpId, start2, ToDate);
            //var eventList = GetEvents();
            var eventList = from e in ApptListForDate
                            select new
                            {
                                id = e.ID,
                                title = e.Title,
                                start = e.StartDateString,
                                end = e.EndDateString,
                                color = e.StatusColor,
                                //className = e.ClassName,
                                //someKey = e.SomeImportantKeyID,
                                allDay = true
                            };
            var rows = eventList.ToArray();
            return Json(rows, JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        public JsonResult GetTimeTableForSelectedEmployee(DateTime start, int EmployeeIdOfSelectedEmployee)
        {
            DateTime ToDate = start.AddDays(42);


            var ApptListForDate = CalendarEvent.LoadAllAppointmentsInDateRange(EmployeeIdOfSelectedEmployee, start, ToDate);
            //var eventList = GetEvents();
            var eventList = from e in ApptListForDate
                            select new
                            {
                                id = e.ID,
                                title = e.Title,
                                start = e.StartDateString,
                                end = e.EndDateString,
                                color = e.StatusColor,
                                //className = e.ClassName,
                                //someKey = e.SomeImportantKeyID,
                                allDay = true
                            };
            var rows = eventList.ToArray();
            return Json(rows, JsonRequestBehavior.AllowGet);

        }

        public ActionResult About()
        {
            return View();
        }

        /// <summary>
        /// ADDED TEST CODE FOR POPUP BY SHRADDHA ON 19 JAN 2017 
        /// CODE TO CREATE A MAIN PAGE ON WHICH POPUP WILL BE LOADED.
        /// <returns></returns>
        public ActionResult PopupTestCode()
        {

            return View();
        }

        public ActionResult PhotoCapture()
        {
            string ip = Request.UserHostAddress;
            string hostname = Request.UserHostName;
       
            return View();
        }

        /// <summary>
        /// ADDED TEST CODE FOR POPUP BY SHRADDHA ON 19 JAN 2017
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Lyubomir()
        {
            return View();
        }

        public ActionResult ChangePasswordForPopup()
        {
            return PartialView();
        }

        /// <summary>
        /// ADDED TEST CODE FOR TRYING CHANGE PASSWORD IN POPUP BY SHRADDHA ON 19 JAN 2017
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ChangePasswordForPopup(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                //Added By Shraddha on 05 Nov 2016 for change Password logic Apply
                Cryptography objcrypt = new Cryptography();

                string OldPassword = objcrypt.EncryptPassword(model.OldPassword.Trim());
                string NewPassword = model.NewPassword;

                var user = WetosDB.Users.Where(u => u.Password == OldPassword).FirstOrDefault();


                if (user != null)
                {

                    user.Password = objcrypt.EncryptPassword(NewPassword.Trim());
                    //user.Password = NewPassword;
                    WetosDB.SaveChanges();
                    ViewBag.Message = " Your password has been changed successfully.";
                    Session["Username"] = user.UserName;
                    // ADDED BY PUSHKAR FOR AuditLog ON 21 DEC 2016
                    AddAuditTrail("Password Changed");

                    //return Json(new { success = true });
                    return RedirectToAction("PopupTestCode");
                }

                else
                {
                    ModelState.AddModelError("", "The Old password is incorrect.");
                    //return Json(new { success = false });
                    return PartialView();
                }

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                //bool changePasswordSucceeded;

                //    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                //    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);


                //if (changePasswordSucceeded)
                //{
                //    return RedirectToAction("ChangePasswordSuccess");
                //}
                //else
                //{
                //    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                //}
            }

            else
            {
                return PartialView();
                //return Json(new { success = false });
            }
            // If we got this far, something failed, redisplay form

        }


        public ActionResult Calender()
        {
            return View();
        }

        public ActionResult ContactUs()
        {
            return View();
        }

        public ActionResult Index1()
        {
            DateTime StartDate = new DateTime(2015, 1, 1);
            DateTime EndDate = new DateTime(2015, 1, 31);

            List<DailyTransaction> DailyTransactionList = WetosDB.DailyTransactions.Where(dailyt => dailyt.EmployeeId == 1100 && dailyt.TranDate >= StartDate && dailyt.TranDate <= EndDate).ToList();

            if (DailyTransactionList != null)
            {
                return View(DailyTransactionList);
            }
            else
            {
                return View();
            }
        }

        // NOTIFICATION REGION ADDED BY RAJAS ON 31 DEC 2016
        #region NOTIFICATION

        /// <summary>
        /// Added By Rajas on 31 DEC 2016 for Getting Notification List
        /// </summary>
        /// <param name="ToID"></param>
        /// <returns></returns>
        /// Updated by Rajas on 5 APRIL 2017
        public ActionResult Notification(int ToID)
        {
            try
            {
                List<SP_GetNotifications_Result> NotificationList = WetosDB.SP_GetNotifications(ToID).OrderByDescending(a => a.NotificationID).ToList();

                // NOTIFICATION COUNT
                int NoTiCount = WetosDB.Notifications.Where(a => a.ToID == ToID && (a.ReadFlag == false || a.ReadFlag == null)).Count();
                Session["NotificationCount"] = NoTiCount;


                return View(NotificationList);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in Notification display due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                Error("Error in displaying Notifications. Please try again!");

                return null;
            }
        }

        /// <summary>
        /// ADDED BY SHRADDHA ON 10 JAN 2017 TO SHOW BIRTHDAY LIST OF CURRENT MONTH ON IEW 
        /// </summary>
        /// <returns></returns>
        /// Updated by Rajas on 5 APRIL 2017
        [HttpPost]
        public ActionResult Birthday()
        {
            try
            {
                int CurrentMonth = DateTime.Now.Month;
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                //List<VwActiveEmployee> EmployeeList = WetosDB.VwActiveEmployees.Where(a => a.BirthDate.Value.Month == CurrentMonth).Take(4).ToList();
                List<SP_VwActiveEmployee_Result> EmployeeList = WetosDB.SP_VwActiveEmployee(EmployeeId).Where(a => a.BirthDate.Value.Month == CurrentMonth).Take(4).ToList();
                #endregion

                return PartialView(EmployeeList);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in Birthday list due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                return null;
            }
        }

        /// <summary>
        /// Updated by Rajas on 5 APRIL 2017
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Selfbirthday()
        {
            try
            {
                int EmpId = Convert.ToInt32(Session["EmployeeNo"]);

                int CurrentMonth = DateTime.Now.Month;

                // Updated by Rajas on 5 APRIL 2017 Employee replaced with VwActiveEmployee
                #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                //List<VwActiveEmployee> EmployeeObj = WetosDB.VwActiveEmployees.Where(a => a.BirthDate.Value.Month == CurrentMonth && a.EmployeeId == EmpId).ToList();
                List<SP_VwActiveEmployee_Result> EmployeeObj = WetosDB.SP_VwActiveEmployee(EmpId).Where(a => a.BirthDate.Value.Month == CurrentMonth && a.EmployeeId == EmpId).ToList();
                #endregion

                return PartialView(EmployeeObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in Self birthday display due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                Error("Birthday not displayed. Please try again!");

                return null;
            }
        }

        /// <summary>
        /// BIRTHDAY LIST SHOW ON 11 JAN 2017 BY SHRADDHA
        /// Updated by Rajas on 5 APRIL 2017
        /// </summary>
        /// <returns></returns>
        public ActionResult BirthdayListView()
        {
            try
            {
                int CurrentMonth = DateTime.Now.Month;

                //List<Employee> EmployeeObj = WetosDB.Employees.Where(a => a.BirthDate.Value.Month == CurrentMonth).ToList();

                // Updated by Rajas on 2 MARCH 2017 for active employees
                #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //List<VwActiveEmployee> EmployeeObj = WetosDB.VwActiveEmployees.Where(a => a.BirthDate.Value.Month == CurrentMonth && (a.ActiveFlag == null || a.ActiveFlag == true)).ToList();
                List<SP_VwActiveEmployee_Result> EmployeeObj = WetosDB.SP_VwActiveEmployee(EmployeeId).Where(a => a.BirthDate.Value.Month == CurrentMonth).ToList();
                #endregion
                return View(EmployeeObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in Birthday list due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                Error("Birthday list not diplayed, Please try again and verify!");

                return null;
            }
        }


        /// <summary>
        /// ADDED BY SHRADDHA ON 10 JAN 2017 TO SHOW CONFIRMATION LIST OF CURRENT MONTH ON VIEW 
        /// </summary>
        /// <returns></returns>
        /// Updated by Rajas on 5 APRIL 2017
        [HttpPost]
        public ActionResult Confirmation()
        {
            try
            {
                int CurrentMonth = DateTime.Now.Month;

                int CurrentYear = DateTime.Now.Year;

                // Employee replaced with VwActiveEmployee, by Rajas on 5 APRIL 2017
                List<VwActiveEmployee> EmployeeObj = WetosDB.VwActiveEmployees.Where(a => a.ConfirmDate.Value.Month == CurrentMonth
                    && a.ConfirmDate.Value.Year == CurrentYear).Take(4).ToList();

                return PartialView(EmployeeObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in confirmation due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                Error("Error in confirmation of Employee. Please try again!");

                return null;
            }
        }

        /// <summary>
        /// ADDED BY SHRADDHA ON 10 JAN 2017 TO SHOW CONFIRMATION FOR EMPLOYEE
        /// </summary>
        /// <returns></returns>
        /// Updated by Rajas on 5 APRIL 2017
        [HttpPost]
        public ActionResult SelfConfirmation()
        {
            try
            {
                int EmpId = Convert.ToInt32(Session["EmployeeNo"]);
                int CurrentMonth = DateTime.Now.Month;
                int CurrentYear = DateTime.Now.Year;


                #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //List<Employee> EmployeeObj = WetosDB.Employees.Where(a => a.ConfirmDate.Value.Month == CurrentMonth && a.ConfirmDate.Value.Year == CurrentYear && a.EmployeeId == EmpId).ToList();
                var EmployeeObj = WetosDB.SP_VwActiveEmployee(EmployeeId).Where(a => a.ConfirmDate.Value.Month == CurrentMonth && a.ConfirmDate.Value.Year == CurrentYear && a.EmployeeId == EmpId).ToList();
                #endregion

                return PartialView(EmployeeObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                Error("Please try again and verify confirmation details");

                return null;
            }
        }

        /// <summary>
        /// BIRTHDAY LIST SHOW ON 11 JAN 2017 BY SHRADDHA
        /// </summary>
        /// <returns></returns>
        public ActionResult ConfirmationListView()
        {
            try
            {
                int CurrentMonth = DateTime.Now.Month;

                int CurrentYear = DateTime.Now.Year;

                // Employee replaced with VwActiveEmployee, by Rajas on 5 APRIL 2017

                #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                // List<VwActiveEmployee> EmployeeObj = WetosDB.VwActiveEmployees.Where(a => a.ConfirmDate.Value.Month == CurrentMonth
                // && a.ConfirmDate.Value.Year == CurrentYear).ToList();
                List<SP_VwActiveEmployee_Result> EmployeeObj = WetosDB.SP_VwActiveEmployee(EmployeeId).Where(a => a.ConfirmDate.Value.Month == CurrentMonth
                    && a.ConfirmDate.Value.Year == CurrentYear).ToList();
                #endregion
                return View(EmployeeObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in displaying Confirmation List View due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                Error("Error in displaying confirmation list");

                return null;
            }
        }


        /// <summary>
        /// Added By Rajas on 31 DEC 2016 for Mark as read notification list
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult NotificationRead(int id)
        {
            try
            {
                Notification Notification = WetosDB.Notifications.Where(a => a.NotificationID == id).FirstOrDefault();
                if (Notification != null)
                {
                    Notification.ReadFlag = true;
                    Notification.ReadDate = DateTime.Now;

                    WetosDB.SaveChanges();
                }
                return RedirectToAction("Notification", new { ToID = Notification.ToID });
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in viewing Notification due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                Error("Error in viewing Notification");

                return null;
            }
        }

        /// <summary>
        /// Added By Rajas on 31 DEC 2016 for Opening Notification Content
        /// </summary>
        /// <param name="NotificationID"></param>
        /// <returns></returns>
        /// Updated by Rajas on 5 APRIL 2017
        public ActionResult NotificationContent(int NotificationID)
        {
            try
            {
                Notification NotificationObj = WetosDB.Notifications.Where(a => a.NotificationID == NotificationID).FirstOrDefault();

                if (NotificationObj != null)
                {
                    NotificationObj.ReadDate = DateTime.Now;
                    NotificationObj.ReadFlag = true;

                    WetosDB.SaveChanges();
                }

                return View(NotificationObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error in viewing Notification due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                Error("Error in viewing Notification");

                return null;
            }
        }

        #endregion


        #region NOTIFICATION ON DASHBOARD AS PER MARS CODE
        public JsonResult ReadMore(DateTime FromDate, DateTime ToDate)
        {
            int EmpNo = Convert.ToInt32(Session["EmployeeNo"]);
            List<WetosDB.Notification> vmessages = new List<WetosDB.Notification>();

            // Updated by Rajas on 16 FEB 2017 for Displaying distinct notifications for logged in user
            vmessages = WetosDB.Notifications.Where(a => a.ToID == EmpNo).OrderByDescending(a => a.NotificationID).ToList();

            var obj = MvcHelpers.RenderPartialView(this, "ReadMore", vmessages);
            return Json(obj);
        }

        /// <summary>
        /// Updated by Rajas on 5 APRIL 2017
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult MessageDetails(int id)
        {
            try
            {
                WetosDB.Notification vmes = WetosDB.Notifications.Where(a => a.NotificationID == id).FirstOrDefault();
                int msgfrom_id = Convert.ToInt32(vmes.FromID);
                var msgFrom = WetosDB.Employees.Where(a => a.EmployeeId == msgfrom_id).FirstOrDefault();
                ViewBag.msgFrom = msgFrom;
                return PartialView(vmes);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                Error("Inconsistency between notification messages, please verify!");

                return null;
            }

        }

        /// <summary>
        /// Commented by Rajas on 3 APRIL 2017
        /// </summary>
        /// <returns></returns>
        //public ActionResult Newmessage()
        //{

        //    var msgType = WetosDB.MessageTypes.Select(a => new { id = a.MsgTypeId, name = a.MsgTypeName }).ToList();
        //    ViewBag.msgType = new SelectList(msgType, "id", "name");

        //    var sel_messageto = WetosDB.Employees.Select(a => new { id = a.EmployeeId, name = a.Title + " " + a.FirstName + " " + a.MiddleName + " " + a.LastName }).ToList();
        //    ViewBag.sel_messageto = new SelectList(sel_messageto, "id", "name");



        //    return PartialView();

        //}

        public JsonResult AutoCompleteMsgTo(string term)
        {
            var result = (from r in WetosDB.Employees
                          where r.FirstName.ToLower().Contains(term.ToLower())
                          //EARLIER CODE COMMENTED AND ADDED NEW CODE BY SHRADDHA AS PER SUGGESTED BY MSJ ON TO HANDLE EMPTY MIDDLE NAME PROBLEM 30 OCT 2017 START
                          //select new { id = r.EmployeeId, name = r.Title + "" + r.FirstName + "" + r.MiddleName + "" + r.LastName }).Distinct();
                          select new { id = r.EmployeeId, name = r.Title + " " + r.FirstName + " " + r.LastName }).Distinct();
            //EARLIER CODE COMMENTED AND ADDED NEW CODE BY SHRADDHA AS PER SUGGESTED BY MSJ ON TO HANDLE EMPTY MIDDLE NAME PROBLEM 30 OCT 2017 END

            return Json(result, JsonRequestBehavior.AllowGet);

            //var result = (from r in db.Customers
            //              where r.Country.ToLower().Contains(term.ToLower())
            //              select new { r.Country }).Distinct();
            //return Json(result, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        public ActionResult SendMessage(string ToEmpTypeId, string MessageSubject, string MessageContent, string FromEmpId, string MessageSubjectText, string ToEmpId)
        {
            try
            {
                MessageModel MessageModelObj = new MessageModel();
                MessageModelObj.ToEmpTypeId = Convert.ToInt32(ToEmpTypeId == null ? "0" : ToEmpTypeId);
                MessageModelObj.MessageSubject = Convert.ToInt32(MessageSubject == null ? "0" : MessageSubject);
                MessageModelObj.MessageContent = MessageContent;
                MessageModelObj.FromEmpId = Convert.ToInt32(FromEmpId == null ? "0" : FromEmpId);
                string Message_Sub = string.Empty;
                if (MessageModelObj.MessageSubject == 2)
                {
                    Message_Sub = MessageSubjectText;
                }
                else
                {

                    Message_Sub = WetosDB.DropdownDatas.Where(a => a.TypeId == 16 && a.Value == MessageModelObj.MessageSubject).Select(a => a.Text).FirstOrDefault();
                }

                int e_id = SessionPersister.UserInfo.UserId;

                if (MessageModelObj.ToEmpTypeId == 1)
                {
                    List<SP_GetHRList_Result> HRList = WetosDB.SP_GetHRList().ToList();
                    if (HRList.Count > 0)
                    {
                        foreach (SP_GetHRList_Result HROj in HRList)
                        {
                            Notification nmsg = new Notification();
                            nmsg.SendDate = System.DateTime.Now;
                            nmsg.FromID = SessionPersister.UserInfo.UserId;
                            nmsg.ToID = HROj.EmployeeId;
                            nmsg.NotificationContent = Message_Sub + " : " + MessageModelObj.MessageContent;
                            WetosDB.Notifications.Add(nmsg);
                            WetosDB.SaveChanges();
                        }

                    }

                }

                else if (MessageModelObj.ToEmpTypeId == 2)
                {
                    List<SP_GetLeaveApproverList_Result> LeaveApproverList = WetosDB.SP_GetLeaveApproverList(SessionPersister.UserInfo.UserId).ToList();
                    if (LeaveApproverList.Count > 0)
                    {
                        foreach (SP_GetLeaveApproverList_Result LeaveApproverObj in LeaveApproverList)
                        {
                            Notification nmsg = new Notification();
                            nmsg.SendDate = System.DateTime.Now;
                            nmsg.FromID = SessionPersister.UserInfo.UserId;
                            nmsg.ToID = LeaveApproverObj.EmployeeId;
                            nmsg.NotificationContent = Message_Sub + " : " + MessageModelObj.MessageContent;
                            WetosDB.Notifications.Add(nmsg);
                            WetosDB.SaveChanges();
                        }

                    }

                }
                else if (MessageModelObj.ToEmpTypeId == 3)
                {
                    List<SP_GetSanctionerList_Result> SanctionerList = WetosDB.SP_GetSanctionerList(SessionPersister.UserInfo.UserId).ToList();
                    if (SanctionerList.Count > 0)
                    {
                        foreach (SP_GetSanctionerList_Result SanctionerObj in SanctionerList)
                        {
                            Notification nmsg = new Notification();
                            nmsg.SendDate = System.DateTime.Now;
                            nmsg.FromID = SessionPersister.UserInfo.UserId;
                            nmsg.ToID = SanctionerObj.EmployeeId;
                            nmsg.NotificationContent = Message_Sub + " : " + MessageModelObj.MessageContent;
                            WetosDB.Notifications.Add(nmsg);
                            WetosDB.SaveChanges();
                        }
                    }
                }
                else if (MessageModelObj.ToEmpTypeId == 4)
                {
                    List<SP_GetEmployeeListReportingTome_Result> EmpReportingToMeList = WetosDB.SP_GetEmployeeListReportingTome(SessionPersister.UserInfo.UserId).ToList();
                    if (EmpReportingToMeList.Count > 0)
                    {
                        foreach (SP_GetEmployeeListReportingTome_Result EmpReportingToMeObj in EmpReportingToMeList)
                        {
                            Notification nmsg = new Notification();
                            nmsg.SendDate = System.DateTime.Now;
                            nmsg.FromID = SessionPersister.UserInfo.UserId;
                            nmsg.ToID = EmpReportingToMeObj.EmployeeId;
                            nmsg.NotificationContent = Message_Sub + " : " + MessageModelObj.MessageContent;
                            WetosDB.Notifications.Add(nmsg);
                            WetosDB.SaveChanges();
                        }

                    }

                }
                else if (MessageModelObj.ToEmpTypeId == 5)
                {
                    Notification nmsg = new Notification();
                    nmsg.SendDate = System.DateTime.Now;
                    nmsg.FromID = SessionPersister.UserInfo.UserId;
                    nmsg.ToID = Convert.ToInt32(ToEmpId == null ? "0" : ToEmpId);
                    nmsg.NotificationContent = Message_Sub + " : " + MessageModelObj.MessageContent;
                    WetosDB.Notifications.Add(nmsg);
                    WetosDB.SaveChanges();
                }

                AddAuditTrail("Message Successfully sent : From EmployeeId - " + SessionPersister.UserInfo.UserId);

            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Message Sending failed:" + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));
            }


            return Redirect("Index");

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult NewMessage(MessageModel MessageModelObj, FormCollection fc)
        {
            try
            {
                string Message_Sub = string.Empty;
                if (MessageModelObj.MessageSubject == 2)
                {
                    Message_Sub = fc.GetValue("MessageSubjectText").AttemptedValue;
                }
                else
                {

                    Message_Sub = WetosDB.DropdownDatas.Where(a => a.TypeId == 16 && a.Value == MessageModelObj.MessageSubject).Select(a => a.Text).FirstOrDefault();
                }

                int e_id = SessionPersister.UserInfo.UserId;

                if (MessageModelObj.ToEmpTypeId == 1)
                {
                    List<SP_GetHRList_Result> HRList = WetosDB.SP_GetHRList().ToList();
                    if (HRList.Count > 0)
                    {
                        foreach (SP_GetHRList_Result HROj in HRList)
                        {
                            Notification nmsg = new Notification();
                            nmsg.SendDate = System.DateTime.Now;
                            nmsg.FromID = SessionPersister.UserInfo.UserId;
                            nmsg.ToID = HROj.EmployeeId;
                            nmsg.NotificationContent = Message_Sub + " : " + MessageModelObj.MessageContent;
                            WetosDB.Notifications.Add(nmsg);
                            WetosDB.SaveChanges();
                        }

                    }

                }

                else if (MessageModelObj.ToEmpTypeId == 2)
                {
                    List<SP_GetLeaveApproverList_Result> LeaveApproverList = WetosDB.SP_GetLeaveApproverList(SessionPersister.UserInfo.UserId).ToList();
                    if (LeaveApproverList.Count > 0)
                    {
                        foreach (SP_GetLeaveApproverList_Result LeaveApproverObj in LeaveApproverList)
                        {
                            Notification nmsg = new Notification();
                            nmsg.SendDate = System.DateTime.Now;
                            nmsg.FromID = SessionPersister.UserInfo.UserId;
                            nmsg.ToID = LeaveApproverObj.EmployeeId;
                            nmsg.NotificationContent = Message_Sub + " : " + MessageModelObj.MessageContent;
                            WetosDB.Notifications.Add(nmsg);
                            WetosDB.SaveChanges();
                        }

                    }

                }

                else if (MessageModelObj.ToEmpTypeId == 3)
                {
                    List<SP_GetSanctionerList_Result> SanctionerList = WetosDB.SP_GetSanctionerList(SessionPersister.UserInfo.UserId).ToList();
                    if (SanctionerList.Count > 0)
                    {
                        foreach (SP_GetSanctionerList_Result SanctionerObj in SanctionerList)
                        {
                            Notification nmsg = new Notification();
                            nmsg.SendDate = System.DateTime.Now;
                            nmsg.FromID = SessionPersister.UserInfo.UserId;
                            nmsg.ToID = SanctionerObj.EmployeeId;
                            nmsg.NotificationContent = Message_Sub + " : " + MessageModelObj.MessageContent;
                            WetosDB.Notifications.Add(nmsg);
                            WetosDB.SaveChanges();
                        }

                    }

                }

                else if (MessageModelObj.ToEmpTypeId == 4)
                {
                    List<SP_GetEmployeeListReportingTome_Result> EmpReportingToMeList = WetosDB.SP_GetEmployeeListReportingTome(SessionPersister.UserInfo.UserId).ToList();
                    if (EmpReportingToMeList.Count > 0)
                    {
                        foreach (SP_GetEmployeeListReportingTome_Result EmpReportingToMeObj in EmpReportingToMeList)
                        {
                            Notification nmsg = new Notification();
                            nmsg.SendDate = System.DateTime.Now;
                            nmsg.FromID = SessionPersister.UserInfo.UserId;
                            nmsg.ToID = EmpReportingToMeObj.EmployeeId;
                            nmsg.NotificationContent = Message_Sub + " : " + MessageModelObj.MessageContent;
                            WetosDB.Notifications.Add(nmsg);
                            WetosDB.SaveChanges();
                        }

                    }

                }

                else if (MessageModelObj.ToEmpTypeId == 5)
                {

                    Notification nmsg = new Notification();
                    nmsg.SendDate = System.DateTime.Now;
                    nmsg.FromID = SessionPersister.UserInfo.UserId;
                    nmsg.ToID = Convert.ToInt32(fc.GetValue("ToEmpId").AttemptedValue);
                    nmsg.NotificationContent = Message_Sub + " : " + MessageModelObj.MessageContent;
                    WetosDB.Notifications.Add(nmsg);
                    WetosDB.SaveChanges();


                }
                AddAuditTrail("Message Successfully sent : From EmployeeId - " + SessionPersister.UserInfo.UserId);

            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Message Sending failed:" + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));
            }


            return Redirect("Index");

        }

        #endregion

        /// <summary>
        /// CHANGED BY SHRADDHA ON 18 SEP 2017
        /// </summary>
        /// <returns></returns>
        public ActionResult Newmessage()
        {

            int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            MessageModel MessageModelObj = new MessageModel();
            MessageModelObj.FromEmpId = EmployeeId;

            //DropdownData typeid=17 For Message SendTo
            var MessageSendToList = WetosDB.DropdownDatas.Where(a => a.TypeId == 17).Select(a => new { Value = a.Value, Text = a.Text }).ToList();
            ViewBag.MessageSendToList = new SelectList(MessageSendToList, "Value", "Text").ToList();

            //DropdownData typeid=16 For MessageSubject
            var MessageSubjectList = WetosDB.DropdownDatas.Where(a => a.TypeId == 16).Select(a => new { Value = a.Value, Text = a.Text }).ToList();
            ViewBag.MessageSubjectList = new SelectList(MessageSubjectList, "Value", "Text").ToList();


            var EmployeeList = new List<SP_GetEmployeeListForMessaging_Result>(); //WetosDB.Departments.Select(a => new { DepartmentId = a.DepartmentId, DepartmentName = a.DepartmentName }).ToList();
            ViewBag.EmployeeList = new SelectList(EmployeeList, "EmployeeId", "EmployeeName").ToList();
            return PartialView(MessageModelObj);

        }


        /// <summary>
        /// Json return for to get Branch dropdown list on basis of company selection
        /// Added by Rajas on 27 DEC 2016
        /// </summary>
        /// <returns></returns>
        public JsonResult EmployeesReportingtoMe(int EmployeeId)
        {

            if (EmployeeId == 0)
            {
                EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
            }


            var EmployeeList = WetosDB.SP_GetEmployeeListReportingTome(EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " - " + (a.FirstName == null ? "" : a.FirstName) + " " + (a.LastName == null ? "" : a.LastName) }).ToList();


            return Json(EmployeeList);
        }


        /// <summary>
        /// FUNCTION ADDED BY SHRADDHA ON 05 SEP 2017 
        /// TO TRANSLATE DIGITS FROM HINDI TO ENGLISH
        /// THIS IS TEMPORARY TRANSLATION SOLUTION. WE MAY NEED TO USE GOOGLE TRANSLATOR IN FUTURE FOR FULL TRANSLATION FACILITY
        /// <returns></returns>
        public static string ConvertDigits(string s)
        {
            return s
                .Replace("०", "0")
                .Replace("१", "1")
                .Replace("२", "2")
                .Replace("३", "3")
                .Replace("४", "4")
                .Replace("५", "5")
                .Replace("६", "6")
                .Replace("७", "7")
                .Replace("८", "8")
                .Replace("९", "9");
        }

    }



    //ADDED BY Shraddha for timetable
    public class timeTable
    {

        public String title { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public String backgroundColor { get; set; }
        public String borderColor { get; set; }

    }




}

