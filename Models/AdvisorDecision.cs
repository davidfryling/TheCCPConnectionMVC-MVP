using System;
using System.ComponentModel.DataAnnotations;

namespace TheCCPConnection.Models
{
    public class AdvisorDecision
    {
        public int ID { get; set; }

        [Display(Name = "Advisor Decision Entered At")]
        public DateTime Timestamp { get; set; }

        public bool Registered { get; set; }

        [Display(Name = "Reason for Advisor Decision")]
        public string Reason { get; set; }

        public Guid AdvisorID { get; set; }
        public virtual Advisor Advisor { get; set; }

        public int RequestID { get; set; }
        public virtual Request Request { get; set; }
    }
}