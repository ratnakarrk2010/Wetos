using System;

namespace WetosMVCMainApp.Utilities
{
    public sealed class ClientConstants
    {
        #region //Id and Name
        public const int ParentId = 0;
        public const string DepartmentId = "DepartmentId";
        public const string DepartmentName = "DepartmentName";
        public const string BranchId = "BranchId";
        public const string BranchName = "BranchName";
        public const string CompanyId = "CompanyId";
        public const string CompanyName = "CompanyName";
        public const string LocationId = "LocationId";
        public const string LocationName = "LocationName";
        public const string RoleId = "RoleId";
        public const string RoleName = "RoleName";
        public const string RightsId = "RightsId";
        public const string RightsName = "RightsName";
        public const string DesignationId = "DesignationId";
        public const string DivisionId = "DivisionId";
        public const string DivisionName = "DivisionName";
        public const string DesignationName = "DesignationName";
        public const string GradeId = "GradeId";
        public const string GradeName = "GradeName";
        public const string ReligionId = "ReligionId";
        public const string ReligionName = "ReligionName";
        public const string FinancialId = "FinancialId";
        public const string FinancialName = "FinancialName";
        public const string BloodGroupId = "BloodGroupId";
        public const string BloodGroupName = "BloodGroupName";
        public const string ShiftId = "ShiftId";
        public const string ShiftName = "ShiftName";
        public const string RotationId = "RotationId";
        public const string RotationName = "RotationName";
        public const string StatusId = "StatusId";
        public const string StatusName = "StatusName";
        public const string EmployeeId = "EmployeeId";
        public const string EmployeeCode = "EmployeeCode";
        public const string EmployeeTypeId = "EmployeeTypeId";
        public const string EmployeeTypeName = "EmployeeTypeName";
        public const string EmployeeName = "FullNameWithEmployeeCode";
        public const string EmpCodeAndName = "EmpCodeAndName";
        public const string EmployeeGroupId = "EmployeeGroupId";
        public const string EmployeeGroupName = "EmployeeGroupName";
        public const string GroupName = "Group Name";
        public const string SearchBy_EmployeeCode = "Employee Code";
        public const string LeaveCode = "LeaveType";
        public const string LeaveCredit = "LEAVECREDIT";
        public const string LeaveDebit = "LEAVEDEBIT";
        #endregion

        #region //Date
        public const string DefultDate = "01/01/1900";
        public const string sqlDefultDate = "01/01/1900";
        public const string oracleDefultDate = "01/01/0001";
        public const string DefultTime = "00:00:00";
        public const double Zero = 0;
        public static DateTime ZEROTIME;
        public const string DateTimeFoemate = "dd/MM/yyyy hh:mm:ss";
        #endregion

        #region //ALL,SELECT & NULL
        public const string ALL = "ALL";
        public const string SELECT = "SELECT";
        public const string NULL = "NULL";
        public const string NULL_BLANK = "";
        public const string BLANK = "---";
        public const string BLANKVAL = "-";
        public const string NegationString = "-1";
        public const int Negation = -1;
        public const int IntDefaultId = 0;
        #endregion

        #region //WetosAdmin
        public const string MenuList = "1,2,3,4,5,6,7,8";
        public const string WetosAdminRights = "WA";
        public const string WetosAdminSanctionRights = "1,2,3,4,5,6,7,8,9";
        public const int WetosAdminReportingId = 0;
        #endregion

        #region //----- MenuSelectedIndex --------------
        public const Int32 MASTERS = 0;
        public const Int32 ATTENDANCE = 1;
        public const Int32 APPLICATIONS = 2;
        public const Int32 SANCTIONS = 3;
        public const Int32 REPORTS = 4;
        public const Int32 ADMINISTRATION = 5;
        public const Int32 UPDATES = 6;
        public const Int32 EXIT = 7;
        #endregion  //----MenuSelectedIndex ----------------

        #region// ---- Select Criteria -----------------
        public const string BranchWise = "Branch Wise";
        public const string EmployeeWise = "Employee Wise";
        public const string LeaveType = "Leave Type";
        #endregion// ---- Select Criteria -----------------

        #region//// ---- Leave Types ---------------------
        public const string PL = "PL";
        public const string CL = "CL";
        public const string SL = "SL";
        public const string LWP = "LWP";
        public const string ML = "ML";
        public const string OH = "OH";
       // public const string 
        #endregion//// ---- Leave Types ---------------------

        #region // ---- Holiday Type -----------------
        public const string NationalHoliday = "National Holiday";
        public const string OptionalHoliday = "Optional Holiday";
        public const string PublicHoliday = "Public Holiday";
        public const string ReligiousHoliday = "Religious Holiday";
        #endregion //----- Holiday Type -----------------

