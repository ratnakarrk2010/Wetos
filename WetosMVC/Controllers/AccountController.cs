using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WETOS.Util;
using WetosDB;
using WetosMVC.Models;
using WetosMVCMainApp.Models;
using System.Data.Entity;
using WetosMVCMainApp.Utilities;
//using System.Net.Http;

namespace WetosMVC.Controllers
{
    public class AccountController : BaseController
    {
        //
        // GET: /Account/LogOn
        public ActionResult FinancialYear()
        {
            var username = (Session["Username"]).ToString();
            ViewData["user"] = username;
            return View();
            //return RedirectToAction("FinancialYear", "Account");
        }

        [HttpPost]
        public ActionResult FinancialYear(string returnUrl)
        {
            //if (Session["UserRole"].ToString() == "User")
            //{
            //    return RedirectToAction("Dashboard", "Salary");
            //}

            //else if (Session["UserRole"].ToString() == "HR Manager")
            //{
            //    return RedirectToAction("Index", "ManagerDashboard");
            //}
            //else if (Session["UserRole"].ToString() == "Manager")
            //{
            //    return RedirectToAction("Index", "ManagerDashboard");
            //}
            //else if (Session["UserRole"].ToString() == "Director")
            //{
            //    return RedirectToAction("Index", "ManagerDashboard");
            //}
            //else
            //{

            //Temporary Commented By Shraddha on 27 DEC 2016 to redirect to another actionresult
            //return RedirectToAction("Index", "ManagerDashboard");

            //Temporary Added By Shraddha on 27 DEC 2016 to redirect to another actionresult
            return RedirectToAction("CalendarIndex", "ManagerDashboard");
            //}

        }

        public ActionResult LogOn()
        {
            //string Entrypt1Str = WETOS.Util.CryptorEngine.Encrypt("Ajanta Pharma Paithan|1|1|15-04-2021|1000|05DC3428-A876-45D1-BFB2-23A3A0EA268F|2370EBB3-FA82-40CB-ABCA-42DDF53E1A51", true);
            //string Entrypt2Str = WETOS.Util.CryptorEngine.Encrypt(Entrypt1Str, true);
            ////string Decrypt1Str = WETOS.Util.CryptorEngine.Decrypt(Entrypt2Str, true);
            ////string Decrypt2Str = WETOS.Util.CryptorEngine.Decrypt(Decrypt1Str, true);

            //string LicensePath = Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["VirtualDir"] + "/App_Data/WETOS.ini");

            //bool IsExist = System.IO.File.Exists(LicensePath);

            //if (IsExist == false) // License file exist
            //{
            //    System.IO.File.WriteAllText(LicensePath, Entrypt2Str);

            //}

            //using (var client = new HttpClient())
            //{
            //    client.BaseAddress = new Uri("http://localhost:63316/api/PostProduct");

            //    ProductModel ABCD = new ProductModel();
            //    ABCD.ProductId = 13;
            //    ABCD.ProductKey = "1234567890";
            //    ABCD.Description = "1234567890";
            //    ABCD.CurrentVersion = "1234567890";
            //    ABCD.ReleaseDate = new DateTime(2020, 04, 15);
            //    ABCD.ValidityDate = 1;
            //    ABCD.Status = 1;

            //    //HTTP POST
            //    var postTask = client.PostAsJsonAsync<ProductModel>("Product", ABCD);
            //    postTask.Wait();

            //    var result = postTask.Result;
            //    if (result.IsSuccessStatusCode)
            //    {
            //        string returnStr = result.Content.ReadAsStringAsync().Result;// result.Content.ReadAsStringAsync();
            //        return RedirectToAction("Index");
            //    }
            //}

            //ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");           

            return View();
        }

