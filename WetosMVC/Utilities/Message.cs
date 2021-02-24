using System;

namespace WetosMVCMainApp.Utilities
{
    public static class Messages
    {
        public static string ErrorDisplay(Int32 errorNo)
        {
            switch (errorNo)
            {

                case 0:
                    {
                        return " ";
                    }
                case 1:
                    {
                        return "Please select the company";
                    }
                case 2:
                    {
                        return "Please select a branch name";
                    }
                case 3:
                    {
                        return "Please select a department name";
                    }
                case 4:
                    {
                        return "Please select a designation";
                    }
                case 5:
                    {
                        return "Please select the employee";
                    }
                case 6:
                    {
                        return "Please select From date";
                    }
                case 7:
                    {
                        return "Please select To date";
                    }
                case 8:
                    {
                        return "To date should be greater than From date";
                    }
                case 9:
                    {
                        return "Please select shift code";
                    }
                case 10:
                    {
                        return "Selected From date and To date months are different";
                    }
                case 11:
                    {
                        return "Department code already exists!!!";
                    }
                case 12:
                    {
                        return "Department name already exists!!!";
                    }
                case 13:
                    {
                        return "You can not enter a blank value";
                    }
                case 14:
                    {
                        return "Logout time should be greater than Login time";
                    }

                case 15:
                    {
                        return "Employee code already exists!!!";
                    }
                case 16:
                    {
                        return "Please enter a valid Shift time!";
                    }
                case 17:
                    {
                        return "Please enter a valid Lunch time!";
                    }
                case 18:
                    {
                        return "Please enter a grade";
                    }

                case 19:
                    {
                        return "You  are not allowed to apply for this date!!!";
                    }
                case 20:
                    {
                        return "Salary has been processed for the given date!!!";
                    }
                case 21:
                    {
                        return "Sorry,Comp Off is not allowed for previous date";
                    }
                case 100:
                    {
                        return "No records found";
                    }
                case 101:
                    {
                        return "Files deleted successfully";
                    }
                case 102:
                    {
                        return "No records found for selected SSC/Funloc";
                    }
                case 103:
                    {
                        return "";
                    }
                case 104:
                    {
                        return "Unable to connect to the application server";
                    }
                case 105:
                    {
                        return "Unable to send the e-mail message(s)";
                    }
                case 106:
                    {
                        return "Unable to delete file(s)";
                    }
                case 107:
                    {
                        return "Data is already locked";
                    }
                case 108:
                    {
                        return "Data is locked";
                    }
                case 109:
                    {
                        return "Data is UnLocked";
                    }
                case 301:
                    {
                        return "You can not apply the Comp off for";
                    }
                case 302:
                    {
                        return "Please select the extra hour worked record";
                    }
                case 303:
                    {
                        return "Please give Reject reason...";
                    }
                case 304:
                    {
                        return "Please enter the place";
                    }
                case 305:
                    {
                        return "Please enter the purpose";
                    }
                case 306:
                    {
                        return "Sorry,Comp Off are not allowed";
                    }
                case 401:
                    {
                        return "Leave applied successFully !!!";
                    }
                case 402:
                    {
                        return "Error while saving Leave record !!!";
                    }
                case 403:
                    {
                        return "Already present on this Day !!!";
                    }
                case 404:
                    {
                        return "You are already Present on this Session !!!";
                    }
                case 405:
                    {
                        return "Leave is already applied for this Date !!!";
                    }
                case 406:
                    {
                        return "You have already crossed the Limit for this Month !!!";
                    }
                case 407:
                    {
                        return "No.Of Days exceed from the Max. Limit !!!";
                    }
                case 408:
                    {
                        return "From date should be greater or equal than To date ";
                    }
                case 409:
                    {
                        return "You don't have enough Leave Balance !!!";
                    }
                case 410:
                    {
                        return "Please select a Leave type!!!";
                    }
                case 411:
                    {
                        return "Please select a Financial year !!!";
                    }
                case 412:
                    {
                        return "Enter Encash value !!!";
                    }
                case 413:
                    {
                        return "You are already present for the first half of this date ";
                    }
                case 414:
                    {
                        return "You are already present for the second half of this date ";
                    }
                case 415:
                    {
                        return "You are already present on this date ";
                    }

                case 416:
                    {
                        return "Data not found !!!";

                    }
                case 417:
                    {
                        return "Enter leave purpose";
                    }
                case 418:
                    {
                        return "OD/Tour is already applied for this Date !!!";
                    }
                case 419:
                    {
                        return "Holiday is already applied";
                    }
                case 420:
                    {
                        return "Please enter a proper time";
                    }

                case 421:
                    {
                        return "Condone date should not be less than Current date !!! ";
                    }
                case 422:
                    {
                        return "Selected day should not be WeeklyOff !!! ";
                    }
                case 423:
                    {
                        return "Selected Day should not be Holiday !!! ";
                    }

                case 424:
                    {
                        return "First click on Show All button";
                    }
                case 425:
                    {
                        return "OD/TOUR is already applied for this Day!!! ";
                    }
                case 426:
                    {
                        return "Complementary working day 1 already applied for some other Day";
                    }
                case 427:
                    {
                        return "Complementary working day 2 already applied for some other Day";
                    }
                case 428:
                    {
                        return "Declare Holiday date should not be less than Current date";
                    }
                case 429:
                    {
                        return "You can not apply for Complementary working day less than Current day";
                    }
                case 430:
                    {
                        return "You can not apply for Late/Early more than two times in a Month";
                    }
                case 431:
                    {
                        return "You can't apply for Leave, OD is already applied for First Half";
                    }
                case 432:
                    {
                        return "You can't apply for Leave, OD is already applied for Second Half";
                    }
                case 433:
                    {
                        return "Applying date should be same as optional holiday date";
                    }

                case 434:
                    {
                        return "Leave is already for this day";
                    }

                case 501:
                    {
                        return "Please check your Mail settings!!!";
                    }
                case 999:
                    {
                        return "Record already exist !!!";

                    }

                case 201:
                    {
                        return "Leave applied successFully!!!";

                    }

                case 202:
                    {
                        return "Leave updated successfully!!!";

                    }
                case 203:
                    {
                        return "Please create Leave rules for the particular Employee group!!!";

                    }
                case 204:
                    {
                        return " Updated successfully!!!";

                    }


                case 205:
                    {
                        return "Shift schedule does not exist for the particular dates!!";

                    }
                case 206:
                    {
                        return "Please select Employee group name !!!";

                    }
                case 207:
                    {
                        return "Select Employee group first !!!";

                    }
                case 208:
                    {
                        return "First select Company!!!";

                    }

                case 209:
                    {
                        return "No Leaves are Present.First fillup Leave rules !!!";

                    }
                case 210:
                    {
                        return "You have already selected one Department!!!";

                    }

                case 211:
                    {
                        return "Financial year already exists for the particular dates .Please select a valid Date range";

                    }
                case 212:
                    {
                        return "Designation name already exists!!!";

                    }

                case 213:
                    {
                        return "Designation code already exists!!!";

                    }
                case 214:
                    {
                        return "Bulk User insertion completed";
                    }
                case 215:
                    {
                        return "Sorry!!! No optional holiday remaining";
                    }
                case 216:
                    {
                        return "Financial year is not created.";
                    }
                case 217:
                    {
                        return "You have already availed compoff in this month";
                    }

                case 601:
                    {
                        return "Selected days should be less than or equal to 31";

                    }
                case 701:
                    {
                        return "Select Branch first !!!";

                    }

                case 702:
                    {
                        return "Generate Txt file in directory";

                    }
                case 703:
                    {
                        return "Generate CSV file in directory";

                    }
                case 704:
                    {
                        return "Generate EXCEL file in directory";

                    }
                case 705:
                    {
                        return "Not a valid license.Please contact Eviska Infotech support team!!!!";

                    }
                case 706:
                    {
                        return "Access denied !!!!";

                    }
                case 707:
                    {
                        return "More companies are added in database than agreed in license";

                    }
                case 708:
                    {
                        return "More branches are added in database than agreed in license";

                    }
                case 709:
                    {
                        return "Your License for WETOS is expired. Please contact Eviska Infotech";

                    }
                case 710:
                    {
                        return "Please create a Financial year!!";
                    }
                case 800:
                    {
                        return "Child record found.You can not delete this record";
                    }
                case 801:
                    {
                        return "Maximum accumulation limit is ";
                    }
                case 802:
                    {
                        return "Maximum leaves allocated is ";
                    }
                case 1001:
                    {
                        return "You did not come Late on this day!!! ";
                    }
                case 1002:
                    {
                        return "You did not went Early on this day!!! ";
                    }
                case 600:
                    {
                        return "You can not do the AdjustmentEntry,Data is locked";
                    }
                case 610:
                    {
                        return "You can not apply for Leave,Data is locked";
                    }
                case 620:
                    {
                        return "You can not apply for OD/Tour,Data is locked";
                    }
                case 630:
                    {
                        return "You can not apply for Exception Entry,Data is locked";
                    }
                case 900:
                    {
                        return "CompOff updated successfully";
                    }
                case 901:
                    {
                        return "CompOff already applied for this date";
                    }
                case 902:
                    {
                        return "Your current year leave is not credit, Sanction is pending";
                    }
                case 903:
                    {
                        return "OD/TOUR apply successFully !!!";
                    }
                case 904:
                    {
                        return "Exception Entry is already applied for this date !!!";
                    }
                case 905:
                    {
                        return "Leave combination is not allowed for this Leave !!!";
                    }
                case 906:
                    {
                        return "You are login for the first time and needs to change your password !!!";
                    }
                case 907:
                    {
                        return "Please select proper Fromday status and Today status";
                    }
                case 908:
                    {
                        return "Please select division";
                    }
                case 909:
                    {
                        return "Please select grade";
                    }
                case 910:
                    {
                        return "Please select Employee group";
                    }
                case 911:
                    {
                        return "Not contains any employee";
                    }

                case 912:
                    {
                        return "HalfDay leave is not allowed";
                    }
                case 913:
                    {
                        return "FromDate and ToDate should be same.";
                    }
                case 914:
                    {
                        return "Please specify the charge handover to.";
                    }
                case 915:
                    {
                        return "You are not eligible to apply PL during the notice period";
                    }
                case 916:
                    {
                        return "From Date and ToDate should be of same financial year.";
                    }
                default:
                    {
                        return "";
                    }
            }
        }
    }

    public static class SuccessMessages
    {
        public const string Empty = "";
        public const string SaveSuccessFully = "Record saved successfully";
        public const string UpdateSuccessFully = "Record updated successfully";
        public const string DeleteSuccessFully = "Record deleted successfully";
        public const string FileSentSuccessfully = "File(s) sent successfully";
        public const string ApprovedSuccessFully = "Application is approved successfully !";
        public const string SanctionedSuccessFully = "Application is sanctioned successfully !";
        public const string RejectedSuccessFully = "Application is rejected successfully !";
        public const string CancelledSuccessFully = "Application is cancelled successfully !";
        public const string AppliedForCancelSuccessFully = "Application applied for cancellation successfully !";
        public const string MailSentSuccessfully = "Your error message has already sent to the WETOS team automatically...!";
        public const string AppliedSuccessfully = "Exception application is applied successfully...!";
    }
}
