using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TheCCPConnection.Models
{
    public class Advisor : Decider
    {
        [Display(Name = "Schools")]
        public virtual ICollection<AdvisorSchool> AdvisorSchools { get; set; }
    }
}