using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Entity;
//using System.Data.EntityModel;
using System.Globalization;
using System.ComponentModel;
using WetosDB;

namespace WetosMVCMainApp.Models
{
    public class CalendarEvent
    {

        public long ID;
        public string Title;
        public int SomeImportantKeyID;
        public string StartDateString;
        public string EndDateString;
        public string StatusString;
        public string StatusColor;
        public string ClassName;
        public bool AllDay;

        WetosDBEntities ent = new WetosDBEntities();

        //public static List<CalendarEvent> LoadAllAppointmentsInDateRange(double start, double end)
        //public static List<CalendarEvent> LoadAllAppointmentsInDateRange(double start, double end, string propid)
        //{
        //    var fromDate = ConvertFromUnixTimestamp(start);
        //    var toDate = ConvertFromUnixTimestamp(end);
        //    using (WetosDBEntities ent = new WetosDBEntities())
        //    {
        //        List<CalendarEvent> result = new List<CalendarEvent>();

        //        long PropId = Convert.ToInt64(propid);

        //        //MODIFIED BY GNG ON 05 DEC 2015 START

        //        //REPropertyCalender PropCalObj = ent.REPropertyCalenders.Where(a => a.PropertyId == PropId).FirstOrDefault();

        //        //if (PropCalObj != null)
        //        //{
        //        //    REAppointment aptObj = ent.REAppointments.Where(a => a.PropCalId == PropCalObj.PropCalId).FirstOrDefault();
        //        // List<REPropertyCalender> PropcalList = ent.REPropertyCalenders.Where(a => a.PropertyId == PropId).ToList();

        //        List<RE_SPGetEventsForSeller_Result> EventList = ent.RE_SPGetEventsForSeller(PropId).ToList();

        //        if (EventList != null || EventList.Count == 0)
        //        {
        //            // for (int i = 0; i < EventList.Count; i++)
        //            foreach (RE_SPGetEventsForSeller_Result events in EventList)
        //            {
        //                long PropertyCalendarId = events.PropCalId;
        //                List<REAppointment> AptList = ent.REAppointments.Where(a => a.PropCalId == PropertyCalendarId).ToList();

        //                // List<RE_SPGetEventsForSeller_Result> EventList = ent.RE_SPGetEventsForSeller(PropcalList[i].PropertyId).ToList();

        //                //foreach (RE_SPGetEventsForSeller_Result events in EventList)                           
        //                //{
        //                //}
        //                // int i = 0;
        //                if (events.StartDate != null)
        //                {
        //                    CalendarEvent calendar = new CalendarEvent();
        //                    //calendar.ID = events.AppointmentId;
        //                    //calendar.SomeImportantKeyID = item.SomeImportantKey == null ? 0 : item.SomeImportantKey.Value;
        //                    calendar.StartDateString = events.StartDate.ToString("s"); // "s" is a preset format that outputs as: "2009-02-27T12:12:22"
        //                    calendar.EndDateString = events.StartDate.ToString("s"); // field AppointmentLength is in minutes
        //                   // calendar.Title = events.Title;

