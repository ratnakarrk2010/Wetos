using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace WetosMVCMainApp.Models
{
    public static class MvcHelpers
    {

        //  Reference: http://www.atlanticbt.com/blog/asp-net-mvc-using-ajax-json-and-partialviews/
        //  Reference: http://www.codeproject.com/Articles/305308/MVC-Techniques-with-JQuery-JSON-Knockout-and-Cshar

        public static string RenderPartialView(this Controller controller, string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = controller.ControllerContext.RouteData.GetRequiredString("action");

            controller.ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                try
                {
                    viewResult.View.Render(viewContext, sw);
                }
                catch (System.Exception e)
                {
                    sw.Write("Error: " + e.Message);
                }

                return sw.GetStringBuilder().ToString();
            }

            //public static string RenderPartialView(this Controller controller, string viewName, object model)
            //{
            //    if (string.IsNullOrEmpty(viewName))
            //        viewName = controller.ControllerContext.RouteData.GetRequiredString("action");

            //    controller.ViewData.Model = model;
            //    using (var sw = new StringWriter())
            //    {
            //        ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
            //        var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
            //        viewResult.View.Render(viewContext, sw);

            //        return sw.GetStringBuilder().ToString();
            //    }
            //}

        }
    }
}