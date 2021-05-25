using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web.Mvc;
using WetosDB;

namespace WetosMVC.Controllers
{
    [SessionExpire] 
    [Authorize]
    public class WetosEmailController : BaseController
    {
        //ADDED BY SHRADDHA ON 23 JAN 2017 FOR CREATING OBJECT OF WetosDBEntities
        
        //
        // GET: /Email/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Added for Email Management_send mail
        /// Added by Rajas On 16 Nov 2016
        /// </summary>
        /// <returns></returns>
        //public ActionResult SendEmail(FormCollection collection)
        public ActionResult SendEmail(int ToEmail, string Subject, string Message, string SubjectText)
        {

            //ADDED CODE FOR GETTING EMAIL ID FROM EMPLOYEEID BY SHRADDHA ON 23 JAN 2017
            string EmailId = WetosDB.Employees.Where(a => a.EmployeeId == ToEmail).Select(a => a.Email).FirstOrDefault();
            EmailId = "devphp11@gmail.com";  //delete after testing

            var SubjectInt = Convert.ToInt32(Subject);
            string MsgSubject = WetosDB.DropdownDatas.Where(x => x.Value == SubjectInt && x.TypeId == 16).Select(x => x.Text).FirstOrDefault();

            if (SubjectText != "PreSelect" && SubjectText != "")
            {
                MsgSubject = SubjectText;
            }

            // SEND EMAIL START
            string SMTPServerName = ConfigurationManager.AppSettings["SMTPServerName"];
            string SMTPUsername = ConfigurationManager.AppSettings["SMTPUsername"];
            string SMTPPassword = ConfigurationManager.AppSettings["SMTPPassword"];
            string SMTPPort = ConfigurationManager.AppSettings["SMTPPort"];

            int SMTPPortInt = Convert.ToInt32(SMTPPort);
            //string Contact1 = ConfigurationManager.AppSettings["Contact1"];
            //string Contact2 = ConfigurationManager.AppSettings["Contact2"];
            //string Support = ConfigurationManager.AppSettings["Support"];

            //String name = //collection.Get("name");
            //String email = //collection.Get("email");
            //string subject = Subject;// //collection.Get("subject");
            // string message = Message; ////collection.Get("message");

            if (ModelState.IsValid)
            {
                try
                {
                    MailMessage msg = new MailMessage();
                    SmtpClient smtp = new SmtpClient();
                    //MailAddress from = new MailAddress(SMTPUsername); //"info@foodpatronservices.com");
                    MailAddress from = new MailAddress("devphp11@gmail.com");
                    StringBuilder sb = new StringBuilder();

                    //TO
                    msg.To.Add(new MailAddress(EmailId));
                    //msg.To.Add(EmailId); //"mjoshi@sushmatechnology.com");
                    //msg.To.Add(Contact1); //"mjoshi@sushmatechnology.com");
                    //msg.To.Add(email); //"mjoshi@sushmatechnology.com");

                    //BCC
                    //msg.Bcc.Add(Support);
                    //msg.Subject = Subject; // "Contact Us";
                    msg.Subject = MsgSubject;

                    msg.IsBodyHtml = false;
                    //msg.IsBodyHtml = true;  //to make message body as html  

                    msg.From = new MailAddress(SMTPUsername); //"info@foodpatronservices.com");
                    //msg.From = new MailAddress("devphp11@gmail.com");

                    smtp.Port = SMTPPortInt;
                    smtp.Host = SMTPServerName; // "smtpout.asia.secureserver.net";
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;

                    //smtp.Credentials = new System.Net.NetworkCredential("info@foodpatronservices.com", "info@fps");
                    //smtp.Credentials = new System.Net.NetworkCredential("devphp11@gmail.com", "php@2018");
                    smtp.Credentials = new System.Net.NetworkCredential(SMTPUsername, SMTPPassword);
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;


                    //sb.Append("Name: " + name);
                    //sb.Append(Environment.NewLine);
                    //sb.Append("Email: " + email);
                    //sb.Append(Environment.NewLine);
                    //sb.Append("Subject: " + subject);
                    //sb.Append(Environment.NewLine);
                    //sb.Append("Comments: " + message);
                    //Message

                    msg.Body = Message; // sb.ToString();

                    smtp.Send(msg);

                    msg.Dispose();

                    return Json(new { status = "Success" }, JsonRequestBehavior.AllowGet);
                    //return RedirectToAction("Index", "Home");
                }
                catch (Exception Ex)
                {
                    String path = System.Web.HttpContext.Current.Server.MapPath("~/ErrorLog/myFile.txt");
                    using (StreamWriter writer = new StreamWriter(path, true))
                    {
                        writer.WriteLine("Error in SendEmail: " + Ex.Message.ToString() + " _ " + (Ex.InnerException == null ? string.Empty : Ex.InnerException.Message) + " Stack trace: " + Ex.StackTrace.ToString());
                    }
                    return View("Error");
                }
            }
            // SEND EMAIL END
            return View();
        }

