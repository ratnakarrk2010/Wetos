using System;
//using System.Net.Http.Formatting;
//using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web;
using WetosMVC.Controllers;
using System.Configuration;
using System.Web.Http;
using System.Net.Http.Formatting;

namespace WetosMVC
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();


            // CODE ADDED BY SHRADDHA ON 20 NOV 2017 START
            string IsInternalPostingTrue = ConfigurationManager.AppSettings["InternalPosting"];
            if (!string.IsNullOrEmpty(IsInternalPostingTrue) && IsInternalPostingTrue.Trim().ToUpper() == "TRUE")
            {
                JobScheduler.Start();
            }
            // CODE ADDED BY SHRADDHA ON 20 NOV 2017 END

            GlobalConfiguration.Configuration.Formatters.JsonFormatter.MediaTypeMappings.Add(new RequestHeaderMapping("Accept",
                              "text/html",
                              StringComparison.InvariantCultureIgnoreCase,
                              true,
                              "application/json"));
        }

        protected void Application_BeginRequest()
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
        }
    }
}