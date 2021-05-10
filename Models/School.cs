using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheCCPConnection.Models
{
    public enum SchoolType
    {
        HighSchool,
        College
    }

    public class School
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid ID { get; set; }

        [Display(Name = "School Name")]
        public string Name { get; set; }

        public SchoolType Type { get; set; }

        public virtual ICollection<AdvisorSchool> AdvisorSchools { get; set; }
        public virtual ICollection<CounselorSchool> CounselorSchools { get; set; }
        public virtual ICollection<StudentSchool> StudentSchools { get; set; }
    }
}