        //                    // If appointment is fixed, Show appointment with different color to buyer and seller
        //                    //if (events.OwnerId == SimpleSessionPersister.UserID && events.Status == 2)
        //                    //{
        //                    //    calendar.StatusString = Enums.GetName<AppointmentStatus>((AppointmentStatus)events.Status);
        //                    //    calendar.StatusColor = Enums.GetEnumDescription<AppointmentStatus>(calendar.StatusString);
        //                    //    string ColorCode = calendar.StatusColor.Substring(0, calendar.StatusColor.IndexOf(":")); ;
        //                    //    calendar.ClassName = calendar.StatusColor.Substring(calendar.StatusColor.IndexOf(":") + 1, calendar.StatusColor.Length - ColorCode.Length - 1);
        //                    //    calendar.StatusColor = ColorCode;
        //                    //}
        //                    //    //If user is not seller and buyer to perticular property, show event in orange color.
        //                    //else 
        //                    if (AptList == null)
        //                    {
        //                        calendar.StatusString = "Available";
        //                        calendar.StatusColor = "#FF8000:Available";
        //                        string ColorCode = "#FF8000";
        //                        calendar.ClassName = "Available";
        //                        calendar.StatusColor = ColorCode;
        //                    }
        //                    else if (AptList != null)
        //                    {
        //                        //For other users, it will be shown as blocked appoitnment
        //                        foreach (var aptObj in AptList)
        //                        {
        //                            if (aptObj.SellerId == SimpleSessionPersister.UserID && events.Status == 2)
        //                            {
        //                                calendar.StatusString = Enums.GetName<AppointmentStatus>((AppointmentStatus)events.Status);
        //                                calendar.StatusColor = Enums.GetEnumDescription<AppointmentStatus>(calendar.StatusString);
        //                                string ColorCode = calendar.StatusColor.Substring(0, calendar.StatusColor.IndexOf(":")); ;
        //                                calendar.ClassName = calendar.StatusColor.Substring(calendar.StatusColor.IndexOf(":") + 1, calendar.StatusColor.Length - ColorCode.Length - 1);
        //                                calendar.StatusColor = ColorCode;
        //                            }
        //                            else if (aptObj.BuyerId == SimpleSessionPersister.UserID && events.Status == 2)
        //                            {
        //                                calendar.StatusString = Enums.GetName<AppointmentStatus>((AppointmentStatus)events.Status);
        //                                calendar.StatusColor = Enums.GetEnumDescription<AppointmentStatus>(calendar.StatusString);
        //                                string ColorCode = calendar.StatusColor.Substring(0, calendar.StatusColor.IndexOf(":")); ;
        //                                calendar.ClassName = calendar.StatusColor.Substring(calendar.StatusColor.IndexOf(":") + 1, calendar.StatusColor.Length - ColorCode.Length - 1);
        //                                calendar.StatusColor = ColorCode;
        //                            }
        //                            else
        //                            {
        //                                calendar.StatusString = "Available";
        //                                calendar.StatusColor = "#FF8000:Available";
        //                                string ColorCode = "#FF8000";
        //                                calendar.ClassName = "Available";
        //                                calendar.StatusColor = ColorCode;
        //                            }
        //                        }
        //                    }

        //                    result.Add(calendar);
        //                }
        //                //}
        //                // }
        //                //MODIFIED BY GNG ON 05 DEC 2015 END
        //            }
        //        }
        //        return result;
        //    }
        //}

        //public static List<CalendarEvent> LoadAppointmentSummaryInDateRange(double start, double end)
        //{

        //    var fromDate = ConvertFromUnixTimestamp(start);
        //    var toDate = ConvertFromUnixTimestamp(end);
        //    using (WetosDBEntities ent = new WetosDBEntities())
        //    {
        //        var rslt = ent.REAppointments.Where(s => s.StartDate >= fromDate)
        //                                                .GroupBy(s => System.Data.Objects.EntityFunctions.TruncateTime(s.StartDate))
        //                                                .Select(x => new { StartDate = x.Key, Count = x.Count() });

        //        List<CalendarEvent> result = new List<CalendarEvent>();
        //        int i = 0;
        //        foreach (var item in rslt)
        //        {
        //            CalendarEvent rec = new CalendarEvent();
        //            rec.ID = i; //we dont link this back to anything as its a group summary but the fullcalendar needs unique IDs for each event item (unless its a repeating event)
        //            //rec.SomeImportantKeyID = -1;
        //            string StringDate = string.Format("{0:yyyy-MM-dd}", item.StartDate);
        //            rec.StartDateString = StringDate + "T00:00:00"; //ISO 8601 format
        //            // rec.EndDateString = StringDate + "T23:59:59";
        //            rec.Title = "Booked: " + item.Count.ToString();
        //            result.Add(rec);
        //            i++;
        //        }

        //        return result;
        //    }
        //}

        /// <summary>
        /// Update event time and date
        /// </summary>
        /// <param name="id"></param>
        /// <param name="NewEventStart"></param>
        /// <param name="NewEventEnd"></param>
        //public static void UpdateEventDate(int id, string NewEventStart, string NewEventEnd)
        //{
        //    // EventStart comes ISO 8601 format, eg:  "2000-01-10T10:00:00Z" - need to convert to DateTime
        //    using (WetosDBEntities ent = new WetosDBEntities())
        //    {
        //        long propId = Convert.ToInt64(id);
        //        long propcalId = ent.REPropertyCalenders.Where(a => a.PropertyId == propId).Select(a => a.PropCalId).FirstOrDefault();
        //        var rec = ent.REAppointments.FirstOrDefault(s => s.PropCalId == propcalId);
        //        if (rec != null)
        //        {
        //            //  DateTime DateTimeStart = DateTime.Parse(NewEventStart, null, DateTimeStyles.RoundtripKind).ToLocalTime(); // and convert offset to localtime
        //            DateTime startdate = Convert.ToDateTime(NewEventStart);
        //            DateTime enddate = Convert.ToDateTime(NewEventEnd);

