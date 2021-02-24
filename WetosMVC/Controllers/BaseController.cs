using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using WetosMVCMainApp.Models;
using WetosDB;
using WetosMVC.Models;
using System.IO;
using System.Data.EntityClient;
using System.Data.SqlClient;
using System.Data.Objects;
using WetosMVCMainApp.Utilities;
using WetosMVCMainApp.Extensions;

namespace WetosMVC.Controllers
{
    // BASE CLASS
    public class BaseController : Controller
    {
        public WetosDBEntities WetosDB = null;

        protected static int flagcount = 0;

        public BaseController()
        {
            if (WetosDB == null)
            {
                WetosDB = new WetosDBEntities();
                WetosDB.CommandTimeout = 2000;

                //// assumes a connectionString name in .config of MyDbEntities
                //var selectedDb = new WetosDBEntities();
                //// so only reference the changed properties
                //// using the object parameters by name

                //var object_context = GetObjectContextFromEntity(WetosDB);

                //selectedDb.ChangeDatabase(                       
                //    initialCatalog: "name-of-another-initialcatalog",
                //    userId: "jackthelady",
                //    password: "nomoresecrets",
                //    dataSource: @".\sqlexpress" // could be ip address 120.273.435.167 etc
                //);

            }
        }

        /// <summary>
        /// GetFYStartDate
        /// </summary>
        /// <returns></returns>
        public DateTime GetFYStartDate()
        {

            //ABOVE LINE COMMENTED AND BELOW LINE IS ADDED BY SHALAKA ON 13TH DEC 2017
            GlobalSetting GlobalSettingObj = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.CurrentFinancialYear).FirstOrDefault();
            //DateTime CalanderStartDate = GetFYStartDate(); // DateTime CalanderStartDate = WetosDB.FinancialYears.Where(a => a.FinancialName == GlobalSettingObj.SettingValue).Select(a => a.StartDate).FirstOrDefault();


            FinancialYear TempCurrentFY = WetosDB.FinancialYears.Where(a => a.FinancialName == GlobalSettingObj.SettingValue).FirstOrDefault();
            FinancialYear TempPreviousFY = WetosDB.FinancialYears.Where(a => a.FinancialYearId == TempCurrentFY.PrevFYId).FirstOrDefault();

            //FinancialYear FY = TempPreviousFY == null ? TempCurrentFY : TempPreviousFY;

            DateTime CalanderStartDate = TempPreviousFY == null ? TempCurrentFY.StartDate : TempPreviousFY.StartDate;

            return CalanderStartDate;
        }

        public void Attention(string message)
        {
            TempData.Clear();
            TempData.Add(Alerts.ATTENTION, message);
        }

        public void Success(string message)
        {
            TempData.Clear();
            TempData.Add(Alerts.SUCCESS, message);
        }

        public void Information(string message)
        {

            TempData.Add(Alerts.INFORMATION, message);
        }

        public void Error(string message)
        {
            TempData.Clear();
            TempData.Add(Alerts.ERROR, message);
        }

        // ADDED BY MSJ ON 29 APR 2020 
        public void Clear()
        {
            TempData.Clear();          
        }

        [SessionExpire]
        public ActionResult AddAuditTrail(string UserAction)
        {
            //  MODIFIED BY PUSHKAR ON 21 DEC 2016 FOR NEW TABLE AUDITTRAIL START 

            try
            {
                WriteErrorToFile(UserAction + "_" + DateTime.Now); // CODE ADDED BY SHRADDHA ON 14 MAR 2018
                int LoginId = Convert.ToInt32(Session["EmployeeNo"]);
                string LoginNo = Session["EmployeeNo"] == null ? "" : Session["EmployeeNo"].ToString();
                int RoleId = Convert.ToInt32((Session["RoleId"]) == null ? 0 : Session["RoleId"]);
                AuditTrail action = new AuditTrail();
                action.LogDate = DateTime.Now;

                // Updated by Rajas on 1 AUGUST 2017 START
                if (UserAction.Length > 1024)
                {
                    action.Action = UserAction.Substring(0, 1000);
                }
                else
                {
                    action.Action = UserAction;
                }
                // Updated by Rajas on 1 AUGUST 2017 END

                action.userid = LoginId; // SessionPersister.UserInfo.UserId;
                action.UserName = LoginNo; // SessionPersister.UserInfo.UserName;
                action.UserRoleNo = RoleId;
                action.y_id = 0;
                action.SoftwareVersion = "STPLSERVER";
                action.SessionId = 0;

                action.MachineIP = " ";
                action.MachineNo = " ";

                WetosDB.AuditTrails.AddObject(action);

                WetosDB.SaveChanges();

                return null;
            }
            catch
            {
                return RedirectToAction("LogOn", "Account");
            }

        }

