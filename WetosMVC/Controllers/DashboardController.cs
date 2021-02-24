using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using WetosDB;

namespace WetosMVC.Controllers
{
    [SessionExpire] 
    [Authorize]
    public class DashboardController : BaseController  // Updated on Rajas on 30 MARCH 2017
    {
        
        //
        // GET: /Dashboard/

        public ActionResult Index()
        {
            try
            {
                return RedirectToAction("EmployeeDashboard");
            }
            catch
            {
                return View();
            }
            //return View();
        }

        public ActionResult EmployeeDashboard()
        {
            List<WetosDB.sp_get_Notifications_Result> notifications = new List<sp_get_Notifications_Result>();

            try
            {
                int EmpId = Convert.ToInt32(Session["EmployeeNo"]);
                if (SessionPersister.UserInfo.UserId > 0)
                {
                    // Updated by Rajas on 18 FEB 2017 for displaying latest 10 contents in notificationon dashboard
                    notifications = WetosDB.sp_get_Notifications(SessionPersister.UserInfo.UserId, DateTime.Now).Where(a => a.readflag == 0).OrderByDescending(a => a.SendDate).Take(5).ToList();
                }
                // Code added by Rajas on 7 APRIL 2017 for displaying leave table

                //CODE ADDED BY SHRADDHA TO GET GLOBAL SETTING VALUE FOR APPLICATION ALLOWED FROM CALENDAR OR NOT ON 03 NOV 2017 START
                PopulateDropdown(EmpId);

                //CODE ADDED BY SHRADDHA ON 20 NOV 2017 START
                DisplayLeaveDataOnDashboard(EmpId);
                //CODE ADDED BY SHRADDHA ON 20 NOV 2017 END

                // LIST OF EMPLOYEE REPORTING START // ADDED BY MSJ ON 24 DEC 2018

                var TempReportingEmployeeListObj = WetosDB.SP_VwActiveEmployee(EmpId).Where(a => a.EmployeeReportingId == EmpId && (a.ActiveFlag == null || a.ActiveFlag == true)).ToList();
                var ReportingEmployeeList = TempReportingEmployeeListObj.Select(a => new { Value = a.EmployeeId, Text = a.EmployeeCode + " - " + a.FirstName + " " + a.LastName }).ToList();
                ViewBag.ActiveEmployeeListVB = new SelectList(ReportingEmployeeList, " Value", "Text").ToList();

                int ReportingEmpCount = ReportingEmployeeList.Count;
                ViewBag.ActiveEmployeeListCountVB = ReportingEmpCount;
                // LIST OF EMPLOYEE REPORTING END // ADDED BY MSJ ON 24 DEC 2018

                // CHECK REQUIRED DATA PRESNT OR NOT
                // LEAVE DATA
                List<LeaveCredit> MyLeaveCreditList = WetosDB.LeaveCredits.Where(a => a.EmployeeId == EmpId && a.FinancialYearId == 5).ToList();
                List<LeaveBalance> MyLeaveBalanceList = WetosDB.LeaveBalances.Where(a => a.EmployeeId == EmpId ).ToList(); //&& a.FinacialYearId == 5).ToList();

                if (MyLeaveCreditList.Count == 0 || MyLeaveBalanceList.Count == 0)
                {
                    Information("Import Leave Credit and Balance for Current Year");
                }


                return View(notifications);
            }
            catch (System.Exception ex)
            {
                // Added by Rajas on 30 MARCH 2017 START
                AddAuditTrail("Error in EmployeeDashboard due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                Error("" + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));
                // Added by Rajas on 30 MARCH 2017 END

                return View(notifications);
            }
        }

        /// <summary>
        /// CODE ADDED BY SHRADDHA ON 20 NOV 2017
        /// CUT CODE FROM POPULATE DROPDOWN FUNCTION AND PASTE IN THIS FUNCTION
        /// <param name="EmpId"></param>
        public void DisplayLeaveDataOnDashboard(int EmpId)
        {
            // Added by Rajas on 14 AUGUST 2017 START
            //GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText.Trim() == "Is Detailed Leave Data").FirstOrDefault();
            //ABOVE LINE COMMENTED AND BELOW LINE IS ADDED BY SHALAKA ON 13TH DEC 2017
            GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText.Trim() == GlobalSettingsConstant.IsDetailedLeaveData).FirstOrDefault();

            if (GlobalSettingObj == null)
            {
                ViewBag.LeaveBalanceDetailsVB = DashboardController.GetLeaveDetailsOnallControllers(WetosDB, EmpId, DateTime.Now);
                ViewBag.LeaveTableview = "0";
            }
            else
            {
                if (GlobalSettingObj.SettingValue == "1")
                {
                    ViewBag.LeaveBalanceDetailsVB = DashboardController.GetLeaveDetailsOnallControllersNew(WetosDB, EmpId, DateTime.Now);
                    ViewBag.LeaveTableview = "1";
                }
                else
                {
                    ViewBag.LeaveBalanceDetailsVB = DashboardController.GetLeaveDetailsOnallControllers(WetosDB, EmpId, DateTime.Now);
                    ViewBag.LeaveTableview = "0";
                }
            }
            // Added by Rajas on 14 AUGUST 2017 END
        }

