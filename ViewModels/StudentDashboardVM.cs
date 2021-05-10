using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TheCCPConnection.Models;

namespace TheCCPConnection.ViewModels
{
    public class StudentDashboardVM : DashboardVM
    {
        [Display(Name = "Your Name")]
        public List<Student> Students { get; set; }

        [Display(Name = "Your Parent's Name")]
        public List<Parent> Parents { get; set; }
    }
}