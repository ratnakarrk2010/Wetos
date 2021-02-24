using System.Web.Mvc;

namespace WetosMVC.Controllers
{
    [SessionExpire] 
    [Authorize]
    public class WetosErrorMessageController : BaseController
    {

        /// <summary>
        /// Error Messages
        /// Added by Rajas on 5 JUNE 2017
        /// </summary>
        /// <param name="ErrorNo"></param>
        /// <return ErrorMessage =s></return ErrorMessage =s>
        public static string GetErrorMessage(int ErrorNo)
        {
            string ErrorMessage = string.Empty;

            switch (ErrorNo)
            {
                case 0:
                    {
                        return ErrorMessage;
                    }
                case 1:
                    {
                        return ErrorMessage = "Error in updating company";
                    }
                case 2:
                    {
                        return ErrorMessage = "Error in creating company";
                    }
                case 3:
                    {
                        return ErrorMessage = "Error in deleting company";
                    }
                case 4:
                    {
                        return ErrorMessage = "Company already available";
                    }
                case 5:
                    {
                        return ErrorMessage = "Error in updating branch";
                    }
                case 6:
                    {
                        return ErrorMessage = "Error in creating branch";
                    }
                case 7:
                    {
                        return ErrorMessage = "Error in deleting branch";
                    }
                case 8:
                    {
                        return ErrorMessage = "Branch already available";
                    }
                case 9:
                    {
                        return ErrorMessage = "Inconsistent data. Please try again!";
                    }
                case 10:
                    {
                        return ErrorMessage = "Error in updating financial year";
                    }
                case 11:
                    {
                        return ErrorMessage = "Error in creating financial year";
                    }
                case 12:
                    {
                        return ErrorMessage = "Error in deleting financial year";
                    }
                case 13:
                    {
                        return ErrorMessage = "Financial Year already available";
                    }
                case 14:
                    {
                        return ErrorMessage = "Invalid End Date";
                    }

                    //CODE ADDED BY SHRADDHA ON 16 JAN 2018
                case 15:
                    {
                        return ErrorMessage = "Error in deleting Condone Entry";
                    }
                default:
                    {
                        return ErrorMessage = "Error";
                    }
            }

        }

    }
}
