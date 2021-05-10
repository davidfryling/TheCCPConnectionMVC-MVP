using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TheCCPConnection.Models
{
    public class Counselor : Decider
    {
        [Display(Name = "Schools")]
        public virtual ICollection<CounselorSchool> CounselorSchools { get; set; }
    }
}