        //  MODIFIED BY PUSHKAR ON 21 DEC 2016 FOR NEW TABLE AUDITTRAIL END

        /// <summary>
        /// OnResultExecuting
        /// </summary>
        /// <param name="context"></param>
        protected override void OnResultExecuting(ResultExecutingContext context)
        {
            context.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            context.HttpContext.Response.Cache.SetValidUntilExpires(false);
            context.HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            context.HttpContext.Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
            context.HttpContext.Response.Cache.SetNoStore();

            base.OnResultExecuting(context);

        }


        public void SetWeekDatePeriod(DateTime frmdate, DateTime todate)
        {

            ViewBag.FromDate = frmdate.ToString("dd-MM-yyyy");
            ViewBag.ToDate = todate.ToString("dd-MM-yyyy");
        }

        // authorization start added by msj on 18 jULY 2017 start
        /// <summary>
        /// Identity
        /// </summary>
        protected CustomIdentity Identity
        {
            get { return (CustomIdentity)User.Identity; }
        }

        /// <summary>
        /// OnAuthorization
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            //Code Commented for giving some issue END
            if (!string.IsNullOrEmpty(SessionPersister.UserName))//(SimpleSessionPersister.Username)) //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 03 NOV 2017
            {
                filterContext.HttpContext.User =
                    new CustomPrincipal(
                        new CustomIdentity(SessionPersister.UserName));//(SimpleSessionPersister.Username)); //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 03 NOV 2017
            }
            base.OnAuthorization(filterContext);
        }

        // authorization start added by msj on 18 jULY 2017 start


        /// CODE ADDED BY SHRADDHA ON 14 MAR 2018
        /// TO WRITE ERROR IN MYFILE.TXT
        /// IF FILE NOT EXISTS THEN IT WILL BE GENERATED INTO MYFOLDER
        /// MY FOLDER FOLDER IS REQUIRED
        public bool WriteErrorToFile(string Message)
        {
            try
            {
                String path = System.Web.HttpContext.Current.Server.MapPath("~/ErrorLog/myFile.txt");
                using (StreamWriter writer = new StreamWriter(path, true))
                {
                    writer.WriteLine(Message);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }



        public static DbContext GetDbContextFromEntity(object entity)
        {
            var object_context = GetObjectContextFromEntity(entity);

            if (object_context == null)
                return null;

            return new DbContext(object_context, dbContextOwnsObjectContext: false);
        }

        private static ObjectContext GetObjectContextFromEntity(object entity)
        {
            var field = entity.GetType().GetField("_entityWrapper");

            if (field == null)
                return null;

            var wrapper = field.GetValue(entity);
            var property = wrapper.GetType().GetProperty("Context");
            var context = (ObjectContext)property.GetValue(wrapper, null);

            return context;
        }

        /// <summary>
        /// Check License
        /// </summary>
        public void CheckLicense(string UserName, string Password)
        {
            LoginUserAccessRights objUser = new LoginUserAccessRights();
            CryptorEngineUtil CryptorEngine = new CryptorEngineUtil();

            // string iniFile = ConfigurationManager.AppSettings["iniFile"].ToString();
            //bool isLicensed = CryptorEngine.ReadIni(iniFile);
            bool isLicensed = CryptorEngine.ReadIni(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["VirtualDir"] + "/App_Data/WETOS.ini"));
            try
            {
                // if (isLicensed && CryptorEngine.ValidateWetos(ref FailureText))
                // {

                if (UserName == "WetosAdmin" && Password == "nimdAsoteW")
                {
                    Session["Flag"] = "TRUE";
                    WETOSSession.UserName = "WetosAdmin";
                    objUser.BranchList = ClientConstants.WetosAdminRights;
                    objUser.CompanyList = ClientConstants.WetosAdminRights;
                    objUser.DepartmentList = ClientConstants.WetosAdminRights;
                    objUser.LocationList = ClientConstants.WetosAdminRights;
                    //objUser.AdministrationMenuRights = ClientConstants.WetosAdminRights;
                    objUser.AdministrationMenuRights = ClientConstants.WetosAdminRights;
                    //objUser.ApplicationMenuRights = ClientConstants.WetosAdminRights;
                    objUser.ApplicationMenuRights = ClientConstants.WetosAdminRights;
                    //objUser.AttendanceMenuRights = ClientConstants.WetosAdminRights;
                    objUser.AttendanceMenuRights = ClientConstants.WetosAdminRights;
                    objUser.MasterMenuRights = ClientConstants.WetosAdminRights;
                    //objUser.MasterMenuRights = ClientConstants.WetosAdminSanctionRights;
                    objUser.ReportMenuRights = ClientConstants.WetosAdminRights;
                    //objUser.ReportMenuRights = ClientConstants.WetosAdminSanctionRights;
                    //objUser.SanctionMenuRights = ClientConstants.WetosAdminRights;
                    objUser.SanctionMenuRights = ClientConstants.WetosAdminSanctionRights;
                    objUser.UpdatesMenuRights = ClientConstants.WetosAdminRights;
                    //objUser.UpdatesMenuRights = ClientConstants.WetosAdminSanctionRights;
                    objUser.UseraRoleName = ClientConstants.WetosAdminRights;
                    objUser.MenuIdList = ClientConstants.MenuList;
                    objUser.EmployeeId = ClientConstants.WetosAdminReportingId;
                    objUser.DivisionList = ClientConstants.WetosAdminRights;
                    objUser.RoleTypeId = ClientConstants.WetosAdminReportingId;
                    objUser.UnderObservationemployeeidList = ClientConstants.WetosAdminRights;

                    //WETOSSession.LoginUserRights = objUser;

                    //btnLogin.Attributes.Add("OnClick", "OpenMainWindow()");
                    //  string loginUrl = "../MasterPage.aspx";
                    string loginUrl = "MasterPage.aspx";
                    //OpenFormMenu(loginUrl);
                }
                //else if (UserName.Text.Trim() == Password.Text.Trim())
                //{
                //    //FailureText.Text = Messages.ErrorDisplay(906);
                //    //Response.Write(Messages.ErrorDisplay(906));
                //    Response.Redirect("ChangePassword.aspx?Errormessage=" + Messages.ErrorDisplay(906));
                //}
                else
                {
                    string EncryptedPassword;
                    Cryptography objCrypt = new Cryptography();
                    EncryptedPassword = objCrypt.EncryptPassword(Password.Trim());
                    //objUser = BusinessFactory.Instance.BLUser.SelectUserForLogin(UserName.Trim(), EncryptedPassword);

                    //lstGetAllBranchRules = objBL.GetAllBranchRules(cmpidBrid.BranchId, cmpidBrid.CompanyId)

                    WETOSSession.UserName = UserName.Trim();
                    //WETOSSession.LoginUserRights = objUser;

                }
                // string chkposting = "";
                //chkposting = BusinessFactory.Instance.BLPostingService.ProcessData(DateTime.Now.AddDays(-2), DateTime.Now.AddDays(-2), objUser.EmployeeId.ToString(), objUser.BranchId, objUser.CompanyId);
                if (objUser != null)
                {
                    Cryptography objCrypt = new Cryptography();
                    if (UserName.Trim().ToUpper() == objCrypt.DecryptPassword(objUser.Password))
                    {
                        Response.Redirect("ChangePassword.aspx?Errormessage=" + Messages.ErrorDisplay(906));
                    }
                    else
                    {


                        Session["Flag"] = "TRUE";

                        //if (WETOSSession.GetAllRules == null || WETOSSession.GetAllRules.Count == 0)
                        //{
                        //    WETOSSession.GetAllRules = BusinessFactory.Instance.BLRules.GetAllRule_EmpWise(objUser.UnderObservationemployeeidList);
                        //}

                        ////WETOSSession.GetAllRulesforProcessdate = BusinessFactory.Instance.BLRules.GetAllRule(objUser.CompanyId.ToString(),objUser.CompanyId.ToString());

                        //if (WETOSSession.GetAllLeaveRules == null || WETOSSession.GetAllLeaveRules.Count == 0)
                        //{
                        //    WETOSSession.GetAllLeaveRules = BusinessFactory.Instance.BLLeaveOperations.GetAllLeaveAppLeaveCodeMaster_EmpWise(objUser.UnderObservationemployeeidList);

                        //}
                        // btnLogin.Attributes.Add("OnClick", "OpenMainWindow()");
                        string loginUrl = "MasterPage.aspx";
                        //OpenFormMenu(loginUrl);
                    }
                }
                else
                {
                    //FailureText.Text = Messages.ErrorDisplay(706);
                }


                //}
                //else
                //{
                //    if (String.IsNullOrEmpty(FailureText.Text))
                //    {
                //        FailureText.Text = Messages.ErrorDisplay(705);  
                //    }

                //}
            }
            catch (Exception ex)
            {
                //FailureText.Text = Messages.ErrorDisplay(705); ;
            }
        }

        //// all params are optional
        //public static void ChangeDatabase(
        //    this DbContext source,
        //    string initialCatalog = "",
        //    string dataSource = "",
        //    string userId = "",
        //    string password = "",
        //    bool integratedSecuity = true,
        //    string configConnectionStringName = "")
        ///* this would be used if the
        //*  connectionString name varied from 
        //*  the base EF class name */
        //{
        //    try
        //    {
        //        // use the const name if it's not null, otherwise
        //        // using the convention of connection string = EF contextname
        //        // grab the type name and we're done
        //        var configNameEf = string.IsNullOrEmpty(configConnectionStringName)
        //            ? source.GetType().Name
        //            : configConnectionStringName;

        //        // add a reference to System.Configuration
        //        var entityCnxStringBuilder = new EntityConnectionStringBuilder
        //            (System.Configuration.ConfigurationManager
        //                .ConnectionStrings[configNameEf].ConnectionString);

        //        // init the sqlbuilder with the full EF connectionstring cargo
        //        var sqlCnxStringBuilder = new SqlConnectionStringBuilder
        //            (entityCnxStringBuilder.ProviderConnectionString);

        //        // only populate parameters with values if added
        //        if (!string.IsNullOrEmpty(initialCatalog))
        //            sqlCnxStringBuilder.InitialCatalog = initialCatalog;
        //        if (!string.IsNullOrEmpty(dataSource))
        //            sqlCnxStringBuilder.DataSource = dataSource;
        //        if (!string.IsNullOrEmpty(userId))
        //            sqlCnxStringBuilder.UserID = userId;
        //        if (!string.IsNullOrEmpty(password))
        //            sqlCnxStringBuilder.Password = password;

        //        // set the integrated security status
        //        sqlCnxStringBuilder.IntegratedSecurity = integratedSecuity;

        //        // now flip the properties that were changed
        //        source.Database.Connection.ConnectionString
        //            = sqlCnxStringBuilder.ConnectionString;
        //    }
        //    catch (Exception ex)
        //    {
        //        // set log item if required
        //    }
        //}


    }

    /// <summary>
    /// ALERT CLASS
    /// </summary>
    public static class Alerts
    {
        public const string SUCCESS = "success";
        public const string ATTENTION = "attention";
        public const string ERROR = "error";
        public const string INFORMATION = "info";

        public static string[] ALL
        {
            get
            {
                return new[]
                           {
                               SUCCESS,
                               ATTENTION,
                               INFORMATION,
                               ERROR
                           };
            }
        }
    }

    /// <summary>
    /// ConnectionTools
    /// </summary>
    public static class ConnectionTools
    {
        // all params are optional
        public static void ChangeDatabase(
            this DbContext source,
            string initialCatalog = "",
            string dataSource = "",
            string userId = "",
            string password = "",
            bool integratedSecuity = true,
            string configConnectionStringName = "")
        /* this would be used if the
        *  connectionString name varied from 
        *  the base EF class name */
        {
            try
            {
                // use the const name if it's not null, otherwise
                // using the convention of connection string = EF contextname
                // grab the type name and we're done
                var configNameEf = string.IsNullOrEmpty(configConnectionStringName)
                    ? source.GetType().Name
                    : configConnectionStringName;

                // add a reference to System.Configuration
                var entityCnxStringBuilder = new EntityConnectionStringBuilder
                    (System.Configuration.ConfigurationManager
                        .ConnectionStrings[configNameEf].ConnectionString);

                // init the sqlbuilder with the full EF connectionstring cargo
                var sqlCnxStringBuilder = new SqlConnectionStringBuilder
                    (entityCnxStringBuilder.ProviderConnectionString);

                // only populate parameters with values if added
                if (!string.IsNullOrEmpty(initialCatalog))
                    sqlCnxStringBuilder.InitialCatalog = initialCatalog;
                if (!string.IsNullOrEmpty(dataSource))
                    sqlCnxStringBuilder.DataSource = dataSource;
                if (!string.IsNullOrEmpty(userId))
                    sqlCnxStringBuilder.UserID = userId;
                if (!string.IsNullOrEmpty(password))
                    sqlCnxStringBuilder.Password = password;

                // set the integrated security status
                sqlCnxStringBuilder.IntegratedSecurity = integratedSecuity;

                // now flip the properties that were changed
                source.Database.Connection.ConnectionString
                    = sqlCnxStringBuilder.ConnectionString;
            }
            catch (Exception ex)
            {
                // set log item if required
            }
        }
    }
}