        /// <summary>
        /// CODE FOR ADDING TABLE TO SHOWING DAILY ATTENDANCE REPORT TO DIRECTOR
        /// ADDED BY SHRADDHA ON 13 FEB 2017
        /// <returns></returns>
        public ActionResult DailyAttendanceReportForDirector()
        {
            List<DailyAttendanceDataCls> DailyAttendanceDataClsList = new List<DailyAttendanceDataCls>();

            // GET CURRENT DATE AND PREV DATE
            DateTime CurrentDay = DateTime.Now.Date;
            DateTime PreviousDay = DateTime.Now.AddDays(-1).Date;           

            // LOCAL STORAGE FOR PREV DAY DATA
            List<DailyTransaction> DailyTransactionListForPreviousDay = WetosDB.DailyTransactions.Where(a => a.TranDate == PreviousDay).ToList();

            // LOCAL STORAGE FOR CURRENT DAY DATA
            List<DailyTransaction> DailyTransactionListForCurrentDay = WetosDB.DailyTransactions.Where(a => a.TranDate == CurrentDay).ToList();

            foreach (DailyTransaction DailyTransactionObj in DailyTransactionListForCurrentDay)
            {
                // GET EMPLOYEE DETAILS FROM EMPLOYEE TABLE
                Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == DailyTransactionObj.EmployeeId).FirstOrDefault();

                // CHECK NULL
                if (EmployeeObj != null)
                {                    
                    DailyAttendanceDataCls DailyAttendanceDataClsObj = new DailyAttendanceDataCls();

                    // ADD EMPLYEE FULL NAME
                    DailyAttendanceDataClsObj.EmployeeName = EmployeeObj.FirstName + " " + EmployeeObj.MiddleName + " " + EmployeeObj.LastName;

                    // ADD PREV DAY INFO
                    DailyTransaction DailyTransactionForPreviousDay = DailyTransactionListForPreviousDay.Where(a => a.EmployeeId == DailyTransactionObj.EmployeeId).FirstOrDefault();
                    DailyAttendanceDataClsObj.OutTime = DailyTransactionForPreviousDay.LogOut.ToString("HH:mm");
                    DailyAttendanceDataClsObj.PreviousDay = DailyTransactionForPreviousDay.TranDate.ToString("dd-MMM-yyyy");
                    DailyAttendanceDataClsObj.Status = DailyTransactionForPreviousDay.Status;
                    
                    // ADD PREV DAY INFO
                    DailyTransaction DailyTransactionForCurrentDay = DailyTransactionListForCurrentDay.Where(a => a.EmployeeId == DailyTransactionObj.EmployeeId).FirstOrDefault();
                    DailyAttendanceDataClsObj.InTime = DailyTransactionForCurrentDay.Login.ToString("HH:mm");
                    DailyAttendanceDataClsObj.CurrentDay = DailyTransactionForCurrentDay.TranDate.ToString("dd-MMM-yyyy");

                    // ADD NEW CLASS OBJECT T LIST
                    DailyAttendanceDataClsList.Add(DailyAttendanceDataClsObj);
                }
            }

            //List<DailyTransaction> DailyTransactionListForCurrentDay = WetosDB.DailyTransactions.Where(a => a.TranDate == CurrentDay).ToList();

            //foreach (var DailyTransactionObjForCurrentDay in DailyTransactionListForCurrentDay)
            //{
            //    DailyAttendanceDataClsObj.InTime = DailyTransactionObjForCurrentDay.Login.ToString("HH:mm");

            //}           

            // PASS LIST TO VIEW AS MODEL
            return View(DailyAttendanceDataClsList);
        }

        /// <summary>
        /// CODE FOR ADDING EMPLOYEE AS WELL AS DAILY ATTENDANCE DATA INTO SINGLE CLASS  
        /// ADDED BY SHRADDHA ON 13 FEB 2017
        /// <returns></returns>
        public class DailyAttendanceDataCls
        {

            public string EmployeeName { get; set; }
            public string Status { get; set; }
            public string PreviousDay { get; set; }
            public string CurrentDay { get; set; }
            public string InTime { get; set; }
            public string OutTime { get; set; }

        }

    }
}
