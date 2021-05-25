using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WetosDB;

namespace WetosMVC.Controllers
{
    [SessionExpire] 
    [Authorize]
    public class WetosUpdatesController : BaseController
    {
        
        //
        // GET: /WetosUpdates/

        public ActionResult Index()
        {
            return View();
        }
       

        /// <summary>
        /// CODE FOR UPLOAD COMPANY POLICIES
        /// ADDED BY SHRADDHA ON 31 JAN 2017
        /// <returns></returns>
        public ActionResult UploadCompanyPolicies() {

            return View();
        }


        /// <summary>
        /// CODE FOR SAVING UPLOADED COMPANY POLICIES
        /// ADDED BY SHRADDHA ON 31 JAN 2017
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadCompanyPolicies(UploadCompanyDocument UploadCompanyDocumentObj, HttpPostedFileBase UploadFile)
        {

            if (UploadFile != null && UploadFile.ContentLength > 0)
            {
                try
                {

                    string DocumentFileName =(DateTime.Now.ToString("yyyy_MM_dd_HHmmss"))+"_"+ UploadFile.FileName;
                    string Attachment = (DocumentFileName);

                    string path = Path.Combine(Server.MapPath("~/User_Data/Company_Documents"), Attachment);


                    string fileExtension = Path.GetExtension(UploadFile.FileName);



                    UploadFile.SaveAs(path);


                    UploadCompanyDocument UploadCompanyDocumentTblObj = WetosDB.UploadCompanyDocuments.FirstOrDefault();

                    if (UploadCompanyDocumentTblObj == null)
                    {
                        UploadCompanyDocument UploadCompanyDocumentNewObj = new UploadCompanyDocument();
                        UploadCompanyDocumentNewObj.DocContentType = UploadCompanyDocumentObj.DocContentType;
                        UploadCompanyDocumentNewObj.DocDescription = UploadCompanyDocumentObj.DocDescription;
                        UploadCompanyDocumentNewObj.DocStatus = UploadCompanyDocumentObj.DocStatus;
                        UploadCompanyDocumentNewObj.DocType = fileExtension;
                        UploadCompanyDocumentNewObj.ExtraDetails = UploadCompanyDocumentObj.ExtraDetails;
                        UploadCompanyDocumentNewObj.DocFolder = "Company_Documents";
                        UploadCompanyDocumentNewObj.DocPath = path;
                        UploadCompanyDocumentNewObj.FileName = Attachment;

                        WetosDB.UploadCompanyDocuments.Add(UploadCompanyDocumentNewObj);
                    }
                    else {
                        UploadCompanyDocumentTblObj.DocContentType = UploadCompanyDocumentObj.DocContentType;
                        UploadCompanyDocumentTblObj.DocDescription = UploadCompanyDocumentObj.DocDescription;
                        UploadCompanyDocumentTblObj.DocStatus = UploadCompanyDocumentObj.DocStatus;
                        UploadCompanyDocumentTblObj.DocType = fileExtension;
                        UploadCompanyDocumentTblObj.ExtraDetails = UploadCompanyDocumentObj.ExtraDetails;
                        UploadCompanyDocumentTblObj.DocFolder = "Company_Documents";
                        UploadCompanyDocumentTblObj.DocPath = path;
                        UploadCompanyDocumentTblObj.FileName = Attachment;
                    
                    }
                    WetosDB.SaveChanges();


                   
                    ViewBag.Message = "File Processed Successfully";

                }
                catch (System.Exception ex)
                {
                    ViewBag.Message = "Error:" + ex.Message.ToString();
                   
                    return View();

                }
                ViewBag.Message = "Document Uploaded Successful";
                Session["SuccessMessage"] = ViewBag.Message;
               
                return View();
            }
            else
            {
                ViewBag.Message = "You have not specified a file";
                
                return View();
            }


           
        }


        /// <summary>
        /// CODE FOR DOWNLOAD AND VIEW UPLOADED COMPANY POLICIES
        /// ADDED BY SHRADDHA ON 31 JAN 2017
        /// <returns></returns>
        public ActionResult ViewCompanyPolicies() {

            UploadCompanyDocument UploadCompanyDocumentObj = WetosDB.UploadCompanyDocuments.FirstOrDefault();
            if (UploadCompanyDocumentObj != null)
            {
                ViewBag.IsFileAvailableOrNotFlag = true;
                return View(UploadCompanyDocumentObj);
            }
            else {
                ViewBag.IsFileAvailableOrNotFlag = false;
                return View();
            }
        }

    }
}