        //
        // POST: /Account/LogOn

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            try
            {
                // LOGIN CODE START
                Cryptography objcrypt = new Cryptography();
                string username = model.UserName;
                string password = objcrypt.EncryptPassword(model.Password.Trim());

                var pass = objcrypt.DecryptPassword("w+86oW2qltYIY79MXiR5fQ==");
                if (string.IsNullOrEmpty(username) == true || string.IsNullOrEmpty(password) == true)
                {
                    ModelState.AddModelError("", "Please provide your user id and password");
                    return View(model);
                }

                // MODIFIED BY MSJ ON 23 JAN 2020 to allow only incrypted password
                User UserObj = WetosDB.Users.Where(u => u.UserName == username && u.Password == password).FirstOrDefault();               

                if (UserObj == null)
                {
                    ModelState.AddModelError("AddModelError", "Please enter correct user id and password");
                    return View(model);
                }

                Session["EmployeeNo"] = UserObj.EmployeeId;

                RoleDef RoleDefObj = WetosDB.RoleDefs.Where(a => a.RoleId == UserObj.RoleTypeId).FirstOrDefault();

                if (RoleDefObj != null)
                {
                    Session["UserRole"] = RoleDefObj.RoleName;

                    if (RoleDefObj.RoleName.ToUpper() == "BRANCH ADMIN")
                    {
                        Session["BranchAdmin"] = UserObj.BranchId;
                    }
                    else
                    {
                        Session["BranchAdmin"] = 0;
                    }
                }
                else
                {
                    Session["UserRole"] = "GENERAL STAFF";
                }

                //COMMENTED BY SHRADDHA AND ADDED NEW CODE ON 03 NOV 2017 START               
                SessionPersister.UserID = UserObj.UserId;
                SessionPersister.UserName = UserObj.UserName;
                SessionPersister.LastLogin = DateTime.Now.ToShortDateString();
                SessionPersister.DepartmentId = UserObj.DepartmentId;
                //COMMENTED BY SHRADDHA AND ADDED NEW CODE ON 03 NOV 2017 END

                //ADDED NEW CODE BY SHRADDHA ON 03 NOV 2017 TO GET ALL GLOBALSETTING VALUES IN SESSION PERSISTER START

                //GET LIST OF GLOBAL SETTING
                List<GlobalSetting> GlobalSettingList = WetosDB.GlobalSettings.ToList();
                if (GlobalSettingList.Count > 0)
                {
                    //TO GET WHETHER APPLICATION FROM CALENDAR IS ALLOWED OR NOT
                    //string IsCalendarApplicationOn = GlobalSettingList.Where(a => a.SettingText == "Is Calendar Application On").Select(a => a.SettingValue).FirstOrDefault();

                    //ABOVE LINE COMMENTED AND BELOW LINE IS ADDED BY SHALAKA ON 13TH DEC 2017
                    string IsCalendarApplicationOn = GlobalSettingList.Where(a => a.SettingText == GlobalSettingsConstant.IsCalendarApplicationOn)
                        .Select(a => a.SettingValue).FirstOrDefault();

                    if (!string.IsNullOrEmpty(IsCalendarApplicationOn))
                    {
                        SessionPersister.IsCalendarApplicationOn = Convert.ToInt32(IsCalendarApplicationOn);

                    }

                    //TO GET WHETHER APPLICATION FROM CALENDAR IS ALLOWED OR NOT
                    //string DefaultLang = GlobalSettingList.Where(a => a.SettingText == "DefaultLang").Select(a => a.SettingValue).FirstOrDefault();

                    //ABOVE LINE COMMENTED AND BELOW LINE IS ADDED BY SHALAKA ON 13TH DEC 2017
                    string DefaultLang = GlobalSettingList.Where(a => a.SettingText == GlobalSettingsConstant.DefaultLang).Select(a => a.SettingValue)
                        .FirstOrDefault();

                    if (!string.IsNullOrEmpty(DefaultLang))
                    {
                        SessionPersister.DefaultLang = DefaultLang == null ? "en" : DefaultLang.Trim();

                    }
                }
                //ADDED NEW CODE BY SHRADDHA ON 03 NOV 2017 TO GET ALL GLOBALSETTING VALUES IN SESSION PERSISTER END    

                Employee EmployeeObj = WetosDB.Employees.Where(a => a.EmployeeId == UserObj.EmployeeId).FirstOrDefault();

                if (EmployeeObj != null)
                {
                    DateTime ConsiderableLeavingDate = new DateTime(1900, 01, 01);
                    if ((EmployeeObj.Leavingdate != null && EmployeeObj.Leavingdate != ConsiderableLeavingDate) && EmployeeObj.Leavingdate < DateTime.Now)
                    {

                        ModelState.AddModelError("", "UserName not found");
                        return View(model);
                    }
                }

                //Modified By Shraddha on 27th OCT 2016 for user authentication Start
                Session["UserRoleId"] = UserObj.RoleTypeId;
                //Modified By Shraddha on 27th OCT 2016 for user authentication End

                // UPDATED BY MSJ ON 10 DEC 2018
                Employee LoggedInEmp = WetosDB.Employees.Where(a => a.EmployeeId == UserObj.EmployeeId).FirstOrDefault();

                string FirstName = LoggedInEmp == null ? string.Empty : LoggedInEmp.FirstName; // UPDATED BY MSJ ON 10 DEC 2018
                string LastName = LoggedInEmp == null ? string.Empty : LoggedInEmp.LastName; // UPDATED BY MSJ ON 10 DEC 2018
                Session["Username"] = FirstName + " " + LastName;
                Session["Id"] = UserObj.EmployeeId;
                ViewData["Username"] = UserObj.UserName;
                //Session["EmployeeNo"] = user.EmployeeNo;
                TempData["EmployeeNo"] = UserObj.EmployeeId;

                //ADDED BY SHRADDHA ON 15 FEB 2017 START
                //Session["VersionName"] = "Test Version";
                //ADDED BY SHRADDHA ON 15 FEB 2017 END

                //ADDED BY SHRADDHA ON 28 DEC 2016
                Session["Picture"] = WetosDB.Employees.Where(a => a.EmployeeId == UserObj.EmployeeId).Select(a => a.Picture).FirstOrDefault();
                //Session["Picture"] = null;
                //ADDED BY SHRADDHA ON 07 FEB 2017 FOR SHOWING DIFFERENT IMAGES FOR DIFFERENT GENDER START

                //string Gender = EmployeeObj.Sex;
                Session["Gender"] = LoggedInEmp == null ? string.Empty : LoggedInEmp.Gender; // UPDATED BY MSJ ON 10 DEC 2018

                //ADDED BY SHRADDHA ON 07 FEB 2017 FOR SHOWING DIFFERENT IMAGES FOR DIFFERENT GENDER END

                int CompanyId = WetosDB.Employees.Where(a => a.EmployeeId == UserObj.EmployeeId).Select(a => a.CompanyId).FirstOrDefault();
                string CompanyName = WetosDB.Companies.Where(a => a.CompanyId == CompanyId).Select(a => a.CompanyName).FirstOrDefault();
                Session["CompanyName"] = CompanyName;
                //Added By Shraddha on 14Th Oct 2016 For CompanyId and BranchId

                // Added by Rajas on 26 JULy 2017 START
                Session["CompanyNameFooter"] = ConfigurationManager.AppSettings["CompanyNameFooter"];
                Session["CompanyLink"] = ConfigurationManager.AppSettings["CompanyLink"];
                Session["ProductName"] = ConfigurationManager.AppSettings["Product"];
                // Added by Rajas on 26 JULy 2017 END

                Session["CompanyId"] = UserObj.CompanyId;
                Session["BranchId"] = UserObj.BranchId;

                //ADDED BY PUSHKAR ON 3 APRIL 2017 for IDs to be taken in financial year
                int comId = UserObj.CompanyId;
                int branchid = UserObj.BranchId;
                //ADDED BY PUSHKAR ON 3 APRIL 2017 for IDs to be taken in financial year

                // LOGIN CODE END

                int NotificationCount = WetosDB.Notifications.Where(a => a.ToID == UserObj.EmployeeId && (a.ReadFlag == false || a.ReadFlag == null)).Count();
                Session["NotificationCount"] = NotificationCount;

                SessionPersister.YearInfo = new YearInfo();

                SessionPersister.YearInfo.y_id = 1; // yearinfo.y_id;

                //ADDED CODE BY SHRADDHA ON 22 MARCH 2017 START
                //CODE TO SAVE CURRENT FINANCIAL YEAR IN SESSION
                //string YearInfo = WetosDB.GlobalSettings.Where(a => a.SettingText == "CurrentFinancialYear").Select(a => a.SettingValue).FirstOrDefault();

                //ABOVE LINE COMMENTED AND BELOW LINE IS ADDED BY SHALAKA ON 13TH DEC 2017 
                string YearInfo = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.CurrentFinancialYear).Select(a => a.SettingValue).FirstOrDefault();
                Session["CurrentFinancialYear"] = YearInfo;



