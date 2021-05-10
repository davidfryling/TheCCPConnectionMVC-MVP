using System;
using System.ComponentModel.DataAnnotations;

namespace TheCCPConnection.Models
{
    public enum RequestType
    {
        Add,
        Drop
    }
    
    public class Request
    {
        public int ID { get; set; }

        [Display(Name = "Course Request Created At")]
        public DateTime Timestamp { get; set; }
        
        public Guid StudentID { get; set; }
        public virtual Student Student { get; set; }

        public int CourseID { get; set; }
        public virtual Course Course { get; set; }

        public RequestType Type { get; set; }

        public bool Active { get; set; }
    }
}