using System;
using System.ComponentModel.DataAnnotations;

namespace TheCCPConnection.Models
{
    public class CounselorDecision
    {
        public int ID { get; set; }

        [Display(Name = "Counselor Decision Entered At")]
        public DateTime Timestamp { get; set; }

        public bool Approved { get; set; }

        [Display(Name = "Reason for Counselor Decision")]
        public string Reason { get; set; }

        public Guid CounselorID { get; set; }
        public virtual Counselor Counselor { get; set; }

        public int RequestID { get; set; }
        public virtual Request Request { get; set; }
    }
}