        //            rec.EndDate = enddate;
        //            rec.StartDate = startdate;
        //            if (!String.IsNullOrEmpty(NewEventEnd))
        //            {
        //                TimeSpan span = DateTime.Parse(NewEventEnd, null, DateTimeStyles.RoundtripKind).ToLocalTime() - startdate;
        //                // rec.StartTime = DateTime.Parse(span.TotalMinutes);
        //            }
        //            ent.SaveChanges();
        //        }
        //    }

        //}

        //public static void ConfirmAppointment(int id, int pId)
        //{
        //    using (WetosDBEntities ent = new WetosDBEntities())
        //    {
        //        var propcal = ent.REPropertyCalenders.FirstOrDefault(a => a.PropertyId == pId);

        //        var apt = ent.REAppointments.FirstOrDefault(a => a.PropCalId == propcal.PropCalId);

        //        if (apt != null)
        //        {
        //            apt.Status = 3;
        //        }
        //        if (propcal != null)
        //        {
        //            propcal.Status = 3;
        //        }
        //    }
        //}

        /// <summary>
        /// Convert from Unix time stamp
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        private static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }

        /// <summary>
        /// Save new event
        /// </summary>
        /// <param name="Title"></param>
        /// <param name="NewEventDate"></param>
        /// <param name="Description"></param>
        /// <param name="NewEventTime"></param>
        /// <param name="PropertyId"></param>
        /// <param name="NewEventDuration"></param>
        /// <returns></returns>
        //public static bool CreateNewEvent(string Title, string NewEventDate, string Description, string NewEventTime, string PropertyId, string NewEventDuration)
        //{
        //    try
        //    {
        //        WetosDBEntities ent = new WetosDBEntities();

        //        // REAppointment rec = new REAppointment();
        //        WetosDBEntities pcalObj = new WetosDBEntities();

        //        pcalObj.StartDate = DateTime.ParseExact(NewEventDate + " " + NewEventTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
        //        pcalObj.PropertyId = Convert.ToInt64(PropertyId);
        //        pcalObj.Duration = NewEventDuration;

        //        ent.REPropertyCalenders.Add(pcalObj);
        //        ent.SaveChanges();
        //    }
        //    catch (System.Exception)
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        //public static bool CreateAppointment(string Title, string NewEventDate, string Description, string NewEventTime, string PropertyId, string NewEventDuration)
        //{
        //    try
        //    {
        //        WetosDBEntities ent = new WetosDBEntities();
        //        REAppointment aptobj = new REAppointment();
        //        REPropertyCalender propcalobj = new REPropertyCalender();

        //        long propid = Convert.ToInt32(PropertyId);
        //        REPropertyCalender pcalObj = ent.REPropertyCalenders.Where(a => a.PropertyId == propid).FirstOrDefault();
        //        REProperty pobj = ent.REProperties.Where(a => a.PropertyId == propid).FirstOrDefault();

        //        aptobj.StartDate = DateTime.ParseExact(NewEventDate.ToString() + " " + NewEventTime.ToString(), "MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture);
        //        //aptobj.StartDate = DateTime.ParseExact(NewEventDate.ToString(), "MM/dd/yyyy", CultureInfo.InvariantCulture);
        //        // aptobj.StartTime = DateTime.ParseExact(NewEventTime, "HH:mm", CultureInfo.InvariantCulture);
        //        aptobj.PropCalId = pcalObj.PropCalId;
        //        //aptobj.Duration = DateTime.ParseExact(NewEventDuration, "HH:mm", CultureInfo.InvariantCulture);
        //        aptobj.Duration = Convert.ToInt32(NewEventDuration);
        //        aptobj.EndDate = aptobj.StartDate.AddMinutes(Convert.ToDouble(aptobj.Duration));
        //        aptobj.Title = Title;
        //        aptobj.Description = Description;
        //        aptobj.BuyerId = SimpleSessionPersister.UserID;
        //        aptobj.SellerId = pobj.OwnerId;
        //        aptobj.Status = 2;

