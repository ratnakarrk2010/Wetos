using System;
using System.Globalization;
using System.Linq;
using Quartz;
using Quartz.Impl;
using WetosDB;
using System.Collections.Generic;
using WetosMVC.Controllers;

namespace WetosMVC.Controllers
{
    // [SessionExpire] 
    public class Jobclass : IJob
    {
        WetosDBEntities WetosDB = new WetosDBEntities();

        private static DateTime GetSaturdayByWeek(DateTime dateofMonth, int weekNumber)
        {
            DateTime firstDateofMonth = new DateTime(dateofMonth.Year, dateofMonth.Month, 1);
            DateTime resultDate = CultureInfo.InvariantCulture.Calendar.AddWeeks(firstDateofMonth, weekNumber - 1);
            int day = Convert.ToInt32(resultDate.DayOfWeek) < 6 ? (Convert.ToInt32(resultDate.DayOfWeek) - 6) * -1 : 0;
            return resultDate.AddDays(day);
        }

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                // Region added by Rajas on 20 MAY 2017
                #region EXTRA CODE FROM SCHEDULAR TO GET FROM AND TO DATE
                // UPDATE DAILY TRANSACTION BASED ON COMMAN DATA FOR SELECTED DATE IN DATE RANGE (ALL DAYS) START

                //CODE UPDATED BY SHRADDHA ON 11 AUG 2017 TAKEN ORDERBYDESCENDING (TRANDATE) INSTEAD OD DAILYTRNID START (NEED TO VERIFY)
                DateTime fromdate = WetosDB.DailyTransactions.OrderByDescending(a => a.TranDate).Select(a => a.TranDate).FirstOrDefault();
                //CODE UPDATED BY SHRADDHA ON 11 AUG 2017 TAKEN ORDERBYDESCENDING (TRANDATE) INSTEAD OD DAILYTRNID END (NEED TO VERIFY)

                if (fromdate == null)
                {
                    fromdate = DateTime.Now.AddDays(-1);  // PROCESS DATE FOR TWO DAYS TODAY AND YESTERDAY ADDDED BY MSJ ON 04 MARCH 2018
                }

                //// CommonDataId replaced with cd_id, by Rajas on 5 APRIL 2017
                //CommanData CommonDataObj = WetosDB.CommanDatas.OrderByDescending(a => a.cd_id).FirstOrDefault();

                DateTime todate = DateTime.Now;
                //if (CommonDataObj != null)
                //{
                //    todate = Convert.ToDateTime(CommonDataObj.CmnDate);

                //}

                //DateTime fromdate = DateTime.Now;
                //DateTime todate = DateTime.Now;

                List<string> EmployeeList = null;
                #endregion

                string ErrorMessage = string.Empty;  // Added by Rajas on 18 AUGUST 2017

                // COMMON POSTING FUNCTION
                // Updated by Rajas on 18 AUGUST 2017 for ref ErrorMessage
                WetosAdministrationController.ProcessAttendenceEx(WetosDB, fromdate, todate, EmployeeList, ref ErrorMessage);

                //ADDED CODE BY SHRADDHA ON 22 NOV 2017 2017 START
                if (!string.IsNullOrEmpty(ErrorMessage))
                {
                    AuditLog AuditLogObj = new AuditLog();
                    AuditLogObj.AuditDate = DateTime.Now;
                    AuditLogObj.FormName = "SYSTEM";
                    AuditLogObj.AuditMode = "Error in Posting Attendance:";
                    AuditLogObj.NewRecord = ErrorMessage.Length > 1000 ? ErrorMessage.Substring(0, 990) : ErrorMessage; // ADDED BY MSJ ON 11 JAN 2018
                    WetosDB.AuditLogs.Add(AuditLogObj);
                    WetosDB.SaveChanges();
                }

