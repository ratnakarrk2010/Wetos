using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WetosDB;
using WetosMVCMainApp.Models;


public static class SessionPersister
{
    private static string usernameSessionVar = "Username";
    private static string roleinfoSessionVarmenu = "session_roleinfomenu";
    private static string userinfoSessionVar = "session_userinfo";
    private static string lastloginSessionVar = "session_lastlogin";
    private static string yearinfosessionvar = "session_yearinfo";
    //private static string companyinfoSessionVar = "session_companyinfo";
    //private static string BranchInFoSessionVar = "session_BranchInFo";
    //private static string RoleInfoSessionvar = "session_roleinfo";
    //private static string SysInfoSessionVar = "session_sysinfo";
    //private static string actionlogSessionVar = "session_actionlog";
    //private static string actionuserSessionVar = "session_actionuser";

    //Added by PUSHKAR ON 3 APRIL 2017 for series from session**************************************************START
    public static string seriesSessionVar = "series";


    /// <summary>
    /// Series
    /// </summary>
    public static string Series
    {
        get
        {
            if (HttpContext.Current == null) return string.Empty;

            var sessionVar = HttpContext.Current.Session[seriesSessionVar];

            if (sessionVar != null)
                return sessionVar as string;

            return null;
        }

        set
        {
            HttpContext.Current.Session[seriesSessionVar] = value;
        }
    }


    //Added by PUSHKAR ON 3 APRIL 2017 for series from session**************************************************END

    public static List<sp_get_user_role_menu_Result> roleInfo
    {
        get
        {
            if (HttpContext.Current == null) return null;

            var sessionVar =
                HttpContext.Current.Session[roleinfoSessionVarmenu];
            if (sessionVar != null)
                return sessionVar as List<sp_get_user_role_menu_Result>;
            return null;
        }
        set
        {
            HttpContext.Current.Session[roleinfoSessionVarmenu] = value;
        }
    }


    //public static List<MarsUserData.vUserFormActionRight> ActionUserInfo
    //{
    //    get
    //    {
    //        if (HttpContext.Current == null) return null;
    //        var sessionActionVar =
    //            HttpContext.Current.Session[actionuserSessionVar];
    //        if (sessionActionVar != null)
    //            return sessionActionVar as List<MarsUserData.vUserFormActionRight>;
    //        return null;
    //    }
    //    set
    //    {
    //        HttpContext.Current.Session[actionuserSessionVar] = value;
    //    }
    //}

    //COMMENTED BY SHRADDHA ON 03 NOV 2017 BECAUSE THIS IS REDUNDANT AND NOT REQUIRED VARIABLE CODE START
    //public static string Username
    //{
    //    get
    //    {
    //        if (HttpContext.Current == null) return string.Empty;

    //        var sessionVar =
    //            HttpContext.Current.Session[usernameSessionVar];
    //        if (sessionVar != null)
    //            return sessionVar as string;
    //        return null;
    //    }
    //    set
    //    {
    //        HttpContext.Current.Session[usernameSessionVar] = value;
    //    }
    //}
    //COMMENTED BY SHRADDHA ON 03 NOV 2017 BECAUSE THIS IS REDUNDANT AND NOT REQUIRED VARIABLE CODE END

    public static string UserName
    {
        get
        {
            if (HttpContext.Current == null) return string.Empty;

            var sessionVar =
                HttpContext.Current.Session[usernameSessionVar];
            if (sessionVar != null)
                return sessionVar as string;
            return null;
        }
        set
        {
            HttpContext.Current.Session[usernameSessionVar] = value;
        }
    }


    //public static MarsUserData.AppUser UserInfo
    //{
    //    get
    //    {
    //        if (HttpContext.Current == null) return null;

    //        var sessionVar =
    //            HttpContext.Current.Session[userinfoSessionVar];
    //        if (sessionVar != null)
    //            return sessionVar as MarsUserData.AppUser;
    //        return null;
    //    }
    //    set
    //    {
    //        HttpContext.Current.Session[userinfoSessionVar] = value;
    //    }
    //}

    //public static MarsUserData.sysglobal SysGlobalInfo
    //{
    //    get {
    //        if (HttpContext.Current == null) return null;

    //        var sessionvar =
    //            HttpContext.Current.Session[SysInfoSessionVar];

    //        if (sessionvar != null)
    //            return sessionvar as MarsUserData.sysglobal;

    //        return null;

    //    }
    //    set
    //    {
    //        HttpContext.Current.Session[SysInfoSessionVar] = value;
    //    }
    //}

