using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using TheCCPConnection.Models;

namespace TheCCPConnection.ViewModels
{
    public class AdminDashboardVM
    {
        public List<IdentityUser> CounselorUsers { get; set; }
        
        public List<IdentityUser> AdvisorUsers { get; set; }

        public List<Counselor> Counselors { get; set; }

        public List<Advisor> Advisors { get; set; }

        public List<School> Schools { get; set; }
    }
}