                //ADDED CODE BY SHRADDHA ON 22 MARCH 2017 END

                SessionPersister.UserInfo = new UserInfo();

                //SessionPersister.UserInfo.UserName = empinfo.Title + " " + empinfo.FirstName + " " + empinfo.MiddleName + " " + empinfo.LastName;
                SessionPersister.UserInfo.UserId = WetosDB.Employees.Where(a => a.EmployeeId == UserObj.EmployeeId).Select(a => a.EmployeeId).FirstOrDefault();

                // Check whether download folder is available, if not exist add download folder
                // Added by Rajas on 6 JULY 2017 START
                //bool IsExist = System.IO.Directory.Exists(Server.MapPath("~/User_Data/download/"));

                //if (!IsExist)
                //{
                //    System.IO.Directory.CreateDirectory(Server.MapPath("~/User_Data/download/"));
                //}
                //// Added by Rajas on 6 JULY 2017 END

                ////ADDED BY PUSHKAR ON 3 APRIL 2017 for series
                ////SessionPersister.Series = MakeSeries(comId, branchid);
                //// ADDED BY PUSHKAR FOR AuditLog ON 21 DEC 2016

                ////Added by Pushkar on 30 MARCH 2017 for DELETION of DOWNLOADS
                //System.IO.DirectoryInfo di = new DirectoryInfo(HttpContext.Server.MapPath("~/User_Data/download/"));
                ////TRY CATCH BLOCK ADDED BY SHRADDHA ON 06 MAR 2018
                //try
                //{
                //    foreach (FileInfo FileToDelete in di.GetFiles().Where(a => a.LastAccessTime < DateTime.Now.AddMinutes(5)))
                //    {
                //        //TRY CATCH BLOCK ADDED BY SHRADDHA ON 06 MAR 2018
                //        try
                //        {
                //            FileToDelete.Delete();
                //        }
                //        catch (Exception E)
                //        {
                //            AddAuditTrail("Error in file delete from download folder" + E.StackTrace); //(E.InnerException == null ? string.Empty : E.InnerException.Message));
                //        }
                //    }
                //}
                //catch (Exception E)
                //{
                //    AddAuditTrail("Error in file delete from download folder" + E.StackTrace);// E.InnerException == null ? string.Empty : E.InnerException.Message );
                //}
                //Added by Pushkar on 30 MARCH 2017 for DELETION of DOWNLOADS

                AddAuditTrail("Logged In");

                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 28 OCT 2017 TO EXTEND EXPIRY DATE TILL NOV ENDING START

                // ADDED BY MSJ ON 23 FEB 2020 START
                //string FailureText = string.Empty;

                //LoginUserAccessRights objUser = new LoginUserAccessRights();

                //CryptorEngineUtil CryptorEngine = new CryptorEngineUtil();

                ////string iniFile = ConfigurationManager.AppSettings["iniFile"].ToString();
                //// bool isLicensed = CryptorEngine.ReadIni(iniFile);//System.Configuration.ConfigurationManager.AppSettings["VirtualDir"] +

                //string LicensePath = Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["VirtualDir"] + "/App_Data/WETOS.ini");

                //bool IsExist = System.IO.File.Exists(LicensePath);

                //if (IsExist == true) // License file exist
                //{
                //    bool isLicensed = CryptorEngine.ReadIni(LicensePath);
                //    try
                //    {
                //        if (isLicensed && CryptorEngine.ValidateWetos(ref FailureText))
                //        {
                //        }
                //        else
                //        {
                //            if (!string.IsNullOrEmpty(FailureText))
                //            {
                //                string ErrorMessage = FailureText;
                //                Attention(ErrorMessage);

