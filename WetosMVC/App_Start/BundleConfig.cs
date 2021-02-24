using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace WetosMVC
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-2.2.3.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/FinalStyle").Include("~/Content/finalStyle.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));

            #region Extra Bundles for WETOS
            //ADDED BY SHRADDHA ON 06 SEP 2017 START

            #region STYLE BUNDLE
            //BOOTSTRAP PACKAGE STYLE BUNDLE
            bundles.Add(new StyleBundle("~/Content/BootstrapPackage").Include("~/Content/bootstrap.min.css"));

            //LAYOUT STYLE BUNDLE
            bundles.Add(new StyleBundle("~/Content/LayoutStyle").Include("~/Content/font-awesome.min.css",
                "~/Content/ionicons.css"));

            //FULL CALENDAR STYLE BUNDLE
            bundles.Add(new StyleBundle("~/Content/Fullcalendar").Include("~/Content/fullcalendar.min.css"));


            //DATATABLE STYLE BUNDLE
            bundles.Add(new StyleBundle("~/Content/DataTable").Include("~/Content/dataTables.bootstrap.css"));

            //AdminLTE STYLE BUNDLE
            bundles.Add(new StyleBundle("~/Content/AdminLTE").Include("~/Content/AdminLTE.min.css"));

            //COMPONENT STYLE BUNDLE
            bundles.Add(new StyleBundle("~/Content/Components").Include("~/Content/layout.css",
                "~/Content/components.css",
                "~/Content/_all-skins.min.css"));

            //DATEPICKER STYLE BUNDLE
            bundles.Add(new StyleBundle("~/Content/DatePicker").Include("~/Content/datepicker3.css"));

            //SELECT2 STYLE BUNDLE
            bundles.Add(new StyleBundle("~/Content/Select2").Include("~/Content/select2.min.css"));

        //ICHECK STYLE BUNDLE
            bundles.Add(new StyleBundle("~/Content/Icheck").Include("~/Scripts/iCheck/all.css"));

            #endregion


            #region SCRIPT BUNDLE
            //BOOTSTRAP PACKAGE SCRIPT BUNDLE
            bundles.Add(new ScriptBundle("~/bundles/LayoutTheme").Include("~/Scripts/jqueri.ui.min.js",
                "~/Scripts/scripts/layout.js"
                ));

            //BOOTSTRAP SCRIPT
            bundles.Add(new ScriptBundle("~/bundles/Bootstrap").Include("~/Scripts/bootstrap.min.js"
                ));

            //FULL CALENDAR SCRIPT BUNDLE
            bundles.Add(new ScriptBundle("~/bundles/Fullcalendar").Include("~/Scripts/moment.min.js",
                "~/Scripts/fullcalendar.min.js",
                "~/Scripts/lang-all.js"
                ));


            //DATATABLE SCRIPT BUNDLE
            bundles.Add(new ScriptBundle("~/bundles/DataTable").Include("~/Scripts/jquery.dataTables.min.js",
                "~/Scripts/dataTables.bootstrap.min.js"));

            //CHARTS SCRIPT BUNDLE
            bundles.Add(new ScriptBundle("~/bundles/Charts").Include("~/Scripts/custom.js",
                "~/Scripts/Chart.min.js",
                "~/Scripts/BarChart.js",
                "~/Scripts/DoughnutChartData.js",
                "~/Scripts/LineChartData.js",
                "~/Scripts/PieChartData.js"
                ));

            //DATEPICKER SCRIPT BUNDLE
            bundles.Add(new ScriptBundle("~/bundles/DatePicker").Include("~/Scripts/bootstrap-datepicker.js"));

            //SELECT2 SCRIPT BUNDLE
            bundles.Add(new ScriptBundle("~/bundles/Select2").Include("~/Scripts/select2.full.min.js"));

            //SLIMSCROLL SCRIPT BUNDLE
            bundles.Add(new ScriptBundle("~/bundles/SlimScroll").Include("~/Scripts/jquery.slimscroll.min.js"));

            //FASTCLICK SCRIPT BUNDLE
            bundles.Add(new ScriptBundle("~/bundles/FastClick").Include("~/Scripts/fastclick.js"));

            //ADMINLTEAPP SCRIPT BUNDLE
            bundles.Add(new ScriptBundle("~/bundles/AdminLTEApp").Include("~/Scripts/app.min.js",
               "~/Scripts/demo.js"));

            //ICHECK STYLE BUNDLE
            bundles.Add(new ScriptBundle("~/bundles/Icheck").Include("~/Scripts/iCheck/icheck.min.js"));

            //CKEditor STYLE BUNDLE
            bundles.Add(new ScriptBundle("~/bundles/CKEditor").Include("~/CKEditor/ckeditor/ckeditor.js"));
            #endregion


            //INPUT MASK BUNDLE ADDED ON 08 SEP 2017
            bundles.Add(new ScriptBundle("~/bundles/InputMask").Include("~/Scripts/jquery.inputmask.js",
                "~/Scripts/jquery.inputmask.date.extensions.js",
                "~/Scripts/jquery.inputmask.extensions.js"));

            // ADDED BY MSJ ON 16 FEB 2020
            bundles.Add(new ScriptBundle("~/bundles/jqueryvalidate").Include("~/Scripts/jquery.validate.min.js",
               "~/Scripts/jquery.validate.unobtrusive.min.js"));
            #endregion


        }
    }
}