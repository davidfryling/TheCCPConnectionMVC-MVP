using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TheCCPConnection.Models
{
    public class ParentPIN
    {
        [Required(ErrorMessage = "Please enter PIN")]
        [Display(Name = "Parent PIN")]
        public int ID { get; set; } // ID starts at 1000

        public Guid ParentID { get; set; }
        public virtual Parent Parent { get; set; }
    }
}