                //                ModelState.AddModelError("", ErrorMessage);
                //                return View(model);
                //            }
                //        }
                //    }
                //    catch (System.Exception ex)
                //    {
                //        //AddAuditTrail(ex.Message);// + ( ex.InnerException == null ? string.Empty : ex.InnerException.Message));  // Added by Rajas on 22 SEP 2017

                //        AddAuditTrail(ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));  // Added by Rajas on 22 SEP 2017
                //        //ModelState.AddModelError("", "Unable to connect to DB");

                //        ModelState.AddModelError("", "Unable to connect to DB" + ex.StackTrace);
                //        return View(model);
                //    }
                //}

                DateTime ExpireDate = new DateTime(2021, 04, 15);

                //int VV = 0;
                //int WW = 10 / VV;

                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 28 OCT 2017 TO EXTEND EXPIRY DATE TILL NOV ENDING END
                if (DateTime.Now.Date > ExpireDate)
                {
                    //Attention("Login failed. Please Contact Administrator."); //CHANGED COMMENT FROM Please Contact STPL TO Please Contact Administration BY SHRADDHA ON 01 NOV 2017
                    //return View(model);
                    ModelState.AddModelError("", "Login failed. Please Contact Administrator.");
                    return View(model);
                }
                else
                {
                    //if (Membership.ValidateUser(model.UserName, model.Password))
                    //{
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);

                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        //string UpdateStatus = string.Empty;
                        //int ErrorNumber = 0;

                        // Added by shalaka on 17th July 2017  ---Start
                        //int CompanyListCount = 0;
                        //int BranchListCount = 0;
                        //int DepartmentListCount = 0;
                        //int DesignationListCount = 0;
                        //int GradeListCount = 0;
                        //int EmployeeListCount = 0;
                        //int EmployeeGroupListCount = 0;
                        //int LeaveMasterListCount = 0;
                        //int LeaveBalanceListCount = 0;
                        //int LeaveCreditListCount = 0;
                        //int UserListCount = 0;
                        //int ShiftCount = 0;
                        //int FinancialYearCount = 0;
                        //int UserRoleCount = 0;
                        //int RuleEngineDataCount = 0; //CODE ADDED BY SHRADDHA ON 08 FEB 2018

                        // To check for data is consistent or not
                        // Modified by shalaka on 17th July 2017
                        //if (IsDataConsistent(WetosDB, ref UpdateStatus, ref ErrorNumber,
                        //    ref CompanyListCount, ref BranchListCount, ref DepartmentListCount, ref DesignationListCount, ref GradeListCount,
                        //    ref EmployeeListCount, ref EmployeeGroupListCount, ref LeaveMasterListCount, ref LeaveBalanceListCount, ref LeaveCreditListCount,
                        //    ref UserListCount, ref ShiftCount, ref FinancialYearCount, ref UserRoleCount, ref RuleEngineDataCount) == false)
                        //{
                        //    //AddAuditTrail(UpdateStatus);

                        //    // Added by Rajas on 6 JULY 2017 START
                        //    if (RoleDefObj.RoleId == 1 || RoleDefObj.RoleId == 6)
                        //    {
                        //        //Error(UpdateStatus);
                        //        ViewBag.ErrorNumber = ErrorNumber;

                        //        // Modified by shalaka on 17th July 2017--- Start
                        //        ViewBag.CompanyListCount = CompanyListCount;
                        //        ViewBag.BranchListCount = BranchListCount;
                        //        ViewBag.DepartmentListCount = DepartmentListCount;
                        //        ViewBag.DesignationListCount = DesignationListCount;
                        //        ViewBag.GradeListCount = GradeListCount;
                        //        ViewBag.EmployeeListCount = EmployeeListCount;
                        //        ViewBag.EmployeeGroupListCount = EmployeeGroupListCount;
                        //        ViewBag.LeaveMasterListCount = LeaveMasterListCount;
                        //        ViewBag.LeaveBalanceListCount = LeaveBalanceListCount;
                        //        ViewBag.LeaveCreditListCount = LeaveCreditListCount;
                        //        ViewBag.UserListCount = UserListCount;
                        //        ViewBag.ShiftCount = ShiftCount;
                        //        ViewBag.FinancialYearCount = FinancialYearCount;
                        //        ViewBag.UserRoleCount = UserRoleCount;

                        //        // Modified by shalaka on 17th July 2017--- End

                        //        return RedirectToAction("ImportData", "WetosAdministration");
                        //    }
                        //    else // Login not allowed if data is not consistent
                        //    {

                        //        //Information("Access denied. Reason : system is not configured properly. Please contact system admin");
                        //        return View(model);
                        //    }
                        //    // Added by Rajas on 6 JULY 2017 END
                        //}