        //        propcalobj.Status = 2;

        //        ent.REAppointments.Add(aptobj);
        //        //  ent.REPropertyCalenders.Add(propcalobj);

        //        ent.SaveChanges();
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        public class timeTable
        {

            public String title { get; set; }
            public DateTime start { get; set; }
            public DateTime end { get; set; }
            public String backgroundColor { get; set; }
            public String borderColor { get; set; }

        }


        public static List<CalendarEvent> LoadAllAppointmentsInDateRange(int EmpId, DateTime start, DateTime ToDate)
        {
            using (WetosDBEntities ent = new WetosDBEntities())
            {
                List<CalendarEvent> result = new List<CalendarEvent>();

                List<timeTable> TimetableList = new List<timeTable>();
                //DateTime start = Convert.ToDateTime(StartDate);
                // DateTime end = Convert.ToDateTime(EndDate);



                //List<ShiftSchedulePattern> ShiftSchedulePatternList = ent.ShiftSchedulePatterns.Where(a => a.EmployeeId == EmpId && (a.EffectiveStartDate <= start || a.EffectiveStartDate >= start) && (a.EffectiveEndDate >= ToDate || a.EffectiveEndDate <= ToDate)).ToList();

                // if (ShiftSchedulePatternList != null)
                // {
                //     List<ShiftSchedulePattern> ShiftSchedulePatternListForPerticularDateRange = ShiftSchedulePatternList.ToList();


                //         foreach (ShiftSchedulePattern ShiftSchedulePatternObjForPerticularDateRange in ShiftSchedulePatternList)
                //         {


                //             string command = string.Format(@"select * from ShiftSchedulePattern where EmployeeId={0} and (EffectiveStartDate <='{1}/{2}/{3} {4}' or EffectiveStartDate >='{1}/{2}/{3} {4}') and (EffectiveEndDate>='{5}/{6}/{7} {8}' or EffectiveEndDate<='{5}/{6}/{7} {8}');"
                //               , EmpId, start.Year, start.Month, start.Day,"00:00",ToDate.Year,ToDate.Month,ToDate.Day,"23:59");
                //           var xyz=   ent.ExecuteStoreQuery<List<string>>(command, "").ToList();

                //         }

                // }


                //#region shiftregion
                //List<string> MonthList = MyFunction(start, ToDate);
                //MonthList.Add(ToDate.ToString("yyyy,MM", CultureInfo.InvariantCulture));

                //foreach (string MonthYear in MonthList)
                //{

                //    List<string> ColumnStringList = new List<string>();
                //    List<string> ValueStringList = new List<string>();

                //    string[] StartDateArray = MonthYear.Split(',');
                //    int TmpStartYear = Convert.ToInt32(StartDateArray[0].Trim());
                //    int TmpStartMonth = Convert.ToInt32(StartDateArray[1].Trim());
                //    List<SP_DisplayShiftOnCalendar_Result> ShiftSchedulePatternList = ent.SP_DisplayShiftOnCalendar(EmpId, TmpStartMonth, TmpStartYear).ToList();
                //    if (ShiftSchedulePatternList.Count > 0)
                //    {

                //        foreach (SP_DisplayShiftOnCalendar_Result ShiftSchedulePatternObj in ShiftSchedulePatternList)
                //        {

                //            //for (int x = 1; x <= 31; x++)
                //            //{
                //            //    ColumnStringList.Add("Day" + x.ToString());
                //            //}
                //            // string ColumnStr = string.Join(",", ColumnStringList.ToArray());
                //            int j = 0;
                //            DateTime TempStartDateObj = new DateTime(TmpStartYear, TmpStartMonth, 1);
                //            DateTime EndDateObj = TempStartDateObj.AddMonths(1).AddDays(-1);
                //            for (int x = 1; x <= EndDateObj.Day; x++)
                //            {
                //                string DayStr = "Day" + x.ToString();

                //                var ShiftStr = ShiftSchedulePatternObj.GetType().GetProperty(DayStr).GetValue(ShiftSchedulePatternObj, null);

                //                //string ShiftDayCount = "ShiftSchedulePatternObj." + ColumnStringList[x].ToString();
                //                //var y = ShiftDayCount;
                //                CalendarEvent calendar = new CalendarEvent();
                //                calendar.ID = j++;
                //                DateTime CalendarDate = new DateTime(TmpStartYear, TmpStartMonth, x);
                //                //calendar.SomeImportantKeyID = item.SomeImportantKey == null ? 0 : item.SomeImportantKey.Value;
                //                calendar.StartDateString = CalendarDate.ToString("s"); // "s" is a preset format that outputs as: "2009-02-27T12:12:22"
                //                calendar.EndDateString = CalendarDate.ToString("s"); // field AppointmentLength is in minutes
                //                if (ShiftStr == null)
                //                {
                //                    Employee EmployeeObj = ent.Employees.Where(a => a.EmployeeId == EmpId).FirstOrDefault();
                //                    calendar.Title = EmployeeObj.DefaultShift;
                //                }
                //                else
                //                {
                //                    calendar.Title = ShiftStr.ToString();
                //                }
                //                calendar.StatusColor = "Orange";
                //                result.Add(calendar);
                //            }

                //        }
                //    }
                //    else
                //    {
                //        int j = 0;
                //        DateTime TempStartDateObj = new DateTime(TmpStartYear, TmpStartMonth, 1);
                //        DateTime EndDateObj = TempStartDateObj.AddMonths(1).AddDays(-1);
                //        for (int x = 1; x <= EndDateObj.Day; x++)
                //        {
                //            Employee EmployeeObj = ent.Employees.Where(a => a.EmployeeId == EmpId).FirstOrDefault();
                //            if (EmployeeObj!=null)
                //            {
                //                if (EmployeeObj.DefaultShift != null)
                //                {

                //                    CalendarEvent calendar = new CalendarEvent();
                //                    calendar.ID = j++;
                //                    DateTime CalendarDate = new DateTime(TmpStartYear, TmpStartMonth, x);
                //                    //calendar.SomeImportantKeyID = item.SomeImportantKey == null ? 0 : item.SomeImportantKey.Value;
                //                    calendar.StartDateString = CalendarDate.ToString("s"); // "s" is a preset format that outputs as: "2009-02-27T12:12:22"
                //                    calendar.EndDateString = CalendarDate.ToString("s"); // field AppointmentLength is in minutes

                //                    calendar.Title = EmployeeObj.DefaultShift;

                //                    calendar.StatusColor = "Orange";
                //                    result.Add(calendar);
                //                }
                //            }
                //        }
                //    }
                //}

                //#endregion
                Employee EmployeeObj = ent.Employees.Where(a => a.EmployeeId == EmpId).FirstOrDefault();
                if (EmployeeObj != null)
                {
                    // UPDATED BY Rajas on 29 JULY 2017 for MarkedAsDelete
                    List<HoliDay> HolidayList = ent.HoliDays.Where(a => a.FromDate >= start && a.ToDate <= ToDate && a.CompanyId == EmployeeObj.CompanyId
                        && a.Branchid == EmployeeObj.BranchId && a.MarkedAsDelete == 0).ToList();
                    if (HolidayList != null || HolidayList.Count() > 0)
                    {
                        int i = 0;
                        foreach (var HolidayObj in HolidayList)
                        {
                            CalendarEvent calendar = new CalendarEvent();
                            calendar.ID = i++;
                            //calendar.SomeImportantKeyID = item.SomeImportantKey == null ? 0 : item.SomeImportantKey.Value;
                            calendar.StartDateString = HolidayObj.FromDate.ToString("s"); // "s" is a preset format that outputs as: "2009-02-27T12:12:22"
                            calendar.EndDateString = HolidayObj.ToDate.ToString("s"); // field AppointmentLength is in minutes
                            calendar.Title = HolidayObj.Description;
                            calendar.StatusColor = "Orange";
                            result.Add(calendar);
                        }
                    }

                    List<DailyTransaction> DailyTransactionList = ent.DailyTransactions.Where(a => a.EmployeeId == EmpId && a.TranDate >= start && a.TranDate <= ToDate).ToList();
                    //if (DailyTransactionObj != null)
                    //{
                    //    timeTable timetableobj = new timeTable();
                    //    timetableobj.backgroundColor = "Red";
                    //    timetableobj.borderColor = "#00c0ef";
                    //    timetableobj.title = DailyTransactionObj.Status;
                    //    timetableobj.start = DailyTransactionObj.Login;
                    //    timetableobj.end = DailyTransactionObj.LogOut;
                    //    TimetableList.Add(timetableobj);
                    //}


                    //List<SP_GetEvent_Result> EventList = ent.SP_GetEvent(SimpleSessionPersister.UserID).ToList();

                    if (DailyTransactionList != null || DailyTransactionList.Count == 0)
                    {
                        // for (int i = 0; i < EventList.Count; i++)
                        foreach (DailyTransaction DailyTransactionObj in DailyTransactionList)
                        {

                            //long PropertyCalendarId = events.PropCalId.Value;
                            //List<REAppointment> AptList = ent.REAppointments.Where(a => a.PropCalId == PropertyCalendarId).ToList();

                            // List<RE_SPGetEventsForSeller_Result> EventList = ent.RE_SPGetEventsForSeller(PropcalList[i].PropertyId).ToList();

                            //foreach (RE_SPGetEventsForSeller_Result events in EventList)                           
                            //{
                            int i = 0;
                            if (DailyTransactionObj.TranDate != null)
                            {
                                CalendarEvent calendar = new CalendarEvent();

                                calendar.ID = i++;

                                //calendar.SomeImportantKeyID = item.SomeImportantKey == null ? 0 : item.SomeImportantKey.Value;
                                //CODE ADDED FOR CAL NS PRESENTATION BY SHRADDHA ON 01 FEB 2018
                                DateTime TranDate = DailyTransactionObj.TranDate.Date;
                                DateTime LoginDate = DailyTransactionObj.Login.Date;

                                if (TranDate != LoginDate)
                                {
                                    calendar.StartDateString = DailyTransactionObj.Login.AddDays(-1).ToString("s");
                                    calendar.EndDateString = DailyTransactionObj.LogOut.AddDays(-1).ToString("s");
                                }
                                else
                                {
                                    calendar.StartDateString = DailyTransactionObj.Login.ToString("s"); // "s" is a preset format that outputs as: "2009-02-27T12:12:22"
                                    calendar.EndDateString = DailyTransactionObj.LogOut.ToString("s"); // field AppointmentLength is in minutes
                                }

                                string StTime = calendar.StartDateString.Substring(11, 5); // MODIFIED BY MSJ ON 16 JAN 2018

                                string EndTime = calendar.EndDateString.Substring(11, 5); // MODIFIED BY MSJ ON 16 JAN 2018

                                // Updated by Rajas on 19 MAY 2017 for Early count
                                //if ((DailyTransactionObj.LateCount != null || DailyTransactionObj.LateCount > 0) && (DailyTransactionObj.EarlyCount != null || DailyTransactionObj.EarlyCount > 0))
                                if (DailyTransactionObj.LateCount != null || DailyTransactionObj.LateCount > 0)
                                {
                                    // EG count added by Rajas on 19 MAY 2017
                                    //calendar.Title = DailyTransactionObj.Status + " LC=" + DailyTransactionObj.LateCount + " EG =" + DailyTransactionObj.EarlyCount;
                                    calendar.Title += DailyTransactionObj.Status + " LC=" + DailyTransactionObj.LateCount;
                                }
                                else if (DailyTransactionObj.EarlyCount != null || DailyTransactionObj.EarlyCount > 0)
                                {
                                    calendar.Title += DailyTransactionObj.Status + " EG=" + DailyTransactionObj.EarlyCount;
                                }
                                else
                                {
                                    calendar.Title += DailyTransactionObj.Status;
                                }

                                bool ToUpdate = false;
                                bool ExcludeTime = false;

                                if (calendar.Title == "PPPP")
                                {
                                    calendar.StatusColor = "Green";
                                    ToUpdate = true;

                                }

                                else if (calendar.Title == "AAAA")
                                {
                                    if (HolidayList.Count() > 0)
                                    {
                                        foreach (var HolidayObj in HolidayList)
                                        {
                                            if (DailyTransactionObj.Login == HolidayObj.FromDate)
                                            {
                                                ToUpdate = false;
                                            }
                                            else
                                            {
                                                calendar.StatusColor = "Red";
                                                ToUpdate = true;
                                                ExcludeTime = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        calendar.StatusColor = "Red";
                                        ToUpdate = true;
                                        ExcludeTime = true;
                                    }
                                }

                                else if (calendar.Title == "WOWO")
                                {
                                    //CHANGED Title by SHRADDHA ON 31 JAN 2017
                                    calendar.Title = "WOWO";
                                    calendar.StatusColor = "Purple";
                                    ToUpdate = true;
                                    ExcludeTime = true;
                                }
                                else if (calendar.Title == "PLPL")
                                {
                                    //CHANGED Title by SHRADDHA ON 31 JAN 2017
                                    calendar.Title = "PLPL";
                                    calendar.StatusColor = "#1c325d";
                                    ToUpdate = true;
                                    ExcludeTime = true;
                                }
                                else if (calendar.Title == "APLAPL")
                                {
                                    // Added by Rajas on 16 AUGUST 2017
                                    calendar.Title = "APLAPL";
                                    calendar.StatusColor = "#1c325d";
                                    ToUpdate = true;
                                    ExcludeTime = true;
                                }
                                else if (calendar.Title == "NBLNBL")
                                {
                                    // Added by Rajas on 16 AUGUST 2017 
                                    calendar.Title = "NBLNBL";
                                    calendar.StatusColor = "#1c325d";
                                    ToUpdate = true;
                                    ExcludeTime = true;
                                }
                                else if (calendar.Title == "ODOD")
                                {
                                    // Added by Rajas on 20 MAY 2017
                                    calendar.Title = "ODOD";
                                    calendar.StatusColor = "#1c325d";
                                    ToUpdate = true;
                                    ExcludeTime = true;
                                }
                                else if (calendar.Title == "TOTO")  // Added by Rajas on 9 JULY 2017
                                {
                                    calendar.Title = "TOTO";
                                    calendar.StatusColor = "#1c325d";
                                    ToUpdate = true;
                                    ExcludeTime = true;
                                }
                                else if (calendar.Title == "CLCL")
                                {
                                    //CHANGED Title by SHRADDHA ON 31 JAN 2017
                                    calendar.Title = "CLCL";
                                    calendar.StatusColor = "#1c325d";
                                    ToUpdate = true;
                                    ExcludeTime = true;
                                }
                                else if (calendar.Title == "WPWP")
                                {
                                    //CHANGED Title by SHRADDHA ON 31 JAN 2017
                                    calendar.Title = "WPWP";
                                    calendar.StatusColor = "slateblue";
                                    ToUpdate = true;
                                  //  ExcludeTime = true;//COMMENTED BY SHRADDHA ON 09 FEB 2018 BECAUSE IT REQUIRES LOGIN AND LOGOUT TIME TO BE SHOWN ON CALENDER IN CASE OF WPWP
                                }
                                else if (calendar.Title == "AAPP")
                                {
                                    calendar.Title = "AAPP";
                                    calendar.StatusColor = "#f91cb8";
                                    ToUpdate = true;
                                }
                                else if (calendar.Title == "AAPP^")
                                {
                                    calendar.Title = "AAPP^";
                                    calendar.StatusColor = "#f91cb8";
                                    ToUpdate = true;
                                }
                                else if (calendar.Title == "PPAA^")
                                {
                                    calendar.Title = "PPAA^";
                                    calendar.StatusColor = "#f91cb8";
                                    ToUpdate = true;
                                }

                                else if (calendar.Title == "PPAA")
                                {
                                    calendar.Title = "PPAA";
                                    calendar.StatusColor = "#f91cb8";
                                    ToUpdate = true;
                                }

                                else if (calendar.Title != null && calendar.Title.Contains("HHHH"))
                                {
                                    //calendar.Title = "PPAA";
                                    //calendar.StatusColor = "#f91cb8";
                                    ToUpdate = true;
                                    ExcludeTime = true;//ADDED BY SHRADDHA ON 09 FEB 2018 BECAUSE IT REQUIRES LOGIN AND LOGOUT TIME NOT TO BE SHOWN ON CALENDER IN CASE OF HHHH
                                }

                                else
                                {
                                    calendar.StatusColor = "#3a87ad";
                                    ToUpdate = true;
                                }

                                if (ToUpdate == true)
                                {
                                    if (ExcludeTime == false)
                                    {
                                        if (TranDate == LoginDate)
                                        {
                                            calendar.Title = DailyTransactionObj.ShiftId + "\r\n" + StTime + "-" + EndTime + "\r\n" + calendar.Title;
                                        }
                                        else
                                        {
                                            calendar.Title = DailyTransactionObj.ShiftId + " - NS" + "\r\n" + StTime + "-" + EndTime + "\r\n" + calendar.Title;
                                        }
                                        //if (( DailyTransactionObj.Login.ToString("HH:mm") != "00:00") && (DailyTransactionObj.LogOut.ToString("HH:mm") != "00:00"))
                                        //{
                                        //    calendar.Title = (DailyTransactionObj.Login.ToString("HH:mm") + " - " + DailyTransactionObj.LogOut.ToString("HH:mm")) + "\r\n" + calendar.Title;
                                        //}
                                        //else
                                        //{
                                        //    //calendar.Title = calendar.Title;
                                        //}
                                    }
                                    calendar.AllDay = true;
                                    result.Add(calendar);
                                }

                                //calendar.Title = DailyTransactionObj.Login.ToString("hh:mm") + "\r\n" + DailyTransactionObj.LogOut.ToString("hh:mm") + "\r\n" + calendar.Title;
                                //calendar.AllDay = true; 
                                //result.Add(calendar);
                            }

                        }
                    }
                }
                return result;
            }
            //   return result
        }

        #region MYFUNCTION by MSJ
        private static List<string> MyFunction(DateTime date1, DateTime date2)
        {

            var listOfMonths = new List<string>();
            if (date1.CompareTo(date2) == 1)
            {
                var temp = date2;
                date2 = date1;
                date1 = temp;
            }

            var mosSt = date1.Month;
            var mosEn = date2.Month;
            var yearSt = date1.Year;
            var yearEn = date2.Year;

            while (mosSt < mosEn || yearSt < yearEn)
            {
                var temp = new DateTime(yearSt, mosSt, 1);
                listOfMonths.Add(temp.ToString("yyyy,MM", CultureInfo.InvariantCulture));
                mosSt++;
                if (mosSt < 13) continue;
                yearSt++;
                mosSt = 1;
            }

            return listOfMonths;
        }
        #endregion

    }

    public class Events
    {
        public string id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string date { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        // public string url { get; set; }
        public string color { get; set; }
        public string classname { get; set; }
        public bool allDay { get; set; }
    }

    // resource: html-color-codes  http://html-color-codes.info/
    // left part stores color, right part stores className for html rendering
    public enum AppointmentStatus
    {
        [Description("#330033:Sold")] // purple
        Sold,
        [Description("#FF8000:Available")] // orange
        Available,
        [Description("#FF0000:Tentative Appointment")] // red
        TentativeAppointment,
        [Description("#01DF3A:Booked")] //green
        Booked,
        [Description("#0000ff :Confirmed Appointment")] // blue
        ConfirmedAppointment,
        [Description("#FF8000:Open House")] // orange
        OpenHouse,


    }

    public static class Enums
    {
        /// Get all values
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        /// Get all the names
        public static IEnumerable<T> GetNames<T>()
        {
            return Enum.GetNames(typeof(T)).Cast<T>();
        }

        /// Get the name for the enum value
        public static string GetName<T>(T enumValue)
        {
            return Enum.GetName(typeof(T), enumValue);
        }

        /// Get the underlying value for the Enum string
        public static int GetValue<T>(string enumString)
        {
            return (int)Enum.Parse(typeof(T), enumString.Trim());
        }

        public static string GetEnumDescription<T>(string value)
        {
            Type type = typeof(T);
            var name = Enum.GetNames(type).Where(f => f.Equals(value, StringComparison.CurrentCultureIgnoreCase)).Select(d => d).FirstOrDefault();

            if (name == null)
            {
                return string.Empty;
            }
            var field = type.GetField(name);
            var customAttribute = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return customAttribute.Length > 0 ? ((DescriptionAttribute)customAttribute[0]).Description : name;
        }
    }
}