                //ADDED CODE BY SHRADDHA ON 22 NOV 2017 2017 END
            }
            catch (System.Exception ex)
            {
                AuditLog AuditLogObj = new AuditLog();
                AuditLogObj.AuditDate = DateTime.Now;
                AuditLogObj.FormName = "SYSTEM";
                AuditLogObj.AuditMode = "Exception Occured:";
                AuditLogObj.NewRecord = ex.Message;
                WetosDB.AuditLogs.Add(AuditLogObj);
                WetosDB.SaveChanges();
            }
        }

    }

    /// <summary>
    /// Updated by Rajas on 31 AUGUST 2017
    /// </summary>
    public class JobclassForEmail : IJob
    {
        WetosDBEntities WetosDB = new WetosDBEntities();

        public void Execute(IJobExecutionContext context)
        {
            //ADDED CODE BY SHRADDHA ON 16 SEP 2017 START
            AuditLog AuditLogObj = new AuditLog();
            AuditLogObj.AuditDate = DateTime.Now;
            AuditLogObj.FormName = "SYSTEM";
            AuditLogObj.AuditMode = "EMAIL SCHEDULING";
            AuditLogObj.NewRecord = "EMAIL IS SENT";
            WetosDB.AuditLogs.Add(AuditLogObj);
            WetosDB.SaveChanges();
            //ADDED CODE BY SHRADDHA ON 16 SEP 2017 END
        }
    }

    /// <summary>
    /// Job Scheduler
    /// </summary>
    public class JobScheduler
    {
        /// <summary>
        /// Start
        /// </summary>
        public static void Start()
        {
            try
            {
                IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();

                WetosDBEntities WetosDB = new WetosDBEntities();

                //ADDED CODE BY SHRADDHA ON 16 SEP 2017 START
                AuditLog AuditLogObj = new AuditLog();
                AuditLogObj.AuditDate = DateTime.Now;
                AuditLogObj.FormName = "SYSTEM";
                AuditLogObj.AuditMode = "SCHEDULER START";
                AuditLogObj.NewRecord = "SCHEDULER START";
                WetosDB.AuditLogs.Add(AuditLogObj);
                WetosDB.SaveChanges();
                //string ScheduleFlag = WetosDB.GlobalSettings.Where(a => a.SettingText == "DefaultSchedule").Select(a => a.SettingValue).FirstOrDefault();
                //if (ScheduleFlag == "1")
                //{
                //    scheduler.Start();

                //}

                // Added by Rajas on 30 AUGUST 2017 START
                // Get list of Available Schedular
                // List of Active schedulars only. Updated by Rajas on 31 AUGUST 2017
                List<Schedular> SchedularList = WetosDB.Schedulars.Where(a => a.SchedularStatus == true).ToList();

                if (SchedularList.Count > 0)
                {
                    //ADDED CODE BY SHRADDHA ON 16 SEP 2017 START
                    AuditLog AuditLogObj2 = new AuditLog();
                    AuditLogObj2.AuditDate = DateTime.Now;
                    AuditLogObj2.FormName = "SYSTEM";
                    AuditLogObj2.AuditMode = "SCHEDULER ON";
                    AuditLogObj2.NewRecord = "SCHEDULER START";
                    WetosDB.AuditLogs.Add(AuditLogObj2);
                    WetosDB.SaveChanges();
                    scheduler.Start();

                }

                string ScheduleDefaultMinTime = string.Empty;
                string ScheduleDefaultMaxTime = string.Empty;
                string ScheduleDefaultRepeatTime = string.Empty;
                string ScheduleDefaultSetTime = string.Empty;

                // ScheduleId = 1 is DataProcess
                Schedular ActualSchedularObj = SchedularList.Where(a => a.ScheduleId == 1).FirstOrDefault();

                if (ActualSchedularObj != null)
                {
                    ScheduleDefaultMinTime = Convert.ToString(ActualSchedularObj.StartTime.Value.Hour);

                    ScheduleDefaultMaxTime = Convert.ToString(ActualSchedularObj.EndTime.Value.Hour);

                    ScheduleDefaultRepeatTime = Convert.ToString(ActualSchedularObj.RepeatCycle);
                    ScheduleDefaultSetTime = Convert.ToString(ActualSchedularObj.FrequencyInMin);

                    IJobDetail job = JobBuilder.Create<Jobclass>().Build();
                    if (!string.IsNullOrEmpty(ScheduleDefaultMinTime) && !string.IsNullOrEmpty(ScheduleDefaultMaxTime) 
                        && !string.IsNullOrEmpty(ScheduleDefaultRepeatTime) && !string.IsNullOrEmpty(ScheduleDefaultSetTime))
                    {
                        ITrigger trigger = TriggerBuilder.Create()
                          .WithIdentity("trigger1", "group1")
                          .StartNow()
                          .WithSchedule(CronScheduleBuilder.CronSchedule(ScheduleDefaultSetTime + " 0/" + ScheduleDefaultRepeatTime + " " + ScheduleDefaultMinTime + "-" + ScheduleDefaultMaxTime + " ? * *"))
                            //.WithSimpleSchedule(x => x
                            //.WithIntervalInHours(24)

                          //.RepeatForever())
                          .Build();

                        scheduler.ScheduleJob(job, trigger);

                        //ADDED CODE BY SHRADDHA ON 16 SEP 2017 START
                        AuditLog AuditLogObj3 = new AuditLog();
                        AuditLogObj3.AuditDate = DateTime.Now;
                        AuditLogObj3.FormName = "SYSTEM";
                        AuditLogObj3.AuditMode = "POSTING SCHEDULING ON";
                        AuditLogObj3.NewRecord = "POSTING SCHEDULER START";
                        WetosDB.AuditLogs.Add(AuditLogObj3);
                        WetosDB.SaveChanges();
                    }
                    // Added by Rajas on 30 AUGUST 2017 END
                }

                // Added by Rajas on 31 AUGUST 2017 START
                // ScheduleId = 2 is Email send
                ActualSchedularObj = SchedularList.Where(a => a.ScheduleId == 2).FirstOrDefault();

                if (ActualSchedularObj != null)
                {
                    int TriggerDay = DateTime.Now.Day;
                    if (TriggerDay == 16)
                    {
                        ScheduleDefaultMinTime = Convert.ToString(ActualSchedularObj.StartTime.Value.Hour);

                        ScheduleDefaultMaxTime = Convert.ToString(ActualSchedularObj.EndTime.Value.Hour);

                        ScheduleDefaultRepeatTime = Convert.ToString(ActualSchedularObj.RepeatCycle);
                        ScheduleDefaultSetTime = Convert.ToString(ActualSchedularObj.FrequencyInMin);

                        IJobDetail EmailSend = JobBuilder.Create<JobclassForEmail>().Build();
                        if (!string.IsNullOrEmpty(ScheduleDefaultMinTime) && !string.IsNullOrEmpty(ScheduleDefaultMaxTime) && !string.IsNullOrEmpty(ScheduleDefaultRepeatTime) && !string.IsNullOrEmpty(ScheduleDefaultSetTime))
                        {
                            ITrigger trigger = TriggerBuilder.Create()
                             .WithIdentity("trigger2", "group2")
                             .StartNow()
                             .WithSchedule(CronScheduleBuilder.CronSchedule(ScheduleDefaultSetTime + " 0/" + ScheduleDefaultRepeatTime + " " + ScheduleDefaultMinTime + "-" + ScheduleDefaultMaxTime + " " + TriggerDay + " * ?"))
                                //.WithSimpleSchedule(x => x
                                //.WithIntervalInHours(24)

                             //.RepeatForever())
                             .Build();
                            scheduler.ScheduleJob(EmailSend, trigger);
                            //ADDED CODE BY SHRADDHA ON 16 SEP 2017 START
                            AuditLog AuditLogObj4 = new AuditLog();
                            AuditLogObj4.AuditDate = DateTime.Now;
                            AuditLogObj4.FormName = "SYSTEM";
                            AuditLogObj4.AuditMode = "EMAIL SCHEDULING ON";
                            AuditLogObj4.NewRecord = "EMAIL SCHEDULER START";
                            WetosDB.AuditLogs.Add(AuditLogObj4);
                            WetosDB.SaveChanges();
                        }
                    }
                }


            }
            catch (System.Exception ex)
            {
                // ADDED TRY CATCH BY MSJ ON 18 JAN 2018 START
                try
                {

                    WetosDBEntities WetosDB = new WetosDBEntities();
                    AuditLog AuditLogObj4 = new AuditLog();
                    AuditLogObj4.AuditDate = DateTime.Now;
                    AuditLogObj4.FormName = "SYSTEM";
                    AuditLogObj4.AuditMode = "SCHEDULING START EXCEPTION:";
                    AuditLogObj4.NewRecord = ex.Message;
                    WetosDB.AuditLogs.Add(AuditLogObj4);
                    WetosDB.SaveChanges();
                    //throw; // COMMENTED BY MSJ
                }
                catch (Exception)
                {
                    //throw;
                }
                // ADDED TRY CATCH BY MSJ ON 18 JAN 2018 END
            }
        }
    }
}
