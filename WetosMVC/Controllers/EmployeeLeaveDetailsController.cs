using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WetosMVC;

namespace WetosMVCMainApp.Controllers
{
    public class EmpLeaveDetailModel
    {
        public List<EmployeeLeaveDetail> EmpLeaveDetail1 { get; set; }
        public List<EmployeeLeaveDetail> EmpLeaveDetail { get; set; }
        public List<EmployeeLeaveBalance> EmpLeaveBal { get; set; }

        public EmpLeaveDetailModel()
        {
            if (EmpLeaveDetail == null)
            {
                EmpLeaveDetail = new List<EmployeeLeaveDetail>();
            }

            if (EmpLeaveBal == null)
            {
                EmpLeaveBal = new List<EmployeeLeaveBalance>();
            }

            if (EmpLeaveDetail1 == null)
            {
                EmpLeaveDetail1 = new List<EmployeeLeaveDetail>();
            }

        }
    }

    public class EmployeeLeaveDetailsController : Controller
    {
        //
        // GET: /EmployeeLeaveDetails/

        FlagshipInternationalEntities db = new FlagshipInternationalEntities();





        /// <summary>
        /// To List Details of Employee Leave Coded by Akshay on 26th May 2016.
        /// </summary>
        /// <returns></returns>       
        public ActionResult Index(EmployeeLeaveDetail EmpLeaveDetailObj, FormCollection collection)
        {
            EmpLeaveDetailModel ABCD = new EmpLeaveDetailModel();
            ABCD.EmpLeaveDetail1 = db.EmployeeLeaveDetails.Where(a => a.Status == null).OrderByDescending(a => a.LeaveID).ToList();
            ABCD.EmpLeaveDetail = db.EmployeeLeaveDetails.Where(a => a.Status == "Approved" || a.Status == "Rejected").ToList();
            //            ABCD.EmpLeaveDetail = db.EmployeeLeaveDetails.OrderByDescending(a => a.EmployeeID).ToList();
            ABCD.EmpLeaveBal = db.EmployeeLeaveBalances.OrderBy(a => a.EmployeeID).ToList();
            return View(ABCD);

        }
        //EmployeeLeaveBalance EmpBal = new EmployeeLeaveBalance();
        //EmpBal.EmployeeID = EmpLeaveDetailObj.EmployeeID;
        //EmpBal.LeaveType = EmpLeaveDetailObj.LeaveType;
        //EmpBal.PreviousBalance = EmpLeaveDetailObj.ActualDays;
        //EmpBal.CurrentBalance = EmpLeaveDetailObj.ActualDays - EmpLeaveDetailObj.AppliedDays;
        //EmpBal.LeaveTaken = EmpLeaveDetailObj.AppliedDays;
        //db.EmployeeLeaveBalances.AddObject(EmpBal);
        //db.SaveChanges();
        //return RedirectToAction("Index");




        public ActionResult Create()
        {

            return View();
        }

        /// <summary>
        /// To save Leave Details of Employee Coded by Akshay on 26th May 2016. 
        /// </summary>
        /// <param name="EmpLeaveDetailObj"></param>
        /// <param name="collection"></param>
        /// <returns></returns>

        [HttpPost]
        public ActionResult Create(EmployeeLeaveDetail EmpLeaveDetailObj, FormCollection collection)
        {
           
            try
            {
                EmployeeLeaveDetail EmpLeaveDet = new EmployeeLeaveDetail();
                EmpLeaveDet.EmployeeID = Session["EmployeeNo"].ToString();//EmpLeaveDetailObj.EmployeeID;
                EmpLeaveDet.LeaveType = EmpLeaveDetailObj.LeaveType;
                EmpLeaveDet.FromDate = Convert.ToDateTime(collection["date"]); //EmpLeaveDetailObj.FromDate;//
                EmpLeaveDet.ToDate = Convert.ToDateTime(collection["date1"]); //EmpLeaveDetailObj.ToDate;//
                EmpLeaveDet.AppliedDays = EmpLeaveDetailObj.AppliedDays;
                EmpLeaveDet.ActualDays = EmpLeaveDetailObj.ActualDays;
                EmpLeaveDet.Purpose = EmpLeaveDetailObj.Purpose;
                EmpLeaveDet.AvailableDays = EmpLeaveDet.ActualDays - EmpLeaveDet.AppliedDays;
                db.EmployeeLeaveDetails.AddObject(EmpLeaveDet);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View();
            }

        }

        /// <summary>
        /// Details of Leave details coded by Akshay on 27th May 2016.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public ActionResult Details(int id)
        {
            EmployeeLeaveDetail EmpLeaveDet = new EmployeeLeaveDetail();
            EmpLeaveDet = db.EmployeeLeaveDetails.Where(a => a.LeaveID == id).FirstOrDefault();
            return View(EmpLeaveDet);
        }

