using System;
using System.Collections.Generic;
using System.Linq;
using WetosDB;
using System.Web;

namespace WetosMVCMainApp.Utilities
{

    public sealed class CryptorEngineUtil
    {
        public WetosDBEntities WetosDB = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileDetails"></param>
        /// <returns></returns>
        public bool ReadIni(string fileDetails)
        {

            //string Entrypt1Str = WETOS.Util.CryptorEngine.Encrypt("Ajanta Pharma Paithan|1|1|15-04-2021|1000|05DC3428-A876-45D1-BFB2-23A3A0EA268F|2370EBB3-FA82-40CB-ABCA-42DDF53E1A51", true);
            //string Entrypt2Str = WETOS.Util.CryptorEngine.Encrypt(Entrypt1Str, true);
            //string Decrypt1Str = WETOS.Util.CryptorEngine.Decrypt(Entrypt2Str, true);
            //string Decrypt2Str = WETOS.Util.CryptorEngine.Decrypt(Decrypt1Str, true);

            bool returnValue = false;

            string[] splitedValues;

            try
            {
                string DecryptedStr = WETOS.Util.CryptorEngine.Decrypt(System.IO.File.ReadAllText(fileDetails), true);
                string ReDecryptedStr = WETOS.Util.CryptorEngine.Decrypt(DecryptedStr, true);

                splitedValues = ReDecryptedStr.Split('|');

                WETOSSession.LicenseCompanyName = splitedValues[0];
                WETOSSession.CountCompanyLicense = splitedValues[1];
                WETOSSession.CountBranchLicense = splitedValues[2];
                WETOSSession.DateOfLicenseExpiry = splitedValues[3];

                //WETOSSession.DateOfLicenseExpiry = splitedValues[4];
                //WETOSSession.DateOfLicenseExpiry = splitedValues[5];
                //WETOSSession.DateOfLicenseExpiry = splitedValues[6];

                returnValue = true;
            }
            catch (System.Exception ex)
            {
                returnValue = false;

            }
            return returnValue;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="fileDetails"></param>
        ///// <returns></returns>
        //public bool WriteIni(string FilePath, string LicenseKey)
        //{
        //    bool returnValue = false;

        //    string[] splitedValues;

        //    try
        //    {
        //        string DecryptedStr = WETOS.Util.CryptorEngine.Decrypt(System.IO.File.ReadAllText(fileDetails), true);
        //        splitedValues = DecryptedStr.Split('|');

        //        WETOSSession.LicenseCompanyName = splitedValues[0];
        //        WETOSSession.CountCompanyLicense = splitedValues[1];
        //        WETOSSession.CountBranchLicense = splitedValues[2];
        //        WETOSSession.DateOfLicenseExpiry = splitedValues[3];
        //        returnValue = true;
        //    }
        //    catch (System.Exception ex)
        //    {
        //        returnValue = false;

        //    }
        //    return returnValue;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public bool ValidateWetos(ref string errorMsg) //System.Web.UI.WebControls.Label errorMsg)
        {
            bool returnValue = true;
            int countCompanyValue = 0;
            int countBranchValue = 0;

            Int32.TryParse(WETOSSession.CountCompanyLicense, out countCompanyValue);
            Int32.TryParse(WETOSSession.CountBranchLicense, out countBranchValue);

            if (WetosDB == null)
            {
                WetosDB = new WetosDBEntities();
                //WetosDB.CommandTimeout = 2000;
            }

            List<Company> m_ListCompany = WetosDB.Companies.ToList(); // BusinessFactory.Instance.BLCompany.GetAllCompany(ClientConstants.WetosAdminRights, ClientConstants.WetosAdminRights, Convert.ToString(ClientConstants.WetosAdminReportingId));
            List<Branch> m_ListBranch = WetosDB.Branches.ToList(); //BusinessFactory.Instance.BLBranch.GetAllBranch(ClientConstants.WetosAdminRights, ClientConstants.WetosAdminRights, ClientConstants.WetosAdminRights, Convert.ToString(ClientConstants.WetosAdminReportingId));
            if (m_ListCompany.Count > countCompanyValue)
            {
                errorMsg = Messages.ErrorDisplay(707);
                return false;
            }
            if (m_ListBranch.Count > countBranchValue)
            {
                errorMsg = Messages.ErrorDisplay(708);
                return false;
            }
            DateTimeConvertor objDateTimeConvertor = new DateTimeConvertor();

            if (objDateTimeConvertor.GetFormatedDateInDate("dd/MM/yyyy", System.DateTime.Today.ToShortDateString()) > objDateTimeConvertor.GetFormatedDateInDate("dd/MM/yyyy", WETOSSession.DateOfLicenseExpiry))
            {
                errorMsg = Messages.ErrorDisplay(709);
                return false;
            }

            if (objDateTimeConvertor.GetFormatedDateInDate("dd-MM-yyyy", System.DateTime.Today.ToShortDateString())
                > objDateTimeConvertor.GetFormatedDateInDate("dd-MM-yyyy", WETOSSession.DateOfLicenseExpiry))
            {
                errorMsg = Messages.ErrorDisplay(709);
                return false;
            }
            errorMsg = Messages.ErrorDisplay(0);
            return returnValue;

        }



    }
}