    //public static BranchInfo BranchInFo
    //{
    //    get
    //    {
    //        if (HttpContext.Current == null) return null;

    //        var sessionVar =
    //            HttpContext.Current.Session[BranchInFoSessionVar];
    //        if (sessionVar != null)
    //            return sessionVar as BranchInfo;
    //        return null;
    //    }
    //    set
    //    {
    //        HttpContext.Current.Session[BranchInFoSessionVar] = value;
    //    }
    //}


    public static string LastLogin
    {
        get
        {
            if (HttpContext.Current == null) return null;

            var sessionVar =
                HttpContext.Current.Session[lastloginSessionVar];
            if (sessionVar != null)
                return sessionVar as string;
            return null;
        }
        set
        {
            HttpContext.Current.Session[lastloginSessionVar] = value;
        }
    }

    public static YearInfo YearInfo
    {
        get
        {
            if (HttpContext.Current == null) return null;

            var sessionVar =
                HttpContext.Current.Session[yearinfosessionvar];
            if (sessionVar != null)
                return sessionVar as YearInfo;
            return null;
        }
        set
        {
            HttpContext.Current.Session[yearinfosessionvar] = value;
        }
    }

    //public static ActionInfo ActionInfo
    //{
    //    get
    //    {
    //        if (HttpContext.Current == null) return null;

    //        var actionvar =
    //            HttpContext.Current.Session[actionlogSessionVar];
    //        if (actionvar != null)
    //            return actionvar as Mars.Models.ActionInfo;
    //        return null;
    //    }
    //    set
    //    {
    //        HttpContext.Current.Session[actionlogSessionVar] = value;
    //    }
    //}

    public static UserInfo UserInfo
    {
        get
        {
            if (HttpContext.Current == null) return null;

            var sessionVar =
                   HttpContext.Current.Session[userinfoSessionVar];
            if (sessionVar != null)
                return sessionVar as UserInfo;
            return null;
        }
        set
        {
            HttpContext.Current.Session[userinfoSessionVar] = value;
        }
    }

    //public static SparesMgmt.Models.CompanyInfo CompanyInFo
    //{
    //    get
    //    {
    //        if (HttpContext.Current == null) return null;

    //        var sessionVar =
    //            HttpContext.Current.Session[companyinfoSessionVar];
    //        if (sessionVar != null)
    //            return sessionVar as SparesMgmt.Models.CompanyInfo;
    //        return null;
    //    }
    //    set
    //    {
    //        HttpContext.Current.Session[companyinfoSessionVar] = value;
    //    }
    //}


    //public static SparesMgmt.Models.BranchInfo BranchInFo
    //{
    //    get
    //    {
    //        if (HttpContext.Current == null) return null;

    //        var sessionVar =
    //            HttpContext.Current.Session[BranchInFoSessionVar];
    //        if (sessionVar != null)
    //            return sessionVar as SparesMgmt.Models.BranchInfo;
    //        return null;
    //    }
    //    set
    //    {
    //        HttpContext.Current.Session[BranchInFoSessionVar] = value;
    //    }
    //}


    //public static SparesMgmt.Models.UserRoleInfo UserRoleInfo
    //{
    //    get
    //    {
    //        if (HttpContext.Current == null) return null;

    //        var sessionVar =
    //            HttpContext.Current.Session[RoleInfoSessionvar];
    //        if (sessionVar != null)
    //            return sessionVar as SparesMgmt.Models.UserRoleInfo;
    //        return null;
    //    }
    //    set
    //    {
    //        HttpContext.Current.Session[RoleInfoSessionvar] = value;
    //    }
    //}

    public static string CompanyName { get; set; }
   // public static string UserName { get; set; }
    public static string DivisionName { get; set; }
    public static string userrole { get; set; }
    public static string Address { get; set; }
    public static string phone { get; set; }
    public static int employeeid { get; set; }
    public static string empimg { get; set; }
    public static int useractiongroupid { get; set; }
    public static string rolename { get; set; }
    public static int gy_id { get; set; }
    public static int roleid { get; set; }

    //ADDED BY PUSHKAR FOR AUDITTRAIL ON 21 DEC 2106

    public static int y_id { get; set; }
    public static string softwareversion { get; set; }
    public static int Sessionid { get; set; }

    //ADDED BY SHRADDHA ON 03 NOV 2017 START
    public static int UserID { get; set; }
    public static int DepartmentId { get; set; }
    public static int IsCalendarApplicationOn { get; set; }
    public static string DefaultLang { get; set; }
    //ADDED BY SHRADDHA ON 03 NOV 2017 END

}
