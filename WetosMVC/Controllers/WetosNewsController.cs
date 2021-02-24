using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WetosDB;
using WetosMVCMainApp.Models;

namespace WetosMVC.Controllers
{
    [SessionExpire] 
    [Authorize]
    public class WetosNewsController : BaseController
    {

        /// <summary>
        /// Added by Rajas on 10 APRIL 2017
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            try
            {

                List<VwNewsList> NewsList = WetosDB.VwNewsLists.ToList();

                //ADDED BY RAJAS ON 27 DEC 2016
                AddAuditTrail("Success - Visited news master"); // Updated by Rajas on 16 JAN 2017

                return View(NewsList);
            }

            catch (System.Exception ex)
            {
                // Added by Rajas on 16 JAN 2017
                AddAuditTrail("Exception - " + ex.Message);

                return View("Error");
            }
        }


        /// <summary>
        /// DETAILS
        /// Added by Rajas on 10 APRIL 2017
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int id)
        {
            try
            {
                VwNewsList NewsDetails = WetosDB.VwNewsLists.Single(a => a.NewsId == id);

                //ADDED BY RAJAS ON 27 DEC 2016
                AddAuditTrail("Checked details for " + NewsDetails.Type + " for " + NewsDetails.NewsDate);

                return View(NewsDetails);
            }

            catch (System.Exception ex)
            {
                // Added by Rajas on 16 JAN 2017
                AddAuditTrail("Exception - " + ex.Message);

                return View();
            }
        }

        /// <summary>
        /// CREATE GET
        /// Added by Rajas on 10 APRIL 2017
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            try
            {
                PopulateDropDown();

                return View();
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error on news due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Inconsistent data. Please try again!");

                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// POST
        /// Create News
        /// Added by Rajas on 10 APRIL 2017
        /// </summary>
        /// <param name="id"></param>
        /// <param name="NewsModelObj"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(NewsModel NewsModelObj)
        {
            try
            {
                // Added by Rajas on 27 MARCH 2017
                string UpdateStatus = string.Empty;

                if (ModelState.IsValid)
                {
                    // Added by Rajas on 28 JULY 2017 START
                    NewsModelObj.EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                    NewsModelObj.BranchId = WetosDB.Employees.Where(a => a.EmployeeId == NewsModelObj.EmployeeId).Select(a => a.BranchId).FirstOrDefault();
                    NewsModelObj.EmployeeGroupId = WetosDB.EmployeeGroupDetails.Where(a => a.Employee.EmployeeId == NewsModelObj.EmployeeId)
                        .Select(a => a.EmployeeGroup.EmployeeGroupId).FirstOrDefault();
                    // Added by Rajas on 28 JULY 2017 END

                    if (UpdateNewsData(NewsModelObj, ref UpdateStatus) == true)
                    {
                        AddAuditTrail("News : " + NewsModelObj.Type + " : " + NewsModelObj.Info + " is added successfully");

                        Success("News : " + NewsModelObj.Type + " : " + NewsModelObj.Info + " is added successfully");
                    }
                    else
                    {
                        AddAuditTrail(UpdateStatus);

                        Error(UpdateStatus);
                    }

                    return RedirectToAction("Index");
                }

                else
                {
                    // POPULATE VIEWBAG FRO DROPDOWN
                    PopulateDropDownEdit();

                    AddAuditTrail("Error - Add new news failed");

                    Error("Error - Add new news failed");

                    return View();

                }

            }
            catch (System.Exception ex)
            {
                // POPULATE VIEWBAG FRO DROPDOWN
                PopulateDropDownEdit();

                AddAuditTrail("Error - Add new news failed due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Error - Add new news failed");

                return View();
            }
        }


        /// <summary>
        /// GET
        /// News edit
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            try
            {
                NewsModel NewsModelObj = new NewsModel();

                News NewsEdit = WetosDB.News.Single(b => b.NewsId == id);

                NewsModelObj.Type = NewsEdit.Type;

                NewsModelObj.NewsId = NewsEdit.NewsId; // Added by Rajas on 30 AUGUST 2017

                NewsModelObj.Info = NewsEdit.Info;

                NewsModelObj.NewsDate = NewsEdit.NewsDate;

                NewsModelObj.CompanyId = NewsEdit.Company.CompanyId;

                NewsModelObj.BranchId = NewsEdit.Branch.BranchId;

                NewsModelObj.EmployeeId = NewsEdit.Employee.EmployeeId;

                NewsModelObj.EmployeeGroupId = NewsEdit.EmployeeGroupId;

                NewsModelObj.NewsFor = NewsEdit.NewsFor;

                NewsModelObj.Active = NewsEdit.Active;

                NewsModelObj.NewsEndDate = NewsEdit.NewsEndDate;

                // POPULATE VIEWBAG FRO DROPDOWN
                PopulateDropDownEdit();

                return View(NewsModelObj);
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message);

                Error("Inconsistent data. Please try again!");

                return View();
            }
        }


        /// <summary>
        /// POST
        /// Edit News
        /// Added by Rajas on 10 APRIL 2017
        /// </summary>
        /// <param name="id"></param>
        /// <param name="NewsModelObj"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(int id, NewsModel NewsModelObj)
        {
            try
            {
                // Added by Rajas on 27 MARCH 2017
                string UpdateStatus = string.Empty;

                if (ModelState.IsValid)
                {
                    if (UpdateNewsData(NewsModelObj, ref UpdateStatus) == true)
                    {
                        AddAuditTrail("News : " + NewsModelObj.Type + " : " + NewsModelObj.Info + " is updated successfully");

                        Success("News : " + NewsModelObj.Type + " : " + NewsModelObj.Info + " is updated successfully");
                    }
                    else
                    {
                        AddAuditTrail(UpdateStatus);

                        Error(UpdateStatus);
                    }

                    return RedirectToAction("Index");
                }

                else
                {
                    // POPULATE VIEWBAG FRO DROPDOWN
                    PopulateDropDownEdit();

                    AddAuditTrail("Error - News update failed");

                    Error("Error - News update failed");

                    return View();

                }

            }
            catch (System.Exception ex)
            {
                // POPULATE VIEWBAG FRO DROPDOWN
                PopulateDropDownEdit();

                AddAuditTrail("Error - News update failed due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) );

                Error("Error - News update failed");

                return View();
            }
        }

        //
        // GET: /WetosNews/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /WetosNews/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        /// <summary>
        ///  Populate All Dropdown ViewBag
        ///  Added by Rajas on 10 APRIL 2017
        /// </summary>
        public bool PopulateDropDown()
        {
            bool ReturnStatus = false;

            try
            {
                //CODE FOR DROPDOWN for COMPANY

                #region COMPANY LIST CODE ADDED BY SHRADDHA ON 30 MAR 2018
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //var CompanyName = WetosDB.Companies.Select(a => new { CompanyId = a.CompanyId, CompanyName = a.CompanyName }).ToList();
                var CompanyName = WetosDB.SP_GetCompanyList(EmployeeId).Select(m => new { Companyid = m.CompanyId, CompanyName = m.CompanyName }).ToList();
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
                #endregion

                ViewBag.CompanyList = new SelectList(CompanyName, "CompanyId", "CompanyName").ToList();

                //CODE FOR DROPDOWN for BRANCH
                var Branch = new List<Branch>();
                ViewBag.BranchList = new SelectList(Branch, "BranchId", "BranchName").ToList();

                //CODE FOR DROPDOWN for Employee group
                var EmployeeGroup = new List<EmployeeGroup>();
                ViewBag.EmployeeGroupList = new SelectList(EmployeeGroup, "EmployeeGroupId", "EmployeeGroupName").ToList();

                //CODE FOR DROPDOWN for Employee
                var Employee = new List<Employee>();
                ViewBag.EmployeeList = new SelectList(Employee, "EmployeeId", "EmployeeName").ToList();

                // News type Dropdown, added by Rajas on 10 APRIL 2017
                List<SelectListItem> NewsTypeList = new List<SelectListItem>();
                NewsTypeList.Add(new SelectListItem { Text = "Announcement", Value = "Announcement" });
                NewsTypeList.Add(new SelectListItem { Text = "Meeting", Value = "Meeting" });
                ViewBag.NewsTypeListVB = new SelectList(NewsTypeList, "Value", "Text").ToList();

                return ReturnStatus = true;

            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message);

                Error("Inconsistent data. Please try again!");

                return ReturnStatus;
            }
        }

        /// <summary>
        ///  Populate All Dropdown ViewBag
        ///  Added by Rajas on 10 APRIL 2017
        /// </summary>
        public bool PopulateDropDownEdit()
        {
            bool ReturnStatus = false;

            try
            {
                //CODE FOR DROPDOWN for COMPANY

                #region COMPANY LIST CODE ADDED BY SHRADDHA ON 30 MAR 2018
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
                int EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //var CompanyName = WetosDB.Companies.Select(a => new { CompanyId = a.CompanyId, CompanyName = a.CompanyName }).ToList();
                var CompanyName = WetosDB.SP_GetCompanyList(EmployeeId).Select(m => new { Companyid = m.CompanyId, CompanyName = m.CompanyName }).ToList();
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
                #endregion

                ViewBag.CompanyList = new SelectList(CompanyName, "CompanyId", "CompanyName").ToList();

                //CODE FOR DROPDOWN for BRANCH
                #region CODE FOR GET BRANCH LIST ADDED BY SHRADDHA ON 30 MAR 2018
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN START
                //var Branch = WetosDB.Branches.Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).OrderByDescending(a => a.BranchId).ToList();
                var Branch = WetosDB.SP_GetBranchList(EmployeeId).Select(a => new { BranchId = a.BranchId, BranchName = a.BranchName }).ToList(); //.Select(m => new { BranchId = m.BranchId, BranchName = m.BranchName}).ToList();
                //COMMENTED EARLIER CODE AND ADDED NEW CODE BY SHRADDHA ON 30 MAR 2018 FOR BRANCH WISE ADMIN END
                #endregion
                
                ViewBag.BranchList = new SelectList(Branch, "BranchId", "BranchName").ToList();

                //CODE FOR DROPDOWN for EmployeeGroup
                var EmployeeGroup = WetosDB.EmployeeGroups.Select(a => new { EmployeeGroupId = a.EmployeeGroupId, EmployeeGroupName = a.EmployeeGroupName }).ToList();
                ViewBag.EmployeeGroupList = new SelectList(EmployeeGroup, "EmployeeGroupId", "EmployeeGroupName").ToList();

                //CODE FOR DROPDOWN for BRANCH
                
                #region CODE ADDED BY SHRADDHA ON 31 MAR 2018 FOR BRANCH WISE ADMIN GET EMPLOYEE LIST
                EmployeeId = Convert.ToInt32(Session["EmployeeNo"]);
                //var Employee = WetosDB.Employees.Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " | " + a.FirstName + a.LastName }).ToList();
                var Employee = WetosDB.SP_VwActiveEmployee(EmployeeId).Select(a => new { EmployeeId = a.EmployeeId, EmployeeName = a.EmployeeCode + " | " + a.FirstName + a.LastName }).ToList();
                #endregion

                ViewBag.EmployeeList = new SelectList(Employee, "EmployeeId", "EmployeeName").ToList();

                // News type Dropdown, added by Rajas on 10 APRIL 2017
                List<SelectListItem> NewsTypeList = new List<SelectListItem>();
                NewsTypeList.Add(new SelectListItem { Text = "Announcement", Value = "Announcement" });
                NewsTypeList.Add(new SelectListItem { Text = "Meeting", Value = "Meeting" });
                ViewBag.NewsTypeListVB = new SelectList(NewsTypeList, "Value", "Text").ToList();

                return ReturnStatus = true;
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message);

                Error("Inconsistent data. Please try again!");

                return ReturnStatus;
            }
        }

        /// <summary>
        /// Common function to validate the data for edit or create
        /// Added by Rajas on 10 APRIl 2017
        /// </summary>
        private bool UpdateNewsData(NewsModel NewsModelObj, ref string UpdateStatus)
        {
            bool ReturnStatus = false;

            try
            {
                // Updated by Rajas on 30 AUGUST 2017
                WetosDB.News NewsTblObj = WetosDB.News.Where(a => a.NewsId == NewsModelObj.NewsId).FirstOrDefault();   //  || a.Type == NewsModelObj.Type

                bool IsNew = false;
                if (NewsTblObj == null)
                {
                    NewsTblObj = new WetosDB.News();

                    IsNew = true;
                }

                // New Leave table object

                NewsTblObj.Type = NewsModelObj.Type;

                NewsTblObj.Info = NewsModelObj.Info;

                NewsTblObj.NewsDate = NewsModelObj.NewsDate;

                NewsTblObj.Company.CompanyId = NewsModelObj.CompanyId.Value;

                NewsTblObj.Branch.BranchId = NewsModelObj.BranchId.Value;

                NewsTblObj.Employee.EmployeeId = NewsModelObj.EmployeeId.Value;

                NewsTblObj.EmployeeGroupId = NewsModelObj.EmployeeGroupId;

                NewsTblObj.NewsFor = NewsModelObj.NewsFor;

                NewsTblObj.Active = NewsModelObj.Active;

                NewsTblObj.NewsEndDate = NewsModelObj.NewsEndDate;

                // Add new table object 
                if (IsNew)
                {
                    WetosDB.News.AddObject(NewsTblObj);
                }

                WetosDB.SaveChanges();

                ReturnStatus = true;

                return ReturnStatus;
            }
            catch (System.Exception ex)
            {
                AddAuditTrail("Error due to " + ex.Message);

                UpdateStatus = "Error due to " + ex.Message + (ex.InnerException == null ? string.Empty : ex.InnerException.Message);

                return ReturnStatus;
            }
        }
    }
}
