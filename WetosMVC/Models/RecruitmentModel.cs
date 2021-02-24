using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WetosMVC.Models
{
    public class RecruitmentModel
    {
        public class CandidateModel
        {
            public int CandidateId { get; set; }

            [Required]
            public string CandidateNo { get; set; }

            [Required]
            public string CandidateName { get; set; }

            [Required]
            public string CandidateAddress { get; set; }

            [Required]
            public string CandidateEmail { get; set; }

            [Required]
            public string CandidateMobileNo { get; set; }

            [Required]
            public string CandidateApplyPost { get; set; }

            [Required]
            public string CandidateQualification { get; set; }

            [Required]
            public string CandidateResumePath { get; set; }

            [Required]
            public string CandidateStatus { get; set; }

            [Required]
            public int AnnualExpectedAmount { get; set; }

            [Required]
            public DateTime? CandiadateDOB { get; set; }

        }
        public class OnBoardCandidateModel
        {
            public int OnBoardingId { get; set; }

            public int CandidadateId { get; set; }

            [Required]
            public int JoiningProcessId { get; set; }

            [Required]
            public DateTime? JoinedOn { get; set; }

            [Required]
            public string JoinedLocation { get; set; }

            [Required]
            public int JoinedToGroup { get; set; }

            [Required]
            public string OfficialEmailId { get; set; }

            [Required]
            public string OffcialMobileNo { get; set; }

            [Required]
            public string MobileIMEAno { get; set; }

            [Required]
            public string LaptopSrNo { get; set; }

            [Required]
            public string LaptopDetails { get; set; }

            [Required]
            public string WorkLocation { get; set; }
        

        }

        public class EmploymentOfferModel
        {
            public int EmploymentOfferId { get; set; }

            public int SalaryStructureId { get; set; }
            [Required]
            public int CTC { get; set; }

            [Required]
            public int OfferLetterGenerated { get; set; }
            [Required]
            public DateTime? reatedBy { get; set; }

            [Required]
            public string JoinedLocation { get; set; }

            [Required]
            public int JoinedToGroup { get; set; }

            [Required]
            public string OfficialEmailId { get; set; }

            [Required]
            public string OffcialMobileNo { get; set; }

            [Required]
            public string MobileIMEAno { get; set; }

            [Required]
            public string LaptopSrNo { get; set; }

            [Required]
            public string LaptopDetails { get; set; }

            [Required]
            public string WorkLocation { get; set; }


        }
        public class InterviewScheduleModel
        {
            public int InterviewScheduleId { get; set; }

            public int CandidateId { get; set; }

            [Required]
            public DateTime InterviewDateTime { get; set; }

            [Required]
            public string InterviewType { get; set; }

            [Required]
            public int InterviewConfirmation { get; set; }

            [Required]
            public int IsWrittenTest { get; set; }

            [Required]
            public int IsHR { get; set; }

            [Required]
            public int IsTechnical { get; set; }

            [Required]
            public int IsFinal { get; set; }

            [Required]
            public int CommunicationStatus { get; set; }

            [Required]
            public int CommunicationMode { get; set; }

            [Required]
            public DateTime CommunicatedOn { get; set; }

            [Required]
            public int CreatedBy { get; set; }

            [Required]
            public DateTime CreatedOn { get; set; }

            [Required]
            public int LastModifiedBy { get; set; }

            [Required]
            public DateTime LastModifiedOn { get; set; }

            [Required]
            public string IsInterviewAttended { get; set; }

            [Required]
            public int InterviewStatus { get; set; }

            [Required]
            public string CandidateEvaluationFormPath { get; set; }



        }
        public class JoiningProcessModel
        {
            public int JoiningProcessId { get; set; }

            public int EmployeeOfferId { get; set; }
            [Required]
            public int CreatedBy { get; set; }

            [Required]
            public DateTime CreatedOn { get; set; }

            [Required]
            public int LastModifiedBy { get; set; }

            [Required]
            public DateTime LastModifiedOn { get; set; }

            [Required]
            public string OfferSignedDocPath { get; set; }

            [Required]
            public string AdharCardNo { get; set; }

            [Required]
            public string PanCardNo { get; set; }

            [Required]
            public string ResidanceProof { get; set; }

            [Required]
            public string BackgroundCheckStatus { get; set; }

            [Required]
            public string MedicalReport { get; set; }

            [Required]
            public string AdharCardDocPath { get; set; }

            [Required]
            public string PanCardDocPath { get; set; }

            [Required]
            public string ResidanceProofDocPath { get; set; }

            [Required]
            public string BackgroundCheckDocPath { get; set; }

            [Required]
            public string MedicalReportDocPath { get; set; }

            [Required]
            public DateTime JoinedOn { get; set; }

            [Required]
            public int JoiningStatus { get; set; }

            [Required]
            public string JoiningLocation { get; set; }
       
        }
    }
}