        #region  //For Day Status(Attendance)----------------------------------
        
        public const string FullDay = "Full Day";
        public const string HalfDay = "Half Day";
        public const string FirstHalf = "First Half";
        public const string SecondHalf = "Second Half";
        public const string AdjustmentEntryStatus = "AD";
        #endregion//For Day Status ----------------------------------

        #region  //For Day Status(OD/Tour) ----------------------------------
        public const string OD = "OD";
       
        public const string TOUR = "Tour";
        public const string Late="Late";
        public const string Early="Early";
        public const string TOURForEMAIL = "TOUR";
        #endregion//For Day Status ----------------------------------

        #region //Application Pending,Approve,Sanction and Reject
        public const string ApplicationPendingStatus = "P";
        public const string ApplicationApproveStatus = "A";
        public const string ApplicationSanctionStatus = "S";
        public const string ApplicationRejectStatus = "R";
        public const string ApplicationCancelStatus = "C";
        public const string CancelApplicationStatus = "CA";
        public const string ApplicationPendingName = "Pending for Approval";
        public const string ApplicationApproveName = "Approved";
        public const string ApplicationSanctionName = "Sanctioned";
        public const string ApplicationRejectName = "Rejected";
        public const string ApplicationCancelName = "Cancel";
        public const string ApplicationCancel = "Pending for Cancellation";
        #endregion

        #region// *************** Attendance Constant ********************************
        public const string PrsesentStatus = "PPPP";
        public const string AdjPrsesentStatus = "PPPP*";
        public const string AbsentStatus = "AAAA";
        public const string FirstHalfAbsent = "AAPP";
        public const string SecondHalfAbsents = "PPAA";
        public const string HalfDayAbsent = "AA";
        #endregion// *************** Ateendance Constant ********************************

        //Values For Search  dropdown 

        public const string Division_Name_Wise = "Division Name";
        public const string Department_Code_Wise = "Department Code";
        public const string Department_Name_Wise = "Department Name";
        public const string Branch_Name_Wise = "Branch Name";
        public const string Company_Name_Wise = "Company Name";

        #region//Access Rights
        public const char ApproveRights = '5';
        public const char SanctionRights = '6';
        public const char RejectRights = '7';
        #endregion

        #region //Shift Swap Application Type
        public const string Swap = "S";
        public const string Swapping = "Swap Shift";
        public const string Replace = "R";
        public const string Replacement = "Replace Shift";
        #endregion

        #region //For Shift Scheduling
        public const string Day1 = "Day1";
        public const string Day2 = "Day2";
        public const string Day3 = "Day3";
        public const string Day4 = "Day4";
        public const string Day5 = "Day5";
        public const string Day6 = "Day6";
        public const string Day7 = "Day7";
        public const string Day8 = "Day8";
        public const string Day9 = "Day9";
        public const string Day10 = "Day10";
        public const string Day11 = "Day11";
        public const string Day12 = "Day12";
        public const string Day13 = "Day13";
        public const string Day14 = "Day14";
        public const string Day15 = "Day15";
        public const string Day16 = "Day16";
        public const string Day17 = "Day17";
        public const string Day18 = "Day18";
        public const string Day19 = "Day19";
        public const string Day20 = "Day20";
        public const string Day21 = "Day21";
        public const string Day22 = "Day22";
        public const string Day23 = "Day23";
        public const string Day24 = "Day24";
        public const string Day25 = "Day25";
        public const string Day26 = "Day26";
        public const string Day27 = "Day27";
        public const string Day28 = "Day28";
        public const string Day29 = "Day29";
        public const string Day30 = "Day30";
        public const string Day31 = "Day31";
        #endregion

        #region //RoleType
        public const int UserRoleTypeId = 6;
        public const int ReportMenuid = 5;
        #endregion

        #region //Authority Modes
        public enum Mode
        {
            Add = 1,
            Save,
            Edit,
            Update,
            Delete,
            View,
            Cancel,
            OnlyView
        }
        #endregion

        #region //News MessageDisplay
        public const string BirthDayHeader = "BIRTHDAY LIST ";
        public const string WeddingHeader = "WEDDING ANNIVERSARY LIST ";
        public const string BirthDayMessagae = "Wish You Happy BirthDay !!!";
        public const string AnniversaryMessagae = "Wish You Happy Anniversary !!!";
        public const string Announcement = "ANNOUNCEMENT ";
        public const string Meeting = "MEETING ";

        public const string CompanyWiseNews = "C";
        public const string BranchWiseNews = "B";
        public const string EmployeeGroupWiseNews = "EG";
        public const string EmployeeWiseNews = "E";
        #endregion

