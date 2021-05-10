using System;
using System.ComponentModel.DataAnnotations;

namespace TheCCPConnection.Models
{
    public class ParentDecision
    {
        public int ID { get; set; }

        [Display(Name = "Parent Decision Entered At")]
        public DateTime Timestamp { get; set; }

        public bool Approved { get; set; }

        [Display(Name = "Reason for Parent Decision")]
        public string Reason { get; set; }

        public Guid ParentID { get; set; }
        public virtual Parent Parent { get; set; }

        public int RequestID { get; set; }
        public virtual Request Request { get; set; }
    }
}