        /// <summary>
        /// Edit Leave Details Coded by Akshay on 27th May 2016.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public ActionResult Edit(int id)
        {
            EmployeeLeaveDetail EmpLevDet = new EmployeeLeaveDetail();
            EmpLevDet = db.EmployeeLeaveDetails.Where(a => a.LeaveID == id).FirstOrDefault();
            return View(EmpLevDet);
        }

        /// <summary>
        /// Save Leave Details After Edit Coded by Akshay on 27th May 2016. 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="collection"></param>
        /// <returns></returns>


        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                EmployeeLeaveDetail EmpLevDet = db.EmployeeLeaveDetails.Single(e => e.LeaveID == id);
                EmpLevDet.EmployeeID = collection["EmployeeID"];
                EmpLevDet.LeaveType = collection["LeaveType"];
                EmpLevDet.FromDate = Convert.ToDateTime(collection["date"]);
                EmpLevDet.ToDate = Convert.ToDateTime(collection["date1"]);
                EmpLevDet.AppliedDays = Convert.ToInt32(collection["AppliedDays"]);
                EmpLevDet.ActualDays = Convert.ToInt32(collection["ActualDays"]);
                EmpLevDet.Purpose = collection["Purpose"];

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        /// <summary>
        /// Confirmation to Delete Record From database Coded by Akshay On 27th May 2016.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public ActionResult Delete(int id)
        {
            EmployeeLeaveDetail EmpDet = db.EmployeeLeaveDetails.Single(a => a.LeaveID == id);
            return View(EmpDet);
        }

        /// <summary>
        /// To Delete Record From database Coded by Akshay On 27th May 2016.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="collection"></param>
        /// <returns></returns>

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                EmployeeLeaveDetail EmpLev = db.EmployeeLeaveDetails.Single(a => a.LeaveID == id);
                db.EmployeeLeaveDetails.DeleteObject(EmpLev);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View();

            }

        }


        /// <summary>
        /// to get the no.of leaves on selection of leavetype
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Ajaxnoofleaves(int id)
        {
            string employeeid = Session["EmployeeNo"].ToString();

            var noofleavesobj = db.LeaveMasters.Where(a => a.LeaveID == id).Select(a => new { a.NoOfLeaves }).FirstOrDefault();///modified by Akshay on 29 JULY 2016
            string leavetype = id.ToString();
            //var status = db.EmployeeLeaveDetails.Where(b => b.EmployeeID == employeeid).Where(b => b.LeaveType == leavetype).Where(b=>b.Status!=null).Select(b => new { b.Status }).OrderByDescending();
            //int AppliedDays = 0;
            //if (status.Status == "Approved")
            //{
            ////////    var noofapplieddays = db.EmployeeLeaveDetails.Where(b => b.EmployeeID == employeeid).Where(b => b.LeaveType == leavetype).Select(b => new { b.AppliedDays }).FirstOrDefault();///added by shraddha on 26 AUG 2016
            ////////    AppliedDays = Convert.ToInt32(noofapplieddays.AppliedDays);
            ////////}
            int AppliedDays = 0;
            List<EmployeeLeaveDetail> emp = db.EmployeeLeaveDetails.Where(b=>b.EmployeeID==employeeid).ToList();
            if (emp.Count() != 0)
            {
                var noofapplieddays = db.EmployeeLeaveDetails.Where(b => b.EmployeeID == employeeid).Where(b => b.LeaveType == leavetype).Where(b => b.Status == "Approved").Select(b => new { b.AppliedDays }).FirstOrDefault();///added by shraddha on 26 AUG 2016
                AppliedDays = Convert.ToInt32(noofapplieddays.AppliedDays);
            }
            else {

                 AppliedDays = 0;
            }
            
            var actualdays = Convert.ToInt32(noofleavesobj.NoOfLeaves);
            int availabledays = actualdays - AppliedDays;
            actualdays = availabledays;
            return Json(new
            {
                param1 = availabledays

            },
            JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public double GetBalalnceLeaves(int id)
        {
            string employeeid = Session["EmployeeNo"].ToString();

            var noofleavesobj = db.LeaveMasters.Where(a => a.LeaveID == id).Select(a => new { a.NoOfLeaves }).FirstOrDefault();///modified by Akshay on 29 JULY 2016
            string leavetype = id.ToString();

            int AppliedDays = 0;

            List<EmployeeLeaveDetail> emp = db.EmployeeLeaveDetails.Where(b => b.EmployeeID == employeeid).ToList();
            if (emp.Count() != 0)
            {
                var noofapplieddays = db.EmployeeLeaveDetails.Where(b => b.EmployeeID == employeeid).Where(b => b.LeaveType == leavetype).Where(b => b.Status == "Approved").Select(b => new { b.AppliedDays }).FirstOrDefault();///added by shraddha on 26 AUG 2016
                AppliedDays = Convert.ToInt32(noofapplieddays.AppliedDays);
            }
            else
            {

                AppliedDays = 0;
            }

            var actualdays = Convert.ToInt32(noofleavesobj.NoOfLeaves);
            int availabledays = actualdays - AppliedDays;
            actualdays = availabledays;
            
            return availabledays;

        }


    }
}
