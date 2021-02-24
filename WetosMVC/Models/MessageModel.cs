using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WetosMVCMainApp.Models
{
    public class MessageModel
    {
        [Required]
        [Display(Name = "From")]
        public int FromEmpId { get; set; }

        [Required]
        
        public int ToEmpId { get; set; }

       
        public int ToEmpTypeId { get; set; }

        [Required]
        [Display(Name = "Subject")]
        public int MessageSubject { get; set; }

        [Required]
        [Display(Name = "Content")]
        public string MessageContent { get; set; }
    }
}