                        if (EmployeeObj == null)
                        {
                            ModelState.AddModelError("", "Please Login from User other than Admin");
                            return View(model);

                        }
                        //REDIRECTED TO HOME/INDEX INSTEAD OF FINANCIAL YEAR BY SHRADDHA ON 12 JAN 2017
                        return RedirectToAction("Index", "Home");
                    }

                }

            }
            catch (System.Exception ex)
            {
                //AddAuditTrail(ex.Message);// + ( ex.InnerException == null ? string.Empty : ex.InnerException.Message));  // Added by Rajas on 22 SEP 2017

                AddAuditTrail(ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message));  // Added by Rajas on 22 SEP 2017
                //ModelState.AddModelError("", "Unable to connect to DB");

                ModelState.AddModelError("", "Unable to connect to DB" + ex.StackTrace);
                return View(model);
            }


            // If we got this far, something failed, redisplay form
            //return View(model);
        }

        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registration(RegisterModel model, string returnUrl)
        {
            return View();
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {

            // ADDED BY PUSHKAR FOR AuditLog ON 21 DEC 2016
            AddAuditTrail("Logged Off");

            //
            // COMMENTED FOLLOWING LINES BY MSJ ON 18 JULY 2017 START

            //FormsAuthentication.SignOut();
            //Session.RemoveAll();
            //Session.Abandon();  // Added by Rajas on 4 APRIL 2017

            // COMMENTED FOLLOWING LINES BY MSJ ON 18 JULY 2017 END


            // ADDED BY MSJ ON 18 JULY 2017 START
            FormsAuthentication.SignOut();

            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 03 NOV 2017 START
            //SimpleSessionPersister.Username = string.Empty;
            //SimpleSessionPersister.LastLogin = string.Empty;
            SessionPersister.UserName = string.Empty;
            SessionPersister.LastLogin = string.Empty;
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 03 NOV 2017 END

            Session.RemoveAll();
            Session.Abandon();

            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 03 NOV 2017 START
            //SimpleSessionPersister.UserID = 0;
            SessionPersister.UserID = 0;
            //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 03 NOV 2017 END
            // ADDED BY MSJ ON 18 JULY 2017 END


            return RedirectToAction("LogOn", "Account");
        }

        //
        // GET: /Account/Register

        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus;
                Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, true, null, out createStatus);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, false /* createPersistentCookie */);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePassword

        ///COMMENTED INPUT PARAMETER EMPLOYEEID BY SHRADDHA ON 31 JAN 2017
        ///BECAUSE CHANGE PASSWORD RIGHTS GIVE TO EMPLOYEE ITSELF ON LOGIN PAGE. AND CAN NOT PASS INPUT PARAMETER FROM LOGIN PAGE.
        public ActionResult ChangePassword()
        //public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword


        public ActionResult Error()
        {
            return View();

        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                //Added By Shraddha on 05 Nov 2016 for change Password logic Apply
                Cryptography objcrypt = new Cryptography();

                string OldPassword = objcrypt.EncryptPassword(model.OldPassword.Trim());
                string NewPassword = model.NewPassword;

                ///ADDED OR CONDITION BY SHRADDHA ON 31 JAN 2017 FOR GETTING LOGIN WHILE PASSWORD IS NOT SAVED IN ENCRYPTED FORMAT
                var user = WetosDB.Users.Where(u => u.Password == OldPassword || u.Password == model.OldPassword).FirstOrDefault();


                if (user != null)
                {
                    //DateTime ExpireDate = new DateTime(2017, 12, 30);
                    DateTime ExpireDate = new DateTime(2022, 02, 28);
                    if (DateTime.Now > ExpireDate)
                    {
                        Attention("Login failed. Please Contact STPL");

                    }

                    else
                    {
                        user.Password = objcrypt.EncryptPassword(NewPassword.Trim());
                        //user.Password = NewPassword;
                        WetosDB.SaveChanges();
                        //ViewBag.Message = " Your password has been changed successfully.";
                        //Session["Username"] = user.UserName;
                        // ADDED BY PUSHKAR FOR AuditLog ON 21 DEC 2016
                        AddAuditTrail("Password Changed");


                        ///REDIRECTION CHANGED BY SHRADDHA FROM FINANCIAL YEAR TO HOME INDEX BY 31 JAN 2017 START
                        //return RedirectToAction("FinancialYear");
                        return RedirectToAction("Index", "Home");
                        ///REDIRECTION CHANGED BY SHRADDHA FROM FINANCIAL YEAR TO HOME INDEX BY 31 JAN 2017 END

                    }
                }
                else
                {
                    DateTime ExpireDate = new DateTime(2022, 12, 30);
                    if (DateTime.Now > ExpireDate)
                    {
                        Attention("Login failed. Please Contact STPL");

                    }

                    else
                    {
                        ModelState.AddModelError("", "The Old password is incorrect.");
                    }
                    return View();

                }

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                //bool changePasswordSucceeded;

                //    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                //    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);


                //if (changePasswordSucceeded)
                //{
                //    return RedirectToAction("ChangePasswordSuccess");
                //}
                //else
                //{
                //    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                //}
            }

            else
            {
                return View();
            }
            // If we got this far, something failed, redisplay form
            return View();
        }


        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        public FileContentResult getImg(int id)
        {
            byte[] byteArray = WetosDB.Employees.Where(a => a.EmployeeId == id).Select(a => a.Picture).FirstOrDefault();
            return byteArray != null
                ? new FileContentResult(byteArray, "image/jpeg")
                : null;
        }


        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion

        /// <summary>
        /// TO CHECK FOR CONSISTENCY OF THE DATA DURING LOG ON
        /// Added by Rajas on 27 MARCH 2017
        /// Modified by Shalaka on 17th July 2017
        /// </summary>
        /// <param name="UpdateStatus"></param>
        /// <returns></returns>
        public static bool IsDataConsistent(WetosDBEntities db, ref string UpdateStatus, ref int ErrorNumber,
            ref int CompanyListCount, ref int BranchListCount, ref int DepartmentListCount, ref int DesignationListCount,
            ref int GradeListCount, ref int EmployeeListCount, ref int EmployeeGroupListCount, ref int LeaveMasterListCount,
            ref int LeaveBalanceListCount, ref int LeaveCreditListCount, ref int UserListCount, ref int ShiftCount, ref int FinancialYearCount, ref int UserRoleCount, ref int RuleEngineDataCount) // ADDED RuleEngineDataCount BY SHRADDHA ON 08 FEB 2018
        {
            bool ReturnStatus = false;

            try
            {
                List<WetosDB.Company> CompanyList = db.Companies.ToList();

                List<Branch> BranchList = db.Branches.ToList();

                List<Department> DepartmentList = db.Departments.ToList();

                List<Designation> DesignationList = db.Designations.ToList();

                List<Grade> GradeList = db.Grades.ToList();

                List<Employee> EmployeeList = db.Employees.ToList();

                List<EmployeeGroup> EmployeeGroupList = db.EmployeeGroups.ToList();

                List<LeaveMaster> LeaveMasterList = db.LeaveMasters.ToList();

                List<LeaveBalance> LeaveBalanceList = db.LeaveBalances.ToList();

                List<LeaveCredit> LeaveCreditList = db.LeaveCredits.ToList();

                List<User> UserList = db.Users.ToList();

                //-- Added by shalaka on 17th July 2017--- Start
                List<Shift> Shift = db.Shifts.ToList();

                List<FinancialYear> FinancialYear = db.FinancialYears.ToList();

                List<UserRole> UserRole = db.UserRoles.ToList();

                List<RuleEngine> RuleEngine = db.RuleEngines.ToList();//CODE ADDED BY SHRADDHA ON 08 FEB 2018

                //-- Added by shalaka on 17th July 2017--- Start
                CompanyListCount = CompanyList.Count;
                BranchListCount = BranchList.Count;
                DepartmentListCount = DepartmentList.Count;
                DesignationListCount = DesignationList.Count;
                GradeListCount = GradeList.Count;
                EmployeeListCount = EmployeeList.Count;
                EmployeeGroupListCount = EmployeeGroupList.Count;
                LeaveMasterListCount = LeaveMasterList.Count;
                LeaveBalanceListCount = LeaveBalanceList.Count;
                LeaveCreditListCount = LeaveCreditList.Count;
                UserListCount = UserList.Count;
                ShiftCount = Shift.Count;
                FinancialYearCount = FinancialYear.Count;
                UserRoleCount = UserRole.Count;
                //-- Added by shalaka on 17th July 2017-- End


                RuleEngineDataCount = RuleEngine.Count; // CODE ADDED BY SHRADDHA ON 08 FEB 2018


                if (CompanyList.Count == 0 || BranchList.Count == 0 || DepartmentList.Count == 0
                    || DesignationList.Count == 0 || GradeList.Count == 0 || EmployeeList.Count == 0
                    || EmployeeGroupList.Count == 0)
                {
                    UpdateStatus = "Please import Employee sheet";
                    ErrorNumber = 1;
                    return ReturnStatus;
                }
                else if (LeaveCreditList.Count == 0)
                {
                    UpdateStatus = "Please import Leave credit sheet";
                    ErrorNumber = 2;
                    return ReturnStatus;
                }
                else if (LeaveBalanceList.Count == 0)
                {
                    UpdateStatus = "Please import Leave balance sheet";
                    ErrorNumber = 3;
                    return ReturnStatus;
                }
                else if (CompanyList.Count == 0)
                {
                    UpdateStatus = "Please Update Company details first";
                    ErrorNumber = 4;
                    return ReturnStatus;
                }
                else if (BranchList.Count == 0)
                {
                    UpdateStatus = "Please Update Branch details first";
                    ErrorNumber = 5;
                    return ReturnStatus;
                }
                else if (DepartmentList.Count == 0)
                {
                    UpdateStatus = "Please Update Department details first";
                    ErrorNumber = 6;
                    return ReturnStatus;
                }
                else if (DesignationList.Count == 0)
                {
                    UpdateStatus = "Please Update Designation details first";
                    ErrorNumber = 7;
                    return ReturnStatus;
                }
                else if (GradeList.Count == 0)
                {
                    UpdateStatus = "Please Update Grade details first";
                    ErrorNumber = 8;
                    return ReturnStatus;
                }
                else if (EmployeeList.Count == 0)
                {
                    UpdateStatus = "Please Update Employee details first";
                    ErrorNumber = 9;
                    return ReturnStatus;
                }
                else if (EmployeeGroupList.Count == 0)
                {
                    UpdateStatus = "Please Update Employee group details first";
                    ErrorNumber = 10;
                    return ReturnStatus;
                }
                else if (UserList.Count <= 1)
                {
                    UpdateStatus = "Please add Users";
                    ErrorNumber = 11;
                    return ReturnStatus;
                }
                else if (UserRoleCount <= 1) //CODE ADDED BY SHRADDHA ON 20 NOV 2017 START TO PROVIDE ACCESS RIGHT TO ADMIN FOR ASSIIGN ROLE RIGHT FOR FIRST TIME START
                {
                    UpdateStatus = "Please assign role to atleast one user";
                    //ErrorNumber = 11;
                    return ReturnStatus;
                }               //CODE ADDED BY SHRADDHA ON 20 NOV 2017 START TO PROVIDE ACCESS RIGHT TO ADMIN FOR ASSIIGN ROLE RIGHT FOR FIRST TIME END
                else if (RuleEngineDataCount == 0) //CODE ADDED BY SHRADDHA ON 08 FEB 2018 START
                {
                    UpdateStatus = "Please add Rule Data";
                    ErrorNumber = 12;
                    return ReturnStatus;//CODE ADDED BY SHRADDHA ON 08 FEB 2018 START
                }
                else
                {
                    ReturnStatus = true;
                }

                return ReturnStatus;
            }
            catch (System.Exception ex)
            {
                //AddAuditTrail("Error due to " + ex.Message + ( ex.InnerException == null ? string.Empty : ex.InnerException.Message));

                UpdateStatus = "Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message);

                return ReturnStatus;
            }
        }

        /// <summary>
        /// MAKE SERIES
        /// ADDED BY PUSHKAR ON 3 APRIL 2017 FOR SERIES 
        /// </summary>
        /// <returns></returns>
        public string MakeSeries(int comid, int branchid)
        {
            try
            {
                string Series = string.Empty;

                DateTime CurrentServerDate = DateTime.Now;

                FinancialYear CurrentFY = WetosDB.FinancialYears.Where(a => a.StartDate <= CurrentServerDate && a.EndDate >= CurrentServerDate && a.Company.CompanyId == comid && a.Branch.BranchId == branchid).FirstOrDefault();

                if (CurrentFY == null)
                {
                    int MaxFYId = WetosDB.FinancialYears.Max(a => a.FinancialYearId);

                    FinancialYear LastFY = WetosDB.FinancialYears.Where(a => a.Company.CompanyId == comid && a.Branch.BranchId == branchid).OrderByDescending(a => a.FinancialYearId).FirstOrDefault();

                    int CurrentYear = CurrentServerDate.Year - 2000;
                    int CurrentYearStr = CurrentServerDate.Year;
                    int NextYear = CurrentYear + 1;

                    int PrevYear = CurrentServerDate.Year - 1;

                    if (CurrentServerDate.Month <= 3)
                    {
                        Series = string.Format("{0}-{1}", PrevYear, CurrentYear);
                    }
                    else
                    {
                        Series = string.Format("{0}-{1}", CurrentYearStr, NextYear);
                    }

                    int FinancialNameNew = Convert.ToInt32(LastFY.FinancialName) + 1;

                    // ADD NEW FY
                    CurrentFY = new FinancialYear();
                    CurrentFY.Company = WetosDB.Companies.Where(a => a.CompanyId == comid).FirstOrDefault();
                    CurrentFY.Branch = WetosDB.Branches.Where(a => a.BranchId == branchid).FirstOrDefault();
                    CurrentFY.FinancialName = FinancialNameNew.ToString();
                    CurrentFY.Series = Series;
                    CurrentFY.StartDate = LastFY.StartDate.AddYears(1);
                    CurrentFY.EndDate = LastFY.EndDate.AddYears(1);
                    CurrentFY.PrevFYId = LastFY.FinancialYearId;
                    CurrentFY.NextFYId = MaxFYId + 1;
                    //CurrentFY.FinancialYearId = MaxFYId + 1;
                    CurrentFY.MarkedAsDelete = 0;

                    WetosDB.FinancialYears.Add(CurrentFY);

                    WetosDB.SaveChanges();

                    /// Added by Rajas on 19 MAY 2017
                    /// As per meeting discussion Previous FY should be locked 
                    FinancialYear PrevFY = WetosDB.FinancialYears.Where(a => a.FinancialYearId == (CurrentFY.FinancialYearId - 1)).FirstOrDefault();

                    if (PrevFY != null)
                    {
                        WetosDB.usp_UpdateDailyTrans_Lock("Y", PrevFY.Branch.BranchId, PrevFY.Company.CompanyId, PrevFY.StartDate, PrevFY.EndDate, "0");
                    }

                    //CODE MODIFIED BY SHRADDHA ON 01 NOV 2017 TO COMPARE GLOBAL SETTING BY STRING INSTEAD OF ID BECAUSE IT IS NOT NECESSORY TO HAVE SAME ID FOR ALL INSTALLATIONS AT ALL LOCATIONS AND WE ARE GOING TO MAKE GENERIC CODE FOR ALL LOCATION START
                    //GlobalSetting GFY = WetosDB.GlobalSettings.Where(a => a.SettingId == 2).FirstOrDefault();

                    //GlobalSetting GFY = WetosDB.GlobalSettings.Where(a => a.SettingText == "Current Financial Year").FirstOrDefault();

                    //ABOVE LINE COMMENTED AND BELOW LINE IS ADDED BY SHALAKA ON 13TH DEC 2017 
                    GlobalSetting GFY = WetosDB.GlobalSettings.Where(a => a.SettingText == GlobalSettingsConstant.CurrentFinancialYear).FirstOrDefault();

                    if (GFY == null)
                    {
                        GFY = new GlobalSetting();
                        GFY.SettingText = GlobalSettingsConstant.CurrentFinancialYear;
                        GFY.SettingType = 1;
                        GFY.SettingValue = CurrentFY.FinancialName.ToString();
                        WetosDB.GlobalSettings.Add(GFY);
                        WetosDB.SaveChanges();
                    }
                    else
                    {
                        GFY.SettingValue = CurrentFY.FinancialName.ToString();
                        WetosDB.SaveChanges();
                    }
                    //CODE MODIFIED BY SHRADDHA ON 01 NOV 2017 TO COMPARE GLOBAL SETTING BY STRING INSTEAD OF ID BECAUSE IT IS NOT NECESSORY TO HAVE SAME ID FOR ALL INSTALLATIONS AT ALL LOCATIONS AND WE ARE GOING TO MAKE GENERIC CODE FOR ALL LOCATION END

                }
                else
                {
                    Series = CurrentFY.Series;
                }

                //On Error GoTo errhandler

                //    Dim myyear As Integer
                //    myyear = Val(Right(str(Year(Date)), 2))

                //    If Month(Date) = 1 Or Month(Date) = 2 Or Month(Date) = 3 Then
                //        TempVars.Add "myseries", Trim(str((myyear - 1))) + "-" + Trim(str(myyear))
                //    Else
                //        TempVars.Add "myseries", Trim(str(myyear)) + "-" + Trim(str((myyear + 1)))
                //    End If

                //Exit Sub
                //errhandler:
                //    err_number = Err.Number
                //    err_desc = Err.Description
                //    MsgBox err_desc
                //    Call globalerrorhandler
                //End Sub
                return Series;
            }
            catch (System.Exception E)
            {
                string Series = "";
                return Series;
            }
        }

        //ADDED BY PUSHKAR ON 22 FEB 2019 FOR NEW JOINEE LEAVE CREDIT CL ONLY -----START
        public static bool NewJoineeLeaveBalance(int FinYearId, WetosDBEntities db)
        {
            try
            {

                GlobalSetting GlobalSettingObj = db.GlobalSettings.Where(a => a.SettingText == "NewJoineeMonth").FirstOrDefault();
                if (GlobalSettingObj != null)
                {
                    GlobalSettingObj.SettingValue = DateTime.Now.Month.ToString();
                    db.SaveChanges();
                }

                GlobalSetting GlobalSettingObjUpdate = db.GlobalSettings.Where(a => a.SettingText == "NewJoineeUpdatedMonth").FirstOrDefault();
                int GSCurrentVal = Convert.ToInt32(GlobalSettingObj.SettingValue);
                int GSUpdMonth = Convert.ToInt32(GlobalSettingObjUpdate == null ? "0" : GlobalSettingObjUpdate.SettingValue);
                if (GlobalSettingObjUpdate != null)
                {
                    if (GSUpdMonth != GSCurrentVal)
                    {
                        List<EmployeeGroup> EmployeeGroupListProb = db.EmployeeGroups.Where(a => a.EmployeeGroupName.ToUpper().Contains("PROBATION")).ToList();
                        foreach (EmployeeGroup EmployeeGroupInDB in EmployeeGroupListProb)
                        {
                            List<EmployeeGroupDetail> EmployeeGroupDetailList = db.EmployeeGroupDetails.Where(a => a.EmployeeGroup.EmployeeGroupId == EmployeeGroupInDB.EmployeeGroupId).ToList();
                            if (EmployeeGroupDetailList.Count > 0)
                            {
                                foreach (var EmployeeGroupDetailObj in EmployeeGroupDetailList)
                                {
                                    Employee EmployeeObj = db.Employees.Where(a => a.EmployeeId == EmployeeGroupDetailObj.Employee.EmployeeId && a.ActiveFlag == true).FirstOrDefault();
                                    int IdEmployee = EmployeeObj == null ? 0 : EmployeeObj.EmployeeId;
                                    LeaveBalance LeaveBalanceObj = db.LeaveBalances.Where(a => a.LeaveType == "CL" && a.EmployeeId == IdEmployee).FirstOrDefault();
                                    if (LeaveBalanceObj != null)
                                    {
                                        LeaveBalanceObj.CurrentBalance = (LeaveBalanceObj.CurrentBalance == null ? 0 : LeaveBalanceObj.CurrentBalance.Value) + 1;
                                        if (LeaveBalanceObj.CurrentBalance > 6)
                                        {
                                            //LeaveBalanceObj.CurrentBalance = 6; //TEMP COMMENTED BY PUSHKAR ON 06 MARCH 2019
                                        }

                                        db.SaveChanges();
                                    }
                                    FinancialYear PayRollFinYearData = db.FinancialYears.Where(a => a.StartDate <= DateTime.Now && a.EndDate >= DateTime.Now
                                        && a.Branch.BranchId == EmployeeObj.BranchId).FirstOrDefault();

                                    LeaveCredit LeaveCreditObj = db.LeaveCredits.Where(a => a.EmployeeId == IdEmployee && a.LeaveType == "CL"
                                        && a.FinancialYearId == PayRollFinYearData.FinancialYearId).FirstOrDefault();

                                    LeaveCreditObj.OpeningBalance = (LeaveCreditObj.OpeningBalance == null ? 0 : LeaveCreditObj.OpeningBalance.Value) + 1;
                                    db.SaveChanges();
                                }
                            }
                            GlobalSettingObjUpdate.SettingValue = DateTime.Now.Month.ToString();
                            db.SaveChanges();

                        }
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
                //throw;
            }
        }
        //ADDED BY PUSHKAR ON 22 FEB 2019 FOR NEW JOINEE LEAVE CREDIT CL ONLY -----END

    }
}
