using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WetosMVC.Models
{
    public class ManPowerRequirementFormModel
    {
        public int MRFId { get; set; }

        [Required]
        public string MRFPosition { get; set; }

        [Required]
        public int MRFDepartmentId { get; set; }

        [Required]
        public int MRFLocation { get; set; }

        [Required]
        public string MRFExperience { get; set; }

        [Required]
        public string SpecificQualification { get; set; }

        [Required]
        public string CompansationRange { get; set; }


        [Required]
        public string ReportingTo { get; set; }

        [Required]
        public string RequirementType { get; set; }

        [Required]
        public string IfReplacement { get; set; }

        [Required]
        public string IfNewVacancy { get; set; }

        [Required]
        public string PreferredIndustry { get; set; }

        [Required]
        public string EssentialSkillsRequired { get; set; }

        [Required]
        public string PerformanceChallenges { get; set; }

        [Required]
        public string CompanyCultureCode { get; set; }

        [Required]
        public string Sign { get; set; }

        [Required]
        public string DesignationHOD { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string RemarkOnApproval { get; set; }
    }
}