        #region //mailSubjects
        //Application
        public const string LeaveApplication = "Leave Application";
        public const string ExceptionApplication = "Did Not Swipe Application";
        public const string CompOffApplication = "CompOff Application";
        public const string ODTourApplication = "Out Door Duty";
        public const string ODApplicationForEmail = "Out Door Duty Application";
        public const string ShiftSwappingApplication = "Shift Swapping Application";
        public const string LeaveEncashmentApplication = "Leave Encashment Application";
        public const string ShiftSwapping = "Shift Swapping";
        public const string ShiftReplacement = "Shift Replacement";

        //Sanction
        public const string LeaveSanction = "Leave Sanction";
        public const string CompOffSanction = "CompOff Sanction";
        public const string ODTourSanction = "ODTour Sanction";
        public const string ExceptionSanction = "ExceptionEntry Sanction";
        public const string ShiftSwappingSanction = "Shift Swapping Sanction";
        public const string OutDoorSanction = "Sanction of Out Door Duty Application";


        //Rejection
        public const string LeaveRejection = "Leave Rejection";
        public const string CompOffRejection = "CompOff Rejection";
        public const string ODTourRejection = "ODTour Rejection";
        public const string ExceptionRejection = "ExceptionEntry Reject";
        public const string ExceptionRejectionnew = "Did Not Swipe Rejection";
        public const string ShiftSwappingRejection = "Shift Swapping Rejection";
        public const string ErrorMail = "Error Message Mail";

        //Approve
        public const string LeaveApprove = "Leave Approve";
        public const string ODTourApprove = "ODTour Approve";
        public const string CompOffApprove = "CompOff Approve";
        public const string ExceptionApprove = "ExceptionEntry Approve";

        #endregion

        #region //Activate and De-Activate
        public const string ActiveUser = "A";
        public const string DeActiveUser = "D";
        #endregion

        #region String True & False Constants For Bool
        public const string False = "0";
        public const string True = "1";
        #endregion

        #region Report Formats

        public const string PDF = "PDF";
        public const string EXCEL = "EXCEL";
        public const string DOC = "DOC";
        public const string RTF = "RTF";

        #endregion

        #region Shift Codes
        public const string WW = "WO";
        public const string HH = "HH";
        public const string WP = "WP";
        public const string HP = "HP";
        public const string Star = "*";
        #endregion


        public static string ShortDateFormat = "dd/MM/yyyy";
        public static string ShortTimeFormate = "00:00:00";
        public static string LongDateFormat = "dd/MM/yyyy H:mm:ss tt";
        public static string ExcelExtention = ".xls";
        public static string CSVExtention = ".csv";
        public static string space = "&nbsp;";
        public static string Remark = "Balance CompOff hrs is ";
        public static string TEXTExtention = ".txt";
        public const string Pending = "P";
        public const string MonthStart = "26";
        public const string MonthEnd = "25";


        //
        #region Error Message from inner layers
        public const string EmailError = "Failure sending mail.";
        public const string SQLConstraintError = "REFERENCE constraint";
        #endregion
      
       

        #region Error Message Mail

        public static string CompanyNameForMail = System.Configuration.ConfigurationManager.AppSettings["CompanyName"];
        public static string ClientCompanyNameForMail = WETOSSession.LicenseCompanyName;
        public static string ErrorMessageFromMail = System.Configuration.ConfigurationManager.AppSettings["SMTPUsername"];
        public static string ErrorMessageToMail = System.Configuration.ConfigurationManager.AppSettings["ErrorMessageToMail"];
        public static string ErrorMessageCCMail = System.Configuration.ConfigurationManager.AppSettings["ErrorMessageCCMail"];
        public static string ErrorMessageTo = System.Configuration.ConfigurationManager.AppSettings["ErrorMessageTo"];


        #endregion 

        #region  Excel Sheet Type
        public static string ImportEmployee = "Employee Details";
        public static string WeekOff = "Week Off";
        public static string LeaveBalance = "Leave Balance";
        public static string ShiftShedule = "Shift Shedule";



        #endregion

        #region Lock/Unlock
        public const string Lock = "Lock";
        public const string Unlock = "Unlock";
        public const string LockAll = "LockALL";
        public const string UnlockAll = "UnlockAll";
        public const string LockStatus = "Y";
        public const string UnlockStatus = "N";

        #endregion

        #region LeaveConstant
        public const int PreviousBalance = 1;
        public const int CurrentBalance = 1;
        public const string CompOff = "CO";
        #endregion

        public static string ExcludedFactoryBranch = System.Configuration.ConfigurationManager.AppSettings["ExcludedFactoryBranch"].ToString();
    }
}
