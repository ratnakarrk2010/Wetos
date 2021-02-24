using System;
using System.Globalization;
//[assembly: SecurityPermission(SecurityAction.RequestMinimum, ControlThread = true)]

namespace WetosMVCMainApp.Utilities
{
    public class DateTimeConvertor
    {
        #region DateTime Formating
        public CultureInfo curcinfo;
        public DateTimeFormatInfo ddateformat = new DateTimeFormatInfo();

        public string GetFormatedDateInString(string strFormat, string date)
        {
            try
            {
                DateTime Date;
                string modifiedDate, month, year, day;
                curcinfo = CultureInfo.CurrentCulture;
              
                if (strFormat == curcinfo.DateTimeFormat.ShortDatePattern)
                {
                    curcinfo = CultureInfo.CurrentCulture;
                    ddateformat = curcinfo.DateTimeFormat;
                    Date = DateTime.Parse(date);
                    return Date.ToString(strFormat, curcinfo.DateTimeFormat);
                }
                else
                {
                    day = date.Substring(0, 2);
                    month = date.Substring(3, 2);
                    year = date.Substring(6, 4);

                    modifiedDate = day + "/" + month + "/" + year;

                    return modifiedDate;

                }
            }
            catch 
            {
                throw;
            }
        }

        #region Old Date In Date
        public DateTime GetFormatedDateInDateOld(string SelectedFormat, string date)
        {
            string modifiedDate, month, year, day;//ComonCode.ClientConstants.ShortDateFormat
            month = string.Empty;
            day = string.Empty;
            year = string.Empty;
            DateTime Date = DateTime.Now;
            //DateTime Date;
            curcinfo = CultureInfo.CurrentCulture;
            try
            {
                if (SelectedFormat == curcinfo.DateTimeFormat.ShortDatePattern)
                {

                    ddateformat = curcinfo.DateTimeFormat;
                    Date = DateTime.Parse(date);
                }
                else if (curcinfo.DateTimeFormat.ShortDatePattern == "MM/dd/yyyy")
                {
                    if (date.Length == 9 || date.Length == 8)//9/2/2009
                    {
                        day = date.Substring(0, 1);
                        month = date.Substring(2, 1);
                        year = date.Substring(4, 4);
                        if (day.EndsWith("/"))
                        {
                            day = day.Remove(day.Length - 1);
                            day = day.Replace("/", "");
                            day = day.Trim();
                            month = date.Substring(3, 2);
                            if (month.EndsWith("/"))
                            {
                                //month = date.Substring(2, 2);
                                month = month.Remove(day.Length - 1);
                                month = month.Replace("/", "");
                                month = month.Trim();

                                year = date.Substring(4, 4);
                            }
                            year = date.Substring(5, 4);
                        }
                        if (month.EndsWith("/"))
                        {
                            //day = day.Remove(day.Length - 1);
                            day = date.Substring(0, 2);
                            month = date.Substring(3, 1);
                            year = date.Substring(5, 4);
                        }
                    }


                    else if (date.Length == 10 || date.Length > 10)
                    {
                        //month = date.Substring(0, 2);
                        //day = date.Substring(3, 2);
                        //year = date.Substring(6, 4);

                        day = date.Substring(0, 2);
                        month = date.Substring(3, 2);
                        year = date.Substring(6, 4);

                    }
                    modifiedDate = month + "/" + day + "/" + year;
                    //modifiedDate = day + "/" + month + "/" + year;
                    ddateformat = curcinfo.DateTimeFormat;
                    Date = DateTime.Parse(modifiedDate);

                }

                return Date;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        public DateTime GetFormatedDateInDate(string SelectedFormat, string date)
        {
            string modifiedDate, month, year, day;//ComonCode.ClientConstants.ShortDateFormat
            month = string.Empty;
            day = string.Empty;
            year = string.Empty;
            DateTime Date = DateTime.Now;
            //DateTime Date;
            curcinfo = CultureInfo.CurrentCulture;
            try
            {
                if (SelectedFormat == curcinfo.DateTimeFormat.ShortDatePattern)
                {

                    ddateformat = curcinfo.DateTimeFormat;
                    Date = DateTime.Parse(date);
                }
                else if (curcinfo.DateTimeFormat.ShortDatePattern == "MM/dd/yyyy" || curcinfo.DateTimeFormat.ShortDatePattern == "M/d/yyyy")
                {
                    if (date.Length == 9 || date.Length == 8)//9/2/2009
                    {
                        day = date.Substring(0, 1);
                        month = date.Substring(2, 1);
                        year = date.Substring(4, 4);
                        if (day.EndsWith("/"))
                        {
                            day = day.Remove(day.Length - 1);
                            day = day.Replace("/", "");
                            day = day.Trim();
                            month = date.Substring(3, 2);
                            if (month.EndsWith("/"))
                            {
                                //month = date.Substring(2, 2);
                                month = month.Remove(day.Length - 1);
                                month = month.Replace("/", "");
                                month = month.Trim();

                                year = date.Substring(4, 4);
                            }
                            year = date.Substring(5, 4);
                        }
                        if (month.EndsWith("/"))
                        {
                            //day = day.Remove(day.Length - 1);
                            day = date.Substring(0, 2);
                            month = date.Substring(3, 1);
                            year = date.Substring(5, 4);
                        }
                    }


                    else if (date.Length == 10 || date.Length > 10)
                    {
                        //month = date.Substring(0, 2);
                        //day = date.Substring(3, 2);
                        //year = date.Substring(6, 4);

                        day = date.Substring(0, 2);
                        month = date.Substring(3, 2);
                        year = date.Substring(6, 4);

                    }
                    modifiedDate = month + "/" + day + "/" + year;
                    //modifiedDate = day + "/" + month + "/" + year;
                    ddateformat = curcinfo.DateTimeFormat;
                    Date = DateTime.Parse(modifiedDate);

                }
                return Date;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetFormatedStringInStringDate(string SelectedFormat, string date)
        {
            curcinfo = CultureInfo.CurrentCulture;
            try
            {
                if (SelectedFormat == curcinfo.DateTimeFormat.ShortDatePattern)
                {
                    string month, day, year, modifiedDate;
                    day = date.Substring(0, 2);
                    month = date.Substring(3, 2);
                    year = date.Substring(6, 4);
                    modifiedDate = day + "/" + month + "/" + year;
                    date = modifiedDate;
                }
                else if (curcinfo.DateTimeFormat.ShortDatePattern == "MM/dd/yyyy")
                {
                    string month, day, year, modifiedDate;
                    month = date.Substring(0, 2);
                    day = date.Substring(3, 2);
                    year = date.Substring(6, 4);
                    modifiedDate = day + "/" + month + "/" + year;
                    date = modifiedDate;
                }

                return date;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string ConcateDateTimeInString(string strformat, string date, string time)
        {
            DateTimeFormatInfo d = new DateTimeFormatInfo();
            try
            {
                DateTime Date;
                //d = curcinfo.DateTimeFormat;
                string dateTimeString = date + " " + time;
                curcinfo = CultureInfo.CurrentCulture;
                Date = DateTime.Parse(dateTimeString);
                return Date.ToString(strformat, curcinfo.DateTimeFormat);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DateTime ConcateDateTimeInDateTime(string strformat, string date, string time)
        {
            DateTimeFormatInfo d = new DateTimeFormatInfo();
            try
            {
                DateTime Date;
                //d = curcinfo.DateTimeFormat;
                string dateTimeString = date + " " + time;
                curcinfo = CultureInfo.CurrentCulture;
                Date = DateTime.Parse(dateTimeString);
                GetFormatedDateInDate(ClientConstants.ShortDateFormat, Date.ToString(strformat, curcinfo.DateTimeFormat));
                return Date;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region GetIntegerArrayFromStringArray
        public int[] StringArrayToIntegerArray(string strArry)
        {
            try
            {
                string[] values = strArry.Split(',');
                return Array.ConvertAll<string, int>(values, delegate(string s) { return int.Parse(s); });
            }
            catch
            {

                throw;
            }
        }
        #endregion

        #region  NthDay OF Month For Checking WeeklyOff

        public DateTime NthDayOfMonth(int year, int month, int nth, string day_of_the_week)
        {
            //int d = date.Day; 
            //return date.DayOfWeek == dow && (d / 7 == n || (d / 7 == (n - 1) && d % 7 > 0)); 
            // validate month value
            if (month < 1 || month > 12)
            { throw new ArgumentOutOfRangeException("Invalid month value."); }
            // validate the nth value
            if (nth < 0 || nth > 5) { throw new ArgumentOutOfRangeException("Invalid nth value."); }
            // start from the first day of the month
            DateTime dt = new DateTime(year, month, 1);
            // loop until we find our first match day of the week 
            while (dt.DayOfWeek.ToString().ToUpper() != day_of_the_week)
            { dt = dt.AddDays(1); }

            dt = dt.AddDays((nth - 1) * 7);
            return dt;
        }
        #endregion End WeekOffChk
    }
}
