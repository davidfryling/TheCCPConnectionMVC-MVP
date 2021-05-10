using System;
using System.ComponentModel.DataAnnotations;

namespace TheCCPConnection.Models
{
    public class StudentCancel
    {
        public int ID { get; set; }

        [Display(Name = "Student Cancelled Request At")]
        public DateTime Timestamp { get; set; }

        [Display(Name = "Reason for Student Cancelling")]
        public string Reason { get; set; }

        public Guid? StudentID { get; set; }
        public virtual Student Student { get; set; }

        public int RequestID { get; set; }
        public virtual Request Request { get; set; }
    }
}