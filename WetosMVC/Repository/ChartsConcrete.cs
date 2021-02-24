using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using WetosMVC.Controllers;

namespace WetosMVC.Repository
{
    public class ChartsConcrete : BaseController, ICharts
    {
        public void ProductWiseSales(out string MobileCountList, out string ProductList)
        {
            //using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            //{
            //    var productdata = con.Query<ProductModel>("Usp_GetTotalsalesProductwise", null, null, true, 0, CommandType.StoredProcedure).ToList();

            //    var MobileSalesCounts = (from temp in productdata
            //                             select temp.Qty).ToList();

            //    var ProductNames = (from temp in productdata
            //                        select temp.ProductName).ToList();

            //    MobileCountList = string.Join(", ", MobileSalesCounts);
            //    ProductList = string.Join(", ", ProductNames);
            //}


            DataTable table5 = new DataTable();
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);

            SqlCommand com1 = new SqlCommand("usp_Report_GetAllCantinousAbsentisum", con);
            com1.CommandType = CommandType.StoredProcedure;
            com1.Parameters.Add(new SqlParameter("@Fdate", SqlDbType.DateTime)).Value = new DateTime(2017, 6, 1); //new DateTime(2013, 01, 01);
            com1.Parameters.Add(new SqlParameter("@Tdate", SqlDbType.DateTime)).Value = new DateTime(2017, 6, 30); //new DateTime(2014, 01, 31);
            com1.Parameters.Add(new SqlParameter("@NoOfDays", SqlDbType.Int)).Value = 1;

            // Updated by Rajas on 13 Jan 2017
            string EmployeeString = "";

            //WetosDB.Employees.Select(a =>a.EmployeeId
            EmployeeString = (String.Join(",", WetosDB.Employees.Select(a => a.EmployeeId).ToArray()));

            com1.Parameters.Add(new SqlParameter("@EmpidList", SqlDbType.NVarChar, 500)).Value = EmployeeString;

            SqlDataAdapter da1 = new SqlDataAdapter(com1);

            da1.Fill(table5);

            List<string> Abc = new List<string>();
            List<string> pqr = new List<string>();
            int count = 0;
            foreach (DataRow dr in table5.Rows)
            {
                if (count > 0 && count < 7)
                {

                    string newStr = "\"" + dr[10].ToString() + "\"";
                    Abc.Add(newStr);
                    pqr.Add(dr[13].ToString());
                }
                count++;
                //arrayList.Add(dr["Name"]);
                // arrayList.Add(dr["Name"]);
            }


            MobileCountList = string.Join(", ", pqr.ToArray());
            ProductList = string.Join(", ", Abc.ToArray());

        }


        public void StateWiseSales(out string MobileCountList, out string StateNameList)
        {
            //using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            //{
            //    var productdata = con.Query<StateModel>("Usp_GetTotalsalesStatewise", null, null, true, 0, CommandType.StoredProcedure).ToList();

            //    var MobileSalesCounts = (from temp in productdata
            //                             select temp.Qty).ToList();

            //    var StateNames = (from temp in productdata
            //                      select temp.StateName).ToList();

            //    MobileCountList = string.Join(",", MobileSalesCounts);
            //    StateNameList = string.Join(",", StateNames);
            //}
            //return 0;

            DataTable table5 = new DataTable();
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);

            //SqlCommand com1 = new SqlCommand("usp_Report_GetAllCantinousAbsentisum", con);
            //com1.CommandType = CommandType.StoredProcedure;
            //com1.Parameters.Add(new SqlParameter("@Fdate", SqlDbType.DateTime)).Value = new DateTime(2017, 6, 1); //new DateTime(2013, 01, 01);
            //com1.Parameters.Add(new SqlParameter("@Tdate", SqlDbType.DateTime)).Value = new DateTime(2017, 6, 30); //new DateTime(2014, 01, 31);
            //com1.Parameters.Add(new SqlParameter("@NoOfDays", SqlDbType.Int)).Value = 1;


            //SqlCommand com1 = new SqlCommand("usp_Rpt_GetTimeCardNew", con);
            //com1.CommandType = CommandType.StoredProcedure;
            //com1.Parameters.Add(new SqlParameter("@Fdate", SqlDbType.DateTime)).Value = new DateTime(2017, 8, 1); //new DateTime(2013, 01, 01);
            //com1.Parameters.Add(new SqlParameter("@Tdate", SqlDbType.DateTime)).Value = new DateTime(2017, 8, 30); // new DateTime(2013, 01, 31);           

            SqlCommand com1 = new SqlCommand("usp_Report_GetAllCantinousAbsentisum", con);
            com1.CommandType = CommandType.StoredProcedure;
            com1.Parameters.Add(new SqlParameter("@Fdate", SqlDbType.DateTime)).Value = new DateTime(2017, 6, 1); //new DateTime(2013, 01, 01);
            com1.Parameters.Add(new SqlParameter("@Tdate", SqlDbType.DateTime)).Value = new DateTime(2017, 6, 30); //new DateTime(2014, 01, 31);
            com1.Parameters.Add(new SqlParameter("@NoOfDays", SqlDbType.Int)).Value = 1;


            // Updated by Rajas on 13 Jan 2017
            string EmployeeString = "";

            //WetosDB.Employees.Select(a =>a.EmployeeId
            EmployeeString = (String.Join(",", WetosDB.Employees.Where(a => a.ActiveFlag == null || a.ActiveFlag == true).Select(a => a.EmployeeId).ToArray()));
            EmployeeString = (String.Join(",", WetosDB.Employees.Where(a => a.ActiveFlag == null || a.ActiveFlag == true && a.CompanyId == 1 && a.BranchId == 1 && a.DepartmentId == 1).Select(a => a.EmployeeId).ToArray()));

