using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WetosMVC
{
    /// <summary>
    ///  ADDED BY SHALAKA ON 13TH DEC 2017 TO REMOVE ALL HARD CODDED GLOBAL SETTING TEXT IN APPLICATION
    /// </summary>
    public static class GlobalSettingsConstant
    {
        #region Global Setting Text

        public static readonly string DefaultSchedule = "Default Schedule";
        public static readonly string CurrentFinancialYear = "Current Financial Year";
        public static readonly string LastEsslFileName = "Last Essl File Name";
        public static readonly string LastEsslRow = "Last Essl Row";
        public static readonly string MarkHalfDayShift = "Mark Half Day Shift";
        public static readonly string IsContLateDeduction = "Is Cont Late Deduction";
        public static readonly string SendEmail = "Send Email";
        public static readonly string IsDetailedLeaveData = "Is Detailed Leave Data";
        public static readonly string CompOffValidityDays = "CompOff Validity Days";
        public static readonly string ExportToPDFEnabled = "Export To PDF Enabled";
        public static readonly string ExportToExcelEnabled = "Export To Excel Enabled";
        public static readonly string IsCalendarApplicationOn = "Is Calendar Application On";
        public static readonly string DefaultLang = "Default Lang";
        public static readonly string FamilyDetailsMaxRecords = "Family details max. records"; //CODE ADDED BY SHRADDHA ON 10 FEB 2018 FOR Family Details Max. Records
        public static readonly string AutoSanction = "Auto Sanction";
        #endregion Global Setting Text
    }
}