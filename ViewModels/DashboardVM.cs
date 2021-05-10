using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TheCCPConnection.Models;

namespace TheCCPConnection.ViewModels
{
    public abstract class DashboardVM
    {
        public List<Request> Requests { get; set; }

        [Display(Name = "Course Information")]
        public List<Course> Courses { get; set; }

        [Display(Name = "Parent Decision")]
        public List<ParentDecision> ParentDecisions { get; set; }

        [Display(Name = "Counselor Decision")]
        public List<CounselorDecision> CounselorDecisions { get; set; }

        [Display(Name = "Advisor Decision")]
        public List<AdvisorDecision> AdvisorDecisions { get; set; }

        [Display(Name = "Advisor Decision")]
        public List<StudentCancel> StudentCancels { get; set; }
    }
}