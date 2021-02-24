using WetosMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WetosMVC.Repository
{
    public interface ICharts
    {
        void ProductWiseSales(out string MobileCountList, out string ProductList);

        void StateWiseSales(out string MobileCountList, out string StateNameList);

        //ADDED BY SHRADDHA ON 08 SEP 2017
        void departmentWiseLateCountForSelectedDateRange(out string MobileCountList, out string StateNameList);

        void departmentWiseAttendanceSummaryReportForSelectedDateRange(out string MobileCountList, out string StateNameList);
    }
}