        /// <summary>
        /// CODE ADDED BY SHRADDHA ON 30 AUG 2017 TO POPULATE DATA ON EMPLOYEE DASHBOARD
        /// </summary>
        /// <param name="EmpId"></param>
        public void PopulateDropdown(int EmpId)
        {
            //ADDED CODE BY SHRADDHA ON 29 AUG 2017 FOR LEAVE DAY STATUS Dropdown list for FULL DAY or HALF DAY Leave START
            var LeaveTypeStatusObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 9).Select(a => new { LeaveStatusID = a.Value, LeaveStatus = a.Text }).ToList();
            ViewBag.LeaveTypeStatusList = new SelectList(LeaveTypeStatusObj, "LeaveStatusID", "LeaveStatus").ToList();
            //ADDED CODE BY SHRADDHA ON 29 AUG 2017 FOR LEAVE DAY STATUS Dropdown list for FULL DAY or HALF DAY Leave END

            //CODE FOR OD REQUISITION TYPE LIST IN DROPDOWN ADDED BY SHRADDHA ON 29 AUG 2017 START
            var RequisitionTYpeObj = WetosDB.DropdownDatas.Where(a => a.TypeId == 1).Select(a => new { ODTourId = a.Text, ODTourType = a.Text }).ToList();
            ViewBag.RequisitionTYpeList = new SelectList(RequisitionTYpeObj, "ODTourId", "ODTourType").ToList();
            //CODE FOR OD REQUISITION TYPE LIST IN DROPDOWN ADDED BY SHRADDHA ON 29 AUG 2017 END

            //CODE FOR OD PURPOSE LIST IN DROPDOWN ADDED BY SHRADDHA ON 29 AUG 2017 START
            var ODpurpose = WetosDB.DropdownDatas.Where(a => a.TypeId == 15).Select(a => new { ODPurposeId = a.Text, ODPurposeType = a.Text }).ToList();
            ViewBag.ODPurposeList = new SelectList(ODpurpose, "ODPurposeId", "ODPurposeType").ToList();
            //CODE FOR OD PURPOSE LIST IN DROPDOWN ADDED BY SHRADDHA ON 29 AUG 2017 END

            //ADDED CODE BY SHRADDHA ON 29 AUG 2017 FOR LEAVE DAY STATUS Dropdown list for FULL DAY or HALF DAY Leave START
            Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == EmpId).FirstOrDefault();

