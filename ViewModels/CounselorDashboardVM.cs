using System.Collections.Generic;
using TheCCPConnection.Models;

namespace TheCCPConnection.ViewModels
{
    public class CounselorDashboardVM : DashboardVM
    {
        public List<Counselor> Counselors { get; set; }

        public List<CounselorSchool> CounselorSchools { get; set; }

        public List<Student> Students { get; set; }

        public List<StudentSchool> StudentSchools { get; set; }
    }
}