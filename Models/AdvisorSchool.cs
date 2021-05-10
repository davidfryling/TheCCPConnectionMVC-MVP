using System;

namespace TheCCPConnection.Models
{
    public class AdvisorSchool
    {
        public Guid AdvisorID { get; set; }
        public virtual Advisor Advisor { get; set; }

        public Guid SchoolID { get; set; }
        public virtual School School { get; set; }
    }
}