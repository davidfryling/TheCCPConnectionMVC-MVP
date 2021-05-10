using System;

namespace TheCCPConnection.Models
{
    public class CounselorSchool
    {
        public Guid CounselorID { get; set; }
        public virtual Counselor Counselor { get; set; }

        public Guid SchoolID { get; set; }
        public virtual School School { get; set; }
    }
}