using System;

namespace TheCCPConnection.Models
{
    public class StudentSchool
    {
        public Guid StudentID { get; set; }
        public virtual Student Student { get; set; }

        public Guid SchoolID { get; set; }
        public virtual School School { get; set; }
    }
}