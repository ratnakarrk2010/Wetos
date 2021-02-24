using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
using WetosDB;
using WetosMVC;

namespace WetosMVCMainApp.Models
{

    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LogOnModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public string EmployeeNo { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public string Company { get; set; }   // Added by Rajas on 6 JULY 2017

    }



    /// <summary>
    /// Users Model
    /// Added by Rajas on 9 JULY 2017
    /// </summary>
    public class UserModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Please Enter User Name")]
        [Display(Name = "UserName")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "User name cannot be longer than 20 characters.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please Enter Password")]   // Added by Rajas on 26 JULY 2017
        [DataType(DataType.Password)]  // Added by Rajas on 20 JULY 2017 for Password masking
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Password cannot be longer than 50 characters.")]
        public string Password { get; set; }

        // Added by Rajas on 20 JULY 2017 for Password2
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Please Enter Confirm Password")]   // Added by Rajas on 26 JULY 2017
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Password cannot be longer than 50 characters.")]
        public string Password2 { get; set; }

        [Required(ErrorMessage = "Please Select Employee")]
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Employee")]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Please Select Role")]
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Role")]
        public int RoleTypeId { get; set; }

        public string Status { get; set; }
        public string LocationList { get; set; }
        public string CompanyList { get; set; }
        public string BranchList { get; set; }
        public string DepartmentList { get; set; }
        public string MenuId { get; set; }
        public string MastersRights { get; set; }
        public string AttendanceRights { get; set; }
        public string ApplicationsRights { get; set; }
        public string SanctionsRights { get; set; }
        public string ReportsRights { get; set; }
        public string AdministrationRights { get; set; }
        public string UpdatesRights { get; set; }
        public int BranchId { get; set; }
        public int CompanyId { get; set; }
        public string DivisionList { get; set; }
        public int DepartmentId { get; set; }
        public int DesignationId { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    //Added By shraddha on 17th OCT 2016 for model validation
    public class HolidayInputModel
    {
        public int FinancialYearId { get; set; } //Code added by shraddha on 22 FEB 2017 for adding financial year id in holiday table

        public int EmployeeId { get; set; }
        public int ReligionId { get; set; }
        public string Criteria { get; set; }

        public int DayStatus { get; set; }
        [Required]
        public string HLType { get; set; }

        public int CompanyId { get; set; }
        [Required]
        [Display(Name = "From Date")]
        //[DataType(DataType.DateTime)]
        public DateTime FromDate { get; set; }

        [Display(Name = "Branch")]
        public int Branchid { get; set; }
        [Required]
        [Display(Name = "From Date")]
        //[DataType(DataType.DateTime)]
        public DateTime ToDate { get; set; }

        [Display(Name = "Description")]
        [StringLength(150, MinimumLength = 1, ErrorMessage = "Description cannot be longer than 150 characters.")]
        public string Description { get; set; }

        public int HoliDayId { get; set; }  // Adde by Rajas on 14 MARCH 2017

        [Display(Name = "Holiday Date")]
        //[DataType(DataType.DateTime)]
        public DateTime HolidayDate { get; set; } //Code added by shraddha on 14 MAR 2017

        // Delete status
        public int? MarkedAsDelete { get; set; }  // Added by Rajas on 29 MARCH 2017


        public HolidayInputModel()
        {
            EmployeeId = 0;
            CompanyId = 0;
            Branchid = 0;
            Criteria = "";
            HLType = "";
            ToDate = DateTime.Now; //Code added by shraddha on 22 FEB 2017
            FromDate = DateTime.Now; //Code added by shraddha on 22 FEB 2017

            HolidayDate = DateTime.Now; //Code added by shraddha on 14 MAR 2017

            DayStatus = 0;
            ReligionId = 0;
            Description = "";
            MarkedAsDelete = 0;
        }

    }

    public class LeaveModel
    {

        public int EmployeeId { get; set; }

        [Required]
        [Display(Name = "Leave Type")]
        public int LeaveId { get; set; } // MODIFIED BY MSJ ON 22 JULY 2017

        public string LeaveName { get; set; } // ADDED BY MSJ ON 22 JULY 2017

        public string LeaveCode { get; set; } // ADDED BY SHRADDHA ON 29 NOV 2017

        public int CompanyId { get; set; }

        [Required]
        [Display(Name = "From Date")]
        //[DataType(DataType.DateTime)]
        public DateTime FromDate { get; set; }

        [Required]
        [Display(Name = "Branch")]                      //Validations added by Pushkar on 3 Nov
        public int Branchid { get; set; }

        [Required]
        [Display(Name = "To Date")]
        //[GreaterThan("FromDate")]
        //[DataType(DataType.DateTime)]
        public DateTime ToDate { get; set; }

        [Required]
        public string Purpose { get; set; }                 //Modified by Shraddha on 19th OCT 2016 for Leave Purpose checkbox

        public double AppliedDays { get; set; }

        public double ActualDays { get; set; }

        [Required]
        public int StatusId { get; set; }

        public int LeaveApplicationId { get; set; }

        [Required]
        [Display(Name = "From Day Status")]                  //Validations added by Pushkar on 3 Nov
        public int? FromDayStatus { get; set; }              //Modified by Shraddha on 03 Nov 2016 for Int FromDayStatus

        [Required]
        [Display(Name = "To Day Status")]             //Validations added by Pushkar on 3 Nov
        public int? ToDayStatus { get; set; }            //Modified by Shraddha on 03 Nov 2016 for Int ToDayStatus

        public string RejectReason { get; set; }

        public string Status { get; set; }          //Added By Shraddha on 12 DEC 2016 for taking Status along with statusId 

        // To check status during apply leave is self or other's
        public bool MySelf { get; set; }   // Added by Rajas on 13 MARCH 2017

        // Delete status
        public int? MarkedAsDelete { get; set; }  // Added by Rajas on 29 MARCH 2017

        public double DayDeduct { get; set; }   // Added by Rajas on 3 JULY 2017 for Sandwich deduction days popup

        // ADDED BY MSJ ON 19 JULY 2017 START
        public bool IsPreSandwichCase { get; set; }   //FLAG TO BE SET TO TRUE IF SANDWICH CASE DETECTED IN START DATE
        public bool IsPostSandwichCase { get; set; }   //FLAG TO BE SET TO TRUE IF SANDWICH CASE DETECTED IN END DATE
        public bool IsLWP { get; set; }   //FLAG TO BE SET TO TRUE IF LWP CASE DETECTED

        //public bool AddToNewTable { get; set; }

        //public DateTime PreSandwichStartDate { get; set; }  //ADD PRE SANWICH CASE START DATA
        //public DateTime PreSandwichEndDate { get; set; }    //ADD PRE SANWICH CASE END DATA
        //public DateTime PostSandwichStartDate { get; set; }    //ADD POST SANWICH CASE START DATA
        //public DateTime PostSandwichEndDate { get; set; }    //ADD POST SANWICH CASE END DATA
        //public DateTime LWPStartDate { get; set; }    //ADD LWP CASE START DATA
        //public DateTime LWPEndDate { get; set; }    //ADD LWP CASE END DATA

        //public double DaysCountedASSandwich { get; set; }
        //public double DaysCountedAsLWP { get; set; }
        // ADDED BY MSJ ON 19 JULY 2017 END

        // ADDED BY MSJ ON 22 JULY 2017 START
        public double LWP { get; set; }
        public double TotalDeductableDays { get; set; }
        public double LeaveConsumed { get; set; }
        public double PreSandwitchDays { get; set; }
        public double PostSandwitchDays { get; set; }

        public int PreSandwitchCaseLeaveId { get; set; }
        public int PostSandwitchCaseLeaveId { get; set; }
        // ADDED BY MSJ ON 22 JULY 2017 END


        //ADDED BY SHRADDHA ON 16 FEB 2018 START
        public bool? IsAttachmentNeeded { get; set; }
        public double? AttachmentRequiredForMinNoOfLeave { get; set; }
        public string DocPath { get; set; }
        public string DocFileName { get; set; }
        public string DocFolder { get; set; }
        //ADDED BY SHRADDHA ON 16 FEB 2018 END


        public DateTime? EffectiveDate { get; set; } //ADDED BY SHRADDHA ON 16 MAR 2018

        public string LeaveAddress { get; set; }

        public string AltContactNo { get; set; }

        /// <summary>
        /// Leave Model
        /// </summary>
        public LeaveModel()
        {
            EmployeeId = 0;

            CompanyId = 0;
            Branchid = 0;
            ToDayStatus = 0;
            ToDayStatus = 0;
            ToDate = DateTime.Now;
            FromDate = DateTime.Now;

            MySelf = true;

            StatusId = 1;
            RejectReason = "";

            Status = ""; //Added By Shraddha on 12 DEC 2016 for taking Status along with statusId 

            MarkedAsDelete = 0;
            DayDeduct = 0.0; // Added by Rajas on 3 JULY 2017 for Sandwich deduction days popup

            //CODE ADDED BY MSJ ON 19 JULY 2017 START
            IsPreSandwichCase = false; //FLAG TO BE SET TO TRUE IF SANDWICH CASE DETECTED IN START DATE
            IsPostSandwichCase = false; //FLAG TO BE SET TO TRUE IF SANDWICH CASE DETECTED IN END DATE
            IsLWP = false; //FLAG TO BE SET TO TRUE IF LWP CASE DETECTED

            //AddToNewTable = false;

            //PreSandwichStartDate = new DateTime();  //ADD PRE SANWICH CASE START DATA
            //PreSandwichEndDate = new DateTime();  //ADD PRE SANWICH CASE END DATA
            //PostSandwichStartDate = new DateTime();  //ADD POST SANWICH CASE START DATA
            //PostSandwichEndDate = new DateTime();  //ADD POST SANWICH CASE END DATA
            //LWPStartDate = new DateTime();  //ADD LWP CASE START DATA
            //LWPEndDate = new DateTime();  //ADD LWP CASE END DATA

            //DaysCountedASSandwich = 0.0;
            //DaysCountedAsLWP = 0.0;
            //CODE ADDED BY MSJ ON 19 JULY 2017 END
        }

        public IEnumerable<ValidationResult> Validation(ValidationContext context)
        {

            if (FromDate > ToDate)
            {
                yield return
                    new ValidationResult("End Date Should be Greater than Start Date", memberNames: new[] { "ToDate" });
            }
        }
    }

    public class SchedularModel
    {
        [Required]
        public string SchedularName { get; set; }

        public string Description { get; set; }

        public int? ScheduleType { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public int? FrequencyInMin { get; set; }

        public int? RepeatCycle { get; set; }

        public int ScheduleId { get; set; }// ADDED BY SHRADDHA ON 13 FEB 2018
    }

    public class ODTravelModel
    {
        //[Required]   //Commented By Shraddha on 14 DEC 2016 Because this validation not required                        //Validations added by Pushkar on 3 Nov
        [Display(Name = "Employee Name")]
        public int EmployeeId { get; set; }
        public string LeaveType { get; set; }


        public int CompanyId { get; set; }
        //[Required]
        [Display(Name = "From Date")]
        //[DataType(DataType.DateTime)]
        public DateTime? FromDate { get; set; }

        [Display(Name = "Branch")]
        public int Branchid { get; set; }
        //[Required]
        [Display(Name = "To Date")]
        //[DataType(DataType.DateTime)]
        public DateTime? ToDate { get; set; }



        public string OdTourHeadCode { get; set; }

        public double AppliedDay { get; set; }

        public double ActualDay { get; set; }


        public string RejectReason { get; set; }

        public int JourneyType { get; set; }
        //[Required]                                   //Validations added by Pushkar on 3 Nov
        [Display(Name = "To Day Status")]
        public int ODDayStatus1 { get; set; }     // Updated by Rajas on 19 AUGUST 2017 datatype changed from string to int

        public string TransportType { get; set; }

        //[Required]
        [Display(Name = "From Day Status")]
        public int ODDayStatus { get; set; }  // Updated by Rajas on 19 AUGUST 2017 datatype changed from string to int

        [Required]                                       //Validations added by Pushkar on 3 Nov
        [Display(Name = "Requisition Type")]
        public string ODTourType { get; set; }
        [Required]
        public string Place { get; set; }
        [Required]
        public string Purpose { get; set; }

        public int StatusId { get; set; }

        public int ODTourId { get; set; }

        public bool MySelf { get; set; }

        //CODE ADDED BY SHRADDHA ON 30 JAN 2018 START
        [Display(Name = "OD Date")]
        public DateTime? ODDate { get; set; }
        //[Required]
        [Display(Name = "OD Day Status")]
        public int ODDayStatus2 { get; set; }
        public DateTime? ODLoginTime { get; set; }
        public DateTime? ODLogOutTime { get; set; }
        public bool? IsInPunchInNextDay { get; set; }
        public bool? IsOutPunchInNextDay { get; set; }
        //CODE ADDED BY SHRADDHA ON 30 JAN 2018 END

        public DateTime? EffectiveDate { get; set; }  //CODE ADDED BY SHRADDHA ON 17 MAR 2018

        [Required]
        public string PurposeDesc { get; set; }

        public string ClientName { get; set; }

        public string CustomerName { get; set; }

        public string CSRNo { get; set; }

        public ODTravelModel()
        {
            EmployeeId = 0;
            CompanyId = 0;
            Branchid = 0;
            OdTourHeadCode = "";
            ODDayStatus1 = 0;
            ToDate = DateTime.Now;
            FromDate = DateTime.Now;
            Place = "";
            Purpose = "";
            StatusId = 1;
            ODTourId = 0;
            ODTourType = "";
            ODDayStatus = 0;
            TransportType = "";
            JourneyType = 0;
            RejectReason = "";
            MySelf = true; // Added by Rajas on 22 AUGUST 2017
            PurposeDesc = "";
            //CODE ADDED BY SHRADDHA ON 30 JAN 2018 START
            IsInPunchInNextDay = false;
            IsOutPunchInNextDay = false;
            //CODE ADDED BY SHRADDHA ON 30 JAN 2018 END
        }

    }



    public class COMPOffApplicationModel
    {
        public int CompOffId { get; set; }

        public int CompOffApplicationId { get; set; }

        //[Required]                                          //Validations added by Pushkar on 3 Nov
        [Display(Name = "From Day Status")]
        public int FromDateStatus { get; set; } //DATATYPE CHANGED BY SHRADDHA ON 11 AUG 2017 FROM STRING TO INT

        //[Required]                                              //Validations added by Pushkar on 3 Nov
        [Display(Name = "To Day Status")]
        public int ToDateStatus { get; set; } //DATATYPE CHANGED BY SHRADDHA ON 11 AUG 2017 FROM STRING TO INT

        public string Purpose { get; set; }

        public int StatusId { get; set; }

        public string RejectReason { get; set; }

        public string ExtrasHoursDate { get; set; }

        [Required]                                                //Validations added by Pushkar on 3 Nov
        [Display(Name = "Company Name")]
        public int CompanyId { get; set; }

        [Required]
        [Display(Name = "Branch Name")]
        public int BranchId { get; set; }

        [Required]
        [Display(Name = "Employee Name")]
        public int EmployeeId { get; set; }

        [Required]
        [Display(Name = "From Date")]
        [DataType(DataType.DateTime)]
        public DateTime? FromDate { get; set; }        //DataType Modified By Shraddha on 12 DEC 2016 from string to DateTime for Model Validation

        [Required]
        [Display(Name = "To Date")]
        [DataType(DataType.DateTime)]
        public DateTime? ToDate { get; set; }        //DataType Modified By Shraddha on 12 DEC 2016 from string to DateTime for Model Validation

        public DateTime? EffectiveDate { get; set; } //CODE ADDED BY SHRADDHA ON 17 MAR 2018

        public string Myself { get; set; }

        public COMPOffApplicationModel()
        {
            CompOffId = 0;
            CompanyId = 0;
            BranchId = 0;
            EmployeeId = 0;
            // COMMENTED BY MSJ 15 SEP 2017
            //FromDate = 0;  //DATATYPE CHANGED BY SHRADDHA ON 11 AUG 2017 FROM STRING TO INT
            ToDateStatus = 0; //DATATYPE CHANGED BY SHRADDHA ON 11 AUG 2017 FROM STRING TO INT
            ToDate = DateTime.Now;
            FromDate = DateTime.Now;  //UNCOMMENTED BY SHRADDHA ON 16 SEP 2017 BECAUSE IT IS REQUIRED Removed .Tostring() because fromdate is now datetime.
            Purpose = "";
            StatusId = 1;
            RejectReason = "";
            ExtrasHoursDate = "";
            Myself = "";
        }
    }

    /// <summary>
    /// ExceptionEntryModel
    /// Added by Rajas on 2 OCT 2017
    /// </summary>
    public class ExceptionEntryModel
    {
        public int ExceptionId { get; set; }
        public int EmployeeId { get; set; }

        public DateTime ExceptionDate { get; set; }

        public string ShiftId { get; set; }

        [Required]
        public DateTime LoginTime { get; set; }

        [Required]
        public DateTime LogOutTime { get; set; }

        public DateTime Login2Time { get; set; }
        public DateTime LogOut2Time { get; set; }

        public DateTime Late { get; set; }
        public DateTime Early { get; set; }

        public DateTime WorkingHrs { get; set; }
        public DateTime ExtraHrs { get; set; }

        public string ActualDayStatus { get; set; }

        public DateTime? PreviousLoginTime { get; set; }
        public DateTime? PreviousLogOutTime { get; set; }

        public DateTime PreviousLogin2Time { get; set; }
        public DateTime PreviousLogOut2Time { get; set; }
        public DateTime PreviousEarly { get; set; }
        public DateTime PreviousLate { get; set; }
        public DateTime PreviousWrokingHrs { get; set; }
        public DateTime PreviousExtraHrs { get; set; }

        public string PreviousDayStatus { get; set; }
        public string PreviousShiftId { get; set; }

        public string Remark { get; set; }

        public string Status { get; set; }

        public string RejectReason { get; set; }

        public int CompanyId { get; set; }
        public int BranchId { get; set; }
        public int DailyTrnId { get; set; }
        public int MarkedAsDelete { get; set; }
        public int StatusId { get; set; }
        public bool MySelf { get; set; }
        public DateTime TranDate { get; set; }

        public string DailyTranStatus { get; set; }

        public bool? IsOutPunchInNextDay { get; set; }//ADDED BY SHRADDHA ON 15 JAN 2018
        public bool? IsInPunchInNextDay { get; set; }//ADDED BY SHRADDHA ON 15 JAN 2018

        public DateTime? EffectiveDate { get; set; } // CODE ADDED BY SHRADDHA 0N 17 MAR 2018

        //[Required]
        public string Description { get; set; }

        // ADDED BY MSJ ON 27 JAN 2020 START
        [Required]
        public DateTime FromDate { get; set; }

        [Required]
        public DateTime ToDate { get; set; }

        [Required]
        public DateTime SelectionCriteria { get; set; }
        // ADDED BY MSJ ON 27 JAN 2020 END

        public int Reason { get; set; } // ADDED BY MSJ ON 14 FEB 2020

    }



    /// <summary>
    /// ADDED BY SHRADDHA ON 18TH OCT 2016 for Condone Validation
    /// Updated by Rajas on 6 MARCH 2017
    /// </summary>
    public class CondoneModel
    {
        public int CondoneId { get; set; } // Added By Rajas on 6 MARCH 2017

        public int TransportId { get; set; }

        public int? DivisionId { get; set; }

        public string Employeelist { get; set; }

        public int CompanyId { get; set; }

        public int BranchId { get; set; }

        [Required]
        [Display(Name = "Condone Date")]
        public DateTime CondoneDate { get; set; }

        [Required]
        public string ShiftId { get; set; }

        public string Reason { get; set; }

        public string CondoneStatus { get; set; }  // Added By SHraddha on 28 DEC 2016

        public string LateEarly { get; set; }

        public CondoneModel()
        {
            TransportId = 0;
            DivisionId = 0;
            CompanyId = 0;
            BranchId = 0;
            Employeelist = "";
            ShiftId = "";
            CondoneDate = DateTime.Now;  // Updated by Rajas on 23 FEB 2017
            Reason = "";
            LateEarly = "";
        }
    }



    /// <summary>
    /// EmployeeModel
    /// </summary>
    /// Updated by Rajas on 2 JUNE 2017
    public class EmployeeModel
    {
        //Modified By Shraddha on 13 DEC 2016 removed all [Required] conditions

        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Please Enter Employee code")]  // Added by Rajas on 6 MARCH 2017
        [Display(Name = "Employee code")]
        //[Required] 
        [StringLength(20, MinimumLength = 1, ErrorMessage = "Code cannot be longer than 20 characters.")]
        public string EmployeeCode { get; set; }

        [Display(Name = "Employee Type")]
        // Specify Range Validation for dropdwon 
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Employee type")]
        public int EmployeeTypeId { get; set; }

        [Required(ErrorMessage = "Please Enter First Name")]  // Added by Rajas on 6 MARCH 2017
        [Display(Name = "First Name")]
        [StringLength(25, ErrorMessage = "First Name cannot be longer than 25 characters.")]
        //[Required]  // Added by Rajas on 6 MARCH 2017
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please Enter Last Name")]  // Added by Rajas on 6 MARCH 2017
        [Display(Name = "Last Name")]
        [StringLength(25, MinimumLength = 1, ErrorMessage = "Last Name cannot be longer than 25 characters.")]
        //[Required]  // Added by Rajas on 6 MARCH 2017
        public string LastName { get; set; }

        //[Required(ErrorMessage = "Please Enter Middle Name")]  // Added by Rajas on 6 MARCH 2017
        [Display(Name = "Middle Name")]
        [StringLength(25, MinimumLength = 1, ErrorMessage = "Middle Name cannot be longer than 25 characters.")]
        public string MiddleName { get; set; }

        //[Display(Name = "Address1")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Address1 cannot be longer than 255 characters.")]
        public string Address1 { get; set; }

        //[Display(Name = "Address2")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Address2 cannot be longer than 255 characters.")]
        public string Address2 { get; set; }

        [Display(Name = "Email")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail adress")]
        public string MailAddress { get; set; }

        //[Required(ErrorMessage = "Please Enter Prefix")]  // Added by Rajas on 6 MARCH 2017
        [Display(Name = "Prefix")]
        [StringLength(6, MinimumLength = 1, ErrorMessage = "Prefix cannot be longer than 6 characters.")]
        public string Prefix { get; set; }

        [Required(ErrorMessage = "Please select Gender")]  // Added by Rajas on 6 MARCH 2017
        [Display(Name = "Gender")]
        public string Gender { get; set; }

        //[Required]
        [StringLength(25, MinimumLength = 1, ErrorMessage = "Card number cannot be longer than 25 characters.")]
        public string CardNumber { get; set; }

        [Required]
        public string DefaultShift { get; set; }

        // Commented by Rajas as per point discussed in 5 MAY 2017 meeting at Eviska
        //[Required(ErrorMessage = "Please select Blood group")]  // Added by Rajas on 6 MARCH 2017
        public int? BloodGroupId { get; set; }

        // Commented by Rajas as per point discussed in 5 MAY 2017 meeting at Eviska
        //[Required(ErrorMessage = "Please select Religion")]  // Added by Rajas on 6 MARCH 2017
        public int? ReligionId { get; set; }

        [Display(Name = "Designation")]
        // Specify Range Validation for dropdwon 
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Designation")]
        public int DesignationId { get; set; }

        [Display(Name = "Department")]
        // Specify Range Validation for dropdwon 
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Department")]
        public int DepartmentId { get; set; }

        //[Required] //COMMENTED BY SHRADDHA ON 19 JULY 2017 AS PER SUGGESTED BY MSJ
        [Display(Name = "Please select Reporting person")]
        // Specify Range Validation for dropdwon 
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Reporting person")]
        public int? EmployeeReportingId { get; set; }

        //[Required(ErrorMessage = "Please select Grade")]  // Added by Rajas on 16 MARCH 2017 //COMMENTED BY SHRADDHA ON 19 JULY 2017 AS PER SUGGESTED BY MSJ
        // Specify Range Validation for dropdwon 
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Grade")]
        public int? GradeId { get; set; }

        public string DeptHeadFlag { get; set; }  // Added by Rajas on 4 MARCH 2017

        public string MarritalStatus { get; set; }

        [Display(Name = "Email")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail adress")]
        public string Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid Phone number")]
        public string Telephone1 { get; set; }

        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid Phone number")]
        public string Telephone2 { get; set; }

        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid Phone number")]
        public string Moblie { get; set; }

        //[Required(ErrorMessage = "Birth Date")] //COMMENTED BY SHRADDHA ON 19 JULY 2017 AS PER SUGGESTED BY MSJ
        public DateTime? BirthDate { get; set; }

        //[Required(ErrorMessage = "Confirm Date")]  // Commented by Rajas as per point discussed in 5 MAY 2017 meeting at Eviska
        public DateTime? ConfirmDate { get; set; }


        [Required(ErrorMessage = "Joining Date")]
        public DateTime? JoiningDate { get; set; }

        public DateTime? Leavingdate { get; set; }

        // Added by Shraddha
        //[Required(ErrorMessage = "Leave Applicable from Date")] //COMMENTED BY SHRADDHA ON 09 FEB 2018 AS PER REQUIRED BY LNT NAGAR AND AS PER SUGGESTED BY MSJ
        public DateTime? LeaveEffectiveFromDate { get; set; }

        public string PFNO { get; set; }

        public string PANNO { get; set; }

        //public string AADHAARNO { get; set; }
        public string ESICNO { get; set; }

        public string ESICDCD { get; set; }

        public string BankCode { get; set; }

        public string BankNo { get; set; }

        public string LICNo { get; set; }

        public string SwipeFlag { get; set; }

        public string ActiveFlag { get; set; } // Added by Rajas on 4 MARCH 2017

        public string SalPDFlag { get; set; }  // Added by Rajas on 4 MARCH 2017

        public DateTime? ResignationDate { get; set; } //ADDED BY PRATHMESH ON 17 APRIL 2019

        public int? NoticePeriod { get; set; } //ADDED BY PRATHMESH ON 17 APRIL 2019

        //Added by Rajas on 11 JAN 2017, as week off selection provided instead of hardcode value START

        [Required(ErrorMessage = "Please select week off")]  // Added by Rajas on 6 MARCH 2017
        //[Required]  // Added by Rajas on 4 MARCH 2017
        public string WeeklyOff1 { get; set; }


        public string WeeklyOff2 { get; set; }

        public bool? First { get; set; }

        public bool? Second { get; set; }

        public bool? Third { get; set; }

        public bool? Fourth { get; set; }

        public bool? Fifth { get; set; }

        //public bool? IsSecondWeekoffHalfDay { get; set; }
        //Added by Rajas on 11 JAN 2017, as week off selection provided instead of hardcode value START

        // Added by Rajas on 18 FEB 2017
        [Display(Name = "Employee Group")]
        [Required(ErrorMessage = "Please select Employee group")]
        public int EmployeeGroupId { get; set; }

        [Display(Name = "Company")]
        // Specify Range Validation for dropdwon 
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Company")]
        public int CompanyId { get; set; }

        [Display(Name = "Branch")]
        // Specify Range Validation for dropdwon 
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Branch")]
        public int BranchId { get; set; }

        [Display(Name = "Division")]
        // Specify Range Validation for dropdwon 
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Division")]
        public int? DivisionId { get; set; }

        //[Required] //COMMENTED BY SHRADDHA ON 19 JULY 2017 AS PER SUGGESTED BY MSJ
        [Display(Name = "Please select Reporting person")]
        // Specify Range Validation for dropdwon 
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Reporting person")]
        public int? EmployeeReportingId2 { get; set; } // ADDED by Rajas on 20 JULY 2017 To handle nullable

        public byte[] Picture { get; set; } // ADDED BY SHRADDHA ON 29 DEC 2016 FOR PICTURE SAVE IN BYTE[]

        [Display(Name = "Qualification")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Qualification cannot be longer than 255 characters.")]
        public string Qualification { get; set; }

        [Display(Name = "Graduation Passing Year")]

        public string GradPassingYear { get; set; }


        [Display(Name = "Post Graduation Passing Year")]

        public string PostGradPassingYear { get; set; }

        [Display(Name = "Previous Experience")]

        public string PrevExperience { get; set; }

        [Display(Name = "Experience With Flagship")]

        public string FlagshipExperience { get; set; }

        [Display(Name = "Other Passing Year")]

        public string OtherPassingYear { get; set; }
        // Delete status
        public int? MarkedAsDelete { get; set; }  // Added by Rajas on 29 MARCH 2017


        [Display(Name = "Graduation")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Graduation Qualification cannot be longer than 50 characters.")]
        public string GraduationQualification { get; set; }

        [Display(Name = "Post Graduation")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Post Graduation Qualification cannot be longer than 50 characters.")]
        public string PostGraduationQualification { get; set; }


        [Display(Name = "Other Graduation")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Post Graduation Qualification cannot be longer than 50 characters.")]
        public string OtherGraduationQualification { get; set; }

        //CODE ADDED BY SHRADDHA ON 10 FEB 2018 START
        public List<FamilyDetails> TblFamilyDetailList { get; set; }

        //CODE ADDED BY SHRADDHA ON 10 FEB 2018 START
        public int FamilyDetailsMaxRecordsCount { get; set; }


        [Display(Name = "Seperation Remark")]
        public string SeperationRemark { get; set; }//ADDED BY SHRADDHA ON 06 MAR 2018

        public bool? IsSecondWeekoffHalfDay { get; set; } // CODE ADDED BY SHRADDHA ON 12 MAR 2018

        public EmployeeModel()
        {
            SeperationRemark = string.Empty; //ADDED BY SHRADDHA ON 06 MAR 2018
            EmployeeId = 0;
            CompanyId = 0;
            BranchId = 0;
            EmployeeTypeId = 1;
            DivisionId = 1;
            MarkedAsDelete = 0;
            FamilyDetailsMaxRecordsCount = 0;//CODE ADDED BY SHRADDHA ON 10 FEB 2018 START
            if (TblFamilyDetailList == null)
            {

                TblFamilyDetailList = new List<FamilyDetails>();
            }

        }

        //CODE ADDED BY SHRADDHA ON 10 FEB 2018 END
    }

    public class FamilyDetails
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public string Relation { get; set; }
        public string Gender { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public string DateOfBirth { get; set; }
        public byte[] Picture { get; set; }
        public int Id { get; set; }

    }

    //ADDED BY SHRADDHA ON 28 DEC 2016 TO USE SHIFT MODEL IN SHIFT EDIT PAGE INSTEAD OF TABLE
    public class ShiftModel
    {
        [Required(ErrorMessage = "Please select Company")]
        [Display(Name = "Company Name")]
        // Specify Range Validation for dropdwon 
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Company")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "Please select Branch")]
        [Display(Name = "Branch Name")]
        // Specify Range Validation for dropdwon 
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Branch")]
        public int BranchId { get; set; }

        [Display(Name = "Shift Type")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "Shift type cannot be longer than 20 characters.")]
        public string ShiftType { get; set; }

        [Display(Name = "Shift Name")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Shift name cannot be longer than 50 characters.")]
        public string ShiftName { get; set; }

        //[Required(ErrorMessage = "Please Enter Working Hours")]
        //[Display(Name = "WorkingHours")]
        //public DateTime? WorkingHours { get; set; }

        [Required(ErrorMessage = "Please Enter Shift Code")]
        [StringLength(4, MinimumLength = 1, ErrorMessage = "Code cannot be longer than 4 characters.")]
        public string ShiftCode { get; set; }

        public int ShiftId { get; set; }

        [Required(ErrorMessage = "Please Enter First In Time in 24 Hrs format")]
        [Display(Name = "FirstInTime")]
        public string FirstInTime { get; set; }

        [Required(ErrorMessage = "Please Enter First Out Time in 24 Hrs format")]
        [Display(Name = "FirstOutTime")]
        public string FirstOutTime { get; set; }

        public string SecondInTime { get; set; }

        public string SecondOutTime { get; set; }

        [Required(ErrorMessage = "Please Enter Lunch start time in 24 Hrs format")]
        [Display(Name = "LunchStartTime")]
        public string LunchStartTime { get; set; }

        [Required(ErrorMessage = "Please Enter Lunch end time in 24 Hrs format")]
        [Display(Name = "LunchEndTime")]
        public string LunchEndTime { get; set; }

        public string LunchTime { get; set; }

        public bool? LunchTimeExcludeFlag { get; set; }

        public string WorkHours { get; set; }

        public bool? NightShiftFlag { get; set; }

        // Delete status
        public int? MarkedAsDelete { get; set; }  // Added by Rajas on 29 MARCH 2017

        public bool? IsOutPunchInNextDay { get; set; } // ADDED BY SHRADDHA ON 15 JAN 2018 FOR LNT SECOND SHIFT INTIME: 15:30:00 OUT TIME: 00:00:00 IN NEXT DAY

        public ShiftModel()
        {

            FirstInTime = "";

            FirstOutTime = "";

            SecondInTime = "";

            SecondOutTime = "";
            LunchStartTime = "";

            LunchEndTime = "";
            LunchTime = "";

            WorkHours = "";

        }

    }

    /// <summary>
    /// RoleDefModel
    /// Added by Rajas on 3 JUNE 2017
    /// </summary>
    public class RoleDefModel
    {
        public int RoleId { get; set; }

        [Required(ErrorMessage = "Please Enter Role Name")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Role name cannot be longer than 50 characters.")]
        public string RoleName { get; set; }

        [Required(ErrorMessage = "Please Enter Role Description")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Role Description cannot be longer than 100 characters.")]
        public string RoleDescription { get; set; }

        public int MarkedAsDelete { get; set; }
    }

    // ADDED BY RAJAS ON 29 DEC 2016 FOR VALIDATE DATA THROUGH MODEL START
    #region MASTER VALIDATION START

    // Location model added for validation of Location master
    public class LocationModel
    {
        public int LocationId { get; set; }

        [Required(ErrorMessage = "Please Enter Location")]
        [Display(Name = "Location")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Location Name cannot be longer than 50 characters.")]
        public string LocationName { get; set; }

        [Display(Name = "Address")]
        [StringLength(150, MinimumLength = 1, ErrorMessage = "Address cannot be longer than 150 characters.")]
        public string Address { get; set; }

        [Display(Name = "City")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "City name cannot be longer than 20 characters.")]
        public string City { get; set; }

        // Delete status
        public int? MarkedAsDelete { get; set; }  // Added by Rajas on 28 MARCH 2017
    }

    // LeaveMasterModel added for validation of Leave master 
    public class LeaveMasterModel
    {
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Location Name cannot be longer than 50 characters.")]
        [Required(ErrorMessage = "Please Enter Leave Code")]
        [Display(Name = "LeaveCode")]
        public string LeaveCode { get; set; }

        [StringLength(50, MinimumLength = 1, ErrorMessage = "Location Name cannot be longer than 50 characters.")]
        [Required(ErrorMessage = "Please Enter Leave Name")]
        [Display(Name = "LeaveName")]
        public string LeaveName { get; set; }


        [StringLength(50, MinimumLength = 1, ErrorMessage = "Location Name cannot be longer than 50 characters.")]
        public string LeaveDescription { get; set; }

        [Required(ErrorMessage = "Please enter No Of Days Allowed In Year")]
        [Display(Name = "NoOfDaysAllowedInYear")]
        public double? NoOfDaysAllowedInYear { get; set; }

        [Required(ErrorMessage = "Please enter Max No Of Times Allowed In Year")]
        [Display(Name = "MaxNoOfTimesAllowedInYear")]
        public double? MaxNoOfTimesAllowedInYear { get; set; }

        [Required(ErrorMessage = "Please enter Max No Of Leaves Allowed At a Time")]
        [Display(Name = "MaxNoOfLeavesAllowedAtaTime")]
        public double? MaxNoOfLeavesAllowedAtaTime { get; set; }

        [Required(ErrorMessage = "Please enter Max No Of Leaves Allowed In Month")]
        [Display(Name = "MaxNoOfLeavesAllowedInMonth")]
        public double? MaxNoOfLeavesAllowedInMonth { get; set; }

        public bool? AccumulationAllowedOrNot { get; set; }

        public double? MaxAccumulationDays { get; set; }

        public bool? EncashmentAllowedOrNot { get; set; }

        public double? NoOfDaysEncashed { get; set; }

        public bool? NegativeBalanceAllowed { get; set; }

        public bool? IsCarryForword { get; set; }

        public bool? WOBetLevConsiderLeave { get; set; }

        public bool? HHBetLevConsiderLeave { get; set; }

        public bool? ISHalfAllowed { get; set; }

        [Required(ErrorMessage = "Please Select Employee Group")]
        [Display(Name = "EmployeeGrpId")]
        public int EmployeeGrpId { get; set; }

        [Required(ErrorMessage = "Please Select Branch")]
        [Display(Name = "BranchId")]
        public int BranchId { get; set; }

        [Required(ErrorMessage = "Please Select Company")]
        [Display(Name = "CompanyId")]
        public int CompanyId { get; set; }

        public bool? LeaveType { get; set; }

        //[Required(ErrorMessage = "Please enter Min No of Leave Allowed at a time")]
        [Display(Name = "MinNoofLeaveAllowedatatime")]
        public double? MinNoofLeaveAllowedatatime { get; set; }

        public bool? IsLeaveCombination { get; set; }

        public int LeaveId { get; set; }

        public int EmployeeTypeId { get; set; }


        [Required(ErrorMessage = "Please select Valid Leave Credit Type")]
        public int? LeaveCreditTypeId { get; set; }  // Added by Rajas on 1 MARCH 2017 as field is available in Eviska database

        public string ApplicableToMaleFemale { get; set; } // Added by Rajas on 1 MARCH 2017 as field is available in Eviska database

        // Delete status
        public int? MarkedAsDelete { get; set; }  // Added by Rajas on 29 MARCH 2017

        //ADDED CODE BY SHRADDHA ON 16 FEB 2018 START
        public bool? IsAttachmentNeeded { get; set; }
        public double? AttachmentRequiredForMinNoOfLeave { get; set; }
        //ADDED CODE BY SHRADDHA ON 16 FEB 2018 END


        public string PrefixId { get; set; }
        public string SuffixId { get; set; }


        public string PrefixIdStr { get; set; }
        public string SuffixIdStr { get; set; }

        // Added by Rajas on 9 JUNE 2017
        public LeaveMasterModel()
        {
            MaxAccumulationDays = 0;
            NoOfDaysEncashed = 0;
            MaxNoOfLeavesAllowedAtaTime = 0;
            MaxNoOfLeavesAllowedInMonth = 0;
            MaxNoOfTimesAllowedInYear = 0;
            MinNoofLeaveAllowedatatime = 0;
            NoOfDaysAllowedInYear = 0;
        }

    }

    /// <summary>
    /// TEMP ADDED FOR LEAVE BY MSJ ON 15 JAN 2017 START
    /// </summary>
    public class LeaveCompBranchDeptModel
    {
        //[Required(ErrorMessage = "Please Select Employee Group")]
        [Display(Name = "EmployeeGrpId")]
        public int? EmployeeGrpId { get; set; }

        //[Required(ErrorMessage = "Please Select Branch")]
        [Display(Name = "BranchId")]
        public int? BranchId { get; set; }

        [Required(ErrorMessage = "Please Select Company")]
        [Display(Name = "CompanyId")]
        public int CompanyId { get; set; }
    }
    /// TEMP ADDED FOR LEAVE BY MSJ ON 15 JAN 2017 START

    /// <summary>
    /// Added for holiday master validation
    /// </summary>
    /// Updated by Rajas 
    public class HoliDayModel
    {
        public int HoliDayId { get; set; }

        public string HLType { get; set; }

        public int EmployeeId { get; set; }

        public int ReligionId { get; set; }

        public string FromDate { get; set; }

        public string ToDate { get; set; }

        public string Criteria { get; set; }

        [StringLength(150, MinimumLength = 1, ErrorMessage = "Description cannot be longer than 150 characters.")]
        public string Description { get; set; }

        public int DayStatus { get; set; }

        // Specify Range Validation for dropdwon 
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Company")]
        public int CompanyId { get; set; }

        // Specify Range Validation for dropdwon 
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Branch")]
        public int Branchid { get; set; }

    }

    /// <summary>
    /// Added for Bloodgroup master validation
    /// </summary>
    public class BloodGroupModel
    {
        public int BloodGroupId { get; set; }

        [Required(ErrorMessage = "Please Enter Blood Group Name")]
        [Display(Name = "BloodGroupName")]
        public string BloodGroupName { get; set; }
    }

    /// <summary>
    /// Added for Device master validation
    /// </summary>
    public class DeviceModel
    {
        public int DeviceId { get; set; }

        [Required(ErrorMessage = "Please Enter Device Name")]
        [Display(Name = "DeviceName")]
        public string DeviceName { get; set; }

        public string DeviceNo { get; set; }

        public string DISPLAYNAME { get; set; }
    }

    /// <summary>
    /// Addded for department master validation
    /// </summary>
    /// Updated by Rajas on 2 JUNE 2017
    public class DepartmentModel
    {
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Please Enter Department Name")]
        [Display(Name = "Department")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Department name cannot be longer than 50 characters.")]
        public string DepartmentName { get; set; }

        [Required(ErrorMessage = "Please Enter Department Code")]
        [Display(Name = "Department code")]
        [StringLength(15, MinimumLength = 1, ErrorMessage = "Department code cannot be longer than 15 characters.")]
        public string DepartmentCode { get; set; }

        [Required(ErrorMessage = "Please Select Company Name")]
        [Display(Name = "Company")]
        // Range validation for dropdown
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Company Name")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "Please Select Branch Name")]
        [Display(Name = "Branch")]
        public int BranchId { get; set; }

        // Delete status
        public int? MarkedAsDelete { get; set; }  // Added by Rajas on 29 MARCH 2017
    }

    /// <summary>
    /// CompanyModel
    /// </summary>
    /// Updated by Rajas on 2 JUNE 2017
    public class CompanyModel
    {
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "Please Enter Company Name")]
        [Display(Name = "CompanyName")]
        [StringLength(50, ErrorMessage = "Company Name cannot be longer than 50 characters.")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Please Select Location")]
        [Display(Name = "LocationId")]
        // Specify Range
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Location")]
        public int LocationId { get; set; }

        //[Required(ErrorMessage = "Please Enter Address")]
        [Display(Name = "Address1")]
        [StringLength(255, ErrorMessage = "Address1 cannot be longer than 255 characters.")]
        public string Address1 { get; set; }

        [StringLength(255, ErrorMessage = "Address2 cannot be longer than 255 characters.")]
        public string Address2 { get; set; }

        // Model validation added by Rajas on 17 JAN 2017
        [DataType(DataType.PhoneNumber)]
        //[RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid Phone number")]
        //[Required(ErrorMessage = "Please Enter Telephone")]
        [Display(Name = "Telephone")]
        [StringLength(15, ErrorMessage = "Telephone number cannot be longer than 15 characters.")]
        public string Telephone { get; set; }

        [StringLength(15, ErrorMessage = "FAX cannot be longer than 15 characters.")]
        public string Fax { get; set; }

        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail adress")]
        public string Email { get; set; }

        [StringLength(12, ErrorMessage = "PAN Number cannot be longer than 12 characters.")]
        public string PAN { get; set; }

        public string CST { get; set; }

        public string EXC { get; set; }

        public string TDS { get; set; }

        public string SGNNM { get; set; }

        public string SGNPlace { get; set; }

        // Delete status
        public int? MarkedAsDelete { get; set; }  // Added by Rajas on 28 MARCH 2017

        //Added by shalaka on 16th Dec 2017 -- Start
        [StringLength(6, ErrorMessage = "PIN CODE Number cannot be longer than 6 characters.")]
        public string PinCode { get; set; }
        //Added by shalaka on 16th Dec 2017 -- End

        public CompanyModel()
        {
            CST = string.Empty;
            EXC = string.Empty;
            TDS = string.Empty;
            SGNNM = string.Empty;
            SGNPlace = string.Empty;
            CompanyName = string.Empty;
            Address2 = string.Empty;
            Address1 = string.Empty;
            Fax = string.Empty;
            Telephone = string.Empty;
        }

    }

    /// <summary>
    /// Branch Model
    /// </summary>
    /// Updated on 2 JUNE 2017
    public class BranchModel
    {
        public int BranchId { get; set; }

        [Required(ErrorMessage = "Please Enter Branch Name")]
        [Display(Name = "BranchName")]
        // Validation for string length
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Branch name cannot be longer than 50 characters.")]
        public string BranchName { get; set; }

        [Required(ErrorMessage = "Please Select Company")]
        // Specify Range Validation for dropdwon 
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Company")]
        [Display(Name = "CompanyId")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "Please Select Location")]
        [Display(Name = "LocationId")]
        public int LocationId { get; set; }

        [Display(Name = "Address1")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Address1 cannot be longer than 100 characters.")]
        public string Address1 { get; set; }

        [StringLength(100, MinimumLength = 1, ErrorMessage = "Address2 cannot be longer than 100 characters.")]
        public string Address2 { get; set; }

        [StringLength(15, MinimumLength = 1, ErrorMessage = "Telephone cannot be longer than 15 characters.")]
        public string Telephone { get; set; }

        [StringLength(15, MinimumLength = 1, ErrorMessage = "FAX cannot be longer than 15 characters.")]
        public string FAX { get; set; }

        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail adress")]
        public string Email { get; set; }

        public int? MarkedAsDelete { get; set; }

    }


    /// <summary>
    /// DesignationModel
    /// </summary>
    /// Updated by Rajas on 2 JUNE 2017
    public class DesignationModel
    {
        public int DesignationId { get; set; }

        [Required(ErrorMessage = "Please Enter Designation Name")]
        [Display(Name = "DesignationName")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Designation name cannot be longer than 50 characters.")]
        public string DesignationName { get; set; }

        [Required(ErrorMessage = "Please Enter Designation Code")]
        [Display(Name = "DesignationCode")]
        [StringLength(6, MinimumLength = 1, ErrorMessage = "Designation code cannot be longer than 6 characters.")]
        public string DesignationCode { get; set; }

        [Required(ErrorMessage = "Please Select Department")]
        [Display(Name = "DepartmentId")]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Please Select Company")]
        [Display(Name = "CompanyId")]
        // Specify Range Validation for dropdwon 
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Company")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "Please Select Branch")]
        [Display(Name = "BranchId")]
        // Specify Range Validation for dropdwon 
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Branch")]
        public int BranchId { get; set; }

        // Delete status
        public int? MarkedAsDelete { get; set; }  // Added by Rajas on 29 MARCH 2017

    }


    /// <summary>
    /// Added for Employee group validation
    /// </summary>
    /// Updared by Rajas on 2 JUNE 2017
    public class EmployeeGroupModel
    {
        public int EmployeeGroupId { get; set; }

        [Required(ErrorMessage = "Please Enter Employee Group Name")]
        [Display(Name = "EmployeeGroupName")]
        [StringLength(25, MinimumLength = 1, ErrorMessage = "Employee group name cannot be longer than 25 characters.")]
        public string EmployeeGroupName { get; set; }

        [Required(ErrorMessage = "Please Select Company")]
        [Display(Name = "CompanyId")]
        // Specify Range Validation for dropdwon 
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Company")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "Please Select Branch")]
        [Display(Name = "BranchId")]
        // Specify Range Validation for dropdwon 
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Branch")]
        public int BranchId { get; set; }

        // Delete status
        public int? MarkedAsDelete { get; set; }  // Added by Rajas on 29 MARCH 2017
    }

    /// <summary>
    /// Added for grade master validation
    /// </summary>
    public class GradeModel
    {
        public int GradeId { get; set; }

        [Required(ErrorMessage = "Please Enter Grade Name")]
        [Display(Name = "GradeName")]
        [StringLength(25, MinimumLength = 1, ErrorMessage = "Grade name cannot be longer than 25 characters.")]
        public string GradeName { get; set; }

        [Required(ErrorMessage = "Please Select Employee Type")]
        [Display(Name = "EmployeeTypeId")]
        // Specify Range Validation for dropdwon 
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Employee Type")]
        public int EmployeeTypeId { get; set; }

        [Required(ErrorMessage = "Please Select Branch")]
        [Display(Name = "BranchId")]
        // Specify Range Validation for dropdwon 
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Branch")]
        public int BranchId { get; set; }

        [Required(ErrorMessage = "Please Select Company")]
        [Display(Name = "CompanyId")]
        // Specify Range Validation for dropdwon 
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Company")]
        public int CompanyId { get; set; }

        // Delete status
        public int? MarkedAsDelete { get; set; }  // Added by Rajas on 29 MARCH 2017
    }

    /// <summary>
    /// Added for Employee type master validation
    /// </summary>
    public class EmployeeTypeModel
    {
        public int EmployeeTypeId { get; set; }

        [Required(ErrorMessage = "Please Enter Employee Type Name")]
        [Display(Name = "EmployeeTypeName")]
        [StringLength(25, MinimumLength = 1, ErrorMessage = "Employee type cannot be longer than 25 characters.")]
        public string EmployeeTypeName { get; set; }

        // Delete status
        public int? MarkedAsDelete { get; set; }  // Added by Rajas on 29 MARCH 2017

    }

    #endregion
    // ADDED BY RAJAS ON 29 DEC 2016 FOR VALIDATE DATA THROUGH MODEL END


    #region New Models added on 24 FEB 2017 by Rajas

    /// <summary>
    /// Added by Rajas on 24 FEB 2017
    /// LeaveCreditModel
    /// </summary>

    public class LeaveCreditModel
    {
        public int LeaveCreditId { get; set; }

        public int EmployeeId { get; set; }

        public int FinancialYearId { get; set; }

        public string LeaveType { get; set; }

        public double? Availed { get; set; }

        public double? CarryForward { get; set; }

        public double? Encash { get; set; }

        public double? OpeningBalance { get; set; }

        public int CompanyId { get; set; }

        public int BranchId { get; set; }

        [Required]
        public DateTime? ApplicableFromDate { get; set; } // ADDED BY SHRADDHA ON 03 JUNE 2017

    }

    /// <summary>
    /// LeaveBalanceModel
    /// </summary>
    public class LeaveBalanceModel
    {

        public int LeaveBalanceId { get; set; }

        public int EmployeeId { get; set; }

        public string LeaveType { get; set; }

        public double? PreviousBalance { get; set; }

        public double? CurrentBalance { get; set; }

        public double? LeaveUsed { get; set; }

        public int BranchId { get; set; }

        public int CompanyId { get; set; }

    }

    /// <summary>
    /// FinancialYearModel
    /// </summary>
    public class FinancialYearModel
    {
        public int FinancialId { get; set; }

        public string FinancialName { get; set; }

        public DateTime StartDate { get; set; }


        public DateTime EndDate { get; set; }

        public DateTime? FYStartDate { get; set; }


        public DateTime? FYEndDate { get; set; }

        public int CompanyId { get; set; }

        public int BranchId { get; set; }

    }

    public class RulesMasterModel
    {
        public int RuleId { get; set; }

        public string RuleName { get; set; }

        public string ErrorMessage { get; set; }

        public string HeadCode { get; set; }
    }

    #endregion

    //Added By shraddha on 17th OCT 2016 for model validation
    public class DeclaredHolidayInputModel
    {

        [Display(Name = "Branch")]
        public int Branchid { get; set; }

        public string Description { get; set; }

        public int HoliDayId { get; set; }  // Adde by Rajas on 14 MARCH 2017

        [Display(Name = "Holiday Date")]
        //[DataType(DataType.DateTime)]
        public DateTime HolidayDate { get; set; } //Code added by shraddha on 14 MAR 2017


        [Display(Name = "Extra Working Date Date")]
        public DateTime CompWorkDay1 { get; set; } //Code added by shraddha on 14 MAR 2018

        public DeclaredHolidayInputModel()
        {

            Branchid = 0;

            HolidayDate = DateTime.Now; //Code added by shraddha on 14 MAR 2017

            CompWorkDay1 = DateTime.Now;//Code added by shraddha on 14 MAR 2018

            Description = "";

        }

    }

    /// <summary>
    /// ADDED MODEL BY SHRADDHA ON 19 SEP 2017
    /// </summary>
    public class ManualCompOffModel
    {
        [Required]
        public int ManualCompOffId { get; set; }

        [Required(ErrorMessage = "Employee Name is required.")]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Extra working date is required.")]

        public DateTime FromDate { get; set; }
        [Required(ErrorMessage = "Extra working time is required.")]
        public DateTime ExtraWorkingHrs { get; set; }
        [Required(ErrorMessage = "Comp off purpose is required.")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Purpose cannot be longer than 255 characters.")]
        public string CompOffPurpose { get; set; }
        [Required(ErrorMessage = "Branch name is required.")]
        public int BranchId { get; set; }
        [Required(ErrorMessage = "Comp off status is required.")]
        public int StatusId { get; set; }
        [Required(ErrorMessage = "Comp off calculation is required.")]
        public double CompOffBalance { get; set; }
        [Required(ErrorMessage = "Company name is required.")]
        public int CompanyId { get; set; }
        public string RejectReason { get; set; }

        public int MarkedAsDelete { get; set; }

        // Uncommented by Rajas on 29 SEP 2017 START
        [Required(ErrorMessage = "Login time is required.")]
        public DateTime LoginTime { get; set; }

        [Required(ErrorMessage = "Logout time is required.")]
        public DateTime LogoutTime { get; set; }
        // Uncommented by Rajas on 29 SEP 2017 END

        public bool MySelf { get; set; }

        // Check whether entry is added manually or through system during POSTING
        public int IsAutoEntry { get; set; }  // Added by Rajas on 29 SEP 2017

        //CHECK WHETHER IN PUNCH IS IN SAME DAY OR IN NEXT DAY
        public bool? IsInPunchInNextDay { get; set; } // Added by Shraddha on 29 JAN 2018


        //CHECK WHETHER IN PUNCH IS IN SAME DAY OR IN NEXT DAY
        public bool? IsOutPunchInNextDay { get; set; } // Added by Shraddha on 29 JAN 2018

        public DateTime? EffectiveDate { get; set; } // Added by Shraddha on 16 MAR 2018

        public string Purpose { get; set; }

        public ManualCompOffModel()
        {
            ManualCompOffId = 0;
            EmployeeId = 0;
            FromDate = DateTime.Now;
            int CurrentYearInt = DateTime.Now.Year; // ADDED BY MSJ ON 02 JAN 2018
            ExtraWorkingHrs = new DateTime(CurrentYearInt, 01, 01, 00, 00, 00);
            CompOffPurpose = "";
            BranchId = 0;
            StatusId = 1;
            CompOffBalance = 0;
            CompanyId = 0;
            MarkedAsDelete = 0;
            MySelf = true;
            IsAutoEntry = 0; // Added by Rajas on 29 SEP 2017

            //FLAGS ADDED BY SHRADDHA ON 29 JAN 2018 START
            IsInPunchInNextDay = false;
            IsOutPunchInNextDay = false;
            //FLAGS ADDED BY SHRADDHA ON 29 JAN 2018 END
        }
    }

    public class ProductModel
    {
        public int ProductId { get; set; } // [int] IDENTITY(1,1) NOT NULL,
        public string ProductKey { get; set; } // [nvarchar](50) NULL,
        public string Description { get; set; } // [nvarchar](max) NULL,
        public string CurrentVersion { get; set; } //[nvarchar](50) NULL,
        public DateTime ReleaseDate { get; set; } // [datetime] NULL,
        public int? ValidityDate { get; set; } // //] [int] NULL,
        public int? Status { get; set; } //] [int] NULL,
    }

    //ADDED BY NANDINI ON 14 APRIL 2020
    public class DropdownDataModel
    {
        public int DropdownId { get; set; }
        [Required]
        public int TypeId { get; set; }

        [Required(ErrorMessage = "Please Enter Type Name")]
        public string TypeName { get; set; }

        [Required(ErrorMessage = "Please Enter Text Name")]
        public string TextName { get; set; }
        [Required]
        public int Value { get; set; }
        public int TypeNameID { get; set; }  //ADDED BY NANDINI ON 02 MAY 2020
    }

    //ADDED BY NANDINI ON 14 aAPRIL 2020
    public class NavMenuModel
    {
        public int NavMenuId { get; set; }
        [Required]
        public int ParId { get; set; }

        [Required(ErrorMessage = "Please Enter Nav Menu Name")]
        public string NavMenuName { get; set; }

        [Required(ErrorMessage = "Please Enter Nav Link")]
        public string NavLink { get; set; }
        [Required]
        public int SrNo { get; set; }
        [Required(ErrorMessage = "Please Enter Nav Menu Icon")]
        public int NavMenuIcon { get; set; }

        [Required(ErrorMessage = "Please Enter Menu Name")]
        public int MenuNameId { get; set; }

        public int MasterId { get; set; }   //ADDED BY NANDINI ON 02 MAY 2020
        //[Required]
        public int AttendanceId { get; set; }  //ADDED BY NANDINI ON 02 MAY 2020
        public int PayrollId { get; set; }  //ADDED BY NANDINI ON 02 MAY 2020
        public int ApplicationId { get; set; }  //ADDED BY NANDINI ON 02 MAY 2020
        public int SanctionId { get; set; }  //ADDED BY NANDINI ON 02 MAY 2020
        public int ReportsId { get; set; }  //ADDED BY NANDINI ON 02 MAY 2020
        public int AdministrationId { get; set; }  //ADDED BY NANDINI ON 02 MAY 2020
        public int EmpMgmtId { get; set; }  //ADDED BY NANDINI ON 02 MAY 2020
        public int OtherAppId { get; set; }  //ADDED BY NANDINI ON 02 MAY 2020
        public int UpdatesId { get; set; }  //ADDED BY NANDINI ON 02 MAY 2020
        public int ChartsId { get; set; }  //ADDED BY NANDINI ON 02 MAY 2020
        public int WetosReportsId { get; set; }  //ADDED BY NANDINI ON 02 MAY 2020
    }

    //ADDED BY NANDINI ON 17 APRIL 2020
    public class SqlQueryModel
    {
        public int SqlQueryId { get; set; }
        public string SqlQueryName { get; set; }
        public string SqlQueryDescription { get; set; }
        public string SqlQuery { get; set; }
        public int Status { get; set; }
        public int Deleted { get; set; }
        public List<SqlQueryDetailsModel> SQLQueryDetList { get; set; }
        public SqlQueryModel()
        {
            if (SQLQueryDetList == null)
            {

                SQLQueryDetList = new List<SqlQueryDetailsModel>();
            }
        }

    }

    public class SqlQueryDetailsModel
    {
        public int SqlListId { get; set; }
    }
}










