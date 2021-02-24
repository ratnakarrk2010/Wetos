using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WetosMVC.Repository;

//--- Controller Added By Shalaka on 21st July 2017
namespace WetosMVC.Controllers
{
    /// <summary>
    /// CHART CONTROLLER 
    /// </summary>
   
    [SessionExpire] 
    [Authorize]
    public class WetosChartController : BaseController
    {
        // PRIVATE VARABLES
        ICharts _ICharts;

        /// <summary>
        /// CONSTRUCTOR
        /// </summary>
        public WetosChartController()
        {
            _ICharts = new ChartsConcrete();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            try
            {
                string tempMobile = string.Empty;
                string tempProduct = string.Empty;
                //string version = "10,9,8,7,6,5,4";
                _ICharts.ProductWiseSales(out tempMobile, out tempProduct);

                ViewBag.MobileCount_List = tempMobile.Trim();

                ViewBag.Productname_List = tempProduct.Trim();

                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Bar Chart
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult BarChart()
        {
            try
            {
                string tempMobile = string.Empty;
                string tempProduct = string.Empty;

                var DepartmentName = WetosDB.Departments.Where(a => a.MarkedAsDelete == null || a.MarkedAsDelete == 0).Select(a => new { DepartmentId = a.DepartmentId, DepartmentName = a.DepartmentCode + " - " + a.DepartmentName }).ToList();//ADDED DEPARTMENT CODE BY SHRADDHA ON 15 FEB 2018 
                ViewBag.DepartmentNameList = new SelectList(DepartmentName, "DepartmentId", "DepartmentName").ToList();
              
                _ICharts.departmentWiseAttendanceSummaryReportForSelectedDateRange(out tempMobile, out tempProduct);

                ViewBag.MobileCount_List = tempMobile.Trim();

                ViewBag.Productname_List = tempProduct.Trim();

                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Pie Chart
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult PieChart()
        {
            try
            {
                string tempMobile = string.Empty;
                string tempProduct = string.Empty;

                _ICharts.departmentWiseAttendanceSummaryReportForSelectedDateRange(out tempMobile, out tempProduct);

                ViewBag.MobileCount_List = tempMobile.Trim();

                ViewBag.Productname_List = tempProduct.Trim();

                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Line Chart
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult LineChart()
        {
            try
            {
                string MobileCountList = string.Empty;
                string StateNameList = string.Empty;

                _ICharts.StateWiseSales(out MobileCountList, out StateNameList);

                ViewBag.MobileCount_List = MobileCountList.Trim();

                ViewBag.Statename_List = StateNameList.Trim();

                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// DoughnutChart
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult DoughnutChart()
        {
            try
            {
                string tempMobile = string.Empty;
                string tempProduct = string.Empty;

                _ICharts.ProductWiseSales(out tempMobile, out tempProduct);

                ViewBag.MobileCount_List = tempMobile.Trim();

                ViewBag.Productname_List = tempProduct.Trim();

                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

}
