using System;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Linq;
using WetosDB;

namespace WetosMVC.Controllers
{
    // ADDED BY MSJ ON 26 JAN 2019 START // CRITICAL ISSUE FIXED
    public class SessionExpire : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //if (HttpContext.Current.Session["UserInfo"] == null)
            if (String.IsNullOrEmpty(SessionPersister.UserName))
            {
                FormsAuthentication.SignOut();
                filterContext.Result =
               new RedirectToRouteResult(new RouteValueDictionary   
            {  
             { "action", "LogOn" },  
            { "controller", "Account" }
            //,  { "returnUrl", filterContext.HttpContext.Request.RawUrl}  
             });

                return;
            }
            else
            {
                if (SessionPersister.roleInfo != null)
                {
                    //string RawUrl = filterContext.HttpContext.Request.RawUrl;
                    string RawUrl = filterContext.HttpContext.Request.RawUrl;

                    WetosDBEntities WetosDB = new WetosDBEntities();

                    NavMenu LinkList = WetosDB.NavMenus.Where(a => a.navlink.Trim().ToUpper() == RawUrl.Trim().ToUpper()).FirstOrDefault();

                    if (LinkList != null)
                    {

                        if (!RawUrl.Contains("Dashboard") && !RawUrl.Contains("Home") && !RawUrl.Contains("Account") && !RawUrl.Contains("profile"))
                        {
                            sp_get_user_role_menu_Result CurentMenu = SessionPersister.roleInfo.Where(a => a.navlink.ToUpper() == RawUrl.ToUpper()).FirstOrDefault();
                            if (CurentMenu == null)
                            {
                                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary {  
                                { "action", "EmployeeDashboard" }, { "controller", "Dashboard" }});

                                return;
                            }
                        }
                    }
                    else if (RawUrl != null || RawUrl != "") //ADDED BY PUSHKAR ON 21 FEB 2019 FOR AUTHORIZATION ISSUE
                    {
                        if (!RawUrl.Contains("Dashboard") && !RawUrl.Contains("Home") && !RawUrl.Contains("Account") && !RawUrl.Contains("profile"))
                        {
                            if (RawUrl == "/WetosEmployee" || RawUrl == "/WetosEmployee/" || RawUrl == "/WetosEmployee/Index")
                            {
                                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary {  
                                { "action", "EmployeeDashboard" }, { "controller", "Dashboard" }});

                                return;
                            }
                        }
                    }
                    //else
                    //{
                    //    // ADDED BY MSJ ON 25 JAN 2019 START
                    //    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary {  
                    //            { "action", "EmployeeDashboard" }, { "controller", "Dashboard" }});

                    //    return;
                    //    // ADDED BY MSJ ON 25 JAN 2019 END
                    //}
                }
            }

        }
    }
    // ADDED BY MSJ ON 26 JAN 2019 END

    //public class SessionExpire : ActionFilterAttribute
    //{
    //    public override void OnActionExecuting(ActionExecutingContext filterContext)
    //    {


    //        //if (HttpContext.Current.Session["UserInfo"] == null)
    //        if (String.IsNullOrEmpty(SessionPersister.UserName))
    //        {
    //            FormsAuthentication.SignOut();
    //            filterContext.Result =
    //           new RedirectToRouteResult(new RouteValueDictionary   
    //        {  
    //         { "action", "LogOn" },  
    //        { "controller", "Account" }
    //        //,  { "returnUrl", filterContext.HttpContext.Request.RawUrl}  
    //         });

    //            return;
    //        }
    //    }

    //}

    //public class SessionTimeoutAttribute : ActionFilterAttribute
    //{
    //    public override void OnActionExecuting(ActionExecutingContext filterContext)
    //    {
    //        //HttpContext ctx = HttpContext.Current;
    //        //if (HttpContext.Current.Session["EmployeeNo"] == null)
    //        //{
    //        if (string.IsNullOrEmpty(SessionPersister.Username))
    //        {
    //            filterContext.Result = new RedirectResult("~/Account/LogOn");
    //            return;
    //        }
    //        //}
    //        base.OnActionExecuting(filterContext);
    //    }
    //}



    //public class SessionExpireController : BaseController
    //{
    //    //
    //    // GET: /SessionExpire/

    //    //public ActionResult Index()
    //    //{
    //    //    return View();
    //    //}

    //    public SessionExpireController()
    //    {
    //        ReturnLogin();
    //    }

    //    public ActionResult ReturnLogin()
    //    {
    //        if (String.IsNullOrEmpty(SessionPersister.UserName))
    //        {
    //            TempData["alertMessage"] = "Sorry,Session Timed Out";
    //            return RedirectToAction("Index", "Login");
    //        }
    //        else
    //        {
    //            return null;
    //        }

    //    }

    //}
}