            com1.Parameters.Add(new SqlParameter("@EmpidList", SqlDbType.NVarChar, 500)).Value = EmployeeString;
            //com1.Parameters.Add(new SqlParameter("@RoleTypeId", SqlDbType.NVarChar, 500)).Value = "1";
            //com1.Parameters.Add(new SqlParameter("@ReportingId", SqlDbType.Int)).Value = DBNull.Value;
            //com1.Parameters.Add(new SqlParameter("@Empid", SqlDbType.NVarChar)).Value = EmployeeString;
            //com1.Parameters.Add(new SqlParameter("@BranchListId", SqlDbType.NVarChar, 500)).Value = "1";
            //com1.Parameters.Add(new SqlParameter("@CompanyListId", SqlDbType.NVarChar, 500)).Value = "1";

            SqlDataAdapter da1 = new SqlDataAdapter(com1);

            da1.Fill(table5);

            List<string> Abc = new List<string>();
            List<string> pqr = new List<string>();
            int count = 0;
            foreach (DataRow dr in table5.Rows)
            {
                if (count > 0 && count < 7)
                {

                    string newStr = "\"" + dr[2].ToString() + "\"";
                    Abc.Add(newStr);
                    pqr.Add(dr[3].ToString());
                }
                count++;
                //arrayList.Add(dr["Name"]);
                // arrayList.Add(dr["Name"]);
            }


            MobileCountList = string.Join(", ", pqr.ToArray());
            StateNameList = string.Join(", ", Abc.ToArray());
        }

        /// <summary>
        /// ADDED BY SRHADDHA ON 08 SEP 2017
        /// </summary>
        /// <param name="MobileCountList"></param>
        /// <param name="StateNameList"></param>
        public void departmentWiseLateCountForSelectedDateRange(out string MobileCountList, out string StateNameList)
        {

            DataTable table5 = new DataTable();
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);


            SqlCommand com1 = new SqlCommand("SP_GetDepartmentwiseLateEmployeeCount", con);
            com1.CommandType = CommandType.StoredProcedure;
            com1.Parameters.Add(new SqlParameter("@FDATE", SqlDbType.DateTime)).Value = new DateTime(2017, 10, 01); //new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day); //new DateTime(2013, 01, 01);
            com1.Parameters.Add(new SqlParameter("@TDATE", SqlDbType.DateTime)).Value = new DateTime(2017, 10, 30);//new DateTime(DateTime.Now.AddDays(1).Year, DateTime.Now.AddDays(1).Month, DateTime.Now.AddDays(1).Day);// new DateTime(2013, 01, 31);
            com1.Parameters.Add(new SqlParameter("@CompanyId", SqlDbType.Int)).Value = 1;
            com1.Parameters.Add(new SqlParameter("@BranchId", SqlDbType.Int)).Value = 1;
            com1.Parameters.Add(new SqlParameter("@AllowedLateCount", SqlDbType.Int)).Value = 1;
            SqlDataAdapter da1 = new SqlDataAdapter(com1);

            da1.Fill(table5);

            List<string> Abc = new List<string>();
            List<string> pqr = new List<string>();
            int count = 0;
            foreach (DataRow dr in table5.Rows)
            {
                if (count > 0 && count < 7)
                {

                    string newStr = "\"" + dr[1].ToString() + "\"";
                    Abc.Add(newStr);
                    pqr.Add(dr[3].ToString());
                }
                count++;
                //arrayList.Add(dr["Name"]);
                // arrayList.Add(dr["Name"]);
            }

            //pqr.Add("100");
            MobileCountList = string.Join(", ", pqr.ToArray());
            StateNameList = string.Join(", ", Abc.ToArray());
        }

        /// <summary>
        /// ADDED BY SRHADDHA ON 08 SEP 2017
        /// </summary>
        /// <param name="MobileCountList"></param>
        /// <param name="StateNameList"></param>
        public void departmentWiseAttendanceSummaryReportForSelectedDateRange(out string MobileCountList, out string StateNameList)
        {

            DataTable table5 = new DataTable();
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WetosMvcConnectionString"].ConnectionString);

            string EmployeeString = string.Empty;

            SqlCommand com1 = new SqlCommand("usp_Report_GetAllAttendanceSumarry", con);
            com1.CommandType = CommandType.StoredProcedure;
            com1.Parameters.Add(new SqlParameter("@FDATE", SqlDbType.DateTime)).Value = new DateTime(2017, 10, 01); //new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day); //new DateTime(2013, 01, 01);
            com1.Parameters.Add(new SqlParameter("@TDATE", SqlDbType.DateTime)).Value = new DateTime(2017, 10, 30);//new DateTime(DateTime.Now.AddDays(1).Year, DateTime.Now.AddDays(1).Month, DateTime.Now.AddDays(1).Day);// new DateTime(2013, 01, 31);
            com1.Parameters.Add(new SqlParameter("@EmpidList", SqlDbType.NVarChar)).Value = "1,2,3"; //DBNull.Value; //"1195";
            SqlDataAdapter da1 = new SqlDataAdapter(com1);

            da1.Fill(table5);

            List<string> Abc = new List<string>();
            List<string> pqr = new List<string>();
            int count = 0;
            foreach (DataRow dr in table5.Rows)
            {
                if (count > 0 && count < 7)
                {

                    string newStr = "\"" + dr[10].ToString() + "\"";
                    Abc.Add(newStr);
                    pqr.Add(dr[11].ToString());
                }
                count++;
                //arrayList.Add(dr["Name"]);
                // arrayList.Add(dr["Name"]);
            }

            //pqr.Add("100");
            MobileCountList = string.Join(", ", pqr.ToArray());
            StateNameList = string.Join(", ", Abc.ToArray());
        }

    }
}