            //CODE ADDED BY SHRADDHA ON 20 NOV 2017 TO HANDLE EMPLOYEEOBJ IS NULL START
            if (EmployeeObj == null)
            {

            }
            else //CODE ADDED BY SHRADDHA ON 20 NOV 2017 TO HANDLE EMPLOYEEOBJ IS NULL END
            {
                EmployeeGroupDetail EmployeeGroupDetailObj = WetosDB.EmployeeGroupDetails.Where(a => a.Employee.EmployeeId == EmpId).FirstOrDefault();

                if (EmployeeGroupDetailObj == null)
                { }
                else
                {
                    var LeaveMasterList = WetosDB.LeaveMasters.Where(a => a.Company.CompanyId == EmployeeObj.CompanyId && a.BranchId == EmployeeObj.BranchId
                            && a.EmployeeGroup.EmployeeGroupId == EmployeeGroupDetailObj.EmployeeGroup.EmployeeGroupId).Select(a => new { LeaveTypeID = a.LeaveId, LeaveType = a.LeaveCode }).ToList();

                    ViewBag.LeaveTypeList = new SelectList(LeaveMasterList, "LeaveTypeID", "LeaveType").ToList();
                    ViewBag.EmployeeId = EmpId;
                    //ADDED CODE BY SHRADDHA ON 29 AUG 2017 FOR LEAVE DAY STATUS Dropdown list for FULL DAY or HALF DAY Leave END

                    //CODE FOR SHIFT LIST IN DROPDOWN ADDED BY SHRADDHA ON 30 AUG 2017 START
                    var Shift = WetosDB.Shifts.Where(a => a.Company.CompanyId == EmployeeObj.CompanyId && a.BranchId == EmployeeObj.BranchId).Select(a => new { ShiftId = a.ShiftCode, ShiftName = a.ShiftName }).ToList();
                    ViewBag.ShiftList = new SelectList(Shift, "ShiftId", "ShiftName").ToList();
                    //CODE FOR SHIFT LIST IN DROPDOWN ADDED BY SHRADDHA ON 30 AUG 2017 END

                    //ADDED CODE BY SHRADDHA ON 30 AUG 2017 FOR EXCEPTION REASON Dropdown list  START
                    var ExceptionReasonList = WetosDB.DropdownDatas.Where(a => a.TypeId == 10).Select(a => new { ExceptionReasonID = a.Value, ExceptionReasonText = a.Text }).ToList();
                    ViewBag.ExceptionReasonList = new SelectList(ExceptionReasonList, "ExceptionReasonID", "ExceptionReasonText").ToList();
                    //ADDED CODE BY SHRADDHA ON 30 AUG 2017 FOR EXCEPTION REASON Dropdown list END
                }
            }



        }



        public ActionResult ShowCalendarofReportingToMe(int EmployeeId)
        {
            ViewBag.SelectedEmployeeId = EmployeeId;
            return View();
        }

        /// <summary>
        /// GENERIC CODE FOR GETTING LEAVE DETAILS ON ALL CONTROLLERS
        /// </summary>
        /// <param name="WetosDB"></param>
        /// <param name="EmpId"></param>
        /// <returns></returns>
        public static List<SP_LeaveTableDataNew_Result> GetLeaveDetailsOnallControllersNew(WetosDBEntities WetosDB, int EmpId, DateTime LeaveAsOnDateTime)
        {
            // Above line commented and below SP code adde by Rajas on 13 APRIL 2017
            //List<SP_LeaveTableData_Result> LeaveBalanceList = WetosDB.SP_LeaveTableData(EmpId).ToList();
            List<SP_LeaveTableDataNew_Result> LeaveBalanceList = WetosDB.SP_LeaveTableDataNew(EmpId, LeaveAsOnDateTime).ToList();
            List<SP_LeaveTableDataNew_Result> FinalLeaveBalanceList = new List<SP_LeaveTableDataNew_Result>();

            if (EmpId > 0)
            {
                // MODIFIED BY MSJ on 31 MAY 2017 START
                List<SP_LeaveTableDataNew_Result> LeaveBalanceList1 = WetosEmployeeController.GetLeavedataNewLogicNew(WetosDB, EmpId, LeaveAsOnDateTime);

                foreach (SP_LeaveTableDataNew_Result ABCD in LeaveBalanceList1)
                {
                    SP_LeaveTableDataNew_Result PQRS = LeaveBalanceList.Where(a => a.LeaveType.ToUpper().Trim() == ABCD.LeaveType.ToUpper().Trim()).FirstOrDefault();
                    if (PQRS != null)
                    {
                        if (ABCD.LeaveUsed == 0)
                        {
                            PQRS.LeaveAllowed = ABCD.LeaveAllowed;
                        }
                        else
                        {
                            PQRS.LeaveAllowed = ABCD.LeaveAllowed - (Convert.ToDecimal(ABCD.LeaveUsed) - Convert.ToDecimal(PQRS.TotalPaidLeaves));
                        }

                        PQRS.Pending = ABCD.Pending;
                        PQRS.OpeningBalance = ABCD.OpeningBalance;
                        PQRS.LeaveUsed = ABCD.LeaveUsed;
                        if (Convert.ToDecimal(PQRS.TotalDeductableDays) > PQRS.LeaveAllowed)
                        {
                            PQRS.TotalPaidLeaves = Convert.ToDouble(PQRS.LeaveAllowed);
                            PQRS.TotalUnPaidLeaves = PQRS.TotalDeductableDays - PQRS.TotalPaidLeaves;
                        }
                        else
                        {
                            PQRS.TotalPaidLeaves = Convert.ToDouble(PQRS.TotalDeductableDays);
                            PQRS.TotalUnPaidLeaves = PQRS.TotalDeductableDays - PQRS.TotalPaidLeaves;
                        }
                        PQRS.CurrentBalance = ABCD.CurrentBalance;
                        FinalLeaveBalanceList.Add(PQRS);
                    }
                }
            }
            //
            // MODIFIED BY MSJ on 31 MAY 2017 END

            return FinalLeaveBalanceList;
        }


        /// <summary>
        /// GENERIC CODE FOR GETTING CORRECT LEAVE DETAILS ON ALL REPORTS
        /// ADDED BY SHRADDHA ON 26 JULY 2017
        /// <param name="WetosDB"></param>
        /// <param name="EmpId"></param>
        /// <returns></returns>
        public static List<SP_LeaveTableDataNew_Result> GetLeaveDetailsForReport(WetosDBEntities WetosDB, int EmpId, DateTime LeaveAsOnDateTime, string LeaveType)
        {


            // Above line commented and below SP code adde by Rajas on 13 APRIL 2017
            //List<SP_LeaveTableData_Result> LeaveBalanceList = WetosDB.SP_LeaveTableData(EmpId).ToList();
            List<SP_LeaveTableDataNew_Result> LeaveBalanceList = WetosDB.SP_LeaveTableDataNew(EmpId, LeaveAsOnDateTime).ToList();
            List<SP_LeaveTableDataNew_Result> FinalLeaveBalanceList = new List<SP_LeaveTableDataNew_Result>();

            if (EmpId > 0)
            {
                // MODIFIED BY MSJ on 31 MAY 2017 START
                List<SP_LeaveTableDataNew_Result> LeaveBalanceList1 = WetosEmployeeController.GetLeaveDataOnReport(WetosDB, EmpId, LeaveAsOnDateTime, LeaveType);



                foreach (SP_LeaveTableDataNew_Result ABCD in LeaveBalanceList1)
                {
                    SP_LeaveTableDataNew_Result PQRS = LeaveBalanceList.Where(a => a.LeaveType.ToUpper().Trim() == ABCD.LeaveType.ToUpper().Trim()).FirstOrDefault();
                    if (PQRS != null)
                    {
                        if (ABCD.LeaveUsed == 0)
                        {
                            PQRS.LeaveAllowed = ABCD.LeaveAllowed;
                        }
                        else
                        {
                            PQRS.LeaveAllowed = ABCD.LeaveAllowed - (Convert.ToDecimal(ABCD.LeaveUsed) - Convert.ToDecimal(PQRS.TotalPaidLeaves));
                        }

                        PQRS.Pending = ABCD.Pending;
                        PQRS.OpeningBalance = ABCD.OpeningBalance;
                        PQRS.LeaveUsed = ABCD.LeaveUsed;
                        if (Convert.ToDecimal(PQRS.TotalDeductableDays) > PQRS.LeaveAllowed)
                        {
                            PQRS.TotalPaidLeaves = Convert.ToDouble(PQRS.LeaveAllowed);
                            PQRS.TotalUnPaidLeaves = PQRS.TotalDeductableDays - PQRS.TotalPaidLeaves;
                        }
                        else
                        {
                            PQRS.TotalPaidLeaves = Convert.ToDouble(PQRS.TotalDeductableDays);
                            PQRS.TotalUnPaidLeaves = PQRS.TotalDeductableDays - PQRS.TotalPaidLeaves;
                        }
                        PQRS.CurrentBalance = ABCD.CurrentBalance;
                        FinalLeaveBalanceList.Add(PQRS);
                    }
                }
            }
            //
            // MODIFIED BY MSJ on 31 MAY 2017 END

            return FinalLeaveBalanceList;
        }


        /// <summary>
        /// GENERIC CODE FOR GETTING LEAVE DETAILS ON ALL CONTROLLERS
        /// </summary>
        /// <param name="WetosDB"></param>
        /// <param name="EmpId"></param>
        /// <returns></returns>
        public static List<SP_LeaveTableData_Result> GetLeaveDetailsOnallControllers(WetosDBEntities WetosDB, int EmpId, DateTime LeaveAsOnDateTime)
        {


            // Above line commented and below SP code adde by Rajas on 13 APRIL 2017
            //List<SP_LeaveTableData_Result> LeaveBalanceList = WetosDB.SP_LeaveTableData(EmpId).ToList();
            List<SP_LeaveTableData_Result> LeaveBalanceList = WetosDB.SP_LeaveTableData(EmpId).ToList();
            List<SP_LeaveTableData_Result> FinalLeaveBalanceList = new List<SP_LeaveTableData_Result>();

            if (EmpId > 0)
            {
                // MODIFIED BY MSJ on 31 MAY 2017 START
                List<SP_LeaveTableData_Result> LeaveBalanceList1 = WetosEmployeeController.GetLeavedataNewLogic(WetosDB, EmpId, LeaveAsOnDateTime);



                foreach (SP_LeaveTableData_Result ABCD in LeaveBalanceList1)
                {
                    SP_LeaveTableData_Result PQRS = LeaveBalanceList.Where(a => a.LeaveType.ToUpper().Trim() == ABCD.LeaveType.ToUpper().Trim()).FirstOrDefault();
                    if (PQRS != null)
                    {
                        PQRS.LeaveAllowed = ABCD.LeaveAllowed;
                        //PQRS.Pending = ABCD.Pending;
                        PQRS.OpeningBalance = ABCD.OpeningBalance;
                        PQRS.LeaveUsed = ABCD.LeaveUsed;
                        PQRS.CurrentBalance = ABCD.CurrentBalance;
                        FinalLeaveBalanceList.Add(PQRS);
                    }
                }

                // ADD LWP 
                SP_LeaveTableData_Result LWP = new SP_LeaveTableData_Result();
                if (LWP != null)
                {
                    DateTime Today = DateTime.Now;
                    DateTime MonthStart = new DateTime(Today.Year, Today.Month, 1);

                    int LWPNo = WetosDB.DailyTransactions.Where(a => a.EmployeeId == EmpId &&a.TranDate >= MonthStart && a.TranDate <= Today 
                        && a.Status.Contains("AAAA")).Count();

                    LWP.LeaveType = "LWP";

                    //LWP.LeaveAllowed = ABCD.LeaveAllowed;
                    //PQRS.Pending = ABCD.Pending;
                    //LWP.OpeningBalance = ABCD.OpeningBalance;

                    LWP.LeaveUsed = LWPNo;

                    //LWP.CurrentBalance = ABCD.CurrentBalance;
                    FinalLeaveBalanceList.Add(LWP);
                }
            }
            //
            // MODIFIED BY MSJ on 31 MAY 2017 END

            return FinalLeaveBalanceList;
        }

        /// <summary>
        /// ADDED ACTIONRESULT AND VIEW BY SHRADDHA ON 16 MARCH 2017 FOR HR DASHBOARD
        /// </summary>
        /// <returns></returns>
        public ActionResult HRDashboard()
        {
            ViewBag.malepresent = 0;
            ViewBag.femalepresent = 0;
            ViewBag.total = 0;
            int Total = WetosDB.DailyTransactions.Where(y => y.TranDate.Day == DateTime.Now.Day
                                      && y.TranDate.Month == DateTime.Now.Month && y.TranDate.Year == DateTime.Now.Year && !y.Status.Contains("A")).Count();
            if (Total > 0)
            {
                ViewBag.total = Total;
            }
            int FemalePresentCount = (from y in WetosDB.DailyTransactions
                                      join z in WetosDB.Employees on y.EmployeeId equals z.EmployeeId
                                      where ((z.Gender.ToUpper().Trim() == "FEMALE" || z.Gender.Trim() == "F") && y.TranDate.Day == DateTime.Now.Day
                                      && y.TranDate.Month == DateTime.Now.Month && y.TranDate.Year == DateTime.Now.Year && !y.Status.Contains("A")
                                      )
                                      select y).Count();
            if (FemalePresentCount > 0)
            {
                ViewBag.femalepresent = FemalePresentCount;
            }
            int MalePresentCount = (from y in WetosDB.DailyTransactions
                                    join z in WetosDB.Employees on y.EmployeeId equals z.EmployeeId
                                    where ((z.Gender.ToUpper().Trim() == "MALE" || z.Gender.Trim() == "M") && y.TranDate.Day == DateTime.Now.Day
                                      && y.TranDate.Month == DateTime.Now.Month && y.TranDate.Year == DateTime.Now.Year && !y.Status.Contains("A"))
                                    select y).Count();
            if (MalePresentCount > 0)
            {
                ViewBag.malepresent = MalePresentCount;
            }


            ViewBag.total = Total;
            return View();
        }


        public ActionResult HRDashboard2()
        {
            return View();
        }

        [HttpPost]
        public JsonResult getLineChartData()
        {
            List<Company> abc = WetosDB.Companies.ToList();
            return Json(abc, JsonRequestBehavior.AllowGet);
        }
        public DataTable commonFuntionGetData(string strQuery)
        {
            SqlConnection cn = new SqlConnection();
            SqlDataAdapter dap = new SqlDataAdapter(strQuery, cn);
            DataSet ds = new DataSet();
            dap.Fill(ds);
            return ds.Tables[0];
        }
    }
}
