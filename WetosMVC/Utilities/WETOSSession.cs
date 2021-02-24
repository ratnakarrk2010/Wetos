using System;
using System.Data;
using System.Web;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace WetosMVCMainApp.Utilities
{
    public class WETOSSession
    {

        #region Declarations
        //private const string sitemappath = "SitemapPath";

        #region "For License"
        private const string nOfBranchLicense = "nOfBranchLicense";
        private const string nLicenseCompanyName = "nLicenseCompanyName";
        private const string nOfCompanyLicense = "nOfCompanyLicense";
        private const string nDateOfLicenseExpiry = "nDateOfLicenseExpiry";
        #endregion

        private const string userName = "username";
        private const string userGridData = "UserGridData";

        //private const string getAllBloodGroup = "getAllBloodGroup";
        //private const string getAllRoles = "getAllRoles";
        //private const string loginUserRights = "LoginUserRights";
        //private const string getAllCompany = "getAllCompany";
        //private const string getAllReligion = "getAllReligion";
        //private const string getAllLocation = "getAllLocation";
        //private const string getAllGrade = "getAllGrade";
        //private const string getAllGroup = "getAllGroup";
        //private const string getAllFinancialYear = "getAllFinancialYear";
        //private const string getAllEmployee = "getAllEmployee";
        //private const string getAllDesignation = "getAllDesignation";
        //private const string getAllDepartment = "getAllDepartment";
        //private const string getAllBranch = "getAllBranch";
        //private const string getAllMenus = "getAllMenus";
        //private const string getAllRights = "getAllRights";
        //private const string getAllShifts = "getAllShifts";
        //private const string getAllShiftRotations = "getAllShiftRotations";
        //private const string getAllShiftSchedule = "getAllShiftSchedule";
        //private const string getAllUsers = "getAllUsers";
        //private const string getAllEmployeeType = "getAllEmployeeTypes";
        //private const string getAllLeaveAppls = "getAllLeaveAppls";
        //private const string getAllAdjustMentEntry = "getAllAdjustMentEntry";
        //private const string getMenuSelectedIndex = "getMenuSelectedIndex";
        //private const string getAllRules = "getAllRules";
        //private const string getAllLeaveRules = "getAllLeaveRules";
        //private const string getServerErrorMsg = " getServerErrorMsg";
        //private const string getEmployeeIdList = "getEmployeeIdList";
        //private const string getSelectedBranchList = "getSelectedBranchList";
        //private const string getSelectedDivisionList = "getSelectedDivisionList";
        //private const string getSelectedDepartmentList = "getSelectedDepartmentList";
        //private const string getSelectedEmployeegroupList = "getSelectedEmployeegroupList";
        ////********** Reports ******************************
        //private const string getReportsEmployeeIdList = "getReportsEmployeeIdList";
        //private const string getReportsName = "getReportsName";
        //private const string getSelectedEmployeeId = "getSelectedEmployeeId";
        //private const string getAllNews = "getAllNews";
        //private const string getAllDevice = "getAllDevice";
        //private const string getAllDivision = "getAllDivision";

        //private const string companyId = "CompanyId";
        //private const string getallRulesforProcessdate = "GetAllRulesforProcessdate";
        //private const string getAllHoliDay = "getAllHoliDay";

        #endregion

        //#region Temp Masters
        //private const string TempgetAllDepartment = "TempgetAllDepartment";
        //private const string TempgetAllDivision = "TempgetAllDivision";
        //private const string TempgetAllReligion = "TempgetAllReligion";
        //private const string TempgetAllLocation = "TempgetAllLocation";
        //private const string TempgetAllGrade = "TempgetAllGrade";
        //private const string TempgetAllGroup = "TempgetAllGroup";
        //private const string TempgetAllFinancialYear = "TempgetAllFinancialYear";
        //private const string TempgetAllEmployee = "TempgetAllEmployee";
        //private const string TempgetAllDesignation = "TempgetAllDesignation";
        //private const string TempgetAllCompany = "TempgetAllCompany";
        //private const string TempgetAllBranch = "TempgetAllBranch";
        //private const string TempgetAllBloodGroup = "TempgetAllBloodGroup";
        //private const string TempgetAllShifts = "TempgetAllShifts";
        //private const string TempgetAllShiftRotations = "TempgetAllShiftRotations";
        //private const string TempgetAllEmployeeTypes = "TempgetAllEmployeeTypes";
        //private const string TempgetAllRules = "TempgetAllRules";
        //private const string TempgetAllLeaveRules = "TempgetAllLeaveRules";
        //#endregion

        #region temp
        public static string CountBranchLicense
        {
            set
            {
                HttpContext.Current.Session[nOfBranchLicense] = value;
            }
            get
            {
                return (string)HttpContext.Current.Session[nOfBranchLicense];
            }
        }

        public static string LicenseCompanyName
        {
            set
            {
                HttpContext.Current.Session[nLicenseCompanyName] = value;
            }
            get
            {
                return (string)HttpContext.Current.Session[nLicenseCompanyName];
            }
        }

        public static string CountCompanyLicense
        {
            set
            {
                HttpContext.Current.Session[nOfCompanyLicense] = value;
            }
            get
            {
                return (string)HttpContext.Current.Session[nOfCompanyLicense];
            }
        }

        public static string DateOfLicenseExpiry
        {
            set
            {
                HttpContext.Current.Session[nDateOfLicenseExpiry] = value;
            }
            get
            {
                return (string)HttpContext.Current.Session[nDateOfLicenseExpiry];
            }
        }


        public static string UserName
        {
            set
            {
                HttpContext.Current.Session[userName] = value;
            }
            get
            {
                return (string)HttpContext.Current.Session[userName];
            }
        }

        public static DataTable UserGridData
        {
            set
            {
                HttpContext.Current.Session[userGridData] = value;
            }
            get
            {
                return (DataTable)HttpContext.Current.Session[userGridData];
            }
        }


        //public static WETOS.DataEntity.LoginUserAccessRights LoginUserRights
        //{
        //    get
        //    {
        //        DataEntity.LoginUserAccessRights localValue = new WETOS.DataEntity.LoginUserAccessRights();
        //        localValue = (WETOS.DataEntity.LoginUserAccessRights)HttpContext.Current.Session[loginUserRights];
        //        return localValue;
        //    }
        //    set
        //    {

        //        HttpContext.Current.Session[loginUserRights] = value;
        //    }
        //}
        //public static StringBuilder SitemapPath
        //{
        //    set
        //    {
        //        HttpContext.Current.Session[sitemappath] = value;
        //    }
        //    get
        //    {
        //        return (StringBuilder)HttpContext.Current.Session[sitemappath];
        //    }
        //}
        //public static string LoginCompanyId
        //{
        //    set
        //    {
        //        HttpContext.Current.Session[companyId] = value;
        //    }
        //    get
        //    {
        //        return (string)HttpContext.Current.Session[companyId];
        //    }
        //}
        #endregion

        //#region Session For Master's

        //public static List<WETOS.DataEntity.Rules> GetAllRulesforProcessdate
        //{
        //    get
        //    {
        //        return (List<DataEntity.Rules>)HttpContext.Current.Session[getallRulesforProcessdate];
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[getallRulesforProcessdate] = value;
        //    }
        //}






        //public static List<WETOS.DataEntity.Division> GetAllDivision
        //{
        //    get
        //    {

        //        List<DataEntity.Division> localValue = (List<WETOS.DataEntity.Division>)HttpContext.Current.Session[getAllDivision];
        //        if (localValue == null)
        //        {
        //            localValue = BusinessFactory.Instance.BLDivision.GetAllDivision(WETOSSession.LoginUserRights.LocationList, WETOSSession.LoginUserRights.CompanyList, WETOSSession.LoginUserRights.BranchList, Convert.ToString(WETOSSession.LoginUserRights.RoleTypeId), Convert.ToString(WETOSSession.LoginUserRights.DivisionList));
        //        }
        //        return localValue;

        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[getAllDivision] = value;
        //    }
        //}
        //public static string GetSelectedEmployeeId
        //{
        //    set
        //    {
        //        HttpContext.Current.Session[getSelectedEmployeeId] = value;
        //    }
        //    get
        //    {
        //        return (string)HttpContext.Current.Session[getSelectedEmployeeId];
        //    }
        //}

        //public static List<WETOS.DataEntity.Employee> GetEmployeeIdList
        //{
        //    get
        //    {
        //        return (List<WETOS.DataEntity.Employee>)HttpContext.Current.Session[getEmployeeIdList];
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[getEmployeeIdList] = value;
        //    }
        //}
        //public static string GetSelectedBranchList
        //{
        //    get
        //    {
        //        return (string)HttpContext.Current.Session[getSelectedBranchList];
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[getSelectedBranchList] = value;
        //    }
        //}
        //public static string GetSelectedDivisionList
        //{
        //    get
        //    {
        //        return (string)HttpContext.Current.Session[getSelectedDivisionList];
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[getSelectedDivisionList] = value;
        //    }
        //}
        //public static string GetSelectedDepartmentList
        //{
        //    get
        //    {
        //        return (string)HttpContext.Current.Session[getSelectedDepartmentList];
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[getSelectedDepartmentList] = value;
        //    }
        //}
        //public static string GetSelectedEmployeeGroupList
        //{
        //    get
        //    {
        //        return (string)HttpContext.Current.Session[getSelectedEmployeegroupList];
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[getSelectedEmployeegroupList] = value;
        //    }
        //}
        //public static List<WETOS.DataEntity.Religion> GetAllReligion
        //{
        //    get
        //    {
        //        List<DataEntity.Religion> localValue = (List<WETOS.DataEntity.Religion>)HttpContext.Current.Session[getAllReligion];
        //        if (localValue == null)
        //        {
        //            localValue = BusinessFactory.Instance.BLReligion.GetAllReligion();
        //        }
        //        return localValue;                 
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[getAllReligion] = value;
        //    }
        //}

        //public static List<WETOS.DataEntity.Location> GetAllLocation
        //{           
        //    get
        //    {

        //        List<DataEntity.Location> localValue = (List<WETOS.DataEntity.Location>)HttpContext.Current.Session[getAllLocation];
        //        if (localValue == null)
        //        {
        //            localValue = BusinessFactory.Instance.BLLocation.GetAllLocation(ClientConstants.WetosAdminRights);
        //        }
        //        return localValue;                 

        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[getAllLocation] = value;
        //    }
        //}

        //public static List<WETOS.DataEntity.Grade> GetAllGrade
        //{            
        //    get
        //    {

        //        List<DataEntity.Grade> localValue = (List<WETOS.DataEntity.Grade>)HttpContext.Current.Session[getAllGrade];
        //        if (localValue == null)
        //        {
        //            localValue = BusinessFactory.Instance.BLGrade.GetAllGrade(WETOSSession.LoginUserRights.BranchList, WETOSSession.LoginUserRights.CompanyList);
        //        }
        //        return localValue;                 
        //    }            

        //    set
        //    {
        //        HttpContext.Current.Session[getAllGrade] = value;
        //    }
        //}

        //public static List<WETOS.DataEntity.EmployeeGroup> GetAllGroup
        //{
        //    get
        //    {
        //        return (List<WETOS.DataEntity.EmployeeGroup>)HttpContext.Current.Session[getAllGroup];
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[getAllGroup] = value;
        //    }
        //}

        //public static List<WETOS.DataEntity.FinancialYear> GetAllFinancialYear
        //{
        //    get
        //    {
        //        List<DataEntity.FinancialYear> localValue = (List<WETOS.DataEntity.FinancialYear>)HttpContext.Current.Session[getAllFinancialYear];
        //        if (localValue == null)
        //        {
        //            localValue = BusinessFactory.Instance.BLFinancialYear.GetAllFinancialYear();
        //        }
        //        return localValue;                 
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[getAllFinancialYear] = value;
        //    }
        //}

        //public static List<WETOS.DataEntity.Employee> GetAllEmployee
        //{
        //    get
        //    {
        //        return (List<WETOS.DataEntity.Employee>)HttpContext.Current.Session[getAllEmployee];
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[getAllEmployee] = value;
        //    }
        //}

        //public static List<WETOS.DataEntity.Designation> GetAllDesignation
        //{            
        //    get
        //    {

        //        List<DataEntity.Designation> localValue = (List<WETOS.DataEntity.Designation>)HttpContext.Current.Session[getAllDesignation];
        //        if (localValue == null)
        //        {
        //            localValue = BusinessFactory.Instance.BLDesignation.GetAllDesignation(WETOSSession.LoginUserRights.DepartmentList, WETOSSession.LoginUserRights.BranchList, WETOSSession.LoginUserRights.CompanyList);
        //        }
        //        return localValue;                 

        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[getAllDesignation] = value;
        //    }
        //}

        //public static List<WETOS.DataEntity.Department> GetAllDepartment
        //{            
        //    get
        //    {

        //        List<DataEntity.Department> localValue = (List<WETOS.DataEntity.Department>)HttpContext.Current.Session[getAllDepartment];
        //        if (localValue == null)
        //        {
        //            localValue = BusinessFactory.Instance.BLDepartment.GetAllDepartment(WETOSSession.LoginUserRights.LocationList, WETOSSession.LoginUserRights.CompanyList, WETOSSession.LoginUserRights.BranchList, Convert.ToString(WETOSSession.LoginUserRights.RoleTypeId), WETOSSession.LoginUserRights.DepartmentList);
        //        }
        //        return localValue;                 

        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[getAllDepartment] = value;
        //    }
        //}

        //public static List<WETOS.DataEntity.Company> GetAllCompany
        //{
        //    get
        //    {

        //        List<DataEntity.Company> localValue = (List<WETOS.DataEntity.Company>)HttpContext.Current.Session[getAllCompany];
        //        if (localValue == null)
        //        {
        //            localValue = BusinessFactory.Instance.BLCompany.GetAllCompany(WETOSSession.LoginUserRights.LocationList, WETOSSession.LoginUserRights.CompanyList,Convert.ToString(WETOSSession.LoginUserRights.RoleTypeId));
        //        }
        //        return localValue;                 

        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[getAllCompany] = value;
        //    }
        //}

        //public static List<WETOS.DataEntity.Branch> GetAllBranch
        //{
        //    get
        //    {

        //        List<DataEntity.Branch> localValue = (List<WETOS.DataEntity.Branch>)HttpContext.Current.Session[getAllBranch];
        //        if (localValue == null)
        //        {
        //            localValue = BusinessFactory.Instance.BLBranch.GetAllBranch(WETOSSession.LoginUserRights.LocationList, WETOSSession.LoginUserRights.CompanyList, WETOSSession.LoginUserRights.BranchList, Convert.ToString(WETOSSession.LoginUserRights.RoleTypeId));
        //        }
        //        return localValue;                 

        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[getAllBranch] = value;
        //    }
        //}

        //public static List<WETOS.DataEntity.BloodGroup> GetAllBloodGroup
        //{
        //    get
        //    {
        //        List<DataEntity.BloodGroup> localValue = (List<WETOS.DataEntity.BloodGroup>)HttpContext.Current.Session[getAllBloodGroup];
        //        if (localValue == null)
        //        {
        //            localValue = BusinessFactory.Instance.BLBloodGroup.GetAllBloodGroup();
        //        }
        //        return localValue;

        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[getAllBloodGroup] = value;
        //    }
        //}

        //public static List<WETOS.DataEntity.Roles> GetAllRoles
        //{
        //    get
        //    {
        //        return (List<WETOS.DataEntity.Roles>)HttpContext.Current.Session[getAllRoles];
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[getAllRoles] = value;
        //    }
        //}

        //public static List<WETOS.DataEntity.Menu> GetAllMenus
        //{
        //    get
        //    {
        //        return (List<WETOS.DataEntity.Menu>)HttpContext.Current.Session[getAllMenus];
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[getAllMenus] = value;
        //    }
        //}

        //public static List<WETOS.DataEntity.Rights> GetAllRights
        //{
        //    get
        //    {
        //        return (List<WETOS.DataEntity.Rights>)HttpContext.Current.Session[getAllRights];
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[getAllRights] = value;
        //    }
        //}

        //public static List<WETOS.DataEntity.User> GetAllUsers
        //{
        //    get
        //    {
        //        List<DataEntity.User> localValue = (List<WETOS.DataEntity.User>)HttpContext.Current.Session[getAllUsers];
        //        if (localValue == null)
        //        {
        //            localValue = BusinessFactory.Instance.BLUser.GetAllUser();
        //        }
        //        return localValue;

        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[getAllUsers] = value;
        //    }
        //}

        //public static List<WETOS.DataEntity.Shift> GetAllShifts
        //{
        //    get
        //    {
        //        return (List<WETOS.DataEntity.Shift>)HttpContext.Current.Session[getAllShifts];
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[getAllShifts] = value;
        //    }
        //}

        //public static List<WETOS.DataEntity.ShiftRotation> GetAllShiftsRotations
        //{
        //    get
        //    {
        //        return (List<WETOS.DataEntity.ShiftRotation>)HttpContext.Current.Session[getAllShiftRotations];
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[getAllShiftRotations] = value;
        //    }
        //}

        //public static List<WETOS.DataEntity.WetosAttendance> GetAllAdjustMentEntry
        //{
        //    get
        //    {
        //        return (List<WETOS.DataEntity.WetosAttendance>)HttpContext.Current.Session[getAllAdjustMentEntry];
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[getAllAdjustMentEntry] = value;
        //    }
        //}

        //public static List<WETOS.DataEntity.ShiftSchedule> GetAllShiftSchedule
        //{
        //    get
        //    {

        //        return (List<WETOS.DataEntity.ShiftSchedule>)HttpContext.Current.Session[getAllShiftSchedule];
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[getAllShiftSchedule] = value;
        //    }
        //}

        //public static List<WETOS.DataEntity.EmployeeType> GetAllEmployeeType
        //{
        //    get
        //    {
        //        List<DataEntity.EmployeeType> localValue = (List<WETOS.DataEntity.EmployeeType>)HttpContext.Current.Session[getAllEmployeeType];
        //        if (localValue == null)
        //        {
        //            localValue = BusinessFactory.Instance.BLEmployeeType.GetAllEmployeeType();
        //        }
        //        return localValue;
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[getAllEmployeeType] = value;
        //    }
        //}

        //public static List<DataEntity.LeaveOperations> GetAllLeaveAppls
        //{
        //    get
        //    {
        //        return (List<DataEntity.LeaveOperations>)HttpContext.Current.Session[getAllLeaveAppls];
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[getAllLeaveAppls] = value;
        //    }
        //}

        //public static List<DataEntity.News> GetAllNews
        //{
        //    get
        //    {
        //        return (List<DataEntity.News>)HttpContext.Current.Session[getAllNews];
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[getAllNews] = value;
        //    }
        //}


        //public static List<WETOS.DataEntity.Rules> GetAllRules
        //{
        //    get
        //    {
        //        return (List<DataEntity.Rules>)HttpContext.Current.Session[getAllRules];
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[getAllRules] = value;
        //    }
        //}
        //public static List<WETOS.DataEntity.Rules> TempGetAllRules
        //{
        //    get
        //    {
        //        return (List<DataEntity.Rules>)HttpContext.Current.Session[TempgetAllRules];
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[TempgetAllRules] = value;
        //    }
        //}
        //public static List<DataEntity.LeaveOperations> GetAllLeaveRules
        //{
        //    get
        //    {
        //        return (List<DataEntity.LeaveOperations>)HttpContext.Current.Session[getAllLeaveRules];
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[getAllLeaveRules] = value;
        //    }
        //}
        //public static List<DataEntity.LeaveOperations> TempGetAllLeaveRules
        //{
        //    get
        //    {
        //        return (List<DataEntity.LeaveOperations>)HttpContext.Current.Session[TempgetAllLeaveRules];
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[TempgetAllLeaveRules] = value;
        //    }
        //}
        //public static List<WETOS.DataEntity.Device> GetAllDevice
        //{
        //    get
        //    {

        //        List<DataEntity.Device> localValue = (List<WETOS.DataEntity.Device>)HttpContext.Current.Session[getAllDevice];
        //        if (localValue == null)
        //        {
        //            localValue = BusinessFactory.Instance.BLDevice.GetAllDevice();
        //        }
        //        return localValue;

        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[TempgetAllLocation] = value;
        //    }
        //}
        //public static List<WETOS.DataEntity.WetosAttendance> GetAllHoliDay
        //{
        //    get
        //    {
        //        return (List<DataEntity.WetosAttendance>)HttpContext.Current.Session[getAllHoliDay];
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[getAllHoliDay] = value;
        //    }
        //}

        //#endregion getAllShiftRotations


        //#region Session For Temp Master's

        //public static List<WETOS.DataEntity.Division> TempGetAllDivision
        //{
        //    get
        //    {

        //        List<DataEntity.Division> localValue = (List<WETOS.DataEntity.Division>)HttpContext.Current.Session[TempgetAllDivision];
        //        if (localValue == null)
        //        {
        //            localValue = BusinessFactory.Instance.BLDivision.GetAllDivision(ClientConstants.WetosAdminRights, ClientConstants.WetosAdminRights, ClientConstants.WetosAdminRights, Convert.ToString(WETOSSession.LoginUserRights.RoleTypeId), Convert.ToString(ClientConstants.WetosAdminRights));
        //        }
        //        return localValue;

        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[TempgetAllDivision] = value;
        //    }
        //}

        //public static List<WETOS.DataEntity.Religion> TempGetAllReligion
        //{
        //    get
        //    {
        //        List<DataEntity.Religion> localValue = (List<WETOS.DataEntity.Religion>)HttpContext.Current.Session[TempgetAllReligion];
        //        if (localValue == null)
        //        {
        //            localValue = BusinessFactory.Instance.BLReligion.GetAllReligion();
        //        }
        //        return localValue;
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[TempgetAllReligion] = value;
        //    }
        //}

        //public static List<WETOS.DataEntity.Location> TempGetAllLocation
        //{
        //    get
        //    {

        //        List<DataEntity.Location> localValue = (List<WETOS.DataEntity.Location>)HttpContext.Current.Session[TempgetAllLocation];
        //        if (localValue == null)
        //        {
        //            localValue = BusinessFactory.Instance.BLLocation.GetAllLocation(ClientConstants.WetosAdminRights);
        //        }
        //        return localValue;

        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[TempgetAllLocation] = value;
        //    }
        //}

        //public static List<WETOS.DataEntity.Grade> TempGetAllGrade
        //{
        //    get
        //    {

        //        List<DataEntity.Grade> localValue = (List<WETOS.DataEntity.Grade>)HttpContext.Current.Session[TempgetAllGrade];
        //        if (localValue == null)
        //        {
        //            localValue = BusinessFactory.Instance.BLGrade.GetAllGrade(ClientConstants.WetosAdminRights, ClientConstants.WetosAdminRights);
        //        }
        //        return localValue;
        //    }

        //    set
        //    {
        //        HttpContext.Current.Session[TempgetAllGrade] = value;
        //    }
        //}

        //public static List<WETOS.DataEntity.EmployeeGroup> TempGetAllGroup
        //{
        //    get
        //    {
        //        return (List<WETOS.DataEntity.EmployeeGroup>)HttpContext.Current.Session[TempgetAllGroup];
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[TempgetAllGroup] = value;
        //    }
        //}

        //public static List<WETOS.DataEntity.FinancialYear> TempGetAllFinancialYear
        //{
        //    get
        //    {
        //        List<DataEntity.FinancialYear> localValue = (List<WETOS.DataEntity.FinancialYear>)HttpContext.Current.Session[TempgetAllFinancialYear];
        //        if (localValue == null)
        //        {
        //            localValue = BusinessFactory.Instance.BLFinancialYear.GetAllFinancialYear();
        //        }
        //        return localValue;
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[TempgetAllFinancialYear] = value;
        //    }
        //}

        //public static List<WETOS.DataEntity.Employee> TempGetAllEmployee
        //{
        //    get
        //    {
        //        return (List<WETOS.DataEntity.Employee>)HttpContext.Current.Session[TempgetAllEmployee];
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[TempgetAllEmployee] = value;
        //    }
        //}

        //public static List<WETOS.DataEntity.Designation> TempGetAllDesignation
        //{
        //    get
        //    {

        //        List<DataEntity.Designation> localValue = (List<WETOS.DataEntity.Designation>)HttpContext.Current.Session[TempgetAllDesignation];
        //        if (localValue == null)
        //        {
        //            localValue = BusinessFactory.Instance.BLDesignation.GetAllDesignation(ClientConstants.WetosAdminRights, ClientConstants.WetosAdminRights, ClientConstants.WetosAdminRights);
        //        }
        //        return localValue;

        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[TempgetAllDesignation] = value;
        //    }
        //}

        //public static List<WETOS.DataEntity.Department> TempGetAllDepartment
        //{
        //    get
        //    {

        //        List<DataEntity.Department> localValue = (List<WETOS.DataEntity.Department>)HttpContext.Current.Session[TempgetAllDepartment];
        //        if (localValue == null)
        //        {
        //            localValue = BusinessFactory.Instance.BLDepartment.GetAllDepartment(ClientConstants.WetosAdminRights, ClientConstants.WetosAdminRights, ClientConstants.WetosAdminRights, Convert.ToString(WETOSSession.LoginUserRights.RoleTypeId), ClientConstants.WetosAdminRights);
        //        }
        //        return localValue;

        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[TempgetAllDepartment] = value;
        //    }
        //}

        //public static List<WETOS.DataEntity.Company> TempGetAllCompany
        //{
        //    get
        //    {

        //        List<DataEntity.Company> localValue = (List<WETOS.DataEntity.Company>)HttpContext.Current.Session[TempgetAllCompany];
        //        if (localValue == null)
        //        {
        //            localValue = BusinessFactory.Instance.BLCompany.GetAllCompany(ClientConstants.WetosAdminRights, ClientConstants.WetosAdminRights, Convert.ToString(WETOSSession.LoginUserRights.RoleTypeId));
        //        }
        //        return localValue;

        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[TempgetAllCompany] = value;
        //    }
        //}

        //public static List<WETOS.DataEntity.Branch> TempGetAllBranch
        //{
        //    get
        //    {

        //        List<DataEntity.Branch> localValue = (List<WETOS.DataEntity.Branch>)HttpContext.Current.Session[TempgetAllBranch];
        //        if (localValue == null)
        //        {
        //            localValue = BusinessFactory.Instance.BLBranch.GetAllBranch(ClientConstants.WetosAdminRights, ClientConstants.WetosAdminRights, ClientConstants.WetosAdminRights, Convert.ToString(WETOSSession.LoginUserRights.RoleTypeId));
        //        }
        //        return localValue;

        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[TempgetAllBranch] = value;
        //    }
        //}

        //public static List<WETOS.DataEntity.BloodGroup> TempGetAllBloodGroup
        //{
        //    get
        //    {
        //        List<DataEntity.BloodGroup> localValue = (List<WETOS.DataEntity.BloodGroup>)HttpContext.Current.Session[TempgetAllBloodGroup];
        //        if (localValue == null)
        //        {
        //            localValue = BusinessFactory.Instance.BLBloodGroup.GetAllBloodGroup();
        //        }
        //        return localValue;

        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[TempgetAllBloodGroup] = value;
        //    }
        //}

        //public static List<WETOS.DataEntity.Shift> TempGetAllShifts
        //{
        //    get
        //    {
        //        return (List<WETOS.DataEntity.Shift>)HttpContext.Current.Session[TempgetAllShifts];
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[TempgetAllShifts] = value;
        //    }
        //}

        //public static List<WETOS.DataEntity.ShiftRotation> TempGetAllShiftsRotations
        //{
        //    get
        //    {
        //        return (List<WETOS.DataEntity.ShiftRotation>)HttpContext.Current.Session[TempgetAllShiftRotations];
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[TempgetAllShiftRotations] = value;
        //    }
        //}

        //public static List<WETOS.DataEntity.EmployeeType> TempGetAllEmployeeType
        //{
        //    get
        //    {
        //        List<DataEntity.EmployeeType> localValue = (List<WETOS.DataEntity.EmployeeType>)HttpContext.Current.Session[TempgetAllEmployeeTypes];
        //        if (localValue == null)
        //        {
        //            localValue = BusinessFactory.Instance.BLEmployeeType.GetAllEmployeeType();
        //        }
        //        return localValue;
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[TempgetAllEmployeeTypes] = value;
        //    }
        //}

        //#endregion


        //public static int GetMenuSelectedIndex
        //{
        //    set
        //    {
        //        HttpContext.Current.Session[getMenuSelectedIndex] = value;
        //    }
        //    get
        //    {
        //        int localValue;
        //        localValue = Convert.ToInt32((HttpContext.Current.Session[getMenuSelectedIndex] == null) ? -1 : HttpContext.Current.Session[getMenuSelectedIndex]);
        //        if (localValue == 0)
        //        {
        //            //code for fetching menu list
        //            //localValue = BusinessFactory.Instance.BLUser.GetAllUser;
        //        }
        //        return localValue;
        //    }
        //}

        //#region IDisposable Members

        //public static void Dispose()
        //{
        //    GC.Collect();
        //    GC.RemoveMemoryPressure(GC.GetTotalMemory(true));
        //}

        //#endregion

    }
}
