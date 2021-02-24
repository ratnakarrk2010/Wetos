using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WetosDB;

namespace WetosMVCMainApp.Models
{
    /// <summary>
    /// Added by Rajas
    /// MonthlyReportsModel for reeport selection
    /// Updated by Rajas on 22 DEC 2016
    /// </summary>
    public class MonthlyReportsModel
    {
        public string RaportSelected { get; set; }

        // To select report format as excel or pdf
        public int ReportFormat { get; set; } // Added by Rajas on 14 MARCH 2017

        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0: dd-MM-yyyy}")]
        [Required(ErrorMessage = "Please select From Date")]
        [Display(Name = "From Date")]
        [DataType(DataType.DateTime)]
        public string FromDate { get; set; }

        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0: dd-MM-yyyy}")]
        [Required(ErrorMessage = "Please select To Date")]
        [Display(Name = "To Date")]
        [DataType(DataType.DateTime)]
        public string ToDate { get; set; }

        //[Display(Name = "Company")]
        public int? CompanyId { get; set; }

        //[Required(ErrorMessage = "Please select branch")]
        //[Display(Name = "Branch")]
        public int? BranchId { get; set; }

        //[Display(Name = "Department")]
        public int? DepartmentId { get; set; }
        //public string DepartmentId { get; set; }

        public string ReportName { get; set; }

        //  Added by Rajas on 6 JAN 2017 for employee selection
        //[Display(Name = "Employee Name")]
        public int? EmployeeId { get; set; } //CODE UPDATED BY SHRADDHA ON 17 MAR 2018

        // Added by Rajas on 9 FEB 2017 for selection of status according to radio button selection
        public string Status { get; set; }

        public int UserId { get; set; }  // Added by Rajas on 12 SEP 2017 for UserId

        public int? FinancialYearId { get; set; }  // Added by MSJ ON 25 DEC 2017

        public int? MonthId { get; set; }  // Added by MSJ ON 25 DEC 2017

        public int? EmployeeTypeId { get; set; } //EMPLOYEETYPE ADDED BY SHRADDHA ON 15 FEB 2018

        public string EmployeeString { get; set; }  // Added by SHRADDHA ON 17 MAR 2018
    }

    /// <summary>
    /// Added by Rajas
    /// DailyReportsModel for reeport selection
    /// Updated by Rajas on 22 DEC 2016
    /// </summary>
    public class DailyReportsModel
    {
        public string ReportSelected { get; set; }

        // To select report format as excel or pdf
        public int ReportFormat { get; set; } // Added by Rajas on 14 MARCH 2017

        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0: dd-MM-yyyy}")]
        [Required(ErrorMessage = "Please select Date")]
        [Display(Name = "From Date")]
        [DataType(DataType.DateTime)]
        public string FromDate { get; set; }

        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0: dd-MM-yyyy}")]
        [Required(ErrorMessage = "Please select To Date")]
        [Display(Name = "To Date")]
        [DataType(DataType.DateTime)]
        public string ToDate { get; set; }

        //[Display(Name = "Company")]
        public int? CompanyId { get; set; }

        //[Required(ErrorMessage = "Please select branch")]
        //[Display(Name = "Branch")]
        public int? BranchId { get; set; }

        // [Display(Name = "Department")]
        public int? DepartmentId { get; set; }

        public string ReportName { get; set; }

        //  Added by Rajas on 6 JAN 2017 for employee selection
        //[Display(Name = "Employee Name")]
        public int? EmployeeId { get; set; } // Updated by Rajas on 8 FEB 2017 string replaced with int

        // Added by Rajas on 9 FEB 2017 for selection of status according to radio button selection
        public string Status { get; set; }

        public int UserId { get; set; }  // Added by Rajas on 12 SEP 2017 for UserId

        public string EmployeeString { get; set; } // CODE ADDED BY SHRADDHA ON 17 MAR 2018

        public int? EmployeeTypeId { get; set; } //EMPLOYEETYPE ADDED BY SHRADDHA ON 15 FEB 2018
    }

    /// <summary>
    /// Added By Rajas
    /// MasterReportsModel for reeport selection
    /// Updated by Rajas on 22 DEC 2016
    /// </summary>
    public class MasterReportsModel
    {
        public string ReportsSelected { get; set; }

        // To select report format as excel or pdf
        public int ReportFormat { get; set; } // Added by Rajas on 14 MARCH 2017

        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0: dd-MM-yyyy}")]
        //[Required(ErrorMessage = "Please select From Date")]
        [Display(Name = "From Date")]
        [DataType(DataType.DateTime)]
        public string FromDate { get; set; }

        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0: dd-MM-yyyy}")]
        //[Required(ErrorMessage = "Please select To Date")]
        [Display(Name = "To Date")]
        [DataType(DataType.DateTime)]
        public string ToDate { get; set; }

        // [Display(Name = "Company")]
        public int CompanyId { get; set; }

        //[Required(ErrorMessage = "Please select branch")]
        [Display(Name = "Branch")]
        public int BranchId { get; set; }

        //[Display(Name = "Department")]
        public int DepartmentId { get; set; }

        public string ReportName { get; set; }
    }

    /// <summary>
    /// Added by Rajas on 9 SEP 2017
    /// AttendanceMail
    /// </summary>
    public class AttendanceMail
    {
        public string ToEmail { get; set; }

        public string EmployeeCode { get; set; }

        public string FileNameInAttachment { get; set; }

        public string AttachmentPath { get; set; }

        public string MailBody { get; set; }

        public int EmployeeId { get; set; }
    }

    /// <summary>
    /// AttendanceDataProcessModel
    /// Added by Rajas on 31 AUGUST 2017
    /// </summary>
    public class AttendanceDataProcessModel
    {
        [Required(ErrorMessage = "Please select From Date")]
        [Display(Name = "From Date")]
        [DataType(DataType.DateTime)]
        public DateTime FromDate { get; set; }

        [Required(ErrorMessage = "Please select To Date")]
        [Display(Name = "To Date")]
        [DataType(DataType.DateTime)]
        public DateTime ToDate { get; set; }

        //[Display(Name = "Company")]
        public int CompanyId { get; set; }

        //[Required(ErrorMessage = "Please select branch")]
        //[Display(Name = "Branch")]
        public int BranchId { get; set; }

        //[Display(Name = "Department")]
        public int DepartmentId { get; set; }

        //  Added by Rajas on 6 JAN 2017 for employee selection
        //[Display(Name = "Employee Name")]
        public int EmployeeId { get; set; } // Updated by Rajas on 8 FEB 2017 string replaced with int
    }

    /// <summary>
    /// RuleEngineModel
    /// Added by Rajas on 21 APRIL 2017
    /// </summary>
    public class RuleEngineModel
    {
        public int RuleId { get; set; }

        public int EmployeeGroupId { get; set; }

        public string Formula { get; set; }

        public string SubCode { get; set; }

        public string HeadCode { get; set; }

        public string Active { get; set; }

        public int RuleEngineId { get; set; }

        [Required(ErrorMessage = "Please Enter Rule Data Type.")]
        public string RuleType { get; set; }

        [Required(ErrorMessage = "Please Select Rule Unit.")]
        public string RuleUnit { get; set; }

        public string RuleName { get; set; }
    }

    /// <summary>
    /// DailyTransactionAttendanceStatus
    /// Added by Rajas on 27 APRIL 2017
    /// </summary>
    public class DailyTransactionAttendanceStatus
    {
        public int MonthId { get; set; }

        public int FinancialYearId { get; set; }

    }

    /// <summary>
    /// GlobalSetting
    /// Added by Rajas on 7 JUNE 2017
    /// </summary>
    public class GlobalSettingModel
    {
        public int SettingId { get; set; }

        [Required(ErrorMessage = "Please Enter Setting Text")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Setting Text cannot be longer than 255 characters.")]
        public string SettingText { get; set; }

        public int SettingType { get; set; }

        [Required(ErrorMessage = "Please Enter Setting Value")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Setting Value cannot be longer than 255 characters.")]
        public string SettingValue { get; set; }

        public GlobalSettingModel()
        {
            SettingText = string.Empty;
            SettingValue = string.Empty;
            SettingType = 1;

        }
    }

    /// <summary>
    /// PostingFlagModel
    /// Added by Rajas on 7 AUGUST 2017
    /// </summary>
    public class PostingFlagModel
    {
        public bool IsHoliDay { get; set; }
        public bool IsWeekOff { get; set; }
        public bool IsCondone { get; set; }
        public bool IsExceptionEntry { get; set; }  // Added by Rajas on 2 SEP 2017

        // Holiday
        // Added by Rajas on 19 AUGUST 2017 START
        public bool IsFullDayPresentOnHoliday { get; set; }
        public bool IsFirstHalfPresentOnHolidDay { get; set; }
        public bool IsSecondHalfPresentOnHolidDay { get; set; }

        // WO
        public bool IsFullDayPresentOnWO { get; set; }
        public bool IsFirstHalfPresentOnWO { get; set; }
        public bool IsSecondHalfPresentOnWO { get; set; }
        // Added by Rajas on 19 AUGUST 2017 END

        // Leave
        public bool IsOnLeave { get; set; }
        public bool IsFirstHalfLeave { get; set; }
        public bool IsSecondHalfLeave { get; set; }
        public bool IsFullDayLeave { get; set; }

        // OD
        public bool IsOdTour { get; set; }
        public bool IsFirstHalfOD { get; set; }
        public bool IsSecondHalfOD { get; set; }
        public bool IsFullDayOD { get; set; }

        // Due to Late
        public bool IsLate { get; set; }   // Added by Rajas on 8 AUGUST 2017
        public bool IsFirstHalfAbsentDueToLatecoming { get; set; }
        public bool IsSecondHalfAbsentDueToEarlygoing { get; set; }
        public bool IsEarly { get; set; }    // Added by Rajas on 8 AUGUST 2017
        public bool IsFullDayAbsentDuetoLate { get; set; }
        public bool IsLateCountReduced { get; set; }  // Added by Rajas on 8 AUGUST 2017

        // CO
        public bool IsFirstHalfCOff { get; set; }
        public bool IsSecondHalfCOff { get; set; }
        public bool IsFullDayCOff { get; set; }
        public bool IsCO { get; set; } //ADDED BY SHRADDHA ON 11 AUG 2017 AS PER SUGGESTED BY RAJAS


        public bool DeclaredHoliDayStatus { get; set; } //ADDED BY SHRADDHA ON 14 MAR 2018

        public PostingFlagModel()
        {
            IsHoliDay = false;
            IsWeekOff = false;
            IsCondone = false;
            IsExceptionEntry = false;  // Added by Rajas on 2 SEP 2017

            // Added by Rajas on 19 AUGUST 2017 START
            IsFullDayPresentOnHoliday = false;
            IsFirstHalfPresentOnHolidDay = false;
            IsSecondHalfPresentOnHolidDay = false;
            // Added by Rajas on 19 AUGUST 2017 END

            // Added by Rajas on 19 AUGUST 2017 START
            IsFullDayPresentOnWO = false;
            IsFirstHalfPresentOnWO = false;
            IsSecondHalfPresentOnWO = false;
            // Added by Rajas on 19 AUGUST 2017 END

            IsOnLeave = false;
            IsFirstHalfLeave = false;
            IsSecondHalfLeave = false;
            IsFullDayLeave = false;

            IsOdTour = false;
            IsFirstHalfOD = false;
            IsSecondHalfOD = false;
            IsFullDayOD = false;

            IsLate = false;
            IsFirstHalfAbsentDueToLatecoming = false;
            IsSecondHalfAbsentDueToEarlygoing = false;
            IsFullDayAbsentDuetoLate = false;
            IsEarly = false;
            IsLateCountReduced = false;

            IsFirstHalfCOff = false;
            IsSecondHalfCOff = false;
            IsFullDayCOff = false;
            IsCO = false; //ADDED BY SHRADDHA ON 11 AUG 2017 AS PER SUGGESTED BY RAJAS
            DeclaredHoliDayStatus = false;//ADDED BY SHRADDHA ON 14 MAR 2018
        }
    }

    /// <summary>
    /// Added by Rajas
    /// CommonSettingModel for tab mechanism in administartion
    /// </summary>
    public class CommonSettingModel
    {
        public LeaveMaster LeaveModel { get; set; }

        public RuleTransaction RulesTransactionModel { get; set; }

        public int CompanyId { get; set; }

        public int BranchId { get; set; }

        public int EmployeeGroupId { get; set; }

        public string Formula { get; set; }

        // ADDED BY RAJAS ON 26 DEC 2016 FOR COMMON SETTING SCREEN START

        public string GraceLateTime { get; set; }
        public int GraceLateTimeId { get; set; }

        public string GraceEarlyTime { get; set; }
        public int GraceEarlyTimeId { get; set; }

        public string LateAllowLimit { get; set; }
        public int LateAllowLimitId { get; set; }

        public string EarlyAllowLimit { get; set; }
        public int EarlyAllowLimitId { get; set; }

        public string SearchBeforeHrs { get; set; }
        public int SearchBeforeHrsId { get; set; }

        public string SearchAfterHrs { get; set; }
        public int SearchAfterHrsId { get; set; }

        public string TimeBetweenTwoSwaps { get; set; }
        public int TimeBetweenTwoSwapsId { get; set; }

        public string HalfDayLimit { get; set; }
        public int HalfDayLimitId { get; set; }

        public string FullDayLimit { get; set; }
        public int FullDayLimitId { get; set; }

        public string MinOTLimit { get; set; }
        public int MinOTLimitId { get; set; }

        public string GradeForOTAllowed { get; set; }
        public int GradeForOTAllowedId { get; set; }

        public string MinLimitForHalfDayCompoff { get; set; }
        public int MinLimitForHalfDayCompoffId { get; set; }

        public string OTStartAfter { get; set; }
        public int OTStartAfterId { get; set; }

        public string MinLimitForFullDayCompoff { get; set; }
        public int MinLimitForFullDayCompoffId { get; set; }

        public string LateCountInMonth { get; set; }
        public int LateCountInMonthId { get; set; }

        public string NoDaysDeduct { get; set; }
        public int NoDaysDeductId { get; set; }

        public bool DeductFrmAttendance { get; set; }
        public int DeductFrmAttendanceId { get; set; } // Updated by Rajas on 30 MAY 2017

        public string GradeForWhichLateDeductNotDone { get; set; }
        public int GradeForWhichLateDeductNotDoneId { get; set; }

        public string GradeForCompoffAllowedId { get; set; } // Updated by Rajas on 30 MAY 2017
        public int GradeForCompoffAllowed { get; set; } // Updated by Rajas on 30 MAY 2017

        public string CheckMonthStart { get; set; }
        public int CheckMonthStartId { get; set; }

        public string CheckDays { get; set; }
        public int CheckDaysId { get; set; }

        public string LeaveCode { get; set; }

        public string Name { get; set; }

        public string LeaveDescription { get; set; }

        public string LeaveType { get; set; }

        public string NoOfDaysAllowedInYear { get; set; }

        public string MaxNoOfTimesAllowedInYear { get; set; }

        public string MaxNoOfLeavesAllowedAtaTime { get; set; }

        public string MaxNoOfLeavesAllowedInMonth { get; set; }

        public string MinNoofLeaveAllowedatatime { get; set; }

        public string MaxAccumulationDays { get; set; }

        public bool EncashmentAllowedOrNot { get; set; }

        public bool NegativeBalanceAllowed { get; set; }

        public bool ISHalfAllowed { get; set; }

        public bool IsCarryForword { get; set; }

        public bool WOBetLevConsiderLeave { get; set; }

        public bool HolidayBetLevConsiderLeave { get; set; }

        public CommonSettingModel()
        {
            GraceLateTimeId = 4;
            GraceEarlyTimeId = 3;
            LateAllowLimitId = 5;
            EarlyAllowLimitId = 6;
            SearchBeforeHrsId = 1;
            SearchAfterHrsId = 2;
            TimeBetweenTwoSwapsId = 18;
            HalfDayLimitId = 7;
            FullDayLimitId = 21;
            MinOTLimitId = 8;
            GradeForOTAllowedId = 29;
            MinLimitForHalfDayCompoffId = 11;
            OTStartAfterId = 28;
            MinLimitForFullDayCompoffId = 10;
            LateCountInMonthId = 20;
            NoDaysDeductId = 17;
            //GradeForWhichLateDeductNotDoneId = 
            //GradeForCompoffAllowed = 27;
            CheckMonthStartId = 25;
            CheckDaysId = 22;

            // GraceLateTime = "00:30:00";
            GraceEarlyTime = "00:30:00";
            GraceLateTime = "0";
        }

        // ADDED BY RAJAS ON 26 DEC 2016 FOR COMMON SETTING SCREEN END

    }
    public class UserInfo
    {
        public string UserName { get; set; }
        public int UserId { get; set; }
        public int EmpId { get; set; }
        public string EmpCode { get; set; }
        public string LoginName { get; set; }
        public string imgpath { get; set; }
        public string UserRole { get; set; }
        public int GroupId { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class YearInfo
    {
        public int y_id { get; set; }
        public string period { get; set; }
        public DateTime fm_date { get; set; }
        public DateTime to_date { get; set; }
        public int next_year { get; set; }
        public int previous_year { get; set; }
        public int c_id { get; set; }

        public string FromDateSave
        {
            get
            {
                return String.Format("{0:yyyy-MM-dd}", fm_date);
            }
            set
            {
                fm_date = Convert.ToDateTime(value);
            }
        }

        public string ToDateSave
        {
            get
            {
                return String.Format("{0:yyyy-MM-dd}", to_date);
            }
            set
            {
                to_date = Convert.ToDateTime(value);
            }
        }

        /// <summary>
        /// From Date in {dd-MM-yyyy} format
        /// </summary>
        public string FromDate
        {
            get
            {
                return String.Format("{0:dd-MM-yyyy}", fm_date);
            }
        }

        /// <summary>
        /// To Date in {dd-MM-yyyy} format
        /// </summary>
        public string ToDate
        {
            get
            {
                return String.Format("{0:dd-MM-yyyy}", to_date);
            }
        }
    }

    /// <summary>
    /// Model LockUnlockData 
    /// Added by Rajas on 9 MARCH 2017
    /// Administration data lock/unlock
    /// </summary>
    public class LockUnlockData
    {
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0: dd-MM-yyyy}")]
        //[Required(ErrorMessage = "Please select From Date")]
        //[Display(Name = "From Date")]
        //[DataType(DataType.DateTime)]
        [Required(ErrorMessage = "Please select From Date"), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0: dd-MM-yyyy}")]
        public DateTime FromDate { get; set; }

        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0: dd-MM-yyyy}")]
        //[Required(ErrorMessage = "Please select To Date")]
        //[Display(Name = "To Date")]
        //[DataType(DataType.DateTime)]
        [Required(ErrorMessage = "Please select To Date"), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0: dd-MM-yyyy}")]
        public DateTime ToDate { get; set; }

        [Required(ErrorMessage = "Please select Company")]
        [Display(Name = "Company")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "Please select branch")]
        [Display(Name = "Branch")]
        public int BranchId { get; set; }

        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

        //[Display(Name = "Employee Name")]
        public string EmployeeId { get; set; }

        public int EmployeeTypeId { get; set; }

        public string RadioButtonSelected { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public LockUnlockData()
        {

        }
    }

    /// <summary>
    /// NewsModel 
    /// Added by Rajas on 10 APRIL 2017
    /// </summary>
    public class NewsModel
    {
        public int NewsId { get; set; }

        public string Type { get; set; }

        public string Info { get; set; }

        public DateTime NewsDate { get; set; }

        public int? CompanyId { get; set; }

        public int? BranchId { get; set; }

        public int? EmployeeId { get; set; }

        public int? EmployeeGroupId { get; set; }

        public string NewsFor { get; set; }

        public bool Active { get; set; }

        public DateTime? NewsEndDate { get; set; }

        public NewsModel()
        {
            NewsDate = DateTime.Now;

            NewsEndDate = DateTime.Now;

            BranchId = 0;

            EmployeeGroupId = 0;

            EmployeeId = 0;
        }
    }

    /// <summary>
    /// ShiftScheduleModel
    /// Added by Rajas on 11 APRIL 2017
    /// </summary>
    public class ShiftScheduleModel
    {
        public int ShiftScheduleId { get; set; }

        public int RotationId { get; set; }

        public int EmployeeId { get; set; }

        public int? EmployeeGroupId { get; set; }

        public int ShiftMonth { get; set; }

        public int ShiftYear { get; set; }

        public string Day1 { get; set; }

        public string Day2 { get; set; }

        public string Day3 { get; set; }

        public string Day4 { get; set; }

        public string Day5 { get; set; }

        public string Day6 { get; set; }

        public string Day7 { get; set; }

        public string Day8 { get; set; }

        public string Day9 { get; set; }

        public string Day10 { get; set; }

        public string Day11 { get; set; }

        public string Day12 { get; set; }

        public string Day13 { get; set; }

        public string Day14 { get; set; }

        public string Day15 { get; set; }

        public string Day16 { get; set; }

        public string Day17 { get; set; }

        public string Day18 { get; set; }

        public string Day19 { get; set; }

        public string Day20 { get; set; }

        public string Day21 { get; set; }

        public string Day22 { get; set; }

        public string Day23 { get; set; }

        public string Day24 { get; set; }

        public string Day25 { get; set; }

        public string Day26 { get; set; }

        public string Day27 { get; set; }

        public string Day28 { get; set; }

        public string Day29 { get; set; }

        public string Day30 { get; set; }

        public string Day31 { get; set; }

        public int CompanyId { get; set; }

        public int BranchId { get; set; }

        public ShiftScheduleModel()
        {
            RotationId = 0;
        }
    }


    /// <summary>
    /// ADDED CODE BY MSJ ON 01 NOV 2017 
    /// </summary>
    public class ReportModel
    {
        public int ReportFormat { get; set; }
        public int? CompanyId { get; set; }
        public int? BranchId { get; set; }
        public int? DepartmentId { get; set; }
        public string ReportName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }


    /// <summary>
    /// ADDED BY SHRADDHA ON 01 DEC 2017 FOR PROCESS ATTENDANCE
    /// </summary>
    public class PostingAttendanceModel
    {
        public int? EmployeeId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public PostingAttendanceModel()
        {

        }
    }

    /// <summary>
    /// ADDED BY SHRADDHA ON 22 FEB 2018
    /// </summary>
    public class AssignHRManagerModel
    {
        [Required(ErrorMessage = "Please select Employee from list.")]
        public int EmployeeId { get; set; }
        [Required(ErrorMessage = "Please select Company from list.")]
        public string CompanyId { get; set; }
        [Required(ErrorMessage = "Please select Branch from list.")]
        public string BranchId { get; set; }
        
        public string DepartmentId { get; set; }
        public int RoleId { get; set; }
        public List<SP_BranchAdminList_Result> BranchAdminList { get; set; }

        public AssignHRManagerModel()
        {

        }
    }

    public class EmployeeReportsModel
    {
        public string RaportSelected { get; set; }

        // To select report format as excel or pdf
        public int ReportFormat { get; set; } // Added by Rajas on 14 MARCH 2017

        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0: dd-MM-yyyy}")]
        [Required(ErrorMessage = "Please select From Date")]
        [Display(Name = "From Date")]
        [DataType(DataType.DateTime)]
        public string FromDate { get; set; }

        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0: dd-MM-yyyy}")]
        [Required(ErrorMessage = "Please select To Date")]
        [Display(Name = "To Date")]
        [DataType(DataType.DateTime)]
        public string ToDate { get; set; }

        //[Display(Name = "Company")]
        public int? CompanyId { get; set; }

        //[Required(ErrorMessage = "Please select branch")]
        //[Display(Name = "Branch")]
        public int? BranchId { get; set; }

        //[Display(Name = "Department")]
        public int? DepartmentId { get; set; }

        public string ReportName { get; set; }

        //  Added by Rajas on 6 JAN 2017 for employee selection
        //[Display(Name = "Employee Name")]
        public int? EmployeeId { get; set; } //CODE UPDATED BY SHRADDHA ON 17 MAR 2018

        // Added by Rajas on 9 FEB 2017 for selection of status according to radio button selection
        public string Status { get; set; }

        public int UserId { get; set; }  // Added by Rajas on 12 SEP 2017 for UserId

        public int? FinancialYearId { get; set; }  // Added by MSJ ON 25 DEC 2017

        public int? MonthId { get; set; }  // Added by MSJ ON 25 DEC 2017

        public int? EmployeeTypeId { get; set; } //EMPLOYEETYPE ADDED BY SHRADDHA ON 15 FEB 2018

        public string EmployeeString { get; set; }  // Added by SHRADDHA ON 17 MAR 2018
    }

    public class ReligionModel //Added by dhanashri on 3 june 2019 Start
    {
        public int ReligionId { get; set; }

        public string ReligionName { get; set; }
    }
    //Added by dhanashri on 3 june 2019 End


    public class LoginUserAccessRights
    {

        # region Declaration
        private int _userId;
        private int _employeeId;
        private int _roleTypeId;
        private string _UseraRoleName, _employeeName;
        private string _divisionList;
        private string _locationList;
        private string _companyList;
        private string _branchList;
        private string _departmentList;
        private string _menuIdList;
        private string _mastersRights;
        private string _attendanceRights;
        private string _applicationsRights;
        private string _sanctionsRights;
        private string _reportsRights;
        private string _administrationRights;
        private string _updatesRights;
        private int _branchId;
        private int _companyId;
        private int _reportingPersonId;
        private int _employeeGrpId;
        private string _UnderObservationemployeeidList;//Employeeid's which are under observation of LoginUser 
        private string _reportingPersonName;
        private string _centerList;
        private string _password;
        private int _divisionId;
        private int _departmentId;
        #endregion

       
        public int UserId
        {
            get
            {
                return _userId;
            }
            set
            {
                _userId = value;
            }
        }
      
        public int EmployeeId { get { return _employeeId; } set { _employeeId = value; } }
      
        public int RoleTypeId { get { return _roleTypeId; } set { _roleTypeId = value; } }
      
        public string UnderObservationemployeeidList { get { return _UnderObservationemployeeidList; } set { _UnderObservationemployeeidList = value; } }
        
        public string ReportingPersonName { get { return _reportingPersonName; } set { _reportingPersonName = value; } }
        
        public int ReportingPersonId { get { return _reportingPersonId; } set { _reportingPersonId = value; } }
        
        public int DivisionId { get { return _divisionId; } set { _divisionId = value; } }
        
        public int DepartmentId { get { return _departmentId; } set { _departmentId = value; } }

        
        public int EmployeeGrpId { get { return _employeeGrpId; } set { _employeeGrpId = value; } }
        
        public string UseraRoleName { get { return _UseraRoleName; } set { _UseraRoleName = value; } }
        
        public string LocationList { get { return _locationList; } set { _locationList = value; } }
        
        public string CompanyList { get { return _companyList; } set { _companyList = value; } }
        
        public string BranchList { get { return _branchList; } set { _branchList = value; } }
        
        public string DepartmentList { get { return _departmentList; } set { _departmentList = value; } }
        
        public string DivisionList { get { return _divisionList; } set { _divisionList = value; } }

        
        public string MenuIdList { get { return _menuIdList; } set { _menuIdList = value; } }
        
        public string MasterMenuRights { get { return _mastersRights; } set { _mastersRights = value; } }
        
        public string AttendanceMenuRights { get { return _attendanceRights; } set { _attendanceRights = value; } }
        
        public string ApplicationMenuRights { get { return _applicationsRights; } set { _applicationsRights = value; } }
        
        public string SanctionMenuRights { get { return _sanctionsRights; } set { _sanctionsRights = value; } }
        
        public string ReportMenuRights { get { return _reportsRights; } set { _reportsRights = value; } }
        
        public string AdministrationMenuRights { get { return _administrationRights; } set { _administrationRights = value; } }
        
        public string UpdatesMenuRights { get { return _updatesRights; } set { _updatesRights = value; } }
        
        public int BranchId { get { return _branchId; } set { _branchId = value; } }
        
        public int CompanyId { get { return _companyId; } set { _companyId = value; } }
        
        public string EmployeeName { get { return _employeeName; } set { _employeeName = value; } }
        
        public string CenterList { get { return _centerList; } set { _centerList = value; } }
        public string Password { get { return _password; } set { _password = value; } }
    }
}
