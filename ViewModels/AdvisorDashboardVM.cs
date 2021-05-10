using System.Collections.Generic;
using TheCCPConnection.Models;

namespace TheCCPConnection.ViewModels
{
    public class AdvisorDashboardVM : DashboardVM
    {
        public List<Advisor> Advisors { get; set; }

        public List<AdvisorSchool> AdvisorSchools { get; set; }

        public List<Student> Students { get; set; }

        public List<StudentSchool> StudentSchools { get; set; }
    }
}